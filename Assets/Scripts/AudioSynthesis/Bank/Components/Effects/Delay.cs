namespace DaggerfallWorkshop.AudioSynthesis.Bank.Components.Effects
{
    using System;

    public class Delay : IAudioEffect
    {
        private float[] buffer1;
        private float[] buffer2;
        private int position1;
        private int position2;

        public Delay(int sampleRate, double delay)
        {
            buffer1 = new float[(int)(sampleRate * delay)];
            position1 = 0;
        }
        public Delay(int sampleRate, double delay1, double delay2)
        {
            buffer1 = new float[(int)(sampleRate * delay1)];
            position1 = 0;
            buffer2 = new float[(int)(sampleRate * delay2)];
            position2 = 0;
        }
        public void ApplyEffect(float[] source)
        {
            int x = 0, end = buffer1.Length - 1;
            while (x < source.Length)
            {
                if (source.Length - x >= end)
                {
                    while (position1 < end)
                    {
                        buffer1[position1++] = source[x];
                        source[x++] = buffer1[position1];
                    }
                    buffer1[position1] = source[x];
                    position1 = 0;
                    source[x++] = buffer1[position1];
                }
                else
                {
                    while (x < source.Length)
                    {
                        buffer1[position1++] = source[x];
                        source[x++] = buffer1[position1];
                    }
                }
            }
        }
        public void ApplyEffect(float[] source1, float[] source2)
        {
            int x, end;
            //source1
            x = 0;
            end = buffer1.Length - 1;
            while (x < source1.Length)
            {
                if (source1.Length - x >= end)
                {
                    while (position1 < end)
                    {
                        buffer1[position1++] = source1[x];
                        source1[x++] = buffer1[position1];
                    }
                    buffer1[position1] = source1[x];
                    position1 = 0;
                    source1[x++] = buffer1[position1];
                }
                else
                {
                    while (x < source1.Length)
                    {
                        buffer1[position1++] = source1[x];
                        source1[x++] = buffer1[position1];
                    }
                }
            }
            //source2
            x = 0;
            end = buffer2.Length - 1;
            while (x < source2.Length)
            {
                if (source2.Length - x >= end)
                {
                    while (position2 < end)
                    {
                        buffer2[position2++] = source2[x];
                        source2[x++] = buffer2[position2];
                    }
                    buffer2[position2] = source2[x];
                    position2 = 0;
                    source2[x++] = buffer2[position2];
                }
                else
                {
                    while (x < source2.Length)
                    {
                        buffer2[position2++] = source2[x];
                        source2[x++] = buffer2[position2];
                    }
                }
            }
        }
        public void Reset()
        {
            position1 = 0;
            position2 = 0;
            Array.Clear(buffer1, 0, buffer1.Length);
            if(buffer2 != null)
                Array.Clear(buffer2, 0, buffer2.Length);
        }
    }
}
