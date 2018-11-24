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
    public class AdjacentSurface
    {
        public readonly float turnHitDistance;
        public readonly int adjacentTurns;
        /// <summary>
        /// a ray that originates from the adjacent wall we're moving towards and points toward the corner intersecting with our current plane.
        /// </summary>
        public readonly Ray adjacentSurfaceRay;
        /// <summary>
        /// The force vector needed to maintain collision with the surface.
        /// </summary>
        public readonly Vector3 adjacentGrabDirection;
        public AdjacentSurface(Ray adjSurfRay, Vector3 surfaceHitNormal, int turnCount, float hitDistance)
        {
            adjacentSurfaceRay = adjSurfRay;
            adjacentGrabDirection = surfaceHitNormal;
            adjacentTurns = turnCount;
            turnHitDistance = hitDistance;
        }
    }

    /// <summary>
    /// Detects information about where the player is going to step
    /// </summary>
    public class PlayerMoveScanner : MonoBehaviour
    {
        public enum RotationDirection
        {
            XZClockwise,
            XZCounterClockwise,
            YZClockwise,
            YZCounterClockwise
        }
        CharacterController controller;
        AcrobatMotor acrobatMotor;
        PlayerMotor playerMotor;
        private int turnCount = 0;
        
        public float HeadHitRadius { get; private set; }
        public float StepHitDistance { get; private set; }
        public float HeadHitDistance { get; private set; }
        void Start()
        {
            controller = GetComponent<CharacterController>();
            acrobatMotor = GetComponent<AcrobatMotor>();
            playerMotor = GetComponent<PlayerMotor>();
            HeadHitRadius = controller.radius * 0.85f;
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
                StepHitDistance = hit.distance;
            else
                StepHitDistance = 0f;

        }
        public bool FindHeadHit(Ray ray)
        {
            //Ray ray = new Ray(controller.transform.position, Vector3.up);
            RaycastHit hit = new RaycastHit();
            if (Physics.SphereCast(ray, HeadHitRadius, out hit, 2f))
            {
                if (hit.collider.GetComponent<MeshCollider>())
                {
                    HeadHitDistance = hit.distance;
                    //Debug.Log(HeadHitDistance + "\n");
                    return true;
                }
            }
            return false;
        }
        public AdjacentSurface GetAdjacentSurface(Vector3 origin, Vector3 direction, RotationDirection turnDirection)
        {
            RaycastHit hit;
            
            float distance = direction.magnitude;

            // use recursion to raycast vectors to find the adjacent wall
            Debug.DrawRay(origin, direction, Color.green, Time.deltaTime);
            if (Physics.Raycast(origin, direction, out hit, distance)
                && (hit.collider.gameObject.GetComponent<MeshCollider>() != null))
            {
                // normal vector of raycasthit
                Debug.DrawRay(hit.point, hit.normal);
                Ray adjSurfaceRay;

                switch(turnDirection)
                {
                    case RotationDirection.XZClockwise:
                        adjSurfaceRay = new Ray(hit.point, Vector3.Cross(hit.normal, Vector3.up));
                        break;
                    case RotationDirection.XZCounterClockwise:
                        adjSurfaceRay = new Ray(hit.point, Vector3.Cross(Vector3.up, hit.normal));
                        break;
                    case RotationDirection.YZClockwise:
                        adjSurfaceRay = new Ray(hit.point, Vector3.Cross(-hit.normal, transform.right));
                        break;
                    case RotationDirection.YZCounterClockwise:
                        adjSurfaceRay = new Ray(hit.point, Vector3.Cross(transform.right, -hit.normal));
                        break;
                    default:
                        adjSurfaceRay = new Ray();
                        break;
                }

                Debug.DrawRay(adjSurfaceRay.origin, adjSurfaceRay.direction, Color.cyan);

                int turns = turnCount;
                turnCount = 0;
                return new AdjacentSurface(adjSurfaceRay, -hit.normal, turns, hit.distance);
            }
            else
            {
                turnCount++;
                if (turnCount < 4)
                {
                    // find next vector info now
                    Vector3 lastOrigin = origin;
                    origin = origin + direction;
                    Vector3 lastDirection = lastOrigin - origin;
                    Vector3 nextDirection = Vector3.zero;
                    switch(turnDirection)
                    {
                        case RotationDirection.XZClockwise:
                            nextDirection = Vector3.Cross(lastDirection, Vector3.up).normalized * distance;
                            break;
                        case RotationDirection.XZCounterClockwise:
                            nextDirection = Vector3.Cross(Vector3.up, lastDirection).normalized * distance;
                            break;
                        case RotationDirection.YZClockwise:
                            nextDirection = Vector3.Cross(lastDirection, transform.right).normalized * distance;

                            if (turnCount == 3)
                                // special Case: at top tip of a C-shaped scan looking for wall to climb onto,
                                // Need to check if there is a slanted-eaved roof to climb onto the edge of.
                                // upward rappel movement places the player on side of eave and triggers climbing
                                // need to scan diagonally forward down
                                nextDirection = Vector3.Reflect((lastDirection + nextDirection).normalized, lastDirection);

                            break;
                        case RotationDirection.YZCounterClockwise:
                            nextDirection = Vector3.Cross(transform.right, lastDirection).normalized * distance;
                            break;
                        default:
                            nextDirection = Vector3.zero;
                            break;
                    }
                    return GetAdjacentSurface(origin, nextDirection, turnDirection);
                }
                turnCount = 0;
                return null;
            }
        }
    }
}