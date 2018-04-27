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

        const int magicCastSoundID = 349;
        const int poisonCastSoundID = 350;
        const int shockCastSoundID = 351;
        const int fireCastSoundID = 352;
        const int coldCastSoundID = 353; 

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

        List<InstancedBundle> instancedBundles = new List<InstancedBundle>();
        List<InstancedBundle> bundlesToRemove = new List<InstancedBundle>();
        bool clearBundles = false;

        int[] combinedStatMods = new int[DaggerfallStats.Count];
        int[] combinedSkillMods = new int[DaggerfallSkills.Count];
        float refreshModsTimer = 0;
        const float refreshModsDelay = 0.2f;

        #endregion

        #region Structs

        /// <summary>
        /// Stores an instanced effect bundle for executing effects.
        /// </summary>
        public struct InstancedBundle
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

        public InstancedBundle[] EffectBundles
        {
            get { return instancedBundles.ToArray(); }
        }

        #endregion

        #region Unity

        private void Awake()
        {
            // Check if this is player's effect manager
            // We do some extra coordination for player
            entityBehaviour = GetComponent<DaggerfallEntityBehaviour>();
            if (entityBehaviour)
            {
                isPlayerEntity = (entityBehaviour.EntityType == EntityTypes.Player);
            }

            // Only player listens for release frame
            if (isPlayerEntity)
                GameManager.Instance.PlayerSpellCasting.OnReleaseFrame += PlayerSpellCasting_OnReleaseFrame;

            // Wire up events
            EntityEffectBroker.OnNewMagicRound += EntityEffectBroker_OnNewMagicRound;
            SaveLoadManager.OnStartLoad += SaveLoadManager_OnStartLoad;
            StartGameBehaviour.OnNewGame += StartGameBehaviour_OnNewGame;
        }

        private void Start()
        {
            // Listen for entity death to remove effect bundles
            if (entityBehaviour && entityBehaviour.Entity != null)
            {
                entityBehaviour.Entity.OnDeath += Entity_OnDeath;
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

            // Refresh mods more frequently than magic rounds, but not too frequently
            refreshModsTimer += Time.deltaTime;
            if (refreshModsTimer > refreshModsDelay)
            {
                UpdateEntityMods();
                refreshModsTimer = 0;
            }

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

            // Clear bundles if scheduled - doing here ensures not currently iterating bundles during a magic round
            if (clearBundles)
            {
                ClearBundles();
                clearBundles = false;
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
                DaggerfallUI.AddHUDText(HardStrings.pressButtonToFireSpell, 0.4f);
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
                Debug.LogWarning("AssignBundle() could not assign bundle as source has no effects");
                return;
            }

            // Create new instanced bundle and copy settings from source bundle
            InstancedBundle instancedBundle = new InstancedBundle();
            instancedBundle.settings = sourceBundle.Settings;
            instancedBundle.caster = sourceBundle.CasterEntityBehaviour;
            instancedBundle.effects = new List<IEntityEffect>();
            instancedBundles.Add(instancedBundle);
            //Debug.LogFormat("Adding bundle {0}", instancedBundle.GetHashCode());

            // Instantiate all effects in this bundle
            for (int i = 0; i < instancedBundle.settings.Effects.Length; i++)
            {
                IEntityEffect effect = GameManager.Instance.EntityEffectBroker.InstantiateEffect(instancedBundle.settings.Effects[i]);
                if (effect == null)
                {
                    Debug.LogWarningFormat("AssignBundle() could not add effect as key '{0}' was not found by broker.");
                    continue;
                }

                // Start effect
                instancedBundle.effects.Add(effect);
                effect.Start(this, sourceBundle.CasterEntityBehaviour);
            }
        }

        /// <summary>
        /// Wipe all effect bundles from this entity.
        /// </summary>
        public void ClearBundles()
        {
            instancedBundles.Clear();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Tick new "magic round" on all instanced bundles for this entity.
        /// </summary>
        void DoMagicRound()
        {
            // Do nothing further if entity has perished or object disabled
            if (entityBehaviour.Entity.CurrentHealth <= 0 || !entityBehaviour.enabled)
                return;

            // Update all effects for all bundles
            foreach (InstancedBundle bundle in instancedBundles)
            {
                bool hasRemainingEffectRounds = false;
                foreach (IEntityEffect effect in bundle.effects)
                {
                    // Update effects with remaining rounds
                    if (effect.RoundsRemaining > 0)
                    {
                        effect.MagicRound();
                        if (effect.RoundsRemaining > 0)
                            hasRemainingEffectRounds = true;
                    }
                }

                // Expire this bundle once all effects have 0 rounds remaining
                if (!hasRemainingEffectRounds)
                    bundlesToRemove.Add(bundle);
            }

            // Remove any bundles pending deletion
            if (bundlesToRemove.Count > 0)
            {
                foreach (InstancedBundle bundle in bundlesToRemove)
                {
                    RemoveBundle(bundle);
                    //Debug.LogFormat("Removing bundle {0}", bundle.GetHashCode());
                }
                bundlesToRemove.Clear();
            }
        }

        void RemoveBundle(InstancedBundle bundle)
        {
            foreach (IEntityEffect effect in bundle.effects)
                effect.End();

            instancedBundles.Remove(bundle);
            //Debug.LogFormat("Expired bundle {0} with {1} effects", bundle.settings.Name, bundle.settings.Effects.Length);
        }

        void ClearReadySpellHistory()
        {
            lastSpell = null;
            readySpell = null;
        }

        int GetCastSoundID(ElementTypes elementType)
        {
            switch (readySpell.Settings.ElementType)
            {
                case ElementTypes.Cold:
                    return coldCastSoundID;
                case ElementTypes.Fire:
                    return fireCastSoundID;
                case ElementTypes.Poison:
                    return poisonCastSoundID;
                case ElementTypes.Shock:
                    return shockCastSoundID;
                case ElementTypes.Magic:
                    return magicCastSoundID;
                default:
                    return -1;
            }
        }

        DaggerfallMissile InstantiateMissile(ElementTypes elementType)
        {
            switch (elementType)
            {
                case ElementTypes.Cold:
                    return Instantiate(ColdMissilePrefab);
                case ElementTypes.Fire:
                    return Instantiate(FireMissilePrefab);
                case ElementTypes.Poison:
                    return Instantiate(PoisonMissilePrefab);
                case ElementTypes.Shock:
                    return Instantiate(ShockMissilePrefab);
                case ElementTypes.Magic:
                    return Instantiate(MagicMissilePrefab);
                default:
                    return null;
            }
        }

        void UpdateEntityMods()
        {
            // Clear all mods
            Array.Clear(combinedStatMods, 0, DaggerfallStats.Count);
            Array.Clear(combinedSkillMods, 0, DaggerfallSkills.Count);

            // Add together every mod for every live effect
            foreach (InstancedBundle bundle in instancedBundles)
            {
                foreach (IEntityEffect effect in bundle.effects)
                {
                    MergeStatMods(effect, ref combinedStatMods);
                    MergeSkillMods(effect, ref combinedSkillMods);
                }
            }

            // Assign to host entity
            entityBehaviour.Entity.Stats.AssignMods(combinedStatMods);
            entityBehaviour.Entity.Skills.AssignMods(combinedSkillMods);
        }

        void MergeStatMods(IEntityEffect effect, ref int[] combinedStatMods)
        {
            for (int i = 0; i < effect.StatMods.Length; i++)
            {
                combinedStatMods[i] += effect.StatMods[i];
            }
        }

        void MergeSkillMods(IEntityEffect effect, ref int[] combinedSkillMods)
        {
            for (int i = 0; i < effect.SkillMods.Length; i++)
            {
                combinedSkillMods[i] += effect.SkillMods[i];
            }
        }

        #endregion

        #region Event Handling

        private void PlayerSpellCasting_OnReleaseFrame()
        {
            // TODO: Split missile generation from player spell casting so monsters can also cast spells
            // Using player as sole testing platform for now

            // Must have a ready spell
            if (readySpell == null)
                return;

            // Play cast sound from caster audio source
            if (readySpell.CasterEntityBehaviour)
            {
                int castSoundID = GetCastSoundID(readySpell.Settings.ElementType);
                DaggerfallAudioSource audioSource = readySpell.CasterEntityBehaviour.GetComponent<DaggerfallAudioSource>();
                if (castSoundID != -1 && audioSource)
                {
                    audioSource.PlayOneShot((uint)castSoundID);
                }
            }

            // Assign bundle directly to self if target is caster
            // Otherwise instatiate missile prefab based on element type
            if (readySpell.Settings.TargetType == TargetTypes.CasterOnly)
            {
                AssignBundle(readySpell);
            }
            else
            {
                DaggerfallMissile missile = InstantiateMissile(readySpell.Settings.ElementType);
                if (missile)
                    missile.Payload = readySpell;
            }

            lastSpell = readySpell;
            readySpell = null;
            instantCast = false;
        }

        private void EntityEffectBroker_OnNewMagicRound()
        {
            DoMagicRound();
        }

        private void SaveLoadManager_OnStartLoad(SaveData_v1 saveData)
        {
            ClearReadySpellHistory();
            ClearBundles();
        }

        private void StartGameBehaviour_OnNewGame()
        {
            ClearReadySpellHistory();
        }

        private void Entity_OnDeath(DaggerfallEntity entity)
        {
            clearBundles = true;
            entityBehaviour.Entity.OnDeath -= Entity_OnDeath;
            //Debug.LogFormat("Cleared all effect bundles after death of {0}", entity.Name);
        }

        #endregion
    }
}