// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes: All additions or modifications that differ from the source code copyright (c) 2021-2022 Osorkon
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
            // [OSORKON] Fall damage doubled, starts at lower heights. Falling 2 blocks is 25 damage. Damage then increases
            // by 10 per metre fallen.
            const float threshold = 3.8f;
            const float HPPerMetre = 10f;

            if (entityBehaviour)
            {
                int damage = (int)(HPPerMetre * (fallDistance - threshold));
                RemoveHealth(damage);
            }
        }
    }
}