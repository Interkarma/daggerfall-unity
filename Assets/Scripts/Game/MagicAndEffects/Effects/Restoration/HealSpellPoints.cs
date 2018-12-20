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
        public override void SetProperties()
        {
            properties.Key = "Heal-SpellPoints";
            //properties.ClassicKey = MakeClassicKey(10, 9);
            properties.GroupName = TextManager.Instance.GetText(textDatabase, "heal");
            properties.SubGroupName = TextManager.Instance.GetText(textDatabase, "spellPoints");
            //properties.SpellMakerDescription = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1549);
            //properties.SpellBookDescription = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1249);
            properties.SupportMagnitude = true;
            properties.AllowedTargets = EntityEffectBroker.TargetFlags_Self;
            properties.AllowedElements = EntityEffectBroker.ElementFlags_MagicOnly;
            properties.AllowedCraftingStations = MagicCraftingStations.PotionMaker;
            //properties.MagicSkill = DFCareer.MagicSkills.Restoration;
            //properties.MagnitudeCosts = MakeEffectCosts(8, 28);
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
