// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2019 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Numidium
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Player;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Formulas;
using DaggerfallWorkshop.Utility.AssetInjection;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Implements special advantage/disadvantage window.
    /// </summary>
    public class CreateCharSpecialAdvantageWindow : DaggerfallPopupWindow
    {
        public struct SpecialAdvDis 
        {
            public string primaryString;
            public string secondaryString;
            public int difficulty;
        };

        const string nativeImgName = "CUST01I0.IMG";
        const string nativeImgOverlayName = "CUST02I0.IMG";
        const int maxItems = 7;
        const int maxLabels = maxItems * 2;
        const int labelSpacing = 8;
        const int tandemLabelSpacing = 6;
        const int advPickerItemCount = 12;
        const float defaultSpellPointMod = .5f;

        DFCareer advantageData;
        List<SpecialAdvDis> advDisList;
        List<SpecialAdvDis> otherList;

        Texture2D nativeTexture;
        Texture2D nativeOverlayTexture;
        DaggerfallFont font;
        Panel advantagePanel = new Panel();
        Panel overlayPanel = new Panel();
        bool isDisadvantages;

        #region List picker strings

        string[] advantageStrings = new string[]
        {
            HardStrings.acuteHearing,
            HardStrings.adrenalineRush,
            HardStrings.athleticism,
            HardStrings.bonusToHit,
            HardStrings.expertiseIn,
            HardStrings.immunity,
            HardStrings.increasedMagery,
            HardStrings.rapidHealing,
            HardStrings.regenerateHealth,
            HardStrings.resistance,
            HardStrings.spellAbsorption
        };
        string[] disadvantageStrings = new string[]
        {
            HardStrings.criticalWeakness,
            HardStrings.damage,
            HardStrings.darknessPoweredMagery,
            HardStrings.forbiddenArmorType,
            HardStrings.forbiddenMaterial,
            HardStrings.forbiddenShieldTypes,
            HardStrings.forbiddenWeaponry,
            HardStrings.inabilityToRegen,
            HardStrings.lightPoweredMagery,
            HardStrings.lowTolerance,
            HardStrings.phobia
        };
        string[] enemyTypeStrings = new string[]
        {
            HardStrings.animals,
            HardStrings.daedra,
            HardStrings.humanoid,
            HardStrings.undead
        };
        string[] weaponTypeStrings = new string[]
        {
            HardStrings.axe,
            HardStrings.bluntWeapon,
            HardStrings.handToHand,
            HardStrings.longBlade,
            HardStrings.missileWeapon,
            HardStrings.shortBlade
        };
        string[] effectTypeStrings = new string[]
        {
            HardStrings.toDisease,
            HardStrings.toFire,
            HardStrings.toFrost,
            HardStrings.toMagic,
            HardStrings.toParalysis,
            HardStrings.toPoison,
            HardStrings.toShock
        };
        string[] increasedMageryStrings = new string[]
        {
            HardStrings.intInSpellPoints15,
            HardStrings.intInSpellPoints175,
            HardStrings.intInSpellPoints2,
            HardStrings.intInSpellPoints3,
            HardStrings.intInSpellPoints
        };
        string[] effectEnvStrings = new string[]
        {
            HardStrings.general,
            HardStrings.inDarkness,
            HardStrings.inLight
        };
        string[] regenHealthStrings = new string[]
        {
            HardStrings.general,
            HardStrings.inDarkness,
            HardStrings.inLight,
            HardStrings.whileImmersed
        };
        string[] damageEnvStrings = new string[]
        {
            HardStrings.fromHolyPlaces,
            HardStrings.fromSunlight
        };
        string[] darknessPoweredStrings = new string[]
        {
            HardStrings.lowerMagicAbilityDaylight,
            HardStrings.unableToUseMagicInDaylight
        };
        string[] lightPoweredStrings = new string[]
        {
            HardStrings.lowerMagicAbilityDarkness,
            HardStrings.unableToUseMagicInDarkness
        };
        string[] armorTypeStrings = new string[]
        {
            HardStrings.chain,
            HardStrings.leather,
            HardStrings.plate
        };
        string[] materialStrings = new string[]
        {
            HardStrings.adamantium,
            HardStrings.daedric,
            HardStrings.dwarven,
            HardStrings.ebony,
            HardStrings.elven,
            HardStrings.iron,
            HardStrings.mithril,
            HardStrings.orcish,
            HardStrings.silver,
            HardStrings.steel
        };
        string[] shieldTypeStrings = new string[]
        {
            HardStrings.buckler,
            HardStrings.kiteShield,
            HardStrings.roundShield,
            HardStrings.towerShield
        };

        #endregion

        #region Difficulty adjustment dictionary

        Dictionary<string, int> difficultyDict;

        #endregion

        #region UI Rects

        Rect addAdvantageButtonRect = new Rect(80, 4, 72, 22);
        Rect exitButtonRect = new Rect(6, 179, 155, 13);

        #endregion

        #region Buttons

        Button addAdvantageButton;
        Button exitButton;

        #endregion

        #region Text Labels

        TextLabel[] advantageLabels = new TextLabel[maxLabels];

        #endregion

        public CreateCharSpecialAdvantageWindow(IUserInterfaceManager uiManager, List<SpecialAdvDis> advDisList, List<SpecialAdvDis> otherList, DFCareer careerData, IUserInterfaceWindow previous = null, bool isDisadvantages = false)
            : base(uiManager, previous)
        {
            this.isDisadvantages = isDisadvantages;
            this.advDisList = advDisList;
            this.otherList = otherList;
            this.advantageData = careerData;
        }

        #region Setup Methods

        protected override void Setup()
        {
            if (IsSetup)
                return;

            base.Setup();

            // Load native texture
            nativeTexture = DaggerfallUI.GetTextureFromImg(nativeImgName);
            nativeOverlayTexture = DaggerfallUI.GetTextureFromImg(nativeImgOverlayName);
            if (!nativeTexture || !nativeOverlayTexture)
                throw new Exception("CreateCharSpecialAdvantage: Could not load native texture.");

            // Create panel for window
            advantagePanel.Size = TextureReplacement.GetSize(nativeTexture, nativeImgName);
            advantagePanel.HorizontalAlignment = HorizontalAlignment.Left;
            advantagePanel.VerticalAlignment = VerticalAlignment.Top;
            advantagePanel.BackgroundTexture = nativeTexture;
            advantagePanel.BackgroundTextureLayout = BackgroundLayout.StretchToFill;
            NativePanel.Components.Add(advantagePanel);

            // Setup UI components
            font = DaggerfallUI.SmallFont;
            Panel buttonPanel = NativePanel;
            if (!isDisadvantages)  // Adding this overlay makes it appear as Special Advantages instead of Disadvantages
            {
                overlayPanel.Size = TextureReplacement.GetSize(nativeOverlayTexture, nativeImgOverlayName);
                overlayPanel.HorizontalAlignment = HorizontalAlignment.Left;
                overlayPanel.VerticalAlignment = VerticalAlignment.Top;
                overlayPanel.BackgroundTexture = nativeOverlayTexture;
                overlayPanel.BackgroundTextureLayout = BackgroundLayout.StretchToFill;
                advantagePanel.Components.Add(overlayPanel);
                buttonPanel = overlayPanel;
            }
            addAdvantageButton = DaggerfallUI.AddButton(addAdvantageButtonRect, buttonPanel);
            addAdvantageButton.OnMouseClick += AddAdvantageButton_OnMouseClick;
            addAdvantageButton.ClickSound = DaggerfallUI.Instance.GetAudioClip(SoundClips.ButtonClick);
            exitButton = DaggerfallUI.AddButton(exitButtonRect, NativePanel);
            exitButton.OnMouseClick += ExitButton_OnMouseClick;
            exitButton.ClickSound = DaggerfallUI.Instance.GetAudioClip(SoundClips.ButtonClick);
            for (int i = 0; i < maxLabels; i++)
            {
                advantageLabels[i] = DaggerfallUI.AddTextLabel(font, new Vector2(8, 35 + i * labelSpacing), string.Empty, NativePanel);
                advantageLabels[i].OnMouseClick += AdvantageLabel_OnMouseClick;
                advantageLabels[i].Tag = -1;
            }
            UpdateLabels();
            InitializeAdjustmentDict();

            IsSetup = true;
        }

        #endregion

        public override void Update()
        {
            base.Update();
        }

        public override void Draw()
        {
            base.Draw();
        }

        #region Event Handlers

        void AddAdvantageButton_OnMouseClick(BaseScreenComponent sender, Vector2 pos)
        {
            string[] items;

            if (advDisList.Count == maxItems)
            {
                return;
            }

            DaggerfallListPickerWindow advantagePicker = new DaggerfallListPickerWindow(uiManager, this, DaggerfallUI.SmallFont, advPickerItemCount);
            advantagePicker.OnItemPicked += AdvantagePicker_OnItemPicked;

            items = isDisadvantages ? disadvantageStrings : advantageStrings;
            foreach (string s in items)
            {
                advantagePicker.ListBox.AddItem(s);
            }

            uiManager.PushWindow(advantagePicker);
        }

        void AdvantagePicker_OnItemPicked(int index, string advantageName)
        {
            CloseWindow();

            SpecialAdvDis s = new SpecialAdvDis 
            {
                primaryString = advantageName
                , secondaryString = string.Empty
            };
            string[] secondaryList = null;
            // advantages/disadvantages with secondary options
            switch (advantageName)
            {
                case HardStrings.bonusToHit:
                case HardStrings.phobia:
                    secondaryList = enemyTypeStrings;
                    break;
                case HardStrings.expertiseIn:
                case HardStrings.forbiddenWeaponry:
                    secondaryList = weaponTypeStrings;
                    break;
                case HardStrings.immunity:
                case HardStrings.resistance:
                case HardStrings.criticalWeakness:
                    secondaryList = effectTypeStrings;
                    break;
                case HardStrings.increasedMagery:
                    // limit to 1 magery increase advantage for the character
                    bool alreadyAdded = false;
                    foreach (SpecialAdvDis item in advDisList)
                    {
                        if (item.primaryString == HardStrings.increasedMagery)
                        {
                            alreadyAdded = true;
                        }
                    }
                    if (alreadyAdded)
                    {
                        return;
                    }
                    secondaryList = increasedMageryStrings;
                    break;
                case HardStrings.rapidHealing:
                case HardStrings.spellAbsorption:
                    secondaryList = effectEnvStrings;
                    break;
                case HardStrings.regenerateHealth:
                    secondaryList = regenHealthStrings;
                    break;
                case HardStrings.damage:
                    secondaryList = damageEnvStrings;
                    break;
                case HardStrings.darknessPoweredMagery:
                    secondaryList = darknessPoweredStrings;
                    break;
                case HardStrings.forbiddenArmorType:
                    secondaryList = armorTypeStrings;
                    break;
                case HardStrings.forbiddenMaterial:
                    secondaryList = materialStrings;
                    break;
                case HardStrings.forbiddenShieldTypes:
                    secondaryList = shieldTypeStrings;
                    break;
                case HardStrings.lightPoweredMagery:
                    secondaryList = lightPoweredStrings;
                    break;
                case HardStrings.lowTolerance:
                    secondaryList = effectTypeStrings;
                    break;
                default:
                    break;
            }
            if (secondaryList == null)
            {
                if (CannotAddAdvantage(s))
                {
                    return;
                }
                s = new SpecialAdvDis 
                {
                    primaryString = advantageName
                    , secondaryString = string.Empty
                    , difficulty = GetAdvDisAdjustment(advantageName, string.Empty)
                };
                advDisList.Add(s);
                UpdateLabels();
                UpdateDifficultyAdjustment();
            } 
            else
            {
                DaggerfallListPickerWindow secondaryPicker = new DaggerfallListPickerWindow(uiManager, this, DaggerfallUI.SmallFont, advPickerItemCount);
                secondaryPicker.ListBox.Font = DaggerfallUI.SmallFont;
                secondaryPicker.OnItemPicked += SecondaryPicker_OnItemPicked;
                secondaryPicker.OnCancel += SecondaryPicker_OnCancel;
                foreach (string secondaryString in secondaryList)
                {
                    secondaryPicker.ListBox.AddItem(secondaryString);
                }
                uiManager.PushWindow(secondaryPicker);
                advDisList.Add(s);
            }
        }

        void SecondaryPicker_OnItemPicked(int index, string itemString)
        {
            CloseWindow();
            string primary = advDisList[advDisList.Count - 1].primaryString;
            SpecialAdvDis item = new SpecialAdvDis { primaryString = primary, secondaryString = itemString, difficulty = GetAdvDisAdjustment(primary, itemString) };
            if (CannotAddAdvantage(item))
            {
                advDisList.RemoveAt(advDisList.Count - 1);
                return;
            }
            advDisList[advDisList.Count - 1] = item;
            UpdateLabels();
            UpdateDifficultyAdjustment();
        }

        void SecondaryPicker_OnCancel(DaggerfallPopupWindow sender)
        {
            advDisList.RemoveAt(advDisList.Count - 1);
        }

        void AdvantageLabel_OnMouseClick(BaseScreenComponent sender, Vector2 pos)
        {
            for (int i = 0; i < advDisList.Count; i++)
            {
                if (i == (int)sender.Tag)
                {
                    // set spell point modifier back to default if user removes magery advantage
                    if (advDisList[i].primaryString == HardStrings.increasedMagery)
                    {
                        advantageData.SpellPointMultiplier = DFCareer.SpellPointMultipliers.Times_0_50;
                        advantageData.SpellPointMultiplierValue = defaultSpellPointMod;
                    }
                    advDisList.RemoveAt(i);
                    sender.Tag = -1;
                    UpdateLabels();
                    UpdateDifficultyAdjustment();
                    return;
                }
            }
        }

        void ExitButton_OnMouseClick(BaseScreenComponent sender, Vector2 pos)
        {
            CloseWindow();
        }

        #endregion

        #region Helper methods

        int GetAdvDisAdjustment(string primary, string secondary)
        {
            switch (primary)
            {
                // secondary strings have no effect on the following cases
                case HardStrings.expertiseIn:
                case HardStrings.immunity:
                case HardStrings.resistance:
                case HardStrings.criticalWeakness:
                case HardStrings.forbiddenShieldTypes:
                case HardStrings.forbiddenWeaponry:
                case HardStrings.lowTolerance:
                case HardStrings.phobia:
                    return difficultyDict[primary];
                default:
                    return difficultyDict[primary + secondary];
            }
        }

        void UpdateLabels()
        {
            // Clear all labels
            for (int i = 0; i < maxLabels; i++)
            {
                advantageLabels[i].Text = string.Empty;
                advantageLabels[i].Tag = -1;
            }
            // Insert string(s) at the bottommost label
            for (int i = 0; i < advDisList.Count; i++)
            {
                int j = -1;
                while (advantageLabels[++j].Text != string.Empty)
                    ;
                advantageLabels[j].Text = advDisList[i].primaryString;
                advantageLabels[j].Tag = i;
                if (advDisList[i].secondaryString != string.Empty)
                {
                    advantageLabels[j + 1].Text = advDisList[i].secondaryString;
                    advantageLabels[j + 1].Tag = i;
                    // squish labels together if they represent the same item
                    advantageLabels[j + 1].Position = new Vector2(advantageLabels[j + 1].Position.x, advantageLabels[j].Position.y + tandemLabelSpacing);
                    // ensure default label spacing for the rest of the labels
                    for (int k = j + 1; k < maxLabels - 1; k++)
                    {
                        advantageLabels[k + 1].Position = new Vector2(advantageLabels[k + 1].Position.x, advantageLabels[k].Position.y + labelSpacing);
                    }
                } 
                else
                {
                    // ensure default label spacing
                    for (int k = j; k < maxLabels - 1; k++)
                    {
                        advantageLabels[k + 1].Position = new Vector2(advantageLabels[k + 1].Position.x, advantageLabels[k].Position.y + labelSpacing);
                    }
                }
            }
        }

        void UpdateDifficultyAdjustment()
        {
            int total = 0;

            foreach (SpecialAdvDis s in advDisList)
            {
                total += s.difficulty;
            }

            CreateCharCustomClass prevWindow = (CreateCharCustomClass)this.PreviousWindow;
            if (isDisadvantages)
            {
                prevWindow.DisadvantageAdjust = total;
            } 
            else 
            {
                prevWindow.AdvantageAdjust = total;
            }
        }

        bool CannotAddAdvantage(SpecialAdvDis advDis)
        {
            // Combine advantage and disadvantage lists
            List<SpecialAdvDis> adList = new List<SpecialAdvDis>();
            adList.AddRange(advDisList);
            adList.AddRange(otherList);
            for (int i = 0; i < adList.Count; i++)
            {
                // Duplicate
                if (advDis.primaryString == adList[i].primaryString && advDis.secondaryString == adList[i].secondaryString)
                    return true;
                // Incompatible advantage/disadvantage pairs
                if (IsMatchingAdvPair(HardStrings.bonusToHit, HardStrings.phobia, advDis, adList[i]))
                    return true;
                if (IsMatchingAdvPair(HardStrings.expertiseIn, HardStrings.forbiddenWeaponry, advDis, adList[i]))
                    return true;
                // No immunity, resistance, low tolerance, or critical weakness may co-exist
                if (IsMatchingAdvPair(HardStrings.immunity, HardStrings.resistance, advDis, adList[i]))
                    return true;
                if (IsMatchingAdvPair(HardStrings.immunity, HardStrings.criticalWeakness, advDis, adList[i]))
                    return true;
                if (IsMatchingAdvPair(HardStrings.immunity, HardStrings.lowTolerance, advDis, adList[i]))
                    return true;
                if (IsMatchingAdvPair(HardStrings.resistance, HardStrings.lowTolerance, advDis, adList[i]))
                    return true;
                if (IsMatchingAdvPair(HardStrings.resistance, HardStrings.criticalWeakness, advDis, adList[i]))
                    return true;
                if (IsMatchingAdvPair(HardStrings.lowTolerance, HardStrings.criticalWeakness, advDis, adList[i]))
                    return true;
            }

            return false;
        }

        bool IsMatchingAdvPair(string str1, string str2, SpecialAdvDis candidate, SpecialAdvDis incumbent)
        {
            if ((candidate.primaryString == str1 && incumbent.primaryString == str2
                || candidate.primaryString == str2 && incumbent.primaryString == str1)
                && candidate.secondaryString == incumbent.secondaryString)
            {
                return true;
            }

            return false;
        }

        void SetAttackModifier(DFCareer.AttackModifier mod, string secondary)
        {
            switch (secondary)
            {
                case HardStrings.undead:
                    advantageData.UndeadAttackModifier = mod;
                    break;
                case HardStrings.daedra:
                    advantageData.DaedraAttackModifier = mod;
                    break;
                case HardStrings.humanoid:
                    advantageData.HumanoidAttackModifier = mod;
                    break;
                case HardStrings.animals:
                    advantageData.AnimalsAttackModifier = mod;
                    break;
                default:
                    break;
            }
        }

        void SetProficiency(DFCareer.Proficiency mod, string secondary)
        {
            switch (secondary)
            {
                case HardStrings.axe:
                    advantageData.Axes = mod;
                    switch (mod)
                    {
                        case DFCareer.Proficiency.Expert:
                            advantageData.ExpertProficiencies |= DFCareer.ProficiencyFlags.Axes;
                            break;
                        case DFCareer.Proficiency.Forbidden:
                            advantageData.ForbiddenProficiencies |= DFCareer.ProficiencyFlags.Axes;
                            break;
                        default:
                            break;
                    }
                    break;
                case HardStrings.bluntWeapon:
                    advantageData.BluntWeapons = mod;
                    switch (mod)
                    {
                        case DFCareer.Proficiency.Expert:
                            advantageData.ExpertProficiencies |= DFCareer.ProficiencyFlags.BluntWeapons;
                            break;
                        case DFCareer.Proficiency.Forbidden:
                            advantageData.ForbiddenProficiencies |= DFCareer.ProficiencyFlags.BluntWeapons;
                            break;
                        default:
                            break;
                    }
                    break;
                case HardStrings.handToHand:
                    advantageData.HandToHand = mod;
                    switch (mod)
                    {
                        case DFCareer.Proficiency.Expert:
                            advantageData.ExpertProficiencies |= DFCareer.ProficiencyFlags.HandToHand;
                            break;
                        case DFCareer.Proficiency.Forbidden:
                            advantageData.ForbiddenProficiencies |= DFCareer.ProficiencyFlags.HandToHand;
                            break;
                        default:
                            break;
                    }
                    break;
                case HardStrings.longBlade:
                    advantageData.LongBlades = mod;
                    switch (mod)
                    {
                        case DFCareer.Proficiency.Expert:
                            advantageData.ExpertProficiencies |= DFCareer.ProficiencyFlags.LongBlades;
                            break;
                        case DFCareer.Proficiency.Forbidden:
                            advantageData.ForbiddenProficiencies |= DFCareer.ProficiencyFlags.LongBlades;
                            break;
                        default:
                            break;
                    }
                    break;
                case HardStrings.missileWeapon:
                    advantageData.MissileWeapons = mod;
                    switch (mod)
                    {
                        case DFCareer.Proficiency.Expert:
                            advantageData.ExpertProficiencies |= DFCareer.ProficiencyFlags.MissileWeapons;
                            break;
                        case DFCareer.Proficiency.Forbidden:
                            advantageData.ForbiddenProficiencies |= DFCareer.ProficiencyFlags.MissileWeapons;
                            break;
                        default:
                            break;
                    }
                    break;
                case HardStrings.shortBlade:
                    advantageData.ShortBlades = mod;
                    switch (mod)
                    {
                        case DFCareer.Proficiency.Expert:
                            advantageData.ExpertProficiencies |= DFCareer.ProficiencyFlags.ShortBlades;
                            break;
                        case DFCareer.Proficiency.Forbidden:
                            advantageData.ForbiddenProficiencies |= DFCareer.ProficiencyFlags.ShortBlades;
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
        }

        void SetTolerance(DFCareer.Tolerance mod, string secondary)
        {
            switch (secondary)
            {
                case HardStrings.toDisease:
                    advantageData.Disease = mod;
                    break;
                case HardStrings.toFire:
                    advantageData.Fire = mod;
                    break;
                case HardStrings.toFrost:
                    advantageData.Frost = mod;
                    break;
                case HardStrings.toMagic:
                    advantageData.Magic = mod;
                    break;
                case HardStrings.toParalysis:
                    advantageData.Paralysis = mod;
                    break;
                case HardStrings.toPoison:
                    advantageData.Poison = mod;
                    break;
                case HardStrings.toShock:
                    advantageData.Shock = mod;
                    break;
                default:
                    break;
            }
        }

        void SetMagery(string secondary)
        {
            switch (secondary)
            {
                case HardStrings.intInSpellPoints15:
                    advantageData.SpellPointMultiplier = DFCareer.SpellPointMultipliers.Times_1_50;
                    advantageData.SpellPointMultiplierValue = 1.5f;
                    break;
                case HardStrings.intInSpellPoints175:
                    advantageData.SpellPointMultiplier = DFCareer.SpellPointMultipliers.Times_1_75;
                    advantageData.SpellPointMultiplierValue = 1.75f;
                    break;
                case HardStrings.intInSpellPoints2:
                    advantageData.SpellPointMultiplier = DFCareer.SpellPointMultipliers.Times_2_00;
                    advantageData.SpellPointMultiplierValue = 2f;
                    break;
                case HardStrings.intInSpellPoints3:
                    advantageData.SpellPointMultiplier = DFCareer.SpellPointMultipliers.Times_3_00;
                    advantageData.SpellPointMultiplierValue = 3f;
                    break;
                case HardStrings.intInSpellPoints:
                    advantageData.SpellPointMultiplier = DFCareer.SpellPointMultipliers.Times_1_00;
                    advantageData.SpellPointMultiplierValue = 1f;
                    break;
                default:
                    break;
            }
        }

        void SetRapidHealing(string secondary)
        {
            switch (secondary)
            {
                case HardStrings.general:
                    advantageData.RapidHealing = DFCareer.RapidHealingFlags.Always;
                    break;
                case HardStrings.inDarkness:
                    advantageData.RapidHealing = DFCareer.RapidHealingFlags.InDarkness;
                    break;
                case HardStrings.inLight:
                    advantageData.RapidHealing = DFCareer.RapidHealingFlags.InLight;
                    break;
                default:
                    break;
            }
        }

        void SetAbsorption(string secondary)
        {
            switch (secondary)
            {
                case HardStrings.general:
                    advantageData.SpellAbsorption = DFCareer.SpellAbsorptionFlags.Always;
                    break;
                case HardStrings.inDarkness:
                    advantageData.SpellAbsorption = DFCareer.SpellAbsorptionFlags.InDarkness;
                    break;
                case HardStrings.inLight:
                    advantageData.SpellAbsorption = DFCareer.SpellAbsorptionFlags.InLight;
                    break;
                default:
                    break;
            }
        }

        void SetRegeneration(string secondary)
        {
            switch (secondary)
            {
                case HardStrings.general:
                    advantageData.Regeneration = DFCareer.RegenerationFlags.Always;
                    break;
                case HardStrings.inDarkness:
                    advantageData.Regeneration = DFCareer.RegenerationFlags.InDarkness;
                    break;
                case HardStrings.inLight:
                    advantageData.Regeneration = DFCareer.RegenerationFlags.InLight;
                    break;
                case HardStrings.whileImmersed:
                    advantageData.Regeneration = DFCareer.RegenerationFlags.InWater;
                    break;
                default:
                    break;
            }
        }

        void SetDarknessMagery(string secondary)
        {
            switch(secondary)
            {
                case HardStrings.lowerMagicAbilityDaylight:
                    advantageData.DarknessPoweredMagery = DFCareer.DarknessMageryFlags.ReducedPowerInLight;
                    break;
                case HardStrings.unableToUseMagicInDaylight:
                    advantageData.DarknessPoweredMagery = DFCareer.DarknessMageryFlags.UnableToCastInLight;
                    break;
                default:
                    break;
            }
        }

        void SetForbiddenArmor(string secondary)
        {
            switch (secondary)
            {
                case HardStrings.chain:
                    advantageData.ForbiddenArmors = advantageData.ForbiddenArmors | DFCareer.ArmorFlags.Chain;
                    break;
                case HardStrings.leather:
                    advantageData.ForbiddenArmors = advantageData.ForbiddenArmors | DFCareer.ArmorFlags.Leather;
                    break;
                case HardStrings.plate:
                    advantageData.ForbiddenArmors = advantageData.ForbiddenArmors | DFCareer.ArmorFlags.Plate;
                    break;
                default:
                    break;
            }
        }

        void SetForbiddenMaterial(string secondary)
        {
            switch (secondary)
            {
                case HardStrings.adamantium:
                    advantageData.ForbiddenMaterials = advantageData.ForbiddenMaterials | DFCareer.MaterialFlags.Adamantium;
                    break;
                case HardStrings.daedric:
                    advantageData.ForbiddenMaterials = advantageData.ForbiddenMaterials | DFCareer.MaterialFlags.Daedric;
                    break;
                case HardStrings.dwarven:
                    advantageData.ForbiddenMaterials = advantageData.ForbiddenMaterials | DFCareer.MaterialFlags.Dwarven;
                    break;
                case HardStrings.ebony:
                    advantageData.ForbiddenMaterials = advantageData.ForbiddenMaterials | DFCareer.MaterialFlags.Ebony;
                    break;
                case HardStrings.elven:
                    advantageData.ForbiddenMaterials = advantageData.ForbiddenMaterials | DFCareer.MaterialFlags.Elven;
                    break;
                case HardStrings.iron:
                    advantageData.ForbiddenMaterials = advantageData.ForbiddenMaterials | DFCareer.MaterialFlags.Iron;
                    break;
                case HardStrings.mithril:
                    advantageData.ForbiddenMaterials = advantageData.ForbiddenMaterials | DFCareer.MaterialFlags.Mithril;
                    break;
                case HardStrings.orcish:
                    advantageData.ForbiddenMaterials = advantageData.ForbiddenMaterials | DFCareer.MaterialFlags.Orcish;
                    break;
                case HardStrings.silver:
                    advantageData.ForbiddenMaterials = advantageData.ForbiddenMaterials | DFCareer.MaterialFlags.Silver;
                    break;
                case HardStrings.steel:
                    advantageData.ForbiddenMaterials = advantageData.ForbiddenMaterials | DFCareer.MaterialFlags.Steel;
                    break;
                default:
                    break;
            }
        }

        void SetForbiddenShields(string secondary)
        {
            switch (secondary)
            {
                case HardStrings.buckler:
                    advantageData.ForbiddenShields = advantageData.ForbiddenShields | DFCareer.ShieldFlags.Buckler;
                    break;
                case HardStrings.kiteShield:
                    advantageData.ForbiddenShields = advantageData.ForbiddenShields | DFCareer.ShieldFlags.KiteShield;
                    break;
                case HardStrings.roundShield:
                    advantageData.ForbiddenShields = advantageData.ForbiddenShields | DFCareer.ShieldFlags.RoundShield;
                    break;
                case HardStrings.towerShield:
                    advantageData.ForbiddenShields = advantageData.ForbiddenShields | DFCareer.ShieldFlags.TowerShield;
                    break;
                default:
                    break;
            }
        }

        void SetLightMagery(string secondary)
        {
            switch (secondary)
            {
                case HardStrings.lowerMagicAbilityDarkness:
                    advantageData.LightPoweredMagery = DFCareer.LightMageryFlags.ReducedPowerInDarkness;
                    break;
                case HardStrings.unableToUseMagicInDarkness:
                    advantageData.LightPoweredMagery = DFCareer.LightMageryFlags.UnableToCastInDarkness;
                    break;
                default:
                    break;
            }
        }

        void SetDamage(string secondary)
        {
            switch (secondary)
            {
                case HardStrings.fromHolyPlaces:
                    advantageData.DamageFromHolyPlaces = true;
                    break;
                case HardStrings.fromSunlight:
                    advantageData.DamageFromSunlight = true;
                    break;
                default:
                    break;
            }
        }

        public void ParseCareerData()
        {
            foreach (SpecialAdvDis advDis in advDisList)
            {
                switch (advDis.primaryString)
                {
                    case HardStrings.bonusToHit:
                        SetAttackModifier(DFCareer.AttackModifier.Bonus, advDis.secondaryString);
                        break;
                    case HardStrings.phobia:
                        SetAttackModifier(DFCareer.AttackModifier.Phobia, advDis.secondaryString);
                        break;
                    case HardStrings.expertiseIn:
                        SetProficiency(DFCareer.Proficiency.Expert, advDis.secondaryString);
                        break;
                    case HardStrings.forbiddenWeaponry:
                        SetProficiency(DFCareer.Proficiency.Forbidden, advDis.secondaryString);
                        break;
                    case HardStrings.immunity:
                        SetTolerance(DFCareer.Tolerance.Immune, advDis.secondaryString);
                        break;
                    case HardStrings.resistance:
                        SetTolerance(DFCareer.Tolerance.Resistant, advDis.secondaryString);
                        break;
                    case HardStrings.criticalWeakness:
                        SetTolerance(DFCareer.Tolerance.CriticalWeakness, advDis.secondaryString);
                        break;
                    case HardStrings.increasedMagery:
                        SetMagery(advDis.secondaryString);
                        break;
                    case HardStrings.rapidHealing:
                        SetRapidHealing(advDis.secondaryString);
                        break;
                    case HardStrings.spellAbsorption:
                        SetAbsorption(advDis.secondaryString);
                        break;
                    case HardStrings.regenerateHealth:
                        SetRegeneration(advDis.secondaryString);
                        break;
                    case HardStrings.damage:
                        SetDamage(advDis.secondaryString);
                        break;
                    case HardStrings.darknessPoweredMagery:
                        SetDarknessMagery(advDis.secondaryString);
                        break;
                    case HardStrings.forbiddenArmorType:
                        SetForbiddenArmor(advDis.secondaryString);
                        break;
                    case HardStrings.forbiddenMaterial:
                        SetForbiddenMaterial(advDis.secondaryString);
                        break;
                    case HardStrings.forbiddenShieldTypes:
                        SetForbiddenShields(advDis.secondaryString);
                        break;
                    case HardStrings.lightPoweredMagery:
                        SetLightMagery(advDis.secondaryString);
                        break;
                    case HardStrings.lowTolerance:
                        SetTolerance(DFCareer.Tolerance.LowTolerance, advDis.secondaryString);
                        break;
                    case HardStrings.inabilityToRegen:
                        advantageData.NoRegenSpellPoints = true;
                        break;
                    case HardStrings.acuteHearing:
                        advantageData.AcuteHearing = true;
                        break;
                    case HardStrings.athleticism:
                        advantageData.Athleticism = true;
                        break;
                    case HardStrings.adrenalineRush:
                        advantageData.AdrenalineRush = true;
                        break;
                    default:
                        break;
                }
            }
        }

        void InitializeAdjustmentDict()
        {
            difficultyDict = new Dictionary<string, int> {
                { HardStrings.acuteHearing, 1 },
                { HardStrings.adrenalineRush, 4 },
                { HardStrings.athleticism, 4 },
                { HardStrings.bonusToHit + HardStrings.animals, 6 },
                { HardStrings.bonusToHit + HardStrings.daedra, 3 },
                { HardStrings.bonusToHit + HardStrings.humanoid, 6 },
                { HardStrings.bonusToHit + HardStrings.undead, 6 },
                { HardStrings.expertiseIn, 2 },
                { HardStrings.immunity, 10 },
                { HardStrings.increasedMagery + HardStrings.intInSpellPoints, 2 },
                { HardStrings.increasedMagery + HardStrings.intInSpellPoints15, 4 },
                { HardStrings.increasedMagery + HardStrings.intInSpellPoints175, 6 },
                { HardStrings.increasedMagery + HardStrings.intInSpellPoints2, 8 },
                { HardStrings.increasedMagery + HardStrings.intInSpellPoints3, 10 },
                { HardStrings.rapidHealing + HardStrings.general, 4 },
                { HardStrings.rapidHealing + HardStrings.inDarkness, 3 },
                { HardStrings.rapidHealing + HardStrings.inLight, 2 },
                { HardStrings.regenerateHealth + HardStrings.general, 14 },
                { HardStrings.regenerateHealth + HardStrings.inDarkness, 10 },
                { HardStrings.regenerateHealth + HardStrings.inLight, 6 },
                { HardStrings.regenerateHealth + HardStrings.whileImmersed, 2 },
                { HardStrings.resistance, 5 },
                { HardStrings.spellAbsorption + HardStrings.general, 14 },
                { HardStrings.spellAbsorption + HardStrings.inDarkness, 12 },
                { HardStrings.spellAbsorption + HardStrings.inLight, 8 },
                { HardStrings.criticalWeakness, -14 },
                { HardStrings.damage + HardStrings.fromHolyPlaces, -6 },
                { HardStrings.damage + HardStrings.fromSunlight, -10 },
                { HardStrings.darknessPoweredMagery + HardStrings.lowerMagicAbilityDaylight, -7 },
                { HardStrings.darknessPoweredMagery + HardStrings.unableToUseMagicInDaylight, -10 },
                { HardStrings.forbiddenArmorType + HardStrings.chain, -2 },
                { HardStrings.forbiddenArmorType + HardStrings.leather, -1 },
                { HardStrings.forbiddenArmorType + HardStrings.plate, -5 },
                { HardStrings.forbiddenMaterial + HardStrings.adamantium, -5 },
                { HardStrings.forbiddenMaterial + HardStrings.daedric, -2 },
                { HardStrings.forbiddenMaterial + HardStrings.dwarven, -7 },
                { HardStrings.forbiddenMaterial + HardStrings.ebony, -5 },
                { HardStrings.forbiddenMaterial + HardStrings.elven, -9 },
                { HardStrings.forbiddenMaterial + HardStrings.iron, -1 },
                { HardStrings.forbiddenMaterial + HardStrings.mithril, -6 },
                { HardStrings.forbiddenMaterial + HardStrings.orcish, -3 },
                { HardStrings.forbiddenMaterial + HardStrings.silver, -6 },
                { HardStrings.forbiddenMaterial + HardStrings.steel, -10 },
                { HardStrings.forbiddenShieldTypes, -1 },
                { HardStrings.forbiddenWeaponry, -2 },
                { HardStrings.inabilityToRegen, -14 },
                { HardStrings.lightPoweredMagery + HardStrings.lowerMagicAbilityDarkness, -10 },
                { HardStrings.lightPoweredMagery + HardStrings.unableToUseMagicInDarkness, -14 },
                { HardStrings.lowTolerance, -5 },
                { HardStrings.phobia, -4 }
            };
        }

        #endregion
    }    
}
