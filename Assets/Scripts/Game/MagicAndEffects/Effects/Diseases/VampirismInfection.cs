// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
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
    /// Stage one disease effect for vampirism.
    /// Handles deployment tasks during three-day infection window.
    /// This disease can be cured in the usual way up until it completes.
    /// Note: This disease should only be assigned to player entity.
    ///
    /// TODO:
    ///  * Death video once days have elapsed
    ///  * Schedule vampire questline
    ///  * Clear guild memberships and reset reputations
    ///  * Teleport player to small crypt inside region
    ///  * Display "death is not the end" popup
    ///  * Shut down this disease effect (in fact cure all diseases and poisons) and start vampirism effect
    ///  * Pass infection region to vampirism effect for clan-specific work
    /// </summary>
    public class VampirismInfection : DiseaseEffect
    {
        public const string VampirismInfectionKey = "Vampirism-Infection";

        uint startingDay = 0;
        bool warningDreamVideoPlayed = false;
        int infectionRegionIndex;

        public override void SetProperties()
        {
            properties.Key = VampirismInfectionKey;
            properties.ShowSpellIcon = false;
            classicDiseaseType = Diseases.None;
            diseaseData = new DiseaseData(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0xFF, 0xFF); // Permanent no-effect disease, will manage custom lifecycle
        }

        public override void Start(EntityEffectManager manager, DaggerfallEntityBehaviour caster = null)
        {
            base.Start(manager, caster);

            // Record starting day of infection
            startingDay = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime() / DaggerfallDateTime.MinutesPerDay;

            // Record region of infection for clan at time of deployment
            // Think classic uses current region at time of turning, this will use current region at time of infection
            infectionRegionIndex = GameManager.Instance.PlayerGPS.CurrentRegionIndex;

            // Capture rest and travel events for disease progression on Start
            DaggerfallRestWindow.OnSleepTick += ProgressDiseaseAfterSleepOrTravel;
            DaggerfallTravelPopUp.OnPostFastTravel += ProgressDiseaseAfterSleepOrTravel;
        }

        public override void Resume(EntityEffectManager.EffectSaveData_v1 effectData, EntityEffectManager manager, DaggerfallEntityBehaviour caster = null)
        {
            base.Resume(effectData, manager, caster);

            // Capture rest and travel events for disease progression on Resume
            DaggerfallRestWindow.OnSleepTick += ProgressDiseaseAfterSleepOrTravel;
            DaggerfallTravelPopUp.OnPostFastTravel += ProgressDiseaseAfterSleepOrTravel;
        }

        #region Events

        void ProgressDiseaseAfterSleepOrTravel()
        {
            const string videoName = "ANIM0004.VID";    // Vampire dream video

            // Get current day and number of days that have passed (e.g. fast travel can progress time several days)
            uint currentDay = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime() / DaggerfallDateTime.MinutesPerDay;
            int daysPast = (int)(currentDay - startingDay);

            // Always allow dream to play on first rest or travel event
            // In current implementation, disease will not progress to stage 2 effect until player has experienced dream then rests or travels a second time
            if (daysPast > 0 && !warningDreamVideoPlayed)
            {
                DaggerfallVidPlayerWindow vidPlayerWindow = new DaggerfallVidPlayerWindow(DaggerfallUI.UIManager, videoName);
                DaggerfallUI.UIManager.PushWindow(vidPlayerWindow);
                warningDreamVideoPlayed = true;
            }
            else if (daysPast > 3 && warningDreamVideoPlayed)
            {
                // TODO: End stage one disease effect and deploy vampirism effect
            }
        }

        #endregion

        #region Serialization

        [fsObject("v1")]
        public struct CustomSaveData_v1
        {
            public bool warningDreamVideoPlayed;
            public uint startingDay;
            public int infectionRegionIndex;
        }

        protected override object GetCustomDiseaseSaveData()
        {
            CustomSaveData_v1 data = new CustomSaveData_v1();
            data.warningDreamVideoPlayed = warningDreamVideoPlayed;
            data.startingDay = startingDay;
            data.infectionRegionIndex = infectionRegionIndex;

            return data;
        }

        protected override void RestoreCustomDiseaseSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            CustomSaveData_v1 data = (CustomSaveData_v1)dataIn;
            warningDreamVideoPlayed = data.warningDreamVideoPlayed;
            startingDay = data.startingDay;
            infectionRegionIndex = data.infectionRegionIndex;
        }

        #endregion
    }
}