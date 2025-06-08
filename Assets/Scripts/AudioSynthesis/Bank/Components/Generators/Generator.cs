using System;
using DaggerfallWorkshop.AudioSynthesis.Bank.Descriptors;

namespace DaggerfallWorkshop.AudioSynthesis.Bank.Components.Generators
{
    public abstract class Generator
    {
        //--Fields
        readonly static internal SineGenerator DefaultSine = new SineGenerator(new GeneratorDescriptor());
        readonly static internal SawGenerator DefaultSaw = new SawGenerator(new GeneratorDescriptor());
        readonly static internal SquareGenerator DefaultSquare = new SquareGenerator(new GeneratorDescriptor());
        readonly static internal TriangleGenerator DefaultTriangle = new TriangleGenerator(new GeneratorDescriptor());

        protected LoopModeEnum loopMethod;
        protected double loopStart;
        protected double loopEnd;
        protected double start;
        protected double end;
        protected double startOffset;
        protected double genPeriod;
        protected double freq;
        protected short root;
        protected short noteTrack;
        protected short velTrack;
        protected short tuneCents;

        //--Properties
        public LoopModeEnum LoopMode 
        { 
            get { return loopMethod; }
            set { loopMethod = value; }
        }
        public double LoopStartPhase 
        { 
            get { return loopStart; }
            set { loopStart = value; }
        }
        public double LoopEndPhase 
        { 
            get { return loopEnd; }
            set { loopEnd = value; }
        }
        public double StartPhase 
        { 
            get { return start; }
            set { start = value; }
        }
        public double EndPhase 
        { 
            get { return end; }
            set { end = value; }
        }
        public double Offset 
        { 
            get { return startOffset; }
            set { startOffset = value; }
        }
        public double Period 
        { 
            get { return genPeriod; }
            set { genPeriod = value; }
        }
        public double Frequency 
        { 
            get { return freq; }
            set { freq = value; }
        }
        public short RootKey 
        { 
            get { return root; }
            set { root = value; }
        }
        public short KeyTrack 
        { 
            get { return noteTrack; }
            set { noteTrack = value; }
        }
        public short VelocityTrack 
        { 
            get { return velTrack; }
            set { velTrack = value; }
        }
        public short Tune 
        { 
            get { return tuneCents; }
            set { tuneCents = value; }
        }

        //--Methods
        public Generator(GeneratorDescriptor description)
        {
            loopMethod = description.LoopMethod;
            loopStart = description.LoopStartPhase;
            loopEnd = description.LoopEndPhase;
            start = description.StartPhase;
            end = description.EndPhase;
            startOffset = description.Offset;
            genPeriod = description.Period;
            root = description.Rootkey;
            noteTrack = description.KeyTrack;
            velTrack = description.VelTrack;
            tuneCents = description.Tune;
        }
        public void Release(GeneratorParameters generatorParams)
        {
            if (loopMethod == LoopModeEnum.LoopUntilNoteOff)
            {
                generatorParams.currentState = GeneratorStateEnum.PostLoop;
                generatorParams.currentStart = start;
                generatorParams.currentEnd = end;
            }
        }
        public abstract float GetValue(double phase);
        public abstract void GetValues(GeneratorParameters generatorParams, float[] blockBuffer, double increment);
        public override string ToString()
        {
            return string.Format("LoopMode: {0}, RootKey: {1}, Period: {2:0.00}", loopMethod, root, genPeriod);
        }

        public static WaveformEnum GetWaveformFromString(string value)
        {
            switch (value.ToLower().Trim())
            {
                case "sine":
                    return WaveformEnum.Sine;
                case "square":
                    return WaveformEnum.Square;
                case "saw":
                case "sawtooth":
                    return WaveformEnum.Saw;
                case "triangle":
                    return WaveformEnum.Triangle;
                case "sample":
                case "sampledata":
                    return WaveformEnum.SampleData;
                case "noise":
                case "whitenoise":
                    return WaveformEnum.WhiteNoise;
                default:
                    throw new Exception("No such waveform: " + value);
            }
        }
        public static InterpolationEnum GetInterpolationFromString(string value)
        {
            switch (value.ToLower())
            {
                case "none":
                    return InterpolationEnum.None;
                case "linear":
                    return InterpolationEnum.Linear;
                case "cosine":
                    return InterpolationEnum.Cosine;
                case "cubic":
                    return InterpolationEnum.CubicSpline;
                default:
                    throw new Exception("No such interpolation: " + value);
            }
        }
        public static LoopModeEnum GetLoopModeFromString(string value)
        {
            switch (value.ToLower())
            {
                case "noloop":
                case "none":
                    return LoopModeEnum.NoLoop;
                case "oneshot":
                    return LoopModeEnum.OneShot;
                case "continuous":
                    return LoopModeEnum.Continuous;
                case "sustain":
                    return LoopModeEnum.LoopUntilNoteOff;
                default:
                    throw new Exception("No such loop mode: " + value);
            }
        }
    }
}
