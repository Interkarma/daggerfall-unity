// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut
// Contributors:    

using System;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallConnect.Arena2;
using System.Collections.Generic;
using DaggerfallConnect;

namespace DaggerfallWorkshop.Game.Guilds
{
    public class NonMemberGuild : Guild
    {
        private bool canTrain = false;

        public override string[] RankTitles { get { throw new NotImplementedException(); } }

        public override List<DFCareer.Skills> GuildSkills { get { throw new NotImplementedException(); } }

        public override List<DFCareer.Skills> TrainingSkills { get { throw new NotImplementedException(); } }

        public NonMemberGuild(bool canTrain = false)
        {
            this.canTrain = canTrain;
        }

        public override bool IsMember()
        {
            return false;
        }

        public override int GetFactionId()
        {
            return 0;
        }

        public override TextFile.Token[] UpdateRank(PlayerEntity playerEntity)
        {
            return null;
        }

        public override void Join()
        {
            throw new NotImplementedException();
        }

        public override bool CanAccessService(GuildServices service)
        {
            if (service == GuildServices.Training)
                return canTrain;
            else
                return base.CanAccessService(service);
        }

        public override bool IsEligibleToJoin(PlayerEntity playerEntity)
        {
            throw new NotImplementedException();
        }

        public override TextFile.Token[] TokensPromotion(int newRank)
        {
            throw new NotImplementedException();
        }
        public override TextFile.Token[] TokensIneligible(PlayerEntity playerEntity)
        {
            throw new NotImplementedException();
        }
        public override TextFile.Token[] TokensEligible(PlayerEntity playerEntity)
        {
            throw new NotImplementedException();
        }
        public override TextFile.Token[] TokensWelcome()
        {
            throw new NotImplementedException();
        }

    }

}
