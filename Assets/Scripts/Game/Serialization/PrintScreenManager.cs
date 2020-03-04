// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2020 Daggerfall Workshop
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

        private string unityScreenshotsPath;
        private KeyCode prtscrBinding = KeyCode.None;

        #endregion

        #region Properties

        public string UnityScreenshotsPath
        {
            get { return GetUnityScreenshotsPath(); } 
        }

        #endregion

        #region Unity

        void Update ()
        {
            if (!DaggerfallUI.Instance.HotkeySequenceProcessed)
            {
                //Trying to set 'prtscrBinding' on Start() or Awake() will set it to 'None'
                //This 'if' statement avoids calling GetBinding() every Update() for when I GetKeyUp()
                if(prtscrBinding == KeyCode.None
                    && InputManager.Instance.GetBinding(InputManager.Actions.PrintScreen) != KeyCode.None)
                    prtscrBinding = InputManager.Instance.GetBinding(InputManager.Actions.PrintScreen);

                if (Input.GetKeyUp(prtscrBinding))
                    StartCoroutine(TakeScreenshot());
            }
        }

        #endregion

        #region Private Methods

        IEnumerator TakeScreenshot()
        {
            string name = DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss");
            int inc = 1;

            if (File.Exists(Path.Combine(UnityScreenshotsPath, name + ".png")))
            {
                while(File.Exists(Path.Combine(UnityScreenshotsPath, name + "_" + inc + ".png")))
                    inc++;
                name += "_" + inc;
            }

            string path = Path.Combine(UnityScreenshotsPath, name + ".png");

            //this is an async function
            ScreenCapture.CaptureScreenshot(path);

            //prevent the HUD text below from appearing on the screenshot
            while (!File.Exists(path))
                yield return new WaitForSeconds(0.1f);

            DaggerfallUI.AddHUDText("Screenshot captured as '" + name + ".png'");
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
                result = Path.Combine(Application.persistentDataPath, rootScreenshotsFolder);
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
