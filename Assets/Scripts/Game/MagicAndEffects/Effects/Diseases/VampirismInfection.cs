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

using System.Collections.Generic;
using FullSerializer;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Formulas;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Utility;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Stage one disease effect for vampirism.
    /// Handles deployment tasks over three-day infection window.
    /// This disease can be cured in the usual way up until it completes.
    /// Note: This disease should only be assigned to player entity.
    ///
    /// TODO:
    ///  * Clear guild memberships and reset reputations
    /// </summary>
    public class VampirismInfection : DiseaseEffect
    {
        public const string VampirismInfectionKey = "Vampirism-Infection";
        const string spellsFilename = "SPELLS.STD";

        uint startingDay = 0;
        bool warningDreamVideoPlayed = false;
        bool fakeDeathVideoPlayed = false;
        int infectionRegionIndex = -1;

        public override void SetProperties()
        {
            properties.Key = VampirismInfectionKey;
            properties.ShowSpellIcon = false;
            classicDiseaseType = Diseases.None;
            diseaseData = new DiseaseData(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0xFF, 0xFF); // Permanent no-effect disease, will manage custom lifecycle
        }

        public int InfectionRegionIndex
        {
            get { return infectionRegionIndex; }
        }

        public VampireClans InfectionVampireClan
        {
            get { return FormulaHelper.GetVampireClan((DaggerfallRegions)infectionRegionIndex); }
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
            if (IsIncumbent)
            {
                DaggerfallRestWindow.OnSleepTick += ProgressDiseaseAfterSleepOrTravel;
                DaggerfallTravelPopUp.OnPostFastTravel += ProgressDiseaseAfterSleepOrTravel;
            }
        }

        public override void Resume(EntityEffectManager.EffectSaveData_v1 effectData, EntityEffectManager manager, DaggerfallEntityBehaviour caster = null)
        {
            base.Resume(effectData, manager, caster);

            // Capture rest and travel events for disease progression on Resume
            if (IsIncumbent)
            {
                DaggerfallRestWindow.OnSleepTick += ProgressDiseaseAfterSleepOrTravel;
                DaggerfallTravelPopUp.OnPostFastTravel += ProgressDiseaseAfterSleepOrTravel;
            }
        }

        protected override void UpdateDisease()
        {
            // Not calling base as this is a very custom disease that manages its own lifecycle
        }

        public override void End()
        {
            base.End();
            DaggerfallRestWindow.OnSleepTick -= ProgressDiseaseAfterSleepOrTravel;
            DaggerfallTravelPopUp.OnPostFastTravel -= ProgressDiseaseAfterSleepOrTravel;
        }

        #region Private Methods

        void ProgressDiseaseAfterSleepOrTravel()
        {
            const string dreamVideoName = "ANIM0004.VID";   // Vampire dream video
            const string deathVideoName = "ANIM0012.VID";   // Death video  

            // Get current day and number of days that have passed (e.g. fast travel can progress time several days)
            uint currentDay = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime() / DaggerfallDateTime.MinutesPerDay;
            int daysPast = (int)(currentDay - startingDay);

            // Always allow dream to play on first rest or travel event
            // In current implementation, disease will not progress to stage 2 effect until player has experienced dream then rests or travels a second time
            if (daysPast > 0 && !warningDreamVideoPlayed)
            {
                // Play infection warning dream video
                DaggerfallVidPlayerWindow vidPlayerWindow = new DaggerfallVidPlayerWindow(DaggerfallUI.UIManager, dreamVideoName);
                DaggerfallUI.UIManager.PushWindow(vidPlayerWindow);
                warningDreamVideoPlayed = true;
            }
            else if (daysPast > 3 && warningDreamVideoPlayed && !fakeDeathVideoPlayed)
            {
                // Play "death" video ahead of final stage of infection
                DaggerfallVidPlayerWindow vidPlayerWindow = new DaggerfallVidPlayerWindow(DaggerfallUI.UIManager, deathVideoName);
                DaggerfallUI.UIManager.PushWindow(vidPlayerWindow);
                vidPlayerWindow.OnClose += DeployFullBlownVampirism;
                fakeDeathVideoPlayed = true;
            }
        }

        private void DeployFullBlownVampirism()
        {
            const int deathIsNotEternalTextID = 401;

            // Cancel rest window if sleeping
            if (DaggerfallUI.Instance.UserInterfaceManager.TopWindow is DaggerfallRestWindow)
                (DaggerfallUI.Instance.UserInterfaceManager.TopWindow as DaggerfallRestWindow).CloseWindow();

            // Halt random enemy spawns for next playerEntity update so player isn't bombarded by spawned enemies after transform time
            GameManager.Instance.PlayerEntity.PreventEnemySpawns = true;

            // Reset legal reputation for all regions and strip player of guild memberships
            ResetLegalRepAndGuildMembership();

            // Raise game time to an evening two weeks later
            float raiseTime = (2 * DaggerfallDateTime.SecondsPerWeek) + (DaggerfallDateTime.DuskHour + 1 - DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.Hour) * 3600;
            DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.RaiseTime(raiseTime);

            // Transfer player to a random cemetery
            // Always using a small cemetery, nothing spoils that first vampire moment like being lost the guts of a massive dungeon
            // Intentionally not spawning enemies, for this time the PLAYER is the monster lurking inside the crypt
            DFLocation location = GetRandomCemetery();
            DFPosition mapPixel = MapsFile.LongitudeLatitudeToMapPixel(location.MapTableData.Longitude, location.MapTableData.Latitude);
            DFPosition worldPos = MapsFile.MapPixelToWorldCoord(mapPixel.X, mapPixel.Y);
            GameManager.Instance.PlayerEnterExit.RespawnPlayer(
                worldPos.X,
                worldPos.Y,
                true,
                false);

            // Assign vampire spells to spellbook
            GameManager.Instance.PlayerEntity.AssignPlayerVampireSpells(InfectionVampireClan);

            // Fade in from black
            DaggerfallUI.Instance.FadeBehaviour.FadeHUDFromBlack(1.0f);

            // Start permanent vampirism effect stage two
            EntityEffectBundle bundle = GameManager.Instance.PlayerEffectManager.CreateVampirismCurse();
            GameManager.Instance.PlayerEffectManager.AssignBundle(bundle);

            // Display popup
            DaggerfallMessageBox mb = DaggerfallUI.MessageBox(deathIsNotEternalTextID);
            mb.Show();
        }

        DFLocation GetRandomCemetery()
        {
            // Get player region data
            int regionIndex = GameManager.Instance.PlayerGPS.CurrentRegionIndex;
            DFRegion regionData = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetRegion(regionIndex);

            // Collect all cemetery locations
            List<int> foundLocationIndices = new List<int>();
            for (int i = 0; i < regionData.LocationCount; i++)
            {
                if (((int)regionData.MapTable[i].DungeonType) == (int)DFRegion.DungeonTypes.Cemetery)
                    foundLocationIndices.Add(i);
            }

            // Select one at random
            int index = UnityEngine.Random.Range(0, foundLocationIndices.Count);
            DFLocation location = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetLocation(regionIndex, foundLocationIndices[index]);
            if (!location.Loaded)
                throw new System.Exception("VampirismInfection.GetRandomCemetery() could not find a cemetery in this region.");

            return location;
        }

        void ResetLegalRepAndGuildMembership()
        {
            // TODO: Reset legal reputation for all regions and remove player's guilds memberships, but keep backup of current ranks
            // UESP indicates that rank is restored after 28 days when player re-joins a guild as a vampire
            // https://en.uesp.net/wiki/Daggerfall:Vampirism#Reputations
        }

        #endregion

        #region Serialization

        [fsObject("v1")]
        public struct CustomSaveData_v1
        {
            public bool warningDreamVideoPlayed;
            public bool fakeDeathVideoPlayed;
            public uint startingDay;
            public int infectionRegionIndex;
        }

        protected override object GetCustomDiseaseSaveData()
        {
            CustomSaveData_v1 data = new CustomSaveData_v1();
            data.warningDreamVideoPlayed = warningDreamVideoPlayed;
            data.fakeDeathVideoPlayed = fakeDeathVideoPlayed;
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
            fakeDeathVideoPlayed = data.fakeDeathVideoPlayed;
            startingDay = data.startingDay;
            infectionRegionIndex = data.infectionRegionIndex;
        }

        #endregion
    }
}