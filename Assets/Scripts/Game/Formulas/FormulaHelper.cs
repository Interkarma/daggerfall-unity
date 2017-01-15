// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2016 Daggerfall Workshop
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

namespace DaggerfallWorkshop.Game.Formulas
{
    /// <summary>
    /// Common formulas used throughout game.
    /// Where the exact formula is unknown, a "best effort" approximation will be used.
    /// Will likely be migrated to a pluggable IFormulaProvider at a later date.
    /// </summary>
    public static class FormulaHelper
    {
        #region Basic Formulas

        public static int DamageModifier(int strength)
        {
            return (int)Mathf.Floor((float)strength / 10f) - 5;
        }

        public static int MaxEncumbrance(int strength)
        {
            return (int)Mathf.Floor((float)strength * 1.5f);
        }

        public static int SpellPoints(int intelligence, float multiplier)
        {
            return (int)Mathf.Floor((float)intelligence * multiplier);
        }

        public static int MagicResist(int willpower)
        {
            return (int)Mathf.Floor((float)willpower / 10f);
        }

        public static int ToHitModifier(int agility)
        {
            return (int)Mathf.Floor((float)agility / 10f) - 5;
        }

        public static int HitPointsModifier(int endurance)
        {
            return (int)Mathf.Floor((float)endurance / 10f) - 5;
        }

        public static int HealingRateModifier(int endurance)
        {
            // Original Daggerfall seems to have a bug where negative endurance modifiers on healing rate
            // are applied as modifier + 1. Not recreating that here.
            return (int)Mathf.Floor((float)endurance / 10f) - 5;
        }

        public static float HealingRateModifierMedical(int medical)
        {
            return ((float)medical / 10f) + 6;
        }

        public static float HealingRateModifierMaxHealth(int maxHealth)
        {
            return ((float)maxHealth / 100f);
        }

        #endregion

        #region Player

        // Generates player health based on level, endurance, and career hit points per level
        public static int RollMaxHealth(int level, int endurance, int hitPointsPerLevel)
        {
            const int baseHealth = 25;

            int maxHealth = baseHealth;
            int bonusHealth = HitPointsModifier(endurance);
            int minRoll = hitPointsPerLevel / 2;
            int maxRoll = hitPointsPerLevel + 1;    // Adding +1 as Unity Random.Range(int,int) is exclusive of maximum value
            for (int i = 0; i < level; i++)
            {
                maxHealth += UnityEngine.Random.Range(minRoll, maxRoll) + bonusHealth;
            }

            return maxHealth;
        }

        // Calculate how much health the player should recover per hour of rest
        public static int CalculateHealthRecoveryRate(int medical, int endurance, int maxHealth)
        {           
            return (int)Mathf.Floor((HealingRateModifierMedical(medical) * HealingRateModifierMaxHealth(maxHealth)) + HealingRateModifier(endurance));
        }

        #endregion

        #region Damage

        public static int CalculateWeaponMinDamage(WeaponTypes weaponType, MetalTypes metalType, int handToHandSkill)
        {
            // Temp value, to be replaced
            int damage_low = 1;

            if (weaponType == WeaponTypes.Melee)
            {
                int skill = handToHandSkill;
                damage_low = (skill / 10) + 1;
            }

            return damage_low;
        }

        public static int CalculateWeaponMaxDamage(WeaponTypes weaponType, MetalTypes metalType, int handToHandSkill)
        {
            // Temp value, to be replaced
            int damage_high = 24;

            // Daggerfall Chronicles table lists hand-to-hand skills of 80 and above (45 through 79 are omitted)
            // as if they cause 2 to be added to damage_high instead of 1, but the hand-to-hand damage display
            // in the in-game character sheet contradicts this.
            if (weaponType == WeaponTypes.Melee)
            {
                int skill = handToHandSkill;
                damage_high = (skill / 5) + 1;
            }

            return damage_high;
        }

        public static int CalculateMeleeDamage(FPSWeapon weapon, DaggerfallWorkshop.Game.Entity.PlayerEntity player)
        {
            int damage_low = CalculateWeaponMinDamage(weapon.WeaponType, weapon.MetalType, player.Skills.HandToHand);
            int damage_high = CalculateWeaponMaxDamage(weapon.WeaponType, weapon.MetalType, player.Skills.HandToHand);
            int damage = UnityEngine.Random.Range(damage_low, damage_high + 1);

            // Apply the strength modifier. Testing in original Daggerfall shows hand-to-hand ignores it.
            if (weapon.WeaponType != WeaponTypes.Melee)
            {
                // Weapons can do 0 damage. Plays no hit sound or blood splash.
                damage = Mathf.Max(0, damage + DamageModifier(player.Stats.Strength));
            }

            return damage;
        }

        #endregion
    }
}