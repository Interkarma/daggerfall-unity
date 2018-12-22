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
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Utility;
using DaggerfallConnect.Save;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Utility.AssetInjection;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Game.MagicAndEffects;
using DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects;

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// Implements Daggerfall's user interface with internal UI system.
    /// </summary>
    [RequireComponent(typeof(FadeBehaviour))]
    [RequireComponent(typeof(DaggerfallAudioSource))]
    public class DaggerfallUI : MonoBehaviour
    {
        const string fontsFolderName = "Fonts";
        const string parchmentBorderRCIFile = "SPOP.RCI";
        const string splashVideo = "ANIM0001.VID";
        const string deathVideo = "ANIM0012.VID";

        public static Color DaggerfallDefaultTextColor = new Color32(243, 239, 44, 255);
        public static Color DaggerfallDefaultInputTextColor = new Color32(227, 223, 0, 255);
        public static Color DaggerfallHighlightTextColor = new Color32(219, 130, 40, 255);
        public static Color DaggerfallAlternateHighlightTextColor = new Color32(255, 130, 40, 255);
        public static Color DaggerfallQuestionTextColor = new Color(0.698f, 0.812f, 1.0f);
        public static Color DaggerfallAnswerTextColor = DaggerfallDefaultInputTextColor;
        public static Color DaggerfallDefaultShadowColor = new Color32(93, 77, 12, 255);
        public static Color DaggerfallAlternateShadowColor1 = new Color32(44, 60, 60, 255);
        public static Color DaggerfallDefaultSelectedTextColor = new Color32(162, 36, 12, 255);
        public static Color DaggerfallBrighterSelectedTextColor = new Color32(254, 56, 18, 255);
        public static Color DaggerfallUnityStatDrainedTextColor = new Color32(190, 85, 24, 255);
        public static Color DaggerfallUnityStatIncreasedTextColor = new Color32(178, 207, 255, 255);
        public static Color DaggerfallDefaultTextCursorColor = new Color32(154, 134, 0, 200);
        public static Color DaggerfallUnityDefaultToolTipBackgroundColor = new Color32(64, 64, 64, 210);
        public static Color DaggerfallUnityDefaultToolTipTextColor = new Color32(230, 230, 200, 255);
        public static Color DaggerfallUnityDefaultCheckboxToggleColor = new Color32(146, 12, 4, 255);
        public static Color DaggerfallUnityNotImplementedColor = new Color(1, 0, 0, 0.5f);
        public static Color DaggerfallPrisonDaysUntilFreedomColor = new Color32(232, 196, 76, 255);
        public static Color DaggerfallPrisonDaysUntilFreedomShadowColor = new Color32(48, 36, 20, 255);
        public static Vector2 DaggerfallDefaultShadowPos = Vector2.one;

        public FilterMode globalFilterMode = FilterMode.Point;
        public string startupMessage = string.Empty;
        public bool enableHUD = false;
        public bool enableVideos = true;

        DaggerfallUnity dfUnity;
        AudioSource audioSource;
        DaggerfallAudioSource dfAudioSource;
        DaggerfallSongPlayer dfSongPlayer;
        UserInterfaceManager uiManager = new UserInterfaceManager();
        UserInterfaceRenderTarget customRenderTarget = null;
        Vector2? customMousePosition = null;

        SpellIconCollection spellIconCollection;

        Texture2D[] daggerfallParchmentTextures;
        Vector2Int daggerfallParchmentTexturesSize;
        DaggerfallFont[] daggerfallFonts = new DaggerfallFont[5];
        char lastCharacterTyped;
        KeyCode lastKeyCode;
        FadeBehaviour fadeBehaviour = null;

        bool hudSetup = false;
        DaggerfallHUD dfHUD;
        DaggerfallPauseOptionsWindow dfPauseOptionsWindow;
        DaggerfallCharacterSheetWindow dfCharacterSheetWindow;
        DaggerfallInventoryWindow dfInventoryWindow;
        DaggerfallControlsWindow dfControlsWindow;
        DaggerfallJoystickControlsWindow dfJoystickControlsWindow;
        DaggerfallUnityMouseControlsWindow dfUnityMouseControlsWindow;
        DaggerfallTravelMapWindow dfTravelMapWindow;
        DaggerfallAutomapWindow dfAutomapWindow;
        DaggerfallExteriorAutomapWindow dfExteriorAutomapWindow;
        DaggerfallBookReaderWindow dfBookReaderWindow;
        DaggerfallTalkWindow dfTalkWindow;
        DaggerfallQuestJournalWindow dfQuestJournalWindow;
        DaggerfallPlayerHistoryWindow dfPlayerHistoryWindow;
        DaggerfallSpellBookWindow dfSpellBookWindow;
        DaggerfallUseMagicItemWindow dfUseMagicItemWindow;
        DaggerfallSpellMakerWindow dfSpellMakerWindow;
        DaggerfallItemMakerWindow dfItemMakerWindow;
        DaggerfallPotionMakerWindow dfPotionMakerWindow;
        DaggerfallCourtWindow dfCourtWindow;

        Material pixelFontMaterial;
        Material sdfFontMaterial;

        Questing.Actions.GivePc lastPendingOfferSender = null;

        public DaggerfallFont Font1 { get { return GetFont(1); } }
        public DaggerfallFont Font2 { get { return GetFont(2); } }
        public DaggerfallFont Font3 { get { return GetFont(3); } }
        public DaggerfallFont Font4 { get { return GetFont(4); } }
        public DaggerfallFont Font5 { get { return GetFont(5); } }

        public UserInterfaceManager UserInterfaceManager { get { return uiManager; } }

        public static DaggerfallFont LargeFont { get { return Instance.GetFont(1); } }
        public static DaggerfallFont TitleFont { get { return Instance.GetFont(2); } }
        public static DaggerfallFont SmallFont { get { return Instance.GetFont(3); } }
        public static DaggerfallFont DefaultFont { get { return Instance.GetFont(4); } }
        
        public static IUserInterfaceManager UIManager { get { return Instance.uiManager; } }

        public Material PixelFontMaterial { get { return pixelFontMaterial; } set { pixelFontMaterial = value; } }
        public Material SDFFontMaterial { get { return sdfFontMaterial; } set { sdfFontMaterial = value; } }

        public UserInterfaceRenderTarget CustomRenderTarget
        {
            get { return customRenderTarget; }
            set { customRenderTarget = value; }
        }

        public Vector2? CustomMousePosition
        {
            get { return customMousePosition; }
            set { customMousePosition = value; }
        }

        public AudioSource AudioSource
        {
            get { return audioSource; }
        }

        public DaggerfallAudioSource DaggerfallAudioSource
        {
            get { return dfAudioSource; }
        }

        public DaggerfallSongPlayer DaggerfallSongPlayer
        {
            get { return dfSongPlayer; }
        }

        public FadeBehaviour FadeBehaviour
        {
            get { return fadeBehaviour; }
        }

        public FilterMode GlobalFilterMode
        {
            get { return globalFilterMode; }
            set { globalFilterMode = value; }
        }

        public char LastCharacterTyped
        {
            get { return lastCharacterTyped; }
        }

        public KeyCode LastKeyCode
        {
            get { return lastKeyCode; }
        }

        public DaggerfallHUD DaggerfallHUD
        {
            get { return dfHUD; }
        }

        public DaggerfallInventoryWindow InventoryWindow
        {
            get { return dfInventoryWindow; }
        }

        public DaggerfallControlsWindow ControlsWindow
        {
            get { return dfControlsWindow; }
        }

        public DaggerfallJoystickControlsWindow JoystickControlsWindow
        {
            get { return dfJoystickControlsWindow; }
        }

        public DaggerfallUnityMouseControlsWindow MouseControlsWindow
        {
            get { return dfUnityMouseControlsWindow; }
        }

        public DaggerfallBookReaderWindow BookReaderWindow
        {
            get { return dfBookReaderWindow; }
        }

        public DaggerfallTalkWindow TalkWindow
        {
            get { return dfTalkWindow; }
        }

        public DaggerfallAutomapWindow AutomapWindow
        {
            get { return dfAutomapWindow; }
        }

        public DaggerfallExteriorAutomapWindow ExteriorAutomapWindow
        {
            get { return dfExteriorAutomapWindow; }
        }

        public DaggerfallTravelMapWindow DfTravelMapWindow
        {
            get { return dfTravelMapWindow; }
        }

        public DaggerfallCourtWindow DfCourtWindow
        {
            get { return dfCourtWindow; }
        }

        public DaggerfallSpellMakerWindow DfSpellMakerWindow
        {
            get { return dfSpellMakerWindow; }
        }

        public DaggerfallItemMakerWindow DfItemMakerWindow
        {
            get { return dfItemMakerWindow; }
        }

        public DaggerfallPotionMakerWindow DfPotionMakerWindow
        {
            get { return dfPotionMakerWindow; }
        }

        public string FontsFolder
        {
            get { return Path.Combine(Application.streamingAssetsPath, fontsFolderName); }
        }

        public enum PopupStyle
        {
            Parchment,
        }

        public enum HQPixelFonts
        {
            Petrock_32,
        }

        public bool ShowVersionText { get; set; }

        /// <summary>
        /// Gets spell icon collection for UI systems.
        /// </summary>
        public SpellIconCollection SpellIconCollection
        {
            get { return spellIconCollection; }
        }

        void Awake()
        {
            dfUnity = DaggerfallUnity.Instance;
            audioSource = GetComponent<AudioSource>();
            audioSource.spatialBlend = 0;
            dfAudioSource = GetComponent<DaggerfallAudioSource>();
            dfSongPlayer = GetComponent<DaggerfallSongPlayer>();
            fadeBehaviour = GetComponent<FadeBehaviour>();

            dfPauseOptionsWindow = new DaggerfallPauseOptionsWindow(uiManager);
            dfCharacterSheetWindow = new DaggerfallCharacterSheetWindow(uiManager);
            dfInventoryWindow = new DaggerfallInventoryWindow(uiManager);
            dfControlsWindow = new DaggerfallControlsWindow(uiManager);
            dfJoystickControlsWindow = new DaggerfallJoystickControlsWindow(uiManager);
            dfUnityMouseControlsWindow = new DaggerfallUnityMouseControlsWindow(uiManager);
            dfTravelMapWindow = new DaggerfallTravelMapWindow(uiManager);
            dfAutomapWindow = new DaggerfallAutomapWindow(uiManager);
            dfBookReaderWindow = new DaggerfallBookReaderWindow(uiManager);
            dfQuestJournalWindow = new DaggerfallQuestJournalWindow(uiManager);
            dfPlayerHistoryWindow = new DaggerfallPlayerHistoryWindow(uiManager);
            dfTalkWindow = new DaggerfallTalkWindow(uiManager);
            dfSpellBookWindow = new DaggerfallSpellBookWindow(uiManager);
            dfUseMagicItemWindow = new DaggerfallUseMagicItemWindow(uiManager);
            dfSpellMakerWindow = new DaggerfallSpellMakerWindow(uiManager);
            dfItemMakerWindow = new DaggerfallItemMakerWindow(uiManager);
            dfPotionMakerWindow = new DaggerfallPotionMakerWindow(uiManager);
            dfCourtWindow = new DaggerfallCourtWindow(uiManager);
            dfExteriorAutomapWindow = new DaggerfallExteriorAutomapWindow(uiManager);

            Questing.Actions.GivePc.OnOfferPending += GivePc_OnOfferPending;

            SetupSingleton();
        }

        void Start()
        {
            // Post start message
            PostMessage(startupMessage);

            // Load spell icon collection
            spellIconCollection = new SpellIconCollection();

            // Create standard pixel font material
            if (pixelFontMaterial == null)
                pixelFontMaterial = new Material(Shader.Find(MaterialReader._DaggerfallPixelFontShaderName));

            // Create SDF font material
            if (sdfFontMaterial == null)
                sdfFontMaterial = new Material(Shader.Find(MaterialReader._DaggerfallSDFFontShaderName));
        }

        void Update()
        {
            // Toggle font rendering between classic and SDF using LeftShift+F
            if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKeyDown(KeyCode.F11))
            {
                DaggerfallUnity.Settings.SDFFontRendering = !DaggerfallUnity.Settings.SDFFontRendering;
                DaggerfallUnity.Settings.SaveSettings();
                //Debug.LogFormat("SDFFontRendering={0}", DaggerfallUnity.Settings.SDFFontRendering.ToString());
            }

            // HUD is always first window on stack when ready
            if (dfUnity.IsPathValidated && !hudSetup)
            {
                if (enableHUD)
                {
                    dfHUD = new DaggerfallHUD(uiManager);
                    uiManager.PushWindow(dfHUD);
                    fadeBehaviour.FadeTargetPanel = dfHUD.ParentPanel;
                    Debug.Log("HUD pushed to stack.");
                }
                hudSetup = true;
            }

            // Route messages to top window or handle locally
            if (uiManager.MessageCount > 0)
            {
                // Top window has first chance at message
                if (uiManager.TopWindow != null)
                    uiManager.TopWindow.ProcessMessages();

                // Then process locally
                ProcessMessages();
            }

            // Update top window
            if (uiManager.TopWindow != null)
            {
                uiManager.TopWindow.ParentPanel.CustomMousePosition = customMousePosition;
                uiManager.TopWindow.Update();
            }

            // Clear key state every frame
            lastCharacterTyped = (char)0;
            lastKeyCode = KeyCode.None;
        }

        void OnGUI()
        {
            // Set depth of GUI to appear on top of other elements
            GUI.depth = 0;

            // Store key downs for alternate input (e.g. text box input)
            // Possible to get multiple keydown events per frame, one with character, one with keycode
            // Only accept character or keycode if valid
            if (Event.current.type == EventType.KeyDown)
            {
                if (Event.current.character != (char)0)
                    lastCharacterTyped = Event.current.character;

                if (Event.current.keyCode != KeyCode.None)
                    lastKeyCode = Event.current.keyCode;

                if (lastCharacterTyped > 255)
                    lastCharacterTyped = (char)0;
            }

            if (Event.current.type == EventType.Repaint)
            {
                RenderTexture oldRT = null;
                if (customRenderTarget)
                {
                    oldRT = RenderTexture.active;
                    RenderTexture.active = customRenderTarget.TargetTexture;
                    customRenderTarget.Clear();
                }

                // Draw top window
                if (uiManager.TopWindow != null)
                {
                    uiManager.TopWindow.Draw();
                }

                if (customRenderTarget)
                {
                    RenderTexture.active = oldRT;
                }
            }
        }

        void ProcessMessages()
        {
            switch (uiManager.GetMessage())
            {
                case DaggerfallUIMessages.dfuiSetupGameWizard:
                    uiManager.PushWindow(new DaggerfallUnitySetupGameWizard(uiManager));
                    break;
                case DaggerfallUIMessages.dfuiInitGame:
                    InitGame(splashVideo);
                    break;
                case DaggerfallUIMessages.dfuiInitGameFromDeath:
                    InitGame(deathVideo);
                    break;
                case DaggerfallUIMessages.dfuiStartNewGameWizard:
                    uiManager.PushWindow(new StartNewGameWizard(uiManager));
                    break;
                case DaggerfallUIMessages.dfuiOpenPauseOptionsDialog:
                    uiManager.PushWindow(dfPauseOptionsWindow);
                    break;
                case DaggerfallUIMessages.dfuiOpenCharacterSheetWindow:
                    uiManager.PushWindow(dfCharacterSheetWindow);
                    break;
                case DaggerfallUIMessages.dfuiOpenInventoryWindow:
                    uiManager.PushWindow(dfInventoryWindow);
                    break;
                case DaggerfallUIMessages.dfuiOpenControlsWindow:
                    uiManager.PushWindow(dfControlsWindow);
                    break;
                case DaggerfallUIMessages.dfuiOpenJoystickControlsWindow:
                    uiManager.PushWindow(dfJoystickControlsWindow);
                    break;
                case DaggerfallUIMessages.dfuiOpenMouseControlsWindow:
                    uiManager.PushWindow(dfUnityMouseControlsWindow);
                    break;
                case DaggerfallUIMessages.dfuiOpenSpellBookWindow:
                    if (!GameManager.Instance.PlayerSpellCasting.IsPlayingAnim)
                    {
                        if (GameManager.Instance.PlayerEntity.Items.Contains(Items.ItemGroups.MiscItems, (int)Items.MiscItems.Spellbook))
                            uiManager.PushWindow(dfSpellBookWindow);
                        else
                            AddHUDText(TextManager.Instance.GetText("ClassicEffects", "noSpellbook"));
                    }
                    break;
                case DaggerfallUIMessages.dfuiOpenUseMagicItemWindow:
                    uiManager.PushWindow(dfUseMagicItemWindow);
                    break;
                case DaggerfallUIMessages.dfuiOpenCourtWindow:
                    uiManager.PushWindow(dfCourtWindow);
                    break;
                case DaggerfallUIMessages.dfuiOpenSpellMakerWindow:
                    uiManager.PushWindow(dfSpellMakerWindow);
                    break;
                case DaggerfallUIMessages.dfuiOpenTravelMapWindow:
                    if (GameManager.Instance.IsPlayerInside)
                    {
                        AddHUDText(HardStrings.cannotTravelIndoors);
                    }
                    else
                    {
                        if (GameManager.Instance.AreEnemiesNearby())
                        {
                            MessageBox(HardStrings.cannotTravelWithEnemiesNearby);
                        }
                        else
                        {
                            if (!GiveOffer())
                                uiManager.PushWindow(dfTravelMapWindow);
                        }
                    }
                    break;
                case DaggerfallUIMessages.dfuiOpenAutomap:
                    if (GameManager.Instance.IsPlayerInside) // open automap only if player is in interior or dungeon - TODO: location automap for exterior locations
                    {
                        GameManager.Instance.PauseGame(true);
                        uiManager.PushWindow(dfAutomapWindow);
                    }
                    else
                    {
                        ContentReader.MapSummary mapSummary;
                        DFPosition mapPixel = GameManager.Instance.PlayerGPS.CurrentMapPixel;
                        if (DaggerfallUnity.Instance.ContentReader.HasLocation(mapPixel.X, mapPixel.Y, out mapSummary))
                        {
                            // There's a location at this map pixel
                            GameManager.Instance.PauseGame(true);
                            uiManager.PushWindow(dfExteriorAutomapWindow);
                        }
                    }
                    break;
                case DaggerfallUIMessages.dfuiOpenRestWindow:
                    if (GameManager.Instance.AreEnemiesNearby())
                    {
                        // Alert player if monsters nearby
                        const int enemiesNearby = 354;
                        MessageBox(enemiesNearby);
                    }
                    else if (GameManager.Instance.PlayerEnterExit.IsPlayerSwimming ||
                             !GameManager.Instance.PlayerController.isGrounded)
                    {
                        const int cannotRestNow = 355;
                        MessageBox(cannotRestNow);
                    }
                    else
                    {
                        if (!GiveOffer())
                            uiManager.PushWindow(new DaggerfallRestWindow(uiManager));
                    }
                    break;
                case DaggerfallUIMessages.dfuiOpenTransportWindow:
                    if (GameManager.Instance.IsPlayerInside)
                    {
                        AddHUDText(HardStrings.cannotChangeTransportationIndoors);
                    }
                    else
                    {
                        if (GameManager.Instance.PlayerController.isGrounded)
                            uiManager.PushWindow(new DaggerfallTransportWindow(uiManager));
                    }
                    break;
                case DaggerfallUIMessages.dfuiOpenBookReaderWindow:
                    uiManager.PushWindow(dfBookReaderWindow);
                    break;
                case DaggerfallUIMessages.dfuiOpenQuestJournalWindow:
                    dfQuestJournalWindow.DisplayMode = DaggerfallQuestJournalWindow.JournalDisplay.ActiveQuests;
                    uiManager.PushWindow(dfQuestJournalWindow);
                    break;
                case DaggerfallUIMessages.dfuiOpenNotebookWindow:
                    dfQuestJournalWindow.DisplayMode = DaggerfallQuestJournalWindow.JournalDisplay.Notebook;
                    uiManager.PushWindow(dfQuestJournalWindow);
                    break;
                case DaggerfallUIMessages.dfuiOpenPlayerHistoryWindow:
                    uiManager.PushWindow(dfPlayerHistoryWindow);
                    break;
                case DaggerfallUIMessages.dfuiStatusInfo:
                    DisplayStatusInfo();
                    break;
                case DaggerfallUIMessages.dfuiExitGame:
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
                    break;
            }
        }

        #region Helpers

        public static void AddHUDText(string message)
        {
            if (Instance.dfHUD != null)
            {
                Instance.dfHUD.PopupText.AddText(message);
            }
        }

        public static void AddHUDText(string message, float delay)
        {
            if (Instance.dfHUD != null)
            {
                Instance.dfHUD.PopupText.AddText(message, delay);
            }
        }

        public static void AddHUDText(TextFile.Token[] tokens, float delay)
        {
            if (Instance.dfHUD != null)
            {
                Instance.dfHUD.PopupText.AddText(tokens, delay);
            }
        }

        public static void SetMidScreenText(string message, float delay = 1.5f)
        {
            if (Instance.dfHUD != null)
            {
                Instance.dfHUD.SetMidScreenText(message, delay);
            }
        }

        public static void PostMessage(string message)
        {
            if (Instance.uiManager != null)
                Instance.uiManager.PostMessage(message);
        }

        public void PopupMessage(string text)
        {
            if (dfHUD != null)
                dfHUD.PopupText.AddText(text);
        }

        /// <summary>
        /// Pops all windows down to HUD (if present).
        /// </summary>
        public void PopToHUD()
        {
            if (!enableHUD || dfHUD == null)
                return;

            while (uiManager.TopWindow != dfHUD)
                uiManager.PopWindow();
        }

        /// <summary>
        /// Gets a new DaggerfallFont.
        /// </summary>
        /// <param name="index">Index of font between 1-5 (default is 4).</param>
        /// <returns>DaggerfallFont</returns>
        public DaggerfallFont GetFont(int index = 4)
        {
            // Attempt to use StreamingAssets for FNT files
            string path = FontsFolder;
            if (!Directory.Exists(path))
            {
                Debug.LogErrorFormat("Fonts directory path {0} not found. Falling back to Arena2 folder.", path);

                // Try Arena2 path
                path = string.Empty;
                if (dfUnity.IsPathValidated)
                    path = dfUnity.Arena2Path;
            }

            // Must have a fonts path by now
            if (string.IsNullOrEmpty(path))
            {
                throw new System.Exception("DaggerfallUI could not find a valid path for fonts in StreamingAssets/Fonts or fallback Arena2 folder.");
            }

            // Try to load font from target path
            switch (index)
            {
                case 1:
                    if (daggerfallFonts[0] == null) daggerfallFonts[0] = new DaggerfallFont(path, DaggerfallFont.FontName.FONT0000);
                    daggerfallFonts[0].FilterMode = globalFilterMode;
                    return daggerfallFonts[0];
                case 2:
                    if (daggerfallFonts[1] == null) daggerfallFonts[1] = new DaggerfallFont(path, DaggerfallFont.FontName.FONT0001);
                    daggerfallFonts[1].FilterMode = globalFilterMode;
                    return daggerfallFonts[1];
                case 3:
                    if (daggerfallFonts[2] == null) daggerfallFonts[2] = new DaggerfallFont(path, DaggerfallFont.FontName.FONT0002);
                    daggerfallFonts[2].FilterMode = globalFilterMode;
                    return daggerfallFonts[2];
                case 4:
                default:
                    if (daggerfallFonts[3] == null) daggerfallFonts[3] = new DaggerfallFont(path, DaggerfallFont.FontName.FONT0003);
                    daggerfallFonts[3].FilterMode = globalFilterMode;
                    return daggerfallFonts[3];
                case 5:
                    if (daggerfallFonts[4] == null) daggerfallFonts[4] = new DaggerfallFont(path, DaggerfallFont.FontName.FONT0004);
                    daggerfallFonts[4].FilterMode = globalFilterMode;
                    return daggerfallFonts[4];
            }
        }

        public void SetDaggerfallPopupStyle(PopupStyle style, Panel panel)
        {
            // Do nothing if DaggerfallUnity path not valid
            if (!DaggerfallUnity.Instance.IsPathValidated)
                return;

            panel.BackgroundTexture = null;
            panel.BackgroundColor = Color.clear;

            if (style == PopupStyle.Parchment)
            {
                LoadDaggerfallParchmentTextures();
                panel.SetBorderTextures(
                    daggerfallParchmentTextures[0],
                    daggerfallParchmentTextures[1],
                    daggerfallParchmentTextures[2],
                    daggerfallParchmentTextures[3],
                    daggerfallParchmentTextures[4],
                    daggerfallParchmentTextures[5],
                    daggerfallParchmentTextures[6],
                    daggerfallParchmentTextures[7],
                    daggerfallParchmentTextures[8],
                    globalFilterMode,
                    new Border<Vector2Int>(daggerfallParchmentTexturesSize));
            }

            panel.SetMargins(Margins.All, 10);
        }

        void LoadDaggerfallParchmentTextures()
        {
            if (daggerfallParchmentTextures == null || daggerfallParchmentTextures.Length == 0)
            {
                CifRciFile cif = new CifRciFile(Path.Combine(dfUnity.Arena2Path, parchmentBorderRCIFile), FileUsage.UseMemory, true);
                cif.LoadPalette(Path.Combine(dfUnity.Arena2Path, cif.PaletteName));

                DFSize recordSize = cif.GetSize(0);
                daggerfallParchmentTexturesSize = new Vector2Int(recordSize.Width, recordSize.Height);
                daggerfallParchmentTextures = new Texture2D[cif.RecordCount];
                for (int i = 0; i < cif.RecordCount; i++)
                {
                    daggerfallParchmentTextures[i] = TextureReader.CreateFromAPIImage(cif, i, 0, 0);
                }
            }
        }

        public AudioClip GetAudioClip(SoundClips clip)
        {
            return dfAudioSource.GetAudioClip((int)clip);
        }

        public void PlayOneShot(AudioClip clip)
        {
            if (audioSource)
                audioSource.PlayOneShot(clip, DaggerfallUnity.Settings.SoundVolume);
        }

        public void PlayOneShot(SoundClips clip)
        {
            if (dfAudioSource)
                dfAudioSource.PlayOneShot(clip, 0);
        }

        #endregion

        #region Static Helpers

        public static void SetFocus(BaseScreenComponent control)
        {
            IUserInterfaceWindow topWindow = Instance.uiManager.TopWindow;
            if (topWindow != null)
            {
                topWindow.SetFocus(control);
            }
        }

        public static Button AddButton(Vector2 position, Vector2 size, Panel panel = null)
        {
            Button button = new Button();
            button.Position = position;
            button.Size = size;
            if (panel != null)
                panel.Components.Add(button);

            return button;
        }

        public static Button AddButton(Rect rect, Panel panel = null)
        {
            return AddButton(
                new Vector2(rect.x, rect.y),
                new Vector2(rect.width, rect.height),
                panel);
        }

        public static Button AddButton(Vector2 position, Vector2 size, string clickMessage, Panel panel = null)
        {
            Button button = AddButton(position, size, panel);
            button.ClickMessage = clickMessage;

            return button;
        }

        public static Button AddButton(Vector2 position, Vector2 size, string clickMessage, string doubleClickMessage, Panel panel = null)
        {
            Button button = AddButton(position, size, panel);
            button.ClickMessage = clickMessage;
            button.DoubleClickMessage = doubleClickMessage;

            return button;
        }

        public static Checkbox AddCheckbox(Vector2 position, bool isChecked, Panel panel = null)
        {
            Checkbox checkbox = new Checkbox();
            checkbox.Position = position;
            checkbox.Size = new Vector2(2, 2);
            checkbox.CheckBoxColor = Color.white;
            checkbox.IsChecked = isChecked;
            if (panel != null)
                panel.Components.Add(checkbox);
            return checkbox;
        }

        public static TextLabel AddTextLabel(DaggerfallFont font, Vector2 position, string text, Panel panel = null, int glyphSpacing = 1)
        {
            TextLabel textLabel = new TextLabel();
            textLabel.AutoSize = AutoSizeModes.None;
            textLabel.Font = font;
            textLabel.Position = position;
            textLabel.Text = text;
            if (panel != null)
                panel.Components.Add(textLabel);

            return textLabel;
        }

        public static TextBox AddTextBox(Rect rect, string defaultText, Panel panel = null, int maxCharacters = -1, DaggerfallFont font = null, int glyphSpacing = 1)
        {
            TextBox textBox = new TextBox(font);

            textBox.Position = new Vector2(rect.x, rect.y);
            textBox.Size = new Vector2(rect.width, rect.height);
            textBox.DefaultText = defaultText;
            textBox.MaxCharacters = maxCharacters;
            textBox.TextOffset = 2;

            if (panel != null)
                panel.Components.Add(textBox);

            return textBox;
        }

        public static TextBox AddTextBoxWithFocus(Rect rect, string defaultText, Panel panel = null, int maxCharacters = -1, DaggerfallFont font = null)
        {
            TextBox textBox = new TextBox(font);
            textBox.Position = rect.position;
            textBox.Size = rect.size;
            textBox.FixedSize = true;
            textBox.DefaultText = defaultText;
            if (maxCharacters > 0)
                textBox.MaxCharacters = maxCharacters;
            textBox.UseFocus = true;
            textBox.Outline.Enabled = true;

            if (panel != null)
                panel.Components.Add(textBox);

            return textBox;
        }

        public static Button AddTextButton(Rect rect, string text, Panel panel = null)
        {
            Button button = new UserInterface.Button();
            button.Position = new Vector2(rect.x, rect.y);
            button.Size = new Vector2(rect.width, rect.height);
            button.Outline.Enabled = true;
            button.Label.HorizontalAlignment = HorizontalAlignment.Center;
            button.Label.ShadowPosition = Vector2.zero;
            button.Label.Text = text;
            if (panel != null)
                panel.Components.Add(button);

            return button;
        }

        public static TextLabel AddDefaultShadowedTextLabel(Vector2 position, Panel panel = null, int glyphSpacing = 1)
        {
            TextLabel textLabel = AddTextLabel(DefaultFont, position, string.Empty, panel, glyphSpacing);
            textLabel.TextColor = DaggerfallDefaultTextColor;
            textLabel.ShadowColor = DaggerfallAlternateShadowColor1;
            textLabel.ShadowPosition = DaggerfallDefaultShadowPos;

            return textLabel;
        }

        public static HorizontalSlider AddSlider(Vector2 position, Action<HorizontalSlider> setIndicator, float textScale = 1, Panel panel = null)
        {
            var slider = new HorizontalSlider();
            slider.Position = position;
            slider.Size = new Vector2(80.0f, 4.0f);
            slider.DisplayUnits = 20;
            slider.BackgroundColor = new Color(0.5f, 0.5f, 0.5f, 0.3f);
            slider.TintColor = new Color(153, 153, 0);
            if (panel != null)
                panel.Components.Add(slider);

            setIndicator(slider);
            slider.IndicatorOffset = 15;
            slider.Indicator.TextScale = textScale;
            slider.Indicator.TextColor = Color.white;
            slider.Indicator.ShadowColor = Color.clear;
            slider.Indicator.HorizontalTextAlignment = TextLabel.HorizontalTextAlignmentSetting.Right;
            return slider;
        }

        public static Button AddColorPicker(Vector2 position, Color32 color, IUserInterfaceManager uiManager, DaggerfallBaseWindow previous = null, Panel panel = null)
        {
            Button preview = new Button();
            preview.Position = position;
            preview.AutoSize = AutoSizeModes.None;
            preview.Size = new Vector2(40, 6);
            preview.Outline.Enabled = true;
            preview.BackgroundColor = color;
            preview.OnMouseClick += (BaseScreenComponent sender, Vector2 pos) =>
                uiManager.PushWindow(new ColorPicker(uiManager, previous, (Button)sender));
            if (panel != null)
                panel.Components.Add(preview);
            return preview;
        }

        public static Outline AddOutline(Rect rect, Color color, Panel panel = null)
        {
            Outline outline = new Outline();
            outline.AutoSize = AutoSizeModes.None;
            outline.Color = color;
            outline.Position = new Vector2(rect.x, rect.y);
            outline.Size = new Vector2(rect.width, rect.height);
            if (panel != null)
                panel.Components.Add(outline);

            return outline;
        }

        public static Panel AddPanel(Rect rect, Panel panel = null)
        {
            Panel newPanel = new Panel();
            newPanel.AutoSize = AutoSizeModes.None;
            newPanel.Position = new Vector2(rect.x, rect.y);
            newPanel.Size = new Vector2(rect.width, rect.height);
            if (panel != null)
                panel.Components.Add(newPanel);

            return newPanel;
        }

        public static Panel AddPanel(Panel panel = null, AutoSizeModes scaling = AutoSizeModes.ResizeToFill)
        {
            Panel newPanel = new Panel();
            newPanel.AutoSize = scaling;
            if (panel != null)
                panel.Components.Add(newPanel);

            return newPanel;
        }

        public static Texture2D GetTextureFromImg(string name, TextureFormat format = TextureFormat.ARGB32, bool readOnly = true)
        {
            DFPosition offset;
            Texture2D texture = GetTextureFromImg(name, out offset, format, readOnly);

            return texture;
        }

        /// <summary>
        /// Loads IMG file to texture using a subrect of source image.
        /// Origin of source image (0,0) is bottom-left corner.
        /// </summary>
        public static Texture2D GetTextureFromImg(string name, Rect subRect, TextureFormat format = TextureFormat.ARGB32, bool readOnly = true)
        {
            ImgFile imgFile = new ImgFile(Path.Combine(DaggerfallUnity.Instance.Arena2Path, name), FileUsage.UseMemory, readOnly);
            imgFile.LoadPalette(Path.Combine(DaggerfallUnity.Instance.Arena2Path, imgFile.PaletteName));

            DFBitmap bitmap = imgFile.GetDFBitmap();
            Color32[] colors = imgFile.GetColor32(bitmap, 0);

            // Invert Y as Unity textures have origin 0,0 at bottom-left and UI expects top-left
            subRect.y = bitmap.Height - subRect.height;

            Color32[] newColors = new Color32[(int)subRect.width * (int)subRect.height];
            ImageProcessing.CopyColors(
                ref colors,
                ref newColors,
                new DFSize(bitmap.Width, bitmap.Height),
                new DFSize((int)subRect.width, (int)subRect.height),
                new DFPosition((int)subRect.x, (int)subRect.y),
                new DFPosition(0, 0),
                new DFSize((int)subRect.width, (int)subRect.height));

            Texture2D texture = new Texture2D((int)subRect.width, (int)subRect.height, format, false);
            texture.SetPixels32(newColors, 0);
            texture.Apply(false, true);
            texture.filterMode = DaggerfallUI.Instance.GlobalFilterMode;

            return texture;
        }

        public static Texture2D GetTextureFromImg(string name, out DFPosition offset, TextureFormat format = TextureFormat.ARGB32, bool readOnly = true)
        {
            offset = new DFPosition();

            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;
            if (!dfUnity.IsReady || !dfUnity.IsPathValidated)
                return null;

            ImgFile imgFile = new ImgFile(Path.Combine(dfUnity.Arena2Path, name), FileUsage.UseMemory, readOnly);
            Texture2D texture;
            if (!TextureReplacement.TryImportImage(name, out texture))
            {
                imgFile.LoadPalette(Path.Combine(dfUnity.Arena2Path, imgFile.PaletteName));
                texture = GetTextureFromImg(imgFile, format, readOnly);
            }
                
            texture.filterMode = DaggerfallUI.Instance.GlobalFilterMode;
            offset = imgFile.ImageOffset;

            return texture;
        }

        public static Texture2D GetTextureFromImg(ImgFile img, TextureFormat format = TextureFormat.ARGB32, bool readOnly = true)
        {
            DFBitmap bitmap = img.GetDFBitmap();
            Texture2D texture = new Texture2D(bitmap.Width, bitmap.Height, format, false);
            texture.SetPixels32(img.GetColor32(bitmap, 0));
            texture.Apply(false, readOnly);
            texture.filterMode = DaggerfallUI.Instance.GlobalFilterMode;

            return texture;
        }

        public static DFBitmap GetImgBitmap(string name)
        {
            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;
            if (!dfUnity.IsReady)
                return null;

            ImgFile imgFile = new ImgFile(Path.Combine(dfUnity.Arena2Path, name), FileUsage.UseMemory, true);
            return imgFile.GetDFBitmap();
        }

        public static Texture2D GetTextureFromCifRci(string name, int record, int frame = 0, TextureFormat format = TextureFormat.ARGB32)
        {
            DFPosition offset;
            Texture2D texture = GetTextureFromCifRci(name, record, out offset, frame, format);

            return texture;
        }

        public static Texture2D GetTextureFromCifRci(string name, int record, out DFPosition offset, int frame = 0, TextureFormat format = TextureFormat.ARGB32)
        {
            offset = new DFPosition();
            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;
            if (!dfUnity.IsReady)
                return null;

            CifRciFile cifRciFile = new CifRciFile(Path.Combine(dfUnity.Arena2Path, name), FileUsage.UseMemory, true);
            Texture2D texture;
            if (!TextureReplacement.TryImportCifRci(name, record, frame, out texture))
            {
                cifRciFile.LoadPalette(Path.Combine(dfUnity.Arena2Path, cifRciFile.PaletteName));
                DFBitmap bitmap = cifRciFile.GetDFBitmap(record, frame);
                texture = new Texture2D(bitmap.Width, bitmap.Height, format, false);
                texture.SetPixels32(cifRciFile.GetColor32(bitmap, 0));
                texture.Apply(false, true);
            }
            texture.filterMode = DaggerfallUI.Instance.GlobalFilterMode;

            offset = cifRciFile.GetOffset(record);

            return texture;
        }

        public static Texture2D GetTextureFromSaveImage(SaveImage image, TextureFormat format = TextureFormat.ARGB32)
        {
            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;
            if (!dfUnity.IsReady)
                return null;

            DFBitmap bitmap = image.GetDFBitmap();
            Texture2D texture = new Texture2D(bitmap.Width, bitmap.Height, format, false);
            texture.SetPixels32(image.GetColor32(bitmap, 0));
            texture.Apply(false, true);
            texture.filterMode = DaggerfallUI.Instance.GlobalFilterMode;

            return texture;
        }

        /// <summary>
        /// Get a texture from resources with modding support.
        /// </summary>
        public static Texture2D GetTextureFromResources(string name)
        {
            Texture2D tex;
            return TextureReplacement.TryImportTexture(name, out tex) ? tex : Resources.Load<Texture2D>(name);
        }

        /// <summary>
        /// Get a texture from resources with modding support.
        /// Size is read from xml or set as texture size.
        /// </summary>
        public static Texture2D GetTextureFromResources(string name, out Vector2 size)
        {
            Texture2D tex = GetTextureFromResources(name);
            if (!TextureReplacement.TryGetSize(name, out size))
                size = new Vector2(tex.width, tex.height);
            return tex;
        }

        public static DaggerfallMessageBox MessageBox(string message, bool wrapText = false, IMacroContextProvider mcp = null)
        {
            DaggerfallMessageBox messageBox = new DaggerfallMessageBox(Instance.uiManager, Instance.uiManager.TopWindow, wrapText);
            messageBox.SetText(message, mcp);
            messageBox.ClickAnywhereToClose = true;
            messageBox.Show();
            return messageBox;
        }

        public static DaggerfallMessageBox MessageBox(string[] message, IMacroContextProvider mcp = null)
        {
            DaggerfallMessageBox messageBox = new DaggerfallMessageBox(Instance.uiManager, Instance.uiManager.TopWindow);
            messageBox.SetText(message, mcp);
            messageBox.ClickAnywhereToClose = true;
            messageBox.Show();
            return messageBox;
        }

        public static DaggerfallMessageBox MessageBox(int id, IMacroContextProvider mcp = null)
        {
            DaggerfallMessageBox messageBox = new DaggerfallMessageBox(Instance.uiManager, Instance.uiManager.TopWindow);
            messageBox.SetTextTokens(id, mcp);
            messageBox.ClickAnywhereToClose = true;
            messageBox.Show();
            return messageBox;
        }

        public static Texture2D CreateSolidTexture(Color color, int dim)
        {
            Texture2D texture = null;

            texture = new Texture2D(dim, dim);
            Color32[] colors = new Color32[dim * dim];
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = color;
            }
            texture.SetPixels32(colors);
            texture.Apply(false, true);
            texture.filterMode = FilterMode.Point;

            return texture;
        }

        #endregion

        #region Singleton

        static DaggerfallUI instance = null;
        public static DaggerfallUI Instance
        {
            get
            {
                if (instance == null)
                {
                    if (!FindDaggerfallUI(out instance))
                    {
                        GameObject go = new GameObject();
                        go.name = "DaggerfallUI";
                        instance = go.AddComponent<DaggerfallUI>();
                    }
                }
                return instance;
            }
        }

        public static bool HasInstance
        {
            get
            {
                return (instance != null);
            }
        }

        public static bool FindDaggerfallUI(out DaggerfallUI dfUnityOut)
        {
            dfUnityOut = GameObject.FindObjectOfType<DaggerfallUI>();
            if (dfUnityOut == null)
            {
                DaggerfallUnity.LogMessage("Could not locate DaggerfallUI GameObject instance in scene!", true);
                return false;
            }

            return true;
        }

        private void SetupSingleton()
        {
            if (instance == null)
                instance = this;
            else if (instance != this)
            {
                if (Application.isPlaying)
                {
                    DaggerfallUnity.LogMessage("Multiple DaggerfallUI instances detected in scene!", true);
                    Destroy(gameObject);
                }
            }
        }

        #endregion

        #region Private Methods

        void DisplayStatusInfo()
        {
            // Setup status info as the first message box.
            DaggerfallMessageBox statusBox = new DaggerfallMessageBox(Instance.uiManager, Instance.uiManager.TopWindow);
            statusBox.ExtraProceedBinding = InputManager.Instance.GetBinding(InputManager.Actions.Status); // set proceed binding for statusBox to key binding for status window
            statusBox.SetTextTokens(22);

            // Setup health info as the second message box.
            DaggerfallMessageBox healthBox = CreateHealthStatusBox(statusBox);
            healthBox.ExtraProceedBinding = InputManager.Instance.GetBinding(InputManager.Actions.Status); // set proceed binding for healthBox to key binding for status window
            statusBox.AddNextMessageBox(healthBox);

            statusBox.Show();
        }

        public DaggerfallMessageBox CreateHealthStatusBox(IUserInterfaceWindow previous = null)
        {
            const int youAreHealthyID = 18;
            const int youHaveBeenPoisoned = 117;

            DaggerfallMessageBox healthBox = new DaggerfallMessageBox(uiManager, previous);

            // Show "You are healthy." if there are no diseases and no poisons
            int diseaseCount = GameManager.Instance.PlayerEffectManager.DiseaseCount;
            int poisonCount = GameManager.Instance.PlayerEffectManager.PoisonCount;
            if (diseaseCount == 0 && poisonCount == 0)
            {
                healthBox.SetTextTokens(youAreHealthyID);
            }
            else
            {
                EntityEffectManager playerEffectManager = GameManager.Instance.PlayerEffectManager;

                // Get disease descriptions for each disease effect
                TextFile.Token[] tokens = null;
                LiveEffectBundle[] bundles = playerEffectManager.DiseaseBundles;
                foreach (LiveEffectBundle bundle in bundles)
                {
                    foreach (IEntityEffect effect in bundle.liveEffects)
                    {
                        if (effect is DiseaseEffect)
                        {
                            DiseaseEffect disease = (DiseaseEffect)effect;
                            if (disease.IncubationOver)
                            {
                                if (tokens == null)
                                {
                                    tokens = disease.ContractedMessageTokens;
                                }
                                else // Concatenate descriptions for multiple diseases with a new line in-between
                                {
                                    tokens = TextFile.AppendTokens(tokens, disease.ContractedMessageTokens);
                                }
                            }
                        }
                    }
                }

                // Only report poisoning if one or more poisons are active
                bool poisonActive = false;
                LiveEffectBundle[] poisonBundles = playerEffectManager.PoisonBundles;
                foreach(LiveEffectBundle poisonBundle in poisonBundles)
                {
                    foreach (IEntityEffect effect in poisonBundle.liveEffects)
                    {
                        if (effect is PoisonEffect)
                        {
                            PoisonEffect poison = (PoisonEffect)effect;
                            if (poison.CurrentState != PoisonEffect.PoisonStates.Waiting)
                            {
                                poisonActive = true;
                                break;
                            }
                        }
                    }
                }

                // Append "You have been poisoned." if one or more poisons present and active
                if (poisonCount > 0 && poisonActive)
                    tokens = TextFile.AppendTokens(tokens, DaggerfallUnity.Instance.TextProvider.GetRSCTokens(youHaveBeenPoisoned));

                // If no diseases were done with incubation, or no poisons, show "You are healthy."
                if (tokens == null)
                    healthBox.SetTextTokens(youAreHealthyID);
                else
                    healthBox.SetTextTokens(tokens);
            }
            healthBox.ClickAnywhereToClose = true;
            return healthBox;
        }

        // Re-init game with specified video
        void InitGame(string video)
        {
            if (dfUnity.IsPathValidated)
            {
                uiManager.PushWindow(new DaggerfallStartWindow(uiManager));
                if (!string.IsNullOrEmpty(video) && enableVideos)
                    uiManager.PushWindow(new DaggerfallVidPlayerWindow(uiManager, video));
            }
        }

        bool GiveOffer()
        {
            if (lastPendingOfferSender != null)
            {
                lastPendingOfferSender.OfferImmediately();
                lastPendingOfferSender = null;
                return true;
            }

            return false;
        }

        #endregion

        #region Events

        private void GivePc_OnOfferPending(Questing.Actions.GivePc sender)
        {
            lastPendingOfferSender = sender;
        }

        #endregion
    }
}
