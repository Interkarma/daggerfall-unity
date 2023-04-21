namespace DaggerfallWorkshop.AudioSynthesis.Sf2
{
    public class Sf2Region
    {
        private short[] generators;

        public short[] Generators
        {
            get { return generators; }
        }

        public Sf2Region()
        {
            generators = new short[61];
        }
        public void ApplyDefaultValues()
        {
            generators[0] = 0; //startAddrsOffset
            generators[1] = 0; //endAddrsOffset
            generators[2] = 0; //startloopAddrsOffset
            generators[3] = 0; //endloopAddrsOffset
            generators[4] = 0; //startAddrsCoarseOffset
            generators[5] = 0; //modLfoToPitch
            generators[6] = 0; //vibLfoToPitch
            generators[7] = 0; //modEnvToPitch
            generators[8] = 13500; //initialFilterFc
            generators[9] = 0; //initialFilterQ
            generators[10] = 0; //modLfoToFilterFc
            generators[11] = 0; //modEnvToFilterFc
            generators[12] = 0; //endAddrsCoarseOffset
            generators[13] = 0; //modLfoToVolume
            generators[15] = 0; //chorusEffectsSend
            generators[16] = 0; //reverbEffectsSend
            generators[17] = 0; //pan
            generators[21] = -12000; //delayModLFO
            generators[22] = 0; //freqModLFO
            generators[23] = -12000; //delayVibLFO
            generators[24] = 0; //freqVibLFO
            generators[25] = -12000; //delayModEnv
            generators[26] = -12000; //attackModEnv
            generators[27] = -12000; //holdModEnv
            generators[28] = -12000; //decayModEnv
            generators[29] = 0; //sustainModEnv
            generators[30] = -12000; //releaseModEnv
            generators[31] = 0; //keynumToModEnvHold
            generators[32] = 0; //keynumToModEnvDecay
            generators[33] = -12000; //delayVolEnv
            generators[34] = -12000; //attackVolEnv
            generators[35] = -12000; //holdVolEnv
            generators[36] = -12000; //decayVolEnv
            generators[37] = 0; //sustainVolEnv
            generators[38] = -12000; //releaseVolEnv
            generators[39] = 0; //keynumToVolEnvHold
            generators[40] = 0; //keynumToVolEnvDecay
            generators[43] = 0x7F00;//keyRange
            generators[44] = 0x7F00;//velRange
            generators[45] = 0; //startloopAddrsCoarseOffset
            generators[46] = -1; //keynum
            generators[47] = -1; //velocity
            generators[48] = 0; //initialAttenuation
            generators[50] = 0; //endloopAddrsCoarseOffset
            generators[51] = 0; //coarseTune
            generators[52] = 0; //fineTune
            generators[54] = 0; //sampleModes
            generators[56] = 100; //scaleTuning
            generators[57] = 0; //exclusiveClass
            generators[58] = -1; //overridingRootKey
        }
    }
}
