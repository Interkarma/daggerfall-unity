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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DaggerfallConnect;
using DaggerfallConnect.Utility;
using DaggerfallConnect.Arena2;

namespace DaggerfallWorkshop
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class DaggerfallMesh : MonoBehaviour
    {
        [SerializeField]
        private List<int> defaultTextures = new List<int>();
        [SerializeField]
        private ClimateBases currentClimate;
        [SerializeField]
        private ClimateSeason currentSeason;
        [SerializeField]
        private WindowStyle currentWindowStyle;

        public ClimateBases Climate
        {
            get { return currentClimate; }
            set { currentClimate = value; }
        }

        public ClimateSeason Season
        {
            get { return currentSeason; }
            set { currentSeason = value; }
        }

        public WindowStyle WindowStyle
        {
            get { return currentWindowStyle; }
            set { currentWindowStyle = value; }
        }

        /// <summary>
        /// Gets number of default textures on model.
        /// </summary>
        public int DefaultTexturesCount
        {
            get { return defaultTextures.Count; }
        }

        /// <summary>
        /// Set default texture keys.
        /// This is used to rebuild materials for texture swaps.
        /// </summary>
        /// <param name="textureKeys">Array of texture keys.</param>
        public void SetDefaultTextures(int[] textureKeys)
        {
            if (textureKeys == null)
            {
                defaultTextures.Clear();
                return;
            }

            defaultTextures.AddRange(textureKeys);
        }

        /// <summary>
        /// Get default texture keys.
        /// </summary>
        /// <returns>Copy of default texture array.</returns>
        public int[] GetDefaultTextures()
        {
            return defaultTextures.ToArray();
        }

        /// <summary>
        /// Rebuild materials back to default with no climate modifier.
        /// </summary>
        /// <param name="dfUnity">DaggerfallUnity singleton. Required for content readers and settings.</param>
        /// <param name="climate">Climate to set.</param>
        /// <param name="season">Season to set.</param>
        /// <param name="windowStyle">Style of window to set.</param>
        public void SetClimate(ClimateBases climate, ClimateSeason season, WindowStyle windowStyle)
        {
            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;

            if (defaultTextures.Count == 0)
                return;

            // Get new material array
            Material[] materials = new Material[defaultTextures.Count];
            for (int i = 0; i < defaultTextures.Count; i++)
            {
                materials[i] = dfUnity.MaterialReader.ChangeClimate(defaultTextures[i], climate, season, windowStyle);
            }

            // Assign material array
            if (materials != null)
                GetComponent<MeshRenderer>().sharedMaterials = materials;

            // Store climate settings
            currentClimate = climate;
            currentSeason = season;
            currentWindowStyle = windowStyle;
        }

        /// <summary>
        /// Applies current climate settings to model.
        /// </summary>
        public void ApplyCurrentClimate()
        {
            SetClimate(currentClimate, currentSeason, currentWindowStyle);
        }

        /// <summary>
        /// Rebuild materials back to default with no climate modifier.
        /// </summary>
        /// <param name="dfUnity">DaggerfallUnity singleton. Required for content readers and settings.</param>
        public void DisableClimate(DaggerfallUnity dfUnity)
        {
            if (defaultTextures.Count == 0)
                return;

            // Get new material array
            int archive, record, frame;
            Material[] materials = new Material[defaultTextures.Count];
            for (int i = 0; i < defaultTextures.Count; i++)
            {
                MaterialReader.ReverseTextureKey(defaultTextures[i], out archive, out record, out frame);
                materials[i] = dfUnity.MaterialReader.GetMaterial(archive, record);
            }

            // Assign material array
            if (materials != null)
                GetComponent<MeshRenderer>().sharedMaterials = materials;
        }

        /// <summary>
        /// Apply dungeon texture table.
        /// </summary>
        /// <param name="dungeonTextureTable">Dungeon texture table changes to apply.</param>
        public void SetDungeonTextures(int[] dungeonTextureTable)
        {
            if (defaultTextures.Count == 0)
                return;

            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;
            if (!dfUnity.IsReady)
                return;

            // Get new material array
            Material[] materials = new Material[defaultTextures.Count];
            DFLocation.ClimateBaseType climateIndex = Game.GameManager.Instance.PlayerGPS.ClimateSettings.ClimateType;
            for (int i = 0; i < defaultTextures.Count; i++)
            {
                MaterialReader.ReverseTextureKey(defaultTextures[i], out int archive, out int record, out _);
                archive = DungeonTextureTables.ApplyTextureTable(archive, dungeonTextureTable, climateIndex);
                materials[i] = dfUnity.MaterialReader.GetMaterial(archive, record);
            }

            // Assign material array
            if (materials != null)
                GetComponent<MeshRenderer>().sharedMaterials = materials;
        }
    }
}