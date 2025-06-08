namespace DaggerfallWorkshop.AudioSynthesis.Midi.Event
{
    public class MetaTextEvent: MetaEvent
    {
        private string mText;
        public string Text
        {
            get { return mText; }
        }
        public MetaTextEvent(int delta, byte status, byte metaId, string text)
            : base(delta, status, metaId, 0)
        {
            this.mText = text;
        }
    }
}
