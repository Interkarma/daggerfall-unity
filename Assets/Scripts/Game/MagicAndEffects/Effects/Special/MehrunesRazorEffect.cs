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

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Used by Mehrunes Razor. Kills target instantly if it does not save vs. magic.
    /// </summary>
    public class MehrunesRazorEffect : BaseEntityEffect
    {
        public static readonly string EffectKey = "MehrunesRazorEffect";

        public override void MagicRound()
        {
            base.MagicRound();
            DaggerfallEntityBehaviour entityBehaviour = GetPeeredEntityBehaviour(manager);
            if (!entityBehaviour)
                return;
            // Reduce weapon health by damage done to target.
            bool foundRazor = false;
            DaggerfallUnityItem item;
            item = GameManager.Instance.PlayerEntity.ItemEquipTable.GetItem(EquipSlots.RightHand);
            foundRazor = IsRazor(item);
            if (!foundRazor)
            {
                item = GameManager.Instance.PlayerEntity.ItemEquipTable.GetItem(EquipSlots.LeftHand);
                foundRazor = IsRazor(item);
            }

            if (foundRazor)
                item.currentCondition -= entityBehaviour.Entity.CurrentHealth;
            entityBehaviour.Entity.CurrentHealth = 0;
        }

        public override void SetProperties()
        {
            properties.Key = EffectKey;
        }

        private bool IsRazor(DaggerfallUnityItem item)
        {
            if (item != null && item.Enchantments != null)
            {
                foreach (DaggerfallEnchantment enchantment in item.Enchantments)
                {
                    if (enchantment.type == EnchantmentTypes.SpecialArtifactEffect && enchantment.param == (int)ArtifactsSubTypes.Mehrunes_Razor)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
