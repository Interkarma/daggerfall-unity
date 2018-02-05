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

namespace DaggerfallWorkshop.Game.Effects
{
    /// <summary>
    /// How this spell targets entities in world space.
    /// Can be used as flags by effect system to declare supported targets.
    /// </summary>
    public enum TargetTypes
    {
        None = 0,
        CasterOnly = 1,
        ByTouch = 2,
        SingleTargetAtRange = 4,
        AreaAroundCaster = 8,
        AreaAtRange = 16,
    }

    /// <summary>
    /// How effect manifests for cast animations, billboard effects, resist checks, etc.
    /// Can be used as flags by effect system to declare supported elements.
    /// </summary>
    public enum EffectTypes
    {
        None = 0,
        Fire = 1,
        Cold = 2,
        Poison = 4,
        Shock = 8,
        Magic = 16,
    }
}