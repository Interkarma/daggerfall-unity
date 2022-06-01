// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
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
using System.Linq;
using System.Collections.Generic;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects;
using DaggerfallWorkshop.Utility.AssetInjection;

namespace DaggerfallWorkshop.Game.Utility
{
    /// <summary>
    /// Render paper doll at higher pixel densities using custom layer flags.
    /// Maintains layout data to sample equip index of item texture under mouse.
    /// Replacement paper doll textures must be readable.
    /// </summary>
    public class PaperDollRenderer
    {
        #region Fields

        public const int paperDollWidth = 110;
        public const int paperDollHeight = 184;
        public const int waistHeight = 40;

        const string paperDollMaterialName = "Daggerfall/PaperDoll";

        readonly Material paperDollMaterial;
        RenderTexture target = null;
        Texture2D paperDollTexture = null;
        readonly DFPosition paperDollOrigin = new DFPosition(200, 8);    // Used to translate hard-coded IMG file offsets back to origin

        float scale;
        readonly List<ItemElement> itemLayout = new List<ItemElement>();

        #endregion

        #region Structs & Enums

        struct ItemElement
        {
            public DaggerfallUnityItem item;
            public Rect rect;
            public Texture2D texture;
        }

        /// <summary>
        /// These flags determine which parts of paper doll build are rendered into output texture.
        /// </summary>
        [Flags]
        public enum LayerFlags
        {
            None = 0,
            CloakInterior = 1,
            Body = 2,
            Items = 4,
            All = 7,
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets render output since last refresh.
        /// </summary>
        public Texture2D PaperDollTexture
        {
            get { return paperDollTexture; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="scale">Scale of paper doll render applied to classic dimensions. Cannot be lower than 1.0.</param>
        public PaperDollRenderer(float scale = 1.0f)
        {
            paperDollMaterial = new Material(Shader.Find(paperDollMaterialName));
            ChangeRenderScale(scale);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Change dimensions of render output.
        /// </summary>
        /// <param name="scale">Scale of paper doll render applied to classic dimensions. Cannot be lower than 1.0.</param>
        void ChangeRenderScale(float scale = 1.0f)
        {
            // Release old render texture
            if (target != null)
            {
                target.Release();
                target = null;
            }

            // Destroy old output texture
            if (paperDollTexture != null)
            {
                GameObject.Destroy(paperDollTexture);
                paperDollTexture = null;
            }

            // Scale cannot be lower than 1.0
            if (scale < 1)
                scale = 1;

            this.scale = scale;

            // Create scaled render texture
            target = new RenderTexture((int)(paperDollWidth * scale), (int)(paperDollHeight * scale), 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);

            // Create output texture
            paperDollTexture = new Texture2D(target.width, target.height, TextureFormat.ARGB32, false, true);
            paperDollTexture.filterMode = DaggerfallUI.Instance.GlobalFilterMode;
        }

        /// <summary>
        /// Renders a new paper doll image for the specified player entity.
        /// </summary>
        /// <param name="layers">Flags controlling which layers of paper doll to draw.</param>
        /// <param name="playerEntity">Player entity to use for paper doll construction. Use null for live player entity.</param>
        public void Refresh(LayerFlags layers = LayerFlags.All, PlayerEntity playerEntity = null)
        {
            // Clear current item layout
            itemLayout.Clear();

            // Get current player entity if one not provided
            if (playerEntity == null)
                playerEntity = GameManager.Instance.PlayerEntity;

            // Racial override can suppress body and items
            bool suppressBody = false;
            RacialOverrideEffect racialOverride = GameManager.Instance.PlayerEffectManager.GetRacialOverrideEffect();
            if (racialOverride != null)
                suppressBody = racialOverride.SuppressPaperDollBodyAndItems;

            // Start rendering to paper doll target
            RenderTexture oldRT = RenderTexture.active;
            RenderTexture.active = target;
            GL.PushMatrix();
            GL.LoadPixelMatrix(0, target.width, target.height, 0);

            // Clear render target
            GL.Clear(true, true, Color.clear);

            if (!suppressBody)
            {
                // Cloak interior
                if ((layers & LayerFlags.CloakInterior) == LayerFlags.CloakInterior)
                    BlitCloakInterior(playerEntity);

                // Body
                if ((layers & LayerFlags.Body) == LayerFlags.Body)
                    BlitBody(playerEntity);

                // Items
                if ((layers & LayerFlags.Items) == LayerFlags.Items)
                    BlitItems(playerEntity);
            }

            // Copy render to new output
            paperDollTexture.ReadPixels(new Rect(0, 0, target.width, target.height), 0, 0);
            paperDollTexture.Apply();

            // Switch back to previous render target
            GL.PopMatrix();
            RenderTexture.active = oldRT;
        }

        /// <summary>
        /// Gets equip index at position. Must be in unscaled PaperDoll native coordinates.
        /// </summary>
        /// <param name="x">X position to sample.</param>
        /// <param name="y">Y position to sample.</param>
        /// <returns>Equip index or 0xff if point empty.</returns>
        public byte GetEquipIndex(int x, int y)
        {
            // Source mouse position is in classic coordinates
            // Item elements are in scaled coordinates
            // First scale mouse point to match scaled coordinates
            float scaleX = target.width / (float)paperDollWidth;
            float scaleY = target.height / (float)paperDollHeight;
            Vector2 point = new Vector2(x * scaleX, y * scaleY);

            // Look for item texture pixel under mouse position
            // Iterating backwards over list as higher draw order added last
            // Return item equip slot index of first non-transparent pixel found under mouse
            for (int i = itemLayout.Count - 1; i >= 0; i--)
            {
                ItemElement element = itemLayout[i];
                if (element.item == null || element.texture == null)
                    continue;

                if (element.rect.Contains(point))
                {
                    // Get relative coordinates inside of item in 0-1 domain
                    float u = (point.x - element.rect.x) / element.rect.width;
                    float v = (point.y - element.rect.y) / element.rect.height;

                    // Read pixel colour at image coordinates inside of texture
                    // Remembering that Unity texture origin is bottom-left
                    int imageX = (int)(element.texture.width * u);
                    int imageY = (int)(element.texture.height * v);
                    Color color = element.texture.GetPixel(imageX, element.texture.height - 1 - imageY);

                    // Return equip slot index if found, otherwise keep on searching down stack
                    if (color.a != 0)
                        return (byte)element.item.EquipSlot;
                }
            }

            return 0xff;
        }

        #endregion

        #region Common Rendering Methods

        void DrawTexture(ImageData srcImage, DaggerfallUnityItem item = null)
        {
            DrawTexture(
                srcImage,
                new Rect(0, 0, 1, 1),
                new Rect(srcImage.offset.X, srcImage.offset.Y, srcImage.width, srcImage.height),
                item);
        }

        void DrawTexture(ImageData srcImage, Rect srcRect, Rect targetRect, DaggerfallUnityItem item = null)
        {
            // Calculate image position relative to origin
            int posX = (int)targetRect.xMin - paperDollOrigin.X;
            int posY = (int)targetRect.yMin - paperDollOrigin.Y;

            // Scale to paper doll render texture
            float scaleX = target.width / (float)paperDollWidth;
            float scaleY = target.height / (float)paperDollHeight;

            // Get target rect
            Rect screenRect = new Rect(
                posX * scaleX,
                posY * scaleY,
                targetRect.width * scaleX,
                targetRect.height * scaleY);

            // Allow for some adjustment so artist can finetune how their new item images should be positioned.
            TextureReplacement.OverridePaperdollItemRect(item, srcImage, scale, ref screenRect);

            // Draw with custom shader for paper doll item masking
            if (item != null)
            {
                paperDollMaterial.SetTexture("_MaskTex", srcImage.maskTexture);
                Graphics.DrawTexture(screenRect, srcImage.texture, srcRect, 0, 0, 0, 0, paperDollMaterial);
            }
            else
            {
                Graphics.DrawTexture(screenRect, srcImage.texture, srcRect, 0, 0, 0, 0);
            }

            // Store layout element for mouse picking
            if (item != null)
            {
                ItemElement element = new ItemElement();
                element.item = item;
                element.rect = screenRect;
                element.texture = srcImage.texture;
                itemLayout.Add(element);
            }
        }

        #endregion

        #region Body Rendering

        ImageData GetHeadImageData(PlayerEntity entity)
        {
            // Check for racial override head
            ImageData customHead;
            RacialOverrideEffect racialOverride = GameManager.Instance.PlayerEffectManager.GetRacialOverrideEffect();
            if (racialOverride != null && racialOverride.GetCustomHeadImageData(entity, out customHead))
                return customHead;

            // Otherwise just get standard head based on gender and race
            switch (entity.Gender)
            {
                default:
                case Genders.Male:
                    return ImageReader.GetImageData(entity.RaceTemplate.PaperDollHeadsMale, entity.FaceIndex, 0, true);
                case Genders.Female:
                    return ImageReader.GetImageData(entity.RaceTemplate.PaperDollHeadsFemale, entity.FaceIndex, 0, true);
            }
        }

        void BlitBody(PlayerEntity entity)
        {
            // Get gender-based body parts
            ImageData nudeBody = new ImageData();
            ImageData clothedBody = new ImageData();
            ImageData head = GetHeadImageData(entity);
            if (entity.Gender == Genders.Male)
            {
                nudeBody = ImageReader.GetImageData(entity.RaceTemplate.PaperDollBodyMaleUnclothed, 0, 0, true);
                clothedBody = ImageReader.GetImageData(entity.RaceTemplate.PaperDollBodyMaleClothed, 0, 0, true);
            }
            else if (entity.Gender == Genders.Female)
            {
                nudeBody = ImageReader.GetImageData(entity.RaceTemplate.PaperDollBodyFemaleUnclothed, 0, 0, true);
                clothedBody = ImageReader.GetImageData(entity.RaceTemplate.PaperDollBodyFemaleClothed, 0, 0, true);
            }
            else
            {
                return;
            }

            // Draw standard body
            DrawTexture(nudeBody);

            // Censor nudity if this setting enabled by using welded-on clothes.
            // But only if censored part of body is actually unclothed.
            // Otherwise welded-on clothes can be visible around equipped clothes.
            // This involves a special blit to draw top and bottom halves independently.
            if (!DaggerfallUnity.Settings.PlayerNudity)
            {
                if (!entity.ItemEquipTable.IsUpperClothed())
                    BlitUpperBody(clothedBody);

                if (!entity.ItemEquipTable.IsLowerClothed())
                    BlitLowerBody(clothedBody);
            }

            // Blit head
            DrawTexture(head);
        }

        // Special blit for upper half of player body
        void BlitUpperBody(ImageData body)
        {
            float waistSplit = (float)waistHeight / body.height;
            DrawTexture(
                body,
                new Rect(0, 1 - waistSplit, 1, 1 * waistSplit),
                new Rect(body.offset.X, body.offset.Y, body.width, body.height * waistSplit));
        }

        // Special blit for lower half of player body
        void BlitLowerBody(ImageData body)
        {
            float waistSplit = (float)waistHeight / body.height;
            DrawTexture(
                body,
                new Rect(0, 0, 1, 1 - waistSplit),
                new Rect(body.offset.X, body.offset.Y + body.height * waistSplit, body.width, body.height - body.height * waistSplit));
        }

        #endregion

        #region Item Rendering

        // Blit both cloak interiors
        void BlitCloakInterior(PlayerEntity entity)
        {
            // Draw cloak2 interior - stops here if this cloak is drawn
            DaggerfallUnityItem cloak2 = entity.ItemEquipTable.GetItem(EquipSlots.Cloak2);
            if (cloak2 != null)
            {
                ImageData interior2 = DaggerfallUnity.Instance.ItemHelper.GetCloakInteriorImage(cloak2);
                DrawTexture(interior2, cloak2);
                return;
            }

            // Draw cloak1 interior
            DaggerfallUnityItem cloak1 = entity.ItemEquipTable.GetItem(EquipSlots.Cloak1);
            if (cloak1 != null)
            {
                ImageData interior1 = DaggerfallUnity.Instance.ItemHelper.GetCloakInteriorImage(cloak1);
                DrawTexture(interior1, cloak1);
            }
        }

        // Blit items
        void BlitItems(PlayerEntity entity)
        {
            // Create list of all equipped items
            List<DaggerfallUnityItem> equippedItems = new List<DaggerfallUnityItem>();

            // Find equipped slots, skip empty slots
            foreach (var item in entity.ItemEquipTable.EquipTable)
            {
                if (item != null)
                    equippedItems.Add(item);
            }

            // Sort equipped items by draw order
            List<DaggerfallUnityItem> orderedItems = equippedItems.OrderBy(o => o.drawOrder).ToList();

            // Blit item images
            foreach (var item in orderedItems)
            {
                if (item.ItemGroup == ItemGroups.MensClothing || item.ItemGroup == ItemGroups.WomensClothing ||
                    item.ItemGroup == ItemGroups.Armor || item.ItemGroup == ItemGroups.Weapons || 
					(item.ItemGroup == ItemGroups.Jewellery && IsEquippedToBody(item)))
                {
                    BlitItem(item);
                }
            }
        }

        // Blits a normal item
        void BlitItem(DaggerfallUnityItem item)
        {
            ImageData source = DaggerfallUnity.Instance.ItemHelper.GetItemImage(item, true, true);
            DrawTexture(source, item);
        }

        // Equip slots > 11 are considered equipped to body
        bool IsEquippedToBody(DaggerfallUnityItem item)
        {
            return ((int)item.EquipSlot > 11);
        }

        #endregion
    }
}