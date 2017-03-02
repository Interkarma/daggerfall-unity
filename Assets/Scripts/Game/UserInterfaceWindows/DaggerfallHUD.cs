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

        public float CrosshairScale
        {
            get { return crosshairScale; }
            set { crosshairScale = value; }
        }

        public HUDVitals HUDVitals
        {
            get { return vitals; }
        }

        public DaggerfallHUD(IUserInterfaceManager uiManager)
            :base(uiManager)
        {
            parentPanel.BackgroundColor = Color.clear;
            ShowPopupText = true;
            ShowCrosshair = DaggerfallUnity.Settings.Crosshair;
            ShowVitals = true;
            ShowCompass = true;

            // Get references
            player = GameObject.FindGameObjectWithTag("Player");
            playerEntity = player.GetComponent<DaggerfallEntityBehaviour>();

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

            // Align compass to screen panel
            Rect screenRect = ParentPanel.Rectangle;
            float compassX = screenRect.width - (compass.Size.x);
            float compassY = screenRect.height - (compass.Size.y);
            compass.Position = new Vector2(compassX, compassY);

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