// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2017 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Michael Rauter (Nystul)
// Contributors:    
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

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// this class manages talk topics and resulting actions/answers
    /// </summary>
    public class TalkManager : MonoBehaviour
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
            OrganisationInfo,
            Work,
            LocalBuilding,
            Regional,
            Person,
            Thing
        }

        public class ListItem
        {
            public ListItemType type = ListItemType.Item; // list item can be either a normal item, a navigation item (to get to parent list) or an item group (contains list of child items)
            public string caption = "undefined";
            public QuestionType questionType = QuestionType.NoQuestion;
            public List<ListItem> listChildItems = null; // null if type == ListItemType.Navigation or ListItemType.Item, only contains a list if type == ListItemType.ItemGroup
            public List<ListItem> listParentItems = null; // null if type == ListItemType.ItemGroup or ListItemType.Item, only contains a list if type == ListItemType.Navigation
        }

        List<ListItem> listTellMeAbout;
        List<ListItem> listTopicLocation;
        List<ListItem> listTopicPerson;
        List<ListItem> listTopicThing;

        int numQuestionsAsked = 0;
        string questionOpeningText = ""; // randomize PC opening text only once for every new question so save it in this string after creating it


        struct BuildingInfo
        {
            public string name;
            public DFLocation.BuildingTypes buildingType;
        }       
        List<BuildingInfo> listBuildings = null;

        #endregion

        #region Properties

        public List<ListItem> ListTellMeAbout
        {
            get { return listTellMeAbout; }
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

        public void StartNewConversation()
        {
            numQuestionsAsked = 0;
        }

        public string GetNPCGreetingText()
        {
            //string greetingString = DaggerfallUnity.Instance.TextProvider.GetRandomText(7206);
            //string greetingString = DaggerfallUnity.Instance.TextProvider.GetRandomText(7207);
            //string greetingString = DaggerfallUnity.Instance.TextProvider.GetRandomText(7208);
            string greetingString = DaggerfallUnity.Instance.TextProvider.GetRandomText(7209);
            return (greetingString);
        }

        public string GetPCGreetingText(DaggerfallTalkWindow.TalkTone talkTone)
        {
            int toneIndex = DaggerfallTalkWindow.TalkToneToIndex(talkTone);
            string greetingString = DaggerfallUnity.Instance.TextProvider.GetRandomText(7215 + toneIndex) + " ";
            return (greetingString);
        }

        public string GetPCFollowUpText(DaggerfallTalkWindow.TalkTone talkTone)
        {
            int toneIndex = DaggerfallTalkWindow.TalkToneToIndex(talkTone);
            string followUpString = DaggerfallUnity.Instance.TextProvider.GetRandomText(7218 + toneIndex) + " ";
            return (followUpString);
        }

        public string GetQuestionText(TalkManager.ListItem listItem, DaggerfallTalkWindow.TalkTone talkTone)
        {
            int toneIndex = DaggerfallTalkWindow.TalkToneToIndex(talkTone);
            string question = "";
            if (questionOpeningText == "")
            {
                if (numQuestionsAsked == 0)
                    questionOpeningText = GetPCGreetingText(talkTone);
                else
                    questionOpeningText = GetPCFollowUpText(talkTone);
            }
            question += questionOpeningText;

            switch (listItem.questionType)
            {
                case QuestionType.NoQuestion:
                default:
                    break;
                case QuestionType.News:
                    question += DaggerfallUnity.Instance.TextProvider.GetRandomText(7231 + toneIndex);
                    break;
                case QuestionType.OrganisationInfo:
                    question += "not implemented";
                    break;
                case QuestionType.LocalBuilding:
                    question += DaggerfallUnity.Instance.TextProvider.GetRandomText(7225) + toneIndex;
                    break;
                case QuestionType.Person:
                    question += DaggerfallUnity.Instance.TextProvider.GetRandomText(7225) + toneIndex;
                    break;
                case QuestionType.Thing:
                    question += "not implemented";
                    break;
                case QuestionType.Regional:
                    question += "not implemented";
                    break;
                case QuestionType.Work:
                    question += DaggerfallUnity.Instance.TextProvider.GetRandomText(7211) + toneIndex;
                    break;
            }            
            return question;
        }

        public string GetNewsOrRumors()
        {
            string news = DaggerfallUnity.Instance.TextProvider.GetRandomText(1400);
            return (news);
        }

        public string GetAnswerText(TalkManager.ListItem listItem)
        {
            string answer = "";
            switch (listItem.questionType)
            {
                case QuestionType.NoQuestion:
                default:                    
                    answer = DaggerfallUnity.Instance.TextProvider.GetRandomText(7280);
                    break;
                case QuestionType.News:
                    answer = GetNewsOrRumors();
                    break;
                case QuestionType.OrganisationInfo:
                    answer = "not implemented";
                    break;
                case QuestionType.LocalBuilding:
                    answer = DaggerfallUnity.Instance.TextProvider.GetRandomText(7285) + DaggerfallUnity.Instance.TextProvider.GetRandomText(7332);
                    break;
                case QuestionType.Person:
                    answer = DaggerfallUnity.Instance.TextProvider.GetRandomText(7280);
                    break;
                case QuestionType.Thing:
                    answer = "not implemented";
                    break;
                case QuestionType.Regional:
                    answer = "not implemented";
                    break;
                case QuestionType.Work:
                    answer = DaggerfallUnity.Instance.TextProvider.GetRandomText(8076);
                    break;

            }

            numQuestionsAsked++;
            questionOpeningText = ""; // reset questionOpeningText so that it is newly created for next question
            return answer;
        }

        #endregion

        #region Private Methods

        void GetBuildingList()
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

            DFBlock[] blocks;
            RMBLayout.GetLocationBuildingData(location, out blocks);
            int width = location.Exterior.ExteriorData.Width;
            int height = location.Exterior.ExteriorData.Height;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = y * width + x;
                    BuildingSummary[] buildingsInBlock = RMBLayout.GetBuildingData(blocks[index], x, y);

                    foreach (BuildingSummary buildingSummary in buildingsInBlock)
                    {
                        try
                        {
                            string locationName = BuildingNames.GetName(buildingSummary.NameSeed, buildingSummary.BuildingType, buildingSummary.FactionId, location.Name, location.RegionName);
                            BuildingInfo item;
                            item.buildingType = buildingSummary.BuildingType;
                            item.name = locationName;
                            listBuildings.Add(item);
                        }
                        catch (Exception e)
                        {
                            string exceptionMessage = String.Format("exception occured in function BuildingNames.GetName (exception message: " + e.Message + @") with params: 
                                                                        seed: {0}, type: {1}, factionID: {2}, locationName: {3}, regionName: {4}",
                                                                        buildingSummary.NameSeed, buildingSummary.BuildingType, buildingSummary.FactionId, location.Name, location.RegionName);
                            DaggerfallUnity.LogMessage(exceptionMessage, true);

                        }
                    }
                }
            }
        }

        string BuildingTypeToGroupString(DFLocation.BuildingTypes buildingType)
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

        bool checkBuildingTypeInSkipList(DFLocation.BuildingTypes buildingType)
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

        void AssembleTopicLists()
        {
            AssembleTopicListTellMeAbout();
            AssembleTopicListLocation();
            AssembleTopicListPerson();
            AssembleTopicListThing();
        }

        void AssembleTopicListTellMeAbout()
        {
            listTellMeAbout = new List<ListItem>();
            ListItem itemAnyNews = new ListItem();
            itemAnyNews.type = ListItemType.Item;
            itemAnyNews.questionType = QuestionType.News;
            itemAnyNews.caption = "Any News?";
            listTellMeAbout.Add(itemAnyNews);

            for (int i = 0; i < 10; i++)
            {
                ListItem itemOrganisationInfo = new ListItem();
                itemOrganisationInfo.type = ListItemType.Item;
                itemOrganisationInfo.questionType = QuestionType.OrganisationInfo;
                itemOrganisationInfo.caption = "Placeholder for Organisation";
                listTellMeAbout.Add(itemOrganisationInfo);
            }
        }

        void AssembleTopicListLocation()
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
            for (int i = 0; i < 7; i++)
            {
                ListItem item;
                if (i == 0)
                {
                    item = new ListItem();
                    item.type = ListItemType.NavigationBack;
                    item.caption = "Previous List";
                    item.listParentItems = listTopicLocation;
                    itemBuildingTypeGroup.listChildItems.Add(item);
                }
                item = new ListItem();
                item.type = ListItemType.Item;
                item.questionType = QuestionType.Regional;
                item.caption = "regional temple (placeholder) " + i;
                itemBuildingTypeGroup.listChildItems.Add(item);
            }
            listTopicLocation.Add(itemBuildingTypeGroup);            
        }

        void AssembleTopicListPerson()
        {
            listTopicPerson = new List<ListItem>();
            for (int i = 0; i < 12; i++)
            {
                ListItem item = new ListItem();
                item.type = ListItemType.Item;
                item.questionType = QuestionType.Person;
                item.caption = "dummy person " + i + " (here will be the name of the person later on)";
                listTopicPerson.Add(item);
            }
        }

        void AssembleTopicListThing()
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

        #endregion
    }
}