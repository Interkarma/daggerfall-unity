// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2020 Daggerfall Workshop
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
using DaggerfallWorkshop.Game.MagicAndEffects;

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

        readonly DaggerfallEntity parentEntity = null;
        DaggerfallUnityItem[] equipTable = new DaggerfallUnityItem[equipTableLength];

        #endregion

        #region Properties

        /// <summary>
        /// Gets length of equip table.
        /// </summary>
        public static int EquipTableLength
        {
            get { return equipTableLength; }
        }

        /// <summary>
        /// Gets current equip table.
        /// </summary>
        public DaggerfallUnityItem[] EquipTable
        {
            get { return equipTable; }
        }

        #endregion

        #region Constructors

        public ItemEquipTable(DaggerfallEntity parentEntity)
        {
            this.parentEntity = parentEntity;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Clears equip table.
        /// Does not unequip items, just wipes table.
        /// Used when clearing state for a new game.
        /// </summary>
        public void Clear()
        {
            equipTable = new DaggerfallUnityItem[equipTableLength];
        }

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
        public List<DaggerfallUnityItem> EquipItem(DaggerfallUnityItem item, bool alwaysEquip = true, bool playEquipSounds = true)
        {
            if (item == null)
                return null;

            // Get slot for this item
            EquipSlots slot = GetEquipSlot(item);
            if (slot == EquipSlots.None)
                return null;

            // If more than one item selected, equip only one
            if (item.IsAStack())
                item = GameManager.Instance.PlayerEntity.Items.SplitStack(item, 1);
            
            List<DaggerfallUnityItem> unequippedList = new List<DaggerfallUnityItem>();

            // Special weapon handling
            if (item.ItemGroup == ItemGroups.Weapons)
            {
                // Cannot equip arrows
                if (item.TemplateIndex == (int)Weapons.Arrow)
                    return null;

                // Equipping a 2H weapons will always unequip both hands
                if (GetItemHands(item) == ItemHands.Both)
                {
                    UnequipItem(EquipSlots.LeftHand, unequippedList);
                    UnequipItem(EquipSlots.RightHand, unequippedList);
                }
            }

            // Equipping a shield will always unequip 2H weapon
            if (GetItemHands(item) == ItemHands.LeftOnly)
            {
                // If holding a 2H weapon then unequip
                DaggerfallUnityItem rightHandItem = equipTable[(int)EquipSlots.RightHand];
                if (rightHandItem != null && GetItemHands(rightHandItem) == ItemHands.Both)
                    UnequipItem(EquipSlots.RightHand, unequippedList);
            }

            // Unequip any previous item
            if (!IsSlotOpen(slot) && !alwaysEquip)
                return null;
            else
                UnequipItem(slot, unequippedList);

            // Equip item to slot
            item.EquipSlot = slot;
            equipTable[(int)slot] = item;

            // Play equip sound
            if (playEquipSounds)
                DaggerfallUI.Instance.PlayOneShot(item.GetEquipSound());

            // Allow entity effect manager to start any enchantments on this item
            StartEquippedItem(item);

            //Debug.Log(string.Format("Equipped {0} to {1}", item.LongName, slot.ToString()));

            return unequippedList;
        }

        public void UnequipItem(EquipSlots slot, List<DaggerfallUnityItem> list)
        {
            DaggerfallUnityItem item = UnequipItem(slot);
            if (item != null)
                list.Add(item);

            // Allow entity effect manager to stop any enchantments on this item
            StopEquippedItem(item);
        }

        /// <summary>
        /// Checks if an item is currently equipped.
        /// </summary>
        /// <param name="item">Item to check.</param>
        /// <returns>True if item is equipped.</returns>
        public bool IsEquipped(DaggerfallUnityItem item)
        {
            if (item == null)
                return false;

            for (int i = 0; i < equipTable.Length; i++)
            {
                if (equipTable[i] == null)
                    continue;

                if (equipTable[i].UID == item.UID)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Attempt to unequip item from slot.
        /// </summary>
        /// <param name="slot">Slot to unequip.</param>
        /// <returns>The item unequipped, otherwise null.</returns>
        public DaggerfallUnityItem UnequipItem(EquipSlots slot)
        {
            if (!IsSlotOpen(slot))
            {
                DaggerfallUnityItem item = equipTable[(int)slot];
                equipTable[(int)slot].EquipSlot = EquipSlots.None;
                equipTable[(int)slot] = null;

                // Allow entity effect manager to stop any enchantments on this item
                StopEquippedItem(item);

                return item;
            }

            return null;
        }

        /// <summary>
        /// Attempt to unequip a specific item.
        /// </summary>
        /// <param name="item">Item to unequip.</param>
        /// <returns>True if item was found to be equipped and was unequipped.</returns>
        public bool UnequipItem(DaggerfallUnityItem item)
        {
            if (item == null)
                return false;

            for (int i = 0; i < equipTable.Length; i++)
            {
                if (equipTable[i] != null && equipTable[i].UID == item.UID)
                {
                    equipTable[i].EquipSlot = EquipSlots.None;
                    equipTable[i] = null;
                    return true;
                }
            }

            // Allow entity effect manager to stop any enchantments on this item
            StopEquippedItem(item);

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

            // Resolve based on equipment category
            switch (item.ItemGroup)
            {
                case ItemGroups.Gems:
                    result = GetGemSlot();
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
            if (!IsSlotOpen(EquipSlots.LegsClothes))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Serialize equip table into list of equipped UIDs.
        /// </summary>
        /// <returns>ulong array.</returns>
        public ulong[] SerializeEquipTable()
        {
            ulong[] data = new ulong[equipTableLength];
            for (int i = 0; i < equipTableLength; i++)
            {
                if (equipTable[i] != null)
                    data[i] = equipTable[i].UID;
                else
                    data[i] = 0;
            }

            return data;
        }

        /// <summary>
        /// Deserialize equip table and attempt to relink equipped items in collection.
        /// Item UID must exist inside collection provided or entry will not be restored and item not equipped.
        /// </summary>
        /// <param name="data">UID array exactly this.equipTableLength in length.</param>
        /// <param name="items">Item collection.</param>
        public void DeserializeEquipTable(ulong[] data, ItemCollection items)
        {
            if (data == null || items == null || data.Length != equipTableLength)
                return;

            Clear();

            for (int i = 0; i < equipTableLength; i++)
            {
                ulong uid = data[i];
                if (items.Contains(uid))
                {
                    DaggerfallUnityItem item = items.GetItem(uid);
                    if (item != null)
                    {
                        equipTable[i] = item;
                        item.EquipSlot = (EquipSlots)i;
                    }
                }
            }
        }

        #endregion

        #region Private Methods

        EquipSlots GetGemSlot()
        {
            return GetFirstSlot(EquipSlots.Crystal0, EquipSlots.Crystal1);
        }

        EquipSlots GetJewellerySlot(DaggerfallUnityItem item)
        {
            ItemTemplate template = item.ItemTemplate;
            switch ((Jewellery)template.index)
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
            switch ((Armor)template.index)
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
            // If a 2H weapon is currently equipped then next weapon will always replace it in right hand
            DaggerfallUnityItem rightHandItem = equipTable[(int)EquipSlots.RightHand];
            if (rightHandItem != null && GetItemHands(rightHandItem) == ItemHands.Both && GetItemHands(item) != ItemHands.LeftOnly)
                return EquipSlots.RightHand;

            // Find best hand for this item
            ItemHands hands = GetItemHands(item);
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
            switch ((MensClothing)template.index)
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
                case MensClothing.Plain_robes:
                case MensClothing.Priest_robes:
                case MensClothing.Short_shirt:
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

                case MensClothing.Casual_pants:
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
                case WomensClothing.Khajiit_suit:
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

        void StartEquippedItem(DaggerfallUnityItem item)
        {
            if (parentEntity != null && parentEntity.EntityBehaviour)
            {
                EntityEffectManager manager = parentEntity.EntityBehaviour.GetComponent<EntityEffectManager>();
                if (manager)
                    manager.DoItemEnchantmentPayloads(EnchantmentPayloadFlags.Equipped | EnchantmentPayloadFlags.Held, item);
            }
        }

        void StopEquippedItem(DaggerfallUnityItem item)
        {
            if (parentEntity != null && parentEntity.EntityBehaviour)
            {
                EntityEffectManager manager = parentEntity.EntityBehaviour.GetComponent<EntityEffectManager>();
                if (manager)
                    manager.DoItemEnchantmentPayloads(EnchantmentPayloadFlags.Unequipped, item);
            }
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Gets how item is held in the hands.
        /// Item templates define whether item is 1 or 2 handed, but this method
        /// provides more exact information about which hand item can be used in.
        /// </summary>
        public static ItemHands GetItemHands(DaggerfallUnityItem item)
        {
            // Must be of group Weapons or Armor (for shields)
            if (item.ItemGroup != ItemGroups.Weapons &&
                item.ItemGroup != ItemGroups.Armor)
            {
                return ItemHands.None;
            }

            // Compare against supported weapon types
            switch ((Weapons)item.TemplateIndex)
            {
                // These weapons can be used in either hand
                case Weapons.Dagger:
                case Weapons.Tanto:
                case Weapons.Shortsword:
                case Weapons.Wakazashi:
                case Weapons.Broadsword:
                case Weapons.Saber:
                case Weapons.Longsword:
                case Weapons.Katana:
                case Weapons.Battle_Axe:
                case Weapons.Mace:
                    return ItemHands.Either;

                // Two-handed weapons only
                case Weapons.Claymore:
                case Weapons.Dai_Katana:
                case Weapons.War_Axe:
                case Weapons.Staff:
                case Weapons.Flail:
                case Weapons.Warhammer:
                    return ItemHands.Both;

                case Weapons.Short_Bow:
                case Weapons.Long_Bow:
                    return DaggerfallUnity.Settings.BowLeftHandWithSwitching ? ItemHands.LeftOnly : ItemHands.Both;
            }

            // Compare against supported armor types
            switch ((Armor)item.TemplateIndex)
            {
                case Armor.Buckler:
                case Armor.Round_Shield:
                case Armor.Kite_Shield:
                case Armor.Tower_Shield:
                    return ItemHands.LeftOnly;
            }

            // Nothing found
            return ItemHands.None;
        }

        #endregion
    }
}
