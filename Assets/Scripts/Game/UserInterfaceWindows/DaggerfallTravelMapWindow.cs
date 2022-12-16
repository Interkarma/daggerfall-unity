// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Lypyl (lypyl@dfworkshop.net), Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    Hazelnut, TheLacus
// 
// Notes:
//

using UnityEngine;
using System;
using System.Linq;
using System.IO;
using DaggerfallConnect;
using DaggerfallConnect.Utility;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.UserInterface;
using System.Collections.Generic;
using Wenzil.Console;
using Wenzil.Console.Commands;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Utility.AssetInjection;
using DaggerfallWorkshop.Game.Questing;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Implements Daggerfall's travel map.
    /// </summary>
    public class DaggerfallTravelMapWindow : DaggerfallPopupWindow
    {
        #region Fields

        protected const int betonyIndex = 19;

        protected const string overworldImgName                       = "TRAV0I00.IMG";
        protected const string regionPickerImgName                    = "TRAV0I01.IMG";
        protected const string findAtButtonImgName                    = "TRAV0I03.IMG";
        protected const string locationFilterButtonEnabledImgName     = "TRAV01I0.IMG";
        protected const string locationFilterButtonDisabledImgName    = "TRAV01I1.IMG";
        protected const string downArrowImgName                       = "TRAVAI05.IMG";
        protected const string upArrowImgName                         = "TRAVBI05.IMG";
        protected const string rightArrowImgName                      = "TRAVCI05.IMG";
        protected const string leftArrowImgName                       = "TRAVDI05.IMG";
        protected const string regionBorderImgName                    = "MBRD00I0.IMG";
        protected const string colorPaletteColName                    = "FMAP_PAL.COL";
        protected const int regionPanelOffset                         = 12;
        protected const int identifyFlashCount                        = 4;
        protected const int identifyFlashCountSelected                = 2;
        protected const float identifyFlashInterval                   = 0.5f;
        protected const int dotsOutlineThickness                      = 1;
        protected Color32 dotOutlineColor                             = new Color32(0, 0, 0, 128);
        protected Vector2[] outlineDisplacements =
        {
            new Vector2(-0.5f, -0f),
            new Vector2(0f, -0.5f),
            new Vector2(0f, 0.5f),
            new Vector2(0.5f, 0f)
        };

        protected DaggerfallTravelPopUp popUp;

        protected Dictionary<string, Vector2> offsetLookup = new Dictionary<string, Vector2>();
        protected string[] selectedRegionMapNames;

        protected Place gotoPlace; // Used by journal click-through to fast travel to a specific quest location

        protected DFBitmap regionPickerBitmap;
        protected DFRegion currentDFRegion;
        protected int currentDFRegionIndex = -1;
        protected int lastQueryLocationIndex = -1;
        protected string lastQueryLocationName;
        protected ContentReader.MapSummary locationSummary;

        protected KeyCode toggleClosedBinding;

        protected Panel borderPanel;
        protected Panel regionTextureOverlayPanel;
        protected Panel[] regionLocationDotsOutlinesOverlayPanel;
        protected Panel regionLocationDotsOverlayPanel;
        protected Panel playerRegionOverlayPanel;
        protected Panel identifyOverlayPanel;

        protected TextLabel regionLabel;

        protected Texture2D overworldTexture;
        protected Texture2D identifyTexture;
        protected Texture2D locationDotsTexture;
        protected Texture2D locationDotsOutlineTexture;
        protected Texture2D findButtonTexture;
        protected Texture2D atButtonTexture;
        protected Texture2D dungeonFilterButtonEnabled;
        protected Texture2D dungeonFilterButtonDisabled;
        protected Texture2D templesFilterButtonEnabled;
        protected Texture2D templesFilterButtonDisabled;
        protected Texture2D homesFilterButtonEnabled;
        protected Texture2D homesFilterButtonDisabled;
        protected Texture2D townsFilterButtonEnabled;
        protected Texture2D townsFilterButtonDisabled;
        protected Texture2D upArrowTexture;
        protected Texture2D downArrowTexture;
        protected Texture2D leftArrowTexture;
        protected Texture2D rightArrowTexture;
        protected Texture2D borderTexture;

        protected Button findButton;
        protected Button atButton;
        protected Button exitButton;
        protected Button horizontalArrowButton    = new Button();
        protected Button verticalArrowButton      = new Button();
        protected Button dungeonsFilterButton     = new Button();
        protected Button templesFilterButton      = new Button();
        protected Button homesFilterButton        = new Button();
        protected Button townsFilterButton        = new Button();

        protected Rect playerRegionOverlayPanelRect   = new Rect(0, 0, 320, 200);
        protected Rect regionTextureOverlayPanelRect  = new Rect(0, regionPanelOffset, 320, 160);
        protected Rect dungeonsFilterButtonSrcRect    = new Rect(0, 0, 99, 11);
        protected Rect templesFilterButtonSrcRect     = new Rect(0, 11, 99, 11);
        protected Rect homesFilterButtonSrcRect       = new Rect(99, 0, 80, 11);
        protected Rect townsFilterButtonSrcRect       = new Rect(99, 11, 80, 11);
        protected Rect findButtonRect                 = new Rect(0, 0, 45, 11);
        protected Rect atButtonRect                   = new Rect(0, 11, 45, 11);

        protected Color32[] identifyPixelBuffer;
        protected Color32[] locationDotsPixelBuffer;
        protected Color32[] locationDotsOutlinePixelBuffer;
        protected Color32[] locationPixelColors;              // Pixel colors for different location types
        protected Color identifyFlashColor;

        protected int zoomfactor                  = 2;
        protected int mouseOverRegion             = -1;
        protected int selectedRegion              = -1;
        protected int mapIndex                    = 0;        // Current index of loaded map from selectedRegionMapNames
        protected float scale                     = 1.0f;
        protected float identifyLastChangeTime    = 0;
        protected float identifyChanges           = 0;

        protected bool identifyState          = false;
        protected bool identifying            = false;
        protected bool locationSelected       = false;
        protected bool findingLocation        = false;
        protected bool zoom                   = false;        // Toggles zoom mode
        protected bool teleportationTravel    = false;        // Indicates travel should be by teleportation
        protected static bool revealUndiscoveredLocations;    // Flag used to indicate cheat/debugging mode for revealing undiscovered locations

        protected bool filterDungeons = false;
        protected bool filterTemples = false;
        protected bool filterHomes = false;
        protected bool filterTowns = false;

        protected Vector2 lastMousePos = Vector2.zero;
        protected Vector2 zoomOffset = Vector2.zero;
        protected Vector2 zoomPosition = Vector2.zero;

        protected readonly Dictionary<string, Texture2D> regionTextures = new Dictionary<string, Texture2D>();
        protected readonly Dictionary<int, Texture2D> importedOverlays = new Dictionary<int, Texture2D>();

        protected readonly int maxMatchingResults = 1000;
        protected string distanceRegionName = null;
        protected IDistance distance;

        // Populated with localized names whenever player searches or lists inside this region
        // Used to complete search and list on localized names over canonical names
        protected Dictionary<string, int> localizedMapNameLookup = new Dictionary<string, int>();

        #endregion

        #region Properties

        protected string RegionImgName { get; set; }

        protected bool HasMultipleMaps
        {
            get { return (selectedRegionMapNames.Length > 1) ? true : false; }
        }

        protected bool HasVerticalMaps
        {
            get { return (selectedRegionMapNames.Length > 2) ? true : false; }
        }

        protected bool RegionSelected
        {
            get { return selectedRegion != -1; }
        }

        protected bool MouseOverRegion
        {
            get { return mouseOverRegion != -1; }
        }

        protected bool MouseOverOtherRegion
        {
            get { return RegionSelected && (selectedRegion != mouseOverRegion); }
        }

        protected bool FindingLocation
        {
            get { return identifying && findingLocation && RegionSelected; }
        }

        public ContentReader.MapSummary LocationSummary { get => locationSummary; }

        public void ActivateTeleportationTravel()
        {
            teleportationTravel = true;
        }

        public void GotoPlace(Place place)
        {
            gotoPlace = place;
        }

        #endregion

        #region Constructors

        public DaggerfallTravelMapWindow(IUserInterfaceManager uiManager)
            : base(uiManager)
        {
            // Register console commands
            try
            {
                TravelMapConsoleCommands.RegisterCommands();
            }
            catch (Exception ex)
            {
                Debug.LogError(string.Format("Error Registering Travelmap Console commands: {0}", ex.Message));
            }

            // Prevent duplicate close calls with base class's exitKey (Escape)
            AllowCancel = false;
        }

        #endregion

        #region User Interface

        protected override void Setup()
        {
            ParentPanel.BackgroundColor = Color.black;

            // Set location pixel colors and identify flash color from palette file
            DFPalette colors = new DFPalette();
            if (!colors.Load(Path.Combine(DaggerfallUnity.Instance.Arena2Path, colorPaletteColName)))
                throw new Exception("DaggerfallTravelMap: Could not load color palette.");

            locationPixelColors = new Color32[]
            {
                new Color32(colors.GetRed(237), colors.GetGreen(237), colors.GetBlue(237), 255),  //dunglab (R215, G119, B39)
                new Color32(colors.GetRed(240), colors.GetGreen(240), colors.GetBlue(240), 255),  //dungkeep (R191, G87, B27)
                new Color32(colors.GetRed(243), colors.GetGreen(243), colors.GetBlue(243), 255),  //dungruin (R171, G51, B15)
                new Color32(colors.GetRed(246), colors.GetGreen(246), colors.GetBlue(246), 255),  //graveyards (R147, G15, B7)
                new Color32(colors.GetRed(0), colors.GetGreen(0), colors.GetBlue(0), 255),        //coven (R15, G15, B15)
                new Color32(colors.GetRed(53), colors.GetGreen(53), colors.GetBlue(53), 255),     //farms (R165, G100, B70)
                new Color32(colors.GetRed(51), colors.GetGreen(51), colors.GetBlue(51), 255),     //wealthy (R193, G133, B100)
                new Color32(colors.GetRed(55), colors.GetGreen(55), colors.GetBlue(55), 255),     //poor (R140, G86, B55)
                new Color32(colors.GetRed(96), colors.GetGreen(96), colors.GetBlue(96), 255),     //temple (R176, G205, B255)
                new Color32(colors.GetRed(101), colors.GetGreen(101), colors.GetBlue(101), 255),  //cult (R68, G124, B192)
                new Color32(colors.GetRed(39), colors.GetGreen(39), colors.GetBlue(39), 255),     //tavern (R126, G81, B89)
                new Color32(colors.GetRed(33), colors.GetGreen(33), colors.GetBlue(33), 255),     //city (R220, G177, B177)
                new Color32(colors.GetRed(35), colors.GetGreen(35), colors.GetBlue(35), 255),     //hamlet (R188, G138, B138)
                new Color32(colors.GetRed(37), colors.GetGreen(37), colors.GetBlue(37), 255),     //village (R155, G105, B106)
            };

            identifyFlashColor = new Color32(colors.GetRed(244), colors.GetGreen(244), colors.GetBlue(244), 255); // (R163, G39, B15)

            // Populate the offset dict
            PopulateRegionOffsetDict();

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

            // Location dots overlay panel
            if (DaggerfallUnity.Settings.TravelMapLocationsOutline)
            {
                regionLocationDotsOutlinesOverlayPanel = new Panel[outlineDisplacements.Length];
                for (int i = 0; i < outlineDisplacements.Length; i++)
                {
                    Rect modifedPanelRect = regionTextureOverlayPanelRect;
                    modifedPanelRect.x += outlineDisplacements[i].x * dotsOutlineThickness / NativePanel.LocalScale.x;
                    modifedPanelRect.y += outlineDisplacements[i].y * dotsOutlineThickness / NativePanel.LocalScale.y;
                    regionLocationDotsOutlinesOverlayPanel[i] = DaggerfallUI.AddPanel(modifedPanelRect, NativePanel);
                    regionLocationDotsOutlinesOverlayPanel[i].Enabled = false;
                }
            }
            regionLocationDotsOverlayPanel = DaggerfallUI.AddPanel(regionTextureOverlayPanelRect, NativePanel);
            regionLocationDotsOverlayPanel.Enabled = false;

            // Current region overlay panel
            playerRegionOverlayPanel = DaggerfallUI.AddPanel(playerRegionOverlayPanelRect, NativePanel);
            playerRegionOverlayPanel.Enabled = false;

            // Overlay for the region panel
            identifyOverlayPanel = DaggerfallUI.AddPanel(regionTextureOverlayPanelRect, NativePanel);
            identifyOverlayPanel.Enabled = false;

            // Borders around the region maps
            borderTexture = DaggerfallUI.GetTextureFromImg(regionBorderImgName);
            borderPanel = DaggerfallUI.AddPanel(new Rect(new Vector2(0, regionTextureOverlayPanelRect.position.y), regionTextureOverlayPanelRect.size), NativePanel);
            borderPanel.BackgroundTexture = borderTexture;
            borderPanel.Enabled = false;

            // Load native overworld texture
            overworldTexture = ImageReader.GetTexture(overworldImgName);
            NativePanel.BackgroundTexture = overworldTexture;

            // Setup pixel buffer and texture for region/location identify
            identifyPixelBuffer = new Color32[(int)regionTextureOverlayPanelRect.width * (int)regionTextureOverlayPanelRect.height];
            identifyTexture = new Texture2D((int)regionTextureOverlayPanelRect.width, (int)regionTextureOverlayPanelRect.height, TextureFormat.ARGB32, false);
            identifyTexture.filterMode = FilterMode.Point;

            // Setup pixel buffer and texture for location dots overlay
            locationDotsOutlinePixelBuffer = new Color32[(int)regionTextureOverlayPanelRect.width * (int)regionTextureOverlayPanelRect.height];
            locationDotsPixelBuffer = new Color32[(int)regionTextureOverlayPanelRect.width * (int)regionTextureOverlayPanelRect.height];
            locationDotsOutlineTexture = new Texture2D((int)regionTextureOverlayPanelRect.width, (int)regionTextureOverlayPanelRect.height, TextureFormat.ARGB32, false);
            locationDotsOutlineTexture.filterMode = FilterMode.Point;
            locationDotsTexture = new Texture2D((int)regionTextureOverlayPanelRect.width, (int)regionTextureOverlayPanelRect.height, TextureFormat.ARGB32, false);
            locationDotsTexture.filterMode = FilterMode.Point;

            // Load map names for player region
            selectedRegionMapNames = GetRegionMapNames(GetPlayerRegion());

            // Identify current region
            StartIdentify();
            UpdateIdentifyTextureForPlayerRegion();
        }

        public override void OnPush()
        {
            base.OnPush();

            toggleClosedBinding = InputManager.Instance.GetBinding(InputManager.Actions.TravelMap);

            if (IsSetup)
            {
                StartIdentify();
                UpdateIdentifyTextureForPlayerRegion();
                CloseRegionPanel();
            }

        }

        public override void OnPop()
        {
            base.OnPop();
            teleportationTravel = false;
            findingLocation = false;
            gotoPlace = null;
            distanceRegionName = null;
            distance = null;
        }

        public override void Update()
        {
            base.Update();

            // Toggle window closed with same hotkey used to open it
            if (InputManager.Instance.GetKeyUp(toggleClosedBinding) || InputManager.Instance.GetBackButtonUp())
            {
                if (RegionSelected)
                    CloseRegionPanel();
                else
                    CloseWindow();
            }

            // Input handling
            HotkeySequence.KeyModifiers keyModifiers = HotkeySequence.GetKeyboardKeyModifiers();
            Vector2 currentMousePos = new Vector2((NativePanel.ScaledMousePosition.x), (NativePanel.ScaledMousePosition.y));

            if (currentMousePos != lastMousePos)
            {
                lastMousePos = currentMousePos;
                if (RegionSelected == true)
                    UpdateMouseOverLocation();
                else
                    UpdateMouseOverRegion();
            }

            UpdateRegionLabel();

            if (RegionSelected)
            {
                if (InputManager.Instance.GetMouseButtonUp(1))
                {
                    // Zoom to mouse position
                    zoomPosition = currentMousePos;
                    zoom = !zoom;
                    ZoomMapTextures();
                }
                else if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && zoom && NativePanel.MouseOverComponent)
                {
                    // Scrolling while zoomed in
                    zoomPosition = currentMousePos;
                    ZoomMapTextures();
                }
                if (DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.TravelMapList).IsUpWith(keyModifiers))
                {

                    if (!RegionSelected || currentDFRegion.LocationCount < 1)
                        return;

                    string[] locations = GetCurrentRegionLocalizedMapNames().OrderBy(p => p).ToArray();
                    ShowLocationPicker(locations, true);
                }
                else if (DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.TravelMapFind).IsUpWith(keyModifiers))
                    FindlocationButtonClickHandler(null, Vector2.zero);
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                {
                    if (identifying)
                        OpenRegionPanel(GetPlayerRegion());
                }
            }

            // Show/hide identify panel when identify is running
            identifyOverlayPanel.Enabled = identifying && identifyState;
            AnimateIdentify();

            // If a goto location specified, find it and ask if player wants to travel.
            if (gotoPlace != null)
            {
                // Get localized name for search with fallback to canonical name
                string localizedGotoPlaceName = TextManager.Instance.GetLocalizedLocationName(gotoPlace.SiteDetails.mapId, gotoPlace.SiteDetails.locationName);

                // Open region and search for localizedGotoPlaceName
                mouseOverRegion = MapsFile.PatchRegionIndex(gotoPlace.SiteDetails.regionIndex, gotoPlace.SiteDetails.regionName);
                OpenRegionPanel(mouseOverRegion);
                UpdateRegionLabel();
                HandleLocationFindEvent(null, localizedGotoPlaceName);
                gotoPlace = null;
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
            findButton = DaggerfallUI.AddButton(new Rect(3, 175, findButtonRect.width, findButtonRect.height), NativePanel);
            findButton.BackgroundTexture = findButtonTexture;
            findButton.OnMouseClick += FindlocationButtonClickHandler;
            findButton.Enabled = false;

            // I'm At button
            atButton = DaggerfallUI.AddButton(new Rect(3, 186, atButtonRect.width, atButtonRect.height), NativePanel);
            atButton.BackgroundTexture = atButtonTexture;
            atButton.OnMouseClick += AtButtonClickHandler;

            // Dungeons filter button
            dungeonsFilterButton.Position = new Vector2(50, 175);
            dungeonsFilterButton.Size = new Vector2(dungeonsFilterButtonSrcRect.width, dungeonsFilterButtonSrcRect.height);
            dungeonsFilterButton.Name = "dungeonsFilterButton";
            dungeonsFilterButton.OnMouseClick += FilterButtonClickHandler;
            NativePanel.Components.Add(dungeonsFilterButton);

            // Temples filter button
            templesFilterButton.Position = new Vector2(50, 186);
            templesFilterButton.Size = new Vector2(templesFilterButtonSrcRect.width, templesFilterButtonSrcRect.height);
            templesFilterButton.Name = "templesFilterButton";
            templesFilterButton.OnMouseClick += FilterButtonClickHandler;
            NativePanel.Components.Add(templesFilterButton);

            // Homes filter button
            homesFilterButton.Position = new Vector2(149, 175);
            homesFilterButton.Size = new Vector2(homesFilterButtonSrcRect.width, homesFilterButtonSrcRect.height);
            homesFilterButton.Name = "homesFilterButton";
            homesFilterButton.OnMouseClick += FilterButtonClickHandler;
            NativePanel.Components.Add(homesFilterButton);

            // Towns filter button
            townsFilterButton.Position = new Vector2(149, 186);
            townsFilterButton.Size = new Vector2(townsFilterButtonSrcRect.width, townsFilterButtonSrcRect.height);
            townsFilterButton.Name = "townsFilterButton";
            townsFilterButton.OnMouseClick += FilterButtonClickHandler;
            NativePanel.Components.Add(townsFilterButton);

            // Horizontal arrow button
            horizontalArrowButton.Position = new Vector2(231, 176);
            horizontalArrowButton.Size = new Vector2(22, 20);
            horizontalArrowButton.Enabled = false;
            NativePanel.Components.Add(horizontalArrowButton);
            horizontalArrowButton.Name = "horizontalArrowButton";
            horizontalArrowButton.OnMouseClick += ArrowButtonClickHandler;

            // Vertical arrow button
            verticalArrowButton.Position = new Vector2(254, 176);
            verticalArrowButton.Size = new Vector2(22, 20);
            verticalArrowButton.Enabled = false;
            NativePanel.Components.Add(verticalArrowButton);
            verticalArrowButton.Name = "verticalArrowButton";
            verticalArrowButton.OnMouseClick += ArrowButtonClickHandler;

            // Store toggle closed binding for this window
            toggleClosedBinding = InputManager.Instance.GetBinding(InputManager.Actions.TravelMap);

        }

        protected virtual void SetupArrowButtons()
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
            Texture2D baselocationFilterButtonEnabledText = ImageReader.GetTexture(locationFilterButtonEnabledImgName);
            Texture2D baselocationFilterButtonDisabledText = ImageReader.GetTexture(locationFilterButtonDisabledImgName);
            DFSize baseSize = new DFSize(179, 22);

            // Dungeons toggle button
            dungeonFilterButtonEnabled = ImageReader.GetSubTexture(baselocationFilterButtonEnabledText, dungeonsFilterButtonSrcRect, baseSize);
            dungeonFilterButtonDisabled = ImageReader.GetSubTexture(baselocationFilterButtonDisabledText, dungeonsFilterButtonSrcRect, baseSize);

            // Dungeons toggle button
            templesFilterButtonEnabled = ImageReader.GetSubTexture(baselocationFilterButtonEnabledText, templesFilterButtonSrcRect, baseSize);
            templesFilterButtonDisabled = ImageReader.GetSubTexture(baselocationFilterButtonDisabledText, templesFilterButtonSrcRect, baseSize);

            // Homes toggle button
            homesFilterButtonEnabled = ImageReader.GetSubTexture(baselocationFilterButtonEnabledText, homesFilterButtonSrcRect, baseSize);
            homesFilterButtonDisabled = ImageReader.GetSubTexture(baselocationFilterButtonDisabledText, homesFilterButtonSrcRect, baseSize);

            // Towns toggle button
            townsFilterButtonEnabled = ImageReader.GetSubTexture(baselocationFilterButtonEnabledText, townsFilterButtonSrcRect, baseSize);
            townsFilterButtonDisabled = ImageReader.GetSubTexture(baselocationFilterButtonDisabledText, townsFilterButtonSrcRect, baseSize);

            DFSize buttonsFullSize = new DFSize(45, 22);

            findButtonTexture = ImageReader.GetTexture(findAtButtonImgName);
            findButtonTexture = ImageReader.GetSubTexture(findButtonTexture, findButtonRect, buttonsFullSize);

            atButtonTexture = ImageReader.GetTexture(findAtButtonImgName);
            atButtonTexture = ImageReader.GetSubTexture(atButtonTexture, atButtonRect, buttonsFullSize);

            // Arrows
            upArrowTexture = ImageReader.GetTexture(upArrowImgName);
            downArrowTexture = ImageReader.GetTexture(downArrowImgName);
            leftArrowTexture = ImageReader.GetTexture(leftArrowImgName);
            rightArrowTexture = ImageReader.GetTexture(rightArrowImgName);
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
            offsetLookup.Add("FMAP0I19.IMG", new Vector2(80, 123));     // Betony scale different
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

        #region Map Texture Management

        // Called when a region is selected
        protected virtual void UpdateMapTextures()
        {
            // Region must be selected
            if (!RegionSelected)
                return;

            // Cached region texture if not available
            string mapName = selectedRegionMapNames[mapIndex];
            if (!regionTextures.ContainsKey(mapName))
            {
                Texture2D regionTextureOut;
                if (!TextureReplacement.TryImportImage(selectedRegionMapNames[mapIndex], false, out regionTextureOut))
                    regionTextureOut = ImageReader.GetTexture(mapName);
                regionTextures.Add(mapName, regionTextureOut);
            }

            // Present region and locations
            regionTextureOverlayPanel.BackgroundTexture = regionTextures[mapName];
            UpdateMapLocationDotsTexture();
        }

        // Updates location dots
        protected virtual void UpdateMapLocationDotsTexture()
        {
            // Get map and dimensions
            string mapName = selectedRegionMapNames[mapIndex];
            Vector2 origin = offsetLookup[mapName];
            int originX = (int)origin.x;
            int originY = (int)origin.y;
            int width = (int)regionTextureOverlayPanelRect.width;
            int height = (int)regionTextureOverlayPanelRect.height;

            // Plot locations to color array
            scale = GetRegionMapScale(selectedRegion);
            Array.Clear(locationDotsPixelBuffer, 0, locationDotsPixelBuffer.Length);
            Array.Clear(locationDotsOutlinePixelBuffer, 0, locationDotsOutlinePixelBuffer.Length);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int offset = (int)((((height - y - 1) * width) + x) * scale);
                    if (offset >= (width * height))
                        continue;
                    int sampleRegion = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetPoliticIndex(originX + x, originY + y) - 128;

                    // Set location pixel if inside region area
                    if (sampleRegion == selectedRegion)
                    {
                        ContentReader.MapSummary summary;
                        if (DaggerfallUnity.Instance.ContentReader.HasLocation(originX + x, originY + y, out summary))
                        {
                            if (!checkLocationDiscovered(summary))
                                continue;

                            int index = GetPixelColorIndex(summary.LocationType);
                            if (index == -1)
                                continue;
                            else
                            {
                                if (DaggerfallUnity.Settings.TravelMapLocationsOutline)
                                    locationDotsOutlinePixelBuffer[offset] = dotOutlineColor;
                                locationDotsPixelBuffer[offset] = locationPixelColors[index];
                            }
                        }
                    }
                }
            }

            // Apply updated color array to texture
            if (DaggerfallUnity.Settings.TravelMapLocationsOutline)
            {
                locationDotsOutlineTexture.SetPixels32(locationDotsOutlinePixelBuffer);
                locationDotsOutlineTexture.Apply();
            }
            locationDotsTexture.SetPixels32(locationDotsPixelBuffer);
            locationDotsTexture.Apply();

            // Present texture
            if (DaggerfallUnity.Settings.TravelMapLocationsOutline)
                for (int i = 0; i < outlineDisplacements.Length; i++)
                    regionLocationDotsOutlinesOverlayPanel[i].BackgroundTexture = locationDotsOutlineTexture;
            regionLocationDotsOverlayPanel.BackgroundTexture = locationDotsTexture;
        }

        // Zoom and pan region texture
        protected virtual void ZoomMapTextures()
        {
            // Exit cropped rendering
            if (!RegionSelected || !zoom)
            {
                regionTextureOverlayPanel.BackgroundTextureLayout = BackgroundLayout.StretchToFill;
                if (DaggerfallUnity.Settings.TravelMapLocationsOutline)
                    for (int i = 0; i < outlineDisplacements.Length; i++)
                        regionLocationDotsOutlinesOverlayPanel[i].BackgroundTextureLayout = BackgroundLayout.StretchToFill;
                regionLocationDotsOverlayPanel.BackgroundTextureLayout = BackgroundLayout.StretchToFill;
                identifyOverlayPanel.BackgroundTextureLayout = BackgroundLayout.StretchToFill;
                UpdateBorder();
                return;
            }

            // Get current region texture
            Texture2D regionTexture;
            if (!regionTextures.TryGetValue(selectedRegionMapNames[mapIndex], out regionTexture))
                return;

            // Centre cropped porition over mouse using classic dimensions
            int width = (int)regionTextureOverlayPanelRect.width;
            int height = (int)regionTextureOverlayPanelRect.height;
            int zoomWidth = width / (zoomfactor * 2);
            int zoomHeight = height / (zoomfactor * 2);
            int startX = (int)zoomPosition.x - zoomWidth;
            int startY = (int)(height + (-zoomPosition.y - zoomHeight)) + regionPanelOffset;

            // Clamp to edges
            if (startX < 0)
                startX = 0;
            else if (startX + width / zoomfactor >= width)
                startX = width - width / zoomfactor;
            if (startY < 0)
                startY = 0;
            else if (startY + height / zoomfactor >= height)
                startY = height - height / zoomfactor;

            zoomOffset = new Vector2(startX, startY);

            // Set cropped area in region texture - can be a replacement texture so need to determine ratio compared to classic
            float ratioX = regionTexture.width / (float)width;
            float ratioY = regionTexture.height / (float)height;
            regionTextureOverlayPanel.BackgroundTextureLayout = BackgroundLayout.Cropped;
            regionTextureOverlayPanel.BackgroundCroppedRect = new Rect(startX * ratioX, startY * ratioY, width / zoomfactor * ratioX, height / zoomfactor * ratioY);

            // Set cropped area in location dots panel - always at classic dimensions            
            Rect locationDotsNewRect = new Rect(startX, startY, width / zoomfactor, height / zoomfactor);
            if (DaggerfallUnity.Settings.TravelMapLocationsOutline)
                for (int i = 0; i < outlineDisplacements.Length; i++)
                {
                    Rect modifiedRect = locationDotsNewRect;
                    modifiedRect.x += outlineDisplacements[i].x * dotsOutlineThickness / NativePanel.LocalScale.x;
                    modifiedRect.y += outlineDisplacements[i].y * dotsOutlineThickness / NativePanel.LocalScale.y;
                    regionLocationDotsOutlinesOverlayPanel[i].BackgroundTextureLayout = BackgroundLayout.Cropped;
                    regionLocationDotsOutlinesOverlayPanel[i].BackgroundCroppedRect = modifiedRect;
                }
            regionLocationDotsOverlayPanel.BackgroundTextureLayout = BackgroundLayout.Cropped;
            regionLocationDotsOverlayPanel.BackgroundCroppedRect = locationDotsNewRect;

            // Set cropped area in identify panel - always at classic dimensions
            // This ensures zoomed crosshair pans with location dots panel
            identifyOverlayPanel.BackgroundTextureLayout = BackgroundLayout.Cropped;
            identifyOverlayPanel.BackgroundCroppedRect = regionLocationDotsOverlayPanel.BackgroundCroppedRect;

            UpdateBorder();
        }

        // Show/hide map borders based on state
        protected virtual void UpdateBorder()
        {
            borderPanel.Enabled = (RegionSelected && !zoom);
        }

        // Set region block for identify overlay
        protected virtual void UpdateIdentifyTextureForPlayerRegion()
        {
            // Only for overworld map
            if (RegionSelected)
                return;

            // Player must be inside a valid region
            int playerRegion = GetPlayerRegion();
            if (playerRegion == -1)
                return;

            // Clear existing pixel buffer
            Array.Clear(identifyPixelBuffer, 0, identifyPixelBuffer.Length);

            // Import custom map overlays named TRAV0I00.IMG-RegionName (e.g. TRAV0I00.IMG-Ilessan Hills) if available
            // Custom image must be based on 320x160 interior snip of TRAV0I00.IMG (so exclude top and bottom bars) but can be a higher resolution like 1600x800
            Texture2D customRegionOverlayTexture;
            if (importedOverlays.TryGetValue(playerRegion, out customRegionOverlayTexture) ||
                TextureReplacement.TryImportImage(string.Format("{0}-{1}", overworldImgName, GetRegionNameForMapReplacement(playerRegion)), false, out customRegionOverlayTexture))
            {
                identifyOverlayPanel.BackgroundTexture = importedOverlays[playerRegion] = customRegionOverlayTexture;
                return;
            }

            // Region shape is filled from picker bitmap, so this has to be open
            if (regionPickerBitmap == null)
                regionPickerBitmap = DaggerfallUI.GetImgBitmap(regionPickerImgName);

            // Create a texture overlay for the region area
            int width = regionPickerBitmap.Width;
            int height = regionPickerBitmap.Height;
            int pickerOverlayPanelHeightDifference = height - (int)regionTextureOverlayPanelRect.height - regionPanelOffset + 1;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int srcOffset = y * width + x;
                    int dstOffset = ((height - y - pickerOverlayPanelHeightDifference) * width) + x;
                    int sampleRegion = regionPickerBitmap.Data[srcOffset] - 128;
                    if (sampleRegion == playerRegion)
                        identifyPixelBuffer[dstOffset] = identifyFlashColor;
                }
            }
            identifyTexture.SetPixels32(identifyPixelBuffer);
            identifyTexture.Apply();
            identifyOverlayPanel.BackgroundTexture = identifyTexture;
        }

        protected virtual void UpdateCrosshair()
        {
            if (FindingLocation)
                UpdateIdentifyTextureForPosition(MapsFile.GetPixelFromPixelID(locationSummary.ID), locationSummary.RegionIndex);
            else
                UpdateIdentifyTextureForPosition(TravelTimeCalculator.GetPlayerTravelPosition(), selectedRegion);
        }

        protected virtual void UpdateIdentifyTextureForPosition(DFPosition pos, int regionIndex = -1)
        {
            if (regionIndex == -1)
                regionIndex = GetPlayerRegion();
            UpdateIdentifyTextureForPosition(pos.X, pos.Y, regionIndex);
        }

        // Set location crosshair for identify overlay
        protected virtual void UpdateIdentifyTextureForPosition(int mapPixelX, int mapPixelY, int regionIndex)
        {
            // Only for regions
            if (!RegionSelected)
                return;

            // Clear existing pixel buffer
            Array.Clear(identifyPixelBuffer, 0, identifyPixelBuffer.Length);

            string mapName = selectedRegionMapNames[mapIndex];
            Vector2 origin = offsetLookup[mapName];
            float scale = GetRegionMapScale(regionIndex);

            // Manually adjust Betony vertical offset
            int yAdjust = 0;
            if (regionIndex == betonyIndex)
                yAdjust = -477;

            int scaledX = (int)((mapPixelX - origin.x) * scale);
            int scaledY = (int)((mapPixelY - origin.y) * scale) + regionPanelOffset + yAdjust;

            int width = (int)regionTextureOverlayPanelRect.width;
            int height = (int)regionTextureOverlayPanelRect.height;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (x == scaledX || y + regionPanelOffset == scaledY)
                    {
                        identifyPixelBuffer[(height - y - 1) * width + x] = identifyFlashColor;
                    }
                }
            }
            identifyTexture.SetPixels32(identifyPixelBuffer);
            identifyTexture.Apply();
            identifyOverlayPanel.BackgroundTexture = identifyTexture;
        }

        #endregion

        #region Event Handlers

        // Handle clicks on the main panel
        protected virtual void ClickHandler(BaseScreenComponent sender, Vector2 position)
        {
            position.y -= regionPanelOffset;

            // Ensure clicks are inside region texture
            if (position.x < 0 || position.x > regionTextureOverlayPanelRect.width || position.y < 0 || position.y > regionTextureOverlayPanelRect.height)
                return;

            if (RegionSelected == false)
            {
                if (MouseOverRegion)
                    OpenRegionPanel(mouseOverRegion);
            }
            else if (locationSelected)
            {
                if (FindingLocation)
                    StopIdentify(true);
                else
                    CreatePopUpWindow();
            }
            else if (MouseOverOtherRegion)
            {
                // If clicked while mouse over other region & not a location, switch to that region
                OpenRegionPanel(mouseOverRegion);
            }
        }

        protected virtual void ExitButtonClickHandler(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            CloseTravelWindows();
        }

        protected virtual void AtButtonClickHandler(BaseScreenComponent sender, Vector2 position)
        {
            // Identify region or map location
            findingLocation = false;
            StartIdentify();
            UpdateCrosshair();
        }

        protected virtual void FindlocationButtonClickHandler(BaseScreenComponent sender, Vector2 position)
        {
            // Open find location pop-up
            if (RegionSelected)
            {
                DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
                DaggerfallInputMessageBox findPopUp = new DaggerfallInputMessageBox(uiManager, null, 31, TextManager.Instance.GetLocalizedText("findLocationPrompt"), true, this);
                findPopUp.TextPanelDistanceY = 5;
                findPopUp.TextBox.WidthOverride = 308;
                findPopUp.TextBox.MaxCharacters = 32;
                findPopUp.OnGotUserInput += HandleLocationFindEvent;
                findPopUp.Show();
            }
        }

        /// <summary>
        /// Button handler for travel confirmation pop up. This is a temporary solution until implementing the final pop-up.
        /// </summary>
        protected virtual void ConfirmTravelPopupButtonClick(DaggerfallMessageBox sender, DaggerfallMessageBox.MessageBoxButtons messageBoxButton)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            sender.CloseWindow();
            if (messageBoxButton == DaggerfallMessageBox.MessageBoxButtons.Yes)
                CreatePopUpWindow();
            else
                StopIdentify();
        }

        /// <summary>
        /// Handles click events for the arrow buttons in the region view
        /// </summary>
        protected virtual void ArrowButtonClickHandler(BaseScreenComponent sender, Vector2 position)
        {
            if (RegionSelected == false || !HasMultipleMaps)
                return;
            int newIndex = mapIndex;

            if (sender.Name == "horizontalArrowButton")
            {
                if (newIndex % 2 == 0)
                    newIndex += 1;          // Move right
                else
                    newIndex -= 1;          // Move left
            }
            else if (sender.Name == "verticalArrowButton")
            {
                if (newIndex > 1)
                    newIndex -= 2;          // Move up
                else
                    newIndex += 2;          // Move down
            }
            else
            {
                return;
            }

            mapIndex = newIndex;
            SetupArrowButtons();
            UpdateMapTextures();
            UpdateCrosshair();
        }

        /// <summary>
        /// Handles click events for the filter buttons in the region view
        /// </summary>
        protected virtual void FilterButtonClickHandler(BaseScreenComponent sender, Vector2 position)
        {
            if (sender.Name == "dungeonsFilterButton")
            {
                filterDungeons = !filterDungeons;
            }
            else if (sender.Name == "templesFilterButton")
            {
                filterTemples = !filterTemples;
            }
            else if (sender.Name == "homesFilterButton")
            {
                filterHomes = !filterHomes;
            }
            else if (sender.Name == "townsFilterButton")
            {
                filterTowns = !filterTowns;
            }
            else
            {
                return;
            }

            if (filterDungeons)
                dungeonsFilterButton.BackgroundTexture = dungeonFilterButtonDisabled;
            else
                dungeonsFilterButton.BackgroundTexture = dungeonFilterButtonEnabled;
            if (filterTemples)
                templesFilterButton.BackgroundTexture = templesFilterButtonDisabled;
            else
                templesFilterButton.BackgroundTexture = templesFilterButtonEnabled;
            if (filterHomes)
                homesFilterButton.BackgroundTexture = homesFilterButtonDisabled;
            else
                homesFilterButton.BackgroundTexture = homesFilterButtonEnabled;
            if (filterTowns)
                townsFilterButton.BackgroundTexture = townsFilterButtonDisabled;
            else
                townsFilterButton.BackgroundTexture = townsFilterButtonEnabled;

            UpdateMapLocationDotsTexture();
        }

        #endregion

        #region Private Methods

        // Set selected region and open region panel
        protected virtual void OpenRegionPanel(int region)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            string[] mapNames = GetRegionMapNames(region);
            if (mapNames == null || mapNames.Length == 0)
                return;

            mapIndex = 0;
            selectedRegion = region;
            selectedRegionMapNames = mapNames;
            regionTextureOverlayPanel.Enabled = true;
            if (DaggerfallUnity.Settings.TravelMapLocationsOutline)
                for (int i = 0; i < outlineDisplacements.Length; i++)
                    regionLocationDotsOutlinesOverlayPanel[i].Enabled = true;
            regionLocationDotsOverlayPanel.Enabled = true;
            findButton.Enabled = true;
            findingLocation = false;
            currentDFRegion = DaggerfallUnity.ContentReader.MapFileReader.GetRegion(region);
            currentDFRegionIndex = region;
            lastQueryLocationIndex = -1;
            SetupArrowButtons();
            UpdateMapTextures();
            UpdateBorder();
            StartIdentify();
            UpdateCrosshair();
        }

        // Close region panel and reset values
        protected virtual void CloseRegionPanel()
        {
            selectedRegion = -1;
            mouseOverRegion = -1;
            locationSelected = false;
            mapIndex = 0;
            regionTextureOverlayPanel.Enabled = false;
            if (DaggerfallUnity.Settings.TravelMapLocationsOutline)
                for (int i = 0; i < outlineDisplacements.Length; i++)
                    regionLocationDotsOutlinesOverlayPanel[i].Enabled = false;
            regionLocationDotsOverlayPanel.Enabled = false;
            horizontalArrowButton.Enabled = false;
            verticalArrowButton.Enabled = false;
            findButton.Enabled = false;
            zoom = false;
            ZoomMapTextures();
            StartIdentify();
            UpdateIdentifyTextureForPlayerRegion();
        }

        // Check if location with MapSummary summary is already discovered
        protected virtual bool checkLocationDiscovered(ContentReader.MapSummary summary)
        {
            // Check location MapTableData.Discovered flag in world replacement data then cached MAPS.BSA data
            bool discovered = false;
            DFLocation location;
            if (WorldDataReplacement.GetDFLocationReplacementData(summary.RegionIndex, summary.MapIndex, out location))
                discovered = location.MapTableData.Discovered;
            else
                discovered = summary.Discovered;

            return GameManager.Instance.PlayerGPS.HasDiscoveredLocation(summary.ID) || discovered || revealUndiscoveredLocations == true;
        }

        // Check if place is discovered, so it can be found on map.
        public bool CanFindPlace(string regionName, string name)
        {
            DFLocation location;
            if (DaggerfallUnity.Instance.ContentReader.GetLocation(regionName, name, out location))
            {
                DFPosition mapPixel = MapsFile.LongitudeLatitudeToMapPixel(location.MapTableData.Longitude, location.MapTableData.Latitude);
                ContentReader.MapSummary summary;
                if (DaggerfallUnity.Instance.ContentReader.HasLocation(mapPixel.X, mapPixel.Y, out summary))
                    return checkLocationDiscovered(summary);
            }
            return false;
        }

        protected Vector2 GetCoordinates()
        {
            string mapName = selectedRegionMapNames[mapIndex];
            Vector2 origin = offsetLookup[mapName];
            int height = (int)regionTextureOverlayPanelRect.height;

            Vector2 results = Vector2.zero;
            Vector2 pos = regionTextureOverlayPanel.ScaledMousePosition;

            if (zoom)
            {
                results.x = (int)Math.Floor(pos.x / zoomfactor + zoomOffset.x + origin.x);
                float diffy = height / zoomfactor - pos.y;
                results.y = (int)Math.Floor(height - pos.y / zoomfactor - zoomOffset.y - diffy + origin.y);
            }
            else
            {
                results.x = (int)Math.Floor(origin.x + pos.x);
                results.y = (int)Math.Floor(origin.y + pos.y);
            }

            return results;
        }


        // Check if player mouse over valid location while region selected & not finding location
        protected virtual void UpdateMouseOverLocation()
        {
            if (RegionSelected == false || FindingLocation)
                return;

            locationSelected = false;
            mouseOverRegion = selectedRegion;

            if (lastMousePos.x < 0 ||
                lastMousePos.x > regionTextureOverlayPanelRect.width ||
                lastMousePos.y < regionPanelOffset ||
                lastMousePos.y > regionTextureOverlayPanel.Size.y + regionPanelOffset)
                return;

            float scale = GetRegionMapScale(selectedRegion);
            Vector2 coordinates = GetCoordinates();
            int x = (int)(coordinates.x / scale);
            int y = (int)(coordinates.y / scale);

            if (selectedRegion == betonyIndex)      // Manually correct Betony offset
            {
                x += 60;
                y += 212;
            }

            if (selectedRegion == 61)               // Fix for Cybiades zoom-in map. Map is more zoomed in than for other regions but the pixel coordinates are not scaled to match.
                                                    // The upper right corner of Cybiades (about x=440 y=340) is the same for both Cybiades's zoomed-in map and Sentinel's less zoomed in map,
                                                    // so that is being used as the base for this fix.
            {
                int xDiff = x - 440;
                int yDiff = y - 340;
                xDiff /= 4;
                yDiff /= 4;
                x = 440 + xDiff;
                y = 340 + yDiff;
            }

            int sampleRegion = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetPoliticIndex(x, y) - 128;

            if (sampleRegion != selectedRegion && sampleRegion >= 0 && sampleRegion < DaggerfallUnity.Instance.ContentReader.MapFileReader.RegionCount)
            {
                mouseOverRegion = sampleRegion;
                return;
            }

            if (DaggerfallUnity.ContentReader.HasLocation(x, y) && !FindingLocation)
            {
                DaggerfallUnity.ContentReader.HasLocation(x, y, out locationSummary);

                if (locationSummary.MapIndex < 0 || locationSummary.MapIndex >= currentDFRegion.MapNames.Length)
                    return;
                else
                {
                    int index = GetPixelColorIndex(locationSummary.LocationType);
                    if (index == -1)
                        return;

                    // Only make location selectable if it is already discovered
                    if (!checkLocationDiscovered(locationSummary))
                        return;

                    locationSelected = true;
                }
            }
        }

        // Check if mouse over a region
        protected virtual void UpdateMouseOverRegion()
        {
            mouseOverRegion = -1;

            int x = 0;
            int y = 0;

            if (zoom)
            {
                var zoomCoords = GetCoordinates();
                x = (int)zoomCoords.x;
                y = (int)zoomCoords.y;
            }
            else
            {
                x = (int)lastMousePos.x;
                y = (int)lastMousePos.y;
            }

            // Get offset into region picker bitmap
            int offset = y * regionPickerBitmap.Width + x;
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
        protected virtual void UpdateRegionLabel()
        {
            if (RegionSelected == false)
                regionLabel.Text = GetRegionName(mouseOverRegion);
            else if (locationSelected)
                regionLabel.Text = string.Format("{0} : {1}", GetRegionName(mouseOverRegion), GetLocationNameInCurrentRegion(locationSummary.MapIndex, true));
            else if (MouseOverOtherRegion)
                regionLabel.Text = string.Format(TextManager.Instance.GetLocalizedText("switchToRegion"), GetRegionName(mouseOverRegion));
            else
                regionLabel.Text = GetRegionName(mouseOverRegion);
        }

        // Closes windows based on context
        public void CloseTravelWindows(bool forceClose = false)
        {
            if (RegionSelected == false || forceClose)
                CloseWindow();
            else
                CloseRegionPanel();
        }

        // Updates search button toggle state based on current flags
        protected virtual void UpdateSearchButtons()
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

        public TravelMapSaveData GetTravelMapSaveData()
        {
            TravelMapSaveData data = new TravelMapSaveData();
            data.filterDungeons = filterDungeons;
            data.filterHomes = filterHomes;
            data.filterTemples = filterTemples;
            data.filterTowns = filterTowns;

            if (popUp != null)
            {
                data.sleepInn = popUp.SleepModeInn;
                data.speedCautious = popUp.SpeedCautious;
                data.travelShip = popUp.TravelShip;
            }

            return data;
        }

        public void SetTravelMapFromSaveData(TravelMapSaveData data)
        {
            // If doesn't have save data, use defaults
            if (data == null)
                data = new TravelMapSaveData();

            filterDungeons = data.filterDungeons;
            filterHomes = data.filterHomes;
            filterTemples = data.filterTemples;
            filterTowns = data.filterTowns;

            if (popUp == null)
            {
                popUp = (DaggerfallTravelPopUp)UIWindowFactory.GetInstanceWithArgs(UIWindowType.TravelPopUp, new object[] { uiManager, this, this });
            }

            popUp.SleepModeInn = data.sleepInn;
            popUp.SpeedCautious = data.speedCautious;
            popUp.TravelShip = data.travelShip;

            UpdateSearchButtons();
        }
        #endregion

        #region Helper Methods

        // Get index to locationPixelColor array or -1 if invalid or filtered
        protected virtual int GetPixelColorIndex(DFRegion.LocationTypes locationType)
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
                case DFRegion.LocationTypes.Graveyard:
                    index = 3;
                    break;
                case DFRegion.LocationTypes.Coven:
                    index = 4;
                    break;
                case DFRegion.LocationTypes.HomeFarms:
                    index = 5;
                    break;
                case DFRegion.LocationTypes.HomeWealthy:
                    index = 6;
                    break;
                case DFRegion.LocationTypes.HomePoor:
                    index = 7;
                    break;
                case DFRegion.LocationTypes.HomeYourShips:
                    break;
                case DFRegion.LocationTypes.ReligionTemple:
                    index = 8;
                    break;
                case DFRegion.LocationTypes.ReligionCult:
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
            else if (index < 5 && filterDungeons)
                index = -1;
            else if (index > 4 && index < 8 && filterHomes)
                index = -1;
            else if (index > 7 && index < 10 && filterTemples)
                index = -1;
            else if (index > 9 && index < 14 && filterTowns)
                index = -1;
            return index;
        }

        // Handles events from Find Location pop-up.
        protected virtual void HandleLocationFindEvent(DaggerfallInputMessageBox inputMessageBox, string locationName)
        {
            List<DistanceMatch> matching;
            if (FindLocation(locationName, out matching))
            {
                if (matching.Count == 1)
                { //place flashing crosshair over location
                    locationSelected = true;
                    findingLocation = true;
                    StartIdentify();
                    UpdateCrosshair();
                }
                else
                {
                    ShowLocationPicker(matching.ConvertAll(match => match.text).ToArray(), false);
                }
            }
            else
            {
                TextFile.Token[] textTokens = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(13);
                DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, this);
                messageBox.SetTextTokens(textTokens);
                messageBox.ClickAnywhereToClose = true;
                uiManager.PushWindow(messageBox);
                return;
            }
        }

        // Get localized names of all locations in current region with fallback to canonical name
        // Builds a new name lookup dictionary for this region on every call used to complete search
        protected string[] GetCurrentRegionLocalizedMapNames()
        {
            localizedMapNameLookup.Clear();
            List<string> localizedNames = new List<string>(currentDFRegion.MapNames.Length);
            for (int l = 0; l < currentDFRegion.MapNames.Length; l++)
            {
                // Handle duplicate names in same way as Region.MapNameLookup
                string name = TextManager.Instance.GetLocalizedLocationName(currentDFRegion.MapTable[l].MapId, currentDFRegion.MapNames[l]);
                if (!localizedNames.Contains(name))
                {
                    localizedNames.Add(name);
                    localizedMapNameLookup.Add(name, l);
                }
            }
            return localizedNames.ToArray();
        }

        // Find location by name
        protected virtual bool FindLocation(string name, out List<DistanceMatch> matching)
        {
            matching = new List<DistanceMatch>();
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            if (distanceRegionName != currentDFRegion.Name)
            {
                distanceRegionName = currentDFRegion.Name;
                distance = DaggerfallDistance.GetDistance();
                distance.SetDictionary(GetCurrentRegionLocalizedMapNames());
            }

            DistanceMatch[] bestMatches = distance.FindBestMatches(name, maxMatchingResults);

            // Check if selected locations actually exist/are visible
            MatchesCutOff cutoff = null;
            ContentReader.MapSummary findLocationSummary;

            foreach (DistanceMatch match in bestMatches)
            {
                // Must have called GetCurrentRegionLocalizedMapNames() prior to this point
                if (!localizedMapNameLookup.ContainsKey(match.text))
                {
                    Debug.LogWarningFormat("Error: location name '{0}' key not found in localizedMapNameLookup dictionary for this region.", match.text);
                    continue;
                }
                int index = localizedMapNameLookup[match.text];
                DFRegion.RegionMapTable locationInfo = currentDFRegion.MapTable[index];
                DFPosition pos = MapsFile.LongitudeLatitudeToMapPixel((int)locationInfo.Longitude, (int)locationInfo.Latitude);
                if (DaggerfallUnity.ContentReader.HasLocation(pos.X, pos.Y, out findLocationSummary))
                {
                    // only make location searchable if it is already discovered
                    if (!checkLocationDiscovered(findLocationSummary))
                        continue;

                    if (cutoff == null)
                    {
                        cutoff = new MatchesCutOff(match.relevance);

                        // Set locationSummary to first result's MapSummary in case we skip the location list picker step
                        locationSummary = findLocationSummary;
                    }
                    else
                    {
                        if (!cutoff.Keep(match.relevance))
                            break;
                    }
                    matching.Add(match);
                }
            }

            return matching.Count > 0;
        }

        private class MatchesCutOff
        {
            private readonly float threshold;

            public MatchesCutOff(float bestRelevance)
            {
                // If perfect match exists, return all perfect matches only
                // Normally there should be only one perfect match, but if string canonization generates collisions that's no longer guaranteed
                threshold = bestRelevance == 1f ? 1f : bestRelevance * 0.5f;
            }

            public bool Keep(float relevance)
            {
                return relevance >= threshold;
            }
        }

        // Creates a ListPickerWindow with a list of locations from current region
        // Locations displayed will be filtered out depending on the dungeon / town / temple / home button settings
        private void ShowLocationPicker(string[] locations, bool applyFilters)
        {
            DaggerfallListPickerWindow locationPicker = new DaggerfallListPickerWindow(uiManager, this);
            locationPicker.OnItemPicked += HandleLocationPickEvent;
            locationPicker.ListBox.MaxCharacters = 29;

            for (int i = 0; i < locations.Length; i++)
            {
                if (applyFilters)
                {
                    // Must have called GetCurrentRegionLocalizedMapNames() prior to this point
                    int index = localizedMapNameLookup[locations[i]];
                    if (GetPixelColorIndex(currentDFRegion.MapTable[index].LocationType) == -1)
                        continue;
                }
                locationPicker.ListBox.AddItem(locations[i]);
            }

            uiManager.PushWindow(locationPicker);
        }

        public void HandleLocationPickEvent(int index, string locationName)
        {
            if (!RegionSelected || currentDFRegion.LocationCount < 1)
                return;

            CloseWindow();
            HandleLocationFindEvent(null, locationName);
        }

        // Gets current player region or -1 if player not in any region (e.g. in ocean)
        protected int GetPlayerRegion()
        {
            DFPosition position = TravelTimeCalculator.GetPlayerTravelPosition();
            int region = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetPoliticIndex(position.X, position.Y) - 128;
            if (region < 0 || region >= DaggerfallUnity.Instance.ContentReader.MapFileReader.RegionCount)
                return -1;

            return region;
        }

        // Gets name of region
        protected string GetRegionName(int region)
        {
            return TextManager.Instance.GetLocalizedRegionName(region);
        }
        protected string GetRegionNameForMapReplacement(int region)
        {
            return DaggerfallUnity.Instance.ContentReader.MapFileReader.GetRegionName(region); // Using non-localized name for map replacement path
        }

        // Gets name of location in currently open region - tries world data replacement then falls back to MAPS.BSA
        protected virtual string GetLocationNameInCurrentRegion(int locationIndex, bool cacheName = false)
        {
            // Must have a region open
            if (currentDFRegionIndex == -1)
                return string.Empty;

            // Cache the last location index when requested and only update it when index changes
            if (cacheName && lastQueryLocationIndex == locationIndex)
                return lastQueryLocationName;

            // Localized name has first priority if one exists
            string localizedName = TextManager.Instance.GetLocalizedLocationName(locationSummary.MapID, string.Empty);
            if (!string.IsNullOrEmpty(localizedName))
                return localizedName;

            // Get location name from world data replacement if available or fall back to MAPS.BSA cached names
            DFLocation location;
            if (WorldDataReplacement.GetDFLocationReplacementData(currentDFRegionIndex, locationIndex, out location))
            {
                lastQueryLocationName = location.Name;
                lastQueryLocationIndex = locationIndex;
                return location.Name;
            }
            else
            {
                return currentDFRegion.MapNames[locationSummary.MapIndex];
            }
        }

        // Gets maps for region
        protected string[] GetRegionMapNames(int region)
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
        protected virtual float GetRegionMapScale(int region)
        {
            if (region == betonyIndex)
                return 4f;
            else
                return 1;
        }

        protected virtual void CreateConfirmationPopUp()
        {
            const int doYouWishToTravelToTextId = 31;

            if (!locationSelected)
                return;

            // Get text tokens
            TextFile.Token[] textTokens = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(doYouWishToTravelToTextId);

            // Hack to set location name in text token for now
            textTokens[2].text = textTokens[2].text.Replace(
                "%tcn",
                TextManager.Instance.GetLocalizedLocationName(locationSummary.MapID, GetLocationNameInCurrentRegion(locationSummary.MapIndex)));

            DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, this);
            messageBox.SetTextTokens(textTokens);
            messageBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.Yes);
            messageBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.No);
            messageBox.OnButtonClick += ConfirmTravelPopupButtonClick;
            uiManager.PushWindow(messageBox);
        }

        protected virtual void CreatePopUpWindow()
        {
            DFPosition pos = MapsFile.GetPixelFromPixelID(locationSummary.ID);
            if (teleportationTravel)
            {
                DaggerfallTeleportPopUp telePopup = (DaggerfallTeleportPopUp)UIWindowFactory.GetInstanceWithArgs(UIWindowType.TeleportPopUp, new object[] { uiManager, uiManager.TopWindow, this });
                telePopup.DestinationPos = pos;
                telePopup.DestinationName = GetLocationNameInCurrentRegion(locationSummary.MapIndex);
                uiManager.PushWindow(telePopup);
            }
            else
            {
                if (popUp == null)
                {
                    popUp = (DaggerfallTravelPopUp)UIWindowFactory.GetInstanceWithArgs(UIWindowType.TravelPopUp, new object[] { uiManager, uiManager.TopWindow, this });
                }
                popUp.EndPos = pos;
                uiManager.PushWindow(popUp);
            }
        }

        #endregion


        #region Region Identification

        // Start region identification & location crosshair
        void StartIdentify()
        {
            // Stop animation
            if (identifying)
                StopIdentify(false);

            identifying = true;
            identifyState = false;
            identifyChanges = 0;
            identifyLastChangeTime = 0;
        }

        // Stop region identification & location crosshair
        void StopIdentify(bool createPopUp = true)
        {
            if (FindingLocation && createPopUp)
                CreateConfirmationPopUp();

            identifying = false;
            identifyState = false;
            identifyChanges = 0;
            identifyLastChangeTime = 0;
        }

        // Animate region identification & location crosshair
        void AnimateIdentify()
        {
            if (!identifying)
                return;

            // Check if enough time has elapsed since last flash and toggle state
            bool lastIdentifyState = identifyState;
            float time = Time.realtimeSinceStartup;

            if (time > identifyLastChangeTime + identifyFlashInterval)
            {
                identifyState = !identifyState;
                identifyLastChangeTime = time;
            }

            // Turn off flash after specified number of on states
            if (!lastIdentifyState && identifyState)
            {
                int flashCount = locationSelected ? identifyFlashCountSelected : identifyFlashCount;
                if (++identifyChanges > flashCount)
                {
                    StopIdentify();
                }
            }
        }


        #endregion

        #region console_commands

        public static class TravelMapConsoleCommands
        {
            public static void RegisterCommands()
            {
                try
                {
                    ConsoleCommandsDatabase.RegisterCommand(RevealLocations.name, RevealLocations.description, RevealLocations.usage, RevealLocations.Execute);
                    ConsoleCommandsDatabase.RegisterCommand(HideLocations.name, HideLocations.description, HideLocations.usage, HideLocations.Execute);
                    ConsoleCommandsDatabase.RegisterCommand(RevealLocation.name, RevealLocation.description, RevealLocation.usage, RevealLocation.Execute);
                }
                catch (System.Exception ex)
                {
                    DaggerfallUnity.LogMessage(ex.Message, true);
                }
            }

            private static class RevealLocations
            {
                public static readonly string name = "map_reveallocations";
                public static readonly string description = "Reveals undiscovered locations on travelmap (temporary)";
                public static readonly string usage = "map_reveallocations";


                public static string Execute(params string[] args)
                {
                    if (GameManager.Instance.IsPlayerInside)
                    {
                        return "this command only has an effect when outside";
                    }

                    revealUndiscoveredLocations = true;
                    return "undiscovered locations have been revealed (temporary) on the travelmap";
                }
            }

            private static class HideLocations
            {
                public static readonly string name = "map_hidelocations";
                public static readonly string description = "Hides undiscovered locations on travelmap";
                public static readonly string usage = "map_hidelocations";


                public static string Execute(params string[] args)
                {
                    if (GameManager.Instance.IsPlayerInside)
                    {
                        return "this command only has an effect when outside";
                    }

                    revealUndiscoveredLocations = false;
                    return "undiscovered locations have been hidden on the travelmap again";
                }

            }

            private static class RevealLocation
            {
                public static readonly string name = "map_reveallocation";
                public static readonly string error = "Failed to reveal location with given regionName and locatioName on travelmap";
                public static readonly string description = "Permanently reveals the location with [locationName] in region [regionName] on travelmap";
                public static readonly string usage = "map_reveallocation [regionName] [locationName] - inside the name strings use underscores instead of spaces, e.g Dragontail_Mountains";

                public static string Execute(params string[] args)
                {
                    if (args == null || args.Length < 2)
                    {
                        try
                        {
                            Wenzil.Console.Console.Log("please provide both a region name as well as a location name");
                            return HelpCommand.Execute(RevealLocation.name);
                        }
                        catch
                        {
                            return HelpCommand.Execute(RevealLocation.name);
                        }
                    }
                    else
                    {
                        string regionName = args[0];
                        string locationName = args[1];
                        regionName = regionName.Replace("_", " ");
                        locationName = locationName.Replace("_", " ");
                        try
                        {
                            GameManager.Instance.PlayerGPS.DiscoverLocation(regionName, locationName);
                            return String.Format("revealed location {0} : {1} on the travelmap", regionName, locationName);
                        }
                        catch (Exception ex)
                        {
                            return string.Format("Could not reveal location: {0}", ex.Message);
                        }
                    }
                }
            }
        }

        #endregion
    }
}
