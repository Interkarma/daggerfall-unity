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
    /// Water Breathing.
    /// </summary>
    public class WaterBreathing : IncumbentEffect
    {
        public static readonly string EffectKey = "WaterBreathing";

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.ClassicKey = MakeClassicKey(30, 255);
            properties.GroupName = TextManager.Instance.GetText(textDatabase, "waterBreathing");
            properties.SubGroupName = string.Empty;
            properties.SpellMakerDescription = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1582);
            properties.SpellBookDescription = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1282);
            properties.SupportDuration = true;
            properties.AllowedTargets = EntityEffectBroker.TargetFlags_All;
            properties.AllowedElements = ElementTypes.Magic;
            properties.AllowedCraftingStations = MagicCraftingStations.SpellMaker | MagicCraftingStations.PotionMaker;
            properties.MagicSkill = DFCareer.MagicSkills.Alteration;
            properties.DurationCosts = MakeEffectCosts(20, 8);
        }

        public override void SetPotionProperties()
        {
            PotionRecipe waterBreathing = new PotionRecipe(
                TextManager.Instance.GetText(textDatabase, "waterBreathing"),
                100,
                DefaultEffectSettings(),
                (int)Items.MiscellaneousIngredients1.Rain_water,
                (int)Items.MiscellaneousIngredients1.Elixir_vitae,
                (int)Items.MiscellaneousIngredients2.Ivory);

            waterBreathing.TextureRecord = 32;
            AssignPotionRecipes(waterBreathing);
        }

        public override void ConstantEffect()
        {
            base.ConstantEffect();
            StartWaterBreathing();
        }

        public override void Start(EntityEffectManager manager, DaggerfallEntityBehaviour caster = null)
        {
            base.Start(manager, caster);
            StartWaterBreathing();
        }

        public override void Resume(EntityEffectManager.EffectSaveData_v1 effectData, EntityEffectManager manager, DaggerfallEntityBehaviour caster = null)
        {
            base.Resume(effectData, manager, caster);
            StartWaterBreathing();
        }

        public override void End()
        {
            base.End();
            StopWaterBreathing();
        }

        protected override bool IsLikeKind(IncumbentEffect other)
        {
            return (other is WaterBreathing);
        }

        protected override void AddState(IncumbentEffect incumbent)
        {
            // Stack my rounds onto incumbent
            incumbent.RoundsRemaining += RoundsRemaining;
        }

        void StartWaterBreathing()
        {
            // Get peered entity gameobject
            DaggerfallEntityBehaviour entityBehaviour = GetPeeredEntityBehaviour(manager);
            if (!entityBehaviour)
                return;

            entityBehaviour.Entity.IsWaterBreathing = true;
        }

        void StopWaterBreathing()
        {
            // Get peered entity gameobject
            DaggerfallEntityBehaviour entityBehaviour = GetPeeredEntityBehaviour(manager);
            if (!entityBehaviour)
                return;

            entityBehaviour.Entity.IsWaterBreathing = false;
        }
    }
}
