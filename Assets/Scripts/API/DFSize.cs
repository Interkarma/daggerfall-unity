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

namespace DaggerfallConnect.Utility
{
    /// <summary>
    /// Stores size data.
    /// </summary>
    public class DFSize
    {
        #region Fields

        public int Width;

        public int Height;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DFSize()
        {
            Width = 0;
            Height = 0;
        }

        /// <summary>
        /// Value constructor.
        /// </summary>
        public DFSize(int width, int height)
        {
            Width = width;
            Height = height;
        }

        #endregion
    }
}
