// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Lypyl (lypyldf@gmail.com), Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    Allofich, Hazelnut
// 
// Notes:
//

using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.MagicAndEffects;
using DaggerfallWorkshop.Game.Formulas;
using DaggerfallWorkshop.Utility;
using DaggerfallConnect.Save;
using DaggerfallConnect.Arena2;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Spellbook UI for both casting spells and purchasing spells from guilds.
    /// </summary>
    public class DaggerfallSpellBookWindow : DaggerfallPopupWindow, IMacroContextProvider
    {
        #region UI Rects

        Vector2 spellNameLabelPos = new Vector2(123, 2);
        Vector2 spellPointsLabelPos = new Vector2(214, 2);
        Vector2 spellCostLabelPos = new Vector2(76, 154);

        Rect mainPanelRect = new Rect(0, 0, 259, 164);
        Rect spellsListBoxRect = new Rect(5, 13, 110, 130);
        Rect deleteOrBuyButtonRect = new Rect(3, 152, 38, 9);
        //Rect upButtonRect = new Rect(48, 152, 38, 9);
        //Rect downButtonRect = new Rect(132, 152, 38, 9);
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
        Button buyButton;
        //Button downButton;
        //Button upButton;
        Button sortButton;
        Button upArrowButton;
        Button downArrowButton;

        TextLabel spellNameLabel;
        TextLabel spellPointsLabel;
        TextLabel spellCostLabel;
        TextLabel[] spellEffectLabels;

        #endregion

        #region UI Textures

        Texture2D baseTexture;

        #endregion

        #region Fields

        const string textDatabase = "SpellmakerUI";
        const string spellBookTextureFilename = "SPBK00I0.IMG";
        const string spellBookBuyModeTextureFilename = "SPBK01I0.IMG";
        const string spellsFilename = "SPELLS.STD";

        const SoundClips openSpellBook = SoundClips.OpenBook;
        const SoundClips openSpellBookBuyMode = SoundClips.ButtonClick;
        const SoundClips closeSpellBook = SoundClips.PageTurn;

        bool buyMode = false;
        bool autoClose = false;
        int deleteSpellIndex = -1;
        KeyCode toggleClosedBinding;
        List<EffectBundleSettings> offeredSpells = new List<EffectBundleSettings>();
        PlayerGPS.DiscoveredBuilding buildingDiscoveryData;
        int presentedCost;

        #endregion

        #region Constructors

        public DaggerfallSpellBookWindow(IUserInterfaceManager uiManager, DaggerfallBaseWindow previous = null, bool buyMode = false)
            : base(uiManager, previous)
        {
            this.buyMode = buyMode;
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

            // Store toggle closed binding for this window
            toggleClosedBinding = InputManager.Instance.GetBinding(InputManager.Actions.CastSpell);
        }

        public override void OnPush()
        {
            if (buyMode && GameManager.Instance.PlayerEnterExit.IsPlayerInside)
                buildingDiscoveryData = GameManager.Instance.PlayerEnterExit.BuildingDiscoveryData;

            if (IsSetup)
            {
                RefreshSpellsList();
                SetDefaults();
            }

            if (!buyMode)
                DaggerfallUI.Instance.PlayOneShot(openSpellBook);
            else
                DaggerfallUI.Instance.PlayOneShot(openSpellBookBuyMode);
        }

        public override void OnPop()
        {
            if (!buyMode)
                DaggerfallUI.Instance.PlayOneShot(closeSpellBook);
        }

        void SetDefaults()
        {
            // Set spell points label
            if (!buyMode)
            {
                int curSpellPoints = GameManager.Instance.PlayerEntity.CurrentMagicka;
                int maxSpellPoints = GameManager.Instance.PlayerEntity.MaxMagicka;
                spellPointsLabel.Text = string.Format("{0}/{1}", curSpellPoints, maxSpellPoints);
            }

            // Default selected spell info
            spellNameLabel.Text = string.Empty;
            spellIconPanel.BackgroundTexture = null;
            spellTargetIconPanel.BackgroundTexture = null;
            spellElementIconPanel.BackgroundTexture = null;
            ClearEffectLabels();

            // Select default spell
            spellsListBox.SelectedIndex = 0;
        }

        public override void Update()
        {
            base.Update();

            // Toggle window closed with same hotkey used to open it
            if (Input.GetKeyUp(toggleClosedBinding))
                CloseWindow();
        }

        void RefreshSpellsList()
        {
            // Clear existing list
            spellsListBox.ClearItems();

            // Add spells based on mode
            if (buyMode)
            {
                // Load spells for sale
                offeredSpells.Clear();
                List<SpellRecord.SpellRecordData> standardSpells = DaggerfallSpellReader.ReadSpellsFile(Path.Combine(DaggerfallUnity.Arena2Path, spellsFilename));
                if (standardSpells == null || standardSpells.Count == 0)
                {
                    Debug.LogError("Failed to load SPELLS.STD for spellbook in buy mode.");
                    return;
                }

                for (int i = 0; i < standardSpells.Count; i++)
                {
                    // Filter internal spells starting with exclamation point '!'
                    if (standardSpells[i].spellName.StartsWith("!"))
                        continue;

                    // NOTE: Classic allows purchase of duplicate spells
                    // If ever changing this, must ensure spell is an *exact* duplicate (i.e. not a custom spell with same name)
                    // Just allowing duplicates for now as per classic and let user manage preference

                    // Get effect bundle settings from classic spell
                    EffectBundleSettings bundle;
                    if (!GameManager.Instance.EntityEffectBroker.ClassicSpellRecordDataToEffectBundleSettings(standardSpells[i], BundleTypes.Spell, out bundle))
                        continue;

                    // Store offered spell and add to list box
                    offeredSpells.Add(bundle);
                    spellsListBox.AddItem(standardSpells[i].spellName);
                }
            }
            else
            {
                // Add player spells to list
                EffectBundleSettings[] spellbook = GameManager.Instance.PlayerEntity.GetSpells();
                if (spellbook != null)
                {
                    for (int i = 0; i < spellbook.Length; i++)
                    {
                        // Show spell name and cost
                        // Costs can change based on player skills and stats so must be calculated each time
                        int goldCost, spellPointCost;
                        FormulaHelper.CalculateTotalEffectCosts(spellbook[i].Effects, spellbook[i].TargetType, out goldCost, out spellPointCost, null, spellbook[i].MinimumCastingCost);
                        spellsListBox.AddItem(string.Format("{0} - {1}", spellPointCost, spellbook[i].Name));
                    }
                }
            }
        }

        #endregion

        #region Private Methods

        void LoadTextures()
        {
            if (!buyMode)
                baseTexture = DaggerfallUI.GetTextureFromImg(spellBookTextureFilename);
            else
                baseTexture = DaggerfallUI.GetTextureFromImg(spellBookBuyModeTextureFilename);
        }

        void SetupMain()
        {
            // Main panel
            mainPanel = DaggerfallUI.AddPanel(mainPanelRect, NativePanel);
            mainPanel.BackgroundTexture = baseTexture;
            mainPanel.HorizontalAlignment = HorizontalAlignment.Center;
            mainPanel.VerticalAlignment = VerticalAlignment.Middle;
            if (buyMode)
                mainPanel.BackgroundColor = Color.black;

            // Spells list
            spellsListBox = new ListBox();
            spellsListBox.Position = new Vector2(spellsListBoxRect.x, spellsListBoxRect.y);
            spellsListBox.Size = new Vector2(spellsListBoxRect.width, spellsListBoxRect.height);
            spellsListBox.RowsDisplayed = 16;
            spellsListBox.MaxCharacters = 22;
            spellsListBox.OnSelectItem += SpellsListBox_OnSelectItem;
            if (buyMode)
                spellsListBox.OnMouseDoubleClick += SpellsListBox_OnMouseDoubleClick;
            else
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

            // TODO: Prepare UI for spell buy mode
        }

        void SetupButtons()
        {
            // Bottom row buttons
            if (!buyMode)
            {
                deleteButton = DaggerfallUI.AddButton(deleteOrBuyButtonRect, mainPanel);
                deleteButton.OnMouseClick += DeleteButton_OnMouseClick;

                //upButton = DaggerfallUI.AddButton(upButtonRect, mainPanel);
                sortButton = DaggerfallUI.AddButton(sortButtonRect, mainPanel);
                //downButton = DaggerfallUI.AddButton(downButtonRect, mainPanel);

                sortButton.OnMouseClick += SortButton_OnMouseClick;
            }
            else
            {
                buyButton = DaggerfallUI.AddButton(deleteOrBuyButtonRect, mainPanel);
                buyButton.OnMouseClick += BuyButton_OnMouseClick;
            }

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

            // Spell cost
            if (buyMode)
            {
                spellCostLabel = DaggerfallUI.AddTextLabel(DaggerfallUI.DefaultFont, spellCostLabelPos, string.Empty, mainPanel);
                spellCostLabel.ShadowPosition = Vector2.zero;
            }

            // Spell points
            if (!buyMode)
            {
                spellPointsLabel = DaggerfallUI.AddTextLabel(DaggerfallUI.DefaultFont, spellPointsLabelPos, string.Empty, mainPanel);
                spellPointsLabel.ShadowColor = DaggerfallUI.DaggerfallAlternateShadowColor1;
            }

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

            // Get spell settings selected from player spellbook or offered spells
            EffectBundleSettings spellSettings;
            if (buyMode)
            {
                spellSettings = offeredSpells[spellsListBox.SelectedIndex];

                // The price shown in buy mode is the player casting cost * 4
                int goldCost, spellPointCost;
                FormulaHelper.CalculateTotalEffectCosts(spellSettings.Effects, spellSettings.TargetType, out goldCost, out spellPointCost);
                presentedCost = spellPointCost * 4;

                // Presented cost is halved on Witches Festival holiday
                uint gameMinutes = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime();
                int holidayID = FormulaHelper.GetHolidayId(gameMinutes, 0);
                if (holidayID == (int)DaggerfallConnect.DFLocation.Holidays.Witches_Festival)
                {
                    presentedCost >>= 1;
                    if (presentedCost == 0)
                        presentedCost = 1;
                }

                spellCostLabel.Text = presentedCost.ToString();
            }
            else
            {
                // Get spell and exit if spell index not found
                if (!GameManager.Instance.PlayerEntity.GetSpell(spellsListBox.SelectedIndex, out spellSettings))
                    return;
            }

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
            spellEffectLabels[labelIndex].Text = effect.Properties.GroupName;
            spellEffectLabels[labelIndex + 1].Text = effect.Properties.SubGroupName;
        }

        void ShowEffectPopup(IEntityEffect effect)
        {
            if (effect == null)
                return;

            DaggerfallMessageBox spellEffectPopup = new DaggerfallMessageBox(uiManager, this);
            spellEffectPopup.ClickAnywhereToClose = true;
            spellEffectPopup.SetTextTokens(effect.Properties.SpellBookDescription, effect);
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

        int GetTradePrice()
        {
            return FormulaHelper.CalculateTradePrice(presentedCost, buildingDiscoveryData.quality, false);
        }

        #endregion

        #region Macro handling

        public MacroDataSource GetMacroDataSource()
        {
            return new SpellbookMacroDataSource(this);
        }

        /// <summary>
        /// MacroDataSource context sensitive methods for spellbook window.
        /// </summary>
        private class SpellbookMacroDataSource : MacroDataSource
        {
            private DaggerfallSpellBookWindow parent;
            public SpellbookMacroDataSource(DaggerfallSpellBookWindow spellBookWindow)
            {
                this.parent = spellBookWindow;
            }

            public override string Amount()
            {
                return parent.GetTradePrice().ToString();
            }

            public override string ShopName()
            {
                return parent.buildingDiscoveryData.displayName;
            }

            public override string GuildTitle()
            {
                return MacroHelper.GetFirstname(GameManager.Instance.PlayerEntity.Name);
            }
        }

        #endregion

        #region Events

        void SpellEffectPanelClick(BaseScreenComponent sender, Vector2 position)
        {
            // Get spell settings
            EffectBundleSettings spellSettings;
            if (buyMode)
            {
                spellSettings = offeredSpells[spellsListBox.SelectedIndex];
            }
            else
            {
                if (!GameManager.Instance.PlayerEntity.GetSpell(spellsListBox.SelectedIndex, out spellSettings))
                    return;
            }

            // Settings must look valid
            if (spellSettings.Version < EntityEffectBroker.MinimumSupportedSpellVersion)
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
                DaggerfallUI.Instance.PopToHUD();
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
            spellsListBox.SelectPrevious();
        }

        private void DownArrowButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            spellsListBox.SelectNext();
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
                GameManager.Instance.PlayerEntity.DeleteSpell(deleteSpellIndex);
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
            // Just sort as alpha for now
            GameManager.Instance.PlayerEntity.SortSpellsAlpha();
            RefreshSpellsList();
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

        private void BuyButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            autoClose = false;
            BuySpellHandler();
        }

        private void SpellsListBox_OnMouseDoubleClick(BaseScreenComponent sender, Vector2 position)
        {
            autoClose = true;
            BuySpellHandler();
        }

        private void BuySpellHandler()
        {
            const int tradeMessageBaseId = 260;
            const int notEnoughGoldId = 454;
            int tradePrice = GetTradePrice();
            int msgOffset = 0;

            if (GameManager.Instance.PlayerEntity.GetGoldAmount() < tradePrice)
            {
                DaggerfallUI.MessageBox(notEnoughGoldId);
            }
            else
            {
                if (presentedCost >> 1 <= tradePrice)
                {
                    if (presentedCost - (presentedCost >> 2) <= tradePrice)
                        msgOffset = 2;
                    else
                        msgOffset = 1;
                }

                DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, this);
                TextFile.Token[] tokens = DaggerfallUnity.Instance.TextProvider.GetRandomTokens(tradeMessageBaseId + msgOffset);
                messageBox.SetTextTokens(tokens, this);
                messageBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.Yes);
                messageBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.No);
                messageBox.OnButtonClick += ConfirmTrade_OnButtonClick;
                uiManager.PushWindow(messageBox);
            }
        }

        private void ConfirmTrade_OnButtonClick(DaggerfallMessageBox sender, DaggerfallMessageBox.MessageBoxButtons messageBoxButton)
        {
            if (messageBoxButton == DaggerfallMessageBox.MessageBoxButtons.Yes)
            {
                // Deduct gold - adding gold sound for additional feedback
                GameManager.Instance.PlayerEntity.DeductGoldAmount(GetTradePrice());
                DaggerfallUI.Instance.PlayOneShot(SoundClips.GoldPieces);

                // Add to player entity spellbook
                GameManager.Instance.PlayerEntity.AddSpell(offeredSpells[spellsListBox.SelectedIndex]);
            }
            if (autoClose)
            {
                // Drop back to HUD like classic
                DaggerfallUI.Instance.PopToHUD();
            }
            else
                CloseWindow();
        }

        #endregion
    }
}
