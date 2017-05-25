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
using System;
using System.Collections;
using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.Formulas;
using DaggerfallWorkshop.Game.Player;

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

        private int lastGameMinute = -1;
        PlayerMotor playerMotor = null;

        // Fatigue loss per in-game minute
        private int DefaultFatigueLoss = 11;        // According to DF Chronicles and verified in classic
        //private int ClimbingFatigueLoss = 22;     // According to DF Chronicles
        private int RunningFatigueLoss = 88;        // According to DF Chronicles and verified in classic
        //private int SwimmingFatigueLoss = 44;     // According to DF Chronicles

        private int JumpingFatigueLoss = 11;        // According to DF Chronicles and verified in classic
        private bool CheckedCurrentJump = false;

        private bool gameStarted = false;

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

        #endregion

        #region Unity

        void Start()
        {
            SetEntityType(EntityType);
            playerMotor = GameManager.Instance.PlayerMotor;
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

            if (EntityType == EntityTypes.Player)
            {
                // Wait until game has started and the game time has been set.
                // If the game time is taken before then "30" is returned, which causes an initial player fatigue loss
                // after loading or starting a game with a non-30 minute.
                if (!gameStarted && !GameManager.Instance.StateManager.GameInProgress)
                    return;
                else if (!gameStarted)
                    gameStarted = true;

                // Every game minute, apply fatigue loss to the player
                if (lastGameMinute == -1 && lastGameMinute != DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.Minute)
                    lastGameMinute = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.Minute;
                else if (lastGameMinute != DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.Minute)
                {
                    lastGameMinute = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.Minute;
                    if (playerMotor.IsRunning)
                        Entity.DecreaseFatigue(RunningFatigueLoss);
                    else
                        Entity.DecreaseFatigue(DefaultFatigueLoss);
                }
                // Reduce fatigue when jumping and tally jumping skill
                if (!CheckedCurrentJump && playerMotor.IsJumping)
                {
                    Entity.DecreaseFatigue(JumpingFatigueLoss);
                    Entity.TallySkill((short)Skills.Jumping, 1);
                    CheckedCurrentJump = true;
                }
                // Reset jump fatigue check when grounded
                if (CheckedCurrentJump && !playerMotor.IsJumping)
                {
                    CheckedCurrentJump = false;
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
                    Entity = new PlayerEntity();
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