// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
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
    public class MagesGuild : Guild
    {
        #region Constants

        protected const int IneligibleBadRepId = 612;
        protected const int IneligibleLowSkillId = 611;
        protected const int EligibleMsgId = 606;
        protected const int WelcomeMsgId = 5293;
        protected const int PromotionMsgId = 5236;
        protected const int PromotionLibraryId = 5230;
        protected const int PromotionMagicItemsId = 5231;
        protected const int PromotionEnchantId = 5232;
        protected const int PromotionSummonId = 5233;
        protected const int PromotionTeleportId = 5234;

        private const int factionId = (int)FactionFile.FactionIDs.The_Mages_Guild;

        #endregion

        #region Properties & Data

        static List<DFCareer.Skills> guildSkills = new List<DFCareer.Skills>() {
                DFCareer.Skills.Alteration,
                DFCareer.Skills.Destruction,
                DFCareer.Skills.Illusion,
                DFCareer.Skills.Mysticism,
                DFCareer.Skills.Restoration,
                DFCareer.Skills.Thaumaturgy
            };

        static List<DFCareer.Skills> trainingSkills = new List<DFCareer.Skills>() {
                DFCareer.Skills.Alteration,
                DFCareer.Skills.Daedric,
                DFCareer.Skills.Destruction,
                DFCareer.Skills.Dragonish,
                DFCareer.Skills.Harpy,
                DFCareer.Skills.Illusion,
                DFCareer.Skills.Impish,
                DFCareer.Skills.Mysticism,
                DFCareer.Skills.Orcish,
                DFCareer.Skills.Restoration,
                DFCareer.Skills.Spriggan,
                DFCareer.Skills.Thaumaturgy
            };

        public override string[] RankTitles { get { return TextManager.Instance.GetLocalizedTextList("magesRanks"); } }

        public override List<DFCareer.Skills> GuildSkills { get { return guildSkills; } }

        public override List<DFCareer.Skills> TrainingSkills { get { return trainingSkills; } }

        #endregion

        #region Guild Membership and Faction

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
                    return PromotionLibraryId;
                case 3:
                    return PromotionMagicItemsId;
                case 6:
                    return PromotionSummonId;
                case 8:
                    return PromotionTeleportId;
                default:
                    return PromotionMsgId;
            }
        }

        #endregion

        #region Benefits

        public override bool HallAccessAnytime()
        {
            return (rank >= 6);
        }

        public override bool FreeMagickaRecharge()
        {
            if (IsMember() && GameManager.Instance.PlayerEntity.Career.NoRegenSpellPoints)
                return true;
            return false;
        }

        #endregion

        #region Service Access:

        public override bool CanAccessLibrary()
        {
            return (rank >= 2);
        }

        public override bool CanAccessService(GuildServices service)
        {
            switch (service)
            {
                case GuildServices.Training:
                    return IsMember();
                case GuildServices.Quests:
                    return true;
                case GuildServices.Identify:
                    return true;
                case GuildServices.BuySpellsMages:
                    return true;
                case GuildServices.MakeSpells:
                    return IsMember();
                case GuildServices.BuyMagicItems:
                    return (rank >= 3);
                case GuildServices.MakeMagicItems:
                    return (rank >= 5);
                case GuildServices.Teleport:
                    return (rank >= 8);
                case GuildServices.DaedraSummoning:
                    return (rank >= 6);
                case GuildServices.BuySoulgems:
                    return (rank >= 4);

                default:
                    return false;
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
    }

}
