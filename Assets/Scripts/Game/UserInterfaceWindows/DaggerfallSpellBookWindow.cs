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
using DaggerfallWorkshop.Utility;

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
        Rect spellsListScrollBarRect = new Rect(122, 28, 7, 103);
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

        const SoundClips openSpellBook = SoundClips.OpenBook;
        const SoundClips closeSpellBook = SoundClips.PageTurn;

        int deleteSpellIndex = -1;

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

            RefreshSpellsList();
            SetDefaults();
        }

        public override void OnPush()
        {
            if (IsSetup)
            {
                RefreshSpellsList();
                SetDefaults();
            }

            DaggerfallUI.Instance.PlayOneShot(openSpellBook);
        }

        public override void OnPop()
        {
            DaggerfallUI.Instance.PlayOneShot(closeSpellBook);
        }

        void SetDefaults()
        {
            // Set spell points label
            int curSpellPoints = GameManager.Instance.PlayerEntity.CurrentMagicka;
            int maxSpellPoints = GameManager.Instance.PlayerEntity.MaxMagicka;
            spellPointsLabel.Text = string.Format("{0}/{1}", curSpellPoints, maxSpellPoints);

            // Default selected spell info
            spellNameLabel.Text = string.Empty;
            spellIconPanel.BackgroundTexture = null;
            spellTargetIconPanel.BackgroundTexture = null;
            spellElementIconPanel.BackgroundTexture = null;
            ClearEffectLabels();

            // Select default spell
            spellsListBox.SelectedIndex = 0;
        }

        void RefreshSpellsList()
        {
            // Clear existing list
            spellsListBox.ClearItems();

            // Add player spells to list
            EffectBundleSettings[] spellbook = GameManager.Instance.PlayerEntity.GetSpells();
            if (spellbook != null)
            {
                for (int i = 0; i < spellbook.Length; i++)
                {
                    // All spell costs are zero for now - not implemented
                    spellsListBox.AddItem(string.Format("0 - {0}", spellbook[i].Name));
                }
            }
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
            spellsListBox.OnSelectItem += SpellsListBox_OnSelectItem;
            spellsListBox.OnUseSelectedItem += SpellsListBox_OnUseSelectedItem;
            spellsListBox.OnMouseScrollDown += SpellsListBox_OnMouseScroll;
            spellsListBox.OnMouseScrollUp += SpellsListBox_OnMouseScroll;
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
            spellEffectPanels[0].Name = "effect1Panel";
            spellEffectPanels[0].OnMouseClick += SpellEffectPanelClick;
            spellEffectPanels[1] = DaggerfallUI.AddPanel(effect2PanelRect, mainPanel);
            spellEffectPanels[1].Name = "effect2Panel";
            spellEffectPanels[1].OnMouseClick += SpellEffectPanelClick;
            spellEffectPanels[2] = DaggerfallUI.AddPanel(effect3PanelRect, mainPanel);
            spellEffectPanels[2].Name = "effect3Panel";
            spellEffectPanels[2].OnMouseClick += SpellEffectPanelClick;
        }

        void SetupButtons()
        {
            // Bottom row buttons
            deleteButton = DaggerfallUI.AddButton(deleteButtonRect, mainPanel);
            deleteButton.OnMouseClick += DeleteButton_OnMouseClick;

            upButton = DaggerfallUI.AddButton(upButtonRect, mainPanel);
            sortButton = DaggerfallUI.AddButton(sortButtonRect, mainPanel);
            downButton = DaggerfallUI.AddButton(downButtonRect, mainPanel);

            exitButton = DaggerfallUI.AddButton(exitButtonRect, mainPanel);
            exitButton.OnMouseClick += ExitButton_OnMouseClick;

            // Scroller buttons
            upArrowButton = DaggerfallUI.AddButton(upArrowButtonRect, mainPanel);
            upArrowButton.OnMouseClick += UpArrowButton_OnMouseClick;
            downArrowButton = DaggerfallUI.AddButton(downArrowButtonRect, mainPanel);
            downArrowButton.OnMouseClick += DownArrowButton_OnMouseClick;
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

            // Effect labels
            spellEffectLabels = new TextLabel[spellEffectPanels.Length * 2];
            for (int i = 0; i < spellEffectLabels.Length; i++)
            {
                spellEffectLabels[i] = new TextLabel();
                spellEffectLabels[i].MaxCharacters = 24;
                spellEffectLabels[i].HorizontalAlignment = HorizontalAlignment.Center;
                spellEffectLabels[i].ShadowColor = DaggerfallUI.DaggerfallAlternateShadowColor1;

                if (i % 2 == 0)
                    spellEffectLabels[i].Position = new Vector2(0, 5);
                else
                    spellEffectLabels[i].Position = new Vector2(0, 17);

                spellEffectPanels[i / 2].Components.Add(spellEffectLabels[i]);
            }
        }

        void UpdateSelection()
        {
            // Update spell list scroller
            spellsListScrollBar.Reset(spellsListBox.RowsDisplayed, spellsListBox.Count, spellsListBox.ScrollIndex);
            spellsListScrollBar.TotalUnits = spellsListBox.Count;
            spellsListScrollBar.ScrollIndex = spellsListBox.ScrollIndex;

            // Get spell and exit if spell index not found
            EffectBundleSettings spellSettings;
            if (!GameManager.Instance.PlayerEntity.GetSpell(spellsListBox.SelectedIndex, out spellSettings))
                return;

            // Update spell name label
            spellNameLabel.Text = spellSettings.Name;

            // Update effect labels
            for (int i = 0; i < 3; i++)
            {
                if (i < spellSettings.Effects.Length)
                    SetEffectLabels(spellSettings.Effects[i].Key, i);
                else
                    SetEffectLabels(string.Empty, i);
            }

            // Update spell icons
            spellIconPanel.BackgroundTexture = GetSpellIcon(spellSettings.IconIndex);
            spellTargetIconPanel.BackgroundTexture = GetSpellTargetIcon(spellSettings.TargetType);
            spellElementIconPanel.BackgroundTexture = GetSpellElementIcon(spellSettings.ElementType);
        }

        void ClearEffectLabels()
        {
            for (int i = 0; i < 3; i++)
            {
                SetEffectLabels(string.Empty, i);
            }
        }

        void SetEffectLabels(string key, int effectIndex)
        {
            int labelIndex = effectIndex * 2;

            // Just clear labels if no effect key
            if (string.IsNullOrEmpty(key))
            {
                spellEffectLabels[labelIndex].Text = string.Empty;
                spellEffectLabels[labelIndex + 1].Text = string.Empty;
                return;
            }

            // Get interface to effect
            IEntityEffect effect = GameManager.Instance.EntityEffectBroker.GetEffectTemplate(key);
            if (effect == null)
            {
                // Handle effect not found
                spellEffectLabels[labelIndex].Text = TextManager.Instance.GetText(textDatabase, "effectNotFoundError");
                spellEffectLabels[labelIndex + 1].Text = key;
                return;
            }

            // Update labels
            spellEffectLabels[labelIndex].Text = effect.GroupName;
            spellEffectLabels[labelIndex + 1].Text = effect.SubGroupName;
        }

        void ShowEffectPopup(IEntityEffect effect)
        {
            if (effect == null)
                return;

            DaggerfallMessageBox spellEffectPopup = new DaggerfallMessageBox(uiManager, this);
            spellEffectPopup.ClickAnywhereToClose = true;
            spellEffectPopup.SetTextTokens(effect.SpellBookDescription, effect);
            spellEffectPopup.Show();
        }

        TextLabel[] GetEffectLabels(int panelIndex)
        {
            TextLabel[] labels = new TextLabel[2];
            labels[0] = spellEffectLabels[panelIndex * 2];
            labels[1] = spellEffectLabels[panelIndex * 2 + 1];
            return labels;
        }

        Texture2D GetSpellIcon(int index)
        {
            return DaggerfallUI.Instance.SpellIconCollection.GetSpellIcon(index);
        }

        Texture2D GetSpellTargetIcon(TargetTypes targetType)
        {
            return DaggerfallUI.Instance.SpellIconCollection.GetSpellTargetIcon(targetType);
        }

        Texture2D GetSpellElementIcon(ElementTypes elementType)
        {
            return DaggerfallUI.Instance.SpellIconCollection.GetSpellElementIcon(elementType);
        }

        #endregion

        #region Events

        void SpellEffectPanelClick(BaseScreenComponent sender, Vector2 position)
        {
            // Get spell and exit if spell index not found
            EffectBundleSettings spellSettings;
            if (!GameManager.Instance.PlayerEntity.GetSpell(spellsListBox.SelectedIndex, out spellSettings))
                return;

            // Get effect index of panel clicked
            int effectIndex;
            if (sender.Name == spellEffectPanels[0].Name && spellSettings.Effects.Length >= 1)
                effectIndex = 0;
            else if (sender.Name == spellEffectPanels[1].Name && spellSettings.Effects.Length >= 2)
                effectIndex = 1;
            else if (sender.Name == spellEffectPanels[2].Name && spellSettings.Effects.Length >= 3)
                effectIndex = 2;
            else
                return;

            // Create effect instance with settings and show popup
            IEntityEffect effect = GameManager.Instance.EntityEffectBroker.InstantiateEffect(spellSettings.Effects[effectIndex]);
            ShowEffectPopup(effect);
        }

        private void SpellsListBox_OnSelectItem()
        {
            UpdateSelection();
        }

        private void SpellsListBox_OnUseSelectedItem()
        {
            // Get spell settings and exit if spell index not found
            EffectBundleSettings spellSettings;
            if (!GameManager.Instance.PlayerEntity.GetSpell(spellsListBox.SelectedIndex, out spellSettings))
                return;

            // Assign to player effect manager as ready spell
            EntityEffectManager playerEffectManager = GameManager.Instance.PlayerEffectManager;
            if (playerEffectManager)
            {
                playerEffectManager.SetReadySpell(new EntityEffectBundle(spellSettings, GameManager.Instance.PlayerEntityBehaviour));
                CloseWindow();
            }
        }

        private void SpellsListBox_OnMouseScroll(BaseScreenComponent sender)
        {
            spellsListScrollBar.ScrollIndex = spellsListBox.ScrollIndex;
        }

        void SpellsListScrollBar_OnScroll()
        {
            spellsListBox.ScrollIndex = spellsListScrollBar.ScrollIndex;
        }

        private void UpArrowButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            spellsListBox.SelectedIndex--;
        }

        private void DownArrowButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            spellsListBox.SelectedIndex++;
        }

        void DeleteButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            // Prompt and delete spell
            deleteSpellIndex = spellsListBox.SelectedIndex;
            DaggerfallMessageBox mb = new DaggerfallMessageBox(uiManager, DaggerfallMessageBox.CommonMessageBoxButtons.YesNo, TextManager.Instance.GetText(textDatabase, "deleteSpell"), this);
            mb.OnButtonClick += DeleteSpellConfirm_OnButtonClick;            
            mb.Show();
        }

        private void DeleteSpellConfirm_OnButtonClick(DaggerfallMessageBox sender, DaggerfallMessageBox.MessageBoxButtons messageBoxButton)
        {
            if (deleteSpellIndex != -1 && messageBoxButton == DaggerfallMessageBox.MessageBoxButtons.Yes)
            {
                spellsListBox.RemoveItem(deleteSpellIndex);
                deleteSpellIndex = -1;
                RefreshSpellsList();
            }

            CloseWindow();
        }

        void ExitButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            CloseWindow();
        }

        void SwapButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            //if(sender.Name == downButton.Name && spellsListBox.SelectedIndex < spellsListBox.Count-1)
            //    spellsListBox.SwapItems(spellsListBox.SelectedIndex, ++spellsListBox.SelectedIndex);
            //else if(sender.Name == upButton.Name && spellsListBox.SelectedIndex > 0)
            //    spellsListBox.SwapItems(spellsListBox.SelectedIndex, --spellsListBox.SelectedIndex);
        }

        // Not implemented in Daggerfall, could be useful. Possibly move through different sorts (lexigraphic, date added, cost etc.)
        public void SortButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
        }

        public void SpellNameLabel_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallInputMessageBox renameSpellPrompt;
            renameSpellPrompt = new DaggerfallInputMessageBox(uiManager, this);
            renameSpellPrompt.SetTextBoxLabel(TextManager.Instance.GetText(textDatabase, "enterSpellName") + " ");
            renameSpellPrompt.TextBox.Text = spellsListBox.SelectedItem;
            renameSpellPrompt.OnGotUserInput += RenameSpellPromptHandler;
            uiManager.PushWindow(renameSpellPrompt);
        }

        public void RenameSpellPromptHandler(DaggerfallInputMessageBox sender, string input)
        {
            // Must not be blank
            if (string.IsNullOrEmpty(input))
                return;

            // TODO: Rename spell
        }

        #endregion
    }
}
