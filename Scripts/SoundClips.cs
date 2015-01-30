// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

using System;
using System.Collections.Generic;

namespace DaggerfallWorkshop
{
    /// <summary>
    /// Enum for Daggerfall sound clips.
    /// This will be filled out over time.
    /// </summary>
    public enum SoundClips
    {
        None = -1,

        SpookyHigh = 0,
        SpookyMid = 1,
        SpookyLow = 2,

        ArrowHit1 = 3,
        ArrowHit2 = 4,

        // No valid sound data
        Invalid = 5,

        AmbientCrickets = 6,

        PlayerDoorBash = 7,

        AmbientCreepyBirdLaughs = 13,
        AmbientCreepyBirdCall = 14,

        Ignite = 16,

        ActivateGears = 19,
        ActivateDoorClose = 20,

        Heartbeat1 = 22,
        Heartbeat2 = 23,

        DungeonDoorClose = 24,
        DungeonDoorOpen = 25,

        AmbientDripShortHigh = 26,
        AmbientDripLongHigh = 27,
        AmbientDrums = 28,
        AmbientWindMoan2 = 29,

        FanfarePart1 = 32,
        FanfarePart2 = 33,

        AmbientDistantMoan = 38,
        AmbientCloseMoan = 39,

        ActivateGrind = 40,

        // 42-62 Grunts and groans. Perhaps player pain sounds by race and gender?
        // Needs more research.

        AmbientDripShort = 63,
        AmbientDripLong = 64,
        AmbientWindMoan = 65,
        AmbientWindMoanDeep = 66,
        AmbientDoorOpen = 67,
        AmbientGrind = 68,
        AmbientStrumming = 69,
        AmbientWindBlow1 = 70,
        AmbientWindBlow1a = 71,
        AmbientWindBlow1b = 72,
        AmbientWindBlow2 = 73,
        AmbientMetalJangleLow = 74,
        AmbientBirdCall = 75,
        AmbientClank = 76,

        FanfareSolo = 77,

        DrawWeapon = 78,

        CastSpell1 = 81,
        CastSpell2 = 82,
        CastSpell3 = 83,
        CastSpell4 = 84,
        CastSpell5 = 85,
        CastSpell6 = 86,

        Clash = 87,

        SpellEffectZap = 88,
        SpellEffectExplosion = 89,
        SpellEffectBurn = 90,

        FallDamage = 91,
        FallHard = 92,

        NormalDoorClose = 93,
        NormalDoorOpen = 94,

        LongSlam = 95,

        LevelUp = 96,
        HorseClop = 97,

        Spooky2 = 98,

        AnimalHorse = 99,
        AnimalDog = 100,
        AnimalCat = 101,
        AnimalPig = 102,
        AnimalCow = 103,
        HorseAndCart = 104,

        EnemySwing1 = 105,
        EnemySwing2 = 106,

        Bells = 107,

        // 108-112 are weapon hit sounds
        Hit1 = 108,
        Hit2 = 109,
        Hit3 = 110,
        Hit4 = 111,
        Hit5 = 112,

        AmbientDistantHowl = 113,
        AmbientWaterBubbles = 114,

        #region Enemy Sounds

        // Enemy sounds generally follow same index order as textures and enemy definitions.
        // There are a few exceptions where sounds don't align numerically or a sound is missing.

        EnemyRatMove = 115,
        EnemyRatBark = 116,
        EnemyRatAttack = 117,

        EnemyImpMove = 118,
        EnemyImpBark = 119,
        EnemyImpAttack = 120,

        EnemySprigganMove = 121,
        EnemySprigganBark = 122,
        EnemySprigganAttack = 123,

        EnemyGiantBatMove = 124,
        EnemyGiantBatBark = 125,
        EnemyGiantBatAttack = 126,

        EnemyBearMove = 127,
        EnemyBearBark = 128,
        EnemyBearAttack = 129,

        EnemyTigerMove = 130,
        EnemyTigerBark = 131,
        EnemyTigerAttack = 132,

        EnemySpiderMove = 133,
        EnemySpiderBark = 134,
        EnemySpiderAttack = 135,

        EnemyOrcMove = 136,
        EnemyOrcBark = 137,
        EnemyOrcAttack = 138,

        EnemyCentaurMove = 139,
        EnemyCentaurBark = 140,
        EnemyCentaurAttack = 141,

        EnemyWerewolfMove = 142,
        EnemyWerewolfBark = 143,
        EnemyWerewolfAttack = 144,

        EnemyNymphMove = 145,
        EnemyNymphBark = 146,
        EnemyNymphAttack = 147,

        EnemyEelMove = 148,
        EnemyEelBark = 149,
        EnemyEelAttack = 150,

        EnemyOrcSergeantMove = 151,
        EnemyOrcSergeantBark = 152,
        EnemyOrcSergeantAttack = 153,

        EnemyHarpyMove = 154,
        EnemyHarpyBark = 155,
        EnemyHarpyAttack = 156,

        EnemyWereboarMove = 157,
        EnemyWereboarBark = 158,
        EnemyWereboarAttack = 159,

        EnemySkeletonMove = 160,
        EnemySkeletonBark = 161,
        EnemySkeletonAttack = 162,

        EnemyGiantMove = 163,
        EnemyGiantBark = 164,
        EnemyGiantAttack = 165,

        EnemyZombieMove = 166,
        EnemyZombieBark = 167,
        EnemyZombieAttack = 168,

        EnemyGhostMove = 169,
        EnemyGhostBark = 170,
        EnemyGhostAttack = 171,

        EnemyMummyMove = 172,
        EnemyMummyBark = 173,
        EnemyMummyAttack = 174,

        EnemyScorpionMove = 175,
        EnemyScorpionBark = 176,
        EnemyScorpionAttack = 177,

        EnemyOrcShamanMove = 178,
        EnemyOrcShamanBark = 179,
        EnemyOrcShamanAttack = 180,

        EnemyGargoyleMove = 181,
        EnemyGargoyleBark = 182,
        // Gargoyle attack sound missing.

        EnemyWraithMove = 183,
        EnemyWraithBark = 184,
        EnemyWraithAttack = 185,

        EnemyOrcWarlordMove = 186,
        EnemyOrcWarlordBark = 187,
        EnemyOrcWarlordAttack = 188,

        EnemyFrostDaedraMove = 189,
        EnemyFrostDaedraBark = 190,
        EnemyFrostDaedraAttack = 191,

        EnemyFireDaedraMove = 192,
        EnemyFireDaedraBark = 193,
        EnemyFireDaedraAttack = 194,

        EnemyLesserDaedraMove = 195,
        EnemyLesserDaedraBark = 196,
        EnemyLesserDaedraAttack = 197,

        EnemyFemaleVampireMove = 198,
        EnemyFemaleVampireBark = 199,
        EnemyFemaleVampireAttack = 200,

        EnemySeducerMove = 201,
        EnemySeducerBark = 202,
        EnemySeducerAttack = 203,

        EnemyVampireMove = 204,
        EnemyVampireBark = 205,
        EnemyVampireAttack = 206,

        EnemyDaedraLordMove = 207,
        EnemyDaedraLordBark = 208,
        EnemyDaedraLordAttack = 209,

        EnemyLichMove = 210,
        EnemyLichBark = 211,
        EnemyLichAttack = 212,

        EnemyLichKingMove = 213,
        EnemyLichKingBark = 214,
        EnemyLichKingAttack = 215,

        EnemyFaeryDragonMove = 216,
        EnemyFaeryDragonBark = 217,
        EnemyFaeryDragonAttack = 218,

        EnemyFireAtronachMove = 219,
        EnemyFireAtronachBark = 220,
        EnemyFireAtronachAttack = 221,

        EnemyIronAtronachMove = 222,
        EnemyIronAtronachBark = 223,
        EnemyIronAtronachAttack = 224,

        EnemyFleshAtronachMove = 225,
        EnemyFleshAtronachBark = 226,
        EnemyFleshAtronachAttack = 227,

        EnemyIceAtronachMove = 228,
        EnemyIceAtronachBark = 229,
        EnemyIceAtronachAttack = 230,

        EnemyCentaur2Move = 231,	    // This is Faery Dragon (#2) in texture order, but sounds like Centaur.
        EnemyCentaur2Bark = 232,
        EnemyCentaur2Attack = 233,

        EnemyImp2Move = 234,		    // This is Dreugh in texture order, but sounds like Imp.
        EnemyImp2Bark = 235,
        EnemyImp2Attack = 236,

        EnemyDreughMove = 237,		    // This is Lamia in texture order, but is actually Dreugh.
        EnemyDreughBark = 238,
        EnemyDreughAttack = 239,

        EnemyLamiaMove = 240,		    // This is past end of texture order, but is actually Lamia.
        EnemyLamiaBark = 241,
        EnemyLamiaAttack = 242,

        // Human sounds. These are mapped to the 18 human enemy types.
        // Note: Humans don't generally play their sounds in-game.

        EnemyHumanMove = 243,
        EnemyHumanBark = 244,           // Human bark sounds more like another "attack" sound.
        EnemyHumanAttack = 245,

        // 246-297 simply repeats 243-245 over and over.
        // There is no female variant of this triplet.

        #endregion

        HorseClop2 = 298,

        Gasp = 299,

        DiceRoll = 300,

        HaltWeak = 302,

        // Player footstep and movement sounds
        // These are played with variable pitch
        PlayerFootstepPuddle = 307,
        PlayerFootstepMetal = 308,
        PlayerFootstepNormal = 309,
        PlayerFootstepSnow = 310,
        PlayerFootstepUnknown = 311,
        PlayerSwimming = 312,
        PlayerFootstepWood = 313,

        ActivateLockUnlock = 316,

        ActivateOpenGate = 325,
        ActivateCreak = 326,

        ActivateRatchet = 328,

        AmbientSqueaks = 329,

        SplashSmallLow = 334,

        HighPitchWail = 335,
        HighPitchScream = 336,

        // 340-341 sounds like more player movement sounds
        // Needs more research.

        SplashLarge = 342,

        StopThief = 345,
        SplashSmall = 346,

        // Player swing pitch is changed based on weapon speed.
        PlayerSwing = 347,

        StormLightningShort = 348,
        StormLightningThunder = 349,
        StormThunderRoll = 350,

        PlayerSwing2 = 353,

        StrangledHowl = 357,
        LongMoanHigh = 358,
        LongMoanLow = 359,

        ButtonClick = 360,

        AmbientMetalJangleHigh = 361,

        PageTurn = 362,
        ParchmentScratching = 363,

        // 364-375 sound like racial music from player creation screen.
        // Fill these out later.

        DiceRoll2 = 376,

        Equip1 = 377,
        Equip2 = 378,
        Equip3 = 379,

        BlowingWindIntro = 388,
        AmbientRaining = 389,

        // 390-412 are more pain sounds.
        // Needs more research.

        // These seem to be equip sounds
        Equip4 = 413,
        Equip5 = 414,
        Equip6 = 415,

        Equip7 = 419,

        Burning = 420,

        // The below beep is ID 0 in sound file.
        // It was probably used to strongly alert content designer a sound was missing.
        LongBeep = 421,

        SpookyHigh2 = 423,

        // 428-436 are weapon parry sounds
        Parry1 = 428,
        Parry2 = 429,
        Parry3 = 430,
        Parry4 = 431,
        Parry5 = 432,
        Parry6 = 433,
        Parry7 = 434,
        Parry8 = 435,
        Parry9 = 436,

        BirdCall1 = 437,
        BirdCall2 = 438,

        WaterGentle = 439,
        WaterRough = 440,

        AmbientPeople1 = 441,
        AmbientPeople2 = 442,
        AmbientPeople3 = 443,
        AmbientPeople4 = 444,
        AmbientPeople5 = 445,
        AmbientPeople6 = 446,
        AmbientPeople7 = 447,
        AmbientPeople8 = 448,
        AmbientPeople9 = 449,
        AmbientPeople10 = 450,

        // 452-453 might be activation sounds.
        // Needs more research.

        Vengeance = 455,

        Halt = 456,
        Halt2 = 457,

        Groan = 458,
    }
}