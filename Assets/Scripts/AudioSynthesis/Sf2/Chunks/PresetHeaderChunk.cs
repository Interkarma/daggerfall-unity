using System.IO;
using DaggerfallWorkshop.AudioSynthesis.Util.Riff;
using DaggerfallWorkshop.AudioSynthesis.Util;
using System;

namespace DaggerfallWorkshop.AudioSynthesis.Sf2.Chunks
{
    public class PresetHeaderChunk : Chunk
    {
        private class RawPreset
        {
            public string name;
            public ushort patchNumber;
            public ushort bankNumber;
            public ushort startPresetZoneIndex;
            public ushort endPresetZoneIndex;
            public uint library;
            public uint genre;
            public uint morphology;
        }

        private RawPreset[] rawPresets;

        public PresetHeaderChunk(string id, int size, BinaryReader reader)
            : base(id, size)
        {
            if (size % 38 != 0)
                throw new Exception("Invalid SoundFont. The preset chunk was invalid.");
            rawPresets = new RawPreset[size / 38];
            RawPreset lastPreset = null;
            for (int x = 0; x < rawPresets.Length; x++)
            {
                RawPreset p = new RawPreset();
                p.name = IOHelper.Read8BitString(reader, 20);
                p.patchNumber = reader.ReadUInt16();
                p.bankNumber = reader.ReadUInt16();
                p.startPresetZoneIndex = reader.ReadUInt16();
                p.library = reader.ReadUInt32();
                p.genre = reader.ReadUInt32();
                p.morphology = reader.ReadUInt32();
                if (lastPreset != null)
                    lastPreset.endPresetZoneIndex = (ushort)(p.startPresetZoneIndex - 1);
                rawPresets[x] = p;
                lastPreset = p;
            }
        }

        public PresetHeader[] ToPresets(Zone[] presetZones)
        {
            PresetHeader[] presets = new PresetHeader[rawPresets.Length - 1];
            for (int x = 0; x < presets.Length; x++)
            {
                RawPreset rawPreset = rawPresets[x];
                PresetHeader p = new PresetHeader();
                p.BankNumber = rawPreset.bankNumber;
                p.Genre = (int)rawPreset.genre;
                p.Library = (int)rawPreset.library;
                p.Morphology = (int)rawPreset.morphology;
                p.Name = rawPreset.name;
                p.PatchNumber = rawPreset.patchNumber;
                p.Zones = new Zone[rawPreset.endPresetZoneIndex - rawPreset.startPresetZoneIndex + 1];
                Array.Copy(presetZones, rawPreset.startPresetZoneIndex, p.Zones, 0, p.Zones.Length);
                presets[x] = p;
            }
            return presets;
        }
    }
}
