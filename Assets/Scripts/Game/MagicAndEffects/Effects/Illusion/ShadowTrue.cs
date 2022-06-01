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

using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Shadow - True
    /// </summary>
    public class ShadowTrue : ConcealmentEffect
    {
        public static readonly string EffectKey = "Shadow-True";

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.ClassicKey = MakeClassicKey(24, 1);
            properties.SupportDuration = true;
            properties.AllowedTargets = EntityEffectBroker.TargetFlags_All;
            properties.AllowedElements = EntityEffectBroker.ElementFlags_MagicOnly;
            properties.AllowedCraftingStations = MagicCraftingStations.SpellMaker;
            properties.MagicSkill = DFCareer.MagicSkills.Illusion;
            properties.DurationCosts = MakeEffectCosts(40, 120);
            concealmentFlag = MagicalConcealmentFlags.ShadeTrue;
            startConcealmentMessageKey = "youAreAShade";
        }

        public override string GroupName => TextManager.Instance.GetLocalizedText("shadow");
        public override string SubGroupName => TextManager.Instance.GetLocalizedText("true");
        public override string DisplayName => string.Format("{0} ({1})", GroupName, SubGroupName);
        public override TextFile.Token[] SpellMakerDescription => DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1574);
        public override TextFile.Token[] SpellBookDescription => DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1274);

        protected override bool IsLikeKind(IncumbentEffect other)
        {
            return (other is ShadowTrue);
        }
    }
}
