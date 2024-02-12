// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Podleron (podleron@gmail.com)

namespace DaggerfallWorkshop.Game.Addons.RmbBlockEditor
{
    public static class GroundHelper
    {
        public static int GetRotationDeg(bool isRotated, bool isFlipped)
        {
            var rotation = 0;

            if (isRotated)
            {
                rotation -= 90;
            }

            if (isFlipped)
            {
                rotation += 180;
            }

            if (rotation < 0)
            {
                rotation = 270;
            }

            if (rotation > 270)
            {
                rotation = 0;
            }

            return rotation;
        }

        public static bool GetIsRotated(int deg)
        {
            var abs = deg % 360;
            return abs == 90 || abs == 270;
        }

        public static bool GetIsFlipped(int deg)
        {
            var abs = deg % 360;
            return abs == 90 || abs == 180;
        }
    }

}