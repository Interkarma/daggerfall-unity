// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2020 Daggerfall Workshop
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
    /// Morph Self (Lycanthropy)
    /// </summary>
    public class MorphSelf : BaseEntityEffect
    {
        public static readonly string EffectKey = "MorphSelf";

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.ClassicKey = MakeClassicKey(29, 255);
            properties.GroupName = TextManager.Instance.GetText("ClassicEffects", "morphSelf");
            properties.SubGroupName = string.Empty;
            properties.SpellMakerDescription = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1579);
            properties.SpellBookDescription = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1279);
            properties.AllowedTargets = EntityEffectBroker.TargetFlags_Self;
            properties.AllowedElements = EntityEffectBroker.ElementFlags_MagicOnly;
            properties.AllowedCraftingStations = MagicCraftingStations.None;
            properties.ShowSpellIcon = false;
            properties.MagicSkill = DFCareer.MagicSkills.Illusion;
        }

        public override void MagicRound()
        {
            base.MagicRound();

            // Find lycanthropy racial override and trigger morph
            // Cooldown and state of transformation is handled by racial override effect
            LycanthropyEffect lycanthropy = GameManager.Instance.PlayerEffectManager.GetRacialOverrideEffect() as LycanthropyEffect;
            if (lycanthropy != null)
                lycanthropy.MorphSelf();

            // Can end immediately
            RoundsRemaining = 0;
        }
    }
}
