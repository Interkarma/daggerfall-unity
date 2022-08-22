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

using System.Collections;
using System.Collections.Generic;
using System.IO;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Utility.AssetInjection;
using UnityEditor;
using UnityEngine;

namespace DaggerfallWorkshop
{
#if UNITY_EDITOR
    [ExecuteInEditMode]
#endif
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class DaggerfallPetMobileUnit : PetMobileUnit
    {
        const int numberOrientations = 8;
        const float anglePerOrientation = 360f / numberOrientations;

        Camera mainCamera = null;
        MeshFilter meshFilter = null;
        MeshRenderer meshRenderer = null;

        Vector3 cameraPosition;
        float petFacingAngle;
        int lastOrientation;
        int currentFrame;
        int frameIterator;
        bool restartAnims = true;
        bool freezeAnims = false;
        bool animReversed = false;
        int frameSpeedDivisor = 1;

        public override bool IsSetup
        {
            get { return summary.IsSetup; }
            protected set { summary.IsSetup = value; }
        }

        public int CurrentFrame
        {
            get { return currentFrame; }
        }
        public int FrameSpeedDivisor
        {
            get { return frameSpeedDivisor; }
            set { frameSpeedDivisor = (value < 1) ? 1 : value; }
        }

        public override bool FreezeAnims
        {
            get { return freezeAnims; }
            set { freezeAnims = value; }
        }

        public override MobilePet Pet
        {
            get { return summary.Pet; }
            protected set { summary.Pet = value; }
        }

        public override MobileStates PetState
        {
            get { return summary.PetState; }
            protected set { summary.PetState = value; }
        }

        public override byte ClassicSpawnDistanceType
        {
            get { return summary.ClassicSpawnDistanceType; }
            protected set { summary.ClassicSpawnDistanceType = value; }
        }


        public override bool SpecialTransformationCompleted
        {
            get { return summary.specialTransformationCompleted; }
            protected set { summary.specialTransformationCompleted = value; }
        }

        void Start()
        {
            if (Application.isPlaying)
            {
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
            if (restartAnims)
            {
                StartCoroutine(AnimatePet());
                restartAnims = false;
            }

            // Rotate to face camera in game
            if (mainCamera && Application.isPlaying)
            {
                // Rotate billboard based on camera facing
                cameraPosition = mainCamera.transform.position;
                Vector3 viewDirection = -new Vector3(mainCamera.transform.forward.x, 0, mainCamera.transform.forward.z);
                transform.LookAt(transform.position + viewDirection);

                // Orient pet based on camera position
                UpdateOrientation();
            }
        }

        public override Vector3 GetSize()
        {
            return summary.RecordSizes[0];
        }

        protected override void ApplyPet(DaggerfallUnity dfUnity)
        {
            // Load pet content
            int archive = GetTextureArchive();
            CacheRecordSizesAndFrames(dfUnity, archive);
            AssignMeshAndMaterial(dfUnity, archive);

            // Apply pet state and update orientation
            lastOrientation = -1;
            ApplyPetState();
        }

        protected override void ApplyPetStateChange(MobileStates currentState, MobileStates newState)
        {
            bool resetFrame = true;

            if (!summary.Pet.HasIdle &&
                ((currentState == MobileStates.Idle && newState == MobileStates.Move) ||
                 (currentState == MobileStates.Move && newState == MobileStates.Idle)))
                resetFrame = false;

            if (resetFrame)
            {
                currentFrame = 0;
                animReversed = false;
            }

            ApplyPetState();
        }

        #region Private Methods

        /// <summary>
        /// Updates pet state based on current settings.
        /// Called automatially by SetPetType().
        /// This should be called after changing pet state (e.g. from in code or in editor).
        /// </summary>
        private void ApplyPetState()
        {
            // Get state animations
            summary.StateAnims = GetStateAnims(summary.PetState);

            // Set to the first frame of this animation, and prepare frameIterator to start from the second frame when AnimatePet() next runs
            currentFrame = 0;
            frameIterator = 1;

            // Orient pet relative to camera
            UpdateOrientation();
        }

        /// <summary>
        /// Updates pet orientation based on angle between pet and camera.
        /// </summary>
        private void UpdateOrientation()
        {
            Transform parent = transform.parent;
            if (parent == null)
                return;

            // Get direction normal to camera, ignore y axis
            Vector3 dir = Vector3.Normalize(
                new Vector3(cameraPosition.x, 0, cameraPosition.z) -
                new Vector3(transform.position.x, 0, transform.position.z));

            // Get parent forward normal, ignore y axis
            Vector3 parentForward = transform.parent.forward;
            parentForward.y = 0;

            // Get angle and cross product for left/right angle
            petFacingAngle = Vector3.Angle(dir, parentForward);
            petFacingAngle = petFacingAngle * -Mathf.Sign(Vector3.Cross(dir, parentForward).y);

            // Facing index
            int orientation = -Mathf.RoundToInt(petFacingAngle / anglePerOrientation);
            // Wrap values to 0 .. numberOrientations-1
            orientation = (orientation + numberOrientations) % numberOrientations;

            // Change pet to this orientation
            if (orientation != lastOrientation)
                OrientPet(orientation);
        }

        /// <summary>
        /// Sets pet orientation index.
        /// </summary>
        /// <param name="orientation">New orientation index.</param>
        private void OrientPet(int orientation)
        {
            if (summary.StateAnims == null || summary.StateAnims.Length == 0)
                return;

            // Get mesh filter
            if (meshFilter == null)
                meshFilter = GetComponent<MeshFilter>();

            // Try to fix if anim array is null
            if (summary.StateAnims == null)
                ApplyPetState();

            // Get pet size and scale for this state
            int record = summary.StateAnims[orientation].Record;
            summary.AnimStateRecord = record;
            Vector2 size = summary.RecordSizes[record];

            // Post-fix female texture scale for 475 while casting spells
            // The scale read from Daggerfall's files is too small 
            if (summary.Pet.FemaleTexture == 475 &&
                summary.Pet.Gender == MobileGender.Female &&
                record >= 20 && record <= 24)
            {
                size *= 1.35f;
            }

            // Ensure walking enemies keep their feet aligned between states
            if (summary.PetState != MobileStates.Idle)
            {
                Vector2 idleSize = summary.RecordSizes[0];
                transform.localPosition = new Vector3(0f, (size.y - idleSize.y) * 0.5f, 0f);
            }
            else
            {
                transform.localPosition = Vector3.zero;
            }

            // Set mesh scale for this state
            transform.localScale = new Vector3(size.x, size.y, 1);

            // Check if orientation flip needed
            bool flip = summary.StateAnims[orientation].FlipLeftRight;

            // Update Record/Frame texture
            if (summary.ImportedTextures.HasImportedTextures)
            {
                if (meshRenderer == null)
                    meshRenderer = GetComponent<MeshRenderer>();

                // Assign imported texture
                meshRenderer.material.mainTexture = summary.ImportedTextures.Albedo[record][currentFrame];
                if (summary.ImportedTextures.IsEmissive)
                    meshRenderer.material.SetTexture(Uniforms.EmissionMap, summary.ImportedTextures.EmissionMaps[record][currentFrame]);

                // Update UVs on mesh
                Vector2[] uvs = new Vector2[4];
                if (flip)
                {
                    uvs[0] = new Vector2(1, 1);
                    uvs[1] = new Vector2(0, 1);
                    uvs[2] = new Vector2(1, 0);
                    uvs[3] = new Vector2(0, 0);
                }
                else
                {
                    uvs[0] = new Vector2(0, 1);
                    uvs[1] = new Vector2(1, 1);
                    uvs[2] = new Vector2(0, 0);
                    uvs[3] = new Vector2(1, 0);
                }
                meshFilter.sharedMesh.uv = uvs;
            }
            else
            {
                // Daggerfall Atlas: Update UVs on mesh
                Rect rect = summary.AtlasRects[summary.AtlasIndices[record].startIndex + currentFrame];
                Vector2[] uvs = new Vector2[4];
                if (flip)
                {
                    uvs[0] = new Vector2(rect.xMax, rect.yMax);
                    uvs[1] = new Vector2(rect.x, rect.yMax);
                    uvs[2] = new Vector2(rect.xMax, rect.y);
                    uvs[3] = new Vector2(rect.x, rect.y);
                }
                else
                {
                    uvs[0] = new Vector2(rect.x, rect.yMax);
                    uvs[1] = new Vector2(rect.xMax, rect.yMax);
                    uvs[2] = new Vector2(rect.x, rect.y);
                    uvs[3] = new Vector2(rect.xMax, rect.y);
                }
                meshFilter.sharedMesh.uv = uvs;
            }

            // Assign new orientation
            lastOrientation = orientation;
        }

        IEnumerator AnimatePet()
        {
            float fps = 10;
            while (true)
            {
                if (!freezeAnims && summary.IsSetup && summary.StateAnims != null && summary.StateAnims.Length > 0)
                {
                    // Update pet and fps
                    OrientPet(lastOrientation);
                    fps = summary.StateAnims[lastOrientation].FramePerSecond / FrameSpeedDivisor;

                    // Enforce a lower limit on animation speed when using a custom FrameSpeedDivisor
                    if (FrameSpeedDivisor > 1 && fps < 4)
                        fps = 4;


                    currentFrame =0;

                    // Set boolean if frame to attempt to apply damage or shoot arrow is encountered, and proceed to next frame if it exists
                    if (currentFrame == -1)
                    {
                        if (frameIterator < summary.StateAnimFrames.Length)
                            currentFrame = summary.StateAnimFrames[frameIterator++];
                    }


                    if (currentFrame >= summary.StateAnims[lastOrientation].NumFrames ||
                        animReversed && currentFrame <= 0)
                    {

                        // Otherwise keep looping frames
                        bool bounceAnim = summary.StateAnims[lastOrientation].BounceAnim;
                        if (bounceAnim && !animReversed)
                        {
                            currentFrame = summary.StateAnims[lastOrientation].NumFrames - 2;
                            animReversed = true;
                        }
                        else
                        {
                            currentFrame = 0;
                            animReversed = false;
                        }
                    }
                }

                yield return new WaitForSeconds(1f / fps);
            }
        }
        #endregion

        #region Content Loading

        /// <summary>
        /// Get texture archive index based on gender.
        /// </summary>
        /// <returns>Texture archive index.</returns>
        private int GetTextureArchive()
        {
            // If human with unspecified gender then randomise gender
            if (summary.Pet.Affinity == MobileAffinity.Human && summary.Pet.Gender == MobileGender.Unspecified)
            {
                if (DFRandom.random_range(0, 2) == 0)
                    summary.Pet.Gender = MobileGender.Male;
                else
                    summary.Pet.Gender = MobileGender.Female;
            }

            // Monster genders are always unspecified as there is no male/female variant
            if (summary.Pet.Gender == MobileGender.Male || summary.Pet.Gender == MobileGender.Unspecified)
                return summary.Pet.MaleTexture;
            else
                return summary.Pet.FemaleTexture;
        }

        /// <summary>
        /// Precalculate and cache billboard scale for every record.
        /// This will change based on animation state and orientation.
        /// Cache this to array so it only needs to be calculated once.
        /// Also store number of frames for state animations.
        /// </summary>
        /// <param name="dfUnity">DaggerfallUnity singleton. Required for content readers and settings.</param>
        /// <param name="archive">Texture archive index derived from type and gender.</param>
        private void CacheRecordSizesAndFrames(DaggerfallUnity dfUnity, int archive)
        {
            // Open texture file
            string path = Path.Combine(dfUnity.Arena2Path, TextureFile.IndexToFileName(archive));
            TextureFile textureFile = new TextureFile();

            // Might be updated later through texture replacement
            if (!textureFile.Load(path, FileUsage.UseMemory, true))
                return;

            // Cache size and scale for each record
            summary.RecordSizes = new Vector2[textureFile.RecordCount];
            summary.RecordFrames = new int[textureFile.RecordCount];
            for (int i = 0; i < textureFile.RecordCount; i++)
            {
                // Get size and scale of this texture
                DFSize size = textureFile.GetSize(i);
                DFSize scale = textureFile.GetScale(i);

                // Set start size
                Vector2 startSize;
                startSize.x = size.Width;
                startSize.y = size.Height;

                // Apply scale
                Vector2 finalSize;
                int xChange = (int)(size.Width * (scale.Width / BlocksFile.ScaleDivisor));
                int yChange = (int)(size.Height * (scale.Height / BlocksFile.ScaleDivisor));
                finalSize.x = (size.Width + xChange);
                finalSize.y = (size.Height + yChange);

                // Set optional scale
                TextureReplacement.SetBillboardScale(archive, i, ref finalSize);

                // Store final size and frame count
                summary.RecordSizes[i] = finalSize * MeshReader.GlobalScale;
                summary.RecordFrames[i] = textureFile.GetFrameCount(i);
            }
        }

        /// <summary>
        /// Creates mesh and material for this pet.
        /// </summary>
        /// <param name="dfUnity">DaggerfallUnity singleton. Required for content readers and settings.</param>
        /// <param name="archive">Texture archive index derived from type and gender.</param>
        private void AssignMeshAndMaterial(DaggerfallUnity dfUnity, int archive)
        {
            // Get mesh filter
            if (meshFilter == null)
                meshFilter = GetComponent<MeshFilter>();

            // Vertices for a 1x1 unit quad
            // This is scaled to correct size depending on facing and orientation
            float hx = 0.5f, hy = 0.5f;
            Vector3[] vertices = new Vector3[4];
            vertices[0] = new Vector3(hx, hy, 0);
            vertices[1] = new Vector3(-hx, hy, 0);
            vertices[2] = new Vector3(hx, -hy, 0);
            vertices[3] = new Vector3(-hx, -hy, 0);

            // Indices
            int[] indices = new int[6]
                {
                    0, 1, 2,
                    3, 2, 1,
                };

            // Normals
            Vector3 normal = Vector3.Normalize(Vector3.up + Vector3.forward);
            Vector3[] normals = new Vector3[4];
            normals[0] = normal;
            normals[1] = normal;
            normals[2] = normal;
            normals[3] = normal;

            // Create mesh
            Mesh mesh = new Mesh();
            mesh.name = string.Format("MobileEnemyMesh");
            mesh.vertices = vertices;
            mesh.triangles = indices;
            mesh.normals = normals;

            // Assign mesh
            meshFilter.sharedMesh = mesh;
            dfUnity.EditorUpdate();

            // Create material
            Material material = TextureReplacement.GetMobileBillboardMaterial(archive, GetComponent<MeshFilter>(), ref summary.ImportedTextures) ??
                dfUnity.MaterialReader.GetMaterialAtlas(
                archive,
                0,
                4,
                1024,
                out summary.AtlasRects,
                out summary.AtlasIndices,
                4,
                true,
                0,
                false,
                true);

            // Update cached record values in case of non-classic texture
            if (summary.RecordSizes == null || summary.RecordSizes.Length == 0)
            {
                if (summary.ImportedTextures.Albedo != null && summary.ImportedTextures.Albedo.Length > 0)
                {
                    int recordCount = summary.ImportedTextures.Albedo.Length;

                    // Cache size and scale for each record
                    summary.RecordSizes = new Vector2[recordCount];
                    summary.RecordFrames = new int[recordCount];
                    for (int i = 0; i < recordCount; i++)
                    {
                        // Get size and scale of this texture
                        Texture2D firstFrame = summary.ImportedTextures.Albedo[i][0];

                        Vector2 size = new Vector2(firstFrame.width, firstFrame.height);

                        // Set optional scale
                        TextureReplacement.SetBillboardScale(archive, i, ref size);

                        // Store final size and frame count
                        summary.RecordSizes[i] = size * MeshReader.GlobalScale;
                        summary.RecordFrames[i] = summary.ImportedTextures.Albedo[i].Length;
                    }
                }
                else
                {
                    Debug.LogError($"Texture archive {archive} has no valid records");
                }
            }

            // Set new pet material
            GetComponent<MeshRenderer>().sharedMaterial = material;
        }

        /// <summary>
        /// Gets cloned animation set for specified state.
        /// </summary>
        /// <param name="state">pet state.</param>
        /// <returns>Array of mobile animations.</returns>
        private MobileAnimation[] GetStateAnims(MobileStates state)
        {
            // Clone static animation state
            MobileAnimation[] anims;
            switch (state)
            {
                case MobileStates.Move:
                    anims = (MobileAnimation[])EnemyBasics.MoveAnims.Clone();
                    break;
                case MobileStates.Hurt:
                    anims = (MobileAnimation[])EnemyBasics.HurtAnims.Clone();
                    break;
                case MobileStates.Idle:
                    if (!summary.Pet.HasIdle)
                        anims = (MobileAnimation[])EnemyBasics.MoveAnims.Clone();
                    else
                        anims = (MobileAnimation[])EnemyBasics.IdleAnims.Clone();
                    break;
                default:
                    return null;
            }

            // Assign number of frames per anim
            for (int i = 0; i < anims.Length; i++)
                anims[i].NumFrames = summary.RecordFrames[anims[i].Record];

            return anims;
        }

        #endregion

#if UNITY_EDITOR
        /// <summary>
        /// Rotate pet to face editor camera while game not running.
        /// </summary>
        public void OnDrawGizmos()
        {
            if (!Application.isPlaying)
            {
                // Rotate to face camera
                UnityEditor.SceneView sceneView = GetActiveSceneView();
                if (sceneView)
                {
                    // Editor camera stands in for player camera in edit mode
                    cameraPosition = sceneView.camera.transform.position;
                    Vector3 viewDirection = -new Vector3(sceneView.camera.transform.forward.x, 0, sceneView.camera.transform.forward.z);
                    transform.LookAt(transform.position + viewDirection);
                    UpdateOrientation();
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