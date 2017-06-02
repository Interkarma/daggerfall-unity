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
            string declMatchStr = @"(Person|person) (?<symbol>\w+)";
            string optionsMatchStr = @"named (?<permanentNPCName>\w+)|" +
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

                // Match options
                Match options = Regex.Match(line, optionsMatchStr);

                // Permanent NPC
                Group permanentNPCNameGroup = options.Groups["permanentNPCName"];
                if (permanentNPCNameGroup.Success)
                    permanentNPCName = permanentNPCNameGroup.Value;

                // Face
                Group faceGroup = options.Groups["faceIndex"];
                if (faceGroup.Success)
                    faceIndex = Parser.ParseInt(faceGroup.Value);

                // Faction type
                Group factionTypeGroup = options.Groups["factionType"];
                if (factionTypeGroup.Success)
                    factionTypeName = factionTypeGroup.Value;

                // Faction alliance
                Group factionAllianceGroup = options.Groups["factionAlliance"];
                if (factionAllianceGroup.Success)
                    factionAllianceName = faceGroup.Value;

                // Group
                Group groupAllianceGroup = options.Groups["groupAlliance"];
                if (groupAllianceGroup.Success)
                    groupAllianceName = groupAllianceGroup.Value;

                // Gender
                Group genderGroup = options.Groups["gender"];
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
                Group atHomeGroup = options.Groups["atHome"];
                if (atHomeGroup.Success)
                    atHome = true;
            }
        }

        #endregion
    }
}