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
    /// Lowers chance to hit and armour class when the specific enemy type is closely nearby.
    /// </summary>
    public class BadReactionsFrom : BaseEntityEffect
    {
        public static readonly string EffectKey = EnchantmentTypes.BadReactionsFrom.ToString();

        const float nearbyRange = 8;
        const int decreaseArmorValue = -5;
        const int decreaseHitChanceValue = -5;

        bool isAffected;

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.ShowSpellIcon = false;
            properties.AllowedCraftingStations = MagicCraftingStations.ItemMaker;
            properties.ItemMakerFlags = ItemMakerFlags.AllowMultiplePrimaryInstances;
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
                    ClassicType = EnchantmentTypes.BadReactionsFrom,
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

            // Get peered entity gameobject
            DaggerfallEntityBehaviour entityBehaviour = GetPeeredEntityBehaviour(manager);
            if (!entityBehaviour)
                return;

            // Apply penalty
            // Penalty to armour class matches "weakens armor" enchantment value
            // Penalty to hit is like a negative "adrenaline rush" value
            // While actual values unknown these are reasonably in line with similar effects
            if (isAffected)
            {
                entityBehaviour.Entity.SetDecreasedArmorValueModifier(decreaseArmorValue);
                entityBehaviour.Entity.ChangeChanceToHitModifier(decreaseHitChanceValue);
            }
        }

        public override void MagicRound()
        {
            base.MagicRound();

            // Check if monitored enemy type is nearby
            isAffected = false;
            Params param = (Params)EnchantmentParam.Value.ClassicParam;
            if (param == Params.Animals)
            {
                List<PlayerGPS.NearbyObject> animals = GameManager.Instance.PlayerGPS.GetNearbyObjects(PlayerGPS.NearbyObjectFlags.Animal, nearbyRange);
                if (animals != null && animals.Count > 0)
                {
                    isAffected = true;
                    //UnityEngine.Debug.Log("Player armour and chance to hit affected by animals");
                }
            }
            else if (param == Params.Humanoids)
            {
                List<PlayerGPS.NearbyObject> humanoids = GameManager.Instance.PlayerGPS.GetNearbyObjects(PlayerGPS.NearbyObjectFlags.Humanoid, nearbyRange);
                if (humanoids != null && humanoids.Count > 0)
                {
                    isAffected = true;
                    //UnityEngine.Debug.Log("Player armour and chance to hit affected by humanoids");
                }
            }
            else if (param == Params.Daedra)
            {
                List<PlayerGPS.NearbyObject> daedra = GameManager.Instance.PlayerGPS.GetNearbyObjects(PlayerGPS.NearbyObjectFlags.Daedra, nearbyRange);
                if (daedra != null && daedra.Count > 0)
                {
                    isAffected = true;
                    //UnityEngine.Debug.Log("Player armour and chance to hit affected by daedra");
                }
            }
        }

        #endregion

        #region Classic Support

        public enum Params
        {
            Humanoids,
            Animals,
            Daedra,
        }

        static short[] classicParamCosts =
        {
            -120,   //Humanoids
            -80,    //Animals
            -120,   //Daedra
        };

        static string[] classicTextKeys =
        {
            "fromHumanoids",
            "fromAnimals",
            "fromDaedra",
        };

        #endregion
    }
}