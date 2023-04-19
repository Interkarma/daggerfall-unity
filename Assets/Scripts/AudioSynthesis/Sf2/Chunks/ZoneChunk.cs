using System;
using System.IO;
using DaggerfallWorkshop.AudioSynthesis.Util.Riff;

namespace DaggerfallWorkshop.AudioSynthesis.Sf2.Chunks
{
    public class ZoneChunk : Chunk
    {
        private class RawZoneData
        {
            public ushort generatorIndex;
            public ushort modulatorIndex;
            public ushort generatorCount;
            public ushort modulatorCount;
        }

        private RawZoneData[] zoneData;

        public ZoneChunk(string id, int size, BinaryReader reader)
            : base(id, size)
        {
            if (size % 4 != 0)
                throw new Exception("Invalid SoundFont. The presetzone chunk was invalid.");
            zoneData = new RawZoneData[size / 4];
            RawZoneData lastZone = null;
            for (int x = 0; x < zoneData.Length; x++)
            {
                RawZoneData z = new RawZoneData();
                z.generatorIndex = reader.ReadUInt16();
                z.modulatorIndex = reader.ReadUInt16();
                if (lastZone != null)
                {
                    lastZone.generatorCount = (ushort)(z.generatorIndex - lastZone.generatorIndex);
                    lastZone.modulatorCount = (ushort)(z.modulatorIndex - lastZone.modulatorIndex);
                }
                zoneData[x] = z;
                lastZone = z;
            }
        }

        public Zone[] ToZones(Modulator[] modulators, Generator[] generators)
        {
            Zone[] zones = new Zone[zoneData.Length - 1];
            for (int x = 0; x < zones.Length; x++)
            {
                RawZoneData rawZone = zoneData[x];
                Zone zone = new Zone();
                zone.Generators = new Generator[rawZone.generatorCount];
                Array.Copy(generators, rawZone.generatorIndex, zone.Generators, 0, rawZone.generatorCount);
                zone.Modulators = new Modulator[rawZone.modulatorCount];
                Array.Copy(modulators, rawZone.modulatorIndex, zone.Modulators, 0, rawZone.modulatorCount);
                zones[x] = zone;
            }
            return zones;
        }
    }
}
