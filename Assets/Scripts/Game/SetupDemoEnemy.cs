using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// Sets up enemy using demo components.
    /// Currently using this component to setup enemy entity.
    /// TODO: Revise enemy instantiation and entity assignment.
    /// </summary>
    [RequireComponent(typeof(EnemyMotor))]
    public class SetupDemoEnemy : MonoBehaviour
    {
        public MobileTypes EnemyType = MobileTypes.SkeletalWarrior;
        public MobileReactions EnemyReaction = MobileReactions.Hostile;
        public MobileGender EnemyGender = MobileGender.Unspecified;
        public byte ClassicSpawnDistanceType = 0;

        DaggerfallEntityBehaviour entityBehaviour;

        void Awake()
        {
            // Must have an entity behaviour
            entityBehaviour = GetComponent<DaggerfallEntityBehaviour>();
            if (!entityBehaviour)
                gameObject.AddComponent<DaggerfallEntityBehaviour>();
        }

        void Start()
        {
            // Disable this game object if missing mobile setup
            DaggerfallMobileUnit dfMobile = GetMobileBillboardChild();
            if (dfMobile == null)
                this.gameObject.SetActive(false);
            if (!dfMobile.Summary.IsSetup)
                this.gameObject.SetActive(false);
        }

        /// <summary>
        /// Sets up enemy based on current settings.
        /// </summary>
        public void ApplyEnemySettings(MobileGender gender)
        {
            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;
            Dictionary<int, MobileEnemy> enemyDict = GameObjectHelper.EnemyDict;
            MobileEnemy mobileEnemy = enemyDict[(int)EnemyType];

            // Find mobile unit in children
            DaggerfallMobileUnit dfMobile = GetMobileBillboardChild();
            if (dfMobile != null)
            {
                // Setup mobile billboard
                Vector2 size = Vector2.one;
                mobileEnemy.Gender = gender;
                dfMobile.SetEnemy(dfUnity, mobileEnemy, EnemyReaction, ClassicSpawnDistanceType);

                // Setup controller
                CharacterController controller = GetComponent<CharacterController>();
                if (controller)
                {
                    // Set base height from sprite
                    size = dfMobile.Summary.RecordSizes[0];
                    controller.height = size.y;

                    // Reduce height of flying creatures as their wing animation makes them taller than desired
                    // This helps them get through doors while aiming for player eye height
                    if (dfMobile.Summary.Enemy.Behaviour == MobileBehaviour.Flying)
                        controller.height /= 2f;

                    // Limit maximum controller height
                    // Some particularly tall sprites (e.g. giants) require this hack to get through doors
                    if (controller.height > 1.78f)
                    {
                        // Adjust center so that sprite doesn't sink into the ground
                        Vector3 newCenter = controller.center;
                        newCenter.y += (1.78f - controller.height) / 2;
                        controller.center = newCenter;
                        controller.height = 1.78f;
                    }

                    controller.gameObject.layer = LayerMask.NameToLayer("Enemies");
                }

                // Setup sounds
                EnemySounds enemySounds = GetComponent<Game.EnemySounds>();
                if (enemySounds)
                {
                    enemySounds.MoveSound = (SoundClips)dfMobile.Summary.Enemy.MoveSound;
                    enemySounds.BarkSound = (SoundClips)dfMobile.Summary.Enemy.BarkSound;
                    enemySounds.AttackSound = (SoundClips)dfMobile.Summary.Enemy.AttackSound;
                }

                // Setup entity
                if (entityBehaviour)
                {
                    EnemyEntity entity = new EnemyEntity(entityBehaviour);
                    entityBehaviour.Entity = entity;

                    // Enemies are initially added to same world context as player
                    entity.WorldContext = GameManager.Instance.PlayerEnterExit.WorldContext;

                    int enemyIndex = (int)EnemyType;
                    if (enemyIndex >= 0 && enemyIndex <= 42)
                    {
                        entityBehaviour.EntityType = EntityTypes.EnemyMonster;
                        entity.SetEnemyCareer(mobileEnemy, entityBehaviour.EntityType);
                    }
                    else if (enemyIndex >= 128 && enemyIndex <= 146)
                    {
                        entityBehaviour.EntityType = EntityTypes.EnemyClass;
                        entity.SetEnemyCareer(mobileEnemy, entityBehaviour.EntityType);
                    }
                    else
                    {
                        entityBehaviour.EntityType = EntityTypes.None;
                    }
                }
            }
        }

        /// <summary>
        /// Change enemy settings and configure in a single call.
        /// </summary>
        /// <param name="enemyType">Enemy type.</param>
        public void ApplyEnemySettings(MobileTypes enemyType, MobileReactions enemyReaction, MobileGender gender, byte classicSpawnDistanceType = 0)
        {
            EnemyType = enemyType;
            EnemyReaction = enemyReaction;
            ClassicSpawnDistanceType = classicSpawnDistanceType;
            ApplyEnemySettings(gender);
        }

        /// <summary>
        /// Change enemy settings and configure in a single call.
        /// </summary>
        public void ApplyEnemySettings(EntityTypes entityType, int careerIndex, MobileGender gender, bool isHostile = true)
        {
            // Get mobile type based on entity type and career index
            MobileTypes mobileType;
            if (entityType == EntityTypes.EnemyMonster)
                mobileType = (MobileTypes)careerIndex;
            else if (entityType == EntityTypes.EnemyClass)
                mobileType = (MobileTypes)(careerIndex + 128);
            else
                return;

            MobileReactions enemyReaction = (isHostile) ? MobileReactions.Hostile : MobileReactions.Passive;
            MobileGender enemyGender = gender;

            ApplyEnemySettings(mobileType, enemyReaction, enemyGender);
        }

        public void AlignToGround()
        {
            CharacterController controller = GetComponent<CharacterController>();
            if (controller != null)
                GameObjectHelper.AlignControllerToGround(controller);
        }

        /// <summary>
        /// Finds mobile billboard in children.
        /// </summary>
        /// <returns>DaggerfallMobileUnit.</returns>
        public DaggerfallMobileUnit GetMobileBillboardChild()
        {
            return GetComponentInChildren<DaggerfallMobileUnit>();
        }
    }
}