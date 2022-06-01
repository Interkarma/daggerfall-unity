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
    /// Defines layout of background textures.
    /// </summary>
    public enum BackgroundLayout
    {
        Tile,                           // Texture is tiled inside parent
        StretchToFill,                  // Texture will stretch to fill parent
        ScaleToFit,                     // Texture will scale to fit parent while maintaining aspect ratio
        Cropped,                        // Texture is cropped and visible part scaled to fit
    }

    /// <summary>
    /// Defines how component should size itself relative to parent.
    /// </summary>
    public enum AutoSizeModes
    {
        None,                           // Panel will not resize or scale
        Scale,                          // Panel will use own scale
        ResizeToFill,                   // Panel will resize to fill parent vertically and horizontally
        ScaleToFit,                     // Panel will scale to fit parent while maintaining aspect ratio
        ScaleFreely,                    // Panel will scale to fit parent with no regard to aspect ratio
    }
}
