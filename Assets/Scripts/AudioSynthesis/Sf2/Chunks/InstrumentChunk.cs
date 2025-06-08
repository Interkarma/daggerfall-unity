using System;
using System.IO;
using DaggerfallWorkshop.AudioSynthesis.Util;
using DaggerfallWorkshop.AudioSynthesis.Util.Riff;

namespace DaggerfallWorkshop.AudioSynthesis.Sf2.Chunks
{
    public class InstrumentChunk : Chunk
    {
        private class RawInstrument
        {
            public string name;
            public ushort startInstrumentZoneIndex;
            public ushort endInstrumentZoneIndex;
        }

        private RawInstrument[] rawInstruments;

        public InstrumentChunk(string id, int size, BinaryReader reader)
            : base(id, size)
        {
            if (size % 22 != 0)
                throw new Exception("Invalid SoundFont. The preset chunk was invalid.");
            rawInstruments = new RawInstrument[size / 22];
            RawInstrument lastInstrument = null;
            for (int x = 0; x < rawInstruments.Length; x++)
            {
                RawInstrument i = new RawInstrument();
                i.name = IOHelper.Read8BitString(reader, 20);
                i.startInstrumentZoneIndex = reader.ReadUInt16();
                if (lastInstrument != null)
                    lastInstrument.endInstrumentZoneIndex = (ushort)(i.startInstrumentZoneIndex - 1);
                rawInstruments[x] = i;
                lastInstrument = i;
            }
        }

        public Instrument[] ToInstruments(Zone[] zones)
        {
            Instrument[] inst = new Instrument[rawInstruments.Length - 1];
            for (int x = 0; x < inst.Length; x++)
            {
                RawInstrument rawInst = rawInstruments[x];
                Instrument i = new Instrument();
                i.Name = rawInst.name;
                i.Zones = new Zone[rawInst.endInstrumentZoneIndex - rawInst.startInstrumentZoneIndex + 1];
                Array.Copy(zones, rawInst.startInstrumentZoneIndex, i.Zones, 0, i.Zones.Length);
                inst[x] = i;
            }
            return inst;
        }

    }
}
