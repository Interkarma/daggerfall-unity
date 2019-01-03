using UnityEngine;
using System.Linq;
using System.IO;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using DaggerfallWorkshop.Game.Utility.ModSupport.ModSettings;

public class ModLoaderInterfaceWindow : DaggerfallPopupWindow
{

    struct ModSettings
    {
        public ModInfo modInfo;
        public bool enabled;
    }

    DaggerfallMessageBox ModDescriptionMessageBox;

    Panel ModPanel = new Panel();
    Panel ModListPanel = new Panel();

    ListBox modList = new ListBox();
    VerticalScrollBar modListScrollBar = new VerticalScrollBar();

    Button increaseLoadOrderButton  = new Button();
    Button decreaseLoadOrderButton  = new Button();
    Button backButton               = new Button();
    Button refreshButton            = new Button();
    Button enableAllButton          = new Button();
    Button disableAllButton         = new Button();
    Button saveAndCloseButton       = new Button();
    Button extractFilesButton       = new Button();
    Button showModDescriptionButton = new Button();
    Button modSettingsButton        = new Button();

    Checkbox modEnabledCheckBox         = new Checkbox();
    TextLabel modLoadPriorityLabel      = new TextLabel();
    TextLabel modTitleLabel             = new TextLabel();
    TextLabel modVersionLabel           = new TextLabel();
    TextLabel modAuthorLabel            = new TextLabel();
    TextLabel modAuthorContactLabel     = new TextLabel();
    TextLabel modDFTFUVersionLabel      = new TextLabel();
    TextLabel modsFound                 = new TextLabel();

    Color backgroundColor = new Color(0, 0, 0, 0.7f);
    Color unselectedTextColor = new Color(0.6f, 0.6f, 0.6f, 1f);
    Color selectedTextColor = new Color(0.0f, 0.8f, 0.0f, 1.0f);
    Color textColor = new Color(0.0f, 0.5f, 0.0f, 0.4f);

    int currentSelection = -1;

    ModSettings[] modSettings;

    public ModLoaderInterfaceWindow(IUserInterfaceManager uiManager)
    : base(uiManager)
    {
    }

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
        modList.MaxCharacters = 20;
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
        modEnabledCheckBox.OnToggleState += modEnabledCheckBox_OnToggleState;
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

        modAuthorContactLabel = new TextLabel();
        modAuthorContactLabel.Position = new Vector2(5, 60);
        modAuthorContactLabel.MaxCharacters = 40;
        ModPanel.Components.Add(modAuthorContactLabel);

        modDFTFUVersionLabel.Position = new Vector2(5, 70);
        modDFTFUVersionLabel.MaxCharacters = 40;
        ModPanel.Components.Add(modDFTFUVersionLabel);

        showModDescriptionButton = new Button();
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
        ModPanel.Components.Add(refreshButton);

        saveAndCloseButton.Size = new Vector2(70, 12);
        saveAndCloseButton.Outline.Enabled = true;
        saveAndCloseButton.BackgroundColor = textColor;
        saveAndCloseButton.VerticalAlignment = VerticalAlignment.Bottom;
        saveAndCloseButton.HorizontalAlignment = HorizontalAlignment.Center;
        saveAndCloseButton.Label.Text = ModManager.GetText("saveClose");
        saveAndCloseButton.Label.ToolTipText = ModManager.GetText("saveCloseInfo");
        saveAndCloseButton.OnMouseClick += SaveAndCloseButton_OnMouseClick;
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
        var mods = ModManager.Instance.GetAllMods(true);

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
            modList.AddItem(modsett.modInfo.ModTitle);
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

        if (ms.enabled)
            modList.SelectedTextColor = selectedTextColor;
        else
            modList.SelectedTextColor = Color.red;

        // Update buttons
        if (ModManager.Instance.GetMod(ms.modInfo.ModTitle).HasSettings)
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
        if(modSettings == null)
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
        //save current mod settings to file
        ModManager.WriteModSettings();
        DaggerfallUI.UIManager.PopWindow();
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
        string path = System.IO.Path.Combine(mod.DirPath, mod.Title + "_ExtractedFiles");

        if (!System.IO.Directory.Exists(path))
        {
            System.IO.Directory.CreateDirectory(path);
        }

        for (int i = 0; i < assets.Length; i++)
        {
            string extension = assets[i].Substring(assets[i].LastIndexOf('.'));

            if (!ModManager.textExtensions.Contains(extension))
                continue;

            TextAsset asset = mod.GetAsset<TextAsset>(assets[i]);

            if (asset == null)
                continue;
            System.IO.File.WriteAllText(System.IO.Path.Combine(path, asset.name + ".txt"), asset.ToString()); //append .txt at end of asset name so mod info file will never end in .dfmod
                                                                                                               //which would cause it to be tried to load by mod manager as an asset bundle if in mod directory
            Debug.Log(string.Format("asset type for asset : {0} {1}", asset.name, asset.GetType().Name));
        }
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
        }

        UpdateModPanel();
    }

    void ShowModDescriptionPopUp_OnMouseClick(BaseScreenComponent sender, Vector2 position)
    {
        if (modSettings == null || modSettings.Length < 1)
            return;
        else if (string.IsNullOrEmpty(modSettings[currentSelection].modInfo.ModDescription))
            return;

        ModDescriptionMessageBox = new DaggerfallMessageBox(uiManager, this);
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

    void modEnabledCheckBox_OnToggleState()
    {
        if (modSettings == null || modSettings.Length < 1)
            return;

        ModSettings ms = modSettings[modList.SelectedIndex];

        if (ms.modInfo == null)
            return;

        modSettings[modList.SelectedIndex].enabled = modEnabledCheckBox.IsChecked;
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

