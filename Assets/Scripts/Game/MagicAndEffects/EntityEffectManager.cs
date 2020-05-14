// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2020 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    Numidium
// 
// Notes:
//

using UnityEngine;
using System;
using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallConnect.FallExe;
using DaggerfallConnect.Save;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects;
using DaggerfallWorkshop.Game.Formulas;
using DaggerfallWorkshop.Game.Items;
using FullSerializer;

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

        const string textDatabase = "ClassicEffects";
        const string youDontHaveTheSpellPointsMessageKey = "youDontHaveTheSpellPoints";
        const int minAcceptedSpellVersion = 1;
        const int rerollMinimumHours = 6;

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
        bool castInProgress = false;
        bool readySpellDoesNotCostSpellPoints = false;
        int readySpellCastingCost;

        DaggerfallEntityBehaviour entityBehaviour = null;
        EntityTypes entityType;

        readonly List<LiveEffectBundle> instancedBundles = new List<LiveEffectBundle>();
        readonly List<LiveEffectBundle> bundlesToRemove = new List<LiveEffectBundle>();
        bool wipeAllBundles = false;
        float timeLastCastSoundPlayed;

        int[] directStatMods = new int[DaggerfallStats.Count];
        int[] directSkillMods = new int[DaggerfallSkills.Count];
        int[] combinedStatMods = new int[DaggerfallStats.Count];
        int[] combinedStatMaxMods = new int[DaggerfallStats.Count];
        int[] combinedSkillMods = new int[DaggerfallSkills.Count];
        int[] combinedResistanceMods = new int[DaggerfallResistances.Count];
        float refreshModsTimer = 0;
        const float refreshModsDelay = 0.2f;

        RacialOverrideEffect racialOverrideEffect;
        PassiveSpecialsEffect passiveSpecialsEffect;
        
        Dictionary<ulong, DaggerfallUnityItem> activeMagicItemsInRound = new Dictionary<ulong, DaggerfallUnityItem>();
        Dictionary<ulong, DaggerfallUnityItem> itemsPendingReroll = new Dictionary<ulong, DaggerfallUnityItem>();

        #endregion

        #region Properties

        public bool HasReadySpell
        {
            get { return (readySpell != null); }
        }

        public EntityEffectBundle ReadySpell
        {
            get { return readySpell; }
            set { readySpell = value; }
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
            get { return (entityType == EntityTypes.Player); }
        }

        public LiveEffectBundle[] EffectBundles
        {
            get { return instancedBundles.ToArray(); }
        }

        public int EffectCount
        {
            get { return instancedBundles.Count; }
        }

        public int DiseaseCount
        {
            get { return GetDiseaseCount(); }
        }

        public LiveEffectBundle[] DiseaseBundles
        {
            get { return GetDiseaseBundles(); }
        }

        public int PoisonCount
        {
            get { return GetPoisonCount(); }
        }

        public LiveEffectBundle[] PoisonBundles
        {
            get { return GetPoisonBundles(); }
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
                entityType = entityBehaviour.EntityType;
            }

            // Only player listens for release frame
            if (IsPlayerEntity)
                GameManager.Instance.PlayerSpellCasting.OnReleaseFrame += PlayerSpellCasting_OnReleaseFrame;

            // Wire up events
            EntityEffectBroker.OnNewMagicRound += EntityEffectBroker_OnNewMagicRound;
            SaveLoadManager.OnStartLoad += SaveLoadManager_OnStartLoad;
            StartGameBehaviour.OnNewGame += StartGameBehaviour_OnNewGame;
            if (IsPlayerEntity)
            {
                DaggerfallRestWindow.OnSleepEnd += DaggerfallRestWindow_OnSleepEnd;
                EntityEffectBroker.OnEndSyntheticTimeIncrease += EntityEffectBroker_OnEndSyntheticTimeIncrease;
            }
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
            SaveLoadManager.OnStartLoad -= SaveLoadManager_OnStartLoad;
            StartGameBehaviour.OnNewGame -= StartGameBehaviour_OnNewGame;
            if (IsPlayerEntity)
            {
                DaggerfallRestWindow.OnSleepEnd -= DaggerfallRestWindow_OnSleepEnd;
                EntityEffectBroker.OnEndSyntheticTimeIncrease -= EntityEffectBroker_OnEndSyntheticTimeIncrease;
            }
        }

        private void Update()
        {
            // Do nothing if no peer entity, game not in play, or load in progress
            if (!entityBehaviour || !GameManager.Instance.IsPlayingGame() || SaveLoadManager.Instance.LoadInProgress)
                return;

            // Remove any bundles pending deletion
            RemovePendingBundles();

            // Run any per-frame constant effects
            DoConstantEffects();

            // Refresh mods more frequently than magic rounds, but not too frequently
            refreshModsTimer += Time.deltaTime;
            if (refreshModsTimer > refreshModsDelay)
            {
                UpdateEntityMods();
                refreshModsTimer = 0;
            }

            // Wipe all bundles if scheduled - doing here ensures not currently iterating bundles during a magic round
            if (wipeAllBundles)
            {
                WipeAllBundles();
                wipeAllBundles = false;
                return;
            }

            // Player can cast a spell, recast last spell, or abort current spell
            // Handling input here is similar to handling weapon input in WeaponManager
            if (IsPlayerEntity)
            {
                // Player must always have passive specials effect
                PassiveSpecialsCheck();

                // Fire no anim spells
                if (readySpell != null && readySpell.Settings.NoCastingAnims)
                {
                    CastNoAnimSpell();
                    return;
                }

                // Fire instant cast spells
                if (readySpell != null && instantCast)
                {
                    CastReadySpell();
                    return;
                }

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
                    if (GameManager.Instance.PlayerEntity.Items.Contains(ItemGroups.MiscItems, (int)MiscItems.Spellbook))
                        SetReadySpell(lastSpell);
                    else
                        DaggerfallUI.AddHUDText(TextManager.Instance.GetText(textDatabase, "noSpellbook"));
                    return;
                }

                // Abort spell
                if (InputManager.Instance.ActionStarted(InputManager.Actions.AbortSpell) && readySpell != null)
                {
                    AbortReadySpell();
                }
            }
            // Enemies always cast ready spell instantly once queued
            else
            {
                CastReadySpell();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets ready spell directly from a classic spell index.
        /// </summary>
        public bool SetReadySpell(int classicSpellIndex, bool noSpellPointCost = false)
        {
            SpellRecord.SpellRecordData spell;
            if (GameManager.Instance.EntityEffectBroker.GetClassicSpellRecord(classicSpellIndex, out spell))
            {
                // Create effect bundle settings from classic spell
                EffectBundleSettings bundleSettings = new EffectBundleSettings();
                if (GameManager.Instance.EntityEffectBroker.ClassicSpellRecordDataToEffectBundleSettings(spell, BundleTypes.Spell, out bundleSettings))
                {
                    EntityEffectBundle bundle = new EntityEffectBundle(bundleSettings, GameManager.Instance.PlayerEntityBehaviour);
                    return SetReadySpell(bundle, noSpellPointCost);
                }
            }
            else
            {
                Debug.LogFormat("SetReadySpell() failed to GetClassicSpellRecord() for classic spell index {0}", classicSpellIndex);
            }

            return false;
        }

        /// <summary>
        /// Assigns a new spell to be cast.
        /// For player entity, this will display "press button to fire spell" message.
        /// </summary>
        public bool SetReadySpell(EntityEffectBundle spell, bool noSpellPointCost = false)
        {
            // Do nothing if silenced or cast already in progress
            if ((SilenceCheck() && !noSpellPointCost) || castInProgress)
                return false;

            // Spell must appear valid
            if (spell == null || spell.Settings.Version < minAcceptedSpellVersion)
                return false;

            // Get spellpoint costs of this spell
            int totalGoldCostUnused;
            FormulaHelper.CalculateTotalEffectCosts(spell.Settings.Effects, spell.Settings.TargetType, out totalGoldCostUnused, out readySpellCastingCost, null, spell.Settings.MinimumCastingCost);

            // Allow casting spells of any cost if entity is player and godmode enabled
            bool godModeCast = (IsPlayerEntity && GameManager.Instance.PlayerEntity.GodMode);

            // Enforce spell point costs - Daggerfall does this when setting ready spell
            // Classic does not enforce this for enemies, they can cast any spell as long as they still have at least 1 spell point.
            // Doing the same here. This also matters for classic AI combat logic, as it uses the existence of any remaining spell points
            // to determine whether or not it can still cast spells.
            if (IsPlayerEntity && entityBehaviour.Entity.CurrentMagicka < readySpellCastingCost && !godModeCast && !noSpellPointCost)
            {
                DaggerfallUI.AddHUDText(TextManager.Instance.GetText(textDatabase, youDontHaveTheSpellPointsMessageKey));

                readySpell = null;
                readySpellCastingCost = 0;
                return false;
            }

            // Assign spell - caster only spells are cast instantly
            readySpell = spell;
            RaiseOnNewReadySpell(spell);
            readySpellDoesNotCostSpellPoints = noSpellPointCost;
            if (readySpell.Settings.TargetType == TargetTypes.CasterOnly)
                instantCast = true;

            if (IsPlayerEntity && !instantCast)
            {
                DaggerfallUI.AddHUDText(HardStrings.pressButtonToFireSpell, 0.4f);
            }

            return true;
        }

        public void AbortReadySpell()
        {
            readySpell = null;
            readySpellDoesNotCostSpellPoints = false;
        }

        public void CastNoAnimSpell()
        {
            // Must have a spell loaded
            if (readySpell == null)
                return;

            // Assign bundle directly to self if target is caster
            // Otherwise instatiate missile prefab based on element type
            if (readySpell.Settings.TargetType == TargetTypes.CasterOnly)
            {
                AssignBundle(readySpell);
            }
            else
            {
                DaggerfallMissile missile = InstantiateSpellMissile(readySpell.Settings.ElementType);
                if (missile)
                    missile.Payload = readySpell;
            }

            // Deduct spellpoint cost from entity if not free (magic item, innate ability)
            if (!readySpellDoesNotCostSpellPoints)
                entityBehaviour.Entity.DecreaseMagicka(readySpellCastingCost);

            // Clear ready spell and reset casting - do not store last spell for no anim spells (prevent spamming)
            RaiseOnCastReadySpell(readySpell);
            lastSpell = null;
            readySpell = null;
            readySpellCastingCost = 0;
            instantCast = false;
            castInProgress = false;
            readySpellDoesNotCostSpellPoints = false;
        }

        public void CastReadySpell()
        {
            // Do nothing if silenced
            if (SilenceCheck())
                return;

            // Must have a ready spell and a previous cast must not be in progress
            if (readySpell == null || castInProgress)
                return;

            // Player must be in range to release a touch spell
            // Enemies use AI to only cast touch spells within range
            if (IsPlayerEntity && readySpell.Settings.TargetType == TargetTypes.ByTouch)
            {
                Vector3 aimPosition = GameManager.Instance.MainCamera.transform.position;
                Vector3 aimDirection = GameManager.Instance.MainCamera.transform.forward;
                if (DaggerfallMissile.GetEntityTargetInTouchRange(aimPosition, aimDirection) == null)
                {
                    //Debug.Log("Target entity not in range for touch spell.");
                    return;
                }
            }

            // Deduct spellpoint cost from entity if not free (magic item, innate ability)
            if (!readySpellDoesNotCostSpellPoints)
                entityBehaviour.Entity.DecreaseMagicka(readySpellCastingCost);

            // Play casting animation based on element type
            // Spell is released by event handler PlayerSpellCasting_OnReleaseFrame
            // TODO: Do not need to show spellcasting animations for certain spell effects
            if (IsPlayerEntity)
            {
                // Play casting animation and block further casting attempts until previous cast is complete
                GameManager.Instance.PlayerSpellCasting.PlayOneShot(readySpell.Settings.ElementType);
                castInProgress = true;
            }
            else
            {
                EnemyCastReadySpell();
            }
        }

        public void AssignBundle(EntityEffectBundle sourceBundle, AssignBundleFlags flags = AssignBundleFlags.None)
        {
            // Check flags
            bool showNonPlayerFailures = (flags & AssignBundleFlags.ShowNonPlayerFailures) == AssignBundleFlags.ShowNonPlayerFailures;
            bool bypassSavingThrows = (flags & AssignBundleFlags.BypassSavingThrows) == AssignBundleFlags.BypassSavingThrows;
            bool specialInfection = (flags & AssignBundleFlags.SpecialInfection) == AssignBundleFlags.SpecialInfection;

            // Source bundle must have one or more effects
            if (sourceBundle.Settings.Effects == null || sourceBundle.Settings.Effects.Length == 0)
            {
                Debug.LogWarning("AssignBundle() could not assign bundle as source has no effects");
                return;
            }

            // Create new instanced bundle and copy settings from source bundle
            LiveEffectBundle instancedBundle = new LiveEffectBundle();
            instancedBundle.version = sourceBundle.Settings.Version;
            instancedBundle.bundleType = sourceBundle.Settings.BundleType;
            instancedBundle.targetType = sourceBundle.Settings.TargetType;
            instancedBundle.elementType = sourceBundle.Settings.ElementType;
            instancedBundle.runtimeFlags = sourceBundle.Settings.RuntimeFlags;
            instancedBundle.name = sourceBundle.Settings.Name;
            instancedBundle.iconIndex = sourceBundle.Settings.IconIndex;
            instancedBundle.icon = sourceBundle.Settings.Icon;
            instancedBundle.fromEquippedItem = sourceBundle.FromEquippedItem;
            instancedBundle.castByItem = sourceBundle.CastByItem;
            instancedBundle.liveEffects = new List<IEntityEffect>();
            if (sourceBundle.CasterEntityBehaviour)
            {
                instancedBundle.caster = sourceBundle.CasterEntityBehaviour;
                instancedBundle.casterEntityType = sourceBundle.CasterEntityBehaviour.EntityType;
                instancedBundle.casterLoadID = GetCasterLoadID(sourceBundle.CasterEntityBehaviour);
            }

            // Instantiate all effects in this bundle
            for (int i = 0; i < sourceBundle.Settings.Effects.Length; i++)
            {
                // Instantiate effect
                IEntityEffect effect = GameManager.Instance.EntityEffectBroker.InstantiateEffect(sourceBundle.Settings.Effects[i]);
                if (effect == null)
                {
                    Debug.LogWarningFormat("AssignBundle() could not add effect as key '{0}' was not found by broker.", sourceBundle.Settings.Effects[i].Key);
                    continue;
                }

                // Assign any enchantment params to live effect
                effect.EnchantmentParam = sourceBundle.Settings.Effects[i].EnchantmentParam;

                // Incoming disease and paralysis effects are blocked if entity is hard immune (e.g. vampires/lycanthropes)
                // The exceptions are vampirism/lycanthropy special infections themselves which ignore disease resistance
                if (effect is DiseaseEffect && IsEntityImmuneToDisease() && !specialInfection ||
                    effect is Paralyze && IsEntityImmuneToParalysis())
                {
                    continue;
                }

                // Set parent bundle
                effect.ParentBundle = instancedBundle;

                // Spell Absorption, Reflection, Resistance - must have a caster entity set
                if (sourceBundle.CasterEntityBehaviour)
                {
                    // Spell Absorption
                    int absorbSpellPoints;
                    if (sourceBundle.Settings.BundleType == BundleTypes.Spell && TryAbsorption(effect, sourceBundle.Settings.TargetType, sourceBundle.CasterEntityBehaviour.Entity, out absorbSpellPoints))
                    {
                        // Spell passed all checks and was absorbed - return cost output to target
                        entityBehaviour.Entity.IncreaseMagicka(absorbSpellPoints);

                        // Output "Spell was absorbed."
                        DaggerfallUI.AddHUDText(TextManager.Instance.GetText(textDatabase, "spellAbsorbed"));

                        continue;
                    }

                    // Spell Reflection
                    if (!bypassSavingThrows && sourceBundle.Settings.BundleType == BundleTypes.Spell && TryReflection(sourceBundle))
                        continue;

                    // Spell Resistance
                    if (!bypassSavingThrows && sourceBundle.Settings.BundleType == BundleTypes.Spell && TryResistance(sourceBundle))
                        continue;
                }

                // Start effect
                effect.Start(this, sourceBundle.CasterEntityBehaviour);

                // Do not proceed if chance failed
                if (effect.Properties.SupportChance &&
                    effect.Properties.ChanceFunction == ChanceFunction.OnCast &&
                    !effect.ChanceSuccess)
                {
                    // Output failure messages
                    if (IsPlayerEntity && sourceBundle.Settings.TargetType == TargetTypes.CasterOnly)
                    {
                        // Output "Spell effect failed." for caster only spells
                        DaggerfallUI.AddHUDText(TextManager.Instance.GetText(textDatabase, "spellEffectFailed"));
                    }
                    else if (IsPlayerEntity || showNonPlayerFailures)
                    {
                        // Output "Save versus spell made." for external contact spells
                        DaggerfallUI.AddHUDText(TextManager.Instance.GetText(textDatabase, "saveVersusSpellMade"));
                    }

                    continue;
                }

                // Do not add unflagged incumbent effects
                // But allow for an icon refresh as duration might have changed and we want to update this sooner than next magic round
                if (effect is IncumbentEffect && !(effect as IncumbentEffect).IsIncumbent)
                {
                    RaiseOnAssignBundle(instancedBundle);
                    continue;
                }

                // Saving throw handling
                // For effects without magnitude (e.g. paralysis) the entity has a chance to save against entire effect using a saving throw
                // For effects with magnitude (e.g. most Destruction magic) this is handled later when the effect rolls for magnitude
                // Self-cast spells (e.g. self heals and buffs) should never be saved against
                if (!bypassSavingThrows &&
                    !effect.BypassSavingThrows &&
                    !effect.Properties.SupportMagnitude &&
                    sourceBundle.Settings.TargetType != TargetTypes.CasterOnly)
                {
                    // Immune if saving throw made
                    if (FormulaHelper.SavingThrow(effect, entityBehaviour.Entity) == 0)
                    {
                        if (IsPlayerEntity || showNonPlayerFailures)
                        {
                            // Output "Save versus spell made." for external contact spells
                            DaggerfallUI.AddHUDText(TextManager.Instance.GetText(textDatabase, "saveVersusSpellMade"));
                        }
                        continue;
                    }
                }

                // Player is immune to paralysis in god mode
                if (IsPlayerEntity && GameManager.Instance.PlayerEntity.GodMode && effect is Paralyze)
                    continue;

                // Add effect
                instancedBundle.liveEffects.Add(effect);

                // Cache racial override effect
                if (effect is RacialOverrideEffect)
                    racialOverrideEffect = (RacialOverrideEffect)effect;

                // At this point effect is ready and gets initial magic round
                effect.MagicRound();
            }

            // Add bundles with at least one effect
            if (instancedBundle.liveEffects.Count > 0)
            {
                instancedBundles.Add(instancedBundle);
                RaiseOnAssignBundle(instancedBundle);
                Debug.LogFormat("Adding bundle [{0}] {1}", instancedBundle.name, instancedBundle.GetHashCode());
            }
        }

        /// <summary>
        /// Checks if peered entity is globally immune to disease from career, race, and effect system.
        /// </summary>
        /// <returns>True if entity immune to disease.</returns>
        public bool IsEntityImmuneToDisease()
        {
            // Entity is hard immune from career or effect
            if (entityBehaviour.Entity.Career.Disease == DFCareer.Tolerance.Immune || entityBehaviour.Entity.IsImmuneToDisease)
                return true;

            // Player entity is hard immune from racial template unless they have an overriding career weakness
            if (IsPlayerEntity && (((PlayerEntity)entityBehaviour.Entity).GetLiveRaceTemplate().ImmunityFlags & DFCareer.EffectFlags.Disease) != 0)
            {
                return entityBehaviour.Entity.Career.Disease != DFCareer.Tolerance.LowTolerance &&
                       entityBehaviour.Entity.Career.Disease != DFCareer.Tolerance.CriticalWeakness;
            }

            // Not hard immune - fallback to saving throws where entity may still have enhanced resistance
            return false;
        }

        /// <summary>
        /// Checks if peered entity is globally immune to paralysis from career, race, and effect system.
        /// </summary>
        /// <returns>True if entity immune to paralysis.</returns>
        public bool IsEntityImmuneToParalysis()
        {
            // Entity is hard immune from career or effect
            if (entityBehaviour.Entity.Career.Paralysis == DFCareer.Tolerance.Immune || entityBehaviour.Entity.IsImmuneToParalysis)
                return true;

            // Player entity is hard immune from racial template unless they have an overriding career weakness
            if (IsPlayerEntity && (((PlayerEntity)entityBehaviour.Entity).GetLiveRaceTemplate().ImmunityFlags & DFCareer.EffectFlags.Paralysis) != 0)
            {
                return entityBehaviour.Entity.Career.Paralysis != DFCareer.Tolerance.LowTolerance &&
                       entityBehaviour.Entity.Career.Paralysis != DFCareer.Tolerance.CriticalWeakness;
            }

            // Not hard immune - fallback to saving throws where entity may still have enhanced resistance
            return false;
        }

        /// <summary>
        /// Searches all effects in all bundles to find incumbent of type T.
        /// </summary>
        /// <typeparam name="T">Effect class to search for.</typeparam>
        /// <returns>Found incumbent effect of type T or null.</returns>
        public IEntityEffect FindIncumbentEffect<T>()
        {
            foreach (LiveEffectBundle bundle in instancedBundles)
            {
                foreach (IEntityEffect effect in bundle.liveEffects)
                {
                    if (effect is T)
                        return effect;
                }
            }

            return null;
        }

        /// <summary>
        /// Searches all effects in all bundles to find all incumbents of type T.
        /// Useful for finding all effects inheriting from a specific parent effect.
        /// </summary>
        /// <typeparam name="T">Effect class to search for.</typeparam>
        /// <returns>All found incumbent effect of type T. Can be null or empty.</returns>
        public IEntityEffect[] FindIncumbentEffects<T>()
        {
            List<IEntityEffect> effects = new List<IEntityEffect>();
            foreach (LiveEffectBundle bundle in instancedBundles)
            {
                foreach (IEntityEffect effect in bundle.liveEffects)
                {
                    if (effect is T)
                        effects.Add(effect);
                }
            }

            return effects.ToArray();
        }

        /// <summary>
        /// Gets current racial override effect if one is present.
        /// Racial override is a special case effect that is cached when started/resumed on entity.
        /// While is possible to use <see cref="FindIncumbentEffect{RacialOverrideEffect}()"/>, this method is more efficient.
        /// </summary>
        /// <returns>RacialOverrideEffect or null.</returns>
        public RacialOverrideEffect GetRacialOverrideEffect()
        {
            return racialOverrideEffect;
        }

        /// <summary>
        /// Searches all effects in all bundles to heal an amount of attribute loss.
        /// Will spread healing amount across multiple effects if required.
        /// </summary>
        /// <param name="stat">Attribute to heal.</param>
        /// <param name="amount">Amount to heal. Must be a positive value.</param>
        public void HealAttribute(DFCareer.Stats stat, int amount)
        {
            if (amount < 0)
            {
                Debug.LogWarning("EntityEffectManager.HealDamagedAttribute() received a negative value for amount - ignoring.");
                return;
            }

            int remaining = amount;
            foreach (LiveEffectBundle bundle in instancedBundles)
            {
                foreach (BaseEntityEffect effect in bundle.liveEffects)
                {
                    // Get attribute modifier of this effect and ignore if attribute not damaged
                    int mod = effect.GetAttributeMod(stat);
                    if (mod >= 0)
                        continue;

                    // Heal all or part of damage depending on how much healing remains
                    int damage = Mathf.Abs(mod);
                    if (remaining > damage)
                    {
                        effect.HealAttributeDamage(stat, damage);
                        remaining -= damage;
                    }
                    else
                    {
                        effect.HealAttributeDamage(stat, remaining);
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Cancels all remaining rounds of any active incumbent effect of type T and calls End() on that effect.
        /// If incumbent effect T is only live effect in bundle then whole bundle will be removed.
        /// If other effects remain in bundle then incumbent effect will stop operation and bundle will expire when other effects allow it.
        /// Does nothing if no incumbent effect of type T found.
        /// </summary>
        /// <typeparam name="T">IncumbentEffect type T to end.</typeparam>
        public void EndIncumbentEffect<T>()
        {
            IEntityEffect effect = FindIncumbentEffect<T>();
            if (effect != null)
            {
                effect.RoundsRemaining = 0;
                effect.End();
            }
        }

        /// <summary>
        /// Wipe all effect bundles from this entity.
        /// </summary>
        private void WipeAllBundles()
        {
            instancedBundles.Clear();
            itemsPendingReroll.Clear();
            RaiseOnRemoveBundle(null);
            racialOverrideEffect = null;
            passiveSpecialsEffect = null;
        }

        /// <summary>
        /// Merge custom stat mods directly into this entity.
        /// Changes reset at the start of each magic round.
        /// </summary>
        /// <param name="statMods">Stat mods array, must be DaggerfallStats.Count length.</param>
        public void MergeDirectStatMods(int[] statMods)
        {
            if (statMods == null || statMods.Length != DaggerfallStats.Count)
                return;

            for (int i = 0; i < statMods.Length; i++)
            {
                directStatMods[i] += statMods[i];
            }
        }

        /// <summary>
        /// Merge custom skill mods directly into this entity.
        /// Changes reset at the start of each magic round.
        /// </summary>
        /// <param name="skillMods">Skill mods array, must be DaggerfallSkills.Count length.</param>
        public void MergeDirectSkillMods(int[] skillMods)
        {
            if (skillMods == null || skillMods.Length != DaggerfallSkills.Count)
                return;

            for (int i = 0; i < skillMods.Length; i++)
            {
                directSkillMods[i] += skillMods[i];
            }
        }

        /// <summary>
        /// Clears all bundles of BundleTypes.Spell.
        /// </summary>
        public void ClearSpellBundles()
        {
            foreach (LiveEffectBundle bundle in instancedBundles)
            {
                // Expire spell bundles
                if (bundle.bundleType == BundleTypes.Spell)
                    bundlesToRemove.Add(bundle);
            }

            RemovePendingBundles();
        }

        /// <summary>
        /// Instantiates a spell missile based on prefabs set to player.
        /// Mainly used by player casting and action records that throw spells at player.
        /// </summary>
        /// <param name="elementType">Element of missile.</param>
        /// <returns>DaggerfallMissile.</returns>
        public DaggerfallMissile InstantiateSpellMissile(ElementTypes elementType)
        {
            DaggerfallMissile missile = null;
            switch (elementType)
            {
                case ElementTypes.Cold:
                    missile = Instantiate(ColdMissilePrefab);
                    break;
                case ElementTypes.Fire:
                    missile = Instantiate(FireMissilePrefab);
                    break;
                case ElementTypes.Poison:
                    missile = Instantiate(PoisonMissilePrefab);
                    break;
                case ElementTypes.Shock:
                    missile = Instantiate(ShockMissilePrefab);
                    break;
                case ElementTypes.Magic:
                    missile = Instantiate(MagicMissilePrefab);
                    break;
                default:
                    return null;
            }

            if (missile)
            {
                missile.transform.parent = GameObjectHelper.GetBestParent();
            }

            return missile;
        }

        /// <summary>
        /// Allows any effect to update HUD icons when an immediate refresh is required.
        /// Example is when an effect ends prematurely due to some condition (e.g. Shield spell busted).
        /// </summary>
        public void UpdateHUDSpellIcons()
        {
            if (DaggerfallUI.Instance.enableHUD && DaggerfallUI.Instance.DaggerfallHUD.ActiveSpells != null)
                DaggerfallUI.Instance.DaggerfallHUD.ActiveSpells.UpdateIcons();
        }

        /// <summary>
        /// Creates a simple single-effect spell bundle from the specified effect key.
        /// </summary>
        /// <param name="effectKey">Effect key to create bundle from.</param>
        /// <param name="element">Element type for bundle. Defaults to Magic.</param>
        /// <param name="effectSettings">Effect settings. Uses full defaults if null.</param>
        /// <returns>EntityEffectBundle.</returns>
        public EntityEffectBundle CreateSpellBundle(string effectKey, ElementTypes element = ElementTypes.Magic, EffectSettings? effectSettings = null)
        {
            if (effectSettings == null)
                effectSettings = BaseEntityEffect.DefaultEffectSettings();

            EffectBundleSettings settings = new EffectBundleSettings()
            {
                Version = EntityEffectBroker.CurrentSpellVersion,
                BundleType = BundleTypes.Spell,
                ElementType = element,
                Effects = new EffectEntry[] { new EffectEntry(effectKey, effectSettings.Value) },
            };

            return new EntityEffectBundle(settings, entityBehaviour);
        }

        #endregion

        #region Potions

        public void DrinkPotion(DaggerfallUnityItem item)
        {
            // Item must be a valid potion.
            if (item == null || item.PotionRecipeKey == 0)
                return;

            // Get potion recipe and main effect. (most potions only have one effect)
            EntityEffectBroker effectBroker = GameManager.Instance.EntityEffectBroker;
            PotionRecipe potionRecipe = effectBroker.GetPotionRecipe(item.PotionRecipeKey);
            IEntityEffect potionEffect = effectBroker.GetPotionRecipeEffect(potionRecipe);

            // Get any secondary effects and generate the effect entry array. (a single settings struct is shared between the effects)
            EffectEntry[] potionEffects;
            List<string> secondaryEffects = potionRecipe.SecondaryEffects;
            if (secondaryEffects != null)
            {
                potionEffects = new EffectEntry[secondaryEffects.Count + 1];
                potionEffects[0] = new EffectEntry(potionEffect.Key, potionRecipe.Settings);
                for (int i = 0; i < secondaryEffects.Count; i++)
                {
                    IEntityEffect effect = effectBroker.GetEffectTemplate(secondaryEffects[i]);
                    potionEffects[i+1] = new EffectEntry(effect.Key, potionRecipe.Settings);
                }
            }
            else
            {
                potionEffects = new EffectEntry[] { new EffectEntry(potionEffect.Key, potionRecipe.Settings) };
            }
            // Create the effect bundle settings.
            EffectBundleSettings bundleSettings = new EffectBundleSettings()
            {
                Version = EntityEffectBroker.CurrentSpellVersion,
                BundleType = BundleTypes.Potion,
                TargetType = TargetTypes.CasterOnly,
                Effects = potionEffects,
            };
            // Assign effect bundle.
            EntityEffectBundle bundle = new EntityEffectBundle(bundleSettings, entityBehaviour);
            AssignBundle(bundle, AssignBundleFlags.BypassSavingThrows);

            // Play cast sound on drink for player only.
            if (IsPlayerEntity)
                PlayCastSound(entityBehaviour, GetCastSoundID(ElementTypes.Magic));
        }

        #endregion

        #region Magic Items

        /// <summary>
        /// Executes payloads on enchanted items.
        /// </summary>
        /// <param name="flags">Payloads to execute. Required by all payloads.</param>
        /// <param name="sourceItem">Item to execute payloads from. Required by all payloads. Item must be enchanted.</param>
        /// <param name="sourceCollection">ItemCollection the enchanted item belongs to. Required by Used payload.</param>
        /// <param name="targetEntity">Target entity of attack by this entity. Required by Strikes payload.</param>
        /// <param name="damageIn">Input damage before effects. Required by Strikes payload.</param>
        /// <returns>Output damage after effect. Only changes for Strikes payload.</returns>
        public int DoItemEnchantmentPayloads(
            EnchantmentPayloadFlags flags,
            DaggerfallUnityItem sourceItem,
            ItemCollection sourceCollection = null,
            DaggerfallEntityBehaviour targetEntity = null,
            int damageIn = 0)
        {
            int damageOut = damageIn;

            // Must specify an item
            if (sourceItem == null)
                return damageOut;

            // Get combined enchantments
            EnchantmentSettings[] enchantments = sourceItem.GetCombinedEnchantmentSettings();
            if (enchantments == null || enchantments.Length == 0)
                return damageOut;

            // Process all enchantments
            foreach (EnchantmentSettings settings in enchantments)
            {
                // Get effect template
                IEntityEffect effectTemplate = GameManager.Instance.EntityEffectBroker.GetEffectTemplate(settings.EffectKey);
                if (effectTemplate == null)
                {
                    Debug.LogWarningFormat("DoItemEnchantmentPayloads() effect key {0} not found in broker.", settings.EffectKey);
                    return damageOut;
                }

                // Equipped payload
                if ((flags & EnchantmentPayloadFlags.Equipped) == EnchantmentPayloadFlags.Equipped && effectTemplate.HasEnchantmentPayloadFlags(EnchantmentPayloadFlags.Equipped))
                    StartEquippedItem(effectTemplate, sourceItem, settings);

                // Held payload
                // Note: EnchantmentPayloadFlags.Held means the effect wants a long-running bundle assigned, it is unrelated to "cast when held" legacy enchantment
                if ((flags & EnchantmentPayloadFlags.Held) == EnchantmentPayloadFlags.Held && effectTemplate.HasEnchantmentPayloadFlags(EnchantmentPayloadFlags.Held))
                    StartHeldItem(effectTemplate, sourceItem, settings);

                // Unequip payload
                if ((flags & EnchantmentPayloadFlags.Unequipped) == EnchantmentPayloadFlags.Unequipped)
                    UnequipHeldItem(effectTemplate, sourceItem, settings);

                // Used payload
                if ((flags & EnchantmentPayloadFlags.Used) == EnchantmentPayloadFlags.Used && effectTemplate.HasEnchantmentPayloadFlags(EnchantmentPayloadFlags.Used))
                    UseItem(effectTemplate, sourceItem, settings, sourceCollection);

                // Strikes payload
                if ((flags & EnchantmentPayloadFlags.Strikes) == EnchantmentPayloadFlags.Strikes && effectTemplate.HasEnchantmentPayloadFlags(EnchantmentPayloadFlags.Strikes))
                    damageOut += StrikeWithItem(effectTemplate, sourceItem, settings, targetEntity, damageIn);

                // Breaks payload
                if ((flags & EnchantmentPayloadFlags.Breaks) == EnchantmentPayloadFlags.Breaks && effectTemplate.HasEnchantmentPayloadFlags(EnchantmentPayloadFlags.Breaks))
                    BrokenItem(effectTemplate, sourceItem, settings);

                // Do not call following payloads if item has broken
                if (sourceItem.currentCondition > 0)
                {
                    // MagicRound payload
                    if ((flags & EnchantmentPayloadFlags.MagicRound) == EnchantmentPayloadFlags.MagicRound && effectTemplate.HasEnchantmentPayloadFlags(EnchantmentPayloadFlags.MagicRound))
                        MagicRoundCallback(effectTemplate, sourceItem, settings);

                    // RerollEffect payload
                    if ((flags & EnchantmentPayloadFlags.RerollEffect) == EnchantmentPayloadFlags.RerollEffect && effectTemplate.HasEnchantmentPayloadFlags(EnchantmentPayloadFlags.RerollEffect))
                        RerollEffectCallback(effectTemplate, sourceItem, settings);
                }
            }

            // Clamp damageOut to 0
            if (damageOut < 0)
                damageOut = 0;

            return damageOut;
        }

        void StartEquippedItem(IEntityEffect effectTemplate, DaggerfallUnityItem item, EnchantmentSettings settings)
        {
            // Equipped payload callback
            EnchantmentParam param = new EnchantmentParam() { ClassicParam = settings.ClassicParam, CustomParam = settings.CustomParam };
            PayloadCallbackResults? results = effectTemplate.EnchantmentPayloadCallback(EnchantmentPayloadFlags.Equipped, param, entityBehaviour, entityBehaviour, item);
            if (results != null && results.Value.durabilityLoss > 0)
                item.LowerCondition(results.Value.durabilityLoss, entityBehaviour.Entity, entityBehaviour.Entity.Items);
        }

        void StartHeldItem(IEntityEffect effectTemplate, DaggerfallUnityItem item, EnchantmentSettings settings)
        {
            // Held payload assigns a new bundle with a fully stateful effect instance - does not use callback to effect template
            EnchantmentParam param = new EnchantmentParam() { ClassicParam = settings.ClassicParam, CustomParam = settings.CustomParam };
            EffectBundleSettings heldEffectSettings = new EffectBundleSettings()
            {
                Version = EntityEffectBroker.CurrentSpellVersion,
                BundleType = BundleTypes.HeldMagicItem,
                TargetType = TargetTypes.None,
                ElementType = ElementTypes.None,
                Name = settings.EffectKey,
                Effects = new EffectEntry[] { new EffectEntry(effectTemplate.Key, param) },
            };
            EntityEffectBundle heldEffectBundle = new EntityEffectBundle(heldEffectSettings, entityBehaviour);
            heldEffectBundle.FromEquippedItem = item;
            AssignBundle(heldEffectBundle, AssignBundleFlags.BypassSavingThrows);
            DoConstantEffects();
        }

        void UnequipHeldItem(IEntityEffect effectTemplate, DaggerfallUnityItem item, EnchantmentSettings settings)
        {
            // Unequipped payload callback
            if (effectTemplate.HasEnchantmentPayloadFlags(EnchantmentPayloadFlags.Unequipped))
            {
                EnchantmentParam param = new EnchantmentParam() { ClassicParam = settings.ClassicParam, CustomParam = settings.CustomParam };
                effectTemplate.EnchantmentPayloadCallback(EnchantmentPayloadFlags.Unequipped, param, entityBehaviour, entityBehaviour, item);
            }

            // Check all running bundles for any linked to this item and schedule instant removal
            foreach (LiveEffectBundle bundle in instancedBundles)
            {
                if (bundle.fromEquippedItem != null && bundle.fromEquippedItem.UID == item.UID)
                {
                    if (!bundlesToRemove.Contains(bundle))
                        bundlesToRemove.Add(bundle);
                }
            }
            RemovePendingBundles();
            DoConstantEffects();
        }

        void UseItem(IEntityEffect effectTemplate, DaggerfallUnityItem item, EnchantmentSettings settings, ItemCollection sourceCollection)
        {
            // Used payload callback
            EnchantmentParam param = new EnchantmentParam() { ClassicParam = settings.ClassicParam, CustomParam = settings.CustomParam };
            PayloadCallbackResults? results = effectTemplate.EnchantmentPayloadCallback(EnchantmentPayloadFlags.Used, param, entityBehaviour, entityBehaviour, item);
            if (results != null && results.Value.durabilityLoss > 0)
                item.LowerCondition(results.Value.durabilityLoss, GameManager.Instance.PlayerEntity, sourceCollection);
        }

        int StrikeWithItem(IEntityEffect effectTemplate, DaggerfallUnityItem item, EnchantmentSettings settings, DaggerfallEntityBehaviour targetEntity, int damageIn)
        {
            // Strikes payload callback
            EnchantmentParam param = new EnchantmentParam() { ClassicParam = settings.ClassicParam, CustomParam = settings.CustomParam };
            PayloadCallbackResults? results = effectTemplate.EnchantmentPayloadCallback(EnchantmentPayloadFlags.Strikes, param, entityBehaviour, targetEntity, item, damageIn);
            if (results != null)
            {
                if (results.Value.durabilityLoss > 0)
                    item.LowerCondition(results.Value.durabilityLoss, entityBehaviour.Entity, entityBehaviour.Entity.Items);

                return results.Value.strikesModulateDamage;
            }

            return 0;
        }

        void BrokenItem(IEntityEffect effectTemplate, DaggerfallUnityItem item, EnchantmentSettings settings)
        {
            // Breaks payload callback
            EnchantmentParam param = new EnchantmentParam() { ClassicParam = settings.ClassicParam, CustomParam = settings.CustomParam };
            effectTemplate.EnchantmentPayloadCallback(EnchantmentPayloadFlags.Breaks, param, entityBehaviour, entityBehaviour, item);
        }

        void MagicRoundCallback(IEntityEffect effectTemplate, DaggerfallUnityItem item, EnchantmentSettings settings)
        {
            // MagicRound payload callback
            EnchantmentParam param = new EnchantmentParam() { ClassicParam = settings.ClassicParam, CustomParam = settings.CustomParam };
            effectTemplate.EnchantmentPayloadCallback(EnchantmentPayloadFlags.MagicRound, param, entityBehaviour, entityBehaviour, item);
        }

        void RerollEffectCallback(IEntityEffect effectTemplate, DaggerfallUnityItem item, EnchantmentSettings settings)
        {
            // RerollEffect payload callback
            EnchantmentParam param = new EnchantmentParam() { ClassicParam = settings.ClassicParam, CustomParam = settings.CustomParam };
            effectTemplate.EnchantmentPayloadCallback(EnchantmentPayloadFlags.RerollEffect, param, entityBehaviour, entityBehaviour, item);
        }

        #endregion

        #region Spell Absorption

        /// <summary>
        /// Tests incoming effect for spell absorption. If absorption succeeds the entity will
        /// block effect and recover spell points equal to the casting cost of blocked effect.
        /// If target does not have enough spell points free to absorb effect cost then effect will NOT be absorbed.
        /// For example if player has 0 of 50 spell points available, they can absorb an incoming effect costing up to 50 spell points.
        /// An effect costing 51 spell points cannot be absorbed. It's "all or nothing".
        /// Notes:
        ///  - There are two variants of spell absorption in Daggerfall.
        ///     - Career-based: This is the "none / in light / in darkness / always" assigned to entity career kit.
        ///     - Effect-based: Generated by having an active Spell Absorption effect from a spell or item.
        ///  - In classic effect-based absorption from spells/items will override career-based absorption. Not sure if bug.
        ///  - Career-based absorption will always succeed chance check.
        ///  - Spell-based will roll for check on each absorb attempt.
        /// </summary>
        /// <param name="effect">Incoming effect.</param>
        /// <param name="targetType">Source bundle target type for spell cost calculation.</param>
        /// <param name="casterEntity">Source caster entity behaviour for spell cost calculation.</param>
        /// <param name="absorbSpellPointsOut">Number of spell points absorbed. Only valid when returning true.</param>
        /// <returns>True if absorbed.</returns>
        bool TryAbsorption(IEntityEffect effect, TargetTypes targetType, DaggerfallEntity casterEntity, out int absorbSpellPointsOut)
        {
            absorbSpellPointsOut = 0;

            // Effect cannot be null
            if (effect == null)
                return false;

            // Currently only absorbing Destruction magic - not sure on status of absorbing other magic schools
            // This is to prevent something as benign as a self-heal from player being blocked and absorbed
            // With current design, absorption is checked for ALL incoming effects to entity so require some sanity checks
            if (effect.Properties.MagicSkill != DFCareer.MagicSkills.Destruction)
                return false;

            // Get casting cost for this effect
            // Costs are calculated as if target cast the spell, not the actual caster
            // Note that if player self-absorbs a spell this will be equal anyway
            int effectCastingCost = GetEffectCastingCost(effect, targetType, entityBehaviour.Entity);

            // The entity must have enough spell points free to absorb incoming effect
            int availableSpellPoints = entityBehaviour.Entity.MaxMagicka - entityBehaviour.Entity.CurrentMagicka;
            if (effectCastingCost > availableSpellPoints)
                return false;
            else
                absorbSpellPointsOut = effectCastingCost;

            // Handle effect-based absorption
            SpellAbsorption absorbEffect = FindIncumbentEffect<SpellAbsorption>() as SpellAbsorption;
            if (absorbEffect != null && TryEffectBasedAbsorption(effect, absorbEffect, entityBehaviour.Entity))
                return true;

            // Handle career-based absorption
            if (entityBehaviour.Entity.Career.SpellAbsorption != DFCareer.SpellAbsorptionFlags.None && TryCareerBasedAbsorption(effect, entityBehaviour.Entity))
                return true;

            // Handle persistant absorption (e.g. special advantage general/day/night or from weapon effects)
            if (entityBehaviour.Entity.IsAbsorbingSpells)
                return true;

            return false;
        }

        /// <summary>
        /// Tests incoming spell bundle for reflection.
        /// </summary>
        /// <param name="sourceBundle">Source bundle of spell to reflect.</param>
        /// <returns>True if reflected.</returns>
        bool TryReflection(EntityEffectBundle sourceBundle)
        {
            // Cannot reflect bundle more than once
            // Could increase this later to allow for limited "reflect volleys" with two reflecting entities and first one to fail save cops the spell
            if (sourceBundle.ReflectedCount > 0)
                return false;

            // Entity must be reflecting
            SpellReflection reflectEffect = FindIncumbentEffect<SpellReflection>() as SpellReflection;
            if (reflectEffect == null)
                return false;

            // Do not reflect spells from same caster onto self
            if (sourceBundle.CasterEntityBehaviour == EntityBehaviour)
                return false;

            // Roll for reflection chance
            if (reflectEffect.RollChance())
            {
                // Redirect source bundle back on caster entity
                // They will have all their usual processes to absorb or resist spell on arrival
                sourceBundle.IncrementReflectionCount();
                EntityEffectManager casterEffectManager = sourceBundle.CasterEntityBehaviour.GetComponent<EntityEffectManager>();
                casterEffectManager.AssignBundle(sourceBundle);

                // Output "Spell was reflected." when player is the one reflecting spell
                if (IsPlayerEntity)
                    DaggerfallUI.AddHUDText(TextManager.Instance.GetText(textDatabase, "spellReflected"));

                return true;
            }

            return false;
        }

        /// <summary>
        /// Tests incoming effect for spell resistance.
        /// </summary>
        /// <param name="sourceBundle">Source bundle of spell to reflect.</param>
        /// <returns>True if resisted.</returns>
        bool TryResistance(EntityEffectBundle sourceBundle)
        {
            // Entity must be resisting
            SpellResistance resistEffect = FindIncumbentEffect<SpellResistance>() as SpellResistance;
            if (resistEffect == null)
                return false;

            // Do not resist "caster only" self-cast spells
            // Allow to resist "other target" spells, e.g. when caught inside own AOE radius or spell reflected
            if (sourceBundle.Settings.TargetType == TargetTypes.CasterOnly)
                return false;

            // Roll for resistance chance
            if (resistEffect.RollChance())
            {
                // Output "Spell was resisted." when player is the one resisting spell
                if (IsPlayerEntity)
                    DaggerfallUI.AddHUDText(TextManager.Instance.GetText(textDatabase, "spellResisted"));

                return true;
            }

            return false;
        }

        int GetEffectCastingCost(IEntityEffect effect, TargetTypes targetType, DaggerfallEntity casterEntity)
        {
            int goldCost, spellPointCost;
            FormulaHelper.CalculateEffectCosts(effect, effect.Settings, out goldCost, out spellPointCost, casterEntity);
            spellPointCost = FormulaHelper.ApplyTargetCostMultiplier(spellPointCost, targetType);

            // Spells always cost at least 5 spell points
            // Otherwise it's possible for absorbs to make spell point pool go down as spell costs 5 but caster absorbs 0
            if (spellPointCost < 5)
                spellPointCost = 5;

            //Debug.LogFormat("Calculated {0} spell point cost for effect {1}", spellPointCost, effect.Key);

            return spellPointCost;
        }

        bool TryEffectBasedAbsorption(IEntityEffect effect, SpellAbsorption absorbEffect, DaggerfallEntity entity)
        {
            int chance = absorbEffect.Settings.ChanceBase + absorbEffect.Settings.ChancePlus * (int)Mathf.Floor(entity.Level / absorbEffect.Settings.ChancePerLevel);

            return Dice100.SuccessRoll(chance);
        }

        bool TryCareerBasedAbsorption(IEntityEffect effect, DaggerfallEntity entity)
        {
            // Always resists
            DFCareer.SpellAbsorptionFlags spellAbsorption = entity.Career.SpellAbsorption;
            if (spellAbsorption == DFCareer.SpellAbsorptionFlags.Always)
                return true;

            // Resist in darkness (inside building or dungeon or outside at night)
            // Use player for inside/outside context - everything is where the player is
            if (spellAbsorption == DFCareer.SpellAbsorptionFlags.InDarkness)
            {
                if (GameManager.Instance.PlayerEnterExit.IsPlayerInside)
                    return true;
                else if (DaggerfallUnity.Instance.WorldTime.Now.IsNight)
                    return true;
            }

            // Resist in light (outside during the day)
            if (spellAbsorption == DFCareer.SpellAbsorptionFlags.InLight)
            {
                if (!GameManager.Instance.PlayerEnterExit.IsPlayerInside && DaggerfallUnity.Instance.WorldTime.Now.IsDay)
                    return true;
            }

            return false;
        }

        #endregion

        #region Poisons

        /// <summary>
        /// Helper to create a classic poison effect bundle.
        /// </summary>
        /// <param name="poisonType">Classic poison type.</param>
        /// <returns>EntityEffectBundle.</returns>
        public EntityEffectBundle CreatePoison(Poisons poisonType)
        {
            EffectBundleSettings settings = new EffectBundleSettings()
            {
                Version = EntityEffectBroker.CurrentSpellVersion,
                BundleType = BundleTypes.Poison,
                Effects = new EffectEntry[] { new EffectEntry(PoisonEffect.GetClassicPoisonEffectKey(poisonType)) },
            };

            return new EntityEffectBundle(settings, entityBehaviour);
        }

        #endregion

        #region Diseases

        /// <summary>
        /// Helper to create a classic disease effect bundle.
        /// </summary>
        /// <param name="diseaseType">Classic disease type.</param>
        /// <returns>EntityEffectBundle.</returns>
        public EntityEffectBundle CreateDisease(Diseases diseaseType)
        {
            EffectBundleSettings settings = new EffectBundleSettings()
            {
                Version = EntityEffectBroker.CurrentSpellVersion,
                BundleType = BundleTypes.Disease,
                Effects = new EffectEntry[] { new EffectEntry(DiseaseEffect.GetClassicDiseaseEffectKey(diseaseType)) },
            };

            return new EntityEffectBundle(settings, entityBehaviour);
        }

        /// <summary>
        /// Helper to create a disease effect bundle from any effect key.
        /// This is just here for testing right now as no custom diseases exist.
        /// </summary>
        /// <param name="key">Effect key to use as infection.</param>
        /// <returns>EntityEffectBundle.</returns>
        public EntityEffectBundle CreateDisease(string key)
        {
            EffectBundleSettings settings = new EffectBundleSettings()
            {
                Version = EntityEffectBroker.CurrentSpellVersion,
                BundleType = BundleTypes.Disease,
                Effects = new EffectEntry[] { new EffectEntry(key) },
            };

            return new EntityEffectBundle(settings, entityBehaviour);
        }

        /// <summary>
        /// Helper to create stage one infection disease for vampirism.
        /// </summary>
        /// <returns>EntityEffectBundle.</returns>
        public EntityEffectBundle CreateVampirismDisease()
        {
            EffectBundleSettings settings = new EffectBundleSettings()
            {
                Version = EntityEffectBroker.CurrentSpellVersion,
                BundleType = BundleTypes.Disease,
                Effects = new EffectEntry[] { new EffectEntry(VampirismInfection.VampirismInfectionKey) },
            };

            return new EntityEffectBundle(settings, entityBehaviour);
        }

        public EntityEffectBundle CreateLycanthropyDisease(LycanthropyTypes infectionType)
        {
            string effectKey;
            switch (infectionType)
            {
                case LycanthropyTypes.Werewolf:
                    effectKey = WerewolfInfection.WerewolfInfectionKey;
                    break;
                case LycanthropyTypes.Wereboar:
                    effectKey = WereboarInfection.WereboarInfectionKey;
                    break;
                default:
                    throw new Exception("CreateLycanthropyInfection() infectionType cannot be LycanthropyTypes.None");
            }

            EffectBundleSettings settings = new EffectBundleSettings()
            {
                Version = EntityEffectBroker.CurrentSpellVersion,
                BundleType = BundleTypes.Disease,
                Effects = new EffectEntry[] { new EffectEntry(effectKey) },
            };

            return new EntityEffectBundle(settings, entityBehaviour);
        }

        /// <summary>
        /// Helper to create stage two curse of vampirism.
        /// </summary>
        /// <returns>EntityEffectBundle.</returns>
        public EntityEffectBundle CreateVampirismCurse()
        {
            EffectBundleSettings settings = new EffectBundleSettings()
            {
                Version = EntityEffectBroker.CurrentSpellVersion,
                BundleType = BundleTypes.None,
                Effects = new EffectEntry[] { new EffectEntry(VampirismEffect.VampirismCurseKey) },
            };

            return new EntityEffectBundle(settings, entityBehaviour);
        }

        /// <summary>
        /// Helper to create stage two curse of lycanthropy.
        /// </summary>
        /// <returns>EntityEffectBundle.</returns>
        public EntityEffectBundle CreateLycanthropyCurse()
        {
            EffectBundleSettings settings = new EffectBundleSettings()
            {
                Version = EntityEffectBroker.CurrentSpellVersion,
                BundleType = BundleTypes.None,
                Effects = new EffectEntry[] { new EffectEntry(LycanthropyEffect.LycanthropyCurseKey) },
            };

            return new EntityEffectBundle(settings, entityBehaviour);
        }

        public void CureDisease(Diseases disease)
        {
            // Find specific disease incumbent
            LiveEffectBundle[] bundles = GetDiseaseBundles();
            foreach (LiveEffectBundle bundle in bundles)
            {
                // Must have a live effect
                if (bundle.liveEffects == null || bundle.liveEffects.Count == 0)
                    continue;

                // Must be a disease effect
                if (!(bundle.liveEffects[0] is DiseaseEffect))
                    continue;

                // Must be correct type of disease effect
                DiseaseEffect effect = bundle.liveEffects[0] as DiseaseEffect;
                if (effect.ClassicDiseaseType == disease)
                {
                    effect.CureDisease();
                    Debug.LogFormat("Cured disease {0}", disease);
                }
            }
        }

        public void CureAllDiseases()
        {
            // Cure all disease bundles
            LiveEffectBundle[] bundles = GetDiseaseBundles();
            foreach (LiveEffectBundle bundle in bundles)
            {
                RemoveBundle(bundle);
                Debug.LogFormat("Removing disease bundle {0}", bundle.GetHashCode());
            }
        }

        int GetDiseaseCount()
        {
            int count = 0;
            foreach (LiveEffectBundle bundle in instancedBundles)
            {
                if (bundle.bundleType == BundleTypes.Disease)
                    count++;
            }

            return count;
        }

        LiveEffectBundle[] GetDiseaseBundles()
        {
            List<LiveEffectBundle> diseaseBundles = new List<LiveEffectBundle>();
            foreach (LiveEffectBundle bundle in instancedBundles)
            {
                if (bundle.bundleType == BundleTypes.Disease)
                    diseaseBundles.Add(bundle);
            }

            return diseaseBundles.ToArray();
        }

        int GetPoisonCount()
        {
            int count = 0;
            foreach (LiveEffectBundle bundle in instancedBundles)
            {
                if (bundle.bundleType == BundleTypes.Poison)
                    count++;
            }

            return count;
        }

        LiveEffectBundle[] GetPoisonBundles()
        {
            List<LiveEffectBundle> poisonBundles = new List<LiveEffectBundle>();
            foreach (LiveEffectBundle bundle in instancedBundles)
            {
                if (bundle.bundleType == BundleTypes.Poison)
                    poisonBundles.Add(bundle);
            }

            return poisonBundles.ToArray();
        }

        public void CureAllPoisons()
        {
            // Cure all poison bundles
            LiveEffectBundle[] bundles = GetPoisonBundles();
            foreach (LiveEffectBundle bundle in bundles)
            {
                RemoveBundle(bundle);
                Debug.LogFormat("Removing poison bundle {0}", bundle.GetHashCode());
            }
        }

        public void CureAllAttributes()
        {
            foreach (LiveEffectBundle bundle in instancedBundles)
            {
                foreach (IEntityEffect effect in bundle.liveEffects)
                {
                    (effect as BaseEntityEffect).CureAttributeDamage();
                }
            }
        }

        public void CureAllSkills()
        {
            foreach (LiveEffectBundle bundle in instancedBundles)
            {
                foreach (IEntityEffect effect in bundle.liveEffects)
                {
                    (effect as BaseEntityEffect).CureSkillDamage();
                }
            }
        }

        public bool HasDamagedAttributes()
        {
            foreach (LiveEffectBundle bundle in instancedBundles)
            {
                foreach (IEntityEffect effect in bundle.liveEffects)
                {
                    if (!(effect as BaseEntityEffect).AllAttributesHealed())
                        return true;
                }
            }

            return false;
        }

        public bool HasDamagedSkills()
        {
            foreach (LiveEffectBundle bundle in instancedBundles)
            {
                foreach (IEntityEffect effect in bundle.liveEffects)
                {
                    if (!(effect as BaseEntityEffect).AllSkillsHealed())
                        return true;
                }
            }

            return false;
        }

        public void CureAll()
        {
            DaggerfallEntity entity = entityBehaviour.Entity;
            entity.CurrentHealth = entity.MaxHealth;
            entity.CurrentFatigue = entity.MaxFatigue;
            entity.CurrentMagicka = entity.MaxMagicka;
            CureAllPoisons();
            CureAllDiseases();
            CureAllAttributes();
            CureAllSkills();
        }

        public bool HasVampirism()
        {
            return racialOverrideEffect is VampirismEffect;
        }

        public bool HasLycanthropy()
        {
            return racialOverrideEffect is LycanthropyEffect;
        }

        public bool IsTransformedLycanthrope()
        {
            if (HasLycanthropy())
                return (racialOverrideEffect as LycanthropyEffect).IsTransformed;
            else
                return false;
        }

        public void EndVampirism()
        {
            if (HasVampirism())
                (racialOverrideEffect as VampirismEffect).CureVampirism();
        }

        public void EndLycanthropy()
        {
            if (HasLycanthropy())
                (racialOverrideEffect as LycanthropyEffect).CureLycanthropy();
        }

        #endregion

        #region Static Helpers

        public static void BreakNormalPowerConcealmentEffects(DaggerfallEntityBehaviour entityBehaviour)
        {
            // Get entity effect manager
            EntityEffectManager manager = entityBehaviour.GetComponent<EntityEffectManager>();
            if (!manager)
                return;

            // End Chameleon-Normal
            if (entityBehaviour.Entity.HasConcealment(MagicalConcealmentFlags.BlendingNormal))
                manager.EndIncumbentEffect<ChameleonNormal>();

            // End Invisibility-Normal
            if (entityBehaviour.Entity.HasConcealment(MagicalConcealmentFlags.InvisibleNormal))
                manager.EndIncumbentEffect<InvisibilityNormal>();

            // End Shadow-Normal
            if (entityBehaviour.Entity.HasConcealment(MagicalConcealmentFlags.ShadeNormal))
                manager.EndIncumbentEffect<ShadowNormal>();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Tick constant effects on instanced bundles for this entity.
        /// </summary>
        void DoConstantEffects()
        {
            // Clear existing constant effects
            entityBehaviour.Entity.ClearConstantEffects();

            // Do nothing further if entity has perished or object disabled
            if (entityBehaviour.Entity.CurrentHealth <= 0 || !entityBehaviour.enabled)
                return;

            foreach (LiveEffectBundle bundle in instancedBundles)
            {
                foreach (IEntityEffect effect in bundle.liveEffects)
                {
                    // Update constant effects until ended
                    if (!effect.HasEnded)
                        effect.ConstantEffect();
                }
            }

            // MaxMagickaModifier stacks with no upper limit
            // This can cause player to lose current spellpoints when loading game or stacking effects using this modifier
            // So clamp spellpoints to current max only after applying all constant effects for this entity
            if (entityBehaviour.Entity.CurrentMagicka > entityBehaviour.Entity.MaxMagicka)
                entityBehaviour.Entity.CurrentMagicka = entityBehaviour.Entity.MaxMagicka;
        }

        /// <summary>
        /// Tick new "magic round" on all instanced bundles for this entity.
        /// </summary>
        void DoMagicRound()
        {
            // Clear direct mods
            Array.Clear(directStatMods, 0, DaggerfallStats.Count);
            Array.Clear(directSkillMods, 0, DaggerfallSkills.Count);
            if (IsPlayerEntity)
                (entityBehaviour.Entity as PlayerEntity).ClearReactionMods();

            // Do nothing further if no bundles, entity has perished, or object disabled
            if (instancedBundles.Count == 0 || entityBehaviour.Entity.CurrentHealth <= 0 || !entityBehaviour.enabled)
                return;

            // Run all bundles
            activeMagicItemsInRound.Clear();
            uint currentTime = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime();
            foreach (LiveEffectBundle bundle in instancedBundles)
            {
                // Run effects for this bundle
                bool hasRemainingEffectRounds = false;
                foreach (IEntityEffect effect in bundle.liveEffects)
                {
                    // Update effects with remaining rounds, item effects are always ticked
                    if (effect.RoundsRemaining > 0 || bundle.fromEquippedItem != null)
                    {
                        effect.MagicRound();
                        if (effect.RoundsRemaining > 0)
                            hasRemainingEffectRounds = true;
                    }
                }

                if (bundle.fromEquippedItem != null && bundle.fromEquippedItem.IsEquipped)
                {
                    // If bundle has an item source keep it alive until item breaks or is unequipped
                    hasRemainingEffectRounds = true;

                    if (IsPlayerEntity)
                    {
                        // Track individual held items for magic round callback
                        if (!activeMagicItemsInRound.ContainsKey(bundle.fromEquippedItem.UID))
                            activeMagicItemsInRound.Add(bundle.fromEquippedItem.UID, bundle.fromEquippedItem);

                        // Schedule items pending reroll
                        uint hoursSinceLastReroll = (currentTime - bundle.fromEquippedItem.timeEffectsLastRerolled) / DaggerfallDateTime.MinutesPerHour;
                        if (hoursSinceLastReroll >= rerollMinimumHours && !itemsPendingReroll.ContainsKey(bundle.fromEquippedItem.UID))
                            itemsPendingReroll.Add(bundle.fromEquippedItem.UID, bundle.fromEquippedItem);
                    }
                }

                // Expire this bundle once all effects have 0 rounds remaining
                if (!hasRemainingEffectRounds)
                    bundlesToRemove.Add(bundle);
            }

            // Do MagicRound payload callback for each item
            if (activeMagicItemsInRound.Count > 0)
            {
                foreach (DaggerfallUnityItem item in activeMagicItemsInRound.Values)
                {
                    DoItemEnchantmentPayloads(EnchantmentPayloadFlags.MagicRound, item);
                }
                activeMagicItemsInRound.Clear();
            }

            RemovePendingBundles();
        }

        void RemoveBundle(LiveEffectBundle bundle)
        {
            foreach (IEntityEffect effect in bundle.liveEffects)
            {
                effect.End();

                // Remove racial override cache
                if (effect is RacialOverrideEffect)
                    racialOverrideEffect = null;
            }

            instancedBundles.Remove(bundle);
            RaiseOnRemoveBundle(bundle);
            //Debug.LogFormat("Expired bundle {0} with {1} effects", bundle.settings.Name, bundle.settings.Effects.Length);
        }

        void RemovePendingBundles()
        {
            if (bundlesToRemove.Count > 0)
            {
                foreach (LiveEffectBundle bundle in bundlesToRemove)
                {
                    RemoveBundle(bundle);
                    Debug.LogFormat("Removing bundle [{0}] {1}", bundle.name, bundle.GetHashCode());
                }
                bundlesToRemove.Clear();
            }
        }

        void ClearReadySpellHistory()
        {
            lastSpell = null;
            readySpell = null;
            readySpellDoesNotCostSpellPoints = false;
        }

        public int GetCastSoundID(ElementTypes elementType)
        {
            switch (elementType)
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

        void UpdateEntityMods()
        {
            // Clear all mods
            Array.Clear(combinedStatMods, 0, DaggerfallStats.Count);
            Array.Clear(combinedStatMaxMods, 0, DaggerfallStats.Count);
            Array.Clear(combinedSkillMods, 0, DaggerfallSkills.Count);
            Array.Clear(combinedResistanceMods, 0, DaggerfallResistances.Count);

            // Add together every mod for every live effect
            foreach (LiveEffectBundle bundle in instancedBundles)
            {
                foreach (IEntityEffect effect in bundle.liveEffects)
                {
                    MergeStatMods(effect);
                    MergeStatMaxMods(effect);
                    MergeSkillMods(effect);
                    MergeResistanceMods(effect);
                }
            }

            // Add direct mods on this entity
            MergeDirectMods();

            // Assign to host entity
            entityBehaviour.Entity.Stats.AssignMods(combinedStatMods, combinedStatMaxMods);
            entityBehaviour.Entity.Skills.AssignMods(combinedSkillMods);
            entityBehaviour.Entity.Resistances.AssignMods(combinedResistanceMods);

            // Kill host if any stat is reduced to 1
            for (int i = 0; i < DaggerfallStats.Count; i++)
            {
                if (entityBehaviour.Entity.Stats.GetLiveStatValue(i) == 1)
                {
                    entityBehaviour.Entity.CurrentHealth = 0;
                    return;
                }
            }
        }

        void MergeStatMods(IEntityEffect effect)
        {
            for (int i = 0; i < effect.StatMods.Length; i++)
            {
                combinedStatMods[i] += effect.StatMods[i];
            }
        }

        void MergeStatMaxMods(IEntityEffect effect)
        {
            for (int i = 0; i < effect.StatMaxMods.Length; i++)
            {
                combinedStatMaxMods[i] += effect.StatMaxMods[i];
            }
        }

        void MergeSkillMods(IEntityEffect effect)
        {
            for (int i = 0; i < effect.SkillMods.Length; i++)
            {
                combinedSkillMods[i] += effect.SkillMods[i];
            }
        }

        void MergeResistanceMods(IEntityEffect effect)
        {
            for (int i = 0; i < effect.ResistanceMods.Length; i++)
            {
                combinedResistanceMods[i] += effect.ResistanceMods[i];
            }
        }

        void MergeDirectMods()
        {
            for (int i = 0; i < combinedStatMods.Length; i++)
            {
                combinedStatMods[i] += directStatMods[i];
            }

            for (int i = 0; i < combinedSkillMods.Length; i++)
            {
                combinedSkillMods[i] += directSkillMods[i];
            }
        }

        ulong GetCasterLoadID(DaggerfallEntityBehaviour caster)
        {
            // Only supporting LoadID from enemies at this time
            if (caster.EntityType == EntityTypes.EnemyMonster || caster.EntityType == EntityTypes.EnemyClass)
            {
                ISerializableGameObject serializableEnemy = caster.GetComponent<SerializableEnemy>() as ISerializableGameObject;
                return serializableEnemy.LoadID;
            }
            else
            {
                return 0;
            }
        }

        bool SilenceCheck()
        {
            if (entityBehaviour.Entity.IsSilenced)
            {
                // Output "You are silenced." if the host manager is player
                // Just to let them know why casting isn't working
                if (entityBehaviour == GameManager.Instance.PlayerEntityBehaviour)
                    DaggerfallUI.AddHUDText(TextManager.Instance.GetText(textDatabase, "youAreSilenced"), 1.5f);

                readySpell = null;
                return true;
            }

            return false;
        }

        public void PlayCastSound(DaggerfallEntityBehaviour casterEntityBehaviour, int castSoundID, bool throttle = false)
        {
            // Throttle casting sound to once per 0.5f seconds to prevent playing overlapping effects on item equip/recast
            if (throttle & Time.realtimeSinceStartup - timeLastCastSoundPlayed < 0.5f)
                return;

            if (casterEntityBehaviour)
            {
                DaggerfallAudioSource audioSource = casterEntityBehaviour.GetComponent<DaggerfallAudioSource>();
                if (castSoundID != -1 && audioSource)
                    audioSource.PlayOneShot((uint)castSoundID);
            }

            timeLastCastSoundPlayed = Time.realtimeSinceStartup;
        }

        void TallyPlayerReadySpellEffectSkills()
        {
            // Validate ready spell
            if (readySpell == null || readySpell.Settings.Effects == null)
                return;

            // Loop through effects in spell bundle and tally related magic skill
            // Normally spells will have no more than 3 effects
            for (int i = 0; i < readySpell.Settings.Effects.Length; i++)
            {
                IEntityEffect effect = GameManager.Instance.EntityEffectBroker.GetEffectTemplate(readySpell.Settings.Effects[i].Key);
                if (effect != null)
                    GameManager.Instance.PlayerEntity.TallySkill((DFCareer.Skills)effect.Properties.MagicSkill, 1);
            }
        }

        void PassiveSpecialsCheck()
        {
            // Do nothing if effect already found
            if (passiveSpecialsEffect != null)
                return;

            // Attempt to find effect
            passiveSpecialsEffect = (PassiveSpecialsEffect)FindIncumbentEffect<PassiveSpecialsEffect>();
            if (passiveSpecialsEffect != null)
                return;

            // Instantiate effect
            EffectBundleSettings settings = new EffectBundleSettings()
            {
                Version = EntityEffectBroker.CurrentSpellVersion,
                BundleType = BundleTypes.None,
                Effects = new EffectEntry[] { new EffectEntry(PassiveSpecialsEffect.EffectKey) },
            };
            AssignBundle(new EntityEffectBundle(settings, entityBehaviour), AssignBundleFlags.BypassSavingThrows);
        }

        void RerollItemEffects()
        {
            // Must have at least one item pending reroll
            if (itemsPendingReroll.Count == 0)
                return;

            // Recast enchantments in item flagged for reroll
            foreach (DaggerfallUnityItem item in itemsPendingReroll.Values)
            {
                Debug.LogFormat("Rerolling flagged item effects on {0}", item.LongName);

                // Schedule live bundles from this item to be removed
                foreach (LiveEffectBundle bundle in instancedBundles)
                {
                    if (bundle.fromEquippedItem != null && bundle.fromEquippedItem.UID == item.UID &&
                        (bundle.runtimeFlags & BundleRuntimeFlags.ItemRecastEnabled) == BundleRuntimeFlags.ItemRecastEnabled)
                    {
                        bundlesToRemove.Add(bundle);
                    }
                }
                RemovePendingBundles();

                if (item.IsEquipped)
                {
                    // Execute reroll callbacks on item if still equipped
                    DoItemEnchantmentPayloads(EnchantmentPayloadFlags.RerollEffect, item);

                    // Update recast time in item
                    item.timeEffectsLastRerolled = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime();
                }
            }

            // Clean up
            itemsPendingReroll.Clear();
        }

        #endregion

        #region EnemyCasting

        // For enemies this is equivalent to PlayerSpellCasting_OnReleaseFrame()
        // Might need to time cast to enemy release - whatever looks best
        void EnemyCastReadySpell()
        {
            // Must have a ready spell
            if (readySpell == null)
                return;

            // Play cast sound from caster audio source
            if (readySpell.CasterEntityBehaviour)
            {
                PlayCastSound(readySpell.CasterEntityBehaviour, GetCastSoundID(readySpell.Settings.ElementType));
            }

            // Create magic sparkles effect
            if (readySpell.Settings.TargetType != TargetTypes.SingleTargetAtRange &&
                readySpell.Settings.TargetType != TargetTypes.AreaAtRange)
            {
                EnemyBlood sparkles = readySpell.CasterEntityBehaviour.GetComponent<EnemyBlood>();

                CharacterController targetController = entityBehaviour.transform.GetComponent<CharacterController>();
                Vector3 sparklesPos = entityBehaviour.transform.position + targetController.center;
                sparklesPos.y += targetController.height / 8;

                if (sparkles)
                {
                    sparkles.ShowMagicSparkles(sparklesPos);
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
                DaggerfallMissile missile = InstantiateSpellMissile(readySpell.Settings.ElementType);
                if (missile)
                    missile.Payload = readySpell;
            }

            // Clear ready spell and reset casting - do not store last spell if casting from item
            RaiseOnCastReadySpell(readySpell);
            lastSpell = readySpell;
            readySpell = null;
            readySpellCastingCost = 0;
            instantCast = false;
            castInProgress = false;
            readySpellDoesNotCostSpellPoints = false;
        }

        #endregion  

        #region Event Handling

        private void PlayerSpellCasting_OnReleaseFrame()
        {
            // Must have a ready spell
            if (readySpell == null)
                return;

            // Always tally magic skills when player physically casts a spell
            // Cancelled spells do not reach this point
            TallyPlayerReadySpellEffectSkills();

            // Play cast sound from caster audio source
            if (readySpell.CasterEntityBehaviour)
            {
                PlayCastSound(readySpell.CasterEntityBehaviour, GetCastSoundID(readySpell.Settings.ElementType));
            }

            // Assign bundle directly to self if target is caster
            // Otherwise instatiate missile prefab based on element type
            if (readySpell.Settings.TargetType == TargetTypes.CasterOnly)
            {
                AssignBundle(readySpell);
            }
            else
            {
                DaggerfallMissile missile = InstantiateSpellMissile(readySpell.Settings.ElementType);
                if (missile)
                    missile.Payload = readySpell;
            }

            // Clear ready spell and reset casting - do not store last spell if casting from item
            RaiseOnCastReadySpell(readySpell);
            lastSpell = (readySpellDoesNotCostSpellPoints) ? null : readySpell;
            readySpell = null;
            readySpellCastingCost = 0;
            instantCast = false;
            castInProgress = false;
            readySpellDoesNotCostSpellPoints = false;
        }

        private void EntityEffectBroker_OnNewMagicRound()
        {
            DoMagicRound();
        }

        private void SaveLoadManager_OnStartLoad(SaveData_v1 saveData)
        {
            ClearReadySpellHistory();
            WipeAllBundles();
        }

        private void StartGameBehaviour_OnNewGame()
        {
            ClearReadySpellHistory();
            WipeAllBundles();
        }

        private void Entity_OnDeath(DaggerfallEntity entity)
        {
            wipeAllBundles = true;
            entityBehaviour.Entity.OnDeath -= Entity_OnDeath;
            //Debug.LogFormat("Cleared all effect bundles after death of {0}", entity.Name);
        }

        private void DaggerfallRestWindow_OnSleepEnd()
        {
            RerollItemEffects();
        }

        private void EntityEffectBroker_OnEndSyntheticTimeIncrease()
        {
            RerollItemEffects();
        }

        #endregion

        #region Serialization

        [fsObject("v1")]
        public struct EffectBundleSaveData_v1
        {
            public int version;
            public BundleTypes bundleType;
            public TargetTypes targetType;
            public ElementTypes elementType;
            public BundleRuntimeFlags runtimeFlags;
            public string name;
            public int iconIndex;
            public SpellIcon icon;
            public EntityTypes casterEntityType;
            public ulong casterLoadID;
            public ulong fromEquippedItemID;
            public ulong castByItemID;
            public bool fromPoison;
            public EffectSaveData_v1[] liveEffects;
        }

        [fsObject("v1")]
        public struct EffectSaveData_v1
        {
            public string key;
            public EffectSettings effectSettings;
            public EnchantmentParam? enchantmentParam;
            public int roundsRemaining;
            public bool chanceSuccess;
            public int[] statMods;
            public int[] statMaxMods;
            public int[] skillMods;
            public bool isIncumbent;
            public int variantCount;
            public int currentVariant;
            public bool effectEnded;
            public object effectSpecific;
        }

        /// <summary>
        /// Get instanced bundles save data.
        /// </summary>
        public EffectBundleSaveData_v1[] GetInstancedBundlesSaveData()
        {
            List<EffectBundleSaveData_v1> bundlesSaveData = new List<EffectBundleSaveData_v1>();
            foreach (LiveEffectBundle bundle in instancedBundles)
            {
                EffectBundleSaveData_v1 bundleData = new EffectBundleSaveData_v1();
                bundleData.version = bundle.version;
                bundleData.bundleType = bundle.bundleType;
                bundleData.targetType = bundle.targetType;
                bundleData.elementType = bundle.elementType;
                bundleData.runtimeFlags = bundle.runtimeFlags;
                bundleData.name = bundle.name;
                bundleData.iconIndex = bundle.iconIndex;
                bundleData.icon = bundle.icon;
                bundleData.casterEntityType = bundle.casterEntityType;
                bundleData.casterLoadID = bundle.casterLoadID;
                if (bundle.fromEquippedItem != null) bundleData.fromEquippedItemID = bundle.fromEquippedItem.UID;
                if (bundle.castByItem != null) bundleData.castByItemID = bundle.castByItem.UID;

                List<EffectSaveData_v1> liveEffectsSaveData = new List<EffectSaveData_v1>();
                foreach (IEntityEffect effect in bundle.liveEffects)
                {
                    EffectSaveData_v1 effectData = GetEffectSaveData(effect);
                    liveEffectsSaveData.Add(effectData);
                }

                bundleData.liveEffects = liveEffectsSaveData.ToArray();
                bundlesSaveData.Add(bundleData);
            }

            return bundlesSaveData.ToArray();
        }

        /// <summary>
        /// Get full effect save data including effect specific data.
        /// </summary>
        public EffectSaveData_v1 GetEffectSaveData(IEntityEffect effect)
        {
            EffectSaveData_v1 effectData = new EffectSaveData_v1();
            effectData.key = effect.Key;
            effectData.effectSettings = effect.Settings;
            effectData.enchantmentParam = effect.EnchantmentParam;
            effectData.roundsRemaining = effect.RoundsRemaining;
            effectData.chanceSuccess = effect.ChanceSuccess;
            effectData.statMods = effect.StatMods;
            effectData.statMaxMods = effect.StatMaxMods;
            effectData.skillMods = effect.SkillMods;
            effectData.isIncumbent = (effect is IncumbentEffect) ? (effect as IncumbentEffect).IsIncumbent : false;
            effectData.variantCount = effect.VariantCount;
            effectData.currentVariant = effect.CurrentVariant;
            effectData.effectEnded = effect.HasEnded;
            effectData.effectSpecific = effect.GetSaveData();

            return effectData;
        }

        /// <summary>
        /// Restore instanced bundles save data.
        /// </summary>
        public void RestoreInstancedBundleSaveData(EffectBundleSaveData_v1[] data)
        {
            WipeAllBundles();

            if (data == null || data.Length == 0)
                return;

            foreach (EffectBundleSaveData_v1 bundleData in data)
            {
                LiveEffectBundle instancedBundle = new LiveEffectBundle();
                instancedBundle.version = bundleData.version;
                instancedBundle.bundleType = bundleData.bundleType;
                instancedBundle.targetType = bundleData.targetType;
                instancedBundle.elementType = bundleData.elementType;
                instancedBundle.runtimeFlags = bundleData.runtimeFlags;
                instancedBundle.name = bundleData.name;
                instancedBundle.iconIndex = bundleData.iconIndex;
                instancedBundle.icon = bundleData.icon;
                instancedBundle.casterEntityType = bundleData.casterEntityType;
                instancedBundle.casterLoadID = bundleData.casterLoadID;
                instancedBundle.liveEffects = new List<IEntityEffect>();
                instancedBundle.caster = GetCasterReference(bundleData.casterEntityType, bundleData.casterLoadID);
                if (instancedBundle.caster)
                {
                    instancedBundle.fromEquippedItem = instancedBundle.caster.Entity.Items.GetItem(bundleData.fromEquippedItemID);
                    instancedBundle.castByItem = instancedBundle.caster.Entity.Items.GetItem(bundleData.castByItemID);
                }

                // If bundle is supposed to be an equipped item, and we did not find that item, then do not restore bundle
                if (instancedBundle.bundleType == BundleTypes.HeldMagicItem && instancedBundle.fromEquippedItem == null)
                    continue;

                // Migrate from old spell icon index
                // The old icon index will be changed into a SpellIcon with a null pack key
                if (string.IsNullOrEmpty(instancedBundle.icon.key) && instancedBundle.icon.index == 0)
                    instancedBundle.icon.index = instancedBundle.iconIndex;

                // Resume effects
                foreach (EffectSaveData_v1 effectData in bundleData.liveEffects)
                {
                    IEntityEffect effect = GameManager.Instance.EntityEffectBroker.InstantiateEffect(effectData.key, effectData.effectSettings);
                    if (effect == null)
                    {
                        Debug.LogWarningFormat("RestoreInstancedBundleSaveData() could not restore effect as key '{0}' was not found by broker.", effectData.key);
                        continue;
                    }

                    // Resume effect
                    effect.ParentBundle = instancedBundle;
                    effect.EnchantmentParam = effectData.enchantmentParam;
                    effect.Resume(effectData, this, instancedBundle.caster);
                    effect.RestoreSaveData(effectData.effectSpecific);
                    instancedBundle.liveEffects.Add(effect);

                    // Cache racial override effect
                    if (effect is RacialOverrideEffect)
                        racialOverrideEffect = (RacialOverrideEffect)effect;
                }

                // Do not instantiate bundle if no live effects restored
                // This can happen when effects are removed from game (e.g. mod removed or code refactor)
                if (instancedBundle.liveEffects.Count == 0)
                {
                    Debug.LogWarningFormat("RestoreInstancedBundleSaveData() not restoring bundle {0} as it restored no live effects.", instancedBundle.name);
                    continue;
                }

                instancedBundles.Add(instancedBundle);
            }
        }

        /// <summary>
        /// Helper to relink caster type and ID back to a real DaggerfallEntityBehaviour in scene.
        /// May experience concurrency issues once enemies start casting spells as very likely that
        /// player will save while under effect of a bundle cast by an enemy monster.
        /// Likewise possible for monster A and monster B to both catch each other in their AOEs and
        /// have a co-depdendency on each other as caster. So the first monster loaded will not be
        /// able to find reference for second monster as it has not been loaded yet.
        /// Already have strategies in mind to resolve this, depending on how bad problem is in practice.
        /// Don't want to "prematurely optimise" until this is actually a problem worth fixing.
        /// </summary>
        DaggerfallEntityBehaviour GetCasterReference(EntityTypes casterEntityType, ulong loadID)
        {
            DaggerfallEntityBehaviour caster = null;

            // Only supporting player and enemy entity types as casters for now
            if (casterEntityType == EntityTypes.Player)
            {
                caster = GameManager.Instance.PlayerEntityBehaviour;
            }
            else if ((casterEntityType == EntityTypes.EnemyMonster || casterEntityType == EntityTypes.EnemyClass) && loadID != 0)
            {
                SerializableEnemy serializableEnemy = SaveLoadManager.StateManager.GetEnemy(loadID);
                if (!serializableEnemy)
                {
                    Debug.LogWarning(string.Format("EntityEffect.RestoreEffectSaveData() could not find SerializableEnemy for LoadID {0} in StateManager.", loadID));
                    return null;
                }

                caster = serializableEnemy.GetComponent<DaggerfallEntityBehaviour>();
                if (!caster)
                    throw new Exception(string.Format("EntityEffect.RestoreEffectSaveData() could not find DaggerfallEntityBehaviour for LoadID {0} in StateManager.", loadID));
            }

            return caster;
        }

        #endregion

        #region Events

        // OnAssignBundle
        public delegate void OnAssignBundleEventHandler(LiveEffectBundle bundleAdded);
        public event OnAssignBundleEventHandler OnAssignBundle;
        protected virtual void RaiseOnAssignBundle(LiveEffectBundle bundleAdded)
        {
            if (OnAssignBundle != null)
                OnAssignBundle(bundleAdded);
        }

        // OnRemoveBundle
        public delegate void OnRemoveBundleEventHandler(LiveEffectBundle bundleRemoved);
        public event OnRemoveBundleEventHandler OnRemoveBundle;
        protected virtual void RaiseOnRemoveBundle(LiveEffectBundle bundleRemoved)
        {
            if (OnRemoveBundle != null)
                OnRemoveBundle(bundleRemoved);
        }

        // OnAddIncumbentState
        public delegate void OnAddIncumbentStateEventHandler();
        public event OnAddIncumbentStateEventHandler OnAddIncumbentState;
        protected virtual void RaiseOnAddIncumbentState()
        {
            if (OnAddIncumbentState != null)
                OnAddIncumbentState();
        }

        // OnNewReadySpell
        public delegate void OnNewReadySpellEventHandler(EntityEffectBundle spell);
        public event OnNewReadySpellEventHandler OnNewReadySpell;
        protected virtual void RaiseOnNewReadySpell(EntityEffectBundle spell)
        {
            if (OnNewReadySpell != null)
                OnNewReadySpell(spell);
        }

        // OnCastReadySpell
        public delegate void OnCastReadySpellEventHandler(EntityEffectBundle spell);
        public event OnCastReadySpellEventHandler OnCastReadySpell;
        protected virtual void RaiseOnCastReadySpell(EntityEffectBundle spell)
        {
            if (OnCastReadySpell != null)
                OnCastReadySpell(spell);
        }

        #endregion
    }
}
