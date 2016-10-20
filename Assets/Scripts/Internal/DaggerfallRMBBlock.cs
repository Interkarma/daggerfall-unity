// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2016 Daggerfall Workshop
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
using DaggerfallConnect;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop
{
    /// <summary>
    /// Stores information related to exterior city blocks (RMB blocks).
    /// Should be attached to city block game objects.
    /// Currently storing building information for testing.
    /// May be expanded later to track additional block-specific information.
    /// </summary>
    [RequireComponent(typeof(DaggerfallStaticDoors))]
    public class DaggerfallRMBBlock : MonoBehaviour
    {
        public string Name;
        public int BuildingCount;
        public RMBLayout.BuildingSummary[] Buildings;

        /// <summary>
        /// Sets block information during scene layout.
        /// </summary>
        /// <param name="blockData">DFBlock data.</param>
        public void SetBlockData(DFBlock blockData)
        {
            // Create block summary
            Name = blockData.Name;
            BuildingCount = blockData.RmbBlock.SubRecords.Length;
            Buildings = RMBLayout.GetBuildingData(blockData);
        }
    }
}