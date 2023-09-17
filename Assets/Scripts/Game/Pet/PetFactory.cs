using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Utility;
using UnityEngine;

namespace Game.Pet
{
    public class PetFactory
    {
        private readonly GameObject _petPrefab;

        public PetFactory(GameObject petPrefab)
        {
            _petPrefab = petPrefab;
        }

        public GameObject CreateEnemy(MobileTypes mobileType, Vector3 localPosition,
            MobileGender mobileGender = MobileGender.Unspecified, Transform parent = null,
            MobileReactions mobileReaction = MobileReactions.Passive)
        {
            var displayName = $"Pet [{mobileType.ToString()}]";
            var instance = GameObjectHelper.InstantiatePrefab(_petPrefab, displayName, parent, Vector3.zero);
            var setupEnemy = instance.GetComponent<PetConfigurer>();

            instance.transform.localPosition = localPosition;

            MobileGender gender;
            if (mobileGender == MobileGender.Unspecified)
                gender = Random.Range(0f, 1f) < 0.5f ? MobileGender.Male : MobileGender.Female;
            else
                gender = mobileGender;

            setupEnemy.ApplySettings(mobileType, mobileReaction, gender, 1);

            var controller = instance.GetComponent<CharacterController>();
            if (controller != null)
            {
                var mobileUnit = setupEnemy.GetMobileBillboardChild();
                if (mobileUnit.Enemy.Behaviour != MobileBehaviour.Flying)
                    GameObjectHelper.AlignControllerToGround(controller);
            }

            if (GameManager.Instance != null)
                GameManager.Instance.RaiseOnEnemySpawnEvent(instance);

            return instance;
        }
    }
}