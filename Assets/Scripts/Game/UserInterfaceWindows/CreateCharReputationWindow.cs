// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
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
    /// Implements reputation adjustment window.
    /// </summary>
    public class CreateCharReputationWindow : DaggerfallPopupWindow
    {
        const string nativeImgName = "CUST03I0.IMG";
        const float barTopY = 24f;
        const float barBottomY = 128f;
        const float barMiddleY = 76f;
        const float barLength = 50f;
        const int strBalanceMustEqualZero = 303;

        Color greenBarColor = new Color(.388f, .549f, .223f);
        Color redBarColor = new Color(.580f, .031f, 0f);
        Vector2 repBarSize = new Vector2(5f, 0f);
        Texture2D nativeTexture;
        DaggerfallFont font;
        Panel repPanel = new Panel();
        short pointsToDistribute = 0;
        CreateCharCustomClass prevWindow;

        #region UI Rects

        Rect exitButtonRect = new Rect(129, 165, 33, 14);

        #endregion

        #region UI Panels

        Panel merchantsGreenPanel = new Panel();
        Panel merchantsRedPanel = new Panel();
        Panel peasantsGreenPanel = new Panel();
        Panel peasantsRedPanel = new Panel();
        Panel scholarsGreenPanel = new Panel();
        Panel scholarsRedPanel = new Panel();
        Panel nobilityGreenPanel = new Panel();
        Panel nobilityRedPanel = new Panel();
        Panel underworldGreenPanel = new Panel();
        Panel underworldRedPanel = new Panel();

        #endregion

        #region Buttons

        Button exitButton;

        #endregion

        #region Text Labels

        TextLabel merchantsPtsLabel;
        TextLabel peasantsPtsLabel;
        TextLabel scholarsPtsLabel;
        TextLabel nobilityPtsLabel;
        TextLabel underworldPtsLabel;
        TextLabel distributePtsLabel;

        #endregion

        public CreateCharReputationWindow(IUserInterfaceManager uiManager, IUserInterfaceWindow previous = null)
            : base(uiManager, previous)
        {
        }

        #region Setup Methods

        protected override void Setup()
        {
            if (IsSetup)
                return;

            base.Setup();

            prevWindow = (CreateCharCustomClass)this.PreviousWindow;

            // Load native texture
            nativeTexture = DaggerfallUI.GetTextureFromImg(nativeImgName);
            if (!nativeTexture)
                throw new Exception("CreateCharSpecialAdvantage: Could not load native texture.");

            // Create panel for window
            repPanel.Size = TextureReplacement.GetSize(nativeTexture, nativeImgName);
            repPanel.HorizontalAlignment = HorizontalAlignment.Center;
            repPanel.VerticalAlignment = VerticalAlignment.Middle;
            repPanel.BackgroundTexture = nativeTexture;
            repPanel.BackgroundTextureLayout = BackgroundLayout.StretchToFill;
            repPanel.OnMouseClick += RepPanel_OnMouseClick;
            NativePanel.Components.Add(repPanel);

            // Setup UI components
            font = DaggerfallUI.DefaultFont;
            exitButton = DaggerfallUI.AddButton(exitButtonRect, repPanel);
            exitButton.OnMouseClick += ExitButton_OnMouseClick;
            exitButton.ClickSound = DaggerfallUI.Instance.GetAudioClip(SoundClips.ButtonClick);

            // Setup adjustable bars
            SetupRepBars(merchantsGreenPanel, merchantsRedPanel, new Vector2(3f, 75f), new Vector2(3f, 77f), prevWindow.MerchantsRep);
            SetupRepBars(peasantsGreenPanel, peasantsRedPanel, new Vector2(36f, 75f), new Vector2(36f, 77f), prevWindow.PeasantsRep);
            SetupRepBars(scholarsGreenPanel, scholarsRedPanel, new Vector2(69f, 75f), new Vector2(69f, 77f), prevWindow.ScholarsRep);
            SetupRepBars(nobilityGreenPanel, nobilityRedPanel, new Vector2(102f, 75f), new Vector2(102f, 77f), prevWindow.NobilityRep);
            SetupRepBars(underworldGreenPanel, underworldRedPanel, new Vector2(135f, 75f), new Vector2(135f, 77f), prevWindow.UnderworldRep);

            // Setup text labels
            merchantsPtsLabel = DaggerfallUI.AddTextLabel(font, new Vector2(18f, 143f), prevWindow.MerchantsRep.ToString(), repPanel);
            peasantsPtsLabel = DaggerfallUI.AddTextLabel(font, new Vector2(50f, 143f), prevWindow.PeasantsRep.ToString(), repPanel);
            scholarsPtsLabel = DaggerfallUI.AddTextLabel(font, new Vector2(82f, 143f), prevWindow.ScholarsRep.ToString(), repPanel);
            nobilityPtsLabel = DaggerfallUI.AddTextLabel(font, new Vector2(114f, 143f), prevWindow.NobilityRep.ToString(), repPanel);
            underworldPtsLabel = DaggerfallUI.AddTextLabel(font, new Vector2(146f, 143f), prevWindow.UnderworldRep.ToString(), repPanel);
            distributePtsLabel = DaggerfallUI.AddTextLabel(font, new Vector2(64f, 173f), pointsToDistribute.ToString(), repPanel);

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

        void RepPanel_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (position.y >= barTopY && position.y <= barBottomY)
            {
                if (position.x >= 134f) // Underworld
                {
                    prevWindow.UnderworldRep = UpdateRep(position, underworldGreenPanel, underworldRedPanel, underworldPtsLabel);
                }
                else if (position.x >= 101f) // Nobility
                {
                    prevWindow.NobilityRep = UpdateRep(position, nobilityGreenPanel, nobilityRedPanel, nobilityPtsLabel);
                }
                else if (position.x >= 68f) // Scholars
                {
                    prevWindow.ScholarsRep = UpdateRep(position, scholarsGreenPanel, scholarsRedPanel, scholarsPtsLabel);
                }
                else if (position.x >= 35f) // Peasants
                {
                    prevWindow.PeasantsRep = UpdateRep(position, peasantsGreenPanel, peasantsRedPanel, peasantsPtsLabel);
                }
                else // Merchants
                {
                    prevWindow.MerchantsRep = UpdateRep(position, merchantsGreenPanel, merchantsRedPanel, merchantsPtsLabel);
                }
                UpdatePointsToDistribute();
                DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            }
        }

        void ExitButton_OnMouseClick(BaseScreenComponent sender, Vector2 pos)
        {
            if (pointsToDistribute != 0) 
            {
                DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, this);
                messageBox.SetTextTokens(strBalanceMustEqualZero);
                messageBox.ClickAnywhereToClose = true;
                messageBox.Show();
            }
            else 
            {
                CloseWindow();
            }
        }

        #endregion

        #region Helper methods

        void SetupRepBars(Panel greenBar, Panel redBar, Vector2 greenBarPos, Vector2 redBarPos, short val)
        {
            // Positive rep
            greenBar.Position = greenBarPos;
            greenBar.BackgroundColor = greenBarColor;
            greenBar.Size = repBarSize;
            repPanel.Components.Add(greenBar);

            // Negative rep
            redBar.Position = redBarPos;
            redBar.BackgroundColor = redBarColor;
            redBar.Size = repBarSize;
            repPanel.Components.Add(redBar);

            // Initialize value
            if (val > 0) 
            {
                greenBar.Enabled = true;
                redBar.Enabled = false;
                greenBar.Position = new Vector2(greenBar.Position.x, barMiddleY - (float)val * 5f);
                greenBar.Size = new Vector2(greenBar.Size.x, barMiddleY - greenBar.Position.y);
            }
            if (val < 0) 
            {
                redBar.Enabled = true;
                greenBar.Enabled = false;
                redBar.Size = new Vector2(redBar.Size.x, (float)val * -5f);
            }
        }

        int RoundNearestBarHeight(int number)
        {
            const int increment = 5;
            const int maxHeight = 50;
            int ret;

            if (number % increment > 3)
            {
                ret = 5 * ((number + (increment - 1)) / 5);
            } 
            else 
            {
                ret = increment * (number / increment);
            }

            return ret > maxHeight ? maxHeight : ret; // don't go over the top or bottom
        }

        short UpdateRep(Vector2 mousePos, Panel greenBar, Panel redBar, TextLabel label)
        {
            float clickedHeight = ((float)Math.Abs(barMiddleY - mousePos.y) / barLength) * barLength;
            int nearestHeight = RoundNearestBarHeight((int)clickedHeight);
            int sign = 1; // positive or negative
            short repVal;

            if (mousePos.y < barMiddleY)
            {
                greenBar.Enabled = true;
                redBar.Enabled = false;
                greenBar.Position = new Vector2(greenBar.Position.x, barMiddleY - (float)nearestHeight);
                greenBar.Size = new Vector2(repBarSize.x, barMiddleY - greenBar.Position.y);
            }
            if (mousePos.y > barMiddleY)
            {
                redBar.Enabled = true;
                greenBar.Enabled = false;
                redBar.Size = new Vector2(repBarSize.x, (float)nearestHeight);
                sign = -1;
            }

            // Update distributed points
            repVal = (short)(sign * nearestHeight / 5);
            label.Text = repVal.ToString();

            return repVal;
        }

        void UpdatePointsToDistribute()
        {
            pointsToDistribute = (short)(-prevWindow.MerchantsRep - prevWindow.PeasantsRep - prevWindow.ScholarsRep - prevWindow.NobilityRep - prevWindow.UnderworldRep);
            distributePtsLabel.Text = pointsToDistribute.ToString();
        }

        #endregion
    }    
}