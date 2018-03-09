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
    public class MagesGuild : Guild
    {
        #region Constants

        protected const int IneligibleBadRepId = 612;
        protected const int IneligibleLowSkillId = 611;
        protected const int EligibleMsgId = 606;
        protected const int WelcomeMsgId = 5293;
        protected const int PromotionMsgId = 5236;

        private const int factionId = 40;

        #endregion

        #region Properties & Data

        static string[] rankTitles = new string[] {
                "Apprentice", "Journeyman", "Evoker", "Conjurer", "Magician", "Enchanter", "Warlock", "Wizard", "Master Wizard", "Archmage"
        };

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

        #endregion

        #region Guild Ranks

        public override TextFile.Token[] TokensPromotion(int newRank)
        {
            return DaggerfallUnity.Instance.TextProvider.GetRSCTokens(PromotionMsgId);
        }

        #endregion

        #region Benefits

        // TESTING ONLY - REMOVE!
        public override bool CanRest()
        {
            return IsMember();
        }

        public override bool HallAccessAnytime()
        {
            return (rank >= 6);
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
                case GuildServices.BuySpells:
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