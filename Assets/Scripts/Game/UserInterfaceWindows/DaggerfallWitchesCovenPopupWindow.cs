// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut
// Contributors: 
// 
// Notes:
//  This is only for witch covens currently, but may be generalised if similarly handled NPCs are found.

using UnityEngine;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Questing;
using DaggerfallConnect.Arena2;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    public class DaggerfallWitchesCovenPopupWindow : DaggerfallQuestPopupWindow
    {
        #region UI Rects

        Rect talkButtonRect = new Rect(5, 5, 120, 7);
        Rect summonButtonRect = new Rect(5, 14, 120, 7);
        Rect questButtonRect = new Rect(5, 23, 120, 7);
        Rect exitButtonRect = new Rect(44, 33, 43, 15);

        #endregion

        #region UI Controls

        Panel mainPanel = new Panel();
        Button talkButton = new Button();
        Button summonButton = new Button();
        Button questButton = new Button();
        Button exitButton = new Button();

        #endregion

        #region Fields

        const string baseTextureName = "DAED00I0.IMG";      // Talk / Daedra Summoning / Quest
        Texture2D baseTexture;

        StaticNPC witchNPC;

        bool isCloseWindowDeferred = false;
        bool isTalkWindowDeferred = false;
        bool isSummonDeferred = false;
        bool isGetQuestDeferred = false;

        #endregion

        #region Constructors

        public DaggerfallWitchesCovenPopupWindow(IUserInterfaceManager uiManager, StaticNPC npc)
            : base(uiManager)
        {
            witchNPC = npc;
            // Clear background
            ParentPanel.BackgroundColor = Color.clear;
        }

        #endregion

        #region Setup Methods

        protected override void Setup()
        {
            // Load all textures
            LoadTextures();

            // Create interface panel
            mainPanel.HorizontalAlignment = HorizontalAlignment.Center;
            mainPanel.VerticalAlignment = VerticalAlignment.Middle;
            mainPanel.BackgroundTexture = baseTexture;
            mainPanel.Position = new Vector2(0, 50);
            mainPanel.Size = new Vector2(130, 51);

            // Talk button
            talkButton = DaggerfallUI.AddButton(talkButtonRect, mainPanel);
            talkButton.OnMouseClick += TalkButton_OnMouseClick;
            talkButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.WitchesTalk);
            talkButton.OnKeyboardEvent += TalkButton_OnKeyboardEvent;

            // Summon button
            summonButton = DaggerfallUI.AddButton(summonButtonRect, mainPanel);
            summonButton.OnMouseClick += SummonButton_OnMouseClick;
            summonButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.WitchesDaedraSummon);
            summonButton.OnKeyboardEvent += SummonButton_OnKeyboardEvent;

            // Quest button
            questButton = DaggerfallUI.AddButton(questButtonRect, mainPanel);
            questButton.OnMouseClick += QuestButton_OnMouseClick;
            questButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.WitchesQuest);
            questButton.OnKeyboardEvent += QuestButton_OnKeyboardEvent;

            // Exit button
            exitButton = DaggerfallUI.AddButton(exitButtonRect, mainPanel);
            exitButton.OnMouseClick += ExitButton_OnMouseClick;
            exitButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.WitchesExit);
            exitButton.OnKeyboardEvent += ExitButton_OnKeyboardEvent;

            NativePanel.Components.Add(mainPanel);
        }

        #endregion

        #region Private Methods

        void LoadTextures()
        {
            baseTexture = ImageReader.GetTexture(baseTextureName);
        }

        #endregion

        #region Quest handling

        protected override void GetQuest()
        {
            // Just exit if this NPC already involved in an active quest
            // If quest conditions are complete the quest system should pickup ending
            if (QuestMachine.Instance.IsLastNPCClickedAnActiveQuestor())
            {
                CloseWindow();
                return;
            }

            // Get the faction id for affecting reputation on success/failure, and current rep
            int factionId = witchNPC.Data.factionID;
            int reputation = GameManager.Instance.PlayerEntity.FactionData.GetReputation(factionId);
            int level = GameManager.Instance.PlayerEntity.Level;     // Not a proper guild so rank = player level

            // Select a quest at random from appropriate pool
            offeredQuest = GameManager.Instance.QuestListsManager.GetGuildQuest(FactionFile.GuildGroups.Witches, MembershipStatus.Nonmember, factionId, reputation, level);
            if (offeredQuest != null)
            {
                // Log offered quest
                Debug.LogFormat("Offering quest {0} from Guild {1} affecting factionId {2}", offeredQuest.QuestName, FactionFile.GuildGroups.Witches, offeredQuest.FactionId);

                // Offer the quest to player
                DaggerfallMessageBox messageBox = QuestMachine.Instance.CreateMessagePrompt(offeredQuest, (int)QuestMachine.QuestMessages.QuestorOffer);// TODO - need to provide an mcp for macros
                if (messageBox != null)
                {
                    messageBox.OnButtonClick += OfferQuest_OnButtonClick;
                    messageBox.Show();
                }
            }
            else
            {
                ShowFailGetQuestMessage();
            }
        }

        #endregion

        #region Event Handlers

        private void TalkButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            CloseWindow();
            GameManager.Instance.TalkManager.TalkToStaticNPC(witchNPC);
        }

        void TalkButton_OnKeyboardEvent(BaseScreenComponent sender, Event keyboardEvent)
        {
            if (keyboardEvent.type == EventType.KeyDown)
            {
                DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
                isTalkWindowDeferred = true;
            }
            else if (keyboardEvent.type == EventType.KeyUp && isTalkWindowDeferred)
            {
                isTalkWindowDeferred = false;
                CloseWindow();
                GameManager.Instance.TalkManager.TalkToStaticNPC(witchNPC);
            }
        }

        private void SummonButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            DaedraSummoningService(witchNPC.Data.factionID);
        }

        void SummonButton_OnKeyboardEvent(BaseScreenComponent sender, Event keyboardEvent)
        {
            if (keyboardEvent.type == EventType.KeyDown)
            {
                DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
                isSummonDeferred = true;
            }
            else if (keyboardEvent.type == EventType.KeyUp && isSummonDeferred)
            {
                isSummonDeferred = false;
                DaedraSummoningService(witchNPC.Data.factionID);
            }
        }

        private void QuestButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            GetQuest();
        }

        void QuestButton_OnKeyboardEvent(BaseScreenComponent sender, Event keyboardEvent)
        {
            if (keyboardEvent.type == EventType.KeyDown)
            {
                DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
                isGetQuestDeferred = true;
            }
            else if (keyboardEvent.type == EventType.KeyUp && isGetQuestDeferred)
            {
                isGetQuestDeferred = false;
                GetQuest();
            }
        }

        private void ExitButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            CloseWindow();
        }

        protected void ExitButton_OnKeyboardEvent(BaseScreenComponent sender, Event keyboardEvent)
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

        #endregion

        #region Macro handling

        public override MacroDataSource GetMacroDataSource()
        {
            return new WitchCovenMacroDataSource(this);
        }

        /// <summary>
        /// MacroDataSource context sensitive methods for guild services window.
        /// </summary>
        private class WitchCovenMacroDataSource : MacroDataSource
        {
            private DaggerfallWitchesCovenPopupWindow parent;
            public WitchCovenMacroDataSource(DaggerfallWitchesCovenPopupWindow witchCovenWindow)
            {
                this.parent = witchCovenWindow;
            }

            public override string Daedra()
            {
                FactionFile.FactionData factionData;
                if (GameManager.Instance.PlayerEntity.FactionData.GetFactionData(parent.daedraToSummon.factionId, out factionData))
                    return factionData.name;
                else
                    return "%dae[error]";
            }
        }

        #endregion

    }
}