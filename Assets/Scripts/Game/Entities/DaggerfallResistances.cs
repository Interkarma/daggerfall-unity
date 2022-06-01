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

namespace DaggerfallWorkshop.Game.Entity
{
    /// <summary>
    /// Daggerfall resistances collection for every entity.
    /// </summary>
    [Serializable]
    public class DaggerfallResistances
    {
        #region Fields

        public const int Count = 5;

        // Current permanent resistance values
        [SerializeField] int Fire;
        [SerializeField] int Frost;
        [SerializeField] int DiseaseOrPoison;
        [SerializeField] int Shock;
        [SerializeField] int Magic;

        // Mods are temporary changes to resistance values from effects
        // Default is 0 - effects can raise/lower mod values during their lifecycle
        // This is designed so that effects are never operating on permanent resistance values
        int[] mods = new int[Count];

        #endregion

        #region Properties

        public int LiveFire { get { return GetLiveResistanceValue(DFCareer.Elements.Fire); } }
        public int LiveFrost { get { return GetLiveResistanceValue(DFCareer.Elements.Frost); } }
        public int LiveDiseaseOrPoison { get { return GetLiveResistanceValue(DFCareer.Elements.DiseaseOrPoison); } }
        public int LiveShock { get { return GetLiveResistanceValue(DFCareer.Elements.Shock); } }
        public int LiveMagic { get { return GetLiveResistanceValue(DFCareer.Elements.Magic); } }

        public int PermanentFire { get { return GetPermanentResistanceValue(DFCareer.Elements.Fire); } }
        public int PermanentFrost { get { return GetPermanentResistanceValue(DFCareer.Elements.Frost); } }
        public int PermanentDiseaseOrPoison { get { return GetPermanentResistanceValue(DFCareer.Elements.DiseaseOrPoison); } }
        public int PermanentShock { get { return GetPermanentResistanceValue(DFCareer.Elements.Shock); } }
        public int PermanentMagic { get { return GetPermanentResistanceValue(DFCareer.Elements.Magic); } }

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
            Frost = 0;
            DiseaseOrPoison = 0;
            Shock = 0;
            Magic = 0;
            Array.Clear(mods, 0, Count);
        }

        public void Copy(DaggerfallResistances other)
        {
            Fire = other.Fire;
            Frost = other.Frost;
            DiseaseOrPoison = other.DiseaseOrPoison;
            Shock = other.Shock;
            Magic = other.Magic;
        }

        #endregion

        #region Getters

        public int GetLiveResistanceValue(DFCareer.Elements resistance)
        {
            int mod = mods[(int)resistance];
            int value = GetPermanentResistanceValue(resistance) + mod;

            return value;
        }

        public int GetLiveResistanceValue(int index)
        {
            if (index < 0 || index >= Count)
                return 0;

            return GetLiveResistanceValue((DFCareer.Elements)index);
        }

        public int GetPermanentResistanceValue(int index)
        {
            if (index < 0 || index >= Count)
                return 0;

            return GetPermanentResistanceValue((DFCareer.Elements)index);
        }

        public int GetPermanentResistanceValue(DFCareer.Elements resistance)
        {
            switch (resistance)
            {
                case DFCareer.Elements.Fire:
                    return Fire;
                case DFCareer.Elements.Frost:
                    return Frost;
                case DFCareer.Elements.DiseaseOrPoison:
                    return DiseaseOrPoison;
                case DFCareer.Elements.Shock:
                    return Shock;
                case DFCareer.Elements.Magic:
                    return Magic;
                default:
                    return 0;
            }
        }

        #endregion

        #region Setters

        public void SetPermanentResistanceValue(DFCareer.Elements resistance, int value)
        {
            switch (resistance)
            {
                case DFCareer.Elements.Fire:
                    Fire = value;
                    break;
                case DFCareer.Elements.Frost:
                    Frost = value;
                    break;
                case DFCareer.Elements.DiseaseOrPoison:
                    DiseaseOrPoison = value;
                    break;
                case DFCareer.Elements.Shock:
                    Shock = value;
                    break;
                case DFCareer.Elements.Magic:
                    Magic = value;
                    break;
                default:
                    return;
            }
        }

        public void SetPermanentResistanceValue(int index, int value)
        {
            SetPermanentResistanceValue((DFCareer.Elements)index, value);
        }

        /// <summary>
        /// Assign mods from effect manager.
        /// </summary>
        public void AssignMods(int[] resistanceMods)
        {
            Array.Copy(resistanceMods, mods, Count);
        }

        #endregion
    }
}
