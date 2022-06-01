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

namespace DaggerfallWorkshop.Localization
{
    //  Note: Will expand and change over time until localization is complete.
    
    /// <summary>
    /// Legacy text sources from classic Daggerfall.
    /// Used to reference where a text resource originated from.
    /// </summary>
    public enum LegacySources
    {
        None,
        TextRSC,
        FallEXE,
        QuestQRC,
        FactionTXT,
        FlatsCFG,
        BookTXT,
        MonsterBSA,
        MapsBSA,
        BioDAT,
        BiographyTXT,
        SpellsSTD,
    }

    /// <summary>
    /// New text groups for sorting text records.
    /// </summary>
    public enum TextGroups
    {
        Main,
        Books,
        Quests,
        Spells,
        Locations,
    }

    /// <summary>
    /// How to justify a text element.
    /// </summary>
    public enum JustifyStyles
    {
        None,
        Left,
        Right,
        Center,
    }

    /// <summary>
    /// Which font to use for text element.
    /// Mainly for books. Most other UIs disregard font selection at this time.
    /// </summary>
    public enum FontStyles
    {
        None = 0,
        Large = 1,              // FONT0000
        Title = 2,              // FONT0001
        Small = 3,              // FONT0002
        Default = 4,            // FONT0003
        Font5 = 5,              // FONT0005
    }
}