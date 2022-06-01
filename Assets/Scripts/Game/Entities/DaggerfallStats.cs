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

using UnityEngine;
using System;
using DaggerfallConnect;
using DaggerfallWorkshop.Game.Formulas;

namespace DaggerfallWorkshop.Game.Entity
{
    /// <summary>
    /// Daggerfall stats collection for every entity.
    /// </summary>
    [Serializable]
    public partial class DaggerfallStats
    {
        #region Fields

        public const int Count = 8;
        const int defaultValue = 50;

        // Current permanent stat values
        [SerializeField] int Strength;
        [SerializeField] int Intelligence;
        [SerializeField] int Willpower;
        [SerializeField] int Agility;
        [SerializeField] int Endurance;
        [SerializeField] int Personality;
        [SerializeField] int Speed;
        [SerializeField] int Luck;

        // Mods are temporary changes to stat values from effects
        // Default is 0 - effects can raise/lower mod values during their lifecycle
        // This is designed so that effects are never operating on permanent stat values
        int[] mods = new int[Count];
        int[] maxMods = new int[Count];

        #endregion

        #region Properties

        public int LiveStrength { get { return GetLiveStatValue(DFCareer.Stats.Strength); } }
        public int LiveIntelligence { get { return GetLiveStatValue(DFCareer.Stats.Intelligence); } }
        public int LiveWillpower { get { return GetLiveStatValue(DFCareer.Stats.Willpower); } }
        public int LiveAgility { get { return GetLiveStatValue(DFCareer.Stats.Agility); } }
        public int LiveEndurance { get { return GetLiveStatValue(DFCareer.Stats.Endurance); } }
        public int LivePersonality { get { return GetLiveStatValue(DFCareer.Stats.Personality); } }
        public int LiveSpeed { get { return GetLiveStatValue(DFCareer.Stats.Speed); } }
        public int LiveLuck { get { return GetLiveStatValue(DFCareer.Stats.Luck); } }

        public int PermanentStrength { get { return GetPermanentStatValue(DFCareer.Stats.Strength); } }
        public int PermanentIntelligence { get { return GetPermanentStatValue(DFCareer.Stats.Intelligence); } }
        public int PermanentWillpower { get { return GetPermanentStatValue(DFCareer.Stats.Willpower); } }
        public int PermanentAgility { get { return GetPermanentStatValue(DFCareer.Stats.Agility); } }
        public int PermanentEndurance { get { return GetPermanentStatValue(DFCareer.Stats.Endurance); } }
        public int PermanentPersonality { get { return GetPermanentStatValue(DFCareer.Stats.Personality); } }
        public int PermanentSpeed { get { return GetPermanentStatValue(DFCareer.Stats.Speed); } }
        public int PermanentLuck { get { return GetPermanentStatValue(DFCareer.Stats.Luck); } }

        #endregion

        #region Constructors

        public DaggerfallStats()
        {
            SetDefaults();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Check if all permanent stat values are at max.
        /// </summary>
        /// <returns>True if all at max.</returns>
        public bool IsAllMax()
        {
            int max = FormulaHelper.MaxStatValue();
            return (
                PermanentStrength == max &&
                PermanentIntelligence == max &&
                PermanentWillpower == max &&
                PermanentAgility == max &&
                PermanentEndurance == max &&
                PermanentPersonality == max &&
                PermanentSpeed == max &&
                PermanentLuck == max);
        }

        /// <summary>
        /// Set default value to each stat.
        /// </summary>
        public void SetDefaults()
        {
            Strength = defaultValue;
            Intelligence = defaultValue;
            Willpower = defaultValue;
            Agility = defaultValue;
            Endurance = defaultValue;
            Personality = defaultValue;
            Speed = defaultValue;
            Luck = defaultValue;
            Array.Clear(mods, 0, Count);
            Array.Clear(maxMods, 0, Count);
        }

        /// <summary>
        /// Copy contents of another DaggerfallStats into this one.
        /// Does not copy active effect mods.
        /// </summary>
        /// <param name="other">Stats collection to copy from.</param>
        public void Copy(DaggerfallStats other)
        {
            Strength = other.Strength;
            Intelligence = other.Intelligence;
            Willpower = other.Willpower;
            Agility = other.Agility;
            Endurance = other.Endurance;
            Personality = other.Personality;
            Speed = other.Speed;
            Luck = other.Luck;
        }

        /// <summary>
        /// Create a new copy of this stat collection.
        /// Does not copy active effect mods.
        /// </summary>
        /// <returns>New DaggerfallStats which is a copy of this DaggerfallStats.</returns>
        public DaggerfallStats Clone()
        {
            DaggerfallStats newStats = new DaggerfallStats();
            newStats.Copy(this);

            return newStats;
        }

        #endregion

        #region Getters

        /// <summary>
        /// Gets live stat value by enum, including effect mods.
        /// </summary>
        /// <param name="stat">Stat to get.</param>
        /// <returns>Stat value.</returns>
        public int GetLiveStatValue(DFCareer.Stats stat)
        {
            int value = GetPermanentStatValue(stat) + mods[(int)stat];
            int maxValue = FormulaHelper.MaxStatValue() + maxMods[(int)stat];

            // Clamp live stat to 0-maxValue (accounting for any max value mods)
            value = Mathf.Clamp(value, 0, maxValue);

            return (short)value;
        }

        /// <summary>
        /// Gets live stat value by index, including effect mods.
        /// </summary>
        /// <param name="index">Index of stat.</param>
        /// <returns>Stat value.</returns>
        public int GetLiveStatValue(int index)
        {
            if (index < 0 || index >= Count)
                return 0;

            return GetLiveStatValue((DFCareer.Stats)index);
        }

        /// <summary>
        /// Gets permanent stat value by index, does not include effect mods.
        /// </summary>
        /// <param name="index">Index of stat.</param>
        /// <returns>Stat value.</returns>
        public int GetPermanentStatValue(int index)
        {
            if (index < 0 || index >= Count)
                return 0;

            return GetPermanentStatValue((DFCareer.Stats)index);
        }

        /// <summary>
        /// Gets permanent stat value by enum, does not include effect mods.
        /// </summary>
        /// <param name="stat">Stat to get.</param>
        /// <returns>Stat value.</returns>
        public int GetPermanentStatValue(DFCareer.Stats stat)
        {
            switch (stat)
            {
                case DFCareer.Stats.Strength:
                    return Strength;
                case DFCareer.Stats.Intelligence:
                    return Intelligence;
                case DFCareer.Stats.Willpower:
                    return Willpower;
                case DFCareer.Stats.Agility:
                    return Agility;
                case DFCareer.Stats.Endurance:
                    return Endurance;
                case DFCareer.Stats.Personality:
                    return Personality;
                case DFCareer.Stats.Speed:
                    return Speed;
                case DFCareer.Stats.Luck:
                    return Luck;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Assign mods from effect manager.
        /// </summary>
        public void AssignMods(int[] statMods, int[] statMaxMods)
        {
            Array.Copy(statMods, mods, Count);
            Array.Copy(statMaxMods, maxMods, Count);
        }

        #endregion

        #region Setters

        /// <summary>
        /// Sets permanent stat value by enum, does not change effect mods.
        /// </summary>
        /// <param name="stat">Stat to set.</param>
        /// <param name="value">Stat value.</param>
        public void SetPermanentStatValue(DFCareer.Stats stat, int value)
        {
            switch (stat)
            {
                case DFCareer.Stats.Strength:
                    Strength = value;
                    break;
                case DFCareer.Stats.Intelligence:
                    Intelligence = value;
                    break;
                case DFCareer.Stats.Willpower:
                    Willpower = value;
                    break;
                case DFCareer.Stats.Agility:
                    Agility = value;
                    break;
                case DFCareer.Stats.Endurance:
                    Endurance = value;
                    break;
                case DFCareer.Stats.Personality:
                    Personality = value;
                    break;
                case DFCareer.Stats.Speed:
                    Speed = value;
                    break;
                case DFCareer.Stats.Luck:
                    Luck = value;
                    break;
            }
        }

        /// <summary>
        /// Sets permanent stat value by index, does not change effect mods.
        /// </summary>
        /// <param name="index">Index of stat.</param>
        /// <param name="value">Stat value.</param>
        public void SetPermanentStatValue(int index, int value)
        {
            SetPermanentStatValue((DFCareer.Stats)index, value);
        }

        /// <summary>
        /// Set permanent stat values from career, does not change effect mods.
        /// </summary>
        /// <param name="career">Career to set stats from.</param>
        public void SetPermanentFromCareer(DFCareer career)
        {
            if (career != null)
            {
                Strength = career.Strength;
                Intelligence = career.Intelligence;
                Willpower = career.Willpower;
                Agility = career.Agility;
                Endurance = career.Endurance;
                Personality = career.Personality;
                Speed = career.Speed;
                Luck = career.Luck;
            }
        }

        #endregion
    }
}
