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

using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using FullSerializer;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Disease effect base.
    /// Does all the heavy lifting for time management, reducing stats, curing, serialization, etc.
    /// Child class can just use a custom DiseaseData matrix or override virtuals to do completely their own thing.
    /// </summary>
    public abstract class DiseaseEffect : IncumbentEffect
    {
        #region Fields

        const int permanentDiseaseValue = 0xff;
        const int completedDiseaseValue = 0xfe;

        protected int forcedRoundsRemaining = 1;
        protected Diseases classicDiseaseType = Diseases.None;
        protected DiseaseData diseaseData;
        protected TextFile.Token[] contractedMessageTokens;
        protected bool incubationOver = false;
        protected uint lastDay;
        protected int daysOfSymptomsLeft;

        #endregion

        #region Properties

        public Diseases ClassicDiseaseType
        {
            get { return classicDiseaseType; }
        }

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

        public TextFile.Token[] ContractedMessageTokens
        {
            get { return contractedMessageTokens; }
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

        public override void Start(EntityEffectManager manager, DaggerfallEntityBehaviour caster = null)
        {
            // Player must be greater than level 1 to acquire a disease
            DaggerfallEntityBehaviour host = GetPeeredEntityBehaviour(manager);
            if (host.EntityType == EntityTypes.Player && host.Entity.Level < 2)
            {
                forcedRoundsRemaining = 0;
                return;
            }

            // Store first day of infection - diseases operate in 24-hour ticks from the very next day after infection
            lastDay = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime() / DaggerfallDateTime.MinutesPerDay;

            // If disease not permanent then set a range for how long stats will fall
            // Otherwise stats will continue to fall until cured
            if (!IsDiseasePermanent())
                daysOfSymptomsLeft = (byte)UnityEngine.Random.Range(diseaseData.daysOfSymptomsMin, diseaseData.daysOfSymptomsMax + 1);

            base.Start(manager, caster);
        }

        protected override void AddState(IncumbentEffect incumbent)
        {
            // The player can catch multiple instances of the same disease in classic, but
            // in Daggerfall Unity host cannot catch same disease twice so do nothing further here.
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
        /// This runs once every magic tick so will implement own timer to run on a daily tick.
        /// A custom disease can override to perform their own logic.
        /// </summary>
        protected virtual void UpdateDisease()
        {
            // Do nothing if expiring out
            if (forcedRoundsRemaining == 0)
                return;

            // Get current day and number of days that have passed (e.g. fast travel can progress time several days)
            uint currentDay = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime() / DaggerfallDateTime.MinutesPerDay;
            int daysPast = (int)(currentDay - lastDay);

            // Do nothing if still same day or disease has run its course
            // if this is same day host contracted disease it is considered incubation time
            if (daysPast == 0 || daysOfSymptomsLeft == completedDiseaseValue)
                return;

            // Raise incubation over flag on first tick
            if (!incubationOver)
                incubationOver = true;

            // Increment effects for this day
            for (int i = 0; i < daysPast; i++)
            {
                IncrementDailyDiseaseEffects();
            }

            // Update day tracking
            lastDay = currentDay;

            // Count down days remaining
            if (!IsDiseasePermanent() && (daysOfSymptomsLeft -= daysPast) <= 0)
            {
                daysOfSymptomsLeft = 0;
                EndDisease();
            }

            // Output alert text
            DaggerfallUI.AddHUDText(UserInterfaceWindows.HardStrings.youFeelSomewhatBad, 2.5f);
        }

        /// <summary>
        /// Apply DiseaseData effects for each day passed - custom diseases can override this to do whatever they want for effect payload
        /// They don't even need to call this base to apply basic DiseaseData effects if they don't want to 
        /// </summary>
        protected virtual void IncrementDailyDiseaseEffects()
        {
            // Get amount of damage for today
            int damageAmount = UnityEngine.Random.Range(diseaseData.minDamage, diseaseData.maxDamage + 1);

            // This is a twist on DiseaseData - using it like a mult matrix by building on how it uses a byte value to indicate active components
            // This means custom (non-classic) DiseaseData can use 0 (no effect), 1 (normal effect), or 2-255 to accelerate stat decay even faster
            // This also simplifies the need to perform a conditional check for each element as a 0 will cancel any change for that component
            DaggerfallEntityBehaviour host = GetPeeredEntityBehaviour(manager);
            ChangeStatMod(DFCareer.Stats.Strength, -diseaseData.STR * damageAmount);
            ChangeStatMod(DFCareer.Stats.Intelligence, -diseaseData.INT * damageAmount);
            ChangeStatMod(DFCareer.Stats.Willpower, -diseaseData.WIL * damageAmount);
            ChangeStatMod(DFCareer.Stats.Agility, -diseaseData.AGI * damageAmount);
            ChangeStatMod(DFCareer.Stats.Endurance, -diseaseData.END * damageAmount);
            ChangeStatMod(DFCareer.Stats.Personality, -diseaseData.PER * damageAmount);
            ChangeStatMod(DFCareer.Stats.Speed, -diseaseData.SPD * damageAmount);
            ChangeStatMod(DFCareer.Stats.Luck, -diseaseData.LUC * damageAmount);
            host.Entity.DecreaseHealth(diseaseData.HEA * damageAmount);
            host.Entity.DecreaseFatigue(diseaseData.FAT * damageAmount);
            host.Entity.DecreaseMagicka(diseaseData.SPL * damageAmount);
        }

        protected virtual void EndDisease()
        {
            // Set disease as completed and allow effect system to expire effect
            daysOfSymptomsLeft = completedDiseaseValue;
            forcedRoundsRemaining = 0;
        }

        protected TextFile.Token[] GetClassicContractedMessageTokens(Diseases diseaseType)
        {
            // Only supports classic diseases, otherwise effect must provide own token stream
            if (diseaseType == Diseases.None)
                return null;
            else
                return DaggerfallUnity.Instance.TextProvider.GetRSCTokens(100 + (int)diseaseType);
        }

        protected DiseaseData GetClassicDiseaseData(Diseases diseaseType)
        {
            // Only supports classic diseases, otherwise effect must provide own disease data
            if (diseaseType == Diseases.None)
                return new DiseaseData();

            // Disease data. Found in FALL.EXE (1.07.213) from offset 0x1C0053.
            DiseaseData[] diseaseDataSources =
            {              //  STR  INT  WIL  AGI  END  PER  SPD  LUC  HEA  FAT  SPL MIND  MAXD  MINS  MAXS
                new DiseaseData( 1,   0,   0,   0,   1,   0,   0,   0,   1,   0,   0,   2,   10, 0xFF, 0xFF), // Witches' Pox
                new DiseaseData( 1,   0,   1,   1,   1,   1,   1,   1,   1,   1,   1,   3,   30, 0xFF, 0xFF), // Plague
                new DiseaseData( 0,   0,   1,   0,   1,   0,   0,   0,   1,   0,   0,   5,   10, 0xFF, 0xFF), // Yellow Fever
                new DiseaseData( 0,   0,   0,   0,   0,   0,   0,   0,   1,   0,   0,   1,    5, 0xFF, 0xFF), // Stomach Rot
                new DiseaseData( 1,   0,   1,   1,   0,   0,   0,   0,   0,   0,   0,   2,   10, 0xFF, 0xFF), // Consumption
                new DiseaseData( 0,   0,   1,   0,   0,   1,   0,   0,   1,   0,   0,   1,    5, 0xFF, 0xFF), // Brain Fever
                new DiseaseData( 1,   0,   1,   1,   0,   0,   0,   0,   0,   0,   0,   2,   10, 0xFF, 0xFF), // Swamp Rot
                new DiseaseData( 1,   0,   0,   1,   0,   0,   1,   0,   0,   0,   0,   5,   10,    3,   18), // Caliron's Curse
                new DiseaseData( 1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   5,   30, 0xFF, 0xFF), // Cholera
                new DiseaseData( 1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   0,   5,   30, 0xFF, 0xFF), // Leprosy
                new DiseaseData( 1,   0,   0,   0,   1,   0,   0,   0,   1,   0,   0,   2,    4, 0xFF, 0xFF), // Wound Rot
                new DiseaseData( 0,   0,   0,   0,   1,   1,   0,   0,   0,   1,   0,   2,   10, 0xFF, 0xFF), // Red Death
                new DiseaseData( 0,   0,   1,   0,   0,   1,   0,   0,   1,   0,   0,   5,   10,    3,   18), // Blood Rot
                new DiseaseData( 0,   1,   0,   0,   1,   0,   0,   0,   1,   0,   0,   2,   10, 0xFF, 0xFF), // Typhoid Fever
                new DiseaseData( 0,   1,   1,   0,   0,   1,   0,   0,   0,   0,   0,   2,   10, 0xFF, 0xFF), // Dementia
                new DiseaseData( 0,   1,   0,   0,   0,   0,   0,   0,   0,   0,   1,   5,   10, 0xFF, 0xFF), // Chrondiasis
                new DiseaseData( 0,   1,   0,   0,   0,   0,   0,   0,   0,   0,   1,   2,    4,    3,   18), // Wizard Fever
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
            EndDisease();
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
            public bool incubationOver;
            public uint lastDay;
            public int daysOfSymptomsLeft;
            public object customDiseaseData;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.forcedRoundsRemaining = forcedRoundsRemaining;
            data.incubationOver = incubationOver;
            data.lastDay = lastDay;
            data.daysOfSymptomsLeft = daysOfSymptomsLeft;
            data.customDiseaseData = GetCustomDiseaseSaveData();

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            forcedRoundsRemaining = data.forcedRoundsRemaining;
            incubationOver = data.incubationOver;
            lastDay = data.lastDay;
            daysOfSymptomsLeft = data.daysOfSymptomsLeft;
            RestoreCustomDiseaseSaveData(data.customDiseaseData);
        }

        protected virtual object GetCustomDiseaseSaveData()
        {
            return null;
        }

        protected virtual void RestoreCustomDiseaseSaveData(object dataIn)
        {
        }

        #endregion
    }
}
