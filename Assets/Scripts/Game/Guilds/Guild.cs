// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut
// Contributors:    

using UnityEngine;
using System.Collections.Generic;
using DaggerfallConnect;
using System;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop.Game.Guilds
{
    /// <summary>
    ///  Guild objects define player status and benefits with the guild.
    /// </summary>
    public abstract class Guild : IMacroContextProvider  // TODO: Extract interface?
    {
        #region Constants

        public const int memberTrainingCost = 100;
        public const int nonMemberTrainingCost = 400;

        protected const int DemotionId = 667;
        protected const int ExpulsionId = 668;

        #endregion

        #region Static Data

        protected static string[] rankTitles;

        protected static int[] rankReqReputation = new int[] {  0, 10, 20, 30, 40, 50, 60, 70, 80, 90 };
        protected static int[] rankReqSkillHigh = new int[]  { 22, 23, 31, 39, 47, 55, 63, 71, 79, 87 };
        protected static int[] rankReqSkillLow = new int[]   {  4,  5,  9, 13, 17, 21, 25, 29, 33, 37 };

        protected static List<DFCareer.Skills> guildSkills;
        protected static List<DFCareer.Skills> trainingSkills;

        #endregion

        #region Properties

        public virtual List<DFCareer.Skills> GuildSkills { get { return guildSkills; } }

        public virtual List<DFCareer.Skills> TrainingSkills { get { return trainingSkills; } }

        #endregion

        #region Guild Ranks

        protected int rank = -1;

        protected int lastRankChange = 0;

        public int Rank { get { return rank; } }

        public virtual TextFile.Token[] UpdateRank(PlayerEntity playerEntity)
        {
            TextFile.Token[] tokens = null;

            // Have 28 days passed?
            if (CalculateDaySinceZero(DaggerfallUnity.Instance.WorldTime.Now) >= lastRankChange + 1) //28)
            {
                // Does player qualify for promotion / demotion?
                int newRank = CalculateNewRank(playerEntity);
                if (newRank != rank)
                {
                    if (newRank > rank)         // Promotion
                        tokens = TokensPromotion(newRank);
                    else if (newRank < 0)       // Expulsion
                        tokens = TokensExpulsion();
                    else if (newRank < rank)    // Demotion
                        tokens = TokensDemotion();

                    rank = newRank;
                    lastRankChange = CalculateDaySinceZero(DaggerfallUnity.Instance.WorldTime.Now);
                }
            }
            return tokens;
        }

        protected virtual int CalculateNewRank(PlayerEntity playerEntity)
        {
            // Check reputation & skills
            int rep = GetReputation(playerEntity);
            if (rep < 0)
                return -1;  // Expelled.

            int high = 0, low = 0, r;
            for (r = 0; r < rankReqReputation.Length; r++)
            {
                CalculateNumHighLowSkills(playerEntity, r, out high, out low);
                if (rep < rankReqReputation[r] || high < 1 || low + high < 2)
                    break;
            }
            Debug.LogFormat("rep: {0} high#: {1} low#: {2} new rank: {3}", rep, high, low, r-1);
            return --r;
        }

        protected void CalculateNumHighLowSkills(PlayerEntity playerEntity, int rank, out int high, out int low)
        {
            high = 0;
            low = 0;
            foreach (DFCareer.Skills skill in GuildSkills)
            {
                int skillVal = playerEntity.Skills.GetPermanentSkillValue(skill);
                if (skillVal >= rankReqSkillHigh[rank])
                    high++;
                else if (skillVal >= rankReqSkillLow[rank])
                    low++;
            }
        }

        private static int CalculateDaySinceZero(DaggerfallDateTime date)
        {
            return (date.Year * DaggerfallDateTime.DaysPerYear) + date.DayOfYear;
        }

        public virtual TextFile.Token[] TokensPromotion(int newRank)
        {
            throw new NotImplementedException();
        }

        public virtual TextFile.Token[] TokensDemotion()
        {
            return DaggerfallUnity.Instance.TextProvider.GetRandomTokens(DemotionId);
        }

        public virtual TextFile.Token[] TokensExpulsion()
        {
            return DaggerfallUnity.Instance.TextProvider.GetRandomTokens(ExpulsionId);
        }

        #endregion

        #region Guild Membership and Faction Data

        public abstract bool IsMember();

        public abstract int GetFactionId();

        public virtual int GetReputation(PlayerEntity playerEntity)
        {
            return playerEntity.FactionData.GetReputation(GetFactionId());
        }

        public virtual string GetFactionName()
        {
            FactionFile.FactionData factionData;
            if (DaggerfallUnity.Instance.ContentReader.FactionFileReader.GetFactionData(GetFactionId(), out factionData))
                return factionData.name;
            return "unknown-guild";
        }

        public virtual string GetTitle()
        {
            return IsMember() ? rankTitles[rank] : "non-member";
        }

        #endregion

        #region Benefits

        public virtual bool CanRest()
        {
            return false;
        }

        public virtual bool HallAccessAnytime()
        {
            return false;
        }

        public virtual bool FreeHealing()
        {
            return false;
        }

        public virtual int ReducedRepairCost(int price)
        {
            return price;
        }

        #endregion

        #region Special temple benefits:

        public virtual int FastTravel(int duration)
        {
            return duration;
        }

        public virtual int ReducedCureCost(int price)
        {
            return price;
        }

        public virtual int DeepBreath(int duration)
        {
            return duration;
        }

        public virtual bool AvoidDeath()
        {
            return false;
        }

        #endregion

        #region Service Access:

        public virtual bool Training()
        {
            return IsMember();
        }

        public virtual bool Library()
        {
            return false;
        }

        public virtual bool BuyPotions()
        {
            return false;
        }

        public virtual bool MakePotions()
        {
            return false;
        }

        public virtual bool BuyMagic()
        {
            return false;
        }

        public virtual bool MakeMagic()
        {
            return false;
        }

        public virtual bool BuySpells()
        {
            return false;
        }

        public virtual bool MakeSpells()
        {
            return false;
        }

        public virtual bool SoulGems()
        {
            return false;
        }

        public virtual bool Summoning()
        {
            return false;
        }

        #endregion

        #region Service: Training

        public virtual int GetTrainingPrice()
        {
            int costPerLev = IsMember() ? memberTrainingCost : nonMemberTrainingCost;
            return costPerLev * GameManager.Instance.PlayerEntity.Level;
        }

        #endregion

        #region Joining

        public virtual void Join()
        {
            rank = 0;
            lastRankChange = CalculateDaySinceZero(DaggerfallUnity.Instance.WorldTime.Now);
        }

        public virtual bool IsEligibleToJoin(PlayerEntity playerEntity)
        {
            // Check reputation & skills
            int rep = playerEntity.FactionData.GetReputation(GetFactionId());
            int high, low;
            CalculateNumHighLowSkills(playerEntity, 0, out high, out low);
            return (rep >= rankReqReputation[0] && high > 0 && low + high > 1);
        }

        public virtual TextFile.Token[] TokensIneligible(PlayerEntity playerEntity)
        {
            throw new NotImplementedException();
        }
        public virtual TextFile.Token[] TokensEligible(PlayerEntity playerEntity)
        {
            throw new NotImplementedException();
        }
        public virtual TextFile.Token[] TokensWelcome()
        {
            throw new NotImplementedException();
        }

        #endregion


        #region Macro Handling

        public virtual MacroDataSource GetMacroDataSource()
        {
            return new GuildMacroDataSource(this);
        }

        /// <summary>
        /// MacroDataSource context sensitive methods for guilds.
        /// </summary>
        protected class GuildMacroDataSource : MacroDataSource
        {
            private Guild parent;
            public GuildMacroDataSource(Guild guild)
            {
                parent = guild;
            }

            public override string Amount()
            {
                return parent.GetTrainingPrice().ToString();
            }

            public override string GuildTitle()
            {
                return parent.GetTitle();
            }
        }

        #endregion
    }
}