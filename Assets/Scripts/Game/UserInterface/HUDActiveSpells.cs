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

using System;
using System.Collections.Generic;
using UnityEngine;
using DaggerfallWorkshop.Game.MagicAndEffects;
using DaggerfallWorkshop.Game.Serialization;

namespace DaggerfallWorkshop.Game.UserInterface
{
    /// <summary>
    /// Displays active spell icons on player HUD.
    /// </summary>
    public class HUDActiveSpells : Panel
    {
        #region Fields

        const float blinkInterval = 0.25f;
        const int maxIconPool = 24;

        class IconsPositioning
        {
            public readonly Vector2 iconSize;

            public readonly Vector2 origin;
            public readonly Vector2 columnStep;
            public readonly Vector2 rowStep;

            public readonly int iconColumns;

            public IconsPositioning(Vector2 iconSize, Vector2 origin, Vector2 columnStep, Vector2 rowStep, int iconColumns)
            {
                this.iconSize = iconSize;
                this.origin = origin;
                this.columnStep = columnStep;
                this.rowStep = rowStep;
                this.iconColumns = iconColumns;
            }
        }

        IconsPositioning selfIconsPositioning;
        IconsPositioning otherIconsPositioning;

        Panel[] iconPool = new Panel[maxIconPool];
        List<ActiveSpellIcon> activeSelfList = new List<ActiveSpellIcon>();
        List<ActiveSpellIcon> activeOtherList = new List<ActiveSpellIcon>();

        bool blinkState = false;
        float blinkTimer = 0;
        ToolTip defaultToolTip = null;
        bool lastLargeHUD;

        #endregion

        #region Structs & Enums

        /// <summary>
        /// Stores information for icon display.
        /// </summary>
        public struct ActiveSpellIcon
        {
            public int iconIndex;
            public SpellIcon icon;
            public string displayName;
            public bool expiring;
            public int poolIndex;
            public bool isItem;
        }

        #endregion

        #region Constructors

        public HUDActiveSpells()
            : base()
        {
            AutoSize = AutoSizeModes.None;

            // Update icon state for player every round or when a bundle is assigned, removed, or state added
            EntityEffectBroker.OnNewMagicRound += UpdateIcons;
            GameManager.Instance.PlayerEffectManager.OnAssignBundle += UpdateIcons;
            GameManager.Instance.PlayerEffectManager.OnRemoveBundle += UpdateIcons;
            GameManager.Instance.PlayerEffectManager.OnAddIncumbentState += UpdateIcons;
            SaveLoadManager.OnLoad += SaveLoadManager_OnLoad;

            lastLargeHUD = DaggerfallUnity.Settings.LargeHUD;

            InitIcons();
        }

        #endregion

        #region Public Methods

        public override void Update()
        {
            base.Update();

            // Run blink timer
            blinkTimer += Time.deltaTime;
            if (blinkTimer > blinkInterval)
            {
                blinkTimer -= blinkInterval;
                blinkState = !blinkState;
            }

            // Blink expiring icons when game not paused
            // Otherwise always show icons when paused
            if (!GameManager.IsGamePaused)
            {
                SetIconBlinkState(activeSelfList, blinkState);
                SetIconBlinkState(activeOtherList, blinkState);
            }
            else
            {
                // Disable blink and update tooltips when paused
                SetIconBlinkState(activeSelfList, true);
                SetIconBlinkState(activeOtherList, true);
                if (defaultToolTip != null)
                    defaultToolTip.Update();
            }

            // Adjust icons when large HUD state changes
            if (DaggerfallUI.Instance.DaggerfallHUD != null &&
                DaggerfallUI.Instance.DaggerfallHUD.LargeHUD.Enabled != lastLargeHUD)
            {
                lastLargeHUD = DaggerfallUI.Instance.DaggerfallHUD.LargeHUD.Enabled;
                UpdateIcons();
            }
        }

        public override void Draw()
        {
            base.Draw();

            // Draw tooltips when paused or cursor active
            if ((GameManager.IsGamePaused || GameManager.Instance.PlayerMouseLook.cursorActive) && defaultToolTip != null)
                defaultToolTip.Draw();
        }

        #endregion

        #region Private Methods

        void SetIconBlinkState(List<ActiveSpellIcon> icons, bool state)
        {
            foreach(ActiveSpellIcon spell in icons)
            {
                if (spell.expiring && !spell.isItem)
                    iconPool[spell.poolIndex].Enabled = state;
            }
        }

        int GetMaxRoundsRemaining(LiveEffectBundle bundle)
        {
            // Get most remaining rounds of all effects
            // A spell can have multiple effects with different round durations
            int maxRoundsRemaining = 0;
            foreach (IEntityEffect effect in bundle.liveEffects)
            {
                if (effect.RoundsRemaining > maxRoundsRemaining)
                    maxRoundsRemaining = effect.RoundsRemaining;
            }

            return maxRoundsRemaining;
        }

        bool ShowIcon(LiveEffectBundle bundle)
        {
            // At least one effect with remaining rounds must want to show an icon, or be from an equipped item
            // Never show passive items specials icon, this is an internal system effect only
            foreach (IEntityEffect effect in bundle.liveEffects)
            {
                if (effect.Properties.ShowSpellIcon && (effect.RoundsRemaining >= 0 || bundle.fromEquippedItem != null))
                {
                    return true;
                }
            }

            return false;
        }

        void InitIcons()
        {
            switch (DaggerfallUnity.Settings.IconsPositioningScheme.ToLower())
            {
                case "classic":
                    // row of big buff icons at the top, wrapping downward, row of debuffs at the bottom, wrapping upward
                    selfIconsPositioning = new IconsPositioning(new Vector2(16, 16), new Vector2(27, 16), new Vector2(24, 0), new Vector2(0, 24), 12);
                    otherIconsPositioning = new IconsPositioning(new Vector2(16, 16), new Vector2(27, 177), new Vector2(24, 0), new Vector2(0, -24), 12);
                    break;
                case "medium":
                    // same as classic, slightly smaller icons
                    selfIconsPositioning = new IconsPositioning(new Vector2(12, 12), new Vector2(27, 16), new Vector2(16, 0), new Vector2(0, 16), 16);
                    otherIconsPositioning = new IconsPositioning(new Vector2(12, 12), new Vector2(27, 177), new Vector2(16, 0), new Vector2(0, -16), 16);
                    break;
                case "small":
                    // same as classic, even smaller icons, wrapped to stays on left side of screen
                    selfIconsPositioning = new IconsPositioning(new Vector2(8, 8), new Vector2(27, 16), new Vector2(10, 0), new Vector2(0, 10), 6);
                    otherIconsPositioning = new IconsPositioning(new Vector2(8, 8), new Vector2(27, 177), new Vector2(10, 0), new Vector2(0, -10), 6);
                    break;
                case "smalldeckleft":
                    // same as small, but slightly slanted toward the center of the screen
                    selfIconsPositioning = new IconsPositioning(new Vector2(8, 8), new Vector2(27, 28), new Vector2(10, -2), new Vector2(0, 10), 6);
                    otherIconsPositioning = new IconsPositioning(new Vector2(8, 8), new Vector2(27, 165), new Vector2(10, 2), new Vector2(0, -10), 6);
                    break;
                case "smalldeckright":
                    // same as smalldeckleft, but on the right side of screen
                    selfIconsPositioning = new IconsPositioning(new Vector2(8, 8), new Vector2(296, 28), new Vector2(-10, -2), new Vector2(0, 10), 6);
                    otherIconsPositioning = new IconsPositioning(new Vector2(8, 8), new Vector2(296, 165), new Vector2(-10, 2), new Vector2(0, -10), 6);
                    break;
                case "smallvertleft":
                    // Two aligned columns on the left, buffs going downward, debuffs going upward
                    selfIconsPositioning = new IconsPositioning(new Vector2(8, 8), new Vector2(27, 16), new Vector2(0, 10), new Vector2(10, 0), 10);
                    otherIconsPositioning = new IconsPositioning(new Vector2(8, 8), new Vector2(27, 177), new Vector2(0, -10), new Vector2(10, 0), 4);
                    break;
                case "smallvertright":
                    // Two aligned columns on the right, buffs going downward, debuffs going upward
                    selfIconsPositioning = new IconsPositioning(new Vector2(8, 8), new Vector2(296, 16), new Vector2(0, 10), new Vector2(-10, 0), 10);
                    otherIconsPositioning = new IconsPositioning(new Vector2(8, 8), new Vector2(296, 177), new Vector2(0, -10), new Vector2(-10, 0), 4);
                    break;
                case "smallhorzbottom":
                    // No wrapping, two rows at the bottom of screen, debuffs above buffs
                    selfIconsPositioning = new IconsPositioning(new Vector2(8, 8), new Vector2(27, 177), new Vector2(10, 0), new Vector2(0, 0), 0);
                    otherIconsPositioning = new IconsPositioning(new Vector2(8, 8), new Vector2(27, 167), new Vector2(10, 0), new Vector2(0, 0), 0);
                    break;
            }

            // Setup default tooltip
            if (DaggerfallUnity.Settings.EnableToolTips)
            {
                defaultToolTip = new ToolTip();
                defaultToolTip.ToolTipDelay = DaggerfallUnity.Settings.ToolTipDelayInSeconds;
                defaultToolTip.BackgroundColor = DaggerfallUnity.Settings.ToolTipBackgroundColor;
                defaultToolTip.TextColor = DaggerfallUnity.Settings.ToolTipTextColor;
                defaultToolTip.Parent = this;
            }

            // Setup icon panels
            for (int i = 0; i < iconPool.Length; i++)
            {
                iconPool[i] = new Panel
                {
                    BackgroundColor = Color.clear, // classic uses a black background
                    AutoSize = AutoSizeModes.None,
                    Enabled = false,
                    ToolTip = defaultToolTip
                };
                Components.Add(iconPool[i]);
            }
        }

        void ClearIcons()
        {
            for (int i = 0; i < iconPool.Length; i++)
            {
                iconPool[i].BackgroundTexture = null;
                iconPool[i].Enabled = false;
            }

            activeSelfList.Clear();
            activeOtherList.Clear();
        }

        void UpdateIcons(LiveEffectBundle bundleAdded)
        {
            UpdateIcons();
        }

        public void UpdateIcons()
        {
            ClearIcons();

            // Get all effect bundles currently operating on player
            EntityEffectManager playerEffectManager = GameManager.Instance.PlayerEffectManager;
            LiveEffectBundle[] effectBundles = playerEffectManager.EffectBundles;
            if (effectBundles == null || effectBundles.Length == 0)
                return;

            // Sort icons into active spells in self and other icon lists
            int poolIndex = 0;
            for (int i = 0;  i < effectBundles.Length; i++)
            {
                LiveEffectBundle bundle = effectBundles[i];

                // Don't add effect icon for instant spells, must have at least 1 round remaining or be from an equipped item
                if (!ShowIcon(bundle))
                    continue;

                // Setup icon information and sort into self (player is caster) or other (player not caster)
                // Need to check where spells cast by RDB actions are placed (e.g. click skull to cast levitate)
                // And offensive spells the player catches themselves with
                // Will need to refine how this works as more effects and situations become available
                ActiveSpellIcon item = new ActiveSpellIcon();
                item.displayName = bundle.name.TrimStart('!'); // Non-vendor spells start with !, don't show this on the UI
                item.iconIndex = bundle.iconIndex;
                item.icon = bundle.icon;
                item.poolIndex = poolIndex++;
                item.expiring = (GetMaxRoundsRemaining(bundle) < 2) ? true : false;
                item.isItem = (effectBundles[i].fromEquippedItem != null);
                if (bundle.caster == null || bundle.caster != GameManager.Instance.PlayerEntityBehaviour)
                    activeOtherList.Add(item);
                else
                    activeSelfList.Add(item);
            }

            // Update icon panels in pooled collection
            AlignIcons(activeSelfList, selfIconsPositioning);
            AlignIcons(activeOtherList, otherIconsPositioning);
        }

        void AlignIcons(List<ActiveSpellIcon> icons, IconsPositioning iconsPositioning)
        {
            Vector2 rowOrigin = iconsPositioning.origin;
            Vector2 position = rowOrigin;
            int column = 0;
            foreach (ActiveSpellIcon spell in icons)
            {
                if(spell.poolIndex < maxIconPool)
                {
                    Panel icon = iconPool[spell.poolIndex];
                    icon.Enabled = true;
                    icon.BackgroundTexture = DaggerfallUI.Instance.SpellIconCollection.GetSpellIcon(spell.icon);
                    icon.Position = position;
                    AdjustIconPositionForLargeHUD(icon);
                    icon.Size = iconsPositioning.iconSize;
                    icon.ToolTipText = spell.displayName;
                    if (++column == iconsPositioning.iconColumns)
                    {
                        rowOrigin += iconsPositioning.rowStep;
                        position = rowOrigin;
                        column = 0;
                    }
                    else
                    {
                        position += iconsPositioning.columnStep;
                    }
                }
            }
        }

        void AdjustIconPositionForLargeHUD(Panel icon)
        {
            // Adjust position for variable sized large HUD
            // Icon will remain in default position unless it needs to avoid being drawn under HUD
            if (DaggerfallUI.Instance.DaggerfallHUD != null && DaggerfallUI.Instance.DaggerfallHUD.LargeHUD.Enabled)
            {
                float startY = icon.Position.y;
                float offset = Screen.height - (int)DaggerfallUI.Instance.DaggerfallHUD.LargeHUD.Rectangle.height;
                float localY = (offset / LocalScale.y) - 18;
                if (localY < startY)
                    icon.Position = new Vector2(icon.Position.x, (int)localY);
            }
        }

        private void SaveLoadManager_OnLoad(SaveData_v1 saveData)
        {
            UpdateIcons();
        }

        #endregion
    }
}