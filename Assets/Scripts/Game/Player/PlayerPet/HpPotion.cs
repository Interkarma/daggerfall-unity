using UnityEngine;

namespace Game.Player.PlayerPet
{
    public class HpPotion : MonoBehaviour
    {
        [field:SerializeField] public int HealthAmount { get; private set; }
    }
}