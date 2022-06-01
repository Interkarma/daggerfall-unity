// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    Numidium
// 
// Notes:
//

using System;
using System.Collections.Generic;
using DaggerfallConnect.Save;
using DaggerfallConnect.FallExe;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.Formulas;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Cast spell when item held (equipped).
    /// </summary>
    public class CastWhenHeld : BaseEntityEffect
    {
        protected const int normalMagicItemDegradeRate = 4;
        protected const int restingMagicItemDegradeRate = 60;

        public static readonly string EffectKey = EnchantmentTypes.CastWhenHeld.ToString();

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.ShowSpellIcon = false;
            properties.AllowedCraftingStations = MagicCraftingStations.ItemMaker;
            properties.ItemMakerFlags = ItemMakerFlags.AllowMultiplePrimaryInstances | ItemMakerFlags.AlphaSortSecondaryList;
            properties.EnchantmentPayloadFlags = EnchantmentPayloadFlags.Equipped | EnchantmentPayloadFlags.MagicRound | EnchantmentPayloadFlags.RerollEffect;
        }

        public override string GroupName => TextManager.Instance.GetLocalizedText(EffectKey);

        /// <summary>
        /// Outputs spells available to this item effect abstracted as EnchantmentSettings array.
        /// When EnchantmentSettings.ClassicParam is set, it refers to a classic spell ID.
        /// When EnchantmentSettings.CustomParam is set, it refers to a custom spell bundle.
        /// </summary>
        public override EnchantmentSettings[] GetEnchantmentSettings()
        {
            List<EnchantmentSettings> enchantments = new List<EnchantmentSettings>();

            // Enumerate classic spells
            SpellRecord.SpellRecordData spellRecord;
            for(int i = 0; i < classicSpellIDs.Length; i++)
            {
                short id = classicSpellIDs[i];
                if (!GameManager.Instance.EntityEffectBroker.GetClassicSpellRecord(id, out spellRecord))
                    throw new Exception(string.Format("Could not find classic spell record with ID '{0}'", id));

                EnchantmentSettings enchantment = new EnchantmentSettings()
                {
                    Version = 1,
                    EffectKey = EffectKey,
                    ClassicType = EnchantmentTypes.CastWhenHeld,
                    ClassicParam = id,
                    PrimaryDisplayName = GroupName,
                    SecondaryDisplayName = spellRecord.spellName,
                    EnchantCost = classicSpellCosts[i],
                };

                enchantments.Add(enchantment);
            }

            // Enumerate custom spell bundle offers supporting CastWhenHeldEnchantment flag
            EntityEffectBroker.CustomSpellBundleOffer[] offers = GameManager.Instance.EntityEffectBroker.GetCustomSpellBundleOffers(EntityEffectBroker.CustomSpellBundleOfferUsage.CastWhenHeldEnchantment);
            foreach(EntityEffectBroker.CustomSpellBundleOffer offer in offers)
            {
                EnchantmentSettings enchantment = new EnchantmentSettings()
                {
                    Version = 1,
                    EffectKey = EffectKey,
                    CustomParam = offer.Key,
                    PrimaryDisplayName = GroupName,
                    SecondaryDisplayName = offer.BundleSetttings.Name,
                    EnchantCost = offer.EnchantmentCost,
                };

                enchantments.Add(enchantment);
            }

            return enchantments.ToArray();
        }

        #region Payloads

        public override PayloadCallbackResults? EnchantmentPayloadCallback(EnchantmentPayloadFlags context, EnchantmentParam? param = null, DaggerfallEntityBehaviour sourceEntity = null, DaggerfallEntityBehaviour targetEntity = null, DaggerfallUnityItem sourceItem = null, int sourceDamage = 0)
        {
            base.EnchantmentPayloadCallback(context, param, sourceEntity, targetEntity, sourceItem, sourceDamage);

            // Validate
            if ((context != EnchantmentPayloadFlags.Equipped &&
                 context != EnchantmentPayloadFlags.MagicRound &&
                 context != EnchantmentPayloadFlags.RerollEffect) ||
                param == null || sourceEntity == null || sourceItem == null)
                return null;

            // Get caster effect manager
            EntityEffectManager casterManager = sourceEntity.GetComponent<EntityEffectManager>();
            if (!casterManager)
                return null;

            if (context == EnchantmentPayloadFlags.Equipped)
            {
                // Cast when held enchantment invokes a spell bundle that is permanent until item is removed
                InstantiateSpellBundle(param.Value, sourceEntity, sourceItem, casterManager);
            }
            else if (context == EnchantmentPayloadFlags.MagicRound)
            {
                // Apply CastWhenHeld durability loss
                ApplyDurabilityLoss(sourceItem, sourceEntity);
            }
            else if (context == EnchantmentPayloadFlags.RerollEffect)
            {
                // Recast spell bundle - previous instance has already been removed by EntityEffectManager prior to callback
                InstantiateSpellBundle(param.Value, sourceEntity, sourceItem, casterManager, true);
            }

            return null;
        }

        protected virtual void ApplyDurabilityLoss(DaggerfallUnityItem item, DaggerfallEntityBehaviour entity)
        {
            if (!GameManager.Instance.EntityEffectBroker.SyntheticTimeIncrease)
            {
                int degradeRate = GameManager.Instance.PlayerEntity.IsResting ? restingMagicItemDegradeRate : normalMagicItemDegradeRate;
                if (GameManager.Instance.EntityEffectBroker.MagicRoundsSinceStartup % degradeRate == 0)
                {
                    item.LowerCondition(1, entity.Entity, entity.Entity.Items);
                    //UnityEngine.Debug.LogFormat("CastWhenHeld degraded '{0}' by 1 durability point. {1}/{2} remaining.", item.LongName, item.currentCondition, item.maxCondition);
                }
            }
        }

        protected virtual void InstantiateSpellBundle(EnchantmentParam param, DaggerfallEntityBehaviour sourceEntity, DaggerfallUnityItem sourceItem, EntityEffectManager casterManager, bool recast = false)
        {
            if (!string.IsNullOrEmpty(param.CustomParam))
            {
                // TODO: Instantiate a custom spell bundle
            }
            else
            {
                // Instantiate a classic spell bundle
                SpellRecord.SpellRecordData spell;
                if (GameManager.Instance.EntityEffectBroker.GetClassicSpellRecord(param.ClassicParam, out spell))
                {
                    UnityEngine.Debug.LogFormat("CastWhenHeld callback found enchantment '{0}'", spell.spellName);

                    // Create effect bundle settings from classic spell
                    EffectBundleSettings bundleSettings;
                    if (GameManager.Instance.EntityEffectBroker.ClassicSpellRecordDataToEffectBundleSettings(spell, BundleTypes.HeldMagicItem, out bundleSettings))
                    {
                        // Assign bundle
                        EntityEffectBundle bundle = new EntityEffectBundle(bundleSettings, sourceEntity);
                        bundle.FromEquippedItem = sourceItem;
                        bundle.AddRuntimeFlags(BundleRuntimeFlags.ItemRecastEnabled);
                        casterManager.AssignBundle(bundle, AssignBundleFlags.BypassSavingThrows);

                        // Play cast sound on equip for player only
                        if (casterManager.IsPlayerEntity)
                            casterManager.PlayCastSound(sourceEntity, casterManager.GetCastSoundID(bundle.Settings.ElementType), true);

                        // Classic uses an item last "cast when held" effect spell cost to determine its durability loss on equip
                        // Here, all effects are considered, as it seems more coherent to do so
                        if (!recast)
                        {
                            int amount = FormulaHelper.CalculateCastingCost(spell, false);
                            sourceItem.LowerCondition(amount, sourceEntity.Entity, sourceEntity.Entity.Items);
                            //UnityEngine.Debug.LogFormat("CastWhenHeld degraded '{0}' by {1} durability points on equip. {2}/{3} remaining.", sourceItem.LongName, amount, sourceItem.currentCondition, sourceItem.maxCondition);
                        }
                    }

                    // Store equip time as last reroll time
                    sourceItem.timeEffectsLastRerolled = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime();
                }
            }
        }

        #endregion

        #region Classic Support

        // Classic spell IDs available for this effect are hard-coded and automatically populated

        static short[] classicSpellIDs =
        {
            37,     //Slowfalling
            39,     //Spell Resistance
            41,     //Water Walking
            10,     //Free Action
            42,     //Water Breathing
            11,     //Resist Cold
            12,     //Resist Fire
            26,     //Resist Poison
            13,     //Resist Shock
            6,      //Invisibility
            44,     //Chameleon
            45,     //Shadow Form
            46,     //Spell Reflection
            24,     //Troll's Blood
            47,     //Spell Absorption
            4,      //Levitate
            49,     //Tongues
            82,     //Orc Strength
            83,     //Wisdom
            84,     //Iron Will
            85,     //Nimbleness
            86,     //Feet of Notorgo
            87,     //Fortitude
            88,     //Charisma
            89,     //Jack of Trades
        };

        static short[] classicSpellCosts =
        {
            240,    //Slowfalling
            1230,   //Spell Resistance
            170,    //Water Walking
            1650,   //Free Action
            170,    //Water Breathing
            1560,   //Resist Cold
            1560,   //Resist Fire
            1560,   //Resist Poison
            1560,   //Resist Shock
            540,    //Invisibility
            210,    //Chameleon
            150,    //Shadow Form
            1720,   //Spell Reflection
            920,    //Troll's Blood
            1720,   //Spell Absorption
            330,    //Levitate
            1590,   //Tongues
            1020,   //Orc Strength
            1200,   //Wisdom
            1200,   //Iron Will
            1200,   //Nimbleness
            1200,   //Feet of Notorgo
            1200,   //Fortitude
            1200,   //Charisma
            1200,   //Jack of Trades
        };

        #endregion
    }
}