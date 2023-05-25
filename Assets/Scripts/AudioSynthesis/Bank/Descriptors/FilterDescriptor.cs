namespace DaggerfallWorkshop.AudioSynthesis.Bank.Descriptors
{
    using System;
    using System.IO;
    using DaggerfallWorkshop.AudioSynthesis.Bank.Components;
    using DaggerfallWorkshop.AudioSynthesis.Synthesis;

    public class FilterDescriptor : IDescriptor
    {
        public const string ID = "fltr";
        public const int SIZE = 16;

        public FilterTypeEnum FilterMethod;
        public float CutOff;
        public float Resonance;
        public short RootKey;
        public short KeyTrack;
        public short VelTrack;

        public FilterDescriptor()
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
                        case "type":
                            FilterMethod = GetFilterType(paramValue.ToLower());
                            break;
                        case "cutoff":
                            CutOff = float.Parse(paramValue);
                            break;
                        case "resonance":
                            Resonance = float.Parse(paramValue);
                            break;
                        case "keycenter":
                            RootKey = short.Parse(paramValue);
                            break;
                        case "keytrack":
                            KeyTrack = short.Parse(paramValue);
                            break;
                        case "velocitytrack":
                            VelTrack = short.Parse(paramValue);
                            break;
                    }
                }
            }
            CheckValidParameters();
        }
        public int Read(BinaryReader reader)
        {
            FilterMethod = (FilterTypeEnum)reader.ReadInt16();
            CutOff = reader.ReadSingle();
            Resonance = reader.ReadSingle();
            RootKey = reader.ReadInt16();
            KeyTrack = reader.ReadInt16();
            VelTrack = reader.ReadInt16();
            CheckValidParameters();
            return SIZE;
        }
        public int Write(BinaryWriter writer)
        {
            writer.Write((short)FilterMethod);
            writer.Write(CutOff);
            writer.Write(Resonance);
            writer.Write(RootKey);
            writer.Write(KeyTrack);
            writer.Write(VelTrack);
            return SIZE;
        }

        private static FilterTypeEnum GetFilterType(string value)
        {
            switch (value.ToLower())
            {
                case "lowpass":
                case "onepolelowpass":
                    return FilterTypeEnum.OnePoleLowpass;
                case "biquadlowpass":
                    return FilterTypeEnum.BiquadLowpass;
                case "biquadhighpass":
                    return FilterTypeEnum.BiquadHighpass;
                case "none":
                    return FilterTypeEnum.None;
                default:
                    throw new Exception("Unknown filter type: " + value);
            }
        }
        private void ApplyDefault()
        {
            FilterMethod = FilterTypeEnum.None;
            CutOff = -1;
            Resonance = 1;
            RootKey = 60;
            KeyTrack = 0;
            VelTrack = 0;
        }
        private void CheckValidParameters()
        {
            //limit cutoff
            if (CutOff <= 0)
                FilterMethod = FilterTypeEnum.None;
            if (RootKey < 0 || RootKey > 127)
                RootKey = 60;
            KeyTrack = SynthHelper.Clamp(KeyTrack, (short)0, (short)1200);
            VelTrack = SynthHelper.Clamp(VelTrack, (short)-9600, (short)9600);
        }
        
    }
}
