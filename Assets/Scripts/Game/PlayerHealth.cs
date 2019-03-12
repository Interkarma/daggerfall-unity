// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2019 Daggerfall Workshop
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
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// Temporary class to remove player health on damage.
    /// </summary>
    [RequireComponent(typeof(ShowPlayerDamage))]
    public class PlayerHealth : MonoBehaviour
    {
        //public bool GodMode = false;

        DaggerfallEntityBehaviour entityBehaviour;

        void Awake()
        {
            entityBehaviour = GetComponent<DaggerfallEntityBehaviour>();
        }

        /// <summary>
        /// Player has been damaged.
        /// </summary>
        void RemoveHealth(int amount)
        {
            GetComponent<ShowPlayerDamage>().Flash();
            if (entityBehaviour)
            {
                PlayerEntity entity = entityBehaviour.Entity as PlayerEntity;
                entity.DecreaseHealth(amount);
            }
        }

        /// <summary>
        /// Player has been damaged by a fall.
        /// </summary>
        void ApplyPlayerFallDamage(float fallDistance)
        {
            const float threshold = 10f;
            const float percentPerMetre = 50.0f / 100f;

            if (entityBehaviour)
            {
                // Remove percent of max health for every metre over threshold
                PlayerEntity entity = entityBehaviour.Entity as PlayerEntity;
                int unit = (int)(entity.MaxHealth * percentPerMetre);
                int damage = unit * (int)(fallDistance - threshold);
                RemoveHealth(damage);
            }
        }
    }
}