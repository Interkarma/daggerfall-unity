using System;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Utility;
using UnityEngine;

namespace Game.Pet
{
    public class PetSpawner : MonoBehaviour
    {
        public GameObject PetPrefab;
        public float MaxDistance;

        private GameObject pet;

        [ContextMenu("Spawn Pet")]
        public void SpawnPet()
        {
            pet = CreateEnemy("Pet", MobileTypes.Imp,
                GameManager.Instance.PlayerObject.transform.position,
                MobileGender.Female, null, MobileReactions.Passive);
        }

        private void Update()
        {
            if (GameManager.IsGamePaused)
                return;

            if (pet != null &&
                Vector3.Distance(pet.transform.position, GameManager.Instance.PlayerObject.transform.position) >
                MaxDistance)
            {
                pet.transform.position = GameManager.Instance.PlayerObject.transform.position;
            }
        }

        public GameObject CreateEnemy(string name, MobileTypes mobileType, Vector3 localPosition,
            MobileGender mobileGender = MobileGender.Unspecified, Transform parent = null,
            MobileReactions mobileReaction = MobileReactions.Hostile)
        {
            string displayName = string.Format("{0} [{1}]", name, mobileType.ToString());
            GameObject go = GameObjectHelper.InstantiatePrefab(PetPrefab,
                displayName,
                parent, Vector3.zero);
            SetupDemoEnemy setupEnemy = go.GetComponent<SetupDemoEnemy>();

            // Set position
            go.transform.localPosition = localPosition;

            // Assign humanoid gender randomly if unspecfied
            // This does not affect monsters like rats, bats, etc
            MobileGender gender;
            if (mobileGender == MobileGender.Unspecified)
            {
                if (UnityEngine.Random.Range(0f, 1f) < 0.5f)
                    gender = MobileGender.Male;
                else
                    gender = MobileGender.Female;
            }
            else
            {
                gender = mobileGender;
            }

            // Configure enemy
            setupEnemy.ApplyEnemySettings(mobileType, mobileReaction, gender);

            CharacterController controller = go.GetComponent<CharacterController>();
            if (controller != null)
            {
                // Align non-flying units with ground
                MobileUnit mobileUnit = setupEnemy.GetMobileBillboardChild();
                if (mobileUnit.Enemy.Behaviour != MobileBehaviour.Flying)
                    GameObjectHelper.AlignControllerToGround(controller);
            }

            if (GameManager.Instance != null)
                GameManager.Instance.RaiseOnEnemySpawnEvent(go);

            return go;
        }
    }
}