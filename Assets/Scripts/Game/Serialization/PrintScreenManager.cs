// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    jefetienne
//
// Notes:
//

using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DaggerfallConnect;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Game.Formulas;
using DaggerfallWorkshop.Game.MagicAndEffects;
using DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop.Game.Serialization
{

    public class PrintScreenManager : MonoBehaviour
    {

        #region Fields

        const string rootScreenshotsFolder = "Screenshots";
        const string fileExtension = ".jpg";

        private string unityScreenshotsPath;
        private KeyCode prtscrBinding = KeyCode.None;

        WaitForEndOfFrame endOfFrame = new WaitForEndOfFrame();

        #endregion

        #region Properties

        public string UnityScreenshotsPath
        {
            get { return GetUnityScreenshotsPath(); } 
        }

        #endregion

        #region Unity

        void Start ()
        {
            DaggerfallWorkshop.Game.InputManager.OnSavedKeyBinds += GetPrintScreenKeyBind;
            GetPrintScreenKeyBind();
        }

        void Update ()
        {
            if (DaggerfallUI.Instance.HotkeySequenceProcessed == HotkeySequence.HotkeySequenceProcessStatus.NotFound)
            {
                if (InputManager.Instance.GetKeyUp(prtscrBinding))
                    StartCoroutine(TakeScreenshot());
            }
        }

        #endregion

        #region Private Methods

        void GetPrintScreenKeyBind()
        {
            prtscrBinding = InputManager.Instance.GetBinding(InputManager.Actions.PrintScreen);
        }

        IEnumerator TakeScreenshot()
        {
            string name = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
            int inc = 1;

            if (File.Exists(Path.Combine(UnityScreenshotsPath, name + fileExtension)))
            {
                while (File.Exists(Path.Combine(UnityScreenshotsPath, name + "_" + inc + fileExtension)))
                    inc++;
                name += "_" + inc;
            }

            yield return endOfFrame;

            string path = Path.Combine(UnityScreenshotsPath, name + fileExtension);

            Texture2D pic = new Texture2D(Screen.width, Screen.height);
            pic.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            pic.Apply();
            byte[] bytes = pic.EncodeToJPG();

            // Save file
            System.IO.File.WriteAllBytes(path, bytes);

            // Prevent the HUD text below from appearing on the screenshot
            while (!File.Exists(path))
                yield return new WaitForSeconds(0.1f);

            DaggerfallUI.AddHUDText("Screenshot captured as '" + name + fileExtension + "'");
        }

        string GetUnityScreenshotsPath()
        {
            if (!string.IsNullOrEmpty(unityScreenshotsPath))
                return unityScreenshotsPath;

            string result = string.Empty;

            // Try settings
            result = DaggerfallUnity.Settings.MyDaggerfallUnityScreenshotsPath;
            if (string.IsNullOrEmpty(result) || !Directory.Exists(result))
            {
                // Default to dataPath
                result = Path.Combine(DaggerfallUnity.Settings.PersistentDataPath, rootScreenshotsFolder);
                if (!Directory.Exists(result))
                {
                    // Attempt to create path
                    Directory.CreateDirectory(result);
                }
            }

            // Test result is a valid path
            if (!Directory.Exists(result))
                throw new Exception("Could not locate valid path for Unity screenshot files. Check 'MyDaggerfallUnitysScreenshotsPath' in settings.ini.");

            // Log result and save path
            DaggerfallUnity.LogMessage(string.Format("Using path '{0}' for Unity screenshots.", result), true);
            unityScreenshotsPath = result;

            return result;
        }

        #endregion
    }
}
