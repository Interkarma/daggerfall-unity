// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2016 Daggerfall Workshop
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
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.UserInterfaceWindows;

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// Handles enemy death.
    /// </summary>
    public class EnemyDeath : MonoBehaviour
    {
        #region Fields

        public static System.EventHandler OnEnemyDeath;

        DaggerfallMobileUnit mobile;
        DaggerfallEntityBehaviour entityBehaviour;
        EnemyEntity enemyEntity;

        #endregion

        #region Unity

        void Awake()
        {
            mobile = GetComponentInChildren<DaggerfallMobileUnit>();
            entityBehaviour = GetComponent<DaggerfallEntityBehaviour>();
            entityBehaviour.OnSetEntity += EntityBehaviour_OnSetEntity;
        }

        #endregion

        #region Event Handlers

        private void EntityBehaviour_OnSetEntity(DaggerfallEntity oldEntity, DaggerfallEntity newEntity)
        {
            if (oldEntity != null)
            {
                oldEntity.OnDeath -= EnemyEntity_OnDeath;
            }

            if (newEntity != null)
            {
                enemyEntity = newEntity as EnemyEntity;
                enemyEntity.OnDeath += EnemyEntity_OnDeath; ;
            }
        }
        private void EnemyEntity_OnDeath(DaggerfallEntity entity)
        {
            // Disable enemy gameobject
            // Do not destroy as we must still save enemy state when dead
            gameObject.SetActive(false);

            // Show death message
            string deathMessage = HardStrings.thingJustDied;
            deathMessage = deathMessage.Replace("%s", mobile.Summary.Enemy.Name);
            DaggerfallUI.Instance.PopupMessage(deathMessage);

            // Generate lootable corpse marker
            DaggerfallLoot loot = GameObjectHelper.CreateLootableCorpseMarker(
                GameManager.Instance.PlayerObject,
                entityBehaviour.gameObject,
                mobile.Summary.Enemy.CorpseTexture,
                DaggerfallUnity.NextUID);

            // Raise static event
            if (OnEnemyDeath != null)
                OnEnemyDeath(this, null);
        }

        #endregion
    }
}