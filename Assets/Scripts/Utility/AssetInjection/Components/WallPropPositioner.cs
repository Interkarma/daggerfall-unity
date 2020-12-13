// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2020 Daggerfall Workshop
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
    /// Finds an appropriate position and rotation for objects that replace billboard wall props such as torches.
    /// </summary>
    /// <remarks>
    /// This component performs three operations:
    /// 1. Ensures the object faces the wall on the set direction and rotates if needed.
    /// 2. Moves the object next to the wall or away from it if clipping.
    /// 3. Aligns the object if wall is not perpendicular to the floor.
    /// </remarks>
    [HelpURL("http://www.dfworkshop.net/projects/daggerfall-unity/modding/models-flats/#WallPropPositioner")]
    public class WallPropPositioner : ObjectPositioner
    {
        /// <summary>
        /// Always false because rotation is set for wall direction.
        /// </summary>
        public sealed override bool AllowFlatRotation
        {
            get { return false; }
        }

        /// <summary>
        /// Performs all positioning operations.
        /// </summary>
        /// <param name="direction">A normalized direction in local space.</param>
        protected override void PerformPositioning(Vector3 direction)
        {
            Rotate(direction);
            Move(direction);
            Align(direction);
        }

        /// <summary>
        /// Rotates the object from the given direction to a collider found next to it.
        /// </summary>
        /// <param name="from">A normalized direction in local space.</param>
        protected void Rotate(Vector3 from)
        {
            Vector3? to = null;

            if (HitWall(Vector3.forward))
                to = Vector3.forward;
            else if (HitWall(Vector3.left))
                to = Vector3.left;
            else if (HitWall(Vector3.right))
                to = Vector3.right;
            else if (HitWall(Vector3.back))
                to = Vector3.back;

            if (to.HasValue)
                transform.Rotate(Vector3.up, Vector3.SignedAngle(from, to.Value, Vector3.up));
        }

        /// <summary>
        /// Calculates the angle with the wall and apply a corresponding rotation.
        /// </summary>
        /// <param name="direction">A normalized direction in local space.</param>
        protected void Align(Vector3 direction)
        {
            Bounds bounds = Renderer.bounds;
            Vector3 worldSpaceDirection = transform.TransformDirection(direction);

            // Bottom point on bounds
            Vector3 bottomPoint = gameObject.transform.position;
            bottomPoint.y = bounds.min.y;
            RaycastHit hitInfoBottom;
            if (!Physics.Raycast(new Ray(bottomPoint, worldSpaceDirection), out hitInfoBottom, maxDistance))
                return;

            // Top point on bounds
            Vector3 topPoint = gameObject.transform.position;
            topPoint.y = bounds.max.y;
            RaycastHit hitInfoTop;
            if (!Physics.Raycast(new Ray(topPoint, worldSpaceDirection), out hitInfoTop, maxDistance))
                return;

            // Calculate delta
            float delta = hitInfoBottom.distance - hitInfoTop.distance;
            if (Mathf.Approximately(delta, 0))
                return;

            // Apply rotation
            Vector3 axis = Vector3.Cross(worldSpaceDirection, Vector3.up);
            float angle = 90f - (Mathf.Atan2(topPoint.y - bottomPoint.y, delta) * Mathf.Rad2Deg);
            transform.Rotate(axis, angle, Space.World); 
        }

        /// <summary>
        /// Checks if there is a collision with a wall in the given direction.
        /// </summary>
        /// <param name="direction">A direction in local space.</param>
        /// <returns>True if wall is found.</returns>
        private bool HitWall(Vector3 direction)
        {
            Vector3 worldSpaceDirection = transform.TransformDirection(direction);
            return Physics.Raycast(gameObject.transform.position, worldSpaceDirection, maxDistance);
        }
    }
}
