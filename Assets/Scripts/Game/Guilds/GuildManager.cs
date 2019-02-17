// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
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

        // Custom guild registry.
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
            if (quest.QuestName == ThievesGuild.InitiationQuestName)
            {
                Guild tg = CreateGuildObj(FactionFile.GuildGroups.GeneralPopulace);
                AddMembership(FactionFile.GuildGroups.GeneralPopulace, tg);
            }
            if (quest.QuestName == DarkBrotherhood.InitiationQuestName)
            {
                Guild db = CreateGuildObj(FactionFile.GuildGroups.DarkBrotherHood);
                AddMembership(FactionFile.GuildGroups.DarkBrotherHood, db);
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

        private readonly Dictionary<FactionFile.GuildGroups, Guild> memberships = new Dictionary<FactionFile.GuildGroups, Guild>();

        public List<Guild> GetMemberships()
        {
            List<Guild> guilds = new List<Guild>();
            foreach (Guild guild in memberships.Values)
                guilds.Add(guild);
            return guilds;
        }

        public void AddMembership(FactionFile.GuildGroups guildGroup, Guild guild)
        {
            guild.Join();
            memberships[guildGroup] = guild;
        }

        public void RemoveMembership(Guild guild)
        {
            FactionFile.GuildGroups guildGroup = FactionFile.GuildGroups.None;
            foreach (FactionFile.GuildGroups group in memberships.Keys)
            {
                if (memberships[group] == guild)
                {
                    guildGroup = group;
                    break;
                }
            }
            if (guildGroup != FactionFile.GuildGroups.None)
                memberships.Remove(guildGroup);
        }

        public bool HasJoined(FactionFile.GuildGroups guildGroup)
        {
            return memberships.ContainsKey(guildGroup);
        }

        public bool GetJoinedGuildOfGuildGroup(FactionFile.GuildGroups guildGroup, out Guild value)
        {
            if (memberships.TryGetValue(guildGroup, out value))
                return true;

            return false;
        }

        public Guild JoinGuild(FactionFile.GuildGroups guildGroup, int buildingFactionId = 0)
        {
            if (memberships.ContainsKey(guildGroup))
                return memberships[guildGroup];

            return CreateGuildObj(guildGroup, buildingFactionId);
        }

        private Guild CreateGuildObj(FactionFile.GuildGroups guildGroup, int variant = 0)
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
                    Type guildType;
                    if (customGuilds.TryGetValue(guildGroup, out guildType))
                        return (Guild)Activator.CreateInstance(guildType);
                    else
                        return null;
            }
        }

        /// <summary>
        /// Retrieve the guild object for the given guild group.
        /// </summary>
        /// <param name="guildGroup"></param>
        /// <param name="buildingFactionId">Specify this to ensure only the temple of matching Divine is returned</param>
        /// <returns>Guild object</returns>
        public Guild GetGuild(FactionFile.GuildGroups guildGroup, int buildingFactionId = 0)
        {
            Guild guild;
            memberships.TryGetValue(guildGroup, out guild);

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
        public Guild GetGuild(int factionId)
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

        private FactionFile.GuildGroups GetGuildGroup(int factionId)
        {
            PersistentFactionData persistentFactionData = GameManager.Instance.PlayerEntity.FactionData;
            FactionFile.GuildGroups guildGroup = FactionFile.GuildGroups.None;
            FactionFile.FactionData factionData;
            if (persistentFactionData.GetFactionData(factionId, out factionData))
            {
                guildGroup = (FactionFile.GuildGroups)factionData.ggroup;

                // Handle temples nested under deity
                if (factionData.children != null && (guildGroup == FactionFile.GuildGroups.None && factionData.children.Count > 0))
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
            memberships.Clear();
        }

        public Dictionary<int, GuildMembership_v1> GetMembershipData()
        {
            Dictionary<int, GuildMembership_v1> membershipData = new Dictionary<int, GuildMembership_v1>();
            foreach (FactionFile.GuildGroups guildGroup in memberships.Keys)
                membershipData[(int)guildGroup] = memberships[guildGroup].GetGuildData();
            return membershipData;
        }

        public void RestoreMembershipData(Dictionary<int, GuildMembership_v1> data)
        {
            ClearMembershipData();
            if (data != null)
            {
                foreach (FactionFile.GuildGroups guildGroup in data.Keys)
                {
                    GuildMembership_v1 guildMembershipData = data[(int)guildGroup];
                    Guild guild = CreateGuildObj(guildGroup, guildMembershipData.variant);
                    if (guild != null)
                    {
                        guild.RestoreGuildData(guildMembershipData);
                        memberships[guildGroup] = guild;
                    }
                }
            }
        }

        /// <summary>
        /// Imports guild membership records from classic save data.
        /// </summary>
        public void ImportMembershipData(List<SaveTreeBaseRecord> guildMembershipRecords)
        {
            ClearMembershipData();
            foreach (GuildMembershipRecord record in guildMembershipRecords)
            {
                FactionFile.GuildGroups guildGroup = GetGuildGroup(record.ParsedData.factionID);
                Guild guild = CreateGuildObj(guildGroup, record.ParsedData.factionID);
                AddMembership(guildGroup, guild);

                // Set rank and time from parsed data.
                guild.Rank = record.ParsedData.rank;
                guild.ImportLastRankChange(record.ParsedData.timeOfLastRankChange);
            }
        }

        #endregion

        #region Special guild benefits

        public bool FreeShipTravel()
        {
            foreach (Guild guild in memberships.Values)
                if (guild.FreeShipTravel())
                    return true;

            return false;
        }

        public int FastTravel(int duration)
        {
            int newDuration = duration;
            foreach (Guild guild in memberships.Values)
                newDuration = guild.FastTravel(newDuration);
            return newDuration;
        }

        public int DeepBreath(int duration)
        {
            int newDuration = duration;
            foreach (Guild guild in memberships.Values)
                newDuration = guild.DeepBreath(newDuration);
            return newDuration;
        }

        public bool AvoidDeath()
        {
            foreach (Guild guild in memberships.Values)
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
                                Guild guild = guildManager.JoinGuild(guildGroup, factionId);
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
                            Guild guild = guildManager.JoinGuild(guildGroup);
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
                                Guild guild = guildManager.GetGuild(guildGroup);
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
