namespace DaggerfallWorkshop.AudioSynthesis.Wave
{
    using System;
    using System.Diagnostics;
    using System.IO;

    public sealed class WaveFileWriter : IDisposable
    {
        //--Fields        
        private int length;
        private int sRate;
        private int channels;
        private int bits;
        private IResource tempR;
        private BinaryWriter writer;
        private IResource wavR;

        //--Methods
        public WaveFileWriter(int sampleRate, int channels, int bitsPerSample, IResource tempFile, IResource waveFile)
        {
            this.sRate = sampleRate;
            this.channels = channels;
            this.bits = bitsPerSample;
            if (!tempFile.WriteAllowed() || !tempFile.ReadAllowed() || !tempFile.DeleteAllowed())
                throw new Exception("A valid temporary file with read/write/and delete access is required.");
            tempR = tempFile;
            writer = new BinaryWriter(tempR.OpenResourceForWrite());
            if (!waveFile.WriteAllowed())
                throw new Exception("A valid wave file with write access is required.");
            wavR = waveFile;
        }
        public void Write(byte[] buffer)
        {
            writer.Write(buffer);
            length += buffer.Length;
        }
        public void Write(float[] buffer)
        {
            Write(WaveHelper.ConvertToPcm(buffer, bits));
        }
        public void Write(float[][] buffer)
        {
            Write(WaveHelper.ConvertToPcm(buffer, bits));
        }
        public void Close()
        {
            if (writer == null)
                return;
            //writer.Dispose();
            writer = null;
            using (BinaryWriter bw2 = new BinaryWriter(wavR.OpenResourceForWrite()))
            {
                bw2.Write((Int32)1179011410);
                bw2.Write((Int32)44 + length - 8);
                bw2.Write((Int32)1163280727);
                bw2.Write((Int32)544501094);
                bw2.Write((Int32)16);
                bw2.Write((Int16)1);
                bw2.Write((Int16)channels);
                bw2.Write((Int32)sRate);
                bw2.Write((Int32)(sRate * channels * (bits / 8)));
                bw2.Write((Int16)(channels * (bits / 8)));
                bw2.Write((Int16)bits);
                bw2.Write((Int32)1635017060);
                bw2.Write((Int32)length);
                using (BinaryReader br = new BinaryReader(tempR.OpenResourceForRead()))
                {
                    byte[] buffer = new byte[1024];
                    int count = br.Read(buffer, 0, buffer.Length);
                    while (count > 0)
                    {
                        bw2.Write(buffer, 0, count);
                        count = br.Read(buffer, 0, buffer.Length);
                    }
                }
            }
            tempR.DeleteResource();
        }
        public void Dispose()
        {
            if (writer == null)
                return;
            Close();
        }
    }
}
