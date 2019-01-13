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

        #endregion

        #region Overrides

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.ShowSpellIcon = false;
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
                }
            }

            //long totalTime = stopwatch.ElapsedMilliseconds - startTime;
            //Debug.LogFormat("Time to run PassiveItemSpecialsEffect.ConstantEnchantments(): {0}ms", totalTime);
        }

        void RoundBasedEnchantments()
        {
            // TODO: Round-based enchantments tick once every magic round (game minute)
            //for (int i = 0; i < enchantedItem.Enchantments.Length; i++)
            //{
            //    switch (enchantedItem.Enchantments[i].type)
            //    {
            //    }
            //}
        }

        private void OnWeaponStrikeEnchantments(DaggerfallUnityItem item, DaggerfallEntityBehaviour receiver)
        {
            // Must have an item and receiver
            if (item == null || !receiver)
                return;

            // TODO: Weapon strike enchantments tick whenever owning item hits a target entity
            for (int i = 0; i < enchantedItem.Enchantments.Length; i++)
            {
                switch (enchantedItem.Enchantments[i].type)
                {
                    case EnchantmentTypes.PotentVs:
                        PotentVs(enchantedItem.Enchantments[i], receiver);
                        break;
                }
            }

            //Debug.LogFormat("Entity {0} hit target {1} with enchanted weapon {2}.", entityBehaviour.Entity.Name, receiver.Entity.Name, enchantedItem.LongName);
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