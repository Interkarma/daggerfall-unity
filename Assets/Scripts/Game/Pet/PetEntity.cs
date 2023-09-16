using UnityEngine;
using DaggerfallConnect;
using DaggerfallWorkshop.Game.Formulas;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Items;

namespace Game.Pet
{
    /// <summary>
    /// The parameters involved in creating an enemy loot pile
    /// </summary>
    public class PetLootSpawnedEventArgs : System.EventArgs
    {
        public ItemCollection Items { get; set; }
    }

    /// <summary>
    /// Implements DaggerfallEntity with properties specific to enemies.
    /// </summary>
    public class PetEntity : EnemyEntity
    {
        #region Fields

        public static System.EventHandler<PetLootSpawnedEventArgs> OnLootSpawned;

        int careerIndex = -1;
        EntityTypes entityType = EntityTypes.None;
        MobileEnemy mobileEnemy;
        bool pickpocketByPlayerAttempted = false;
        int questFoeSpellQueueIndex = -1;
        int questFoeItemQueueIndex = -1;
        bool suppressInfighting = false;

        // From FALL.EXE offset 0x1C0F14
        static byte[] ImpSpells = {0x07, 0x0A, 0x1D, 0x2C};
        static byte[] GhostSpells = {0x22};
        static byte[] OrcShamanSpells = {0x06, 0x07, 0x16, 0x19, 0x1F};
        static byte[] WraithSpells = {0x1C, 0x1F};
        static byte[] FrostDaedraSpells = {0x10, 0x14};
        static byte[] FireDaedraSpells = {0x0E, 0x19};
        static byte[] DaedrothSpells = {0x16, 0x17, 0x1F};
        static byte[] VampireSpells = {0x33};
        static byte[] SeducerSpells = {0x34, 0x43};
        static byte[] VampireAncientSpells = {0x08, 0x32};
        static byte[] DaedraLordSpells = {0x08, 0x0A, 0x0E, 0x3C, 0x43};
        static byte[] LichSpells = {0x08, 0x0A, 0x0E, 0x22, 0x3C};
        static byte[] AncientLichSpells = {0x08, 0x0A, 0x0E, 0x1D, 0x1F, 0x22, 0x3C};

        static byte[][] EnemyClassSpells =
        {
            FrostDaedraSpells, DaedrothSpells, OrcShamanSpells, VampireAncientSpells, DaedraLordSpells, LichSpells,
            AncientLichSpells
        };

        #endregion

        #region Properties

        public EntityTypes EntityType => entityType;

        public int CareerIndex => careerIndex;

        public MobileEnemy MobileEnemy => mobileEnemy;

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

        public bool WabbajackActive { get; set; }

        public delegate void EnemyStartingEquipment(PlayerEntity player, EnemyEntity enemyEntity, int variant);

        public static EnemyStartingEquipment AssignEnemyEquipment =
            DaggerfallUnity.Instance.ItemHelper.AssignEnemyStartingEquipment;

        #endregion

        #region Constructors

        public PetEntity(DaggerfallEntityBehaviour entityBehaviour)
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
                    level = mobileEnemy.Level;
                    maxHealth = Random.Range(mobileEnemy.MinHealth, mobileEnemy.MaxHealth + 1);
                    for (int i = 0; i < ArmorValues.Length; i++)
                    {
                        ArmorValues[i] = (sbyte) (mobileEnemy.ArmorValue * 5);
                    }
                }
                else
                {
                    // Default like a class enemy
                    level = GameManager.Instance.PlayerEntity.Level;
                    maxHealth = FormulaHelper.RollEnemyClassMaxHealth(level, career.HitPointsPerLevel);
                }
            }
            else if (entityType == EntityTypes.EnemyMonster)
            {
                careerIndex = mobileEnemy.ID;
                career = GetMonsterCareerTemplate((MonsterCareers) careerIndex);
                stats.SetPermanentFromCareer(career);

                // Enemy monster has predefined level, health and armor values.
                // Armor values can be modified below by equipment.
                level = mobileEnemy.Level;
                maxHealth = UnityEngine.Random.Range(mobileEnemy.MinHealth, mobileEnemy.MaxHealth + 1);
                for (int i = 0; i < ArmorValues.Length; i++)
                {
                    ArmorValues[i] = (sbyte) (mobileEnemy.ArmorValue * 5);
                }
            }
            else if (entityType == EntityTypes.EnemyClass)
            {
                careerIndex = mobileEnemy.ID - 128;
                career = GetClassCareerTemplate((ClassCareers) careerIndex);
                stats.SetPermanentFromCareer(career);

                // Enemy class is levelled to player and uses similar health rules
                // City guards are 3 to 6 levels above the player
                level = GameManager.Instance.PlayerEntity.Level;
                if (careerIndex == (int) MobileTypes.Knight_CityWatch - 128)
                    level += UnityEngine.Random.Range(3, 7);

                maxHealth = FormulaHelper.RollEnemyClassMaxHealth(level, career.HitPointsPerLevel);
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

            short skillsLevel = (short) ((level * 5) + 30);
            if (skillsLevel > 100)
            {
                skillsLevel = 100;
            }

            for (int i = 0; i <= DaggerfallSkills.Count; i++)
            {
                skills.SetPermanentSkillValue(i, skillsLevel);
            }


            FillVitalSigns();
        }

        #endregion
    }
}