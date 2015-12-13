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

namespace DaggerfallWorkshop.Game.Entity
{
    /// <summary>
    /// Collection of items held by an Entity.
    /// This may be an inventory, loot, equipped items, harvestable items, etc.
    /// This class is under active development and may change several times before completed.
    /// </summary>
    [Serializable]
    public class EntityItems
    {
        #region Fields
        #endregion

        #region Structures
        #endregion

        #region Enums
        #endregion

        #region Public Methods

        /// <summary>
        /// Copies items from another collection.
        /// </summary>
        /// <param name="other">Source of items to copy from.</param>
        public void Copy(EntityItems other)
        {
        }

        /// <summary>
        /// Transfers items from another collection.
        /// Items will be removed from other collection and placed in this one.
        /// </summary>
        /// <param name="other">Source of items to transfer from.</param>
        public void Transfer(EntityItems other)
        {
        }

        #endregion

        #region Private Methods
        #endregion
    }
}