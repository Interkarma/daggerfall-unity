// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    Allofich
// 
// Notes:
//

using System.Collections;
using System.Collections.Generic;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace DaggerfallWorkshop.Game.Entity
{
    public class PetBehaviour : MonoBehaviour
    {
        public Slider HpTransform;

        private PlayerHealth playerHealth;
        DaggerfallEntityBehaviour playerEntity;

        void Start()
        {
            playerHealth = GameManager.Instance.PlayerHealth;

            playerEntity = GameManager.Instance.PlayerEntityBehaviour;

            HpTransform.transform.position = transform.position + new Vector3(0, 0.5f, 0);

            playerHealth.OnHealthChangedAction += CheckPlayerHealth;
            Debug.Log("PET BEHAVIUR ");
        }

        public void SetHealth(int maxHealth)
        {
            HpTransform.maxValue = maxHealth;
            HpTransform.value = maxHealth;
        }

        private void CheckPlayerHealth(int health, int maxHealth)
        {
            int healthPercent = (health / maxHealth) * 100;
            if (healthPercent <= 25)
            {
                // cast heal
                Debug.LogWarning("health CHANGE");

                ItemCollection droppedItems = new ItemCollection();
                droppedItems.AddItem(ItemBuilder.CreatePotion(4937012));
                if (droppedItems.Count > 0)
                {
                    DaggerfallLoot droppedLootContainer = GameObjectHelper.CreateDroppedLootContainer(GameManager.Instance.PlayerObject, DaggerfallUnity.NextUID);

                    droppedLootContainer.Items.TransferAll(droppedItems);

                    Vector3 pos = new Vector3(transform.position.x, droppedLootContainer.transform.position.y, transform.position.z);
                    droppedLootContainer.transform.position = pos;
                }
            }

        }

    }
}