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
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop
{
    /// <summary>
    /// A multi-orientation mobile unit.
    /// Handles loading resources and rendering billboard based on orientation and state.
    /// Will rotate and scale based on camera angle and texture, so this component should
    /// only be attached to a child GameObject.
    /// Uses parent GameObject to determine actual facing in world.
    /// </summary>
    #if UNITY_EDITOR
    [ExecuteInEditMode]
    #endif
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class DaggerfallMobileUnit : MonoBehaviour
    {
        [SerializeField]
        MobileUnitSummary summary = new MobileUnitSummary();

        Camera mainCamera = null;
        MeshFilter meshFilter = null;

        Vector3 cameraPosition;
        float enemyFacingAngle;
        int lastOrientation;
        int currentFrame;
        bool restartAnims = true;

        public MobileUnitSummary Summary
        {
            get { return summary; }
        }

        [Serializable]
        public struct MobileUnitSummary
        {
            public Rect[] AtlasRects;                           // Array of rectangles for atlased materials
            public RecordIndex[] AtlasIndices;                  // Indices into rect array for atlased materials, supports animations
            public Vector2[] RecordSizes;                       // Size and scale of individual records
            public int[] RecordFrames;                          // Number of frames of individual records
            public MobileEnemy Enemy;                           // Mobile enemy settings
            public MobileStates EnemyState;                     // Animation state
            public MobileAnimation[] StateAnims;                // Animation frames for this state
        }

        void Start()
        {
            if (Application.isPlaying)
            {
                // Get component references
                mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
                meshFilter = GetComponent<MeshFilter>();
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
                StartCoroutine(AnimateEnemy());
                restartAnims = false;
            }

            // Rotate to face camera in game
            if (mainCamera && Application.isPlaying)
            {
                // Rotate billboard based on camera facing
                cameraPosition = mainCamera.transform.position;
                Vector3 viewDirection = -new Vector3(mainCamera.transform.forward.x, 0, mainCamera.transform.forward.z);
                transform.LookAt(transform.position + viewDirection);

                // Orient enemy based on camera position
                UpdateOrientation();
            }
        }

        /// <summary>
        /// Sets new enemy type.
        /// </summary>
        /// <param name="dfUnity">DaggerfallUnity singleton. Required for content readers and settings.</param>
        /// <param name="enemyType">Enemy type.</param>
        public void SetEnemy(DaggerfallUnity dfUnity, MobileEnemy enemy, MobileReactions reaction)
        {
            // Initial enemy settings
            summary.Enemy = enemy;
            summary.EnemyState = MobileStates.Idle;
            summary.Enemy.Reactions = reaction;

            // Load enemy content
            int archive = GetTextureArchive();
            CacheRecordSizesAndFrames(dfUnity, archive);
            AssignMeshAndMaterial(dfUnity, archive);
            ApplyEnemyState();
        }

        /// <summary>
        /// Sets a new enemy state and restarts frame counter.
        /// Certain states are one-shot only (such as attack and hurt) and return to idle when completed.
        /// Continuous states (such as move) will keep looping until changed.
        /// </summary>
        /// <param name="state">New state.</param>
        /// <returns>Frames per second of new state.</returns>
        public float ChangeEnemyState(MobileStates state)
        {
            // Only change if in a different state
            if (summary.EnemyState != state)
            {
                summary.EnemyState = state;
                currentFrame = 0;
                ApplyEnemyState();
            }

            return summary.StateAnims[(int)summary.EnemyState].FramePerSecond;
        }

        /// <summary>
        /// Gets true if playing a one-shot animation like attack and hurt.
        /// </summary>
        /// <returns>True if playing one-shot animation.</returns>
        public bool IsPlayingOneShot()
        {
            if (summary.EnemyState == MobileStates.Hurt ||
                summary.EnemyState == MobileStates.PrimaryAttack ||
                summary.EnemyState == MobileStates.RangedAttack1 ||
                summary.EnemyState == MobileStates.RangedAttack2)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets true if playing any attack animation.
        /// </summary>
        /// <returns>True if playing attack animation.</returns>
        public bool IsAttacking()
        {
            if (summary.EnemyState == MobileStates.PrimaryAttack ||
                summary.EnemyState == MobileStates.RangedAttack1 ||
                summary.EnemyState == MobileStates.RangedAttack2)
            {
                return true;
            }

            return false;
        }

        #region Private Methods

        /// <summary>
        /// Updates enemy state based on current settings.
        /// Called automatially by SetEnemyType().
        /// This should be called after changing enemy state (e.g. from in code or in editor).
        /// </summary>
        private void ApplyEnemyState()
        {
            // Get state animations
            summary.StateAnims = GetStateAnims(summary.EnemyState);
            if (summary.StateAnims == null)
            {
                // Log error message
                DaggerfallUnity.LogMessage(string.Format("DaggerfalMobileUnit: Enemy does not have animation for {0} state. Defaulting to Idle state.", summary.EnemyState.ToString()), true);

                // Set back to idle (which every enemy has in one form or another)
                summary.EnemyState = MobileStates.Idle;
                summary.StateAnims = GetStateAnims(summary.EnemyState);
            }

            // Orient enemy relative to camera
            UpdateOrientation();
        }

        /// <summary>
        /// Updates enemy orientation based on angle between enemy and camera.
        /// </summary>
        private void UpdateOrientation()
        {
            // Get direction normal to camera, ignore y axis
            Vector3 dir = Vector3.Normalize(
                new Vector3(cameraPosition.x, 0, cameraPosition.z) -
                new Vector3(transform.position.x, 0, transform.position.z));

            // Get parent forward normal, ignore y axis
            Vector3 parentForward = transform.parent.forward;
            parentForward.y = 0;

            // Get angle and cross product for left/right angle
            enemyFacingAngle = Vector3.Angle(dir, parentForward);
            enemyFacingAngle = enemyFacingAngle * -Mathf.Sign(Vector3.Cross(dir, parentForward).y);

            // Hand-tune facing index
            int orientation = 0;

            // Right-hand side
            if (enemyFacingAngle > 0.0f && enemyFacingAngle < 22.5f)
                orientation = 0;
            if (enemyFacingAngle > 22.5f && enemyFacingAngle < 67.5f)
                orientation = 7;
            if (enemyFacingAngle > 67.5f && enemyFacingAngle < 112.5f)
                orientation = 6;
            if (enemyFacingAngle > 112.5f && enemyFacingAngle < 157.5f)
                orientation = 5;
            if (enemyFacingAngle > 157.5f && enemyFacingAngle < 180.0f)
                orientation = 4;

            // Left-hand side
            if (enemyFacingAngle < 0.0f && enemyFacingAngle > -22.5f)
                orientation = 0;
            if (enemyFacingAngle < -22.5f && enemyFacingAngle > -67.5f)
                orientation = 1;
            if (enemyFacingAngle < -67.5f && enemyFacingAngle > -112.5f)
                orientation = 2;
            if (enemyFacingAngle < -112.5f && enemyFacingAngle > -157.5f)
                orientation = 3;
            if (enemyFacingAngle < -157.5f && enemyFacingAngle > -180.0f)
                orientation = 4;

            // Change enemy to this orientation
            if (orientation != lastOrientation)
                OrientEnemy(orientation);
        }

        /// <summary>
        /// Sets enemy orientation index.
        /// </summary>
        /// <param name="orientation">New orientation index.</param>
        private void OrientEnemy(int orientation)
        {
            // Get mesh filter
            if (meshFilter == null)
                meshFilter = GetComponent<MeshFilter>();

            // Try to fix if anim array is null
            if (summary.StateAnims == null)
                ApplyEnemyState();
            
            // Get enemy size and scale for this state
            int record = summary.StateAnims[orientation].Record;
            Vector2 size = summary.RecordSizes[record];

            // Post-fix female thief scale for orentations 1 and 7
            // The scale read from Daggerfall's files is too small
            if ((MobileTypes)summary.Enemy.ID == MobileTypes.Thief &&
                summary.Enemy.Gender == MobileGender.Female &&
                (orientation == 1 || orientation == 7))
            {
                size *= 1.35f;
            }

            // Ensure walking enemies keep their feet aligned between states
            if (summary.Enemy.Behaviour != MobileBehaviour.Flying &&
                summary.Enemy.Behaviour != MobileBehaviour.Aquatic &&
                summary.EnemyState != MobileStates.Idle)
            {
                Vector2 idleSize = summary.RecordSizes[0];
                Vector2 stateSize = summary.RecordSizes[record];
                transform.localPosition = new Vector3(0f, (stateSize.y - idleSize.y) * 0.5f, 0f);
            }
            else
            {
                transform.localPosition = Vector3.zero;
            }

            // Set mesh scale for this state
            transform.localScale = new Vector3(size.x, size.y, 1);
            Rect rect = summary.AtlasRects[summary.AtlasIndices[record].startIndex + currentFrame];

            // Check if orientation flip needed
            bool flip = summary.StateAnims[orientation].FlipLeftRight;

            // Rat Idle animation needs to be inverted
            if (summary.Enemy.ID == (int)MobileTypes.Rat && summary.EnemyState == MobileStates.Idle)
                flip = !flip;

            // Scorpion Idle and Move animations need to be inverted
            if (summary.Enemy.ID == (int)MobileTypes.GiantScorpion &&
                summary.EnemyState == MobileStates.Idle || summary.EnemyState == MobileStates.Move)
                flip = !flip;

            // Update UVs on mesh
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

            // Assign new orientation
            lastOrientation = orientation;
        }

        IEnumerator AnimateEnemy()
        {
            while (true)
            {
                float fps = 10;
                if (summary.StateAnims != null)
                {
                    // Step frame
                    currentFrame++;
                    if (currentFrame >= summary.StateAnims[lastOrientation].NumFrames)
                    {
                        if (IsPlayingOneShot())
                            ChangeEnemyState(MobileStates.Idle);    // If this is a one-shot anim, revert back to idle state at end
                        else
                            currentFrame = 0;                       // Otherwise keep looping frames
                    }

                    // Update enemy and fps
                    OrientEnemy(lastOrientation);
                    fps = summary.StateAnims[lastOrientation].FramePerSecond;
                }

                yield return new WaitForSeconds(1f / fps);
            }
        }

        #endregion

        #region Content Loading

        /// <summary>
        /// Get texture archive index based on gender.
        /// Assigns random gender for humans enemies with unspecified gender.
        /// </summary>
        /// <returns>Texture archive index.</returns>
        private int GetTextureArchive()
        {
            // If human with unspecified gender then randomise gender
            if (summary.Enemy.Affinity == MobileAffinity.Human && summary.Enemy.Gender == MobileGender.Unspecified)
            {
                if (DFRandom.random_range(0, 1) == 0)
                    summary.Enemy.Gender = MobileGender.Male;
                else
                    summary.Enemy.Gender = MobileGender.Female;
            }

            // Monster genders are always unspecified as there is no male/female variant
            if (summary.Enemy.Gender == MobileGender.Male || summary.Enemy.Gender == MobileGender.Unspecified)
                return summary.Enemy.MaleTexture;
            else
                return summary.Enemy.FemaleTexture;
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
            TextureFile textureFile = new TextureFile(path, FileUsage.UseMemory, true);

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

                // Store final size and frame count
                summary.RecordSizes[i] = finalSize * MeshReader.GlobalScale;
                summary.RecordFrames[i] = textureFile.GetFrameCount(i);
            }
        }

        /// <summary>
        /// Creates mesh and material for this enemy.
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

            // Load material atlas
            Material material = dfUnity.MaterialReader.GetMaterialAtlas(
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
                Shader.Find(dfUnity.MaterialReader.DefaultBillboardShaderName));

            // Set new enemy material
            GetComponent<MeshRenderer>().sharedMaterial = material;
        }

        /// <summary>
        /// Gets cloned animation set for specified state.
        /// </summary>
        /// <param name="state">Enemy state.</param>
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
                case MobileStates.PrimaryAttack:
                    anims = (MobileAnimation[])EnemyBasics.PrimaryAttackAnims.Clone();
                    break;
                case MobileStates.Hurt:
                    anims = (MobileAnimation[])EnemyBasics.HurtAnims.Clone();
                    break;
                case MobileStates.Idle:
                    anims = (summary.Enemy.HasIdle) ? (MobileAnimation[])EnemyBasics.IdleAnims.Clone() : (MobileAnimation[])EnemyBasics.MoveAnims.Clone();
                    break;
                case MobileStates.RangedAttack1:
                    anims = (summary.Enemy.HasRangedAttack1) ? (MobileAnimation[])EnemyBasics.RangedAttack1Anims.Clone() : null;
                    break;
                case MobileStates.RangedAttack2:
                    anims = (summary.Enemy.HasRangedAttack2) ? (MobileAnimation[])EnemyBasics.RangedAttack2Anims.Clone() : null;
                    break;
                default:
                    return null;
            }

            // Assign number number of frames per anim
            for (int i = 0; i < anims.Length; i++)
                anims[i].NumFrames = summary.RecordFrames[anims[i].Record];
            
            return anims;
        }

        #endregion

#if UNITY_EDITOR
        /// <summary>
        /// Rotate enemy to face editor camera while game not running.
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