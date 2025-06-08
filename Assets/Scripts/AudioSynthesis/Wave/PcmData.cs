using System;

namespace DaggerfallWorkshop.AudioSynthesis.Wave
{
    public abstract class PcmData
    {
        protected byte[] data;
        protected byte bytes;
        protected int length;

        public int Length { get { return length; } }
        public int BytesPerSample { get { return bytes; } }
        public int BitsPerSample { get { return bytes * 8; } }

        protected PcmData(int bits, byte[] pcmData, bool isDataInLittleEndianFormat)
        {
            bytes = (byte)(bits / 8);
            if (pcmData.Length % bytes != 0)
                throw new Exception("Invalid PCM format. The PCM data was an invalid size.");
            data = pcmData;
            length = data.Length / bytes;
            if (BitConverter.IsLittleEndian != isDataInLittleEndianFormat)
                WaveHelper.SwapEndianess(data, bits);
        }
        public abstract float this[int index] { get; }

        public static PcmData Create(int bits, byte[] pcmData, bool isDataInLittleEndianFormat)
        {
            switch (bits)
            {
                case 8:
                    return new PcmData8Bit(bits,pcmData,isDataInLittleEndianFormat);
                case 16:
                    return new PcmData16Bit(bits, pcmData, isDataInLittleEndianFormat);
                case 24:
                    return new PcmData24Bit(bits,pcmData,isDataInLittleEndianFormat);
                case 32:
                    return new PcmData32Bit(bits,pcmData,isDataInLittleEndianFormat);
                default:
                    throw new Exception("Invalid PCM format. " + bits + "bit pcm data is not supported.");
            }
        }
    }
    public class PcmData8Bit : PcmData
    {
        public PcmData8Bit(int bits, byte[] pcmData, bool isDataInLittleEndianFormat) : base(bits, pcmData, isDataInLittleEndianFormat) { }
        public override float this[int index]
        {
            get { return ((data[index] / 255f) * 2f) - 1f; }
        }
    }
    public class PcmData16Bit : PcmData
    {
        public PcmData16Bit(int bits, byte[] pcmData, bool isDataInLittleEndianFormat) : base(bits, pcmData, isDataInLittleEndianFormat) { }
        public override float this[int index]
        {
            get { index *= 2; return (((data[index] | (data[index + 1] << 8)) << 16) >> 16) / 32768f; }
        }
    }
    public class PcmData24Bit : PcmData
    {
        public PcmData24Bit(int bits, byte[] pcmData, bool isDataInLittleEndianFormat) : base(bits, pcmData, isDataInLittleEndianFormat) { }
        public override float this[int index]
        {
            get { index *= 3; return (((data[index] | (data[index + 1] << 8) | (data[index + 2] << 16)) << 12) >> 12) / 8388608f; }
        }
    }
    public class PcmData32Bit : PcmData
    {
        public PcmData32Bit(int bits, byte[] pcmData, bool isDataInLittleEndianFormat) : base(bits, pcmData, isDataInLittleEndianFormat) { }
        public override float this[int index]
        {
            get { index *= 4; return (data[index] | (data[index + 1] << 8) | (data[index + 2] << 16) | (data[index + 3] << 24)) / 2147483648f; }
        }
    }

}
