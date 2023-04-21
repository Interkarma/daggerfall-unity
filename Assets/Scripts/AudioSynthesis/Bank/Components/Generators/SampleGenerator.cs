using System;
using DaggerfallWorkshop.AudioSynthesis.Synthesis;
using DaggerfallWorkshop.AudioSynthesis.Bank.Descriptors;
using DaggerfallWorkshop.AudioSynthesis.Util;
using DaggerfallWorkshop.AudioSynthesis.Wave;

namespace DaggerfallWorkshop.AudioSynthesis.Bank.Components.Generators
{
    public class SampleGenerator : Generator
    {
        private PcmData data;

        public PcmData Samples
        {
            get { return data; }
            set { data = value; }
        }

        public SampleGenerator()
            : base(new GeneratorDescriptor()) { }
        public SampleGenerator(GeneratorDescriptor description, AssetManager assets)
            : base(description)
        {
            SampleDataAsset sample = assets.FindSample(IOHelper.GetFileNameWithoutExtension(description.AssetName));
            if (sample == null)
                throw new Exception("Could not find asset: (" + description.AssetName + ").");
            data = sample.SampleData;
            freq = sample.SampleRate;
            if (end < 0)
                end = sample.End;
            if (start < 0)
                start = sample.Start;
            if (loopEnd < 0)
            {
                if (sample.LoopEnd < 0)
                    loopEnd = end;
                else
                    loopEnd = sample.LoopEnd;
            }
            if (loopStart < 0)
            {
                if (sample.LoopStart < 0)
                    loopStart = start;
                else
                    loopStart = sample.LoopStart;
            }
            if (genPeriod < 0)
                genPeriod = 1;
            if (root < 0)
            {
                root = sample.RootKey;
                if (tuneCents == 0)
                    tuneCents = sample.Tune;
            }
            //check sample end and loop end for consistency
            if (end > data.Length)
                end = data.Length;
            if (loopEnd > end)
                loopEnd = end;
        }
        public override float GetValue(double phase)
        {
            return data[(int)phase];
        }
        public override void GetValues(GeneratorParameters generatorParams, float[] blockBuffer, double increment)
        {
            int proccessed = 0;
            do
            {
                int samplesAvailable = (int)Math.Ceiling((generatorParams.currentEnd - generatorParams.phase) / increment);
                if (samplesAvailable > blockBuffer.Length - proccessed)
                {
                    Interpolate(generatorParams, blockBuffer, increment, proccessed, blockBuffer.Length);
                    return; //proccessed = blockBuffer.Length;
                }
                else
                {
                    int endProccessed = proccessed + samplesAvailable;
                    Interpolate(generatorParams, blockBuffer, increment, proccessed, endProccessed);
                    proccessed = endProccessed;
                    switch (generatorParams.currentState)
                    {
                        case GeneratorStateEnum.PreLoop:
                            generatorParams.currentStart = loopStart;
                            generatorParams.currentEnd = loopEnd;
                            generatorParams.currentState = GeneratorStateEnum.Loop;
                            break;
                        case GeneratorStateEnum.Loop:
                            generatorParams.phase += generatorParams.currentStart - generatorParams.currentEnd;
                            break;
                        case GeneratorStateEnum.PostLoop:
                            generatorParams.currentState = GeneratorStateEnum.Finished;
                            while (proccessed < blockBuffer.Length)
                                blockBuffer[proccessed++] = 0f;
                            break;
                    }
                }
            }
            while (proccessed < blockBuffer.Length);
        }

        private void Interpolate(GeneratorParameters generatorParams, float[] blockBuffer, double increment, int start, int end)
        {
            switch (Synthesizer.InterpolationMode)
            {
                case InterpolationEnum.Linear:
                #region Linear
                {
                    double _end = generatorParams.currentState == GeneratorStateEnum.Loop ? this.loopEnd - 1 : this.end - 1;
                    int index;
                    float s1, s2, mu;
                    while (start < end && generatorParams.phase < _end)//do this until we reach an edge case or fill the buffer
                    {
                        index = (int)generatorParams.phase;
                        s1 = data[index];
                        s2 = data[index + 1];
                        mu = (float)(generatorParams.phase - index);
                        blockBuffer[start++] = s1 + mu * (s2 - s1);
                        generatorParams.phase += increment;
                    }
                    while (start < end)//edge case, if in loop wrap to loop start else use duplicate sample
                    {
                        index = (int)generatorParams.phase;
                        s1 = data[index];
                        if (generatorParams.currentState == GeneratorStateEnum.Loop)
                            s2 = data[(int)generatorParams.currentStart];
                        else
                            s2 = s1;
                        mu = (float)(generatorParams.phase - index);
                        blockBuffer[start++] = s1 + mu * (s2 - s1);
                        generatorParams.phase += increment;
                    }
                }
                #endregion
                break;
                case InterpolationEnum.Cosine:
                #region Cosine
                {
                    double _end = generatorParams.currentState == GeneratorStateEnum.Loop ? this.loopEnd - 1 : this.end - 1;
                    int index;
                    float s1, s2, mu;
                    while (start < end && generatorParams.phase < _end)//do this until we reach an edge case or fill the buffer
                    {
                        index = (int)generatorParams.phase;
                        s1 = data[index];
                        s2 = data[index + 1];
                        mu = (1f - (float)Math.Cos((generatorParams.phase - index) * Math.PI)) * 0.5f;
                        blockBuffer[start++] = s1 * (1f - mu) + s2 * mu;
                        generatorParams.phase += increment;
                    }
                    while (start < end)//edge case, if in loop wrap to loop start else use duplicate sample
                    {
                        index = (int)generatorParams.phase;
                        s1 = data[index];
                        if (generatorParams.currentState == GeneratorStateEnum.Loop)
                            s2 = data[(int)generatorParams.currentStart];
                        else
                            s2 = s1;
                        mu = (1f - (float)Math.Cos((generatorParams.phase - index) * Math.PI)) * 0.5f;
                        blockBuffer[start++] = s1 * (1f - mu) + s2 * mu;
                        generatorParams.phase += increment;
                    }
                }
                #endregion
                break;
                case InterpolationEnum.CubicSpline:
                #region CubicSpline
                {
                    double _end = generatorParams.currentState == GeneratorStateEnum.Loop ? this.loopStart + 1 : this.start + 1;
                    int index;
                    float s0, s1, s2, s3, mu;
                    while (start < end && generatorParams.phase < _end)//edge case, wrap to endpoint or duplicate sample
                    {
                        index = (int)generatorParams.phase;
                        if (generatorParams.currentState == GeneratorStateEnum.Loop)
                            s0 = data[(int)generatorParams.currentEnd - 1];
                        else
                            s0 = data[index];
                        s1 = data[index];
                        s2 = data[index + 1];
                        s3 = data[index + 2];
                        mu = (float)(generatorParams.phase - index);
                        blockBuffer[start++] = ((-0.5f * s0 + 1.5f * s1 - 1.5f * s2 + 0.5f * s3) * mu * mu * mu + (s0 - 2.5f * s1 + 2f * s2 - 0.5f * s3) * mu * mu + (-0.5f * s0 + 0.5f * s2) * mu + (s1));
                        generatorParams.phase += increment;
                    }
                    _end = generatorParams.currentState == GeneratorStateEnum.Loop ? this.loopEnd - 2 : this.end - 2;
                    while (start < end && generatorParams.phase < _end)
                    {
                        index = (int)generatorParams.phase;
                        s0 = data[index - 1];
                        s1 = data[index];
                        s2 = data[index + 1];
                        s3 = data[index + 2];
                        mu = (float)(generatorParams.phase - index);
                        blockBuffer[start++] = ((-0.5f * s0 + 1.5f * s1 - 1.5f * s2 + 0.5f * s3) * mu * mu * mu + (s0 - 2.5f * s1 + 2f * s2 - 0.5f * s3) * mu * mu + (-0.5f * s0 + 0.5f * s2) * mu + (s1));
                        generatorParams.phase += increment;
                    }
                    _end += 1;
                    while (start < end)//edge case, wrap to startpoint or duplicate sample
                    {
                        index = (int)generatorParams.phase;
                        s0 = data[index - 1];
                        s1 = data[index];
                        if (generatorParams.phase < _end)
                        {
                            s2 = data[index + 1];
                            if (generatorParams.currentState == GeneratorStateEnum.Loop)
                                s3 = data[(int)generatorParams.currentStart];
                            else
                                s3 = s2;
                        }
                        else
                        {
                            if (generatorParams.currentState == GeneratorStateEnum.Loop)
                            {
                                s2 = data[(int)generatorParams.currentStart];
                                s3 = data[(int)generatorParams.currentStart + 1];
                            }
                            else
                            {
                                s2 = s1;
                                s3 = s1;
                            }
                        }
                        mu = (float)(generatorParams.phase - index);
                        blockBuffer[start++] = ((-0.5f * s0 + 1.5f * s1 - 1.5f * s2 + 0.5f * s3) * mu * mu * mu + (s0 - 2.5f * s1 + 2f * s2 - 0.5f * s3) * mu * mu + (-0.5f * s0 + 0.5f * s2) * mu + (s1));
                        generatorParams.phase += increment;
                    }
                }
                #endregion
                break;
                default:
                #region None
                {
                    while (start < end)
                    {
                        blockBuffer[start++] = data[(int)generatorParams.phase];
                        generatorParams.phase += increment;
                    }
                }
                #endregion
                break;
            }
        }
    }
}
