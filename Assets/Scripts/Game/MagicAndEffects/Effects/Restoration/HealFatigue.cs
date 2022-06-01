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
    /// Heal - Fatigue
    /// </summary>
    public class HealFatigue : BaseEntityEffect
    {
        public static readonly string EffectKey = "Heal-Fatigue";

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.ClassicKey = MakeClassicKey(10, 9);
            properties.SupportMagnitude = true;
            properties.AllowedTargets = EntityEffectBroker.TargetFlags_All;
            properties.AllowedElements = EntityEffectBroker.ElementFlags_MagicOnly;
            properties.AllowedCraftingStations = MagicCraftingStations.SpellMaker | MagicCraftingStations.PotionMaker;
            properties.MagicSkill = DFCareer.MagicSkills.Restoration;
            properties.MagnitudeCosts = MakeEffectCosts(8, 28);
        }

        public override string GroupName => TextManager.Instance.GetLocalizedText("heal");
        public override string SubGroupName => TextManager.Instance.GetLocalizedText("fatigue");
        public override TextFile.Token[] SpellMakerDescription => DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1549);
        public override TextFile.Token[] SpellBookDescription => DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1249);

        public override void SetPotionProperties()
        {
            // Magnitude 5-5 + 4-4 per 1 levels
            EffectSettings staminaSettings = SetEffectMagnitude(DefaultEffectSettings(), 5, 5, 4, 4, 1);
            PotionRecipe stamina = new PotionRecipe(
                "stamina",
                25,
                staminaSettings,
                (int)Items.MiscellaneousIngredients1.Pure_water,
                (int)Items.PlantIngredients2.Aloe,
                (int)Items.PlantIngredients2.Ginkgo_leaves);

            // Assign recipe
            AssignPotionRecipes(stamina);
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
            entityBehaviour.Entity.IncreaseFatigue(magnitude, true);

            //Debug.LogFormat("{0} incremented {1}'s fatigue by {2} points", Key, entityBehaviour.EntityType.ToString(), magnitude);
        }
    }
}
