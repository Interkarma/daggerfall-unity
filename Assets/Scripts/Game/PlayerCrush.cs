using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DaggerfallWorkshop.Game
{
    public class PlayerCrush : MonoBehaviour
    {
        private PlayerHeightChanger heightChanger;
        private PlayerMotor playerMotor;
        private CharacterController controller;
        private float previousHeightHit;
    
	    void Start ()
        {
            heightChanger = GetComponent<PlayerHeightChanger>();
            playerMotor = GetComponent<PlayerMotor>();
            controller = GetComponent<CharacterController>();
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

            Ray ray = new Ray(controller.transform.position, Vector3.up);
            RaycastHit hit = new RaycastHit();
            if (Physics.SphereCast(ray, controller.radius * 0.85f, out hit, distance))
            {
                if (hit.collider.GetComponent<MeshCollider>())
                {
                    // If player is standing, crushing object forces them into a crouch, 
                    if (!playerMotor.IsCrouching && heightChanger.HeightAction != HeightChangeAction.DoCrouching)
                    {
                        heightChanger.HeightAction = HeightChangeAction.DoCrouching;
                    }
                    // if player already crouching and on the ground, then kill.
                    else if (playerMotor.IsCrouching && playerMotor.IsGrounded)
                    {
                        if (previousHeightHit > 0 && previousHeightHit > hit.distance)
                            GameManager.Instance.PlayerEntity.SetHealth(0);
                    }
                    previousHeightHit = hit.distance;
                }
            }
            else
                previousHeightHit = 0f;
        }
    }
}
