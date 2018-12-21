// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut
// Contributors:
//
// Notes:
//

using UnityEngine;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Items;
using System.Collections.Generic;
using DaggerfallConnect.FallExe;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    public class DaggerfallUseMagicItemWindow : DaggerfallListPickerWindow
    {

        KeyCode toggleClosedBinding;

        List<DaggerfallUnityItem> magicUseItems = new List<DaggerfallUnityItem>();

        #region Constructors

        public DaggerfallUseMagicItemWindow(IUserInterfaceManager uiManager, DaggerfallBaseWindow previous = null)
            : base(uiManager, previous)
        {
            ParentPanel.BackgroundColor = Color.clear;
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
            // Toggle window closed with same hotkey used to open it
            if (Input.GetKeyUp(toggleClosedBinding))
                CloseWindow();
            else
                base.Update();
        }

        void Refresh()
        {
            ItemCollection playerItems = GameManager.Instance.PlayerEntity.Items;
            for (int i = 0; i < playerItems.Count; i++)
            {
                DaggerfallUnityItem item = playerItems.GetItem(i);
                if (item.IsEnchanted)
                {
                    foreach(DaggerfallEnchantment enchantment in item.Enchantments)
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

            if (magicUseItems.Count > 0)
            {
                ListBox.ClearItems();
                foreach (DaggerfallUnityItem magicUseItem in magicUseItems)
                {
                    ListBox.AddItem(magicUseItem.LongName);
                }
            }
            else
                CloseWindow();
        }

        #endregion

        #region Private Methods

        #endregion

        #region Event Handlers

        public void MagicItemPicker_OnItemPicked(int index, string itemName)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            // Use item
            DaggerfallUnityItem itemToUse = magicUseItems[index];
            if (itemToUse.IsPotion)
            {
                GameManager.Instance.PlayerEffectManager.DrinkPotion(itemToUse);
                GameManager.Instance.PlayerEntity.Items.RemoveOne(itemToUse);
            }
            else if (itemToUse.IsEnchanted)
                GameManager.Instance.PlayerEffectManager.UseItem(itemToUse, GameManager.Instance.PlayerEntity.Items);

            CloseWindow();
        }

        #endregion
    }
}