// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Daggerfall Workshop
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

namespace DaggerfallWorkshop.Game.Entity
{
    /// <summary>
    /// Daggerfall stats collection for every entity.
    /// </summary>
    [Serializable]
    public struct DaggerfallStats
    {
        const int defaultValue = 50;

        public int Strength;
        public int Intelligence;
        public int Willpower;
        public int Agility;
        public int Endurance;
        public int Personality;
        public int Speed;
        public int Luck;

        public void SetDefaults()
        {
            Strength = defaultValue;
            Intelligence = defaultValue;
            Willpower = defaultValue;
            Agility = defaultValue;
            Endurance = defaultValue;
            Personality = defaultValue;
            Speed = defaultValue;
            Luck = defaultValue;
        }
    }
}