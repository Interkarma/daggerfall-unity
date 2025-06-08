using System;
using DaggerfallWorkshop.AudioSynthesis.Bank.Descriptors;
using DaggerfallWorkshop.AudioSynthesis.Synthesis;

namespace DaggerfallWorkshop.AudioSynthesis.Bank.Components.Generators
{
    public class WhiteNoiseGenerator : Generator
    {
        private static readonly Random random = new Random();
        //--Methods
        public WhiteNoiseGenerator(GeneratorDescriptor description)
            : base(description)
        {
            if(end < 0)
                end = 1;
            if (start < 0)
                start = 0;
            if (loopEnd < 0)
                loopEnd = end;
            if (loopStart < 0)
                loopStart = start;
            if (genPeriod < 0)
                genPeriod = 1;
            if (root < 0)
                root = 69;
            freq = 440;
        }
        public override float GetValue(double phase)
        {
            return (float)((random.NextDouble() * 2.0) - 1.0);
        }
        public override void GetValues(GeneratorParameters generatorParams, float[] blockBuffer, double increment)
        {
            int proccessed = 0;
            do
            {
                int samplesAvailable = (int)Math.Ceiling((generatorParams.currentEnd - generatorParams.phase) / increment);
                if (samplesAvailable > blockBuffer.Length - proccessed)
                {
                    while (proccessed < blockBuffer.Length)
                    {
                        blockBuffer[proccessed++] = (float)((random.NextDouble() * 2.0) - 1.0);
                        generatorParams.phase += increment;
                    }
                }
                else
                {
                    int endProccessed = proccessed + samplesAvailable;
                    while (proccessed < endProccessed)
                    {
                        blockBuffer[proccessed++] = (float)((random.NextDouble() * 2.0) - 1.0);
                        generatorParams.phase += increment;
                    }
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
    }
}