namespace DaggerfallWorkshop.AudioSynthesis.Util.Riff
{
    using System.IO;
    using System.Collections.Generic;

    public class PlaylistChunk : Chunk
    {
        //--Fields
        private Segment[] segments;
        //--Properties
        public IList<Segment> SegmentList
        {
            get { return segments; }
        }
        //--Methods
        public PlaylistChunk(string id, int size, BinaryReader reader)
            : base(id, size)
        {
            int segCount = reader.ReadInt32();
            segments = new Segment[segCount];
            for (int x = 0; x < segments.Length; x++)
            {
                segments[x] = new Segment(reader);
            }
        }
        //--Internal classes and structs
        public class Segment
        {
            //--Fields
            private int segmentCuePointId;
            private int segmentLength;
            private int segmentRepeats;
            //--Properties
            public int CuePointId
            {
                get { return segmentCuePointId; }
            }
            public int SampleLength
            {
                get { return segmentLength; }
            }
            public int RepeatCount
            {
                get { return segmentRepeats; }
            }
            //--Methods
            public Segment(BinaryReader reader)
            {
                segmentCuePointId = reader.ReadInt32();
                segmentLength = reader.ReadInt32();
                segmentRepeats = reader.ReadInt32();
            }
        }
    }
}
