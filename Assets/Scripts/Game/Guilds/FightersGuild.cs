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
    public class FightersGuild : Guild
    {
        #region Constants

        protected const int IneligibleBadRepId = 679;
        protected const int IneligibleLowSkillId = 680;
        protected const int EligibleMsgId = 681;
        protected const int WelcomeMsgId = 684;
        protected const int PromotionMsgId = 686;

        private const int factionId = 41;

        #endregion

        #region Static Data

        static FightersGuild()
        {
            rankTitles = new string[] {
                "Apprentice", "Journeyman", "Swordsman", "Protector", "Defender", "Warder", "Guardian", "Champion", "Warrior", "Master"
            };

            guildSkills = new List<DFCareer.Skills>() {
                DFCareer.Skills.Archery,    // 14 melissa
                DFCareer.Skills.Axe,        // 6
                DFCareer.Skills.BluntWeapon,// 3
                DFCareer.Skills.Giantish,   // 5
                DFCareer.Skills.LongBlade,  // 30
                DFCareer.Skills.Orcish,     // 6
                DFCareer.Skills.ShortBlade, // 11
            };

            trainingSkills = new List<DFCareer.Skills>() {
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
        }

        #endregion

        #region Guild Membership and Faction

        public override bool IsMember()
        {
            return rank >= 0;
        }

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
            return rank >= 6;
        }

        public override int ReducedRepairCost(int price)
        {
            return ((10 - rank) / 10) * price;
        }

        #endregion

        #region Service: Training

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