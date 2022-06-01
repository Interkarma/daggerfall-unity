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
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Extra spell points.
    /// </summary>
    public class ExtraSpellPts : BaseEntityEffect
    {
        public static readonly string EffectKey = EnchantmentTypes.ExtraSpellPts.ToString();

        const float nearbyRadius = 18f;             // Reasonably matched to classic with testing

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
            for (int i = 0; i < classicParams.Length; i++)
            {
                short id = classicParams[i];

                EnchantmentSettings enchantment = new EnchantmentSettings()
                {
                    Version = 1,
                    EffectKey = EffectKey,
                    ClassicType = EnchantmentTypes.ExtraSpellPts,
                    ClassicParam = id,
                    PrimaryDisplayName = GroupName,
                    SecondaryDisplayName = TextManager.Instance.GetLocalizedText(classicTextKeys[i]),
                    EnchantCost = classicParamCosts[i],
                };

                enchantments.Add(enchantment);
            }

            return enchantments.ToArray();
        }

        public override void Resume(EntityEffectManager.EffectSaveData_v1 effectData, EntityEffectManager manager, DaggerfallEntityBehaviour caster = null)
        {
            base.Resume(effectData, manager, caster);

            ConstantEffect();
        }

        #region Payloads

        /// <summary>
        /// Adds +75 to maximum spell points when certain conditions are met.
        /// </summary>
        public override void ConstantEffect()
        {
            base.ConstantEffect();

            const int maxIncrease = 75;

            // Must have a param
            if (EnchantmentParam == null)
                return;

            // Get peered entity gameobject
            DaggerfallEntityBehaviour entityBehaviour = GetPeeredEntityBehaviour(manager);
            if (!entityBehaviour)
                return;

            // Seasonal params are 0-3
            bool apply = false;
            Params type = (Params)EnchantmentParam.Value.ClassicParam;
            if (EnchantmentParam.Value.ClassicParam < 4)
            {
                DaggerfallDateTime.Seasons currentSeason = DaggerfallUnity.Instance.WorldTime.Now.SeasonValue;
                if (type == Params.DuringWinter && currentSeason == DaggerfallDateTime.Seasons.Winter ||
                    type == Params.DuringSpring && currentSeason == DaggerfallDateTime.Seasons.Spring ||
                    type == Params.DuringSummer && currentSeason == DaggerfallDateTime.Seasons.Summer ||
                    type == Params.DuringFall && currentSeason == DaggerfallDateTime.Seasons.Fall)
                    apply = true;
            }

            // Moon params are 4-6
            if (EnchantmentParam.Value.ClassicParam >= 4 && EnchantmentParam.Value.ClassicParam <= 6)
            {
                if (type == Params.DuringFullMoon && IsFullMoon() ||
                    type == Params.DuringHalfMoon && IsHalfMoon() ||
                    type == Params.DuringNewMoon && IsNewMoon())
                    apply = true;
            }

            // Nearby params are 7-10)
            // Core tracks nearby objects at low frequencies and nearby lookup is only checking a managed list using Linq
            if (EnchantmentParam.Value.ClassicParam > 6)
            {
                if (type == Params.NearUndead && IsNearUndead() ||
                    type == Params.NearDaedra && IsNearDaedra() ||
                    type == Params.NearHumanoids && IsNearHumanoids() ||
                    type == Params.NearAnimals && IsNearAnimals())
                    apply = true;
            }

            // Apply extra spell points when conditions are met
            if (apply)
                entityBehaviour.Entity.ChangeMaxMagickaModifier(maxIncrease);
        }

        bool IsFullMoon()
        {
            LunarPhases massarPhase = DaggerfallUnity.Instance.WorldTime.Now.MassarLunarPhase;
            LunarPhases secundaPhase = DaggerfallUnity.Instance.WorldTime.Now.SecundaLunarPhase;
            return massarPhase == LunarPhases.Full || secundaPhase == LunarPhases.Full;
        }

        bool IsHalfMoon()
        {
            LunarPhases massarPhase = DaggerfallUnity.Instance.WorldTime.Now.MassarLunarPhase;
            LunarPhases secundaPhase = DaggerfallUnity.Instance.WorldTime.Now.SecundaLunarPhase;
            return massarPhase == LunarPhases.HalfWane || massarPhase == LunarPhases.HalfWax ||
                   secundaPhase == LunarPhases.HalfWane || secundaPhase == LunarPhases.HalfWax;

        }

        bool IsNewMoon()
        {
            LunarPhases massarPhase = DaggerfallUnity.Instance.WorldTime.Now.MassarLunarPhase;
            LunarPhases secundaPhase = DaggerfallUnity.Instance.WorldTime.Now.SecundaLunarPhase;
            return massarPhase == LunarPhases.New || secundaPhase == LunarPhases.New;
        }

        bool IsNearUndead()
        {
            List<PlayerGPS.NearbyObject> nearby = GameManager.Instance.PlayerGPS.GetNearbyObjects(PlayerGPS.NearbyObjectFlags.Undead, nearbyRadius);
            return nearby != null && nearby.Count > 0;
        }

        bool IsNearDaedra()
        {
            List<PlayerGPS.NearbyObject> nearby = GameManager.Instance.PlayerGPS.GetNearbyObjects(PlayerGPS.NearbyObjectFlags.Daedra, nearbyRadius);
            return nearby != null && nearby.Count > 0;
        }

        bool IsNearHumanoids()
        {
            List<PlayerGPS.NearbyObject> nearby = GameManager.Instance.PlayerGPS.GetNearbyObjects(PlayerGPS.NearbyObjectFlags.Humanoid, nearbyRadius);
            return nearby != null && nearby.Count > 0;
        }

        bool IsNearAnimals()
        {
            List<PlayerGPS.NearbyObject> nearby = GameManager.Instance.PlayerGPS.GetNearbyObjects(PlayerGPS.NearbyObjectFlags.Animal, nearbyRadius);
            return nearby != null && nearby.Count > 0;
        }

        #endregion

        #region Classic Support

        enum Params
        {
            DuringWinter = 0,
            DuringSpring = 1,
            DuringSummer = 2,
            DuringFall = 3,
            DuringFullMoon = 4,
            DuringHalfMoon = 5,
            DuringNewMoon = 6,
            NearUndead = 7,
            NearDaedra = 8,
            NearHumanoids = 9,
            NearAnimals = 10,
        }

        static short[] classicParams =
        {
            0,      //During Winter
            1,      //During Spring
            2,      //During Summer
            3,      //During Fall
            4,      //During Full Moon
            5,      //During Half Moon
            6,      //During New Moon
            7,      //Near Undead
            8,      //Near Daedra
            9,      //Near Humanoids
            10,     //Near Animals
        };

        static short[] classicParamCosts =
        {
            500,    //During Winter
            500,    //During Spring
            500,    //During Summer
            500,    //During Fall
            200,    //During Full Moon
            200,    //During Half Moon
            200,    //During New Moon
            700,    //Near Undead
            800,    //Near Daedra
            900,    //Near Humanoids
            1000,   //Near Animals
        };

        static string[] classicTextKeys =
        {
            "duringWinter",
            "duringSpring",
            "duringSummer",
            "duringFall",
            "duringFullMoon",
            "duringHalfMoon",
            "duringNewMoon",
            "nearUndead",
            "nearDaedra",
            "nearHumanoids",
            "nearAnimals",
        };

        #endregion
    }
}