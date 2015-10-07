// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Daggerfall Workshop
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
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Player;

namespace DaggerfallWorkshop.Game.Utility
{
    /// <summary>
    /// This component is intended to survive from the start menu into main game.
    /// It will carry settings for new characters or detail an existing game to load.
    /// Uses DontDestroyOnLoad() to keep information live during transition.
    /// </summary>
    public class StartGameBehaviour : MonoBehaviour
    {
        StartMethods startMethod = StartMethods.Nothing;
        StartMethods lastMethod = StartMethods.Nothing;
        CharacterSheet characterSheet;
        int classicSaveIndex = -1;

        public StartMethods StartMethod
        {
            get { return startMethod; }
            set { startMethod = value; }
        }

        public CharacterSheet CharacterSheet
        {
            get { return characterSheet; }
            set { characterSheet = value; }
        }

        public int ClassicSaveIndex
        {
            get { return classicSaveIndex; }
            set { classicSaveIndex = value; }
        }

        public enum StartMethods
        {
            Nothing,
            DefaultCharacter,
            CharacterSheet,
            LoadClassicSave,
            LoadDaggerfallUnitySave,
        }

        void Awake()
        {
            DontDestroyOnLoad(transform.gameObject);
        }

        void Update()
        {
            if (startMethod != lastMethod)
            {
                lastMethod = startMethod;
                InvokeStartMethod();
            }
        }

        void InvokeStartMethod()
        {
            switch(startMethod)
            {
                case StartMethods.CharacterSheet:
                    break;
                default:
                    break;
            }
        }
    }
}
