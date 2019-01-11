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
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Uber-effect used to deliver passive special advantages and disadvantages to player.
    /// More active specials (e.g. critical strike, disallowed armour types) are handled in related systems.
    /// NOTES:
    ///  * This effect is a work in progress and will be added to over time.
    ///  * Could also be assigned to other entities but at this time only using on player.
    /// </summary>
    public class PassiveSpecialsEffect : IncumbentEffect
    {
        #region Fields

        public static readonly string EffectKey = "Passive-Specials";
        const int sunDamageAmount = 12;
        const int holyDamageAmount = 12;
        const int regenerateAmount = 1;
        const int sunDamagePerRounds = 4;
        const int holyDamagePerRounds = 4;
        const int regeneratePerRounds = 4;

        int forcedRoundsRemaining = 1;
        DaggerfallEntityBehaviour entityBehaviour;

        #endregion

        #region Overrides

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.ShowSpellIcon = false;
        }

        public override int RoundsRemaining
        {
            get { return forcedRoundsRemaining; }
        }

        protected override int RemoveRound()
        {
            return forcedRoundsRemaining;
        }

        protected override bool IsLikeKind(IncumbentEffect other)
        {
            return (other is PassiveSpecialsEffect);
        }

        protected override void AddState(IncumbentEffect incumbent)
        {
            return;
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

        public override void ConstantEffect()
        {
            base.ConstantEffect();

            // Execute constant advantages/disadvantages
            if (entityBehaviour)
            {
                LightPoweredMagery();
                DarknessPoweredMagery();
            }
        }

        public override void MagicRound()
        {
            base.MagicRound();

            // Execute round-based effects
            if (entityBehaviour)
            {
                RegenerateHealth();
                DamageFromSunlight();
                DamageFromHolyPlaces();
            }
        }

        #endregion

        #region Regeneration

        void RegenerateHealth()
        {
            // This special only triggers once every regeneratePerRounds
            if (GameManager.Instance.EntityEffectBroker.MagicRoundsSinceStartup % regeneratePerRounds != 0)
                return;

            // Check for regenerate conditions
            bool regenerate = false;
            switch(entityBehaviour.Entity.Career.Regeneration)
            {
                default:
                case DFCareer.RegenerationFlags.None:
                    return;
                case DFCareer.RegenerationFlags.Always:
                    regenerate = true;
                    break;
                case DFCareer.RegenerationFlags.InDarkness:
                    regenerate = DaggerfallUnity.Instance.WorldTime.Now.IsNight || GameManager.Instance.PlayerEnterExit.WorldContext == WorldContext.Dungeon;
                    break;
                case DFCareer.RegenerationFlags.InLight:
                    regenerate = DaggerfallUnity.Instance.WorldTime.Now.IsDay && GameManager.Instance.PlayerEnterExit.WorldContext != WorldContext.Dungeon;
                    break;
                case DFCareer.RegenerationFlags.InWater:
                    regenerate = manager.IsPlayerEntity && (GameManager.Instance.PlayerMotor.IsSwimming || GameManager.Instance.PlayerMotor.OnExteriorWater == PlayerMotor.OnExteriorWaterMethod.Swimming);
                    break;
            }

            // Tick regeneration when conditions are right
            if (regenerate)
                entityBehaviour.Entity.IncreaseHealth(regenerateAmount);
        }

        #endregion

        #region Sun Damage

        void DamageFromSunlight()
        {
            // This special only triggers once every sunDamagePerRounds
            if (GameManager.Instance.EntityEffectBroker.MagicRoundsSinceStartup % sunDamagePerRounds != 0)
                return;

            // From entity career (e.g. vampire enemy mobile)
            bool fromCareer = entityBehaviour.Entity.Career.DamageFromSunlight;

            // From player race (e.g. vampire curse)
            bool fromRace = (manager.IsPlayerEntity) ?
                ((entityBehaviour.Entity as PlayerEntity).RaceTemplate.SpecialAbilities & DFCareer.SpecialAbilityFlags.SunDamage) == DFCareer.SpecialAbilityFlags.SunDamage :
                false;

            // Must have career or race active
            if (!fromCareer && !fromRace)
                return;

            // Must be outside during the day
            // Note: Active entities are always where the player is, so we can just use player context
            if (GameManager.Instance.PlayerEnterExit.WorldContext == WorldContext.Exterior && DaggerfallUnity.Instance.WorldTime.Now.IsDay)
            {
                // Assign sunDamageAmount points of damage
                entityBehaviour.Entity.DecreaseHealth(sunDamageAmount);
                //Debug.LogFormat("Applied {0} points of sun damage after {1} magic (game minutes)", sunDamageAmount, sunDamagePerRounds);
            }
        }

        #endregion

        #region Holy Damage

        void DamageFromHolyPlaces()
        {
            // This special only triggers once every holyDamagePerRounds
            if (GameManager.Instance.EntityEffectBroker.MagicRoundsSinceStartup % holyDamagePerRounds != 0)
                return;

            // From entity career (e.g. vampire enemy mobile)
            bool fromCareer = entityBehaviour.Entity.Career.DamageFromHolyPlaces;

            // From player race (e.g. vampire curse)
            bool fromRace = (manager.IsPlayerEntity) ?
                ((entityBehaviour.Entity as PlayerEntity).RaceTemplate.SpecialAbilities & DFCareer.SpecialAbilityFlags.HolyDamage) == DFCareer.SpecialAbilityFlags.HolyDamage :
                false;

            // Must have career or race active
            if (!fromCareer && !fromRace)
                return;

            // Must be inside
            // Note: Active entities are always where the player is, so we can just use player context
            if (GameManager.Instance.PlayerEnterExit.WorldContext == WorldContext.Interior)
            {
                // Holy places include all Temples and guildhalls of the Fighter Trainers (faction #849)
                // https://en.uesp.net/wiki/Daggerfall:ClassMaker#Special_Disadvantages
                DaggerfallInterior interior = GameManager.Instance.PlayerEnterExit.Interior;
                if (interior.BuildingData.BuildingType == DFLocation.BuildingTypes.Temple ||
                    interior.BuildingData.FactionId == (int)FactionFile.FactionIDs.Fighter_Trainers)
                {
                    // Assign holyDamageAmount points of damage
                    entityBehaviour.Entity.DecreaseHealth(holyDamageAmount);
                    //Debug.LogFormat("Applied {0} points of holy damage after {1} magic rounds (game minutes)", holyDamageAmount, holyDamagePerRounds);
                }
            }
        }

        #endregion

        #region Light & Dark Powered Magery

        void LightPoweredMagery()
        {
            // Entity suffers darkness disadvantage at night or inside dungeons
            // They will not receive penalty going in and out of well-lit buildings during the day
            if (DaggerfallUnity.Instance.WorldTime.Now.IsNight || GameManager.Instance.PlayerEnterExit.WorldContext == WorldContext.Dungeon)
            {
                // Disadvantage has two variants
                switch (entityBehaviour.Entity.Career.LightPoweredMagery)
                {
                    case DFCareer.LightMageryFlags.ReducedPowerInDarkness:
                        entityBehaviour.Entity.ChangeMaxMagickaModifier((int)(entityBehaviour.Entity.RawMaxMagicka * -0.33f));  // 33% less magicka in darkness
                        break;

                    case DFCareer.LightMageryFlags.UnableToCastInDarkness:
                        entityBehaviour.Entity.ChangeMaxMagickaModifier(-10000000);                                             // 0 magicka in light
                        break;
                }
            }
        }

        void DarknessPoweredMagery()
        {
            // Entity suffers light disadvantage during the day while outside of dungeons
            // Even well-lit daytime buildings will make entity unable to cast
            if (DaggerfallUnity.Instance.WorldTime.Now.IsDay && GameManager.Instance.PlayerEnterExit.WorldContext != WorldContext.Dungeon)
            {
                // Disadvantage has two variants
                switch (entityBehaviour.Entity.Career.DarknessPoweredMagery)
                {
                    case DFCareer.DarknessMageryFlags.ReducedPowerInLight:
                        entityBehaviour.Entity.ChangeMaxMagickaModifier((int)(entityBehaviour.Entity.RawMaxMagicka * -0.33f));  // 33% less magicka in light
                        break;

                    case DFCareer.DarknessMageryFlags.UnableToCastInLight:
                        entityBehaviour.Entity.ChangeMaxMagickaModifier(-10000000);                                             // 0 magicka in light
                        break;
                }
            }
        }

        #endregion

        #region Private Methods

        void CacheReferences()
        {
            // Cache reference to peered entity behaviour
            if (!entityBehaviour)
                entityBehaviour = GetPeeredEntityBehaviour(manager);
        }

        #endregion
    }
}