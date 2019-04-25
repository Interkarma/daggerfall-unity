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
    /// Notes:
    ///  * Classic reaction increase amount currently unknown.
    ///  * Only allowing one enchantment variant to be added here.
    /// TODO:
    ///  * Find correct reaction adjustment value.
    ///  * Allow adding multiple individual groups, without duplicates, exclusive to all.
    /// </summary>
    public class GoodRepWith : BaseEntityEffect
    {
        public static readonly string EffectKey = EnchantmentTypes.GoodRepWith.ToString();

        const int adjustmentAmount = 25;

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.GroupName = TextManager.Instance.GetText(textDatabase, EffectKey);
            properties.ShowSpellIcon = false;
            properties.AllowedCraftingStations = MagicCraftingStations.ItemMaker;
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

        #endregion

        #region Classic Support

        enum Params
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