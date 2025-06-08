// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2023 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace DaggerfallWorkshop.Game.Utility
{
    /// <summary>
    /// Launches game or startup scene based on path validation.
    /// </summary>
    public class SceneControl : MonoBehaviour
    {
        public const int StartupSceneIndex = 0;
        public const int GameSceneIndex = 1;
        public GameObject defaultSky = null;

        void Start()
        {
            // Resolution
            if (DaggerfallUnity.Settings.ExclusiveFullscreen && DaggerfallUnity.Settings.Fullscreen)
            {
                Screen.SetResolution(
                    DaggerfallUnity.Settings.ResolutionWidth,
                    DaggerfallUnity.Settings.ResolutionHeight,
                    FullScreenMode.ExclusiveFullScreen);
            }
            else
            {
                Screen.SetResolution(
                    DaggerfallUnity.Settings.ResolutionWidth,
                    DaggerfallUnity.Settings.ResolutionHeight,
                    DaggerfallUnity.Settings.Fullscreen);
            }

            // Check arena2 path is validated OK, otherwise start game setup
            if (!DaggerfallUnity.Instance.IsPathValidated || DaggerfallUnity.Settings.ShowOptionsAtStart || Input.anyKey)
            {
                // Enable sky for test models
                if (defaultSky != null)
                    defaultSky.SetActive(true);

                // Post message to launch game setup
                DaggerfallUI.PostMessage(DaggerfallUIMessages.dfuiSetupGameWizard);
            }
            else
            {
                SceneManager.LoadScene(GameSceneIndex);
            }
        }

        //if scenes not in build menu, not guarenteed to be correct!
        public static int GetCurrentSceneIndex()
        {
            int index = SceneManager.GetActiveScene().buildIndex;
            DaggerfallUnity.LogMessage("scene index = " + index, false);
            return index;
        }

        public static bool StartupSceneLoaded()
        {
            return GetCurrentSceneIndex() == StartupSceneIndex;
        }

        public static bool GameSceneLoaded()
        {
            return GetCurrentSceneIndex() == GameSceneIndex;
        }


    }
}
