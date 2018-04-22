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
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Heal - Health
    /// </summary>
    public class HealHealth : BaseEntityEffect
    {
        public override string Key { get { return "Heal-Health"; } }
        public override string GroupName { get { return TextManager.Instance.GetText("ClassicEffects", "heal"); } }
        public override string SubGroupName { get { return TextManager.Instance.GetText("ClassicEffects", "health"); } }
        public override int ClassicGroup { get { return 10; } }
        public override int ClassicSubGroup { get { return 8; } }
        public override TextFile.Token[] SpellMakerDescription { get { return DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1548); } }
        public override TextFile.Token[] SpellBookDescription { get { return DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1248); } }
        public override bool SupportDuration { get { return false; } }
        public override bool SupportChance { get { return false; } }
        public override TargetTypes AllowedTargets { get { return EntityEffectBroker.TargetFlags_All; } }
        public override ElementTypes AllowedElements { get { return EntityEffectBroker.ElementFlags_MagicOnly; } }

        public override void MagicRound(EntityEffectManager manager, DaggerfallEntityBehaviour caster = null)
        {
            // Get peered entity gameobject
            DaggerfallEntityBehaviour entityBehaviour = GetPeeredEntityBehaviour(manager);
            if (!entityBehaviour)
                return;

            // Implement effect
            int magnitude = GetMagnitude(caster);
            entityBehaviour.Entity.SetHealth(entityBehaviour.Entity.CurrentHealth + magnitude);

            Debug.LogFormat("{0} incremented {1}'s health by {2} points", Key, entityBehaviour.EntityType.ToString(), magnitude);
        }
    }
}