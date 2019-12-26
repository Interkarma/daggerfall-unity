using System.Collections;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using UnityEngine;

public class HealthIndicatorMod : MonoBehaviour
{
    private static Mod _mod;

    private GameObject _npcHealthIndicatorPrefab;

    [Invoke(StateManager.StateTypes.Start, 0)]
    public static void Init(InitParams initParams)
    {
        _mod = initParams.Mod;
        var gameObject = new GameObject(_mod.Title);
        gameObject.AddComponent<HealthIndicatorMod>();
    }

    void Awake()
    {
        _npcHealthIndicatorPrefab = _mod.GetAsset<GameObject>("Assets/NPCHealthIndicator.prefab");
        StartCoroutine(EntityBehavioursUpdateLoop());
        _mod.IsReady = true;
    }

    private IEnumerator EntityBehavioursUpdateLoop()
    {
        while (true)
        {
            var entityBehaviours = FindObjectsOfType<DaggerfallEntityBehaviour>();
            foreach (var entityBehaviour in entityBehaviours)
            {
                if (entityBehaviour.transform.CompareTag("Player"))
                {
                    continue;
                }
                var hasHealthIndicator = entityBehaviour.transform.Find("NPCHealthIndicator(Clone)") != null;
                if (!hasHealthIndicator)
                {
                    var npcHealthIndicator = Instantiate(_npcHealthIndicatorPrefab, entityBehaviour.transform);
                    npcHealthIndicator.AddComponent<NPCHealthIndicator>();
                }
            }
            yield return new WaitForSeconds(1.0f);
        }
    }
}
