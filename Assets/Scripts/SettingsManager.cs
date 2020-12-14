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

//#define SEPARATE_DEV_PERSISTENT_PATH

using UnityEngine;
using System;
using System.Globalization;
using System.Collections.Generic;
using System.IO;
using DaggerfallWorkshop.Game;
using IniParser;
using IniParser.Model;

namespace DaggerfallWorkshop
{
    /// <summary>
    /// Settings manager for reading game configuration from INI file.
    /// Deploys new settings from Resources/defaults.ini to persistentDataPath/settings.ini.
    /// Settings no longer present in defaults.ini are removed from settings.ini.
    /// Reads on start to public property cache. This prevents expensive string parsing if property checked every frame.
    /// Transfers settings from public property cache back to ini data on save.
    /// </summary>
    public class SettingsManager
    {
        const string defaultsIniName = "defaults.ini";
        const string settingsIniName = "settings.ini";

        const string sectionDaggerfall = "Daggerfall";
        const string sectionVideo = "Video";
        const string sectionAudio = "Audio";
        const string sectionChildGuard = "ChildGuard";
        const string sectionGUI = "GUI";
        const string sectionSpells = "Spells";
        const string sectionControls = "Controls";
        const string sectionMap = "Map";
        const string sectionStartup = "Startup";
        const string sectionExperimental = "Experimental";
        const string sectionEnhancements = "Enhancements";

        FileIniDataParser iniParser = new FileIniDataParser();
        IniData defaultIniData = null;
        IniData userIniData = null;

        string persistentPath = null;

        public string PersistentDataPath
        {
            get
            {
                if (string.IsNullOrEmpty(persistentPath))
                {
#if UNITY_EDITOR && SEPARATE_DEV_PERSISTENT_PATH
                    persistentPath = String.Concat(Application.persistentDataPath, ".devenv");
                    Directory.CreateDirectory(persistentPath);
#else
                    persistentPath = Application.persistentDataPath;
#endif
                }
                return persistentPath;
            }
        }

        public SettingsManager()
        {
            LoadSettings();
        }

        #region Public Settings Properties

        // [Daggerfall]
        public string MyDaggerfallPath { get; set; }
        public string MyDaggerfallUnitySavePath { get; set; }
        public string MyDaggerfallUnityScreenshotsPath { get; set; }

        // [Video]
        public int ResolutionWidth { get; set; }
        public int ResolutionHeight { get; set; }
        public int RetroRenderingMode { get; set; }
        public int PostProcessingInRetroMode { get; set; }
        public bool UseMipMapsInRetroMode { get; set; }
        public int PalettizationLUTShift { get; set; }
        public bool VSync { get; set; }
        public int TargetFrameRate { get; set; }
        public bool Fullscreen { get; set; }
        public bool ExclusiveFullscreen { get; set; }
        public int FieldOfView { get; set; }
        public int ShadowResolutionMode { get; set; }
        public int MainFilterMode { get; set; }
        public int QualityLevel { get; set; }
        public bool DungeonLightShadows { get; set; }
        public bool InteriorLightShadows { get; set; }
        public bool ExteriorLightShadows { get; set; }
        public bool MobileNPCShadows { get; set; }
        public bool GeneralBillboardShadows { get; set; }
        public float DungeonShadowDistance { get; set; }
        public float InteriorShadowDistance { get; set; }
        public float ExteriorShadowDistance { get; set; }
        public bool EnableTextureArrays { get; set; }
        public int RandomDungeonTextures { get; set; }
        public int CursorWidth { get; set; }
        public int CursorHeight { get; set; }

        // [Audio]
        public string SoundFont { get; set; }
        public bool AlternateMusic { get; set; }

        // [ChildGuard]
        public bool PlayerNudity { get; set; }

        // [GUI]
        public bool ShowOptionsAtStart { get; set; }
        public int GUIFilterMode { get; set; }
        public int VideoFilterMode { get; set; }
        public bool Crosshair { get; set; }
        public string InteractionModeIcon { get; set; }
        public bool SwapHealthAndFatigueColors { get; set; }
        public float DimAlphaStrength { get; set; }
        public bool FreeScaling { get; set; }
        public bool EnableToolTips { get; set; }
        public float ToolTipDelayInSeconds { get; set; }
        public Color32 ToolTipBackgroundColor { get; set; }
        public Color32 ToolTipTextColor { get; set; }
        public int ShopQualityPresentation { get; set; }
        public int ShopQualityHUDDelay { get; set; }
        public bool ShowQuestJournalClocksAsCountdown { get; set; }
        public bool EnableInventoryInfoPanel { get; set; }
        public bool EnableEnhancedItemLists { get; set; }
        public bool EnableModernConversationStyleInTalkWindow { get; set; }
        public string IconsPositioningScheme { get; set; }
        public int HelmAndShieldMaterialDisplay { get; set; }
        public bool AccelerateUICopyTexture { get; set; }
        public bool EnableVitalsIndicators { get; set; }
        public bool SDFFontRendering { get; set; }
        public bool EnableGeographicBackgrounds { get; set; }
        public bool EnableArrowCounter { get; set; }
        public bool DungeonExitWagonPrompt { get; set; }
        public bool TravelMapLocationsOutline { get; set; }
        public bool IllegalRestWarning { get; set; }
        public bool LargeHUD { get; set; }
        public bool LargeHUDDocked { get; set; }
        public float LargeHUDUndockedScale { get; set; }
        public int LargeHUDUndockedAlignment { get; set; }
        public bool LargeHUDUndockedOffsetWeapon { get; set; }
        public bool LargeHUDOffsetHorse { get; set; }
        public bool CanDropQuestItems { get; set; }
        public bool RunInBackground { get; set; }

        // [Spells]
        public bool EnableSpellLighting { get; set; }
        public bool EnableSpellShadows { get; set; }

        // [Controls]
        public bool InvertMouseVertical { get; set; }
        public bool MouseLookSmoothing { get; set; }
        public float MouseLookSensitivity { get; set; }
        public float JoystickLookSensitivity { get; set; }
        public float JoystickCursorSensitivity { get; set; }
        public float JoystickMovementThreshold { get; set; }
        public float JoystickDeadzone { get; set; }
        public bool EnableController { get; set; }

        public bool HeadBobbing { get; set; }
        public int Handedness { get; set; }
        public float WeaponAttackThreshold { get; set; }
        public float WeaponSensitivity { get; set; }
        public bool MovementAcceleration { get; set; }
        public bool ToggleSneak { get; set; }
        public bool ClickToAttack { get; set; }
        public int CameraRecoilStrength { get; set; }
        public float MusicVolume { get; set; }
        public float SoundVolume { get; set; }
        public bool InstantRepairs { get; set; }
        public bool AllowMagicRepairs { get; set; }
        public bool BowDrawback { get; set; }

        // [Map]
        public int AutomapNumberOfDungeons { get; set; }
        public bool AutomapDisableMicroMap { get; set; }
        public bool AutomapRememberSliceLevel { get; set; }
        public bool AutomapAlwaysMaxOutSliceLevel { get; set; }
        public float ExteriorMapDefaultZoomLevel { get; set; }
        public bool ExteriorMapResetZoomLevelOnNewLocation { get; set; }

        // [Startup]
        public int StartCellX { get; set; }
        public int StartCellY { get; set; }
        public bool StartInDungeon { get; set; }

        // [Experimental]
        public int TerrainDistance { get; set; }
        public float TerrainHeightmapPixelError { get; set; }
        public bool SmallerDungeons { get; set; }
        public int AssetCacheThreshold { get; set; }

        // [Enhancements]
        public bool LypyL_GameConsole { get; set; }
        public bool LypyL_ModSystem { get; set; }
        public bool AssetInjection { get; set; }
        public bool CompressModdedTextures { get; set; }
        public bool NearDeathWarning { get; set; }
        public bool AlternateRandomEnemySelection { get; set; }
        public bool AdvancedClimbing { get; set; }
        public float DungeonAmbientLightScale { get; set; }
        public float NightAmbientLightScale { get; set; }
        public float PlayerTorchLightScale { get; set; }
        public bool PlayerTorchFromItems { get; set; }
        public bool CombatVoices { get; set; }
        public bool EnemyInfighting { get; set; }
        public bool EnhancedCombatAI { get; set; }
        public bool GuildQuestListBox { get; set; }
        public bool BowLeftHandWithSwitching { get; set; }
        public int LoiterLimitInHours { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Load settings from settings.ini to live properties.
        /// </summary>
        public void LoadSettings()
        {
            // Read settings from persistent file
            ReadSettingsFile();

            // Read ini data to property cache
            MyDaggerfallPath = GetString(sectionDaggerfall, "MyDaggerfallPath");
            MyDaggerfallUnitySavePath = GetString(sectionDaggerfall, "MyDaggerfallUnitySavePath");
            MyDaggerfallUnityScreenshotsPath = GetString(sectionDaggerfall, "MyDaggerfallUnityScreenshotsPath");

            ResolutionWidth = GetInt(sectionVideo, "ResolutionWidth");
            ResolutionHeight = GetInt(sectionVideo, "ResolutionHeight");
            RetroRenderingMode = GetInt(sectionVideo, "RetroRenderingMode", 0, 2);
            PostProcessingInRetroMode = GetInt(sectionVideo, "PostProcessingInRetroMode");
            UseMipMapsInRetroMode = GetBool(sectionVideo, "UseMipMapsInRetroMode");
            PalettizationLUTShift = GetInt(sectionVideo, "PalettizationLUTShift");
            VSync = GetBool(sectionVideo, "VSync");
            TargetFrameRate = GetInt(sectionVideo, "TargetFrameRate", 0, 300);
            Fullscreen = GetBool(sectionVideo, "Fullscreen");
            ExclusiveFullscreen = GetBool(sectionVideo, "ExclusiveFullscreen");
            RunInBackground = GetBool(sectionVideo, "RunInBackground");
            FieldOfView = GetInt(sectionVideo, "FieldOfView", 60, 80);
            MainFilterMode = GetInt(sectionVideo, "MainFilterMode", 0, 2);
            ShadowResolutionMode = GetInt(sectionVideo, "ShadowResolutionMode", 0, 3);
            QualityLevel = GetInt(sectionVideo, "QualityLevel", 0, 5);
            DungeonLightShadows = GetBool(sectionVideo, "DungeonLightShadows");
            InteriorLightShadows = GetBool(sectionVideo, "InteriorLightShadows");
            ExteriorLightShadows = GetBool(sectionVideo, "ExteriorLightShadows");
            MobileNPCShadows = GetBool(sectionVideo, "MobileNPCShadows");
            GeneralBillboardShadows = GetBool(sectionVideo, "GeneralBillboardShadows");
            DungeonShadowDistance = GetFloat(sectionVideo, "DungeonShadowDistance", 0.1f, 50.0f);
            InteriorShadowDistance = GetFloat(sectionVideo, "InteriorShadowDistance", 0.1f, 50.0f);
            ExteriorShadowDistance = GetFloat(sectionVideo, "ExteriorShadowDistance", 0.1f, 150.0f);
            EnableTextureArrays = GetBool(sectionVideo, "EnableTextureArrays");
            RandomDungeonTextures = GetInt(sectionVideo, "RandomDungeonTextures", 0, 4);

            SoundFont = GetString(sectionAudio, "SoundFont");
            AlternateMusic = GetBool(sectionAudio, "AlternateMusic");

            PlayerNudity = GetBool(sectionChildGuard, "PlayerNudity");

            ShowOptionsAtStart = GetBool(sectionGUI, "ShowOptionsAtStart");
            GUIFilterMode = GetInt(sectionGUI, "GUIFilterMode", 0, 2);
            VideoFilterMode = GetInt(sectionGUI, "VideoFilterMode");
            Crosshair = GetBool(sectionGUI, "Crosshair");
            InteractionModeIcon = GetString(sectionGUI, "InteractionModeIcon");
            SwapHealthAndFatigueColors = GetBool(sectionGUI, "SwapHealthAndFatigueColors");
            DimAlphaStrength = GetFloat(sectionGUI, "DimAlphaStrength", 0, 1);
            FreeScaling = GetBool(sectionGUI, "FreeScaling");
            EnableToolTips = GetBool(sectionGUI, "EnableToolTips");
            ToolTipDelayInSeconds = GetFloat(sectionGUI, "ToolTipDelayInSeconds", 0, 10);
            ToolTipBackgroundColor = GetColor(sectionGUI, "ToolTipBackgroundColor", DaggerfallUI.DaggerfallUnityDefaultToolTipBackgroundColor);
            ToolTipTextColor = GetColor(sectionGUI, "ToolTipTextColor", DaggerfallUI.DaggerfallUnityDefaultToolTipTextColor);
            EnableInventoryInfoPanel = GetBool(sectionGUI, "EnableInventoryInfoPanel");
            EnableEnhancedItemLists = GetBool(sectionGUI, "EnableEnhancedItemLists");
            EnableModernConversationStyleInTalkWindow = GetBool(sectionGUI, "EnableModernConversationStyleInTalkWindow");
            IconsPositioningScheme = GetString(sectionGUI, "IconsPositioningScheme");
            HelmAndShieldMaterialDisplay = GetInt(sectionGUI, "HelmAndShieldMaterialDisplay", 0, 3);            
            ShopQualityPresentation = GetInt(sectionGUI, "ShopQualityPresentation", 0, 2);
            ShopQualityHUDDelay = GetInt(sectionGUI, "ShopQualityHUDDelay", 1, 10);
            ShowQuestJournalClocksAsCountdown = GetBool(sectionGUI, "ShowQuestJournalClocksAsCountdown");
            AccelerateUICopyTexture = GetBool(sectionGUI, "AccelerateUICopyTexture");
            EnableVitalsIndicators = GetBool(sectionGUI, "EnableVitalsIndicators");
            SDFFontRendering = GetBool(sectionGUI, "SDFFontRendering");
            EnableGeographicBackgrounds = GetBool(sectionGUI, "EnableGeographicBackgrounds");
            EnableArrowCounter = GetBool(sectionGUI, "EnableArrowCounter");
            DungeonExitWagonPrompt = GetBool(sectionGUI, "DungeonExitWagonPrompt");
            TravelMapLocationsOutline = GetBool(sectionGUI, "TravelMapLocationsOutline");
            IllegalRestWarning = GetBool(sectionGUI, "IllegalRestWarning");
            LargeHUD = GetBool(sectionGUI, "LargeHUD");
            LargeHUDDocked = GetBool(sectionGUI, "LargeHUDDocked");
            LargeHUDUndockedScale = GetFloat(sectionGUI, "LargeHUDUndockedScale", 0.25f, 2.0f);
            LargeHUDUndockedAlignment = GetInt(sectionGUI, "LargeHUDUndockedAlignment", 0, 3);
            LargeHUDUndockedOffsetWeapon = GetBool(sectionGUI, "LargeHUDUndockedOffsetWeapon");
            LargeHUDOffsetHorse = GetBool(sectionGUI, "LargeHUDOffsetHorse");
            CanDropQuestItems = GetBool(sectionGUI, "CanDropQuestItems");

            EnableSpellLighting = GetBool(sectionSpells, "EnableSpellLighting");
            EnableSpellShadows = GetBool(sectionSpells, "EnableSpellShadows");

            InvertMouseVertical = GetBool(sectionControls, "InvertMouseVertical");
            MouseLookSmoothing = GetBool(sectionControls, "MouseLookSmoothing");
            MouseLookSensitivity = GetFloat(sectionControls, "MouseLookSensitivity", 0.1f, 8.0f);
            JoystickLookSensitivity = GetFloat(sectionControls, "JoystickLookSensitivity", 0.1f, 4.0f);
            JoystickCursorSensitivity = GetFloat(sectionControls, "JoystickCursorSensitivity", 0.1f, 5.0f);
            JoystickMovementThreshold = GetFloat(sectionControls, "JoystickMovementThreshold", 0.0f, 1.0f);
            MovementAcceleration = GetBool(sectionControls, "MovementAcceleration");
            JoystickDeadzone = GetFloat(sectionControls, "JoystickDeadzone", 0.001f, 1.0f);
            EnableController = GetBool(sectionControls, "EnableController");
            ToggleSneak = GetBool(sectionControls, "ToggleSneak");
            HeadBobbing = GetBool(sectionControls, "HeadBobbing");
            Handedness = GetInt(sectionControls, "Handedness", 0, 3);
            WeaponAttackThreshold = GetFloat(sectionControls, "WeaponAttackThreshold", 0.001f, 1.0f);
            WeaponSensitivity = GetFloat(sectionControls, "WeaponSensitivity", 0.1f, 10.0f);
            ClickToAttack = GetBool(sectionControls, "ClickToAttack");
            CameraRecoilStrength = GetInt(sectionControls, "CameraRecoilStrength", 0, 4);
            SoundVolume = GetFloat(sectionControls, "SoundVolume", 0f, 1.0f);
            MusicVolume = GetFloat(sectionControls, "MusicVolume", 0f, 1.0f);
            InstantRepairs = GetBool(sectionControls, "InstantRepairs");
            AllowMagicRepairs = GetBool(sectionControls, "AllowMagicRepairs");
            BowDrawback = GetBool(sectionControls, "BowDrawback");

            AutomapNumberOfDungeons = GetInt(sectionMap, "AutomapNumberOfDungeons", 0, 100);
            AutomapDisableMicroMap = GetBool(sectionMap, "AutomapDisableMicroMap");
            AutomapRememberSliceLevel = GetBool(sectionMap, "AutomapRememberSliceLevel");
            AutomapAlwaysMaxOutSliceLevel = GetBool(sectionMap, "AutomapAlwaysMaxOutSliceLevel");            
            ExteriorMapDefaultZoomLevel = GetFloat(sectionMap, "ExteriorMapDefaultZoomLevel", 4, 31);
            ExteriorMapResetZoomLevelOnNewLocation = GetBool(sectionMap, "ExteriorMapResetZoomLevelOnNewLocation");

            StartCellX = GetInt(sectionStartup, "StartCellX", 2, 997);
            StartCellY = GetInt(sectionStartup, "StartCellY", 2, 497);
            StartInDungeon = GetBool(sectionStartup, "StartInDungeon");

            TerrainDistance = GetInt(sectionExperimental, "TerrainDistance", 1, 4);
            TerrainHeightmapPixelError = GetFloat(sectionExperimental, "TerrainHeightmapPixelError", 1, 10);
            SmallerDungeons = GetBool(sectionExperimental, "SmallerDungeons");
            AssetCacheThreshold = GetInt(sectionExperimental, "AssetCacheThreshold", 0, 120);

            LypyL_GameConsole = GetBool(sectionEnhancements, "LypyL_GameConsole");
            LypyL_ModSystem = GetBool(sectionEnhancements, "LypyL_ModSystem");
            AssetInjection = GetBool(sectionEnhancements, "AssetInjection");
            CompressModdedTextures = GetBool(sectionEnhancements, "CompressModdedTextures");
            NearDeathWarning = GetBool(sectionEnhancements, "NearDeathWarning");
            AlternateRandomEnemySelection = GetBool(sectionEnhancements, "AlternateRandomEnemySelection");
            AdvancedClimbing = GetBool(sectionEnhancements, "AdvancedClimbing");
            DungeonAmbientLightScale = GetFloat(sectionEnhancements, "DungeonAmbientLightScale", 0.0f, 1.0f);
            NightAmbientLightScale = GetFloat(sectionEnhancements, "NightAmbientLightScale", 0.0f, 1.0f);
            PlayerTorchLightScale = GetFloat(sectionEnhancements, "PlayerTorchLightScale", 0.0f, 1.0f);
            PlayerTorchFromItems = GetBool(sectionEnhancements, "PlayerTorchFromItems");
            CombatVoices = GetBool(sectionEnhancements, "CombatVoices");
            EnemyInfighting = GetBool(sectionEnhancements, "EnemyInfighting");
            EnhancedCombatAI = GetBool(sectionEnhancements, "EnhancedCombatAI");
            GuildQuestListBox = GetBool(sectionEnhancements, "GuildQuestListBox");
            BowLeftHandWithSwitching = GetBool(sectionEnhancements, "BowLeftHandWithSwitching");
            LoiterLimitInHours = GetInt(sectionEnhancements, "LoiterLimitInHours");
        }

        /// <summary>
        /// Save live properties back to settings.ini.
        /// </summary>
        public void SaveSettings()
        {
            // Write property cache to ini data
            SetString(sectionDaggerfall, "MyDaggerfallPath", MyDaggerfallPath);
            SetString(sectionDaggerfall, "MyDaggerfallUnitySavePath", MyDaggerfallUnitySavePath);
            SetString(sectionDaggerfall, "MyDaggerfallUnityScreenshotsPath", MyDaggerfallUnityScreenshotsPath);

            SetInt(sectionVideo, "ResolutionWidth", ResolutionWidth);
            SetInt(sectionVideo, "ResolutionHeight", ResolutionHeight);
            SetInt(sectionVideo, "RetroRenderingMode", RetroRenderingMode);
            SetInt(sectionVideo, "PostProcessingInRetroMode", PostProcessingInRetroMode);
            SetBool(sectionVideo, "UseMipMapsInRetroMode", UseMipMapsInRetroMode);
            SetInt(sectionVideo, "PalettizationLUTShift", PalettizationLUTShift);
            SetBool(sectionVideo, "VSync", VSync);
            SetInt(sectionVideo, "TargetFrameRate", TargetFrameRate);
            SetBool(sectionVideo, "Fullscreen", Fullscreen);
            SetBool(sectionVideo, "ExclusiveFullscreen", ExclusiveFullscreen);
            SetBool(sectionVideo, "RunInBackground", RunInBackground);
            SetInt(sectionVideo, "FieldOfView", FieldOfView);
            SetInt(sectionVideo, "MainFilterMode", MainFilterMode);
            SetInt(sectionVideo, "ShadowResolutionMode", ShadowResolutionMode);
            SetInt(sectionVideo, "QualityLevel", QualityLevel);
            SetBool(sectionVideo, "DungeonLightShadows", DungeonLightShadows);
            SetBool(sectionVideo, "InteriorLightShadows", InteriorLightShadows);
            SetBool(sectionVideo, "ExteriorLightShadows", ExteriorLightShadows);
            SetBool(sectionVideo, "MobileNPCShadows", MobileNPCShadows);
            SetBool(sectionVideo, "GeneralBillboardShadows", GeneralBillboardShadows);
            SetFloat(sectionVideo, "DungeonShadowDistance", DungeonShadowDistance);
            SetFloat(sectionVideo, "InteriorShadowDistance", InteriorShadowDistance);
            SetFloat(sectionVideo, "ExteriorShadowDistance", ExteriorShadowDistance);
            SetBool(sectionVideo, "EnableTextureArrays", EnableTextureArrays);
            SetInt(sectionVideo, "RandomDungeonTextures", RandomDungeonTextures);

            SetString(sectionAudio, "SoundFont", SoundFont);
            SetBool(sectionAudio, "AlternateMusic", AlternateMusic);

            SetBool(sectionChildGuard, "PlayerNudity", PlayerNudity);

            SetBool(sectionGUI, "ShowOptionsAtStart", ShowOptionsAtStart);
            SetInt(sectionGUI, "GUIFilterMode", GUIFilterMode);
            SetInt(sectionGUI, "VideoFilterMode", VideoFilterMode);
            SetBool(sectionGUI, "Crosshair", Crosshair);
            SetString(sectionGUI, "InteractionModeIcon", InteractionModeIcon);
            SetBool(sectionGUI, "SwapHealthAndFatigueColors", SwapHealthAndFatigueColors);
            SetFloat(sectionGUI, "DimAlphaStrength", DimAlphaStrength);
            SetBool(sectionGUI, "FreeScaling", FreeScaling);
            SetBool(sectionGUI, "EnableToolTips", EnableToolTips);
            SetFloat(sectionGUI, "ToolTipDelayInSeconds", ToolTipDelayInSeconds);
            SetColor(sectionGUI, "ToolTipBackgroundColor", ToolTipBackgroundColor);
            SetColor(sectionGUI, "ToolTipTextColor", ToolTipTextColor);
            SetBool(sectionGUI, "EnableInventoryInfoPanel", EnableInventoryInfoPanel);
            SetBool(sectionGUI, "EnableEnhancedItemLists", EnableEnhancedItemLists);
            SetBool(sectionGUI, "EnableModernConversationStyleInTalkWindow", EnableModernConversationStyleInTalkWindow);
            SetString(sectionGUI, "IconsPositioningScheme", IconsPositioningScheme);
            SetInt(sectionGUI, "HelmAndShieldMaterialDisplay", HelmAndShieldMaterialDisplay);
            SetInt(sectionGUI, "AutomapNumberOfDungeons", AutomapNumberOfDungeons);
            SetInt(sectionGUI, "ShopQualityPresentation", ShopQualityPresentation);
            SetInt(sectionGUI, "ShopQualityHUDDelay", ShopQualityHUDDelay);
            SetBool(sectionGUI, "ShowQuestJournalClocksAsCountdown", ShowQuestJournalClocksAsCountdown);
            SetBool(sectionGUI, "AccelerateUICopyTexture", AccelerateUICopyTexture);
            SetBool(sectionGUI, "EnableVitalsIndicators", EnableVitalsIndicators);
            SetBool(sectionGUI, "SDFFontRendering", SDFFontRendering);
            SetBool(sectionGUI, "EnableGeographicBackgrounds", EnableGeographicBackgrounds);
            SetBool(sectionGUI, "EnableArrowCounter", EnableArrowCounter);
            SetBool(sectionGUI, "DungeonExitWagonPrompt", DungeonExitWagonPrompt);
            SetBool(sectionGUI, "TravelMapLocationsOutline", TravelMapLocationsOutline);
            SetBool(sectionGUI, "IllegalRestWarning", IllegalRestWarning);
            SetBool(sectionGUI, "LargeHUD", LargeHUD);
            SetBool(sectionGUI, "LargeHUDDocked", LargeHUDDocked);
            SetFloat(sectionGUI, "LargeHUDUndockedScale", LargeHUDUndockedScale);
            SetInt(sectionGUI, "LargeHUDUndockedAlignment", LargeHUDUndockedAlignment);
            SetBool(sectionGUI, "LargeHUDUndockedOffsetWeapon", LargeHUDUndockedOffsetWeapon);
            SetBool(sectionGUI, "LargeHUDOffsetHorse", LargeHUDOffsetHorse);
            SetBool(sectionGUI, "CanDropQuestItems", CanDropQuestItems);

            SetBool(sectionSpells, "EnableSpellLighting", EnableSpellLighting);
            SetBool(sectionSpells, "EnableSpellShadows", EnableSpellShadows);

            SetBool(sectionControls, "InvertMouseVertical", InvertMouseVertical);
            SetBool(sectionControls, "MouseLookSmoothing", MouseLookSmoothing);
            SetFloat(sectionControls, "MouseLookSensitivity", MouseLookSensitivity);
            SetFloat(sectionControls, "JoystickLookSensitivity", JoystickLookSensitivity);
            SetFloat(sectionControls, "JoystickCursorSensitivity", JoystickCursorSensitivity);
            SetFloat(sectionControls, "JoystickMovementThreshold", JoystickMovementThreshold);
            SetBool(sectionControls, "MovementAcceleration", MovementAcceleration);
            SetFloat(sectionControls, "JoystickDeadzone", JoystickDeadzone);
            SetBool(sectionControls, "EnableController", EnableController);
            SetBool(sectionControls, "ToggleSneak", ToggleSneak);
            SetBool(sectionControls, "HeadBobbing", HeadBobbing);
            SetInt(sectionControls, "Handedness", Handedness);
            SetFloat(sectionControls, "WeaponAttackThreshold", WeaponAttackThreshold);
            SetFloat(sectionControls, "WeaponSensitivity", WeaponSensitivity);
            SetBool(sectionControls, "ClickToAttack", ClickToAttack);
            SetInt(sectionControls, "CameraRecoilStrength", CameraRecoilStrength);
            SetFloat(sectionControls, "SoundVolume", SoundVolume);
            SetFloat(sectionControls, "MusicVolume", MusicVolume);
            SetBool(sectionControls, "InstantRepairs", InstantRepairs);
            SetBool(sectionControls, "AllowMagicRepairs", AllowMagicRepairs);
            SetBool(sectionControls, "BowDrawback", BowDrawback);

            SetInt(sectionStartup, "StartCellX", StartCellX);
            SetInt(sectionStartup, "StartCellY", StartCellY);
            SetBool(sectionStartup, "StartInDungeon", StartInDungeon);

            SetInt(sectionExperimental, "TerrainDistance", TerrainDistance);
            SetFloat(sectionExperimental, "TerrainHeightmapPixelError", TerrainHeightmapPixelError);
            SetBool(sectionExperimental, "SmallerDungeons", SmallerDungeons);
            SetInt(sectionExperimental, "AssetCacheThreshold", AssetCacheThreshold);

            SetBool(sectionEnhancements, "LypyL_GameConsole", LypyL_GameConsole);
            SetBool(sectionEnhancements, "LypyL_ModSystem", LypyL_ModSystem);
            SetBool(sectionEnhancements, "AssetInjection", AssetInjection);
            SetBool(sectionEnhancements, "CompressModdedTextures", CompressModdedTextures);
            SetBool(sectionEnhancements, "NearDeathWarning", NearDeathWarning);
            SetBool(sectionEnhancements, "AlternateRandomEnemySelection", AlternateRandomEnemySelection);
            SetBool(sectionEnhancements, "AdvancedClimbing", AdvancedClimbing);
            SetFloat(sectionEnhancements, "DungeonAmbientLightScale", DungeonAmbientLightScale);
            SetFloat(sectionEnhancements, "NightAmbientLightScale", NightAmbientLightScale);
            SetFloat(sectionEnhancements, "PlayerTorchLightScale", PlayerTorchLightScale);
            SetBool(sectionEnhancements, "PlayerTorchFromItems", PlayerTorchFromItems);
            SetBool(sectionEnhancements, "CombatVoices", CombatVoices);
            SetBool(sectionEnhancements, "EnemyInfighting", EnemyInfighting);
            SetBool(sectionEnhancements, "EnhancedCombatAI", EnhancedCombatAI);
            SetBool(sectionEnhancements, "GuildQuestListBox", GuildQuestListBox);
            SetBool(sectionEnhancements, "BowLeftHandWithSwitching", BowLeftHandWithSwitching);
            SetInt(sectionEnhancements, "LoiterLimitInHours", LoiterLimitInHours);

            // Write settings to persistent file
            WriteSettingsFile();
        }

        #endregion

        #region Private Methods

        void ReadSettingsFile()
        {
            // Load defaults.ini
            TextAsset asset = Resources.Load<TextAsset>(defaultsIniName);
            MemoryStream stream = new MemoryStream(asset.bytes);
            StreamReader reader = new StreamReader(stream);
            defaultIniData = iniParser.ReadData(reader);
            reader.Close();

            // Must have settings.ini in persistent data path
            string message;
            string userIniPath = Path.Combine(PersistentDataPath, settingsIniName);
            if (!File.Exists(userIniPath))
            {
                // Create file
                message = string.Format("Creating new '{0}' at path '{1}'", settingsIniName, userIniPath);
                File.WriteAllBytes(userIniPath, asset.bytes);
                DaggerfallUnity.LogMessage(message);
            }

            // Log ini path in use
            message = string.Format("Using '{0}' at path '{1}'", settingsIniName, userIniPath);
            DaggerfallUnity.LogMessage(message);

            // Load settings.ini or set as read-only
            userIniData = iniParser.ReadFile(userIniPath);

            // Ensure user ini data in sync with default ini data
            SyncIniData();
        }

        void WriteSettingsFile()
        {
            if (iniParser != null)
            {
                try
                {
                    string path = Path.Combine(PersistentDataPath, settingsIniName);
                    if (File.Exists(path))
                    {
                        iniParser.WriteFile(path, userIniData);
                    }
                }
                catch
                {
                    DaggerfallUnity.LogMessage("Failed to write settings.ini.");
                }
            }
        }

        string GetData(string sectionName, string valueName)
        {
            try
            {
                if (userIniData != null)
                    return userIniData[sectionName][valueName];
                else
                    throw new Exception();
            }
            catch
            {
                if (defaultIniData != null)
                    return defaultIniData[sectionName][valueName];
                else
                    throw new Exception("GetData() could not find settings.ini or defaults.ini.");
            }
        }

        void SetData(string sectionName, string valueName, string valueData)
        {
            try
            {
                if (userIniData != null)
                    userIniData[sectionName][valueName] = valueData;
                else
                    throw new Exception();
            }
            catch
            {
                throw new Exception(string.Format("Failed to SetData() for value {0}.", valueName));
            }
        }

        string GetString(string sectionName, string valueName)
        {
            return GetData(sectionName, valueName);
        }

        void SetString(string sectionName, string valueName, string value)
        {
            SetData(sectionName, valueName, value);
        }

        bool GetBool(string sectionName, string valueName)
        {
            try
            {
                return bool.Parse(GetData(sectionName, valueName));
            }
            catch (Exception ex)
            {
                Debug.LogWarningFormat("GetBool() could not read value [{0}]{1}. Returning False. The exception was '{2}'.", sectionName, valueName, ex.Message);
                return false;
            }
        }

        void SetBool(string sectionName, string valueName, bool value)
        {
            SetData(sectionName, valueName, value.ToString());
        }

        int GetInt(string sectionName, string valueName)
        {
            try
            {
                return int.Parse(GetData(sectionName, valueName));
            }
            catch (Exception ex)
            {
                Debug.LogWarningFormat("GetInt() could not read value [{0}]{1}. Returning 0. The exception was '{2}'.", sectionName, valueName, ex.Message);
                return 0;
            }
        }

        int GetInt(string sectionName, string valueName, int min, int max)
        {
            try
            {
                int value = int.Parse(GetData(sectionName, valueName));
                return Mathf.Clamp(value, min, max);
            }
            catch (Exception ex)
            {
                Debug.LogWarningFormat("GetInt() could not read value [{0}]{1}. Returning {2}. The exception was '{3}'.", sectionName, valueName, min, ex.Message);
                return min;
            }
        }

        void SetInt(string sectionName, string valueName, int value)
        {
            SetData(sectionName, valueName, value.ToString());
        }

        float GetFloat(string sectionName, string valueName)
        {
            try
            {
                return float.Parse(GetData(sectionName, valueName), NumberStyles.Float, CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                Debug.LogWarningFormat("GetFloat() could not read value [{0}]{1}. Returning 0. The exception was '{2}'.", sectionName, valueName, ex.Message);
                return 0;
            }
        }

        float GetFloat(string sectionName, string valueName, float min, float max)
        {
            try
            {
                float value = float.Parse(GetData(sectionName, valueName), NumberStyles.Float, CultureInfo.InvariantCulture);
                return Mathf.Clamp(value, min, max);
            }
            catch (Exception ex)
            {
                Debug.LogWarningFormat("GetFloat() could not read value [{0}]{1}. Returning {2}. The exception was '{3}'.", sectionName, valueName, min, ex.Message);
                return min;
            }
        }

        void SetFloat(string sectionName, string valueName, float value)
        {
            SetData(sectionName, valueName, value.ToString(CultureInfo.InvariantCulture));
        }

        Color GetColor(string sectionName, string valueName, Color defaultColor)
        {
            try
            {
                string colorStr = GetData(sectionName, valueName);
                Color result = StringToColor(colorStr);
                return result;
            }
            catch (Exception ex)
            {
                Debug.LogWarningFormat("GetColor() could not read value [{0}]{1}. Returning {2}. The exception was '{3}'.", sectionName, valueName, defaultColor, ex.Message);
                return defaultColor;
            }
        }

        void SetColor(string sectionName, string valueName, Color value)
        {
            SetData(sectionName, valueName, ColorToString(value));
        }

        #endregion

        #region Custom Parsers

        string ColorToString(Color32 color)
        {
            return string.Format("{0:X02}{1:X02}{2:X02}{3:X02}", color.r, color.g, color.b, color.a);
        }

        Color32 StringToColor(string colorStr)
        {
            const string errorMsg = "Color must be in 32-bit RGBA format, e.g. black is 000000FF.";

            if (colorStr.Length != 8)
                throw new Exception(errorMsg);

            byte r = byte.Parse(colorStr.Substring(0, 2), NumberStyles.HexNumber);
            byte g = byte.Parse(colorStr.Substring(2, 2), NumberStyles.HexNumber);
            byte b = byte.Parse(colorStr.Substring(4, 2), NumberStyles.HexNumber);
            byte a = byte.Parse(colorStr.Substring(6, 2), NumberStyles.HexNumber);

            return new Color32(r, g, b, a);
        }

        #endregion

        #region Syncing

        void SyncIniData()
        {
            // Default and user settings must both be loaded
            if (defaultIniData == null || userIniData == null)
                return;

            // Sync sections
            AddSections(defaultIniData, userIniData);
            RemoveSections(defaultIniData, userIniData);

            // Sync settings
            foreach (var srcSection in defaultIniData.Sections)
            {
                SectionData dstSection = userIniData.Sections.GetSectionData(srcSection.SectionName);
                if (dstSection != null)
                {
                    AddSettings(srcSection, dstSection);
                    RemoveSettings(srcSection, dstSection);
                }
            }

            // Write updated settings
            WriteSettingsFile();
        }

        void AddSections(IniData srcData, IniData dstData)
        {
            // Must have data
            if (srcData == null || dstData == null)
                return;

            // Add any sections missing from source
            foreach (var section in srcData.Sections)
            {
                if (!dstData.Sections.ContainsSection(section.SectionName))
                {
                    Debug.Log("SettingsManager: Adding section " + section.SectionName);
                    dstData.Sections.AddSection(section.SectionName);
                }
            }
        }

        void RemoveSections(IniData srcData, IniData dstData)
        {
            // Must have data
            if (srcData == null || dstData == null)
                return;

            // Gather all sections not present in source
            List<string> sectionsToRemove = new List<string>();
            foreach (var section in dstData.Sections)
            {
                if (!srcData.Sections.ContainsSection(section.SectionName))
                    sectionsToRemove.Add(section.SectionName);
            }

            // Remove sections
            foreach (var name in sectionsToRemove)
            {
                Debug.Log("SettingsManager: Removing section " + name);
                dstData.Sections.RemoveSection(name);
            }
        }

        void AddSettings(SectionData srcSection, SectionData dstSection)
        {
            // Must have data
            if (srcSection == null || dstSection == null)
                return;

            // Add source keys missing in destination
            foreach (var key in srcSection.Keys)
            {
                if (!dstSection.Keys.ContainsKey(key.KeyName))
                {
                    Debug.Log("SettingsManager: Adding setting " + key.KeyName);
                    dstSection.Keys.AddKey(key);
                }
            }
        }

        void RemoveSettings(SectionData srcSection, SectionData dstSection)
        {
            // Must have data
            if (srcSection == null || dstSection == null)
                return;

            // Gather all keys missing in source
            List<string> keysToRemove = new List<string>();
            foreach (var key in dstSection.Keys)
            {
                if (!srcSection.Keys.ContainsKey(key.KeyName))
                    keysToRemove.Add(key.KeyName);
            }

            // Remove keys
            foreach(var name in keysToRemove)
            {
                Debug.Log("SettingsManager: Removing setting " + name);
                dstSection.Keys.RemoveKey(name);
            }
        }

        #endregion
    }
}
