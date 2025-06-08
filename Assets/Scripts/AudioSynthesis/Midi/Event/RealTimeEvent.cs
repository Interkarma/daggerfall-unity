namespace DaggerfallWorkshop.AudioSynthesis.Midi.Event
{
    public class RealTimeEvent : MidiEvent
    {
        public override int Channel
        {
            get { return -1; }
        }
        public override int Command
        {
            get { return message & 0x00000FF; }
        }
        public RealTimeEvent(int delta, byte status, byte data1, byte data2)
            : base(delta, status, data1, data2) { }
    }
}
