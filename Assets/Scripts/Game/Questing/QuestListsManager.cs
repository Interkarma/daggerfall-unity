// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2023 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut
// Contributors:    

using System.Collections.Generic;
using System;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Utility;
using System.IO;
using UnityEngine;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game.Questing
{
    public enum MembershipStatus
    {
        Nonmember = 'N',
        Member   = 'M',
        Prospect = 'P',
        Akatosh  = 'T',
        Arkay    = 'A',
        Dibella  = 'D',
        Julianos = 'J',
        Kynareth = 'K',
        Mara     = 'R',
        Stendarr = 'S',
        Zenithar = 'Z',
    }

    public struct QuestData
    {
        public string name;
        public string path;
        public string group;
        public char membership;
        public int minReq;
        public bool oneTime;    // flag = 1
        public bool adult;      // flag = X
    }

    /// <summary>
    /// Manager class for Quest Lists and Quest scripts
    /// 
    /// Quest lists are tables of quest names and metadata.
    /// They are discovered and loaded at startup time. (although loaded at runtime in editor)
    /// The files must be named: QuestList-{name}.txt
    /// 
    /// Quest scripts sit alongside list and must be uniquely named. They are loaded at runtime.
    ///
    /// Get quests by calling one of these methods:
    /// GetQuest()
    /// GetGuildQuest()
    /// GetSocialQuest()
    /// </summary>
    public class QuestListsManager
    {
        public const string InitAtGameStart = "InitAtGameStart";
        public const string QuestListPrefix = "QuestList-";
        private const string QuestListPattern = QuestListPrefix + "*.txt";
        private const string QuestPacksFolderName = "QuestPacks";
        private const string QExt = ".txt";

        // Quest data tables
        private Dictionary<FactionFile.GuildGroups, List<QuestData>> guilds;
        private Dictionary<FactionFile.SocialGroups, List<QuestData>> social;
        private List<QuestData> init;

        // List of one time quests player has previously accepted
        public List<string> oneTimeQuestsAccepted;

        // Registered quest lists
        private static List<string> questLists = new List<string>();

        // Constructor discovers and loads lists.
        public QuestListsManager()
        {
            DiscoverQuestPackLists();
            LoadQuestLists();

            QuestMachine.OnQuestStarted += QuestMachine_OnQuestStarted;
        }

        public void QuestMachine_OnQuestStarted(Quest quest)
        {
            // Record that this quest was accepted so it doesn't get offered again.
            if (quest.OneTime && oneTimeQuestsAccepted != null)
            {
                oneTimeQuestsAccepted.Add(quest.QuestName);
            }
        }

        #region Quest Packs

        /// <summary>
        /// Gets Quest Packs folder in StreamingAssets.
        /// </summary>
        public string QuestPacksFolder
        {
            get { return Path.Combine(Application.streamingAssetsPath, QuestPacksFolderName); }
        }

        public void DiscoverQuestPackLists()
        {
            string[] listFiles = Directory.GetFiles(QuestPacksFolder, QuestListPattern, SearchOption.AllDirectories);
            foreach (string listFile in listFiles)
                if (!RegisterQuestList(listFile))
                    Debug.LogErrorFormat("QuestList already registered. {0}", listFile);

            if (ModManager.Instance == null)
            {
                return;
            }

            foreach (var mod in ModManager.Instance.GetAllModsWithContributes(x => x.QuestLists != null))
            {
                foreach (var questList in mod.ModInfo.Contributes.QuestLists)
                {
                    if (!RegisterQuestList(questList))
                    {
                        Debug.LogErrorFormat("QuestList {0} is already registered.", questList);
                    }
                }
            }
        }

        #endregion

        #region Quest Lists

        /// <summary>
        /// Register a quest list contained in a mod. Only pass the name of the list, not the full filename.
        /// </summary>
        public static bool RegisterQuestList(string name)
        {
            DaggerfallUnity.LogMessage("RegisterQuestList: " + name, true);
            if (questLists.Contains(name))
                return false;
            else
                questLists.Add(name);
            return true;
        }

        /// <summary>
        /// Loads all the quest lists: default, discovered and registered.
        /// </summary>
        public void LoadQuestLists()
        {
            guilds = new Dictionary<FactionFile.GuildGroups, List<QuestData>>();
            social = new Dictionary<FactionFile.SocialGroups, List<QuestData>>();
            init = new List<QuestData>();

            LoadQuestList(QuestListPrefix + "Classic", QuestMachine.QuestSourceFolder);
            LoadQuestList(QuestListPrefix + "DFU", QuestMachine.QuestSourceFolder);

            foreach (string questList in questLists)
                LoadQuestList(questList);
        }

        private void LoadQuestList(string questList)
        {
            // Attempt to load quest pack quest list
            if (File.Exists(questList))
            {
                string questsPath = questList.Substring(0, questList.LastIndexOf(Path.DirectorySeparatorChar));
                LoadQuestList(questList, questsPath);
            }
            else
            {
                // Seek from mods using pattern: QuestList-<packName>.txt
                TextAsset questListAsset;
                string fileName = QuestListPrefix + questList + QExt;
                if (ModManager.Instance != null && ModManager.Instance.TryGetAsset(fileName, false, out questListAsset))
                {
                    try {
                        List<string> lines = ModManager.GetTextAssetLines(questListAsset);
                        Table table = new Table(lines.ToArray());
                        ParseQuestList(table);
                    } catch (Exception ex) {
                        Debug.LogErrorFormat("QuestListsManager unable to parse quest list table {0} with exception message {1}", questListAsset.name, ex.Message);
                    }
                }
                else
                {
                    Debug.LogErrorFormat("QuestList {0} not found in a mod or in quest packs folder.", questList);
                }
            }
        }

        private void LoadQuestList(string questListFilename, string questsPath)
        {
            try {
                Table table = new Table(QuestMachine.Instance.GetTableSourceText(questListFilename));
                ParseQuestList(table, questsPath);
            } catch (Exception ex) {
                Debug.LogErrorFormat("QuestListsManager unable to parse quest list table {0} with exception message {1}", questListFilename, ex.Message);
            }
        }

        private void ParseQuestList(Table questsTable, string questsPath = "")
        {
            for (int i = 0; i < questsTable.RowCount; i++)
            {
                QuestData questData = new QuestData();
                questData.path = questsPath;
                string minRep = questsTable.GetValue("minReq", i);
                int d = 0;
                if (int.TryParse(minRep, out d))
                {
                    questData.name = questsTable.GetValue("name", i);
                    questData.group = questsTable.GetValue("group", i);
                    questData.membership = questsTable.GetValue("membership", i)[0];
                    questData.minReq = d;
                    char flag = questsTable.GetValue("flag", i)[0];
                    questData.oneTime = (flag == '1');
                    questData.adult = (flag == 'X');

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
                    // Is the group a social group?
                    else if (Enum.IsDefined(typeof(FactionFile.SocialGroups), questData.group))
                    {
                        FactionFile.SocialGroups socialGroup = (FactionFile.SocialGroups) Enum.Parse(typeof(FactionFile.SocialGroups), questData.group);
                        List<QuestData> socialQuests;
                        if (!social.TryGetValue(socialGroup, out socialQuests))
                        {
                            socialQuests = new List<QuestData>();
                            social.Add(socialGroup, socialQuests);
                        }
                        socialQuests.Add(questData);
                    }
                    // Is this a quest initialised when a new game is started?
                    else if (questData.group == InitAtGameStart)
                    {
                        init.Add(questData);
                    }
                    // else TODO other groups
                }
            }
        }

        #endregion

        #region Quest Loading

        /// <summary>
        /// Initialises and starts any quests marked InitAtGameStart
        /// </summary>
        public void InitAtGameStartQuests()
        {
            foreach (QuestData questData in init)
            {
                Quest quest = LoadQuest(questData, 0);
                if (quest == null)
                    continue;

                QuestMachine.Instance.StartQuest(quest);
            }
        }

        /// <summary>
        /// Get a specific named quest from any registered lists, or from QuestMachine.QuestSourceFolder property path.
        /// NOTE: Since this is driven from quest name, any duplicate names will result in the first in order of precedence being returned.
        /// </summary>
        public Quest GetQuest(string questName, int factionId = 0)
        {
            // First check QuestSourceFolder containing classic quests.
            string questFileName = questName + QExt;
            string questFile = Path.Combine(QuestMachine.QuestSourceFolder, questFileName);
            if (File.Exists(questFile))
                return LoadQuest(questName, QuestMachine.QuestSourceFolder, factionId);

            // Check each registered init quest & containing folder.
            foreach (QuestData questData in init)
            {
                if (questData.name == questName)
                    return LoadQuest(questData, factionId);

                questFile = Path.Combine(questData.path, questFileName);
                if (File.Exists(questFile))
                    return LoadQuest(questName, questData.path, factionId);
            }

            // Check guild quests.
            foreach (FactionFile.GuildGroups guildGroup in guilds.Keys)
                foreach (QuestData questData in guilds[guildGroup])
                    if (questData.name == questName)
                        return LoadQuest(questData, factionId);

            // Check social quests.
            foreach (FactionFile.SocialGroups socialGroup in social.Keys)
                foreach (QuestData questData in social[socialGroup])
                    if (questData.name == questName)
                        return LoadQuest(questData, factionId);

            return null;
        }

        /// <summary>
        /// Get a random quest for a guild from appropriate subset.
        /// </summary>
        public Quest GetGuildQuest(FactionFile.GuildGroups guildGroup, MembershipStatus status, int factionId, int rep, int rank)
        {
            List<QuestData> pool = GetGuildQuestPool(guildGroup, status, factionId, rep, rank);
            return SelectQuest(pool, factionId);
        }

        /// <summary>
        /// Gets a pool of elligible quests for a guild to offer.
        /// </summary>
        public List<QuestData> GetGuildQuestPool(FactionFile.GuildGroups guildGroup, MembershipStatus status, int factionId, int rep, int rank)
        {
#if UNITY_EDITOR    // Reload every time when in editor
            LoadQuestLists();
#endif

            // Create one-time quest list if not already created
            if (oneTimeQuestsAccepted == null)
                oneTimeQuestsAccepted = new List<string>();

            List<QuestData> guildQuests;
            if (guilds.TryGetValue(guildGroup, out guildQuests))
            {
                // Modifications for Temple dual membership status
                MembershipStatus tplMemb = (guildGroup == FactionFile.GuildGroups.HolyOrder && status != MembershipStatus.Nonmember) ? MembershipStatus.Member : status;

                List<QuestData> pool = new List<QuestData>();
                foreach (QuestData quest in guildQuests)
                {
                    if ((status == (MembershipStatus)quest.membership || tplMemb == (MembershipStatus)quest.membership) &&
                        ((status == MembershipStatus.Nonmember && quest.minReq == 0) || (quest.minReq < 10 && quest.minReq <= rank) || (quest.minReq >= 10 && quest.minReq <= rep)))
                    {
                        if ((!quest.adult || DaggerfallUnity.Settings.PlayerNudity) && !(quest.oneTime && oneTimeQuestsAccepted.Contains(quest.name)))
                            pool.Add(quest);
                    }
                }
                return pool;
            }
            return null;
        }

        public Quest GetSocialQuest(FactionFile.SocialGroups socialGroup, int factionId, Genders gender, int rep, int level)
        {
#if UNITY_EDITOR    // Reload every time when in editor
            LoadQuestLists();
#endif
            // Create one-time quest list if not already created
            if (oneTimeQuestsAccepted == null)
                oneTimeQuestsAccepted = new List<string>();

            List<QuestData> socialQuests;
            if (social.TryGetValue(socialGroup, out socialQuests))
            {
                List<QuestData> pool = new List<QuestData>();
                foreach (QuestData quest in socialQuests)
                {
                    if (((quest.minReq < 10 && quest.minReq <= level) || rep >= quest.minReq) &&
                        (quest.membership == 'N' ||
                         (quest.membership == 'M' && gender == Genders.Male) ||
                         (quest.membership == 'F' && gender == Genders.Female)))
                    {
                        if ((!quest.adult || DaggerfallUnity.Settings.PlayerNudity) && !(quest.oneTime && oneTimeQuestsAccepted.Contains(quest.name)))
                            pool.Add(quest);
                    }
                }
                return SelectQuest(pool, factionId);
            }
            return null;
        }

        public Quest SelectQuest(List<QuestData> pool, int factionId)
        {
            Debug.Log("Quest pool has " + pool.Count);
            // Choose random quest from pool and try to parse it
            if (pool.Count > 0)
            {
                QuestData questData = pool[UnityEngine.Random.Range(0, pool.Count)];
                try
                {
                    return LoadQuest(questData, factionId);
                }
                catch (Exception ex)
                {   // Log exception
                    DaggerfallUnity.LogMessage("Exception for quest " + questData.name + " during quest compile: " + ex.Message, true);
                }
            }
            return null;
        }

        private Quest LoadQuest(string questName, string questPath, int factionId)
        {
            return LoadQuest(new QuestData() { name = questName, path = questPath }, factionId);
        }

        /// <summary>
        /// Loads a quest script from the quest data.
        /// </summary>
        /// <param name="questData">The quest data object of the quest to load.</param>
        /// <param name="factionId">Faction id that should get the rep change for quest success/failure.</param>
        /// <param name="partialParse">If true the quest will only be partially parsed and cannot be instantiated.</param>
        /// <returns></returns>
        public Quest LoadQuest(QuestData questData, int factionId, bool partialParse = false)
        {
            // Append extension if not present
            string questName = questData.name;
            if (!questName.EndsWith(QExt))
                questName += QExt;

            // Attempt to load quest source file
            Quest quest;
            string questFile = Path.Combine(questData.path, questName);
            if (File.Exists(questFile))
            {
                quest = QuestMachine.Instance.ParseQuest(questName, File.ReadAllLines(questFile), factionId, partialParse);
                if (quest == null)
                    return null;
            }
            else
            {
                // Seek from mods using name
                TextAsset questAsset;
                if (ModManager.Instance != null && ModManager.Instance.TryGetAsset(questName, false, out questAsset))
                {
                    List<string> lines = ModManager.GetTextAssetLines(questAsset);
                    quest = QuestMachine.Instance.ParseQuest(questName, lines.ToArray(), factionId, partialParse);
                    if (quest == null)
                        return null;
                }
                else
                    throw new Exception("Quest file " + questFile + " not found.");
            }
            quest.OneTime = questData.oneTime;
            return quest;
        }

        #endregion
    }
}