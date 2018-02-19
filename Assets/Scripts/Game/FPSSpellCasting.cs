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
using System.IO;
using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallConnect.Utility;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Utility.AssetInjection;
using DaggerfallWorkshop.Game.Effects;

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// Renders first-person spellcasting animations for player.
    /// Spellcasting animations have different texture and layout requirements to weapons
    /// and are never mixed with weapons directly on screen at same time.
    /// Opted to create a new class to play these animations and separate from FPSWeapon.
    /// </summary>
    [RequireComponent(typeof(DaggerfallAudioSource))]
    public class FPSSpellCasting : MonoBehaviour
    {
        #region Fields

        const int nativeScreenWidth = 320;
        const int nativeScreenHeight = 200;
        const float animSpeed = 0.3f;

        Dictionary<SpellTypes, Texture2D[]> castAnims = new Dictionary<SpellTypes, Texture2D[]>();
        Texture2D[] currentAnims;
        int currentFrame = -1;

        DaggerfallAudioSource dfAudioSource;
        Rect leftHandPosition;
        Rect rightHandPosition;
        Rect leftHandAnimRect;
        Rect rightHandAnimRect;
        float handScaleX;
        float handScaleY;

        #endregion

        #region Properties

        public bool IsPlayingAnim
        {
            get { return currentFrame >= 0; }
        }

        #endregion

        #region Unity

        void Start()
        {
            dfAudioSource = GetComponent<DaggerfallAudioSource>();
            StartCoroutine(AnimateSpellCast());

            // TEMP: Start playing test anim
            //SetCurrentAnims(SpellTypes.Fire);
            //currentFrame = 0;
        }

        void OnGUI()
        {
            // Must be ready
            if (!ReadyCheck() || GameManager.IsGamePaused)
                return;

            UpdateSpellCast();

            if (Event.current.type.Equals(EventType.Repaint))
            {
                // Draw spell cast texture behind other HUD elements
                GUI.depth = 1;
                GUI.DrawTextureWithTexCoords(leftHandPosition, currentAnims[currentFrame], leftHandAnimRect);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Get animations for current spellcast.
        /// This happens the first time a spell is cast and stored for re-casting.
        /// It's likely player will use a wide variety of spell types in normal play.
        /// </summary>
        void SetCurrentAnims(SpellTypes spellType, int border = 0, bool dilate = false)
        {
            // Attempt to get current anims
            if (castAnims.ContainsKey(spellType))
            {
                currentAnims = castAnims[spellType];
                return;
            }

            // Load spellcast file
            string filename = WeaponBasics.GetSpellAnimFilename(spellType);
            string path = Path.Combine(DaggerfallUnity.Instance.Arena2Path, filename);
            CifRciFile cifFile = new CifRciFile();
            if (!cifFile.Load(path, FileUsage.UseMemory, true))
                throw new Exception(string.Format("Could not load spell anims file {0}", path));

            // Load CIF palette
            cifFile.Palette.Load(Path.Combine(DaggerfallUnity.Instance.Arena2Path, cifFile.PaletteName));

            // Load textures - spells have a single frame per record unlike weapons
            Texture2D[] frames = new Texture2D[cifFile.RecordCount];
            for (int record = 0; record < cifFile.RecordCount; record++)
            {
                Texture2D texture = null;

                // Import custom texture or load classic texture
                if (TextureReplacement.CustomCifExist(filename, record, 0, MetalTypes.None))
                {
                    texture = TextureReplacement.LoadCustomCif(filename, record, 0, MetalTypes.None);
                }
                else
                {
                    // Get Color32 array
                    DFSize sz;
                    Color32[] colors = cifFile.GetColor32(record, 0, 0, border, out sz);

                    // Dilate edges
                    if (border > 0 && dilate)
                        ImageProcessing.DilateColors(ref colors, sz);

                    // Create Texture2D
                    texture = new Texture2D(sz.Width, sz.Height, TextureFormat.RGBA32, false);
                    texture.SetPixels32(colors);
                    texture.Apply(true);
                }

                // Set filter mode and store in frames array
                if (texture)
                {
                    texture.filterMode = (FilterMode)DaggerfallUnity.Settings.MainFilterMode;
                    frames[record] = texture;
                }
            }

            // Add frames array to dictionary
            castAnims.Add(spellType, frames);

            // Use as current anims
            currentAnims = frames;
        }

        private void UpdateSpellCast()
        {
            // Do nothing if cast frame < 0
            if (currentFrame < 0)
                return;

            // Get frame dimensions
            int width = currentAnims[currentFrame].width;
            int height = currentAnims[currentFrame].height;

            // Get weapon scale
            handScaleX = (float)Screen.width / (float)nativeScreenWidth;
            handScaleY = (float)Screen.height / (float)nativeScreenHeight;

            // Adjust scale to be slightly larger when not using point filtering
            // This reduces the effect of filter shrink at edge of display
            if (DaggerfallUnity.Instance.MaterialReader.MainFilterMode != FilterMode.Point)
            {
                handScaleX *= 1.01f;
                handScaleY *= 1.01f;
            }

            // Get source rect
            leftHandAnimRect = new Rect(0, 0, 1.0f, 1.0f);
            rightHandAnimRect = new Rect(width, height, -width, height);

            // Source casting animations are designed to fit inside a fixed 320x200 display
            // This means they might be a little stretched on widescreen displays
            AlignLeftHand(width, height);
            AlignRightHand(width, height);
        }

        private void AlignLeftHand(int width, int height)
        {
            leftHandPosition = new Rect(
                Screen.width * 0f,
                Screen.height - height * handScaleY,
                width * handScaleX,
                height * handScaleY);
        }

        private void AlignRightHand(int width, int height)
        {
            rightHandPosition = new Rect(
                Screen.width * (1f - 0f) - width * handScaleX,
                Screen.height - height * handScaleY,
                width * handScaleX,
                height * handScaleY);
        }

        IEnumerator AnimateSpellCast()
        {
            while (true)
            {
                if (currentAnims != null && currentAnims.Length > 0)
                {
                    // Step frame until end
                    currentFrame++;
                    if (currentFrame >= currentAnims.Length)
                        currentFrame = 0;
                }

                yield return new WaitForSeconds(animSpeed);
            }
        }

        private bool ReadyCheck()
        {
            // Do nothing if DaggerfallUnity not ready
            if (!DaggerfallUnity.Instance.IsReady)
            {
                DaggerfallUnity.LogMessage("FPSSpellCasting: DaggerfallUnity component is not ready. Have you set your Arena2 path?");
                return false;
            }

            // Must have current spell texture anims
            if (currentAnims == null || currentAnims.Length == 0)
                return false;

            return true;
        }

        #endregion
    }
}