// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2019 Daggerfall Workshop
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
    public class Plague : DiseaseEffect
    {
        public override void SetProperties()
        {
            Diseases diseaseType = Diseases.Plague;

            properties.Key = GetClassicDiseaseEffectKey(diseaseType);
            properties.ShowSpellIcon = false;
            classicDiseaseType = diseaseType;
            diseaseData = GetClassicDiseaseData(diseaseType);
            contractedMessageTokens = GetClassicContractedMessageTokens(diseaseType);
        }
    }
}
