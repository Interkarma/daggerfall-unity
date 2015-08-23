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
    /// Entity stats
    /// </summary>
    public enum Stats
    {
        Strength,
        Intelligence,
        Willpower,
        Agility,
        Endurance,
        Personality,
        Speed,
        Luck,
    }

    /// <summary>
    /// Flags for entity special abilities.
    /// These appear to be consistent in ENEMY*.CFG and CLASS*.CFG files.
    /// </summary>
    [Flags]
    public enum SpecialAbilityFlags
    {
        None = 0,
        AcuteHearing = 1,
        Athleticism = 2,
        AdrenalineRush = 4,
        NoRegenSpellPoints = 8,
        SunDamage = 16,
        HolyDamage = 32,
    }

    /// <summary>
    /// Entity skills.
    /// The indices below match those in BIOG*.TXT files and CLASS*.CFG files.
    /// Likely to be the same indices using internally by the game.
    /// TEXT.RSC description records start at 1360, but are in a different order to below.
    /// </summary>
    public enum Skills
    {
        Medical = 0,
        Etiquette = 1,
        Streetwise = 2,
        Jumping = 3,
        Orcish = 4,
        Harpy = 5,
        Giantish = 6,
        Dragonish = 7,
        Nymph = 8,
        Daedrice = 9,
        Spriggan = 10,
        Centaurian = 11,
        Impish = 12,
        Lockpicking = 13,
        Mercantile = 14,
        Pickpocket = 15,
        Stealth = 16,
        Swimming = 17,
        Climbing = 18,
        Backstabbing = 19,
        Dodging = 20,
        Running = 21,
        Destruction = 22,
        Restoration = 23,
        Illusion = 24,
        Alteration = 25,
        Thaumaturgy = 26,
        Mysticism = 27,
        ShortBlade = 28,
        LongBlade = 29,
        HandToHand = 30,
        Axe = 31,
        BluntWeapon = 32,
        Archery = 33,
        CriticalStrike = 34,
    }

    /// <summary>
    /// Materials enum for forbidden materials.
    /// </summary>
    public enum Materials
    {
        Iron = 1,
        Steel = 2,
        Silver = 4,
        Elven = 8,
        Dwarven = 16,
        Mithril = 32,
        Adamantium = 64,
        Ebony = 128,
        Orcish = 256,
        Daedric = 512,
    }
}