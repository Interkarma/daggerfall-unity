// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2023 Daggerfall Workshop
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
using System.Linq;
using System.Text.RegularExpressions;
using DaggerfallConnect;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallConnect.Arena2;
using FullSerializer;
using DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects;
using DaggerfallWorkshop.Game.Guilds;

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
        Symbol homePlaceSymbol = null;
        Symbol lastAssignedPlaceSymbol = null;
        bool assignedToHome = false;
        string factionTableKey = string.Empty;
        FactionFile.FactionData factionData;
        StaticNPC.NPCData questorData;
        bool discoveredThroughTalkManager = false;
        bool isMuted = false;
        bool isDestroyed = false;

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
            set { isQuestor = value; }
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

        public string HomeTownName
        {
            get { return GetHomePlaceLocationName(); }
        }

        public string HomeRegionName
        {
            get { return GetHomePlaceRegionName(); }
        }

        public int HomeRegionIndex
        {
            get { return GetHomePlaceRegionIndex(); }
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

        public bool IsMuted
        {
            get { return isMuted; }
            set { isMuted = value; }
        }

        public bool IsDestroyed
        {
            get { return isDestroyed; }
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
            string locationScopeName = string.Empty;

            base.SetResource(line);

            // Match strings
            string declMatchStr = @"(Person|person) (?<symbol>[a-zA-Z0-9'_.-]+)";
            string optionsMatchStr = @"named (?<individualNPCName>[a-zA-Z0-9'_.-]+)|" +
                                     @"face (?<faceIndex>\d+)|" +
                                     @"(factionType|factiontype) (?<factionType>[a-zA-Z0-9'_.-]+)|" +
                                     @"faction (?<factionAlliance>[a-zA-Z0-9'_.-]+)|" +
                                     @"group (?<careerAlliance>[a-zA-Z0-9'_.-]+)|" +
                                     @"(?<gender>female|male)|" +
                                     @"(?<locationScope>local|remote)|" +
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

                    // Location
                    Group locationGroup = option.Groups["locationScope"];
                    if (locationGroup.Success)
                        locationScopeName = locationGroup.Value;

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
                AssignHomeTown(locationScopeName);

                // Is NPC at home?
                isIndividualAtHome = atHome;

                // Done
                Debug.LogFormat("Created NPC {0} with FactionID #{1}.", displayName, factionData.id);
            }
        }

        public override bool ExpandMacro(MacroTypes macro, out string textOut)
        {
            // TODO:
            //  * Support for home town/building (believe this is just random unless NPC moved from a Place)
            //  * Support for pronoun (%g1, %g2, %g2, %g2self, %g3)
            //  * Support for class (not sure what NPCs have a class, need to see this used in a quest)
            //  * Support for faction (believe this is just the name of faction they belong to, e.g. The Merchants)

            // Store this person in quest as last Person encountered
            // This will be used for subsequent pronoun macros, etc.
            ParentQuest.LastResourceReferenced = this;

            Place dialogPlace = GetDialogPlace();
            if (dialogPlace != null)
            {
                ParentQuest.LastPlaceReferenced = dialogPlace;
            }

            textOut = string.Empty;
            bool result = true;
            switch (macro)
            {
                case MacroTypes.NameMacro1:             // Display name
                    textOut = displayName;
                    break;

                case MacroTypes.NameMacro2:             // building name
                    textOut = (dialogPlace != null ? dialogPlace.SiteDetails.buildingName : BLANK);
                    break;

                case MacroTypes.NameMacro3:             // town name
                    textOut = (dialogPlace != null ? TextManager.Instance.GetLocalizedLocationName(dialogPlace.SiteDetails.mapId, dialogPlace.SiteDetails.locationName) : BLANK);
                    break;

                case MacroTypes.NameMacro4:             // region name
                    textOut = (dialogPlace != null ? TextManager.Instance.GetLocalizedRegionName(dialogPlace.SiteDetails.regionIndex) : BLANK);
                    break;

                case MacroTypes.DetailsMacro:           // Details macro
                    textOut = GetFlatDetailsString();
                    break;

                case MacroTypes.FactionMacro:           // Faction macro
                    if (isQuestor)
                    { 
                        // Want name of guild, not the person
                        FactionFile.FactionData guildData;
                        if (GameManager.Instance.PlayerEntity.FactionData.GetFactionData(ParentQuest.FactionId, out guildData))
                            textOut = guildData.name;
                        else
                            result = false;
                    }
                    else
                    {
                        textOut = factionData.name;
                    }
                    break;

                default:                                // Macro not supported
                    result = false;
                    break;
            }

            return result;
        }

        string GetFlatDetailsString()
        {
            // Get billboard texture data
            FactionFile.FlatData flatData;
            if (IsIndividualNPC)
            {
                // Individuals are always flat1 no matter gender
                flatData = FactionFile.GetFlatData(FactionData.flat1);
            }
            if (Gender == Genders.Male)
            {
                // Male has flat1
                flatData = FactionFile.GetFlatData(FactionData.flat1);
            }
            else
            {
                // Female has flat2
                flatData = FactionFile.GetFlatData(FactionData.flat2);
            }

            // Get flat ID for this person
            int flatID = FlatsFile.GetFlatID(flatData.archive, flatData.record);

            // Get flat caption for this ID, e.g. "young lady in green", or fallback to race
            FlatsFile.FlatData flatCFG;
            if (DaggerfallUnity.Instance.ContentReader.FlatsFileReader.GetFlatData(flatID, out flatCFG))
                return TextManager.Instance.GetLocalizedText(flatID.ToString(), TextCollections.TextFlats);
            else
                return RaceTemplate.GetRaceDictionary()[(int)race].Name;
        }

        public override void Tick(Quest caller)
        {
            base.Tick(caller);

            // Auto-assign NPC to home Place if available and player enters
            // (but only if not already placed, e.g. by PlaceNpc action)
            // This only happens for very specific NPC types
            // Equivalent to calling "place anNPC at aPlace" from script
            // Will not be called again as assignment is permanent for duration of quest
            if (lastAssignedPlaceSymbol == null && homePlaceSymbol != null && !assignedToHome)
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
        /// Gets best Place resource for use in dialog.
        /// </summary>
        /// <returns>Assigned Place resource or Home Place resource if not assigned.</returns>
        public Place GetDialogPlace()
        {
            if (lastAssignedPlaceSymbol != null)
            {
                return ParentQuest.GetPlace(lastAssignedPlaceSymbol);
            }

            return GetHomePlace();
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

        /// <summary>
        /// Set Person.IsDestroyed=true.
        /// Person can no longer be placed or clicked.
        /// </summary>
        public void DestroyNPC()
        {
            isDestroyed = true;
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
        /// Gets region index of home Place (if any).
        /// </summary>
        /// <returns>Region index of home Place or -1 if none set or not resolved.</returns>
        int GetHomePlaceRegionIndex()
        {
            Place place = GetHomePlace();
            if (place == null)
                return -1;

            return DaggerfallUnity.Instance.ContentReader.MapFileReader.GetRegionIndex(place.SiteDetails.regionName);
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
            // Try to get the race from the current faction
            race = RaceTemplate.GetRaceFromFactionRace((FactionFile.FactionRaces)factionData.race);
            if (race == Races.None)
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

        void AssignHomeTown(string scopeString)
        {
            const string houseString = "house";

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

            // If this is an individual NPC who is not at home, don't place for now
            // as this has to be done by the "place _person_ at" command
            if (IsIndividualNPC)
                return;

            // For other NPCs use the given scope if any 
            if (string.IsNullOrEmpty(scopeString))
            {
                // Else generate it at random only if there are local buildings
                if (GameManager.Instance.PlayerGPS.HasCurrentLocation &&
                    GameManager.Instance.PlayerGPS.CurrentLocation.Exterior.BuildingCount > 0)
                {
                    scopeString = UnityEngine.Random.Range(0.0f, 1.0f) < 0.5f ? "local" : "remote";
                }
                else
                {
                    scopeString = "remote";
                }
            }

            // Adjust building type based on faction hints
            string buildingTypeString = houseString;
            int p1 = 0, p2 = 0, p3 = 0;
            if (!string.IsNullOrEmpty(factionTableKey))
            {
                // Get faction parameters
                p1 = Parser.ParseInt(QuestMachine.Instance.FactionsTable.GetValue("p1", factionTableKey));
                p2 = Parser.ParseInt(QuestMachine.Instance.FactionsTable.GetValue("p2", factionTableKey));
                p3 = Parser.ParseInt(QuestMachine.Instance.FactionsTable.GetValue("p3", factionTableKey));
                if (p1 == 0 && p2 >= 0 && p2 <= 20 && p3 == 0)
                {
                    // Set to a specific building type
                    buildingTypeString = QuestMachine.Instance.PlacesTable.GetKeyForValue("p2", p2.ToString());
                }
            }

            // Create the home location - this will try to match NPC group (e.g. a Noble will select a Palace)
            try
            {
                // Try preferred location type
                string source = string.Format("Place {0} {1} {2}", symbolName, scopeString, buildingTypeString);
                homePlace = new Place(ParentQuest, source);
            }
            catch
            {
                // Otherwise try to use a generic house
                // If this doesn't work for some reason then next exception will prevent quest from starting
                string source = string.Format("Place {0} {1} {2}", symbolName, scopeString, houseString);
                homePlace = new Place(ParentQuest, source);
            }

            // Complete assigning home place
            homePlaceSymbol = homePlace.Symbol.Clone();
            ParentQuest.AddResource(homePlace);
            LogHomePlace(homePlace);
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

                // This must be an individual NPC or Daedra
                if (factionData.type != (int) FactionFile.FactionTypes.Individual 
                    && factionData.type != (int) FactionFile.FactionTypes.Daedra)
                {
                    throw new Exception(string.Format("Named NPC {0} with FactionID {1} is not an individual NPC or Daedra", individualNPCName, factionID));
                }

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

                // Classic is bugged there as it always selects a random vampire clan.
                // Here we fix it by using player vampire clan for vampire and cure vampirism quests
                // and by using the vampire clan affiliated to the current region otherwise.
                case FactionFile.FactionTypes.VampireClan:
                    if (ParentQuest.QuestName.StartsWith("P0") || ParentQuest.QuestName.StartsWith("$CUREVAM"))
                    {
                        RacialOverrideEffect racialEffect = GameManager.Instance.PlayerEffectManager.GetRacialOverrideEffect();
                        return (int)(racialEffect as VampirismEffect).VampireClan;
                    }
                    else
                    { 
                        return GameManager.Instance.PlayerGPS.GetCurrentRegionVampireClan();
                    }

                // Assign an NPC from current player region
                case FactionFile.FactionTypes.Province:
                    return GameManager.Instance.PlayerGPS.GetCurrentRegionFaction();

                // Not all regions have a witches coven associated
                // Just select a random coven for now
                case FactionFile.FactionTypes.WitchesCoven:
                    return GetRandomFactionOfType(factionType);

                // Type 10 Knightly_Guard does not exist in FACTION.TXT but IS used in some quests
                // Redirecting this to a random knightly order to ensure NPC is created and faction is properly resolved
                case FactionFile.FactionTypes.KnightlyGuard:
                    var knightlyOrderIds = (int[])Enum.GetValues(typeof(KnightlyOrder.Orders));
                    return knightlyOrderIds[UnityEngine.Random.Range(0, knightlyOrderIds.Length)];

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
                    return GameManager.Instance.PlayerGPS.GetCourtOfCurrentRegion();

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
            const int nobles = 242;
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

            // Initial handling for Local_X.X career groups pending further review and information
            // These appear to use P3 for career association and will all resolve to Merchants for now
            // P3=0 (Apothecary), P3=1 (Town1), P3=2 (Armory), P3=3 (Bank), P3=10000 (unused in any quests)
            if (careerID < 0)
                careerID = Parser.ParseInt(factionsTable.GetValue("p3", careerAllianceName));

            // Handle Local_4.10k - unused in any quests and will default to Merchants for now
            if (careerID == 10000)
                careerID = 0;

            // Assign factionID based on careerID
            // How Daggerfall links these is not 100% confirmed, some guesses below
            // Most of these NPC careers seem to be aligned with faction #510 Merchants
            switch (careerID)
            {
                case 0:
                case 1:
                case 2:
                case 3:
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
                    return nobles;                          // Random Noble
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

        int GetRandomFactionOfType(int factionType)
        {
            // Find all factions of type
            FactionFile.FactionData[] factions = GameManager.Instance.PlayerEntity.FactionData.FindFactions(factionType);

            // Strip "Generic Temple" from random Temple pool
            if (factionType == (int)FactionFile.FactionTypes.Temple)
                factions = factions.Where(val => val.id != 450).ToArray();

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
            public Symbol homePlaceSymbol;
            public Symbol lastAssignedPlaceSymbol;
            public bool assignedToHome;
            public int factionID;
            public string factionTableKey;
            public StaticNPC.NPCData questorData;
            public bool discoveredThroughTalkManager;
            public bool isMuted;
            public bool isDestroyed;
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
            data.homePlaceSymbol = homePlaceSymbol;
            data.lastAssignedPlaceSymbol = lastAssignedPlaceSymbol;
            data.assignedToHome = assignedToHome;
            data.factionID = factionData.id;
            data.factionTableKey = factionTableKey;
            data.questorData = questorData;
            data.discoveredThroughTalkManager = discoveredThroughTalkManager;
            data.isMuted = isMuted;
            data.isDestroyed = isDestroyed;

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
            homePlaceSymbol = data.homePlaceSymbol;
            lastAssignedPlaceSymbol = data.lastAssignedPlaceSymbol;
            assignedToHome = data.assignedToHome;
            factionData = dsfactionData;
            factionTableKey = data.factionTableKey;
            questorData = data.questorData;
            discoveredThroughTalkManager = data.discoveredThroughTalkManager;
            isMuted = data.isMuted;
            isDestroyed = data.isDestroyed;
        }

        #endregion
    }
}