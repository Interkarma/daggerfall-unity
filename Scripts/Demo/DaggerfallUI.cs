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
using System.Collections;
using System.Collections.Generic;
using DaggerfallWorkshop.Demo.UserInterface;
using DaggerfallWorkshop.Demo.UserInterfaceWindows;

namespace DaggerfallWorkshop.Demo
{
    /// <summary>
    /// Implements a basic Daggerfall user interface.
    /// UI windows are managed by a state machine stack.
    /// A simple messaging system allows windows to send events via the manager.
    /// </summary>
    public class DaggerfallUI : MonoBehaviour
    {
        UserInterfaceManager uiManager = new UserInterfaceManager();
        DaggerfallStartWindow dfStartWindow;
        DaggerfallLoadSavedGameWindow dfLoadGameWindow;

        void Awake()
        {
            dfStartWindow = new DaggerfallStartWindow(uiManager);
            dfLoadGameWindow = new DaggerfallLoadSavedGameWindow(uiManager);
            uiManager.SendMessage(DaggerfallUIMessages.dfuiInitGame);
        }

        void Update()
        {
            // Process messages in queue
            if (uiManager.MessageCount > 0)
                ProcessMessageQueue();

            // Update top window
            if (uiManager.TopWindow != null)
            {
                uiManager.TopWindow.Update();
            }
        }

        void OnGUI()
        {
            // Draw top window
            if (uiManager.TopWindow != null)
            {
                uiManager.TopWindow.Draw();
            }
        }

        public static void SendWindowMessage(string message)
        {
            DaggerfallUI dfui = GameObject.FindObjectOfType<DaggerfallUI>();
            if (dfui)
            {
                dfui.uiManager.SendMessage(message);
            }
        }

        #region Private Methods

        void ProcessMessageQueue()
        {
            // Handle support messages
            string message = uiManager.PeekMessage();
            switch (message)
            {
                case DaggerfallUIMessages.dfuiInitGame:
                    uiManager.PushWindow(dfStartWindow);
                    break;
                case DaggerfallUIMessages.dfuiOpenLoadSavedGameWindow:
                    uiManager.PushWindow(dfLoadGameWindow);
                    break;
                case DaggerfallUIMessages.dfuiExitGame:
                    // TODO: Exit game
                    break;
                case WindowMessages.wmCloseWindow:
                    uiManager.PopWindow();
                    break;
                default:
                    return;
            }

            // Message was handled, pop from stack
            uiManager.PopMessage();
        }

        #endregion
    }
}