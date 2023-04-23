using System;
using System.IO;
using System.Collections.Generic;
using DaggerfallWorkshop.AudioSynthesis.Bank.Patches;
using DaggerfallWorkshop.AudioSynthesis.Sf2;
using DaggerfallWorkshop.AudioSynthesis.Util;
using DaggerfallWorkshop.AudioSynthesis.Util.Riff;

namespace DaggerfallWorkshop.AudioSynthesis.Bank
{
    public class PatchBank
    {
        public const float BankVersion = 1.000f;
        public const int DrumBank = 128;
        public const int BankSize = 128;
        private static Dictionary<string, Type> patchTypes;
        private Dictionary<int, Patch[]> bank;
        private AssetManager assets;
        private string bankName;
        private string comment;

        //patch type mappings
        static PatchBank()
        {
            patchTypes = new Dictionary<string, Type>();
            ClearCustomPatchTypes();
        }
        public static void AddCustomPatchType(string id, Type type)
        {//add a patch type/id pair to the map
            if (!type.IsSubclassOf(typeof(Patch)))
                throw new Exception("Type must be a subtype of patch.");
            if (patchTypes.ContainsKey(id))
                throw new Exception("The specified id already exists.");
            if (id.Length == 0 || id.Trim().Equals(""))
                throw new Exception("The specified id is invalid.");
            patchTypes.Add(id, type);
        }
        public static void ClearCustomPatchTypes()
        {//clear any custom patch types from the map
            patchTypes.Clear();
            patchTypes.Add("mult", typeof(MultiPatch));
            patchTypes.Add("basc", typeof(BasicPatch));
            patchTypes.Add("fm2 ", typeof(Fm2Patch));
            patchTypes.Add("sfz ", typeof(SfzPatch));
        }

        public string Name
        {
            get { return bankName; }
        }
        public string Comments
        {
            get { return comment; }
        }

        public PatchBank()
        {
            bank = new Dictionary<int, Patch[]>();
            assets = new AssetManager();
            bankName = string.Empty;
            comment = string.Empty;
        }
        public PatchBank(IResource bankFile)
        {
            bank = new Dictionary<int, Patch[]>();
            assets = new AssetManager();
            bankName = string.Empty;
            comment = string.Empty;
            LoadBank(bankFile);
        }
        public void LoadBank(IResource bankFile)
        {
            if (!bankFile.ReadAllowed())
                throw new Exception("The bank file provided does not have read access.");
            bank.Clear();
            assets.PatchAssetList.Clear();
            assets.SampleAssetList.Clear();
            bankName = string.Empty;
            comment = string.Empty;
            switch (IOHelper.GetExtension(bankFile.GetName()).ToLower())
            {
                case ".bank":
                    bankName = IOHelper.GetFileNameWithoutExtension(bankFile.GetName());
                    LoadMyBank(bankFile.OpenResourceForRead());
                    break;
                case ".sf2":
                    bankName = "SoundFont";
                    LoadSf2(bankFile.OpenResourceForRead());
                    break;
                default:
                    throw new Exception("Invalid bank resource was provided. An extension must be included in the resource name.");
            }
            assets.PatchAssetList.TrimExcess();
            assets.SampleAssetList.TrimExcess();
        }
        //public void LoadPatch(string patchFile, int bankNumber, int startRange, int endRange)
        //{
        //    string patchName = Path.GetFileNameWithoutExtension(patchFile);
        //    string directory = Path.GetDirectoryName(patchFile);
        //    //check for duplicate patch
        //    PatchAsset patchAsset = assets.FindPatch(patchName);
        //    if (patchAsset != null)
        //    {
        //        AssignPatchToBank(patchAsset.Patch, bankNumber, startRange, endRange);
        //        return;
        //    }
        //    //load patch here
        //    Patch patch;
        //    switch (Path.GetExtension(patchFile).ToLower())
        //    {
        //        case ".patch":
        //            patch = LoadMyPatch(CrossPlatformHelper.OpenResource(patchFile), patchName, directory);
        //            break;
        //        case ".sfz":
        //            patch = LoadSfzPatch(CrossPlatformHelper.OpenResource(patchFile), patchName, directory);
        //            break;
        //        default:
        //            throw new Exception("The patch: " + Path.GetFileName(patchFile) + " is unsupported.");
        //    }
        //    AssignPatchToBank(patch, bankNumber, startRange, endRange);
        //    assets.PatchAssetList.Add(new PatchAsset(patchName, patch));
        //}
        public int[] GetLoadedBanks()
        {
            int[] copy = new int[bank.Keys.Count];
            bank.Keys.CopyTo(copy, 0);
            return copy;
        }
        public Patch[] GetBank(int bankNumber)
        {
            if(bank.ContainsKey(bankNumber))
                return bank[bankNumber];
            return null;
        }
        public Patch GetPatch(int bankNumber, int patchNumber)
        {
            if (bank.ContainsKey(bankNumber))
            {
                return bank[bankNumber][patchNumber];
            }
            return null;
        }
        public Patch GetPatch(int bankNumber, string name)
        {
            if (bank.ContainsKey(bankNumber))
            {
                Patch[] patches = bank[bankNumber];
                for (int x = 0; x < patches.Length; x++)
                {
                    if (patches[x] != null && patches[x].Name.Equals(name))
                        return patches[x];
                }
            }
            return null;
        }
        public bool IsBankLoaded(int bankNumber)
        {
            return bank.ContainsKey(bankNumber);
        }

        //private Patch LoadMyPatch(Stream stream, string patchName, string directory)
        //{
        //    DescriptorList description;
        //    Patch patch;
        //    using (StreamReader reader = new StreamReader(stream))
        //    {
        //        if (!AssertCorrectVersion(ReadNextLine(reader)).Equals("patch"))
        //            throw new FormatException("Invalid patch version. Current version is: v" + string.Format("{0:0.000}", BankVersion) + " Patch ID: " + patchName);
        //        string patchTypeString = ReadNextLine(reader);
        //        Type type;
        //        if (patchTypes.TryGetValue(patchTypeString, out type))
        //            patch = (Patch)Activator.CreateInstance(type, new object[] { patchName });
        //        else
        //            throw new Exception("Invalid patch type \"" + patchTypeString + "\"! Patch ID: " + patchName);
        //        description = new DescriptorList(reader);
        //    }
        //    LoadSampleAssets(patchName, directory, description);
        //    patch.Load(description, assets);
        //    return patch;
        //}
        //private Patch LoadSfzPatch(Stream stream, string patchName, string directory)
        //{
        //    SfzReader sfz = new SfzReader(stream, patchName);
        //    MultiPatch multi = new MultiPatch(patchName);
        //    multi.LoadSfz(sfz.Regions, assets, directory);
        //    return multi;
        //}
        private void LoadMyBank(Stream stream)
        {
            using (BinaryReader reader = new BinaryReader(stream))
            {
                //read riff chunk
                string id = IOHelper.Read8BitString(reader, 4).ToLower();
                int size = reader.ReadInt32();
                if(!id.Equals("riff", StringComparison.InvariantCultureIgnoreCase))
                    throw new Exception("Invalid bank file. The riff header is missing.");
                if(!new RiffTypeChunk(id, size, reader).TypeId.Equals("bank", StringComparison.InvariantCultureIgnoreCase))
                    throw new Exception("Invalid bank file. The riff type is incorrect.");
                //read info chunk
                id = IOHelper.Read8BitString(reader, 4).ToLower();
                size = reader.ReadInt32();
                if (!id.Equals("info", StringComparison.InvariantCultureIgnoreCase))
                    throw new Exception("Invalid bank file. The INFO chunk is missing.");
                if (reader.ReadSingle() != BankVersion)
                    throw new Exception(string.Format("Invalid bank file. The bank version is incorrect, the correct version is {0:0.000}.", BankVersion));
                this.comment = IOHelper.Read8BitString(reader, size - 4);
                //read asset list chunk
                id = IOHelper.Read8BitString(reader, 4).ToLower();
                size = reader.ReadInt32();
                if (!id.Equals("list", StringComparison.InvariantCultureIgnoreCase))
                    throw new Exception("Invalid bank file. The ASET LIST chunk is missing.");
                long readTo = reader.BaseStream.Position + size;
                id = IOHelper.Read8BitString(reader, 4).ToLower();
                if (!id.Equals("aset", StringComparison.InvariantCultureIgnoreCase))
                    throw new Exception("Invalid bank file. The LIST chunk is not of type ASET.");
                //--read assets
                while(reader.BaseStream.Position < readTo)
                {
                    id = IOHelper.Read8BitString(reader, 4).ToLower();
                    size = reader.ReadInt32();
                    if (!id.Equals("smpl", StringComparison.InvariantCultureIgnoreCase))
                        throw new Exception("Invalid bank file. Only SMPL chunks are allowed in the asset list chunk.");
                    assets.SampleAssetList.Add(new SampleDataAsset(size, reader));
                }
                //read instrument list chunk
                id = IOHelper.Read8BitString(reader, 4).ToLower();
                size = reader.ReadInt32();
                if (!id.Equals("list", StringComparison.InvariantCultureIgnoreCase))
                    throw new Exception("Invalid bank file. The INST LIST chunk is missing.");
                readTo = reader.BaseStream.Position + size;
                id = IOHelper.Read8BitString(reader, 4).ToLower();
                if (!id.Equals("inst", StringComparison.InvariantCultureIgnoreCase))
                    throw new Exception("Invalid bank file. The LIST chunk is not of type INST.");
                //--read instruments
                while (reader.BaseStream.Position < readTo)
                {
                    id = IOHelper.Read8BitString(reader, 4).ToLower();
                    size = reader.ReadInt32();
                    if (!id.Equals("ptch", StringComparison.InvariantCultureIgnoreCase))
                        throw new Exception("Invalid bank file. Only PTCH chunks are allowed in the instrument list chunk.");
                    string patchName = IOHelper.Read8BitString(reader, 20);
                    string patchType = IOHelper.Read8BitString(reader, 4);
                    Patch patch;
                    Type type;
                    if (patchTypes.TryGetValue(patchType, out type))
                        patch = (Patch)Activator.CreateInstance(type, new object[] { patchName });
                    else
                        throw new Exception("Invalid patch type \"" + patchType + "\"! Patch ID: " + patchName);
                    patch.Load(new DescriptorList(reader), assets);
                    assets.PatchAssetList.Add(new PatchAsset(patchName, patch));
                    int rangeCount = reader.ReadInt16();
                    for (int x = 0; x < rangeCount; x++)
                    {
                        int bankNum = reader.ReadInt16();
                        int start = reader.ReadByte();
                        int end = reader.ReadByte();
                        AssignPatchToBank(patch, bankNum, start, end);
                    }
                }
            }
        }
        private void LoadSf2(Stream stream)
        {
            SoundFont sf = new SoundFont(stream);
            bankName = sf.Info.BankName;
            comment = sf.Info.Comments;
            //load samples
            for (int x = 0; x < sf.Presets.SampleHeaders.Length; x++)
                assets.SampleAssetList.Add(new SampleDataAsset(sf.Presets.SampleHeaders[x], sf.SampleData));
            //create instrument regions first
            Sf2Region[][] inst = ReadSf2Instruments(sf.Presets.Instruments);
            //load each patch
            foreach (PresetHeader p in sf.Presets.PresetHeaders)
            {
                Generator[] globalGens = null;
                int i;
                if (p.Zones[0].Generators.Length == 0 || p.Zones[0].Generators[p.Zones[0].Generators.Length - 1].GeneratorType != GeneratorEnum.Instrument)
                {
                    globalGens = p.Zones[0].Generators;
                    i = 1;
                }
                else
                    i = 0;
                List<Sf2Region> regionList = new List<Sf2Region>();
                while (i < p.Zones.Length)
                {
                    byte presetLoKey = 0;
                    byte presetHiKey = 127;
                    byte presetLoVel = 0;
                    byte presetHiVel = 127;
                    if (p.Zones[i].Generators[0].GeneratorType == GeneratorEnum.KeyRange)
                    {
                        if (BitConverter.IsLittleEndian)
                        {
                            presetLoKey = (byte)(p.Zones[i].Generators[0].AmountInt16 & 0xFF);
                            presetHiKey = (byte)((p.Zones[i].Generators[0].AmountInt16 >> 8) & 0xFF);
                        }
                        else
                        {
                            presetHiKey = (byte)(p.Zones[i].Generators[0].AmountInt16 & 0xFF);
                            presetLoKey = (byte)((p.Zones[i].Generators[0].AmountInt16 >> 8) & 0xFF);
                        }
                        if (p.Zones[i].Generators.Length > 1 && p.Zones[i].Generators[1].GeneratorType == GeneratorEnum.VelocityRange)
                        {
                            if (BitConverter.IsLittleEndian)
                            {
                                presetLoVel = (byte)(p.Zones[i].Generators[1].AmountInt16 & 0xFF);
                                presetHiVel = (byte)((p.Zones[i].Generators[1].AmountInt16 >> 8) & 0xFF);
                            }
                            else
                            {
                                presetHiVel = (byte)(p.Zones[i].Generators[1].AmountInt16 & 0xFF);
                                presetLoVel = (byte)((p.Zones[i].Generators[1].AmountInt16 >> 8) & 0xFF);
                            }
                        }
                    }
                    else if (p.Zones[i].Generators[0].GeneratorType == GeneratorEnum.VelocityRange)
                    {
                        if (BitConverter.IsLittleEndian)
                        {
                            presetLoVel = (byte)(p.Zones[i].Generators[0].AmountInt16 & 0xFF);
                            presetHiVel = (byte)((p.Zones[i].Generators[0].AmountInt16 >> 8) & 0xFF);
                        }
                        else
                        {
                            presetHiVel = (byte)(p.Zones[i].Generators[0].AmountInt16 & 0xFF);
                            presetLoVel = (byte)((p.Zones[i].Generators[0].AmountInt16 >> 8) & 0xFF);
                        }
                    }
                    if(p.Zones[i].Generators[p.Zones[i].Generators.Length - 1].GeneratorType == GeneratorEnum.Instrument)
                    {
                        Sf2Region[] insts = inst[p.Zones[i].Generators[p.Zones[i].Generators.Length - 1].AmountInt16];
                        for (int x = 0; x < insts.Length; x++)
                        {
                            byte instLoKey;
                            byte instHiKey;
                            byte instLoVel;
                            byte instHiVel;
                            if (BitConverter.IsLittleEndian)
                            {
                                instLoKey = (byte)(insts[x].Generators[(int)GeneratorEnum.KeyRange] & 0xFF);
                                instHiKey = (byte)((insts[x].Generators[(int)GeneratorEnum.KeyRange] >> 8) & 0xFF);
                                instLoVel = (byte)(insts[x].Generators[(int)GeneratorEnum.VelocityRange] & 0xFF);
                                instHiVel = (byte)((insts[x].Generators[(int)GeneratorEnum.VelocityRange] >> 8) & 0xFF);
                            }
                            else
                            {
                                instHiKey = (byte)(insts[x].Generators[(int)GeneratorEnum.KeyRange] & 0xFF);
                                instLoKey = (byte)((insts[x].Generators[(int)GeneratorEnum.KeyRange] >> 8) & 0xFF);
                                instHiVel = (byte)(insts[x].Generators[(int)GeneratorEnum.VelocityRange] & 0xFF);
                                instLoVel = (byte)((insts[x].Generators[(int)GeneratorEnum.VelocityRange] >> 8) & 0xFF);
                            }
                            if ((instLoKey <= presetHiKey && presetLoKey <= instHiKey) && (instLoVel <= presetHiVel && presetLoVel <= instHiVel))
                            {
                                Sf2Region r = new Sf2Region();
                                Array.Copy(insts[x].Generators, r.Generators, r.Generators.Length);
                                ReadSf2Region(r, globalGens, p.Zones[i].Generators, true);
                                regionList.Add(r);
                            }
                        }
                    }
                    i++;
                }
                MultiPatch mp = new MultiPatch(p.Name);
                mp.LoadSf2(regionList.ToArray(), assets);
                assets.PatchAssetList.Add(new PatchAsset(mp.Name, mp));
                AssignPatchToBank(mp, p.BankNumber, p.PatchNumber, p.PatchNumber);
            }
        }
        private Sf2Region[][] ReadSf2Instruments(Instrument[] instruments)
        {
            Sf2Region[][] regions = new Sf2Region[instruments.Length][];
            for (int x = 0; x < regions.Length; x++)
            {
                Generator[] globalGens = null;
                int i;
                if (instruments[x].Zones[0].Generators.Length == 0 ||
                    instruments[x].Zones[0].Generators[instruments[x].Zones[0].Generators.Length - 1].GeneratorType != GeneratorEnum.SampleID)
                {
                    globalGens = instruments[x].Zones[0].Generators;
                    i = 1;
                }
                else
                    i = 0;
                regions[x] = new Sf2Region[instruments[x].Zones.Length - i];
                for (int j = 0; j < regions[x].Length; j++)
                {
                    Sf2Region r = new Sf2Region();
                    r.ApplyDefaultValues();
                    ReadSf2Region(r, globalGens, instruments[x].Zones[j + i].Generators, false);
                    regions[x][j] = r;
                }
            }
            return regions;
        }
        private void ReadSf2Region(Sf2Region region, Generator[] globals, Generator[] gens, bool isRelative)
        {
            if (isRelative == false)
            {
                if (globals != null)
                {
                    for (int x = 0; x < globals.Length; x++)
                        region.Generators[(int)globals[x].GeneratorType] = globals[x].AmountInt16;
                }
                for (int x = 0; x < gens.Length; x++)
                    region.Generators[(int)gens[x].GeneratorType] = gens[x].AmountInt16;
            }
            else
            {
                List<Generator> genList = new List<Generator>(gens);
                if (globals != null)
                {
                    for (int x = 0; x < globals.Length; x++)
                    {
                        bool found = false;
                        for (int i = 0; i < genList.Count; i++)
                        {
                            if (genList[i].GeneratorType == globals[x].GeneratorType)
                            {
                                found = true;
                                break;
                            }
                        }
                        if (!found)
                            genList.Add(globals[x]);
                    }
                }
                for (int x = 0; x < genList.Count; x++)
                {
                    int value = (int)genList[x].GeneratorType;
                    if (value < 5 || value == 12 || value == 45 || value == 46 || value == 47 || value == 50 || value == 54 || value == 57 || value == 58)
                        continue;
                    else if (value == 43 || value == 44)
                    {//calculate intersect
                        byte lo_a;
                        byte hi_a;
                        byte lo_b;
                        byte hi_b;
                        if (BitConverter.IsLittleEndian)
                        {
                            lo_a = (byte)(region.Generators[value] & 0xFF);
                            hi_a = (byte)((region.Generators[value] >> 8) & 0xFF);
                            lo_b = (byte)(genList[x].AmountInt16 & 0xFF);
                            hi_b = (byte)((genList[x].AmountInt16 >> 8) & 0xFF);
                        }
                        else
                        {
                            hi_a = (byte)(region.Generators[value] & 0xFF);
                            lo_a = (byte)((region.Generators[value] >> 8) & 0xFF);
                            hi_b = (byte)(genList[x].AmountInt16 & 0xFF);
                            lo_b = (byte)((genList[x].AmountInt16 >> 8) & 0xFF);
                        }
                        lo_a = Math.Max(lo_a, lo_b);
                        hi_a = Math.Min(hi_a, hi_b);

                        if (lo_a > hi_a)
                            throw new Exception("Invalid sf2 region. The range generators do not intersect.");
                        if (BitConverter.IsLittleEndian)
                            region.Generators[value] = (short)(lo_a | (hi_a << 8));
                        else
                            region.Generators[value] = (short)((lo_a << 8) | hi_a);
                    }
                    else
                        region.Generators[value] += genList[x].AmountInt16;
                }
            }
        }

        //private void LoadSampleAssets(string patchName, string directory, DescriptorList description)
        //{
        //    for (int x = 0; x < description.GenDescriptions.Length; x++)
        //    {
        //        if (description.GenDescriptions[x].SamplerType == WaveformEnum.SampleData && !description.GenDescriptions[x].AssetName.Equals("null"))
        //        {
        //            assets.LoadSampleAsset(description.GenDescriptions[x].AssetName, patchName, directory);
        //        }
        //    }
        //}
        private void AssignPatchToBank(Patch patch, int bankNumber, int startRange, int endRange)
        {
            //make sure bank is valid
            if (bankNumber < 0)
                return;
            //make sure range is valid
            if (startRange > endRange)
            {
                int range = startRange;
                startRange = endRange;
                endRange = range;
            }
            if(startRange < 0 || startRange >= BankSize)
                throw new ArgumentOutOfRangeException("startRange");
            if (endRange < 0 || endRange >= BankSize)
                throw new ArgumentOutOfRangeException("endRange");
            //create bank if necessary and load assign patches
            Patch[] patches;
            if (bank.ContainsKey(bankNumber))
            {
                patches = bank[bankNumber];
            }
            else
            {
                patches = new Patch[BankSize];
                bank.Add(bankNumber, patches);
            }
            for (int x = startRange; x <= endRange; x++)
                patches[x] = patch;
        }
        
        private static string ReadNextLine(StreamReader reader)
        {
            while (!reader.EndOfStream)
            {
                string s = reader.ReadLine();
                int x = s.IndexOf('#');
                if (x >= 0)
                {
                    int y = s.IndexOf('#', x + 1);
                    if (y > x)
                        s = s.Remove(x, y - x);
                    else
                        s = s.Remove(x, s.Length - x);
                }
                if (!s.Trim().Equals(string.Empty))
                    return s;
            }
            return string.Empty;
        }
        private static string AssertCorrectVersion(string header)
        {
            string[] args = header.Split(new string[] { "v" }, StringSplitOptions.RemoveEmptyEntries);
            if (args.Length != 2 || float.Parse(args[1]) != BankVersion)
                return string.Empty;
            return args[0].Trim().ToLower();
        }
    }
}
