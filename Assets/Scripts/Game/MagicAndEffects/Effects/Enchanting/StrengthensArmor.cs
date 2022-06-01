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

using DaggerfallConnect.FallExe;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Adds +5 to armour class in every armor slot. Does not stack with other items of this effect.
    /// </summary>
    public class StrengthensArmor : BaseEntityEffect
    {
        public static readonly string EffectKey = EnchantmentTypes.StrengthensArmor.ToString();

        const int enchantCost = 700;
        const int increaseArmorValue = -5;          // Lower armor value equals a stronger armor rating

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.ShowSpellIcon = false;
            properties.AllowedCraftingStations = MagicCraftingStations.ItemMaker;
            properties.ItemMakerFlags = ItemMakerFlags.None;
            properties.EnchantmentPayloadFlags = EnchantmentPayloadFlags.Held;
        }

        public override string GroupName => TextManager.Instance.GetLocalizedText(EffectKey);

        public override EnchantmentSettings[] GetEnchantmentSettings()
        {
            EnchantmentSettings[] enchantments = new EnchantmentSettings[1];
            enchantments[0] = new EnchantmentSettings()
            {
                Version = 1,
                EffectKey = EffectKey,
                ClassicType = EnchantmentTypes.StrengthensArmor,
                ClassicParam = -1,
                PrimaryDisplayName = GroupName,
                EnchantCost = enchantCost,
            };

            return enchantments;
        }

        public override void ConstantEffect()
        {
            base.ConstantEffect();

            // Get peered entity gameobject
            DaggerfallEntityBehaviour entityBehaviour = GetPeeredEntityBehaviour(manager);
            if (!entityBehaviour)
                return;

            entityBehaviour.Entity.SetIncreasedArmorValueModifier(increaseArmorValue);
        }

        public override bool IsEnchantmentExclusiveTo(EnchantmentSettings[] settingsToTest, EnchantmentParam? comparerParam = null)
        {
            // Exclusive to WeakensArmor - no param test required
            string weakensArmorKey = EnchantmentTypes.WeakensArmor.ToString();
            foreach (EnchantmentSettings settings in settingsToTest)
            {
                if (settings.EffectKey == weakensArmorKey)
                    return true;
            }

            return false;
        }
    }
}