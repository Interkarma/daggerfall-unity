// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    DFIronman, Hazelnut
// 
// Notes:
// 

using System.Text.RegularExpressions;
using DaggerfallWorkshop.Game.Entity;
using FullSerializer;
using System;

namespace DaggerfallWorkshop.Game.Questing
{
    /// <summary>
    /// Changes a foe's team.
    /// </summary>
    public class ChangeFoeTeam : ActionTemplate
    {
        Symbol npcSymbol;
        int teamNumber;

        public override string Pattern
        {
            get
            {
                return @"change foe (?<anNPC>[a-zA-Z0-9_.-]+) team (?<teamNumber>\d+)|" +
                       @"change foe (?<anNPC>[a-zA-Z0-9_.-]+) team (?<teamName>\w+)";
            }
        }

        public ChangeFoeTeam(Quest parentQuest)
            : base(parentQuest)
        {
        }

        public override IQuestAction CreateNew(string source, Quest parentQuest)
        {
            // Source must match pattern
            Match match = Test(source);
            if (!match.Success)
                return null;

            // Factory new action
            ChangeFoeTeam action = new ChangeFoeTeam(parentQuest);
            action.npcSymbol = new Symbol(match.Groups["anNPC"].Value);
            action.teamNumber = Parser.ParseInt(match.Groups["teamNumber"].Value);

            // Resolve static message back to ID
            string teamName = match.Groups["teamName"].Value;
            if (teamName != "")
            {
                if (!Enum.IsDefined(typeof(MobileTeams), teamName))
                {
                    SetComplete();
                    throw new Exception(string.Format("ChangeFoeTeam: Team {0} is not a known team from MobileTeams enum.", teamName));
                }
                action.teamNumber = (int)Enum.Parse(typeof(MobileTeams), teamName);
            }

            return action;
        }

        public override void Update(Task caller)
        {
            base.Update(caller);

            // Get related Foe resource
            Foe foe = ParentQuest.GetFoe(npcSymbol);
            if (foe == null)
                return;

            foreach (DaggerfallEnemy enemy in UnityEngine.Object.FindObjectsOfType<DaggerfallEnemy>())
            {
                if (enemy.QuestSpawn)
                {
                    QuestResourceBehaviour qrb = enemy.GetComponent<QuestResourceBehaviour>();
                    if (qrb && qrb.TargetSymbol == foe.Symbol)
                    {
                        DaggerfallEntityBehaviour deb = enemy.GetComponent<DaggerfallEntityBehaviour>();
                        if (deb)
                        {
                            EnemyEntity enemyEntity = deb.Entity as EnemyEntity;
                            //Debug.Log("Changed team from " + enemyEntity.Team + " to " + (MobileTeams)teamNumber);
                            enemyEntity.Team = (MobileTeams)teamNumber;

                            SetComplete();
                        }
                    }
                }
            }
        }

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public Symbol npcSymbol;
            public int teamNumber;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.npcSymbol = npcSymbol;
            data.teamNumber = teamNumber;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            npcSymbol = data.npcSymbol;
            teamNumber = data.teamNumber;
        }

        #endregion
    }
}