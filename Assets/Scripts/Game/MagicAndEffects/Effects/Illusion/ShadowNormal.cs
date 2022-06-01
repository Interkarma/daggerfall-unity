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
    /// Shadow - Normal
    /// </summary>
    public class ShadowNormal : ConcealmentEffect
    {
        public static readonly string EffectKey = "Shadow-Normal";

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.ClassicKey = MakeClassicKey(24, 0);
            properties.SupportDuration = true;
            properties.AllowedTargets = EntityEffectBroker.TargetFlags_All;
            properties.AllowedElements = EntityEffectBroker.ElementFlags_MagicOnly;
            properties.AllowedCraftingStations = MagicCraftingStations.SpellMaker | MagicCraftingStations.PotionMaker;
            properties.MagicSkill = DFCareer.MagicSkills.Illusion;
            properties.DurationCosts = MakeEffectCosts(20, 80);
            concealmentFlag = MagicalConcealmentFlags.ShadeNormal;
            startConcealmentMessageKey = "youAreAShade";
        }

        public override string GroupName => TextManager.Instance.GetLocalizedText("shadow");
        public override string SubGroupName => TextManager.Instance.GetLocalizedText("normal");
        public override string DisplayName => string.Format("{0} ({1})", GroupName, SubGroupName);
        public override TextFile.Token[] SpellMakerDescription => DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1573);
        public override TextFile.Token[] SpellBookDescription => DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1273);

        public override void SetPotionProperties()
        {
            PotionRecipe shadowForm = new PotionRecipe(
                "shadowForm",
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
