// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
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
    public class DaggerfallActionCollision : PlayerCollisionHandler
    {
        public bool Colliding               = false;
        public float Timeout                = .12f;
        public DaggerfallAction.TriggerTypes CollisionType = DaggerfallAction.TriggerTypes.None;

        private DaggerfallAction thisAction = null;
        private float timer                 = 0;


        public void Start()
        {
            if (!thisAction)
                thisAction = GetComponent<DaggerfallAction>();
        }



        void FixedUpdate()
        {

            if ((timer += Time.deltaTime) < Timeout)
                return;
            if (!Colliding)
                return;

            //only trigger collision when player has actively moved
            if (InputManager.Instance.HasAction(InputManager.Actions.MoveForwards)
                || InputManager.Instance.HasAction(InputManager.Actions.MoveBackwards)
                || InputManager.Instance.HasAction(InputManager.Actions.MoveLeft)
                || InputManager.Instance.HasAction(InputManager.Actions.MoveRight))//up/down//jump don't seem to trigger action in daggerfall
                {
                    //start action
                    thisAction.Receive(GameManager.Instance.PlayerObject, CollisionType);
                    timer = 0;
                }

            Colliding = false;
            CollisionType = DaggerfallAction.TriggerTypes.None;

        }


        public override void OnCharacterCollided(ControllerColliderHit hit, Transform other)
        {
            base.OnCharacterCollided(hit, other);

            if (Colliding)
                return;

            //check if hit point beneath player
            var dir = (hit.point - other.position).normalized;
            if (dir.y < -0.9)
                CollisionType = DaggerfallAction.TriggerTypes.WalkOn;
            else
            {
                if (thisAction.TriggerFlag == DaggerfallConnect.DFBlock.RdbTriggerFlags.Collision01)
                {
                    Vector3 origin = new Vector3(hit.controller.transform.position.x, hit.controller.transform.position.y - hit.controller.height / 2, hit.controller.transform.position.z);
                    Ray ray = new Ray(origin, Vector3.down);
                    RaycastHit hitInfo;

                    //if hit not below controller, see if player standing on this action object w/ raycast & if action flag is Collision01 (walk on)
                    //set trigger type & activate if so - to avoid player being able to push against wall to avoid
                    if (hit.collider.Raycast(ray, out hitInfo, hit.controller.skinWidth))
                        CollisionType = DaggerfallAction.TriggerTypes.WalkOn;
                    else
                        CollisionType = DaggerfallAction.TriggerTypes.WalkInto;
                }
                else
                    CollisionType = DaggerfallAction.TriggerTypes.WalkInto;

            }

            Colliding = true;
        }

    }

}
