// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2019 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using System.Collections.Generic;
using DaggerfallConnect.FallExe;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Increases carry weight.
    /// </summary>
    public class IncreasedWeightAllowance : BaseEntityEffect
    {
        public static readonly string EffectKey = EnchantmentTypes.IncreasedWeightAllowance.ToString();

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.GroupName = TextManager.Instance.GetText(textDatabase, EffectKey);
            properties.ShowSpellIcon = false;
            properties.AllowedCraftingStations = MagicCraftingStations.ItemMaker;
            properties.EnchantmentPayloadFlags = EnchantmentPayloadFlags.None; // TEMP: Payload currently handled by PassiveItemSpecialsEffect
        }

        /// <summary>
        /// Outputs all variant settings for this enchantment.
        /// </summary>
        public override EnchantmentSettings[] GetEnchantmentSettings()
        {
            List<EnchantmentSettings> enchantments = new List<EnchantmentSettings>();

            // Enumerate classic params
            for (int i = 0; i < classicParamCosts.Length; i++)
            {
                EnchantmentSettings enchantment = new EnchantmentSettings()
                {
                    Version = 1,
                    EffectKey = EffectKey,
                    ClassicType = EnchantmentTypes.IncreasedWeightAllowance,
                    ClassicParam = (short)i,
                    PrimaryDisplayName = properties.GroupName,
                    SecondaryDisplayName = TextManager.Instance.GetText(textDatabase, classicTextKeys[i]),
                    EnchantCost = classicParamCosts[i],
                };

                enchantments.Add(enchantment);
            }

            return enchantments.ToArray();
        }

        #region Classic Support

        static short[] classicParamCosts =
        {
            400,    //25% additional
            600,    //50% additional
        };

        static string[] classicTextKeys =
        {
            "add25Percent",
            "add50Percent",
        };

        #endregion
    }
}