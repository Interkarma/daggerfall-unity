// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
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
            data.localPosition = loot.transform.localPosition;
            data.worldCompensation = GameManager.Instance.StreamingWorld.WorldCompensation;
            data.heightScale = loot.transform.localScale.y;
            data.worldContext = loot.WorldContext;
            data.textureArchive = loot.TextureArchive;
            data.textureRecord = loot.TextureRecord;
            data.stockedDate = loot.stockedDate;
            data.corpseQuestUID = loot.corpseQuestUID;
            data.playerOwned = loot.playerOwned;
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
                Billboard billboard = loot.GetComponent<Billboard>();

                // Interiors and exteriors need special handling to ensure loot is always placed correctly for pre and post floating y saves
                // Dungeons are not involved with floating y and don't need any changes
                WorldContext lootContext = GetLootWorldContext(loot);
                if (lootContext == WorldContext.Interior)
                {
                    RestoreInteriorPositionHandler(loot, data, lootContext);
                }
                else if (lootContext == WorldContext.Exterior)
                {
                    RestoreExteriorPositionHandler(loot, data, lootContext);
                }
                else
                {
                    loot.transform.position = data.currentPosition;
                }

                // Restore appearance
                if (MeshReplacement.SwapCustomFlatGameobject(data.textureArchive, data.textureRecord, loot.transform, Vector3.zero, lootContext == WorldContext.Dungeon))
                {
                    // Use imported model instead of billboard
                    if (billboard) Destroy(billboard);
                    Destroy(GetComponent<MeshRenderer>());
                }
                else
                {
                    // Restore billboard if previously replaced by custom model
                    // This happens when the record is changed and new model is not provided by mods
                    if (!billboard)
                        billboard = loot.transform.gameObject.AddComponent<DaggerfallBillboard>();

                    // Restore billboard appearance
                    billboard.SetMaterial(data.textureArchive, data.textureRecord);

                    // Fix position if custom scale changed
                    if (data.heightScale == 0)
                        data.heightScale = 1;
                    if (data.heightScale != billboard.transform.localScale.y)
                    {
                        float height = billboard.Summary.Size.y * (data.heightScale / billboard.transform.localScale.y);
                        billboard.transform.Translate(0, (billboard.Summary.Size.y - height) / 2f, 0);
                    }
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
            loot.corpseQuestUID = data.corpseQuestUID;
            loot.playerOwned = data.playerOwned;
            loot.customDrop = data.customDrop;
            loot.entityName = data.entityName;
            loot.isEnemyClass = data.isEnemyClass;

            // Remove loot container if empty
            if (loot.Items.Count == 0)
                GameObjectHelper.RemoveLootContainer(loot);
        }

        #endregion

        #region Private Methods

        void RestoreExteriorPositionHandler(DaggerfallLoot loot, LootContainerData_v1 data, WorldContext lootContext)
        {
            // If loot context matches serialized world context then loot was saved after floating y change
            // Need to get relative difference between current and serialized world compensation to get actual y position
            if (lootContext == data.worldContext)
            {
                float diffY = GameManager.Instance.StreamingWorld.WorldCompensation.y - data.worldCompensation.y;
                loot.transform.position = data.currentPosition + new Vector3(0, diffY, 0);
                return;
            }

            // Otherwise we migrate a legacy exterior position by adjusting for world compensation
            loot.transform.position = data.currentPosition + GameManager.Instance.StreamingWorld.WorldCompensation;
        }

        void RestoreInteriorPositionHandler(DaggerfallLoot loot, LootContainerData_v1 data, WorldContext lootContext)
        {
            // If loot context matches serialized world context then loot was saved after floating y change
            // Can simply restore local position relative to parent interior
            if (lootContext == data.worldContext)
            {
                loot.transform.localPosition = data.localPosition;
                return;
            }

            // Otherwise we need to migrate a legacy interior position to floating y
            if (GameManager.Instance.PlayerEnterExit.LastInteriorStartFlag)
            {
                // Loading interior uses serialized absolute position (as interior also serialized this way)
                loot.transform.position = data.currentPosition;
            }
            else
            {
                // Transition to interior must offset serialized absolute position by floating y compensation
                loot.transform.position = data.currentPosition + GameManager.Instance.StreamingWorld.WorldCompensation;
            }
        }

        WorldContext GetLootWorldContext(DaggerfallLoot loot)
        {
            // Must be a parented loot container
            if (!loot || !loot.transform.parent)
                return WorldContext.Nothing;

            // Interior
            if (loot.transform.parent.GetComponentInParent<DaggerfallInterior>())
                return WorldContext.Interior;

            // Dungeon
            if (loot.transform.parent.GetComponentInParent<DaggerfallDungeon>())
                return WorldContext.Dungeon;

            // Exterior (loose world object)
            return WorldContext.Exterior;
        }

        bool HasChanged()
        {
            // Save all loot containers, except for shelves & house containers that have not been opened and have a zero stockedDate
            return !((loot.ContainerType == LootContainerTypes.ShopShelves || loot.ContainerType == LootContainerTypes.HouseContainers) && loot.stockedDate == 0);
        }

        ulong GetLoadID()
        {
            return loot.LoadID;
        }

        #endregion
    }
}