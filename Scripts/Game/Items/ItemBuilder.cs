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

using System;
using System.Collections.Generic;
using UnityEngine;
using DaggerfallConnect.FallExe;
using DaggerfallConnect.Save;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game.Items
{
    /// <summary>
    /// Generates new items for various game systems.
    /// This helper still under development.
    /// </summary>
    public static class ItemBuilder
    {
        public static DaggerfallUnityItem NewItem(ItemTemplate template)
        {
            return new DaggerfallUnityItem();
        }
    }
}