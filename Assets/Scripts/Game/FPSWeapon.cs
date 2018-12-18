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

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DaggerfallConnect;
using DaggerfallConnect.Utility;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Utility.AssetInjection;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// Renders first-person weapons and attack animations.
    /// Recommended for this component to be on its own game object.
    /// Cam modify pitch of audio source for different weapon speed effects.
    /// </summary>
    [RequireComponent(typeof(DaggerfallAudioSource))]
    public class FPSWeapon : MonoBehaviour
    {
        public bool ShowWeapon = true;
        public bool FlipHorizontal = false;
        public WeaponTypes WeaponType = WeaponTypes.None;
        public MetalTypes MetalType = MetalTypes.None;
        public float Reach = 2.5f;
        public float AttackSpeedScale = 1.0f;
        public float Cooldown = 0.0f;
        public SoundClips DrawWeaponSound = SoundClips.DrawWeapon;
        public SoundClips SwingWeaponSound = SoundClips.SwingMediumPitch;

        WeaponTypes currentWeaponType;
        MetalTypes currentMetalType;

        const int nativeScreenWidth = 320;
        const int nativeScreenHeight = 200;

        readonly byte[] leftUnarmedAnims = { 0, 1, 2, 3, 4, 2, 1, 0 };
        int leftUnarmedAnimIndex = 0;

        DaggerfallUnity dfUnity;
        CifRciFile cifFile;
        Texture2D weaponAtlas;
        Rect[] weaponRects;
        RecordIndex[] weaponIndices;
        Rect weaponPosition;
        float weaponScaleX;
        float weaponScaleY;

        DaggerfallAudioSource dfAudioSource;
        WeaponAnimation[] weaponAnims;
        WeaponStates weaponState = WeaponStates.Idle;
        int currentFrame = 0;
        Rect curAnimRect;

        readonly Dictionary<int, Texture2D> customTextures = new Dictionary<int, Texture2D>();
        Texture2D curCustomTexture;

        #region Properties

        public WeaponStates WeaponState { get { return weaponState; } }

        #endregion

        void Start()
        {
            dfUnity = DaggerfallUnity.Instance;
            dfAudioSource = GetComponent<DaggerfallAudioSource>();
            StartCoroutine(AnimateWeapon());
        }

        void OnGUI()
        {
            GUI.depth = 1;

            // Must be ready
            if (!ReadyCheck() || WeaponType == WeaponTypes.None || GameManager.IsGamePaused)
                return;

            // Must have current weapon texture atlas
            if (weaponAtlas == null || WeaponType != currentWeaponType || MetalType != currentMetalType)
            {
                LoadWeaponAtlas();
                if (weaponAtlas == null)
                    return;
            }
            UpdateWeapon();

            if (Event.current.type.Equals(EventType.Repaint) && ShowWeapon)
            {
                // Draw weapon texture behind other HUD elements
                GUI.DrawTextureWithTexCoords(weaponPosition, curCustomTexture ? curCustomTexture : weaponAtlas, curAnimRect);
            }
        }

        public void OnAttackDirection(WeaponManager.MouseDirections direction)
        {
            // Get state based on attack direction
            WeaponStates state;

            // Bows has only one type of attack
            if (WeaponType == WeaponTypes.Bow)
                state = WeaponStates.StrikeDown;
            else switch (direction)
            {
                case WeaponManager.MouseDirections.Down:
                    state = WeaponStates.StrikeDown;
                    break;
                case WeaponManager.MouseDirections.DownLeft:
                    state = WeaponStates.StrikeDownLeft;
                    break;
                case WeaponManager.MouseDirections.Left:
                    state = WeaponStates.StrikeLeft;
                    break;
                case WeaponManager.MouseDirections.Right:
                    state = WeaponStates.StrikeRight;
                    break;
                case WeaponManager.MouseDirections.DownRight:
                    state = WeaponStates.StrikeDownRight;
                    break;
                case WeaponManager.MouseDirections.Up:
                    state = WeaponStates.StrikeUp;
                    break;
                default:
                    return;
            }

            // Do not change if already playing attack animation
            if (!IsPlayingOneShot())
                ChangeWeaponState(state);
        }

        public void ChangeWeaponState(WeaponStates state)
        {
            weaponState = state;

            if (!(WeaponType == WeaponTypes.Bow && state == WeaponStates.StrikeDown))
                currentFrame = 0;

            UpdateWeapon();
        }

        public bool IsAttacking()
        {
            return IsPlayingOneShot();
        }

        public int GetHitFrame()
        {
            if (WeaponType == WeaponTypes.Bow)
                return 5;
            else
                return 2;
        }

        public int GetCurrentFrame()
        {
            return currentFrame;
        }

        public void PlayActivateSound()
        {
            if (dfAudioSource)
            {
                dfAudioSource.AudioSource.pitch = 1f;// *AttackSpeedScale;
                dfAudioSource.PlayOneShot(DrawWeaponSound, 0);
            }
        }

        public void PlaySwingSound()
        {
            if (dfAudioSource)
            {
                dfAudioSource.AudioSource.pitch = 1f * AttackSpeedScale;
                dfAudioSource.PlayOneShot(SwingWeaponSound, 0, 1.1f);
            }
        }

        public void PlayAttackVoice()
        {
            if (dfAudioSource)
            {
                PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;
                SoundClips sound = DaggerfallEntity.GetRaceGenderAttackSound(playerEntity.Race, playerEntity.Gender);
                float pitch = dfAudioSource.AudioSource.pitch;
                dfAudioSource.AudioSource.pitch = pitch + UnityEngine.Random.Range(0, 0.3f);
                dfAudioSource.PlayOneShot(sound, 0, 1f);
                dfAudioSource.AudioSource.pitch = pitch;
            }
        }

        #region Private Methods

        private bool IsPlayingOneShot()
        {
            if (weaponState != WeaponStates.Idle)
                return true;

            return false;
        }

        private void UpdateWeapon()
        {
            // Do nothing if weapon not ready
            if (weaponAtlas == null || weaponAnims == null ||
                weaponRects == null || weaponIndices == null)
            {
                return;
            }

            // Reset state if weapon not visible
            if (!ShowWeapon || WeaponType == WeaponTypes.None)
            {
                weaponState = WeaponStates.Idle;
                currentFrame = 0;
            }

            // Handle bow with no arrows
            if (!GameManager.Instance.WeaponManager.Sheathed && WeaponType == WeaponTypes.Bow && GameManager.Instance.PlayerEntity.Items.GetItem(Items.ItemGroups.Weapons, (int)Items.Weapons.Arrow) == null)
            {
                GameManager.Instance.WeaponManager.SheathWeapons();
                DaggerfallUI.SetMidScreenText(UserInterfaceWindows.HardStrings.youHaveNoArrows);
            }

            // Store rect and anim
            int weaponAnimRecordIndex;
            if (WeaponType == WeaponTypes.Bow)
                weaponAnimRecordIndex = 0; // Bow has only 1 animation
            else
                weaponAnimRecordIndex = (int)weaponState;

            try
            {
                bool isImported = customTextures.TryGetValue(MaterialReader.MakeTextureKey(0, (byte)weaponState, (byte)currentFrame), out curCustomTexture);
                if (FlipHorizontal && (weaponState == WeaponStates.Idle || weaponState == WeaponStates.StrikeDown || weaponState == WeaponStates.StrikeUp))
                {
                    // Mirror weapon rect
                    if (isImported)
                    {
                        curAnimRect = new Rect(0, 1, -1, 1);
                    }
                    else
                    {
                        Rect rect = weaponRects[weaponIndices[weaponAnimRecordIndex].startIndex + currentFrame];
                        curAnimRect = new Rect(rect.xMax, rect.yMin, -rect.width, rect.height);
                    }
                }
                else
                {
                    curAnimRect = isImported ? new Rect(0, 0, 1, 1) : weaponRects[weaponIndices[weaponAnimRecordIndex].startIndex + currentFrame];
                }
                WeaponAnimation anim = weaponAnims[(int)weaponState];

                // Get weapon dimensions
                int width = weaponIndices[weaponAnimRecordIndex].width;
                int height = weaponIndices[weaponAnimRecordIndex].height;

                // Get weapon scale
                weaponScaleX = (float)Screen.width / (float)nativeScreenWidth;
                weaponScaleY = (float)Screen.height / (float)nativeScreenHeight;

                // Adjust scale to be slightly larger when not using point filtering
                // This reduces the effect of filter shrink at edge of display
                if (dfUnity.MaterialReader.MainFilterMode != FilterMode.Point)
                {
                    weaponScaleX *= 1.01f;
                    weaponScaleY *= 1.01f;
                }

                // Source weapon images are designed to overlay a fixed 320x200 display.
                // Some weapons need to align with both top, bottom, and right of display.
                // This means they might be a little stretched on widescreen displays.
                switch (anim.Alignment)
                {
                    case WeaponAlignment.Left:
                        AlignLeft(anim, width, height);
                        break;

                    case WeaponAlignment.Center:
                        AlignCenter(anim, width, height);
                        break;

                    case WeaponAlignment.Right:
                        AlignRight(anim, width, height);
                        break;
                }
            }
            catch (IndexOutOfRangeException)
            {
                DaggerfallUnity.LogMessage("Index out of range exception for weapon animation. Probably due to weapon breaking + being unequipped during animation.");
            }
        }

        private void AlignLeft(WeaponAnimation anim, int width, int height)
        {
            weaponPosition = new Rect(
                Screen.width * anim.Offset,
                Screen.height - height * weaponScaleY,
                width * weaponScaleX,
                height * weaponScaleY);
        }

        private void AlignCenter(WeaponAnimation anim, int width, int height)
        {
            weaponPosition = new Rect(
                Screen.width / 2f - (width * weaponScaleX) / 2f,
                Screen.height - height * weaponScaleY,
                width * weaponScaleX,
                height * weaponScaleY);
        }

        private void AlignRight(WeaponAnimation anim, int width, int height)
        {
            if (FlipHorizontal && (weaponState == WeaponStates.Idle || weaponState == WeaponStates.StrikeDown || weaponState == WeaponStates.StrikeUp))
            {
                // Flip alignment
                AlignLeft(anim, width, height);
                return;
            }

            weaponPosition = new Rect(
                Screen.width * (1f - anim.Offset) - width * weaponScaleX,
                Screen.height - height * weaponScaleY,
                width * weaponScaleX,
                height * weaponScaleY);
        }

        private bool ReadyCheck()
        {
            // Do nothing if DaggerfallUnity not ready
            if (!dfUnity.IsReady)
            {
                DaggerfallUnity.LogMessage("FPSWeapon: DaggerfallUnity component is not ready. Have you set your Arena2 path?");
                return false;
            }

            // Ensure cif reader is ready
            if (cifFile == null)
            {
                cifFile = new CifRciFile();
                cifFile.Palette.Load(Path.Combine(dfUnity.Arena2Path, cifFile.PaletteName));
            }

            //// Must have weapon texture atlas
            //if (weaponAtlas == null ||
            //    WeaponType != lastWeaponType ||
            //    MetalType != lastMetalType)
            //{
            //    LoadWeaponAtlas();
            //    if (weaponAtlas == null)
            //        return false;
            //    UpdateWeapon();
            //}

            return true;
        }

        IEnumerator AnimateWeapon()
        {
            while (true)
            {
                Entity.PlayerEntity player = GameManager.Instance.PlayerEntity;
                float speed = 0;
                float time = 0;
                if (player != null)
                {
                    if (WeaponType == WeaponTypes.Bow)
                        time = Entity.PlayerEntity.ClassicUpdateInterval;
                    else
                    {
                        speed = 3 * (115 - player.Stats.LiveSpeed);
                        time = speed / 980; // Approximation of classic frame update
                    }
                }

                if (weaponAnims != null && ShowWeapon)
                {
                    int frameBeforeStepping = currentFrame;

                    // Special animation for unarmed attack to left
                    if ((WeaponType == WeaponTypes.Melee || WeaponType == WeaponTypes.Werecreature)
                        && WeaponState == WeaponStates.StrikeLeft)
                    {
                        // Step frame
                        currentFrame = leftUnarmedAnims[leftUnarmedAnimIndex];
                        leftUnarmedAnimIndex++;
                        if (leftUnarmedAnimIndex >= leftUnarmedAnims.Length)
                        {
                            ChangeWeaponState(WeaponStates.Idle);
                            leftUnarmedAnimIndex = 0;
                        }
                    }
                    else
                    {
                        // Step frame
                        currentFrame++;
                        if (currentFrame >= weaponAnims[(int)weaponState].NumFrames)
                        {
                            if (IsPlayingOneShot())
                            {
                                ChangeWeaponState(WeaponStates.Idle);   // If this is a one-shot anim go to queued weapon state
                                if (WeaponType == WeaponTypes.Bow)
                                    ShowWeapon = false;                 // Immediately hide bow so its idle frame doesn't show before it is hidden for its cooldown
                            }
                            else if (WeaponType == WeaponTypes.Bow)
                                currentFrame = 3;
                            else
                                currentFrame = 0;                       // Otherwise keep looping frames
                        }
                    }

                    // Only update if the frame actually changed
                    if (frameBeforeStepping != currentFrame)
                        UpdateWeapon();
                }

                yield return new WaitForSeconds(time);
            }
        }

        private void LoadWeaponAtlas()
        {
            // Get weapon filename
            string filename = WeaponBasics.GetWeaponFilename(WeaponType);

            // Load the weapon texture atlas
            // Texture is dilated into a transparent coloured border to remove dark edges when filtered
            // Important to use returned UV rects when drawing to get right dimensions
            weaponAtlas = GetWeaponTextureAtlas(filename, MetalType, out weaponRects, out weaponIndices, 2, 2, true);
            weaponAtlas.filterMode = dfUnity.MaterialReader.MainFilterMode;

            // Get weapon anims
            weaponAnims = (WeaponAnimation[])WeaponBasics.GetWeaponAnims(WeaponType).Clone();

            // Store current weapon
            currentWeaponType = WeaponType;
            currentMetalType = MetalType;
        }

        #endregion

        #region Texture Loading

        private Texture2D GetWeaponTextureAtlas(
            string filename,
            MetalTypes metalType,
            out Rect[] rectsOut,
            out RecordIndex[] indicesOut,
            int padding,
            int border,
            bool dilate = false)
        {
            // Load texture file
            cifFile.Load(Path.Combine(dfUnity.Arena2Path, filename), FileUsage.UseMemory, true);

            // Read every image in archive
            Rect rect;
            List<Texture2D> textures = new List<Texture2D>();
            List<RecordIndex> indices = new List<RecordIndex>();
            customTextures.Clear();
            for (int record = 0; record < cifFile.RecordCount; record++)
            {
                int frames = cifFile.GetFrameCount(record);
                DFSize size = cifFile.GetSize(record);
                RecordIndex ri = new RecordIndex()
                {
                    startIndex = textures.Count,
                    frameCount = frames,
                    width = size.Width,
                    height = size.Height,
                };
                indices.Add(ri);
                for (int frame = 0; frame < frames; frame++)
                {
                    textures.Add(GetWeaponTexture2D(filename, record, frame, metalType, out rect, border, dilate));

                    Texture2D tex;
                    if (TextureReplacement.TryImportCifRci(filename, record, frame, metalType, out tex))
                    {
                        tex.filterMode = dfUnity.MaterialReader.MainFilterMode;
                        customTextures.Add(MaterialReader.MakeTextureKey(0, (byte)record, (byte)frame), tex);
                    }
                }
            }

            // Pack textures into atlas
            Texture2D atlas = new Texture2D(2048, 2048, TextureFormat.ARGB32, false);
            rectsOut = atlas.PackTextures(textures.ToArray(), padding, 2048);
            indicesOut = indices.ToArray();

            // Shrink UV rect to compensate for internal border
            float ru = 1f / atlas.width;
            float rv = 1f / atlas.height;
            for (int i = 0; i < rectsOut.Length; i++)
            {
                Rect rct = rectsOut[i];
                rct.xMin += border * ru;
                rct.xMax -= border * ru;
                rct.yMin += border * rv;
                rct.yMax -= border * rv;
                rectsOut[i] = rct;
            }

            return atlas;
        }

        private Texture2D GetWeaponTexture2D(
            string filename,
            int record,
            int frame,
            MetalTypes metalType,
            out Rect rectOut,
            int border = 0,
            bool dilate = false)
        {
            // Get source bitmap
            DFBitmap dfBitmap = cifFile.GetDFBitmap(record, frame);

            // Tint based on metal type
            // But not for steel as that is default colour in files
            if (metalType != MetalTypes.Steel)
                dfBitmap = ImageProcessing.ChangeDye(dfBitmap, ImageProcessing.GetMetalDyeColor(metalType), DyeTargets.WeaponsAndArmor);

            // Get Color32 array
            DFSize sz;
            Color32[] colors = cifFile.GetColor32(dfBitmap, 0, border, out sz);

            // Dilate edges
            if (border > 0 && dilate)
                ImageProcessing.DilateColors(ref colors, sz);

            // Create Texture2D
            Texture2D texture = new Texture2D(sz.Width, sz.Height, TextureFormat.ARGB32, false);
            texture.SetPixels32(colors);
            texture.Apply(true);

            // Shrink UV rect to compensate for internal border
            float ru = 1f / sz.Width;
            float rv = 1f / sz.Height;
            rectOut = new Rect(border * ru, border * rv, (sz.Width - border * 2) * ru, (sz.Height - border * 2) * rv);

            return texture;
        }

        #endregion
    }
}