﻿using System;
using System.Diagnostics;

namespace FamiStudio
{
    public class ChannelStateEPSMSquare : ChannelState
    {
        private int  channelIdx = 0;
        private int  invToneMask;

        // Last channel will upload these.       
        private int  toneReg = 0x38;
        private int  envPeriod;
        private int  envShape;
        private bool envPeriodEffect;
        private bool envAutoPitch;
        private int  envAutoOctave;
        private bool envReset;
        private int  noiseFreq;

        // From instrument.
        private bool instAutoPitch;
        private int  instAutoOctave;
        private int  instEnvShape;
        private int  instEnvPeriod;

        public ChannelStateEPSMSquare(IPlayerInterface player, int apuIdx, int channelType, bool pal) : base(player, apuIdx, channelType, pal)
        {
            channelIdx = channelType - ChannelType.EPSMSquare1;
            invToneMask = 0xff - (9 << channelIdx);
        }

        protected override void LoadInstrument(Instrument instrument)
        {
            if (instrument != null)
            {
                Debug.Assert(instrument.IsEpsm);

                if (instrument.IsEpsm)
                {
                    if (instrument.EPSMSquareEnvelopeShape > 0)
                    {
                        instEnvShape   = (byte)(instrument.EPSMSquareEnvelopeShape + 7); // 1...8 maps to 0x8...0xf
                        instAutoPitch  = instrument.EPSMSquareEnvAutoPitch;
                        instAutoOctave = instrument.EPSMSquareEnvAutoPitchOctave;
                        instEnvPeriod  = instrument.EPSMSquareEnvelopePitch;
                    }
                    else
                    {
                        instEnvShape = 0;
                    }
                }
            }
        }

        public override void UpdateAPU()
        {
            var lastChannel = player.GetChannelByType(instrumentPlayer ? InnerChannelType : ChannelType.EPSMSquare3) as ChannelStateEPSMSquare;
            var firstChannelIndex = instrumentPlayer ? channelIdx : 0;
            var lastChannelIndex  = instrumentPlayer ? channelIdx : 2;

            // All channels will update the channel 3 variables. This is pretty ugly
            // but mimics what the assemble code does pretty closely.
            if (channelIdx == firstChannelIndex)
            {
                lastChannel.envAutoPitch = false;
                lastChannel.envReset = false;
                lastChannel.noiseFreq = 0;
                lastChannel.envPeriodEffect = false;
            }

            lastChannel.envReset |= (noteTriggered && instEnvShape > 0);

            if (note.HasEnvelopePeriod)
            {
                lastChannel.envPeriod = note.EnvelopePeriod;
                lastChannel.envPeriodEffect = true;
            }

            if (note.IsStop)
            {
                WriteRegister(NesApu.EPSM_ADDR0, NesApu.EPSM_REG_VOL_A + channelIdx);
                WriteRegister(NesApu.EPSM_DATA0, 0);
            }
            else if (note.IsMusical)
            {
                var period = GetPeriod(); // While the formula for YM2149 does not add 1 to the period, that is baked into the pitch table
                var volume = GetVolume();

                var periodHi = (period >> 8) & 0x0f;
                var periodLo = (period >> 0) & 0xff;

                WriteRegister(NesApu.EPSM_ADDR0, NesApu.EPSM_REG_LO_A + channelIdx * 2);
                WriteRegister(NesApu.EPSM_DATA0, periodLo);
                WriteRegister(NesApu.EPSM_ADDR0, NesApu.EPSM_REG_HI_A + channelIdx * 2);
                WriteRegister(NesApu.EPSM_DATA0, periodHi);
                WriteRegister(NesApu.EPSM_ADDR0, NesApu.EPSM_REG_VOL_A + channelIdx);
                WriteRegister(NesApu.EPSM_DATA0, volume | (instEnvShape != 0 ? 0x10 : 0x00));

                var mixerEnv  = envelopeValues[EnvelopeType.S5BMixer];
                var noiseFreq = envelopeValues[EnvelopeType.S5BNoiseFreq];
                var noiseEnabled = (mixerEnv & 0x2) == 0;
                lastChannel.toneReg = (lastChannel.toneReg & invToneMask) | (((mixerEnv & 1) | ((mixerEnv & 0x2) << 2)) << channelIdx);
                lastChannel.noiseFreq = noiseEnabled ? noiseFreq : lastChannel.noiseFreq;

                if (instEnvShape != 0)
                {
                    lastChannel.envPeriod = instAutoPitch ? period : (lastChannel.envPeriodEffect || !noteTriggered ? lastChannel.envPeriod : instEnvPeriod);
                    lastChannel.envShape = instEnvShape;
                    lastChannel.envAutoOctave = instAutoOctave;
                    lastChannel.envAutoPitch = instAutoPitch;
                }
            }

            // Last channel will be in charge of writing to the shared registers.
            if (channelIdx == lastChannelIndex)
            {
                if (envAutoPitch)
                {
                    if (envAutoOctave > 0)
                    {
                        envPeriod >>= Math.Abs(envAutoOctave) - 1;
                        if ((envPeriod & 1) != 0) envPeriod++;
                        envPeriod >>= 1;
                    }
                    else
                    {
                        envPeriod <<= Math.Abs(envAutoOctave);
                    }
                }

                var envPeriodLo = (envPeriod >> 0) & 0xff;
                var envPeriodHi = (envPeriod >> 8) & 0xff;

                WriteRegister(NesApu.EPSM_ADDR0, NesApu.EPSM_REG_ENV_LO);
                WriteRegister(NesApu.EPSM_DATA0, envPeriodLo);
                WriteRegister(NesApu.EPSM_ADDR0, NesApu.EPSM_REG_ENV_HI);
                WriteRegister(NesApu.EPSM_DATA0, envPeriodHi);
                WriteRegister(NesApu.EPSM_ADDR0, NesApu.EPSM_REG_NOISE_FREQ);
                WriteRegister(NesApu.EPSM_DATA0, noiseFreq);
                WriteRegister(NesApu.EPSM_ADDR0, NesApu.EPSM_REG_MIXER_SETTING);
                WriteRegister(NesApu.EPSM_DATA0, toneReg);

                if (envReset)
                {
                    WriteRegister(NesApu.EPSM_ADDR0, NesApu.EPSM_REG_SHAPE);
                    WriteRegister(NesApu.EPSM_DATA0, envShape);
                }
            }

            base.UpdateAPU();
        }
    };
}
