// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2016 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using System;
using System.Collections.Generic;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Utility.AssetInjection;

namespace DaggerfallWorkshop.Game.Serialization
{
    public class SerializableLootContainer : MonoBehaviour, ISerializableGameObject
    {
        #region Fields

        DaggerfallLoot loot;

        #endregion

        #region Unity

        void Awake()
        {
            loot = GetComponent<DaggerfallLoot>();
            if (!loot)
                throw new Exception("DaggerfallLoot not found.");
        }

        void Start()
        {
            if (LoadID != 0)
                SaveLoadManager.RegisterSerializableGameObject(this);
        }

        void OnDestroy()
        {
            if (LoadID != 0)
                SaveLoadManager.DeregisterSerializableGameObject(this);
        }

        #endregion

        #region ISerializableGameObject

        public ulong LoadID { get { return GetLoadID(); } }
        public bool ShouldSave { get { return HasChanged(); } }

        public object GetSaveData()
        {
            if (!loot)
                return null;

            // Create save data
            LootContainerData_v1 data = new LootContainerData_v1();
            data.loadID = LoadID;
            data.containerType = loot.ContainerType;
            data.containerImage = loot.ContainerImage;
            data.currentPosition = loot.transform.position;
            data.textureArchive = loot.TextureArchive;
            data.textureRecord = loot.TextureRecord;
            data.lootTableKey = loot.LootTableKey;
            data.playerOwned = loot.playerOwned;
            data.customDrop = loot.customDrop;
            data.items = loot.Items.SerializeItems();

            return data;
        }

        public void RestoreSaveData(object dataIn)
        {
            if (!loot)
                return;

            LootContainerData_v1 data = (LootContainerData_v1)dataIn;
            if (data.loadID != LoadID)
                return;

            DaggerfallBillboard billboard = loot.GetComponent<DaggerfallBillboard>();

            // Restore position
            loot.transform.position = data.currentPosition;

            // Restore billboard appearance if present
            if (billboard)
            {
                billboard.SetMaterial(data.textureArchive, data.textureRecord);

                // Setup custom material if available
                if (TextureReplacement.CustomTextureExist(data.textureArchive, data.textureRecord))
                    TextureReplacement.SetBillboardCustomMaterial(billboard.gameObject, data.textureArchive, data.textureRecord);
            }

            // Restore items
            loot.Items.DeserializeItems(data.items);

            // Restore other data
            loot.ContainerType = data.containerType;
            loot.ContainerImage = data.containerImage;
            loot.LootTableKey = data.lootTableKey;
            loot.TextureArchive = data.textureArchive;
            loot.TextureRecord = data.textureRecord;
            loot.playerOwned = data.playerOwned;
            loot.customDrop = data.customDrop;
            loot.name = loot.ContainerType.ToString();

            // Remove loot container if empty
            if (loot.Items.Count == 0)
                GameObjectHelper.RemoveLootContainer(loot);
        }

        #endregion

        #region Private Methods

        bool HasChanged()
        {
            // Always save loot containers
            return true;
        }

        ulong GetLoadID()
        {
            return loot.LoadID;
        }

        #endregion
    }
}