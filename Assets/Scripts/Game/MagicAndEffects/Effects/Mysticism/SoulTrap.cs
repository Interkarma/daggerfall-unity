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
    /// Soul Trap
    /// </summary>
    public class SoulTrap : IncumbentEffect
    {
        public static readonly string EffectKey = "SoulTrap";

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.ClassicKey = MakeClassicKey(12, 255);
            properties.GroupName = TextManager.Instance.GetText("ClassicEffects", "soulTrap");
            properties.SpellMakerDescription = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1603);
            properties.SpellBookDescription = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1303);
            properties.ShowSpellIcon = false;
            properties.SupportDuration = true;
            properties.SupportChance = true;
            properties.AllowedTargets = EntityEffectBroker.TargetFlags_Other;
            properties.AllowedElements = EntityEffectBroker.ElementFlags_All;
            properties.AllowedCraftingStations = MagicCraftingStations.SpellMaker;
            properties.MagicSkill = DFCareer.MagicSkills.Mysticism;
            properties.DurationCosts = MakeEffectCosts(60, 68);
            properties.ChanceCosts = MakeEffectCosts(40, 68);
        }

        public override void Start(EntityEffectManager manager, DaggerfallEntityBehaviour caster = null)
        {
            base.Start(manager, caster);
        }

        public override void Resume(EntityEffectManager.EffectSaveData_v1 effectData, EntityEffectManager manager, DaggerfallEntityBehaviour caster = null)
        {
            base.Resume(effectData, manager, caster);
        }

        protected override bool IsLikeKind(IncumbentEffect other)
        {
            return other is SoulTrap;
        }

        protected override void AddState(IncumbentEffect incumbent)
        {
            incumbent.RoundsRemaining += RoundsRemaining;
        }
    }
}
