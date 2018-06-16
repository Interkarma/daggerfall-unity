// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut

using UnityEngine;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.Entity;
using System.Collections.Generic;

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

        static Rect[] ingredientButtonRects = new Rect[]
        {
            new Rect(0, 0, 28, 28),     new Rect(56, 0, 28, 28),    new Rect(112, 0, 28, 28),
            new Rect(0, 38, 28, 28),    new Rect(56, 38, 28, 28),   new Rect(112, 38, 28, 28),
            new Rect(0, 76, 28, 28),    new Rect(56, 76, 28, 28),   new Rect(112, 76, 28, 28),
            new Rect(0, 114, 28, 28),   new Rect(56, 114, 28, 28),  new Rect(112, 114, 28, 28)
        };


        #endregion

        #region UI Controls

        Button recipesButton;
        Button mixButton;
        Button exitButton;

        ItemListScroller ingredientsListScroller;

        #endregion

        #region UI Textures

        Texture2D baseTexture;
        const string baseTextureName = "MASK00I0.IMG";
        const int alternateAlphaIndex = 12;

        #endregion

        List<DaggerfallUnityItem> ingredients = new List<DaggerfallUnityItem>();

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

            // Setup buttons
            SetupButtons();
            SetupItemListScrollers();
            Refresh();
        }

        public override void OnPush()
        {
            if (!IsSetup)
                return;

            Refresh();
        }

        void Refresh()
        {
            // Add ingredient items to list
            ingredients.Clear();
            ItemCollection playerItems = GameManager.Instance.PlayerEntity.Items;
            for (int i = 0; i < playerItems.Count; i++)
            {
                DaggerfallUnityItem item = playerItems.GetItem(i);
                if (item.IsIngredient)
                    ingredients.Add(item);
            }
            ingredientsListScroller.Items = ingredients;
        }

        #endregion

        #region Private Methods

        void LoadTextures()
        {
            baseTexture = ImageReader.GetTexture(baseTextureName, 0, 0, true, alternateAlphaIndex);
        }

        void SetupButtons()
        {
            // Exit button
            exitButton = DaggerfallUI.AddButton(exitButtonRect, NativePanel);
            exitButton.OnMouseClick += ExitButton_OnMouseClick;
        }

        protected void SetupItemListScrollers()
        {
            TextLabel miscLabelTemplate = new TextLabel(DaggerfallUI.Instance.Font3)
            {
                Position = new Vector2(0, ingredientButtonRects[0].height),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.None
            };
            // Setup item list scroller for ingredients
            ingredientsListScroller = new ItemListScroller(4, 3, ingredientsListRect, ingredientButtonRects, miscLabelTemplate, defaultToolTip, 2, 0.8f)
            {
                Position = new Vector2(ingredientsListScrollerRect.x, ingredientsListScrollerRect.y),
                Size = new Vector2(ingredientsListScrollerRect.width, ingredientsListScrollerRect.height),
                LabelTextHandler = ItemLabelTextHandler
            };
            NativePanel.Components.Add(ingredientsListScroller);
            ingredientsListScroller.OnItemClick += IngredientsListScroller_OnItemClick;
        }

        string ItemLabelTextHandler(DaggerfallUnityItem item)
        {
            return item.ItemName.ToUpper();
        }

        #endregion

        #region Event Handlers

        protected virtual void IngredientsListScroller_OnItemClick(DaggerfallUnityItem item)
        {
            Debug.Log("click");
        }

        private void ExitButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            CloseWindow();
        }

        private void RecipesButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            CloseWindow();
        }

        private void MixButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            CloseWindow();
        }

        #endregion
    }
}