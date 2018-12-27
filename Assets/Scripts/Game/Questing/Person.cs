// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DaggerfallConnect;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallConnect.Arena2;
using FullSerializer;

namespace DaggerfallWorkshop.Game.Questing
{
    /// <summary>
    /// Defines an NPC involved in a quest.
    /// </summary>
    public class Person : QuestResource
    {
        #region Fields

        const int faceCount = 10;

        Races race = Races.Breton;
        NameHelper.BankTypes nameBank = NameHelper.BankTypes.Breton;
        Genders npcGender = Genders.Male;
        int faceIndex = 0;
        int nameSeed = -1;
        bool isQuestor = false;
        bool isIndividualNPC = false;
        bool isIndividualAtHome = false;
        string displayName = string.Empty;
        string godName = string.Empty;
        Symbol homePlaceSymbol = null;
        Symbol lastAssignedPlaceSymbol = null;
        bool assignedToHome = false;
        string factionTableKey = string.Empty;
        FactionFile.FactionData factionData;
        StaticNPC.NPCData questorData;
        bool discoveredThroughTalkManager = false;

        #endregion

        #region Properties

        public int FactionIndex
        {
            get { return factionData.id; }
        }

        public int FaceIndex
        {
            get { return faceIndex; }
        }

        public int NameSeed
        {
            get { return nameSeed; }
            set { nameSeed = value; }
        }

        public Races Race
        {
            get { return race; }
        }

        public override Genders Gender
        {
            get { return npcGender; }
        }

        public bool IsQuestor
        {
            get { return isQuestor; }
        }

        public bool IsIndividualNPC
        {
            get { return isIndividualNPC; }
        }

        public bool IsIndividualAtHome
        {
            get { return isIndividualAtHome; }
        }

        public string DisplayName
        {
            get { return displayName; }
        }

        public string GodName
        {
            get { return godName; }
        }

        public string HomeTownName
        {
            get { return GetHomePlaceLocationName(); }
        }

        public string HomeRegionName
        {
            get { return GetHomePlaceRegionName(); }
        }

        public string HomeBuildingName
        {
            get { return GetHomeBuildingName(); }
        }

        public FactionFile.FactionData FactionData
        {
            get { return factionData; }
        }

        public StaticNPC.NPCData QuestorData
        {
            get { return questorData; }
        }

        public bool DiscoveredThroughTalkManager
        {
            get { return discoveredThroughTalkManager; }
            set { discoveredThroughTalkManager = value; }
        }

        #endregion

        #region Constructors

        public Person(Quest parentQuest)
            : base (parentQuest)
        {
        }

        public Person(Quest parentQuest, string line)
            : base (parentQuest)
        {
            SetResource(line);
        }

        #endregion

        #region Overrides

        public override void SetResource(string line)
        {
            string individualNPCName = string.Empty;
            string factionTypeName = string.Empty;
            string factionAllianceName = string.Empty;
            string careerAllianceName = string.Empty;
            string genderName = string.Empty;
            int faceIndex = -1;
            bool atHome = false;

            base.SetResource(line);

            // Match strings
            string declMatchStr = @"(Person|person) (?<symbol>[a-zA-Z0-9'_.-]+)";
            string optionsMatchStr = @"named (?<individualNPCName>[a-zA-Z0-9'_.-]+)|" +
                                     @"face (?<faceIndex>\d+)|" +
                                     @"(factionType|factiontype) (?<factionType>[a-zA-Z0-9'_.-]+)|" +
                                     @"faction (?<factionAlliance>[a-zA-Z0-9'_.-]+)|" +
                                     @"group (?<careerAlliance>[a-zA-Z0-9'_.-]+)|" +
                                     @"(?<gender>female|male)|" +
                                     @"(?<atHome>(atHome|athome))";

            // Try to match source line with pattern
            Match match = Regex.Match(line, declMatchStr);
            if (match.Success)
            {
                // Store symbol for quest system
                Symbol = new Symbol(match.Groups["symbol"].Value);

                // Match all options
                MatchCollection options = Regex.Matches(line, optionsMatchStr);
                foreach (Match option in options)
                {
                    // Individual NPC
                    Group individualNPCNameGroup = option.Groups["individualNPCName"];
                    if (individualNPCNameGroup.Success)
                        individualNPCName = individualNPCNameGroup.Value;

                    // Face
                    Group faceGroup = option.Groups["faceIndex"];
                    if (faceGroup.Success)
                        faceIndex = Parser.ParseInt(faceGroup.Value);

                    // Faction type
                    Group factionTypeGroup = option.Groups["factionType"];
                    if (factionTypeGroup.Success)
                        factionTypeName = factionTypeGroup.Value;

                    // Faction alliance
                    Group factionAllianceGroup = option.Groups["factionAlliance"];
                    if (factionAllianceGroup.Success)
                        factionAllianceName = factionAllianceGroup.Value;

                    // Group
                    Group careerAllianceGroup = option.Groups["careerAlliance"];
                    if (careerAllianceGroup.Success)
                        careerAllianceName = careerAllianceGroup.Value;

                    // Gender
                    Group genderGroup = option.Groups["gender"];
                    if (genderGroup.Success)
                        genderName = genderGroup.Value;

                    // At home
                    Group atHomeGroup = option.Groups["atHome"];
                    atHome = atHomeGroup.Success;
                }

                // Setup NPC based on input parameters
                if (!string.IsNullOrEmpty(individualNPCName))
                {
                    SetupIndividualNPC(individualNPCName);
                }
                else if (!string.IsNullOrEmpty(careerAllianceName))
                {
                    SetupCareerAllianceNPC(careerAllianceName);
                }
                else if (!string.IsNullOrEmpty(factionTypeName))
                {
                    SetupFactionTypeNPC(factionTypeName);
                }
                else if (!string.IsNullOrEmpty(factionAllianceName))
                {
                    SetupFactionAllianceNPC(factionAllianceName);
                }
                else
                {
                    throw new Exception(string.Format("Person resource could not identify NPC from line {0}", line));
                }

                // Assign NPC details
                AssignRace();
                AssignGender(genderName);
                AssignHUDFace(faceIndex);
                AssignDisplayName();
                AssignHomeTown();
                AssignGod();

                // Is NPC at home?
                isIndividualAtHome = atHome;

                // add conversation topics from anyInfo command tag
                AddConversationTopics();

                // Done
                Debug.LogFormat("Created NPC {0} with FactionID #{1}.", displayName, factionData.id);
            }
        }

        public override bool ExpandMacro(MacroTypes macro, out string textOut)
        {
            // TODO:
            //  * Support for home town/building (believe this is just random unless NPC moved from a Place)
            //  * Support for %god (TEXT.RSC 4077-4084)
            //  * Support for pronoun (%g1, %g2, %g2, %g2self, %g3)
            //  * Support for class (not sure what NPCs have a class, need to see this used in a quest)
            //  * Support for faction (believe this is just the name of faction they belong to, e.g. The Merchants)

            // Store this person in quest as last Person encountered
            // This will be used for subsequent pronoun macros, etc.
            ParentQuest.LastResourceReferenced = this;

            textOut = string.Empty;
            bool result = true;
            switch (macro)
            {
                case MacroTypes.NameMacro1:             // Display name
                    textOut = displayName;
                    break;

                case MacroTypes.NameMacro2:             // Home building name
                    textOut = GetHomeBuildingName();
                    break;

                case MacroTypes.NameMacro3:             // Home town name
                    textOut = GetHomePlaceLocationName();
                    break;

                case MacroTypes.NameMacro4:             // Home region name
                    textOut = GetHomePlaceRegionName();
                    break;

                case MacroTypes.DetailsMacro:           // Race
                    textOut = RaceTemplate.GetRaceDictionary()[(int)race].Name;
                    break;

                case MacroTypes.FactionMacro:           // Faction macro
                    // Want name of guild, not the person
                    FactionFile.FactionData guildData;
                    if (GameManager.Instance.PlayerEntity.FactionData.GetFactionData(ParentQuest.FactionId, out guildData))
                        textOut = guildData.name;
                    else
                        result = false;
                    break;

                default:                                // Macro not supported
                    result = false;
                    break;
            }

            return result;
        }

        public override void Tick(Quest caller)
        {
            base.Tick(caller);

            // Auto-assign NPC to home Place if available and player enters
            // This only happens for very specific NPC types
            // Equivalent to calling "place anNPC at aPlace" from script
            // Will not be called again as assignment is permanent for duration of quest
            if (homePlaceSymbol != null && !assignedToHome)
            {
                Place home = ParentQuest.GetPlace(homePlaceSymbol);
                if (home == null)
                    return;

                // Hot-place NPC at this location when player enters
                if (home.IsPlayerHere())
                    PlaceAtHome();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Places NPC to home. This can happen automatically when player enters building or when
        /// quest script uses "create npc" action to automatically assign Person to home.
        /// </summary>
        public bool PlaceAtHome()
        {
            // Does not attempt to place a questor as they should be statically placed or moved manually
            // Individual NPCs are also excluded as they are either automatically at home or moved elsewhere by quest
            Place homePlace = ParentQuest.GetPlace(homePlaceSymbol);
            if (homePlace == null || isQuestor || isIndividualNPC)
                return false;

            // Create SiteLink if not already present
            if (!QuestMachine.HasSiteLink(ParentQuest, homePlaceSymbol))
                QuestMachine.CreateSiteLink(ParentQuest, homePlaceSymbol);
            
            // Assign to home place
            homePlace.AssignQuestResource(Symbol);
            SetAssignedPlaceSymbol(homePlace.Symbol);
            assignedToHome = true;

            return true;
        }

        /// <summary>
        /// Gets home Place resource assigned to this NPC (if any).
        /// </summary>
        /// <returns>Home Place resource or null.</returns>
        public Place GetHomePlace()
        {
            if (homePlaceSymbol != null)
                return ParentQuest.GetPlace(homePlaceSymbol);

            return null;
        }

        /// <summary>
        /// Called by "place npc" to help track current Place assignment.
        /// </summary>
        /// <param name="symbol">Place symbol where Person was assigned.</param>
        public void SetAssignedPlaceSymbol(Symbol placeSymbol)
        {
            lastAssignedPlaceSymbol = placeSymbol;
        }

        /// <summary>
        /// Gets most recent Place this Person was assigned to.
        /// Notes:
        ///  - Can be null when NPC not yet assigned. They are technically nowhere.
        ///  - Will continue to return last assigned place even when "hide npc" is used.
        ///  - The action "destroy npc" will revoke assignment.
        /// </summary>
        /// <returns>Most recent Place symbol assigned, or null if not assigned.</returns>
        public Symbol GetAssignedPlaceSymbol()
        {
            return lastAssignedPlaceSymbol;
        }

        /// <summary>
        /// Checks if player in same world cell as Place this Person was assigned to.
        /// Does not care about specific building/dungeon or interior/exterior, just matching location mapID.
        /// Does not care if player actually inside bounds, just if inside same world cell.
        /// </summary>
        /// <returns>True if player in same world cell as location.</returns>
        public bool IsPlayerInSameLocationWorldCell()
        {
            // Get Place resource
            Place place = ParentQuest.GetPlace(lastAssignedPlaceSymbol);
            if (place == null)
                return false;

            // Compare mapID of player location and Place
            DFLocation location = GameManager.Instance.PlayerGPS.CurrentLocation;
            if (location.Loaded)
            {
                if (location.MapTableData.MapId == place.SiteDetails.mapId)
                    return true;
            }

            return false;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets region name of home Place (if any).
        /// </summary>
        /// <returns>Region name of home Place or BLANK if not set.</returns>
        string GetHomePlaceRegionName()
        {
            Place place = GetHomePlace();
            if (place == null)
                return BLANK;

            return place.SiteDetails.regionName;
        }

        /// <summary>
        /// Gets location name of home Place (if any).
        /// </summary>
        /// <returns>Location name of home Place or BLANK if not set.</returns>
        string GetHomePlaceLocationName()
        {
            Place place = GetHomePlace();
            if (place == null)
                return BLANK;

            return place.SiteDetails.locationName;
        }

        /// <summary>
        /// Gets building name of home Place (if any).
        /// </summary>
        /// <returns>Building name of home Place or BLANK if not set.</returns>
        string GetHomeBuildingName()
        {
            Place place = GetHomePlace();
            if (place == null)
                return BLANK;

            return place.SiteDetails.buildingName;
        }

        void AssignRace()
        {
            // Use faction race only for individuals
            if (isIndividualNPC)
            {
                FactionFile.FactionRaces factionRace = (FactionFile.FactionRaces)factionData.race;
                if (factionRace != FactionFile.FactionRaces.None)
                {
                    switch (factionRace)
                    {
                        case FactionFile.FactionRaces.Redguard:
                            race = Races.Redguard;
                            return;
                        case FactionFile.FactionRaces.Nord:
                            race = Races.Nord;
                            return;
                        case FactionFile.FactionRaces.DarkElf:
                            race = Races.DarkElf;
                            return;
                        case FactionFile.FactionRaces.WoodElf:
                            race = Races.WoodElf;
                            return;
                        case FactionFile.FactionRaces.Breton:
                        default:
                            race = Races.Breton;
                            return;
                    }
                }
            }

            // Otherwise use race of current region
            race = GameManager.Instance.PlayerGPS.GetRaceOfCurrentRegion();
            nameBank = GameManager.Instance.PlayerGPS.GetNameBankOfCurrentRegion();
        }

        void AssignGender(string genderName)
        {
            // Set gender if not already assigned by questor injection
            if (!isQuestor)
                npcGender = GetGender(genderName);
        }

        void AssignHUDFace(int faceIndex = -1)
        {
            this.faceIndex = UnityEngine.Random.Range(0, faceCount);

            //// Set face index
            //if (faceIndex != -1)
            //    this.faceIndex = faceIndex;
            //else
            //    this.faceIndex = UnityEngine.Random.Range(0, faceCount);
        }

        // Sets gender and display name
        // Has some logic to handle individual faction objects and certain flat limitations
        void AssignDisplayName()
        {
            // Witches and prostitutes only have female flats in Daggerfall
            if (factionData.type == (int)FactionFile.FactionTypes.WitchesCoven ||
                factionData.id == 512)
            {
                npcGender = Genders.Female;
            }

            // Assign name - some types have their own individual name to use
            if ((factionData.type == (int)FactionFile.FactionTypes.Individual ||
                factionData.type == (int)FactionFile.FactionTypes.Daedra) &&
                factionData.id != 0)
            {
                // Use individual name
                displayName = factionData.name;
            }
            else
            {
                // Set a name seed if not configured
                if (nameSeed == -1)
                    nameSeed = DateTime.Now.Millisecond;

                // Generate a random name based on gender and race name bank
                DFRandom.srand(nameSeed);
                displayName = DaggerfallUnity.Instance.NameHelper.FullName(nameBank, npcGender);
            }
        }

        void AssignHomeTown()
        {
            Place homePlace;
            string symbolName = string.Format("_{0}_home_", Symbol.Name);

            // If this is a Questor or individual NPC then use current location of player using a special helper
            if (isQuestor || (IsIndividualNPC && isIndividualAtHome))
            {
                homePlace = new Place(ParentQuest);
                if (GameManager.Instance.PlayerGPS.HasCurrentLocation)
                {
                    if (!homePlace.ConfigureFromPlayerLocation(symbolName))
                        throw new Exception("AssignHomeTown() could not configure questor/individual home from current player location.");

                    homePlaceSymbol = homePlace.Symbol;
                    ParentQuest.AddResource(homePlace);
                    LogHomePlace(homePlace);
                    return;
                }
            }

            // For other NPCs use default scope and building type
            Place.Scopes scope = Place.Scopes.Remote;
            string buildingTypeString = "house";

            // Adjust scope and building type based on faction hints
            int p1 = 0, p2 = 0, p3 = 0;
            if (!string.IsNullOrEmpty(factionTableKey))
            {
                // Get faction parameters
                p1 = Parser.ParseInt(QuestMachine.Instance.FactionsTable.GetValue("p1", factionTableKey));
                p2 = Parser.ParseInt(QuestMachine.Instance.FactionsTable.GetValue("p2", factionTableKey));
                p3 = Parser.ParseInt(QuestMachine.Instance.FactionsTable.GetValue("p3", factionTableKey));

                // Set based on parameters
                if (p1 == 0 && p2 == -3 || p1 == 0 && p2 == -4)
                {
                    // For local types set to local place
                    // This will support Local_3.0 - Local_4.10k
                    // Referencing quest Sx009 where player must locate and click an NPC with only a home location to go by
                    scope = Place.Scopes.Local;
                }
                else if (p1 == 0 && p2 >= 0 && p2 <= 20 && p3 == 0)
                {
                    // Set to a specific building type
                    buildingTypeString = QuestMachine.Instance.PlacesTable.GetKeyForValue("p2", p2.ToString());
                }
            }

            // Get scope string - must be "local" or "remote"
            string scopeString = string.Empty;
            if (scope == Place.Scopes.Local)
                scopeString = "local";
            else if (scope == Place.Scopes.Remote)
                scopeString = "remote";
            else
                throw new Exception("AssignHomeTown() scope must be either 'local' or 'remote'.");

            // Create the home location
            string source = string.Format("Place {0} {1} {2}", symbolName, scopeString, buildingTypeString);
            homePlace = new Place(ParentQuest, source);
            homePlaceSymbol = homePlace.Symbol.Clone();
            ParentQuest.AddResource(homePlace);
            LogHomePlace(homePlace);


            //
            // NOTE: Keeping the below for reference only at this time
            //

            //const string blank = "BLANK";

            //// If this is a Questor or individual NPC then use current location name
            //// Person is being instantiated where player currently is
            //if (isQuestor || (IsIndividualNPC && isIndividualAtHome))
            //{
            //    if (GameManager.Instance.PlayerGPS.HasCurrentLocation)
            //    {
            //        homeTownName = GameManager.Instance.PlayerGPS.CurrentLocation.Name;
            //        homeRegionName = GameManager.Instance.PlayerGPS.CurrentLocation.RegionName;
            //        homeBuildingName = blank;
            //        return;
            //    }
            //}

            //// Handle specific home Place assigned at create time
            //if (homePlaceSymbol != null)
            //{
            //    Place home = ParentQuest.GetPlace(homePlaceSymbol);
            //    if (home != null)
            //    {
            //        homeTownName = home.SiteDetails.locationName;
            //        homeRegionName = home.SiteDetails.regionName;
            //        homeBuildingName = home.SiteDetails.buildingName;
            //    }
            //}
            //else
            //{
            //    // Find a random location name from town types for flavour text
            //    // This might take a few attempts but will very quickly find a random town name
            //    int index;
            //    bool found = false;
            //    int regionIndex = GameManager.Instance.PlayerGPS.CurrentRegionIndex;
            //    DFRegion regionData = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetRegion(regionIndex);
            //    while (!found)
            //    {
            //        index = UnityEngine.Random.Range(0, regionData.MapTable.Length);
            //        DFRegion.LocationTypes locationType = regionData.MapTable[index].LocationType;
            //        if (locationType == DFRegion.LocationTypes.TownCity ||
            //            locationType == DFRegion.LocationTypes.TownHamlet ||
            //            locationType == DFRegion.LocationTypes.TownVillage)
            //        {
            //            homeTownName = regionData.MapNames[index];
            //            homeRegionName = regionData.Name;
            //            homeBuildingName = blank;
            //            found = true;
            //        }
            //    }
            //}

            //// Handle Local_3.x group NPCs (limited)
            //// These appear to be a special case of assigning a residential person who is automatically instantiated to home Place
            //// Creating a full target Place for this person automatically and storing in Quest
            //// NOTE: Understanding is still being developed here, likely will need to rework this later
            //if (QuestMachine.Instance.FactionsTable.HasValue(careerAllianceName))
            //{
            //    // Get params for this case
            //    int p1 = Parser.ParseInt(QuestMachine.Instance.FactionsTable.GetValue("p1", careerAllianceName));
            //    int p2 = Parser.ParseInt(QuestMachine.Instance.FactionsTable.GetValue("p2", careerAllianceName));
            //    //int p3 = Parser.ParseInt(QuestMachine.Instance.FactionsTable.GetValue("p3", careerAllianceName));

            //    // Only supporting specific cases for now - can expand later based on testing and iteration of support
            //    // This will support Local_3.0 - Local_3.3
            //    // Referencing quest Sx009 here where player must locate and click an NPC with only a home location to go by
            //    if (p1 == 0 && p2 == -3)
            //    {
            //        // Just using "house2" here as actual meaning of p3 unknown
            //        string homeSymbol = string.Format("_{0}_home_", Symbol.Name);
            //        string source = string.Format("Place {0} remote house2", homeSymbol);
            //        Place home = new Place(ParentQuest, source);
            //        homePlaceSymbol = home.Symbol.Clone();
            //        ParentQuest.AddResource(home);
            //    }
            //    else if (p1 == 0 && p2 >= 0)
            //    {
            //        // Handle standard building types
            //        string buildingSymbol = string.Format("_{0}_building_", Symbol.Name);
            //        string buildingType = QuestMachine.Instance.PlacesTable.GetKeyForValue("p2", p2.ToString());
            //        string source = string.Format("Place {0} remote {1}", buildingSymbol, buildingType);
            //        Place building = new Place(ParentQuest, source);
            //        homePlaceSymbol = building.Symbol.Clone();
            //        ParentQuest.AddResource(building);
            //    }
            //}
        }

        void LogHomePlace(Place homePlace)
        {
            QuestMachine.LogFormat(
                ParentQuest,
                "Generated Home for Person {0} [{1}] at '{2}/{3}' in building '{4}'",
                Symbol.Original,
                DisplayName,
                HomeRegionName,
                HomeTownName,
                HomeBuildingName);
        }

        void AssignGod()
        {
            godName = GetRandomGodName();
        }

        void AddConversationTopics()
        {
            List<TextFile.Token[]> anyInfoAnswers = null;
            List<TextFile.Token[]> anyRumorsAnswers = null;
            if (this.InfoMessageID != -1)
            {
                anyInfoAnswers = new List<TextFile.Token[]>();                
                Message message = this.ParentQuest.GetMessage(this.InfoMessageID);
                if (message != null)
                {
                    for (int i = 0; i < message.VariantCount; i++)
                    {
                        TextFile.Token[] tokens = message.GetTextTokensByVariant(i, false); // do not expand macros here (they will be expanded just in time by TalkManager class)
                        anyInfoAnswers.Add(tokens);
                    }
                }

                message = this.ParentQuest.GetMessage(this.RumorsMessageID);
                anyRumorsAnswers = new List<TextFile.Token[]>();
                if (message != null)
                {
                    for (int i = 0; i < message.VariantCount; i++)
                    {
                        TextFile.Token[] tokens = message.GetTextTokensByVariant(i, false); // do not expand macros here (they will be expanded just in time by TalkManager class)
                        anyRumorsAnswers.Add(tokens);
                    }
                }                
            }

            string key = this.Symbol.Name;
            GameManager.Instance.TalkManager.AddQuestTopicWithInfoAndRumors(this.ParentQuest.UID, this, key, TalkManager.QuestInfoResourceType.Person, anyInfoAnswers, anyRumorsAnswers);
        }

        Genders GetGender(string genderName)
        {
            Genders gender;
            switch (genderName)
            {
                case "female":
                    gender = Genders.Female;
                    break;
                case "male":
                    gender = Genders.Male;
                    break;
                default:
                    // Random gender
                    if (UnityEngine.Random.Range(0.0f, 1.0f) < 0.5f)
                        gender = Genders.Male;
                    else
                        gender = Genders.Female;
                    break;
            }

            return gender;
        }

        #endregion

        #region NPC Setup Methods

        // Creates an individual NPC like King Gothryd or Brisienna
        void SetupIndividualNPC(string individualNPCName)
        {
            // Get faction data
            int factionID = GetIndividualFactionID(individualNPCName);
            if (factionID != -1)
            {
                FactionFile.FactionData factionData = GetFactionData(factionID);

                // This must is an individual NPC
                if (factionData.type != (int)FactionFile.FactionTypes.Individual)
                    throw new Exception(string.Format("Named NPC {0} with FactionID {1} is not an individual NPC", individualNPCName, factionID));

                // Setup Person resource
                isIndividualNPC = true;
                this.factionTableKey = individualNPCName;
                this.factionData = factionData;
            }
            else
            {
                Debug.LogErrorFormat("SetupIndividualNPC() failed to setup {0}", individualNPCName);
            }
        }

        // Create an NPC aligned to a specific faction
        void SetupFactionAllianceNPC(string factionAllianceName)
        {
            // Get faction data
            // This also comes from a specific factionID
            int factionID = GetIndividualFactionID(factionAllianceName);
            if (factionID != -1)
            {
                FactionFile.FactionData factionData = GetFactionData(factionID);

                // Setup Person resource
                this.factionTableKey = factionAllianceName;
                this.factionData = factionData;
            }
            else
            {
                Debug.LogErrorFormat("SetupFactionAllianceNPC() failed to setup {0}", factionAllianceName);
            }
        }

        // Creates a career-based NPC like a Shopkeeper or Banker
        void SetupCareerAllianceNPC(string careerAllianceName)
        {
            // Special handling for Questor class
            // Will revisit this later when guilds are more integrated
            if (careerAllianceName.Equals("Questor", StringComparison.InvariantCultureIgnoreCase))
            {
                if (SetupQuestorNPC())
                    return;
            }

            // Get faction data
            int factionID = GetCareerFactionID(careerAllianceName);
            if (factionID != -1)
            {
                FactionFile.FactionData factionData = GetFactionData(factionID);

                // Setup Person resource
                this.factionTableKey = careerAllianceName;
                this.factionData = factionData;
            }
            else
            {
                Debug.LogErrorFormat("SetupCareerAllianceNPC() failed to setup {0}", careerAllianceName);
            }
        }

        // Creates an NPC based on faction type
        // This can yield some strange results - even Daggerfall uses it selectively
        // Some guesses have been made and blanks filled in as not all faction types even exist in game
        // For example, faction types 11, 12, 13 have no matching faction type and are never used in quests
        // Redirecting these to known and similar factions to ensure an NPC is created over crashing the game
        void SetupFactionTypeNPC(string factionTypeName)
        {
            // Get faction data
            int factionID = GetFactionTypeFactionID(factionTypeName);
            if (factionID != -1)
            {
                FactionFile.FactionData factionData = GetFactionData(factionID);

                // Setup Person resource
                this.factionTableKey = factionTypeName;
                this.factionData = factionData;
            }
            else
            {
                Debug.LogErrorFormat("SetupFactionTypeNPC() failed to setup {0}", factionTypeName);
            }
        }

        // Creates NPC as Questor, a special Person resource mapped to an NPC who exists full-time in world
        // The QuestorData pack should have enough information for quest system to identify NPC in world
        // Questor NPC data is derived from last NPC player clicked, this seems to fit Daggerfall which
        // can offer quests from certain types of NPC at random in addition to usual guild questors.
        bool SetupQuestorNPC()
        {
            // Must have a questor set
            if (QuestMachine.Instance.LastNPCClicked == null)
            {
                Debug.LogErrorFormat("Quest Person _{0}_ is expecting a Questor NPC, but one has not been clicked. Proceeding with a virtual NPC so quest will compile.", Symbol.Name);
                return false;
            }

            // Set questor data
            questorData = QuestMachine.Instance.LastNPCClicked.Data;
            isQuestor = true;

            // Setup Person resource
            FactionFile.FactionData factionData = GetFactionData(questorData.factionID);
            this.factionData = factionData;
            nameSeed = questorData.nameSeed;
            npcGender = questorData.gender;

            return true;
        }

        // Gets live faction data from player entity for faction ID
        static public FactionFile.FactionData GetFactionData(int factionID)
        {
            FactionFile.FactionData factionData;
            if (!GameManager.Instance.PlayerEntity.FactionData.GetFactionData(factionID, out factionData) && factionID != 0)
                throw new Exception(string.Format("Could not find faction data for FactionID {0}", factionID));

            return factionData;
        }

        public static string GetRandomGodName()
        {
            const int minGodID = 4077;
            const int maxGodID = 4084;

            // Select a random god for this NPC
            int godID = UnityEngine.Random.Range(minGodID, maxGodID + 1);
            return DaggerfallUnity.Instance.TextProvider.GetRandomText(godID);
        }

        #endregion

        #region FactionID Lookups

        // Gets factionID of an individual NPC
        static public int GetIndividualFactionID(string individualNPCName)
        {
            // P3 is individual factionID
            Table factionsTable = QuestMachine.Instance.FactionsTable;
            if (factionsTable.HasValue(individualNPCName))
            {
                return Parser.ParseInt(factionsTable.GetValue("p3", individualNPCName));
            }
            else
            {
                Debug.LogErrorFormat("Could not find individualNPCName {0}", individualNPCName);
                return -1;
            }
        }

        // Gets factionID of a faction type NPC
        int GetFactionTypeFactionID(string factionTypeName)
        {
            // Only allowing a small range of random faction types for now
            FactionFile.FactionTypes[] randomFactionTypes = new FactionFile.FactionTypes[]
            {
                FactionFile.FactionTypes.Courts,
                FactionFile.FactionTypes.Province,
                FactionFile.FactionTypes.People,
                FactionFile.FactionTypes.Temple,
            };

            // P3 is faction type
            int factionType;
            Table factionsTable = QuestMachine.Instance.FactionsTable;
            if (factionsTable.HasValue(factionTypeName))
            {
                factionType = Parser.ParseInt(factionsTable.GetValue("p3", factionTypeName));
            }
            else
            {
                Debug.LogErrorFormat("Could not find factionTypeName {0}", factionTypeName);
                return -1;
            }

            // Handle random faction type
            // This selects from a restricted pool to ensure vital NPCs don't get randomly selected
            if (factionType == -1)
            {
                factionType = (int)UnityEngine.Random.Range(0, randomFactionTypes.Length);
            }

            // Assign factionID based on factionType
            // This value is 0-15 and maps to "type:" in faction.txt
            // Daggerfall seems to largely select from selected pool of factionType objects at random
            // But some factionTypes do not exist in file and suspect special handling for others
            // Treating on a case-by-case basis for now
            switch ((FactionFile.FactionTypes)factionType)
            {
                // These faction types do not generally have a specific region associated with them
                // Select from pool of all objects this faction type
                case FactionFile.FactionTypes.Daedra:
                case FactionFile.FactionTypes.Group:
                case FactionFile.FactionTypes.Subgroup:
                case FactionFile.FactionTypes.Official:
                case FactionFile.FactionTypes.Temple:
                    return GetRandomFactionOfType(factionType);

                // Faction type of God is never used in quests
                // Many of these factions do not have an NPC flat
                // Recommend never using - not sure how to redirect to working state yet
                case FactionFile.FactionTypes.God:
                    return GetRandomFactionOfType(factionType);

                // The individual type is not used by any canonical quests
                // It is more or less equivalent to reserving an individual NPC
                // Not recommended to use this in quests
                // Just returning a random person to ensure NPC is created
                case FactionFile.FactionTypes.Individual:
                    return GetRandomFactionOfType(factionType);

                // Not sure how to use vampire clans yet
                // These are *mostly* used by vampire quests where its assumed the player's vampire faction will be used
                // It wouldn't make sense for player to gain reputation with another vampire clan after all
                // As vampire factions not in game yet, just select one at random to ensure NPC is created
                case FactionFile.FactionTypes.VampireClan:
                    return GetRandomFactionOfType(factionType);

                // Assign an NPC from current player region
                case FactionFile.FactionTypes.Province:
                    return GetCurrentRegionFaction();

                // Not all regions have a witches coven associated
                // Just select a random coven for now
                case FactionFile.FactionTypes.WitchesCoven:
                    return GetRandomFactionOfType(factionType);

                // Type 10 Knightly_Guard does not exist in FACTION.TXT but IS used in some quests
                // Redirecting this to "Generic Knightly Order" #844 to ensure NPC is created
                case FactionFile.FactionTypes.KnightlyGuard:
                    return 844;

                // Type 11 Magic_User does not exist in FACTION.TXT and is not used in any quests
                // Redirecting this to "Mages Guild" #40 to ensure NPC is created
                case FactionFile.FactionTypes.MagicUser:
                    return 40;

                // Type 12 Generic_Group does not exist in FACTION.TXT and is not used in any quests
                // Redirecting this to a random choice between "Generic Temple" #450 and "Generic Knightly Order" #844 to ensure NPC is created
                case FactionFile.FactionTypes.Generic:
                    return (UnityEngine.Random.Range(0f, 1f) < 0.5f) ? 450 : 844;

                // Type 13 Thieves_Den does not exist in FACTION.TXT and is not used in any quests
                // Redirecting this to "Thieves Guild" #42 to ensure NPC is created
                case FactionFile.FactionTypes.Thieves:
                    return 42;

                // Get "court of" current region
                case FactionFile.FactionTypes.Courts:
                    return GetCourtOfCurrentRegion();

                // Get "people of" current region
                case FactionFile.FactionTypes.People:
                    return GameManager.Instance.PlayerGPS.GetPeopleOfCurrentRegion();

                // Give up
                default:
                    return -1;
            }
        }

        // Gets factionID of a career NPC
        int GetCareerFactionID(string careerAllianceName)
        {
            const int magesGuild = 40;
            const int genericTemple = 450;
            const int merchants = 510;

            // P2 is careerID
            int careerID;
            Table factionsTable = QuestMachine.Instance.FactionsTable;
            if (factionsTable.HasValue(careerAllianceName))
            {
                careerID = Parser.ParseInt(factionsTable.GetValue("p2", careerAllianceName));
            }
            else
            {
                Debug.LogErrorFormat("Could not find careerAllianceName {0}", careerAllianceName);
                return -1;
            }

            // Assign factionID based on careerID
            // How Daggerfall links these is not 100% confirmed, some guesses below
            // Most of these NPC careers seem to be aligned with faction #510 Merchants
            switch (careerID)
            {
                case 0:
                case 2:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 12:
                case 13:
                case 15:
                    return merchants;                       // Merchants
                case 11:
                    return magesGuild;                      // Mages Guild
                case 14:
                    return genericTemple;                   // Generic Temple seems to link all the temples together
                case 16:
                    return GetCourtOfCurrentRegion();       // Not sure if "Noble" career maps to regional "court of" in classic
                case 17:
                case 18:
                case 19:
                case 20:
                                                            // Default for everything else will just be fairly generic "people of" faction
                                                            // This at least ensures the object will compile to something valid
                default:                                    // Not sure if "Resident1-4" career really maps to regional "people of" in classic
                    return GameManager.Instance.PlayerGPS.GetPeopleOfCurrentRegion();
            }
        }

        int GetCurrentRegionFaction()
        {
            int oneBasedPlayerRegion = GameManager.Instance.PlayerGPS.CurrentRegionIndex;
            FactionFile.FactionData[] factions = GameManager.Instance.PlayerEntity.FactionData.FindFactions(
                (int)FactionFile.FactionTypes.Province, -1, -1, oneBasedPlayerRegion);

            // Should always find a single region
            if (factions == null || factions.Length != 1)
                throw new Exception("GetCurrentRegionFaction() did not find exactly 1 match.");

            return factions[0].id;
        }

        // Gets the noble court faction in current region
        int GetCourtOfCurrentRegion()
        {
            // Find court in current region
            int oneBasedPlayerRegion = GameManager.Instance.PlayerGPS.CurrentRegionIndex;
            FactionFile.FactionData[] factions = GameManager.Instance.PlayerEntity.FactionData.FindFactions(
                (int)FactionFile.FactionTypes.Courts,
                (int)FactionFile.SocialGroups.Nobility,
                (int)FactionFile.GuildGroups.Region,
                oneBasedPlayerRegion);

            // Should always find a single court
            if (factions == null || factions.Length != 1)
                throw new Exception("GetCourtOfCurrentRegion() did not find exactly 1 match.");

            return factions[0].id;
        }

        int GetRandomFactionOfType(int factionType)
        {
            // Find all factions of type
            FactionFile.FactionData[] factions = GameManager.Instance.PlayerEntity.FactionData.FindFactions(factionType);

            // Should always find at least one
            if (factions == null || factions.Length == 0)
                throw new Exception("GetRandomFactionOfType() found 0 matches.");

            return factions[UnityEngine.Random.Range(0, factions.Length)].id;
        }

        #endregion

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public Races race;
            public NameHelper.BankTypes nameBank;
            public Genders npcGender;
            public int faceIndex;
            public int nameSeed;
            public bool isQuestor;
            public bool isIndividualNPC;
            public bool isIndividualAtHome;
            public string displayName;
            public string godName;
            public Symbol homePlaceSymbol;
            public Symbol lastAssignedPlaceSymbol;
            public bool assignedToHome;
            public int factionID;
            public string factionTableKey;
            public StaticNPC.NPCData questorData;
            public bool discoveredThroughTalkManager;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.race = race;
            data.nameBank = nameBank;
            data.npcGender = npcGender;
            data.faceIndex = faceIndex;
            data.nameSeed = nameSeed;
            data.isQuestor = isQuestor;
            data.isIndividualNPC = isIndividualNPC;
            data.isIndividualAtHome = isIndividualAtHome;
            data.displayName = displayName;
            data.godName = godName;
            data.homePlaceSymbol = homePlaceSymbol;
            data.lastAssignedPlaceSymbol = lastAssignedPlaceSymbol;
            data.assignedToHome = assignedToHome;
            data.factionID = factionData.id;
            data.factionTableKey = factionTableKey;
            data.questorData = questorData;
            data.discoveredThroughTalkManager = discoveredThroughTalkManager;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            
            FactionFile.FactionData dsfactionData;
            if (!GameManager.Instance.PlayerEntity.FactionData.GetFactionData(data.factionID, out dsfactionData) && data.factionID != 0)
                throw new Exception("Could not deserialize Person resource FactionID to FactionData");

            race = data.race;
            nameBank = data.nameBank;
            npcGender = data.npcGender;
            faceIndex = data.faceIndex;
            nameSeed = data.nameSeed;
            isQuestor = data.isQuestor;
            isIndividualNPC = data.isIndividualNPC;
            isIndividualAtHome = data.isIndividualAtHome;
            displayName = data.displayName;
            godName = data.godName;
            homePlaceSymbol = data.homePlaceSymbol;
            lastAssignedPlaceSymbol = data.lastAssignedPlaceSymbol;
            assignedToHome = data.assignedToHome;
            factionData = dsfactionData;
            factionTableKey = data.factionTableKey;
            questorData = data.questorData;
            discoveredThroughTalkManager = data.discoveredThroughTalkManager;
        }

        #endregion
    }
}