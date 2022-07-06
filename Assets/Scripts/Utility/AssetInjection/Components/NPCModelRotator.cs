// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut
// Contributors:
// 
// Notes:
//

using DaggerfallWorkshop.Game;
using UnityEngine;

namespace DaggerfallWorkshop.Utility.AssetInjection
{
    /// <summary>
    /// Finds an appropriate position and rotation for objects that replace billboard wall props such as torches.
    /// </summary>
    /// <remarks>
    /// This component performs three operations:
    /// 1. Ensures the object faces the wall on the set direction and rotates if needed.
    /// 2. Moves the object next to the wall or away from it if clipping.
    /// 3. Aligns the object if wall is not perpendicular to the floor.
    /// </remarks>
    public class NPCModelRotator : MonoBehaviour, IObjectPositioner
    {
        /// <summary>
        /// Always false because rotation is set for NPC facing.
        /// </summary>
        public virtual bool AllowFlatRotation
        {
            get { return false; }
        }

        private void Start()
        {
            // If player in inside a building, then apply rotation to model so it faces the nearest internal (action) door.
            // Prefers visible doors if there are any.
            // TODO: Add checks for static doors? (entry / exit doors)

            PlayerEnterExit playerEnterExit = GameManager.Instance.PlayerEnterExit;
            if (playerEnterExit.IsPlayerInsideBuilding)
            {
                DaggerfallActionDoor[] actionDoors = playerEnterExit.InteriorParent.GetComponentsInChildren<DaggerfallActionDoor>();

                float closestDoorDistance = float.MaxValue;
                bool closestDoorVisible = false;
                int closestDoorIdx = -1;
                for (int i = 0; i < actionDoors.Length; i++)
                {
                    Vector3 doorCentre = GetDoorCenter(actionDoors[i]);
                    //Debug.LogFormat("Door at {0}, centre {1}  rot {2}", actionDoors[i].transform.position, doorCentre, actionDoors[i].transform.rotation);

                    bool visible = !Physics.Linecast(transform.position, doorCentre, out RaycastHit hitInfo);
                    float distance = Vector3.Distance(transform.position, doorCentre);

                    if (visible)
                        closestDoorVisible = true;

                    if (distance < closestDoorDistance && closestDoorVisible == visible)
                    {
                        closestDoorDistance = distance;
                        closestDoorIdx = i;
                    }
                }

                // Rotate model to look at the closest door position
                if (closestDoorIdx >= 0)
                {
                    Vector3 lookPos = GetDoorCenter(actionDoors[closestDoorIdx]);
                    lookPos.y = transform.position.y;
                    transform.LookAt(lookPos);
                }
            }
        }

        private static Vector3 GetDoorCenter(DaggerfallActionDoor actionDoor)
        {
            // Not sure why Vector3.right is correct here rather than forward, likely to do with DF door models
            return actionDoor.transform.position + (actionDoor.transform.rotation * Vector3.right * 0.6f);
        }
    }
}
