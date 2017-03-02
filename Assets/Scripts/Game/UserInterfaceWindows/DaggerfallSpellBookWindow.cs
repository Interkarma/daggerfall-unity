// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2016 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Lypyl (lypyldfa@dgmail.com)
// Contributors:    
// 
// Notes:
//

//#define LAYOUT

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallConnect.Arena2;
using System;


//Spellbook / spell buying effect text starts at 1200
//spell creator effect text starts at index 15xx. 

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// The Spellbook window.  Still very much a W.I.P.
    /// </summary>
    public class DaggerfallSpellBookWindow : DaggerfallPopupWindow
    {
        const string SPELLBOOKIMAGENAME     = "SPBK00I0.IMG";
        const string ICONIMAGENAME          = "ICON00I0.IMG";
        const string RANGEICONSIMAGENAME    = "MASK04I0.IMG";
        const string CHANGETEXT             = "Enter Spell Name: ";


        Texture2D nativeTexture;

        ListBox listBox;                    //main spell list
        VerticalScrollBar scrollBar;

        Panel mainPanel;
        Panel spellIcon;
        Panel spellRangeIcon;
        Panel spellElementIcon;
        Panel[] spellEffectPanels;


        Button exitButton;
        Button deleteButton;
        Button downButton;
        Button upButton;
        Button sortButton;
        Button upArrowButton;
        Button downArrowButton;

        TextLabel spellName;                    //name of selected spell at top
        TextLabel spellPoints;                  //spell points label at top
        TextLabel[] spellEffectLabels;          //effect labels - 2 for each effect (6 total)

        int SelectedIndex   { get { return listBox.SelectedIndex; } }
        bool ValidIndex     { get { return SelectedIndex >= 0 && SelectedIndex < listBox.Count; } }
        bool Refresh        { get; set; }

        public DaggerfallSpellBookWindow(IUserInterfaceManager uiManager, DaggerfallBaseWindow previous = null)
            : base(uiManager, previous)
        {
        }

        protected override void Setup()
        {
            base.Setup();

            ParentPanel.BackgroundColor = ScreenDimColor;

            nativeTexture = DaggerfallUI.GetTextureFromImg(SPELLBOOKIMAGENAME);
            if (!nativeTexture)
                throw new Exception("DaggerfallTravelMap: Could not load native texture.");

            mainPanel                       = new Panel();
            mainPanel.BackgroundTexture     = nativeTexture;
            mainPanel.Size                  = new Vector2(nativeTexture.width, nativeTexture.height);
            mainPanel.HorizontalAlignment   = HorizontalAlignment.Center;
            mainPanel.VerticalAlignment     = VerticalAlignment.Middle;
            mainPanel.Name                  = "main_panel";
            NativePanel.Components.Add(mainPanel);

            listBox = new ListBox();
            listBox.Position                = new Vector2(5, 12);
            listBox.Size                    = new Vector2(111, 132);
            listBox.RowsDisplayed           = 16;
            listBox.MaxCharacters           = 22;
            listBox.Name                    = "spell_list";
            listBox.OnMouseClick            += listBox_OnMouseClickHandler;
            listBox.OnMouseDoubleClick      += listBox_OnMouseDoubleClickHandler;
            listBox.OnMouseScrollDown       += listBox_OnMouseScroll;
            listBox.OnMouseScrollUp         += listBox_OnMouseScroll;
            mainPanel.Components.Add(listBox);

            deleteButton                = DaggerfallUI.AddButton(new Rect(3, 152, 38, 9), mainPanel);
            deleteButton.Name           = "delete_button";
            deleteButton.OnMouseClick   += deleteButton_OnMouseClick;

            upButton                    = DaggerfallUI.AddButton(new Rect(48, 152, 38, 9), mainPanel);
            upButton.Name                = "up_button";
            upButton.OnMouseClick       += swapButton_OnMouseClick;

            sortButton                  = DaggerfallUI.AddButton(new Rect(90, 152, 38, 9), mainPanel);
            sortButton.Name             = "sort_button";
            sortButton.OnMouseClick     += sortButton_OnMouseClick;

            downButton                  = DaggerfallUI.AddButton(new Rect(132, 152, 38, 9), mainPanel);
            downButton.Name             = "down_button";
            downButton.OnMouseClick     += swapButton_OnMouseClick;

            upArrowButton               = DaggerfallUI.AddButton(new Rect(121, 11, 9, 16), mainPanel);
            upArrowButton.Name          = "upArrow_button";
            upArrowButton.OnMouseClick  += arrowButton_OnMouseClick;

            downArrowButton             = DaggerfallUI.AddButton(new Rect(121, 132, 9, 16), mainPanel);
            downArrowButton.Name        = "downArrow_button";
            downArrowButton.OnMouseClick += arrowButton_OnMouseClick;

            exitButton                  = DaggerfallUI.AddButton(new Rect(216, 149, 42.5f, 14.5f), mainPanel);
            exitButton.Name             = "exit_button";
            exitButton.OnMouseClick     += exitButton_OnMouseClick;

            scrollBar                       = new VerticalScrollBar();
            scrollBar.HorizontalAlignment   = HorizontalAlignment.None;
            scrollBar.VerticalAlignment     = VerticalAlignment.None;
            scrollBar.Position              = new Vector2(121, 27);
            scrollBar.Size                  = new Vector2(9, 104);
            scrollBar.TotalUnits            = listBox.Count;
            scrollBar.DisplayUnits          = listBox.RowsDisplayed;
            scrollBar.ScrollIndex           = 0;
            scrollBar.Name                  = "scrollbar";
            scrollBar.OnScroll              += ScrollBar_OnScroll;
            mainPanel.Components.Add(scrollBar);

            spellIcon                       = DaggerfallUI.AddPanel(new Rect(149.25f, 14, 16, 16), mainPanel);
            spellIcon.Name                  = "spell_icon";
            spellIcon.BackgroundTextureLayout = BackgroundLayout.StretchToFill;

            spellRangeIcon                  = DaggerfallUI.AddPanel(new Rect(182, 14, 25, 16), mainPanel);
            spellRangeIcon.Name             = "spellRange_icon";
            spellRangeIcon.BackgroundTextureLayout = BackgroundLayout.StretchToFill;

            spellElementIcon                = DaggerfallUI.AddPanel(new Rect(223, 14, 16, 16), mainPanel);
            spellElementIcon.Name           = "spellEffect_icon";
            spellElementIcon.BackgroundTextureLayout = BackgroundLayout.StretchToFill;

            spellName                   = new TextLabel();
            spellName.Position          = new Vector2(123, 1);
            spellName.Size              = new Vector2(122, 7);
            spellName.MaxCharacters     = 18;
            spellName.Name              = "spellName_label";
            spellName.OnMouseClick      += spellName_OnMouseClick;
            mainPanel.Components.Add(spellName);

            spellPoints                 = new TextLabel();
            spellPoints.Position        = new Vector2(214, 1.5f);
            spellPoints.Size            = new Vector2(40, 7);
            spellPoints.MaxCharacters   = 12;
            spellPoints.Text            = "1000/1000";
            spellPoints.Name            = "spellPoints_label";
            mainPanel.Components.Add(spellPoints);

            spellEffectPanels = new Panel[3];

            spellEffectPanels[0]              = new Panel();
            spellEffectPanels[0].Position     = new Vector2(138, 40);
            spellEffectPanels[0].Size         = new Vector2(118, 28);
            spellEffectPanels[0].Name         = "effect_top";
            spellEffectPanels[0].OnMouseClick += SpellEffectPanelClick;
            mainPanel.Components.Add(spellEffectPanels[0]);

            spellEffectPanels[1]              = new Panel();
            spellEffectPanels[1].Position     = new Vector2(138, 78);
            spellEffectPanels[1].Size         = new Vector2(118, 28);
            spellEffectPanels[1].Name         = "effect_middle";
            spellEffectPanels[1].OnMouseClick += SpellEffectPanelClick;
            mainPanel.Components.Add(spellEffectPanels[1]);

            spellEffectPanels[2]              = new Panel();
            spellEffectPanels[2].Position     = new Vector2(138, 116);
            spellEffectPanels[2].Size         = new Vector2(118, 28);
            spellEffectPanels[2].Name         = "effect_bottom";
            spellEffectPanels[2].OnMouseClick += SpellEffectPanelClick;
            mainPanel.Components.Add(spellEffectPanels[2]);

            //configure effect labels
            spellEffectLabels               = new TextLabel[spellEffectPanels.Length * 2];

            for (int i = 0; i < spellEffectLabels.Length; i++)
            {
                spellEffectLabels[i]                        = new TextLabel();
                spellEffectLabels[i].MaxCharacters          = 24;
                spellEffectLabels[i].Text                   = string.Format("index: {0} panel: {1}", i, i/2);
                spellEffectLabels[i].Name                   = "effect_label_" + i;
                spellEffectLabels[i].HorizontalAlignment    = HorizontalAlignment.Center;

                if (i % 2 == 0)
                    spellEffectLabels[i].Position           = new Vector2(spellEffectLabels[i].Position.x, spellEffectPanels[i/2].Size.y *.125f);
                else
                    spellEffectLabels[i].Position           = new Vector2(spellEffectLabels[i].Position.x, spellEffectPanels[i/2].Size.y * .5f);

                spellEffectPanels[i/2].Components.Add(spellEffectLabels[i]);
            }

            Refresh = true;

            //##below just fills in examples for testing until spells are implemented

            spellIcon.BackgroundTexture = GetSpellIcon(34);
            spellRangeIcon.BackgroundTexture = GetSpellRangeIcon(0);
            spellElementIcon.BackgroundTexture = GetElementTypeIcon(0);
            spellName.Text = "selected spell";


            for (int i = 0; i < spellEffectPanels.Length; i++)
            {
                var labels = GetEffectLabels(i);
                SetEffectLabel(labels, new string[] { labels[0].Name, labels[1].Name });
            }

            for (int i = 0; i < 50; i++)
            {
                listBox.AddItem(i + "- spell name");
            }

#if LAYOUT
            SetBackgroundColors();
#endif

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


            if (Refresh)
                UpdateSelection();

        }

        //updates labels / icons etc. when something has changed
        void UpdateSelection()
        {
            if (!ValidIndex)
            {
                if(listBox.Count > 0)
                {
                    listBox.SelectedIndex = 0;
                    scrollBar.ScrollIndex = 0;
                }
                else
                    return;
            }

            scrollBar.Reset(listBox.RowsDisplayed, listBox.Count, listBox.ScrollIndex);
            scrollBar.TotalUnits = listBox.Count;
            scrollBar.ScrollIndex = listBox.ScrollIndex;
            spellName.Text = listBox.SelectedItem;


            ///TODO:
            ///1. set spell icons
            ///2. set spell effect labels
            ///3. update magica cost

            Refresh = false;
        }

        //set the text for the effect label
        void SetEffectLabel(TextLabel[] labels, string[] effectDescriptions)
        {
            if (labels == null || labels.Length < 2)
                return;
            else if (effectDescriptions == null || effectDescriptions.Length < 2)
                return;

            labels[0].Text = effectDescriptions[0];
            labels[1].Text = effectDescriptions[1];

        }

        //Spellbook / spell buying effect text starts at 1200
        void ShowEffectPopup(int textIndex)
        {
            DaggerfallMessageBox spellEffectPopup = new DaggerfallMessageBox(uiManager, this);
            spellEffectPopup.ClickAnywhereToClose = true;
            spellEffectPopup.SetTextTokens(textIndex);
            spellEffectPopup.Show();
        }

        //helper function to get effect labels for a panel
        TextLabel[] GetEffectLabels(int panelIndex)
        {
            TextLabel[] labels = new TextLabel[2];
            labels[0]           = spellEffectLabels[panelIndex*2];
            labels[1]           = spellEffectLabels[panelIndex*2+1];
            return labels;
        }

        //returns icon texture for corresponding index
        Texture2D GetSpellIcon(int iconIndex)
        {
            var x = iconIndex % 10;
            var y = iconIndex / 10;
            var rect = new Rect(x*16, y*16, 16, 16);
            var iconTexture = DaggerfallUI.GetTextureFromImg(System.IO.Path.Combine(DaggerfallUnity.Arena2Path, ICONIMAGENAME), rect, TextureFormat.ARGB32);
            return iconTexture;
        }

        //returns icon texture for corresponding index
        //this is not working properly for some reason - the y pos. isn't being respected
        Texture2D GetSpellRangeIcon(int iconIndex)
        {
            var x = 0;
            var y = iconIndex * 16;

            var rect = new Rect(x, y, 24, 16);
            var iconTexture = DaggerfallUI.GetTextureFromImg(System.IO.Path.Combine(DaggerfallUnity.Arena2Path, RANGEICONSIMAGENAME), rect, TextureFormat.ARGB32);
            Debug.Log(string.Format("tw: {0} th: {1} x: {2} y: {3}", iconTexture.width, iconTexture.height, rect.x, rect.y));
            return iconTexture;
        }

        //returns icon texture for corresponding index
        Texture2D GetElementTypeIcon(int iconIndex)
        {
            var x = 24;
            var y = iconIndex * 16;
            var rect = new Rect(x, y, 16, 16);
            var iconTexture = DaggerfallUI.GetTextureFromImg(System.IO.Path.Combine(DaggerfallUnity.Arena2Path, RANGEICONSIMAGENAME), rect, TextureFormat.ARGB32);
            return iconTexture;
        }

        #region event_handlers

        //handles clicks on the effect panels
        void SpellEffectPanelClick(BaseScreenComponent sender, Vector2 position)
        {
            Debug.Log("spell effect panel clicked: " + sender.Name);

            int testTextIndex = 1200;   //just for testing - 
                                        //need to get index for this effect from spell when implemented

            if(sender.Name == spellEffectPanels[0].Name)
            {
                ShowEffectPopup(testTextIndex);
            }
            else if(sender.Name == spellEffectPanels[1].Name)
            {
                ShowEffectPopup(testTextIndex);
            }
            else if(sender.Name == spellEffectPanels[2].Name)
            {
                ShowEffectPopup(testTextIndex);
            }
        }

        void listBox_OnMouseClickHandler(BaseScreenComponent sender, Vector2 position)
        {
            Debug.Log("list box clicked");

            Refresh = true;
        }

        //handles double clicks on the spell list
        void listBox_OnMouseDoubleClickHandler(BaseScreenComponent sender, Vector2 position)
        {
            //TODO
            Debug.Log("list box Double Clicked");
        }

        void listBox_OnMouseScroll()
        {
            Debug.Log("list box mouse scroll down");

            Refresh = true;
        }

        void ScrollBar_OnScroll()
        {
            Debug.Log("Scroll bar scrolling: " + listBox.ScrollIndex);
            listBox.ScrollIndex = scrollBar.ScrollIndex;

            Refresh = true;
        }

        //scroll up/down arrow buttons
        void arrowButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            Debug.Log("Arrow button clicked: " + sender.Name);
            if(sender.Name == upArrowButton.Name)
                listBox.SelectPrevious();
            else if(sender.Name == downArrowButton.Name)
                listBox.SelectNext();

            Refresh = true;
        }

        //handles clicks on delete button - should remove selected spell
        void deleteButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            Debug.Log("delete button clicked");
            //TODO
            Refresh = true;
        }

        //handles clicks on exit button - close window w/o selecting spell
        void exitButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            Debug.Log("Exit bttn clicked");
            CloseWindow();
        }

        //handles clicks for up/down buttons on bottom panel - swap spells
        //currently just moves examples in listBox
        void swapButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            Debug.Log("swap position button clicked: " + sender.Name);

            if(sender.Name == downButton.Name && listBox.SelectedIndex < listBox.Count-1)
                listBox.SwapItems(listBox.SelectedIndex, ++listBox.SelectedIndex);
            else if(sender.Name == upButton.Name && listBox.SelectedIndex > 0)
                listBox.SwapItems(listBox.SelectedIndex, --listBox.SelectedIndex);

            Refresh = true;
        }

        //not implemented in Daggerfall, could be useful. Possibly move through different sorts (lexigraphic, date added, cost etc.)
        public void sortButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            Debug.Log("sortButton clicked");
            //TODO
            Refresh = true;
        }

        //spell name shown above spell icons
        public void spellName_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            Debug.Log("SpellName label clicked");

            if (!ValidIndex)
                return;

            DaggerfallInputMessageBox renameSpellPrompt;
            renameSpellPrompt = new DaggerfallInputMessageBox(this.uiManager, this);
            renameSpellPrompt.SetTextBoxLabel(CHANGETEXT);
            renameSpellPrompt.TextBox.Text = listBox.SelectedItem;
            renameSpellPrompt.OnGotUserInput += renameSpellPromptHandler;
            uiManager.PushWindow(renameSpellPrompt);

            Refresh = true;
        }

        //called by messagebox closing when player clicks on spell name
        //currently this just updates selectedItem in the list box
        public void renameSpellPromptHandler(DaggerfallInputMessageBox sender, string input)
        {
            //daggerfall allows empty spell name, seems like a bad idea
            if (string.IsNullOrEmpty(input))
                return;

            //TODO - 
            listBox.AddItem(input, listBox.SelectedIndex);
            listBox.RemoveItem(SelectedIndex+1);

        }

        #endregion


#if LAYOUT

        void SetBackgroundColors()
        {
            Color[] colors = new Color[]{
                new Color(0,0,0, .75f),
                new Color(1,0,0, .75f),
                new Color(0,1,0, .75f),
                new Color(0,0,0, .75f),
                new Color(1, 1, 1, 0.75f),
                new Color(1, 1, 0, 0.75f),
                new Color(1, 0, 1, 0.75f),
                new Color(0, 1, 1, 0.75f),
            };


            int i = 0;
            int color_index = 0;
            while (i < mainPanel.Components.Count)
            {
                if (color_index >= colors.Length)
                    color_index = 0;

                mainPanel.Components[i].BackgroundColor = colors[color_index];
                i++;
                color_index++;
            }
        }

#endif

    }
}
