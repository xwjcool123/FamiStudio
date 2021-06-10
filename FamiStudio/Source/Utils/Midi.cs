﻿using System;
using System.Runtime.InteropServices;

namespace FamiStudio
{
    public class Midi
    {
        internal const int MMSYSERR_NOERROR = 0;
        internal const int CALLBACK_FUNCTION = 0x00030000;

        internal const int MM_MIM_OPEN      = 0x3C1;
        internal const int MM_MIM_CLOSE     = 0x3C2;
        internal const int MM_MIM_DATA      = 0x3C3;
        internal const int MM_MIM_LONGDATA  = 0x3C4;
        internal const int MM_MIM_ERROR     = 0x3C5;
        internal const int MM_MIM_LONGERROR = 0x3C6;

        internal const int STATUS_NOTE_ON   = 0x90;
        internal const int STATUS_NOTE_OFF  = 0x80;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal unsafe struct MIDIINCAPS
        {
            public ushort wMid;
            public ushort wPid;
            public uint vDriverVersion;
            public fixed char szPname[32];
            public uint dwSupport;
        };

        internal delegate void MidiInProc(IntPtr hMidiIn, int wMsg, IntPtr dwInstance, int dwParam1, int dwParam2);
        [DllImport("winmm.dll")]
        internal static extern int midiInGetNumDevs();
        [DllImport("winmm.dll")]
        internal static extern int midiInClose(IntPtr hMidiIn);
        [DllImport("winmm.dll")]
        internal static extern int midiInOpen(out IntPtr lphMidiIn, int uDeviceID, MidiInProc dwCallback, IntPtr dwCallbackInstance, int dwFlags);
        [DllImport("winmm.dll")]
        internal static extern int midiInStart(IntPtr hMidiIn);
        [DllImport("winmm.dll")]
        internal static extern int midiInStop(IntPtr hMidiIn);
        [DllImport("winmm.dll", EntryPoint = "midiInGetDevCapsW")]
        internal static extern int midiInGetDevCaps(uint uDeviceID, IntPtr pmic, uint cbmic);

        public delegate void MidiNoteDelegate(int note, bool on);
        public static event MidiNoteDelegate NotePlayed;

        private static MidiInProc midiInProc;
        private static IntPtr handle;

        public static void Initialize()
        {
            if (midiInProc == null)
            {
                midiInProc = new MidiInProc(MidiProc);
                handle = IntPtr.Zero;
            }
        }

        public static void Shutdown()
        {
            if (midiInProc != null)
            {
                Close();
                midiInProc = null;
            }
        }

        public static int InputCount
        {
            get
            {
                return midiInGetNumDevs();
            }
        }

        public static unsafe string GetDeviceName(int idx)
        {
            MIDIINCAPS caps = new MIDIINCAPS();
            midiInGetDevCaps((uint)idx, new IntPtr(&caps), (uint)Marshal.SizeOf<MIDIINCAPS>());
            return new string(caps.szPname);
        }

        public static bool Open(int id)
        {
            if (midiInOpen(out handle, id, midiInProc, IntPtr.Zero, CALLBACK_FUNCTION) != MMSYSERR_NOERROR)
                return false;

            return midiInStart(handle) == MMSYSERR_NOERROR;
        }

        public static void Close()
        {
            if (handle != IntPtr.Zero)
            {
                midiInStop(handle);
                midiInClose(handle);
                handle = IntPtr.Zero;
            }
        }

        private static void MidiProc(IntPtr hMidiIn, int wMsg, IntPtr dwInstance, int dwParam1, int dwParam2)
        {
            if (wMsg == MM_MIM_DATA)
            {
                int velocity = (dwParam1 >> 16) & 0xff;
                int note = (dwParam1 >> 8) & 0xff;
                int status = dwParam1 & 0xf0;

                if (status == STATUS_NOTE_OFF || (status == STATUS_NOTE_ON && velocity == 0))
                {
                    NotePlayed?.Invoke(note, false);
                }
                else if (status == STATUS_NOTE_ON)
                {
                    NotePlayed?.Invoke(note, true);
                }
            }
        }
    }
}
