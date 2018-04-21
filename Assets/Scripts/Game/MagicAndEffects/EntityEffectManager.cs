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
using System;
using System.Collections.Generic;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game.Utility;

namespace DaggerfallWorkshop.Game.MagicAndEffects
{
    /// <summary>
    /// Peered with a DaggerfallEntityBehaviour for magic and effect handling related to that entity.
    /// Manages list of active effects currently operating on peered entity.
    /// Used by player and enemies to send and receive magic effects from various sources.
    /// NOTE: Under active development and subject to frequent change.
    /// </summary>
    public class EntityEffectManager : MonoBehaviour
    {
        #region Fields

        public DaggerfallMissile ColdMissilePrefab;
        public DaggerfallMissile FireMissilePrefab;
        public DaggerfallMissile MagicMissilePrefab;
        public DaggerfallMissile PoisonMissilePrefab;
        public DaggerfallMissile ShockMissilePrefab;

        Spell readySpell = null;
        Spell lastSpell = null;

        DaggerfallEntityBehaviour entityBehaviour = null;
        bool isPlayerEntity = false;
        bool allowSelfDamage = true;

        List<EntityEffectBundle> activeBundles = new List<EntityEffectBundle>();

        #endregion

        #region Properties

        public bool HasReadySpell
        {
            get { return (readySpell != null); }
        }

        public Spell ReadySpell
        {
            get { return readySpell; }
        }

        public Spell LastSpell
        {
            get { return lastSpell; }
        }

        public DaggerfallEntityBehaviour EntityBehaviour
        {
            get { return entityBehaviour; }
        }

        public bool IsPlayerEntity
        {
            get { return isPlayerEntity; }
        }

        public bool AllowSelfDamage
        {
            get { return allowSelfDamage; }
            set { allowSelfDamage = value; }
        }

        #endregion

        #region Unity

        private void Awake()
        {
            GameManager.Instance.PlayerSpellCasting.OnReleaseFrame += PlayerSpellCasting_OnReleaseFrame;
            EntityEffectBroker.OnNewMagicRound += EntityEffectBroker_OnNewMagicRound;
            SaveLoadManager.OnLoad += SaveLoadManager_OnLoad;
            StartGameBehaviour.OnNewGame += StartGameBehaviour_OnNewGame;
        }

        private void Start()
        {
            entityBehaviour = GetComponent<DaggerfallEntityBehaviour>();
            if (entityBehaviour)
            {
                isPlayerEntity = (entityBehaviour.EntityType == EntityTypes.Player);
            }
        }

        private void OnDestroy()
        {
            EntityEffectBroker.OnNewMagicRound -= EntityEffectBroker_OnNewMagicRound;
        }

        private void Update()
        {
            // Do nothing if no peer entity
            if (!entityBehaviour)
                return;

            // Player can cast a spell, recast last spell, or abort current spell
            // Handling input here is similar to handling weapon input in WeaponManager
            if (isPlayerEntity)
            {
                // Cast spell
                if (InputManager.Instance.ActionStarted(InputManager.Actions.ActivateCenterObject) && readySpell != null)
                {
                    CastReadySpell();
                    return;
                }

                // Recast spell - not available while playing another spell animation
                if (InputManager.Instance.ActionStarted(InputManager.Actions.RecastSpell) && lastSpell != null &&
                    !GameManager.Instance.PlayerSpellCasting.IsPlayingAnim)
                {
                    SetReadySpell(lastSpell);
                    return;
                }

                // Abort spell
                if (InputManager.Instance.ActionStarted(InputManager.Actions.AbortSpell) && readySpell != null)
                {
                    AbortReadySpell();
                    return;
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Assigns a new spell to be cast.
        /// For player entity, this will display "press button to fire spell" message.
        /// </summary>
        /// <param name="spell"></param>
        public void SetReadySpell(Spell spell)
        {
            readySpell = spell;

            if (isPlayerEntity)
            {
                DaggerfallUI.AddHUDText(HardStrings.pressButtonToFireSpell);
            }
        }

        public void AbortReadySpell()
        {
            readySpell = null;
        }

        public void CastReadySpell()
        {
            if (readySpell != null)
            {
                // Play casting animation based on element type
                GameManager.Instance.PlayerSpellCasting.PlayOneShot(readySpell.Settings.ElementType);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Tick new "magic round" on all bundles attached to this entity.
        /// </summary>
        void DoMagicRound()
        {
            // Update effect bundles operating on this entity
            foreach (EntityEffectBundle bundle in activeBundles)
            {
                bundle.MagicRound(this);
            }
        }

        void ClearReadySpellHistory()
        {
            lastSpell = null;
            readySpell = null;
        }

        #endregion

        #region Event Handling

        private void PlayerSpellCasting_OnReleaseFrame()
        {
            // Get missile vectors
            Vector3 missileDirection = GameManager.Instance.MainCamera.transform.forward;
            Vector3 missilePosition = transform.position + Vector3.up * 0.35f + missileDirection * 0.85f;

            // Instatiate missile prefab based on element type
            DaggerfallMissile missile;
            switch(readySpell.Settings.ElementType)
            {
                case ElementTypes.Cold:
                    missile = Instantiate(ColdMissilePrefab);
                    break;
                default:
                    return;
            }

            // Setup missile
            missile.UseSpellBillboardAnims(readySpell.Settings.ElementType);
            missile.Spell = readySpell;
            // TODO: Setup based on target type
            
            // TODO: Execute missile based on target type

            //// TEMP: Just hurling test missiles with no payload at this time
            //DaggerfallMissile 
            //missile.UseSpellBillboardAnims(ElementTypes.Cold);
            //missile.ExecuteMobileMissile(missilePosition, missileDirection);

            lastSpell = readySpell;
            readySpell = null;
        }

        private void EntityEffectBroker_OnNewMagicRound()
        {
            DoMagicRound();
        }

        private void SaveLoadManager_OnLoad(SaveData_v1 saveData)
        {
            ClearReadySpellHistory();
        }

        private void StartGameBehaviour_OnNewGame()
        {
            ClearReadySpellHistory();
        }

        #endregion
    }
}