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
using System.Collections;

namespace DaggerfallWorkshop.Game
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