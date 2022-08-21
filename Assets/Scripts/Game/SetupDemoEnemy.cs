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
        public bool AlliedToPlayer = false;
        public byte ClassicSpawnDistanceType = 0;

        DaggerfallEntityBehaviour entityBehaviour;

        public GameObject LightAura;

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
            MobileUnit dfMobile = GetMobileBillboardChild();
            if (dfMobile == null)
                this.gameObject.SetActive(false);
            if (!dfMobile.IsSetup)
                this.gameObject.SetActive(false);
        }


        enum ControllerJustification
        {
            TOP,
            CENTER,
            BOTTOM
        }

        static void AdjustControllerHeight(CharacterController controller, float newHeight, ControllerJustification justification)
        {
            Vector3 newCenter = controller.center;
            switch (justification)
            {
                case ControllerJustification.TOP:
                    newCenter.y -= (newHeight - controller.height) / 2;
                    break;

                case ControllerJustification.BOTTOM:
                    newCenter.y += (newHeight - controller.height) / 2;
                    break;

                case ControllerJustification.CENTER:
                    // do nothing, centered is normal CharacterController behavior
                    break;
            }
            controller.height = newHeight;
            controller.center = newCenter;
        }

        /// <summary>
        /// Sets up enemy based on current settings.
        /// </summary>
        public void ApplyEnemySettings(MobileGender gender)
        {
            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;
            Dictionary<int, MobileEnemy> enemyDict = GameObjectHelper.EnemyDict;

            if (!enemyDict.TryGetValue((int)EnemyType, out MobileEnemy mobileEnemy))
                return;

            if (AlliedToPlayer)
                mobileEnemy.Team = MobileTeams.PlayerAlly;

            // Find mobile unit in children
            MobileUnit dfMobile = GetMobileBillboardChild();
            if (dfMobile != null)
            {
                // Setup mobile billboard
                Vector2 size = Vector2.one;
                mobileEnemy.Gender = gender;
                mobileEnemy.Reactions = EnemyReaction;
                dfMobile.SetEnemy(dfUnity, mobileEnemy, EnemyReaction, ClassicSpawnDistanceType);

                // Setup controller
                CharacterController controller = GetComponent<CharacterController>();
                if (controller)
                {
                    // Set base height from sprite
                    size = dfMobile.GetSize();
                    controller.height = size.y;

                    // Reduce height of flying creatures as their wing animation makes them taller than desired
                    // This helps them get through doors while aiming for player eye height
                    if (dfMobile.Enemy.Behaviour == MobileBehaviour.Flying)
                        // (in frame 0 wings are in high position, assume body is  the lower half)
                        AdjustControllerHeight(controller, controller.height / 2, ControllerJustification.BOTTOM);

                    // Limit minimum controller height
                    // Stops very short characters like rats from being walked upon
                    if (controller.height < 1.6f)
                        AdjustControllerHeight(controller, 1.6f, ControllerJustification.BOTTOM);

                    controller.gameObject.layer = LayerMask.NameToLayer("Enemies");
                }

                // Setup sounds
                EnemySounds enemySounds = GetComponent<Game.EnemySounds>();
                if (enemySounds)
                {
                    enemySounds.MoveSound = (SoundClips)dfMobile.Enemy.MoveSound;
                    enemySounds.BarkSound = (SoundClips)dfMobile.Enemy.BarkSound;
                    enemySounds.AttackSound = (SoundClips)dfMobile.Enemy.AttackSound;
                }

                MeshRenderer meshRenderer = dfMobile.GetComponent<MeshRenderer>();
                if (meshRenderer)
                {
                    if (dfMobile.Enemy.Behaviour == MobileBehaviour.Spectral)
                    {
                        meshRenderer.material.shader = Shader.Find(MaterialReader._DaggerfallGhostShaderName);
                        meshRenderer.material.SetFloat("_Cutoff", 0.1f);
                    }
                    if (dfMobile.Enemy.NoShadow)
                    {
                        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    }
                    if (dfMobile.Enemy.GlowColor != null)
                    {
                        meshRenderer.receiveShadows = false;
                        GameObject enemyLightGameObject = Instantiate(LightAura);
                        enemyLightGameObject.transform.parent = dfMobile.transform;
                        enemyLightGameObject.transform.localPosition = new Vector3(0, 0.3f, 0.2f);
                        Light enemyLight = enemyLightGameObject.GetComponent<Light>();
                        enemyLight.color = (Color)dfMobile.Enemy.GlowColor;
                        enemyLight.shadows = DaggerfallUnity.Settings.DungeonLightShadows ? LightShadows.Soft : LightShadows.None;
                    }
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
                    else if (DaggerfallEntity.GetCustomCareerTemplate(enemyIndex) != null)
                    {
                        if (DaggerfallEntity.IsClassEnemyId(enemyIndex))
                        {
                            entityBehaviour.EntityType = EntityTypes.EnemyClass;
                        }
                        else
                        {
                            entityBehaviour.EntityType = EntityTypes.EnemyMonster;
                        }
                        entity.SetEnemyCareer(mobileEnemy, entityBehaviour.EntityType);
                    }
                    else
                    {
                        entityBehaviour.EntityType = EntityTypes.None;
                    }
                }

                // Add special behaviour for Daedra Seducer mobiles
                if (dfMobile.Enemy.ID == (int)MobileTypes.DaedraSeducer)
                {
                    dfMobile.gameObject.AddComponent<DaedraSeducerMobileBehaviour>();
                }
            }
        }

        /// <summary>
        /// Change enemy settings and configure in a single call.
        /// </summary>
        /// <param name="enemyType">Enemy type.</param>
        public void ApplyEnemySettings(MobileTypes enemyType, MobileReactions enemyReaction, MobileGender gender, byte classicSpawnDistanceType = 0, bool alliedToPlayer = false)
        {
            EnemyType = enemyType;
            EnemyReaction = enemyReaction;
            ClassicSpawnDistanceType = classicSpawnDistanceType;
            AlliedToPlayer = alliedToPlayer;
            ApplyEnemySettings(gender);
        }

        /// <summary>
        /// Change enemy settings and configure in a single call.
        /// </summary>
        public void ApplyEnemySettings(EntityTypes entityType, int careerIndex, MobileGender gender, bool isHostile = true, bool alliedToPlayer = false)
        {
            // Get mobile type based on entity type and career index
            MobileTypes mobileType;

            // For classic enemies, careerIndex is equal to enemyId for monsters, or enemyId - 128 for class enemies (ex: Mage, enemyId=128, careerIndex=0)
            // For custom enemies, we just always store the enemyId in careerIndex, even if class type
            if (careerIndex < 256)
            {                
                if (entityType == EntityTypes.EnemyMonster)
                    mobileType = (MobileTypes)careerIndex;
                else if (entityType == EntityTypes.EnemyClass)
                    mobileType = (MobileTypes)(careerIndex + 128);
                else
                    return;
            }
            else
            {
                mobileType = (MobileTypes)careerIndex;
            }

            MobileReactions enemyReaction = (isHostile) ? MobileReactions.Hostile : MobileReactions.Passive;
            MobileGender enemyGender = gender;

            ApplyEnemySettings(mobileType, enemyReaction, enemyGender, alliedToPlayer: alliedToPlayer);
        }

        public void AlignToGround()
        {
            CharacterController controller = GetComponent<CharacterController>();
            if (controller != null)
                GameObjectHelper.AlignControllerToGround(controller);
        }

        /// <summary>
        /// Finds mobile billboard or custom implementation in children.
        /// </summary>
        /// <returns>Mobile Unit component.</returns>
        public MobileUnit GetMobileBillboardChild()
        {
#if UNITY_EDITOR
            // Get component from prefab in edit mode
            if (!Application.isPlaying)
                return GetComponentInChildren<DaggerfallMobileUnit>();
#endif

            // Get default or custom implementation
            return GetComponent<DaggerfallEnemy>().MobileUnit;
        }
    }
}