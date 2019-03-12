// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2019 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using UnityEngine.Rendering;
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
    /// Draws a large number of atlased billboards using a single mesh and custom geometry shader.
    /// Supports animated billboards with a random start frame, but only one animation timer per batch.
    /// Currently used for exterior billboards only (origin = centre-bottom).
    /// Support for interior/dungeon billboards will be added later (origin = centre).
    /// Tries to not recreate Mesh and Material where possible.
    /// Generates some garbage when rebuilding mesh layout. This can probably be improved.
    /// </summary>
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class DaggerfallBillboardBatch : MonoBehaviour
    {
        // Maximum allowable billboards before mesh buffer overrun
        const int maxBillboardCount = 16250;

        [SerializeField, HideInInspector]
        Material customMaterial = null;
        [SerializeField, HideInInspector]
        CachedMaterial cachedMaterial;
        [SerializeField, HideInInspector]
        List<BillboardItem> billboardItems = new List<BillboardItem>();
        [SerializeField, HideInInspector]
        Mesh billboardMesh;
        [SerializeField, HideInInspector]
        Vector2[] uvs;

        [NonSerialized, HideInInspector]
        public Vector3 BlockOrigin = Vector3.zero;

        [Range(0, 511)]
        public int TextureArchive = 504;
        [Range(0, 30)]
        public float FramesPerSecond = 0;
        public bool RandomStartFrame = true;
        public ShadowCastingMode ShadowCasting = ShadowCastingMode.TwoSided;
        [Range(1, 127)]
        public int RandomWidth = 16;
        [Range(1, 127)]
        public int RandomDepth = 16;
        public float RandomSpacing = BlocksFile.TileDimension * MeshReader.GlobalScale;

        DaggerfallUnity dfUnity;
        int currentArchive = -1;
        float lastFramesPerSecond = 0;
        bool restartAnims = true;
        MeshRenderer meshRenderer;

        const int vertsPerQuad = 4;
        const int indicesPerQuad = 6;

        // Just using a simple animation speed for simple billboard anims
        // You can adjust this or extend as needed
        const int animalFps = 5;
        const int lightFps = 12;

        [Serializable]
        struct BillboardItem
        {
            public int record;                  // The texture record to display
            public Vector3 position;            // Position from origin to render billboard
            public int totalFrames;             // Total animation frames
            public int currentFrame;            // Current animation frame
            public Rect customRect;             // Rect for custom material path
            public Vector2 customSize;          // Size for custom material path
            public Vector2 customScale;         // Scale for custom material path
        }

        public bool IsCustom
        {
            get { return (customMaterial == null) ? false : true; }
        }

        void Start()
        {
        }

        void OnDisable()
        {
            restartAnims = true;
        }

        void Update()
        {
            // Stop coroutine if frames per second drops to 0
            if (FramesPerSecond == 0 && lastFramesPerSecond > 0)
                StopCoroutine(AnimateBillboards());
            else if (FramesPerSecond == 0 && lastFramesPerSecond == 0)
                restartAnims = true;

            // Store frames per second for this frame
            lastFramesPerSecond = FramesPerSecond;

            // Restart animation coroutine if not running and frames per second greater than 0
            if (restartAnims && cachedMaterial.key != 0 && FramesPerSecond > 0 && customMaterial == null)
            {
                StartCoroutine(AnimateBillboards());
                restartAnims = false;
            }
        }

        IEnumerator AnimateBillboards()
        {
            while (true)
            {
                // Tick animation when valid
                if (FramesPerSecond > 0 && cachedMaterial.key != 0 && customMaterial == null && uvs != null)
                {
                    // Look for animated billboards
                    for (int billboard = 0; billboard < billboardItems.Count; billboard++)
                    {
                        // Get billboard and do nothing if single frame
                        BillboardItem bi = billboardItems[billboard];
                        if (bi.totalFrames > 1)
                        {
                            // Increment current billboard frame
                            if (++bi.currentFrame >= bi.totalFrames)
                            {
                                bi.currentFrame = 0;
                            }
                            billboardItems[billboard] = bi;

                            // Set new UV properties based on current frame
                            Rect rect = cachedMaterial.atlasRects[cachedMaterial.atlasIndices[bi.record].startIndex + bi.currentFrame];
                            int offset = billboard * vertsPerQuad;
                            uvs[offset] = new Vector2(rect.x, rect.yMax);
                            uvs[offset + 1] = new Vector2(rect.xMax, rect.yMax);
                            uvs[offset + 2] = new Vector2(rect.x, rect.y);
                            uvs[offset + 3] = new Vector2(rect.xMax, rect.y);
                        }
                    }

                    // Store new mesh UV set
                    if (uvs != null && uvs.Length > 0)
                        billboardMesh.uv = uvs;
                }

                yield return new WaitForSeconds(1f / FramesPerSecond);
            }
        }

        /// <summary>
        /// Set material all billboards in this batch will share.
        /// This material is always atlased.
        /// </summary>
        /// <param name="archive">Archive index.</param>
        /// <param name="force">Force new archive, even if already set.</param>
        public void SetMaterial(int archive, bool force = false)
        {
            if (!ReadyCheck())
                return;

            // Do nothing if this archive already set
            if (archive == currentArchive && !force)
                return;

            // Get atlas size
            int size = DaggerfallUnity.Settings.AssetInjection ? 4096 : 2048;

            // Get standard atlas material
            // Just going to steal texture and settings
            // TODO: Revise material loading for custom shaders
            Rect[] atlasRects;
            RecordIndex[] atlasIndices;
            Material material = dfUnity.MaterialReader.GetMaterialAtlas(
                    archive,
                    0,
                    4,
                    size,
                    out atlasRects,
                    out atlasIndices,
                    4,
                    true,
                    0,
                    false,
                    true);

            // Serialize cached material information
            dfUnity.MaterialReader.GetCachedMaterialAtlas(archive, out cachedMaterial);

            // Steal textures from source material
            Texture albedoMap = material.mainTexture;
            Texture normalMap = material.GetTexture("_BumpMap");
            Texture emissionMap = material.GetTexture("_EmissionMap");

            // Create local material
            // TODO: This should be created by MaterialReader
            Shader shader = Shader.Find(MaterialReader._DaggerfallBillboardBatchShaderName);
            Material atlasMaterial = new Material(shader);
            atlasMaterial.mainTexture = albedoMap;

            // Assign other maps
            if (normalMap != null)
            {
                atlasMaterial.SetTexture("_BumpMap", normalMap);
                atlasMaterial.EnableKeyword("_NORMALMAP");
            }
            if (emissionMap != null)
            {
                atlasMaterial.SetTexture("_EmissionMap", emissionMap);
                atlasMaterial.SetColor("_EmissionColor", material.GetColor("_EmissionColor"));
                atlasMaterial.EnableKeyword("_EMISSION");
            }

            // Assign renderer properties
            // Turning off receive shadows to prevent self-shadowing
            meshRenderer.sharedMaterial = atlasMaterial;
            meshRenderer.receiveShadows = false;

            // Set shadow casting mode - force off for lights
            if (archive == Utility.TextureReader.LightsTextureArchive)
                meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
            else
                meshRenderer.shadowCastingMode = ShadowCasting;

            // Set animation speed for supported archives
            if (archive == Utility.TextureReader.AnimalsTextureArchive)
                FramesPerSecond = animalFps;
            else if (archive == Utility.TextureReader.LightsTextureArchive)
                FramesPerSecond = lightFps;
            else
                FramesPerSecond = 0;

            // Clear custom material
            customMaterial = null;

            TextureArchive = archive;
            currentArchive = archive;
        }

        /// <summary>
        /// Directly set custom atlas material all billboards in batch will share.
        /// Custom material allows you to directly set item rects in batch.
        /// </summary>
        /// <param name="material"></param>
        public void SetMaterial(Material material)
        {
            if (!ReadyCheck())
                return;

            // Custom material does not support animation for now
            customMaterial = material;

            // Create local material from source
            Shader shader = Shader.Find(MaterialReader._DaggerfallBillboardBatchShaderName);
            Material atlasMaterial = new Material(shader);
            atlasMaterial.mainTexture = customMaterial.mainTexture;

            // Assign renderer properties
            meshRenderer.sharedMaterial = atlasMaterial;
            meshRenderer.receiveShadows = false;
            FramesPerSecond = 0;
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
            // Cannot use with a custom material
            if (customMaterial != null)
                throw new Exception("Cannot use with custom material. Use AddItem(Rect rect, Vector2 size, Vector2 scale, Vector3 localPosition) overload instead.");

            // Must have set a material
            if (cachedMaterial.key == 0)
            {
                DaggerfallUnity.LogMessage("DaggerfallBillboardBatch: Must call SetMaterial() before adding items.", true);
                return;
            }

            // Limit maximum billboards in batch
            if (billboardItems.Count + 1 > maxBillboardCount)
            {
                DaggerfallUnity.LogMessage("DaggerfallBillboardBatch: Maximum batch size reached.", true);
                return;
            }

            // Get frame count and start frame
            int frameCount = cachedMaterial.atlasFrameCounts[record];
            int startFrame = 0;
            if (RandomStartFrame)
                startFrame = UnityEngine.Random.Range(0, frameCount);

            // Add new billboard to batch
            BillboardItem bi = new BillboardItem()
            {
                record = record,
                position = BlockOrigin + localPosition,
                totalFrames = frameCount,
                currentFrame = startFrame,
            };
            billboardItems.Add(bi);
        }

        /// <summary>
        /// Add a billboard to batch.
        /// Use this overload for custom atlas material.
        /// </summary>
        public void AddItem(Rect rect, Vector2 size, Vector2 scale, Vector3 localPosition)
        {
            // Cannot use with auto material
            if (customMaterial == null)
                throw new Exception("Cannot use with auto material. Use AddItem(int record, Vector3 localPosition) overload instead.");

            // Add new billboard to batch
            BillboardItem bi = new BillboardItem()
            {
                position = BlockOrigin + localPosition,
                customRect = rect,
                customSize = size,
                customScale = scale,
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
            // Apply material
            if (customMaterial != null)
                CreateMeshForCustomMaterial();
            else
                CreateMesh();
           
            // Update name
            UpdateName();
        }

        #region Editor Support

        public void __EditorClearBillboards()
        {
            Clear();
            Apply();
        }

        public void __EditorRandomLayout()
        {
            SetMaterial(TextureArchive, true);
            Clear();

            // Set min record - nature flats will ignore marker index 0
            int minRecord = (TextureArchive < 500) ? 0 : 1;
            int maxRecord = cachedMaterial.atlasIndices.Length;

            float dist = RandomSpacing;
            for (int y = 0; y < RandomDepth; y++)
            {
                for (int x = 0; x < RandomWidth; x++)
                {
                    int record = UnityEngine.Random.Range(minRecord, maxRecord);
                    AddItem(record, new Vector3(x * dist, 0, y * dist));
                }
            }
            Apply();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// TEMP: Create mesh for custom material path.
        /// This can be improved as it's mostly the same as CreateMesh().
        /// Keeping separate for now until super-atlases are better integrated.
        /// </summary>
        private void CreateMeshForCustomMaterial()
        {
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
            uvs = new Vector2[vertexCount];
            int[] indices = new int[indexCount];
            int currentIndex = 0;
            for (int billboard = 0; billboard < billboardItems.Count; billboard++)
            {
                int offset = billboard * vertsPerQuad;
                BillboardItem bi = billboardItems[billboard];

                // Billboard size and origin
                Vector2 finalSize = GetScaledBillboardSize(bi.customSize, bi.customScale);
                float hy = (finalSize.y / 2);
                Vector3 position = bi.position + new Vector3(0, hy, 0);

                // Billboard UVs
                Rect rect = bi.customRect;
                uvs[offset] = new Vector2(rect.x, rect.yMax);
                uvs[offset + 1] = new Vector2(rect.xMax, rect.yMax);
                uvs[offset + 2] = new Vector2(rect.x, rect.y);
                uvs[offset + 3] = new Vector2(rect.xMax, rect.y);

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
                billboardMesh.name = "BillboardBatchMesh [CustomPath]";
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
            billboardMesh.uv = uvs;                         // Standard uv coordinates into atlas

            // Manually update bounds to account for max billboard height
            billboardMesh.bounds = newBounds;

            // Assign mesh
            MeshFilter filter = GetComponent<MeshFilter>();
            filter.sharedMesh = billboardMesh;
        }

        // Packs all billboards into single mesh
        private void CreateMesh()
        {
            // Using half way between forward and up for billboard normal
            // Workable for most lighting but will need a better system eventually
            Vector3 normalTemplate = Vector3.Normalize(Vector3.up + Vector3.forward);

            // Create billboard data
            // Serializing UV array creates less garbage than recreating every time animation ticks
            Bounds newBounds = new Bounds();
            int vertexCount = billboardItems.Count * vertsPerQuad;
            int indexCount = billboardItems.Count * indicesPerQuad;
            Vector3[] vertices = new Vector3[vertexCount];
            Vector3[] normals = new Vector3[vertexCount];
            Vector4[] tangents = new Vector4[vertexCount];
            uvs = new Vector2[vertexCount];
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
                Rect rect = cachedMaterial.atlasRects[cachedMaterial.atlasIndices[bi.record].startIndex + bi.currentFrame];
                uvs[offset] = new Vector2(rect.x, rect.yMax);
                uvs[offset + 1] = new Vector2(rect.xMax, rect.yMax);
                uvs[offset + 2] = new Vector2(rect.x, rect.y);
                uvs[offset + 3] = new Vector2(rect.xMax, rect.y);

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
            billboardMesh.uv = uvs;                         // Standard uv coordinates into atlas

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
            Vector2 size = cachedMaterial.recordSizes[record];
            Vector2 scale = cachedMaterial.recordScales[record];

            return GetScaledBillboardSize(size, scale);
        }

        // Gets scaled billboard size to properly size billboard in world
        private Vector2 GetScaledBillboardSize(Vector2 size, Vector2 scale)
        {
            // Apply scale
            Vector2 finalSize;
            int xChange = (int)(size.x * (scale.x / BlocksFile.ScaleDivisor));
            int yChange = (int)(size.y * (scale.y / BlocksFile.ScaleDivisor));
            finalSize.x = (size.x + xChange);
            finalSize.y = (size.y + yChange);

            return finalSize * MeshReader.GlobalScale;
        }

        /// <summary>
        /// Apply new name based on archive index.
        /// </summary>
        private void UpdateName()
        {
            if (customMaterial != null)
                this.name = "DaggerfallBillboardBatch [CustomMaterial]";
            else
                this.name = string.Format("DaggerfallBillboardBatch [{0}]", TextureArchive);
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

            // Save references
            meshRenderer = GetComponent<MeshRenderer>();

            return true;
        }

        #endregion
    }
}
