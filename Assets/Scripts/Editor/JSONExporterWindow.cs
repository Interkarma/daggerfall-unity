// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallConnect.FallExe;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Utility;
using DaggerfallConnect.Save;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop
{
    /// <summary>
    /// Editor window for exporting certain information to JSON.
    /// NOTE:
    ///  As exported data can be manually updated, its possible that re-exporting data will overwrite changes.
    ///  One example is the ItemTemplates.txt which has several fixes post export.
    /// </summary>
    public class JSONExporterWindow : EditorWindow
    {
        const string fallExe = "FALL.EXE";
        const string magicDef = "MAGIC.DEF";
        const string spellsStd = "SPELLS.STD";
        const string windowTitle = "JSON Exporter";
        const string menuPath = "Daggerfall Tools/JSON Exporter";
        const string itemTemplatesFilename = "ItemTemplates.txt";
        const string magicItemTemplatesFilename = "MagicItemTemplates.txt";
        const string startingSpellsFilename = "StartingSpells.txt";

        DaggerfallUnity dfUnity;

        [MenuItem(menuPath)]
        static void Init()
        {
            JSONExporterWindow window = (JSONExporterWindow)EditorWindow.GetWindow(typeof(JSONExporterWindow));
            window.titleContent = new GUIContent(windowTitle);
        }

        void OnGUI()
        {
            if (!IsReady())
            {
                EditorGUILayout.HelpBox("DaggerfallUnity instance not ready. Have you set your Arena2 path?", MessageType.Info);
                return;
            }

            EditorGUILayout.HelpBox("Warning: Exporting new JSON files might overwrite manual edits to current files. Only export when necessary and check for changes in existing file before merging.", MessageType.Warning);

            if (GUILayout.Button("Generate 'ItemTemplates.txt'"))
            {
                string fallExePath = Path.Combine(Path.GetDirectoryName(dfUnity.Arena2Path), fallExe);
                string outputPath = Path.Combine(Application.dataPath, itemTemplatesFilename);
                CreateItemTemplatesJSON(fallExePath, outputPath);
            }

            if (GUILayout.Button("Generate 'MagicItemTemplates.txt'"))
            {
                string magicDefPath = Path.Combine(dfUnity.Arena2Path, magicDef);
                string outputPath = Path.Combine(Application.dataPath, magicItemTemplatesFilename);
                CreateMagicItemTemplatesJSON(magicDefPath, outputPath);
            }

            if (GUILayout.Button("Generate 'StartingSpells.txt'"))
            {
                string fallExePath = Path.Combine(Path.GetDirectoryName(dfUnity.Arena2Path), fallExe);
                string outputPath = Path.Combine(Application.dataPath, startingSpellsFilename);
                CreateStartingSpellsJSON(fallExePath, outputPath);
            }
        }

        bool IsReady()
        {
            if (!dfUnity)
                dfUnity = DaggerfallUnity.Instance;

            if (!dfUnity.IsReady || string.IsNullOrEmpty(dfUnity.Arena2Path))
                return false;

            return true;
        }

        #region JSON Exporters

        /// <summary>
        /// Initial implementation just dumps ItemDescription to JSON.
        /// </summary>
        /// <param name="fallExePath">Path to FALL.EXE containing item database.</param>
        /// <param name="outputPath">Output path for JSON file.</param>
        static void CreateItemTemplatesJSON(string fallExePath, string outputPath)
        {
            ItemsFile itemsFile = new ItemsFile(fallExePath);
            List<ItemTemplate> itemDescriptions = new List<ItemTemplate>(itemsFile.ItemsCount);
            for (int i = 0; i < itemsFile.ItemsCount; i++)
            {
                itemDescriptions.Add(itemsFile.GetItemDescription(i));
            }

            string json = SaveLoadManager.Serialize(itemDescriptions.GetType(), itemDescriptions);
            File.WriteAllText(outputPath, json);
        }

        static void CreateMagicItemTemplatesJSON(string magicDefPath, string outputPath)
        {
            MagicItemsFile magicItemsFile = new MagicItemsFile(magicDefPath);
            List<MagicItemTemplate> magicItems = magicItemsFile.MagicItemsList;
            string json = SaveLoadManager.Serialize(magicItems.GetType(), magicItems);
            File.WriteAllText(outputPath, json);
        }

        static void CreateStartingSpellsJSON(string fallExePath, string outputPath)
        {
            const int recordLength = 6;                        // Length of starting spells record per career
            const int casterRecordCount = 7;                   // Number of caster career spell records

            const long startingSpellsOffset = 0x1B064F;        // Offset into FALL.EXE for starting spell data (can be different based on .EXE version)
            /*
             Alternate offset: 0x1B513F
             Offset data in FALL.EXE should start like so (credit to Allofich for information)
                01 25 02 61 08 FF  // Mage
                08 2C 25 FF FF FF  // Spellsword / Custom class (if any of the primary or major skills is a magic skill)
                08 02 25 FF FF FF  // Battlemage
                01 25 02 61 08 FF  // Sorcerer
                61 02 01 25 FF FF  // Healer
                2C 02 FF FF FF FF  // Nightblade
                25 FF FF FF FF FF  // Bard
                ...
            */

            // Read all CLASS*.CFG files
            List<DFCareer> classList = new List<DFCareer>();
            string[] files = Directory.GetFiles(DaggerfallUnity.Instance.Arena2Path, "CLASS*.CFG");
            if (files != null && files.Length > 0)
            {
                for (int i = 0; i < files.Length - 1; i++)
                {
                    ClassFile classFile = new ClassFile(files[i]);
                    classList.Add(classFile.Career);
                }
            }

            // Get list of spells
            List<SpellRecord.SpellRecordData> standardSpells = DaggerfallSpellReader.ReadSpellsFile(Path.Combine(DaggerfallUnity.Instance.Arena2Path, spellsStd));

            // Read starting spell records for these classes
            byte[] record;
            FileProxy exeFile = new FileProxy(fallExePath, FileUsage.UseDisk, true);
            BinaryReader reader = exeFile.GetReader(startingSpellsOffset);
            List<CareerStartingSpells> careerList = new List<CareerStartingSpells>();
            for (int i = 0; i < casterRecordCount; i++)
            {
                CareerStartingSpells careerItem = new CareerStartingSpells()
                {
                    CareerIndex = i,
                    CareerName = classList[i].Name,
                };

                List<StartingSpell> spellsList = new List<StartingSpell>();
                record = reader.ReadBytes(recordLength);
                for (int j = 0; j < recordLength; j++)
                {
                    if (record[j] == 0xff)
                        continue;

                    // Get spell record data
                    // Some careers reference spells that don't exist in SPELLS.STD - skipping over these
                    SpellRecord.SpellRecordData spellRecordData;
                    if (!FindSpellByID(record[j], standardSpells, out spellRecordData))
                    {
                        //Debug.LogErrorFormat("Spell ID {0} not found while reading career '{1}'", record[j], careerItem.CareerName);
                        continue;
                    }

                    // Barbarian has !Nux Vomica (a poison) in their starting spell list? - skipping for now
                    if (spellRecordData.spellName.StartsWith("!"))
                            continue;

                    StartingSpell spellItem = new StartingSpell()
                    {
                        SpellID = record[j],
                        SpellName = spellRecordData.spellName,
                    };
                    spellsList.Add(spellItem);
                }
                careerItem.SpellsList = spellsList.ToArray();
                careerList.Add(careerItem);
            }

            // Output JSON file
            string json = SaveLoadManager.Serialize(careerList.GetType(), careerList);
            File.WriteAllText(outputPath, json);
        }

        static bool FindSpellByID(int id, List<SpellRecord.SpellRecordData> spells, out SpellRecord.SpellRecordData spellRecordDataOut)
        {
            spellRecordDataOut = null;
            foreach (var record in spells)
            {
                if (record.index == id)
                {
                    spellRecordDataOut = record;
                    return true;
                }
            }

            return false;
        }

        #endregion
    }
}