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
using System.Collections.Generic;
using DaggerfallConnect.FallExe;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Uber-effect used to deliver unique item powers and side-effects to entities.
    /// Not incumbent as most item powers are stackable and entity could have multiple instances of this effect running.
    /// NOTES:
    ///  * This effect is a work in progress and will be added to over time.
    /// </summary>
    public class PassiveItemSpecialsEffect : BaseEntityEffect
    {
        #region Fields

        public static readonly string EffectKey = "Passive-Item-Specials";

        const float nearbyRadius = 18f;             // Reasonably matched to classic with testing
        const int potentVsDamage = 5;               // Setting this to a small amount for now
        const float vampiricDrainRange = 2.25f;     // Testing classic shows range of vampiric effect items is approx. melee distance
        const int regenerateAmount = 1;
        const int regeneratePerRounds = 4;
        const int conditionAmount = 1;
        const int conditionPerRounds = 4;

        DaggerfallUnityItem enchantedItem;
        DaggerfallEntityBehaviour entityBehaviour;

        #endregion

        #region Enums

        enum ExtraSpellPtTypes
        {
            DuringWinter = 0,
            DuringSpring = 1,
            DuringSummer = 2,
            DuringFall = 3,
            DuringFullMoon = 4,
            DuringHalfMoon = 5,
            DuringNewMoon = 6,
            NearUndead = 7,
            NearDaedra = 8,
            NearHumanoids = 9,
            NearAnimals = 10,
        }

        enum PotentVsTypes
        {
            Undead = 0,
            Daedra = 1,
            Humanoid = 2,
            Animals = 3,
        }

        enum RegenerateTypes
        {
            AllTheTime = 0,
            InSunlight = 1,
            InDarkness = 2,
        }

        enum VampiricEffectTypes
        {
            AtRange = 0,
            WhenStrikes = 1,
        }

        enum IncreasedWeightAllowanceTypes
        {
            OneQuarterExtra = 0,
            OneHalfExtra = 1,
        }

        #endregion

        #region Overrides

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.ShowSpellIcon = false;
            bypassSavingThrows = true;
        }

        public override void Start(EntityEffectManager manager, DaggerfallEntityBehaviour caster = null)
        {
            base.Start(manager, caster);
            CacheReferences();
            SubscribeEvents();
        }

        public override void Resume(EntityEffectManager.EffectSaveData_v1 effectData, EntityEffectManager manager, DaggerfallEntityBehaviour caster = null)
        {
            base.Resume(effectData, manager, caster);
            CacheReferences();
            SubscribeEvents();
        }

        public override void End()
        {
            base.End();
            UnsubscribeEvents();
        }

        public override void ConstantEffect()
        {
            base.ConstantEffect();

            // Execute constant advantages/disadvantages
            if (entityBehaviour && enchantedItem != null)
                ConstantEnchantments();
        }

        public override void MagicRound()
        {
            base.MagicRound();

            // Execute round-based advantages/disadvantages
            if (entityBehaviour && enchantedItem != null)
                RoundBasedEnchantments();
        }

        #endregion

        #region Private Methods

        void CacheReferences()
        {
            // Cache reference to item carrying enchantments for this effect
            if (ParentBundle == null || ParentBundle.fromEquippedItem != null)
                enchantedItem = ParentBundle.fromEquippedItem;

            // Cache reference to peered entity behaviour
            if (!entityBehaviour)
                entityBehaviour = GetPeeredEntityBehaviour(manager);
        }

        void SubscribeEvents()
        {
            if (enchantedItem != null)
            {
                enchantedItem.OnWeaponStrike += OnWeaponStrikeEnchantments;
            }
        }

        void UnsubscribeEvents()
        {
            if (enchantedItem != null)
            {
                enchantedItem.OnWeaponStrike -= OnWeaponStrikeEnchantments;
            }
        }

        void ConstantEnchantments()
        {
            //System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
            //long startTime = stopwatch.ElapsedMilliseconds;

            // Constant enchantments tick every frame
            for (int i = 0; i < enchantedItem.Enchantments.Length; i++)
            {
                switch (enchantedItem.Enchantments[i].type)
                {
                    case EnchantmentTypes.ExtraSpellPts:
                        ExtraSpellPoints(enchantedItem.Enchantments[i]);
                        break;
                    case EnchantmentTypes.IncreasedWeightAllowance:
                        IncreasedWeightAllowance(enchantedItem.Enchantments[i]);
                        break;
                    case EnchantmentTypes.AbsorbsSpells:
                        entityBehaviour.Entity.IsAbsorbingSpells = true;
                        break;
                }
            }

            //long totalTime = stopwatch.ElapsedMilliseconds - startTime;
            //Debug.LogFormat("Time to run PassiveItemSpecialsEffect.ConstantEnchantments(): {0}ms", totalTime);
        }

        void RoundBasedEnchantments()
        {
            // Round-based enchantments tick once every magic round (game minute)
            for (int i = 0; i < enchantedItem.Enchantments.Length; i++)
            {
                switch (enchantedItem.Enchantments[i].type)
                {
                    case EnchantmentTypes.RegensHealth:
                        RegenerateHealth(enchantedItem.Enchantments[i]);
                        break;
                    case EnchantmentTypes.VampiricEffect:
                        VampiricEffectRanged(enchantedItem.Enchantments[i]);
                        break;
                    case EnchantmentTypes.RepairsObjects:
                        RepairItems(enchantedItem.Enchantments[i]);
                        break;
                }
            }
        }

        private void OnWeaponStrikeEnchantments(DaggerfallUnityItem item, DaggerfallEntityBehaviour receiver, int damage)
        {
            // Must have an item and receiver
            if (item == null || !receiver)
                return;

            // Weapon strike enchantments tick whenever owning item hits a target entity
            for (int i = 0; i < enchantedItem.Enchantments.Length; i++)
            {
                switch (enchantedItem.Enchantments[i].type)
                {
                    case EnchantmentTypes.PotentVs:
                        PotentVs(enchantedItem.Enchantments[i], receiver);
                        break;
                    case EnchantmentTypes.VampiricEffect:
                        VampiricEffectWhenStrikes(enchantedItem.Enchantments[i], receiver, damage);
                        break;
                }
            }

            //Debug.LogFormat("Entity {0} hit target {1} with enchanted weapon {2}.", entityBehaviour.Entity.Name, receiver.Entity.Name, enchantedItem.LongName);
        }

        #endregion

        #region Repairs Objects

        /// <summary>
        /// Testing classic yields the following results:
        ///  - Only equipped items will receive repairs, items just stored in inventory are not repaired.
        ///  - Repair will happen whether player is resting or just standing around while game time passes.
        ///  - Repair does not happen when player is fast travelling (possibly game balance reasons?).
        /// The following assumptions have been made from observation:
        ///  - Items are repaired 1 hit point every 4 minutes. Takes around 8-9 hours for a Dwarven Dagger to go from "battered" to "new" in classic and DFU with this timing.
        ///  - Uncertain if not repairing during travel is intended or not - it doesn't seem to make sense for a passive enchantment that works all other times.
        ///  - Not doing anything around this right now so that repair ticks work consistently with other passive enchantment effects.
        ///  - Assuming only a single item is repaired per tick. Priority is based on equip order enumeration.
        /// </summary>
        void RepairItems(DaggerfallEnchantment enchantment)
        {
            // Only works on player entity
            if (entityBehaviour.EntityType != EntityTypes.Player)
                return;

            // This special only triggers once every conditionPerRounds
            if (GameManager.Instance.EntityEffectBroker.MagicRoundsSinceStartup % conditionPerRounds != 0)
                return;

            // Get equipped items
            DaggerfallUnityItem[] equippedItems = GameManager.Instance.PlayerEntity.ItemEquipTable.EquipTable;
            if (equippedItems == null || equippedItems.Length == 0)
                return;

            // Improve condition of a single items not at max condition
            for (int i = 0; i < equippedItems.Length; i++)
            {
                DaggerfallUnityItem item = equippedItems[i];
                if (item != null && equippedItems[i].currentCondition < equippedItems[i].maxCondition)
                {
                    // Do not repair magic items unless settings allow it
                    if (item.IsEnchanted && !DaggerfallUnity.Settings.AllowMagicRepairs)
                        continue;

                    // Improve condition of item and exit
                    item.currentCondition += conditionAmount;
                    //Debug.LogFormat("Improved condition of item {0} by {1} points", item.LongName, conditionAmount);
                    return;
                }
            }
        }

        #endregion

        #region Increased Weight Allowance

        void IncreasedWeightAllowance(DaggerfallEnchantment enchantment)
        {
            switch((IncreasedWeightAllowanceTypes)enchantment.param)
            {
                case IncreasedWeightAllowanceTypes.OneQuarterExtra:
                    entityBehaviour.Entity.SetIncreasedWeightAllowanceMultiplier(0.25f);
                    break;
                case IncreasedWeightAllowanceTypes.OneHalfExtra:
                    entityBehaviour.Entity.SetIncreasedWeightAllowanceMultiplier(0.5f);
                    break;
            }
        }

        #endregion

        #region Vampiric Effect

        /// <summary>
        /// Vampirically drains health from nearby enemies.
        /// Classic seems to follow the 15 health per hour rule (or 1 health per 4 game minutes) at very close range.
        /// While exact range is unknown, testing in classic shows that player needs to be roughly within melee distance or no effect.
        /// </summary>
        void VampiricEffectRanged(DaggerfallEnchantment enchantment)
        {
            // Must be correct vampiric effect type
            VampiricEffectTypes type = (VampiricEffectTypes)enchantment.param;
            if (type != VampiricEffectTypes.AtRange)
                return;

            // This special only triggers once every regeneratePerRounds
            if (GameManager.Instance.EntityEffectBroker.MagicRoundsSinceStartup % regeneratePerRounds != 0)
                return;

            // Drain all enemies in range
            List<PlayerGPS.NearbyObject> nearby = GameManager.Instance.PlayerGPS.GetNearbyObjects(PlayerGPS.NearbyObjectFlags.Enemy, vampiricDrainRange);
            if (nearby != null && nearby.Count > 0)
            {
                foreach(PlayerGPS.NearbyObject enemy in nearby)
                {
                    // Get entity behaviour from found object
                    DaggerfallEntityBehaviour enemyBehaviour = (enemy.gameObject) ? enemy.gameObject.GetComponent<DaggerfallEntityBehaviour>() : null;
                    if (!enemyBehaviour)
                        continue;

                    // Transfer health from remote entity to this one
                    enemyBehaviour.Entity.CurrentHealth -= regenerateAmount;
                    entityBehaviour.Entity.CurrentHealth += regenerateAmount;
                    //Debug.LogFormat("Entity {0} drained {1} health from nearby {2}", entityBehaviour.Entity.Name, regenerateAmount, enemyBehaviour.Entity.Name);
                }
            }
        }

        /// <summary>
        /// Vampirically drain health from target equal to damage delivered.
        /// Was not able to fully confirm this how effect works, but seems close from observation alone.
        /// Not sure if only base weapon should be delivered (e.g. exclude factors like critical strike).
        /// TODO: This will likely need more research and refinement.
        /// </summary>
        void VampiricEffectWhenStrikes(DaggerfallEnchantment enchantment, DaggerfallEntityBehaviour receiver, int damage)
        {
            // Must be correct vampiric effect type
            VampiricEffectTypes type = (VampiricEffectTypes)enchantment.param;
            if (type != VampiricEffectTypes.WhenStrikes)
                return;

            // Check this is an enemy type
            EnemyEntity enemyEntity = null;
            if (receiver.EntityType == EntityTypes.EnemyMonster || receiver.EntityType == EntityTypes.EnemyClass)
                enemyEntity = receiver.Entity as EnemyEntity;

            // Drain target entity by damage amount and heal this entity by same amount
            enemyEntity.CurrentHealth -= damage;
            entityBehaviour.Entity.CurrentHealth += damage;
            //Debug.LogFormat("Entity {0} drained {1} health by striking {2}", entityBehaviour.Entity.Name, damage, enemyEntity.Name);
        }

        #endregion

        #region Regeneration

        /// <summary>
        /// Regenerates health in a manner similar to career special.
        /// Classic will regenerate 15 health per hour, stacked per item with enchanement.
        /// </summary>
        void RegenerateHealth(DaggerfallEnchantment enchantment)
        {
            // This special only triggers once every regeneratePerRounds
            if (GameManager.Instance.EntityEffectBroker.MagicRoundsSinceStartup % regeneratePerRounds != 0)
                return;

            // Check for regenerate conditions
            bool regenerate = false;
            RegenerateTypes type = (RegenerateTypes)enchantment.param;
            switch (type)
            {
                case RegenerateTypes.AllTheTime:
                    regenerate = true;
                    break;
                case RegenerateTypes.InDarkness:
                    regenerate = DaggerfallUnity.Instance.WorldTime.Now.IsNight || GameManager.Instance.PlayerEnterExit.WorldContext == WorldContext.Dungeon;
                    break;
                case RegenerateTypes.InSunlight:
                    regenerate = DaggerfallUnity.Instance.WorldTime.Now.IsDay && GameManager.Instance.PlayerEnterExit.WorldContext != WorldContext.Dungeon;
                    break;
            }

            // Tick regeneration when conditions are right
            if (regenerate)
                entityBehaviour.Entity.IncreaseHealth(regenerateAmount);
        }

        #endregion

        #region Potent Vs

        /// <summary>
        /// UESP states this power has no effect in classic.
        /// This implementation simply adds +5 damage to receiving entity on strike.
        /// Minor potency is better than nothing. Can be researched and improved later.
        /// </summary>
        void PotentVs(DaggerfallEnchantment enchantment, DaggerfallEntityBehaviour receiver)
        {
            // Check this is an enemy type
            EnemyEntity enemyEntity = null;
            if (receiver.EntityType == EntityTypes.EnemyMonster || receiver.EntityType == EntityTypes.EnemyClass)
                enemyEntity = receiver.Entity as EnemyEntity;
            else
                return;

            PotentVsTypes type = (PotentVsTypes)enchantment.param;
            if (type == PotentVsTypes.Undead && enemyEntity.MobileEnemy.Affinity == MobileAffinity.Undead ||
                type == PotentVsTypes.Daedra && enemyEntity.MobileEnemy.Affinity == MobileAffinity.Daedra ||
                type == PotentVsTypes.Humanoid && enemyEntity.MobileEnemy.Affinity == MobileAffinity.Human ||
                type == PotentVsTypes.Animals && enemyEntity.MobileEnemy.Affinity == MobileAffinity.Animal)
            {
                receiver.Entity.CurrentHealth -= potentVsDamage;
                //Debug.LogFormat("Applied +{0} potent vs damage to {1}.", potentVsDamage, type.ToString());
            }
        }

        #endregion

        #region Extra Spell Points

        /// <summary>
        /// Adds +75 to maximum spell points when certain conditions are met.
        /// </summary>
        void ExtraSpellPoints(DaggerfallEnchantment enchantment)
        {
            const int maxIncrease = 75;

            bool apply = false;
            ExtraSpellPtTypes type = (ExtraSpellPtTypes)enchantment.param;

            // Seasonal params are 0-3
            if (enchantment.param < 4)
            {
                DaggerfallDateTime.Seasons currentSeason = DaggerfallUnity.Instance.WorldTime.Now.SeasonValue;
                if (type == ExtraSpellPtTypes.DuringWinter && currentSeason == DaggerfallDateTime.Seasons.Winter ||
                    type == ExtraSpellPtTypes.DuringSpring && currentSeason == DaggerfallDateTime.Seasons.Spring ||
                    type == ExtraSpellPtTypes.DuringSummer && currentSeason == DaggerfallDateTime.Seasons.Summer ||
                    type == ExtraSpellPtTypes.DuringFall && currentSeason == DaggerfallDateTime.Seasons.Fall)
                {
                    apply = true;
                }
            }

            // Moon params are 4-6
            if (enchantment.param >= 4 && enchantment.param <= 6)
            {
                if (type == ExtraSpellPtTypes.DuringFullMoon && IsFullMoon() ||
                    type == ExtraSpellPtTypes.DuringHalfMoon && IsHalfMoon() ||
                    type == ExtraSpellPtTypes.DuringNewMoon && IsNewMoon())
                {
                    apply = true;
                }
            }

            // Nearby params are 7-10)
            // Core tracks nearby objects at low frequencies and nearby lookup is only checking a managed list using Linq
            if (enchantment.param > 6)
            {
                if (type == ExtraSpellPtTypes.NearUndead && IsNearUndead() ||
                    type == ExtraSpellPtTypes.NearDaedra && IsNearDaedra() ||
                    type == ExtraSpellPtTypes.NearHumanoids && IsNearHumanoids() ||
                    type == ExtraSpellPtTypes.NearAnimals && IsNearAnimals())
                {
                    apply = true;
                }
            }

            // Apply extra spell points when conditions are met
            if (apply)
            {
                entityBehaviour.Entity.ChangeMaxMagickaModifier(maxIncrease);
            }
        }

        bool IsFullMoon()
        {
            LunarPhases massarPhase = DaggerfallUnity.Instance.WorldTime.Now.MassarLunarPhase;
            LunarPhases secundaPhase = DaggerfallUnity.Instance.WorldTime.Now.SecundaLunarPhase;
            return massarPhase == LunarPhases.Full || secundaPhase == LunarPhases.Full;
        }

        bool IsHalfMoon()
        {
            LunarPhases massarPhase = DaggerfallUnity.Instance.WorldTime.Now.MassarLunarPhase;
            LunarPhases secundaPhase = DaggerfallUnity.Instance.WorldTime.Now.SecundaLunarPhase;
            return massarPhase == LunarPhases.HalfWane || massarPhase == LunarPhases.HalfWax ||
                   secundaPhase == LunarPhases.HalfWane || secundaPhase == LunarPhases.HalfWax;

        }

        bool IsNewMoon()
        {
            LunarPhases massarPhase = DaggerfallUnity.Instance.WorldTime.Now.MassarLunarPhase;
            LunarPhases secundaPhase = DaggerfallUnity.Instance.WorldTime.Now.SecundaLunarPhase;
            return massarPhase == LunarPhases.New || secundaPhase == LunarPhases.New;
        }

        bool IsNearUndead()
        {
            List<PlayerGPS.NearbyObject> nearby = GameManager.Instance.PlayerGPS.GetNearbyObjects(PlayerGPS.NearbyObjectFlags.Undead, nearbyRadius);
            return nearby != null && nearby.Count > 0;
        }

        bool IsNearDaedra()
        {
            List<PlayerGPS.NearbyObject> nearby = GameManager.Instance.PlayerGPS.GetNearbyObjects(PlayerGPS.NearbyObjectFlags.Daedra, nearbyRadius);
            return nearby != null && nearby.Count > 0;
        }

        bool IsNearHumanoids()
        {
            List<PlayerGPS.NearbyObject> nearby = GameManager.Instance.PlayerGPS.GetNearbyObjects(PlayerGPS.NearbyObjectFlags.Humanoid, nearbyRadius);
            return nearby != null && nearby.Count > 0;
        }

        bool IsNearAnimals()
        {
            List<PlayerGPS.NearbyObject> nearby = GameManager.Instance.PlayerGPS.GetNearbyObjects(PlayerGPS.NearbyObjectFlags.Animal, nearbyRadius);
            return nearby != null && nearby.Count > 0;
        }

        #endregion
    }
}