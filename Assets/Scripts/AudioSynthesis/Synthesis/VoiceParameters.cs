using DaggerfallWorkshop.AudioSynthesis.Bank.Components;
using DaggerfallWorkshop.AudioSynthesis.Bank.Components.Generators;
using System;

namespace DaggerfallWorkshop.AudioSynthesis.Synthesis
{
    public class VoiceParameters
    {
        public int channel;
        public int note;
        public int velocity;
        public bool noteOffPending;
        public VoiceStateEnum state;
        public int pitchOffset;
        public float volOffset;
        public float[] blockBuffer;
        public UnionData[] pData;    //used for anything, counters, params, or mixing
        public SynthParameters synthParams;
        public GeneratorParameters[] generatorParams;
        public Envelope[] envelopes;    //set by parameters (quicksetup)
        public Filter[] filters;        //set by parameters (quicksetup)
        public Lfo[] lfos;              //set by parameters (quicksetup)
        private float mix1, mix2;

        public float CombinedVolume
        {
            get { return mix1 + mix2; }
        }

        public VoiceParameters()
        {
            blockBuffer = new float[Synthesizer.DefaultBlockSize];
            //create default number of each component
            pData = new UnionData[Synthesizer.MaxVoiceComponents];
            generatorParams = new GeneratorParameters[Synthesizer.MaxVoiceComponents];
            envelopes = new Envelope[Synthesizer.MaxVoiceComponents];
            filters = new Filter[Synthesizer.MaxVoiceComponents];
            lfos = new Lfo[Synthesizer.MaxVoiceComponents];
            //initialize each component
            for (int x = 0; x < Synthesizer.MaxVoiceComponents; x++)
            {
                generatorParams[x] = new GeneratorParameters();
                envelopes[x] = new Envelope();
                filters[x] = new Filter();
                lfos[x] = new Lfo();
            }
        }
        public void Reset()
        {
            noteOffPending = false;
            pitchOffset = 0;
            volOffset = 0;
            Array.Clear(pData, 0, pData.Length);
            mix1 = 0;
            mix2 = 0;
        }
        public void MixMonoToMonoInterp(int startIndex, float volume)
        {
            float inc = (volume - mix1) / Synthesizer.DefaultBlockSize;
            for (int i = 0; i < blockBuffer.Length; i++)
            {
                mix1 += inc;
                synthParams.synth.sampleBuffer[startIndex + i] += blockBuffer[i] * mix1;
            }
            mix1 = volume;
        }
        public void MixMonoToStereoInterp(int startIndex, float leftVol, float rightVol)
        {
            float inc_l = (leftVol - mix1) / Synthesizer.DefaultBlockSize;
            float inc_r = (rightVol - mix2) / Synthesizer.DefaultBlockSize;
            for (int i = 0; i < blockBuffer.Length; i++)
            {
                mix1 += inc_l;
                mix2 += inc_r;
                synthParams.synth.sampleBuffer[startIndex] += blockBuffer[i] * mix1;
                synthParams.synth.sampleBuffer[startIndex + 1] += blockBuffer[i] * mix2;
                startIndex += 2;
            }
            mix1 = leftVol;
            mix2 = rightVol;
        }
        public void MixStereoToStereoInterp(int startIndex, float leftVol, float rightVol)
        {
            float inc_l = (leftVol - mix1) / Synthesizer.DefaultBlockSize;
            float inc_r = (rightVol - mix2) / Synthesizer.DefaultBlockSize;
            for (int i = 0; i < blockBuffer.Length; i++)
            {
                mix1 += inc_l;
                mix2 += inc_r;
                synthParams.synth.sampleBuffer[startIndex + i] += blockBuffer[i] * mix1;
                i++;
                synthParams.synth.sampleBuffer[startIndex + i] += blockBuffer[i] * mix2;
            }
            mix1 = leftVol;
            mix2 = rightVol;
        }

        public override string ToString()
        {
            return string.Format("Channel: {0}, Key: {1}, Velocity: {2}, State: {3}", channel, note, velocity, state);
        }
    }
}
