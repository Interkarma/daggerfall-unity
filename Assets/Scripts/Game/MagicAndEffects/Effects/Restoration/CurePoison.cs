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
    /// Cure - Poison
    /// </summary>
    public class CurePoison : BaseEntityEffect
    {
        public static readonly string EffectKey = "Cure-Poison";

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.ClassicKey = MakeClassicKey(3, 1);
            properties.GroupName = TextManager.Instance.GetText(textDatabase, "cure");
            properties.SubGroupName = TextManager.Instance.GetText(textDatabase, "poison");
            properties.SpellMakerDescription = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1510);
            properties.SpellBookDescription = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1210);
            properties.SupportChance = true;
            properties.AllowedTargets = EntityEffectBroker.TargetFlags_All;
            properties.AllowedElements = EntityEffectBroker.ElementFlags_MagicOnly;
            properties.AllowedCraftingStations = MagicCraftingStations.SpellMaker | MagicCraftingStations.PotionMaker;
            properties.MagicSkill = DFCareer.MagicSkills.Restoration;
            properties.ChanceCosts = MakeEffectCosts(8, 100);
        }

        public override void SetPotionProperties()
        {
            EffectSettings cureSettings = SetEffectChance(DefaultEffectSettings(), 5, 19, 1);
            PotionRecipe curePoison = new PotionRecipe(
                TextManager.Instance.GetText(textDatabase, "curePoison"),
                200,
                cureSettings,
                (int)Items.MiscellaneousIngredients1.Ichor,
                (int)Items.CreatureIngredients2.Giant_scorpion_stinger,
                (int)Items.MiscellaneousIngredients1.Small_tooth,
                (int)Items.MiscellaneousIngredients2.Pearl);

            curePoison.TextureRecord = 35;
            AssignPotionRecipes(curePoison);
        }

        public override void MagicRound()
        {
            base.MagicRound();

            // Get peered entity gameobject
            DaggerfallEntityBehaviour entityBehaviour = GetPeeredEntityBehaviour(manager);
            if (!entityBehaviour)
                return;

            // Implement effect
            manager.CureAllPoisons();

            //Debug.LogFormat("Cured entity of all poisons");
        }
    }
}
