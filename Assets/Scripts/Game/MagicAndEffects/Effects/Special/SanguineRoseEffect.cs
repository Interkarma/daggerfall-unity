// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2019 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Numidium
// Contributors:    
// 
// Notes:
//
using UnityEngine;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using System.Collections.Generic;
using DaggerfallWorkshop.Game.Items;
using DaggerfallConnect.FallExe;
using System;
using DaggerfallConnect;
using DaggerfallWorkshop.Game.Utility;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Used by the Sanguine Rose. Spawns a Daedroth ally.
    /// </summary>
    public class SanguineRoseEffect : BaseEntityEffect
    {
        public static readonly string EffectKey = "SanguineRoseEffect";

        public override void MagicRound()
        {
            base.MagicRound();
            if (GameManager.Instance.AreEnemiesNearby())
            {
                // Summon a Daedroth to fight for the player.
                GameObject gameObject = GameObjectHelper.CreateFoeSpawner(foeType: MobileTypes.Daedroth, spawnCount: 1, alliedToPlayer: true);
                FoeSpawner foeSpawner = gameObject.GetComponent<FoeSpawner>();
                ItemCollection items = GameManager.Instance.PlayerEntity.Items;
                DaggerfallUnityItem item = items.GetItem(ItemGroups.Artifacts, (int)ArtifactsSubTypes.Sanguine_Rose);
                for (int i = 0; i < items.Count; i++)
                {
                    item = items.GetItem(i);
                    if (IsRose(item))
                        break;
                }
                if (item != null)
                    item.currentCondition -= 100;
            }
            else
            {
                DaggerfallUI.Instance.PopupMessage(HardStrings.noMonstersNearby);
            }
        }

        private bool IsRose(DaggerfallUnityItem item)
        {
            if (item != null && item.Enchantments != null)
            {
                foreach (DaggerfallEnchantment enchantment in item.Enchantments)
                {
                    if (enchantment.type == EnchantmentTypes.SpecialArtifactEffect && enchantment.param == (int)ArtifactsSubTypes.Sanguine_Rose)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public override void SetProperties()
        {
            properties.Key = EffectKey;
        }
    }
}
