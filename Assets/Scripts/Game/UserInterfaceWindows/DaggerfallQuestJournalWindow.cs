// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
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
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    public class DaggerfallQuestJournalWindow : DaggerfallPopupWindow
    {
        #region Fields

        const string nativeImgName = "LGBK00I0.IMG";

        const SoundClips openJournal = SoundClips.OpenBook;
        const SoundClips editNotebook = SoundClips.PageTurn; // same as spellbook edit sounds

        const int NULLINT = -1;
        public const int maxLinesQuests = 20;
        public const int maxLinesSmall = 28;
        const float textScaleSmall = 0.8f;

        protected int lastMessageIndex = NULLINT;
        protected int currentMessageIndex = 0;
        protected List<int> entryLineMap;
        protected int selectedEntry = NULLINT;

        protected List<Message> questMessages;
        protected int messageCount = 0;
        protected Place findPlace;

        KeyCode toggleClosedBinding1;
        KeyCode toggleClosedBinding2;

        #endregion

        #region UI Controls

        protected TextLabel titleLabel;
        protected MultiFormatTextLabel questLogLabel;

        Panel mainPanel;

        Button dialogButton;
        Button upArrowButton;
        Button downArrowButton;
        Button exitButton;

        protected bool isCloseWindowDeferred = false;

        public JournalDisplay DisplayMode { get; set; }

        public enum JournalDisplay
        {
            ActiveQuests,
            FinshedQuests,
            Notebook,
            Messages
        }

        #endregion

        #region Constructors

        public DaggerfallQuestJournalWindow(IUserInterfaceManager uiManager) : base(uiManager)
        {
            // Prevent duplicate close calls with base class's exitKey (Escape)
            AllowCancel = false;
        }

        #endregion

        #region Setup Methods

        protected override void Setup()
        {
            base.Setup();

            // Always dim background
            ParentPanel.BackgroundColor = ScreenDimColor;

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
            dialogButton.Hotkey             = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.JournalNextCategory);
            dialogButton.Name               = "dialog_button";
            dialogButton.ToolTip            = defaultToolTip;
            dialogButton.ToolTipText        = TextManager.Instance.GetLocalizedText("dialogButtonInfo");
            mainPanel.Components.Add(dialogButton);

            upArrowButton                   = new Button();
            upArrowButton.Position          = new Vector2(181, 188);
            upArrowButton.Size              = new Vector2(13, 7);
            upArrowButton.OnMouseClick      += UpArrowButton_OnMouseClick;
            upArrowButton.Hotkey            = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.JournalPreviousPage);
            upArrowButton.Name              = "uparrow_button";
            mainPanel.Components.Add(upArrowButton);

            downArrowButton                 = new Button();
            downArrowButton.Position        = new Vector2(209, 188);
            downArrowButton.Size            = new Vector2(13, 7);
            downArrowButton.OnMouseClick    += DownArrowButton_OnMouseClick;
            downArrowButton.Hotkey          = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.JournalNextPage);
            downArrowButton.Name            = "downarrow_button";
            mainPanel.Components.Add(downArrowButton);

            exitButton                      = new Button();
            exitButton.Position             = new Vector2(278, 187);
            exitButton.Size                 = new Vector2(30, 9);
            exitButton.OnMouseClick         += ExitButton_OnMouseClick;
            exitButton.Hotkey               = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.JournalExit);
            exitButton.OnKeyboardEvent      += ExitButton_OnKeyboardEvent;
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
                ToolTipText = TextManager.Instance.GetLocalizedText("activeQuestsInfo"),
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

            toggleClosedBinding1 = InputManager.Instance.GetBinding(InputManager.Actions.LogBook);
            toggleClosedBinding2 = InputManager.Instance.GetBinding(InputManager.Actions.NoteBook);

            questMessages       = QuestMachine.Instance.GetAllQuestLogMessages();
            lastMessageIndex    = NULLINT;
            currentMessageIndex = 0;
            selectedEntry       = NULLINT;
            DaggerfallUI.Instance.PlayOneShot(openJournal);
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

            if (DaggerfallUI.Instance.HotkeySequenceProcessed == HotkeySequence.HotkeySequenceProcessStatus.NotFound)
            {
                // Toggle window closed with same hotkey used to open it
                if (InputManager.Instance.GetKeyUp(toggleClosedBinding1)
                    || InputManager.Instance.GetKeyUp(toggleClosedBinding2)
                    || InputManager.Instance.GetBackButtonUp())
                    CloseWindow();
            }

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
                    case JournalDisplay.Messages:
                        SetTextMessages();
                        break;
                }
            }
        }

        #endregion

        #region events

        protected virtual void DialogButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
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
                    DisplayMode = JournalDisplay.Messages;
                    break;
                case JournalDisplay.Messages:
                    DisplayMode = JournalDisplay.ActiveQuests;
                    break;
            }
            lastMessageIndex = NULLINT;
            currentMessageIndex = 0;
            selectedEntry = NULLINT;
            DaggerfallUI.Instance.PlayOneShot(openJournal);
        }

        protected virtual void UpArrowButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.PageTurn);
            if (currentMessageIndex - 1 >= 0)
                currentMessageIndex -= 1;
        }

        protected virtual void DownArrowButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.PageTurn);
            if (currentMessageIndex + 1 < messageCount)
                currentMessageIndex += 1;
        }

        protected virtual void MainPanel_OnMouseScrollUp(BaseScreenComponent sender)
        {
            if (currentMessageIndex - 1 >= 0)
                currentMessageIndex -= 1;
        }

        protected virtual void MainPanel_OnMouseScrollDown(BaseScreenComponent sender)
        {
            if (currentMessageIndex + 1 < messageCount)
                currentMessageIndex += 1;
        }

        protected virtual void TitlePanel_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            EnterNote(0);
        }

        public virtual void ExitButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            CloseWindow();
        }

        protected virtual void ExitButton_OnKeyboardEvent(BaseScreenComponent sender, Event keyboardEvent)
        {
            if (keyboardEvent.type == EventType.KeyDown)
            {
                DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
                isCloseWindowDeferred = true;
            }
            else if (keyboardEvent.type == EventType.KeyUp && isCloseWindowDeferred)
            {
                isCloseWindowDeferred = false;
                CloseWindow();
            }
        }

        protected virtual void QuestLogLabel_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            HandleClick(position);
        }

        protected virtual void QuestLogLabel_OnRightMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            HandleClick(position, true);
        }

        protected virtual void RemoveEntry_OnButtonClick(DaggerfallMessageBox sender, DaggerfallMessageBox.MessageBoxButtons messageBoxButton)
        {
            if (messageBoxButton == DaggerfallMessageBox.MessageBoxButtons.Yes)
            {
                RemoveEntry(selectedEntry);
                if (currentMessageIndex == selectedEntry)
                    currentMessageIndex = 0;
                lastMessageIndex = NULLINT;
                DaggerfallUI.Instance.PlayOneShot(editNotebook);
            }
            selectedEntry = NULLINT;
            sender.CloseWindow();
        }

        protected virtual void MoveEntry_OnButtonClick(DaggerfallMessageBox sender, DaggerfallMessageBox.MessageBoxButtons messageBoxButton)
        {
            if (messageBoxButton != DaggerfallMessageBox.MessageBoxButtons.Yes)
                selectedEntry = NULLINT;
            sender.CloseWindow();
        }

        protected virtual void FindPlace_OnButtonClick(DaggerfallMessageBox sender, DaggerfallMessageBox.MessageBoxButtons messageBoxButton)
        {
            sender.CloseWindow();
            if (messageBoxButton == DaggerfallMessageBox.MessageBoxButtons.Yes && findPlace != null)
            {
                Debug.LogFormat("Finding location {0} in region {1}", findPlace.SiteDetails.locationName, findPlace.SiteDetails.regionName);
                this.CloseWindow();
                DaggerfallUI.Instance.DfTravelMapWindow.GotoPlace(findPlace);
                DaggerfallUI.PostMessage(DaggerfallUIMessages.dfuiOpenTravelMapWindow);
            }
        }

        protected virtual void EnterNote_OnGotUserInput(DaggerfallInputMessageBox sender, string enteredNoteLine)
        {
            if (!string.IsNullOrEmpty(enteredNoteLine))
            {
                GameManager.Instance.PlayerEntity.Notebook.AddNote(enteredNoteLine, selectedEntry);
                lastMessageIndex = NULLINT;
                DaggerfallUI.Instance.PlayOneShot(editNotebook);
            }
            selectedEntry = NULLINT;
        }

        protected virtual void EnterNote_OnCancel(DaggerfallPopupWindow sender)
        {
            selectedEntry = NULLINT;
        }

        #endregion

        #region Protected Methods

        protected virtual void HandleClick(Vector2 position, bool remove = false)
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
                HandleQuestClicks(questMessages[selectedEntry]);
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
                            DaggerfallMessageBox dialogBox = CreateDialogBox(entry[0].text, remove ? "confirmRemove" : "confirmMove");
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
                    DaggerfallUI.Instance.PlayOneShot(editNotebook);
                }
            }
        }

        // Handle current quest clicks - ask if want to travel to last location for quest.
        protected virtual void HandleQuestClicks(Message questMessage)
        {
            // Get last actual Place resource mentioned in quest message
            // If no Place resouce is mentioned specifically then Place will be null
            // An example of where this happens is the DB initiation quest where entry is kept secret from journal
            // When using ParentQuest.LastPlaceReferenced this can result in player being sent to an unrelated home location for last NPC processed
            Place place = GetLastPlaceMentionedInMessage(questMessage);

            Debug.Log(questMessage.ParentQuest.QuestName);
            if (place != null &&
                !string.IsNullOrEmpty(place.SiteDetails.locationName) &&
                place.SiteDetails.locationName != GameManager.Instance.PlayerGPS.CurrentLocation.Name)
            {
                string findPlaceName = place.SiteDetails.locationName;
                if (DaggerfallUI.Instance.DfTravelMapWindow.CanFindPlace(place.SiteDetails.regionName, findPlaceName)) // Check using canonical name
                {
                    // Workaround for quests compiled before DFU 0.15.0 or later
                    int regionIndex = MapsFile.PatchRegionIndex(place.SiteDetails.regionIndex, place.SiteDetails.regionName);

                    findPlace = place;
                    string entryStr = string.Format( // Display using localized name
                        TextManager.Instance.GetLocalizedText("locationInRegionProvince"),
                        TextManager.Instance.GetLocalizedLocationName(place.SiteDetails.mapId, findPlaceName),
                        TextManager.Instance.GetLocalizedRegionName(regionIndex));
                    DaggerfallMessageBox dialogBox = CreateDialogBox(entryStr, "confirmFind");
                    dialogBox.OnButtonClick += FindPlace_OnButtonClick;
                    DaggerfallUI.UIManager.PushWindow(dialogBox);
                }
            }
        }

        protected virtual Place GetLastPlaceMentionedInMessage(Message message)
        {
            QuestMacroHelper helper = new QuestMacroHelper();
            QuestResource[] resources = helper.GetMessageResources(message);
            if (resources == null || resources.Length == 0)
                return null;

            Place lastPlace = null;
            foreach(QuestResource resource in resources)
            {
                if (resource is Place)
                    lastPlace = (Place)resource;
            }

            return lastPlace;
        }

        protected virtual DaggerfallMessageBox CreateDialogBox(string entryStr, string baseKey)
        {
            string heading = TextManager.Instance.GetLocalizedText(baseKey + "Head");
            string action = TextManager.Instance.GetLocalizedText(baseKey);
            string explanation = TextManager.Instance.GetLocalizedText(baseKey + "2");
            TextFile.Token[] tokens = new TextFile.Token[] {
                TextFile.CreateTextToken(heading), TextFile.CreateFormatToken(TextFile.Formatting.JustifyCenter), TextFile.NewLineToken,
                TextFile.CreateTextToken(action), TextFile.NewLineToken, TextFile.NewLineToken,
                new TextFile.Token() { text = entryStr, formatting = TextFile.Formatting.TextHighlight }, TextFile.CreateFormatToken(TextFile.Formatting.JustifyCenter), TextFile.NewLineToken,
                TextFile.CreateTextToken(explanation), TextFile.CreateFormatToken(TextFile.Formatting.EndOfRecord)
            };

            DaggerfallMessageBox dialogBox = new DaggerfallMessageBox(uiManager, this);
            dialogBox.SetHighlightColor(Color.white);
            dialogBox.SetTextTokens(tokens);
            dialogBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.Yes);
            dialogBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.No);
            return dialogBox;
        }

        protected virtual void EnterNote(int index)
        {
            if (DisplayMode == JournalDisplay.Notebook)
            {
                selectedEntry = -index + currentMessageIndex;
                Debug.Log("Add at " + selectedEntry);

                TextFile.Token prompt = new TextFile.Token() {
                    text = TextManager.Instance.GetLocalizedText("enterNote"),
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

        protected virtual TextFile.Token[] GetEntry(int index)
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

        protected virtual void MoveEntry(int srcIdx, int destIdx)
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

        protected virtual void RemoveEntry(int index)
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

        protected virtual void SetTextActiveQuests()
        {
            messageCount = questMessages.Count;
            questLogLabel.TextScale = 1;
            titleLabel.Text = TextManager.Instance.GetLocalizedText("activeQuests");
            titleLabel.ToolTipText = TextManager.Instance.GetLocalizedText("activeQuestsInfo");

            int totalLineCount = 0;
            entryLineMap = new List<int>(maxLinesQuests);
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

        protected virtual void SetTextFinshedQuests()
        {
            List<TextFile.Token[]> finishedQuests = GameManager.Instance.PlayerEntity.Notebook.GetFinishedQuests();
            messageCount = finishedQuests.Count;
            questLogLabel.TextScale = 1;
            titleLabel.Text = TextManager.Instance.GetLocalizedText("finishedQuests");
            titleLabel.ToolTipText = TextManager.Instance.GetLocalizedText("finishedQuestsInfo");
            SetTextWithListEntries(finishedQuests, maxLinesQuests);
        }

        protected virtual void SetTextNotebook()
        {
            List<TextFile.Token[]> notes = GameManager.Instance.PlayerEntity.Notebook.GetNotes();
            messageCount = notes.Count;
            questLogLabel.TextScale = textScaleSmall;
            titleLabel.Text = TextManager.Instance.GetLocalizedText("notebook");
            titleLabel.ToolTipText = TextManager.Instance.GetLocalizedText("notebookInfo");
            SetTextWithListEntries(notes, maxLinesSmall);
        }

        protected virtual void SetTextMessages()
        {
            List<TextFile.Token[]> messages = GameManager.Instance.PlayerEntity.Notebook.GetMessages();
            messageCount = messages.Count;
            questLogLabel.TextScale = textScaleSmall;
            titleLabel.Text = TextManager.Instance.GetLocalizedText("messages");
            titleLabel.ToolTipText = TextManager.Instance.GetLocalizedText("messagesInfo");
            SetTextWithListEntries(messages, maxLinesSmall);
        }

        protected virtual void SetTextWithListEntries(List<TextFile.Token[]> entries, int maxLines)
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
