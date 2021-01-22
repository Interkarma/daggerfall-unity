// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2020 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: TheLacus
// Contributors:     
// 
// Notes:
//

using System;
using UnityEngine;
using DaggerfallConnect;
using DaggerfallWorkshop.Game;

namespace DaggerfallWorkshop.Utility.AssetInjection
{
    /// <summary>
    /// Defines a Daggerfall material to be created at runtime.
    /// </summary>
    [Serializable]
    public struct RuntimeMaterial
    {
        [Tooltip("Material index on the MeshRenderer")]
        public int Index;

        [Tooltip("Daggerfall texture archive.")]
        public int Archive;

        [Tooltip("Daggerfall Texture record.")]
        public int Record;

        [Tooltip("Use texture for current climate and season. Archive must be the base archive.")]
        public bool ApplyClimate;
    }

    /// <summary>
    /// Holds a list of archives and records and uses them to assign materials when the prefab is instantiated at runtime.
    /// Materials are automatically applied on Awake or, if <see cref="UseDungeonTextureTable"/> is true, as part of dungeon layout
    /// or when requested with an overload of <see cref="ApplyMaterials"/>.
    /// </summary>
    [RequireComponent(typeof(MeshRenderer))]
    public sealed class RuntimeMaterials : MonoBehaviour
    {
        [SerializeField]
        [HideInInspector]
        private bool hasAppliedMaterials = false;

        [SerializeField]
        [Tooltip("List of materials to be assigned at runtime with Daggerfall textures.")]
        private RuntimeMaterial[] Materials;

        [SerializeField]
        [Tooltip("Use textures defined by dungeon texture table. Must be set by API if not part of classic dungeon layout.")]
        private bool UseDungeonTextureTable = false;

        bool subscribedToOnSetDungeon = false;

        void Awake()
        {
            // Apply materials when the gameobject is first instantiated (not when cloned).
            if (!hasAppliedMaterials)
            {
                ApplyMaterials(false);

                if (UseDungeonTextureTable)
                {
                    DaggerfallDungeon.OnSetDungeon += DaggerfallDungeon_OnSetDungeon;
                    subscribedToOnSetDungeon = true;
                }
            }
        }

        private void OnDestroy()
        {
            if (subscribedToOnSetDungeon)
            {
                subscribedToOnSetDungeon = false;
                DaggerfallDungeon.OnSetDungeon -= DaggerfallDungeon_OnSetDungeon;
            }
        }

        /// <summary>
        /// Applies materials to MeshRender found on gameobject using existing configuration.
        /// </summary>
        [ContextMenu("Reapply Materials")]
        public void ApplyMaterials()
        {
            if (Application.isPlaying)
                ApplyMaterials(true);
        }

        /// <summary>
        /// Applies materials to MeshRender found on gameobject with the given configuration.
        /// </summary>
        public void ApplyMaterials(RuntimeMaterial[] materials)
        {
            if (Materials == null || Materials.Length == 0)
                throw new ArgumentException("Materials array doesn't contain any item.", "materials");

            if (Application.isPlaying)
            {
                Materials = materials;
                UseDungeonTextureTable = false;
                ApplyMaterials(true);
            }
        }

        /// <summary>
        /// Applies materials to MeshRender found on gameobject with the given dungeon texture table
        /// and optionally overrides configuration.
        /// </summary>
        /// <param name="dungeonTextureTable">Texture table to be used.</param>
        /// <param name="materials">Overrides materials configuration if not null.</param>
        public void ApplyMaterials(int[] dungeonTextureTable, RuntimeMaterial[] materials = null)
        {
            if (dungeonTextureTable == null)
                throw new ArgumentNullException(nameof(dungeonTextureTable));

            if (Application.isPlaying)
            {
                if (materials != null)
                    Materials = materials;
                UseDungeonTextureTable = true;
                ApplyMaterials(true, dungeonTextureTable);
            }
        }

        private void ApplyMaterials(bool force, int[] dungeonTextureTable = null)
        {
            if (Materials == null || Materials.Length == 0)
                return;

            try
            {
                var meshRenderer = GetComponent<MeshRenderer>();
                if (!meshRenderer)
                {
                    Debug.LogErrorFormat("Failed to find MeshRenderer on {0}.", name);
                    return;
                }

                DFLocation.ClimateBaseType climateBaseType = GameManager.Instance.PlayerGPS.ClimateSettings.ClimateType;
                ClimateBases climate = ClimateSwaps.FromAPIClimateBase(climateBaseType);
                ClimateSeason season = DaggerfallUnity.Instance.WorldTime.Now.SeasonValue == DaggerfallDateTime.Seasons.Winter ? ClimateSeason.Winter : ClimateSeason.Summer;

                Material[] materials = meshRenderer.sharedMaterials;
                for (int i = 0; i < Materials.Length; i++)
                {
                    int index = Materials[i].Index;

                    if (!force && materials[index])
                        Debug.LogWarningFormat("A runtime material is being assigned to {0} (index {1}) but current material is not equal to null." +
                        " Make sure you are not including unnecessary auto-generated materials.", meshRenderer.name, i);

                    materials[index] = GetMaterial(Materials[i], climateBaseType, climate, season, dungeonTextureTable);
                    if (!materials[index])
                        Debug.LogErrorFormat("Failed to find material for {0} (index {1}).", meshRenderer.name, i);
                }
                meshRenderer.sharedMaterials = materials;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            finally
            {
                hasAppliedMaterials = true;
            }
        }

        private Material GetMaterial(RuntimeMaterial runtimeMaterial, DFLocation.ClimateBaseType climateBaseType, ClimateBases climate, ClimateSeason season, int[] dungeonTextureTable)
        {
            int archive = runtimeMaterial.Archive;
            int record = runtimeMaterial.Record;

            if (dungeonTextureTable != null)
                archive = DungeonTextureTables.ApplyTextureTable(archive, dungeonTextureTable, climateBaseType);

            if (runtimeMaterial.ApplyClimate)
                archive = ClimateSwaps.ApplyClimate(archive, record, climate, season);

            return DaggerfallUnity.Instance.MaterialReader.GetMaterial(archive, record);
        }

#if UNITY_EDITOR
        [ContextMenu("Init from MeshRenderer")]
        private void InitFromMeshRenderer()
        {
            var meshRenderer = GetComponent<MeshRenderer>();
            if (meshRenderer)
            {
                Materials = new RuntimeMaterial[meshRenderer.sharedMaterials.Length];
                for (int i = 0; i < Materials.Length; i++)
                {
                    Materials[i] = new RuntimeMaterial()
                    {
                        Index = i
                    };
                }
            }
            else
            {
                Materials = null;
            }
        }
#endif

        private void DaggerfallDungeon_OnSetDungeon(DaggerfallDungeon daggerfallDungeon)
        {
            if (transform.IsChildOf(daggerfallDungeon.transform))
            {
                DaggerfallDungeon.OnSetDungeon -= DaggerfallDungeon_OnSetDungeon;
                subscribedToOnSetDungeon = false;
                ApplyMaterials(true, daggerfallDungeon.DungeonTextureTable);
            }
        }
    }
}
