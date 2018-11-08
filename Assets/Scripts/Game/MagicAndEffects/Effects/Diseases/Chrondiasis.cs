﻿// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    public class Chrondiasis : DiseaseEffect
    {
        public override void SetProperties()
        {
            Diseases diseaseType = Diseases.Chrondiasis;

            properties.Key = GetClassicDiseaseEffectKey(diseaseType);
            properties.ShowSpellIcon = false;
            classicDiseaseType = diseaseType;
            diseaseData = GetClassicDiseaseData(diseaseType);
            contractedMessageTokens = GetClassicContractedMessageTokens(diseaseType);
        }
    }
}
