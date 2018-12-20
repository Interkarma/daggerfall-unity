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
    /// Heal - Health
    /// </summary>
    public class HealHealth : BaseEntityEffect
    {
        public static readonly string EffectKey = "Heal-Health";

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.ClassicKey = MakeClassicKey(10, 8);
            properties.GroupName = TextManager.Instance.GetText(textDatabase, "heal");
            properties.SubGroupName = TextManager.Instance.GetText(textDatabase, "health");
            properties.SpellMakerDescription = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1548);
            properties.SpellBookDescription = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1248);
            properties.SupportMagnitude = true;
            properties.AllowedTargets = EntityEffectBroker.TargetFlags_All;
            properties.AllowedElements = EntityEffectBroker.ElementFlags_MagicOnly;
            properties.AllowedCraftingStations = MagicCraftingStations.SpellMaker | MagicCraftingStations.PotionMaker;
            properties.MagicSkill = DFCareer.MagicSkills.Restoration;
            properties.MagnitudeCosts = MakeEffectCosts(20, 28);
        }

        public override void SetPotionProperties()
        {
            // First recipe variant: Magnitude 5-5 + 9-9 per 1 levels
            EffectSettings healingSettings = SetEffectMagnitude(DefaultEffectSettings(), 5, 5, 9, 9, 1);
            PotionRecipe healing = new PotionRecipe(
                TextManager.Instance.GetText(textDatabase, "healing"),
                50,
                healingSettings,
                (int)Items.MiscellaneousIngredients1.Elixir_vitae,
                (int)Items.PlantIngredients1.Red_berries,
                (int)Items.MetalIngredients.Mercury,
                (int)Items.CreatureIngredients1.Troll_blood);

            // Second recipe variant: Magnitude 5-5 + 19-19 per 1 levels
            EffectSettings healTrueSettings = SetEffectMagnitude(DefaultEffectSettings(), 5, 5, 19, 19, 1);
            PotionRecipe healTrue = new PotionRecipe(
                TextManager.Instance.GetText(textDatabase, "healTrue"),
                100,
                healTrueSettings,
                (int)Items.MiscellaneousIngredients1.Elixir_vitae,
                (int)Items.PlantIngredients1.Red_berries,
                (int)Items.PlantIngredients1.Pine_branch,
                (int)Items.CreatureIngredients3.Unicorn_horn);

            // Assign recipes
            healing.TextureRecord = 15;
            healTrue.TextureRecord = 16;
            AssignPotionRecipes(healing, healTrue);
        }

        public override void MagicRound()
        {
            base.MagicRound();

            // Get peered entity gameobject
            DaggerfallEntityBehaviour entityBehaviour = GetPeeredEntityBehaviour(manager);
            if (!entityBehaviour)
                return;

            // Implement effect
            int magnitude = GetMagnitude(caster);
            entityBehaviour.Entity.IncreaseHealth(magnitude);

            UnityEngine.Debug.LogFormat("{0} incremented {1}'s health by {2} points", Key, entityBehaviour.EntityType.ToString(), magnitude);
        }
    }
}
