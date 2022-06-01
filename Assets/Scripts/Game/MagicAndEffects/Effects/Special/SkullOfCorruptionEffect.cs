// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Numidium
// Contributors:    
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
    /// Used by the Skull of Corruption. Creates a clone of the nearest enemy as an ally to the player.
    /// </summary>
    class SkullOfCorruptionEffect : BaseEntityEffect
    {
        public static readonly string EffectKey = ArtifactsSubTypes.Skull_of_Corruption.ToString();

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

            // Must have nearby non-allied enemies
            List<PlayerGPS.NearbyObject> nearby = GameManager.Instance.PlayerGPS.GetNearbyObjects(PlayerGPS.NearbyObjectFlags.Enemy, enemyRange)
                .Where(x => ((EnemyEntity)x.gameObject.GetComponent<DaggerfallEntityBehaviour>().Entity).Team != MobileTeams.PlayerAlly).ToList();
            MobileTypes nearestType;
            if (nearby.Count == 0)
            {
                ShowSummonFailMessage();
                return null;
            }
            else
            {
                // Use nearest enemy for cloning
                PlayerGPS.NearbyObject nearest = nearby[0];
                foreach (PlayerGPS.NearbyObject nearbyObject in nearby.Skip(1))
                {
                    if (nearbyObject.distance < nearest.distance)
                    {
                        nearest = nearbyObject;
                    }
                }
                EnemyEntity enemy = (EnemyEntity)nearest.gameObject.GetComponent<DaggerfallEntityBehaviour>().Entity;
                if (enemy.Team == MobileTeams.PlayerAlly)
                {
                    ShowSummonFailMessage();
                    return null;
                }
                nearestType = (MobileTypes)enemy.MobileEnemy.ID;
            }

            // Spawn clone
            GameObjectHelper.CreateFoeSpawner(foeType: nearestType, spawnCount: 1, alliedToPlayer: true);

            // Durability loss for this effect
            return new PayloadCallbackResults()
            {
                durabilityLoss = 100,
            };
        }

        #endregion

        private void ShowSummonFailMessage()
        {
            DaggerfallUI.Instance.PopupMessage(TextManager.Instance.GetLocalizedText("noMonstersNearby"));
        }
    }
}
