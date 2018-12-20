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
    /// Slowfall
    /// </summary>
    public class Slowfall : IncumbentEffect
    {
        public static readonly string EffectKey = "Slowfall";

        bool awakeAlert = true;

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.ClassicKey = MakeClassicKey(25, 255);
            properties.GroupName = TextManager.Instance.GetText(textDatabase, "slowfall");
            properties.SubGroupName = string.Empty;
            properties.SpellMakerDescription = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1575);
            properties.SpellBookDescription = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1275);
            properties.SupportDuration = true;
            properties.AllowedTargets = TargetTypes.CasterOnly;
            properties.AllowedElements = ElementTypes.Magic;
            properties.AllowedCraftingStations = MagicCraftingStations.SpellMaker | MagicCraftingStations.PotionMaker;
            properties.MagicSkill = DFCareer.MagicSkills.Alteration;
            properties.DurationCosts = MakeEffectCosts(20, 100);
        }

        public override void SetPotionProperties()
        {
            PotionRecipe slowFalling = new PotionRecipe(
                TextManager.Instance.GetText(textDatabase, "slowFalling"),
                100,
                DefaultEffectSettings(),
                (int)Items.MiscellaneousIngredients1.Pure_water,
                (int)Items.PlantIngredients2.White_poppy,
                (int)Items.PlantIngredients2.Black_poppy);

            AssignPotionRecipes(slowFalling);
        }

        public override void ConstantEffect()
        {
            base.ConstantEffect();
            StartSlowfalling();
        }

        public override void Resume(EntityEffectManager.EffectSaveData_v1 effectData, EntityEffectManager manager, DaggerfallEntityBehaviour caster = null)
        {
            base.Resume(effectData, manager, caster);
            StartSlowfalling();
        }

        public override void End()
        {
            base.End();
            StopSlowfalling();
        }

        protected override bool IsLikeKind(IncumbentEffect other)
        {
            return (other is Slowfall);
        }

        protected override void AddState(IncumbentEffect incumbent)
        {
            // Stack my rounds onto incumbent
            incumbent.RoundsRemaining += RoundsRemaining;
        }

        void StartSlowfalling()
        {
            // Get peered entity gameobject
            DaggerfallEntityBehaviour entityBehaviour = GetPeeredEntityBehaviour(manager);
            if (!entityBehaviour)
                return;

            entityBehaviour.Entity.IsSlowFalling = true;

            // Output "Slow fall active." if the host manager is player
            if (awakeAlert && manager.EntityBehaviour == GameManager.Instance.PlayerEntityBehaviour)
            {
                DaggerfallUI.AddHUDText(TextManager.Instance.GetText(textDatabase, "slowFallActive"), 1.5f);
                awakeAlert = false;
            }
        }

        void StopSlowfalling()
        {
            // Get peered entity gameobject
            DaggerfallEntityBehaviour entityBehaviour = GetPeeredEntityBehaviour(manager);
            if (!entityBehaviour)
                return;

            entityBehaviour.Entity.IsSlowFalling = false;
        }
    }
}
