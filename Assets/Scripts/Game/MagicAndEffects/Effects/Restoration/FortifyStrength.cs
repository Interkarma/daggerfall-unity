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

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Fortify Attribute - Strength
    /// </summary>
    public class FortifyStrength : FortifyEffect
    {
        public static readonly string EffectKey = "Fortify-Strength";

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.ClassicKey = MakeClassicKey(9, 0);
            properties.GroupName = TextManager.Instance.GetText(textDatabase, "fortifyAttribute");
            properties.SubGroupName = TextManager.Instance.GetText(textDatabase, "strength");
            properties.SpellMakerDescription = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1532);
            properties.SpellBookDescription = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1232);
            properties.SupportDuration = true;
            properties.SupportMagnitude = true;
            properties.AllowedTargets = EntityEffectBroker.TargetFlags_All;
            properties.AllowedElements = EntityEffectBroker.ElementFlags_MagicOnly;
            properties.AllowedCraftingStations = MagicCraftingStations.SpellMaker | MagicCraftingStations.PotionMaker;
            properties.MagicSkill = DFCareer.MagicSkills.Restoration;
            properties.DurationCosts = MakeEffectCosts(28, 100);
            properties.MagnitudeCosts = MakeEffectCosts(40, 120);
            fortifyStat = DFCareer.Stats.Strength;
        }

        public override void SetPotionProperties()
        {
            // Magnitude 1-1 + 14-14 per 1 levels
            EffectSettings orcStrengthSettings = SetEffectMagnitude(DefaultEffectSettings(), 1, 1, 14, 14, 1);
            PotionRecipe orcStrength = new PotionRecipe(
                TextManager.Instance.GetText(textDatabase, "orcStrength"),
                50,
                orcStrengthSettings,
                (int)Items.CreatureIngredients1.Orcs_blood,
                (int)Items.MetalIngredients.Iron,
                (int)Items.MiscellaneousIngredients1.Pure_water);

            // Assign recipe
            orcStrength.TextureRecord = 13;
            AssignPotionRecipes(orcStrength);
        }
    }
}
