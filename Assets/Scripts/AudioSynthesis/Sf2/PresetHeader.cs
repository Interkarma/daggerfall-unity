namespace DaggerfallWorkshop.AudioSynthesis.Sf2
{
    public class PresetHeader
    {
        private string name;
        private ushort patchNumber;
        private ushort bankNumber;
        private int library;
        private int genre;
        private int morphology;
        private Zone[] zones;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public int PatchNumber
        {
            get { return patchNumber; }
            set { patchNumber = (ushort)value; }
        }
        public int BankNumber
        {
            get { return bankNumber; }
            set { bankNumber = (ushort)value; }
        }
        public int Library
        {
            get { return library; }
            set { library = value; }
        }
        public int Genre
        {
            get { return genre; }
            set { genre = value; }
        }
        public int Morphology
        {
            get { return morphology; }
            set { morphology = value; }
        }
        public Zone[] Zones
        {
            get { return zones; }
            set { zones = value; }
        }

        public override string ToString()
        {
            return string.Format("{0}-{1} {2}", bankNumber, patchNumber, name);
        }
    }
}
