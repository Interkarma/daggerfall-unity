using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Utility;
using UnityEngine;

namespace Game.Pet
{
    public class PetConfigurer : MonoBehaviour //todo make private
    {
        [SerializeField] public MobileTypes petType;
        [SerializeField] public MobileReactions reaction;
        [SerializeField] public MobileGender gender;
        [SerializeField] public byte classicSpawnDistanceType;
        [SerializeField] public GameObject lightAura;

        [SerializeField] private DaggerfallEntityBehaviour entityBehaviour;

        private void Start()
        {
            var dfMobile = GetMobileBillboardChild();
            if (dfMobile == null)
                gameObject.SetActive(false);
            if (!dfMobile.IsSetup)
                gameObject.SetActive(false);
        }

        public void ApplySettings(MobileTypes enemyType, MobileReactions enemyReaction, MobileGender gender,
            byte classicSpawnDistanceType = 0)
        {
            petType = enemyType;
            reaction = enemyReaction;
            this.classicSpawnDistanceType = classicSpawnDistanceType;
            this.gender = gender;
            ApplySettings();
        }

        public MobileUnit GetMobileBillboardChild()
        {
        #if UNITY_EDITOR
            if (!Application.isPlaying)
                return GetComponentInChildren<DaggerfallMobileUnit>();
        #endif
            return GetComponent<DaggerfallEnemy>().MobileUnit;
        }

        private void ApplySettings()
        {
            var dfUnity = DaggerfallUnity.Instance;
            var enemyDict = GameObjectHelper.EnemyDict;

            if (!enemyDict.TryGetValue((int) petType, out var mobileEnemy)) return;

            mobileEnemy.Team = MobileTeams.PlayerAlly;
            mobileEnemy.Gender = gender;
            mobileEnemy.Reactions = reaction;

            var dfMobile = GetMobileBillboardChild();

            if (dfMobile == null) return;

            dfMobile.SetEnemy(dfUnity, mobileEnemy, reaction, classicSpawnDistanceType);
            var controller = GetComponent<CharacterController>();

            if (controller)
            {
                var size = dfMobile.GetSize();
                controller.height = size.y;

                if (dfMobile.Enemy.Behaviour == MobileBehaviour.Flying)
                    // (in frame 0 wings are in high position, assume body is  the lower half)
                    AdjustControllerHeight(controller, controller.height / 2, ControllerJustification.Bottom);

                if (controller.height < 1.6f)
                    AdjustControllerHeight(controller, 1.6f, ControllerJustification.Bottom);
            }

            var sounds = GetComponent<EnemySounds>();
            if (sounds)
            {
                sounds.MoveSound = (SoundClips) dfMobile.Enemy.MoveSound;
                sounds.BarkSound = (SoundClips) dfMobile.Enemy.BarkSound;
                sounds.AttackSound = (SoundClips) dfMobile.Enemy.AttackSound;
            }

            var meshRenderer = dfMobile.GetComponent<MeshRenderer>();
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
                    var enemyLightGameObject = Instantiate(lightAura, dfMobile.transform, true);
                    enemyLightGameObject.transform.localPosition = new Vector3(0, 0.3f, 0.2f);

                    var enemyLight = enemyLightGameObject.GetComponent<Light>();
                    enemyLight.color = (Color) dfMobile.Enemy.GlowColor;
                    enemyLight.shadows = DaggerfallUnity.Settings.DungeonLightShadows
                        ? LightShadows.Soft
                        : LightShadows.None;
                }
            }

            if (entityBehaviour)
            {
                var entity = new PetEntity(entityBehaviour);
                entityBehaviour.Entity = entity;

                entity.WorldContext = GameManager.Instance.PlayerEnterExit.WorldContext;

                var enemyIndex = (int) petType;
                if (enemyIndex >= 0 && enemyIndex <= 42)
                {
                    entityBehaviour.EntityType = EntityTypes.EnemyMonster;
                    entity.SetCareer(mobileEnemy, entityBehaviour.EntityType);
                }
                else if (enemyIndex >= 128 && enemyIndex <= 146)
                {
                    entityBehaviour.EntityType = EntityTypes.EnemyClass;
                    entity.SetCareer(mobileEnemy, entityBehaviour.EntityType);
                }
                else if (DaggerfallEntity.GetCustomCareerTemplate(enemyIndex) != null)
                {
                    entityBehaviour.EntityType = DaggerfallEntity.IsClassEnemyId(enemyIndex)
                        ? EntityTypes.EnemyClass
                        : EntityTypes.EnemyMonster;

                    entity.SetCareer(mobileEnemy, entityBehaviour.EntityType);
                }
                else
                {
                    entityBehaviour.EntityType = EntityTypes.None;
                }
            }

            if (dfMobile.Enemy.ID == (int) MobileTypes.DaedraSeducer)
            {
                dfMobile.gameObject.AddComponent<DaedraSeducerMobileBehaviour>();
            }
        }

        private void AdjustControllerHeight(CharacterController controller, float newHeight,
            ControllerJustification justification)
        {
            var newCenter = controller.center;
            switch (justification)
            {
                case ControllerJustification.Top:
                    newCenter.y -= (newHeight - controller.height) / 2;
                    break;

                case ControllerJustification.Bottom:
                    newCenter.y += (newHeight - controller.height) / 2;
                    break;

                case ControllerJustification.Center:
                    // do nothing, centered is normal CharacterController behavior
                    break;
            }

            controller.height = newHeight;
            controller.center = newCenter;
        }

        private enum ControllerJustification
        {
            Top = 0,
            Center = 1,
            Bottom = 2
        }
    }
}