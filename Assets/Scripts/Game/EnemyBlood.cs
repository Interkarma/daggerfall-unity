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
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// Example enemy blood effect.
    /// </summary>
    public class EnemyBlood : MonoBehaviour
    {
        const int bloodArchive = 380;
        const int sparklesIndex = 3;

        public void ShowBloodSplash(int bloodIndex, Vector3 bloodPosition)
        {
            // Create oneshot animated billboard for blood effect
            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;
            if (dfUnity)
            {
                GameObject go = GameObjectHelper.CreateDaggerfallBillboardGameObject(bloodArchive, bloodIndex, null);
                go.name = "BloodSplash";
                Billboard c = go.GetComponent<Billboard>();
                go.transform.position = bloodPosition + transform.forward * 0.02f;
                c.OneShot = true;
                c.FramesPerSecond = 10;
            }
        }

        public void ShowMagicSparkles(Vector3 sparklesPosition)
        {
            // Create oneshot animated billboard for sparkles effect
            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;
            if (dfUnity)
            {
                GameObject go = GameObjectHelper.CreateDaggerfallBillboardGameObject(bloodArchive, sparklesIndex, null);
                go.name = "MagicSparkles";
                Billboard c = go.GetComponent<Billboard>();
                go.transform.position = sparklesPosition + transform.forward * 0.02f;
                c.OneShot = true;
                c.FramesPerSecond = 10;
            }
        }
    }
}
