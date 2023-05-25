using System.IO;
using DaggerfallWorkshop.AudioSynthesis.Midi;

namespace DaggerfallWorkshop.AudioSynthesis.Sf2
{
    public class ModulatorType
    {
        private PolarityEnum polarity;
        private DirectionEnum direction;
        private bool midiContinuousController;
        private SourceTypeEnum sourceType;
        private ushort controllerSource;

        public PolarityEnum Polarity
        {
            get { return polarity; }
            set { polarity = value; }
        }
        public DirectionEnum Direction
        {
            get { return direction; }
            set { direction = value; }
        }
        public SourceTypeEnum SourceType
        {
            get { return sourceType; }
            set { sourceType = value; }
        }

        public ModulatorType(BinaryReader reader)
        {
            ushort raw = reader.ReadUInt16();

            if ((raw & 0x0200) == 0x0200)
                polarity = PolarityEnum.Bipolar;
            else
                polarity = PolarityEnum.Unipolar;
            if ((raw & 0x0100) == 0x0100)
                direction = DirectionEnum.MaxToMin;
            else
                direction = DirectionEnum.MinToMax;
            midiContinuousController = ((raw & 0x0080) == 0x0080);
            sourceType = (SourceTypeEnum)((raw & (0xFC00)) >> 10);
            controllerSource = (ushort)(raw & 0x007F);
        }
        public bool isMidiContinousController()
        {
            return midiContinuousController;
        }

        public override string ToString()
        {
            if (midiContinuousController)
                return string.Format("{0} : {1} : {2} : CC {3}", polarity, direction, sourceType, (ControllerTypeEnum)controllerSource);
            else
                return string.Format("{0} : {1} : {2} : {3}", polarity, direction, sourceType, (ControllerSourceEnum)controllerSource);
        }
    }
}
