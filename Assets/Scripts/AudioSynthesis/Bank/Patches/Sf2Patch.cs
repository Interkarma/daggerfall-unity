using System;
using DaggerfallWorkshop.AudioSynthesis.Synthesis;
using DaggerfallWorkshop.AudioSynthesis.Sf2;
using DaggerfallWorkshop.AudioSynthesis.Bank.Descriptors;
using DaggerfallWorkshop.AudioSynthesis.Bank.Components.Generators;
using DaggerfallWorkshop.AudioSynthesis.Bank.Components;

namespace DaggerfallWorkshop.AudioSynthesis.Bank.Patches
{
    public class Sf2Patch : Patch
    {
        private int iniFilterFc;
        private double filterQ;
        private float initialAttn;
        private short keyOverride;
        private short velOverride;
        private short keynumToModEnvHold ;
        private short keynumToModEnvDecay;
        private short keynumToVolEnvHold ;
        private short keynumToVolEnvDecay;
        private PanComponent pan;
        private short modLfoToPitch;
        private short vibLfoToPitch;
        private short modEnvToPitch;
        private short modLfoToFilterFc;
        private short modEnvToFilterFc;
        private float modLfoToVolume;
        private DaggerfallWorkshop.AudioSynthesis.Bank.Components.Generators.Generator gen;
        private EnvelopeDescriptor mod_env, vel_env;
        private LfoDescriptor mod_lfo, vib_lfo;
        private FilterDescriptor fltr;

        public Sf2Patch(string name) : base(name) { }
        public override bool Start(VoiceParameters voiceparams)
        {
            int note = keyOverride > -1 ? keyOverride : voiceparams.note;
            int vel = velOverride > -1 ? velOverride : voiceparams.velocity;
            //setup generator
            voiceparams.generatorParams[0].QuickSetup(gen);
            //setup envelopes
            voiceparams.envelopes[0].QuickSetupSf2(voiceparams.synthParams.synth.SampleRate, note, keynumToModEnvHold, keynumToModEnvDecay, false, mod_env);
            voiceparams.envelopes[1].QuickSetupSf2(voiceparams.synthParams.synth.SampleRate, note, keynumToVolEnvHold, keynumToVolEnvDecay, true, vel_env);
            //setup filter
            //voiceparams.pData[0].int1 = iniFilterFc - (int)(2400 * CalculateModulator(SourceTypeEnum.Linear, TransformEnum.Linear, DirectionEnum.MaxToMin, PolarityEnum.Unipolar, voiceparams.velocity, 0, 127)); 
            //if (iniFilterFc >= 13500 && fltr.Resonance <= 1)
                voiceparams.filters[0].Disable();
            //else
            //    voiceparams.filters[0].QuickSetup(voiceparams.synthParams.synth.SampleRate, note, 1f, fltr);
            //setup lfos
            voiceparams.lfos[0].QuickSetup(voiceparams.synthParams.synth.SampleRate, mod_lfo);
            voiceparams.lfos[1].QuickSetup(voiceparams.synthParams.synth.SampleRate, vib_lfo);
            //calculate initial pitch
            voiceparams.pitchOffset = (note - gen.RootKey) * gen.KeyTrack + gen.Tune;
            voiceparams.pitchOffset += (int)(100.0 * (voiceparams.synthParams.masterCoarseTune + (voiceparams.synthParams.masterFineTune.Combined - 8192.0) / 8192.0));
            //calculate initial volume
            voiceparams.volOffset = initialAttn;
            voiceparams.volOffset -= 96.0f * (float)CalculateModulator(SourceTypeEnum.Concave, TransformEnum.Linear, DirectionEnum.MaxToMin, PolarityEnum.Unipolar, voiceparams.velocity, 0, 127);
            voiceparams.volOffset -= 96.0f * (float)CalculateModulator(SourceTypeEnum.Concave, TransformEnum.Linear, DirectionEnum.MaxToMin, PolarityEnum.Unipolar, voiceparams.synthParams.volume.Coarse, 0, 127);
            //check if we have finished before we have begun
            return voiceparams.generatorParams[0].currentState != GeneratorStateEnum.Finished && voiceparams.envelopes[1].CurrentState != EnvelopeStateEnum.None;
        }       
        public override void Stop(VoiceParameters voiceparams)
        {
            gen.Release(voiceparams.generatorParams[0]);
            if (gen.LoopMode != LoopModeEnum.OneShot)
            {
                voiceparams.envelopes[0].Release(Synthesis.Synthesizer.DenormLimit);
                voiceparams.envelopes[1].ReleaseSf2VolumeEnvelope();
            }
        }
        public override void Process(VoiceParameters voiceparams, int startIndex, int endIndex)
        {
            //--Base pitch calculation
            double basePitch = SynthHelper.CentsToPitch(voiceparams.pitchOffset + voiceparams.synthParams.currentPitch)
                * gen.Frequency / voiceparams.synthParams.synth.SampleRate;
            float baseVolume = voiceparams.synthParams.currentVolume * voiceparams.synthParams.synth.MixGain;
            //--Main Loop
            for (int x = startIndex; x < endIndex; x += Synthesizer.DefaultBlockSize * voiceparams.synthParams.synth.AudioChannels)
            {
                voiceparams.envelopes[0].Increment(Synthesizer.DefaultBlockSize);
                voiceparams.envelopes[1].Increment(Synthesizer.DefaultBlockSize);
                voiceparams.lfos[0].Increment(Synthesizer.DefaultBlockSize);
                voiceparams.lfos[1].Increment(Synthesizer.DefaultBlockSize);
                //--Calculate pitch and get next block of samples
                gen.GetValues(voiceparams.generatorParams[0], voiceparams.blockBuffer, basePitch *
                    SynthHelper.CentsToPitch((int)(voiceparams.envelopes[0].Value * modEnvToPitch +
                    voiceparams.lfos[0].Value * modLfoToPitch + voiceparams.lfos[1].Value * vibLfoToPitch)));
                //--Filter
                if (voiceparams.filters[0].Enabled)
                {
                    double centsFc = voiceparams.pData[0].int1 + voiceparams.lfos[0].Value * modLfoToFilterFc + voiceparams.envelopes[0].Value * modEnvToFilterFc;
                    if (centsFc > 13500)
                        centsFc = 13500;
                    voiceparams.filters[0].Cutoff = SynthHelper.KeyToFrequency(centsFc / 100.0, 69);
                    if (voiceparams.filters[0].CoeffNeedsUpdating)
                        voiceparams.filters[0].ApplyFilterInterp(voiceparams.blockBuffer, voiceparams.synthParams.synth.SampleRate);
                    else
                        voiceparams.filters[0].ApplyFilter(voiceparams.blockBuffer);
                }
                //--Volume calculation
                float volume = (float)SynthHelper.DBtoLinear(voiceparams.volOffset + voiceparams.envelopes[1].Value + voiceparams.lfos[0].Value * modLfoToVolume) * baseVolume;
                //--Mix block based on number of channels
                if (voiceparams.synthParams.synth.AudioChannels == 2)
                    voiceparams.MixMonoToStereoInterp(x,
                        volume * pan.Left * voiceparams.synthParams.currentPan.Left,
                        volume * pan.Right * voiceparams.synthParams.currentPan.Right);
                else
                    voiceparams.MixMonoToMonoInterp(x, volume);
                //--Check and end early if necessary
                if ((voiceparams.envelopes[1].CurrentState > EnvelopeStateEnum.Hold && volume <= Synthesizer.NonAudible) || voiceparams.generatorParams[0].currentState == GeneratorStateEnum.Finished)
                {
                    voiceparams.state = VoiceStateEnum.Stopped;
                    return;
                }
            }
        }
        public override void Load(DescriptorList description, AssetManager assets)
        {
            throw new Exception("Sf2 does not load from patch descriptions.");
        }
        public void Load(Sf2Region region, AssetManager assets)
        {
            this.exGroup = region.Generators[(int)GeneratorEnum.ExclusiveClass];
            this.exTarget = exGroup;
            iniFilterFc = region.Generators[(int)GeneratorEnum.InitialFilterCutoffFrequency];
            filterQ = SynthHelper.DBtoLinear(region.Generators[(int)GeneratorEnum.InitialFilterQ] / 10.0);
            initialAttn = -region.Generators[(int)GeneratorEnum.InitialAttenuation] / 10f;
            keyOverride = region.Generators[(int)GeneratorEnum.KeyNumber];
            velOverride = region.Generators[(int)GeneratorEnum.Velocity];
            keynumToModEnvHold = region.Generators[(int)GeneratorEnum.KeyNumberToModulationEnvelopeHold];
            keynumToModEnvDecay = region.Generators[(int)GeneratorEnum.KeyNumberToModulationEnvelopeDecay];
            keynumToVolEnvHold = region.Generators[(int)GeneratorEnum.KeyNumberToVolumeEnvelopeHold];
            keynumToVolEnvDecay = region.Generators[(int)GeneratorEnum.KeyNumberToVolumeEnvelopeDecay];
            pan = new PanComponent(region.Generators[(int)GeneratorEnum.Pan] / 500f, PanFormulaEnum.Neg3dBCenter);
            modLfoToPitch = region.Generators[(int)GeneratorEnum.ModulationLFOToPitch];
            vibLfoToPitch = region.Generators[(int)GeneratorEnum.VibratoLFOToPitch];
            modEnvToPitch = region.Generators[(int)GeneratorEnum.ModulationEnvelopeToPitch];
            modLfoToFilterFc = region.Generators[(int)GeneratorEnum.ModulationLFOToFilterCutoffFrequency];
            modEnvToFilterFc = region.Generators[(int)GeneratorEnum.ModulationEnvelopeToFilterCutoffFrequency];
            modLfoToVolume = region.Generators[(int)GeneratorEnum.ModulationLFOToVolume] / 10f;
            LoadGen(region, assets);
            LoadEnvelopes(region);
            LoadLfos(region);
            LoadFilter(region);
        }
       
        private void LoadGen(Sf2Region region, AssetManager assets)
        {
            SampleDataAsset sda = assets.SampleAssetList[region.Generators[(int)GeneratorEnum.SampleID]];
            gen = new SampleGenerator();
            gen.EndPhase = sda.End + region.Generators[(int)GeneratorEnum.EndAddressOffset] + 32768 * region.Generators[(int)GeneratorEnum.EndAddressCoarseOffset];
            gen.Frequency = sda.SampleRate;
            gen.KeyTrack = region.Generators[(int)GeneratorEnum.ScaleTuning];
            gen.LoopEndPhase = sda.LoopEnd + region.Generators[(int)GeneratorEnum.EndLoopAddressOffset] + 32768 * region.Generators[(int)GeneratorEnum.EndLoopAddressCoarseOffset];
            switch (region.Generators[(int)GeneratorEnum.SampleModes] & 0x3)
            {
                case 0x0:
                case 0x2:
                    gen.LoopMode = LoopModeEnum.NoLoop;
                    break;
                case 0x1:
                    gen.LoopMode = LoopModeEnum.Continuous;
                    break;
                case 0x3:
                    gen.LoopMode = LoopModeEnum.LoopUntilNoteOff;
                    break;
            }
            gen.LoopStartPhase = sda.LoopStart + region.Generators[(int)GeneratorEnum.StartLoopAddressOffset] + 32768 * region.Generators[(int)GeneratorEnum.StartLoopAddressCoarseOffset];
            gen.Offset = 0;
            gen.Period = 1.0;
            if (region.Generators[(int)GeneratorEnum.OverridingRootKey] > -1)
                gen.RootKey = region.Generators[(int)GeneratorEnum.OverridingRootKey];
            else
                gen.RootKey = sda.RootKey;
            gen.StartPhase = sda.Start + region.Generators[(int)GeneratorEnum.StartAddressOffset] + 32768 * region.Generators[(int)GeneratorEnum.StartAddressCoarseOffset];
            gen.Tune = (short)(sda.Tune + region.Generators[(int)GeneratorEnum.FineTune] + 100 * region.Generators[(int)GeneratorEnum.CoarseTune]);
            gen.VelocityTrack = 0;
            ((SampleGenerator)gen).Samples = sda.SampleData;
        }
        private void LoadEnvelopes(Sf2Region region)
        {
            //mod env
            mod_env = new EnvelopeDescriptor();
            mod_env.AttackTime = (float)Math.Pow(2, region.Generators[(int)GeneratorEnum.AttackModulationEnvelope] / 1200.0);
            mod_env.AttackGraph = 3;
            mod_env.DecayTime = (float)Math.Pow(2, region.Generators[(int)GeneratorEnum.DecayModulationEnvelope] / 1200.0);
            mod_env.DelayTime = (float)Math.Pow(2, region.Generators[(int)GeneratorEnum.DelayModulationEnvelope] / 1200.0);
            mod_env.HoldTime = (float)Math.Pow(2, region.Generators[(int)GeneratorEnum.HoldModulationEnvelope] / 1200.0);
            mod_env.PeakLevel = 1;
            mod_env.ReleaseTime = (float)Math.Pow(2, region.Generators[(int)GeneratorEnum.ReleaseModulationEnvelope] / 1200.0);
            mod_env.StartLevel = 0;
            mod_env.SustainLevel = 1f - SynthHelper.Clamp(region.Generators[(int)GeneratorEnum.SustainModulationEnvelope], (short)0, (short)1000) / 1000f;
            //checks
            if (mod_env.AttackTime < 0.001f)
                mod_env.AttackTime = 0.001f;
            else if (mod_env.AttackTime > 100f)
                mod_env.AttackTime = 100f;
            if (mod_env.DecayTime < 0.001f)
                mod_env.DecayTime = 0;
            else if (mod_env.DecayTime > 100f)
                mod_env.DecayTime = 100f;
            if (mod_env.DelayTime < 0.001f)
                mod_env.DelayTime = 0;
            else if (mod_env.DelayTime > 20f)
                mod_env.DelayTime = 20f;
            if (mod_env.HoldTime < 0.001f)
                mod_env.HoldTime = 0;
            else if (mod_env.HoldTime > 20f)
                mod_env.HoldTime = 20f;
            if (mod_env.ReleaseTime < 0.001f)
                mod_env.ReleaseTime = 0.001f;
            else if (mod_env.ReleaseTime > 100f)
                mod_env.ReleaseTime = 100f;
            //volume env
            vel_env = new EnvelopeDescriptor();
            vel_env.AttackTime = (float)Math.Pow(2, region.Generators[(int)GeneratorEnum.AttackVolumeEnvelope] / 1200.0);
            vel_env.AttackGraph = 3;
            vel_env.DecayTime = (float)Math.Pow(2, region.Generators[(int)GeneratorEnum.DecayVolumeEnvelope] / 1200.0);
            vel_env.DelayTime = (float)Math.Pow(2, region.Generators[(int)GeneratorEnum.DelayVolumeEnvelope] / 1200.0);
            vel_env.HoldTime = (float)Math.Pow(2, region.Generators[(int)GeneratorEnum.HoldVolumeEnvelope] / 1200.0);
            vel_env.PeakLevel = 0;
            vel_env.ReleaseTime = (float)Math.Pow(2, region.Generators[(int)GeneratorEnum.ReleaseVolumeEnvelope] / 1200.0);
            vel_env.StartLevel = -100;
            vel_env.SustainLevel = SynthHelper.Clamp(region.Generators[(int)GeneratorEnum.SustainVolumeEnvelope], (short)0, (short)1000) / -10f;
            //checks
            if (vel_env.AttackTime < 0.001f)
                vel_env.AttackTime = 0.001f;
            else if (vel_env.AttackTime > 100f)
                vel_env.AttackTime = 100f;
            if (vel_env.DecayTime < 0.001f)
                vel_env.DecayTime = 0;
            else if (vel_env.DecayTime > 100f)
                vel_env.DecayTime = 100f;
            if (vel_env.DelayTime < 0.001f)
                vel_env.DelayTime = 0;
            else if (vel_env.DelayTime > 20f)
                vel_env.DelayTime = 20f;
            if (vel_env.HoldTime < 0.001f)
                vel_env.HoldTime = 0;
            else if (vel_env.HoldTime > 20f)
                vel_env.HoldTime = 20f;
            if (vel_env.ReleaseTime < 0.001f)
                vel_env.ReleaseTime = 0.001f;
            else if (vel_env.ReleaseTime > 100f)
                vel_env.ReleaseTime = 100f;
        }
        private void LoadLfos(Sf2Region region)
        {
            mod_lfo = new LfoDescriptor();
            mod_lfo.DelayTime = (float)Math.Pow(2, region.Generators[(int)GeneratorEnum.DelayModulationLFO] / 1200.0);
            mod_lfo.Frequency = (float)(Math.Pow(2, region.Generators[(int)GeneratorEnum.FrequencyModulationLFO] / 1200.0) * 8.176);
            mod_lfo.Generator = DaggerfallWorkshop.AudioSynthesis.Bank.Components.Generators.Generator.DefaultSine;
            vib_lfo = new LfoDescriptor();
            vib_lfo.DelayTime = (float)Math.Pow(2, region.Generators[(int)GeneratorEnum.DelayVibratoLFO] / 1200.0);
            vib_lfo.Frequency = (float)(Math.Pow(2, region.Generators[(int)GeneratorEnum.FrequencyVibratoLFO] / 1200.0) * 8.176);
            vib_lfo.Generator = DaggerfallWorkshop.AudioSynthesis.Bank.Components.Generators.Generator.DefaultSine;
        }
        private void LoadFilter(Sf2Region region)
        {
            fltr = new FilterDescriptor();
            fltr.FilterMethod = FilterTypeEnum.BiquadLowpass;
            fltr.CutOff = (float)SynthHelper.KeyToFrequency(region.Generators[(int)GeneratorEnum.InitialFilterCutoffFrequency] / 100.0, 69);
            fltr.Resonance = (float)SynthHelper.DBtoLinear(region.Generators[(int)GeneratorEnum.InitialFilterQ] / 10.0);
        }

        private static double CalculateModulator(SourceTypeEnum s, TransformEnum t, DirectionEnum d, PolarityEnum p, int value, int min, int max)
        {
            double output = 0;
            int i;
            value = value - min;
            max = max - min;
            if (d == DirectionEnum.MaxToMin)
                value = max - value;
            switch (s)
            {
                case SourceTypeEnum.Linear:
                    output = value / max;
                    break;
                case SourceTypeEnum.Concave:
                    i = 127 - value;
                    output = -(20.0 / 96.0) * Math.Log10((i * i) / (double)(max * max));
                    break;
                case SourceTypeEnum.Convex:
                    i = value;
                    output = 1 + (20.0 / 96.0) * Math.Log10((i * i) / (double)(max * max));
                    break;
                case SourceTypeEnum.Switch:
                    if (value <= (max / 2))
                        output = 0;
                    else
                        output = 1;
                    break;
            }
            if (p == PolarityEnum.Bipolar)
                output = (output * 2) - 1;
            if (t == TransformEnum.AbsoluteValue)
                output = Math.Abs(output);
            return output;
        }
    }
}
