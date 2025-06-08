namespace DaggerfallWorkshop.AudioSynthesis.Bank.Descriptors
{
    using System;
    using System.IO;
    using DaggerfallWorkshop.AudioSynthesis.Synthesis;
    using DaggerfallWorkshop.AudioSynthesis.Bank.Components;
    using DaggerfallWorkshop.AudioSynthesis.Bank.Components.Generators;

    public class LfoDescriptor : IDescriptor
    {
        public const string ID = "lfo ";
        public const int SIZE = 14;

        public float DelayTime;
        public float Frequency;
        public float Depth;
        public Generator Generator;
        
        public LfoDescriptor()
        {
            ApplyDefault();
        }
        public void Read(string[] description)
        {
            ApplyDefault();
            for (int x = 0; x < description.Length; x++)
            {
                int index = description[x].IndexOf('=');
                if (index >= 0 && index < description[x].Length)
                {
                    string paramName = description[x].Substring(0, index).Trim().ToLower();
                    string paramValue = description[x].Substring(index + 1).Trim();
                    switch (paramName)
                    {
                        case "delaytime":
                            DelayTime = float.Parse(paramValue);
                            break;
                        case "frequency":
                            Frequency = float.Parse(paramValue);
                            break;
                        case "depth":
                            Depth = float.Parse(paramValue);
                            break;
                        case "type":
                            Generator = GetGenerator(Generator.GetWaveformFromString(paramValue.ToLower()));
                            break;
                    }
                }
            }
            CheckValidParameters();
        }
        public int Read(BinaryReader reader)
        {
            DelayTime = reader.ReadSingle();
            Frequency = reader.ReadSingle();
            Depth = reader.ReadSingle();
            Generator = GetGenerator((WaveformEnum)reader.ReadInt16());
            CheckValidParameters();
            return SIZE;
        }
        public int Write(BinaryWriter writer)
        {
            writer.Write(DelayTime);
            writer.Write(Frequency);
            writer.Write(Depth);
            writer.Write((short)GetWaveform(Generator));
            return SIZE;
        }

        private static WaveformEnum GetWaveform(Generator gen)
        {
            if (gen == Generator.DefaultSaw)
                return WaveformEnum.Saw;
            else if (gen == Generator.DefaultSine)
                return WaveformEnum.Sine;
            else if (gen == Generator.DefaultSquare)
                return WaveformEnum.Square;
            else if (gen == Generator.DefaultTriangle)
                return WaveformEnum.Triangle;
            else
                throw new Exception("Invalid lfo waveform.");
        }
        private static Generator GetGenerator(WaveformEnum waveform)
        {
            switch (waveform)
            {
                case WaveformEnum.Saw:
                    return Generator.DefaultSaw;
                case WaveformEnum.Square:
                    return Generator.DefaultSquare;
                case WaveformEnum.Triangle:
                    return Generator.DefaultTriangle;
                default:
                    return Generator.DefaultSine;
            }
        }
        private void ApplyDefault()
        {
            DelayTime = 0f;
            Frequency = (float)Synthesizer.DefaultLfoFrequency;
            Depth = 1;
            Generator = Generator.DefaultSine;
        }
        private void CheckValidParameters()
        {
            DelayTime = Math.Max(DelayTime, 0);
            Frequency = SynthHelper.Clamp(Frequency, 1e-5f, 20f);
        }
        
    }
}