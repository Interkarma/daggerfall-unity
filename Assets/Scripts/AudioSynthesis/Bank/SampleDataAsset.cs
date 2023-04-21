using System;
using System.IO;
using DaggerfallWorkshop.AudioSynthesis.Wave;
using DaggerfallWorkshop.AudioSynthesis.Util;
using DaggerfallWorkshop.AudioSynthesis.Util.Riff;
using DaggerfallWorkshop.AudioSynthesis.Sf2;

namespace DaggerfallWorkshop.AudioSynthesis.Bank
{
    public class SampleDataAsset
    {
        private string assetName;
        private int audioChannels = 1; //Force mono samples
        private int sampleRate;
        private short rootKey = 60;
        private short tune = 0;
        private double start;
        private double end;
        private double loopStart = -1;
        private double loopEnd = -1;
        private PcmData sampleData;

        public string Name
        {
            get { return assetName; }
        }
        public int Channels
        {
            get { return audioChannels; }
        }
        public int SampleRate
        {
            get { return sampleRate; }
        }
        public short RootKey
        {
            get { return rootKey; }
        }
        public short Tune
        {
            get { return tune; }
        }
        public double Start
        {
            get { return start; }
        }
        public double End
        {
            get { return end; }
        }
        public double LoopStart
        {
            get { return loopStart; }
        }
        public double LoopEnd
        {
            get { return loopEnd; }
        }
        public PcmData SampleData
        {
            get { return sampleData; }
        }

        public SampleDataAsset(int size, BinaryReader reader)
        {
            assetName = IOHelper.Read8BitString(reader, 20);
            sampleRate = reader.ReadInt32();
            rootKey = reader.ReadInt16();
            tune = reader.ReadInt16();
            loopStart = reader.ReadDouble();
            loopEnd = reader.ReadDouble();
            byte bits = reader.ReadByte();
            byte chans = reader.ReadByte();
            byte[] data = reader.ReadBytes(size - 46);
            if (chans != audioChannels) //reformat to supported channels
                data = WaveHelper.GetChannelPcmData(data, bits, chans, audioChannels);
            sampleData = PcmData.Create(bits, data, true);
            start = 0;
            end = sampleData.Length;
        }
        public SampleDataAsset(string name, WaveFile wave)
        {
            if (name == null)
                throw new ArgumentNullException("An asset must be given a valid name.");
            assetName = name;
            SamplerChunk smpl = wave.FindChunk<SamplerChunk>();
            if (smpl != null)
            {
                sampleRate = (int)(44100.0 * (1.0 / (smpl.SamplePeriod / 22675.0)));
                rootKey = (short)smpl.UnityNote;
                tune = (short)(smpl.PitchFraction * 100);
                if (smpl.Loops.Length > 0)
                {
                    //--WARNING ASSUMES: smpl.Loops[0].Type == SamplerChunk.SampleLoop.LoopType.Forward
                    loopStart = smpl.Loops[0].Start;
                    loopEnd = smpl.Loops[0].End + smpl.Loops[0].Fraction + 1;
                }
            }
            else
            {
                sampleRate = wave.Format.SampleRate;
            }
            byte[] data = wave.Data.RawSampleData;
            if (wave.Format.ChannelCount != audioChannels) //reformat to supported channels
                data = WaveHelper.GetChannelPcmData(data, wave.Format.BitsPerSample, wave.Format.ChannelCount, audioChannels);
            sampleData = PcmData.Create(wave.Format.BitsPerSample, data, true);
            start = 0;
            end = sampleData.Length;
        }
        public SampleDataAsset(SampleHeader sample, SoundFontSampleData sampleData)
        {
            this.assetName = sample.Name;
            this.sampleRate = sample.SampleRate;
            this.rootKey = sample.RootKey;
            this.tune = sample.Tune;
            this.start = sample.Start;
            this.end = sample.End;
            this.loopStart = sample.StartLoop;
            this.loopEnd = sample.EndLoop;
            this.sampleData = PcmData.Create(sampleData.BitsPerSample, sampleData.SampleData, true);
        }

        public override string ToString()
        {
            return string.Format("Name: {0}, SampleRate: {1}Hz, Size: {2} bytes", assetName, sampleRate, sampleData.Length);
        }
    }
}
