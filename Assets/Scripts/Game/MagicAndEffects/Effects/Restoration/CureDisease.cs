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
        public override void SetProperties()
        {
            properties.Key = "Cure-Disease";
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
            PotionRecipe cureDisease = new PotionRecipe(
                TextManager.Instance.GetText(textDatabase, "cureDisease"),
                100,
                DefaultEffectSettings(),
                (int)Items.MiscellaneousIngredients1.Elixir_vitae,
                (int)Items.PlantIngredients2.Fig,
                (int)Items.MiscellaneousIngredients1.Big_tooth);

            AssignPotionRecipes(cureDisease);
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

            //Debug.LogFormat("Cured entity of all diseases");
        }
    }
}
