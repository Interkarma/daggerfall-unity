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
    /// Vampiric effect at range or on strike.
    /// </summary>
    public class VampiricEffect : BaseEntityEffect
    {
        public static readonly string EffectKey = EnchantmentTypes.VampiricEffect.ToString();

        const float vampiricDrainRange = 2.25f;     // Testing classic shows range of vampiric effect items is approx. melee distance
        const int regeneratePerRounds = 4;
        const int regenerateAmount = 1;

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.ShowSpellIcon = false;
            properties.AllowedCraftingStations = MagicCraftingStations.ItemMaker;
            properties.ItemMakerFlags = ItemMakerFlags.AllowMultiplePrimaryInstances;
            properties.EnchantmentPayloadFlags = EnchantmentPayloadFlags.Held | EnchantmentPayloadFlags.Strikes;
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
                    ClassicType = EnchantmentTypes.VampiricEffect,
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

            // Must have a param
            if (EnchantmentParam == null)
                return;

            // Must be correct vampiric effect type
            Params type = (Params)EnchantmentParam.Value.ClassicParam;
            if (type != Params.AtRange)
                return;

            // This special only triggers once every regeneratePerRounds
            if (GameManager.Instance.EntityEffectBroker.MagicRoundsSinceStartup % regeneratePerRounds != 0)
                return;

            // Get peered entity gameobject
            DaggerfallEntityBehaviour entityBehaviour = GetPeeredEntityBehaviour(manager);
            if (!entityBehaviour)
                return;

            // Drain all enemies in range
            List<PlayerGPS.NearbyObject> nearby = GameManager.Instance.PlayerGPS.GetNearbyObjects(PlayerGPS.NearbyObjectFlags.Enemy, vampiricDrainRange);
            if (nearby != null && nearby.Count > 0)
            {
                foreach (PlayerGPS.NearbyObject enemy in nearby)
                {
                    // Get entity behaviour from found object
                    DaggerfallEntityBehaviour enemyBehaviour = (enemy.gameObject) ? enemy.gameObject.GetComponent<DaggerfallEntityBehaviour>() : null;
                    if (!enemyBehaviour)
                        continue;

                    // Transfer health from remote entity to this one
                    enemyBehaviour.Entity.CurrentHealth -= regenerateAmount;
                    entityBehaviour.Entity.CurrentHealth += regenerateAmount;
                    //UnityEngine.Debug.LogFormat("Entity {0} drained {1} health from nearby {2}", entityBehaviour.Entity.Name, regenerateAmount, enemyBehaviour.Entity.Name);
                }
            }
        }

        public override PayloadCallbackResults? EnchantmentPayloadCallback(EnchantmentPayloadFlags context, EnchantmentParam? param = null, DaggerfallEntityBehaviour sourceEntity = null, DaggerfallEntityBehaviour targetEntity = null, DaggerfallUnityItem sourceItem = null, int sourceDamage = 0)
        {
            base.EnchantmentPayloadCallback(context, param, sourceEntity, targetEntity, sourceItem);

            // Validate
            if (param == null || sourceEntity == null || targetEntity == null)
                return null;

            // Must be correct vampiric effect type
            Params type = (Params)param.Value.ClassicParam;
            if (type != Params.WhenStrikes)
                return null;

            // Heal source entity by base damage caused to target
            // Was not able to fully confirm this how effect works, but seems close from observation alone.
            // TODO: This will likely need more research and refinement.
            sourceEntity.Entity.CurrentHealth += sourceDamage;
            //UnityEngine.Debug.LogFormat("Entity {0} drained {1} health by striking {2}", sourceEntity.Entity.Name, sourceDamage, targetEntity.Entity.Name);

            return null;
        }

        #endregion

        #region Classic Support

        enum Params
        {
            AtRange = 0,
            WhenStrikes = 1,
        }

        static short[] classicParamCosts =
        {
            2000,   //at range
            1000,   //when strikes
        };

        static string[] classicTextKeys =
        {
            "atRange",
            "whenStrikes",
        };

        #endregion
    }
}