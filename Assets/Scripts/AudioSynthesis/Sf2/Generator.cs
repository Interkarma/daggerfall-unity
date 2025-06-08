using System.IO;

namespace DaggerfallWorkshop.AudioSynthesis.Sf2
{
    public class Generator
    {
        private GeneratorEnum gentype;
        private ushort rawAmount;

        public Generator(BinaryReader reader)
        {
            gentype = (GeneratorEnum)reader.ReadUInt16();
            rawAmount = reader.ReadUInt16();
        }
        public GeneratorEnum GeneratorType
        {
            get { return gentype; }
            set { gentype = value; }
        }
        public short AmountInt16
        {
            get { return (short)rawAmount; }
            set { rawAmount = (ushort)value; }
        }
        public byte LowByteAmount
        {
            get
            {
                return (byte)(rawAmount & 0x00FF);
            }
            set
            {
                rawAmount &= 0xFF00;
                rawAmount += value;
            }
        }
        public byte HighByteAmount
        {
            get
            {
                return (byte)((rawAmount & 0xFF00) >> 8);
            }
            set
            {
                rawAmount &= 0x00FF;
                rawAmount += (ushort)(value << 8);
            }
        }
        public override string ToString()
        {
            return string.Format("Generator {0} {1}", gentype, rawAmount);
        }
    }
}
