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
        const int maxIconRow = 10;
        const int maxIconPool = 20;

        const int classicSelfStartX = 51;
        const int classicSelfStartY = 16;
        const int classicOtherStartX = 75;
        const int classicOtherStartY = 177;
        const int classicIconDim = 16;
        const int classicHorzSpacing = 24;

        Panel[] iconPool = new Panel[maxIconPool];
        List<ActiveSpellIcon> activeSelfList = new List<ActiveSpellIcon>();
        List<ActiveSpellIcon> activeOtherList = new List<ActiveSpellIcon>();

        bool blinkState = false;
        float blinkTimer = 0;

        #endregion

        #region Structs & Enums

        /// <summary>
        /// Stores information for icon display.
        /// </summary>
        public struct ActiveSpellIcon
        {
            public int iconIndex;
            public string displayName;
            public bool expiring;
            public int poolIndex;
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
                SetIconBlinkState(activeSelfList, true);
                SetIconBlinkState(activeOtherList, true);
            }
        }

        #endregion

        #region Private Methods

        void SetIconBlinkState(List<ActiveSpellIcon> icons, bool state)
        {
            foreach(ActiveSpellIcon spell in icons)
            {
                if (spell.expiring)
                    iconPool[spell.poolIndex].Enabled = state;
            }
        }

        int GetMaxRoundsRemaining(EntityEffectManager.InstancedBundle bundle)
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

        bool HasEffectWithIcon(EntityEffectManager.InstancedBundle bundle)
        {
            // At least one effect must must to show an icon
            foreach (IEntityEffect effect in bundle.liveEffects)
            {
                if (effect.Properties.ShowSpellIcon)
                    return true;
            }

            return false;
        }

        void InitIcons()
        {
            for (int i = 0; i < iconPool.Length; i++)
            {
                iconPool[i] = new Panel();
                iconPool[i].BackgroundColor = Color.black;
                iconPool[i].AutoSize = AutoSizeModes.None;
                iconPool[i].Enabled = false;
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

        void UpdateIcons()
        {
            ClearIcons();

            // Get all effect bundles currently operating on player
            EntityEffectManager playerEffectManager = GameManager.Instance.PlayerEffectManager;
            EntityEffectManager.InstancedBundle[] effectBundles = playerEffectManager.EffectBundles;
            if (effectBundles == null || effectBundles.Length == 0)
                return;

            // Sort icons into active spells in self and other icon lists
            for (int i = 0;  i < effectBundles.Length; i++)
            {
                EntityEffectManager.InstancedBundle bundle = effectBundles[i];

                // Don't add effect icon for instant spells, must have at least 1 round remaining
                bool showIcon = HasEffectWithIcon(bundle);
                int maxRoundsRemaining = GetMaxRoundsRemaining(bundle);
                if (!showIcon || maxRoundsRemaining == 0)
                    continue;

                // Setup icon information and sort into self (player is caster) or other (player not caster)
                // Need to check where spells cast by RDB actions are placed (e.g. click skull to cast levitate)
                // And offensive spells the player catches themselves with
                // Will need to refine how this works as more effects and situations become available
                ActiveSpellIcon item = new ActiveSpellIcon();
                item.displayName = bundle.name;
                item.iconIndex = bundle.iconIndex;
                item.poolIndex = i;
                item.expiring = (maxRoundsRemaining <= 2) ? true : false;
                if (bundle.caster == null || bundle.caster != GameManager.Instance.PlayerEntityBehaviour)
                    activeOtherList.Add(item);
                else
                    activeSelfList.Add(item);
            }

            // Update icon panels in pooled collection
            AlignIcons(activeSelfList, classicSelfStartX, classicSelfStartY, classicIconDim, classicIconDim, classicHorzSpacing);
            AlignIcons(activeOtherList, classicOtherStartX, classicOtherStartY, classicIconDim, classicIconDim, classicHorzSpacing);
        }

        void AlignIcons(List<ActiveSpellIcon> icons, float xpos, float ypos, float width, float height, float xspacing, float yspacing = 0)
        {
            int count = 0;
            foreach (ActiveSpellIcon spell in icons)
            {
                iconPool[count].Enabled = true;
                iconPool[count].BackgroundTexture = DaggerfallUI.Instance.SpellIconCollection.GetSpellIcon(spell.iconIndex);
                iconPool[count].Position = new Vector2(xpos, ypos);
                iconPool[count].Size = new Vector2(width, height);
                xpos += xspacing;
                ypos += yspacing;
                if (++count > maxIconPool - 1)
                    break;
            }
        }

        private void SaveLoadManager_OnLoad(SaveData_v1 saveData)
        {
            UpdateIcons();
        }

        #endregion
    }
}