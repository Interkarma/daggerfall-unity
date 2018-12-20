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
    /// Free Action.
    /// </summary>
    public class FreeAction : IncumbentEffect
    {
        public static readonly string EffectKey = "FreeAction";

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.ClassicKey = MakeClassicKey(28, 255);
            properties.GroupName = TextManager.Instance.GetText(textDatabase, "freeAction");
            properties.SubGroupName = string.Empty;
            properties.SpellMakerDescription = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1576);
            properties.SpellBookDescription = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1276);
            properties.SupportDuration = true;
            properties.AllowedTargets = EntityEffectBroker.TargetFlags_All;
            properties.AllowedElements = ElementTypes.Magic;
            properties.AllowedCraftingStations = MagicCraftingStations.SpellMaker | MagicCraftingStations.PotionMaker;
            properties.MagicSkill = DFCareer.MagicSkills.Restoration;
            properties.DurationCosts = MakeEffectCosts(20, 8);
        }

        public override void SetPotionProperties()
        {
            EffectSettings cureSettings = SetEffectChance(DefaultEffectSettings(), 5, 19, 1);
            PotionRecipe freeAction = new PotionRecipe(
                TextManager.Instance.GetText(textDatabase, "freeAction"),
                125,
                cureSettings,
                (int)Items.MiscellaneousIngredients1.Ichor,
                (int)Items.CreatureIngredients1.Spider_venom,
                (int)Items.PlantIngredients1.Twigs,
                (int)Items.PlantIngredients2.Bamboo);

            freeAction.TextureRecord = 14;
            AssignPotionRecipes(freeAction);
        }

        public override void ConstantEffect()
        {
            base.ConstantEffect();
            StartFreeAction();
        }

        public override void Start(EntityEffectManager manager, DaggerfallEntityBehaviour caster = null)
        {
            base.Start(manager, caster);
            StartFreeAction();
        }

        public override void Resume(EntityEffectManager.EffectSaveData_v1 effectData, EntityEffectManager manager, DaggerfallEntityBehaviour caster = null)
        {
            base.Resume(effectData, manager, caster);
            StartFreeAction();
        }

        public override void End()
        {
            base.End();
            StopFreeAction();
        }

        protected override bool IsLikeKind(IncumbentEffect other)
        {
            return (other is FreeAction);
        }

        protected override void AddState(IncumbentEffect incumbent)
        {
            // Stack my rounds onto incumbent
            incumbent.RoundsRemaining += RoundsRemaining;
        }

        void StartFreeAction()
        {
            // Get peered entity gameobject
            DaggerfallEntityBehaviour entityBehaviour = GetPeeredEntityBehaviour(manager);
            if (!entityBehaviour)
                return;

            entityBehaviour.Entity.IsImmuneToParalysis = true;
        }

        void StopFreeAction()
        {
            // Get peered entity gameobject
            DaggerfallEntityBehaviour entityBehaviour = GetPeeredEntityBehaviour(manager);
            if (!entityBehaviour)
                return;

            entityBehaviour.Entity.IsImmuneToParalysis = false;
        }
    }
}
