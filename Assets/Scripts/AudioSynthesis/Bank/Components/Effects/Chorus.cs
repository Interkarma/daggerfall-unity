namespace DaggerfallWorkshop.AudioSynthesis.Bank.Components.Effects
{
    /// <summary>
    ///  A simple single layer chorus that works for both mono and stereo input.
    /// </summary>
    public class Chorus : Flanger
    {
        public Chorus(int sampleRate, double minDelay, double maxDelay)
            :base(sampleRate,minDelay,maxDelay)
        {

        }
    }
}
