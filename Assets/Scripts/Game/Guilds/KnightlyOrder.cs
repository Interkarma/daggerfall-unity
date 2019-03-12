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
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Game.Banking;

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
        protected const int PromotionNoHouseId = 5241;
        protected const int ArmorId = 463;
        protected const int HouseId = 462;
        protected const int NoArmorId = 461;
        protected const int NoHouseId = 460;

        private const int ArmorFlagMask = 1;
        private const int HouseFlagMask = 2;

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

        static string[] rankTitles = {
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

        int flags = 0;

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

        public override TextFile.Token[] UpdateRank(PlayerEntity playerEntity)
        {
            TextFile.Token[] tokens = base.UpdateRank(playerEntity);
            if (tokens != null)
                flags = 0;

            return tokens;
        }

        public override int GetFactionId()
        {
            return (int)order;
        }

        public override string GetTitle()
        {
            if (GameManager.Instance.PlayerEntity.Gender == Genders.Female && rank == 5)
                return "Knight Sister";        // Not calling female chars 'Brother'!

            return IsMember() ? rankTitles[rank] : "non-member";
        }

        #endregion

        #region Guild Ranks

        public override TextFile.Token[] TokensPromotion(int newRank)
        {
            return DaggerfallUnity.Instance.TextProvider.GetRSCTokens(GetPromotionMsgId(newRank));
        }

        private int GetPromotionMsgId(int newRank)
        {
            if (newRank == 4)
                return PromotionFreeRoomsId;
            if (newRank == 6)
                return PromotionFreeShipsId;
            if (newRank == 9)
                return DaggerfallBankManager.OwnsHouse ? PromotionNoHouseId : PromotionHouseId;

            return PromotionMsgId;
        }

        #endregion

        #region Benefits

        public override bool FreeTavernRooms()
        {
            if (rank >= 4)
                return true;

            FactionFile.FactionData factionData;
            if (DaggerfallUnity.Instance.ContentReader.FactionFileReader.GetFactionData(GetFactionId(), out factionData)
                && GameManager.Instance.PlayerGPS.CurrentLocation.RegionIndex == factionData.region)
                return true;

            return false;
        }

        public override bool FreeShipTravel()
        {
            return (rank >= 6);
        }

        #endregion

        #region Service Access:

        public override bool CanAccessService(GuildServices service)
        {
            switch (service)
            {
                case GuildServices.Quests:
                    return true;
                case GuildServices.ReceiveArmor:
                    return IsMember();
                case GuildServices.ReceiveHouse:
                    return true;
                default:
                    return false;
            }
        }

        #endregion

        #region Services

        public void ReceiveArmor(PlayerEntity playerEntity)
        {
            if ((flags & ArmorFlagMask) > 0)
            {
                DaggerfallUI.MessageBox(NoArmorId);
            }
            else
            {   // Give a random armor piece, allowing player to choose one out of 3-6
                ItemCollection rewardArmor = new ItemCollection();
                ArmorMaterialTypes material = ArmorMaterialTypes.Iron + rank;
                for (int i = UnityEngine.Random.Range(3, 7); i >= 0; i--)
                {
                    Armor armor = (Armor)UnityEngine.Random.Range(102, 108 + 1);
                    rewardArmor.AddItem(ItemBuilder.CreateArmor(playerEntity.Gender, playerEntity.Race, armor, material));
                }
                flags = flags | ArmorFlagMask;
                DaggerfallUI.MessageBox(ArmorId);
                DaggerfallUI.Instance.InventoryWindow.SetChooseOne(rewardArmor);
                DaggerfallUI.PostMessage(DaggerfallUIMessages.dfuiOpenInventoryWindow);
            }
        }

        public void ReceiveHouse()
        {
            if (rank < 9)
            {
                DaggerfallUI.MessageBox(NoHouseId);
            }
            else if ((flags & HouseFlagMask) > 0)
            {
                DaggerfallUI.MessageBox(HardStrings.serviceReceiveHouseAlready);
            }
            else
            {   // Give a house if one availiable
                if (DaggerfallBankManager.OwnsHouse)
                    DaggerfallUI.MessageBox((int)TransactionResult.ALREADY_OWN_HOUSE);
                else
                {
                    BuildingDirectory buildingDirectory = GameManager.Instance.StreamingWorld.GetCurrentBuildingDirectory();
                    if (buildingDirectory)
                    {
                        List<BuildingSummary> housesForSale = buildingDirectory.GetHousesForSale();
                        if (housesForSale.Count > 0)
                        {
                            BuildingSummary house = housesForSale[UnityEngine.Random.Range(0, housesForSale.Count)];
                            DaggerfallBankManager.AllocateHouseToPlayer(house, GameManager.Instance.PlayerGPS.CurrentRegionIndex);
                            flags = flags | HouseFlagMask;
                            DaggerfallUI.MessageBox(HouseId);
                            return;
                        }
                    }
                    DaggerfallUI.MessageBox((int)TransactionResult.NO_HOUSES_FOR_SALE);
                }
            }
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
            return new GuildMembership_v1() { rank = rank, lastRankChange = lastRankChange, variant = (int)order, flags = flags };
        }

        internal override void RestoreGuildData(GuildMembership_v1 data)
        {
            base.RestoreGuildData(data);
            order = (Orders)data.variant;
            flags = data.flags;
        }

        #endregion


        #region Macro Handling

        public override MacroDataSource GetMacroDataSource()
        {
            return new OrderMacroDataSource(this);
        }

        /// <summary>
        /// MacroDataSource context sensitive methods for knightly orders.
        /// </summary>
        protected class OrderMacroDataSource : GuildMacroDataSource
        {
            private readonly KnightlyOrder parent;
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
