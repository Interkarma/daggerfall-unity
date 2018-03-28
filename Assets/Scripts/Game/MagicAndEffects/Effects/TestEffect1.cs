// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using DaggerfallConnect.Arena2;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    // TODO: Implement first test effect
    public class TestEffect1 : BaseEntityEffect
    {
        const string groupName = "Test";
        const string subGroupName = "Effect1";
        const string groupKey = "TestEffect1";

        public override string GroupName { get { return groupName; } }
        public override string SubGroupName { get { return subGroupName; } }
        public override string GroupKey { get { return groupKey; } }

        public TestEffect1(EntityEffectBundle parentBundle)
            : base(parentBundle)
        {
        }

        public override IEntityEffect CreateNew(EntityEffectBundle parentBundle)
        {
            TestEffect1 effect = new TestEffect1(parentBundle);

            return effect;
        }
    }
}