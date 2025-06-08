using DaggerfallWorkshop.AudioSynthesis.Synthesis;
using DaggerfallWorkshop.AudioSynthesis.Bank.Components.Generators;

namespace DaggerfallWorkshop.AudioSynthesis.Bank.Patches
{
    /// <summary>
    /// A patch allows a user to create different instrument types. They typically contain components such as generators, 
    /// filters, and envelopes that can be connected or used in many different ways. A patch should not change any of 
    /// it's fields after being loaded. Instead any changes should be implemented inside the abstract methods Process() 
    /// and Start() and only effect a single voice instance. 
    /// </summary>
    public abstract class Patch
    {
        protected string patchName;
        protected int exTarget;
        protected int exGroup;
        //properties
        public int ExclusiveGroupTarget
        {
            get { return exTarget; }
            set { exTarget = value; }
        }
        public int ExclusiveGroup
        {
            get { return exGroup; }
            set { exGroup = value; }
        }
        public string Name
        {
            get { return patchName; }
        }
        //methods
        protected Patch(string name)
        {
            patchName = name;
            exTarget = 0;
            exGroup = 0;
        }     
        public abstract void Process(VoiceParameters voiceparams, int startIndex, int endIndex);
        public abstract bool Start(VoiceParameters voiceparams);
        public abstract void Stop(VoiceParameters voiceparams);
        public abstract void Load(DescriptorList description, AssetManager assets);
        public override string ToString()
        {
            return string.Format("PatchName: {0}", patchName);
        }
    }
}
