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
    /// </summary>
    public class DaggerfallHUD : UserInterfaceWindow
    {
        float hudScale = 2.0f;
        float crosshairScale = 0.5f;

        HUDCrosshair crosshair = new HUDCrosshair();
        HUDVitals vitals = new HUDVitals();
        HUDCompass compass = new HUDCompass();
        GameObject player;
        DaggerfallEntityBehaviour playerEntity;

        public bool ShowCrosshair { get; set; }
        public bool ShowVitals { get; set; }
        public bool ShowCompass { get; set; }

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

            parentPanel.Components.Add(crosshair);
            parentPanel.Components.Add(vitals);
            parentPanel.Components.Add(compass);
        }

        public override void Update()
        {
            // Update UI visibility and scale
            crosshair.Enabled = ShowCrosshair;
            crosshair.CrosshairScale = CrosshairScale;
            vitals.Enabled = ShowVitals;
            vitals.VitalsScale = hudScale;
            compass.Enabled = ShowCompass;
            compass.CompassScale = hudScale;

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