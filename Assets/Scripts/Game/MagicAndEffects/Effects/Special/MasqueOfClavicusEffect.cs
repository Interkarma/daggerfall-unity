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

using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallConnect.Arena2;
using System;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Used by the Masque of Clavicus. Improves all reaction mods.
    /// </summary>
    public class MasqueOfClavicusEffect : BaseEntityEffect
    {
        public static readonly string EffectKey = ArtifactsSubTypes.Masque_of_Clavicus.ToString();

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.ShowSpellIcon = false;
            properties.EnchantmentPayloadFlags = EnchantmentPayloadFlags.Held;
        }

        #region Payloads

        public override void MagicRound()
        {
            PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;
            int adjustmentAmount = playerEntity.Stats.LivePersonality / 5;

            // Improve reactions with all social groups
            foreach (FactionFile.SocialGroups group in Enum.GetValues(typeof(FactionFile.SocialGroups)))
            {
                playerEntity.ChangeReactionMod(group, adjustmentAmount);
            }
        }
        #endregion
    }
}
