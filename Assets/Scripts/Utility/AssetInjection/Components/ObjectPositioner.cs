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
    /// Marks a component that interacts with the Asset-Injection framework to define object position.
    /// </summary>
    public interface IObjectPositioner
    {
        /// <summary>
        /// Defines if the object can be given a random rotation if it replaces a billboard.
        /// </summary>
        bool AllowFlatRotation { get; }
    }

    /// <summary>
    /// Moves an object next to the nearest collider. Can be used to fix bad classic game-data positions.
    /// </summary>
    [HelpURL("http://www.dfworkshop.net/projects/daggerfall-unity/modding/models-flats/#ObjectPositioner")]
    public class ObjectPositioner : MonoBehaviour, IObjectPositioner
    {
        protected const float maxDistance = 1;

#if TEST_TRANSLATION
        private Vector3 originalPosition;
#endif

        /// <summary>
        /// The direction in which the object is moved.
        /// </summary>
        [Tooltip("The direction in which the object is moved.")]
        public Direction Direction;

        /// <summary>
        /// The renderer that defines object bounds.
        /// </summary>
        [Tooltip("The renderer that defines object bounds.")]
        public Renderer Renderer;

        /// <summary>
        /// Always true because this component doesn't affect object rotation.
        /// </summary>
        public virtual bool AllowFlatRotation
        {
            get { return true; }
        }

        private void Start()
        {
            const int ignoreRaycastLayer = 2;

            if (!Renderer && !(Renderer = GetComponent<Renderer>()))
            {
                Debug.LogError("Renderer not found!", this);
                return;
            }

            Vector3 direction = GetDirection(Direction);
            if (direction == Vector3.zero)
            {
                Debug.LogError("Direction is not set!", this);
                return;
            }

#if TEST_TRANSLATION
            originalPosition = MeshRenderer.bounds.center;
#endif

            int layer = gameObject.layer;
            gameObject.layer = ignoreRaycastLayer;

            PerformPositioning(direction);

            gameObject.layer = layer;
        }

#if TEST_TRANSLATION
        private void OnDrawGizmos()
        {
            if (originalPosition != Vector3.zero)
            {
                Gizmos.DrawWireSphere(originalPosition, maxDistance);
                Gizmos.DrawLine(originalPosition, transform.position);
            }
        }
#endif

        /// <summary>
        /// Performs all positioning operations.
        /// </summary>
        /// <param name="direction">A normalized direction in local space.</param>
        protected virtual void PerformPositioning(Vector3 direction)
        {
            Move(direction);
        }

        /// <summary>
        /// Moves the object next the nearest collider in the given direction or away from it if clipping.
        /// </summary>
        /// <param name="direction">A normalized direction in local space.</param>
        protected void Move(Vector3 direction)
        {
            Bounds bounds = Renderer.bounds;
            Ray ray = new Ray(bounds.center, transform.TransformDirection(direction));

            RaycastHit hitInfo;
            if (!Physics.Raycast(ray, out hitInfo, maxDistance))
                return;

            // Get the bound extent on the direction of the ray axis
            Vector3 extents = bounds.extents;
            extents.Scale((bounds.center - hitInfo.point).normalized);

            // Move the object for the distance between collision and bound point
            transform.Translate(hitInfo.point - (bounds.center - extents), Space.World);
        }

#if UNITY_EDITOR
        private void Reset()
        {
            Direction = Direction.None;
            Renderer = GetComponent<Renderer>();
        }
#endif

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