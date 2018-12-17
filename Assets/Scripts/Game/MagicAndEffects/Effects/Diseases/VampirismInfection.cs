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

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Stage one disease effect for vampirism.
    /// Handles deployment tasks during three-day infection window.
    /// This disease can be cured in the usual way up until it completes.
    /// </summary>
    public class VampirismInfection : DiseaseEffect
    {
        public const string VampirismInfectionKey = "Vampirism-Infection";

        public override void SetProperties()
        {
            properties.Key = VampirismInfectionKey;
            properties.ShowSpellIcon = false;
            classicDiseaseType = Diseases.None;
            diseaseData = new DiseaseData(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0xFF, 0xFF); // Permanent no-effect disease, will manage custom lifecycle
        }

        protected override void IncrementDailyDiseaseEffects()
        {
            base.IncrementDailyDiseaseEffects();

            Debug.LogFormat("{0} days of vampirism disease have elapsed", daysPast);
        }
    }
}