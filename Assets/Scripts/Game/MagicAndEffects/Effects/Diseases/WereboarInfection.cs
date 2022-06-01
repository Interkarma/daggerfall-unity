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

using DaggerfallConnect.Arena2;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Stage one disease effect for wereboar variant of lycanthropy.
    /// This disease can be cured in the usual way up until it completes.
    /// Note: This disease should only be assigned to player entity.
    /// </summary>
    public class WereboarInfection : LycanthropyInfection
    {
        public const string WereboarInfectionKey = "Wereboar-Infection";

        public override void SetProperties()
        {
            base.SetProperties();
            properties.Key = WereboarInfectionKey;
            InfectionType = LycanthropyTypes.Wereboar;
        }

        public override TextFile.Token[] ContractedMessageTokens => null;

        protected override void DeployFullBlownLycanthropy()
        {
            // Start permanent wereboar lycanthropy effect stage two
            EntityEffectBundle bundle = GameManager.Instance.PlayerEffectManager.CreateLycanthropyCurse();
            GameManager.Instance.PlayerEffectManager.AssignBundle(bundle, AssignBundleFlags.BypassSavingThrows);
            LycanthropyEffect lycanthropyEffect = (LycanthropyEffect)GameManager.Instance.PlayerEffectManager.FindIncumbentEffect<LycanthropyEffect>();
            if (lycanthropyEffect != null)
                lycanthropyEffect.InfectionType = LycanthropyTypes.Wereboar;
        }
    }
}