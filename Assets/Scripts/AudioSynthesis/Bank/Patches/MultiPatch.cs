using System;
using System.Collections.Generic;
using DaggerfallWorkshop.AudioSynthesis.Synthesis;
using DaggerfallWorkshop.AudioSynthesis.Sf2;

namespace DaggerfallWorkshop.AudioSynthesis.Bank.Patches
{
    /* Patch containing other patches mapped to channel, velocity, or key ranges.
     * Must not contain any other MultiPatches within. */
    public class MultiPatch : Patch
    {
        private class PatchInterval
        {
            public Patch patch = null;
            public byte startChannel = 0;
            public byte startKey = 0;
            public byte startVelocity = 0;
            public byte endChannel = 15;
            public byte endKey = 127;
            public byte endVelocity = 127;

            public PatchInterval()
            {
            }
            public PatchInterval(Patch patch, byte startChannel, byte endChannel, byte startKey, byte endKey, byte startVelocity, byte endVelocity)
            {
                this.patch = patch;
                this.startChannel = startChannel;
                this.endChannel = endChannel;
                this.startKey = startKey;
                this.endKey = endKey;
                this.startVelocity = startVelocity;
                this.endVelocity = endVelocity;
            }
            public bool CheckAllIntervals(int channel, int key, int velocity)
            {
                return (channel >= startChannel && channel <= endChannel) &&
                    (key >= startKey && key <= endKey) &&
                    (velocity >= startVelocity && velocity <= endVelocity);
            }
            public bool CheckChannelAndKey(int channel, int key)
            {
                return (channel >= startChannel && channel <= endChannel) &&
                    (key >= startKey && key <= endKey);
            }
            public bool CheckKeyAndVelocity(int key, int velocity)
            {
                return (key >= startKey && key <= endKey) &&
                    (velocity >= startVelocity && velocity <= endVelocity);
            }
            public bool CheckKey(int key)
            {
                return (key >= startKey && key <= endKey);
            }

            public override string ToString()
            {
                return string.Format("{0}, Channel: {1}-{2}, Key: {3}-{4}, Velocity: {5}-{6}", patch, startChannel, endChannel, startKey, endKey, startVelocity, endVelocity);
            }
        }
        private enum IntervalType { Channel_Key_Velocity, Channel_Key, Key_Velocity, Key };
        private IntervalType iType;
        private PatchInterval[] intervalList;

        public MultiPatch(string name) : base(name) { }
        public int FindPatches(int channel, int key, int velocity, Patch[] layers)
        {
            int count = 0;
            switch (iType)
            {
                case IntervalType.Channel_Key_Velocity:
                    for (int x = 0; x < intervalList.Length; x++)
                    {
                        if (intervalList[x].CheckAllIntervals(channel, key, velocity))
                        {
                            layers[count++] = intervalList[x].patch;
                            if (count == layers.Length)
                                break;
                        }
                    }
                    break;
                case IntervalType.Channel_Key:
                    for (int x = 0; x < intervalList.Length; x++)
                    {
                        if (intervalList[x].CheckChannelAndKey(channel, key))
                        {
                            layers[count++] = intervalList[x].patch;
                            if (count == layers.Length)
                                break;
                        }
                    }
                    break;
                case IntervalType.Key_Velocity:
                    for (int x = 0; x < intervalList.Length; x++)
                    {
                        if (intervalList[x].CheckKeyAndVelocity(key, velocity))
                        {
                            layers[count++] = intervalList[x].patch;
                            if (count == layers.Length)
                                break;
                        }
                    }
                    break;
                case IntervalType.Key:
                    for (int x = 0; x < intervalList.Length; x++)
                    {
                        if (intervalList[x].CheckKey(key))
                        {
                            layers[count++] = intervalList[x].patch;
                            if (count == layers.Length)
                                break;
                        }
                    }
                    break;
            }
            return count;
        }
        public override bool Start(VoiceParameters voiceparams) { throw new NotImplementedException(); }
        public override void Stop(VoiceParameters voiceparams) { throw new NotImplementedException(); }
        public override void Process(VoiceParameters voiceparams, int startIndex, int endIndex) { throw new NotImplementedException(); }
        public override void Load(DescriptorList description, AssetManager assets)
        {
            intervalList = new PatchInterval[description.CustomDescriptions.Length];
            for (int x = 0; x < intervalList.Length; x++)
            {
                if (!description.CustomDescriptions[x].ID.Equals("mpat", StringComparison.InvariantCultureIgnoreCase))
                    throw new Exception(string.Format("The patch: {0} has an invalid descriptor with id {1}", this.patchName, description.CustomDescriptions[x].ID));
                string patchName = (string)description.CustomDescriptions[x].Objects[0];
                PatchAsset pAsset = assets.FindPatch(patchName);
                if(pAsset == null)
                    throw new Exception(string.Format("The patch: {0} could not be found. For multi patches all sub patches must be loaded first.", patchName));
                byte sChan = (byte)description.CustomDescriptions[x].Objects[1];
                byte eChan = (byte)description.CustomDescriptions[x].Objects[2];
                byte sKey = (byte)description.CustomDescriptions[x].Objects[3];
                byte eKey = (byte)description.CustomDescriptions[x].Objects[4];
                byte sVel = (byte)description.CustomDescriptions[x].Objects[5];
                byte eVel = (byte)description.CustomDescriptions[x].Objects[6];
                intervalList[x] = new PatchInterval(pAsset.Patch, sChan, eChan, sKey, eKey, sVel, eVel);
            }
            DetermineIntervalType();
        }
        //public void LoadSfz(SfzRegion[] regions, AssetManager assets, string directory)
        //{
        //    //Load sub instruments first
        //    intervalList = new PatchInterval[regions.Length];
        //    for (int x = 0; x < intervalList.Length; x++)
        //    {
        //        SfzRegion r = regions[x];
        //        DescriptorList descList = new DescriptorList(r);
        //        assets.LoadSampleAsset(descList.GenDescriptions[0].AssetName, patchName, directory);
        //        SfzPatch sfzPatch = new SfzPatch(patchName + "_" + x);
        //        sfzPatch.Load(descList, assets);
        //        intervalList[x] = new PatchInterval(sfzPatch, r.loChan, r.hiChan, r.loKey, r.hiKey, r.loVel, r.hiVel);
        //    }
        //    DetermineIntervalType();
        //}
        public void LoadSf2(Sf2.Sf2Region[] regions, AssetManager assets)
        {
            intervalList = new PatchInterval[regions.Length];
            for (int x = 0; x < intervalList.Length; x++)
            {
                byte loKey;
                byte hiKey;
                byte loVel;
                byte hiVel;
                if(BitConverter.IsLittleEndian)
                {
                    loKey = (byte)(regions[x].Generators[(int)GeneratorEnum.KeyRange] & 0xFF);
                    hiKey = (byte)((regions[x].Generators[(int)GeneratorEnum.KeyRange] >> 8) & 0xFF);
                    loVel = (byte)(regions[x].Generators[(int)GeneratorEnum.VelocityRange] & 0xFF);
                    hiVel = (byte)((regions[x].Generators[(int)GeneratorEnum.VelocityRange] >> 8) & 0xFF);
                }
                else
                {
                    hiKey = (byte)(regions[x].Generators[(int)GeneratorEnum.KeyRange] & 0xFF);
                    loKey = (byte)((regions[x].Generators[(int)GeneratorEnum.KeyRange] >> 8) & 0xFF);
                    hiVel = (byte)(regions[x].Generators[(int)GeneratorEnum.VelocityRange] & 0xFF);
                    loVel = (byte)((regions[x].Generators[(int)GeneratorEnum.VelocityRange] >> 8) & 0xFF);
                }
                Sf2Patch sf2 = new Sf2Patch(patchName + "_" + x);
                sf2.Load(regions[x], assets);
                intervalList[x] = new PatchInterval(sf2, 0, 15, loKey, hiKey, loVel, hiVel);
            }
            DetermineIntervalType();
        }
        public override string ToString()
        {
            return string.Format("MultiPatch: {0}, IntervalCount: {1}, IntervalType: {2}", patchName, intervalList.Length, iType);
        }

        private void DetermineIntervalType()
        {//see if checks on channel and velocity intervals are necessary
            bool checkChannel = false;
            bool checkVelocity = false;
            for (int x = 0; x < intervalList.Length; x++)
            {
                if (intervalList[x].startChannel != 0 || intervalList[x].endChannel != 15)
                {
                    checkChannel = true;
                    if (checkChannel && checkVelocity)
                        break;
                }
                if (intervalList[x].startVelocity != 0 || intervalList[x].endVelocity != 127)
                {
                    checkVelocity = true;
                    if (checkChannel && checkVelocity)
                        break;
                }
            }
            if(checkChannel & checkVelocity)
                iType = IntervalType.Channel_Key_Velocity;
            else if (checkChannel)
                iType = IntervalType.Channel_Key;
            else if (checkVelocity)
                iType = IntervalType.Key_Velocity;
            else
                iType = IntervalType.Key;
        }
    }
}
