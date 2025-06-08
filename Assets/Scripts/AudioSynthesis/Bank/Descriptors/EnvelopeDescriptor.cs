namespace DaggerfallWorkshop.AudioSynthesis.Bank.Descriptors
{
    using System;
    using System.IO;
    using DaggerfallWorkshop.AudioSynthesis.Synthesis;

    public class EnvelopeDescriptor : IDescriptor
    {
        public const string ID = "envp";
        public const int SIZE = 74;

        public float DelayTime;    //delay time (a time before the attack section that holds the amplitude at zero)
        public float AttackTime;   //attack time (the amount of time taken to reach the peak level from zero)
        public short AttackGraph;
        public float HoldTime;     //hold time (the amount of time to stay at peak level)
        public float DecayTime;    //decay time (the amount of time taken to reach the sustain level)
        public short DecayGraph;
        public float SustainTime;  //sustain time (the amount of time to stay at sustain level)
        public float ReleaseTime;  //release time (the amount of time taken to reach zero from the current amplitude)
        public short ReleaseGraph;
        public float SustainLevel;  //the amplitude value for the sustain section
        public float PeakLevel;     //the amplitude value for the attack section
        public float StartLevel;
        public float Depth;
        public float Vel2Delay;
        public float Vel2Attack;
        public float Vel2Hold;
        public float Vel2Decay;
        public float Vel2Sustain;
        public float Vel2Release;
        public float Vel2Depth;

        public EnvelopeDescriptor()
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
                        case "attacktime":
                            AttackTime = float.Parse(paramValue);
                            break;
                        case "attackgraph":
                            AttackGraph = GetGraphID(paramValue.ToLower());
                            break;
                        case "holdtime":
                            HoldTime = float.Parse(paramValue);
                            break;
                        case "decaytime":
                            DecayTime = float.Parse(paramValue);
                            break;
                        case "decaygraph":
                            DecayGraph = GetGraphID(paramValue.ToLower());
                            break;
                        case "sustaintime":
                            SustainTime = float.Parse(paramValue);
                            break;
                        case "releasetime":
                            ReleaseTime = float.Parse(paramValue);
                            break;
                        case "releasegraph":
                            ReleaseGraph = GetGraphID(paramValue.ToLower());
                            break;
                        case "sustainlevel":
                            SustainLevel = float.Parse(paramValue);
                            break;
                        case "attacklevel":
                            PeakLevel = float.Parse(paramValue);
                            break;
                        case "startlevel":
                            StartLevel = float.Parse(paramValue);
                            break;
                        case "depth":
                            Depth = float.Parse(paramValue);
                            break;
                        case "vel2delay":
                            Vel2Delay = float.Parse(paramValue);
                            break;
                        case "vel2attack":
                            Vel2Attack = float.Parse(paramValue);
                            break;
                        case "vel2hold":
                            Vel2Hold = float.Parse(paramValue);
                            break;
                        case "vel2decay":
                            Vel2Decay = float.Parse(paramValue);
                            break;
                        case "vel2sustain":
                            Vel2Sustain = float.Parse(paramValue);
                            break;
                        case "vel2release":
                            Vel2Release = float.Parse(paramValue);
                            break;
                        case "vel2depth":
                            Vel2Depth = float.Parse(paramValue);
                            break;
                    }
                }
            }
            CheckValidParameters();
        }
        public int Read(BinaryReader reader)
        {
            DelayTime = reader.ReadSingle();
            AttackTime = reader.ReadSingle();
            AttackGraph = reader.ReadInt16();
            HoldTime = reader.ReadSingle();
            DecayTime = reader.ReadSingle();
            DecayGraph = reader.ReadInt16();
            SustainTime = reader.ReadSingle();
            ReleaseTime = reader.ReadSingle();
            ReleaseGraph = reader.ReadInt16();
            SustainLevel = reader.ReadSingle();
            PeakLevel = reader.ReadSingle();
            StartLevel = reader.ReadSingle();
            Depth = reader.ReadSingle();
            Vel2Delay = reader.ReadSingle();
            Vel2Attack = reader.ReadSingle();
            Vel2Hold = reader.ReadSingle();
            Vel2Decay = reader.ReadSingle();
            Vel2Sustain = reader.ReadSingle();
            Vel2Release = reader.ReadSingle();
            Vel2Depth = reader.ReadSingle();
            CheckValidParameters();
            return SIZE;
        }
        public int Write(BinaryWriter writer)
        {
            writer.Write(DelayTime);
            writer.Write(AttackTime);
            writer.Write(AttackGraph);
            writer.Write(HoldTime);
            writer.Write(DecayTime);
            writer.Write(DecayGraph);
            writer.Write(SustainTime);
            writer.Write(ReleaseTime);
            writer.Write(ReleaseGraph);
            writer.Write(SustainLevel);
            writer.Write(PeakLevel);
            writer.Write(StartLevel);
            writer.Write(Depth);
            writer.Write(Vel2Delay);
            writer.Write(Vel2Attack);
            writer.Write(Vel2Hold);
            writer.Write(Vel2Decay);
            writer.Write(Vel2Sustain);
            writer.Write(Vel2Release);
            writer.Write(Vel2Depth);
            return SIZE;
        }

        private static short GetGraphID(string value)
        {
            switch (value)
            {
                case "constant":
                case "straight":
                case "sustain":
                    return 0;
                case "lin":
                case "linear":
                    return 1;
                case "concave":
                    return 2;
                case "convex":
                    return 3;
                default:
                    throw new Exception("Unsupported envelope mode: " + value);
            }
        }

        private void ApplyDefault()
        {
            DelayTime = 0f;
            AttackTime = 0f;
            AttackGraph = 1;
            HoldTime = 0f;
            DecayTime = 0f;
            DecayGraph = 1;
            SustainTime = 3600f;
            ReleaseTime = 0f;
            ReleaseGraph = 1;
            SustainLevel = 0f;
            PeakLevel = 1f;
            StartLevel = 0f;
            Depth = 1f;
            Vel2Delay = 0f;
            Vel2Attack = 0f;
            Vel2Hold = 0f;
            Vel2Decay = 0f;
            Vel2Sustain = 0f;
            Vel2Release = 0f;
            Vel2Depth = 0f;
        }
        private void CheckValidParameters()
        {
            //release must be with min and max
            ReleaseTime = SynthHelper.Clamp(ReleaseTime, 0.001f, 2.0f);
            //positive only checks
            DelayTime = Math.Max(DelayTime, 0f);
            AttackTime = Math.Max(AttackTime, 0f);
            HoldTime = Math.Max(HoldTime, 0f);
            DecayTime = Math.Max(DecayTime, 0f);
            SustainTime = Math.Max(SustainTime, 0);
            ReleaseTime = Math.Max(ReleaseTime, 0f);
            SustainLevel = Math.Max(SustainLevel, 0f);
            PeakLevel = Math.Max(PeakLevel, 0f);
            StartLevel = SynthHelper.Clamp(StartLevel, 0, PeakLevel);
        }
    }
}
