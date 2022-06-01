// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using System;
using System.Collections.Generic;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Questing;
using DaggerfallConnect.Save;

namespace DaggerfallWorkshop.Game.Player
{
    /// <summary>
    /// Stores global variables for PlayerEntity.
    /// Could have just used a bool[64] array but this design conveys more information.
    /// </summary>
    public class PersistentGlobalVars
    {
        const int totalGlobalVars = 64;

        readonly Dictionary<int, GlobalVar> globalVarsDict = new Dictionary<int, GlobalVar>();

        /// <summary>
        /// Gets full details of a global variable.
        /// </summary>
        public GlobalVar GetGlobalVarDetails(int key)
        {
            if (!globalVarsDict.ContainsKey(key))
                throw new Exception(string.Format("Could not locate key {0} in GlobalVars", key));

            return globalVarsDict[key];
        }

        /// <summary>
        /// Get current value of global variable.
        /// </summary>
        /// <returns></returns>
        public bool GetGlobalVar(int key)
        {
            return globalVarsDict[key].value;
        }

        /// <summary>
        /// Sets a global variable value.
        /// </summary>
        public void SetGlobalVar(int key, bool value)
        {
            GlobalVar gv = GetGlobalVarDetails(key);
            gv.value = value;
            globalVarsDict.Remove(key);
            globalVarsDict.Add(gv.index, gv);
        }

        /// <summary>
        /// Resets global vars back to starting point.
        /// Must call this at least once before using global vars.
        /// </summary>
        public void Reset()
        {
            // Get source table
            Table globalVarsTable = QuestMachine.Instance.GlobalVarsTable;

            // Rebuild global vars dict
            globalVarsDict.Clear();
            for (int i = 0; i < totalGlobalVars; i++)
            {
                string[] row = globalVarsTable.GetRow(i);
                GlobalVar gv = new GlobalVar();
                gv.index = i;
                gv.name = row[1];
                gv.value = false;
                globalVarsDict.Add(i, gv);
            }
        }

        /// <summary>
        /// Get global variables as a flat array for serialization.
        /// </summary>
        public GlobalVar[] SerializeGlobalVars()
        {
            List<GlobalVar> globalVarsList = new List<GlobalVar>();
            foreach (GlobalVar gv in globalVarsDict.Values)
            {
                globalVarsList.Add(gv);
            }

            return globalVarsList.ToArray();
        }

        /// <summary>
        /// Restore global variables from deserialized list.
        /// </summary>
        public void DeserializeGlobalVars(GlobalVar[] globalVarsList)
        {
            // Ignore an empty list from older save game and just reset to scratch
            if (globalVarsList == null || globalVarsList.Length == 0)
            {
                Reset();
                return;
            }

            // Restore items
            globalVarsDict.Clear();
            foreach (GlobalVar gv in globalVarsList)
            {
                globalVarsDict.Add(gv.index, gv);
            }
        }

        /// <summary>
        /// Import global variables from classic save.
        /// </summary>
        /// <param name="saveVars"></param>
        public void ImportClassicGlobalVars(SaveVars saveVars)
        {
            byte[] globals = saveVars.GlobalVars;
            for (int i = 0; i < globals.Length; i++)
            {
                GlobalVar globalVar = globalVarsDict[i];
                if (globals[i] == 0)
                    globalVar.value = false;
                else if (globals[i] == 1)
                    globalVar.value = true;
                else
                    throw new Exception("ImportClassicGlobalVars() Ecnountered an unexpected global variable value.");
                globalVarsDict[i] = globalVar;
            }
        }

        /// <summary>
        /// Set all global variables to false.
        /// </summary>
        public void ZeroAllGlobalVars()
        {
            for (int i = 0; i < totalGlobalVars; i++)
            {
                SetGlobalVar(i, false);
            }
        }
    }
}
