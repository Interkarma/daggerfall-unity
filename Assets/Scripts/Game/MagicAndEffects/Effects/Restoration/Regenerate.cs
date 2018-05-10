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
    /// Regenerate
    /// </summary>
    public class Regenerate : BaseEntityEffect
    {
        public override void SetProperties()
        {
            properties.Key = "Regenerate";
            properties.ClassicKey = MakeClassicKey(18, 255);
            properties.GroupName = TextManager.Instance.GetText("ClassicEffects", "regenerate");
            properties.SubGroupName = string.Empty;
            properties.SpellMakerDescription = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1566);
            properties.SpellBookDescription = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1266);
            properties.SupportDuration = true;
            properties.SupportMagnitude = true;
            properties.AllowedTargets = EntityEffectBroker.TargetFlags_All;
            properties.AllowedElements = EntityEffectBroker.ElementFlags_MagicOnly;
            properties.AllowedCraftingStations = EntityEffectBroker.MagicCraftingFlags_None;
            properties.MagicSkill = DFCareer.MagicSkills.Restoration;
            properties.DurationCosts = MakeEffectCosts(100, 20);
            properties.MagnitudeCosts = MakeEffectCosts(8, 8);
        }

        public override void MagicRound()
        {
            base.MagicRound();

            //// Get peered entity gameobject
            //DaggerfallEntityBehaviour entityBehaviour = GetPeeredEntityBehaviour(manager);
            //if (!entityBehaviour)
            //    return;

            // TODO: Implement effect
        }
    }
}
