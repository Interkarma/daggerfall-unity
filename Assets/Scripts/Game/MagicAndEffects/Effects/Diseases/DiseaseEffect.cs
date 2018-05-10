// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    Allofich
// 
// Notes:
//

using System;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallConnect.Arena2;
using FullSerializer;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Disease effect base.
    /// Does all the heavy lifting for incubation, reducing stats, serialization, etc. based on disease data provided.
    /// Designed to handle both classic diseases and to give new disease effect mods enough virtual hooks to override with custom behaviour.
    /// </summary>
    public abstract class DiseaseEffect : IncumbentEffect
    {
        #region Fields

        const string textDatabase = "ClassicEffects";
        const int permanentDiseaseValue = 0xff;
        const int incubationTimeInSeconds = 3 * DaggerfallDateTime.SecondsPerDay; // Using 3 days for incubation period - is this common to all diseases?

        protected int forcedRoundsRemaining = 1;
        protected Diseases classicDiseaseType = Diseases.None;
        protected DaggerfallDisease.DiseaseData diseaseData;
        protected TextFile.Token[] contractedMessageTokens;
        protected ulong startIncubationTime;
        protected bool incubationOver = false;
        protected int daysOfSymptomsLeft = 0;

        #endregion

        #region Properties

        public bool IncubationOver
        {
            get { return incubationOver; }
            set { incubationOver = value; }
        }

        public int DaysOfSymptomsLeft
        {
            get { return daysOfSymptomsLeft; }
            set { daysOfSymptomsLeft = value; }
        }

        #endregion

        #region Overrides

        // Disease effects manage their own lifecycle
        protected override int RemoveRound()
        {
            return forcedRoundsRemaining;
        }

        // Always present at least one round remaining so effect system does not remove
        // Set forcedRoundsRemaining to 0 to allow effect to expire once disease has run course or cured externally
        public override int RoundsRemaining
        {
            get { return forcedRoundsRemaining; }
        }

        public override void MagicRound()
        {
            base.MagicRound();
            UpdateDisease();
        }

        protected override void BecomeIncumbent()
        {
            // If disease not permanent then set a range for how long stats will fall
            // Otherwise stats will continue to fall until cured
            if (!IsDiseasePermanent())
                daysOfSymptomsLeft = (byte)UnityEngine.Random.Range(diseaseData.daysOfSymptomsMin, diseaseData.daysOfSymptomsMax + 1);

            // Start incubation from this moment
            startIncubationTime = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToSeconds();
        }

        protected override void AddState(IncumbentEffect incumbent)
        {
            // Player cannot catch same disease twice so do nothing further here
            // Specific diseases can override and do something else if they require
        }

        protected override bool IsLikeKind(IncumbentEffect other)
        {
            // Comparing keys should be enough for like-kind test
            // Child classes can override test if they need to
            return (other.Key == Key);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Executes default work on disease based on data.
        /// A custom disease can override to perform their own logic.
        /// </summary>
        protected virtual void UpdateDisease()
        {
            // Run incubation period
            if (!incubationOver)
            {
                if (startIncubationTime + incubationTimeInSeconds > DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToSeconds())
                    incubationOver = true;
                else
                    return;
            }

            // TODO: Track disease progression and update mod payload
        }

        protected TextFile.Token[] GetClassicContractedMessageTokens(Diseases diseaseType)
        {
            // Only supports classic diseases, otherwise effect must provide own token stream
            if (diseaseType == Diseases.None)
                return null;
            else
                return DaggerfallUnity.Instance.TextProvider.GetRSCTokens(100 + (int)diseaseType);
        }

        protected DaggerfallDisease.DiseaseData GetClassicDiseaseData(Diseases diseaseType)
        {
            // Only supports classic diseases, otherwise effect must provide own disease data
            if (diseaseType == Diseases.None)
                return new DaggerfallDisease.DiseaseData();

            // Disease data. Found in FALL.EXE (1.07.213) from offset 0x1C0053.
            DaggerfallDisease.DiseaseData[] diseaseDataSources = new DaggerfallDisease.DiseaseData[]
            {                                //  STR  INT  WIL  AGI  END  PER  SPD  LUC  HEA  FAT  SPL MIND  MAXD  MINS  MAXS
                new DaggerfallDisease.DiseaseData( 1,   0,   0,   0,   1,   0,   0,   0,   1,   0,   0,   2,   10, 0xFF, 0xFF), // Witches' Pox
                new DaggerfallDisease.DiseaseData( 1,   0,   1,   1,   1,   1,   1,   1,   1,   1,   1,   3,   30, 0xFF, 0xFF), // Plague
                new DaggerfallDisease.DiseaseData( 0,   0,   1,   0,   1,   0,   0,   0,   1,   0,   0,   5,   10, 0xFF, 0xFF), // Yellow Fever
                new DaggerfallDisease.DiseaseData( 0,   0,   0,   0,   0,   0,   0,   0,   1,   0,   0,   1,    5, 0xFF, 0xFF), // Stomach Rot
                new DaggerfallDisease.DiseaseData( 1,   0,   1,   1,   0,   0,   0,   0,   0,   0,   0,   2,   10, 0xFF, 0xFF), // Consumption
                new DaggerfallDisease.DiseaseData( 0,   0,   1,   0,   0,   1,   0,   0,   1,   0,   0,   1,    5, 0xFF, 0xFF), // Brain Fever
                new DaggerfallDisease.DiseaseData( 1,   0,   1,   1,   0,   0,   0,   0,   0,   0,   0,   2,   10, 0xFF, 0xFF), // Swamp Rot
                new DaggerfallDisease.DiseaseData( 1,   0,   0,   1,   0,   0,   1,   0,   0,   0,   0,   5,   10,    3,   18), // Caliron's Curse
                new DaggerfallDisease.DiseaseData( 1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   5,   30, 0xFF, 0xFF), // Cholera
                new DaggerfallDisease.DiseaseData( 1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   0,   5,   30, 0xFF, 0xFF), // Leprosy
                new DaggerfallDisease.DiseaseData( 1,   0,   0,   0,   1,   0,   0,   0,   1,   0,   0,   2,    4, 0xFF, 0xFF), // Wound Rot
                new DaggerfallDisease.DiseaseData( 0,   0,   0,   0,   1,   1,   0,   0,   0,   1,   0,   2,   10, 0xFF, 0xFF), // Red Death
                new DaggerfallDisease.DiseaseData( 0,   0,   1,   0,   0,   1,   0,   0,   1,   0,   0,   5,   10,    3,   18), // Blood Rot
                new DaggerfallDisease.DiseaseData( 0,   1,   0,   0,   1,   0,   0,   0,   1,   0,   0,   2,   10, 0xFF, 0xFF), // Typhoid Fever
                new DaggerfallDisease.DiseaseData( 0,   1,   1,   0,   0,   1,   0,   0,   0,   0,   0,   2,   10, 0xFF, 0xFF), // Dementia
                new DaggerfallDisease.DiseaseData( 0,   1,   0,   0,   0,   0,   0,   0,   0,   0,   1,   5,   10, 0xFF, 0xFF), // Chrondiasis
                new DaggerfallDisease.DiseaseData( 0,   1,   0,   0,   0,   0,   0,   0,   0,   0,   1,   2,    4,    3,   18), // Wizard Fever
            };

            return diseaseDataSources[(int)diseaseType];
        }

        protected bool IsDiseasePermanent()
        {
            return (diseaseData.daysOfSymptomsMin == permanentDiseaseValue);
        }

        #endregion

        #region Public Methods

        public virtual void CureDisease()
        {
            // TODO: Handle curing disease - just allowing effect to expire out for now
            forcedRoundsRemaining = 0;
            daysOfSymptomsLeft = 0;
        }

        public static string GetClassicDiseaseEffectKey(Diseases diseaseType)
        {
            return string.Format("Disease-{0}", diseaseType.ToString());
        }

        #endregion

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public int forcedRoundsRemaining;
            public Diseases classicDiseaseType;
            public bool incubationOver;
            public int daysOfSymptomsLeft;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.forcedRoundsRemaining = forcedRoundsRemaining;
            data.classicDiseaseType = classicDiseaseType;
            data.incubationOver = incubationOver;
            data.daysOfSymptomsLeft = daysOfSymptomsLeft;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            SaveData_v1 data = (SaveData_v1)dataIn;
            if (dataIn == null)
                return;

            forcedRoundsRemaining = data.forcedRoundsRemaining;
            classicDiseaseType = data.classicDiseaseType;
            incubationOver = data.incubationOver;
            daysOfSymptomsLeft = data.daysOfSymptomsLeft;
        }

        #endregion
    }
}