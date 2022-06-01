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
using DaggerfallWorkshop.Game.Items;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Item quality degrades by around one condition level per day.
    /// Only equipped items lose condition, they do not degrade just sitting in inventory.
    /// Item does not degrade while fast travelling, possibly for balance reasons.
    /// Classic allows multiples of each side-effect with stacking penalties.
    /// </summary>
    public class ItemDeteriorates : BaseEntityEffect
    {
        public static readonly string EffectKey = EnchantmentTypes.ItemDeteriorates.ToString();

        const int conditionLossPerRounds = 4;
        const int conditionLossAmount = 1;

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.ShowSpellIcon = false;
            properties.AllowedCraftingStations = MagicCraftingStations.ItemMaker;
            properties.ItemMakerFlags = ItemMakerFlags.AllowMultiplePrimaryInstances | ItemMakerFlags.AllowMultipleSecondaryInstances;
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
                    ClassicType = EnchantmentTypes.ItemDeteriorates,
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

        public override void MagicRound()
        {
            base.MagicRound();

            // Do nothing if item not set, param not set, or during synthetic time increase
            if (ParentBundle.fromEquippedItem == null ||
                EnchantmentParam == null ||
                GameManager.Instance.EntityEffectBroker.SyntheticTimeIncrease)
                return;

            // Only triggers once per conditionLossPerRounds
            if (GameManager.Instance.EntityEffectBroker.MagicRoundsSinceStartup % conditionLossPerRounds != 0)
                return;

            // Validate conditional params
            Params param = (Params)EnchantmentParam.Value.ClassicParam;
            if (param == Params.InSunlight && !GameManager.Instance.PlayerEnterExit.IsPlayerInSunlight ||
                param == Params.InHolyPlaces && !GameManager.Instance.PlayerEnterExit.IsPlayerInHolyPlace)
                return;

            // Get owner entity
            DaggerfallEntity ownerEntity = null;
            ItemCollection ownerItemCollection = null;
            DaggerfallEntityBehaviour entityBehaviour = GetPeeredEntityBehaviour(manager);
            if (entityBehaviour)
            {
                ownerEntity = entityBehaviour.Entity;
                ownerItemCollection = entityBehaviour.Entity.Items;
            }

            // Lower condition of this item - gone forever once it breaks
            ParentBundle.fromEquippedItem.LowerCondition(conditionLossAmount, ownerEntity, ownerItemCollection);

            //UnityEngine.Debug.LogFormat("Item {0} lost {1} points of condition", ParentBundle.fromEquippedItem.LongName, conditionLossAmount);
        }

        #endregion

        #region Classic Support

        public enum Params
        {
            AllTheTime = 0,
            InSunlight = 1,
            InHolyPlaces = 2,
        }

        static short[] classicParamCosts =
        {
            -3000,  //all the time
            -1500,  //in sunlight
            -500,   //in holy places
        };

        static string[] classicTextKeys =
        {
            "allTheTime",
            "inSunlight",
            "inHolyPlaces",
        };

        #endregion
    }
}