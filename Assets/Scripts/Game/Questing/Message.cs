// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2016 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using System.Collections;

namespace DaggerfallWorkshop.Game.Questing
{
    /// <summary>
    /// A message stores text for popups, journal, letters, rumours, etc.
    /// </summary>
    public class Message : QuestResource
    {
        #region Fields

        int id;

        #endregion

        #region Properties

        public int ID
        {
            get { return id; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Message(Quest parentQuest)
            : base(parentQuest)
        {
        }

        /// <summary>
        /// Load message constructor.
        /// </summary>
        public Message(Quest parentQuest, int id, string[] source)
            : base(parentQuest)
        {
            LoadMessage(id, source);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Load 
        /// </summary>
        /// <param name="id">ID of message.</param>
        /// <param name="source">Array of source lines in message.</param>
        public void LoadMessage(int id, string[] source)
        {
            this.id = id;
        }

        #endregion

        #region Private Methods
        #endregion
    }
}