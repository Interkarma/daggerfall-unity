namespace DaggerfallWorkshop.AudioSynthesis.Sf2
{
    public class Instrument
    {
        private string name;
        private Zone[] zones;

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }
        public Zone[] Zones
        {
            get
            {
                return zones;
            }
            set
            {
                zones = value;
            }
        }

        public override string ToString()
        {
            return this.name;
        }
    }
}
