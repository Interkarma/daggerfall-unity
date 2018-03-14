// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut
// Contributors:    

using System.Collections.Generic;
using System;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Questing;
using System.IO;
using UnityEngine;

namespace DaggerfallWorkshop.Game.Questing
{
    public enum MembershipStatus
    {
        Nonmember = 'N',
        Member   = 'M',
        Akatosh  = 'T',
        Arkay    = 'A',
        Dibella  = 'D',
        Julianos = 'J',
        Kynareth = 'K',
        Mara     = 'R',
        Stendarr = 'S',
        Zenithar = 'Z',
    }

    struct QuestData
    {
        public string name;
        public string path;
        public string group;
        public char membership;
        public int minRep;
        public bool unitWildC;
    }

    public class QuestTablesManager
    {
        public const string questListPrefix = "QuestList-";
        private const string questPacksFolderName = "QuestPacks";

        // Quest data tables
        private Dictionary<FactionFile.GuildGroups, List<QuestData>> guilds;

        // Registered quest packs
        private static List<string> questPacks = new List<string>();

        public QuestTablesManager()
        {
            LoadQuestLists();
        }

        /// <summary>
        /// Gets Quest Packs folder in StreamingAssets.
        /// </summary>
        public string QuestPacksFolder
        {
            get { return Path.Combine(Application.streamingAssetsPath, questPacksFolderName); }
        }

        public static bool RegisterQuestPack(string name)
        {
            if (questPacks.Contains(name))
                return false;
            else
                questPacks.Add(name);
            return true;
        }

        public void LoadQuestLists()
        {
            guilds = new Dictionary<FactionFile.GuildGroups, List<QuestData>>();

            LoadQuestList(questListPrefix + "Classic", QuestMachine.QuestSourceFolder);
            LoadQuestList(questListPrefix + "DFU", QuestMachine.QuestSourceFolder);

            foreach(string packName in questPacks)
            {
                LoadQuestPack(packName);
            }
        }

        public Quest GetGuildQuest(FactionFile.GuildGroups guildGroup, MembershipStatus status, int factionId, int rep)
        {
#if UNITY_EDITOR    // Reload every time when in editor
            LoadQuestLists();
#endif
            List<QuestData> guildQuests;
            if (guilds.TryGetValue(guildGroup, out guildQuests))
            {
                MembershipStatus tplMemb = (guildGroup == FactionFile.GuildGroups.HolyOrder && status != MembershipStatus.Nonmember) ? MembershipStatus.Member : status;
                List<QuestData> pool = new List<QuestData>();
                foreach (QuestData quest in guildQuests)
                {
                    if ((status == (MembershipStatus)quest.membership || tplMemb == (MembershipStatus)quest.membership) &&
                        (rep >= quest.minRep || status == MembershipStatus.Nonmember) &&
                        (!quest.unitWildC || rep < quest.minRep + 10))
                    {
                        pool.Add(quest);
                    }
                }
                Debug.Log("Quest pool has " + pool.Count);
                // Choose random quest from pool and try to parse it
                if (pool.Count > 0)
                {
                    QuestData questData = pool[UnityEngine.Random.Range(0, pool.Count)];
                    try
                    {
                        return InstantiateQuest(questData, factionId);
                    }
                    catch (Exception ex)
                    {   // Log exception
                        DaggerfallUnity.LogMessage("Exception during quest compile: " + ex.Message, true);
                    }
                }
            }
            return null;
        }

        private Quest InstantiateQuest(QuestData questData, int factionId)
        {
            // Append extension if not present
            string questName = questData.name;
            if (!questName.EndsWith(".txt"))
                questName += ".txt";

            // Attempt to load quest source file
            string questFile = Path.Combine(questData.path, questName);
            if (!File.Exists(questFile))
                throw new Exception("Quest file " + questFile + " not found.");

            Quest quest = QuestMachine.Instance.ParseQuest(File.ReadAllLines(questFile));
            quest.FactionId = factionId;
            return quest;
        }

        private void LoadQuestPack(string packName)
        {
            // Attempt to find quest pack folder
            string path = Path.Combine(QuestPacksFolder, packName);

            if (!Directory.Exists(path))
            {
                Debug.LogErrorFormat("QuestPack directory {0} not found.", path);
            }
            else
            {
                string filePattern = questListPrefix + "*.txt";
                string[] listFiles = Directory.GetFiles(path, filePattern, SearchOption.AllDirectories);
                foreach (string listFile in listFiles)
                    LoadQuestList(listFile, path);
            }
        }

        private void LoadQuestList(string listFile, string questsPath)
        {
            Table table = new Table(QuestMachine.Instance.GetTableSourceText(listFile));
            for (int i = 0; i < table.RowCount; i++)
            {
                QuestData questData = new QuestData();
                questData.path = questsPath;
                string minRep = table.GetValue("minRep", i);
                if (minRep.EndsWith("X"))
                {
                    questData.unitWildC = true;
                    minRep = minRep.Replace("X", "0");
                }
                int d = 0;
                if (int.TryParse(minRep, out d))
                {
                    questData.name = table.GetValue("name", i);
                    questData.group = table.GetValue("group", i);
                    questData.membership = table.GetValue("membership", i)[0];
                    questData.minRep = d;

                    // Is the group a guild group?
                    if (Enum.IsDefined(typeof(FactionFile.GuildGroups), questData.group))
                    {
                        FactionFile.GuildGroups guildGroup = (FactionFile.GuildGroups) Enum.Parse(typeof(FactionFile.GuildGroups), questData.group);
                        List<QuestData> guildQuests;
                        if (!guilds.TryGetValue(guildGroup, out guildQuests))
                        {
                            guildQuests = new List<QuestData>();
                            guilds.Add(guildGroup, guildQuests);
                        }
                        guildQuests.Add(questData);
                    }
                    // else TODO other groups
                }
            }
        }

    }
}