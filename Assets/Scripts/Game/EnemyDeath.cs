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

        MobileUnit mobile;
        DaggerfallEntityBehaviour entityBehaviour;
        EnemyEntity enemyEntity;

        bool performDeath = false;

        #endregion

        #region Unity

        void Awake()
        {
            entityBehaviour = GetComponent<DaggerfallEntityBehaviour>();
            entityBehaviour.OnSetEntity += EntityBehaviour_OnSetEntity;
        }

        private void Start()
        {
            mobile = GetComponent<DaggerfallEnemy>().MobileUnit;
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

            // Disable enemy gameobject
            // Do not destroy as we must still save enemy state when dead
            gameObject.SetActive(false);

            // Show death message
            string deathMessage = TextManager.Instance.GetLocalizedText("thingJustDied");
            deathMessage = deathMessage.Replace("%s", TextManager.Instance.GetLocalizedEnemyName(mobile.Enemy.ID));
            if (!DaggerfallUnity.Settings.DisableEnemyDeathAlert)
                DaggerfallUI.Instance.PopupMessage(deathMessage);

            int corpseTexture;
            if(mobile.Enemy.Gender == MobileGender.Female && mobile.Enemy.FemaleCorpseTexture != 0)
            {
                corpseTexture = mobile.Enemy.FemaleCorpseTexture;
            }
            else
            {
                corpseTexture = mobile.Enemy.CorpseTexture;
            }

            // Generate lootable corpse marker
            DaggerfallLoot loot = GameObjectHelper.CreateLootableCorpseMarker(
                GameManager.Instance.PlayerObject,
                entityBehaviour.gameObject,
                enemyEntity,
                corpseTexture,
                DaggerfallUnity.NextUID);

            // Tag corpse loot marker with quest UID
            if (questResourceBehaviour)
                loot.corpseQuestUID = questResourceBehaviour.QuestUID;

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

            // Play body collapse sound
            if (DaggerfallUI.Instance.DaggerfallAudioSource)
            {
                DaggerfallUI.Instance.DaggerfallAudioSource.PlayClipAtPoint(SoundClips.BodyFall, loot.transform.position, 1f);
            }

            // Lower enemy alert state on player now that enemy is dead
            // If this is final enemy targeting player then alert state will remain clear
            // Other enemies still targeting player will continue to raise alert state every update
            EnemySenses senses = entityBehaviour.GetComponent<EnemySenses>();
            if (senses && senses.Target == GameManager.Instance.PlayerEntityBehaviour)
                GameManager.Instance.PlayerEntity.SetEnemyAlert(false);

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