namespace DaggerfallWorkshop.AudioSynthesis.Wave
{
    using System;
    using DaggerfallWorkshop.AudioSynthesis.Util;
    using DaggerfallWorkshop.AudioSynthesis.Synthesis;

    public static class WaveHelper
    {
        //--Methods
        public static float[] GetSampleDataInterleaved(WaveFile wave, int expectedChannels)
        {
            return GetSampleDataInterleaved(wave.Data.RawSampleData, wave.Format.BitsPerSample, wave.Format.ChannelCount, expectedChannels);
        }
        public static float[] GetSampleDataInterleaved(byte[] pcmData, int bitsPerSample, int channelCount, int expectedChannels)
        {
            int samplesPerChannel = pcmData.Length / ((bitsPerSample / 8) * channelCount);
            int channels = Math.Min(expectedChannels, channelCount);
            float[] sampleData = new float[samplesPerChannel * expectedChannels];
            for (int x = 0; x < channels; x++)
                ToSamplesFromPcm(pcmData, bitsPerSample, channelCount, sampleData, x, true);
            return sampleData;
        }
        public static float[][] GetSampleDataDeinterleaved(WaveFile wave, int expectedChannels)
        {
            return GetSampleDataDeinterleaved(wave.Data.RawSampleData, wave.Format.BitsPerSample, wave.Format.ChannelCount, expectedChannels);
        }
        public static float[][] GetSampleDataDeinterleaved(byte[] pcmData, int bitsPerSample, int channelCount, int expectedChannels)
        {
            int samplesPerChannel = pcmData.Length / ((bitsPerSample / 8) * channelCount);
            int channels = Math.Min(expectedChannels, channelCount);
            float[][] sampleData = new float[expectedChannels][];
            for (int x = 0; x < sampleData.Length; x++)
                sampleData[x] = new float[samplesPerChannel];
            for (int x = 0; x < channels; x++)
                ToSamplesFromPcm(pcmData, bitsPerSample, channelCount, sampleData[x], x, false);
            return sampleData;
        }
        public static float[][] Deinterleave(float[] data, int channelCount)
        {
            if (data.Length % channelCount != 0)
                throw new Exception("The data provided is invalid or channel count is invalid");
            float[][] sampleData = new float[channelCount][];
            int channelSize = data.Length / channelCount;
            for (int x = 0; x < sampleData.Length; x++)
            {
                sampleData[x] = new float[channelSize];
                int i = x;
                for (int y = 0; y < sampleData[x].Length; y++)
                {
                    sampleData[x][y] = data[i];
                    i += channelCount;
                }
            }
            return sampleData;
        }
        public static float[] Interleave(float[][] data)
        {
            if (data.Length == 0)
                return new float[0];
            int slen = data[0].Length;
            for (int x = 1; x < data.Length; x++)
            {//if channels are not the same size the smallest channel size is used
                if (data[x].Length < slen)
                    slen = data[x].Length;
            }
            float[] sampleData = new float[data.Length * slen];
            for(int x = 0; x < sampleData.Length; x+= data.Length)
            {
                int z = x / data.Length;
                for (int y = 0; y < data.Length; y++)
                    sampleData[x + y] = data[y][z];
            }
            return sampleData;
        }
        public static byte[] ConvertToPcm(float[][] buffer, int bitsPerSample)
        {
            int slen = buffer[0].Length;
            for (int x = 1; x < buffer.Length; x++)
            {//if channels are not the same size the smallest channel size is used
                if (buffer[x].Length < slen)
                    slen = buffer[x].Length;
            }
            byte[] output = new byte[buffer.Length * slen * bitsPerSample / 8];
            for (int x = 0; x < buffer.Length; x++)
                ToPcmFromSamples(buffer[x], bitsPerSample, buffer.Length, output, x * bitsPerSample / 8);
            return output;
        }
        public static byte[] ConvertToPcm(float[] buffer, int bitsPerSample)
        {
            byte[] output = new byte[buffer.Length * bitsPerSample / 8];
            ToPcmFromSamples(buffer, bitsPerSample, 1, output, 0);
            return output;
        }
        public static byte[] GetChannelPcmData(byte[] pcmData, int bits, int channelCount, int expectedChannels)
        {
            int bytes = bits / 8;
            int channels = Math.Min(expectedChannels, channelCount);
            byte[] newData = new byte[expectedChannels * (pcmData.Length / channelCount)];
            int inc = bytes * channelCount;
            int len = bytes * channels;
            for (int x = 0; x < pcmData.Length; x += inc)
                Array.Copy(pcmData, x, newData, (x / inc) * len, len);
            return newData;
        }
        public static void SwapEndianess(byte[] data, int bits)
        {
            bits /= 8; //get bytes per sample
            byte[] swapArray = new byte[bits];
            for (int x = 0; x < data.Length; x += bits)
            {
                Array.Copy(data, x, swapArray, 0, bits);
                Array.Reverse(swapArray);
                Array.Copy(swapArray, 0, data, x, bits);
            }
        }

        //returns raw audio data in little endian form
        private static void ToPcmFromSamples(float[] input, int bitsPerSample, int channels, byte[] output, int index) 
        {
            switch (bitsPerSample)
            {
                case 8:
                    for (int x = 0; x < input.Length; x++)
                    {
                        output[index] = (byte)((input[x] + 1f) / 2f * 255f);
                        index += channels;
                    }
                    break;
                case 16:
                    for (int x = 0; x < input.Length; x++)
                    {
                        LittleEndianHelper.WriteInt16((short)SynthHelper.Clamp(input[x] * 32768f, -32768f, 32767f), output, index);
                        index += channels * 2;
                    }
                    break;
                case 24:
                    for (int x = 0; x < input.Length; x++)
                    {
                        LittleEndianHelper.WriteInt24((int)SynthHelper.Clamp(input[x] * 8388608f, -8388608f, 8388607f), output, index);
                        index += channels * 3;
                    }
                    break;
                case 32:
                    for (int x = 0; x < input.Length; x++)
                    {
                        LittleEndianHelper.WriteInt32((int)SynthHelper.Clamp(input[x] * 2147483648f, -2147483648f, 2147483647f), output, index);
                        index += channels * 4;
                    }
                    break;
                default:
                    throw new ArgumentException("Invalid bitspersample value. Supported values are 8, 16, 24, and 32.");
            }
        }
        private static void ToSamplesFromPcm(byte[] input, int bitsPerSample, int channelCount, float[] output, int channel, bool interleaved)
        {
            int x,xc,i,ic;
            if(interleaved)
            {
                x = channel;
                xc = channelCount;
            }
            else
            {
                x = 0;
                xc = 1;
            }
            i = channel * bitsPerSample / 8;
            ic = channelCount * bitsPerSample / 8;
            switch (bitsPerSample)
            {
                case 8:
                    while (x < output.Length)
                    {
                        output[x] = ((input[i] / 255f) * 2f) - 1f;
                        x += xc;
                        i += ic;
                    }
                    break;
                case 16:
                    while (x < output.Length)
                    {
                        output[x] = LittleEndianHelper.ReadInt16(input, i) / 32768f;
                        x += xc;
                        i += ic;
                    }
                    break;
                case 24:
                    while (x < output.Length)
                    {
                        output[x] = LittleEndianHelper.ReadInt24(input, i) / 8388608f;
                        x += xc;
                        i += ic;
                    }
                    break;
                case 32:
                    while (x < output.Length)
                    {
                        output[x] = LittleEndianHelper.ReadInt32(input, i) / 2147483648f;
                        x += xc;
                        i += ic;
                    }
                    break;
                default:
                    throw new Exception("Invalid sample format: PCM " + bitsPerSample + " bit.");
            }
        }
        private static void ToSamplesFromFloat(byte[] input, int bitsPerSample, int channelCount, float[] output, int channel)
        {
            byte[] buffer = new byte[bitsPerSample / 8];
            switch (bitsPerSample)
            {
                case 32:
                    for (int x = channel; x < output.Length; x += channelCount)
                    {
                        Array.Copy(input, x * 4, buffer, 0, 4);
                        if (!BitConverter.IsLittleEndian)
                            Array.Reverse(buffer);
                        output[x] = BitConverter.ToSingle(buffer, 0);
                    }
                    break;
                case 64:
                    for (int x = channel; x < output.Length; x += channelCount)
                    {
                        Array.Copy(input, x * 8, buffer, 0, 8);
                        if (!BitConverter.IsLittleEndian)
                            Array.Reverse(buffer);
                        output[x] = (float)BitConverter.ToDouble(buffer, 0);
                    }
                    break;
                default:
                    throw new Exception("Invalid sample format: FLOAT " + bitsPerSample + "bps.");
            }
        }
    }
}
