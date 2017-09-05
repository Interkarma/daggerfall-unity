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

            ParentPanel.BackgroundColor = ScreenDimColor;

            mainPanel                       = DaggerfallUI.AddPanel(NativePanel, AutoSizeModes.None);
            mainPanel.BackgroundTexture = textureBackground;
            mainPanel.Size = new Vector2(textureBackground.width, textureBackground.height);
            mainPanel.HorizontalAlignment   = HorizontalAlignment.Center;
            mainPanel.VerticalAlignment     = VerticalAlignment.Middle;

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

            UpdateLabels();
            UpdateButtons();
        }

        public override void OnPush()
        {
            base.OnPush();
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


        void UpdateLabels()
        {

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

        #region event handlers

        void buttonTellMeAbout_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            selectedTalkOption = TalkOption.TellMeAbout;
            UpdateButtons();
        }

        void buttonWhereIs_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            selectedTalkOption = TalkOption.WhereIs;
            UpdateButtons();
        }

        void buttonCategoryLocation_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (selectedTalkOption == TalkOption.WhereIs)
            {
                selectedTalkCategory = TalkCategory.Location;
                UpdateButtons();
            }
        }

        void buttonCategoryPeople_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (selectedTalkOption == TalkOption.WhereIs)
            {
                selectedTalkCategory = TalkCategory.People;
                UpdateButtons();
            }
        }

        void buttonCategoryThings_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (selectedTalkOption == TalkOption.WhereIs)
            {
                selectedTalkCategory = TalkCategory.Things;
                UpdateButtons();
            }
        }

        void buttonCategoryWork_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (selectedTalkOption == TalkOption.WhereIs)
            {
                selectedTalkCategory = TalkCategory.Work;
                UpdateButtons();
            }
        }

        void buttonTonePolite_OnClickHandler(BaseScreenComponent sender, Vector2 position)
        {
            selectedTalkTone = TalkTone.Polite;
            UpdateButtons();
        }

        void buttonToneNormal_OnClickHandler(BaseScreenComponent sender, Vector2 position)
        {
            selectedTalkTone = TalkTone.Normal;
            UpdateButtons();
        }

        void buttonToneBlunt_OnClickHandler(BaseScreenComponent sender, Vector2 position)
        {
            selectedTalkTone = TalkTone.Blunt;
            UpdateButtons();
        }

        void buttonGoodbye_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            CloseWindow();
        }

        #endregion
    }
}
