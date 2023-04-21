namespace DaggerfallWorkshop.AudioSynthesis.Sf2
{
    public class Zone
    {
        private Modulator[] modulators;
        private Generator[] generators;

        public Modulator[] Modulators
        {
            get
            {
                return modulators;
            }
            set
            {
                modulators = value;
            }
        }
        public Generator[] Generators
        {
            get
            {
                return generators;
            }
            set
            {
                generators = value;
            }
        }

        public override string ToString()
        {
            return string.Format("Gens:{0} Mods:{1}", generators == null ? 0 : generators.Length, modulators == null ? 0 : modulators.Length);
        }
    }
}
