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

namespace DaggerfallWorkshop.Game.Guilds
{
    public class Temple : Guild
    {
        #region Constants

        protected const int IneligibleBadRepId = 745;
        protected const int IneligibleLowSkillId = 744;
        protected const int EligibleMsgId = 740;
        protected const int WelcomeMsgId = 5290;
        protected const int PromotionMsgIds = 5249;      // Kynareth specific

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
        protected const int PromotionTrainerId = 6610;

        #endregion

        #region Enums and Structs

        public enum Divines
        {   // value = factionId
            //None     = 0,
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
            public readonly int promotionMsgId; // Generic promotion msg.

            public RankBenefits(int library, int healing, int buyPotions, int makePotions, int buyMagic, int makeItems, int buySpells, int makeSpells, int soulGems, int summoning, int promotionMsgId)
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
                this.promotionMsgId = promotionMsgId;
            }

            public int GetPromotionMsgId(int rank)
            {
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
            { Divines.Akatosh,  new RankBenefits(2, 1, 4, 5,-1,-1,-1,-1,-1, 7, 5245) },
            { Divines.Arkay,    new RankBenefits(3, 0, 1, 4,-1,-1,-1,-1, 4, 7, 5287) },
            { Divines.Dibella,  new RankBenefits(4, 2, 1, 5,-1,-1,-1,-1,-1, 7, 5247) },
            { Divines.Julianos, new RankBenefits(0, 2,-1,-1, 3, 5,-1,-1,-1, 6, 5246) },
            { Divines.Kynareth, new RankBenefits(4, 1,-1,-1,-1,-1, 3, 6,-1, 7, 5249) },
            { Divines.Mara,     new RankBenefits(4, 1, 2, 5,-1,-1,-1,-1,-1, 7, 5244) },
            { Divines.Stendarr, new RankBenefits(4, 0, 2, 5,-1,-1,-1,-1,-1, 7, 5248) },
            { Divines.Zenithar, new RankBenefits(4, 1, 1, 6,-1,-1,-1,-1,-1, 8, 5288) },
        };


        static Temple()
        {
            rankTitles = new string[] {
                "Novice", "Initiate", "Acolyte", "Adept", "Curate", "Disciple", "Brother", "Diviner", "Master", "Patriarch"
            };

            guildSkills = new List<DFCareer.Skills>() {  // Kynareth same as training? How to check?
                DFCareer.Skills.Archery,    // 26 tim
                DFCareer.Skills.Climbing,   // 16
                DFCareer.Skills.Daedric,    // 5
                DFCareer.Skills.Destruction,// 4
                DFCareer.Skills.Dodging,    // 20
                DFCareer.Skills.Dragonish,  // 6
                DFCareer.Skills.Harpy,      // 5
                DFCareer.Skills.Illusion,   // 3
                DFCareer.Skills.Jumping,    // 15
                DFCareer.Skills.Running,    // 19
                DFCareer.Skills.Stealth     // 10
            };

            trainingSkills = new List<DFCareer.Skills>() {
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
            };
        }

        #endregion

        #region Temple Deity

        Divines deity;

        public Divines Deity { get { return deity; } }

        public Temple(Divines deity) : base()
        {
            this.deity = deity;
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

        public override bool IsMember()
        {
            return rank >= 0;
        }

        public override int GetFactionId()
        {
            return (int) deity;
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
            return (rank > 0);
        }

        public override int DeepBreath(int duration)
        {
            if (deity == Divines.Kynareth)
                return ((10 + rank) / 10) * duration;
            else
                return duration;
        }

        #endregion

        #region Service Access:

        public override bool Training()
        {
            return true;
        }

        public override bool Library()
        {
            return (templeRankBenefits[deity].library <= rank);
        }

        public override bool BuyPotions()
        {
            return (templeRankBenefits[deity].buyPotions <= rank);
        }

        public override bool MakePotions()
        {
            return (templeRankBenefits[deity].makePotions <= rank);
        }

        public override bool BuyMagic()
        {
            return (templeRankBenefits[deity].buyMagic <= rank);
        }

        public override bool MakeMagic()
        {
            return (templeRankBenefits[deity].makeMagic <= rank);
        }

        public override bool BuySpells()
        {
            return (templeRankBenefits[deity].buySpells <= rank);
        }

        public override bool MakeSpells()
        {
            return (templeRankBenefits[deity].makeSpells <= rank);
        }

        public override bool SoulGems()
        {
            return (templeRankBenefits[deity].soulGems <= rank);
        }

        public override bool Summoning()
        {
            return (templeRankBenefits[deity].library <= rank);
        }

        #endregion

        #region Joining

        public override TextFile.Token[] TokensIneligible(PlayerEntity playerEntity)
        {
            int rep = playerEntity.FactionData.GetReputation(GetFactionId());
            int msgId = (rep < 0) ? IneligibleBadRepId : IneligibleLowSkillId;
            return DaggerfallUnity.Instance.TextProvider.GetRSCTokens(msgId);
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
    }

}