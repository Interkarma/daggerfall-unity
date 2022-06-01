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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DaggerfallWorkshop.Game;

namespace DaggerfallWorkshop
{
    /// <summary>
    /// Moves player up an down ladders in building interiors.
    /// </summary>
    public class DaggerfallLadder : MonoBehaviour
    {
        /// <summary>
        /// Transition player up or down a ladder.
        /// </summary>
        public void ClimbLadder()
        {
            // Player must be inside a building
            PlayerMotor playerMotor = GameManager.Instance.PlayerMotor;
            PlayerEnterExit playerEnterExit = GameManager.Instance.PlayerEnterExit;
            if (!playerEnterExit.IsPlayerInsideBuilding)
                return;

            // Get bottom marker
            Vector3 bottomMarker;
            bool foundBottom = playerEnterExit.Interior.FindClosestMarker(out bottomMarker, DaggerfallInterior.InteriorMarkerTypes.LadderBottom, playerMotor.transform.position);

            // Get top marker
            Vector3 topMarker;
            bool foundTop = playerEnterExit.Interior.FindClosestMarker(out topMarker, DaggerfallInterior.InteriorMarkerTypes.LadderTop, playerMotor.transform.position);

            // Teleport to top marker
            if (foundTop && playerMotor.transform.position.y < topMarker.y)
            {
                playerMotor.transform.position = topMarker;
                playerMotor.FixStanding();
            }
            else if (foundBottom && playerMotor.transform.position.y > bottomMarker.y)
            {
                playerMotor.transform.position = bottomMarker;
                playerMotor.FixStanding();
            }
        }
    }
}