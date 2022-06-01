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

using System.Collections.Generic;
using DaggerfallConnect.FallExe;
using DaggerfallWorkshop.Game.Entity;

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
            properties.ShowSpellIcon = false;
            properties.AllowedCraftingStations = MagicCraftingStations.ItemMaker;
            properties.EnchantmentPayloadFlags = EnchantmentPayloadFlags.Held;
        }

        public override string GroupName => TextManager.Instance.GetLocalizedText(EffectKey);

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
                    PrimaryDisplayName = GroupName,
                    SecondaryDisplayName = TextManager.Instance.GetLocalizedText(classicTextKeys[i]),
                    EnchantCost = classicParamCosts[i],
                };

                enchantments.Add(enchantment);
            }

            return enchantments.ToArray();
        }

        #region Payloads

        public override void ConstantEffect()
        {
            base.ConstantEffect();

            // Must have a param
            if (EnchantmentParam == null)
                return;

            // Get peered entity gameobject
            DaggerfallEntityBehaviour entityBehaviour = GetPeeredEntityBehaviour(manager);
            if (!entityBehaviour)
                return;

            // Increase weight allowance
            Params type = (Params)EnchantmentParam.Value.ClassicParam;
            switch (type)
            {
                case Params.OneQuarterExtra:
                    entityBehaviour.Entity.SetIncreasedWeightAllowanceMultiplier(0.25f);
                    break;
                case Params.OneHalfExtra:
                    entityBehaviour.Entity.SetIncreasedWeightAllowanceMultiplier(0.5f);
                    break;
            }
        }

        #endregion

        #region Classic Support

        enum Params
        {
            OneQuarterExtra = 0,
            OneHalfExtra = 1,
        }

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