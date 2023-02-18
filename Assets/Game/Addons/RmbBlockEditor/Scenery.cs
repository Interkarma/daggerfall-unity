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
        private ClimateBases climate;
        private ClimateSeason season;

        public void CreateObject(DFBlock.RmbGroundScenery[,] scenery, ClimateBases climate, ClimateSeason season)
        {
            GroundScenery = scenery;
            this.climate = climate;
            this.season = season;

            SceneryGameObject = new GameObject("Scenery");
            SceneryGameObject.transform.parent = transform;
            CreateItems();
        }

        public void SetClimate(ClimateBases climate, ClimateSeason season)
        {
            this.climate = climate;
            this.season = season;
            var items = SceneryGameObject.GetComponentsInChildren<Billboard>();
            foreach (var billboard in items)
            {
                DestroyImmediate(billboard.gameObject);
            }

            CreateItems();
        }

        private string getTextureArchive()
        {
            if (climate == ClimateBases.Desert)
            {
                return "503";
            }

            if (climate == ClimateBases.Swamp)
            {
                return "502";
            }

            if (climate == ClimateBases.Temperate && season != ClimateSeason.Winter)
            {
                return "504";
            }

            if (climate == ClimateBases.Temperate && season == ClimateSeason.Winter)
            {
                return "505";
            }

            if (climate == ClimateBases.Mountain && season != ClimateSeason.Winter)
            {
                return "510";
            }

            if (climate == ClimateBases.Mountain && season == ClimateSeason.Winter)
            {
                return "511";
            }

            return "504";
        }

        public void AddItem(int textureRecord, int i, int j)
        {
            var xPos = i * 6.4f;
            var zPos = j * 6.4f;
            if (textureRecord != -1)
            {
                // Check if there is already a scenery item there and remove it
                var allItems = SceneryGameObject.GetComponentsInChildren<SceneryItem>();
                foreach (var item in allItems)
                {
                    if (item.i == i && item.j == j)
                    {
                        DestroyImmediate(item.gameObject);
                    }
                }

                // Place the new item
                var sceneryFlatObject = RmbBlockHelper.AddFlatObject(getTextureArchive() + "." + textureRecord);
                var billboard = sceneryFlatObject.GetComponent<Billboard>();
                sceneryFlatObject.transform.position = new Vector3(xPos, billboard.Summary.Size.y / 2, -zPos);
                sceneryFlatObject.transform.parent = SceneryGameObject.transform;
                var component = sceneryFlatObject.gameObject.AddComponent<SceneryItem>();
                component.CreateObject(textureRecord, i, j);
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
                    groundScenery[i, j].TextureRecord = -1;
                }
            }

            var sceneryItems = GetComponentsInChildren<SceneryItem>();
            foreach (var sceneryItem in sceneryItems)
            {
                groundScenery[sceneryItem.i, sceneryItem.j].TextureRecord = sceneryItem.textureRecord;
            }

            return groundScenery;
        }
    }
    #endif
}