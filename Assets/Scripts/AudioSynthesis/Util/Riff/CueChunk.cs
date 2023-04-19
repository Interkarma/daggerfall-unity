namespace DaggerfallWorkshop.AudioSynthesis.Util.Riff
{
    using System.IO;
    using System.Collections.Generic;
    using DaggerfallWorkshop.AudioSynthesis.Util;

    public class CueChunk : Chunk
    {
        //--Fields
        private CuePoint[] cues;
        //--Properties
        public IList<CuePoint> CuePoints
        {
            get { return cues; }
        }
        //--Methods
        public CueChunk(string id, int size, BinaryReader reader)
            : base(id, size)
        {
            cues = new CuePoint[reader.ReadInt32()];
            for (int x = 0; x < cues.Length; x++)
            {
                cues[x] = new CuePoint(reader);
            }
        }
        //--Internal classes and structs
        public class CuePoint
        {
            private int cueId;
            private int cuePosition;
            private string cueDataChunkId;
            private int cueChunkStart;
            private int cueBlockStart;
            private int cueSampleOffset;

            public int Id
            {
                get { return cueId; }
            }
            public int Position
            {
                get { return cuePosition; }
            }
            public string DataChunkId
            {
                get { return cueDataChunkId; }
            }
            public int ChunkStart
            {
                get { return cueChunkStart; }
            }
            public int BlockStart
            {
                get { return cueBlockStart; }
            }
            public int SampleOffset
            {
                get { return cueSampleOffset; }
            }

            public CuePoint(BinaryReader reader)
            {
                this.cueId = reader.ReadInt32();
                this.cuePosition = reader.ReadInt32();
                this.cueDataChunkId = new string(IOHelper.Read8BitChars(reader, 4));
                this.cueChunkStart = reader.ReadInt32();
                this.cueBlockStart = reader.ReadInt32();
                this.cueSampleOffset = reader.ReadInt32();
            }
        }
    }
}