// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2017 Daggerfall Workshop
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
using System.Text.RegularExpressions;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Utility;
using DaggerfallConnect.Arena2;

namespace DaggerfallWorkshop.Game.Questing
{
    /// <summary>
    /// Defines an NPC involved in a quest.
    /// </summary>
    public class Person : QuestResource
    {
        #region Fields

        Genders npcGender = Genders.Male;
        int faceIndex = 0;
        bool isIndividualNPC = false;
        bool isIndividualAtHome = false;
        string displayName = string.Empty;
        bool hasPlayerClicked = false;
        FactionFile.FactionData factionData;

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

        public Genders Gender
        {
            get { return npcGender; }
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

        public bool HasPlayerClicked
        {
            get { return hasPlayerClicked; }
        }

        public FactionFile.FactionData FactionData
        {
            get { return factionData; }
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
            bool atHome = false;

            base.SetResource(line);

            // Match strings
            string declMatchStr = @"(Person|person) (?<symbol>[a-zA-Z0-9_.-]+)";
            string optionsMatchStr = @"named (?<individualNPCName>[a-zA-Z0-9_.-]+)|" +
                                     @"face (?<faceIndex>\d+)|" +
                                     @"factionType (?<factionType>\w+)|" +
                                     @"faction (?<factionAlliance>[a-zA-Z0-9_.-]+)|" +
                                     @"group (?<careerAlliance>\w+)|" +
                                     @"(?<gender>female|male)|" +
                                     @"(?<atHome>(atHome|athome))";

            // Try to match source line with pattern
            Match match = Regex.Match(line, declMatchStr);
            if (match.Success)
            {
                // Seed random
                UnityEngine.Random.InitState(Time.renderedFrameCount);

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

                // Set gender and display name
                AssignDisplayName(genderName);

                // Is NPC at home?
                isIndividualAtHome = atHome;

                // Done
                Debug.LogFormat("Created NPC {0} with FactionID #{1}.", displayName, factionData.id);
            }
        }

        public override bool ExpandMacro(MacroTypes macro, out string textOut)
        {
            textOut = string.Empty;
            bool result = true;
            switch (macro)
            {
                // TODO: Just stubbing out for testing right now as Person class not complete enough to return real values

                case MacroTypes.NameMacro1:             // Testing name
                    textOut = displayName;
                    break;

                default:                                // Macro not supported
                    result = false;
                    break;
            }

            return result;
        }

        #endregion

        #region Private Methods

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
            // Get faction data
            int factionID = GetCareerFactionID(careerAllianceName);
            if (factionID != -1)
            {
                FactionFile.FactionData factionData = GetFactionData(factionID);

                // Setup Person resource
                DFRandom.srand(Time.frameCount);
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
                this.factionData = factionData;
            }
            else
            {
                Debug.LogErrorFormat("SetupFactionTypeNPC() failed to setup {0}", factionTypeName);
            }
        }

        // Gets live faction data from player entity for faction ID
        FactionFile.FactionData GetFactionData(int factionID)
        {
            FactionFile.FactionData factionData;
            if (!GameManager.Instance.PlayerEntity.FactionData.GetFactionData(factionID, out factionData))
                throw new Exception(string.Format("Could not find faction data for FactionID {0}", factionID));

            return factionData;
        }

        // Sets gender and display name
        // Has some logic to handle individual faction objects and certain flat limitations
        void AssignDisplayName(string genderName)
        {
            // Set gender
            npcGender = GetGender(genderName);

            // Witches only have female flats
            if (factionData.type == (int)FactionFile.FactionTypes.WitchesCoven)
                npcGender = Genders.Female;

            // Assign name - some types have their own individual name to use
            if (factionData.type == (int)FactionFile.FactionTypes.Individual ||
                factionData.type == (int)FactionFile.FactionTypes.Daedra)
            {
                displayName = factionData.name;
            }
            else
            {
                DFRandom.srand(Time.frameCount);
                displayName = DaggerfallUnity.Instance.NameHelper.FullName(Utility.NameHelper.BankTypes.Breton, npcGender);
            }
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

        #region FactionID Lookups

        // Gets factionID of an individual NPC
        int GetIndividualFactionID(string individualNPCName)
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
                    return GetPeopleOfCurrentRegion();

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
            // TODO: Questor to be handled elsewhere
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
                    return GetPeopleOfCurrentRegion();      // Not sure if "Resident1-4" career map to regional "people of" in classic
                default:
                    return -1;
            }
        }

        int GetCurrentRegionFaction()
        {
            int oneBasedPlayerRegion = GameManager.Instance.PlayerGPS.CurrentOneBasedRegionIndex;
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
            int oneBasedPlayerRegion = GameManager.Instance.PlayerGPS.CurrentOneBasedRegionIndex;
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

        // Gets the people of faction in current region
        int GetPeopleOfCurrentRegion()
        {
            // Find people of current region
            int oneBasedPlayerRegion = GameManager.Instance.PlayerGPS.CurrentOneBasedRegionIndex;
            FactionFile.FactionData[] factions = GameManager.Instance.PlayerEntity.FactionData.FindFactions(
                (int)FactionFile.FactionTypes.People,
                (int)FactionFile.SocialGroups.Commoners,
                (int)FactionFile.GuildGroups.GeneralPopulace,
                oneBasedPlayerRegion);

            // Should always find a single people of
            if (factions == null || factions.Length != 1)
                throw new Exception("GetPeopleOfCurrentRegion() did not find exactly 1 match.");

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

        #region Public Methods

        public void SetPlayerClicked()
        {
            hasPlayerClicked = true;
        }

        #endregion
    }
}