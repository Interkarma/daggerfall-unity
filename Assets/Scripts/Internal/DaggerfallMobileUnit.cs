// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2023 Daggerfall Workshop
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
using DaggerfallWorkshop.Game;

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
    public class DaggerfallMobileUnit : MobileUnit
    {
        const int numberOrientations = 8;
        const float anglePerOrientation = 360f / numberOrientations;

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

        public override int FrameSpeedDivisor
        {
            get { return frameSpeedDivisor; }
            set { frameSpeedDivisor = (value < 1) ? 1 : value; }
        }

        public override bool DoMeleeDamage
        {
            get { return doMeleeDamage; }
            set { doMeleeDamage = value; }
        }

        public override bool ShootArrow
        {
            get { return shootArrow; }
            set { shootArrow = value; }
        }

        public override bool FreezeAnims
        {
            get { return freezeAnims; }
            set { freezeAnims = value; }
        }

        public override MobileEnemy Enemy
        {
            get { return summary.Enemy; }
            protected set { summary.Enemy = value; }
        }

        public override MobileStates EnemyState
        {
            get { return summary.EnemyState; }
            protected set { summary.EnemyState = value; }
        }

        public override bool IsBackFacing
        {
            get { return summary.AnimStateRecord % 5 > 2; }
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

        protected void LogMobileError(string  message)
        {
            string enemyName = TextManager.Instance.GetLocalizedEnemyName(summary.Enemy.ID);
            Debug.LogError($"Enemy '{enemyName}' (state={summary.EnemyState}): {message}");
        }

        protected override void ApplyEnemy(DaggerfallUnity dfUnity)
        {
            // Load enemy content
            int archive = GetTextureArchive();
            CacheRecordSizesAndFrames(dfUnity, archive);
            AssignMeshAndMaterial(dfUnity, archive);

            // Apply enemy state and update orientation
            lastOrientation = -1;
            ApplyEnemyState();
        }

        protected override void ApplyEnemyStateChange(MobileStates currentState, MobileStates newState)
        {
            // Don't reset frame to 0 for idle/move switches for enemies without idle animations
            bool resetFrame = true;

            if (!summary.Enemy.HasIdle &&
                ((currentState == MobileStates.Idle && newState == MobileStates.Move) ||
                (currentState == MobileStates.Move && newState == MobileStates.Idle)))
                resetFrame = false;

            if (resetFrame)
            {
                currentFrame = 0;
                animReversed = false;
            }

            ApplyEnemyState();
        }

        public override Vector3 GetSize()
        {
            return summary.RecordSizes[0];
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

            if (summary.EnemyState == MobileStates.SeducerTransform1)
            {
                // Switch to flying sprite alignment while crouched and growing wings
                summary.Enemy.Behaviour = MobileBehaviour.Flying;
                summary.StateAnimFrames = summary.Enemy.SeducerTransform1Frames;

                // Set to the first frame of this animation, and prepare frameIterator to start from the second frame when AnimateEnemy() next runs
                currentFrame = summary.StateAnimFrames[0];
                frameIterator = 1;
            }

            if (summary.EnemyState == MobileStates.SeducerTransform2)
            {
                // Switch to grounded sprite alignment while standing and spreading wings
                summary.Enemy.Behaviour = MobileBehaviour.General;
                summary.StateAnimFrames = summary.Enemy.SeducerTransform2Frames;

                // Set to the first frame of this animation, and prepare frameIterator to start from the second frame when AnimateEnemy() next runs
                currentFrame = summary.StateAnimFrames[0];
                frameIterator = 1;
            }

            if (summary.StateAnims == null)
            {
                // Log error message
                LogMobileError($"Enemy does not have animation for {summary.EnemyState} state. Defaulting to Idle state.");

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

            // Facing index
            int orientation = - Mathf.RoundToInt(enemyFacingAngle / anglePerOrientation);
            // Wrap values to 0 .. numberOrientations-1
            orientation = (orientation + numberOrientations) % numberOrientations;

            // Change enemy to this orientation
            if (orientation != lastOrientation)
            {
                // Different orientations may not have the same amount of frames
                // For example, archive 288 (Ancient Lich) has 4 frames in six orientations (front, back, front diagonals, sides), but 8 frames for the two back diagonals
                // If you change orientation from a back diagonal during frame 4, 5, 6, or 7, you overflow the other anims, where only 0 to 3 are valid
                if (lastOrientation >= 0)
                {
                    currentFrame = currentFrame * summary.StateAnims[orientation].NumFrames / summary.StateAnims[lastOrientation].NumFrames;
                }

                OrientEnemy(orientation);
            }
        }

        /// <summary>
        /// Sets enemy orientation index.
        /// </summary>
        /// <param name="orientation">New orientation index.</param>
        private void OrientEnemy(int orientation)
        {
            if (summary.StateAnims == null || summary.StateAnims.Length == 0)
                return;

            if (orientation < 0 || orientation >= summary.StateAnims.Length)
            {
                LogMobileError($"Orientation '{orientation}' was invalid for state anims (length = {summary.StateAnims.Length}))");
                return;
            }

            if (currentFrame < 0)
            {
                LogMobileError($"Invalid frame '{currentFrame}'");
                return;
            }
            
            // Get mesh filter
            if (meshFilter == null)
                meshFilter = GetComponent<MeshFilter>();
            
            // Get enemy size and scale for this state
            int record = summary.StateAnims[orientation].Record;
            summary.AnimStateRecord = record;

            if(record < 0 || record >= summary.RecordSizes.Length)
            {
                LogMobileError($"Invalid record '{record}' for state anim '{orientation}' (length = {summary.RecordSizes.Length})");
                return;
            }           

            Vector2 size = summary.RecordSizes[record];

            // Post-fix female texture scale for 475 while casting spells
            // The scale read from Daggerfall's files is too small 
            if (summary.Enemy.FemaleTexture == 475 &&
                summary.Enemy.Gender == MobileGender.Female &&
                record >= 20 && record <= 24)
            {
                size *= 1.35f;
            }

            // Ensure walking enemies keep their feet aligned between states
            if (summary.Enemy.Behaviour != MobileBehaviour.Flying &&
                summary.Enemy.Behaviour != MobileBehaviour.Aquatic &&
                summary.EnemyState != MobileStates.Idle)
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

            // Scorpion animations need to be inverted
            if (summary.Enemy.ID == (int)MobileTypes.GiantScorpion)
                flip = !flip;
            
            // Update Record/Frame texture
            if (summary.ImportedTextures.HasImportedTextures)
            {
                if (meshRenderer == null)
                    meshRenderer = GetComponent<MeshRenderer>();

                // Check that record and frame are valid for albedo
                if(record >= summary.ImportedTextures.Albedo.Length)
                {
                    LogMobileError($"Invalid record '{record}' for imported textures albedo (length = {summary.ImportedTextures.Albedo.Length})");
                    return;
                }

                if(currentFrame >= summary.ImportedTextures.Albedo[record].Length)
                {
                    LogMobileError($"Invalid frame '{currentFrame}' for imported textures albedo record '{record}' (length = {summary.ImportedTextures.Albedo[record].Length})");
                    return;
                }

                // Assign imported texture
                meshRenderer.material.mainTexture = summary.ImportedTextures.Albedo[record][currentFrame];
                if (summary.ImportedTextures.IsEmissive)
                {
                    // Check that record and frame are valid for emissives
                    if (record >= summary.ImportedTextures.EmissionMaps.Length)
                    {
                        LogMobileError($"Invalid record '{record}' for imported textures emissives (length = {summary.ImportedTextures.EmissionMaps.Length})");
                        return;
                    }

                    if (currentFrame >= summary.ImportedTextures.EmissionMaps[record].Length)
                    {
                        LogMobileError($"Invalid frame '{currentFrame}' for imported textures emissives record '{record}' (length = {summary.ImportedTextures.EmissionMaps[record].Length})");
                        return;
                    }

                    meshRenderer.material.SetTexture(Uniforms.EmissionMap, summary.ImportedTextures.EmissionMaps[record][currentFrame]);
                }

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
                // Check record and "rect index" for atlas
                if (record >= summary.AtlasIndices.Length)
                {
                    LogMobileError($"Invalid record '{record}' for atlas indices (length = {summary.AtlasIndices.Length})");
                    return;
                }

                int rectIndex = summary.AtlasIndices[record].startIndex + currentFrame;

                if (rectIndex >= summary.AtlasRects.Length)
                {
                    LogMobileError($"Invalid rect index '{rectIndex}' (start={summary.AtlasIndices[record].startIndex}, current={currentFrame}) for atlas rects (length = {summary.AtlasRects.Length})");
                    return;
                }

                // Daggerfall Atlas: Update UVs on mesh
                Rect rect = summary.AtlasRects[rectIndex];
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
                    fps = summary.StateAnims[lastOrientation].FramePerSecond / FrameSpeedDivisor;

                    // Enforce a lower limit on animation speed when using a custom FrameSpeedDivisor
                    if (FrameSpeedDivisor > 1 && fps < 4)
                        fps = 4;

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
                        currentFrame = (animReversed) ? currentFrame - 1 : currentFrame + 1;
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

                    if (currentFrame >= summary.StateAnims[lastOrientation].NumFrames ||
                        animReversed && currentFrame <= 0)
                    {
                        if (IsPlayingOneShot())
                            ChangeEnemyState(NextStateAfterCurrentOneShot());   // If this is a one-shot anim, revert to next state (usually idle)
                        else
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
                }

                yield return new WaitForSeconds(1f / fps);
            }
        }

        /// <summary>
        /// Gets the next state after finished playing current oneshot state
        /// </summary>
        /// <returns>Next state.</returns>
        MobileStates NextStateAfterCurrentOneShot()
        {
            switch(summary.EnemyState)
            {
                case MobileStates.SeducerTransform1:
                    return MobileStates.SeducerTransform2;
                case MobileStates.SeducerTransform2:
                    SetSpecialTransformationCompleted();
                    return MobileStates.Idle;
                default:
                    return MobileStates.Idle;
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
                    LogMobileError($"Texture archive {archive} has no valid records");
                }
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
                    else if ((MobileTypes)summary.Enemy.ID == MobileTypes.DaedraSeducer && summary.specialTransformationCompleted)
                        anims = (MobileAnimation[])EnemyBasics.SeducerIdleMoveAnims.Clone();
                    else if ((MobileTypes)summary.Enemy.ID == MobileTypes.Slaughterfish)
                        anims = (MobileAnimation[])EnemyBasics.SlaughterfishMoveAnims.Clone();
                    else
                        anims = (MobileAnimation[])EnemyBasics.MoveAnims.Clone();
                    break;
                case MobileStates.PrimaryAttack:
                    if ((MobileTypes)summary.Enemy.ID == MobileTypes.Ghost ||
                        (MobileTypes)summary.Enemy.ID == MobileTypes.Wraith)
                        anims = (MobileAnimation[])EnemyBasics.GhostWraithAttackAnims.Clone();
                    else if ((MobileTypes)summary.Enemy.ID == MobileTypes.DaedraSeducer && summary.specialTransformationCompleted)
                        anims = (MobileAnimation[])EnemyBasics.SeducerAttackAnims.Clone();
                    else
                        anims = (MobileAnimation[])EnemyBasics.PrimaryAttackAnims.Clone();
                    break;
                case MobileStates.Hurt:
                    if ((MobileTypes)summary.Enemy.ID == MobileTypes.DaedraSeducer && summary.specialTransformationCompleted)
                        anims = (MobileAnimation[])EnemyBasics.SeducerIdleMoveAnims.Clone();
                    else
                        anims = (MobileAnimation[])EnemyBasics.HurtAnims.Clone();
                    break;
                case MobileStates.Idle:
                    if ((MobileTypes)summary.Enemy.ID == MobileTypes.Ghost ||
                        (MobileTypes)summary.Enemy.ID == MobileTypes.Wraith)
                        anims = (MobileAnimation[])EnemyBasics.GhostWraithMoveAnims.Clone();
                    else if ((MobileTypes)summary.Enemy.ID == MobileTypes.DaedraSeducer && summary.specialTransformationCompleted)
                        anims = (MobileAnimation[])EnemyBasics.SeducerIdleMoveAnims.Clone();
                    else if (summary.Enemy.FemaleTexture == 483 &&
                        summary.Enemy.Gender == MobileGender.Female)
                        anims = (MobileAnimation[])EnemyBasics.FemaleThiefIdleAnims.Clone();
                    else if ((MobileTypes)summary.Enemy.ID == MobileTypes.Rat)
                        anims = (MobileAnimation[])EnemyBasics.RatIdleAnims.Clone();
                    else if ((MobileTypes)summary.Enemy.ID == MobileTypes.Slaughterfish)
                        anims = (MobileAnimation[])EnemyBasics.SlaughterfishMoveAnims.Clone();
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
                    if ((MobileTypes)summary.Enemy.ID == MobileTypes.DaedraSeducer && summary.specialTransformationCompleted)
                        anims = (MobileAnimation[])EnemyBasics.SeducerAttackAnims.Clone();
                    else
                        anims = (summary.Enemy.HasSpellAnimation) ? (MobileAnimation[])EnemyBasics.RangedAttack1Anims.Clone() : (MobileAnimation[])EnemyBasics.PrimaryAttackAnims.Clone();
                    break;
                case MobileStates.SeducerTransform1:
                    anims = (summary.Enemy.HasSeducerTransform1) ? (MobileAnimation[])EnemyBasics.SeducerTransform1Anims.Clone() : null;
                    break;
                case MobileStates.SeducerTransform2:
                    anims = (summary.Enemy.HasSeducerTransform2) ? (MobileAnimation[])EnemyBasics.SeducerTransform2Anims.Clone() : null;
                    break;
                default:
                    LogMobileError($"Invalid mobile state '{state}' in GetStateAnims");
                    return null;
            }

            // Assign number of frames per anim
            for (int i = 0; i < anims.Length; i++)
            {
                if (anims[i].Record < 0 || anims[i].Record >= summary.RecordFrames.Length)
                {
                    LogMobileError($"Invalid record '{anims[i].Record}' for available record frames (length = {summary.RecordFrames.Length})");
                    return null;
                }
                anims[i].NumFrames = summary.RecordFrames[anims[i].Record];
            }

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