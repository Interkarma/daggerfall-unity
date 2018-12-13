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
    public class VectorMeasurement
    {
        private Vector3 InitialPosition;
        /// <summary>
        /// Initialize with a vector3, and call distance to measure horizontal distance.
        /// </summary>
        public VectorMeasurement(Vector3 pos)
        {
            InitialPosition = pos;
        }
        public float Distance(Vector3 newPos)
        {
            return Vector2.Distance(new Vector2(InitialPosition.x, InitialPosition.z), new Vector2(newPos.x, newPos.z));
        }
    }

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
        public readonly Vector3 grabDirection;
        public AdjacentSurface(Ray adjSurfRay, Vector3 surfaceHitNormal, int turnCount, float hitDistance)
        {
            adjacentSurfaceRay = adjSurfRay;
            grabDirection = surfaceHitNormal;
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
        HangingMotor hangingMotor;
        ClimbingMotor climbingMotor;
        AcrobatMotor acrobatMotor;
        PlayerMotor playerMotor;
        private int turnCount = 0;

        public float HeadHitRadius { get; private set; }
        public float StepHitDistance { get; private set; }
        public float HeadHitDistance { get; private set; }
        public bool HitSomethingInFront { get; private set; }
        /// <summary>
        /// Saved WallGrab vector after detaching from a wall.  
        /// </summary>
        public Vector3 WallDetachedVector { get; private set; }
        /// <summary>
        /// The wall above and behind the hanging player
        /// </summary>
        public AdjacentSurface AboveBehindWall { get; private set; }
        /// <summary>
        /// The wall directly in front of the player if he's hanging
        /// </summary>
        public AdjacentSurface FrontWall { get; private set; }
        /// <summary>
        /// The wall on a side wrapped around an inside corner or outside corner while climbing.
        /// </summary>
        public AdjacentSurface SideWall { get; private set; }
        /// <summary>
        /// The wall behind a player that he can rappel down to while walking backwards
        /// </summary>
        public AdjacentSurface BelowBehindWall { get; private set; }
        /// <summary>
        /// The ceiling which is underneath the player in front of him while climbing
        /// </summary>
        public AdjacentSurface FrontUnderCeiling { get; private set; }
        void Start()
        {
            controller = GetComponent<CharacterController>();
            acrobatMotor = GetComponent<AcrobatMotor>();
            playerMotor = GetComponent<PlayerMotor>();
            hangingMotor = GetComponent<HangingMotor>();
            climbingMotor = GetComponent<ClimbingMotor>();
            HeadHitRadius = controller.radius * 0.85f;
        }

        /// <summary>
        /// Delete Scanner's AboveBehindWall after copying the direction of the wall to it
        /// </summary>
        /// <param name="WallDirection">The vector to be copied to</param>
        public void CutAndPasteAboveBehindWallTo(ref Vector3 WallDirection)
        {
            WallDirection = AboveBehindWall.grabDirection;
            AboveBehindWall = null;
        }
        public void CutAndPasteFrontWallTo(ref Vector3 WallDirection)
        {
            WallDirection = FrontWall.grabDirection;
            FrontWall = null;
        }
        public void CutAndPasteBelowBehindWallTo(ref Vector3 WallDirection)
        {
            WallDirection = BelowBehindWall.grabDirection;
            BelowBehindWall = null;
        }
        public void CutAndPasteFrontUnderCeilingTo(ref Vector3 CeilingDirection)
        {
            CeilingDirection = FrontUnderCeiling.grabDirection;
            FrontUnderCeiling = null;
        }
        public void CutAndPasteVectorToDetached(ref Vector3 wallGrab)
        {
            WallDetachedVector = wallGrab;
            wallGrab = Vector3.zero;
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
        
        private void AssignAdjacentSurface(AdjacentSurface surf, RotationDirection turnDirection)
        {
            /// using info about the surface found, and the turn direction used to find it, determine
            /// the type of surface to surface transition and assign to appropriate surface variable
            /// if surf is null, we didn't find an adjacent surface.
            /// if the number of turns to find the surface is the wrong number, we didn't find the desired surface
            int turns = 0;
            if (surf != null)
                turns = surf.adjacentTurns;

            if (turnDirection == RotationDirection.XZClockwise || turnDirection == RotationDirection.XZCounterClockwise)
                SideWall = surf;
            else if (turnDirection == RotationDirection.YZClockwise && turns >= 2)
                AboveBehindWall = surf;
            else if (turnDirection == RotationDirection.YZCounterClockwise)
            {
                if (hangingMotor.IsHanging && turns == 1)
                    FrontWall = surf;
                else if (turns == 2 && surf.grabDirection.y < 0.2f)
                    BelowBehindWall = surf;
                else if (turns == 3 && surf.grabDirection.y > 0.8f)
                    FrontUnderCeiling = surf;
            } 
        }
        /// <summary>
        /// Finds the adjacent surface to the one that the player is on.
        /// </summary>
        /// <param name="origin">The place from which the scanning ray is cast</param>
        /// <param name="direction"></param>
        /// <param name="turnDirection"></param>
        public void FindAdjacentSurface(Vector3 origin, Vector3 direction, RotationDirection turnDirection)
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
                AssignAdjacentSurface(new AdjacentSurface(adjSurfaceRay, -hit.normal, turns, hit.distance), turnDirection);
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
                                // upward rappel movement puts the player on side of eave and triggers climbing
                                // need to scan diagonally forward down
                                nextDirection = Vector3.Reflect((lastDirection + nextDirection).normalized, lastDirection);

                            break;
                        case RotationDirection.YZCounterClockwise:
                            nextDirection = Vector3.Cross(transform.right, lastDirection).normalized * distance;
                            // TODO: write code to make a diagonal upwards forward check for turncount 3 to find
                            // a ceiling under the wall the player is climbing on.  Could possibly be another wall
                            // to rappel onto?

                            if (turnCount == 3)
                                nextDirection = Vector3.Reflect((lastDirection + nextDirection).normalized, lastDirection);
                            break;
                        default:
                            nextDirection = Vector3.zero;
                            break;
                    }
                    FindAdjacentSurface(origin, nextDirection, turnDirection);
                }
                turnCount = 0;
                AssignAdjacentSurface(null, turnDirection);
            }
        }

        public void SetHitSomethingInFront()
        {
            RaycastHit hit;
            Vector3 inFrontDirection;
                
            if (climbingMotor.IsClimbing)
                inFrontDirection = climbingMotor.WallGrabDirection;
            else
                inFrontDirection = controller.transform.forward;
               
            HitSomethingInFront = (Physics.Raycast(controller.transform.position, inFrontDirection, out hit, 0.3f));
        }
    }
}