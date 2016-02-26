// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Lypyl (lypyl@dfworkshop.net)
// Contributors:    
// 
// Notes:
//


using DaggerfallWorkshop.Game;
using UnityEngine;

namespace DaggerfallWorkshop
{
    [RequireComponent(typeof(Rigidbody))]
    public class DaggerfallActionCollision : MonoBehaviour
    {
        private DaggerfallAction thisAction = null;
        private Rigidbody rigBody           = null;
        float timer                         = 0;
        public bool colliding               = false;
        public float timeout                = .125f;

        // Use this for initialization
        void Awake()
        {
            SetupCollision();
        }


        public void SetupCollision()
        {
            if (!thisAction)
                thisAction = GetComponent<DaggerfallAction>();
            if(!rigBody)
                rigBody = GetComponent<Rigidbody>();

            rigBody.isKinematic     = true;
            rigBody.useGravity      = false;
            rigBody.freezeRotation  = true;

        }

        void OnTriggerEnter(Collider col)
        {
            if (col.transform.gameObject.tag != "Player")
                return;
            colliding = true;
        }

        void OnTriggerStay(Collider col)
        {
            if (col.transform.gameObject.tag != "Player")
                return;
            colliding = true;
        }

        void OnTriggerExit(Collider col)
        {
            if (col.transform.gameObject.tag != "Player")
                return;
            colliding = false;
        }


        void Update()
        {
            if(!colliding)
                return;
            if ((timer += Time.deltaTime) < timeout)
                return;
            else
                timer = 0;

            //only trigger collision when player has moved
            if (InputManager.Instance.HasAction(InputManager.Actions.MoveForwards)
                    || InputManager.Instance.HasAction(InputManager.Actions.MoveBackwards)
                    || InputManager.Instance.HasAction(InputManager.Actions.MoveLeft)
                    || InputManager.Instance.HasAction(InputManager.Actions.MoveRight)
                    //|| InputManager.Instance.HasAction(InputManager.Actions.FloatUp)        //up/down don't seem to trigger collisions in daggerfall
                    //|| InputManager.Instance.HasAction(InputManager.Actions.FloatDown)
                    || InputManager.Instance.HasAction(InputManager.Actions.Jump)
                )
            {
                //start action
                //TODO: Determine if colloision is something player is walking on or into like wall, and set TriggerType accordingly
               thisAction.Receive(GameManager.Instance.PlayerObject, DaggerfallAction.TriggerTypes.WalkInto);
            }

        }

    }

}
