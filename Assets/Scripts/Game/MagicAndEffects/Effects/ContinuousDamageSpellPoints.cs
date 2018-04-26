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
    /// Continuous Damage - Spell Points
    /// </summary>
    public class ContinuousDamageSpellPoints : BaseEntityEffect
    {
        public override void SetProperties()
        {
            properties.Key = "ContinuousDamage-SpellPoints";
            properties.ClassicKey = MakeClassicKey(1, 2);
            properties.GroupName = TextManager.Instance.GetText("ClassicEffects", "continuousDamage");
            properties.SubGroupName = TextManager.Instance.GetText("ClassicEffects", "spellPoints");
            properties.SpellMakerDescription = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1506);
            properties.SpellBookDescription = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1206);
            properties.SupportDuration = true;
            properties.SupportMagnitude = true;
            properties.AllowedTargets = EntityEffectBroker.TargetFlags_Other;
            properties.AllowedElements = EntityEffectBroker.ElementFlags_All;
            properties.AllowedCraftingStations = EntityEffectBroker.MagicCraftingFlags_None;
            properties.MagicSkill = DFCareer.MagicSkills.Destruction;
            // TODO: Confirm costs - these are just fudge costs for now
            properties.DurationCosts = MakeEffectCosts(28, 8);
            properties.MagnitudeCosts = MakeEffectCosts(40, 28);
        }

        public override void MagicRound(EntityEffectManager manager, DaggerfallEntityBehaviour caster = null)
        {
            // Get peered entity gameobject
            DaggerfallEntityBehaviour entityBehaviour = GetPeeredEntityBehaviour(manager);
            if (!entityBehaviour)
                return;

            // TODO: Implement effect
            //int magnitude = GetMagnitude(caster);
        }
    }
}