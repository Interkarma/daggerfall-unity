// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
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
    /// This is back to a placeholder component for now.
    /// Building lookup has been moved to DaggerfallLocation object by way of BuildingDirectory.
    /// </summary>
    [RequireComponent(typeof(DaggerfallStaticDoors))]
    public class DaggerfallRMBBlock : MonoBehaviour
    {
        //public string Name;
        //public int BuildingCount;
        //public int LayoutX = -1;
        //public int LayoutY = -1;
        //public BuildingSummary[] Buildings;

        ///// <summary>
        ///// Sets block building information during scene layout.
        ///// </summary>
        ///// <param name="blockData">DFBlock data.</param>
        //public void SetBlockBuildingData(DFBlock blockData)
        //{
        //    // Create block summary
        //    Name = blockData.Name;
        //    BuildingCount = blockData.RmbBlock.SubRecords.Length;
        //    Buildings = RMBLayout.GetBuildingData(blockData);
        //}
    }
}