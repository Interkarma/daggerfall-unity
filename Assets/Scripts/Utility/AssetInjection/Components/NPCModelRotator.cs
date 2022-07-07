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
    /// Applies rotation for NPC model replacements.
    /// </summary>
    /// <remarks>
    /// This component does the following:
    /// Building Interiors - Ensures the object faces the nearest internal door, preferring visible.
    /// </remarks>
    public class NPCModelRotator : MonoBehaviour, IObjectPositioner
    {
        /// <summary>
        /// Return false if NPC model is inside a building interior to disable random rotation.
        /// </summary>
        public virtual bool AllowFlatRotation
        {
            get { return transform.parent.gameObject.name != DaggerfallInterior.peopleFlats; }
        }

        private void Start()
        {
            // If model in inside building interior, then apply rotation to model so it faces the nearest internal (action) door, preferring visible doors if there are any.
            GameObject interiorParent = GameManager.Instance.PlayerEnterExit.InteriorParent;
            if (interiorParent == transform.root.gameObject)
            {
                // Disable model colliders so raycasts cannot hit self
                Collider[] colliders = GetComponents<Collider>();
                for (int i = 0; i < colliders.Length; i++)
                    colliders[i].enabled = false;

                float closestDoorDistance = float.MaxValue;
                bool closestDoorVisible = false;
                int closestStaticDoorIdx = -1;
                int closestActionDoorIdx = -1;

                // Find the closest static door
                DaggerfallStaticDoors staticDoors = interiorParent.GetComponentInChildren<DaggerfallStaticDoors>();
                if (staticDoors.FindClosestDoorToPlayer(transform.position, -1, out Vector3 closestStaticDoorPos, out closestStaticDoorIdx, DoorTypes.Building))
                {
                    closestDoorDistance = Vector3.Distance(transform.position, closestStaticDoorPos);

                    bool visible = !Physics.Raycast(transform.position, closestStaticDoorPos - transform.position, closestDoorDistance - 0.5f);
                    if (visible)
                        closestDoorVisible = true;
                }

                // Find closest action door
                DaggerfallActionDoor[] actionDoors = interiorParent.GetComponentsInChildren<DaggerfallActionDoor>();
                for (int i = 0; i < actionDoors.Length; i++)
                {
                    Vector3 doorCentre = GetDoorCenter(actionDoors[i]);
                    //Debug.LogFormat("Door at {0}, centre {1}  rot {2}", actionDoors[i].transform.position, doorCentre, actionDoors[i].transform.rotation);

                    float distance = Vector3.Distance(transform.position, doorCentre);
                    bool visible = !Physics.Raycast(transform.position, doorCentre - transform.position, distance - 0.5f);

                    if (visible)
                    {
                        if (!closestDoorVisible)
                            closestDoorDistance = float.MaxValue;   // Reset distance if first visible door
                        closestDoorVisible = true;
                    }
                    if (distance < closestDoorDistance && closestDoorVisible == visible)
                    {
                        closestDoorDistance = distance;
                        closestActionDoorIdx = i;
                    }
                }

                // Rotate model to look at the closest door position
                if (closestActionDoorIdx >= 0)
                {
                    Vector3 lookPos = GetDoorCenter(actionDoors[closestActionDoorIdx]);
                    lookPos.y = transform.position.y;
                    transform.LookAt(lookPos);
                }
                else if (closestStaticDoorIdx >= 0)
                {
                    Vector3 lookPos = staticDoors.GetDoorPosition(closestStaticDoorIdx);
                    lookPos.y = transform.position.y;
                    transform.LookAt(lookPos);
                }

                // Re-enable model colliders
                for (int i = 0; i < colliders.Length; i++)
                    colliders[i].enabled = true;
            }
        }

        private static Vector3 GetDoorCenter(DaggerfallActionDoor actionDoor)
        {
            // Not sure why Vector3.right is correct here rather than forward, likely to do with DF door models
            return actionDoor.transform.position + (actionDoor.transform.rotation * Vector3.right * 0.6f);
        }
    }
}
