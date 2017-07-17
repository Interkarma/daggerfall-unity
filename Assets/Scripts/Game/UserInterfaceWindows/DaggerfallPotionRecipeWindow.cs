// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2016 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: InconsolableCellist
//
// Notes:
//

using DaggerfallConnect.Arena2;
using DaggerfallConnect.FallExe;
using DaggerfallWorkshop.Game.UserInterface;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Extends the DaggerfallMessageBox to implement the potion recipe reader
    /// When a recipe is read from the inventory window the name of the potion recipe is displayed, followed by a second pop-up of all the ingredients
    /// </summary>
    public class DaggerfallPotionRecipeWindow : DaggerfallMessageBox
    {
        private ReaderState state;
        string recipeName;
        Recipe[] recipes;

        // The potion reader opens the description on the first click, then opens the actual recipe with the second click
        private enum ReaderState
        {
            DescriptionView = 0,
            RecipeView = 1
        }

        /// <summary>
        /// Creates a new Daggerfall potion recipe reader window, which can be displayed like any other DaggerfallMessageBox.
        /// Daggerfall stores the ID of the potion recipe in the "typeDependentData" field.
        /// If an incorrect value of typeDependentData is provided the recipe won't be found.
        /// </summary>
        /// <param name="uiManager">The IUserInterfaceManager</param>
        /// <param name="typeDependentData">The typeDependentData variable that is the ID of the potion recipe</param>
        /// <param name="previous">The previous IUserInterfaceWindow</param>
        /// <returns>DaggerfallUnityItem.</returns>
        public DaggerfallPotionRecipeWindow(IUserInterfaceManager uiManager, int typeDependentData, IUserInterfaceWindow previous = null)
            :base(uiManager, previous)
        {
            this.recipeID = typeDependentData;
        }

        public int recipeID
        {
            set
            {
                try
                {
                    KeyValuePair<string, Recipe[]> mapping = DaggerfallUnity.Instance.ItemHelper.getPotionRecipesByID(value);
                    recipeName = HardStrings.potionRecipeFor.Replace("%po", mapping.Key); // "Recipe for %po"
                    recipes = mapping.Value;
                    doLayout();
                }
                catch (Exception)
                {
                    Debug.Log("An error occurred while trying to get a potion recipe!");
                }
            }
        }

        // Does the necessary layout for the first window, which displays which potion this recipe describes
        private void doLayout()
        {
            TextFile.Token nameToken = new TextFile.Token();
            nameToken.text = recipeName;
            nameToken.formatting = TextFile.Formatting.Text;

            List<TextFile.Token> tokens = new List<TextFile.Token>();
            tokens.Add(nameToken);

            this.SetTextTokens(tokens.ToArray());
        }

        // The second window displays a list of all the ingredients in this potion recipe
        private void showIngredientsWindow()
        {
            DaggerfallMessageBox ingredientsWindow = new DaggerfallMessageBox(uiManager, this);
            List<TextFile.Token> tokens = new List<TextFile.Token>();

            // Potions can have multiple recipes, and it's unclear how this variation is stored
            // The actual variation could be stored in the currentVariation field, but I haven't been able find any recipes
            // in the game that aren't just the first recipe in the list;
            // We'll just pick the first one here
            Ingredient[] ingredients = recipes[0].ingredients;
            for (int x=0; x<ingredients.Length; ++x)
            {
                TextFile.Token ingredientToken = new TextFile.Token();
                ingredientToken.text = ingredients[x].name;
                ingredientToken.formatting = TextFile.Formatting.Text;
                tokens.Add(ingredientToken);
                tokens.Add(TextFile.NewLineToken);
            }

            ingredientsWindow.SetTextTokens(tokens.ToArray());
            ingredientsWindow.ScreenDimColor = new Color32(0, 0, 0, 0); // matches Daggerfall behavior
            ingredientsWindow.ClickAnywhereToClose = true;
            // When the child closes let us know about it
            ingredientsWindow.OnClose += ChildPanel_OnClose;
            uiManager.PushWindow(ingredientsWindow);
        }

        protected override void Setup()
        {
            base.Setup();

            this.state = ReaderState.DescriptionView;
        }

        public override void OnPush()
        {
            base.OnPush();
            parentPanel.OnMouseClick += ParentPanel_OnMouseClick;
        }

        public override void OnPop()
        {
            base.OnPop();
            parentPanel.OnMouseClick -= ParentPanel_OnMouseClick;
        }

        private void ParentPanel_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (uiManager.TopWindow == this)
            {
                if (state == ReaderState.DescriptionView)
                {
                    state = ReaderState.RecipeView;
                    showIngredientsWindow();
                }
                else
                    CloseWindow();
            }
        }

        // When the ingredients view closes, we should close to (matchs Daggerfall behavior)
        private void ChildPanel_OnClose()
        {
            if (uiManager.TopWindow == this)
            {
                state = ReaderState.DescriptionView; // just to be thorough
                CloseWindow();
            }
        }
    }
}