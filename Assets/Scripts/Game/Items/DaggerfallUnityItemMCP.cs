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
            private string[] conditions = new string[] { "Broken", "Useless", "Battered", "Worn", "Used", "Slightly Used", "Almost New", "New" };
            private int[] conditionThresholds = new int[] {1, 5, 15, 40, 60, 75, 91, 101};

            private DaggerfallUnityItem parent;
            public ItemMacroDataSource(DaggerfallUnityItem item)
            {
                this.parent = item;
            }

            public override string ItemName()
            {
                return parent.LongName;
            }

            public override string Worth()
            {
                return parent.value.ToString();
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
                if (parent.maxCondition > 0 && parent.currentCondition <= parent.maxCondition)
                {
                    int conditionPercentage = 100 * parent.currentCondition / parent.maxCondition;
                    int i = 0;
                    while (conditionPercentage > conditionThresholds[i])
                        i++;
                    return conditions[i];
                }
                else
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

            // Armour mod is double what classic displays, but this is correct according to Allofich.
            public override string ArmourMod()
            {   // %mod
                return parent.GetMaterialArmorValue().ToString("+0;-0;0");
            }
        }
    }
}