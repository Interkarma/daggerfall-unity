// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2019 Daggerfall Workshop
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
using System.IO;
using System.Collections.Generic;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// Text manager singleton class. Provides common text services to game systems.
    /// This is likely to replace most or all functionality currently in TextProvider.
    /// </summary>
    public class TextManager : MonoBehaviour
    {
        #region Fields

        const string textFolderName = "Text";
        const string textColumn = "text";

        Dictionary<string, Table> textDatabases = new Dictionary<string, Table>();

        #endregion

        #region Unity

        private void Awake()
        {
            EnumerateTextDatabases();    
        }

        #endregion

        #region Text Database Methods

        /// <summary>
        /// Checks if text database table was found enumerated StreamingAssets/Text folder.
        /// </summary>
        /// <param name="databaseName">Name of database.</param>
        /// <returns>True if database was enumerated.</returns>
        public bool HasDatabase(string databaseName)
        {
            return textDatabases.ContainsKey(databaseName);
        }

        /// <summary>
        /// Checks if both text database and text key exists.
        /// </summary>
        /// <param name="databaseName">Name of database.</param>
        /// <param name="key">Key of text in database.</param>
        /// <returns>True if both database and text key enumerated.</returns>
        public bool HasText(string databaseName, string key)
        {
            if (!HasDatabase(databaseName))
                return false;

            return textDatabases[databaseName].HasValue(key);
        }

        /// <summary>
        /// Gets text value from database.
        /// </summary>
        /// <param name="databaseName">Name of text database.</param>
        /// <param name="key">Key of text in database.</param>
        /// <returns>Text if found, otherwise return an error string instead.</returns>
        public string GetText(string databaseName, string key)
        {
            // Show an error if text not found
            if (!HasText(databaseName, key))
                return "<TextError-NotFound>";

            return textDatabases[databaseName].GetValue(textColumn, key);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Enumerate all available text databases.
        /// </summary>
        protected void EnumerateTextDatabases()
        {
            // Get all text files in target path
            Debug.Log("TextManager enumerating text databases.");
            string path = Path.Combine(Application.streamingAssetsPath, textFolderName);
            string[] files = Directory.GetFiles(path, "*.txt");

            // Attempt to read each file as a table with a text schema
            foreach (string file in files)
            {
                try
                {
                    // Create table from text file
                    Table table = new Table(File.ReadAllLines(file));

                    // Get database key from filename
                    string databaseName = Path.GetFileNameWithoutExtension(file);
                    if (HasDatabase(databaseName))
                        throw new Exception(string.Format("TextManager database name {0} already exists.", databaseName));

                    // Assign database to collection
                    textDatabases.Add(databaseName, table);
                    Debug.LogFormat("TextManager read text database table {0} with {1} rows", databaseName, table.RowCount);
                }
                catch (Exception ex)
                {
                    Debug.LogFormat("TextManager unable to parse text database table {0} with exception message {1}", file, ex.Message);
                    continue;
                }
            }
        }

        #endregion

        #region Singleton

        static TextManager instance = null;
        public static TextManager Instance
        {
            get
            {
                if (instance == null)
                {
                    if (!FindSingleton(out instance))
                    {
                        GameObject go = new GameObject();
                        go.name = "TextManager";
                        instance = go.AddComponent<TextManager>();
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

        public static bool FindSingleton(out TextManager singletonOut)
        {
            singletonOut = GameObject.FindObjectOfType<TextManager>();
            if (singletonOut == null)
            {
                DaggerfallUnity.LogMessage("Could not locate TextManager GameObject instance in scene!", true);
                return false;
            }

            return true;
        }

        #endregion
    }
}
