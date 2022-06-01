// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    LypyL
// 
// Notes:
//

#region Using Statements
using System;
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

        public override string ToString()//##Lypyl
        {
            return string.Format("{0}, {1}", X, Y);
        }

        /// <summary>
        /// Get distance between 2 world points represented by DFPosition objects
        /// Returns -1 on error
        /// </summary>
        public double Distance(DFPosition pos)//##Lypyl
        {
            double dist = -1;
            try
            {
                dist = Math.Sqrt(Math.Pow(Math.Abs(X - pos.X), 2) + Math.Pow(Math.Abs(Y - pos.Y), 2));
            }
            catch (Exception ex)
            {
                DaggerfallWorkshop.DaggerfallUnity.LogMessage(ex.Message, true);
            }
            return dist;
        }

        #endregion
    }
}
