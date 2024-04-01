using UnityEngine;

namespace Game.Player.PlayerPet
{
    public class PetFollow : MonoBehaviour
    {
#pragma warning disable 649
        [SerializeField] private Transform target;
        [SerializeField] private float teleportDistance = 10f;
#pragma warning restore 649

        public void Init(Transform playerTransform)
        {
            target = playerTransform;
        }

        void Update()
        {
            float distance = Vector3.Distance(transform.position, target.position);

            if (distance > teleportDistance)
            {
                transform.position = target.position;
            }
        }

    }
}