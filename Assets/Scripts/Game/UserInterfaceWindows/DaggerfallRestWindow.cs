// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2016 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallConnect.Arena2;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    public class DaggerfallRestWindow : DaggerfallPopupWindow
    {
        #region Classic Text IDs
        #endregion

        #region UI Rects

        Rect whileButtonRect = new Rect(87, 63, 48, 24);
        Rect healedButtonRect = new Rect(136, 63, 48, 24);
        Rect loiterButtonRect = new Rect(185, 63, 48, 24);
        Rect counterPanelRect = new Rect(0, 50, 105, 41);

        Vector2 counterTextPos = new Vector2(10, 12);

        #endregion

        #region UI Controls

        Button whileButton;
        Button healedButton;
        Button loiterButton;

        HUDVitals vitals = new HUDVitals();
        Panel mainPanel = new Panel();
        Panel counterPanel = new Panel();

        TextLabel counterLabel = new TextLabel();

        #endregion

        #region UI Textures

        Texture2D baseTexture;
        Texture2D hoursPastTexture;
        Texture2D hoursRemainingTexture;

        #endregion

        #region Fields

        const string baseTextureName = "REST00I0.IMG";              // Rest type
        const string hoursPastTextureName = "REST01I0.IMG";         // "Hours past"
        const string hoursRemainingTextureName = "REST02I0.IMG";    // "Hours remaining"

        const float restWaitTimePerHour = 0.75f;
        const float loiterWaitTimePerHour = 1.25f;

        RestModes currentRestMode = RestModes.Selection;
        int hoursRemaining = 0;
        int hoursPast = 0;
        int totalHours = 0;
        float waitTimer = 0;

        #endregion

        #region Enums

        enum RestModes
        {
            Selection,
            TimedRest,
            FullRest,
            Loiter,
        }

        #endregion

        #region Constructors

        public DaggerfallRestWindow(IUserInterfaceManager uiManager)
            : base(uiManager)
        {
        }

        #endregion

        #region Setup Methods

        protected override void Setup()
        {
            // Load all the textures used by rest interface
            LoadTextures();

            // Hide world while resting
            ParentPanel.BackgroundColor = Color.black;

            // Create interface panel
            mainPanel.HorizontalAlignment = HorizontalAlignment.Center;
            mainPanel.BackgroundTexture = baseTexture;
            mainPanel.Position = new Vector2(0, 50);
            mainPanel.Size = new Vector2(baseTexture.width, baseTexture.height);
            NativePanel.Components.Add(mainPanel);

            // Create buttons
            whileButton = DaggerfallUI.AddButton(whileButtonRect, NativePanel);
            whileButton.OnMouseClick += WhileButton_OnMouseClick;
            healedButton = DaggerfallUI.AddButton(healedButtonRect, NativePanel);
            healedButton.OnMouseClick += HealedButton_OnMouseClick;
            loiterButton = DaggerfallUI.AddButton(loiterButtonRect, NativePanel);
            loiterButton.OnMouseClick += LoiterButton_OnMouseClick;

            // Create vitals
            ParentPanel.Components.Add(vitals);
            UpdateVitals();

            // Setup counter panel
            counterPanel.Position = new Vector2(counterPanelRect.x, counterPanelRect.y);
            counterPanel.Size = new Vector2(counterPanelRect.width, counterPanelRect.height);
            counterPanel.HorizontalAlignment = HorizontalAlignment.Center;
            counterPanel.Enabled = false;
            NativePanel.Components.Add(counterPanel);

            // Setup counter text
            counterLabel.Position = counterTextPos;
            counterLabel.Text = "3";
            counterPanel.Components.Add(counterLabel);
        }

        #endregion

        #region Overrides

        public override void Update()
        {
            base.Update();

            UpdateVitals();

            ShowStatus();
            if (currentRestMode != RestModes.Selection)
            {
                if (TickRest())
                    EndRest();
            }
        }

        public override void OnPush()
        {
            base.OnPush();

            // Reset counters
            hoursRemaining = 0;
            hoursPast = 0;
            totalHours = 0;
            waitTimer = 0;
        }

        public override void OnPop()
        {
            base.OnPop();

            // Progress world time
            DaggerfallUnity.WorldTime.Now.RaiseTime(totalHours * DaggerfallDateTime.SecondsPerHour);
            Debug.Log(string.Format("Resting raised time by {0} hours", totalHours));
        }

        #endregion

        #region Private Methods

        void LoadTextures()
        {
            baseTexture = ImageReader.GetTexture(baseTextureName);
            hoursPastTexture = ImageReader.GetTexture(hoursPastTextureName);
            hoursRemainingTexture = ImageReader.GetTexture(hoursRemainingTextureName);
        }

        void UpdateVitals()
        {
            // Scale vitals elements
            vitals.Scale = NativePanel.LocalScale;

            // Adjust vitals based on current player state
            PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;
            if (playerEntity != null)
            {
                vitals.Health = (float)playerEntity.CurrentHealth / (float)playerEntity.MaxHealth;
                vitals.Fatigue = (float)playerEntity.CurrentFatigue / (float)playerEntity.MaxFatigue;
                vitals.Magicka = (float)playerEntity.CurrentMagicka / (float)playerEntity.MaxMagicka;
            }
        }

        void ShowStatus()
        {
            if (currentRestMode == RestModes.Selection)
            {
                mainPanel.Enabled = true;
                counterPanel.Enabled = false;
            }
            else if (currentRestMode == RestModes.Loiter)
            {
                mainPanel.Enabled = false;
                counterPanel.Enabled = true;
                counterPanel.BackgroundTexture = hoursRemainingTexture;
                counterLabel.Text = hoursRemaining.ToString();
            }
        }

        bool TickRest()
        {
            bool finished = false;
            if (Time.realtimeSinceStartup > waitTimer + loiterWaitTimePerHour)
            {
                totalHours++;
                waitTimer = Time.realtimeSinceStartup;
                if (currentRestMode == RestModes.TimedRest)
                {
                    hoursPast++;
                    if (hoursPast > totalHours)
                        finished = true;
                }
                else if (currentRestMode == RestModes.Loiter)
                {
                    hoursRemaining--;
                    if (hoursRemaining < 1)
                        finished = true;
                }
            }

            return finished;
        }

        void EndRest()
        {
            const int finishedLoitering = 349;

            if (currentRestMode == RestModes.Loiter)
            {
                DaggerfallMessageBox mb = DaggerfallUI.MessageBox(finishedLoitering);
                mb.OnClose += RestFinishedPopup_OnClose;
                currentRestMode = RestModes.Selection;
            }
        }

        #endregion

        #region Event Handlers

        private void WhileButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            Debug.Log("Clicked while button");
        }

        private void HealedButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            Debug.Log("Clicked healed button");
        }

        private void LoiterButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallInputMessageBox mb = new DaggerfallInputMessageBox(uiManager, this);
            mb.SetTextBoxLabel(HardStrings.loiterHowManyHours);
            mb.TextPanelDistance = 0;
            mb.TextBox.Text = "0";
            mb.TextBox.Numeric = true;
            mb.OnGotUserInput += LoiterPrompt_OnGotUserInput;
            mb.Show();
        }

        private void RestFinishedPopup_OnClose()
        {
            DaggerfallUI.Instance.PopToHUD();
        }

        #endregion

        #region Loiter Events

        private void LoiterPrompt_OnGotUserInput(DaggerfallInputMessageBox sender, string input)
        {
            const int cannotLoiterMoreThan3Hours = 27;

            // Validate input
            int time = 0;
            bool result = int.TryParse(input, out time);
            if (!result)
                return;

            // Validate range
            if (time < 0)
            {
                time = 0;
            }
            else if (time > 3)
            {
                DaggerfallUI.MessageBox(cannotLoiterMoreThan3Hours);
                return;
            }

            hoursRemaining = time;
            waitTimer = Time.realtimeSinceStartup;
            currentRestMode = RestModes.Loiter;
        }

        #endregion
    }
}