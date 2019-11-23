// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2019 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut
// Contributors: Numidium

using UnityEngine;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.Formulas;
using DaggerfallWorkshop.Game.Entity;
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
        const int tooManyDaysFutureId = 16;
        const int offerPriceId = 262;
        const int notEnoughGoldId = 454;
        const int howManyAdditionalDaysId = 5100;
        const int howManyDaysId = 5102;

        static readonly string[] tavernMenu =  {
            HardStrings.tavernAle, HardStrings.tavernBeer, HardStrings.tavernMead, HardStrings.tavernWine,
            HardStrings.tavernBread, HardStrings.tavernBroth, HardStrings.tavernCheese, HardStrings.tavernFowl,
            HardStrings.tavernGruel, HardStrings.tavernPie, HardStrings.tavernStew };
        byte[] tavernFoodAndDrinkPrices = { 1, 1, 2, 3, 1, 1, 2, 3, 2, 2, 3 };

        StaticNPC merchantNPC;
        PlayerGPS.DiscoveredBuilding buildingData;
        RoomRental_v1 rentedRoom;
        int daysToRent = 0;
        int tradePrice = 0;

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
            mainPanel.Size = new Vector2(130, 44);

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

        #region Event Handlers

        private void ExitButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            CloseWindow();
        }

        private void RoomButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            int mapId = GameManager.Instance.PlayerGPS.CurrentLocation.MapTableData.MapId;
            int buildingKey = GameManager.Instance.PlayerEnterExit.BuildingDiscoveryData.buildingKey;
            GameManager.Instance.PlayerEntity.RemoveExpiredRentedRooms();
            rentedRoom = GameManager.Instance.PlayerEntity.GetRentedRoom(mapId, buildingKey);

            DaggerfallInputMessageBox inputMessageBox = new DaggerfallInputMessageBox(uiManager, this);
            inputMessageBox.SetTextTokens((rentedRoom == null) ? howManyDaysId : howManyAdditionalDaysId, this);
            inputMessageBox.TextPanelDistanceY = 0;
            inputMessageBox.InputDistanceX = 24;
            //inputMessageBox.InputDistanceY = -4;
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

            int daysAlreadyRented = 0;
            if (rentedRoom != null)
            {
                daysAlreadyRented = (int)((rentedRoom.expiryTime - DaggerfallUnity.Instance.WorldTime.Now.ToSeconds()) / DaggerfallDateTime.SecondsPerDay);
                if (daysAlreadyRented < 0)
                    daysAlreadyRented = 0;
            }

            if (daysToRent + daysAlreadyRented > 350)
            {
                DaggerfallUI.MessageBox(tooManyDaysFutureId);
            }
            else if (GameManager.Instance.GuildManager.GetGuild(FactionFile.GuildGroups.KnightlyOrder).FreeTavernRooms())
            {
                DaggerfallUI.MessageBox(HardStrings.roomFreeForKnightSuchAsYou);
                RentRoom();
            }
            else
            {
                int cost = FormulaHelper.CalculateRoomCost(daysToRent);
                tradePrice = FormulaHelper.CalculateTradePrice(cost, buildingData.quality, false);

                DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, this);
                TextFile.Token[] tokens = DaggerfallUnity.Instance.TextProvider.GetRandomTokens(offerPriceId);
                messageBox.SetTextTokens(tokens, this);
                messageBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.Yes);
                messageBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.No);
                messageBox.OnButtonClick += ConfirmRenting_OnButtonClick;
                uiManager.PushWindow(messageBox);
            }
        }

        private void ConfirmRenting_OnButtonClick(DaggerfallMessageBox sender, DaggerfallMessageBox.MessageBoxButtons messageBoxButton)
        {
            CloseWindow();
            if (messageBoxButton == DaggerfallMessageBox.MessageBoxButtons.Yes)
            {
                PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;
                if (playerEntity.GetGoldAmount() >= tradePrice)
                {
                    playerEntity.DeductGoldAmount(tradePrice);
                    RentRoom();
                }
                else
                    DaggerfallUI.MessageBox(notEnoughGoldId);
            }
        }

        private void RentRoom()
        {
            PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;
            PlayerEnterExit playerEnterExit = GameManager.Instance.PlayerEnterExit;

            int mapId = GameManager.Instance.PlayerGPS.CurrentLocation.MapTableData.MapId;
            string sceneName = DaggerfallInterior.GetSceneName(mapId, buildingData.buildingKey);
            if (rentedRoom == null)
            {
                // Get rest markers and select a random marker index for allocated bed
                // We store marker by index as building positions are not stable, they can move from terrain mods or floating Y
                Vector3[] restMarkers = playerEnterExit.Interior.FindMarkers(DaggerfallInterior.InteriorMarkerTypes.Rest);
                int markerIndex = Random.Range(0, restMarkers.Length);

                // Create room rental and add it to player rooms
                RoomRental_v1 room = new RoomRental_v1()
                {
                    name = buildingData.displayName,
                    mapID = mapId,
                    buildingKey = buildingData.buildingKey,
                    allocatedBedIndex = markerIndex,
                    expiryTime = DaggerfallUnity.Instance.WorldTime.Now.ToSeconds() + (ulong)(DaggerfallDateTime.SecondsPerDay * daysToRent)
                };
                playerEntity.RentedRooms.Add(room);
                SaveLoadManager.StateManager.AddPermanentScene(sceneName);
                Debug.LogFormat("Rented room for {1} days. {0}", sceneName, daysToRent);
            }
            else
            {
                rentedRoom.expiryTime += (ulong)(DaggerfallDateTime.SecondsPerDay * daysToRent);
                Debug.LogFormat("Rented room for additional {1} days. {0}", sceneName, daysToRent);
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
            uint gameMinutes = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime();

            if ((gameMinutes - (GameManager.Instance.PlayerEntity.LastTimePlayerAteOrDrankAtTavern)) >= 240)
            {
                DaggerfallListPickerWindow foodAndDrinkPicker = new DaggerfallListPickerWindow(uiManager, this);
                foodAndDrinkPicker.OnItemPicked += FoodAndDrink_OnItemPicked;

                foreach (string menuItem in tavernMenu)
                    foodAndDrinkPicker.ListBox.AddItem(menuItem);

                uiManager.PushWindow(foodAndDrinkPicker);
            }
            else
            {
                DaggerfallUI.MessageBox(HardStrings.youAreNotHungry);
            }
        }

        public void FoodAndDrink_OnItemPicked(int index, string foodOrDrinkName)
        {
            CloseWindow();
            int price = tavernFoodAndDrinkPrices[index];
            uint gameMinutes = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime();
            int holidayID = FormulaHelper.GetHolidayId(gameMinutes, 0);
            PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;

            // Note: In-game holiday description for both New Life Festival and Harvest's End say they offer free drinks.
            //       Below is what they actually do (New Life Festival = everything free,
            //       Harvest's End = everything half-price but at least 1 gold).

            if (holidayID == (int)DaggerfallConnect.DFLocation.Holidays.Harvest_End)
            {
                price >>= 1;
                if (price == 0)
                    price = 1;
            }
            if (holidayID != (int)DaggerfallConnect.DFLocation.Holidays.New_Life && playerEntity.GetGoldAmount() < price)
            {
                DaggerfallUI.MessageBox(notEnoughGoldId);
            }
            else
            {
                if (holidayID != (int)DaggerfallConnect.DFLocation.Holidays.New_Life)
                    playerEntity.DeductGoldAmount(price);
                playerEntity.SetHealth(playerEntity.CurrentHealth + 2 * price);
                playerEntity.LastTimePlayerAteOrDrankAtTavern = gameMinutes;
            }
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
                return parent.tradePrice.ToString();
            }

            public override string RoomHoursLeft()
            {
                return PlayerEntity.GetRemainingHours(parent.rentedRoom).ToString();
            }
        }

        #endregion

    }
}