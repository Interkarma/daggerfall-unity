using System;
using DaggerfallWorkshop.Game.Entity;
using JetBrains.Annotations;

namespace DaggerfallWorkshop.Game.Companion.Entity
{
    public class EntityHealthDropHandler
    {
        private readonly DaggerfallEntity _target;
        private readonly float _threshold;

        private bool subscribed;

        public event Action OnReachedThreshold;

        /// <summary>
        ///
        /// </summary>
        /// <param name="entity">Target</param>
        /// <param name="threshold">(0,1)</param>
        public EntityHealthDropHandler([NotNull] DaggerfallEntity entity, float threshold)
        {
            _target = entity;
            _threshold = threshold;
            Subscribe();
        }

        public void Subscribe()
        {
            if (!subscribed)
                _target.OnHealthChanged += TargetOnOnHealthChanged;
        }

        public void Unsubscribe()
        {
            if (subscribed)
                _target.OnHealthChanged += TargetOnOnHealthChanged;
        }

        ~EntityHealthDropHandler()
        {
            _target.OnHealthChanged -= TargetOnOnHealthChanged;
        }

        private void TargetOnOnHealthChanged(DaggerfallEntity entity, int change)
        {
            if (change > 0)
                return;

            float maxHealth = entity.MaxHealth;
            float currHealthRatio = entity.CurrentHealth / maxHealth;
            float prevHealthRatio = currHealthRatio - change / maxHealth;
            if (prevHealthRatio < _threshold)
                return;

            if (currHealthRatio < _threshold)
            {
                OnReachedThreshold?.Invoke();
            }
        }
    }
}