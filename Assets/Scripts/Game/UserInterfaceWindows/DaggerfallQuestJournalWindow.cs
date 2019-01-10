// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Lypyl (lypyldf@gmail.com)
// Contributors:    Hazelnut
// 
// Notes:
//

//#define LAYOUT

using UnityEngine;
using System.Collections.Generic;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Questing;
using DaggerfallWorkshop.Game.Player;
using DaggerfallConnect;
using System.Linq;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    public class DaggerfallQuestJournalWindow : DaggerfallPopupWindow
    {
        #region Fields

        const string textDatabase = "DaggerfallUI";
        const string nativeImgName = "LGBK00I0.IMG";

        const int NULLINT = -1;
        public const int maxLinesQuests = 20;
        public const int maxLinesSmall = 28;
        const float textScaleSmall = 0.8f;

        int lastMessageIndex = NULLINT;
        int currentMessageIndex = 0;
        List<int> entryLineMap;
        int selectedEntry = NULLINT;

        List<Message> questMessages;
        int messageCount = 0;
        int findPlaceRegion;
        string findPlaceName;

        KeyCode toggleClosedBinding1;
        KeyCode toggleClosedBinding2;

        #endregion

        #region UI Controls

        TextLabel titleLabel;
        MultiFormatTextLabel questLogLabel;

        Panel mainPanel;

        Button dialogButton;
        Button upArrowButton;
        Button downArrowButton;
        Button exitButton;

        public JournalDisplay DisplayMode { get; set; }

        public enum JournalDisplay
        {
            ActiveQuests,
            FinshedQuests,
            Notebook
        }

        #endregion

        #region Constructors

        public DaggerfallQuestJournalWindow(UserInterfaceManager uiManager) : base(uiManager) 
        {
        }

        #endregion

        #region Setup Methods

        protected override void Setup()
        {
            base.Setup();

            Texture2D texture = DaggerfallUI.GetTextureFromImg(nativeImgName);
            if (texture == null)
            {
                Debug.LogError("failed to load texture: " + nativeImgName);
                CloseWindow();
            }
            if (defaultToolTip != null)
                defaultToolTip.ToolTipDelay = 1;

            mainPanel                       = DaggerfallUI.AddPanel(NativePanel, AutoSizeModes.None);
            mainPanel.BackgroundTexture     = texture;
            mainPanel.Size                  = new Vector2(320, 200);
            mainPanel.HorizontalAlignment   = HorizontalAlignment.Center;
            mainPanel.VerticalAlignment     = VerticalAlignment.Middle;
            mainPanel.OnMouseScrollDown     += MainPanel_OnMouseScrollDown;
            mainPanel.OnMouseScrollUp       += MainPanel_OnMouseScrollUp;

            dialogButton                    = new Button();
            dialogButton.Position           = new Vector2(32, 187);
            dialogButton.Size               = new Vector2(68, 10);
            dialogButton.OnMouseClick       += DialogButton_OnMouseClick;
            dialogButton.Name               = "dialog_button";
            dialogButton.ToolTip            = defaultToolTip;
            dialogButton.ToolTipText        = TextManager.Instance.GetText(textDatabase, "dialogButtonInfo");
            mainPanel.Components.Add(dialogButton);

            upArrowButton                   = new Button();
            upArrowButton.Position          = new Vector2(181, 188);
            upArrowButton.Size              = new Vector2(13, 7);
            upArrowButton.OnMouseClick      += UpArrowButton_OnMouseClick;
            upArrowButton.Name              = "uparrow_button";
            mainPanel.Components.Add(upArrowButton);

            downArrowButton                 = new Button();
            downArrowButton.Position        = new Vector2(209, 188);
            downArrowButton.Size            = new Vector2(13, 7);
            downArrowButton.OnMouseClick    += DownArrowButton_OnMouseClick;
            downArrowButton.Name            = "downarrow_button";
            mainPanel.Components.Add(downArrowButton);

            exitButton                      = new Button();
            exitButton.Position             = new Vector2(278, 187);
            exitButton.Size                 = new Vector2(30, 9);
            exitButton.OnMouseClick         += ExitButton_OnMouseClick;
            exitButton.Name                 = "exit_button";
            mainPanel.Components.Add(exitButton);

            questLogLabel                   = new MultiFormatTextLabel();
            questLogLabel.Position          = new Vector2(30, 38);
            questLogLabel.Size              = new Vector2(238, 138);
            questLogLabel.HighlightColor    = Color.white;
            questLogLabel.OnMouseClick      += QuestLogLabel_OnMouseClick;
            questLogLabel.OnRightMouseClick += QuestLogLabel_OnRightMouseClick;
            mainPanel.Components.Add(questLogLabel);

            Panel titlePanel = new Panel {
                Position = new Vector2(30, 22),
                Size = new Vector2(238, 16),
            };
            titleLabel = new TextLabel {
                HorizontalAlignment = HorizontalAlignment.Center,
                Font = DaggerfallUI.LargeFont,
                ShadowColor = new Color(0f, 0.2f, 0.5f),
                ToolTip = defaultToolTip,
                ToolTipText = TextManager.Instance.GetText(textDatabase, "activeQuestsInfo"),
            };
            titlePanel.Components.Add(titleLabel);
            mainPanel.Components.Add(titlePanel);
            titlePanel.OnMouseClick += TitlePanel_OnMouseClick;

            questMessages = QuestMachine.Instance.GetAllQuestLogMessages();

            // Store toggle closed bindings for this window
            toggleClosedBinding1 = InputManager.Instance.GetBinding(InputManager.Actions.LogBook);
            toggleClosedBinding2 = InputManager.Instance.GetBinding(InputManager.Actions.NoteBook);

#if LAYOUT
            SetBackgroundColors();
#endif
        }

        #endregion

        #region Overrides

        public override void OnPush()
        {
            base.OnPush();
            questMessages       = QuestMachine.Instance.GetAllQuestLogMessages();
            lastMessageIndex    = NULLINT;
            currentMessageIndex = 0;
            selectedEntry       = NULLINT;
        }

        public override void OnPop()
        {
            base.OnPop();
            questMessages = null;
            questLogLabel.Clear();
        }

        public override void Update()
        {
            base.Update();

            // Toggle window closed with same hotkey used to open it
            if (Input.GetKeyUp(toggleClosedBinding1) || Input.GetKeyUp(toggleClosedBinding2))
                CloseWindow();

            if (lastMessageIndex != currentMessageIndex)
            {
                lastMessageIndex = currentMessageIndex;
                questLogLabel.Clear();

                switch (DisplayMode)
                {
                    case JournalDisplay.ActiveQuests:
                        SetTextActiveQuests();
                        break;
                    case JournalDisplay.FinshedQuests:
                        SetTextFinshedQuests();
                        break;
                    case JournalDisplay.Notebook:
                        SetTextNotebook();
                        break;
                }
            }
        }

        #endregion

        #region events

        void DialogButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            switch (DisplayMode)
            {
                case JournalDisplay.ActiveQuests:
                    DisplayMode = JournalDisplay.FinshedQuests;
                    break;
                case JournalDisplay.FinshedQuests:
                    DisplayMode = JournalDisplay.Notebook;
                    break;
                case JournalDisplay.Notebook:
                    DisplayMode = JournalDisplay.ActiveQuests;
                    break;
            }
            lastMessageIndex = NULLINT;
            currentMessageIndex = 0;
            selectedEntry = NULLINT;
        }

        void UpArrowButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (currentMessageIndex - 1 >= 0)
                currentMessageIndex -= 1;
        }

        void DownArrowButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (currentMessageIndex + 1 < messageCount)
                currentMessageIndex += 1;
        }

        void MainPanel_OnMouseScrollUp(BaseScreenComponent sender)
        {
            UpArrowButton_OnMouseClick(sender, Vector2.zero);
        }

        void MainPanel_OnMouseScrollDown(BaseScreenComponent sender)
        {
            DownArrowButton_OnMouseClick(sender, Vector2.zero);
        }

        void TitlePanel_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            EnterNote(0);
        }

        public void ExitButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            CloseWindow();
        }

        void QuestLogLabel_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            HandleClick(position);
        }

        void QuestLogLabel_OnRightMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            HandleClick(position, true);
        }

        void RemoveEntry_OnButtonClick(DaggerfallMessageBox sender, DaggerfallMessageBox.MessageBoxButtons messageBoxButton)
        {
            if (messageBoxButton == DaggerfallMessageBox.MessageBoxButtons.Yes)
            {
                RemoveEntry(selectedEntry);
                if (currentMessageIndex == selectedEntry)
                    currentMessageIndex = 0;
                lastMessageIndex = NULLINT;
            }
            selectedEntry = NULLINT;
            sender.CloseWindow();
        }

        void MoveEntry_OnButtonClick(DaggerfallMessageBox sender, DaggerfallMessageBox.MessageBoxButtons messageBoxButton)
        {
            if (messageBoxButton != DaggerfallMessageBox.MessageBoxButtons.Yes)
                selectedEntry = NULLINT;
            sender.CloseWindow();
        }

        void FindPlace_OnButtonClick(DaggerfallMessageBox sender, DaggerfallMessageBox.MessageBoxButtons messageBoxButton)
        {
            sender.CloseWindow();
            if (messageBoxButton == DaggerfallMessageBox.MessageBoxButtons.Yes)
            {
                Debug.Log("Find " + findPlaceName + findPlaceRegion);
                this.CloseWindow();
                DaggerfallUI.Instance.DfTravelMapWindow.GotoLocation(findPlaceName, findPlaceRegion);
                DaggerfallUI.PostMessage(DaggerfallUIMessages.dfuiOpenTravelMapWindow);
            }
        }

        void EnterNote_OnGotUserInput(DaggerfallInputMessageBox sender, string enteredNoteLine)
        {
            if (!string.IsNullOrEmpty(enteredNoteLine))
            {
                GameManager.Instance.PlayerEntity.Notebook.AddNote(enteredNoteLine, selectedEntry);
                lastMessageIndex = NULLINT;
            }
            selectedEntry = NULLINT;
        }

        void EnterNote_OnCancel(DaggerfallPopupWindow sender)
        {
            selectedEntry = NULLINT;
        }

        #endregion

        #region Private Methods

        private void HandleClick(Vector2 position, bool remove = false)
        {
            if (entryLineMap == null)
                return;
            
            int moveSrcIdx = selectedEntry;    // Will be set if moving an entry
            int line = (int)(position.y / questLogLabel.LineHeight);

            if (line < entryLineMap.Count)
                selectedEntry = entryLineMap[line];
            else
                selectedEntry = entryLineMap[entryLineMap.Count - 1];
            Debug.LogFormat("line is: {0} entry: {1}", line, selectedEntry);

            if (DisplayMode == JournalDisplay.ActiveQuests)
            {
                // Handle current quest clicks - ask if want to travel to last location for quest.
                Message questMessage = questMessages[selectedEntry];
                Debug.Log(questMessage.ParentQuest.QuestName);
                Place place = questMessage.ParentQuest.LastPlaceReferenced;
                if (place != null &&
                    !string.IsNullOrEmpty(place.SiteDetails.locationName) &&
                    place.SiteDetails.locationName != GameManager.Instance.PlayerGPS.CurrentLocation.Name)
                {
                    findPlaceName = place.SiteDetails.locationName;
                    if (DaggerfallUI.Instance.DfTravelMapWindow.CanFindPlace(place.SiteDetails.regionName, findPlaceName))
                    {
                        findPlaceRegion = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetRegionIndex(place.SiteDetails.regionName);
                        string entryStr = string.Format("{0} in {1} province", findPlaceName, place.SiteDetails.regionName);
                        DaggerfallMessageBox dialogBox = CreateDialogBox(GetDialogText(entryStr, "selectedPlace", "confirmFind"));
                        dialogBox.OnButtonClick += FindPlace_OnButtonClick;
                        DaggerfallUI.UIManager.PushWindow(dialogBox);
                    }
                }
            }
            else
            {
                // Handle finished quest or notebook clicks - move, remove or add note.
                if (moveSrcIdx == NULLINT)
                {   // Process the click on or between entries
                    if (selectedEntry < 0)
                    {   // Add a note between entries
                        EnterNote(selectedEntry);
                    }
                    else
                    {   // Move or remove when click on entry
                        TextFile.Token[] entry = GetEntry(selectedEntry);
                        if (entry != null && entry.Length > 0)
                        {
                            DaggerfallMessageBox dialogBox = CreateDialogBox(GetDialogText(entry[0].text, "selectedEntry", remove ? "confirmRemove" : "confirmMove"));
                            if (remove)
                                dialogBox.OnButtonClick += RemoveEntry_OnButtonClick;
                            else
                                dialogBox.OnButtonClick += MoveEntry_OnButtonClick;
                            DaggerfallUI.UIManager.PushWindow(dialogBox);
                        }
                    }
                }
                else
                {   // Move the selected entry to this position
                    if (selectedEntry < 0)
                        selectedEntry = -selectedEntry + currentMessageIndex;
                    MoveEntry(moveSrcIdx, selectedEntry);
                    lastMessageIndex = NULLINT;
                    selectedEntry = NULLINT;
                }
            }
        }

        private static string[] GetDialogText(string entryStr, string preKey, string postKey)
        {
            return new string[] {
                TextManager.Instance.GetText(textDatabase, preKey),
                "", "    " + entryStr, "",
                TextManager.Instance.GetText(textDatabase, postKey), "",
                TextManager.Instance.GetText(textDatabase, postKey + "2"),
            };
        }

        private DaggerfallMessageBox CreateDialogBox(string[] dialogText)
        {
            DaggerfallMessageBox dialogBox = new DaggerfallMessageBox(uiManager, this);
            dialogBox.SetText(dialogText);
            dialogBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.Yes);
            dialogBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.No);
            return dialogBox;
        }

        private void EnterNote(int index)
        {
            if (DisplayMode == JournalDisplay.Notebook)
            {
                selectedEntry = -index + currentMessageIndex;
                Debug.Log("Add at " + selectedEntry);

                TextFile.Token prompt = new TextFile.Token() {
                    text = TextManager.Instance.GetText(textDatabase, "enterNote"),
                    formatting = TextFile.Formatting.Text,
                };
                DaggerfallInputMessageBox enterNote =
                    new DaggerfallInputMessageBox(uiManager, new TextFile.Token[] { prompt }, PlayerNotebook.MaxLineLenth, "", true, this);
                //enterNote.TextPanelDistanceY = 5;
                enterNote.TextBox.WidthOverride = 318;
                enterNote.OnGotUserInput += EnterNote_OnGotUserInput;
                enterNote.OnCancel += EnterNote_OnCancel;
                enterNote.Show();
            }
        }

        private TextFile.Token[] GetEntry(int index)
        {
            switch (DisplayMode)
            {
                case JournalDisplay.FinshedQuests:
                    return GameManager.Instance.PlayerEntity.Notebook.GetFinishedQuest(index);
                case JournalDisplay.Notebook:
                    return GameManager.Instance.PlayerEntity.Notebook.GetNote(index);
            }
            return null;
        }

        private void MoveEntry(int srcIdx, int destIdx)
        {
            switch (DisplayMode)
            {
                case JournalDisplay.FinshedQuests:
                    GameManager.Instance.PlayerEntity.Notebook.MoveFinishedQuest(srcIdx, destIdx);
                    break;
                case JournalDisplay.Notebook:
                    GameManager.Instance.PlayerEntity.Notebook.MoveNote(srcIdx, destIdx);
                    break;
            }
        }

        private void RemoveEntry(int index)
        {
            switch (DisplayMode)
            {
                case JournalDisplay.FinshedQuests:
                    GameManager.Instance.PlayerEntity.Notebook.RemoveFinishedQuest(index);
                    break;
                case JournalDisplay.Notebook:
                    GameManager.Instance.PlayerEntity.Notebook.RemoveNote(index);
                    break;
            }
        }

        private void SetTextActiveQuests()
        {
            messageCount = questMessages.Count;
            questLogLabel.TextScale = 1;
            titleLabel.Text = TextManager.Instance.GetText(textDatabase, "activeQuests");
            titleLabel.ToolTipText = TextManager.Instance.GetText(textDatabase, "activeQuestsInfo");

            int totalLineCount = 0;
            entryLineMap = new List<int>(maxLinesQuests);;
            List<TextFile.Token> textTokens = new List<TextFile.Token>();

            for (int i = currentMessageIndex; i < questMessages.Count; i++)
            {
                if (totalLineCount >= maxLinesQuests)
                    break;

                var tokens = questMessages[i].GetTextTokens();

                for (int j = 0; j < tokens.Length; j++)
                {
                    if (totalLineCount >= maxLinesQuests)
                        break;

                    var token = tokens[j];

                    if (token.formatting == TextFile.Formatting.Text || token.formatting == TextFile.Formatting.NewLine)
                    {
                        totalLineCount++;
                        entryLineMap.Add(i);
                    }
                    else
                        token.formatting = TextFile.Formatting.JustifyLeft;

                    textTokens.Add(token);
                }
                textTokens.Add(TextFile.NewLineToken);
                totalLineCount++;
                entryLineMap.Add(i);
            }

            questLogLabel.SetText(textTokens.ToArray());
        }

        private void SetTextFinshedQuests()
        {
            List<TextFile.Token[]> finishedQuests = GameManager.Instance.PlayerEntity.Notebook.GetFinishedQuests();
            messageCount = finishedQuests.Count;
            questLogLabel.TextScale = 1;
            titleLabel.Text = TextManager.Instance.GetText(textDatabase, "finishedQuests");
            titleLabel.ToolTipText = TextManager.Instance.GetText(textDatabase, "finishedQuestsInfo");
            SetTextWithListEntries(finishedQuests, maxLinesQuests);
        }

        private void SetTextNotebook()
        {
            List<TextFile.Token[]> notes = GameManager.Instance.PlayerEntity.Notebook.GetNotes();
            messageCount = notes.Count;
            questLogLabel.TextScale = textScaleSmall;
            titleLabel.Text = TextManager.Instance.GetText(textDatabase, "notebook");
            titleLabel.ToolTipText = TextManager.Instance.GetText(textDatabase, "notebookInfo");
            SetTextWithListEntries(notes, maxLinesSmall);
        }

        private void SetTextWithListEntries(List<TextFile.Token[]> entries, int maxLines)
        {
            int totalLineCount = 0;
            int boundary = 0;
            entryLineMap = new List<int>(maxLines);
            List<TextFile.Token> textTokens = new List<TextFile.Token>();

            for (int i = currentMessageIndex; i < entries.Count; i++)
            {
                if (totalLineCount >= maxLines)
                    break;

                TextFile.Token[] tokens = entries[i];
                for (int j = 0; j < tokens.Length; j++)
                {
                    if (totalLineCount >= maxLines)
                        break;

                    var token = tokens[j];
                    if (token.formatting == TextFile.Formatting.Text ||
                        token.formatting == TextFile.Formatting.TextHighlight ||
                        token.formatting == TextFile.Formatting.TextAnswer ||
                        token.formatting == TextFile.Formatting.TextQuestion ||
                        token.formatting == TextFile.Formatting.NewLine)
                    {
                        totalLineCount++;
                        entryLineMap.Add(i);
                    }
                    else
                        token.formatting = TextFile.Formatting.JustifyLeft;

                    textTokens.Add(token);
                }
                textTokens.Add(TextFile.NewLineToken);
                totalLineCount++;
                entryLineMap.Add(--boundary);
            }

            questLogLabel.SetText(textTokens.ToArray());
            questLogLabel.Size = new Vector2(questLogLabel.Size.x, 138);
        }


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
        #endregion
    }
}
