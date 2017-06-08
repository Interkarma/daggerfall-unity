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
    /// A quest person is often used to assign or progress a quest.
    /// Can contain tags, such as to add info and rumours to NPC dialog.
    /// Persons can be added to world as needed or accessed from home position.
    /// </summary>
    public class Person : QuestResource
    {
        #region Fields

        string permanentNPCName = string.Empty;
        int faceIndex = 0;
        string factionTypeName = string.Empty;
        string factionAllianceName = string.Empty;
        string groupAllianceName = string.Empty;
        Genders npcGender = Genders.Male;
        bool atHome = false;

        string displayName = string.Empty;
        int individualFactionIndex = 0;

        #endregion

        #region Properties

        public string PermanentNPCName
        {
            get { return permanentNPCName; }
        }

        public int FaceIndex
        {
            get { return faceIndex; }
        }

        public string FactionTypeName
        {
            get { return factionTypeName; }
        }

        public string FactionAllianceName
        {
            get { return factionAllianceName; }
        }

        public string GroupAllianceName
        {
            get { return groupAllianceName; }
        }

        public Genders NPCGender
        {
            get { return npcGender; }
        }

        public bool AtHome
        {
            get { return atHome; }
        }

        public string DisplayName
        {
            get { return displayName; }
        }

        public int IndividualFactionIndex
        {
            get { return individualFactionIndex; }
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
            base.SetResource(line);

            // Match strings
            string declMatchStr = @"(Person|person) (?<symbol>[a-zA-Z0-9_.-]+)";
            string optionsMatchStr = @"named (?<permanentNPCName>[a-zA-Z0-9_.-]+)|" +
                                     @"face (?<faceIndex>\d+)|" +
                                     @"factionType (?<factionType>\w+)|" +
                                     @"faction (?<factionAlliance>\w+)|" +
                                     @"group (?<groupAlliance>\w+)|" +
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
                    // Permanent NPC
                    Group permanentNPCNameGroup = option.Groups["permanentNPCName"];
                    if (permanentNPCNameGroup.Success)
                        permanentNPCName = permanentNPCNameGroup.Value;

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
                    Group groupAllianceGroup = option.Groups["groupAlliance"];
                    if (groupAllianceGroup.Success)
                        groupAllianceName = groupAllianceGroup.Value;

                    // Gender
                    Group genderGroup = option.Groups["gender"];
                    if (genderGroup.Success)
                    {
                        switch (genderGroup.Value)
                        {
                            case "female":
                                npcGender = Genders.Female;
                                break;
                            case "male":
                                npcGender = Genders.Male;
                                break;
                            default:
                                // Random gender
                                if (UnityEngine.Random.Range(0.0f, 1.0f) < 0.5f)
                                    npcGender = Genders.Male;
                                else
                                    npcGender = Genders.Female;
                                break;
                        }
                    }

                    // At home
                    Group atHomeGroup = option.Groups["atHome"];
                    if (atHomeGroup.Success)
                        atHome = true;
                }

                // Get permanent NPC display name and faction index
                // This is a little clunky because display name comes from symbolic link to live faction data in player entity
                // Might review how this is handled later
                if (!string.IsNullOrEmpty(permanentNPCName))
                {
                    // Try to read place variables from data table
                    Table factionsTable = QuestMachine.Instance.FactionsTable;
                    if (factionsTable.HasValue(permanentNPCName))
                    {
                        // Get fixed NPC params
                        int p3 = Parser.ParseInt(factionsTable.GetValue("p3", permanentNPCName));

                        // Get faction info for p3
                        FactionFile.FactionData factionData;
                        if (!GameManager.Instance.PlayerEntity.FactionData.GetFactionData(p3, out factionData))
                            throw new Exception(string.Format("Could not find faction data for named NPC {0} with FactionID {1}", permanentNPCName, p3));

                        // Check this is an individual NPC
                        if (factionData.type != (int)FactionFile.FactionTypes.Individual)
                            throw new Exception(string.Format("Named NPC {0} with FactionID {1} is not an individual NPC", permanentNPCName, p3));

                        // Store permanent NPC display name
                        displayName = factionData.name;
                        individualFactionIndex = factionData.id;
                    }
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
    }
}