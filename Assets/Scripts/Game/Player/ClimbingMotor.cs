using UnityEngine;
using System;
using System.Collections;
using DaggerfallConnect;

namespace DaggerfallWorkshop.Game
{
    [RequireComponent(typeof(PlayerMotor))]
    [RequireComponent(typeof(LevitateMotor))]
    [RequireComponent(typeof(CharacterController))]
    public class ClimbingMotor : MonoBehaviour
    {
        private Entity.PlayerEntity player;
        private PlayerGroundMotor groundMotor;
        private RappelMotor rappelMotor;
        private HangingMotor hangingMotor;
        private PlayerMotor playerMotor;
        private LevitateMotor levitateMotor;
        private CharacterController controller;
        private PlayerEnterExit playerEnterExit;
        private AcrobatMotor acrobatMotor;
        private PlayerSpeedChanger speedChanger;
        private PlayerMoveScanner moveScanner;
        private bool releasedFromCeiling = false;
        private bool overrideSkillCheck = false;
        private bool isClimbing = false;
        private bool isSlipping = false;
        private bool atOutsideCorner = false;
        private bool atInsideCorner = false;
        private float climbingStartTimer = 0;
        private float climbingContinueTimer = 0;
        private bool showClimbingModeMessage = true;
        #region Rays/Vectors
        private Vector2 lastHorizontalPosition = Vector2.zero;
        /// <summary>
        /// The current horizontal direction of the ledge of the wall the player is on 
        /// </summary>
        private Vector3 myLedgeDirection = Vector3.zero;
        /// <summary>
        /// the adjacent wall's horizontal ledge direction, opposite of its normal
        /// </summary>
        private Vector3 adjacentLedgeDirection = Vector3.zero;
        /// <summary>
        /// The ray that points toward the corner we're moving towards.  Only useful when strafing toward a nearby corner.
        /// </summary>
        private Ray myStrafeRay = new Ray();
        /// <summary>
        /// The ray that originates from the adjacent wall we're moving towards and points toward the corner.  Useless when not near a corner.
        /// </summary>
        private Ray adjacentWallRay = new Ray();
        /// <summary>
        /// The normal that sticks diagonally out of the wall corner that we're near. Equal-angled on both sides of the normal to the walls.
        /// </summary>
        private Ray cornerNormalRay = new Ray();
        /// <summary>
        /// the final movement direction that is taken
        /// </summary>
        private Vector3 moveDirection = Vector3.zero;
        #endregion
        private float startClimbHorizontalTolerance;
        private float startClimbSkillCheckFrequency;
        // how long it takes before we do another skill check to see if we can continue climbing
        private const int continueClimbingSkillCheckFrequency = 15; 
        // how long it takes before we try to regain hold if slipping
        private readonly float regainHoldSkillCheckFrequency = 5; 
        // minimum percent chance to regain hold per skill check if slipping, gets closer to 100 with higher skill
        private const int regainHoldMinChance = 20;
        // minimum percent chance to continue climbing per skill check, gets closer to 100 with higher skill
        private const int continueClimbMinChance = 70;
        private const int graspWallMinChance = 50;

        public bool IsClimbing
        {
            get { return isClimbing; }
            set { isClimbing = value; }
        }
        /// <summary>
        /// true if player is climbing but trying to regain hold of wall
        /// </summary>
        public bool IsSlipping
        {
            get { return isSlipping; }
        }
        /// <summary>
        /// True if player just jumped from a wall
        /// </summary>
        public bool WallEject { get; private set; }
        /// <summary>
        /// True if player just initiated climbing under wall to hang from ceiling beneath
        /// </summary>
        public bool ClimbQuitMoveUnderToHang { get; private set; }
        public Vector3 WallGrabDirection { get { return myLedgeDirection; } }
        void Start()
        {
            player = GameManager.Instance.PlayerEntity;
            playerMotor = GetComponent<PlayerMotor>();
            groundMotor = GetComponent<PlayerGroundMotor>();
            levitateMotor = GetComponent<LevitateMotor>();
            controller = GetComponent<CharacterController>();
            playerEnterExit = GetComponent<PlayerEnterExit>();
            acrobatMotor = GetComponent<AcrobatMotor>();
            speedChanger = GetComponent<PlayerSpeedChanger>();
            rappelMotor = GetComponent<RappelMotor>();
            hangingMotor = GetComponent<HangingMotor>();
            moveScanner = GetComponent<PlayerMoveScanner>();
        }

        /// <summary>
        /// Perform climbing check, and if successful, start climbing movement.
        /// </summary>
        public void ClimbingCheck()
        {
            bool advancedClimbingOn = DaggerfallUnity.Settings.AdvancedClimbing;

            // true if we should try climbing wall and are airborne
            bool airborneGraspWall = (!isClimbing && !isSlipping && acrobatMotor.Falling);
            bool inputBack = InputManager.Instance.HasAction(InputManager.Actions.MoveBackwards);
            bool inputForward = InputManager.Instance.HasAction(InputManager.Actions.MoveForwards);
            //inputForward = true;

            // boolean that means ground directly below us is too close for climbing or rappelling
            bool tooCloseToGroundForClimb = (((isClimbing && (inputBack || isSlipping)) || airborneGraspWall)
                // short circuit evaluate the raycast, also prevents bug where you could teleport across town
                && Physics.Raycast(controller.transform.position, Vector3.down, controller.height / 2 + 0.12f));

            CalcFrequencyAndToleranceOfWallChecks(airborneGraspWall);

            bool inputAbortCondition;
            if (advancedClimbingOn)
            {
                // TODO: prevent crouch from toggling crouch when aborting climb
                inputAbortCondition = (InputManager.Instance.HasAction(InputManager.Actions.Crouch)
                                      || InputManager.Instance.HasAction(InputManager.Actions.Jump));
            }
            else
                inputAbortCondition = !inputForward;

            // reset for next use
            WallEject = false;

            if (releasedFromCeiling)
            {
                MoveForwardIfNotClimbing();
            }
            if ((playerMotor.CollisionFlags & CollisionFlags.Sides) != 0)
            {
                releasedFromCeiling = false;
            }

            bool horizontallyStationary = Vector2.Distance(lastHorizontalPosition, new Vector2(controller.transform.position.x, controller.transform.position.z)) < startClimbHorizontalTolerance;
            bool touchingSides = (playerMotor.CollisionFlags & CollisionFlags.Sides) != 0;
            bool touchingGround = (playerMotor.CollisionFlags & CollisionFlags.Below) != 0;
            bool touchingAbove = (playerMotor.CollisionFlags & CollisionFlags.Above) != 0;
            bool slippedToGround = isSlipping && touchingGround;
            bool nonOrthogonalStart = !isClimbing && inputForward && !horizontallyStationary;
            bool forwardStationaryNearCeiling = inputForward && hangingMotor.IsWithinHangingDistance && horizontallyStationary;
            bool pushingFaceAgainstWallNearCeiling = hangingMotor.IsHanging && !isClimbing && touchingSides && forwardStationaryNearCeiling;
            bool climbingOrForwardOrGrasping = (isClimbing || inputForward || airborneGraspWall);
            RaycastHit hit;
            bool hangTouchNonVertical = hangingMotor.IsHanging && touchingSides && Physics.Raycast(controller.transform.position, controller.transform.forward, out hit, 0.40f) && Mathf.Abs(hit.normal.y) > 0.06f;

            ClimbQuitMoveUnderToHang = (inputBack && !moveScanner.HitSomethingInFront && moveScanner.FrontUnderCeiling != null);

            // Should we reset climbing starter timers?
            if ((!pushingFaceAgainstWallNearCeiling)
                &&
                (inputAbortCondition
                || ClimbQuitMoveUnderToHang
                || !climbingOrForwardOrGrasping 
                || !touchingSides && !releasedFromCeiling
                || levitateMotor.IsLevitating
                || playerMotor.IsRiding
                || slippedToGround
                // quit climbing if climbing down and ground is really close, prevents teleportation bug
                || tooCloseToGroundForClimb
                // don't do horizontal position check if already climbing
                || nonOrthogonalStart
                // if we're hanging, and touching sides with a wall that isn't mostly vertical
                || hangTouchNonVertical))
            {
                if (isClimbing && inputAbortCondition && advancedClimbingOn)
                    WallEject = true;

                isClimbing = false;
                showClimbingModeMessage = true;
                StopClimbing();
                releasedFromCeiling = false;
                // Reset position for horizontal distance check and timer to wait for climbing start
                lastHorizontalPosition = new Vector2(controller.transform.position.x, controller.transform.position.z);
            }
            else // countdown climbing events
            { 
                // countdown to climbing start
                if (climbingStartTimer <= (playerMotor.systemTimerUpdatesDivisor * startClimbSkillCheckFrequency))
                    climbingStartTimer += Time.deltaTime;
                // Begin Climbing
                else if (!isClimbing)
                {   
                    if (hangingMotor.IsHanging)
                    {   // grab wall from ceiling
                        overrideSkillCheck = true;
                        releasedFromCeiling = true;
                    }

                    // automatic success if not falling
                    if ((!airborneGraspWall && !hangingMotor.IsHanging) || releasedFromCeiling)
                        StartClimbing();
                    // skill check to see if we catch the wall 
                    else if (ClimbingSkillCheck(graspWallMinChance))
                        StartClimbing();
                    else
                        climbingStartTimer = 0;
                }

                if (isClimbing)
                {
                    // countdown to climb update, Faster updates if slipping
                    if (climbingContinueTimer <= (playerMotor.systemTimerUpdatesDivisor * (isSlipping ? regainHoldSkillCheckFrequency : continueClimbingSkillCheckFrequency)))
                        climbingContinueTimer += Time.deltaTime;
                    else
                    {
                        climbingContinueTimer = 0;

                        // don't allow slipping if not moving.
                        if (!InputManager.Instance.HasAction(InputManager.Actions.MoveForwards)
                                && !InputManager.Instance.HasAction(InputManager.Actions.MoveBackwards)
                                && !InputManager.Instance.HasAction(InputManager.Actions.MoveLeft)
                                && !InputManager.Instance.HasAction(InputManager.Actions.MoveRight))
                            isSlipping = false;
                        // it's harder to regain hold while slipping than it is to continue climbing with a good hold on wall
                        else if (isSlipping)
                            isSlipping = !ClimbingSkillCheck(regainHoldMinChance);
                        else
                            isSlipping = !ClimbingSkillCheck(continueClimbMinChance);
                    }
                }

            }

            // Climbing Cycle
            if (isClimbing)
            {
                // evalate the ledge direction
                GetClimbedWallInfo();

                ClimbMovement();

                // both variables represent similar situations, but different context
                acrobatMotor.Falling = isSlipping;
            }
            else if (!rappelMotor.IsRappelling)
            {
                isSlipping = false;
                atOutsideCorner = false;
                atInsideCorner = false;
                showClimbingModeMessage = true;
                moveDirection = Vector3.zero;
                // deletes locally and saves myLedgeDirection to variable in class
                if (myLedgeDirection != Vector3.zero)
                    moveScanner.CutAndPasteVectorToDetached(ref myLedgeDirection);
            }
        }

        public void StopClimbing(bool fromHanging = false)
        {
            climbingStartTimer = 0;
        }

        private void CalcFrequencyAndToleranceOfWallChecks(bool airborneGraspWall)
        {
            if (DaggerfallUnity.Settings.AdvancedClimbing)
            {
                if (rappelMotor.IsRappelling)
                {   // very lenient because we're trying to attach to wall with guarunteed success
                    startClimbHorizontalTolerance = 2f;
                    startClimbSkillCheckFrequency = 0;
                }
                else if (airborneGraspWall)
                {   // more lenient because we could not jump really orthogonal onto the wall
                    startClimbHorizontalTolerance = 0.90f;
                    startClimbSkillCheckFrequency = 5;
                }
                else if (releasedFromCeiling)
                {
                    startClimbHorizontalTolerance = 0.90f;
                    startClimbSkillCheckFrequency = 0;
                }
                else
                {   // least leniency because we want climbing to be very intentional here
                    startClimbHorizontalTolerance = 0.12f;
                    startClimbSkillCheckFrequency = 14;
                }
            }
            else
            {   // least leniency because we want climbing to be very intentional here
                startClimbHorizontalTolerance = 0.12f;
                startClimbSkillCheckFrequency = 14;
            }
        }

        /// <summary>
        /// Set climbing to true and show climbing mode message once
        /// </summary>
        private void StartClimbing()
        {
            if (!isClimbing)
            {
                if (showClimbingModeMessage)
                    DaggerfallUI.AddHUDText(UserInterfaceWindows.HardStrings.climbingMode);
                // Disable further showing of climbing mode message until current climb attempt is stopped
                // to keep it from filling message log
                showClimbingModeMessage = false;
                isClimbing = true;
                hangingMotor.CancelHanging();
                // reset jumping in case we jumped onto the wall
                acrobatMotor.Jumping = false;
            }
        }

        /// <summary>
        /// Physically check for wall info between player and wall he's attached to.  Searches in front of player if no wall already set.
        /// </summary>
        private void GetClimbedWallInfo()
        {
            RaycastHit hit;

            Vector3 p1 = controller.transform.position + controller.center + Vector3.up * -controller.height * 0.40f;
            Vector3 p2 = p1 + Vector3.up * controller.height;

            // decide what direction to look towards to get the ledge direction vector
            Vector3 wallDirection;

            if (moveScanner.AboveBehindWall != null)
                moveScanner.CutAndPasteAboveBehindWallTo(ref myLedgeDirection);
            else if (moveScanner.BelowBehindWall != null)
                moveScanner.CutAndPasteBelowBehindWallTo(ref myLedgeDirection);

            if (myLedgeDirection == Vector3.zero)
                wallDirection = controller.transform.forward;
            else if (!atOutsideCorner)
                wallDirection = myLedgeDirection;
            else
                wallDirection = -cornerNormalRay.direction;
            // Cast character controller shape forward to see if it is about to hit anything.
            Debug.DrawRay(controller.transform.position, wallDirection, Color.gray);
            if (Physics.CapsuleCast(p1, p2, controller.radius, wallDirection, out hit, 0.20f))
            {
                // Get the negative horizontal component of the hitnormal, so gabled roofs don't mess it up
                myLedgeDirection = Vector3.ProjectOnPlane(-hit.normal, Vector3.up).normalized;

                // set origin of strafe ray to y level of controller
                // direction is set to hitnormal until it can be adjusted when we have a side movement direction
                myStrafeRay = new Ray(new Vector3(hit.point.x, controller.transform.position.y, hit.point.z), hit.normal);
            }
            else 
            {
                if (myLedgeDirection == Vector3.zero && moveScanner.FrontWall != null)
                    moveScanner.CutAndPasteFrontWallTo(ref myLedgeDirection);
            }
        }

        private bool GetAdjacentWallInfo(Vector3 origin, Vector3 direction, PlayerMoveScanner.RotationDirection turnDirection)
        {
            moveScanner.FindAdjacentSurface(origin, direction, turnDirection);
            AdjacentSurface adjSurface = moveScanner.SideWall;

            if (adjSurface != null && adjSurface.adjacentTurns == 0)
                atInsideCorner = IsAtInsideCorner(adjSurface.turnHitDistance);

            if (adjSurface != null)
            {
                adjacentLedgeDirection = adjSurface.grabDirection;
                adjacentWallRay = adjSurface.adjacentSurfaceRay;
            }

            return (adjSurface != null);
        }
        private bool IsAtInsideCorner(float hitDistance)
        {
            // not sure if there's a better way to do this?
            float myAngle;
            myAngle = Vector3.Angle(cornerNormalRay.direction, -myStrafeRay.direction);
            if (hitDistance < controller.radius + 0.17f && hitDistance > controller.radius + 0.15f
                && myAngle < 68 && myAngle > 66.5f
                ||
                hitDistance < controller.radius + 0.07f && hitDistance > controller.radius + 0.045f
                && myAngle < 45.5f && myAngle > 44.5f
                )
            {
                //Debug.Log("Adjacent wall distance: " + hitDistance);
                //Debug.Log("Dist: " + hitDistance + "Angle = " + myAngle);

                return true;
            }    

            return false;
        }
        /// <summary>
        /// Perform Climbing Movement
        /// </summary>
        private void ClimbMovement()
        {
            // Try to move along wall and forwards at same time
            // This helps player maintain collision checks with the wall and step onto the ledge once it's found

            // if strafing to either side, this will be set so we can check for wrap-around corners.
            Vector3 checkDirection = Vector3.zero;

            if (!isSlipping)
            {
                float climbScalar = speedChanger.GetClimbingSpeed();
                moveDirection = Vector3.zero;
                bool movedForward = InputManager.Instance.HasAction(InputManager.Actions.MoveForwards);
                bool movedBackward = InputManager.Instance.HasAction(InputManager.Actions.MoveBackwards);
                bool movedLeft = InputManager.Instance.HasAction(InputManager.Actions.MoveLeft);
                bool movedRight = InputManager.Instance.HasAction(InputManager.Actions.MoveRight);

                if (DaggerfallUnity.Settings.AdvancedClimbing)
                {
                    bool hitSomethingInFront = moveScanner.HitSomethingInFront;
                    #region Vertical Climbing
                    // TODO: Disallow rappel when pressing back if auto climbing
                    if (!movedBackward && !atOutsideCorner && (movedForward || !hitSomethingInFront))
                    {
                        // TODO: devise horizontal distance-based solution to terminate forward movement
                        // It may fix the problem of climbing to the top of gabled roofs.
                        if (!overrideSkillCheck)
                            overrideSkillCheck = !hitSomethingInFront;
                        moveDirection.y = Vector3.up.y * climbScalar;
                    }
                    else if (movedBackward)
                        moveDirection.y = Vector3.down.y * climbScalar;
                    #endregion
                    #region Horizontal Climbing
                    if (movedRight || movedLeft)
                    {
                        float checkScalar = controller.radius + 0.5f;
                        if (movedRight)
                            checkDirection = Vector3.Cross(Vector3.up, myLedgeDirection).normalized;
                        else if (movedLeft)
                            checkDirection = Vector3.Cross(myLedgeDirection, Vector3.up).normalized;

                        // adjust direction so it can intersect with adjacentWallRay
                        myStrafeRay.direction = checkDirection;
                        Debug.DrawRay(myStrafeRay.origin, myStrafeRay.direction, Color.red);

                        // perform check for adjacent wall
                        GetAdjacentWallInfo(controller.transform.position, checkDirection * checkScalar, movedLeft ? PlayerMoveScanner.RotationDirection.XZClockwise : PlayerMoveScanner.RotationDirection.XZCounterClockwise);

                        Vector3 intersection;
                        Vector3 intersectionOrthogonal;
                        Vector3 wrapDirection = Vector3.zero; // direction to move while wrapping around corner
                        // did we find the wall corner intersection?
                        if (LineLineIntersection(out intersection, myStrafeRay.origin, myStrafeRay.direction, adjacentWallRay.origin, adjacentWallRay.direction))
                        {
                            intersectionOrthogonal = (-myLedgeDirection - adjacentLedgeDirection).normalized;
                            Debug.DrawRay(intersection, intersectionOrthogonal, Color.yellow);
                            atOutsideCorner = ((myStrafeRay.origin - intersection).magnitude < 0.01f);
                            if (atOutsideCorner)
                            {
                                // perform outside corner wrap
                                if (movedRight)
                                    wrapDirection = Vector3.Cross(intersectionOrthogonal, Vector3.up).normalized;
                                else if (movedLeft)
                                    wrapDirection = Vector3.Cross(Vector3.up, intersectionOrthogonal).normalized;
                            }

                            cornerNormalRay = new Ray(intersection, intersectionOrthogonal);   
                        }

                        // exiting outside wall corner?
                        if (atInsideCorner || ( atOutsideCorner && IsAlmostParallel(wrapDirection, adjacentWallRay.direction)))
                        {
                            myLedgeDirection = adjacentLedgeDirection;
                            wrapDirection = -adjacentWallRay.direction;
                            checkDirection = wrapDirection;
                            atOutsideCorner = false;
                            atInsideCorner = false;
                        }

                        // if at outside corner, use corner-updated directions to wrap around it
                        if (atOutsideCorner)
                        {
                            Debug.DrawRay(intersection, wrapDirection, Color.magenta);
                            moveDirection += wrapDirection * climbScalar;
                        }
                        else // move in wasd direction
                            moveDirection += checkDirection * climbScalar;
                    }
                    #endregion
                    // need to add horizontal movement towards wall for collision
                    moveDirection.x += myLedgeDirection.x * playerMotor.Speed;
                    moveDirection.z += myLedgeDirection.z * playerMotor.Speed;
                }
                else // do normal climbing
                {
                    moveDirection = myLedgeDirection * playerMotor.Speed;
                    moveDirection.y = Vector3.up.y * climbScalar;
                }  
            }
            else // do slipping down wall
            {
                acrobatMotor.CheckInitFall(ref moveDirection);
                acrobatMotor.ApplyGravity(ref moveDirection);
            }
            // finalize climbing movement
            controller.Move(moveDirection * Time.deltaTime);
            playerMotor.CollisionFlags = controller.collisionFlags;
        }
        private void MoveForwardIfNotClimbing()
        {
            if (!isClimbing)
            {
                overrideSkillCheck = true;
                controller.Move(controller.transform.forward * Time.deltaTime);
                playerMotor.CollisionFlags = controller.collisionFlags;
            }


        }
        #region Helpers
        /// <summary>
        ///  Calculate the intersection point of two lines. Returns true if lines intersect, otherwise false.
        /// </summary>
        /// <param name="intersection">The calculated intersection, if found</param>
        /// <param name="linePoint1">Origin 1</param>
        /// <param name="lineVec1">Direction 1</param>
        /// <param name="linePoint2">Origin 2</param>
        /// <param name="lineVec2">Direction 2</param>
        /// <returns>Returns true if lines intersect, otherwise false</returns>
        private bool LineLineIntersection(out Vector3 intersection, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
        {
            Vector3 lineVec3 = linePoint2 - linePoint1;
            Vector3 crossVec1and2 = Vector3.Cross(lineVec1, lineVec2);
            Vector3 crossVec3and2 = Vector3.Cross(lineVec3, lineVec2);

            float planarFactor = Vector3.Dot(lineVec3, crossVec1and2);

            //is coplanar, and not parallel
            if (Mathf.Abs(planarFactor) < 0.01f && crossVec1and2.sqrMagnitude > 0.01f)
            {
                float s = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
                intersection = linePoint1 + (lineVec1 * s);
                return true;
            }
            else
            {
                intersection = Vector3.zero;
                return false;
            }
        }

        /// <summary>
        /// Check if two vectors are almost parallel
        /// </summary>
        private bool IsAlmostParallel( Vector3 lineVec1, Vector3 lineVec2)
        {
            if (Vector3.Cross(lineVec1, lineVec2).sqrMagnitude < 0.01f)
                return true;
            return false;
        }
        // TODO: Migrate this into seperate class accessible to climbingMotor and HangingMotor
        /// <summary>
        /// See if the player can pass a climbing skill check
        /// </summary>
        /// <returns>true if player passed climbing skill check</returns>
        public bool ClimbingSkillCheck(int basePercentSuccess)
        {
            player.TallySkill(DFCareer.Skills.Climbing, 1);

            if (overrideSkillCheck)
                return true;

            int skill = player.Skills.GetLiveSkillValue(DFCareer.Skills.Climbing);
            int luck = player.Stats.GetLiveStatValue(DFCareer.Stats.Luck);
            if (player.Race == Entity.Races.Khajiit)
                skill += 30;

            // Climbing effect states "target can climb twice as well" - doubling effective skill after racial applied
            if (player.IsEnhancedClimbing)
                skill *= 2;

            // Clamp skill range
            skill = Mathf.Clamp(skill, 5, 95);
            float luckFactor = Mathf.Lerp(0, 10, luck * 0.01f);

            // Skill Check
            float percentRolled = Mathf.Lerp(basePercentSuccess, 100, skill * .01f) + luckFactor;

            if (percentRolled < UnityEngine.Random.Range(1, 101)) // Failed Check?
            {
                // Don't allow skill check to break climbing while swimming
                // Water makes it easier to climb
                var playerPos = controller.transform.position.y + (76 * MeshReader.GlobalScale) - 0.95f;
                var playerFootPos = playerPos - (controller.height / 2) - 1.20f; // to prevent player from failing to climb out of water
                var waterPos = playerEnterExit.blockWaterLevel * -1 * MeshReader.GlobalScale;
                if (playerFootPos >= waterPos) // prevent fail underwater
                    return false;
            }
            return true;
        }
        #endregion
    }
}
