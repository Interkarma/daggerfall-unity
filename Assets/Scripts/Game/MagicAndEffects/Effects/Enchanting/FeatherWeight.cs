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
using DaggerfallWorkshop.Game.Items;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Item weight is reduced to a low fixed value regardless of original weight.
    /// </summary>
    public class FeatherWeight : BaseEntityEffect
    {
        public static readonly string EffectKey = EnchantmentTypes.FeatherWeight.ToString();

        const int enchantCost = 100;
        const float weightValue = 0.25f;

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.ShowSpellIcon = false;
            properties.AllowedCraftingStations = MagicCraftingStations.ItemMaker;
            properties.ItemMakerFlags = ItemMakerFlags.None;
            properties.EnchantmentPayloadFlags = EnchantmentPayloadFlags.Enchanted;
        }

        public override string GroupName => TextManager.Instance.GetLocalizedText(EffectKey);

        public override EnchantmentSettings[] GetEnchantmentSettings()
        {
            EnchantmentSettings[] enchantments = new EnchantmentSettings[1];
            enchantments[0] = new EnchantmentSettings()
            {
                Version = 1,
                EffectKey = EffectKey,
                ClassicType = EnchantmentTypes.FeatherWeight,
                ClassicParam = -1,
                PrimaryDisplayName = GroupName,
                EnchantCost = enchantCost,
            };

            return enchantments;
        }

        public override PayloadCallbackResults? EnchantmentPayloadCallback(EnchantmentPayloadFlags context, EnchantmentParam? param = null, DaggerfallEntityBehaviour sourceEntity = null, DaggerfallEntityBehaviour targetEntity = null, DaggerfallUnityItem sourceItem = null, int sourceDamage = 0)
        {
            base.EnchantmentPayloadCallback(context, param, sourceEntity, targetEntity, sourceItem);

            // Lower weight when item is enchanted
            if (context == EnchantmentPayloadFlags.Enchanted && sourceItem != null)
                sourceItem.weightInKg = weightValue;

            return null;
        }

        public override bool IsEnchantmentExclusiveTo(EnchantmentSettings[] settingsToTest, EnchantmentParam? comparerParam = null)
        {
            // Exclusive to ExtraWeight - no param test required
            string extraWeightKey = EnchantmentTypes.ExtraWeight.ToString();
            foreach (EnchantmentSettings settings in settingsToTest)
            {
                if (settings.EffectKey == extraWeightKey)
                    return true;
            }

            return false;
        }
    }
}