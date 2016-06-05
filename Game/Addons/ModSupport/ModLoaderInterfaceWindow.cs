using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Game.Utility.ModSupport;


/// <summary>
/// 1. Need to reposition modList box
/// 2. Need to setup scrollbar for list box
/// 3. Mod description
/// </summary>
public class ModLoaderInterfaceWindow : DaggerfallPopupWindow
{

    struct ModSettings
    {
        public ModInfo modInfo;
        public bool enabled;
    }

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

    Checkbox modEnabledCheckBox         = new Checkbox();
    TextLabel modLoadPriorityLabel      = new TextLabel();
    TextLabel modTitleLabel             = new TextLabel();
    TextLabel modVersionLabel           = new TextLabel();
    TextLabel modAuthorLabel            = new TextLabel();
    TextLabel modAuthorContactLabel     = new TextLabel();
    TextLabel modDFTFUVersionLabel      = new TextLabel();
    MultiFormatTextLabel modDescription = new MultiFormatTextLabel();

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

        modList.BackgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.5f);
        modList.Size = new Vector2(120, 115);
        modList.HorizontalAlignment = HorizontalAlignment.Center;
        modList.Position = new Vector2(60, 60);

        modList.TextColor = unselectedTextColor;
        modList.SelectedTextColor = textColor;//selectedTextColor;
        modList.ShadowPosition = Vector2.zero;
        modList.RowsDisplayed = 8;
        modList.RowAlignment = HorizontalAlignment.Center;
        modList.SelectedShadowPosition = DaggerfallUI.DaggerfallDefaultShadowPos;
        modList.SelectedShadowColor = Color.black;
        ModListPanel.Components.Add(modList);

        modListScrollBar.Size = new Vector2(5, 62);
        modList.VerticalAlignment = VerticalAlignment.Middle;
        modList.HorizontalAlignment = HorizontalAlignment.Left;
        //modListScrollBar.VerticalAlignment = VerticalAlignment.Middle;
        // modListScrollBar.HorizontalAlignment = HorizontalAlignment.Right;
        modListScrollBar.Position = new Vector2(100, 12);
        modListScrollBar.BackgroundColor = Color.cyan;
        //ModListPanel.Components.Add(modListScrollBar);

        backButton.Size = new Vector2(45, 12);
        backButton.Label.Text = "< Options";
        backButton.Label.ShadowPosition = Vector2.zero;
        backButton.Label.TextColor = Color.gray;
        backButton.ToolTip = defaultToolTip;
        backButton.ToolTipText = "Back to options without saving changes";
        backButton.VerticalAlignment = VerticalAlignment.Top;
        backButton.HorizontalAlignment = HorizontalAlignment.Left;
        backButton.OnMouseClick +=  BackButton_OnMouseClick;
        ModListPanel.Components.Add(backButton);

        increaseLoadOrderButton.Size = new Vector2(40, 12);
        increaseLoadOrderButton.Position = new Vector2(62, 150);
        increaseLoadOrderButton.Outline.Enabled = true;
        increaseLoadOrderButton.BackgroundColor = textColor;
        increaseLoadOrderButton.Label.Text = "Increase";
        increaseLoadOrderButton.OnMouseClick += IncreaseLoadOrderButton_OnMouseClick;
        ModListPanel.Components.Add(increaseLoadOrderButton);

        decreaseLoadOrderButton.Size = new Vector2(40, 12);
        decreaseLoadOrderButton.Position = new Vector2(21, 150);
        decreaseLoadOrderButton.Outline.Enabled = true;
        decreaseLoadOrderButton.BackgroundColor = textColor;
        decreaseLoadOrderButton.Label.Text = "Lower";
        decreaseLoadOrderButton.OnMouseClick += DecreaseLoadOrderButton_OnMouseClick;
        ModListPanel.Components.Add(decreaseLoadOrderButton);

        enableAllButton.Size = new Vector2(40, 12);
        enableAllButton.Position = new Vector2(21, 163);
        enableAllButton.Outline.Enabled = true;
        enableAllButton.BackgroundColor = textColor;
        enableAllButton.VerticalAlignment = VerticalAlignment.Bottom;
        enableAllButton.Label.Text = "ALL ON";
        enableAllButton.ToolTipText = "Enable All Mods";
        enableAllButton.OnMouseClick += EnableAllButton_OnMouseClick;
        ModListPanel.Components.Add(enableAllButton);

        disableAllButton.Size = new Vector2(40, 12);
        disableAllButton.Position = new Vector2(62, 163);
        disableAllButton.Outline.Enabled = true;
        disableAllButton.BackgroundColor = textColor;
        disableAllButton.VerticalAlignment = VerticalAlignment.Bottom;
        disableAllButton.Label.Text = "ALL OFF";
        disableAllButton.ToolTipText = "Disable All Mods";
        disableAllButton.OnMouseClick += DisableAllButton_OnMouseClick;
        ModListPanel.Components.Add(disableAllButton);

        //Add main mod panel
        ModPanel.Outline.Enabled = true;
        ModPanel.BackgroundColor = backgroundColor;
        ModPanel.HorizontalAlignment = HorizontalAlignment.Right;
        ModPanel.VerticalAlignment = VerticalAlignment.Middle;
        ModPanel.Size = new Vector2(200, 175);
        NativePanel.Components.Add(ModPanel);

        modEnabledCheckBox.Label.Text = "Enabled";
        modEnabledCheckBox.Label.TextColor = selectedTextColor;
        modEnabledCheckBox.CheckBoxColor = selectedTextColor;
        modEnabledCheckBox.ToolTip = defaultToolTip;
        modEnabledCheckBox.ToolTipText = "Toggle Mod";
        modEnabledCheckBox.IsChecked = true;
        modEnabledCheckBox.Position = new Vector2(1, 5);
        modEnabledCheckBox.OnToggleState += modEnabledCheckBox_OnToggleState;
        ModPanel.Components.Add(modEnabledCheckBox);

        modLoadPriorityLabel.Position = new Vector2(60, 5);
        ModPanel.Components.Add(modLoadPriorityLabel);

        modTitleLabel.Position = new Vector2(5, 20);
        ModPanel.Components.Add(modTitleLabel);

        modVersionLabel.Position = new Vector2(5, 30);
        ModPanel.Components.Add(modVersionLabel);

        modAuthorLabel.Position = new Vector2(5, 40);
        ModPanel.Components.Add(modAuthorLabel);

        modAuthorContactLabel = new TextLabel();
        modAuthorContactLabel.Position = new Vector2(5, 50);
        ModPanel.Components.Add(modAuthorContactLabel);

        modDFTFUVersionLabel.Position = new Vector2(5, 60);
        ModPanel.Components.Add(modDFTFUVersionLabel);

        modDescription.Position = new Vector2(5, 70);
        modDescription.AddTextLabel("Mod Description:");
        modDescription.NewLine();
        ModPanel.Components.Add(modDescription);

        refreshButton.Size = new Vector2(50, 12);
        refreshButton.Outline.Enabled = true;
        refreshButton.BackgroundColor = textColor;
        refreshButton.VerticalAlignment = VerticalAlignment.Bottom;
        refreshButton.HorizontalAlignment = HorizontalAlignment.Left;
        refreshButton.Label.Text = "Refresh";
        refreshButton.Label.ToolTipText = "Check for changes in mod directory";
        refreshButton.OnMouseClick += RefreshButton_OnMouseClick;
        ModPanel.Components.Add(refreshButton);

        saveAndCloseButton.Size = new Vector2(70, 12);
        saveAndCloseButton.Outline.Enabled = true;
        saveAndCloseButton.BackgroundColor = textColor;
        saveAndCloseButton.VerticalAlignment = VerticalAlignment.Bottom;
        saveAndCloseButton.HorizontalAlignment = HorizontalAlignment.Center;
        saveAndCloseButton.Label.Text = "Save and Close";
        saveAndCloseButton.Label.ToolTipText = "Save changes and return to options menu";
        saveAndCloseButton.OnMouseClick += SaveAndCloseButton_OnMouseClick;
        ModPanel.Components.Add(saveAndCloseButton);

        extractFilesButton.Size = new Vector2(60, 12);
        extractFilesButton.Outline.Enabled = true;
        extractFilesButton.BackgroundColor = textColor;
        extractFilesButton.VerticalAlignment = VerticalAlignment.Bottom;
        extractFilesButton.HorizontalAlignment = HorizontalAlignment.Right;
        extractFilesButton.Label.Text = "Extract Text";
        extractFilesButton.Label.ToolTipText = "Extract Text Assets";
        extractFilesButton.OnMouseClick += ExtractFilesButton_OnMouseClick;
        ModPanel.Components.Add(extractFilesButton);

        GetLoadedMods();
    }

    public override void Update()
    {
        base.Update();

        if(currentSelection != modList.SelectedIndex && modList.Count > 0)
        {
                currentSelection = modList.SelectedIndex;
                UpdateModPanel();
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
        var mods = ModManager.Instance.GetAllMods(true).ToArray();

        modList.ClearItems();

        if(modSettings == null || modSettings.Length != mods.Count())
        {
            modSettings = new ModSettings[mods.Count()];
        }

        for (int i = 0; i < mods.Count(); i++)
        {
            ModSettings modsett = new ModSettings();
            modsett.modInfo = mods[i].ModInfo;
            modsett.enabled = mods[i].Enabled;
            modSettings[i] = modsett;
            modList.AddItem(modsett.modInfo.ModName);
        }
    }

    void UpdateModPanel()
    {
        if (modSettings.Length < 1 || currentSelection < 0)
            return;

        ModSettings ms = modSettings[modList.SelectedIndex];

        if (ms.modInfo == null)
            return;

        modEnabledCheckBox.IsChecked = ms.enabled;
        modLoadPriorityLabel.Text = "Load Priority: " + modList.SelectedIndex;
        modTitleLabel.Text = "Mod Title: " + ms.modInfo.ModTitle;
        modVersionLabel.Text = "Version: " + ms.modInfo.ModVersion;
        modAuthorLabel.Text = "Author: " + ms.modInfo.ModAuthor;
        modAuthorContactLabel.Text = "Contact: " + ms.modInfo.ContactInfo;
        modDFTFUVersionLabel.Text = "DFUnity Version: " + ms.modInfo.DFUnity_Verion;

        if (ms.enabled)
            modList.SelectedTextColor = selectedTextColor;
        else
            modList.SelectedTextColor = Color.red;

        modDescription.Clear();
        modDescription.AddTextLabel("Mod Description: ");
        modDescription.NewLine();
        modDescription.NewLine();
    }

    #region Events


    void DecreaseLoadOrderButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
    {
        if (modList.Count <= 2)
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
        if (modList.Count <= 2)
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
        GetLoadedMods();
        UpdateModPanel();
    }

    void SaveAndCloseButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
    {
        if(modSettings == null)
        {
            Debug.LogWarning("modsettings array was null");
            return;
        }

        Mod mod;
        for (int i = 0; i < modSettings.Length; i++)
        {
            if (!ModManager.Instance.GetMod(modSettings[i].modInfo.ModTitle, out mod))
            {
                Debug.Log("failed to get mod with key: " + modSettings[i].modInfo.ModTitle);
                continue;
            }

            //for (int j = 0; j < modList.Count; j++)
            //{
            //    string label = modList.GetItemText(j);
            //    if (string.Equals(label, mod.Name))
            //    {
            //        mod.LoadPriority = j;
            //        break;
            //    }
            //}
            mod.LoadPriority = i;
            mod.Enabled = modSettings[i].enabled;
        }
        modList.ClearItems();
        modSettings = null;
        DaggerfallUI.UIManager.PopWindow();
    }

    void ExtractFilesButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
    {
        if (modSettings.Length < 1)
            return;

        Mod mod;

        if(!ModManager.Instance.GetMod(modSettings[modList.SelectedIndex].modInfo.ModTitle, out mod))
        {
            return;
        }

        string[] assets = mod.AssetBundle.GetAllAssetNames();

        string path = System.IO.Path.Combine(mod.DirPath, mod.Name + "_ExtractedFiles");

        if (!System.IO.Directory.Exists(path))
        {
            System.IO.Directory.CreateDirectory(path);
        }

        for (int i = 0; i < assets.Length; i++)
        {
            UnityEngine.Object asset = mod.AssetBundle.LoadAsset(assets[i]);
            if (asset == null)
                continue;
            else if(asset.GetType() == typeof(UnityEngine.TextAsset))
            {
                System.IO.File.WriteAllText(System.IO.Path.Combine(path, asset.name+".txt"), ((TextAsset)asset).ToString()); //append .txt at end of asset name so mod info file will never end in .dfmod
            }                                                                                                                //which would cause it to be tried to load by mod manager as an asset bundle if in mod directory
            Debug.Log(string.Format("asset type for asset : {0} {1}", asset.name, asset.GetType().Name));
        }

        RefreshButton_OnMouseClick(null, Vector2.zero);
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

    void modEnabledCheckBox_OnToggleState()
    {
        if (modList.SelectedIndex < 0 || modList.SelectedIndex > modSettings.Length)
            return;

        ModSettings ms = modSettings[modList.SelectedIndex];

        if (ms.modInfo == null)
            return;

        modSettings[modList.SelectedIndex].enabled = modEnabledCheckBox.IsChecked;
        UpdateModPanel();
    }

    #endregion

}
