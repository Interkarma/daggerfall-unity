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
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Items;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Remove health when used or if not used frequently enough.
    /// </summary>
    public class HealthLeech : BaseEntityEffect
    {
        public static readonly string EffectKey = EnchantmentTypes.HealthLeech.ToString();

        const int timeLeechPerRounds = 4;
        const int timeLeechAmount = 1;
        const int leechCastAmount = 16;
        const int leechWeaponAmount = 8;

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.ShowSpellIcon = false;
            properties.AllowedCraftingStations = MagicCraftingStations.ItemMaker;
            properties.ItemMakerFlags = ItemMakerFlags.AllowMultiplePrimaryInstances;
            properties.EnchantmentPayloadFlags = EnchantmentPayloadFlags.Held | EnchantmentPayloadFlags.Used | EnchantmentPayloadFlags.Strikes | EnchantmentPayloadFlags.Enchanted;
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
                    ClassicType = EnchantmentTypes.HealthLeech,
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

        public override PayloadCallbackResults? EnchantmentPayloadCallback(EnchantmentPayloadFlags context, EnchantmentParam? param = null, DaggerfallEntityBehaviour sourceEntity = null, DaggerfallEntityBehaviour targetEntity = null, DaggerfallUnityItem sourceItem = null, int sourceDamage = 0)
        {
            // Update world time enchantment last operated this item or when first enchanting item
            // Either striking with a weapon or "using" item from inventory / use UI will update this time
            // Payload callbacks are done in effect template (not the live effect) so we track time on an item field reserved for this effect
            if (sourceItem != null && (context == EnchantmentPayloadFlags.Strikes || context == EnchantmentPayloadFlags.Used || context == EnchantmentPayloadFlags.Enchanted))
                sourceItem.timeHealthLeechLastUsed = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime();

            // Must specify enchantment param
            if (param == null)
                return null;

            // Health leech operator on weapon strikes or casting from item
            Params type = (Params)param.Value.ClassicParam;
            if (sourceEntity != null && type == Params.WheneverUsed)
            {
                if (context == EnchantmentPayloadFlags.Strikes)
                    sourceEntity.Entity.DecreaseHealth(leechWeaponAmount);
                else if (context == EnchantmentPayloadFlags.Used)
                    sourceEntity.Entity.DecreaseHealth(leechCastAmount);
            }

            return null;
        }

        public override void MagicRound()
        {
            base.MagicRound();

            // Do nothing if item not set, param not set, or during synthetic time increase
            if (ParentBundle.fromEquippedItem == null ||
                EnchantmentParam == null ||
                GameManager.Instance.EntityEffectBroker.SyntheticTimeIncrease)
                return;

            // Check if timed health leech should be active
            bool timeLeechActive = false;
            uint minutesSinceLastUsed = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime() - ParentBundle.fromEquippedItem.timeHealthLeechLastUsed;
            Params type = (Params)EnchantmentParam.Value.ClassicParam;
            switch (type)
            {
                case Params.UnlessUsedDaily:
                    timeLeechActive = minutesSinceLastUsed > DaggerfallDateTime.MinutesPerDay;
                    break;
                case Params.UnlessUsedWeekly:
                    timeLeechActive = minutesSinceLastUsed > DaggerfallDateTime.MinutesPerDay * DaggerfallDateTime.DaysPerWeek;
                    break;
            }

            // Damage player on schedule if leech active
            if (timeLeechActive)
            {
                // Get peered entity gameobject
                DaggerfallEntityBehaviour entityBehaviour = GetPeeredEntityBehaviour(manager);
                if (!entityBehaviour)
                    return;

                // Health leech operator every timeLeechPerRounds
                if (GameManager.Instance.EntityEffectBroker.MagicRoundsSinceStartup % timeLeechPerRounds == 0)
                    entityBehaviour.Entity.DecreaseHealth(timeLeechAmount);
            }
        }

        #endregion

        #region Classic Support

        public enum Params
        {
            WheneverUsed,
            UnlessUsedDaily,
            UnlessUsedWeekly,
        }

        static short[] classicParamCosts =
        {
            -4000,  //Whenever used
            -500,   //Unless used daily
            -200,   //Unless used weekly
        };

        static string[] classicTextKeys =
        {
            "wheneverUsed",
            "unlessUsedDaily",
            "unlessUsedWeekly",
        };

        #endregion
    }
}