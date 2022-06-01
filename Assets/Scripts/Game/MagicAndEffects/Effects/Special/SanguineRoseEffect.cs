// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Numidium
// Contributors:    Gavin Clayton (interkarma@dfworkshop.net)
// 
// Notes:
//

using System.Collections.Generic;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.Entity;
using System.Linq;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Used by the Sanguine Rose. Spawns a Daedroth ally.
    /// </summary>
    public class SanguineRoseEffect : BaseEntityEffect
    {
        public static readonly string EffectKey = ArtifactsSubTypes.Sanguine_Rose.ToString();

        const float enemyRange = 12;

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.ShowSpellIcon = false;
            properties.EnchantmentPayloadFlags = EnchantmentPayloadFlags.Used;
        }

        #region Payloads

        public override PayloadCallbackResults? EnchantmentPayloadCallback(EnchantmentPayloadFlags context, EnchantmentParam? param = null, DaggerfallEntityBehaviour sourceEntity = null, DaggerfallEntityBehaviour targetEntity = null, DaggerfallUnityItem sourceItem = null, int sourceDamage = 0)
        {
            base.EnchantmentPayloadCallback(context, param, sourceEntity, targetEntity, sourceItem, sourceDamage);

            // Must be Used payload
            if (context != EnchantmentPayloadFlags.Used)
                return null;

            // Must have nearby enemies
            List<PlayerGPS.NearbyObject> nearby = GameManager.Instance.PlayerGPS.GetNearbyObjects(PlayerGPS.NearbyObjectFlags.Enemy, enemyRange)
                .Where(x => ((EnemyEntity)x.gameObject.GetComponent<DaggerfallEntityBehaviour>().Entity).Team != MobileTeams.PlayerAlly).ToList();
            if (nearby.Count == 0)
            {
                DaggerfallUI.Instance.PopupMessage(TextManager.Instance.GetLocalizedText("noMonstersNearby"));
                return null;
            }

            // Summon a Daedroth to fight for the player
            GameObjectHelper.CreateFoeSpawner(foeType: MobileTypes.Daedroth, spawnCount: 1, alliedToPlayer: true);

            // Durability loss for this effect
            return new PayloadCallbackResults()
            {
                durabilityLoss = 100,
            };
        }

        #endregion
    }
}
