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
        DaggerfallClassPickerWindow dfClassSelectWindow;

        WizardStages WizardStage
        {
            get { return wizardStage; }
        }

        public enum WizardStages
        {
            SelectRace,
            SelectGender,
            SelectClassMethod,      // Not implemented, will go straight to class list
            SelectClassFromList,    // Custom class not implemented
            EndWizard,
        }

        public StartNewGameWizard(IUserInterfaceManager uiManager)
            : base(uiManager)
        {
        }

        protected override void Setup()
        {
            // Wizard starts with race selection
            //SetRaceSelectWindow();
            SetClassSelectWindow();
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

            wizardStage = WizardStages.SelectRace;
            dfRaceSelectWindow.Clear();
            uiManager.PushWindow(dfRaceSelectWindow);
        }

        void SetGenderSelectWindow()
        {
            if (dfGenderSelectWindow == null)
            {
                dfGenderSelectWindow = new DaggerfallGenderSelectWindow(uiManager, dfRaceSelectWindow);
                dfGenderSelectWindow.OnClose += GenderSelectWindow_OnClose;
            }

            wizardStage = WizardStages.SelectGender;
            uiManager.PushWindow(dfGenderSelectWindow);
        }

        void SetClassSelectWindow()
        {
            if (dfClassSelectWindow == null)
            {
                dfClassSelectWindow = new DaggerfallClassPickerWindow(uiManager, dfRaceSelectWindow);
                dfClassSelectWindow.OnClose += ClassSelectWindow_OnClose;
            }

            wizardStage = WizardStages.SelectClassFromList;
            uiManager.PushWindow(dfClassSelectWindow);
        }

        #endregion

        #region Window Message Handling

        void RaceSelectWindow_OnClose()
        {
            if (dfRaceSelectWindow.SelectedRace != null)
            {
                characterSheet.race = dfRaceSelectWindow.SelectedRace;
                SetGenderSelectWindow();
            }
        }

        void GenderSelectWindow_OnClose()
        {
            if (!dfGenderSelectWindow.Cancelled)
            {
                characterSheet.gender = dfGenderSelectWindow.SelectedGender;
                SetClassSelectWindow();
            }
            else
            {
                SetRaceSelectWindow();
            }
        }

        void ClassSelectWindow_OnClose()
        {
            if (!dfClassSelectWindow.Cancelled)
            {
            }
            else
            {
                SetRaceSelectWindow();
            }
        }

        #endregion
    }
}