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
        const string nativeImgName = "CUST01I0.IMG";
        const string nativeImgOverlayName = "CUST02I0.IMG";
        const int maxItems = 7;

        Texture2D nativeTexture;
        Texture2D nativeOverlayTexture;
        DaggerfallFont font;
        Panel advantagePanel = new Panel();
        Panel overlayPanel = new Panel();
        bool isDisadvantages;
        int labelCount = 0;

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

        TextLabel[] advantageLabels = new TextLabel[maxItems];

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
            for (int i = 0; i < maxItems; i++)
            {
                advantageLabels[i] = DaggerfallUI.AddTextLabel(font, new Vector2(8, 35 + i * 8), "", NativePanel);
                advantageLabels[i].OnMouseClick += AdvantageLabel_OnMouseClick;
                advantageLabels[i].Tag = i;
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

            if (labelCount == maxItems)
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
            labelCount++;
            int labelInd = labelCount - 1;
            advantageLabels[labelInd].Text = advantageName;
        }

        void AdvantageLabel_OnMouseClick(BaseScreenComponent sender, Vector2 pos)
        {
            // Remove item and update list
            labelCount--;
            for (int i = (int)sender.Tag; i < maxItems - 1; i++)
            {
                advantageLabels[i].Text = advantageLabels[i + 1].Text;
            }
            advantageLabels[maxItems - 1].Text = "";
        }

        void ExitButton_OnMouseClick(BaseScreenComponent sender, Vector2 pos)
        {
            CloseWindow();
        }

        #endregion
    }    
}