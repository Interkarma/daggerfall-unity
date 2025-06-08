namespace DaggerfallWorkshop.AudioSynthesis.Util.Riff
{
    using System.IO;

    public class UnknownChunk : Chunk
    {
        //--Fields
        private byte[] data;
        //--Properties
        public byte[] Data
        {
            get { return data; }
        }
        //--Methods
        public UnknownChunk(string id, int size, BinaryReader reader)
            : base(id, size)
        {
            data = reader.ReadBytes(size);
            if (size % 2 == 1 && reader.PeekChar() == 0)
                reader.ReadByte();
        }
    }
}
