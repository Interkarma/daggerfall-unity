using System;
using DaggerfallWorkshop.AudioSynthesis.Synthesis;
using DaggerfallWorkshop.AudioSynthesis.Bank.Descriptors;
using DaggerfallWorkshop.AudioSynthesis.Bank.Components;
using DaggerfallWorkshop.AudioSynthesis.Bank.Components.Generators;

namespace DaggerfallWorkshop.AudioSynthesis.Bank.Patches
{
    /* A single generator patch with sfz parameters 
     * 
     * (Pitch)  (Cutoff)  (Volume)
     *    |        |        |  
     *   ENV0     ENV1     ENV2
     *    |        |        |
     *   LFO0     LFO1     LFO2
     *    |        |        |
     *   GEN0 --> FLT0 --> MIX --> OUTPUT
     *   
     * ENV0 : An envelope that effects pitch
     * ENV1 : An envelope that effects the filter's cutoff
     * ENV2 : An envelope that effects volume
     * LFO0 : LFO used for pitch modulation
     * LFO1 : LFO used to alter the filter's cutoff
     * LFO2 : LFO for tremulo effect on volume
     * GEN0 : A sample generator
     * FLT0 : A filter
     * MIX  : Handles volume mixing (interp and panning)
     */
    public class SfzPatch : Patch
    {
        private float sfzVolume;
        private float ampKeyTrack;
        private float ampVelTrack;
        private PanComponent sfzPan;
        private short ampRootKey;
        private Generator gen;
        private EnvelopeDescriptor ptch_env, fltr_env, amp_env;
        private LfoDescriptor ptch_lfo, fltr_lfo, amp_lfo;
        private FilterDescriptor fltr;

        public SfzPatch(string name) : base(name) { }
        public override bool Start(VoiceParameters voiceparams)
        {
            //calculate velocity
            float fVel = voiceparams.velocity / 127f;
            //setup generator
            voiceparams.generatorParams[0].QuickSetup(gen);
            //setup envelopes
            voiceparams.envelopes[0].QuickSetup(voiceparams.synthParams.synth.SampleRate, fVel, ptch_env);
            voiceparams.envelopes[1].QuickSetup(voiceparams.synthParams.synth.SampleRate, fVel, fltr_env);
            voiceparams.envelopes[2].QuickSetup(voiceparams.synthParams.synth.SampleRate, fVel, amp_env);
            //setup lfos
            voiceparams.lfos[0].QuickSetup(voiceparams.synthParams.synth.SampleRate, ptch_lfo);
            voiceparams.lfos[1].QuickSetup(voiceparams.synthParams.synth.SampleRate, fltr_lfo);
            voiceparams.lfos[2].QuickSetup(voiceparams.synthParams.synth.SampleRate, amp_lfo);
            //setup filter
            voiceparams.filters[0].QuickSetup(voiceparams.synthParams.synth.SampleRate, voiceparams.note, fVel, fltr);
            voiceparams.pData[0].double1 = voiceparams.filters[0].Cutoff;
            if (!voiceparams.filters[0].Enabled)
            {//disable filter components if necessary
                voiceparams.envelopes[1].Depth = 0f;
                voiceparams.lfos[1].Depth = 0f;
            }
            //setup sfz params
              //calculate initial pitch
            voiceparams.pitchOffset = (voiceparams.note - gen.RootKey) * gen.KeyTrack + (int)(fVel * gen.VelocityTrack) + gen.Tune;
            voiceparams.pitchOffset += (int)(100.0 * (voiceparams.synthParams.masterCoarseTune + (voiceparams.synthParams.masterFineTune.Combined - 8192.0) / 8192.0));
              //calculate initial vol
            voiceparams.volOffset = voiceparams.synthParams.volume.Combined / 16383f;
            voiceparams.volOffset *= voiceparams.volOffset * voiceparams.synthParams.synth.MixGain;
            float dBVel = -20.0f * (float)Math.Log10(16129.0 / (voiceparams.velocity * voiceparams.velocity));
            voiceparams.volOffset *= (float)SynthHelper.DBtoLinear((voiceparams.note - ampRootKey) * ampKeyTrack + dBVel * ampVelTrack + sfzVolume);
            //check if we have finished before we have begun
            return voiceparams.generatorParams[0].currentState != GeneratorStateEnum.Finished && voiceparams.envelopes[2].CurrentState != EnvelopeStateEnum.None;
        }
        public override void Stop(VoiceParameters voiceparams)
        {
            gen.Release(voiceparams.generatorParams[0]);
            if (gen.LoopMode != LoopModeEnum.OneShot)
            {
                voiceparams.envelopes[0].Release(Synthesis.Synthesizer.DenormLimit);
                voiceparams.envelopes[1].Release(Synthesis.Synthesizer.DenormLimit);
                voiceparams.envelopes[2].Release(Synthesis.Synthesizer.NonAudible);
            }
        }
        public override void Process(VoiceParameters voiceparams, int startIndex, int endIndex)
        {
            //--Base pitch calculation
            double basePitch = SynthHelper.CentsToPitch(voiceparams.pitchOffset + voiceparams.synthParams.currentPitch)
                * gen.Frequency / voiceparams.synthParams.synth.SampleRate;
            //--Base volume calculation
            float baseVolume = voiceparams.volOffset * voiceparams.synthParams.currentVolume;
            //--Main Loop
            for (int x = startIndex; x < endIndex; x += Synthesizer.DefaultBlockSize * voiceparams.synthParams.synth.AudioChannels)
            {
                //--Envelope Calculations
                if (voiceparams.envelopes[0].Depth != 0)
                    voiceparams.envelopes[0].Increment(Synthesizer.DefaultBlockSize); //pitch envelope
                if (voiceparams.envelopes[1].Depth != 0)
                    voiceparams.envelopes[1].Increment(Synthesizer.DefaultBlockSize); //filter envelope
                voiceparams.envelopes[2].Increment(Synthesizer.DefaultBlockSize); //amp envelope (do not skip)
                //--LFO Calculations
                if (voiceparams.lfos[0].Depth + voiceparams.synthParams.currentMod != 0)
                    voiceparams.lfos[0].Increment(Synthesizer.DefaultBlockSize); //pitch lfo
                if (voiceparams.lfos[1].Depth != 0)
                    voiceparams.lfos[1].Increment(Synthesizer.DefaultBlockSize); //filter lfo
                if (voiceparams.lfos[2].Depth != 1.0)//linear scale 1.0 = 0dB
                    voiceparams.lfos[2].Increment(Synthesizer.DefaultBlockSize); //amp lfo
                //--Calculate pitch and get next block of samples
                gen.GetValues(voiceparams.generatorParams[0], voiceparams.blockBuffer, basePitch *
                    SynthHelper.CentsToPitch((int)(voiceparams.envelopes[0].Value * voiceparams.envelopes[0].Depth +
                    voiceparams.lfos[0].Value * (voiceparams.lfos[0].Depth + voiceparams.synthParams.currentMod))));
                //--Filter if enabled
                if (voiceparams.filters[0].Enabled)
                {
                    int cents = (int)(voiceparams.envelopes[1].Value * voiceparams.envelopes[1].Depth) + (int)(voiceparams.lfos[1].Value * voiceparams.lfos[1].Depth);
                    voiceparams.filters[0].Cutoff = voiceparams.pData[0].double1 * SynthHelper.CentsToPitch(cents);
                    if (voiceparams.filters[0].CoeffNeedsUpdating)
                        voiceparams.filters[0].ApplyFilterInterp(voiceparams.blockBuffer, voiceparams.synthParams.synth.SampleRate);
                    else
                        voiceparams.filters[0].ApplyFilter(voiceparams.blockBuffer);
                }
                //--Volume calculation
                float volume = baseVolume * voiceparams.envelopes[2].Value * (float)(Math.Pow(voiceparams.lfos[2].Depth, voiceparams.lfos[2].Value));
                //--Mix block based on number of channels
                if (voiceparams.synthParams.synth.AudioChannels == 2)
                    voiceparams.MixMonoToStereoInterp(x,
                        volume * sfzPan.Left * voiceparams.synthParams.currentPan.Left,
                        volume * sfzPan.Right * voiceparams.synthParams.currentPan.Right);
                else
                    voiceparams.MixMonoToMonoInterp(x, volume);
                //--Check and end early if necessary
                if (voiceparams.envelopes[2].CurrentState == EnvelopeStateEnum.None || voiceparams.generatorParams[0].currentState == GeneratorStateEnum.Finished)
                {
                    voiceparams.state = VoiceStateEnum.Stopped;
                    return;
                }
            }
        }
        public override void Load(DescriptorList description, AssetManager assets)
        {
            //read in sfz params
            CustomDescriptor sfzConfig = description.FindCustomDescriptor("sfzi");
            exTarget = (int)sfzConfig.Objects[0];
            exGroup = (int)sfzConfig.Objects[1];
            sfzVolume = (float)sfzConfig.Objects[2];
            sfzPan = new PanComponent((float)sfzConfig.Objects[3], PanFormulaEnum.Neg3dBCenter);
            ampKeyTrack = (float)sfzConfig.Objects[4];
            ampRootKey = (byte)sfzConfig.Objects[5];
            ampVelTrack = (float)sfzConfig.Objects[6];
            //read in the generator info
            GeneratorDescriptor gdes = description.GenDescriptions[0];
            if (gdes.SamplerType != WaveformEnum.SampleData)
                throw new Exception("Sfz can only support sample data generators.");
            gen = gdes.ToGenerator(assets);
            //read in the envelope info
            ptch_env = description.EnvelopeDescriptions[0];
            fltr_env = description.EnvelopeDescriptions[1];
            amp_env = description.EnvelopeDescriptions[2];
            //read in the lfo info
            ptch_lfo = description.LfoDescriptions[0];
            fltr_lfo = description.LfoDescriptions[1];
            amp_lfo = description.LfoDescriptions[2];
            //read in the filter info
            fltr = description.FilterDescriptions[0];
        }
    }
}