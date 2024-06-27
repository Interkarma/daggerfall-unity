// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2023 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Lypyl (lypyl@dfworkshop.net)
// Contributors:    TheLacus
// 
// Notes:
//

using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using DaggerfallWorkshop.Game.Utility.ModSupport.ModSettings;

public class ModLoaderInterfaceWindow : DaggerfallPopupWindow
{
    private enum Stage
    {
        None,
        Cleanup,
        CheckDependencies,
        Close
    }

    struct ModSettings
    {
        public ModInfo modInfo;
        public bool enabled;
    }

    #region Fields

    DaggerfallMessageBox ModDescriptionMessageBox;

    readonly Panel ModPanel = new Panel();
    readonly Panel ModListPanel = new Panel();

    readonly ListBox modList = new ListBox();
    readonly VerticalScrollBar modListScrollBar = new VerticalScrollBar();

    readonly Button increaseLoadOrderButton  = new Button();
    readonly Button decreaseLoadOrderButton  = new Button();
    readonly Button backButton               = new Button();
    readonly Button refreshButton            = new Button();
    readonly Button enableAllButton          = new Button();
    readonly Button disableAllButton         = new Button();
    readonly Button saveAndCloseButton       = new Button();
    readonly Button extractFilesButton       = new Button();
    readonly Button showModDescriptionButton = new Button();
    readonly Button modSettingsButton        = new Button();

    readonly Checkbox modEnabledCheckBox         = new Checkbox();
    readonly TextLabel modLoadPriorityLabel      = new TextLabel();
    readonly TextLabel modTitleLabel             = new TextLabel();
    readonly TextLabel modVersionLabel           = new TextLabel();
    readonly TextLabel modAuthorLabel            = new TextLabel();
    readonly TextLabel modAuthorContactLabel     = new TextLabel();
    readonly TextLabel modDFTFUVersionLabel      = new TextLabel();
    readonly TextLabel modsFound                 = new TextLabel();

    readonly Color backgroundColor = new Color(0, 0, 0, 0.7f);
    readonly Color unselectedTextColor = new Color(0.6f, 0.6f, 0.6f, 1f);
    readonly Color selectedTextColor = new Color(0.0f, 0.8f, 0.0f, 1.0f);
    readonly Color textColor = new Color(0.0f, 0.5f, 0.0f, 0.4f);
    readonly Color disabledModTextColor = new Color(0.35f, 0.35f, 0.35f, 1);
    readonly Color disabledButtonBackground = new Color(0.35f, 0.35f, 0.35f, 0.4f);

    Stage currentStage = Stage.None;
    bool moveNextStage = false;

    int currentSelection = -1;
    ModSettings[] modSettings;

    #endregion

    #region Constructors

    public ModLoaderInterfaceWindow(IUserInterfaceManager uiManager)
    : base(uiManager)
    {
    }

    #endregion

    #region Methods

    protected override void Setup()
    {
        ParentPanel.BackgroundColor = Color.clear;

        ModListPanel.Outline.Enabled = true;
        ModListPanel.BackgroundColor = backgroundColor;
        ModListPanel.HorizontalAlignment = HorizontalAlignment.Left;
        ModListPanel.VerticalAlignment = VerticalAlignment.Middle;
        ModListPanel.Size = new Vector2(120, 175);
        NativePanel.Components.Add(ModListPanel);

        modsFound.HorizontalAlignment = HorizontalAlignment.Center;
        modsFound.Position = new Vector2(10, 20);
        modsFound.Text = string.Format("{0}: ", ModManager.GetText("modsFound"));
        ModListPanel.Components.Add(modsFound);

        modList.BackgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.5f);
        modList.Size = new Vector2(110, 115);
        modList.HorizontalAlignment = HorizontalAlignment.Center;
        modList.VerticalAlignment = VerticalAlignment.Middle;
        modList.TextColor = unselectedTextColor;
        modList.SelectedTextColor = textColor;
        modList.ShadowPosition = Vector2.zero;
        modList.RowsDisplayed = 14;
        modList.RowAlignment = HorizontalAlignment.Left;
        modList.LeftMargin += 4;
        modList.SelectedShadowPosition = DaggerfallUI.DaggerfallDefaultShadowPos;
        modList.SelectedShadowColor = Color.black;
        modList.OnScroll += ModList_OnScroll;
        ModListPanel.Components.Add(modList);

        modListScrollBar.Size = new Vector2(5, 115);
        modListScrollBar.HorizontalAlignment = HorizontalAlignment.Right;
        modListScrollBar.VerticalAlignment = VerticalAlignment.Middle;
        modListScrollBar.Position = new Vector2(100, 12);
        modListScrollBar.BackgroundColor = Color.grey;
        modListScrollBar.DisplayUnits = 14;
        modListScrollBar.TotalUnits = modList.Count;
        modListScrollBar.OnScroll += ModListScrollBar_OnScroll;
        ModListPanel.Components.Add(modListScrollBar);
        modList.ScrollToSelected();

        backButton.Size = new Vector2(45, 12);
        backButton.Label.Text = string.Format("< {0}", ModManager.GetText("backToOptions"));
        backButton.Label.ShadowPosition = Vector2.zero;
        backButton.Label.TextColor = Color.gray;
        backButton.ToolTip = defaultToolTip;
        backButton.ToolTipText = ModManager.GetText("backToOptionsInfo");
        backButton.VerticalAlignment = VerticalAlignment.Top;
        backButton.HorizontalAlignment = HorizontalAlignment.Left;
        backButton.OnMouseClick +=  BackButton_OnMouseClick;
        backButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.GameSetupBackToOptions);
        ModListPanel.Components.Add(backButton);

        increaseLoadOrderButton.Size = new Vector2(40, 12);
        increaseLoadOrderButton.Position = new Vector2(62, 150);
        increaseLoadOrderButton.Outline.Enabled = true;
        increaseLoadOrderButton.BackgroundColor = textColor;
        increaseLoadOrderButton.Label.Text = ModManager.GetText("increase");
        increaseLoadOrderButton.OnMouseClick += IncreaseLoadOrderButton_OnMouseClick;
        ModListPanel.Components.Add(increaseLoadOrderButton);

        decreaseLoadOrderButton.Size = new Vector2(40, 12);
        decreaseLoadOrderButton.Position = new Vector2(21, 150);
        decreaseLoadOrderButton.Outline.Enabled = true;
        decreaseLoadOrderButton.BackgroundColor = textColor;
        decreaseLoadOrderButton.Label.Text = ModManager.GetText("lower");
        decreaseLoadOrderButton.OnMouseClick += DecreaseLoadOrderButton_OnMouseClick;
        ModListPanel.Components.Add(decreaseLoadOrderButton);

        enableAllButton.Size = new Vector2(40, 12);
        enableAllButton.Position = new Vector2(21, 163);
        enableAllButton.Outline.Enabled = true;
        enableAllButton.BackgroundColor = textColor;
        enableAllButton.VerticalAlignment = VerticalAlignment.Bottom;
        enableAllButton.Label.Text = ModManager.GetText("enableAll");
        enableAllButton.ToolTipText = ModManager.GetText("enableAllInfo");
        enableAllButton.OnMouseClick += EnableAllButton_OnMouseClick;
        ModListPanel.Components.Add(enableAllButton);

        disableAllButton.Size = new Vector2(40, 12);
        disableAllButton.Position = new Vector2(62, 163);
        disableAllButton.Outline.Enabled = true;
        disableAllButton.BackgroundColor = textColor;
        disableAllButton.VerticalAlignment = VerticalAlignment.Bottom;
        disableAllButton.Label.Text = ModManager.GetText("disableAll");
        disableAllButton.ToolTipText = ModManager.GetText("disableAllInfo");
        disableAllButton.OnMouseClick += DisableAllButton_OnMouseClick;
        ModListPanel.Components.Add(disableAllButton);

        //Add main mod panel
        ModPanel.Outline.Enabled = true;
        ModPanel.BackgroundColor = backgroundColor;
        ModPanel.HorizontalAlignment = HorizontalAlignment.Right;
        ModPanel.VerticalAlignment = VerticalAlignment.Middle;
        ModPanel.Size = new Vector2(200, 175);
        NativePanel.Components.Add(ModPanel);

        modEnabledCheckBox.Label.Text = ModManager.GetText("enabled");
        modEnabledCheckBox.Label.TextColor = selectedTextColor;
        modEnabledCheckBox.CheckBoxColor = selectedTextColor;
        modEnabledCheckBox.ToolTip = defaultToolTip;
        modEnabledCheckBox.ToolTipText = ModManager.GetText("enabledInfo");
        modEnabledCheckBox.IsChecked = true;
        modEnabledCheckBox.Position = new Vector2(1, 25);
        modEnabledCheckBox.OnToggleState += ModEnabledCheckBox_OnToggleState;
        ModPanel.Components.Add(modEnabledCheckBox);

        modLoadPriorityLabel.Position = new Vector2(60, 25);
        ModPanel.Components.Add(modLoadPriorityLabel);

        modTitleLabel.Position = new Vector2(0, 5);
        modTitleLabel.HorizontalAlignment = HorizontalAlignment.Center;
        modTitleLabel.MaxCharacters = 40;
        ModPanel.Components.Add(modTitleLabel);

        modVersionLabel.Position = new Vector2(5, 40);
        modVersionLabel.MaxCharacters = 40;
        ModPanel.Components.Add(modVersionLabel);

        modAuthorLabel.Position = new Vector2(5, 50);
        modAuthorLabel.MaxCharacters = 40;
        ModPanel.Components.Add(modAuthorLabel);

        modAuthorContactLabel.Position = new Vector2(5, 60);
        modAuthorContactLabel.MaxCharacters = 40;
        ModPanel.Components.Add(modAuthorContactLabel);

        modDFTFUVersionLabel.Position = new Vector2(5, 70);
        modDFTFUVersionLabel.MaxCharacters = 40;
        ModPanel.Components.Add(modDFTFUVersionLabel);

        showModDescriptionButton.Position = new Vector2(5, 95);
        showModDescriptionButton.Size = new Vector2(75, 12);
        showModDescriptionButton.HorizontalAlignment = HorizontalAlignment.Center;
        showModDescriptionButton.Label.Text = ModManager.GetText("modDescription");
        showModDescriptionButton.BackgroundColor = textColor;
        showModDescriptionButton.Outline.Enabled = true;
        showModDescriptionButton.OnMouseClick += ShowModDescriptionPopUp_OnMouseClick;
        ModPanel.Components.Add(showModDescriptionButton);

        refreshButton.Size = new Vector2(50, 12);
        refreshButton.Position = new Vector2(5, 139);
        refreshButton.Outline.Enabled = true;
        refreshButton.BackgroundColor = textColor;
        refreshButton.HorizontalAlignment = HorizontalAlignment.Center;
        refreshButton.Label.Text = ModManager.GetText("refresh");
        refreshButton.Label.ToolTipText = ModManager.GetText("RrefreshInfo");
        refreshButton.OnMouseClick += RefreshButton_OnMouseClick;
        refreshButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.GameSetupRefresh);
        ModPanel.Components.Add(refreshButton);

        saveAndCloseButton.Size = new Vector2(70, 12);
        saveAndCloseButton.Outline.Enabled = true;
        saveAndCloseButton.BackgroundColor = textColor;
        saveAndCloseButton.VerticalAlignment = VerticalAlignment.Bottom;
        saveAndCloseButton.HorizontalAlignment = HorizontalAlignment.Center;
        saveAndCloseButton.Label.Text = ModManager.GetText("saveClose");
        saveAndCloseButton.Label.ToolTipText = ModManager.GetText("saveCloseInfo");
        saveAndCloseButton.OnMouseClick += SaveAndCloseButton_OnMouseClick;
        saveAndCloseButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.GameSetupSaveAndClose);
        ModPanel.Components.Add(saveAndCloseButton);

        extractFilesButton.Size = new Vector2(60, 12);
        extractFilesButton.Position = new Vector2(5, 117);
        extractFilesButton.Outline.Enabled = true;
        extractFilesButton.BackgroundColor = textColor;
        extractFilesButton.HorizontalAlignment = HorizontalAlignment.Center;
        extractFilesButton.Label.Text = ModManager.GetText("extractText");
        extractFilesButton.Label.ToolTipText = ModManager.GetText("extractTextInfo");
        extractFilesButton.OnMouseClick += ExtractFilesButton_OnMouseClick;
        ModPanel.Components.Add(extractFilesButton);

        modSettingsButton.Size = new Vector2(60, 12);
        modSettingsButton.Position = new Vector2(5, 103);
        modSettingsButton.Outline.Enabled = true;
        modSettingsButton.BackgroundColor = textColor;
        modSettingsButton.HorizontalAlignment = HorizontalAlignment.Center;
        modSettingsButton.Label.Text = ModManager.GetText("settings");
        modSettingsButton.Label.ToolTipText = ModManager.GetText("settingsInfo");
        modSettingsButton.OnMouseClick += ModSettingsButton_OnMouseClick;
        modSettingsButton.Enabled = false;
        ModPanel.Components.Add(modSettingsButton);

        GetLoadedMods();
        UpdateModPanel();
    }

    public override void Update()
    {
        base.Update();

        if(currentSelection != modList.SelectedIndex && modList.Count > 0)
        {
            currentSelection = modList.SelectedIndex;
            UpdateModPanel();
        }

        modListScrollBar.TotalUnits = modList.Count;
        modListScrollBar.DisplayUnits = modList.RowsDisplayed;

        if (modListScrollBar.DraggingThumb)
        {
            modList.ScrollIndex = modListScrollBar.ScrollIndex;
        }
        else
        {
            modListScrollBar.ScrollIndex = modList.ScrollIndex;
        }

        if (moveNextStage)
        {
            moveNextStage = false;
            MoveNextStage();
        }
    }

    bool GetModSettings(ref ModSettings ms)
    {
         if (modList.SelectedIndex < 0 || modList.SelectedIndex > modSettings.Count())
            return false;

         ms = modSettings[modList.SelectedIndex];
         return ms.modInfo != null;
    }

    void GetLoadedMods()
    {
        var mods = ModManager.Instance.GetAllMods();

        modList.ClearItems();

        if(modSettings == null || modSettings.Length != mods.Length)
        {
            modSettings = new ModSettings[mods.Length];
        }

        for (int i = 0; i < mods.Length; i++)
        {
            ModSettings modsett = new ModSettings();
            modsett.modInfo = mods[i].ModInfo;
            modsett.enabled = mods[i].Enabled;
            modSettings[i] = modsett;
            modList.AddItem(modsett.modInfo.ModTitle, out ListBox.ListItem item);
            item.textColor = modsett.enabled ? unselectedTextColor : disabledModTextColor;
        }

        if (modList.SelectedIndex < 0 || modList.SelectedIndex >= modList.Count)
        {
            modList.SelectedIndex = 0;
        }
        mods = null;
    }

    void UpdateModPanel()
    {
        modLoadPriorityLabel.Text   = string.Format("{0}: ", ModManager.GetText("modLoadPriority"));
        modTitleLabel.Text          = string.Format("{0}: ", ModManager.GetText("modTitle"));
        modVersionLabel.Text        = string.Format("{0}: ", ModManager.GetText("modVersion"));
        modAuthorLabel.Text         = string.Format("{0}: ", ModManager.GetText("modAuthor"));
        modAuthorContactLabel.Text  = string.Format("{0}: ", ModManager.GetText("modAuthorContact"));
        modDFTFUVersionLabel.Text   = string.Format("{0}: ", ModManager.GetText("modDFTFUVersion"));

        if (modSettings.Length < 1 || currentSelection < 0)
        {
            return;
        }

        ModSettings ms = modSettings[modList.SelectedIndex];

        if (ms.modInfo == null)
            return;

        modEnabledCheckBox.IsChecked = ms.enabled;
        modLoadPriorityLabel.Text   += modList.SelectedIndex;
        modTitleLabel.Text          += ms.modInfo.ModTitle;
        modVersionLabel.Text        += ms.modInfo.ModVersion;
        modAuthorLabel.Text         += ms.modInfo.ModAuthor;
        modAuthorContactLabel.Text  += ms.modInfo.ContactInfo;
        modDFTFUVersionLabel.Text   += ms.modInfo.DFUnity_Version;

        Mod mod = ModManager.Instance.GetMod(ms.modInfo.ModTitle);

        modDFTFUVersionLabel.TextColor = mod.IsGameVersionSatisfied() == false ? Color.red : DaggerfallUI.DaggerfallDefaultTextColor;

#if UNITY_EDITOR
        if (mod.IsVirtual)
            modTitleLabel.Text += " (debug)";
#endif

        bool hasDescription = !string.IsNullOrWhiteSpace(ms.modInfo.ModDescription);
        showModDescriptionButton.BackgroundColor = hasDescription ? textColor : disabledButtonBackground;

        // Update buttons
        if (mod.HasSettings)
        {
            modSettingsButton.Enabled = true;
            showModDescriptionButton.Position = new Vector2(5, 83);
            extractFilesButton.Position = new Vector2(5, 123);
            refreshButton.Position = new Vector2(5, 143);
        }
        else
        {
            modSettingsButton.Enabled = false;
            showModDescriptionButton.Position = new Vector2(5, 95);
            extractFilesButton.Position = new Vector2(5, 117);
            refreshButton.Position = new Vector2(5, 139);
        }
    }

    private void CleanConfigurationDirectory()
    {
        var unknownDirectories = Directory.GetDirectories(ModManager.Instance.ModDataDirectory)
            .Select(x => new DirectoryInfo(x))
            .Where(x => ModManager.Instance.GetModFromGUID(x.Name) == null)
            .ToArray();

        if (unknownDirectories.Length > 0)
        {
            var cleanConfigMessageBox = new DaggerfallMessageBox(uiManager, this);
            cleanConfigMessageBox.ParentPanel.BackgroundTexture = null;
            cleanConfigMessageBox.SetText(ModManager.GetText("cleanConfigurationDir"));
            cleanConfigMessageBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.Yes);
            cleanConfigMessageBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.No, true);
            cleanConfigMessageBox.OnButtonClick += (messageBox, messageBoxButton) =>
            {
                if (messageBoxButton == DaggerfallMessageBox.MessageBoxButtons.Yes)
                {
                    foreach (var directory in unknownDirectories)
                        directory.Delete(true);
                }

                messageBox.CancelWindow();
                moveNextStage = true;
            };
            uiManager.PushWindow(cleanConfigMessageBox);
        }
        else
        {
            moveNextStage = true;
        }
    }

    private void CheckDependencies()
    {
        bool hasSortIssues = false;
        List<string> errorMessages = null;
        var modErrorMessages = new List<string>();
        
        foreach (Mod mod in ModManager.Instance.Mods.Where(x => x.Enabled))
        {
            bool? isGameVersionSatisfied = mod.IsGameVersionSatisfied();
            if (!isGameVersionSatisfied.HasValue)
                Debug.LogErrorFormat("Mod {0} requires unknown game version ({1}).", mod.Title, mod.ModInfo.DFUnity_Version);
            else if (!isGameVersionSatisfied.Value)
                modErrorMessages.Add(string.Format(ModManager.GetText("gameVersionUnsatisfied"), mod.ModInfo.DFUnity_Version));

            ModManager.Instance.CheckModDependencies(mod, modErrorMessages, ref hasSortIssues);
            if (modErrorMessages.Count > 0)
            {
                if (errorMessages == null)
                {
                    errorMessages = new List<string>();
                    errorMessages.Add(ModManager.GetText("dependencyErrorMessage"));
                    errorMessages.Add(string.Empty);
                }

                errorMessages.Add(string.Format("- {0}", mod.Title));
                errorMessages.AddRange(modErrorMessages);
                errorMessages.Add(string.Empty);
                modErrorMessages.Clear();
            }
        }

        if (errorMessages != null && errorMessages.Count > 0)
        {
            if (hasSortIssues)
                errorMessages.Add(ModManager.GetText("sortModsQuestion"));

            var messageBox = new DaggerfallMessageBox(uiManager, this);
            messageBox.EnableVerticalScrolling(80);
            messageBox.SetText(errorMessages.ToArray());
            if (hasSortIssues)
            {
                messageBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.Yes);
                messageBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.No, true);
                messageBox.OnButtonClick += (sender, button) =>
                {
                    if (button == DaggerfallMessageBox.MessageBoxButtons.Yes)
                    {
                        ModManager.Instance.AutoSortMods();
                        Debug.Log("Mods have been sorted automatically");
                    }

                    sender.CancelWindow();
                    moveNextStage = true;
                };
            }
            else
            {
                messageBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.OK, true);
                messageBox.OnButtonClick += (sender, button) =>
                {
                    sender.CancelWindow();
                    moveNextStage = true;
                };
            }
            messageBox.Show();
        }
        else
        {
            moveNextStage = true;
        }
    }

    private void SaveAndClose()
    {
        ModManager.WriteModSettings();
        CloseWindow();
    }

    private void MoveNextStage()
    {
        switch (currentStage = (Stage)((int)currentStage + 1))
        {
            case Stage.Cleanup:
                CleanConfigurationDirectory();
                break;
            case Stage.CheckDependencies:
                CheckDependencies();
                break;
            default:
                SaveAndClose();
                break;
        }
    }

    #endregion

    #region Events

    void DecreaseLoadOrderButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
    {
        if (modList.Count < 2)
            return;
        else if (modList.SelectedIndex == modList.Count - 1)    //last index already
            return;

        modList.SwapItems(modList.SelectedIndex, modList.SelectedIndex + 1);

        ModSettings temp = modSettings[modList.SelectedIndex];
        modSettings[modList.SelectedIndex] = modSettings[modList.SelectedIndex + 1];
        modSettings[modList.SelectedIndex + 1] = temp;

        modList.SelectedIndex++;
    }

    void IncreaseLoadOrderButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
    {
        if (modList.Count < 2)
            return;
        else if (modList.SelectedIndex == 0)    //first priority already
            return;

        modList.SwapItems(modList.SelectedIndex, modList.SelectedIndex - 1);

        ModSettings temp = modSettings[modList.SelectedIndex];
        modSettings[modList.SelectedIndex] = modSettings[modList.SelectedIndex - 1];
        modSettings[modList.SelectedIndex - 1] = temp;

        modList.SelectedIndex--;
    }

    void RefreshButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
    {
        ModManager.Instance.Refresh();
        int count = modSettings.Length;
        GetLoadedMods();
        if (modSettings.Length != count)
            currentSelection = -1;
        UpdateModPanel();
    }

    void SaveAndCloseButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
    {
        if (modSettings == null)
        {
            return;
        }

        for (int i = 0; i < modSettings.Length; i++)
        {
            Mod mod = ModManager.Instance.GetMod(modSettings[i].modInfo.ModTitle);
            if (mod == null)
                continue;
            mod.Enabled = modSettings[i].enabled;
            mod.LoadPriority = i;
            mod = null;
        }

        ModManager.Instance.SortMods();
        MoveNextStage();
    }

    void ExtractFilesButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
    {
        if (modSettings.Length < 1)
            return;

        Mod mod = ModManager.Instance.GetMod(modSettings[modList.SelectedIndex].modInfo.ModTitle);

        if (mod == null)
        {
            return;
        }

        string[] assets = mod.AssetNames;
        if (assets == null)
            return;

        string path = Path.Combine(DaggerfallUnityApplication.PersistentDataPath, "Mods", "ExtractedFiles", mod.FileName);
        Directory.CreateDirectory(path);

        for (int i = 0; i < assets.Length; i++)
        {
            string extension = Path.GetExtension(assets[i]);
            if (!ModManager.textExtensions.Contains(extension))
                continue;

            var asset = mod.GetAsset<TextAsset>(assets[i]);
            if (asset == null)
                continue;

            if (assets[i].EndsWith(".bytes", StringComparison.Ordinal))
            {
                // Export binary asset without .bytes extension
                File.WriteAllBytes(Path.Combine(path, asset.name), asset.bytes);
            }
            else if (assets[i].EndsWith(".cs.txt", StringComparison.Ordinal))
            {
                // Export C# script without .txt extension
                File.WriteAllText(Path.Combine(path, asset.name), asset.text);
            }
            else
            {
                // Export text asset with original extension
                File.WriteAllText(Path.Combine(path, asset.name + extension), asset.text);
            }
        }

        var messageBox = new DaggerfallMessageBox(uiManager, this, true);
        messageBox.AllowCancel = true;
        messageBox.ClickAnywhereToClose = true;
        messageBox.ParentPanel.BackgroundTexture = null;
        messageBox.SetText(string.Format(ModManager.GetText("extractTextConfirmation"), path));
        uiManager.PushWindow(messageBox);
    }

    void BackButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
    {
        DaggerfallUI.UIManager.PopWindow();
    }

    void EnableAllButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
    {
        if (modSettings == null || modSettings.Length < 1)
            return;

        for (int i = 0; i < modSettings.Length; i++)
        {
            modSettings[i].enabled = true;
            modList.GetItem(i).textColor = unselectedTextColor;
        }
        UpdateModPanel();
    }

    void DisableAllButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
    {
        if (modSettings == null || modSettings.Length < 1)
            return;

        for (int i = 0; i < modSettings.Length; i++)
        {
            modSettings[i].enabled = false;
            modList.GetItem(i).textColor = disabledModTextColor;
        }

        UpdateModPanel();
    }

    void ShowModDescriptionPopUp_OnMouseClick(BaseScreenComponent sender, Vector2 position)
    {
        if (modSettings == null || modSettings.Length < 1)
            return;
        else if (string.IsNullOrWhiteSpace(modSettings[currentSelection].modInfo.ModDescription))
            return;

        ModDescriptionMessageBox = new DaggerfallMessageBox(uiManager, this, true);
        ModDescriptionMessageBox.AllowCancel = true;
        ModDescriptionMessageBox.ClickAnywhereToClose = true;
        ModDescriptionMessageBox.ParentPanel.BackgroundTexture = null;

        Mod mod = ModManager.Instance.GetMod(modSettings[currentSelection].modInfo.ModTitle);
        string[] modDescription = (mod.TryLocalize("Mod", "Description") ?? mod.ModInfo.ModDescription).Split('\n');
        ModDescriptionMessageBox.SetText(modDescription);
        uiManager.PushWindow(ModDescriptionMessageBox);
    }

    void ModSettingsButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
    {
        Mod mod = ModManager.Instance.GetMod(modSettings[modList.SelectedIndex].modInfo.ModTitle);
        ModSettingsWindow modSettingsWindow = new ModSettingsWindow(DaggerfallUI.UIManager, mod);
        DaggerfallUI.UIManager.PushWindow(modSettingsWindow);
    }

    void ModEnabledCheckBox_OnToggleState()
    {
        if (modSettings == null || modSettings.Length < 1)
            return;

        ModSettings ms = modSettings[modList.SelectedIndex];

        if (ms.modInfo == null)
            return;

        modSettings[modList.SelectedIndex].enabled = modEnabledCheckBox.IsChecked;
        modList.SelectedValue.textColor = modEnabledCheckBox.IsChecked ? unselectedTextColor : disabledModTextColor;
        UpdateModPanel();
    }

    void ModList_OnScroll()
    {
        modListScrollBar.ScrollIndex = modList.ScrollIndex;
    }

    void ModListScrollBar_OnScroll()
    {
        modList.ScrollIndex = modListScrollBar.ScrollIndex;
    }

    #endregion
}
