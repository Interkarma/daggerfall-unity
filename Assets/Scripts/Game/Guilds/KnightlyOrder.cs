// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
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
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop.Game.Guilds
{
    public class KnightlyOrder : Guild, IMacroContextProvider
    {
        #region Constants

        protected const int IneligibleBadRepId = 751;
        protected const int IneligibleLowSkillId = 750;
        protected const int EligibleMsgId = 752;
        protected const int WelcomeMsgId = 5291;
        protected const int PromotionMsgId = 5237;
        protected const int PromotionFreeRoomsId = 5238;
        protected const int PromotionFreeShipsId = 5239;
        protected const int PromotionHouseId = 5240;

        #endregion

        #region Enums and Structs

        public enum Orders
        {
            Horn    = 411,
            Dragon  = 368,
            Flame   = 410,
            Hawk    = 417,
            Owl     = 413,
            Rose    = 409,
            Wheel   = 415,
            Candle  = 408,
            Raven   = 414,
            Scarab  = 416
        }

        #endregion

        #region Properties & Data

        static string[] rankTitles = new string[] {
                "Aspirant", "Squire", "Gallant", "Chevalier", "Keeper", "Knight Brother", "Commander", "Marshall", "Seneschal", "Paladin"
        };

        static List<DFCareer.Skills> guildSkills = new List<DFCareer.Skills>() {
                DFCareer.Skills.Archery,
                DFCareer.Skills.CriticalStrike,
                DFCareer.Skills.Dragonish,
                DFCareer.Skills.Etiquette,
                DFCareer.Skills.Giantish,
                DFCareer.Skills.LongBlade,
                DFCareer.Skills.Medical,
            };

        public override string[] RankTitles { get { return rankTitles; } }

        public override List<DFCareer.Skills> GuildSkills { get { return guildSkills; } }

        public override List<DFCareer.Skills> TrainingSkills { get { return null; } }

        #endregion

        #region Knightly Order

        Orders order;

        public Orders Order { get { return order; } }

        public KnightlyOrder(Orders order) : base()
        {
            this.order = order;
        }

        public static Orders GetOrder(int factionId)
        {
            // Temple hall:
            if (Enum.IsDefined(typeof(Orders), factionId))
                return (Orders)factionId;

            throw new ArgumentOutOfRangeException("There is no Order that matches the factionId: " + factionId);
        }

        #endregion

        #region Guild Membership and Faction

        public override int GetFactionId()
        {
            return (int) order;
        }

        public override string GetTitle()
        {
            if (GameManager.Instance.PlayerEntity.Gender == Genders.Female)
                if (rank == 5)
                    return "Knight Sister";        // Not calling female chars 'Brother'!

            return IsMember() ? rankTitles[rank] : "non-member";
        }

        #endregion


        #region Guild Ranks

        public override TextFile.Token[] TokensPromotion(int newRank)
        {
            return DaggerfallUnity.Instance.TextProvider.GetRSCTokens(GetPromotionMsgId(newRank));
        }

        private int GetPromotionMsgId(int rank)
        {
            if (rank == 4)
                return PromotionFreeRoomsId;
            if (rank == 6)
                return PromotionFreeShipsId;
            if (rank == 9)
                return PromotionHouseId;

            return PromotionMsgId;
        }

        #endregion

        #region Benefits

        // TESTING ONLY - REMOVE!
        public override bool CanRest()
        {
            return IsMember();
        }

        public override bool FreeTavernRooms()
        {
            if (rank >= 4)
                return true;

            FactionFile.FactionData factionData;
            if (DaggerfallUnity.Instance.ContentReader.FactionFileReader.GetFactionData(GetFactionId(), out factionData))
                if (GameManager.Instance.PlayerGPS.CurrentLocation.RegionIndex + 1 == factionData.region)
                    return true;

            return false;
        }

        #endregion


        #region Joining

        public override TextFile.Token[] TokensIneligible(PlayerEntity playerEntity)
        {
            int msgId = (GetReputation(playerEntity) < 0) ? IneligibleBadRepId : IneligibleLowSkillId;
            return DaggerfallUnity.Instance.TextProvider.GetRandomTokens(msgId);
        }
        public override TextFile.Token[] TokensEligible(PlayerEntity playerEntity)
        {
            return DaggerfallUnity.Instance.TextProvider.GetRSCTokens(EligibleMsgId);
        }
        public override TextFile.Token[] TokensWelcome()
        {
            return DaggerfallUnity.Instance.TextProvider.GetRSCTokens(WelcomeMsgId);
        }

        #endregion

        #region Serialization

        internal override GuildMembership_v1 GetGuildData()
        {
            return new GuildMembership_v1() { rank = rank, lastRankChange = lastRankChange, variant = (int) order };
        }

        internal override void RestoreGuildData(GuildMembership_v1 data)
        {
            base.RestoreGuildData(data);
            order = (Orders) data.variant;
        }

        #endregion


        #region Macro Handling

        public override MacroDataSource GetMacroDataSource()
        {
            return new OrderMacroDataSource(this);
        }

        /// <summary>
        /// MacroDataSource context sensitive methods for temples.
        /// </summary>
        protected class OrderMacroDataSource : GuildMacroDataSource
        {
            private KnightlyOrder parent;
            public OrderMacroDataSource(KnightlyOrder guild) : base(guild)
            {
                parent = guild;
            }

            public override string FactionOrderName()
            {
                return parent.GetGuildName();
            }
        }

        #endregion
    }
}