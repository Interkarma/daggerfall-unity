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
    /// Regenerate
    /// </summary>
    public class Regenerate : IncumbentEffect
    {
        public static readonly string EffectKey = "Regenerate";

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.ClassicKey = MakeClassicKey(18, 255);
            properties.GroupName = TextManager.Instance.GetText("ClassicEffects", "regenerate");
            properties.SubGroupName = string.Empty;
            properties.SpellMakerDescription = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1566);
            properties.SpellBookDescription = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1266);
            properties.SupportDuration = true;
            properties.SupportMagnitude = true;
            properties.AllowedTargets = EntityEffectBroker.TargetFlags_All;
            properties.AllowedElements = EntityEffectBroker.ElementFlags_MagicOnly;
            properties.AllowedCraftingStations = MagicCraftingStations.SpellMaker;
            properties.MagicSkill = DFCareer.MagicSkills.Restoration;
            properties.DurationCosts = MakeEffectCosts(100, 20);
            properties.MagnitudeCosts = MakeEffectCosts(8, 8);
        }

        public override void Start(EntityEffectManager manager, DaggerfallEntityBehaviour caster = null)
        {
            base.Start(manager, caster);

            // Output "You are regenerating." if the host manager is player
            if (manager.EntityBehaviour == GameManager.Instance.PlayerEntityBehaviour)
            {
                DaggerfallUI.AddHUDText(TextManager.Instance.GetText(textDatabase, "youAreRegenerating"), 1.5f);
            }
        }

        protected override bool IsLikeKind(IncumbentEffect other)
        {
            return (other is Regenerate);
        }

        protected override void AddState(IncumbentEffect incumbent)
        {
            // Stack my rounds onto incumbent
            incumbent.RoundsRemaining += RoundsRemaining;
        }

        public override void MagicRound()
        {
            base.MagicRound();

            // Get peered entity gameobject
            DaggerfallEntityBehaviour entityBehaviour = GetPeeredEntityBehaviour(manager);
            if (!entityBehaviour)
                return;

            // Increase target health
            entityBehaviour.Entity.IncreaseHealth(GetMagnitude(caster));
        }
    }
}
