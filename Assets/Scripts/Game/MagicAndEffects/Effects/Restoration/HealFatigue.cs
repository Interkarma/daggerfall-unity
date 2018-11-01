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

using DaggerfallConnect;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Heal - Fatigue
    /// </summary>
    public class HealFatigue : BaseEntityEffect
    {
        public override void SetProperties()
        {
            properties.Key = "Heal-Fatigue";
            properties.ClassicKey = MakeClassicKey(10, 9);
            properties.GroupName = TextManager.Instance.GetText("ClassicEffects", "heal");
            properties.SubGroupName = TextManager.Instance.GetText("ClassicEffects", "fatigue");
            properties.SpellMakerDescription = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1549);
            properties.SpellBookDescription = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1249);
            properties.SupportMagnitude = true;
            properties.AllowedTargets = EntityEffectBroker.TargetFlags_All;
            properties.AllowedElements = EntityEffectBroker.ElementFlags_MagicOnly;
            properties.AllowedCraftingStations = MagicCraftingStations.SpellMaker;
            properties.MagicSkill = DFCareer.MagicSkills.Restoration;
            properties.MagnitudeCosts = MakeEffectCosts(8, 28);
        }

        public override void MagicRound()
        {
            base.MagicRound();

            // Get peered entity gameobject
            DaggerfallEntityBehaviour entityBehaviour = GetPeeredEntityBehaviour(manager);
            if (!entityBehaviour)
                return;

            // Implement effect
            int magnitude = GetMagnitude(caster);
            entityBehaviour.Entity.IncreaseFatigue(magnitude, true);

            //Debug.LogFormat("{0} incremented {1}'s fatigue by {2} points", Key, entityBehaviour.EntityType.ToString(), magnitude);
        }
    }
}
