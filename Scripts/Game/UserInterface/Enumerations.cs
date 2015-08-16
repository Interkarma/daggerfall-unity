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

using System;

namespace DaggerfallWorkshop.Game.UserInterface
{
    /// <summary>
    /// Defines margins used in panels.
    /// </summary>
    [Flags]
    public enum Margins
    {
        None = 0,
        Top = 1,
        Bottom = 2,
        Left = 4,
        Right = 8,
        All = 15,
    }

    /// <summary>
    /// Defines horizontal alignment options.
    /// </summary>
    public enum HorizontalAlignment
    {
        None,
        Left,
        Center,
        Right,
    }

    /// <summary>
    /// Defines vertical alignment options.
    /// </summary>
    public enum VerticalAlignment
    {
        None,
        Top,
        Middle,
        Bottom,
    }

    /// <summary>
    /// Defines sides for various operations.
    /// </summary>
    [Flags]
    public enum Sides
    {
        None = 0,
        Top = 1,
        Bottom = 2,
        Left = 4,
        Right = 8,
        All = 15,
    }

    /// <summary>
    /// Defines slices for bitmap windows.
    /// </summary>
    public enum Slices
    {
        None,
        TopLeft,
        Top,
        TopRight,
        Left,
        Fill,
        Right,
        BottomLeft,
        Bottom,
        BottomRight,
    }

    /// <summary>
    /// Defines layout of certain textures, such as backgrounds.
    /// </summary>
    public enum TextureLayout
    {
        Tile,                           // Texture is tiled inside parent
        StretchToFill,                  // Texture will stretch to fill parent
        ScaleToFit,                     // Texture will scale to fit parent while maintaining aspect ratio
    }

    /// <summary>
    /// Defines how component should scale itself to parent.
    /// </summary>
    public enum Scaling
    {
        None,                           // Panel scale is fixed
        StretchToFill,                  // Panel will stretch to fill parent
        ScaleToFit,                     // Panel will scale to fit parent while maintaining aspect ratio
    }
}
