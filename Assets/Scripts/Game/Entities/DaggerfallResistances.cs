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

using UnityEngine;
using System;
using DaggerfallConnect;

namespace DaggerfallWorkshop.Game.Entity
{
    /// <summary>
    /// Daggerfall resistances collection for every entity.
    /// </summary>
    [Serializable]
    public class DaggerfallResistances
    {
        #region Fields

        public const int Count = 7;

        // Current permanent resistance values
        [SerializeField] int Fire;
        [SerializeField] int Cold;
        [SerializeField] int Poison;
        [SerializeField] int Shock;
        [SerializeField] int Magic;
        [SerializeField] int Disease;
        [SerializeField] int Paralysis;

        // Mods are temporary changes to resistance values from effects
        // Default is 0 - effects can raise/lower mod values during their lifecycle
        // This is designed so that effects are never operating on permanent resistance values
        int[] mods = new int[Count];

        #endregion

        #region Properties

        public int LiveFire { get { return GetLiveResistanceValue(DFCareer.Resistances.Fire); } }
        public int LiveCold { get { return GetLiveResistanceValue(DFCareer.Resistances.Cold); } }
        public int LivePoison { get { return GetLiveResistanceValue(DFCareer.Resistances.Poison); } }
        public int LiveShock { get { return GetLiveResistanceValue(DFCareer.Resistances.Shock); } }
        public int LiveMagic { get { return GetLiveResistanceValue(DFCareer.Resistances.Magic); } }
        public int LiveDisease { get { return GetLiveResistanceValue(DFCareer.Resistances.Disease); } }
        public int LiveParalysis { get { return GetLiveResistanceValue(DFCareer.Resistances.Paralysis); } }

        public int PermanentFire { get { return GetPermanentResistanceValue(DFCareer.Resistances.Fire); } }
        public int PermanentCold { get { return GetPermanentResistanceValue(DFCareer.Resistances.Cold); } }
        public int PermanentPoison { get { return GetPermanentResistanceValue(DFCareer.Resistances.Poison); } }
        public int PermanentShock { get { return GetPermanentResistanceValue(DFCareer.Resistances.Shock); } }
        public int PermanentMagic { get { return GetPermanentResistanceValue(DFCareer.Resistances.Magic); } }
        public int PermanentDisease { get { return GetPermanentResistanceValue(DFCareer.Resistances.Disease); } }
        public int PermanentParalysis { get { return GetPermanentResistanceValue(DFCareer.Resistances.Paralysis); } }


        #endregion

        #region Constructors

        public DaggerfallResistances()
        {
            SetDefaults();
        }

        #endregion

        #region Public Methods

        public void SetDefaults()
        {
            Fire = 0;
            Cold = 0;
            Poison = 0;
            Shock = 0;
            Magic = 0;
            Disease = 0;
            Paralysis = 0;
            Array.Clear(mods, 0, Count);
        }

        public void Copy(DaggerfallResistances other)
        {
            Fire = other.Fire;
            Cold = other.Cold;
            Poison = other.Poison;
            Shock = other.Shock;
            Magic = other.Magic;
            Disease = other.Disease;
            Paralysis = other.Paralysis;
        }

        #endregion

        #region Getters

        public int GetLiveResistanceValue(DFCareer.Resistances resistance)
        {
            int mod = mods[(int)resistance];
            int value = GetPermanentResistanceValue(resistance) + mod;

            return value;
        }

        public int GetLiveResistanceValue(int index)
        {
            if (index < 0 || index >= Count)
                return 0;

            return GetLiveResistanceValue((DFCareer.Resistances)index);
        }

        public int GetPermanentResistanceValue(int index)
        {
            if (index < 0 || index >= Count)
                return 0;

            return GetPermanentResistanceValue((DFCareer.Resistances)index);
        }

        public int GetPermanentResistanceValue(DFCareer.Resistances resistance)
        {
            switch (resistance)
            {
                case DFCareer.Resistances.Fire:
                    return Fire;
                case DFCareer.Resistances.Cold:
                    return Cold;
                case DFCareer.Resistances.Poison:
                    return Poison;
                case DFCareer.Resistances.Shock:
                    return Shock;
                case DFCareer.Resistances.Magic:
                    return Magic;
                case DFCareer.Resistances.Disease:
                    return Disease;
                case DFCareer.Resistances.Paralysis:
                    return Paralysis;
                default:
                    return 0;
            }
        }

        #endregion

        #region Setters

        public void SetPermanentResistanceValue(DFCareer.Resistances resistance, int value)
        {
            switch (resistance)
            {
                case DFCareer.Resistances.Fire:
                    Fire = value;
                    break;
                case DFCareer.Resistances.Cold:
                    Cold = value;
                    break;
                case DFCareer.Resistances.Poison:
                    Poison = value;
                    break;
                case DFCareer.Resistances.Shock:
                    Shock = value;
                    break;
                case DFCareer.Resistances.Magic:
                    Magic = value;
                    break;
                case DFCareer.Resistances.Disease:
                    Disease = value;
                    break;
                case DFCareer.Resistances.Paralysis:
                    Paralysis = value;
                    break;
                default:
                    return;
            }
        }

        public void SetPermanentResistanceValue(int index, int value)
        {
            SetPermanentResistanceValue((DFCareer.Resistances)index, value);
        }

        #endregion
    }
}