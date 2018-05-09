// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    ifkopifko
// 
// Notes:
//

using UnityEngine;
using System;
using DaggerfallConnect;
using DaggerfallWorkshop.Game.Guilds;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.MagicAndEffects;
using System.Collections.Generic;
using DaggerfallConnect.Arena2;

namespace DaggerfallWorkshop.Game.Formulas
{
    /// <summary>
    /// Common formulas used throughout game.
    /// Where the exact formula is unknown, a "best effort" approximation will be used.
    /// Most formula can be overridden by registering a new method matching the appropriate delegate signature.
    /// Other signatures can be added upon demand.
    /// </summary>
    public static class FormulaHelper
    {
        // Delegate method signatures for overriding default formula
        public delegate int Formula_1i(int a);
        public delegate int Formula_2i(int a, int b);
        public delegate int Formula_3i(int a, int b, int c);
        public delegate int Formula_1i_1f(int a, float b);
        public delegate int Formula_2de(DaggerfallEntity de1, DaggerfallEntity de2);
        public delegate bool Formula_1pe_1sk(PlayerEntity pe, DFCareer.Skills sk);

        // Registries for overridden formula
        public static Dictionary<string, Formula_1i>        formula_1i = new Dictionary<string, Formula_1i>();
        public static Dictionary<string, Formula_2i>        formula_2i = new Dictionary<string, Formula_2i>();
        public static Dictionary<string, Formula_3i>        formula_3i = new Dictionary<string, Formula_3i>();
        public static Dictionary<string, Formula_1i_1f>     formula_1i_1f = new Dictionary<string, Formula_1i_1f>();
        public static Dictionary<string, Formula_2de>       formula_2de = new Dictionary<string, Formula_2de>();
        public static Dictionary<string, Formula_1pe_1sk>   formula_1pe_1sk = new Dictionary<string, Formula_1pe_1sk>();

        #region Basic Formulas

        public static int DamageModifier(int strength)
        {
            Formula_1i del;
            if (formula_1i.TryGetValue("DamageModifier", out del))
                return del(strength);

            return (int)Mathf.Floor((float)(strength - 50) / 5f);
        }

        public static int MaxEncumbrance(int strength)
        {
            Formula_1i del;
            if (formula_1i.TryGetValue("MaxEncumbrance", out del))
                return del(strength);

            return (int)Mathf.Floor((float)strength * 1.5f);
        }

        public static int SpellPoints(int intelligence, float multiplier)
        {
            Formula_1i_1f del;
            if (formula_1i_1f.TryGetValue("SpellPoints", out del))
                return del(intelligence, multiplier);

            return (int)Mathf.Floor((float)intelligence * multiplier);
        }

        public static int MagicResist(int willpower)
        {
            Formula_1i del;
            if (formula_1i.TryGetValue("MagicResist", out del))
                return del(willpower);

            return (int)Mathf.Floor((float)willpower / 10f);
        }

        public static int ToHitModifier(int agility)
        {
            Formula_1i del;
            if (formula_1i.TryGetValue("ToHitModifier", out del))
                return del(agility);

            return (int)Mathf.Floor((float)agility / 10f) - 5;
        }

        public static int HitPointsModifier(int endurance)
        {
            Formula_1i del;
            if (formula_1i.TryGetValue("HitPointsModifier", out del))
                return del(endurance);

            return (int)Mathf.Floor((float)endurance / 10f) - 5;
        }

        public static int HealingRateModifier(int endurance)
        {
            Formula_1i del;
            if (formula_1i.TryGetValue("HealingRateModifier", out del))
                return del(endurance);

            // Original Daggerfall seems to have a bug where negative endurance modifiers on healing rate
            // are applied as modifier + 1. Not recreating that here.
            return (int)Mathf.Floor((float)endurance / 10f) - 5;
        }

        #endregion

        #region Player

        // Generates player health based on level and career hit points per level
        public static int RollMaxHealth(int level, int hitPointsPerLevel)
        {
            Formula_2i del;
            if (formula_2i.TryGetValue("RollMaxHealth", out del))
                return del(level, hitPointsPerLevel);

            const int baseHealth = 25;
            int maxHealth = baseHealth + hitPointsPerLevel;

            for (int i = 1; i < level; i++)
            {
                maxHealth += UnityEngine.Random.Range(1, hitPointsPerLevel + 1);
            }

            return maxHealth;
        }

        // Calculate how much health the player should recover per hour of rest
        public static int CalculateHealthRecoveryRate(PlayerEntity player)
        {
            Formula_2de del;
            if (formula_2de.TryGetValue("CalculateHealthRecoveryRate", out del))
                return del(player, null);

            short medical = player.Skills.GetLiveSkillValue(DFCareer.Skills.Medical);
            int endurance = player.Stats.LiveEndurance;
            int maxHealth = player.MaxHealth;
            PlayerEnterExit playerEnterExit;
            playerEnterExit = GameManager.Instance.PlayerGPS.GetComponent<PlayerEnterExit>();
            DFCareer.RapidHealingFlags rapidHealingFlags = player.Career.RapidHealing;

            short addToMedical = 60;

            if (rapidHealingFlags == DFCareer.RapidHealingFlags.Always)
                addToMedical = 100;
            else if (DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.IsDay && !playerEnterExit.IsPlayerInside)
            {
                if (rapidHealingFlags == DFCareer.RapidHealingFlags.InLight)
                    addToMedical = 100;
            }
            else if (rapidHealingFlags == DFCareer.RapidHealingFlags.InDarkness)
                addToMedical = 100;

            medical += addToMedical;

            return Mathf.Max((int)Mathf.Floor(HealingRateModifier(endurance) + medical * maxHealth / 1000), 1);
        }

        // Calculate how much fatigue the player should recover per hour of rest
        public static int CalculateFatigueRecoveryRate(int maxFatigue)
        {
            Formula_1i del;
            if (formula_1i.TryGetValue("HealingRateModifier", out del))
                return del(maxFatigue);

            return Mathf.Max((int)Mathf.Floor(maxFatigue / 8), 1);
        }

        // Calculate how many spell points the player should recover per hour of rest
        public static int CalculateSpellPointRecoveryRate(int maxSpellPoints)
        {
            Formula_1i del;
            if (formula_1i.TryGetValue("HealingRateModifier", out del))
                return del(maxSpellPoints);

            return Mathf.Max((int)Mathf.Floor(maxSpellPoints / 8), 1);
        }

        // Calculate chance of successfully lockpicking a door in an interior (an animating door). If this is higher than a random number between 0 and 100 (inclusive), the lockpicking succeeds.
        public static int CalculateInteriorLockpickingChance(int level, int lockvalue, int lockpickingSkill)
        {
            Formula_3i del;
            if (formula_3i.TryGetValue("CalculateInteriorLockpickingChance", out del))
                return del(level, lockvalue, lockpickingSkill);

            int chance = (5 * (level - lockvalue) + lockpickingSkill);
            return Mathf.Clamp(chance, 5, 95);
        }

        // Calculate chance of successfully lockpicking a door in an exterior (a door that leads to an interior). If this is higher than a random number between 0 and 100 (inclusive), the lockpicking succeeds.
        public static int CalculateExteriorLockpickingChance(int lockvalue, int lockpickingSkill)
        {
            Formula_2i del;
            if (formula_2i.TryGetValue("CalculateExteriorLockpickingChance", out del))
                return del(lockvalue, lockpickingSkill);

            int chance = lockpickingSkill - (5 * lockvalue);
            return Mathf.Clamp(chance, 5, 95);
        }

        // Calculate chance of successfully pickpocketing a target
        public static int CalculatePickpocketingChance(PlayerEntity player, EnemyEntity target)
        {
            Formula_2de del;
            if (formula_2de.TryGetValue("CalculatePickpocketingChance", out del))
                return del(player, target);

            int chance = player.Skills.GetLiveSkillValue(DFCareer.Skills.Pickpocket);
            // If target is an enemy mobile, apply level modifier.
            if (target != null)
            {
                chance += 5 * ((player.Level) - (target.Level));
            }
            return Mathf.Clamp(chance, 5, 95);
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
            Formula_2i del;
            if (formula_2i.TryGetValue("CalculatePlayerLevel", out del))
                return del(startingLevelUpSkillsSum, currentLevelUpSkillsSum);

            return (int)Mathf.Floor((currentLevelUpSkillsSum - startingLevelUpSkillsSum + 28) / 15);
        }

        // Calculate hit points player gains per level.
        public static int CalculateHitPointsPerLevelUp(PlayerEntity player)
        {
            Formula_2de del;
            if (formula_2de.TryGetValue("CalculateHitPointsPerLevelUp", out del))
                return del(player, null);

            int minRoll = player.Career.HitPointsPerLevel / 2;
            int maxRoll = player.Career.HitPointsPerLevel + 1; // Adding +1 as Unity Random.Range(int,int) is exclusive of maximum value
            int addHitPoints = UnityEngine.Random.Range(minRoll, maxRoll);
            addHitPoints += HitPointsModifier(player.Stats.LiveEndurance);
            if (addHitPoints < 1)
                addHitPoints = 1;
            return addHitPoints;
        }

        // Calculate whether the player is successful at pacifying an enemy.
        public static bool CalculateEnemyPacification(PlayerEntity player, DFCareer.Skills languageSkill)
        {
            Formula_1pe_1sk del;
            if (formula_1pe_1sk.TryGetValue("CalculateEnemyPacification", out del))
                return del(player, languageSkill);

            double chance = 0;
            if (languageSkill == DFCareer.Skills.Etiquette ||
                languageSkill == DFCareer.Skills.Streetwise)
            {
                chance += player.Skills.GetLiveSkillValue(languageSkill) / 10;
                chance += player.Stats.LivePersonality / 5;
            }
            else
            {
                chance += player.Skills.GetLiveSkillValue(languageSkill);
                chance += player.Stats.LivePersonality / 10;
            }
            chance += GameManager.Instance.WeaponManager.Sheathed ? 10 : -25;

            int roll = UnityEngine.Random.Range(0, 200);
            bool success = (roll < chance);
            if (success)
                player.TallySkill(languageSkill, 1);
            else if (languageSkill != DFCareer.Skills.Etiquette && languageSkill != DFCareer.Skills.Streetwise)
                player.TallySkill(languageSkill, 1);

            Debug.LogFormat("Pacification {3} using {0} skill: chance= {1}  roll= {2}", languageSkill, chance, roll, success ? "success" : "failure");
            return success;
        }

        // Calculate whether the player is blessed when donating to a Temple.
        public static int CalculateTempleBlessing(int donationAmount, int deityRep)
        {
            return 1;   // TODO Amount of stat boost, guessing what this formula might need...
        }

        #endregion

        #region Damage

        public static int CalculateHandToHandMinDamage(int handToHandSkill)
        {
            Formula_1i del;
            if (formula_1i.TryGetValue("CalculateHandToHandMinDamage", out del))
                return del(handToHandSkill);

            return (handToHandSkill / 10) + 1;
        }

        public static int CalculateHandToHandMaxDamage(int handToHandSkill)
        {
            Formula_1i del;
            if (formula_1i.TryGetValue("CalculateHandToHandMaxDamage", out del))
                return del(handToHandSkill);

            // Daggerfall Chronicles table lists hand-to-hand skills of 80 and above (45 through 79 are omitted)
            // as if they give max damage of (handToHandSkill / 5) + 2, but the hand-to-hand damage display in the character sheet
            // in classic Daggerfall shows it as continuing to be (handToHandSkill / 5) + 1
            return (handToHandSkill / 5) + 1;
        }

        public static int CalculateAttackDamage(DaggerfallEntity attacker, DaggerfallEntity target, int weaponEquipSlot, int enemyAnimStateRecord)
        {
            if (attacker == null || target == null)
                return 0;

            int minBaseDamage = 0;
            int maxBaseDamage = 0;
            int damageModifiers = 0;
            int damage = 0;
            int chanceToHitMod = 0;
            int backstabbingLevel = 0;
            PlayerEntity player = GameManager.Instance.PlayerEntity;
            Items.DaggerfallUnityItem weapon = attacker.ItemEquipTable.GetItem((Items.EquipSlots)weaponEquipSlot);
            short skillID = 0;

            // Choose whether weapon-wielding enemies use their weapons or weaponless attacks.
            // In classic, weapon-wielding enemies use the damage values of their weapons
            // instead of their weaponless values.
            // For some enemies this gives lower damage than similar-tier monsters
            // and the weaponless values seems more appropriate, so here
            // enemies will choose to use their weaponless attack if it is more damaging.
            EnemyEntity AIAttacker = attacker as EnemyEntity;
            if (AIAttacker != null && weapon != null)
            {
                int weaponAverage = ((minBaseDamage + maxBaseDamage) / 2);
                int noWeaponAverage = ((AIAttacker.MobileEnemy.MinDamage + AIAttacker.MobileEnemy.MaxDamage) / 2);

                if (noWeaponAverage > weaponAverage)
                {
                    // Use hand-to-hand
                    weapon = null;
                }
            }

            if (weapon != null)
            {
                // If the attacker is using a weapon, check if the material is high enough to damage the target
                if (target.MinMetalToHit > (Items.WeaponMaterialTypes)weapon.NativeMaterialValue)
                {
                    if (attacker == player)
                    {
                        DaggerfallUI.Instance.PopupMessage(UserInterfaceWindows.HardStrings.materialIneffective);
                    }

                    return 0;
                }

                // Get weapon skill used
                skillID = weapon.GetWeaponSkillIDAsShort();
            }
            else
            {
                skillID = (short)DFCareer.Skills.HandToHand;
            }

            chanceToHitMod = attacker.Skills.GetLiveSkillValue(skillID);

            EnemyEntity AITarget = null;
            if (target != player)
            {
                AITarget = target as EnemyEntity;
            }

            if (attacker == player)
            {
                // Apply swing modifiers. Not applied to hand-to-hand in classic.
                FPSWeapon onscreenWeapon = GameManager.Instance.WeaponManager.ScreenWeapon;

                if (onscreenWeapon != null)
                {
                    // The Daggerfall manual groups diagonal slashes to the left and right as if they are the same, but they are different.
                    // Classic does not apply swing modifiers to hand-to-hand.
                    if (onscreenWeapon.WeaponState == WeaponStates.StrikeUp)
                    {
                        damageModifiers += -4;
                        chanceToHitMod += 10;
                    }
                    if (onscreenWeapon.WeaponState == WeaponStates.StrikeDownRight)
                    {
                        damageModifiers += -2;
                        chanceToHitMod += 5;
                    }
                    if (onscreenWeapon.WeaponState == WeaponStates.StrikeDownLeft)
                    {
                        damageModifiers += 2;
                        chanceToHitMod += -5;
                    }
                    if (onscreenWeapon.WeaponState == WeaponStates.StrikeDown)
                    {
                        damageModifiers += 4;
                        chanceToHitMod += -10;
                    }
                }

                if (weapon != null)
                {
                    // Apply weapon proficiency
                    if (((int)attacker.Career.ExpertProficiencies & weapon.GetWeaponSkillUsed()) != 0)
                    {
                        damageModifiers += ((attacker.Level / 3) + 1);
                        chanceToHitMod += attacker.Level;
                    }
                }
                // Apply hand-to-hand proficiency. Hand-to-hand proficiency is not applied in classic.
                else if (((int)attacker.Career.ExpertProficiencies & (int)(DFCareer.ProficiencyFlags.HandToHand)) != 0)
                {
                    damageModifiers += ((attacker.Level / 3) + 1);
                    chanceToHitMod += attacker.Level;
                }

                // Apply racial bonuses
                if (weapon != null)
                {
                    if (player.RaceTemplate.ID == (int)Races.DarkElf)
                    {
                        damageModifiers += (attacker.Level / 4);
                        chanceToHitMod += (attacker.Level / 4);
                    }
                    else if (skillID == (short)DFCareer.Skills.Archery)
                    {
                        if (player.RaceTemplate.ID == (int)Races.WoodElf)
                        {
                            damageModifiers += (attacker.Level / 3);
                            chanceToHitMod += (attacker.Level / 3);
                        }
                    }
                    else if (player.RaceTemplate.ID == (int)Races.Redguard)
                    {
                        damageModifiers += (attacker.Level / 3);
                        chanceToHitMod += (attacker.Level / 3);
                    }
                }

                // Apply backstabbing
                if (enemyAnimStateRecord % 5 > 2) // Facing away from player
                {
                    chanceToHitMod += attacker.Skills.GetLiveSkillValue(DFCareer.Skills.Backstabbing);
                    attacker.TallySkill(DFCareer.Skills.Backstabbing, 1); // backstabbing
                    backstabbingLevel = attacker.Skills.GetLiveSkillValue(DFCareer.Skills.Backstabbing);
                }
            }

            // Choose struck body part
            int[] bodyParts = { 0, 0, 1, 1, 1, 2, 2, 2, 3, 3, 3, 3, 4, 4, 4, 4, 5, 5, 5, 6 };
            int struckBodyPart = bodyParts[UnityEngine.Random.Range(0, bodyParts.Length)];

            // Get damage for weaponless attacks
            if (skillID == (short)DFCareer.Skills.HandToHand)
            {
                if (attacker == player)
                {
                    if (CalculateSuccessfulHit(attacker, target, chanceToHitMod, null, struckBodyPart))
                    {
                        minBaseDamage = CalculateHandToHandMinDamage(attacker.Skills.GetLiveSkillValue(DFCareer.Skills.HandToHand));
                        maxBaseDamage = CalculateHandToHandMaxDamage(attacker.Skills.GetLiveSkillValue(DFCareer.Skills.HandToHand));
                        damage = UnityEngine.Random.Range(minBaseDamage, maxBaseDamage + 1);

                        // Apply damage modifiers.
                        damage += damageModifiers;
                        // Apply strength modifier. It is not applied in classic despite what the in-game description for the Strength attribute says.
                        damage += DamageModifier(attacker.Stats.LiveStrength);

                        // Handle backstabbing
                        if (backstabbingLevel > 0 && UnityEngine.Random.Range(1, 101) <= backstabbingLevel)
                        {
                            damage *= 3;
                            string backstabMessage = UserInterfaceWindows.HardStrings.successfulBackstab;
                            DaggerfallUI.Instance.PopupMessage(backstabMessage);
                        }
                    }
                }
                else // attacker is monster
                {
                    // Handle multiple attacks by AI
                    int attackNumber = 0;
                    while (attackNumber < 3) // Classic supports up to 5 attacks but no monster has more than 3
                    {
                        if (attackNumber == 0)
                        {
                            minBaseDamage = AIAttacker.MobileEnemy.MinDamage;
                            maxBaseDamage = AIAttacker.MobileEnemy.MaxDamage;
                        }
                        else if (attackNumber == 1)
                        {
                            minBaseDamage = AIAttacker.MobileEnemy.MinDamage2;
                            maxBaseDamage = AIAttacker.MobileEnemy.MaxDamage2;
                        }
                        else if (attackNumber == 2)
                        {
                            minBaseDamage = AIAttacker.MobileEnemy.MinDamage3;
                            maxBaseDamage = AIAttacker.MobileEnemy.MaxDamage3;
                        }

                        if (DFRandom.rand() % 100 < 50 && minBaseDamage > 0 && CalculateSuccessfulHit(attacker, target, chanceToHitMod, null, struckBodyPart))
                        {
                            int hitDamage = UnityEngine.Random.Range(minBaseDamage, maxBaseDamage + 1);
                            // Apply special monster attack effects
                            if (hitDamage > 0)
                                OnMonsterHit(AIAttacker, target, hitDamage);

                            // TODO: Apply Ring of Namira effect

                            damage += hitDamage;
                        }

                        ++attackNumber;
                    }
                }
            }
            // Handle weapon attacks
            else
            {
                if (CalculateSuccessfulHit(attacker, target, chanceToHitMod, weapon, struckBodyPart))
                {
                    damage = UnityEngine.Random.Range(weapon.GetBaseDamageMin(), weapon.GetBaseDamageMax() + 1);
                    damage += damageModifiers;

                    if (AITarget != null && AITarget.CareerIndex == (int)MonsterCareers.SkeletalWarrior)
                    {
                        // Apply edged-weapon damage modifier for Skeletal Warrior
                        if ((weapon.flags & 0x10) == 0)
                            damage /= 2;

                        // Apply silver weapon damage modifier for Skeletal Warrior
                        // Arena applies a silver weapon damage bonus for undead enemies, which
                        // is probably where this comes from.
                        if (weapon.NativeMaterialValue == (int)Items.WeaponMaterialTypes.Silver)
                            damage *= 2;
                    }

                    // TODO: Apply strength bonus from Mace of Molag Bal

                    // Apply strength modifier
                    damage += DamageModifier(attacker.Stats.LiveStrength);

                    // Apply material modifier.
                    // The in-game display in Daggerfall of weapon damages with material modifiers is incorrect. The material modifier is half of what the display suggests.
                    damage += weapon.GetWeaponMaterialModifier();

                    if (damage < 1)
                        damage = 0;

                    damage += GetBonusOrPenaltyByEnemyType(attacker, AITarget);

                    if (backstabbingLevel > 1 && UnityEngine.Random.Range(1, 100 + 1) <= backstabbingLevel)
                    {
                        damage *= 3;
                        string backstabMessage = UserInterfaceWindows.HardStrings.successfulBackstab;
                        DaggerfallUI.Instance.PopupMessage(backstabMessage);
                    }
                }
            }

            damage = Mathf.Max(0, damage);

            // If damage was done by a weapon, damage the weapon and armor of the hit body part.
            // In classic, shields are never damaged, only armor specific to the hitbody part is.
            // Here, if an equipped shield covers the hit body part, it takes damage instead.
            if (weapon != null && damage > 0)
            {
                // TODO: Inflict poison
                // TODO: Inflict weapon magic effects
                // TODO: If attacker is AI, apply Ring of Namira effect
                weapon.DamageThroughPhysicalHit(damage, attacker);

                Items.DaggerfallUnityItem shield = target.ItemEquipTable.GetItem(Items.EquipSlots.LeftHand);
                bool shieldTakesDamage = false;
                if (shield != null)
                {
                    Items.BodyParts[] protectedBodyParts = shield.GetShieldProtectedBodyParts();

                    for (int i = 0; (i < protectedBodyParts.Length) && !shieldTakesDamage; i++)
                    {
                        if (protectedBodyParts[i] == (Items.BodyParts)struckBodyPart)
                            shieldTakesDamage = true;
                    }
                }

                if (shieldTakesDamage)
                    shield.DamageThroughPhysicalHit(damage, target);
                else
                {
                    Items.EquipSlots hitSlot = Items.DaggerfallUnityItem.GetEquipSlotForBodyPart((Items.BodyParts)struckBodyPart);
                    Items.DaggerfallUnityItem armor = target.ItemEquipTable.GetItem(hitSlot);
                    if (armor != null)
                        armor.DamageThroughPhysicalHit(damage, target);
                }
            }

            return damage;
        }

        public static void OnMonsterHit(EnemyEntity attacker, DaggerfallEntity target, int damage)
        {
            byte[] diseaseListA = { 1 };
            byte[] diseaseListB = { 1, 3, 5 };
            byte[] diseaseListC = { 1, 2, 3, 4, 5, 6, 8, 9, 11, 13, 14 };

            switch (attacker.CareerIndex)
            {
                case (int)MonsterCareers.Rat:
                    // In classic rat can only give plague (diseaseListA), but DF Chronicles says plague, stomach rot and brain fever (diseaseListB).
                    // Don't know which was intended. Using B since it has more variety.
                    if (UnityEngine.Random.Range(1, 100 + 1) <= 5)
                        InflictDisease(target, diseaseListB);
                    break;
                case (int)MonsterCareers.GiantBat:
                    // Classic uses 2% chance, but DF Chronicles says 5% chance. Not sure which was intended.
                    if (UnityEngine.Random.Range(1, 100 + 1) <= 2)
                        InflictDisease(target, diseaseListB);
                    break;
                case (int)MonsterCareers.Spider:
                    // if target does not have paralyze (spell id 66), cast it
                    break;
                case (int)MonsterCareers.Werewolf:
                    //uint random = DFRandom.rand();
                    //if (random < 400)
                    //  InflictLycanthropy (werewolf version)
                    break;
                case (int)MonsterCareers.Nymph:
                    FatigueDamage(target, damage);
                    break;
                case (int)MonsterCareers.Wereboar:
                    //uint random = DFRandom.rand();
                    //if (random < 400)
                    //  InflictLycanthropy (wereboar version)
                    break;
                case (int)MonsterCareers.Zombie:
                    // Nothing in classic. DF Chronicles says 2% chance of disease, which seems like it was probably intended.
                    // Diseases listed in DF Chronicles match those of mummy (except missing cholera, probably a mistake)
                    if (UnityEngine.Random.Range(1, 100 + 1) <= 2)
                        InflictDisease(target, diseaseListC);
                    break;
                case (int)MonsterCareers.Mummy:
                    if (UnityEngine.Random.Range(1, 100 + 1) <= 5)
                        InflictDisease(target, diseaseListC);
                    break;
                case (int)MonsterCareers.GiantScorpion:
                    // if target does not have paralyze (spell id 66), cast it
                    break;
                case (int)MonsterCareers.Vampire:
                case (int)MonsterCareers.VampireAncient:
                    uint random = DFRandom.rand();
                    if (random >= 400)
                    {
                        if (UnityEngine.Random.Range(1, 100 + 1) <= 2)
                            InflictDisease(target, diseaseListA);
                    }
                    // else
                    //{
                    //    InflictVampirism
                    //}
                    break;
                case (int)MonsterCareers.Lamia:
                    // Nothing in classic, but DF Chronicles says 2 pts of fatigue damage per health damage
                    FatigueDamage(target, damage);
                    break;
                default:
                    break;
            }
        }

        public static bool CalculateSuccessfulHit(DaggerfallEntity attacker, DaggerfallEntity target, int chanceToHitMod, Items.DaggerfallUnityItem weapon, int struckBodyPart)
        {
            if (attacker == null || target == null)
                return false;

            int chanceToHit = chanceToHitMod;
            PlayerEntity player = GameManager.Instance.PlayerEntity;
            EnemyEntity AITarget = target as EnemyEntity;

            int armorValue = 0;

            // Apply hit mod from character biography
            if (target == player)
            {
                chanceToHit -= player.BiographyAvoidHitMod;
            }

            // Get armor value for struck body part
            if (struckBodyPart <= target.ArmorValues.Length)
            {
                armorValue = target.ArmorValues[struckBodyPart];
            }

            chanceToHit += armorValue;

            // Apply adrenaline rush modifiers.
            if (attacker.Career.AdrenalineRush && attacker.CurrentHealth < (attacker.MaxHealth / 8))
            {
                chanceToHit += 5;
            }

            if (target.Career.AdrenalineRush && target.CurrentHealth < (target.MaxHealth / 8))
            {
                chanceToHit -= 5;
            }

            // Apply luck modifier.
            chanceToHit += ((attacker.Stats.LiveLuck - target.Stats.LiveLuck) / 10);

            // Apply agility modifier.
            chanceToHit += ((attacker.Stats.LiveAgility - target.Stats.LiveAgility) / 10);

            // Apply weapon material modifier.
            if (weapon != null)
            {
                chanceToHit += (weapon.GetWeaponMaterialModifier() * 10);
            }

            // Apply dodging modifier.
            // This modifier is bugged in classic and the attacker's dodging skill is used rather than the target's.
            // DF Chronicles says the dodging calculation is (dodging / 10), but it actually seems to be (dodging / 4).
            chanceToHit -= (target.Skills.GetLiveSkillValue(DFCareer.Skills.Dodging) / 4);

            // Apply critical strike modifier.
            if (UnityEngine.Random.Range(0, 100 + 1) < attacker.Skills.GetLiveSkillValue(DFCareer.Skills.CriticalStrike))
            {
                chanceToHit += (attacker.Skills.GetLiveSkillValue(DFCareer.Skills.CriticalStrike) / 10);
            }

            // Apply monster modifier.
            if ((target != player) && (AITarget.EntityType == EntityTypes.EnemyMonster))
            {
                chanceToHit += 40;
            }

            // DF Chronicles says -60 is applied at the end, but it actually seems to be -50.
            chanceToHit -= 50;

            Mathf.Clamp(chanceToHit, 3, 97);

            int roll = UnityEngine.Random.Range(0, 100 + 1);

            if (roll <= chanceToHit)
                return true;
            else
                return false;
        }

        static int GetBonusOrPenaltyByEnemyType(DaggerfallEntity attacker, EnemyEntity AITarget)
        {
            Formula_2de del;
            if (formula_2de.TryGetValue("GetBonusOrPenaltyByEnemyType", out del))
                return del(attacker, AITarget);

            if (attacker == null || AITarget == null)
                return 0;

            int damage = 0;
            // Apply bonus or penalty by opponent type.
            // In classic this is broken and only works if the attack is done with a weapon that has the maximum number of enchantments.
            if (AITarget.GetEnemyGroup() == DFCareer.EnemyGroups.Undead)
            {
                if (((int)attacker.Career.UndeadAttackModifier & (int)DFCareer.AttackModifier.Bonus) != 0)
                {
                    damage += attacker.Level;
                }
                if (((int)attacker.Career.UndeadAttackModifier & (int)DFCareer.AttackModifier.Phobia) != 0)
                {
                    damage -= attacker.Level;
                }
            }
            else if (AITarget.GetEnemyGroup() == DFCareer.EnemyGroups.Daedra)
            {
                if (((int)attacker.Career.DaedraAttackModifier & (int)DFCareer.AttackModifier.Bonus) != 0)
                {
                    damage += attacker.Level;
                }
                if (((int)attacker.Career.DaedraAttackModifier & (int)DFCareer.AttackModifier.Phobia) != 0)
                {
                    damage -= attacker.Level;
                }
            }
            else if (AITarget.GetEnemyGroup() == DFCareer.EnemyGroups.Humanoid)
            {
                if (((int)attacker.Career.HumanoidAttackModifier & (int)DFCareer.AttackModifier.Bonus) != 0)
                {
                    damage += attacker.Level;
                }
                if (((int)attacker.Career.HumanoidAttackModifier & (int)DFCareer.AttackModifier.Phobia) != 0)
                {
                    damage -= attacker.Level;
                }
            }
            else if (AITarget.GetEnemyGroup() == DFCareer.EnemyGroups.Animals)
            {
                if (((int)attacker.Career.AnimalsAttackModifier & (int)DFCareer.AttackModifier.Bonus) != 0)
                {
                    damage += attacker.Level;
                }
                if (((int)attacker.Career.AnimalsAttackModifier & (int)DFCareer.AttackModifier.Phobia) != 0)
                {
                    damage -= attacker.Level;
                }
            }

            return damage;
        }

        static int SavingThrow(DFCareer.EffectFlags effectFlags, DaggerfallEntity target)
        {
            PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;

            int savingThrow = 50;
            DFCareer.Tolerance toleranceFlags = 0;
            int biographyMod = 0;

            if (effectFlags == DFCareer.EffectFlags.Disease)
            {
                toleranceFlags = target.Career.Disease;
                if (target == playerEntity)
                    biographyMod = playerEntity.BiographyResistDiseaseMod;
            }

            if (toleranceFlags == DFCareer.Tolerance.Immune)
                return 0;
            if (toleranceFlags == DFCareer.Tolerance.CriticalWeakness)
                return 100;
            if (toleranceFlags == DFCareer.Tolerance.Resistant)
                savingThrow = 25;
            if (toleranceFlags == DFCareer.Tolerance.LowTolerance)
                savingThrow = 75;

            savingThrow += biographyMod;
            Mathf.Clamp(savingThrow, 5, 95);

            int percentDamageOrDuration = 0;
            int roll = UnityEngine.Random.Range(1, 100 + 1);

            if (roll <= savingThrow)
            {
                // Percent damage/duration is prorated at within 20 of failed roll, as described in DF Chronicles
                if (savingThrow - 20 <= roll)
                    percentDamageOrDuration = 100 - 5 * (savingThrow - roll);
                else
                    percentDamageOrDuration = 0;
            }
            else
                percentDamageOrDuration = 100;

            return percentDamageOrDuration;
        }

        #endregion

        #region Enemies

        public static void InflictDisease(DaggerfallEntity target, byte[] diseaseList)
        {
            PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;

            if (target != playerEntity)
                return;

            // Player cannot catch diseases at level 1 in classic. Maybe to keep new players from dying at the start of the game.
            if (playerEntity.Level != 1)
            {
                // Return if disease resisted
                if (SavingThrow(DFCareer.EffectFlags.Disease, target) == 0)
                    return;
                int diseaseChoice = UnityEngine.Random.Range(diseaseList[0], diseaseList.Length);
                Diseases disease = (Diseases)(diseaseChoice + 100); // Adding 100 to match to enums
                playerEntity.Diseases.Add(new DaggerfallDisease(disease));
            }
        }

        public static void FatigueDamage(DaggerfallEntity target, int damage)
        {
            // In classic, nymphs do 10-30 fatigue damage per hit, and lamias don't do any.
            // DF Chronicles says nymphs have "Energy Leech", which is a spell in
            // the game and not what they use, and for lamias "Every 1 pt of health damage = 2 pts of fatigue damage".
            // Lamia health damage is 5-15. Multiplying this by 2 may be where 10-30 came from. Nymph health damage is 1-5.
            // Not sure what was intended here, but using the "Every 1 pt of health damage = 2 pts of fatigue damage"
            // seems sensible, since the fatigue damage will scale to the health damage and lamias are a higher level opponent
            // than nymphs and will do more fatigue damage.
            target.SetFatigue(target.CurrentFatigue - ((damage * 2) * 64));

            // TODO: When nymphs drain the player's fatigue level to 0, the player is supposed to fall asleep for 14 days
            // and then wake up, according to DF Chronicles. This doesn't work correctly in classic. Classic does advance
            // time 14 days but the player dies like normal because of the "collapse from exhaustion near monsters = die" code.
        }

        // Generates health for enemy classes based on level and class
        public static int RollEnemyClassMaxHealth(int level, int hitPointsPerLevel)
        {
            Formula_2i del;
            if (formula_2i.TryGetValue("RollEnemyClassMaxHealth", out del))
                return del(level, hitPointsPerLevel);

            const int baseHealth = 10;
            int maxHealth = baseHealth;

            for (int i = 0; i < level; i++)
            {
                maxHealth += UnityEngine.Random.Range(1, hitPointsPerLevel + 1);
            }
            return maxHealth;
        }

        #endregion

        #region Holidays

        public static int GetHolidayId(uint gameMinutes, int regionIndex)
        {
            // Gives which regions celebrate which holidays.
            // Values are region IDs, index is holiday ID. 0xFF means all regions celebrate the holiday.
            byte[] regionIndexCelebratingHoliday = { 0xFF, 0x19, 0x01, 0xFF, 0x1D, 0x05, 0x19, 0x06, 0x3C, 0xFF, 0x29, 0x1A,
                0xFF, 0x02, 0x19, 0x01, 0x0E, 0x12, 0x14, 0xFF, 0xFF, 0x1C, 0x21, 0x1F, 0x2C, 0xFF, 0x12,
                0x23, 0xFF, 0x38, 0xFF, 0x01, 0x30, 0x29, 0x0B, 0x16, 0xFF, 0xFF, 0x11, 0x17, 0x14, 0x01,
                0xFF, 0x13, 0xFF, 0x33, 0x3C, 0x2E, 0xFF, 0xFF, 0x01, 0x2D, 0x18 };

            // Gives the day of the year that holidays are celebrated on.
            // Value are days of the year, index is holiday ID.
            short[] holidayDaysOfYear = { 0x01, 0x02, 0x0C, 0x0F, 0x10, 0x12, 0x20, 0x23, 0x26, 0x2E, 0x39, 0x3A,
                0x43, 0x45, 0x55, 0x56, 0x5B, 0x67, 0x6E, 0x76, 0x7F, 0x81, 0x8C, 0x96, 0x97, 0xA6, 0xAD,
                0xAE, 0xBE, 0xC0, 0xC8, 0xD1, 0xD4, 0xDD, 0xE0, 0xE7, 0xED, 0xF3, 0xF6, 0xFC, 0x103, 0x113,
                0x11B, 0x125, 0x12C, 0x12F, 0x134, 0x13E, 0x140, 0x159, 0x15C, 0x162, 0x163 };

            int holidayID = 0;
            uint dayOfYear = gameMinutes % 518400 / 1440 + 1;
            if (dayOfYear <= 355)
            {
                while (holidayID < 53)
                {
                    if ((regionIndexCelebratingHoliday[holidayID] == 0xFF || regionIndexCelebratingHoliday[holidayID] == regionIndex + 1)
                        && dayOfYear == holidayDaysOfYear[holidayID])
                    {
                        return holidayID + 1;
                    }
                    ++holidayID;
                }
            }

            // Not a holiday
            return 0;
        }

        #endregion

        #region Commerce

        public static int CalculateRoomCost(int daysToRent)
        {
            Formula_1i del;
            if (formula_1i.TryGetValue("CalculateRoomCost", out del))
                return del(daysToRent);

            int cost = 0;
            int dayOfYear = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.DayOfYear;
            if (dayOfYear <= 46 && dayOfYear + daysToRent > 46)
                cost = 7 * (daysToRent - 1);  // No charge for Heart's Day
            else
                cost = 7 * daysToRent;

            // If player is member of region's knightly order, or rank 4 or higher in any knightly order
            // DaggerfallUI.MessageBox(UserInterfaceWindows.HardStrings.roomFreeForKnightSuchAsYou);
            // cost = 0;
            if (cost == 0) // Only renting for Heart's Day
            {
                DaggerfallUI.MessageBox(UserInterfaceWindows.HardStrings.roomFreeDueToHeartsDay);
            }

            return cost;
        }

        public static int CalculateCost(int baseItemValue, int shopQuality)
        {
            Formula_2i del;
            if (formula_2i.TryGetValue("CalculateCost", out del))
                return del(baseItemValue, shopQuality);

            int cost = baseItemValue;

            if (cost < 1)
                cost = 1;

            cost = ApplyRegionalPriceAdjustment(cost);
            cost = 2 * (cost * (shopQuality - 10) / 100 + cost);

            return cost;
        }

        public static int CalculateItemRepairCost(int baseItemValue, int shopQuality, int condition, int max, Guild guild)
        {
            // Don't cost already repaired item
            if (condition == max)
                return 0;

            int cost = baseItemValue;

            cost = 10 * baseItemValue / 100;

            if (cost < 1)
                cost = 1;

            cost = CalculateCost(cost, shopQuality);

            if (guild != null)
                cost = guild.ReducedRepairCost(cost);

            return cost;
        }

        public static int CalculateItemIdentifyCost(int baseItemValue, Guild guild)
        {
            // Free on Witches Festival
            uint minutes = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime();
            PlayerGPS gps = GameManager.Instance.PlayerGPS;
            if (gps.HasCurrentLocation)
            {
                int holidayId = GetHolidayId(minutes, gps.CurrentRegionIndex);
                if (holidayId == (int)DFLocation.Holidays.Witches_Festival)
                    return 0;
            }
            int cost = (25 * baseItemValue) >> 8;

            if (guild != null)
                cost = guild.ReducedIdentifyCost(cost);

            return cost;
        }

        public static int CalculateTradePrice(int cost, int shopQuality, bool selling)
        {
            Formula_3i del;
            if (formula_3i.TryGetValue("CalculateTradePrice", out del))
                return del(cost, shopQuality, selling ? 1 : 0);

            PlayerEntity player = GameManager.Instance.PlayerEntity;
            int merchant_mercantile_level = 5 * (shopQuality - 10) + 50;
            int merchant_personality_level = 5 * (shopQuality - 10) + 50;

            int delta_mercantile;
            int delta_personality;
            int amount = 0;

            if (selling)
            {
                delta_mercantile = (((100 - merchant_mercantile_level) << 8) / 200 + 128) * (((player.Skills.GetLiveSkillValue(DFCareer.Skills.Mercantile)) << 8) / 200 + 128) >> 8;
                delta_personality = (((100 - merchant_personality_level) << 8) / 200 + 128) * ((player.Stats.LivePersonality << 8) / 200 + 128) >> 8;
                amount = ((((179 * delta_mercantile) >> 8) + ((51 * delta_personality) >> 8)) * cost) >> 8;
            }
            else // buying
            {
                delta_mercantile = ((merchant_mercantile_level << 8) / 200 + 128) * (((100 - (player.Skills.GetLiveSkillValue(DFCareer.Skills.Mercantile))) << 8) / 200 + 128) >> 8;
                delta_personality = ((merchant_personality_level << 8) / 200 + 128) * (((100 - player.Stats.LivePersonality) << 8) / 200 + 128) >> 8 << 6;
                amount = ((((192 * delta_mercantile) >> 8) + (delta_personality >> 8)) * cost) >> 8;
            }

            return amount;
        }

        public static int ApplyRegionalPriceAdjustment(int cost)
        {
            Formula_1i del;
            if (formula_1i.TryGetValue("ApplyRegionalPriceAdjustment", out del))
                return del(cost);

            int adjustedCost;
            int currentRegionIndex;
            PlayerEntity player = GameManager.Instance.PlayerEntity;
            PlayerGPS gps = GameManager.Instance.PlayerGPS;
            if (gps.HasCurrentLocation)
                currentRegionIndex = gps.CurrentRegionIndex;
            else
                return cost;

            adjustedCost = cost * player.RegionData[currentRegionIndex].PriceAdjustment / 1000;
            if (adjustedCost < 1)
                adjustedCost = 1;
            return adjustedCost;
        }

        public static void RandomizeInitialRegionalPrices(ref PlayerEntity.RegionDataRecord[] regionData)
        {
            for (int i = 0; i < regionData.Length; i++)
                regionData[i].PriceAdjustment = (ushort)(UnityEngine.Random.Range(0, 501) + 750);
        }

        public static void UpdateRegionalPrices(ref PlayerEntity.RegionDataRecord[] regionData, int times)
        {
            PlayerEntity player = GameManager.Instance.PlayerEntity;
            FactionFile.FactionData merchantsFaction;
            if (!player.FactionData.GetFactionData(510, out merchantsFaction))
                return;

            for (int i = 0; i < regionData.Length; ++i)
            {
                FactionFile.FactionData regionFaction;
                if (player.FactionData.FindFactionByTypeAndRegion(7, i + 1, out regionFaction))
                {
                    for (int j = 0; j < times; ++j)
                    {
                        int chanceOfPriceRise = ((merchantsFaction.power) - (regionFaction.power)) / 5
                            + 50 - (regionData[i].PriceAdjustment - 1000) / 25;
                        if (UnityEngine.Random.Range(0, 101) >= chanceOfPriceRise)
                            regionData[i].PriceAdjustment = (ushort)(49 * regionData[i].PriceAdjustment / 50);
                        else
                            regionData[i].PriceAdjustment = (ushort)(51 * regionData[i].PriceAdjustment / 50);

                        Mathf.Clamp(regionData[i].PriceAdjustment, 250, 4000);
                        if (regionData[i].PriceAdjustment <= 2000)
                        {
                            if (regionData[i].PriceAdjustment >= 500)
                            {
                                player.TurnOffConditionFlag(i, PlayerEntity.RegionDataFlags.PricesHigh);
                                player.TurnOffConditionFlag(i, PlayerEntity.RegionDataFlags.PricesLow);
                            }
                            else
                                player.TurnOnConditionFlag(i, PlayerEntity.RegionDataFlags.PricesLow);
                        }
                        else
                            player.TurnOnConditionFlag(i, PlayerEntity.RegionDataFlags.PricesHigh);
                    }
                }
            }
        }

        #endregion

        #region Spell Costs

        /// <summary>
        /// Performs complete gold and spellpoint costs for an array of effects.
        /// Also calculates multipliers for target type.
        /// </summary>
        /// <param name="effectEntries">EffectEntry array for spell.</param>
        /// <param name="targetType">Target type of spell.</param>
        /// <param name="totalGoldCostOut">Total gold cost out.</param>
        /// <param name="totalSpellPointCostOut">Total spellpoint cost out.</param>
        public static void CalculateTotalEffectCosts(EffectEntry[] effectEntries, TargetTypes targetType, out int totalGoldCostOut, out int totalSpellPointCostOut)
        {
            totalGoldCostOut = 0;
            totalSpellPointCostOut = 0;

            // Add costs for each active effect slot
            for (int i = 0; i < effectEntries.Length; i++)
            {
                if (string.IsNullOrEmpty(effectEntries[i].Key))
                    continue;

                int goldCost, spellPointCost;
                CalculateEffectCosts(effectEntries[i], out goldCost, out spellPointCost);
                totalGoldCostOut += goldCost;
                totalSpellPointCostOut += spellPointCost;
            }

            // Multipliers for target type
            totalGoldCostOut = ApplyTargetCostMultiplier(totalGoldCostOut, targetType);
            totalSpellPointCostOut = ApplyTargetCostMultiplier(totalSpellPointCostOut, targetType);
        }

        public static void CalculateEffectCosts(EffectEntry effectEntry, out int goldCostOut, out int spellPointCostOut)
        {
            int activeComponents = 0;
            goldCostOut = 0;
            spellPointCostOut = 0;

            // Get effect template
            IEntityEffect effectTemplate = GameManager.Instance.EntityEffectBroker.GetEffectTemplate(effectEntry.Key);
            if (effectTemplate == null)
                return;

            // Get related skill
            int skillValue = GameManager.Instance.PlayerEntity.Skills.GetLiveSkillValue((DFCareer.Skills)effectTemplate.Properties.MagicSkill);

            // Duration costs
			int durationGoldCost = 0;
            if (effectTemplate.Properties.SupportDuration)
            {
                activeComponents++;
                GetEffectComponentCosts(
                    out durationGoldCost,
                    effectTemplate.Properties.DurationCosts,
                    effectEntry.Settings.DurationBase,
                    effectEntry.Settings.DurationPlus,
                    effectEntry.Settings.DurationPerLevel,
                    skillValue);

                //Debug.LogFormat("Duration: gold {0} spellpoints {1}", durationGoldCost, durationSpellPointCost);
            }

            // Chance costs
			int chanceGoldCost = 0;
            if (effectTemplate.Properties.SupportChance)
            {
                activeComponents++;
                GetEffectComponentCosts(
                    out chanceGoldCost,
                    effectTemplate.Properties.ChanceCosts,
                    effectEntry.Settings.ChanceBase,
                    effectEntry.Settings.ChancePlus,
                    effectEntry.Settings.ChancePerLevel,
                    skillValue);

                //Debug.LogFormat("Chance: gold {0} spellpoints {1}", chanceGoldCost, chanceSpellPointCost);
            }

            // Magnitude costs
			int magnitudeGoldCost = 0;
            if (effectTemplate.Properties.SupportMagnitude)
            {
                activeComponents++;
                int magnitudeBase = (effectEntry.Settings.MagnitudeBaseMax + effectEntry.Settings.MagnitudeBaseMin) / 2;
                int magnitudePlus = (effectEntry.Settings.MagnitudePlusMax + effectEntry.Settings.MagnitudePlusMin) / 2;
                GetEffectComponentCosts(
                    out magnitudeGoldCost,
                    effectTemplate.Properties.MagnitudeCosts,
                    magnitudeBase,
                    magnitudePlus,
                    effectEntry.Settings.MagnitudePerLevel,
                    skillValue);

                //Debug.LogFormat("Magnitude: gold {0} spellpoints {1}", magnitudeGoldCost, magnitudeSpellPointCost);
            }

            // Add gold costs together and calculate spellpoint cost from the result
			goldCostOut = durationGoldCost + chanceGoldCost + magnitudeGoldCost;
			spellPointCostOut = goldCostOut * (110 - skillValue)/400;

            //Debug.LogFormat("Costs: gold {0} spellpoints {1}", finalGoldCost, finalSpellPointCost);
        }

        public static int ApplyTargetCostMultiplier(int cost, TargetTypes targetType)
        {
            switch (targetType)
            {
                case TargetTypes.CasterOnly:                // x1.0
                case TargetTypes.ByTouch:
                    // These do not change costs, just including here for completeness
                    break;
                case TargetTypes.SingleTargetAtRange:       // x1.5
                    cost = (int)(cost * 1.5f);
                    break;
                case TargetTypes.AreaAroundCaster:          // x2.0
                    cost = (int)(cost * 2.0f);
                    break;
                case TargetTypes.AreaAtRange:               // x2.5
                    cost = (int)(cost * 2.5f);
                    break;
            }

            return cost;
        }

        static void GetEffectComponentCosts(
            out int goldCost,
            EffectCosts costs,
            int starting,
            int increase,
            int perLevel,
            int skillValue)
        {

			//Calculate effect gold cost, spellpoint cost is calculated from gold cost after adding up for duration, chance and magnitude
			goldCost = trunc(costs.OffsetGold + costs.CostA * starting + costs.CostB * trunc(increase / perLevel));
        }

        // Just makes formulas more readable
        static int trunc(double value) { return (int)Math.Truncate(value); }

        #endregion
    }
}
