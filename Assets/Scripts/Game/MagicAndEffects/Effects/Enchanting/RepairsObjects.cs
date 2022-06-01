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

using DaggerfallConnect.FallExe;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Items;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Repairs equipped items.
    /// </summary>
    public class RepairsObjects : BaseEntityEffect
    {
        public static readonly string EffectKey = EnchantmentTypes.RepairsObjects.ToString();

        const int conditionPerRounds = 4;
        const int conditionAmount = 1;
        const int enchantCost = 900;

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.ShowSpellIcon = false;
            properties.AllowedCraftingStations = MagicCraftingStations.ItemMaker;
            properties.ItemMakerFlags = ItemMakerFlags.AllowMultiplePrimaryInstances;
            properties.EnchantmentPayloadFlags = EnchantmentPayloadFlags.Held;
        }

        public override string GroupName => TextManager.Instance.GetLocalizedText(EffectKey);

        public override EnchantmentSettings[] GetEnchantmentSettings()
        {
            EnchantmentSettings[] enchantments = new EnchantmentSettings[1];
            enchantments[0] = new EnchantmentSettings()
            {
                Version = 1,
                EffectKey = EffectKey,
                ClassicType = EnchantmentTypes.RepairsObjects,
                ClassicParam = -1,
                PrimaryDisplayName = GroupName,
                EnchantCost = enchantCost,
            };

            return enchantments;
        }

        #region Payloads

        /// <summary>
        /// Testing classic yields the following results:
        ///  - Only equipped items will receive repairs, items just stored in inventory are not repaired.
        ///  - Repair will happen whether player is resting or just standing around while game time passes.
        ///  - Repair does not happen when player is fast travelling (possibly game balance reasons?).
        /// The following assumptions have been made from observation:
        ///  - Items are repaired 1 hit point every 4 minutes. Takes around 8-9 hours for a Dwarven Dagger to go from "battered" to "new" in classic and DFU with this timing.
        ///  - Uncertain if not repairing during travel is intended or not - it doesn't seem to make sense for a passive enchantment that works all other times.
        ///  - Not doing anything around this right now so that repair ticks work consistently with other passive enchantment effects.
        ///  - Assuming only a single item is repaired per tick. Priority is based on equip order enumeration.
        /// </summary>
        public override void MagicRound()
        {
            base.MagicRound();

            // Get peered entity gameobject
            DaggerfallEntityBehaviour entityBehaviour = GetPeeredEntityBehaviour(manager);
            if (!entityBehaviour)
                return;

            // Only works on player entity
            if (entityBehaviour.EntityType != EntityTypes.Player)
                return;

            // This special only triggers once every conditionPerRounds
            if (GameManager.Instance.EntityEffectBroker.MagicRoundsSinceStartup % conditionPerRounds != 0)
                return;

            // Improve condition of a single item not at max condition
            for (int i = 0; i < GameManager.Instance.PlayerEntity.Items.Count; i++)
            {
                DaggerfallUnityItem item = GameManager.Instance.PlayerEntity.Items.GetItem(i);
                if (item != null && item.currentCondition < item.maxCondition)
                {
                    // Do not repair magic items unless settings allow it
                    if (item.IsEnchanted && !DaggerfallUnity.Settings.AllowMagicRepairs)
                        continue;

                    // Improve condition of item and exit
                    item.currentCondition += conditionAmount;
                    //UnityEngine.Debug.LogFormat("Improved condition of item {0} by {1} points", item.LongName, conditionAmount);
                    return;
                }
            }
        }

        #endregion
    }
}