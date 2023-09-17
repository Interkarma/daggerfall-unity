using DaggerfallWorkshop.Game.Entity;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Pet
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Image progressImage;
        [SerializeField] DaggerfallEntityBehaviour petBehaviour;

        private Camera _mainCamera;

        private void Awake()
        {
            _mainCamera = Camera.main;
            petBehaviour = GetComponentInParent<DaggerfallEntityBehaviour>();
        }

        private void Update()
        {
            transform.LookAt(_mainCamera.transform);
            transform.Rotate(0, 180, 0);

            UpdateHealthBar(petBehaviour.Entity.CurrentHealthPercent);
        }

        [Button]
        public void UpdateHealthBar(float healthPercent)
        {
            progressImage.fillAmount = healthPercent;
        }
    }
}