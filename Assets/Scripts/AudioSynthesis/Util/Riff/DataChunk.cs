namespace DaggerfallWorkshop.AudioSynthesis.Util.Riff
{
    using System.IO;

    public class DataChunk : Chunk
    {
        //--Fields
        private byte[] rawData;
        //--Properties
        public byte[] RawSampleData
        {
            get { return rawData; }
        }
        //--Methods
        public DataChunk(string id, int size, BinaryReader reader)
            : base(id, size)
        {
            rawData = reader.ReadBytes(size);
            if (size % 2 == 1 && reader.PeekChar() == 0)
                reader.ReadByte();
        }
    }
}