namespace DaggerfallWorkshop.AudioSynthesis.Bank.Components
{
    public enum GeneratorStateEnum { PreLoop, Loop, PostLoop, Finished };
    public enum EnvelopeStateEnum { Delay, Attack, Hold, Decay, Sustain, Release, None };
    public enum LfoStateEnum { Delay, Sustain};
    public enum WaveformEnum { Sine, Square, Saw, Triangle, SampleData, WhiteNoise };
    public enum InterpolationEnum { None, Linear, Cosine, CubicSpline };
    public enum LoopModeEnum { NoLoop, OneShot, Continuous, LoopUntilNoteOff };
    public enum FilterTypeEnum { None, BiquadLowpass, BiquadHighpass, OnePoleLowpass };
}
