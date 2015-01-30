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
    public class DFPosition
    {
        #region Fields

        public int X;
        public int Y;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DFPosition()
        {
            X = 0;
            Y = 0;
        }

        /// <summary>
        /// Value constructor.
        /// </summary>
        public DFPosition(int x, int y)
        {
            X = x;
            Y = y;
        }

        #endregion
    }
}