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
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game.Utility;

namespace DaggerfallWorkshop.Game.Guilds
{
    public class DarkBrotherhood : Guild
    {
        #region Constants

        public const string InitiationQuestName = "L0A01L00";

        protected const int WelcomeMsgId = 5292;    // Not used AFAIK
        protected const int PromotionMsgId = 666;   // How appropriate!
        protected const int PromotionBuyPotionsId = 6611;
        protected const int PromotionMakePotionsId = 6612;
        protected const int PromotionSoulGemsId = 6613;
        protected const int PromotionSpymasterId = 6614;
        protected const int BribesJudgeId = 551;

        private const int factionId = (int)FactionFile.FactionIDs.The_Dark_Brotherhood;

        private DFLocation revealedDungeon;

        #endregion

        #region Properties & Data

        static string[] rankTitles = {
                "Apprentice", "Journeyman", "Operator", "Slayer", "Executioner", "Punisher", "Terminator", "Assassin", "Dark Brother", "Master Assassin"
        };

        static List<DFCareer.Skills> guildSkills = new List<DFCareer.Skills>() {
                DFCareer.Skills.Archery,
                DFCareer.Skills.Backstabbing,
                DFCareer.Skills.Climbing,
                DFCareer.Skills.CriticalStrike,
                DFCareer.Skills.Daedric,
                DFCareer.Skills.Destruction,
                DFCareer.Skills.ShortBlade,
                DFCareer.Skills.Stealth,
                DFCareer.Skills.Streetwise,
            };

        static List<DFCareer.Skills> trainingSkills = new List<DFCareer.Skills>() {
                DFCareer.Skills.Archery,
                DFCareer.Skills.Backstabbing,
                DFCareer.Skills.Climbing,
                DFCareer.Skills.CriticalStrike,
                DFCareer.Skills.Daedric,
                DFCareer.Skills.Destruction,
                DFCareer.Skills.Dodging,
                DFCareer.Skills.Running,
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

        public static int FactionId { get { return factionId; } }

        public override int GetFactionId()
        {
            return factionId;
        }

        public override string GetTitle()
        {
            if (GameManager.Instance.PlayerEntity.Gender == Genders.Female && rank == 8)
                return "Dark Sister";        // Not calling female chars 'Brother'!

            return IsMember() ? rankTitles[rank] : "non-member";
        }

        #endregion

        #region Guild Ranks

        public override TextFile.Token[] TokensPromotion(int newRank)
        {
            return DaggerfallUnity.Instance.TextProvider.GetRandomTokens(GetPromotionMsgId(newRank));
        }

        private int GetPromotionMsgId(int newRank)
        {
            revealedDungeon = GameManager.Instance.PlayerGPS.DiscoverRandomLocation();
            if (!string.IsNullOrEmpty(revealedDungeon.Name))
            {
                GameManager.Instance.PlayerEntity.Notebook.AddNote(
                   TextManager.Instance.GetText("DaggerfallUI", "readMapDB").Replace("%map", revealedDungeon.Name));
            }
            switch (newRank)
            {
                case 1:
                    return PromotionBuyPotionsId;
                case 3:
                    return PromotionMakePotionsId;
                case 5:
                    return PromotionSoulGemsId;
                case 7:
                    return PromotionSpymasterId;
                default:
                    return PromotionMsgId;
            }
        }

        protected override int CalculateNewRank(PlayerEntity playerEntity)
        {
            int newRank = base.CalculateNewRank(playerEntity);
            return AllowGuildExpulsion(playerEntity, newRank);
        }

        protected virtual int AllowGuildExpulsion(PlayerEntity playerEntity, int newRank)
        {
            // Dark Brotherhood never expel members (I assume at some point they 'retire' you instead!)
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
                case GuildServices.BuyPotions:
                    return (rank >= 1);
                case GuildServices.MakePotions:
                    return (rank >= 3);
                case GuildServices.BuySoulgems:
                    return (rank >= 5);
                case GuildServices.Spymaster:
                    return (rank >= 7);

                default:
                    return false;
            }
        }

        #endregion

        #region Joining

        override public void Join()
        {
            base.Join();
            RegisterEvents();
            RevealGuildHallOnMap();
        }

        override public void Leave()
        {
            UnregisterEvents();
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

        private void RegisterEvents()
        {
            // Register events for location entry events so can auto discover guild houses.
            PlayerGPS.OnEnterLocationRect += PlayerGPS_OnEnterLocationRect;
            StreamingWorld.OnAvailableLocationGameObject += StreamingWorld_OnAvailableLocationGameObject;

            // Register events so as to know when to unregister events.
            SaveLoadManager.OnStartLoad += SaveLoadManager_OnStartLoad;
            StartGameBehaviour.OnNewGame += StartGameBehaviour_OnNewGame;
        }

        private void UnregisterEvents()
        {
            // Unregister events
            PlayerGPS.OnEnterLocationRect -= PlayerGPS_OnEnterLocationRect;
            StreamingWorld.OnAvailableLocationGameObject -= StreamingWorld_OnAvailableLocationGameObject;
            SaveLoadManager.OnStartLoad -= SaveLoadManager_OnStartLoad;
            StartGameBehaviour.OnNewGame -= StartGameBehaviour_OnNewGame;
        }

        private void StartGameBehaviour_OnNewGame()
        {
            UnregisterEvents();
        }

        private void SaveLoadManager_OnStartLoad(SaveData_v1 saveData)
        {
            UnregisterEvents();
        }

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

        #region Serialization

        public override void RestoreGuildData(GuildMembership_v1 data)
        {
            base.RestoreGuildData(data);
            RegisterEvents();
        }

        #endregion

        #region Macro Handling

        public override MacroDataSource GetMacroDataSource()
        {
            return new DarkBrotherhoodMacroDataSource(this);
        }

        /// <summary>
        /// MacroDataSource context sensitive methods for Dark Brotherhood.
        /// </summary>
        protected class DarkBrotherhoodMacroDataSource : GuildMacroDataSource
        {
            private readonly DarkBrotherhood parent;
            public DarkBrotherhoodMacroDataSource(DarkBrotherhood guild) : base(guild)
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
