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
using DaggerfallWorkshop.Game.Formulas;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    public class DaggerfallRestWindow : DaggerfallPopupWindow
    {
        #region UI Rects

        Rect whileButtonRect = new Rect(4, 13, 48, 24);
        Rect healedButtonRect = new Rect(53, 13, 48, 24);
        Rect loiterButtonRect = new Rect(102, 13, 48, 24);
        Rect counterPanelRect = new Rect(0, 50, 105, 41);
        Rect counterTextPanelRect = new Rect(4, 10, 16, 8);
        Rect stopButtonRect = new Rect(33, 26, 40, 10);

        #endregion

        #region UI Controls

        Button whileButton;
        Button healedButton;
        Button loiterButton;
        Button stopButton;

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
        int totalHours = 0;
        float waitTimer = 0;

        PlayerEntity playerEntity;
        DaggerfallHUD hud;

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
            mainPanel.Size = new Vector2(ImageReader.GetImageData("REST00I0.IMG", 0, 0, false, false).width, ImageReader.GetImageData("REST00I0.IMG", 0, 0, false, false).height);

            NativePanel.Components.Add(mainPanel);

            // Create buttons
            whileButton = DaggerfallUI.AddButton(whileButtonRect, mainPanel);
            whileButton.OnMouseClick += WhileButton_OnMouseClick;
            healedButton = DaggerfallUI.AddButton(healedButtonRect, mainPanel);
            healedButton.OnMouseClick += HealedButton_OnMouseClick;
            loiterButton = DaggerfallUI.AddButton(loiterButtonRect, mainPanel);
            loiterButton.OnMouseClick += LoiterButton_OnMouseClick;

            // Setup counter panel
            counterPanel.Position = new Vector2(counterPanelRect.x, counterPanelRect.y);
            counterPanel.Size = new Vector2(counterPanelRect.width, counterPanelRect.height);
            counterPanel.HorizontalAlignment = HorizontalAlignment.Center;
            counterPanel.Enabled = false;
            NativePanel.Components.Add(counterPanel);

            // Setup counter text
            Panel counterTextPanel = DaggerfallUI.AddPanel(counterTextPanelRect, counterPanel);
            counterLabel.Position = new Vector2(0, 2);
            counterLabel.HorizontalAlignment = HorizontalAlignment.Center;
            counterTextPanel.Components.Add(counterLabel);

            // Stop button
            stopButton = DaggerfallUI.AddButton(stopButtonRect, counterPanel);
            stopButton.OnMouseClick += StopButton_OnMouseClick;
        }

        #endregion

        #region Overrides

        public override void Update()
        {
            base.Update();

            // Update HUD
            if (hud != null)
            {
                hud.Update();
            }

            ShowStatus();
            if (currentRestMode != RestModes.Selection)
            {
                if (TickRest())
                    EndRest();
            }
        }

        public override void Draw()
        {
            base.Draw();

            // Draw vitals
            if (hud != null)
            {
                hud.HUDVitals.Draw();
            }
        }

        public override void OnPush()
        {
            base.OnPush();

            // Reset counters
            hoursRemaining = 0;
            totalHours = 0;
            waitTimer = 0;

            // Get references
            playerEntity = GameManager.Instance.PlayerEntity;
            hud = DaggerfallUI.Instance.DaggerfallHUD;
        }

        public override void OnPop()
        {
            base.OnPop();

            // Progress world time
            DaggerfallUnity.WorldTime.Now.RaiseTime(totalHours * DaggerfallDateTime.SecondsPerHour);
            Debug.Log(string.Format("Resting raised time by {0} hours", totalHours));
            playerEntity.RaiseSkills();
        }

        #endregion

        #region Private Methods

        void LoadTextures()
        {
            baseTexture = ImageReader.GetTexture(baseTextureName);
            hoursPastTexture = ImageReader.GetTexture(hoursPastTextureName);
            hoursRemainingTexture = ImageReader.GetTexture(hoursRemainingTextureName);
        }

        void ShowStatus()
        {
            // Display status based on current rest state
            if (currentRestMode == RestModes.Selection)
            {
                mainPanel.Enabled = true;
                counterPanel.Enabled = false;
            }
            else if (currentRestMode == RestModes.FullRest)
            {
                mainPanel.Enabled = false;
                counterPanel.Enabled = true;
                counterPanel.BackgroundTexture = hoursPastTexture;
                counterLabel.Text = totalHours.ToString();
            }
            else if (currentRestMode == RestModes.TimedRest)
            {
                mainPanel.Enabled = false;
                counterPanel.Enabled = true;
                counterPanel.BackgroundTexture = hoursRemainingTexture;
                counterLabel.Text = hoursRemaining.ToString();
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
            // Loitering runs at a slower rate to rest
            float waitTime = (currentRestMode == RestModes.Loiter) ? loiterWaitTimePerHour : restWaitTimePerHour;

            // Tick timer by rate and count based on rest type
            bool finished = false;
            if (Time.realtimeSinceStartup > waitTimer + waitTime)
            {
                totalHours++;
                waitTimer = Time.realtimeSinceStartup;
                if (currentRestMode == RestModes.TimedRest)
                {
                    TickVitals();
                    hoursRemaining--;
                    if (hoursRemaining < 1)
                        finished = true;
                }
                else if (currentRestMode == RestModes.FullRest)
                {
                    if (TickVitals())
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
            const int youWakeUpTextId = 353;
            const int youAreHealedTextId = 350;
            const int finishedLoiteringTextId = 349;

            if (currentRestMode == RestModes.TimedRest)
            {
                DaggerfallMessageBox mb = DaggerfallUI.MessageBox(youWakeUpTextId);
                mb.OnClose += RestFinishedPopup_OnClose;
                currentRestMode = RestModes.Selection;
            }
            else if (currentRestMode == RestModes.FullRest)
            {
                DaggerfallMessageBox mb = DaggerfallUI.MessageBox(youAreHealedTextId);
                mb.OnClose += RestFinishedPopup_OnClose;
                currentRestMode = RestModes.Selection;
            }
            else if (currentRestMode == RestModes.Loiter)
            {
                DaggerfallMessageBox mb = DaggerfallUI.MessageBox(finishedLoiteringTextId);
                mb.OnClose += RestFinishedPopup_OnClose;
                currentRestMode = RestModes.Selection;
            }
        }

        bool TickVitals()
        {
            int healthRecoveryRate = FormulaHelper.CalculateHealthRecoveryRate(playerEntity.Skills.Medical, playerEntity.Stats.Endurance, playerEntity.MaxHealth);
            int fatigueRecoveryRate = FormulaHelper.CalculateFatigueRecoveryRate(playerEntity.MaxFatigue);
            int spellPointRecoveryRate = FormulaHelper.CalculateSpellPointRecoveryRate(playerEntity.MaxMagicka);

            playerEntity.CurrentHealth += healthRecoveryRate;
            playerEntity.CurrentFatigue += fatigueRecoveryRate;
            playerEntity.CurrentMagicka += spellPointRecoveryRate;

            playerEntity.TallySkill((short)Skills.Medical, 1);

            // Check if player fully healed
            // Will eventually need to tailor check for character
            // For example, sorcerers cannot recover magicka from resting
            if (playerEntity.CurrentHealth == playerEntity.MaxHealth &&
                playerEntity.CurrentFatigue == playerEntity.MaxFatigue &&
                playerEntity.CurrentMagicka == playerEntity.MaxMagicka)
            {
                return true;
            }

            return false;
        }

        #endregion

        #region Event Handlers

        private void WhileButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallInputMessageBox mb = new DaggerfallInputMessageBox(uiManager, this);
            mb.SetTextBoxLabel(HardStrings.restHowManyHours);
            mb.TextPanelDistanceX = 9;
            mb.TextPanelDistanceY = 8;
            mb.TextBox.Text = "0";
            mb.TextBox.Numeric = true;
            mb.TextBox.MaxCharacters = 8;
            mb.TextBox.WidthOverride = 286;
            mb.OnGotUserInput += TimedRestPrompt_OnGotUserInput;
            mb.Show();
        }

        private void HealedButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            waitTimer = Time.realtimeSinceStartup;
            currentRestMode = RestModes.FullRest;
        }

        private void LoiterButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallInputMessageBox mb = new DaggerfallInputMessageBox(uiManager, this);
            mb.SetTextBoxLabel(HardStrings.loiterHowManyHours);
            mb.TextPanelDistanceX = 5;
            mb.TextPanelDistanceY = 8;
            mb.TextBox.Text = "0";
            mb.TextBox.Numeric = true;
            mb.TextBox.MaxCharacters = 8;
            mb.TextBox.WidthOverride = 286;
            mb.OnGotUserInput += LoiterPrompt_OnGotUserInput;
            mb.Show();
        }

        private void StopButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PopToHUD();
        }

        private void RestFinishedPopup_OnClose()
        {
            DaggerfallUI.Instance.PopToHUD();
        }

        #endregion

        #region Rest Events

        private void TimedRestPrompt_OnGotUserInput(DaggerfallInputMessageBox sender, string input)
        {
            const int cannotRestMoreThan99HoursTextId = 26;

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
            else if (time > 99)
            {
                DaggerfallUI.MessageBox(cannotRestMoreThan99HoursTextId);
                return;
            }

            hoursRemaining = time;
            waitTimer = Time.realtimeSinceStartup;
            currentRestMode = RestModes.TimedRest;
        }

        private void LoiterPrompt_OnGotUserInput(DaggerfallInputMessageBox sender, string input)
        {
            const int cannotLoiterMoreThan3HoursTextId = 27;

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
                DaggerfallUI.MessageBox(cannotLoiterMoreThan3HoursTextId);
                return;
            }

            hoursRemaining = time;
            waitTimer = Time.realtimeSinceStartup;
            currentRestMode = RestModes.Loiter;
        }

        #endregion
    }
}