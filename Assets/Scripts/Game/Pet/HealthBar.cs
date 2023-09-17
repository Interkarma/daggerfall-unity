using DaggerfallWorkshop;
using DaggerfallWorkshop.Game.Entity;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Pet
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Image progressImage;
        [SerializeField] private DaggerfallEntityBehaviour petBehaviour;
        [SerializeField] private Canvas canvas;
        [SerializeField] private MobileUnit petBillboard;

        private Camera _mainCamera;

        private void Awake()
        {
            _mainCamera = Camera.main;
            canvas.worldCamera = _mainCamera;
        }

        private void Start()
        {
            SetPositionDependsOnHeight();
        }

        private void Update()
        {
            FaceToCamera();
            UpdateHealthBar(petBehaviour.Entity.CurrentHealthPercent);
        }

        private void SetPositionDependsOnHeight()
        {
            canvas.transform.localPosition = new Vector3(0, petBillboard.GetSize().y / 2f, 0);
        }

        private void FaceToCamera()
        {
            transform.LookAt(_mainCamera.transform);
            transform.Rotate(0, 180, 0);
        }

        private void UpdateHealthBar(float healthPercent)
        {
            progressImage.fillAmount = healthPercent;
        }
    }
}