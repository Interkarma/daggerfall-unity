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

using System;
using System.Collections.Generic;
using DaggerfallConnect.Save;
using DaggerfallConnect.FallExe;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Items;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Cast spell when item used.
    /// </summary>
    public class CastWhenUsed : BaseEntityEffect
    {
        public static readonly string EffectKey = EnchantmentTypes.CastWhenUsed.ToString();

        // Items lose 10 durability points for every spell cast on use
        // http://en.uesp.net/wiki/Daggerfall:Magical_Items#Durability_of_Magical_Items
        const int durabilityLossOnUse = 10;

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.ShowSpellIcon = false;
            properties.AllowedCraftingStations = MagicCraftingStations.ItemMaker;
            properties.ItemMakerFlags = ItemMakerFlags.AllowMultiplePrimaryInstances | ItemMakerFlags.AlphaSortSecondaryList;
            properties.EnchantmentPayloadFlags = EnchantmentPayloadFlags.Used;
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
                    ClassicType = EnchantmentTypes.CastWhenUsed,
                    ClassicParam = id,
                    PrimaryDisplayName = GroupName,
                    SecondaryDisplayName = spellRecord.spellName,
                    EnchantCost = classicSpellCosts[i],
                };

                enchantments.Add(enchantment);
            }

            // Enumerate custom spell bundle offers supporting CastWhenUsedEnchantment flag
            EntityEffectBroker.CustomSpellBundleOffer[] offers = GameManager.Instance.EntityEffectBroker.GetCustomSpellBundleOffers(EntityEffectBroker.CustomSpellBundleOfferUsage.CastWhenUsedEnchantment);
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
            base.EnchantmentPayloadCallback(context, param, sourceEntity, targetEntity, sourceItem);

            // Validate
            if (context != EnchantmentPayloadFlags.Used || sourceEntity == null || param == null)
                return null;

            // Get caster effect manager
            EntityEffectManager effectManager = sourceEntity.GetComponent<EntityEffectManager>();
            if (!effectManager)
                return null;

            // Do not activate enchantment if broken
            // But still return durability loss so "item has broken" message displays
            // If AllowMagicRepairs enabled then item will not disappear
            if (sourceItem != null && sourceItem.currentCondition <= 0)
                return new PayloadCallbackResults()
                {
                    durabilityLoss = durabilityLossOnUse
                };

            // Cast when used enchantment prepares a new ready spell
            if (!string.IsNullOrEmpty(param.Value.CustomParam))
            {
                // TODO: Ready a custom spell bundle
            }
            else
            {
                // Ready a classic spell bundle
                SpellRecord.SpellRecordData spell;
                EffectBundleSettings bundleSettings;
                EntityEffectBundle bundle;
                if (GameManager.Instance.EntityEffectBroker.GetClassicSpellRecord(param.Value.ClassicParam, out spell))
                {
                    if (GameManager.Instance.EntityEffectBroker.ClassicSpellRecordDataToEffectBundleSettings(spell, BundleTypes.Spell, out bundleSettings))
                    {
                        // Self-cast spells are all assigned directly to self, "click to cast" spells are loaded to ready spell
                        // TODO: Support multiple ready spells so all loaded spells are launched on click
                        bundle = new EntityEffectBundle(bundleSettings, sourceEntity);
                        bundle.CastByItem = sourceItem;
                        if (bundle.Settings.TargetType == TargetTypes.CasterOnly)
                            effectManager.AssignBundle(bundle, AssignBundleFlags.BypassSavingThrows | AssignBundleFlags.BypassChance);
                        else
                            effectManager.SetReadySpell(bundle, true);
                    }
                }
            }

            return new PayloadCallbackResults()
            {
                durabilityLoss = durabilityLossOnUse
            };
        }

        #endregion

        #region Classic Support

        // Classic spell IDs available for this effect are hard-coded and automatically populated

        static short[] classicSpellIDs =
        {
            4,      //Levitate
            5,      //Light
            6,      //Invisibility
            7,      //Wizard's Fire
            8,      //Shock
            9,      //Strength Leech
            10,     //Free Action
            18,     //Open
            11,     //Resist Cold
            12,     //Resist Fire
            13,     //Resist Shock
            19,     //Wizard Lock
            14,     //Fireball
            15,     //Cure Poison
            16,     //Ice Bolt
            17,     //Shield
            22,     //Spell Shield
            23,     //Silence
            24,     //Troll's Blood
            20,     //Ice Storm
            25,     //Fire Storm
            26,     //Resist Poison
            33,     //Wildfire
            27,     //Spell Drain
            28,     //Far Silence
            29,     //Toxic Cloud
            34,     //Wizard Rend
            30,     //Shalidor's Mirror
            31,     //Lightning
            35,     //Medusa's Gaze
            36,     //Force Bolt
            32,     //Gods' Fire
            40,     //Stamina
            64,     //Heal
            60,     //Balyna's Antidote
            94,     //Recall
        };

        static short[] classicSpellCosts =
        {
            330,    //Levitate
            250,    //Light
            540,    //Invisibility
            480,    //Wizard's Fire
            380,    //Shock
            480,    //Strength Leech
            1650,   //Free Action
            900,    //Open
            1560,   //Resist Cold
            1560,   //Resist Fire
            1560,   //Resist Shock
            1740,   //Wizard Lock
            470,    //Fireball
            1020,   //Cure Poison
            990,    //Ice Bolt
            1040,   //Shield
            1980,   //Spell Shield
            1530,   //Silence
            920,    //Troll's Blood
            1420,   //Ice Storm
            840,    //Fire Storm
            1650,   //Resist Poison
            1020,   //Wildfire
            1300,   //Spell Drain
            2290,   //Far Silence
            1020,   //Toxic Cloud
            1610,   //Wizard Rend
            1930,   //Shalidor's Mirror
            760,    //Lightning
            2140,   //Medusa's Gaze
            3030,   //Force Bolt
            1750,   //Gods' Fire
            130,    //Stamina
            360,    //Heal
            930,    //Balyna's Antidote
            480,    //Recall
        };

        #endregion
    }
}