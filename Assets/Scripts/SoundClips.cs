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
using System.Collections.Generic;

namespace DaggerfallWorkshop
{
    /// <summary>
    /// Enum for Daggerfall sound clips.
    /// This will be filled out over time.
    /// </summary>
    [Serializable]
    public enum SoundClips
    {
        None = -1,

        SpookyHigh = 0,
        SpookyMid = 1,
        SpookyLow = 2,

        ArrowShoot = 3,
        ArrowHit = 4,

        // No valid sound data
        Invalid = 5,

        AmbientCrickets = 6,

        PlayerDoorBash = 7,

        ArenaFireDaemon = 11,

        AmbientCreepyBirdLaughs = 13,
        AmbientCreepyBirdCall = 14,

        BodyFall = 15,

        Ignite = 16,

        ActivateGears = 19,
        ActivateDoorClose = 20,

        ArenaFootstep1 = 22,
        ArenaFootstep2 = 23,

        DungeonDoorClose = 24,
        DungeonDoorOpen = 25,

        AmbientDripShortHigh = 26,
        AmbientDripLongHigh = 27,
        AmbientDrums = 28,
        AmbientWindMoan2 = 29,
        ArenaHitSound = 30,

        ArenaFanfareLevelUp = 32,
        ArenaFanfareStaffOfChaos = 33,

        ArenaFireDaemon2 = 36,

        ArenaGhost = 38,
        ArenaGhoul = 39,

        ActivateGrind = 40,
        
        ArenaHellHound = 41,

        ArgonianMalePain3 = 42, // See 390-412

        // 43-62 are female pain sounds.
        // These are all unused in classic,
        // but seem to correspond to the order that it stores
        // the races. It sounds like they may go in increasing levels of pain,
        // or the third is supposed to be when the character dies.
        // These are IDs 267 through 286 in the sound file.
        // The final ones, IDs 287 through 290, are below at 424-427 
        BretonFemalePain1 = 43,
        BretonFemalePain2 = 44,
        BretonFemalePain3 = 45,
        RedguardFemalePain1 = 46,
        RedguardFemalePain2 = 47,
        RedguardFemalePain3 = 48,
        NordFemalePain1 = 49,
        NordFemalePain2 = 50,
        NordFemalePain3 = 51,
        DarkElfFemalePain1 = 52,
        DarkElfFemalePain2 = 53,
        DarkElfFemalePain3 = 54,
        HighElfFemalePain1 = 55,
        HighElfFemalePain2 = 56,
        HighElfFemalePain3 = 57,
        WoodElfFemalePain1 = 58,
        WoodElfFemalePain2 = 59,
        WoodElfFemalePain3 = 60,
        KhajiitFemalePain1 = 61,
        KhajiitFemalePain2 = 62,

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
        AmbientMonsterRoar = 73,
        AmbientGoldPieces = 74,
        AmbientBirdCall = 75,
        AmbientDoorClose = 76,

        FanfareSolo = 77,

        DrawWeapon = 78,

        CastSpell1 = 81,
        CastSpell2 = 82,
        CastSpell3 = 83,
        CastSpell4 = 84,
        CastSpell5 = 85,

        SpellImpactMagic = 86,
        SpellImpactPoison = 87,
        SpellImpactShock = 88,
        SpellImpactFire = 89,
        SpellImpactCold = 90,

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

        SwingLowPitch = 105,
        SwingHighPitch = 106,

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
        EnemyOrcShamanAttack = 138,

        EnemyGargoyleMove = 181,
        EnemyGargoyleBark = 182,
        EnemyGargoyleAttack = 180,

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

        PainSound = 299,

        DiceRoll = 300,

        ArenaMinotaur = 301,

        HaltWeak = 302,

        ArenaHomonculus = 305,

        ArenaIceGolem = 307,

        FootstepMetal = 308, // May be unused

        // Player footstep and movement sounds
        PlayerFootstepStone1 = 309,
        PlayerFootstepOutside1 = 310,
        PlayerFootstepSnow1 = 311,
        PlayerSwimming = 312,
        PlayerFootstepWood1 = 313,

        ArenaGoblin = 314,

        ActivateLockUnlock = 316,

        ArenaSkeleton = 318,
        ArenaLich = 319,
        ArenaLizardMan = 320,
        ArenaGhoul2 = 321,

        ActivateOpenGate = 325,

        ArenaOpenDoor = 326,
        ArenaOrc = 327,

        ActivateRatchet = 328,

        ArenaRat = 329,

        // Player footstep sounds for other foot
        PlayerFootstepStone2 = 330,
        PlayerFootstepOutside2 = 331,
        PlayerFootstepSnow2 = 332,
        PlayerFootstepWood2 = 333,

        SplashSmallLow = 334,

        HighPitchWail = 335,
        HighPitchScream = 336,

        ArenaSpider = 337,

        ArenaFootstep3 = 340,
        // 341 sounds similar to 340.

        SplashLarge = 342,

        ArenaStoneGolem = 344,
        ArenaStopThief = 345,

        SplashSmall = 346,

        SwingMediumPitch = 347,

        StormLightningShort = 348,
        StormLightningThunder = 349,
        StormThunderRoll = 350,

        ArenaTroll = 351,

        SwingMediumPitch2 = 353,

        ArenaVampire = 354,

        ArenaWolf = 357,
        ArenaWraith = 358,
        ArenaZombie = 359,

        ButtonClick = 360,

        GoldPieces = 361,

        PageTurn = 362,
        ParchmentScratching = 363,

        MakeItem = 364,
        MakePotion = 365,

        RaceBreton = 366,
        RaceRedguard = 367,
        RaceNord = 368,
        RaceDarkElf = 369,
        RaceHighElf = 370,
        RaceWoodElf = 371,
        RaceKhajiit = 372,
        RaceArgonian = 373,

        SelectClassDrums = 374,

        // 375 Sounds like tearing paper

        DiceRoll2 = 376,

        EquipShortBlade = 377,
        EquipLongBlade = 378,
        EquipTwoHandedBlade = 379,
        EquipStaff = 380,
        EquipClothing = 381,

        EquipJewellery = 383,
        OpenBook = 384,

        SnoringByFire = 385,
        MaleGasp = 386,
        FemaleGasp = 387,

        BlowingWindIntro = 388,
        AmbientRaining = 389,

        // 390-412 are male pain sounds.
        // These are unused in classic except for 405, which is used for player death,
        // but they seem to correspond to the order that it stores
        // the races.  It sounds like they may go in increasing levels of pain,
        // or the third is supposed to be when the character dies. These are ID 243 through 265 in the sound file.
        // The last Argonian Male pain effect, ID 266, is at index 42.
        BretonMalePain1 = 390,
        BretonMalePain2 = 391,
        BretonMalePain3 = 392,
        RedguardMalePain1 = 393,
        RedguardMalePain2 = 394,
        RedguardMalePain3 = 395,
        NordMalePain1 = 396,
        NordMalePain2 = 397,
        NordMalePain3 = 398,
        DarkElfMalePain1 = 399,
        DarkElfMalePain2 = 400,
        DarkElfMalePain3 = 401,
        HighElfMalePain1 = 402,
        HighElfMalePain2 = 403,
        HighElfMalePain3 = 404,
        WoodElfMalePain1 = 405,
        WoodElfMalePain2 = 406,
        WoodElfMalePain3 = 407,
        KhajiitMalePain1 = 408,
        KhajiitMalePain2 = 409,
        KhajiitMalePain3 = 410,
        ArgonianMalePain1 = 411,
        ArgonianMalePain2 = 412,

        EquipMaceOrHammer = 413,
        EquipFlail = 414,
        EquipAxe = 415,
        EquipBow = 416,
        EquipLeather = 417,
        EquipChain = 418,
        EquipPlate = 419,

        Burning = 420,

        // The below beep is ID 0 in sound file.
        // It was probably used to strongly alert content designer a sound was missing.
        LongBeep = 421,
        
        ArenaIronGolem = 422,

        SpookyHigh2 = 423,

        // Continuation of 43-62
        KhajiitFemalePain3 = 424,
        ArgonianFemalePain1 = 425,
        ArgonianFemalePain2 = 426,
        ArgonianFemalePain3 = 427,

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

        // 452 sounds like a boat creaking
        // 453 may be activation sound.
        // 454 is another wind sound.

        Vengeance = 455,

        Halt = 456,
        Halt2 = 457,

        Groan = 458,
    }
}