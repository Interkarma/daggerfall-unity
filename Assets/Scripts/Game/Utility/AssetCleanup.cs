// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Numidium
// Contributors:    
// 
// Notes:
//
using UnityEditor;

namespace DaggerfallWorkshop.Game.Utility
{
    public static class AssetCleanup
    {
        /// <summary>
        /// Destroys a unity object if it is a loose asset (i.e. created by a "new"). For use on textures, meshes, materials, and audio clips.
        /// </summary>
        /// <param name="obj"></param>
        public static void CleanAsset(UnityEngine.Object obj)
        {
            if (obj && !AssetDatabase.Contains(obj))
                UnityEngine.Object.Destroy(obj);
        }
    }
}
