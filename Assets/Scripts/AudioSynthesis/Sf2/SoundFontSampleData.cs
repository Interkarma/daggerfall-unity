namespace DaggerfallWorkshop.AudioSynthesis.Sf2
{
    using System;
    using System.Globalization;
    using System.IO;
    using DaggerfallWorkshop.AudioSynthesis.Util;

    public class SoundFontSampleData
    {
        //--Fields
        private byte[] samples;
        private int bitsPerSample;
        //--Properties
        public int BitsPerSample
        {
            get { return bitsPerSample; }
        }
        public byte[] SampleData
        {
            get { return samples; }
        }
        //--Methods
        public SoundFontSampleData(BinaryReader reader)
        {
            if (new string(IOHelper.Read8BitChars(reader, 4)).Equals("list", StringComparison.InvariantCultureIgnoreCase) == false)
                throw new Exception("Invalid soundfont. Could not find SDTA LIST chunk.");
            long readTo = reader.ReadInt32();
            readTo += reader.BaseStream.Position;
            if (new string(IOHelper.Read8BitChars(reader, 4)).Equals("sdta", StringComparison.InvariantCultureIgnoreCase) == false)
                throw new Exception("Invalid soundfont. List is not of type sdta.");
            bitsPerSample = 0;
            byte[] rawSampleData = null;
            while (reader.BaseStream.Position < readTo)
            {
                string subID = new string(IOHelper.Read8BitChars(reader, 4));
                int size = reader.ReadInt32();
                switch (subID.ToLower(CultureInfo.InvariantCulture))
                {
                    case "smpl":
                        bitsPerSample = 16;
                        rawSampleData = reader.ReadBytes(size);
                        break;
                    case "sm24":
                        if (rawSampleData == null || size != (int)Math.Ceiling(samples.Length / 2.0))
                        {//ignore this chunk if wrong size or if it comes first
                            reader.ReadBytes(size);
                        }
                        else
                        {
                            bitsPerSample = 24;
                            samples = new byte[rawSampleData.Length + size];
                            for (int x = 0, i = 0; x < samples.Length; x+=3, i +=2)
                            {
                                samples[x] = reader.ReadByte();
                                samples[x + 1] = rawSampleData[i];
                                samples[x + 2] = rawSampleData[i + 1];
                            }
                        }
                        if (size % 2 == 1 && reader.PeekChar() == 0)
                            reader.ReadByte();
                        break;
                    default:
                        throw new Exception("Invalid soundfont. Unknown chunk id: " + subID + ".");
                }
            }
            if (bitsPerSample == 16)
            {
                samples = rawSampleData;
            }
            else if (bitsPerSample != 24)
                throw new NotSupportedException("Only 16 and 24 bit samples are supported.");
        }
    }
}
