// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Podleron (podleron@gmail.com)

using DaggerfallConnect;
using UnityEngine;

namespace DaggerfallWorkshop.Game.Addons.RmbBlockEditor
{
#if UNITY_EDITOR
    [ExecuteInEditMode]
    public class Scenery : MonoBehaviour
    {
        private DFBlock.RmbGroundScenery[,] GroundScenery;
        private GameObject SceneryGameObject;
        private GameObject[,] SceneryItems;

        public void CreateObject(DFBlock.RmbGroundScenery[,] scenery)
        {
            GroundScenery = scenery;

            SceneryGameObject = new GameObject("Scenery");
            SceneryGameObject.transform.parent = transform;
            SceneryItems = new GameObject[16, 16];
            CreateItems();
        }

        public void AddItem(int textureRecord, int i, int j)
        {
            if (textureRecord != -1)
            {
                // Check if there is already a scenery item there and remove it
                var existingItem = SceneryItems[i, j];
                if (existingItem != null)
                {
                    DestroyImmediate(existingItem);
                }

                // Place the new item
                var goName = string.Format("Scenery-{0}-{1}", i, j);
                var sceneryItemGo = new GameObject(goName);
                var sceneryItem = sceneryItemGo.AddComponent<SceneryItem>();
                sceneryItem.CreateObject(textureRecord, i, j);
                sceneryItemGo.transform.parent = SceneryGameObject.transform;
                SceneryItems[i, j] = sceneryItemGo;
            }
        }

        private void CreateItems()
        {
            for (var i = 0; i < 16; i++)
            {
                for (var j = 0; j < 16; j++)
                {
                    var textureRecord = GroundScenery[i, j].TextureRecord;
                    AddItem(textureRecord, i, j);
                }
            }
        }

        public DFBlock.RmbGroundScenery[,] getGroundScenery()
        {
            DFBlock.RmbGroundScenery[,] groundScenery = new DFBlock.RmbGroundScenery[16, 16];
            for (var i = 0; i < 16; i++)
            {
                for (var j = 0; j < 16; j++)
                {
                    var itemGo = SceneryItems[i, j];
                    if (itemGo != null)
                    {
                        var item = itemGo.GetComponent<SceneryItem>();
                        groundScenery[i, j].TextureRecord = item.textureRecord;
                    }
                    else
                    {
                        groundScenery[i, j].TextureRecord = -1;
                    }
                }
            }

            return groundScenery;
        }
    }
#endif
}