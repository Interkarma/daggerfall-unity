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
using System.IO;
using System.Text;
using System.Collections;

namespace DaggerfallWorkshop.Game.Questing
{
    /// <summary>
    /// Hosts quests and manages their execution during play.
    /// Quests are instantiated from a source text template.
    /// It's possible to have the same quest multiple times (e.g. same fetch quest from two different mage guildhalls).
    /// Running quests can perform actions in the world (e.g. spawn enemies and play sounds).
    /// Or they can provide data to external systems like the NPC dialog interface (e.g. 'tell me about' and 'rumors').
    /// Quest support is considered to be in very early prototype stages and may change at any time.
    /// </summary>
    public class QuestMachine : MonoBehaviour
    {
        public const string GlobalsDataName = "Globals";

        public string TestQuest = "_BRISIEN";       // TEMP: Test quest to parse at startup

        Globals globals;

        void Awake()
        {
            SetupSingleton();
        }

        void Start()
        {
            // Setup required data classes
            globals = new Globals();

            // Load test quest is specified
            if (!string.IsNullOrEmpty(TestQuest))
            {
                // Attempt to parse quest source
                Parser.Parse(GetQuestText(TestQuest));
            }
        }

        #region Public Methods

        /// <summary>
        /// Quests source hard-coded into Resources for now.
        /// </summary>
        public string GetQuestSourceAssetFolder()
        {
            return "Quests";
        }

        /// <summary>
        /// Quest source data hard-coded into Resources for now.
        /// </summary>
        public string GetQuestSourceDataAssetFolder()
        {
            return "Quests/Data";
        }

        /// <summary>
        /// Gets quest source text.
        /// </summary>
        /// <param name="sourceName">Source name of quest. e.g. _BRISIEN</param>
        /// <returns>Array of lines from text file.</returns>
        public string[] GetQuestText(string sourceName)
        {
            TextAsset source = Resources.Load<TextAsset>(Path.Combine(GetQuestSourceAssetFolder(), sourceName));
            return source.text.Split('\n');
        }

        /// <summary>
        /// Gets quest data source text.
        /// </summary>
        /// <param name="dataName">Name of quest data file. e.g. Globals</param>
        /// <returns>Array of lines from text file.</returns>
        public string[] GetQuestDataText(string dataName)
        {
            TextAsset source = Resources.Load<TextAsset>(Path.Combine(GetQuestSourceDataAssetFolder(), dataName));
            return source.text.Split('\n');
        }

        #endregion

        #region Private Methods
        #endregion

        #region Singleton

        static QuestMachine instance = null;
        public static QuestMachine Instance
        {
            get
            {
                if (instance == null)
                {
                    if (!FindQuestMachine(out instance))
                    {
                        GameObject go = new GameObject();
                        go.name = "QuestMachine";
                        instance = go.AddComponent<QuestMachine>();
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

        public static bool FindQuestMachine(out QuestMachine questMachineOut)
        {
            questMachineOut = GameObject.FindObjectOfType(typeof(QuestMachine)) as QuestMachine;
            if (questMachineOut == null)
            {
                DaggerfallUnity.LogMessage("Could not locate QuestMachine GameObject instance in scene!", true);
                return false;
            }

            return true;
        }

        private void SetupSingleton()
        {
            if (instance == null)
                instance = this;
            else if (instance != this)
            {
                if (Application.isPlaying)
                {
                    DaggerfallUnity.LogMessage("Multiple QuestMachine instances detected in scene!", true);
                    Destroy(gameObject);
                }
            }
        }

        #endregion
    }
}