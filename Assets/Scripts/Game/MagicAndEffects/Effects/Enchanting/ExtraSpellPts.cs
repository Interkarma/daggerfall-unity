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
using DaggerfallConnect.FallExe;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Extra spell points.
    /// </summary>
    public class ExtraSpellPts : BaseEntityEffect
    {
        public static readonly string EffectKey = EnchantmentTypes.ExtraSpellPts.ToString();

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.GroupName = TextManager.Instance.GetText(textDatabase, EffectKey);
            properties.AllowedCraftingStations = MagicCraftingStations.ItemMaker;
            properties.ItemMakerFlags = ItemMakerFlags.AllowMultiplePrimaryInstances;
        }

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
                    PrimaryDisplayName = properties.GroupName,
                    SecondaryDisplayName = TextManager.Instance.GetText(textDatabase, classicTextKeys[i]),
                    EnchantCost = classicParamCosts[i],
                };

                enchantments.Add(enchantment);
            }

            return enchantments.ToArray();
        }

        #region Classic Support

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