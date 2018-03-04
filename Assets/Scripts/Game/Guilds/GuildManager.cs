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

        #region Guild membership handling

        private Dictionary<FactionFile.GuildGroups, Guild> memberships = new Dictionary<FactionFile.GuildGroups, Guild>();

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

        public bool HasJoined(FactionFile.GuildGroups guildGroup)
        {
            return memberships.ContainsKey(guildGroup);
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

/*                case FactionFile.GuildGroups.KnightlyOrder:
                    Temple.Divines deity = (Temple.Divines) buildingFactionId;
                    return new KnightlyOrder(region);
*/
                default:
                    Type guildType;
                    if (customGuilds.TryGetValue(guildGroup, out guildType))
                        return (Guild) Activator.CreateInstance(guildType);
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
                    Temple temple = (Temple) guild;
                    if (temple.Deity == deity)
                        return guild;
                }
                return templeNotMember;
            }
            return (guild != null) ? guild : guildNotMember;
        }

        /// <summary>
        /// Retrieve the guild object for the given faction id.
        /// </summary>
        public Guild GetGuild(int factionId)
        {
            FactionFile.GuildGroups guildGroup = GetGuildGroup(factionId);
            if (guildGroup == FactionFile.GuildGroups.None)
                throw new Exception("Can't find guild for faction id: " + factionId);

            return GetGuild(guildGroup, factionId);
        }

        private FactionFile.GuildGroups GetGuildGroup(int factionId)
        {
            PersistentFactionData persistentFactionData = GameManager.Instance.PlayerEntity.FactionData;
            FactionFile.GuildGroups guildGroup = FactionFile.GuildGroups.None;
            FactionFile.FactionData factionData;
            if (persistentFactionData.GetFactionData(factionId, out factionData))
            {
                guildGroup = (FactionFile.GuildGroups) factionData.ggroup;

                // Handle temples... TODO is this needed since they are always open?
                if (guildGroup == FactionFile.GuildGroups.None && factionData.children.Count > 0)
                {
                    FactionFile.FactionData firstChild;
                    if (persistentFactionData.GetFactionData(factionData.children[0], out firstChild))
                    {
                        guildGroup = (FactionFile.GuildGroups) firstChild.ggroup;
                    }
                }
            }

            // Thieves guild is general populace / underworld
            if (guildGroup == FactionFile.GuildGroups.GeneralPopulace && (FactionFile.SocialGroups) factionData.sgroup == FactionFile.SocialGroups.Underworld)
            {
                Debug.Log("Theves Guild.");
            }

            Debug.LogFormat("faction id: {0}, social group: {1}, guild: {2}",
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
            memberships.Clear();
            if (data != null)
            {
                foreach (FactionFile.GuildGroups guildGroup in data.Keys)
                {
                    GuildMembership_v1 guildMembershipData = data[(int)guildGroup];
                    Guild guild = CreateGuildObj(guildGroup, guildMembershipData.variant);
                    guild.RestoreGuildData(guildMembershipData);
                    memberships[guildGroup] = guild;
                }
            }
        }

        #endregion

        #region Special guild benefits

        public virtual int FastTravel(int duration)
        {
            int newDuration = duration;
            foreach (Guild guild in memberships.Values)
                newDuration = guild.FastTravel(newDuration);
            return newDuration;
        }

        public virtual int DeepBreath(int duration)
        {
            int newDuration = duration;
            foreach (Guild guild in memberships.Values)
                newDuration = guild.DeepBreath(newDuration);
            return newDuration;
        }

        public virtual bool AvoidDeath()
        {
            foreach (Guild guild in memberships.Values)
                if (guild.AvoidDeath())
                    return true;
            return false;
        }

        #endregion

    }
}