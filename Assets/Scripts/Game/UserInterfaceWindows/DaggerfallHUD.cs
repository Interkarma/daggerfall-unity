// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using System;
using System.IO;
using System.Collections;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Implements heads-up-display for default view mode.
    /// TODO: Migrate to full DaggerfallBaseWindow setup
    /// </summary>
    public class DaggerfallHUD : DaggerfallBaseWindow
    {
        float crosshairScale = 0.75f;

        PopupText popupText = new PopupText();
        TextLabel midScreenTextLabel = new TextLabel();
        HUDCrosshair crosshair = new HUDCrosshair();
        HUDVitals vitals = new HUDVitals();
        HUDCompass compass = new HUDCompass();
        HUDFlickerController flickerController = new HUDFlickerController();
        HUDInteractionModeIcon interactionModeIcon = new HUDInteractionModeIcon();
        HUDPlaceMarker placeMarker = new HUDPlaceMarker();
        EscortingNPCFacePanel escortingFaces = new EscortingNPCFacePanel();
        HUDQuestDebugger questDebugger = new HUDQuestDebugger();
        HUDActiveSpells activeSpells = new HUDActiveSpells();
        //GameObject player;
        //DaggerfallEntityBehaviour playerEntity;

        float midScreenTextTimer = -1;
        float midScreenTextDelay = 1.5f;

        public bool ShowPopupText { get; set; }
        public bool ShowMidScreenText { get; set; }
        public bool ShowCrosshair { get; set; }
        public bool ShowVitals { get; set; }
        public bool ShowCompass { get; set; }
        public bool ShowInteractionModeIcon { get; set; }
        public bool ShowLocalQuestPlaces { get; set; }
        public bool ShowEscortingFaces { get; set; }
        public bool ShowActiveSpells { get; set; }

        public PopupText PopupText
        {
            get { return popupText; }
        }

        public float CrosshairScale
        {
            get { return crosshairScale; }
            set { crosshairScale = value; }
        }

        public HUDVitals HUDVitals
        {
            get { return vitals; }
        }

        public HUDCompass HUDCompass
        {
            get { return compass; }
        }

        public EscortingNPCFacePanel EscortingFaces
        {
            get { return escortingFaces; }
        }

        public HUDPlaceMarker PlaceMarker
        {
            get { return placeMarker; }
        }

        public HUDQuestDebugger QuestDebugger
        {
            get { return questDebugger; }
        }

        public HUDActiveSpells ActiveSpells
        {
            get { return activeSpells; }
        }

        public DaggerfallHUD(IUserInterfaceManager uiManager)
            :base(uiManager)
        {
            parentPanel.BackgroundColor = Color.clear;
            ShowPopupText = true;
            ShowMidScreenText = true;
            ShowCrosshair = DaggerfallUnity.Settings.Crosshair;
            ShowVitals = true;
            ShowCompass = true;
            ShowInteractionModeIcon = DaggerfallUnity.Settings.InteractionModeIcon.ToLower() != "none";
            ShowEscortingFaces = true;
            ShowLocalQuestPlaces = true;
            ShowActiveSpells = true;

            // Get references
            //player = GameObject.FindGameObjectWithTag("Player");
            //playerEntity = player.GetComponent<DaggerfallEntityBehaviour>();

            ParentPanel.Components.Add(crosshair);
            ParentPanel.Components.Add(vitals);
            ParentPanel.Components.Add(compass);
            ParentPanel.Components.Add(interactionModeIcon);
            ParentPanel.Components.Add(flickerController);
        }

        protected override void Setup()
        {
            activeSpells.Size = NativePanel.Size;
            activeSpells.HorizontalAlignment = HorizontalAlignment.Center;
            NativePanel.Components.Add(activeSpells);

            popupText.Size = NativePanel.Size;
            NativePanel.Components.Add(popupText);

            midScreenTextLabel.HorizontalAlignment = HorizontalAlignment.Center;
            midScreenTextLabel.Position = new Vector2(0, 146);
            NativePanel.Components.Add(midScreenTextLabel);

            placeMarker.Size = new Vector2(640, 400);
            placeMarker.AutoSize = AutoSizeModes.ScaleToFit;
            ParentPanel.Components.Add(placeMarker);

            escortingFaces.Size = NativePanel.Size;
            escortingFaces.AutoSize = AutoSizeModes.ScaleToFit;
            ParentPanel.Components.Add(escortingFaces);

            questDebugger.Size = new Vector2(640, 400);
            questDebugger.AutoSize = AutoSizeModes.ScaleToFit;
            ParentPanel.Components.Add(questDebugger);
        }

        public override void Update()
        {
            // Update HUD visibility
            popupText.Enabled = ShowPopupText;
            midScreenTextLabel.Enabled = ShowMidScreenText;
            crosshair.Enabled = ShowCrosshair;
            vitals.Enabled = ShowVitals;
            compass.Enabled = ShowCompass;
            interactionModeIcon.Enabled = ShowInteractionModeIcon;
            placeMarker.Enabled = ShowLocalQuestPlaces;
            escortingFaces.EnableBorder = ShowEscortingFaces;
            questDebugger.Enabled = !(questDebugger.State == HUDQuestDebugger.DisplayState.Nothing);
            activeSpells.Enabled = ShowActiveSpells;

            // Scale HUD elements
            compass.Scale = NativePanel.LocalScale;
            vitals.Scale = NativePanel.LocalScale;
            crosshair.CrosshairScale = CrosshairScale;
            interactionModeIcon.Scale = NativePanel.LocalScale;

            // Align compass to screen panel
            Rect screenRect = ParentPanel.Rectangle;
            float compassX = screenRect.width - (compass.Size.x);
            float compassY = screenRect.height - (compass.Size.y);
            compass.Position = new Vector2(compassX, compassY);

            // Update midscreen text timer and remove once complete
            if (midScreenTextTimer != -1)
            {
                midScreenTextTimer += Time.deltaTime;
                if (midScreenTextTimer > midScreenTextDelay)
                {
                    midScreenTextTimer = -1;
                    midScreenTextLabel.Text = string.Empty;
                }
            }

            // Cycle quest debugger state
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Tab))
            {
                questDebugger.NextState();
            }

            flickerController.NextCycle();

            base.Update();
        }

        public void SetMidScreenText(string message, float delay = 1.5f)
        {
            // Set text and start timing
            midScreenTextLabel.Text = message;
            midScreenTextTimer = 0;
            midScreenTextDelay = delay;
        }
    }
}