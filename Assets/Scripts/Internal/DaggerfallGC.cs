// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Pango (petchema@concept-micro.com)
// Contributors:    
// 
// Notes:
//

using System;
using UnityEngine;

namespace DaggerfallWorkshop
{
    public static class DaggerfallGC
    {
        // Min time between two unused assets collections
        private const float uuaThrottleDelay = 180f;

        private static float uuaTimer = Time.realtimeSinceStartup;

        public static void ThrottledUnloadUnusedAssets()
        {
            if (Time.realtimeSinceStartup >= uuaTimer)
                ForcedUnloadUnusedAssets();
        }

        public static void ForcedUnloadUnusedAssets()
        {
            Resources.UnloadUnusedAssets();
            uuaTimer = Time.realtimeSinceStartup + uuaThrottleDelay;
        }
    }
}
