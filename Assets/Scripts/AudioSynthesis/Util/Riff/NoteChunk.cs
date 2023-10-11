namespace DaggerfallWorkshop.AudioSynthesis.Util.Riff
{
    using System.IO;
    using DaggerfallWorkshop.AudioSynthesis.Util;

    public class NoteChunk : Chunk
    {
        //--Fields
        private int noteCuePointId;
        private string noteText;
        //--Properties
        public int CuePointId
        {
            get { return noteCuePointId; }
        }
        public string Text
        {
            get { return noteText; }
        }
        //--Methods
        public NoteChunk(string id, int size, BinaryReader reader)
            : base(id, size)
        {
            noteCuePointId = reader.ReadInt32();
            noteText = IOHelper.Read8BitString(reader);
            if (size % 2 == 1 && reader.PeekChar() == 0)
                reader.ReadByte();
        }
    }
}
