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
    public class DaggerfallCityGate : MonoBehaviour
    {
        bool isOpen;

        public void SetOpen(bool open)
        {
            // Do nothing if no change
            if (open == isOpen)
                return;

            // TODO: Change model

            // Save new state
            isOpen = open;
        }
    }
}