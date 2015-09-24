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
        CreateCharAddBonusStats createCharAddBonusStatsWindow;
        CreateCharAddBonusSkills createCharAddBonusSkillsWindow;
        CreateCharReflexSelect createCharReflexSelectWindow;
        CreateCharSummary createCharSummaryWindow;

        WizardStages WizardStage
        {
            get { return wizardStage; }
        }

        public enum WizardStages
        {
            SelectRace,
            SelectGender,
            SelectClassMethod,      // Class questions not implemented, goes straight to SelectClassFromList
            SelectClassFromList,
            CustomClassBuilder,     // Custom class not implemented
            SelectBiographyMethod,  // Biography not implemented
            SelectName,
            SelectFace,
            AddBonusStats,
            AddBonusSkills,
            SelectReflexes,
            Summary,
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

        void SetAddBonusStatsWindow()
        {
            if (createCharAddBonusStatsWindow == null)
            {
                createCharAddBonusStatsWindow = new CreateCharAddBonusStats(uiManager);
                createCharAddBonusStatsWindow.OnClose += AddBonusStatsWindow_OnClose;
                createCharAddBonusStatsWindow.DFClass = characterSheet.career;
                createCharAddBonusStatsWindow.Reroll();
            }

            // Update class and reroll if player changed class selection
            if (createCharAddBonusStatsWindow.DFClass != characterSheet.career)
            {
                createCharAddBonusStatsWindow.DFClass = characterSheet.career;
                createCharAddBonusStatsWindow.Reroll();
            }

            wizardStage = WizardStages.AddBonusStats;
            uiManager.PushWindow(createCharAddBonusStatsWindow);
        }

        void SetAddBonusSkillsWindow()
        {
            if (createCharAddBonusSkillsWindow == null)
            {
                createCharAddBonusSkillsWindow = new CreateCharAddBonusSkills(uiManager);
                createCharAddBonusSkillsWindow.OnClose += AddBonusSkillsWindow_OnClose;
                createCharAddBonusSkillsWindow.DFClass = characterSheet.career;
            }

            // Update class if player changes class selection
            if (createCharAddBonusSkillsWindow.DFClass != characterSheet.career)
            {
                createCharAddBonusSkillsWindow.DFClass = characterSheet.career;
            }

            wizardStage = WizardStages.AddBonusSkills;
            uiManager.PushWindow(createCharAddBonusSkillsWindow);
        }

        void SetSelectReflexesWindow()
        {
            if (createCharReflexSelectWindow == null)
            {
                createCharReflexSelectWindow = new CreateCharReflexSelect(uiManager);
                createCharReflexSelectWindow.OnClose += ReflexSelectWindow_OnClose;
            }

            wizardStage = WizardStages.SelectReflexes;
            uiManager.PushWindow(createCharReflexSelectWindow);
        }

        void SetSummaryWindow()
        {
            if (createCharSummaryWindow == null)
            {
                createCharSummaryWindow = new CreateCharSummary(uiManager);
                createCharSummaryWindow.OnClose += SummaryWindow_OnClose;
            }

            createCharSummaryWindow.CharacterSheet = characterSheet;

            wizardStage = WizardStages.Summary;
            uiManager.PushWindow(createCharSummaryWindow);
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
                characterSheet.career = createCharClassSelectWindow.SelectedClass;
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
                characterSheet.name = createCharNameSelectWindow.CharacterName;
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
                SetAddBonusStatsWindow();
            }
            else
            {
                SetNameSelectWindow();
            }
        }

        void AddBonusStatsWindow_OnClose()
        {
            if (!createCharAddBonusStatsWindow.Cancelled)
            {
                characterSheet.startingStats = createCharAddBonusStatsWindow.StartingStats;
                characterSheet.workingStats = createCharAddBonusStatsWindow.WorkingStats;
                SetAddBonusSkillsWindow();
            }
            else
            {
                SetFaceSelectWindow();
            }
        }

        void AddBonusSkillsWindow_OnClose()
        {
            if (!createCharAddBonusSkillsWindow.Cancelled)
            {
                characterSheet.startingSkills = createCharAddBonusSkillsWindow.StartingSkills;
                characterSheet.workingSkills = createCharAddBonusSkillsWindow.WorkingSkills;
                SetSelectReflexesWindow();
            }
            else
            {
                SetAddBonusStatsWindow();
            }
        }

        void ReflexSelectWindow_OnClose()
        {
            if (!createCharReflexSelectWindow.Cancelled)
            {
                characterSheet.reflexes = createCharReflexSelectWindow.PlayerReflexes;
                SetSummaryWindow();
            }
            else
            {
                SetAddBonusSkillsWindow();
            }
        }

        void SummaryWindow_OnClose()
        {
            if (!createCharSummaryWindow.Cancelled)
            {
            }
            else
            {
                SetSelectReflexesWindow();
            }
        }

        #endregion
    }
}