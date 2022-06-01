// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut

using UnityEngine;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Banking;
using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop.Utility.AssetInjection;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    public class DaggerfallBankPurchasePopUp : DaggerfallPopupWindow
    {
        #region UI Rects

        Rect displayPanelRect = new Rect(117, 12, 104, 91);
        Rect buyButtonRect = new Rect(38, 106, 40, 19);
        Rect exitButtonRect = new Rect(150, 106, 40, 19);
        Rect upArrowRect = new Rect(0, 0, 9, 16);
        Rect downArrowRect = new Rect(0, 64, 9, 16);
        DFSize arrowsFullSize = new DFSize(9, 80);

        #endregion

        #region UI Controls

        Panel mainPanel = new Panel();

        ListBox priceListBox;
        Button priceListUpButton;
        Button priceListDownButton;
        VerticalScrollBar priceListScrollBar;

        Panel displayPanel;
        Button buyButton;
        Button exitButton;

        #endregion

        #region UI Textures

        const string baseTextureName = "BANK01I0.IMG";
        Texture2D baseTexture;
        const string greenArrowsTextureName = "BANK01I1.IMG";   // Green up/down arrows when more items available
        const string redArrowsTextureName = "BANK01I2.IMG";     // Red up/down arrows when no more items available
        Texture2D redUpArrow;
        Texture2D greenUpArrow;
        Texture2D redDownArrow;
        Texture2D greenDownArrow;

        Texture2D displayTexture;
        Vector2 displayResolution;

        #endregion

        #region Constants

        private const int listDisplayUnits = 10;  // Number of items displayed in scrolling area
        private const int scrollNum = 1;          // Number of items on each scroll tick
        private const string goName = "BankPurchase";
        private const float rotSpeed = 0.02f;
        private const float brightness = 0.4f;

        #endregion

        #region Properties & Variables

        protected DaggerfallBankingWindow bankingWindow;
        private List<BuildingSummary> housesForSale;

        static int layer = 12;
        static GameObject goBankPurchase;

        GameObject goCameraBankPurchase;
        Camera camera;
        GameObject goLight;
        GameObject goModel;

        float lastRotTime;

        #endregion

        #region Constructors

        public DaggerfallBankPurchasePopUp(IUserInterfaceManager uiManager, DaggerfallBankingWindow previousWindow = null, List<BuildingSummary> housesForSale = null)
            : base(uiManager, previousWindow)
        {
            this.housesForSale = housesForSale;
            bankingWindow = previousWindow;
        }

        static DaggerfallBankPurchasePopUp()
        {
            int editorLayer = LayerMask.NameToLayer(goName);
            if (editorLayer == -1)
                DaggerfallUnity.LogMessage("Did not find Layer with name \"BankPurchase\"! Defaulting to Layer 12.", true);
            else
                layer = editorLayer;

            Camera.main.cullingMask = Camera.main.cullingMask & ~((1 << layer)); // don't render this layer with main camera

            goBankPurchase = GameObject.Find(goName);
        }

        #endregion

        #region Setup Methods

        protected override void Setup()
        {
            // Load textures
            LoadTextures();

            // Create interface panel
            mainPanel.HorizontalAlignment = HorizontalAlignment.Center;
            mainPanel.VerticalAlignment = VerticalAlignment.Middle;
            mainPanel.BackgroundTexture = baseTexture;
            mainPanel.Position = new Vector2(0, 50);
            mainPanel.Size = new Vector2(225, 129);

            // Buy button
            buyButton = DaggerfallUI.AddButton(buyButtonRect, mainPanel);
            buyButton.OnMouseClick += BuyButton_OnMouseClick;

            // No button
            exitButton = DaggerfallUI.AddButton(exitButtonRect, mainPanel);
            exitButton.OnMouseClick += ExitButton_OnMouseClick;

            SetupScrollBar();
            SetupScrollButtons();
            SetupPriceList();
            SetupDisplayPanel();

            NativePanel.Components.Add(mainPanel);
        }

        public override void OnPop()
        {
            if (goModel)
            {
                Object.Destroy(goModel);
                goModel = null;
            }
            if (goLight)
                Object.Destroy(goLight);

            if (goCameraBankPurchase)
            {
                RenderTexture renderTexture = camera.targetTexture;
                camera.targetTexture = null;
                Object.Destroy(renderTexture);
                Object.Destroy(goCameraBankPurchase);
            }
        }

        public override void Update()
        {
            base.Update();

            if (Time.realtimeSinceStartup > lastRotTime + rotSpeed)
            {
                lastRotTime = Time.realtimeSinceStartup;
                if (goModel)
                {
                    // Render the model into display panel
                    RenderModel();
                    // Rotate model
                    goModel.transform.Rotate(Vector3.up, -1);
                }
            }
        }

        #endregion

        #region Private, Setup methods

        private void PopulatePriceList()
        {
            if (housesForSale == null)
            {
                for (int i = 0; i < 2; i++)
                    priceListBox.AddItem(TextManager.Instance.GetLocalizedText("bankPurchasePrice").Replace("%s", DaggerfallBankManager.GetShipPrice((ShipType) i).ToString()), i);
            }
            else
            {   // List all the houses for sale in this location
                foreach (BuildingSummary house in housesForSale)
                {
                    priceListBox.AddItem(TextManager.Instance.GetLocalizedText("bankPurchasePrice").Replace("%s", DaggerfallBankManager.GetHousePrice(house).ToString()));
                }
            }
        }

        private void SetupDisplayPanel()
        {
            displayPanel = DaggerfallUI.AddPanel(displayPanelRect, mainPanel);
            displayResolution = new Vector2(displayPanelRect.width * NativePanel.LocalScale.x, displayPanelRect.height * NativePanel.LocalScale.y);
            displayTexture = new Texture2D((int)displayResolution.x, (int)displayResolution.y, TextureFormat.ARGB32, false);

            // Create camera
            if (!goCameraBankPurchase)
            {
                goCameraBankPurchase = new GameObject("Camera");
                camera = goCameraBankPurchase.AddComponent<Camera>();
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.cullingMask = 1 << layer;
                camera.renderingPath = Camera.main.renderingPath;
                camera.nearClipPlane = 0.7f;
                camera.farClipPlane = 100.0f;
                goCameraBankPurchase.transform.SetParent(goBankPurchase.transform);
            }
            if (!goLight)
            {
                goLight = new GameObject("Light");
                Light light = goLight.AddComponent<Light>();
                light.type = LightType.Directional;
                light.transform.position = new Vector3(0, 50, -50);
                light.intensity = brightness;
                light.shadows = LightShadows.Hard;
                light.cullingMask = 1 << layer;
                goLight.transform.SetParent(goBankPurchase.transform);
            }

            RenderTexture renderTexture = new RenderTexture((int)displayResolution.x, (int)displayResolution.y, 16);
            camera.targetTexture = renderTexture;
        }

        private void Display3dModelSelection(int selectedIdx)
        {
            if (goModel)
            {
                Object.Destroy(goModel);
                goModel = null;
            }

            // Position camera and set model id
            uint modelId = 0;
            if (housesForSale == null)
            {
                camera.transform.position = new Vector3(0, 12, DaggerfallBankManager.GetShipCameraDist((ShipType)selectedIdx));
                modelId = DaggerfallBankManager.GetShipModelId((ShipType)selectedIdx);
            }
            else
            {
                camera.transform.position = new Vector3(0, 3, -20);
                BuildingSummary house = housesForSale[selectedIdx];
                modelId = house.ModelID;
            }

            // Inject custom GameObject if available else create standard mesh game object for the model
            goModel = MeshReplacement.ImportCustomGameobject(modelId, goBankPurchase.transform, new Matrix4x4());
            if (goModel == null)
                goModel = GameObjectHelper.CreateDaggerfallMeshGameObject(modelId, goBankPurchase.transform);

            goModel.layer = layer;

            // Apply current climate
            ClimateBases climateBase = ClimateSwaps.FromAPIClimateBase(GameManager.Instance.PlayerGPS.ClimateSettings.ClimateType);
            ClimateSeason season = (DaggerfallUnity.WorldTime.Now.SeasonValue == DaggerfallDateTime.Seasons.Winter) ? ClimateSeason.Winter : ClimateSeason.Summer;
            DaggerfallMesh dfMesh = goModel.GetComponent<DaggerfallMesh>();
            dfMesh.SetClimate(climateBase, season, WindowStyle.Day);
        }

        private void RenderModel()
        {
            camera.Render();

            RenderTexture.active = camera.targetTexture;
            displayTexture.ReadPixels(new Rect(0, 0, (int)displayResolution.x, (int)displayResolution.y), 0, 0);
            displayTexture.Apply(false);
            RenderTexture.active = null;
            displayPanel.BackgroundTexture = displayTexture;

        }

        private void SetupPriceList()
        {
            priceListBox = new ListBox()
            {
                Name = "price_list",
                Position = new Vector2(5, 24),
                Size = new Vector2(99, 78),
                RowsDisplayed = listDisplayUnits,
                MaxCharacters = 20,
            };
            mainPanel.Components.Add(priceListBox);
            priceListBox.OnScroll += PriceListBox_OnScroll;
            priceListBox.OnSelectItem += PriceListBox_OnSelectItem;

            PopulatePriceList();

            priceListBox.SelectNone();
            priceListScrollBar.TotalUnits = priceListBox.Count;
            UpdateListScrollerButtons(priceListBox.ScrollIndex, priceListBox.Count);
        }

        void SetupScrollBar()
        {
            // Price list scroll bar
            priceListScrollBar = new VerticalScrollBar
            {
                Position = new Vector2(106, 39),
                Size = new Vector2(7, 48),
                DisplayUnits = listDisplayUnits
            };
            mainPanel.Components.Add(priceListScrollBar);
            priceListScrollBar.OnScroll += PriceScrollBar_OnScroll;
        }

        void SetupScrollButtons()
        {
            // Item list scroll buttons
            priceListUpButton = new Button
            {
                Position = new Vector2(105, 23),
                Size = new Vector2(9, 16),
                BackgroundTexture = redUpArrow
            };
            mainPanel.Components.Add(priceListUpButton);
            priceListUpButton.OnMouseClick += PriceUpButton_OnMouseClick;

            priceListDownButton = new Button
            {
                Position = new Vector2(105, 87),
                Size = new Vector2(9, 16),
                BackgroundTexture = redDownArrow
            };
            mainPanel.Components.Add(priceListDownButton);
            priceListDownButton.OnMouseClick += PriceDownButton_OnMouseClick;
        }

        // Updates red/green state of scroller buttons
        void UpdateListScrollerButtons(int index, int count)
        {
            // Update up button
            priceListUpButton.BackgroundTexture = (index > 0) ? greenUpArrow : redUpArrow;

            // Update down button
            priceListDownButton.BackgroundTexture = (index < (count - listDisplayUnits)) ? greenDownArrow : redDownArrow;

            // No items above or below
            if (count <= listDisplayUnits)
            {
                priceListUpButton.BackgroundTexture = redUpArrow;
                priceListDownButton.BackgroundTexture = redDownArrow;
            }
        }

        void LoadTextures()
        {
            baseTexture = ImageReader.GetTexture(baseTextureName);

            // Cut out red up/down arrows
            Texture2D redArrowsTexture = ImageReader.GetTexture(redArrowsTextureName);
            redUpArrow = ImageReader.GetSubTexture(redArrowsTexture, upArrowRect, arrowsFullSize);
            redDownArrow = ImageReader.GetSubTexture(redArrowsTexture, downArrowRect, arrowsFullSize);

            // Cut out green up/down arrows
            Texture2D greenArrowsTexture = ImageReader.GetTexture(greenArrowsTextureName);
            greenUpArrow = ImageReader.GetSubTexture(greenArrowsTexture, upArrowRect, arrowsFullSize);
            greenDownArrow = ImageReader.GetSubTexture(greenArrowsTexture, downArrowRect, arrowsFullSize);
        }

        #endregion

        #region Event Handlers

        private void ExitButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            CloseWindow();
        }

        private void BuyButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            if (priceListBox.SelectedIndex < 0)
                return;

            CloseWindow();
            if (housesForSale == null)
                bankingWindow.GeneratePurchaseShipPopup((ShipType)priceListBox.SelectedIndex);
            else
                bankingWindow.GeneratePurchaseHousePopup(housesForSale[priceListBox.SelectedIndex]);
        }

        void PriceListBox_OnSelectItem()
        {
            Debug.Log("Selected " + priceListBox.SelectedIndex);
            Display3dModelSelection(priceListBox.SelectedIndex);
        }


        void PriceListBox_OnScroll()
        {
            UpdateListScrollerButtons(priceListBox.ScrollIndex, priceListBox.Count);
            priceListScrollBar.ScrollIndex = priceListBox.ScrollIndex;
            priceListScrollBar.Update();
        }

        void PriceScrollBar_OnScroll()
        {
            priceListBox.ScrollIndex = priceListScrollBar.ScrollIndex;
        }

        void PriceUpButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            priceListBox.ScrollUp();
        }

        void PriceDownButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            priceListBox.ScrollDown();
        }

        void PriceListPanel_OnMouseScrollUp(BaseScreenComponent sender)
        {
            priceListBox.ScrollUp();
        }

        void PriceListPanel_OnMouseScrollDown(BaseScreenComponent sender)
        {
            priceListBox.ScrollDown();
        }

        #endregion
    }
}
