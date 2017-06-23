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
            return (int)Mathf.Floor((float)(strength - 50) / 5f);
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

        #endregion

        #region Player

        // Generates player health based on level and career hit points per level
        public static int RollMaxHealth(int level, int hitPointsPerLevel)
        {
            const int baseHealth = 25;
            int maxHealth = baseHealth + hitPointsPerLevel;

            for (int i = 1; i < level; i++)
            {
                maxHealth += UnityEngine.Random.Range(1, hitPointsPerLevel + 1);
            }

            return maxHealth;
        }

        // Calculate how much health the player should recover per hour of rest
        public static int CalculateHealthRecoveryRate(Entity.PlayerEntity player)
        {
            short medical = player.Skills.Medical;
            int endurance = player.Stats.Endurance;
            int maxHealth = player.MaxHealth;
            PlayerEnterExit playerEnterExit;
            playerEnterExit = GameManager.Instance.PlayerGPS.GetComponent<PlayerEnterExit>();
            DaggerfallConnect.DFCareer.RapidHealingFlags rapidHealingFlags = player.Career.RapidHealing;

            short addToMedical = 60;

            if (rapidHealingFlags == DaggerfallConnect.DFCareer.RapidHealingFlags.Always)
                addToMedical = 100;
            else if (DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.IsDay && !playerEnterExit.IsPlayerInside)
            {
                if (rapidHealingFlags == DaggerfallConnect.DFCareer.RapidHealingFlags.InLight)
                    addToMedical = 100;
            }
            else if (rapidHealingFlags == DaggerfallConnect.DFCareer.RapidHealingFlags.InDarkness)
                addToMedical = 100;

            medical += addToMedical;

            return Mathf.Max((int)Mathf.Floor(HealingRateModifier(endurance) + medical * maxHealth / 1000), 1);
        }

        // Calculate how much fatigue the player should recover per hour of rest
        public static int CalculateFatigueRecoveryRate(int maxFatigue)
        {
            return Mathf.Max((int)Mathf.Floor(maxFatigue / 8), 1);
        }

        // Calculate how many spell points the player should recover per hour of rest
        public static int CalculateSpellPointRecoveryRate(int maxSpellPoints)
        {
            return Mathf.Max((int)Mathf.Floor(maxSpellPoints / 8), 1);
        }

        // Calculate chance of successfully lockpicking a door in an interior (an animating door). If this is higher than a random number between 0 and 100 (inclusive), the lockpicking succeeds.
        public static int CalculateInteriorLockpickingChance(int level, int lockvalue, int lockpickingSkill)
        {
            int lockpickingChance = (5 * (level - lockvalue) + lockpickingSkill);
            if (lockpickingChance > 95)
                lockpickingChance = 95;
            else if (lockpickingChance < 5)
                lockpickingChance = 5;
            return lockpickingChance;
        }

        // Calculate chance of successfully lockpicking a door in an exterior (a door that leads to an interior). If this is higher than a random number between 0 and 100 (inclusive), the lockpicking succeeds.
        public static int CalculateExteriorLockpickingChance(int lockvalue, int lockpickingSkill)
        {
            int lockpickingChance = lockpickingSkill - (5 * lockvalue);
            if (lockpickingChance > 95)
                lockpickingChance = 95;
            else if (lockpickingChance < 5)
                lockpickingChance = 5;
            return lockpickingChance;
        }

        // Calculate how many uses a skill needs before its value will rise.
        public static int CalculateSkillUsesForAdvancement(int skillValue, int skillAdvancementMultiplier, float careerAdvancementMultiplier, int level)
        {
            double levelMod = Math.Pow(1.04, level);
            return (int)Math.Floor((skillValue * skillAdvancementMultiplier * careerAdvancementMultiplier * levelMod * 2 / 5) + 1);
        }

        // Calculate player level.
        public static int CalculatePlayerLevel(int startingLevelUpSkillsSum, int currentLevelUpSkillsSum)
        {
            return (int)Mathf.Floor((currentLevelUpSkillsSum - startingLevelUpSkillsSum + 28) / 15);
        }

        // Calculate hit points player gains per level.
        public static int CalculateHitPointsPerLevelUp(Entity.PlayerEntity player)
        {
            int minRoll = player.Career.HitPointsPerLevelOrMonsterLevel / 2;
            int maxRoll = player.Career.HitPointsPerLevelOrMonsterLevel + 1; // Adding +1 as Unity Random.Range(int,int) is exclusive of maximum value
            int addHitPoints = UnityEngine.Random.Range(minRoll, maxRoll);
            addHitPoints += HitPointsModifier(player.Stats.Endurance);
            if (addHitPoints < 1)
                addHitPoints = 1;
            return addHitPoints;
        }

        #endregion

        #region Damage

        public static int CalculateHandToHandMinDamage(int handToHandSkill)
        {
            return (handToHandSkill / 10) + 1;
        }

        public static int CalculateHandToHandMaxDamage(int handToHandSkill)
        {
            // Daggerfall Chronicles table lists hand-to-hand skills of 80 and above (45 through 79 are omitted)
            // as if they give max damage of (handToHandSkill / 5) + 2, but the hand-to-hand damage display in the character sheet
            // in classic Daggerfall shows it as continuing to be (handToHandSkill / 5) + 1
            return (handToHandSkill / 5) + 1;
        }

        public static int CalculateWeaponDamage(Entity.DaggerfallEntity attacker, Entity.DaggerfallEntity target, FPSWeapon onScreenWeapon)
        {
            // In classic, hand-to-hand damage is not affected by the strength modifier, by the type of swing or by hand-to-hand proficiency.
            // Both the game manual and strength attribute description say that the strength modifier applies to hand-to-hand damage,
            // and hand-to-hand proficiency would have no effect if it didn't do something for damage and chance to hit.

            int damageLow = 0;
            int damageHigh = 0;
            int damageResult = 0;
            int chanceToHitMod = 0;

            // TODO: Calculations for AI characters.
            if (attacker == GameManager.Instance.PlayerEntity)
            {
                Items.DaggerfallUnityItem weapon;
                if (GameManager.Instance.WeaponManager.UsingRightHand)
                    weapon = attacker.ItemEquipTable.GetItem(Items.EquipSlots.RightHand);
                else
                    weapon = attacker.ItemEquipTable.GetItem(Items.EquipSlots.LeftHand);

                if (weapon == null)
                {
                    damageLow = CalculateHandToHandMinDamage(attacker.Skills.HandToHand);
                    damageHigh = CalculateHandToHandMaxDamage(attacker.Skills.HandToHand);
                    chanceToHitMod += attacker.Skills.GetSkillValue(attacker.Skills.HandToHand);
                }
                else
                {
                    damageLow = weapon.GetBaseDamageMin();
                    damageHigh = weapon.GetBaseDamageMax();
                    short skill = weapon.GetWeaponSkillID();
                    chanceToHitMod += attacker.Skills.GetSkillValue((DaggerfallConnect.DFCareer.Skills)skill);
                }

                damageResult = UnityEngine.Random.Range(damageLow, damageHigh + 1);

                if (onScreenWeapon != null)
                {
                    // Apply swing modifier.
                    // The Daggerfall manual groups diagonal slashes to the left and right as if they are the same, but they are different.
                    if (onScreenWeapon.WeaponState == WeaponStates.StrikeUp)
                    {
                        damageResult += -4;
                        chanceToHitMod += 10;
                    }
                    if (onScreenWeapon.WeaponState == WeaponStates.StrikeDownRight)
                    {
                        damageResult += -2;
                        chanceToHitMod += 5;
                    }
                    if (onScreenWeapon.WeaponState == WeaponStates.StrikeDownLeft)
                    {
                        damageResult += 2;
                        chanceToHitMod -= 5;
                    }
                    if (onScreenWeapon.WeaponState == WeaponStates.StrikeDown)
                    {
                        damageResult += 4;
                        chanceToHitMod -= 10;
                    }
                }

                // Apply weapon expertise modifier.
                if (weapon != null && ((int)attacker.Career.ExpertProficiencies & weapon.GetWeaponSkillUsed()) != 0)
                {
                    damageResult += ((attacker.Level / 3) + 1);
                    chanceToHitMod += attacker.Level;
                }
                // Apply hand-to-hand expertise modifier.
                else if (weapon == null && ((int)attacker.Career.ExpertProficiencies & (int)(DaggerfallConnect.DFCareer.ProficiencyFlags.HandToHand)) != 0)
                {
                    damageResult += ((attacker.Level / 3) + 1);
                    chanceToHitMod += attacker.Level;
                }

                // Apply bonus or penalty from opponent type.
                // In classic this is broken and only works if the attack is done with a weapon that has the maximum number of enchantments.
                Entity.EnemyEntity enemyEntity = target as Entity.EnemyEntity;
                if (enemyEntity.GetEnemyGroup() == DaggerfallConnect.DFCareer.EnemyGroups.Undead)
                {
                    if (((int)attacker.Career.UndeadAttackModifier & (int)DaggerfallConnect.DFCareer.AttackModifier.Bonus) != 0)
                    {
                        damageResult += attacker.Level;
                    }
                    if (((int)attacker.Career.UndeadAttackModifier & (int)DaggerfallConnect.DFCareer.AttackModifier.Phobia) != 0)
                    {
                        damageResult -= attacker.Level;
                    }
                }
                else if (enemyEntity.GetEnemyGroup() == DaggerfallConnect.DFCareer.EnemyGroups.Daedra)
                {
                    if (((int)attacker.Career.DaedraAttackModifier & (int)DaggerfallConnect.DFCareer.AttackModifier.Bonus) != 0)
                    {
                        damageResult += attacker.Level;
                    }
                    if (((int)attacker.Career.DaedraAttackModifier & (int)DaggerfallConnect.DFCareer.AttackModifier.Phobia) != 0)
                    {
                        damageResult -= attacker.Level;
                    }
                }
                else if (enemyEntity.GetEnemyGroup() == DaggerfallConnect.DFCareer.EnemyGroups.Humanoid)
                {
                    if (((int)attacker.Career.HumanoidAttackModifier & (int)DaggerfallConnect.DFCareer.AttackModifier.Bonus) != 0)
                    {
                        damageResult += attacker.Level;
                    }
                    if (((int)attacker.Career.HumanoidAttackModifier & (int)DaggerfallConnect.DFCareer.AttackModifier.Phobia) != 0)
                    {
                        damageResult -= attacker.Level;
                    }
                }
                else if (enemyEntity.GetEnemyGroup() == DaggerfallConnect.DFCareer.EnemyGroups.Animals)
                {
                    if (((int)attacker.Career.AnimalsAttackModifier & (int)DaggerfallConnect.DFCareer.AttackModifier.Bonus) != 0)
                    {
                        damageResult += attacker.Level;
                    }
                    if (((int)attacker.Career.AnimalsAttackModifier & (int)DaggerfallConnect.DFCareer.AttackModifier.Phobia) != 0)
                    {
                        damageResult -= attacker.Level;
                    }
                }

                // Apply racial modifier.
                Entity.PlayerEntity playerEntity = attacker as Entity.PlayerEntity;
                if (weapon != null)
                {
                    if (playerEntity.RaceTemplate.ID == (int)Entity.Races.DarkElf)
                    {
                        damageResult += (attacker.Level / 4);
                        chanceToHitMod += (attacker.Level / 4);
                    }
                    else if (weapon.GetWeaponSkillUsed() == (int)DaggerfallConnect.DFCareer.ProficiencyFlags.MissileWeapons)
                    {
                        if (playerEntity.RaceTemplate.ID == (int)Entity.Races.WoodElf)
                        {
                            damageResult += (attacker.Level / 3);
                            chanceToHitMod += (attacker.Level / 3);
                        }
                    }
                    else if (playerEntity.RaceTemplate.ID == (int)Entity.Races.Redguard)
                    {
                        damageResult += (attacker.Level / 3);
                        chanceToHitMod += (attacker.Level / 3);
                    }
                }

                // At this point we have everything we need to calculate a hit or miss,
                // so check for a successful hit before continuing damage calculations.
                if (CalculateSuccessfulHit(attacker, target, chanceToHitMod, weapon) == false)
                    return 0;

                // Apply modifiers for Skeletal Warrior.
                if (weapon != null && enemyEntity.CareerIndex == (int)Entity.MonsterCareers.SkeletalWarrior)
                {
                    if (weapon.NativeMaterialValue == (int)Items.WeaponMaterialTypes.Silver)
                        damageResult *= 2;
                    if (weapon.GetWeaponSkillUsed() != (int)DaggerfallConnect.DFCareer.ProficiencyFlags.BluntWeapons)
                        damageResult /= 2;
                }

                // Apply strength modifier.
                // The in-game display of the strength modifier in Daggerfall is incorrect. It is actually ((STR - 50) / 5).
                damageResult += DamageModifier(attacker.Stats.Strength);

                // Apply material modifier.
                // The in-game display in Daggerfall of weapon damages with material modifiers is incorrect. The material modifier is half of what the display suggests.
                if (weapon != null)
                {
                    damageResult += weapon.GetMaterialModifier();
                }

                // 0 damage is possible. Creates no blood splash.
                damageResult = Mathf.Max(0, damageResult);
            }

            return damageResult;
        }

        public static bool CalculateSuccessfulHit(Entity.DaggerfallEntity attacker, Entity.DaggerfallEntity target, int chanceToHitMod, Items.DaggerfallUnityItem weapon)
        {
            int chanceToHit = chanceToHitMod;

            // Apply random modifier
            int[] randomMods = { 0, 1, 1, 1, 1, 2, 2, 2, 3, 3, 3, 3, 4, 4, 4, 4, 5, 5, 5, 6 };
            int randomMod = randomMods[(UnityEngine.Random.Range(0, 19 + 1))];

            chanceToHit += randomMod;

            // Apply armor value modifier.
            Entity.EnemyEntity enemyEntity = target as Entity.EnemyEntity;
            int armorValue = (enemyEntity.MobileEnemy.ArmorValue * 5);

            // TODO: Add enemy classes' equipment armor values. For now using fudged value of 60.
            if (enemyEntity.EntityType == EntityTypes.EnemyClass)
                armorValue = 60;

            chanceToHit += armorValue;

            // Apply adrenaline rush modifiers.
            if (attacker.Career.AdrenalineRush && attacker.CurrentHealth < (attacker.MaxHealth / 8))
                chanceToHit += 5;
            if (target.Career.AdrenalineRush && target.CurrentHealth < (target.MaxHealth / 8))
                chanceToHit -= 5;

            // Apply luck modifier.
            chanceToHit += ((attacker.Stats.Luck - target.Stats.Luck) / 10);

            // Apply agility modifier.
            chanceToHit += ((attacker.Stats.Agility - target.Stats.Agility) / 10);

            // Apply weapon material modifier.
            if (weapon != null)
            {
                chanceToHit += (weapon.GetMaterialModifier() * 10);
            }

            // Apply dodging modifier.
            // This modifier is bugged in classic and the attacker's dodging skill is used rather than the target's.
            // DF Chronicles says the dodging calculation is (dodging / 10), but it actually seems to be (dodging / 4).
            chanceToHit -= (target.Skills.Dodging / 4);

            // Apply critical strike modifier.
            if (UnityEngine.Random.Range(0, 100 + 1) < attacker.Skills.CriticalStrike)
            {
                chanceToHit += (attacker.Skills.CriticalStrike / 10);
            }

            // Apply monster modifier.
            if (enemyEntity.EntityType == EntityTypes.EnemyMonster)
                chanceToHit += 40;

            // DF Chronicles says -60 is applied at the end, but it actually seems to be -50.
            chanceToHit -= 50;

            if (chanceToHit > 97)
                chanceToHit = 97;
            if (chanceToHit < 3)
                chanceToHit = 3;

            int roll = UnityEngine.Random.Range(0, 100 + 1);

            if (roll <= chanceToHit)
                return true;
            else
                return false;
        }

        #endregion

        #region Enemies

        // Generates health for enemy classes based on level and class
        public static int RollEnemyClassMaxHealth(int level, int hitPointsPerLevel)
        {
            const int baseHealth = 10;
            int maxHealth = baseHealth;

            for (int i = 0; i < level; i++)
            {
                maxHealth += UnityEngine.Random.Range(1, hitPointsPerLevel + 1);
            }

            return maxHealth;
        }

        #endregion

        #region Fast Travel

        // Calculate fast travel cost. Based on observing classic Daggerfall behavior.
        public static int CalculateTripCost(float travelTimeTotalLand, float travelTimeTotalWater, bool speedCautious, bool sleepModeInn, bool travelFoot, int cautiousMod, float shipMod)
        {
            int tripCost = 0;

            // For cost calculations we need to know the time taken on land and water for both a cautious and reckless trip
            float travelTimeTotalLandCautious = travelTimeTotalLand;
            float travelTimeTotalLandReckless = travelTimeTotalLand;
            float travelTimeTotalWaterCautious = travelTimeTotalWater;
            float travelTimeTotalWaterReckless = travelTimeTotalWater;

            if (speedCautious)
            {
                travelTimeTotalLandReckless /= cautiousMod;
                travelTimeTotalWaterReckless /= cautiousMod;
            }
            else
            {
                travelTimeTotalLandCautious *= cautiousMod;
                travelTimeTotalWaterCautious *= cautiousMod;
            }

            // Get reckless travel times in days
            int travelTimeDaysLandReckless = 0;
            int travelTimeDaysWaterReckless = 0;
            int travelTimeDaysTotalReckless = 0;

            if (travelTimeTotalLandReckless > 0)
                travelTimeDaysLandReckless = (int)((travelTimeTotalLandReckless / 60 / 24) + 0.5);
            if (travelTimeTotalWaterReckless > 0)
                travelTimeDaysWaterReckless = (int)((travelTimeTotalWaterReckless / 60 / 24) + 0.5);
            travelTimeDaysTotalReckless = travelTimeDaysLandReckless + travelTimeDaysWaterReckless;

            // Get cautious travel times in days
            int travelTimeDaysLandCautious = 0;
            int travelTimeDaysWaterCautious = 0;
            int travelTimeDaysTotalCautious = 0;

            if (travelTimeTotalLandCautious > 0)
                travelTimeDaysLandCautious = (int)((travelTimeTotalLandCautious / 60 / 24) + 0.5);
            if (travelTimeTotalWaterCautious > 0)
                travelTimeDaysWaterCautious = (int)((travelTimeTotalWaterCautious / 60 / 24) + 0.5);
            travelTimeDaysTotalCautious = travelTimeDaysLandCautious + travelTimeDaysWaterCautious;

            // Calculate inn costs. Use cautious travel cost as a base.
            // Inns cost (5 * total land travel days) for land-only travel or travel that crosses water if a ship is used.
            if ((travelTimeTotalWater <= 0 || !travelFoot) && sleepModeInn && (travelTimeTotalLand > 0))
                tripCost += Mathf.Max(5, travelTimeDaysLandCautious * 5);

            // For travel that crosses water and is by foot/horse, the cost of
            // inns is (5 * (total travel days - days on water for reckless travel using a ship)).
            else if ((travelTimeTotalWater > 0) && sleepModeInn && travelFoot)
            {
                int travelTimeRecklessShipDays = (int)((travelTimeTotalWaterReckless * shipMod / 60 / 24) + 0.5);
                tripCost += Mathf.Max(5, (travelTimeDaysTotalCautious - travelTimeRecklessShipDays) * 5);
            }

            // Cost for inns in reckless travel is reduced from cautious travel cost by
            // (number of days less than cautious travel * 5)
            if (!speedCautious)
            {
                if (sleepModeInn)
                    tripCost -= Mathf.Min(tripCost, ((travelTimeDaysTotalCautious - travelTimeDaysTotalReckless) * 5));
            }

            // Calculate ship costs.
            // Ships cost (25 * (days on water for cautious travel))
            if (!travelFoot && (travelTimeTotalWater > 0))
                tripCost += Mathf.Max(25, travelTimeDaysWaterCautious * 25);

            return tripCost;
        }

        #endregion
    }
}