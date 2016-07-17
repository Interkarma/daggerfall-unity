// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2016 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Lypyl (lypyl@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using DaggerfallWorkshop.Game.UserInterface;
using System.Collections.Generic;
using DaggerfallConnect.Utility;


namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{

    public class DaggerfallTravelPopUp : DaggerfallPopupWindow
    {

        #region fields
        DaggerfallTravelMapWindow travelWindow = null;

        const string nativeImgName = "TRAV0I04.IMG";

        public float InnModifier        = .86f;
        public float HorseMod           = .5f;
        public float CartMod            = .75f;
        public float ShipMod            = 1f;
        public int CautiousMod          = 2;

        public float BaseTemperateTravelTime        = 60.5f;    //represents time to travel 1 pixel on foot recklessly, camping out, for different terrains
        public float BaseDesert224_225TravelTime    = 63.5f;    //should be fairly close
        public float BaseDesert229TravelTime        = 65.5f;
        public float BaseMountain226TravelTime      = 67.5f;
        public float BaseSwamp227_228TravelTime     = 72.5f;
        public float BaseMountain230TravelTime      = 60.5f;
        public float BaseOceanTravelTime            = 153.65f;


        enum TerrainTypes
        {
            None        = 0,
            ocean       = 223,
            Desert      = 224,
            Desert2     = 225,
            Mountain    = 226,
            Swamp       = 227,
            Swamp2      = 228,
            Desert3     = 229,
            Mountain2   = 230,
            Temperate   = 231,
            Temperate2  = 232
        };
 

        Color32 toggleColor = new Color32(85, 117, 48, 255);

        Panel travelPanel;
        Panel speedToggleColorPanel;
        Panel transportToggleColorPanel;
        Panel sleepToggleColorPanel;

        Button beginButton;
        Button exitButton;
        Button speedToggleButton;
        Button transportModeToggleButton;
        Button campOutToggleButton;
        Button innToggleButton;
        Texture2D nativeTexture;

        //rects
        Rect nativePanelRect        = new Rect(49, 28, 223, 97);
        Rect exitButtonRect         = new Rect(222, 112, 48, 10);
        Rect beginButtonRect        = new Rect(222, 98, 48, 10);
        Rect speedButtonRect        = new Rect(50, 51, 108, 20);
        Rect transportButtonRect    = new Rect(163, 51, 108, 20);
        Rect innsButtonRect         = new Rect(50, 83, 108, 9);
        Rect campoutButtonRect      = new Rect(163, 83, 108, 9);

        Vector2 colorPanelSize      = new Vector2(4.5f, 5f);
        Vector2 cautiousPanelPos    = new Vector2(52, 53.25f);
        Vector2 recklessPanelPos    = new Vector2(52, 63.25f);
        Vector2 innPanelPos         = new Vector2(52, 85.25f);
        Vector2 campoutPos          = new Vector2(165, 85.25f);
        Vector2 footPos             = new Vector2(165, 53.25f);
        Vector2 shipPos             = new Vector2(165, 63.25f);
        DFPosition endPos           = new DFPosition(109, 158);

        TextLabel avaliableGoldLabel;
        TextLabel tripCostLabel;
        TextLabel travelTimeLabel;

        bool speedCautious  = true;
        bool travelFoot     = true;
        bool sleepModeInn   = true;

        int tripCost = 0;
        double finalTravelTime = 1;

        List<TerrainTypes> terrains = new List<TerrainTypes>();

        #endregion

        #region Properties

        internal DFPosition EndPos { get { return endPos; } set { endPos = value;} }
        internal DaggerfallTravelMapWindow TravelWindow { get { return travelWindow; } set { travelWindow = value; } }

        public bool PlayerHasHorse { get; set; }
        public bool PlayerHasCart { get; set; }
        public bool PlayerHasShip { get; set; }

        #endregion

        #region constructors

        public DaggerfallTravelPopUp(IUserInterfaceManager uiManager, IUserInterfaceWindow previousWindow = null, DaggerfallTravelMapWindow travelWindow = null)
            : base(uiManager, previousWindow)
        {
            this.travelWindow = travelWindow;
        }

        #endregion


        #region User InterFace

        protected override void Setup()
        {
            base.Setup();

            nativeTexture = DaggerfallUI.GetTextureFromImg(nativeImgName);
            if (!nativeTexture)
                throw new System.Exception("DaggerfallTravelMap: Could not load native texture.");

            ParentPanel.BackgroundColor = Color.clear;

            travelPanel = DaggerfallUI.AddPanel(nativePanelRect, NativePanel);
            travelPanel.BackgroundTexture = nativeTexture;

            avaliableGoldLabel = DaggerfallUI.AddTextLabel(DaggerfallUI.DefaultFont, new Vector2(148, 97), "0", NativePanel);
            avaliableGoldLabel.MaxCharacters = 12;

            tripCostLabel = DaggerfallUI.AddTextLabel(DaggerfallUI.DefaultFont, new Vector2(117,107), "0", NativePanel);
            tripCostLabel.MaxCharacters = 18;

            travelTimeLabel = DaggerfallUI.AddTextLabel(DaggerfallUI.DefaultFont, new Vector2(129,117), "0", NativePanel);
            travelTimeLabel.MaxCharacters = 16;

            speedToggleColorPanel = DaggerfallUI.AddPanel(new Rect(cautiousPanelPos, colorPanelSize), NativePanel);
            speedToggleColorPanel.BackgroundColor = toggleColor;

            sleepToggleColorPanel = DaggerfallUI.AddPanel(new Rect(innPanelPos, colorPanelSize), NativePanel);
            sleepToggleColorPanel.BackgroundColor = toggleColor;

            transportToggleColorPanel = DaggerfallUI.AddPanel(new Rect(footPos, colorPanelSize), NativePanel);
            transportToggleColorPanel.BackgroundColor = toggleColor;

            SetupButtons();
            Refresh();
        }



        void SetupButtons()
        {
            beginButton = DaggerfallUI.AddButton(beginButtonRect, NativePanel );
            beginButton.OnMouseClick += BeginButtonOnClickHandler;

            exitButton = DaggerfallUI.AddButton(exitButtonRect, NativePanel);
            exitButton.OnMouseClick += ExitButtonOnClickHandler;

            speedToggleButton = DaggerfallUI.AddButton(speedButtonRect, NativePanel);
            speedToggleButton.OnMouseClick += SpeedButtonOnClickHandler;

            transportModeToggleButton = DaggerfallUI.AddButton(transportButtonRect, NativePanel);
            transportModeToggleButton.OnMouseClick += TransportModeButtonOnClickHandler;

            innToggleButton = DaggerfallUI.AddButton(innsButtonRect, NativePanel);
            innToggleButton.OnMouseClick += SleepModeButtonOnClickHandler;

            campOutToggleButton = DaggerfallUI.AddButton(campoutButtonRect, NativePanel);
            campOutToggleButton.OnMouseClick += SleepModeButtonOnClickHandler;
        }


        public override void OnPush()
        {
            base.OnPush();
            GetPath(endPos);
            if (base.IsSetup)
                Refresh();
        }

        #endregion


        #region Methods

        //Update when player pushes buttons etc.
        void Refresh()
        {
            finalTravelTime = CalculateTravelTimeTotal(speedCautious, sleepModeInn, travelFoot);
            UpdateTogglePanels();
            UpdateLabels();
        }


        //Updates the positions for the panels to indicate which button is selected
        void UpdateTogglePanels()
        {
            if (speedCautious)
                speedToggleColorPanel.Position = cautiousPanelPos;
            else
                speedToggleColorPanel.Position = recklessPanelPos;
            if (sleepModeInn)
                sleepToggleColorPanel.Position = innPanelPos;
            else
                sleepToggleColorPanel.Position = campoutPos;
            if (travelFoot)
                transportToggleColorPanel.Position = footPos;
            else
                transportToggleColorPanel.Position = shipPos;
        }

        //Gets path from player location to destination
        void GetPath(DFPosition endPos)
        {
            Vector2[] directions = new Vector2[] { new Vector2(0, 1), new Vector2(1, 0), new Vector2(0, -1), new Vector2(-1, 0), new Vector2(1, 1), new Vector2(-1, 1), new Vector2(1, -1), new Vector2(-1, -1) };
            //Vector2[] directions = new Vector2[] { new Vector2(0, 1), new Vector2(1, 0), new Vector2(0, -1), new Vector2(-1, 0)}; //4 direction movement
            Vector2 current = new Vector2(GameManager.Instance.PlayerGPS.CurrentMapPixel.X, GameManager.Instance.PlayerGPS.CurrentMapPixel.Y);
            Vector2 end = new Vector2(endPos.X, endPos.Y);
            terrains.Clear();
            while(current != end)
            {
                float distance = Vector2.Distance(current,end);
                int selection = 0;

                for (int i = 0; i < directions.Length; i++)
                {
                    Vector2 next = current + directions[i];
                    if (current.x < 0 || current.y < 0 || current.x >= DaggerfallConnect.Arena2.MapsFile.MaxMapPixelX || current.y >= DaggerfallConnect.Arena2.MapsFile.MaxMapPixelY)
                        continue;

                    float check = Vector2.Distance(next, end);
                    if(check < distance)
                    {
                        distance = check;
                        selection = i;
                    }

                }

                current += directions[selection];
                terrains.Add((TerrainTypes)DaggerfallUnity.Instance.ContentReader.MapFileReader.GetClimateIndex((int)current.x, (int)current.y));
            }
        }

        float CalculateTravelTimeTotal(bool cautiousSpeed = false, bool inn = false, bool horse = false, bool cart = false, bool ship = false)
        {
            float time = 0;
            foreach(TerrainTypes terrain in terrains)
            {
                time += CalculateTravelTime(terrain, cautiousSpeed, inn, horse, cart, ship);
            }

            //Debug.Log(string.Format("Total Time Cost: {0}  Inn: {1} PlayerHasShip {2} PlayerHasCart: {3} PlayerHasHorse: {4}", time, inn, PlayerHasShip, PlayerHasCart, PlayerHasHorse));
            return time;

        }


        float CalculateTravelTime(TerrainTypes terrainType, bool cautiousSpeed = false, bool inn = false, bool horse = false, bool cart = false, bool ship = false)
        {
            float time = 0;

            switch (terrainType)
            {
                case TerrainTypes.None:
                    time += BaseTemperateTravelTime;
                    break;
                case TerrainTypes.ocean:
                    time += BaseOceanTravelTime;
                    break;
                case TerrainTypes.Desert:
                    time += BaseDesert224_225TravelTime;
                    break;
                case TerrainTypes.Desert2:
                    time += BaseDesert224_225TravelTime;
                    break;
                case TerrainTypes.Mountain:
                    time += BaseMountain226TravelTime;
                    break;
                case TerrainTypes.Swamp:
                    time += BaseSwamp227_228TravelTime;
                    break;
                case TerrainTypes.Swamp2:
                    time += BaseSwamp227_228TravelTime;
                    break;
                case TerrainTypes.Desert3:
                    time += BaseDesert229TravelTime;
                    break;
                case TerrainTypes.Mountain2:
                    time += BaseMountain230TravelTime;
                    break;
                case TerrainTypes.Temperate:
                    time += BaseTemperateTravelTime;
                    break;
                case TerrainTypes.Temperate2:
                    time += BaseTemperateTravelTime;
                    break;
                default:
                    time += BaseTemperateTravelTime;
                    break;
            }

            if (cautiousSpeed)
                time = time * 2;
            if (terrainType == TerrainTypes.ocean && PlayerHasShip)
                time *= ShipMod;
            else if (terrainType != TerrainTypes.ocean)
            {
                if (inn)
                    time *= InnModifier;
                if (PlayerHasCart)
                    time *= CartMod;
                else if (PlayerHasHorse)
                    time *= HorseMod;
            }

            //Debug.Log(string.Format("Time Cost: {0} Terrain Type: {1} Inn: {2} PlayerHasShip {3} PlayerHasCart: {4} PlayerHasHorse: {5}", time, terrainType.ToString(), inn, PlayerHasShip, PlayerHasCart, PlayerHasHorse));
            return time;

        }

        //Updates text labels
        void UpdateLabels()
        {
            avaliableGoldLabel.Text = "0";  //##TODO - when player gold implemented, update this
            tripCostLabel.Text = tripCost.ToString();

            int travelTimeHours = (int)finalTravelTime / 60;
            if (travelTimeHours <= 24)
                travelTimeLabel.Text = "1";
            else
                travelTimeLabel.Text = string.Format("{0}", travelTimeHours / 24);
        }

        #endregion


        #region events

        public void BeginButtonOnClickHandler(BaseScreenComponent sender, Vector2 position)
        {
            Refresh();
            Debug.Log("Final Travel Time: " + finalTravelTime);
            GameManager.Instance.StreamingWorld.TeleportToCoordinates((int)endPos.X, (int)endPos.Y, StreamingWorld.RepositionMethods.RandomStartMarker);
            DaggerfallUnity.WorldTime.DaggerfallDateTime.RaiseTime((float)finalTravelTime * 60);
            terrains.Clear();
            DaggerfallUI.Instance.UserInterfaceManager.PopWindow();
            travelWindow.CloseTravelWindows(true);
        }

        public void ExitButtonOnClickHandler(BaseScreenComponent sender, Vector2 position)
        {
            terrains.Clear();
            DaggerfallUI.Instance.UserInterfaceManager.PopWindow();
        }

        public void SpeedButtonOnClickHandler(BaseScreenComponent sender, Vector2 position)
        {
            speedCautious = !speedCautious;
            Refresh();
        }

        public void TransportModeButtonOnClickHandler(BaseScreenComponent sender, Vector2 position)
        {
            travelFoot = !travelFoot;
            Refresh();
        }

        public void SleepModeButtonOnClickHandler(BaseScreenComponent sender, Vector2 position)
        {
            sleepModeInn = !sleepModeInn;
            Refresh();
        }

        #endregion

    }
}
