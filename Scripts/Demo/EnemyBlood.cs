// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

using UnityEngine;
using System.Collections;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop.Demo
{
    /// <summary>
    /// Example enemy blood effect.
    /// </summary>
    public class EnemyBlood : MonoBehaviour
    {
        const int bloodArchive = 380;

        public void ShowBloodSplash(int bloodIndex, Vector3 bloodPosition)
        {
            // Create oneshot animated billboard for blood effect
            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;
            if (dfUnity)
            {
                GameObject go = GameObjectHelper.CreateDaggerfallBillboardGameObject(bloodArchive, bloodIndex, null, true);
                go.name = "BloodSplash";
                DaggerfallBillboard c = go.GetComponent<DaggerfallBillboard>();
                go.transform.position = bloodPosition + transform.forward * 0.02f;
                c.OneShot = true;
                c.FramesPerSecond = 10;
            }
        }
    }
}
