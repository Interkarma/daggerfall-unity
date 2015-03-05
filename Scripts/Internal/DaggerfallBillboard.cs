// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System;
using System.IO;
using DaggerfallConnect;
using DaggerfallConnect.Utility;
using DaggerfallConnect.Arena2;

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

        [SerializeField]
        BillboardSummary summary = new BillboardSummary();

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
            public bool InDungeon;                              // Billboard is inside a dungeon
            public bool IsMobile;                               // Billboard is a mobile enemy
            public int Archive;                                 // Texture archive index
            public int Record;                                  // Texture record index
            public int Gender;                                  // RDB gender field
            public int FactionMobileID;                         // RDB Faction/Mobile ID
            public MobileTypes FixedEnemyType;                  // Type for fixed enemy marker
        }

        void Start()
        {
            if (Application.isPlaying)
            {
                // Set self inactive if this is an editor marker
                if (summary.FlatType == FlatTypes.Editor)
                {
                    this.gameObject.SetActive(false);
                    return;
                }

                // Get component references
                mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
                meshFilter = GetComponent<MeshFilter>();
                meshRenderer = GetComponent<MeshRenderer>();
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
                Vector3 viewDirection = -new Vector3(mainCamera.transform.forward.x, 0, mainCamera.transform.forward.z);
                transform.LookAt(transform.position + viewDirection);
            }
        }

        IEnumerator AnimateBillboard()
        {
            while (true)
            {
                float speed = FramesPerSecond;
                if (summary.Archive == 201) speed = animalFps;
                else if (summary.Archive == 210) speed = lightFps;
                if (meshFilter != null)
                {
                    summary.CurrentFrame++;
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

                yield return new WaitForSeconds(1f / speed);
            }
        }

        /// <summary>
        /// Sets extended data about billboard from RDB flat resource data.
        /// </summary>
        public void SetResourceData(DFBlock.RdbFlatResource resource)
        {
            // Fixed mobile types
            if (resource.TextureArchive == 199 && resource.TextureRecord == 16)
            {
                summary.IsMobile = true;
                summary.Gender = (int)resource.Gender;
                summary.FactionMobileID = (int)resource.FactionMobileId;
                summary.EditorFlatType = EditorFlatTypes.FixedMobile;
                summary.FixedEnemyType = (MobileTypes)(summary.FactionMobileID & 0xff);
            }
            else
            {
                summary.FixedEnemyType = MobileTypes.None;
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
        /// <param name="dungeon">This is a dungeon billboard.</param>
        /// <returns>Material.</returns>
        public Material SetMaterial(DaggerfallUnity dfUnity, int archive, int record, int frame, bool dungeon)
        {
            Shader shader;
            //if (archive == 210 && dungeon) // Dungeon light billboards use unlit billboard shader
            if (archive == 210) // All light billboards use unlit billboard shader
                shader = Shader.Find(dfUnity.MaterialReader.DefaultUnlitBillboardShaderName);
            else
                shader = Shader.Find(dfUnity.MaterialReader.DefaultBillboardShaderName);

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
                    shader);
                mesh = dfUnity.MeshReader.GetBillboardMesh(
                    summary.AtlasRects[summary.AtlasIndices[record].startIndex],
                    archive,
                    record,
                    out size,
                    dungeon);
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
                    shader);
                mesh = dfUnity.MeshReader.GetBillboardMesh(
                    summary.Rect,
                    archive,
                    record,
                    out size,
                    dungeon);
                summary.AtlasedMaterial = false;
                summary.AnimatedMaterial = false;
            }

            // Set summary
            summary.InDungeon = dungeon;
            summary.FlatType = MaterialReader.GetFlatType(archive);
            summary.Archive = archive;
            summary.Record = record;
            summary.Size = size;

            // Set editor flat types
            if (summary.FlatType == FlatTypes.Editor)
                summary.EditorFlatType = MaterialReader.GetEditorFlatType(summary.Record);

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
                DestroyImmediate(oldMesh);
            }

            return material;
        }

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
                    Vector3 viewDirection = -new Vector3(sceneView.camera.transform.forward.x, 0, sceneView.camera.transform.forward.z);
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
