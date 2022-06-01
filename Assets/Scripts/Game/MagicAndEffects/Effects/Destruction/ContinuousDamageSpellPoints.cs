// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
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
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Continuous Damage - Spell Points
    /// </summary>
    public class ContinuousDamageSpellPoints : IncumbentEffect
    {
        public static readonly string EffectKey = "ContinuousDamage-SpellPoints";

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.ClassicKey = MakeClassicKey(1, 2);
            properties.SupportDuration = true;
            properties.SupportMagnitude = true;
            properties.AllowedTargets = EntityEffectBroker.TargetFlags_Other;
            properties.AllowedElements = EntityEffectBroker.ElementFlags_All;
            properties.AllowedCraftingStations = MagicCraftingStations.SpellMaker;
            properties.MagicSkill = DFCareer.MagicSkills.Destruction;
            properties.DurationCosts = MakeEffectCosts(40, 8);
            properties.MagnitudeCosts = MakeEffectCosts(40, 28);
        }

        public override string GroupName => TextManager.Instance.GetLocalizedText("continuousDamage");
        public override string SubGroupName => TextManager.Instance.GetLocalizedText("spellPoints");
        public override TextFile.Token[] SpellMakerDescription => DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1506);
        public override TextFile.Token[] SpellBookDescription => DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1206);

        public override void MagicRound()
        {
            base.MagicRound();

            // Get peered entity gameobject
            DaggerfallEntityBehaviour entityBehaviour = GetPeeredEntityBehaviour(manager);
            if (!entityBehaviour)
                return;

            // Implement effect
            int magnitude = GetMagnitude(caster);
            entityBehaviour.DamageMagickaFromSource(this, magnitude);

            Debug.LogFormat("Effect {0} damaged {1} by {2} spell points points and has {3} magic rounds remaining.", Key, entityBehaviour.name, magnitude, RoundsRemaining);
        }

        protected override bool IsLikeKind(IncumbentEffect other)
        {
            return (other is ContinuousDamageSpellPoints);
        }

        protected override void AddState(IncumbentEffect incumbent)
        {
            // Stack my rounds onto incumbent
            incumbent.RoundsRemaining += RoundsRemaining;
        }
    }
}
