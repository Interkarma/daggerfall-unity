// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Daggerfall Workshop
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
        float hudScale = 2.0f;
        float crosshairScale = 0.5f;

        PopupText popupText = new PopupText();
        HUDCrosshair crosshair = new HUDCrosshair();
        HUDVitals vitals = new HUDVitals();
        HUDCompass compass = new HUDCompass();
        GameObject player;
        DaggerfallEntityBehaviour playerEntity;

        public bool ShowPopupText { get; set; }
        public bool ShowCrosshair { get; set; }
        public bool ShowVitals { get; set; }
        public bool ShowCompass { get; set; }

        public PopupText PopupText
        {
            get { return popupText; }
        }

        /// <summary>
        /// Set scale of UI components, except crosshair.
        /// </summary>
        public float HUDScale
        {
            get { return hudScale; }
            set { hudScale = value; }
        }

        public float CrosshairScale
        {
            get { return crosshairScale; }
            set { crosshairScale = value; }
        }

        public DaggerfallHUD(IUserInterfaceManager uiManager)
            :base(uiManager)
        {
            parentPanel.BackgroundColor = Color.clear;
            ShowPopupText = true;
            ShowCrosshair = true;
            ShowVitals = true;
            ShowCompass = true;

            // Get references
            player = GameObject.FindGameObjectWithTag("Player");
            playerEntity = player.GetComponent<DaggerfallEntityBehaviour>();

            // Auto-set initial scale based on viewport size
            if (Screen.currentResolution.height >= 1080 && Screen.currentResolution.height < 1440)
                hudScale = 3.0f;
            if (Screen.currentResolution.height >= 1440)
                hudScale = 4.0f;

            ParentPanel.Components.Add(crosshair);
            ParentPanel.Components.Add(vitals);
            ParentPanel.Components.Add(compass);
        }

        protected override void Setup()
        {
            popupText.Size = NativePanel.Size;
            NativePanel.Components.Add(popupText);
        }

        public override void Update()
        {
            // Update HUD visibility
            popupText.Enabled = ShowPopupText;
            crosshair.Enabled = ShowCrosshair;
            vitals.Enabled = ShowVitals;
            compass.Enabled = ShowCompass;

            // Scale HUD elements
            compass.Scale = NativePanel.LocalScale;
            vitals.Scale = NativePanel.LocalScale;
            crosshair.CrosshairScale = CrosshairScale;

            // Adjust vitals based on current player state
            if (playerEntity)
            {
                PlayerEntity entity = playerEntity.Entity as PlayerEntity;
                vitals.Health = (float)entity.CurrentHealth / (float)entity.MaxHealth;
                vitals.Fatigue = (float)entity.CurrentFatigue / (float)entity.MaxFatigue;
                vitals.Magicka = (float)entity.CurrentMagicka / (float)entity.MaxMagicka;
            }

            base.Update();
        }
    }
}