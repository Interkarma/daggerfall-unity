namespace DaggerfallWorkshop.AudioSynthesis.Midi.Event
{
    public class MetaNumberEvent: MetaEvent
    {
        private int mNumber;
        public int Value
        {
            get { return mNumber; }
        }
        public MetaNumberEvent(int delta, byte status, byte metaId, int number)
            : base(delta, status, metaId, 0)
        {
            this.mNumber = number;
        }
    }
}
