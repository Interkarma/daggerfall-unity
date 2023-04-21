using System;
using DaggerfallWorkshop.AudioSynthesis.Bank.Patches;

namespace DaggerfallWorkshop.AudioSynthesis.Bank
{
    public class PatchAsset
    {
        private string assetName;
        private Patch patch;

        public string Name
        {
            get { return assetName; }
        }
        public Patch Patch
        {
            get { return patch; }
        }

        public PatchAsset(string name, Patch patch)
        {
            if (name == null)
                throw new ArgumentNullException("An asset must be given a valid name.");
            this.assetName = name;
            this.patch = patch;
        }
        public override string ToString()
        {
            if (patch == null)
                return "null";
            return patch.ToString();
        }

    }
}
