// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using DaggerfallConnect;
using DaggerfallConnect.Utility;
using DaggerfallConnect.Arena2;

namespace DaggerfallWorkshop
{
    /// <summary>
    /// Draws a large number of atlased, non-animated billboards using a single mesh.
    /// Currently used for exterior nature billboards only (origin = centre-bottom).
    /// Support for dungeon billboards will be added later (origin = centre).
    /// Tries to not recreate Mesh and Material where possible.
    /// Generates some garbage when rebuilding mesh layout.
    /// This can probably be improved.
    /// </summary>
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class DaggerfallBillboardBatch : MonoBehaviour
    {
        // Maximum allowable billboards before mesh buffer overrun
        const int maxBillboardCount = 16250;

        [SerializeField, HideInInspector]
        Rect[] atlasRects;
        [SerializeField, HideInInspector]
        RecordIndex[] atlasIndices;
        [SerializeField, HideInInspector]
        Vector2[] atlasSizes;
        [SerializeField, HideInInspector]
        Vector2[] atlasScales;
        [SerializeField, HideInInspector]
        List<BillboardItem> billboardItems = new List<BillboardItem>();
        [SerializeField, HideInInspector]
        Mesh billboardMesh;

        [NonSerialized, HideInInspector]
        public Vector3 origin = Vector3.zero;

        [Range(500, 511)]
        public int NatureMaterial = 504;
        public bool CastShadows = true;
        [Range(1, 127)]
        public int RandomWidth = 16;
        [Range(1, 127)]
        public int RandomDepth = 16;
        public float RandomSpacing = BlocksFile.TileDimension * MeshReader.GlobalScale;

        DaggerfallUnity dfUnity;
        int currentArchive = -1;

        [Serializable]
        struct BillboardItem
        {
            public int record;
            public Vector3 position;
        }

        /// <summary>
        /// Set material all billboards in this batch will share.
        /// This material is always atlased.
        /// </summary>
        /// <param name="archive">Archive index.</param>
        public void SetMaterial(int archive, bool force = false)
        {
            if (!ReadyCheck())
                return;

            // Do nothing if this archive already set
            if (archive == currentArchive && !force)
                return;

            // Get standard atlas material
            // Just going to steal texture and settings
            // TODO: Revise material loading for custom shaders
            Material material = dfUnity.MaterialReader.GetMaterialAtlas(
                    archive,
                    0,
                    4,
                    2048,
                    out atlasRects,
                    out atlasIndices,
                    4,
                    true,
                    0,
                    false,
                    Shader.Find(dfUnity.MaterialReader.DefaultBillboardShaderName));

            // Cache size and scale for each record
            // This is required to properly calculate world size of billboard
            CachedMaterial cm;
            dfUnity.MaterialReader.GetCachedMaterialAtlas(archive, out cm);
            atlasSizes = cm.recordSizes;
            atlasScales = cm.recordScales;

            // Create material
            // TODO: This should be created by MaterialReader so identical materials will batch
            Shader shader = Shader.Find(MaterialReader._DaggerfallBillboardBatchShaderName);
            Material atlasMaterial = new Material(shader);
            atlasMaterial.mainTexture = material.mainTexture;

            // Assign renderer properties
            // Turning off receive shadows to prevent self-shadowing
            renderer.sharedMaterial = atlasMaterial;
            renderer.castShadows = CastShadows;
            renderer.receiveShadows = false;

            NatureMaterial = archive;
            currentArchive = archive;
        }

        /// <summary>
        /// Clear all billboards from batch.
        /// </summary>
        public void Clear()
        {
            billboardItems.Clear();
        }

        /// <summary>
        /// Add a billboard to batch.
        /// </summary>
        public void AddItem(int record, Vector3 localPosition)
        {
            // Limit maximum billboards in batch
            if (billboardItems.Count + 1 > maxBillboardCount)
            {
                DaggerfallUnity.LogMessage("DaggerfallBillboardBatch: Maximum batch size reached.", true);
                return;
            }

            // Add new billboard to batch
            BillboardItem bi = new BillboardItem()
            {
                record = record,
                position = origin + localPosition,
            };
            billboardItems.Add(bi);
        }

        /// <summary>
        /// Apply items to batch.
        /// Must be called once all items added.
        /// You can add more items later, but will need to apply again.
        /// </summary>
        public void Apply()
        {
            CreateMesh();
        }

        #region Editor Support

        public void __EditorClearBillboards()
        {
            Clear();
            Apply();
        }

        public void __EditorRandomLayout()
        {
            SetMaterial(NatureMaterial);

            Clear();
            float dist = RandomSpacing;
            for (int y = 0; y < RandomDepth; y++)
            {
                for (int x = 0; x < RandomWidth; x++)
                {
                    int record = UnityEngine.Random.Range(1, 32);
                    AddItem(record, new Vector3(x * dist, 0, y * dist));
                }
            }
            Apply();
        }

        #endregion

        #region Private Methods

        // Packs all billboards into single mesh
        private void CreateMesh()
        {
            const int vertsPerQuad = 4;
            const int indicesPerQuad = 6;

            // Using half way between forward and up for billboard normal
            // Workable for most lighting but will need a better system eventually
            Vector3 normalTemplate = Vector3.Normalize(Vector3.up + Vector3.forward);

            // Create billboard data
            Bounds newBounds = new Bounds();
            int vertexCount = billboardItems.Count * vertsPerQuad;
            int indexCount = billboardItems.Count * indicesPerQuad;
            Vector3[] vertices = new Vector3[vertexCount];
            Vector3[] normals = new Vector3[vertexCount];
            Vector4[] tangents = new Vector4[vertexCount];
            Vector2[] uv = new Vector2[vertexCount];
            int[] indices = new int[indexCount];
            int currentIndex = 0;
            for (int billboard = 0; billboard < billboardItems.Count; billboard++)
            {
                int offset = billboard * vertsPerQuad;
                BillboardItem bi = billboardItems[billboard];

                // Billboard size and origin
                Vector2 finalSize = GetScaledBillboardSize(bi.record);
                //float hx = (finalSize.x / 2);
                float hy = (finalSize.y / 2);
                Vector3 position = bi.position + new Vector3(0, hy, 0);

                // Billboard UVs
                Rect rect = atlasRects[atlasIndices[bi.record].startIndex];
                uv[offset] = new Vector2(rect.x, rect.yMax);
                uv[offset + 1] = new Vector2(rect.xMax, rect.yMax);
                uv[offset + 2] = new Vector2(rect.x, rect.y);
                uv[offset + 3] = new Vector2(rect.xMax, rect.y);

                // Tangent data for shader is used to size billboard
                tangents[offset] = new Vector4(finalSize.x, finalSize.y, 0, 1);
                tangents[offset + 1] = new Vector4(finalSize.x, finalSize.y, 1, 1);
                tangents[offset + 2] = new Vector4(finalSize.x, finalSize.y, 0, 0);
                tangents[offset + 3] = new Vector4(finalSize.x, finalSize.y, 1, 0);

                // Other data for shader
                for (int vertex = 0; vertex < vertsPerQuad; vertex++)
                {
                    vertices[offset + vertex] = position;
                    normals[offset + vertex] = normalTemplate;
                }

                // Assign index data
                indices[currentIndex] = offset;
                indices[currentIndex + 1] = offset + 1;
                indices[currentIndex + 2] = offset + 2;
                indices[currentIndex + 3] = offset + 3;
                indices[currentIndex + 4] = offset + 2;
                indices[currentIndex + 5] = offset + 1;
                currentIndex += indicesPerQuad;

                // Update bounds tracking using actual position and size
                // This can be a little wonky with single billboards side-on as AABB does not rotate
                // But it generally works well for large batches as intended
                // Multiply finalSize * 2f if culling problems with standalone billboards
                Bounds currentBounds = new Bounds(position, finalSize);
                newBounds.Encapsulate(currentBounds);
            }

            // Create mesh
            if (billboardMesh == null)
            {
                // New mesh
                billboardMesh = new Mesh();
                billboardMesh.name = "BillboardBatchMesh";
            }
            else
            {
                // Existing mesh
                if (billboardMesh.vertexCount == vertices.Length)
                    billboardMesh.Clear(true);      // Same vertex layout
                else
                    billboardMesh.Clear(false);     // New vertex layout
            }

            // Assign mesh data
            billboardMesh.vertices = vertices;              // Each vertex is positioned at billboard origin
            billboardMesh.tangents = tangents;              // Tangent stores corners and size
            billboardMesh.triangles = indices;              // Standard indices
            billboardMesh.normals = normals;                // Standard normals
            billboardMesh.uv = uv;                          // Standard uv coordinates into atlas

            // Manually update bounds to account for max billboard height
            billboardMesh.bounds = newBounds;

            // Assign mesh
            MeshFilter filter = GetComponent<MeshFilter>();
            filter.sharedMesh = billboardMesh;
        }

        // Gets scaled billboard size to properly size billboard in world
        private Vector2 GetScaledBillboardSize(int record)
        {
            // Get size and scale
            Vector2 size = atlasSizes[record];
            Vector2 scale = atlasScales[record];

            // Apply scale
            Vector2 finalSize;
            int xChange = (int)(size.x * (scale.x / BlocksFile.ScaleDivisor));
            int yChange = (int)(size.y * (scale.y / BlocksFile.ScaleDivisor));
            finalSize.x = (size.x + xChange);
            finalSize.y = (size.y + yChange);

            return finalSize * MeshReader.GlobalScale;
        }

        private bool ReadyCheck()
        {
            // Ensure we have a DaggerfallUnity reference
            if (dfUnity == null)
            {
                dfUnity = DaggerfallUnity.Instance;
            }

            // Do nothing if DaggerfallUnity not ready
            if (!dfUnity.IsReady)
            {
                DaggerfallUnity.LogMessage("DaggerfallBillboardBatch: DaggerfallUnity component is not ready. Have you set your Arena2 path?");
                return false;
            }

            return true;
        }

        #endregion
    }
}