// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2017 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut

using System;
using DaggerfallWorkshop.Utility;
using DaggerfallConnect.Arena2;
using System.Collections.Generic;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using UnityEngine;

namespace DaggerfallWorkshop.Game.Items
{
    public partial class DaggerfallUnityItem : IMacroContextProvider
    {
        public MacroDataSource GetMacroDataSource()
        {
            return new ItemMacroDataSource(this);
        }

        /// <summary>
        /// MacroDataSource context sensitive methods for items in Daggerfall Unity.
        /// </summary>
        private class ItemMacroDataSource : MacroDataSource
        {
            private string[] conditions = new string[] { "Broken", "Useless", "Battered", "Worn", "Used", "Slightly Used", "Almost New", "New" };
            private int[] conditionThresholds = new int[] {1, 5, 15, 40, 60, 75, 91, 101};

            private DaggerfallUnityItem parent;
            public ItemMacroDataSource(DaggerfallUnityItem item)
            {
                this.parent = item;
            }

            public override string ItemName()
            {
                return parent.ItemName;
            }

            public override string Worth()
            {
                return parent.value.ToString();
            }

            public override string Material()
            {   // %mat
                switch (parent.itemGroup)
                {
                    case ItemGroups.Armor:
                        return DaggerfallUnity.Instance.TextProvider.GetArmorMaterialName((ArmorMaterialTypes) parent.nativeMaterialValue);
                    case ItemGroups.Weapons:
                        return DaggerfallUnity.Instance.TextProvider.GetWeaponMaterialName((WeaponMaterialTypes) parent.nativeMaterialValue);
                    default:
                        return base.Material();
                }
            }

            public override string Condition()
            {   // %qua
                if (parent.maxCondition > 0 && parent.currentCondition <= parent.maxCondition)
                {
                    int conditionPercentage = 100 * parent.currentCondition / parent.maxCondition;
                    int i = 0;
                    while (conditionPercentage > conditionThresholds[i])
                        i++;
                    return conditions[i];
                }
                else
                    return parent.currentCondition.ToString();
            }

            public override string Weight()
            {   // %kg
                float weight = parent.weightInKg * parent.stackCount;
                return String.Format(weight % 1 == 0 ? "{0:F0}" : "{0:F2}", weight);
            }

            public override string WeaponDamage()
            {   // %wdm
                int matMod = parent.GetWeaponMaterialModifier();
                return String.Format("{0} - {1}", parent.GetBaseDamageMin() + matMod, parent.GetBaseDamageMax() + matMod);
            }

            // Armour mod is double what classic displays, but this is correct according to Allofich.
            public override string ArmourMod()
            {   // %mod
                return parent.GetMaterialArmorValue().ToString("+0;-0;0");
            }

            public override string BookAuthor()
            {   // %ba
                BookFile bookFile = new BookFile();
                bookFile.OpenBook(DaggerfallUnity.Instance.Arena2Path, BookFile.messageToBookFilename(parent.message));
                // Should the bookfile get closed?
                return bookFile.Author;
            }

            public override TextFile.Token[] MagicPowers(TextFile.Formatting format)
            {   // %mpw
                if (parent.IsArtifact)
                {
                    // Use appropriate artifact description message. (8700-8721)
                    try {
                        ArtifactsSubTypes artifactType = ItemHelper.GetArtifactSubType(parent.shortName);
                        return DaggerfallUnity.Instance.TextProvider.GetRSCTokens(8700 + (int)artifactType);
                    } catch (KeyNotFoundException e) {
                        Debug.Log(e.Message);
                        return null;
                    }
                }
                else if (!parent.IsIdentified)
                {
                    // Powers unknown.
                    TextFile.Token nopowersToken = new TextFile.Token();
                    nopowersToken.text = HardStrings.powersUnknown;
                    nopowersToken.formatting = TextFile.Formatting.Text;
                    return new TextFile.Token[] { nopowersToken };
                }
                else
                {
                    // List item powers. 
                    // TODO: Update once magic effects have been implemented. (just puts "Power number N" for now)
                    // Pretty sure low numbers are type of application, and higher ones are effects.
                    // e.g. shield of fortitude is [1, 87] which maps to "Cast when held: Fortitude" in classic.
                    List<TextFile.Token> magicPowersTokens = new List<TextFile.Token>();
                    for (int i = 0; i < parent.legacyMagic.Length; i++)
                    {
                        if (parent.legacyMagic[i] == 0xffff)
                            break;
                        TextFile.Token powerToken = new TextFile.Token();
                        powerToken.text = "Power number " + parent.legacyMagic[i];
                        powerToken.formatting = TextFile.Formatting.Text;
                        magicPowersTokens.Add(powerToken);
                        TextFile.Token formatToken = new TextFile.Token();
                        formatToken.formatting = format;
                        magicPowersTokens.Add(formatToken);
                    }
                    return magicPowersTokens.ToArray();
                }
            }

        }
    }
}