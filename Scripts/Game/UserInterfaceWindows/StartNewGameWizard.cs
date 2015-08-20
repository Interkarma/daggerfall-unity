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
using System.IO;
using System.Collections;
using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Player;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Strings together the various parts of character creation and starting a new game.
    /// </summary>
    public class StartNewGameWizard : DaggerfallBaseWindow
    {
        WizardStages wizardStage;
        CharacterSheet characterSheet = new CharacterSheet();

        DaggerfallRaceSelectWindow dfRaceSelectWindow;
        DaggerfallGenderSelectWindow dfGenderSelectWindow;

        enum WizardStages
        {
            RaceSelect,
            GenderSelect,
            EndWizard,
        }

        public StartNewGameWizard(IUserInterfaceManager uiManager)
            : base(uiManager)
        {
        }

        protected override void Setup()
        {
            // Wizard starts with race selection
            SetRaceSelectWindow();
        }

        public override void Update()
        {
            base.Update();

            // If user has backed out of all windows then drop back to start screen
            if (uiManager.TopWindow == this)
                CloseWindow();
        }

        public override void Draw()
        {
            base.Draw();
        }

        #region Window Management

        void SetRaceSelectWindow()
        {
            if (dfRaceSelectWindow == null)
            {
                dfRaceSelectWindow = new DaggerfallRaceSelectWindow(uiManager);
                dfRaceSelectWindow.OnClose += RaceSelectWindow_OnClose;
            }

            wizardStage = WizardStages.RaceSelect;
            dfRaceSelectWindow.Clear();
            uiManager.PushWindow(dfRaceSelectWindow);
        }

        void SetGenderSelectWindow()
        {
            if (dfGenderSelectWindow == null)
            {
                // Uses race select as a background to popup
                dfGenderSelectWindow = new DaggerfallGenderSelectWindow(uiManager, dfRaceSelectWindow);
                dfGenderSelectWindow.OnClose += GenderSelectWindow_OnClose;
            }

            wizardStage = WizardStages.GenderSelect;
            uiManager.PushWindow(dfGenderSelectWindow);
        }

        #endregion

        #region Window Message Handling

        void RaceSelectWindow_OnClose()
        {
            if (dfRaceSelectWindow.SelectedRace != null)
                SetGenderSelectWindow();
        }

        void GenderSelectWindow_OnClose()
        {
            if (dfGenderSelectWindow.Cancelled)
                SetRaceSelectWindow();
        }

        #endregion
    }
}