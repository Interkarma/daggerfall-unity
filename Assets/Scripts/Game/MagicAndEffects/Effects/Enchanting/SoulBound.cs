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

using DaggerfallConnect;
using DaggerfallConnect.FallExe;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Items;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Binds a soul to item, increasing enchantment power.
    /// Can force other enchantments onto item.
    /// Bound soul is released when item breaks and will attack player.
    /// </summary>
    public class SoulBound : BaseEntityEffect
    {
        public static readonly string EffectKey = EnchantmentTypes.SoulBound.ToString();

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.GroupName = TextManager.Instance.GetText(textDatabase, EffectKey);
            properties.ShowSpellIcon = false;
            properties.AllowedCraftingStations = MagicCraftingStations.ItemMaker;
            properties.ItemMakerFlags = ItemMakerFlags.None;
            properties.EnchantmentPayloadFlags = EnchantmentPayloadFlags.Enchanted;
        }

        public override EnchantmentSettings[] GetEnchantmentSettings()
        {
            EnchantmentSettings[] enchantments = new EnchantmentSettings[1];
            enchantments[0] = new EnchantmentSettings()
            {
                Version = 1,
                EffectKey = EffectKey,
                ClassicType = EnchantmentTypes.SoulBound,
                ClassicParam = -1,
                PrimaryDisplayName = properties.GroupName,
                EnchantCost = -1,
            };

            return enchantments;
        }

        public override PayloadCallbackResults? EnchantmentPayloadCallback(EnchantmentPayloadFlags context, EnchantmentParam? param = null, DaggerfallEntityBehaviour sourceEntity = null, DaggerfallEntityBehaviour targetEntity = null, DaggerfallUnityItem sourceItem = null, int sourceDamage = 0)
        {
            return base.EnchantmentPayloadCallback(context, param, sourceEntity, targetEntity, sourceItem);
        }

        #region Classic Support

        // Defines a single forced enchantment effect with param
        struct ForcedEnchantment
        {
            public EnchantmentTypes type;
            public EnchantmentParam param;

            public ForcedEnchantment(EnchantmentTypes enchantment, short classicParam = -1)
            {
                type = enchantment;
                param = new EnchantmentParam() { ClassicParam = classicParam, CustomParam = string.Empty };
            }
        }

        // Contains a set of forced effects keyed to a valid mobile type
        struct ForcedEnchantmentSet
        {
            public MobileTypes soulType;
            public ForcedEnchantment[] forcedEffects;
        }

        // Forced effects for each mobile type
        // Notes:
        //  - Classic seems to crossover atronachs Flesh>Air, Iron>Earth, Frost>Water when slain
        //  - These monster types do not exist but their souls do appear to exist as perhaps a rare capture from each parent type
        //  - Multiple sources claim traps populated with these souls can be found for purchase and sometimes trapped from parent type
        //  - https://en.uesp.net/wiki/Daggerfall_talk:Bestiary#Air_and_Water_Atronachs
        //  - https://en.uesp.net/wiki/Daggerfall_talk:Enchantment_Power
        //  - At this time these souls are not found in DFU and not possible to trap from Flesh, Iron, or Frost atronachs
        //  - Possible to add these as rare souls later using a special range in MobileTypes with related support code
        //  - Will investigate this in future, for now only valid monster souls can be trapped or consumed by Soul Bound enchantment
        readonly static ForcedEnchantmentSet[] MobileForcedEnchantmentSets = new ForcedEnchantmentSet[]
        {
            // TODO: Air Atronach

            // Daedra Lord
            new ForcedEnchantmentSet
            {
                soulType = MobileTypes.DaedraLord,
                forcedEffects = new ForcedEnchantment[]
                {
                    new ForcedEnchantment(EnchantmentTypes.PotentVs, (short)PotentVs.Params.Daedra),
                    new ForcedEnchantment(EnchantmentTypes.UserTakesDamage, (short)UserTakesDamage.Params.InHolyPlaces),
                    new ForcedEnchantment(EnchantmentTypes.ExtraWeight),
                }
            },

            // Daedra Seducer
            new ForcedEnchantmentSet
            {
                soulType = MobileTypes.DaedraSeducer,
                forcedEffects = new ForcedEnchantment[]
                {
                    new ForcedEnchantment(EnchantmentTypes.GoodRepWith, (short)GoodRepWith.Params.All),
                    new ForcedEnchantment(EnchantmentTypes.ItemDeteriorates, (short)ItemDeteriorates.Params.InSunlight),
                    new ForcedEnchantment(EnchantmentTypes.UserTakesDamage, (short)UserTakesDamage.Params.InHolyPlaces),
                    new ForcedEnchantment(EnchantmentTypes.HealthLeech, (short)HealthLeech.Params.UnlessUsedWeekly),
                    new ForcedEnchantment(EnchantmentTypes.BadReactionsFrom, (short)BadReactionsFrom.Params.Animals),
                }
            },

            // Daedroth
            new ForcedEnchantmentSet
            {
                soulType = MobileTypes.Daedroth,
                forcedEffects = new ForcedEnchantment[]
                {
                    new ForcedEnchantment(EnchantmentTypes.LowDamageVs, (short)LowDamageVs.Params.Daedra),
                    new ForcedEnchantment(EnchantmentTypes.BadReactionsFrom, (short)BadReactionsFrom.Params.Daedra),
                    new ForcedEnchantment(EnchantmentTypes.ItemDeteriorates, (short)ItemDeteriorates.Params.InHolyPlaces),
                }
            },

            // TODO: Earth Atronach

            // Fire Atronach
            new ForcedEnchantmentSet
            {
                soulType = MobileTypes.FireAtronach,
                forcedEffects = new ForcedEnchantment[]
                {
                    new ForcedEnchantment(EnchantmentTypes.CastWhenUsed, 12), // 12=Resist Fire
                }
            },

            // Fire Daedra
            new ForcedEnchantmentSet
            {
                soulType = MobileTypes.FireDaedra,
                forcedEffects = new ForcedEnchantment[]
                {
                    new ForcedEnchantment(EnchantmentTypes.EnhancesSkill, (short)DFCareer.Skills.Daedric),
                    new ForcedEnchantment(EnchantmentTypes.CastWhenUsed, 12), // 12=Resist Fire
                    new ForcedEnchantment(EnchantmentTypes.BadReactionsFrom, (short)BadReactionsFrom.Params.Animals),
                }
            },

            // Frost Daedra
            new ForcedEnchantmentSet
            {
                soulType = MobileTypes.FrostDaedra,
                forcedEffects = new ForcedEnchantment[]
                {
                    new ForcedEnchantment(EnchantmentTypes.EnhancesSkill, (short)DFCareer.Skills.Daedric),
                    new ForcedEnchantment(EnchantmentTypes.CastWhenUsed, 11), // 11=Resist Cold
                    new ForcedEnchantment(EnchantmentTypes.ItemDeteriorates, (short)ItemDeteriorates.Params.InHolyPlaces),
                }
            },

            // Ghost
            new ForcedEnchantmentSet
            {
                soulType = MobileTypes.Ghost,
                forcedEffects = new ForcedEnchantment[]
                {
                    new ForcedEnchantment(EnchantmentTypes.FeatherWeight),
                    new ForcedEnchantment(EnchantmentTypes.ItemDeteriorates, (short)ItemDeteriorates.Params.InHolyPlaces),
                    new ForcedEnchantment(EnchantmentTypes.LowDamageVs, (short)LowDamageVs.Params.Undead),
                }
            },

            // Lich
            new ForcedEnchantmentSet
            {
                soulType = MobileTypes.Lich,
                forcedEffects = new ForcedEnchantment[]
                {
                    new ForcedEnchantment(EnchantmentTypes.EnhancesSkill, (short)DFCareer.Skills.Destruction),
                    new ForcedEnchantment(EnchantmentTypes.ItemDeteriorates, (short)ItemDeteriorates.Params.InSunlight),
                    new ForcedEnchantment(EnchantmentTypes.LowDamageVs, (short)LowDamageVs.Params.Undead),
                }
            },

            // TODO: Water Atronach

            // Wraith
            new ForcedEnchantmentSet
            {
                soulType = MobileTypes.Wraith,
                forcedEffects = new ForcedEnchantment[]
                {
                    new ForcedEnchantment(EnchantmentTypes.RegensHealth, (short)RegensHealth.Params.InDarkness),
                    new ForcedEnchantment(EnchantmentTypes.ItemDeteriorates, (short)ItemDeteriorates.Params.InHolyPlaces),
                    new ForcedEnchantment(EnchantmentTypes.LowDamageVs, (short)LowDamageVs.Params.Undead),
                }
            },
        };

        #endregion
    }
}