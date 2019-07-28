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
using DaggerfallConnect;
using DaggerfallConnect.FallExe;
using DaggerfallWorkshop.Utility;
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
            properties.ItemMakerFlags = ItemMakerFlags.AlphaSortSecondaryList;
            properties.EnchantmentPayloadFlags = EnchantmentPayloadFlags.Enchanted;
        }

        public override EnchantmentSettings[] GetEnchantmentSettings()
        {
            List<EnchantmentSettings> enchantments = new List<EnchantmentSettings>();

            // TEMP: Showing all souls for testing purposes
            // TODO: Enumerate available soul traps in player inventory without duplicates

            for (int i = 0; i < classicParamCosts.Length; i++)
            {
                EnchantmentSettings enchantment = new EnchantmentSettings()
                {
                    Version = 1,
                    EffectKey = EffectKey,
                    ClassicType = EnchantmentTypes.SoulBound,
                    ClassicParam = (short)i,
                    PrimaryDisplayName = properties.GroupName,
                    SecondaryDisplayName = EnemyBasics.Enemies[i].Name,
                    EnchantCost = classicParamCosts[i],
                };

                enchantments.Add(enchantment);
            }

            return enchantments.ToArray();
        }

        public override ForcedEnchantmentSet? GetForcedEnchantments(EnchantmentParam? param = null)
        {
            // Must have a param set
            if (param == null)
                return null;

            // Find forced enchantments for monster ID
            foreach (ForcedEnchantmentSet set in MobileForcedEnchantmentSets)
            {
                if (param.Value.ClassicParam == (short)set.soulType)
                    return set;
            }

            return null;
        }

        public override PayloadCallbackResults? EnchantmentPayloadCallback(EnchantmentPayloadFlags context, EnchantmentParam? param = null, DaggerfallEntityBehaviour sourceEntity = null, DaggerfallEntityBehaviour targetEntity = null, DaggerfallUnityItem sourceItem = null, int sourceDamage = 0)
        {
            return base.EnchantmentPayloadCallback(context, param, sourceEntity, targetEntity, sourceItem);
        }

        #region Classic Support

        // Matches monster IDs 0-42
        static short[] classicParamCosts =
        {
            -0,      //Rat
            -10,     //Imp
            -20,     //Spriggan
            -0,      //GiantBat
            -0,      //GrizzlyBear
            -0,      //SabertoothTiger
            -0,      //Spider
            -10,     //Orc
            -30,     //Centaur
            -90,     //Werewolf
            -100,    //Nymph
            -0,      //Slaughterfish
            -10,     //OrcSergeant
            -30,     //Harpy
            -140,    //Wereboar
            -0,      //SkeletalWarrior
            -30,     //Giant
            -0,      //Zombie
            -300,    //Ghost
            -100,    //Mummy
            -0,      //GiantScorpion
            -30,     //OrcShaman
            -30,     //Gargoyle
            -300,    //Wraith
            -10,     //OrcWarlord
            -500,    //FrostDaedra
            -500,    //FireDaedra
            -100,    //Daedroth
            -700,    //Vampire
            -1500,   //DaedraSeducer
            -1000,   //VampireAncient
            -8000,   //DaedraLord
            -1000,   //Lich
            -2500,   //AncientLich
            -0,      //Dragonling (no soul, general spawn)
            -300,    //FireAtronach
            -300,    //IronAtronach
            -300,    //FleshAtronach
            -300,    //IceAtronach
            -0,      //Horse_Invalid
            -5000,   //Dragonling_Alternate (has soul, quest spawn only)
            -100,    //Dreugh
            -100,    //Lamia
        };

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
                    new ForcedEnchantment(PotentVs.EffectKey, (short)PotentVs.Params.Daedra),
                    new ForcedEnchantment(UserTakesDamage.EffectKey, (short)UserTakesDamage.Params.InHolyPlaces),
                    new ForcedEnchantment(ExtraWeight.EffectKey),
                }
            },

            // Daedra Seducer
            new ForcedEnchantmentSet
            {
                soulType = MobileTypes.DaedraSeducer,
                forcedEffects = new ForcedEnchantment[]
                {
                    new ForcedEnchantment(GoodRepWith.EffectKey, (short)GoodRepWith.Params.All),
                    new ForcedEnchantment(ItemDeteriorates.EffectKey, (short)ItemDeteriorates.Params.InSunlight),
                    new ForcedEnchantment(UserTakesDamage.EffectKey, (short)UserTakesDamage.Params.InHolyPlaces),
                    new ForcedEnchantment(HealthLeech.EffectKey, (short)HealthLeech.Params.UnlessUsedWeekly),
                    new ForcedEnchantment(BadReactionsFrom.EffectKey, (short)BadReactionsFrom.Params.Animals),
                }
            },

            // Daedroth
            new ForcedEnchantmentSet
            {
                soulType = MobileTypes.Daedroth,
                forcedEffects = new ForcedEnchantment[]
                {
                    new ForcedEnchantment(LowDamageVs.EffectKey, (short)LowDamageVs.Params.Daedra),
                    new ForcedEnchantment(BadReactionsFrom.EffectKey, (short)BadReactionsFrom.Params.Daedra),
                    new ForcedEnchantment(ItemDeteriorates.EffectKey, (short)ItemDeteriorates.Params.InHolyPlaces),
                }
            },

            // TODO: Earth Atronach

            // Fire Atronach
            new ForcedEnchantmentSet
            {
                soulType = MobileTypes.FireAtronach,
                forcedEffects = new ForcedEnchantment[]
                {
                    new ForcedEnchantment(CastWhenUsed.EffectKey, 12), // 12=Resist Fire
                }
            },

            // Fire Daedra
            new ForcedEnchantmentSet
            {
                soulType = MobileTypes.FireDaedra,
                forcedEffects = new ForcedEnchantment[]
                {
                    new ForcedEnchantment(EnhancesSkill.EffectKey, (short)DFCareer.Skills.Daedric),
                    new ForcedEnchantment(CastWhenUsed.EffectKey, 12), // 12=Resist Fire
                    new ForcedEnchantment(BadReactionsFrom.EffectKey, (short)BadReactionsFrom.Params.Animals),
                }
            },

            // Frost Daedra
            new ForcedEnchantmentSet
            {
                soulType = MobileTypes.FrostDaedra,
                forcedEffects = new ForcedEnchantment[]
                {
                    new ForcedEnchantment(EnhancesSkill.EffectKey, (short)DFCareer.Skills.Daedric),
                    new ForcedEnchantment(CastWhenUsed.EffectKey, 11), // 11=Resist Cold
                    new ForcedEnchantment(ItemDeteriorates.EffectKey, (short)ItemDeteriorates.Params.InHolyPlaces),
                }
            },

            // Ghost
            new ForcedEnchantmentSet
            {
                soulType = MobileTypes.Ghost,
                forcedEffects = new ForcedEnchantment[]
                {
                    new ForcedEnchantment(FeatherWeight.EffectKey),
                    new ForcedEnchantment(ItemDeteriorates.EffectKey, (short)ItemDeteriorates.Params.InHolyPlaces),
                    new ForcedEnchantment(LowDamageVs.EffectKey, (short)LowDamageVs.Params.Undead),
                }
            },

            // Lich
            new ForcedEnchantmentSet
            {
                soulType = MobileTypes.Lich,
                forcedEffects = new ForcedEnchantment[]
                {
                    new ForcedEnchantment(EnhancesSkill.EffectKey, (short)DFCareer.Skills.Destruction),
                    new ForcedEnchantment(ItemDeteriorates.EffectKey, (short)ItemDeteriorates.Params.InSunlight),
                    new ForcedEnchantment(LowDamageVs.EffectKey, (short)LowDamageVs.Params.Undead),
                }
            },

            // TODO: Water Atronach

            // Wraith
            new ForcedEnchantmentSet
            {
                soulType = MobileTypes.Wraith,
                forcedEffects = new ForcedEnchantment[]
                {
                    new ForcedEnchantment(RegensHealth.EffectKey, (short)RegensHealth.Params.InDarkness),
                    new ForcedEnchantment(ItemDeteriorates.EffectKey, (short)ItemDeteriorates.Params.InHolyPlaces),
                    new ForcedEnchantment(LowDamageVs.EffectKey, (short)LowDamageVs.Params.Undead),
                }
            },
        };

        #endregion
    }
}