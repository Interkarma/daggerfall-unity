// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes: All additions or modifications that differ from the source code copyright (c) 2021-2022 Osorkon
//

using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Dispel - Magic
    /// </summary>
    public class DispelMagic : BaseEntityEffect
    {
        public static readonly string EffectKey = "Dispel-Magic";

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.ClassicKey = MakeClassicKey(6, 0);
            properties.SupportChance = true;
            properties.AllowedTargets = EntityEffectBroker.TargetFlags_Self;
            properties.AllowedElements = EntityEffectBroker.ElementFlags_MagicOnly;

            // [OSORKON] I added PotionMaker to the allowed crafting stations to support the new Dispel Magic potion.
            properties.AllowedCraftingStations = MagicCraftingStations.SpellMaker | MagicCraftingStations.PotionMaker;
            properties.MagicSkill = DFCareer.MagicSkills.Mysticism;
            properties.ChanceCosts = MakeEffectCosts(120, 180);
        }

        public override string GroupName => TextManager.Instance.GetLocalizedText("dispel");
        public override string SubGroupName => TextManager.Instance.GetLocalizedText("magic");
        public override TextFile.Token[] SpellMakerDescription => DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1516);
        public override TextFile.Token[] SpellBookDescription => DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1216);

        // [OSORKON] I followed other potion templates when I wrote the Dispel Magic potion code.
        public override void SetPotionProperties()
        {
            PotionRecipe dispelMagic = new PotionRecipe(
                "dispelMagic",
                150,

                // [OSORKON] I put this in because other potion code had it. Good thing I did - it must set the
                // effect's success rate to 100%, as I never had a Dispel Magic potion fail to work. By accident,
                // I got the result I wanted.
                DefaultEffectSettings(),

                // [OSORKON] I looked at UESP's list of ingredients to find any with inherent Dispel Magic effects.
                // Pure Water doesn't have that effect but I needed a liquid potion base (to match other potion
                // recipes) and it sounded cleansing. 
                (int)Items.MiscellaneousIngredients1.Pure_water,
                (int)Items.CreatureIngredients1.Werewolfs_blood,
                (int)Items.MetalIngredients.Silver,
                (int)Items.MiscellaneousIngredients1.Holy_relic);

            dispelMagic.TextureRecord = 33;
            AssignPotionRecipes(dispelMagic);
        }

        public override void MagicRound()
        {
            base.MagicRound();

            // Clear all spell bundles on target - including this one
            manager.ClearSpellBundles();
        }
    }
}
