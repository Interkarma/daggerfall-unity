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
    /// Silence
    /// </summary>
    public class Silence : IncumbentEffect
    {
        public static readonly string EffectKey = "Silence";

        bool awakeAlert = true;

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.ClassicKey = MakeClassicKey(19, 255);
            properties.GroupName = TextManager.Instance.GetText("ClassicEffects", "silence");
            properties.SubGroupName = string.Empty;
            properties.SpellMakerDescription = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1567);
            properties.SpellBookDescription = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1267);
            properties.SupportDuration = true;
            properties.SupportChance = true;
            properties.AllowedTargets = EntityEffectBroker.TargetFlags_All;
            properties.AllowedElements = EntityEffectBroker.ElementFlags_All;
            properties.AllowedCraftingStations = MagicCraftingStations.SpellMaker;
            properties.MagicSkill = DFCareer.MagicSkills.Mysticism;
            properties.DurationCosts = MakeEffectCosts(20, 100);
            properties.ChanceCosts = MakeEffectCosts(20, 100);
        }

        public override void Start(EntityEffectManager manager, DaggerfallEntityBehaviour caster = null)
        {
            base.Start(manager, caster);
            PlayerAggro();
        }

        public override void ConstantEffect()
        {
            base.ConstantEffect();
            StartSilence();
        }

        public override void Resume(EntityEffectManager.EffectSaveData_v1 effectData, EntityEffectManager manager, DaggerfallEntityBehaviour caster = null)
        {
            base.Resume(effectData, manager, caster);
            StartSilence();
        }

        public override void End()
        {
            base.End();
            EndSilence();
        }

        protected override bool IsLikeKind(IncumbentEffect other)
        {
            return (other is Silence);
        }

        protected override void AddState(IncumbentEffect incumbent)
        {
            // Stack my rounds onto incumbent
            incumbent.RoundsRemaining += RoundsRemaining;
        }

        void StartSilence()
        {
            // Get peered entity gameobject
            DaggerfallEntityBehaviour entityBehaviour = GetPeeredEntityBehaviour(manager);
            if (!entityBehaviour)
                return;

            entityBehaviour.Entity.IsSilenced = true;
            PlayerAggro();

            // Output "You are silenced." if the host manager is player
            if (awakeAlert && manager.EntityBehaviour == GameManager.Instance.PlayerEntityBehaviour)
            {
                DaggerfallUI.AddHUDText(TextManager.Instance.GetText(textDatabase, "youAreSilenced"), 1.5f);
                awakeAlert = false;
            }
        }

        void EndSilence()
        {
            // Get peered entity gameobject
            DaggerfallEntityBehaviour entityBehaviour = GetPeeredEntityBehaviour(manager);
            if (!entityBehaviour)
                return;

            entityBehaviour.Entity.IsSilenced = false;
        }
    }
}
