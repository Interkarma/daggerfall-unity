using System;
using System.IO;
using DaggerfallWorkshop.AudioSynthesis.Util;

namespace DaggerfallWorkshop.AudioSynthesis.Sf2
{
    public class SoundFont
    {
        //--Fields
        private SoundFontInfo info;
        private SoundFontSampleData data;
        private SoundFontPresets presets;

        //--Properties
        public SoundFontInfo Info
        {
            get { return info; }
        }
        public SoundFontSampleData SampleData
        {
            get { return data; }
        }
        public SoundFontPresets Presets
        {
            get { return presets; }
        }


        //--Methods
        public SoundFont(IResource soundFont)
        {
            if(!soundFont.ReadAllowed())
                throw new Exception("A soundFont resource must have read access.");
            Load(soundFont.OpenResourceForRead());
        }
        public SoundFont(Stream stream)
        {
            Load(stream);
        }

        private void Load(Stream stream)
        {
            using (BinaryReader reader = new BinaryReader(stream))
            {
                string id = new string(IOHelper.Read8BitChars(reader, 4));
                int size = reader.ReadInt32();
                if (!id.Equals("riff", StringComparison.InvariantCultureIgnoreCase))
                    throw new Exception("Invalid soundfont. Could not find RIFF header.");
                id = new string(IOHelper.Read8BitChars(reader, 4));
                if (!id.Equals("sfbk", StringComparison.InvariantCultureIgnoreCase))
                    throw new Exception("Invalid soundfont. Riff type is invalid.");
                info = new SoundFontInfo(reader);
                data = new SoundFontSampleData(reader);
                presets = new SoundFontPresets(reader);
            }
        }
    }
}
