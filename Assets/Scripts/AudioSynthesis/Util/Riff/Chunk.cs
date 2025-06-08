namespace DaggerfallWorkshop.AudioSynthesis.Util.Riff
{
    public abstract class Chunk
    {
        //--Fields
        protected string id;
        protected int size;
        //--Properties
        public string ChunkId
        {
            get { return id; }
        }
        public int ChunkSize
        {
            get { return size; }
        }
        //--Methods
        public Chunk(string id, int size)
        {
            this.id = id;
            this.size = size;
        }
    }
}