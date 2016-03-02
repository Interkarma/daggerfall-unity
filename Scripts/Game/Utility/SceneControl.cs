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
using System.Collections;

namespace DaggerfallWorkshop.Game
{
    public class SceneControl : MonoBehaviour
    {
        public const int StartupSceneIndex = 0;
        public const int GameSceneIndex = 1;

        void Start()
        {
            // Check arena2 path is validated OK, otherwise start game setup
            if (!DaggerfallUnity.Instance.IsPathValidated)
            {
                DaggerfallUI.PostMessage(DaggerfallUIMessages.dfuiSetupGameWizard);
                return;
            }
            else
            {
                Application.LoadLevel(GameSceneIndex);
            }
        }
    }
}