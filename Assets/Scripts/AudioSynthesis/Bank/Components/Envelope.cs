/*
 *    ______   __ __     _____             __  __  
 *   / ____/__/ // /_   / ___/__  ______  / /_/ /_ 
 *  / /    /_  _  __/   \__ \/ / / / __ \/ __/ __ \
 * / /___ /_  _  __/   ___/ / /_/ / / / / /_/ / / /
 * \____/  /_//_/     /____/\__, /_/ /_/\__/_/ /_/ 
 *                         /____/                 
 * Envelope
 *   An envelope class that follows the six stage DAHDSR model and uses tables for fast calculation.
 */

namespace DaggerfallWorkshop.AudioSynthesis.Bank.Components
{
    using System;
    using DaggerfallWorkshop.AudioSynthesis.Bank.Descriptors;
    using DaggerfallWorkshop.AudioSynthesis.Util;

    public class Envelope
    {
        //--Classes and Enum
        private class EnvelopeStage
        {
            public int time;
            public float[] graph;
            public float scale;
            public float offset;
            public bool reverse;

            public EnvelopeStage()
            {
                time = 0;
                graph = null;
                scale = 0;
                offset = 0;
                reverse = false;
            }
        }

        //--Fields
        private EnvelopeStateEnum envState;
        private EnvelopeStage[] stages;
        private EnvelopeStage stage;
        private int index;
        private float value;
        private float depth;

        //--Properties
        public float Value
        {
            get { return value; }
            set { this.value = value; }
        }
        public EnvelopeStateEnum CurrentState
        {
            get { return envState; }
        }
        public float Depth
        {
            get { return depth; }
            set { depth = value; }
        }

        //--Methods
        public Envelope()
        {
            stages = new EnvelopeStage[7];
            for (int x = 0; x < stages.Length; x++)
            {
                stages[x] = new EnvelopeStage();
                stages[x].graph = Tables.EnvelopeTables[0]; 
            }
            stages[3].reverse = true;
            stages[5].reverse = true;
            stages[6].time = 100000000;
            envState = EnvelopeStateEnum.None;
            stage = stages[(int)envState];
        }
        public void QuickSetup(int sampleRate, float velocity, EnvelopeDescriptor envelopeInfo)
        {
            depth = envelopeInfo.Depth + velocity * envelopeInfo.Vel2Depth;
            //Delay
            stages[0].offset = 0;
            stages[0].scale = 0;
            stages[0].time = Math.Max(0, (int)(sampleRate * (envelopeInfo.DelayTime + envelopeInfo.Vel2Delay * velocity)));
            //Attack
            stages[1].offset = envelopeInfo.StartLevel;
            stages[1].scale = envelopeInfo.PeakLevel - envelopeInfo.StartLevel;
            stages[1].time = Math.Max(0, (int)(sampleRate * (envelopeInfo.AttackTime + envelopeInfo.Vel2Attack * velocity)));
            stages[1].graph = Tables.EnvelopeTables[envelopeInfo.AttackGraph];
            //Hold
            stages[2].offset = 0;
            stages[2].scale = envelopeInfo.PeakLevel;
            stages[2].time = Math.Max(0, (int)(sampleRate * (envelopeInfo.HoldTime + envelopeInfo.Vel2Hold * velocity)));
            //Decay
            stages[3].offset = envelopeInfo.SustainLevel;
            stages[3].scale = envelopeInfo.PeakLevel - envelopeInfo.SustainLevel;
            stages[3].time = Math.Max(0, (int)(sampleRate * (envelopeInfo.DecayTime + envelopeInfo.Vel2Decay * velocity)));
            stages[3].graph = Tables.EnvelopeTables[envelopeInfo.DecayGraph];
            //Sustain
            stages[4].offset = 0;
            stages[4].scale = envelopeInfo.SustainLevel + envelopeInfo.Vel2Sustain * velocity;
            stages[4].time = (int)(sampleRate * envelopeInfo.SustainTime);
            //Release
            stages[5].offset = 0;
            stages[5].scale = (stages[3].time == 0 && stages[4].time == 0) ? envelopeInfo.PeakLevel : stages[4].scale;
            stages[5].time = Math.Max(0, (int)(sampleRate * (envelopeInfo.ReleaseTime + envelopeInfo.Vel2Release * velocity)));
            stages[5].graph = Tables.EnvelopeTables[envelopeInfo.ReleaseGraph];
            //None
            stages[6].scale = 0;
            //Reset value, index, and starting state
            index = 0; value = 0;
            envState = EnvelopeStateEnum.Delay;
            while (stages[(int)envState].time == 0)
            {
                envState++;
            }
            stage = stages[(int)envState];
        }
        public void QuickSetupSf2(int sampleRate, int note, short keyNumToHold, short keyNumToDecay, bool isVolumeEnvelope, EnvelopeDescriptor envelopeInfo)
        {
            depth = envelopeInfo.Depth;
            //Delay
            stages[0].offset = 0;
            stages[0].scale = 0;
            stages[0].time = Math.Max(0, (int)(sampleRate * (envelopeInfo.DelayTime)));
            //Attack
            stages[1].offset = envelopeInfo.StartLevel;
            stages[1].scale = envelopeInfo.PeakLevel - envelopeInfo.StartLevel;
            stages[1].time = Math.Max(0, (int)(sampleRate * (envelopeInfo.AttackTime)));
            stages[1].graph = Tables.EnvelopeTables[envelopeInfo.AttackGraph];
            //Hold
            stages[2].offset = 0;
            stages[2].scale = envelopeInfo.PeakLevel;
            stages[2].time = Math.Max(0, (int)(sampleRate * (envelopeInfo.HoldTime) * Math.Pow(2, ((60 - note) * keyNumToHold) / 1200.0)));
            //Decay
            stages[3].offset = envelopeInfo.SustainLevel;
            stages[3].scale = envelopeInfo.PeakLevel - envelopeInfo.SustainLevel;
            if (envelopeInfo.SustainLevel == envelopeInfo.PeakLevel)
                stages[3].time = 0;
            else
                stages[3].time = Math.Max(0, (int)(sampleRate * (envelopeInfo.DecayTime) * Math.Pow(2, ((60 - note) * keyNumToDecay) / 1200.0)));
            stages[3].graph = Tables.EnvelopeTables[envelopeInfo.DecayGraph];
            //Sustain
            stages[4].offset = 0;
            stages[4].scale = envelopeInfo.SustainLevel;
            stages[4].time = (int)(sampleRate * envelopeInfo.SustainTime);
            //Release
            stages[5].scale = stages[3].time == 0 && stages[4].time == 0 ? envelopeInfo.PeakLevel : stages[4].scale;
            if (isVolumeEnvelope)
            {
                stages[5].offset = -100;
                stages[5].scale += 100;
                stages[6].scale = -100;
            }
            else
            {
                stages[5].offset = 0;
                stages[6].scale = 0;
            }
            stages[5].time = Math.Max(0, (int)(sampleRate * (envelopeInfo.ReleaseTime)));
            stages[5].graph = Tables.EnvelopeTables[envelopeInfo.ReleaseGraph];
            //Reset value, index, and starting state
            index = 0; value = 0;
            envState = EnvelopeStateEnum.Delay;
            while (stages[(int)envState].time == 0)
            {
                envState++;
            }
            stage = stages[(int)envState];
        }
        public void Increment(int samples)
        {
            do
            {
                int neededSamples = stage.time - index;
                if (neededSamples > samples)
                {
                    index += samples;
                    samples = 0;
                }
                else
                {
                    index = 0;
                    if (envState != EnvelopeStateEnum.None)
                    {
                        do
                        {
                            stage = stages[(int)++envState];
                        } 
                        while (stage.time == 0);
                    }
                    samples -= neededSamples;
                }
            }
            while (samples > 0);
            
            int i = (int)(stage.graph.Length * (index / (double)stage.time));
            if (stage.reverse)
                value = (1f - stage.graph[i]) * stage.scale + stage.offset;
            else
                value = stage.graph[i] * stage.scale + stage.offset;
        }
        public void Release(float lowerLimit)
        {
            if (value <= lowerLimit)
            {
                index = 0;
                envState = EnvelopeStateEnum.None;
                stage = stages[(int)envState];
            }
            else if (envState < EnvelopeStateEnum.Release)
            {
                index = 0;
                envState = EnvelopeStateEnum.Release;
                stage = stages[(int)envState];
                stage.scale = value;
            }
        }
        public void ReleaseSf2VolumeEnvelope()
        {
            if (value <= -100)
            {
                index = 0;
                envState = EnvelopeStateEnum.None;
                stage = stages[(int)envState];
            }
            else if (envState < EnvelopeStateEnum.Release)
            {
                index = 0;
                envState = EnvelopeStateEnum.Release;
                stage = stages[(int)envState];
                stage.offset = -100;
                stage.scale = 100 + value;
            }
        }
        public override string ToString()
        {
            return string.Format("State: {0}, Time: {1}%, Value: {2:0.00}", envState, (int)((index / (float)stage.time) * 100f), value);
        }
    }
}
