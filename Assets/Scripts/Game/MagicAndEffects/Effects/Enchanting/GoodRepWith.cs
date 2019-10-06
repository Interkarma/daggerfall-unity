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
using DaggerfallConnect.Arena2;
using DaggerfallConnect.FallExe;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Increase reaction with selected social groups while item held.
    /// </summary>
    public class GoodRepWith : BaseEntityEffect
    {
        public static readonly string EffectKey = EnchantmentTypes.GoodRepWith.ToString();

        const int adjustmentAmount = 10;

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.GroupName = TextManager.Instance.GetText(textDatabase, EffectKey);
            properties.ShowSpellIcon = false;
            properties.AllowedCraftingStations = MagicCraftingStations.ItemMaker;
            properties.ItemMakerFlags = ItemMakerFlags.AllowMultiplePrimaryInstances;
            properties.EnchantmentPayloadFlags = EnchantmentPayloadFlags.Held;
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
                    ClassicType = EnchantmentTypes.GoodRepWith,
                    ClassicParam = (short)i,
                    PrimaryDisplayName = properties.GroupName,
                    SecondaryDisplayName = TextManager.Instance.GetText(textDatabase, classicTextKeys[i]),
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

            // Get peered entity gameobject - can only operate on player entity
            DaggerfallEntityBehaviour entityBehaviour = GetPeeredEntityBehaviour(manager);
            if (!entityBehaviour || entityBehaviour.EntityType != EntityTypes.Player)
                return;

            // Apply reaction mod
            PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;
            Params param = (Params)EnchantmentParam.Value.ClassicParam;
            if (param == Params.All)
            {
                playerEntity.ChangeReactionMod(FactionFile.SocialGroups.Commoners, adjustmentAmount);
                playerEntity.ChangeReactionMod(FactionFile.SocialGroups.Merchants, adjustmentAmount);
                playerEntity.ChangeReactionMod(FactionFile.SocialGroups.Scholars, adjustmentAmount);
                playerEntity.ChangeReactionMod(FactionFile.SocialGroups.Nobility, adjustmentAmount);
                playerEntity.ChangeReactionMod(FactionFile.SocialGroups.Underworld, adjustmentAmount);
            }
            else
            {
                playerEntity.ChangeReactionMod((FactionFile.SocialGroups)EnchantmentParam.Value.ClassicParam, adjustmentAmount);
            }
        }

        public override bool IsEnchantmentExclusiveTo(EnchantmentSettings[] settingsToTest, EnchantmentParam? comparerParam = null)
        {
            string badRepWithKey = EnchantmentTypes.BadRepWith.ToString();
            foreach (EnchantmentSettings settings in settingsToTest)
            {
                // Self tests
                if (settings.EffectKey == EffectKey)
                {
                    // Exclusive with self once "All" selected
                    if (settings.ClassicParam == (int)Params.All)
                        return true;

                    // "All" is exclusive with other groups
                    if (comparerParam != null && settings.ClassicParam != (int)Params.All && comparerParam.Value.ClassicParam == (int)Params.All)
                        return true;
                }

                // Exclusive with opposing BadRepWith param
                if (settings.EffectKey == badRepWithKey && comparerParam != null && settings.ClassicParam == comparerParam.Value.ClassicParam)
                    return true;
            }

            return false;
        }

        #endregion

        #region Classic Support

        public enum Params
        {
            Commoners,
            Merchants,
            Scholars,
            Nobility,
            Underworld,
            All,
        }

        static short[] classicParamCosts =
        {
            1000,   //Commoners
            1000,   //Merchants
            1000,   //Scholars
            1000,   //Nobility
            1000,   //Underworld
            5000,   //All
        };

        static string[] classicTextKeys =
        {
            "commoners",
            "merchants",
            "scholars",
            "nobility",
            "underworld",
            "all",
        };

        #endregion
    }
}