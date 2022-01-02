// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes: All additions or modifications that differ from the source code copyright (c) 2021-2022 Osorkon
//

using UnityEngine;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Continuous Damage - Health
    /// </summary>
    public class ContinuousDamageHealth : IncumbentEffect
    {
        public static readonly string EffectKey = "ContinuousDamage-Health";

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.ClassicKey = MakeClassicKey(1, 0);

            // [OSORKON] I don't know how to remove specific spells from the circinate spell list at
            // Mage Guilds and Julianos temples, so Wildfire is still available for purchase. As
            // a crude workaround, I commented out the line below. Now Wildfire does impact damage
            // but nothing over time, as it's still OP at high player levels. I'm sure there's a
            // way to remove Wildfire, but it's pretty low on my priority list.
            // properties.SupportDuration = true;

            properties.SupportMagnitude = true;
            properties.AllowedTargets = EntityEffectBroker.TargetFlags_Other;
            properties.AllowedElements = EntityEffectBroker.ElementFlags_All;

            // [OSORKON] I commented out the line below, so Continuous Damage - Health is no longer
            // a selectable effect from the Spell Maker. This effect is ridiculously OP at higher
            // player levels and was one of the first effects to get removed.
            // properties.AllowedCraftingStations = MagicCraftingStations.SpellMaker;

            properties.MagicSkill = DFCareer.MagicSkills.Destruction;
            properties.DurationCosts = MakeEffectCosts(28, 8);
            properties.MagnitudeCosts = MakeEffectCosts(40, 28);
        }

        public override string GroupName => TextManager.Instance.GetLocalizedText("continuousDamage");
        public override string SubGroupName => TextManager.Instance.GetLocalizedText("health");
        public override TextFile.Token[] SpellMakerDescription => DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1504);
        public override TextFile.Token[] SpellBookDescription => DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1204);

        public override void MagicRound()
        {
            base.MagicRound();

            // Get peered entity gameobject
            DaggerfallEntityBehaviour entityBehaviour = GetPeeredEntityBehaviour(manager);
            if (!entityBehaviour)
                return;

            // Implement effect
            int magnitude = GetMagnitude(caster);
            entityBehaviour.DamageHealthFromSource(this, magnitude, false, Vector3.zero);

            //Debug.LogFormat("Effect {0} damaged {1} by {2} health points and has {3} magic rounds remaining.", Key, entityBehaviour.name, magnitude, RoundsRemaining);
        }

        protected override bool IsLikeKind(IncumbentEffect other)
        {
            return (other is ContinuousDamageHealth);
        }

        protected override void AddState(IncumbentEffect incumbent)
        {
            // Stack my rounds onto incumbent
            incumbent.RoundsRemaining += RoundsRemaining;
        }
    }
}
