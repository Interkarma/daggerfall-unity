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

using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Cure - Disease
    /// </summary>
    public class CureDisease : BaseEntityEffect
    {
        public static readonly string EffectKey = "Cure-Disease";

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.ClassicKey = MakeClassicKey(3, 0);
            properties.SupportChance = true;
            properties.AllowedTargets = EntityEffectBroker.TargetFlags_All;
            properties.AllowedElements = EntityEffectBroker.ElementFlags_MagicOnly;
            properties.AllowedCraftingStations = MagicCraftingStations.SpellMaker | MagicCraftingStations.PotionMaker;
            properties.MagicSkill = DFCareer.MagicSkills.Restoration;
            properties.ChanceCosts = MakeEffectCosts(8, 100);
        }

        public override string GroupName => TextManager.Instance.GetLocalizedText("cure");
        public override string SubGroupName => TextManager.Instance.GetLocalizedText("disease");
        public override TextFile.Token[] SpellMakerDescription => DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1509);
        public override TextFile.Token[] SpellBookDescription => DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1209);

        public override void SetPotionProperties()
        {
            EffectSettings cureSettings = SetEffectChance(DefaultEffectSettings(), 1, 10, 1);
            PotionRecipe cureDisease = new PotionRecipe(
                "cureDisease",
                100,
                cureSettings,
                (int)Items.MiscellaneousIngredients1.Elixir_vitae,
                (int)Items.PlantIngredients2.Fig,
                (int)Items.MiscellaneousIngredients1.Big_tooth);

            EffectSettings purificationSettings = SetEffectChance(DefaultEffectSettings(), 1, 10, 1);
            purificationSettings = SetEffectMagnitude(purificationSettings, 5, 5, 19, 19, 1);
            PotionRecipe purification = new PotionRecipe(
                "purification",
                500,
                purificationSettings,
                (int)Items.MiscellaneousIngredients1.Elixir_vitae,
                (int)Items.MiscellaneousIngredients1.Nectar,
                (int)Items.MiscellaneousIngredients1.Rain_water,
                (int)Items.PlantIngredients2.Fig,
                (int)Items.MiscellaneousIngredients1.Big_tooth,
                (int)Items.CreatureIngredients1.Ectoplasm,
                (int)Items.Gems.Diamond,
                (int)Items.CreatureIngredients2.Mummy_wrappings);
            purification.AddSecondaryEffect(HealHealth.EffectKey);
            purification.AddSecondaryEffect(InvisibilityNormal.EffectKey);

            cureDisease.TextureRecord = 35;
            purification.TextureRecord = 35;
            AssignPotionRecipes(cureDisease, purification);
        }

        public override void MagicRound()
        {
            base.MagicRound();

            // Get peered entity gameobject
            DaggerfallEntityBehaviour entityBehaviour = GetPeeredEntityBehaviour(manager);
            if (!entityBehaviour)
                return;

            // Implement effect
            manager.CureAllDiseases();

            UnityEngine.Debug.LogFormat("Cured entity of all diseases");
        }
    }
}
