using System;
using DaggerfallWorkshop.AudioSynthesis.Bank.Components;

namespace DaggerfallWorkshop.AudioSynthesis.Synthesis
{
    public partial class Synthesizer
    {
        public static InterpolationEnum InterpolationMode = InterpolationEnum.Linear;

        public const double TwoPi = 2.0 * Math.PI;      //period constant for sin()
        public const double HalfPi = Math.PI / 2.0;     //half of pi
        public const double InverseSqrtOfTwo = 0.707106781186;
        public const double DefaultLfoFrequency = 8.0; //lfo frequency
        public const int DefaultModDepth = 100;
        public const int DefaultPolyphony = 40;     //number of voices used when not specified
        public const int MinPolyphony = 5;          //Lowest possible number of voices
        public const int MaxPolyphony = 250;        //Highest possible number of voices
        public const int DefaultBlockSize = 64;     //determines alignment of samples when using block processing
        public const double MaxBufferSize = 0.05;   //maximum time before updating midi controls is necessary : 50 ms
        public const double MinBufferSize = 0.001;  //minimum time before updating midi controls is necessary : 1 ms
        public const float DenormLimit = 1e-38f;    //loose denorm limit
        public const float NonAudible = 1e-5f;      //lowest value for volume
        public const int MaxVoiceComponents = 4;    //max number of envelopes, lfos, generators, etc for patches
        public const int DefaultChannelCount = 16;  //The number of synth channels for midi processing. default is 16: 0 - 15
        public const int DefaultKeyCount = 128;     //Then number of keys on a midi keyboard ie: 0-127
    }
}
