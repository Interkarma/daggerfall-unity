// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Lypyl (lypyl@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using System;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Game.Serialization;

namespace DaggerfallWorkshop.Game
{

    /// <summary>
    /// A simple state manager class, driven by events
    /// </summary>
    public class StateManager
    {

        public System.EventHandler OnStartNewGameHandler;
        private StateTypes currentState;
        private StateTypes lastState;
        private bool gameInProgress;

        public StateTypes CurrentState
        {
            get { return currentState; }
        }

        public StateTypes LastState
        {
            get { return lastState; }
        }

        public bool GameInProgress
        {
            get { return gameInProgress; }
        }


        public StateManager(StateTypes startState)
        {
            currentState = startState;
            DaggerfallUI.UIManager.OnWindowChange   += UIManager_OnWindowChangeHandler;
            StartGameBehaviour.OnStartMenu          += StartGameBehaviour_OnStartMenuHandler;
            StartGameBehaviour.OnStartGame          += StartGameBehaviour_OnStartGameHandler;
            SaveLoadManager.OnLoad                  += SaveLoadManager_OnLoadHandler;
            PlayerDeath.OnPlayerDeath               += PlayerDeath_OnPlayerDeathHandler;
        }

        public enum StateTypes
        {
            None,
            //Setup,
            Start,
            //Loading,
            Game,
            Death,
            UI,
            Paused,
        }

        /// <summary>
        /// Changes state to specified state
        /// </summary>
        /// <param name="nextState">desired state</param>
        /// <returns></returns>
        public bool ChangeState(StateTypes nextState)
        {
            if (nextState == CurrentState)
                return false;
            else
            {
                Debug.Log(string.Format("StateManager changing state...previous state: {0} new state: {1}", CurrentState, nextState));
                lastState = CurrentState;
                currentState = nextState;
                TriggerStateChange(nextState);

                if (CurrentState == StateTypes.Start)
                    gameInProgress = false;
                else if (CurrentState == StateTypes.Game)
                    gameInProgress = true;
                return true;
            }

        }

        public void StartGameBehaviour_OnStartMenuHandler(object sender, EventArgs e)
        {
            ChangeState(StateTypes.Start);
        }

        //triggered by StartGameBehaviourwhen a new game starts from , either a new char or loading a classic save
        public void StartGameBehaviour_OnStartGameHandler(object sender, EventArgs e)
        {
            if (OnStartNewGameHandler != null)
                OnStartNewGameHandler(this, null);

            ChangeState(StateTypes.Game);
        }

        //triggered by SaveLoadManager when a quicksave is loaded
        public void SaveLoadManager_OnLoadHandler(SaveData_v1 saveData)
        {
            if (OnStartNewGameHandler != null)
                OnStartNewGameHandler(this, null);

            ChangeState(StateTypes.Game);
        }

        // Triggered when a UI window is opened/closed - does nothing if a game is not in progress
        public void UIManager_OnWindowChangeHandler(object sender, EventArgs e)
        {
            if (!GameInProgress) //don't override non-game state when UI push
                return;
            else if (DaggerfallUI.UIManager.WindowCount > 0)
                ChangeState(StateTypes.UI);
            else if (!GameManager.IsGamePaused)
                ChangeState(LastState);
            else
            {
                ChangeState(StateTypes.None);
            }
        }

        // triggered when player death animation starts
        public void PlayerDeath_OnPlayerDeathHandler(object sender, EventArgs e)
        {
            ChangeState(StateTypes.Death);
        }


        public delegate void StateChange(StateTypes state);
        public event StateChange OnStateChange;

        public void TriggerStateChange(StateTypes state)
        {
            if (OnStateChange != null)
                OnStateChange(state);
        }

    }
}
