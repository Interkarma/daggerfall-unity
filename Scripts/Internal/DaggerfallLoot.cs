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
using System.Collections.Generic;
using DaggerfallWorkshop.Game.Items;

namespace DaggerfallWorkshop
{
    /// <summary>
    /// Enables a world object to be lootable by player.
    /// </summary>
    public class DaggerfallLoot : MonoBehaviour
    {
        long loadID = 0;
        LootContainerTypes containerType = LootContainerTypes.Nothing;
        public string lootTableKey = string.Empty;
        List<DaggerfallUnityItem> items = new List<DaggerfallUnityItem>();

        public long LoadID
        {
            get { return loadID; }
            set { loadID = value; }
        }

        public LootContainerTypes ContainerType
        {
            get { return containerType; }
            set { containerType = value; }
        }

        public string LootTableKey
        {
            get { return lootTableKey; }
            set { lootTableKey = value; }
        }

        public List<DaggerfallUnityItem> Items
        {
            get { return items; }
        }
    }
}