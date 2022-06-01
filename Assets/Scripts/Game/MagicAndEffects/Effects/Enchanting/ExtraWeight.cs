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
    /// Item weight is increased by multiple times.
    /// </summary>
    public class ExtraWeight : BaseEntityEffect
    {
        public static readonly string EffectKey = EnchantmentTypes.ExtraWeight.ToString();

        const int enchantCost = -100;
        const float weightMultiplier = 4;

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
                ClassicType = EnchantmentTypes.ExtraWeight,
                ClassicParam = -1,
                PrimaryDisplayName = GroupName,
                EnchantCost = enchantCost,
            };

            return enchantments;
        }

        public override PayloadCallbackResults? EnchantmentPayloadCallback(EnchantmentPayloadFlags context, EnchantmentParam? param = null, DaggerfallEntityBehaviour sourceEntity = null, DaggerfallEntityBehaviour targetEntity = null, DaggerfallUnityItem sourceItem = null, int sourceDamage = 0)
        {
            base.EnchantmentPayloadCallback(context, param, sourceEntity, targetEntity, sourceItem);

            // Increase weight when item is enchanted
            if (context == EnchantmentPayloadFlags.Enchanted && sourceItem != null)
                sourceItem.weightInKg *= weightMultiplier;

            return null;
        }

        public override bool IsEnchantmentExclusiveTo(EnchantmentSettings[] settingsToTest, EnchantmentParam? comparerParam = null)
        {
            // Exclusive to FeatherWeight - no param test required
            string featherWeightKey = EnchantmentTypes.FeatherWeight.ToString();
            foreach (EnchantmentSettings settings in settingsToTest)
            {
                if (settings.EffectKey == featherWeightKey)
                    return true;
            }

            return false;
        }
    }
}