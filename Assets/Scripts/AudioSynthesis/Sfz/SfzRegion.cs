namespace DaggerfallWorkshop.AudioSynthesis.Sfz
{
    using DaggerfallWorkshop.AudioSynthesis.Bank.Components;

    public class SfzRegion
    {
        public enum OffModeEnum { Fast, Normal }

        //Sample Definition
        public string sample = string.Empty;
        //Input Controls
        public byte loChan = 0;
        public byte hiChan = 15;
        public byte loKey = 0;
        public byte hiKey = 127;
        public byte loVel = 0;
        public byte hiVel = 127;
        public short loBend = -8192;
        public short hiBend = 8192;
        public byte loChanAft = 0;
        public byte hiChanAft = 127;
        public byte loPolyAft = 0;
        public byte hiPolyAft = 127;
        public int group = 0;
        public int offBy = 0;
        public OffModeEnum offMode = OffModeEnum.Fast;
        //Sample Player
        public float delay = 0;
        public int offset = 0;
        public int end = 0;
        public int count = 0;
        public LoopModeEnum loopMode = LoopModeEnum.NoLoop;
        public int loopStart = -1;
        public int loopEnd = -1;
        //Pitch
        public short transpose = 0;
        public short tune = 0;
        public short pitchKeyCenter = 60;
        public short pitchKeyTrack = 100;
        public short pitchVelTrack = 0;
        //Pitch EG
        public float pitchEGDelay = 0;
        public float pitchEGStart = 0;
        public float pitchEGAttack = 0;
        public float pitchEGHold = 0;
        public float pitchEGDecay = 0;
        public float pitchEGSustain = 100f;
        public float pitchEGRelease = 0;
        public short pitchEGDepth = 0;
        public float pitchEGVel2Delay = 0;
        public float pitchEGVel2Attack = 0;
        public float pitchEGVel2Hold = 0;
        public float pitchEGVel2Decay = 0;
        public float pitchEGVel2Sustain = 0;
        public float pitchEGVel2Release = 0;
        public short pitchEGVel2Depth = 0;
        //Pitch Lfo
        public float pitchLfoDelay = 0;
        public float pitchLfoFrequency = 0;
        public short pitchLfoDepth = 0;
        //Filter
        public FilterTypeEnum filterType = FilterTypeEnum.BiquadLowpass;
        public float cutOff = -1;
        public float resonance = 0;
        public short filterKeyTrack = 0;
        public byte filterKeyCenter = 60;
        public short filterVelTrack = 0;
        //Filter EG
        public float filterEGDelay = 0;
        public float filterEGStart = 0;
        public float filterEGAttack = 0;
        public float filterEGHold = 0;
        public float filterEGDecay = 0;
        public float filterEGSustain = 100f;
        public float filterEGRelease = 0;
        public short filterEGDepth = 0;
        public float filterEGVel2Delay = 0;
        public float filterEGVel2Attack = 0;
        public float filterEGVel2Hold = 0;
        public float filterEGVel2Decay = 0;
        public float filterEGVel2Sustain = 0;
        public float filterEGVel2Release = 0;
        public short filterEGVel2Depth = 0;
        //Filter Lfo
        public float filterLfoDelay = 0;
        public float filterLfoFrequency = 0;
        public float filterLfoDepth = 0;
        //Amplifier
        public float volume = 0;
        public float pan = 0;
        public float ampKeyTrack = 0;
        public byte ampKeyCenter = 60;
        public float ampVelTrack = 1;
        //Amplifier EG
        public float ampEGDelay = 0;
        public float ampEGStart = 0;
        public float ampEGAttack = 0;
        public float ampEGHold = 0;
        public float ampEGDecay = 0;
        public float ampEGSustain = 100f;
        public float ampEGRelease = 0;
        public float ampEGVel2Delay = 0;
        public float ampEGVel2Attack = 0;
        public float ampEGVel2Hold = 0;
        public float ampEGVel2Decay = 0;
        public float ampEGVel2Sustain = 0;
        public float ampEGVel2Release = 0;
        //Amplifier Lfo
        public float ampLfoDelay = 0;
        public float ampLfoFrequency = 0;
        public float ampLfoDepth = 0;

        public SfzRegion(bool isGlobal)
        {
            if(isGlobal)
            {
                //Sample Definition
                sample = null;
                //Input Controls
                loChan = 255;
                hiChan = 255;
                loKey = 255;
                hiKey = 255;
                loVel = 255;
                hiVel = 255;
                loBend = short.MaxValue;
                hiBend = short.MaxValue;
                loChanAft = 255;
                hiChanAft = 255;
                loPolyAft = 255;
                hiPolyAft = 255;
                group = int.MaxValue;
                offBy = int.MaxValue;
                offMode = (OffModeEnum)int.MaxValue;
                //Sample Player
                delay = float.MaxValue;
                offset = int.MaxValue;
                end = int.MaxValue;
                count = int.MaxValue;
                loopMode = (LoopModeEnum)int.MaxValue;
                loopStart = int.MaxValue;
                loopEnd = int.MaxValue;
                //Pitch
                transpose = short.MaxValue;
                tune = short.MaxValue;
                pitchKeyCenter = short.MaxValue;
                pitchKeyTrack = short.MaxValue;
                pitchVelTrack = short.MaxValue;
                //Pitch EG
                pitchEGDelay = float.MaxValue;
                pitchEGStart = float.MaxValue;
                pitchEGAttack = float.MaxValue;
                pitchEGHold = float.MaxValue;
                pitchEGDecay = float.MaxValue;
                pitchEGSustain = float.MaxValue;
                pitchEGRelease = float.MaxValue;
                pitchEGDepth = short.MaxValue;
                pitchEGVel2Delay = float.MaxValue;
                pitchEGVel2Attack = float.MaxValue;
                pitchEGVel2Hold = float.MaxValue;
                pitchEGVel2Decay = float.MaxValue;
                pitchEGVel2Sustain = float.MaxValue;
                pitchEGVel2Release = float.MaxValue;
                pitchEGVel2Depth = short.MaxValue;
                //Pitch Lfo
                pitchLfoDelay = float.MaxValue;
                pitchLfoFrequency = float.MaxValue;
                pitchLfoDepth = short.MaxValue;
                //Filter
                filterType = FilterTypeEnum.None;
                cutOff = float.MaxValue;
                resonance = float.MaxValue;
                filterKeyTrack = short.MaxValue;
                filterKeyCenter = 255;
                filterVelTrack = short.MaxValue;
                //Filter EG
                filterEGDelay = float.MaxValue;
                filterEGStart = float.MaxValue;
                filterEGAttack = float.MaxValue;
                filterEGHold = float.MaxValue;
                filterEGDecay = float.MaxValue;
                filterEGSustain = float.MaxValue;
                filterEGRelease = float.MaxValue;
                filterEGDepth = short.MaxValue;
                filterEGVel2Delay = float.MaxValue;
                filterEGVel2Attack = float.MaxValue;
                filterEGVel2Hold = float.MaxValue;
                filterEGVel2Decay = float.MaxValue;
                filterEGVel2Sustain = float.MaxValue;
                filterEGVel2Release = float.MaxValue;
                filterEGVel2Depth = short.MaxValue;
                //Filter Lfo
                filterLfoDelay = float.MaxValue;
                filterLfoFrequency = float.MaxValue;
                filterLfoDepth = float.MaxValue;
                //Amplifier
                volume = float.MaxValue;
                pan = float.MaxValue;
                ampKeyTrack = float.MaxValue;
                ampKeyCenter = 255;
                ampVelTrack = float.MaxValue;
                //Amplifier EG
                ampEGDelay = float.MaxValue;
                ampEGStart = float.MaxValue;
                ampEGAttack = float.MaxValue;
                ampEGHold = float.MaxValue;
                ampEGDecay = float.MaxValue;
                ampEGSustain = float.MaxValue;
                ampEGRelease = float.MaxValue;
                ampEGVel2Delay = float.MaxValue;
                ampEGVel2Attack = float.MaxValue;
                ampEGVel2Hold = float.MaxValue;
                ampEGVel2Decay = float.MaxValue;
                ampEGVel2Sustain = float.MaxValue;
                ampEGVel2Release = float.MaxValue;
                //Amplifier Lfo
                ampLfoDelay = float.MaxValue;
                ampLfoFrequency = float.MaxValue;
                ampLfoDepth = float.MaxValue;
            }
        }
        public void ApplyGlobal(SfzRegion globalRegion)
        {
            if (globalRegion.sample != null)
                this.sample = globalRegion.sample;
            if (globalRegion.loChan != 255)
                this.loChan = globalRegion.loChan;
            if (globalRegion.hiChan != 255)
                this.hiChan = globalRegion.hiChan;
            if (globalRegion.loKey != 255)
                this.loKey = globalRegion.loKey;
            if (globalRegion.hiKey != 255)
                this.hiKey = globalRegion.hiKey;
            if (globalRegion.loVel != 255)
                this.loVel = globalRegion.loVel;
            if (globalRegion.hiVel != 255)
                this.hiVel = globalRegion.hiVel;
            if (globalRegion.loBend != short.MaxValue)
                this.loBend = globalRegion.loBend;
            if (globalRegion.hiBend != short.MaxValue)
                this.hiBend = globalRegion.hiBend;
            if (globalRegion.loChanAft != 255)
                this.loChanAft = globalRegion.loChanAft;
            if (globalRegion.hiChanAft != 255)
                this.hiChanAft = globalRegion.hiChanAft;
            if (globalRegion.loPolyAft != 255)
                this.loPolyAft = globalRegion.loPolyAft;
            if (globalRegion.hiPolyAft != 255)
                this.hiPolyAft = globalRegion.hiPolyAft;
            if (globalRegion.group != int.MaxValue)
                this.group = globalRegion.group;
            if (globalRegion.offBy != int.MaxValue)
                this.offBy = globalRegion.offBy;
            if ((int)globalRegion.offMode != int.MaxValue)
                this.offMode = globalRegion.offMode;
            if (globalRegion.delay != float.MaxValue)
                this.delay = globalRegion.delay;
            if (globalRegion.offset != int.MaxValue)
                this.offset = globalRegion.offset;
            if (globalRegion.end != int.MaxValue)
                this.end = globalRegion.end;
            if (globalRegion.count != int.MaxValue)
                this.count = globalRegion.count;
            if ((int)globalRegion.loopMode != int.MaxValue)
                this.loopMode = globalRegion.loopMode;
            if (globalRegion.loopStart != int.MaxValue)
                this.loopStart = globalRegion.loopStart;
            if (globalRegion.loopEnd != int.MaxValue)
                this.loopEnd = globalRegion.loopEnd;
            if (globalRegion.transpose != short.MaxValue)
                this.transpose = globalRegion.transpose;
            if (globalRegion.tune != short.MaxValue)
                this.tune = globalRegion.tune;
            if (globalRegion.pitchKeyCenter != short.MaxValue)
                this.pitchKeyCenter = globalRegion.pitchKeyCenter;
            if (globalRegion.pitchKeyTrack != short.MaxValue)
                this.pitchKeyTrack = globalRegion.pitchKeyTrack;
            if (globalRegion.pitchVelTrack != short.MaxValue)
                this.pitchVelTrack = globalRegion.pitchVelTrack;
            if (globalRegion.pitchEGDelay != float.MaxValue)
                this.pitchEGDelay = globalRegion.pitchEGDelay;
            if(globalRegion.pitchEGStart != float.MaxValue)
                this.pitchEGStart = globalRegion.pitchEGStart;
            if(globalRegion.pitchEGAttack != float.MaxValue)
                this.pitchEGAttack = globalRegion.pitchEGAttack;
            if(globalRegion.pitchEGHold != float.MaxValue)
                this.pitchEGHold = globalRegion.pitchEGHold;
            if(globalRegion.pitchEGDecay != float.MaxValue)
                this.pitchEGDecay = globalRegion.pitchEGDecay;
            if(globalRegion.pitchEGSustain != float.MaxValue)
                this.pitchEGSustain = globalRegion.pitchEGSustain;
            if(globalRegion.pitchEGRelease != float.MaxValue)
                this.pitchEGRelease = globalRegion.pitchEGRelease;
            if(globalRegion.pitchEGDepth != short.MaxValue)
                this.pitchEGDepth = globalRegion.pitchEGDepth;
            if(globalRegion.pitchEGVel2Delay != float.MaxValue)
                this.pitchEGVel2Delay = globalRegion.pitchEGVel2Delay;
            if(globalRegion.pitchEGVel2Attack != float.MaxValue)
                this.pitchEGVel2Attack = globalRegion.pitchEGVel2Attack;
            if(globalRegion.pitchEGVel2Hold != float.MaxValue)
                this.pitchEGVel2Hold = globalRegion.pitchEGVel2Hold;
            if(globalRegion.pitchEGVel2Decay != float.MaxValue)
                this.pitchEGVel2Decay = globalRegion.pitchEGVel2Decay;
            if(globalRegion.pitchEGVel2Sustain != float.MaxValue)
                this.pitchEGVel2Sustain = globalRegion.pitchEGVel2Sustain;
            if(globalRegion.pitchEGVel2Release != float.MaxValue)
                this.pitchEGVel2Release = globalRegion.pitchEGVel2Release;
            if(globalRegion.pitchEGVel2Depth != short.MaxValue)
                this.pitchEGVel2Depth = globalRegion.pitchEGVel2Depth;
            if(globalRegion.pitchLfoDelay != float.MaxValue)
                this.pitchLfoDelay = globalRegion.pitchLfoDelay;
            if(globalRegion.pitchLfoFrequency != float.MaxValue)
                this.pitchLfoFrequency = globalRegion.pitchLfoFrequency;
            if(globalRegion.pitchLfoDepth != short.MaxValue)
                this.pitchLfoDepth = globalRegion.pitchLfoDepth;
            if(globalRegion.filterType != FilterTypeEnum.None)
                this.filterType = globalRegion.filterType;
            if(globalRegion.cutOff != float.MaxValue)
                this.cutOff = globalRegion.cutOff;
            if(globalRegion.resonance != float.MaxValue)
                this.resonance = globalRegion.resonance;
            if (globalRegion.filterKeyTrack != short.MaxValue)
                this.filterKeyTrack = globalRegion.filterKeyTrack;
            if (globalRegion.filterKeyCenter != 255)
                this.filterKeyCenter = globalRegion.filterKeyCenter;
            if (globalRegion.filterVelTrack != short.MaxValue)
                this.filterVelTrack = globalRegion.filterVelTrack;
            if (globalRegion.filterEGDelay != float.MaxValue)
                this.filterEGDelay = globalRegion.filterEGDelay;
            if (globalRegion.filterEGStart != float.MaxValue)
                this.filterEGStart = globalRegion.filterEGStart;
            if (globalRegion.filterEGAttack != float.MaxValue)
                this.filterEGAttack = globalRegion.filterEGAttack;
            if (globalRegion.filterEGHold != float.MaxValue)
                this.filterEGHold = globalRegion.filterEGHold;
            if (globalRegion.filterEGDecay != float.MaxValue)
                this.filterEGDecay = globalRegion.filterEGDecay;
            if (globalRegion.filterEGSustain != float.MaxValue)
                this.filterEGSustain = globalRegion.filterEGSustain;
            if (globalRegion.filterEGRelease != float.MaxValue)
                this.filterEGRelease = globalRegion.filterEGRelease;
            if (globalRegion.filterEGDepth != short.MaxValue)
                this.filterEGDepth = globalRegion.filterEGDepth;
            if (globalRegion.filterEGVel2Delay != float.MaxValue)
                this.filterEGVel2Delay = globalRegion.filterEGVel2Delay;
            if (globalRegion.filterEGVel2Attack != float.MaxValue)
                this.filterEGVel2Attack = globalRegion.filterEGVel2Attack;
            if (globalRegion.filterEGVel2Hold != float.MaxValue)
                this.filterEGVel2Hold = globalRegion.filterEGVel2Hold;
            if (globalRegion.filterEGVel2Decay != float.MaxValue)
                this.filterEGVel2Decay = globalRegion.filterEGVel2Decay;
            if (globalRegion.filterEGVel2Sustain != float.MaxValue)
                this.filterEGVel2Sustain = globalRegion.filterEGVel2Sustain;
            if (globalRegion.filterEGVel2Release != float.MaxValue)
                this.filterEGVel2Release = globalRegion.filterEGVel2Release;
            if (globalRegion.filterEGVel2Depth != short.MaxValue)
                this.filterEGVel2Depth = globalRegion.filterEGVel2Depth;
            if (globalRegion.filterLfoDelay != float.MaxValue)
                this.filterLfoDelay = globalRegion.filterLfoDelay;
            if (globalRegion.filterLfoFrequency != float.MaxValue)
                this.filterLfoFrequency = globalRegion.filterLfoFrequency;
            if (globalRegion.filterLfoDepth != float.MaxValue)
                this.filterLfoDepth = globalRegion.filterLfoDepth;
            if (globalRegion.volume != float.MaxValue)
                this.volume = globalRegion.volume;
            if (globalRegion.pan != float.MaxValue)
                this.pan = globalRegion.pan;
            if (globalRegion.ampKeyTrack != float.MaxValue)
                this.ampKeyTrack = globalRegion.ampKeyTrack;
            if (globalRegion.ampKeyCenter != 255)
                this.ampKeyCenter = globalRegion.ampKeyCenter;
            if (globalRegion.ampVelTrack != float.MaxValue)
                this.ampVelTrack = globalRegion.ampVelTrack;
            if(globalRegion.ampEGDelay != float.MaxValue)
                this.ampEGDelay = globalRegion.ampEGDelay;
            if(globalRegion.ampEGStart != float.MaxValue)
                this.ampEGStart = globalRegion.ampEGStart;
            if(globalRegion.ampEGAttack != float.MaxValue)
                this.ampEGAttack = globalRegion.ampEGAttack;
            if(globalRegion.ampEGHold != float.MaxValue)
                this.ampEGHold = globalRegion.ampEGHold;
            if(globalRegion.ampEGDecay != float.MaxValue)
                this.ampEGDecay = globalRegion.ampEGDecay;
            if(globalRegion.ampEGSustain != float.MaxValue)
                this.ampEGSustain = globalRegion.ampEGSustain;
            if(globalRegion.ampEGRelease != float.MaxValue)
                this.ampEGRelease = globalRegion.ampEGRelease;
            if(globalRegion.ampEGVel2Delay != float.MaxValue)
                this.ampEGVel2Delay = globalRegion.ampEGVel2Delay;
            if(globalRegion.ampEGVel2Attack != float.MaxValue)
                this.ampEGVel2Attack = globalRegion.ampEGVel2Attack;
            if(globalRegion.ampEGVel2Hold != float.MaxValue)
                this.ampEGVel2Hold = globalRegion.ampEGVel2Hold;
            if(globalRegion.ampEGVel2Decay != float.MaxValue)
                this.ampEGVel2Decay = globalRegion.ampEGVel2Decay;
            if(globalRegion.ampEGVel2Sustain != float.MaxValue)
                this.ampEGVel2Sustain = globalRegion.ampEGVel2Sustain;
            if(globalRegion.ampEGVel2Release != float.MaxValue)
                this.ampEGVel2Release = globalRegion.ampEGVel2Release;
            if(globalRegion.ampLfoDelay != float.MaxValue)
                this.ampLfoDelay = globalRegion.ampLfoDelay;
            if(globalRegion.ampLfoFrequency != float.MaxValue)
                this.ampLfoFrequency = globalRegion.ampLfoFrequency;
            if(globalRegion.ampLfoDepth != float.MaxValue)
                this.ampLfoDepth = globalRegion.ampLfoDepth;
        }
        public override string ToString()
        {
            return string.Format("{0}, Chan: {1}-{2}, Key: {3}-{4}", sample, loChan, hiChan, loKey, hiKey);
        }

    }
}
