// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Pango (petchema@concept-micro.com)
// Contributors:    Numidium
// 
// Notes:
//

using UnityEngine;

namespace DaggerfallWorkshop
{
    public static class DaggerfallGC
    {
        public static void ForcedUnloadUnusedAssets()
        {
            Resources.UnloadUnusedAssets();
        }
    }
}
