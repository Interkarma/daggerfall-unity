// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut
// Contributors:    
// 
// Notes:
//

using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Heal - Magicka
    /// </summary>
    public class HealSpellPoints : BaseEntityEffect
    {
        public static readonly string EffectKey = "Heal-SpellPoints";

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.GroupName = TextManager.Instance.GetText(textDatabase, "heal");
            properties.SubGroupName = TextManager.Instance.GetText(textDatabase, "spellPoints");
            properties.SupportMagnitude = true;
            properties.AllowedTargets = EntityEffectBroker.TargetFlags_Self;
            properties.AllowedElements = EntityEffectBroker.ElementFlags_MagicOnly;
            properties.AllowedCraftingStations = MagicCraftingStations.PotionMaker;
        }

        public override void SetPotionProperties()
        {
            // Magnitude 5-5 + 4-4 per 1 levels
            EffectSettings restorePowerSettings = SetEffectMagnitude(DefaultEffectSettings(), 5, 5, 4, 4, 1);
            PotionRecipe restorePower = new PotionRecipe(
                TextManager.Instance.GetText(textDatabase, "restorePower"),
                75,
                restorePowerSettings,
                (int)Items.MiscellaneousIngredients1.Nectar,
                (int)Items.MetalIngredients.Silver,
                (int)Items.CreatureIngredients1.Werewolfs_blood,
                (int)Items.CreatureIngredients1.Saints_hair);

            // Assign recipe
            restorePower.TextureRecord = 12;
            AssignPotionRecipes(restorePower);
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
            entityBehaviour.Entity.IncreaseMagicka(magnitude);

            UnityEngine.Debug.LogFormat("{0} incremented {1}'s magicka by {2} points", Key, entityBehaviour.EntityType.ToString(), magnitude);
        }
    }
}
