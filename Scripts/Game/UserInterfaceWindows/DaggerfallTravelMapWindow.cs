using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallConnect;
using DaggerfallConnect.Utility;
using DaggerfallConnect.Arena2;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{


    public class RegionRecord
    {
        public RegionRecord(){}

        public RegionRecord(string[] regionStrings, DFRegion region, int ID)
        {

            this.Region = region;
            this.ID = ID;
            if(regionStrings != null)
            {
                if(regionStrings.Length > 0)
                {
                    this.RegionName = regionStrings[0];
                    this.RegionImgNames = new string[regionStrings.Length - 1];
                    Array.ConstrainedCopy(regionStrings, 1, this.RegionImgNames, 0, regionStrings.Length - 1);
                }
            }
        }


        public DFRegion Region { get; private set; }

        public int ID { get; private set; }

        public string RegionName { get; private set; }

        public string[] RegionImgNames { get; private set; }

        public string FirstImgName
        {
            get
            {
                if (RegionImgNames == null)
                    return null;
                else if (RegionImgNames.Length < 1)
                    return null;
                else
                    return RegionImgNames[0];
            }
        }

        public bool HasMultipleMaps
        {
            get
            {
                if (RegionImgNames == null)
                    return false;
                return RegionImgNames.Length > 1;
            }
        }

        public bool HasVerticalMaps
        {
            get
            {
                if (RegionImgNames == null)
                    return false;
                else
                    return RegionImgNames.Length > 2;
            }
        }
    }

    enum SearchPatterns
    {
        All,
        Cities,
        Dungeons,
        Homes,
        Temples,
    }

    public class DaggerfallTravelMapWindow : DaggerfallPopupWindow
    {
        const string nativeImgName              = "TRAV0I00.IMG";
        const string regionPickerImgName        = "TRAV0I01.IMG";
        const string findAtBttnImgName          = "TRAV0I03.IMG";
        const string locationFilterBttnImgName  = "TRAV01I0.IMG";
        const string downArrowImgName           = "TRAVAI05.IMG";
        const string upArrowImgName             = "TRAVBI05.IMG";
        const string rightArrowImgName          = "TRAVCI05.IMG";
        const string leftArrowImgName           = "TRAVDI05.IMG";

        private string regionImgName { get; set; }

        TextLabel regionLabel;
        Texture2D nativeTexture;
        Texture2D regionTexture;
        Texture2D findAtBttnTexture;
        Texture2D locationFilterTexture;
        Texture2D horiztonalArrowTexture;
        Texture2D verticalArrowTexture;
        DFBitmap regionPickerBitmap;

        Panel findAtBttnPanel;
        Panel locationFilterBttnPanel;
        Panel regionTextureOverlayPanel;

        Button findBttn;
        Button atBttn;
        Button exitBttn;
        Button horizontalArrowBttn;
        Button verticalArrowBttn;

        Rect findAtBttnPanelRect            = new Rect(1, 175, 39, 22);
        Rect locationFilterBttnPanelRect    = new Rect(40, 175, 200, 22);
        Rect regionTextureOverlayPanelRect  = new Rect(0, 12, 320, 160);
        Rect exitBttnRect                   = new Rect(278, 175, 39, 22);
        Rect horizontalArrowBttnRect        = new Rect(261, 175, 17, 22);
        Rect verticalArrowBttnRect          = new Rect(244, 175, 17, 22);
        Vector2 offScreenPos                = new Vector2(Screen.width, Screen.height);

        //DaggerfallTravelMapFindPopUpWindow findLocationPopUp;

        public static Dictionary<int, string[]> regionLookUp = new Dictionary<int, string[]>();
        bool regionSelected = false;
        bool locationSelected = false;
        int mapIndex = 0;

        DFRegion.RegionMapTable locationInfo;                               //location selected to travel to
        RegionRecord regionRecord;                                          //current region in region panel if open
        List<DaggerfallConnect.DFRegion.RegionMapTable> locations;          //list of locations in selected region


        public DaggerfallTravelMapWindow(IUserInterfaceManager uiManager)
            : base(uiManager)
        {

        }



        protected override void Setup()
        {
            nativeTexture = DaggerfallUI.GetTextureFromImg(nativeImgName);
            if (!nativeTexture)
                throw new Exception("DaggerfallTravelMap: Could not load native texture.");

            NativePanel.ScalingMode = Scaling.StretchToFill;

            // Setup native panel background
            NativePanel.BackgroundTexture = nativeTexture;

            //populate the region lookup
            PopulateRegionDictionary();

            // Load picker colours
            regionPickerBitmap = DaggerfallUI.GetImgBitmap(regionPickerImgName);

            //Add region label
            regionLabel = DaggerfallUI.AddTextLabel(DaggerfallUI.DefaultFont, new Vector2(150, 2), "", NativePanel);

            // Handle clicks
            NativePanel.OnMouseClick += ClickHandler;

            //draw texture for find & Im at buttons
            findAtBttnPanel = DaggerfallUI.AddPanel(findAtBttnPanelRect, NativePanel);
            findAtBttnPanel.BackgroundTexture = DaggerfallUI.GetTextureFromImg(findAtBttnImgName);

            //draw textures for location filter buttons
            locationFilterBttnPanel = DaggerfallUI.AddPanel(locationFilterBttnPanelRect, NativePanel);
            locationFilterBttnPanel.BackgroundTexture = DaggerfallUI.GetTextureFromImg(locationFilterBttnImgName);
            //setup buttons
            SetupButtons();

            regionTextureOverlayPanel = DaggerfallUI.AddPanel(regionTextureOverlayPanelRect, NativePanel);
        }

        public override void Update()
        {
            base.Update();

            if (Input.GetKeyDown(exitKey))
            {
                CloseTravelWindows();
            }
            UpdateRegionlabel();
        }

        /// <summary>
        /// Handle clicks on the main panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="position"></param>
        public void ClickHandler(BaseScreenComponent sender, Vector2 position)
        {
            if (regionSelected)         //don't handle clicks when region panel open; could also unregister click handler
                return;

            RegionRecord record = null;                     //check if player clicked on region, set & open region panel if so
            if(GetRegionRecord(position, ref record) && record != null)
            {
                OpenRegionPanel(record);
            }
        }

        /// <summary>
        /// Handle clicks on the region panel overlay
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="position"></param>
        public void RegionPanelClickHandler(BaseScreenComponent sender, Vector2 position)
        {

        }


        public void ExitButtonClickHandler(BaseScreenComponent sender, Vector2 position)
        {
            CloseTravelWindows();
        }


        public void AtButtonClickHandler(BaseScreenComponent sender, Vector2 position)
        {
            if(!regionSelected)             //get current player region, open
            {
                string regionName = GameObject.FindObjectOfType<PlayerGPS>().CurrentRegionName;
                if(GetRegionRecordByName(regionName, ref regionRecord))
                {
                    OpenRegionPanel(regionRecord);
                }
            }
            else                              //locate player in region map
            {
                throw new NotImplementedException();
            }
        }

        public void FindlocationButtonClickHandler(BaseScreenComponent sender, Vector2 position)
        {
            if (!regionSelected)             //do nothing
            {
                return;
            }
            else                              //open find location pop-up
            {
                //DaggerfallTravelMapFindPopUpWindow findPopUp = new DaggerfallTravelMapFindPopUpWindow(DaggerfallUI.UIManager, this);
                //findPopUp.OnEnterLocation += HandleLocationFindEvent;
                //findPopUp.Show();
                DaggerfallInputMessageBox findPopUp = new DaggerfallInputMessageBox(uiManager, null, 31, HardStrings.findLocationPrompt, true, true, this);
                findPopUp.OnGotUserInput += HandleLocationFindEvent;
                findPopUp.Show();
            }
        }

        public void HorizontalArrowButtonClickHander(BaseScreenComponent sender, Vector2 position)
        {
            if (!regionSelected || regionRecord == null)
                return;
            if (!regionRecord.HasMultipleMaps)
                return;

            int newIndex = mapIndex;
            if (newIndex % 2 == 0)   //move right
            {
                newIndex += 1;
            }
            else //move left
            {
                newIndex -= 1;
            }
            if (newIndex >= regionRecord.RegionImgNames.Length || newIndex < 0)
            {
                DaggerfallUnity.LogMessage(string.Format("Invalid map index selected: {0} {1} {2}", regionRecord.RegionName, regionRecord.RegionImgNames.Length, newIndex));
                return;
            }

            mapIndex = newIndex;
            SetupArrowButtons();
            regionTextureOverlayPanel.BackgroundTexture = DaggerfallUI.GetTextureFromImg(regionRecord.RegionImgNames[mapIndex]);
        }


        public void VerticalArrowButtonClickHander(BaseScreenComponent sender, Vector2 position)
        {

            if (!regionSelected || regionRecord == null)
                return;
            if (!regionRecord.HasVerticalMaps)
                return;

            int newIndex = mapIndex;
            if (newIndex > 1)   //move up, decrease index
            {
                newIndex -=  2;
            }
            else                      //move down, increase index
            {
                newIndex += 2;
            }

            if (newIndex >= regionRecord.RegionImgNames.Length || newIndex < 0)
            {
                DaggerfallUnity.LogMessage(string.Format("Invalid map index selected: {0} {1} {2}", regionRecord.RegionName, regionRecord.RegionImgNames.Length, newIndex));
                return;
            }

            mapIndex = newIndex;
            SetupArrowButtons();
            regionTextureOverlayPanel.BackgroundTexture = DaggerfallUI.GetTextureFromImg(regionRecord.RegionImgNames[mapIndex]);
        }


        /// <summary>
        /// Handles events from Find Location pop-up
        /// </summary>
        /// <param name="locationName"></param>
        public void HandleLocationFindEvent(DaggerfallInputMessageBox inputMessageBox, string locationName)
        {

            locationSelected = false;

            if (string.IsNullOrEmpty(locationName))
            {
                TextFile.Token[] textTokens = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(13);
                DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, this);
                messageBox.SetTextTokens(textTokens);
                messageBox.ClickAnywhereToClose = true;
                uiManager.PushWindow(messageBox);
                return;
            }

            else if (!FindLocation(locationName, out locationInfo))
            {
                TextFile.Token[] textTokens = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(13);
                DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, this);
                messageBox.SetTextTokens(textTokens);
                messageBox.ClickAnywhereToClose = true;
                uiManager.PushWindow(messageBox);
                return;
            }

            else
            {
                locationSelected = true;
                TextFile.Token[] textTokens = new TextFile.Token[2];
                int index = regionRecord.Region.MapIdLookup[locationInfo.MapId];
                textTokens[0].text = string.Format("Travel to  {0} : {1} ?", regionRecord.Region.Name, regionRecord.Region.MapNames[index]);
                textTokens[0].formatting = TextFile.Formatting.Text;
                textTokens[1].text = null;
                textTokens[1].formatting = TextFile.Formatting.NewLine;

                DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, this);    //temp solution
                messageBox.SetTextTokens(textTokens);
                messageBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.Yes);
                messageBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.No);
                messageBox.OnButtonClick += ConfirmTravelPopupButtonClick;
                uiManager.PushWindow(messageBox);
            }
        }


        /// <summary>
        /// Button handler for travel confirmation pop up. Theis is a temp. solution until implementing the final pop-up
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="messageBoxButton"></param>
        public void ConfirmTravelPopupButtonClick(DaggerfallMessageBox sender, DaggerfallMessageBox.MessageBoxButtons messageBoxButton)
        {
            sender.CloseWindow();

            if (messageBoxButton == DaggerfallMessageBox.MessageBoxButtons.Yes)
            {
                DFPosition pos = MapsFile.LongitudeLatitudeToMapPixel((int)locationInfo.Longitude, (int)locationInfo.Latitude);
                TravelToLocation(pos);
            }
        }




        /// <summary>
        /// Opens the region panel to the specified region
        /// </summary>
        /// <param name="record"></param>
        public void OpenRegionPanel(RegionRecord record)
        {
            if (record == null)
            {
                DaggerfallUnity.LogMessage("Error: Regionrecord Null in OpenregionPanel, can't open Region");
                return;
            }

            regionRecord = record;
            locationSelected = false;
            mapIndex = 0;

            //regionTextureOverlayPanel = DaggerfallUI.AddPanel(regionTextureOverlayPanelRect, NativePanel);
            regionTextureOverlayPanel.Position = regionTextureOverlayPanelRect.position;
            regionTexture = DaggerfallUI.GetTextureFromImg(record.FirstImgName);
            regionTextureOverlayPanel.BackgroundTexture = regionTexture;
            regionTextureOverlayPanel.OnMouseClick += RegionPanelClickHandler;
            SetupArrowButtons();
            regionSelected = true;
        }


        /// <summary>
        /// Close region panel and reset values
        /// </summary>
        void CloseRegionPanel()
        {
            regionRecord = null;
            regionSelected = false;
            locationSelected = false;
            mapIndex = 0;
            locations = null;

            if (regionTextureOverlayPanel != null)
            {
                regionTextureOverlayPanel.Position = offScreenPos;
                regionTexture = null;
                regionTextureOverlayPanel.OnMouseClick -= RegionPanelClickHandler;

            }
            if(verticalArrowBttn != null)
                verticalArrowBttn.Position = offScreenPos;
            if(horizontalArrowBttn != null)
                horizontalArrowBttn.Position = offScreenPos;
        }


        void SetupArrowButtons()
        {
            string horImgName = rightArrowImgName;
            string vertImgName = downArrowImgName;
            Vector2 vertPos = offScreenPos;
            Vector2 horzPos = offScreenPos;

            if(regionRecord.HasVerticalMaps)
            {
                vertPos = verticalArrowBttnRect.position;

                if (mapIndex > 1)                       //move up, decrease index
                {
                    vertImgName = upArrowImgName;
                }
                else                                    //move down, increase index
                {
                    vertImgName = downArrowImgName;
                }
            }
            if (regionRecord.HasMultipleMaps)
            {
                horzPos = horizontalArrowBttnRect.position;
                if (mapIndex % 2 == 0)                  //move right
                {
                    horImgName = rightArrowImgName;
                }
                else                                    //move left
                {
                    horImgName = leftArrowImgName;
                }
            }

            horizontalArrowBttn.Position = horzPos;
            verticalArrowBttn.Position = vertPos;
            horizontalArrowBttn.BackgroundTexture = DaggerfallUI.GetTextureFromImg(horImgName);
            verticalArrowBttn.BackgroundTexture = DaggerfallUI.GetTextureFromImg(vertImgName);
        }




        /// <summary>
        /// Find location by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool FindLocation(string name, out DFRegion.RegionMapTable locationInfo)
        {
            locationInfo = new DFRegion.RegionMapTable();

            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            string[] locations = regionRecord.Region.MapNames.OrderBy(p => p).ToArray();
            name = name.ToLower();

            for (int i = 0; i < locations.Count(); i++)
            {
                if (locations[i].ToLower().StartsWith(name))                       //valid location found,
                {
                    if(!regionRecord.Region.MapNameLookup.ContainsKey(locations[i]))
                    {
                        DaggerfallUnity.LogMessage("Error: location name key not found in Region MapNameLookup dictionary");
                        return false;
                    }

                    int index = regionRecord.Region.MapNameLookup[locations[i]];
                    locationInfo = regionRecord.Region.MapTable[index];
                    return true;

                }
                else if (locations[i][0] > name[0])
                {
                    return false;
                }

            }
            return false;
        }


        /// <summary>
        /// Teleports player to coordinates of location
        /// </summary>
        /// <param name="lon"></param>
        /// <param name="lat"></param>
        void TravelToLocation(int lon, int lat)
        {
            try
            {
                GameObject.FindObjectOfType<StreamingWorld>().TeleportToCoordinates(lon, lat);
                CloseTravelWindows(true);
            }
            catch (Exception ex)
            {
                DaggerfallUnity.LogMessage(ex.Message);
            }
        }

        void TravelToLocation(DFPosition pos)
        {
            TravelToLocation(pos.X, pos.Y);
        }

        /// <summary>
        /// Returns a region record based on scaled mouse position
        /// </summary>
        /// <param name="mousePos"></param>
        /// <param name="record"></param>
        /// <returns></returns>
        bool GetRegionRecord(Vector2 mousePos, ref RegionRecord record)
        {

            int offset = (int)mousePos.y * regionPickerBitmap.Width + (int)mousePos.x;

            if (offset < 0 || offset >= regionPickerBitmap.Data.Length)
                return false;
            int id = regionPickerBitmap.Data[offset];

            if (regionLookUp.ContainsKey(id))
            {
                string regionName = regionLookUp[id][0];
                record = new RegionRecord
                    (
                        regionLookUp[id],
                        DaggerfallUnity.Instance.ContentReader.MapFileReader.GetRegion(regionName),
                        id
                    );
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// Get region record by region name; mainly used to find region player is in
        /// </summary>
        /// <param name="regionName"></param>
        /// <param name="record"></param>
        /// <returns></returns>
        bool GetRegionRecordByName(string regionName, ref RegionRecord record)
        {
            if(string.IsNullOrEmpty(regionName))
            {
                return false;
            }


            foreach(KeyValuePair<int, string[]> kvp in regionLookUp)
            {
                if(kvp.Value[0] == regionName)
                {
                    record = new RegionRecord
                    (
                            kvp.Value,
                            DaggerfallUnity.Instance.ContentReader.MapFileReader.GetRegion(kvp.Value[0]),
                            kvp.Key
                     );
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Updates the text label at top of screen
        /// </summary>
        void UpdateRegionlabel()
        {
            if(!regionSelected)
            {
                RegionRecord record = null;
                if (GetRegionRecord(base.NativePanel.ScaledMousePosition, ref record))
                {
                    regionLabel.Text = record.RegionName;
                }
                else
                    regionLabel.Text = "";
            }
            else
            {
                regionLabel.Text = regionRecord.RegionName;
            }
            regionLabel.Position = new Vector2(150 - regionLabel.Size.x / 2, regionLabel.Position.y);
        }


        /// <summary>
        /// Initial button setup
        /// </summary>
        void SetupButtons()
        {
            exitBttn = DaggerfallUI.AddButton(exitBttnRect, NativePanel);
            exitBttn.OnMouseClick += ExitButtonClickHandler;


            findBttn = DaggerfallUI.AddButton(new Rect(findAtBttnPanelRect.x, findAtBttnPanelRect.y, findAtBttnPanelRect.width, findAtBttnPanelRect.height / 2.0f), NativePanel);
            findBttn.OnMouseClick += FindlocationButtonClickHandler;

            atBttn = DaggerfallUI.AddButton(new Rect(findAtBttnPanelRect.x, findAtBttnPanelRect.y + findAtBttnPanelRect.height/2, findAtBttnPanelRect.width, findAtBttnPanelRect.height), NativePanel);
            atBttn.OnMouseClick += AtButtonClickHandler;

            horizontalArrowBttn = DaggerfallUI.AddButton(new Rect(offScreenPos, horizontalArrowBttnRect.size), NativePanel);
            horizontalArrowBttn.OnMouseClick += HorizontalArrowButtonClickHander;

            verticalArrowBttn = DaggerfallUI.AddButton(new Rect(offScreenPos, verticalArrowBttnRect.size), NativePanel);
            verticalArrowBttn.OnMouseClick += VerticalArrowButtonClickHander;
        }

        /// <summary>
        ///Adds region map elements to region lookup
        /// </summary>
        void PopulateRegionDictionary()
        {

            regionLookUp = new Dictionary<int, string[]>();
            regionLookUp.Add(133, new string[] { "Dwynnen", "FMAP0I05.IMG" });
            regionLookUp.Add(137, new string[] { "Isle of Balfiera", "FMAP0I09.IMG" });
            regionLookUp.Add(139, new string[] { "Dak'fron", "FMAP0I11.IMG" });
            regionLookUp.Add(145, new string[] { "Daggerfall", "FMAP0I17.IMG" });
            regionLookUp.Add(146, new string[] { "Glenpoint", "FMAP0I18.IMG" });
            regionLookUp.Add(147, new string[] { "Betony", "FMAP0I19.IMG" });
            regionLookUp.Add(148, new string[] { "Sentinel", "FMAP0I20.IMG" });
            regionLookUp.Add(149, new string[] { "Anticlere", "FMAP0I21.IMG" });
            regionLookUp.Add(150, new string[] { "Lainlyn", "FMAP0I22.IMG" });
            regionLookUp.Add(151, new string[] { "Wayrest", "FMAP0I23.IMG" });
            regionLookUp.Add(154, new string[] { "Orsinium Area", "FMAP0I26.IMG" });
            regionLookUp.Add(160, new string[] { "Northmoor", "FMAP0I32.IMG" });
            regionLookUp.Add(161, new string[] { "Menevia", "FMAP0I33.IMG" });
            regionLookUp.Add(162, new string[] { "Alcaire", "FMAP0I34.IMG" });
            regionLookUp.Add(163, new string[] { "Koegria", "FMAP0I35.IMG" });
            regionLookUp.Add(164, new string[] { "Bhoriane", "FMAP0I36.IMG" });
            regionLookUp.Add(165, new string[] { "Kambria", "FMAP0I37.IMG" });
            regionLookUp.Add(166, new string[] { "Phrygias", "FMAP0I38.IMG" });
            regionLookUp.Add(167, new string[] { "Urvsiud", "FMAP0I39.IMG" });
            regionLookUp.Add(168, new string[] { "Ykalon", "FMAP0I40.IMG" });
            regionLookUp.Add(169, new string[] { "Daenia", "FMAP0I41.IMG" });
            regionLookUp.Add(170, new string[] { "Shalgora", "FMAP0I42.IMG" });
            regionLookUp.Add(171, new string[] { "Abibon-Gora", "FMAP0I43.IMG" });
            regionLookUp.Add(172, new string[] { "Kairou", "FMAP0I44.IMG" });
            regionLookUp.Add(173, new string[] { "Pothago", "FMAP0I45.IMG" });
            regionLookUp.Add(174, new string[] { "Myrkwasa", "FMAP0I46.IMG" });
            regionLookUp.Add(175, new string[] { "Ayasofya", "FMAP0I47.IMG" });
            regionLookUp.Add(176, new string[] { "Tigonus", "FMAP0I48.IMG" });
            regionLookUp.Add(177, new string[] { "Kozanset", "FMAP0I49.IMG" });
            regionLookUp.Add(178, new string[] { "Satakalaam", "FMAP0I50.IMG" });
            regionLookUp.Add(179, new string[] { "Totambu", "FMAP0I51.IMG" });
            regionLookUp.Add(180, new string[] { "Mournoth", "FMAP0I52.IMG" });
            regionLookUp.Add(181, new string[] { "Ephesus", "FMAP0I53.IMG" });
            regionLookUp.Add(182, new string[] { "Santaki", "FMAP0I54.IMG" });
            regionLookUp.Add(183, new string[] { "Antiphyllos", "FMAP0I55.IMG" });
            regionLookUp.Add(184, new string[] { "Bergama", "FMAP0I56.IMG" });
            regionLookUp.Add(185, new string[] { "Gavaudon", "FMAP0I57.IMG" });
            regionLookUp.Add(186, new string[] { "Tulune", "FMAP0I58.IMG" });
            regionLookUp.Add(187, new string[] { "Glenumbra Moors", "FMAP0I59.IMG" });
            regionLookUp.Add(188, new string[] { "Ilessan Hills", "FMAP0I60.IMG" });
            regionLookUp.Add(128, new string[] { "Alik'r Desert", "FMAPAI00.IMG", "FMAPBI00.IMG" });
            regionLookUp.Add(129, new string[] { "Dragontail Mountains", "FMAPAI01.IMG", "FMAPBI01.IMG", "FMAPCI01.IMG", "FMAPDI01.IMG" });
            regionLookUp.Add(144, new string[] { "Wrothgarian Mountains", "FMAPAI16.IMG", "FMAPBI16.IMG", "FMAPCI16.IMG", "FMAPDI16.IMG" });
        }


        /// <summary>
        /// Closes windows based on context.
        /// </summary>
        /// <param name="forceClose"></param>
        void CloseTravelWindows(bool forceClose = false)
        {
            if(!regionSelected || forceClose)
            {
                CloseWindow();
            }
            //close region panel
            CloseRegionPanel();
        }

    }

}
