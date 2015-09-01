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

        CreateCharRaceSelect createCharRaceSelectWindow;
        CreateCharGenderSelect createCharGenderSelectWindow;
        CreateCharClassSelect createCharClassSelectWindow;
        CreateCharNameSelect createCharNameSelectWindow;
        CreateCharFaceSelect createCharFaceSelectWindow;
        CreateCharAddBonusPoints createCharAddBonusPointsWindow;

        WizardStages WizardStage
        {
            get { return wizardStage; }
        }

        public enum WizardStages
        {
            SelectRace,
            SelectGender,
            SelectClassMethod,      // Not implemented, will go to class list
            SelectClassFromList,    // Custom class not implemented
            SelectBiographyMethod,  // Not implemented, will go to name selection
            SelectName,
            SelectFace,
            AddBonusPoints,
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
            SetAddBonusPointsWindow();
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
            if (createCharRaceSelectWindow == null)
            {
                createCharRaceSelectWindow = new CreateCharRaceSelect(uiManager);
                createCharRaceSelectWindow.OnClose += RaceSelectWindow_OnClose;
            }

            createCharRaceSelectWindow.Reset();
            characterSheet.race = null;

            wizardStage = WizardStages.SelectRace;

            if (uiManager.TopWindow != createCharRaceSelectWindow)
                uiManager.PushWindow(createCharRaceSelectWindow);
        }

        void SetGenderSelectWindow()
        {
            if (createCharGenderSelectWindow == null)
            {
                createCharGenderSelectWindow = new CreateCharGenderSelect(uiManager, createCharRaceSelectWindow);
                createCharGenderSelectWindow.OnClose += GenderSelectWindow_OnClose;
            }

            wizardStage = WizardStages.SelectGender;
            uiManager.PushWindow(createCharGenderSelectWindow);
        }

        void SetClassSelectWindow()
        {
            if (createCharClassSelectWindow == null)
            {
                createCharClassSelectWindow = new CreateCharClassSelect(uiManager, createCharRaceSelectWindow);
                createCharClassSelectWindow.OnClose += ClassSelectWindow_OnClose;
            }

            wizardStage = WizardStages.SelectClassFromList;
            uiManager.PushWindow(createCharClassSelectWindow);
        }

        void SetNameSelectWindow()
        {
            if (createCharNameSelectWindow == null)
            {
                createCharNameSelectWindow = new CreateCharNameSelect(uiManager);
                createCharNameSelectWindow.OnClose += NameSelectWindow_OnClose;
            }

            wizardStage = WizardStages.SelectName;
            uiManager.PushWindow(createCharNameSelectWindow);
        }

        void SetFaceSelectWindow()
        {
            if (createCharFaceSelectWindow == null)
            {
                createCharFaceSelectWindow = new CreateCharFaceSelect(uiManager);
                createCharFaceSelectWindow.OnClose += FaceSelectWindow_OnClose;
            }

            createCharFaceSelectWindow.SetFaceTextures(characterSheet.race, characterSheet.gender);

            wizardStage = WizardStages.SelectFace;
            uiManager.PushWindow(createCharFaceSelectWindow);
        }

        void SetAddBonusPointsWindow()
        {
            if (createCharAddBonusPointsWindow == null)
            {
                createCharAddBonusPointsWindow = new CreateCharAddBonusPoints(uiManager);
                createCharAddBonusPointsWindow.OnClose += AddBonusPointsWindow_OnClose;
            }

            createCharAddBonusPointsWindow.DFClass = characterSheet.dfClass;
            createCharAddBonusPointsWindow.Reroll();

            wizardStage = WizardStages.AddBonusPoints;
            uiManager.PushWindow(createCharAddBonusPointsWindow);
        }

        #endregion

        #region Event Handlers

        void RaceSelectWindow_OnClose()
        {
            if (!createCharRaceSelectWindow.Cancelled)
            {
                characterSheet.race = createCharRaceSelectWindow.SelectedRace;
                SetGenderSelectWindow();
            }
            else
            {
                characterSheet.race = null;
            }
        }

        void GenderSelectWindow_OnClose()
        {
            if (!createCharGenderSelectWindow.Cancelled)
            {
                characterSheet.gender = createCharGenderSelectWindow.SelectedGender;
                SetClassSelectWindow();
            }
            else
            {
                SetRaceSelectWindow();
            }
        }

        void ClassSelectWindow_OnClose()
        {
            if (!createCharClassSelectWindow.Cancelled)
            {
                characterSheet.dfClass = createCharClassSelectWindow.SelectedClass;
                SetNameSelectWindow();
            }
            else
            {
                SetRaceSelectWindow();
            }
        }

        void NameSelectWindow_OnClose()
        {
            if (!createCharNameSelectWindow.Cancelled)
            {
                characterSheet.name = createCharNameSelectWindow.Name;
                SetFaceSelectWindow();
            }
            else
            {
                SetClassSelectWindow();
            }
        }

        void FaceSelectWindow_OnClose()
        {
            if (!createCharFaceSelectWindow.Cancelled)
            {
                characterSheet.faceIndex = createCharFaceSelectWindow.FaceIndex;
                SetAddBonusPointsWindow();
            }
            else
            {
                SetNameSelectWindow();
            }
        }

        void AddBonusPointsWindow_OnClose()
        {
            if (!createCharAddBonusPointsWindow.Cancelled)
            {
            }
            else
            {
                SetFaceSelectWindow();
            }
        }

        #endregion
    }
}