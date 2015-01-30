// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

#region Using Statements
using System;
using System.Text;
#endregion

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