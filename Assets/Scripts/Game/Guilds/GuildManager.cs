// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2023 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut
// Contributors:    

using UnityEngine;
using System.Collections.Generic;
using System;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.Player;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game.Questing;
using Wenzil.Console;
using Wenzil.Console.Commands;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallConnect.Save;

namespace DaggerfallWorkshop.Game.Guilds
{
    public class GuildManager
    {
        // A base guild class defining non-membership.
        public static Guild guildNotMember = new NonMemberGuild();
        // A base temple class defining non-membership.
        public static Guild templeNotMember = new NonMemberGuild(true);

        // Custom guild registry. Can override vanilla guilds.
        private static Dictionary<FactionFile.GuildGroups, Type> customGuilds = new Dictionary<FactionFile.GuildGroups, Type>();

        public static bool RegisterCustomGuild(FactionFile.GuildGroups guildGroup, Type guildType)
        {
            DaggerfallUnity.LogMessage("RegisterCustomGuild: " + guildGroup, true);
            if (!customGuilds.ContainsKey(guildGroup))
            {
                customGuilds.Add(guildGroup, guildType);
                return true;
            }
            return false;
        }

        public GuildManager()
        {
            // Listen for quest end events which trigger joining TG & DB.
            QuestMachine.OnQuestEnded += QuestMachine_OnQuestEnded;

            // Register console commands
            GuildConsoleCommands.RegisterCommands();
        }

        public void QuestMachine_OnQuestEnded(Quest quest)
        {
            if (quest.QuestSuccess)
            {
                if (quest.QuestName == ThievesGuild.InitiationQuestName)
                {
                    IGuild tg = CreateGuildObj(FactionFile.GuildGroups.GeneralPopulace);
                    AddMembership(FactionFile.GuildGroups.GeneralPopulace, tg);
                }
                if (quest.QuestName == DarkBrotherhood.InitiationQuestName)
                {
                    IGuild db = CreateGuildObj(FactionFile.GuildGroups.DarkBrotherHood);
                    AddMembership(FactionFile.GuildGroups.DarkBrotherHood, db);
                }
            }
        }

        /// <summary>
        /// Get the faction id for a guild group. (used for non-member quests)
        /// Returns 0 for HolyOrder and KnightlyOrder since they have variants each with different faction ids.
        /// </summary>
        public int GetGuildFactionId(FactionFile.GuildGroups guildGroup)
        {
            switch (guildGroup)
            {
                case FactionFile.GuildGroups.FightersGuild:
                    return FightersGuild.FactionId;

                case FactionFile.GuildGroups.MagesGuild:
                    return MagesGuild.FactionId;

                case FactionFile.GuildGroups.HolyOrder:
                case FactionFile.GuildGroups.KnightlyOrder:
                    return 0;

                case FactionFile.GuildGroups.GeneralPopulace:
                    return ThievesGuild.FactionId;

                case FactionFile.GuildGroups.DarkBrotherHood:
                    return DarkBrotherhood.FactionId;

                default:
                    Type guildType;
                    if (customGuilds.TryGetValue(guildGroup, out guildType))
                        return (int)guildType.GetProperty("FactionId").GetValue(null, null);
                    else
                        return 0;
            }
        }

        #region Guild membership handling

        private readonly Dictionary<FactionFile.GuildGroups, IGuild> memberships = new Dictionary<FactionFile.GuildGroups, IGuild>();

        private readonly Dictionary<FactionFile.GuildGroups, IGuild> vampMemberships = new Dictionary<FactionFile.GuildGroups, IGuild>();

        private Dictionary<FactionFile.GuildGroups, IGuild> Memberships
        {
            get { return GameManager.Instance.PlayerEffectManager.HasVampirism() ? vampMemberships : memberships; }
        }

        public List<IGuild> GetMemberships()
        {
            List<IGuild> guilds = new List<IGuild>();
            foreach (IGuild guild in Memberships.Values)
                guilds.Add(guild);
            return guilds;
        }

        public void AddMembership(FactionFile.GuildGroups guildGroup, IGuild guild)
        {
            guild.Join();
            Memberships[guildGroup] = guild;
        }

        public void RemoveMembership(IGuild guild)
        {
            FactionFile.GuildGroups guildGroup = FactionFile.GuildGroups.None;
            foreach (FactionFile.GuildGroups group in Memberships.Keys)
            {
                if (Memberships[group] == guild)
                {
                    guildGroup = group;
                    break;
                }
            }
            if (guildGroup != FactionFile.GuildGroups.None)
            {
                guild.Leave();
                Memberships.Remove(guildGroup);
            }
        }

        public bool HasJoined(FactionFile.GuildGroups guildGroup)
        {
            return Memberships.ContainsKey(guildGroup);
        }

        public bool GetJoinedGuildOfGuildGroup(FactionFile.GuildGroups guildGroup, out IGuild value)
        {
            if (Memberships.TryGetValue(guildGroup, out value))
                return true;

            return false;
        }

        public IGuild JoinGuild(FactionFile.GuildGroups guildGroup, int buildingFactionId = 0)
        {
            if (Memberships.ContainsKey(guildGroup))
                return Memberships[guildGroup];

            return CreateGuildObj(guildGroup, buildingFactionId);
        }

        private IGuild CreateGuildObj(FactionFile.GuildGroups guildGroup, int variant = 0)
        {
            Type guildType;
            if (customGuilds.TryGetValue(guildGroup, out guildType))
            {
                switch (guildGroup)
                {
                    case FactionFile.GuildGroups.HolyOrder:
                        return (IGuild)Activator.CreateInstance(guildType, new object[] { Temple.GetDivine(variant) });

                    case FactionFile.GuildGroups.KnightlyOrder:
                        return (IGuild)Activator.CreateInstance(guildType, new object[] { KnightlyOrder.GetOrder(variant) });

                    default:
                        if (guildType.GetConstructor(new Type[] { typeof(int) }) != null)
                            return (IGuild)Activator.CreateInstance(guildType, new object[] { variant });
                        else
                            return (IGuild)Activator.CreateInstance(guildType);
                }
            }
            else
            {
                switch (guildGroup)
                {
                    case FactionFile.GuildGroups.FightersGuild:
                        return new FightersGuild();

                    case FactionFile.GuildGroups.MagesGuild:
                        return new MagesGuild();

                    case FactionFile.GuildGroups.HolyOrder:
                        return new Temple(Temple.GetDivine(variant));

                    case FactionFile.GuildGroups.KnightlyOrder:
                        return new KnightlyOrder(KnightlyOrder.GetOrder(variant));

                    case FactionFile.GuildGroups.GeneralPopulace:
                        return new ThievesGuild();

                    case FactionFile.GuildGroups.DarkBrotherHood:
                        return new DarkBrotherhood();

                    default:
                        return null;
                }
            }
        }

        /// <summary>
        /// Retrieve the guild object for the given guild group.
        /// </summary>
        /// <param name="guildGroup"></param>
        /// <param name="buildingFactionId">Specify this to ensure only the temple of matching Divine is returned</param>
        /// <returns>IGuild object</returns>
        public IGuild GetGuild(FactionFile.GuildGroups guildGroup, int buildingFactionId = 0)
        {
            IGuild guild;
            Memberships.TryGetValue(guildGroup, out guild);

            if (guildGroup == FactionFile.GuildGroups.HolyOrder && buildingFactionId > 0)
            {
                if (guild != null)
                {
                    Temple.Divines deity = Temple.GetDivine(buildingFactionId);
                    Temple temple = (Temple)guild;
                    if (temple.Deity == deity)
                        return guild;
                }
                return templeNotMember;
            }
            else if (guildGroup == FactionFile.GuildGroups.KnightlyOrder && buildingFactionId > 0)
            {
                if (guild != null)
                {
                    KnightlyOrder.Orders order = KnightlyOrder.GetOrder(buildingFactionId);
                    KnightlyOrder knightlyOrder = (KnightlyOrder)guild;
                    if (knightlyOrder.Order == order)
                        return guild;
                }
                return guildNotMember;
            }
            return (guild != null) ? guild : guildNotMember;
        }

        /// <summary>
        /// Retrieve the guild object for the given faction id.
        /// </summary>
        public IGuild GetGuild(int factionId)
        {
            try {
                FactionFile.GuildGroups guildGroup = GetGuildGroup(factionId);
                if (guildGroup == FactionFile.GuildGroups.None)
                    return guildNotMember;
                else
                    return GetGuild(guildGroup, factionId);
            // Catch erroneous faction data entries. (e.g. #91)
            } catch (ArgumentOutOfRangeException e) {
                DaggerfallUnity.LogMessage(e.Message, true);
                return guildNotMember;
            }
        }

        public FactionFile.GuildGroups GetGuildGroup(int factionId)
        {
            if (factionId == 510)   // Shops are marked as FG in faction data, hardcode to none to prevent them acting as FG guildhalls.
                return FactionFile.GuildGroups.None;

            PersistentFactionData persistentFactionData = GameManager.Instance.PlayerEntity.FactionData;
            FactionFile.GuildGroups guildGroup = FactionFile.GuildGroups.None;
            FactionFile.FactionData factionData;
            if (persistentFactionData.GetFactionData(factionId, out factionData))
            {
                guildGroup = (FactionFile.GuildGroups)factionData.ggroup;

                // Handle temples nested under deity
                if (factionData.type == (int)FactionFile.FactionTypes.God
                    && factionData.children != null && (guildGroup == FactionFile.GuildGroups.None && factionData.children.Count > 0))
                {
                    FactionFile.FactionData firstChild;
                    if (persistentFactionData.GetFactionData(factionData.children[0], out firstChild))
                    {
                        guildGroup = (FactionFile.GuildGroups)firstChild.ggroup;
                    }
                }
            }
            Debug.LogFormat("Got guild for faction id: {0}, social group: {1}, guild: {2}",
                factionId, (FactionFile.SocialGroups)factionData.sgroup, guildGroup);

            return guildGroup;
        }

        public void ClearMembershipData()
        {
            ClearMembershipData(false);
            ClearMembershipData(true);
        }

        private void ClearMembershipData(bool vampire = false)
        {
            if (vampire)
                vampMemberships.Clear();
            else
                memberships.Clear();
        }

        public Dictionary<int, GuildMembership_v1> GetMembershipData(bool vampire = false)
        {
            Dictionary<int, GuildMembership_v1> membershipData = new Dictionary<int, GuildMembership_v1>();
            Dictionary<FactionFile.GuildGroups, IGuild> memberDict = vampire ? vampMemberships : memberships;
            foreach (FactionFile.GuildGroups guildGroup in memberDict.Keys)
                membershipData[(int)guildGroup] = memberDict[guildGroup].GetGuildData();
            return membershipData;
        }

        public void RestoreMembershipData(Dictionary<int, GuildMembership_v1> data, bool vampire = false)
        {
            ClearMembershipData(vampire);
            if (data != null)
            {
                foreach (FactionFile.GuildGroups guildGroup in data.Keys)
                {
                    GuildMembership_v1 guildMembershipData = data[(int)guildGroup];
                    IGuild guild = CreateGuildObj(guildGroup, guildMembershipData.variant);
                    if (guild != null)
                    {
                        guild.RestoreGuildData(guildMembershipData);
                        if (vampire)
                            vampMemberships[guildGroup] = guild;
                        else
                            memberships[guildGroup] = guild;
                    }
                }
            }
        }

        /// <summary>
        /// Imports guild membership records from classic save data.
        /// </summary>
        public void ImportMembershipData(List<SaveTreeBaseRecord> guildMembershipRecords, bool vampire = false)
        {
            ClearMembershipData(vampire);
            foreach (GuildMembershipRecord record in guildMembershipRecords)
            {
                FactionFile.GuildGroups guildGroup = GetGuildGroup(record.ParsedData.factionID);
                IGuild guild = CreateGuildObj(guildGroup, record.ParsedData.factionID);
                if (vampire)
                    vampMemberships[guildGroup] = guild;
                else
                    memberships[guildGroup] = guild;

                if (vampire == GameManager.Instance.PlayerEffectManager.HasVampirism())
                    // Player should only join guilds from either his standard or vampire membership lists, not both
                    guild.Join();

                // Set rank and time from parsed data.
                guild.Rank = record.ParsedData.rank;
                guild.ImportLastRankChange(record.ParsedData.timeOfLastRankChange);
            }
        }

        #endregion

        #region Special guild benefits

        public bool FreeShipTravel()
        {
            foreach (IGuild guild in Memberships.Values)
                if (guild.FreeShipTravel())
                    return true;

            return false;
        }

        public int FastTravel(int duration)
        {
            int newDuration = duration;
            foreach (IGuild guild in Memberships.Values)
                newDuration = guild.FastTravel(newDuration);
            return newDuration;
        }

        public int DeepBreath(int duration)
        {
            int newDuration = duration;
            foreach (IGuild guild in Memberships.Values)
                newDuration = guild.DeepBreath(newDuration);
            return newDuration;
        }

        public bool AvoidDeath()
        {
            foreach (IGuild guild in Memberships.Values)
                if (guild.AvoidDeath())
                    return true;
            return false;
        }

        #endregion

    }

    #region Guild Console Commands

    public static class GuildConsoleCommands
    {
        public static void RegisterCommands()
        {
            try {
                ConsoleCommandsDatabase.RegisterCommand(GuildJoin.name, GuildJoin.description, GuildJoin.usage, GuildJoin.Execute);
                ConsoleCommandsDatabase.RegisterCommand(GuildRank.name, GuildRank.description, GuildRank.usage, GuildRank.Execute);
            }
            catch (Exception e) {
                DaggerfallUnity.LogMessage(string.Format("Error registering GuildManager console commands: {0}", e.Message), true);
            }
        }

        private static class GuildJoin
        {
            public static readonly string name = "guildjoin";
            public static readonly string description = "Join a guild";
            public static readonly string usage = "guildjoin guildGroupName [factionId]";

            public static string Execute(params string[] args)
            {
                if (args.Length == 0)
                {
                    return HelpCommand.Execute(name);
                }
                else
                {
                    GuildManager guildManager = GameManager.Instance.GuildManager;

                    // Is the group a guild group?
                    if (Enum.IsDefined(typeof(FactionFile.GuildGroups), args[0]))
                    {
                        FactionFile.GuildGroups guildGroup = (FactionFile.GuildGroups)Enum.Parse(typeof(FactionFile.GuildGroups), args[0]);

                        if (guildManager.HasJoined(guildGroup))
                        {
                            return "Already a member.";
                        }
                        else if (guildGroup == FactionFile.GuildGroups.HolyOrder || guildGroup == FactionFile.GuildGroups.KnightlyOrder)
                        {
                            if (args.Length > 1)
                            {
                                int factionId = int.Parse(args[1]);
                                IGuild guild = guildManager.JoinGuild(guildGroup, factionId);
                                guildManager.AddMembership(guildGroup, guild);
                                return "Guild joined.";
                            }
                            else
                            {
                                return "Need a faction id for temples & knightly orders.";
                            }
                        }
                        else
                        {
                            IGuild guild = guildManager.JoinGuild(guildGroup);
                            guildManager.AddMembership(guildGroup, guild);
                            return "Guild " + guildGroup.ToString() + " joined.";
                        }
                    }
                    else
                    {
                        return "Not a recognised guild group, see FactionFile.GuildGroups enum.";
                    }
                }
            }
        }

        private static class GuildRank
        {
            public static readonly string name = "guildrank";
            public static readonly string description = "Set rank in a guild that you are a member of";
            public static readonly string usage = "guildrank guildGroupName rank";

            public static string Execute(params string[] args)
            {
                if (args.Length != 2)
                {
                    return HelpCommand.Execute(name);
                }
                else
                {
                    GuildManager guildManager = GameManager.Instance.GuildManager;
                    PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;

                    // Is the group a guild group?
                    if (Enum.IsDefined(typeof(FactionFile.GuildGroups), args[0]))
                    {
                        FactionFile.GuildGroups guildGroup = (FactionFile.GuildGroups)Enum.Parse(typeof(FactionFile.GuildGroups), args[0]);

                        if (guildManager.HasJoined(guildGroup))
                        {
                            int newRank = int.Parse(args[1]);
                            if (newRank > 0 && newRank < 10)
                            {
                                IGuild guild = guildManager.GetGuild(guildGroup);
                                int rep = guild.GetReputation(playerEntity);
                                int newRep = Guild.rankReqReputation[newRank];
                                playerEntity.FactionData.ChangeReputation(guild.GetFactionId(), newRep - rep, true);
                                guild.Rank = newRank;
                                return "Rank & reputation updated.";
                            }
                            else
                            {
                                return "Rank must be between 1 and 9.";
                            }
                        }
                        else
                        {
                            return "Not a member of that guild.";
                        }
                    }
                    else
                    {
                        return "Not a recognised guild group, see FactionFile.GuildGroups enum.";
                    }
                }
            }
        }
    }

    #endregion
}
