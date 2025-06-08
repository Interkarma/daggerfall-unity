using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DaggerfallWorkshop.Utility
{
    /// <summary>
    /// Builds an atlas from any combination of texture archives.
    /// Does not yet integrate with main material system.
    /// Currently some limitations around caching and serialization.
    /// At this time used only for static misc billboards in cities.
    /// Will be enhanced in future.
    /// </summary>
    public class TextureAtlasBuilder
    {
        #region Fields

        Dictionary<int, TextureItem> textureItems = new Dictionary<int, TextureItem>();
        Dictionary<int, AtlasItem> atlasItems = new Dictionary<int, AtlasItem>();
        Texture2D atlasTexture = null;
        Material atlasMaterial = null;
        int maxAtlasDim = 2048;
        bool mipMaps = true;
        int padding = 4;
        bool stayReadable = false;

        #endregion

        #region Structures & Enums

        /// <summary>
        /// Each individual texture is held in a list of texture items.
        /// </summary>
        public struct TextureItem
        {
            public int key;                         // Unique key of this texture item
            public int archive;                     // Texture archive (index of texture file, such as TEXTURE.210)
            public int record;                      // Archive record (index of texture inside file)
            public int frame;                       // Record frame (index of frame inside record)
            public int frameCount;                  // Number of frames in this set
            public Vector2 size;                    // Size of this texture in DaggerfallUnits.
            public Vector2 scale;                   // Scale of this texture in DaggerfallUnits.
            public Texture2D texture;               // Individual texture used when repacking
        }

        /// <summary>
        /// The packed textures are each referenced by an atlas item with a unique key
        /// </summary>
        public struct AtlasItem
        {
            public int key;                         // Unique key of this atlas item
            public Rect rect;                       // Rect of this texture inside atlas
            public TextureItem textureItem;         // Details of source texture this item created from
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the atlas Texture2D created after packing.
        /// </summary>
        public Texture2D AtlasTexture
        {
            get { return atlasTexture; }
        }

        /// <summary>
        /// Gets the atlas material generated from texture.
        /// </summary>
        public Material AtlasMaterial
        {
            get { return (atlasMaterial != null) ? atlasMaterial : GetMaterial(); }
        }

        /// <summary>
        /// Gets or sets max dimensions of texture atlas.
        /// </summary>
        public int MaxAtlasDim
        {
            get { return maxAtlasDim; }
            set { maxAtlasDim = value; }
        }

        /// <summary>
        /// Gets or sets flag to create mipmaps for atlas texture.
        /// </summary>
        public bool MipMaps
        {
            get { return mipMaps; }
            set { mipMaps = value; }
        }

        /// <summary>
        /// Gets or sets padding around each texture in atlas.
        /// </summary>
        public int Padding
        {
            get { return padding; }
            set { padding = value; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Add a new texture item to source list.
        /// These items are used to build atlas.
        /// </summary>
        /// <param name="texture">Individual Texture2D. Must be readable.</param>
        /// <param name="archive">Archive index of this texture.</param>
        /// <param name="record">Record index of this texture.</param>
        /// <param name="frame">Frame index of this texture.</param>
        /// <param name="frameCount">Number of frames in this set.</param>
        public void AddTextureItem(Texture2D texture, int archive, int record, int frame, int frameCount, Vector2 size, Vector2 scale)
        {
            // Create texture item
            int key = MakeTextureKey((short)archive, (byte)record, (byte)frame);
            TextureItem item = new TextureItem()
            {
                key = key,
                texture = texture,
                archive = archive,
                record = record,
                frame = frame,
                frameCount = frameCount,
                size = size,
                scale = scale,
            };
            textureItems.Add(key, item);
        }

        /// <summary>
        /// Rebuilds source textures into new atlas.
        /// </summary>
        public void Rebuild(int borderSize)
        {
            // Create sequential array of texture references
            int index = 0;
            Texture2D[] textureRefs = new Texture2D[textureItems.Count];
            foreach (var ti in textureItems)
            {
                textureRefs[index++] = ti.Value.texture;
            }

            // Create atlas texture
            atlasTexture = new Texture2D(maxAtlasDim, maxAtlasDim, TextureFormat.ARGB32, mipMaps);
            Rect[] rects = atlasTexture.PackTextures(textureRefs, padding, maxAtlasDim, !stayReadable);

            // Shrink UV rect to compensate for internal border
            if (borderSize > 0)
            {
                float ru = 1f / atlasTexture.width;
                float rv = 1f / atlasTexture.height;
                for (int i = 0; i < rects.Length; i++)
                {
                    Rect rct = rects[i];
                    rct.xMin += borderSize * ru;
                    rct.xMax -= borderSize * ru;
                    rct.yMin += borderSize * rv;
                    rct.yMax -= borderSize * rv;
                    rects[i] = rct;
                }
            }

            // Rebuild dict
            index = 0;
            atlasItems.Clear();
            foreach (var ti in textureItems)
            {
                // Create atlas item
                AtlasItem item = new AtlasItem()
                {
                    key = ti.Key,
                    rect = rects[index],
                    textureItem = ti.Value,
                };
                atlasItems.Add(ti.Key, item);
                index++;
            }

            // Promote to new material
            GetMaterial();
        }

        /// <summary>
        /// Gets a Daggerfall/Default or Daggerfall/Billboard material from current atlas texture based on blendMode.
        /// Does not return normal or emission map.
        /// </summary>
        /// <param name="blendMode">Blend mode of material used to determine material returned.</param>
        /// <returns>Material.</returns>
        public Material GetMaterial(MaterialReader.CustomBlendMode blendMode = MaterialReader.CustomBlendMode.Cutout)
        {
            Material material = (blendMode == MaterialReader.CustomBlendMode.Cutout) ? MaterialReader.CreateBillboardMaterial() : MaterialReader.CreateDefaultMaterial();
            material.name = "SuperAtlas Material";
            material.mainTexture = atlasTexture;
            material.mainTexture.filterMode = DaggerfallUnity.Instance.MaterialReader.MainFilterMode;
            atlasMaterial = material;

            return material;
        }

        /// <summary>
        /// Gets atlas details of a particular item.
        /// If texture does not exist in atlas an empty item is returned.
        /// </summary>
        /// <param name="archive">Archive index of texture.</param>
        /// <param name="record">Record index of texture.</param>
        /// <param name="frame">Frame index of texture.</param>
        /// <returns>AtlasItem. Key member contains -1 if item not present.</returns>
        public AtlasItem GetAtlasItem(int archive, int record, int frame = 0)
        {
            int key = MakeTextureKey((short)archive, (byte)record, (byte)frame);

            return GetAtlasItem(key);
        }

        /// <summary>
        /// Gets details of a particular atlas item.
        /// If texture does not exist in atlas an empty item is returned.
        /// </summary>
        /// <param name="key">Key of item.</param>
        /// <returns>AtlasItem. Key member contains -1 if item not present.</returns>
        public AtlasItem GetAtlasItem(int key)
        {
            AtlasItem item = new AtlasItem();
            if (!atlasItems.TryGetValue(key, out item))
                item.key = -1;

            return item;
        }

        /// <summary>
        /// Gets frame count of any item.
        /// </summary>
        /// <returns>Number of frame in this set.</returns>
        public int GetFrameCount(int archive, int record)
        {
            int count = 0;
            int key = MakeTextureKey((short)archive, (byte)record, (byte)0);
            TextureItem item = new TextureItem();
            if (textureItems.TryGetValue(key, out item))
            {
                count = item.frameCount;
            }

            return count;
        }

        /// <summary>
        /// Clear all texture items and reset atlas.
        /// </summary>
        public void Clear()
        {
            textureItems.Clear();
            atlasItems.Clear();
            atlasTexture = null;
            atlasMaterial = null;
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Create a texture key from indices.
        /// </summary>
        public static int MakeTextureKey(short archive, byte record, byte frame)
        {
            return (archive << 16) + (record << 8) + frame;
        }

        /// <summary>
        /// Reverse a texture key back into indices.
        /// </summary>
        public static void ReverseTextureKey(int key, out int archiveOut, out int recordOut, out int frameOut)
        {
            archiveOut = (key >> 16);
            recordOut = (key >> 8) & 0xff;
            frameOut = key & 0xff;
        }

        #endregion
    }
}