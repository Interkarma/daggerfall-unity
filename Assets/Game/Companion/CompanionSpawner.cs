using System;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game.Utility;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DaggerfallWorkshop.Game.Companion
{
    public class CompanionSpawner : MonoBehaviour
    {
        [SerializeField] private SetupDemoEnemy setup;
        [SerializeField] private float spawnRange = 2;

        private SetupDemoEnemy companion;

        private void Awake()
        {
            SaveLoadManager.OnLoad += SaveLoadManagerOnOnLoad;
            StartGameBehaviour.OnStartGame += OnStartGame;
        }

        public void Spawn()
        {
            if (companion)
                return;

            Vector3 pos = transform.position;
            Vector3 offset =
                Random.insideUnitCircle * spawnRange;
            offset.z = offset.y;
            offset.y = 0;
            pos += offset;
            companion = Instantiate(setup, pos, Quaternion.identity);
            companion.ApplyEnemySettings(companion.EnemyGender);
        }

        private void OnStartGame(object sender, EventArgs e)
        {
            Spawn();
        }

        private void SaveLoadManagerOnOnLoad(SaveData_v1 savedata)
        {
            // savedata.playerData.playerPosition.position;
            Spawn();
        }

        private void OnDestroy()
        {
            SaveLoadManager.OnLoad -= SaveLoadManagerOnOnLoad;
            StartGameBehaviour.OnStartGame += OnStartGame;
        }
    }
}