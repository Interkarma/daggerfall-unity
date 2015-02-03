// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

using UnityEngine;
using System.Collections;
using System.IO;
using DaggerfallConnect;
using DaggerfallConnect.Utility;
using DaggerfallConnect.Arena2;

namespace DaggerfallWorkshop
{
    /// <summary>
    /// Attached to a dungeon block.
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