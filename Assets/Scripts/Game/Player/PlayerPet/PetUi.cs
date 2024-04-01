using UnityEngine;
using UnityEngine.UI;

namespace Game.Player.PlayerPet
{
    public class PetUi : MonoBehaviour
    {
        [SerializeField] private Image fillImage;

        public void SetHp(float to)
        {
            fillImage.fillAmount = to;
        }
    }
}