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

        Dictionary<SpellTypes, Texture2D[]> castAnims = new Dictionary<SpellTypes, Texture2D[]>();
        Texture2D[] currentAnims;
        int currentFrame = -1;

        #endregion

        #region Private Methods

        /// <summary>
        /// Get animations for current spellcast.
        /// This happens the first time a spell is cast and stored for re-casting.
        /// It's likely player will use a wide variety of spell types in normal play.
        /// </summary>
        /// <param name="spellType"></param>
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

        #endregion
    }
}