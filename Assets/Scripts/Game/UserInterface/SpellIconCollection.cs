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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DaggerfallWorkshop.Game.UserInterface
{
    /// <summary>
    /// Stores spell icons for use in game UI (e.g. HUD, Spellbook, Spellmaker).
    /// The classic spell icon file "ICON00I0.IMG" is a 320x64 atlas of 69 textures.
    /// There are three rows of 20 icons each and a fourth row of 9 icons.
    /// Icons are numbered 0-68 and tightly packed left-to-right top-to-bottom.
    /// When loading atlas image this class assumes each icon has a dimension 1/20th of source width.
    /// This means default file results in 16x16 pixels per icon (320/20=16).
    /// If injecting a new spell icon atlas use exact multiples of 320x64 (e.g. 640x128, 1280x256, 2560x512).
    /// This ensures icons are extracted in the same order as classic at new size without any further metadata.
    /// Current design does not allow for adding any more or less than 69 icons to collection.
    /// This could be an enhancement added later but would need to be carefully considered as it crosses multiple UIs.
    /// </summary>
    public class SpellIconCollection : IEnumerable
    {
        #region Fields

        const string spellIconsFile = "ICON00I0.IMG";
        const int spellIconsRowCount = 20;
        const int spellIconsCount = 69;

        List<Texture2D> spellIcons = new List<Texture2D>();

        #endregion

        #region Properties

        /// <summary>
        /// Static count of expected spell icons (always 69).
        /// </summary>
        public int Count
        {
            get { return spellIconsCount; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SpellIconCollection()
        {
            LoadSpellIcons();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get spell icon texture from index.
        /// </summary>
        public Texture2D GetIcon(int index)
        {
            if (index < 0 || index >= spellIcons.Count)
                return null;

            return spellIcons[index];
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Loads classic spell icons.
        /// </summary>
        void LoadSpellIcons()
        {
            // Clear existing collection
            spellIcons.Clear();

            // Get source atlas
            Texture2D spellIconAtlas = DaggerfallUI.GetTextureFromImg(spellIconsFile);
            if (spellIconAtlas == null)
            {
                Debug.LogError("Could not load spell icons atlas texture.");
                return;
            }

            // Derive dimension of each icon from atlas width
            const int rowCount = 20;
            int dim = spellIconAtlas.width / rowCount;

            // Read icons to their own texture (remembering Unity textures are flipped vertically)
            int srcX = 0, srcY = spellIconAtlas.height - dim;
            for (int i = 0; i < Count; i++)
            {
                // Extract texture
                Texture2D iconTexture = new Texture2D(dim, dim);
                Graphics.CopyTexture(spellIconAtlas, 0, 0, srcX, srcY, dim, dim, iconTexture, 0, 0, 0, 0);
                spellIcons.Add(iconTexture);

                // Step to next source icon position and wrap to next row
                srcX += dim;
                if (srcX >= spellIconAtlas.width)
                {
                    srcX = 0;
                    srcY -= dim;
                }
            }

            // Log success
            Debug.LogFormat("Loaded {0} spell icons for UI with a dimension of {1}x{2} each.", Count, dim, dim);
        }

        #endregion

        #region IEnumerable

        /// <summary>
        /// Gets IEnumerator for the spell icon collection.
        /// </summary>
        /// <returns>IEnumerator object.</returns>
        public IEnumerator GetEnumerator()
        {
            return (spellIcons as IEnumerable).GetEnumerator();
        }

        #endregion
    }
}
