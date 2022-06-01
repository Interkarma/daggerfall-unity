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
    /// Chameleon - Normal
    /// </summary>
    public class ChameleonNormal : ConcealmentEffect
    {
        public static readonly string EffectKey = "Chameleon-Normal";

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.ClassicKey = MakeClassicKey(23, 0);
            properties.SupportDuration = true;
            properties.AllowedTargets = EntityEffectBroker.TargetFlags_All;
            properties.AllowedElements = EntityEffectBroker.ElementFlags_MagicOnly;
            properties.AllowedCraftingStations = MagicCraftingStations.SpellMaker | MagicCraftingStations.PotionMaker;
            properties.MagicSkill = DFCareer.MagicSkills.Illusion;
            properties.DurationCosts = MakeEffectCosts(20, 80);
            concealmentFlag = MagicalConcealmentFlags.BlendingNormal;
            startConcealmentMessageKey = "youAreBlending";
        }

        public override string GroupName => TextManager.Instance.GetLocalizedText("chameleon");
        public override string SubGroupName => TextManager.Instance.GetLocalizedText("normal");
        public override string DisplayName => string.Format("{0} ({1})", GroupName, SubGroupName);
        public override TextFile.Token[] SpellMakerDescription => DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1571);
        public override TextFile.Token[] SpellBookDescription => DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1271);

        public override void SetPotionProperties()
        {
            PotionRecipe chameleonForm = new PotionRecipe(
                "chameleonForm",
                200,
                DefaultEffectSettings(),
                (int)Items.MiscellaneousIngredients1.Rain_water,
                (int)Items.MiscellaneousIngredients1.Nectar,
                (int)Items.PlantIngredients2.Green_leaves,
                (int)Items.PlantIngredients2.Yellow_flowers,
                (int)Items.PlantIngredients2.Red_berries);

            chameleonForm.TextureRecord = 33;
            AssignPotionRecipes(chameleonForm);
        }

        protected override bool IsLikeKind(IncumbentEffect other)
        {
            return (other is ChameleonNormal);
        }
    }
}
