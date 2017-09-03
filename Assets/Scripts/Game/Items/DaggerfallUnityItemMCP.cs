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
    public partial class DaggerfallUnityItem : IMacroContextProvider
    {
        public MacroDataSource GetMacroDataSource()
        {
            return new ItemMacroDataSource(this);
        }

        /// <summary>
        /// MacroDataSource context sensitive methods for items in Daggerfall Unity.
        /// </summary>
        private class ItemMacroDataSource : MacroDataSource
        {
            private DaggerfallUnityItem parent;
            public ItemMacroDataSource(DaggerfallUnityItem item)
            {
                this.parent = item;
            }

            public override string Material()
            {   // %mat
                switch (parent.itemGroup)
                {
                    case ItemGroups.Armor:
                        return ((ArmorMaterialTypes)parent.nativeMaterialValue).ToString();
                    case ItemGroups.Weapons:
                        return ((WeaponMaterialTypes)parent.nativeMaterialValue).ToString();
                    default:
                        return base.Material();
                }
            }

            public override string Condition()
            {   // %qua
                if (parent.currentCondition == parent.maxCondition)
                    return "New";
                else
                    // TODO: map to condition strings.
                    return parent.currentCondition.ToString();
            }

            public override string Weight()
            {   // %kg
                return parent.weightInKg.ToString();
            }

            public override string WeaponDamage()
            {   // %wdm
                int matMod = parent.GetWeaponMaterialModifier();
                return String.Format("{0} - {1}", parent.GetBaseDamageMin() + matMod, parent.GetBaseDamageMax() + matMod);
            }

            public override string ItemName()
            {
                return parent.shortName;
            }

            // Armour not fully implemented yet.
            //public override string Modification()
            //{   // %mod
            //    return shortName;
            //}
        }
    }
}