using DaggerfallWorkshop.Game;
using UnityEngine;

namespace Game.Pet
{
    public class PetTeleporter : MonoBehaviour
    {
        [SerializeField] private PetSenses petSenses;
        [SerializeField] private float maxDistance;

        private void FixedUpdate()
        {
            Debug.LogWarning("Distance to target: " + petSenses.DistanceToTarget);
            Debug.LogWarning("CanSee: " + petSenses.CanSeeTarget(petSenses.Target));
            if (GameManager.IsGamePaused)
                return;

            if (Vector3.Distance(transform.position, GameManager.Instance.PlayerObject.transform.position) >
                maxDistance || petSenses.DistanceToTarget > maxDistance)
            {
                transform.position = GameManager.Instance.PlayerObject.transform.position;
            }
        }
    }
}