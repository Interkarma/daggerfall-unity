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
using DaggerfallWorkshop.Utility;
using System;
using DaggerfallWorkshop.Game.Player;
using UnityEngine;
using DaggerfallWorkshop.Game.Serialization;

namespace DaggerfallWorkshop.Game.Guilds
{
    public class Temple : Guild
    {
        #region Constants

        protected const int IneligibleBadRepId = 745;
        protected const int IneligibleLowSkillId = 744;
        protected const int EligibleMsgId = 740;

        protected const int PromotionBuyPotionsId = 6600;
        protected const int PromotionLibraryId = 6601;
        protected const int PromotionMakePotionsId = 6602;
        protected const int PromotionSoulGemsId = 6603;
        protected const int PromotionSummoningId = 6604;
        protected const int PromotionHealingId = 6605;
        protected const int PromotionBuySpellsId = 6606;
        protected const int PromotionMakeSpellsId = 6607;
        protected const int PromotionBuyMagicId = 6608;
        protected const int PromotionMakeMagicId = 6609;
        protected const int PromotionHighestId = 5241;

        #endregion

        #region Enums and Structs

        public enum Divines
        {   // value = factionId
            Akatosh = 26,
            Arkay = 21,
            Dibella = 29,
            Julianos = 27,
            Kynareth = 35,
            Mara = 24,
            Stendarr = 33,
            Zenithar = 22
        }

        struct RankBenefits
        {
            public readonly int library;
            public readonly int healing;
            public readonly int buyPotions;
            public readonly int makePotions;
            public readonly int buyMagic;
            public readonly int makeMagic;
            public readonly int buySpells;
            public readonly int makeSpells;
            public readonly int soulGems;
            public readonly int summoning;
            public readonly int welcomeMsgId;
            public readonly int promotionMsgId;
            public readonly int templeNameMsgId;

            public RankBenefits(int library, int healing, int buyPotions, int makePotions, int buyMagic, int makeItems, int buySpells, int makeSpells, int soulGems, int summoning,
                                int welcomeMsgId, int promotionMsgId, int templeNameMsgId)
            {
                this.library = library;
                this.healing = healing;
                this.buyPotions = buyPotions;
                this.makePotions = makePotions;
                this.buyMagic = buyMagic;
                this.makeMagic = makeItems;
                this.buySpells = buySpells;
                this.makeSpells = makeSpells;
                this.soulGems = soulGems;
                this.summoning = summoning;
                this.welcomeMsgId = welcomeMsgId;
                this.promotionMsgId = promotionMsgId;
                this.templeNameMsgId = templeNameMsgId;
            }

            public int GetWelcomeMsgId()
            {
                return welcomeMsgId;
            }

            public int GetTempleNameMsgId()
            {
                return templeNameMsgId;
            }

            public int GetPromotionMsgId(int rank)
            {
                if (rank == 9)
                    return PromotionHighestId;
                if (library == rank)
                    return PromotionBuyPotionsId;
                if (healing == rank)
                    return PromotionHealingId;
                if (buyPotions == rank)
                    return PromotionBuyPotionsId;
                if (makePotions == rank)
                    return PromotionMakePotionsId;
                if (buyMagic == rank)
                    return PromotionBuyMagicId;
                if (makeMagic == rank)
                    return PromotionMakeMagicId;
                if (buySpells == rank)
                    return PromotionBuySpellsId;
                if (makeSpells == rank)
                    return PromotionMakeSpellsId;
                if (soulGems == rank)
                    return PromotionSoulGemsId;
                if (summoning == rank)
                    return PromotionSummoningId;

                return promotionMsgId;
            }
        }

        #endregion

        #region Static Data

        static Dictionary<Divines, RankBenefits> templeRankBenefits = new Dictionary<Divines, RankBenefits>()
        {
            { Divines.Akatosh,  new RankBenefits(2, 1, 4, 5,-1,-1,-1,-1,-1, 7, 5290, 5245, 4058) },
            { Divines.Arkay,    new RankBenefits(3, 0, 1, 4,-1,-1,-1,-1, 4, 7, 5287, 5242, 4055) },
            { Divines.Dibella,  new RankBenefits(4, 2, 1, 5,-1,-1,-1,-1,-1, 7, 5290, 5247, 4059) },
            { Divines.Julianos, new RankBenefits(0, 2,-1,-1, 3, 5,-1,-1,-1, 6, 6610, 5246, 4060) },
            { Divines.Kynareth, new RankBenefits(4, 1,-1,-1,-1,-1, 3, 6,-1, 7, 5290, 5249, 4062) },
            { Divines.Mara,     new RankBenefits(4, 1, 2, 5,-1,-1,-1,-1,-1, 7, 5289, 5244, 4057) },
            { Divines.Stendarr, new RankBenefits(4, 0, 2, 5,-1,-1,-1,-1,-1, 7, 5289, 5248, 4061) },
            { Divines.Zenithar, new RankBenefits(4, 1, 1, 6,-1,-1,-1,-1,-1, 8, 5288, 5243, 4056) },
        };

        static Dictionary<Divines, List<DFCareer.Skills>> guildSkills = new Dictionary<Divines, List<DFCareer.Skills>>()
        {
            { Divines.Akatosh, new List<DFCareer.Skills>() {
                DFCareer.Skills.Alteration,
                DFCareer.Skills.Daedric,
                DFCareer.Skills.Destruction,
                DFCareer.Skills.Dragonish,
                DFCareer.Skills.LongBlade,
                DFCareer.Skills.Running,
                DFCareer.Skills.Stealth
            } },
            { Divines.Arkay, new List<DFCareer.Skills>() {
                DFCareer.Skills.Axe,
                DFCareer.Skills.Backstabbing,
                DFCareer.Skills.Daedric,
                DFCareer.Skills.Destruction,
                DFCareer.Skills.Medical,
                DFCareer.Skills.Restoration,
                DFCareer.Skills.ShortBlade
            } },
            { Divines.Dibella, new List<DFCareer.Skills>() {
                DFCareer.Skills.Daedric,
                DFCareer.Skills.Etiquette,
                DFCareer.Skills.Illusion,
                DFCareer.Skills.Lockpicking,
                DFCareer.Skills.LongBlade,
                DFCareer.Skills.Nymph,
                DFCareer.Skills.Orcish,
                DFCareer.Skills.Restoration
            } },
            { Divines.Julianos, new List<DFCareer.Skills>() {
                DFCareer.Skills.Alteration,
                DFCareer.Skills.Daedric,
                DFCareer.Skills.Impish,
                DFCareer.Skills.Lockpicking,
                DFCareer.Skills.Mysticism,
                DFCareer.Skills.ShortBlade,
                DFCareer.Skills.Thaumaturgy
            } },
            { Divines.Kynareth, new List<DFCareer.Skills>() {
                DFCareer.Skills.Archery,
                DFCareer.Skills.Climbing,
                DFCareer.Skills.Daedric,
                DFCareer.Skills.Destruction,
                DFCareer.Skills.Dodging,
                DFCareer.Skills.Dragonish,
                DFCareer.Skills.Harpy,
                DFCareer.Skills.Illusion,
                DFCareer.Skills.Jumping,
                DFCareer.Skills.Running,
                DFCareer.Skills.Stealth
            } },
            { Divines.Mara, new List<DFCareer.Skills>() {
                DFCareer.Skills.Archery,
                DFCareer.Skills.CriticalStrike,
                DFCareer.Skills.Daedric,
                DFCareer.Skills.Etiquette,
                DFCareer.Skills.Harpy,
                DFCareer.Skills.Illusion,
                DFCareer.Skills.Medical,
                DFCareer.Skills.Nymph,
                DFCareer.Skills.Restoration,
                DFCareer.Skills.Streetwise } },
            { Divines.Stendarr, new List<DFCareer.Skills>() {
                DFCareer.Skills.Axe,
                DFCareer.Skills.BluntWeapon,
                DFCareer.Skills.CriticalStrike,
                DFCareer.Skills.Daedric,
                DFCareer.Skills.Dodging,
                DFCareer.Skills.Medical,
                DFCareer.Skills.Restoration
            } },
            { Divines.Zenithar, new List<DFCareer.Skills>() {
                DFCareer.Skills.BluntWeapon,
                DFCareer.Skills.Centaurian,
                DFCareer.Skills.Daedric,
                DFCareer.Skills.Etiquette,
                DFCareer.Skills.Giantish,
                DFCareer.Skills.Harpy,
                DFCareer.Skills.Mercantile,
                DFCareer.Skills.Orcish,
                DFCareer.Skills.Pickpocket,
                DFCareer.Skills.Spriggan,
                DFCareer.Skills.Streetwise,
                DFCareer.Skills.Thaumaturgy
            } },
        };

        static Dictionary<Divines, List<DFCareer.Skills>> trainingSkills = new Dictionary<Divines, List<DFCareer.Skills>>()
        {
            { Divines.Akatosh, new List<DFCareer.Skills>() {
                DFCareer.Skills.Alteration, DFCareer.Skills.Archery, DFCareer.Skills.Daedric,
                DFCareer.Skills.Destruction, DFCareer.Skills.Dragonish, DFCareer.Skills.LongBlade,
                DFCareer.Skills.Running, DFCareer.Skills.Stealth, DFCareer.Skills.Swimming } },
            { Divines.Arkay, new List<DFCareer.Skills>() {
                DFCareer.Skills.Axe, DFCareer.Skills.Backstabbing, DFCareer.Skills.Climbing,
                DFCareer.Skills.CriticalStrike, DFCareer.Skills.Daedric, DFCareer.Skills.Destruction,
                DFCareer.Skills.Medical, DFCareer.Skills.Restoration, DFCareer.Skills.ShortBlade } },
            { Divines.Dibella, new List<DFCareer.Skills>() {
                DFCareer.Skills.Daedric, DFCareer.Skills.Etiquette, DFCareer.Skills.Harpy, DFCareer.Skills.Illusion,
                DFCareer.Skills.Lockpicking, DFCareer.Skills.LongBlade, DFCareer.Skills.Nymph,
                DFCareer.Skills.Orcish, DFCareer.Skills.Restoration, DFCareer.Skills.Streetwise } },
            { Divines.Julianos, new List<DFCareer.Skills>() {
                DFCareer.Skills.Alteration, DFCareer.Skills.CriticalStrike, DFCareer.Skills.Daedric,
                DFCareer.Skills.Impish, DFCareer.Skills.Lockpicking, DFCareer.Skills.Mercantile,
                DFCareer.Skills.Mysticism, DFCareer.Skills.ShortBlade, DFCareer.Skills.Thaumaturgy } },
            { Divines.Kynareth, new List<DFCareer.Skills>() {
                DFCareer.Skills.Archery, DFCareer.Skills.Climbing, DFCareer.Skills.Daedric, DFCareer.Skills.Destruction,
                DFCareer.Skills.Dodging, DFCareer.Skills.Dragonish, DFCareer.Skills.Harpy, DFCareer.Skills.Illusion,
                DFCareer.Skills.Jumping, DFCareer.Skills.Running, DFCareer.Skills.Stealth } },
            { Divines.Mara, new List<DFCareer.Skills>() {
                DFCareer.Skills.Archery, DFCareer.Skills.CriticalStrike, DFCareer.Skills.Daedric,
                DFCareer.Skills.Etiquette, DFCareer.Skills.Harpy, DFCareer.Skills.Illusion, DFCareer.Skills.Medical,
                DFCareer.Skills.Nymph, DFCareer.Skills.Restoration, DFCareer.Skills.Streetwise } },
            { Divines.Stendarr, new List<DFCareer.Skills>() {
                DFCareer.Skills.Axe, DFCareer.Skills.BluntWeapon, DFCareer.Skills.CriticalStrike,
                DFCareer.Skills.Daedric, DFCareer.Skills.Dodging, DFCareer.Skills.Medical,
                DFCareer.Skills.Orcish, DFCareer.Skills.Restoration, DFCareer.Skills.Spriggan } },
            { Divines.Zenithar, new List<DFCareer.Skills>() {
                DFCareer.Skills.BluntWeapon, DFCareer.Skills.Centaurian, DFCareer.Skills.Daedric, DFCareer.Skills.Etiquette,
                DFCareer.Skills.Giantish, DFCareer.Skills.Harpy, DFCareer.Skills.Mercantile, DFCareer.Skills.Orcish,
                DFCareer.Skills.Pickpocket, DFCareer.Skills.Spriggan, DFCareer.Skills.Streetwise, DFCareer.Skills.Thaumaturgy } },
        };

        #endregion

        #region Properties & Data

        string[] rankTitles = new string[] {
                "Novice", "Initiate", "Acolyte", "Adept", "Curate", "Disciple", "Brother", "Diviner", "Master", "Patriarch"
        };

        public override string[] RankTitles { get { return rankTitles; } }

        public override List<DFCareer.Skills> GuildSkills { get { return guildSkills[deity]; } }

        public override List<DFCareer.Skills> TrainingSkills { get { return trainingSkills[deity]; } }

        public static List<DFCareer.Skills> GetTrainingSkills(Divines deity)
        {
            return trainingSkills[deity];
        }

        #endregion

        #region Temple Deity

        Divines deity;

        public Divines Deity { get { return deity; } }

        public Temple(Divines deity) : base()
        {
            this.deity = deity;
        }

        public static Divines GetDivine(int factionId)
        {
            // Temple hall:
            if (Enum.IsDefined(typeof(Divines), factionId))
                return (Divines) factionId;

            // Templar order:
            PersistentFactionData persistentFactionData = GameManager.Instance.PlayerEntity.FactionData;
            FactionFile.FactionData factionData;
            if (persistentFactionData.GetFactionData(factionId, out factionData))
            {
                if (Enum.IsDefined(typeof(Divines), factionData.parent))
                    return (Divines) factionData.parent;
            }
            throw new ArgumentOutOfRangeException("There is no Divine that matches the factionId: "+ factionId);
        }

        public DFCareer.Stats BlessingStat()
        {
            switch (deity)
            {
                case Divines.Akatosh:
                    return DFCareer.Stats.Speed;
                case Divines.Dibella:
                    return DFCareer.Stats.Luck;
                case Divines.Julianos:
                    return DFCareer.Stats.Intelligence;
                case Divines.Kynareth:
                    return DFCareer.Stats.Endurance;
                case Divines.Mara:
                    return DFCareer.Stats.Personality;

            }
            return DFCareer.Stats.Endurance;
        }

        #endregion

        #region Guild Membership and Faction

        // TESTING ONLY - REMOVE!
        public override bool CanRest()
        {
            return IsMember();
        }

        public override int GetFactionId()
        {
            return (int) deity;
        }

        // Temple guild names are different from affiliation
        public override string GetGuildName()
        {
            RankBenefits benefits = templeRankBenefits[deity];
            TextFile.Token[] tokens = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(benefits.GetTempleNameMsgId());
            return tokens[0].text;
        }

        public override string GetTitle()
        {
            if (GameManager.Instance.PlayerEntity.Gender == Genders.Female)
                if (rank == 9)
                    return "Matriarch";     // Not calling female chars 'Patriarch'!
                else if (rank == 6)
                    return "Sister";        // Not calling female chars 'Brother'!

            return IsMember() ? rankTitles[rank] : "non-member";
        }

        #endregion

        #region Guild Ranks

        public override TextFile.Token[] TokensPromotion(int newRank)
        {
            RankBenefits benefits = templeRankBenefits[deity];
            return DaggerfallUnity.Instance.TextProvider.GetRSCTokens(benefits.GetPromotionMsgId(newRank));
        }

        #endregion

        #region Benefits

        public override bool FreeHealing()
        {
            return (templeRankBenefits[deity].healing <= rank);
        }

        #endregion

        #region Special guild benefits:

        public override int FastTravel(int duration)
        {
            if (deity == Divines.Akatosh)
            {
                Debug.LogFormat("Akatosh FastTravel= {0} -{1} less hours", duration, duration - (int)(((95f - rank) / 100) * duration));
                return (int)(((95f - rank) / 100) * duration);
            }
            return duration;
        }

        public override int ReducedCureCost(int price)
        {// TEST
            if (deity == Divines.Arkay)
                return ((10 - rank) / 10) * price;
            else
                return price;
        }

        public override int DeepBreath(int duration)
        {
            if (deity == Divines.Kynareth)
            {
                Debug.LogFormat("Kynareth DeepBreath= {0} +{1} extra seconds", duration, (int)((((10f + rank) / 10) - 1) * duration));
                return (int)(((10f + rank) / 10) * duration);
            }
            return duration;
        }

        public override bool AvoidDeath()
        {
            if (deity == Divines.Stendarr &&
                !GameManager.Instance.PlayerEnterExit.IsPlayerSubmerged &&
                UnityEngine.Random.Range(1, 50) < rank)
            {
                Debug.LogFormat("Stendarr AvoidDeath= success");
                // TODO: what is "weaker state" is it more than being left on 1 health?
                return true;
            }
            return false;
        }

        #endregion

        #region Service Access:

        public override bool CanAccessLibrary()
        {
            return (templeRankBenefits[deity].library <= rank);
        }

        public override bool CanAccessService(GuildServices service)
        {
            switch (service)
            {
                case GuildServices.Training:
                    return true;
                case GuildServices.Quests:
                    return true;
                case GuildServices.Donate:
                    return true;
                case GuildServices.CureDisease:
                    return true;
                case GuildServices.BuyPotions:
                    return (templeRankBenefits[deity].buyPotions <= rank);
                case GuildServices.MakePotions:
                    return (templeRankBenefits[deity].makePotions <= rank);
                case GuildServices.BuySpells:
                    return (templeRankBenefits[deity].buySpells <= rank);
                case GuildServices.MakeSpells:
                    return (templeRankBenefits[deity].makeSpells <= rank);
                case GuildServices.BuyMagicItems:
                    return (templeRankBenefits[deity].buyMagic <= rank);
                case GuildServices.MakeMagicItems:
                    return (templeRankBenefits[deity].makeMagic <= rank);
                case GuildServices.DaedraSummoning:
                    return (templeRankBenefits[deity].summoning <= rank);
                case GuildServices.BuySoulgems:
                    return (templeRankBenefits[deity].soulGems <= rank);
                default:
                    return false;
            }
        }

        #endregion

        #region Joining

        public override TextFile.Token[] TokensIneligible(PlayerEntity playerEntity)
        {
            int msgId = (GetReputation(playerEntity) < 0) ? IneligibleBadRepId : IneligibleLowSkillId;
            return DaggerfallUnity.Instance.TextProvider.GetRSCTokens(msgId);
        }
        public override TextFile.Token[] TokensEligible(PlayerEntity playerEntity)
        {
            return DaggerfallUnity.Instance.TextProvider.GetRSCTokens(EligibleMsgId);
        }
        public override TextFile.Token[] TokensWelcome()
        {
            RankBenefits benefits = templeRankBenefits[deity];
            return DaggerfallUnity.Instance.TextProvider.GetRSCTokens(benefits.GetWelcomeMsgId());
        }

        #endregion


        #region Serialization

        internal override GuildMembership_v1 GetGuildData()
        {
            return new GuildMembership_v1() { rank = rank, lastRankChange = lastRankChange, variant = (int)deity };
        }

        internal override void RestoreGuildData(GuildMembership_v1 data)
        {
            base.RestoreGuildData(data);
            deity = (Divines) data.variant;
        }

        #endregion


        #region Macro Handling

        public override MacroDataSource GetMacroDataSource()
        {
            return new TempleMacroDataSource(this);
        }

        /// <summary>
        /// MacroDataSource context sensitive methods for temples.
        /// </summary>
        protected class TempleMacroDataSource : GuildMacroDataSource
        {
            private Temple parent;
            public TempleMacroDataSource(Temple guild) : base(guild)
            {
                parent = guild;
            }

            public override string Divine()
            {
                return parent.deity.ToString();
            }
        }

        #endregion

    }

}