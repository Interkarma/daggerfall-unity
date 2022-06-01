// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: TheLacus
// Contributors:
// 
// Notes:
//

using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace DaggerfallWorkshop.Utility.AssetInjection
{
    /// <summary>
    /// Finds the best position and rotation for a gameobject meant to replace a billboard.
    /// </summary>
    /// <remarks>
    /// This component performs three operations:
    /// 1. Ensures the object faces the wall on the object Z axis and rotates if needed.
    /// 2. Moves the object next to the wall or away from it if clipping.
    /// 3. Aligns the object if wall is not perpendicular.
    /// </remarks>
    [Obsolete("Use WallPropPositioner")]
    [HelpURL("http://www.dfworkshop.net/projects/daggerfall-unity/modding/models-flats/#facewall")]
    public class FaceWall : MonoBehaviour, IObjectPositioner
    {
        #region Fields

        MeshRenderer meshRenderer;

        /// <summary>
        /// The wall will be seeked inside this radius. Big values can cause wrong results.
        /// </summary>
        [Tooltip("The wall will be seeked inside this radius. Big values can cause wrong results.")]
        [FormerlySerializedAs("maxDistance")]
        public float MaxDistance = 1.1f;

        /// <summary>
        /// Translates the object next to the wall if distant or away from the wall if clipping.
        /// </summary>
        [Tooltip("Translates the object next to the wall if distant or away from the wall if clipping.")]
        [FormerlySerializedAs("moveNearWall")]
        public bool MoveNearWall = true;

        /// <summary>
        /// Aligns the object if wall is not perpendicular.
        /// </summary>
        [Tooltip("Aligns the object if wall is not perpendicular.")]
        [FormerlySerializedAs("alignToWall")]
        public bool AlignToWall = true;

        public bool AllowFlatRotation
        {
            get { return false; }
        }

        #endregion

        #region Unity

        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            if (!meshRenderer)
                Debug.LogErrorFormat("FaceWall: MeshRenderer not found on {0}.", name);
        }

        private void Start()
        {
            if (!meshRenderer)
                return;

            if (HitWall(Vector3.left))
                transform.Rotate(Vector3.up, -90f);
            else if (HitWall(Vector3.right))
                transform.Rotate(Vector3.up, 90f);
            else if (HitWall(Vector3.back))
                transform.Rotate(Vector3.up, 180f);

            if (MoveNearWall)
                Move();

            if (AlignToWall)
                Align();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Checks if there is a collision with a wall in the given direction.
        /// </summary>
        /// <param name="direction">A direction in local space.</param>
        /// <returns>True if wall is found.</returns>
        private bool HitWall(Vector3 direction)
        {
            Vector3 worldSpaceDirection = transform.TransformDirection(direction);
            return Physics.Raycast(gameObject.transform.position, worldSpaceDirection, MaxDistance);
        }

        /// <summary>
        /// Translates the object next to the wall if distant or away from the wall if clipping.
        /// </summary>
        private void Move()
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(new Ray(transform.position, transform.forward), out hitInfo, MaxDistance))
            {
                Bounds bounds = meshRenderer.bounds;

                // Get the bound extent on the direction of the ray axis
                Vector3 extents = bounds.extents;
                extents.Scale((bounds.center - hitInfo.point).normalized);

                // Move the object for the distance between collision and bound point
                transform.Translate(hitInfo.point - (bounds.center - extents), Space.World);
            }
        }

        /// <summary>
        /// Calculates the angle with the wall and apply a corresponding rotation.
        /// </summary>
        private void Align()
        {
            Bounds bounds = meshRenderer.bounds;

            // Bottom point on bounds
            Vector3 bottomPoint = gameObject.transform.position;
            bottomPoint.y = bounds.min.y;
            RaycastHit hitInfoBottom;
            if (!Physics.Raycast(new Ray(bottomPoint, transform.forward), out hitInfoBottom, MaxDistance))
                return;

            // Top point on bounds
            Vector3 topPoint = gameObject.transform.position;
            topPoint.y = bounds.max.y;
            RaycastHit hitInfoTop;
            if (!Physics.Raycast(new Ray(topPoint, transform.forward), out hitInfoTop, MaxDistance))
                return;

            // Calculate angle and rotate
            float delta = hitInfoBottom.distance - hitInfoTop.distance;
            if (!Mathf.Approximately(delta, 0))
                transform.Rotate(Vector3.left, 90f - (Mathf.Atan2(topPoint.y - bottomPoint.y, delta) * Mathf.Rad2Deg));
        }

        #endregion
    }
}