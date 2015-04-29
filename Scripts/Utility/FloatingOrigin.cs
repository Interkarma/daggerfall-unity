// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using System;

namespace DaggerfallWorkshop.Utility
{
    /// <summary>
    /// Moves player and world back near origin after a certain threshold distance reached.
    /// Must be attached to Player body parent GameObject that is peered to player controller.
    /// Call Initialize() whenever you move the player manually (e.g. teleport).
    /// </summary>
    public class FloatingOrigin : MonoBehaviour
    {
        const float threshold = 1000f;

        // Must specify streaming world component
        public StreamingWorld StreamingWorld;

        // The children of each additional parent will be moved
        public GameObject[] OtherParents;

        Vector3 lastPlayerPos;

        void Start()
        {
            Initialize();
        }

        void FixedUpdate()
        {
            // Must have streaming world reference
            if (!StreamingWorld)
                return;

            // Do nothing during streaming world init
            if (StreamingWorld.IsInit)
                return;

            // Check if player movements greater than threshold magnitude from start position
            Vector3 playerPos = GetXZPosition();
            Vector3 change = lastPlayerPos - playerPos;
            if (change.magnitude > threshold)
            {
                // Offset streaming world
                OffsetChildren(StreamingWorld.gameObject, change);
                StreamingWorld.OffsetWorldCompensation(change);

                // Offset children of specified parent game objects
                // This gives more control than just moving everything in world
                if (OtherParents != null)
                {
                    for (int i = 0; i < OtherParents.Length; i++)
                    {
                        OffsetChildren(OtherParents[i], change);
                    }
                }

                // Offset player
                OffsetPlayerController(change);

                // Update last position
                lastPlayerPos = GetXZPosition();
            }
        }

        public void Initialize()
        {
            lastPlayerPos = GetXZPosition();
        }

        void OffsetChildren(GameObject go, Vector3 offset)
        {
            foreach (Transform transform in go.transform)
            {
                transform.position += offset;
            }

            // TODO: Offset particles from particle emitters

            // TODO: Offset physic properties
        }

        void OffsetPlayerController(Vector3 offset)
        {
            // Clear moving platform from player controller to prevent player moving improperly with world
            // You should implement ClearActivePlatform() or similar when using your own controller
            SendMessage("ClearActivePlatform", SendMessageOptions.DontRequireReceiver);

            // Offset player
            transform.position += offset;
        }

        Vector3 GetXZPosition()
        {
            return new Vector3(transform.position.x, 0, transform.position.z);
        }
    }
}