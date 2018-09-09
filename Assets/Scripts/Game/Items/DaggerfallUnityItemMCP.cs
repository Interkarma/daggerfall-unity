// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
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
using DaggerfallConnect.FallExe;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game.Items
{
    public partial class DaggerfallUnityItem : IMacroContextProvider
    {
        private Recipe[] recipeArray;

        private struct PaintingInfo
        {
            public string filename;
            public uint fileIdx;
            public string artist;   // for %an macro
            public string sub;      // for %sub macro
            public string adj;      // for %adj macro
            public string pp1;      // for %pp1 macro
            public string pp2;      // for %pp2 macro
        }

        private PaintingInfo paintingInfo;

        public MacroDataSource GetMacroDataSource()
        {
            return new ItemMacroDataSource(this);
        }

        private void InitPaintingInfo()
        {
            if (ItemGroup == ItemGroups.Paintings && paintingInfo.filename == null)
            {
                DFRandom.srand(message);
                uint paintingIndex = DFRandom.rand() % 180;
                paintingInfo.fileIdx = paintingIndex & 7;
                char paintingFileChar = (char)((paintingIndex >> 3) + 97);
                paintingInfo.filename = paintingFileChar + "paint.cif";

                byte[] paintingRecord = DaggerfallUnity.Instance.ContentReader.PaintFileReader.Read(paintingIndex);
                Debug.LogFormat("painting file: {0}, index: {1}, cif idx: {2}", paintingInfo.filename, paintingIndex, paintingInfo.fileIdx);

                int sub = GetPaintingRecordPart(paintingRecord, 0, 9) + 6100; // for %sub macro
                int adj = GetPaintingRecordPart(paintingRecord, 10, 19) + 6200; // for %adj macro
                int pp1 = GetPaintingRecordPart(paintingRecord, 20, 29) + 6300; // for %pp1 macro
                int pp2 = GetPaintingRecordPart(paintingRecord, 30, 39) + 6400; // for %pp2 macro

                ITextProvider textProvider = DaggerfallUnity.Instance.TextProvider;
                paintingInfo.sub = textProvider.GetRandomTokens(sub, (message & 0xF0) >> 4)[0].text;
                paintingInfo.adj = textProvider.GetRandomTokens(adj, (message & 0xF))[0].text;
                paintingInfo.pp1 = textProvider.GetRandomTokens(pp1, (message & 0xF00) >> 8)[0].text;
                paintingInfo.pp2 = textProvider.GetRandomTokens(pp2, (message & 0xF000) >> 12)[0].text;

                DFRandom.srand(message);
                NameHelper.BankTypes type = (NameHelper.BankTypes) DFRandom.random_range_inclusive(0, 7);
                Genders gender = (Genders) DFRandom.random_range_inclusive(0, 1);
                paintingInfo.artist = DaggerfallUnity.Instance.NameHelper.FullName(type, gender);

            }
        }

        private int GetPaintingRecordPart(byte[] paintingRecord, int start, int end)
        {
            int i = start;
            while (i <= end && paintingRecord[i] != 0xFF)
                i++;
            return (i - start == 1) ? paintingRecord[i-1] : paintingRecord[DFRandom.random_range_inclusive(start, i - 1)];
        }

        /// <summary>
        /// MacroDataSource context sensitive methods for items in Daggerfall Unity.
        /// </summary>
        private class ItemMacroDataSource : MacroDataSource
        {
            private string[] conditions = new string[] { HardStrings.Broken, HardStrings.Useless, HardStrings.Battered, HardStrings.Worn, HardStrings.Used, HardStrings.SlightlyUsed, HardStrings.AlmostNew, HardStrings.New };
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

            public override string PaintingAdjective()
            {   // %adj
                parent.InitPaintingInfo();
                return parent.paintingInfo.adj;
            }
            public override string ArtistName()
            {   // %an
                parent.InitPaintingInfo();
                return parent.paintingInfo.artist;
            }
            public override string PaintingPrefix1()
            {   // %pp1
                parent.InitPaintingInfo();
                return parent.paintingInfo.pp1;
            }
            public override string PaintingPrefix2()
            {   // %pp2
                parent.InitPaintingInfo();
                return parent.paintingInfo.pp2;
            }
            public override string PaintingSubject()
            {   // %sub
                parent.InitPaintingInfo();
                return parent.paintingInfo.sub;
            }

            public override string HeldSoul()
            {   // %hs
                if (parent.trappedSoulType == MobileTypes.None)
                    return HardStrings.Nothing;
                MobileEnemy soul;
                EnemyBasics.GetEnemy(parent.trappedSoulType, out soul);
                return soul.Name;
            }

            public override string Potion()
            {   // %po
                KeyValuePair<string, Recipe[]> mapping = DaggerfallUnity.Instance.ItemHelper.getPotionRecipesByID(parent.typeDependentData);
                parent.recipeArray = mapping.Value;
                if (parent.TemplateIndex == (int)MiscItems.Potion_recipe)
                    return mapping.Key;                                          // "Potion recipe for %po"
                else if (parent.TemplateIndex == (int)UselessItems1.Glass_Bottle)
                    return HardStrings.potionOf.Replace("%po", mapping.Key);     // "Potion of %po"
                throw new NotImplementedException();
            }


            public override TextFile.Token[] PotionRecipeIngredients(TextFile.Formatting format)
            {
                // InconsolableCellist:
                // Potions can have multiple recipes, and it's unclear how this variation is stored
                // The actual variation could be stored in the currentVariation field, but I haven't been able find any recipes
                // in the game that aren't just the first recipe in the list; for now we'll just pick the first one here
                List<TextFile.Token> ingredientsTokens = new List<TextFile.Token>();
                Ingredient[] ingredients = parent.recipeArray[0].ingredients;
                for (int i = 0; i < ingredients.Length; ++i)
                {
                    ingredientsTokens.Add(TextFile.CreateTextToken(ingredients[i].name));
                    ingredientsTokens.Add(TextFile.CreateFormatToken(format));
                }
                return ingredientsTokens.ToArray();
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
                    TextFile.Token nopowersToken = TextFile.CreateTextToken(HardStrings.powersUnknown);
                    return new TextFile.Token[] { nopowersToken };
                }
                else
                {
                    // List item powers. 
                    List<TextFile.Token> magicPowersTokens = new List<TextFile.Token>();
                    for (int i = 0; i < parent.legacyMagic.Length; i++)
                    {
                        // Also 65535 to handle saves from when the type was read as an unsigned value
                        if (parent.legacyMagic[i].type == EnchantmentTypes.None || (int)parent.legacyMagic[i].type == 65535)
                            break;

                        string firstPart = HardStrings.itemPowers[(int)parent.legacyMagic[i].type] + " ";

                        if (parent.legacyMagic[i].type == EnchantmentTypes.SoulBound && parent.legacyMagic[i].param != -1)
                        {
                            magicPowersTokens.Add(TextFile.CreateTextToken(firstPart + HardStrings.enemyNames[parent.legacyMagic[i].param]));
                        }
                        else if (parent.legacyMagic[i].type == EnchantmentTypes.ExtraSpellPts)
                        {
                            magicPowersTokens.Add(TextFile.CreateTextToken(firstPart + HardStrings.extraSpellPtsTimes[parent.legacyMagic[i].param]));
                        }
                        else if (parent.legacyMagic[i].type == EnchantmentTypes.PotentVs || parent.legacyMagic[i].type == EnchantmentTypes.LowDamageVs)
                        {
                            magicPowersTokens.Add(TextFile.CreateTextToken(firstPart + HardStrings.enemyGroupNames[parent.legacyMagic[i].param]));
                        }
                        else if (parent.legacyMagic[i].type == EnchantmentTypes.RegensHealth)
                        {
                            magicPowersTokens.Add(TextFile.CreateTextToken(firstPart + HardStrings.regensHealthTimes[parent.legacyMagic[i].param]));
                        }
                        else if (parent.legacyMagic[i].type == EnchantmentTypes.VampiricEffect)
                        {
                            magicPowersTokens.Add(TextFile.CreateTextToken(firstPart + HardStrings.vampiricEffectRanges[parent.legacyMagic[i].param]));
                        }
                        else if (parent.legacyMagic[i].type == EnchantmentTypes.IncreasedWeightAllowance)
                        {
                            magicPowersTokens.Add(TextFile.CreateTextToken(firstPart + HardStrings.increasedWeightAllowances[parent.legacyMagic[i].param]));
                        }
                        else if (parent.legacyMagic[i].type == EnchantmentTypes.EnhancesSkill)
                        {
                            magicPowersTokens.Add(TextFile.CreateTextToken(firstPart + DaggerfallUnity.Instance.TextProvider.GetSkillName((DaggerfallConnect.DFCareer.Skills)parent.legacyMagic[i].param)));
                        }
                        else if (parent.legacyMagic[i].type == EnchantmentTypes.ImprovesTalents)
                        {
                            magicPowersTokens.Add(TextFile.CreateTextToken(firstPart + HardStrings.improvedTalents[parent.legacyMagic[i].param]));
                        }
                        else if (parent.legacyMagic[i].type == EnchantmentTypes.GoodRepWith || parent.legacyMagic[i].type == EnchantmentTypes.BadRepWith)
                        {
                            magicPowersTokens.Add(TextFile.CreateTextToken(firstPart + HardStrings.repWithGroups[parent.legacyMagic[i].param]));
                        }
                        else if (parent.legacyMagic[i].type == EnchantmentTypes.ItemDeteriorates)
                        {
                            magicPowersTokens.Add(TextFile.CreateTextToken(firstPart + HardStrings.itemDeteriorateLocations[parent.legacyMagic[i].param]));
                        }
                        else if(parent.legacyMagic[i].type == EnchantmentTypes.UserTakesDamage)
                        {
                            magicPowersTokens.Add(TextFile.CreateTextToken(firstPart + HardStrings.userTakesDamageLocations[parent.legacyMagic[i].param]));
                        }
                        else if (parent.legacyMagic[i].type == EnchantmentTypes.HealthLeech)
                        {
                            magicPowersTokens.Add(TextFile.CreateTextToken(firstPart + HardStrings.healthLeechStopConditions[parent.legacyMagic[i].param]));
                        }
                        else if (parent.legacyMagic[i].type == EnchantmentTypes.BadReactionsFrom)
                        {
                            magicPowersTokens.Add(TextFile.CreateTextToken(firstPart + HardStrings.badReactionFromEnemyGroups[parent.legacyMagic[i].param]));
                        }
                        else if (parent.legacyMagic[i].type <= EnchantmentTypes.CastWhenStrikes)
                        {
                            List<DaggerfallConnect.Save.SpellRecord.SpellRecordData> spells = DaggerfallSpellReader.ReadSpellsFile();
                            bool found = false;

                            foreach (DaggerfallConnect.Save.SpellRecord.SpellRecordData spell in spells)
                            {
                                if (spell.index == parent.legacyMagic[i].param)
                                {
                                    magicPowersTokens.Add(TextFile.CreateTextToken(firstPart + spell.spellName));
                                    found = true;
                                    break;
                                }
                            }

                            if (found == false)
                                magicPowersTokens.Add(TextFile.CreateTextToken(firstPart + "ERROR"));
                        }
                        else
                        {
                            magicPowersTokens.Add(TextFile.CreateTextToken(firstPart));
                        }

                        magicPowersTokens.Add(TextFile.CreateFormatToken(format));
                    }
                    return magicPowersTokens.ToArray();
                }
            }

        }
    }
}