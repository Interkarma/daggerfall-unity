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

namespace DaggerfallWorkshop
{
    public static class JobA
    {
        public static int Idx(int r, int c, int dim)
        {
            return r + (c * dim);
        }

        public static int Row(int index, int dim)
        {
            return index % dim;
        }

        public static int Col(int index, int dim)
        {
            return index / dim;
        }
    }
}