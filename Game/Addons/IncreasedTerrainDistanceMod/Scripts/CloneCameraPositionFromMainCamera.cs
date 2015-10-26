//Increased Terrain Distance Mod for Daggerfall Tools For Unity
//http://www.reddit.com/r/dftfu
//http://www.dfworkshop.net/
//Author: Michael Rauter (a.k.a. Nystul)
//License: MIT License (http://www.opensource.org/licenses/mit-license.php)
//Version: 1.54

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
    public class CloneCameraPositionFromMainCamera : MonoBehaviour
    {
        void LateUpdate()
        {
            this.transform.position = Camera.main.transform.position;
        }

    }
}