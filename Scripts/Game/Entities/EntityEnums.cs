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

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace DaggerfallWorkshop.Game.Entity
{
    /// <summary>
    /// Entity genders.
    /// </summary>
    public enum Genders
    {
        Male,
        Female,
    }

    /// <summary>
    /// Default races.
    /// </summary>
    public enum Races
    {
        None = 0,
        Breton = 1,
        Redguard = 2,
        Nord = 3,
        DarkElf = 4,
        HighElf = 5,
        WoodElf = 6,
        Khajiit = 7,
        Argonian = 8,
    }

    /// <summary>
    /// Default classes.
    /// </summary>
    public enum Classes
    {
        None = -1,
        Mage = 0,
        Spellsword = 1,
        Battlemage = 2,
        Sorcerer = 3,
        Healer = 4,
        Nightblade = 5,
        Bard = 6,
        Burglar = 7,
        Rogue = 8,
        Acrobat = 9,
        Thief = 10,
        Assassin = 11,
        Monk = 12,
        Archer = 13,
        Ranger = 14,
        Barbarian = 15,
        Warrior = 16,
        Knight = 17,
    }

    /// <summary>
    /// Player reflex settings for enemy speed.
    /// </summary>
    public enum PlayerReflexes
    {
        VeryHigh,
        High,
        Average,
        Low,
        VeryLow,
    }
}