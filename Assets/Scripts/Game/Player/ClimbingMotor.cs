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
        private PlayerMotor playerMotor;
        private LevitateMotor levitateMotor;
        private CharacterController controller;
        private PlayerEnterExit playerEnterExit;
        //private PlayerHeightChanger heightChanger;
        private bool failedClimbingCheck = false;
        private bool isClimbing = false;
        private float climbingStartTimer = 0;
        private float climbingContinueTimer = 0;
        private uint timeOfLastClimbingCheck = 0;
        private bool showClimbingModeMessage = true;
        private Vector2 lastHorizontalPosition = Vector2.zero;
        private Vector3 ledgeDirection = Vector3.zero;
        public bool IsClimbing
        {
            get { return isClimbing; }
        }
        void Start()
        {
            playerMotor = GetComponent<PlayerMotor>();
            levitateMotor = GetComponent<LevitateMotor>();
            controller = GetComponent<CharacterController>();
            playerEnterExit = GetComponent<PlayerEnterExit>();
            //heightChanger = GetComponent<PlayerHeightChanger>();
        }

        /// <summary>
        /// Perform climbing check, and if successful, start climbing movement.
        /// </summary>
        /// <param name="collisionFlags"></param>
        public void ClimbingCheck(ref CollisionFlags collisionFlags)
        {
            float stopClimbingDistance = 0.12f;

            if (isClimbing)
                collisionFlags = CollisionFlags.Sides;

            // Climbing
            uint gameMinutes = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime();
            if (!InputManager.Instance.HasAction(InputManager.Actions.MoveForwards)
                || (collisionFlags & CollisionFlags.Sides) == 0
                || failedClimbingCheck
                || levitateMotor.IsLevitating
                || playerMotor.IsRiding
                || (isClimbing && CanWalkOntoLedge()) // Don't check for ledge walk unless climbing.
                //|| (playerMotor.IsCrouching && !heightChanger.ForcedSwimCrouch)
                // don't do horizontal position check if already climbing
                || (!isClimbing && Vector2.Distance(lastHorizontalPosition, new Vector2(controller.transform.position.x, controller.transform.position.z)) > stopClimbingDistance))
            {
                isClimbing = false;
                showClimbingModeMessage = true;
                climbingStartTimer = 0;
                timeOfLastClimbingCheck = gameMinutes;

                // Reset position for horizontal distance check
                lastHorizontalPosition = new Vector2(controller.transform.position.x, controller.transform.position.z);
            }
            else
            {
                if (climbingStartTimer <= (playerMotor.systemTimerUpdatesPerSecond* 14))
                    climbingStartTimer += Time.deltaTime;
                else
                {
                    if (!isClimbing)
                    {
                        if (showClimbingModeMessage)
                            DaggerfallUI.AddHUDText(UserInterfaceWindows.HardStrings.climbingMode);
                        // Disable further showing of climbing mode message until current climb attempt is stopped
                        // to keep it from filling message log
                        showClimbingModeMessage = false;
                        isClimbing = true;
                    }

                    // Initial check to start climbing
                    if ((gameMinutes - timeOfLastClimbingCheck) > 18)
                    {
                        Entity.PlayerEntity player = GameManager.Instance.PlayerEntity;
                        player.TallySkill(DFCareer.Skills.Climbing, 1);
                        timeOfLastClimbingCheck = gameMinutes;
                        if (UnityEngine.Random.Range(1, 101) > 95)
                        {
                            if (UnityEngine.Random.Range(1, 101) > player.Skills.GetLiveSkillValue(DFCareer.Skills.Climbing))
                            {
                                isClimbing = false;
                                failedClimbingCheck = true;
                            }
                        }
                    }
                }
            }

            if (isClimbing)
                ClimbMovement();
            else
                ledgeDirection = Vector3.zero;
        }

        private bool CanWalkOntoLedge()
        {
            RaycastHit hit;

            Vector3 p1 = controller.transform.position + controller.center + Vector3.up * -controller.height * 0.40f;
            Vector3 p2 = p1 + Vector3.up * controller.height;
            float distanceToObstacle = 0;

            // Cast character controller shape forward to see if it is about to hit anything.
            if (Physics.CapsuleCast(p1, p2, controller.radius, controller.transform.forward, out hit, 0.15f))
            {
                distanceToObstacle = hit.distance;
                //ledgeDirection = -hit.normal;
            }

            bool canWalkOntoLedge = (distanceToObstacle == 0);

            // evalate the ledge direction only once when starting to climb
            if (ledgeDirection == Vector3.zero)
                ledgeDirection = -hit.normal;
            else if (canWalkOntoLedge)
                ledgeDirection = Vector3.zero;

            //Debug.Log("DistanceToWall: " + distanceToObstacle);

            return canWalkOntoLedge;
        }

        private void ClimbMovement()
        {
            Entity.PlayerEntity player = GameManager.Instance.PlayerEntity;

            // Try to move up and forwards at same time
            // This helps player smoothly mantle the top of whatever they are climbing
            // Horizontal distance check in ClimbingCheck() will cancel climb once player mantles
            // This has the happy side effect of fixing issue where player climbs endlessly into sky or starting to climb when not facing wall
            Vector3 moveDirection = ledgeDirection * playerMotor.Speed;
            moveDirection.y = Vector3.up.y;

            // Climbing effect states "target can climb twice as well" - doubling climbing speed
            if (player.IsEnhancedClimbing)
                moveDirection.y *= 2;

            controller.Move(moveDirection * Time.deltaTime);

            if (climbingContinueTimer <= (playerMotor.systemTimerUpdatesPerSecond * 15))
                climbingContinueTimer += Time.deltaTime;
            else
            {
                climbingContinueTimer = 0;
                player.TallySkill(DFCareer.Skills.Climbing, 1);
                int skill = player.Skills.GetLiveSkillValue(DFCareer.Skills.Climbing);
                if (player.Race == Entity.Races.Khajiit)
                    skill += 30;

                // Climbing effect states "target can climb twice as well" - doubling effective skill after racial applied
                if (player.IsEnhancedClimbing)
                    skill *= 2;

                // Clamp skill range
                skill = Mathf.Clamp(skill, 5, 95);

                if ((UnityEngine.Random.Range(1, 101) > 90)
                    || (UnityEngine.Random.Range(1, 101) > skill))
                {
                    // Don't allow skill check to break climbing while swimming
                    // This is another reason player can't climb out of water - any slip in climb will throw them back into swim mode
                    // For now just pretend water is supporting player while they climb
                    // It's not enough to check if they are swimming, need to check if their feet are above water. - MeteoricDragon
                    var playerPos = controller.transform.position.y + (76 * MeshReader.GlobalScale) - 0.95f;
                    var playerFootPos = playerPos - (controller.height / 2) - 1.20f; // to prevent player from failing to climb out of water
                    var waterPos = playerEnterExit.blockWaterLevel * -1 * MeshReader.GlobalScale;
                    if (playerFootPos >= waterPos)
                        isClimbing = false;
                }
            }
        }
    }
}


