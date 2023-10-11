using System;
using DaggerfallWorkshop.AudioSynthesis.Util;
using DaggerfallWorkshop.AudioSynthesis.Midi;
using System.Runtime.InteropServices;

namespace DaggerfallWorkshop.AudioSynthesis.Synthesis
{
    //structs and enum
    public enum VoiceStealEnum { Oldest, Quietest, Skip };
    public enum PanFormulaEnum { Neg3dBCenter, Neg6dBCenter, ZeroCenter }
    public enum VoiceStateEnum { Stopped, Stopping, Playing }

    public struct MidiMessage
    {
        public int delta;
        public byte channel;
        public byte command;
        public byte data1;
        public byte data2;

        public MidiMessage(byte channel, byte command, byte data1, byte data2)
            : this(0, channel, command, data1, data2) { }
        public MidiMessage(int delta, byte channel, byte command, byte data1, byte data2)
        {
            this.delta = delta;
            this.channel = channel;
            this.command = command;
            this.data1 = data1;
            this.data2 = data2;
        }
        public override string ToString()
        {
            if (command >= 0x80 && command <= 0xEF)
                return string.Format("Type: {0}, Channel: {1}, P1: {2}, P2: {3}", (MidiEventTypeEnum)(command & 0xF0), channel, data1, data2);
            else if (command >= 0xF0 && command <= 0xF7)
                return "System Common message";
            else if (command >= 0xF8 && command <= 0xFF)
                return "Realtime message";
            else
                return "Unknown midi message";
        }
    }
    public struct PanComponent
    {
        public float Left;
        public float Right;

        public PanComponent(float value, PanFormulaEnum formula)
        {
            value = SynthHelper.Clamp(value, -1f, 1f);
            switch (formula)
            {
                case PanFormulaEnum.Neg3dBCenter:
                {
                    double dvalue = Synthesizer.HalfPi * (value + 1f) / 2f;
                    Left = (float)Math.Cos(dvalue);
                    Right = (float)Math.Sin(dvalue);
                }
                    break;
                case PanFormulaEnum.Neg6dBCenter:
                {
                    Left = .5f + value * -.5f;
                    Right = .5f + value * .5f;
                }
                    break;
                case PanFormulaEnum.ZeroCenter:
                {
                    double dvalue = Synthesizer.HalfPi * (value + 1.0) / 2.0;
                    Left = (float)(Math.Cos(dvalue) / Synthesizer.InverseSqrtOfTwo);
                    Right = (float)(Math.Sin(dvalue) / Synthesizer.InverseSqrtOfTwo);
                }
                    break;
                default:
                    throw new Exception("Invalid pan law selected.");
            }
        }
        public PanComponent(float right, float left)
        {
            this.Right = right;
            this.Left = left;
        }
        public override string ToString()
        {
            return string.Format("Left: {0:0.0}, Right: {1:0.0}", Left, Right);
        }
    }
    public struct CCValue
    {
        private byte coarseValue;
        private byte fineValue;
        private short combined;

        public byte Coarse
        {
            get { return coarseValue; }
            set { coarseValue = value; UpdateCombined(); }
        }
        public byte Fine
        {
            get { return fineValue; }
            set { fineValue = value; UpdateCombined(); }
        }
        public short Combined
        {
            get { return combined; }
            set { combined = value; UpdateCoarseFinePair(); }
        }

        public CCValue(byte coarse, byte fine)
        {
            coarseValue = coarse;
            fineValue = fine;
            combined = 0;
            UpdateCombined();
        }
        public override string ToString()
        {
            return string.Format("7BitValue: {0}, 14BitValue: {1}", coarseValue, combined);
        }
        private void UpdateCombined()
        {
            if (BitConverter.IsLittleEndian)
                combined = (short)((coarseValue << 7) | fineValue);
            else
                combined = (short)((fineValue << 7) | coarseValue);
        }
        private void UpdateCoarseFinePair()
        {
            if (BitConverter.IsLittleEndian)
            {
                coarseValue = (byte)(combined >> 7);
                fineValue = (byte)(combined & 0x7F);
            }
            else
            {
                fineValue = (byte)(combined >> 7);
                coarseValue = (byte)(combined & 0x7F);
            }
        }
    }
    [StructLayout(LayoutKind.Explicit)]
    public struct UnionData
    {
        //double values
        [FieldOffset(0)] public double double1;
        //float values
        [FieldOffset(0)] public float float1;
        [FieldOffset(4)] public float float2;
        //int values
        [FieldOffset(0)] public int int1;
        [FieldOffset(4)] public int int2;
    }

    //static helper methods
    public static class SynthHelper
    {
        //Math related calculations
        public static double Clamp(double value, double min, double max)
        {
            if (value <= min)
                return min;
            else if (value >= max)
                return max;
            else
                return value;
        }
        public static float Clamp(float value, float min, float max)
        {
            if (value <= min)
                return min;
            else if (value >= max)
                return max;
            else
                return value;
        }
        public static int Clamp(int value, int min, int max)
        {
            if (value <= min)
                return min;
            else if (value >= max)
                return max;
            else
                return value;
        }
        public static short Clamp(short value, short min, short max)
        {
            if (value <= min)
                return min;
            else if (value >= max)
                return max;
            else
                return value;
        }

        public static double NearestPowerOfTwo(double value)
        {
            return Math.Pow(2, Math.Round(Math.Log(value, 2)));
        }
        public static double SamplesFromTime(int sampleRate, double seconds)
        {
            return sampleRate * seconds;
        }
        public static double TimeFromSamples(int sampleRate, int samples)
        {
            return samples / (double)sampleRate;
        }
        
        public static double DBtoLinear(double dBvalue)
        {
            return Math.Pow(10.0, (dBvalue / 20.0));
        }
        public static double LineartoDB(double linearvalue)
        {
            return 20.0 * Math.Log10(linearvalue);
        }
        public static double CalculateRMS(float[] data, int start, int length)
        {
            double sum = 0;
            int end = start + length;
            for (int i = start; i < end; i++)
            {
                double v = data[i];
                sum += v * v;
            }
            return Math.Sqrt(sum / length);
        }

        //Midi Note and Frequency Conversions
        public static double FrequencyToKey(double frequency, int rootkey)
        {
            return 12.0 * Math.Log(frequency / 440.0, 2.0) + rootkey;
        }
        public static double KeyToFrequency(double key, int rootkey)
        {
            return Math.Pow(2.0, (key - rootkey) / 12.0) * 440.0;
        }

        public static double SemitoneToPitch(int key)
        {//does not return a frequency, only the 2^(1/12) value.
            if (key < -127)
                key = -127;
            else if (key > 127)
                key = 127;
            return Tables.SemitoneTable[127 + key];
        }
        public static double CentsToPitch(int cents)
        {//does not return a frequency, only the 2^(1/12) value.
            int key = cents / 100;
            cents -= key * 100;
            if (key < -127)
                key = -127;
            else if (key > 127)
                key = 127;
            return Tables.SemitoneTable[127 + key] * Tables.CentTable[100 + cents];
        }

    }
}
