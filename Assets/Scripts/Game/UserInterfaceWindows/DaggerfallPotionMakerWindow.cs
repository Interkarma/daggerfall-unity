// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut
// Contributors:
//
// Notes:
//

using UnityEngine;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Items;
using System.Collections.Generic;
using DaggerfallWorkshop.Game.MagicAndEffects;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    public class DaggerfallPotionMakerWindow : DaggerfallPopupWindow
    {
        #region UI Rects

        Rect recipesButtonRect = new Rect(169, 26, 36, 16);
        Rect mixButtonRect = new Rect(169, 42, 36, 16);
        Rect exitButtonRect = new Rect(290, 178, 24, 16);

        Rect ingredientsListScrollerRect = new Rect(5, 30, 151, 142);
        Rect ingredientsListRect = new Rect(11, 0, 140, 142);

        static readonly Rect[] ingredientButtonRects = new Rect[]
        {
            new Rect(0, 0, 28, 28),     new Rect(56, 0, 28, 28),    new Rect(112, 0, 28, 28),
            new Rect(0, 38, 28, 28),    new Rect(56, 38, 28, 28),   new Rect(112, 38, 28, 28),
            new Rect(0, 76, 28, 28),    new Rect(56, 76, 28, 28),   new Rect(112, 76, 28, 28),
            new Rect(0, 114, 28, 28),   new Rect(56, 114, 28, 28),  new Rect(112, 114, 28, 28)
        };

        Rect cauldronListScrollerRect = new Rect(221, 30, 84, 142);
        Rect cauldronListRect = new Rect(0, 0, 84, 142);

        static readonly Rect[] cauldronButtonRects = new Rect[]
        {
            new Rect(0, 0, 28, 28),     new Rect(56, 0, 28, 28),
            new Rect(0, 38, 28, 28),    new Rect(56, 38, 28, 28),
            new Rect(0, 76, 28, 28),    new Rect(56, 76, 28, 28),
            new Rect(0, 114, 28, 28),   new Rect(56, 114, 28, 28),
        };


        #endregion

        #region UI Controls

        TextLabel nameLabel = new TextLabel();
        //TextLabel costLabel = new TextLabel();
        TextLabel goldLabel = new TextLabel();

        Button recipesButton;
        Button mixButton;
        Button exitButton;

        ItemListScroller ingredientsListScroller;
        ItemListScroller cauldronListScroller;

        DaggerfallListPickerWindow recipePicker;

        #endregion

        #region UI Textures

        Texture2D baseTexture;
        const string baseTextureName = "MASK00I0.IMG";
        const int alternateAlphaIndex = 12;
        const string textDatabase = "DaggerfallUI";

        #endregion

        List<DaggerfallUnityItem> ingredients = new List<DaggerfallUnityItem>();
        List<DaggerfallUnityItem> cauldron = new List<DaggerfallUnityItem>();
        List<PotionRecipe> recipes = new List<PotionRecipe>();

        #region Constructors

        public DaggerfallPotionMakerWindow(IUserInterfaceManager uiManager, DaggerfallBaseWindow previous = null)
            : base(uiManager, previous)
        {
        }

        #endregion

        #region Setup Methods

        protected override void Setup()
        {
            // Load textures
            LoadTextures();

            // Always dim background
            ParentPanel.BackgroundColor = ScreenDimColor;

            // Setup native panel background
            NativePanel.BackgroundColor = new Color(0, 0, 0, 0.60f);
            NativePanel.BackgroundTexture = baseTexture;

            // Setup UI
            SetupLabels();
            SetupButtons();
            SetupItemListScrollers();

            recipePicker = new DaggerfallListPickerWindow(uiManager, this, DaggerfallUI.SmallFont, 12);
            recipePicker.OnItemPicked += RecipePicker_OnItemPicked;

            Refresh();
        }

        public override void OnPush()
        {
            if (!IsSetup)
                return;

            Refresh();
        }

        public override void OnPop()
        {
            ClearCauldron();
        }

        void Refresh()
        {
            // Update labels
            goldLabel.Text = GameManager.Instance.PlayerEntity.GetGoldAmount().ToString();

            // Add ingredient items to list and gather recipes - from inventory and wagon
            ingredients.Clear();
            List<DaggerfallUnityItem> recipeItems = new List<DaggerfallUnityItem>();
            foreach (ItemCollection playerItems in new ItemCollection[] { GameManager.Instance.PlayerEntity.Items, GameManager.Instance.PlayerEntity.WagonItems })
            {
                for (int i = 0; i < playerItems.Count; i++)
                {
                    DaggerfallUnityItem item = playerItems.GetItem(i);
                    if (item.IsIngredient)
                        ingredients.Add(item);
                    else if (item.IsPotionRecipe)
                        recipeItems.Add(item);
                }
            }
            ingredientsListScroller.Items = ingredients;

            // Clear cauldron and assign to scroller
            cauldron.Clear();
            cauldronListScroller.Items = cauldron;

            // Populate picker from recipe items
            recipes.Clear();
            recipePicker.ListBox.ClearItems();
            foreach (DaggerfallUnityItem recipeItem in recipeItems)
            {
                PotionRecipe potionRecipe = GameManager.Instance.EntityEffectBroker.GetPotionRecipe(recipeItem.PotionRecipeKey);
                if (!recipes.Contains(potionRecipe))
                    recipes.Add(potionRecipe);
            }
            recipes.Sort((x, y) => (x.DisplayName.CompareTo(y.DisplayName)));
            foreach (PotionRecipe potionRecipe in recipes)
                recipePicker.ListBox.AddItem(potionRecipe.DisplayName);
        }

        #endregion

        #region Private Methods

        void LoadTextures()
        {
            baseTexture = ImageReader.GetTexture(baseTextureName, 0, 0, true, alternateAlphaIndex);
        }

        void SetupButtons()
        {
            // Recipes button
            recipesButton = DaggerfallUI.AddButton(recipesButtonRect, NativePanel);
            recipesButton.OnMouseClick += RecipesButton_OnMouseClick;

            // Mixing button
            mixButton = DaggerfallUI.AddButton(mixButtonRect, NativePanel);
            mixButton.OnMouseClick += MixButton_OnMouseClick;

            // Exit button
            exitButton = DaggerfallUI.AddButton(exitButtonRect, NativePanel);
            exitButton.OnMouseClick += ExitButton_OnMouseClick;
        }

        void SetupLabels()
        {
            nameLabel = DaggerfallUI.AddDefaultShadowedTextLabel(new Vector2(33, 185), NativePanel);
            //costLabel = DaggerfallUI.AddDefaultShadowedTextLabel(new Vector2(174, 185), NativePanel);
            goldLabel = DaggerfallUI.AddDefaultShadowedTextLabel(new Vector2(237, 185), NativePanel);
        }

        void SetupItemListScrollers()
        {
            // Create misc text label template
            TextLabel miscLabelTemplate = new TextLabel(DaggerfallUI.Instance.Font3)
            {
                Position = new Vector2(0, ingredientButtonRects[0].height - 2),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.None
            };

            // Setup item list scroller for ingredients with offset of 5 on column 1 for misc labels
            ingredientsListScroller = new ItemListScroller(4, 3, ingredientsListRect, ingredientButtonRects, miscLabelTemplate, defaultToolTip, 2, 0.8f, true, 5, 1)
            {
                Position = new Vector2(ingredientsListScrollerRect.x, ingredientsListScrollerRect.y),
                Size = new Vector2(ingredientsListScrollerRect.width, ingredientsListScrollerRect.height),
                LabelTextHandler = ItemLabelTextHandler
            };
            NativePanel.Components.Add(ingredientsListScroller);
            ingredientsListScroller.OnItemClick += IngredientsListScroller_OnItemClick;

            // Setup item list for cauldron, no scrolling
            cauldronListScroller = new ItemListScroller(4, 2, cauldronListRect, cauldronButtonRects, miscLabelTemplate, defaultToolTip, 2, 1, false)
            {
                Position = new Vector2(cauldronListScrollerRect.x, cauldronListScrollerRect.y),
                Size = new Vector2(cauldronListScrollerRect.width, cauldronListScrollerRect.height),
                LabelTextHandler = ItemLabelTextHandler
            };
            NativePanel.Components.Add(cauldronListScroller);
            cauldronListScroller.OnItemClick += CauldronListScroller_OnItemClick;
        }

        string ItemLabelTextHandler(DaggerfallUnityItem item)
        {
            return item.ItemName.ToUpper();
        }

        void AddToCauldron(DaggerfallUnityItem item)
        {
            if (cauldron.Count < 8)
            {
                nameLabel.Text = "";
                if (item.stackCount == 1)
                {
                    cauldron.Add(item);
                    ingredients.Remove(item);
                }
                else
                {
                    item.stackCount--;
                    DaggerfallUnityItem newItem = item.Clone();
                    newItem.stackCount = 1;
                    cauldron.Add(newItem);
                }
                ingredientsListScroller.Items = ingredients;
                cauldronListScroller.Items = cauldron;
            }
        }

        void RemoveFromCauldron(DaggerfallUnityItem item)
        {
            nameLabel.Text = "";
            cauldron.Remove(item);
            bool stacked = false;
            foreach (DaggerfallUnityItem checkItem in ingredients)
            {
                if (checkItem.ItemGroup == item.ItemGroup && checkItem.GroupIndex == item.GroupIndex)
                {
                    checkItem.stackCount++;
                    stacked = true;
                    break;
                }
            }
            if (!stacked)
                ingredients.Add(item);

            ingredientsListScroller.Items = ingredients;
            cauldronListScroller.Items = cauldron;
        }

        void ClearCauldron()
        {
            // Remove all ingredients from cauldron to restore correct stack sizes
            while (cauldron.Count > 0)
                RemoveFromCauldron(cauldron[0]);
        }

        void AddRecipeToCauldron(int index, string recipeName)
        {
            ItemCollection playerItems = GameManager.Instance.PlayerEntity.Items;
            PotionRecipe recipe = recipes[index];
            Dictionary<int, DaggerfallUnityItem> recipeIngreds = new Dictionary<int, DaggerfallUnityItem>();
            foreach (PotionRecipe.Ingredient ingred in recipe.Ingredients)
                recipeIngreds.Add(ingred.id, null);

            // Find matching items for the recipe ingredients
            for (int i = 0; i < playerItems.Count; i++)
            {
                DaggerfallUnityItem item = playerItems.GetItem(i);
                if (item.IsIngredient && recipeIngreds.ContainsKey(item.TemplateIndex) && recipeIngreds[item.TemplateIndex] == null)
                    recipeIngreds[item.TemplateIndex] = item;
            }
            // If player doesn't have all the required ingredients, display message else move ingredients into cauldron.
            if (recipeIngreds.ContainsValue(null))
            {
                DaggerfallUI.MessageBox(TextManager.Instance.GetText(textDatabase, "reqIngredients"));
            }
            else
            {
                ClearCauldron();
                foreach (DaggerfallUnityItem item in recipeIngreds.Values)
                    AddToCauldron(item);
                nameLabel.Text = recipeName;
            }
        }

        void MixCauldron()
        {
            // Check recipes and create appropriate potion in player inventory if a match found
            List<int> cauldronIngredients = new List<int>(cauldron.Count);
            foreach (DaggerfallUnityItem item in cauldron)
                cauldronIngredients.Add(item.TemplateIndex);
            int recipeKey = new PotionRecipe(cauldronIngredients).GetHashCode();

            //IEntityEffect potionEffect = GameManager.Instance.EntityEffectBroker.GetPotionRecipeEffect(recipe);
            PotionRecipe potionRecipe = GameManager.Instance.EntityEffectBroker.GetPotionRecipe(recipeKey);
            if (potionRecipe != null)
            {
                Debug.LogFormat("Potion matched: {0}", potionRecipe.DisplayName);
                GameManager.Instance.PlayerEntity.Items.AddItem(ItemBuilder.CreatePotion(recipeKey));
                DaggerfallUI.MessageBox(TextManager.Instance.GetText(textDatabase, "potionMixed"));
                DaggerfallUI.Instance.PlayOneShot(SoundClips.MakePotion);
            }
            else
            {
                // Changed from classic, don't create useless 'Unknown Powers' potions.
                //GameManager.Instance.PlayerEntity.Items.AddItem(ItemBuilder.CreatePotion(0));
                DaggerfallUI.MessageBox(TextManager.Instance.GetText(textDatabase, "potionFailed"));
            }

            // Remove item from player inventory unless a stack remains.
            foreach (DaggerfallUnityItem item in cauldron)
            {
                bool stacked = false;
                foreach (DaggerfallUnityItem checkItem in ingredients)
                {
                    if (checkItem.ItemGroup == item.ItemGroup && checkItem.GroupIndex == item.GroupIndex)
                    {
                        stacked = true;
                        break;
                    }
                }
                if (!stacked)
                    GameManager.Instance.PlayerEntity.Items.RemoveItem(item);
            }

            // Empty cauldron and update list displays
            cauldron.Clear();
            ingredientsListScroller.Items = ingredients;
            cauldronListScroller.Items = cauldron;
        }

        #endregion

        #region Event Handlers

        protected virtual void IngredientsListScroller_OnItemClick(DaggerfallUnityItem item)
        {
            AddToCauldron(item);
        }

        protected virtual void CauldronListScroller_OnItemClick(DaggerfallUnityItem item)
        {
            RemoveFromCauldron(item);
        }

        private void RecipesButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (recipes.Count > 0)
                uiManager.PushWindow(recipePicker);
            else
                DaggerfallUI.MessageBox(TextManager.Instance.GetText(textDatabase, "noRecipes"));
        }

        public void RecipePicker_OnItemPicked(int index, string recipeName)
        {
            Debug.LogFormat("Picked recipe {0} at idx {1}.", recipeName, index);
            recipePicker.CloseWindow();
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            if (index < recipes.Count)
                AddRecipeToCauldron(index, recipeName);
        }

        private void MixButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            if (cauldron.Count > 0)
                MixCauldron();
        }

        private void ExitButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            CloseWindow();
        }

        #endregion
    }
}