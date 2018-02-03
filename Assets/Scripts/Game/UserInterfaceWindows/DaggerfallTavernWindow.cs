// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut

using UnityEngine;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.Banking;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.Formulas;
using System;
using DaggerfallWorkshop.Game.Entity;
using System.Collections.Generic;
using DaggerfallWorkshop.Game.Serialization;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    public class DaggerfallTavernWindow : DaggerfallPopupWindow, IMacroContextProvider
    {
        #region UI Rects

        Rect roomButtonRect = new Rect(5, 5, 120, 7);
        Rect talkButtonRect = new Rect(5, 14, 120, 7);
        Rect foodButtonRect = new Rect(5, 23, 120, 7);
        Rect exitButtonRect = new Rect(5, 32, 120, 7);

        #endregion

        #region UI Controls

        Panel mainPanel = new Panel();
        Button roomButton;
        Button talkButton;
        Button foodButton;
        Button exitButton;

        #endregion

        #region UI Textures

        Texture2D baseTexture;

        #endregion

        #region Fields

        const string baseTextureName = "TVRN00I0.IMG";
        const int offerPriceId = 262;
        const int notEnoughGoldId = 454;
        const int howManyDaysId = 5102;

        StaticNPC merchantNPC;
        PlayerGPS.DiscoveredBuilding buildingData;
        int daysToRent = 0;

        #endregion

        #region Constructors

        public DaggerfallTavernWindow(IUserInterfaceManager uiManager, StaticNPC npc)
            : base(uiManager)
        {
            merchantNPC = npc;
            buildingData = GameManager.Instance.PlayerEnterExit.BuildingDiscoveryData;
            // Clear background
            ParentPanel.BackgroundColor = Color.clear;
        }

        public static int GetRemainingHours(RoomRental_v1 room)
        {
            if (room == null)
                return 0;

            ulong remainingSecs = room.expiryTime - DaggerfallUnity.Instance.WorldTime.Now.ToSeconds();
            return (int)(remainingSecs / DaggerfallDateTime.SecondsPerHour);
        }

        #endregion

        #region Setup Methods

        protected override void Setup()
        {
            // Load all textures
            baseTexture = ImageReader.GetTexture(baseTextureName);

            // Create interface panel
            mainPanel.HorizontalAlignment = HorizontalAlignment.Center;
            mainPanel.VerticalAlignment = VerticalAlignment.Middle;
            mainPanel.BackgroundTexture = baseTexture;
            mainPanel.Position = new Vector2(0, 50);
            mainPanel.Size = new Vector2(baseTexture.width, baseTexture.height);

            // Room button
            roomButton = DaggerfallUI.AddButton(roomButtonRect, mainPanel);
            roomButton.OnMouseClick += RoomButton_OnMouseClick;

            // Talk button
            talkButton = DaggerfallUI.AddButton(talkButtonRect, mainPanel);
            talkButton.OnMouseClick += TalkButton_OnMouseClick;

            // Food button
            foodButton = DaggerfallUI.AddButton(foodButtonRect, mainPanel);
            foodButton.OnMouseClick += FoodButton_OnMouseClick;

            // Exit button
            exitButton = DaggerfallUI.AddButton(exitButtonRect, mainPanel);
            exitButton.OnMouseClick += ExitButton_OnMouseClick;

            NativePanel.Components.Add(mainPanel);
        }

        #endregion

        #region Private Methods

        private int GetRoomPrice()
        {
            return FormulaHelper.CalculateRoomCost(buildingData.quality) * daysToRent;
        }

        #endregion

        #region Event Handlers

        private void ExitButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            CloseWindow();
        }

        private void RoomButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallInputMessageBox inputMessageBox = new DaggerfallInputMessageBox(uiManager, this);
            inputMessageBox.SetTextTokens(howManyDaysId);
            inputMessageBox.TextPanelDistanceY = 0;
            inputMessageBox.InputDistanceX = 15;
            inputMessageBox.InputDistanceY = -6;
            inputMessageBox.TextBox.Numeric = true;
            inputMessageBox.TextBox.MaxCharacters = 3;
            inputMessageBox.TextBox.Text = "1";
            inputMessageBox.OnGotUserInput += InputMessageBox_OnGotUserInput;
            inputMessageBox.Show();
        }

        private void InputMessageBox_OnGotUserInput(DaggerfallInputMessageBox sender, string input)
        {
            daysToRent = 0;
            bool result = int.TryParse(input, out daysToRent);
            if (!result || daysToRent < 1)
                return;

            DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, this);
            TextFile.Token[] tokens = DaggerfallUnity.Instance.TextProvider.GetRandomTokens(offerPriceId);
            messageBox.SetTextTokens(tokens, this);
            messageBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.Yes);
            messageBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.No);
            messageBox.OnButtonClick += ConfirmRenting_OnButtonClick;
            uiManager.PushWindow(messageBox);
        }

        private void ConfirmRenting_OnButtonClick(DaggerfallMessageBox sender, DaggerfallMessageBox.MessageBoxButtons messageBoxButton)
        {
            CloseWindow();
            if (messageBoxButton == DaggerfallMessageBox.MessageBoxButtons.Yes)
            {
                PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;
                if (playerEntity.GetGoldAmount() >= GetRoomPrice())
                {
                    playerEntity.DeductGoldAmount(GetRoomPrice());
                    int mapId = GameManager.Instance.PlayerGPS.CurrentLocation.MapTableData.MapId;
                    string sceneName = DaggerfallInterior.GetSceneName(mapId, buildingData.buildingKey);
                    RoomRental_v1 room = new RoomRental_v1() {
                        name = buildingData.displayName,
                        mapID = mapId,
                        buildingKey = buildingData.buildingKey,
                        expiryTime = DaggerfallUnity.Instance.WorldTime.Now.ToSeconds() + (ulong)(DaggerfallDateTime.SecondsPerDay * daysToRent)
                    };
                    playerEntity.RoomRentals.Add(room);
                    SaveLoadManager.StateManager.AddPermanentScene(sceneName);
                    Debug.LogFormat("Rented room for {1} days. {0}", sceneName, daysToRent);
                }
                else
                    DaggerfallUI.MessageBox(notEnoughGoldId);
            }
        }

        private void TalkButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            CloseWindow();
            GameManager.Instance.TalkManager.TalkToStaticNPC(merchantNPC);
        }

        private void FoodButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            CloseWindow();
        }

        #endregion

        #region Macro handling

        public MacroDataSource GetMacroDataSource()
        {
            return new TavernMacroDataSource(this);
        }

        /// <summary>
        /// MacroDataSource context sensitive methods for tavern window.
        /// </summary>
        private class TavernMacroDataSource : MacroDataSource
        {
            private DaggerfallTavernWindow parent;
            public TavernMacroDataSource(DaggerfallTavernWindow tavernWindow)
            {
                this.parent = tavernWindow;
            }

            public override string Amount()
            {
                return parent.GetRoomPrice().ToString();
            }
        }

        #endregion

    }
}