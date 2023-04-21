namespace DaggerfallWorkshop.AudioSynthesis.Util.Riff
{
    using System.IO;
    using System;

    public class FormatChunk : Chunk
    {
        //--Enum
        public enum CompressionCode 
        { 
            Unknown = 0x0000, 
            Pcm = 0x0001,
            MicrosoftAdpcm = 0x0002,
            IeeeFloat = 0x0003, 
            Alaw = 0x0006, 
            Mulaw = 0x0007, 
            Extensible = 0xFFFE,
            Experimental = 0xFFFF 
        };
        //--Fields
        private int formatCompressionCode; //WORD
        private short formatChannels;     //WORD
        private int formatSampleRate;  //DWORD
        private int formatByteRate; //DWORD
        private short formatBlockAlign; //WORD
        private short formatBitsPerSample; //WORD
        private byte[] formatExtendedData; //extended format fields stored here
        //--Properties
        public CompressionCode FormatCode
        {
            get 
            {
                if (Enum.IsDefined(typeof(CompressionCode), formatCompressionCode))
                    return (CompressionCode)formatCompressionCode;
                return CompressionCode.Unknown;
            }
        }
        public short ChannelCount
        {
            get { return formatChannels; }
        }
        public int SampleRate
        {
            get { return formatSampleRate; }
        }
        public int AverageBytesPerSecond
        {
            get { return formatByteRate; }
        }
        public short BlockAlign
        {
            get { return formatBlockAlign; }
        }
        public short BitsPerSample
        {
            get { return formatBitsPerSample; }
        }
        public byte[] ExtendedData
        {
            get { return formatExtendedData; }
        }
        //--Methods
        public FormatChunk(string id, int size, BinaryReader reader)
            : base(id, size)
        {
            formatCompressionCode = reader.ReadUInt16();
            formatChannels = reader.ReadInt16();
            formatSampleRate = reader.ReadInt32();
            formatByteRate = reader.ReadInt32();
            formatBlockAlign = reader.ReadInt16();
            formatBitsPerSample = reader.ReadInt16();
            if (size > 16 && formatCompressionCode > (int)CompressionCode.Pcm)
            {
                formatExtendedData = new byte[reader.ReadInt16()]; //read cb size
                reader.Read(formatExtendedData, 0, formatExtendedData.Length);
                if (formatExtendedData.Length % 2 == 1 && reader.PeekChar() == 0)
                    reader.ReadByte();
            }
        }
    }
}
