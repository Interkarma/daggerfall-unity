// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2020 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Michael Rauter (Nystul)
// Contributors:    Numidium, Allofich, Interkarma, Ferital
// 
// Notes:
//

using UnityEngine;
using System;
using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Questing;
using System.Linq;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Game.Player;
using DaggerfallWorkshop.Game.Guilds;
using Wenzil.Console;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Game.Formulas;

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// This class manages talk topics and resulting actions/answers
    /// </summary>
    public partial class TalkManager : MonoBehaviour
    {
        #region Singleton

        static TalkManager instance = null;
        public static TalkManager Instance
        {
            get
            {
                if (instance == null && !FindTalkManager(out instance))
                {
                    GameObject go = new GameObject();
                    go.name = "TalkManager";
                    instance = go.AddComponent<TalkManager>();
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

        public static bool FindTalkManager(out TalkManager talkManagerOut)
        {
            talkManagerOut = GameObject.FindObjectOfType<TalkManager>();
            if (talkManagerOut == null)
            {
                DaggerfallUnity.LogMessage("Could not locate TalkManager GameObject instance in scene!", true);
                return false;
            }

            return true;
        }

        private void SetupSingleton()
        {
            if (instance == null)
                instance = this;
            else if (instance != this && Application.isPlaying)
            {
                DaggerfallUnity.LogMessage("Multiple TalkManager instances detected in scene!", true);
                Destroy(gameObject);
            }
        }

        #endregion

        #region Fields

        const int minNeutralReaction = 0;
        const int minLikeReaction = 10;
        const int minVeryLikeReaction = 30;

        // Changed from classic (classis uses { 10, -5, 0, 0, 0, -10, -5, -5 } )
        readonly short[] questionTypeReactionMods = { 5, 0, 0, 0, 5, 0, 0, 0 };

        /// Improvement over classic, add reaction modifiers when using Etiquette
        /// and Streetwise in relation to social groups.
        readonly short[] etiquetteReactionMods = { -10, 5, 10, 15, -15 };
        readonly short[] streetwiseReactionMods = { 10, 5, -10, -15, 15 };

        // From FALL.EXE. In classic the answers for sgroup 0 (commoners) and sgroup 1 (merchants) are reversed, and classic flips reference to these sgroups
        // during dialogue to compensate, but it's cleaner to just fix the data. It is fixed to the correct order here.
        // For Underworld social group "dislike player + don't know answer" combination, classic points to 7254 but this is empty. Using 7304 instead, which
        // seems to be where those responses actually are.
        readonly ushort[] answersToDirections =     { 7251, 7266, 7281, 7250, 7265, 7280, 7252, 7267, 7282, 7253, 7268, 7283, 7304, 7269, 7284,
                                                      7256, 7271, 7286, 7255, 7270, 7285, 7257, 7272, 7287, 7258, 7273, 7288, 7259, 7274, 7289};
        readonly ushort[] answersToNonDirections =  { 7251, 7266, 7281, 7250, 7265, 7280, 7252, 7267, 7282, 7253, 7268, 7283, 7304, 7269, 7284,
                                                      7261, 7276, 7291, 7260, 7275, 7290, 7262, 7277, 7292, 7263, 7278, 7293, 7264, 7279, 7294};

        // Greeting records extracted from FALL.EXE.
        readonly ushort[] greetings =               { 8550, 8551, 8552, 8553, 8554, 8555, 8556, 8557, 8558, 8559, 8560, 8561, 8562, 8562,
                                                      8563, 8564, 8564, 8565, 8566, 8566, 8567, 8568, 8568, 8569, 8570, 8570, 8571 };

        readonly ushort[] allowedBulletinTextIds =  { 1475, 1476, 1477, 1478, 1479, 1482, 1483 };

        const float DefaultChanceKnowsSomethingAboutWhereIs = 0.6f; // Chances unknown
        const float DefaultChanceKnowsSomethingAboutQuest = 0.8f; // Chances unknown
        const float DefaultChanceKnowsSomethingAboutOrganizationsStaticNPC = 0.8f; // Chances unknown
        const float DefaultChanceKnowsSomethingAboutOrganizationsMobileNPC = 0.8f; // Chances unknown
        const float ChanceToRevealLocationOnMap = 0.35f; // Chances unknown

        const int maxNumAnswersNpcGivesTellMeAboutOrRumors = 1; // Maximum number of answers npc gives about "tell me about" questions or rumors

        // Specifies entry type of list item in topic lists
        public enum ListItemType
        {
            Item, // A item that can be talked about
            ItemGroup, // A group containing other items
            NavigationBack // A special item to navigate out of group items ("Previous list")
        }

        public enum QuestionType
        {
            NoQuestion, // Used for list entries that are not of ListItemType item
            News, // Used for "any news" question
            WhereAmI, // Used for "Where am I?" question
            OrganizationInfo, // Used for "tell me about" -> organizations
            Work, // Used for "where is" -> "work"
            LocalBuilding, // Used for "where is" -> "location"
            Regional, // Used for "where is" -> "location" -> "regional"
            Person, // Used for "where is" -> "person"
            Thing, // Not used ("where is" -> "thing") - Not implemented in classic either
            QuestLocation, // Used for quest resources that are locations that get added to "tell me about" section
            QuestPerson, // Used for quest resources that are persons that get added to "tell me about" section
            QuestItem // Used for quest resources that are items that get added to "tell me about" section
        }

        public enum NPCKnowledgeAboutItem
        {
            NotSet,
            DoesNotKnowAboutItem,
            KnowsAboutItem
        }

        // this class is used to specify an entry in the topic lists and holds information about this specific topic/the question and other related data
        public class ListItem
        {
            public ListItemType type = ListItemType.Item; // list item can be either a normal item, a navigation item (to get to parent list) or an item group (contains list of child items)
            public string caption = "undefined"; // the caption text that is displayed for this topic in the left sub window of the talk window
            public string key = String.Empty; // the key used for entries belonging to quest resources, is String.Empty if the entry is not a quest resource
            public QuestionType questionType = QuestionType.NoQuestion; // the question type of the entry (see description of QuestionType)
            public NPCKnowledgeAboutItem npcKnowledgeAboutItem = NPCKnowledgeAboutItem.NotSet; // the knowledge of the current npc talk partner about this topic
            public int buildingKey = -1; // used for listitems that are buildings to identify buildings
            public bool npcInSameBuildingAsTopic = false; // used for listitems that are buildings to mark if npc talk partner is in very same building (and thus should always give an answer)
            public ulong questID = 0; // the questID of the quest to which this ListItem belongs to (if this ListItem belongs to a topic/question about a quest resource)
            public int index = -1; // the index of the ListItem in the topic list (e.g. 2nd entry of the topic list - every ListItem belongs to a certain topic list)
            public List<ListItem> listChildItems = null; // null if type == ListItemType.Navigation or ListItemType.Item, only contains a list if type == ListItemType.ItemGroup
            public List<ListItem> listParentItems = null; // null if type == ListItemType.ItemGroup or ListItemType.Item, only contains a list if type == ListItemType.Navigation
        }

        // current target npc for conversion
        StaticNPC targetStaticNPC = null;

        // this class holds information about an npc talk partner
        public class NPCData
        {
            public Races race;
            public FactionFile.SocialGroups socialGroup;
            public FactionFile.GuildGroups guildGroup;
            public FactionFile.FactionData factionData;
            public string npcFactionName; // kept for guild related greetings
            public string pcFactionName; // kept for guild related greetings
            public string allyFactionName; // kept for guild related greetings
            public string enemyFactionName; // kept for guild related greetings
            public float chanceKnowsSomethingAboutWhereIs; // the general chance that the current npc knows the answer to pc's "where is" question
            public float chanceKnowsSomethingAboutQuest; // the general chance that the current npc knows the answer to pc's quest related question
            public float chanceKnowsSomethingAboutOrganizations; // the general chance that the current npc knows the answer to pc's question about organizations
            public int numAnswersGivenTellMeAboutOrRumors; // the number of (successful) answers to a "tell me about" question or rumors given by the npc (answers about npc knew something)
            public bool isSpyMaster;
            public bool allowGuildResponse = true;
        }
        NPCData npcData;

        // type of npc talk partners for a conversion
        enum NPCType
        {
            Static,
            Mobile,
            Unset
        }
        MobilePersonNPC lastTargetMobileNPC = null; // the last mobile npc talk partner
        StaticNPC lastTargetStaticNPC = null; // the last static npc talk partner
        NPCType currentNPCType = NPCType.Unset; // current type of npc talk partner
        bool sameTalkTargetAsBefore = false; // used to indicate same dialog partner / talk target as in conversation before
        bool alreadyRejectedOnce = false; // used to display a random rejection text first time when talking to an npc that dislikes pc, trying to talk a 2nd time (for same npc) pc gets msg "you get no response"

        bool consoleCommandFlag_npcsKnowEverything = false; // used for console commands "npc_knowsEverything" and "npc_knowsUsual"

        public enum KeySubjectType
        {
            Unset,
            Building,
            Person,
            Thing,
            Work,
            QuestTopic,
            Organization
        }

        string nameNPC = "";
        string greetingNameNPC = ""; // used only for PC first question

        string npcGreetingText = ""; // the last NPC greeting text

        bool rebuildTopicLists = true; // flag to indicate that topic lists need to be rebuild next time talkwindow is opened

        // when an instant rebuild is forced these 4 flags indicate which topic lists need to be created
        bool instantRebuildTopicListTellMeAbout = false;
        bool instantRebuildTopicListLocation = false;
        bool instantRebuildTopicListPerson = false;
        bool instantRebuildTopicListThing = false;

        ListItem currentQuestionListItem = null; // current question list item selected on topic list
        string currentKeySubject = "";
        KeySubjectType currentKeySubjectType = KeySubjectType.Unset;
        int currentKeySubjectBuildingKey = -1; // building key of building if key subject is building, also used when person's location is determined if person is in a building
        int reactionToPlayer = 0;
        int reactionToPlayer_0_1_2 = 0;
        int[] toneReactionForTalkSession = { 0, 0, 0 };
        int lastToneIndex = -1;

        //The lists that contain the topics to select
        List<ListItem> listTopicTellMeAbout;
        List<ListItem> listTopicLocation;
        List<ListItem> listTopicPerson;
        List<ListItem> listTopicThing;

        int numQuestionsAsked = 0; // used to determine if greetings text or follow up text needs to be used (not to mix up with npcData.numAnswersGivenTellMeAboutOrRumors)
        string questionOpeningText = ""; // randomize PC opening text only once for every new question so save it in this string after creating it (so opening text does not change when picking different questions/topics)

        bool markLocationOnMap = false; // flag to guide the macrohelper (macro resolving) to either give directional hints or mark buildings on the map

        // the current selected talk tone in the talk window
        DaggerfallTalkWindow.TalkTone currentTalkTone = DaggerfallTalkWindow.TalkTone.Normal;

        // meta data for buildings used in location topic list
        struct BuildingInfo
        {
            public string name;
            public DFLocation.BuildingTypes buildingType;
            public int buildingKey;
            public Vector2 position;
        }

        // the list of buildings in the current location used for location topic list
        List<BuildingInfo> listBuildings = null;

        short[] FactionsAndBuildings = { 0x1A, 0x15, 0x1D, 0x1B, 0x23, 0x18, 0x21, 0x16, 0x19E, 0x170, 0x19D, 0x198, 0x19A, 0x19B, 0x199, 0x19F, 0x1A0,
                                          0x1A1, 0x28, 0x29, 0x0F, 0x0A, 0x0D, 0x2, 0x0, 0x3, 0x5, 0x6, 0x8 };

        public string LocationOfRegionalBuilding;

        // quest injected stuff

        // quest info resource type
        public enum QuestInfoResourceType
        {
            NotSet,
            Location,
            Person,
            Thing
        }

        // quest info answers about quest resource
        public class QuestResourceInfo
        {
            // used for quest place resources - which type of hints were given by npc about a quest place resource
            public enum BuildingLocationHintTypeGiven
            {
                None,
                ReceivedDirectionalHints,
                LocationWasMarkedOnMap
            }

            public QuestResourceInfo()
            {
                this.anyInfoAnswers = null;
                this.rumorsAnswers = null;
                this.resourceType = QuestInfoResourceType.NotSet;
                this.availableForDialog = true;
                this.hasEntryInTellMeAbout = false;
                this.questPlaceResourceHintTypeReceived = BuildingLocationHintTypeGiven.None;
                this.dialogLinkedLocations = new List<string>();
                this.dialogLinkedPersons = new List<string>();
                this.dialogLinkedThings = new List<string>();
                this.questResource = null;
            }

            public QuestInfoResourceType resourceType;
            public List<TextFile.Token[]> anyInfoAnswers;
            public List<TextFile.Token[]> rumorsAnswers;
            public bool availableForDialog; // if it will show up in talk window (any dialog link for this resource will set this false, if no dialog link is present it will be set to true)
            public bool hasEntryInTellMeAbout; // if resource will get entry in section "Tell Me About" (anyInfo or rumors available)
            public bool hasEntryInWhereIs; // if resource will get entry in section "Where Is" (e.g. person resources)
            public BuildingLocationHintTypeGiven questPlaceResourceHintTypeReceived; // used if resource is place resource - indicates if an npc gave directional hints or already marked the resource on the map
            public List<string> dialogLinkedLocations; // list of location quest resources dialog-linked to this quest resource
            public List<string> dialogLinkedPersons; // list of person quest resources dialog-linked to this quest resource
            public List<string> dialogLinkedThings; // list of thing quest resources dialog-linked to this quest resource
            public QuestResource questResource; // reference to this quest resource
        }

        // quest related resources and npcs
        public class QuestResources
        {
            // a dictionary of quest resources (key resource name, value is the QuestResourceInfo)
            public Dictionary<string, QuestResourceInfo> resourceInfo;
        }

        // dictionary of quests (key is questID, value is QuestInfo) - questInfo contains resources, dialog links, and state of pc's knowledge about it (if pc learned about it), etc.
        Dictionary<ulong, QuestResources> dictQuestInfo = new Dictionary<ulong, QuestResources>();

        public enum RumorType
        {
            CommonRumor, // generic rumor
            QuestProgressRumor, // RumorsDuringQuest, RumorsPostfailure, RumorsPostsuccess
            QuestRumorMill // "rumor mill" quest action
        }

        // rumor mill data
        public class RumorMillEntry
        {
            public RumorType rumorType;
            public List<TextFile.Token[]> listRumorVariants;
            public ulong questID; // questID used for RumorType::QuestProgressRumor and RumorType::QuestRumorMill, otherwise not set
            public ulong timeLimit; // Classic game minute after which this rumor expires
            public int faction1; // First faction ID involved
            public int faction2; // Second faction ID involved
            public int regionID; // ID of region involved.
            public int flags; // Rumor flags
            public int type; // Rumor type
            public int textID; // Text ID
        }
        // list of rumors in rumor mill
        List<RumorMillEntry> listRumorMill = new List<RumorMillEntry>();

        // questor post quest message stuff (QuestorPostsuccess, QuestorPostfailure)
        Dictionary<ulong, TextFile.Token[]> dictQuestorPostQuestMessage = new Dictionary<ulong, TextFile.Token[]>();

        public class SaveDataConversation
        {
            public Dictionary<ulong, QuestResources> dictQuestInfo;
            public List<RumorMillEntry> listRumorMill;
            public Dictionary<ulong, TextFile.Token[]> dictQuestorPostQuestMessage;
            public Dictionary<int, NpcWorkEntry> npcsWithWork;
            public Dictionary<int, bool> castleNPCsSpokenTo = new Dictionary<int, bool>();
        }

        // faction IDs for factions listed in "tell me about"
        int[] infoFactionIDs = { 42, 40, 108, 129, 306, 353, 41, 67, 82, 84, 88, 92, 94, 106, 36, 83, 85, 89, 93, 95, 99, 107, 37, 368, 408, 409, 410, 411, 413, 414, 415, 416, 417, 98 };

        // Data for NPC "work" quests in the current town
        public struct NpcWorkEntry
        {
            public StaticNPC.NPCData npc;
            public FactionFile.SocialGroups socialGroup;
            public string buildingName;
        }

        Dictionary<int, NpcWorkEntry> npcsWithWork = new Dictionary<int, NpcWorkEntry>();
        Dictionary<int, bool> castleNPCsSpokenTo = new Dictionary<int, bool>();

        // note Nystul: I changed the name from former lastExteriorEntered into exteriorUsedForQuestors to better reflect that a specific exterior was used to build questor dictionary
        //              Since there was a bug when player loaded save game where character is in interior environment and thus this flag was always zero,
        //              resulting in rebuilding of the dict on every call to GetBuildingList() - flag is now set to false after first creation (if exterior changes it is set to true again
        //              for forcing rebuilding - this was already implemented correctly)
        int exteriorUsedForQuestors;

        int selectedNpcWorkKey;

        #endregion

        #region Properties

        public string NameNPC
        {
            get { return nameNPC; }
        }

        public string GreetingNameNPC
        {
            get { return greetingNameNPC; }
        }

        public ListItem CurrentQuestionListItem
        {
            get { return currentQuestionListItem; }
        }

        public string CurrentKeySubject
        {
            get { return currentKeySubject; }
        }

        public KeySubjectType CurrentKeySubjectType
        {
            get { return currentKeySubjectType; }
        }

        public bool MarkLocationOnMap
        {
            get { return markLocationOnMap; }
        }

        public List<ListItem> ListTopicTellMeAbout
        {
            get { return listTopicTellMeAbout; }
        }

        public List<ListItem> ListTopicLocation
        {
            get { return listTopicLocation; }
        }

        public List<ListItem> ListTopicPerson
        {
            get { return listTopicPerson; }
        }

        public List<ListItem> ListTopicThings
        {
            get { return listTopicThing; }
        }

        public bool ConsoleCommandFlag_npcsKnowEverything
        {
            get { return consoleCommandFlag_npcsKnowEverything; }
            set { consoleCommandFlag_npcsKnowEverything = value; }
        }

        public string NPCGreetingText
        {
            get { return npcGreetingText; }
        }

        #endregion

        #region Unity

        void Awake()
        {
            SetupSingleton();

            // Important that transition events/delegates are created in Awake() instead of OnEnable (since exteriorAutomap gameobject is disabled when going indoors and enabled when going outdoors)
            PlayerGPS.OnMapPixelChanged += OnMapPixelChanged;
            PlayerEnterExit.OnTransitionExterior += OnTransitionToExterior;
            PlayerEnterExit.OnTransitionDungeonExterior += OnTransitionToDungeonExterior;
            PlayerEnterExit.OnTransitionDungeonInterior += OnTransitionToDungeonInterior;
            SaveLoadManager.OnLoad += OnLoadEvent;
            StartGameBehaviour.OnNewGame += OnStartGame;

            // Initialize work variables
            exteriorUsedForQuestors = 0;
            selectedNpcWorkKey = -1;
        }

        void OnDestroy()
        {
            // Important that transition events/delegates are destroyed in OnDestroy() instead of OnDisable (since exteriorAutomap gameobject is disabled when going indoors and enabled when going outdoors)
            PlayerGPS.OnMapPixelChanged -= OnMapPixelChanged;
            PlayerEnterExit.OnTransitionExterior -= OnTransitionToExterior;
            PlayerEnterExit.OnTransitionDungeonExterior -= OnTransitionToDungeonExterior;
            PlayerEnterExit.OnTransitionDungeonInterior -= OnTransitionToDungeonInterior;
            SaveLoadManager.OnLoad -= OnLoadEvent;
            StartGameBehaviour.OnNewGame -= OnStartGame;
        }

        void Start()
        {
            // Register console commands
            try
            {
                TalkConsoleCommands.RegisterCommands();
            }
            catch (Exception ex)
            {
                Debug.LogError(string.Format("Error Registering Talk Console commands: {0}", ex.Message));
            }
        }

        #endregion

        #region Public Methods

        public void ResetNPCKnowledge()
        {
            ResetNPCKnowledgeInTopicListRecursively(listTopicLocation);
            ResetNPCKnowledgeInTopicListRecursively(listTopicPerson);
            ResetNPCKnowledgeInTopicListRecursively(listTopicThing);
            ResetNPCKnowledgeInTopicListRecursively(listTopicTellMeAbout);
            rebuildTopicLists = true;
        }

        public int GetReactionToPlayer(FactionFile.FactionData factionData)
        {
            PlayerEntity player = GameManager.Instance.PlayerEntity;

            int reaction = factionData.rep + player.BiographyReactionMod + player.GetReactionMod((FactionFile.SocialGroups)factionData.sgroup);

            if (factionData.sgroup >= 0 && factionData.sgroup < player.SGroupReputations.Length)
                reaction += player.SGroupReputations[factionData.sgroup];

            return reaction;
        }

        int GetReactionToPlayer_0_1_2(QuestionType qt, FactionFile.SocialGroups npcSocialGroup)
        {
            int socialGroup = (int)npcSocialGroup;
            if (socialGroup >= 5)
                socialGroup = 1; // Merchants

            int toneModifier = 0;
            int toneIndex = DaggerfallTalkWindow.TalkToneToIndex(currentTalkTone);
            PlayerEntity player = GameManager.Instance.PlayerEntity;
            short skillValue = 0;

            if (toneIndex == 0)
            {
                skillValue = player.Skills.GetLiveSkillValue(DFCareer.Skills.Etiquette);
                toneModifier += etiquetteReactionMods[socialGroup];
                if (toneReactionForTalkSession[0] == 0)
                    player.TallySkill(DFCareer.Skills.Etiquette, 1);
            }
            else if (toneIndex == 2)
            {
                skillValue = player.Skills.GetLiveSkillValue(DFCareer.Skills.Streetwise);
                toneModifier += streetwiseReactionMods[socialGroup];
                if (toneReactionForTalkSession[2] == 0)
                    player.TallySkill(DFCareer.Skills.Streetwise, 1);
            }

            if (toneIndex != 1)
                toneModifier += Dice100.FailedRoll(skillValue) ? -10 : 5;

            // Convert question type to index to classic data
            int classicDataIndex = 2; // Using as default, as this gives no bonus or penalty.

            switch (qt)
            {
                case QuestionType.LocalBuilding:
                case QuestionType.Regional:
                    classicDataIndex = 0; // == Where is Location
                    break;
                case QuestionType.Person:
                    classicDataIndex = 1; // == Where is Person
                    break;
                case QuestionType.Thing: // Not used
                    classicDataIndex = 2; // == Where is Thing
                    break;
                case QuestionType.Work:
                    classicDataIndex = 3; // == Where is Work
                    break;
                case QuestionType.QuestLocation:
                case QuestionType.OrganizationInfo:
                    classicDataIndex = 4; // == Tell me about Location (Also sticking OrganizationInfo here. In classic I think "OrganizationInfo" might just
                                          // take whichever of location, person, item or work buttons you've last clicked on for the reaction roll.)
                    break;
                case QuestionType.QuestPerson:
                    classicDataIndex = 5; // == Tell me about Person
                    break;
                case QuestionType.QuestItem:
                    classicDataIndex = 6; // == Tell me about Thing
                    break;
                    // 7 == Tell me about Work (not used)
            }

            int reaction = player.Stats.LivePersonality / 5
               + questionTypeReactionMods[classicDataIndex]
               + toneModifier;

            // Make roll result be the same every time for a given NPC
            if (currentNPCType == NPCType.Mobile)
                DFRandom.Seed = (uint)lastTargetMobileNPC.GetHashCode();
            else if (currentNPCType == NPCType.Static)
                DFRandom.Seed = (uint)lastTargetStaticNPC.GetHashCode();

            int rollToBeat = DFRandom.random_range_inclusive(0, 20);
            if (toneReactionForTalkSession[toneIndex] != 0)
                reaction = toneReactionForTalkSession[toneIndex];
            else
                toneReactionForTalkSession[toneIndex] = reaction;

            // Store that we've done the reaction check for this tone this talk session
            lastToneIndex = toneIndex;

            if (reaction < rollToBeat)
                return 0;
            if (reaction < rollToBeat + 20) // Lowered from classic to be less difficult (classic uses +30)
                return 1;
            return 2;
        }

        // Player has clicked on a mobile talk target
        public void TalkToMobileNPC(MobilePersonNPC targetNPC)
        {
            currentNPCType = NPCType.Mobile;

            // All mobile NPCs use "People of" current region faction
            int npcFactionId = GameManager.Instance.PlayerGPS.GetPeopleOfCurrentRegion();
            FactionFile.FactionData npcFactionData;
            GameManager.Instance.PlayerEntity.FactionData.GetFactionData(npcFactionId, out npcFactionData);

            // Get reaction to player
            reactionToPlayer = GetReactionToPlayer(npcFactionData);

            sameTalkTargetAsBefore = false;
            SetTargetNPC(targetNPC, npcFactionData, ref sameTalkTargetAsBefore);

            npcData.numAnswersGivenTellMeAboutOrRumors = 0; // Important to reset this here so even if NPCs is the same as previous talk session PC will give one correct answer if NPC knows about topic (as implemented in classic)

            TalkToNpc();
        }

        // Player has clicked or static talk target or clicked the talk button inside a popup-window
        public void TalkToStaticNPC(StaticNPC targetNPC, bool menu = true, bool isSpyMaster = false)
        {
            // Populate NPC faction data
            FactionFile.FactionData npcFactionData;
            GetStaticNPCFactionData(targetNPC.Data.factionID, GameManager.Instance.PlayerEnterExit.BuildingType, out npcFactionData);

            // Check if this is a child NPC
            bool isChildNPC = targetNPC.IsChildNPC;

            IUserInterfaceManager uiManager = DaggerfallUI.UIManager;

            if (!isChildNPC && IsNpcOfferingQuest(targetNPC.Data.nameSeed))
            {
                uiManager.PushWindow(UIWindowFactory.GetInstanceWithArgs(UIWindowType.QuestOffer,
                    new object[]
                    {
                        uiManager, npcsWithWork[targetNPC.Data.nameSeed].npc,
                        npcsWithWork[targetNPC.Data.nameSeed].socialGroup, menu
                    }));
                return;
            }
            else if (!isChildNPC && IsCastleNpcOfferingQuest(targetNPC.Data.nameSeed))
            {
                uiManager.PushWindow(UIWindowFactory.GetInstanceWithArgs(UIWindowType.QuestOffer,
                    new object[] { uiManager, targetNPC.Data, (FactionFile.SocialGroups)npcFactionData.sgroup, menu }));
                return;
            }
            currentNPCType = NPCType.Static;

            // Matched to classic for NPCs that are not of type 2 (Group), 7 (Province) or 9 (Temple): use their first parent if such a parent exists
            // Change from classic: do the same for Courts, People and Individual since they have their own reputation. Moreover
            // classic already uses people and courts factions to get greetings and answers so it add more consistency
            while (npcFactionData.parent != 0 &&
                   npcFactionData.type != (int)FactionFile.FactionTypes.Group && // Avoid using the group leader faction (e.g. Mobar and "The Royal Guard")
                   npcFactionData.type != (int)FactionFile.FactionTypes.Province && // Avoid using "Daggerfall" for "Betony"
                   npcFactionData.type != (int)FactionFile.FactionTypes.Temple && // Avoid using the parent god faction
                   npcFactionData.type != (int)FactionFile.FactionTypes.People && // Avoid using the parent province faction
                   npcFactionData.type != (int)FactionFile.FactionTypes.Courts && // Avoid using the parent province faction
                   npcFactionData.type != (int)FactionFile.FactionTypes.Individual // Always use an individual faction when available
                   )
            {
                GameManager.Instance.PlayerEntity.FactionData.GetFactionData(npcFactionData.parent, out npcFactionData);
            }

            // Get reaction to player
            reactionToPlayer = GetReactionToPlayer(npcFactionData);

            sameTalkTargetAsBefore = false;
            SetTargetNPC(targetNPC, npcFactionData, ref sameTalkTargetAsBefore);

            npcData.numAnswersGivenTellMeAboutOrRumors = 0; // Important to reset this here so even if NPCs is the same as previous talk session PC will can one correct answer if NPC knows about topic (as implemented in classic)
            npcData.isSpyMaster = isSpyMaster;
            npcData.allowGuildResponse = !(!menu && isSpyMaster);   // Disables guild response for spymasters when talk is not from guild menu.

            TalkToNpc();
        }

        public void SetTargetNPC(MobilePersonNPC targetMobileNPC, FactionFile.FactionData factionData, ref bool sameTalkTargetAsBefore)
        {
            sameTalkTargetAsBefore = false;

            if (targetMobileNPC == lastTargetMobileNPC)
            {
                sameTalkTargetAsBefore = true;
                return;
            }

            alreadyRejectedOnce = false;

            DaggerfallUI.Instance.TalkWindow.SetNPCPortrait(DaggerfallTalkWindow.FacePortraitArchive.CommonFaces, targetMobileNPC.PersonFaceRecordId);

            lastTargetMobileNPC = targetMobileNPC;
            nameNPC = targetMobileNPC.NameNPC;
            DaggerfallUI.Instance.TalkWindow.UpdateNameNPC();

            npcData = new NPCData();
            npcData.socialGroup = FactionFile.SocialGroups.Commoners;
            npcData.guildGroup = FactionFile.GuildGroups.None;
            npcData.factionData = factionData;
            npcData.race = targetMobileNPC.Race;
            npcData.chanceKnowsSomethingAboutWhereIs = DefaultChanceKnowsSomethingAboutWhereIs + FormulaHelper.BonusChanceToKnowWhereIs();
            npcData.chanceKnowsSomethingAboutQuest = DefaultChanceKnowsSomethingAboutQuest;
            npcData.chanceKnowsSomethingAboutOrganizations = DefaultChanceKnowsSomethingAboutOrganizationsMobileNPC;
            npcData.isSpyMaster = false;

            AssembleTopicListPerson(); // Update "Where Is" -> "Person" list since this list may hide the questor (if talking to the questor)
        }

        public void SetTargetNPC(StaticNPC targetNPC, FactionFile.FactionData factionData, ref bool sameTalkTargetAsBefore)
        {
            sameTalkTargetAsBefore = false;

            if (targetNPC == lastTargetStaticNPC)
            {
                sameTalkTargetAsBefore = true;
                return;
            }

            alreadyRejectedOnce = false;
            targetStaticNPC = targetNPC;

            DaggerfallTalkWindow.FacePortraitArchive facePortraitArchive;
            int recordIndex;
            GetPortraitIndexFromStaticNPCBillboard(out facePortraitArchive, out recordIndex);
            DaggerfallUI.Instance.TalkWindow.SetNPCPortrait(facePortraitArchive, recordIndex);

            lastTargetStaticNPC = targetNPC;

            nameNPC = targetNPC.DisplayName;
            DaggerfallUI.Instance.TalkWindow.UpdateNameNPC();

            npcData = new NPCData();
            // Social group assignment. Matched to classic.
            npcData.socialGroup = factionData.sgroup < 5 ? (FactionFile.SocialGroups)factionData.sgroup : FactionFile.SocialGroups.Merchants;
            npcData.guildGroup = (FactionFile.GuildGroups)factionData.ggroup;
            npcData.factionData = factionData;
            npcData.race = targetNPC.Data.race;
            npcData.chanceKnowsSomethingAboutWhereIs = DefaultChanceKnowsSomethingAboutWhereIs + FormulaHelper.BonusChanceToKnowWhereIs();
            npcData.chanceKnowsSomethingAboutQuest = DefaultChanceKnowsSomethingAboutQuest;
            npcData.chanceKnowsSomethingAboutOrganizations = DefaultChanceKnowsSomethingAboutOrganizationsStaticNPC;
            npcData.isSpyMaster = false;

            AssembleTopicListPerson(); // Update "Where Is" -> "Person" list since this list may hide the questor (if talking to the questor)
        }

        public void StartNewConversation()
        {
            numQuestionsAsked = 0;
            questionOpeningText = "";
            currentQuestionListItem = null;
            if (rebuildTopicLists)
            {
                AssembleTopicLists();
                rebuildTopicLists = false;
            }
            SetupRumorMill();
        }

        /// <summary>
        /// Get a static NPC faction data from his faction ID. Handles special cases
        /// for NPCs with a faction ID equal to 0 and NPCs from generic nobility.
        /// </summary>
        /// <param name="factionId">The NPC faction ID.</param>
        /// <param name="buildingType">The NPC location building type.</param>
        /// <param name="factionData">The NPC faction data.</param>
        private void GetStaticNPCFactionData(int factionId, DFLocation.BuildingTypes buildingType, out FactionFile.FactionData factionData)
        {
            if (factionId == 0)
            {
                // Matched to classic: an NPC with a null faction id is assigned to court or people of current region
                if (buildingType == DFLocation.BuildingTypes.Palace)
                    factionId = GameManager.Instance.PlayerGPS.GetCourtOfCurrentRegion();
                else
                    factionId = GameManager.Instance.PlayerGPS.GetPeopleOfCurrentRegion();
            }
            else if (factionId == (int)FactionFile.FactionIDs.Random_Ruler ||
                     factionId == (int)FactionFile.FactionIDs.Random_Noble ||
                     factionId == (int)FactionFile.FactionIDs.Random_Knight)
            {
                // Change from classic: use "Court of" current region for Random Ruler, Random Noble
                // and Random Knight because these generic factions have no use at all
                factionId = GameManager.Instance.PlayerGPS.GetCourtOfCurrentRegion();
            }

            GameManager.Instance.PlayerEntity.FactionData.GetFactionData(factionId, out factionData);
        }

        private string GetNPCQuestGreeting()
        {
            IGuild guild = GameManager.Instance.GuildManager.GetGuild((int)GameManager.Instance.PlayerEnterExit.FactionID);

            if (currentNPCType == NPCType.Static)
            {
                foreach (KeyValuePair<ulong, TextFile.Token[]> entry in dictQuestorPostQuestMessage)
                {
                    ulong questID = entry.Key;
                    Quest quest = GameManager.Instance.QuestMachine.GetQuest(questID);
                    if (quest == null)
                        continue;
                    foreach (Person person in quest.GetAllResources(typeof(Person)))
                    {
                        if (person.IsQuestor &&
                            (GameManager.Instance.QuestMachine.IsNPCDataEqual(person.QuestorData, lastTargetStaticNPC.Data) ||
                             person.IsIndividualNPC && person.FactionData.id == lastTargetStaticNPC.Data.factionID))
                        {
                            TextFile.Token[] tokens = dictQuestorPostQuestMessage[questID];

                            // Set external context to guild if player is a member
                            if (guild.IsMember())
                                quest.ExternalMCP = guild;
                            // Expand tokens and reveal dialog-linked resources
                            QuestMacroHelper macroHelper = new QuestMacroHelper();
                            macroHelper.ExpandQuestMessage(quest, ref tokens, true);
                            return TokensToString(tokens);
                        }
                    }
                }
            }

            return string.Empty;
        }

        private int GetNPCGreetingRecord()
        {
            FactionFile.FactionData parentFactionData;
            GameManager.Instance.PlayerEntity.FactionData.GetParentGroupFaction(npcData.factionData, out parentFactionData);

            // Almost matched to classic: avoid faction related greetings for children of
            // a province. In classic, this is the case only for people or court of the
            // current region. DFU also blocks it for court related unique NPCs, to avoid
            // weird greetings like "As a member of Sentinel...".
            if (parentFactionData.type != (int)FactionFile.FactionTypes.Province)
            {
                int reputation = npcData.factionData.rep;
                int greetingIndex = GetGreetingIndex(ref reputation, parentFactionData);

                if (npcData.factionData.sgroup < 5)
                    reputation += GameManager.Instance.PlayerEntity.SGroupReputations[npcData.factionData.sgroup];

                int reaction = DFRandom.random_range_inclusive(0, 15) - 10;

                if (reputation >= reaction)
                {
                    // Improvement over classic: in classic, if greeting index is
                    // equal to 8, NPC will always meet the player with text entry
                    // 8570 i.e. "Well met, stranger.", that even if PC reputation
                    // with NPC is very good. In DFU, the player is now considered
                    // a stranger only if his reputation with NPC is of 5 or below.
                    if (npcData.factionData.rep >= 30 && greetingIndex != 8)
                        return greetings[3 * greetingIndex];
                    else if (npcData.factionData.rep <= 5 || greetingIndex != 8)
                        return greetings[1 + 3 * greetingIndex];
                }
                else
                {
                    alreadyRejectedOnce = true;
                    return greetings[2 + 3 * greetingIndex];
                }
            }

            const int dislikePlayerGreetingTextId = 7206;
            const int neutralToPlayerGreetingTextId = 7207;
            const int likePlayerGreetingTextId = 7208;
            const int veryLikePlayerGreetingTextId = 7209;

            if (reactionToPlayer >= minVeryLikeReaction)
                return veryLikePlayerGreetingTextId;
            if (reactionToPlayer >= minLikeReaction)
                return likePlayerGreetingTextId;
            if (reactionToPlayer >= minNeutralReaction)
                return neutralToPlayerGreetingTextId;
            return dislikePlayerGreetingTextId;
        }

        /// <summary>
        /// Get an NPC greeting index based on player guild affiliations and NPC group membership.
        /// </summary>
        /// <param name="reputation">Player overall reputation with NPC based on guild memberships.</param>
        /// <param name="npcGroupFaction">Faction group to which NPC belongs to.</param>
        /// <returns>The greeting index.</returns>
        private int GetGreetingIndex(ref int reputation, FactionFile.FactionData npcGroupFaction)
        {
            PersistentFactionData persistentFactionData = GameManager.Instance.PlayerEntity.FactionData;

            int greetingIndex = 8;
            List<IGuild> guildMemberships = GameManager.Instance.GuildManager.GetMemberships();
            foreach (IGuild guild in guildMemberships)
            {
                FactionFile.FactionData guildFactionData;
                persistentFactionData.GetFactionData(guild.GetFactionId(), out guildFactionData);

                // Check if NPC and PC are in the same guild
                if (npcGroupFaction.id == guildFactionData.id)
                {
                    npcData.npcFactionName = guildFactionData.name;
                    npcData.pcFactionName = guildFactionData.name;
                    return 0;
                }

                // Check if guild and NPC have the same parent or if one is parent of the other
                if ((guildFactionData.parent != 0 && npcGroupFaction.parent != 0 &&
                    guildFactionData.parent == npcGroupFaction.parent ||
                    guildFactionData.parent == npcGroupFaction.id ||
                    npcGroupFaction.parent == guildFactionData.id) &&
                    greetingIndex > 1)
                {
                    npcData.npcFactionName = npcGroupFaction.name;
                    npcData.pcFactionName = guildFactionData.name;
                    greetingIndex = 1;
                    reputation += 15;
                }

                // Check if guild and NPC are allies
                if (FactionFile.IsAlly(ref guildFactionData, ref npcGroupFaction) ||
                    FactionFile.IsAlly(ref npcGroupFaction, ref guildFactionData)
                  && greetingIndex > 2)
                {
                    npcData.npcFactionName = npcGroupFaction.name;
                    npcData.pcFactionName = guildFactionData.name;
                    reputation += 10;
                    greetingIndex = 2;
                }

                // Check if guild and NPC are enemies
                if (FactionFile.IsEnemy(ref guildFactionData, ref npcGroupFaction) ||
                    FactionFile.IsEnemy(ref npcGroupFaction, ref guildFactionData)
                  && greetingIndex > 3)
                {
                    npcData.npcFactionName = npcGroupFaction.name;
                    npcData.pcFactionName = guildFactionData.name;
                    // Fixed a bug from classic where reputation is set to 20 instead of being decreased
                    reputation -= 20;
                    greetingIndex = 3;
                }

                // Check if guild and NPC have enemies in common
                int[] guildEnemies = { guildFactionData.enemy1, guildFactionData.enemy2, guildFactionData.enemy3 };
                int[] npcEnemies = { npcGroupFaction.enemy1, npcGroupFaction.enemy2, npcGroupFaction.enemy3 };
                for (int i = 0; i < 3; ++i)
                {
                    for (int j = 0; j < 3; ++j)
                    {
                        if (guildEnemies[i] != 0 &&
                            guildEnemies[i] == npcEnemies[j] &&
                            greetingIndex > 4)
                        {
                            npcData.npcFactionName = npcGroupFaction.name;
                            npcData.pcFactionName = guildFactionData.name;
                            npcData.enemyFactionName = persistentFactionData.GetFactionName(guildEnemies[i]);
                            greetingIndex = 4;
                            reputation += 5;
                        }
                    }
                }

                // Check if guild and NPC have allies in common
                int[] guildAllies = { guildFactionData.ally1, guildFactionData.ally2, guildFactionData.ally3 };
                int[] npcAllies = { npcGroupFaction.ally1, npcGroupFaction.ally2, npcGroupFaction.ally3 };
                for (int i = 0; i < 3; ++i)
                {
                    for (int j = 0; j < 3; ++j)
                    {
                        if (guildAllies[i] != 0 &&
                            guildAllies[i] == npcAllies[j] &&
                            greetingIndex > 5)
                        {
                            npcData.npcFactionName = npcGroupFaction.name;
                            npcData.pcFactionName = guildFactionData.name;
                            npcData.allyFactionName = persistentFactionData.GetFactionName(guildAllies[i]);
                            greetingIndex = 5;
                            reputation += 5;
                        }
                    }
                }

                // Check if at least one of guild allies is an NPC enemy
                for (int i = 0; i < 3; ++i)
                {
                    FactionFile.FactionData enemy;
                    if (persistentFactionData.GetFactionData(npcEnemies[i], out enemy) &&
                        FactionFile.IsAlly(ref guildFactionData, ref enemy) &&
                        greetingIndex > 6)
                    {
                        npcData.npcFactionName = npcGroupFaction.name;
                        npcData.pcFactionName = guildFactionData.name;
                        npcData.enemyFactionName = enemy.name;
                        greetingIndex = 6;
                        reputation -= 5;
                    }
                }

                // Check if at least one of guild enemies is an NPC ally
                for (int i = 0; i < 3; ++i)
                {
                    FactionFile.FactionData ally;
                    if (persistentFactionData.GetFactionData(npcAllies[i], out ally) &&
                        FactionFile.IsEnemy(ref guildFactionData, ref ally) &&
                        greetingIndex > 7)
                    {
                        npcData.npcFactionName = npcGroupFaction.name;
                        npcData.pcFactionName = guildFactionData.name;
                        npcData.allyFactionName = ally.name;
                        greetingIndex = 7;
                        reputation -= 5;
                    }
                }
            }

            return greetingIndex;
        }

        public string GetPCGreetingText(DaggerfallTalkWindow.TalkTone talkTone)
        {
            if (reactionToPlayer <= 0)
                greetingNameNPC = ExpandRandomTextRecord(7221 + DaggerfallTalkWindow.TalkToneToIndex(talkTone));
            else
                greetingNameNPC = nameNPC;
            string greetingText = ExpandRandomTextRecord(7215 + DaggerfallTalkWindow.TalkToneToIndex(talkTone));
            greetingNameNPC = string.Empty;
            return greetingText;
        }

        public string GetPCFollowUpText(DaggerfallTalkWindow.TalkTone talkTone)
        {
            return ExpandRandomTextRecord(7218 + DaggerfallTalkWindow.TalkToneToIndex(talkTone));
        }

        public string GetPCGreetingOrFollowUpText()
        {
            if (numQuestionsAsked == 0)
                questionOpeningText = GetPCGreetingText(currentTalkTone);
            else
                questionOpeningText = GetPCFollowUpText(currentTalkTone);
            return questionOpeningText;
        }

        public string GetWorkString()
        {
            return ExpandRandomTextRecord(7211);
        }

        public string DirectionVector2DirectionHintString(Vector2 vecDirectionToTarget)
        {
            float angle = Mathf.Acos(Vector2.Dot(vecDirectionToTarget, Vector2.right) / vecDirectionToTarget.magnitude) / Mathf.PI * 180.0f;
            if (vecDirectionToTarget.y < 0)
                angle = 180.0f + (180.0f - angle);

            if ((angle >= 0.0f && angle < 22.5f) || (angle >= 337.5f && angle <= 360.0f))
                return TextManager.Instance.GetLocalizedText("east");
            else if (angle >= 22.5f && angle < 67.5f)
                return TextManager.Instance.GetLocalizedText("northeast");
            else if (angle >= 67.5f && angle < 112.5f)
                return TextManager.Instance.GetLocalizedText("north");
            else if (angle >= 112.5f && angle < 157.5f)
                return TextManager.Instance.GetLocalizedText("northwest");
            else if (angle >= 157.5f && angle < 202.5f)
                return TextManager.Instance.GetLocalizedText("west");
            else if (angle >= 202.5f && angle < 247.5f)
                return TextManager.Instance.GetLocalizedText("southwest");
            else if (angle >= 247.5f && angle < 292.5f)
                return TextManager.Instance.GetLocalizedText("south");
            else if (angle >= 292.5f && angle < 337.5f)
                return TextManager.Instance.GetLocalizedText("southeast");
            else
                return TextManager.Instance.GetLocalizedText("resolvingError");
        }

        public string GetKeySubjectLocationCompassDirection()
        {

            if (dictQuestInfo.ContainsKey(currentQuestionListItem.questID)
                && dictQuestInfo[currentQuestionListItem.questID].resourceInfo.ContainsKey(currentQuestionListItem.key)
                && dictQuestInfo[currentQuestionListItem.questID].resourceInfo[currentQuestionListItem.key].questPlaceResourceHintTypeReceived
                != QuestResourceInfo.BuildingLocationHintTypeGiven.LocationWasMarkedOnMap)
            {
                dictQuestInfo[currentQuestionListItem.questID].resourceInfo[currentQuestionListItem.key].questPlaceResourceHintTypeReceived = QuestResourceInfo.BuildingLocationHintTypeGiven.ReceivedDirectionalHints;
            }

            return GetBuildingCompassDirection(currentKeySubjectBuildingKey);
        }

        public string GetBuildingCompassDirection(int buildingKey)
        {
            BuildingInfo buildingInfoCurrentBuilding;
            BuildingInfo buildingInfoTargetBuilding = listBuildings.Find(x => x.buildingKey == buildingKey);

            // Note Nystul:
            // I reused coordinate mapping from buildings from exterior automap layout implementation here
            // So both building position as well as player position are calculated in map coordinates and compared

            Vector2 playerPos;
            if (!GameManager.Instance.IsPlayerInside)
            {
                float scale = MapsFile.WorldMapTerrainDim * MeshReader.GlobalScale;
                playerPos.x = ((GameManager.Instance.PlayerGPS.transform.position.x) % scale) / scale;
                playerPos.y = ((GameManager.Instance.PlayerGPS.transform.position.z) % scale) / scale;
                int refWidth = (int)(ExteriorAutomap.blockSizeWidth * ExteriorAutomap.numMaxBlocksX * GameManager.Instance.ExteriorAutomap.LayoutMultiplier);
                int refHeight = (int)(ExteriorAutomap.blockSizeHeight * ExteriorAutomap.numMaxBlocksY * GameManager.Instance.ExteriorAutomap.LayoutMultiplier);
                playerPos.x *= refWidth;
                playerPos.y *= refHeight;
                playerPos.x -= refWidth * 0.5f;
                playerPos.y -= refHeight * 0.5f;
            }
            else
            {
                buildingInfoCurrentBuilding = GetBuildingInfoCurrentBuildingOrPalace();
                playerPos = new Vector2(buildingInfoCurrentBuilding.position.x, buildingInfoCurrentBuilding.position.y);

                if (buildingInfoCurrentBuilding.buildingKey == buildingInfoTargetBuilding.buildingKey)
                    return TextManager.Instance.GetLocalizedText("thisPlace");
            }

            Vector2 vecDirectionToTarget = buildingInfoTargetBuilding.position - playerPos;
            return DirectionVector2DirectionHintString(vecDirectionToTarget);
        }

        public string GetLocationCompassDirection(Place questPlace)
        {
            Vector2 positionPlayer;
            Vector2 positionLocation = Vector2.zero;

            DFPosition position = new DFPosition();
            PlayerGPS playerGPS = GameManager.Instance.PlayerGPS;
            if (playerGPS)
                position = playerGPS.CurrentMapPixel;

            positionPlayer = new Vector2(position.X, position.Y);

            int region = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetPoliticIndex(position.X, position.Y) - 128;
            if (region < 0 || region >= DaggerfallUnity.Instance.ContentReader.MapFileReader.RegionCount)
                region = -1;

            DFRegion.RegionMapTable locationInfo = new DFRegion.RegionMapTable();

            DFRegion currentDFRegion = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetRegion(region);
            string name = questPlace.SiteDetails.locationName.ToLower();
            string[] locations = currentDFRegion.MapNames;
            for (int i = 0; i < locations.Length; i++)
            {
                if (locations[i].ToLower() == name) // Valid location found with exact name
                {
                    if (currentDFRegion.MapNameLookup.ContainsKey(locations[i]))
                    {
                        int index = currentDFRegion.MapNameLookup[locations[i]];
                        locationInfo = currentDFRegion.MapTable[index];
                        position = MapsFile.LongitudeLatitudeToMapPixel((int)locationInfo.Longitude, (int)locationInfo.Latitude);
                        positionLocation = new Vector2(position.X, position.Y);
                    }
                }
            }

            if (positionLocation != Vector2.zero)
            {
                Vector2 vecDirectionToTarget = positionLocation - positionPlayer;
                vecDirectionToTarget.y = -vecDirectionToTarget.y; // invert y axis
                return GameManager.Instance.TalkManager.DirectionVector2DirectionHintString(vecDirectionToTarget);
            }

            return TextManager.Instance.GetLocalizedText("resolvingError");
        }

        public void MarkKeySubjectLocationOnMap()
        {
            BuildingInfo buildingInfo = listBuildings.Find(x => x.buildingKey == currentKeySubjectBuildingKey);
            if (buildingInfo.buildingKey != 0)
            {
                if (dictQuestInfo.ContainsKey(currentQuestionListItem.questID) && dictQuestInfo[currentQuestionListItem.questID].resourceInfo.ContainsKey(currentQuestionListItem.key))
                    dictQuestInfo[currentQuestionListItem.questID].resourceInfo[currentQuestionListItem.key].questPlaceResourceHintTypeReceived = QuestResourceInfo.BuildingLocationHintTypeGiven.LocationWasMarkedOnMap;
                GameManager.Instance.PlayerGPS.DiscoverBuilding(buildingInfo.buildingKey);

                // above line could also be done with these statements:
                // Place place = (Place)dictQuestInfo[currentQuestionListItem.questID].resourceInfo[currentQuestionListItem.key].questResource;
                // GameManager.Instance.PlayerGPS.DiscoverBuilding(buildingInfo.buildingKey, place.SiteDetails.buildingName);
            }
        }

        public string GetQuestionText(ListItem listItem, DaggerfallTalkWindow.TalkTone talkTone)
        {
            int toneIndex = DaggerfallTalkWindow.TalkToneToIndex(talkTone);
            string question = "";

            currentTalkTone = talkTone;

            currentKeySubject = listItem.caption; // Set key to current caption for now (which is in case of buildings the building name)

            currentQuestionListItem = listItem;

            switch (listItem.questionType)
            {
                case QuestionType.News:
                    question = ExpandRandomTextRecord(7231 + toneIndex);
                    break;
                case QuestionType.WhereAmI:
                    question = TextManager.Instance.GetLocalizedText("WhereAmI");
                    break;
                case QuestionType.OrganizationInfo:
                    currentKeySubjectType = KeySubjectType.Organization;
                    question = ExpandRandomTextRecord(7212 + toneIndex);
                    break;
                case QuestionType.LocalBuilding:
                    currentKeySubjectType = KeySubjectType.Building;
                    currentKeySubjectBuildingKey = listItem.buildingKey;
                    question = ExpandRandomTextRecord(7225 + toneIndex);
                    break;
                case QuestionType.Person:
                    currentKeySubjectType = KeySubjectType.Person;
                    question = ExpandRandomTextRecord(7225 + toneIndex);
                    break;
                case QuestionType.Thing:
                    question = "Not implemented"; // Classic did not implement this either
                    break;
                case QuestionType.Regional:
                    currentKeySubjectType = KeySubjectType.Building;

                    // Improvement over classic. Make "Any" lower-case since it will be in the middle of a sentence.
                    currentKeySubject = currentKeySubject.Replace(TextManager.Instance.GetLocalizedText("toBeReplacedStringRegional"), TextManager.Instance.GetLocalizedText("replacementStringRegional"));

                    question = ExpandRandomTextRecord(7225 + toneIndex);
                    break;
                case QuestionType.QuestLocation:
                case QuestionType.QuestPerson:
                case QuestionType.QuestItem:
                    currentKeySubjectType = KeySubjectType.QuestTopic;
                    question = ExpandRandomTextRecord(7212 + toneIndex);
                    break;
                case QuestionType.Work:
                    currentKeySubjectType = KeySubjectType.Work;
                    question = ExpandRandomTextRecord(7212 + toneIndex);
                    break;
            }
            return question;
        }

        public string GetNewsOrRumorsForBulletinBoard()
        {
            string news = string.Empty;

            List<RumorMillEntry> validRumors = GetValidRumors(true);

            if (validRumors.Count == 0)
                return news;

            // Simply use first rumor available
            RumorMillEntry validRumor = validRumors.FirstOrDefault(x => x.rumorType == RumorType.CommonRumor);
            if (validRumor != null && validRumor.listRumorVariants != null)
            {
                TextFile.Token[] tokens = validRumor.listRumorVariants[0];
                int regionID = -1;
                FactionFile.FactionData factionData;

                if (validRumor.regionID != -1)
                    regionID = validRumor.regionID;
                else if (GameManager.Instance.PlayerEntity.FactionData.GetFactionData(validRumor.faction1, out factionData) && factionData.region != -1)
                    regionID = factionData.region;
                else if (GameManager.Instance.PlayerEntity.FactionData.GetFactionData(validRumor.faction2, out factionData) && factionData.region != -1)
                    regionID = factionData.region;
                else // Classic uses a random region in this case, but that can create odd results for the witches rumor and maybe more. Using current region.
                    regionID = GameManager.Instance.PlayerGPS.CurrentRegionIndex;

                MacroHelper.SetFactionIdsAndRegionID(validRumor.faction1, validRumor.faction2, regionID);
                MacroHelper.ExpandMacros(ref tokens, this);
                MacroHelper.SetFactionIdsAndRegionID(-1, -1, -1); // Reset again so %reg macro may resolve to current region if needed
                news = TokensToString(tokens, false);
            }

            return news;
        }

        public string GetNewsOrRumors()
        {
            const int outOfNewsRecordIndex = 1457;
            if (npcData.numAnswersGivenTellMeAboutOrRumors < maxNumAnswersNpcGivesTellMeAboutOrRumors || npcData.isSpyMaster || consoleCommandFlag_npcsKnowEverything)
            {
                string news = TextManager.Instance.GetLocalizedText("resolvingError");
                List<RumorMillEntry> validRumors = GetValidRumors();

                if (validRumors.Count == 0)
                    return ExpandRandomTextRecord(outOfNewsRecordIndex);

                int randomIndex = UnityEngine.Random.Range(0, validRumors.Count);
                RumorMillEntry entry = validRumors[randomIndex];
                if (entry.rumorType == RumorType.CommonRumor)
                {
                    if (entry.listRumorVariants != null)
                    {
                        TextFile.Token[] tokens = entry.listRumorVariants[0];
                        int regionID = -1;
                        FactionFile.FactionData factionData;

                        if (entry.regionID != -1)
                            regionID = entry.regionID;
                        else if (GameManager.Instance.PlayerEntity.FactionData.GetFactionData(entry.faction1, out factionData) && factionData.region != -1)
                            regionID = factionData.region;
                        else if (GameManager.Instance.PlayerEntity.FactionData.GetFactionData(entry.faction2, out factionData) && factionData.region != -1)
                            regionID = factionData.region;
                        else // Classic uses a random region in this case, but that can create odd results for the witches rumor and maybe more. Using current region.
                            regionID = GameManager.Instance.PlayerGPS.CurrentRegionIndex;

                        MacroHelper.SetFactionIdsAndRegionID(entry.faction1, entry.faction2, regionID);
                        MacroHelper.ExpandMacros(ref tokens, this);
                        MacroHelper.SetFactionIdsAndRegionID(-1, -1, -1); // Reset again so %reg macro may resolve to current region if needed
                        news = TokensToString(tokens, false);
                    }
                }
                else if (entry.rumorType == RumorType.QuestRumorMill || entry.rumorType == RumorType.QuestProgressRumor)
                {
                    int variant = UnityEngine.Random.Range(0, entry.listRumorVariants.Count);
                    TextFile.Token[] tokens = entry.listRumorVariants[variant];

                    // Expand tokens and reveal dialog-linked resources
                    QuestMacroHelper macroHelper = new QuestMacroHelper();
                    macroHelper.ExpandQuestMessage(GameManager.Instance.QuestMachine.GetQuest(entry.questID), ref tokens, true);
                    news = TokensToString(tokens);
                }

                npcData.numAnswersGivenTellMeAboutOrRumors++;

                return news;
            }

            return ExpandRandomTextRecord(outOfNewsRecordIndex);
        }

        private List<RumorMillEntry> GetValidRumors(bool readingSign = false)
        {
            List<RumorMillEntry> validRumors = new List<RumorMillEntry>();

            foreach (RumorMillEntry entry in listRumorMill)
            {
                if (readingSign && !allowedBulletinTextIds.Any(x => x == entry.textID))
                {
                    // skip non-bulletin texts if we are getting rumors for a bulletin board
                    continue;
                }

                if (entry.rumorType == RumorType.CommonRumor)
                {
                    // Note: Classic only checks that regionID matches for sign messages. Because of this, some rumors that seem they were supposed to
                    // show only in affected regions (crime wave, new ruler) are seen everywhere, and their regionID data goes unused.
                    // The ruler messages seem relevant for everyone to say, and are commonly seen in classic, so for DFU the regionID component is removed.
                    // Crime waves, though, should only be talked about for the affected regions.
                    if (entry.regionID != -1 && entry.regionID != GameManager.Instance.PlayerGPS.CurrentRegionIndex)
                        continue;

                    // skip sign rumors if talking
                    if (!readingSign && (entry.flags & 1) == 1)
                    {
                        continue;
                    }
                    // skip spoken rumors if reading sign
                    if (readingSign && (entry.flags & 1) != 1)
                    {
                        continue;
                    }

                    if (entry.faction1 != 0 || entry.faction2 != 0 || entry.type != 100)
                    {
                        FactionFile.FactionData factionData1;
                        FactionFile.FactionData factionData2;

                        // Flag 1 being set makes faction's rumors less likely to appear in conversation.
                        if (((entry.faction1 != 0 && GameManager.Instance.PlayerEntity.FactionData.GetFactionData(entry.faction1, out factionData1) && (factionData1.flags & 1) == 1)
                            || (entry.faction2 != 0 && GameManager.Instance.PlayerEntity.FactionData.GetFactionData(entry.faction2, out factionData2) && (factionData2.flags & 1) == 1))
                            && Dice100.SuccessRoll(75))
                        {
                            continue;
                        }
                    }
                }

                validRumors.Add(entry);
            }

            return validRumors;
        }

        public string GetAnswerWhereAmI()
        {
            // All NPCs know this answer
            if (GameManager.Instance.IsPlayerInside)
            {
                if (GameManager.Instance.PlayerEnterExit.ExteriorDoors.Length > 0) // In building
                {
                    PlayerGPS.DiscoveredBuilding discoveredBuilding;
                    if (GameManager.Instance.PlayerGPS.GetAnyBuilding(GameManager.Instance.PlayerEnterExit.ExteriorDoors[0].buildingKey, out discoveredBuilding))
                    {
                        return String.Format(TextManager.Instance.GetLocalizedText("AnswerTextWhereAmI"), discoveredBuilding.displayName, GameManager.Instance.PlayerGPS.CurrentLocation.Name);
                    }
                    // Fallback if no discovery info was found
                    BuildingInfo currentBuilding = listBuildings.Find(x => x.buildingKey == GameManager.Instance.PlayerEnterExit.ExteriorDoors[0].buildingKey);

                    return string.Format(TextManager.Instance.GetLocalizedText("AnswerTextWhereAmI"), currentBuilding.name, GameManager.Instance.PlayerGPS.CurrentLocation.Name);
                }

                if (GameManager.Instance.IsPlayerInsideCastle || GameManager.Instance.IsPlayerInsideDungeon) // In dungeon
                {
                    string dungeonName = GameManager.Instance.PlayerEnterExit.Dungeon.GetSpecialDungeonName();
                    return string.Format(TextManager.Instance.GetLocalizedText("AnswerTextWhereAmI"), dungeonName, GameManager.Instance.PlayerEnterExit.Dungeon.Summary.RegionName);
                }
            }
            else
            {
                return string.Format(TextManager.Instance.GetLocalizedText("AnswerTextWhereAmI"), GameManager.Instance.PlayerGPS.CurrentLocation.Name, GameManager.Instance.PlayerGPS.CurrentRegionName);
            }
            return TextManager.Instance.GetLocalizedText("resolvingError");
        }

        public string GetOrganizationInfo(ListItem listItem)
        {
            int index = (listItem.index > 7 ? listItem.index + 1 : listItem.index); // Note Nystul: this looks error-prone because we are assuming specific indices here -> what if this changes some day?
            return ExpandRandomTextRecord(860 + index);
        }

        public void AddQuestRumorToRumorMill(ulong questID, Message message)
        {
            if (message == null)
                throw new NullReferenceException("AddQuestRumorToRumorMill(): Message cannot be null.");

            if (listRumorMill == null || listRumorMill.Count == 0)
                SetupRumorMill();

            RumorMillEntry entry = new RumorMillEntry();
            entry.rumorType = RumorType.QuestRumorMill;
            entry.questID = questID;
            entry.listRumorVariants = new List<TextFile.Token[]>();
            for (int i = 0; i < message.VariantCount; i++)
            {
                TextFile.Token[] variantItem = message.GetTextTokensByVariant(i, false); // Do not expand macros
                entry.listRumorVariants.Add(variantItem);
            }

            listRumorMill.Add(entry);
        }

        public void AddQuestRumorToRumorMill(ulong questID, List<TextFile.Token[]> listTokens)
        {
            if (listRumorMill == null || listRumorMill.Count == 0)
                SetupRumorMill();

            if (listTokens.Count > 0)
            {
                RumorMillEntry entry = new RumorMillEntry();
                entry.rumorType = RumorType.QuestRumorMill;
                entry.questID = questID;
                entry.listRumorVariants = listTokens;

                listRumorMill.Add(entry);
            }
        }

        public void AddOrReplaceQuestProgressRumor(ulong questID, Message message)
        {
            if (message == null)
                throw new NullReferenceException("AddOrReplaceQuestProgressRumor(): Message cannot be null.");

            if (listRumorMill == null || listRumorMill.Count == 0)
                SetupRumorMill();

            int i;
            for (i = 0; i < listRumorMill.Count; i++)
            {
                if (listRumorMill[i].rumorType == RumorType.QuestProgressRumor && listRumorMill[i].questID == questID)
                {
                    break;
                }
            }

            List<TextFile.Token[]> listRumorVariants = new List<TextFile.Token[]>();
            for (int v = 0; v < message.VariantCount; v++)
            {
                TextFile.Token[] variantItem = message.GetTextTokensByVariant(v, false); // Do not expand macros
                listRumorVariants.Add(variantItem);
            }

            RumorMillEntry entry;
            if (i >= listRumorMill.Count) // No entry was found -> create new
            {
                entry = new RumorMillEntry();
                entry.rumorType = RumorType.QuestProgressRumor;
                entry.questID = questID;
                entry.listRumorVariants = listRumorVariants;
                listRumorMill.Add(entry);
            }
            else // Existing entry for questID -> replace
            {
                entry = listRumorMill[i];
                entry.listRumorVariants = listRumorVariants;
                listRumorMill[i] = entry;
            }
        }

        public void RemoveQuestRumorsFromRumorMill(ulong questID)
        {
            if (listRumorMill == null)
                return;

            int i = 0;
            while (i < listRumorMill.Count)
            {
                if (listRumorMill[i].rumorType == RumorType.QuestRumorMill &&
                    listRumorMill[i].questID == questID)
                {
                    listRumorMill.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
        }

        public void RemoveQuestProgressRumorsFromRumorMill(ulong questID)
        {
            if (listRumorMill == null)
                return;

            int i = 0;
            while (i < listRumorMill.Count)
            {
                if (listRumorMill[i].rumorType == RumorType.QuestProgressRumor &&
                    listRumorMill[i].questID == questID)
                {
                    listRumorMill.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
        }

        public void AddQuestorPostQuestMessage(ulong questID, Message message)
        {
            if (message == null)
                throw new NullReferenceException("AddQuestorPostQuestMessage(): Message cannot be null.");

            dictQuestorPostQuestMessage[questID] = message.GetTextTokens(0, false); // Do not expand macros
        }

        public void RemoveQuestorPostQuestMessage(ulong questID)
        {
            if (dictQuestorPostQuestMessage.ContainsKey(questID))
            {
                dictQuestorPostQuestMessage.Remove(questID);
            }
        }

        public string GetKeySubjectBuildingDirection()
        {
            markLocationOnMap = false; // When preprocessing messages in ExpandRandomTextRecord() do not reveal location accidentally (no %loc macro resolving)
            return ExpandRandomTextRecord(7333);
        }

        public string GetKeySubjectBuildingOnMap()
        {
            markLocationOnMap = true; // Only reveal on purpose
            string answer = ExpandRandomTextRecord(7332);
            markLocationOnMap = false; // Don't forget so future %loc macro resolving when preprocessing messages does not reveal location

            return answer;
        }

        public string GetKeySubjectBuildingHint()
        {
            string answer;

            // Decide if npc gives directional hints or marks building on map, always only give directional hints if player is inside
            float randomFloat = UnityEngine.Random.Range(0.0f, 1.0f);
            if (randomFloat > ChanceToRevealLocationOnMap || GameManager.Instance.IsPlayerInside)
            {
                answer = GetKeySubjectBuildingDirection();
            }
            else
            {
                answer = GetKeySubjectBuildingOnMap();
            }

            return answer;
        }

        public string GetKeySubjectPersonHint()
        {
            string key = currentQuestionListItem.key;

            // Override key if it is explicitly set (by quest person resources, place resources)
            if (currentQuestionListItem.key != string.Empty)
                key = currentQuestionListItem.key;

            string backupKeySubject = currentKeySubject; // Backup current key subject

            currentKeySubject = "";

            Person person;
            GetPersonResource(currentQuestionListItem.questID, key, out person);

            int buildingKey;

            if (person.IsQuestor)
            {
                buildingKey = GetPersonBuildingKey(ref person);
            }
            else
            {
                SiteDetails siteDetails = GetPersonSiteDetails(ref person);
                buildingKey = siteDetails.buildingKey;
                currentKeySubject = siteDetails.buildingName;
            }

            if (string.IsNullOrEmpty(currentKeySubject) || currentKeySubject == TextManager.Instance.GetLocalizedText("residence"))
                // Default to person home
                currentKeySubject = person.HomeBuildingName;

            currentKeySubjectBuildingKey = buildingKey;

            markLocationOnMap = true; // Only reveal on purpose
            string answer = GetKeySubjectBuildingHint();
            markLocationOnMap = false; // Don't forget so future %loc macro resolving when preprocessing messages does not reveal location

            currentKeySubject = backupKeySubject; // Restore old key subject

            return answer;
        }

        public string GetDialogHint(ListItem listItem)
        {
            if (dictQuestInfo.ContainsKey(listItem.questID) && dictQuestInfo[listItem.questID].resourceInfo.ContainsKey(listItem.key))
            {
                List<TextFile.Token[]> answers = dictQuestInfo[listItem.questID].resourceInfo[listItem.key].anyInfoAnswers;
                return GetAnswerFromTokensArray(listItem.questID, answers);
            }
            return TextManager.Instance.GetLocalizedText("resolvingError"); // Error case - should never ever occur
        }

        public string GetDialogHint2(ListItem listItem)
        {
            if (dictQuestInfo.ContainsKey(listItem.questID) && dictQuestInfo[listItem.questID].resourceInfo.ContainsKey(listItem.key))
            {
                List<TextFile.Token[]> answers;
                if (npcData.isSpyMaster) // Spymaster only gives "true" answers (anyinfo messages) also for %hnt2 (note: intended that consoleCommandFlag_npcsKnowEverything does not apply here)
                    answers = dictQuestInfo[listItem.questID].resourceInfo[listItem.key].anyInfoAnswers;
                else // Everybody else gives rumors here for %hnt2
                    answers = dictQuestInfo[listItem.questID].resourceInfo[listItem.key].rumorsAnswers;

                if (answers == null || answers.Count == 0) // If no rumors are available, fall back to anyInfoAnswers
                    answers = dictQuestInfo[listItem.questID].resourceInfo[listItem.key].anyInfoAnswers;
                return GetAnswerFromTokensArray(listItem.questID, answers);
            }
            return TextManager.Instance.GetLocalizedText("resolvingError"); // error case - should never ever occur
        }

        public string GetFactionNPCAlly()
        {
            return npcData.allyFactionName;
        }

        public string GetFactionNPCEnemy()
        {
            return npcData.enemyFactionName;
        }

        public string GetFactionNPC()
        {
            return npcData.npcFactionName;
        }

        public string GetFactionPC()
        {
            return npcData.pcFactionName;
        }

        public string GetFactionName()
        {
            if (npcData.guildGroup == FactionFile.GuildGroups.HolyOrder)
            {
                Temple temple = (Temple)GameManager.Instance.GuildManager.GetGuild(npcData.guildGroup, (int)GameManager.Instance.PlayerEnterExit.FactionID);
                MacroDataSource mcp = temple.GetMacroDataSource();
                return mcp.FactionOrderName();
            }
            return npcData.pcFactionName;
        }

        public string GetHonoric()
        {
            if (GameManager.Instance.PlayerEntity.Gender == Genders.Male)
                return TextManager.Instance.GetLocalizedText("Sir");
            else
                return TextManager.Instance.GetLocalizedText("Ma'am");
        }

        public string GetOldLeaderFateString(int index)
        {
            return TextManager.Instance.GetLocalizedText(string.Format("oldLeaderFate{0}", index));
        }

        public string GetAnswerWhereIs(ListItem listItem)
        {
            if (listItem.npcKnowledgeAboutItem == NPCKnowledgeAboutItem.NotSet)
            {
                // Decide here if npcs knows question's answer (spymaster always knows)
                float randomFloat = UnityEngine.Random.Range(0.0f, 1.0f);
                if (CheckNPCisInSameBuildingAsTopic(listItem, QuestionType.Person) || randomFloat < npcData.chanceKnowsSomethingAboutWhereIs || npcData.isSpyMaster || consoleCommandFlag_npcsKnowEverything)
                    listItem.npcKnowledgeAboutItem = NPCKnowledgeAboutItem.KnowsAboutItem;
                else
                    listItem.npcKnowledgeAboutItem = NPCKnowledgeAboutItem.DoesNotKnowAboutItem;
            }

            // Test if player is inside dungeon and retrieve dungeon name in this case
            string dungeonName = "";
            if (GameManager.Instance.IsPlayerInsideCastle || GameManager.Instance.IsPlayerInsideDungeon)
            {
                dungeonName = GameManager.Instance.PlayerEnterExit.Dungeon.GetSpecialDungeonName();
            }

            // Test if npc is asked about building and is in the same building (also for quest persons) -> then he/she should know about building
            if (listItem.questionType == QuestionType.LocalBuilding || listItem.questionType == QuestionType.QuestLocation && GameManager.Instance.IsPlayerInside)
            {
                if (GameManager.Instance.PlayerEnterExit.ExteriorDoors.Length > 0 && listItem.buildingKey == GameManager.Instance.PlayerEnterExit.ExteriorDoors[0].buildingKey ||
                    dungeonName == listItem.caption)
                {
                    listItem.npcKnowledgeAboutItem = NPCKnowledgeAboutItem.KnowsAboutItem;
                    listItem.npcInSameBuildingAsTopic = true;
                }
            }

            if (listItem.npcKnowledgeAboutItem == NPCKnowledgeAboutItem.DoesNotKnowAboutItem)
            {
                // Messages if NPC doesn't know answer to give directions
                return ExpandRandomTextRecord(answersToDirections[3 * (int)npcData.socialGroup + reactionToPlayer_0_1_2]);
            }

            // Check if npc is in same building if topic is building
            if (currentQuestionListItem.questionType == QuestionType.LocalBuilding && currentQuestionListItem.npcInSameBuildingAsTopic)
                return string.Format(TextManager.Instance.GetLocalizedText("YouAreInSameBuilding"), currentQuestionListItem.caption);

            // Check if npc is in same building as quest person when asking about quest person via "Where is"->"Person"
            if (currentQuestionListItem.questionType == QuestionType.Person)
            {
                int buildingKey;
                string buildingName = "";

                string key = currentQuestionListItem.key;
                Person person;
                GetPersonResource(currentQuestionListItem.questID, key, out person);
                if (person.IsQuestor)
                {
                    buildingKey = GetPersonBuildingKey(ref person);
                    if (buildingKey != 0)
                        buildingName = GetBuildingNameForBuildingKey(buildingKey);
                }
                else
                {
                    SiteDetails siteDetails = GetPersonSiteDetails(ref person);
                    buildingKey = siteDetails.buildingKey;
                    buildingName = siteDetails.buildingName;
                }

                // in case building name could not be resolved correctly
                if (string.IsNullOrEmpty(buildingName) || buildingName == TextManager.Instance.GetLocalizedText("residence"))
                    // Default to person home
                    buildingName = person.HomeBuildingName;

                if (GameManager.Instance.IsPlayerInside &&
                    (GameManager.Instance.PlayerEnterExit.ExteriorDoors.Length > 0 && buildingKey == GameManager.Instance.PlayerEnterExit.ExteriorDoors[0].buildingKey) ||
                    dungeonName == buildingName)
                {
                    currentQuestionListItem.npcInSameBuildingAsTopic = true;

                    if (buildingName != string.Empty)
                        return string.Format(TextManager.Instance.GetLocalizedText("NpcInSameBuilding"), currentQuestionListItem.caption, buildingName);

                    return TextManager.Instance.GetLocalizedText("resolvingError");
                }
            }

            // Messages if NPC does know answer to give directions
            return ExpandRandomTextRecord(answersToDirections[15 + 3 * (int)npcData.socialGroup + reactionToPlayer_0_1_2]);
        }

        public string GetAnswerAboutRegionalBuilding(ListItem listItem)
        {
            if (GetRegionalLocationCityName(listItem))
                return ExpandRandomTextRecord(10);
            else
                return ExpandRandomTextRecord(11);
        }

        public bool GetRegionalLocationCityName(ListItem listItem)
        {
            byte[] lookUpIndexes = { 0x0D, 0x0D, 0x0D, 0x0D, 0x0D, 0x0D, 0x0D, 0x0D, 0x19, 0x1A, 0x1B, 0x1D, 0x1E, 0x1F, 0x20,
                                     0x21, 0x22, 0x23, 0x24, 0x27, 0x00, 0x0B, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x0A };

            DFLocation location = new DFLocation();
            if (GetLocationWithRegionalBuilding(lookUpIndexes[listItem.index], FactionsAndBuildings[listItem.index], ref location))
            {
                LocationOfRegionalBuilding = location.Name;
                return true;
            }

            return false;
        }

        public bool GetLocationWithRegionalBuilding(byte index, short faction, ref DFLocation location)
        {
            PlayerGPS gps = GameManager.Instance.PlayerGPS;
            int locationsWithRegionalBuildingCount = 0;

            // Get how many locations in the region exist with the building being asked about
            for (int i = 0; i < gps.CurrentRegion.LocationCount; i++)
            {
                locationsWithRegionalBuildingCount += CheckLocationKeyForRegionalBuilding(gps.CurrentRegion.MapTable[i].Key, index, faction);
            }
            if (locationsWithRegionalBuildingCount > 0)
            {
                int locationToChoose = UnityEngine.Random.Range(0, locationsWithRegionalBuildingCount) + 1;
                // Get the location
                for (int i = 0; i < gps.CurrentRegion.LocationCount; i++)
                {
                    locationToChoose -= CheckLocationKeyForRegionalBuilding(gps.CurrentRegion.MapTable[i].Key, index, faction);
                    if (locationToChoose == 0)
                    {
                        location = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetLocation(gps.CurrentRegionIndex, i);
                        return true;
                    }
                }
                return false;
            }

            return false;
        }

        public int CheckLocationKeyForRegionalBuilding(uint key, byte index, short faction)
        {
            byte templeFlags = (byte)(key & 0xff);
            byte storeFlags = (byte)((key >> 8) & 0xff);
            byte guildFlags = (byte)((key >> 16) & 0xff);

            if (index > 0x27) // Out of range
                return 0;
            switch (index)
            {
                case 0: // Tavern
                    return storeFlags & 1;
                case 3: // Weapon Smith
                    return (byte)(storeFlags << 6) >> 7;
                case 4: // Armorer
                    return (byte)(storeFlags << 5) >> 7;
                case 5: // Alchemist
                    return (byte)(storeFlags << 4) >> 7;
                case 6: // Bank
                    return (byte)(storeFlags << 3) >> 7;
                case 7: // Bookstore
                    return (byte)(storeFlags << 2) >> 7;
                case 8: // Clothing store
                    return (byte)(storeFlags << 1) >> 7;
                case 0xA: // Gem store
                    return storeFlags >> 7;
                case 0xB: // Library
                    return guildFlags & 1;
                case 0xD: // Temples
                    switch (faction)
                    {
                        case 21: // Arkay
                            return (byte)(templeFlags << 6) >> 7;
                        case 22: // Zen
                            return templeFlags >> 7;
                        case 24: // Mara
                            return (byte)(templeFlags << 2) >> 7;
                        case 26: // Akatosh
                            return templeFlags & 1;
                        case 27: // Julianos
                            return (byte)(templeFlags << 4) >> 7;
                        case 29: // Dibella
                            return (byte)(templeFlags << 5) >> 7;
                        case 33: // Stendarr
                            return (byte)(templeFlags << 1) >> 7;
                        case 35: // Kynareth
                            return (byte)(templeFlags << 3) >> 7;
                        default:
                            return 0;
                    }
                case 0x19: // Order of the Raven
                case 0x1A: // Knights of the Dragon
                case 0x1B: // Knights of the Owl
                case 0x1D: // Order of the Candle
                case 0x1E: // Knights of the Flame
                case 0x1F: // Host of the Horn
                case 0x20: // Knights of the Rose
                case 0x21: // Knights of the Wheel
                case 0x22: // Order of the Scarab
                case 0x23: // Knights of the Hawk
                    return (byte)(guildFlags << 6) >> 7;
                case 0x24: // The Mages Guild
                    return (byte)(guildFlags << 5) >> 7;
                case 0x27: // The Fighters Guild
                    return (byte)(guildFlags << 2) >> 7;
                default:
                    return 0;
            }
        }

        public string GetAnswerText(ListItem listItem)
        {
            if (lastToneIndex != DaggerfallTalkWindow.TalkToneToIndex(currentTalkTone))
                reactionToPlayer_0_1_2 = GetReactionToPlayer_0_1_2(listItem.questionType, npcData.socialGroup);

            string answer = "";
            currentQuestionListItem = listItem;
            switch (listItem.questionType)
            {
                case QuestionType.News:
                    answer = GetNewsOrRumors();
                    break;
                case QuestionType.WhereAmI:
                    answer = GetAnswerWhereAmI();
                    break;
                case QuestionType.OrganizationInfo:
                    answer = GetAnswerTellMeAboutTopic(listItem, npcData.chanceKnowsSomethingAboutOrganizations);
                    break;
                case QuestionType.LocalBuilding:
                    answer = GetAnswerWhereIs(listItem);
                    break;
                case QuestionType.Person:
                    answer = GetAnswerWhereIs(listItem);
                    break;
                case QuestionType.Thing:
                    answer = GetAnswerWhereIs(listItem); // Never reached since there are no "where is"-type questions for things in classic
                    break;
                case QuestionType.Regional:
                    answer = GetAnswerAboutRegionalBuilding(listItem);
                    break;
                case QuestionType.QuestLocation:
                case QuestionType.QuestPerson:
                case QuestionType.QuestItem:
                    answer = GetAnswerTellMeAboutTopic(listItem, npcData.chanceKnowsSomethingAboutQuest);
                    break;
                case QuestionType.Work:
                    if (!WorkAvailable)
                    {
                        answer = ExpandRandomTextRecord(8078);
                        break;
                    }
                    if (reactionToPlayer_0_1_2 == 0)
                    {
                        answer = ExpandRandomTextRecord(8075);
                        break;
                    }
                    if (reactionToPlayer_0_1_2 == 1)
                        answer = ExpandRandomTextRecord(8076);
                    else if (reactionToPlayer_0_1_2 == 2)
                        answer = ExpandRandomTextRecord(8077);
                    SetRandomQuestor(); // Pick a random Work questor from the pool
                    break;
            }

            numQuestionsAsked++;
            questionOpeningText = ""; // Reset questionOpeningText so that it is newly created for next question
            return answer;
        }

        public string GetAnswerTellMeAboutTopic(ListItem listItem, float chanceNPCknowsSomething)
        {
            if (listItem.npcKnowledgeAboutItem == NPCKnowledgeAboutItem.NotSet)
            {
                // Decide here if npcs knows question's answer (spymaster always knows)
                float randomFloat = UnityEngine.Random.Range(0.0f, 1.0f);
                if ((CheckNPCisInSameBuildingAsTopic(listItem, QuestionType.QuestPerson) || randomFloat < chanceNPCknowsSomething || npcData.isSpyMaster || consoleCommandFlag_npcsKnowEverything) && CheckNPCcanKnowAboutTellMeAboutTopic(listItem))
                    listItem.npcKnowledgeAboutItem = NPCKnowledgeAboutItem.KnowsAboutItem;
                else
                    listItem.npcKnowledgeAboutItem = NPCKnowledgeAboutItem.DoesNotKnowAboutItem;
            }

            if (listItem.npcKnowledgeAboutItem == NPCKnowledgeAboutItem.DoesNotKnowAboutItem || (npcData.numAnswersGivenTellMeAboutOrRumors >= maxNumAnswersNpcGivesTellMeAboutOrRumors && !CheckNPCisInSameBuildingAsTopic(listItem, QuestionType.QuestPerson) && !npcData.isSpyMaster && !consoleCommandFlag_npcsKnowEverything))
            {
                // Messages if NPC doesn't know answer to non-directions question
                return ExpandRandomTextRecord(answersToNonDirections[3 * (int)npcData.socialGroup + reactionToPlayer_0_1_2]);
            }
            else
            {
                npcData.numAnswersGivenTellMeAboutOrRumors++;

                // Messages if NPC does know answer to non-directions question
                return ExpandRandomTextRecord(answersToNonDirections[15 + 3 * (int)npcData.socialGroup + reactionToPlayer_0_1_2]);
            }
        }


        public void ForceTopicListsUpdate()
        {
            AssembleTopicLists();
        }

        public void AddQuestTopicWithInfoAndRumors(Quest quest)
        {
            // Add RumorsDuringQuest rumor to rumor mill
            Message message = quest.GetMessage((int)QuestMachine.QuestMessages.RumorsDuringQuest);
            if (message != null)
                AddOrReplaceQuestProgressRumor(quest.UID, message);

            // Add topics for the places to see, people to meet and items to handle.
            foreach (QuestResource resource in quest.GetAllResources())
            {
                QuestInfoResourceType type = GetQuestInfoResourceType(resource);
                List<TextFile.Token[]> anyInfoAnswers = resource.GetMessage(resource.InfoMessageID);
                List<TextFile.Token[]> rumorsAnswers = resource.GetMessage(resource.RumorsMessageID);

                AddQuestTopicWithInfoAndRumors(quest.UID, resource, resource.Symbol.Name, type, anyInfoAnswers, rumorsAnswers);
            }
        }

        private static QuestInfoResourceType GetQuestInfoResourceType(QuestResource questResource)
        {
            QuestInfoResourceType type;
            if (questResource is Person)
                type = QuestInfoResourceType.Person;
            else if (questResource is Place)
                type = QuestInfoResourceType.Location;
            else if (questResource is Item)
                type = QuestInfoResourceType.Thing;
            else
                type = QuestInfoResourceType.NotSet;
            return type;
        }

        public void AddQuestTopicWithInfoAndRumors(ulong questID, QuestResource questResource, string resourceName, QuestInfoResourceType resourceType, List<TextFile.Token[]> anyInfoAnswers, List<TextFile.Token[]> rumorsAnswers)
        {
            QuestResources questResources;
            if (dictQuestInfo.ContainsKey(questID))
            {
                questResources = dictQuestInfo[questID];
            }
            else
            {
                questResources = new QuestResources();
                questResources.resourceInfo = new Dictionary<string, QuestResourceInfo>();
            }

            if (resourceName == null || resourceName == "")
                return;

            QuestResourceInfo questResourceInfo;
            if (questResources.resourceInfo.ContainsKey(resourceName))
            {
                questResourceInfo = questResources.resourceInfo[resourceName];
            }
            else
            {
                questResourceInfo = new QuestResourceInfo();
            }
            questResourceInfo.anyInfoAnswers = anyInfoAnswers;
            questResourceInfo.rumorsAnswers = rumorsAnswers;
            questResourceInfo.resourceType = resourceType;

            // questResourceInfo.availableForDialog = true; // already set by QuestResourceInfo constructor

            //if (resourceType == QuestInfoResourceType.Person && person.IsQuestor) // questors are always available for dialog
            //    questResourceInfo.availableForDialog = true;

            if (questResourceInfo.anyInfoAnswers != null || questResourceInfo.rumorsAnswers != null)
                questResourceInfo.hasEntryInTellMeAbout = true;

            if (resourceType == QuestInfoResourceType.Person || resourceType == QuestInfoResourceType.Location)
                questResourceInfo.hasEntryInWhereIs = true;
            else
                questResourceInfo.hasEntryInWhereIs = false;

            questResourceInfo.questResource = questResource;

            questResources.resourceInfo[resourceName] = questResourceInfo;

            dictQuestInfo[questID] = questResources;

            if (questResourceInfo.resourceType == QuestInfoResourceType.Location)
            {
                // Undiscover residences when they are a quest resource (named residence) when creating quest resource
                // Otherwise previously discovered residences will automatically show up on the automap when used in a quest            
                UndiscoverQuestResidence(questID, resourceName, questResourceInfo);
            }

            // update topic lists
            rebuildTopicLists = true;
        }

        public void DialogLinkForQuestInfoResource(ulong questID, string resourceName, QuestInfoResourceType resourceType, string linkedResourceName = null, QuestInfoResourceType linkedResourceType = QuestInfoResourceType.NotSet)
        {
            QuestResources questResources;
            if (dictQuestInfo.ContainsKey(questID))
            {
                questResources = dictQuestInfo[questID];
            }
            else
            {
                Debug.Log(string.Format("AddDialogLinkForQuestInfoResource() could not find quest with questID {0}", questID));
                return;
            }

            QuestResourceInfo questResource;
            if (questResources.resourceInfo.ContainsKey(resourceName))
            {
                questResource = questResources.resourceInfo[resourceName];
            }
            else
            {
                Debug.Log(string.Format("AddDialogLinkForQuestInfoResource() could not find a quest info resource with name {0}", resourceName));
                return;
            }

            switch (linkedResourceType)
            {
                case QuestInfoResourceType.NotSet:
                    // No linked resource specified - don't create entries in linked resource lists but proceed (leave switch statement) so flag "availableForDialog" is set to false for resource
                    break;
                case QuestInfoResourceType.Location:
                    if (!questResource.dialogLinkedLocations.Contains(linkedResourceName))
                        questResource.dialogLinkedLocations.Add(linkedResourceName);
                    break;
                case QuestInfoResourceType.Person:
                    if (!questResource.dialogLinkedPersons.Contains(linkedResourceName))
                        questResource.dialogLinkedPersons.Add(linkedResourceName);
                    break;
                case QuestInfoResourceType.Thing:
                    if (!questResource.dialogLinkedThings.Contains(linkedResourceName))
                        questResource.dialogLinkedThings.Add(linkedResourceName);
                    break;
                default:
                    Debug.Log("AddDialogLinkForQuestInfoResource(): unknown linked quest resource type");
                    return;
            }

            // "Hide" quest resource dialog entry
            questResource.availableForDialog = false;

            if (linkedResourceName != null)
            {
                // "Hide" linked quest resource dialog entry as well
                if (questResources.resourceInfo.ContainsKey(linkedResourceName))
                    questResources.resourceInfo[linkedResourceName].availableForDialog = false;
                else
                    Debug.Log("AddDialogLinkForQuestInfoResource(): linked quest resource not found");
            }

            // Update topic list
            AssembleTopiclistTellMeAbout();
        }

        public void AddDialogForQuestInfoResource(ulong questID, string resourceName, QuestInfoResourceType resourceType, bool instantRebuildTopicLists = true)
        {
            QuestResources questResources;
            if (dictQuestInfo.ContainsKey(questID))
            {
                questResources = dictQuestInfo[questID];
            }
            else
            {
                Debug.Log(string.Format("AddDialogLinkForQuestInfoResource() could not find quest with questID {0}", questID));
                return;
            }

            if (resourceName != null)
            {
                QuestResourceInfo questResourceInfo;
                if (questResources.resourceInfo.ContainsKey(resourceName))
                {
                    questResourceInfo = questResources.resourceInfo[resourceName];
                }
                else
                {
                    Debug.Log(string.Format("AddDialogLinkForQuestInfoResource() could not find a quest info resource with name {0}", resourceName));
                    return;
                }

                questResourceInfo.availableForDialog = true;

                if (questResourceInfo.hasEntryInTellMeAbout)
                {
                    instantRebuildTopicListTellMeAbout = true;
                }

                if (questResourceInfo.resourceType == QuestInfoResourceType.Location)
                {
                    instantRebuildTopicListLocation = true;
                }
                else if (questResourceInfo.resourceType == QuestInfoResourceType.Person)
                {
                    instantRebuildTopicListPerson = true;
                }
                else if (questResourceInfo.resourceType == QuestInfoResourceType.Thing)
                {
                    instantRebuildTopicListThing = true;
                }
            }

            // Update topic lists
            if (instantRebuildTopicLists)
                AssembleTopicLists(true);
            else
                rebuildTopicLists = true;
        }

        public void RemoveQuestInfoTopicsForSpecificQuest(ulong questID)
        {
            if (dictQuestInfo.ContainsKey(questID))
            {
                dictQuestInfo.Remove(questID);
            }

            // Update topic lists
            rebuildTopicLists = true;
        }

        public void GetPersonResource(ulong questID, string resourceName, out Person person)
        {
            if (!dictQuestInfo.ContainsKey(questID))
            {
                throw new Exception(string.Format("GetBuildingKeyForPersonResource(): Could not find quest with questID {0}", questID));
            }

            QuestResources questResources = dictQuestInfo[questID];

            if (resourceName == null || resourceName == "")
                throw new Exception("GetBuildingKeyForPersonResource(): No valid resourceName was provided.");

            QuestResourceInfo questResourceInfo;
            if (questResources.resourceInfo.ContainsKey(resourceName))
            {
                questResourceInfo = questResources.resourceInfo[resourceName];
            }
            else
            {
                throw new Exception(string.Format("GetBuildingKeyForPersonResource(): Could not find resource with resourceName {1} for quest with questID {0}", questID, resourceName));
            }

            if (questResourceInfo.resourceType != QuestInfoResourceType.Person)
                throw new Exception(string.Format("GetBuildingKeyForPersonResource(): Resource is not of type Person but was expected to be"));

            person = (Person)questResourceInfo.questResource;
        }

        public int GetPersonBuildingKey(ref Person person)
        {
            if (person.IsQuestor)
            {
                if (person.QuestorData.buildingKey != 0)
                {
                    return person.QuestorData.buildingKey;
                }
                else
                {
                    string homeBuildingName = person.HomeBuildingName;
                    BuildingInfo buildingInfoCurrentBuilding = listBuildings.Find(x => x.name == homeBuildingName);
                    return buildingInfoCurrentBuilding.buildingKey;
                }
            }

            Symbol assignedPlaceSymbol = person.GetAssignedPlaceSymbol();
            if (assignedPlaceSymbol == null)
                throw new Exception(string.Format("GetBuildingKeyForPersonResource(): Resource is not of type Person but was expected to be"));

            Place assignedPlace = person.ParentQuest.GetPlace(assignedPlaceSymbol);

            return assignedPlace.SiteDetails.buildingKey;
        }

        public SiteDetails GetPersonSiteDetails(ref Person person)
        {
            Symbol assignedPlaceSymbol = person.GetAssignedPlaceSymbol();
            if (assignedPlaceSymbol == null)
                throw new Exception(string.Format("GetBuildingKeyForPersonResource(): Resource is not of type Person but was expected to be"));

            Place assignedPlace = person.ParentQuest.GetPlace(assignedPlaceSymbol);

            return assignedPlace.SiteDetails;
        }


        public DFLocation.BuildingTypes GetBuildingTypeForBuildingKey(int buildingKey)
        {
            if (listBuildings == null)
                GetBuildingList();
            List<BuildingInfo> matchingBuildings = listBuildings.FindAll(x => x.buildingKey == buildingKey);
            if (matchingBuildings.Count == 0)
                throw new Exception(String.Format("GetBuildingTypeForBuildingKey(): No building with the queried key found"));
            if (matchingBuildings.Count > 1)
                throw new Exception(String.Format("GetBuildingTypeForBuildingKey(): More than one building with the queried key found"));
            return matchingBuildings[0].buildingType;
        }

        public string GetBuildingNameForBuildingKey(int buildingKey)
        {
            if (listBuildings == null)
                GetBuildingList();
            List<BuildingInfo> matchingBuildings = listBuildings.FindAll(x => x.buildingKey == buildingKey);
            if (matchingBuildings.Count == 0)
                throw new Exception(String.Format("GetBuildingNameForBuildingKey(): No building with the queried key found"));
            if (matchingBuildings.Count > 1)
                throw new Exception(String.Format("GetBuildingNameForBuildingKey(): M<ore than one building with the queried key found"));
            return matchingBuildings[0].name;
        }

        public bool IsBuildingQuestResource(int mapID, int buildingKey, ref string overrideBuildingName, ref bool pcLearnedAboutExistence, ref bool receivedDirectionalHints, ref bool locationWasMarkedOnMapByNPC)
        {
            pcLearnedAboutExistence = false;
            receivedDirectionalHints = false;
            locationWasMarkedOnMapByNPC = false;
            overrideBuildingName = string.Empty;

            foreach (ulong questID in GameManager.Instance.QuestMachine.GetAllActiveQuests())
            {
                Quest quest = GameManager.Instance.QuestMachine.GetQuest(questID);

                if (dictQuestInfo.ContainsKey(questID))
                {
                    QuestResources questInfo = dictQuestInfo[questID]; // Get questInfo containing orphaned list of quest resources

                    QuestResource[] questResources = quest.GetAllResources(typeof(Place)); // Get list of place quest resources
                    for (int i = 0; i < questResources.Length; i++)
                    {
                        Place place = (Place)questResources[i];
                        string key = place.Symbol.Name;

                        // Always ensure we are locating building key in current location, not just same building key in another location within same quest
                        if (place.SiteDetails.mapId != mapID || place.SiteDetails.buildingKey != buildingKey)
                            continue;

                        if (questInfo.resourceInfo.ContainsKey(key))
                        {
                            if (questInfo.resourceInfo[key].availableForDialog)
                                pcLearnedAboutExistence = true;

                            if (questInfo.resourceInfo[key].questPlaceResourceHintTypeReceived >= QuestResourceInfo.BuildingLocationHintTypeGiven.ReceivedDirectionalHints)
                                receivedDirectionalHints = true;

                            if (questInfo.resourceInfo[key].questPlaceResourceHintTypeReceived == QuestResourceInfo.BuildingLocationHintTypeGiven.LocationWasMarkedOnMap)
                                locationWasMarkedOnMapByNPC = true;

                            overrideBuildingName = place.SiteDetails.buildingName;

                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the conversation dictionary for save.
        /// </summary>
        public SaveDataConversation GetConversationSaveData()
        {
            SaveDataConversation saveDataConversation = new SaveDataConversation();
            saveDataConversation.dictQuestInfo = dictQuestInfo;
            saveDataConversation.listRumorMill = listRumorMill;
            saveDataConversation.dictQuestorPostQuestMessage = dictQuestorPostQuestMessage;
            saveDataConversation.npcsWithWork = npcsWithWork;
            saveDataConversation.castleNPCsSpokenTo = castleNPCsSpokenTo;
            return saveDataConversation;
        }

        /// <summary>
        /// Restores the conversation dictionary for load.
        /// </summary>
        public void RestoreConversationData(SaveDataConversation data)
        {
            if (data == null)
                data = new SaveDataConversation();

            dictQuestInfo = data.dictQuestInfo;
            if (dictQuestInfo == null)
                dictQuestInfo = new Dictionary<ulong, QuestResources>();

            ulong[] questIDs = GameManager.Instance.QuestMachine.GetAllQuests();

            // Search for orphaned entries in dictQuestInfo
            List<ulong> idsToDelete = new List<ulong>();
            foreach (KeyValuePair<ulong, QuestResources> questResourceInfo in dictQuestInfo)
            {
                if (GameManager.Instance.QuestMachine.GetQuest(questResourceInfo.Key) == null)
                {
                    idsToDelete.Add(questResourceInfo.Key);
                }
            }
            for (int i = idsToDelete.Count - 1; i >= 0; i--)
            {
                Debug.Log(string.Format("Save data contains orphaned quest info for quest with id {0}. Removing these entries...", idsToDelete[i]));
                dictQuestInfo.Remove(idsToDelete[i]);
            }

            // For each running quest
            foreach (ulong questID in questIDs)
            {
                Quest quest = GameManager.Instance.QuestMachine.GetQuest(questID);
                // Test if there exists a dictQuestInfo entry
                if (dictQuestInfo.ContainsKey(questID))
                {
                    // If yes, relink quest resources
                    QuestResources questInfo = dictQuestInfo[questID]; // Get questInfo containing orphaned list of quest resources
                    QuestResource[] questResources = quest.GetAllResources(typeof(Person)); // Get list of person quest resources
                    for (int i = 0; i < questResources.Length; i++)
                    {
                        Person person = (Person)questResources[i];

                        string key = person.Symbol.Name;

                        if (questInfo.resourceInfo.ContainsKey(key)) // If list of quest resources contains a matching entry
                        {
                            questInfo.resourceInfo[key].questResource = person; // Update (relink) it
                        }
                    }

                    questResources = quest.GetAllResources(typeof(Place)); // Get list of place quest resources                   
                    for (int i = 0; i < questResources.Length; i++)
                    {
                        Place place = (Place)questResources[i];
                        string key = place.Symbol.Name;

                        if (questInfo.resourceInfo.ContainsKey(key)) // If list of quest resources contains a matching entry
                        {
                            questInfo.resourceInfo[key].questResource = place; // Update (relink) it
                        }
                    }

                    questResources = quest.GetAllResources(typeof(Item)); // Get list of item quest resources
                    for (int i = 0; i < questResources.Length; i++)
                    {
                        Item item = (Item)questResources[i];
                        string key = item.Symbol.Name;

                        if (questInfo.resourceInfo.ContainsKey(key)) // If list of quest resources contains a matching entry
                        {
                            questInfo.resourceInfo[key].questResource = item; // Update (relink) it
                        }
                    }

                }
            }

            listRumorMill = data.listRumorMill;
            if (listRumorMill == null || listRumorMill.Count == 0)
            {
                SetupRumorMill();
            }

            // Search for orphaned entries in rumor mill
            for (int i = listRumorMill.Count - 1; i >= 0; i--)
            {
                if (listRumorMill[i].rumorType == RumorType.QuestRumorMill || listRumorMill[i].rumorType == RumorType.QuestProgressRumor)
                {
                    ulong questID = listRumorMill[i].questID;
                    if (GameManager.Instance.QuestMachine.GetQuest(questID) == null)
                    {
                        Debug.Log(string.Format("Save data contains orphaned rumors for quest with id {0}. Removing these rumors...", questID));
                        listRumorMill.Remove(listRumorMill[i]);
                    }
                }
            }

            dictQuestorPostQuestMessage = data.dictQuestorPostQuestMessage;
            if (dictQuestorPostQuestMessage == null)
            {
                dictQuestorPostQuestMessage = new Dictionary<ulong, TextFile.Token[]>();
            }

            if (data.npcsWithWork != null)
                npcsWithWork = data.npcsWithWork;

            if (data.castleNPCsSpokenTo != null)
                castleNPCsSpokenTo = data.castleNPCsSpokenTo;

            // Update topic list
            AssembleTopiclistTellMeAbout();
        }

        public bool IsNpcOfferingQuest(int nameSeed)
        {
            return npcsWithWork.ContainsKey(nameSeed) && !QuestMachine.Instance.IsLastNPCClickedAnActiveQuestor();
        }

        public bool IsCastleNpcOfferingQuest(int nameSeed)
        {
            if (!GameManager.Instance.IsPlayerInsideCastle || castleNPCsSpokenTo.ContainsKey(nameSeed) || QuestMachine.Instance.IsLastNPCClickedAnActiveQuestor())
            {
                return false;
            }
            // 25% chance that a castle NPC is offering a quest
            // TODO: Determine probability in classic.
            int rand = UnityEngine.Random.Range(0, 4);
            bool result = false;
            if (rand == 0)
            {
                result = true;
            }
            castleNPCsSpokenTo.Add(nameSeed, true); // Don't offer more than one quest at a time.
            Debug.Log("TalkManager: Added in-castle potential questor to list. Key: " + nameSeed + " Pool Size: " + castleNPCsSpokenTo.Keys.Count);
            return result;
        }

        public void SetRandomQuestor()
        {
            if (!WorkAvailable)
            {
                return;
            }
            System.Random rand = new System.Random();
            selectedNpcWorkKey = npcsWithWork.Keys.ToList()[rand.Next(npcsWithWork.Count)];
            Debug.Log("TalkManager: Picked random questor. Key: " + selectedNpcWorkKey + ", Pool Size: " + npcsWithWork.Keys.Count);
        }

        public string GetQuestorName()
        {
            DFRandom.srand(npcsWithWork[selectedNpcWorkKey].npc.nameSeed);
            return DaggerfallUnity.Instance.NameHelper.FullName(npcsWithWork[selectedNpcWorkKey].npc.nameBank, npcsWithWork[selectedNpcWorkKey].npc.gender);
        }

        public Genders GetQuestorGender()
        {
            return npcsWithWork[selectedNpcWorkKey].npc.gender;
        }

        public string GetQuestorLocation()
        {
            return npcsWithWork[selectedNpcWorkKey].buildingName;
        }

        public void RemoveNpcQuestor(int nameSeed)
        {
            npcsWithWork.Remove(nameSeed);
        }

        public bool WorkAvailable
        {
            get { return npcsWithWork.Count != 0; }
        }

        #endregion

        #region Private Methods

        private void TalkToNpc()
        {
            const int youGetNoResponseTextId = 7205;

            // Racial override can suppress talk
            // Also doing suppression here to prevent talk window flashing open and closed in some cases
            string suppressTalkMessage = string.Empty;
            MagicAndEffects.MagicEffects.RacialOverrideEffect racialOverride = GameManager.Instance.PlayerEffectManager.GetRacialOverrideEffect();
            if (racialOverride != null && racialOverride.GetSuppressTalk(out suppressTalkMessage))
            {
                DaggerfallUI.MessageBox(suppressTalkMessage);
                return;
            }

            if (reactionToPlayer < -20 || alreadyRejectedOnce)
            {
                DaggerfallUI.MessageBox(youGetNoResponseTextId);
                return;
            }

            // Get a quest greeting if any
            npcGreetingText = GetNPCQuestGreeting();
            if (string.IsNullOrEmpty(npcGreetingText))
            {
                int npcGreetingRecord = GetNPCGreetingRecord();
                if (alreadyRejectedOnce)
                {
                    TextFile.Token[] responseTokens =
                        DaggerfallUnity.Instance.TextProvider.GetRandomTokens(npcGreetingRecord);
                    DaggerfallUI.MessageBox(responseTokens);
                    return;
                }

                npcGreetingText = ExpandRandomTextRecord(npcGreetingRecord);
            }

            // Reset NPC knowledge, for now it resets every time the NPC has changed (player talked to new NPC)
            // TODO: Match classic daggerfall - in classic NPC remembers their knowledge about topics for their time of existence
            if (!sameTalkTargetAsBefore)
                ResetNPCKnowledge();

            DaggerfallUI.UIManager.PushWindow(DaggerfallUI.Instance.TalkWindow);

            // Reset last checked tone index for NPC reaction and tone results
            lastToneIndex = -1;
            toneReactionForTalkSession[0] = 0;
            toneReactionForTalkSession[1] = 0;
            toneReactionForTalkSession[2] = 0;
        }

        public void ImportClassicRumor(RumorFile.DaggerfallRumor rumor)
        {
            if (listRumorMill == null)
                listRumorMill = new List<RumorMillEntry>();

            RumorMillEntry entry = new RumorMillEntry();

            TextFile.Token[] tokens = TextFile.ReadTokens(ref rumor.RumorText, 0, TextFile.Formatting.EndOfRecord);

            if ((rumor.Flags & 4) != 0) // A quest rumor, don't import for now
                return;

            if ((rumor.Flags & 1) != 0) // A sign message, don't import for now
                return;

            if (rumor.NPCID != 0) // A post-quest greeting specific to a particular NPC. Don't import for now.
                return;

            entry.rumorType = RumorType.CommonRumor;
            entry.listRumorVariants = new List<TextFile.Token[]>();
            entry.listRumorVariants.Add(tokens);
            entry.faction1 = rumor.Faction1;
            entry.faction2 = rumor.Faction2;
            entry.type = (int)rumor.Type;
            entry.regionID = rumor.RegionID;
            entry.flags = rumor.Flags;
            entry.timeLimit = rumor.TimeLimit;
            listRumorMill.Add(entry);
        }

        public void AddNonQuestRumor(int faction1, int faction2, int regionID, int type, int textId)
        {
            if (listRumorMill == null)
                listRumorMill = new List<RumorMillEntry>();

            var tokens = DaggerfallUnity.Instance.TextProvider.GetRandomTokens(textId);

            RumorMillEntry entry = new RumorMillEntry
            {
                rumorType = RumorType.CommonRumor,
                listRumorVariants = new List<TextFile.Token[]> { tokens },
                faction1 = faction1,
                faction2 = faction2,
                type = type,
                regionID = regionID,
                flags = GetFlagsForNewRumor(type),
                timeLimit = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime() + 43140,
                textID = textId
            };
            listRumorMill.Add(entry);
        }

        private int GetFlagsForNewRumor(int type)
        {
            switch (type)
            {
                case 10: // Witch burnings
                case 18: // Persecuted temple
                case 7:  // Famine
                case 4:  // Plague
                case 28: // War started
                case 27: // Enemy faction
                case 26: // Alliance started
                    return 1; // Sign message

                default:
                    return 8; // Spoken rumor
            }
        }

        public void RefreshRumorMill()
        {
            if (listRumorMill == null)
                listRumorMill = new List<RumorMillEntry>();

            uint nowClassic = DaggerfallUnity.Instance.WorldTime.Now.ToClassicDaggerfallTime();

            listRumorMill.RemoveAll(x => x.timeLimit < nowClassic);
        }

        private void SetupRumorMill()
        {
            if (listRumorMill == null)
                listRumorMill = new List<RumorMillEntry>();
        }

        // Creates building list and "any work" questors
        private void GetBuildingList()
        {
            listBuildings = new List<BuildingInfo>();

            ContentReader.MapSummary mapSummary;
            DFPosition mapPixel = GameManager.Instance.PlayerGPS.CurrentMapPixel;
            if (!DaggerfallUnity.Instance.ContentReader.HasLocation(mapPixel.X, mapPixel.Y, out mapSummary))
            {
                // No location found
                return; // Do nothing
            }

            DFLocation location = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetLocation(mapSummary.RegionIndex, mapSummary.MapIndex);
            if (!location.Loaded)
            {
                // Location not loaded, something went wrong
                DaggerfallUnity.LogMessage("Error when loading location in TalkManager.GetBuildingList", true);
            }

            ExteriorAutomap.BlockLayout[] blockLayout = GameManager.Instance.ExteriorAutomap.ExteriorLayout;

            DFBlock[] blocks;
            RMBLayout.GetLocationBuildingData(location, out blocks);
            int width = location.Exterior.ExteriorData.Width;
            int height = location.Exterior.ExteriorData.Height;
            bool populateQuestors = false;
            if (exteriorUsedForQuestors != GameManager.Instance.PlayerGPS.CurrentLocationIndex)
            {
                npcsWithWork.Clear();
                populateQuestors = true;
            }

            int[] workStats = new int[12];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = y * width + x;
                    BuildingSummary[] buildingsInBlock = RMBLayout.GetBuildingData(blocks[index], x, y);

                    for (int i = 0; i < buildingsInBlock.Length; i++)
                    {
                        BuildingSummary buildingSummary = buildingsInBlock[i];
                        try
                        {
                            BuildingInfo item;
                            item.buildingType = buildingSummary.BuildingType;
                            item.name = BuildingNames.GetName(buildingSummary.NameSeed, buildingSummary.BuildingType, buildingSummary.FactionId, location.Name, location.RegionName);
                            item.buildingKey = buildingSummary.buildingKey;
                            // Compute building position in map coordinate system
                            float xPosBuilding = blockLayout[index].rect.xpos + (int)(buildingSummary.Position.x / (BlocksFile.RMBDimension * MeshReader.GlobalScale) * ExteriorAutomap.blockSizeWidth) - GameManager.Instance.ExteriorAutomap.LocationWidth * ExteriorAutomap.blockSizeWidth * 0.5f;
                            float yPosBuilding = blockLayout[index].rect.ypos + (int)(buildingSummary.Position.z / (BlocksFile.RMBDimension * MeshReader.GlobalScale) * ExteriorAutomap.blockSizeHeight) - GameManager.Instance.ExteriorAutomap.LocationHeight * ExteriorAutomap.blockSizeHeight * 0.5f;
                            item.position = new Vector2(xPosBuilding, yPosBuilding);
                            if (item.buildingKey != 0)
                                listBuildings.Add(item);
                        }
                        catch (Exception e)
                        {
                            string exceptionMessage = string.Format("exception occured in function BuildingNames.GetName (exception message: " + e.Message + @") with params: 
                                                                        seed: {0}, type: {1}, factionID: {2}, locationName: {3}, regionName: {4}",
                                                                        buildingSummary.NameSeed, buildingSummary.BuildingType, buildingSummary.FactionId, location.Name, location.RegionName);
                            DaggerfallUnity.LogMessage(exceptionMessage, true);
                        }

                        // Populate potential questors in this building
                        if (populateQuestors)
                        {
                            PersistentFactionData factions = GameManager.Instance.PlayerEntity.FactionData;
                            DFBlock.RmbBlockPeopleRecord[] buildingNpcs = blocks[index].RmbBlock.SubRecords[i].Interior.BlockPeopleRecords;
                            for (int p = 0; p < buildingNpcs.Length; p++)
                            {
                                FactionFile.FactionData factionData;
                                GetStaticNPCFactionData(buildingNpcs[p].FactionID, buildingSummary.BuildingType, out factionData);

                                FactionFile.SocialGroups socialGroup = (FactionFile.SocialGroups)factionData.sgroup;
                                if (socialGroup == FactionFile.SocialGroups.Merchants ||
                                    socialGroup == FactionFile.SocialGroups.Commoners ||
                                    socialGroup == FactionFile.SocialGroups.Nobility)
                                {
                                    // 25% chance that an NPC will offer a quest.
                                    // TODO: how does classic decide?
                                    int randomChance = UnityEngine.Random.Range(0, 4);
                                    workStats[(int)socialGroup]++;
                                    if (randomChance < 3)
                                        continue;

                                    StaticNPC.NPCData npcData2 = new StaticNPC.NPCData();
                                    StaticNPC.SetLayoutData(ref npcData2,
                                                            buildingNpcs[p].XPos, buildingNpcs[p].YPos, buildingNpcs[p].ZPos,
                                                            buildingNpcs[p].Flags,
                                                            factionData.id,
                                                            buildingNpcs[p].TextureArchive,
                                                            buildingNpcs[p].TextureRecord,
                                                            buildingNpcs[p].Position,
                                                            buildingSummary.buildingKey);

                                    // Exclude children from NPCs with work
                                    if (StaticNPC.IsChildNPCData(npcData2))
                                        continue;

                                    if (!npcsWithWork.ContainsKey(npcData2.nameSeed))
                                    {
                                        npcData2.buildingKey = buildingSummary.buildingKey;
                                        npcData2.nameBank = GameManager.Instance.PlayerGPS.GetNameBankOfCurrentRegion();
                                        NpcWorkEntry npcWork = new NpcWorkEntry
                                        {
                                            npc = npcData2,
                                            socialGroup = socialGroup,
                                            buildingName = BuildingNames.GetName(buildingSummary.NameSeed, buildingSummary.BuildingType, buildingSummary.FactionId, location.Name, location.RegionName)
                                        };

                                        if (!RMBLayout.IsNamedBuilding(buildingSummary.BuildingType))
                                        {
                                            workStats[(int)socialGroup + 8]++;
                                            continue;
                                        }

                                        workStats[(int)socialGroup + 4]++;
                                        npcsWithWork.Add(npcData2.nameSeed, npcWork);
                                        selectedNpcWorkKey = npcData2.nameSeed;
                                        Debug.LogFormat("Added {4} questor: ns={0} bk={1} name={2} building={3} factionId={5}", npcData2.nameSeed, buildingSummary.buildingKey, GetQuestorName(), npcWork.buildingName, socialGroup, npcData2.factionID);
                                    }
                                }
                            }
                            exteriorUsedForQuestors = GameManager.Instance.PlayerGPS.CurrentLocationIndex; // Create only once for current location
                        }
                    }
                }
            }
            if (populateQuestors)
                Debug.LogFormat("Populated work available from NPCs. (allocated+noBuilding/candidates) Merchants: {0}(+{6})/{1}, Commoners: {2}(+{7})/{3}, Nobles: {4}(+{8})/{5}",
                    workStats[5], workStats[1], workStats[4], workStats[0], workStats[7], workStats[3], workStats[9], workStats[8], workStats[11]);
        }

        private string BuildingTypeToGroupString(DFLocation.BuildingTypes buildingType)
        {
            switch (buildingType)
            {
                case DFLocation.BuildingTypes.Alchemist:
                    return TextManager.Instance.GetLocalizedText("Alchemists");
                case DFLocation.BuildingTypes.Armorer:
                    return TextManager.Instance.GetLocalizedText("Armorers");
                case DFLocation.BuildingTypes.Bank:
                    return TextManager.Instance.GetLocalizedText("Banks");
                case DFLocation.BuildingTypes.Bookseller:
                    return TextManager.Instance.GetLocalizedText("Bookstores");
                case DFLocation.BuildingTypes.ClothingStore:
                    return TextManager.Instance.GetLocalizedText("Clothingstores");
                case DFLocation.BuildingTypes.GemStore:
                    return TextManager.Instance.GetLocalizedText("Gemstores");
                case DFLocation.BuildingTypes.GeneralStore:
                    return TextManager.Instance.GetLocalizedText("Generalstores");
                case DFLocation.BuildingTypes.GuildHall:
                    return TextManager.Instance.GetLocalizedText("Guilds");
                case DFLocation.BuildingTypes.Library:
                    return TextManager.Instance.GetLocalizedText("Libraries");
                case DFLocation.BuildingTypes.PawnShop:
                    return TextManager.Instance.GetLocalizedText("Pawnshops");
                case DFLocation.BuildingTypes.Tavern:
                    return TextManager.Instance.GetLocalizedText("Taverns");
                case DFLocation.BuildingTypes.WeaponSmith:
                    return TextManager.Instance.GetLocalizedText("Weaponsmiths");
                case DFLocation.BuildingTypes.Temple:
                    return TextManager.Instance.GetLocalizedText("Localtemples");
                default:
                    return "";
            }
        }

        private bool CheckBuildingTypeInSkipList(DFLocation.BuildingTypes buildingType)
        {
            return buildingType == DFLocation.BuildingTypes.AllValid ||
                buildingType == DFLocation.BuildingTypes.FurnitureStore ||
                buildingType == DFLocation.BuildingTypes.House1 ||
                buildingType == DFLocation.BuildingTypes.House2 ||
                buildingType == DFLocation.BuildingTypes.House3 ||
                buildingType == DFLocation.BuildingTypes.House4 ||
                buildingType == DFLocation.BuildingTypes.House5 ||
                buildingType == DFLocation.BuildingTypes.House6 ||
                buildingType == DFLocation.BuildingTypes.HouseForSale ||
                buildingType == DFLocation.BuildingTypes.Palace ||
                buildingType == DFLocation.BuildingTypes.Ship ||
                buildingType == DFLocation.BuildingTypes.Special1 ||
                buildingType == DFLocation.BuildingTypes.Special2 ||
                buildingType == DFLocation.BuildingTypes.Special3 ||
                buildingType == DFLocation.BuildingTypes.Special4 ||
                buildingType == DFLocation.BuildingTypes.Town23 ||
                buildingType == DFLocation.BuildingTypes.Town4;
        }

        private void UndiscoverQuestResidence(ulong questID, string resourceName, QuestResourceInfo questResourceInfo)
        {
            Quest quest = GameManager.Instance.QuestMachine.GetQuest(questID);
            if (quest == null)
                return;

            if (questResourceInfo.resourceType == QuestInfoResourceType.Location)
            {
                QuestResource[] allQuestResources = quest.GetAllResources(typeof(Place)); // Get list of place quest resources
                for (int i = 0; i < allQuestResources.Length; i++)
                {
                    Place place = (Place)allQuestResources[i];
                    int buildingKey = place.SiteDetails.buildingKey;
                    string symbolName = place.Symbol.Name;

                    if (symbolName != resourceName)
                        continue;

                    GameManager.Instance.PlayerGPS.UndiscoverBuilding(buildingKey, true, place.SiteDetails.buildingName);
                }
            }
        }

        private void ResetNPCKnowledgeInTopicListRecursively(List<ListItem> list)
        {
            if (list == null)
                return;

            for (int i = 0; i < list.Count; i++)
            {
                list[i].npcKnowledgeAboutItem = NPCKnowledgeAboutItem.NotSet;
                list[i].npcInSameBuildingAsTopic = false;
                if (list[i].type == ListItemType.ItemGroup && list[i].listChildItems != null)
                    ResetNPCKnowledgeInTopicListRecursively(list[i].listChildItems);
            }
        }

        private BuildingInfo GetBuildingInfoCurrentBuildingOrPalace()
        {
            BuildingInfo buildingInfoCurrentBuilding;
            if (GameManager.Instance.IsPlayerInsideBuilding)
            {
                buildingInfoCurrentBuilding = listBuildings.Find(x => x.buildingKey == GameManager.Instance.PlayerEnterExit.Interior.EntryDoor.buildingKey);
            }
            else
            {
                // note Nystul :
                // resolving is not optimal here but it works - when not inside building but instead castle it will resolve via building type
                // since there is only one castle per location this finds the castle (a better way would be to have the building key of the palace entered,
                // but I could not find an easy way to determine building key of castle (PlayerGPS and PlayerEnterExit do not provide this, nor do other classes))                    
                buildingInfoCurrentBuilding = listBuildings.Find(x => x.buildingType == DFLocation.BuildingTypes.Palace);
            }
            return buildingInfoCurrentBuilding;
        }

        private bool CheckNPCcanKnowAboutTellMeAboutTopic(ListItem item)
        {
            Quest quest = GameManager.Instance.QuestMachine.GetQuest(item.questID);

            if (item.questionType == QuestionType.QuestLocation)
            {
                QuestResource questResource = quest.GetResource(item.key);
                Place place = (Place)questResource;
                if (place.SiteDetails.regionName != GameManager.Instance.PlayerGPS.CurrentRegionName)
                    return false;
            }
            else if (item.questionType == QuestionType.QuestPerson)
            {
                QuestResource questResource = quest.GetResource(item.key);
                Person person = (Person)questResource;
                if (person.HomeRegionName != GameManager.Instance.PlayerGPS.CurrentRegionName)
                    return false;
            }

            return true;
        }

        private bool CheckNPCisInSameBuildingAsTopic(ListItem item, QuestionType questionType)
        {
            if (item.questionType != questionType)
                return false;

            if (!GameManager.Instance.IsPlayerInside) // only if player is inside it can be a building/palace person of interest is in
                return false;

            Quest quest = GameManager.Instance.QuestMachine.GetQuest(item.questID);
            QuestResource questResource = quest.GetResource(item.key);
            Person person = (Person)questResource;
            Symbol assignedPlaceSymbol = person.GetAssignedPlaceSymbol();
            Place place;

            if (assignedPlaceSymbol != null)
            {
                place = quest.GetPlace(assignedPlaceSymbol);  // Gets actual place resource
            }
            else
            {
                place = person.GetHomePlace(); // get home place if no assigned place was found
            }

            BuildingInfo buildingInfoCurrentBuilding = GetBuildingInfoCurrentBuildingOrPalace();

            if (place.SiteDetails.regionName != GameManager.Instance.PlayerGPS.CurrentRegionName)
                return false;

            if (place.SiteDetails.locationName != GameManager.Instance.PlayerGPS.CurrentLocation.Name)
                return false;

            if (place.SiteDetails.buildingKey != 0) // building key can be 0 for palaces (so only use building key if != 0)
            {
                if (place.SiteDetails.buildingKey != buildingInfoCurrentBuilding.buildingKey)
                    return false;
            }
            else // otherwise use building name
            {
                if (place.SiteDetails.buildingName != buildingInfoCurrentBuilding.name)
                    return false;
            }

            return true;
        }


        private void AssembleTopicLists(bool isInstantRebuild = false)
        {
            if (!isInstantRebuild)
            {
                AssembleTopiclistTellMeAbout();
                AssembleTopicListLocation();
                AssembleTopicListPerson();
                AssembleTopicListThing();
            }
            else
            {
                if (instantRebuildTopicListTellMeAbout)
                    AssembleTopiclistTellMeAbout();
                if (instantRebuildTopicListLocation)
                    AssembleTopicListLocation();
                if (instantRebuildTopicListPerson)
                    AssembleTopicListPerson();
                if (instantRebuildTopicListThing)
                    AssembleTopicListThing();
                instantRebuildTopicListTellMeAbout = false;
                instantRebuildTopicListLocation = false;
                instantRebuildTopicListPerson = false;
                instantRebuildTopicListThing = false;
                DaggerfallUI.Instance.TalkWindow.UpdateListboxTopic();
            }
        }

        private void AssembleTopiclistTellMeAbout()
        {
            List<ListItem> oldTopicList = null;
            if (listTopicTellMeAbout != null)
                oldTopicList = listTopicTellMeAbout; // store old topic list to inject some of the info into new list

            listTopicTellMeAbout = new List<ListItem>();
            ListItem itemAnyNews = new ListItem();
            itemAnyNews.type = ListItemType.Item;
            itemAnyNews.questionType = QuestionType.News;
            itemAnyNews.caption = (TextManager.Instance.GetLocalizedText("AnyNews"));
            listTopicTellMeAbout.Add(itemAnyNews);

            ListItem itemWhereAmI = new ListItem();
            itemWhereAmI.type = ListItemType.Item;
            itemWhereAmI.questionType = QuestionType.WhereAmI;
            itemWhereAmI.caption = (TextManager.Instance.GetLocalizedText("WhereAmI"));
            listTopicTellMeAbout.Add(itemWhereAmI);

            foreach (KeyValuePair<ulong, QuestResources> questInfo in dictQuestInfo)
            {
                foreach (KeyValuePair<string, QuestResourceInfo> questResourceInfo in questInfo.Value.resourceInfo)
                {
                    ListItem itemQuestTopic = new ListItem();
                    itemQuestTopic.type = ListItemType.Item;
                    string captionString = string.Empty;
                    bool dialogPartnerIsSamePersonAsPersonResource = false;
                    switch (questResourceInfo.Value.resourceType)
                    {
                        case QuestInfoResourceType.NotSet:
                        default:
                            itemQuestTopic.questionType = QuestionType.NoQuestion;

                            break;
                        case QuestInfoResourceType.Location:
                            itemQuestTopic.questionType = QuestionType.QuestLocation;
                            Place place = (Place)questResourceInfo.Value.questResource;
                            if (place.SiteDetails.buildingName != null)
                                captionString = place.SiteDetails.buildingName;
                            else
                                captionString = place.SiteDetails.locationName;
                            break;
                        case QuestInfoResourceType.Person:
                            itemQuestTopic.questionType = QuestionType.QuestPerson;

                            Person person = (Person)questResourceInfo.Value.questResource;

                            captionString = person.DisplayName;
                            // test if dialog partner is same person as person resource
                            if (targetStaticNPC != null && currentNPCType == NPCType.Static &&
                                nameNPC == captionString &&
                                ((GameManager.Instance.IsPlayerInside &&
                                  !GameManager.Instance.IsPlayerInsideCastle &&
                                  targetStaticNPC.Data.buildingKey == GameManager.Instance.PlayerEnterExit.Interior
                                      .EntryDoor.buildingKey) ||
                                 (GameManager.Instance.IsPlayerInsideCastle &&
                                  person.IsQuestor &&
                                  person.QuestorData.context == StaticNPC.Context.Dungeon)))
                                dialogPartnerIsSamePersonAsPersonResource = true;
                            break;
                        case QuestInfoResourceType.Thing:
                            itemQuestTopic.questionType = QuestionType.QuestItem;
                            Item item = (Item)questResourceInfo.Value.questResource;
                            if (item != null && item.DaggerfallUnityItem != null)
                                captionString = item.DaggerfallUnityItem.ItemName;
                            break;
                    }

                    itemQuestTopic.questID = questInfo.Key;
                    itemQuestTopic.caption = captionString;
                    itemQuestTopic.key = questResourceInfo.Key;

                    if (questResourceInfo.Value.availableForDialog &&
                        questResourceInfo.Value.hasEntryInTellMeAbout && // Only make it available for talk if it is not "hidden" by dialog link command
                        !dialogPartnerIsSamePersonAsPersonResource)
                        listTopicTellMeAbout.Add(itemQuestTopic);
                }
            }

            for (int i = 0; i < infoFactionIDs.Length; i++)
            {
                ListItem itemOrganizationInfo = new ListItem();
                FactionFile.FactionData factionData;
                itemOrganizationInfo.type = ListItemType.Item;
                itemOrganizationInfo.questionType = QuestionType.OrganizationInfo;
                DaggerfallUnity.Instance.ContentReader.FactionFileReader.GetFactionData(infoFactionIDs[i], out factionData);
                itemOrganizationInfo.caption = factionData.name;
                itemOrganizationInfo.index = i;
                listTopicTellMeAbout.Add(itemOrganizationInfo);
            }

            if (oldTopicList != null)
            {
                for (int i = 0; i < listTopicTellMeAbout.Count; i++)
                {
                    ListItem oldItem = oldTopicList.Find(x => x.caption == listTopicTellMeAbout[i].caption);
                    if (oldItem != null)
                    {
                        listTopicTellMeAbout[i].npcKnowledgeAboutItem = oldItem.npcKnowledgeAboutItem;
                    }
                }
            }
        }

        private void AssembleTopicListLocation()
        {
            List<ListItem> oldTopicList = null;
            if (listTopicLocation != null)
                oldTopicList = listTopicLocation; // store old topic list to inject some of the info into new list

            listTopicLocation = new List<ListItem>();

            GetBuildingList();

            ListItem itemBuildingTypeGroup = null;
            List<BuildingInfo> matchingBuildings;

            foreach (DFLocation.BuildingTypes buildingType in Enum.GetValues(typeof(DFLocation.BuildingTypes)))
            {
                matchingBuildings = listBuildings.FindAll(x => x.buildingType == buildingType);
                if (CheckBuildingTypeInSkipList(buildingType))
                    continue;

                if (matchingBuildings.Count > 0)
                {
                    itemBuildingTypeGroup = new ListItem();
                    itemBuildingTypeGroup.type = ListItemType.ItemGroup;
                    itemBuildingTypeGroup.caption = BuildingTypeToGroupString(buildingType);

                    itemBuildingTypeGroup.listChildItems = new List<ListItem>();

                    ListItem itemPreviousList = new ListItem();
                    itemPreviousList.type = ListItemType.NavigationBack;
                    itemPreviousList.caption = (TextManager.Instance.GetLocalizedText("PreviousList"));
                    itemPreviousList.listParentItems = listTopicLocation;
                    itemBuildingTypeGroup.listChildItems.Add(itemPreviousList);

                    foreach (BuildingInfo buildingInfo in matchingBuildings)
                    {
                        ListItem item = new ListItem();
                        item.type = ListItemType.Item;
                        item.questionType = QuestionType.LocalBuilding;
                        item.caption = buildingInfo.name;
                        item.buildingKey = buildingInfo.buildingKey;
                        itemBuildingTypeGroup.listChildItems.Add(item);
                    }

                    listTopicLocation.Add(itemBuildingTypeGroup);
                }
            }

            bool alreadyCreatedGeneralSubSection = false;
            foreach (KeyValuePair<ulong, QuestResources> questInfo in dictQuestInfo)
            {
                foreach (KeyValuePair<string, QuestResourceInfo> questResourceInfo in questInfo.Value.resourceInfo)
                {
                    if (questResourceInfo.Value.resourceType == QuestInfoResourceType.Location)
                    {
                        Place place = (Place)questResourceInfo.Value.questResource;
                        // (Fixes bug reports http://forums.dfworkshop.net/viewtopic.php?f=24&t=996, http://forums.dfworkshop.net/viewtopic.php?f=24&t=997)
                        // Only build entries for place quest resources that are in the same location as the PC
                        if (GameManager.Instance.PlayerGPS.CurrentLocation.MapTableData.MapId != place.SiteDetails.mapId)
                            continue;

                        if (place.SiteDetails.buildingKey == 0)
                            continue;

                        DFLocation.BuildingTypes buildingType;
                        try
                        {
                            buildingType = GetBuildingTypeForBuildingKey(place.SiteDetails.buildingKey);
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError(string.Format("Error in TalkManager.GetBuildingTypeForBuildingKey: {0}", ex.Message));
                            continue;
                        }

                        if (RMBLayout.IsResidence(buildingType))
                        {
                            ListItem item = new ListItem();
                            item.type = ListItemType.Item;
                            item.questionType = QuestionType.LocalBuilding;
                            item.questID = questInfo.Key;
                            item.caption = place.SiteDetails.buildingName;
                            item.buildingKey = place.SiteDetails.buildingKey;

                            item.key = questResourceInfo.Key;

                            if (questResourceInfo.Value.availableForDialog && questResourceInfo.Value.hasEntryInWhereIs)
                            {
                                if (!alreadyCreatedGeneralSubSection)
                                {
                                    itemBuildingTypeGroup = new ListItem();
                                    itemBuildingTypeGroup.type = ListItemType.ItemGroup;
                                    itemBuildingTypeGroup.caption = (TextManager.Instance.GetLocalizedText("General"));
                                    listTopicLocation.Add(itemBuildingTypeGroup);
                                    alreadyCreatedGeneralSubSection = true;
                                }

                                if (itemBuildingTypeGroup.listChildItems == null)
                                {
                                    ListItem itemPreviousList = new ListItem();
                                    itemPreviousList.type = ListItemType.NavigationBack;
                                    itemPreviousList.caption = (TextManager.Instance.GetLocalizedText("PreviousList"));
                                    itemPreviousList.listParentItems = listTopicLocation;
                                    itemBuildingTypeGroup.listChildItems = new List<ListItem>();
                                    itemBuildingTypeGroup.listChildItems.Add(itemPreviousList);
                                }
                                itemBuildingTypeGroup.listChildItems.Add(item);
                            }
                        }
                    }
                }
            }

            matchingBuildings = listBuildings.FindAll(x => x.buildingType == DFLocation.BuildingTypes.Palace);
            if (matchingBuildings.Count > 0)
            {
                if (!alreadyCreatedGeneralSubSection)
                {
                    itemBuildingTypeGroup = new ListItem();
                    itemBuildingTypeGroup.type = ListItemType.ItemGroup;
                    itemBuildingTypeGroup.caption = (TextManager.Instance.GetLocalizedText("General"));
                    listTopicLocation.Add(itemBuildingTypeGroup);
                }

                if (itemBuildingTypeGroup.listChildItems == null)
                {
                    ListItem itemPreviousList = new ListItem();
                    itemPreviousList.type = ListItemType.NavigationBack;
                    itemPreviousList.caption = (TextManager.Instance.GetLocalizedText("PreviousList"));
                    itemPreviousList.listParentItems = listTopicLocation;
                    itemBuildingTypeGroup.listChildItems = new List<ListItem>();
                    itemBuildingTypeGroup.listChildItems.Add(itemPreviousList);
                }

                foreach (BuildingInfo buildingInfo in matchingBuildings)
                {
                    ListItem item = new ListItem();
                    item.type = ListItemType.Item;
                    item.questionType = QuestionType.LocalBuilding;
                    item.caption = buildingInfo.name;
                    item.buildingKey = buildingInfo.buildingKey;
                    itemBuildingTypeGroup.listChildItems.Add(item);
                }
            }

            itemBuildingTypeGroup = new ListItem();
            itemBuildingTypeGroup.type = ListItemType.ItemGroup;
            itemBuildingTypeGroup.caption = (TextManager.Instance.GetLocalizedText("Regional"));
            itemBuildingTypeGroup.listChildItems = new List<ListItem>();

            ListItem prevListItem = new ListItem();
            prevListItem.type = ListItemType.NavigationBack;
            prevListItem.caption = (TextManager.Instance.GetLocalizedText("PreviousList"));
            prevListItem.listParentItems = listTopicLocation;
            itemBuildingTypeGroup.listChildItems.Add(prevListItem);

            AddRegionalItems(ref itemBuildingTypeGroup);
            listTopicLocation.Add(itemBuildingTypeGroup);

            if (oldTopicList != null)
            {
                for (int i = 0; i < listTopicLocation.Count; i++)
                {
                    ListItem oldItem = oldTopicList.Find(x => x.caption == listTopicLocation[i].caption);
                    if (oldItem != null)
                    {
                        listTopicLocation[i].npcKnowledgeAboutItem = oldItem.npcKnowledgeAboutItem;
                    }
                }
            }
        }

        private void AddRegionalItems(ref ListItem itemBuildingTypeGroup)
        {
            int playerRegion = GameManager.Instance.PlayerGPS.CurrentRegionIndex;
            byte[] KnightlyOrderRegions = { 0x05, 0x11, 0x12, 0x14, 0x15, 0x16, 0x17, 0x2B, 0x33, 0x37 };

            for (int i = 0; i < 28; ++i)
            {
                if (i >= 8 && i <= 17) // Is a knightly order
                {
                    if (playerRegion == KnightlyOrderRegions[i - 8] && !DoesBuildingExistLocally(FactionsAndBuildings[i], false))
                        AddRegionalBuildingTalkItem(i, ref itemBuildingTypeGroup);
                }
                else if (i >= 20) // Is a store
                {
                    if (!DoesBuildingExistLocally(FactionsAndBuildings[i], true))
                        AddRegionalBuildingTalkItem(i, ref itemBuildingTypeGroup);
                }
                else if (!DoesBuildingExistLocally(FactionsAndBuildings[i], false)) // Is a temple
                {
                    AddRegionalBuildingTalkItem(i, ref itemBuildingTypeGroup);
                }
            }
        }

        private bool DoesBuildingExistLocally(short SearchedFor, bool SearchByBuildingType)
        {
            DFLocation location = GameManager.Instance.PlayerGPS.CurrentLocation;

            if (location.Loaded)
            {
                foreach (var building in location.Exterior.Buildings)
                {
                    if (SearchByBuildingType)
                    {
                        if ((short)building.BuildingType == SearchedFor)
                            return true;
                    }
                    else if (building.FactionId == SearchedFor)
                        return true;
                }
            }

            return false;
        }

        private void AddRegionalBuildingTalkItem(int index, ref ListItem itemBuildingTypeGroup)
        {
            ListItem item;

            string[] buildingNames = TextManager.Instance.GetLocalizedTextList("buildingNames");
            if (buildingNames == null || index < 0 || index > buildingNames.Length - 1)
                throw new Exception("buildingNames array text not found or idex out of range.");

            item = new ListItem();
            item.type = ListItemType.Item;
            item.questionType = QuestionType.Regional;
            item.caption = TextManager.Instance.GetLocalizedText("any").Replace("%s", buildingNames[index]);
            item.index = index;
            itemBuildingTypeGroup.listChildItems.Add(item);
        }

        private void AssembleTopicListPerson()
        {
            List<ListItem> oldTopicList = null;
            if (listTopicLocation != null)
                oldTopicList = listTopicLocation; // Store old topic list to inject some of the info into new list

            listTopicPerson = new List<ListItem>();

            foreach (KeyValuePair<ulong, QuestResources> questInfo in dictQuestInfo)
            {
                foreach (KeyValuePair<string, QuestResourceInfo> questResourceInfo in questInfo.Value.resourceInfo)
                {
                    if (questResourceInfo.Value.resourceType == QuestInfoResourceType.Person)
                    {
                        Person person = (Person)questResourceInfo.Value.questResource;

                        ListItem item = new ListItem();
                        item.type = ListItemType.Item;
                        item.questionType = QuestionType.Person;

                        ulong questID = questInfo.Key;
                        item.questID = questID;

                        string captionString = person.DisplayName;
                        item.caption = captionString;
                        item.key = questResourceInfo.Key;

                        bool IsPlayerInSameLocationWorldCell = false;
                        bool dialogPartnerIsSamePersonAsPersonResource = false;

                        // In case person is questor, check if questor is in same mapID
                        if (person.IsQuestor)
                        {
                            if (person.QuestorData.mapID == GameManager.Instance.PlayerGPS.CurrentMapID)
                                IsPlayerInSameLocationWorldCell = true;

                            // test if dialog partner is same person as person resource
                            if (targetStaticNPC != null && currentNPCType == NPCType.Static && nameNPC == captionString &&
                                ((GameManager.Instance.IsPlayerInside &&
                                 !GameManager.Instance.IsPlayerInsideCastle &&
                                 targetStaticNPC.Data.buildingKey == GameManager.Instance.PlayerEnterExit.Interior.EntryDoor.buildingKey) ||
                                 (GameManager.Instance.IsPlayerInsideCastle &&
                                  person.IsQuestor &&
                                  person.QuestorData.context == StaticNPC.Context.Dungeon)))
                            {
                                dialogPartnerIsSamePersonAsPersonResource = true;
                            }
                        }
                        else
                        {
                            /* - note: found a different way to do it via mapId
                            try
                            {
                                IsPlayerInSameLocationWorldCell = ((Questing.Person)questResourceInfo.Value.questResource).IsPlayerInSameLocationWorldCell();
                            }
                            catch (Exception e)
                            {
                            }*/

                            Symbol assignedPlaceSymbol = person.GetAssignedPlaceSymbol();

                            if (assignedPlaceSymbol != null)
                            {
                                Quest quest = GameManager.Instance.QuestMachine.GetQuest(questID);
                                Place assignedPlace = quest.GetPlace(assignedPlaceSymbol);  // Gets actual place resource
                                if (assignedPlace.SiteDetails.mapId == GameManager.Instance.PlayerGPS.CurrentMapID)
                                    IsPlayerInSameLocationWorldCell = true;
                            }
                        }

                        // only make it available for talk if...
                        if (!dialogPartnerIsSamePersonAsPersonResource &&   // Dialog partner is not the same person as the person resource talked about
                            questResourceInfo.Value.availableForDialog &&   // It is not "hidden" by dialog link command
                            questResourceInfo.Value.hasEntryInWhereIs &&    // It is meant to have an entry in the where is section (currently this is always true, TODO: check if we can get rid of this)
                            IsPlayerInSameLocationWorldCell)                // If person resource is in same world map location as player
                        {
                            listTopicPerson.Add(item);
                        }
                    }

                }
            }

            if (oldTopicList != null)
            {
                for (int i = 0; i < listTopicPerson.Count; i++)
                {
                    ListItem oldItem = oldTopicList.Find(x => x.caption == listTopicPerson[i].caption);
                    if (oldItem != null)
                    {
                        listTopicPerson[i].npcKnowledgeAboutItem = oldItem.npcKnowledgeAboutItem;
                    }
                }
            }
        }

        private void AssembleTopicListThing()
        {
            List<ListItem> oldTopicList = null;
            if (listTopicLocation != null)
                oldTopicList = listTopicLocation; // Store old topic list to inject some of the info into a new list

            listTopicThing = new List<ListItem>();

            if (oldTopicList != null)
            {
                for (int i = 0; i < listTopicThing.Count; i++)
                {
                    ListItem oldItem = oldTopicList.Find(x => x.caption == listTopicThing[i].caption);
                    if (oldItem != null)
                    {
                        listTopicThing[i].npcKnowledgeAboutItem = oldItem.npcKnowledgeAboutItem;
                    }
                }
            }
        }

        /// <summary>
        /// Gets portrait archive and texture record index for the current set target static NPC
        /// </summary>
        private void GetPortraitIndexFromStaticNPCBillboard(out DaggerfallTalkWindow.FacePortraitArchive facePortraitArchive, out int recordIndex)
        {
            FactionFile.FactionData factionData;
            GameManager.Instance.PlayerEntity.FactionData.GetFactionData(targetStaticNPC.Data.factionID, out factionData);

            FactionFile.FlatData factionFlatData = FactionFile.GetFlatData(factionData.flat1);
            FactionFile.FlatData factionFlatData2 = FactionFile.GetFlatData(factionData.flat2);

            // Get face for special NPCs here and return in this case
            if (factionData.type == 4)
            {
                facePortraitArchive = (factionData.face > 60) ? DaggerfallTalkWindow.FacePortraitArchive.CommonFaces : DaggerfallTalkWindow.FacePortraitArchive.SpecialFaces;
                recordIndex = factionData.face;
                return;
            }

            // If no special NPC, resolving process for common faces starts here
            facePortraitArchive = DaggerfallTalkWindow.FacePortraitArchive.CommonFaces;

            // Use "oops" as default - so use it if we fail to resolve face later on in this resolving process
            recordIndex = 410;

            FlatsFile.FlatData flatData;

            // Resolve face from NPC's faction data as default
            int archive = factionFlatData.archive;
            int record = factionFlatData.record;
            if (targetStaticNPC.Data.gender == Genders.Female)
            {
                archive = factionFlatData2.archive;
                record = factionFlatData2.record;
            }
            if (DaggerfallUnity.Instance.ContentReader.FlatsFileReader.GetFlatData(FlatsFile.GetFlatID(archive, record), out flatData)) // (if flat data exists in FlatsFile, overwrite index)
                recordIndex = flatData.faceIndex;

            // Overwrite if target NPC's billboard archive and record index can be resolved (more specific than just the factiondata - which will always resolve to the same portrait for a specific faction)
            if (DaggerfallUnity.Instance.ContentReader.FlatsFileReader.GetFlatData(FlatsFile.GetFlatID(targetStaticNPC.Data.billboardArchiveIndex, targetStaticNPC.Data.billboardRecordIndex), out flatData))
                recordIndex = flatData.faceIndex;
        }

        private string GetAnswerFromTokensArray(ulong questID, List<TextFile.Token[]> answers)
        {
            int randomNumAnswer = UnityEngine.Random.Range(0, answers.Count);

            // Cloning is important here: we want to evaluate every time the answer is created so altering macros are re-expanded correctly
            // e.g. Missing Prince quest allows player to ask for dungeon and there is a "%di" macro that needs to be re-evaluated
            // to correctly show current direction to dungeon (when in different towns it is likely to be different)
            TextFile.Token[] tokens = (TextFile.Token[])answers[randomNumAnswer].Clone();

            // Expand tokens and reveal dialog-linked resources
            QuestMacroHelper macroHelper = new QuestMacroHelper();
            macroHelper.ExpandQuestMessage(GameManager.Instance.QuestMachine.GetQuest(questID), ref tokens, true);

            return TokensToString(tokens);
        }

        private string TokensToString(TextFile.Token[] tokens, bool addSpaceAtTokenEnd = true)
        {
            // Create return string from expanded tokens
            string separatorString = " ";
            if (!addSpaceAtTokenEnd)
                separatorString = "";
            var builder = new System.Text.StringBuilder();
            for (int i = 0; i < tokens.Length; i++)
            {
                string textFragment = tokens[i].text;
                if (textFragment != null && textFragment != string.Empty)
                    builder.Append(textFragment);
                else
                    builder.Append(separatorString);
            }

            return builder.ToString();
        }

        private string ExpandRandomTextRecord(int recordIndex)
        {
            TextFile.Token[] tokens = DaggerfallUnity.Instance.TextProvider.GetRandomTokens(recordIndex);

            MacroHelper.ExpandMacros(ref tokens, this); // this... TalkManager (TalkManagerMCP)

            return TokensToString(tokens, false);
        }

        #endregion

        #region Event handlers

        private void OnMapPixelChanged(DFPosition mapPixel)
        {
            rebuildTopicLists = true;
            GetBuildingList();
        }

        private void OnTransitionToExterior(PlayerEnterExit.TransitionEventArgs args)
        {
            rebuildTopicLists = true;
            GetBuildingList();
        }

        private void OnTransitionToDungeonExterior(PlayerEnterExit.TransitionEventArgs args)
        {
            rebuildTopicLists = true;
            GetBuildingList();
        }

        private void OnTransitionToDungeonInterior(PlayerEnterExit.TransitionEventArgs args)
        {
            castleNPCsSpokenTo.Clear();
        }

        void OnLoadEvent(SaveData_v1 saveData)
        {
            rebuildTopicLists = true;
            GetBuildingList();
        }

        /// <summary>
        /// deletes any quest resources and rumors in rumor mill - important when starting from classic save import
        /// </summary>
        void OnStartGame()
        {
            dictQuestInfo.Clear();
            listRumorMill.Clear();
        }

        #endregion

        #region Console commands

        public static class TalkConsoleCommands
        {
            public static void RegisterCommands()
            {
                try
                {
                    ConsoleCommandsDatabase.RegisterCommand(TalkNpcsKnowEverything.name, TalkNpcsKnowEverything.description, TalkNpcsKnowEverything.usage, TalkNpcsKnowEverything.Execute);
                    ConsoleCommandsDatabase.RegisterCommand(TalkNpcsKnowUsual.name, TalkNpcsKnowUsual.description, TalkNpcsKnowUsual.usage, TalkNpcsKnowUsual.Execute);
                }
                catch (System.Exception ex)
                {
                    DaggerfallUnity.LogMessage(ex.Message, true);
                }
            }

            private static class TalkNpcsKnowEverything
            {
                public static readonly string name = "talk_npcsKnowEverything";
                public static readonly string description = "NPCs know everything and do not run out of answers";
                public static readonly string usage = "talk_npcsKnowEverything";


                public static string Execute(params string[] args)
                {
                    GameManager.Instance.TalkManager.ConsoleCommandFlag_npcsKnowEverything = true;
                    return "NPCS know everything now";
                }
            }

            private static class TalkNpcsKnowUsual
            {
                public static readonly string name = "talk_npcsKnowUsual";
                public static readonly string description = "NPCs know the usual number of things and run out of answers";
                public static readonly string usage = "talk_npcsKnowUsual";


                public static string Execute(params string[] args)
                {
                    GameManager.Instance.TalkManager.ConsoleCommandFlag_npcsKnowEverything = false;
                    return "NPCS know the usual stuff now";
                }
            }
        }

        #endregion
    }
}
