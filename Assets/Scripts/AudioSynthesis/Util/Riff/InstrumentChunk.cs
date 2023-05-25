namespace DaggerfallWorkshop.AudioSynthesis.Util.Riff
{
    using System.IO;

    public class InstrumentChunk : Chunk
    {
        //--Fields
        private byte instNote;
        private sbyte instFineTune;
        private sbyte instGain;
        private byte instLowNote;
        private byte instHighNote;
        private byte instLowVelocity;
        private byte instHighVelocity;
        //--Properties
        public byte Note
        {
            get { return instNote; }
        }
        public int FineTuneCents
        {
            get { return instFineTune; }
        }
        public double Gain
        {
            get { return instGain; }
        }
        public byte LowNote
        {
            get { return instLowNote; }
        }
        public byte HighNote
        {
            get { return instHighNote; }
        }
        public byte LowVelocity
        {
            get { return instLowVelocity; }
        }
        public byte HighVelocity
        {
            get { return instHighVelocity; }
        }
        //--Methods
        public InstrumentChunk(string id, int size, BinaryReader reader)
            : base(id, size)
        {
            instNote = reader.ReadByte();
            instFineTune = reader.ReadSByte();
            instGain = reader.ReadSByte();
            instLowNote = reader.ReadByte();
            instHighNote = reader.ReadByte();
            instLowVelocity = reader.ReadByte();
            instHighVelocity = reader.ReadByte();
            reader.ReadByte(); //always read pad
        }
    }
}
