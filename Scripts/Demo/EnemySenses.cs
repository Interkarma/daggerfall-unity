// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

using UnityEngine;
using System.Collections;

namespace DaggerfallWorkshop.Demo
{
    /// <summary>
    /// Example enemy senses.
    /// </summary>
    public class EnemySenses : MonoBehaviour
    {
        public static Vector3 ResetPlayerPos = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

        public float SightRadius = 16f;         // Range of enemy sight
        public float HearingRadius = 8f;        // Range of enemy hearing
        public float FieldOfView = 120f;        // Enemy field of view

        GameObject player;
        DaggerfallMobileUnit mobile;
        bool playerInSight;
        bool playerInEarshot;
        Vector3 directionToPlayer;
        float distanceToPlayer;
        Vector3 lastKnownPlayerPos;
        DaggerfallActionDoor actionDoor;
        float distanceToActionDoor;

        public GameObject Player
        {
            get { return player; }
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

        void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            mobile = GetComponentInChildren<DaggerfallMobileUnit>();
            lastKnownPlayerPos = ResetPlayerPos;
        }

        void FixedUpdate()
        {
            if (player != null)
            {
                directionToPlayer = player.transform.position - transform.position;
                distanceToPlayer = directionToPlayer.magnitude;
                playerInSight = CanSeePlayer();
                playerInEarshot = CanHearPlayer();
            }
        }

        #region Private Methods

        private bool CanSeePlayer()
        {
            bool seen = false;
            actionDoor = null;
            distanceToActionDoor = float.MaxValue;

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
                        if (hit.transform.gameObject == player)
                        {
                            seen = true;
                            lastKnownPlayerPos = player.transform.position;
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
                if (hit.transform.gameObject != player && hit.transform.gameObject.isStatic)
                    hearingScale = 0.5f;
            }

            // TODO: Modify this by how much noise the player is making
            if (distanceToPlayer < (HearingRadius * hearingScale) + mobile.Summary.Enemy.HearingModifier)
            {
                heard = true;
                lastKnownPlayerPos = player.transform.position;
            }

            return heard;
        }

        #endregion
    }
}
