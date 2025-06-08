using System;
using DaggerfallWorkshop.AudioSynthesis.Synthesis;
using DaggerfallWorkshop.AudioSynthesis.Bank.Components;
using DaggerfallWorkshop.AudioSynthesis.Bank.Components.Generators;
using DaggerfallWorkshop.AudioSynthesis.Bank.Descriptors;

namespace DaggerfallWorkshop.AudioSynthesis.Bank.Patches
{
    /* FM 2 Operator Patch
     *   M --> C --> OUT
     *    
     *    LFO1              LFO1
     *     |                 |
     *    GEN1 --> ENV1 --> GEN2 --> ENV2 --> OUT 
     *         
     * LFO1 : Usually generates vibrato. Responds to the MOD Controller (MIDI Controlled).
     * GEN1 : A generator with a continuous loop type. The Modulator.
     * ENV1 : An envelope controlling the amplitude of GEN1.
     * GEN2 : A generator with a continuous loop type. The Carrier.
     * ENV2 : An envelope controlling the amplitude of GEN2.
     * 
     * Note: GEN 1 & 2 must also wrap mathmatically on its input like Sin() does
     */
    public class Fm2Patch : Patch
    {
        public enum SyncMode { Soft, Hard };
        private SyncMode sync;
        private double mIndex, cIndex, feedBack;
        private Generator cGen, mGen;
        private EnvelopeDescriptor cEnv, mEnv;
        private LfoDescriptor lfo;

        public SyncMode SynchronizationMethod
        {
            get { return sync; }
        }
        public double ModulationIndex
        {
            get { return mIndex; }
        }
        public double CarrierIndex
        {
            get { return cIndex; }
        }

        public Fm2Patch(string name) : base(name) { }
        public override bool Start(VoiceParameters voiceparams)
        {
            //calculate velocity
            float fVel = voiceparams.velocity / 127f;
            //reset counters
            voiceparams.pData[0].double1 = cGen.LoopStartPhase;
            voiceparams.pData[1].double1 = mGen.LoopStartPhase;
            voiceparams.pData[2].double1 = 0.0;
            //reset envelopes
            voiceparams.envelopes[0].QuickSetup(voiceparams.synthParams.synth.SampleRate, fVel, cEnv);
            voiceparams.envelopes[1].QuickSetup(voiceparams.synthParams.synth.SampleRate, fVel, mEnv);
            //reset lfo (vibra)
            voiceparams.lfos[0].QuickSetup(voiceparams.synthParams.synth.SampleRate, lfo);
            //calculate initial pitch
            voiceparams.pitchOffset = (int)(100.0 * (voiceparams.synthParams.masterCoarseTune + (voiceparams.synthParams.masterFineTune.Combined - 8192.0) / 8192.0));
            //calc initial volume
            voiceparams.volOffset = voiceparams.synthParams.volume.Combined / 16383f;
            voiceparams.volOffset *= voiceparams.volOffset * fVel * voiceparams.synthParams.synth.MixGain;
            //check if we have finished before we have begun
            return voiceparams.envelopes[0].CurrentState != EnvelopeStateEnum.None;
        }
        public override void Stop(VoiceParameters voiceparams)
        {
            voiceparams.envelopes[0].Release(Synthesis.Synthesizer.NonAudible);
            voiceparams.envelopes[1].Release(Synthesis.Synthesizer.DenormLimit);
        }
        public override void Process(VoiceParameters voiceparams, int startIndex, int endIndex)
        {
            //--Base pitch calculation
            double carrierPitch = SynthHelper.CentsToPitch((voiceparams.note - cGen.RootKey) * cGen.KeyTrack + cGen.Tune + voiceparams.pitchOffset + voiceparams.synthParams.currentPitch)
                * cGen.Period * cGen.Frequency * cIndex / voiceparams.synthParams.synth.SampleRate;
            double modulatorPitch = SynthHelper.CentsToPitch((voiceparams.note - mGen.RootKey) * mGen.KeyTrack + mGen.Tune + voiceparams.pitchOffset + voiceparams.synthParams.currentPitch)
                * mGen.Period * mGen.Frequency * mIndex / voiceparams.synthParams.synth.SampleRate;
            //--Base volume calculation
            float baseVolume = voiceparams.volOffset * voiceparams.synthParams.currentVolume;
            //--Main Loop
            for (int x = startIndex; x < endIndex; x += Synthesizer.DefaultBlockSize * voiceparams.synthParams.synth.AudioChannels)
            {
                //--Calculate pitch modifications
                double pitchMod;
                if (voiceparams.synthParams.modRange.Combined != 0)
                {
                    voiceparams.lfos[0].Increment(Synthesizer.DefaultBlockSize);
                    pitchMod = SynthHelper.CentsToPitch((int)(voiceparams.lfos[0].Value * voiceparams.synthParams.currentMod));
                }
                else
                {
                    pitchMod = 1;
                }
                //--Get amplitude values for carrier and modulator
                voiceparams.envelopes[0].Increment(Synthesizer.DefaultBlockSize);
                voiceparams.envelopes[1].Increment(Synthesizer.DefaultBlockSize);
                float c_amp = baseVolume * voiceparams.envelopes[0].Value;
                float m_amp = voiceparams.envelopes[1].Value;
                //--Interpolator for modulator amplitude
                float linear_m_amp = (m_amp - voiceparams.pData[3].float1) / Synthesizer.DefaultBlockSize;
                //--Process block
                for (int i = 0; i < voiceparams.blockBuffer.Length; i++)
                {
                    //calculate current modulator amplitude
                    voiceparams.pData[3].float1 += linear_m_amp;
                    //calculate sample
                    voiceparams.blockBuffer[i] = cGen.GetValue(voiceparams.pData[0].double1 + voiceparams.pData[3].float1 * mGen.GetValue(voiceparams.pData[1].double1 + voiceparams.pData[2].double1 * feedBack));
                    //store sample for feedback calculation
                    voiceparams.pData[2].double1 = voiceparams.blockBuffer[i];
                    //increment phase counters
                    voiceparams.pData[0].double1 += carrierPitch * pitchMod;
                    voiceparams.pData[1].double1 += modulatorPitch * pitchMod;
                }
                voiceparams.pData[3].float1 = m_amp;
                //--Mix block based on number of channels
                if (voiceparams.synthParams.synth.AudioChannels == 2)
                    voiceparams.MixMonoToStereoInterp(x,
                        c_amp * voiceparams.synthParams.currentPan.Left,
                        c_amp * voiceparams.synthParams.currentPan.Right);
                else
                    voiceparams.MixMonoToMonoInterp(x, c_amp);
                //--Bounds check
                if (sync == SyncMode.Soft)
                {
                    if (voiceparams.pData[0].double1 >= cGen.LoopEndPhase)
                        voiceparams.pData[0].double1 = cGen.LoopStartPhase + (voiceparams.pData[0].double1 - cGen.LoopEndPhase) % (cGen.LoopEndPhase - cGen.LoopStartPhase);
                    if (voiceparams.pData[1].double1 >= mGen.LoopEndPhase)
                        voiceparams.pData[1].double1 = mGen.LoopStartPhase + (voiceparams.pData[1].double1 - mGen.LoopEndPhase) % (mGen.LoopEndPhase - mGen.LoopStartPhase);
                }
                else
                {
                    if (voiceparams.pData[0].double1 >= cGen.LoopEndPhase)
                    {
                        voiceparams.pData[0].double1 = cGen.LoopStartPhase;
                        voiceparams.pData[1].double1 = mGen.LoopStartPhase;
                    }
                }
                //--Check and end early if necessary
                if (voiceparams.envelopes[0].CurrentState == EnvelopeStateEnum.None)
                {
                    voiceparams.state = VoiceStateEnum.Stopped;
                    return;
                }
            }
        }
        public override void Load(DescriptorList description, AssetManager assets)
        {
            CustomDescriptor fmConfig = description.FindCustomDescriptor("fm2c");
            cIndex = (double)fmConfig.Objects[0];
            mIndex = (double)fmConfig.Objects[1];
            feedBack = (double)fmConfig.Objects[2];
            sync = GetSyncModeFromString((string)fmConfig.Objects[3]);
            if (description.GenDescriptions[0].LoopMethod != LoopModeEnum.Continuous || description.GenDescriptions[1].LoopMethod != LoopModeEnum.Continuous)
                throw new Exception("Fm2 patches must have continuous generators with wrapping bounds.");
            cGen = description.GenDescriptions[0].ToGenerator(assets);
            mGen = description.GenDescriptions[1].ToGenerator(assets);
            cEnv = description.EnvelopeDescriptions[0];
            mEnv = description.EnvelopeDescriptions[1];
            lfo = description.LfoDescriptions[0];
        }
        public override string ToString()
        {
            return string.Format("Fm2Patch: {0}, GeneratorCount: 2, SyncMode: {1}", patchName, sync);
        }

        public static SyncMode GetSyncModeFromString(string value)
        {
            switch (value)
            {
                case "hard":
                    return SyncMode.Hard;
                case "soft":
                    return SyncMode.Soft;
                default:
                    throw new Exception("Invalid sync mode: " + value + ".");
            }
        }
    }
}
