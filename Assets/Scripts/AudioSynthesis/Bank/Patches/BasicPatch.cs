using DaggerfallWorkshop.AudioSynthesis.Synthesis;
using DaggerfallWorkshop.AudioSynthesis.Bank.Components;
using DaggerfallWorkshop.AudioSynthesis.Bank.Components.Generators;
using DaggerfallWorkshop.AudioSynthesis.Bank.Descriptors;

namespace DaggerfallWorkshop.AudioSynthesis.Bank.Patches
{
    /* A simple single generator patch
     * 
     *    LFO1
     *     |
     *    GEN1 --> ENV1 --> OUT
     *    
     * LFO1 : Usually generates vibrato. Responds to the MOD Controller (MIDI Controlled).
     * GEN1 : Any generator. No restriction on sampler type.
     * ENV1 : An envelope controlling the amplitude of GEN1.
     */
    public class BasicPatch : Patch
    {
        private Generator gen;
        private EnvelopeDescriptor env;
        private LfoDescriptor lfo;

        public BasicPatch(string name) : base(name) { }
        public override bool Start(VoiceParameters voiceparams)
        {
            //calculate velocity
            float fVel = voiceparams.velocity / 127f;
            //reset generator
            voiceparams.generatorParams[0].QuickSetup(gen);
            //reset envelope
            voiceparams.envelopes[0].QuickSetup(voiceparams.synthParams.synth.SampleRate, fVel, env);
            //reset lfo (vibra)
            voiceparams.lfos[0].QuickSetup(voiceparams.synthParams.synth.SampleRate, lfo);
            //calculate initial pitch
            voiceparams.pitchOffset = (voiceparams.note - gen.RootKey) * gen.KeyTrack + (int)(fVel * gen.VelocityTrack) + gen.Tune;
            voiceparams.pitchOffset += (int)(100.0 * (voiceparams.synthParams.masterCoarseTune + (voiceparams.synthParams.masterFineTune.Combined - 8192.0) / 8192.0));
            //calculate initial volume
            voiceparams.volOffset = voiceparams.synthParams.volume.Combined / 16383f;
            voiceparams.volOffset *= voiceparams.volOffset * fVel * voiceparams.synthParams.synth.MixGain;
            //check if we have finished before we have begun
            return voiceparams.generatorParams[0].currentState != GeneratorStateEnum.Finished && voiceparams.envelopes[0].CurrentState != EnvelopeStateEnum.None;
        }
        public override void Stop(VoiceParameters voiceparams)
        {
            gen.Release(voiceparams.generatorParams[0]);
            if (gen.LoopMode != LoopModeEnum.OneShot)
                voiceparams.envelopes[0].Release(Synthesis.Synthesizer.NonAudible);
        }
        public override void Process(VoiceParameters voiceparams, int startIndex, int endIndex)
        {
            //--Base pitch calculation
            double basePitch = SynthHelper.CentsToPitch(voiceparams.pitchOffset + voiceparams.synthParams.currentPitch)
                * gen.Period * gen.Frequency / voiceparams.synthParams.synth.SampleRate;
            //--Base volume calculation
            float baseVolume = voiceparams.volOffset * voiceparams.synthParams.currentVolume;
            //--Main Loop
            for (int x = startIndex; x < endIndex; x += Synthesizer.DefaultBlockSize * voiceparams.synthParams.synth.AudioChannels)
            {
                //--Volume Envelope
                voiceparams.envelopes[0].Increment(Synthesizer.DefaultBlockSize);
                //--Lfo pitch modulation
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
                //--Get next block of samples
                gen.GetValues(voiceparams.generatorParams[0], voiceparams.blockBuffer, basePitch * pitchMod);
                //--Mix block based on number of channels
                float volume = baseVolume * voiceparams.envelopes[0].Value;
                if (voiceparams.synthParams.synth.AudioChannels == 2)
                    voiceparams.MixMonoToStereoInterp(x, 
                        volume * voiceparams.synthParams.currentPan.Left, 
                        volume * voiceparams.synthParams.currentPan.Right);
                else
                    voiceparams.MixMonoToMonoInterp(x, volume);
                //--Check and end early if necessary
                if (voiceparams.envelopes[0].CurrentState == EnvelopeStateEnum.None || voiceparams.generatorParams[0].currentState == GeneratorStateEnum.Finished)
                {
                    voiceparams.state = VoiceStateEnum.Stopped;
                    return;
                }
            }
        }
        public override void Load(DescriptorList description, AssetManager assets)
        {
            gen = description.GenDescriptions[0].ToGenerator(assets);
            env = description.EnvelopeDescriptions[0];
            lfo = description.LfoDescriptions[0];
        }
        public override string ToString()
        {
            return string.Format("BasicPatch: {0}, GeneratorCount: 1", patchName);
        }
    }
}
