// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
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
    /// Disables dungeon lights when out of range.
    /// A dungeon block has a 2048x2048 DF unit footprint.
    /// This means lights just outside this range can be safely disabled,
    /// returning performance for more lighting effects such as shadows.
    /// </summary>
    public class DungeonLightHandler : MonoBehaviour
    {
        public float UnscaledBlockRange = 2060;
        public float UpdateInSeconds = 0.4f;

        GameObject player;
        Light myLight;
        float timer = 0;

        void Start()
        {
            // Get components
            myLight = GetComponent<Light>();
            player = GameManager.Instance.PlayerObject;

            // Disable dungeon light shadows
            if (!DaggerfallUnity.Settings.DungeonLightShadows)
                myLight.shadows = LightShadows.None;
        }

        void Update()
        {
            timer += Time.deltaTime;
            if (timer > UpdateInSeconds)
            {
                CheckLight();
                timer = 0;
            }
        }

        void CheckLight()
        {   
            // Must have both player and light set or exit
            if (!player || !myLight)
                return;

            // Block range must be scaled to suit mesh size at import time
            float scaledRange = UnscaledBlockRange * MeshReader.GlobalScale;

            // Comparing XZ distance only as dungeon blocks have no defined vertical height
            Vector3 lightXZ = new Vector3(myLight.transform.position.x, 0, myLight.transform.position.z);
            Vector3 playerXZ = new Vector3(player.transform.position.x, 0, player.transform.position.z);

            // Check distance
            if (Vector3.Distance(lightXZ, playerXZ) > scaledRange)
            {
                myLight.enabled = false;
            }
            else
            {
                myLight.enabled = true;
            }
        }
    }
}