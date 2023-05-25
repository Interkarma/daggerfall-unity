/*
 *    ______   __ __     _____             __  __  
 *   / ____/__/ // /_   / ___/__  ______  / /_/ /_ 
 *  / /    /_  _  __/   \__ \/ / / / __ \/ __/ __ \
 * / /___ /_  _  __/   ___/ / /_/ / / / / /_/ / / /
 * \____/  /_//_/     /____/\__, /_/ /_/\__/_/ /_/ 
 *                         /____/                  
 * Midi Input Sequencer 
 *  Used for midi input using short messages. 
 *  Tempo is calculated by the input device so the messages are all processed at the same time with: FillSequencerQueue(...)
 */
using System.Collections.Generic;
using DaggerfallWorkshop.AudioSynthesis.Synthesis;

namespace DaggerfallWorkshop.AudioSynthesis.Sequencer
{
    public class MidiInputSequencer
    {
        private Synthesizer synth;

        public Synthesizer Synth
        {
            get { return synth; }
            set { synth = value; }
        }

        public MidiInputSequencer(Synthesizer synth)
        {
            this.synth = synth;
        }
        public void AddMidiEvent(MidiMessage midiMsg)
        {
            midiMsg.delta = 0;
            synth.midiEventQueue.Enqueue(midiMsg);
            synth.midiEventCounts[0]++;
        }
    }
}
