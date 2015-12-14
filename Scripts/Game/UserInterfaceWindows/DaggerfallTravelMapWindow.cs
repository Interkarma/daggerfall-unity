﻿// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Lypyl (lypyl@dfworkshop.net), Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//


using UnityEngine;
using System;
using System.Linq;
using DaggerfallConnect;
using DaggerfallConnect.Utility;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.UserInterface;
using System.Collections;
using System.Collections.Generic;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    ///TODO:
    ///1. Betony
    ///2. Zoom
    ///3. Final travel options window + pathfinding


    /// <summary>
    /// Implements Daggerfall's travel map.
    /// </summary>
    public class DaggerfallTravelMapWindow : DaggerfallPopupWindow
    {
        #region Fields

        const string nativeImgName = "TRAV0I00.IMG";
        const string regionPickerImgName = "TRAV0I01.IMG";
        const string findAtButtonImgName = "TRAV0I03.IMG";
        const string locationFilterButtonEnabledImgName = "TRAV01I0.IMG";
        const string locationFilterButtonDisabledImgName = "TRAV01I1.IMG";
        const string downArrowImgName = "TRAVAI05.IMG";
        const string upArrowImgName = "TRAVBI05.IMG";
        const string rightArrowImgName = "TRAVCI05.IMG";
        const string leftArrowImgName = "TRAVDI05.IMG";
        const string regionBorderImgName = "MBRD00I0.IMG";

        const int identifyFlashCount = 4;
        const float identifyFlashInterval = 0.5f;

        Dictionary<string, Vector2> offsetLookup = new Dictionary<string, Vector2>();

        DFBitmap regionPickerBitmap;
        Panel regionTextureOverlayPanel;
        Panel locationClusterPanel;
        Panel identifyRegionPanel;
        Panel crossHairPanel;
        Panel borderPanel;

        TextLabel regionLabel;
        Texture2D nativeTexture;
        Texture2D regionTexture;

        Texture2D dungeonFilterButtonEnabled;
        Texture2D dungeonFilterButtonDisabled;
        Texture2D templesFilterButtonEnabled;
        Texture2D templesFilterButtonDisabled;
        Texture2D homesFilterButtonEnabled;
        Texture2D homesFilterButtonDisabled;
        Texture2D townsFilterButtonEnabled;
        Texture2D townsFilterButtonDisabled;
        Texture2D upArrowTexture;
        Texture2D downArrowTexture;
        Texture2D leftArrowTexture;
        Texture2D rightArrowTexture;
        Texture2D borderTexture;

        Texture2D identifyRegionOverlayTexture = null;
        Texture2D crossHairTexture;
        Texture2D locationClusterTexture;


        Button findButton;
        Button atButton;
        Button exitButton;
        Button horizontalArrowButton = new Button();
        Button verticalArrowButton = new Button();
        Button dungeonsFilterButton = new Button();
        Button templesFilterButton = new Button();
        Button homesFilterButton = new Button();
        Button townsFilterButton = new Button();

        Rect regionTextureOverlayPanelRect = new Rect(0, 12, 320, 160);
        Rect dungeonsFilterButtonSrcRect = new Rect(0, 0, 99, 11);
        Rect templesFilterButtonSrcRect = new Rect(0, 11, 99, 11);
        Rect homesFilterButtonSrcRect = new Rect(99, 0, 80, 11);
        Rect townsFilterButtonSrcRect = new Rect(99, 11, 80, 11);
        Rect crossHairRect = new Rect(12, 12, 296, 160);//x pos + 12 & size - 24 to comp. for borders

        int mouseOverRegion = -1;
        int selectedRegion = -1;
        string[] selectedRegionMapNames = new string[0];

        int mapIndex = 0;

        Color32[] locationClusterColors;
        Color identifyFlashColor = new Color32(162, 36, 12, 255);

        bool identifyState = false;
        float identifyLastChangeTime = 0;
        float identifyChanges = 0;
        bool identifyingRegion = false;
        bool findingLocation = false;
        bool locationSelected = false;
        DFRegion currentRegion;
        ContentReader.MapSummary locationSummary;

        static bool filterDungeons = false;
        static bool filterTemples = false;
        static bool filterHomes = false;
        static bool filterTowns = false;

        float identifyLocationLastChangeTime = 0;
        float identifyLocationChanges = 0;
        float scale = 1.0f;

        Vector2 lastMousePos = Vector2.zero;

        Color32[] locationPixelColors = new Color32[]
        {
            new Color32(215,117,36, 255),           //dunglab
            new Color32(190, 85, 24, 255),          //dungkeep
            new Color32(170, 48, 12,255),           //dungruin
            new Color32(146, 12, 4, 255),           //Graveyards
            new Color32(166, 101, 69, 255),         //farms
            new Color32(195, 134, 101, 255),        //wealthy
            new Color32(142, 85, 52, 255),          //poor
            new Color32(178, 207, 255, 255),        //temple
            new Color32(69, 125, 195, 255),         //cult
            new Color32(12, 12, 12, 255),           //coven
            new Color32(125, 81, 89, 255),          //tavern
            new Color32(223, 178, 178, 255),        //city
            new Color32(190, 138, 138, 255),        //hamlet
            new Color32(154, 105, 105, 255),        //village
        };

        #endregion

        #region Properties

        string RegionImgName { get; set; }

        bool HasMultipleMaps
        {
            get { return (selectedRegionMapNames.Length > 1) ? true : false; }
        }

        bool HasVerticalMaps
        {
            get { return (selectedRegionMapNames.Length > 2) ? true : false; }
        }

        #endregion


        #region Constructors

        public DaggerfallTravelMapWindow(IUserInterfaceManager uiManager)
            : base(uiManager)
        {
        }

        #endregion

        #region User Interface

        protected override void Setup()
        {
            // Load native texture
            nativeTexture = DaggerfallUI.GetTextureFromImg(nativeImgName);
            if (!nativeTexture)
                throw new Exception("DaggerfallTravelMap: Could not load native texture.");

            // Always dim background
            ParentPanel.BackgroundColor = ScreenDimColor;

            // Setup native panel background
            NativePanel.BackgroundTexture = nativeTexture;

            // Populate the offset dict
            PopulateRegionOffsetDict();

            // Don't allow automatic cancel, we will handle this locally
            AllowCancel = false;

            // Load picker colours
            regionPickerBitmap = DaggerfallUI.GetImgBitmap(regionPickerImgName);

            // Add region label
            regionLabel = DaggerfallUI.AddTextLabel(DaggerfallUI.DefaultFont, new Vector2(0, 2), string.Empty, NativePanel);
            regionLabel.HorizontalAlignment = HorizontalAlignment.Center;

            // Handle clicks
            NativePanel.OnMouseClick += ClickHandler;

            // Setup buttons for first time
            LoadButtonTextures();
            SetupButtons();
            UpdateSearchButtons();


            // Region overlay panel
            regionTextureOverlayPanel = DaggerfallUI.AddPanel(regionTextureOverlayPanelRect, NativePanel);
            regionTextureOverlayPanel.Enabled = false;

            // Location cluster panel
            locationClusterPanel = DaggerfallUI.AddPanel(regionTextureOverlayPanelRect, regionTextureOverlayPanel);
            locationClusterPanel.HorizontalAlignment = HorizontalAlignment.Left;
            locationClusterPanel.VerticalAlignment = VerticalAlignment.Top;

            // Location cluster will be updated when user opens a region
            locationClusterColors = new Color32[(int)regionTextureOverlayPanelRect.width * (int)regionTextureOverlayPanelRect.height];
            locationClusterTexture = new Texture2D((int)regionTextureOverlayPanelRect.width, (int)regionTextureOverlayPanelRect.height);
            locationClusterTexture.filterMode = DaggerfallUI.Instance.GlobalFilterMode;

            //borders around the region maps
            borderTexture = DaggerfallUI.GetTextureFromImg(regionBorderImgName);
            borderPanel = DaggerfallUI.AddPanel(new Rect(new Vector2(0,0), regionTextureOverlayPanelRect.size), regionTextureOverlayPanel);
            borderPanel.BackgroundTexture = borderTexture;

            // Identify region panel is displayed to show user what region they are in
            identifyRegionPanel = new Panel();
            identifyRegionPanel.Position = new Vector2(0, 0);
            identifyRegionPanel.Size = NativePanel.Size;
            identifyRegionPanel.BackgroundTexture = identifyRegionOverlayTexture;
            identifyRegionPanel.Enabled = false;
            NativePanel.Components.Add(identifyRegionPanel);

            //crosshair panel
            //CreateCrossHair(GetPlayerMapPosition());
            crossHairPanel = new Panel();
            crossHairPanel.Position = crossHairRect.position;//new Vector2(12, regionTextureOverlayPanelRect.position.y);
            crossHairPanel.Size = crossHairRect.size;//new Vector2(regionTextureOverlayPanelRect.size.x-24, regionTextureOverlayPanelRect.size.y);
            crossHairPanel.BackgroundTexture = crossHairTexture;
            crossHairPanel.Enabled = false;
            NativePanel.Components.Add(crossHairPanel);

            // This is same behaviour as Daggerfall which flashes player region when travel map opens
            //on first push need to setup here
            identifyRegionOverlayTexture = CreatePlayerRegionOverlayTexture();
            identifyRegionPanel.BackgroundTexture = identifyRegionOverlayTexture;
            StartRegionIdentify();

        }

        public override void OnPush()
        {
            base.OnPush();
            // This is same behaviour as Daggerfall which flashes player region when travel map opens
            // Always identify region on push
            if(base.IsSetup)
            {
                identifyRegionOverlayTexture = CreatePlayerRegionOverlayTexture();
                identifyRegionPanel.BackgroundTexture = identifyRegionOverlayTexture;
                StartRegionIdentify();
            }
        }

        public override void OnPop()
        {
            base.OnPop();

            // Stop any identification in progress
            StopRegionIdentify();
        }

        public override void Update()
        {
            base.Update();

            //if (Input.GetKeyDown(exitKey))
                //CloseTravelWindows();
            if(Input.GetKeyDown(exitKey))
            {
                if (findingLocation || identifyingRegion)
                    StopRegionIdentify();
                else if (selectedRegion != -1)
                    CloseTravelWindows();
            }
            if (Input.GetKeyDown(KeyCode.F) && selectedRegion != -1)
                FindlocationButtonClickHandler(null, Vector2.zero);

            // Play identify animations
            if (identifyingRegion && selectedRegion == -1)
                AnimateRegionIdentify(identifyRegionPanel);
            else if (identifyingRegion && selectedRegion != -1)
                AnimateRegionIdentify(crossHairPanel);

            Vector2 currentMousePos = NativePanel.ScaledMousePosition;
            if(currentMousePos != lastMousePos)
            {
                lastMousePos = currentMousePos;
                UpdateMouseOverRegion();
                UpdateRegionLabel();
                if (selectedRegion != -1)
                    UpdateMouseOverLocation();
            }

        }

        #endregion

        #region Setup

        // Initial button setup
        void SetupButtons()
        {
            // Exit button
            exitButton = DaggerfallUI.AddButton(new Rect(278, 175, 39, 22), NativePanel);
            exitButton.OnMouseClick += ExitButtonClickHandler;

            // Find button
            Texture2D findButtonTexture = DaggerfallUI.GetTextureFromImg(findAtButtonImgName, new Rect(0, 0, 45, 11));
            findButton = DaggerfallUI.AddButton(new Rect(3, 175, findButtonTexture.width, findButtonTexture.height), NativePanel);
            findButton.BackgroundTexture = findButtonTexture;
            //findButton.Enabled = false;
            findButton.OnMouseClick += FindlocationButtonClickHandler;

            // I'm At button
            Texture2D atButtonTexture = DaggerfallUI.GetTextureFromImg(findAtButtonImgName, new Rect(0, 11, 45, 11));
            atButton = DaggerfallUI.AddButton(new Rect(3, 186, findButtonTexture.width, findButtonTexture.height), NativePanel);
            atButton.BackgroundTexture = atButtonTexture;
            atButton.OnMouseClick += AtButtonClickHandler;

            // Dungeons filter button
            dungeonsFilterButton.Position = new Vector2(50, 175);
            dungeonsFilterButton.Size = new Vector2(dungeonsFilterButtonSrcRect.width, dungeonsFilterButtonSrcRect.height);
            dungeonsFilterButton.OnMouseClick += DungeonsFilterButtonClickHandler;
            NativePanel.Components.Add(dungeonsFilterButton);

            // Temples filter button
            templesFilterButton.Position = new Vector2(50, 186);
            templesFilterButton.Size = new Vector2(templesFilterButtonSrcRect.width, templesFilterButtonSrcRect.height);
            templesFilterButton.OnMouseClick += TemplesFilterButtonClickHandler;
            NativePanel.Components.Add(templesFilterButton);

            // Homes filter button
            homesFilterButton.Position = new Vector2(149, 175);
            homesFilterButton.Size = new Vector2(homesFilterButtonSrcRect.width, homesFilterButtonSrcRect.height);
            homesFilterButton.OnMouseClick += HomesFilterButtonClickHandler;
            NativePanel.Components.Add(homesFilterButton);

            // Towns filter button
            townsFilterButton.Position = new Vector2(149, 186);
            townsFilterButton.Size = new Vector2(townsFilterButtonSrcRect.width, townsFilterButtonSrcRect.height);
            townsFilterButton.OnMouseClick += TownsFilterButtonClickHandler;
            NativePanel.Components.Add(townsFilterButton);

            // Horizontal arrow button
            horizontalArrowButton.Position = new Vector2(231, 176);
            horizontalArrowButton.Size = new Vector2(leftArrowTexture.width, leftArrowTexture.height);
            horizontalArrowButton.Enabled = false;
            NativePanel.Components.Add(horizontalArrowButton);
            horizontalArrowButton.OnMouseClick += HorizontalArrowButtonClickHander;

            // Vertical arrow button
            verticalArrowButton.Position = new Vector2(254, 176);
            verticalArrowButton.Size = new Vector2(upArrowTexture.width, upArrowTexture.height);
            verticalArrowButton.Enabled = false;
            NativePanel.Components.Add(verticalArrowButton);
            verticalArrowButton.OnMouseClick += VerticalArrowButtonClickHander;

        }

        void SetupArrowButtons()
        {
            // Vertical arrow
            if (selectedRegionMapNames.Length > 2)
            {
                verticalArrowButton.Enabled = true;
                verticalArrowButton.BackgroundTexture = (mapIndex > 1) ? upArrowTexture : downArrowTexture;
            }
            else
                verticalArrowButton.Enabled = false;

            // Horizontal arrow
            if (selectedRegionMapNames.Length > 1)
            {
                horizontalArrowButton.Enabled = true;
                horizontalArrowButton.BackgroundTexture = (mapIndex % 2 == 0) ? rightArrowTexture : leftArrowTexture;
            }
            else
                horizontalArrowButton.Enabled = false;
        }

        // Loads textures for buttons
        void LoadButtonTextures()
        {
            // Dungeons toggle button
            dungeonFilterButtonEnabled = DaggerfallUI.GetTextureFromImg(locationFilterButtonEnabledImgName, dungeonsFilterButtonSrcRect);
            dungeonFilterButtonDisabled = DaggerfallUI.GetTextureFromImg(locationFilterButtonDisabledImgName, dungeonsFilterButtonSrcRect);

            // Dungeons toggle button
            templesFilterButtonEnabled = DaggerfallUI.GetTextureFromImg(locationFilterButtonEnabledImgName, templesFilterButtonSrcRect);
            templesFilterButtonDisabled = DaggerfallUI.GetTextureFromImg(locationFilterButtonDisabledImgName, templesFilterButtonSrcRect);

            // Homes toggle button
            homesFilterButtonEnabled = DaggerfallUI.GetTextureFromImg(locationFilterButtonEnabledImgName, homesFilterButtonSrcRect);
            homesFilterButtonDisabled = DaggerfallUI.GetTextureFromImg(locationFilterButtonDisabledImgName, homesFilterButtonSrcRect);

            // Towns toggle button
            townsFilterButtonEnabled = DaggerfallUI.GetTextureFromImg(locationFilterButtonEnabledImgName, townsFilterButtonSrcRect);
            townsFilterButtonDisabled = DaggerfallUI.GetTextureFromImg(locationFilterButtonDisabledImgName, townsFilterButtonSrcRect);

            // Arrows
            upArrowTexture = DaggerfallUI.GetTextureFromImg(upArrowImgName);
            downArrowTexture = DaggerfallUI.GetTextureFromImg(downArrowImgName);
            leftArrowTexture = DaggerfallUI.GetTextureFromImg(leftArrowImgName);
            rightArrowTexture = DaggerfallUI.GetTextureFromImg(rightArrowImgName);
        }

        // Creates the region overlay for current player region
        Texture2D CreatePlayerRegionOverlayTexture()
        {
            // Player must be inside a valid region
            int playerRegion = GetPlayerRegion();
            if (playerRegion == -1)
                return null;

            if (regionPickerBitmap == null)
                regionPickerBitmap = DaggerfallUI.GetImgBitmap(regionPickerImgName);

            // Create a texture map overlay for the region area
            int width = regionPickerBitmap.Width;
            int height = regionPickerBitmap.Height;

            Texture2D texture = new Texture2D(width, height);
            texture.filterMode = DaggerfallUI.Instance.GlobalFilterMode;

            // Create array for region fill
            Color32[] colors = new Color32[width * height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int srcOffset = y * width + x;
                    int dstOffset = ((height - y - 1) * width) + x;
                    int sampleRegion = regionPickerBitmap.Data[srcOffset] - 128;
                    if (sampleRegion == playerRegion)
                        colors[dstOffset] = identifyFlashColor;
                }
            }

            // Assign colors to texture
            texture.SetPixels32(colors, 0);
            texture.Apply(false, true);

            return texture;
        }

        void CreateCrossHair(DFPosition pos, int regionIndex = -1)
        {
            if (regionIndex == -1)
                regionIndex = GetPlayerRegion();
            CreateCrossHair(pos.X, pos.Y, regionIndex);
        }

        /// <summary>
        /// sets up crosshair texture & panel used to by "at" and "find buttons
        /// </summary>
        /// <param name="mapPixelX"></param>
        /// <param name="mapPixelY"></param>
        void CreateCrossHair(int mapPixelX, int mapPixelY, int regionIndex)
        {
            if (selectedRegion == -1)
                return;

            string mapName = selectedRegionMapNames[mapIndex];
            Vector2 origin = offsetLookup[mapName];
            scale = GetRegionMapScale(regionIndex);

            int scaledX = (int)((mapPixelX - origin.x) / scale);
            int scaledY = (int)((mapPixelY - origin.y + 12) / scale);

            if (crossHairTexture == null)
                crossHairTexture = new Texture2D((int)crossHairPanel.Size.x, (int)crossHairPanel.Size.y);

            for (int x = 0; x < crossHairTexture.width; x++)
            {
                for (int y = 0; y < crossHairTexture.height; y++)
                {
                    if (x + 12 == scaledX || y + 12 == scaledY)
                        crossHairTexture.SetPixel(x, crossHairTexture.height - y - 1, identifyFlashColor);
                    else
                        crossHairTexture.SetPixel(x, crossHairTexture.height - y - 1, Color.clear);
                }
            }
            crossHairTexture.Apply();
            crossHairPanel.BackgroundTexture = crossHairTexture;
            crossHairPanel.Enabled = true;
        }

        // Populates offset dictionary for aligning top-left of map to map pixel coordinates.
        // Most maps have a 1:1 pixel ratio with map cells. A couple of maps have a larger scale.
        void PopulateRegionOffsetDict()
        {
            offsetLookup = new Dictionary<string, Vector2>();
            offsetLookup.Add("FMAPAI00.IMG", new Vector2(212, 340));
            offsetLookup.Add("FMAPBI00.IMG", new Vector2(322, 340));
            offsetLookup.Add("FMAPAI01.IMG", new Vector2(583, 279));
            offsetLookup.Add("FMAPBI01.IMG", new Vector2(680, 279));
            offsetLookup.Add("FMAPCI01.IMG", new Vector2(583, 340));
            offsetLookup.Add("FMAPDI01.IMG", new Vector2(680, 340));
            offsetLookup.Add("FMAP0I05.IMG", new Vector2(381, 4));
            offsetLookup.Add("FMAP0I09.IMG", new Vector2(525, 114));
            offsetLookup.Add("FMAP0I11.IMG", new Vector2(437, 340));
            offsetLookup.Add("FMAPAI16.IMG", new Vector2(578, 0));
            offsetLookup.Add("FMAPBI16.IMG", new Vector2(680, 0));
            offsetLookup.Add("FMAPCI16.IMG", new Vector2(578, 52));
            offsetLookup.Add("FMAPDI16.IMG", new Vector2(680, 52));
            offsetLookup.Add("FMAP0I17.IMG", new Vector2(39, 106));
            offsetLookup.Add("FMAP0I18.IMG", new Vector2(20, 29));
            offsetLookup.Add("FMAP0I19.IMG", new Vector2(80, 123));        // Betony scale different
            offsetLookup.Add("FMAP0I20.IMG", new Vector2(217, 293));
            offsetLookup.Add("FMAP0I21.IMG", new Vector2(263, 79));
            offsetLookup.Add("FMAP0I22.IMG", new Vector2(548, 219));
            offsetLookup.Add("FMAP0I23.IMG", new Vector2(680, 146));
            offsetLookup.Add("FMAP0I26.IMG", new Vector2(680, 80));
            offsetLookup.Add("FMAP0I32.IMG", new Vector2(41, 0));
            offsetLookup.Add("FMAP0I33.IMG", new Vector2(660, 101));
            offsetLookup.Add("FMAP0I34.IMG", new Vector2(578, 40));
            offsetLookup.Add("FMAP0I35.IMG", new Vector2(525, 3));
            offsetLookup.Add("FMAP0I36.IMG", new Vector2(440, 40));
            offsetLookup.Add("FMAP0I37.IMG", new Vector2(448, 0));
            offsetLookup.Add("FMAP0I38.IMG", new Vector2(366, 0));
            offsetLookup.Add("FMAP0I39.IMG", new Vector2(300, 8));
            offsetLookup.Add("FMAP0I40.IMG", new Vector2(202, 0));
            offsetLookup.Add("FMAP0I41.IMG", new Vector2(223, 6));
            offsetLookup.Add("FMAP0I42.IMG", new Vector2(148, 76));
            offsetLookup.Add("FMAP0I43.IMG", new Vector2(15, 340));
            offsetLookup.Add("FMAP0I44.IMG", new Vector2(61, 340));
            offsetLookup.Add("FMAP0I45.IMG", new Vector2(86, 338));
            offsetLookup.Add("FMAP0I46.IMG", new Vector2(132, 340));
            offsetLookup.Add("FMAP0I47.IMG", new Vector2(344, 309));
            offsetLookup.Add("FMAP0I48.IMG", new Vector2(381, 251));
            offsetLookup.Add("FMAP0I49.IMG", new Vector2(553, 255));
            offsetLookup.Add("FMAP0I50.IMG", new Vector2(661, 217));
            offsetLookup.Add("FMAP0I51.IMG", new Vector2(672, 275));
            offsetLookup.Add("FMAP0I52.IMG", new Vector2(680, 256));
            offsetLookup.Add("FMAP0I53.IMG", new Vector2(680, 340));
            offsetLookup.Add("FMAP0I54.IMG", new Vector2(491, 340));
            offsetLookup.Add("FMAP0I55.IMG", new Vector2(293, 340));
            offsetLookup.Add("FMAP0I56.IMG", new Vector2(263, 340));
            offsetLookup.Add("FMAP0I57.IMG", new Vector2(680, 157));
            offsetLookup.Add("FMAP0I58.IMG", new Vector2(17, 53));
            offsetLookup.Add("FMAP0I59.IMG", new Vector2(0, 0));        // Glenumbra Moors correct at 0,0
            offsetLookup.Add("FMAP0I60.IMG", new Vector2(107, 11));
            offsetLookup.Add("FMAP0I61.IMG", new Vector2(255, 275));    // Cybiades
        }

        #endregion


        #region Event Handlers

        /// <summary>
        /// Handle clicks on the main panel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="position"></param>
        void ClickHandler(BaseScreenComponent sender, Vector2 position)
        {
            if (selectedRegion == -1 && mouseOverRegion != -1)
                OpenRegionPanel(mouseOverRegion);

            else if(locationSelected)
            {
                if (position.x < 0 || position.x > regionTextureOverlayPanelRect.width || position.y < 12 || position.y > regionTextureOverlayPanelRect.height) //make sure clicks are inside region texture
                    return;                                                                                                                                     //to prevent creating popup when player clicking buttons etc.
                TextFile.Token[] textTokens = new TextFile.Token[2];
                //int index = currentRegion.MapIdLookup[locationSummary.MapIndex];
                textTokens[0].text = string.Format("Travel to  {0} : {1} ?", currentRegion.Name, currentRegion.MapNames[locationSummary.MapIndex]);
                textTokens[0].formatting = TextFile.Formatting.Text;
                textTokens[1].text = null;
                textTokens[1].formatting = TextFile.Formatting.NewLine;

                DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, this);    // Temp solution, replace w/ final travel pop-up
                messageBox.SetTextTokens(textTokens);
                messageBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.Yes);
                messageBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.No);
                messageBox.OnButtonClick += ConfirmTravelPopupButtonClick;
                uiManager.PushWindow(messageBox);
            }
        }

        /// <summary>
        /// Handle clicks on the region panel overlay.
        /// </summary>
        void RegionPanelClickHandler(BaseScreenComponent sender, Vector2 position)
        {
        }

        void ExitButtonClickHandler(BaseScreenComponent sender, Vector2 position)
        {
            CloseTravelWindows();
        }

        void AtButtonClickHandler(BaseScreenComponent sender, Vector2 position)
        {
            // Identify region or map location
            if (selectedRegion == -1)
                StartRegionIdentify();
            else
            {
                CreateCrossHair(GetPlayerMapPosition());//put flashing crosshair over player pos
                StartRegionIdentify();
            }
        }

        void FindlocationButtonClickHandler(BaseScreenComponent sender, Vector2 position)
        {
            if (selectedRegion == -1)           // Do nothing
                return;
            else                                // Open find location pop-up
            {
                StopRegionIdentify();
                DaggerfallInputMessageBox findPopUp = new DaggerfallInputMessageBox(uiManager, null, 31, HardStrings.findLocationPrompt, true, true, this);
                findPopUp.TextPanelDistance = 0;
                findPopUp.OnGotUserInput += HandleLocationFindEvent;
                findPopUp.Show();
            }
        }

        /// <summary>
        /// Button handler for travel confirmation pop up. This is a temporary solution until implementing the final pop-up.
        /// </summary>
        void ConfirmTravelPopupButtonClick(DaggerfallMessageBox sender, DaggerfallMessageBox.MessageBoxButtons messageBoxButton)
        {
            sender.CloseWindow();
            if (messageBoxButton == DaggerfallMessageBox.MessageBoxButtons.Yes)
            {
                DFRegion.RegionMapTable map = currentRegion.MapTable[locationSummary.MapIndex];
                TravelToLocation(MapsFile.LongitudeLatitudeToMapPixel((int)map.Longitude, (int)map.Latitude));
            }
            else
                StopRegionIdentify();
        }

        void HorizontalArrowButtonClickHander(BaseScreenComponent sender, Vector2 position)
        {
            if (selectedRegion == -1 || !HasMultipleMaps)
                return;

            int newIndex = mapIndex;
            if (newIndex % 2 == 0)
                newIndex += 1;          // Move right
            else
                newIndex -= 1;          // Move left

            mapIndex = newIndex;
            SetupArrowButtons();
            regionTextureOverlayPanel.BackgroundTexture = DaggerfallUI.GetTextureFromImg(selectedRegionMapNames[mapIndex]);
            UpdateLocationCluster();
        }

        void VerticalArrowButtonClickHander(BaseScreenComponent sender, Vector2 position)
        {
            if (selectedRegion == -1 || !HasVerticalMaps)
                return;

            int newIndex = mapIndex;
            if (newIndex > 1)
                newIndex -= 2;          // Move up
            else
                newIndex += 2;          // Move down

            mapIndex = newIndex;
            SetupArrowButtons();
            regionTextureOverlayPanel.BackgroundTexture = DaggerfallUI.GetTextureFromImg(selectedRegionMapNames[mapIndex]);
            UpdateLocationCluster();
        }

        void DungeonsFilterButtonClickHandler(BaseScreenComponent sender, Vector2 position)
        {
            filterDungeons = !filterDungeons;

            if (filterDungeons)
                dungeonsFilterButton.BackgroundTexture = dungeonFilterButtonDisabled;
            else
                dungeonsFilterButton.BackgroundTexture = dungeonFilterButtonEnabled;

            if (selectedRegion == -1)
                return;

            UpdateLocationCluster();
        }

        void TemplesFilterButtonClickHandler(BaseScreenComponent sender, Vector2 position)
        {
            filterTemples = !filterTemples;

            if (filterTemples)
                templesFilterButton.BackgroundTexture = templesFilterButtonDisabled;
            else
                templesFilterButton.BackgroundTexture = templesFilterButtonEnabled;

            if (selectedRegion == -1)
                return;

            UpdateLocationCluster();
        }

        void HomesFilterButtonClickHandler(BaseScreenComponent sender, Vector2 position)
        {
            filterHomes = !filterHomes;

            if (filterHomes)
                homesFilterButton.BackgroundTexture = homesFilterButtonDisabled;
            else
                homesFilterButton.BackgroundTexture = homesFilterButtonEnabled;

            if (selectedRegion == -1)
                return;

            UpdateLocationCluster();
        }


        void TownsFilterButtonClickHandler(BaseScreenComponent sender, Vector2 position)
        {
            filterTowns = !filterTowns;
            if (filterTowns)
                townsFilterButton.BackgroundTexture = townsFilterButtonDisabled;
            else
                townsFilterButton.BackgroundTexture = townsFilterButtonEnabled;

            if (selectedRegion == -1)
                return;

            UpdateLocationCluster();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Opens the region panel to the specified region.
        /// </summary>
        void OpenRegionPanel(int region)
        {
            string[] mapNames = GetRegionMapNames(region);
            if (mapNames == null || mapNames.Length == 0)
                return;

            mapIndex = 0;
            selectedRegion = region;
            selectedRegionMapNames = mapNames;
            StopRegionIdentify();
            SetupArrowButtons();

            regionTextureOverlayPanel.Enabled = true;
            regionTexture = DaggerfallUI.GetTextureFromImg(mapNames[0]);
            regionTextureOverlayPanel.BackgroundTexture = regionTexture;
            regionTextureOverlayPanel.OnMouseClick += RegionPanelClickHandler;



            currentRegion = DaggerfallUnity.ContentReader.MapFileReader.GetRegion(region);
            UpdateLocationCluster();
        }

        // Close region panel and reset values
        void CloseRegionPanel()
        {
            selectedRegion = -1;
            locationSelected = false;
            findingLocation = false;
            mapIndex = 0;
            StopRegionIdentify();

            if (regionTextureOverlayPanel != null)
            {
                regionTextureOverlayPanel.Enabled = false;
                regionTexture = null;
                regionTextureOverlayPanel.OnMouseClick -= RegionPanelClickHandler;
                horizontalArrowButton.Enabled = false;
                verticalArrowButton.Enabled = false;
            }
        }

        // Updates location cluster texture
        void UpdateLocationCluster()
        {
            string mapName = selectedRegionMapNames[mapIndex];
            Vector2 origin = offsetLookup[mapName];
            int originX = (int)origin.x;
            int originY = (int)origin.y;

            // Find locations within this region
            int width = (int)regionTextureOverlayPanelRect.width;
            int height = (int)regionTextureOverlayPanelRect.height;

            scale = GetRegionMapScale(selectedRegion);

            //clear texture
            for (int i = 0; i < locationClusterColors.Length; i++)
            {
                locationClusterColors[i] = Color.clear;
            }


            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int offset = (int)((((height - y - 1) * width) + x) * scale);
                    if (offset >= locationClusterColors.Length)
                        continue;
                    int sampleRegion = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetPoliticIndex(originX + x, originY + y) - 128;

                    // Set location pixel if inside region area
                    if (sampleRegion == selectedRegion)
                    {
                        ContentReader.MapSummary summary;
                        if (DaggerfallUnity.Instance.ContentReader.HasLocation(originX + x, originY + y, out summary))
                        {
                            int index = GetPixelColorIndex(summary.LocationType);
                            if (index == -1)
                                continue;
                            else
                                locationClusterColors[offset] = locationPixelColors[index];
                        }
                    }

                    //// TEST: Mark entire region red to test sampling area
                    //if (sampleRegion == selectedRegion)
                    //    locationClusterColors[offset] = Color.red;
                    //else
                    //    locationClusterColors[offset] = Color.clear;
                }
            }

            // Assign map to texture
            locationClusterTexture.SetPixels32(locationClusterColors);
            locationClusterTexture.Apply(false, false);

            // Assign texture to panel
            locationClusterPanel.BackgroundTexture = locationClusterTexture;
        }

        //checks if player mouse over valid location while region selected & not finding location
        void UpdateMouseOverLocation()
        {
            if (selectedRegion == -1 || findingLocation)
                return;
            string mapName = selectedRegionMapNames[mapIndex];
            Vector2 origin = offsetLookup[mapName];
            scale = GetRegionMapScale(selectedRegion);
            Vector2 position = NativePanel.ScaledMousePosition;
            int x = (int)(((position.x + origin.x)) * scale);
            int y = (int)(((position.y + origin.y - 12)) * scale);
            int sampleRegion = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetPoliticIndex(x, y) - 128;
            locationSelected = false;
            if (sampleRegion != selectedRegion)
                return;

            if (DaggerfallUnity.ContentReader.HasLocation(x, y) && !findingLocation)
            {
                DaggerfallUnity.ContentReader.HasLocation(x, y, out locationSummary);

                if (locationSummary.MapIndex < 0 || locationSummary.MapIndex >= currentRegion.MapNames.Length)
                    return;
                else
                {
                    int index = GetPixelColorIndex(locationSummary.LocationType);
                    if (index == -1)
                        return;
                    locationSelected = true;
                    // regionLabel.Text = string.Format("{0} : {1}", DaggerfallUnity.ContentReader.MapFileReader.GetRegionName(selectedRegion), currentRegion.MapNames[locationSummary.MapIndex]);
                }
            }
        }


        void UpdateMouseOverRegion()
        {
            mouseOverRegion = -1;

            // Get offset into region picker bitmap
            Vector2 position = NativePanel.ScaledMousePosition;
            int offset = (int)position.y * regionPickerBitmap.Width + (int)position.x;
            if (offset < 0 || offset >= regionPickerBitmap.Data.Length)
                return;

            // Get region from bitmap, if any
            int region = regionPickerBitmap.Data[offset] - 128;
            if (region < 0 || region >= DaggerfallUnity.Instance.ContentReader.MapFileReader.RegionCount)
                return;

            // Store valid region
            mouseOverRegion = region;
        }

        // Updates the text label at top of screen
        void UpdateRegionLabel()
        {
            if (selectedRegion == -1)
                regionLabel.Text = GetRegionName(mouseOverRegion);
            else if (locationSelected)
                regionLabel.Text = string.Format("{0} : {1}", DaggerfallUnity.ContentReader.MapFileReader.GetRegionName(selectedRegion), currentRegion.MapNames[locationSummary.MapIndex]);
            else
                regionLabel.Text = GetRegionName(selectedRegion);
        }

        // Closes windows based on context
        void CloseTravelWindows(bool forceClose = false)
        {
            if (selectedRegion == -1 || forceClose)
            {
                // TODO: Need to ensure this does not "fall through" into general input and pause game
                CloseWindow();
            }

            // Close region panel
            CloseRegionPanel();
        }

        // Updates search button toggle state based on current flags
        void UpdateSearchButtons()
        {
            // Dungeons
            if (!filterDungeons)
                dungeonsFilterButton.BackgroundTexture = dungeonFilterButtonEnabled;
            else
                dungeonsFilterButton.BackgroundTexture = dungeonFilterButtonDisabled;

            // Temples
            if (!filterTemples)
                templesFilterButton.BackgroundTexture = templesFilterButtonEnabled;
            else
                templesFilterButton.BackgroundTexture = templesFilterButtonDisabled;

            // Homes
            if (!filterHomes)
                homesFilterButton.BackgroundTexture = homesFilterButtonEnabled;
            else
                homesFilterButton.BackgroundTexture = homesFilterButtonDisabled;

            // Towns
            if (!filterTowns)
                townsFilterButton.BackgroundTexture = townsFilterButtonEnabled;
            else
                townsFilterButton.BackgroundTexture = townsFilterButtonDisabled;
        }

        #endregion

        #region Helper Methods

        //returns index to locationPixelColor array or -1 if invalid or filtered
        int GetPixelColorIndex(DFRegion.LocationTypes locationType)
        {
            int index = -1;
            switch (locationType)
            {
                case DFRegion.LocationTypes.DungeonLabyrinth:
                    index = 0;
                    break;
                case DFRegion.LocationTypes.DungeonKeep:
                    index = 1;
                    break;
                case DFRegion.LocationTypes.DungeonRuin:
                    index = 2;
                    break;
                case DFRegion.LocationTypes.GraveyardCommon:
                    index = 3;
                    break;
                case DFRegion.LocationTypes.GraveyardForgotten:
                    index = 3;
                    break;
                case DFRegion.LocationTypes.HomeFarms:
                    index = 4;
                    break;
                case DFRegion.LocationTypes.HomeWealthy:
                    index = 5;
                    break;
                case DFRegion.LocationTypes.HomePoor:
                    index = 6;
                    break;
                case DFRegion.LocationTypes.HomeYourShips:
                    break;
                case DFRegion.LocationTypes.ReligionTemple:
                    index = 7;
                    break;
                case DFRegion.LocationTypes.ReligionCult:
                    index = 8;
                    break;
                case DFRegion.LocationTypes.ReligionCoven:
                    index = 9;
                    break;
                case DFRegion.LocationTypes.Tavern:
                    index = 10;
                    break;
                case DFRegion.LocationTypes.TownCity:
                    index = 11;
                    break;
                case DFRegion.LocationTypes.TownHamlet:
                    index = 12;
                    break;
                case DFRegion.LocationTypes.TownVillage:
                    index = 13;
                    break;
                default:
                    break;
            }
            if (index < 0)
                return index;
            else if (index < 4 && filterDungeons)
                index = -1;
            else if (index > 3 && index < 7 && filterHomes)
                index = -1;
            else if (index > 6 && index < 9 && filterTemples)
                index = -1;
            else if (index > 9 && index < 14 && filterTowns)
                index = -1;
            return index;
        }

        /// <summary>
        /// Handles events from Find Location pop-up.
        /// </summary>
        void HandleLocationFindEvent(DaggerfallInputMessageBox inputMessageBox, string locationName)
        {
            //locationSelected = false;

            if (string.IsNullOrEmpty(locationName) || !FindLocation(locationName))
            {
                TextFile.Token[] textTokens = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(13);
                DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, this);
                messageBox.SetTextTokens(textTokens);
                messageBox.ClickAnywhereToClose = true;
                uiManager.PushWindow(messageBox);
                return;
            }
            else //place flashing crosshair over location
            {
                locationSelected = true;
                findingLocation = true;
                CreateCrossHair(MapsFile.GetPixelFromPixelID(locationSummary.ID), locationSummary.RegionIndex);
                StartRegionIdentify();
            }
        }

        // Find location by name
        bool FindLocation(string name)
        {
            DFRegion.RegionMapTable locationInfo = new DFRegion.RegionMapTable();

            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            string[] locations = currentRegion.MapNames.OrderBy(p => p).ToArray();
            name = name.ToLower();

            for (int i = 0; i < locations.Count(); i++)
            {
                if (locations[i].ToLower().StartsWith(name))                        // Valid location found,
                {
                    if (!currentRegion.MapNameLookup.ContainsKey(locations[i]))
                    {
                        DaggerfallUnity.LogMessage("Error: location name key not found in Region MapNameLookup dictionary");
                        return false;
                    }
                    int index = currentRegion.MapNameLookup[locations[i]];
                    locationInfo = currentRegion.MapTable[index];
                    DFPosition pos = MapsFile.LongitudeLatitudeToMapPixel((int)locationInfo.Longitude, (int)locationInfo.Latitude);
                    if (DaggerfallUnity.ContentReader.HasLocation(pos.X, pos.Y, out locationSummary))
                        return true;
                    else
                        return false;

                }
                else if (locations[i][0] > name[0])
                    return false;
            }

            return false;
        }

        // Teleports player to position
        void TravelToLocation(DFPosition pos)
        {
            TravelToLocation(pos.X, pos.Y);
        }

        // Teleports player to coordinates of location
        void TravelToLocation(int x, int y)
        {
            try
            {
                GameObject.FindObjectOfType<StreamingWorld>().TeleportToCoordinates(x, y);
                CloseTravelWindows(true);
            }
            catch (Exception ex)
            {
                DaggerfallUnity.LogMessage(ex.Message);
            }
        }

        // Gets current player position in map pixels
        DFPosition GetPlayerMapPosition()
        {
            DFPosition position = new DFPosition();
            PlayerGPS playerGPS = GameManager.Instance.PlayerGPS;
            if (playerGPS)
                position = playerGPS.CurrentMapPixel;

            return position;
        }

        // Gets current player region or -1 if player not in any region (e.g. in ocean)
        int GetPlayerRegion()
        {
            DFPosition position = GetPlayerMapPosition();
            int region = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetPoliticIndex(position.X, position.Y) - 128;
            if (region < 0 || region >= DaggerfallUnity.Instance.ContentReader.MapFileReader.RegionCount)
                return -1;

            return region;
        }

        // Gets name of region
        string GetRegionName(int region)
        {
            return DaggerfallUnity.Instance.ContentReader.MapFileReader.GetRegionName(region);
        }

        // Gets maps for region
        string[] GetRegionMapNames(int region)
        {
            // Get map name array with special handling for multi-screen regions
            if (region == 0)
                return new string[] { "FMAPAI00.IMG", "FMAPBI00.IMG" };
            else if (region == 1)
                return new string[] { "FMAPAI01.IMG", "FMAPBI01.IMG", "FMAPCI01.IMG", "FMAPDI01.IMG" };
            else if (region == 16)
                return new string[] { "FMAPAI16.IMG", "FMAPBI16.IMG", "FMAPCI16.IMG", "FMAPDI16.IMG" };
            else
                return new string[] { string.Format("FMAP0I{0:00}.IMG", region) };
        }

        // Gets scale of region map
        float GetRegionMapScale(int region)
        {
            if (region == 19)//betony
                return 4f;
            else
                return 1;
        }

        #endregion


        #region Region Identification

        // Start region identification & location crosshair
        void StartRegionIdentify()
        {
            if (identifyingRegion)//stop animation
                StopRegionIdentify();
            identifyingRegion = true;
            identifyState = false;
            identifyChanges = 0;
            identifyLastChangeTime = 0;
        }

        // Stop region identification & location crosshair
        void StopRegionIdentify()
        {
            identifyingRegion = false;
            identifyState = false;
            identifyChanges = 0;
            identifyLastChangeTime = 0;
            identifyRegionPanel.Enabled = false;
            crossHairPanel.Enabled = false;
            findingLocation = false;
        }

        // Animate region identification & location crosshair
        void AnimateRegionIdentify(Panel atPanel)
        {
            // Must have an overlay texture and not have a region selected
            //if (!identifyRegionOverlayTexture || selectedRegion != -1)
            //{
               // StopRegionIdentify();
                //return;
            //}

            if(!atPanel.BackgroundTexture)
            {
                StopRegionIdentify();
                return;
            }

            // Check if enough time has elapsed since last flash and toggle state
            bool lastIdentifyState = identifyState;
            float time = Time.realtimeSinceStartup;
            if (time > identifyLastChangeTime + identifyFlashInterval)
            {
                identifyState = !identifyState;
                identifyLastChangeTime = time;
            }

            // Set panel visibility based on state
            atPanel.Enabled = identifyState;
            //identifyRegionPanel.Enabled = identifyState;

            // Turn off flash after specified number of on states
            if (!lastIdentifyState && identifyState)
            {
                if (++identifyChanges > identifyFlashCount)
                {
                    StopRegionIdentify();
                }
            }
        }

        #endregion
    }
}
