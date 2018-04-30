// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Michael Rauter (Nystul)
// Contributors: Numidium   
// 
// Notes:
//

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Questing;
using System.Linq;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Game.Player;

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
     
        public enum ListItemType
        {
            Item,
            ItemGroup,
            NavigationBack
        }

        public enum QuestionType
        {
            NoQuestion, // used for list entries that are not of ListItemType item
            News,
            OrganizationInfo,
            Work,
            LocalBuilding,
            Regional,
            Person, // not sure if we will ever have a person entry which is no quest person...
            Thing, // not sure if we will ever have a thing entry which is no quest thing...
            QuestLocation,
            QuestPerson,
            QuestItem
        }

        public enum NPCKnowledgeAboutItem
        {
            NotSet,
            DoesNotKnowAboutItem,
            KnowsAboutItem
        }

        public class ListItem
        {
            public ListItemType type = ListItemType.Item; // list item can be either a normal item, a navigation item (to get to parent list) or an item group (contains list of child items)
            public string caption = "undefined";
            public QuestionType questionType = QuestionType.NoQuestion;
            public NPCKnowledgeAboutItem npcKnowledgeAboutItem = NPCKnowledgeAboutItem.NotSet;
            public int buildingKey = -1;
            public ulong questID = 0; // used for listitems that are question about quest resources
            public int index = -1;
            public List<ListItem> listChildItems = null; // null if type == ListItemType.Navigation or ListItemType.Item, only contains a list if type == ListItemType.ItemGroup
            public List<ListItem> listParentItems = null; // null if type == ListItemType.ItemGroup or ListItemType.Item, only contains a list if type == ListItemType.Navigation
        }

        // current target npc for conversion
        //MobilePersonNPC targetMobileNPC = null;
        StaticNPC targetStaticNPC = null;
        public class NPCData
        {
            public Races race;
            public FactionFile.SocialGroups socialGroup;
        }
        NPCData npcData;

        // last target npc for a conversion
        enum NPCType
        {
            Static,
            Mobile,
            Unset
        }
        MobilePersonNPC lastTargetMobileNPC = null;
        StaticNPC lastTargetStaticNPC = null;
        NPCType currentNPCType = NPCType.Unset;

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


        ListItem currentQuestionListItem = null; // current question list item        
        string currentKeySubject = "";
        KeySubjectType currentKeySubjectType = KeySubjectType.Unset;
        int currentKeySubjectBuildingKey = -1;
        int reactionToPlayer = 0;


        //The lists that contain the topics to select
        List<ListItem> listTopicTellMeAbout;
        List<ListItem> listTopicLocation;
        List<ListItem> listTopicPerson;
        List<ListItem> listTopicThing;

        int numQuestionsAsked = 0;
        string questionOpeningText = ""; // randomize PC opening text only once for every new question so save it in this string after creating it

        bool markLocationOnMap = false;

        DaggerfallTalkWindow.TalkTone currentTalkTone = DaggerfallTalkWindow.TalkTone.Normal;

        struct BuildingInfo
        {
            public string name;
            public DFLocation.BuildingTypes buildingType;
            public int buildingKey;
            public Vector2 position;
        }       
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
            public QuestInfoResourceType resourceType;
            public List<TextFile.Token[]> anyInfoAnswers;
            public List<TextFile.Token[]> rumorsAnswers;
            public bool availableForDialog; // if it will show up in talk window (any dialog link for this resource will set this false, if no dialog link is present it will be set to true)
            public bool hasEntryInTellMeAbout; // if resource will get entry in section "Tell Me About" (anyInfo or rumors available)
            public bool hasEntryInWhereIs; // if resource will get entry in section "Where Is" (e.g. person resources)
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

        // dictionary of quests (key is questID, value is QuestInfo)
        Dictionary<ulong, QuestResources> dictQuestInfo = new Dictionary<ulong, QuestResources>();


        public enum RumorType
        {
            CommonRumor,
            QuestProgressRumor,
            QuestRumorMill
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


        // questor post quest message stuff
        Dictionary<ulong, TextFile.Token[]> dictQuestorPostQuestMessage = new Dictionary<ulong,TextFile.Token[]>();

        public class SaveDataConversation
        {
            public Dictionary<ulong, QuestResources> dictQuestInfo;
            public List<RumorMillEntry> listRumorMill;
            public Dictionary<ulong, TextFile.Token[]> dictQuestorPostQuestMessage;
        }

        // faction IDs for factions listed in "tell me about"
        int[] infoFactionIDs = { 42, 40, 108, 129, 306, 353, 41, 67, 82, 84, 88, 92, 94, 106, 36, 83, 85, 89, 93, 95, 99, 107, 37, 368, 408, 409, 410, 411, 413, 414, 415, 416, 417, 98 };

        // Data for NPC "work" quests in the current town
        struct NpcWorkEntry
        {
            public StaticNPC.NPCData npc;
            public FactionFile.SocialGroups socialGroup;
            public string buildingName;
        }

        Dictionary<int, NpcWorkEntry> npcsWithWork = new Dictionary<int, NpcWorkEntry>();
        int lastExteriorEntered;
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

        public int LastExteriorEntered
        {
            get { return lastExteriorEntered; }
            set { lastExteriorEntered = value; }
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
            SaveLoadManager.OnLoad += OnLoadEvent;

            // initialize work variables
            lastExteriorEntered = 0;
            selectedNpcWorkKey = -1;
        }

        void OnDestroy()
        {
            // important that transition events/delegates are destroyed in OnDestroy() instead of OnDisable (since exteriorAutomap gameobject is disabled when going indoors and enabled when going outdoors)
            PlayerGPS.OnMapPixelChanged -= OnMapPixelChanged;
            PlayerEnterExit.OnTransitionExterior -= OnTransitionToExterior;
            PlayerEnterExit.OnTransitionDungeonExterior -= OnTransitionToDungeonExterior;
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

        }

        void Update()
        {

        }

        #endregion

        #region Public Methods

        // Player has clicked on a mobile talk target
        public void TalkToMobileNPC(MobilePersonNPC targetNPC)
        {
            currentNPCType = NPCType.Mobile;

            const int youGetNoResponseTextId = 7205;

            // Get NPC faction
            FactionFile.FactionData NPCfaction;
            int oneBasedPlayerRegion = GameManager.Instance.PlayerGPS.CurrentOneBasedRegionIndex;
            FactionFile.FactionData[] factions = GameManager.Instance.PlayerEntity.FactionData.FindFactions(
                (int)FactionFile.FactionTypes.Province, -1, -1, oneBasedPlayerRegion);

            // Should always find a region
            if (factions == null || factions.Length == 0)
                throw new Exception("TalkToMobileNPC() did not find a match for NPC faction.");

            // Warn if more than 1 region is found
            if (factions.Length > 1)
                Debug.LogWarningFormat("TalkToMobileNPC() found more than 1 matching NPC faction for region {0}.", oneBasedPlayerRegion);

            NPCfaction = factions[0];

            // Get reaction to player
            int reactionToPlayer = 0;
            PlayerEntity player = GameManager.Instance.PlayerEntity;
            reactionToPlayer = NPCfaction.rep;
            reactionToPlayer += player.BiographyReactionMod;

            if (NPCfaction.sgroup < player.SGroupReputations.Length) // one of the five general social groups
                reactionToPlayer += player.SGroupReputations[NPCfaction.sgroup];

            if (reactionToPlayer >= -20)
            {
                DaggerfallUI.UIManager.PushWindow(DaggerfallUI.Instance.TalkWindow);
                GameManager.Instance.TalkManager.SetTargetNPC(targetNPC, reactionToPlayer);
            }
            else
            {
                DaggerfallUI.MessageBox(youGetNoResponseTextId);
            }            
        }

        // Player has clicked or static talk target or clicked the talk button inside a popup-window
        public void TalkToStaticNPC(StaticNPC targetNPC)
        {
            if (IsNpcOfferingQuest(targetNPC.Data.nameSeed)) {
                DaggerfallUI.UIManager.PushWindow(new DaggerfallQuestOfferWindow(DaggerfallUI.UIManager, npcsWithWork[targetNPC.Data.nameSeed].npc, npcsWithWork[targetNPC.Data.nameSeed].socialGroup));
                return;
            }
            currentNPCType = NPCType.Static;

            const int youGetNoResponseTextId = 7205;

            // Get NPC faction
            FactionFile.FactionData NPCfaction;
            int oneBasedPlayerRegion = GameManager.Instance.PlayerGPS.CurrentOneBasedRegionIndex;
            FactionFile.FactionData[] factions = GameManager.Instance.PlayerEntity.FactionData.FindFactions(
                (int)FactionFile.FactionTypes.Province, -1, -1, oneBasedPlayerRegion);

            // Should always find a region
            if (factions == null || factions.Length == 0)
                throw new Exception("TalkToStaticNPC() did not find a match for NPC faction.");

            // Warn if more than 1 region is found (NOTE: this happens for Glenmoril Coven which clashes with Septim Empire - probably should remove this warning)
            if (factions.Length > 1)
                Debug.LogWarningFormat("TalkToStaticNPC() found more than 1 matching NPC faction for region {0}.", oneBasedPlayerRegion);

            NPCfaction = factions[0];

            // Get reaction to player
            int reactionToPlayer = 0;
            PlayerEntity player = GameManager.Instance.PlayerEntity;
            reactionToPlayer = NPCfaction.rep;
            reactionToPlayer += player.BiographyReactionMod;

            if (NPCfaction.sgroup < player.SGroupReputations.Length) // one of the five general social groups
                reactionToPlayer += player.SGroupReputations[NPCfaction.sgroup];

            if (reactionToPlayer >= -20)
            {
                DaggerfallUI.UIManager.PushWindow(DaggerfallUI.Instance.TalkWindow);
                GameManager.Instance.TalkManager.SetTargetNPC(targetNPC, reactionToPlayer);
            }
            else
            {
                DaggerfallUI.MessageBox(youGetNoResponseTextId);
            }
        }


        public void SetTargetNPC(MobilePersonNPC targetMobileNPC, int reactionToPlayer)
        {
            if (targetMobileNPC == lastTargetMobileNPC)
                return;

            //this.targetMobileNPC = targetMobileNPC;

            DaggerfallUI.Instance.TalkWindow.SetNPCPortrait(DaggerfallTalkWindow.FacePortraitArchive.CommonFaces, targetMobileNPC.PersonFaceRecordId);

            lastTargetMobileNPC = targetMobileNPC;

            nameNPC = targetMobileNPC.NameNPC;
            DaggerfallUI.Instance.TalkWindow.UpdateNameNPC();

            npcData = new NPCData();
            npcData.socialGroup = FactionFile.SocialGroups.Commoners;
            npcData.race = targetMobileNPC.Race;

            this.reactionToPlayer = reactionToPlayer;

            // reset npc knowledge, for now it resets every time the npc has changed (player talked to new npc)
            // TODO: match classic daggerfall - in classic npc remember their knowledge about topics for their time of existence
            resetNPCKnowledgeInTopicListRecursively(listTopicLocation);
            resetNPCKnowledgeInTopicListRecursively(listTopicPerson);
            resetNPCKnowledgeInTopicListRecursively(listTopicThing);
            resetNPCKnowledgeInTopicListRecursively(listTopicTellMeAbout);
        }

        public void SetTargetNPC(StaticNPC targetNPC, int reactionToPlayer)
        {
            if (targetNPC == lastTargetStaticNPC)
                return;

            this.targetStaticNPC = targetNPC;

            DaggerfallTalkWindow.FacePortraitArchive facePortraitArchive;
            int recordIndex;
            getPortraitIndexFromStaticNPCBillboard(out facePortraitArchive, out recordIndex);
            DaggerfallUI.Instance.TalkWindow.SetNPCPortrait(facePortraitArchive, recordIndex);

            lastTargetStaticNPC = targetNPC;

            nameNPC = targetNPC.DisplayName;
            DaggerfallUI.Instance.TalkWindow.UpdateNameNPC();

            FactionFile.FactionData factionData;
            GameManager.Instance.PlayerEntity.FactionData.GetFactionData(targetStaticNPC.Data.factionID, out factionData);

            npcData = new NPCData();
            npcData.socialGroup = (FactionFile.SocialGroups)factionData.sgroup;
            npcData.race = Races.Breton; // TODO: find a way to get race for static npc

            this.reactionToPlayer = reactionToPlayer;

            // reset npc knowledge, for now it resets every time the npc has changed (player talked to new npc)
            // TODO: match classic daggerfall - in classic npc remember their knowledge about topics for their time of existence
            resetNPCKnowledgeInTopicListRecursively(listTopicLocation);
            resetNPCKnowledgeInTopicListRecursively(listTopicPerson);
            resetNPCKnowledgeInTopicListRecursively(listTopicThing);
            resetNPCKnowledgeInTopicListRecursively(listTopicTellMeAbout);
        }

        public void StartNewConversation()
        {
            numQuestionsAsked = 0;
            questionOpeningText = "";
            currentQuestionListItem = null;
            SetupRumorMill();
        }

        public string GetNPCGreetingText()
        {
            const int dislikePlayerGreetingTextId = 7206;
            const int neutralToPlayerGreetingTextId = 7207;
            const int likePlayerGreetingTextId = 7208;
            const int veryLikePlayerGreetingTextId = 7209;

            string greetingString = "";

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

                                // expand tokens and reveal dialog-linked resources
                                QuestMacroHelper macroHelper = new QuestMacroHelper();
                                macroHelper.ExpandQuestMessage(GameManager.Instance.QuestMachine.GetQuest(questID), ref tokens, true);
                                greetingString = TokensToString(tokens);

                                return (greetingString);                                
                            }
                        }
                    }
                }
            }

            if (reactionToPlayer >= 0)
            {
                if (reactionToPlayer >= 10)
                {
                    if (reactionToPlayer >= 30)
                        greetingString = expandRandomTextRecord(veryLikePlayerGreetingTextId);
                    else
                        greetingString = expandRandomTextRecord(likePlayerGreetingTextId);
                }
                else
                {
                    greetingString = expandRandomTextRecord(neutralToPlayerGreetingTextId);
                }
            }
            else
            {
                greetingString = expandRandomTextRecord(dislikePlayerGreetingTextId);
            }
            
            return (greetingString);
        }

        public string GetPCGreetingText(DaggerfallTalkWindow.TalkTone talkTone)
        {
            int toneIndex = DaggerfallTalkWindow.TalkToneToIndex(talkTone);
            string greetingString = expandRandomTextRecord(7215 + toneIndex);

            return (greetingString);
        }

        public string GetPCFollowUpText(DaggerfallTalkWindow.TalkTone talkTone)
        {
            int toneIndex = DaggerfallTalkWindow.TalkToneToIndex(talkTone);
            string followUpString = expandRandomTextRecord(7218 + toneIndex);
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
            return expandRandomTextRecord(7211);
        }

        public string DirectionVector2DirectionHintString(Vector2 vecDirectionToTarget)
        {
            float angle = Mathf.Acos(Vector2.Dot(vecDirectionToTarget, Vector2.right) / vecDirectionToTarget.magnitude) / Mathf.PI * 180.0f;
            if (vecDirectionToTarget.y < 0)
                angle = 180.0f + (180.0f - angle);

            if ((angle >= 0.0f && angle < 22.5f) || (angle >= 337.5f && angle <= 360.0f))
                return "east";
            else if (angle >= 22.5f && angle < 67.5f)
                return "northeast";
            else if (angle >= 67.5f && angle < 112.5f)
                return "north";
            else if (angle >= 112.5f && angle < 157.5f)
                return "northwest";
            else if (angle >= 157.5f && angle < 202.5f)
                return "west";
            else if (angle >= 202.5f && angle < 247.5f)
                return "southwest";
            else if (angle >= 247.5f && angle < 292.5f)
                return "south";
            else if (angle >= 292.5f && angle < 337.5f)
                return "southeast";
            else
                return "nevermind...";
        }

        public string GetKeySubjectLocationDirection()
        {
            // note Nystul:
            // I reused coordinate mapping from buildings from exterior automap layout implementation here
            // So both building position as well as player position are calculated in map coordinates and compared
            Vector2 playerPos;
            float scale = MapsFile.WorldMapTerrainDim * MeshReader.GlobalScale;
            playerPos.x = ((GameManager.Instance.PlayerGPS.transform.position.x) % scale) / scale;
            playerPos.y = ((GameManager.Instance.PlayerGPS.transform.position.z) % scale) / scale;
            int refWidth = (int)(DaggerfallExteriorAutomap.blockSizeWidth * DaggerfallExteriorAutomap.numMaxBlocksX * GameManager.Instance.ExteriorAutomap.LayoutMultiplier);
            int refHeight = (int)(DaggerfallExteriorAutomap.blockSizeHeight * DaggerfallExteriorAutomap.numMaxBlocksY * GameManager.Instance.ExteriorAutomap.LayoutMultiplier);
            playerPos.x *= refWidth;
            playerPos.y *= refHeight;
            playerPos.x -= refWidth * 0.5f;
            playerPos.y -= refHeight * 0.5f;

            BuildingInfo buildingInfo = listBuildings.Find(x => x.buildingKey == currentKeySubjectBuildingKey);

            Vector2 vecDirectionToTarget = buildingInfo.position - playerPos;
            return DirectionVector2DirectionHintString(vecDirectionToTarget);
        }

        public void MarkKeySubjectLocationOnMap()
        {
            BuildingInfo buildingInfo = listBuildings.Find(x => x.buildingKey == currentKeySubjectBuildingKey);
            GameManager.Instance.PlayerGPS.DiscoverBuilding(buildingInfo.buildingKey);
        }

        public string GetQuestionText(TalkManager.ListItem listItem, DaggerfallTalkWindow.TalkTone talkTone)
        {
            int toneIndex = DaggerfallTalkWindow.TalkToneToIndex(talkTone);
            string question = "";

            currentTalkTone = talkTone;

            currentKeySubject = listItem.caption; // set key to current caption for now (which is in case of buildings the building name)

            switch (listItem.questionType)
            {
                case QuestionType.NoQuestion:
                default:
                    break;
                case QuestionType.News:
                    question = expandRandomTextRecord(7231 + toneIndex);
                    break;
                case QuestionType.OrganizationInfo:
                    currentKeySubjectType = KeySubjectType.Organization;
                    question = expandRandomTextRecord(7212 + toneIndex);
                    break;
                case QuestionType.LocalBuilding:
                    currentKeySubjectType = KeySubjectType.Building;
                    currentKeySubjectBuildingKey = listItem.buildingKey;
                    question = expandRandomTextRecord(7225 + toneIndex);
                    break;
                case QuestionType.Person:
                    question = expandRandomTextRecord(7225 + toneIndex);
                    break;
                case QuestionType.Thing:
                    question = "not implemented";
                    break;
                case QuestionType.Regional:
                    currentKeySubjectType = KeySubjectType.Building;

                    // Improvement over classic. Make "Any" lower-case since it will be in the middle of a sentence.
                    currentKeySubject = currentKeySubject.Replace("Any", "any");

                    question = expandRandomTextRecord(7225 + toneIndex);
                    break;
                case QuestionType.QuestLocation:
                case QuestionType.QuestPerson:
                case QuestionType.QuestItem:
                    currentKeySubjectType = KeySubjectType.QuestTopic;
                    question = expandRandomTextRecord(7212 + toneIndex);
                    break;
                case QuestionType.Work:
                    currentKeySubjectType = KeySubjectType.Work;
                    question = expandRandomTextRecord(7212 + toneIndex);
                    break;
            }            
            return question;
        }

        public string GetNewsOrRumors()
        {
            string news = "nevermind ...";
            int randomIndex = UnityEngine.Random.Range(0, listRumorMill.Count);
            RumorMillEntry entry = listRumorMill[randomIndex];
            if (entry.rumorType == RumorType.CommonRumor)
            {
                if (entry.listRumorVariants != null)
                {
                    TextFile.Token[] tokens = entry.listRumorVariants[0];
                    MacroHelper.ExpandMacros(ref tokens, this);
                    news = tokens[0].text;
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
            
            return (news);
        }

        public string GetOrganizationInfo(TalkManager.ListItem listItem)
        {
            int index = (listItem.index > 7 ? listItem.index + 1 : listItem.index);
            return expandRandomTextRecord(860 + index);
        }

        public void AddQuestRumorToRumorMill(ulong questID, Message message)
        {
            if (listRumorMill == null)
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
            if (listRumorMill == null)
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
            if (listRumorMill == null)
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

        public string GetKeySubjectLocationHint()
        {
            string answer;

            // chances unknown - so there is a 75% chance for now that npc gives location direction hints and a 25% chance that npc will reveal location on map
            // always only give directional hints if player is inside
            int randomNum = UnityEngine.Random.Range(0, 4);
            if (randomNum > 0 || GameManager.Instance.IsPlayerInside)
            {
                markLocationOnMap = false;
                answer = expandRandomTextRecord(7333);
            }
            else
            {
                markLocationOnMap = true;
                answer = expandRandomTextRecord(7332);
                markLocationOnMap = false;
            }

            return answer;
        }

        public string GetDialogHint(ListItem listItem)
        {
            if (dictQuestInfo.ContainsKey(listItem.questID))
            {
                if (dictQuestInfo[listItem.questID].resourceInfo.ContainsKey(listItem.caption))
                {
                    List<TextFile.Token[]> answers = dictQuestInfo[listItem.questID].resourceInfo[listItem.caption].anyInfoAnswers;
                    return getAnswerFromTokensArray(listItem.questID, answers);
                }
            }
            return "Never mind..."; // error case - should never ever occur
        }

        public string GetDialogHint2(ListItem listItem)
        {
            if (dictQuestInfo.ContainsKey(listItem.questID))
            {
                if (dictQuestInfo[listItem.questID].resourceInfo.ContainsKey(listItem.caption))
                {
                    List<TextFile.Token[]> answers = dictQuestInfo[listItem.questID].resourceInfo[listItem.caption].rumorsAnswers;
                    return getAnswerFromTokensArray(listItem.questID, answers);
                }
            }
            return "Never mind..."; // error case - should never ever occur
        }

        public string GetAnswerAboutLocation(TalkManager.ListItem listItem)
        {
            string answer;

            if (listItem.npcKnowledgeAboutItem == NPCKnowledgeAboutItem.NotSet)
            {
                // chances unknown - so there is a 50% chance for now that npc knows
                int randomNum = UnityEngine.Random.Range(0, 2);
                if (randomNum == 0)
                    listItem.npcKnowledgeAboutItem = NPCKnowledgeAboutItem.DoesNotKnowAboutItem;
                else
                    listItem.npcKnowledgeAboutItem = NPCKnowledgeAboutItem.KnowsAboutItem;
            }

            if (listItem.npcKnowledgeAboutItem == NPCKnowledgeAboutItem.DoesNotKnowAboutItem)
                answer = getRecordIdByNpcsSocialGroup(7281, 7280, 7280, 7283, 7282, 7284); // messages if npc does not know
            else
            {
                answer = getRecordIdByNpcsSocialGroup(7271, 7270, 7270, 7273, 7272, 7274); // location related messages if npc knows
            }
            return answer;
        }

        public string GetAnswerAboutRegionalBuilding(TalkManager.ListItem listItem)
        {
            if (GetRegionalLocationCityName(listItem))
                return expandRandomTextRecord(10);
            else
                return expandRandomTextRecord(11);
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
                    answer = getRecordIdByNpcsSocialGroup(7281, 7280, 7280, 7283, 7282, 7284); // messages if npc does not know
                    break;
                case QuestionType.News:
                    answer = GetNewsOrRumors();
                    break;
                case QuestionType.OrganizationInfo:
                    answer = GetOrganizationInfo(listItem);
                    break;
                case QuestionType.LocalBuilding:
                    answer = GetAnswerAboutLocation(listItem);
                    break;
                case QuestionType.Person:
                    answer = getRecordIdByNpcsSocialGroup(7281, 7280, 7280, 7283, 7282, 7284); // messages if npc does not know
                    break;
                case QuestionType.Thing:
                    answer = "not implemented";
                    break;
                case QuestionType.Regional:
                    answer = GetAnswerAboutRegionalBuilding(listItem);
                    break;
                case QuestionType.QuestLocation:
                case QuestionType.QuestPerson:
                case QuestionType.QuestItem:
                    answer = GetAnswerAboutQuestTopic(listItem);
                    break;
                case QuestionType.Work:
                    if (!WorkAvailable)
                    {
                        answer = expandRandomTextRecord(8078);
                        break;
                    }
                    else
                    {
                        SetRandomQuestor(); // Pick a random Work questor from the pool
                        answer = expandRandomTextRecord(8076);
                        break;
                    }
            }

            numQuestionsAsked++;
            questionOpeningText = ""; // reset questionOpeningText so that it is newly created for next question
            return answer;
        }

        public string GetAnswerAboutQuestTopic(TalkManager.ListItem listItem)
        {
            if (listItem.npcKnowledgeAboutItem == NPCKnowledgeAboutItem.NotSet)
            {
                // chances unknown - so there is a 50% chance for now that npc knows
                int randomNum = UnityEngine.Random.Range(0, 2);
                if (randomNum == 0)
                    listItem.npcKnowledgeAboutItem = NPCKnowledgeAboutItem.DoesNotKnowAboutItem;
                else
                    listItem.npcKnowledgeAboutItem = NPCKnowledgeAboutItem.KnowsAboutItem;
            }

            if (listItem.npcKnowledgeAboutItem == NPCKnowledgeAboutItem.DoesNotKnowAboutItem)
                return getRecordIdByNpcsSocialGroup(7281, 7280, 7280, 7283, 7282, 7284); // messages if npc does not know
            else
                return getRecordIdByNpcsSocialGroup(7276, 7275, 7275, 7278, 7277, 7279); // quest topic related messages if npc knows
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

            QuestResourceInfo questResourceInfo = new QuestResourceInfo();
            questResourceInfo.anyInfoAnswers = anyInfoAnswers;
            questResourceInfo.rumorsAnswers = rumorsAnswers;
            questResourceInfo.resourceType = resourceType;
            questResourceInfo.availableForDialog = true;
            questResourceInfo.hasEntryInTellMeAbout = true;
            if (resourceType == QuestInfoResourceType.Person || resourceType == QuestInfoResourceType.Location)
                questResourceInfo.hasEntryInWhereIs = true;
            else
                questResourceInfo.hasEntryInWhereIs = false;
            questResourceInfo.dialogLinkedLocations = new List<string>();
            questResourceInfo.dialogLinkedPersons = new List<string>();
            questResourceInfo.dialogLinkedThings = new List<string>();
            questResourceInfo.questResource = questResource;
         
            questResources.resourceInfo[resourceName] = questResourceInfo;

            dictQuestInfo[questID] = questResources;

            // update topic list
            AssembleTopiclistTellMeAbout();

            // update rumor mill
            GameManager.Instance.TalkManager.AddQuestRumorToRumorMill(questID, questResourceInfo.rumorsAnswers);
        }

        public void AddPersonTopic(ulong questID, DaggerfallWorkshop.Game.Questing.Person person)
        {
            string resourceName = person.DisplayName;

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

            QuestResourceInfo questResourceInfo;
            if (questResources.resourceInfo.ContainsKey(resourceName))
            {
                questResourceInfo = questResources.resourceInfo[resourceName];
            }
            else
            {
                questResourceInfo = new QuestResourceInfo();
                questResourceInfo.anyInfoAnswers = null;
                questResourceInfo.rumorsAnswers = null;
                questResourceInfo.resourceType = QuestInfoResourceType.Person;
                questResourceInfo.availableForDialog = false;
                questResourceInfo.hasEntryInTellMeAbout = false;
                questResourceInfo.hasEntryInWhereIs = true;
                questResourceInfo.dialogLinkedLocations = new List<string>();
                questResourceInfo.dialogLinkedPersons = new List<string>();
                questResourceInfo.dialogLinkedThings = new List<string>();
                questResourceInfo.questResource = person;
            }

            //QuestMacroHelper macroHelper = new QuestMacroHelper();
            
            questResources.resourceInfo[resourceName] = questResourceInfo;
            dictQuestInfo[questID] = questResources;
            
            // update topic list
            AssembleTopicListPerson();
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

        public void AddDialogForQuestInfoResource(ulong questID, string resourceName, QuestInfoResourceType resourceType)
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

                questResource.availableForDialog = true;
            }

            // update topic list
            AssembleTopiclistTellMeAbout();
        }

        public void RemoveQuestInfoTopicsForSpecificQuest(ulong questID)
        {
            if (dictQuestInfo.ContainsKey(questID))
            {
                dictQuestInfo.Remove(questID);
            }

            // update topic lists
            AssembleTopiclistTellMeAbout();
            AssembleTopicListLocation();
            AssembleTopicListPerson();
            AssembleTopicListThing();
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
            
            listRumorMill = data.listRumorMill;
            if (listRumorMill == null)
            {
                SetupRumorMill();
            }

            dictQuestorPostQuestMessage = data.dictQuestorPostQuestMessage;
            if (dictQuestorPostQuestMessage == null)
            {
                dictQuestorPostQuestMessage = new Dictionary<ulong, TextFile.Token[]>();
            }

            // update topic list
            AssembleTopiclistTellMeAbout();
        }

        public bool IsNpcOfferingQuest(int nameSeed)
        {
            return npcsWithWork.ContainsKey(nameSeed) && !QuestMachine.Instance.IsLastNPCClickedAnActiveQuestor();
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

        private void SetupRumorMill()
        {
            if (listRumorMill == null)
                listRumorMill = new List<RumorMillEntry>();
            if (listRumorMill.Count == 0)
            {
                for (int i = 0; i < 10; i++) // setup 10 random comman rumors (this is very early work in progress)
                {
                    RumorMillEntry entry = new RumorMillEntry();

                    TextFile.Token[] tokens;
                    int randomNum = UnityEngine.Random.Range(0, 12);
                    if (randomNum >= 0 && randomNum <= 9)
                        tokens = DaggerfallUnity.Instance.TextProvider.GetRandomTokens(1400 + randomNum);
                    else if (randomNum == 10)
                        tokens = DaggerfallUnity.Instance.TextProvider.GetRandomTokens(1456);
                    else if (randomNum == 11)
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

            DaggerfallExteriorAutomap.BlockLayout[] blockLayout = GameManager.Instance.ExteriorAutomap.ExteriorLayout;

            DFBlock[] blocks;
            RMBLayout.GetLocationBuildingData(location, out blocks);
            int width = location.Exterior.ExteriorData.Width;
            int height = location.Exterior.ExteriorData.Height;
            bool populateQuestors = false;
            if (lastExteriorEntered != GameManager.Instance.PlayerGPS.CurrentLocationIndex)
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
                            float xPosBuilding = blockLayout[index].rect.xpos + (int)(buildingSummary.Position.x / (BlocksFile.RMBDimension * MeshReader.GlobalScale) * DaggerfallExteriorAutomap.blockSizeWidth) - GameManager.Instance.ExteriorAutomap.LocationWidth * DaggerfallExteriorAutomap.blockSizeWidth * 0.5f;
                            float yPosBuilding = blockLayout[index].rect.ypos + (int)(buildingSummary.Position.z / (BlocksFile.RMBDimension * MeshReader.GlobalScale) * DaggerfallExteriorAutomap.blockSizeHeight) - GameManager.Instance.ExteriorAutomap.LocationHeight * DaggerfallExteriorAutomap.blockSizeHeight * 0.5f;
                            item.position = new Vector2(xPosBuilding, yPosBuilding);
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
                                        -1, GameManager.Instance.PlayerGPS.CurrentOneBasedRegionIndex);
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
                    return ("Alchemists");
                case DFLocation.BuildingTypes.Armorer:
                    return ("Armorers");
                case DFLocation.BuildingTypes.Bank:
                    return ("Banks");
                case DFLocation.BuildingTypes.Bookseller:
                    return ("Bookstores");
                case DFLocation.BuildingTypes.ClothingStore:
                    return ("Clothing stores");
                case DFLocation.BuildingTypes.GemStore:
                    return ("Gem stores");
                case DFLocation.BuildingTypes.GeneralStore:
                    return ("General stores");
                case DFLocation.BuildingTypes.GuildHall:
                    return ("Guilds");
                case DFLocation.BuildingTypes.Library:
                    return ("Libraries");
                case DFLocation.BuildingTypes.PawnShop:
                    return ("Pawn shops");
                case DFLocation.BuildingTypes.Tavern:
                    return ("Taverns");
                case DFLocation.BuildingTypes.WeaponSmith:
                    return ("Weapon smiths");
                case DFLocation.BuildingTypes.Temple:
                    return ("Local temples");
                default:
                    return ("");
            }
        }

        private bool checkBuildingTypeInSkipList(DFLocation.BuildingTypes buildingType)
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

        private void resetNPCKnowledgeInTopicListRecursively(List<ListItem> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                list[i].npcKnowledgeAboutItem = NPCKnowledgeAboutItem.NotSet;
                if (list[i].type == ListItemType.ItemGroup && list[i].listChildItems != null)
                    resetNPCKnowledgeInTopicListRecursively(list[i].listChildItems);
            }
        }

        private void AssembleTopicLists()
        {
            AssembleTopiclistTellMeAbout();
            AssembleTopicListLocation();
            AssembleTopicListPerson();
            AssembleTopicListThing();
        }

        private void AssembleTopiclistTellMeAbout()
        {
            listTopicTellMeAbout = new List<ListItem>();
            ListItem itemAnyNews = new ListItem();
            itemAnyNews.type = ListItemType.Item;
            itemAnyNews.questionType = QuestionType.News;
            itemAnyNews.caption = "Any news?";
            listTopicTellMeAbout.Add(itemAnyNews);

            foreach (KeyValuePair<ulong, QuestResources> questInfo in dictQuestInfo)
            {
                foreach (KeyValuePair<string, QuestResourceInfo> questResourceInfo in questInfo.Value.resourceInfo)
                {
                    ListItem itemQuestTopic = new ListItem();
                    itemQuestTopic.type = ListItemType.Item;
                    switch (questResourceInfo.Value.resourceType)
                    {
                        case QuestInfoResourceType.NotSet:
                        default:
                            itemQuestTopic.questionType = QuestionType.NoQuestion;
                            break;
                        case QuestInfoResourceType.Location:
                            itemQuestTopic.questionType = QuestionType.QuestLocation;
                            break;
                        case QuestInfoResourceType.Person:
                            itemQuestTopic.questionType = QuestionType.QuestPerson;
                            break;
                        case QuestInfoResourceType.Thing:
                            itemQuestTopic.questionType = QuestionType.QuestItem;
                            break;
                    }
                    ulong questID = questInfo.Key;
                    itemQuestTopic.questID = questID;
                    string captionString = questResourceInfo.Key;
                    //QuestMacroHelper macroHelper = new QuestMacroHelper();
                    //macroHelper.ExpandQuestString(GameManager.Instance.QuestMachine.GetQuest(questID), ref captionString);
                    itemQuestTopic.caption = captionString;

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
        }

        private void AssembleTopicListLocation()
        {
            listTopicLocation = new List<ListItem>();

            GetBuildingList();

            ListItem itemBuildingTypeGroup;
            List<BuildingInfo> matchingBuildings = new List<BuildingInfo>();

            foreach (DFLocation.BuildingTypes buildingType in Enum.GetValues(typeof(DFLocation.BuildingTypes)))
            {                
                matchingBuildings = listBuildings.FindAll(x => x.buildingType == buildingType);
                if (checkBuildingTypeInSkipList(buildingType))
                    continue;

                if (matchingBuildings.Count > 0)
                {
                    itemBuildingTypeGroup = new ListItem();
                    itemBuildingTypeGroup.type = ListItemType.ItemGroup;
                    itemBuildingTypeGroup.caption = BuildingTypeToGroupString(buildingType);

                    itemBuildingTypeGroup.listChildItems = new List<ListItem>();

                    ListItem itemPreviousList = new ListItem();
                    itemPreviousList.type = ListItemType.NavigationBack;
                    itemPreviousList.caption = "Previous List";
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
            
            matchingBuildings = listBuildings.FindAll(x => x.buildingType == DFLocation.BuildingTypes.Palace);
            if (matchingBuildings.Count > 0)
            {
                itemBuildingTypeGroup = new ListItem();
                itemBuildingTypeGroup.type = ListItemType.ItemGroup;
                itemBuildingTypeGroup.caption = "General";
                listTopicLocation.Add(itemBuildingTypeGroup);

                ListItem itemPreviousList;
                itemPreviousList = new ListItem();
                itemPreviousList.type = ListItemType.NavigationBack;
                itemPreviousList.caption = "Previous List";
                itemPreviousList.listParentItems = listTopicLocation;
                itemBuildingTypeGroup.listChildItems = new List<ListItem>();
                itemBuildingTypeGroup.listChildItems.Add(itemPreviousList);

                foreach (BuildingInfo buildingInfo in matchingBuildings)
                {
                    ListItem item = new ListItem();
                    item.type = ListItemType.Item;
                    item.questionType = QuestionType.LocalBuilding;
                    item.caption = buildingInfo.name;
                    itemBuildingTypeGroup.listChildItems.Add(item);
                }
            }

            itemBuildingTypeGroup = new ListItem();
            itemBuildingTypeGroup.type = ListItemType.ItemGroup;
            itemBuildingTypeGroup.caption = "Regional";
            itemBuildingTypeGroup.listChildItems = new List<ListItem>();

            ListItem prevListItem;
            prevListItem = new ListItem();
            prevListItem.type = ListItemType.NavigationBack;
            prevListItem.caption = "Previous List";
            prevListItem.listParentItems = listTopicLocation;
            itemBuildingTypeGroup.listChildItems.Add(prevListItem);

            AddRegionalItems(ref itemBuildingTypeGroup);
            listTopicLocation.Add(itemBuildingTypeGroup);
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
            listTopicPerson = new List<ListItem>();

            foreach (KeyValuePair<ulong, QuestResources> questInfo in dictQuestInfo)
            {
                foreach (KeyValuePair<string, QuestResourceInfo> questResourceInfo in questInfo.Value.resourceInfo)
                {
                    if (questResourceInfo.Value.resourceType == QuestInfoResourceType.Person)
                    {
                        ListItem item = new ListItem();
                        item.type = ListItemType.Item;
                        item.questionType = QuestionType.Person;

                        ulong questID = questInfo.Key;
                        item.questID = questID;

                        string captionString = questResourceInfo.Key;
                        item.caption = captionString;

                        bool IsPlayerInSameLocationWorldCell = false;  //((Questing.Person)questResourceInfo.Value.questResource).IsPlayerInSameLocationWorldCell();
                        if (questResourceInfo.Value.availableForDialog && // only make it available for talk if it is not "hidden" by dialog link command
                            questResourceInfo.Value.hasEntryInWhereIs &&
                            IsPlayerInSameLocationWorldCell)
                            listTopicPerson.Add(item);
                    }

                }
            }
        }

        private void AssembleTopicListThing()
        {
            listTopicThing = new List<ListItem>();
            for (int i = 0; i < 30; i++)
            {
                ListItem item = new ListItem();
                item.type = ListItemType.Item;
                item.questionType = QuestionType.Thing;
                item.caption = "thing " + i;
                listTopicThing.Add(item);
            }
        }

        /// <summary>
        /// get portrait archive and texture record index for current set target static npc
        /// </summary>
        /// <returns></returns>
        private void getPortraitIndexFromStaticNPCBillboard(out DaggerfallTalkWindow.FacePortraitArchive facePortraitArchive, out int recordIndex)
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

        private string getAnswerFromTokensArray(ulong questID, List<TextFile.Token[]> answers)
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

        private string TokensToString(TextFile.Token[] tokens)
        {
            // create return string from expanded tokens
            string returnString = "";
            for (int i = 0; i < tokens.Length; i++)
            {
                string textFragment = tokens[i].text;
                if (textFragment.Length > 0 && i < textFragment.Length)
                    returnString += textFragment;
                else
                    returnString += " ";
            }
            return returnString;
        }


        private string expandRandomTextRecord(int recordIndex)
        {
            TextFile.Token[] tokens = DaggerfallUnity.Instance.TextProvider.GetRandomTokens(recordIndex);

            MacroHelper.ExpandMacros(ref tokens, this); // this... TalkManager (TalkManagerMCP)

            return (tokens[0].text);
        }

        #endregion

        #region event handlers

        private void OnMapPixelChanged(DFPosition mapPixel)
        {
            AssembleTopicLists();
        }

        private void OnTransitionToExterior(PlayerEnterExit.TransitionEventArgs args)
        {
            AssembleTopicLists();
        }

        private void OnTransitionToDungeonExterior(PlayerEnterExit.TransitionEventArgs args)
        {
            AssembleTopicLists();
        }

        void OnLoadEvent(SaveData_v1 saveData)
        {
            AssembleTopicLists();
        }

        private string getRecordIdByNpcsSocialGroup(int textRecordIdDefault, int textRecordIdGuildMembers, int textRecordIdMerchants, int textRecordIdNobility, int textRecordIdScholars, int textRecordIdUnderworld)
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
                    return expandRandomTextRecord(textRecordIdDefault);
                case FactionFile.SocialGroups.GuildMembers:
                    return expandRandomTextRecord(textRecordIdGuildMembers);
                case FactionFile.SocialGroups.Merchants:
                    return expandRandomTextRecord(textRecordIdMerchants);
                case FactionFile.SocialGroups.Nobility:
                    return expandRandomTextRecord(textRecordIdNobility);
                case FactionFile.SocialGroups.Scholars:
                    return expandRandomTextRecord(textRecordIdScholars);
                case FactionFile.SocialGroups.Underworld:
                    return expandRandomTextRecord(textRecordIdUnderworld); // todo: this needs to be tested with a npc of social group underworld in vanilla df
            }
        }

        #endregion         
    }
}