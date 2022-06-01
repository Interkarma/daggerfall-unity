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
    /// Regenerate health under specific conditions.
    /// </summary>
    public class RegensHealth : BaseEntityEffect
    {
        public static readonly string EffectKey = EnchantmentTypes.RegensHealth.ToString();

        const int regeneratePerRounds = 4;
        const int regenerateAmount = 1;

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
                    ClassicType = EnchantmentTypes.RegensHealth,
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

        /// <summary>
        /// Regenerates health in a manner similar to career special.
        /// Classic will regenerate 15 health per hour, stacked per item with enchantment.
        /// </summary>
        public override void MagicRound()
        {
            base.MagicRound();

            // Must have a param
            if (EnchantmentParam == null)
                return;

            // This special only triggers once every regeneratePerRounds
            if (GameManager.Instance.EntityEffectBroker.MagicRoundsSinceStartup % regeneratePerRounds != 0)
                return;

            // Get peered entity gameobject
            DaggerfallEntityBehaviour entityBehaviour = GetPeeredEntityBehaviour(manager);
            if (!entityBehaviour)
                return;

            // Check for regenerate conditions
            bool regenerate = false;
            Params type = (Params)EnchantmentParam.Value.ClassicParam;
            switch (type)
            {
                case Params.AllTheTime:
                    regenerate = true;
                    break;
                case Params.InDarkness:
                    regenerate = GameManager.Instance.PlayerEnterExit.IsPlayerInDarkness;
                    break;
                case Params.InSunlight:
                    regenerate = GameManager.Instance.PlayerEnterExit.IsPlayerInSunlight;
                    break;
            }

            // Tick regeneration when conditions are right
            if (regenerate)
                entityBehaviour.Entity.IncreaseHealth(regenerateAmount);
        }

        #endregion

        #region Classic Support

        public enum Params
        {
            AllTheTime = 0,
            InSunlight = 1,
            InDarkness = 2,
        }

        static short[] classicParamCosts =
        {
            4000,   //all the time
            3000,   //in sunlight
            3000,   //in darkness
        };

        static string[] classicTextKeys =
        {
            "allTheTime",
            "inSunlight",
            "inDarknessLower",
        };

        #endregion
    }
}