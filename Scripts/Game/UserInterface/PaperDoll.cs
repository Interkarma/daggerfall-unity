// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Daggerfall Workshop
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
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Utility;
using DaggerfallConnect.FallExe;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Player;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Items;

namespace DaggerfallWorkshop.Game.UserInterface
{
    /// <summary>
    /// Implements character paper doll.
    /// </summary>
    public class PaperDoll : Panel
    {
        #region UI Rects

        const int paperDollWidth = 110;
        const int paperDollHeight = 184;
        const int waistHeight = 40;

        Rect backgroundSubRect = new Rect(8, 7, paperDollWidth, paperDollHeight);

        #endregion

        #region Fields

        static Color32 maskColor = new Color(255, 0, 200, 0);   // Special mask colour used on helmets, cloaks, etc.
        DFPosition paperDollOrigin = new DFPosition(200, 8);    // Used to translate hard-coded IMG file offsets back to origin

        bool showBackgroundLayer = true;
        bool showCharacterLayer = true;
        bool showSelectionLayer = false;

        Color32[] paperDollColors;                  // Paper doll as shown to player
        Color32[] paperDollSelectionColors;         // Paper doll selection mask - using Color32 so it can be rendered visibly for testing

        Texture2D paperDollTexture;

        Panel backgroundPanel = new Panel();
        Panel characterPanel = new Panel();

        string lastBackgroundName = string.Empty;

        #endregion

        #region Properties

        public static Color32 MaskColor
        {
            get { return maskColor; }
        }

        #endregion

        #region Constructors

        public PaperDoll()
        {
            // Create target arrays
            paperDollColors = new Color32[paperDollWidth * paperDollHeight];
            paperDollSelectionColors = new Color32[paperDollWidth * paperDollHeight];

            // Setup panels
            characterPanel.Size = new Vector2(paperDollWidth, paperDollHeight);

            // Add panels
            Components.Add(backgroundPanel);
            Components.Add(characterPanel);

            // Set initial display flags
            backgroundPanel.Enabled = showBackgroundLayer;
            characterPanel.Enabled = showCharacterLayer;
        }

        #endregion

        #region Overrides

        public override void Update()
        {
            // Update display flags
            backgroundPanel.Enabled = showBackgroundLayer;
            characterPanel.Enabled = showCharacterLayer;

            base.Update();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Redraws paper doll image and selection mask.
        /// Call this after changing equipment, loading a new game, etc.
        /// Only call when required as constructing paper doll image is expensive.
        /// </summary>
        /// <param name="playerEntity"></param>
        public void Refresh(PlayerEntity playerEntity = null)
        {
            // Get current player entity if one not provided
            if (playerEntity == null)
                playerEntity = GameManager.Instance.PlayerEntity;

            // Update paper doll
            ClearColors(ref paperDollColors);
            ClearColors(ref paperDollSelectionColors);
            RefreshBackground(playerEntity);
            BlitBody(playerEntity);
            BlitItems(playerEntity);

            // Destroy old paper doll texture
            characterPanel.BackgroundTexture = null;
            GameObject.Destroy(paperDollTexture);
            paperDollTexture = null;

            // Update paper doll texture
            paperDollTexture = ImageReader.GetTexture(paperDollColors, paperDollWidth, paperDollHeight);
            characterPanel.BackgroundTexture = paperDollTexture;
        }

        #endregion

        #region Private Methods

        // Simple clear of array
        void ClearColors(ref Color32[] colors)
        {
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = Color.clear;
            }
        }

        // Update player background panel
        void RefreshBackground(PlayerEntity entity)
        {
            if (lastBackgroundName != entity.Race.PaperDollBackground)
            {
                ImageData data = ImageReader.GetImageData(entity.Race.PaperDollBackground, 0, 0, false, false);
                Texture2D texture = ImageReader.GetSubTexture(data, backgroundSubRect);

                backgroundPanel.BackgroundTexture = texture;
                backgroundPanel.Size = new Vector2(texture.width, texture.height);
                lastBackgroundName = entity.Race.PaperDollBackground;
            }
        }

        // Copy body parts to target
        void BlitBody(PlayerEntity entity)
        {
            // Get gender-based body parts
            ImageData nudeBody = new ImageData();
            ImageData clothedBody = new ImageData();
            ImageData head = new ImageData();
            if (entity.Gender == Genders.Male)
            {
                nudeBody = ImageReader.GetImageData(entity.Race.PaperDollBodyMaleUnclothed, 0, 0, true);
                clothedBody = ImageReader.GetImageData(entity.Race.PaperDollBodyMaleClothed, 0, 0, true);
                head = ImageReader.GetImageData(entity.Race.PaperDollHeadsMale, entity.FaceIndex, 0, true);
            }
            else if (entity.Gender == Genders.Female)
            {
                nudeBody = ImageReader.GetImageData(entity.Race.PaperDollBodyFemaleUnclothed, 0, 0, true);
                clothedBody = ImageReader.GetImageData(entity.Race.PaperDollBodyFemaleClothed, 0, 0, true);
                head = ImageReader.GetImageData(entity.Race.PaperDollHeadsFemale, entity.FaceIndex, 0, true);
            }
            else
            {
                return;
            }

            // Draw standard body
            BlitPaperDoll(nudeBody);

            // Censor nudity if this setting enabled by using welded-on clothes.
            // But only if censored part of body is actually unclothed.
            // Otherwise welded-on clothes can be visible around equipped clothes.
            // This involves a special blit to draw top and bottom halves independently.
            if (DaggerfallUnity.Settings.NoPlayerNudity)
            {
                if (!entity.ItemEquipTable.IsUpperClothed())
                    BlitUpperBody(clothedBody);

                if (!entity.ItemEquipTable.IsLowerClothed())
                    BlitLowerBody(clothedBody);
            }

            // Blit head
            BlitPaperDoll(head);
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
                {
                    equippedItems.Add(item);
                }
            }

            // Sort equipped items
            List<DaggerfallUnityItem> orderedItems = equippedItems.OrderBy(o => o.DrawOrder).ToList();

            // Blit item images
            foreach(var item in orderedItems)
            {
                // Get item image
                ImageData source = DaggerfallUnity.Instance.ItemHelper.GetItemImage(item);

                // Some items need special handling
                switch (item.TemplateIndex)
                {
                    case (int)MensClothing.Formal_cloak:
                    case (int)MensClothing.Casual_cloak:
                    case (int)WomensClothing.Formal_cloak:
                    case (int)WomensClothing.Casual_cloak:
                        BlitCloak(item);
                        break;

                    default:
                        BlitPaperDoll(source);
                        break;
                }
            }
        }

        // Formal/casual cloaks require special blit handling for cloak interior
        void BlitCloak(DaggerfallUnityItem item)
        {
            // Get cloak images
            ImageData interior = DaggerfallUnity.Instance.ItemHelper.GetCloakInteriorImage(item);
            ImageData exterior = DaggerfallUnity.Instance.ItemHelper.GetItemImage(item);

            // Blit images
            BlitPaperDoll(interior);
            BlitPaperDoll(exterior);
        }

        #endregion

        #region Custom Image Processing

        // Blit source ImageData onto paper doll
        void BlitPaperDoll(ImageData source)
        {
            Color32[] colors = ImageReader.GetColors(source);

            BlitImage(
                ref colors,
                ref paperDollColors,
                new Vector2(source.width, source.height),
                new Vector2(paperDollWidth, paperDollHeight),
                new Vector2(source.offset.X, source.offset.Y),
                maskColor);
        }

        // Blit source ImageData as an index mask onto selection layer
        void BlitSelectionLayer(ImageData source, int index)
        {
            Color32[] colors = ImageReader.GetColors(source);

            BlitImage(
                ref colors,
                ref paperDollSelectionColors,
                new Vector2(source.width, source.height),
                new Vector2(paperDollWidth, paperDollHeight),
                new Vector2(source.offset.X, source.offset.Y),
                maskColor,
                index);
        }

        // Special-purpose blit function to build paper doll image in 32-bit RGBA.
        // Copies source Color32 array into target Color32 array with mask colour and optional forced red channel value for selection masks
        public void BlitImage(
            ref Color32[] source,
            ref Color32[] target,
            Vector2 sourceSize,
            Vector2 targetSize,
            Vector2 targetPosition,
            Color32 maskColor,
            int selectionValue = -1)
        {
            // Calculate image offsets
            int xOffset = (int)targetPosition.x - paperDollOrigin.X;
            int yOffset = (int)((targetSize.y - sourceSize.y) - (targetPosition.y - paperDollOrigin.Y));

            // Create selection colour
            Color selectionColor = new Color32((byte)selectionValue, 0, 0, 128);

            // Copy image data
            for (int y = 0; y < sourceSize.y; y++)
            {
                for (int x = 0; x < sourceSize.x; x++)
                {
                    // Get source colour
                    Color col = source[y * (int)sourceSize.x + x];
                    if (col == Color.clear)
                        continue;

                    // Handle item masking
                    if (col == maskColor)
                        col = Color.clear;

                    // Handle forced value for generating selection mask
                    if (selectionValue >= 0)
                        col = selectionColor;

                    // Write to target
                    int targetOffset = (yOffset + y) * (int)targetSize.x + xOffset + x;
                    if (targetOffset >= 0 && targetOffset < target.Length)
                    {
                        target[targetOffset] = col;
                    }
                }
            }
        }

        #endregion

        #region ChildGuard Image Processing

        // Special blit for upper half of player body
        void BlitUpperBody(ImageData body)
        {
            // Get body above waist
            Color32[] colors = ImageReader.GetColors(body);
            Rect rect = new Rect(0, body.height - waistHeight, body.width, waistHeight);
            Color32[] newColors = ImageReader.GetSubColors(colors, rect, (int)rect.width, (int)rect.height);

            BlitImage(
                ref newColors,
                ref paperDollColors,
                new Vector2(rect.width, rect.height),
                new Vector2(paperDollWidth, paperDollHeight),
                new Vector2(body.offset.X, body.offset.Y),
                maskColor);
        }

        // Special blit for lower half of player body
        void BlitLowerBody(ImageData body)
        {
            Color32[] colors = ImageReader.GetColors(body);
            Rect rect = new Rect(0, 0, body.width, body.height - waistHeight);
            Color32[] newColors = ImageReader.GetSubColors(colors, rect, (int)rect.width, (int)rect.height);

            BlitImage(
                ref newColors,
                ref paperDollColors,
                new Vector2(rect.width, rect.height),
                new Vector2(paperDollWidth, paperDollHeight),
                new Vector2(body.offset.X, body.offset.Y + waistHeight),
                maskColor);
        }

        #endregion
    }
}