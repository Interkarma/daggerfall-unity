using DaggerfallWorkshop.Game.Entity;
using UnityEngine;
using UnityEngine.UI;

namespace DaggerfallWorkshop.Game.Companion.Entity
{
    [DefaultExecutionOrder(1000)]
    public class EntityHealthBar : MonoBehaviour
    {
        [SerializeField] private DaggerfallEntityBehaviour daggerfallEntity;
        [SerializeField] private Image bar;

        private void Start()
        {
            daggerfallEntity.Entity.OnHealthChanged += OnHealthChanged;
            bar.fillAmount = daggerfallEntity.Entity.CurrentHealthPercent;
        }

        private void OnHealthChanged(DaggerfallEntity entity, int change)
        {
            bar.fillAmount = entity.CurrentHealthPercent;
        }
    }
}