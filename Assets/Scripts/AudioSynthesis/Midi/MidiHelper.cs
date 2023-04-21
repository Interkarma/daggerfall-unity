namespace DaggerfallWorkshop.AudioSynthesis.Midi
{
    //structs and enum
    public enum MidiEventTypeEnum
    {
        NoteOff = 0x80,
        NoteOn = 0x90,
        NoteAftertouch = 0xA0,
        Controller = 0xB0,
        ProgramChange = 0xC0,
        ChannelAftertouch = 0xD0,
        PitchBend = 0xE0
    }
    public enum MetaEventTypeEnum
    {
        SequenceNumber = 0x00,
        TextEvent = 0x01,
        CopyrightNotice = 0x02,
        SequenceOrTrackName = 0x03,
        InstrumentName = 0x04,
        LyricText = 0x05,
        MarkerText = 0x06,
        CuePoint = 0x07,
        MidiChannel = 0x20,
        MidiPort = 0x21,
        EndOfTrack = 0x2F,
        Tempo = 0x51,
        SmpteOffset = 0x54,
        TimeSignature = 0x58,
        KeySignature = 0x59,
        SequencerSpecific = 0x7F
    }
    public enum SystemCommonTypeEnum
    {
        SystemExclusive = 0xF0,
        MtcQuarterFrame = 0xF1,
        SongPosition = 0xF2,
        SongSelect = 0xF3,
        TuneRequest = 0xF6
    }
    public enum ControllerTypeEnum
    {
        BankSelectCoarse = 0x00,
        ModulationCoarse = 0x01,
        BreathControllerCoarse = 0x02,
        FootControllerCoarse = 0x04,
        PortamentoTimeCoarse = 0x05,
        DataEntryCoarse = 0x06,
        VolumeCoarse = 0x07,
        BalanceCoarse = 0x08,
        PanCoarse = 0x0A,
        ExpressionControllerCoarse = 0x0B,
        EffectControl1Coarse = 0x0C,
        EffectControl2Coarse = 0x0D,
        GeneralPurposeSlider1 = 0x10,
        GeneralPurposeSlider2 = 0x11,
        GeneralPurposeSlider3 = 0x12,
        GeneralPurposeSlider4 = 0x13,
        BankSelectFine = 0x20,
        ModulationFine = 0x21,
        BreathControllerFine = 0x22,
        FootControllerFine = 0x24,
        PortamentoTimeFine = 0x25,
        DataEntryFine = 0x26,
        VolumeFine = 0x27,
        BalanceFine = 0x28,
        PanFine = 0x2A,
        ExpressionControllerFine = 0x2B,
        EffectControl1Fine = 0x2C,
        EffectControl2Fine = 0x2D,
        HoldPedal = 0x40,
        Portamento = 0x41,
        SostenutoPedal = 0x42,
        SoftPedal = 0x43,
        LegatoPedal = 0x44,
        Hold2Pedal = 0x45,
        SoundVariation = 0x46,
        SoundTimbre = 0x47,
        SoundReleaseTime = 0x48,
        SoundAttackTime = 0x49,
        SoundBrightness = 0x4A,
        SoundControl6 = 0x4B,
        SoundControl7 = 0x4C,
        SoundControl8 = 0x4D,
        SoundControl9 = 0x4E,
        SoundControl10 = 0x4F,
        GeneralPurposeButton1 = 0x50,
        GeneralPurposeButton2 = 0x51,
        GeneralPurposeButton3 = 0x52,
        GeneralPurposeButton4 = 0x53,
        EffectsLevel = 0x5B,
        TremuloLevel = 0x5C,
        ChorusLevel = 0x5D,
        CelesteLevel = 0x5E,
        PhaseLevel = 0x5F,
        DataButtonIncrement = 0x60,
        DataButtonDecrement = 0x61,
        NonRegisteredParameterFine = 0x62,
        NonRegisteredParameterCourse = 0x63,
        RegisteredParameterFine = 0x64,
        RegisteredParameterCourse = 0x65,
        AllSoundOff = 0x78,
        ResetControllers = 0x79,
        LocalKeyboard = 0x7A,
        AllNotesOff = 0x7B,
        OmniModeOff = 0x7C,
        OmniModeOn = 0x7D,
        MonoMode = 0x7E,
        PolyMode = 0x7F
    }

    //static helper methods and constants
    public static class MidiHelper
    {
        //--Constants
        public const int MicroSecondsPerMinute = 60000000; //microseconds in a minute
        public const int MinChannel = 0;
        public const int MaxChannel = 15;
        public const int DrumChannel = 9;
    }
}
