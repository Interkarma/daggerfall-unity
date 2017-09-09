// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2017 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Michael Rauter (Nystul)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// this class manages talk topics and resulting actions/answers
    /// </summary>
    public class TalkManager : MonoBehaviour
    {
        #region Singleton

        static TalkManager instance = null;
        public static TalkManager Instance
        {
            get
            {
                if (instance == null)
                {
                    if (!FindTalkManager(out instance))
                    {
                        GameObject go = new GameObject();
                        go.name = "TalkManager";
                        instance = go.AddComponent<TalkManager>();
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

        public static bool FindTalkManager(out TalkManager talkManagerOut)
        {
            talkManagerOut = GameObject.FindObjectOfType<TalkManager>();
            if (talkManagerOut == null)
            {
                DaggerfallUnity.LogMessage("Could not locate TalkManager GameObject instance in scene!", true);
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
                    DaggerfallUnity.LogMessage("Multiple TalkManager instances detected in scene!", true);
                    Destroy(gameObject);
                }
            }
        }

        #endregion

        #region Fields        

        List<string> listTopicLocation;
        List<string> listTopicPeople;
        List<string> listTopicThings;

        List<string> listAnswers;

        #endregion

        #region Properties

        public List<string> ListTopicLocation
        {
            get { return listTopicLocation; }
        }

        public List<string> ListTopicPeople
        {
            get { return listTopicPeople; }
        }

        public List<string> ListTopicThings
        {
            get { return listTopicThings; }
        }

        #endregion

        #region Unity

        void Awake()
        {
            SetupSingleton();

            PrepareTestTopicLists();
        }

        void OnDestroy()
        {

        }

        void OnEnable()
        {
            
        }

        void OnDisable()
        {
            
        }

        void Start()
        {

        }

        void Update()
        {

        }

        #endregion

        #region Public Methods

        #endregion

        #region Private Methods

        void PrepareTestTopicLists()
        {
            listTopicLocation = new List<string>();
            for (int i = 0; i < 50; i++)
            {
                listTopicLocation.Add("location " + i + " test string");
            }

            listTopicPeople = new List<string>();
            for (int i = 0; i < 12; i++)
            {
                listTopicPeople.Add("dummy person " + i + " (here will be the name of the person later on)");
            }

            listTopicThings = new List<string>();
            for (int i = 0; i < 30; i++)
            {
                listTopicThings.Add("thing " + i);
            }
        }

        #endregion

        #region event handlers

        #endregion
    }
}