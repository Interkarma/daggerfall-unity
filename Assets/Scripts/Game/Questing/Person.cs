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
using System.Collections;
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
                                     @"faction (?<factionAlliance>\w+)|" +
                                     @"group (?<careerAlliance>\w+)|" +
                                     @"(?<gender>female|male)|" +
                                     @"(?<atHome>atHome)";

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
                        factionAllianceName = faceGroup.Value;

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
                    SetupIndividualNPC(individualNPCName, atHome);
                }
                else if (!string.IsNullOrEmpty(careerAllianceName))
                {
                    SetupCareerAllianceNPC(careerAllianceName, genderName);
                }
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
        void SetupIndividualNPC(string individualNPCName, bool atHome)
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
                displayName = factionData.name;
                isIndividualAtHome = atHome;
                this.factionData = factionData;
            }
            else
            {
                Debug.LogErrorFormat("SetupIndividualNPC() failed to setup {0}", individualNPCName);
            }
        }

        // Creates a career-based NPC like a Shopkeeper or Banker
        void SetupCareerAllianceNPC(string careerAllianceName, string genderName)
        {
            // Get faction data
            int factionID = GetCareerFactionID(careerAllianceName);
            if (factionID != -1)
            {
                FactionFile.FactionData factionData = GetFactionData(factionID);

                // Setup Person resource with a random display name
                DFRandom.srand(Time.frameCount);
                npcGender = GetGender(genderName);
                displayName = DaggerfallUnity.Instance.NameHelper.FullName(Utility.NameHelper.BankTypes.Breton, npcGender);
                this.factionData = factionData;
            }
            else
            {
                Debug.LogErrorFormat("SetupCareerAllianceNPC() failed to setup {0}", careerAllianceName);
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

        // Gets the noble court faction in current region
        int GetCourtOfCurrentRegion()
        {
            // Find court in current region
            int oneBasedPlayerRegion = GameManager.Instance.PlayerGPS.CurrentRegionIndex + 1;
            FactionFile.FactionData[] factions = GameManager.Instance.PlayerEntity.FactionData.FindFactions(
                (int)FactionFile.FactionTypes.Courts,
                (int)FactionFile.SocialGroups.Nobility,
                (int)FactionFile.GuildGroups.Region,
                oneBasedPlayerRegion);

            // Should always find a single court
            if (factions == null || factions.Length != 1)
                throw new Exception("GetCourtOfCurrentRegion() encountered did not find exactly 1 match.");

            return factions[0].id;
        }

        // Gets the people of faction in current region
        int GetPeopleOfCurrentRegion()
        {
            // Find people of current region
            int oneBasedPlayerRegion = GameManager.Instance.PlayerGPS.CurrentRegionIndex + 1;
            FactionFile.FactionData[] factions = GameManager.Instance.PlayerEntity.FactionData.FindFactions(
                (int)FactionFile.FactionTypes.People,
                (int)FactionFile.SocialGroups.Commoners,
                (int)FactionFile.GuildGroups.GeneralPopulace,
                oneBasedPlayerRegion);

            // Should always find a single people of
            if (factions == null || factions.Length != 1)
                throw new Exception("GetPeopleOfCurrentRegion() encountered did not find exactly 1 match.");

            return factions[0].id;
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