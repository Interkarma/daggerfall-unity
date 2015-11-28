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
using System;
using System.Collections;
using System.Collections.Generic;

namespace DaggerfallWorkshop.Game.Items
{
    /// <summary>
    /// Parent class for any individual item.
    /// </summary>
    public class DaggerfallItem
    {
        #region Fields

        string name = string.Empty;

        // Interim support for native magical item data
        int nativeMagicType;
        int nativeItemGroup;
        int nativeItemSubGroup;
        short[] nativeEnchantments;
        short nativeUsageCount;
        short nativeItemCost;
        byte nativeMaterialType;

        // Interim support for native item import
        short nativeCategory;
        short nativeIcon;
        int nativeValue;
        short nativeHits;
        byte nativeConstruction;
        byte nativeColor;
        int nativeWeight;
        short nativeEnchantmentPoints;
        short nativeMessage;
        short nativeMagic;

        #endregion

        #region Properties

        public string Name { get { return name; } set { name = value; } }

        #endregion

        #region Enums
        #endregion
    }
}