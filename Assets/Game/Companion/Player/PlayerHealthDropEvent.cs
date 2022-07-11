using DaggerfallWorkshop.Game.Companion.Entity;
using UnityEngine;
using UnityEngine.Events;

namespace DaggerfallWorkshop.Game.Companion.Player
{
    public class PlayerHealthDropEvent : MonoBehaviour
    {
        [Range(0.01f, 0.99f)]
        [SerializeField] private float threshold = 0.25f;

        [SerializeField] private UnityEvent onHealthDropped;

        private EntityHealthDropHandler _healthDropHandler;

        private void Start()
        {
            var player = GameManager.Instance.PlayerEntityBehaviour;
            _healthDropHandler = new EntityHealthDropHandler(player.Entity, threshold);
            _healthDropHandler.OnReachedThreshold += onHealthDropped.Invoke;
        }

        private void OnEnable()
        {
            _healthDropHandler?.Subscribe();
        }

        private void OnDisable()
        {
            _healthDropHandler?.Unsubscribe();
        }
    }
}