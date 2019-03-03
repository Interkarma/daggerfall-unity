// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Numidium
// Contributors:    
// 
// Notes:
//
using UnityEngine;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Used by the Wabbajack artifact staff to change enemies into random monsters.
    /// </summary>
    public class WabbajackEffect : BaseEntityEffect
    {
        public static readonly string EffectKey = "WabbajackEffect";

        public override void MagicRound()
        {
            base.MagicRound();

            Debug.Log("WabbajackEffect activated.");
        }

        public override void SetProperties()
        {
            properties.Key = EffectKey;
        }
    }
}
