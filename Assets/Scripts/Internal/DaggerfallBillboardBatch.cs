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

using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using System.Collections.Generic;
using System;
using DaggerfallConnect;
using DaggerfallConnect.Utility;
using DaggerfallConnect.Arena2;
using Unity.Profiling;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;

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
        public const int maxBillboardCount = 16250;

        [SerializeField, HideInInspector]
        Material customMaterial = null;
        [SerializeField, HideInInspector]
        CachedMaterial cachedMaterial;
        
        NativeList<BillboardItem> billboardData;

        [SerializeField, HideInInspector]
        Mesh mesh;
        NativeArray<VertexBuffer> vertexBuffer;
        NativeArray<ushort> indexBuffer;
        NativeArray<Bounds> aabbPtr;
        JobHandle Dependency;

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
            public float3 position;            // Position from origin to render billboard
            public int totalFrames;             // Total animation frames
            public int currentFrame;            // Current animation frame
            public Rect customRect;             // Rect for custom material path
            public float2 customSize;          // Size for custom material path
            public float2 customScale;         // Scale for custom material path
        }

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct VertexBuffer
        {
            public float3 vertex;// Each vertex is positioned at billboard origin
            public float3 normal;// Standard normals
            public float4 tangent;// Tangent stores corners and size
            public float2 uv;// Standard uv coordinates into atlas

            // byte* offsets for member fields
            public const byte
                vertexOffset = 0,
                normalOffset = vertexOffset + 3*4,
                tangentOffset = normalOffset + 3*4,
                uvOffset = tangentOffset + 4*4;
            public override string ToString() => $"(v:{vertex},n:{normal},t:{tangent},uv:{uv})";
        }

        // note: this must 100% reflect VertexBuffer structure
        static readonly VertexAttributeDescriptor[] vertexBufferLayout = new VertexAttributeDescriptor[]{
            new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3),
            new VertexAttributeDescriptor(VertexAttribute.Normal, VertexAttributeFormat.Float32, 3),
            new VertexAttributeDescriptor(VertexAttribute.Tangent, VertexAttributeFormat.Float32, 4),
            new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, 2),
        };

        public bool IsCustom => customMaterial != null;

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
            ___reuseMesh = new ProfilerMarker("reuse mesh"),
            ___assignMesh = new ProfilerMarker("assign mesh"),
            ___assignMeshData = new ProfilerMarker("push mesh data"),
            ___indexBufferInitialize = new ProfilerMarker("index buffer initialize"),
            ___vertexBufferInitialize = new ProfilerMarker("vertex buffer initialize"),
            ___indexBufferPush = new ProfilerMarker("index buffer push"),
            ___vertexBufferPush = new ProfilerMarker("vertex buffer push"),
            ___setMaterial = new ProfilerMarker("set material");

        void Awake()
        {
            mesh = new Mesh();
            billboardData = new NativeList<BillboardItem>(initialCapacity: maxBillboardCount, Allocator.Persistent);
            vertexBuffer = new NativeArray<VertexBuffer>(0, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            indexBuffer = new NativeArray<ushort>(0, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            aabbPtr = new NativeArray<Bounds>(1, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
        }

        void OnDestroy()
        {
            Dependency.Complete();// make sure there are no unfinished jobs
            if (billboardData.IsCreated) billboardData.Dispose();
            if (vertexBuffer.IsCreated) vertexBuffer.Dispose();
            if (indexBuffer.IsCreated) indexBuffer.Dispose();
            if (aabbPtr.IsCreated) aabbPtr.Dispose();
            if (mesh != null) Destroy(mesh);
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
            float waitFps = FramesPerSecond;
            WaitForSeconds wait = new WaitForSeconds(1f / FramesPerSecond);// reuse

            while (true)
            {
                if (FramesPerSecond > waitFps || FramesPerSecond < waitFps)
                {
                    waitFps = FramesPerSecond;
                    wait = new WaitForSeconds(1f / waitFps);
                }
                
                // Tick animation when valid
                ___tick.Begin();
                int numBillboardsToAnimate = vertexBuffer.Length / vertsPerQuad;
                if (
                        FramesPerSecond > 0
                    &&  cachedMaterial.key != 0
                    &&  customMaterial == null
                    &&  numBillboardsToAnimate!=0
                )
                {
                    // schedule jobs:
                    ___schedule.Begin();
                    var animateUVJob = new AnimateUVJob
                    {
                        AtlasRects      = cachedMaterial.atlasRects.AsNativeArray(out ulong gcHandleRects),
                        AtlasIndices    = cachedMaterial.atlasIndices.AsNativeArray(out ulong gcHandleIndices),
                        Billboards      = billboardData,
                        Buffer          = vertexBuffer,
                    };
                    var jobHandle = animateUVJob.Schedule(numBillboardsToAnimate, 128,Dependency);
                    JobUtility.ReleaseGCObject(gcHandleRects, jobHandle);
                    JobUtility.ReleaseGCObject(gcHandleIndices,jobHandle);
                    ___schedule.End();

                    // complete jobs:
                    ___complete.Begin();
                    jobHandle.Complete();
                    ___complete.End();

                    // Store new mesh UV set
                    ___setUVs.Begin();
                    if( mesh.vertexCount==vertexBuffer.Length )
                    {
                        PushNewMeshData();
                    }
                    ___setUVs.End();
                }
                ___tick.End();

                yield return wait;
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
            ___setMaterial.Begin();

            if (!ReadyCheck())
            {
                ___setMaterial.End();
                return;
            }

            // Do nothing if this archive already set
            if (archive == currentArchive && !force)
            {
                ___setMaterial.End();
                return;
            }

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

            ___setMaterial.End();
        }

        /// <summary>
        /// Directly set custom atlas material all billboards in batch will share.
        /// Custom material allows you to directly set item rects in batch.
        /// </summary>
        /// <param name="material"></param>
        public void SetMaterial(Material material)
        {
            ___setMaterial.Begin();

            if (!ReadyCheck())
            {
                ___setMaterial.End();
                return;
            }

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

            ___setMaterial.End();
        }

        /// <summary>
        /// Clear all billboards from batch.
        /// </summary>
        public void Clear()
        {
            Dependency.Complete();// make sure there are no unfinished jobs
            billboardData.Clear();
        }

        /// <summary>
        /// Add a billboard to batch.
        /// </summary>
        [System.Obsolete("Please use " + nameof(AddItemsAsync) + " instead")]
        public void AddItem(int record, Vector3 localPosition)
        {
            Dependency.Complete();// make sure there are no unfinished jobs

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
            if (billboardData.Length + 1 > maxBillboardCount)
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
            var billboard = new BillboardItem
            {
                record          = record,
                position        = BlockOrigin + localPosition,
                totalFrames     = frameCount,
                currentFrame    = startFrame,
            };
            billboardData.Add(billboard);
        }
        /// <summary>
        /// Schedules a job that adds an array of billboard data to batch
        /// </summary>
        public JobHandle AddItemsAsync( NativeArray<ItemToAdd> itemsToAdd )
        {
            // Cannot use with a custom material
            if (customMaterial != null)
                throw new Exception("Cannot use with custom material. Use AddItem(Rect rect, Vector2 size, Vector2 scale, Vector3 localPosition) overload instead.");

            // Must have set a material
            if (cachedMaterial.key == 0)
            {
                DaggerfallUnity.LogMessage("DaggerfallBillboardBatch: Must call SetMaterial() before adding items.", true);
                return default(JobHandle);
            }

            // Limit maximum billboards in batch
            int available = maxBillboardCount - billboardData.Length;
            int numItemsToAdd = math.min(available, itemsToAdd.Length);
            if (numItemsToAdd != 0)
            {
                var job = new AddItemsJob
                {
                    Source              = itemsToAdd,
                    AtlasFrameCounts    = cachedMaterial.atlasFrameCounts.AsNativeArray(out ulong gcHandleCounts),
                    RandomStartFrame    = RandomStartFrame,
                    Seed                = (uint)((Environment.TickCount * this.GetHashCode()).GetHashCode()),
                    BlockOrigin         = BlockOrigin,
                    BillboardItems      = billboardData.AsParallelWriter(),
                };
                Dependency = job.Schedule(arrayLength: numItemsToAdd, innerloopBatchCount: 128, dependsOn: Dependency);
                JobUtility.ReleaseGCObject(gcHandleCounts, Dependency);
            }
            
            if (billboardData.Length == maxBillboardCount)
                DaggerfallUnity.LogMessage("DaggerfallBillboardBatch: Maximum batch size reached.", true);
            
            return Dependency;
        }

        /// <summary>
        /// Add a billboard to batch.
        /// Use this overload for custom atlas material.
        /// </summary>
        public void AddItem(Rect rect, Vector2 size, Vector2 scale, Vector3 localPosition)
        {
            Dependency.Complete();// make sure there are no unfinished jobs

            // Cannot use with auto material
            if (customMaterial == null)
                throw new Exception("Cannot use with auto material. Use AddItem(int record, Vector3 localPosition) overload instead.");

            // Add new billboard to batch
            var billboard = new BillboardItem
            {
                position        = BlockOrigin + localPosition,
                customRect      = rect,
                customSize      = size,
                customScale     = scale,
            };
            billboardData.Add(billboard);
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
            {
                ___createMeshForCustomMaterial.Begin();
                PrefareMeshDataForCustomMaterial();
                ___createMeshForCustomMaterial.End();
            }
            else
            {
                ___createMesh.Begin();
                PrepareMeshData();
                ___createMesh.End();
            }
           
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

            var items = new NativeArray<ItemToAdd>(RandomDepth * RandomWidth, Allocator.TempJob);
            float dist = RandomSpacing;
            int i = 0;
            for (int y = 0; y < RandomDepth; y++)
            for (int x = 0; x < RandomWidth; x++)
            {
                int record = UnityEngine.Random.Range(minRecord, maxRecord);
                float3 localPosition = new float3(x * dist, 0, y * dist);
                items[i++] = new ItemToAdd(record, localPosition);
            }
            var op = AddItemsAsync(items);
            op.Complete();
            Apply();
        }

        #endregion

        #region Private Methods

        
        /// <summary> Resizes when it's size is invalid. </summary>
        void ResizeMeshBuffers()
        {
            const MeshUpdateFlags flags = MeshUpdateFlags.DontRecalculateBounds | MeshUpdateFlags.DontNotifyMeshUsers | MeshUpdateFlags.DontValidateIndices | MeshUpdateFlags.DontResetBoneBounds;
            int numBillboards = billboardData.Length;
            int numVertices = numBillboards * vertsPerQuad;
            int numIndices = numBillboards * indicesPerQuad;
            if (vertexBuffer.Length != numVertices)
            {
                vertexBuffer.Dispose(Dependency);
                vertexBuffer = new NativeArray<VertexBuffer>(numVertices, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);

                ___vertexBufferInitialize.Begin();
                mesh.SetVertexBufferParams(numVertices, vertexBufferLayout);
                ___vertexBufferInitialize.End();
            }
            if (indexBuffer.Length != numIndices)
            {
                indexBuffer.Dispose(Dependency);
                indexBuffer = new NativeArray<ushort>(numIndices, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);

                ___indexBufferInitialize.Begin();
                mesh.SetIndexBufferParams(numIndices, IndexFormat.UInt16);
                mesh.subMeshCount = 1;
                mesh.SetSubMesh(0, new SubMeshDescriptor(0, numIndices, MeshTopology.Triangles), flags);
                ___indexBufferInitialize.End();
            }
        }

        /// <summary>
        /// TEMP: Create mesh for custom material path.
        /// This can be improved as it's mostly the same as CreateMesh().
        /// Keeping separate for now until super-atlases are better integrated.
        /// </summary>
        private void PrefareMeshDataForCustomMaterial()
        {
            Dependency.Complete();// make sure there are no unfinished jobs

            // Create billboard data
            ___schedule.Begin();
            ResizeMeshBuffers();
            int numBillboards = billboardData.Length;
            var origins = new NativeArray<float3>(numBillboards, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
            var sizes = new NativeArray<float2>(numBillboards, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);

            var getCustomBatchDataJobHandle = new GetCustomMaterialBatchDataJob
            {
                Billboards      = billboardData,
                AtlasRects      = cachedMaterial.atlasRects.AsNativeArray(out ulong gcHandleRects),
                AtlasIndices    = cachedMaterial.atlasIndices.AsNativeArray(out ulong gcHandleIndices),
                Origin          = origins,
                Size            = sizes,
            }.Schedule(numBillboards, 128,Dependency);
            JobUtility.ReleaseGCObject(gcHandleRects, getCustomBatchDataJobHandle);
            JobUtility.ReleaseGCObject(gcHandleIndices, getCustomBatchDataJobHandle);

            var boundsJobHandle = new BoundsJob
            {
                NumBillboards   = numBillboards,
                Origin          = origins,
                Size            = sizes,
                AABB            = aabbPtr
            }.Schedule(getCustomBatchDataJobHandle);

            var vertexJobHandle = new VertexJob
            {
                Origin          = origins,
                Buffer          = vertexBuffer,
            }.Schedule(numBillboards, 128, getCustomBatchDataJobHandle);
            
            var uvJobHandle = new CustomRectUVJob
            {
                Billboards      = billboardData,
                Buffer          = vertexBuffer,
            }.Schedule(numBillboards, 128, getCustomBatchDataJobHandle);

            var tangentJobHandle = new TangentJob
            {
                Size            = sizes,
                Buffer          = vertexBuffer,
            }.Schedule(numBillboards, 128, getCustomBatchDataJobHandle);

            var normalsJobHandle = new NormalsJob
            {
                Buffer          = vertexBuffer,
            }.Schedule(numBillboards, 128, getCustomBatchDataJobHandle);

            var indicesJobHandle = new Indices16Job
            {
                Indices         = indexBuffer,
            }.Schedule(numBillboards, 128, Dependency);

            var handles = new NativeArray<JobHandle>(6, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            handles[0] = vertexJobHandle;
            handles[1] = normalsJobHandle;
            handles[2] = indicesJobHandle;
            handles[3] = tangentJobHandle;
            handles[4] = uvJobHandle;
            handles[5] = boundsJobHandle;
            Dependency = JobHandle.CombineDependencies(handles);

            // deallocate leftovers:
            origins.Dispose(Dependency);
            sizes.Dispose(Dependency);
            ___schedule.End();

            // Reuse mesh
            ___reuseMesh.Begin();
            mesh.name = "BillboardBatchMesh [CustomPath]";
            mesh.Clear(keepVertexLayout: mesh.vertexCount == vertexBuffer.Length);
            ___reuseMesh.End();

            // Assign mesh
            ___assignMesh.Begin();
            MeshFilter mf = GetComponent<MeshFilter>();
            mf.sharedMesh = mesh;
            ___assignMesh.End();

            PushNewMeshDataDelayed();
        }

        // Packs all billboards into single mesh
        private void PrepareMeshData()
        {
            Dependency.Complete();// make sure there are no unfinished jobs

            // Create billboard data
            ___schedule.Begin();
            ResizeMeshBuffers();
            int numBillboards = billboardData.Length;
            var origins = new NativeArray<float3>(numBillboards, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
            var sizes = new NativeArray<float2>(numBillboards, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
            var uvrects = new NativeArray<Rect>(numBillboards, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
            
            var getBatchDataJob = new GetBatchDataJob
            {
                Billboards      = billboardData,
                RecordSize      = cachedMaterial.recordSizes.AsNativeArray(out ulong gcHandleSize).Reinterpret<float2>(),
                RecordScale     = cachedMaterial.recordScales.AsNativeArray(out ulong gcHandleScale).Reinterpret<float2>(),
                AtlasRects      = cachedMaterial.atlasRects.AsNativeArray(out ulong gcHandleRects),
                AtlasIndices    = cachedMaterial.atlasIndices.AsNativeArray(out ulong gcHandleIndices),
                ScaleDivisor    = BlocksFile.ScaleDivisor,
                
                Origin          = origins,
                Size            = sizes,
                UVRect          = uvrects,
            };
            var getBatchDataJobHandle = getBatchDataJob.Schedule(numBillboards, 128, Dependency);
            JobUtility.ReleaseGCObject(gcHandleSize, getBatchDataJobHandle);
            JobUtility.ReleaseGCObject(gcHandleScale, getBatchDataJobHandle);
            JobUtility.ReleaseGCObject(gcHandleRects, getBatchDataJobHandle);
            JobUtility.ReleaseGCObject(gcHandleIndices, getBatchDataJobHandle);

            var boundsJob = new BoundsJob
            {
                NumBillboards   = numBillboards,
                Origin          = origins,
                Size            = sizes,
                AABB            = aabbPtr
            };
            var boundsJobHandle = boundsJob.Schedule(getBatchDataJobHandle);

            var vertexJob = new VertexJob
            {
                Origin          = origins,
                Buffer          = vertexBuffer,
            };
            var vertexJobHandle = vertexJob.Schedule(numBillboards, 128, getBatchDataJobHandle);
            
            var uvJob = new UVJob
            {
                UVRect          = uvrects,
                Buffer          = vertexBuffer,
            };
            var uvJobHandle = uvJob.Schedule(numBillboards, 128, getBatchDataJobHandle);

            var tangentJob = new TangentJob
            {
                Size            = sizes,
                Buffer          = vertexBuffer,
            };
            var tangentJobHandle = tangentJob.Schedule(numBillboards, 128, getBatchDataJobHandle);

            var normalsJob = new NormalsJob
            {
                Buffer          = vertexBuffer,
            };
            var normalsJobHandle = normalsJob.Schedule(numBillboards, 128, getBatchDataJobHandle);

            var indicesJob = new Indices16Job
            {
                Indices         = indexBuffer,
            };
            var indicesJobHandle = indicesJob.Schedule(numBillboards, 128, Dependency);

            var handles = new NativeArray<JobHandle>(6, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            handles[0] = vertexJobHandle;
            handles[1] = normalsJobHandle;
            handles[2] = indicesJobHandle;
            handles[3] = tangentJobHandle;
            handles[4] = uvJobHandle;
            handles[5] = boundsJobHandle;
            Dependency = JobHandle.CombineDependencies(handles);

            // deallocate leftovers:
            origins.Dispose(Dependency);
            sizes.Dispose(Dependency);
            uvrects.Dispose(Dependency);
            ___schedule.End();

            // Reuse mesh
            ___reuseMesh.Begin();
            mesh.name = "BillboardBatchMesh";
            mesh.Clear(keepVertexLayout: mesh.vertexCount == vertexBuffer.Length);
            ___reuseMesh.End();

            // Assign mesh
            ___assignMesh.Begin();
            MeshFilter mf = GetComponent<MeshFilter>();
            mf.sharedMesh = mesh;
            ___assignMesh.End();

            PushNewMeshDataDelayed();
        }

        /// <summary> Pushes mesh data from buffers to the GPU </summary>
        public void PushNewMeshData ()
        {
            ___complete.Begin();
            Dependency.Complete();
            ___complete.End();

            // Assign mesh data
            ___assignMeshData.Begin();
            {
                const MeshUpdateFlags flags = MeshUpdateFlags.DontRecalculateBounds | MeshUpdateFlags.DontNotifyMeshUsers | MeshUpdateFlags.DontValidateIndices | MeshUpdateFlags.DontResetBoneBounds;

                bool reinitialize = mesh.vertexCount != vertexBuffer.Length;

                ___vertexBufferPush.Begin();
                if (reinitialize)
                {
                    ___vertexBufferInitialize.Begin();
                    mesh.SetVertexBufferParams(vertexBuffer.Length, vertexBufferLayout);
                    ___vertexBufferInitialize.End();
                }
                // Debug.LogWarning($"vertexBuffer: {vertexBuffer.ToReadableString()}");
                mesh.SetVertexBufferData(vertexBuffer, 0, 0, vertexBuffer.Length, 0, flags);
                ___vertexBufferPush.End();

                ___indexBufferPush.Begin();
                if (reinitialize)
                {
                    ___indexBufferInitialize.Begin();
                    mesh.SetIndexBufferParams(indexBuffer.Length, IndexFormat.UInt16);
                    mesh.subMeshCount = 1;
                    mesh.SetSubMesh(0, new SubMeshDescriptor(0, indexBuffer.Length, MeshTopology.Triangles), flags);
                    ___indexBufferInitialize.End();
                }
                // Debug.LogWarning($"indexBuffer: {indexBuffer.ToReadableString()}");
                mesh.SetIndexBufferData(indexBuffer, 0, 0, indexBuffer.Length, flags);
                ___indexBufferPush.End();
                
                mesh.bounds = aabbPtr[0];// Manually update bounds to account for max billboard height
            }
            ___assignMeshData.End();
        }
        /// <summary> Pushes mesh data from buffers to the GPU. Delayed to the end of the current frame. </summary>
        public void PushNewMeshDataDelayed()
            => Invoke(nameof(PushNewMeshData), 0);

        // Gets scaled billboard size to properly size billboard in world
        private Vector2 GetScaledBillboardSize(int record)
        {
            // Get size and scale
            Vector2 size = cachedMaterial.recordSizes[record];
            Vector2 scale = cachedMaterial.recordScales[record];

            return GetScaledBillboardSize(size, scale);
        }

        // Gets scaled billboard size to properly size billboard in world
        private static Vector2 GetScaledBillboardSize(float2 size, float2 scale)
        {
            // Apply scale
            int2 change = (int2)(size * (scale / BlocksFile.ScaleDivisor));
            float2 finalSize = size + change;

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

        #region Jobs

        [Unity.Burst.BurstCompile]
        public struct Indices16Job : IJobParallelFor
        {
            [WriteOnly][NativeDisableParallelForRestriction] public NativeArray<ushort> Indices;
            void IJobParallelFor.Execute(int billboard)
            {
                int currentIndex = billboard * indicesPerQuad;

                ushort a = (ushort)(billboard * vertsPerQuad);
                ushort b = (ushort)(a + 1);
                ushort c = (ushort)(a + 2);
                ushort d = (ushort)(a + 3);

                Indices[currentIndex] = a;
                Indices[currentIndex + 1] = b;
                Indices[currentIndex + 2] = c;
                Indices[currentIndex + 3] = d;
                Indices[currentIndex + 4] = c;
                Indices[currentIndex + 5] = b;
            }
        }

        [Unity.Burst.BurstCompile]
        public struct NormalsJob : IJobParallelFor
        {
            public NativeArray<VertexBuffer> Buffer;
            unsafe void IJobParallelFor.Execute(int billboard)
            {
                // Using half way between forward and up for billboard normal
                // Workable for most lighting but will need a better system eventually
                float3 normal = new float3(0, 0.707106781187f, 0.707106781187f);// Vector3.Normalize(Vector3.up + Vector3.forward);
                int vertex = billboard * vertsPerQuad;
                
                VertexBuffer* quadPtr = (VertexBuffer*)NativeArrayUnsafeUtility.GetUnsafePtr(Buffer) + vertex;

                float3* ptr0 = (float3*)((byte*)(quadPtr + 0) + VertexBuffer.normalOffset);
                float3* ptr1 = (float3*)((byte*)(quadPtr + 1) + VertexBuffer.normalOffset);
                float3* ptr2 = (float3*)((byte*)(quadPtr + 2) + VertexBuffer.normalOffset);
                float3* ptr3 = (float3*)((byte*)(quadPtr + 3) + VertexBuffer.normalOffset);

                Buffer.AssertPtrScope(ptr0);
                Buffer.AssertPtrScope(ptr1);
                Buffer.AssertPtrScope(ptr2);
                Buffer.AssertPtrScope(ptr3);

                *ptr0 = normal;
                *ptr1 = normal;
                *ptr2 = normal;
                *ptr3 = normal;
            }
        }

        [Unity.Burst.BurstCompile]
        public struct VertexJob : IJobParallelFor
        {
            [ReadOnly] public NativeArray<float3> Origin;
            public NativeArray<VertexBuffer> Buffer;
            unsafe void IJobParallelFor.Execute(int billboard)
            {
                float3 origin = Origin[billboard];
                int vertex = billboard * vertsPerQuad;

                VertexBuffer* quadPtr = (VertexBuffer*)NativeArrayUnsafeUtility.GetUnsafePtr(Buffer) + vertex;

                float3* ptr0 = (float3*)((byte*)(quadPtr + 0) + VertexBuffer.vertexOffset);
                float3* ptr1 = (float3*)((byte*)(quadPtr + 1) + VertexBuffer.vertexOffset);
                float3* ptr2 = (float3*)((byte*)(quadPtr + 2) + VertexBuffer.vertexOffset);
                float3* ptr3 = (float3*)((byte*)(quadPtr + 3) + VertexBuffer.vertexOffset);

                Buffer.AssertPtrScope(ptr0);
                Buffer.AssertPtrScope(ptr1);
                Buffer.AssertPtrScope(ptr2);
                Buffer.AssertPtrScope(ptr3);

                *ptr0 = origin;
                *ptr1 = origin;
                *ptr2 = origin;
                *ptr3 = origin;
            }
        }

        [Unity.Burst.BurstCompile]
        struct UVJob : IJobParallelFor
        {
            [ReadOnly] public NativeArray<Rect> UVRect;
            public NativeArray<VertexBuffer> Buffer;
            unsafe void IJobParallelFor.Execute(int billboard)
            {
                Rect rect = UVRect[billboard];
                int vertex = billboard * vertsPerQuad;

                VertexBuffer* quadPtr = (VertexBuffer*)NativeArrayUnsafeUtility.GetUnsafePtr(Buffer) + vertex;

                float2* ptr0 = (float2*)((byte*)(quadPtr + 0) + VertexBuffer.uvOffset);
                float2* ptr1 = (float2*)((byte*)(quadPtr + 1) + VertexBuffer.uvOffset);
                float2* ptr2 = (float2*)((byte*)(quadPtr + 2) + VertexBuffer.uvOffset);
                float2* ptr3 = (float2*)((byte*)(quadPtr + 3) + VertexBuffer.uvOffset);

                Buffer.AssertPtrScope(ptr0);
                Buffer.AssertPtrScope(ptr1);
                Buffer.AssertPtrScope(ptr2);
                Buffer.AssertPtrScope(ptr3);

                *ptr0 = new float2(rect.x,rect.yMax);
                *ptr1 = new float2(rect.xMax,rect.yMax);
                *ptr2 = new float2(rect.x,rect.y);
                *ptr3 = new float2(rect.xMax,rect.y);
            }
        }
        [Unity.Burst.BurstCompile]
        struct CustomRectUVJob : IJobParallelFor
        {
            [ReadOnly] public NativeArray<BillboardItem> Billboards;
            public NativeArray<VertexBuffer> Buffer;
            unsafe void IJobParallelFor.Execute(int billboard)
            {
                BillboardItem bi = Billboards[billboard];
                Rect rect = bi.customRect;
                int vertex = billboard * vertsPerQuad;

                VertexBuffer* quadPtr = (VertexBuffer*)NativeArrayUnsafeUtility.GetUnsafePtr(Buffer) + vertex;

                float2* ptr0 = (float2*)((byte*)(quadPtr + 0) + VertexBuffer.uvOffset);
                float2* ptr1 = (float2*)((byte*)(quadPtr + 1) + VertexBuffer.uvOffset);
                float2* ptr2 = (float2*)((byte*)(quadPtr + 2) + VertexBuffer.uvOffset);
                float2* ptr3 = (float2*)((byte*)(quadPtr + 3) + VertexBuffer.uvOffset);

                Buffer.AssertPtrScope(ptr0);
                Buffer.AssertPtrScope(ptr1);
                Buffer.AssertPtrScope(ptr2);
                Buffer.AssertPtrScope(ptr3);

                *ptr0 = new float2(rect.x,rect.yMax);
                *ptr1 = new float2(rect.xMax,rect.yMax);
                *ptr2 = new float2(rect.x,rect.y);
                *ptr3 = new float2(rect.xMax,rect.y);
            }
        }
        [Unity.Burst.BurstCompile]
        struct AnimateUVJob : IJobParallelFor
        {
            [ReadOnly] public NativeArray<Rect> AtlasRects;
            [ReadOnly] public NativeArray<RecordIndex> AtlasIndices;
            public NativeArray<BillboardItem> Billboards;
            public NativeArray<VertexBuffer> Buffer;
            unsafe void IJobParallelFor.Execute(int billboard)
            {
                BillboardItem bi = Billboards[billboard];
                // Look for animated billboards. Do nothing if single frame
                if (bi.totalFrames > 1)
                {
                    // Increment current billboard frame
                    if (++bi.currentFrame >= bi.totalFrames)
                        bi.currentFrame = 0;
                    Billboards[billboard] = bi;

                    // Set new UV properties based on current frame
                    Rect rect = AtlasRects[AtlasIndices[bi.record].startIndex + bi.currentFrame];
                    int vertex = billboard * vertsPerQuad;

                    VertexBuffer* quadPtr = (VertexBuffer*)NativeArrayUnsafeUtility.GetUnsafePtr(Buffer) + vertex;

                    float2* ptr0 = (float2*)((byte*)(quadPtr + 0) + VertexBuffer.uvOffset);
                    float2* ptr1 = (float2*)((byte*)(quadPtr + 1) + VertexBuffer.uvOffset);
                    float2* ptr2 = (float2*)((byte*)(quadPtr + 2) + VertexBuffer.uvOffset);
                    float2* ptr3 = (float2*)((byte*)(quadPtr + 3) + VertexBuffer.uvOffset);

                    Buffer.AssertPtrScope(ptr0);
                    Buffer.AssertPtrScope(ptr1);
                    Buffer.AssertPtrScope(ptr2);
                    Buffer.AssertPtrScope(ptr3);

                    *ptr0 = new float2(rect.x,rect.yMax);
                    *ptr1 = new float2(rect.xMax,rect.yMax);
                    *ptr2 = new float2(rect.x,rect.y);
                    *ptr3 = new float2(rect.xMax,rect.y);
                }
            }
        }

        [Unity.Burst.BurstCompile]
        struct TangentJob : IJobParallelFor
        {
            [ReadOnly] public NativeArray<float2> Size;
            public NativeArray<VertexBuffer> Buffer;
            unsafe void IJobParallelFor.Execute(int billboard)
            {
                float2 size = Size[billboard];
                int vertex = billboard * vertsPerQuad;

                // Tangent data for shader is used to size billboard

                VertexBuffer* quadPtr = (VertexBuffer*)NativeArrayUnsafeUtility.GetUnsafePtr(Buffer) + vertex;

                float4* ptr0 = (float4*)((byte*)(quadPtr + 0) + VertexBuffer.tangentOffset);
                float4* ptr1 = (float4*)((byte*)(quadPtr + 1) + VertexBuffer.tangentOffset);
                float4* ptr2 = (float4*)((byte*)(quadPtr + 2) + VertexBuffer.tangentOffset);
                float4* ptr3 = (float4*)((byte*)(quadPtr + 3) + VertexBuffer.tangentOffset);

                Buffer.AssertPtrScope(ptr0);
                Buffer.AssertPtrScope(ptr1);
                Buffer.AssertPtrScope(ptr2);
                Buffer.AssertPtrScope(ptr3);

                *ptr0 = new float4(size,0,1);
                *ptr1 = new float4(size,1,1);
                *ptr2 = new float4(size,0,0);
                *ptr3 = new float4(size,1,0);
            }
        }

        [Unity.Burst.BurstCompile]
        struct GetBatchDataJob : IJobParallelFor
        {
            [ReadOnly] public NativeArray<BillboardItem> Billboards;
            [ReadOnly] public NativeArray<float2> RecordSize;
            [ReadOnly] public NativeArray<float2> RecordScale;
            [ReadOnly] public NativeArray<Rect> AtlasRects;
            [ReadOnly] public NativeArray<RecordIndex> AtlasIndices;
            public float ScaleDivisor;

            [WriteOnly] public NativeArray<float3> Origin;
            [WriteOnly] public NativeArray<float2> Size;
            [WriteOnly] public NativeArray<Rect> UVRect;
            
            void IJobParallelFor.Execute(int billboard)
            {
                BillboardItem bi = Billboards[billboard];

                float2 size = DaggerfallBillboardBatch.GetScaledBillboardSize(bi.record, RecordSize, RecordScale, ScaleDivisor);
                float3 origin = bi.position + new float3(0, size.y * 0.5f, 0);
                Rect uvrect = AtlasRects[AtlasIndices[bi.record].startIndex + bi.currentFrame];

                Size[billboard] = size;
                UVRect[billboard] = uvrect;
                Origin[billboard] = origin;
            }
        }
        [Unity.Burst.BurstCompile]
        struct GetCustomMaterialBatchDataJob : IJobParallelFor
        {
            [ReadOnly] public NativeArray<BillboardItem> Billboards;
            [ReadOnly] public NativeArray<Rect> AtlasRects;
            [ReadOnly] public NativeArray<RecordIndex> AtlasIndices;

            [WriteOnly] public NativeArray<float3> Origin;
            [WriteOnly] public NativeArray<float2> Size;
            
            void IJobParallelFor.Execute(int billboard)
            {
                BillboardItem bi = Billboards[billboard];

                float2 size = DaggerfallBillboardBatch.GetScaledBillboardSize(bi.customSize, bi.customScale);
                float3 origin = bi.position + new float3(0, size.y * 0.5f, 0);
                Rect uvrect = AtlasRects[AtlasIndices[bi.record].startIndex + bi.currentFrame];

                Size[billboard] = size;
                Origin[billboard] = origin;
            }
        }

        [Unity.Burst.BurstCompile]
        struct BoundsJob : IJob
        {
            public int NumBillboards;
            [ReadOnly] public NativeArray<float3> Origin;
            [ReadOnly] public NativeArray<float2> Size;

            [WriteOnly] public NativeArray<Bounds> AABB;
            void IJob.Execute()
            {
                // Update bounds tracking using actual position and size
                // This can be a little wonky with single billboards side-on as AABB does not rotate
                // But it generally works well for large batches as intended
                // Multiply finalSize * 2f if culling problems with standalone billboards
                AABB[0] = new Bounds();
                if( NumBillboards==0 ) return;
                Bounds aabb = new Bounds(Origin[0], (Vector2)Size[0]);;
                for (int billboard = 0; billboard < NumBillboards; billboard++)
                    aabb.Encapsulate(new Bounds(Origin[billboard], (Vector2)Size[billboard]));
                AABB[0] = aabb;
            }
        }

        [Unity.Burst.BurstCompile]
        struct AddItemsJob : IJobParallelFor
        {
            [ReadOnly] public NativeArray<ItemToAdd> Source;
            [ReadOnly] public NativeArray<int> AtlasFrameCounts;
            public bool RandomStartFrame;
            public uint Seed;
            public float3 BlockOrigin;
            [WriteOnly] public NativeList<BillboardItem>.ParallelWriter BillboardItems;
            void IJobParallelFor.Execute(int index)
            {
                var item = Source[index];

                // Get frame count and start frame
                int frameCount = AtlasFrameCounts[item.record];
                int startFrame = 0;
                if (RandomStartFrame)
                    startFrame = new Unity.Mathematics.Random(Seed * (uint)(index + 1)).NextInt(0, frameCount);

                // Add new billboard to batch
                var billboard = new BillboardItem
                {
                    record          = item.record,
                    position        = BlockOrigin + item.localPosition,
                    totalFrames     = frameCount,
                    currentFrame    = startFrame,
                };
                BillboardItems.AddNoResize(billboard);
            }
        }

        #endregion


        public struct ItemToAdd
        {
            public int record;
            public float3 localPosition;
            public ItemToAdd (int record, float3 localPosition)
            {
                this.record = record;
                this.localPosition = localPosition;
            }
        }

    }
}
