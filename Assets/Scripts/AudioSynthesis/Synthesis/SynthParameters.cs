using System;

namespace DaggerfallWorkshop.AudioSynthesis.Synthesis
{
    /// <summary>
    /// Parameters for a single synth channel including its program, bank, and cc list.
    /// </summary>
    public class SynthParameters
    {
        public byte program; //program number
        public byte bankSelect; //bank number
        public byte channelAfterTouch; //channel pressure event
        public CCValue pan; //(vol) pan positions controlling both right and left output levels
        public CCValue volume; //(vol) channel volume controller
        public CCValue expression; //(vol) expression controller
        public CCValue modRange; //(pitch) mod wheel pitch modifier in partial cents ie. 22.3
        public CCValue pitchBend; //(pitch) pitch bend including both semitones and cents
        public byte pitchBendRangeCoarse; //controls max and min pitch bend range semitones
        public byte pitchBendRangeFine; //controls max and min pitch bend range cents
        public short masterCoarseTune; //(pitch) transposition in semitones
        public CCValue masterFineTune; //(pitch) transposition in cents
        public bool holdPedal; //hold pedal status (true) for active
        public bool legatoPedal; //legato pedal status (true) for active
        public CCValue rpn; //registered parameter number
        internal Synthesizer synth;

        //These are updated whenever a midi event that affects them is recieved. 
        public float currentVolume;
        public int currentPitch;    //in cents
        public int currentMod;      //in cents
        public PanComponent currentPan;


        public SynthParameters(Synthesizer synth)
        {
            this.synth = synth;
            ResetControllers();
        }
        /// <summary>
        /// Resets all of the channel's controllers to initial first power on values. Not the same as CC-121.
        /// </summary>
        public void ResetControllers()
        {
            program = 0;
            bankSelect = 0;
            channelAfterTouch = 0; //Reset Channel Pressure to 0
            pan.Combined = 0x2000;
            volume.Fine = 0; volume.Coarse = 100; //Reset Vol Positions back to 90/127 (GM spec)
            expression.Combined = 0x3FFF; //Reset Expression positions back to 127/127
            modRange.Combined = 0;
            pitchBend.Combined = 0x2000;
            pitchBendRangeCoarse = 2; //Reset pitch wheel to +-2 semitones (GM spec)
            pitchBendRangeFine = 0;
            masterCoarseTune = 0;
            masterFineTune.Combined = 0x2000; //Reset fine tune
            holdPedal = false;
            legatoPedal = false;
            rpn.Combined = 0x3FFF; //Reset rpn
            UpdateCurrentPan();
            UpdateCurrentPitch();
            UpdateCurrentVolume();
        }
        
        internal void UpdateCurrentVolume()
        {
            currentVolume = expression.Combined / 16383f;
            currentVolume *= currentVolume;
        }
        internal void UpdateCurrentPitch()
        {
            currentPitch = (int)(((pitchBend.Combined - 8192.0) / 8192.0) * ((100 * pitchBendRangeCoarse) + pitchBendRangeFine));
        }
        internal void UpdateCurrentMod()
        {
            currentMod = (int)(Synthesizer.DefaultModDepth * (modRange.Combined / 16383.0));
        }
        internal void UpdateCurrentPan()
        {
            double value = Synthesizer.HalfPi * (pan.Combined / 16383.0);
            currentPan.Left = (float)Math.Cos(value);
            currentPan.Right = (float)Math.Sin(value);
        }
    }
}
