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
using System.Collections;
using System.Collections.Generic;
using DaggerfallConnect;

namespace DaggerfallWorkshop.Game.Entity
{
    /// <summary>
    /// Daggerfall stats collection for every entity.
    /// </summary>
    [Serializable]
    public struct DaggerfallStats
    {
        public const int Count = 8;

        const int defaultValue = 50;

        public int Strength;
        public int Intelligence;
        public int Willpower;
        public int Agility;
        public int Endurance;
        public int Personality;
        public int Speed;
        public int Luck;

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
        }

        public void Copy(DaggerfallStats other)
        {
            this.Strength = other.Strength;
            this.Intelligence = other.Intelligence;
            this.Willpower = other.Willpower;
            this.Agility = other.Agility;
            this.Endurance = other.Endurance;
            this.Personality = other.Personality;
            this.Speed = other.Speed;
            this.Luck = other.Luck;
        }

        public int GetStatValue(DFCareer.Stats stat)
        {
            switch (stat)
            {
                case DFCareer.Stats.Strength:
                    return this.Strength;
                case DFCareer.Stats.Intelligence:
                    return this.Intelligence;
                case DFCareer.Stats.Willpower:
                    return this.Willpower;
                case DFCareer.Stats.Agility:
                    return this.Agility;
                case DFCareer.Stats.Endurance:
                    return this.Endurance;
                case DFCareer.Stats.Personality:
                    return this.Personality;
                case DFCareer.Stats.Speed:
                    return this.Speed;
                case DFCareer.Stats.Luck:
                    return this.Luck;
                default:
                    return 0;
            }
        }

        public void SetStatValue(DFCareer.Stats stat, int value)
        {
            switch (stat)
            {
                case DFCareer.Stats.Strength:
                    this.Strength = value;
                    break;
                case DFCareer.Stats.Intelligence:
                    this.Intelligence = value;
                    break;
                case DFCareer.Stats.Willpower:
                    this.Willpower = value;
                    break;
                case DFCareer.Stats.Agility:
                    this.Agility = value;
                    break;
                case DFCareer.Stats.Endurance:
                    this.Endurance = value;
                    break;
                case DFCareer.Stats.Personality:
                    this.Personality = value;
                    break;
                case DFCareer.Stats.Speed:
                    this.Speed = value;
                    break;
                case DFCareer.Stats.Luck:
                    this.Luck = value;
                    break;
            }
        }

        public int GetStatValue(int index)
        {
            return GetStatValue((DFCareer.Stats)index);
        }

        public void SetStatValue(int index, int value)
        {
            SetStatValue((DFCareer.Stats)index, value);
        }

        public void SetFromCareer(DFCareer career)
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
}