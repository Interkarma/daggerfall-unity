// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2016 Daggerfall Workshop
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
using System.Collections;
using System.Collections.Generic;

namespace DaggerfallWorkshop.Game.Questing
{
    /// <summary>
    /// Track the 64 global variables shared by all quest state.
    /// </summary>
    public class Globals
    {
        #region Fields

        const int globalCount = 64;
        const int variableUnset = 0;
        const int variableSet = 1;

        GlobalVariable[] variables = new GlobalVariable[globalCount];
        Dictionary<string, int> variableNameIndexDict = new Dictionary<string, int>();

        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of global variables.
        /// </summary>
        public int GlobalCount
        {
            get { return globalCount; }
        }

        #endregion

        #region Structures

        /// <summary>
        /// Defines a single global variable for storage.
        /// Global variables are instantiated from quest source data.
        /// Global variable state is serialized with save-game system.
        /// </summary>
        struct GlobalVariable
        {
            public int ID;
            public string Name;
            public int State;
        }

        #endregion

        #region Constructors

        public Globals()
        {
            InitGlobalVariables();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets index of variable from name.
        /// </summary>
        /// <param name="name">Name of variable.</param>
        /// <returns>Index of variable or -1 if not found.</returns>
        public int GetIndex(string name)
        {
            if (variableNameIndexDict.ContainsKey(name))
                return variableNameIndexDict[name];

            return -1;
        }

        /// <summary>
        /// Checks if variable name exists.
        /// </summary>
        /// <param name="name">Name of variable.</param>
        /// <returns>True if variable exists.</returns>
        public bool HasVariable(string name)
        {
            int index = GetIndex(name);
            if (index == -1)
                return false;

            return true;
        }

        /// <summary>
        /// Sets global variable state by index.
        /// </summary>
        /// <param name="index">Index of variable.</param>
        public void Set(int index)
        {
            GlobalVariable variable = variables[index];
            variable.State = variableSet;
            variables[index] = variable;
        }

        /// <summary>
        /// Sets global variable state by name.
        /// </summary>
        /// <param name="name">Name of variable.</param>
        public void Set(string name)
        {
            Set(GetIndex(name));
        }

        /// <summary>
        /// Unsets global variable state by index.
        /// </summary>
        /// <param name="index">Index of variable.</param>
        public void Unset(int index)
        {
            GlobalVariable variable = variables[index];
            variable.State = variableUnset;
            variables[index] = variable;
        }

        /// <summary>
        /// Unsets global variable state by name.
        /// </summary>
        /// <param name="name">Name of variable.</param>
        public void Unset(string name)
        {
            Unset(GetIndex(name));
        }

        /// <summary>
        /// Checks if variable is set by index.
        /// </summary>
        /// <param name="index">Index of variable.</param>
        /// <returns>True if variable set.</returns>
        public bool IsSet(int index)
        {
            GlobalVariable variable = variables[index];
            return (variable.State == variableSet);
        }

        /// <summary>
        /// Checks if variable is set by name.
        /// </summary>
        /// <param name="name">Name of variable.</param>
        /// <returns>True if variable set.</returns>
        public bool IsSet(string name)
        {
            return IsSet(GetIndex(name));
        }

        #endregion

        #region Private Methods

        void InitGlobalVariables()
        {
            string[] lines = QuestMachine.Instance.GetQuestDataText(QuestMachine.GlobalsDataName);
            foreach(string line in lines)
            {
                // Skip comment lines
                string text = line.Trim();
                if (text.StartsWith("--"))
                    continue;

                // Skip empty lines
                if (string.IsNullOrEmpty(text))
                    continue;

                // Split line by whitespace
                // Expects format ID, GlobalName
                string[] tokens = text.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length != 2)
                    throw new Exception("Invalid number of tokens during InitGlobalVariables()");

                // Setup global variable
                GlobalVariable variable = new GlobalVariable();
                variable.ID = int.Parse(tokens[0]);
                variable.Name = tokens[1];
                variable.State = variableUnset;

                // Check name is unique
                if (variableNameIndexDict.ContainsKey(variable.Name))
                    throw new Exception(string.Format("Duplicate global variable name '{0}'", variable.Name));

                // Add global variable and name-id
                variables[variable.ID] = variable;
                variableNameIndexDict.Add(variable.Name, variable.ID);
            }
        }

        #endregion
    }
}