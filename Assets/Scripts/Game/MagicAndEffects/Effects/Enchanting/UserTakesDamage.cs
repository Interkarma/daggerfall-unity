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
    /// User takes damage while item held.
    /// Works like vampire damage from sunlight and holy places but damage is much less (1 HP per 4 minutes).
    /// Classic allows multiples of each side-effect with stacking penalties.
    /// </summary>
    public class UserTakesDamage : BaseEntityEffect
    {
        public static readonly string EffectKey = EnchantmentTypes.UserTakesDamage.ToString();

        const int damagePerRounds = 4;
        const int damageAmount = 1;

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
                    ClassicType = EnchantmentTypes.UserTakesDamage,
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

            // Get peered entity gameobject
            DaggerfallEntityBehaviour entityBehaviour = GetPeeredEntityBehaviour(manager);
            if (!entityBehaviour)
                return;

            // Only triggers once per damagePerRounds
            if (GameManager.Instance.EntityEffectBroker.MagicRoundsSinceStartup % damagePerRounds != 0)
                return;

            // Apply damage
            Params param = (Params)EnchantmentParam.Value.ClassicParam;
            if (param == Params.InSunlight && GameManager.Instance.PlayerEnterExit.IsPlayerInSunlight ||
                param == Params.InHolyPlaces && GameManager.Instance.PlayerEnterExit.IsPlayerInHolyPlace)
                entityBehaviour.Entity.DecreaseHealth(damageAmount);

            //if (ParentBundle.fromEquippedItem != null)
            //    UnityEngine.Debug.LogFormat("Applied {0} points of sun/holy damage from item {1}", damageAmount, ParentBundle.fromEquippedItem.LongName);
        }

        #endregion

        #region Classic Support

        public enum Params
        {
            InSunlight = 0,
            InHolyPlaces = 1,
        }

        static short[] classicParamCosts =
        {
            -6000,  //in sunlight
            -1000,  //in holy places
        };

        static string[] classicTextKeys =
        {
            "inSunlight",
            "inHolyPlaces",
        };

        #endregion
    }
}