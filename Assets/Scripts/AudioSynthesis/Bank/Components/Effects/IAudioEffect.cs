namespace DaggerfallWorkshop.AudioSynthesis.Bank.Components.Effects
{
    /// <summary>
    /// Describes an effect that can be applied on a sample buffer.
    /// </summary>
    public interface IAudioEffect
    {
        void ApplyEffect(float[] source);
        void ApplyEffect(float[] source1, float[] source2);
        void Reset();
    }
}