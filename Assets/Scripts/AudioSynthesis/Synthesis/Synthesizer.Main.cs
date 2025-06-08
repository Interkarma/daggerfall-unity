/*
 *    ______   __ __     _____             __  __  
 *   / ____/__/ // /_   / ___/__  ______  / /_/ /_ 
 *  / /    /_  _  __/   \__ \/ / / / __ \/ __/ __ \
 * / /___ /_  _  __/   ___/ / /_/ / / / / /_/ / / /
 * \____/  /_//_/     /____/\__, /_/ /_/\__/_/ /_/ 
 *                         /____/                  
 * Synthesizer
 *  A synth class that follows the GM spec (for the most part). Use a sequencer to take advantage of easy midi playback, but
 *  the synth can also be used with and external sequencer. See Synthesizer.MidiControl.cs for information about which midi 
 *  events are supported.
 */
using System;
using DaggerfallWorkshop.AudioSynthesis.Bank;
using System.Collections.Generic;
using DaggerfallWorkshop.AudioSynthesis.Midi;
using DaggerfallWorkshop.AudioSynthesis.Bank.Patches;

namespace DaggerfallWorkshop.AudioSynthesis.Synthesis
{
    public partial class Synthesizer
    {
        #region Fields
        //synth variables
        internal float[] sampleBuffer;
        private VoiceManager voiceManager;
        private int audioChannels;
        private bool littleEndian;
        private PatchBank bank;
        private int sampleRate;
        private float mainVolume = 1.0f;
        private float synthGain = .35f;
        private int microBufferSize;
        private int microBufferCount;
        private SynthParameters[] synthChannels;
        #endregion
        #region Properties
        /// <summary>
        /// Controls the method used when stealing voices.
        /// </summary>
        public VoiceStealEnum VoiceStealMethod
        {
            get { return voiceManager.stealingMethod; }
            set { voiceManager.stealingMethod = value; }
        }
        /// <summary>
        /// The number of voices in use.
        /// </summary>
        public int ActiveVoices
        {
            get { return voiceManager.activeVoices.Count; }
        }
        /// <summary>
        /// The number of voices that are not being used.
        /// </summary>
        public int FreeVoices
        {
            get { return voiceManager.freeVoices.Count; }
        }
        /// <summary>
        /// The size of the individual sub buffers in samples
        /// </summary>
        public int MicroBufferSize
        {
            get { return microBufferSize; }
        }
        /// <summary>
        /// The number of sub buffers
        /// </summary>
        public int MicroBufferCount
        {
            get { return microBufferCount; }
        }
        /// <summary>
        /// The size of the entire buffer in bytes
        /// </summary>
        public int RawBufferSize
        {//Assuming 16 bit data;
            get { return sampleBuffer.Length * 2; }
        }
        /// <summary>
        /// The size of the entire buffer in samples
        /// </summary>
        public int WorkingBufferSize
        {
            get { return sampleBuffer.Length; }
        }
        /// <summary>
        /// The number of voices
        /// </summary>
        public int Polyphony
        {
            get { return voiceManager.polyphony; }
        }
        /// <summary>
        /// Global volume control
        /// </summary>
        public float MasterVolume
        {
            get { return mainVolume; }
            set { mainVolume = SynthHelper.Clamp(value, 0.0f, 3.0f); }
        }
        /// <summary>
        /// The mix volume for each voice
        /// </summary>
        public float MixGain
        {
            get { return synthGain; }
            set { synthGain = SynthHelper.Clamp(value, .05f, 1f); }
        }
        /// <summary>
        /// The number of samples per second produced per channel
        /// </summary>
        public int SampleRate
        {
            get { return sampleRate; }
        }
        /// <summary>
        /// The number of audio channels
        /// </summary>
        public int AudioChannels
        {
            get { return audioChannels; }
        }
        /// <summary>
        /// The patch bank that holds all of the currently loaded instrument patches
        /// </summary>
        public PatchBank SoundBank
        {
            get { return bank; }
        }
        #endregion
        #region Methods
        public Synthesizer(int sampleRate, int audioChannels)
            : this(sampleRate, audioChannels, (int)(.01 * sampleRate), 3, DefaultPolyphony) { }
        public Synthesizer(int sampleRate, int audioChannels, int bufferSize, int bufferCount)
            : this(sampleRate, audioChannels, bufferSize, bufferCount, DefaultPolyphony) { }
        public Synthesizer(int sampleRate, int audioChannels, int bufferSize, int bufferCount, int polyphony)
        {
            const int MinSampleRate = 8000;
            const int MaxSampleRate = 96000;
            //Setup synth parameters
            if (sampleRate < MinSampleRate || sampleRate > MaxSampleRate)
                throw new ArgumentException("Invalid paramater: (sampleRate) Valid ranges are " + MinSampleRate + " to " + MaxSampleRate, "sampleRate");
            this.sampleRate = sampleRate;
            if (audioChannels < 1 || audioChannels > 2)
                throw new ArgumentException("Invalid paramater: (audioChannels) Valid ranges are " + 1 + " to " + 2, "audioChannels");
            this.audioChannels = audioChannels;
            this.microBufferSize = SynthHelper.Clamp(bufferSize, (int)(MinBufferSize * sampleRate), (int)(MaxBufferSize * sampleRate));
            this.microBufferSize = (int)Math.Ceiling(this.microBufferSize / (double)DefaultBlockSize) * DefaultBlockSize; //ensure multiple of block size
            this.microBufferCount = Math.Max(1, bufferCount);
            sampleBuffer = new float[microBufferSize * microBufferCount * audioChannels];
            littleEndian = true;
            //Setup Controllers
            synthChannels = new SynthParameters[DefaultChannelCount];
            for (int x = 0; x < synthChannels.Length; x++)
                synthChannels[x] = new SynthParameters(this);            
            //Create synth voices
            voiceManager = new VoiceManager(SynthHelper.Clamp(polyphony, MinPolyphony, MaxPolyphony));
            //Create midi containers
            midiEventQueue = new Queue<MidiMessage>();
            midiEventCounts = new int[this.microBufferCount];
            layerList = new Patch[15]; 
        }
        public bool IsLittleEndian()
        {
            return littleEndian;
        }
        public void SetEndianMode(bool isLittleEndian)
        {
            this.littleEndian = isLittleEndian;
        }
        public void LoadBank(IResource bankFile)
        {
            LoadBank(new PatchBank(bankFile));
        }
        public void LoadBank(PatchBank bank)
        {
            if (bank == null)
                throw new ArgumentNullException("The parameter bank was null.");
            UnloadBank();
            this.bank = bank;
        }
        public void UnloadBank()
        {
            if (this.bank != null)
            {
                NoteOffAll(true);
                voiceManager.UnloadPatches();
                this.bank = null;
            }
        }
        public void ResetSynthControls()
        {
            for (int x = 0; x < synthChannels.Length; x++)
            {
                synthChannels[x].ResetControllers();
            }
            synthChannels[MidiHelper.DrumChannel].bankSelect = PatchBank.DrumBank;
            ReleaseAllHoldPedals();
        }
        public void ResetPrograms()
        {
            for (int x = 0; x < synthChannels.Length; x++)
            {
                synthChannels[x].program = 0;
            }
        }
        public string GetProgramName(int channel)
        {
            if (bank != null)
            {
                SynthParameters sChannel = synthChannels[channel];
                Patch inst = bank.GetPatch(sChannel.bankSelect, sChannel.program);
                if (inst != null)
                    return inst.Name;
            }
            return "Null";
        }
        public Patch GetProgram(int channel)
        {
            if (bank != null)
            {
                SynthParameters sChannel = synthChannels[channel];
                Patch inst = bank.GetPatch(sChannel.bankSelect, sChannel.program);
                if (inst != null)
                    return inst;
            }
            return null;
        }
        public void SetAudioChannelCount(int channels)
        {
            channels = SynthHelper.Clamp(channels, 1, 2);
            if (audioChannels != channels)
            {
                audioChannels = channels;
                sampleBuffer = new float[microBufferSize * microBufferCount * audioChannels];
            }
        }
        public void GetNext(byte[] buffer)
        {
            Array.Clear(sampleBuffer, 0, sampleBuffer.Length);
            FillWorkingBuffer();
            ConvertWorkingBuffer(buffer, sampleBuffer);
        }
        public void GetNext(float[] buffer)
        {
            Array.Clear(sampleBuffer, 0, sampleBuffer.Length);
            FillWorkingBuffer();
            ConvertWorkingBuffer(buffer, sampleBuffer);
        }
        #region Getters
        public float GetChannelVolume(int channel) { return synthChannels[channel].volume.Combined / 16383f; }
        public float GetChannelExpression(int channel) { return synthChannels[channel].expression.Combined / 16383f; }
        public float GetChannelPan(int channel) { return (synthChannels[channel].pan.Combined - 8192.0f) / 8192f; }
        public float GetChannelPitchBend(int channel) { return (synthChannels[channel].pitchBend.Combined - 8192.0f) / 8192f; }
        public bool GetChannelHoldPedalStatus(int channel) { return synthChannels[channel].holdPedal; }
        #endregion
        // private
        private void FillWorkingBuffer()
        {
            /*Break the process loop into sections representing the smallest timeframe before the midi controls need to be updated
            the bigger the timeframe the more efficent the process is, but playback quality will be reduced.*/
            int sampleIndex = 0;
            for (int x = 0; x < microBufferCount; x++)
            {
                if (midiEventQueue.Count > 0)
                {
                    for (int i = 0; i < midiEventCounts[x]; i++)
                    {
                        MidiMessage m = midiEventQueue.Dequeue();
                        ProcessMidiMessage(m.channel, m.command, m.data1, m.data2);
                    }
                }
                //voice processing loop
                LinkedListNode<Voice> node = voiceManager.activeVoices.First; //node used to traverse the active voices
                while (node != null)
                {
                    node.Value.Process(sampleIndex, sampleIndex + microBufferSize * audioChannels);
                    //if an active voice has stopped remove it from the list
                    if (node.Value.VoiceParams.state == VoiceStateEnum.Stopped)
                    {
                        LinkedListNode<Voice> delnode = node; //node used to remove inactive voices
                        node = node.Next;
                        voiceManager.RemoveFromRegistry(delnode.Value);
                        voiceManager.activeVoices.Remove(delnode);
                        voiceManager.freeVoices.AddFirst(delnode);
                    }
                    else
                    {
                        node = node.Next;
                    }
                }
                sampleIndex += microBufferSize * audioChannels;
            }
            Array.Clear(midiEventCounts, 0, midiEventCounts.Length);
        }
        private void ConvertWorkingBuffer(byte[] to, float[] from)
        {
            if (littleEndian)
            {
                for (int x = 0, i = 0; x < from.Length; x++, i += 2)
                {
                    short sample = (short)SynthHelper.Clamp(from[x] * mainVolume * 32768f, -32768f, 32767f);
                    to[i] = (byte)sample;
                    to[i + 1] = (byte)(sample >> 8);
                }
            }
            else
            {
                for (int x = 0, i = 0; x < from.Length; x++, i += 2)
                {
                    short sample = (short)SynthHelper.Clamp(from[x] * mainVolume * 32768f, -32768f, 32767f);
                    to[i] = (byte)(sample >> 8);
                    to[i + 1] = (byte)sample;
                }
            }
        }
        private void ConvertWorkingBuffer(float[] to, float[] from)
        {
            for (int i = 0; i < from.Length; i++)
            {
                float sample = SynthHelper.Clamp(from[i] * mainVolume, -1.0f, 1.0f);
                to[i] = sample;
            }

            //const int bytesPerSample = 2; //again we assume 16 bit audio
            //int channels = from.GetLength(0);
            //int bufferSize = from.GetLength(1);
            //int sampleIndex = 0;
            ////DeadNote
            //if (!(to.Length == bufferSize * channels * bytesPerSample))
            //    Debug.Log("Buffer sizes are mismatched.");

            //for (int i = 0; i < bufferSize; i++)
            //{
            //    for (int c = 0; c < channels; c++)
            //    {
            //        // Apply master volume
            //        float floatSample = from[c, i] * MainVolume;
            //        // Clamp the value to the [-1.0..1.0] range
            //        floatSample = SynthHelper.Clamp(floatSample, -1.0f, 1.0f);
            //        to[sampleIndex++] = floatSample;
            //    }
            //}
        }
        #endregion
    }
}
