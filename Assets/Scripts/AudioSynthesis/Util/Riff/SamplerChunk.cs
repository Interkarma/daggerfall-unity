namespace DaggerfallWorkshop.AudioSynthesis.Util.Riff
{
    using System;
    using System.IO;

    public class SamplerChunk : Chunk
    {
        //--Fields
        private int smplManufacturer;
        private int smplProduct;
        private int smplSamplePeriod;
        private int smplMidiUnityNote;
        private uint smplMidiPitchFraction;
        private int smplSmpteFormat;
        private int smplSmpteOffset;
        private SampleLoop[] smplLoops;
        private byte[] smplData;
        //--Properties
        public int Manufacturer
        {
            get { return smplManufacturer; }
        }
        public int Product
        {
            get { return smplProduct; }
        }
        public int SamplePeriod
        {
            get { return smplSamplePeriod; }
        }
        public int UnityNote
        {
            get { return smplMidiUnityNote; }
        }
        public double PitchFraction
        {
            get { return (smplMidiPitchFraction / (double)0x80000000) / 2.0; }
        }
        public int SmpteFormat
        {
            get { return smplSmpteFormat; }
        }
        public int SmpteOffset
        {
            get { return smplSmpteOffset; }
        }
        public SampleLoop[] Loops
        {
            get { return smplLoops; }
        }
        public byte[] Data
        {
            get { return smplData; }
        }
        //--Methods
        public SamplerChunk(string id, int size, BinaryReader reader)
            : base(id, size)
        {
            smplManufacturer = reader.ReadInt32();
            smplProduct = reader.ReadInt32();
            smplSamplePeriod = reader.ReadInt32();
            smplMidiUnityNote = reader.ReadInt32();
            smplMidiPitchFraction = reader.ReadUInt32();
            smplSmpteFormat = reader.ReadInt32();
            smplSmpteOffset = reader.ReadInt32();
            int smplSampleLoops = reader.ReadInt32();
            int smplSamplerData = reader.ReadInt32();
            smplLoops = new SampleLoop[smplSampleLoops];
            for (int x = 0; x < smplLoops.Length; x++)
            {
                smplLoops[x] = new SampleLoop(reader);
            }
            smplData = reader.ReadBytes(smplSamplerData);
            if (size % 2 == 1 && reader.PeekChar() == 0)
                reader.ReadByte();
        }
        //--Internal classes and structs
        public struct SampleLoop
        {
            public enum LoopType { Forward = 0, Alternating = 1, Reverse = 2, Unknown = 32}
            //--Fields
            private int sloopCuePointId;
            private int sloopType;
            private int sloopStart;
            private int sloopEnd;
            private uint sloopFraction;
            private int sloopPlayCount;
            //--Properties
            public int CuePointId
            {
                get { return sloopCuePointId; }
            }
            public LoopType Type
            {
                get 
                {
                    if (Enum.IsDefined(typeof(LoopType), sloopType))
                        return (LoopType)sloopType;
                    return LoopType.Unknown;
                }
            }
            public int Start
            {
                get { return sloopStart; }
            }
            public int End
            {
                get { return sloopEnd; }
            }
            public double Fraction
            {
                get { return (sloopFraction / (double)0x80000000) / 2.0; }
            }
            public int Count
            {
                get { return sloopPlayCount; }
            }
            //--Methods
            public SampleLoop(BinaryReader reader)
            {
                sloopCuePointId = reader.ReadInt32();
                sloopType = reader.ReadInt32();
                sloopStart = reader.ReadInt32();
                sloopEnd = reader.ReadInt32();
                sloopFraction = reader.ReadUInt32();
                sloopPlayCount = reader.ReadInt32();
            }
        }
    }
}
