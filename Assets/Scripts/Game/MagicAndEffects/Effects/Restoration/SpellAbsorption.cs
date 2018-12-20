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
    /// Spell Absorption
    /// </summary>
    public class SpellAbsorption : IncumbentEffect
    {
        public static readonly string EffectKey = "SpellAbsorption";

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.ClassicKey = MakeClassicKey(20, 255);
            properties.GroupName = TextManager.Instance.GetText(textDatabase, "spellAbsorption");
            properties.SpellMakerDescription = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1568);
            properties.SpellBookDescription = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1268);
            properties.SupportDuration = true;
            properties.SupportChance = true;
            properties.ChanceFunction = ChanceFunction.Custom;
            properties.AllowedTargets = EntityEffectBroker.TargetFlags_All;
            properties.AllowedElements = EntityEffectBroker.ElementFlags_MagicOnly;
            properties.AllowedCraftingStations = MagicCraftingStations.SpellMaker;
            properties.MagicSkill = DFCareer.MagicSkills.Restoration;
            properties.DurationCosts = MakeEffectCosts(28, 140);
            properties.ChanceCosts = MakeEffectCosts(28, 140);
        }

        protected override bool IsLikeKind(IncumbentEffect other)
        {
            return (other.Key == Key) ? true : false;
        }

        protected override void AddState(IncumbentEffect incumbent)
        {
            incumbent.RoundsRemaining += RoundsRemaining;
        }
    }
}
