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

using System.Collections.Generic;
using DaggerfallConnect.FallExe;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Improves natural talents for hearing, athleticism, adrenaline rush.
    /// </summary>
    public class ImprovesTalents : BaseEntityEffect
    {
        public static readonly string EffectKey = EnchantmentTypes.ImprovesTalents.ToString();

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.ShowSpellIcon = false;
            properties.AllowedCraftingStations = MagicCraftingStations.ItemMaker;
            properties.ItemMakerFlags = ItemMakerFlags.AllowMultiplePrimaryInstances;
            properties.EnchantmentPayloadFlags = EnchantmentPayloadFlags.Held;
        }

        public override string GroupName => TextManager.Instance.GetLocalizedText(EffectKey);

        /// <summary>
        /// Outputs all variant settings for this enchantment.
        /// </summary>
        public override EnchantmentSettings[] GetEnchantmentSettings()
        {
            List<EnchantmentSettings> enchantments = new List<EnchantmentSettings>();

            // Enumerate classic params
            for (int i = 0; i < classicParamCosts.Length; i++)
            {
                EnchantmentSettings enchantment = new EnchantmentSettings()
                {
                    Version = 1,
                    EffectKey = EffectKey,
                    ClassicType = EnchantmentTypes.ImprovesTalents,
                    ClassicParam = (short)i,
                    PrimaryDisplayName = GroupName,
                    SecondaryDisplayName = TextManager.Instance.GetLocalizedText(classicTextKeys[i]),
                    EnchantCost = classicParamCosts[i],
                };

                enchantments.Add(enchantment);
            }

            return enchantments.ToArray();
        }

        #region Payloads

        public override void ConstantEffect()
        {
            base.ConstantEffect();

            // Get peered entity gameobject - can only operate on player entity
            DaggerfallEntityBehaviour entityBehaviour = GetPeeredEntityBehaviour(manager);
            if (!entityBehaviour || entityBehaviour.EntityType != EntityTypes.Player)
                return;

            // Apply talent flags
            PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;
            Params param = (Params)EnchantmentParam.Value.ClassicParam;
            switch(param)
            {
                case Params.Hearing:
                    playerEntity.ImprovedAcuteHearing = true;
                    break;
                case Params.Athleticism:
                    playerEntity.ImprovedAthleticism = true;
                    break;
                case Params.AdrenalineRush:
                    playerEntity.ImprovedAdrenalineRush = true;
                    break;
            }
        }

        #endregion

        #region Classic Support

        enum Params
        {
            Hearing,
            Athleticism,
            AdrenalineRush,
        }

        static short[] classicParamCosts =
        {
            500,    //Hearing
            600,    //Athleticism
            600,    //Adrenaline Rush
        };

        static string[] classicTextKeys =
        {
            "hearing",
            "athleticismLower",
            "adrenalineRushLower",
        };

        #endregion
    }
}