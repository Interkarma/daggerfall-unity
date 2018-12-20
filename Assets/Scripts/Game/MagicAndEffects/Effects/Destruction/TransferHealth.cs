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
using DaggerfallConnect;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Transfer - Health
    /// </summary>
    public class TransferHealth : BaseEntityEffect
    {
        public static readonly string EffectKey = "Transfer-Health";

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.ClassicKey = MakeClassicKey(11, 8);
            properties.GroupName = TextManager.Instance.GetText("ClassicEffects", "transfer");
            properties.SubGroupName = TextManager.Instance.GetText("ClassicEffects", "health");
            properties.SpellMakerDescription = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1558);
            properties.SpellBookDescription = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1258);
            properties.SupportMagnitude = true;
            properties.ShowSpellIcon = false;
            properties.AllowedTargets = EntityEffectBroker.TargetFlags_Other;
            properties.AllowedElements = EntityEffectBroker.ElementFlags_MagicOnly;
            properties.AllowedCraftingStations = MagicCraftingStations.SpellMaker;
            properties.MagicSkill = DFCareer.MagicSkills.Destruction;
            properties.MagnitudeCosts = MakeEffectCosts(60, 100, 40);
        }

        public override void MagicRound()
        {
            base.MagicRound();

            // Get peered entity gameobject
            DaggerfallEntityBehaviour entityBehaviour = GetPeeredEntityBehaviour(manager);
            if (!entityBehaviour || !caster)
                return;

            // Damage health on target and heal health of caster
            int magnitude = GetMagnitude(caster);
            entityBehaviour.DamageHealthFromSource(this, magnitude, false, Vector3.zero);
            caster.Entity.IncreaseHealth(magnitude);
            PlayerAggro();
        }
    }
}
