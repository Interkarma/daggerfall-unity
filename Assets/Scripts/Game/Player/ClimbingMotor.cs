using UnityEngine;
using System;
using System.Collections;
using DaggerfallConnect;

namespace DaggerfallWorkshop.Game
{

    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerFootsteps))]
    public class ClimbingMotor : MonoBehaviour
    {
        private PlayerMotor playerMotor;
        private LevitateMotor levitateMotor;
        private CharacterController controller;
        private bool failedClimbingCheck = false;
        private bool isClimbing = false;
        private float climbingStartTimer = 0;
        private float climbingContinueTimer = 0;
        private uint timeOfLastClimbingCheck = 0;
        private bool showClimbingModeMessage = true;
        private Vector2 lastHorizontalPosition = Vector2.zero;
        public bool IsClimbing
        {
            get { return isClimbing; }
            set { isClimbing = value; }
        }
        void Start()
        {
            playerMotor = GetComponent<PlayerMotor>();
            levitateMotor = GetComponent<LevitateMotor>();
            controller = GetComponent<CharacterController>();
        }

        void Update()
        {

        }

        public void ClimbingCheck(ref CollisionFlags collisionFlags)
        {
            // Get pre-movement position for climbing check
            lastHorizontalPosition = new Vector2(controller.transform.position.x, controller.transform.position.z);

            if (isClimbing)
                collisionFlags = CollisionFlags.Sides;
            // Get collision flags for swimming as well, so it's possible to climb out of water TODO: Collision flags from swimming aren't working
            else if (levitateMotor.IsSwimming)
                collisionFlags = levitateMotor.CollisionFlags;

            // Climbing
            uint gameMinutes = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime();
            if (!InputManager.Instance.HasAction(InputManager.Actions.MoveForwards)
                || (collisionFlags & CollisionFlags.Sides) == 0
                || failedClimbingCheck
                || levitateMotor.IsLevitating
                || playerMotor.IsRiding
                || Vector2.Distance(lastHorizontalPosition, new Vector2(controller.transform.position.x, controller.transform.position.z)) >= (0.003f)) // Approximation based on observing classic in-game
            {
                isClimbing = false;
                showClimbingModeMessage = true;
                climbingStartTimer = 0;
                timeOfLastClimbingCheck = gameMinutes;
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
        }

        private void ClimbMovement()
        {
            controller.Move(Vector3.up * Time.deltaTime);
            if (climbingContinueTimer <= (playerMotor.systemTimerUpdatesPerSecond * 15))
                climbingContinueTimer += Time.deltaTime;
            else
            {
                climbingContinueTimer = 0;
                Entity.PlayerEntity player = GameManager.Instance.PlayerEntity;
                player.TallySkill(DFCareer.Skills.Climbing, 1);
                int skill = player.Skills.GetLiveSkillValue(DFCareer.Skills.Climbing);
                if (player.Race == Entity.Races.Khajiit)
                    skill += 30;
                Mathf.Clamp(skill, 5, 95);

                if ((UnityEngine.Random.Range(1, 101) > 90)
                    || (UnityEngine.Random.Range(1, 101) > skill))
                {
                    isClimbing = false;
                }
            }
        }
    }
}


