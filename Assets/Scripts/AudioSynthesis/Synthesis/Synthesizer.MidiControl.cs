using System.Collections.Generic;
using DaggerfallWorkshop.AudioSynthesis.Midi;
using DaggerfallWorkshop.AudioSynthesis.Bank;
using DaggerfallWorkshop.AudioSynthesis.Bank.Patches;

namespace DaggerfallWorkshop.AudioSynthesis.Synthesis
{
    public partial class Synthesizer
    {
        internal Queue<MidiMessage> midiEventQueue;
        internal int[] midiEventCounts;
        private Patch[] layerList;

        public IEnumerator<MidiMessage> MidiMessageEnumerator
        {
            get { return midiEventQueue.GetEnumerator(); }
        }
        /// <summary>
        /// Starts a voice with the given key and velocity.
        /// </summary>
        /// <param name="channel">The midi channel this voice is on.</param>
        /// <param name="note">The key the voice will play in.</param>
        /// <param name="velocity">The volume of the voice.</param>
        public void NoteOn(int channel, int note, int velocity)
        {
            // Get the correct instrument depending if it is a drum or not
            SynthParameters sChan = synthChannels[channel];
            Patch inst = bank.GetPatch(sChan.bankSelect, sChan.program);
            if (inst == null && channel == MidiHelper.DrumChannel)
                inst = bank.GetPatch(sChan.bankSelect, 0); // Attempt falling back to patch 0 for drums
            if (inst == null)
                return;
            // A NoteOn can trigger multiple voices via layers
            int layerCount;
            if (inst is MultiPatch)
            {
                layerCount = ((MultiPatch)inst).FindPatches(channel, note, velocity, layerList);
            }
            else
            {
                layerCount = 1;
                layerList[0] = inst;
            }
            // If a key with the same note value exists, stop it
            if (voiceManager.registry[channel, note] != null)
            {
                VoiceManager.VoiceNode node = voiceManager.registry[channel, note];
                while (node != null)
                {
                    node.Value.Stop();
                    node = node.Next;
                }
                voiceManager.RemoveFromRegistry(channel, note);
            }
            // Check exclusive groups
            for (int x = 0; x < layerCount; x++)
            {
                bool notseen = true;
                for (int i = x - 1; i >= 0; i--)
                {
                    if (layerList[x].ExclusiveGroupTarget == layerList[i].ExclusiveGroupTarget)
                    {
                        notseen = false;
                        break;
                    }
                }
                if (layerList[x].ExclusiveGroupTarget != 0 && notseen)
                {
                    LinkedListNode<Voice> node = voiceManager.activeVoices.First;
                    while (node != null)
                    {
                        if (layerList[x].ExclusiveGroupTarget == node.Value.Patch.ExclusiveGroup)
                        {
                            node.Value.Stop();
                            voiceManager.RemoveFromRegistry(node.Value);
                        }
                        node = node.Next;
                    }
                }
            }
            // Assign a voice to each layer
            for (int x = 0; x < layerCount; x++)
            {
                Voice voice = voiceManager.GetFreeVoice();
                if (voice == null)// out of voices and skipping is enabled
                    break;
                voice.Configure(channel, note, velocity, layerList[x], synthChannels[channel]);
                voiceManager.AddToRegistry(voice);
                voiceManager.activeVoices.AddLast(voice);
                voice.Start();
            }
            // Clear layer list
            for (int x = 0; x < layerCount; x++)
                layerList[x] = null;
        }
        /// <summary>
        /// Attempts to stop a voice by putting it into its release phase. 
        /// If there is no release phase defined the voice will stop immediately.
        /// </summary>
        /// <param name="channel">The channel of the voice.</param>
        /// <param name="note">The key of the voice.</param>
        public void NoteOff(int channel, int note)
        {
            if (synthChannels[channel].holdPedal)
            {
                VoiceManager.VoiceNode node = voiceManager.registry[channel, note];
                while (node != null)
                {
                    node.Value.VoiceParams.noteOffPending = true;
                    node = node.Next;
                }
            }
            else
            {
                VoiceManager.VoiceNode node = voiceManager.registry[channel, note];
                while (node != null)
                {
                    node.Value.Stop();
                    node = node.Next;
                }
                voiceManager.RemoveFromRegistry(channel, note);
            }
        }
        /// <summary>
        /// Stops all voices.
        /// </summary>
        /// <param name="immediate">If true all voices will stop immediately regardless of their release phase.</param>
        public void NoteOffAll(bool immediate)
        {
            LinkedListNode<Voice> node = voiceManager.activeVoices.First;
            if (immediate)
            {//if immediate ignore hold pedals and clear the entire registry
                voiceManager.ClearRegistry();
                while (node != null)
                {
                    node.Value.StopImmediately();
                    LinkedListNode<Voice> delnode = node;
                    node = node.Next;
                    voiceManager.activeVoices.Remove(delnode);
                    voiceManager.freeVoices.AddFirst(delnode);
                }
            }
            else
            {//otherwise we have to check for hold pedals and double check the registry before removing the voice
                while (node != null)
                {
                    VoiceParameters voiceParams = node.Value.VoiceParams;
                    if (voiceParams.state == VoiceStateEnum.Playing)
                    {
                        //if hold pedal is enabled do not stop the voice
                        if (synthChannels[voiceParams.channel].holdPedal)
                        {
                            voiceParams.noteOffPending = true;
                        }
                        else
                        {
                            node.Value.Stop();
                            voiceManager.RemoveFromRegistry(node.Value);
                        }
                    }
                    node = node.Next;
                }
            }
        }
        /// <summary>
        /// Stops all voices on a given channel.
        /// </summary>
        /// <param name="channel">The midi channel.</param>
        /// <param name="immediate">If true the voices will stop immediately regardless of their release phase.</param>
        public void NoteOffAll(int channel, bool immediate)
        {
            LinkedListNode<Voice> node = voiceManager.activeVoices.First;
            while (node != null)
            {
                if (channel == node.Value.VoiceParams.channel)
                {
                    if (immediate)
                    {
                        node.Value.StopImmediately();
                        LinkedListNode<Voice> delnode = node;
                        node = node.Next;
                        voiceManager.activeVoices.Remove(delnode);
                        voiceManager.freeVoices.AddFirst(delnode);
                    }
                    else
                    {
                        //if hold pedal is enabled do not stop the voice
                        if (synthChannels[channel].holdPedal)
                            node.Value.VoiceParams.noteOffPending = true;
                        else
                            node.Value.Stop();
                        node = node.Next;
                    }
                }
            }
        }
        /// <summary>
        /// Executes a midi command without queueing it first.
        /// </summary>
        /// <param name="midimsg">A midi message struct.</param>
        public void ProcessMidiMessage(int channel, int command, int data1, int data2)
        {
            switch (command)
            {
                case 0x80: //NoteOff
                    NoteOff(channel, data1);
                    break;
                case 0x90: //NoteOn
                    if (data2 == 0)
                        NoteOff(channel, data1);
                    else
                        NoteOn(channel, data1, data2);
                    break;
                /*case 0xA0: //NoteAftertouch
                    synth uses channel after touch instead
                    break;*/
                case 0xB0: //Controller
                    #region Controller Switch  
                    switch (data1)
                    {
                        case 0x00: //Bank select coarse
                            if (channel == MidiHelper.DrumChannel)
                                data2 += PatchBank.DrumBank;
                            if (bank.IsBankLoaded(data2))
                                synthChannels[channel].bankSelect = (byte)data2;
                            else
                                synthChannels[channel].bankSelect = (channel == MidiHelper.DrumChannel) ? (byte)PatchBank.DrumBank : (byte)0;
                            break;
                        case 0x01: //Modulation wheel coarse
                            synthChannels[channel].modRange.Coarse = (byte)data2;
                            synthChannels[channel].UpdateCurrentMod();
                            break;
                        case 0x21: //Modulation wheel fine
                            synthChannels[channel].modRange.Fine = (byte)data2;
                            synthChannels[channel].UpdateCurrentMod();
                            break;
                        case 0x07: //Channel volume coarse
                            synthChannels[channel].volume.Coarse = (byte)data2;
                            break;
                        case 0x27: //Channel volume fine
                            synthChannels[channel].volume.Fine = (byte)data2;
                            break;
                        case 0x0A: //Pan coarse
                            synthChannels[channel].pan.Coarse = (byte)data2;
                            synthChannels[channel].UpdateCurrentPan();
                            break;
                        case 0x2A: //Pan fine
                            synthChannels[channel].pan.Fine = (byte)data2;
                            synthChannels[channel].UpdateCurrentPan();
                            break;
                        case 0x0B: //Expression coarse
                            synthChannels[channel].expression.Coarse = (byte)data2;
                            synthChannels[channel].UpdateCurrentVolume();
                            break;
                        case 0x2B: //Expression fine
                            synthChannels[channel].expression.Fine = (byte)data2;
                            synthChannels[channel].UpdateCurrentVolume();
                            break;
                        case 0x40: //Hold Pedal
                            if (synthChannels[channel].holdPedal && !(data2 > 63)) //if hold pedal is released stop any voices with pending release tags
                                ReleaseHoldPedal(channel);
                            synthChannels[channel].holdPedal = data2 > 63;
                            break;
                        case 0x44: //Legato Pedal
                            synthChannels[channel].legatoPedal = data2 > 63;
                            break;
                        case 0x63: //NRPN Coarse Select   //fix for invalid DataEntry after unsupported NRPN events
                            synthChannels[channel].rpn.Combined = 0x3FFF; //todo implement NRPN
                            break;
                        case 0x62: //NRPN Fine Select     //fix for invalid DataEntry after unsupported NRPN events
                            synthChannels[channel].rpn.Combined = 0x3FFF; //todo implement NRPN
                            break;
                        case 0x65: //RPN Coarse Select
                            synthChannels[channel].rpn.Coarse = (byte)data2;
                            break;
                        case 0x64: //RPN Fine Select
                            synthChannels[channel].rpn.Fine = (byte)data2;
                            break;
                        case 0x78: //All Sounds Off
                            NoteOffAll(true);
                            break;
                        case 0x7B: //All Notes Off
                            NoteOffAll(false);
                            break;
                        case 0x06: //DataEntry Coarse
                            switch (synthChannels[channel].rpn.Combined)
                            {
                                case 0: //change semitone, pitchwheel
                                    synthChannels[channel].pitchBendRangeCoarse = (byte)data2;
                                    synthChannels[channel].UpdateCurrentPitch();
                                    break;
                                case 1: //master fine tune coarse
                                    synthChannels[channel].masterFineTune.Coarse = (byte)data2;
                                    break;
                                case 2: //master coarse tune coarse
                                    synthChannels[channel].masterCoarseTune = (short)(data2 - 64);
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case 0x26: //DataEntry Fine
                            switch (synthChannels[channel].rpn.Combined)
                            {
                                case 0: //change cents, pitchwheel
                                    synthChannels[channel].pitchBendRangeFine = (byte)data2;
                                    synthChannels[channel].UpdateCurrentPitch();
                                    break;
                                case 1: //master fine tune fine
                                    synthChannels[channel].masterFineTune.Fine = (byte)data2;
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case 0x79: //Reset the following controllers, follows midi spec: RP-015
                            synthChannels[channel].expression.Combined = 0x3FFF;
                            synthChannels[channel].modRange.Combined = 0;
                            if (synthChannels[channel].holdPedal)
                                ReleaseHoldPedal(channel);
                            synthChannels[channel].holdPedal = false;
                            synthChannels[channel].legatoPedal = false;
                            synthChannels[channel].rpn.Combined = 0x3FFF;
                            synthChannels[channel].pitchBend.Combined = 0x2000;
                            synthChannels[channel].channelAfterTouch = 0;
                            synthChannels[channel].UpdateCurrentPitch(); //because pitchBend was reset
                            synthChannels[channel].UpdateCurrentVolume(); //because expression was reset
                            break;
                        default:
                            return;
                    }
                    #endregion
                    break;
                case 0xC0: //Program Change
                    synthChannels[channel].program = (byte)data1;
                    break;
                case 0xD0: //Channel Aftertouch
                    synthChannels[channel].channelAfterTouch = (byte)data2;
                    break;
                case 0xE0: //Pitch Bend
                    synthChannels[channel].pitchBend.Coarse = (byte)data2;
                    synthChannels[channel].pitchBend.Fine = (byte)data1;
                    synthChannels[channel].UpdateCurrentPitch();
                    break;
                default:
                    return;
            }
        }
        
        //private
        private void ReleaseAllHoldPedals()
        {
            LinkedListNode<Voice> node = voiceManager.activeVoices.First;
            while (node != null)
            {
                if (node.Value.VoiceParams.noteOffPending)
                {
                    node.Value.Stop();
                    voiceManager.RemoveFromRegistry(node.Value);
                }
                node = node.Next;
            }
        }
        private void ReleaseHoldPedal(int channel)
        {
            LinkedListNode<Voice> node = voiceManager.activeVoices.First;
            while (node != null)
            {
                if (node.Value.VoiceParams.channel == channel && node.Value.VoiceParams.noteOffPending)
                {
                    node.Value.Stop();
                    voiceManager.RemoveFromRegistry(node.Value);
                }
                node = node.Next;
            }
        }
    }
}
