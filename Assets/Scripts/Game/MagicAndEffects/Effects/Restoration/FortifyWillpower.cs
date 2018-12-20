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

using DaggerfallConnect;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Fortify Attribute - Willpower
    /// </summary>
    public class FortifyWillpower : FortifyEffect
    {
        public static readonly string EffectKey = "Fortify-Willpower";

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.ClassicKey = MakeClassicKey(9, 2);
            properties.GroupName = TextManager.Instance.GetText("ClassicEffects", "fortifyAttribute");
            properties.SubGroupName = TextManager.Instance.GetText("ClassicEffects", "willpower");
            properties.SpellMakerDescription = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1534);
            properties.SpellBookDescription = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1234);
            properties.SupportDuration = true;
            properties.SupportMagnitude = true;
            properties.AllowedTargets = EntityEffectBroker.TargetFlags_All;
            properties.AllowedElements = EntityEffectBroker.ElementFlags_MagicOnly;
            properties.AllowedCraftingStations = MagicCraftingStations.SpellMaker;
            properties.MagicSkill = DFCareer.MagicSkills.Restoration;
            properties.DurationCosts = MakeEffectCosts(28, 100);
            properties.MagnitudeCosts = MakeEffectCosts(40, 120);
            fortifyStat = DFCareer.Stats.Willpower;
        }
    }
}
