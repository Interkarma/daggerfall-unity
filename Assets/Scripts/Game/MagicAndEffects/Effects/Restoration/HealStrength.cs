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
using DaggerfallConnect;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Heal - Strength
    /// </summary>
    public class HealStrength : HealEffect
    {
        public override void SetProperties()
        {
            properties.Key = "Heal-Strength";
            properties.ClassicKey = MakeClassicKey(10, 0);
            properties.GroupName = TextManager.Instance.GetText("ClassicEffects", "heal");
            properties.SubGroupName = TextManager.Instance.GetText("ClassicEffects", "strength");
            properties.SpellMakerDescription = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1540);
            properties.SpellBookDescription = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1240);
            properties.SupportMagnitude = true;
            properties.AllowedTargets = EntityEffectBroker.TargetFlags_All;
            properties.AllowedElements = EntityEffectBroker.ElementFlags_MagicOnly;
            properties.MagicSkill = DFCareer.MagicSkills.Restoration;
            properties.MagnitudeCosts = MakeEffectCosts(40, 28);
            healStat = DFCareer.Stats.Strength;
        }
    }
}
