using DaggerfallConnect;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DaggerfallWorkshop.Game
{
    public class RappelMotor : MonoBehaviour
    {
        public enum RappelDirection
        {
            None,
            UpBehind,
            DownBehind,
            DownUnder,
            FrontUp
        }
        public enum RappelStage
        {
            Inactive,
            /// <summary>
            /// Rappel Direction has been set
            /// </summary>
            Activated,
            /// <summary>
            /// Player is moving into grappling position
            /// </summary>
            Swooping,
            /// <summary>
            /// player is applying force to surface to allow corresponding motor to attach the player
            /// </summary>
            Grappling
        }
        private float rappelTimer;
        private Vector3 swoopBasePosition = Vector3.zero;
        private bool updateSwoopBasePosition = false;
        private Vector3 rappelPosition;
        private Vector3 grappleDirection = Vector3.zero;
        public bool IsRappelling { get; private set; }
        private RappelDirection rappelDirection;
        private RappelStage rappelStage;
        private AcrobatMotor acrobatMotor;
        private CharacterController controller;
        private PlayerSpeedChanger speedChanger;
        private PlayerGroundMotor groundMotor;
        private ClimbingMotor climbingMotor;
        private PlayerMoveScanner playerScanner;
        private HangingMotor hangingMotor;
        private Entity.PlayerEntity player;
        private VectorMeasurement measure;


        private void Start()
        {
            player = GameManager.Instance.PlayerEntity;

            acrobatMotor = GetComponent<AcrobatMotor>();
            controller = GetComponent<CharacterController>();
            speedChanger = GetComponent<PlayerSpeedChanger>();
            groundMotor = GetComponent<PlayerGroundMotor>();
            climbingMotor = GetComponent<ClimbingMotor>();
            playerScanner = GetComponent<PlayerMoveScanner>();
            hangingMotor = GetComponent<HangingMotor>();
        }

        public void InitialSetRappelType()
        {
            // "rappelling" only happens while moving the player into climbing position 
            // from the ledge he backstepped off and doesn't happen while climbing

            bool rappelAllowed = (DaggerfallUnity.Settings.AdvancedClimbing
                //&& !playerScanner.HitSomethingInFront
                && InputManager.Instance.HasAction(InputManager.Actions.MoveBackwards)
                && !climbingMotor.IsSlipping && acrobatMotor.Falling && !acrobatMotor.Jumping);

            if (!rappelAllowed)
                return;

            // TODO: change this possibly to allow rappelling under to FrontUnderCeiling

            if (rappelStage == RappelStage.Inactive)
            {
                if (playerScanner.AboveBehindWall != null)
                    rappelDirection = RappelDirection.UpBehind;
                else if (playerScanner.FrontUnderCeiling != null)
                    rappelDirection = RappelDirection.DownUnder;
                else if (playerScanner.BelowBehindWall != null)
                    rappelDirection = RappelDirection.DownBehind;

                if (rappelDirection != RappelDirection.None)
                {
                    rappelStage = RappelStage.Activated;
                    IsRappelling = true;
                }

            }
        }

        private bool InitialTooCloseToGround(RappelDirection direction)
        {
            if (rappelStage == RappelStage.Activated && direction == RappelDirection.DownBehind)
            {
                float minRange = (0);
                // greater maxRange values mean player won't rappel from longer heights
                float maxRange = minRange + 2.90f;

                // are we going to step off something too short for rappel to be worthwhile?
                return (playerScanner.StepHitDistance > minRange && playerScanner.StepHitDistance < maxRange);
            }
            return false;
        }
        private void InitialSetGrappleDirection()
        {
            // rappelMotor doesn't call to destroy the adjacent surface because it needs to be
            // used by the corresponding motor (climbing/hanging)
            if (rappelDirection == RappelDirection.UpBehind)
                grappleDirection = Vector3.ProjectOnPlane(playerScanner.AboveBehindWall.grabDirection, Vector3.up);
            else if (rappelDirection == RappelDirection.DownBehind)
                grappleDirection = Vector3.ProjectOnPlane(playerScanner.BelowBehindWall.grabDirection, Vector3.up);
            else if (playerScanner.FrontUnderCeiling != null)
                playerScanner.CutAndPasteFrontUnderCeilingTo(ref grappleDirection);

            // failsafe
            if (grappleDirection == Vector3.zero)
                grappleDirection = controller.transform.forward;
        }
        /// <summary>
        /// Check if should do rappel, and do rappel and attach to wall.
        /// </summary>
        public void RappelChecks()
        {
            if (rappelStage == RappelStage.Inactive)
            {
                measure = null;
            }

            InitialSetRappelType();

            bool inputBackward = InputManager.Instance.HasAction(InputManager.Actions.MoveBackwards);
            Vector3 origin = Vector3.zero;
            Vector3 direction = -controller.transform.forward;

            #region Scan For AdjacentSurface
            if (inputBackward)
            {
                // check from different origins to find different surfaces
                if(!climbingMotor.IsClimbing)
                    origin = controller.transform.position + Vector3.down * (0.25f * controller.height) + controller.transform.forward * (0.8f * controller.radius);
                else
                    origin = controller.transform.position + Vector3.up * (0.25f * controller.height) + controller.transform.forward * (0.8f * controller.radius);

                playerScanner.FindAdjacentSurface(origin, direction, PlayerMoveScanner.RotationDirection.YZCounterClockwise);
            }
            #endregion

            // the only time Ground closeness can cancel the start of the rappel is when we are
            // going to rappel down and behind to a wall
            if (InitialTooCloseToGround(rappelDirection))
            {
                IsRappelling = false;
                return;
            }

            if (rappelStage == RappelStage.Activated)
            {
                DaggerfallUI.AddHUDText(UserInterfaceWindows.HardStrings.rappelMode);
                rappelStage = RappelStage.Swooping;
                InitialSetGrappleDirection();

                swoopBasePosition = controller.transform.position;
                rappelTimer = 0f;
                measure = null;
            }

            // Rappel swooping
            if (rappelStage == RappelStage.Swooping) 
            {
                player.TallySkill(DFCareer.Skills.Climbing, 1);
                Vector3 swoopDirection = grappleDirection;
                // if we are rappelling under to ceiling, grappledirection is different so use adjacentSurfaceRay
                // direction to get right direction to go under the ceiling.
                if (rappelDirection == RappelDirection.DownUnder || rappelDirection == RappelDirection.FrontUp)
                    swoopDirection = playerScanner.WallDetachedVector;
                rappelTimer += Time.deltaTime;

                switch (rappelDirection)
                {
                    case RappelDirection.DownBehind:
                        CurlDown(swoopBasePosition, swoopDirection);
                        break;
                    case RappelDirection.UpBehind:
                        CurlOver(swoopBasePosition, swoopDirection);
                        break;
                    case RappelDirection.DownUnder:
                        CurlUnder(swoopBasePosition, swoopDirection);
                        break;
                    case RappelDirection.FrontUp:
                        if (updateSwoopBasePosition)
                        {   // enables player to bottom out on DownUnder and continue FrontUp
                            swoopBasePosition = controller.transform.position;
                            updateSwoopBasePosition = false;
                            rappelTimer = 0;
                        }
                        CurlUpHalf(swoopBasePosition, swoopDirection);
                        break;
                    default:
                        break;
                }

                controller.transform.position = rappelPosition;
            }

            if (rappelStage == RappelStage.Grappling)
            // perform horizontal measurement-based Wall-grapple direction or vertical for ceiling
            {
                // if measurement hasn't started, start measuring grapple-to-surface movement
                if (measure == null)
                    measure = new VectorMeasurement(controller.transform.position);
                
                if ( !(hangingMotor.IsHanging || climbingMotor.IsClimbing)
                    && measure.Distance(controller.transform.position) < 1f)
                {
                    // Auto move toward surface to grab
                    float speed = speedChanger.GetBaseSpeed();

                    grappleDirection = grappleDirection.normalized * speed * 1.15f;
                    groundMotor.MoveWithMovingPlatform(grappleDirection);
                }
                else // if we've moved past the distance limit
                {
                    rappelStage = RappelStage.Inactive;
                    rappelDirection = RappelDirection.None;
                    rappelTimer = 0f;
                    measure = null;
                    IsRappelling = false;
                }
            }
        }

        private void CurlDown(Vector3 lastPosition, Vector3 xzDirection)
        {
            const float timerMax = 0.7f, xzDist = 0.17f, yDist = 1.60f;
            Vector3 pos = lastPosition;

            if (rappelTimer <= timerMax)
            {
                float sinTimerScaledPI = Mathf.Sin(Mathf.PI * (rappelTimer / timerMax));
                rappelPosition.x = Mathf.Lerp(pos.x, pos.x - (xzDirection.x * xzDist), sinTimerScaledPI);
                rappelPosition.z = Mathf.Lerp(pos.z, pos.z - (xzDirection.z * xzDist), sinTimerScaledPI);
                rappelPosition.y = Mathf.Lerp(pos.y, pos.y - yDist, rappelTimer / timerMax);
            }
            else
                rappelStage = RappelStage.Grappling;
        }
        private void CurlOver(Vector3 lastPosition, Vector3 xzDirection)
        {
            const float timerMax = 0.7f, xzDist = 0.19f, yDist = 1.60f;
            Vector3 pos = lastPosition;
            
            if (rappelTimer <= timerMax)
            {
                float sinTimerScaledPI = Mathf.Sin(Mathf.PI * (rappelTimer / timerMax));
                rappelPosition.x = Mathf.Lerp(pos.x, pos.x - (xzDirection.x * xzDist), sinTimerScaledPI);
                rappelPosition.z = Mathf.Lerp(pos.z, pos.z - (xzDirection.z * xzDist), sinTimerScaledPI);
                rappelPosition.y = Mathf.Lerp(pos.y, pos.y + yDist, rappelTimer / timerMax);
            }
            else
                rappelStage = RappelStage.Grappling;
        }
        private void CurlUnder(Vector3 lastPosition, Vector3 xzDirection)
        {
            float timerMax = 0.7f, xzDist = 0.10f, yDist = 1.4f;
            Vector3 pos = lastPosition;

            if (rappelTimer <= timerMax)
            {
                float sinTimerScaledPI = Mathf.Sin(Mathf.PI * (rappelTimer / timerMax));
                rappelPosition.x = Mathf.Lerp(pos.x, pos.x - (xzDirection.x * xzDist), sinTimerScaledPI);
                rappelPosition.z = Mathf.Lerp(pos.z, pos.z - (xzDirection.z * xzDist), sinTimerScaledPI);
                rappelPosition.y = Mathf.Lerp(pos.y, pos.y - yDist, (rappelTimer / timerMax));
            }
            else
            {
                updateSwoopBasePosition = true;
                rappelDirection = RappelDirection.FrontUp; 
            }
                           
        }
        private void CurlUpHalf(Vector3 lastPosition, Vector3 xzDirection)
        {
            float timerMax = 0.35f, xzDist = 0.70f, yDist = 0.40f;
            Vector3 pos = lastPosition;
            // running for only half the timerMax value so that the sin wave crests at pi/2
            if (rappelTimer <= timerMax/2f)
            {
                float timerPercent = (rappelTimer / timerMax);
                float timerScaledPI = Mathf.PI * timerPercent;
                rappelPosition.x = Mathf.Lerp(pos.x, pos.x + (xzDirection.x * xzDist), Mathf.Sin(timerScaledPI));
                rappelPosition.z = Mathf.Lerp(pos.z, pos.z + (xzDirection.z * xzDist), Mathf.Sin(timerScaledPI));
                rappelPosition.y = Mathf.Lerp(pos.y, pos.y + yDist, timerPercent);
            }
            else
                rappelStage = RappelStage.Grappling;
        }
    }
}
