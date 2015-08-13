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

namespace DaggerfallWorkshop.Game.Effects
{
    /// <summary>
    /// Flags for effect types.
    /// </summary>
    [Flags]
    public enum EffectFlags
    {
        None = 0,
        Paralysis = 1,
        Magic = 2,
        Poison = 4,
        Fire = 8,
        Frost = 16,
        Shock = 64,
        Disease = 128,
    }
}