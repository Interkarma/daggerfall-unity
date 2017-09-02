// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2017 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut

using System;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop.Game.Items
{
    /// <summary>
    /// SymbolsDataSource context sensitive methods for items in Daggerfall Unity.
    /// </summary>
    public partial class DaggerfallUnityItem : MacroDataSource
    {
        public override string Material()
        {   // %mat
            switch (itemGroup)
            {
                case ItemGroups.Armor:
                    return ((ArmorMaterialTypes)nativeMaterialValue).ToString();
                case ItemGroups.Weapons:
                    return ((WeaponMaterialTypes)nativeMaterialValue).ToString();
                default:
                    return base.Material();
            }
        }

        public override string Condition()
        {   // %qua
            if (currentCondition == maxCondition)
                return "New";
            else
                // TODO: map to condition strings.
                return currentCondition.ToString();
        }

        public override string Weight()
        {   // %kg
            return weightInKg.ToString();
        }

        public override string WeaponDamage()
        {   // %wdm
            int matMod = GetWeaponMaterialModifier();
            return String.Format("{0} - {1}", GetBaseDamageMin() + matMod, GetBaseDamageMax() + matMod);
        }

        public override string ItemName()
        {
            return shortName;
        }

        // Armour not fully implemented yet.
        //public override string Modification()
        //{   // %mod
        //    return shortName;
        //}
    }
}