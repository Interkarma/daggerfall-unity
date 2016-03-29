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

namespace DaggerfallWorkshop
{
    /// <summary>
    /// Supported non-alpha texture formats for texture reader.
    /// </summary>
    public enum SupportedNonAlphaTextureFormats
    {
        RGB565,
        RGB24,
    }

    /// <summary>
    /// Supported alpha texture formats for texure reader.
    /// </summary>
    public enum SupportedAlphaTextureFormats
    {
        RGBA444,
        ARGB444,
        RGBA32,
        ARGB32,
    }

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

    ///// <summary>
    ///// Defines core races.
    ///// The races have these indices in race picker image "TAMRIEL2.IMG"
    ///// This is also the order their bodies appear in the game files
    ///// </summary>
    //public enum Races
    //{
    //    None = 0,
    //    Breton = 1,
    //    Redguard = 2,
    //    Nord = 3,
    //    DarkElf = 4,
    //    HighElf = 5,
    //    WoodElf = 6,
    //    Khajiit = 7,
    //    Argonian = 8,
    //}

    /// <summary>
    /// A list of mobile enemy types with ID range 0-42 (monsters) and 128-146 (humanoids).
    /// Do not extend this enum.
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
    /// This is a Daggerfall Unity enum.
    /// For enum matching native data see Items/ItemEnums.cs.
    /// </summary>
    public enum MetalTypes
    {
        None,
        Iron,
        Steel,
        Chain,
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
    /// Available dye colours for armour, weapons, and clothing.
    /// Values match known Daggerfall colour indices.
    /// May change at a later date with new research.
    /// </summary>
    public enum DyeColors
    {
        // Clothing dyes
        Blue = 0,
        Grey = 1,
        Red = 2,
        DarkBrown = 3,
        Purple = 4,
        LightBrown = 5,
        White = 6,
        Aquamarine = 7,
        Yellow = 8,
        Green = 9,

        // 10-14 Unknown or not observed

        // Weapon and armour dyes
        Iron = 15,
        Steel = 16,
        Chain = 17,
        Unchanged = 18,
        SilverOrElven = 19,
        Dwarven = 20,
        Mithril = 21,
        Adamantium = 22,
        Ebony = 23,
        Orcish = 24,
        Daedric = 25,
    }

    /// <summary>
    /// Supported targets for dye changes.
    /// </summary>
    public enum DyeTargets
    {
        Clothing,
        WeaponsAndArmor,
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

    /// <summary>
    /// States for action doors and other objects.
    /// </summary>
    public enum ActionState
    {
        Start,
        PlayingForward,
        PlayingReverse,
        End,
    }

    /// <summary>
    /// Defines various types of living entities in the world.
    /// </summary>
    public enum EntityTypes
    {
        None,
        Player,
        CivilianNPC,
        StaticNPC,
        EnemyMonster,
        EnemyClass,
    }

    /// <summary>
    /// Supported ImageReader file types.
    /// </summary>
    public enum ImageTypes
    {
        None,
        TEXTURE,
        IMG,
        CIF,
        RCI,
    }

    /// <summary>
    /// Defines character's hands.
    /// </summary>
    public enum CharacterHands
    {
        None,
        Left,
        Right,
        Both,
    }

    /// <summary>
    /// Defines how an item is held.
    /// </summary>
    public enum ItemHands
    {
        None,               // Item is not held in the hands
        Either,             // Can wield in either left or right hand (off-hand equip image available)
        Both,               // Can wield in both hands only
        LeftOnly,           // Can wield in left hand only (e.g. shields)
        RightOnly,          // Can wield in right hand only
    }

    /// <summary>
    /// Various containers for inventory management.
    /// Not sure if all of these are used.
    /// May change at a later date.
    /// </summary>
    public enum ContainerTypes
    {
        Corpse1,
        Corpse2,
        Ground,
        Wagon,
        Shelves,
        Chest,
        Merchant,
        Table,
        Magic,
        Backpack,
        Corpse3,
    }

    /// <summary>
    /// Tags set in tag manager - not guarenteed to equal their enum value
    /// </summary>
    public enum Tags
    {
        ExampleTag00,
        ExampleTag01,
        ExampleTag02,
    }

    /// <summary>
    /// Layers set in tag manager - should always equal their enum value
    /// </summary>
    public enum Layers
    {
        ExampleLayer00 = 8,
        ExampleLayer12 = 12,
        Automap = 31
    }
}