using System.Collections.Generic;
using DaggerfallWorkshop.Game.MagicAndEffects;
using UnityEngine;

namespace DaggerfallWorkshop.Game.Companion.Items
{
    [System.Serializable]
    public class PotionSerialized
    {
        [SerializeField] private List<int> ingredientIds;
        private PotionRecipe _recipe;

        public PotionRecipe Recipe
        {
            get => _recipe ?? (_recipe = new PotionRecipe(ingredientIds));
            private set => _recipe = value;
        }
    }

    [System.Serializable]
    public class PotionSerializedCounted
    {
        public PotionSerialized potion;
        public int count;
    }
}