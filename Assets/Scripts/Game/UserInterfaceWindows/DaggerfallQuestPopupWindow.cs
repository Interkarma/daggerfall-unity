// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut
// Contributors:
//
// Notes:
//

using UnityEngine;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.Questing;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Formulas;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Abstract class for popup windows that can offer quests, including from summoned Daedra.
    /// </summary>
    public abstract class DaggerfallQuestPopupWindow : DaggerfallPopupWindow, IMacroContextProvider
    {
        protected const int NotEnoughGoldId = 454;
        protected const int SummonNotToday = 480;
        protected const int SummonAreYouSure = 481;
        protected const int SummonBefore = 482;
        protected const int SummonFailed = 484;
        
        public struct DaedraData
        {
            public readonly int factionId;
            public readonly int dayOfYear;
            public readonly string quest;
            public readonly string vidFile;
            public readonly Weather.WeatherType bonusCond;

            public DaedraData(int factionId, string quest, int dayOfYear, string vidFile, Weather.WeatherType bonusCond)
            {
                this.factionId = factionId;
                this.quest = quest;
                this.dayOfYear = dayOfYear;
                this.vidFile = vidFile;
                this.bonusCond = bonusCond;
            }
        }

        public static DaedraData[] daedraData = {
            new DaedraData((int) FactionFile.FactionIDs.Hircine, "X0C00Y00", 155, "HIRCINE.FLC", Weather.WeatherType.None),  // Restrict to only glenmoril witches?
            new DaedraData((int) FactionFile.FactionIDs.Clavicus_Vile, "V0C00Y00", 1, "CLAVICUS.FLC", Weather.WeatherType.None),
            new DaedraData((int) FactionFile.FactionIDs.Mehrunes_Dagon, "Y0C00Y00", 320, "MEHRUNES.FLC", Weather.WeatherType.None),
            new DaedraData((int) FactionFile.FactionIDs.Molag_Bal, "20C00Y00", 350, "MOLAGBAL.FLC", Weather.WeatherType.None),
            new DaedraData((int) FactionFile.FactionIDs.Sanguine, "70C00Y00", 46, "SANGUINE.FLC", Weather.WeatherType.Rain),
            new DaedraData((int) FactionFile.FactionIDs.Peryite, "50C00Y00", 99, "PERYITE.FLC", Weather.WeatherType.Rain),
            new DaedraData((int) FactionFile.FactionIDs.Malacath, "80C00Y00", 278, "MALACATH.FLC", Weather.WeatherType.None),
            new DaedraData((int) FactionFile.FactionIDs.Hermaeus_Mora, "W0C00Y00", 65, "HERMAEUS.FLC", Weather.WeatherType.None),
            new DaedraData((int) FactionFile.FactionIDs.Sheogorath, "60C00Y00", 32, "SHEOGRTH.FLC", Weather.WeatherType.Thunder),
            new DaedraData((int) FactionFile.FactionIDs.Boethiah, "U0C00Y00", 302, "BOETHIAH.FLC", Weather.WeatherType.Rain),
            new DaedraData((int) FactionFile.FactionIDs.Namira, "30C00Y00", 129, "NAMIRA.FLC", Weather.WeatherType.None),
            new DaedraData((int) FactionFile.FactionIDs.Meridia, "10C00Y00", 13, "MERIDIA.FLC", Weather.WeatherType.None),
            new DaedraData((int) FactionFile.FactionIDs.Vaernima, "90C00Y00", 190, "VAERNIMA.FLC", Weather.WeatherType.None),
            new DaedraData((int) FactionFile.FactionIDs.Nocturnal, "40C00Y00", 248, "NOCTURNA.FLC", Weather.WeatherType.Rain),
            new DaedraData((int) FactionFile.FactionIDs.Mephala, "Z0C00Y00", 283, "MEPHALA.FLC", Weather.WeatherType.None),
            new DaedraData((int) FactionFile.FactionIDs.Azura, "T0C00Y00", 81, "AZURA.FLC", Weather.WeatherType.None),
        };

        public static MobileTypes[] daedricFoes = new MobileTypes[] {
            MobileTypes.DaedraLord, MobileTypes.DaedraSeducer, MobileTypes.Daedroth, MobileTypes.FireDaedra, MobileTypes.FrostDaedra
        };

        protected Quest offeredQuest = null;

        protected DaedraData daedraToSummon;
        protected FactionFile.FactionData summonerFactionData;

        public DaggerfallQuestPopupWindow(IUserInterfaceManager uiManager, IUserInterfaceWindow previousWindow = null)
            : base(uiManager, previousWindow)
        {
        }

        public abstract MacroDataSource GetMacroDataSource();

        #region Service Handling: Quests

        protected abstract void GetQuest();
        
        protected virtual void ShowFailGetQuestMessage()
        {
            const int flavourMessageID = 600;

            // Display random flavour message such as "You're too late I gave the job to some spellsword"
            // This is a generic fallback hanlder, does not require quest data or MCP for this popup
            TextFile.Token[] tokens = DaggerfallUnity.Instance.TextProvider.GetRandomTokens(flavourMessageID);
            DaggerfallMessageBox messageBox = new DaggerfallMessageBox(DaggerfallUI.UIManager);
            messageBox.SetTextTokens(tokens);
            messageBox.ClickAnywhereToClose = true;
            messageBox.AllowCancel = true;
            messageBox.ParentPanel.BackgroundColor = Color.clear;
            messageBox.Show();
        }

        // Show a popup such as accept/reject message close guild window
        protected virtual void ShowQuestPopupMessage(Quest quest, int id, bool exitOnClose = true)
        {
            // Get message resource
            Message message = quest.GetMessage(id);
            if (message == null)
                return;

            // Setup popup message
            TextFile.Token[] tokens = message.GetTextTokens();
            DaggerfallMessageBox messageBox = new DaggerfallMessageBox(DaggerfallUI.UIManager);
            messageBox.SetTextTokens(tokens, this.offeredQuest.ExternalMCP);
            messageBox.ClickAnywhereToClose = true;
            messageBox.AllowCancel = true;
            messageBox.ParentPanel.BackgroundColor = Color.clear;

            // Exit menu on close if requested
            if (exitOnClose)
                messageBox.OnClose += QuestPopupMessage_OnClose;

            // Present popup message
            messageBox.Show();
        }

        protected virtual void OfferQuest_OnButtonClick(DaggerfallMessageBox sender, DaggerfallMessageBox.MessageBoxButtons messageBoxButton)
        {
            if (messageBoxButton == DaggerfallMessageBox.MessageBoxButtons.Yes)
            {
                // Show accept message, add quest
                sender.CloseWindow();
                ShowQuestPopupMessage(offeredQuest, (int)QuestMachine.QuestMessages.AcceptQuest);
                QuestMachine.Instance.InstantiateQuest(offeredQuest);

                // Assign QuestResourceBehaviour to questor NPC - this will be last NPC clicked
                // This will ensure quests actions like "hide npc" will operate on questor at quest startup
                if (QuestMachine.Instance.LastNPCClicked != null)
                    QuestMachine.Instance.LastNPCClicked.AssignQuestResourceBehaviour();
            }
            else
            {
                // inform TalkManager so that it can remove the quest topics that have been added
                // (note by Nystul: I know it is a bit ugly that it is added in the first place at all, but didn't find a good way to do it differently -
                // may revisit this later)
                GameManager.Instance.TalkManager.RemoveQuestInfoTopicsForSpecificQuest(offeredQuest.UID);

                // remove quest rumors (rumor mill command) for this quest from talk manager
                GameManager.Instance.TalkManager.RemoveQuestRumorsFromRumorMill(offeredQuest.UID);

                // remove quest progress rumors for this quest from talk manager
                GameManager.Instance.TalkManager.RemoveQuestProgressRumorsFromRumorMill(offeredQuest.UID);

                // Show refuse message
                sender.CloseWindow();
                ShowQuestPopupMessage(offeredQuest, (int)QuestMachine.QuestMessages.RefuseQuest, false);
            }
        }

        protected virtual void QuestPopupMessage_OnClose()
        {
            CloseWindow();
        }

        #endregion

        #region Service Handling: Daedra Summoning

        protected void DaedraSummoningService(int npcFactionId)
        {
            if (!GameManager.Instance.PlayerEntity.FactionData.GetFactionData(npcFactionId, out summonerFactionData))
            {
                DaggerfallUnity.LogMessage("Error no faction data for NPC FactionId: " + npcFactionId);
                return;
            }
            // Select appropriate Daedra for summoning attempt.
            if (summonerFactionData.id == (int) FactionFile.FactionIDs.The_Glenmoril_Witches)
            {   // Always Hircine at Glenmoril witches.
                daedraToSummon = daedraData[0];
            }
            else if ((FactionFile.FactionTypes) summonerFactionData.type == FactionFile.FactionTypes.WitchesCoven)
            {   // Witches covens summon a random Daedra.
                daedraToSummon = daedraData[Random.Range(1, daedraData.Length)];
            }
            else
            {   // Is this a summoning day?
                int dayOfYear = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.DayOfYear;
                foreach (DaedraData dd in daedraData)
                {
                    if (dd.dayOfYear == dayOfYear)
                    {
                        daedraToSummon = dd;
                        break;
                    }
                }
            }
            DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, uiManager.TopWindow);
            if (daedraToSummon.factionId == 0)
            {   // Display not summoning day message.
                TextFile.Token[] tokens = DaggerfallUnity.Instance.TextProvider.GetRandomTokens(SummonNotToday);
                messageBox.SetTextTokens(tokens, this);
                messageBox.ClickAnywhereToClose = true;
            }
            else
            {   // Ask player if they really want to risk the summoning.
                messageBox.SetTextTokens(SummonAreYouSure, this);
                messageBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.Yes);
                messageBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.No);
                messageBox.OnButtonClick += ConfirmSummon_OnButtonClick;
            }
            messageBox.Show();
        }

        private void ConfirmSummon_OnButtonClick(DaggerfallMessageBox sender, DaggerfallMessageBox.MessageBoxButtons messageBoxButton)
        {
            sender.CloseWindow();
            if (messageBoxButton == DaggerfallMessageBox.MessageBoxButtons.Yes)
            {
                PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;
                int summonCost = FormulaHelper.CalculateDaedraSummoningCost(summonerFactionData.rep);

                if (playerEntity.GetGoldAmount() >= summonCost)
                {
                    playerEntity.DeductGoldAmount(summonCost);

                    // Default 30% bonus is only applicable to some Daedra in specific weather conditions.
                    WeatherManager weatherManager = GameManager.Instance.WeatherManager;
                    int bonus = 0;
                    if (daedraToSummon.bonusCond == Weather.WeatherType.Rain && weatherManager.IsRaining ||
                        daedraToSummon.bonusCond == Weather.WeatherType.Thunder && weatherManager.IsStorming ||
                        daedraToSummon.bonusCond == Weather.WeatherType.None)
                        bonus = 30;

                    // Sheogorath has a 5% (15% if stormy) chance to replace selected daedra.
                    int sheoChance = (weatherManager.IsStorming) ? 15 : 5;
                    // Get summoning chance for selected daedra and roll.
                    int chance = FormulaHelper.CalculateDaedraSummoningChance(playerEntity.FactionData.GetReputation(daedraToSummon.factionId), bonus);
                    int roll = Random.Range(1, 101);
                    Debug.LogFormat("Summoning {0} with chance = {1}%, Sheogorath chance = {2}%, roll = {3}, summoner rep = {4}, cost: {5}",
                        daedraToSummon.vidFile.Substring(0, daedraToSummon.vidFile.Length-4), chance, sheoChance, roll, summonerFactionData.rep, summonCost);

                    if (roll > chance + sheoChance)
                    {   // Daedra stood you up!
                        DaggerfallUI.MessageBox(SummonFailed, this);
                        // Spawn daedric foes if failed at a witches coven.
                        if (summonerFactionData.ggroup == (int) FactionFile.GuildGroups.Witches)
                            GameObjectHelper.CreateFoeSpawner(true, daedricFoes[Random.Range(0, 5)], Random.Range(1, 4), 4, 64);
                        return;
                    }
                    else if (roll > chance)
                    {   // Sheogorath appears instead.
                        daedraToSummon = daedraData[8];
                    }

                    // Has this Daedra already been summoned by the player?
                    if (playerEntity.FactionData.GetFlag(daedraToSummon.factionId, FactionFile.Flags.Summoned))
                    {
                        // Close menu and push DaggerfallDaedraSummoningWindow here for video and dismissal..
                        CloseWindow();
                        uiManager.PushWindow(new DaggerfallDaedraSummonedWindow(uiManager, daedraToSummon, SummonBefore, this));
                    }
                    else
                    {   // Record the summoning.
                        playerEntity.FactionData.SetFlag(daedraToSummon.factionId, FactionFile.Flags.Summoned);

                        // Offer the quest to player.
                        offeredQuest = GameManager.Instance.QuestListsManager.GetQuest(daedraToSummon.quest, summonerFactionData.id);
                        if (offeredQuest != null)
                        {
                            // Close menu and push DaggerfallDaedraSummoningWindow here for video and custom quest offer..
                            CloseWindow();
                            uiManager.PushWindow(new DaggerfallDaedraSummonedWindow(uiManager, daedraToSummon, offeredQuest));
                        }
                    }
                }
                else
                {   // Display customised not enough gold message so players don't need to guess the cost.
                    TextFile.Token[] notEnoughGold = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(NotEnoughGoldId);
                    TextFile.Token[] msg = new TextFile.Token[] {
                        new TextFile.Token() { formatting = TextFile.Formatting.Text, text = HardStrings.serviceSummonCost1 },
                        new TextFile.Token() { formatting = TextFile.Formatting.JustifyCenter },
                        new TextFile.Token() { formatting = TextFile.Formatting.Text, text = HardStrings.serviceSummonCost2 + summonCost + HardStrings.serviceSummonCost3 },
                        new TextFile.Token() { formatting = TextFile.Formatting.JustifyCenter },
                        new TextFile.Token() { formatting = TextFile.Formatting.NewLine },
                        notEnoughGold[0],
                        new TextFile.Token() { formatting = TextFile.Formatting.JustifyCenter },
                    };
                    DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, uiManager.TopWindow);
                    messageBox.SetTextTokens(msg, this);
                    messageBox.ClickAnywhereToClose = true;
                    messageBox.Show();
                }
            }
        }

        #endregion

    }
}