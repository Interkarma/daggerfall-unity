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
    public class FightersGuild : Guild
    {
        #region Constants

        protected const int IneligibleBadRepId = 679;
        protected const int IneligibleLowSkillId = 680;
        protected const int EligibleMsgId = 681;
        protected const int WelcomeMsgId = 684;
        protected const int PromotionMsgId = 686;

        private const int factionId = (int)FactionFile.FactionIDs.The_Fighters_Guild;

        #endregion

        #region Properties & Data

        static List<DFCareer.Skills> guildSkills = new List<DFCareer.Skills>() {
                DFCareer.Skills.Archery,
                DFCareer.Skills.Axe,
                DFCareer.Skills.BluntWeapon,
                DFCareer.Skills.Giantish,
                DFCareer.Skills.LongBlade,
                DFCareer.Skills.Orcish,
                DFCareer.Skills.ShortBlade,
            };

        static List<DFCareer.Skills> trainingSkills = new List<DFCareer.Skills>() {
                DFCareer.Skills.Archery,
                DFCareer.Skills.Axe,
                DFCareer.Skills.BluntWeapon,
                DFCareer.Skills.CriticalStrike,
                DFCareer.Skills.Giantish,
                DFCareer.Skills.Jumping,
                DFCareer.Skills.LongBlade,
                DFCareer.Skills.Orcish,
                DFCareer.Skills.Running,
                DFCareer.Skills.ShortBlade,
                DFCareer.Skills.Swimming
            };

        public override string[] RankTitles { get { return TextManager.Instance.GetLocalizedTextList("fightersRanks"); } }

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
            return DaggerfallUnity.Instance.TextProvider.GetRSCTokens(PromotionMsgId);
        }

        #endregion

        #region Benefits

        public override bool CanRest()
        {
            return IsMember();
        }

        public override bool HallAccessAnytime()
        {
            return (rank >= 6);
        }

        public override int AlterReward(int reward)
        {
            return (((10 + rank) << 8) / 10 * reward) >> 8;
        }

        public override int ReducedRepairCost(int price)
        {
            return (((10 - rank) << 8) / 10 * price) >> 8;
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
            return DaggerfallUnity.Instance.TextProvider.GetRSCTokens(WelcomeMsgId);
        }

        #endregion
    }

}
