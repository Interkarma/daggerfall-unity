// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes: All additions or modifications that differ from the source code copyright (c) 2021-2022 Osorkon
//

using DaggerfallConnect;
using DaggerfallConnect.Save;
using DaggerfallWorkshop.Game.Formulas;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.MagicAndEffects;
using DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects;
using DaggerfallWorkshop.Game.Utility;
using UnityEngine;

namespace DaggerfallWorkshop.Game.Entity
{
    /// <summary>
    /// The parameters involved in creating an enemy loot pile
    /// </summary>
    public class EnemyLootSpawnedEventArgs : System.EventArgs
    {
        /// <summary>
        /// The Mobile object used for the enemy
        /// </summary>
        public MobileEnemy MobileEnemy { get; set; }

        /// <summary>
        /// The Career template of the enemy
        /// </summary>
        public DFCareer EnemyCareer { get; set; }

        /// <summary>
        /// The collection containing all the items of the loot pile. New items can be added
        /// </summary>
        public ItemCollection Items { get; set; }
    }

    /// <summary>
    /// Implements DaggerfallEntity with properties specific to enemies.
    /// </summary>
    public class EnemyEntity : DaggerfallEntity
    {
        #region Fields

        public static System.EventHandler<EnemyLootSpawnedEventArgs> OnLootSpawned;

        int careerIndex = -1;
        EntityTypes entityType = EntityTypes.None;
        MobileEnemy mobileEnemy;
        bool pickpocketByPlayerAttempted = false;
        int questFoeSpellQueueIndex = -1;
        int questFoeItemQueueIndex = -1;
        bool suppressInfighting = false;

        // From FALL.EXE offset 0x1C0F14

        // [OSORKON] The hexadecimal numbers are referenced in classic Daggerfall's SPELL.STD file, which has a
        // list of 99 or so different spells. Changing enemy spells is super easy - all you have to do is find the hex
        // number for that spell effect and plug that into the correct array. I used UESP's SPELL.STD page for reference.
        // I greatly increased enemy spell variety, as knowing what spells to expect takes most of the challenge away.
        // I didn't change Frost or Fire Daedra spells much - I added Frostbite to Frost Daedra spells and God's Fire
        // to Fire Daedra spells.
        static byte[] ImpSpells            = { 0x08, 0x0E, 0x1D, 0x1F, 0x32, 0x33, 0x19, 0x1C, 0x43, 0x34, 0x17, 0x10, 0x14, 0x09, 0x1B, 0x1E, 0x20, 0x23, 0x24, 0x27, 0x35, 0x36, 0x37, 0x40 };
        static byte[] OrcShamanSpells      = { 0x08, 0x0E, 0x1D, 0x1F, 0x32, 0x33, 0x19, 0x1C, 0x43, 0x34, 0x17, 0x10, 0x14, 0x09, 0x1B, 0x1E, 0x20, 0x23, 0x24, 0x27, 0x35, 0x36, 0x37, 0x40 };
        static byte[] WraithSpells         = { 0x08, 0x0E, 0x1D, 0x1F, 0x32, 0x33, 0x19, 0x1C, 0x43, 0x34, 0x17, 0x10, 0x14, 0x09, 0x1B, 0x1E, 0x20, 0x23, 0x24, 0x27, 0x35, 0x36, 0x37, 0x40 };
        static byte[] FrostDaedraSpells    = { 0x10, 0x14, 0x03 };
        static byte[] FireDaedraSpells     = { 0x0E, 0x19, 0x20 };
        static byte[] DaedrothSpells       = { 0x08, 0x0E, 0x1D, 0x1F, 0x32, 0x33, 0x19, 0x1C, 0x43, 0x34, 0x17, 0x10, 0x14, 0x09, 0x1B, 0x1E, 0x20, 0x23, 0x24, 0x27, 0x35, 0x36, 0x37, 0x40 };
        static byte[] VampireSpells        = { 0x08, 0x0E, 0x1D, 0x1F, 0x32, 0x33, 0x19, 0x1C, 0x43, 0x34, 0x17, 0x10, 0x14, 0x09, 0x1B, 0x1E, 0x20, 0x23, 0x24, 0x27, 0x35, 0x36, 0x37, 0x40 };
        static byte[] SeducerSpells        = { 0x08, 0x0E, 0x1D, 0x1F, 0x32, 0x33, 0x19, 0x1C, 0x43, 0x34, 0x17, 0x10, 0x14, 0x09, 0x1B, 0x1E, 0x20, 0x23, 0x24, 0x27, 0x35, 0x36, 0x37, 0x40 };
        static byte[] DaedraLordSpells     = { 0x08, 0x0E, 0x1D, 0x1F, 0x32, 0x33, 0x19, 0x1C, 0x43, 0x34, 0x17, 0x10, 0x14, 0x09, 0x1B, 0x1E, 0x20, 0x23, 0x24, 0x27, 0x35, 0x36, 0x37, 0x40 };
        static byte[] LichSpells           = { 0x08, 0x0E, 0x1D, 0x1F, 0x32, 0x33, 0x19, 0x1C, 0x43, 0x34, 0x17, 0x10, 0x14, 0x09, 0x1B, 0x1E, 0x20, 0x23, 0x24, 0x27, 0x35, 0x36, 0x37, 0x40 };
        static byte[] AncientLichSpells    = { 0x08, 0x0E, 0x1D, 0x1F, 0x32, 0x33, 0x19, 0x1C, 0x43, 0x34, 0x17, 0x10, 0x14, 0x09, 0x1B, 0x1E, 0x20, 0x23, 0x24, 0x27, 0x35, 0x36, 0x37, 0x40 };
        static byte[][] EnemyClassSpells   = { FrostDaedraSpells, DaedrothSpells, OrcShamanSpells, DaedraLordSpells, LichSpells, AncientLichSpells };

        #endregion

        #region Properties

        public EntityTypes EntityType
        {
            get { return entityType; }
        }

        public int CareerIndex
        {
            get { return careerIndex; }
        }

        public MobileEnemy MobileEnemy
        {
            get { return mobileEnemy; }
        }

        public bool PickpocketByPlayerAttempted
        {
            get { return (pickpocketByPlayerAttempted); }
            set { pickpocketByPlayerAttempted = value; }
        }

        public int QuestFoeSpellQueueIndex
        {
            get { return questFoeSpellQueueIndex; }
            set { questFoeSpellQueueIndex = value; }
        }

        public int QuestFoeItemQueueIndex
        {
            get { return questFoeItemQueueIndex; }
            set { questFoeItemQueueIndex = value; }
        }

        /// <summary>
        /// Suppress enemy infighting for this entity.
        /// Entity will not target anyone but player and cannot be a target for infighting.
        /// One example of use is Daedra Secuder whose winged sprites have no facing other than directly forward to player.
        /// If Seducer participates in winged infighting their sprite can no longer align properly with controller facing.
        /// Seducer behaviour will disable infighting once they transform into winged variant so enemy combats player only.
        /// </summary>
        public bool SuppressInfighting
        {
            get { return suppressInfighting; }
            set { suppressInfighting = value; }
        }

        public bool SoulTrapActive { get; set; }

        public bool WabbajackActive { get; set; }

        public delegate void EnemyStartingEquipment(PlayerEntity player, EnemyEntity enemyEntity, int variant);
        public static EnemyStartingEquipment AssignEnemyEquipment = DaggerfallUnity.Instance.ItemHelper.AssignEnemyStartingEquipment;

        #endregion

        #region Constructors

        public EnemyEntity(DaggerfallEntityBehaviour entityBehaviour)
            : base(entityBehaviour)
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Assigns default entity settings.
        /// </summary>
        public override void SetEntityDefaults()
        {
        }

        /// <summary>
        /// Custom handling of SetHealth() for enemies to support soul trap.
        /// </summary>
        public override int SetHealth(int amount, bool restoreMode = false)
        {
            // Just do base if no soul trap active
            if (!SoulTrapActive)
                return base.SetHealth(amount, restoreMode);

            // Reduce health
            currentHealth = Mathf.Clamp(amount, 0, MaxHealth);
            if (currentHealth <= 0)
            {
                // Attempt soul trap and allow entity to die based on outcome
                if (AttemptSoulTrap())
                {
                    SoulTrapActive = false;
                    return base.SetHealth(amount, restoreMode);
                }
            }

            return currentHealth;
        }

        public override void Update(DaggerfallEntityBehaviour sender)
        {
            base.Update(sender);

            // Despawn city watch when active crime state returns to none
            // This can happen when exiting city area, after fast travel, or via console
            if (entityType == EntityTypes.EnemyClass &&
                careerIndex == (int)MobileTypes.Knight_CityWatch - 128 &&
                GameManager.Instance.PlayerEntity.CrimeCommitted == PlayerEntity.Crimes.None)
            {
                GameObject.Destroy(sender.gameObject);
            }
        }

        /// <summary>
        /// Attempt to trap a soul.
        /// </summary>
        /// <returns>True if entity is allowed to die after trap attempt.</returns>
        bool AttemptSoulTrap()
        {
            // Must have a peered DaggerfallEntityBehaviour and EntityEffectManager
            EntityEffectManager manager = (EntityBehaviour) ? EntityBehaviour.GetComponent<EntityEffectManager>() : null;
            if (!manager)
                return true;

            // Find the soul trap incumbent
            SoulTrap soulTrapEffect = (SoulTrap)manager.FindIncumbentEffect<SoulTrap>();
            if (soulTrapEffect == null)
                return true;

            // Roll chance for trap
            // If trap fails then entity should die as normal without trapping a soul
            // If trap succeeds and player has a free soul gem then entity should die after storing soul
            // If trap succeeds and player has no free soul gems then entity will not die until effect expires or fails
            if (soulTrapEffect.RollTrapChance())
            {
                // Attempt to fill an empty soul trap
                if (SoulTrap.FillEmptyTrapItem((MobileTypes)mobileEnemy.ID))
                {
                    // Trap filled, allow entity to die normally
                    DaggerfallUI.AddHUDText(TextManager.Instance.GetLocalizedText("trapSuccess"), 1.5f);
                    return true;
                }
                else
                {
                    // No empty gems, keep entity tethered to life - player is alerted so they know what's happening
                    currentHealth = 1;
                    DaggerfallUI.AddHUDText(TextManager.Instance.GetLocalizedText("trapNoneEmpty"));
                    return false;
                }
            }
            else
            {
                // Trap failed
                DaggerfallUI.AddHUDText(TextManager.Instance.GetLocalizedText("trapFail"), 1.5f);
                return true;
            }
        }

        public override void ClearConstantEffects()
        {
            base.ClearConstantEffects();
            SoulTrapActive = false;
        }

        /// <summary>
        /// Sets enemy career and prepares entity settings.
        /// </summary>
        public void SetEnemyCareer(MobileEnemy mobileEnemy, EntityTypes entityType)
        {
            // Try custom career first
            career = GetCustomCareerTemplate(mobileEnemy.ID);

            if (career != null)
            {
                // Custom enemy
                careerIndex = mobileEnemy.ID;
                stats.SetPermanentFromCareer(career);

                if (entityType == EntityTypes.EnemyMonster)
                {
                    // Default like a monster

                    // [OSORKON] If mods ever add custom monsters their level will vary up and
                    // down a bit, just like regular BOSSFALL monsters.
                    level = mobileEnemy.Level + UnityEngine.Random.Range(-2, 2 + 1);

                    // [OSORKON] I don't know what would happen if their level was 0 or less. I
                    // don't particularly want to find out.
                    if (level < 1)
                    {
                        level = 1;
                    }

                    maxHealth = Random.Range(mobileEnemy.MinHealth, mobileEnemy.MaxHealth + 1);
                    for (int i = 0; i < ArmorValues.Length; i++)
                    {
                        ArmorValues[i] = (sbyte)(mobileEnemy.ArmorValue * 5);
                    }
                }
                // [OSORKON] If mods ever add custom class enemies they will be unleveled starting
                // at player level 7, just like regular BOSSFALL class enemies.
                else if (GameManager.Instance.PlayerEntity.Level > 6)
                {
                    // Default like a class enemy
                    int roll = Dice100.Roll();

                    if (roll > 0)
                    {
                        if (roll > 1)
                        {
                            if (roll > 2)
                            {
                                if (roll > 3)
                                {
                                    if (roll > 6)
                                    {
                                        if (roll > 11)
                                        {
                                            if (roll > 20)
                                            {
                                                if (roll > 33)
                                                {
                                                    if (roll > 50)
                                                    {
                                                        if (roll > 67)
                                                        {
                                                            if (roll > 80)
                                                            {
                                                                if (roll > 89)
                                                                {
                                                                    if (roll > 94)
                                                                    {
                                                                        if (roll > 97)
                                                                        {
                                                                            if (roll > 98)
                                                                            {
                                                                                if (roll > 99)
                                                                                {
                                                                                    level = UnityEngine.Random.Range(18, 20 + 1);
                                                                                }
                                                                                else
                                                                                {
                                                                                    level = 17;
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                level = 16;
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            level = 15;
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        level = 14;
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    level = 13;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                level = 12;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            level = 11;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        level = 10;
                                                    }
                                                }
                                                else
                                                {
                                                    level = 9;
                                                }
                                            }
                                            else
                                            {
                                                level = 8;
                                            }
                                        }
                                        else
                                        {
                                            level = 7;
                                        }
                                    }
                                    else
                                    {
                                        level = 6;
                                    }
                                }
                                else
                                {
                                    level = 5;
                                }
                            }
                            else
                            {
                                level = 4;
                            }
                        }
                        else
                        {
                            level = UnityEngine.Random.Range(1, 3 + 1);
                        }
                    }

                    // [OSORKON] Custom class enemy health works the same as vanilla.
                    maxHealth = FormulaHelper.RollEnemyClassMaxHealth(level, career.HitPointsPerLevel);
                }
                else
                {
                    // [OSORKON] If player is level 6 or less custom class enemies will be within 2 levels
                    // of the player, just like regular BOSSFALL class enemies.
                    level = GameManager.Instance.PlayerEntity.Level + UnityEngine.Random.Range(-2, 2 + 1);

                    // [OSORKON] I don't want custom class enemy levels to go below 1.
                    if (level < 1)
                    {
                        level = 1;
                    }

                    // [OSORKON] Custom class enemy health works the same as vanilla.
                    maxHealth = FormulaHelper.RollEnemyClassMaxHealth(level, career.HitPointsPerLevel);
                }
            }
            else if (entityType == EntityTypes.EnemyMonster)
            {
                careerIndex = mobileEnemy.ID;
                career = GetMonsterCareerTemplate((MonsterCareers)careerIndex);
                stats.SetPermanentFromCareer(career);

                // Enemy monster has predefined level, health and armor values.
                // Armor values can be modified below by equipment.

                // [OSORKON] Enemy monster levels vary up and down a bit. This affects their accuracy,
                // dodging, and spell effectiveness, but doesn't change their armor or HP.
                level = mobileEnemy.Level + UnityEngine.Random.Range(-2, 2 + 1);

                // [OSORKON] Non-boss monster levels can't go below 1 and are capped at 20. Only Daedra
                // Seducers are potentially affected by the level 20 cap.
                if (level < 1)
                {
                    level = 1;
                }
                if (level > 20)
                {
                    level = 20;
                }

                // [OSORKON] This list manually sets boss levels. It doesn't change their armor or HP.
                if (careerIndex == (int)MobileTypes.Vampire)
                {
                    level = UnityEngine.Random.Range(21, 25 + 1);
                }
                else if (careerIndex == (int)MobileTypes.Lich)
                {
                    level = UnityEngine.Random.Range(21, 25 + 1);
                }
                else if (careerIndex == (int)MobileTypes.Dragonling_Alternate)
                {
                    level = UnityEngine.Random.Range(21, 30 + 1);
                }
                else if (careerIndex == (int)MobileTypes.OrcWarlord)
                {
                    level = UnityEngine.Random.Range(21, 30 + 1);
                }
                else if (careerIndex == (int)MobileTypes.VampireAncient)
                {
                    level = UnityEngine.Random.Range(26, 30 + 1);
                }
                else if (careerIndex == (int)MobileTypes.DaedraLord)
                {
                    level = UnityEngine.Random.Range(26, 30 + 1);
                }
                else if (careerIndex == (int)MobileTypes.AncientLich)
                {
                    level = UnityEngine.Random.Range(26, 30 + 1);
                }

                maxHealth = UnityEngine.Random.Range(mobileEnemy.MinHealth, mobileEnemy.MaxHealth + 1);
                for (int i = 0; i < ArmorValues.Length; i++)
                {
                    ArmorValues[i] = (sbyte)(mobileEnemy.ArmorValue * 5);
                }
            }
            else if (entityType == EntityTypes.EnemyClass)
            {
                careerIndex = mobileEnemy.ID - 128;
                career = GetClassCareerTemplate((ClassCareers)careerIndex);
                stats.SetPermanentFromCareer(career);

                // [OSORKON] This formula is responsible for BOSSFALL's unleveled class enemies. Their
                // levels are weighted to usually be around 10. Level 1 and 20 enemies are very rare.
                // Class enemies are unleveled once player is at least level 7.
                if (GameManager.Instance.PlayerEntity.Level > 6)
                {
                    int roll = Dice100.Roll();

                    if (roll > 0)
                    {
                        if (roll > 1)
                        {
                            if (roll > 2)
                            {
                                if (roll > 3)
                                {
                                    if (roll > 6)
                                    {
                                        if (roll > 11)
                                        {
                                            if (roll > 20)
                                            {
                                                if (roll > 33)
                                                {
                                                    if (roll > 50)
                                                    {
                                                        if (roll > 67)
                                                        {
                                                            if (roll > 80)
                                                            {
                                                                if (roll > 89)
                                                                {
                                                                    if (roll > 94)
                                                                    {
                                                                        if (roll > 97)
                                                                        {
                                                                            if (roll > 98)
                                                                            {
                                                                                if (roll > 99)
                                                                                {
                                                                                    level = UnityEngine.Random.Range(18, 20 + 1);
                                                                                }
                                                                                else
                                                                                {
                                                                                    level = 17;
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                level = 16;
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            level = 15;
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        level = 14;
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    level = 13;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                level = 12;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            level = 11;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        level = 10;
                                                    }
                                                }
                                                else
                                                {
                                                    level = 9;
                                                }
                                            }
                                            else
                                            {
                                                level = 8;
                                            }
                                        }
                                        else
                                        {
                                            level = 7;
                                        }
                                    }
                                    else
                                    {
                                        level = 6;
                                    }
                                }
                                else
                                {
                                    level = 5;
                                }
                            }
                            else
                            {
                                level = 4;
                            }
                        }
                        else
                        {
                            level = UnityEngine.Random.Range(1, 3 + 1);
                        }
                    }
                }
                // [OSORKON] If player is level 6 or lower, all class enemies will be within 2 levels of the
                // player. Their level can't go below 1.
                else
                {
                    level = GameManager.Instance.PlayerEntity.Level + UnityEngine.Random.Range(-2, 2 + 1);

                    if (level < 1)
                    {
                        level = 1;
                    }
                }

                // Enemy class is levelled to player and uses similar health rules
                // City guards are 3 to 6 levels above the player

                // [OSORKON] Guard levels are buffed compared to vanilla DFU. They can get a 10 level boost. HALT!
                if (careerIndex == (int)MobileTypes.Knight_CityWatch - 128)
                    level += UnityEngine.Random.Range(0, 10 + 1);

                // [OSORKON] This manually sets Assassins to boss levels, but only if player is at least level 7.
                if (GameManager.Instance.PlayerEntity.Level > 6 && careerIndex == (int)MobileTypes.Assassin - 128)
                    level = UnityEngine.Random.Range(21, 30 + 1);

                maxHealth = FormulaHelper.RollEnemyClassMaxHealth(level, career.HitPointsPerLevel);

                // [OSORKON] Once player is at least level 7, Assassin HP is set to their boss range.
                if (GameManager.Instance.PlayerEntity.Level > 6 && careerIndex == (int)MobileTypes.Assassin - 128)
                    maxHealth = UnityEngine.Random.Range(100, 300 + 1);
            }
            else
            {
                career = new DFCareer();
                careerIndex = -1;
                return;
            }

            this.mobileEnemy = mobileEnemy;
            this.entityType = entityType;
            name = career.Name;
            minMetalToHit = mobileEnemy.MinMetalToHit;
            team = mobileEnemy.Team;

            // [OSORKON] BOSSFALL enemies scale up in skill faster - vanilla scales by 5/level.
            // BOSSFALL enemies have a higher skill cap of 180, compared to vanilla's 100.
            short skillsLevel = (short)((level * 7) + 30);
            if (skillsLevel > 180)
            {
                skillsLevel = 180;
            }

            for (int i = 0; i <= DaggerfallSkills.Count; i++)
            {
                skills.SetPermanentSkillValue(i, skillsLevel);
            }

            // Generate loot table items
            DaggerfallLoot.GenerateItems(mobileEnemy.LootTableKey, items);

            // Enemy classes and some monsters use equipment

            // [OSORKON] This check is responsible for high level enemy loot scaling to their level. If I didn't
            // make bosses and high level monsters use this, all enemy loot would be generated by loot table key
            // only, which does not scale with enemy level. I plan on refining this system in v1.3.
            if (entityType == EntityTypes.EnemyClass || careerIndex == (int)MonsterCareers.OrcSergeant
                || careerIndex == (int)MonsterCareers.OrcShaman || careerIndex == (int)MonsterCareers.OrcWarlord
                || careerIndex == (int)MonsterCareers.FrostDaedra || careerIndex == (int)MonsterCareers.FireDaedra
                || careerIndex == (int)MonsterCareers.Daedroth || careerIndex == (int)MonsterCareers.Vampire
                || careerIndex == (int)MonsterCareers.DaedraSeducer || careerIndex == (int)MonsterCareers.VampireAncient
                || careerIndex == (int)MonsterCareers.DaedraLord || careerIndex == (int)MonsterCareers.Lich
                || careerIndex == (int)MonsterCareers.AncientLich || careerIndex == (int)MonsterCareers.Dragonling_Alternate)
            {
                // [OSORKON] I added 1 to the maximum range. There are 3 possible variants and the third is never
                // used for enemy classes in vanilla. Adds more variety.
                SetEnemyEquipment(UnityEngine.Random.Range(0, 2 + 1));
            }

            // Assign spell lists
            if (entityType == EntityTypes.EnemyMonster)
            {
                // [OSORKON] I condensed the vanilla if/else if list, all monsters use the same spells in BOSSFALL.
                //  No need for extra checks. The only exceptions are Frost/Fire Daedra, who do use different spell lists.
                if (careerIndex == (int)MonsterCareers.Imp || careerIndex == (int)MonsterCareers.OrcShaman
                    || careerIndex == (int)MonsterCareers.Wraith || careerIndex == (int)MonsterCareers.Daedroth
                    || careerIndex == (int)MonsterCareers.Vampire || careerIndex == (int)MonsterCareers.DaedraSeducer
                    || careerIndex == (int)MonsterCareers.DaedraLord || careerIndex == (int)MonsterCareers.Lich
                    || careerIndex == (int)MonsterCareers.AncientLich)
                    SetEnemySpells(ImpSpells);
                else if (careerIndex == (int)MonsterCareers.FrostDaedra)
                    SetEnemySpells(FrostDaedraSpells);
                else if (careerIndex == (int)MonsterCareers.FireDaedra)
                    SetEnemySpells(FireDaedraSpells);
            }
            else if (entityType == EntityTypes.EnemyClass && (mobileEnemy.CastsMagic))
            {
                // [OSORKON] I set enemy classes to use the same expanded spell list as enemy monsters.
                SetEnemySpells(AncientLichSpells);
            }

            // Chance of adding map
            DaggerfallLoot.RandomlyAddMap(mobileEnemy.MapChance, items);

            if (!string.IsNullOrEmpty(mobileEnemy.LootTableKey))
            {
                // Chance of adding potion
                DaggerfallLoot.RandomlyAddPotion(3, items);
                // Chance of adding potion recipe
                DaggerfallLoot.RandomlyAddPotionRecipe(2, items);
            }

            OnLootSpawned?.Invoke(this, new EnemyLootSpawnedEventArgs { MobileEnemy = mobileEnemy, EnemyCareer = career, Items = items });

            FillVitalSigns();
        }

        // [OSORKON] I greatly condensed this entire function, as most of the checks were made redundant by
        // changes I made elsewhere. Enemy armor no longer varies with their equipment, so I didn't
        // need the for loops vanilla had here.
        public void SetEnemyEquipment(int variant)
        {
            // Assign the enemies starting equipment.
            AssignEnemyEquipment(GameManager.Instance.PlayerEntity, this, variant);

            if (entityType == EntityTypes.EnemyClass)
            {
                // [OSORKON] This sets class enemy armor depending on their level. Setting this up to work properly
                // gave me endless headaches - I called MobileEnemy.Level in v1.1, which refers to ArmorValue
                // in EnemyBasics. Unfortunately, class enemies don't have an ArmorValue in EnemyBasics, so v1.1
                // class enemy armor didn't vary by their level. I eventually figured out the proper "level" object
                // to call. Live and learn...
                for (int i = 0; i < ArmorValues.Length; i++)
                {
                    ArmorValues[i] = (sbyte)(60 - (level * 2));

                    // [OSORKON] Once player is at least level 7, Assassins have boss armor. I botched this in v1.1,
                    // as I forgot player gets +40 to hit against monsters (but not classes) and set Assassin armor
                    // to a ridiculous -40. Fortunately I noticed the inconsistency and corrected it in v1.2.
                    if (GameManager.Instance.PlayerEntity.Level > 6 && careerIndex == (int)MobileTypes.Assassin - 128)
                    {
                        ArmorValues[i] = 0;
                    }
                }
            }

        }
        public void SetEnemySpells(byte[] spellList)
        {
            // Enemies don't follow same rule as player for maximum spell points

            // [OSORKON] For whatever reason, enemy spell costs in vanilla vary depending on player
            // spell skill level, which makes it impossible to precisely set how many spells an enemy
            // could cast - and that's exactly what I wanted to do. I solved this problem by manually
            // setting how much magicka each enemy spell would cost in EntityEffectManager. Once I did
            // that, I could precisely manage how many spells enemies could cast. This list sets enemy
            // mana pools, each spell costs 5 mana. Bosses above level 25 have effectively infinite mana.
            if (level > 0 && level < 8)
            {
                MaxMagicka = 9;
            }
            else if (level >= 8 && level < 13)
            {
                MaxMagicka = 14;
            }
            else if (level >= 13 && level < 16)
            {
                MaxMagicka = 19;
            }
            else if (level >= 16 && level < 18)
            {
                MaxMagicka = 24;
            }
            else if (level >= 18 && level < 20)
            {
                MaxMagicka = 29;
            }
            else if (level == 20)
            {
                MaxMagicka = 39;
            }
            else if (level >= 21 && level < 26)
            {
                MaxMagicka = 149;
            }
            else if (level >= 26)
            {
                MaxMagicka = 100000;
            }
            
            currentMagicka = MaxMagicka;

            // Add spells to enemy from standard list
            foreach (byte spellID in spellList)
            {
                SpellRecord.SpellRecordData spellData;
                GameManager.Instance.EntityEffectBroker.GetClassicSpellRecord(spellID, out spellData);
                if (spellData.index == -1)
                {
                    Debug.LogError("Failed to locate enemy spell in standard spells list.");
                    continue;
                }

                EffectBundleSettings bundle;
                if (!GameManager.Instance.EntityEffectBroker.ClassicSpellRecordDataToEffectBundleSettings(spellData, BundleTypes.Spell, out bundle))
                {
                    Debug.LogError("Failed to create effect bundle for enemy spell: " + spellData.spellName);
                    continue;
                }
                AddSpell(bundle);
            }
        }

        public DFCareer.EnemyGroups GetEnemyGroup()
        {
            switch (careerIndex)
            {
                case (int)MonsterCareers.Rat:
                case (int)MonsterCareers.GiantBat:
                case (int)MonsterCareers.GrizzlyBear:
                case (int)MonsterCareers.SabertoothTiger:
                case (int)MonsterCareers.Spider:
                case (int)MonsterCareers.Slaughterfish:
                case (int)MonsterCareers.GiantScorpion:
                case (int)MonsterCareers.Dragonling:
                case (int)MonsterCareers.Horse_Invalid:             // (grouped as undead in classic)
                case (int)MonsterCareers.Dragonling_Alternate:      // (grouped as undead in classic)
                    return DFCareer.EnemyGroups.Animals;
                case (int)MonsterCareers.Imp:
                case (int)MonsterCareers.Spriggan:
                case (int)MonsterCareers.Orc:
                case (int)MonsterCareers.Centaur:
                case (int)MonsterCareers.Werewolf:
                case (int)MonsterCareers.Nymph:
                case (int)MonsterCareers.OrcSergeant:
                case (int)MonsterCareers.Harpy:
                case (int)MonsterCareers.Wereboar:
                case (int)MonsterCareers.Giant:
                case (int)MonsterCareers.OrcShaman:
                case (int)MonsterCareers.Gargoyle:
                case (int)MonsterCareers.OrcWarlord:
                case (int)MonsterCareers.Dreugh:                    // (grouped as undead in classic)
                case (int)MonsterCareers.Lamia:                     // (grouped as undead in classic)
                    return DFCareer.EnemyGroups.Humanoid;
                case (int)MonsterCareers.SkeletalWarrior:
                case (int)MonsterCareers.Zombie:                    // (grouped as animal in classic)
                case (int)MonsterCareers.Ghost:
                case (int)MonsterCareers.Mummy:
                case (int)MonsterCareers.Wraith:
                case (int)MonsterCareers.Vampire:
                case (int)MonsterCareers.VampireAncient:
                case (int)MonsterCareers.Lich:
                case (int)MonsterCareers.AncientLich:
                    return DFCareer.EnemyGroups.Undead;
                case (int)MonsterCareers.FrostDaedra:
                case (int)MonsterCareers.FireDaedra:
                case (int)MonsterCareers.Daedroth:
                case (int)MonsterCareers.DaedraSeducer:
                case (int)MonsterCareers.DaedraLord:
                    return DFCareer.EnemyGroups.Daedra;
                case (int)MonsterCareers.FireAtronach:
                case (int)MonsterCareers.IronAtronach:
                case (int)MonsterCareers.FleshAtronach:
                case (int)MonsterCareers.IceAtronach:
                    return DFCareer.EnemyGroups.None;

                default:
                    return DFCareer.EnemyGroups.None;
            }
        }

        public DFCareer.Skills GetLanguageSkill()
        {
            if (entityType == EntityTypes.EnemyClass)
            {
                switch (careerIndex)
                {   // BCHG: classic uses Ettiquette for all

                    // [OSORKON] I added Sorcerers, Barbarians, and Rangers to the Streetwise list. Now class
                    // enemies are half pacified by Streetwise and the other half Etiquette. I like balance.
                    case (int)ClassCareers.Burglar:
                    case (int)ClassCareers.Rogue:
                    case (int)ClassCareers.Acrobat:
                    case (int)ClassCareers.Thief:
                    case (int)ClassCareers.Assassin:
                    case (int)ClassCareers.Nightblade:
                    case (int)ClassCareers.Sorcerer:
                    case (int)ClassCareers.Barbarian:
                    case (int)ClassCareers.Ranger:
                        return DFCareer.Skills.Streetwise;
                    default:
                        return DFCareer.Skills.Etiquette;
                }
            }

            switch (careerIndex)
            {
                case (int)MonsterCareers.Orc:
                case (int)MonsterCareers.OrcSergeant:
                case (int)MonsterCareers.OrcShaman:
                case (int)MonsterCareers.OrcWarlord:
                    return DFCareer.Skills.Orcish;

                case (int)MonsterCareers.Harpy:
                    return DFCareer.Skills.Harpy;

                case (int)MonsterCareers.Giant:
                case (int)MonsterCareers.Gargoyle:
                    return DFCareer.Skills.Giantish;

                case (int)MonsterCareers.Dragonling:
                case (int)MonsterCareers.Dragonling_Alternate:
                    return DFCareer.Skills.Dragonish;

                case (int)MonsterCareers.Nymph:
                case (int)MonsterCareers.Lamia:
                    return DFCareer.Skills.Nymph;

                case (int)MonsterCareers.FrostDaedra:
                case (int)MonsterCareers.FireDaedra:
                case (int)MonsterCareers.Daedroth:
                case (int)MonsterCareers.DaedraSeducer:
                case (int)MonsterCareers.DaedraLord:
                    return DFCareer.Skills.Daedric;

                case (int)MonsterCareers.Spriggan:
                    return DFCareer.Skills.Spriggan;

                case (int)MonsterCareers.Centaur:
                    return DFCareer.Skills.Centaurian;

                case (int)MonsterCareers.Imp:
                case (int)MonsterCareers.Dreugh:
                    return DFCareer.Skills.Impish;

                case (int)MonsterCareers.Vampire:
                case (int)MonsterCareers.VampireAncient:
                case (int)MonsterCareers.Lich:
                case (int)MonsterCareers.AncientLich:
                    return DFCareer.Skills.Etiquette;

                default:
                    return DFCareer.Skills.None;
            }
        }

        public int GetWeightInClassicUnits()
        {
            // [OSORKON] I greatly condensed this function. In combination with changes I made elsewhere, this
            // heavily nerfs knockback stunlocks with high damage attacks combined with high player SPD. 
            return 100000;
        }

        #endregion
    }
}
