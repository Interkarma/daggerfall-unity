// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2019 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut
// Contributors:    

using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.Entity;
using System;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop.Game.Guilds
{
    public class ThievesGuild : Guild
    {
        #region Constants

        public const string InitiationQuestName = "O0A0AL00";

        protected const int WelcomeMsgId = 5225;    // Not used AFAIK
        protected const int PromotionMsgId = 5235;
        protected const int PromotionFenceId = 5226;
        protected const int PromotionSpymasterId = 5227;
        protected const int PromotionMap1Id = 5228;
        protected const int PromotionMap2Id = 5229;
        protected const int BribesJudgeId = 550;

        private const int factionId = (int)FactionFile.FactionIDs.The_Thieves_Guild;

        private DFLocation revealedDungeon;

        #endregion

        #region Properties & Data

        static string[] rankTitles = {
                "Apprentice", "Journeyman", "Filcher", "Crook", "Robber", "Bandit", "Thief", "Ringleader", "Mastermind", "Master Thief"
        };

        static List<DFCareer.Skills> guildSkills = new List<DFCareer.Skills>() {
                DFCareer.Skills.Backstabbing,
                DFCareer.Skills.Climbing,
                DFCareer.Skills.Lockpicking,
                DFCareer.Skills.Pickpocket,
                DFCareer.Skills.ShortBlade,
                DFCareer.Skills.Stealth,
                DFCareer.Skills.Streetwise
            };

        static List<DFCareer.Skills> trainingSkills = new List<DFCareer.Skills>() {
                DFCareer.Skills.Backstabbing,
                DFCareer.Skills.BluntWeapon,
                DFCareer.Skills.Climbing,
                DFCareer.Skills.Dodging,
                DFCareer.Skills.Jumping,
                DFCareer.Skills.Lockpicking,
                DFCareer.Skills.Pickpocket,
                DFCareer.Skills.ShortBlade,
                DFCareer.Skills.Stealth,
                DFCareer.Skills.Streetwise,
                DFCareer.Skills.Swimming
            };

        public override string[] RankTitles { get { return rankTitles; } }

        public override List<DFCareer.Skills> GuildSkills { get { return guildSkills; } }

        public override List<DFCareer.Skills> TrainingSkills { get { return trainingSkills; } }

        #endregion

        #region Guild Membership and Faction

        public ThievesGuild()
        {
            // Register for location entry events so can auto discover guild houses.
            PlayerGPS.OnEnterLocationRect += PlayerGPS_OnEnterLocationRect;
            StreamingWorld.OnAvailableLocationGameObject += StreamingWorld_OnAvailableLocationGameObject;
        }

        ~ThievesGuild()
        {
            // Unregister events
            PlayerGPS.OnEnterLocationRect -= PlayerGPS_OnEnterLocationRect;
            StreamingWorld.OnAvailableLocationGameObject -= StreamingWorld_OnAvailableLocationGameObject;
        }

        public static int FactionId { get { return factionId; } }

        public override int GetFactionId()
        {
            return factionId;
        }

        #endregion

        #region Guild Ranks

        public override TextFile.Token[] TokensPromotion(int newRank)
        {
            return DaggerfallUnity.Instance.TextProvider.GetRandomTokens(GetPromotionMsgId(newRank));
        }

        private int GetPromotionMsgId(int newRank)
        {
            switch (newRank)
            {
                case 2:
                    return PromotionFenceId;
                case 4:
                    return PromotionSpymasterId;
                case 6:
                    return RevealLocation() ? PromotionMap1Id : PromotionMsgId;
                case 8:
                    return RevealLocation() ? PromotionMap2Id : PromotionMsgId;
                default:
                    return PromotionMsgId;
            }
        }

        private bool RevealLocation()
        {
            revealedDungeon = GameManager.Instance.PlayerGPS.DiscoverRandomLocation();
            if (!string.IsNullOrEmpty(revealedDungeon.Name))
            {
                GameManager.Instance.PlayerEntity.Notebook.AddNote(
                    TextManager.Instance.GetText("DaggerfallUI", "readMapTG").Replace("%map", revealedDungeon.Name));
                return true;
            }
            return false;
        }

        protected override int CalculateNewRank(PlayerEntity playerEntity)
        {
            // Thieves guild never expel members (I assume at some point they 'retire' you instead!)
            int newRank = base.CalculateNewRank(playerEntity);
            return (newRank < 0) ? 0 : newRank;
        }

        #endregion

        #region Benefits

        public override bool HallAccessAnytime()
        {
            return IsMember();
        }

        #endregion

        #region Service Access:

        public override bool CanAccessService(GuildServices service)
        {
            switch (service)
            {
                case GuildServices.Training:
                    return IsMember();
                case GuildServices.Quests:
                    return true;
                case GuildServices.SellMagicItems:
                    return (rank >= 2);
                case GuildServices.Spymaster:
                    return (rank >= 4);

                default:
                    return false;
            }
        }

        #endregion

        #region Joining

        override public void Join()
        {
            base.Join();
            RevealGuildHallOnMap();
        }

        public override TextFile.Token[] TokensIneligible(PlayerEntity playerEntity)
        {
            throw new NotImplementedException();
        }
        public override TextFile.Token[] TokensEligible(PlayerEntity playerEntity)
        {
            throw new NotImplementedException();
        }
        public override TextFile.Token[] TokensWelcome()
        {
            return DaggerfallUnity.Instance.TextProvider.GetRSCTokens(WelcomeMsgId);
        }

        #endregion

        #region Event handlers

        private void PlayerGPS_OnEnterLocationRect(DFLocation location)
        {
            RevealGuildHallOnMap();
        }

        private void StreamingWorld_OnAvailableLocationGameObject()
        {
            RevealGuildHallOnMap();
        }

        #endregion

        #region Private Functions

        private void RevealGuildHallOnMap()
        {
            BuildingDirectory buildingDirectory = GameManager.Instance.StreamingWorld.GetCurrentBuildingDirectory();
            if (buildingDirectory)
                foreach (BuildingSummary building in buildingDirectory.GetBuildingsOfFaction(factionId))
                    GameManager.Instance.PlayerGPS.DiscoverBuilding(building.buildingKey, GetGuildName());
        }

        #endregion

        #region Macro Handling

        public override MacroDataSource GetMacroDataSource()
        {
            return new ThievesGuildMacroDataSource(this);
        }

        /// <summary>
        /// MacroDataSource context sensitive methods for Thieves Guild.
        /// </summary>
        protected class ThievesGuildMacroDataSource : GuildMacroDataSource
        {
            private readonly ThievesGuild parent;
            public ThievesGuildMacroDataSource(ThievesGuild guild) : base(guild)
            {
                parent = guild;
            }

            public override string Dungeon()
            {
                return parent.revealedDungeon.Name;
            }
        }

        #endregion
    }
}
