// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut
// Contributors:    

using System;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallConnect.Arena2;

namespace DaggerfallWorkshop.Game.Guilds
{
    public class NonMemberGuild : Guild
    {
        private bool canTrain = false;

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

        public override bool Training()
        {
            return canTrain;
        }

        public override bool IsEligibleToJoin(PlayerEntity playerEntity)
        {
            throw new NotImplementedException();
        }
    }

}