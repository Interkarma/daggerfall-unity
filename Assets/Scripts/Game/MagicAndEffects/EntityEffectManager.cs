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
using DaggerfallConnect;
using DaggerfallConnect.FallExe;
using DaggerfallConnect.Save;
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
        bool castInProgress = false;
        bool readySpellIsMagicItem = false;

        DaggerfallEntityBehaviour entityBehaviour = null;
        bool isPlayerEntity = false;

        List<InstancedBundle> instancedBundles = new List<InstancedBundle>();
        List<InstancedBundle> bundlesToRemove = new List<InstancedBundle>();
        bool wipeAllBundles = false;

        int[] directStatMods = new int[DaggerfallStats.Count];
        int[] directSkillMods = new int[DaggerfallSkills.Count];
        int[] combinedStatMods = new int[DaggerfallStats.Count];
        int[] combinedSkillMods = new int[DaggerfallSkills.Count];
        int[] combinedResistanceMods = new int[DaggerfallResistances.Count];
        float refreshModsTimer = 0;
        const float refreshModsDelay = 0.2f;

        #endregion

        #region Structs

        /// <summary>
        /// Stores an instanced effect bundle for executing effects.
        /// </summary>
        public struct InstancedBundle
        {
            public int version;
            public BundleTypes bundleType;
            public TargetTypes targetType;
            public ElementTypes elementType;
            public string name;
            public int iconIndex;
            public DaggerfallEntityBehaviour caster;
            public EntityTypes casterEntityType;
            public ulong casterLoadID;
            public DaggerfallUnityItem fromEquippedItem;
            public List<IEntityEffect> liveEffects;
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

        public int EffectCount
        {
            get { return instancedBundles.Count; }
        }

        public int DiseaseCount
        {
            get { return GetDiseaseCount(); }
        }

        public InstancedBundle[] DiseaseBundles
        {
            get { return GetDiseaseBundles(); }
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

            // TODO: Allow enemies to cast their spells
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Assigns a new spell to be cast.
        /// For player entity, this will display "press button to fire spell" message.
        /// </summary>
        public void SetReadySpell(EntityEffectBundle spell, bool isMagicItem = false)
        {
            // Do nothing if silenced
            if (SilenceCheck())
                return;

            // Spell must appear valid
            if (spell == null || spell.Settings.Version < minAcceptedSpellVersion)
                return;

            // Assign spell - caster only spells are cast instantly
            readySpell = spell;
            readySpellIsMagicItem = isMagicItem;
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
            readySpellIsMagicItem = false;
        }

        public void CastReadySpell()
        {
            // Do nothing if silenced
            if (SilenceCheck())
                return;

            // Must have a ready spell and a previous cast must not be in progress
            if (readySpell == null || castInProgress)
                return;

            // Get spellpoint costs of this spell
            int totalGoldCost, totalSpellPointCost;
            FormulaHelper.CalculateTotalEffectCosts(readySpell.Settings.Effects, readySpell.Settings.TargetType, out totalGoldCost, out totalSpellPointCost);

            // TODO: Enforce spellpoint costs - all spells are free to cast right now, even at 0 mana
            // This is to allow for easier testing during build-out stages

            // Deduct spellpoint cost from entity if not using a magic item
            if (!readySpellIsMagicItem)
                entityBehaviour.Entity.DecreaseMagicka(totalSpellPointCost);

            // Play casting animation based on element type
            // Spell is released by event handler PlayerSpellCasting_OnReleaseFrame
            // TODO: Do not need to show spellcasting animations for certain spell effects
            GameManager.Instance.PlayerSpellCasting.PlayOneShot(readySpell.Settings.ElementType);

            // Block further casting attempts until previous cast is complete
            castInProgress = true;
        }

        public void AssignBundle(EntityEffectBundle sourceBundle, bool showNonPlayerFailures = false)
        {
            // Source bundle must have one or more effects
            if (sourceBundle.Settings.Effects == null || sourceBundle.Settings.Effects.Length == 0)
            {
                Debug.LogWarning("AssignBundle() could not assign bundle as source has no effects");
                return;
            }

            // Create new instanced bundle and copy settings from source bundle
            InstancedBundle instancedBundle = new InstancedBundle();
            instancedBundle.version = sourceBundle.Settings.Version;
            instancedBundle.bundleType = sourceBundle.Settings.BundleType;
            instancedBundle.targetType = sourceBundle.Settings.TargetType;
            instancedBundle.elementType = sourceBundle.Settings.ElementType;
            instancedBundle.name = sourceBundle.Settings.Name;
            instancedBundle.iconIndex = sourceBundle.Settings.IconIndex;
            instancedBundle.fromEquippedItem = sourceBundle.FromEquippedItem;
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
                    Debug.LogWarningFormat("AssignBundle() could not add effect as key '{0}' was not found by broker.");
                    continue;
                }

                // Set bundle group
                effect.BundleGroup = sourceBundle.Settings.BundleType;

                // Spell absorption - must have a caster entity set
                if (sourceBundle.CasterEntityBehaviour)
                {
                    int absorbSpellPoints;
                    if (TryAbsorption(effect, sourceBundle.Settings.TargetType, sourceBundle.CasterEntityBehaviour.Entity, out absorbSpellPoints))
                    {
                        // Spell passed all checks and was absorbed - return cost output to target
                        entityBehaviour.Entity.IncreaseMagicka(absorbSpellPoints);

                        // Output "Spell was absorbed."
                        DaggerfallUI.AddHUDText(TextManager.Instance.GetText(textDatabase, "spellAbsorbed"));

                        continue;
                    }
                }

                // Start effect
                effect.Start(this, sourceBundle.CasterEntityBehaviour);

                // Do not proceed if chance failed
                if (effect.Properties.SupportChance &&
                    effect.Properties.ChanceFunction == ChanceFunction.OnCast &&
                    !effect.ChanceSuccess)
                {
                    // Output failure messages
                    if (isPlayerEntity && sourceBundle.Settings.TargetType == TargetTypes.CasterOnly)
                    {
                        // Output "Spell effect failed." for caster only spells
                        DaggerfallUI.AddHUDText(TextManager.Instance.GetText(textDatabase, "spellEffectFailed"));
                    }
                    else if (isPlayerEntity || showNonPlayerFailures)
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
                    RaiseOnAssignBundle();
                    continue;
                }

                // Add effect
                instancedBundle.liveEffects.Add(effect);

                // At this point effect is ready and gets initial magic round
                effect.MagicRound();
            }

            // Add bundles with at least one effect
            if (instancedBundle.liveEffects.Count > 0)
            {
                instancedBundles.Add(instancedBundle);
                RaiseOnAssignBundle();
                Debug.LogFormat("Adding bundle {0}", instancedBundle.GetHashCode());
            }
        }

        /// <summary>
        /// Searches all effects in all bundles to find incumbent of type T.
        /// </summary>
        /// <typeparam name="T">Found incumbent effect of type T or null.</typeparam>
        public IEntityEffect FindIncumbentEffect<T>()
        {
            foreach (InstancedBundle bundle in instancedBundles)
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
        /// Searches all effects in all bundles to find DrainEffect incumbent for a specific stat.
        /// </summary>
        /// <param name="drainStat">The stat being drained.</param>
        /// <returns>DrainEffect incumbent for drainStat, or null if not found.</returns>
        public DrainEffect FindDrainStatIncumbent(DFCareer.Stats drainStat)
        {
            foreach (InstancedBundle bundle in instancedBundles)
            {
                foreach (IEntityEffect effect in bundle.liveEffects)
                {
                    if (effect is DrainEffect)
                    {
                        DrainEffect drainEffect = effect as DrainEffect;
                        if (drainEffect.IsIncumbent && drainEffect.DrainStat == drainStat)
                            return drainEffect;
                    }
                }
            }

            return null;
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
            RaiseOnRemoveBundle();
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

        public void ClearSpellBundles()
        {
            foreach (InstancedBundle bundle in instancedBundles)
            {
                // Expire spell bundles
                if (bundle.bundleType == BundleTypes.Spell)
                    bundlesToRemove.Add(bundle);
            }

            RemovePendingBundles();
        }

        #endregion

        #region Magic Items

        /// <summary>
        /// Handles any magic-related work of equipping an item to this entity.
        /// Does nothing if item contains no "cast when held" enchantments.
        /// </summary>
        /// <param name="item">Item just equipped.</param>
        public void StartEquippedItem(DaggerfallUnityItem item)
        {
            // Item must have enchancements
            if (item == null || !item.IsEnchanted)
                return;

            // Equipped items must have "cast when held" enchantments
            DaggerfallEnchantment[] enchantments = item.Enchantments;
            foreach (DaggerfallEnchantment enchantment in enchantments)
            {
                if (enchantment.type == EnchantmentTypes.CastWhenHeld)
                {
                    SpellRecord.SpellRecordData spell;
                    if (GameManager.Instance.EntityEffectBroker.GetClassicSpellRecord(enchantment.param, out spell))
                    {
                        //Debug.LogFormat("EntityEffectManager.StartEquippedItem: Found CastWhenHeld enchantment '{0}'", spell.spellName);

                        // Create effect bundle settings from classic spell
                        EffectBundleSettings bundleSettings = new EffectBundleSettings();
                        if (!GameManager.Instance.EntityEffectBroker.ClassicSpellRecordDataToEffectBundleSettings(spell, BundleTypes.HeldMagicItem, out bundleSettings))
                            continue;

                        // Assign bundle
                        EntityEffectBundle bundle = new EntityEffectBundle(bundleSettings, entityBehaviour);
                        bundle.FromEquippedItem = item;
                        AssignBundle(bundle);

                        // Play cast sound on equip for player only
                        if (isPlayerEntity)
                            PlayCastSound(entityBehaviour, GetCastSoundID(bundle.Settings.ElementType));

                        // TODO: Use correct icon - the index in spell record data is the not the icon displayed by classic
                        // Not sure how this is determined by classic for equipped items, but it is consistent

                        // TODO: Apply durability loss to equipped item on equip and over time
                        // http://en.uesp.net/wiki/Daggerfall:Magical_Items#Durability_of_Magical_Items
                    }
                }
            }
        }

        /// <summary>
        /// Handles any magic-related work of unequipping an item from this entity
        /// </summary>
        /// <param name="item">Item just unequipped.</param>
        public void StopEquippedItem(DaggerfallUnityItem item)
        {
            // Item must have enchancements
            if (item == null || !item.IsEnchanted)
                return;

            // Check all running bundles for any linked to this item and schedule instant removal
            foreach (InstancedBundle bundle in instancedBundles)
            {
                if (bundle.fromEquippedItem != null && bundle.fromEquippedItem.UID == item.UID)
                    bundlesToRemove.Add(bundle);
            }
        }

        /// <summary>
        /// Offers item to effect manager when used by player in inventory.
        /// </summary>
        /// <param name="item">Item just used.</param>
        public void UseItem(DaggerfallUnityItem item)
        {
            // Item must have enchancements
            if (item == null || !item.IsEnchanted)
                return;

            // Cast first "cast when used" enchantment
            // This works by sending effect to readySpell which currently cannot queue more than one spell
            // Not sure how classic handles multiple "cast when used" effects, especially for "press to release" styled spells
            DaggerfallEnchantment[] enchantments = item.Enchantments;
            foreach (DaggerfallEnchantment enchantment in enchantments)
            {
                if (enchantment.type == EnchantmentTypes.CastWhenUsed)
                {
                    SpellRecord.SpellRecordData spell;
                    if (GameManager.Instance.EntityEffectBroker.GetClassicSpellRecord(enchantment.param, out spell))
                    {
                        //Debug.LogFormat("EntityEffectManager.UseItem: Found CastWhenUsed enchantment '{0}'", spell.spellName);

                        // Create effect bundle settings from classic spell
                        EffectBundleSettings bundleSettings = new EffectBundleSettings();
                        if (!GameManager.Instance.EntityEffectBroker.ClassicSpellRecordDataToEffectBundleSettings(spell, BundleTypes.Spell, out bundleSettings))
                            continue;

                        // Assign bundle to ready spell 
                        EntityEffectBundle bundle = new EntityEffectBundle(bundleSettings, entityBehaviour);
                        SetReadySpell(bundle, true);

                        // TODO: Apply durability loss to used item on use
                        // http://en.uesp.net/wiki/Daggerfall:Magical_Items#Durability_of_Magical_Items
                    }

                    break;
                }
            }
        }

        /// <summary>
        /// Assigns "cast when strikes" effects to this manager.
        /// </summary>
        /// <param name="item">Item striking this entity.</param>
        /// <param name="caster">Entity attacking with item.</param>
        public void StrikeWithItem(DaggerfallUnityItem item, DaggerfallEntityBehaviour caster)
        {
            // Item must have enchancements
            if (item == null || !item.IsEnchanted)
                return;

            // Create bundle for every "cast when strikes" enchantment
            List<EntityEffectBundle> bundles = new List<EntityEffectBundle>();
            DaggerfallEnchantment[] enchantments = item.Enchantments;
            foreach (DaggerfallEnchantment enchantment in enchantments)
            {
                if (enchantment.type == EnchantmentTypes.CastWhenStrikes)
                {
                    SpellRecord.SpellRecordData spell;
                    if (GameManager.Instance.EntityEffectBroker.GetClassicSpellRecord(enchantment.param, out spell))
                    {
                        //Debug.LogFormat("EntityEffectManager.StrikeWithItem: Found CastWhenStrikes enchantment '{0}'", spell.spellName);

                        // Create effect bundle settings from classic spell
                        EffectBundleSettings bundleSettings = new EffectBundleSettings();
                        if (!GameManager.Instance.EntityEffectBroker.ClassicSpellRecordDataToEffectBundleSettings(spell, BundleTypes.Spell, out bundleSettings))
                            continue;

                        // Assign bundle to list
                        EntityEffectBundle bundle = new EntityEffectBundle(bundleSettings, entityBehaviour);
                        bundle.CasterEntityBehaviour = caster;
                        bundles.Add(bundle);

                        // TODO: Apply durability loss to used item on strike
                        // http://en.uesp.net/wiki/Daggerfall:Magical_Items#Durability_of_Magical_Items
                    }
                }
            }

            // Assign bundles to this entity
            foreach (EntityEffectBundle bundle in bundles)
            {
                AssignBundle(bundle, true);
            }
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

            // Check if entity has an absorb incumbent running
            SpellAbsorption absorbEffect = FindIncumbentEffect<SpellAbsorption>() as SpellAbsorption;
            if (absorbEffect != null)
                return TryEffectBasedAbsorption(effect, absorbEffect, casterEntity);

            // Handle career-based absorption
            if (entityBehaviour.Entity.Career.SpellAbsorption != DFCareer.SpellAbsorptionFlags.None)
                return TryCareerBasedAbsorption(effect, casterEntity);

            return false;
        }

        int GetEffectCastingCost(IEntityEffect effect, TargetTypes targetType, DaggerfallEntity casterEntity)
        {
            int goldCost, spellPointCost;
            FormulaHelper.CalculateEffectCosts(effect, effect.Settings, out goldCost, out spellPointCost, casterEntity);
            spellPointCost = FormulaHelper.ApplyTargetCostMultiplier(spellPointCost, targetType);

            //Debug.LogFormat("Calculated {0} spell point cost for effect {1}", spellPointCost, effect.Key);

            return spellPointCost;
        }

        bool TryEffectBasedAbsorption(IEntityEffect effect, SpellAbsorption absorbEffect, DaggerfallEntity casterEntity)
        {
            return RollAbsorptionChance(absorbEffect, casterEntity);
        }

        bool TryCareerBasedAbsorption(IEntityEffect effect, DaggerfallEntity casterEntity)
        {
            // Always resists
            DFCareer.SpellAbsorptionFlags spellAbsorption = casterEntity.Career.SpellAbsorption;
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

        bool RollAbsorptionChance(SpellAbsorption absorbEffect, DaggerfallEntity casterEntity)
        {
            int chance = absorbEffect.Settings.ChanceBase + absorbEffect.Settings.ChancePlus * (int)Mathf.Floor(casterEntity.Level / absorbEffect.Settings.ChancePerLevel);
            int roll = UnityEngine.Random.Range(1, 100);

            return (roll <= chance);
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

        public void CureDisease(Diseases disease)
        {
            // Find specific disease incumbent
            InstancedBundle[] bundles = GetDiseaseBundles();
            foreach (InstancedBundle bundle in bundles)
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
            InstancedBundle[] bundles = GetDiseaseBundles();
            foreach (InstancedBundle bundle in bundles)
            {
                RemoveBundle(bundle);
                Debug.LogFormat("Removing disease bundle {0}", bundle.GetHashCode());
            }
        }

        int GetDiseaseCount()
        {
            int count = 0;
            foreach (InstancedBundle bundle in instancedBundles)
            {
                if (bundle.bundleType == BundleTypes.Disease)
                    count++;
            }

            return count;
        }

        InstancedBundle[] GetDiseaseBundles()
        {
            List<InstancedBundle> diseaseBundles = new List<InstancedBundle>();
            foreach (InstancedBundle bundle in instancedBundles)
            {
                if (bundle.bundleType == BundleTypes.Disease)
                    diseaseBundles.Add(bundle);
            }

            return diseaseBundles.ToArray();
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
            // Do nothing further if entity has perished or object disabled
            if (entityBehaviour.Entity.CurrentHealth <= 0 || !entityBehaviour.enabled)
                return;

            foreach (InstancedBundle bundle in instancedBundles)
            {
                foreach (IEntityEffect effect in bundle.liveEffects)
                {
                    // Update constant effects until ended
                    if (!effect.HasEnded)
                        effect.ConstantEffect();
                }
            }
        }

        /// <summary>
        /// Tick new "magic round" on all instanced bundles for this entity.
        /// </summary>
        void DoMagicRound()
        {
            // Do nothing further if entity has perished or object disabled
            if (entityBehaviour.Entity.CurrentHealth <= 0 || !entityBehaviour.enabled)
                return;

            // Run all bundles
            foreach(InstancedBundle bundle in instancedBundles)
            {
                // Run effects for this bundle
                bool hasRemainingEffectRounds = false;
                foreach (IEntityEffect effect in bundle.liveEffects)
                {
                    // Update effects with remaining rounds
                    if (effect.RoundsRemaining > 0)
                    {
                        effect.MagicRound();
                        if (effect.RoundsRemaining > 0)
                            hasRemainingEffectRounds = true;
                    }
                }

                // If bundle has an item source keep it alive until item breaks or is unequipped
                if (bundle.fromEquippedItem != null)
                {
                    hasRemainingEffectRounds = true;

                    // TODO: Manage item damage the longer it is equipped
                    // See http://en.uesp.net/wiki/Daggerfall:Magical_Items#Durability_of_Magical_Items
                }

                // Expire this bundle once all effects have 0 rounds remaining
                if (!hasRemainingEffectRounds)
                    bundlesToRemove.Add(bundle);
            }

            RemovePendingBundles();
        }

        void RemoveBundle(InstancedBundle bundle)
        {
            foreach (IEntityEffect effect in bundle.liveEffects)
                effect.End();

            instancedBundles.Remove(bundle);
            RaiseOnRemoveBundle();
            //Debug.LogFormat("Expired bundle {0} with {1} effects", bundle.settings.Name, bundle.settings.Effects.Length);
        }

        void RemovePendingBundles()
        {
            if (bundlesToRemove.Count > 0)
            {
                foreach (InstancedBundle bundle in bundlesToRemove)
                {
                    RemoveBundle(bundle);
                    Debug.LogFormat("Removing bundle {0}", bundle.GetHashCode());
                }
                bundlesToRemove.Clear();
            }
        }

        void ClearReadySpellHistory()
        {
            lastSpell = null;
            readySpell = null;
            readySpellIsMagicItem = false;
        }
        
        int GetCastSoundID(ElementTypes elementType)
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
                foreach (IEntityEffect effect in bundle.liveEffects)
                {
                    MergeStatMods(effect, ref combinedStatMods);
                    MergeSkillMods(effect, ref combinedSkillMods);
                }
            }

            // Add direct mods on this entity
            MergeDirectMods();

            // Assign to host entity
            entityBehaviour.Entity.Stats.AssignMods(combinedStatMods);
            entityBehaviour.Entity.Skills.AssignMods(combinedSkillMods);

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

        void PlayCastSound(DaggerfallEntityBehaviour casterEntityBehaviour, int castSoundID)
        {
            if (casterEntityBehaviour)
            {
                DaggerfallAudioSource audioSource = casterEntityBehaviour.GetComponent<DaggerfallAudioSource>();
                if (castSoundID != -1 && audioSource)
                    audioSource.PlayOneShot((uint)castSoundID);
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
                DaggerfallMissile missile = InstantiateMissile(readySpell.Settings.ElementType);
                if (missile)
                    missile.Payload = readySpell;
            }

            // Clear ready spell and reset casting - do not store last spell if casting from item
            lastSpell = (readySpellIsMagicItem) ? null : readySpell;
            readySpell = null;
            instantCast = false;
            castInProgress = false;
            readySpellIsMagicItem = false;
        }

        private void EntityEffectBroker_OnNewMagicRound()
        {
            // Clear direct mods
            Array.Clear(directStatMods, 0, DaggerfallStats.Count);
            Array.Clear(directSkillMods, 0, DaggerfallSkills.Count);

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

        #endregion

        #region Serialization

        [fsObject("v1")]
        public struct EffectBundleSaveData_v1
        {
            public int version;
            public BundleTypes bundleType;
            public TargetTypes targetType;
            public ElementTypes elementType;
            public string name;
            public int iconIndex;
            public EntityTypes casterEntityType;
            public ulong casterLoadID;
            public ulong fromEquippedItemID;
            public EffectSaveData_v1[] liveEffects;
        }

        [fsObject("v1")]
        public struct EffectSaveData_v1
        {
            public string key;
            public EffectSettings effectSettings;
            public int roundsRemaining;
            public bool chanceSuccess;
            public int[] statMods;
            public int[] skillMods;
            public BundleTypes bundleGroup;
            public bool isIncumbent;
            public object effectSpecific;
        }

        /// <summary>
        /// Get instanced bundles save data.
        /// </summary>
        public EffectBundleSaveData_v1[] GetInstancedBundlesSaveData()
        {
            List<EffectBundleSaveData_v1> bundlesSaveData = new List<EffectBundleSaveData_v1>();
            foreach (InstancedBundle bundle in instancedBundles)
            {
                EffectBundleSaveData_v1 bundleData = new EffectBundleSaveData_v1();
                bundleData.version = bundle.version;
                bundleData.bundleType = bundle.bundleType;
                bundleData.targetType = bundle.targetType;
                bundleData.elementType = bundle.elementType;
                bundleData.name = bundle.name;
                bundleData.iconIndex = bundle.iconIndex;
                bundleData.casterEntityType = bundle.casterEntityType;
                bundleData.casterLoadID = bundle.casterLoadID;
                if (bundle.fromEquippedItem != null) bundleData.fromEquippedItemID = bundle.fromEquippedItem.UID;

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
            effectData.roundsRemaining = effect.RoundsRemaining;
            effectData.chanceSuccess = effect.ChanceSuccess;
            effectData.statMods = effect.StatMods;
            effectData.skillMods = effect.SkillMods;
            effectData.bundleGroup = effect.BundleGroup;
            effectData.isIncumbent = (effect is IncumbentEffect) ? (effect as IncumbentEffect).IsIncumbent : false;
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

            foreach(EffectBundleSaveData_v1 bundleData in data)
            {
                InstancedBundle instancedBundle = new InstancedBundle();
                instancedBundle.version = bundleData.version;
                instancedBundle.bundleType = bundleData.bundleType;
                instancedBundle.targetType = bundleData.targetType;
                instancedBundle.elementType = bundleData.elementType;
                instancedBundle.name = bundleData.name;
                instancedBundle.iconIndex = bundleData.iconIndex;
                instancedBundle.casterEntityType = bundleData.casterEntityType;
                instancedBundle.casterLoadID = bundleData.casterLoadID;
                instancedBundle.liveEffects = new List<IEntityEffect>();
                instancedBundle.caster = GetCasterReference(bundleData.casterEntityType, bundleData.casterLoadID);
                if (instancedBundle.caster)
                    instancedBundle.fromEquippedItem = instancedBundle.caster.Entity.Items.GetItem(bundleData.fromEquippedItemID);

                // If bundle is supposed to be an equipped item, and we did not find that item, then do not restore bundle
                if (instancedBundle.bundleType == BundleTypes.HeldMagicItem && instancedBundle.fromEquippedItem == null)
                    continue;

                // Resume effects
                foreach(EffectSaveData_v1 effectData in bundleData.liveEffects)
                {
                    IEntityEffect effect = GameManager.Instance.EntityEffectBroker.InstantiateEffect(effectData.key, effectData.effectSettings);
                    if (effect == null)
                    {
                        Debug.LogWarningFormat("RestoreInstancedBundleSaveData() could not restore effect as key '{0}' was not found by broker.", effectData.key);
                        continue;
                    }

                    // Resume effect
                    effect.Resume(effectData, this, instancedBundle.caster);
                    effect.RestoreSaveData(effectData.effectSpecific);

                    instancedBundle.liveEffects.Add(effect);
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
        DaggerfallEntityBehaviour GetCasterReference(EntityTypes entityType, ulong loadID)
        {
            DaggerfallEntityBehaviour caster = null;

            // Only supporting player and enemy entity types as casters for now
            if (entityType == EntityTypes.Player)
            {
                caster = GameManager.Instance.PlayerEntityBehaviour;
            }
            else if ((entityType == EntityTypes.EnemyMonster || entityType == EntityTypes.EnemyClass) && loadID != 0)
            {
                SerializableEnemy serializableEnemy = SaveLoadManager.StateManager.GetEnemy(loadID);
                if (!serializableEnemy)
                    throw new Exception(string.Format("EntityEffect.RestoreEffectSaveData() could not find SerializableEnemy for LoadID {0} in StateManager.", loadID));

                caster = serializableEnemy.GetComponent<DaggerfallEntityBehaviour>();
                if (!caster)
                    throw new Exception(string.Format("EntityEffect.RestoreEffectSaveData() could not find DaggerfallEntityBehaviour for LoadID {0} in StateManager.", loadID));
            }

            return caster;
        }

        #endregion

        #region Events

        // OnAssignBundle
        public delegate void OnAssignBundleEventHandler();
        public event OnAssignBundleEventHandler OnAssignBundle;
        protected virtual void RaiseOnAssignBundle()
        {
            if (OnAssignBundle != null)
                OnAssignBundle();
        }

        // OnRemoveBundle
        public delegate void OnRemoveBundleEventHandler();
        public event OnRemoveBundleEventHandler OnRemoveBundle;
        protected virtual void RaiseOnRemoveBundle()
        {
            if (OnRemoveBundle != null)
                OnRemoveBundle();
        }

        // OnAddIncumbentState
        public delegate void OnAddIncumbentStateEventHandler();
        public event OnAddIncumbentStateEventHandler OnAddIncumbentState;
        protected virtual void RaiseOnAddIncumbentState()
        {
            if (OnAddIncumbentState != null)
                OnAddIncumbentState();
        }

        #endregion
    }
}