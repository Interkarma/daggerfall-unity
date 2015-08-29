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
using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.UserInterfaceWindows;

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// Implements Daggerfall's user interface with internal UI system.
    /// </summary>
    [RequireComponent(typeof(DaggerfallAudioSource))]
    public class DaggerfallUI : MonoBehaviour
    {
        const string popupBorderRCIFile = "SPOP.RCI";
        const string splashVideo = "ANIM0001.VID";

        public static Color DaggerfallDefaultTextColor = new Color32(243, 239, 44, 255);
        public static Color DaggerfallDefaultInputTextColor = new Color32(227, 223, 0, 255);
        public static Color DaggerfallDefaultShadowColor = new Color32(93, 77, 12, 255);
        public static Color DaggerfallDefaultSelectedTextColor = new Color32(162, 36, 12, 255);
        public static Color DaggerfallDefaultTextCursorColor = new Color32(154, 134, 0, 200);
        public static Vector2 DaggerfallDefaultShadowPos = Vector2.one;

        public FilterMode globalFilterMode = FilterMode.Point;
        public string startupMessage = string.Empty;

        DaggerfallUnity dfUnity;
        AudioSource audioSource;
        UserInterfaceManager uiManager = new UserInterfaceManager();
        bool showSplashVideo = false;

        Texture2D[] daggerfallPopupTextures;
        DaggerfallFont[] daggerfallFonts = new DaggerfallFont[4];
        AudioClip buttonClickSound;
        char lastCharacterTyped;
        KeyCode lastKeyCode;

        public DaggerfallFont Font1 { get { return GetFont(1); } }
        public DaggerfallFont Font2 { get { return GetFont(2); } }
        public DaggerfallFont Font3 { get { return GetFont(3); } }
        public DaggerfallFont Font4 { get { return GetFont(4); } }
        public DaggerfallFont Font5 { get { return GetFont(5); } }
        public DaggerfallFont DefaultFont { get { return GetFont(4); } }

        public AudioSource AudioSource
        {
            get { return audioSource; }
        }

        public AudioClip ButtonClickSound
        {
            get
            {
                if (buttonClickSound == null)
                    buttonClickSound = dfUnity.SoundReader.GetAudioClip(SoundClips.ButtonClick);

                return buttonClickSound;
            }
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

        void Awake()
        {
            dfUnity = DaggerfallUnity.Instance;
            audioSource = GetComponent<AudioSource>();
            audioSource.spatialBlend = 0;

            SetupSingleton();
            PostMessage(startupMessage);
        }

        void Update()
        {
            // Route messages to top window or handle locally
            if (uiManager.MessageCount > 0)
            {
                if (uiManager.TopWindow != null)
                    uiManager.TopWindow.ProcessMessages();
                else
                    ProcessMessages();
            }

            // Update top window
            if (uiManager.TopWindow != null)
            {
                uiManager.TopWindow.Update();
            }
        }

        void OnGUI()
        {
            // Store key downs for alternate input (e.g. text box input)
            if (Event.current.type == EventType.KeyDown)
            {
                lastCharacterTyped = Event.current.character;
                lastKeyCode = Event.current.keyCode;
            }
            else
            {
                lastCharacterTyped = (char)0;
                lastKeyCode = KeyCode.None;
            }

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
                case DaggerfallUIMessages.dfuiInitGame:
                    uiManager.PushWindow(new DaggerfallStartWindow(uiManager));
                    if (showSplashVideo)
                        uiManager.PushWindow(new DaggerfallVidPlayerWindow(uiManager, splashVideo));
                    break;
                case DaggerfallUIMessages.dfuiStartNewGameWizard:
                    uiManager.PushWindow(new StartNewGameWizard(uiManager));
                    break;
                case DaggerfallUIMessages.dfuiExitGame:
                    Application.Quit();
                    break;
            }
        }

        #region Helpers

        public static void PostMessage(string message)
        {
            if (Instance.uiManager != null)
                Instance.uiManager.PostMessage(message);
        }

        public DaggerfallFont GetFont(int index)
        {
            switch (index)
            {
                case 1:
                    if (daggerfallFonts[0] == null) daggerfallFonts[0] = new DaggerfallFont(dfUnity.Arena2Path, DaggerfallFont.FontName.FONT0000);
                    daggerfallFonts[0].FilterMode = globalFilterMode;
                    return daggerfallFonts[0];
                case 2:
                    if (daggerfallFonts[1] == null) daggerfallFonts[1] = new DaggerfallFont(dfUnity.Arena2Path, DaggerfallFont.FontName.FONT0001);
                    daggerfallFonts[1].FilterMode = globalFilterMode;
                    return daggerfallFonts[1];
                case 3:
                    if (daggerfallFonts[2] == null) daggerfallFonts[2] = new DaggerfallFont(dfUnity.Arena2Path, DaggerfallFont.FontName.FONT0002);
                    daggerfallFonts[2].FilterMode = globalFilterMode;
                    return daggerfallFonts[2];
                case 4:
                default:
                    if (daggerfallFonts[3] == null) daggerfallFonts[3] = new DaggerfallFont(dfUnity.Arena2Path, DaggerfallFont.FontName.FONT0003);
                    daggerfallFonts[3].FilterMode = globalFilterMode;
                    return daggerfallFonts[3];
                case 5:
                    if (daggerfallFonts[4] == null) daggerfallFonts[4] = new DaggerfallFont(dfUnity.Arena2Path, DaggerfallFont.FontName.FONT0004);
                    daggerfallFonts[4].FilterMode = globalFilterMode;
                    return daggerfallFonts[4];
            }
        }

        public Texture2D GetDaggerfallPopupSlice(Slices slice)
        {
            LoadDaggerfallPopupTextures();

            switch (slice)
            {
                case Slices.TopLeft:
                    return daggerfallPopupTextures[0];
                case Slices.Top:
                    return daggerfallPopupTextures[1];
                case Slices.TopRight:
                    return daggerfallPopupTextures[2];
                case Slices.Left:
                    return daggerfallPopupTextures[3];
                default:
                case Slices.Fill:
                    return daggerfallPopupTextures[4];
                case Slices.Right:
                    return daggerfallPopupTextures[5];
                case Slices.BottomLeft:
                    return daggerfallPopupTextures[6];
                case Slices.Bottom:
                    return daggerfallPopupTextures[7];
                case Slices.BottomRight:
                    return daggerfallPopupTextures[8];
            }
        }

        public void SetDaggerfallPopupStyle(Panel panel)
        {
            panel.BackgroundTexture = null;
            panel.BackgroundColor = Color.clear;

            panel.SetBorderTextures(
                GetDaggerfallPopupSlice(Slices.TopLeft),
                GetDaggerfallPopupSlice(Slices.Top),
                GetDaggerfallPopupSlice(Slices.TopRight),
                GetDaggerfallPopupSlice(Slices.Left),
                GetDaggerfallPopupSlice(Slices.Fill),
                GetDaggerfallPopupSlice(Slices.Right),
                GetDaggerfallPopupSlice(Slices.BottomLeft),
                GetDaggerfallPopupSlice(Slices.Bottom),
                GetDaggerfallPopupSlice(Slices.BottomRight),
                globalFilterMode);

            panel.SetMargins(Margins.All, 16);
        }

        void LoadDaggerfallPopupTextures()
        {
            // Load borders on first call
            if (daggerfallPopupTextures == null || daggerfallPopupTextures.Length == 0)
            {
                CifRciFile cif = new CifRciFile(Path.Combine(dfUnity.Arena2Path, popupBorderRCIFile), FileUsage.UseMemory, true);
                cif.LoadPalette(Path.Combine(dfUnity.Arena2Path, cif.PaletteName));

                daggerfallPopupTextures = new Texture2D[cif.RecordCount];
                for (int i = 0; i < cif.RecordCount; i++)
                {
                    daggerfallPopupTextures[i] = TextureReader.CreateFromAPIImage(cif, i, 0, 0);
                }
            }
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
    }
}