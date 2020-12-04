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
using DaggerfallConnect.Utility;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects;

namespace DaggerfallWorkshop.Game.UserInterface
{
    /// <summary>
    /// Implements the large HUD in Daggerfall Unity as just another overlay inside of primary HUD.
    /// This is so it can work uniformly in both widescreen and retro modes.
    /// Other HUD elements can still work alongside large HUD, but some will be disabled as they occupy same screen area.
    /// </summary>
    public class HUDLarge : Panel
    {
        const string mainFilename = "MAIN00I0.IMG";
        const string interactionModesFilename = "MAIN01I0.IMG";
        const string compass0Filename = "CMPA00I0.BSS";                 // Standard compass
        const string compass1Filename = "CMPA01I0.BSS";                 // Blue compass (unused)
        const string compass2Filename = "CMPA02I0.BSS";                 // Red compass (unused)
        const string mainColorBackgroundFilename = "MCOL00I0.CIF";      // Color backgrounds for portrait and vitals
        const int compassFrameCount = 32;

        protected Rect mainPanelRect = new Rect(0, 0, 320, 46);
        protected Rect mainColorBackgroundPanelRect = new Rect(5, 5, 66, 36);
        protected Rect stealModeSubrect = new Rect(0, 0, 47, 23);
        protected Rect talkModeSubrect = new Rect(0, 23, 47, 23);
        protected Rect grabModeSubrect = new Rect(0, 46, 47, 23);
        protected Rect infoModeSubrect = new Rect(0, 69, 47, 23);
        protected Rect headPanelRect = new Rect(7, 8, 33, 30);
        protected Rect compassPanelRect = new Rect(275, 2, 43, 42);
        protected Rect healthPanelRect = new Rect(49, 7, 4, 32);
        protected Rect fatiguePanelRect = new Rect(57, 7, 4, 32);
        protected Rect magickaPanelRect = new Rect(65, 7, 4, 32);
        protected Rect interactionModePanelRect = new Rect(131, 0, 47, 23);
        protected Rect optionsPanelRect = new Rect(71, 0, 12, 46);
        protected Rect spellbookPanelRect = new Rect(84, 0, 47, 23);
        protected Rect inventoryPanelRect = new Rect(178, 0, 47, 23);
        protected Rect sheathPanelRect = new Rect(225, 0, 47, 23);
        protected Rect useMagicItemPanelRect = new Rect(84, 23, 47, 23);
        protected Rect transportModePanelRect = new Rect(131, 23, 47, 23);
        protected Rect mapPanelRect = new Rect(178, 23, 47, 23);
        protected Rect restPanelRect = new Rect(225, 23, 47, 23);

        protected DFSize nativeInteractionModesTextureSize = new DFSize(47, 92);

        protected Texture2D mainTexture;
        protected Texture2D mainColorBackgroundTexture;
        protected Texture2D[] compassTextures = new Texture2D[compassFrameCount];
        protected Texture2D stealModeTexture, talkModeTexture, grabModeTexture, infoModeTexture;

        protected Panel mainColorBackgroundPanel = new Panel();
        protected Panel headPanel = new Panel();
        protected Panel compassPanel = new Panel();
        protected HUDVitals vitals = new HUDVitals();
        protected Panel interactionModePanel = new Panel();
        protected Panel optionsPanel = new Panel();
        protected Panel spellbookPanel = new Panel();
        protected Panel inventoryPanel = new Panel();
        protected Panel sheathPanel = new Panel();
        protected Panel useMagicItemPanel = new Panel();
        protected Panel transportModePanel = new Panel();
        protected Panel mapPanel = new Panel();
        protected Panel restPanel = new Panel();

        protected Camera compassCamera;
        protected float eulerAngle;

        protected PlayerEntity playerEntity;

        Vector2 lastCustomScale;

        /// <summary>
        /// Gets or sets a compass camera to automatically determine compass heading.
        /// </summary>
        public Camera CompassCamera
        {
            get { return compassCamera; }
            set { compassCamera = value; }
        }

        /// <summary>
        /// Gets or a sets a Euler angle to use for compass heading.
        /// This value is only observed when CompassCamera is null.
        /// </summary>
        public float EulerAngle
        {
            get { return eulerAngle; }
            set { eulerAngle = Mathf.Clamp(value, 0f, 360f); }
        }

        /// <summary>
        /// Gets or sets texture for head image. Set to null if a refresh of head texture is needed.
        /// Head texture can change at runtime, e.g. loading a new game, lycanthrope shapechange, vampire infection.
        /// </summary>
        public Texture2D HeadTexture { get; set; }

        /// <summary>
        /// Gets height of large HUD in screen space pixels (relative to configured resolution).
        /// Always 0 when large HUD disabled
        /// </summary>
        public float ScreenHeight { get; private set; }

        /// <summary>
        /// Gets or sets custom scaling value for all large HUD controls.
        /// </summary>
        public Vector2 CustomScale { get; set; }

        /// <summary>
        /// True when active mouse cursor is over large HUD.
        /// </summary>
        public bool ActiveMouseOverLargeHUD { get; private set; }

        public HUDLarge()
            : base()
        {
            CompassCamera = Camera.main;
            playerEntity = GameManager.Instance.PlayerEntity;
            LoadAssets();
            Setup();

            // Events to refresh head on new game or load
            Serialization.SaveLoadManager.OnLoad += SaveLoadManager_OnLoad;
            Utility.StartGameBehaviour.OnNewGame += StartGameBehaviour_OnNewGame;
        }

        void LoadAssets()
        {
            // Main large HUD background
            mainTexture = ImageReader.GetTexture(mainFilename);
            mainColorBackgroundTexture = ImageReader.GetTexture(mainColorBackgroundFilename, 0, 0);  // Classic uses blue by default - when are other colors used?

            // Read compass animations
            for (int i = 0; i < compassFrameCount; i++)
            {
                compassTextures[i] = ImageReader.GetTexture(compass0Filename, 0, i, true);
            }

            // Split interaction mode icons
            Texture2D interactionModesTexture = ImageReader.GetTexture(interactionModesFilename);
            stealModeTexture = ImageReader.GetSubTexture(interactionModesTexture, stealModeSubrect, nativeInteractionModesTextureSize);
            talkModeTexture = ImageReader.GetSubTexture(interactionModesTexture, talkModeSubrect, nativeInteractionModesTextureSize);
            grabModeTexture = ImageReader.GetSubTexture(interactionModesTexture, grabModeSubrect, nativeInteractionModesTextureSize);
            infoModeTexture = ImageReader.GetSubTexture(interactionModesTexture, infoModeSubrect, nativeInteractionModesTextureSize);
        }

        void Setup()
        {
            // Setup self as main container for all other large HUD controls
            VerticalAlignment = VerticalAlignment.Bottom;
            BackgroundTexture = mainTexture;
            BackgroundColor = Color.gray;

            // Add main color background panel
            mainColorBackgroundPanel.BackgroundTexture = mainColorBackgroundTexture;
            Components.Add(mainColorBackgroundPanel);

            // Compass
            compassPanel.OnMouseClick += CompassPanel_OnMouseClick;
            compassPanel.OnRightMouseClick += CompassPanel_OnMouseClick;
            Components.Add(compassPanel);

            // Head
            headPanel.BackgroundTextureLayout = BackgroundLayout.ScaleToFit;
            headPanel.OnMouseClick += HeadPanel_OnMouseClick;
            headPanel.OnRightMouseClick += HeadPanel_OnMouseClick;
            Components.Add(headPanel);

            // Vitals
            vitals.HorizontalAlignment = HorizontalAlignment.None;
            vitals.VerticalAlignment = VerticalAlignment.None;
            vitals.AutoSize = AutoSizeModes.None;
            vitals.SetMargins(Margins.All, 0);
            vitals.SetAllAutoSize(AutoSizeModes.None);
            vitals.SetAllHorizontalAlignment(HorizontalAlignment.None);
            vitals.SetAllVerticalAlignment(VerticalAlignment.None);
            Components.Add(vitals);

            // Interaction mode
            interactionModePanel.OnMouseClick += InteractionModePanel_OnMouseClick;
            interactionModePanel.OnRightMouseClick += InteractionModePanel_OnRightMouseClick;
            Components.Add(interactionModePanel);

            // Options
            optionsPanel.OnMouseClick += OptionsPanel_OnMouseClick;
            optionsPanel.OnRightMouseClick += OptionsPanel_OnMouseClick;
            Components.Add(optionsPanel);

            // Spellbook
            spellbookPanel.OnMouseClick += SpellbookPanel_OnMouseClick;
            spellbookPanel.OnRightMouseClick += SpellbookPanel_OnMouseClick;
            Components.Add(spellbookPanel);

            // Inventory
            inventoryPanel.OnMouseClick += InventoryPanel_OnMouseClick;
            inventoryPanel.OnRightMouseClick += InventoryPanel_OnMouseClick;
            Components.Add(inventoryPanel);

            // Sheath
            sheathPanel.OnMouseClick += SheathPanel_OnMouseClick;
            sheathPanel.OnRightMouseClick += SheathPanel_OnMouseClick;
            Components.Add(sheathPanel);

            // Use magic item
            useMagicItemPanel.OnMouseClick += UseMagicItemPanel_OnMouseClick;
            useMagicItemPanel.OnRightMouseClick += UseMagicItemPanel_OnMouseClick;
            Components.Add(useMagicItemPanel);

            // Transport
            transportModePanel.OnMouseClick += TransportModePanel_OnMouseClick;
            transportModePanel.OnRightMouseClick += TransportModePanel_OnMouseClick;
            Components.Add(transportModePanel);

            // Map
            mapPanel.OnMouseClick += MapPanel_OnMouseClick;
            mapPanel.OnRightMouseClick += MapPanel_OnRightMouseClick;
            Components.Add(mapPanel);

            // Rest
            restPanel.OnMouseClick += RestPanel_OnMouseClick;
            restPanel.OnRightMouseClick += RestPanel_OnMouseClick;
            Components.Add(restPanel);
        }

        void Refresh()
        {
            HeadTexture = null;
        }

        public override void Update()
        {
            if (!Enabled)
            {
                ScreenHeight = 0;
                ActiveMouseOverLargeHUD = false;
                return;
            }

            // Update screen height
            ScreenHeight = (int)Rectangle.height;

            // Manually update position and scale of controls to match overall large HUD scale
            // Large HUD exists in screen space not in native 320x200 UI space so scale needs to be amended
            if (lastCustomScale != CustomScale)
            {
                Position = mainPanelRect.position * CustomScale;
                Size = mainPanelRect.size * CustomScale;
                mainColorBackgroundPanel.Position = mainColorBackgroundPanelRect.position * CustomScale;
                mainColorBackgroundPanel.Size = mainColorBackgroundPanelRect.size * CustomScale;
                compassPanel.Position = compassPanelRect.position * CustomScale;
                compassPanel.Size = compassPanelRect.size * CustomScale;
                headPanel.Position = headPanelRect.position * CustomScale;
                headPanel.Size = headPanelRect.size * CustomScale;
                vitals.CustomHealthBarPosition = healthPanelRect.position * CustomScale;
                vitals.CustomHealthBarSize = healthPanelRect.size * CustomScale;
                vitals.CustomFatigueBarPosition = fatiguePanelRect.position * CustomScale;
                vitals.CustomFatigueBarSize = fatiguePanelRect.size * CustomScale;
                vitals.CustomMagickaBarPosition = magickaPanelRect.position * CustomScale;
                vitals.CustomMagickaBarSize = magickaPanelRect.size * CustomScale;
                interactionModePanel.Position = interactionModePanelRect.position * CustomScale;
                interactionModePanel.Size = interactionModePanelRect.size * CustomScale;
                optionsPanel.Position = optionsPanelRect.position * CustomScale;
                optionsPanel.Size = optionsPanelRect.size * CustomScale;
                spellbookPanel.Position = spellbookPanelRect.position * CustomScale;
                spellbookPanel.Size = spellbookPanelRect.size * CustomScale;
                inventoryPanel.Position = inventoryPanelRect.position * CustomScale;
                inventoryPanel.Size = inventoryPanelRect.size * CustomScale;
                sheathPanel.Position = sheathPanelRect.position * CustomScale;
                sheathPanel.Size = sheathPanelRect.size * CustomScale;
                useMagicItemPanel.Position = useMagicItemPanelRect.position * CustomScale;
                useMagicItemPanel.Size = useMagicItemPanelRect.size * CustomScale;
                transportModePanel.Position = transportModePanelRect.position * CustomScale;
                transportModePanel.Size = transportModePanelRect.size * CustomScale;
                mapPanel.Position = mapPanelRect.position * CustomScale;
                mapPanel.Size = mapPanelRect.size * CustomScale;
                restPanel.Position = restPanelRect.position * CustomScale;
                restPanel.Size = restPanelRect.size * CustomScale;

                lastCustomScale = CustomScale;
            }

            // Update head image data when null
            if (HeadTexture == null)
                UpdateHeadTexture();

            // Set head in panel
            headPanel.BackgroundTexture = HeadTexture;

            // Calculate compass rotation percent
            float percent;
            if (compassCamera != null)
                percent = compassCamera.transform.eulerAngles.y / 360f;
            else
                percent = eulerAngle;

            // Update compass pointer
            compassPanel.BackgroundTexture = compassTextures[(int)(compassFrameCount * percent)];

            // Update interaction mode
            UpdateInteractionModeTexture();

            base.Update();
        }

        void UpdateHeadTexture()
        {
            ImageData head;

            // Check for racial override head
            RacialOverrideEffect racialOverride = GameManager.Instance.PlayerEffectManager.GetRacialOverrideEffect();
            if (racialOverride != null && racialOverride.GetCustomHeadImageData(playerEntity, out head))
            {
                HeadTexture = head.texture;
                return;
            }

            // Otherwise just get standard head based on gender and race
            switch (playerEntity.Gender)
            {
                default:
                case Genders.Male:
                    head = ImageReader.GetImageData(playerEntity.RaceTemplate.PaperDollHeadsMale, playerEntity.FaceIndex, 0, true);
                    break;
                case Genders.Female:
                    head = ImageReader.GetImageData(playerEntity.RaceTemplate.PaperDollHeadsFemale, playerEntity.FaceIndex, 0, true);
                    break;
            }
            HeadTexture = head.texture;
        }

        void UpdateInteractionModeTexture()
        {
            switch (GameManager.Instance.PlayerActivate.CurrentMode)
            {
                case PlayerActivateModes.Steal:
                    interactionModePanel.BackgroundTexture = stealModeTexture;
                    break;
                case PlayerActivateModes.Talk:
                    interactionModePanel.BackgroundTexture = talkModeTexture;
                    break;
                case PlayerActivateModes.Grab:
                    interactionModePanel.BackgroundTexture = grabModeTexture;
                    break;
                case PlayerActivateModes.Info:
                    interactionModePanel.BackgroundTexture = infoModeTexture;
                    break;
            }
        }

        protected override void MouseEnter()
        {
            // Cannot be over large HUD when cursor not active or large HUD not enabled
            ActiveMouseOverLargeHUD = GameManager.Instance.PlayerMouseLook.cursorActive && DaggerfallUnity.Settings.LargeHUD;

            base.MouseEnter();
        }

        protected override void MouseLeave(BaseScreenComponent sender)
        {
            ActiveMouseOverLargeHUD = false;

            base.MouseLeave(sender);
        }

        #region Event Handlers

        private void SaveLoadManager_OnLoad(Serialization.SaveData_v1 saveData)
        {
            Refresh();
        }

        private void StartGameBehaviour_OnNewGame()
        {
            Refresh();
        }

        private void InteractionModePanel_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (GameManager.IsGamePaused)
                return;

            // Cycle interaction mode forwards on left click
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            switch (GameManager.Instance.PlayerActivate.CurrentMode)
            {
                case PlayerActivateModes.Steal:
                    GameManager.Instance.PlayerActivate.ChangeInteractionMode(PlayerActivateModes.Talk);
                    break;
                case PlayerActivateModes.Talk:
                    GameManager.Instance.PlayerActivate.ChangeInteractionMode(PlayerActivateModes.Grab);
                    break;
                case PlayerActivateModes.Grab:
                    GameManager.Instance.PlayerActivate.ChangeInteractionMode(PlayerActivateModes.Info);
                    break;
                case PlayerActivateModes.Info:
                    GameManager.Instance.PlayerActivate.ChangeInteractionMode(PlayerActivateModes.Steal);
                    break;
            }
        }

        private void InteractionModePanel_OnRightMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (GameManager.IsGamePaused)
                return;

            // Cycle interaction mode backwards on right click
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            switch (GameManager.Instance.PlayerActivate.CurrentMode)
            {
                case PlayerActivateModes.Steal:
                    GameManager.Instance.PlayerActivate.ChangeInteractionMode(PlayerActivateModes.Info);
                    break;
                case PlayerActivateModes.Talk:
                    GameManager.Instance.PlayerActivate.ChangeInteractionMode(PlayerActivateModes.Steal);
                    break;
                case PlayerActivateModes.Grab:
                    GameManager.Instance.PlayerActivate.ChangeInteractionMode(PlayerActivateModes.Talk);
                    break;
                case PlayerActivateModes.Info:
                    GameManager.Instance.PlayerActivate.ChangeInteractionMode(PlayerActivateModes.Grab);
                    break;
            }
        }

        private void HeadPanel_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (!GameManager.IsGamePaused)
            {
                DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
                DaggerfallUI.Instance.UserInterfaceManager.PostMessage(DaggerfallUIMessages.dfuiOpenCharacterSheetWindow);
            }
        }

        private void OptionsPanel_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (!GameManager.IsGamePaused)
            {
                DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
                DaggerfallUI.Instance.UserInterfaceManager.PostMessage(DaggerfallUIMessages.dfuiOpenPauseOptionsDialog);
            }
        }

        private void SpellbookPanel_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (!GameManager.IsGamePaused)
            {
                DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
                DaggerfallUI.Instance.UserInterfaceManager.PostMessage(DaggerfallUIMessages.dfuiOpenSpellBookWindow);
            }
        }

        private void InventoryPanel_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (!GameManager.IsGamePaused)
            {
                DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
                DaggerfallUI.Instance.UserInterfaceManager.PostMessage(DaggerfallUIMessages.dfuiOpenInventoryWindow);
            }
        }

        private void SheathPanel_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (!GameManager.IsGamePaused)
            {
                DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
                GameManager.Instance.WeaponManager.ToggleSheath();
            }
        }

        private void UseMagicItemPanel_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (!GameManager.IsGamePaused)
            {
                DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
                DaggerfallUI.Instance.UserInterfaceManager.PostMessage(DaggerfallUIMessages.dfuiOpenUseMagicItemWindow);
            }
        }

        private void TransportModePanel_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (!GameManager.IsGamePaused)
            {
                DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
                DaggerfallUI.Instance.UserInterfaceManager.PostMessage(DaggerfallUIMessages.dfuiOpenTransportWindow);
            }
        }

        private void MapPanel_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (!GameManager.IsGamePaused)
            {
                DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
                DaggerfallUI.Instance.UserInterfaceManager.PostMessage(DaggerfallUIMessages.dfuiOpenAutomap);
            }
        }

        private void MapPanel_OnRightMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (!GameManager.IsGamePaused)
            {
                DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
                DaggerfallUI.Instance.UserInterfaceManager.PostMessage(DaggerfallUIMessages.dfuiOpenTravelMapWindow);
            }
        }

        private void RestPanel_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (!GameManager.IsGamePaused)
            {
                DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
                DaggerfallUI.Instance.UserInterfaceManager.PostMessage(DaggerfallUIMessages.dfuiOpenRestWindow);
            }
        }

        private void CompassPanel_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (!GameManager.IsGamePaused)
            {
                DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
                DaggerfallUI.Instance.UserInterfaceManager.PostMessage(DaggerfallUIMessages.dfuiStatusInfo);
            }
        }

        #endregion
    }
}