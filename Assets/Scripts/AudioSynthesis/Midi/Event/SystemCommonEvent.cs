namespace DaggerfallWorkshop.AudioSynthesis.Midi.Event
{
    using System;

    public class SystemCommonEvent : MidiEvent
    {
        public override int Channel
        {//SystemCommon messages have no channel
            get { return -1; }
        }
        public override int Command
        {
            get { return message & 0x00000FF; }
        }
        public SystemCommonEvent(int delta, byte status, byte data1, byte data2)
            : base(delta, status, data1, data2) { }
        public override string ToString()
        {
            return "SystemCommon: " + Enum.GetName(typeof(SystemCommonTypeEnum), Command);
        }
    }
}
