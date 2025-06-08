namespace DaggerfallWorkshop.AudioSynthesis.Midi.Event
{
    public class MetaDataEvent: MetaEvent
    {
        private byte[] mdata;
        public byte[] Data
        {
            get { return mdata; }
        }
        public MetaDataEvent(int delta, byte status, byte metaId, byte[] data)
            : base(delta, status, metaId, 0)
        {
            this.mdata = data;
        }
    }
}
