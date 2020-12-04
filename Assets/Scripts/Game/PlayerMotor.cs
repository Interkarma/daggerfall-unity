using UnityEngine;
using System;
using System.Collections;
using DaggerfallConnect;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Game.Serialization;

namespace DaggerfallWorkshop.Game
{
    //
    // Using FPSWalkerEnhanced from below community wiki entry.
    // http://wiki.unity3d.com/index.php?title=FPSWalkerEnhanced
    //
    // Extended for moving platforms, and ceiling hits, and other tweaks.
    //
    [RequireComponent(typeof(PlayerSpeedChanger))]
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMotor : MonoBehaviour
    {
        #region Fields

        const float walkingRayDistance = 1.0f;
        const float ridingRayDistance = 2.0f;

        bool isCrouching = false;

        bool isRiding = false;

        // If true, diagonal speed (when strafing + moving forward or back) can't exceed normal move speed; otherwise it's about 1.4 times faster
        public bool limitDiagonalSpeed = true;

        public float systemTimerUpdatesDivisor = .0549254f; // Divisor for updates by the system timer at memory location 0x46C.
                                                            // Used for timing various things in classic.

        // FixedUpdate is too choppy to give smooth camera movement. This handles a smooth following child transform.
        public Transform smoothFollower;                // The Transform that follows; will lerp to this Transform's position.
        public float smoothFollowerLerpSpeed = 25.0f;   // Multiplied by dt.
        Vector3 smoothFollowerPrevWorldPos;
        bool smoothFollowerReset = true;

        [HideInInspector, NonSerialized]
        public CharacterController controller;

        private Vector3 moveDirection = Vector3.zero;
        private bool grounded = false;
        private float speed;

        private ClimbingMotor climbingMotor;
        private RappelMotor rappelMotor;
        //private HangingMotor hangingMotor;
        private PlayerHeightChanger heightChanger;
        private PlayerSpeedChanger speedChanger;
        private FrictionMotor frictionMotor;
        private AcrobatMotor acrobatMotor;
        private PlayerEnterExit playerEnterExit;
        private PlayerGroundMotor groundMotor;
        private PlayerMoveScanner playerScanner;

        private CollisionFlags collisionFlags = 0;

        private bool cancelMovement = false;

        LevitateMotor levitateMotor;
        float freezeMotor = 0;
        OnExteriorWaterMethod onExteriorWaterMethod = OnExteriorWaterMethod.None;
        bool onExteriorPathMethod = false;
        bool onExteriorStaticGeometryMethod = false;

        float groundedTime;

        #endregion

        #region Enums

        /// <summary>
        /// Defines the way player can interact with exterior water tiles.
        /// Unrelated to deep water swimming such as in dungeons.
        /// </summary>
        public enum OnExteriorWaterMethod
        {
            /// <summary>Player not touching exterior water at all.</summary>
            None,
            /// <summary>Player is swimming in exterior water.</summary>
            Swimming,
            /// <summary>Player is walking on exterior water.</summary>
            WaterWalking,
        }

        #endregion

        #region Properties

        public bool IsGrounded
        {
            get { return grounded; }
        }

        public float GroundedTime
        {
            get { return groundedTime; }
        }

        public float Speed
        {
            get { return speed; }
        }

        public bool IsRunning
        {
            get { return speedChanger.isRunning; }
        }

        public bool IsStandingStill
        {
            get
            {
                if (grounded)
                {
                    // Set standing still while grounded flag
                    // Casting moveDirection to a Vector2 so constant downward force of gravity not included in magnitude
                    return (new Vector2(moveDirection.x, moveDirection.z).magnitude == 0);
                }
                return false;
            }
        }

        public bool IsJumping
        {
            get { return acrobatMotor.Jumping; }
        }

        public bool IsCrouching
        {
            get { return isCrouching; }
            set { isCrouching = value; }
        }

        public bool IsRiding
        {
            get { return isRiding; }
            set { isRiding = value; }
        }

        public bool IsClimbing
        {
            get { return (climbingMotor) ? climbingMotor.IsClimbing : false; }
        }

        public bool IsSwimming
        {
            get { return (levitateMotor) ? levitateMotor.IsSwimming : false; }
        }

        public bool IsLevitating
        {
            get { return (levitateMotor) ? levitateMotor.IsLevitating : false; }
        }

        public CollisionFlags CollisionFlags
        {
            get { return collisionFlags; }
            set {
                //if (value != collisionFlags)
                //    Debug.Log(collisionFlags + " -> " + value + "\n");
                collisionFlags = value; }
        }

        public bool IsMovingLessThanHalfSpeed
        {
            get
            {
                if (IsStandingStill)
                {
                    return true;
                }
                if (isCrouching)
                {
                    return (speedChanger.GetWalkSpeed(GameManager.Instance.PlayerEntity) / 2) >= speed;
                }
                return (speedChanger.GetBaseSpeed() / 2) >= speed;
            }
        }

        public bool StartRestGroundedCheck()
        {
            // Standard grounded will pass check immediately
            if (grounded)
                return true;

            // Collision fix for when player is levitating but feet are "close enough" to ground to rest
            // This is required as controller physics requires movement to process grounded collision normally
            Ray ray = new Ray(transform.position, Vector3.down);
            return (Physics.Raycast(ray, controller.height / 2 + 0.2f));
        }

        /// <summary>
        /// Cancels all movement impulses next frame.
        /// Used to scrub movement impulse when player dies, opens inventory, or loads game.
        /// Flag will be lowered again after movement cleared.
        /// </summary>
        public bool CancelMovement
        {
            get { return cancelMovement; }
            set { cancelMovement = value; }
        }

        /// <summary>
        /// Freeze motor for an amount of time in seconds.
        /// Used by teleport action to prevent player from falling when teleport is part of a physics change.
        /// It can take a few frames for physics to catch up.
        /// </summary>
        public float FreezeMotor
        {
            get { return freezeMotor; }
            set { freezeMotor = value; }
        }

        public Vector3 MoveDirection
        {
            get
            {
                return moveDirection;
            }
        }

        /// <summary>
        /// The method by which player is standing on exterior water.
        /// </summary>
        public OnExteriorWaterMethod OnExteriorWater
        {
            get { return onExteriorWaterMethod; }
        }

        /// <summary>
        /// The method by which player is standing on outdoor path.
        /// </summary>
        public bool OnExteriorPath
        {
            get { return onExteriorPathMethod; }
        }

        /// <summary>
        /// The method by which player is standing on outdoor path.
        /// </summary>
        public bool OnExteriorStaticGeometry
        {
            get { return onExteriorStaticGeometryMethod; }
        }

        #endregion

        #region Event Handlers

        void Start()
        {
            controller = GetComponent<CharacterController>();
            speedChanger = GetComponent<PlayerSpeedChanger>();
            speed = speedChanger.GetBaseSpeed();
            groundMotor = GetComponent<PlayerGroundMotor>();
            climbingMotor = GetComponent<ClimbingMotor>();
            heightChanger = GetComponent<PlayerHeightChanger>();
            levitateMotor = GetComponent<LevitateMotor>();
            frictionMotor = GetComponent<FrictionMotor>();
            acrobatMotor = GetComponent<AcrobatMotor>();
            playerScanner = GetComponent<PlayerMoveScanner>();
            playerEnterExit = GameManager.Instance.PlayerEnterExit;
            rappelMotor = GetComponent<RappelMotor>();
            //hangingMotor = GetComponent<HangingMotor>();

            // Allow for resetting specific player state on new game or when game starts loading
            SaveLoadManager.OnStartLoad += SaveLoadManager_OnStartLoad;
            StartGameBehaviour.OnNewGame += StartGameBehaviour_OnNewGame;
        }
      
        void FixedUpdate()
        {
            // Check if on a solid surface
            grounded = (collisionFlags & CollisionFlags.Below) != 0;

            // Time how long player has been grounded
            if (grounded)
                groundedTime += Time.deltaTime;
            else
                groundedTime = 0;

            // Clear movement
            if (cancelMovement)
            {
                moveDirection = Vector3.zero;
                cancelMovement = false;
                groundMotor.ClearActivePlatform();
                acrobatMotor.ClearFallingDamage();
                return;
            }

            // Handle freeze movement
            if (freezeMotor > 0)
            {
                freezeMotor -= Time.deltaTime;
                if (freezeMotor <= 0)
                {
                    freezeMotor = 0;
                    CancelMovement = true;
                }
                return;
            }

            playerScanner.FindHeadHit(new Ray(controller.transform.position, Vector3.up));
            playerScanner.SetHitSomethingInFront();

            // Can only engage climbing/rappel mode when on foot
            if (GameManager.Instance.TransportManager.IsOnFoot)
            {
                // Check if should hang
                //hangingMotor.HangingChecks();
                // Handle Rappeling
                rappelMotor.RappelChecks();
                // Handle climbing
                climbingMotor.ClimbingCheck();
            }

            // Do nothing if player levitating/swimming or climbing - replacement motor will take over movement for levitating/swimming
            if (levitateMotor && (levitateMotor.IsLevitating || levitateMotor.IsSwimming) || climbingMotor.IsClimbing /*|| hangingMotor.IsHanging*/)
            {
                moveDirection = Vector3.zero;
                return;
            }


            if (climbingMotor.WallEject)
            {   // True in terms of the player having their feet on solid surface.
                grounded = true;
            }

            UpdateSpeed();

            if (grounded)
            {
                acrobatMotor.Jumping = false;

                acrobatMotor.CheckFallingDamage();

                // checks if sliding and applies movement to moveDirection if true
                frictionMotor.GroundedMovement(ref moveDirection);

                acrobatMotor.HandleJumpInput(ref moveDirection);
            }
            else
            {
                acrobatMotor.CheckInitFall(ref moveDirection);

                acrobatMotor.CheckAirControl(ref moveDirection, speed);
            }

            playerScanner.FindStep(moveDirection);

            acrobatMotor.ApplyGravity(ref moveDirection);

            acrobatMotor.HitHead(ref moveDirection);

            groundMotor.MoveWithMovingPlatform(moveDirection);
        }

        void Update()
        {
            // Update on water check
            onExteriorWaterMethod = GetOnExteriorWaterMethod();
            onExteriorPathMethod = GetOnExteriorPathMethod();
            onExteriorStaticGeometryMethod = GetOnExteriorStaticGeometryMethod();

            heightChanger.DecideHeightAction();

            // Do nothing if player levitating - replacement motor will take over movement.
            // Don't return here for swimming because player should still be able to crouch when swimming.
            if (levitateMotor && levitateMotor.IsLevitating)
                return;

            speedChanger.CaptureInputSpeedAdjustment();

            UpdateSmoothFollower();
        }

        private void UpdateSpeed()
        {
            speed = speedChanger.GetBaseSpeed();
            speedChanger.ApplyInputSpeedAdjustment(ref speed);
            if (playerEnterExit.IsPlayerSwimming && !GameManager.Instance.PlayerEntity.IsWaterWalking)
                speed = speedChanger.GetSwimSpeed(speed);
        }

        // Store point that we're in contact with for use in FixedUpdate if needed
        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            frictionMotor.ContactPoint = hit.point;

            // Don't consider enemies as moving platforms
            // Otherwise, if we're positioned just right on top of one, the player camera gets unstable
            // This still allows standing on enemies
            if (hit.collider.gameObject.GetComponent<DaggerfallEnemy>() != null)
                return;

            // Get active platform
            if (hit.moveDirection.y < -0.9 && hit.normal.y > 0.5)
                groundMotor.ActivePlatform = hit.collider.transform;
        }

        private void StartGameBehaviour_OnNewGame()
        {
            ResetPlayerState();
        }

        private void SaveLoadManager_OnStartLoad(SaveData_v1 saveData)
        {
            ResetPlayerState();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Attempts to find the ground position below player, even if player is jumping/falling
        /// </summary>
        /// <param name="distance">Distance to fire ray.</param>
        /// <returns>Hit point on surface below player, or player position if hit not found in distance.</returns>
        public Vector3 FindGroundPosition(float distance = 10)
        {
            RaycastHit hit;
            Ray ray = new Ray(transform.position, Vector3.down);
            if (Physics.Raycast(ray, out hit, distance))
                return hit.point;

            return transform.position;
        }

        // Snap player to ground
        public bool FixStanding(float extraHeight = 0, float extraDistance = 0)
        {
            RaycastHit hit;
            Ray ray = new Ray(transform.position + (Vector3.up * extraHeight), Vector3.down);
            if (Physics.Raycast(ray, out hit, (controller.height * 2) + extraHeight + extraDistance))
            {
                // Position player at hit position plus just over half controller height up
                transform.position = hit.point + Vector3.up * (controller.height * 0.65f);
                return true;
            }

            return false;
        }

        // Gets distance between position and player
        public float DistanceToPlayer(Vector3 position)
        {
            return Vector3.Distance(transform.position, position);
        }

        #endregion

        #region Private Methods

        void UpdateSmoothFollower()
        {
            if (smoothFollower != null && controller != null)
            {
                float distanceMoved = Vector3.Distance(smoothFollowerPrevWorldPos, smoothFollower.position);        // Assuming the follower is a child of this motor transform we can get the distance travelled.
                float distanceThreshold = speedChanger.GetRunSpeed(speed) * Time.deltaTime;         // Without question any distance travelled less than the running speed is legal.
                float motorVelocity = controller.velocity.magnitude / Time.fixedDeltaTime;
                float maxPossibleDistanceByMotorVelocity = motorVelocity * Time.deltaTime * 2.0f;   // Theoretically the max distance the motor can carry the player with a generous margin.

                // NOTE: Maybe the min distance should also include the height different between crouching / standing.
                if (distanceMoved > distanceThreshold && distanceMoved > maxPossibleDistanceByMotorVelocity)
                {
                    smoothFollowerReset = true;
                }

                if (smoothFollowerReset)
                {
                    // Debug.Log("smooth follower reset");
                    smoothFollowerPrevWorldPos = transform.position;
                    smoothFollowerReset = false;
                }

                smoothFollower.position = Vector3.Lerp(smoothFollowerPrevWorldPos, transform.position, smoothFollowerLerpSpeed * Time.smoothDeltaTime);
                smoothFollowerPrevWorldPos = smoothFollower.position;
            }
        }
        void ResetPlayerState()
        {
            // Cancel levitation at start of loading a new save game
            // This prevents levitation flag carrying over and effect system can still restore it if needed
            if (levitateMotor)
                levitateMotor.IsLevitating = false;
        }

        /// <summary>
        /// Check if player is really standing on an outdoor tile, not just positioned above one.
        /// For example when player is on their ship they are standing above water but should not be swimming.
        /// Same when player is levitating above water they should not hear splash sounds.
        /// </summary>
        /// <returns>True if player is physically in range of an outdoor tile.</returns>
        bool GetOnExteriorGroundMethod()
        {
            float rayDistance = (GameManager.Instance.TransportManager.IsOnFoot) ? walkingRayDistance : ridingRayDistance;

            // Must be outside and actually be standing on a terrain object not some other object (e.g. player ship)
            RaycastHit hit;
            if (GameManager.Instance.PlayerEnterExit.IsPlayerInside || !Physics.Raycast(transform.position, Vector3.down, out hit, rayDistance * 2))
            {
                return false;
            }
            else
            {
                DaggerfallTerrain terrain = hit.transform.GetComponent<DaggerfallTerrain>();
                if (!terrain)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Check if player is really standing on an outdoor StaticGeometry object, not just positioned above one.
        /// For example when player is on their ship they are standing above water but should not be swimming.
        /// Same when player is levitating above water they should not hear splash sounds.
        /// </summary>
        /// <returns>True if player is physically in range of a StaticGeometry object.</returns>
        bool GetOnExteriorStaticGeometryMethod()
        {
            float rayDistance = (GameManager.Instance.TransportManager.IsOnFoot) ? walkingRayDistance : ridingRayDistance;

            // Must be outside and actually be standing on a terrain object not some other object (e.g. player ship)
            RaycastHit hit;
            if (GameManager.Instance.PlayerEnterExit.IsPlayerInside || !Physics.Raycast(transform.position, Vector3.down, out hit, rayDistance * 2))
            {
                return false;
            }
            else
            {
                return hit.collider.tag.Equals("StaticGeometry");
            }
        }

        /// <summary>
        /// Check if player is really standing on an shallow water tile, determined by if the water design takes up the majority of the texture.
        /// </summary>
        /// <returns>True if player is is on a shallow water tile.</returns>
        bool OnShallowWaterTile()
        {
            return GameManager.Instance.StreamingWorld.PlayerTileMapIndex == 5
                || GameManager.Instance.StreamingWorld.PlayerTileMapIndex == 6
                || GameManager.Instance.StreamingWorld.PlayerTileMapIndex == 8
                || GameManager.Instance.StreamingWorld.PlayerTileMapIndex == 20
                || GameManager.Instance.StreamingWorld.PlayerTileMapIndex == 21
                || GameManager.Instance.StreamingWorld.PlayerTileMapIndex == 23
                || GameManager.Instance.StreamingWorld.PlayerTileMapIndex == 30
                || GameManager.Instance.StreamingWorld.PlayerTileMapIndex == 31
                || (GameManager.Instance.StreamingWorld.PlayerTileMapIndex >= 33 && GameManager.Instance.StreamingWorld.PlayerTileMapIndex <= 36)
                || GameManager.Instance.StreamingWorld.PlayerTileMapIndex == 49;
        }

        /// <summary>
        /// Check if player is really standing on a path tile.
        /// </summary>
        /// <returns>True if player is is on a path tile.</returns>
        bool OnPathTile()
        {
            return GameManager.Instance.StreamingWorld.PlayerTileMapIndex == 46
                || GameManager.Instance.StreamingWorld.PlayerTileMapIndex == 47
                || GameManager.Instance.StreamingWorld.PlayerTileMapIndex == 55;
        }

        /// <summary>
        /// Check if player is really standing on an outdoor water tile, not just positioned above one.
        /// For example when player is on their ship they are standing above water but should not be swimming.
        /// Same when player is levitating above water they should not hear splash sounds.
        /// </summary>
        /// <returns>True if player is physically in range of an outdoor water tile.</returns>
        OnExteriorWaterMethod GetOnExteriorWaterMethod()
        {
            bool onShallowWaterTile = OnShallowWaterTile();
            if (!GetOnExteriorGroundMethod()
                || (GameManager.Instance.StreamingWorld.PlayerTileMapIndex != 0 && !onShallowWaterTile))
               return OnExteriorWaterMethod.None;

            // Handle swimming/waterwalking
            if (GameManager.Instance.PlayerEntity.IsWaterWalking || onShallowWaterTile)
                return OnExteriorWaterMethod.WaterWalking;
            else
                return OnExteriorWaterMethod.Swimming;
        }

        /// <summary>
        /// Check if player is really standing on an outdoor path tile, not just positioned above one.
        /// </summary>
        /// <returns>True if player is physically in range of an outdoor path tile.</returns>
        bool GetOnExteriorPathMethod()
        {
            return GetOnExteriorGroundMethod() && OnPathTile();
        }

        #endregion
    }
}
