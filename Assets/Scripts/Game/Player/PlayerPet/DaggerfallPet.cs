using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;
using UnityEngine;

namespace Game.Player.PlayerPet
{
    public class DaggerfallPet : MonoBehaviour
    {
        private const string HpCanvasPath = "Pet/HpCanvas";
        private const string HpPotionPath = "Pet/HpPotion";
        private const float PotionSpawnInterval = 10f;

        private SetupDemoEnemy setupDemoEnemy;
        private EnemyAttack enemyAttack;
        private PetUi petUi;
        private DaggerfallEntityBehaviour entityBehaviour;
        private EnemyEntity entity;
        private GameObject player;
        private DaggerfallEntityBehaviour playerBehaviour;
        private float lastPotionSpawnTime;

        private void Awake()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            gameObject.AddComponent<PetFollow>().Init(player.transform);
            setupDemoEnemy = GetComponent<SetupDemoEnemy>();
            enemyAttack = GetComponent<EnemyAttack>();
            entityBehaviour = GetComponent<DaggerfallEntityBehaviour>();
            entity = entityBehaviour.Entity as EnemyEntity;
            playerBehaviour = player.GetComponent<DaggerfallEntityBehaviour>();

            ApplySettings();
        }

        private void Start()
        {
            var hpCanvasPrefab = Resources.Load<GameObject>(HpCanvasPath);
            petUi = Instantiate(hpCanvasPrefab, transform).GetComponent<PetUi>();
        }

        private void Update()
        {
            if (petUi != null)
            {
                petUi.SetHp((float) entity.CurrentHealth / entity.MaxHealth);
            }

            if (Time.time - lastPotionSpawnTime >= PotionSpawnInterval
                && playerBehaviour.Entity.CurrentHealth < playerBehaviour.Entity.MaxHealth * 0.25)
            {
                SpawnPotion();
                lastPotionSpawnTime = Time.time;
            }
        }

        private void SpawnPotion()
        {
            Instantiate(Resources.Load<GameObject>(HpPotionPath), transform.position, Quaternion.identity);
        }

        private void ApplySettings()
        {
            setupDemoEnemy.AlliedToPlayer = true;
            enemyAttack.enabled = false;
        }
    }
}