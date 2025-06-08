using System.IO;
using DaggerfallWorkshop.AudioSynthesis.Util;

namespace DaggerfallWorkshop.AudioSynthesis.Sf2
{
    public class SampleHeader
    {
        private string sampleName;
        private uint start;
        private uint end;
        private uint startLoop;
        private uint endLoop;
        private uint sampleRate;
        private byte originalPitch;
        private sbyte pitchCorrection;
        private ushort sampleLink;
        private SFSampleLink soundFontSampleLink;

        public string Name
        {
            get { return sampleName; }
        }
        public int Start
        {
            get { return (int)start; }
        }
        public int End
        {
            get { return (int)end; }
        }
        public int StartLoop
        {
            get { return (int)startLoop; }
        }
        public int EndLoop
        {
            get { return (int)endLoop; }
        }
        public int SampleRate
        {
            get { return (int)sampleRate; }
        }
        public byte RootKey
        {
            get { return originalPitch; }
        }
        public short Tune
        {
            get { return pitchCorrection; }
        }

        public SampleHeader(BinaryReader reader)
        {
            sampleName = IOHelper.Read8BitString(reader, 20);
            start = reader.ReadUInt32();
            end = reader.ReadUInt32();
            startLoop = reader.ReadUInt32();
            endLoop = reader.ReadUInt32();
            sampleRate = reader.ReadUInt32();
            originalPitch = reader.ReadByte();
            pitchCorrection = reader.ReadSByte();
            sampleLink = reader.ReadUInt16();
            soundFontSampleLink = (SFSampleLink)reader.ReadUInt16();
        }

        public override string ToString()
        {
            return sampleName;
        }
    }
}
