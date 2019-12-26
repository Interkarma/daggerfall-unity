using DaggerfallWorkshop.Game.Entity;
using TMPro;
using UnityEngine;

public class NPCHealthIndicator : MonoBehaviour
{
    private DaggerfallEntityBehaviour _entityBehaviour;
    private TextMeshPro _textMeshPro;

    private void Start()
    {
        _textMeshPro = gameObject.GetComponent<TextMeshPro>();
        _entityBehaviour = GetComponentInParent<DaggerfallEntityBehaviour>();
    }

    private void Update()
    {
        UpdateHealthText();
        UpdateRotation();
    }

    private void UpdateHealthText()
    {
        var healthPercent = _entityBehaviour.Entity.CurrentHealthPercent;
        _textMeshPro.SetText(
            healthPercent == 0 || healthPercent == 1
                ? string.Empty
                : string.Format("HP: {0:0}%", healthPercent * 100)
        );
    }

    private void UpdateRotation()
    {
        transform.rotation = Camera.main.transform.rotation;
    }
}
