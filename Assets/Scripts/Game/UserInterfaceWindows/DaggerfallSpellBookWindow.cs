// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Lypyl (lypyldf@gmail.com), Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.MagicAndEffects;

//Spellbook / spell buying effect text starts at 1200
//spell creator effect text starts at index 15xx. 

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Spellbook UI.
    /// </summary>
    public class DaggerfallSpellBookWindow : DaggerfallPopupWindow
    {
        #region UI Rects

        Vector2 spellNameLabelPos = new Vector2(123, 2);
        Vector2 spellPointsLabelPos = new Vector2(214, 2);

        Rect mainPanelRect = new Rect(0, 0, 259, 164);
        Rect spellsListBoxRect = new Rect(5, 13, 110, 130);
        Rect deleteButtonRect = new Rect(3, 152, 38, 9);
        Rect upButtonRect = new Rect(48, 152, 38, 9);
        Rect downButtonRect = new Rect(132, 152, 38, 9);
        Rect sortButtonRect = new Rect(90, 152, 38, 9);
        Rect upArrowButtonRect = new Rect(121, 11, 9, 16);
        Rect downArrowButtonRect = new Rect(121, 132, 9, 16);
        Rect exitButtonRect = new Rect(216, 149, 43, 15);
        Rect spellsListScrollBarRect =  new Rect(122, 28, 7, 103);
        Rect spellIconPanelRect = new Rect(149.25f, 14, 16, 16);
        Rect spellTargetPanelRect = new Rect(182, 14, 25, 16);
        Rect spellElementIconPanelRect = new Rect(223, 14, 16, 16);
        Rect effect1PanelRect = new Rect(138, 40, 118, 28);
        Rect effect2PanelRect = new Rect(138, 78, 118, 28);
        Rect effect3PanelRect = new Rect(138, 116, 118, 28);

        #endregion

        #region UI Controls

        ListBox spellsListBox;
        VerticalScrollBar spellsListScrollBar;

        Panel mainPanel;
        Panel spellIconPanel;
        Panel spellTargetIconPanel;
        Panel spellElementIconPanel;
        Panel[] spellEffectPanels;

        Button exitButton;
        Button deleteButton;
        Button downButton;
        Button upButton;
        Button sortButton;
        Button upArrowButton;
        Button downArrowButton;

        TextLabel spellNameLabel;
        TextLabel spellPointsLabel;
        TextLabel[] spellEffectLabels;

        #endregion

        #region UI Textures

        Texture2D baseTexture;

        #endregion

        #region Fields

        const string textDatabase = "SpellmakerUI";

        const string spellBookTextureFilename = "SPBK00I0.IMG";
        const string ICONIMAGENAME = "ICON00I0.IMG";
        const string RANGEICONSIMAGENAME = "MASK04I0.IMG";
        const string CHANGETEXT = "Enter Spell Name: ";

        #endregion

        #region Properties

        int SelectedIndex   { get { return spellsListBox.SelectedIndex; } }
        bool ValidIndex     { get { return SelectedIndex >= 0 && SelectedIndex < spellsListBox.Count; } }
        bool Refresh        { get; set; }

        #endregion

        #region Constructors

        public DaggerfallSpellBookWindow(IUserInterfaceManager uiManager, DaggerfallBaseWindow previous = null)
            : base(uiManager, previous)
        {
        }

        #endregion

        #region Setup Methods

        protected override void Setup()
        {
            // Load all textures used by spellbook window
            LoadTextures();

            // Always dim background
            ParentPanel.BackgroundColor = ScreenDimColor;

            // Setup main layout
            SetupMain();

            // Setup controls
            SetupButtons();
            SetupIcons();
            SetupLabels();

            Refresh = true;

            //##below just fills in examples for testing until spells are implemented

            //spellIcon.BackgroundTexture = GetSpellIcon(34);
            //spellTargetIcon.BackgroundTexture = GetSpellRangeIcon(0);
            //spellElementIcon.BackgroundTexture = GetElementTypeIcon(0);
            //spellName.Text = "selected spell";

            //for (int i = 0; i < spellEffectPanels.Length; i++)
            //{
            //    var labels = GetEffectLabels(i);
            //    SetEffectLabel(labels, new string[] { labels[0].Name, labels[1].Name });
            //}

            // TEMP: Inject player spells
            EffectBundleSettings[] spellbook = GameManager.Instance.PlayerEntity.GetSpells();
            if (spellbook != null)
            {
                for (int i = 0; i < spellbook.Length; i++)
                {
                    // All spell costs are zero for now - not implemented
                    spellsListBox.AddItem(string.Format("0 - {0}", spellbook[i].Name));
                }
            }

            SetDefaults();
        }

        public override void OnPush()
        {
            if (IsSetup)
            {
                SetDefaults();
            }
        }

        public override void Update()
        {
            base.Update();

            if (Refresh)
                UpdateSelection();
        }

        void SetDefaults()
        {
            // Set spell points label
            int curSpellPoints = GameManager.Instance.PlayerEntity.CurrentMagicka;
            int maxSpellPoints = GameManager.Instance.PlayerEntity.MaxMagicka;
            spellPointsLabel.Text = string.Format("{0}/{1}", curSpellPoints, maxSpellPoints);
        }

        #endregion

        #region Private Methods

        void LoadTextures()
        {
            baseTexture = DaggerfallUI.GetTextureFromImg(spellBookTextureFilename);
        }

        void SetupMain()
        {
            // Main panel
            mainPanel = DaggerfallUI.AddPanel(mainPanelRect, NativePanel);
            mainPanel.BackgroundTexture = baseTexture;
            mainPanel.HorizontalAlignment = HorizontalAlignment.Center;
            mainPanel.VerticalAlignment = VerticalAlignment.Middle;

            // Spells list
            spellsListBox = new ListBox();
            spellsListBox.Position = new Vector2(spellsListBoxRect.x, spellsListBoxRect.y);
            spellsListBox.Size = new Vector2(spellsListBoxRect.width, spellsListBoxRect.height);
            spellsListBox.RowsDisplayed = 16;
            spellsListBox.MaxCharacters = 22;
            spellsListBox.OnMouseClick += listBox_OnMouseClickHandler;
            spellsListBox.OnUseSelectedItem += listBox_OnUseSelectedItem;
            spellsListBox.OnMouseScrollDown += listBox_OnMouseScroll;
            spellsListBox.OnMouseScrollUp += listBox_OnMouseScroll;
            mainPanel.Components.Add(spellsListBox);

            // Spells list scroller
            spellsListScrollBar = new VerticalScrollBar();
            spellsListScrollBar.HorizontalAlignment = HorizontalAlignment.None;
            spellsListScrollBar.VerticalAlignment = VerticalAlignment.None;
            spellsListScrollBar.Position = new Vector2(spellsListScrollBarRect.x, spellsListScrollBarRect.y);
            spellsListScrollBar.Size = new Vector2(spellsListScrollBarRect.width, spellsListScrollBarRect.height);
            spellsListScrollBar.TotalUnits = spellsListBox.Count;
            spellsListScrollBar.DisplayUnits = spellsListBox.RowsDisplayed;
            spellsListScrollBar.ScrollIndex = 0;
            spellsListScrollBar.OnScroll += SpellsListScrollBar_OnScroll;
            mainPanel.Components.Add(spellsListScrollBar);

            // Spell effect panels
            spellEffectPanels = new Panel[3];
            spellEffectPanels[0] = DaggerfallUI.AddPanel(effect1PanelRect, mainPanel);
            spellEffectPanels[0].OnMouseClick += SpellEffectPanelClick;
            spellEffectPanels[1] = DaggerfallUI.AddPanel(effect2PanelRect, mainPanel);
            spellEffectPanels[1].OnMouseClick += SpellEffectPanelClick;
            spellEffectPanels[2] = DaggerfallUI.AddPanel(effect3PanelRect, mainPanel);
            spellEffectPanels[2].OnMouseClick += SpellEffectPanelClick;
        }

        void SetupButtons()
        {
            // Bottom row buttons
            deleteButton = DaggerfallUI.AddButton(deleteButtonRect, mainPanel);
            deleteButton.OnMouseClick += deleteButton_OnMouseClick;
            upButton = DaggerfallUI.AddButton(upButtonRect, mainPanel);
            upButton.OnMouseClick += swapButton_OnMouseClick;
            sortButton = DaggerfallUI.AddButton(sortButtonRect, mainPanel);
            sortButton.OnMouseClick += sortButton_OnMouseClick;
            downButton = DaggerfallUI.AddButton(downButtonRect, mainPanel);
            downButton.OnMouseClick += swapButton_OnMouseClick;
            exitButton = DaggerfallUI.AddButton(exitButtonRect, mainPanel);
            exitButton.OnMouseClick += exitButton_OnMouseClick;

            // Scroller buttons
            upArrowButton = DaggerfallUI.AddButton(upArrowButtonRect, mainPanel);
            upArrowButton.OnMouseClick += arrowButton_OnMouseClick;
            downArrowButton = DaggerfallUI.AddButton(downArrowButtonRect, mainPanel);
            downArrowButton.OnMouseClick += arrowButton_OnMouseClick;
        }

        void SetupIcons()
        {
            spellIconPanel = DaggerfallUI.AddPanel(spellIconPanelRect, mainPanel);
            spellIconPanel.BackgroundColor = Color.black;
            spellIconPanel.BackgroundTextureLayout = BackgroundLayout.StretchToFill;

            spellTargetIconPanel = DaggerfallUI.AddPanel(spellTargetPanelRect, mainPanel);
            spellTargetIconPanel.BackgroundColor = Color.black;
            spellTargetIconPanel.BackgroundTextureLayout = BackgroundLayout.StretchToFill;

            spellElementIconPanel = DaggerfallUI.AddPanel(spellElementIconPanelRect, mainPanel);
            spellElementIconPanel.BackgroundColor = Color.black;
            spellElementIconPanel.BackgroundTextureLayout = BackgroundLayout.StretchToFill;
        }

        void SetupLabels()
        {
            // Spell name
            spellNameLabel = DaggerfallUI.AddTextLabel(DaggerfallUI.DefaultFont, spellNameLabelPos, string.Empty, mainPanel);
            spellNameLabel.ShadowColor = DaggerfallUI.DaggerfallAlternateShadowColor1;
            spellNameLabel.MaxCharacters = 18;
            spellNameLabel.OnMouseClick += SpellNameLabel_OnMouseClick;

            // Spell points
            spellPointsLabel = DaggerfallUI.AddTextLabel(DaggerfallUI.DefaultFont, spellPointsLabelPos, string.Empty, mainPanel);
            spellPointsLabel.ShadowColor = DaggerfallUI.DaggerfallAlternateShadowColor1;

            // Effects
            spellEffectLabels = new TextLabel[spellEffectPanels.Length * 2];

            //spellEffectLabels = new TextLabel[spellEffectPanels.Length * 2];
            //for (int i = 0; i < spellEffectLabels.Length; i++)
            //{
            //    spellEffectLabels[i] = new TextLabel();
            //    spellEffectLabels[i].MaxCharacters = 24;
            //    spellEffectLabels[i].Text = string.Format("index: {0} panel: {1}", i, i / 2);
            //    spellEffectLabels[i].Name = "effect_label_" + i;
            //    spellEffectLabels[i].HorizontalAlignment = HorizontalAlignment.Center;

            //    if (i % 2 == 0)
            //        spellEffectLabels[i].Position = new Vector2(spellEffectLabels[i].Position.x, spellEffectPanels[i / 2].Size.y * .125f);
            //    else
            //        spellEffectLabels[i].Position = new Vector2(spellEffectLabels[i].Position.x, spellEffectPanels[i / 2].Size.y * .5f);

            //    spellEffectPanels[i / 2].Components.Add(spellEffectLabels[i]);
            //}
        }

        //updates labels / icons etc. when something has changed
        void UpdateSelection()
        {
            // Validate
            if (!ValidIndex)
            {
                if(spellsListBox.Count > 0)
                {
                    spellsListBox.SelectedIndex = 0;
                    spellsListScrollBar.ScrollIndex = 0;
                }
                else
                    return;
            }

            // Update spell list scroller
            spellsListScrollBar.Reset(spellsListBox.RowsDisplayed, spellsListBox.Count, spellsListBox.ScrollIndex);
            spellsListScrollBar.TotalUnits = spellsListBox.Count;
            spellsListScrollBar.ScrollIndex = spellsListBox.ScrollIndex;

            // Update spell name label
            EffectBundleSettings spell;
            if (GameManager.Instance.PlayerEntity.GetSpell(spellsListBox.SelectedIndex, out spell))
                spellNameLabel.Text = spell.Name;

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

        #endregion

        #region Events

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

        ////handles double clicks on the spell list
        //void listBox_OnMouseDoubleClickHandler(BaseScreenComponent sender, Vector2 position)
        //{
        //    //TODO
        //    Debug.Log("list box Double Clicked");

        //    // TEMP: Issue a fake spell to player's effect manager
        //    // This will expand and eventually be replaced with real spells
        //    // Currently just setting up spellcasting front-end and animations
        //    EntityEffectManager playerEffectManager = GameManager.Instance.PlayerEffectManager;
        //    if (playerEffectManager)
        //    {
        //        playerEffectManager.SetReadySpell(new FakeSpell());
        //        CloseWindow();
        //    }
        //}

        private void listBox_OnUseSelectedItem()
        {
            // TEMP: Issue a fake spell to player's effect manager
            // This will expand and eventually be replaced with real spells
            // Currently just setting up spellcasting front-end and animations
            EntityEffectManager playerEffectManager = GameManager.Instance.PlayerEffectManager;
            if (playerEffectManager)
            {
                playerEffectManager.SetReadySpell(new FakeSpell());
                CloseWindow();
            }
        }

        void listBox_OnMouseScroll(BaseScreenComponent sender)
        {
            Debug.Log("list box mouse scroll down");

            Refresh = true;
        }

        void SpellsListScrollBar_OnScroll()
        {
            //Debug.Log("Scroll bar scrolling: " + spellsListBox.ScrollIndex);
            spellsListBox.ScrollIndex = spellsListScrollBar.ScrollIndex;

            Refresh = true;
        }

        //scroll up/down arrow buttons
        void arrowButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            Debug.Log("Arrow button clicked: " + sender.Name);
            if(sender.Name == upArrowButton.Name)
                spellsListBox.SelectPrevious();
            else if(sender.Name == downArrowButton.Name)
                spellsListBox.SelectNext();

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

            if(sender.Name == downButton.Name && spellsListBox.SelectedIndex < spellsListBox.Count-1)
                spellsListBox.SwapItems(spellsListBox.SelectedIndex, ++spellsListBox.SelectedIndex);
            else if(sender.Name == upButton.Name && spellsListBox.SelectedIndex > 0)
                spellsListBox.SwapItems(spellsListBox.SelectedIndex, --spellsListBox.SelectedIndex);

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
        public void SpellNameLabel_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            Debug.Log("SpellName label clicked");

            if (!ValidIndex)
                return;

            DaggerfallInputMessageBox renameSpellPrompt;
            renameSpellPrompt = new DaggerfallInputMessageBox(this.uiManager, this);
            renameSpellPrompt.SetTextBoxLabel(CHANGETEXT);
            renameSpellPrompt.TextBox.Text = spellsListBox.SelectedItem;
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
            spellsListBox.AddItem(input, spellsListBox.SelectedIndex);
            spellsListBox.RemoveItem(SelectedIndex+1);

        }

        #endregion
    }
}
