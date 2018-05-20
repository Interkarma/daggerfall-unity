using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DaggerfallWorkshop.Game
{
    public class PlayerCrush : MonoBehaviour
    {
        private PlayerHeightChanger heightChanger;
        private PlayerMotor playerMotor;
        private SphereCollider crushCollider;
        private CharacterController controller;
    
	    void Start ()
        {
            heightChanger = GetComponent<PlayerHeightChanger>();
            playerMotor = GetComponent<PlayerMotor>();
            
            crushCollider = GetComponent<SphereCollider>();
            crushCollider.center += new Vector3(0, 0.45f);
            controller = GetComponent<CharacterController>();
	    }
	
	    void Update ()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            // if we didn't collide with a crushing object, return
            if (other.gameObject.GetComponent<MeshCollider>() == null)
                return;

            // If player is standing, crushing object forces them into a crouch, 
            if (!playerMotor.IsCrouching && heightChanger.HeightAction != HeightChangeAction.DoCrouching)
            {
                heightChanger.HeightAction = HeightChangeAction.DoCrouching;
            }
            // if player already crouching, then kill.
            else if (playerMotor.IsCrouching)
            {
                GameManager.Instance.PlayerEntity.SetHealth(0);
            }
        }
    }
}
