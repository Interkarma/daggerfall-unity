// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut
// Contributors:    

using UnityEngine;
using System.Collections.Generic;
using DaggerfallConnect;
using System;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.Player;

namespace DaggerfallWorkshop.Game.Guilds
{
    public class GuildManager
    {
        // A base guild class defining non-membership.
        public static Guild guildNotMember = new NonMemberGuild();

        private Dictionary<FactionFile.GuildGroups, Guild> memberships = new Dictionary<FactionFile.GuildGroups, Guild>();

        public bool IsMember(FactionFile.GuildGroups guildGroup)
        {
            return memberships.ContainsKey(guildGroup);
        }

        public Guild JoinGuild(FactionFile.GuildGroups guildGroup)
        {
            if (memberships.ContainsKey(guildGroup))
                return memberships[guildGroup];

            switch (guildGroup)
            {
                case FactionFile.GuildGroups.FightersGuild:
                    return new FightersGuild();
            }
            return null;
        }

        public void AddMembership(FactionFile.GuildGroups guildGroup, Guild guild)
        {
            guild.Join();
            memberships[guildGroup] = guild;
        }

        /// <summary>
        /// Retrieve the guild object for the given guild group.
        /// </summary>
        public Guild GetGuild(FactionFile.GuildGroups guildGroup)
        {
            Guild guild;
            memberships.TryGetValue(guildGroup, out guild);

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

            return GetGuild(guildGroup);
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
    }
}