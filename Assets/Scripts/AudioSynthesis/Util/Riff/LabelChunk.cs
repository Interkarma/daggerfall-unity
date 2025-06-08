namespace DaggerfallWorkshop.AudioSynthesis.Util.Riff
{
    using System.IO;
    using DaggerfallWorkshop.AudioSynthesis.Util;

    public class LabelChunk : Chunk
    {
        //--Fields
        private int lblCuePointId;
        private string lblText;
        //--Properties
        public int CuePointId
        {
            get { return lblCuePointId; }
        }
        public string Text
        {
            get { return lblText; }
        }
        //--Methods
        public LabelChunk(string id, int size, BinaryReader reader)
            : base(id, size)
        {
            lblCuePointId = reader.ReadInt32();
            lblText = IOHelper.Read8BitString(reader);
            if (size % 2 == 1 && reader.PeekChar() == 0)
                reader.ReadByte();
        }
    }
}
