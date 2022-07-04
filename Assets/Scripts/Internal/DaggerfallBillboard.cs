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

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using DaggerfallConnect;
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
    public class DaggerfallBillboard : Billboard
    {
        // Used by DaggerfallBillboardEditor.DisplayAboutGUI()
        public int customArchive = 210;
        public int customRecord = 0;

        // Just using a simple animation speed for simple billboard anims
        // You can adjust this or extend as needed
        const int animalFps = 5;
        const int lightFps = 12;

        Camera mainCamera = null;
        MeshFilter meshFilter = null;
        bool restartAnims = true;
        MeshRenderer meshRenderer;

        int framesPerSecond = 5;     // General FPS
        bool oneShot = false;        // Plays animation once then destroys GameObject
        bool faceY = false;          // Billboard should also face camera up/down

        public override int FramesPerSecond
        {
            get { return framesPerSecond; }
            set { framesPerSecond = value; }
        }

        public override bool OneShot
        {
            get { return oneShot; }
            set { oneShot = value; }
        }

        public override bool FaceY
        {
            get { return faceY; }
            set { faceY = value; }
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
            // Do not rotate if MeshRenderer disabled. The player can't see it anyway and this could be a hidden editor marker with child objects.
            // In the case of hidden editor markers with child treasure objects, we don't want a 3D replacement spinning around like a billboard.
            // Treasure objects are parented to editor marker in this way as the moving action data for treasure is actually on editor marker parent.
            // Visible child of treasure objects have their own MeshRenderer and DaggerfallBillboard to apply rotations.
            if (mainCamera && Application.isPlaying && meshRenderer.enabled)
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
                    summary.CurrentFrame++;
                }

                yield return new WaitForSeconds(1f / speed);
            }
        }

        /// <summary>
        /// Sets extended data about people billboard from RMB resource data.
        /// </summary>
        /// <param name="person"></param>
        public override void SetRMBPeopleData(DFBlock.RmbBlockPeopleRecord person)
        {
            SetRMBPeopleData(person.FactionID, person.Flags, person.Position);
        }

        /// <summary>
        /// Sets people data directly.
        /// </summary>
        /// <param name="factionID">FactionID of person.</param>
        /// <param name="flags">Person flags.</param>
        public override void SetRMBPeopleData(int factionID, int flags, long position = 0)
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
        public override void SetRDBResourceData(DFBlock.RdbFlatResource resource)
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

                    bool isCustomMarker = resource.IsCustomData;
                    if (!isCustomMarker)
                        summary.FixedEnemyType = (MobileTypes)(summary.FactionOrMobileID & 0xff);
                    else
                        summary.FixedEnemyType = (MobileTypes)(summary.FactionOrMobileID);
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
        public override Material SetMaterial(int archive, int record, int frame = 0)
        {
            // Get DaggerfallUnity
            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;
            if (!dfUnity.IsReady)
                return null;

            // Get references
            meshRenderer = GetComponent<MeshRenderer>();

            Vector2 size;
            Vector2 scale;
            Mesh mesh = null;
            Material material = null;
            if (material = TextureReplacement.GetStaticBillboardMaterial(gameObject, archive, record, ref summary, out scale))
            {
                mesh = dfUnity.MeshReader.GetBillboardMesh(summary.Rect, archive, record, out size);
                size *= scale;
                summary.AtlasedMaterial = false;
                summary.AnimatedMaterial = summary.ImportedTextures.FrameCount > 1;
            }
            else if (dfUnity.MaterialReader.AtlasTextures)
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

            // General billboard shadows if enabled
            bool isLightArchive = (archive == TextureReader.LightsTextureArchive);
            meshRenderer.shadowCastingMode = (DaggerfallUnity.Settings.GeneralBillboardShadows && !isLightArchive) ? ShadowCastingMode.TwoSided : ShadowCastingMode.Off;

            // Add NPC trigger collider
            if (summary.FlatType == FlatTypes.NPC)
            {
                Collider col = gameObject.GetComponent<BoxCollider>();
                if(col == null)
                    col = gameObject.AddComponent<BoxCollider>();
                col.isTrigger = true;
            }

            return material;
        }

        /// <summary>
        /// Sets billboard material with a custom texture.
        /// </summary>
        /// <param name="texture">Texture2D to set on material.</param>
        /// <param name="size">Size of billboard quad in normal units (not Daggerfall units).</param>
        /// <returns>Material.</returns>
        public override Material SetMaterial(Texture2D texture, Vector2 size, bool isLightArchive = false)
        {
            // Get DaggerfallUnity
            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;
            if (!dfUnity.IsReady)
                return null;

            // Get references
            meshRenderer = GetComponent<MeshRenderer>();

            // Create material
            Material material = MaterialReader.CreateBillboardMaterial();
            material.mainTexture = texture;

            // Create mesh
            Mesh mesh = dfUnity.MeshReader.GetSimpleBillboardMesh(size);

            // Set summary
            summary.FlatType = FlatTypes.Decoration;
            summary.Size = size;

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

            // General billboard shadows if enabled
            meshRenderer.shadowCastingMode = (DaggerfallUnity.Settings.GeneralBillboardShadows && !isLightArchive) ? ShadowCastingMode.TwoSided : ShadowCastingMode.Off;

            return material;
        }

        /// <summary>
        /// Aligns billboard to centre of base, rather than exact centre.
        /// Must have already set material using SetMaterial() for billboard dimensions to be known.
        /// </summary>
        public override void AlignToBase()
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
        
        /// <summary>
        /// Draws a circle at the bottom of the bilboard to make it easier to judge the size regardless of rotation.
        /// </summary>
        void OnDrawGizmosSelected()
        {
            Vector3 sizeHalf = summary.Size * 0.5f;
            Handles.DrawWireDisc(transform.position - new Vector3(0, sizeHalf.y, 0), Vector3.up, sizeHalf.x);
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
