// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2016 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Michael Rauter (Nystul)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.UserInterfaceWindows;

using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Utility;


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

        const int maxNumTopicsShown = 13; // max number of items displayed in scrolling area of topics list
        const int maxNumCharactersOfTopicShown = 20; // max number of characters of a topic displayed in scrolling area of topics list

        const int maxNumAnswerLinesShown = 15; // max number of lines displayed in scrolling area of answers

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

        // alignment stuff for checkbox buttons
        Panel panelTone; // used as selection marker
        Vector2 panelTonePolitePos = new Vector2(258, 18);
        Vector2 panelToneNormalPos = new Vector2(258, 28);
        Vector2 panelToneBluntPos = new Vector2(258, 38);
        Vector2 panelToneSize = new Vector2(6f, 6f);
        Color32 toggleColor = new Color32(162, 36, 12, 255);

        // positioning rects for checkbox buttons
        Rect rectButtonTonePolite = new Rect(258, 18, 6, 6);
        Rect rectButtonToneNormal = new Rect(258, 28, 6, 6);
        Rect rectButtonToneBlunt = new Rect(258, 38, 6, 6);

        // normal buttons
        Button buttonTellMeAbout;
        Button buttonWhereIs;
        Button buttonCategoryLocation;
        Button buttonCategoryPeople;
        Button buttonCategoryThings;
        Button buttonCategoryWork;
        Button buttonTopicUp;
        Button buttonTopicDown;
        Button buttonTopicLeft;
        Button buttonTopicRight;
        Button buttonOkay;
        Button buttonGoodbye;

        // checkbox buttons
        Button buttonCheckboxTonePolite;
        Button buttonCheckboxToneNormal;
        Button buttonCheckboxToneBlunt;

        // topic listbox and layout, scrollbar/slider and parameters
        ListBox listboxTopic;
        Rect rectButtonTopicUp = new Rect(102, 68, 9, 16);
        Rect rectButtonTopicDown = new Rect(102, 161, 9, 16);
        Rect rectButtonTopicLeft = new Rect(5, 177, 16, 8);
        Rect rectButtonTopicRight = new Rect(87, 177, 16, 8);
        VerticalScrollBar verticalScrollBarTopicWindow;
        HorizontalSlider horizontalSliderTopicWindow;
        int lengthOfLongestItemInListBox;

        // answers listbox
        ListBox listboxAnswers;

        // green/red arrow buttons (parameters for creation and textures)
        Rect upArrowRectInSrcImg = new Rect(0, 0, 9, 16);
        Rect downArrowRectInSrcImg = new Rect(0, 136, 9, 16);
        Texture2D arrowTopicUpRed;
        Texture2D arrowTopicUpGreen;
        Texture2D arrowTopicDownRed;
        Texture2D arrowTopicDownGreen;
        Texture2D arrowTopicLeftRed;
        Texture2D arrowTopicLeftGreen;
        Texture2D arrowTopicRightRed;
        Texture2D arrowTopicRightGreen;

        public DaggerfallTalkWindow(IUserInterfaceManager uiManager, DaggerfallBaseWindow previous = null)
            : base(uiManager, previous)
        {
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

            UpdatePortrait();

            nameNPC = name;
            UpdateNameNPC();
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
                UpdatePortrait();
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

            UpdateNameNPC();

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

            listboxTopic = new ListBox();
            listboxTopic.Position = new Vector2(6, 71);
            listboxTopic.Size = new Vector2(94, 104);
            listboxTopic.RowsDisplayed = maxNumTopicsShown;
            listboxTopic.MaxCharacters = -1;
            listboxTopic.Name = "list_topic";
            listboxTopic.EnabledHorizontalScroll = true;
            //SetListItems(ref listboxTopic, ref listTopicLocation);
            //listboxTopic.OnMouseClick += listboxTopic_OnMouseClickHandler;
            mainPanel.Components.Add(listboxTopic);

            // Cut out red up/down arrows
            Texture2D redArrowsTexture = ImageReader.GetTexture(redArrowsTextureName);
            arrowTopicUpRed = ImageReader.GetSubTexture(redArrowsTexture, upArrowRectInSrcImg);
            arrowTopicDownRed = ImageReader.GetSubTexture(redArrowsTexture, downArrowRectInSrcImg);

            // Cut out green up/down arrows
            Texture2D greenArrowsTexture = ImageReader.GetTexture(greenArrowsTextureName);
            arrowTopicUpGreen = ImageReader.GetSubTexture(greenArrowsTexture, upArrowRectInSrcImg);
            arrowTopicDownGreen = ImageReader.GetSubTexture(greenArrowsTexture, downArrowRectInSrcImg);

            Color32[] colors;
            Color32[] rotated;
            colors = arrowTopicDownRed.GetPixels32();
            rotated = ImageProcessing.RotateColors(ref colors, arrowTopicUpRed.height, arrowTopicUpRed.width);
            arrowTopicLeftRed = new Texture2D(arrowTopicUpRed.height, arrowTopicUpRed.width, TextureFormat.ARGB32, false);
            arrowTopicLeftRed.SetPixels32(ImageProcessing.FlipHorizontallyColors(ref rotated, arrowTopicLeftRed.width, arrowTopicLeftRed.height), 0);
            arrowTopicLeftRed.Apply(false);
            arrowTopicLeftRed.filterMode = DaggerfallUI.Instance.GlobalFilterMode;

            colors = arrowTopicUpRed.GetPixels32();
            rotated = ImageProcessing.RotateColors(ref colors, arrowTopicDownRed.height, arrowTopicDownRed.width);
            arrowTopicRightRed = new Texture2D(arrowTopicUpRed.height, arrowTopicUpRed.width, TextureFormat.ARGB32, false);
            arrowTopicRightRed.SetPixels32(ImageProcessing.FlipHorizontallyColors(ref rotated, arrowTopicRightRed.width, arrowTopicRightRed.height));
            arrowTopicRightRed.Apply(false);
            arrowTopicRightRed.filterMode = DaggerfallUI.Instance.GlobalFilterMode;

            colors = arrowTopicDownGreen.GetPixels32();
            rotated = ImageProcessing.RotateColors(ref colors, arrowTopicUpGreen.height, arrowTopicUpGreen.width);
            arrowTopicLeftGreen = new Texture2D(arrowTopicUpGreen.height, arrowTopicUpGreen.width, TextureFormat.ARGB32, false);
            arrowTopicLeftGreen.SetPixels32(ImageProcessing.FlipHorizontallyColors(ref rotated, arrowTopicLeftGreen.width, arrowTopicLeftGreen.height));
            arrowTopicLeftGreen.Apply(false);
            arrowTopicLeftGreen.filterMode = DaggerfallUI.Instance.GlobalFilterMode;

            colors = arrowTopicUpGreen.GetPixels32();
            rotated = ImageProcessing.RotateColors(ref colors, arrowTopicDownGreen.height, arrowTopicDownGreen.width);
            arrowTopicRightGreen = new Texture2D(arrowTopicDownGreen.height, arrowTopicDownGreen.width, TextureFormat.ARGB32, false);
            arrowTopicRightGreen.SetPixels32(ImageProcessing.FlipHorizontallyColors(ref rotated, arrowTopicRightGreen.width, arrowTopicRightGreen.height));
            arrowTopicRightGreen.Apply(false);
            arrowTopicRightGreen.filterMode = DaggerfallUI.Instance.GlobalFilterMode;

            listboxAnswers = new ListBox();
            listboxAnswers.Position = new Vector2(188, 64);
            listboxAnswers.Size = new Vector2(115, 128);
            //listboxAnswers.RowSpacing = 1;
            listboxAnswers.RowsDisplayed = maxNumAnswerLinesShown;
            listboxAnswers.MaxCharacters = -1; // text is wrapped, so no max characters defined
            listboxAnswers.Name = "list_answers";
            listboxAnswers.WrapTextItems = true;
            mainPanel.Components.Add(listboxAnswers);

            SetupButtons();
            SetupCheckboxes();
            SetupScrollBars();
            SetupScrollButtons();

            SetTalkModeTellMeAbout();

            //UpdateButtonState();
            UpdateCheckboxes();
            UpdateScrollBars();
            UpdateScrollButtons();

            UpdateLabels();
        }

        void SetupButtons()
        {
            buttonTellMeAbout = new Button();
            buttonTellMeAbout.Position = new Vector2(4, 4);
            buttonTellMeAbout.Size = new Vector2(107, 10);
            buttonTellMeAbout.Name = "button_tellmeabout";
            buttonTellMeAbout.OnMouseClick += ButtonTellMeAbout_OnMouseClick;
            mainPanel.Components.Add(buttonTellMeAbout);

            buttonWhereIs = new Button();
            buttonWhereIs.Position = new Vector2(4, 14);
            buttonWhereIs.Size = new Vector2(107, 10);
            buttonWhereIs.Name = "button_whereis";
            buttonWhereIs.OnMouseClick += ButtonWhereIs_OnMouseClick;
            mainPanel.Components.Add(buttonWhereIs);

            buttonCategoryLocation = new Button();
            buttonCategoryLocation.Position = new Vector2(4, 26);
            buttonCategoryLocation.Size = new Vector2(107, 10);
            buttonCategoryLocation.Name = "button_categoryLocation";
            buttonCategoryLocation.OnMouseClick += ButtonCategoryLocation_OnMouseClick;
            mainPanel.Components.Add(buttonCategoryLocation);

            buttonCategoryPeople = new Button();
            buttonCategoryPeople.Position = new Vector2(4, 36);
            buttonCategoryPeople.Size = new Vector2(107, 10);
            buttonCategoryPeople.Name = "button_categoryPeople";
            buttonCategoryPeople.OnMouseClick += ButtonCategoryPeople_OnMouseClick;
            mainPanel.Components.Add(buttonCategoryPeople);

            buttonCategoryThings = new Button();
            buttonCategoryThings.Position = new Vector2(4, 46);
            buttonCategoryThings.Size = new Vector2(107, 10);
            buttonCategoryThings.Name = "button_categoryThings";
            buttonCategoryThings.OnMouseClick += ButtonCategoryThings_OnMouseClick;
            mainPanel.Components.Add(buttonCategoryThings);

            buttonCategoryWork = new Button();
            buttonCategoryWork.Position = new Vector2(4, 56);
            buttonCategoryWork.Size = new Vector2(107, 10);
            buttonCategoryWork.Name = "button_categoryWork";
            buttonCategoryWork.OnMouseClick += ButtonCategoryWork_OnMouseClick;
            mainPanel.Components.Add(buttonCategoryWork);

            buttonOkay = new Button();
            buttonOkay.Position = new Vector2(4, 186);
            buttonOkay.Size = new Vector2(107, 10);
            buttonOkay.Name = "button_okay";
            buttonOkay.OnMouseClick += ButtonOkay_OnMouseClick;
            mainPanel.Components.Add(buttonOkay);

            buttonGoodbye = new Button();
            buttonGoodbye.Position = new Vector2(118, 183);
            buttonGoodbye.Size = new Vector2(67, 10);
            buttonGoodbye.Name = "button_goodbye";
            buttonGoodbye.OnMouseClick += ButtonGoodbye_OnMouseClick;
            mainPanel.Components.Add(buttonGoodbye);
        }

        void SetupCheckboxes()
        {
            buttonCheckboxTonePolite = DaggerfallUI.AddButton(rectButtonTonePolite, NativePanel);
            buttonCheckboxTonePolite.OnMouseClick += ButtonTonePolite_OnClickHandler;
            buttonCheckboxToneNormal = DaggerfallUI.AddButton(rectButtonToneNormal, NativePanel);
            buttonCheckboxToneNormal.OnMouseClick += ButtonToneNormal_OnClickHandler;
            buttonCheckboxToneBlunt = DaggerfallUI.AddButton(rectButtonToneBlunt, NativePanel);
            buttonCheckboxToneBlunt.OnMouseClick += ButtonToneBlunt_OnClickHandler;

            panelTone = DaggerfallUI.AddPanel(new Rect(panelTonePolitePos, panelToneSize), NativePanel);
            panelTone.BackgroundColor = toggleColor;
        }

        void SetupScrollBars()
        {
            // Local items list scroll bar (e.g. items in character inventory)
            verticalScrollBarTopicWindow = new VerticalScrollBar();
            verticalScrollBarTopicWindow.Position = new Vector2(104, 87);
            verticalScrollBarTopicWindow.Size = new Vector2(5, 73);
            verticalScrollBarTopicWindow.OnScroll += VerticalScrollBarTopicWindow_OnScroll;
            NativePanel.Components.Add(verticalScrollBarTopicWindow);

            horizontalSliderTopicWindow = new HorizontalSlider();
            horizontalSliderTopicWindow.Position = new Vector2(22, 178);
            horizontalSliderTopicWindow.Size = new Vector2(62, 5);
            horizontalSliderTopicWindow.OnScroll += HorizontalSliderTopicWindow_OnScroll;
            NativePanel.Components.Add(horizontalSliderTopicWindow);
        }

        void SetupScrollButtons()
        {
            buttonTopicUp = DaggerfallUI.AddButton(rectButtonTopicUp, NativePanel);
            buttonTopicUp.BackgroundTexture = arrowTopicUpRed;
            buttonTopicUp.OnMouseClick += ButtonTopicUp_OnMouseClick;

            buttonTopicDown = DaggerfallUI.AddButton(rectButtonTopicDown, NativePanel);
            buttonTopicDown.BackgroundTexture = arrowTopicDownRed;
            buttonTopicDown.OnMouseClick += ButtonTopicDown_OnMouseClick;

            buttonTopicLeft = DaggerfallUI.AddButton(rectButtonTopicLeft, NativePanel);
            buttonTopicLeft.BackgroundTexture = arrowTopicLeftRed;
            buttonTopicLeft.OnMouseClick += ButtonTopicLeft_OnMouseClick;

            buttonTopicRight = DaggerfallUI.AddButton(rectButtonTopicRight, NativePanel);
            buttonTopicRight.BackgroundTexture = arrowTopicRightRed;
            buttonTopicRight.OnMouseClick += ButtonTopicRight_OnMouseClick;
        }

        void UpdateScrollBars()
        {
            verticalScrollBarTopicWindow.DisplayUnits = Math.Min(maxNumTopicsShown, listboxTopic.Count);
            verticalScrollBarTopicWindow.TotalUnits = listboxTopic.Count;
            verticalScrollBarTopicWindow.ScrollIndex = 0;
            verticalScrollBarTopicWindow.Update();

            horizontalSliderTopicWindow.DisplayUnits = maxNumCharactersOfTopicShown;
            horizontalSliderTopicWindow.TotalUnits = lengthOfLongestItemInListBox;
            horizontalSliderTopicWindow.ScrollIndex = 0;
            horizontalSliderTopicWindow.Update();
        }

        void UpdateScrollButtons()
        {
            int scrollIndex = GetSafeScrollIndex(verticalScrollBarTopicWindow);
            // Update scroller buttons
            UpdateListScrollerButtons(scrollIndex, listboxTopic.Count, buttonTopicUp, buttonTopicDown);
            buttonTopicUp.Update();
            buttonTopicDown.Update();

            int horizontalScrollIndex = horizontalSliderTopicWindow.ScrollIndex;
            // Update scroller buttons
            UpdateListScrollerButtonsLeftRight(horizontalScrollIndex, lengthOfLongestItemInListBox, buttonTopicLeft, buttonTopicRight);
            buttonTopicLeft.Update();
            buttonTopicRight.Update();
        }

        void SetListboxTopics(ref ListBox listboxTopic, List<string> listTopicLocation)
        {
            listboxTopic.ClearItems();
            for (int i = 0; i < listTopicLocation.Count; i++)
            {
                listboxTopic.AddItem(listTopicLocation[i]);
            }

            // compute length of longest item in listbox from current list items...
            lengthOfLongestItemInListBox = listboxTopic.LengthOfLongestItem();
            // update listboxTopic.MaxHorizontalScrollIndex            
            listboxTopic.MaxHorizontalScrollIndex = Math.Max(0, lengthOfLongestItemInListBox - maxNumCharactersOfTopicShown);
        }

        void ClearListboxTopics()
        {
            listboxTopic.ClearItems();
            lengthOfLongestItemInListBox = 0;
            listboxTopic.MaxHorizontalScrollIndex = 0;
        }

        void UpdatePortrait()
        {
            if (textureBackground)
            {
                textureBackground.SetPixels(119, textureBackground.height - 65 - 64, 64, 64, texturePortrait.GetPixels());
                textureBackground.Apply(false);
            }
        }

        void UpdateNameNPC()
        {
            if (labelNameNPC != null)
            {
                labelNameNPC.Text = nameNPC;
            }
        }

        void UpdateLabels()
        {

        }

        void UpdateCheckboxes()
        {
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

        void SetTalkModeTellMeAbout()
        {
            selectedTalkOption = TalkOption.TellMeAbout;
            
            textureBackground.SetPixels(4, textureBackground.height - 4 - 10, 107, 10, textureTellMeAboutHighlighted);
            textureBackground.SetPixels(4, textureBackground.height - 14 - 10, 107, 10, textureWhereIsNormal);
            textureBackground.SetPixels(4, textureBackground.height - 26 - 10, 107, 10, textureCategoryLocationGrayedOut);
            textureBackground.SetPixels(4, textureBackground.height - 36 - 10, 107, 10, textureCategoryPeopleGrayedOut);
            textureBackground.SetPixels(4, textureBackground.height - 46 - 10, 107, 10, textureCategoryThingsGrayedOut);
            textureBackground.SetPixels(4, textureBackground.height - 56 - 10, 107, 10, textureCategoryWorkGrayedOut);
            textureBackground.Apply(false);

            ClearListboxTopics();
            listboxTopic.Update();

            UpdateScrollBars();
            UpdateScrollButtons();
        }

        void SetTalkModeWhereIs()
        {
            selectedTalkOption = TalkOption.WhereIs;

            textureBackground.SetPixels(4, textureBackground.height - 4 - 10, 107, 10, textureTellMeAboutNormal);
            textureBackground.SetPixels(4, textureBackground.height - 14 - 10, 107, 10, textureWhereIsHighlighted);            
            textureBackground.Apply(false);

            SetTalkCategory(selectedTalkCategory);

            UpdateScrollBars();
            UpdateScrollButtons();
        }

        void SetTalkCategory(TalkCategory talkCategory)
        {
            switch (talkCategory)
            {
                case TalkCategory.Location:
                default:
                    SetTalkCategoryLocation();
                    break;
                case TalkCategory.People:
                    SetTalkCategoryPeople();
                    break;
                case TalkCategory.Things:
                    SetTalkCategoryThings();
                    break;
                case TalkCategory.Work:
                    SetTalkCategoryWork();
                    break;
                case TalkCategory.None:
                    SetTalkCategoryNone();
                    break;
            }
        }

        void SetTalkCategoryNone()
        {
            selectedTalkCategory = TalkCategory.None;

            textureBackground.SetPixels(4, textureBackground.height - 26 - 10, 107, 10, textureCategoryLocationGrayedOut);
            textureBackground.SetPixels(4, textureBackground.height - 36 - 10, 107, 10, textureCategoryPeopleGrayedOut);
            textureBackground.SetPixels(4, textureBackground.height - 46 - 10, 107, 10, textureCategoryThingsGrayedOut);
            textureBackground.SetPixels(4, textureBackground.height - 56 - 10, 107, 10, textureCategoryWorkGrayedOut);
            textureBackground.Apply(false);

            ClearListboxTopics();
            listboxTopic.Update();

            UpdateScrollBars();
            UpdateScrollButtons();
        }

        void SetTalkCategoryLocation()
        {
            selectedTalkCategory = TalkCategory.Location;

            textureBackground.SetPixels(4, textureBackground.height - 26 - 10, 107, 10, textureCategoryLocationHighlighted);
            textureBackground.SetPixels(4, textureBackground.height - 36 - 10, 107, 10, textureCategoryPeopleGrayedOut);
            textureBackground.SetPixels(4, textureBackground.height - 46 - 10, 107, 10, textureCategoryThingsGrayedOut);
            textureBackground.SetPixels(4, textureBackground.height - 56 - 10, 107, 10, textureCategoryWorkGrayedOut);
            textureBackground.Apply(false);

            SetListboxTopics(ref listboxTopic, TalkManager.Instance.ListTopicLocation);
            listboxTopic.Update();

            UpdateScrollBars();
            UpdateScrollButtons();
        }

        void SetTalkCategoryPeople()
        {
            selectedTalkCategory = TalkCategory.People;

            textureBackground.SetPixels(4, textureBackground.height - 26 - 10, 107, 10, textureCategoryLocationGrayedOut);
            textureBackground.SetPixels(4, textureBackground.height - 36 - 10, 107, 10, textureCategoryPeopleHighlighted);
            textureBackground.SetPixels(4, textureBackground.height - 46 - 10, 107, 10, textureCategoryThingsGrayedOut);
            textureBackground.SetPixels(4, textureBackground.height - 56 - 10, 107, 10, textureCategoryWorkGrayedOut);
            textureBackground.Apply(false);

            SetListboxTopics(ref listboxTopic, TalkManager.Instance.ListTopicPeople);
            listboxTopic.Update();

            UpdateScrollBars();
            UpdateScrollButtons();
        }

        void SetTalkCategoryThings()
        {
            selectedTalkCategory = TalkCategory.Things;

            textureBackground.SetPixels(4, textureBackground.height - 26 - 10, 107, 10, textureCategoryLocationGrayedOut);
            textureBackground.SetPixels(4, textureBackground.height - 36 - 10, 107, 10, textureCategoryPeopleGrayedOut);
            textureBackground.SetPixels(4, textureBackground.height - 46 - 10, 107, 10, textureCategoryThingsHighlighted);
            textureBackground.SetPixels(4, textureBackground.height - 56 - 10, 107, 10, textureCategoryWorkGrayedOut);
            textureBackground.Apply(false);

            SetListboxTopics(ref listboxTopic, TalkManager.Instance.ListTopicThings);
            listboxTopic.Update();

            UpdateScrollBars();
            UpdateScrollButtons();
        }

        void SetTalkCategoryWork()
        {
            selectedTalkCategory = TalkCategory.Work;

            textureBackground.SetPixels(4, textureBackground.height - 26 - 10, 107, 10, textureCategoryLocationGrayedOut);
            textureBackground.SetPixels(4, textureBackground.height - 36 - 10, 107, 10, textureCategoryPeopleGrayedOut);
            textureBackground.SetPixels(4, textureBackground.height - 46 - 10, 107, 10, textureCategoryThingsGrayedOut);
            textureBackground.SetPixels(4, textureBackground.height - 56 - 10, 107, 10, textureCategoryWorkHighlighted);
            textureBackground.Apply(false);

            ClearListboxTopics();
            listboxTopic.Update();

            UpdateScrollBars();
            UpdateScrollButtons();
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

        /// <summary>
        /// Gets safe scroll index.
        /// Scroller will be adjust to always be inside display range where possible.
        /// </summary>
        int GetSafeScrollIndex(HorizontalSlider slider)
        {
            // Get current scroller index
            int sliderIndex = slider.ScrollIndex;
            if (sliderIndex < 0)
                sliderIndex = 0;

            // Ensure scroll index within current range
            if (sliderIndex + slider.DisplayUnits > slider.TotalUnits)
            {
                sliderIndex = slider.TotalUnits - slider.DisplayUnits;
                if (sliderIndex < 0) sliderIndex = 0;
                slider.Reset(slider.DisplayUnits, slider.TotalUnits, sliderIndex);
            }

            return sliderIndex;
        }

        // Updates red/green state of scroller buttons
        void UpdateListScrollerButtons(int index, int count, Button upButton, Button downButton)
        {
            // Update up button
            if (index > 0)
                upButton.BackgroundTexture = arrowTopicUpGreen;
            else
                upButton.BackgroundTexture = arrowTopicUpRed;

            // Update down button
            if (index < (count - verticalScrollBarTopicWindow.DisplayUnits))
                downButton.BackgroundTexture = arrowTopicDownGreen;
            else
                downButton.BackgroundTexture = arrowTopicDownRed;

            // No items above or below
            if (count <= verticalScrollBarTopicWindow.DisplayUnits)
            {
                upButton.BackgroundTexture = arrowTopicUpRed;
                downButton.BackgroundTexture = arrowTopicDownRed;
            }
        }

        // Updates red/green state of left/right scroller buttons
        void UpdateListScrollerButtonsLeftRight(int index, int count, Button leftButton, Button rightButton)
        {
            // Update up button
            if (index > 0)
                leftButton.BackgroundTexture = arrowTopicLeftGreen;
            else
                leftButton.BackgroundTexture = arrowTopicLeftRed;

            // Update down button
            if (index < (count - horizontalSliderTopicWindow.DisplayUnits))
                rightButton.BackgroundTexture = arrowTopicRightGreen;
            else
                rightButton.BackgroundTexture = arrowTopicRightRed;

            // No items above or below
            if (count <= horizontalSliderTopicWindow.DisplayUnits)
            {
                leftButton.BackgroundTexture = arrowTopicLeftRed;
                rightButton.BackgroundTexture = arrowTopicRightRed;
            }
        }

        #region event handlers

        private void VerticalScrollBarTopicWindow_OnScroll()
        {
            // Update scroller
            verticalScrollBarTopicWindow.TotalUnits = listboxTopic.Count;
            int scrollIndex = GetSafeScrollIndex(verticalScrollBarTopicWindow);
            
            // Update scroller buttons
            UpdateListScrollerButtons(scrollIndex, listboxTopic.Count, buttonTopicUp, buttonTopicDown);

            listboxTopic.ScrollIndex = scrollIndex;
            listboxTopic.Update();
        }

        private void HorizontalSliderTopicWindow_OnScroll()
        {
            // Update scroller
            horizontalSliderTopicWindow.TotalUnits = lengthOfLongestItemInListBox;
            int horizontalScrollIndex = GetSafeScrollIndex(horizontalSliderTopicWindow); // horizontalSliderTopicWindow.ScrollIndex;

            // Update scroller buttons
            UpdateListScrollerButtonsLeftRight(horizontalScrollIndex, lengthOfLongestItemInListBox, buttonTopicLeft, buttonTopicRight);

            listboxTopic.HorizontalScrollIndex = horizontalScrollIndex;
            listboxTopic.Update();
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

        private void ButtonTellMeAbout_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            SetTalkModeTellMeAbout();
        }

        private void ButtonWhereIs_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            SetTalkModeWhereIs();
        }

        private void ButtonCategoryLocation_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (selectedTalkOption == TalkOption.WhereIs)
            {
                SetTalkCategoryLocation();
            }
        }

        private void ButtonCategoryPeople_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (selectedTalkOption == TalkOption.WhereIs)
            {
                SetTalkCategoryPeople();
            }
        }

        private void ButtonCategoryThings_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (selectedTalkOption == TalkOption.WhereIs)
            {
                SetTalkCategoryThings();
            }
        }

        private void ButtonCategoryWork_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (selectedTalkOption == TalkOption.WhereIs)
            {
                SetTalkCategoryWork();
            }
        }

        private void ButtonTonePolite_OnClickHandler(BaseScreenComponent sender, Vector2 position)
        {
            selectedTalkTone = TalkTone.Polite;
            UpdateCheckboxes();
        }

        private void ButtonToneNormal_OnClickHandler(BaseScreenComponent sender, Vector2 position)
        {
            selectedTalkTone = TalkTone.Normal;
            UpdateCheckboxes();
        }

        private void ButtonToneBlunt_OnClickHandler(BaseScreenComponent sender, Vector2 position)
        {
            selectedTalkTone = TalkTone.Blunt;
            UpdateCheckboxes();
        }

        private void ButtonOkay_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            string answer = "This is a long test string as placeholder for an actual answer. In the future in this scrolling area there will be the correct answers. Right now it is just a dummy text!";
            listboxAnswers.AddItem(answer);
        }

        private void ButtonGoodbye_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            CloseWindow();
        }

        #endregion
    }
}
