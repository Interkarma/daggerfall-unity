namespace DaggerfallWorkshop.AudioSynthesis.Bank.Descriptors
{
    using System;
    using System.IO;
    using DaggerfallWorkshop.AudioSynthesis.Util;
    using DaggerfallWorkshop.AudioSynthesis.Bank.Components;
    using DaggerfallWorkshop.AudioSynthesis.Bank.Components.Generators;
    
    public class GeneratorDescriptor : IDescriptor
    {
        public const string ID = "gen ";
        public const int SIZE = 80;

        public LoopModeEnum LoopMethod;
        public WaveformEnum SamplerType;
        public string AssetName;
        public double EndPhase;
        public double StartPhase;
        public double LoopEndPhase;
        public double LoopStartPhase;
        public double Offset;
        public double Period;
        public short Rootkey;
        public short KeyTrack;
        public short VelTrack;
        public short Tune;

        public GeneratorDescriptor()
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
                        case "loopmode":
                            LoopMethod = Generator.GetLoopModeFromString(paramValue.ToLower());
                            break;
                        case "type":
                            SamplerType = Generator.GetWaveformFromString(paramValue.ToLower());
                            break;
                        case "assetname":
                            AssetName = paramValue;
                            break;
                        case "endphase":
                            EndPhase = double.Parse(paramValue);
                            break;
                        case "startphase":
                            StartPhase = double.Parse(paramValue);
                            break;
                        case "loopendphase":
                            LoopEndPhase = double.Parse(paramValue);
                            break;
                        case "loopstartphase":
                            LoopStartPhase = double.Parse(paramValue);
                            break;
                        case "offset":
                            Offset = double.Parse(paramValue);
                            break;
                        case "period":
                            Period = double.Parse(paramValue);
                            break;
                        case "keycenter":
                        case "rootkey":
                            Rootkey = short.Parse(paramValue);
                            break;
                        case "keytrack":
                            KeyTrack = short.Parse(paramValue);
                            break;
                        case "velocitytrack":
                            VelTrack = short.Parse(paramValue);
                            break;
                        case "tune":
                            Tune = short.Parse(paramValue);
                            break;
                    }
                }
            }
        }
        public int Read(BinaryReader reader)
        {
            LoopMethod = (LoopModeEnum)reader.ReadInt16();
            SamplerType = (WaveformEnum)reader.ReadInt16();
            AssetName = IOHelper.Read8BitString(reader, 20);
            EndPhase = reader.ReadDouble();
            StartPhase = reader.ReadDouble();
            LoopEndPhase = reader.ReadDouble();
            LoopStartPhase = reader.ReadDouble();
            Offset = reader.ReadDouble();
            Period = reader.ReadDouble();
            Rootkey = reader.ReadInt16();
            KeyTrack = reader.ReadInt16();
            VelTrack = reader.ReadInt16();
            Tune = reader.ReadInt16();
            return SIZE;
        }
        public int Write(BinaryWriter writer)
        {
            writer.Write((short)LoopMethod);
            writer.Write((short)SamplerType);
            IOHelper.Write8BitString(writer, AssetName, 20);
            writer.Write(EndPhase);
            writer.Write(StartPhase);
            writer.Write(LoopEndPhase);
            writer.Write(LoopStartPhase);
            writer.Write(Offset);
            writer.Write(Period);
            writer.Write(Rootkey);
            writer.Write(KeyTrack);
            writer.Write(VelTrack);
            writer.Write(Tune);
            return SIZE;
        }
        public Generator ToGenerator(AssetManager assets)
        {
            switch (SamplerType)
            {
                case WaveformEnum.SampleData:
                    return new SampleGenerator(this, assets);
                case WaveformEnum.Saw:
                    return new SawGenerator(this);
                case WaveformEnum.Sine:
                    return new SineGenerator(this);
                case WaveformEnum.Square:
                    return new SquareGenerator(this);
                case WaveformEnum.Triangle:
                    return new TriangleGenerator(this);
                case WaveformEnum.WhiteNoise:
                    return new WhiteNoiseGenerator(this);
                default:
                    throw new Exception(string.Format("Unsupported generator: {0}", SamplerType));
            }
        }

        private void ApplyDefault()
        {
            LoopMethod = LoopModeEnum.NoLoop;
            SamplerType = WaveformEnum.Sine;
            AssetName = "null";
            EndPhase = -1;
            StartPhase = -1;
            LoopEndPhase = -1;
            LoopStartPhase = -1;
            Offset = 0;
            Period = -1;
            Rootkey = -1;
            KeyTrack = 100;
            VelTrack = 0;
            Tune = 0;
        }
    }
}