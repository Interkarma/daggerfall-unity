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
    /// Stores colour data.
    /// </summary>
    public class DFColor
    {
        #region Fields

        public byte R;

        public byte G;

        public byte B;

        public byte A;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DFColor()
        {
        }

        /// <summary>
        /// Value constructor RGB.
        /// </summary>
        public DFColor(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
            A = 0xff;
        }

        /// <summary>
        /// Value constructor RGBA.
        /// </summary>
        public DFColor(byte r, byte g, byte b, byte a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        #endregion
    }
}
