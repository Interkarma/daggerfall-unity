// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Meteoric Dragon
// Contributors:    
// 
// Notes: This class detects information about where the player is going to step
//

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// Detects information about where the player is going to step
    /// </summary>
    public class PlayerStepDetector : MonoBehaviour
    {
        CharacterController controller;
        AcrobatMotor acrobatMotor;

        // could declare other useful properties if more info is needed about the surface hit
        public float HitDistance { get; private set; }

        void Start()
        {
            controller = GetComponent<CharacterController>();
            acrobatMotor = GetComponent<AcrobatMotor>();
        }

        /// <summary>
        /// Detects information about where the player is going to step via SphereCast downwards, Dispaced toward moveDirection
        /// </summary>
        /// <param name="moveDirection"></param>
        public void FindStep(Vector3 moveDirection)
        {
            if (GameManager.IsGamePaused)
                return;
            Vector3 position = controller.transform.position;
            float minRange = (controller.height / 2f);
            float maxRange = minRange + 2.10f;
            // get the normalized horizontal component of player's direction
            Vector3 checkRayOriginDisplacement = Vector3.ProjectOnPlane(moveDirection, Vector3.up).normalized * 0.10f;
            Ray checkStepRay = new Ray(position + checkRayOriginDisplacement, Vector3.down * maxRange);

            RaycastHit hit;

            if (!acrobatMotor.Jumping && Physics.SphereCast(checkStepRay, 0.1f, out hit, maxRange))
                HitDistance = hit.distance;
            else
                HitDistance = 0f;

        }
    }
}