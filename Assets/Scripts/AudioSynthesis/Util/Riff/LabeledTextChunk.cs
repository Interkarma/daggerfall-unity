namespace DaggerfallWorkshop.AudioSynthesis.Util.Riff
{
    using System.IO;
    using DaggerfallWorkshop.AudioSynthesis.Util;

    public class LabeledTextChunk : Chunk
    {
        //--Fields
        private int lbltxtCuePointId;
        private int lbltxtSampleLength;
        private int lbltxtPurposeId;
        private short lbltxtCountry;
        private short lbltxtLanguage;
        private short lbltxtDialect;
        private short lbltxtCodePage;
        private string lbltxtText;
        //--Properties
        public int CuePointId
        {
            get { return lbltxtCuePointId; }
        }
        public int SampleLength
        {
            get { return lbltxtSampleLength; }
        }
        public int PurposeId
        {
            get { return lbltxtPurposeId; }
        }
        public short Country
        {
            get { return lbltxtCountry; }
        }
        public short Language
        {
            get { return lbltxtLanguage; }
        }
        public short Dialect
        {
            get { return lbltxtDialect; }
        }
        public short CodePage
        {
            get { return lbltxtCodePage; }
        }
        public string Text
        {
            get { return lbltxtText; }
        }
        //--Methods
        public LabeledTextChunk(string id, int size, BinaryReader reader)
            : base(id, size)
        {
            lbltxtCuePointId = reader.ReadInt32();
            lbltxtSampleLength = reader.ReadInt32();
            lbltxtPurposeId = reader.ReadInt32();
            lbltxtCountry = reader.ReadInt16();
            lbltxtLanguage = reader.ReadInt16();
            lbltxtDialect = reader.ReadInt16();
            lbltxtCodePage = reader.ReadInt16();
            lbltxtText = IOHelper.Read8BitString(reader);
            if (size % 2 == 1 && reader.PeekChar() == 0)
                reader.ReadByte();
        }
    }
}
