// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    Andrzej Łukasik (andrew.r.lukasik)
// 
// Notes:
//

using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Assertions;
using Unity.Profiling;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
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
        Mesh billboardMesh;

        byte isUnmanagedMemoryAllocated;// 0 - not allocated or disposed, 1 - allocated
        NativeList<BillboardItem> billboardData;
        NativeArray<float3> meshVertices;
        NativeArray<float3> meshNormals;
        NativeArray<ushort> meshIndices;
        NativeArray<float4> meshTangents;
        NativeArray<float2> meshUVs;
        NativeArray<Bounds> meshAABB;
        JobHandle Dependency;
        JobHandle UvAnimationDependency;

        [System.NonSerialized, HideInInspector]
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

        [System.Serializable]
        struct BillboardItem
        {
            public int record;                  // The texture record to display
            public float3 position;            // Position from origin to render billboard
            public int totalFrames;             // Total animation frames
            public int currentFrame;            // Current animation frame
            public Rect customRect;             // Rect for custom material path
            public float2 customSize;          // Size for custom material path
            public float2 customScale;         // Scale for custom material path
        }

        public bool IsCustom
        {
            get { return (customMaterial == null) ? false : true; }
        }

        #region Profiler Markers

        static readonly ProfilerMarker
            ___tick = new ProfilerMarker("tick animation"),
            ___schedule = new ProfilerMarker("schedule"),
            ___complete = new ProfilerMarker("complete"),
            ___setUVs = new ProfilerMarker("set uv"),
            ___getMaterialAtlas = new ProfilerMarker("get material atlas"),
            ___getCachedMaterialAtlas = new ProfilerMarker("get cached material atlas"),
            ___assignOtherMaps = new ProfilerMarker("assign other maps"),
            ___stealTextureFromSourceMaterial = new ProfilerMarker("steal texture from source material"),
            ___createLocalMaterial = new ProfilerMarker("create local material"),
            ___createMeshForCustomMaterial = new ProfilerMarker("create mesh for custom material"),
            ___createMesh = new ProfilerMarker("create mesh"),
            ___newMesh = new ProfilerMarker("new mesh"),
            ___reuseMesh = new ProfilerMarker("reuse mesh"),
            ___assignMesh = new ProfilerMarker("assign mesh"),
            ___assignMeshData = new ProfilerMarker("push mesh data"),
            ___indexBuffer = new ProfilerMarker("index buffer"),
            ___vertexBuffer = new ProfilerMarker("vertex buffer"),
            ___SetMaterial = new ProfilerMarker($"{nameof(DaggerfallBillboardBatch)}.{nameof(SetMaterial)}"),
            ___AddItem = new ProfilerMarker($"{nameof(DaggerfallBillboardBatch)}.{nameof(AddItem)}"),
            ___AddItemsAsync = new ProfilerMarker($"{nameof(DaggerfallBillboardBatch)}.{nameof(AddItemsAsync)}"),
            ___Apply = new ProfilerMarker($"{nameof(DaggerfallBillboardBatch)}.{nameof(Apply)}"),
            ___Clear = new ProfilerMarker($"{nameof(DaggerfallBillboardBatch)}.{nameof(Clear)}"),
            ___CreateMeshForCustomMaterial = new ProfilerMarker($"{nameof(DaggerfallBillboardBatch)}.{nameof(CreateMeshForCustomMaterial)}"),
            ___ResizeMeshBuffers = new ProfilerMarker($"{nameof(DaggerfallBillboardBatch)}.{nameof(ResizeMeshBuffers)}"),
            ___CreateMesh = new ProfilerMarker($"{nameof(DaggerfallBillboardBatch)}.{nameof(CreateMesh)}"),
            ___PushNewMeshData = new ProfilerMarker($"{nameof(DaggerfallBillboardBatch)}.{nameof(PushNewMeshData)}"),
            ___PushUVData = new ProfilerMarker($"{nameof(DaggerfallBillboardBatch)}.{nameof(PushUVData)}"),
            ___AllocateUnmanagedMemory = new ProfilerMarker($"{nameof(DaggerfallBillboardBatch)}.{nameof(AllocateUnmanagedMemory)}"),
            ___DeallocateUnmanagedMemory = new ProfilerMarker($"{nameof(DaggerfallBillboardBatch)}.{nameof(DeallocateUnmanagedMemory)}");

        #endregion

#if UNITY_EDITOR
        // fixes an editor-only issue #2455 https://github.com/Interkarma/daggerfall-unity/issues/2455
        DaggerfallBillboardBatch() => UnityEditor.EditorApplication.playModeStateChanged += OnEditorPlayModeStateChanged;
        ~DaggerfallBillboardBatch() => UnityEditor.EditorApplication.playModeStateChanged -= OnEditorPlayModeStateChanged;
        void OnEditorPlayModeStateChanged(UnityEditor.PlayModeStateChange stateChange)
        {
            if (stateChange == UnityEditor.PlayModeStateChange.ExitingPlayMode)
                OnDestroy();
        }
#endif

        void Awake()
        {
            if (isUnmanagedMemoryAllocated == 0) AllocateUnmanagedMemory();
        }

        void OnDestroy()
        {
            // make sure there are no unfinished jobs:
            Dependency.Complete();
            UvAnimationDependency.Complete();

            if (isUnmanagedMemoryAllocated == 1) DeallocateUnmanagedMemory();
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
            float framesPerSecondInUse = FramesPerSecond;
            WaitForSeconds wait = new WaitForSeconds(1f / FramesPerSecond);// reuse

            while (true)
            {
                if (FramesPerSecond > framesPerSecondInUse || FramesPerSecond < framesPerSecondInUse)
                {
                    framesPerSecondInUse = FramesPerSecond;
                    wait = new WaitForSeconds(1f / framesPerSecondInUse);
                }

                // Tick animation when valid
                ___tick.Begin();
                int numBillboardsToAnimate = meshUVs.Length / vertsPerQuad;
                if (
                        FramesPerSecond > 0
                    && cachedMaterial.key != 0
                    && customMaterial == null
                    && numBillboardsToAnimate != 0
                )
                {
                    // schedule jobs:
                    ___schedule.Begin();
                    AnimateUVJob animateUVJob = new AnimateUVJob
                    {
                        atlasRects = new NativeArray<Rect>(cachedMaterial.atlasRects, Allocator.TempJob),
                        atlasIndices = new NativeArray<RecordIndex>(cachedMaterial.atlasIndices, Allocator.TempJob),
                        billboards = billboardData,
                        uv = meshUVs,
                    };
                    UvAnimationDependency = animateUVJob.Schedule(numBillboardsToAnimate, 128, Dependency);
                    ___schedule.End();

                    // delay finalization not to stall this thread:
                    Invoke(nameof(PushUVData), 0);
                }
                ___tick.End();

                yield return wait;
            }
        }

        void AllocateUnmanagedMemory()
        {
            ___AllocateUnmanagedMemory.Begin();

            Assert.AreEqual(expected: (byte)0, actual: isUnmanagedMemoryAllocated);

            billboardData = new NativeList<BillboardItem>(initialCapacity: maxBillboardCount, Allocator.Persistent);
            meshVertices = new NativeArray<float3>(0, Allocator.Persistent);
            meshNormals = new NativeArray<float3>(0, Allocator.Persistent);
            meshIndices = new NativeArray<ushort>(0, Allocator.Persistent);
            meshTangents = new NativeArray<float4>(0, Allocator.Persistent);
            meshUVs = new NativeArray<float2>(0, Allocator.Persistent);
            meshAABB = new NativeArray<Bounds>(1, Allocator.Persistent);

            isUnmanagedMemoryAllocated = 1;
            
            ___AllocateUnmanagedMemory.End();
        }

        void DeallocateUnmanagedMemory()
        {
            ___DeallocateUnmanagedMemory.Begin();

            Assert.AreEqual(expected: (byte)1, actual: isUnmanagedMemoryAllocated);

            billboardData.Dispose();
            meshVertices.Dispose();
            meshNormals.Dispose();
            meshIndices.Dispose();
            meshTangents.Dispose();
            meshUVs.Dispose();
            meshAABB.Dispose();

            isUnmanagedMemoryAllocated = 0;

            ___DeallocateUnmanagedMemory.End();
        }

        /// <summary>
        /// Set material all billboards in this batch will share.
        /// This material is always atlased.
        /// </summary>
        /// <param name="archive">Archive index.</param>
        /// <param name="force">Force new archive, even if already set.</param>
        public void SetMaterial(int archive, bool force = false)
        {
            ___SetMaterial.Begin();

            if (!ReadyCheck())
                return;

            // Do nothing if this archive already set
            if (archive == currentArchive && !force)
                return;

            // Get atlas size
            int size = DaggerfallUnity.Settings.AssetInjection ? 4096 : 2048;

            // Get standard atlas material
            ___getMaterialAtlas.Begin();
            // Just going to steal texture and settings
            // TODO: Revise material loading for custom shaders
            Material material = dfUnity.MaterialReader.GetMaterialAtlas(
                archive, 0, 4, size,
                out Rect[] atlasRects, out RecordIndex[] atlasIndices,
                4, true, 0, false, true
            );
            ___getMaterialAtlas.End();

            // Serialize cached material information
            ___getCachedMaterialAtlas.Begin();
            dfUnity.MaterialReader.GetCachedMaterialAtlas(archive, out cachedMaterial);
            ___getCachedMaterialAtlas.End();

            // Steal textures from source material
            ___stealTextureFromSourceMaterial.Begin();
            Texture albedoMap = material.mainTexture;
            Texture normalMap = material.GetTexture(Uniforms.BumpMap);
            Texture emissionMap = material.GetTexture(Uniforms.EmissionMap);
            ___stealTextureFromSourceMaterial.End();

            // Create local material
            ___createLocalMaterial.Begin();
            // TODO: This should be created by MaterialReader
            Shader shader = (DaggerfallUnity.Settings.NatureBillboardShadows) ?
                Shader.Find(MaterialReader._DaggerfallBillboardBatchShaderName) :
                Shader.Find(MaterialReader._DaggerfallBillboardBatchNoShadowsShaderName);
            Material atlasMaterial = new Material(shader);
            atlasMaterial.mainTexture = albedoMap;
            ___createLocalMaterial.End();

            // Assign other maps
            ___assignOtherMaps.Begin();
            if (normalMap != null)
            {
                atlasMaterial.SetTexture(Uniforms.BumpMap, normalMap);
                atlasMaterial.EnableKeyword(KeyWords.NormalMap);
            }
            if (emissionMap != null)
            {
                atlasMaterial.SetTexture(Uniforms.EmissionMap, emissionMap);
                atlasMaterial.SetColor(Uniforms.EmissionColor, material.GetColor(Uniforms.EmissionColor));
                atlasMaterial.EnableKeyword(KeyWords.Emission);
            }
            ___assignOtherMaps.End();

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

            ___SetMaterial.End();
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
            Shader shader = (DaggerfallUnity.Settings.NatureBillboardShadows) ?
                Shader.Find(MaterialReader._DaggerfallBillboardBatchShaderName) :
                Shader.Find(MaterialReader._DaggerfallBillboardBatchNoShadowsShaderName);
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
            ___Clear.Begin();

            if (isUnmanagedMemoryAllocated == 1)
            {
                Dependency.Complete();// make sure there are no unfinished jobs
                billboardData.Clear();
            }
            else
            {
                // no data to clear
            }

            ___Clear.End();
        }

        /// <summary>
        /// Add a billboard to batch.
        /// </summary>
        [System.Obsolete("Use " + nameof(AddItemsAsync) + " instead. Reason: billboards are never added in amount of one.")]
        public void AddItem(BasicInfo item)
        {
            AddItem(item.textureRecord, item.localPosition);
        }
        /// <inheritdoc />
        [System.Obsolete("Use " + nameof(AddItemsAsync) + " instead. Reason: billboards are never added in amount of one.")]
        public void AddItem(int record, Vector3 localPosition)
        {
            ___AddItem.Begin();

            Dependency.Complete();// make sure there are no unfinished jobs

            // make sure unmanaged memory is allocated
            if (isUnmanagedMemoryAllocated == 0) AllocateUnmanagedMemory();

            // Cannot use with a custom material
            if (customMaterial != null)
            {
                ___AddItem.End();
                throw new System.Exception("Cannot use with custom material. Use AddItem(Rect rect, Vector2 size, Vector2 scale, Vector3 localPosition) overload instead.");
            }

            // Must have set a material
            if (cachedMaterial.key == 0)
            {
                DaggerfallUnity.LogMessage("DaggerfallBillboardBatch: Must call SetMaterial() before adding items.", true);
                ___AddItem.End();
                return;
            }

            // Limit maximum billboards in batch
            if (billboardData.Length + 1 > maxBillboardCount)
            {
                DaggerfallUnity.LogMessage("DaggerfallBillboardBatch: Maximum batch size reached.", true);
                ___AddItem.End();
                return;
            }

            // Get frame count and start frame
            int frameCount = cachedMaterial.atlasFrameCounts[record];
            int startFrame = 0;
            if (RandomStartFrame)
                startFrame = UnityEngine.Random.Range(0, frameCount);

            // Add new billboard to batch
            BillboardItem billboard = new BillboardItem
            {
                record = record,
                position = BlockOrigin + localPosition,
                totalFrames = frameCount,
                currentFrame = startFrame,
            };
            billboardData.Add(billboard);

            ___AddItem.End();
        }

        /// <summary>
        /// Schedules a job that queues an array of billboard items.
        /// Call <see cref="Apply"/> once there is no more entries to add.
        /// </summary>
        public JobHandle AddItemsAsync(NativeArray<BasicInfo> items, JobHandle dependency = default)
        {
            ___AddItemsAsync.Begin();

            // Cannot use with a custom material
            if (customMaterial != null)
            {
                ___AddItemsAsync.End();
                throw new System.Exception("Cannot use with custom material. Use AddItem(Rect rect, Vector2 size, Vector2 scale, Vector3 localPosition) overload instead.");
            }

            // Must have set a material
            if (cachedMaterial.key == 0)
            {
                DaggerfallUnity.LogMessage("DaggerfallBillboardBatch: Must call SetMaterial() before adding items.", true);
                ___AddItemsAsync.End();
                return default;
            }

            // make sure unmanaged memory is allocated
            if (isUnmanagedMemoryAllocated == 0) AllocateUnmanagedMemory();

            // Limit maximum billboards in batch
            int available = maxBillboardCount - billboardData.Length;
            int numItemsToAdd = math.min(available, items.Length);
            if (numItemsToAdd != 0)
            {
                ___schedule.Begin();
                AddItemsJob job = new AddItemsJob
                {
                    source = items,
                    atlasFrameCounts = new NativeArray<int>(cachedMaterial.atlasFrameCounts, Allocator.TempJob),
                    randomStartFrame = RandomStartFrame,
                    seed = (uint)((System.Environment.TickCount * this.GetHashCode()).GetHashCode()),
                    blockOrigin = BlockOrigin,
                    billboardItems = billboardData.AsParallelWriter(),
                };
                Dependency = job.Schedule(arrayLength: numItemsToAdd, innerloopBatchCount: 128, dependsOn: JobHandle.CombineDependencies(Dependency, dependency));
                ___schedule.End();
            }

            if (billboardData.Length == maxBillboardCount)
                DaggerfallUnity.LogMessage("DaggerfallBillboardBatch: Maximum batch size reached.", true);

            ___AddItemsAsync.End();
            return Dependency;
        }
        /// <inheritdoc />
        public JobHandle AddItemsAsync(BasicInfo[] items, JobHandle dependency = default)
        {
            ___AddItemsAsync.Begin();

            NativeArray<BasicInfo> data = new NativeArray<BasicInfo>(items, Allocator.TempJob);
            JobHandle op = AddItemsAsync(data, dependency);
            new DeallocateArrayJob<BasicInfo>(data).Schedule(op);

            ___AddItemsAsync.End();
            return op;
        }

        /// <summary>
        /// Add a billboard to batch.
        /// Use this overload for custom atlas material.
        /// </summary>
        [System.Obsolete("Use " + nameof(AddItemsAsync) + " instead. Reason: billboards are never added in amount of one.")]
        public void AddItem(CustomInfo item)
        {
            AddItem(item.rect, item.size, item.scale, item.localPosition);
        }
        /// <inheritdoc />
        [System.Obsolete("Use " + nameof(AddItemsAsync) + " instead. Reason: billboards are never added in amount of one.")]
        public void AddItem(Rect rect, Vector2 size, Vector2 scale, Vector3 localPosition)
        {
            ___AddItem.Begin();

            Dependency.Complete();// make sure there are no unfinished jobs
            
            // make sure unmanaged memory is allocated
            if (isUnmanagedMemoryAllocated == 0) AllocateUnmanagedMemory();

            // Cannot use with auto material
            if (customMaterial == null)
                throw new System.Exception("Cannot use with auto material. Use AddItem(int record, Vector3 localPosition) overload instead.");

            // Add new billboard to batch
            BillboardItem billboard = new BillboardItem
            {
                position = BlockOrigin + localPosition,
                customRect = rect,
                customSize = size,
                customScale = scale,
            };
            billboardData.Add(billboard);

            ___AddItem.End();
        }

        /// <summary>
        /// Schedules a job that queues an array of billboard items.
        /// Call <see cref="Apply"/> once there is no more entries to add.
        /// </summary>
        public JobHandle AddItemsAsync(NativeArray<CustomInfo> items, JobHandle dependency = default)
        {
            ___AddItemsAsync.Begin();

            // Cannot use with auto material
            if (customMaterial == null)
            {
                ___AddItemsAsync.End();
                throw new System.Exception("Cannot use with auto material. Use AddItem(int record, Vector3 localPosition) overload instead.");
            }

            // make sure unmanaged memory is allocated
            if (isUnmanagedMemoryAllocated == 0) AllocateUnmanagedMemory();
            
            ___schedule.Begin();
            AddCustomItemsJob job = new AddCustomItemsJob
            {
                source = items,
                blockOrigin = BlockOrigin,
                billboardItems = billboardData.AsParallelWriter(),
            };
            JobHandle jobHandle = job.Schedule(items.Length, items.Length, JobHandle.CombineDependencies(Dependency, dependency));
            ___schedule.End();

            ___AddItemsAsync.End();
            return jobHandle;
        }
        /// <inheritdoc />
        public JobHandle AddItemsAsync(CustomInfo[] items, JobHandle dependency = default)
        {
            ___AddItemsAsync.Begin();

            NativeArray<CustomInfo> data = new NativeArray<CustomInfo>(items, Allocator.TempJob);
            JobHandle op = AddItemsAsync(data, dependency);
            new DeallocateArrayJob<CustomInfo>(data).Schedule(op);

            ___AddItemsAsync.End();
            return op;
        }

        /// <summary>
        /// Apply items to batch.
        /// Must be called once all items added.
        /// You can add more items later, but will need to apply again.
        /// </summary>
        public void Apply()
        {
            ___Apply.Begin();

            // Apply material
            if (customMaterial != null)
            {
                ___createMeshForCustomMaterial.Begin();
                CreateMeshForCustomMaterial();
                ___createMeshForCustomMaterial.End();
            }
            else
            {
                ___createMesh.Begin();
                CreateMesh();
                ___createMesh.End();
            }

            // Update name
            UpdateName();

            ___Apply.End();
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

            NativeArray<BasicInfo> items = new NativeArray<BasicInfo>(RandomDepth * RandomWidth, Allocator.TempJob);
            float dist = RandomSpacing;
            int i = 0;
            for (int y = 0; y < RandomDepth; y++)
            for (int x = 0; x < RandomWidth; x++)
            {
                int record = UnityEngine.Random.Range(minRecord, maxRecord);
                float3 localPosition = new float3(x * dist, 0, y * dist);
                items[i++] = new BasicInfo(record, localPosition);
            }
            JobHandle op = AddItemsAsync(items);
            op.Complete();
            Apply();
        }

        #endregion

        #region Private Methods


        /// <summary> Resizes when it's size is invalid. </summary>
        void ResizeMeshBuffers()
        {
            ___ResizeMeshBuffers.Begin();

            int numBillboards = billboardData.Length;
            int numVertices = numBillboards * vertsPerQuad;
            int numIndices = numBillboards * indicesPerQuad;
            if (meshVertices.Length != numVertices)
            {
                new DeallocateArrayJob<float3>(meshVertices).Schedule(Dependency);
                meshVertices = new NativeArray<float3>(numVertices, Allocator.Persistent);
            }
            if (meshNormals.Length != numVertices)
            {
                new DeallocateArrayJob<float3>(meshNormals).Schedule(Dependency);
                meshNormals = new NativeArray<float3>(numVertices, Allocator.Persistent);
            }
            if (meshIndices.Length != numIndices)
            {
                new DeallocateArrayJob<ushort>(meshIndices).Schedule(Dependency);
                meshIndices = new NativeArray<ushort>(numIndices, Allocator.Persistent);
            }
            if (meshTangents.Length != numVertices)
            {
                new DeallocateArrayJob<float4>(meshTangents).Schedule(Dependency);
                meshTangents = new NativeArray<float4>(numVertices, Allocator.Persistent);
            }
            if (meshUVs.Length != numVertices)
            {
                new DeallocateArrayJob<float2>(meshUVs).Schedule(Dependency);
                meshUVs = new NativeArray<float2>(numVertices, Allocator.Persistent);
            }

            ___ResizeMeshBuffers.End();
        }

        /// <summary>
        /// TEMP: Create mesh for custom material path.
        /// This can be improved as it's mostly the same as CreateMesh().
        /// Keeping separate for now until super-atlases are better integrated.
        /// </summary>
        private void CreateMeshForCustomMaterial()
        {
            ___CreateMeshForCustomMaterial.Begin();

            ___complete.Begin();
            Dependency.Complete();// make sure there are no unfinished jobs
            ___complete.End();

            // make sure unmanaged memory is allocated
            if (isUnmanagedMemoryAllocated == 0) AllocateUnmanagedMemory();

            // Create billboard data
            ___schedule.Begin();
            ResizeMeshBuffers();
            int numBillboards = billboardData.Length;
            NativeArray<float3> origins = new NativeArray<float3>(numBillboards, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
            NativeArray<float2> sizes = new NativeArray<float2>(numBillboards, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);

            JobHandle getCustomBatchDataJobHandle = new GetCustomMaterialBatchDataJob
            {
                billboards = billboardData,
                atlasRects = new NativeArray<Rect>(cachedMaterial.atlasRects, Allocator.TempJob),
                atlasIndices = new NativeArray<RecordIndex>(cachedMaterial.atlasIndices, Allocator.TempJob),
                origin = origins,
                size = sizes,
            }.Schedule(numBillboards, 128, Dependency);

            JobHandle boundsJobHandle = new BoundsJob
            {
                numBillboards = numBillboards,
                origin = origins,
                size = sizes,
                aabb = meshAABB
            }.Schedule(getCustomBatchDataJobHandle);

            JobHandle vertexJobHandle = new VertexJob
            {
                origin = origins,
                vertex = meshVertices,
            }.Schedule(numBillboards, 128, getCustomBatchDataJobHandle);

            JobHandle uvJobHandle = new CustomRectUVJob
            {
                billboards = billboardData,
                uv = meshUVs,
            }.Schedule(numBillboards, 128, getCustomBatchDataJobHandle);

            JobHandle tangentJobHandle = new TangentJob
            {
                size = sizes,
                tangent = meshTangents,
            }.Schedule(numBillboards, 128, getCustomBatchDataJobHandle);

            JobHandle indicesJobHandle = new Indices16Job
            {
                indices = meshIndices,
            }.Schedule(numBillboards, 128, Dependency);

            JobHandle normalsJobHandle = new NormalsJob
            {
                normal = meshNormals
            }.Schedule(meshNormals.Length, meshNormals.Length / SystemInfo.processorCount * 4, Dependency);

            NativeArray<JobHandle> handles = new NativeArray<JobHandle>(6, Allocator.Temp);
            handles[0] = vertexJobHandle;
            handles[1] = normalsJobHandle;
            handles[2] = indicesJobHandle;
            handles[3] = tangentJobHandle;
            handles[4] = uvJobHandle;
            handles[5] = boundsJobHandle;
            Dependency = JobHandle.CombineDependencies(handles);

            // deallocate leftovers:
            new DeallocateArrayJob<float3>(origins).Schedule(Dependency);
            new DeallocateArrayJob<float2>(sizes).Schedule(Dependency);
            ___schedule.End();

            // Create mesh
            if (billboardMesh == null)
            {
                // New mesh
                ___newMesh.Begin();
                billboardMesh = new Mesh();
                billboardMesh.name = "BillboardBatchMesh [CustomPath]";
                ___newMesh.End();
            }
            else
            {
                ___reuseMesh.Begin();
                billboardMesh.Clear(keepVertexLayout: billboardMesh.vertexCount == meshVertices.Length);
                ___reuseMesh.End();
            }

            // Assign mesh
            ___assignMesh.Begin();
            MeshFilter mf = GetComponent<MeshFilter>();
            mf.sharedMesh = billboardMesh;
            ___assignMesh.End();

            // delay finalization until the end of the current frame:
            Invoke(nameof(PushNewMeshData), 0);

            ___CreateMeshForCustomMaterial.End();
        }

        // Packs all billboards into single mesh
        private void CreateMesh()
        {
            ___CreateMesh.Begin();

            ___complete.Begin();
            Dependency.Complete();// make sure there are no unfinished jobs
            ___complete.End();

            // make sure unmanaged memory is allocated
            if (isUnmanagedMemoryAllocated == 0) AllocateUnmanagedMemory();

            // Create billboard data
            ___schedule.Begin();
            ResizeMeshBuffers();
            int numBillboards = billboardData.Length;
            NativeArray<float3> origins = new NativeArray<float3>(numBillboards, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
            NativeArray<float2> sizes = new NativeArray<float2>(numBillboards, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
            NativeArray<Rect> uvrects = new NativeArray<Rect>(numBillboards, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);

            GetBatchDataJob getBatchDataJob = new GetBatchDataJob
            {
                billboards = billboardData,
                recordSize = new NativeArray<Vector2>(cachedMaterial.recordSizes, Allocator.TempJob).Reinterpret<float2>(),
                recordScale = new NativeArray<Vector2>(cachedMaterial.recordScales, Allocator.TempJob).Reinterpret<float2>(),
                atlasRects = new NativeArray<Rect>(cachedMaterial.atlasRects, Allocator.TempJob),
                atlasIndices = new NativeArray<RecordIndex>(cachedMaterial.atlasIndices, Allocator.TempJob),
                scaleDivisor = BlocksFile.ScaleDivisor,

                origin = origins,
                size = sizes,
                uvRect = uvrects,
            };
            JobHandle getBatchDataJobHandle = getBatchDataJob.Schedule(numBillboards, 128, Dependency);

            BoundsJob boundsJob = new BoundsJob
            {
                numBillboards = numBillboards,
                origin = origins,
                size = sizes,
                aabb = meshAABB
            };
            JobHandle boundsJobHandle = boundsJob.Schedule(getBatchDataJobHandle);

            VertexJob vertexJob = new VertexJob
            {
                origin = origins,
                vertex = meshVertices,
            };
            JobHandle vertexJobHandle = vertexJob.Schedule(numBillboards, 128, getBatchDataJobHandle);

            UVJob uvJob = new UVJob
            {
                uvRect = uvrects,
                uv = meshUVs,
            };
            JobHandle uvJobHandle = uvJob.Schedule(numBillboards, 128, getBatchDataJobHandle);

            TangentJob tangentJob = new TangentJob
            {
                size = sizes,
                tangent = meshTangents,
            };
            JobHandle tangentJobHandle = tangentJob.Schedule(numBillboards, 128, getBatchDataJobHandle);

            Indices16Job indicesJob = new Indices16Job
            {
                indices = meshIndices,
            };
            JobHandle indicesJobHandle = indicesJob.Schedule(numBillboards, 128, Dependency);

            NormalsJob normalsJob = new NormalsJob
            {
                normal = meshNormals
            };
            JobHandle normalsJobHandle = normalsJob.Schedule(meshNormals.Length, meshNormals.Length / SystemInfo.processorCount * 4, Dependency);

            NativeArray<JobHandle> handles = new NativeArray<JobHandle>(6, Allocator.Temp);
            handles[0] = vertexJobHandle;
            handles[1] = normalsJobHandle;
            handles[2] = indicesJobHandle;
            handles[3] = tangentJobHandle;
            handles[4] = uvJobHandle;
            handles[5] = boundsJobHandle;
            Dependency = JobHandle.CombineDependencies(handles);

            // deallocate leftovers:
            new DeallocateArrayJob<float3>(origins).Schedule(Dependency);
            new DeallocateArrayJob<float2>(sizes).Schedule(Dependency);
            new DeallocateArrayJob<Rect>(uvrects).Schedule(Dependency);
            ___schedule.End();

            // Create mesh
            ___createMesh.Begin();
            if (billboardMesh == null)
            {
                // New mesh
                billboardMesh = new Mesh();
                billboardMesh.name = "BillboardBatchMesh";
            }
            else
            {
                // Existing mesh
                billboardMesh.Clear(keepVertexLayout: billboardMesh.vertexCount == meshVertices.Length);
            }
            ___createMesh.End();

            // Assign mesh
            ___assignMesh.Begin();
            MeshFilter mf = GetComponent<MeshFilter>();
            mf.sharedMesh = billboardMesh;
            ___assignMesh.End();

            // delay finalization until the end of the current frame:
            Invoke(nameof(PushNewMeshData), 0);

            ___CreateMesh.End();
        }

        /// <summary> Pushes mesh data from buffers to the GPU </summary>
        private void PushNewMeshData()
        {
            ___PushNewMeshData.Begin();

            ___complete.Begin();
            Dependency.Complete();
            UvAnimationDependency.Complete();
            ___complete.End();

            // Assign mesh data
            ___assignMeshData.Begin();
            {
                const MeshUpdateFlags flags = MeshUpdateFlags.DontRecalculateBounds | MeshUpdateFlags.DontNotifyMeshUsers | MeshUpdateFlags.DontValidateIndices | MeshUpdateFlags.DontResetBoneBounds;

                ___vertexBuffer.Begin();
                billboardMesh.SetVertices(meshVertices);// Each vertex is positioned at billboard origin
                billboardMesh.SetNormals(meshNormals);// Standard normals
                billboardMesh.SetTangents(meshTangents);// Tangent stores corners and size
                billboardMesh.SetUVs(channel: 0, meshUVs);// Standard uv coordinates into atlas
                ___vertexBuffer.End();

                ___indexBuffer.Begin();
                billboardMesh.SetIndexBufferParams(meshIndices.Length, IndexFormat.UInt16);
                billboardMesh.SetIndexBufferData(meshIndices, 0, 0, meshIndices.Length, flags);
                billboardMesh.subMeshCount = 1;
                billboardMesh.SetSubMesh(0, new SubMeshDescriptor(0, meshIndices.Length, MeshTopology.Triangles), flags);
                ___indexBuffer.End();

                billboardMesh.bounds = meshAABB[0];// Manually update bounds to account for max billboard height
            }
            ___assignMeshData.End();

            ___PushNewMeshData.End();
        }

        private void PushUVData()
        {
            ___PushUVData.Begin();

            // complete jobs:
            ___complete.Begin();
            UvAnimationDependency.Complete();
            ___complete.End();

            // Store new mesh UV set
            ___setUVs.Begin();
            if (billboardMesh.vertexCount == meshUVs.Length)
                billboardMesh.SetUVs(channel: 0, meshUVs);
            ___setUVs.End();

            ___PushUVData.End();
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
        private static Vector2 GetScaledBillboardSize(Vector2 size, Vector2 scale)
        {
            // Apply scale
            Vector2 finalSize;
            int xChange = (int)(size.x * (scale.x / BlocksFile.ScaleDivisor));
            int yChange = (int)(size.y * (scale.y / BlocksFile.ScaleDivisor));
            finalSize.x = (size.x + xChange);
            finalSize.y = (size.y + yChange);

            return finalSize * MeshReader.GlobalScale;
        }
        static float2 GetScaledBillboardSize(int record, NativeArray<float2> recordSizes, NativeArray<float2> recordScales, float scaleDivisor)
        {
            float2 size = recordSizes[record];
            float2 scale = recordScales[record];
            int2 change = (int2)(size * (scale / scaleDivisor));
            float2 finalSize = size + change;
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
            // Get references
            if (meshRenderer == null)
                meshRenderer = GetComponent<MeshRenderer>();

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

        #region Jobs

        [Unity.Burst.BurstCompile]
        public struct Indices16Job : IJobParallelFor
        {
            [WriteOnly] [NativeDisableParallelForRestriction] public NativeArray<ushort> indices;
            void IJobParallelFor.Execute(int billboard)
            {
                int currentIndex = billboard * indicesPerQuad;

                ushort a = (ushort)(billboard * vertsPerQuad);
                ushort b = (ushort)(a + 1);
                ushort c = (ushort)(a + 2);
                ushort d = (ushort)(a + 3);

                indices[currentIndex] = a;
                indices[currentIndex + 1] = b;
                indices[currentIndex + 2] = c;
                indices[currentIndex + 3] = d;
                indices[currentIndex + 4] = c;
                indices[currentIndex + 5] = b;
            }
        }

        [Unity.Burst.BurstCompile]
        public struct NormalsJob : IJobParallelFor
        {
            [WriteOnly] [NativeDisableParallelForRestriction] public NativeArray<float3> normal;
            void IJobParallelFor.Execute(int index)
            {
                // Using half way between forward and up for billboard normal
                // Workable for most lighting but will need a better system eventually
                normal[index] = new float3(0, 0.707106781187f, 0.707106781187f);// Vector3.Normalize(Vector3.up + Vector3.forward);
            }
        }

        [Unity.Burst.BurstCompile]
        public struct VertexJob : IJobParallelFor
        {
            [ReadOnly] public NativeArray<float3> origin;
            [WriteOnly] [NativeDisableParallelForRestriction] public NativeArray<float3> vertex;
            void IJobParallelFor.Execute(int billboard)
            {
                float3 o = origin[billboard];
                int offset = billboard * vertsPerQuad;
                vertex[offset] = o;
                vertex[offset + 1] = o;
                vertex[offset + 2] = o;
                vertex[offset + 3] = o;
            }
        }

        [Unity.Burst.BurstCompile]
        struct UVJob : IJobParallelFor
        {
            [ReadOnly] public NativeArray<Rect> uvRect;
            [WriteOnly] [NativeDisableParallelForRestriction] public NativeArray<float2> uv;
            void IJobParallelFor.Execute(int billboard)
            {
                Rect rect = uvRect[billboard];
                int offset = billboard * vertsPerQuad;

                uv[offset] = new float2(rect.x, rect.yMax);
                uv[offset + 1] = new float2(rect.xMax, rect.yMax);
                uv[offset + 2] = new float2(rect.x, rect.y);
                uv[offset + 3] = new float2(rect.xMax, rect.y);
            }
        }
        [Unity.Burst.BurstCompile]
        struct CustomRectUVJob : IJobParallelFor
        {
            [ReadOnly] public NativeArray<BillboardItem> billboards;
            [WriteOnly] [NativeDisableParallelForRestriction] public NativeArray<float2> uv;
            void IJobParallelFor.Execute(int billboard)
            {
                BillboardItem bi = billboards[billboard];
                int offset = billboard * vertsPerQuad;

                Rect rect = bi.customRect;
                uv[offset] = new float2(rect.x, rect.yMax);
                uv[offset + 1] = new float2(rect.xMax, rect.yMax);
                uv[offset + 2] = new float2(rect.x, rect.y);
                uv[offset + 3] = new float2(rect.xMax, rect.y);
            }
        }
        [Unity.Burst.BurstCompile]
        struct AnimateUVJob : IJobParallelFor
        {
            [ReadOnly] [DeallocateOnJobCompletion] public NativeArray<Rect> atlasRects;
            [ReadOnly] [DeallocateOnJobCompletion] public NativeArray<RecordIndex> atlasIndices;
            public NativeArray<BillboardItem> billboards;
            [WriteOnly] [NativeDisableParallelForRestriction] public NativeArray<float2> uv;
            void IJobParallelFor.Execute(int billboard)
            {
                BillboardItem bi = billboards[billboard];
                // Look for animated billboards. Do nothing if single frame
                if (bi.totalFrames > 1)
                {
                    // Increment current billboard frame
                    if (++bi.currentFrame >= bi.totalFrames)
                        bi.currentFrame = 0;
                    billboards[billboard] = bi;

                    // Set new UV properties based on current frame
                    Rect rect = atlasRects[atlasIndices[bi.record].startIndex + bi.currentFrame];
                    int offset = billboard * vertsPerQuad;
                    uv[offset] = new float2(rect.x, rect.yMax);
                    uv[offset + 1] = new float2(rect.xMax, rect.yMax);
                    uv[offset + 2] = new float2(rect.x, rect.y);
                    uv[offset + 3] = new float2(rect.xMax, rect.y);
                }
            }
        }

        [Unity.Burst.BurstCompile]
        struct TangentJob : IJobParallelFor
        {
            [ReadOnly] public NativeArray<float2> size;
            [WriteOnly] [NativeDisableParallelForRestriction] public NativeArray<float4> tangent;
            void IJobParallelFor.Execute(int billboard)
            {
                float2 s = size[billboard];
                int offset = billboard * vertsPerQuad;

                // Tangent data for shader is used to size billboard
                tangent[offset] = new float4(s.x, s.y, 0, 1);
                tangent[offset + 1] = new float4(s.x, s.y, 1, 1);
                tangent[offset + 2] = new float4(s.x, s.y, 0, 0);
                tangent[offset + 3] = new float4(s.x, s.y, 1, 0);
            }
        }

        [Unity.Burst.BurstCompile]
        struct GetBatchDataJob : IJobParallelFor
        {
            [ReadOnly] public NativeArray<BillboardItem> billboards;
            [ReadOnly] [DeallocateOnJobCompletion] public NativeArray<float2> recordSize;
            [ReadOnly] [DeallocateOnJobCompletion] public NativeArray<float2> recordScale;
            [ReadOnly] [DeallocateOnJobCompletion] public NativeArray<Rect> atlasRects;
            [ReadOnly] [DeallocateOnJobCompletion] public NativeArray<RecordIndex> atlasIndices;
            public float scaleDivisor;
            [WriteOnly] public NativeArray<float3> origin;
            [WriteOnly] public NativeArray<float2> size;
            [WriteOnly] public NativeArray<Rect> uvRect;
            void IJobParallelFor.Execute(int billboard)
            {
                BillboardItem bi = billboards[billboard];

                float2 size = DaggerfallBillboardBatch.GetScaledBillboardSize(bi.record, recordSize, recordScale, scaleDivisor);
                float3 origin = bi.position + new float3(0, size.y * 0.5f, 0);
                Rect uvrect = atlasRects[atlasIndices[bi.record].startIndex + bi.currentFrame];

                this.size[billboard] = size;
                uvRect[billboard] = uvrect;
                this.origin[billboard] = origin;
            }
        }
        [Unity.Burst.BurstCompile]
        struct GetCustomMaterialBatchDataJob : IJobParallelFor
        {
            [ReadOnly] public NativeArray<BillboardItem> billboards;
            [ReadOnly] [DeallocateOnJobCompletion] public NativeArray<Rect> atlasRects;
            [ReadOnly] [DeallocateOnJobCompletion] public NativeArray<RecordIndex> atlasIndices;
            [WriteOnly] public NativeArray<float3> origin;
            [WriteOnly] public NativeArray<float2> size;
            void IJobParallelFor.Execute(int billboard)
            {
                BillboardItem bi = billboards[billboard];

                float2 s = DaggerfallBillboardBatch.GetScaledBillboardSize((Vector2)bi.customSize, (Vector2)bi.customScale);
                float3 origin = bi.position + new float3(0, s.y * 0.5f, 0);
                Rect uvrect = atlasRects[atlasIndices[bi.record].startIndex + bi.currentFrame];

                size[billboard] = s;
                this.origin[billboard] = origin;
            }
        }

        [Unity.Burst.BurstCompile]
        struct BoundsJob : IJob
        {
            public int numBillboards;
            [ReadOnly] public NativeArray<float3> origin;
            [ReadOnly] public NativeArray<float2> size;
            [WriteOnly] public NativeArray<Bounds> aabb;
            void IJob.Execute()
            {
                // Update bounds tracking using actual position and size
                // This can be a little wonky with single billboards side-on as AABB does not rotate
                // But it generally works well for large batches as intended
                // Multiply finalSize * 2f if culling problems with standalone billboards
                this.aabb[0] = new Bounds();
                if (numBillboards == 0) return;
                Bounds aabb = new Bounds(origin[0], (Vector2)size[0]); ;
                for (int billboard = 0; billboard < numBillboards; billboard++)
                    aabb.Encapsulate(new Bounds(origin[billboard], (Vector2)size[billboard]));
                this.aabb[0] = aabb;
            }
        }

        [Unity.Burst.BurstCompile]
        struct AddItemsJob : IJobParallelFor
        {
            [ReadOnly] public NativeArray<BasicInfo> source;
            [ReadOnly] [DeallocateOnJobCompletion] public NativeArray<int> atlasFrameCounts;
            public bool randomStartFrame;
            public uint seed;
            public float3 blockOrigin;
            [WriteOnly] public NativeList<BillboardItem>.ParallelWriter billboardItems;
            void IJobParallelFor.Execute(int index)
            {
                var item = source[index];

                // Get frame count and start frame
                int frameCount = atlasFrameCounts[item.textureRecord];
                int startFrame = 0;
                if (randomStartFrame)
                    startFrame = new Unity.Mathematics.Random(seed * (uint)(index + 1)).NextInt(0, frameCount);

                // Add new billboard to batch
                var billboard = new BillboardItem
                {
                    record = item.textureRecord,
                    position = blockOrigin + item.localPosition,
                    totalFrames = frameCount,
                    currentFrame = startFrame,
                };
                billboardItems.AddNoResize(billboard);
            }
        }

        [Unity.Burst.BurstCompile]
        struct AddCustomItemsJob : IJobParallelFor
        {
            [ReadOnly] public NativeArray<CustomInfo> source;
            public float3 blockOrigin;
            [WriteOnly] public NativeList<BillboardItem>.ParallelWriter billboardItems;
            void IJobParallelFor.Execute(int index)
            {
                CustomInfo item = source[index];
                BillboardItem billboard = new BillboardItem
                {
                    position = blockOrigin + item.localPosition,
                    customRect = item.rect,
                    customSize = item.size,
                    customScale = item.scale,
                };
                billboardItems.AddNoResize(billboard);
            }
        }

        // @TODO: move this job outside into a dedicated "UniversalJobs.cs" file, later on
        [Unity.Burst.BurstCompile]
        struct DeallocateArrayJob<T> : IJob where T : unmanaged
        {
            [ReadOnly] [DeallocateOnJobCompletion] NativeArray<T> array;
            public DeallocateArrayJob(NativeArray<T> array) => this.array = array;
            void IJob.Execute() { }
        }

        #endregion

        public struct BasicInfo
        {
            public int textureRecord;
            public float3 localPosition;
            public BasicInfo(int record, float3 localPosition)
            {
                this.textureRecord = record;
                this.localPosition = localPosition;
            }
        }

        public struct CustomInfo
        {
            public Rect rect;
            public float2 size;
            public float2 scale;
            public float3 localPosition;
        }

    }
}
