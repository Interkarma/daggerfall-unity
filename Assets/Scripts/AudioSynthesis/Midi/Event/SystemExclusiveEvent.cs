namespace DaggerfallWorkshop.AudioSynthesis.Midi.Event
{
    public class SystemExclusiveEvent : SystemCommonEvent
    {
        private byte[] mdata;
        public byte[] Data
        {
            get { return mdata; }
        }
        public int ManufacturerId
        {
            get { return message >> 8; }
        }
        public SystemExclusiveEvent(int delta, byte status, short id, byte[] data)
            : base(delta, status, (byte)(id & 0x00FF) , (byte)(id >> 8))
        {
            this.mdata = data;
        }
    }
}
