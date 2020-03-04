// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2020 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Items;

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
        TextLabel arrowCountTextLabel = new TextLabel();
        HUDCrosshair crosshair = new HUDCrosshair();
        HUDVitals vitals = new HUDVitals();
        HUDCompass compass = new HUDCompass();
        HUDFlickerController flickerController = new HUDFlickerController();
        HUDInteractionModeIcon interactionModeIcon;
        HUDPlaceMarker placeMarker = new HUDPlaceMarker();
        EscortingNPCFacePanel escortingFaces = new EscortingNPCFacePanel();
        HUDQuestDebugger questDebugger = new HUDQuestDebugger();
        HUDActiveSpells activeSpells = new HUDActiveSpells();
        HUDLarge largeHUD = new HUDLarge();
        bool renderHUD = true;
        bool startupComplete = false;
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
        public bool ShowArrowCount { get; set; }

        public HUDLarge LargeHUD
        {
            get { return largeHUD; }
        }

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
            interactionModeIcon = new HUDInteractionModeIcon(crosshair);
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
            ShowArrowCount = DaggerfallUnity.Settings.EnableArrowCounter;

            // Get references
            //player = GameObject.FindGameObjectWithTag("Player");
            //playerEntity = player.GetComponent<DaggerfallEntityBehaviour>();

            ParentPanel.Components.Add(largeHUD);
            ParentPanel.Components.Add(crosshair);
            ParentPanel.Components.Add(vitals);
            ParentPanel.Components.Add(compass);
            ParentPanel.Components.Add(interactionModeIcon);
            ParentPanel.Components.Add(flickerController);
        }

        protected override void Setup()
        {
            largeHUD.Size = new Vector2(320, 46);
            largeHUD.HorizontalAlignment = HorizontalAlignment.Center;
            largeHUD.VerticalAlignment = VerticalAlignment.Bottom;
            largeHUD.AutoSize = AutoSizeModes.Scale;

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

            arrowCountTextLabel.TextColor = new Color(0.6f, 0.6f, 0.6f);
            arrowCountTextLabel.ShadowPosition = Vector2.zero;
            ParentPanel.Components.Add(arrowCountTextLabel);
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

            // Large HUD will force certain other HUD elements off as they conflict in space or utility
            bool largeHUDEnabled = false;//DaggerfallUnity.Settings.LargeHUD;
            if (largeHUDEnabled)
            {
                largeHUD.Enabled = true;
                vitals.Enabled = false;
                compass.Enabled = false;
                interactionModeIcon.Enabled = false;
            }
            else
            {
                largeHUD.Enabled = false;
            }

            // Scale HUD elements
            largeHUD.Scale = NativePanel.LocalScale * DaggerfallUnity.Settings.LargeHUDScale;
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

            // Update arrow count if player holding an unsheathed bow
            // TODO: Find a spot for arrow counter when large HUD enabled (remembering player could be in 320x200 retro mode)
            arrowCountTextLabel.Enabled = false;
            if (!largeHUDEnabled && ShowArrowCount && !GameManager.Instance.WeaponManager.Sheathed)
            {
                EquipSlots slot = DaggerfallUnity.Settings.BowLeftHandWithSwitching ? EquipSlots.LeftHand : EquipSlots.RightHand;
                DaggerfallUnityItem held = GameManager.Instance.PlayerEntity.ItemEquipTable.GetItem(slot);
                if (held != null && held.ItemGroup == ItemGroups.Weapons &&
                    (held.TemplateIndex == (int)Weapons.Long_Bow || held.TemplateIndex == (int)Weapons.Short_Bow))
                {
                    // Arrow count label position is offset to left of compass and centred relative to compass height
                    // This is done every frame to handle adaptive resolutions
                    Vector2 arrowLabelPos = compass.Position;
                    arrowLabelPos.x -= arrowCountTextLabel.TextWidth;
                    arrowLabelPos.y += compass.Size.y / 2 - arrowCountTextLabel.TextHeight / 2;

                    DaggerfallUnityItem arrows = GameManager.Instance.PlayerEntity.Items.GetItem(ItemGroups.Weapons, (int)Weapons.Arrow);
                    arrowCountTextLabel.Text = (arrows != null) ? arrows.stackCount.ToString() : "0";
                    arrowCountTextLabel.TextScale = NativePanel.LocalScale.x;
                    arrowCountTextLabel.Position = arrowLabelPos;
                    arrowCountTextLabel.Enabled = true;
                }
            }

            HotkeySequence.KeyModifiers keyModifiers = HotkeySequence.GetKeyboardKeyModifiers();
            // Cycle quest debugger state
            if (DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.DebuggerToggle).IsDownWith(keyModifiers))
            {
                questDebugger.NextState();
            }

            // Toggle HUD rendering
            if (DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.HUDToggle).IsDownWith(keyModifiers))
            {
                renderHUD = !renderHUD;
            }

            flickerController.NextCycle();

            // Don't display persistent HUD elements during initial startup
            // Prevents HUD elements being shown briefly at wrong size/scale at game start
            if (!startupComplete && !GameManager.Instance.IsPlayingGame())
            {
                largeHUD.Enabled = false;
                vitals.Enabled = false;
                crosshair.Enabled = false;
                compass.Enabled = false;
            }
            else
            {
                startupComplete = true;
            }

            base.Update();
        }

        public override void Draw()
        {
            if (renderHUD)
                base.Draw();
        }

        public void SetMidScreenText(string message, float delay = 1.5f)
        {
            // Set text and start timing
            midScreenTextLabel.Text = message;
            midScreenTextTimer = 0;
            midScreenTextDelay = delay;
            GameManager.Instance.PlayerEntity.Notebook.AddMessage(message);
        }
    }
}