// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2016 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using System.Collections;

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// Example enemy senses.
    /// </summary>
    public class EnemySenses : MonoBehaviour
    {
        public static readonly Vector3 ResetPlayerPos = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

        public float SightRadius = 18f;         // Range of enemy sight
        public float HearingRadius = 10f;       // Range of enemy hearing
        public float FieldOfView = 140f;        // Enemy field of view

        DaggerfallMobileUnit mobile;
        bool playerInSight;
        bool playerInEarshot;
        Vector3 directionToPlayer;
        float distanceToPlayer;
        Vector3 lastKnownPlayerPos;
        DaggerfallActionDoor actionDoor;
        float distanceToActionDoor;
        bool hasEncounteredPlayer = false;

        GameObject Player
        {
            get { return GameManager.Instance.PlayerObject; }
        }

        public bool PlayerInSight
        {
            get { return playerInSight; }
        }

        public bool PlayerInEarshot
        {
            get { return playerInEarshot; }
        }

        public Vector3 DirectionToPlayer
        {
            get { return directionToPlayer; }
        }

        public float DistanceToPlayer
        {
            get { return distanceToPlayer; }
        }

        public Vector3 LastKnownPlayerPos
        {
            get { return lastKnownPlayerPos; }
            set { lastKnownPlayerPos = value; }
        }

        public DaggerfallActionDoor LastKnownDoor
        {
            get { return actionDoor; }
        }

        public float DistanceToDoor
        {
            get { return distanceToActionDoor; }
        }

        public bool HasEncounteredPlayer
        {
            get { return hasEncounteredPlayer; }
            set { hasEncounteredPlayer = value; }
        }

        void Start()
        {
            mobile = GetComponentInChildren<DaggerfallMobileUnit>();
            lastKnownPlayerPos = ResetPlayerPos;
        }

        void FixedUpdate()
        {
            if (Player != null)
            {
                Vector3 toPlayer = Player.transform.position - transform.position;
                directionToPlayer = toPlayer.normalized;
                distanceToPlayer = toPlayer.magnitude;

                playerInSight = CanSeePlayer();
                playerInEarshot = CanHearPlayer();

                if((playerInEarshot || playerInSight) && !hasEncounteredPlayer)
                    hasEncounteredPlayer = true;
            }
        }

        #region Private Methods

        private bool CanSeePlayer()
        {
            bool seen = false;
            actionDoor = null;

            if (distanceToPlayer < SightRadius + mobile.Summary.Enemy.SightModifier)
            {
                // Check if player in field of view
                float angle = Vector3.Angle(directionToPlayer, transform.forward);
                if (angle < FieldOfView * 0.5f)
                {
                    // Check if line of sight to player
                    RaycastHit hit;
                    Ray ray = new Ray(transform.position, directionToPlayer);
                    if (Physics.Raycast(ray, out hit, SightRadius))
                    {
                        // Check if hit was player
                        if (hit.transform.gameObject == Player)
                        {
                            seen = true;
                            lastKnownPlayerPos = Player.transform.position;
                        }

                        // Check if hit was an action door
                        DaggerfallActionDoor door = hit.transform.gameObject.GetComponent<DaggerfallActionDoor>();
                        if (door != null)
                        {
                            if (!door.IsLocked && !door.IsMagicallyHeld)
                            {
                                actionDoor = door;
                                distanceToActionDoor = Vector3.Distance(transform.position, actionDoor.transform.position);
                            }
                        }
                    }
                }
            }

            return seen;
        }

        private bool CanHearPlayer()
        {
            bool heard = false;
            float hearingScale = 1f;

            // If something is between enemy and player then reduce hearing radius by half
            // Hearing is not impeded by doors or other non-static objects
            RaycastHit hit;
            Ray ray = new Ray(transform.position, directionToPlayer);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject != Player && hit.transform.gameObject.isStatic)
                    hearingScale = 0.5f;
            }

            // TODO: Modify this by how much noise the player is making
            if (distanceToPlayer < (HearingRadius * hearingScale) + mobile.Summary.Enemy.HearingModifier)
            {
                heard = true;
                lastKnownPlayerPos = Player.transform.position;
            }

            return heard;
        }

        #endregion
    }
}
