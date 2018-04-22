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

        const int minAcceptedSpellVersion = 1;

        public DaggerfallMissile FireMissilePrefab;
        public DaggerfallMissile ColdMissilePrefab;
        public DaggerfallMissile PoisonMissilePrefab;
        public DaggerfallMissile ShockMissilePrefab;
        public DaggerfallMissile MagicMissilePrefab;

        EntityEffectBundle readySpell = null;
        EntityEffectBundle lastSpell = null;
        bool instantCast = false;

        DaggerfallEntityBehaviour entityBehaviour = null;
        bool isPlayerEntity = false;
        bool allowSelfDamage = true;

        List<InstancedBundle> instancedBundles = new List<InstancedBundle>();

        #endregion

        #region Structs

        /// <summary>
        /// Stores an instanced effect bundle for executing effects.
        /// </summary>
        struct InstancedBundle
        {
            public EffectBundleSettings settings;
            public DaggerfallEntityBehaviour caster;
            public List<IEntityEffect> effects;
        }

        #endregion

        #region Properties

        public bool HasReadySpell
        {
            get { return (readySpell != null); }
        }

        public EntityEffectBundle ReadySpell
        {
            get { return readySpell; }
        }

        public EntityEffectBundle LastSpell
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

            // Fire instant cast spells
            if (readySpell != null && instantCast)
            {
                CastReadySpell();
                return;
            }

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
        public void SetReadySpell(EntityEffectBundle spell)
        {
            // Spell must appear valid
            if (spell == null || spell.Settings.Version < minAcceptedSpellVersion)
                return;

            // Assign spell - caster only spells are cast instantly
            readySpell = spell;
            if (readySpell.Settings.TargetType == TargetTypes.CasterOnly)
                instantCast = true;

            if (isPlayerEntity && ! instantCast)
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
            // Play casting animation based on element type
            // Spell is released by event handler PlayerSpellCasting_OnReleaseFrame
            GameManager.Instance.PlayerSpellCasting.PlayOneShot(readySpell.Settings.ElementType);

            // TODO: Do not need to show spellcasting animations for certain spell effects
        }

        public void AssignBundle(EntityEffectBundle sourceBundle)
        {
            // Source bundle must have one or more effects
            if (sourceBundle.Settings.Effects == null || sourceBundle.Settings.Effects.Length == 0)
            {
                Debug.Log("AssignBundle() could not assign bundle as source has no effects");
                return;
            }

            // Create new instanced bundle and copy settings from source bundle
            InstancedBundle instancedBundle = new InstancedBundle();
            instancedBundle.settings = sourceBundle.Settings;
            instancedBundle.caster = sourceBundle.CasterEntityBehaviour;
            instancedBundle.effects = new List<IEntityEffect>();

            // Instantiate all effects in this bundle
            for (int i = 0; i < instancedBundle.settings.Effects.Length; i++)
            {
                IEntityEffect effect = GameManager.Instance.EntityEffectBroker.InstantiateEffect(instancedBundle.settings.Effects[i]);
                if (effect == null)
                {
                    Debug.LogFormat("AssignBundle() could not add effect as key '{0}' was not found by broker.");
                    continue;
                }

                // Start effect
                effect.SetDuration(sourceBundle.CasterEntityBehaviour);
                effect.Start();
                effect.MagicRound(this, sourceBundle.CasterEntityBehaviour);
                effect.RemoveRound();

                instancedBundle.effects.Add(effect);
            }

            // Add instanced bundle
            instancedBundles.Add(instancedBundle);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Tick new "magic round" on all instanced bundles for this entity.
        /// </summary>
        void DoMagicRound()
        {
            // Update all effects for all bundles
            foreach (InstancedBundle bundle in instancedBundles)
            {
                foreach (IEntityEffect effect in bundle.effects)
                {
                    // TODO: Expire bundle once all effects have 0 rounds remaining
                    if (effect.RoundsRemaining > 0)
                    {
                        effect.MagicRound(this, bundle.caster);
                        effect.RemoveRound();
                    }
                }
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
            // Assign bundle directly to self if target is caster
            if (readySpell.Settings.TargetType == TargetTypes.CasterOnly)
            {
                AssignBundle(readySpell);
            }
            else
            {

            }

            //// Instatiate missile prefab based on element type
            //DaggerfallMissile missile = null;
            //switch (readySpell.Settings.ElementType)
            //{
            //    case ElementTypes.Fire:
            //        missile = Instantiate(FireMissilePrefab);
            //        break;
            //    case ElementTypes.Cold:
            //        missile = Instantiate(ColdMissilePrefab);
            //        break;
            //    default:
            //        return;
            //}

            //// Set defaults
            //missile.IsTouch = false;
            //missile.AreaOfEffect = false;

            //// A missile can be used for touch, ranged, area around caster, area at range
            //switch (readySpell.Settings.TargetType)
            //{
            //    case TargetTypes.ByTouch:
            //        missile.IsTouch = true;
            //        break;
            //    case TargetTypes.SingleTargetAtRange:
            //        break;
            //    case TargetTypes.AreaAroundCaster:
            //        break;
            //    case TargetTypes.AreaAtRange:
            //        missile.AreaOfEffect = true;
            //        break;
            //}

            // Setup missile
            //missile.UseSpellBillboardAnims(readySpell.Settings.ElementType);
            //missile.Spell = readySpell;

            //// Check if this is a ranged spell
            //bool rangedTarget = false;
            //if (readySpell.Settings.TargetType == TargetTypes.SingleTargetAtRange ||
            //    readySpell.Settings.TargetType == TargetTypes.AreaAtRange)
            //{
            //    rangedTarget = true;
            //}

            //if (rangedTarget)
            //{
            //    // Get missile vectors
            //    Vector3 missileDirection = GameManager.Instance.MainCamera.transform.forward;
            //    Vector3 missilePosition = transform.position + Vector3.up * 0.35f + missileDirection * 0.85f;

            //    // TODO: Execute missile based on target type

            //    //// TEMP: Just hurling test missiles with no payload at this time
            //    //DaggerfallMissile 
            //    //missile.UseSpellBillboardAnims(ElementTypes.Cold);
            //    //missile.ExecuteMobileMissile(missilePosition, missileDirection);
            //}

            lastSpell = readySpell;
            readySpell = null;
            instantCast = false;
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