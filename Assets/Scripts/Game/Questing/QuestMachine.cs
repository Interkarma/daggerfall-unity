// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2023 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    Hazelnut
// 
// Notes:
//

using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using FullSerializer;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Game.Questing.Actions;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using UnityEngine.Localization.Settings;
using System.Globalization;

namespace DaggerfallWorkshop.Game.Questing
{
    /// <summary>
    /// Hosts quests and manages their execution during play.
    /// Quests are instantiated from a source text template.
    /// It's possible to have the same quest multiple times (e.g. same fetch quest from two different mage guildhalls).
    /// Running quests can perform actions in the world (e.g. spawn enemies and play sounds).
    /// Or they can provide data to external systems like the NPC dialog interface (e.g. 'tell me about' and 'rumors').
    /// </summary>
    public class QuestMachine : MonoBehaviour
    {
        #region Fields

        // Public constants
        public const string questPersonTag = "QuestPerson";
        public const string questFoeTag = "QuestFoe";
        public const string questItemTag = "QuestItem";

        const float startupDelay = 0f;          // How long quest machine will wait before running active quests
        const float ticksPerSecond = 10;        // How often quest machine will tick quest logic per second

        // Folder names constants
        const string questSourceFolderName = "Quests";
        const string questTablesFolderName = "Tables";

        // Log file
        const string questLogFilename = "quest_log.txt";

        // Table constants
        const string globalVarsTableFilename = "Quests-GlobalVars";
        const string staticMessagesTableFilename = "Quests-StaticMessages";
        const string placesTableFilename = "Quests-Places";
        const string soundsTableFilename = "Quests-Sounds";
        const string itemsTableFileName = "Quests-Items";
        const string factionsTableFileName = "Quests-Factions";
        const string foesTableFileName = "Quests-Foes";
        const string diseasesTableFileName = "Quests-Diseases";
        const string spellsTableFileName = "Quests-Spells";

        // Localization
        const string localizedFilenameSuffix = "-LOC";
        const string fileExtension = ".txt";
        const string textFolderName = "Text";
        const string questsFolderName = "Quests";

        // Data tables
        Table globalVarsTable;
        Table staticMessagesTable;
        Table placesTable;
        Table soundsTable;
        Table itemsTable;
        Table factionsTable;
        Table foesTable;
        Table diseasesTable;
        Table spellsTable;

        List<IQuestAction> actionTemplates = new List<IQuestAction>();
        Dictionary<ulong, Quest> quests = new Dictionary<ulong, Quest>();
        List<SiteLink> siteLinks = new List<SiteLink>();
        List<Quest> questsToTombstone = new List<Quest>();
        List<Quest> questsToRemove = new List<Quest>();
        List<Quest> questsToInvoke = new List<Quest>();
        List<StoredException> storedExceptions = new List<StoredException>();

        bool waitingForStartup = true;
        float startupTimer = 0;
        float updateTimer = 0;

        StaticNPC lastNPCClicked;
        Dictionary<int, IQuestAction> factionListeners = new Dictionary<int, IQuestAction>();

        Dictionary<string, string> localizedQuestNames = new Dictionary<string, string>();

        System.Random internalSeed = new System.Random();

        #endregion

        #region Properties

        /// <summary>
        /// Gets count of all quests running at this time.
        /// </summary>
        public int QuestCount
        {
            get { return quests.Count; }
        }

        /// <summary>
        /// Gets current count of all active SiteLinks.
        /// </summary>
        public int SiteLinkCount
        {
            get { return siteLinks.Count; }
        }

        /// <summary>
        /// Gets Quests source folder in StreamingAssets.
        /// </summary>
        public static string QuestSourceFolder
        {
            get { return Path.Combine(Application.streamingAssetsPath, questSourceFolderName); }
        }

        /// <summary>
        /// Gets Tables source folder in StreamingAssets.
        /// TODO: This folder isn't ultimately exclusive to quests. Find a more generic spot later, e.g. GameManager.
        /// </summary>
        public string TablesSourceFolder
        {
            get { return Path.Combine(Application.streamingAssetsPath, questTablesFolderName); }
        }

        /// <summary>
        /// Gets the global variables data table.
        /// </summary>
        public Table GlobalVarsTable
        {
            get { return globalVarsTable; }
        }

        /// <summary>
        /// Gets the static message names data table.
        /// </summary>
        public Table StaticMessagesTable
        {
            get { return staticMessagesTable; }
        }

        /// <summary>
        /// Gets the places data table.
        /// </summary>
        public Table PlacesTable
        {
            get { return placesTable; }
        }

        /// <summary>
        /// Gets the sounds data table.
        /// </summary>
        public Table SoundsTable
        {
            get { return soundsTable; }
        }

        /// <summary>
        /// Gets the items data table.
        /// </summary>
        public Table ItemsTable
        {
            get { return itemsTable; }
        }

        /// <summary>
        /// Gets the factions data table.
        /// </summary>
        public Table FactionsTable
        {
            get { return factionsTable; }
        }

        /// <summary>
        /// Gets the foes data table.
        /// </summary>
        public Table FoesTable
        {
            get { return foesTable; }
        }

        /// <summary>
        /// Gets the diseases data table.
        /// </summary>
        public Table DiseasesTable
        {
            get { return diseasesTable; }
        }

        /// <summary>
        /// Gets the spells data table.
        /// </summary>
        public Table SpellsTable
        {
            get { return spellsTable; }
        }

        /// <summary>
        /// Gets or sets StaticNPC last clicked by player.
        /// </summary>
        public StaticNPC LastNPCClicked
        {
            get { return lastNPCClicked; }
            set { SetLastNPCClicked(value); }
        }

        /// <summary>
        /// Returns true if debug mode enabled.
        /// This causes original quest source line to be stored and serialized with quests.
        /// Always enabled at this stage of development.
        /// </summary>
        public bool IsDebugModeEnabled
        {
            get { return true; }
        }

        /// <summary>
        /// Gets full path to quest log file.
        /// </summary>
        private static string LogPath
        {
            get { return Path.Combine(DaggerfallUnity.Settings.PersistentDataPath, questLogFilename); }
        }

        /// <summary>
        /// Return a new random seed
        /// </summary>
        public int InternalSeed { get { return internalSeed.Next(); } }

        #endregion

        #region Structs & Enums

        /// <summary>
        /// Stores information about an exception that would result in quest termination.
        /// This is stored permanently with save data to help with troubleshooting.
        /// </summary>
        public struct StoredException
        {
            public string questName;
            public Exception exception;
            public string stackTrace;
        }

        /// <summary>
        /// Fixed quest message constants.
        /// </summary>
        public enum QuestMessages
        {
            QuestorOffer = 1000,
            RefuseQuest = 1001,
            AcceptQuest = 1002,
            QuestFail = 1003,
            QuestComplete = 1004,
            RumorsDuringQuest = 1005,
            RumorsPostFailure = 1006,
            RumorsPostSuccess = 1007,
            QuestorPostSuccess = 1008,
            QuestorPostFailure = 1009,
        }

        #endregion

        #region Unity

        void Awake()
        {
            SetupSingleton();

            globalVarsTable = new Table(Instance.GetTableSourceText(globalVarsTableFilename));
            staticMessagesTable = new Table(Instance.GetTableSourceText(staticMessagesTableFilename));
            placesTable = new Table(Instance.GetTableSourceText(placesTableFilename));
            soundsTable = new Table(Instance.GetTableSourceText(soundsTableFilename));
            itemsTable = new Table(Instance.GetTableSourceText(itemsTableFileName));
            factionsTable = new Table(Instance.GetTableSourceText(factionsTableFileName));
            foesTable = new Table(Instance.GetTableSourceText(foesTableFileName));
            diseasesTable = new Table(Instance.GetTableSourceText(diseasesTableFileName));
            spellsTable = new Table(Instance.GetTableSourceText(spellsTableFileName));
        }

        void Start()
        {
            ClearLog();
            RegisterActionTemplates();
        }

        private void Update()
        {
            // Handle startup delay
            if (waitingForStartup)
            {
                startupTimer += Time.deltaTime;
                if (startupTimer < startupDelay)
                    return;
                waitingForStartup = false;
            }

            // Do not tick while HUD fading or load in progress
            // This is to prevent quest popups or other actions while player/world unavailable
            if (DaggerfallUI.Instance == null || DaggerfallUI.Instance.FadeBehaviour.FadeInProgress ||
                SaveLoadManager.Instance == null || SaveLoadManager.Instance.LoadInProgress)
            {
                return;
            }

            // Increment update timer
            updateTimer += Time.deltaTime;
            if (updateTimer < (1f / ticksPerSecond))
                return;

            // Tick quest machine
            Tick();

            // Reset update timer
            updateTimer = 0;
        }

        #endregion

        #region Action Methods

        /// <summary>
        /// All actions must be registered here so they can be evaluated and factoried at runtime.
        /// If an action pattern match cannot be found that action will just be ignored by quest system.
        /// The goal is to add incremental action support over time until 100% compatibility is reached.
        /// </summary>
        void RegisterActionTemplates()
        {
            // Register example actions
            //RegisterAction(new JuggleAction(null));

            // Register trigger conditions
            RegisterAction(new WhenPcEntersExits(null));
            RegisterAction(new WhenNpcIsAvailable(null));
            RegisterAction(new WhenReputeWith(null));
            RegisterAction(new WhenSkillLevel(null));
            RegisterAction(new WhenAttributeLevel(null));
            RegisterAction(new WhenTask(null));
            RegisterAction(new ClickedNpc(null));
            RegisterAction(new ClickedItem(null));
            RegisterAction(new LevelCompleted(null));
            RegisterAction(new InjuredFoe(null));
            RegisterAction(new KilledFoe(null));
            RegisterAction(new TotingItemAndClickedNpc(null));
            RegisterAction(new DailyFrom(null));
            RegisterAction(new DroppedItemAtPlace(null));
            RegisterAction(new Season(null));
            RegisterAction(new Actions.Weather(null));
            RegisterAction(new Climate(null));

            // Register default actions
            RegisterAction(new EndQuest(null));
            RegisterAction(new Prompt(null));
            RegisterAction(new Say(null));
            RegisterAction(new PlaySound(null));
            RegisterAction(new StartTask(null));
            RegisterAction(new ClearTask(null));
            RegisterAction(new LogMessage(null));
            RegisterAction(new PickOneOf(null));
            RegisterAction(new RemoveLogMessage(null));
            RegisterAction(new PlayVideo(null));
            RegisterAction(new PcAt(null));
            RegisterAction(new CreateNpcAt(null));
            RegisterAction(new CreateNpc(null));
            RegisterAction(new PlaceNpc(null));
            RegisterAction(new PlaceItem(null));
            RegisterAction(new GivePc(null));
            RegisterAction(new GiveItem(null));
            RegisterAction(new StartStopTimer(null));
            RegisterAction(new CreateFoe(null));
            RegisterAction(new PlaceFoe(null));
            RegisterAction(new HideNpc(null));
            RegisterAction(new RestoreNpc(null));
            RegisterAction(new AddFace(null));
            RegisterAction(new DropFace(null));
            RegisterAction(new GetItem(null));
            RegisterAction(new StartQuest(null));
            RegisterAction(new RunQuest(null));
            RegisterAction(new UnsetTask(null));
            RegisterAction(new ChangeReputeWith(null));
            RegisterAction(new ReputeExceedsDo(null));
            RegisterAction(new RevealLocation(null));
            RegisterAction(new RestrainFoe(null));
            RegisterAction(new MakePermanent(null));
            RegisterAction(new HaveItem(null));
            RegisterAction(new AddAsQuestor(null));
            RegisterAction(new DropAsQuestor(null));
            RegisterAction(new ItemUsedDo(null));
            RegisterAction(new TakeItem(null));
            RegisterAction(new TeleportPc(null));
            RegisterAction(new DialogLink(null));
            RegisterAction(new AddDialog(null));
            RegisterAction(new RumorMill(null));
            RegisterAction(new MakePcDiseased(null));
            RegisterAction(new CurePcDisease(null));
            RegisterAction(new CastSpellDo(null));
            RegisterAction(new CastEffectDo(null));
            RegisterAction(new CastSpellOnFoe(null));
            RegisterAction(new RemoveFoe(null));
            RegisterAction(new LegalRepute(null));
            RegisterAction(new MuteNpc(null));
            RegisterAction(new DestroyNpc(null));
            RegisterAction(new WorldUpdate(null));
            RegisterAction(new Enemies(null));
            RegisterAction(new ClickedFoe(null));
            RegisterAction(new KillFoe(null));
            RegisterAction(new PayMoney(null));
            RegisterAction(new JournalNote(null));
            RegisterAction(new ChangeFoeInfighting(null));
            RegisterAction(new ChangeFoeTeam(null));
            RegisterAction(new PlaySong(null));
            RegisterAction(new SetPlayerCrime(null));
            RegisterAction(new SpawnCityGuards(null));
            RegisterAction(new UnrestrainFoe(null));
            RegisterAction(new TrainPc(null));
            RegisterAction(new PromptMulti(null));

            // Raise event for custom actions to be registered
            RaiseOnRegisterCustomerActionsEvent();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Tick quest machine.
        /// </summary>
        public void Tick()
        {
            // Invoke scheduled quests
            foreach (Quest quest in questsToInvoke)
            {
                if (quest != null)
                {
                    try
                    {
                        StartQuest(quest);
                        RaiseOnQuestStartedEvent(quest);
                    }
                    catch (Exception ex)
                    {
                        LogFormat("QuestMachine encountered an exception while starting quest {0}. Quest will not be started. Exception message: '{1}'", quest.QuestName, ex.Message);
                    }
                }
            }
            questsToInvoke.Clear();

            // Update quests
            questsToTombstone.Clear();
            questsToRemove.Clear();
            foreach (Quest quest in quests.Values)
            {
                try
                {
                    // Tick active quests
                    if (!quest.QuestComplete)
                        quest.Update();
                }
                catch (Exception ex)
                {
                    if (IsProtectedQuest(quest))
                    {
                        LogFormat(quest, "Exception in protected quest. Logging only.");
                        StoreQuestException(quest, ex);
                        LogFormat(ex.Message);
                    }
                    else
                    {
                        LogFormat(quest, "Error in quest follows. Terminating quest runtime.");
                        LogFormat(ex.Message);
                        StoreQuestException(quest, ex);
                        RaiseOnQuestErrorTerminationEvent(quest);
                        questsToRemove.Add(quest);
                    }
                }

                // Schedule completed quests for tombstoning
                if (quest.QuestComplete && !quest.QuestTombstoned)
                    questsToTombstone.Add(quest);

                // Expire tombstoned quests after 1 in-game week
                if (quest.QuestTombstoned)
                {
                    if (DaggerfallUnity.Instance.WorldTime.Now.ToSeconds() - quest.QuestTombstoneTime.ToSeconds() > DaggerfallDateTime.SecondsPerWeek)
                        questsToRemove.Add(quest);
                }
            }

            // Tombstone completed quests after update
            foreach (Quest quest in questsToTombstone)
            {
                TombstoneQuest(quest);
            }

            // Remove expired quests
            foreach (Quest quest in questsToRemove)
            {
                RemoveQuest(quest);
            }

            // Fire tick event
            RaiseOnTickEvent();
        }

        /// <summary>
        /// Checks if a quest is protected from ending prematurely.
        /// </summary>
        /// <returns>True if quest is protected.</returns>
        public static bool IsProtectedQuest(Quest quest)
        {
            return string.Compare(quest.QuestName, "S0000999", true) == 0 ||
                   string.Compare(quest.QuestName, "S0000977", true) == 0 ||
                   string.Compare(quest.QuestName, "_BRISIEN", true) == 0;
        }

        /// <summary>
        /// Resets operating state - clears all quests, sitelinks, debuggers, etc.
        /// Quests will not be disposed or tombstoned they will just be dropped for garbage collector.
        /// </summary>
        public void ClearState()
        {
            // Clear state
            quests.Clear();
            siteLinks.Clear();
            questsToTombstone.Clear();
            questsToRemove.Clear();
            questsToInvoke.Clear();
            lastNPCClicked = null;

            // Clear debugger state
            if (DaggerfallUI.Instance.DaggerfallHUD != null)
                DaggerfallUI.Instance.DaggerfallHUD.PlaceMarker.ClearSiteTargets();
        }

        /// <summary>
        /// Register a new action in the quest engine.
        /// </summary>
        /// <param name="actionTemplate">IQuestAction template.</param>
        public void RegisterAction(IQuestAction actionTemplate)
        {
            actionTemplates.Add(actionTemplate);
        }

        /// <summary>
        /// Attempts to load quest source text from StreamingAssets/Quests.
        /// </summary>
        /// <param name="questName">Quest filename. Extension .txt is optional.</param>
        /// <returns>Array of lines in quest text, or empty array.</returns>
        public string[] GetQuestSourceText(string questName)
        {
            string[] source = new string[0];

            // Append extension if not present
            if (!questName.EndsWith(".txt"))
                questName += ".txt";

            // Attempt to load quest source file
            string path = Path.Combine(QuestSourceFolder, questName);
            if (!File.Exists(path))
            {
                LogFormat(questName, "Quest filename path {0} not found.", path);
            }
            else
            {
                source = File.ReadAllLines(path);
            }

            return source;
        }

        /// <summary>
        /// Attempts to load table text from StreamingAssets/Tables.
        /// TODO: Tables are ultimately not exclusive to quests. Relocate this later.
        /// </summary>
        /// <param name="tableName">Table filename. Extension .txt is optional.</param>
        /// <returns>Array of lines in table text, or empty array.</returns>
        public string[] GetTableSourceText(string tableName)
        {
            string[] table = new string[0];

            // Append extension if not present
            if (!tableName.EndsWith(".txt"))
                tableName += ".txt";

            // Attempt to load quest source file
            string path = Path.Combine(TablesSourceFolder, tableName);
            if (!File.Exists(path))
            {
                LogFormat("Table filename path {0} not found.", path);
            }
            else
            {
                table = File.ReadAllLines(path);
            }

            return table;
        }

        /// <summary>
        /// Returns a list of all active log messages from all active quests
        /// </summary>
        /// <returns>List of log messages.</returns>
        public List<Message> GetAllQuestLogMessages()
        {
            List<Message> questMessages = new List<Message>();

            foreach (var quest in quests.Values)
            {
                Quest.LogEntry[] logEntries = quest.GetLogMessages();
                if (logEntries == null || logEntries.Length == 0)
                    continue;

                foreach (var logEntry in logEntries)
                {
                    var message = quest.GetMessage(logEntry.messageID);
                    if (message != null)
                        questMessages.Add(message);
                }
            }

            return questMessages;
        }

        /// <summary>
        /// Parses a new quest from name.
        /// AJRB: Internal use only, use QuestListsManager.GetQuest() instead.
        /// Quest will attempt to load from QuestSourceFolder property path.
        /// </summary>
        /// <param name="questName">Name of quest filename. Extensions .txt is optional.</param>
        /// <returns>Quest object if successfully parsed, otherwise null.</returns>
        private Quest ParseQuest(string questName)
        {
            string[] source = GetQuestSourceText(questName);
            if (source == null || source.Length == 0)
            {
                LogFormat("Could not load quest '{0}' or source file is empty/invalid.", questName);
                return null;
            }

            return ParseQuest(questName, source);
        }

        /// <summary>
        /// Instantiate a new quest from source text array.
        /// </summary>
        /// <param name="questName">Name of quest filename. Extensions .txt is optional.</param>
        /// <param name="questSource">Array of lines from quest source file.</param>
        /// <param name="factionId">Faction id of quest giver for guilds.</param>
        /// <param name="partialParse">If true the QRC and QBN sections will not be parsed.</param>
        /// <returns>Quest.</returns>
        public Quest ParseQuest(string questName, string[] questSource, int factionId = 0, bool partialParse = false)
        {
            LogFormat("\r\n\r\nParsing quest {0}", questName);

            UnityEngine.Random.InitState(internalSeed.Next());

            try
            {
                // Parse quest
                Parser parser = new Parser();
                Quest quest = parser.Parse(questSource, factionId, partialParse);

                // Parse localized version of quest file (if present) and store display name in quest
                if (ParseLocalizedQuestText(questName))
                    quest.DisplayName = GetLocalizedQuestDisplayName(questName);

                return quest;
            }
            catch (Exception ex)
            {
                LogFormat("Parsing quest {0} FAILED!\r\n{1}", questName, ex.Message);

                return null;
            }
        }

        /// <summary>
        /// Localized quest messages need to be restored when game is loaded.
        /// </summary>
        /// <param name="questName">Name of quest to restore localized messages.</param>
        public bool RestoreLocalizedQuestMessages(string questName)
        {
            return ParseLocalizedQuestText(questName);
        }

        /// <summary>
        /// Parse and start a quest from quest name.
        /// </summary>
        /// <param name="questName">Quest name.</param>
        /// <param name="factionId">Faction id. (optional)</param>
        /// <returns>Quest.</returns>
        public void StartQuest(string questName, int factionId = 0)
        {
            Quest quest = ParseQuest(questName);
            if (quest != null)
            {
                quest.FactionId = factionId;
                StartQuest(quest);
            }
        }

        /// <summary>
        /// Start a parsed quest.
        /// </summary>
        /// <param name="quest">Quest.</param>
        public void StartQuest(Quest quest)
        {
            quest.Start();
            
            GameManager.Instance.TalkManager.AddQuestTopicWithInfoAndRumors(quest);
            
            quests.Add(quest.UID, quest);            

            RaiseOnQuestStartedEvent(quest);

            // Assign QuestResourceBehaviour to questor NPC - this will be last NPC clicked
            // This will ensure quests actions like "hide npc" will operate on questor at quest startup
            if (LastNPCClicked != null)
            {
                LastNPCClicked.AssignQuestResourceBehaviour();
            }
        }

        /// <summary>
        /// Schedules quest to start on next tick.
        /// </summary>
        /// <param name="quest">Quest.</param>
        public void ScheduleQuest(Quest quest)
        {
            questsToInvoke.Add(quest);
        }

        /// <summary>
        /// Find registered action template based on source line.
        /// </summary>
        /// <param name="source">Action source line.</param>
        /// <returns>IQuestAction template.</returns>
        public IQuestAction GetActionTemplate(string source)
        {
            // Brute force check every registered action for now
            // Would like a more elegant way of accomplishing this
            foreach (IQuestAction action in actionTemplates)
            {
                if (action.Test(source).Success)
                    return action;
            }

            // No pattern match found
            return null;
        }

        /// <summary>
        /// Get all Place site details for all active quests.
        /// </summary>
        /// <returns>SiteDetails[] array.</returns>
        public SiteDetails[] GetAllActiveQuestSites()
        {
            List<SiteDetails> sites = new List<SiteDetails>();

            foreach (var kvp in quests)
            {
                Quest quest = kvp.Value;
                if (!quest.QuestComplete)
                {
                    QuestResource[] foundResources = quest.GetAllResources(typeof(Place));
                    foreach (QuestResource resource in foundResources)
                    {
                        sites.Add((resource as Place).SiteDetails);
                    }
                }
            }

            return sites.ToArray();
        }

        /// <summary>
        /// Gets an active or tombstoned quest based on UID.
        /// </summary>
        /// <param name="questUID">Quest UID to retrieve.</param>
        /// <returns>Quest object. Returns null if UID not found.</returns>
        public Quest GetQuest(ulong questUID)
        {
            if (!quests.ContainsKey(questUID))
                return null;

            return quests[questUID];
        }

        /// <summary>
        /// Check if quest UID has been completed in quest machine.
        /// </summary>
        /// <param name="questUID">Quest UID to check.</param>
        /// <returns>True if quest is complete. Also returns false if quest not found.</returns>
        public bool IsQuestComplete(ulong questUID)
        {
            Quest quest = GetQuest(questUID);
            if (quest == null)
                return false;

            return quest.QuestComplete;
        }

        /// <summary>
        /// Check if quest UID has been tombstoned in quest machine.
        /// </summary>
        /// <param name="questUID">Quest UID to check.</param>
        /// <returns>True if quest is tombstoned. Also returns false if quest not found.</returns>
        public bool IsQuestTombstoned(ulong questUID)
        {
            Quest quest = GetQuest(questUID);
            if (quest == null)
                return false;

            return quest.QuestTombstoned;
        }

        /// <summary>
        /// Returns an array of all quest UIDs, even if completed or tombstoned.
        /// </summary>
        /// <returns>ulong[] array of quest UIDs.</returns>
        public ulong[] GetAllQuests()
        {
            List<ulong> keys = new List<ulong>();
            foreach (ulong key in quests.Keys)
            {
                keys.Add(key);
            }

            return keys.ToArray();
        }

        /// <summary>
        /// Returns an array of all active (not completed, not tombstoned) quest UIDs.
        /// </summary>
        /// <returns>ulong[] array of quest UIDs.</returns>
        public ulong[] GetAllActiveQuests()
        {
            List<ulong> keys = new List<ulong>();
            foreach (Quest quest in quests.Values)
            {
                if (!quest.QuestComplete && !quest.QuestTombstoned)
                    keys.Add(quest.UID);
            }

            return keys.ToArray();
        }

        /// <summary>
        /// Finds quests based on quest name.
        /// </summary>
        /// <param name="questName">Quest name to search for.</param>
        /// <param name="activeOnly">Only find active quests.</param>
        /// <returns>ulong[] array of quest UIDs with matching quest name.</returns>
        public ulong[] FindQuests(string questName, bool activeOnly = false)
        {
            List<ulong> keys = new List<ulong>();
            foreach (Quest quest in quests.Values)
            {
                if (quest.QuestName == questName)
                {
                    if (activeOnly && quest.QuestTombstoned)
                        continue;
                    else
                        keys.Add(quest.UID);
                }
            }

            return keys.ToArray();
        }

        /// <summary>
        /// Creates a yes/no prompt from quest message.
        /// Caller must set events and call Show() when ready.
        /// </summary>
        public DaggerfallMessageBox CreateMessagePrompt(Quest quest, int id)
        {
            Message message = quest.GetMessage(id);
            if (message != null)
                return CreateMessagePrompt(message);
            else
                return null;
        }

        /// <summary>
        /// Creates a yes/no prompt from quest message.
        /// Caller must set events and call Show() when ready.
        /// </summary>
        public DaggerfallMessageBox CreateMessagePrompt(Message message)
        {
            TextFile.Token[] tokens = message.GetTextTokens();
            DaggerfallMessageBox messageBox = new DaggerfallMessageBox(DaggerfallUI.UIManager, DaggerfallMessageBox.CommonMessageBoxButtons.YesNo, tokens);
            messageBox.ClickAnywhereToClose = false;
            messageBox.AllowCancel = false;
            messageBox.ParentPanel.BackgroundColor = Color.clear;

            return messageBox;
        }

        /// <summary>
        /// Checks if last NPC clicked is questor for any quests.
        /// This is used for quest turn-in and reward process.
        /// </summary>
        /// <param name="mcp">External secondary context provider for quest macro expansion. (optional)</param>
        /// <returns>True if this NPC is a questor in any quest.</returns>
        public bool IsLastNPCClickedAnActiveQuestor(IMacroContextProvider mcp = null)
        {
            foreach(Quest quest in quests.Values)
            {
                if (quest.QuestComplete)
                    continue;

                QuestResource[] questPeople = quest.GetAllResources(typeof(Person));
                foreach (Person person in questPeople)
                {
                    if (person.IsQuestor)
                    {
                        if (IsNPCDataEqual(person.QuestorData, lastNPCClicked.Data))
                        {
                            LogFormat(quest, "This person is used in quest as Person {1}", person.ParentQuest.UID, person.Symbol.Original);
                            if (mcp != null)
                                quest.ExternalMCP = mcp;
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Sets the last StaticNPC clicked by player.
        /// Always called by PlayerActivate when player clicks on GameObject holding StaticNPC behaviour.
        /// </summary>
        public void SetLastNPCClicked(StaticNPC npc)
        {
            // Store the NPC clicked
            lastNPCClicked = npc;

            // Find Person resource if this NPC is involved in any quests
            foreach (Quest quest in quests.Values)
            {
                QuestResource[] questPeople = quest.GetAllResources(typeof(Person));
                foreach (Person person in questPeople)
                {
                    // Set player click in Person resource
                    if (IsNPCDataEqual(person.QuestorData, lastNPCClicked.Data))
                        person.SetPlayerClicked();
                }
            }
        }

        /// <summary>
        /// Checks if two sets of StaticNPC data reference the same NPC.
        /// Notes:
        ///  * Still working through some issues here.
        ///  * Possible for Questor NPC to be moved.
        ///  * This will likely become more robust and conditional as quest system progresses.
        /// </summary>
        /// <returns>True if person1 and person2 are considered the same.</returns>
        public bool IsNPCDataEqual(StaticNPC.NPCData person1, StaticNPC.NPCData person2)
        {
            if (person1.hash == person2.hash &&
                person1.mapID == person2.mapID &&
                person1.nameSeed == person2.nameSeed &&
                person1.buildingKey == person2.buildingKey)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Immediately tombstones then removes all quests.
        /// </summary>
        /// <returns>Number of quests removed.</returns>
        /// <param name="keepStoryQuests">Retain main story quests (start with S0000).</param>
        public int PurgeAllQuests(bool keepStoryQuests = false)
        {
            ulong[] uids = GetAllQuests();
            if (uids == null || uids.Length == 0)
                return 0;

            int nonStoryPurgeCount = 0;
            foreach (ulong uid in uids)
            {
                if (keepStoryQuests)
                {
                    Quest quest = GetQuest(uid);
                    if (quest != null && !quest.QuestName.StartsWith("S0000"))
                    {
                        RemoveQuest(uid);
                        nonStoryPurgeCount++;
                        continue;
                    }
                }
                else
                {
                    RemoveQuest(uid);
                }
            }

            // End now if only purging non-story quest
            if (keepStoryQuests)
            {
                return nonStoryPurgeCount;
            }

            // Cleanup rest of quest machine
            quests.Clear();
            siteLinks.Clear();
            questsToTombstone.Clear();
            questsToRemove.Clear();
            questsToInvoke.Clear();
            lastNPCClicked = null;

            // Clear debugger state
            DaggerfallUI.Instance.DaggerfallHUD.PlaceMarker.ClearSiteTargets();

            return uids.Length;
        }

        /// <summary>
        /// Tombstones a quest. It will remain in quest machine for "talk" links until removed.
        /// This calls Dispose() on quest, removes all related SiteLinks, then calls OnQuestEnded event.
        /// </summary>
        /// <param name="quest">Quest to Tombstone.</param>
        public void TombstoneQuest(Quest quest)
        {
            quest.Dispose();
            quest.TombstoneQuest();
            RemoveAllQuestSiteLinks(quest.UID);
            RaiseOnQuestEndedEvent(quest);
        }

        /// <summary>
        /// Removes a quest completely from quest machine.
        /// Tombstones quest before removal.
        /// </summary>
        /// <param name="questUID">Quest UID.</param>
        /// <returns>True if quest was removed.</returns>
        public bool RemoveQuest(ulong questUID)
        {
            return RemoveQuest(GetQuest(questUID));
        }

        /// <summary>
        /// Removes a quest completely from quest machine.
        /// Tombstones quest before removal.
        /// </summary>
        /// <param name="quest"></param>
        /// <returns>True if quest was removed.</returns>
        public bool RemoveQuest(Quest quest)
        {
            if (quest == null)
                return false;

            if (!quest.QuestTombstoned)
                TombstoneQuest(quest);

            quests.Remove(quest.UID);

            return true;
        }

        /// <summary>
        /// Gets any assigned Person resources of faction ID across all quests.
        /// </summary>
        /// <param name="factionID">FactionID to search for.</param>
        /// <returns>Person array.</returns>
        public Person[] ActiveFactionPersons(int factionID)
        {
            List<Person> assignedFound = new List<Person>();
            foreach (Quest quest in quests.Values)
            {
                // Exclude completed quests from active person check
                // This prevents completed/tombstoned quests from locking out NPC
                if (quest.QuestComplete)
                    continue;

                QuestResource[] persons = quest.GetAllResources(typeof(Person));
                if (persons == null || persons.Length == 0)
                    continue;

                foreach(Person person in persons)
                {
                    if (person.FactionData.id == factionID)
                        assignedFound.Add(person);
                }
            }

            return assignedFound.ToArray();
        }

        /// <summary>
        /// Find an active questor for an individual NPC faction ID across all quests.
        /// Individual NPCs can only be assigned as a questor by one quest at a time.
        /// </summary>
        /// <param name="factionID">FactionID of individual NPC to search for.</param>
        /// <returns>Person resource.</returns>
        public Person ActiveQuestor(int factionID)
        {
            Person found = null;
            foreach (Quest quest in quests.Values)
            {
                Symbol[] questorSymbols = quest.GetQuestors();
                if (questorSymbols == null || questorSymbols.Length == 0)
                    continue;

                foreach (Symbol symbol in questorSymbols)
                {
                    Person person = quest.GetPerson(symbol);
                    if (person == null)
                        continue;

                    if (person.FactionData.id == factionID)
                    {
                        found = person;
                        break;
                    }
                }
            }

            return found;
        }

        /// <summary>
        /// Finds an active questor given a specific static NPC.
        /// NPCData passed must be populated with runtime data such as map id and building key.
        /// </summary>
        public Person ActiveQuestor(StaticNPC.NPCData npcData)
        {
            Person found = null;
            foreach (Quest quest in quests.Values)
            {
                Symbol[] questorSymbols = quest.GetQuestors();
                if (questorSymbols == null || questorSymbols.Length == 0)
                    continue;

                foreach (Symbol symbol in questorSymbols)
                {
                    Person person = quest.GetPerson(symbol);
                    if (person == null)
                        continue;

                    if (IsNPCDataEqual(npcData, person.QuestorData))
                    {
                        found = person;
                        break;
                    }
                }
            }

            return found;
        }

        /// <summary>
        /// Check if a faction listener is active for the individual faction ID.
        /// </summary>
        public bool HasFactionListener(int factionID)
        {
            return factionListeners.ContainsKey(factionID);
        }

        /// <summary>
        /// Add a faction listener for the individual faction ID.
        /// Does nothing if faction ID already claimed.
        /// </summary>
        public void AddFactionListener(int factionID, IQuestAction owner)
        {
            if (!HasFactionListener(factionID))
                factionListeners.Add(factionID, owner);
        }

        /// <summary>
        /// Remove a faction listener for the individual faction ID.
        /// Does nothing if faction ID not claimed.
        /// </summary>
        /// <param name="factionID"></param>
        public void RemoveFactionListener(int factionID)
        {
            if (HasFactionListener(factionID))
                factionListeners.Remove(factionID);
        }

        public void ClearQuests()
        {
            PurgeAllQuests();
            LastNPCClicked = null;
        }

        /// <summary>
        /// Unmutes all quest NPCs.
        /// Supports console command workaround for "mute npc" bug present up to 0.7.36.
        /// </summary>
        public int UnmuteQuestNPCs()
        {
            int count = 0;
            foreach (Quest quest in quests.Values)
            {
                QuestResource[] persons = quest.GetAllResources(typeof(Person));
                if (persons == null || persons.Length == 0)
                    continue;

                foreach (Person person in persons)
                {
                    if (person.IsMuted)
                    {
                        person.IsMuted = false;
                        count++;
                    }
                }
            }

            return count;
        }

        #endregion

        #region Site Links

        /// <summary>
        /// Adds a site link to quest machine.
        /// There is no strong unique key to use for site links so they are stored in a flat list.
        /// Only a small number of site links will be ever active at one time in normal play.
        /// </summary>
        /// <param name="siteLink">SiteLink to add.</param>
        public void AddSiteLink(SiteLink siteLink)
        {
            siteLinks.Add(siteLink);
        }

        /// <summary>
        /// Removes all site links for a quest.
        /// Typically done when quest has completed.
        /// </summary>
        /// <param name="questUID">UID of quest to remove site links to.</param>
        public void RemoveAllQuestSiteLinks(ulong questUID)
        {
            while (RemoveQuestSiteLink(questUID)) { }
        }

        /// <summary>
        /// Selects all actives site links matching parameters.
        /// Very little information is needed to determine if player is in Town, Dungeon, or Building.
        /// This information is intended to be easily reached by scene builders at layout time.
        /// </summary>
        /// <param name="siteType">Type of sites to select.</param>
        /// <param name="mapId">MapID in world.</param>
        /// <param name="buildingKey">Building key for buidings. Not used if left at default 0.</param>
        /// <param name="magicNumberIndex">Magic number index for fixed sites. Not used is left at default 0.</param>
        /// <returns>SiteLink[] array of found links. Check for null or empty on return.</returns>
        public SiteLink[] GetSiteLinks(SiteTypes siteType, int mapId, int buildingKey = 0, int magicNumberIndex = 0)
        {
            // Collect a copy of all site links matching params
            List<SiteLink> foundSiteLinks = new List<SiteLink>();
            foreach (SiteLink link in siteLinks)
            {
                // Match base siteType and mapID
                bool isMatch = false;
                if (link.siteType == siteType && link.mapId == mapId)
                {
                    isMatch = true;
                }

                // Match buildingKey (if set)
                if (isMatch && buildingKey != 0)
                {
                    if (buildingKey != link.buildingKey)
                        isMatch = false;
                }

                // Match magicNumberIndex (if set)
                if (isMatch && magicNumberIndex != 0)
                {
                    if (magicNumberIndex != link.magicNumberIndex)
                        isMatch = false;
                }

                // Add link if all tests passed
                if (isMatch)
                {
                    foundSiteLinks.Add(link);
                }
            }

            return foundSiteLinks.ToArray();
        }

        /// <summary>
        /// Checks if NPC is a special individual NPC.
        /// These NPCs can exist in world even if not currently part of any active quests.
        /// </summary>
        /// <param name="factionID">Faction ID of individual NPC.</param>
        /// <returns>True if this is an individual NPC.</returns>
        public bool IsIndividualNPC(int factionID)
        {
            if (GameManager.Instance.PlayerEntity != null)
            {
                FactionFile.FactionData factionData;
                bool foundFaction = GameManager.Instance.PlayerEntity.FactionData.GetFactionData(factionID, out factionData);
                if (foundFaction && factionData.type == (int)FactionFile.FactionTypes.Individual)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if NPC is a special individual NPC, then sets it up with components required for quests.
        /// If the NPC has an Individual faction, and the NPC is currently placed somewhere else in a quest,
        /// the GameObject will be deactivated
        /// </summary>
        /// <param name="go">GameObject representing the static NPC</param>
        /// <param name="factionID">Faction ID of the NPC</param>
        /// <returns></returns>
        public bool SetupIndividualStaticNPC(GameObject go, int factionID)
        {
            if (IsIndividualNPC(factionID))
            {
                // Check if NPC has been placed elsewhere on a quest
                if (IsIndividualQuestNPCAtSiteLink(factionID))
                {
                    // Disable individual NPC if placed elsewhere
                    go.SetActive(false);
                    return false;
                }
                else
                {
                    // Always add QuestResourceBehaviour to individual NPC
                    // This is required to bootstrap quest as often questor is not set until after player clicks resource
                    QuestResourceBehaviour questResourceBehaviour = go.AddComponent<QuestResourceBehaviour>();
                    Person[] activePersonResources = QuestMachine.Instance.ActiveFactionPersons(factionID);
                    if (activePersonResources != null && activePersonResources.Length > 0)
                    {
                        Person person = activePersonResources[0];
                        questResourceBehaviour.AssignResource(person);
                        person.QuestResourceBehaviour = questResourceBehaviour;
                    }
                }
            }

            return true;
        }


        /// <summary>
        /// Walks SiteLink > Quest > Place > QuestMarkers > Target to see if an individual NPC has been placed elsewhere.
        /// Used only to determine if an individual NPC should be disabled at home location by layout builders.
        /// Ignores non-individual NPCs.
        /// </summary>
        /// <param name="factionID">Faction ID of individual NPC.</param>
        /// <returns>True if individual has been placed elsewhere, otherwise false.</returns>
        public bool IsIndividualQuestNPCAtSiteLink(int factionID)
        {
            // Check this is a valid individual
            if (!IsIndividualNPC(factionID))
                return false;

            // Iterate site links
            foreach (SiteLink link in siteLinks)
            {
                // Attempt to get Quest target
                Quest quest = GetQuest(link.questUID);
                if (quest == null)
                    continue;

                // Attempt to get Place target
                Place place = quest.GetPlace(link.placeSymbol);
                if (place == null)
                    continue;

                // Must have target resources
                SiteDetails siteDetails = place.SiteDetails;
                QuestMarker marker = siteDetails.selectedMarker;
                if (marker.targetResources == null)
                {
                    Log(quest, "IsIndividualQuestNPCAtSiteLink() found a SiteLink with no targetResources assigned.");
                    continue;
                }

                // Check spawn marker at this site for target NPC resource
                foreach(Symbol target in marker.targetResources)
                {
                    // Get target resource
                    QuestResource resource = quest.GetResource(target);
                    if (resource == null)
                        continue;

                    // Must be a Person resource
                    if (!(resource is Person))
                        continue;

                    // Person must be an individual and not at home
                    Person person = (Person)resource;
                    if (!person.IsIndividualNPC || person.IsIndividualAtHome)
                        continue;

                    // Check if factionID match to placed NPC
                    // This means we found an individual placed at site who is not supposed to be at their home location
                    if (person.FactionData.id == factionID)
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets active marker in player's current location.
        /// </summary>
        /// <param name="markerOut">Active QuestMarker out.</param>
        /// <param name="buildingOriginOut">Building origin in scene, or Vector3.zero if not inside a building.</param>
        /// <returns>True if successful.</returns>
        public bool GetCurrentLocationQuestMarker(out QuestMarker markerOut, out Vector3 buildingOriginOut)
        {
            markerOut = new QuestMarker();
            buildingOriginOut = Vector3.zero;

            // Get PlayerEnterExit for world context
            PlayerEnterExit playerEnterExit = GameManager.Instance.PlayerEnterExit;
            if (!playerEnterExit)
                return false;

            // Get SiteLinks for player's current location
            SiteLink[] siteLinks = null;
            if (playerEnterExit.IsPlayerInsideBuilding)
            {
                StaticDoor[] exteriorDoors = playerEnterExit.ExteriorDoors;
                if (exteriorDoors == null || exteriorDoors.Length < 1)
                    return false;

                siteLinks = GetSiteLinks(SiteTypes.Building, GameManager.Instance.PlayerGPS.CurrentMapID, exteriorDoors[0].buildingKey);
                if (siteLinks == null || siteLinks.Length == 0)
                    return false;

                Vector3 buildingPosition = exteriorDoors[0].buildingMatrix.GetColumn(3);
                buildingOriginOut = exteriorDoors[0].ownerPosition + buildingPosition;
            }
            else if (playerEnterExit.IsPlayerInsideDungeon)
            {
                siteLinks = GetSiteLinks(SiteTypes.Dungeon, GameManager.Instance.PlayerGPS.CurrentMapID);
            }
            else
            {
                return false;
            }

            // Exit if no links found
            if (siteLinks == null || siteLinks.Length == 0)
                return false;

            // Walk through all found SiteLinks
            foreach (SiteLink link in siteLinks)
            {
                // Get the Quest object referenced by this link
                Quest quest = GetQuest(link.questUID);
                if (quest == null)
                    return false;

                // Get the Place resource referenced by this link
                Place place = quest.GetPlace(link.placeSymbol);
                if (place == null)
                    return false;

                // Get marker
                markerOut = place.SiteDetails.selectedMarker;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes an existing quest resource allocation (if present).
        /// This is called when allocating a resource to ensure it is removed from any previous SiteLinks
        /// For example, Sx011 will move Barenziah's book from Orsinium to Scourg Barrow after a time limit expires.
        /// Allocation needs to be removed from Orsinium or item will be present in both locations.
        /// This needs to be done in a way that does not break resources allocated from other quests.
        /// </summary>
        /// <param name="resource">The resource to cull. No action taken if resource null or not found.</param>
        public void CullResourceTarget(QuestResource resource, Symbol newPlace)
        {
            // Do nothing if resource null
            if (resource == null)
                return;

            // Walk through all SiteLinks
            for (int i = 0; i < siteLinks.Count; i++)
            {
                // Get link
                SiteLink link = siteLinks[i];

                // Do nothing if this link not for same quest as resource
                if (link.questUID != resource.ParentQuest.UID)
                    continue;

                // Get the Quest object referenced by this link
                Quest quest = GetQuest(link.questUID);
                if (quest == null)
                {
                    Debug.LogWarningFormat("CullResourceTarget() could not find active quest for UID {0}", link.questUID);
                    return;
                }

                // Get the Place resource referenced by this link
                Place place = quest.GetPlace(link.placeSymbol);
                if (place == null)
                {
                    Debug.LogWarningFormat("CullResourceTarget() could not find Place symbol {0} in quest UID {1}", link.placeSymbol, link.questUID);
                    return;
                }

                // Modify selected spawn QuestMarker for this Place
                QuestMarker selectedMarker = place.SiteDetails.selectedMarker;
                if (selectedMarker.targetResources != null)
                {
                    for (int j = 0; j < selectedMarker.targetResources.Count; j++)
                    {
                        // Get target resource
                        QuestResource existingResource = quest.GetResource(selectedMarker.targetResources[j]);
                        if (existingResource == null)
                            continue;

                        // Cull matching resource
                        if (existingResource.Symbol.Equals(resource.Symbol))
                        {
                            selectedMarker.targetResources.Remove(existingResource.Symbol);
                            LogFormat(quest, "Removed resource {0} from {1}", existingResource.Symbol.Original, place.Symbol.Original);
                            break;
                        }
                    }
                }
            }

            // TODO: Might need to hot-remove items here if timer expires while player inside target site
        }

        /// <summary>
        /// Stores a quest exception in save file.
        /// Should only be used for major quest-ending errors to help diagnose difficult issues.
        /// Will only store a finite number of exceptions before overwriting oldest.
        /// </summary>
        /// <param name="quest">Quest throwing the exception.</param>
        /// <param name="ex">Exception being thrown.</param>
        public void StoreQuestException(Quest quest, Exception ex)
        {
            const int maxExceptions = 100;

            // Remove oldest exception from list if going over max
            if (storedExceptions.Count + 1 > maxExceptions)
                storedExceptions.RemoveRange(0, 1);

            // Store newest exception
            StoredException storedException = new StoredException()
            {
                questName = quest.QuestName,
                exception = ex,
                stackTrace = ex.StackTrace,
            };
            storedExceptions.Add(storedException);
        }

        /// <summary>
        /// Gets array of stored exceptions.
        /// </summary>
        public StoredException[] GetStoredExceptions()
        {   
            return storedExceptions.ToArray();
        }

        /// <summary>
        /// Restores an array of stored exceptions.
        /// </summary>
        public void SetStoredExceptions(StoredException[] exceptions)
        {
            storedExceptions.Clear();
            storedExceptions.AddRange(exceptions);
        }

        /// <summary>
        /// Gets localized version of a quest display name.
        /// Name is cached after first read for better performance.
        /// </summary>
        /// <param name="questName">Original name of quest. Do not append -LOC.</param>
        /// <returns>Localized name of quest if found, otherwise string.Empty.</returns>
        public string GetLocalizedQuestDisplayName(string questName)
        {
            // Remove ".txt" file extension from quest name if present
            if (questName.EndsWith(fileExtension, false, CultureInfo.InvariantCulture))
                questName = questName.Substring(0, questName.Length - fileExtension.Length);

            // Return quest name if already parsed
            if (localizedQuestNames.ContainsKey(questName))
                return localizedQuestNames[questName];

            // Try to parse and return name or empty string on failure
            return ParseLocalizedQuestText(questName) ? localizedQuestNames[questName] : string.Empty;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Removes the first SiteLink found matching quest UID.
        /// </summary>
        /// <returns>True if link removed, false if no matching links found.</returns>
        bool RemoveQuestSiteLink(ulong questUID)
        {
            // Look for a site link matching this questID to remove
            for (int i = 0; i < siteLinks.Count; i++)
            {
                if (siteLinks[i].questUID == questUID)
                {
                    siteLinks.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Seeks a partial quest file from StreamingAssets/Text/Quests.
        /// This file must be called "QuestName-LOC.txt", e.g. "M0B00Y16-LOC.txt".
        /// Only header and text (QRC) parts of this file will be read. Any logic (QBN) parts will be ignored.
        /// Returning false is not a fail state. By default no localized files exist in StreamingAssets/Text/Quests.
        /// Check player log for error messages if localized quest text not displayed in-game.
        /// </summary>
        /// <param name="questName">Standard quest name, do NOT append -LOC here.</param>
        /// <returns>True if localized text was loaded, otherwise false.</returns>
        bool ParseLocalizedQuestText(string questName)
        {
            // Compose filename of localized quest
            string filename = questName;
            string fileNoExt = Path.GetFileNameWithoutExtension(filename);
            if (!fileNoExt.EndsWith(localizedFilenameSuffix))
                filename = fileNoExt + localizedFilenameSuffix + fileExtension;

            // Do nothing if localized quest has previously been parsed
            if (localizedQuestNames.ContainsKey(fileNoExt))
                return true;

            string[] lines = null;

            // Seek localized quest file from mods
            if (ModManager.Instance != null && ModManager.Instance.TryGetAsset(filename, false, out TextAsset textAsset))
            {
                if (!string.IsNullOrWhiteSpace(textAsset.text))
                {
                    lines = textAsset.text.Split('\n');
                }
            }

            if (lines == null)
            {
                // Get path to localized quest file and check it exists
                string path = Path.Combine(Application.streamingAssetsPath, textFolderName, questsFolderName, filename);
                if (!File.Exists(path))
                    return false;

                // Attempt to load file from StreamingAssets/Text/Quests
                lines = File.ReadAllLines(path);
                if (lines == null || lines.Length == 0)
                    return false;
            }

            // Parse localized quest file
            Parser parser = new Parser();
            string displayName = string.Empty;
            Dictionary<int, string> messages = null;
            try
            {
                if (!parser.ParseLocalized(new List<string>(lines), out displayName, out messages))
                    return false;
            }
            catch (Exception ex)
            {
                Debug.LogErrorFormat("Parsing localized quest `{0}` FAILED!\r\n{1}", filename, ex.Message);
            }

            // Validate output
            if (string.IsNullOrEmpty(displayName))
            {
                Debug.LogErrorFormat("Localized quest '{0}' has a null or empty DisplayName: value.", filename);
                return false;
            }
            if (messages == null || messages.Count == 0)
            {
                Debug.LogErrorFormat("Localized quest '{0}' parsed no valid messages. Check source file is a valid format.", filename);
                return false;
            }

            // Get string database
            var stringTable = LocalizationSettings.StringDatabase.GetTable(TextManager.Instance.runtimeQuestsStrings);
            if (stringTable == null)
            {
                Debug.LogErrorFormat("ParseLocalizedQuestText() failed to get string table `{0}`.", TextManager.Instance.runtimeQuestsStrings);
                return false;
            }

            // Store localized messages to Internal_Quests string table
            foreach(var item in messages)
            {
                string key = fileNoExt + "." + item.Key.ToString();
                var targetEntry = stringTable.GetEntry(key);
                if (targetEntry == null)
                    stringTable.AddEntry(key, item.Value);
            }

            // Store localized display name
            localizedQuestNames.Add(fileNoExt, displayName);

            return true;
        }

        #endregion

        #region Static Helper Methods

        /// <summary>
        /// Checks if a Place has a SiteLink available.
        /// </summary>
        public static bool HasSiteLink(Quest parentQuest, Symbol placeSymbol)
        {
            // Attempt to get Place resource
            Place place = parentQuest.GetPlace(placeSymbol);
            if (place == null)
                throw new Exception(string.Format("HasSiteLink() could not find Place symbol {0}", placeSymbol.Name));

            // Collect any SiteLinks associdated with this site
            SiteLink[] siteLinks = Instance.GetSiteLinks(place.SiteDetails.siteType, place.SiteDetails.mapId, place.SiteDetails.buildingKey, place.SiteDetails.magicNumberIndex);
            if (siteLinks == null || siteLinks.Length == 0)
                return false;

            return true;
        }

        /// <summary>
        /// Creates a new SiteLink at Place.
        /// </summary>
        public static void CreateSiteLink(Quest parentQuest, Symbol placeSymbol)
        {
            // Attempt to get Place resource
            Place place = parentQuest.GetPlace(placeSymbol);
            if (place == null)
                throw new Exception(string.Format("Attempted to add SiteLink for invalid Place symbol {0}", placeSymbol.Name));

            // Create SiteLink in QuestMachine
            SiteLink siteLink = new SiteLink();
            siteLink.questUID = parentQuest.UID;
            siteLink.placeSymbol = placeSymbol.Clone();
            siteLink.siteType = place.SiteDetails.siteType;
            siteLink.mapId = place.SiteDetails.mapId;
            siteLink.buildingKey = place.SiteDetails.buildingKey;
            siteLink.magicNumberIndex = place.SiteDetails.magicNumberIndex;
            Instance.AddSiteLink(siteLink);

            // Output debug information
            switch (siteLink.siteType)
            {
                case SiteTypes.Building:
                    LogFormat(parentQuest, "Created Building SiteLink to {0} in {1}/{2}", place.SiteDetails.buildingName, place.SiteDetails.regionName, place.SiteDetails.locationName);
                    break;
                case SiteTypes.Dungeon:
                    if (siteLink.magicNumberIndex == 0)
                        LogFormat(parentQuest, "Created Dungeon SiteLink to {0}/{1}", place.SiteDetails.regionName, place.SiteDetails.locationName);
                    else
                        LogFormat(parentQuest, "Created Dungeon SiteLink to {0}/{1}, index {2}", place.SiteDetails.regionName, place.SiteDetails.locationName, siteLink.magicNumberIndex);
                    break;
            }
        }

        #endregion

        #region Log Management

        static void ClearLog()
        {
            string text = string.Format(
                "Starting new quest log {0} {1}\r\n{2} version {3}\r\n\r\n",
                DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString(),
                VersionInfo.DaggerfallUnityProductName,
                VersionInfo.DaggerfallUnityVersion);

            File.WriteAllText(LogPath, text);
        }

        public static void Log(string text)
        {
            if (!File.Exists(LogPath))
                ClearLog();

            using (StreamWriter sw = File.AppendText(LogPath))
            {
                sw.Write(text);
                sw.Write("\r\n");
            }

            Debug.Log(text.Trim());
        }

        public static void Log(Quest quest, string text)
        {
            Log(string.Format("[{0}] {1}", quest.QuestName, text));
        }

        public static void LogFormat(string text, params object[] p)
        {
            Log(string.Format(text, p));
        }

        public static void LogFormat(Quest quest, string text, params object[] p)
        {
            Log(quest, string.Format(text, p));
        }

        #endregion

        #region Singleton

        static QuestMachine instance = null;
        public static QuestMachine Instance
        {
            get
            {
                if (instance == null)
                {
                    if (!FindQuestMachine(out instance))
                    {
                        GameObject go = new GameObject();
                        go.name = "QuestMachine";
                        instance = go.AddComponent<QuestMachine>();
                    }
                }
                return instance;
            }
        }

        public static bool HasInstance
        {
            get
            {
                return (instance != null);
            }
        }

        public static bool FindQuestMachine(out QuestMachine questMachineOut)
        {
            questMachineOut = GameObject.FindObjectOfType<QuestMachine>();
            if (questMachineOut == null)
            {
                DaggerfallUnity.LogMessage("Could not locate QuestMachine GameObject instance in scene!", true);
                return false;
            }

            return true;
        }

        private void SetupSingleton()
        {
            if (instance == null)
                instance = this;
            else if (instance != this)
            {
                if (Application.isPlaying)
                {
                    DaggerfallUnity.LogMessage("Multiple QuestMachine instances detected in scene!", true);
                    Destroy(gameObject);
                }
            }
        }

        #endregion

        #region Serialization

        [fsObject("v1")]
        public class QuestMachineData_v1
        {
            public SiteLink[] siteLinks;
            public Quest.QuestSaveData_v1[] quests;
        }

        public QuestMachineData_v1 GetSaveData()
        {
            QuestMachineData_v1 data = new QuestMachineData_v1();

            // Save SiteLinks
            data.siteLinks = siteLinks.ToArray();

            // Save Questss
            List<Quest.QuestSaveData_v1> questSaveDataList = new List<Quest.QuestSaveData_v1>();
            foreach(Quest quest in quests.Values)
            {
                questSaveDataList.Add(quest.GetSaveData());
            }
            data.quests = questSaveDataList.ToArray();

            return data;
        }

        public void RestoreSaveData(QuestMachineData_v1 data)
        {
            // Restore SiteLinks
            siteLinks = new List<SiteLink>(data.siteLinks);

            // Restore Quests
            foreach(Quest.QuestSaveData_v1 questData in data.quests)
            {
                try
                {
                    Quest quest = new Quest();
                    quest.RestoreSaveData(questData);
                    quests.Add(quest.UID, quest);
                    quest.ReassignLegacyQuestMarkers();
                }
                catch (Exception ex)
                {
                    Debug.LogWarningFormat("Failed to load quest data for '{0} [{1}]' with UID {2}. This is expected after removing a mod with custom quest actions. Exception message is '{3}'",
                        questData.displayName, questData.questName, questData.uid, ex.Message);

                    DaggerfallUI.AddHUDText(string.Format("Failed to load quest '{0} [{1}]'. This is expected if quest mod removed.", questData.displayName, questData.questName), 3);
                }
            }

            // Remove site links with no matching quest
            RemoveStaleSiteLinks();
        }

        void RemoveStaleSiteLinks()
        {
            List<SiteLink> siteLinksToRemove = new List<SiteLink>();
            if (siteLinks != null && siteLinks.Count > 0)
            {
                foreach (SiteLink link in siteLinks)
                {
                    Quest quest = GetQuest(link.questUID);
                    if (quest == null)
                        siteLinksToRemove.Add(link);
                }
            }

            // Remove any stale sitelinks pointing to quests no longer active
            if (siteLinksToRemove.Count > 0)
            {
                foreach (SiteLink link in siteLinksToRemove)
                {
                    Debug.LogWarningFormat("Removing stale SiteLink {0}. Quest UID {1} not present.", link.placeSymbol.Original, link.questUID);
                    siteLinks.Remove(link);
                }
            }
        }

        #endregion

        #region Events

        public delegate void OnRegisterCustomActionsEventHandler();
        public static event OnRegisterCustomActionsEventHandler OnRegisterCustomActions;
        protected virtual void RaiseOnRegisterCustomerActionsEvent()
        {
            if (OnRegisterCustomActions != null)
                OnRegisterCustomActions();
        }

        // OnTick
        public delegate void OnTickEventHandler();
        public static event OnTickEventHandler OnTick;
        protected virtual void RaiseOnTickEvent()
        {
            if (OnTick != null)
                OnTick();
        }

        // OnQuestStarted
        public delegate void OnQuestStartedEventHandler(Quest quest);
        public static event OnQuestStartedEventHandler OnQuestStarted;
        protected virtual void RaiseOnQuestStartedEvent(Quest quest)
        {
            if (OnQuestStarted != null)
                OnQuestStarted(quest);
        }

        // OnQuestEnded
        public delegate void OnQuestEndedEventHandler(Quest quest);
        public static event OnQuestEndedEventHandler OnQuestEnded;
        protected virtual void RaiseOnQuestEndedEvent(Quest quest)
        {
            if (OnQuestEnded != null)
                OnQuestEnded(quest);
        }

        // OnQuestErrorTermination
        public delegate void OnQuestErrorTerminationEventHandler(Quest quest);
        public static event OnQuestErrorTerminationEventHandler OnQuestErrorTermination;
        protected virtual void RaiseOnQuestErrorTerminationEvent(Quest quest)
        {
            if (OnQuestErrorTermination != null)
                OnQuestErrorTermination(quest);
        }

        #endregion
    }
}