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
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Serialization;

namespace DaggerfallWorkshop.Game.Guilds
{
    /// <summary>
    ///  Guild objects define player rank progression and benefits with the guild.
    ///  For vanilla style guilds, extend abstract class Guild.cs which defines most
    ///  of the vanilla base behaviour.
    /// </summary>
    public interface IGuild : IMacroContextProvider
    {
        #region Properties

        string[] RankTitles { get; }

        List<DFCareer.Skills> GuildSkills { get; }

        List<DFCareer.Skills> TrainingSkills { get; }

        #endregion

        #region Guild Ranks

        int Rank { get; set; }

        void ImportLastRankChange(uint timeOfLastRankChange);

        TextFile.Token[] UpdateRank(PlayerEntity playerEntity);

        TextFile.Token[] TokensPromotion(int newRank);

        TextFile.Token[] TokensDemotion();

        TextFile.Token[] TokensExpulsion();

        #endregion

        #region Guild Membership and Faction Data

        bool IsMember();

        int GetFactionId();

        int GetReputation(PlayerEntity playerEntity);

        string GetGuildName();

        string GetAffiliation();

        string GetTitle();

        #endregion

        #region Common Benefits

        bool CanRest();

        bool HallAccessAnytime();

        bool FreeHealing();

        bool FreeMagickaRecharge();

        int AlterReward(int reward);

        int ReducedRepairCost(int price);

        int ReducedIdentifyCost(int price);

        int ReducedCureCost(int price);

        #endregion

        #region Special benefits:

        bool FreeTavernRooms();

        bool FreeShipTravel();

        int FastTravel(int duration);

        int DeepBreath(int duration);

        bool AvoidDeath();

        #endregion

        #region Service Access:

        bool CanAccessLibrary();

        bool CanAccessService(GuildServices service);

        #endregion

        #region Service: Training

        int GetTrainingMax(DFCareer.Skills skill);

        int GetTrainingPrice();

        #endregion

        #region Joining

        void Join();

        void Leave();

        bool IsEligibleToJoin(PlayerEntity playerEntity);

        TextFile.Token[] TokensIneligible(PlayerEntity playerEntity);

        TextFile.Token[] TokensEligible(PlayerEntity playerEntity);

        TextFile.Token[] TokensWelcome();

        #endregion


        #region Serialization

        GuildMembership_v1 GetGuildData();

        void RestoreGuildData(GuildMembership_v1 data);

        #endregion
    }
}
