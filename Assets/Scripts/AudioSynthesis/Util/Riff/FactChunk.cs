namespace DaggerfallWorkshop.AudioSynthesis.Util.Riff
{
    using System.IO;

    public class FactChunk : Chunk
    {
        //--Fields
        private int factSampleCount; //DWORD
        //--Properties
        public int SampleCount
        {
            get { return factSampleCount; }
        }

        //--Methods
        public FactChunk(string id, int size, BinaryReader reader)
            : base(id, size)
        {
            factSampleCount = reader.ReadInt32();
        }
    }
}
