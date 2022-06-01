// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    TheLacus
// 
// Notes:
//

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.IO;
using System.Collections;
using DaggerfallConnect;
using DaggerfallConnect.Utility;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Utility.AssetInjection;

namespace DaggerfallWorkshop
{
    /// <summary>
    /// The component that handles graphics for a mobile person.
    /// </summary>
    /// <remarks>
    /// Implementations should be added to a prefab named "MobilePersonAsset" bundled with a mod.
    /// This prefab acts as a provider for all custom npcs graphics. This allows mods to replace classic
    /// <see cref="MobilePersonBillboard"/> with 3d models or other alternatives.
    /// The actual graphic asset for specific npc should be loaded when requested by <see cref="SetPerson"/>.
    /// </remarks>
    public abstract class MobilePersonAsset : MonoBehaviour
    {
        /// <summary>
        /// Trigger collider used for interaction with player. The collider should be altered to enclose the entire npc mesh when
        /// <see cref="SetPerson"/> is called. A sign of badly setup collider is misbehaviour of idle state and talk functionalities.
        /// </summary>
        protected internal CapsuleCollider Trigger { get; internal set; }

        /// <summary>
        /// Gets or sets idle state. Daggerfall NPCs are either in or motion or idle facing player.
        /// This only controls animation state, actual motion is handled by <see cref="MobilePersonMotor"/>.
        /// </summary>
        public abstract bool IsIdle { get; set; }

        /// <summary>
        /// Setup this person based on race, gender and outfit variant. Enitities in a npcs pool can be recycled
        /// when out of range, meaning that this method can be called more than once with different parameters.
        /// </summary>
        /// <param name="race">Race of target npc.</param>
        /// <param name="gender">Gender of target npc.</param>
        /// <param name="personVariant">Which basic outfit does the person wear.</param>
        /// <param name="isGuard">True if this npc is a city watch guard.</param>
        public abstract void SetPerson(Races race, Genders gender, int personVariant, bool isGuard, int personFaceVariant, int personFaceRecordId);

        /// <summary>
        /// Gets size of asset used by this person (i.e size of bounds). Used to adjust position on terrain.
        /// </summary>
        /// <returns>Size of npc.</returns>
        public abstract Vector3 GetSize();

        /// <summary>
        /// Gets a bitmask that provides all the layers used by this asset.
        /// </summary>
        /// <returns>A layer mask.</returns>
        public virtual int GetLayerMask()
        {
            return 1 << gameObject.layer;
        }
    }

    /// <summary>
    /// Billboard class for classic wandering NPCs found in town environments.
    /// </summary>
#if UNITY_EDITOR
    [ExecuteInEditMode]
#endif
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class MobilePersonBillboard : MobilePersonAsset
    {
        #region Fields

        const int numberOrientations = 8;
        const float anglePerOrientation = 360f / numberOrientations;

        Vector3 cameraPosition;
        Camera mainCamera = null;
        MeshFilter meshFilter = null;
        MeshRenderer meshRenderer = null;
        float facingAngle;
        int lastOrientation;
        AnimStates currentAnimState;

        Vector2[] recordSizes;
        int[] recordFrames;
        Rect[] atlasRects;
        RecordIndex[] atlasIndices;
        MobileAnimation[] stateAnims;
        MobileAnimation[] moveAnims;
        MobileAnimation[] idleAnims;
        MobileBillboardImportedTextures importedTextures;
        int currentFrame = 0;

        float animSpeed;
        float animTimer = 0;

        bool isUsingGuardTexture = false;

        #endregion

        #region Textures

        int[] maleRedguardTextures = new int[] { 381, 382, 383, 384 };
        int[] femaleRedguardTextures = new int[] { 395, 396, 397, 398 };

        int[] maleNordTextures = new int[] { 387, 388, 389, 390 };
        int[] femaleNordTextures = new int[] { 392, 393, 451, 452 };

        int[] maleBretonTextures = new int[] { 385, 386, 391, 394 };
        int[] femaleBretonTextures = new int[] { 453, 454, 455, 456 };

        int[] guardTextures = { 399 };

        #endregion

        #region Animations

        const int IdleAnimSpeed = 1;
        const int MoveAnimSpeed = 4;
        static MobileAnimation[] IdleAnims = new MobileAnimation[]
        {
            new MobileAnimation() {Record = 5, FramePerSecond = IdleAnimSpeed, FlipLeftRight = false},          // Idle
        };

        static MobileAnimation[] IdleAnimsGuard = new MobileAnimation[]
        {
            new MobileAnimation() {Record = 15, FramePerSecond = IdleAnimSpeed, FlipLeftRight = false},          // Guard idle
        };

        static MobileAnimation[] MoveAnims = new MobileAnimation[]
        {
            new MobileAnimation() {Record = 0, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},          // Facing south (front facing player)
            new MobileAnimation() {Record = 1, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},          // Facing south-west
            new MobileAnimation() {Record = 2, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},          // Facing west
            new MobileAnimation() {Record = 3, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},          // Facing north-west
            new MobileAnimation() {Record = 4, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},          // Facing north (back facing player)
            new MobileAnimation() {Record = 3, FramePerSecond = MoveAnimSpeed, FlipLeftRight = true},           // Facing north-east
            new MobileAnimation() {Record = 2, FramePerSecond = MoveAnimSpeed, FlipLeftRight = true},           // Facing east
            new MobileAnimation() {Record = 1, FramePerSecond = MoveAnimSpeed, FlipLeftRight = true},           // Facing south-east
        };

        enum AnimStates
        {
            Idle,           // Idle facing player
            Move,           // Moving
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets idle state.
        /// Daggerfall NPCs are either in or motion or idle facing player.
        /// This only controls anim state, actual motion is handled by MobilePersonMotor.
        /// </summary>
        public sealed override bool IsIdle
        {
            get { return (currentAnimState == AnimStates.Idle); }
            set { SetIdle(value); }
        }

        #endregion

        #region Unity

        private void Start()
        {
            if (Application.isPlaying)
            {
                // Get component references
                mainCamera = GameManager.Instance.MainCamera;
                meshFilter = GetComponent<MeshFilter>();
            }

            // Mobile NPC shadows if enabled
            if (DaggerfallUnity.Settings.MobileNPCShadows)
                GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.TwoSided;
        }

        private void Update()
        {
            // Rotate to face camera in game
            if (mainCamera && Application.isPlaying)
            {
                // Rotate billboard based on camera facing
                cameraPosition = mainCamera.transform.position;
                Vector3 viewDirection = -new Vector3(mainCamera.transform.forward.x, 0, mainCamera.transform.forward.z);
                transform.LookAt(transform.position + viewDirection);

                // Orient based on camera position
                UpdateOrientation();

                // Tick animations
                animTimer += Time.deltaTime;
                if (animTimer > 1f / animSpeed)
                {
                    if (++currentFrame >= stateAnims[0].NumFrames)
                        currentFrame = 0;

                    UpdatePerson(lastOrientation);
                    animTimer = 0;
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Setup this person based on race and gender.
        /// </summary>
        public override void SetPerson(Races race, Genders gender, int personVariant, bool isGuard, int personFaceVariant, int personFaceRecordId)
        {
            // Must specify a race
            if (race == Races.None)
                return;

            // Get texture range for this race and gender
            int[] textures = null;

            isUsingGuardTexture = isGuard;

            if (isGuard)
            {
                textures = guardTextures;
            }
            else
            {
                switch (race)
                {
                    case Races.Redguard:
                        textures = (gender == Genders.Male) ? maleRedguardTextures : femaleRedguardTextures;
                        break;
                    case Races.Nord:
                        textures = (gender == Genders.Male) ? maleNordTextures : femaleNordTextures;
                        break;
                    case Races.Breton:
                    default:
                        textures = (gender == Genders.Male) ? maleBretonTextures : femaleBretonTextures;
                        break;
                }
            }

            // Setup person rendering
            CacheRecordSizesAndFrames(textures[personVariant]);
            AssignMeshAndMaterial(textures[personVariant]);

            // Setup animation state
            moveAnims = GetStateAnims(AnimStates.Move);
            idleAnims = GetStateAnims(AnimStates.Idle);
            stateAnims = moveAnims;
            animSpeed = stateAnims[0].FramePerSecond;
            currentAnimState = AnimStates.Move;
            lastOrientation = -1;
            UpdateOrientation();
        }

        /// <summary>
        /// Gets billboard size.
        /// </summary>
        /// <returns>Vector2 of billboard width and height.</returns>
        public sealed override Vector3 GetSize()
        {
            if (recordSizes == null || recordSizes.Length == 0)
                return Vector2.zero;

            return recordSizes[0];
        }

        #endregion

        #region Private Methods

        void SetIdle(bool idle)
        {
            if (idle)
            {
                // Switch animation state to idle
                currentAnimState = AnimStates.Idle;
                stateAnims = idleAnims;
                currentFrame = 0;
                lastOrientation = -1;
                animTimer = 1;
                animSpeed = stateAnims[0].FramePerSecond;
            }
            else
            {
                // Switch animation state to move
                currentAnimState = AnimStates.Move;
                stateAnims = moveAnims;
                currentFrame = 0;
                lastOrientation = -1;
                animTimer = 1;
                animSpeed = stateAnims[0].FramePerSecond;
            }
        }

        private void CacheRecordSizesAndFrames(int textureArchive)
        {
            // Open texture file
            string path = Path.Combine(DaggerfallUnity.Instance.Arena2Path, TextureFile.IndexToFileName(textureArchive));
            TextureFile textureFile = new TextureFile(path, FileUsage.UseMemory, true);

            // Cache size and scale for each record
            recordSizes = new Vector2[textureFile.RecordCount];
            recordFrames = new int[textureFile.RecordCount];
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
                TextureReplacement.SetBillboardScale(textureArchive, i, ref finalSize);

                // Store final size and frame count
                recordSizes[i] = finalSize * MeshReader.GlobalScale;
                recordFrames[i] = textureFile.GetFrameCount(i);
            }
        }

        private void AssignMeshAndMaterial(int textureArchive)
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
            mesh.name = string.Format("MobilePersonMesh");
            mesh.vertices = vertices;
            mesh.triangles = indices;
            mesh.normals = normals;

            // Assign mesh
            meshFilter.sharedMesh = mesh;

            // Create material
            Material material = TextureReplacement.GetMobileBillboardMaterial(textureArchive, meshFilter, ref importedTextures) ??
                DaggerfallUnity.Instance.MaterialReader.GetMaterialAtlas(
                textureArchive,
                0,
                4,
                1024,
                out atlasRects,
                out atlasIndices,
                4,
                true,
                0,
                false,
                true);

            // Set new person material
            GetComponent<MeshRenderer>().sharedMaterial = material;
        }

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
            facingAngle = Vector3.Angle(dir, parentForward);
            facingAngle = facingAngle * -Mathf.Sign(Vector3.Cross(dir, parentForward).y);

            // Facing index
            int orientation = - Mathf.RoundToInt(facingAngle / anglePerOrientation);
            // Wrap values to 0 .. numberOrientations-1
            orientation = (orientation + numberOrientations) % numberOrientations;

            // Change person to this orientation
            if (orientation != lastOrientation)
                UpdatePerson(orientation);
        }

        private void UpdatePerson(int orientation)
        {
            if (stateAnims == null || stateAnims.Length == 0)
                return;

            // Get mesh filter
            if (meshFilter == null)
                meshFilter = GetComponent<MeshFilter>();

            // Idle only has a single orientation
            if (currentAnimState == AnimStates.Idle && orientation != 0)
                orientation = 0;

            // Get person size and scale for this state
            int record = stateAnims[orientation].Record;
            Vector2 size = recordSizes[record];

            // Set mesh scale for this state
            transform.localScale = new Vector3(size.x, size.y, 1);

            // Check if orientation flip needed
            bool flip = stateAnims[orientation].FlipLeftRight;

            // Update Record/Frame texture
            if (importedTextures.HasImportedTextures)
            {
                if (meshRenderer == null)
                    meshRenderer = GetComponent<MeshRenderer>();

                // Assign imported texture
                meshRenderer.sharedMaterial.mainTexture = importedTextures.Albedo[record][currentFrame];
                if (importedTextures.IsEmissive)
                    meshRenderer.material.SetTexture(Uniforms.EmissionMap, importedTextures.EmissionMaps[record][currentFrame]);

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
                Rect rect = atlasRects[atlasIndices[record].startIndex + currentFrame];
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

        private MobileAnimation[] GetStateAnims(AnimStates state)
        {
            // Clone static animation state
            MobileAnimation[] anims;
            switch (state)
            {
                case AnimStates.Move:
                    anims = (MobileAnimation[])MoveAnims.Clone();
                    break;
                case AnimStates.Idle:
                    if (isUsingGuardTexture)
                        anims = (MobileAnimation[])IdleAnimsGuard.Clone();
                    else
                        anims = (MobileAnimation[])IdleAnims.Clone();
                    break;
                default:
                    return null;
            }

            // Assign number number of frames per anim
            for (int i = 0; i < anims.Length; i++)
                anims[i].NumFrames = recordFrames[anims[i].Record];

            return anims;
        }

        #endregion

        #region Editor

#if UNITY_EDITOR
        /// <summary>
        /// Rotate person to face editor camera while game not running.
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

    #endregion
    }
}