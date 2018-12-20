// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using System;
using System.Collections.Generic;
using DaggerfallConnect.FallExe;

namespace DaggerfallWorkshop.Game.MagicAndEffects
{
    /// <summary>
    /// Implements a potion recipe to support equality comparison, hashing, and string output.
    /// Internally the recipe is just an array of item template IDs abstracted to an Ingredient type.
    /// A recipe can be constructed from any Ingredient[] array or int[] array of item template IDs.
    /// </summary>
    public class PotionRecipe : IEqualityComparer<PotionRecipe.Ingredient[]>
    {
        // Classic potion recipe mapping to DFU recipe keys:
        //  Stamina, Orc Strength, Healing, Waterwalking, Restore Power, Resist Fire, Resist Frost, Resist Shock, Cure Disease, Slow Falling,
        //  Water Breathing, Heal True, Levitation, Resist Poison, Free Action, Cure Poison, Chameleon Form, Shadow Form, Invisibility, Purification
        public static readonly int[] classicRecipeKeys = { 221871, 239524, 4975678, 5017404, 5188896, 111516185, 4826108, 216843, 224588, 220192,
                                                           240081, 4937012, 228890, 221117, 4870452, 5361377, 112080144, 4842851, 4815872, 2031019196 };

        public static readonly string UnknownPowers = "Unknown Powers";

        #region Fields

        Ingredient[] ingredients = null;
        int textureRecord = 11;

        #endregion

        #region Properties

        /// <summary>
        /// The display name of this potion recipe.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// The price of this potion recipe.
        /// </summary>
        public int Price { get; set; }

        /// <summary>
        /// The texture record from archive 205 to use for this potion, default = 11.
        /// </summary>
        public int TextureRecord
        {
            get { return textureRecord; }
            set { textureRecord = value; }
        }

        /// <summary>
        /// Gets or sets effect settings for this recipe.
        /// </summary>
        public EffectSettings Settings { get; set; }

        /// <summary>
        /// Gets or sets potion recipe ingredients. Ingredients must be sorted by id.
        /// </summary>
        public Ingredient[] Ingredients
        {
            get { return ingredients; }
            set { ingredients = value; }
        }

        /// <summary>
        /// Retrieves a list of secondary effect keys.
        /// </summary>
        public List<string> SecondaryEffects { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public PotionRecipe()
        {
            Settings = BaseEntityEffect.DefaultEffectSettings();
        }

        /// <summary>
        /// Ingredient[] array constructor.
        /// </summary>
        /// <param name="displayName">Potion name to use for this recipe.</param>
        /// <param name="price">Value of potion in gp.</param>
        /// <param name="settings">Settings for this potion recipe.</param>
        /// <param name="ingredients">Ingredient array.</param>
        public PotionRecipe(string displayName, int price, EffectSettings settings, params Ingredient[] ingredients)
        {
            DisplayName = displayName;
            Price = price;
            this.Settings = settings;
            Array.Sort(ingredients);
            this.ingredients = ingredients;
        }

        /// <summary>
        /// Ingredient[] array constructor - for finding and comparisons.
        /// </summary>
        /// <param name="ids">Ingredient ids list.</param>
        public PotionRecipe(List<int> ids)
        {
            ids.Sort();
            if (ids != null && ids.Count > 0)
            {
                ingredients = new Ingredient[ids.Count];
                for (int i = 0; i < ids.Count; i++)
                    ingredients[i].id = ids[i];
            }
        }

        /// <summary>
        /// int[] array of item template IDs constructor.
        /// </summary>
        /// <param name="displayName">Potion name to use for this recipe.</param>
        /// <param name="price">Value of potion in gp.</param>
        /// <param name="settings">Settings for this potion recipe.</param>
        /// <param name="ids">Array of item template IDs.</param>
        public PotionRecipe(string displayName, int price, EffectSettings settings, params int[] ids)
        {
            DisplayName = displayName;
            Price = price;
            this.Settings = settings;
            Array.Sort(ids);
            if (ids != null && ids.Length > 0)
            {
                ingredients = new Ingredient[ids.Length];
                for (int i = 0; i < ids.Length; i++)
                {
                    ingredients[i].id = ids[i];
                }
            }
        }

        #endregion

        #region Structures

        /// <summary>
        /// Abstracts an item ID into an ingredient.
        /// </summary>
        public struct Ingredient
        {
            public int id;

            public Ingredient(int id)
            {
                this.id = id;
            }
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Compare another PotionRecipe class with this one.
        /// </summary>
        /// <param name="other">Other potion recipe class.</param>
        /// <returns>True if both recipes are equal.</returns>
        public override bool Equals(object other)
        {
            if (other == null || !(other is PotionRecipe))
                return false;

            return Equals((other as PotionRecipe).Ingredients);
        }

        /// <summary>
        /// Gets hash code for this recipe.
        /// </summary>
        /// <returns>Hash code.</returns>
        public override int GetHashCode()
        {
            return GetHashCode(ingredients);
        }

        /// <summary>
        /// Gets string listing all ingredients.
        /// </summary>
        /// <returns>Ingredient list.</returns>
        public override string ToString()
        {
            if (ingredients == null || ingredients.Length == 0)
                return string.Empty;

            string result = string.Empty;
            for (int i = 0; i < ingredients.Length; i++)
            {
                ItemTemplate template = DaggerfallUnity.Instance.ItemHelper.GetItemTemplate(ingredients[i].id);
                result += template.name;
                if (i < ingredients.Length - 1)
                    result += ", ";
                else
                    result += ".";
            }

            return result;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Checks if recipe is defined.
        /// </summary>
        /// <returns>True if recipe has at least one ingredient.</returns>
        public bool HasRecipe()
        {
            return (ingredients != null && ingredients.Length > 0);
        }

        /// <summary>
        /// Adds secondary effects to this potion recipe.
        /// </summary>
        /// <param name="effectKey">The EffectKey of the effect to add.</param>
        public void AddSecondaryEffect(string effectKey)
        {
            if (SecondaryEffects == null)
                SecondaryEffects = new List<string>();
            SecondaryEffects.Add(effectKey);
        }

        /// <summary>
        /// Compare a recipe with this one.
        /// </summary>
        /// <param name="ingredients">Other recipe.</param>
        /// <returns>True if other recipe equal with this one.</returns>
        public bool Equals(Ingredient[] ingredients)
        {
            return Equals(this.ingredients, ingredients);
        }

        /// <summary>
        /// Compare two recipes for equality.
        /// </summary>
        /// <param name="ingredients1">First recipe.</param>
        /// <param name="ingredients2">Second recipe.</param>
        /// <returns>True if recipes are equal.</returns>
        public bool Equals(Ingredient[] ingredients1, Ingredient[] ingredients2)
        {
            if (ingredients1 == null || ingredients2 == null || ingredients1.Length != ingredients2.Length)
                return false;

            for (int i = 0; i < ingredients1.Length; i++)
            {
                if (ingredients1[i].id != ingredients2[i].id)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Gets a hash code for recipe from ingredients.
        /// Note: Using hash code calculation from:
        /// https://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode
        /// </summary>
        /// <param name="ingredients">Ingredients.</param>
        /// <returns>Hash code.</returns>
        public int GetHashCode(Ingredient[] ingredients)
        {
            if (ingredients == null || ingredients.Length == 0)
                return 0;

            int hash = 17;
            for (int i = 0; i < ingredients.Length; i++)
            {
                hash = hash * 23 + ingredients[i].id;
            }

            return hash;
        }

        #endregion
    }
}
