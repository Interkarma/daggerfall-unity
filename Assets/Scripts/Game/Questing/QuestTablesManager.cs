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
        public string group;
        public char membership;
        public int minRep;
        public bool unitWildC;
    }

    public class QuestTablesManager
    {
        // Quest data tables
        private Dictionary<FactionFile.GuildGroups, List<QuestData>> guilds;

        public QuestTablesManager()
        {
            LoadQuestTables();
        }

        public void LoadQuestTables()
        {
            LoadQuestTable("Quests-Classic");
        }

        public Quest GetGuildQuest(FactionFile.GuildGroups guildGroup, MembershipStatus status, int factionId, int rep)
        {
#if UNITY_EDITOR    // Reload every time when in editor
            LoadQuestTables();
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
                UnityEngine.Debug.Log("Quest pool has " + pool.Count);
                // Choose random quest from pool and try to parse it
                if (pool.Count > 0)
                {
                    QuestData questData = pool[UnityEngine.Random.Range(0, pool.Count)];
                    try {
                        Quest quest = QuestMachine.Instance.ParseQuest(questData.name);
                        quest.FactionId = factionId;
                        return quest;
                    } catch (Exception ex) {
                        // Log exception
                        DaggerfallUnity.LogMessage("Exception during quest compile: " + ex.Message, true);
                    }
                }
            }
            return null;
        }

        private void LoadQuestTable(string filename)
        {
            guilds = new Dictionary<FactionFile.GuildGroups, List<QuestData>>();

            Table table = new Table(QuestMachine.Instance.GetTableSourceText(filename));
            for (int i = 0; i < table.RowCount; i++)
            {
                QuestData questData = new QuestData();
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