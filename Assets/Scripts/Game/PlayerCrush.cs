using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop.Game
{
    public class PlayerCrush : MonoBehaviour
    {
        private PlayerHeightChanger heightChanger;
        private PlayerMotor playerMotor;
        private CharacterController controller;
        private PlayerMoveScanner moveScanner;
        private float previousHeightHit;
    
	    void Start ()
        {
            heightChanger = GetComponent<PlayerHeightChanger>();
            playerMotor = GetComponent<PlayerMotor>();
            controller = GetComponent<CharacterController>();
            moveScanner = GetComponent<PlayerMoveScanner>();
            previousHeightHit = 0f;
	    }
	
	    void Update ()
        {
            if (GameManager.Instance.PlayerEntity.CurrentHealth < 1 
                || GameManager.IsGamePaused
                || GameManager.Instance.PlayerMotor.IsSwimming
                || GameManager.Instance.PlayerMotor.IsLevitating
                || GameManager.Instance.PlayerMotor.OnExteriorWater == PlayerMotor.OnExteriorWaterMethod.Swimming)
                return;

            float distance;
            if (!playerMotor.IsCrouching)
                distance = (controller.height / 2f) - (controller.height * 0.1f);
            else
                distance = (controller.height / 2f);
            
            if (moveScanner.HeadHitDistance < distance && moveScanner.HeadHitDistance > 0)
            {
                // Tests to prevent player being crushed by static geometry, non-action colliders, or currently stationary action objects
                // Only perform these tests if move scanner head raycast has found a valid transform
                if (moveScanner.HeadRaycastHit.transform)
                {
                    // Do nothing if move scanner has detected a static gameobject
                    // This prevents player from being crushed under sloping non-moving geometry found on boat and inside buildings
                    // Also stops player being forced into a crouch from just brushing up against sloping geometry
                    if (GameObjectHelper.IsStaticGeometry(moveScanner.HeadRaycastHit.transform.gameObject))
                        return;

                    // We found a non-static object, but it really an action object (e.g. moving platform)?
                    DaggerfallAction action = moveScanner.HeadRaycastHit.transform.gameObject.GetComponent<DaggerfallAction>();
                    if (!action)
                        return;

                    // Confirm dynamic object actually in motion, not just a stationary action object player happened to bump their head into
                    if (!action.IsMoving)
                        return;

                    // Exclude "BOX" to prevent unintended crushing hazard in Mantellan Crux - player instead pushed back by physics
                    if (action.ModelDescription == "BOX")
                        return;

                    // This object fits criteria for a crushing object
                    // Player will first be forced into a crouch then killed if they remain under crushing object past threshold height
                    if (!playerMotor.IsCrouching && heightChanger.HeightAction != HeightChangeAction.DoCrouching)
                    {
                        // If player is standing then crushing object forces them into a crouch
                        heightChanger.HeightAction = HeightChangeAction.DoCrouching;
                    }
                    else if (playerMotor.IsCrouching && playerMotor.IsGrounded)
                    {
                        // If player already crouching and on the ground, then kill
                        if (previousHeightHit > 0 && previousHeightHit > moveScanner.HeadHitDistance)
                            GameManager.Instance.PlayerEntity.SetHealth(0);
                    }
                }

                previousHeightHit = moveScanner.HeadHitDistance;
            }
            else
                previousHeightHit = 0f;
        }
    }
}
