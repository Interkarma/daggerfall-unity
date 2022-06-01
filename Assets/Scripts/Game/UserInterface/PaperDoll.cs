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
using DaggerfallConnect;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects;

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

        readonly DFSize backgroundFullSize = new DFSize(125, 198);
        readonly Rect backgroundSubRect = new Rect(8, 7, paperDollWidth, paperDollHeight);

        #endregion

        #region Fields

        static readonly Color32 maskColor = new Color(255, 0, 200, 0);   // Special mask colour used on helmets, cloaks, etc.

        const bool showBackgroundLayer = true;
        const bool showCharacterLayer = true;

        readonly Panel backgroundPanel = new Panel();
        readonly Panel characterPanel = new Panel();

        readonly TextLabel[] armourLabels = new TextLabel[DaggerfallEntity.NumberBodyParts];
        readonly Vector2[] armourLabelPos = new Vector2[] { new Vector2(70, 12), new Vector2(20, 38), new Vector2(86, 38), new Vector2(12, 58), new Vector2(6, 90), new Vector2(18, 120), new Vector2(22, 168) };

        string lastBackgroundName = string.Empty;
        bool showArmorLabels;

        #endregion

        #region Properties

        public static Color32 MaskColor
        {
            get { return maskColor; }
        }

        #endregion

        #region Constructors

        public PaperDoll(bool showArmorValues=true)
        {
            // Setup panels
            Size = new Vector2(paperDollWidth, paperDollHeight);
            characterPanel.Size = new Vector2(paperDollWidth, paperDollHeight);

            // Add panels
            Components.Add(backgroundPanel);
            Components.Add(characterPanel);

            // Set initial display flags
            backgroundPanel.Enabled = showBackgroundLayer;
            characterPanel.Enabled = showCharacterLayer;

            showArmorLabels = showArmorValues;
            if (showArmorLabels)
            {
                for (int bpIdx = 0; bpIdx < DaggerfallEntity.NumberBodyParts; bpIdx++)
                {
                    armourLabels[bpIdx] = DaggerfallUI.AddDefaultShadowedTextLabel(armourLabelPos[bpIdx], characterPanel);
                    armourLabels[bpIdx].Text = "0";
                }
            }
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

            // Racial override can suppress body and items
            bool suppressBody = false;
            RacialOverrideEffect racialOverride = GameManager.Instance.PlayerEffectManager.GetRacialOverrideEffect();
            if (racialOverride != null)
                suppressBody = racialOverride.SuppressPaperDollBodyAndItems;

            // Update background
            RefreshBackground(playerEntity);

            // Display paper doll render
            DaggerfallUI.Instance.PaperDollRenderer.Refresh(PaperDollRenderer.LayerFlags.All, playerEntity);
            characterPanel.BackgroundTexture = DaggerfallUI.Instance.PaperDollRenderer.PaperDollTexture;

            // Update armour values
            RefreshArmourValues(playerEntity, suppressBody);
        }

        /// <summary>
        /// Gets equip index at position.
        /// </summary>
        /// <param name="x">X position to sample.</param>
        /// <param name="y">Y position to sample.</param>
        /// <returns>Equip index or 0xff if point empty.</returns>
        public byte GetEquipIndex(int x, int y)
        {
            return DaggerfallUI.Instance.PaperDollRenderer.GetEquipIndex(x, y);
        }

        #endregion

        #region Private Methods

        // Refresh armour value labels
        void RefreshArmourValues(PlayerEntity playerEntity, bool suppress = false)
        {
            if (!showArmorLabels)
                return;

            for (int bpIdx = 0; bpIdx < DaggerfallEntity.NumberBodyParts; bpIdx++)
            {
                int armorMod = playerEntity.DecreasedArmorValueModifier - playerEntity.IncreasedArmorValueModifier;

                sbyte av = playerEntity.ArmorValues[bpIdx];
                int bpAv = (100 - av) / 5 + armorMod;
                armourLabels[bpIdx].Text = (!suppress) ? bpAv.ToString() : string.Empty;

                if (armorMod < 0)
                    armourLabels[bpIdx].TextColor = DaggerfallUI.DaggerfallUnityStatDrainedTextColor;
                else if (armorMod > 0)
                    armourLabels[bpIdx].TextColor = DaggerfallUI.DaggerfallUnityStatIncreasedTextColor;
                else
                    armourLabels[bpIdx].TextColor = DaggerfallUI.DaggerfallDefaultTextColor;
            }
        }

        // Update player background panel
        void RefreshBackground(PlayerEntity entity)
        {
            // Allow racial override background (vampire / transformed were-creature)
            // If racial override is not present or returns null then standard racial background will be used
            // The racial override has full control over which texture is displayed, such as when were-creature transformed or not
            Texture2D customBackground;
            RacialOverrideEffect racialOverride = GameManager.Instance.PlayerEffectManager.GetRacialOverrideEffect();
            if (racialOverride != null && racialOverride.GetCustomPaperDollBackgroundTexture(entity, out customBackground))
            {
                backgroundPanel.BackgroundTexture = customBackground;
                backgroundPanel.Size = new Vector2(paperDollWidth, paperDollHeight);
                lastBackgroundName = string.Empty;
                return;
            }

            // Use standard racial background
            string backgroundName = GetPaperDollBackground(entity);
            if (lastBackgroundName != backgroundName)
            {
                Texture2D texture = ImageReader.GetTexture(backgroundName, 0, 0, false);
                backgroundPanel.BackgroundTexture = ImageReader.GetSubTexture(texture, backgroundSubRect, backgroundFullSize);
                backgroundPanel.Size = new Vector2(paperDollWidth, paperDollHeight);
                lastBackgroundName = backgroundName;
            }
        }

        readonly char[] regionBackgroundIdxChars =
            {'3','1','2','2', '2','0','5','1', '5','2','1','1', '2','2','2','0', '2','0','2','2', '3','0','5','6', '2','2','2','2', '0','0','0','0',
             '0','6','6','6', '0','6','6','0', '6','0','0','3', '3','3','3','3', '3','5','5','5', '5','1','3','3', '3','2','0','0', '2','3' };

        string GetPaperDollBackground(PlayerEntity entity)
        {
            if (DaggerfallUnity.Settings.EnableGeographicBackgrounds)
            {
                PlayerGPS playerGPS = GameManager.Instance.PlayerGPS;
                PlayerEnterExit playerEnterExit = GameManager.Instance.PlayerEnterExit;
                DFPosition position = playerGPS.CurrentMapPixel;
                int region = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetPoliticIndex(position.X, position.Y) - 128;
                if (region < 0 || region >= DaggerfallUnity.Instance.ContentReader.MapFileReader.RegionCount || region >= regionBackgroundIdxChars.Length)
                    return entity.RaceTemplate.PaperDollBackground;

                // Set background based on location.
                if (playerGPS.IsPlayerInTown(true))
                    return "SCBG04I0.IMG";                                          // Town
                else if (playerEnterExit.IsPlayerInsideDungeon)
                    return "SCBG07I0.IMG";                                          // Dungeon
                else if (playerGPS.CurrentLocation.MapTableData.LocationType == DFRegion.LocationTypes.Graveyard && playerGPS.IsPlayerInLocationRect)
                    return "SCBG08I0.IMG";                                          // Graveyard
                else                            
                    return "SCBG0" + regionBackgroundIdxChars[region] + "I0.IMG";   // Region
            }
            else
            {
                return entity.RaceTemplate.PaperDollBackground;
            }
        }

        #endregion
    }
}