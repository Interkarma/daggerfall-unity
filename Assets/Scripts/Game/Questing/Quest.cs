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
    /// Contains live state of quests in play.
    /// Quests are instantiated from text source and executed inside quest machine.
    /// </summary>
    public class Quest
    {
        #region Fields
        #endregion

        #region Properties
        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Quest()
        {
        }

        /// <summary>
        /// Parse constructor.
        /// </summary>
        /// <param name="sourceName">Quest source.</param>
        public Quest(string[] source)
        {
            Parse(source);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Parse quest source into a quest object.
        /// </summary>
        /// <param name="source">Quest source.</param>
        public void Parse(string[] source)
        {
            Parser parser = new Parser();
            parser.Parse(source);
        }

        #endregion

        #region Private Methods
        #endregion
    }
}