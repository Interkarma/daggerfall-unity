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
    /// Chameleon - True
    /// </summary>
    public class CureParalyzation : BaseEntityEffect
    {
        public override void SetProperties()
        {
            properties.Key = "Cure-Paralyzation";
            properties.ClassicKey = MakeClassicKey(3, 2);
            properties.GroupName = TextManager.Instance.GetText("ClassicEffects", "cure");
            properties.SubGroupName = TextManager.Instance.GetText("ClassicEffects", "paralyzation");
            properties.SpellMakerDescription = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1511);
            properties.SpellBookDescription = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1211);
            properties.SupportChance = true;
            properties.AllowedTargets = EntityEffectBroker.TargetFlags_All;
            properties.AllowedElements = EntityEffectBroker.ElementFlags_MagicOnly;
            properties.AllowedCraftingStations = EntityEffectBroker.MagicCraftingFlags_None;
            properties.MagicSkill = DFCareer.MagicSkills.Restoration;
            properties.ChanceCosts = MakeEffectCosts(20, 140);
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