// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2020 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using System;
using System.Collections.Generic;
using UnityEngine;

namespace DaggerfallWorkshop.Game.Localization
{
    //  Note: Will expand and change over time until text database is complete.

    /// <summary>
    /// Each TextGroup stores one or more TextElements.
    /// </summary>
    public struct TextGroup
    {
        public string DisplayName;
        public LegacySources LegacySource;
        public string PrimaryKey;
        public string SecondaryKey;
        public string TertiaryKey;
        public List<TextElement> Elements;
    }

    public struct TextElement
    {
        public string Text;
        public FontStyles Font;
        public Color Color;
        public JustifyStyles Justify;
        public Vector2 Position;
        public int SyncVersion;
    }
}