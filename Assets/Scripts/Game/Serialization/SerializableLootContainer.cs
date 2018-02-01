// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
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
            data.stockedDate = loot.stockedDate;
            data.customDrop = loot.customDrop;
            data.items = loot.Items.SerializeItems();
            data.entityName = loot.entityName;
            data.isEnemyClass = loot.isEnemyClass;

            return data;
        }

        public void RestoreSaveData(object dataIn)
        {
            if (!loot)
                return;

            LootContainerData_v1 data = (LootContainerData_v1)dataIn;
            if (data.loadID != LoadID)
                return;

            // Restore billboard only if this is a billboard-based loot container
            if (loot.ContainerType == LootContainerTypes.RandomTreasure ||
                loot.ContainerType == LootContainerTypes.CorpseMarker ||
                loot.ContainerType == LootContainerTypes.DroppedLoot)
            {
                DaggerfallBillboard billboard = loot.GetComponent<DaggerfallBillboard>();

                // Restore position
                loot.transform.position = data.currentPosition;

                // Restore appearance
                if (MeshReplacement.ImportCustomFlatGameobject(data.textureArchive, data.textureRecord, Vector3.zero, loot.transform))
                {
                    // Use imported model instead of billboard
                    if (billboard) Destroy(billboard);
                    Destroy(GetComponent<MeshRenderer>());
                }
                else if (billboard)
                {
                    // Restore billboard appearance if present
                    billboard.SetMaterial(data.textureArchive, data.textureRecord);
                }
            }

            // Restore items
            loot.Items.DeserializeItems(data.items);

            // Restore other data
            loot.ContainerType = data.containerType;
            loot.ContainerImage = data.containerImage;
            loot.TextureArchive = data.textureArchive;
            loot.TextureRecord = data.textureRecord;
            loot.stockedDate = data.stockedDate;
            loot.customDrop = data.customDrop;
            loot.name = loot.ContainerType.ToString();
            loot.entityName = data.entityName;
            loot.isEnemyClass = data.isEnemyClass;

            // Remove loot container if empty
            if (loot.Items.Count == 0)
                GameObjectHelper.RemoveLootContainer(loot);
        }

        #endregion

        #region Private Methods

        bool HasChanged()
        {
            // Save all loot containers, except for shelves & house containers that have not been opened and have a zero stockedDate
            return !((loot.ContainerType == LootContainerTypes.ShopShelves || loot.ContainerType == LootContainerTypes.ShopShelves) && loot.stockedDate == 0);
        }

        ulong GetLoadID()
        {
            return loot.LoadID;
        }

        #endregion
    }
}