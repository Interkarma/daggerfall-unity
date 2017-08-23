// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2017 Daggerfall Workshop
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
using System.Collections;
using System.Collections.Generic;
using DaggerfallConnect.Utility;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.Formulas;
using DaggerfallWorkshop.Game.Utility;


namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{

    public class DaggerfallTravelPopUp : DaggerfallPopupWindow
    {
        #region fields
        DaggerfallTravelMapWindow travelWindow = null;

        const string nativeImgName = "TRAV0I04.IMG";

        const float secondsCountdownTickFastTravel = 0.05f; // time used for fast travel countdown for one tick

        TravelTimeCalculator travelTimeCalculator = new TravelTimeCalculator();

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

        TextLabel availableGoldLabel;
        TextLabel tripCostLabel;
        TextLabel travelTimeLabel;

        int countdownValueTravelTimeDays; // used for remaining days in fast travel countdown
        GameObject goCoroutineHandlerFastTravel; // gameobject which is used to attach CoroutineHandlerFastTravel
        CoroutineHandlerFastTravel coroutineHandlerFastTravel; // used for coroutines
        bool inProgressCountdownTravelTime = false;
        bool inProgressFastTravel = false;

        bool speedCautious  = true;
        bool travelFoot     = true;
        bool sleepModeInn   = true;

        int tripCost = 0;

        #endregion

        #region Properties

        internal DFPosition EndPos { get { return endPos; } set { endPos = value;} }
        internal DaggerfallTravelMapWindow TravelWindow { get { return travelWindow; } set { travelWindow = value; } }

        // Interkarma Notes:
        //  * These properties are only read from and will always have default value of false
        //  * PlayerHasShip does not appear to be implemented currently
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

            availableGoldLabel = DaggerfallUI.AddTextLabel(DaggerfallUI.DefaultFont, new Vector2(148, 97), "0", NativePanel);
            availableGoldLabel.MaxCharacters = 12;

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

            goCoroutineHandlerFastTravel = GameObject.Find("GameManager"); // set gameobject to attach CoroutineHandlerFastTravel to to "GameManager"
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
            travelTimeCalculator.GeneratePath(endPos);
            if (base.IsSetup)
                Refresh();
        }

        #endregion


        #region Methods

        //Update when player pushes buttons etc.
        void Refresh()
        {
            travelTimeCalculator.CalculateTravelTimeTotal(travelFoot, PlayerHasCart, PlayerHasHorse, speedCautious, sleepModeInn, travelFoot);
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

        //Updates text labels
        void UpdateLabels()
        {
            availableGoldLabel.Text = GameManager.Instance.PlayerEntity.GoldPieces.ToString();

            int travelTimeDaysLand = 0;
            int travelTimeDaysWater = 0;
            int travelTimeDaysTotal = 0;
            
            if (travelTimeCalculator.TravelTimeTotalLand > 0)
                travelTimeDaysLand = (int)((travelTimeCalculator.TravelTimeTotalLand / 60 / 24) + 0.5);
            if (travelTimeCalculator.TravelTimeTotalWater > 0)
                travelTimeDaysWater = (int)((travelTimeCalculator.TravelTimeTotalWater / 60 / 24) + 0.5);
            travelTimeDaysTotal = travelTimeDaysLand + travelTimeDaysWater;

            tripCost = FormulaHelper.CalculateTripCost(
                travelTimeCalculator.TravelTimeTotalLand,
                travelTimeCalculator.TravelTimeTotalWater,
                speedCautious,
                sleepModeInn,
                travelFoot,
                TravelTimeCalculator.CautiousMod,
                TravelTimeCalculator.ShipMod);

            travelTimeLabel.Text = string.Format("{0}", travelTimeDaysTotal);
            tripCostLabel.Text = tripCost.ToString();

            countdownValueTravelTimeDays = travelTimeDaysTotal;
        }

        public class CoroutineHandlerFastTravel : MonoBehaviour
        {
            DaggerfallTravelPopUp popupWindow;

            //public UpdateTravelTimeEventHandler(DaggerfallTravelPopUp popupWindow)
            public void setDaggerfallTravelPopUp(DaggerfallTravelPopUp popupWindow)
            {
                this.popupWindow = popupWindow;
            }

            public void StartCoroutineFastTravelCountdown()
            {
                StartCoroutine(popupWindow.StartCoroutineFastTravelCountdown());
            }

            public void StartCoroutineFastTravelActions()
            {
                StartCoroutine(popupWindow.StartCoroutineFastTravel());
            }

            public void StartCoroutineCheckFinishedFastTravel()
            {
                StartCoroutine(popupWindow.StartCoroutineCheckFinishedFastTravel());
            }
        }

        private IEnumerator StartCoroutineFastTravelCountdown()
        {
            inProgressCountdownTravelTime = true;
            while(countdownValueTravelTimeDays > 0)
            {
                countdownValueTravelTimeDays--;
                travelTimeLabel.Text = string.Format("{0}", countdownValueTravelTimeDays);
                //Debug.Log(travelTimeLabel.Text);
                travelTimeLabel.Update();
                //yield return new WaitForSeconds(0.1f);  // WON'T WORK since timescale in pause is 0.0
                //yield return new WaitForEndOfFrame(); // works but linked to frame rate
                yield return new WaitForSecondsRealtime(0.05f);
            }
            inProgressCountdownTravelTime = false;
        }


        private IEnumerator StartCoroutineFastTravel()
        {
            inProgressFastTravel = true;
            performFastTravel();
            yield return new WaitForSecondsRealtime(0.0f);
            inProgressFastTravel = false;
        }

        private IEnumerator StartCoroutineCheckFinishedFastTravel()
        {
            while (inProgressCountdownTravelTime || inProgressFastTravel)
                yield return new WaitForSecondsRealtime(0.1f);

            finishFastTravel();
        }

        // Return whether player has enough gold for the selected travel options
        bool enoughGoldCheck()
        {
            return (GameManager.Instance.PlayerEntity.GoldPieces >= tripCost);
        }

        void showNotEnoughGoldPopup()
        {
            const int notEnoughGoldTextId = 454;

            TextFile.Token[] tokens = DaggerfallUnity.TextProvider.GetRSCTokens(notEnoughGoldTextId);
            if (tokens != null && tokens.Length > 0)
            {
                DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, this);
                messageBox.SetTextTokens(tokens);
                messageBox.ClickAnywhereToClose = true;
                messageBox.Show();
            }
        }

        #endregion


        #region events

        public void BeginButtonOnClickHandler(BaseScreenComponent sender, Vector2 position)
        {
            Refresh();
            if (!enoughGoldCheck())
            {
                showNotEnoughGoldPopup();
                return;
            }
            else
                GameManager.Instance.PlayerEntity.GoldPieces -= tripCost;

            if (!coroutineHandlerFastTravel)
            {
                coroutineHandlerFastTravel = goCoroutineHandlerFastTravel.AddComponent<CoroutineHandlerFastTravel>();
            }
            coroutineHandlerFastTravel.setDaggerfallTravelPopUp(this);
            coroutineHandlerFastTravel.StartCoroutineFastTravelCountdown();
            coroutineHandlerFastTravel.StartCoroutineFastTravelActions();
            coroutineHandlerFastTravel.StartCoroutineCheckFinishedFastTravel();
        }

        private void performFastTravel()
        {
            GameManager.Instance.StreamingWorld.TeleportToCoordinates((int)endPos.X, (int)endPos.Y, StreamingWorld.RepositionMethods.RandomStartMarker);

            if (speedCautious)
            {
                GameManager.Instance.PlayerEntity.CurrentHealth = GameManager.Instance.PlayerEntity.MaxHealth;
                GameManager.Instance.PlayerEntity.CurrentFatigue = GameManager.Instance.PlayerEntity.MaxFatigue;
                GameManager.Instance.PlayerEntity.CurrentMagicka = GameManager.Instance.PlayerEntity.MaxMagicka;
            }

            DaggerfallUnity.WorldTime.DaggerfallDateTime.RaiseTime((travelTimeCalculator.TravelTimeTotalLand + travelTimeCalculator.TravelTimeTotalWater) * 60);

            // Raise arrival time to just after 7am if cautious travel would otherwise arrive at night
            // Increasing this from 6am to 7am as game is quite dark on at 6am (in Daggerfall Unity, Daggerfall is lighter)
            // Will consider retuning lighting so this can be like classic, although +1 hours to travel time isn't likely to be a problem for now
            if (speedCautious)
            {
                if ((DaggerfallUnity.WorldTime.DaggerfallDateTime.Hour < 7)
                    || ((DaggerfallUnity.WorldTime.DaggerfallDateTime.Hour == 7) && (DaggerfallUnity.WorldTime.DaggerfallDateTime.Minute < 10)))
                {
                    float raiseTime = (((7 - DaggerfallUnity.WorldTime.DaggerfallDateTime.Hour) * 3600)
                                        + ((10 - DaggerfallUnity.WorldTime.DaggerfallDateTime.Minute) * 60)
                                        - DaggerfallUnity.WorldTime.DaggerfallDateTime.Second);
                    DaggerfallUnity.WorldTime.DaggerfallDateTime.RaiseTime(raiseTime);
                }
                else if (DaggerfallUnity.WorldTime.DaggerfallDateTime.Hour > 17)
                {
                    float raiseTime = (((31 - DaggerfallUnity.WorldTime.DaggerfallDateTime.Hour) * 3600)
                    + ((10 - DaggerfallUnity.WorldTime.DaggerfallDateTime.Minute) * 60)
                    - DaggerfallUnity.WorldTime.DaggerfallDateTime.Second);
                    DaggerfallUnity.WorldTime.DaggerfallDateTime.RaiseTime(raiseTime);
                }
            }
        }

        public void finishFastTravel()
        {
            travelTimeCalculator.ClearPath();
            DaggerfallUI.Instance.UserInterfaceManager.PopWindow();
            travelWindow.CloseTravelWindows(true);
            GameManager.Instance.PlayerEntity.RaiseSkills();
        }

        public void ExitButtonOnClickHandler(BaseScreenComponent sender, Vector2 position)
        {
            travelTimeCalculator.ClearPath();
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
