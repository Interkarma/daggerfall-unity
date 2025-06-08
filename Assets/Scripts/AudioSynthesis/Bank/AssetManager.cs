using System.IO;
using System.Collections.Generic;
using DaggerfallWorkshop.AudioSynthesis.Wave;

namespace DaggerfallWorkshop.AudioSynthesis.Bank
{
    public class AssetManager
    {
        private List<PatchAsset> patchAssets;
        private List<SampleDataAsset> sampleAssets;

        public List<PatchAsset> PatchAssetList
        {
            get { return patchAssets; }
        }
        public List<SampleDataAsset> SampleAssetList
        {
            get { return sampleAssets; }
        }

        public AssetManager()
        {
            patchAssets = new List<PatchAsset>();
            sampleAssets = new List<SampleDataAsset>();
        }
        public PatchAsset FindPatch(string name)
        {
            for (int x = 0; x < patchAssets.Count; x++)
            {
                if (patchAssets[x].Name.Equals(name))
                    return patchAssets[x];
            }
            return null;
        }
        public SampleDataAsset FindSample(string name)
        {
            for (int x = 0; x < sampleAssets.Count; x++)
            {
                if (sampleAssets[x].Name.Equals(name))
                    return sampleAssets[x];
            }
            return null;
        }
        //public void LoadSampleAsset(string assetName, string patchName, string directory)
        //{
        //    string assetNameWithoutExtension;
        //    string extension;
        //    if (Path.HasExtension(assetName))
        //    {
        //        assetNameWithoutExtension = Path.GetFileNameWithoutExtension(assetName);
        //        extension = Path.GetExtension(assetName).ToLower();
        //    }
        //    else
        //    {
        //        assetNameWithoutExtension = assetName;
        //        assetName += ".wav"; //assume .wav
        //        extension = ".wav";
        //    }
        //    if (FindSample(assetNameWithoutExtension) == null)
        //    {
        //        string waveAssetPath;
        //        if (CrossPlatformHelper.ResourceExists(assetName))
        //            waveAssetPath = assetName; //ex. "asset.wav"
        //        else if (CrossPlatformHelper.ResourceExists(directory + Path.DirectorySeparatorChar + assetName))
        //            waveAssetPath = directory + Path.DirectorySeparatorChar + assetName; //ex. "C:\asset.wav"
        //        else if (CrossPlatformHelper.ResourceExists(directory + "/SAMPLES/" + assetName))
        //            waveAssetPath = directory + "/SAMPLES/" + assetName; //ex. "C:\SAMPLES\asset.wav"
        //        else if (CrossPlatformHelper.ResourceExists(directory + Path.DirectorySeparatorChar + patchName + Path.DirectorySeparatorChar + assetName))
        //            waveAssetPath = directory + Path.DirectorySeparatorChar + patchName + Path.DirectorySeparatorChar + assetName; //ex. "C:\Piano\asset.wav"
        //        else
        //            throw new IOException("Could not find sample asset: (" + assetName + ") required for patch: " + patchName);
        //        using (BinaryReader reader = new BinaryReader(CrossPlatformHelper.OpenResource(waveAssetPath)))
        //        {
        //            switch (extension)
        //            {
        //                case ".wav":
        //                    sampleAssets.Add(new SampleDataAsset(assetNameWithoutExtension, WaveFileReader.ReadWaveFile(reader)));
        //                    break;
        //            }
        //        }
        //    }
        //}
    }
}
