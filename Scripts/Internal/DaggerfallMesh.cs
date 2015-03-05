// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

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
        public void SetClimate(DaggerfallUnity dfUnity, ClimateBases climate, ClimateSeason season, WindowStyle windowStyle)
        {
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
            int archive, record, frame;
            Material[] materials = new Material[defaultTextures.Count];
            for (int i = 0; i < defaultTextures.Count; i++)
            {
                MaterialReader.ReverseTextureKey(defaultTextures[i], out archive, out record, out frame);
                switch (archive)
                {
                    case 119:
                        archive = dungeonTextureTable[0];
                        break;
                    case 120:
                        archive = dungeonTextureTable[1];
                        break;
                    case 122:
                        archive = dungeonTextureTable[2];
                        break;
                    case 123:
                        archive = dungeonTextureTable[3];
                        break;
                    case 124:
                        archive = dungeonTextureTable[4];
                        break;
                    case 168:
                        archive = dungeonTextureTable[5];
                        break;
                }
                materials[i] = dfUnity.MaterialReader.GetMaterial(archive, record);
            }

            // Assign material array
            if (materials != null)
                GetComponent<MeshRenderer>().sharedMaterials = materials;
        }
    }
}