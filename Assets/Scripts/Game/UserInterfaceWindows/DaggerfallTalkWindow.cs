// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2016 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Nystul
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using System;
using System.Collections;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.UserInterfaceWindows;

using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Utility;

using System.IO;
using System.Collections.Generic;

namespace DaggerfallWorkshop.Game.UserInterface
{
    public class DaggerfallTalkWindow : DaggerfallPopupWindow
    {
        const string talkWindowImgName    = "TALK01I0.IMG";
        const string talkCategoriesImgName = "TALK02I0.IMG";
        const string highlightedOptionsImgName = "TALK03I0.IMG";

        const string portraitImgName = "TFAC00I0.RCI";

        const string greenArrowsTextureName = "INVE06I0.IMG";       // Green up/down arrows when more items available
        const string redArrowsTextureName = "INVE07I0.IMG";         // Red up/down arrows when no more items available

        Rect rectButtonTopicUp = new Rect(102, 68, 9, 16);
        Rect rectButtonTopicDown = new Rect(102, 161, 9, 16);
        Rect rectButtonTopicLeft = new Rect(5, 177, 15, 8);
        Rect rectButtonTopicRight = new Rect(87, 177, 15, 8);

        const int maxNumListTopics = 13; // max number of items displayed in scrolling area of topics list

        const int maxHorizontalScrollIndex = 15;

        enum TalkOption { 
            TellMeAbout,
            WhereIs
        };
        TalkOption selectedTalkOption = TalkOption.TellMeAbout;

        enum TalkCategory
        {
            None,
            Location,
            People,
            Things,
            Work
        };
        TalkCategory selectedTalkCategory = TalkCategory.Location;

        enum TalkTone
        {
            Polite,
            Normal,
            Blunt
        };
        TalkTone selectedTalkTone = TalkTone.Polite;

        Texture2D textureBackground;
        Texture2D textureHighlightedOptions;
        Texture2D textureGrayedOutCategories;
        Texture2D texturePortrait;

        Panel panelNameNPC;
        TextLabel labelNameNPC = null;
        string nameNPC = "";

        Color[] textureTellMeAboutNormal;
        Color[] textureTellMeAboutHighlighted;
        Color[] textureWhereIsNormal;
        Color[] textureWhereIsHighlighted;

        Color[] textureCategoryLocationHighlighted;
        Color[] textureCategoryLocationGrayedOut;
        Color[] textureCategoryPeopleHighlighted;
        Color[] textureCategoryPeopleGrayedOut;
        Color[] textureCategoryThingsHighlighted;
        Color[] textureCategoryThingsGrayedOut;
        Color[] textureCategoryWorkHighlighted;
        Color[] textureCategoryWorkGrayedOut;

        Panel mainPanel;

        TextLabel pcSay;

        Panel panelTone; // used as selection marker
        Vector2 panelTonePolitePos = new Vector2(258, 18);
        Vector2 panelToneNormalPos = new Vector2(258, 28);
        Vector2 panelToneBluntPos = new Vector2(258, 38);
        Vector2 panelToneSize = new Vector2(6f, 6f);
        Color32 toggleColor = new Color32(162, 36, 12, 255);

        Rect rectButtonTonePolite = new Rect(258, 18, 6, 6);
        Rect rectButtonToneNormal = new Rect(258, 28, 6, 6);
        Rect rectButtonToneBlunt = new Rect(258, 38, 6, 6);

        Button buttonTellMeAbout;
        Button buttonWhereIs;
        Button buttonCategoryLocation;
        Button buttonCategoryPeople;
        Button buttonCategoryThings;
        Button buttonCategoryWork;
        Button buttonTonePolite;
        Button buttonToneNormal;
        Button buttonToneBlunt;

        ListBox listBoxTopicLocation;
        VerticalScrollBar verticalScrollBarTopicWindow;
        HorizontalSlider horizontalSliderTopicWindow;

        Rect upArrowRect = new Rect(0, 0, 9, 16);
        Rect downArrowRect = new Rect(0, 136, 9, 16);
        Texture2D redUpArrow;
        Texture2D greenUpArrow;
        Texture2D redDownArrow;
        Texture2D greenDownArrow;

        Texture2D redLeftArrow;
        Texture2D greenLeftArrow;
        Texture2D redRightArrow;
        Texture2D greenRightArrow;

        Button buttonTopicUp;
        Button buttonTopicDown;
        Button buttonTopicLeft;
        Button buttonTopicRight;

        Button buttonGoodbye;

        public DaggerfallTalkWindow(IUserInterfaceManager uiManager, DaggerfallBaseWindow previous = null)
            : base(uiManager, previous)
        {
        }

        protected override void Setup()
        {
            base.Setup();

            ImgFile imgFile = null;
            DFBitmap bitmap = null;

            ParentPanel.BackgroundColor = ScreenDimColor;

            // Load background texture of talk window
            imgFile = new ImgFile(Path.Combine(DaggerfallUnity.Instance.Arena2Path, talkWindowImgName), FileUsage.UseMemory, false);
            imgFile.LoadPalette(Path.Combine(DaggerfallUnity.Instance.Arena2Path, imgFile.PaletteName));
            bitmap = imgFile.GetDFBitmap();
            textureBackground = new Texture2D(bitmap.Width, bitmap.Height, TextureFormat.ARGB32, false);
            textureBackground.SetPixels32(imgFile.GetColor32(bitmap, 0));
            textureBackground.Apply(false, false); // make readable
            textureBackground.filterMode = DaggerfallUI.Instance.GlobalFilterMode;
            if (!textureBackground)
            {
                Debug.LogError(string.Format("Failed to load background image {0} for talk window", talkWindowImgName));
                CloseWindow();
                return;
            }

            mainPanel = DaggerfallUI.AddPanel(NativePanel, AutoSizeModes.None);
            mainPanel.BackgroundTexture = textureBackground;
            mainPanel.Size = new Vector2(textureBackground.width, textureBackground.height);
            mainPanel.HorizontalAlignment = HorizontalAlignment.Center;
            mainPanel.VerticalAlignment = VerticalAlignment.Middle;

            if (texturePortrait)
            {
                updatePortrait();
            }

            panelNameNPC = DaggerfallUI.AddPanel(mainPanel, AutoSizeModes.None);
            panelNameNPC.Position = new Vector2(117, 52);
            panelNameNPC.Size = new Vector2(197, 10);

            labelNameNPC = new TextLabel();
            labelNameNPC.Position = new Vector2(0, 0);
            labelNameNPC.Size = new Vector2(197, 10);
            labelNameNPC.Name = "label_npcName";
            labelNameNPC.MaxCharacters = 32;
            labelNameNPC.HorizontalAlignment = HorizontalAlignment.Center;
            labelNameNPC.VerticalAlignment = VerticalAlignment.Middle;
            panelNameNPC.Components.Add(labelNameNPC);

            updateNameNPC();

            // Load talk options highlight texture
            imgFile = new ImgFile(Path.Combine(DaggerfallUnity.Instance.Arena2Path, highlightedOptionsImgName), FileUsage.UseMemory, false);
            imgFile.LoadPalette(Path.Combine(DaggerfallUnity.Instance.Arena2Path, imgFile.PaletteName));
            bitmap = imgFile.GetDFBitmap();
            textureHighlightedOptions = new Texture2D(bitmap.Width, bitmap.Height, TextureFormat.ARGB32, false);
            textureHighlightedOptions.SetPixels32(imgFile.GetColor32(bitmap, 0));
            textureHighlightedOptions.Apply(false, false); // make readable
            textureHighlightedOptions.filterMode = DaggerfallUI.Instance.GlobalFilterMode;
            if (!textureHighlightedOptions)
            {
                Debug.LogError(string.Format("Failed to load highlighted options image {0} for talk window", highlightedOptionsImgName));
                CloseWindow();
                return;
            }

            // Load talk categories highlight texture
            imgFile = new ImgFile(Path.Combine(DaggerfallUnity.Instance.Arena2Path, talkCategoriesImgName), FileUsage.UseMemory, false);
            imgFile.LoadPalette(Path.Combine(DaggerfallUnity.Instance.Arena2Path, imgFile.PaletteName));
            bitmap = imgFile.GetDFBitmap();
            textureGrayedOutCategories = new Texture2D(bitmap.Width, bitmap.Height, TextureFormat.ARGB32, false);
            textureGrayedOutCategories.SetPixels32(imgFile.GetColor32(bitmap, 0));
            textureGrayedOutCategories.Apply(false, false); // make readable
            textureGrayedOutCategories.filterMode = DaggerfallUI.Instance.GlobalFilterMode;
            if (!textureGrayedOutCategories)
            {
                Debug.LogError(string.Format("Failed to load grayed-out categories image {0} for talk window", textureGrayedOutCategories));
                CloseWindow();
                return;
            }

            textureTellMeAboutNormal = textureBackground.GetPixels(4, textureBackground.height - 4 - 10, 107, 10);
            textureTellMeAboutHighlighted = textureHighlightedOptions.GetPixels(0, 10, 107, 10);
            textureWhereIsNormal = textureBackground.GetPixels(4, textureBackground.height - 14 - 10, 107, 10);
            textureWhereIsHighlighted = textureHighlightedOptions.GetPixels(0, 0, 107, 10);

            textureCategoryLocationHighlighted = textureBackground.GetPixels(4, textureBackground.height - 26 - 10, 107, 10);
            textureCategoryLocationGrayedOut = textureGrayedOutCategories.GetPixels(0, 30, 107, 10);
            textureCategoryPeopleHighlighted = textureBackground.GetPixels(4, textureBackground.height - 36 - 10, 107, 10);
            textureCategoryPeopleGrayedOut = textureGrayedOutCategories.GetPixels(0, 20, 107, 10);
            textureCategoryThingsHighlighted = textureBackground.GetPixels(4, textureBackground.height - 46 - 10, 107, 10);
            textureCategoryThingsGrayedOut = textureGrayedOutCategories.GetPixels(0, 10, 107, 10);
            textureCategoryWorkHighlighted = textureBackground.GetPixels(4, textureBackground.height - 56 - 10, 107, 10);
            textureCategoryWorkGrayedOut = textureGrayedOutCategories.GetPixels(0, 0, 107, 10);

            /*
            pcSay = new TextLabel();
            pcSay.Position = new Vector2(150, 14);
            pcSay.Size = new Vector2(60, 13);
            pcSay.Name = "accnt_total_label";
            pcSay.MaxCharacters = 13;
            mainPanel.Components.Add(pcSay);
            */

            buttonTellMeAbout = new Button();
            buttonTellMeAbout.Position = new Vector2(4, 4);
            buttonTellMeAbout.Size = new Vector2(107, 10);
            buttonTellMeAbout.Name = "button_tellmeabout";
            buttonTellMeAbout.OnMouseClick += buttonTellMeAbout_OnMouseClick;
            mainPanel.Components.Add(buttonTellMeAbout);

            buttonWhereIs = new Button();
            buttonWhereIs.Position = new Vector2(4, 14);
            buttonWhereIs.Size = new Vector2(107, 10);
            buttonWhereIs.Name = "button_whereis";
            buttonWhereIs.OnMouseClick += buttonWhereIs_OnMouseClick;
            mainPanel.Components.Add(buttonWhereIs);

            buttonCategoryLocation = new Button();
            buttonCategoryLocation.Position = new Vector2(4, 26);
            buttonCategoryLocation.Size = new Vector2(107, 10);
            buttonCategoryLocation.Name = "button_categoryLocation";
            buttonCategoryLocation.OnMouseClick += buttonCategoryLocation_OnMouseClick;
            mainPanel.Components.Add(buttonCategoryLocation);

            buttonCategoryPeople = new Button();
            buttonCategoryPeople.Position = new Vector2(4, 36);
            buttonCategoryPeople.Size = new Vector2(107, 10);
            buttonCategoryPeople.Name = "button_categoryPeople";
            buttonCategoryPeople.OnMouseClick += buttonCategoryPeople_OnMouseClick;
            mainPanel.Components.Add(buttonCategoryPeople);

            buttonCategoryThings = new Button();
            buttonCategoryThings.Position = new Vector2(4, 46);
            buttonCategoryThings.Size = new Vector2(107, 10);
            buttonCategoryThings.Name = "button_categoryThings";
            buttonCategoryThings.OnMouseClick += buttonCategoryThings_OnMouseClick;
            mainPanel.Components.Add(buttonCategoryThings);

            buttonCategoryWork = new Button();
            buttonCategoryWork.Position = new Vector2(4, 56);
            buttonCategoryWork.Size = new Vector2(107, 10);
            buttonCategoryWork.Name = "button_categoryWork";
            buttonCategoryWork.OnMouseClick += buttonCategoryWork_OnMouseClick;
            mainPanel.Components.Add(buttonCategoryWork);

            buttonTonePolite = DaggerfallUI.AddButton(rectButtonTonePolite, NativePanel);
            buttonTonePolite.OnMouseClick += buttonTonePolite_OnClickHandler;
            buttonToneNormal = DaggerfallUI.AddButton(rectButtonToneNormal, NativePanel);
            buttonToneNormal.OnMouseClick += buttonToneNormal_OnClickHandler;
            buttonToneBlunt = DaggerfallUI.AddButton(rectButtonToneBlunt, NativePanel);
            buttonToneBlunt.OnMouseClick += buttonToneBlunt_OnClickHandler;

            buttonGoodbye = new Button();
            buttonGoodbye.Position = new Vector2(118, 183);
            buttonGoodbye.Size = new Vector2(67, 10);
            buttonGoodbye.Name = "button_goodbye";
            buttonGoodbye.OnMouseClick += buttonGoodbye_OnMouseClick;
            mainPanel.Components.Add(buttonGoodbye);

            panelTone = DaggerfallUI.AddPanel(new Rect(panelTonePolitePos, panelToneSize), NativePanel);
            panelTone.BackgroundColor = toggleColor;

            listBoxTopicLocation = new ListBox();
            listBoxTopicLocation.Position = new Vector2(6, 71);
            listBoxTopicLocation.Size = new Vector2(94, 104);
            listBoxTopicLocation.RowsDisplayed = maxNumListTopics;
            listBoxTopicLocation.MaxCharacters = -1;
            listBoxTopicLocation.Name = "list_topic_location";
            listBoxTopicLocation.EnabledHorizontalScroll = true;
            listBoxTopicLocation.MaxHorizontalScrollIndex = maxHorizontalScrollIndex;
            //listBoxTopicLocation.OnMouseClick += listBoxTopicLocation_OnMouseClickHandler;
            mainPanel.Components.Add(listBoxTopicLocation);

            for (int i = 0; i < 50; i++)
            {
                listBoxTopicLocation.AddItem("location " + i + " test string");
            }

            // Cut out red up/down arrows
            Texture2D redArrowsTexture = ImageReader.GetTexture(redArrowsTextureName);
            redUpArrow = ImageReader.GetSubTexture(redArrowsTexture, upArrowRect);
            redDownArrow = ImageReader.GetSubTexture(redArrowsTexture, downArrowRect);

            // Cut out green up/down arrows
            Texture2D greenArrowsTexture = ImageReader.GetTexture(greenArrowsTextureName);
            greenUpArrow = ImageReader.GetSubTexture(greenArrowsTexture, upArrowRect);
            greenDownArrow = ImageReader.GetSubTexture(greenArrowsTexture, downArrowRect);

            Color32[] colors;
            Color32[] rotated;
            colors = redUpArrow.GetPixels32();
            rotated = ImageProcessing.RotateColors(ref colors, redUpArrow.width, redUpArrow.height);
            redLeftArrow = new Texture2D(redUpArrow.height, redUpArrow.width);
            redLeftArrow.SetPixels32(rotated);
            redLeftArrow.Apply(false);

            colors = redDownArrow.GetPixels32();
            rotated = ImageProcessing.RotateColors(ref colors, redUpArrow.width, redUpArrow.height);
            redRightArrow = new Texture2D(redUpArrow.height, redUpArrow.width);
            redRightArrow.SetPixels32(rotated);
            redRightArrow.Apply(false);

            colors = greenUpArrow.GetPixels32();
            rotated = ImageProcessing.RotateColors(ref colors, greenUpArrow.width, greenUpArrow.height);
            greenLeftArrow = new Texture2D(greenUpArrow.height, greenUpArrow.width);
            greenLeftArrow.SetPixels32(rotated);
            greenLeftArrow.Apply(false);

            colors = greenDownArrow.GetPixels32();
            rotated = ImageProcessing.RotateColors(ref colors, greenDownArrow.width, greenDownArrow.height);
            greenRightArrow = new Texture2D(greenDownArrow.height, greenDownArrow.width);
            greenRightArrow.SetPixels32(rotated);
            greenRightArrow.Apply(false);

            SetupScrollBars();
            SetupScrollButtons();

            UpdateLabels();
            UpdateButtons();
        }

        void SetupScrollBars()
        {
            // Local items list scroll bar (e.g. items in character inventory)
            verticalScrollBarTopicWindow = new VerticalScrollBar();
            verticalScrollBarTopicWindow.Position = new Vector2(104, 87);
            verticalScrollBarTopicWindow.Size = new Vector2(5, 73);
            verticalScrollBarTopicWindow.DisplayUnits = Math.Min(maxNumListTopics, listBoxTopicLocation.Count);
            verticalScrollBarTopicWindow.TotalUnits = listBoxTopicLocation.Count;
            verticalScrollBarTopicWindow.OnScroll += verticalScrollBarTopicWindow_OnScroll;
            NativePanel.Components.Add(verticalScrollBarTopicWindow);

            horizontalSliderTopicWindow = new HorizontalSlider();
            horizontalSliderTopicWindow.Position = new Vector2(21, 178);
            horizontalSliderTopicWindow.Size = new Vector2(62, 5);
            horizontalSliderTopicWindow.DisplayUnits = 11;
            horizontalSliderTopicWindow.TotalUnits = maxHorizontalScrollIndex;
            horizontalSliderTopicWindow.OnScroll += horizontalSliderTopicWindow_OnScroll;
            NativePanel.Components.Add(horizontalSliderTopicWindow);
        }

        void SetupScrollButtons()
        {
            buttonTopicUp = DaggerfallUI.AddButton(rectButtonTopicUp, NativePanel);
            buttonTopicUp.BackgroundTexture = redUpArrow;
            buttonTopicUp.OnMouseClick += ButtonTopicUp_OnMouseClick;

            buttonTopicDown = DaggerfallUI.AddButton(rectButtonTopicDown, NativePanel);
            buttonTopicDown.BackgroundTexture = redDownArrow;
            buttonTopicDown.OnMouseClick += ButtonTopicDown_OnMouseClick;

            buttonTopicLeft = DaggerfallUI.AddButton(rectButtonTopicLeft, NativePanel);
            buttonTopicLeft.BackgroundTexture = redLeftArrow;
            buttonTopicLeft.OnMouseClick += ButtonTopicLeft_OnMouseClick;

            buttonTopicRight = DaggerfallUI.AddButton(rectButtonTopicRight, NativePanel);
            buttonTopicRight.BackgroundTexture = redRightArrow;
            buttonTopicRight.OnMouseClick += ButtonTopicRight_OnMouseClick;

            int scrollIndex = GetSafeScrollIndex(verticalScrollBarTopicWindow);
            // Update scroller buttons
            UpdateListScrollerButtons(scrollIndex, listBoxTopicLocation.Count, buttonTopicUp, buttonTopicDown);

            int horizontalScrollIndex = horizontalSliderTopicWindow.ScrollIndex;
            // Update scroller buttons
            UpdateListScrollerButtonsLeftRight(horizontalScrollIndex, maxHorizontalScrollIndex + 1, buttonTopicLeft, buttonTopicRight);
        }

        public override void OnPush()
        {
            base.OnPush();

            // Reset scrollbars
            if (verticalScrollBarTopicWindow != null)
                verticalScrollBarTopicWindow.ScrollIndex = 0;
        }

        public override void OnPop()
        {
            base.OnPop();
        }

        public override void Update()
        {
            base.Update();

            UpdateLabels();
        }

        public void setNPCPortraitAndName(int recordId, string name)
        {
            // Load npc portrait           
            CifRciFile rciFile = new CifRciFile(Path.Combine(DaggerfallUnity.Instance.Arena2Path, portraitImgName), FileUsage.UseMemory, false);
            rciFile.LoadPalette(Path.Combine(DaggerfallUnity.Instance.Arena2Path, rciFile.PaletteName));
            DFBitmap bitmap = rciFile.GetDFBitmap(recordId, 0);
            texturePortrait = new Texture2D(bitmap.Width, bitmap.Height, TextureFormat.ARGB32, false);
            texturePortrait.SetPixels32(rciFile.GetColor32(bitmap, 0));
            texturePortrait.Apply(false, false); // make readable
            texturePortrait.filterMode = DaggerfallUI.Instance.GlobalFilterMode;
            if (!texturePortrait)
            {
                Debug.LogError(string.Format("Failed to load portrait image {0} for talk window", texturePortrait));
                CloseWindow();
                return;
            }

            updatePortrait();

            nameNPC = name;
            updateNameNPC();
        }

        void updatePortrait()
        {
            if (textureBackground)
            {
                textureBackground.SetPixels(119, textureBackground.height - 65 - 64, 64, 64, texturePortrait.GetPixels());
                textureBackground.Apply(false);
            }
        }

        void updateNameNPC()
        {
            if (labelNameNPC != null)
            {
                labelNameNPC.Text = nameNPC;
            }
        }

        void UpdateLabels()
        {

        }

        void UpdateButtons()
        {
            // update talk option selection and talk category selection
            switch (selectedTalkOption)
            {
                case TalkOption.TellMeAbout:
                default:
                    setTalkModeTellMeAbout();
                    setTalkCategoryNone();
                    break;
                case TalkOption.WhereIs:
                    setTalkModeWhereIs();
                    switch (selectedTalkCategory)
                    {
                        case TalkCategory.Location:
                            setTalkCategoryLocation();
                            break;
                        case TalkCategory.People:
                            setTalkCategoryPeople();
                            break;
                        case TalkCategory.Things:
                            setTalkCategoryThings();
                            break;
                        case TalkCategory.Work:
                            setTalkCategoryWork();
                            break;
                        default:
                            setTalkCategoryNone();
                            break;
                    }
                    break;
            }

            //update tone selection
            switch (selectedTalkTone)
            {
                case TalkTone.Polite:
                default:
                    panelTone.Position = panelTonePolitePos;
                    break;
                case TalkTone.Normal:
                    panelTone.Position = panelToneNormalPos;
                    break;
                case TalkTone.Blunt:
                    panelTone.Position = panelToneBluntPos;
                    break;
            }

        }

        void setTalkModeTellMeAbout()
        {
            textureBackground.SetPixels(4, textureBackground.height - 4 - 10, 107, 10, textureTellMeAboutHighlighted);
            textureBackground.SetPixels(4, textureBackground.height - 14 - 10, 107, 10, textureWhereIsNormal);
            textureBackground.Apply(false);
        }

        void setTalkModeWhereIs()
        {
            textureBackground.SetPixels(4, textureBackground.height - 4 - 10, 107, 10, textureTellMeAboutNormal);
            textureBackground.SetPixels(4, textureBackground.height - 14 - 10, 107, 10, textureWhereIsHighlighted);
            textureBackground.Apply(false);
        }

        void setTalkCategoryNone()
        {
            textureBackground.SetPixels(4, textureBackground.height - 26 - 10, 107, 10, textureCategoryLocationGrayedOut);
            textureBackground.SetPixels(4, textureBackground.height - 36 - 10, 107, 10, textureCategoryPeopleGrayedOut);
            textureBackground.SetPixels(4, textureBackground.height - 46 - 10, 107, 10, textureCategoryThingsGrayedOut);
            textureBackground.SetPixels(4, textureBackground.height - 56 - 10, 107, 10, textureCategoryWorkGrayedOut);
            textureBackground.Apply(false);
        }

        void setTalkCategoryLocation()
        {
            textureBackground.SetPixels(4, textureBackground.height - 26 - 10, 107, 10, textureCategoryLocationHighlighted);
            textureBackground.SetPixels(4, textureBackground.height - 36 - 10, 107, 10, textureCategoryPeopleGrayedOut);
            textureBackground.SetPixels(4, textureBackground.height - 46 - 10, 107, 10, textureCategoryThingsGrayedOut);
            textureBackground.SetPixels(4, textureBackground.height - 56 - 10, 107, 10, textureCategoryWorkGrayedOut);
            textureBackground.Apply(false);
        }

        void setTalkCategoryPeople()
        {
            textureBackground.SetPixels(4, textureBackground.height - 26 - 10, 107, 10, textureCategoryLocationGrayedOut);
            textureBackground.SetPixels(4, textureBackground.height - 36 - 10, 107, 10, textureCategoryPeopleHighlighted);
            textureBackground.SetPixels(4, textureBackground.height - 46 - 10, 107, 10, textureCategoryThingsGrayedOut);
            textureBackground.SetPixels(4, textureBackground.height - 56 - 10, 107, 10, textureCategoryWorkGrayedOut);
            textureBackground.Apply(false);
        }

        void setTalkCategoryThings()
        {
            textureBackground.SetPixels(4, textureBackground.height - 26 - 10, 107, 10, textureCategoryLocationGrayedOut);
            textureBackground.SetPixels(4, textureBackground.height - 36 - 10, 107, 10, textureCategoryPeopleGrayedOut);
            textureBackground.SetPixels(4, textureBackground.height - 46 - 10, 107, 10, textureCategoryThingsHighlighted);
            textureBackground.SetPixels(4, textureBackground.height - 56 - 10, 107, 10, textureCategoryWorkGrayedOut);
            textureBackground.Apply(false);
        }

        void setTalkCategoryWork()
        {
            textureBackground.SetPixels(4, textureBackground.height - 26 - 10, 107, 10, textureCategoryLocationGrayedOut);
            textureBackground.SetPixels(4, textureBackground.height - 36 - 10, 107, 10, textureCategoryPeopleGrayedOut);
            textureBackground.SetPixels(4, textureBackground.height - 46 - 10, 107, 10, textureCategoryThingsGrayedOut);
            textureBackground.SetPixels(4, textureBackground.height - 56 - 10, 107, 10, textureCategoryWorkHighlighted);
            textureBackground.Apply(false);
        }

        /// <summary>
        /// Gets safe scroll index.
        /// Scroller will be adjust to always be inside display range where possible.
        /// </summary>
        int GetSafeScrollIndex(VerticalScrollBar scroller)
        {
            // Get current scroller index
            int scrollIndex = scroller.ScrollIndex;
            if (scrollIndex < 0)
                scrollIndex = 0;

            // Ensure scroll index within current range
            if (scrollIndex + scroller.DisplayUnits > scroller.TotalUnits)
            {
                scrollIndex = scroller.TotalUnits - scroller.DisplayUnits;
                if (scrollIndex < 0) scrollIndex = 0;
                scroller.Reset(scroller.DisplayUnits, scroller.TotalUnits, scrollIndex);
            }

            return scrollIndex;
        }

        // Updates red/green state of scroller buttons
        void UpdateListScrollerButtons(int index, int count, Button upButton, Button downButton)
        {
            // Update up button
            if (index > 0)
                upButton.BackgroundTexture = greenUpArrow;
            else
                upButton.BackgroundTexture = redUpArrow;

            // Update down button
            if (index < (count - verticalScrollBarTopicWindow.DisplayUnits))
                downButton.BackgroundTexture = greenDownArrow;
            else
                downButton.BackgroundTexture = redDownArrow;

            // No items above or below
            if (count <= verticalScrollBarTopicWindow.DisplayUnits)
            {
                upButton.BackgroundTexture = redUpArrow;
                downButton.BackgroundTexture = redDownArrow;
            }
        }

        // Updates red/green state of left/right scroller buttons
        void UpdateListScrollerButtonsLeftRight(int index, int count, Button leftButton, Button rightButton)
        {
            // Update up button
            if (index > 0)
                leftButton.BackgroundTexture = greenLeftArrow;
            else
                leftButton.BackgroundTexture = redLeftArrow;

            // Update down button
            if (index < (count - horizontalSliderTopicWindow.DisplayUnits))
                rightButton.BackgroundTexture = greenRightArrow;
            else
                rightButton.BackgroundTexture = redRightArrow;

            // No items above or below
            if (count <= horizontalSliderTopicWindow.DisplayUnits)
            {
                leftButton.BackgroundTexture = redLeftArrow;
                rightButton.BackgroundTexture = redRightArrow;
            }
        }

        #region event handlers

        private void verticalScrollBarTopicWindow_OnScroll()
        {
            // Update scroller
            verticalScrollBarTopicWindow.TotalUnits = listBoxTopicLocation.Count;
            int scrollIndex = GetSafeScrollIndex(verticalScrollBarTopicWindow);
            
            // Update scroller buttons
            UpdateListScrollerButtons(scrollIndex, listBoxTopicLocation.Count, buttonTopicUp, buttonTopicDown);

            listBoxTopicLocation.ScrollIndex = scrollIndex;
            listBoxTopicLocation.Update();
        }

        private void horizontalSliderTopicWindow_OnScroll()
        {
            // Update scroller
            horizontalSliderTopicWindow.TotalUnits = maxHorizontalScrollIndex;
            int horizontalScrollIndex = horizontalSliderTopicWindow.ScrollIndex;

            // Update scroller buttons
            UpdateListScrollerButtonsLeftRight(horizontalScrollIndex, maxHorizontalScrollIndex, buttonTopicLeft, buttonTopicRight);

            listBoxTopicLocation.HorizontalScrollIndex = horizontalScrollIndex;
            listBoxTopicLocation.Update();
        }

        private void ButtonTopicUp_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            verticalScrollBarTopicWindow.ScrollIndex--;
        }

        private void ButtonTopicDown_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            verticalScrollBarTopicWindow.ScrollIndex++;
        }

        private void ButtonTopicLeft_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            horizontalSliderTopicWindow.ScrollIndex--;
        }

        private void ButtonTopicRight_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            horizontalSliderTopicWindow.ScrollIndex++;
        }

        private void buttonTellMeAbout_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            selectedTalkOption = TalkOption.TellMeAbout;
            UpdateButtons();
        }

        private void buttonWhereIs_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            selectedTalkOption = TalkOption.WhereIs;
            UpdateButtons();
        }

        private void buttonCategoryLocation_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (selectedTalkOption == TalkOption.WhereIs)
            {
                selectedTalkCategory = TalkCategory.Location;
                UpdateButtons();
            }
        }

        private void buttonCategoryPeople_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (selectedTalkOption == TalkOption.WhereIs)
            {
                selectedTalkCategory = TalkCategory.People;
                UpdateButtons();
            }
        }

        private void buttonCategoryThings_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (selectedTalkOption == TalkOption.WhereIs)
            {
                selectedTalkCategory = TalkCategory.Things;
                UpdateButtons();
            }
        }

        private void buttonCategoryWork_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (selectedTalkOption == TalkOption.WhereIs)
            {
                selectedTalkCategory = TalkCategory.Work;
                UpdateButtons();
            }
        }

        private void buttonTonePolite_OnClickHandler(BaseScreenComponent sender, Vector2 position)
        {
            selectedTalkTone = TalkTone.Polite;
            UpdateButtons();
        }

        private void buttonToneNormal_OnClickHandler(BaseScreenComponent sender, Vector2 position)
        {
            selectedTalkTone = TalkTone.Normal;
            UpdateButtons();
        }

        private void buttonToneBlunt_OnClickHandler(BaseScreenComponent sender, Vector2 position)
        {
            selectedTalkTone = TalkTone.Blunt;
            UpdateButtons();
        }

        private void buttonGoodbye_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            CloseWindow();
        }

        #endregion
    }
}
