// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Rendering;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallConnect.Utility;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Utility.AssetInjection;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop
{
#if UNITY_EDITOR
    [ExecuteInEditMode]
#endif
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class DaggerfallBillboard : MonoBehaviour
    {
        public int FramesPerSecond = 5;     // General FPS
        public bool OneShot = false;        // Plays animation once then destroys GameObject
        public bool FaceY = false;          // Billboard should also face camera up/down

        [SerializeField]
        BillboardSummary summary = new BillboardSummary();

        public int customArchive = 210;
        public int customRecord = 0;

        Camera mainCamera = null;
        MeshFilter meshFilter = null;
        bool restartAnims = true;
        MeshRenderer meshRenderer;

        // Just using a simple animation speed for simple billboard anims
        // You can adjust this or extend as needed
        const int animalFps = 5;
        const int lightFps = 12;

        public BillboardSummary Summary
        {
            get { return summary; }
        }

        [Serializable]
        public struct BillboardSummary
        {
            public Vector2 Size;                                // Size and scale in world units
            public Rect Rect;                                   // Single UV rectangle for non-atlased materials only
            public Rect[] AtlasRects;                           // Array of UV rectangles for atlased materials only
            public RecordIndex[] AtlasIndices;                  // Indices into UV rect array for atlased materials only, supports animations
            public bool AtlasedMaterial;                        // True if material is part of an atlas
            public bool AnimatedMaterial;                       // True if material uses atlas UV animations (always false for non atlased materials)
            public int CurrentFrame;                            // Current animation frame
            public FlatTypes FlatType;                          // Type of flat
            public EditorFlatTypes EditorFlatType;              // Sub-type of flat when editor/marker
            public bool IsMobile;                               // Billboard is a mobile enemy
            public int Archive;                                 // Texture archive index
            public int Record;                                  // Texture record index
            public int Flags;                                   // NPC Flags found in RMB and RDB NPC data
            public int FactionOrMobileID;                       // FactionID for NPCs, Mobile ID for monsters
            public int NameSeed;                                // NPC name seed
            public MobileTypes FixedEnemyType;                  // Type for fixed enemy marker
            public short WaterLevel;                            // Dungeon water level
            public bool CastleBlock;                            // Non-hostile area of main story castle dungeons
            public BillboardImportedTextures ImportedTextures;  // Textures imported from mods
        }

        void Start()
        {
            if (Application.isPlaying)
            {
                // Get component references
                mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
                meshFilter = GetComponent<MeshFilter>();
                meshRenderer = GetComponent<MeshRenderer>();

                // Hide editor marker from live scene
                bool showEditorFlats = GameManager.Instance.StartGameBehaviour.ShowEditorFlats;
                if (summary.FlatType == FlatTypes.Editor && meshRenderer && !showEditorFlats)
                {
                    // Just disable mesh renderer as actual object can be part of action chain
                    // Example is the treasury in Daggerfall castle, some action records flow through the quest item marker
                    meshRenderer.enabled = false;
                }
            }
        }

        void OnDisable()
        {
            restartAnims = true;
        }

        void Update()
        {
            // Restart animation coroutine if not running
            if (restartAnims && summary.AnimatedMaterial)
            {
                StartCoroutine(AnimateBillboard());
                restartAnims = false;
            }

            // Rotate to face camera in game
            if (mainCamera && Application.isPlaying)
            {
                float y = (FaceY) ? mainCamera.transform.forward.y : 0;
                Vector3 viewDirection = -new Vector3(mainCamera.transform.forward.x, y, mainCamera.transform.forward.z);
                transform.LookAt(transform.position + viewDirection);
            }
        }

        IEnumerator AnimateBillboard()
        {
            while (true)
            {
                float speed = FramesPerSecond;
                if (summary.Archive == Utility.TextureReader.AnimalsTextureArchive) speed = animalFps;
                else if (summary.Archive == Utility.TextureReader.LightsTextureArchive) speed = lightFps;
                if (meshFilter != null)
                {
                    summary.CurrentFrame++;

                    // Original Daggerfall textures
                    if (!summary.ImportedTextures.HasImportedTextures)
                    {
                        if (summary.CurrentFrame >= summary.AtlasIndices[summary.Record].frameCount)
                        {
                            summary.CurrentFrame = 0;
                            if (OneShot)
                                GameObject.Destroy(gameObject);
                        }
                        int index = summary.AtlasIndices[summary.Record].startIndex + summary.CurrentFrame;
                        Rect rect = summary.AtlasRects[index];

                        // Update UVs on mesh
                        Vector2[] uvs = new Vector2[4];
                        uvs[0] = new Vector2(rect.x, rect.yMax);
                        uvs[1] = new Vector2(rect.xMax, rect.yMax);
                        uvs[2] = new Vector2(rect.x, rect.y);
                        uvs[3] = new Vector2(rect.xMax, rect.y);
                        meshFilter.sharedMesh.uv = uvs;
                    }
                    // Custom textures
                    else
                    {
                        // Restart animation or destroy gameobject
                        // The game uses all -and only- textures found on disk, even if they are less or more than vanilla frames
                        if (summary.CurrentFrame >= summary.ImportedTextures.FrameCount)
                        {
                            summary.CurrentFrame = 0;
                            if (OneShot)
                                GameObject.Destroy(gameObject);
                        }

                        // Set imported textures for current frame
                        meshRenderer.material.SetTexture(Uniforms.MainTex, summary.ImportedTextures.Albedo[summary.CurrentFrame]);
                        if (summary.ImportedTextures.IsEmissive)
                            meshRenderer.material.SetTexture(Uniforms.EmissionMap, summary.ImportedTextures.Emission[summary.CurrentFrame]);
                    }
                }

                yield return new WaitForSeconds(1f / speed);
            }
        }

        /// <summary>
        /// Sets extended data about people billboard from RMB resource data.
        /// </summary>
        /// <param name="person"></param>
        public void SetRMBPeopleData(DFBlock.RmbBlockPeopleRecord person)
        {
            SetRMBPeopleData(person.FactionID, person.Flags, person.Position);
        }

        /// <summary>
        /// Sets people data directly.
        /// </summary>
        /// <param name="factionID">FactionID of person.</param>
        /// <param name="flags">Person flags.</param>
        public void SetRMBPeopleData(int factionID, int flags, long position = 0)
        {
            // Add common data
            summary.FactionOrMobileID = factionID;
            summary.FixedEnemyType = MobileTypes.None;
            summary.Flags = flags;

            // TEMP: Add name seed
            summary.NameSeed = (int) position;
        }

        /// <summary>
        /// Sets extended data about billboard from RDB flat resource data.
        /// </summary>
        public void SetRDBResourceData(DFBlock.RdbFlatResource resource)
        {
            // Add common data
            summary.Flags = resource.Flags;
            summary.FactionOrMobileID = (int)resource.FactionOrMobileId;
            summary.FixedEnemyType = MobileTypes.None;

            // TEMP: Add name seed
            summary.NameSeed = (int)resource.Position;

            // Set data of fixed mobile types (e.g. non-random enemy spawn)
            if (resource.TextureArchive == 199)
            {
                if (resource.TextureRecord == 16)
                {
                    summary.IsMobile = true;
                    summary.EditorFlatType = EditorFlatTypes.FixedMobile;
                    summary.FixedEnemyType = (MobileTypes)(summary.FactionOrMobileID & 0xff);
                }
                else if (resource.TextureRecord == 10) // Start marker. Holds data for dungeon block water level and castle block status.
                {
                    if (resource.SoundIndex != 0)
                        summary.WaterLevel = (short)(-8 * resource.SoundIndex);
                    else
                        summary.WaterLevel = 10000; // no water

                    summary.CastleBlock = (resource.Magnitude != 0);
                }
            }
        }

        /// <summary>
        /// Sets new Daggerfall material and recreates mesh.
        /// Will use an atlas if specified in DaggerfallUnity singleton.
        /// </summary>
        /// <param name="dfUnity">DaggerfallUnity singleton. Required for content readers and settings.</param>
        /// <param name="archive">Texture archive index.</param>
        /// <param name="record">Texture record index.</param>
        /// <param name="frame">Frame index.</param>
        /// <returns>Material.</returns>
        public Material SetMaterial(int archive, int record, int frame = 0)
        {
            // Get DaggerfallUnity
            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;
            if (!dfUnity.IsReady)
                return null;

            // Get references
            meshRenderer = GetComponent<MeshRenderer>();

            Vector2 size;
            Mesh mesh = null;
            Material material = null;
            if (dfUnity.MaterialReader.AtlasTextures)
            {
                material = dfUnity.MaterialReader.GetMaterialAtlas(
                    archive,
                    0,
                    4,
                    2048,
                    out summary.AtlasRects,
                    out summary.AtlasIndices,
                    4,
                    true,
                    0,
                    false,
                    true);
                mesh = dfUnity.MeshReader.GetBillboardMesh(
                    summary.AtlasRects[summary.AtlasIndices[record].startIndex],
                    archive,
                    record,
                    out size);
                summary.AtlasedMaterial = true;
                if (summary.AtlasIndices[record].frameCount > 1)
                    summary.AnimatedMaterial = true;
                else
                    summary.AnimatedMaterial = false;
            }
            else
            {
                material = dfUnity.MaterialReader.GetMaterial(
                    archive,
                    record,
                    frame,
                    0,
                    out summary.Rect,
                    4,
                    true,
                    true);
                mesh = dfUnity.MeshReader.GetBillboardMesh(
                    summary.Rect,
                    archive,
                    record,
                    out size);
                summary.AtlasedMaterial = false;
                summary.AnimatedMaterial = false;
            }

            // Set summary
            summary.FlatType = MaterialReader.GetFlatType(archive);
            summary.Archive = archive;
            summary.Record = record;
            summary.Size = size;

            // Set editor flat types
            if (summary.FlatType == FlatTypes.Editor)
                summary.EditorFlatType = MaterialReader.GetEditorFlatType(summary.Record);

            // Set NPC flat type based on archive
            if (RDBLayout.IsNPCFlat(summary.Archive))
                summary.FlatType = FlatTypes.NPC;

            // Assign mesh and material
            MeshFilter meshFilter = GetComponent<MeshFilter>();
            Mesh oldMesh = meshFilter.sharedMesh;
            if (mesh)
            {
                meshFilter.sharedMesh = mesh;
                meshRenderer.sharedMaterial = material;
            }
            if (oldMesh)
            {
                // The old mesh is no longer required
#if UNITY_EDITOR
                DestroyImmediate(oldMesh);
#else
                Destroy(oldMesh);
#endif
            }

            // Import custom textures
            TextureReplacement.SetBillboardImportedTextures(gameObject, ref summary);

            // Standalone billboards never cast shadows
            meshRenderer.shadowCastingMode = ShadowCastingMode.Off;

            // Add NPC trigger collider
            if (summary.FlatType == FlatTypes.NPC)
            {
                Collider col = gameObject.AddComponent<BoxCollider>();
                col.isTrigger = true;
            }

            return material;
        }

        /// <summary>
        /// Aligns billboard to centre of base, rather than exact centre.
        /// Must have already set material using SetMaterial() for billboard dimensions to be known.
        /// </summary>
        public void AlignToBase()
        {
            // Calcuate offset for correct positioning in scene
            Vector3 offset = Vector3.zero;
            offset.y = (summary.Size.y / 2);
            transform.position += offset;
        }

        ///// <summary>
        ///// Gets name of NPC from stored name seed.
        ///// </summary>
        //public string GetRandomNPCName()
        //{
        //    // This is a randomly named NPC from seed values
        //    // TEMP: The correct name seed is not currently known
        //    // Just using record position for now until correct data is found
        //    Genders gender = ((Summary.Flags & 32) == 32) ? Genders.Female : Genders.Male;
        //    DFRandom.srand(Summary.NameSeed);
        //    return DaggerfallUnity.Instance.NameHelper.FullName(NameHelper.BankTypes.Breton, gender);
        //}

#if UNITY_EDITOR
        /// <summary>
        /// Rotate billboards to face editor camera while game not running.
        /// </summary>
        public void OnDrawGizmos()
        {
            if (!Application.isPlaying)
            {
                UnityEditor.SceneView sceneView = GetActiveSceneView();
                if (sceneView)
                {
                    // Editor camera stands in for player camera in edit mode
                    float y = (FaceY) ? mainCamera.transform.forward.y : 0;
                    Vector3 viewDirection = -new Vector3(sceneView.camera.transform.forward.x, y, sceneView.camera.transform.forward.z);
                    transform.LookAt(transform.position + viewDirection);
                }
            }
        }

        private SceneView GetActiveSceneView()
        {
            // Return the focused window if it is a SceneView
            if (EditorWindow.focusedWindow != null && EditorWindow.focusedWindow.GetType() == typeof(SceneView))
                return (SceneView)EditorWindow.focusedWindow;

            return null;
        }
#endif
    }
}