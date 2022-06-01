// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    Alyndiar
// 
// Notes:
//

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.MagicAndEffects;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game.Utility.ModSupport;

namespace DaggerfallWorkshop.Game.UserInterface
{
    /// <summary>
    /// Stores spell icon packs for use in game UI (e.g. HUD, Spellbook, Spellmaker).
    /// The classic spell icon file "ICON00I0.IMG" is a 320x64 atlas of 69 textures.
    /// </summary>
    public class SpellIconCollection
    {
        #region Fields

        const string sourceFolderName = "SpellIcons";
        const string spellIconsFile = "ICON00I0.IMG";
        const string spellTargetAndElementIconsFile = "MASK04I0.IMG";
        const int classicSpellIconsRowCount = 20;
        const int classicSpellIconsCount = 69;
        const int spellTargetIconsCount = 5;
        const int spellElementIconsCount = 5;

        readonly static DFSize spellTargetAndElementIconsSize = new DFSize(40, 80);

        List<Texture2D> spellIcons = new List<Texture2D>();
        List<Texture2D> spellTargetIcons = new List<Texture2D>();
        List<Texture2D> spellElementIcons = new List<Texture2D>();

        readonly Dictionary<string, SpellIconPack> spellIconPacks = new Dictionary<string, SpellIconPack>();

        #endregion

        #region Structs

        public class SpellIconPack
        {
            public string displayName;                      // Display name of icon pack
            public int rowCount;                            // Total number of icons per row in atlas
            public int iconCount;                           // Total number of icons in entire atlas
            public FilterMode filterMode;                   // Preferred filter mode of loaded icon textures
            public SpellIconSettings[] icons;               // Individual icons
        }

        public class SpellIconSettings
        {
            public int index;                               // Actual index for human readability
            public string[] suggestedEffects;               // List of effect keys this icon could be suggested for
            [NonSerialized]public Texture2D texture;        // Actual texture displayed
        }

        #endregion

        #region Properties

        /// <summary>
        /// Static count of expected spell icons (always 69).
        /// </summary>
        public int SpellIconCount
        {
            get { return classicSpellIconsCount; }
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

        public Dictionary<string, SpellIconPack> SpellIconPacks
        {
            get { return spellIconPacks; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SpellIconCollection()
        {
            LoadSpellIconPacks();
            LoadClassicSpellIcons();
            LoadClassicSpellTargetAndElementIcons();
        }

        #endregion

        #region Public Methods

        public bool HasPack(string key)
        {
            return (!string.IsNullOrEmpty(key)) ? spellIconPacks.ContainsKey(key) : false;
        }

        public int GetIconCount(string key)
        {
            // Fallback to classic pack count
            if (!HasPack(key))
                return SpellIconCount;

            return spellIconPacks[key].iconCount;
        }

        public Texture2D GetSpellIcon(SpellIcon icon)
        {
            // Fallback to classic icons
            if (string.IsNullOrEmpty(icon.key) || !HasPack(icon.key))
                return GetSpellIcon(icon.index);

            // If pack and index seem valid then return texture from pack
            SpellIconPack pack = spellIconPacks[icon.key];
            if (pack.iconCount == 0 || pack.icons == null || icon.index < 0 || icon.index >= pack.iconCount)
                return null;
            else
                return pack.icons[icon.index].texture;
        }

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
        /// Populates a dictionary of *.png spell icons.
        /// If no metadata is present for icon atlas then an empty metadata file will be created.
        /// Modder must fill out basic metadata information like row count and icon count before icons will be loaded.
        /// </summary>
        void LoadSpellIconPacks()
        {
            // Start with all the atlases in the spell icons streaming assets path
            string sourcePath = Path.Combine(Application.streamingAssetsPath, sourceFolderName);
            string[] atlasPaths = Directory.GetFiles(sourcePath, "*.png");
            if (atlasPaths != null && atlasPaths.Length != 0)
            {
                // Read each atlas found and its metadata
                foreach (string path in atlasPaths)
                {
                    // Get source atlas
                    Texture2D atlasTexture = LoadAtlasTextureFromPNG(path);
                    if (atlasTexture == null)
                    {
                        Debug.LogWarningFormat("LoadSpellIconPacks(): Could not load spell icons atlas texture '{0}'", path);
                        continue;
                    }

                    // Attempt to load metadata JSON file
                    SpellIconPack pack = null;
                    string packKey = Path.GetFileNameWithoutExtension(path);
                    string metadataFilename = packKey + ".txt";
                    string metadataPath = Path.Combine(sourcePath, metadataFilename);
                    if (File.Exists(metadataPath))
                    {
                        // Try to load existing metadata and icons
                        pack = ReadMetadata(metadataPath);
                        if (TryLoadPackIcons(pack, atlasTexture, metadataPath))
                            spellIconPacks.Add(packKey, pack);
                    }
                    else
                    {
                        // Create empty metadata file if none found
                        pack = CreateIconPackEmptyMetadata(atlasTexture, metadataPath);
                    }
                }
            }    

            // Import icon packs from mods with load order
            if (ModManager.Instance)
            {
                foreach (Mod mod in ModManager.Instance.GetAllModsWithContributes(x => x.SpellIcons != null))
                {
                    foreach (string packName in mod.ModInfo.Contributes.SpellIcons.Where(x => !spellIconPacks.ContainsKey(x)))
                    {
                        // Both atlas and metadata must be provided
                        var atlas = mod.GetAsset<Texture2D>(packName);
                        var metaData = mod.GetAsset<TextAsset>(packName + ".json");
                        if (!atlas || !metaData)
                        {
                            Debug.LogWarningFormat("Failed to retrieve assets for icon pack {0} from {1}.", packName, mod.Title);
                            continue;
                        }

                        // Add pack to dictionary
                        var pack = SaveLoadManager.Deserialize(typeof(SpellIconPack), metaData.ToString()) as SpellIconPack;
                        if (TryLoadPackIcons(pack, atlas))
                            spellIconPacks.Add(packName, pack);
                    }
                }
            }
        }

        Texture2D LoadAtlasTextureFromPNG(string path)
        {
            byte[] data = File.ReadAllBytes(path);
            Texture2D atlas = new Texture2D(0, 0, TextureFormat.ARGB32, false);
            if (atlas.LoadImage(data))
                return atlas;

            return null;
        }

        SpellIconPack CreateIconPackEmptyMetadata(Texture2D atlasTexture, string path)
        {
            // Create an empty metadata file at path
            SpellIconPack pack = new SpellIconPack()
            {
                displayName = Path.GetFileNameWithoutExtension(path),
                rowCount = 0,
                iconCount = 0,
                filterMode = FilterMode.Bilinear,
            };

            WriteMetadata(pack, path);
            //Debug.LogFormat("Created empty spell icon metadata at '{0}'.", path);

            return pack;
        }

        void CreateEmptyIconSettings(SpellIconPack pack, string path = null)
        {
            pack.icons = new SpellIconSettings[pack.iconCount];
            for (int i = 0; i < pack.iconCount; i++)
            {
                pack.icons[i] = new SpellIconSettings()
                {
                    index = i,
                };
            }

            if (path != null)
                WriteMetadata(pack, path);
        }

        bool TryLoadPackIcons(SpellIconPack pack, Texture2D atlas, string path = null)
        {
            // Check pack is valid
            if (pack == null || pack.rowCount == 0 || pack.iconCount == 0 || pack.icons == null)
            {
                string debugInfo = pack != null ? pack.displayName : (path ?? "<unknown>");
                Debug.LogErrorFormat("Failed to import icon pack {0} because metadata is missing or invalid.", debugInfo);
                return false;
            }

            // Might need to generate empty icon settings for first time
            if (pack.icons == null)
                CreateEmptyIconSettings(pack, path);

            // Derive dimension of each icon from atlas width
            int dim = atlas.width / pack.rowCount;

            // Check icons size
            if (atlas.width % dim != 0 || atlas.height % dim != 0)
            {
                Debug.LogErrorFormat("Failed to extract icons from {0} because atlas size is not multiple of icons size.", pack.displayName);
                return false;
            }

            // Read icons to their own texture (remembering Unity textures are flipped vertically)
            int srcX = 0, srcY = atlas.height - dim;
            for (int i = 0; i < pack.iconCount; i++)
            {
                // Extract texture
                Texture2D iconTexture = new Texture2D(dim, dim, atlas.format, false);
                Graphics.CopyTexture(atlas, 0, 0, srcX, srcY, dim, dim, iconTexture, 0, 0, 0, 0);
                iconTexture.filterMode = pack.filterMode;
                pack.icons[i].texture = iconTexture;

                // Step to next source icon position and wrap to next row
                srcX += dim;
                if (srcX >= atlas.width)
                {
                    srcX = 0;
                    srcY -= dim;
                }
            }

            return true;
        }

        void WriteMetadata(SpellIconPack pack, string path)
        {
            string json = SaveLoadManager.Serialize(pack.GetType(), pack);
            File.WriteAllText(path, json);
        }

        SpellIconPack ReadMetadata(string path)
        {
            string json = File.ReadAllText(path);
            return SaveLoadManager.Deserialize(typeof(SpellIconPack), json) as SpellIconPack;
        }

        /// <summary>
        /// Loads classic spell icons.
        /// </summary>
        void LoadClassicSpellIcons()
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

        void LoadClassicSpellTargetAndElementIcons()
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
                Texture2D iconTexture = ImageReader.GetSubTexture(spellTargetAndElementIconAtlas, targetIconRect, spellTargetAndElementIconsSize);
                spellTargetIcons.Add(iconTexture);
                targetIconRect.y = targetIconRect.y + height;
            }

            // Read element icons to their own atlas
            Rect elementIconRect = new Rect(targetIconWidth, 0, elementIconWidth, height);
            for (int i = 0; i < spellElementIconsCount; i++)
            {
                Texture2D iconTexture = ImageReader.GetSubTexture(spellTargetAndElementIconAtlas, elementIconRect, spellTargetAndElementIconsSize);
                spellElementIcons.Add(iconTexture);
                elementIconRect.y = elementIconRect.y + height;
            }

            // Log success
            Debug.LogFormat("Loaded {0} spell target icons and {1} element icons.", spellTargetIcons.Count, spellElementIcons.Count);
        }

        #endregion
    }
}
