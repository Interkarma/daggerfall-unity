// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using DaggerfallWorkshop.Game.MagicAndEffects;

namespace DaggerfallWorkshop.Game.Entity
{
    /// <summary>
    /// Hosts DaggerfallEntity for living GameObjects.
    /// </summary>
    public class DaggerfallEntityBehaviour : MonoBehaviour
    {
        #region Fields

        public EntityTypes EntityType = EntityTypes.None;

        EntityTypes lastEntityType = EntityTypes.None;
        DaggerfallEntity entity = null;
        DaggerfallLoot corpseLootContainer = null;

        #endregion

        #region Properties

        /// <summary>
        /// Gets entity as PlayerEntity.
        /// </summary>
        public DaggerfallEntity Entity
        {
            get { return entity; }
            set { SetEntityValue(value); }
        }

        /// <summary>
        /// Gets or sets reference to loot container spawned at time of entity death.
        /// </summary>
        public DaggerfallLoot CorpseLootContainer
        {
            get { return corpseLootContainer; }
            set { corpseLootContainer = value; }
        }

        #endregion

        #region Unity

        private void Awake()
        {
            SetEntityType(EntityType);
        }

        void Update()
        {
            // Change entity type
            if (EntityType != lastEntityType)
            {
                SetEntityType(EntityType);
                lastEntityType = EntityType;
            }

            // Exit when no entity set
            if (Entity == null)
                return;

            // Update entity
            Entity.Update(this);
        }

        #endregion

        #region Special Damage Methods

        /// <summary>
        /// Cause fatigue damage to entity with additional logic.
        /// </summary>
        /// <param name="sourceEffect">Source effect.</param>
        /// <param name="amount">Amount to damage fatigue.</param>
        /// <param name="assignMultiplier">Optionally assign fatigue multiplier.</param>
        public void DamageFatigueFromSource(IEntityEffect sourceEffect, int amount, bool assignMultiplier = false)
        {
            DamageFatigueFromSource(sourceEffect.Caster, amount, assignMultiplier);
        }

        /// <summary>
        /// Cause damage to entity health with additional logic.
        /// </summary>
        /// <param name="sourceEffect">Source effect.</param>
        /// <param name="amount">Amount to damage health.</param>
        /// <param name="showBlood">Show blood splash.</param>
        /// <param name="bloodPosition">Blood splash position.</param>
        public void DamageHealthFromSource(IEntityEffect sourceEffect, int amount, bool showBlood, Vector3 bloodPosition)
        {
            DamageHealthFromSource(sourceEffect.Caster, amount, showBlood, bloodPosition);
        }

        /// <summary>
        /// Cause spell point damage to entity with additional logic.
        /// </summary>
        /// <param name="sourceEffect">Source effect.</param>
        /// <param name="amount">Amount to damage spell points.</param>
        public void DamageMagickaFromSource(IEntityEffect sourceEffect, int amount)
        {
            DamageMagickaFromSource(sourceEffect.Caster, amount);
        }

        /// <summary>
        /// Cause fatigue damage to entity with additional logic.
        /// </summary>
        /// <param name="source">Source entity behaviour.</param>
        /// <param name="amount">Amount to damage fatigue.</param>
        /// <param name="assignMultiplier">Optionally assign fatigue multiplier.</param>
        public void DamageFatigueFromSource(DaggerfallEntityBehaviour sourceEntityBehaviour, int amount, bool assignMultiplier = false)
        {
            // Remove fatigue amount
            Entity.DecreaseFatigue(amount, assignMultiplier);

            // Post-attack logic on source
            HandleAttackFromSource(sourceEntityBehaviour);
        }

        /// <summary>
        /// Cause damage to entity health with additional logic.
        /// </summary>
        /// <param name="source">Source entity behaviour.</param>
        /// <param name="amount">Amount to damage health.</param>
        /// <param name="showBlood">Show blood splash.</param>
        /// <param name="bloodPosition">Blood splash position.</param>
        public void DamageHealthFromSource(DaggerfallEntityBehaviour sourceEntityBehaviour, int amount, bool showBlood, Vector3 bloodPosition)
        {
            // Remove health amount
            Entity.DecreaseHealth(amount);

            // Post-attack logic on source
            HandleAttackFromSource(sourceEntityBehaviour);

            // Show blood
            if (showBlood)
            {
                EnemyBlood blood = transform.GetComponent<EnemyBlood>();
                if (blood)
                    blood.ShowBloodSplash(0, bloodPosition);
            }
        }

        /// <summary>
        /// Cause spell point damage to entity with additional logic.
        /// </summary>
        /// <param name="source">Source entity behaviour.</param>
        /// <param name="amount">Amount to damage spell points.</param>
        public void DamageMagickaFromSource(DaggerfallEntityBehaviour sourceEntityBehaviour, int amount)
        {
            // Remove fatigue amount
            Entity.DecreaseMagicka(amount);

            // Post-attack logic on source
            HandleAttackFromSource(sourceEntityBehaviour);
        }

        /// <summary>
        /// Handle shared logic when player attacks entity.
        /// </summary>
        public void HandleAttackFromSource(DaggerfallEntityBehaviour sourceEntityBehaviour)
        {
            // Break "normal power" concealment effects on source
            if (sourceEntityBehaviour.Entity.IsMagicallyConcealedNormalPower)
                EntityEffectManager.BreakNormalPowerConcealmentEffects(sourceEntityBehaviour);

            // When source is player
            if (sourceEntityBehaviour == GameManager.Instance.PlayerEntityBehaviour)
            {
                // Handle civilian NPC crime reporting
                if (EntityType == EntityTypes.CivilianNPC)
                {
                    PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;
                    MobilePersonNPC mobileNpc = transform.GetComponent<MobilePersonNPC>();
                    if (mobileNpc)
                    {
                        // Handle assault or murder
                        if (Entity.CurrentHealth > 0)
                        {
                            playerEntity.CrimeCommitted = PlayerEntity.Crimes.Assault;
                            playerEntity.SpawnCityGuards(true);
                        }
                        else
                        {
                            playerEntity.TallyCrimeGuildRequirements(false, 5);
                            // TODO: LOS check from each townsperson. If seen, register crime and start spawning guards as below.
                            playerEntity.CrimeCommitted = PlayerEntity.Crimes.Murder;
                            playerEntity.SpawnCityGuards(true);

                            // Disable when dead
                            mobileNpc.Motor.gameObject.SetActive(false);
                        }
                    }
                }

                // Handle mobile enemy aggro
                if (EntityType == EntityTypes.EnemyClass || EntityType == EntityTypes.EnemyMonster)
                {
                    // Make enemy aggressive to player
                    EnemyMotor enemyMotor = transform.GetComponent<EnemyMotor>();
                    if (enemyMotor)
                    {
                        if (!enemyMotor.IsHostile)
                        {
                            GameManager.Instance.MakeEnemiesHostile();
                        }
                        enemyMotor.MakeEnemyHostileToPlayer(GameManager.Instance.PlayerObject);
                    }
                }
            }
        }

        #endregion

        #region Private Methods

        void SetEntityType(EntityTypes type)
        {
            switch(type)
            {
                case EntityTypes.None:
                    Entity = null;
                    break;
                case EntityTypes.Player:
                    Entity = new PlayerEntity(this);
                    break;
                case EntityTypes.CivilianNPC:
                    Entity = new CivilianEntity(this);
                    break; 
            }

            lastEntityType = type;

            if (Entity != null)
                Entity.SetEntityDefaults();
        }

        void SetEntityValue(DaggerfallEntity value)
        {
            RaiseOnSetEntityHandler(entity, value);
            entity = value;
        }

        #endregion

        #region Events

        public delegate void OnSetEntityHandler(DaggerfallEntity oldEntity, DaggerfallEntity newEntity);
        public event OnSetEntityHandler OnSetEntity;
        void RaiseOnSetEntityHandler(DaggerfallEntity oldEntity, DaggerfallEntity newEntity)
        {
            if (OnSetEntity != null)
                OnSetEntity(oldEntity, newEntity);
        }

        #endregion
    }
}