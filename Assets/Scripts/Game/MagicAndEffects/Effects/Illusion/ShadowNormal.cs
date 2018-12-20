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
    /// Shadow - Normal
    /// </summary>
    public class ShadowNormal : ConcealmentEffect
    {
        public static readonly string EffectKey = "Shadow-Normal";

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.ClassicKey = MakeClassicKey(24, 0);
            properties.GroupName = TextManager.Instance.GetText(textDatabase, "shadow");
            properties.SubGroupName = TextManager.Instance.GetText(textDatabase, "normal");
            properties.DisplayName = string.Format("{0} ({1})", properties.GroupName, properties.SubGroupName);
            properties.SpellMakerDescription = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1573);
            properties.SpellBookDescription = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1273);
            properties.SupportDuration = true;
            properties.AllowedTargets = EntityEffectBroker.TargetFlags_All;
            properties.AllowedElements = EntityEffectBroker.ElementFlags_MagicOnly;
            properties.AllowedCraftingStations = MagicCraftingStations.SpellMaker | MagicCraftingStations.PotionMaker;
            properties.MagicSkill = DFCareer.MagicSkills.Illusion;
            properties.DurationCosts = MakeEffectCosts(20, 80);
            concealmentFlag = MagicalConcealmentFlags.ShadeNormal;
            startConcealmentMessageKey = "youAreAShade";
        }

        public override void SetPotionProperties()
        {
            PotionRecipe shadowForm = new PotionRecipe(
                TextManager.Instance.GetText(textDatabase, "shadowForm"),
                200,
                DefaultEffectSettings(),
                (int)Items.MiscellaneousIngredients1.Rain_water,
                (int)Items.MiscellaneousIngredients1.Nectar,
                (int)Items.Gems.Malachite,
                (int)Items.PlantIngredients2.Black_rose);

            shadowForm.TextureRecord = 33;
            AssignPotionRecipes(shadowForm);
        }

        protected override bool IsLikeKind(IncumbentEffect other)
        {
            return (other is ShadowNormal);
        }
    }
}
