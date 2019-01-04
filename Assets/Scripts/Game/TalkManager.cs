// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Michael Rauter (Nystul)
// Contributors:    Numidium, Allofich, Interkarma
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

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// this class manages talk topics and resulting actions/answers
    /// </summary>
    public partial class TalkManager : MonoBehaviour
    {
        #region Singleton

        static TalkManager instance = null;
        public static TalkManager Instance
        {
            get
            {
                if (instance == null)
                {
                    if (!FindTalkManager(out instance))
                    {
                        GameObject go = new GameObject();
                        go.name = "TalkManager";
                        instance = go.AddComponent<TalkManager>();
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
            else if (instance != this)
            {
                if (Application.isPlaying)
                {
                    DaggerfallUnity.LogMessage("Multiple TalkManager instances detected in scene!", true);
                    Destroy(gameObject);
                }
            }
        }

        #endregion

        #region Fields

        const string textDatabase = "ConversationText";

        // dislike answer set (doesn't tell)
        const int dislikePlayerAnswerWhereIsDefault = 7256;
        const int dislikePlayerAnswerWhereIsGuildMembers = 7255;
        const int dislikePlayerAnswerWhereIsMerchants = 7256;
        const int dislikePlayerAnswerWhereIsNobility = 7258;
        const int dislikePlayerAnswerWhereIsScholars = 7257;
        const int dislikePlayerAnswerWhereIsUnderworld = 7259;
        // neutral answer set
        const int neutralToPlayerAnswerWhereIsDefault = 7271;
        const int neutralToPlayerAnswerWhereIsGuildMembers = 7270;
        const int neutralToPlayerAnswerWhereIsMerchants = 7271;
        const int neutralToPlayerAnswerWhereIsScholars = 7272;
        const int neutralToPlayerAnswerWhereIsNobility = 7273;
        const int neutralToPlayerAnswerWhereIsUnderworld = 7274;
        // like answer set
        const int likePlayerAnswerWhereIsDefault = 7291;
        const int likePlayerAnswerWhereIsGuildMembers = 7290;
        const int likePlayerAnswerWhereIsMerchants = 7291;
        const int likePlayerAnswerWhereIsScholars = 7292;
        const int likePlayerAnswerWhereIsNobility = 7293;
        const int likePlayerAnswerWhereIsUnderworld = 7294;
        // very like answer set
        const int veryLikePlayerAnswerWhereIsDefault = 7286;
        const int veryLikePlayerAnswerWhereIsGuildMembers = 7285;
        const int veryLikePlayerAnswerWhereIsMerchants = 7286;
        const int veryLikePlayerAnswerWhereIsScholars = 7287;
        const int veryLikePlayerAnswerWhereIsNobility = 7288;
        const int veryLikePlayerAnswerWhereIsUnderworld = 7289;

        // dislike does not know set
        const int dislikePlayerDoesNotKnowWhereIsDefault = 7251;
        const int dislikePlayerDoesNotKnowWhereIsGuildMembers = 7250;
        const int dislikePlayerDoesNotKnowWhereIsMerchants = 7251;
        const int dislikePlayerDoesNotKnowWhereIsNobility = 7252;
        const int dislikePlayerDoesNotKnowWhereIsScholars = 7253;
        const int dislikePlayerDoesNotKnowWhereIsUnderworld = 7304; // no matching underworld set (7254 is empty), used 7304 since it seems to fit (note: not in list of uesp)
        // neutral does not know set
        const int neutralToPlayerDoesNotKnowWhereIsDefault = 7266;
        const int neutralToPlayerDoesNotKnowWhereIsGuildMembers = 7265;
        const int neutralToPlayerDoesNotKnowWhereIsMerchants = 7266;
        const int neutralToPlayerDoesNotKnowWhereIsScholars = 7267;
        const int neutralToPlayerDoesNotKnowWhereIsNobility = 7268;
        const int neutralToPlayerDoesNotKnowWhereIsUnderworld = 7269;
        // like does not know set
        const int likePlayerDoesNotKnowWhereIsDefault = 7281;
        const int likePlayerDoesNotKnowWhereIsGuildMembers = 7280;
        const int likePlayerDoesNotKnowWhereIsMerchants = 7281;
        const int likePlayerDoesNotKnowWhereIsScholars = 7282;
        const int likePlayerDoesNotKnowWhereIsNobility = 7283;
        const int likePlayerDoesNotKnowWhereIsUnderworld = 7284;
        // very like does not know set
        const int veryLikePlayerDoesNotKnowWhereIsDefault = 7281;
        const int veryLikePlayerDoesNotKnowWhereIsGuildMembers = 7280;
        const int veryLikePlayerDoesNotKnowWhereIsMerchants = 7281;
        const int veryLikePlayerDoesNotKnowWhereIsScholars = 7282;
        const int veryLikePlayerDoesNotKnowWhereIsNobility = 7283;
        const int veryLikePlayerDoesNotKnowWhereIsUnderworld = 7284;

        // dislike answer set (doesn't tell)
        const int dislikePlayerAnswerTellMeAboutDefault = 7261;
        const int dislikePlayerAnswerTellMeAboutGuildMembers = 7260;
        const int dislikePlayerAnswerTellMeAboutMerchants = 7261;
        const int dislikePlayerAnswerTellMeAboutNobility = 7262;
        const int dislikePlayerAnswerTellMeAboutScholars = 7263;
        const int dislikePlayerAnswerTellMeAboutUnderworld = 7264;
        // neutral answer set
        const int neutralToPlayerAnswerTellMeAboutDefault = 7276;
        const int neutralToPlayerAnswerTellMeAboutGuildMembers = 7275;
        const int neutralToPlayerAnswerTellMeAboutMerchants = 7276;
        const int neutralToPlayerAnswerTellMeAboutScholars = 7277;
        const int neutralToPlayerAnswerTellMeAboutNobility = 7278;
        const int neutralToPlayerAnswerTellMeAboutUnderworld = 7279;
        // like answer set
        const int likePlayerAnswerTellMeAboutDefault = 7291;
        const int likePlayerAnswerTellMeAboutGuildMembers = 7290;
        const int likePlayerAnswerTellMeAboutMerchants = 7291;
        const int likePlayerAnswerTellMeAboutScholars = 7292;
        const int likePlayerAnswerTellMeAboutNobility = 7293;
        const int likePlayerAnswerTellMeAboutUnderworld = 7294;
        // very like answer set
        const int veryLikePlayerAnswerTellMeAboutDefault = 7286;
        const int veryLikePlayerAnswerTellMeAboutGuildMembers = 7285;
        const int veryLikePlayerAnswerTellMeAboutMerchants = 7286;
        const int veryLikePlayerAnswerTellMeAboutScholars = 7287;
        const int veryLikePlayerAnswerTellMeAboutNobility = 7288;
        const int veryLikePlayerAnswerTellMeAboutUnderworld = 7289;

        // dislike does not know set
        const int dislikePlayerDoesNotKnowTellMeAboutDefault = 7251;
        const int dislikePlayerDoesNotKnowTellMeAboutGuildMembers = 7250;
        const int dislikePlayerDoesNotKnowTellMeAboutMerchants = 7251;
        const int dislikePlayerDoesNotKnowTellMeAboutNobility = 7252;
        const int dislikePlayerDoesNotKnowTellMeAboutScholars = 7253;
        const int dislikePlayerDoesNotKnowTellMeAboutUnderworld = 7304; // no matching underworld set (7254 is empty), used 7304 since it seems to fit (note: not in list of uesp)
        // neutral does not know set
        const int neutralToPlayerDoesNotKnowTellMeAboutDefault = 7266;
        const int neutralToPlayerDoesNotKnowTellMeAboutGuildMembers = 7265;
        const int neutralToPlayerDoesNotKnowTellMeAboutMerchants = 7266;
        const int neutralToPlayerDoesNotKnowTellMeAboutScholars = 7267;
        const int neutralToPlayerDoesNotKnowTellMeAboutNobility = 7268;
        const int neutralToPlayerDoesNotKnowTellMeAboutUnderworld = 7269;
        // like does not know set
        const int likePlayerDoesNotKnowTellMeAboutDefault = 7281;
        const int likePlayerDoesNotKnowTellMeAboutGuildMembers = 7280;
        const int likePlayerDoesNotKnowTellMeAboutMerchants = 7281;
        const int likePlayerDoesNotKnowTellMeAboutScholars = 7282;
        const int likePlayerDoesNotKnowTellMeAboutNobility = 7283;
        const int likePlayerDoesNotKnowTellMeAboutUnderworld = 7284;
        // very like does not know set
        const int veryLikePlayerDoesNotKnowTellMeAboutDefault = 7281;
        const int veryLikePlayerDoesNotKnowTellMeAboutGuildMembers = 7280;
        const int veryLikePlayerDoesNotKnowTellMeAboutMerchants = 7281;
        const int veryLikePlayerDoesNotKnowTellMeAboutScholars = 7282;
        const int veryLikePlayerDoesNotKnowTellMeAboutNobility = 7283;
        const int veryLikePlayerDoesNotKnowTellMeAboutUnderworld = 7284;

        const int minNeutralReaction = 0;
        const int minLikeReaction = 10;
        const int minVeryLikeReaction = 30;
        // Lower reaction thresholds for info requests
        const int minInfoNeutralReaction = minNeutralReaction - 5;
        const int minInfoLikeReaction = minLikeReaction - 5;
        const int minInfoVeryLikeReaction = minVeryLikeReaction - 5;

        public static List<FactionFile.FactionIDs> factionsUsedForFactionInNews = new List<FactionFile.FactionIDs>()
        {
            FactionFile.FactionIDs.Abibon_Gora, FactionFile.FactionIDs.Alcaire, FactionFile.FactionIDs.Alikra, FactionFile.FactionIDs.Anticlere,
            FactionFile.FactionIDs.Antiphyllos, FactionFile.FactionIDs.Ayasofya, FactionFile.FactionIDs.Bergama, FactionFile.FactionIDs.Betony,
            FactionFile.FactionIDs.Bhoraine, FactionFile.FactionIDs.Cybiades, FactionFile.FactionIDs.Daenia, FactionFile.FactionIDs.Daggerfall,
            FactionFile.FactionIDs.Dakfron, FactionFile.FactionIDs.Dragontail, FactionFile.FactionIDs.Dwynnen, FactionFile.FactionIDs.Ephesus,
            FactionFile.FactionIDs.Gavaudon, FactionFile.FactionIDs.Glenpoint, FactionFile.FactionIDs.Ilessan_Hills, FactionFile.FactionIDs.Isle_of_Balfiera,
            FactionFile.FactionIDs.Kairou, FactionFile.FactionIDs.Kambria, FactionFile.FactionIDs.Koegria, FactionFile.FactionIDs.Kozanset,
            FactionFile.FactionIDs.Lainlyn, FactionFile.FactionIDs.Menevia, FactionFile.FactionIDs.Mournoth, FactionFile.FactionIDs.Myrkwasa,
            FactionFile.FactionIDs.Northmoor, FactionFile.FactionIDs.Orsinium, FactionFile.FactionIDs.Phrygia, FactionFile.FactionIDs.Pothago,
            FactionFile.FactionIDs.Santaki, FactionFile.FactionIDs.Satakalaam, FactionFile.FactionIDs.Sentinel, FactionFile.FactionIDs.Shalgora,
            FactionFile.FactionIDs.Tigonus, FactionFile.FactionIDs.Totambu, FactionFile.FactionIDs.Tulune, FactionFile.FactionIDs.Urvaius,
            FactionFile.FactionIDs.Wayrest, FactionFile.FactionIDs.Wrothgaria, FactionFile.FactionIDs.Ykalon
        };

        public static List<FactionFile.FactionIDs> factionsUsedForRulers = new List<FactionFile.FactionIDs>()
        {
            FactionFile.FactionIDs.Abibon_Gora, FactionFile.FactionIDs.Alcaire, FactionFile.FactionIDs.Alikra,
            FactionFile.FactionIDs.Antiphyllos, FactionFile.FactionIDs.Ayasofya, FactionFile.FactionIDs.Bergama, FactionFile.FactionIDs.Betony,
            FactionFile.FactionIDs.Bhoraine, FactionFile.FactionIDs.Cybiades, FactionFile.FactionIDs.Daenia,
            FactionFile.FactionIDs.Dakfron, FactionFile.FactionIDs.Dragontail, FactionFile.FactionIDs.Dwynnen, FactionFile.FactionIDs.Ephesus,
            FactionFile.FactionIDs.Gavaudon, FactionFile.FactionIDs.Glenpoint, FactionFile.FactionIDs.Ilessan_Hills,
            FactionFile.FactionIDs.Kairou, FactionFile.FactionIDs.Kambria, FactionFile.FactionIDs.Koegria, FactionFile.FactionIDs.Kozanset,
            FactionFile.FactionIDs.Lainlyn, FactionFile.FactionIDs.Menevia, FactionFile.FactionIDs.Mournoth, FactionFile.FactionIDs.Myrkwasa,
            FactionFile.FactionIDs.Northmoor, FactionFile.FactionIDs.Phrygia, FactionFile.FactionIDs.Pothago,
            FactionFile.FactionIDs.Santaki, FactionFile.FactionIDs.Satakalaam, FactionFile.FactionIDs.Shalgora,
            FactionFile.FactionIDs.Tigonus, FactionFile.FactionIDs.Totambu, FactionFile.FactionIDs.Tulune, FactionFile.FactionIDs.Urvaius,
            FactionFile.FactionIDs.Wrothgaria, FactionFile.FactionIDs.Ykalon
        };

        const float DefaultChanceKnowsSomethingAboutWhereIs = 0.5f; // chances unknown
        const float DefaultChanceKnowsSomethingAboutQuest = 0.5f; // chances unknown
        const float DefaultChanceKnowsSomethingAboutOrganizationsStaticNPC = 0.5f; // chances unknown
        const float DefaultChanceKnowsSomethingAboutOrganizationsMobileNPC = 0.0f; // chances unknown
        const float ChanceToRevealLocationOnMap = 0.25f; //chances unknown

        const int maxNumAnswersNpcGivesTellMeAboutOrRumors = 1; // maximum number of answers npc gives about "tell me about" question or rumors

        // specifies entry type of list item in topic lists
        public enum ListItemType
        {
            Item, // a item that can be talked about
            ItemGroup, // a group containing other items
            NavigationBack // a special item to navigate out of group items ("previous list")
        }

        public enum QuestionType
        {
            NoQuestion, // used for list entries that are not of ListItemType item
            News, // used for "any news" question
            WhereAmI, // used for "Where am I?" question
            OrganizationInfo, // used for "tell me about" -> organizations
            Work, // used for "where is" -> "work"
            LocalBuilding, // used for "where is" -> "location"
            Regional, // used for "where is" -> "location" -> "regional"
            Person, // used for "where is" -> "person"
            Thing, // not used ("where is" -> "thing") - not implemented in vanilla daggerfall as well
            QuestLocation, // used for quest resources that are locations that get added to "tell me about" section
            QuestPerson, // used for quest resources that are persons that get added to "tell me about" section
            QuestItem // used for quest resources that are items that get added to "tell me about" section
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

        // this class holds information about a npc talk partner
        public class NPCData
        {
            public Races race;            
            public FactionFile.SocialGroups socialGroup;
            public FactionFile.GuildGroups guildGroup;
            public FactionFile.FactionData factionData; // only used for static npcs
            public float chanceKnowsSomethingAboutWhereIs; // the general chance that the current npc knows the answer to pc's "where is" question
            public float chanceKnowsSomethingAboutQuest; // the general chance that the current npc knows the answer to pc's quest related question
            public float chanceKnowsSomethingAboutOrganizations; // the general chance that the current npc knows the answer to pc's question about organizations
            public int numAnswersGivenTellMeAboutOrRumors; // the number of (successful) answers to a "tell me about" question or rumors given by the npc (answers about npc knew something)
            public bool isSpyMaster;
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
        bool alreadyRejectedOnce = false; // used to display a random rejection text first time when talking to a npc that dislikes pc, trying to talk a 2nd time (for same npc) pc gets msg "you get no response"

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
        }
        // list of rumors in rumor mill
        List<RumorMillEntry> listRumorMill = new List<RumorMillEntry>();


        // questor post quest message stuff (QuestorPostsuccess, QuestorPostfailure)
        Dictionary<ulong, TextFile.Token[]> dictQuestorPostQuestMessage = new Dictionary<ulong,TextFile.Token[]>();

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
            get { return currentKeySubjectType;  }
        }

        public bool MarkLocationOnMap
        {
            get { return markLocationOnMap;  }
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
            get {  return listTopicThing; }
        }

        public static string TextDatabase
        {
            get { return textDatabase; }
        }

        public bool ConsoleCommandFlag_npcsKnowEverything
        {
            get { return consoleCommandFlag_npcsKnowEverything; }
            set { consoleCommandFlag_npcsKnowEverything = value; }
        }        

        #endregion

        #region Unity

        void Awake()
        {
            SetupSingleton();

            // important that transition events/delegates are created in Awake() instead of OnEnable (since exteriorAutomap gameobject is disabled when going indoors and enabled when going outdoors)
            PlayerGPS.OnMapPixelChanged += OnMapPixelChanged;
            PlayerEnterExit.OnTransitionExterior += OnTransitionToExterior;
            PlayerEnterExit.OnTransitionDungeonExterior += OnTransitionToDungeonExterior;
            PlayerEnterExit.OnTransitionDungeonInterior += OnTransitionToDungeonInterior;
            SaveLoadManager.OnLoad += OnLoadEvent;

            // initialize work variables
            exteriorUsedForQuestors = 0;
            selectedNpcWorkKey = -1;
        }

        void OnDestroy()
        {
            // important that transition events/delegates are destroyed in OnDestroy() instead of OnDisable (since exteriorAutomap gameobject is disabled when going indoors and enabled when going outdoors)
            PlayerGPS.OnMapPixelChanged -= OnMapPixelChanged;
            PlayerEnterExit.OnTransitionExterior -= OnTransitionToExterior;
            PlayerEnterExit.OnTransitionDungeonExterior -= OnTransitionToDungeonExterior;
            PlayerEnterExit.OnTransitionDungeonInterior -= OnTransitionToDungeonInterior;
            SaveLoadManager.OnLoad -= OnLoadEvent;
        }

        void OnEnable()
        {
            
        }

        void OnDisable()
        {
            
        }

        void Start()
        {
            // register console commands
            try
            {
                TalkConsoleCommands.RegisterCommands();
            }
            catch (Exception ex)
            {
                Debug.LogError(string.Format("Error Registering Talk Console commands: {0}", ex.Message));

            }
        }

        void Update()
        {

        }

        #endregion

        #region Public Methods

        public void ResetNPCKnowledge()
        {
            ResetNPCKnowledgeInTopicListRecursively(listTopicLocation);
            ResetNPCKnowledgeInTopicListRecursively(listTopicPerson);
            ResetNPCKnowledgeInTopicListRecursively(listTopicThing);
            ResetNPCKnowledgeInTopicListRecursively(listTopicTellMeAbout);
        }

        public int GetReactionToPlayer(FactionFile.SocialGroups socialGroup, MonoBehaviour target)
        {
            int reactionToPlayer = 0;
            const int reactionRollMax = 15;

            // Get NPC faction
            // TODO: Factor in adjustments for children of regional factions
            FactionFile.FactionData NPCfaction;
            int currentRegionIndex = GameManager.Instance.PlayerGPS.CurrentRegionIndex;
            FactionFile.FactionData[] factions = GameManager.Instance.PlayerEntity.FactionData.FindFactions(
                (int)FactionFile.FactionTypes.Province, -1, -1, currentRegionIndex);

            // Should always find a region
            if (factions == null || factions.Length == 0)
                throw new Exception("GetReactionToPlayer() did not find a match for NPC faction.");

            // Warn if more than 1 region is found
            if (factions.Length > 1)
                Debug.LogWarningFormat("GetReactionToPlayer() found more than 1 matching NPC faction for region {0}.", currentRegionIndex);

            NPCfaction = factions[0];
            
            PlayerEntity player = GameManager.Instance.PlayerEntity;
            reactionToPlayer = NPCfaction.rep;
            reactionToPlayer += player.BiographyReactionMod;

            DFRandom.Seed = (uint)target.GetHashCode(); // Roll result should be the same every time for a given NPC
            reactionToPlayer += DFRandom.random_range_inclusive(0, reactionRollMax);

            if ((int)socialGroup < player.SGroupReputations.Length) // one of the five general social groups
                reactionToPlayer += player.SGroupReputations[(int)socialGroup];

            return (reactionToPlayer);
        }

        // Player has clicked on a mobile talk target
        public void TalkToMobileNPC(MobilePersonNPC targetNPC)
        {
            currentNPCType = NPCType.Mobile;

            // Get reaction to player
            int reactionToPlayer = GetReactionToPlayer(FactionFile.SocialGroups.Commoners, targetNPC); // All mobile NPCs are commoners

            sameTalkTargetAsBefore = false;
            GameManager.Instance.TalkManager.SetTargetNPC(targetNPC, reactionToPlayer, ref sameTalkTargetAsBefore);

            npcData.numAnswersGivenTellMeAboutOrRumors = 0; // important to reset this here so even if npcs is the same as previous talk session pc will give one correct answer if npc knows about topic (as implemented in vanilla df)

            TalkToNpc();
        }

        // Player has clicked or static talk target or clicked the talk button inside a popup-window
        public void TalkToStaticNPC(StaticNPC targetNPC, bool menu = true, bool isSpyMaster = false)
        {
            // Populate NPC faction data
            FactionFile.FactionData targetFactionData;
            PersistentFactionData factionsData;
            factionsData = GameManager.Instance.PlayerEntity.FactionData;
            factionsData.GetFactionData(targetNPC.Data.factionID, out targetFactionData);

            if (IsNpcOfferingQuest(targetNPC.Data.nameSeed)) {
                DaggerfallUI.UIManager.PushWindow(new DaggerfallQuestOfferWindow(DaggerfallUI.UIManager, npcsWithWork[targetNPC.Data.nameSeed].npc, npcsWithWork[targetNPC.Data.nameSeed].socialGroup, menu));
                return;
            }
            else if (IsCastleNpcOfferingQuest(targetNPC.Data.nameSeed))
            {
                DaggerfallUI.UIManager.PushWindow(new DaggerfallQuestOfferWindow(DaggerfallUI.UIManager, targetNPC.Data, (FactionFile.SocialGroups)targetFactionData.sgroup, menu));
                return;
            }
            currentNPCType = NPCType.Static;

            // Get reaction to player
            int reactionToPlayer = GetReactionToPlayer((FactionFile.SocialGroups)targetFactionData.sgroup, targetNPC);

            sameTalkTargetAsBefore = false;
            GameManager.Instance.TalkManager.SetTargetNPC(targetNPC, reactionToPlayer, ref sameTalkTargetAsBefore);

            npcData.numAnswersGivenTellMeAboutOrRumors = 0; // important to reset this here so even if npcs is the same as previous talk session pc will can one correct answer if npc knows about topic (as implemented in vanilla df)
            npcData.isSpyMaster = isSpyMaster;

            TalkToNpc();
        }

        public void SetTargetNPC(MobilePersonNPC targetMobileNPC, int reactionToPlayer, ref bool sameTalkTargetAsBefore)
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
            npcData.race = targetMobileNPC.Race;
            npcData.chanceKnowsSomethingAboutWhereIs = DefaultChanceKnowsSomethingAboutWhereIs;
            npcData.chanceKnowsSomethingAboutQuest = DefaultChanceKnowsSomethingAboutQuest;
            npcData.chanceKnowsSomethingAboutOrganizations = DefaultChanceKnowsSomethingAboutOrganizationsMobileNPC;
            npcData.isSpyMaster = false;

            this.reactionToPlayer = reactionToPlayer;

            AssembleTopicListPerson(); // update "Where Is" -> "Person" list since this list may hide the questor (if talking to the questor)
        }

        public void SetTargetNPC(StaticNPC targetNPC, int reactionToPlayer, ref bool sameTalkTargetAsBefore)
        {
            sameTalkTargetAsBefore = false;

            if (targetNPC == lastTargetStaticNPC)
            {
                sameTalkTargetAsBefore = true;
                return;
            }

            alreadyRejectedOnce = false;

            this.targetStaticNPC = targetNPC;

            DaggerfallTalkWindow.FacePortraitArchive facePortraitArchive;
            int recordIndex;
            GetPortraitIndexFromStaticNPCBillboard(out facePortraitArchive, out recordIndex);
            DaggerfallUI.Instance.TalkWindow.SetNPCPortrait(facePortraitArchive, recordIndex);

            lastTargetStaticNPC = targetNPC;

            nameNPC = targetNPC.DisplayName;
            DaggerfallUI.Instance.TalkWindow.UpdateNameNPC();

            FactionFile.FactionData factionData;
            GameManager.Instance.PlayerEntity.FactionData.GetFactionData(targetStaticNPC.Data.factionID, out factionData);

            // Matched to classic. For dialogue, NPCs that are not of type 2, 7 or 9 use their first parent that is, if such a parent exists
            while (factionData.parent != 0 && factionData.type != 2 && factionData.type != 7 && factionData.type != 9)
            {
                GameManager.Instance.PlayerEntity.FactionData.GetFactionData(factionData.parent, out factionData);
            }

            npcData = new NPCData();
            npcData.socialGroup = (FactionFile.SocialGroups)factionData.sgroup;
            npcData.guildGroup = (FactionFile.GuildGroups)factionData.ggroup;
            npcData.factionData = factionData;
            npcData.race = Races.Breton; // TODO: find a way to get race for static npc
            npcData.chanceKnowsSomethingAboutWhereIs = DefaultChanceKnowsSomethingAboutWhereIs;
            npcData.chanceKnowsSomethingAboutQuest = DefaultChanceKnowsSomethingAboutQuest;
            npcData.chanceKnowsSomethingAboutOrganizations = DefaultChanceKnowsSomethingAboutOrganizationsStaticNPC;
            npcData.isSpyMaster = false;

            this.reactionToPlayer = reactionToPlayer;

            AssembleTopicListPerson(); // update "Where Is" -> "Person" list since this list may hide the questor (if talking to the questor)
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

        public string GetNPCGreetingText()
        {
            const int dislikePlayerGreetingTextId = 7206;
            const int neutralToPlayerGreetingTextId = 7207;
            const int likePlayerGreetingTextId = 7208;
            const int veryLikePlayerGreetingTextId = 7209;

            const int isInSameGuildLikePlayerGreetingTextId = 8550;
            const int isInSameGuildNeutralPlayerGreetingTextId = 8551;

            const int isInSameHolyOrderLikePlayerGreetingTextId = 8553;
            const int isInSameHolyOrderNeutralPlayerGreetingTextId = 8554;

            Guild guild = GameManager.Instance.GuildManager.GetGuild((int) GameManager.Instance.PlayerEnterExit.FactionID);

            // note Nystul: did not find any use of text record ids 8556 - 8569 in my testing - but some of them might be used by nobles of the courtyards

            if (currentNPCType == NPCType.Static)
            {
                foreach(KeyValuePair<ulong, TextFile.Token[]> entry in dictQuestorPostQuestMessage)
                {
                    ulong questID = entry.Key;
                    Quest quest = GameManager.Instance.QuestMachine.GetQuest(questID);
                    if (quest == null)
                        continue;
                    QuestResource[] questPeople = quest.GetAllResources(typeof(Person));
                    foreach (Person person in questPeople)
                    {
                        if (person.IsQuestor)
                        {
                            if (GameManager.Instance.QuestMachine.IsNPCDataEqual(person.QuestorData, lastTargetStaticNPC.Data))
                            {
                                TextFile.Token[] tokens = dictQuestorPostQuestMessage[questID];

                                // Set external context to guild if player is a member
                                if (guild.IsMember())
                                    quest.ExternalMCP = guild;
                                // expand tokens and reveal dialog-linked resources
                                QuestMacroHelper macroHelper = new QuestMacroHelper();
                                macroHelper.ExpandQuestMessage(quest, ref tokens, true);
                                return TokensToString(tokens);
                            }
                        }
                    }
                }
            }

            // check if npc is member of a guild            
            FactionFile.FactionData factionData;
            if (DaggerfallUnity.Instance.ContentReader.FactionFileReader.GetFactionData(guild.GetFactionId(), out factionData) && // first check is important to rule out merchants that are assigned to fighters guild (see this bug report: https://forums.dfworkshop.net/viewtopic.php?f=24&t=1240)
                GameManager.Instance.GuildManager.GetGuild(npcData.guildGroup, (int)GameManager.Instance.PlayerEnterExit.FactionID).IsMember())            
            {
                if (npcData.guildGroup == FactionFile.GuildGroups.HolyOrder) // holy orders use message 8553, 8554
                {
                    if (reactionToPlayer >= minVeryLikeReaction) // what reputation is needed to show like greeting message?
                        return ExpandRandomTextRecord(isInSameHolyOrderLikePlayerGreetingTextId);
                    else if (reactionToPlayer >= minNeutralReaction) // not sure here - are member greeting messages also shown if npc dislikes pc (but still talks to pc)?
                        return ExpandRandomTextRecord(isInSameHolyOrderNeutralPlayerGreetingTextId);
                }
                else // all other guilds (including Knightly Orders) seem to use messages 8550, 8551
                {
                    if (reactionToPlayer >= minVeryLikeReaction) // what reputation is needed to show like greeting message?
                        return ExpandRandomTextRecord(isInSameGuildLikePlayerGreetingTextId);
                    else if (reactionToPlayer >= minNeutralReaction) // not sure here - are member greeting messages also shown if npc dislikes pc (but still talks to pc)?
                        return ExpandRandomTextRecord(isInSameGuildNeutralPlayerGreetingTextId);
                }
            }

            if (reactionToPlayer >= minVeryLikeReaction)
                return ExpandRandomTextRecord(veryLikePlayerGreetingTextId);
            else if (reactionToPlayer >= minLikeReaction)
                return ExpandRandomTextRecord(likePlayerGreetingTextId);
            else if (reactionToPlayer >= minNeutralReaction)
                return ExpandRandomTextRecord(neutralToPlayerGreetingTextId);
            else            
                return ExpandRandomTextRecord(dislikePlayerGreetingTextId);
        }

        public string GetPCGreetingText(DaggerfallTalkWindow.TalkTone talkTone)
        {
            int toneIndex = DaggerfallTalkWindow.TalkToneToIndex(talkTone);
            string greetingString = ExpandRandomTextRecord(7215 + toneIndex);

            return (greetingString);
        }

        public string GetPCFollowUpText(DaggerfallTalkWindow.TalkTone talkTone)
        {
            int toneIndex = DaggerfallTalkWindow.TalkToneToIndex(talkTone);
            string followUpString = ExpandRandomTextRecord(7218 + toneIndex);
            return (followUpString);
        }

        public string GetPCGreetingOrFollowUpText()
        {
            if (questionOpeningText == "")
            {
                if (numQuestionsAsked == 0)
                    questionOpeningText = GetPCGreetingText(currentTalkTone);
                else
                    questionOpeningText = GetPCFollowUpText(currentTalkTone);
            }
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
                return TextManager.Instance.GetText(textDatabase, "east");
            else if (angle >= 22.5f && angle < 67.5f)
                return TextManager.Instance.GetText(textDatabase, "northeast");
            else if (angle >= 67.5f && angle < 112.5f)
                return TextManager.Instance.GetText(textDatabase, "north");
            else if (angle >= 112.5f && angle < 157.5f)
                return TextManager.Instance.GetText(textDatabase, "northwest");
            else if (angle >= 157.5f && angle < 202.5f)
                return TextManager.Instance.GetText(textDatabase, "west");
            else if (angle >= 202.5f && angle < 247.5f)
                return TextManager.Instance.GetText(textDatabase, "southwest");
            else if (angle >= 247.5f && angle < 292.5f)
                return TextManager.Instance.GetText(textDatabase, "south");
            else if (angle >= 292.5f && angle < 337.5f)
                return TextManager.Instance.GetText(textDatabase, "southeast");
            else
                return TextManager.Instance.GetText(textDatabase, "resolvingError");
        }

        public string GetKeySubjectLocationCompassDirection()
        {
            // note Nystul:
            // I reused coordinate mapping from buildings from exterior automap layout implementation here
            // So both building position as well as player position are calculated in map coordinates and compared
            Vector2 playerPos;
            float scale = MapsFile.WorldMapTerrainDim * MeshReader.GlobalScale;
            playerPos.x = ((GameManager.Instance.PlayerGPS.transform.position.x) % scale) / scale;
            playerPos.y = ((GameManager.Instance.PlayerGPS.transform.position.z) % scale) / scale;
            int refWidth = (int)(ExteriorAutomap.blockSizeWidth * ExteriorAutomap.numMaxBlocksX * GameManager.Instance.ExteriorAutomap.LayoutMultiplier);
            int refHeight = (int)(ExteriorAutomap.blockSizeHeight * ExteriorAutomap.numMaxBlocksY * GameManager.Instance.ExteriorAutomap.LayoutMultiplier);
            playerPos.x *= refWidth;
            playerPos.y *= refHeight;
            playerPos.x -= refWidth * 0.5f;
            playerPos.y -= refHeight * 0.5f;

            BuildingInfo buildingInfo = listBuildings.Find(x => x.buildingKey == currentKeySubjectBuildingKey);

            if (dictQuestInfo.ContainsKey(currentQuestionListItem.questID) && dictQuestInfo[currentQuestionListItem.questID].resourceInfo.ContainsKey(currentQuestionListItem.key))
            {
                if (dictQuestInfo[currentQuestionListItem.questID].resourceInfo[currentQuestionListItem.key].questPlaceResourceHintTypeReceived != QuestResourceInfo.BuildingLocationHintTypeGiven.LocationWasMarkedOnMap)
                    dictQuestInfo[currentQuestionListItem.questID].resourceInfo[currentQuestionListItem.key].questPlaceResourceHintTypeReceived = QuestResourceInfo.BuildingLocationHintTypeGiven.ReceivedDirectionalHints;
            }

            Vector2 vecDirectionToTarget = buildingInfo.position - playerPos;
            return DirectionVector2DirectionHintString(vecDirectionToTarget);
        }

        public void MarkKeySubjectLocationOnMap()
        {
            BuildingInfo buildingInfo = listBuildings.Find(x => x.buildingKey == currentKeySubjectBuildingKey);
            if (buildingInfo.buildingKey != 0)
            {
                if (dictQuestInfo.ContainsKey(currentQuestionListItem.questID) && dictQuestInfo[currentQuestionListItem.questID].resourceInfo.ContainsKey(currentQuestionListItem.key))
                    dictQuestInfo[currentQuestionListItem.questID].resourceInfo[currentQuestionListItem.key].questPlaceResourceHintTypeReceived = QuestResourceInfo.BuildingLocationHintTypeGiven.LocationWasMarkedOnMap;
                GameManager.Instance.PlayerGPS.DiscoverBuilding(buildingInfo.buildingKey);
            }
        }

        public string GetQuestionText(TalkManager.ListItem listItem, DaggerfallTalkWindow.TalkTone talkTone)
        {
            int toneIndex = DaggerfallTalkWindow.TalkToneToIndex(talkTone);
            string question = "";

            currentTalkTone = talkTone;

            currentKeySubject = listItem.caption; // set key to current caption for now (which is in case of buildings the building name)

            currentQuestionListItem = listItem;

            switch (listItem.questionType)
            {
                case QuestionType.NoQuestion:
                default:
                    break;
                case QuestionType.News:
                    question = ExpandRandomTextRecord(7231 + toneIndex);
                    break;
                case QuestionType.WhereAmI:
                    question = TextManager.Instance.GetText(textDatabase, "WhereAmI");
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
                    question = "not implemented"; // vanilla df did not implement this as well
                    break;
                case QuestionType.Regional:
                    currentKeySubjectType = KeySubjectType.Building;

                    // Improvement over classic. Make "Any" lower-case since it will be in the middle of a sentence.
                    currentKeySubject = currentKeySubject.Replace(TextManager.Instance.GetText(textDatabase, "toBeReplacedStringRegional"), TextManager.Instance.GetText(textDatabase, "replacementStringRegional"));

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

        public string GetNewsOrRumors()
        {
            const int outOfNewsRecordIndex = 1457;
            if (npcData.numAnswersGivenTellMeAboutOrRumors < maxNumAnswersNpcGivesTellMeAboutOrRumors || npcData.isSpyMaster || consoleCommandFlag_npcsKnowEverything)
            {
                string news = TextManager.Instance.GetText(textDatabase, "resolvingError");
                int randomIndex = UnityEngine.Random.Range(0, listRumorMill.Count);
                RumorMillEntry entry = listRumorMill[randomIndex];
                if (entry.rumorType == RumorType.CommonRumor)
                {
                    if (entry.listRumorVariants != null)
                    {
                        TextFile.Token[] tokens = entry.listRumorVariants[0];
                        MacroHelper.ResetFactionAndRulerIds(); // reset so that a new set of rulers and factions can be generated
                        MacroHelper.ExpandMacros(ref tokens, this);
                        MacroHelper.ResetFactionAndRulerIds(); // reset again so %reg macro may resolve to current region if needed
                        news = TokensToString(tokens, false);
                    }
                }
                else if (entry.rumorType == RumorType.QuestRumorMill || entry.rumorType == RumorType.QuestProgressRumor)
                {
                    int variant = UnityEngine.Random.Range(0, entry.listRumorVariants.Count);
                    TextFile.Token[] tokens = entry.listRumorVariants[variant];

                    // expand tokens and reveal dialog-linked resources
                    QuestMacroHelper macroHelper = new QuestMacroHelper();
                    macroHelper.ExpandQuestMessage(GameManager.Instance.QuestMachine.GetQuest(entry.questID), ref tokens, true);
                    news = TokensToString(tokens);
                }

                npcData.numAnswersGivenTellMeAboutOrRumors++;

                return (news);
            }
            else
                return ExpandRandomTextRecord(outOfNewsRecordIndex);
        }

        public string GetAnswerWhereAmI()
        {
            // all npcs know this answer
            if (GameManager.Instance.IsPlayerInside)
            {
                if (GameManager.Instance.PlayerEnterExit.ExteriorDoors.Length > 0) // in building
                {
                    PlayerGPS.DiscoveredBuilding discoveredBuilding;
                    if (GameManager.Instance.PlayerGPS.GetAnyBuilding(GameManager.Instance.PlayerEnterExit.ExteriorDoors[0].buildingKey, out discoveredBuilding))
                    {
                        return String.Format(TextManager.Instance.GetText(textDatabase, "AnswerTextWhereAmI"), discoveredBuilding.displayName, GameManager.Instance.PlayerGPS.CurrentLocation.Name);
                    }
                    else
                    {
                        // fallback if no discovery info was found
                        BuildingInfo currentBuilding = listBuildings.Find(x => x.buildingKey == GameManager.Instance.PlayerEnterExit.ExteriorDoors[0].buildingKey);
                        return String.Format(TextManager.Instance.GetText(textDatabase, "AnswerTextWhereAmI"), currentBuilding.name, GameManager.Instance.PlayerGPS.CurrentLocation.Name);
                    }
                }
                else if (GameManager.Instance.IsPlayerInsideCastle || GameManager.Instance.IsPlayerInsideDungeon) // in dungeon
                {
                    DaggerfallDungeon.DungeonSummary ds = GameManager.Instance.PlayerEnterExit.Dungeon.Summary;
                    string dungeonName = "";
                    if (ds.RegionName == "Daggerfall" && ds.LocationName == "Daggerfall")
                        dungeonName = ExpandRandomTextRecord(475);
                    else if (ds.RegionName == "Wayrest" && ds.LocationName == "Wayrest")
                        dungeonName = ExpandRandomTextRecord(476);
                    else if (ds.RegionName == "Sentinel" && ds.LocationName == "Sentinel")
                        dungeonName = ExpandRandomTextRecord(477);
                    else
                    {
                        dungeonName = GameManager.Instance.PlayerEnterExit.Dungeon.Summary.LocationName;
                    }

                    dungeonName = dungeonName.TrimEnd('.'); // remove character '.' from castle text record entry

                    return String.Format(TextManager.Instance.GetText(textDatabase, "AnswerTextWhereAmI"), dungeonName, GameManager.Instance.PlayerEnterExit.Dungeon.Summary.RegionName);
                }
            }
            else
            {
                return String.Format(TextManager.Instance.GetText(textDatabase, "AnswerTextWhereAmI"), GameManager.Instance.PlayerGPS.CurrentLocation.Name, GameManager.Instance.PlayerGPS.CurrentRegionName);
            }
            return TextManager.Instance.GetText(textDatabase, "resolvingError");
        }

        public string GetOrganizationInfo(TalkManager.ListItem listItem)
        {           
            int index = (listItem.index > 7 ? listItem.index + 1 : listItem.index); // note Nystul: this looks error-prone because we are assuming specific indices here -> what if this changes some day?
            return ExpandRandomTextRecord(860 + index);
        }

        public void AddQuestRumorToRumorMill(ulong questID, Message message)
        {
            if (listRumorMill == null || listRumorMill.Count == 0)
                SetupRumorMill();

            RumorMillEntry entry = new RumorMillEntry();
            entry.rumorType = RumorType.QuestRumorMill;
            entry.questID = questID;
            entry.listRumorVariants = new List<TextFile.Token[]>();
            for (int i=0; i<message.VariantCount; i++)
            {
                TextFile.Token[] variantItem = message.GetTextTokensByVariant(i, false); // do not expand macros
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
                TextFile.Token[] variantItem = message.GetTextTokensByVariant(v, false); // do not expand macros
                listRumorVariants.Add(variantItem);
            }

            RumorMillEntry entry;
            if (i >= listRumorMill.Count) // no entry was found -> create new
            {
                entry = new RumorMillEntry();
                entry.rumorType = RumorType.QuestProgressRumor;
                entry.questID = questID;
                entry.listRumorVariants = listRumorVariants;
                listRumorMill.Add(entry);
            }
            else // existing entry for questID -> replace
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
            TextFile.Token[] tokens = message.GetTextTokens(0, false); // do not expand macros
            dictQuestorPostQuestMessage[questID] = tokens;
        }

        public void RemoveQuestorPostQuestMessage(ulong questID)
        {
            if (dictQuestorPostQuestMessage.ContainsKey(questID))
            {
                dictQuestorPostQuestMessage.Remove(questID);
            }
        }

        public string GetKeySubjectLocationDirection()
        {
            string answer;

            markLocationOnMap = false; // when preprocessing messages in ExpandRandomTextRecord() do not reveal location accidentally (no %loc macro resolving)
            answer = ExpandRandomTextRecord(7333);

            return answer;
        }

        public string GetKeySubjectLocationOnMap()
        {
            string answer;
            markLocationOnMap = true; // only reveal on purpose
            answer = ExpandRandomTextRecord(7332);
            markLocationOnMap = false; // don't forget so future %loc macro resolving when preprocessing messages does not reveal location
            return answer;
        }

        public string GetKeySubjectLocationHint()
        {
            string answer;            

            // decide if npc gives directional hints or marks building on map, always only give directional hints if player is inside
            float randomFloat = UnityEngine.Random.Range(0.0f, 1.0f);
            if (randomFloat > ChanceToRevealLocationOnMap || GameManager.Instance.IsPlayerInside)
            {
                answer = GetKeySubjectLocationDirection();
            }
            else
            {
                answer = GetKeySubjectLocationOnMap();
            }

            return answer;
        }

        public string GetKeySubjectPersonHint()
        {
            string answer;

            string key = currentQuestionListItem.key;

            // override key if it is explicitly set (by quest person resources, place resources)
            if (currentQuestionListItem.key != String.Empty)
                key = currentQuestionListItem.key;
            
            int buildingKey = GameManager.Instance.TalkManager.GetBuildingKeyForPersonResource(currentQuestionListItem.questID, key);

            string backupKeySubject = this.currentKeySubject; // backup current key subject

            BuildingInfo buildingInfo = listBuildings.Find(x => x.buildingKey == buildingKey);
            this.currentKeySubject = buildingInfo.name;
            this.currentKeySubjectBuildingKey = buildingKey;

            markLocationOnMap = true; // only reveal on purpose
            answer = GetKeySubjectLocationHint();
            markLocationOnMap = false; // don't forget so future %loc macro resolving when preprocessing messages does not reveal location

            this.currentKeySubject = backupKeySubject; // restore old key subject           

            return answer;
        }

        public string GetDialogHint(ListItem listItem)
        {
            if (dictQuestInfo.ContainsKey(listItem.questID))
            {
                if (dictQuestInfo[listItem.questID].resourceInfo.ContainsKey(listItem.key))
                {
                    List<TextFile.Token[]> answers = dictQuestInfo[listItem.questID].resourceInfo[listItem.key].anyInfoAnswers;
                    return GetAnswerFromTokensArray(listItem.questID, answers);
                }
            }
            return TextManager.Instance.GetText(textDatabase, "resolvingError"); // error case - should never ever occur
        }

        public string GetDialogHint2(ListItem listItem)
        {
            if (dictQuestInfo.ContainsKey(listItem.questID))
            {
                if (dictQuestInfo[listItem.questID].resourceInfo.ContainsKey(listItem.key))
                {
                    List<TextFile.Token[]> answers;
                    if (npcData.isSpyMaster) // spymaster only gives "true" answers (anyinfo messages) also for %hnt2 (note: intended that consoleCommandFlag_npcsKnowEverything does not apply here)
                        answers = dictQuestInfo[listItem.questID].resourceInfo[listItem.key].anyInfoAnswers;
                    else // everybody else gives rumors here for %hnt2
                        answers = dictQuestInfo[listItem.questID].resourceInfo[listItem.key].rumorsAnswers;

                    if (answers.Count == 0) // if no rumors are available, fall back to anyInfoAnswers
                        answers = dictQuestInfo[listItem.questID].resourceInfo[listItem.key].anyInfoAnswers;
                    return GetAnswerFromTokensArray(listItem.questID, answers);
                }
            }
            return TextManager.Instance.GetText(textDatabase, "resolvingError"); // error case - should never ever occur
        }

        public string GetGuildNPC()
        {
            return GameManager.Instance.GuildManager.GetGuild((int)GameManager.Instance.PlayerEnterExit.FactionID).GetGuildName();
        }

        public string GetFactionPC()
        {
            if (npcData.guildGroup == FactionFile.GuildGroups.HolyOrder)
                return GetFactionName();
            else
                return GetGuildNPC();
        }

        public string GetFactionName()
        {
            if (npcData.guildGroup == FactionFile.GuildGroups.HolyOrder)
            {
                Temple temple = (Temple)GameManager.Instance.GuildManager.GetGuild(npcData.guildGroup, (int)GameManager.Instance.PlayerEnterExit.FactionID);
                MacroDataSource mcp = temple.GetMacroDataSource();
                return mcp.FactionOrderName();
            }
            return TextManager.Instance.GetText(textDatabase, "resolvingError");
        }

        public string GetHonoric()
        {
            if (GameManager.Instance.PlayerEntity.Gender == Genders.Male)
                return TextManager.Instance.GetText(textDatabase, "Sir");
            else
                return TextManager.Instance.GetText(textDatabase, "Ma'am");
        }

        public string GetOldLeaderFateString(int index)
        {
            return TextManager.Instance.GetText(textDatabase, String.Format("oldLeaderFate{0}", index));
        }

        public string GetAnswerWhereIs(TalkManager.ListItem listItem)
        {
            if (listItem.npcKnowledgeAboutItem == NPCKnowledgeAboutItem.NotSet)
            {
                // decide here if npcs knows question's answer (spymaster always knows)
                float randomFloat = UnityEngine.Random.Range(0.0f, 1.0f);
                if (randomFloat < npcData.chanceKnowsSomethingAboutWhereIs || npcData.isSpyMaster || consoleCommandFlag_npcsKnowEverything)
                    listItem.npcKnowledgeAboutItem = NPCKnowledgeAboutItem.KnowsAboutItem;                
                else
                    listItem.npcKnowledgeAboutItem = NPCKnowledgeAboutItem.DoesNotKnowAboutItem;
            }

            // test if npc is asked about building and is in the same building (also for quest persons) -> then he/she should know about building
            if (listItem.questionType == QuestionType.LocalBuilding || listItem.questionType == QuestionType.QuestLocation)
            {
                if (GameManager.Instance.IsPlayerInside)
                {
                    if (GameManager.Instance.PlayerEnterExit.ExteriorDoors.Length > 0 && listItem.buildingKey == GameManager.Instance.PlayerEnterExit.ExteriorDoors[0].buildingKey)
                    {
                        listItem.npcKnowledgeAboutItem = NPCKnowledgeAboutItem.KnowsAboutItem;
                        listItem.npcInSameBuildingAsTopic = true;
                    }
                    else if (GameManager.Instance.IsPlayerInsideCastle || GameManager.Instance.IsPlayerInsideDungeon) // in dungeon
                    {
                        DaggerfallDungeon.DungeonSummary ds = GameManager.Instance.PlayerEnterExit.Dungeon.Summary;
                        string dungeonName = "";
                        if (ds.RegionName == "Daggerfall" && ds.LocationName == "Daggerfall")
                            dungeonName = ExpandRandomTextRecord(475);
                        else if (ds.RegionName == "Wayrest" && ds.LocationName == "Wayrest")
                            dungeonName = ExpandRandomTextRecord(476);
                        else if (ds.RegionName == "Sentinel" && ds.LocationName == "Sentinel")
                            dungeonName = ExpandRandomTextRecord(477);
                        else
                        {
                            dungeonName = GameManager.Instance.PlayerEnterExit.Dungeon.Summary.LocationName;
                        }

                        dungeonName = dungeonName.TrimEnd('.'); // remove character '.' from castle text record entry

                        if (dungeonName == listItem.caption)
                        {
                            listItem.npcKnowledgeAboutItem = NPCKnowledgeAboutItem.KnowsAboutItem;
                            listItem.npcInSameBuildingAsTopic = true;
                        }
                    }
                }
            }


            if (listItem.npcKnowledgeAboutItem == NPCKnowledgeAboutItem.DoesNotKnowAboutItem)
            {
                // messages if npc does not know
                if (reactionToPlayer >= minInfoVeryLikeReaction)
                    return getRecordIdByNpcsSocialGroup(veryLikePlayerDoesNotKnowWhereIsDefault, veryLikePlayerDoesNotKnowWhereIsGuildMembers, veryLikePlayerDoesNotKnowWhereIsMerchants, veryLikePlayerDoesNotKnowWhereIsScholars, veryLikePlayerDoesNotKnowWhereIsNobility, veryLikePlayerDoesNotKnowWhereIsUnderworld);
                else if (reactionToPlayer >= minInfoLikeReaction)
                    return getRecordIdByNpcsSocialGroup(likePlayerDoesNotKnowWhereIsDefault, likePlayerDoesNotKnowWhereIsGuildMembers, likePlayerDoesNotKnowWhereIsMerchants, likePlayerDoesNotKnowWhereIsScholars, likePlayerDoesNotKnowWhereIsNobility, likePlayerDoesNotKnowWhereIsUnderworld);
                else if (reactionToPlayer >= minInfoNeutralReaction)
                    return getRecordIdByNpcsSocialGroup(neutralToPlayerDoesNotKnowWhereIsDefault, neutralToPlayerDoesNotKnowWhereIsGuildMembers, neutralToPlayerDoesNotKnowWhereIsMerchants, neutralToPlayerDoesNotKnowWhereIsScholars, neutralToPlayerDoesNotKnowWhereIsNobility, neutralToPlayerDoesNotKnowWhereIsUnderworld);
                else
                    return getRecordIdByNpcsSocialGroup(dislikePlayerDoesNotKnowWhereIsDefault, dislikePlayerDoesNotKnowWhereIsGuildMembers, dislikePlayerDoesNotKnowWhereIsMerchants, dislikePlayerDoesNotKnowWhereIsScholars, dislikePlayerDoesNotKnowWhereIsNobility, dislikePlayerDoesNotKnowWhereIsUnderworld);
            }
            else
            {
                // check if npc is in same building if topic is building
                if (currentQuestionListItem.questionType == QuestionType.LocalBuilding && currentQuestionListItem.npcInSameBuildingAsTopic)
                    return String.Format(TextManager.Instance.GetText(textDatabase, "YouAreInSameBuilding"), currentQuestionListItem.caption);

                // check if npc is in same building as quest person when asking about quest person via "Where is"->"Person"
                if (currentQuestionListItem.questionType == QuestionType.Person)
                {
                    string key = currentQuestionListItem.key;
                    int buildingKey = GameManager.Instance.TalkManager.GetBuildingKeyForPersonResource(currentQuestionListItem.questID, key);

                    if (GameManager.Instance.IsPlayerInside && GameManager.Instance.PlayerEnterExit.ExteriorDoors.Length > 0 && buildingKey == GameManager.Instance.PlayerEnterExit.ExteriorDoors[0].buildingKey)
                    {
                        currentQuestionListItem.npcInSameBuildingAsTopic = true;

                        string buildingName = GameManager.Instance.TalkManager.GetBuildingNameForBuildingKey(buildingKey);

                        if (buildingName != string.Empty)
                            return String.Format(TextManager.Instance.GetText(textDatabase, "NpcInSameBuilding"), currentQuestionListItem.caption, buildingName);
                        else
                            return TextManager.Instance.GetText(textDatabase, "resolvingError");
                    }
                }


                // location related messages if npc knows
                if (reactionToPlayer >= minInfoVeryLikeReaction)
                    return getRecordIdByNpcsSocialGroup(veryLikePlayerAnswerWhereIsDefault, veryLikePlayerAnswerWhereIsGuildMembers, veryLikePlayerAnswerWhereIsMerchants, veryLikePlayerAnswerWhereIsScholars, veryLikePlayerAnswerWhereIsNobility, veryLikePlayerAnswerWhereIsUnderworld);
                else if (reactionToPlayer >= minInfoLikeReaction)
                    return getRecordIdByNpcsSocialGroup(likePlayerAnswerWhereIsDefault, likePlayerAnswerWhereIsGuildMembers, likePlayerAnswerWhereIsMerchants, likePlayerAnswerWhereIsScholars, likePlayerAnswerWhereIsNobility, likePlayerAnswerWhereIsUnderworld);
                else if (reactionToPlayer >= minInfoNeutralReaction)
                    return getRecordIdByNpcsSocialGroup(neutralToPlayerAnswerWhereIsDefault, neutralToPlayerAnswerWhereIsGuildMembers, neutralToPlayerAnswerWhereIsMerchants, neutralToPlayerAnswerWhereIsScholars, neutralToPlayerAnswerWhereIsNobility, neutralToPlayerAnswerWhereIsUnderworld);
                else
                    return getRecordIdByNpcsSocialGroup(dislikePlayerAnswerWhereIsDefault, dislikePlayerAnswerWhereIsGuildMembers, dislikePlayerAnswerWhereIsMerchants, dislikePlayerAnswerWhereIsScholars, dislikePlayerAnswerWhereIsNobility, dislikePlayerAnswerWhereIsUnderworld);
            }
        }

        public string GetAnswerAboutRegionalBuilding(TalkManager.ListItem listItem)
        {
            if (GetRegionalLocationCityName(listItem))
                return ExpandRandomTextRecord(10);
            else
                return ExpandRandomTextRecord(11);
        }

        public bool GetRegionalLocationCityName(TalkManager.ListItem listItem)
        {
            byte[] lookUpIndexes = { 0x0D, 0x0D, 0x0D, 0x0D, 0x0D, 0x0D, 0x0D, 0x0D, 0x19, 0x1A, 0x1B, 0x1D, 0x1E, 0x1F, 0x20,
                                     0x21, 0x22, 0x23, 0x24, 0x27, 0x00, 0x0B, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x0A };

            DFLocation location = new DFLocation();
            if (GetLocationWithRegionalBuilding(lookUpIndexes[listItem.index], FactionsAndBuildings[listItem.index], ref location))
            {
                LocationOfRegionalBuilding = location.Name;
                return true;
            }
            else
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
            else
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

        public string GetAnswerText(TalkManager.ListItem listItem)
        {
            string answer = "";
            currentQuestionListItem = listItem;
            switch (listItem.questionType)
            {
                case QuestionType.NoQuestion:
                default:
                    break;
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
                    answer = GetAnswerWhereIs(listItem); // actually never reached since there are no "where is"-type questions for things in vanilla df
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
                        answer = ExpandRandomTextRecord(8078); // TODO: find when 8075 should be used
                        break;
                    }
                    else
                    {
                        SetRandomQuestor(); // Pick a random Work questor from the pool
                        answer = ExpandRandomTextRecord(8076); // TODO: find when 8077 should be used
                        break;
                    }
            }

            numQuestionsAsked++;
            questionOpeningText = ""; // reset questionOpeningText so that it is newly created for next question
            return answer;
        }

        public string GetAnswerTellMeAboutTopic(TalkManager.ListItem listItem, float chanceNPCknowsSomthing)
        {
            if (listItem.npcKnowledgeAboutItem == NPCKnowledgeAboutItem.NotSet)
            {
                // decide here if npcs knows question's answer (spymaster always knows)
                float randomFloat = UnityEngine.Random.Range(0.0f, 1.0f);
                if (randomFloat < chanceNPCknowsSomthing || npcData.isSpyMaster || consoleCommandFlag_npcsKnowEverything)
                    listItem.npcKnowledgeAboutItem = NPCKnowledgeAboutItem.KnowsAboutItem;
                else
                    listItem.npcKnowledgeAboutItem = NPCKnowledgeAboutItem.DoesNotKnowAboutItem;
            }

            if (listItem.npcKnowledgeAboutItem == NPCKnowledgeAboutItem.DoesNotKnowAboutItem || (npcData.numAnswersGivenTellMeAboutOrRumors >= maxNumAnswersNpcGivesTellMeAboutOrRumors && !npcData.isSpyMaster && !consoleCommandFlag_npcsKnowEverything))
            {
                // messages if npc does not know
                if (reactionToPlayer >= minInfoVeryLikeReaction)
                    return getRecordIdByNpcsSocialGroup(veryLikePlayerDoesNotKnowTellMeAboutDefault, veryLikePlayerDoesNotKnowTellMeAboutGuildMembers, veryLikePlayerDoesNotKnowTellMeAboutMerchants, veryLikePlayerDoesNotKnowTellMeAboutScholars, veryLikePlayerDoesNotKnowTellMeAboutNobility, veryLikePlayerDoesNotKnowTellMeAboutUnderworld);
                else if (reactionToPlayer >= minInfoLikeReaction)
                    return getRecordIdByNpcsSocialGroup(likePlayerDoesNotKnowTellMeAboutDefault, likePlayerDoesNotKnowTellMeAboutGuildMembers, likePlayerDoesNotKnowTellMeAboutMerchants, likePlayerDoesNotKnowTellMeAboutScholars, likePlayerDoesNotKnowTellMeAboutNobility, likePlayerDoesNotKnowTellMeAboutUnderworld);
                else if (reactionToPlayer >= minInfoNeutralReaction)
                    return getRecordIdByNpcsSocialGroup(neutralToPlayerDoesNotKnowTellMeAboutDefault, neutralToPlayerDoesNotKnowTellMeAboutGuildMembers, neutralToPlayerDoesNotKnowTellMeAboutMerchants, neutralToPlayerDoesNotKnowTellMeAboutScholars, neutralToPlayerDoesNotKnowTellMeAboutNobility, neutralToPlayerDoesNotKnowTellMeAboutUnderworld);
                else
                    return getRecordIdByNpcsSocialGroup(dislikePlayerDoesNotKnowTellMeAboutDefault, dislikePlayerDoesNotKnowTellMeAboutGuildMembers, dislikePlayerDoesNotKnowTellMeAboutMerchants, dislikePlayerDoesNotKnowTellMeAboutScholars, dislikePlayerDoesNotKnowTellMeAboutNobility, dislikePlayerDoesNotKnowTellMeAboutUnderworld);
            }
            else
            {
                npcData.numAnswersGivenTellMeAboutOrRumors++;

                // location related messages if npc knows
                if (reactionToPlayer >= minInfoVeryLikeReaction)
                    return getRecordIdByNpcsSocialGroup(veryLikePlayerAnswerTellMeAboutDefault, veryLikePlayerAnswerTellMeAboutGuildMembers, veryLikePlayerAnswerTellMeAboutMerchants, veryLikePlayerAnswerTellMeAboutScholars, veryLikePlayerAnswerTellMeAboutNobility, veryLikePlayerAnswerTellMeAboutUnderworld);
                else if (reactionToPlayer >= minInfoLikeReaction)
                    return getRecordIdByNpcsSocialGroup(likePlayerAnswerTellMeAboutDefault, likePlayerAnswerTellMeAboutGuildMembers, likePlayerAnswerTellMeAboutMerchants, likePlayerAnswerTellMeAboutScholars, likePlayerAnswerTellMeAboutNobility, likePlayerAnswerTellMeAboutUnderworld);
                else if (reactionToPlayer >= minInfoNeutralReaction)
                    return getRecordIdByNpcsSocialGroup(neutralToPlayerAnswerTellMeAboutDefault, neutralToPlayerAnswerTellMeAboutGuildMembers, neutralToPlayerAnswerTellMeAboutMerchants, neutralToPlayerAnswerTellMeAboutScholars, neutralToPlayerAnswerTellMeAboutNobility, neutralToPlayerAnswerTellMeAboutUnderworld);
                else
                    return getRecordIdByNpcsSocialGroup(dislikePlayerAnswerTellMeAboutDefault, dislikePlayerAnswerTellMeAboutGuildMembers, dislikePlayerAnswerTellMeAboutMerchants, dislikePlayerAnswerTellMeAboutScholars, dislikePlayerAnswerTellMeAboutNobility, dislikePlayerAnswerTellMeAboutUnderworld);
            }
        }


        public void ForceTopicListsUpdate()
        {
            AssembleTopicLists();
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
                // undiscover residences when they are a quest resource (named residence) when creating quest resource
                // otherwise previously discovered residences will automatically show up on the automap when used in a quest            
                UndiscoverQuestResidence(questID, resourceName, questResourceInfo);
            }

            // update topic lists
            rebuildTopicLists = true;

            // update rumor mill
            if (questResourceInfo.rumorsAnswers != null)
                GameManager.Instance.TalkManager.AddQuestRumorToRumorMill(questID, questResourceInfo.rumorsAnswers);
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
                Debug.Log(String.Format("AddDialogLinkForQuestInfoResource() could not find quest with questID {0}", questID));
                return;
            }

            QuestResourceInfo questResource;
            if (questResources.resourceInfo.ContainsKey(resourceName))
            {
                 questResource = questResources.resourceInfo[resourceName];
            }
            else
            {
                Debug.Log(String.Format("AddDialogLinkForQuestInfoResource() could not find a quest info resource with name {0}", resourceName));
                return;
            }
            
            switch (linkedResourceType)
            {
                case QuestInfoResourceType.NotSet:
                    // no linked resource specified - don't create entries in linked resource lists but proceed (leave switch statement) so flag "availableForDialog" is set to false for resource
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
            
            // "hide" quest resource dialog entry
            questResource.availableForDialog = false;

            if (linkedResourceName != null)
            {
                // "hide" linked quest resource dialog entry as well
                if (questResources.resourceInfo.ContainsKey(linkedResourceName))
                    questResources.resourceInfo[linkedResourceName].availableForDialog = false;
                else
                    Debug.Log("AddDialogLinkForQuestInfoResource(): linked quest resource not found");
            }

            // update topic list
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
                Debug.Log(String.Format("AddDialogLinkForQuestInfoResource() could not find quest with questID {0}", questID));
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
                    Debug.Log(String.Format("AddDialogLinkForQuestInfoResource() could not find a quest info resource with name {0}", resourceName));
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

                    // undiscover residences when they are a quest resource (named residence) when "add dialog" is done for this quest resource
                    // otherwise previously discovered residences will automatically show up on the automap when used in a quest
                    UndiscoverQuestResidence(questID, resourceName, questResourceInfo);
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

            // update topic lists
            if (instantRebuildTopicLists == true)
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

            // update topic lists
            rebuildTopicLists = true;
        }

        public int GetBuildingKeyForPersonResource(ulong questID, string resourceName)
        {
            if (!dictQuestInfo.ContainsKey(questID))
            {
                throw new Exception(String.Format("GetBuildingKeyForPersonResource(): could not find quest with questID {0}", questID));                
            }

            QuestResources questResources = dictQuestInfo[questID];

            if (resourceName == null || resourceName == "")
                throw new Exception("GetBuildingKeyForPersonResource(): - no valid resourceName was provided.");

            QuestResourceInfo questResourceInfo;
            if (questResources.resourceInfo.ContainsKey(resourceName))
            {
                questResourceInfo = questResources.resourceInfo[resourceName];
            }
            else
            {
                throw new Exception(String.Format("GetBuildingKeyForPersonResource(): could not find resource with resourceName {1} for quest with questID {0}", questID, resourceName));
            }

            if (questResourceInfo.resourceType != QuestInfoResourceType.Person)
                throw new Exception(String.Format("GetBuildingKeyForPersonResource(): resource is not of type Person but was expected to be"));

            Quest quest = GameManager.Instance.QuestMachine.GetQuest(questID);

            Questing.Person person = (Questing.Person)questResourceInfo.questResource;

            if (person.IsQuestor)
            {
                return person.QuestorData.buildingKey;
            }
            else
            {
                Symbol assignedPlaceSymbol = person.GetAssignedPlaceSymbol();
                if (assignedPlaceSymbol == null)
                    throw new Exception(String.Format("GetBuildingKeyForPersonResource(): resource is not of type Person but was expected to be"));

                Place assignedPlace = quest.GetPlace(assignedPlaceSymbol);

                return assignedPlace.SiteDetails.buildingKey;
            }
        }

        public DFLocation.BuildingTypes GetBuildingTypeForBuildingKey(int buildingKey)
        {
            if (listBuildings == null)
                GetBuildingList();
            List<BuildingInfo> matchingBuildings = listBuildings.FindAll(x => x.buildingKey == buildingKey);
            if (matchingBuildings.Count == 0)
                throw new Exception(String.Format("GetBuildingTypeForBuildingKey(): no building with the queried key found"));
            else if (matchingBuildings.Count > 1 )
                throw new Exception(String.Format("GetBuildingTypeForBuildingKey(): more than one building with the queried key found"));
            return matchingBuildings[0].buildingType;
        }

        public string GetBuildingNameForBuildingKey(int buildingKey)
        {
            if (listBuildings == null)
                GetBuildingList();
            List<BuildingInfo> matchingBuildings = listBuildings.FindAll(x => x.buildingKey == buildingKey);
            if (matchingBuildings.Count == 0)
                throw new Exception(String.Format("GetBuildingNameForBuildingKey(): no building with the queried key found"));
            else if (matchingBuildings.Count > 1)
                throw new Exception(String.Format("GetBuildingNameForBuildingKey(): more than one building with the queried key found"));
            return matchingBuildings[0].name;
        }

        public bool IsBuildingQuestResource(int buildingKey, ref string overrideBuildingName, ref bool pcLearnedAboutExistence, ref bool receivedDirectionalHints, ref bool locationWasMarkedOnMapByNPC)
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
                    QuestResources questInfo = dictQuestInfo[questID]; // get questInfo containing orphaned list of quest resources

                    QuestResource[] questResources = quest.GetAllResources(typeof(Place)); // get list of place quest resources
                    for (int i = 0; i < questResources.Length; i++)
                    {
                        Questing.Place place = (Questing.Place)(questResources[i]);
                        string key = place.Symbol.Name;

                        if (place.SiteDetails.buildingKey != buildingKey)
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
        /// Gets conversation dictionary for save.
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
        /// Restores conversation dictionary for load.
        /// </summary>
        public void RestoreConversationData(SaveDataConversation data)
        {
            if (data == null)
                data = new SaveDataConversation();

            dictQuestInfo = data.dictQuestInfo;
            if (dictQuestInfo == null)
                dictQuestInfo = new Dictionary<ulong, QuestResources>();

            ulong[] questIDs = GameManager.Instance.QuestMachine.GetAllQuests();
            
            // search for orphaned entries in dictQuestInfo
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
                Debug.Log(String.Format("save data contains orphaned quest info for quest with id {0}. Removing these entries...", idsToDelete[i]));
                dictQuestInfo.Remove(idsToDelete[i]);
            }   
            
            // for each running quest
            foreach (ulong questID in questIDs)
            {
                Quest quest = GameManager.Instance.QuestMachine.GetQuest(questID);
                // test if there exists a dictQuestInfo entry
                if (dictQuestInfo.ContainsKey(questID))
                {
                    //if yes relink quest resources

                    QuestResources questInfo = dictQuestInfo[questID]; // get questInfo containing orphaned list of quest resources
                    
                    QuestResource[] questResources = quest.GetAllResources(typeof(Person)); // get list of person quest resources
                    for (int i=0; i < questResources.Length; i++)
                    {
                        Questing.Person person = (Questing.Person)(questResources[i]);

                        string key = person.Symbol.Name;

                        if (questInfo.resourceInfo.ContainsKey(key)) // if list of quest resources contains a matching entry
                        {
                            questInfo.resourceInfo[key].questResource = person; // update (relink) it
                        }
                    }

                    questResources = quest.GetAllResources(typeof(Place)); // get list of place quest resources                   
                    for (int i = 0; i < questResources.Length; i++)
                    {
                        Questing.Place place = (Questing.Place)(questResources[i]);

                        string key = place.Symbol.Name;

                        if (questInfo.resourceInfo.ContainsKey(key)) // if list of quest resources contains a matching entry
                        {
                            questInfo.resourceInfo[key].questResource = place; // update (relink) it
                        }
                    }

                    questResources = quest.GetAllResources(typeof(Item)); // get list of item quest resources                   
                    for (int i = 0; i < questResources.Length; i++)
                    {
                        Questing.Item item = (Questing.Item)(questResources[i]);

                        string key = item.Symbol.Name;

                        if (questInfo.resourceInfo.ContainsKey(key)) // if list of quest resources contains a matching entry
                        {
                            questInfo.resourceInfo[key].questResource = item; // update (relink) it
                        }
                    }

                }
            }

            listRumorMill = data.listRumorMill;
            if (listRumorMill == null || listRumorMill.Count == 0)
            {
                SetupRumorMill();
            }

            // search for orphaned entries in rumor mill
            for (int i = listRumorMill.Count - 1; i >= 0; i--)
            {
                ulong questID = listRumorMill[i].questID;
                if (GameManager.Instance.QuestMachine.GetQuest(questID) == null)
                {
                    Debug.Log(String.Format("save data contains orphaned rumors for quest with id {0}. Removing these rumors...", questID));
                    listRumorMill.Remove(listRumorMill[i]);
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

            // update topic list
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
            const int dialogRejectionTextId = 8571;
            const int youGetNoResponseTextId = 7205;

            const int isInSameGuildDislikePlayerRefusingToTalkTextId = 8552;

            const int isInSameHolyOrderDislikePlayerRefusingToTalkTextId = 8555;

            if (reactionToPlayer >= -20)
            {
                DaggerfallUI.UIManager.PushWindow(DaggerfallUI.Instance.TalkWindow);

                // reset npc knowledge, for now it resets every time the npc has changed (player talked to new npc)
                // TODO: match classic daggerfall - in classic npc remember their knowledge about topics for their time of existence
                if (!sameTalkTargetAsBefore)
                    ResetNPCKnowledge();
            }
            else
            {
                if (alreadyRejectedOnce)
                {
                    DaggerfallUI.MessageBox(youGetNoResponseTextId);
                }
                else
                {
                    string responseText;
                    if (GameManager.Instance.GuildManager.GetGuild(npcData.guildGroup, (int)GameManager.Instance.PlayerEnterExit.FactionID).IsMember())
                    {
                        if (npcData.guildGroup == FactionFile.GuildGroups.HolyOrder) // holy orders use message 8554
                            responseText = DaggerfallUnity.Instance.TextProvider.GetRandomText(isInSameHolyOrderDislikePlayerRefusingToTalkTextId);
                        else // all other guilds (including Knightly Orders) seem to use message 8552
                            responseText = DaggerfallUnity.Instance.TextProvider.GetRandomText(isInSameGuildDislikePlayerRefusingToTalkTextId);
                    }
                    else
                    {
                        responseText = DaggerfallUnity.Instance.TextProvider.GetRandomText(dialogRejectionTextId);
                    }
                    DaggerfallUI.MessageBox(responseText);
                    alreadyRejectedOnce = true;
                }
            }
        }


        private void SetupRumorMill()
        {
            if (listRumorMill == null)
                listRumorMill = new List<RumorMillEntry>();
            if (listRumorMill.Count == 0)
            {
                for (int i = 0; i < 10; i++) // setup 10 random common rumors (this is very early work in progress)
                {
                    RumorMillEntry entry = new RumorMillEntry();

                    TextFile.Token[] tokens;
                    int randomNum = UnityEngine.Random.Range(0, 20);
                    if (randomNum >= 0 && randomNum <= 9)
                        tokens = DaggerfallUnity.Instance.TextProvider.GetRandomTokens(1400 + randomNum);
                    else if (randomNum == 10)
                        tokens = DaggerfallUnity.Instance.TextProvider.GetRandomTokens(1456);
                    else if (randomNum >= 11 && randomNum <= 15)
                        tokens = DaggerfallUnity.Instance.TextProvider.GetRandomTokens(1480);
                    else if (randomNum >= 16 && randomNum <= 20)
                        tokens = DaggerfallUnity.Instance.TextProvider.GetRandomTokens(1481);
                    else
                        tokens = DaggerfallUnity.Instance.TextProvider.GetRandomTokens(1457);
                    entry.rumorType = RumorType.CommonRumor;
                    entry.listRumorVariants = new List<TextFile.Token[]>();
                    entry.listRumorVariants.Add(tokens);

                    listRumorMill.Add(entry);
                }
            }
        }

        private void GetBuildingList()
        {
            listBuildings = new List<BuildingInfo>();

            ContentReader.MapSummary mapSummary;
            DFPosition mapPixel = GameManager.Instance.PlayerGPS.CurrentMapPixel;
            if (!DaggerfallUnity.Instance.ContentReader.HasLocation(mapPixel.X, mapPixel.Y, out mapSummary))
            {
                // no location found
                return; // do nothing
            }
            DFLocation location = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetLocation(mapSummary.RegionIndex, mapSummary.MapIndex);
            if (!location.Loaded)
            {
                // Location not loaded, something went wrong
                DaggerfallUnity.LogMessage("error when loading location for in TalkManager.GetBuildingList", true);
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
                            string locationName = BuildingNames.GetName(buildingSummary.NameSeed, buildingSummary.BuildingType, buildingSummary.FactionId, location.Name, location.RegionName);
                            BuildingInfo item;
                            item.buildingType = buildingSummary.BuildingType;
                            item.name = locationName;
                            item.buildingKey = buildingSummary.buildingKey;
                            // compute building position in map coordinate system                     
                            float xPosBuilding = blockLayout[index].rect.xpos + (int)(buildingSummary.Position.x / (BlocksFile.RMBDimension * MeshReader.GlobalScale) * ExteriorAutomap.blockSizeWidth) - GameManager.Instance.ExteriorAutomap.LocationWidth * ExteriorAutomap.blockSizeWidth * 0.5f;
                            float yPosBuilding = blockLayout[index].rect.ypos + (int)(buildingSummary.Position.z / (BlocksFile.RMBDimension * MeshReader.GlobalScale) * ExteriorAutomap.blockSizeHeight) - GameManager.Instance.ExteriorAutomap.LocationHeight * ExteriorAutomap.blockSizeHeight * 0.5f;
                            item.position = new Vector2(xPosBuilding, yPosBuilding);
                            if (item.buildingKey != 0)
                                listBuildings.Add(item);
                        }
                        catch (Exception e)
                        {
                            string exceptionMessage = String.Format("exception occured in function BuildingNames.GetName (exception message: " + e.Message + @") with params: 
                                                                        seed: {0}, type: {1}, factionID: {2}, locationName: {3}, regionName: {4}",
                                                                        buildingSummary.NameSeed, buildingSummary.BuildingType, buildingSummary.FactionId, location.Name, location.RegionName);
                            DaggerfallUnity.LogMessage(exceptionMessage, true);
                        }

                        // Populate potential merchant questors in this building
                        if (populateQuestors)
                        {
                            PersistentFactionData factions = GameManager.Instance.PlayerEntity.FactionData;
                            DFBlock.RmbBlockPeopleRecord[] buildingNpcs = blocks[index].RmbBlock.SubRecords[i].Interior.BlockPeopleRecords;
                            for (int p = 0; p < buildingNpcs.Length; p++)
                            {
                                FactionFile.FactionData factionData;
                                // Assuming commoners (people of region) are NPCs with have faction ids of zero.
                                if (buildingNpcs[p].FactionID == 0)
                                {
                                    // Get regional people faction id
                                    FactionFile.FactionData[] factionsData = factions.FindFactions(
                                        (int)FactionFile.FactionTypes.People, (int)FactionFile.SocialGroups.Commoners,
                                        -1, GameManager.Instance.PlayerGPS.CurrentRegionIndex);
                                    if (factionsData.Length == 1)
                                        factionData = factionsData[0];
                                    else
                                        continue;
                                }
                                else
                                {
                                    factions.GetFactionData(buildingNpcs[p].FactionID, out factionData);
                                }
                                FactionFile.SocialGroups socialGroup = (FactionFile.SocialGroups) factionData.sgroup;
                                if (socialGroup == FactionFile.SocialGroups.Merchants ||
                                    socialGroup == FactionFile.SocialGroups.Commoners ||
                                    socialGroup == FactionFile.SocialGroups.Nobility)
                                {
                                    // 25% chance that an npc will offer quest.
                                    // TODO: how does classic decide?
                                    int randomChance = UnityEngine.Random.Range(0, 4);
                                    workStats[(int)socialGroup]++;
                                    if (randomChance < 3)
                                        continue;

                                    StaticNPC.NPCData npcData = new StaticNPC.NPCData();
                                    StaticNPC.SetLayoutData(ref npcData,
                                                            buildingNpcs[p].XPos, buildingNpcs[p].YPos, buildingNpcs[p].ZPos,
                                                            buildingNpcs[p].Flags,
                                                            factionData.id,
                                                            buildingNpcs[p].TextureArchive,
                                                            buildingNpcs[p].TextureRecord,
                                                            buildingNpcs[p].Position,
                                                            buildingSummary.buildingKey);

                                    if (!npcsWithWork.ContainsKey(npcData.nameSeed))
                                    {
                                        npcData.buildingKey = buildingSummary.buildingKey;
                                        npcData.nameBank = GameManager.Instance.PlayerGPS.GetNameBankOfCurrentRegion();
                                        NpcWorkEntry npcWork = new NpcWorkEntry {
                                            npc = npcData,
                                            socialGroup = socialGroup,
                                            buildingName = BuildingNames.GetName(buildingSummary.NameSeed, buildingSummary.BuildingType, buildingSummary.FactionId, location.Name, location.RegionName)
                                        };
                                        if (npcWork.buildingName == string.Empty)
                                        {
                                            workStats[(int)socialGroup+8]++;
                                            continue;
                                        }
                                        workStats[(int)socialGroup+4]++;
                                        npcsWithWork.Add(npcData.nameSeed, npcWork);
                                        selectedNpcWorkKey = npcData.nameSeed;
                                        Debug.LogFormat("Added {4} questor: ns={0} bk={1} name={2} building={3} factionId={5}", npcData.nameSeed, buildingSummary.buildingKey, GetQuestorName(), npcWork.buildingName, socialGroup, npcData.factionID);
                                    }
                                }
                            }
                            exteriorUsedForQuestors = GameManager.Instance.PlayerGPS.CurrentLocationIndex; // create only once for current location
                        }
                    }
                }
            }
            if (populateQuestors)
                Debug.LogFormat("Populated work availiable from NPCs. (allocated+noBuilding/candidates) Merchants: {0}(+{6})/{1}, Commoners: {2}(+{7})/{3}, Nobles: {4}(+{8})/{5}", 
                    workStats[5], workStats[1], workStats[4], workStats[0], workStats[7], workStats[3], workStats[9], workStats[8], workStats[11]);
        }

        private string BuildingTypeToGroupString(DFLocation.BuildingTypes buildingType)
        {
            switch (buildingType)
            {
                case DFLocation.BuildingTypes.Alchemist:
                    return (TextManager.Instance.GetText(textDatabase, "Alchemists"));
                case DFLocation.BuildingTypes.Armorer:
                    return (TextManager.Instance.GetText(textDatabase, "Armorers"));
                case DFLocation.BuildingTypes.Bank:
                    return (TextManager.Instance.GetText(textDatabase, "Banks"));
                case DFLocation.BuildingTypes.Bookseller:
                    return (TextManager.Instance.GetText(textDatabase, "Bookstores"));
                case DFLocation.BuildingTypes.ClothingStore:
                    return (TextManager.Instance.GetText(textDatabase, "Clothingstores"));
                case DFLocation.BuildingTypes.GemStore:
                    return (TextManager.Instance.GetText(textDatabase, "Gemstores"));
                case DFLocation.BuildingTypes.GeneralStore:
                    return (TextManager.Instance.GetText(textDatabase, "Generalstores"));
                case DFLocation.BuildingTypes.GuildHall:
                    return (TextManager.Instance.GetText(textDatabase, "Guilds"));
                case DFLocation.BuildingTypes.Library:
                    return (TextManager.Instance.GetText(textDatabase, "Libraries"));
                case DFLocation.BuildingTypes.PawnShop:
                    return (TextManager.Instance.GetText(textDatabase, "Pawnshops"));
                case DFLocation.BuildingTypes.Tavern:
                    return (TextManager.Instance.GetText(textDatabase, "Taverns"));
                case DFLocation.BuildingTypes.WeaponSmith:
                    return (TextManager.Instance.GetText(textDatabase, "Weaponsmiths"));
                case DFLocation.BuildingTypes.Temple:
                    return (TextManager.Instance.GetText(textDatabase, "Localtemples"));
                default:
                    return ("");
            }
        }

        private bool CheckBuildingTypeInSkipList(DFLocation.BuildingTypes buildingType)
        {
            if (buildingType == DFLocation.BuildingTypes.AllValid ||
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
                buildingType == DFLocation.BuildingTypes.Town4)
                return true;
            return false;
        }

        private void UndiscoverQuestResidence(ulong questID, string resourceName, QuestResourceInfo questResourceInfo)
        {
            Quest quest = GameManager.Instance.QuestMachine.GetQuest(questID);
            if (quest == null)
                return;

            if (questResourceInfo.resourceType == QuestInfoResourceType.Location)
            {
                QuestResource[] allQuestResources = quest.GetAllResources(typeof(Place)); // get list of place quest resources
                for (int i = 0; i < allQuestResources.Length; i++)
                {
                    Questing.Place place = (Questing.Place)(allQuestResources[i]);
                    int buildingKey = place.SiteDetails.buildingKey;
                    string name = place.Symbol.Name;

                    if (name != resourceName)
                        continue;

                    GameManager.Instance.PlayerGPS.UndiscoverBuilding(buildingKey, true);
                }
            }
        }

        private void ResetNPCKnowledgeInTopicListRecursively(List<ListItem> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                list[i].npcKnowledgeAboutItem = NPCKnowledgeAboutItem.NotSet;
                list[i].npcInSameBuildingAsTopic = false;
                if (list[i].type == ListItemType.ItemGroup && list[i].listChildItems != null)
                    ResetNPCKnowledgeInTopicListRecursively(list[i].listChildItems);
            }
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
            itemAnyNews.caption = (TextManager.Instance.GetText(textDatabase, "AnyNews"));
            listTopicTellMeAbout.Add(itemAnyNews);

            ListItem itemWhereAmI = new ListItem();
            itemWhereAmI.type = ListItemType.Item;
            itemWhereAmI.questionType = QuestionType.WhereAmI;
            itemWhereAmI.caption = (TextManager.Instance.GetText(textDatabase, "WhereAmI"));
            listTopicTellMeAbout.Add(itemWhereAmI);

            foreach (KeyValuePair<ulong, QuestResources> questInfo in dictQuestInfo)
            {
                foreach (KeyValuePair<string, QuestResourceInfo> questResourceInfo in questInfo.Value.resourceInfo)
                {
                    ListItem itemQuestTopic = new ListItem();
                    itemQuestTopic.type = ListItemType.Item;
                    string captionString = String.Empty;
                    switch (questResourceInfo.Value.resourceType)
                    {
                        case QuestInfoResourceType.NotSet:
                        default:
                            itemQuestTopic.questionType = QuestionType.NoQuestion;
                            
                            break;
                        case QuestInfoResourceType.Location:
                            itemQuestTopic.questionType = QuestionType.QuestLocation;
                            Questing.Place place = (Questing.Place)questResourceInfo.Value.questResource;
                            if (place.SiteDetails.buildingName != null)
                                captionString = place.SiteDetails.buildingName;
                            else
                                captionString = place.SiteDetails.locationName;
                            break;
                        case QuestInfoResourceType.Person:
                            itemQuestTopic.questionType = QuestionType.QuestPerson;
                            Questing.Person person = (Questing.Person)questResourceInfo.Value.questResource;
                            captionString = person.DisplayName;
                            break;
                        case QuestInfoResourceType.Thing:
                            itemQuestTopic.questionType = QuestionType.QuestItem;
                            Questing.Item item = (Questing.Item)questResourceInfo.Value.questResource;
                            if (item != null && item.DaggerfallUnityItem != null)
                                captionString = item.DaggerfallUnityItem.ItemName;
                            break;
                    }
                    ulong questID = questInfo.Key;
                    itemQuestTopic.questID = questID;
                    
                    itemQuestTopic.caption = captionString;

                    itemQuestTopic.key = questResourceInfo.Key;                    

                    if (questResourceInfo.Value.availableForDialog && questResourceInfo.Value.hasEntryInTellMeAbout) // only make it available for talk if it is not "hidden" by dialog link command
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
            List<BuildingInfo> matchingBuildings = new List<BuildingInfo>();

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
                    itemPreviousList.caption = (TextManager.Instance.GetText(textDatabase, "PreviousList"));
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
                        Questing.Place place = (Questing.Place)questResourceInfo.Value.questResource;
                        // (fix bug reports http://forums.dfworkshop.net/viewtopic.php?f=24&t=996, http://forums.dfworkshop.net/viewtopic.php?f=24&t=997)
                        // only build entries for place quest resources that are in same location as pc
                        if (GameManager.Instance.PlayerGPS.CurrentLocation.MapTableData.MapId != place.SiteDetails.mapId)
                            continue;

                        if (place.SiteDetails.buildingKey == 0)
                            continue;

                        DFLocation.BuildingTypes buildingType;
                        try
                        {
                            buildingType = GameManager.Instance.TalkManager.GetBuildingTypeForBuildingKey(place.SiteDetails.buildingKey);
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

                            ulong questID = questInfo.Key;
                            item.questID = questID;

                            string captionString = place.SiteDetails.buildingName;
                            item.caption = captionString;
                            item.buildingKey = place.SiteDetails.buildingKey;

                            item.key = questResourceInfo.Key;

                            if (questResourceInfo.Value.availableForDialog && questResourceInfo.Value.hasEntryInWhereIs)
                            {
                                if (!alreadyCreatedGeneralSubSection)
                                {
                                    itemBuildingTypeGroup = new ListItem();
                                    itemBuildingTypeGroup.type = ListItemType.ItemGroup;
                                    itemBuildingTypeGroup.caption = (TextManager.Instance.GetText(textDatabase, "General"));
                                    listTopicLocation.Add(itemBuildingTypeGroup);
                                    alreadyCreatedGeneralSubSection = true;
                                }

                                if (itemBuildingTypeGroup.listChildItems == null)
                                {
                                    ListItem itemPreviousList;
                                    itemPreviousList = new ListItem();
                                    itemPreviousList.type = ListItemType.NavigationBack;
                                    itemPreviousList.caption = (TextManager.Instance.GetText(textDatabase, "PreviousList"));
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
                    itemBuildingTypeGroup.caption = (TextManager.Instance.GetText(textDatabase, "General"));
                    listTopicLocation.Add(itemBuildingTypeGroup);
                    alreadyCreatedGeneralSubSection = true;
                }

                if (itemBuildingTypeGroup.listChildItems == null)
                {
                    ListItem itemPreviousList;
                    itemPreviousList = new ListItem();
                    itemPreviousList.type = ListItemType.NavigationBack;
                    itemPreviousList.caption = (TextManager.Instance.GetText(textDatabase, "PreviousList"));
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
            itemBuildingTypeGroup.caption = (TextManager.Instance.GetText(textDatabase, "Regional"));
            itemBuildingTypeGroup.listChildItems = new List<ListItem>();

            ListItem prevListItem;
            prevListItem = new ListItem();
            prevListItem.type = ListItemType.NavigationBack;
            prevListItem.caption = (TextManager.Instance.GetText(textDatabase, "PreviousList"));
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
                if (i >= 8 && i <= 17) // is a knightly order
                {
                    if (playerRegion == KnightlyOrderRegions[i - 8] && !DoesBuildingExistLocally(FactionsAndBuildings[i], false))
                        AddRegionalBuildingTalkItem(i, ref itemBuildingTypeGroup);
                }
                else if (i >= 20) // is a store
                {
                    if (!DoesBuildingExistLocally(FactionsAndBuildings[i], true))
                        AddRegionalBuildingTalkItem(i, ref itemBuildingTypeGroup);
                }
                else if (!DoesBuildingExistLocally(FactionsAndBuildings[i], false)) // is a temple
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
                    else
                    {
                        if (building.FactionId == SearchedFor)
                            return true;
                    }
                }
            }

            return false;
        }

        private void AddRegionalBuildingTalkItem(int index, ref ListItem itemBuildingTypeGroup)
        {
            ListItem item;

            item = new ListItem();
            item.type = ListItemType.Item;
            item.questionType = QuestionType.Regional;
            item.caption = UserInterfaceWindows.HardStrings.any.Replace("%s", UserInterfaceWindows.HardStrings.buildingNames[index]);
            item.index = index;
            itemBuildingTypeGroup.listChildItems.Add(item);
        }

        private void AssembleTopicListPerson()
        {
            List<ListItem> oldTopicList = null;
            if (listTopicLocation != null)
                oldTopicList = listTopicLocation; // store old topic list to inject some of the info into new list

            listTopicPerson = new List<ListItem>();

            foreach (KeyValuePair<ulong, QuestResources> questInfo in dictQuestInfo)
            {
                foreach (KeyValuePair<string, QuestResourceInfo> questResourceInfo in questInfo.Value.resourceInfo)
                {
                    if (questResourceInfo.Value.resourceType == QuestInfoResourceType.Person)
                    {
                        Questing.Person person = (Questing.Person)questResourceInfo.Value.questResource;

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

                        // in case person is questor check if questor is in same mapID                        
                        if (person.IsQuestor == true)
                        {
                            if (person.QuestorData.mapID == GameManager.Instance.PlayerGPS.CurrentMapID)
                                IsPlayerInSameLocationWorldCell = true;

                            // test if dialog partner is same person as person resource
                            if (GameManager.Instance.IsPlayerInside && !GameManager.Instance.IsPlayerInsideCastle)
                            {                              
                                if (this.targetStaticNPC != null && this.currentNPCType == NPCType.Static && this.targetStaticNPC.Data.buildingKey == GameManager.Instance.PlayerEnterExit.Interior.EntryDoor.buildingKey && this.nameNPC == captionString)
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
                        if (!dialogPartnerIsSamePersonAsPersonResource &&   // dialog partner is not the same person as the person resource talked about
                            questResourceInfo.Value.availableForDialog &&   // it is not "hidden" by dialog link command
                            questResourceInfo.Value.hasEntryInWhereIs &&    // it is meant to have an entry in the where is section (currently this is always true, TODO: check if we can get rid of this)
                            IsPlayerInSameLocationWorldCell)                // if person resource is in same world map location as player
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
                oldTopicList = listTopicLocation; // store old topic list to inject some of the info into new list

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
        /// get portrait archive and texture record index for current set target static npc
        /// </summary>
        /// <returns></returns>
        private void GetPortraitIndexFromStaticNPCBillboard(out DaggerfallTalkWindow.FacePortraitArchive facePortraitArchive, out int recordIndex)
        {
            FactionFile.FactionData factionData;
            GameManager.Instance.PlayerEntity.FactionData.GetFactionData(targetStaticNPC.Data.factionID, out factionData);

            FactionFile.FlatData factionFlatData = FactionFile.GetFlatData(factionData.flat1);
            FactionFile.FlatData factionFlatData2 = FactionFile.GetFlatData(factionData.flat2);

            // get face for special npcs here and return in this case
            if (factionData.type == 4)
            {
                facePortraitArchive = DaggerfallTalkWindow.FacePortraitArchive.SpecialFaces;
                recordIndex = factionData.face;
                return;
            }

            // if no special npc, resolving process for common faces starts here
            facePortraitArchive = DaggerfallTalkWindow.FacePortraitArchive.CommonFaces;

            // use oops as default - so use it if we fail to resolve face later on in this resolving process
            recordIndex = 410;

            FlatsFile.FlatData flatData;
            
            // resolve face from npc's faction data as default            
            int archive = factionFlatData.archive;
            int record = factionFlatData.record;
            if (targetStaticNPC.Data.gender == Genders.Female)
            {
                archive = factionFlatData2.archive;
                record = factionFlatData2.record;
            }
            if (DaggerfallUnity.Instance.ContentReader.FlatsFileReader.GetFlatData(FlatsFile.GetFlatID(archive, record), out flatData)) // (if flat data exists in FlatsFile, overwrite index)
                recordIndex = flatData.faceIndex;

            // overwrite if target npc's billboard archive and record index can be resolved (more specific than just the factiondata - which will always resolve to same portrait for a specific faction)
            if (DaggerfallUnity.Instance.ContentReader.FlatsFileReader.GetFlatData(FlatsFile.GetFlatID(targetStaticNPC.Data.billboardArchiveIndex, targetStaticNPC.Data.billboardRecordIndex), out flatData))
                recordIndex = flatData.faceIndex;
        }

        private string GetAnswerFromTokensArray(ulong questID, List<TextFile.Token[]> answers)
        {
            int randomNumAnswer = UnityEngine.Random.Range(0, answers.Count);

            // cloning important here: we want to evaluate every time the answer is created so altering macros are re-expanded correctly
            // e.g. Missing Prince quest allows player to ask for dungeon and there is a "%di" macro that needs to be re-evaluated
            // to correctly show current direction to dungeon (when in different towns it is likely to be different)
            TextFile.Token[] tokens = (TextFile.Token[])answers[randomNumAnswer].Clone();

            // expand tokens and reveal dialog-linked resources
            QuestMacroHelper macroHelper = new QuestMacroHelper();
            macroHelper.ExpandQuestMessage(GameManager.Instance.QuestMachine.GetQuest(questID), ref tokens, true);

            return TokensToString(tokens);

        }

        private string TokensToString(TextFile.Token[] tokens, bool addSpaceAtTokenEnd = true)
        {
            // create return string from expanded tokens
            string returnString = "";
            string seperatorString = " ";
            if (!addSpaceAtTokenEnd)
                seperatorString = "";
            for (int i = 0; i < tokens.Length; i++)
            {
                string textFragment = tokens[i].text;
                if (textFragment != null && textFragment != string.Empty)
                    returnString += textFragment;
                else
                    returnString += seperatorString;
            }
            return returnString;
        }

        private string ExpandRandomTextRecord(int recordIndex)
        {
            TextFile.Token[] tokens = DaggerfallUnity.Instance.TextProvider.GetRandomTokens(recordIndex);

            MacroHelper.ExpandMacros(ref tokens, this); // this... TalkManager (TalkManagerMCP)

            return (tokens[0].text);
        }

        private string getRecordIdByNpcsSocialGroup(int textRecordIdDefault, int textRecordIdGuildMembers, int textRecordIdMerchants, int textRecordIdScholars, int textRecordIdNobility, int textRecordIdUnderworld)
        {
            switch (npcData.socialGroup)
            {
                case FactionFile.SocialGroups.None:
                case FactionFile.SocialGroups.Commoners:
                case FactionFile.SocialGroups.SGroup10:
                case FactionFile.SocialGroups.SGroup5:
                case FactionFile.SocialGroups.SGroup8:
                case FactionFile.SocialGroups.SGroup9:
                case FactionFile.SocialGroups.SupernaturalBeings:
                default:
                    return ExpandRandomTextRecord(textRecordIdDefault);
                case FactionFile.SocialGroups.GuildMembers:
                    return ExpandRandomTextRecord(textRecordIdGuildMembers);
                case FactionFile.SocialGroups.Merchants:
                    return ExpandRandomTextRecord(textRecordIdMerchants);
                case FactionFile.SocialGroups.Nobility:
                    return ExpandRandomTextRecord(textRecordIdNobility);
                case FactionFile.SocialGroups.Scholars:
                    return ExpandRandomTextRecord(textRecordIdScholars);
                case FactionFile.SocialGroups.Underworld:
                    return ExpandRandomTextRecord(textRecordIdUnderworld); // todo: this needs to be tested with a npc of social group underworld in vanilla df
            }
        }


        #endregion

        #region event handlers

        private void OnMapPixelChanged(DFPosition mapPixel)
        {
            rebuildTopicLists = true;
            GetBuildingList(); // create building list and especially "any work" questors
        }

        private void OnTransitionToExterior(PlayerEnterExit.TransitionEventArgs args)
        {
            rebuildTopicLists = true;
            GetBuildingList(); // create building list and especially "any work" questors
        }

        private void OnTransitionToDungeonExterior(PlayerEnterExit.TransitionEventArgs args)
        {
            rebuildTopicLists = true;
            GetBuildingList(); // create building list and especially "any work" questors
        }

        private void OnTransitionToDungeonInterior(PlayerEnterExit.TransitionEventArgs args)
        {
            castleNPCsSpokenTo.Clear();
        }

        void OnLoadEvent(SaveData_v1 saveData)
        {
            rebuildTopicLists = true;
            GetBuildingList(); // create building list and especially "any work" questors            
        }

        #endregion

        #region console_commands

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