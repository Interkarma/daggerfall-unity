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

using System;
using System.Collections.Generic;
using DaggerfallConnect.Save;
using DaggerfallConnect.FallExe;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Cast spell when item held (equipped).
    /// </summary>
    public class CastWhenHeld : BaseEntityEffect
    {
        public static readonly string EffectKey = EnchantmentTypes.CastWhenHeld.ToString();

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.GroupName = TextManager.Instance.GetText(textDatabase, EffectKey);
            properties.AllowedCraftingStations = MagicCraftingStations.ItemMaker;
            properties.ItemMakerFlags = ItemMakerFlags.AlphaSortSecondaryList;
        }

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
                    PrimaryDisplayName = properties.GroupName,
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
                    PrimaryDisplayName = properties.GroupName,
                    SecondaryDisplayName = offer.BundleSetttings.Name,
                    EnchantCost = offer.EnchantmentCost,
                };

                enchantments.Add(enchantment);
            }

            return enchantments.ToArray();
        }

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