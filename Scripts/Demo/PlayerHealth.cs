// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

using UnityEngine;
using System.Collections;

namespace DaggerfallWorkshop.Demo
{
    /// <summary>
    /// Example class to represent player health.
    /// </summary>
    [RequireComponent(typeof(ShowPlayerDamage))]
    public class PlayerHealth : MonoBehaviour
    {
        void Start()
        {
        }

        /// <summary>
        /// Player has been damaged.
        /// </summary>
        void RemoveHealth()
        {
            GetComponent<ShowPlayerDamage>().Flash();
        }

        /// <summary>
        /// Player has been damaged by a fall.
        /// </summary>
        /// <param name="fallDistance"></param>
        void ApplyPlayerFallDamage(float fallDistance)
        {
            GetComponent<ShowPlayerDamage>().Flash();
        }
    }
}