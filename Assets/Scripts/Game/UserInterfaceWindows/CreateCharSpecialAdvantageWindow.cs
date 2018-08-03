// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
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
        struct SpecialAdvDis 
        {
            public string primaryString;
            public string secondaryString;
        };

        const string nativeImgName = "CUST01I0.IMG";
        const string nativeImgOverlayName = "CUST02I0.IMG";
        const int maxItems = 7;
        const int maxLabels = maxItems * 2;

        List<SpecialAdvDis> advDisList = new List<SpecialAdvDis>();

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

        public CreateCharSpecialAdvantageWindow(IUserInterfaceManager uiManager, IUserInterfaceWindow previous = null, bool isDisadvantages = false)
            : base(uiManager, previous)
        {
            this.isDisadvantages = isDisadvantages;
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
            exitButton = DaggerfallUI.AddButton(exitButtonRect, NativePanel);
            exitButton.OnMouseClick += ExitButton_OnMouseClick;
            for (int i = 0; i < maxLabels; i++)
            {
                advantageLabels[i] = DaggerfallUI.AddTextLabel(font, new Vector2(8, 35 + i * 8), string.Empty, NativePanel);
                advantageLabels[i].OnMouseClick += AdvantageLabel_OnMouseClick;
                advantageLabels[i].Tag = -1;
            }

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

            DaggerfallListPickerWindow advantagePicker = new DaggerfallListPickerWindow(uiManager, this);
            advantagePicker.ListBox.Font = DaggerfallUI.SmallFont;
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
            advDisList.Add(s);
            if (secondaryList == null)
            {
                UpdateLabels();
            } 
            else
            {
                DaggerfallListPickerWindow secondaryPicker = new DaggerfallListPickerWindow(uiManager, this);
                secondaryPicker.ListBox.Font = DaggerfallUI.SmallFont;
                secondaryPicker.OnItemPicked += SecondaryPicker_OnItemPicked;
                foreach (string secondaryString in secondaryList)
                {
                    secondaryPicker.ListBox.AddItem(secondaryString);
                }
                uiManager.PushWindow(secondaryPicker);
            }
        }

        void SecondaryPicker_OnItemPicked(int index, string itemString)
        {
            CloseWindow();
            string primary = advDisList[advDisList.Count - 1].primaryString;
            advDisList[advDisList.Count - 1] = new SpecialAdvDis { primaryString = primary, secondaryString = itemString};
            UpdateLabels();
        }

        void AdvantageLabel_OnMouseClick(BaseScreenComponent sender, Vector2 pos)
        {
            for (int i = 0; i < advDisList.Count; i++)
            {
                if (i == (int)sender.Tag)
                {
                    advDisList.RemoveAt(i);
                    sender.Tag = -1;
                    UpdateLabels();
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
                }
            }
        }

        #endregion
    }    
}