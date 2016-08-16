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
using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Utility;
using DaggerfallConnect.Save;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Game.Utility;

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// Implements Daggerfall's user interface with internal UI system.
    /// </summary>
    [RequireComponent(typeof(DaggerfallAudioSource))]
    public class DaggerfallUI : MonoBehaviour
    {
        const string parchmentBorderRCIFile = "SPOP.RCI";
        const string splashVideo = "ANIM0001.VID";
        const string deathVideo = "ANIM0012.VID";

        public static Color DaggerfallDefaultTextColor = new Color32(243, 239, 44, 255);
        public static Color DaggerfallDefaultInputTextColor = new Color32(227, 223, 0, 255);
        public static Color DaggerfallDefaultShadowColor = new Color32(93, 77, 12, 255);
        public static Color DaggerfallAlternateShadowColor1 = new Color32(44, 60, 60, 255);
        public static Color DaggerfallDefaultSelectedTextColor = new Color32(162, 36, 12, 255);
        public static Color DaggerfallDefaultTextCursorColor = new Color32(154, 134, 0, 200);
        public static Color DaggerfallUnityDefaultToolTipBackgroundColor = new Color32(64, 64, 64, 210);
        public static Color DaggerfallUnityDefaultToolTipTextColor = new Color32(230, 230, 200, 255);
        public static Color DaggerfallUnityNotImplementedColor = new Color(1, 0, 0, 0.5f);
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

        Texture2D[] daggerfallParchmentTextures;
        DaggerfallFont[] daggerfallFonts = new DaggerfallFont[4];
        char lastCharacterTyped;
        KeyCode lastKeyCode;

        bool hudSetup = false;
        DaggerfallHUD dfHUD;
        DaggerfallPauseOptionsWindow dfPauseOptionsWindow;
        DaggerfallCharacterSheetWindow dfCharacterSheetWindow;
        DaggerfallInventoryWindow dfInventoryWindow;
        DaggerfallTravelMapWindow dfTravelMapWindow;
        DaggerfallAutomapWindow dfAutomapWindow;

        public DaggerfallFont Font1 { get { return GetFont(1); } }
        public DaggerfallFont Font2 { get { return GetFont(2); } }
        public DaggerfallFont Font3 { get { return GetFont(3); } }
        public DaggerfallFont Font4 { get { return GetFont(4); } }
        public DaggerfallFont Font5 { get { return GetFont(5); } }

        public UserInterfaceManager UserInterfaceManager { get { return uiManager; } }

        public static DaggerfallFont DefaultFont { get { return Instance.GetFont(4); } }
        public static DaggerfallFont TitleFont { get { return Instance.GetFont(2); } }
        public static IUserInterfaceManager UIManager { get { return Instance.uiManager; } }

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

        public enum PopupStyle
        {
            Parchment,
        }

        void Awake()
        {
            dfUnity = DaggerfallUnity.Instance;
            audioSource = GetComponent<AudioSource>();
            audioSource.spatialBlend = 0;
            dfAudioSource = GetComponent<DaggerfallAudioSource>();
            dfSongPlayer = GetComponent<DaggerfallSongPlayer>();

            dfPauseOptionsWindow = new DaggerfallPauseOptionsWindow(uiManager);
            //dfPauseOptionsWindow.OnClose += PauseOptionsDialog_OnClose;

            dfCharacterSheetWindow = new DaggerfallCharacterSheetWindow(uiManager);
            //dfCharacterSheetWindow.OnClose += CharacterSheetWindow_OnClose;

            dfInventoryWindow = new DaggerfallInventoryWindow(uiManager);
            //dfInventoryWindow.OnClose += InventoryWindow_OnClose;

            dfTravelMapWindow = new DaggerfallTravelMapWindow(uiManager);

            dfAutomapWindow = new DaggerfallAutomapWindow(uiManager);
            //dfAutomapWindow.OnClose += AutomapDialog_OnClose;

            SetupSingleton();
        }

        void Start()
        {
            // Post start message
            PostMessage(startupMessage);
        }

        void Update()
        {
            // HUD is always first window on stack when ready
            if (dfUnity.IsPathValidated && !hudSetup)
            {
                if (enableHUD)
                {
                    dfHUD = new DaggerfallHUD(uiManager);
                    uiManager.PushWindow(dfHUD);
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
                uiManager.TopWindow.Update();
            }

            // Clear key state every frame
            lastCharacterTyped = (char)0;
            lastKeyCode = KeyCode.None;
        }

        void OnGUI()
        {
            // Store key downs for alternate input (e.g. text box input)
            // Possible to get multiple keydown events per frame, one with character, one with keycode
            // Only accept character or keycode if valid
            if (Event.current.type == EventType.KeyDown)
            {
                if (Event.current.character != (char)0)
                    lastCharacterTyped = Event.current.character;

                if (Event.current.keyCode != KeyCode.None)
                    lastKeyCode = Event.current.keyCode;
            }

            // Set depth of GUI to appear on top of other elements
            GUI.depth = 0;

            // Draw top window
            if (uiManager.TopWindow != null)
            {
                uiManager.TopWindow.Draw();
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
                case DaggerfallUIMessages.dfuiOpenTravelMapWindow:
                    if(!GameManager.Instance.IsPlayerInside)        //TODO: pop-up when try to travel near enemies
                        uiManager.PushWindow(dfTravelMapWindow);
                    break;
                case DaggerfallUIMessages.dfuiOpenAutomap:
                    if (GameManager.Instance.PlayerEnterExit.IsPlayerInside) // open automap only if player is in interior or dungeon - TODO: location automap for exterior locations
                    {
                        GameManager.Instance.PauseGame(true);
                        uiManager.PushWindow(dfAutomapWindow);
                    }
                    break;
                case DaggerfallUIMessages.dfuiOpenRestWindow:
                    uiManager.PushWindow(new DaggerfallRestWindow(uiManager));
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

        public DaggerfallFont GetFont(int index)
        {
            // Set path
            string path = string.Empty;
            if (dfUnity.IsPathValidated)
                path = dfUnity.Arena2Path;

            // Try to load font from either Daggerfall path or Resources
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
                    globalFilterMode);
            }

            panel.SetMargins(Margins.All, 12);
        }

        void LoadDaggerfallParchmentTextures()
        {
            if (daggerfallParchmentTextures == null || daggerfallParchmentTextures.Length == 0)
            {
                CifRciFile cif = new CifRciFile(Path.Combine(dfUnity.Arena2Path, parchmentBorderRCIFile), FileUsage.UseMemory, true);
                cif.LoadPalette(Path.Combine(dfUnity.Arena2Path, cif.PaletteName));

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
                audioSource.PlayOneShot(clip);
        }

        public void PlayOneShot(SoundClips clip)
        {
            if (dfAudioSource)
                dfAudioSource.PlayOneShot(clip, 0);
        }

        public void FadeHUDToBlack(float fadeDuration = 0.4f)
        {
            if (dfHUD == null)
                return;

            dfHUD.ParentPanel.BackgroundColor = Color.clear;
            StartCoroutine(FadePanelBackground(dfHUD.ParentPanel, dfHUD.ParentPanel.BackgroundColor, Color.black, fadeDuration));
        }

        public void FadeHUDFromBlack(float fadeDuration = 0.4f)
        {
            if (dfHUD == null)
                return;

            dfHUD.ParentPanel.BackgroundColor = Color.clear;
            StartCoroutine(FadePanelBackground(dfHUD.ParentPanel, Color.black, dfHUD.ParentPanel.BackgroundColor, fadeDuration));
        }

        public void ClearFade()
        {
            if (dfHUD == null)
                return;

            dfHUD.ParentPanel.BackgroundColor = Color.clear;
        }

#endregion

#region Static Helpers

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

        public static TextLabel AddTextLabel(PixelFont font, Vector2 position, string text, Panel panel = null, int glyphSpacing = 1)
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

        public static TextLabel AddDefaultShadowedTextLabel(Vector2 position, Panel panel = null, int glyphSpacing = 1)
        {
            TextLabel textLabel = AddTextLabel(DefaultFont, position, string.Empty, panel, glyphSpacing);
            textLabel.TextColor = DaggerfallDefaultTextColor;
            textLabel.ShadowColor = DaggerfallAlternateShadowColor1;
            textLabel.ShadowPosition = DaggerfallDefaultShadowPos;

            return textLabel;
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

        public static Texture2D GetTextureFromImg(string name, TextureFormat format = TextureFormat.ARGB32)
        {
            DFPosition offset;
            Texture2D texture = GetTextureFromImg(name, out offset, format);

            return texture;
        }

        /// <summary>
        /// Loads IMG file to texture using a subrect of source image.
        /// Origin of source image (0,0) is bottom-left corner.
        /// </summary>
        public static Texture2D GetTextureFromImg(string name, Rect subRect, TextureFormat format = TextureFormat.ARGB32)
        {
            ImgFile imgFile = new ImgFile(Path.Combine(DaggerfallUnity.Instance.Arena2Path, name), FileUsage.UseMemory, true);
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

        public static Texture2D GetTextureFromImg(string name, out DFPosition offset, TextureFormat format = TextureFormat.ARGB32)
        {
            offset = new DFPosition();

            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;
            if (!dfUnity.IsReady)
                return null;

            ImgFile imgFile = new ImgFile(Path.Combine(dfUnity.Arena2Path, name), FileUsage.UseMemory, true);
            imgFile.LoadPalette(Path.Combine(dfUnity.Arena2Path, imgFile.PaletteName));
            Texture2D texture = GetTextureFromImg(imgFile, format);
            texture.filterMode = DaggerfallUI.Instance.GlobalFilterMode;

            offset = imgFile.ImageOffset;

            return texture;
        }

        public static Texture2D GetTextureFromImg(ImgFile img, TextureFormat format = TextureFormat.ARGB32)
        {
            DFBitmap bitmap = img.GetDFBitmap();
            Texture2D texture = new Texture2D(bitmap.Width, bitmap.Height, format, false);
            texture.SetPixels32(img.GetColor32(bitmap, 0));
            texture.Apply(false, true);
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
            cifRciFile.LoadPalette(Path.Combine(dfUnity.Arena2Path, cifRciFile.PaletteName));
            DFBitmap bitmap = cifRciFile.GetDFBitmap(record, frame);
            Texture2D texture = new Texture2D(bitmap.Width, bitmap.Height, format, false);
            texture.SetPixels32(cifRciFile.GetColor32(bitmap, 0));
            texture.Apply(false, true);
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

        public static DaggerfallMessageBox MessageBox(string message)
        {
            DaggerfallMessageBox messageBox = new DaggerfallMessageBox(Instance.uiManager, Instance.uiManager.TopWindow);
            messageBox.SetText(message);
            messageBox.ClickAnywhereToClose = true;
            messageBox.Show();
            return messageBox;
        }

        public static DaggerfallMessageBox MessageBox(string[] message)
        {
            DaggerfallMessageBox messageBox = new DaggerfallMessageBox(Instance.uiManager, Instance.uiManager.TopWindow);
            messageBox.SetText(message);
            messageBox.ClickAnywhereToClose = true;
            messageBox.Show();
            return messageBox;
        }

        public static DaggerfallMessageBox MessageBox(int id)
        {
            DaggerfallMessageBox messageBox = new DaggerfallMessageBox(Instance.uiManager, Instance.uiManager.TopWindow);
            messageBox.SetTextTokens(id);
            messageBox.ClickAnywhereToClose = true;
            messageBox.Show();
            return messageBox;
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
            dfUnityOut = GameObject.FindObjectOfType(typeof(DaggerfallUI)) as DaggerfallUI;
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

#region Event Handlers

        //private void PauseOptionsDialog_OnClose()
        //{
        //    GameManager.Instance.PauseGame(false);
        //}

        //private void CharacterSheetWindow_OnClose()
        //{
        //    GameManager.Instance.PauseGame(false);
        //}

        //private void InventoryWindow_OnClose()
        //{
        //    GameManager.Instance.PauseGame(false);
        //}

        //private void AutomapDialog_OnClose()
        //{
        //    GameManager.Instance.PauseGame(false);
        //}

#endregion

#region Private Methods

        // Fades HUD background in from black to briefly hide world while loading
        IEnumerator FadePanelBackground(Panel target, Color startColor, Color targetColor, float fadeDuration = 0.4f, bool popWindowOnCompletion = false)
        {
            const float fadeStep = 0.02f;

            // Must have a HUD to fade
            if (dfHUD == null)
                yield break;

            // Setup fade
            target.BackgroundColor = startColor;

            // Progress fade
            float progress = 0;
            float increment = fadeStep / fadeDuration;
            while (progress < 1)
            {
                target.BackgroundColor = Color.Lerp(startColor, targetColor, progress);
                progress += increment;
                yield return new WaitForSeconds(fadeStep);
            }

            // Ensure starting colour is restored
            target.BackgroundColor = targetColor;

            // Pop window if specified
            if (popWindowOnCompletion)
                uiManager.PopWindow();
        }

        // Re-init game with specified video
        void InitGame(string video)
        {
            if (dfUnity.IsPathValidated)
            {
                uiManager.PushWindow(new DaggerfallStartWindow(uiManager));
                if (!string.IsNullOrEmpty(video) && enableVideos)
                    uiManager.PushWindow(new DaggerfallVidPlayerWindow(uiManager, splashVideo));
            }
        }

#endregion
    }
}