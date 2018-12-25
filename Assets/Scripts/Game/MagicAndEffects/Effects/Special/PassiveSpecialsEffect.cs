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
        const int sunDamageMinutes = 4;
        const int holyDamageMinutes = 4;

        int forcedRoundsRemaining = 1;

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

        public override void ConstantEffect()
        {
            base.ConstantEffect();

            // TODO: Assign constant advantages/disadvantages
        }

        public override void MagicRound()
        {
            base.MagicRound();

            // Get peered entity gameobject
            DaggerfallEntityBehaviour entityBehaviour = GetPeeredEntityBehaviour(manager);
            if (!entityBehaviour)
                return;

            // TODO: Assign advantages/disadvantages aligned to minutes

            DamageFromSunlight(entityBehaviour);
            DamageFromHolyPlaces(entityBehaviour);
        }

        #endregion

        #region Sun Damage

        void DamageFromSunlight(DaggerfallEntityBehaviour entityBehaviour)
        {
            // This special only triggers once every sunDamageMinutes
            if (DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime() % sunDamageMinutes != 0)
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
                Debug.LogFormat("Applied {0} points of sun damage after {1} minutes", sunDamageAmount, sunDamageMinutes);
            }
        }

        #endregion

        #region Holy Damage

        void DamageFromHolyPlaces(DaggerfallEntityBehaviour entityBehaviour)
        {
            // This special only triggers once every holyDamageMinutes
            if (DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime() % holyDamageMinutes != 0)
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
                    Debug.LogFormat("Applied {0} points of holy damage after {1} minutes", holyDamageAmount, holyDamageMinutes);
                }
            }
        }

        #endregion
    }
}