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
    /// Cure - Disease
    /// </summary>
    public class CureDisease : BaseEntityEffect
    {
        public static readonly string EffectKey = "Cure-Disease";

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.ClassicKey = MakeClassicKey(3, 0);
            properties.GroupName = TextManager.Instance.GetText(textDatabase, "cure");
            properties.SubGroupName = TextManager.Instance.GetText(textDatabase, "disease");
            properties.SpellMakerDescription = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1509);
            properties.SpellBookDescription = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1209);
            properties.SupportChance = true;
            properties.AllowedTargets = EntityEffectBroker.TargetFlags_All;
            properties.AllowedElements = EntityEffectBroker.ElementFlags_MagicOnly;
            properties.AllowedCraftingStations = MagicCraftingStations.SpellMaker | MagicCraftingStations.PotionMaker;
            properties.MagicSkill = DFCareer.MagicSkills.Restoration;
            properties.ChanceCosts = MakeEffectCosts(8, 100);
        }

        public override void SetPotionProperties()
        {
            EffectSettings cureSettings = SetEffectChance(DefaultEffectSettings(), 1, 10, 1);
            PotionRecipe cureDisease = new PotionRecipe(
                TextManager.Instance.GetText(textDatabase, "cureDisease"),
                100,
                cureSettings,
                (int)Items.MiscellaneousIngredients1.Elixir_vitae,
                (int)Items.PlantIngredients2.Fig,
                (int)Items.MiscellaneousIngredients1.Big_tooth);

            EffectSettings purificationSettings = SetEffectChance(DefaultEffectSettings(), 1, 10, 1);
            purificationSettings = SetEffectMagnitude(purificationSettings, 5, 5, 19, 19, 1);
            PotionRecipe purification = new PotionRecipe(
                TextManager.Instance.GetText(textDatabase, "purification"),
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
