// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2019 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: TheLacus
// Contributors:
// 
// Notes:
//

// #define TEST_TRANSLATION

using UnityEngine;

namespace DaggerfallWorkshop.Utility.AssetInjection
{
    public enum Direction
    {
        None,
        Up,
        Down,
        Forward,
        Back,
        Left,
        Right
    }

    /// <summary>
    /// Moves an object next to the nearest collider. Can be used to fix bad classic game-data positions.
    /// </summary>
    public class ObjectPositioner : MonoBehaviour
    {
        const float maxDistance = 1;

#if TEST_TRANSLATION
        private Vector3 originalPosition;
#endif

        /// <summary>
        /// The direction in which the object is moved.
        /// </summary>
        [Tooltip("The direction in which the object is moved.")]
        public Direction Direction;

        private void Start()
        {
#if TEST_TRANSLATION
            originalPosition = transform.position;
#endif

            Move(GetDirection(Direction));
        }

#if TEST_TRANSLATION
        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(originalPosition, maxDistance);
            Gizmos.DrawLine(originalPosition, transform.position);
        }
#endif

        /// <summary>
        /// Moves the object next the nearest collider in the given direction.
        /// </summary>
        /// <param name="direction">A normalized direction in local space.</param>
        private void Move(Vector3 direction)
        {
            var meshRenderer = GetComponent<MeshRenderer>();
            if (!meshRenderer)
            {
                Debug.LogErrorFormat("ObjectPositioner: MeshRenderer is not found on {0}", name);
                return;
            }

            Vector3 worldSpaceDirection = transform.TransformDirection(direction);

            RaycastHit hitInfo;
            if (Physics.Raycast(new Ray(transform.position, worldSpaceDirection), out hitInfo, maxDistance))
            {
                float distance = Vector3.Distance(meshRenderer.bounds.ClosestPoint(hitInfo.point), hitInfo.point);
                transform.Translate(worldSpaceDirection * distance, Space.World);
            }
        }

        /// <summary>
        /// Converts a direction to the corresponding normalized vector.
        /// </summary>
        /// <param name="direction">A readable enum direction value.</param>
        /// <returns>A vector which is normalized if a correspondence is found, empty otherwise.</returns>
        private static Vector3 GetDirection(Direction direction)
        {
            return
                direction == Direction.Up ? Vector3.up :
                direction == Direction.Down ? Vector3.down :
                direction == Direction.Forward ? Vector3.forward :
                direction == Direction.Back ? Vector3.back :
                direction == Direction.Left ? Vector3.left :
                direction == Direction.Right ? Vector3.right :
                Vector3.zero;
        }
    }
}