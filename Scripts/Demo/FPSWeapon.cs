// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DaggerfallConnect;
using DaggerfallConnect.Utility;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop.Demo
{
    /// <summary>
    /// Example weapon component.
    /// Recommended for this component to be on its own game object.
    /// Will modify pitch of audio source for different weapon effects.
    /// </summary>
    [RequireComponent(typeof(DaggerfallAudioSource))]
    public class FPSWeapon : MonoBehaviour
    {
        public bool ShowWeapon = true;
        public bool LeftHand = false;
        public WeaponTypes WeaponType = WeaponTypes.Dagger;
        public MetalTypes MetalType = MetalTypes.Dwarven;
        public float Range = 2.5f;
        public float MinDamage = 5f;
        public float MaxDamage = 25f;
        public float AttackSpeedScale = 1.0f;
        public SoundClips DrawWeaponSound = SoundClips.DrawWeapon;
        public SoundClips SwingWeaponSound = SoundClips.PlayerSwing;

        WeaponTypes lastWeaponType;
        MetalTypes lastMetalType;

        const int nativeScreenWidth = 320;
        const int nativeScreenHeight = 200;

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

        void Start()
        {
            dfUnity = DaggerfallUnity.Instance;
            dfAudioSource = GetComponent<DaggerfallAudioSource>();
            StartCoroutine(AnimateWeapon());
        }

        void OnGUI()
        {
            if (!ReadyCheck())
            {
                return;
            }

            if (Event.current.type.Equals(EventType.Repaint) && ShowWeapon)
            {
                // Draw weapon texture
                GUI.DrawTextureWithTexCoords(weaponPosition, weaponAtlas, curAnimRect);
            }
        }

        public void OnAttackDirection(WeaponManager.MouseDirections direction)
        {
            // Get state based on attack direction
            WeaponStates state;
            switch (direction)
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
            {
                PlaySwingSound();
                ChangeWeaponState(state);
            }
        }

        public void ChangeWeaponState(WeaponStates state)
        {
            weaponState = state;
            currentFrame = 0;
            UpdateWeapon();
        }

        public bool IsAttacking()
        {
            return IsPlayingOneShot();
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
                dfAudioSource.PlayOneShot(SwingWeaponSound, 0);
            }
        }

        public void PlayHitSound()
        {
            if (dfAudioSource)
            {
                dfAudioSource.AudioSource.pitch = 1f;
                int sound = (int)SoundClips.Hit1 + UnityEngine.Random.Range(0, 5);
                dfAudioSource.PlayOneShot(sound, 0);
            }
        }

        public void PlayParrySound()
        {
            if (dfAudioSource)
            {
                dfAudioSource.AudioSource.pitch = 1f;
                int sound = (int)SoundClips.Parry1 + UnityEngine.Random.Range(0, 9);
                dfAudioSource.PlayOneShot(sound, 0);
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
                return;

            // Reset state if weapon not visible
            if (!ShowWeapon)
            {
                weaponState = WeaponStates.Idle;
                return;
            }

            // Store rect and anim
            if (LeftHand &&
                (weaponState == WeaponStates.Idle || weaponState == WeaponStates.StrikeDown || weaponState == WeaponStates.StrikeUp))
            {
                // Mirror weapon rect
                Rect rect = weaponRects[weaponIndices[(int)weaponState].startIndex + currentFrame];
                curAnimRect = new Rect(rect.xMax, rect.yMin, -rect.width, rect.height);
            }
            else
            {
                curAnimRect = weaponRects[weaponIndices[(int)weaponState].startIndex + currentFrame];
            }
            WeaponAnimation anim = weaponAnims[(int)weaponState];

            // Get weapon dimensions
            int width = weaponIndices[(int)weaponState].width;
            int height = weaponIndices[(int)weaponState].height;

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
            if (LeftHand &&
                (weaponState == WeaponStates.Idle || weaponState == WeaponStates.StrikeDown || weaponState == WeaponStates.StrikeUp))
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

            // Must have weapon texture atlas
            if (weaponAtlas == null ||
                WeaponType != lastWeaponType ||
                MetalType != lastMetalType)
            {
                LoadWeaponAtlas();
                if (weaponAtlas == null)
                    return false;
                UpdateWeapon();
            }

            return true;
        }

        IEnumerator AnimateWeapon()
        {
            while (true)
            {
                float fps = 10;
                if (weaponAnims != null)
                {
                    // Step frame
                    currentFrame++;
                    if (currentFrame >= weaponAnims[(int)weaponState].NumFrames)
                    {
                        if (IsPlayingOneShot())
                            ChangeWeaponState(WeaponStates.Idle);   // If this is a one-shot anim go to queued weapon state
                        else
                            currentFrame = 0;                       // Otherwise keep looping frames
                    }

                    // Update weapon and fps
                    UpdateWeapon();
                    fps = (int)((float)weaponAnims[(int)weaponState].FramePerSecond * AttackSpeedScale);
                }

                yield return new WaitForSeconds(1f / fps);
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
            lastWeaponType = WeaponType;
            lastMetalType = MetalType;
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
                }
            }

            // Pack textures into atlas
            Texture2D atlas = new Texture2D(2048, 2048, TextureFormat.RGBA32, false);
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
                ImageProcessing.TintWeaponImage(dfBitmap, metalType);

            // Get Color32 array
            DFSize sz;
            Color32[] colors = cifFile.GetColors32(dfBitmap, 0, border, out sz);

            // Dilate edges
            if (border > 0 && dilate)
                ImageProcessing.DilateColors(ref colors, sz);

            // Create Texture2D
            Texture2D texture = new Texture2D(sz.Width, sz.Height, TextureFormat.RGBA32, false);
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