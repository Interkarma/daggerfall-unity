// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2023 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DaggerfallWorkshop
{
    /// <summary>
    /// Stores an array of buildings inside a block object.
    /// This provides the bridge between live scene data and game data.
    /// Used to hit-test buildings for identification and to locate in scene.
    /// </summary>
    public class DaggerfallStaticBuildings : MonoBehaviour
    {
        public StaticBuilding[] Buildings;          // Array of buildings attached to this block object

        void Start()
        {
            //// Debug trigger placement at start
            //for (int i = 0; i < Buildings.Length; i++)
            //{
            //    // Create debug game object
            //    GameObject go = new GameObject();
            //    go.transform.parent = transform;
            //    go.transform.position = Buildings[i].modelMatrix.GetColumn(3);
            //    go.transform.position += transform.position;
            //    go.transform.rotation = Quaternion.LookRotation(Buildings[i].modelMatrix.GetColumn(2), Buildings[i].modelMatrix.GetColumn(1));

            //    // Create debug bounding box
            //    // Building origin is at base, need to raise centre by half height
            //    BoxCollider c = go.AddComponent<BoxCollider>();
            //    c.center = new Vector3(0, Buildings[i].size.y / 2, 0);
            //    c.size = Buildings[i].size * 1.01f;
            //    c.isTrigger = true;
            //}
        }

        /// <summary>
        /// Check for a building hit in world space.
        /// </summary>
        /// <param name="point">Hit point from ray test in world space.</param>
        /// <param name="buildingOut">StaticBuilding out if hit found.</param>
        /// <returns>True if point hits a static building.</returns>
        public bool HasHit(Vector3 point, out StaticBuilding buildingOut)
        {
            buildingOut = new StaticBuilding();
            if (Buildings == null)
                return false;

            // Using a single hidden trigger created when testing buildings
            GameObject go = new GameObject();
            go.hideFlags = HideFlags.HideAndDontSave;
            go.transform.parent = transform;
            BoxCollider c = go.AddComponent<BoxCollider>();
            c.isTrigger = true;

            // Test each building in array
            bool found = false;
            for (int i = 0; i < Buildings.Length; i++)
            {
                // Setup single trigger position and size over each building in turn
                // Building origin is at base, need to raise centre by half height
                // Trigger size is slightly enlarged for containment test
                go.transform.position = Buildings[i].modelMatrix.GetColumn(3);
                go.transform.position += transform.position;
                go.transform.rotation = Quaternion.LookRotation(Buildings[i].modelMatrix.GetColumn(2), Buildings[i].modelMatrix.GetColumn(1));
                c.center = Buildings[i].centre;
                c.size = Buildings[i].size * 1.01f;

                // Check if hit was inside trigger
                if (c.bounds.Contains(point))
                {
                    found = true;
                    buildingOut = Buildings[i];
                    break;
                }
            }

            // Remove temp trigger
            if (go)
                Destroy(go);

            return found;
        }
    }
}
