using System;
using DaggerfallWorkshop.AudioSynthesis.Bank.Descriptors;
using DaggerfallWorkshop.AudioSynthesis.Synthesis;

namespace DaggerfallWorkshop.AudioSynthesis.Bank.Components
{
    public class Filter
    {
        private FilterTypeEnum filterType;
        private float a1, a2, b1, b2; 
        private float m1, m2, m3;
        private double cutOff;
        private double resonance;
        private bool coeffUpdateRequired;

        public FilterTypeEnum FilterMethod
        {
            get { return filterType; }
        }
        public double Cutoff
        {
            get { return cutOff; }
            set 
            {
                if (cutOff != value)
                { cutOff = value; coeffUpdateRequired = true; }
            }
        }
        public double Resonance
        {
            get { return resonance; }
            set 
            {
                if (value != resonance)
                { resonance = value; coeffUpdateRequired = true; }
            }
        }
        public bool CoeffNeedsUpdating
        {
            get { return coeffUpdateRequired; }
        }
        public bool Enabled
        {
            get { return filterType != FilterTypeEnum.None; }
        }

        public void Disable()
        {
            filterType = FilterTypeEnum.None;
        }
        public void QuickSetup(int sampleRate, int note, float velocity, FilterDescriptor filterInfo)
        {
            coeffUpdateRequired = false;
            cutOff = filterInfo.CutOff;
            resonance = filterInfo.Resonance;
            filterType = filterInfo.FilterMethod;
            a1 = 0; a2 = 0; b1 = 0; b2 = 0;
            m1 = 0f; m2 = 0f; m3 = 0f;
            if (cutOff <= 0 || resonance <= 0)
            {
                filterType = FilterTypeEnum.None;
            }
            if (filterType != FilterTypeEnum.None)
            {
                cutOff *= SynthHelper.CentsToPitch((note - filterInfo.RootKey) * filterInfo.KeyTrack + (int)(velocity * filterInfo.VelTrack));
                UpdateCoeff(sampleRate);
            }
        }
        public float ApplyFilter(float sample)
        {
            switch(filterType)
            {
                case FilterTypeEnum.BiquadHighpass:
                case FilterTypeEnum.BiquadLowpass:
                    m3 = sample - a1 * m1 - a2 * m2;
                    sample = b2 * (m3 + m2) + b1 * m1;
                    m2 = m1;
                    m1 = m3;
                    return sample;
                case FilterTypeEnum.OnePoleLowpass:
                    m1 += a1 * (sample - m1);
                    return m1;
                default:
                    return 0f;
            }
        }
        public void ApplyFilter(float[] data)
        {
            switch (filterType)
            {
                case FilterTypeEnum.BiquadHighpass:
                case FilterTypeEnum.BiquadLowpass:
                    for (int x = 0; x < data.Length; x++)
                    {
                        m3 = data[x] - a1 * m1 - a2 * m2;
                        data[x] = b2 * (m3 + m2) + b1 * m1;
                        m2 = m1;
                        m1 = m3;
                    }
                    break;
                case FilterTypeEnum.OnePoleLowpass:
                    for (int x = 0; x < data.Length; x++)
                    {
                        m1 += a1 * (data[x] - m1);
                        data[x] = m1;
                    }
                    break;
                default:
                    break;
            }
        }
        public void ApplyFilterInterp(float[] data, int sampleRate)
        {
            float[] ic = GenerateFilterCoeff(cutOff / sampleRate, resonance);
            float a1_inc = (ic[0] - a1) / data.Length;
            float a2_inc = (ic[1] - a2) / data.Length;
            float b1_inc = (ic[2] - b1) / data.Length;
            float b2_inc = (ic[3] - b2) / data.Length;
            switch (filterType)
            {
                case FilterTypeEnum.BiquadHighpass:
                case FilterTypeEnum.BiquadLowpass:
                    for (int x = 0; x < data.Length; x++)
                    {
                        a1 += a1_inc;
                        a2 += a2_inc;
                        b1 += b1_inc;
                        b2 += b2_inc;
                        m3 = data[x] - a1 * m1 - a2 * m2;
                        data[x] = b2 * (m3 + m2) + b1 * m1;
                        m2 = m1;
                        m1 = m3;
                    }
                    a1 = ic[0];
                    a2 = ic[1];
                    b1 = ic[2];
                    b2 = ic[3];
                    break;
                case FilterTypeEnum.OnePoleLowpass:
                    for (int x = 0; x < data.Length; x++)
                    {
                        a1 += a1_inc;
                        m1 += a1 * (data[x] - m1);
                        data[x] = m1;
                    }
                    a1 = ic[0];
                    break;
                default:
                    break;
            }
            coeffUpdateRequired = false;
        }
        public void UpdateCoeff(int sampleRate)
        {
            float[] coeff = GenerateFilterCoeff(cutOff / sampleRate, resonance);
            a1 = coeff[0];
            a2 = coeff[1];
            b1 = coeff[2];
            b2 = coeff[3];
            coeffUpdateRequired = false;
        }
        public override string ToString()
        {
            if (Enabled)
                return string.Format("Type: {0}, CutOff: {1}Hz, Resonance: {2}", filterType, cutOff, resonance);
            else
                return "Disabled";
        }
        
        //--helper methods for coeff update
        private float[] GenerateFilterCoeff(double fc, double q)
        {
            fc = SynthHelper.Clamp(fc, Synthesizer.DenormLimit, .49);
            float[] coeff = new float[4];
            switch (filterType)
            {
                case FilterTypeEnum.BiquadLowpass:
                    {
                        double w0 = Synthesizer.TwoPi * fc;
                        double cosw0 = Math.Cos(w0);
                        double alpha = Math.Sin(w0) / (2.0 * q);
                        double a0inv = 1.0 / (1.0 + alpha);
                        coeff[0] = (float)(-2.0 * cosw0 * a0inv);
                        coeff[1] = (float)((1.0 - alpha) * a0inv);
                        coeff[2] = (float)((1.0 - cosw0) * a0inv * (1.0 / Math.Sqrt(q)));
                        coeff[3] = b1 * 0.5f;
                    }
                    break;
                case FilterTypeEnum.BiquadHighpass:
                    {
                        double w0 = Synthesizer.TwoPi * fc;
                        double cosw0 = Math.Cos(w0);
                        double alpha = Math.Sin(w0) / (2.0 * q);
                        double a0inv = 1.0 / (1.0 + alpha);
                        double qinv = 1.0 / Math.Sqrt(q);
                        coeff[0] = (float)(-2.0 * cosw0 * a0inv);
                        coeff[1] = (float)((1.0 - alpha) * a0inv);
                        coeff[2] = (float)((-1.0 - cosw0) * a0inv * qinv);
                        coeff[3] = (float)((1.0 + cosw0) * a0inv * qinv * 0.5);
                    }
                    break;
                case FilterTypeEnum.OnePoleLowpass:
                    coeff[0] = 1.0f - (float)Math.Exp(-2.0 * Math.PI * fc);
                    break;
            }
            return coeff;
        }
        
    }
}
