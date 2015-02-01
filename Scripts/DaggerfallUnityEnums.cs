// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

using System;

namespace DaggerfallWorkshop
{
    /// <summary>
    /// Types of billboard flats in Daggerfall blocks.
    /// </summary>
    public enum FlatTypes
    {
        Decoration,                             // Decorative flats in RMB and RDB blocks
        NPC,                                    // Non-player characters
        Editor,                                 // Editor/marker flats (TEXTURE.199)
        Animal,                                 // Animated animal flats (TEXTURE.201)
        Light,                                  // Light-source flats (TEXTURE.210)
        Nature,                                 // Climate nature flats (TEXTURE.500-TEXTURE.511)
    }

    /// <summary>
    /// Sub-types of editor flats.
    /// </summary>
    public enum EditorFlatTypes
    {
        Other,                                  // Unused sub-types
        Enter,                                  // Entrance point for dungeons
        Start,                                  // Starting point for cities after fast travel
        FixedMobile,                            // Fixed mobile enemy (same every load)
        RandomMobile,                           // Random mobile enemy (based on dungeon ecology and player level)
        RandomTreasure,                         // Random treasure pile
    }

    /// <summary>
    /// Location climate usage options.
    /// </summary>
    public enum LocationClimateUse
    {
        Disabled,                               // Don't use climate swaps
        UseLocation,                            // Use location climate settings
        Custom,                                 // Use custom climate settings
    }

    /// <summary>
    /// Base types for climate-aware texture sets.
    /// </summary>
    public enum ClimateBases
    {
        Desert,
        Mountain,
        Temperate,
        Swamp,
    }

    /// <summary>
    /// Climate season modifiers.
    /// </summary>
    public enum ClimateSeason
    {
        Summer,
        Winter,
        Rain,
    }

    /// <summary>
    /// Window textures modifiers.
    /// </summary>
    public enum WindowStyle
    {
        Disabled,
        Day,
        Night,
        Fog,
        Custom,
    }

    /// <summary>
    /// Weather texture modifiers.
    /// </summary>
    public enum WeatherStyle
    {
        Normal = 0,
        Rain1 = 4,
        Rain2 = 5,
        Snow1 = 6,
        Snow2 = 7,
    }

    /// <summary>
    /// Texture sets for nature flats.
    /// Note: Snow sets are only assigned by climate processing.
    /// </summary>
    public enum ClimateNatureSets
    {
        RainForest,              // TEXTURE.500
        SubTropical,             // TEXTURE.501
        Swamp,                   // TEXTURE.502
        Desert,                  // TEXTURE.503
        TemperateWoodland,       // TEXTURE.504
        //SnowWoodland,          // TEXTURE.505
        WoodlandHills,           // TEXTURE.506
        //SnowWoodlandHills,     // TEXTURE.507
        HauntedWoodlands,        // TEXTURE.508
        //SnowHauntedWoodlands,  // TEXTURE.509
        Mountains,               // TEXTURE.510
        //SnowMountains,         // TEXTURE.511
    }

    public enum DungeonTextureUse
    {
        /// <summary>Don't change dungeon textures.</summary>
        Disabled,
        /// <summary>Use dungeon location textures. Partially implemented.</summary>
        UseLocation_PartiallyImplemented,
        /// <summary>Use custom dungeon texture.</summary>
        Custom,
    }

    /// <summary>
    /// A list of mobile enemy types with ID range 0-42 (monsters) and 128-146 (humanoids).
    /// </summary>
    public enum MobileTypes
    {
        // Monster IDs are 0-42
        Rat,
        Imp,
        Spriggan,
        GiantBat,
        GrizzlyBear,
        SabertoothTiger,
        Spider,
        Orc,
        Centaur,
        Werewolf,
        Nymph,
        Slaughterfish,
        OrcSergeant,
        Harpy,
        Wereboar,
        SkeletalWarrior,
        Giant,
        Zombie,
        Ghost,
        Mummy,
        GiantScorpion,
        OrcShaman,
        Gargoyle,
        Wraith,
        OrcWarlord,
        FrostDaedra,
        FireDaedra,
        Daedroth,
        Vampire,
        DaedraSeducer,
        VampireAncient,
        DaedraLord,
        Lich,
        AncientLich,
        Dragonling,
        FireAtronach,
        IronAtronach,
        FleshAtronach,
        IceAtronach,
        Horse_Invalid,              // Not used and no matching texture (294 missing). Crashes DF when spawned in-game.
        Dragonling_Alternate,       // Another dragonling. Seems to work fine when spawned in-game.
        Dreugh,
        Lamia,

        // Humanoid IDs are 128-146
        Mage = 128,
        Spellsword,
        Battlemage,
        Sorcerer,
        Healer,
        Nightblade,
        Bard,
        Burglar,
        Rogue,
        Acrobat,
        Thief,
        Assassin,
        Monk,
        Archer,
        Ranger,
        Barbarian,
        Warrior,
        Knight,
        Knight_CityWatch,           // Just called Knight in-game, but renamed CityWatch here for uniqueness. HALT!

        // No enemy type
        None = (int)0xffff,
    }

    /// <summary>
    /// Mobile animation states.
    /// </summary>
    public enum MobileStates
    {
        Move,               // Records 0-4      (Flying and swimming mobs also uses this animation set for idle)
        PrimaryAttack,      // Records 5-9      (Usually a melee attack animation)
        Hurt,               // Records 10-14    (Mob has been struck)
        Idle,               // Records 15-19    (Frost and ice Daedra have animated idle states)
        RangedAttack1,      // Records 20-24    (Spellcast or bow attack based on mobile type)
        RangedAttack2,      // Records 25-29    (Bow attack on 475, 489, 490 only, absent on other humanoids)
        // TODO: Seducer transform special
    }

    /// <summary>
    /// Mobile enemy behaviour groups.
    /// </summary>
    public enum MobileBehaviour
    {
        General,            // General ground-based enemy
        Flying,             // Flying enemy
        Aquatic,            // Water-only enemy
        Spectral,           // Ghosts with flight and transparent effect
        Guard,              // City Watch - HALT!
    }

    /// <summary>
    /// Mobile affinity for resists/weaknesses, grouping, etc.
    /// This could be extended into a set of flags for multi-affinity creatures.
    /// </summary>
    public enum MobileAffinity
    {
        None,               // No special affinity
        Daylight,           // Daylight creatures (centaur, giant, nymph, spriggan, harpy, dragonling)
        Darkness,           // Darkness creatures (imp, gargoyle, orc, vampires, werecreatures)
        Undead,             // Undead monsters (skeleton, liches, zombie, mummy, ghosts)
        Animal,             // Animals (bat, rat, bear, tiger, spider, scorpion)
        Daedra,             // Daedra (daedroth, fire, frost, lord, seducer)
        Golem,              // Golems (flesh, fire, frost, iron)
        Water,              // Water creatures (dreugh, slaughterfish, lamia)
        Human,              // A human creature
    }

    /// <summary>
    /// Mobile gender.
    /// All monsters have an unspecified gender and no male/female variations.
    /// When specifying gender for humanoids, a value of unspecified will randomly choose between male/female.
    /// </summary>
    public enum MobileGender
    {
        Unspecified,
        Male,
        Female,
    }

    /// <summary>
    /// Reaction settings for mobiles.
    /// </summary>
    public enum MobileReactions
    {
        Hostile,            // Immediately hostile
        Passive,            // Not hostile unless attacked
        Custom,             // Reaction controlled elsewhere
    }

    /// <summary>
    /// Basic combat flags for mobiles.
    /// Every mobile has a basic melee attack available.
    /// This can be extended to create more diverse foes with
    /// a wider range of behaviours.
    /// </summary>
    public enum MobileCombatFlags
    {
        Ranged = 1,         // Ranged weapon available
        Spells = 2,         // Spellcasting available
    }

    /// <summary>
    /// Door types found around locations.
    /// </summary>
    public enum DoorTypes
    {
        None,                   // No door type detected
        Building,               // General building doors for both enctrance and exit
        DungeonEntrance,        // Enter a dungeon
        DungeonExit,            // Exit a dungeon
    }

    /// <summary>
    /// Various metal types in Daggerfall.
    /// </summary>
    public enum MetalTypes
    {
        None,
        Iron,
        Steel,
        Silver,
        Elven,
        Dwarven,
        Mithril,
        Adamantium,
        Ebony,
        Orcish,
        Daedric,
    }

    /// <summary>
    /// Generic weapon types in Daggerfall.
    /// </summary>
    public enum WeaponTypes
    {
        LongBlade,
        LongBlade_Magic,
        Staff,
        Staff_Magic,
        Dagger,
        Dagger_Magic,
        Mace,
        Mace_Magic,
        Flail,
        Flail_Magic,
        Warhammer,
        Warhammer_Magic,
        Battleaxe,
        Battleaxe_Magic,
        Bow,
        Melee,
        Werecreature,
    }

    /// <summary>
    /// Defines how a weapon is aligned.
    /// </summary>
    public enum WeaponAlignment
    {
        Left,
        Center,
        Right,
    }

    /// <summary>
    /// Weapon animation states.
    /// </summary>
    public enum WeaponStates
    {
        Idle,               // Record 0
        StrikeDown,         // Record 1
        StrikeDownLeft,     // Record 2
        StrikeLeft,         // Record 3
        StrikeRight,        // Record 4
        StrikeDownRight,    // Record 5
        StrikeUp,           // Record 6
    }

    /// <summary>
    /// Quick AudioSource presets for DaggerfallAudioSource.
    /// These make minor changes to peer AudioSource component.
    /// </summary>
    public enum AudioPresets
    {
        None,                   // No changes to AudioSource
        OnDemand,               // PlayOnAwake=false, Loop=false
        LoopOnAwake,            // PlayOnAwake=true, Loop=true
        LoopOnDemand,           // PlayOnAwake=false, Loop=true
        LoopIfPlayerNear,       // PlayOnAwake=true, Loop=true, distanceCheck=true
    }
}