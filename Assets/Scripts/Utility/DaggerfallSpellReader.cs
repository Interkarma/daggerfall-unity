// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Lypyl (lypyldf@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using DaggerfallConnect.Save;
using FullSerializer;

namespace DaggerfallWorkshop.Utility
{
    /// <summary>
    /// Reads and parses spell data from spells.std and classic saves. 
    /// </summary>
    public static class DaggerfallSpellReader
    {
        /// <summary> Byte size of an individual spell record. </summary>
        const int SPELLRECORDSIZE               = 0x59;
        const int EFFECT_DESCR_TEXTINDEX        = 1200;
        const int SPELLMAKER_DESCR_TEXTINDEX    = 1500;
        public const string DEFAULT_FILENAME    = "SPELLS.STD";


        /// <summary>
        /// Parses a SPELLS.STD file.
        /// </summary>
        /// <param name="filePath">If null, looks for SPELLS.STD in Arena2 path.</param>
        /// <returns>List of SpellRecordData structs.</returns>
        public static List<SpellRecord.SpellRecordData> ReadSpellsFile(string filePath = null)
        {
            var managedFile = new DaggerfallConnect.Utility.FileProxy();

            if(string.IsNullOrEmpty(filePath))
                filePath = Path.Combine(DaggerfallUnity.Instance.Arena2Path, DEFAULT_FILENAME);
            
            if (!File.Exists(filePath))
            {
                Debug.LogError(string.Format("{0} file not found", DEFAULT_FILENAME));
                return null;
            }
            else if (!managedFile.Load(filePath, DaggerfallConnect.FileUsage.UseMemory, true))
            {
                Debug.LogError(string.Format("Failed to load {0} file\n{1}", DEFAULT_FILENAME, managedFile.LastException.InnerException));
                return null;
            }

            var spells  = new List<SpellRecord.SpellRecordData>(100);
            var fileReader      = managedFile.GetReader();

            try
            {
                while (fileReader.BaseStream.Position + SPELLRECORDSIZE <= fileReader.BaseStream.Length)
                {
                    SpellRecord.SpellRecordData spellRecord = new SpellRecord.SpellRecordData();
                    bool succeded = ReadSpellData(fileReader, out spellRecord);

                    if (succeded)
                        spells.Add(spellRecord);
                    else
                        Debug.LogError(string.Format("Failed to read spell at: {0}", fileReader.BaseStream.Position));

                }
            }
            catch (Exception ex)
            {
                Debug.LogError(string.Format("error while reading {0} : {1}", filePath, ex.Message));
                managedFile.Close();
            }

            managedFile.Close();
            return spells;
        }

        /// <summary>
        /// Creates a spell record from byte array.
        /// </summary>
        /// <param name="chunk">Input spell data; this is an array of bytes with length <see cref="SPELLRECORDSIZE"/>.</param>
        /// <param name="spellRecord">Resulting spell record data.</param>
        /// <returns>True if succeeded.</returns>
        public static bool ReadSpellData(byte[] chunk, out SpellRecord.SpellRecordData spellRecord)
        {
            spellRecord = new SpellRecord.SpellRecordData();
            bool succeeded = false;

            try
            {
                if (chunk == null || chunk.Length < SPELLRECORDSIZE)
                    return succeeded;

                MemoryStream stream = new MemoryStream(chunk);
                BinaryReader reader = new BinaryReader(stream);
                succeeded = ReadSpellData(reader, out spellRecord);

            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                succeeded = false;
            }

            return succeeded;
        }

        /// <summary>
        /// Creates a spell record
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="spellRecord"></param>
        /// <returns></returns>
        public static bool ReadSpellData(BinaryReader reader, out SpellRecord.SpellRecordData spellRecord)
        {
            spellRecord = new SpellRecord.SpellRecordData();

            try
            {
                if (reader == null || reader.BaseStream == null)
                {
                    DaggerfallUnity.LogMessage("Spell Chunk Array was null", true);
                    return false;
                }
                else if (reader.BaseStream.Position + SPELLRECORDSIZE > reader.BaseStream.Length)
                    return false;

                if (!SetSpellTypes(ref spellRecord, reader))
                    return false;

                spellRecord.element     = reader.ReadByte();
                spellRecord.rangeType   = reader.ReadByte();
                spellRecord.cost        = reader.ReadUInt16();
                reader.BaseStream.Seek(4, SeekOrigin.Current);

                if (!SetSpellDurations(ref spellRecord, reader))
                    return false;
                if (!SetSpellChances(ref spellRecord, reader))
                    return false;
                if (!SetSpellMagnitudes(ref spellRecord, reader))
                    return false;

                spellRecord.spellName   = DaggerfallConnect.Utility.FileProxy.ReadCStringSkip(reader, 0, 25);
                //spellRecord.spellName   = spellRecord.spellName.TrimEnd(new char[] { '\0' });
                spellRecord.icon        = reader.ReadByte();
                spellRecord.index       = reader.ReadByte();
                reader.BaseStream.Seek(15, SeekOrigin.Current);
            }
            catch (Exception ex)
            {
                DaggerfallUnity.LogMessage(ex.Message, true);
                return false;
            }

            return true;

        }

        private static bool SetSpellTypes(ref SpellRecord.SpellRecordData spell, BinaryReader reader)
        {
            if (reader == null)
            {
                return false;
            }

            if (spell.effects == null || spell.effects.Length < 3)
                spell.effects = new SpellRecord.EffectRecordData[3];

            SpellRecord.EffectRecordData[] effects = spell.effects;

            for (int i = 0; i < spell.effects.Length; i++)
            {
                effects[i].type = reader.ReadSByte();

                if (effects[i].type == 0xFF)
                    continue;
                else
                    effects[i].subType = reader.ReadSByte();
            }

            return (effects[0].type > -1 || effects[1].type > -1 || effects[2].type > -1);
        }

        //just setting to 0 for now, need to implement a lookup
        private static bool SetTextIndices(ref SpellRecord.SpellRecordData spell, BinaryReader reader)
        {
            if (reader == null)
                return false;
            else if (spell.effects == null)
                return false;

            for (int i = 0; i < spell.effects.Length; i++)
            {
                spell.effects[i].descriptionTextIndex = 0;
                spell.effects[i].spellMakerTextIndex = 0;
            }

            return true;
        }


        private static bool SetSpellDurations(ref SpellRecord.SpellRecordData spell, BinaryReader reader)
        {
            if (reader == null)
                return false;
            else if (spell.effects == null || spell.effects.Length < 3)
                return false;

            SpellRecord.EffectRecordData[] effects = spell.effects;

            for (int i = 0; i < effects.Length; i++)
            {
                effects[i].durationBase = reader.ReadByte();
                effects[i].durationMod = reader.ReadByte();
                effects[i].durationPerLevel = reader.ReadByte();
            }

            return true;
        }

        private static bool SetSpellChances(ref SpellRecord.SpellRecordData spell, BinaryReader reader)
        {
            if (reader == null)
                return false;
            else if (spell.effects == null || spell.effects.Length < 3)
                return false;

            SpellRecord.EffectRecordData[] effects = spell.effects;

            for (int i = 0; i < effects.Length; i++)
            {
                effects[i].chanceBase       = reader.ReadByte();
                effects[i].chanceMod        = reader.ReadByte();
                effects[i].chancePerLevel   = reader.ReadByte();
            }

            return true;
        }

        private static bool SetSpellMagnitudes(ref SpellRecord.SpellRecordData spell, BinaryReader reader)
        {
            if (reader == null)
                return false;
            else if (spell.effects == null || spell.effects.Length < 3)
                return false;

            SpellRecord.EffectRecordData[] effects = spell.effects;

            for (int i = 0; i < effects.Length; i++)
            {
                effects[i].magnitudeBaseLow        = reader.ReadByte();
                effects[i].magnitudeBaseHigh   = reader.ReadByte();
                effects[i].magnitudeLevelBase   = reader.ReadByte();
                effects[i].magnitudeLevelHigh  = reader.ReadByte();
                effects[i].magnitudePerLevel    = reader.ReadByte();
            }

            return true;
        }

        #region json
        /// <summary>
        /// Serializes spell record to json
        /// </summary>
        /// <param name="spell"></param>
        /// <returns></returns>
        public static string SerializeSpell(SpellRecord.SpellRecordData spell)
        {
            return DoSerialize<SpellRecord.SpellRecordData>(spell);
        }

        /// <summary>
        /// serializes array of spells to json
        /// </summary>
        /// <param name="spell"></param>
        /// <returns></returns>
        public static string SerializeSpells(List<SpellRecord.SpellRecordData> spells)
        {
            return DoSerialize<List<SpellRecord.SpellRecordData>>(spells);
        }

        /// <summary>
        /// Deserializes array of spell records 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static List<SpellRecord.SpellRecordData> DeserializeSpells(string data)
        {
            List<SpellRecord.SpellRecordData> spells;
            DoDeserialize<List<SpellRecord.SpellRecordData>>(data, out spells);
            return spells;
        }

        /// <summary>
        /// Deserializes single
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static SpellRecord.SpellRecordData DeserializeSpell(string data)
        {
            SpellRecord.SpellRecordData spell;
            DoDeserialize<SpellRecord.SpellRecordData>(data, out spell);
            return spell;
        }

        private static bool DoDeserialize<T>(string textdata, out T obj)
        {
            obj = default(T);
            bool succeeded = false;

            if (string.IsNullOrEmpty(textdata))
            {
                Debug.LogError("nothing to deserialize");
                return succeeded;
            }

            try
            {
                fsData data = FullSerializer.fsJsonParser.Parse(textdata);
                fsSerializer serializer = new fsSerializer();
                serializer.TryDeserialize<T>(data, ref obj);
                succeeded = true;
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.Message);
                succeeded = false;
            }

            return succeeded;

        }

        private static string DoSerialize<T>(T spell)
        {
            fsData data;
            fsSerializer serializer = new fsSerializer();
            serializer.TrySerialize(typeof(T), spell, out data);
            string results = FullSerializer.fsJsonPrinter.PrettyJson(data);
            return results;
        }

        #endregion
    }

    #region format_info
    /*
     * 
     * classic spell format:
     * 
     * 89 bytes
     * 
     * effect types     (6 bytes)
     * effect type
     * effect subtype 
     * effect type
     * effect subtype 
     * effect type
     * effect subtype 
     * 
     * element          (1 byte)
     * range            (1 byte)
     * cost             (2 byte)
     * 
     * gap              (4 bytes)
     * 
     * durations        (9 bytes)
     * duration_base
     * duration_mod
     * duration_perlevel
     * duration_base
     * duration_mod
     * duration_perlevel
     * duration_base
     * duration_mod
     * duration_perlevel
     * 
     * chances          (9 bytes)
     * chance_base
     * chance_mod
     * chance_perlevel
     * chance_base
     * chance_mod
     * chance_perlevel
     * chance_base
     * chance_mod
     * chance_perlevel
     * 
     * magnitudes       (15 bytes)
     * magnitude_base
     * magnitude_base_mod
     * magnitude_perLevelBase
     * magnitude_perlevelMod
     * magnitude_perlevel
     * magnitude_base
     * magnitude_base_mod
     * magnitude_perLevelBase
     * magnitude_perlevelMod
     * magnitude_perlevel
     * magnitude_base
     * magnitude_base_mod
     * magnitude_perLevelBase
     * magnitude_perlevelMod
     * magnitude_perlevel
     * 
     * spell name       (25 bytes)
     * spell icon       (1 byte)
     * spell index      (1 byte)
     * 
     * gap              (15 bytes)
     */
    #endregion
}
