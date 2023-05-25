// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut
// Contributors:
//
// Notes:
//

using System.Collections.Generic;
using DaggerfallConnect.FallExe;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.UserInterface;
using UnityEngine;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    public class DaggerfallUseMagicItemWindow : DaggerfallListPickerWindow
    {

        KeyCode toggleClosedBinding;
        bool isCloseWindowDeferred = false;

        List<DaggerfallUnityItem> magicUseItems = new List<DaggerfallUnityItem>();

        #region Constructors

        public DaggerfallUseMagicItemWindow(IUserInterfaceManager uiManager, DaggerfallBaseWindow previous = null)
            : base(uiManager, previous)
        {
            ParentPanel.BackgroundColor = Color.clear;
            // Prevent duplicate close calls with base class's exitKey (Escape)
            AllowCancel = false;
        }

        #endregion

        #region Setup Methods

        protected override void Setup()
        {
            base.Setup();

            OnItemPicked += MagicItemPicker_OnItemPicked;

            toggleClosedBinding = InputManager.Instance.GetBinding(InputManager.Actions.UseMagicItem);

            Refresh();
        }

        public override void OnPush()
        {
            toggleClosedBinding = InputManager.Instance.GetBinding(InputManager.Actions.UseMagicItem);

            if (!IsSetup)
                return;

            Refresh();
        }

        public override void OnPop()
        {
            magicUseItems.Clear();
        }

        public override void Update()
        {
            base.Update();

            // Toggle window closed with same hotkey used to open it
            if (InputManager.Instance.GetKeyDown(toggleClosedBinding) || InputManager.Instance.GetBackButtonDown())
                isCloseWindowDeferred = true;
            else if ((InputManager.Instance.GetKeyUp(toggleClosedBinding) || InputManager.Instance.GetBackButtonUp()) && isCloseWindowDeferred)
            {
                isCloseWindowDeferred = false;
                CloseWindow();
            }
        }

        void Refresh()
        {
            ListBox.ClearItems();
            foreach (DaggerfallUnityItem magicUseItem in magicUseItems)
            {
                ListBox.AddItem(magicUseItem.LongName);
            }
        }

        public int UpdateUsableMagicItems()
        {
            magicUseItems.Clear();
            ItemCollection playerItems = GameManager.Instance.PlayerEntity.Items;
            for (int i = 0; i < playerItems.Count; i++)
            {
                DaggerfallUnityItem item = playerItems.GetItem(i);
                if (item.IsEnchanted && item.LegacyEnchantments != null)
                {
                    foreach (DaggerfallEnchantment enchantment in item.LegacyEnchantments)
                        if (enchantment.type == EnchantmentTypes.CastWhenUsed)
                        {
                            magicUseItems.Add(item);
                            break;
                        }
                }
                else if (item.IsPotion)
                {
                    magicUseItems.Add(item);
                }
            }
            return magicUseItems.Count;
        }

        #endregion

        #region Private Methods

        #endregion

        #region Event Handlers

        public void MagicItemPicker_OnItemPicked(int index, string itemName)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            DaggerfallUnityItem itemToUse = magicUseItems[index];
            CloseWindow();
            // Use item
            if (itemToUse.IsPotion)
            {
                GameManager.Instance.PlayerEffectManager.DrinkPotion(itemToUse);
                GameManager.Instance.PlayerEntity.Items.RemoveOne(itemToUse);
            }
            else if (itemToUse.IsEnchanted)
                GameManager.Instance.PlayerEffectManager.DoItemEnchantmentPayloads(MagicAndEffects.EnchantmentPayloadFlags.Used, itemToUse, GameManager.Instance.PlayerEntity.Items);
        }

        #endregion
    }
}