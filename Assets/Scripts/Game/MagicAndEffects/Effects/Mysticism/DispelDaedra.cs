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
    /// Dispel - Daedra
    /// </summary>
    public class DispelDaedra : BaseEntityEffect
    {
        public static readonly string EffectKey = "Dispel-Daedra";

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.ClassicKey = MakeClassicKey(6, 2);
            properties.GroupName = TextManager.Instance.GetText("ClassicEffects", "dispel");
            properties.SubGroupName = TextManager.Instance.GetText("ClassicEffects", "daedra");
            properties.SpellMakerDescription = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1518);
            properties.SpellBookDescription = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1218);
            properties.SupportChance = true;
            properties.ChanceFunction = ChanceFunction.Custom;
            properties.AllowedTargets = EntityEffectBroker.TargetFlags_Self;
            properties.AllowedElements = EntityEffectBroker.ElementFlags_MagicOnly;
            properties.AllowedCraftingStations = MagicCraftingStations.SpellMaker;
            properties.MagicSkill = DFCareer.MagicSkills.Mysticism;
            properties.ChanceCosts = MakeEffectCosts(120, 180);
        }

        public override void Start(EntityEffectManager manager, DaggerfallEntityBehaviour caster = null)
        {
            base.Start(manager, caster);

            // Get nearby daedra
            List<PlayerGPS.NearbyObject> nearbyDaedra = GameManager.Instance.PlayerGPS.GetNearbyObjects(PlayerGPS.NearbyObjectFlags.Daedra);
            foreach (PlayerGPS.NearbyObject obj in nearbyDaedra)
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
