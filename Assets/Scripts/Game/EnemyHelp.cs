using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Utility;
using UnityEngine;

namespace DaggerfallWorkshop.Game
{
    [RequireComponent(typeof(EnemySenses))]
    [RequireComponent(typeof(EnemyMotor))]
    public class EnemyHelp : MonoBehaviour
    {
        [SerializeField] private float _spawnHealPotionCooldown = 120f;

        private float _curSpawnHealPotionCooldown = 0f;

        public void SpawnHealingPotion()
        {
            Debug.Log("Spawn Heal Potion");

            _curSpawnHealPotionCooldown = _spawnHealPotionCooldown;
            GameObjectHelper.CreateHealingPotion(gameObject, DaggerfallUnity.NextUID);
        }

        public bool CanSpawnHealingPotion() {
            return _curSpawnHealPotionCooldown <= 0;
        }

        private void Update() {
            if (_curSpawnHealPotionCooldown > 0)
                _curSpawnHealPotionCooldown -= Time.deltaTime;
        }

    }
}