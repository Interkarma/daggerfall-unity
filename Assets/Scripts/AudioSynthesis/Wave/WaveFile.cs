namespace DaggerfallWorkshop.AudioSynthesis.Wave
{
    using DaggerfallWorkshop.AudioSynthesis.Util.Riff;

    public class WaveFile
    {
        //--Fields
        private Chunk[] chks;
        private DataChunk dataChk;
        private FormatChunk fmtChk;
        //--Properties
        public DataChunk Data
        {
            get { return dataChk; }
        }
        public FormatChunk Format
        {
            get { return fmtChk; }
        }
        public Chunk[] Chunks
        {
            get { return chks; }
        }
        //--Methods
        public WaveFile(Chunk[] chunks)
        {
            this.chks = chunks;
            this.dataChk = FindChunk<DataChunk>();
            this.fmtChk = FindChunk<FormatChunk>();
        }
        public T FindChunk<T>(int startIndex = 0) where T : Chunk
        {
            for (int x = startIndex; x < chks.Length; x++)
            {
                if (chks[x] is T)
                    return (T)chks[x];
            }
            return default(T);
        }
    }
}
