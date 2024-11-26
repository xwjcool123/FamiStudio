﻿using SharpDX;
using SharpDX.Multimedia;
using SharpDX.XAudio2;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace FamiStudio
{
    public class XAudio2Stream : IAudioStream
    {
        private XAudio2 xaudio2;
        private MasteringVoice masteringVoice;

        // Regular voice for streaming NES audio data.
        private WaveFormat waveFormat;
        private SourceVoice sourceVoice;
        private AudioBuffer[] audioBuffersRing;
        private DataPointer[] memBuffers;
        private Semaphore bufferSemaphore;
        private ManualResetEvent quitEvent;
        private GetBufferDataCallback bufferFill;
        private StreamStartingCallback streamStarting;
        private Task playingTask;
        private int bufferPrefillCount;
        private int queuedBufferCount;

        // Immediate voice for playing raw PCM data (used by DPCM editor).
        private SourceVoice immediateVoice;
        private AudioBuffer immediateAudioBuffer;
        private bool immediateDonePlaying;

        public static XAudio2Stream Create(int rate, bool stereo, int bufferSizeMs)
        {
            var xaudio2Stream = new XAudio2Stream();

            xaudio2Stream.xaudio2 = new XAudio2();
            xaudio2Stream.masteringVoice = new MasteringVoice(xaudio2Stream.xaudio2, stereo ? 2 : 1);
            xaudio2Stream.masteringVoice.GetVoiceDetails(out var details);
            xaudio2Stream.waveFormat = new WaveFormat(rate, 16, stereo ? 2 : 1);

            var numBuffers = Math.Max(4, bufferSizeMs / 25);
            var bufferSizeBytes = Utils.RoundUp(rate * bufferSizeMs / 1000 * sizeof(short) * (stereo ? 2 : 1) / numBuffers, sizeof(short) * 2);

            xaudio2Stream.audioBuffersRing = new AudioBuffer[numBuffers];
            xaudio2Stream.memBuffers = new DataPointer[xaudio2Stream.audioBuffersRing.Length];

            for (int i = 0; i < xaudio2Stream.audioBuffersRing.Length; i++)
            {
                xaudio2Stream.audioBuffersRing[i] = new AudioBuffer();
                xaudio2Stream.memBuffers[i].Size = bufferSizeBytes;
                xaudio2Stream.memBuffers[i].Pointer = Utilities.AllocateMemory(xaudio2Stream.memBuffers[i].Size);
            }

            xaudio2Stream.quitEvent = new ManualResetEvent(false);

            return xaudio2Stream;
        }

        private void Xaudio2_CriticalError(object sender, ErrorEventArgs e)
        {
            Debug.WriteLine("CRITICAL ERROR!");
        }

        private void SourceVoice_BufferEnd(IntPtr obj)
        {
            bufferSemaphore.Release();
            Interlocked.Decrement(ref queuedBufferCount);
        }

        public bool IsPlaying => sourceVoice != null;
        public bool RecreateOnDeviceChanged => false;
        public bool Stereo => waveFormat.Channels == 2;

        public void Start(GetBufferDataCallback bufferFillCallback, StreamStartingCallback streamStartCallback)
        {
            Debug.Assert(sourceVoice == null);
            Debug.Assert(bufferSemaphore == null);

            bufferFill = bufferFillCallback;
            streamStarting = streamStartCallback;

            queuedBufferCount = 0;
            bufferSemaphore = new Semaphore(audioBuffersRing.Length, audioBuffersRing.Length);
            bufferPrefillCount = audioBuffersRing.Length;

            sourceVoice = new SourceVoice(xaudio2, waveFormat);
            sourceVoice.BufferEnd += SourceVoice_BufferEnd;

            quitEvent.Reset();

            playingTask = Task.Factory.StartNew(PlayAsync, TaskCreationOptions.LongRunning);
        }

        public void Stop()
        {
            StopInternal(true);
        }

        private void StopInternal(bool mainThread)
        {
            Debug.Assert(Platform.IsInMainThread() == mainThread);

            var acquired = false;

            // If we are on the playing task thread and fail to acquire the lock
            // it means the main thread is here stopping, so we must exist so that
            // the main thread doesnt deadlock on "playingTask.Wait()".
            if (mainThread)
            {
                Monitor.Enter(this);
                acquired = true;
            }
            else
            {
                Monitor.TryEnter(this, ref acquired);
            }

            if (acquired)
            {
                if (mainThread)
                {
                    StopImmediate();

                    if (playingTask != null)
                    {
                        quitEvent.Set();
                        playingTask.Wait();
                    }
                }

                playingTask = null;

                if (sourceVoice != null)
                {
                    sourceVoice.Stop();
                    sourceVoice.FlushSourceBuffers();
                    sourceVoice.DestroyVoice();
                    sourceVoice.BufferEnd -= SourceVoice_BufferEnd;
                    sourceVoice.Dispose();
                    sourceVoice = null;
                }

                if (bufferSemaphore != null)
                {
                    bufferSemaphore.Dispose();
                    bufferSemaphore = null;
                }

                bufferFill = null;
                streamStarting = null;

                Monitor.Exit(this);
            }
        }

        public void Dispose()
        {
            Stop();

            for (int i = 0; i < audioBuffersRing.Length; i++)
            {
                Utilities.FreeMemory(memBuffers[i].Pointer);
                memBuffers[i].Pointer = IntPtr.Zero;
            }

            masteringVoice.DestroyVoice();
            masteringVoice.Dispose();
            masteringVoice = null;

            xaudio2.Dispose();
            xaudio2 = null;
        }

        private void PlayAsync()
        {
#if DEBUG
            try
            {
#endif
                var bufferIndex = 0;
                var waitEvents = new WaitHandle[] { quitEvent, bufferSemaphore };
                var done = false;
                var samples = (short[])null;
                var samplesOffset = 0;

                while (!done)
                {
                    int idx = WaitHandle.WaitAny(waitEvents);

                    if (idx == 0)
                    {
                        break;
                    }

                    var buffer = memBuffers[bufferIndex];
                    var bufferSampleCount = buffer.Size / sizeof(short);
                    var bufferPtr = buffer.Pointer;

                    do
                    {
                        if (samplesOffset == 0)
                        {
                            while (true)
                            {
                                var newSamples = bufferFill(out done);
                                
                                // If we are done, pad the last buffer with zeroes so the stream can start
                                // for super-short (ex: 1-frame) non-looping songs.
                                if (done)
                                    newSamples = new short[bufferSampleCount - samplesOffset];

                                if (newSamples != null)
                                {
                                    samples = newSamples;
                                    break;
                                }
                                
                                if (quitEvent.WaitOne(0))
                                    return;
                                
                                Thread.Sleep(1);
                            }
                        }

                        var numSamplesToCopy = Math.Min(bufferSampleCount, samples.Length - samplesOffset);
                        Marshal.Copy(samples, samplesOffset, bufferPtr, numSamplesToCopy);

                        samplesOffset += numSamplesToCopy;
                        if (samplesOffset == samples.Length)
                            samplesOffset = 0;

                        bufferPtr = IntPtr.Add(bufferPtr, numSamplesToCopy * sizeof(short));
                        bufferSampleCount = bufferSampleCount - numSamplesToCopy;
                    }
                    while (bufferSampleCount != 0);

                    audioBuffersRing[bufferIndex].AudioDataPointer = buffer.Pointer;
                    audioBuffersRing[bufferIndex].AudioBytes = buffer.Size;
                    audioBuffersRing[bufferIndex].Flags = done ? BufferFlags.EndOfStream : BufferFlags.None;

                    Interlocked.Increment(ref queuedBufferCount);
                    sourceVoice.SubmitSourceBuffer(audioBuffersRing[bufferIndex], null);

                    // Only start the stream once the buffers are pre-filled.
                    if ((bufferPrefillCount > 0 && --bufferPrefillCount == 0) || done)
                    {
                        if (!done)
                            streamStarting();

                        sourceVoice.Start();
                    }

                    bufferIndex = ++bufferIndex % audioBuffersRing.Length;
                }
#if DEBUG
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                if (Debugger.IsAttached)
                    Debugger.Break();
            }
#endif

            // Wait for all buffers to be consumed.
            while (queuedBufferCount != 0) 
                Thread.Sleep(1);

            StopInternal(false);
        }

        public void PlayImmediate(short[] data, int sampleRate, float volume, int channel = 0)
        {
            Debug.Assert(Platform.IsInMainThread());

            StopImmediate();

            immediateDonePlaying = false;

            immediateAudioBuffer = new AudioBuffer();
            immediateAudioBuffer.AudioDataPointer = Utilities.AllocateMemory(data.Length * sizeof(short));
            immediateAudioBuffer.AudioBytes = data.Length * sizeof(short);
            Marshal.Copy(data, 0, immediateAudioBuffer.AudioDataPointer, data.Length);

            var waveFormat = new WaveFormat(sampleRate, 16, 1);

            immediateVoice = new SourceVoice(xaudio2, waveFormat);
            immediateVoice.BufferEnd += ImmediateVoice_BufferEnd;
            immediateVoice.SetVolume(volume);
            immediateVoice.SubmitSourceBuffer(immediateAudioBuffer, null);
            immediateVoice.Start();
        }

        public void StopImmediate()
        {
            Debug.Assert(Platform.IsInMainThread());

            if (immediateVoice != null)
            {
                immediateVoice.Stop();
                immediateVoice.FlushSourceBuffers();
                immediateVoice.BufferEnd -= ImmediateVoice_BufferEnd;
                immediateVoice.DestroyVoice();
                immediateVoice.Dispose();
                immediateVoice = null;

                Utilities.FreeMemory(immediateAudioBuffer.AudioDataPointer);
                immediateAudioBuffer = null;
                immediateDonePlaying = true;
            }
        }

        private void ImmediateVoice_BufferEnd(IntPtr obj)
        {
            immediateDonePlaying = true;
        }

        public int ImmediatePlayPosition
        {
            get
            {
                if (immediateVoice != null && !immediateDonePlaying)
                {
                    return (int)immediateVoice.State.SamplesPlayed;
                }
                else
                {
                    return -1;
                }
            }
        }

        public static bool TryDetectXAudio2()
        {
            try
            {
                var xaudio2 = new XAudio2();

                xaudio2.Dispose();
                xaudio2 = null;

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
