using DaggerfallWorkshop.AudioSynthesis.Bank.Descriptors;
using DaggerfallWorkshop.AudioSynthesis.Bank.Components.Generators;

namespace DaggerfallWorkshop.AudioSynthesis.Bank.Components
{
    public class Lfo
    {
        private LfoStateEnum lfoState;
        private double phase;
        private double value;
        private double increment;
        private double frequency;
        private double depth;
        private int delayTime;
        private Generator generator;
        
        public double Frequency
        {
            get { return frequency; }
        }
        public LfoStateEnum CurrentState
        {
            get { return lfoState; }
        }
        public double Value
        {
            get { return value; }
            set { this.value = value; }
        }
        public double Depth
        {
            get { return depth; }
            set { depth = value; }
        }

        public void QuickSetup(int sampleRate, LfoDescriptor lfoInfo)
        {
            generator = lfoInfo.Generator;
            delayTime = (int)(sampleRate * lfoInfo.DelayTime);
            frequency = lfoInfo.Frequency;
            increment = generator.Period * frequency / sampleRate;
            depth = lfoInfo.Depth;
            Reset();
        }
        public void Increment(int amount)
        {
            if (lfoState == LfoStateEnum.Delay)
            {
                phase -= amount;
                if (phase <= 0.0)
                {
                    phase = generator.LoopStartPhase + increment * -phase;
                    value = generator.GetValue(phase);
                    lfoState = LfoStateEnum.Sustain;
                }
            }
            else
            {
                phase += increment * amount;
                if (phase >= generator.LoopEndPhase)
                    phase = generator.LoopStartPhase + (phase - generator.LoopEndPhase) % (generator.LoopEndPhase - generator.LoopStartPhase);
                value = generator.GetValue(phase);
            }
        }
        public void Reset()
        {
            value = 0;
            if (delayTime > 0)
            {
                phase = delayTime;
                lfoState = LfoStateEnum.Delay;
            }
            else
            {
                phase = generator.LoopStartPhase;
                lfoState = LfoStateEnum.Sustain;
            }
        }
        public override string ToString()
        {
            return string.Format("State: {0}, Frequency: {1}Hz, Value: {2:0.00}", lfoState, frequency, value);
        }
    }
}
