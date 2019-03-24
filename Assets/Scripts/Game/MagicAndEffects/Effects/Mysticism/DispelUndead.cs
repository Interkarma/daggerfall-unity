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

using System.Collections.Generic;
using UnityEngine;
using DaggerfallConnect;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Dispel - Undead
    /// </summary>
    public class DispelUndead : BaseEntityEffect
    {
        public static readonly string EffectKey = "Dispel-Undead";

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.ClassicKey = MakeClassicKey(6, 1);
            properties.GroupName = TextManager.Instance.GetText("ClassicEffects", "dispel");
            properties.SubGroupName = TextManager.Instance.GetText("ClassicEffects", "undead");
            properties.SpellMakerDescription = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1517);
            properties.SpellBookDescription = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1217);
            properties.SupportChance = true;
            properties.ChanceFunction = ChanceFunction.Custom;
            properties.AllowedTargets = EntityEffectBroker.TargetFlags_Self;
            properties.AllowedElements = EntityEffectBroker.ElementFlags_MagicOnly;
            properties.AllowedCraftingStations = MagicCraftingStations.SpellMaker;
            properties.MagicSkill = DFCareer.MagicSkills.Mysticism;
            properties.ChanceCosts = MakeEffectCosts(80, 140);
        }

        public override void Start(EntityEffectManager manager, DaggerfallEntityBehaviour caster = null)
        {
            base.Start(manager, caster);

            // Get nearby undead
            List<PlayerGPS.NearbyObject> nearbyUndead = GameManager.Instance.PlayerGPS.GetNearbyObjects(PlayerGPS.NearbyObjectFlags.Undead);
            foreach (PlayerGPS.NearbyObject obj in nearbyUndead)
            {
                // Roll chance for dispel
                // Just like classic, dispel simply destroys serializable enemy object in scene - target is not killed and will drop no loot
                // This can break quests if used carelessly
                if (obj.gameObject && RollChance())
                    GameObject.Destroy(obj.gameObject);
            }
        }
    }
}
