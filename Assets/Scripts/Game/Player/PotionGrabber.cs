using DaggerfallWorkshop.Game.Entity;
using Game.Player.PlayerPet;
using UnityEngine;

namespace Game.Player
{
    public class PotionGrabber : MonoBehaviour
    {
        private DaggerfallEntityBehaviour entityBehaviour;

        void Awake()
        {
            entityBehaviour = GetComponent<DaggerfallEntityBehaviour>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out HpPotion hpPotion))
                return;

            AddHealth(hpPotion.HealthAmount);

            Destroy(other.gameObject);
        }

        void AddHealth(int amount)
        {
            if (!entityBehaviour)
                return;

            if (entityBehaviour.Entity is PlayerEntity entity)
                entity.SetHealth(amount);
        }
    }
}