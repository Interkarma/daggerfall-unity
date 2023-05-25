using System;
using DaggerfallWorkshop.AudioSynthesis.Bank.Patches;

namespace DaggerfallWorkshop.AudioSynthesis.Synthesis
{
    internal class Voice
    {
        // Variables
        private Patch patch;
        private VoiceParameters voiceparams;
        // Properties
        public Patch Patch
        {
            get { return patch; }
        }
        public VoiceParameters VoiceParams
        {
            get { return voiceparams; }
        }
        // Public
        public Voice()
        {
            voiceparams = new VoiceParameters();
        }
        public void Start()
        {
            if (voiceparams.state != VoiceStateEnum.Stopped)
                return;
            if (patch.Start(voiceparams))
                voiceparams.state = VoiceStateEnum.Playing;
        }
        public void Stop()
        {
            if (voiceparams.state != VoiceStateEnum.Playing)
                return;
            voiceparams.state = VoiceStateEnum.Stopping;
            patch.Stop(voiceparams);
        }
        public void StopImmediately()
        {
            voiceparams.state = VoiceStateEnum.Stopped;
        }
        public void Process(int startIndex, int endIndex)
        {
            //do not process if the voice is stopped
            if (voiceparams.state == VoiceStateEnum.Stopped)
                return;            
            //process using the patch's algorithm
            patch.Process(voiceparams, startIndex, endIndex);
        }
        public void Configure(int channel, int note, int velocity, Patch patch, SynthParameters synthParams)
        {
            voiceparams.Reset();
            voiceparams.channel = channel;
            voiceparams.note = note;
            voiceparams.velocity = velocity;
            voiceparams.synthParams = synthParams;
            this.patch = patch;
        }
        public override string ToString()
        {
            return voiceparams.ToString() + ", PatchName: " + (patch == null ? "null" : patch.Name);
        }
    }
}
