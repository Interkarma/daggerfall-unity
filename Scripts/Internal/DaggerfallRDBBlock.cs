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
using System.IO;
using DaggerfallConnect;
using DaggerfallConnect.Utility;
using DaggerfallConnect.Arena2;

namespace DaggerfallWorkshop
{
    /// <summary>
    /// Attached to dungeon blocks.
    /// </summary>
    public class DaggerfallRDBBlock : MonoBehaviour
    {
        public Vector2 RDBPosition = new Vector2();
        public Bounds RDBBounds = new Bounds();

        GameObject[] startMarkers = null;

        public GameObject[] StartMarkers
        {
            get { return startMarkers; }
        }

        public void SetStartMarkers(GameObject[] startMarkers)
        {
            this.startMarkers = startMarkers;
        }
    }
}