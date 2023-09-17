using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Serialization;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Pet
{
    public class PetSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject petPrefab;
        [SerializeField] private MobileTypes petType;

        private GameObject _petObject;
        private PetFactory _petFactory;

        private void Start()
        {
            _petFactory = new PetFactory(petPrefab);

            SaveLoadManager.OnLoad += SaveLoadManagerOnOnLoad;
        }

        private void OnDestroy()
        {
            SaveLoadManager.OnLoad -= SaveLoadManagerOnOnLoad;
        }

        private void SaveLoadManagerOnOnLoad(SaveData_v1 saveData)
        {
            if (_petObject != null) return;

            SpawnPet();
        }

        [Button]
        private void SpawnPet()
        {
            _petObject = _petFactory.CreateEnemy(petType, GameManager.Instance.PlayerObject.transform.position);
        }
    }
}