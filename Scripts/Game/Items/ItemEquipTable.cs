// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using System;
using System.Collections.Generic;
using UnityEngine;
using DaggerfallConnect.FallExe;
using DaggerfallConnect.Save;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game.Items
{
    /// <summary>
    /// Represents items equipped by an entity.
    /// Provides helpers to equip/dequip items.
    /// </summary>
    public class ItemEquipTable
    {
        #region Fields

        const int equipTableLength = 27;

        DaggerfallUnityItem[] equipTable = new DaggerfallUnityItem[equipTableLength];

        #endregion

        #region Properties

        /// <summary>
        /// Gets clone of current equip table.
        /// </summary>
        public DaggerfallUnityItem[] EquipTable
        {
            get { return (DaggerfallUnityItem[])equipTable.Clone(); }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets item equipped to slot.
        /// </summary>
        /// <param name="slot">Slot to check.</param>
        /// <returns>Item present in slot, otherwise null.</returns>
        public DaggerfallUnityItem GetItem(EquipSlots slot)
        {
            if (slot == EquipSlots.None)
                return null;

            return equipTable[(int)slot];
        }

        /// <summary>
        /// Attempt to equip item.
        /// </summary>
        /// <param name="item">Item to equip.</param>
        /// <param name="alwaysEquip">Always equip item, replacing another in the same slot if needed.</param>
        /// <returns>True when item equipped, otherwise false.</returns>
        public bool EquipItem(DaggerfallUnityItem item, bool alwaysEquip = true)
        {
            // Get slot for this item
            EquipSlots slot = GetEquipSlot(item);
            if (slot == EquipSlots.None)
                return false;

            // Unequip any previous item
            if (!IsSlotOpen(slot) && !alwaysEquip)
                return false;
            else
                UnequipItem(slot);

            // Equip item to slot
            item.EquipSlot = slot;
            equipTable[(int)slot] = item;

            Debug.Log("Equipped item: " + item.Name);

            return true;
        }

        /// <summary>
        /// Attempt to unequip item from slot.
        /// </summary>
        /// <param name="slot">Slot to unequip.</param>
        /// <returns>True if item unequipped, otherwise false.</returns>
        public bool UnequipItem(EquipSlots slot)
        {
            if (!IsSlotOpen(slot))
            {
                equipTable[(int)slot].EquipSlot = EquipSlots.None;
                equipTable[(int)slot] = null;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Get the equipment slot this items belongs in.
        /// Not currently 100% known how Daggerfall defines equipment slots.
        /// This method uses the best available knowledge at time of writing.
        /// When item supports multiple slots:
        ///  - If a slot is open, returns first free slot.
        ///  - If both slots populated, returns first slot by order.
        /// </summary>
        /// <param name="item">Item to find equipment slot.</param>
        /// <returns>EquipSlot.</returns>
        public EquipSlots GetEquipSlot(DaggerfallUnityItem item)
        {
            EquipSlots result = EquipSlots.None;

            // Interim use of classic data
            //ItemRecord.ItemRecordData itemRecord = item.ItemRecord.ParsedData;

            // Resolve based on equipment category
            switch (item.ItemGroup)
            {
                case ItemGroups.Gems:
                    result = GetGemSlot(item);
                    break;
                case ItemGroups.Jewellery:
                    result = GetJewellerySlot(item);
                    break;
                case ItemGroups.Armor:
                    result = GetArmorSlot(item);
                    break;
                case ItemGroups.Weapons:
                    result = GetWeaponSlot(item);
                    break;
                case ItemGroups.MensClothing:
                    result = GetMensClothingSlot(item);
                    break;
                case ItemGroups.WomensClothing:
                    result = GetWomensClothingSlot(item);
                    break;
            }

            return result;
        }

        /// <summary>
        /// Checks if a slot is open.
        /// </summary>
        public bool IsSlotOpen(EquipSlots slot)
        {
            if (slot == EquipSlots.None)
                return false;

            if (equipTable[(int)slot] == null)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Returns first open slot or just first slot if both populated.
        /// This follows Daggerfall's behaviour for multi-slot items.
        /// </summary>
        public EquipSlots GetFirstSlot(EquipSlots slot1, EquipSlots slot2)
        {
            if (IsSlotOpen(slot1))
                return slot1;
            else if (IsSlotOpen(slot2))
                return slot2;
            else
                return slot1;
        }

        /// <summary>
        /// Quick test for torso cover.
        /// </summary>
        public bool IsUpperClothed()
        {
            if (!IsSlotOpen(EquipSlots.ChestClothes) || !IsSlotOpen(EquipSlots.ChestArmor))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Quick test for legs cover.
        /// </summary>
        public bool IsLowerClothed()
        {
            if (!IsSlotOpen(EquipSlots.LegsClothes) || !IsSlotOpen(EquipSlots.LegsArmor))
                return true;
            else
                return false;
        }

        #endregion

        #region Private Methods

        EquipSlots GetGemSlot(DaggerfallUnityItem item)
        {
            return GetFirstSlot(EquipSlots.Crystal0, EquipSlots.Crystal1);
        }

        EquipSlots GetJewellerySlot(DaggerfallUnityItem item)
        {
            ItemTemplate template = item.ItemTemplate;
            switch((Jewellery)template.index)
            {
                case Jewellery.Amulet:
                case Jewellery.Torc:
                case Jewellery.Cloth_amulet:
                    return GetFirstSlot(EquipSlots.Amulet0, EquipSlots.Amulet1);
                case Jewellery.Bracer:
                    return GetFirstSlot(EquipSlots.Bracer0, EquipSlots.Bracer1);
                case Jewellery.Ring:
                    return GetFirstSlot(EquipSlots.Ring0, EquipSlots.Ring1);
                case Jewellery.Bracelet:
                    return GetFirstSlot(EquipSlots.Bracelet0, EquipSlots.Bracelet1);
                case Jewellery.Mark:
                    return GetFirstSlot(EquipSlots.Mark0, EquipSlots.Mark1);
                default:
                    return EquipSlots.None;
            }
        }

        EquipSlots GetArmorSlot(DaggerfallUnityItem item)
        {
            ItemTemplate template = item.ItemTemplate;
            switch((Armor)template.index)
            {
                case Armor.Cuirass:
                    return EquipSlots.ChestArmor;
                case Armor.Gauntlets:
                    return EquipSlots.Gloves;
                case Armor.Greaves:
                    return EquipSlots.LegsArmor;
                case Armor.Left_Pauldron:
                    return EquipSlots.LeftArm;
                case Armor.Right_Pauldron:
                    return EquipSlots.RightArm;
                case Armor.Helm:
                    return EquipSlots.Head;
                case Armor.Boots:
                    return EquipSlots.Feet;
                case Armor.Buckler:
                case Armor.Round_Shield:
                case Armor.Kite_Shield:
                case Armor.Tower_Shield:
                    return EquipSlots.LeftHand;
                default:
                    return EquipSlots.None;
            }
        }

        EquipSlots GetWeaponSlot(DaggerfallUnityItem item)
        {
            ItemHands hands = DaggerfallUnity.Instance.ItemHelper.GetItemHands(item);
            switch (hands)
            {
                case ItemHands.RightOnly:
                case ItemHands.Both:
                    return EquipSlots.RightHand;
                case ItemHands.LeftOnly:
                    return EquipSlots.LeftHand;
                case ItemHands.Either:
                    return GetFirstSlot(EquipSlots.RightHand, EquipSlots.LeftHand);
                default:
                    return EquipSlots.None;
            }
        }

        EquipSlots GetMensClothingSlot(DaggerfallUnityItem item)
        {
            ItemTemplate template = item.ItemTemplate;
            switch((MensClothing)template.index)
            {
                case MensClothing.Straps:
                case MensClothing.Armbands:
                case MensClothing.Kimono:
                case MensClothing.Fancy_Armbands:
                case MensClothing.Sash:
                case MensClothing.Eodoric:
                case MensClothing.Dwynnen_surcoat:
                case MensClothing.Short_tunic:
                case MensClothing.Formal_tunic:
                case MensClothing.Toga:
                case MensClothing.Reversible_tunic:
                case MensClothing.Plain_Robes:
                case MensClothing.Priest_Robes:
                case MensClothing.Short_Shirt:
                case MensClothing.Short_shirt_with_belt:
                case MensClothing.Long_shirt:
                case MensClothing.Long_shirt_with_belt:
                case MensClothing.Short_shirt_closed_top:
                case MensClothing.Short_shirt_closed_top2:
                case MensClothing.Long_shirt_closed_top:
                case MensClothing.Long_shirt_closed_top2:
                case MensClothing.Open_Tunic:
                case MensClothing.Anticlere_Surcoat:
                case MensClothing.Challenger_Straps:
                case MensClothing.Short_shirt_unchangeable:
                case MensClothing.Long_shirt_unchangeable:
                case MensClothing.Vest:
                case MensClothing.Champion_straps:
                    return EquipSlots.ChestClothes;

                case MensClothing.Shoes:
                case MensClothing.Tall_Boots:
                case MensClothing.Boots:
                case MensClothing.Sandals:
                    return EquipSlots.Feet;

                case MensClothing.Casual_Pants:
                case MensClothing.Breeches:
                case MensClothing.Short_skirt:
                case MensClothing.Khajiit_suit:
                case MensClothing.Loincloth:
                case MensClothing.Wrap:
                case MensClothing.Long_Skirt:
                    return EquipSlots.LegsClothes;

                case MensClothing.Casual_cloak:
                case MensClothing.Formal_cloak:
                    return GetFirstSlot(EquipSlots.Cloak2, EquipSlots.Cloak1);

                default:
                    return EquipSlots.None;
            }
        }

        EquipSlots GetWomensClothingSlot(DaggerfallUnityItem item)
        {
            ItemTemplate template = item.ItemTemplate;
            switch ((WomensClothing)template.index)
            {
                case WomensClothing.Brassier:
                case WomensClothing.Formal_brassier:
                case WomensClothing.Peasant_blouse:
                case WomensClothing.Eodoric:
                case WomensClothing.Formal_eodoric:
                case WomensClothing.Evening_gown:
                case WomensClothing.Day_gown:
                case WomensClothing.Casual_dress:
                case WomensClothing.Strapless_dress:
                case WomensClothing.Plain_robes:
                case WomensClothing.Priestess_robes:
                case WomensClothing.Short_shirt:
                case WomensClothing.Short_shirt_belt:
                case WomensClothing.Long_shirt:
                case WomensClothing.Long_shirt_belt:
                case WomensClothing.Short_shirt_closed:
                case WomensClothing.Short_shirt_closed_belt:
                case WomensClothing.Long_shirt_closed:
                case WomensClothing.Long_shirt_closed_belt:
                case WomensClothing.Short_shirt_unchangeable:
                case WomensClothing.Long_shirt_unchangeable:
                case WomensClothing.Open_tunic:
                case WomensClothing.Vest:
                    return EquipSlots.ChestClothes;

                case WomensClothing.Shoes:
                case WomensClothing.Tall_boots:
                case WomensClothing.Boots:
                case WomensClothing.Sandals:
                    return EquipSlots.Feet;

                case WomensClothing.Casual_pants:
                case WomensClothing.Khajiitt_suit:
                case WomensClothing.Loincloth:
                case WomensClothing.Wrap:
                case WomensClothing.Long_skirt:
                case WomensClothing.Tights:
                    return EquipSlots.LegsClothes;

                case WomensClothing.Casual_cloak:
                case WomensClothing.Formal_cloak:
                    return GetFirstSlot(EquipSlots.Cloak2, EquipSlots.Cloak1);

                default:
                    return EquipSlots.None;
            }
        }

        #endregion
    }
}