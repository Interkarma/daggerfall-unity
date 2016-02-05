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
using System.Collections;
using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Player;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game.UserInterface
{
    public class CharacterPortrait : Panel
    {
        #region Fields

        PlayerEntity playerEntity;
        Panel characterBackgroundPanel = new Panel();
        Panel characterBodyPanel = new Panel();
        Panel characterHeadPanel = new Panel();
        DFPosition portraitOrigin = new DFPosition(200, 8);     // Used to translate hard-coded IMG file offsets back to origin

        string lastBackgroundName = string.Empty;
        string lastBodyName = string.Empty;
        string lastHeadName = string.Empty;

        #endregion

        #region Properties

        PlayerEntity PlayerEntity
        {
            get { return (playerEntity != null) ? playerEntity : playerEntity = GameManager.Instance.PlayerEntity; }
        }

        #endregion

        #region Constructors

        public CharacterPortrait()
        {
            Components.Add(characterBackgroundPanel);
            Components.Add(characterBodyPanel);
            Components.Add(characterHeadPanel);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Updates portrait layout to latest data.
        /// </summary>
        public void Refresh()
        {
            // Update character background
            if (lastBackgroundName != PlayerEntity.Race.PaperDollBackground)
            {
                Texture2D raceBackground = DaggerfallUI.GetTextureFromImg(PlayerEntity.Race.PaperDollBackground, new Rect(8, 7, 110, 184));
                characterBackgroundPanel.BackgroundTexture = raceBackground;
                characterBackgroundPanel.Size = new Vector2(raceBackground.width, raceBackground.height);
                lastBackgroundName = PlayerEntity.Race.PaperDollBackground;
            }

            // Get character body image name
            string paperDollBodyImageName = string.Empty;
            if (DaggerfallUnity.Settings.NoPlayerNudity)
            {
                if (PlayerEntity.Gender == Genders.Male)
                    paperDollBodyImageName = PlayerEntity.Race.PaperDollBodyMaleClothed;
                else
                    paperDollBodyImageName = PlayerEntity.Race.PaperDollBodyFemaleClothed;
            }
            else
            {
                if (PlayerEntity.Gender == Genders.Male)
                    paperDollBodyImageName = PlayerEntity.Race.PaperDollBodyMaleUnclothed;
                else
                    paperDollBodyImageName = PlayerEntity.Race.PaperDollBodyFemaleUnclothed;
            }

            // Update character body
            DFPosition offset;
            if (lastBodyName != paperDollBodyImageName)
            {
                Texture2D playerBodyTexture = DaggerfallUI.GetTextureFromImg(paperDollBodyImageName, out offset);
                characterBodyPanel.Size = new Vector2(playerBodyTexture.width, playerBodyTexture.height);
                characterBodyPanel.Position = new Vector2(offset.X - portraitOrigin.X, offset.Y - portraitOrigin.Y);
                characterBodyPanel.BackgroundTexture = playerBodyTexture;
                lastBodyName = paperDollBodyImageName;
            }

            // Get character head image name
            string paperDollHeadImageName = string.Empty;
            if (PlayerEntity.Gender == Genders.Male)
                paperDollHeadImageName = PlayerEntity.Race.PaperDollHeadsMale;
            else
                paperDollHeadImageName = PlayerEntity.Race.PaperDollHeadsFemale;

            // Update character head
            if (lastHeadName != paperDollHeadImageName)
            {
                Texture2D playerHeadTexture = DaggerfallUI.GetTextureFromCifRci(paperDollHeadImageName, PlayerEntity.FaceIndex, out offset);
                characterHeadPanel.Size = new Vector2(playerHeadTexture.width, playerHeadTexture.height);
                characterHeadPanel.Position = new Vector2(offset.X - portraitOrigin.X, offset.Y - portraitOrigin.Y);
                characterHeadPanel.BackgroundTexture = playerHeadTexture;
                lastHeadName = paperDollHeadImageName;
            }
        }

        #endregion

        #region Private Methods
        #endregion
    }
}