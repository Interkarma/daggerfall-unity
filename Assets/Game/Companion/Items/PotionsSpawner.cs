using DaggerfallWorkshop.Game.Items;
using UnityEngine;

namespace DaggerfallWorkshop.Game.Companion.Items
{
    public class PotionsSpawner : MonoBehaviour
    {
        [SerializeField] private PotionSerializedCounted[] potions;
        [SerializeField] private DaggerfallLoot lootPrefab;

        [SerializeField] private int archive;
        [SerializeField] private int record;


        public void Spawn()
        {
            var loot = Instantiate(lootPrefab, transform.position, Quaternion.identity);
            if (loot.TryGetComponent(out DaggerfallBillboard billboard))
                billboard.SetMaterial(archive, record);

            foreach (PotionSerializedCounted potion in potions)
            {
                loot.Items.AddItem(ItemBuilder.CreatePotion(potion.potion.Recipe.GetHashCode(), potion.count));
            }
        }
    }
}