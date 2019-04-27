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
    ///  * Now that enchantment system is built, some or all of these will be moved to their corresponding effect.
    ///  * Future item enchantment payloads should be implemented with their own effect class.
    /// </summary>
    public class PassiveItemSpecialsEffect : BaseEntityEffect
    {
        #region Fields

        public static readonly string EffectKey = "Passive-Item-Specials";

        const float nearbyRadius = 18f;             // Reasonably matched to classic with testing
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

        enum RegenerateTypes
        {
            AllTheTime = 0,
            InSunlight = 1,
            InDarkness = 2,
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
        }

        public override void Resume(EntityEffectManager.EffectSaveData_v1 effectData, EntityEffectManager manager, DaggerfallEntityBehaviour caster = null)
        {
            base.Resume(effectData, manager, caster);
            CacheReferences();
        }

        public override void End()
        {
            base.End();
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

        void ConstantEnchantments()
        {
            //System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
            //long startTime = stopwatch.ElapsedMilliseconds;

            // Constant enchantments tick every frame
            for (int i = 0; i < enchantedItem.LegacyEnchantments.Length; i++)
            {
                switch (enchantedItem.LegacyEnchantments[i].type)
                {
                    case EnchantmentTypes.ExtraSpellPts:
                        ExtraSpellPoints(enchantedItem.LegacyEnchantments[i]);
                        break;
                    case EnchantmentTypes.IncreasedWeightAllowance:
                        IncreasedWeightAllowance(enchantedItem.LegacyEnchantments[i]);
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
            for (int i = 0; i < enchantedItem.LegacyEnchantments.Length; i++)
            {
                switch (enchantedItem.LegacyEnchantments[i].type)
                {
                    case EnchantmentTypes.RegensHealth:
                        RegenerateHealth(enchantedItem.LegacyEnchantments[i]);
                        break;
                    case EnchantmentTypes.RepairsObjects:
                        RepairItems(enchantedItem.LegacyEnchantments[i]);
                        break;
                }
            }
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