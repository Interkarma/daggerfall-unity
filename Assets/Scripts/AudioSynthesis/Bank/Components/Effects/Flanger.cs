namespace DaggerfallWorkshop.AudioSynthesis.Bank.Components.Effects
{
    using System;
    using DaggerfallWorkshop.AudioSynthesis.Bank.Descriptors;
    using DaggerfallWorkshop.AudioSynthesis.Synthesis;
    
    public class Flanger : IAudioEffect
    {
        private Lfo lfo;
        private float fBack;
        private float wMix;
        private float dMix;
        private int baseDelay;
        private int minDelay;
        private float[] inputBuffer1;
        private float[] outputBuffer1;
        private int position1;
        private float[] inputBuffer2;
        private float[] outputBuffer2;
        private int position2;

        public Lfo Lfo
        {
            get { return lfo; }
            set { lfo = value; }
        }
        public float FeedBack
        {
            get { return fBack; }
            set { fBack = value; }
        }
        public float WetMix
        {
            get { return wMix; }
            set { wMix = value; }
        }
        public float DryMix
        {
            get { return dMix; }
            set { dMix = value; }
        }

        public Flanger(int sampleRate, double minDelay, double maxDelay)
        {
            if (minDelay > maxDelay)
            {
                double m = minDelay;
                minDelay = maxDelay;
                maxDelay = m;
            }
            LfoDescriptor description = new LfoDescriptor();
            this.lfo = new Lfo();
            this.lfo.QuickSetup(sampleRate, description);

            this.baseDelay = (int)(sampleRate * (maxDelay - minDelay));
            this.minDelay = (int)(sampleRate * minDelay);

            int size = (int)(sampleRate * maxDelay) + 1;
            this.inputBuffer1 = new float[size];
            this.outputBuffer1 = new float[size];
            this.position1 = 0;

            this.inputBuffer2 = new float[size];
            this.outputBuffer2 = new float[size];
            this.position2 = 0;

            this.fBack = .15f;
            this.wMix = .5f;
            this.dMix = .5f;
        }
        public void ApplyEffect(float[] source)
        {
            for (int x = 0; x < source.Length; x++)
            {
                lfo.Increment(1);
                int index = position1 - (int)(baseDelay * (.5 * lfo.Value + .5) + minDelay);
                
                if(index < 0)
                    index += inputBuffer1.Length;

                inputBuffer1[position1] = source[x];
                outputBuffer1[position1] = dMix * inputBuffer1[position1] + wMix * inputBuffer1[index] + fBack * outputBuffer1[index];
                source[x] = outputBuffer1[position1++];

                if (position1 == inputBuffer1.Length)
                    position1 = 0;
            }
        }
        public void ApplyEffect(float[] source1, float[] source2)
        {
            for (int x = 0, index; x < source1.Length; x++)
            {
                lfo.Increment(1);
                double lfoValue = (.5 * lfo.Value + .5);
                //source 1
                index = position1 - (int)(baseDelay * lfoValue + minDelay);
                if (index < 0)
                    index += inputBuffer1.Length;
                inputBuffer1[position1] = source1[x];
                outputBuffer1[position1] = dMix * inputBuffer1[position1] + wMix * inputBuffer1[index] + fBack * outputBuffer1[index];
                source1[x] = outputBuffer1[position1++];
                if (position1 == inputBuffer1.Length)
                    position1 = 0;
                //source 2
                index = position2 - (int)(baseDelay * (1.0 - lfoValue) + minDelay);
                if (index < 0)
                    index += inputBuffer2.Length;
                inputBuffer2[position2] = source2[x];
                outputBuffer2[position2] = dMix * inputBuffer2[position2] + wMix * inputBuffer2[index] + fBack * outputBuffer2[index];
                source2[x] = outputBuffer2[position2++];
                if (position2 == inputBuffer2.Length)
                    position2 = 0;

            }
        }
        public void Reset()
        {
            lfo.Reset();
            Array.Clear(inputBuffer1, 0, inputBuffer1.Length);
            Array.Clear(outputBuffer1, 0, outputBuffer1.Length);
            Array.Clear(inputBuffer2, 0, inputBuffer2.Length);
            Array.Clear(outputBuffer2, 0, outputBuffer2.Length);
            position1 = 0;
            position2 = 0;
        }
    }
}
