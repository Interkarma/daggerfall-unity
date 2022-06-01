// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using FullSerializer;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Parent class for both variants of lycanthropy infection to manage infection lifecycle.
    /// </summary>
    public abstract class LycanthropyInfection : DiseaseEffect
    {
        uint startingDay = 0;
        bool warningDreamVideoScheduled = false;
        bool warningDreamVideoPlayed = false;
        bool deployedFullBlownLycanthropy = false;

        public override void SetProperties()
        {
            properties.ShowSpellIcon = false;
            classicDiseaseType = Diseases.None;
            diseaseData = new DiseaseData(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0xFF, 0xFF); // Permanent no-effect disease, will manage custom lifecycle
            bypassSavingThrows = true;
        }

        public LycanthropyTypes InfectionType { get; protected set; }

        protected override void BecomeIncumbent()
        {
            base.BecomeIncumbent();

            // If player already has a racial override in place (e.g. vampire/lycanthrope) then just cancel infection process
            // Also exit if lycanthropy type not set by this stage (should be set during SetProperties call)
            if (manager.GetRacialOverrideEffect() != null || InfectionType == LycanthropyTypes.None)
                EndDisease();

            // Cancel infection if player already infected with opposing lycanthropy disease type
            if (Key == WerewolfInfection.WerewolfInfectionKey)
            {
                if (manager.FindIncumbentEffect<WereboarInfection>() != null)
                    EndDisease();
            }
            else if (Key == WereboarInfection.WereboarInfectionKey)
            {
                if (manager.FindIncumbentEffect<WerewolfInfection>() != null)
                    EndDisease();
            }
        }

        protected override void AddState(IncumbentEffect incumbent)
        {
            // While there can only be a single disease incumbent per key, incoming effect can remain memory resident for a short time
            // This can present duplicate symptoms during time acceleration (e.g. fast travel) from instances waiting to expire
            // Explicitly terminate non-incumbent payload that it doesn't fire during time acceleration
            EndDisease();
        }

        public override void Start(EntityEffectManager manager, DaggerfallEntityBehaviour caster = null)
        {
            base.Start(manager, caster);

            // Record starting day of infection
            startingDay = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime() / DaggerfallDateTime.MinutesPerDay;
        }

        public override void Resume(EntityEffectManager.EffectSaveData_v1 effectData, EntityEffectManager manager, DaggerfallEntityBehaviour caster = null)
        {
            base.Resume(effectData, manager, caster);
        }

        protected override void UpdateDisease()
        {
            // Not calling base as this is a very custom disease that manages its own lifecycle
            ProgressDisease();
        }

        #region Abstract Methods

        protected abstract void DeployFullBlownLycanthropy();

        #endregion

        #region Private Methods

        void ProgressDisease()
        {
            const string dreamVideoName = "ANIM0002.VID";   // Lycanthropy dream video

            // Do nothing if not incumbent or effect ended
            if (!IsIncumbent || forcedRoundsRemaining == 0 || daysOfSymptomsLeft == completedDiseaseValue)
                return;

            // Get current day and number of days that have passed (e.g. fast travel can progress time several days)
            uint currentDay = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime() / DaggerfallDateTime.MinutesPerDay;
            int daysPast = (int)(currentDay - startingDay);

            // Show dream after 1 day has passed, progress to full-blown lycanthropy after 3 days have passed
            if (daysPast > 0 && !warningDreamVideoScheduled && !warningDreamVideoPlayed)
            {
                // Play infection warning dream video
                DaggerfallVidPlayerWindow vidPlayerWindow = (DaggerfallVidPlayerWindow)
                    UIWindowFactory.GetInstanceWithArgs(UIWindowType.VidPlayer, new object[] { DaggerfallUI.UIManager, dreamVideoName });
                vidPlayerWindow.EndOnAnyKey = false;
                DaggerfallUI.UIManager.PushWindow(vidPlayerWindow);
                vidPlayerWindow.OnClose += WarningDreamVideoCompleted;
                warningDreamVideoScheduled = true;
            }
            else if (daysPast > 3 && warningDreamVideoPlayed && !deployedFullBlownLycanthropy)
            {
                // Assign Lycanthropy spell to spellbook
                GameManager.Instance.PlayerEntity.AssignPlayerLycanthropySpell();

                // Deploy full-blown lycanthropy
                DeployFullBlownLycanthropy();
                deployedFullBlownLycanthropy = true;

                // Terminate custom disease lifecycle
                EndDisease();
            }
        }

        private void WarningDreamVideoCompleted()
        {
            warningDreamVideoPlayed = true;
        }

        #endregion

        #region Serialization

        [fsObject("v1")]
        public struct CustomSaveData_v1
        {
            public LycanthropyTypes infectionType;
            public bool warningDreamVideoPlayed;
            public bool deployedFullBlownLycanthropy;
            public uint startingDay;
        }

        protected override object GetCustomDiseaseSaveData()
        {
            CustomSaveData_v1 data = new CustomSaveData_v1();
            data.infectionType = InfectionType;
            data.warningDreamVideoPlayed = warningDreamVideoPlayed;
            data.deployedFullBlownLycanthropy = deployedFullBlownLycanthropy;
            data.startingDay = startingDay;

            return data;
        }

        protected override void RestoreCustomDiseaseSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            CustomSaveData_v1 data = (CustomSaveData_v1)dataIn;
            InfectionType = data.infectionType;
            warningDreamVideoPlayed = data.warningDreamVideoPlayed;
            deployedFullBlownLycanthropy = data.deployedFullBlownLycanthropy;
            startingDay = data.startingDay;
        }

        #endregion
    }
}