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
        const string uiBootstrapMessage = DaggerfallUIMessages.dfuiOpenBookReaderWindow;

        public static Color DaggerfallDefaultTextColor = new Color32(243, 239, 44, 255);
        public static Color DaggerfallDefaultShadowColor = new Color32(93, 77, 12, 255);
        public static Vector2 DaggerfallDefaultShadowPos = Vector2.one;

        DaggerfallUnity dfUnity;
        AudioSource audioSource;
        UserInterfaceManager uiManager = new UserInterfaceManager();

        DaggerfallStartWindow dfStartWindow;
        DaggerfallLoadSavedGameWindow dfLoadGameWindow;
        DaggerfallBookReaderWindow dfBookReaderWindow;
        DaggerfallVidPlayerWindow dfVidPlayerWindow;
        DaggerfallRaceSelectWindow dfRaceSelectWindow;

        Texture2D[] daggerfallPopupTextures;

        DaggerfallFont font1;
        DaggerfallFont font2;
        DaggerfallFont font3;
        DaggerfallFont font4;
        DaggerfallFont font5;

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

        void Awake()
        {
            dfUnity = DaggerfallUnity.Instance;
            audioSource = GetComponent<AudioSource>();
            audioSource.spatialBlend = 0;

            dfStartWindow = new DaggerfallStartWindow(uiManager);
            dfLoadGameWindow = new DaggerfallLoadSavedGameWindow(uiManager);
            dfBookReaderWindow = new DaggerfallBookReaderWindow(uiManager);
            dfVidPlayerWindow = new DaggerfallVidPlayerWindow(uiManager);
            dfRaceSelectWindow = new DaggerfallRaceSelectWindow(uiManager);
            uiManager.PostMessage(uiBootstrapMessage);

            SetupSingleton();
        }

        void Update()
        {
            // Process messages in queue
            if (uiManager.MessageCount > 0)
                ProcessMessageQueue();

            // Update top window
            if (uiManager.TopWindow != null)
            {
                uiManager.TopWindow.Update();
            }
        }

        void OnGUI()
        {
            // Draw top window
            if (uiManager.TopWindow != null)
            {
                uiManager.TopWindow.Draw();
            }
        }

        public static void PostMessage(string message)
        {
            DaggerfallUI dfui = GameObject.FindObjectOfType<DaggerfallUI>();
            if (dfui)
            {
                dfui.uiManager.PostMessage(message);
            }
        }

        public DaggerfallFont GetFont(int index)
        {
            switch (index)
            {
                case 1:
                    if (font1 == null) font1 = new DaggerfallFont(dfUnity.Arena2Path, DaggerfallFont.FontName.FONT0000);
                    return font1;
                case 2:
                    if (font2 == null) font2 = new DaggerfallFont(dfUnity.Arena2Path, DaggerfallFont.FontName.FONT0001);
                    return font2;
                case 3:
                    if (font3 == null) font3 = new DaggerfallFont(dfUnity.Arena2Path, DaggerfallFont.FontName.FONT0002);
                    return font3;
                case 4:
                default:
                    if (font4 == null) font4 = new DaggerfallFont(dfUnity.Arena2Path, DaggerfallFont.FontName.FONT0003);
                    return font4;
                case 5:
                    if (font5 == null) font5 = new DaggerfallFont(dfUnity.Arena2Path, DaggerfallFont.FontName.FONT0004);
                    return font5;
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
            panel.BackgroundTexture = GetDaggerfallPopupSlice(Slices.Fill);
            panel.BackgroundTextureLayout = TextureLayout.Tile;

            panel.SetBorderTextures(
                GetDaggerfallPopupSlice(Slices.TopLeft),
                GetDaggerfallPopupSlice(Slices.Top),
                GetDaggerfallPopupSlice(Slices.TopRight),
                GetDaggerfallPopupSlice(Slices.Left),
                GetDaggerfallPopupSlice(Slices.Right),
                GetDaggerfallPopupSlice(Slices.BottomLeft),
                GetDaggerfallPopupSlice(Slices.Bottom),
                GetDaggerfallPopupSlice(Slices.BottomRight));
        }

        #region Private Methods

        void ProcessMessageQueue()
        {
            // Process messages
            string message = uiManager.PeekMessage();
            switch (message)
            {
                case DaggerfallUIMessages.dfuiInitGame:
                    uiManager.PushWindow(dfStartWindow);
                    uiManager.PushWindow(dfVidPlayerWindow);
                    dfVidPlayerWindow.PlayOnStart = splashVideo;
                    break;
                case DaggerfallUIMessages.dfuiOpenVIDPlayerWindow:
                    uiManager.PushWindow(dfVidPlayerWindow);
                    break;
                case DaggerfallUIMessages.dfuiOpenBookReaderWindow:
                    uiManager.PushWindow(dfBookReaderWindow);
                    break;
                case DaggerfallUIMessages.dfuiOpenLoadSavedGameWindow:
                    uiManager.PushWindow(dfLoadGameWindow);
                    break;
                case DaggerfallUIMessages.dfuiStartNewGame:
                case DaggerfallUIMessages.dfuiOpenRaceSelectWindow:
                    uiManager.PushWindow(dfRaceSelectWindow);
                    break;
                case DaggerfallUIMessages.dfuiExitGame:
                    Application.Quit();
                    break;
                case WindowMessages.wmCloseWindow:
                    uiManager.PopWindow();
                    break;
                default:
                    return;
            }

            // Message was handled, pop from stack
            uiManager.PopMessage();
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
                    daggerfallPopupTextures[i] = TextureReader.CreateFromAPIImage(cif, i);
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