// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: TheLacus
// Contributors:
// 
// Notes:
//

using UnityEngine;

namespace DaggerfallWorkshop.Utility.AssetInjection
{
    /// <summary>
    /// Rotate a GameObject so that it appears hung on the wall on its Z axis.
    /// This is useful for models meant to replace sprites as they miss the rotation component.
    /// </summary>
    [HelpURL("http://www.dfworkshop.net/projects/daggerfall-unity/modding/models-flats/#facewall")]
    public class FaceWall : MonoBehaviour
    {
        #region Fields

        [Tooltip("The wall will be seeked inside this radius. Big values can cause wrong results.")]
        public float maxDistance = 1.1f;

        [Space()]

        [Tooltip("Make the GameObject parallel to non-straight walls.")]
        public bool alignToWall = true;

        [Tooltip("Angles lower than this will be ignored.")]
        public float minorAngleToIgnore = 0.2f;

        [Tooltip("The actual rotation is corrected by this value to take small inaccuracies into account.")]
        public float rotationCorrection = -7f;

        [Space()]

        [Tooltip("Translate the GameObject so that it touches the wall.")]
        public bool moveNearWall = true;

        [Tooltip("Fix issue where GameObject clips with the wall if the origin is next to it.")]
        public bool fixClipping = true;

        [Tooltip("Remove the GameObject if it can't be positioned correctly.")]
        public bool destroyOnBadPosition = false;

        #endregion

        #region Unity

        void Start()
        {
            // Get current position and rotate accordingly
            if (!HitWall(Vector3.forward))
            {
                if (HitWall(Vector3.left))
                    transform.Rotate(Vector3.up, -90f);
                else if (HitWall(Vector3.right))
                    transform.Rotate(Vector3.up, 90f);
                else if (HitWall(Vector3.back))
                    transform.Rotate(Vector3.up, 180f);
            }

            // Align to wall
            if (alignToWall)
                AlignToWall();

            // Move near wall (and fix clipping)
            if (moveNearWall)
                MoveNearWall();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Cast ray along direction to find the wall
        /// </summary>
        /// <param name="direction">Direction to look for the wall</param>
        /// <returns>True if wall is found</returns>
        private bool HitWall(Vector3 direction)
        {
            // Transforms direction from local space to world space.
            direction = transform.TransformDirection(direction);

            // Casts a ray from the GameObject and check if it intersect 
            // with a collider in the specified direction.
            return Physics.Raycast(gameObject.transform.position, direction, maxDistance);
        }

        /// <summary>
        /// Make the GameObject parallel to the wall.
        /// This is useful for non-straight dungeons walls.
        /// </summary>
        private void AlignToWall()
        {
            var bounds = GetComponent<Renderer>().bounds;

            // Bottom point on bounds
            Vector3 bottomPoint = gameObject.transform.position;
            bottomPoint.y = bounds.min.y;
            Ray lowerRay = new Ray(bottomPoint, transform.forward);
            RaycastHit hitInfoBottom;
            if (!Physics.Raycast(lowerRay, out hitInfoBottom, maxDistance))
                return;

            // Top point on bounds
            Vector3 topPoint = gameObject.transform.position;
            topPoint.y = bounds.max.y;
            Ray higherRay = new Ray(topPoint, transform.forward);
            RaycastHit hitInfoTop;
            if (!Physics.Raycast(higherRay, out hitInfoTop, maxDistance))
                return;

            // Get delta between top-wall and bottom-wall
            float delta = hitInfoBottom.distance - hitInfoTop.distance;
            if (delta < minorAngleToIgnore) // Do not correct invisible angles or false positives
                return;

            // Get angle and apply rotation
            float angle = 90f - (Mathf.Atan2(topPoint.y - bottomPoint.y, delta) * Mathf.Rad2Deg);
            angle += rotationCorrection; // Apply a rotation slightly smaller than calculated
            transform.Rotate(Vector3.left, angle);
        }

        /// <summary>
        /// Translate the GameObject so that it touches the wall
        /// </summary>
        private void MoveNearWall()
        {
            var renderer = GetComponent<Renderer>();

            if (fixClipping)
            {
                // Check if wall is inside bounds
                float extent = renderer.bounds.extents.z;
                if (Physics.Raycast(renderer.bounds.center, transform.forward, extent))
                {
                    // Move GameObject away from the wall by half the lenght of z
                    transform.Translate(0, 0, -extent);
                    maxDistance += extent;
                }
            }

            // Find point on wall
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, maxDistance))
            {
                // Find nearest point on GameObject
                Vector3 pointOnGameObject = renderer.bounds.ClosestPoint(hitInfo.point);

                // Translate GameObject near wall
                float distance = Vector3.Distance(pointOnGameObject, hitInfo.point);
                transform.Translate(0, 0, distance);
            }
            else if (destroyOnBadPosition)
            {
                Debug.Log(string.Format("GameObject {0} was in a bad position and has been removed by FaceWall.", name));
                Destroy(gameObject);
            }
        }

        #endregion
    }
}