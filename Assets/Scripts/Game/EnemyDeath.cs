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
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Game.Questing;

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

        bool performDeath = false;

        #endregion

        #region Unity

        void Awake()
        {
            mobile = GetComponentInChildren<DaggerfallMobileUnit>();
            entityBehaviour = GetComponent<DaggerfallEntityBehaviour>();
            entityBehaviour.OnSetEntity += EntityBehaviour_OnSetEntity;
        }

        private void Update()
        {
            if (performDeath)
                CompleteDeath();
        }

        #endregion

        #region Private Methods

        void CompleteDeath()
        {
            if (!entityBehaviour)
                return;

            // If enemy associated with quest system, make sure quest system is done with it first
            QuestResourceBehaviour questResourceBehaviour = GetComponent<QuestResourceBehaviour>();
            if (questResourceBehaviour)
            {
                if (!questResourceBehaviour.IsFoeDead)
                    return;
            }

            // Play body collapse sound
            if (DaggerfallUI.Instance.DaggerfallAudioSource)
            {
                AudioClip collapseSound = DaggerfallUI.Instance.DaggerfallAudioSource.GetAudioClip((int)SoundClips.BodyFall);
                AudioSource.PlayClipAtPoint(collapseSound, entityBehaviour.transform.position, 1.05f);
            }

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
                enemyEntity,
                mobile.Summary.Enemy.CorpseTexture,
                DaggerfallUnity.NextUID);

            // This is still required so enemy equipment is not marked as equipped
            // This item collection is transferred to loot container below
            for (int i = (int)Items.EquipSlots.Head; i <= (int)Items.EquipSlots.Feet; i++)
            {
                Items.DaggerfallUnityItem item = enemyEntity.ItemEquipTable.GetItem((Items.EquipSlots)i);
                if (item != null)
                {
                    enemyEntity.ItemEquipTable.UnequipItem((Items.EquipSlots)i);
                }
            }

            entityBehaviour.CorpseLootContainer = loot;

            // Transfer any items owned by entity to loot container
            // Many quests will stash a reward in enemy inventory for player to find
            // This will be in addition to normal random loot table generation
            loot.Items.TransferAll(entityBehaviour.Entity.Items);

            // Raise static event
            if (OnEnemyDeath != null)
                OnEnemyDeath(this, null);
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
                enemyEntity.OnDeath += EnemyEntity_OnDeath;
            }
        }

        private void EnemyEntity_OnDeath(DaggerfallEntity entity)
        {
            // Set flag to perform OnDeath tasks
            // It make take a few ticks for enemy to actually die if owned by quest system
            // because some other processing might need to be done in quest (like placing an item)
            // before this enemy can be deactivated and loot container dropped
            performDeath = true;
        }

        #endregion
    }
}