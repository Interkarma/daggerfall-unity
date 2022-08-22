using System.Collections.Generic;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Utility;
using UnityEngine;

namespace DaggerfallWorkshop.Game
{
    public class SetupPetEntity : MonoBehaviour
    {
        public MobileTypes PetType = MobileTypes.SkeletalWarrior;
        public MobileReactions PetReaction = MobileReactions.Hostile;
        public MobileGender PetGender = MobileGender.Unspecified;
        public bool AlliedToPlayer = false;
        public byte ClassicSpawnDistanceType = 0;
        private PetBehaviour petBehaviour;
        DaggerfallEntityBehaviour entityBehaviour;

        public GameObject LightAura;

        void Awake()
        {
            Debug.LogWarning("on Pet setup");
            // Must have an entity behaviour
            petBehaviour = GetComponent<PetBehaviour>();
            entityBehaviour = GetComponent<DaggerfallEntityBehaviour>();
            if (!entityBehaviour)
                gameObject.AddComponent<DaggerfallEntityBehaviour>();

            // Disable this game object if missing mobile setup
            PetMobileUnit dfMobile = GetMobileBillboardChild();
            if (dfMobile == null)
                this.gameObject.SetActive(false);


            ApplyPetSettings(EntityTypes.Pet, 0, MobileGender.Unspecified);
        }


        enum ControllerJustification
        {
            TOP,
            CENTER,
            BOTTOM
        }

        static void AdjustControllerHeight(CharacterController controller, float newHeight,
            ControllerJustification justification)
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
        /// Sets up pet based on current settings.
        /// </summary>
        public void ApplyPetSettings(MobileGender gender)
        {
            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;
            Dictionary<int, MobileEnemy> petDict = GameObjectHelper.EnemyDict;

            MobilePet mobilePet = GameObjectHelper.GetPetStats();

            if (AlliedToPlayer)
                mobilePet.Team = MobileTeams.PlayerAlly;
            
            // Find mobile unit in children
            PetMobileUnit dfMobile = GetMobileBillboardChild();
            if (dfMobile != null)
            {
                // Setup mobile billboard
                Vector2 size = Vector2.one;
                mobilePet.Gender = gender;
                mobilePet.Reactions = PetReaction;
                dfMobile.SetPet(dfUnity, mobilePet, PetReaction, ClassicSpawnDistanceType); 
                // Setup controller
                CharacterController controller = GetComponent<CharacterController>();
                if (controller)
                {
                    // Set base height from sprite
                    size = dfMobile.GetSize();
                    controller.height = size.y;

                    // Limit minimum controller height
                    // Stops very short characters like rats from being walked upon
                    if (controller.height < 1.6f)
                        AdjustControllerHeight(controller, 1.6f, ControllerJustification.BOTTOM);

                    controller.gameObject.layer = LayerMask.NameToLayer("Player");
                }

                // Setup sounds
                PetSound petSounds = GetComponent<Game.PetSound>();
                if (petSounds)
                {
                    petSounds.MoveSound = (SoundClips)dfMobile.Pet.MoveSound;
                    petSounds.BarkSound = (SoundClips)dfMobile.Pet.BarkSound;
                }

                MeshRenderer meshRenderer = dfMobile.GetComponent<MeshRenderer>();
                if (meshRenderer)
                {

                    if (dfMobile.Pet.NoShadow)
                    {
                        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    }

                    if (dfMobile.Pet.GlowColor != null)
                    {
                        meshRenderer.receiveShadows = false;
                        GameObject petLightGameObject = Instantiate(LightAura);
                        petLightGameObject.transform.parent = dfMobile.transform;
                        petLightGameObject.transform.localPosition = new Vector3(0, 0.3f, 0.2f);
                        Light petLight = petLightGameObject.GetComponent<Light>();
                        petLight.color = (Color)dfMobile.Pet.GlowColor;
                        petLight.shadows = DaggerfallUnity.Settings.DungeonLightShadows
                            ? LightShadows.Soft
                            : LightShadows.None;
                    }
                }

                if (entityBehaviour)
                {
                    PetEntity entity = new PetEntity(entityBehaviour);
                    entityBehaviour.Entity = entity;

                    entityBehaviour.EntityType = EntityTypes.Pet;

                    entity.WorldContext = GameManager.Instance.PlayerEnterExit.WorldContext;

                    entity.SetPetCareer(mobilePet, entityBehaviour.EntityType);
                }
                petBehaviour.SetHealth(mobilePet.MaxHealth);
            }
        }

        /// <summary>
        /// Change pet settings and configure in a single call.
        /// </summary>
        /// <param name="PetType">pet type.</param>
        public void ApplyPetSettings(MobileTypes PetType, MobileReactions petReaction, MobileGender gender,
            byte classicSpawnDistanceType = 0, bool alliedToPlayer = false)
        {
            this.PetType = PetType;
            PetReaction = petReaction;
            ClassicSpawnDistanceType = classicSpawnDistanceType;
            AlliedToPlayer = alliedToPlayer;
            ApplyPetSettings(gender);
        }

        /// <summary>
        /// Change pet settings and configure in a single call.
        /// </summary>
        public void ApplyPetSettings(EntityTypes entityType, int careerIndex, MobileGender gender,
            bool isHostile = true, bool alliedToPlayer = false)
        {
            // Get mobile type based on entity type and career index
            MobileTypes mobileType=(MobileTypes)careerIndex;

            MobileReactions petReaction = (isHostile) ? MobileReactions.Hostile : MobileReactions.Passive;
            MobileGender petGender = gender;

            ApplyPetSettings(mobileType, petReaction, petGender, alliedToPlayer: alliedToPlayer);
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
        public PetMobileUnit GetMobileBillboardChild()
        {
#if UNITY_EDITOR
            return GetComponentInChildren<PetMobileUnit>();
#endif
        }
    }
}