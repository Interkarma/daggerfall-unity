namespace DaggerfallWorkshop.AudioSynthesis.Sf2
{
    using System;
    using System.Globalization;
    using System.IO;
    using DaggerfallWorkshop.AudioSynthesis.Util;

    public class SoundFontInfo
    {
        //--Fields
        private short verMajorROM;
        private short verMinorROM;
        private short verMajorSF;
        private short verMinorSF;
        private string waveTableSoundEngine = string.Empty;
        private string bankName = string.Empty;
        private string dataROM = string.Empty;
        private string creationDate = string.Empty;
        private string author = string.Empty;
        private string targetProduct = string.Empty;
        private string copyright = string.Empty;
        private string comments = string.Empty;
        private string tools = string.Empty;

        //--Properties
        public short ROMVersionMajor { get { return verMajorROM; } }
        public short ROMVersionMinor { get { return verMinorROM; } }
        public short SFVersionMajor { get { return verMajorSF; } }
        public short SFVersionMinor { get { return verMinorSF; } }
        public string SoundEngine { get { return waveTableSoundEngine; } }
        public string BankName { get { return bankName; } }
        public string DataROM { get { return dataROM; } }
        public string CreationDate { get { return creationDate; } }
        public string Author { get { return author; } }
        public string TargetProduct { get { return targetProduct; } }
        public string Copyright { get { return copyright; } }
        public string Comments { get { return comments; } }
        public string Tools { get { return tools; } }

        //--Methods
        public SoundFontInfo(BinaryReader reader)
        {
            string id = new string(IOHelper.Read8BitChars(reader, 4));
            int size = reader.ReadInt32();
            if(!id.Equals("list", StringComparison.InvariantCultureIgnoreCase))
                throw new Exception("Invalid soundfont. Could not find INFO LIST chunk.");
            long readTo = reader.BaseStream.Position + size;
            id = new string(IOHelper.Read8BitChars(reader, 4));
            if (!id.Equals("info", StringComparison.InvariantCultureIgnoreCase))
                throw new Exception("Invalid soundfont. The LIST chunk is not of type INFO.");
            while (reader.BaseStream.Position < readTo)
            {
                id = new string(IOHelper.Read8BitChars(reader, 4));
                size = reader.ReadInt32();
                switch (id.ToLower(CultureInfo.InvariantCulture))
                {
                    case "ifil":
                        verMajorSF = reader.ReadInt16();
                        verMinorSF = reader.ReadInt16();
                        break;
                    case "isng":
                        waveTableSoundEngine = IOHelper.Read8BitString(reader, size);
                        break;
                    case "inam":
                        bankName = IOHelper.Read8BitString(reader, size);
                        break;
                    case "irom":
                        dataROM = IOHelper.Read8BitString(reader, size);
                        break;
                    case "iver":
                        verMajorROM = reader.ReadInt16();
                        verMinorROM = reader.ReadInt16();
                        break;
                    case "icrd":
                        creationDate = IOHelper.Read8BitString(reader, size);
                        break;
                    case "ieng":
                        author = IOHelper.Read8BitString(reader, size);
                        break;
                    case "iprd":
                        targetProduct = IOHelper.Read8BitString(reader, size);
                        break;
                    case "icop":
                        copyright = IOHelper.Read8BitString(reader, size);
                        break;
                    case "icmt":
                        comments = IOHelper.Read8BitString(reader, size);
                        break;
                    case "isft":
                        tools = IOHelper.Read8BitString(reader, size);
                        break;
                    default:
                        throw new Exception("Invalid soundfont. The Chunk: " + id + " was not expected.");
                }
            }
        }
        public override string ToString()
        {
            return "Bank Name: " + bankName + "\nAuthor: " + author + "\n\nComments:\n" + comments;
        }
    }
}
