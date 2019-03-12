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

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using DaggerfallConnect;
using DaggerfallConnect.Utility;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Utility.AssetInjection;
using DaggerfallWorkshop.Game.Utility;

namespace DaggerfallWorkshop
{
    /// <summary>
    /// A billboard class for classic Daggerfall mobile sprites with 8 orientations.
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
        MeshRenderer meshRenderer = null;

        Vector3 cameraPosition;
        float enemyFacingAngle;
        int lastOrientation;
        int currentFrame;
        int frameIterator;
        bool doMeleeDamage = false;
        bool shootArrow = false;
        bool restartAnims = true;
        bool freezeAnims = false;

        public MobileUnitSummary Summary
        {
            get { return summary; }
        }

        public int CurrentFrame
        {
            get { return currentFrame; }
        }

        public bool DoMeleeDamage
        {
            get { return doMeleeDamage; }
            set { doMeleeDamage = value; }
        }

        public bool ShootArrow
        {
            get { return shootArrow; }
            set { shootArrow = value; }
        }

        public bool FreezeAnims
        {
            get { return freezeAnims; }
            set { freezeAnims = value; }
        }

        [Serializable]
        public struct MobileUnitSummary
        {
            public bool IsSetup;                                        // Flagged true when mobile settings are populated
            public Rect[] AtlasRects;                                   // Array of rectangles for atlased materials
            public RecordIndex[] AtlasIndices;                          // Indices into rect array for atlased materials, supports animations
            public Vector2[] RecordSizes;                               // Size and scale of individual records
            public int[] RecordFrames;                                  // Number of frames of individual records
            public MobileEnemy Enemy;                                   // Mobile enemy settings
            public MobileStates EnemyState;                             // Animation state
            public MobileAnimation[] StateAnims;                        // Animation frames for this state
            public MobileBillboardImportedTextures ImportedTextures;    // Textures imported from mods
            public int AnimStateRecord;                                 // Record number of animation state
            public int[] StateAnimFrames;                               // Sequence of frames to play for this animation. Used for attacks
            public byte ClassicSpawnDistanceType;                       // 0 through 6 value read from spawn marker that determines distance at which enemy spawns/despawns in classic.
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
        public void SetEnemy(DaggerfallUnity dfUnity, MobileEnemy enemy, MobileReactions reaction, byte classicSpawnDistanceType)
        {
            // Initial enemy settings
            summary.Enemy = enemy;
            summary.EnemyState = MobileStates.Move;
            summary.Enemy.Reactions = reaction;

            // Load enemy content
            int archive = GetTextureArchive();
            CacheRecordSizesAndFrames(dfUnity, archive);
            AssignMeshAndMaterial(dfUnity, archive);

            // Apply enemy state and update orientation
            lastOrientation = -1;
            ApplyEnemyState();

            // Set initial contact range
            summary.ClassicSpawnDistanceType = classicSpawnDistanceType;

            // Raise setup flag
            summary.IsSetup = true;
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
            // Don't change state during animation freeze
            if (freezeAnims)
                return 0;

            // Don't reset frame to 0 for idle/move switches for enemies without idle animations
            bool resetFrame = true;

            if (!summary.Enemy.HasIdle &&
                ((summary.EnemyState == MobileStates.Idle && state == MobileStates.Move) ||
                (summary.EnemyState == MobileStates.Move && state == MobileStates.Idle)))
                resetFrame = false;

            // Only change if in a different state
            if (summary.EnemyState != state)
            {
                summary.EnemyState = state;
                if (resetFrame)
                    currentFrame = 0;
                ApplyEnemyState();
            }

            // Cannot set anims is not setup
            if (!summary.IsSetup)
                return 0;

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
                summary.EnemyState == MobileStates.RangedAttack2 ||
                summary.EnemyState == MobileStates.Spell)
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
            if (summary.EnemyState == MobileStates.PrimaryAttack)
            {
                int random = Dice100.Roll();

                if (random <= summary.Enemy.ChanceForAttack2)
                    summary.StateAnimFrames = summary.Enemy.PrimaryAttackAnimFrames2;
                else
                {
                    random -= summary.Enemy.ChanceForAttack2;
                    if (random <= summary.Enemy.ChanceForAttack3)
                        summary.StateAnimFrames = summary.Enemy.PrimaryAttackAnimFrames3;
                    else
                    {
                        random -= summary.Enemy.ChanceForAttack3;
                        if (random <= summary.Enemy.ChanceForAttack4)
                            summary.StateAnimFrames = summary.Enemy.PrimaryAttackAnimFrames4;
                        else
                        {
                            random -= summary.Enemy.ChanceForAttack4;
                            if (random <= summary.Enemy.ChanceForAttack5)
                                summary.StateAnimFrames = summary.Enemy.PrimaryAttackAnimFrames5;
                            else
                                summary.StateAnimFrames = summary.Enemy.PrimaryAttackAnimFrames;
                        }
                    }
                }

                // Set to the first frame of this animation, and prepare frameIterator to start from the second frame when AnimateEnemy() next runs
                currentFrame = summary.StateAnimFrames[0];
                frameIterator = 1;
            }

            if (summary.EnemyState == MobileStates.RangedAttack1 || summary.EnemyState == MobileStates.RangedAttack2)
            {
                summary.StateAnimFrames = summary.Enemy.RangedAttackAnimFrames;

                // Set to the first frame of this animation, and prepare frameIterator to start from the second frame when AnimateEnemy() next runs
                currentFrame = summary.StateAnimFrames[0];
                frameIterator = 1;
            }

            if (summary.EnemyState == MobileStates.Spell)
            {
                summary.StateAnimFrames = summary.Enemy.SpellAnimFrames;

                // Set to the first frame of this animation, and prepare frameIterator to start from the second frame when AnimateEnemy() next runs
                currentFrame = summary.StateAnimFrames[0];
                frameIterator = 1;
            }

            if (summary.StateAnims == null)
            {
                // Log error message
                DaggerfallUnity.LogMessage(string.Format("DaggerfalMobileUnit: Enemy does not have animation for {0} state. Defaulting to Idle state.", summary.EnemyState.ToString()), true);

                // Set back to idle (which every enemy has in one form or another)
                summary.EnemyState = MobileStates.Idle;
                summary.StateAnims = GetStateAnims(summary.EnemyState);
            }

            // One of the frost daedra's sets of attack frames starts with the hit frame (-1), so we need to check for that right away before updating orientation.
            if (currentFrame == -1 && summary.EnemyState == MobileStates.PrimaryAttack)
            {
                doMeleeDamage = true;
                if (frameIterator < summary.StateAnimFrames.Length)
                    currentFrame = summary.StateAnimFrames[frameIterator++];
            }

            // Orient enemy relative to camera
            UpdateOrientation();
        }

        /// <summary>
        /// Updates enemy orientation based on angle between enemy and camera.
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
            if (summary.StateAnims == null || summary.StateAnims.Length == 0)
                return;

            // Get mesh filter
            if (meshFilter == null)
                meshFilter = GetComponent<MeshFilter>();

            // Try to fix if anim array is null
            if (summary.StateAnims == null)
                ApplyEnemyState();
            
            // Get enemy size and scale for this state
            int record = summary.StateAnims[orientation].Record;
            summary.AnimStateRecord = record;
            Vector2 size = summary.RecordSizes[record];

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

            // Check if orientation flip needed
            bool flip = summary.StateAnims[orientation].FlipLeftRight;

            // Scorpion animations need to be inverted
            if (summary.Enemy.ID == (int)MobileTypes.GiantScorpion)
                flip = !flip;

            // Update Record/Frame texture
            if (summary.ImportedTextures.HasImportedTextures)
            {
                if (meshRenderer == null)
                    meshRenderer = GetComponent<MeshRenderer>();

                // Assign imported texture
                meshRenderer.material.mainTexture = summary.ImportedTextures.Textures[record][currentFrame];

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

        IEnumerator AnimateEnemy()
        {
            float fps = 10;
            while (true)
            {
                if (!freezeAnims && summary.IsSetup && summary.StateAnims != null && summary.StateAnims.Length > 0)
                {
                    // Update enemy and fps
                    OrientEnemy(lastOrientation);
                    fps = summary.StateAnims[lastOrientation].FramePerSecond;

                    bool doingAttackAnimation = (summary.EnemyState == MobileStates.PrimaryAttack ||
                        summary.EnemyState == MobileStates.RangedAttack1 ||
                        summary.EnemyState == MobileStates.RangedAttack2);

                    // Reset to idle if frameIterator has finished. Used for attack animations.
                    if (doingAttackAnimation && frameIterator >= summary.StateAnimFrames.Length)
                    {
                        ChangeEnemyState(MobileStates.Idle);
                        frameIterator = 0;
                    }

                    // Step frame
                    if (!doingAttackAnimation)
                        currentFrame++;
                    else // Attack animation
                    {
                        currentFrame = summary.StateAnimFrames[frameIterator++];

                        // Set boolean if frame to attempt to apply damage or shoot arrow is encountered, and proceed to next frame if it exists
                        if (currentFrame == -1)
                        {
                            if (summary.EnemyState == MobileStates.PrimaryAttack)
                                doMeleeDamage = true;
                            else if (summary.EnemyState == MobileStates.RangedAttack1 || summary.EnemyState == MobileStates.RangedAttack2)
                                shootArrow = true;

                            if (frameIterator < summary.StateAnimFrames.Length)
                                currentFrame = summary.StateAnimFrames[frameIterator++];
                        }
                    }

                    if (currentFrame >= summary.StateAnims[lastOrientation].NumFrames)
                    {
                        if (IsPlayingOneShot())
                            ChangeEnemyState(MobileStates.Idle);    // If this is a one-shot anim, revert back to idle state at end
                        else
                            currentFrame = 0;                       // Otherwise keep looping frames
                    }
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
                if (DFRandom.random_range(0, 2) == 0)
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

                // Set optional scale
                TextureReplacement.SetBillboardScale(archive, i, ref finalSize);
 
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

            // Seek textures from mods
            TextureReplacement.SetMobileBillboardImportedTextures(archive, GetComponent<MeshFilter>(), ref summary.ImportedTextures);

            // Create material
            Material material;
            if (summary.ImportedTextures.HasImportedTextures)
            {
                material = MaterialReader.CreateStandardMaterial(MaterialReader.CustomBlendMode.Cutout);
            }
            else
            {
                material = dfUnity.MaterialReader.GetMaterialAtlas(
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
            }

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
                    if ((MobileTypes)summary.Enemy.ID == MobileTypes.Ghost ||
                        (MobileTypes)summary.Enemy.ID == MobileTypes.Wraith)
                        anims = (MobileAnimation[])EnemyBasics.GhostWraithMoveAnims.Clone();
                    else
                        anims = (MobileAnimation[])EnemyBasics.MoveAnims.Clone();
                    break;
                case MobileStates.PrimaryAttack:
                    if ((MobileTypes)summary.Enemy.ID == MobileTypes.Ghost ||
                        (MobileTypes)summary.Enemy.ID == MobileTypes.Wraith)
                        anims = (MobileAnimation[])EnemyBasics.GhostWraithAttackAnims.Clone();
                    else
                        anims = (MobileAnimation[])EnemyBasics.PrimaryAttackAnims.Clone();
                    break;
                case MobileStates.Hurt:
                    anims = (MobileAnimation[])EnemyBasics.HurtAnims.Clone();
                    break;
                case MobileStates.Idle:
                    if ((MobileTypes)summary.Enemy.ID == MobileTypes.Ghost ||
                        (MobileTypes)summary.Enemy.ID == MobileTypes.Wraith)
                        anims = (MobileAnimation[])EnemyBasics.GhostWraithMoveAnims.Clone();
                    else if (summary.Enemy.FemaleTexture == 483 &&
                        summary.Enemy.Gender == MobileGender.Female)
                        anims = (MobileAnimation[])EnemyBasics.FemaleThiefIdleAnims.Clone();
                    else if ((MobileTypes)summary.Enemy.ID == MobileTypes.Rat)
                        anims = (MobileAnimation[])EnemyBasics.RatIdleAnims.Clone();
                    else if (!summary.Enemy.HasIdle)
                        anims = (MobileAnimation[])EnemyBasics.MoveAnims.Clone();
                    else
                        anims = (MobileAnimation[])EnemyBasics.IdleAnims.Clone();
                    break;
                case MobileStates.RangedAttack1:
                    anims = (summary.Enemy.HasRangedAttack1) ? (MobileAnimation[])EnemyBasics.RangedAttack1Anims.Clone() : null;
                    break;
                case MobileStates.RangedAttack2:
                    anims = (summary.Enemy.HasRangedAttack2) ? (MobileAnimation[])EnemyBasics.RangedAttack2Anims.Clone() : null;
                    break;
                case MobileStates.Spell:
                    anims = (summary.Enemy.HasSpellAnimation) ? (MobileAnimation[])EnemyBasics.RangedAttack1Anims.Clone() : (MobileAnimation[])EnemyBasics.PrimaryAttackAnims.Clone();
                    break;
                default:
                    return null;
            }

            // Assign number number of frames per anim
            for (int i = 0; i < anims.Length; i++)
                anims[i].NumFrames = summary.RecordFrames[anims[i].Record];

            // If flying, set to faster flying animation speed
            if ((state == MobileStates.Move || state == MobileStates.Idle) && summary.Enemy.Behaviour == MobileBehaviour.Flying)
                for (int i = 0; i < anims.Length; i++)
                    anims[i].FramePerSecond = EnemyBasics.FlyAnimSpeed;

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