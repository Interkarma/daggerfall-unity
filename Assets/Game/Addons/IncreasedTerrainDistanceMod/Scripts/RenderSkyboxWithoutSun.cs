//Increased Terrain Distance Mod for Daggerfall Tools For Unity
//http://www.reddit.com/r/dftfu
//http://www.dfworkshop.net/
//Author: Michael Rauter (a.k.a. Nystul)
//License: MIT License (http://www.opensource.org/licenses/mit-license.php)

using UnityEngine;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using DaggerfallConnect;
//using DaggerfallConnect.Arena2;
//using DaggerfallConnect.Utility;
//using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
//using DaggerfallWorkshop.Utility;

namespace ProjectIncreasedTerrainDistance
{
    public class RenderSkyboxWithoutSun : MonoBehaviour
    {
        float revertSunSize;

        void OnPreRender()
        {
            if (RenderSettings.skybox)
            {
                revertSunSize = RenderSettings.skybox.GetFloat("_SunSize");
                RenderSettings.skybox.SetFloat("_SunSize", 0.0f);
            }
        }

        void OnPostRender()
        {
            if (RenderSettings.skybox)
            {
                RenderSettings.skybox.SetFloat("_SunSize", revertSunSize);
            }
        }

    }
}