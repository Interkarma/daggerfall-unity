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
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.MagicAndEffects;

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
        const string spellTargetAndElementIconsFile = "MASK04I0.IMG";
        const int spellIconsRowCount = 20;
        const int spellIconsCount = 69;
        const int spellTargetIconsCount = 5;
        const int spellElementIconsCount = 5;

        List<Texture2D> spellIcons = new List<Texture2D>();
        List<Texture2D> spellTargetIcons = new List<Texture2D>();
        List<Texture2D> spellElementIcons = new List<Texture2D>();

        #endregion

        #region Properties

        /// <summary>
        /// Static count of expected spell icons (always 69).
        /// </summary>
        public int SpellIconCount
        {
            get { return spellIconsCount; }
        }

        /// <summary>
        /// Static count of expected spell target icons (always 5).
        /// </summary>
        public int TargetIconCount
        {
            get { return spellTargetIconsCount; }
        }

        /// <summary>
        /// Static count of expected spell element icons (always 5).
        /// </summary>
        public int ElementIconCount
        {
            get { return spellElementIconsCount; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SpellIconCollection()
        {
            LoadSpellIcons();
            LoadSpellTargetAndElementIcons();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get spell icon texture from index.
        /// </summary>
        public Texture2D GetSpellIcon(int index)
        {
            if (index < 0 || index >= spellIcons.Count)
                return null;

            return spellIcons[index];
        }

        /// <summary>
        /// Get spell target icon texture.
        /// </summary>
        public Texture2D GetSpellTargetIcon(TargetTypes targetType)
        {
            int index;
            switch(targetType)
            {
                case TargetTypes.CasterOnly:
                    index = 0;
                    break;
                case TargetTypes.ByTouch:
                    index = 1;
                    break;
                case TargetTypes.SingleTargetAtRange:
                    index = 2;
                    break;
                case TargetTypes.AreaAroundCaster:
                    index = 3;
                    break;
                case TargetTypes.AreaAtRange:
                    index = 4;
                    break;
                default:
                    return null;
            }

            return spellTargetIcons[index];
        }

        /// <summary>
        /// Get spell element icon texture.
        /// </summary>
        public Texture2D GetSpellElementIcon(ElementTypes elementType)
        {
            int index;
            switch(elementType)
            {
                case ElementTypes.Fire:
                    index = 0;
                    break;
                case ElementTypes.Cold:
                    index = 1;
                    break;
                case ElementTypes.Poison:
                    index = 2;
                    break;
                case ElementTypes.Shock:
                    index = 3;
                    break;
                case ElementTypes.Magic:
                    index = 4;
                    break;
                default:
                    return null;
            }

            return spellElementIcons[index];
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
                Debug.LogWarning("SpellIconCollection: Could not load spell icons atlas texture. Arena2 path might not be set yet.");
                return;
            }

            // Derive dimension of each icon from atlas width
            const int rowCount = 20;
            int dim = spellIconAtlas.width / rowCount;

            // Checks texture imported from mods
            if (spellIconAtlas.format == TextureFormat.DXT5 && dim % 4 != 0)
            {
                Debug.LogErrorFormat("{0} is compressed with a block-based format but icons are not multiple of 4.", spellIconsFile);
                return;
            }

            // Read icons to their own texture (remembering Unity textures are flipped vertically)
            int srcX = 0, srcY = spellIconAtlas.height - dim;
            for (int i = 0; i < SpellIconCount; i++)
            {
                // Extract texture
                Texture2D iconTexture = new Texture2D(dim, dim, spellIconAtlas.format, false);
                Graphics.CopyTexture(spellIconAtlas, 0, 0, srcX, srcY, dim, dim, iconTexture, 0, 0, 0, 0);
                iconTexture.filterMode = DaggerfallUnity.Instance.MaterialReader.MainFilterMode;
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
            //Debug.LogFormat("Loaded {0} spell icons for UI with a dimension of {1}x{2} each.", spellIcons.Count, dim, dim);
        }

        void LoadSpellTargetAndElementIcons()
        {
            const int targetIconWidth = 24;
            const int elementIconWidth = 16;
            const int height = 16;

            // Clear existing collections
            spellTargetIcons.Clear();
            spellElementIcons.Clear();

            // Get source atlas
            Texture2D spellTargetAndElementIconAtlas = DaggerfallUI.GetTextureFromImg(spellTargetAndElementIconsFile, TextureFormat.ARGB32, false);
            if (spellTargetAndElementIconAtlas == null)
            {
                Debug.LogWarning("SpellIconCollection: Could not load spell target and element icons atlas texture.  Arena2 path might not be set yet.");
                return;
            }

            // Read target icons to their own atlas
            Rect targetIconRect = new Rect(0, 0, targetIconWidth, height);
            for (int i = 0; i < spellTargetIconsCount; i++)
            {
                Texture2D iconTexture = ImageReader.GetSubTexture(spellTargetAndElementIconAtlas, targetIconRect);
                spellTargetIcons.Add(iconTexture);
                targetIconRect.y = targetIconRect.y + height;
            }

            // Read element icons to their own atlas
            Rect elementIconRect = new Rect(targetIconWidth, 0, elementIconWidth, height);
            for (int i = 0; i < spellElementIconsCount; i++)
            {
                Texture2D iconTexture = ImageReader.GetSubTexture(spellTargetAndElementIconAtlas, elementIconRect);
                spellElementIcons.Add(iconTexture);
                elementIconRect.y = elementIconRect.y + height;
            }

            // Log success
            Debug.LogFormat("Loaded {0} spell target icons and {1} element icons.", spellTargetIcons.Count, spellElementIcons.Count);
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
