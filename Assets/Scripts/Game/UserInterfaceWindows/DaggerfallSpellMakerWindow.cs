// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
//
// Notes:
//

using UnityEngine;
using System;
using System.Collections.Generic;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.MagicAndEffects;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    public class DaggerfallSpellMakerWindow : DaggerfallPopupWindow
    {
        #region UI Rects

        DFSize baseSize = new DFSize(320, 200);
        Vector2 tipLabelPos = new Vector2(5, 22);
        Rect effect1NameRect = new Rect(3, 30, 314, 9);
        Rect effect2NameRect = new Rect(3, 62, 314, 9);
        Rect effect3NameRect = new Rect(3, 94, 314, 9);
        Rect addEffectButtonRect = new Rect(244, 114, 28, 28);

        #endregion

        #region UI Controls

        TextLabel tipLabel;
        TextLabel effect1NameLabel;
        TextLabel effect2NameLabel;
        TextLabel effect3NameLabel;

        DaggerfallListPickerWindow effectGroupPicker;
        DaggerfallListPickerWindow effectSubGroupPicker;
        DaggerfallEffectSettingsEditorWindow effectEditor;

        #endregion

        #region UI Textures

        Texture2D baseTexture;
        Texture2D effectSetupOverlayTexture;
        Texture2D goldSelectIconsTexture;
        Texture2D colorSelectIconsTexture;

        #endregion

        #region Fields

        const string textDatabase = "SpellmakerUI";

        const string baseTextureFilename = "INFO01I0.IMG";
        const string effectSetupOverlayFilename = "MASK05I0.IMG";
        const string goldSelectIconsFilename = "MASK01I0.IMG";
        const string colorSelectIconsFilename = "MASK04I0.IMG";

        const int alternateAlphaIndex = 12;
        const int maxEffectsPerSpell = 3;

        List<IEntityEffect> enumeratedEffectTemplates = new List<IEntityEffect>();

        EffectEntry[] effectEntries = new EffectEntry[maxEffectsPerSpell];

        int editOrDeleteSlot = -1;

        #endregion

        #region Constructors

        public DaggerfallSpellMakerWindow(IUserInterfaceManager uiManager, DaggerfallBaseWindow previous = null)
            : base(uiManager, previous)
        {
        }

        #endregion

        #region Setup Methods

        protected override void Setup()
        {
            // Load all the textures used by spell maker window
            LoadTextures();

            // Always dim background
            ParentPanel.BackgroundColor = ScreenDimColor;

            // Setup native panel background
            NativePanel.BackgroundTexture = baseTexture;

            // Setup controls
            SetupLabels();
            SetupButtons();
            SetupPickers();

            // Setup effect editor window
            effectEditor = new DaggerfallEffectSettingsEditorWindow(uiManager, this);
            effectEditor.OnClose += EffectEditor_OnClose;

            // TEMP: Launch effect editor immediately to help with UI design process
            // This will be removed after effect editor window is more functional
            //effectEditor.EffectTemplate = GameManager.Instance.EntityEffectBroker.GetEffectTemplate("ContinuousDamage-Health");
            //uiManager.PushWindow(effectEditor);
        }

        public override void OnPush()
        {
            InitEffectSlots();

            if (IsSetup)
            {
                effect1NameLabel.Text = string.Empty;
                effect2NameLabel.Text = string.Empty;
                effect3NameLabel.Text = string.Empty;
            }
        }

        #endregion

        #region Private Methods

        void LoadTextures()
        {
            // Load source textures
            baseTexture = ImageReader.GetTexture(baseTextureFilename, 0, 0, true, alternateAlphaIndex);
            effectSetupOverlayTexture = ImageReader.GetTexture(effectSetupOverlayFilename);
            goldSelectIconsTexture = ImageReader.GetTexture(goldSelectIconsFilename);
            colorSelectIconsTexture = ImageReader.GetTexture(colorSelectIconsFilename);
        }

        void SetupLabels()
        {
            // Tip label
            tipLabel = DaggerfallUI.AddTextLabel(DaggerfallUI.DefaultFont, tipLabelPos, string.Empty, NativePanel);

            // Effect1
            Panel effect1NamePanel = DaggerfallUI.AddPanel(effect1NameRect, NativePanel);
            effect1NamePanel.OnMouseClick += Effect1NamePanel_OnMouseClick;
            effect1NameLabel = DaggerfallUI.AddTextLabel(DaggerfallUI.LargeFont, Vector2.zero, string.Empty, effect1NamePanel);
            effect1NameLabel.HorizontalAlignment = HorizontalAlignment.Center;
            effect1NameLabel.ShadowPosition = Vector2.zero;

            // Effect2
            Panel effect2NamePanel = DaggerfallUI.AddPanel(effect2NameRect, NativePanel);
            effect2NamePanel.OnMouseClick += Effect2NamePanel_OnMouseClick;
            effect2NameLabel = DaggerfallUI.AddTextLabel(DaggerfallUI.LargeFont, Vector2.zero, string.Empty, effect2NamePanel);
            effect2NameLabel.HorizontalAlignment = HorizontalAlignment.Center;
            effect2NameLabel.ShadowPosition = Vector2.zero;

            // Effect3
            Panel effect3NamePanel = DaggerfallUI.AddPanel(effect3NameRect, NativePanel);
            effect3NamePanel.OnMouseClick += Effect3NamePanel_OnMouseClick;
            effect3NameLabel = DaggerfallUI.AddTextLabel(DaggerfallUI.LargeFont, Vector2.zero, string.Empty, effect3NamePanel);
            effect3NameLabel.HorizontalAlignment = HorizontalAlignment.Center;
            effect3NameLabel.ShadowPosition = Vector2.zero;
        }

        void SetupPickers()
        {
            // Use a picker for effect group
            effectGroupPicker = new DaggerfallListPickerWindow(uiManager, this);
            effectGroupPicker.ListBox.OnUseSelectedItem += AddEffectGroupListBox_OnUseSelectedItem;

            // Use another picker for effect subgroup
            // This allows user to hit escape and return to effect group list, unlike classic which dumps whole spellmaker UI
            effectSubGroupPicker = new DaggerfallListPickerWindow(uiManager, this);
            effectSubGroupPicker.ListBox.OnUseSelectedItem += AddEffectSubGroup_OnUseSelectedItem;
        }

        void SetupButtons()
        {
            // Add effect
            Button addEffectButton = DaggerfallUI.AddButton(addEffectButtonRect, NativePanel);
            addEffectButton.OnMouseEnter += AddEffectButton_OnMouseEnter;
            addEffectButton.OnMouseLeave += TipButton_OnMouseLeave;
            addEffectButton.OnMouseClick += AddEffectButton_OnMouseClick;
        }

        void InitEffectSlots()
        {
            effectEntries = new EffectEntry[maxEffectsPerSpell];
            for (int i = 0; i < maxEffectsPerSpell; i++)
            {
                effectEntries[i] = new EffectEntry();
            }
        }

        void ClearEffectSlot(int slot)
        {
            effectEntries[slot] = new EffectEntry();
            UpdateSlotText(slot, string.Empty);
        }

        int GetFreeEffectSlotIndex()
        {
            for (int i = 0; i < maxEffectsPerSpell; i++)
            {
                if (string.IsNullOrEmpty(effectEntries[i].Key))
                    return i;
            }

            return -1;
        }

        void UpdateSlotText(int slot, string text)
        {
            // Get text label to update
            TextLabel label = null;
            switch (slot)
            {
                case 0:
                    label = effect1NameLabel;
                    break;
                case 1:
                    label = effect2NameLabel;
                    break;
                case 2:
                    label = effect3NameLabel;
                    break;
                default:
                    return;
            }

            // Set label text
            label.Text = text;
        }

        void EditOrDeleteSlot(int slot)
        {
            const int howToAlterSpell = 1708;

            // Do nothing if slot not set
            if (string.IsNullOrEmpty(effectEntries[slot].Key))
                return;

            // Offer to edit or delete effect
            editOrDeleteSlot = slot;
            DaggerfallMessageBox mb = new DaggerfallMessageBox(uiManager, this);
            mb.SetTextTokens(howToAlterSpell);
            Button editButton = mb.AddButton(DaggerfallMessageBox.MessageBoxButtons.Edit);
            editButton.OnMouseClick += EditButton_OnMouseClick;
            Button deleteButton = mb.AddButton(DaggerfallMessageBox.MessageBoxButtons.Delete);
            deleteButton.OnMouseClick += DeleteButton_OnMouseClick;
            mb.OnButtonClick += EditOrDeleteSpell_OnButtonClick;
            mb.OnCancel += EditOrDeleteSpell_OnCancel;
            mb.Show();
        }

        #endregion

        #region Button Events

        private void AddEffectButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            const int noMoreThan3Effects = 1707;

            // Must have a free effect slot
            if (GetFreeEffectSlotIndex() == -1)
            {
                DaggerfallMessageBox mb = new DaggerfallMessageBox(
                    uiManager,
                    DaggerfallMessageBox.CommonMessageBoxButtons.Nothing,
                    DaggerfallUnity.Instance.TextProvider.GetRSCTokens(noMoreThan3Effects),
                    this);
                mb.ClickAnywhereToClose = true;
                mb.Show();
                return;
            }

            // Clear existing
            effectGroupPicker.ListBox.ClearItems();
            tipLabel.Text = string.Empty;

            // Populate group names
            string[] groupNames = GameManager.Instance.EntityEffectBroker.GetGroupNames();
            effectGroupPicker.ListBox.AddItems(groupNames);
            effectGroupPicker.ListBox.SelectedIndex = 0;

            // Show effect group picker
            uiManager.PushWindow(effectGroupPicker);
        }

        private void AddEffectGroupListBox_OnUseSelectedItem()
        {
            // Clear existing
            effectSubGroupPicker.ListBox.ClearItems();
            enumeratedEffectTemplates.Clear();

            // Enumerate subgroup effect key name pairs
            enumeratedEffectTemplates = GameManager.Instance.EntityEffectBroker.GetEffectTemplates(effectGroupPicker.ListBox.SelectedItem);
            if (enumeratedEffectTemplates.Count < 1)
                throw new Exception(string.Format("Could not find any effect templates for group {0}", effectGroupPicker.ListBox.SelectedItem));

            // Sort list by subgroup name
            enumeratedEffectTemplates.Sort((s1, s2) => s1.SubGroupName.CompareTo(s2.SubGroupName));

            // Populate subgroup names in list box
            foreach(IEntityEffect effect in enumeratedEffectTemplates)
            {
                effectSubGroupPicker.ListBox.AddItem(effect.SubGroupName);
            }
            effectSubGroupPicker.ListBox.SelectedIndex = 0;

            // Show effect subgroup picker
            // Note: In classic the group name is now shown (and mostly obscured) behind the picker at first available effect slot
            // This is not easily visible and not sure if this really communicates anything useful to user
            // Daggerfall Unity also allows user to cancel via escape back to previous dialog, so changing this beheaviour intentionally
            uiManager.PushWindow(effectSubGroupPicker);
        }

        private void AddEffectSubGroup_OnUseSelectedItem()
        {
            // Close effect pickers
            effectGroupPicker.CloseWindow();
            effectSubGroupPicker.CloseWindow();

            // Get selected effect from those on offer
            IEntityEffect effectTemplate = enumeratedEffectTemplates[effectSubGroupPicker.ListBox.SelectedIndex];
            if (effectTemplate != null)
            {
                effectEditor.EffectTemplate = effectTemplate;
                //Debug.LogFormat("Selected effect {0} {1} with key {2}", effectTemplate.GroupName, effectTemplate.SubGroupName, effectTemplate.Key);
            }

            // Launch effect editor window
            uiManager.PushWindow(effectEditor);
        }

        private void EditOrDeleteSpell_OnCancel(DaggerfallPopupWindow sender)
        {
            editOrDeleteSlot = -1;
        }

        private void EditOrDeleteSpell_OnButtonClick(DaggerfallMessageBox sender, DaggerfallMessageBox.MessageBoxButtons messageBoxButton)
        {
            sender.CloseWindow();
        }

        private void DeleteButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            // Delete effect entry
            ClearEffectSlot(editOrDeleteSlot);
            editOrDeleteSlot = -1;
        }

        private void EditButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            // Edit effect entry
            effectEditor.EffectEntry = effectEntries[editOrDeleteSlot];
            uiManager.PushWindow(effectEditor);
        }

        #endregion

        #region Effect Editor Events

        private void EffectEditor_OnClose()
        {
            // Only proceed if user hit "exit" to accept changes
            // Otherwise this is a result of user cancelling with escape key
            if (!effectEditor.UserExit)
                return;

            // Assign effect entry to slot
            int slot = (editOrDeleteSlot == -1) ? GetFreeEffectSlotIndex() : editOrDeleteSlot;
            effectEntries[slot] = effectEditor.EffectEntry;
            UpdateSlotText(slot, effectEditor.EffectTemplate.DisplayName);
            editOrDeleteSlot = -1;
        }

        private void Effect1NamePanel_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            EditOrDeleteSlot(0);
        }

        private void Effect2NamePanel_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            EditOrDeleteSlot(1);
        }

        private void Effect3NamePanel_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            EditOrDeleteSlot(2);
        }

        #endregion

        #region Tip Events

        private void AddEffectButton_OnMouseEnter(BaseScreenComponent sender)
        {
            tipLabel.Text = TextManager.Instance.GetText(textDatabase, "addEffect");
        }

        private void TipButton_OnMouseLeave(BaseScreenComponent sender)
        {
            tipLabel.Text = string.Empty;
        }

        #endregion
    }
}