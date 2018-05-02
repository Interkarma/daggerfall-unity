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
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.MagicAndEffects;

namespace DaggerfallWorkshop.Game.UserInterface
{
    /// <summary>
    /// Displays active spell icons on player HUD.
    /// </summary>
    public class HUDActiveSpells : Panel
    {
        #region Fields

        const int classicSelfStartX = 51;
        const int classicSelfStartY = 16;
        const int classicOtherStartX = 75;
        const int classicOtherStartY = 177;
        const int classicIconDim = 16;
        const int classicHorzSpacing = 24;

        List<ActiveSpell> activeSelfList = new List<ActiveSpell>();
        List<ActiveSpell> activeOtherList = new List<ActiveSpell>();

        #endregion

        #region Structs & Enums

        /// <summary>
        /// Stores information for display.
        /// </summary>
        public struct ActiveSpell
        {
            public int iconIndex;
            public string displayName;
        }

        /// <summary>
        /// Display styles for active spells on player as shown on HUD.
        /// </summary>
        public enum DisplayStyles
        {
            Classic,        // Classic horizontal row of icons with only light enhancements to placement handling
        }

        #endregion

        #region Constructors

        public HUDActiveSpells()
            : base()
        {
            EntityEffectBroker.OnNewMagicRound += EntityEffectBroker_OnNewMagicRound;
        }

        #endregion

        #region Public Methods

        ///// <summary>
        ///// Adds an active spell to self icon area.
        ///// </summary>
        //public void AddSelfSpell(int iconIndex, string displayName)
        //{
        //    ActiveSpell spell = new ActiveSpell();
        //    spell.iconIndex = iconIndex;
        //    spell.displayName = displayName;
        //    activeSelfList.Add(spell);
        //    Refresh();
        //}

        ///// <summary>
        ///// Manually refresh spell icon layout.
        ///// </summary>
        //public void Refresh()
        //{
        //    ClearChildren();

        //    // Can show up to first 10 spells at present
        //    int count = 0;
        //    float posX = classicSelfStartX;
        //    float posY = classicSelfStartY;
        //    foreach (ActiveSpell spell in activeSelfList)
        //    {
        //        Panel panel = new Panel();
        //        panel.BackgroundColor = Color.black;
        //        //panel.BackgroundTexture = DaggerfallUnity.Instance.ContentReader.SpellIconCollection.GetIcon(spell.iconIndex);
        //        Components.Add(panel);

        //        // Classic layout
        //        panel.Size = new Vector2(16, 16);
        //        panel.Position = new Vector2(posX, posY);
        //        posX += classicHorzSpacing;
        //        if (++count > 9)
        //            break;
        //    }
        //}

        #endregion

        #region Private Methods

        //void ClearChildren()
        //{
        //    Components.Clear();
        //}

        #endregion

        #region Event Handlers

        private void EntityEffectBroker_OnNewMagicRound()
        {
        }

        #endregion
    }
}