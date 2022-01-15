// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes: All additions or modifications that differ from the source code copyright (c) 2021-2022 Osorkon
//

using System;
using System.Collections.Generic;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Items;
using UnityEngine;

namespace DaggerfallWorkshop.Utility
{
    /// <summary>
    /// Static definitions for enemies and their animations.
    /// Remaining data is read from MONSTER.BSA.
    /// </summary>
    public static class EnemyBasics
    {
        #region Enemy Animations

        // Speeds in frames-per-second
        public static int MoveAnimSpeed = 6;
        public static int FlyAnimSpeed = 10;
        public static int PrimaryAttackAnimSpeed = 10;
        public static int HurtAnimSpeed = 4;
        public static int IdleAnimSpeed = 4;
        public static int RangedAttack1AnimSpeed = 10;
        public static int RangedAttack2AnimSpeed = 10;

        // Move animations (double as idle animations for swimming and flying enemies, and enemies without idle animations)
        public static MobileAnimation[] MoveAnims = new MobileAnimation[]
        {
            new MobileAnimation() {Record = 0, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south (front facing player)
            new MobileAnimation() {Record = 1, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south-west
            new MobileAnimation() {Record = 2, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing west
            new MobileAnimation() {Record = 3, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing north-west
            new MobileAnimation() {Record = 4, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing north (back facing player)
            new MobileAnimation() {Record = 3, FramePerSecond = MoveAnimSpeed, FlipLeftRight = true},              // Facing north-east
            new MobileAnimation() {Record = 2, FramePerSecond = MoveAnimSpeed, FlipLeftRight = true},              // Facing east
            new MobileAnimation() {Record = 1, FramePerSecond = MoveAnimSpeed, FlipLeftRight = true},              // Facing south-east
        };

        // PrimaryAttack animations
        public static MobileAnimation[] PrimaryAttackAnims = new MobileAnimation[]
        {
            new MobileAnimation() {Record = 5, FramePerSecond = PrimaryAttackAnimSpeed, FlipLeftRight = false},    // Facing south (front facing player)
            new MobileAnimation() {Record = 6, FramePerSecond = PrimaryAttackAnimSpeed, FlipLeftRight = false},    // Facing south-west
            new MobileAnimation() {Record = 7, FramePerSecond = PrimaryAttackAnimSpeed, FlipLeftRight = false},    // Facing west
            new MobileAnimation() {Record = 8, FramePerSecond = PrimaryAttackAnimSpeed, FlipLeftRight = false},    // Facing north-west
            new MobileAnimation() {Record = 9, FramePerSecond = PrimaryAttackAnimSpeed, FlipLeftRight = false},    // Facing north (back facing player)
            new MobileAnimation() {Record = 8, FramePerSecond = PrimaryAttackAnimSpeed, FlipLeftRight = true},     // Facing north-east
            new MobileAnimation() {Record = 7, FramePerSecond = PrimaryAttackAnimSpeed, FlipLeftRight = true},     // Facing east
            new MobileAnimation() {Record = 6, FramePerSecond = PrimaryAttackAnimSpeed, FlipLeftRight = true},     // Facing south-east
        };

        // Hurt animations
        public static MobileAnimation[] HurtAnims = new MobileAnimation[]
        {
            new MobileAnimation() {Record = 10, FramePerSecond = HurtAnimSpeed, FlipLeftRight = false},            // Facing south (front facing player)
            new MobileAnimation() {Record = 11, FramePerSecond = HurtAnimSpeed, FlipLeftRight = false},            // Facing south-west
            new MobileAnimation() {Record = 12, FramePerSecond = HurtAnimSpeed, FlipLeftRight = false},            // Facing west
            new MobileAnimation() {Record = 13, FramePerSecond = HurtAnimSpeed, FlipLeftRight = false},            // Facing north-west
            new MobileAnimation() {Record = 14, FramePerSecond = HurtAnimSpeed, FlipLeftRight = false},            // Facing north (back facing player)
            new MobileAnimation() {Record = 13, FramePerSecond = HurtAnimSpeed, FlipLeftRight = true},             // Facing north-east
            new MobileAnimation() {Record = 12, FramePerSecond = HurtAnimSpeed, FlipLeftRight = true},             // Facing east
            new MobileAnimation() {Record = 11, FramePerSecond = HurtAnimSpeed, FlipLeftRight = true},             // Facing south-east
        };

        // Idle animations (most monsters have a static idle sprite)
        public static MobileAnimation[] IdleAnims = new MobileAnimation[]
        {
            new MobileAnimation() {Record = 15, FramePerSecond = IdleAnimSpeed, FlipLeftRight = false},            // Facing south (front facing player)
            new MobileAnimation() {Record = 16, FramePerSecond = IdleAnimSpeed, FlipLeftRight = false},            // Facing south-west
            new MobileAnimation() {Record = 17, FramePerSecond = IdleAnimSpeed, FlipLeftRight = false},            // Facing west
            new MobileAnimation() {Record = 18, FramePerSecond = IdleAnimSpeed, FlipLeftRight = false},            // Facing north-west
            new MobileAnimation() {Record = 19, FramePerSecond = IdleAnimSpeed, FlipLeftRight = false},            // Facing north (back facing player)
            new MobileAnimation() {Record = 18, FramePerSecond = IdleAnimSpeed, FlipLeftRight = true},             // Facing north-east
            new MobileAnimation() {Record = 17, FramePerSecond = IdleAnimSpeed, FlipLeftRight = true},             // Facing east
            new MobileAnimation() {Record = 16, FramePerSecond = IdleAnimSpeed, FlipLeftRight = true},             // Facing south-east
        };

        // RangedAttack1 animations (humanoid mobiles only)
        public static MobileAnimation[] RangedAttack1Anims = new MobileAnimation[]
        {
            new MobileAnimation() {Record = 20, FramePerSecond = RangedAttack1AnimSpeed, FlipLeftRight = false},   // Facing south (front facing player)
            new MobileAnimation() {Record = 21, FramePerSecond = RangedAttack1AnimSpeed, FlipLeftRight = false},   // Facing south-west
            new MobileAnimation() {Record = 22, FramePerSecond = RangedAttack1AnimSpeed, FlipLeftRight = false},   // Facing west
            new MobileAnimation() {Record = 23, FramePerSecond = RangedAttack1AnimSpeed, FlipLeftRight = false},   // Facing north-west
            new MobileAnimation() {Record = 24, FramePerSecond = RangedAttack1AnimSpeed, FlipLeftRight = false},   // Facing north (back facing player)
            new MobileAnimation() {Record = 23, FramePerSecond = RangedAttack1AnimSpeed, FlipLeftRight = true},    // Facing north-east
            new MobileAnimation() {Record = 22, FramePerSecond = RangedAttack1AnimSpeed, FlipLeftRight = true},    // Facing east
            new MobileAnimation() {Record = 21, FramePerSecond = RangedAttack1AnimSpeed, FlipLeftRight = true},    // Facing south-east
        };

        // RangedAttack2 animations (475, 489, 490 humanoid mobiles only)
        public static MobileAnimation[] RangedAttack2Anims = new MobileAnimation[]
        {
            new MobileAnimation() {Record = 25, FramePerSecond = RangedAttack2AnimSpeed, FlipLeftRight = false},   // Facing south (front facing player)
            new MobileAnimation() {Record = 26, FramePerSecond = RangedAttack2AnimSpeed, FlipLeftRight = false},   // Facing south-west
            new MobileAnimation() {Record = 27, FramePerSecond = RangedAttack2AnimSpeed, FlipLeftRight = false},   // Facing west
            new MobileAnimation() {Record = 28, FramePerSecond = RangedAttack2AnimSpeed, FlipLeftRight = false},   // Facing north-west
            new MobileAnimation() {Record = 29, FramePerSecond = RangedAttack2AnimSpeed, FlipLeftRight = false},   // Facing north (back facing player)
            new MobileAnimation() {Record = 28, FramePerSecond = RangedAttack2AnimSpeed, FlipLeftRight = true},    // Facing north-east
            new MobileAnimation() {Record = 27, FramePerSecond = RangedAttack2AnimSpeed, FlipLeftRight = true},    // Facing east
            new MobileAnimation() {Record = 26, FramePerSecond = RangedAttack2AnimSpeed, FlipLeftRight = true},    // Facing south-east
        };

        // Female thief idle animations
        public static MobileAnimation[] FemaleThiefIdleAnims = new MobileAnimation[]
        {
            new MobileAnimation() {Record = 15, FramePerSecond = IdleAnimSpeed, FlipLeftRight = false},            // Facing south (front facing player)
            new MobileAnimation() {Record = 11, FramePerSecond = IdleAnimSpeed, FlipLeftRight = false},            // Facing south-west
            new MobileAnimation() {Record = 17, FramePerSecond = IdleAnimSpeed, FlipLeftRight = false},            // Facing west
            new MobileAnimation() {Record = 18, FramePerSecond = IdleAnimSpeed, FlipLeftRight = false},            // Facing north-west
            new MobileAnimation() {Record = 19, FramePerSecond = IdleAnimSpeed, FlipLeftRight = false},            // Facing north (back facing player)
            new MobileAnimation() {Record = 18, FramePerSecond = IdleAnimSpeed, FlipLeftRight = true},             // Facing north-east
            new MobileAnimation() {Record = 17, FramePerSecond = IdleAnimSpeed, FlipLeftRight = true},             // Facing east
            new MobileAnimation() {Record = 11, FramePerSecond = IdleAnimSpeed, FlipLeftRight = true},             // Facing south-east
        };

        // Rat idle animations
        public static MobileAnimation[] RatIdleAnims = new MobileAnimation[]
        {
            new MobileAnimation() {Record = 15, FramePerSecond = IdleAnimSpeed, FlipLeftRight = false},            // Facing south (front facing player)
            new MobileAnimation() {Record = 16, FramePerSecond = IdleAnimSpeed, FlipLeftRight = true},             // Facing south-west
            new MobileAnimation() {Record = 17, FramePerSecond = IdleAnimSpeed, FlipLeftRight = true},             // Facing west
            new MobileAnimation() {Record = 18, FramePerSecond = IdleAnimSpeed, FlipLeftRight = true},             // Facing north-west
            new MobileAnimation() {Record = 19, FramePerSecond = IdleAnimSpeed, FlipLeftRight = false},            // Facing north (back facing player)
            new MobileAnimation() {Record = 18, FramePerSecond = IdleAnimSpeed, FlipLeftRight = false},            // Facing north-east
            new MobileAnimation() {Record = 17, FramePerSecond = IdleAnimSpeed, FlipLeftRight = false},            // Facing east
            new MobileAnimation() {Record = 16, FramePerSecond = IdleAnimSpeed, FlipLeftRight = false},            // Facing south-east
        };

        // Wraith and ghost idle/move animations
        public static MobileAnimation[] GhostWraithMoveAnims = new MobileAnimation[]
        {
            new MobileAnimation() {Record = 0, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south (front facing player)
            new MobileAnimation() {Record = 1, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south-west
            new MobileAnimation() {Record = 2, FramePerSecond = MoveAnimSpeed, FlipLeftRight = true},              // Facing west
            new MobileAnimation() {Record = 3, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing north-west
            new MobileAnimation() {Record = 4, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing north (back facing player)
            new MobileAnimation() {Record = 3, FramePerSecond = MoveAnimSpeed, FlipLeftRight = true},              // Facing north-east
            new MobileAnimation() {Record = 2, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing east
            new MobileAnimation() {Record = 1, FramePerSecond = MoveAnimSpeed, FlipLeftRight = true},              // Facing south-east
        };

        // Ghost and Wraith attack animations
        public static MobileAnimation[] GhostWraithAttackAnims = new MobileAnimation[]
        {
            new MobileAnimation() {Record = 5, FramePerSecond = PrimaryAttackAnimSpeed, FlipLeftRight = false},    // Facing south (front facing player)
            new MobileAnimation() {Record = 6, FramePerSecond = PrimaryAttackAnimSpeed, FlipLeftRight = false},    // Facing south-west
            new MobileAnimation() {Record = 7, FramePerSecond = PrimaryAttackAnimSpeed, FlipLeftRight = true},     // Facing west
            new MobileAnimation() {Record = 8, FramePerSecond = PrimaryAttackAnimSpeed, FlipLeftRight = false},    // Facing north-west
            new MobileAnimation() {Record = 9, FramePerSecond = PrimaryAttackAnimSpeed, FlipLeftRight = false},    // Facing north (back facing player)
            new MobileAnimation() {Record = 8, FramePerSecond = PrimaryAttackAnimSpeed, FlipLeftRight = true},     // Facing north-east
            new MobileAnimation() {Record = 7, FramePerSecond = PrimaryAttackAnimSpeed, FlipLeftRight = false},    // Facing east
            new MobileAnimation() {Record = 6, FramePerSecond = PrimaryAttackAnimSpeed, FlipLeftRight = true},     // Facing south-east
        };

        // Seducer special animations - has player-facing orientation only
        public static MobileAnimation[] SeducerTransform1Anims = new MobileAnimation[]
        {
            new MobileAnimation() {Record = 23, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south (front facing player)
            new MobileAnimation() {Record = 23, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south (front facing player)
            new MobileAnimation() {Record = 23, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south (front facing player)
            new MobileAnimation() {Record = 23, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south (front facing player)
            new MobileAnimation() {Record = 23, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south (front facing player)
            new MobileAnimation() {Record = 23, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south (front facing player)
            new MobileAnimation() {Record = 23, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south (front facing player)
            new MobileAnimation() {Record = 23, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south (front facing player)
        };
        public static MobileAnimation[] SeducerTransform2Anims = new MobileAnimation[]
        {
            new MobileAnimation() {Record = 22, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south (front facing player)
            new MobileAnimation() {Record = 22, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south (front facing player)
            new MobileAnimation() {Record = 22, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south (front facing player)
            new MobileAnimation() {Record = 22, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south (front facing player)
            new MobileAnimation() {Record = 22, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south (front facing player)
            new MobileAnimation() {Record = 22, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south (front facing player)
            new MobileAnimation() {Record = 22, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south (front facing player)
            new MobileAnimation() {Record = 22, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south (front facing player)
        };
        public static MobileAnimation[] SeducerIdleMoveAnims = new MobileAnimation[]
        {
            new MobileAnimation() {Record = 21, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south (front facing player)
            new MobileAnimation() {Record = 21, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south (front facing player)
            new MobileAnimation() {Record = 21, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south (front facing player)
            new MobileAnimation() {Record = 21, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south (front facing player)
            new MobileAnimation() {Record = 21, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south (front facing player)
            new MobileAnimation() {Record = 21, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south (front facing player)
            new MobileAnimation() {Record = 21, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south (front facing player)
            new MobileAnimation() {Record = 21, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south (front facing player)
        };
        public static MobileAnimation[] SeducerAttackAnims = new MobileAnimation[]
        {
            new MobileAnimation() {Record = 20, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south (front facing player)
            new MobileAnimation() {Record = 20, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south (front facing player)
            new MobileAnimation() {Record = 20, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south (front facing player)
            new MobileAnimation() {Record = 20, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south (front facing player)
            new MobileAnimation() {Record = 20, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south (front facing player)
            new MobileAnimation() {Record = 20, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south (front facing player)
            new MobileAnimation() {Record = 20, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south (front facing player)
            new MobileAnimation() {Record = 20, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south (front facing player)
        };

        // Slaughterfish special idle/move animation - needs to bounce back and forth between frame 0-N rather than loop
        // Move animations (double as idle animations for swimming and flying enemies, and enemies without idle animations)
        public static MobileAnimation[] SlaughterfishMoveAnims = new MobileAnimation[]
        {
            new MobileAnimation() {Record = 0, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false, BounceAnim = true},   // Facing south (front facing player)
            new MobileAnimation() {Record = 1, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false, BounceAnim = true},   // Facing south-west
            new MobileAnimation() {Record = 2, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false, BounceAnim = true},   // Facing west
            new MobileAnimation() {Record = 3, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false, BounceAnim = true},   // Facing north-west
            new MobileAnimation() {Record = 4, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false, BounceAnim = true},   // Facing north (back facing player)
            new MobileAnimation() {Record = 3, FramePerSecond = MoveAnimSpeed, FlipLeftRight = true, BounceAnim = true},    // Facing north-east
            new MobileAnimation() {Record = 2, FramePerSecond = MoveAnimSpeed, FlipLeftRight = true, BounceAnim = true},    // Facing east
            new MobileAnimation() {Record = 1, FramePerSecond = MoveAnimSpeed, FlipLeftRight = true, BounceAnim = true},    // Facing south-east
        };

        #endregion

        #region Enemy Definitions

        // Defines additional data for known enemy types
        // Fills in the blanks where source of data in game files is unknown
        // Suspect at least some of this data is also hard-coded in Daggerfall

        // [OSORKON] In BOSSFALL spawn frequency generally determines monster difficulty. Common enemies are weak
        // and low level. As spawn frequency falls, monster Armor, HP, Level, and damage smoothly rise. Rare enemies
        // are very dangerous compared to vanilla. HP doesn't follow the standard DFU xd8 + 10 pattern - all monsters
        // have MaxHealth of (MinHealth * 3). Non-boss human HP is unchanged. All non-boss monsters (except for those
        // with Bonus To Hit: Humanoids) have a minimum damage of 1. Added a new MoveSpeed field to every enemy to
        // greatly reduce movespeed checks done in EnemyMotor. Empty soul gems cost 50,000 gold so I added 50,000
        // to all SoulPts for consistency otherwise filled soul gems would be too cheap. Only bosses and high-level
        // monsters see Invisible, human spellcasters above level 15 also see Invisible. Finally, I set every MinMetalToHit
        // to None. Weaknesses/resistances/immunities are added in the CalculateAttackDamage function in FormulaHelper.
        public static MobileEnemy[] Enemies = new MobileEnemy[]
        {
            // Rat
            new MobileEnemy()
            {
                ID = 0,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Animal,
                MaleTexture = 255,
                FemaleTexture = 255,
                CorpseTexture = CorpseTexture(401, 1),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                MoveSound = (int)SoundClips.EnemyRatMove,
                BarkSound = (int)SoundClips.EnemyRatBark,
                AttackSound = (int)SoundClips.EnemyRatAttack,
                MinMetalToHit = WeaponMaterialTypes.None,

                // [OSORKON] Slightly less damage and armor than vanilla, similar HP. They move somewhat fast.
                // Rats are very common in dungeons, rare in the wilderness during the day and fairly common
                // at night. 5% chance per hit of infecting player with the Plague, and they always charge.
                // They never appear in towns at night.
                MinDamage = 1,
                MaxDamage = 3,
                MinHealth = 6,
                MaxHealth = 18,
                Level = 1,
                ArmorValue = 7,
                ParrySounds = false,
                MapChance = 0,
                Weight = 2,
                SoulPts = 50000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, 2, -1, 3, 4, 5 },
                Team = MobileTeams.Vermin,
                MoveSpeed = 6.5f,
            },

            // Imp
            new MobileEnemy()
            {
                ID = 1,
                Behaviour = MobileBehaviour.Flying,
                Affinity = MobileAffinity.Darkness,
                MaleTexture = 256,
                FemaleTexture = 256,
                CorpseTexture = CorpseTexture(406, 5),
                HasIdle = false,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyImpMove,
                BarkSound = (int)SoundClips.EnemyImpBark,
                AttackSound = (int)SoundClips.EnemyImpAttack,
                MinMetalToHit = WeaponMaterialTypes.None,

                // [OSORKON] Much less damage and armor, slightly more HP. Their spells are a lot nastier and
                // they move somewhat slowly. They are never found outside and are only present in a few dungeon
                // types. I figure they are familiars to spellcasters and don't wander around on their own.
                MinDamage = 1,
                MaxDamage = 10,
                MinHealth = 8,
                MaxHealth = 24,
                Level = 2,
                ArmorValue = 5,
                ParrySounds = false,
                MapChance = 1,
                Weight = 40,
                LootTableKey = "D",
                SoulPts = 51000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, 2, -1, 3, 1 },
                SpellAnimFrames = new int[] { 0, 1, 2, 3, 1 },
                Team = MobileTeams.Magic,
                MoveSpeed = 5.5f,
            },

            // Spriggan
            new MobileEnemy()
            {
                ID = 2,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Daylight,
                MaleTexture = 257,
                FemaleTexture = 257,
                CorpseTexture = CorpseTexture(406, 3),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemySprigganMove,
                BarkSound = (int)SoundClips.EnemySprigganBark,
                AttackSound = (int)SoundClips.EnemySprigganAttack,
                MinMetalToHit = WeaponMaterialTypes.None,

                // [OSORKON] Much less damage and armor, more HP. Immune to Archery, resistant to Short Blades and
                // Hand-to-Hand, takes double damage from Axes. They move extremely slowly. They are in Natural Cave
                // dungeon types and very common in wilderness during the day anywhere except Deserts. Not as active
                // during the night.
                MinDamage = 1,
                MaxDamage = 4,
                MinDamage2 = 1,
                MaxDamage2 = 4,
                MinDamage3 = 1,
                MaxDamage3 = 4,
                MinHealth = 14,
                MaxHealth = 42,
                Level = 3,
                ArmorValue = 6,
                ParrySounds = false,
                MapChance = 0,
                Weight = 240,
                LootTableKey = "B",
                SoulPts = 51000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2, 3, 3, 3 },
                Team = MobileTeams.Spriggans,
                MoveSpeed = 3f,
            },

            // Giant Bat
            new MobileEnemy()
            {
                ID = 3,
                Behaviour = MobileBehaviour.Flying,
                Affinity = MobileAffinity.Animal,
                MaleTexture = 258,
                FemaleTexture = 258,
                CorpseTexture = CorpseTexture(401, 0),
                HasIdle = false,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                MoveSound = (int)SoundClips.EnemyGiantBatMove,
                BarkSound = (int)SoundClips.EnemyGiantBatBark,
                AttackSound = (int)SoundClips.EnemyGiantBatAttack,
                MinMetalToHit = WeaponMaterialTypes.None,

                // [OSORKON] Less damage and HP, slightly better armor. They move very fast. Very common in dungeons,
                // outside they prefer warmer climates. They never spawn outside during the day or in towns at night,
                // but are common in nighttime wilderness. 5% chance per hit of transmitting disease. They always charge.
                MinDamage = 1,
                MaxDamage = 7,
                MinHealth = 8,
                MaxHealth = 24,
                Level = 3,
                ArmorValue = 5,
                ParrySounds = false,
                MapChance = 0,
                Weight = 80,
                SoulPts = 50000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2, 3 },
                Team = MobileTeams.Vermin,
                MoveSpeed = 8f,
            },

            // Grizzly Bear
            new MobileEnemy()
            {
                ID = 4,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Animal,
                MaleTexture = 259,
                FemaleTexture = 259,
                CorpseTexture = CorpseTexture(401, 2),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                MoveSound = (int)SoundClips.EnemyBearMove,
                BarkSound = (int)SoundClips.EnemyBearBark,
                AttackSound = (int)SoundClips.EnemyBearAttack,
                MinMetalToHit = WeaponMaterialTypes.None,

                // [OSORKON] Less damage, much more HP. They move somewhat fast. They are only in Natural Cave
                // dungeon types. Bears love mountain and woodland wilderness, day or night. They always charge.
                MinDamage = 1,
                MaxDamage = 6,
                MinDamage2 = 1,
                MaxDamage2 = 6,
                MinDamage3 = 1,
                MaxDamage3 = 6,
                MinHealth = 33,
                MaxHealth = 99,
                Level = 4,
                ArmorValue = 6,
                ParrySounds = false,
                MapChance = 0,
                Weight = 1000,
                SoulPts = 50000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, 2, -1, 3, 0 },
                Team = MobileTeams.Bears,
                MoveSpeed = 6.75f,
            },

            // Sabertooth Tiger
            new MobileEnemy()
            {
                ID = 5,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Animal,
                MaleTexture = 260,
                FemaleTexture = 260,
                CorpseTexture = CorpseTexture(401, 3),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                MoveSound = (int)SoundClips.EnemyTigerMove,
                BarkSound = (int)SoundClips.EnemyTigerBark,
                AttackSound = (int)SoundClips.EnemyTigerAttack,
                MinMetalToHit = WeaponMaterialTypes.None,

                // [OSORKON] Much less damage, much more HP and armor. They move fast. Never in any dungeons
                // and only outside in subtropical/rainforest climates. Not as active at night. They always charge.
                MinDamage = 1,
                MaxDamage = 7,
                MinDamage2 = 1,
                MaxDamage2 = 7,
                MinDamage3 = 1,
                MaxDamage3 = 7,
                MinHealth = 25,
                MaxHealth = 75,
                Level = 4,
                ArmorValue = 4,
                ParrySounds = false,
                MapChance = 0,
                Weight = 1000,
                SoulPts = 50000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, 2, -1, 3, 4, 5 },
                Team = MobileTeams.Tigers,
                MoveSpeed = 7.25f,
            },

            // Spider
            new MobileEnemy()
            {
                ID = 6,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Animal,
                MaleTexture = 261,
                FemaleTexture = 261,
                CorpseTexture = CorpseTexture(401, 4),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                MoveSound = (int)SoundClips.EnemySpiderMove,
                BarkSound = (int)SoundClips.EnemySpiderBark,
                AttackSound = (int)SoundClips.EnemySpiderAttack,
                MinMetalToHit = WeaponMaterialTypes.None,

                // [OSORKON] Much less damage, more HP. They move very fast. 10% chance/hit to poison, 0.1% chance/hit
                // to inflict Immunity-bypassing poison, doesn't Paralyze. Common in many dungeons. Outside Spiders
                // prefer tropical wilderness, day or night, and are rarely found in woodlands. They always charge.
                MinDamage = 1,
                MaxDamage = 8,
                MinHealth = 14,
                MaxHealth = 42,
                Level = 4,
                ArmorValue = 5,
                ParrySounds = false,
                MapChance = 0,
                Weight = 400,
                SoulPts = 50000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2, 3, 4, 5 },
                Team = MobileTeams.Spiders,
                MoveSpeed = 8f,
            },

            // Orc
            new MobileEnemy()
            {
                ID = 7,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Darkness,
                MaleTexture = 262,
                FemaleTexture = 262,
                CorpseTexture = CorpseTexture(96, 2),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyOrcMove,
                BarkSound = (int)SoundClips.EnemyOrcBark,
                AttackSound = (int)SoundClips.EnemyOrcAttack,
                MinMetalToHit = WeaponMaterialTypes.None,

                // [OSORKON] Much more damage and HP. They move somewhat slowly. Orcs don't wield weapons anymore - they
                // carried way too much loot in vanilla. They prefer tropical climates, day or night, and are only in
                // Orc Stronghold dungeon types. They never appear in towns at night.
                MinDamage = 1,
                MaxDamage = 12,
                MinHealth = 24,
                MaxHealth = 72,
                Level = 5,
                ArmorValue = 7,
                ParrySounds = true,
                MapChance = 0,
                Weight = 600,
                LootTableKey = "A",
                SoulPts = 51000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, 2, -1, 3, 4, -1, 5, 0 },
                ChanceForAttack2 = 50,
                PrimaryAttackAnimFrames2 = new int[] { 4, -1, 5, 0 },
                Team = MobileTeams.Orcs,
                MoveSpeed = 5f,
            },

            // Centaur
            new MobileEnemy()
            {
                ID = 8,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Daylight,
                MaleTexture = 263,
                FemaleTexture = 263,
                CorpseTexture = CorpseTexture(406, 0),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyCentaurMove,
                BarkSound = (int)SoundClips.EnemyCentaurBark,
                AttackSound = (int)SoundClips.EnemyCentaurAttack,
                MinMetalToHit = WeaponMaterialTypes.None,

                // [OSORKON] Less damage, more HP. They don't wield weapons - they carried too much loot in
                // vanilla. They move fast. They are never in dungeons or towns at night, only appearing in
                // daytime temperate and mountain wilderness.
                MinDamage = 1,
                MaxDamage = 15,
                MinHealth = 20,
                MaxHealth = 60,
                Level = 5,
                ArmorValue = 6,
                ParrySounds = true,
                MapChance = 1,
                Weight = 1200,
                LootTableKey = "C",
                SoulPts = 53000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, 1, 1, 2, -1, 3, 3, 4 },
                ChanceForAttack2 = 50,
                PrimaryAttackAnimFrames2 = new int[] { 0, 1, 1, 1, 2, -1, 3, 3, 2, 1, 1, -1, 2, 3, 3, 4 },
                Team = MobileTeams.Centaurs,
                MoveSpeed = 7f,
            },

            // Werewolf
            new MobileEnemy()
            {
                ID = 9,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Darkness,
                MaleTexture = 264,
                FemaleTexture = 264,
                CorpseTexture = CorpseTexture(96, 5),
                HasIdle = false,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyWerewolfMove,
                BarkSound = (int)SoundClips.EnemyWerewolfBark,
                AttackSound = (int)SoundClips.EnemyWerewolfAttack,
                MinMetalToHit = WeaponMaterialTypes.None,

                // [OSORKON] Less damage but way higher Level, HP, and Armor. Non-Silver materials deal half damage.
                // They move very fast and are never in dungeons. They rarely appear outside at night almost anywhere,
                // but never in Desert wilderness. Common in day/night Haunted Woodlands wilderness.
                MinDamage = 1,
                MaxDamage = 8,
                MinDamage2 = 1,
                MaxDamage2 = 8,
                MinDamage3 = 1,
                MaxDamage3 = 8,
                MinHealth = 33,
                MaxHealth = 99,
                Level = 12,
                ArmorValue = 0,
                MapChance = 0,
                ParrySounds = false,
                Weight = 480,
                SoulPts = 51000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2, -1, 2 },
                Team = MobileTeams.Werecreatures,
                MoveSpeed = 8f,
            },

            // Nymph
            new MobileEnemy()
            {
                ID = 10,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Daylight,
                MaleTexture = 265,
                FemaleTexture = 265,
                CorpseTexture = CorpseTexture(406, 2),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyNymphMove,
                BarkSound = (int)SoundClips.EnemyNymphBark,
                AttackSound = (int)SoundClips.EnemyNymphAttack,
                MinMetalToHit = WeaponMaterialTypes.None,

                // [OSORKON] More damage, less HP and much less Armor. They move slowly. They're only found
                // in Natural Cave dungeon types and are common outside in tropical daytime wilderness. They
                // never spawn outside at night.
                MinDamage = 1,
                MaxDamage = 10,
                MinHealth = 15,
                MaxHealth = 45,
                Level = 6,
                ArmorValue = 4,
                ParrySounds = false,
                MapChance = 1,
                Weight = 200,
                LootTableKey = "C",
                SoulPts = 60000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, 2, 3, 4, -1, 5 },
                Team = MobileTeams.Nymphs,
                MoveSpeed = 4.5f,
            },

            // Slaughterfish
            new MobileEnemy()
            {
                ID = 11,
                Behaviour = MobileBehaviour.Aquatic,
                Affinity = MobileAffinity.Water,
                MaleTexture = 266,
                FemaleTexture = 266,
                CorpseTexture = CorpseTexture(305, 1),
                HasIdle = false,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                MoveSound = (int)SoundClips.EnemyEelMove,
                BarkSound = (int)SoundClips.EnemyEelBark,
                AttackSound = (int)SoundClips.EnemyEelAttack,
                MinMetalToHit = WeaponMaterialTypes.None,

                // [OSORKON] Slightly less damage, slightly more HP and armor. They swim slowly. Only found
                // underwater. They always charge.
                MinDamage = 1,
                MaxDamage = 12,
                MinHealth = 20,
                MaxHealth = 60,
                Level = 7,
                ArmorValue = 5,
                ParrySounds = false,
                MapChance = 0,
                Weight = 400,
                SoulPts = 50000,
                PrimaryAttackAnimFrames = new int[] { 0, -1, 1, 2, 3, 4, -1, 5, 0 },
                ChanceForAttack2 = 33,
                PrimaryAttackAnimFrames2 = new int[] { 0, 3, -1, 5, 4, 3, 3, -1, 5, 4, 3, -1, 5, 0 },
                ChanceForAttack3 = 33,
                PrimaryAttackAnimFrames3 = new int[] { 0, 3, -1, 5, 0 },
                Team = MobileTeams.Aquatic,
                MoveSpeed = 4.5f,
            },

            // Orc Sergeant
            new MobileEnemy()
            {
                ID = 12,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Darkness,
                MaleTexture = 267,
                FemaleTexture = 267,
                CorpseTexture = CorpseTexture(96, 2),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyOrcSergeantMove,
                BarkSound = (int)SoundClips.EnemyOrcSergeantBark,
                AttackSound = (int)SoundClips.EnemyOrcSergeantAttack,
                MinMetalToHit = WeaponMaterialTypes.None,

                // [OSORKON] More damage, much higher HP, Level, and Armor. They move somewhat slowly. They are
                // only in Orc Stronghold dungeon types. Outside they prefer tropical climates, day or night. They
                // are rarer than standard Orcs. They never appear in towns at night.
                MinDamage = 1,
                MaxDamage = 25,
                MinHealth = 50,
                MaxHealth = 150,
                Level = 11,
                ArmorValue = 2,
                ParrySounds = true,
                MapChance = 1,
                Weight = 600,
                LootTableKey = "A",
                SoulPts = 51000,
                PrimaryAttackAnimFrames = new int[] { 0, -1, 1, 2, 3, 4, -1, 5, 0 },
                ChanceForAttack2 = 50,
                PrimaryAttackAnimFrames2 = new int[] { 5, 4, 3, -1, 2, 1, 0 },
                Team = MobileTeams.Orcs,
                MoveSpeed = 5.75f,
            },

            // Harpy
            new MobileEnemy()
            {
                ID = 13,
                Behaviour = MobileBehaviour.Flying,
                Affinity = MobileAffinity.Daylight,
                MaleTexture = 268,
                FemaleTexture = 268,
                CorpseTexture = CorpseTexture(406, 4),
                HasIdle = false,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyHarpyMove,
                BarkSound = (int)SoundClips.EnemyHarpyBark,
                AttackSound = (int)SoundClips.EnemyHarpyAttack,
                MinMetalToHit = WeaponMaterialTypes.None,

                // [OSORKON] Slightly more damage, similar HP, much less armor. They move fast. Common
                // in a few dungeon types and are only outside during the day in mountainous regions -
                // I assume mountains make good nesting grounds. They never spawn outside at night.
                MinDamage = 1,
                MaxDamage = 23,
                MinHealth = 25,
                MaxHealth = 75,
                Level = 8,
                ArmorValue = 5,
                ParrySounds = false,
                MapChance = 0,
                Weight = 200,
                LootTableKey = "D",
                SoulPts = 53000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, 2, -1, 3 },
                Team = MobileTeams.Harpies,
                MoveSpeed = 7.5f,
            },

            // Wereboar
            new MobileEnemy()
            {
                ID = 14,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Darkness,
                MaleTexture = 269,
                FemaleTexture = 269,
                CorpseTexture = CorpseTexture(96, 5),
                HasIdle = false,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyWereboarMove,
                BarkSound = (int)SoundClips.EnemyWereboarBark,
                AttackSound = (int)SoundClips.EnemyWereboarAttack,
                MinMetalToHit = WeaponMaterialTypes.None,

                // [OSORKON] More damage and Armor and way higher Level and HP. Non-Silver materials deal half damage.
                // They move fast and are never in dungeons. They rarely appear outside at night almost anywhere, but
                // never in Desert wilderness. Common in day/night Haunted Woodlands wilderness.
                MinDamage = 1,
                MaxDamage = 16,
                MinDamage2 = 1,
                MaxDamage2 = 16,
                MinDamage3 = 1,
                MaxDamage3 = 16,
                MinHealth = 44,
                MaxHealth = 132,
                Level = 12,
                ArmorValue = 2,
                MapChance = 0,
                ParrySounds = false,
                Weight = 560,
                SoulPts = 51000,
                PrimaryAttackAnimFrames = new int[] { 0, -1, 1, 2, 2 },
                Team = MobileTeams.Werecreatures,
                MoveSpeed = 7f,
            },

            // Skeletal Warrior
            new MobileEnemy()
            {
                ID = 15,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Undead,
                MaleTexture = 270,
                FemaleTexture = 270,
                CorpseTexture = CorpseTexture(306, 1),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                BloodIndex = 2,
                MoveSound = (int)SoundClips.EnemySkeletonMove,
                BarkSound = (int)SoundClips.EnemySkeletonBark,
                AttackSound = (int)SoundClips.EnemySkeletonAttack,
                MinMetalToHit = WeaponMaterialTypes.None,

                // [OSORKON] Lower Level, HP, and Armor. They move somewhat fast. Immune to Archery, resistant to
                // Long/Short Blades, takes double damage from Blunt Weapons. Ubiquitous at night in the wilderness
                // and common in many dungeons. Can appear during the day in Haunted Woodlands wilderness. They
                // never spawn in towns at night. 2% chance/hit of transmitting any disease. They always charge
                // and are occasionally found underwater.
                MinDamage = 1,
                MaxDamage = 19,
                MinHealth = 17,
                MaxHealth = 51,
                Level = 8,
                ArmorValue = 4,
                ParrySounds = true,
                MapChance = 1,
                Weight = 80,
                LootTableKey = "H",
                SoulPts = 50000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, 2, 3, -1, 4, 5 },
                Team = MobileTeams.Undead,
                MoveSpeed = 6f,
            },

            // Giant
            new MobileEnemy()
            {
                ID = 16,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Daylight,
                MaleTexture = 271,
                FemaleTexture = 271,
                CorpseTexture = CorpseTexture(406, 1),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyGiantMove,
                BarkSound = (int)SoundClips.EnemyGiantBark,
                AttackSound = (int)SoundClips.EnemyGiantAttack,
                MinMetalToHit = WeaponMaterialTypes.None,

                // [OSORKON] Less damage and Armor, higher Level and way more HP. They move somewhat fast. Only in
                // Giant Stronghold dungeon types. Common outside in woodland and mountain daytime wilderness. They
                // never spawn outside at night. They always charge.
                MinDamage = 1,
                MaxDamage = 30,
                MinHealth = 70,
                MaxHealth = 210,
                Level = 12,
                ArmorValue = 4,
                ParrySounds = false,
                MapChance = 1,
                LootTableKey = "F",
                Weight = 3000,
                SoulPts = 53000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2, 3, 4, -1, 5 },
                Team = MobileTeams.Giants,
                MoveSpeed = 6.5f,
            },

            // Zombie
            new MobileEnemy()
            {
                ID = 17,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Undead,
                MaleTexture = 272,
                FemaleTexture = 272,
                CorpseTexture = CorpseTexture(306, 4),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyZombieMove,
                BarkSound = (int)SoundClips.EnemyZombieBark,
                AttackSound = (int)SoundClips.EnemyZombieAttack,
                MinMetalToHit = WeaponMaterialTypes.None,

                // [OSORKON] Much lower Level, damage, and Armor, more HP. They move extremely slowly. Resistant to Short
                // Blades, Hand-to-Hand, and Archery, takes double damage from Axes. 5% chance/hit of transmitting any disease.
                // Common in many dungeons and outside in wilderness at night. They never spawn in towns at night. Can
                // appear during the day in Haunted Woodlands wilderness. They always charge and are rarely found underwater.
                MinDamage = 1,
                MaxDamage = 15,
                MinHealth = 33,
                MaxHealth = 99,
                Level = 7,
                ArmorValue = 5,
                ParrySounds = false,
                MapChance = 1,
                Weight = 4000,
                LootTableKey = "G",
                SoulPts = 50000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, 2, -1, 3, 4 },
                ChanceForAttack2 = 50,
                PrimaryAttackAnimFrames2 = new int[] { 0, 2, -1, 3, 4 },
                Team = MobileTeams.Undead,
                MoveSpeed = 3f,
            },

            // Ghost
            new MobileEnemy()
            {
                ID = 18,
                Behaviour = MobileBehaviour.Spectral,
                Affinity = MobileAffinity.Undead,
                MaleTexture = 273,
                FemaleTexture = 273,
                CorpseTexture = CorpseTexture(306, 0),
                HasIdle = false,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                BloodIndex = 2,
                MoveSound = (int)SoundClips.EnemyGhostMove,
                BarkSound = (int)SoundClips.EnemyGhostBark,
                AttackSound = (int)SoundClips.EnemyGhostAttack,
                MinMetalToHit = WeaponMaterialTypes.None,

                // [OSORKON] Less damage, much less HP, much more Armor. Casts no spells. Can only be damaged by
                // Silver. Moves extremely slowly. Extremely fragile but hard to hit. Holy Water or Holy Daggers
                // recommended if player isn't a spellcaster. Common in many dungeons and outside in wilderness
                // at night. They can spawn in Haunted Woodland towns at night, and very common in Haunted
                // Woodlands wilderness, day or night. They always charge and are very rarely found underwater.
                MinDamage = 1,
                MaxDamage = 30,
                MinHealth = 5,
                MaxHealth = 15,
                Level = 11,
                ArmorValue = -4,
                ParrySounds = false,
                MapChance = 1,
                Weight = 0,
                LootTableKey = "I",
                NoShadow = true,
                SoulPts = 80000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2, 3 },
                SpellAnimFrames = new int[] { 0, 0, 0, 0, 0, 0 },
                Team = MobileTeams.Undead,
                MoveSpeed = 3.5f,
            },

            // Mummy
            new MobileEnemy()
            {
                ID = 19,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Undead,
                MaleTexture = 274,
                FemaleTexture = 274,
                CorpseTexture = CorpseTexture(306, 5),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                BloodIndex = 2,
                MoveSound = (int)SoundClips.EnemyMummyMove,
                BarkSound = (int)SoundClips.EnemyMummyBark,
                AttackSound = (int)SoundClips.EnemyMummyAttack,
                MinMetalToHit = WeaponMaterialTypes.None,

                // [OSORKON] More damage, lower Level, much more HP. Moves extremely slowly. Resistant to Archery,
                // takes double damage from Axes and Long Blades. Common in a few undead-themed dungeon types, rare
                // outside in wilderness at night. Very common at night in Desert wilderness - the environment suits
                // them. They never spawn in towns at night. Can appear during the day in Haunted Woodlands
                // wilderness. 2% chance/hit of transmitting disease, and they always charge.
                MinDamage = 1,
                MaxDamage = 25,
                MinHealth = 45,
                MaxHealth = 135,
                Level = 10,
                ArmorValue = 2,
                ParrySounds = false,
                MapChance = 1,
                Weight = 300,
                LootTableKey = "E",
                SoulPts = 60000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, 2, -1, 3, 4 },
                Team = MobileTeams.Undead,
                MoveSpeed = 3.5f,
            },

            // Giant Scorpion
            new MobileEnemy()
            {
                ID = 20,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Animal,
                MaleTexture = 275,
                FemaleTexture = 275,
                CorpseTexture = CorpseTexture(401, 5),
                HasIdle = false,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                MoveSound = (int)SoundClips.EnemyScorpionMove,
                BarkSound = (int)SoundClips.EnemyScorpionBark,
                AttackSound = (int)SoundClips.EnemyScorpionAttack,
                MinMetalToHit = WeaponMaterialTypes.None,

                // [OSORKON] Less damage and Armor, more HP. Moves fast. Resistant to Hand-to-Hand and Archery,
                // takes double damage from Axes. 10% chance/hit to poison, 0.1% chance/hit to inflict
                // Immunity-bypassing poison, doesn't Paralyze. Very common in Scorpion Nest dungeon types and
                // outside in daytime Desert wilderness. They never spawn outside at night. They always charge.
                MinDamage = 1,
                MaxDamage = 30,
                MinHealth = 33,
                MaxHealth = 99,
                Level = 12,
                ParrySounds = false,
                ArmorValue = 1,
                MapChance = 0,
                Weight = 600,
                SoulPts = 50000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 3, 2, 1, 0 },
                Team = MobileTeams.Scorpions,
                MoveSpeed = 7f,
            },

            // Orc Shaman
            new MobileEnemy()
            {
                ID = 21,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Darkness,
                MaleTexture = 276,
                FemaleTexture = 276,
                CorpseTexture = CorpseTexture(96, 2),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyOrcShamanMove,
                BarkSound = (int)SoundClips.EnemyOrcShamanBark,
                AttackSound = (int)SoundClips.EnemyOrcShamanAttack,
                MinMetalToHit = WeaponMaterialTypes.None,

                // [OSORKON] Much higher damage, HP, Level, and Armor. They move somewhat slowly. Greatly increased
                // spell variety, increased chance to carry good loot. Only in Orc Stronghold dungeon type. Outside
                // they prefer tropical climates, day or night. They never appear in towns at night. Shamans are very
                // rare compared to standard Orcs.
                MinDamage = 1,
                MaxDamage = 35,
                MinHealth = 55,
                MaxHealth = 165,
                Level = 16,
                ArmorValue = -2,
                ParrySounds = true,
                MapChance = 3,
                Weight = 400,
                LootTableKey = "U",
                SoulPts = 53000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 3, 2, 1, 0 },
                ChanceForAttack2 = 20,
                PrimaryAttackAnimFrames2 = new int[] { 0, -1, 4, 5, 0 },
                ChanceForAttack3 = 20,
                PrimaryAttackAnimFrames3 = new int[] { 0, 1, -1, 3, 2, 1, 0, -1, 4, 5, 0 },
                ChanceForAttack4 = 20,
                PrimaryAttackAnimFrames4 = new int[] { 0, 1, -1, 3, 2, -1, 3, 2, 1, 0 }, // Not used in classic. Fight stance used instead.
                ChanceForAttack5 = 20,
                PrimaryAttackAnimFrames5 = new int[] { 0, -1, 4, 5, -1, 4, 5, 0 }, // Not used in classic. Spell animation played instead.
                HasSpellAnimation = true,
                SpellAnimFrames = new int[] { 0, 0, 1, 2, 3, 3, 3 },
                Team = MobileTeams.Orcs,
                MoveSpeed = 5.25f,
            },

            // Gargoyle
            new MobileEnemy()
            {
                ID = 22,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Darkness,
                MaleTexture = 277,
                FemaleTexture = 277,
                CorpseTexture = CorpseTexture(96, 1),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyGargoyleMove,
                BarkSound = (int)SoundClips.EnemyGargoyleBark,
                AttackSound = (int)SoundClips.EnemyGargoyleAttack,
                MinMetalToHit = WeaponMaterialTypes.None,

                // [OSORKON] Much more damage and HP, slightly better Armor. They move extremely slowly. Immune to
                // Hand-to-Hand and Archery, resistant to Long/Short Blades. Common in Mines, Laboratories, and
                // Giant Strongholds (they speak Giantish). They rarely appear outside at night and rarely spawn
                // in daytime Desert wilderness. They always charge.
                MinDamage = 1,
                MaxDamage = 50,
                MinHealth = 50,
                MaxHealth = 150,
                Level = 14,
                ArmorValue = -1,
                MapChance = 0,
                ParrySounds = false,
                Weight = 300,
                SoulPts = 53000,
                PrimaryAttackAnimFrames = new int[] { 0, 2, 1, 2, 3, -1, 4, 0 },
                Team = MobileTeams.Magic,
                MoveSpeed = 3.5f,
            },

            // Wraith
            new MobileEnemy()
            {
                ID = 23,
                Behaviour = MobileBehaviour.Spectral,
                Affinity = MobileAffinity.Undead,
                MaleTexture = 278,
                FemaleTexture = 278,
                CorpseTexture = CorpseTexture(306, 0),
                HasIdle = false,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                BloodIndex = 2,
                MoveSound = (int)SoundClips.EnemyWraithMove,
                BarkSound = (int)SoundClips.EnemyWraithBark,
                AttackSound = (int)SoundClips.EnemyWraithAttack,
                MinMetalToHit = WeaponMaterialTypes.None,

                // [OSORKON] Much less damage and HP, much more Armor. Immune to all materials except Silver. They move
                // slowly. Greatly increased spell variety. Very fragile but incredibly hard to hit. Holy Water or Holy
                // Daggers recommended if player isn't a spellcaster. Common in Vampire Haunts, somewhat common in a few
                // other dungeon types. Rarely spawns in outside wilderness at night. Can spawn in Haunted Woodland towns
                // at night. Very common in Haunted Woodland wilderness, day or night. They always charge, and are very
                // rarely found underwater.
                MinDamage = 1,
                MaxDamage = 45,
                MinHealth = 10,
                MaxHealth = 30,
                Level = 15,
                ArmorValue = -8,
                ParrySounds = false,
                MapChance = 1,
                Weight = 0,
                LootTableKey = "I",
                NoShadow = true,
                SoulPts = 80000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, 2, -1, 3 },
                SpellAnimFrames = new int[] { 0, 0, 0, 0, 0 },
                Team = MobileTeams.Undead,
                MoveSpeed = 4f,
            },

            // Orc Warlord
            new MobileEnemy()
            {
                ID = 24,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Darkness,
                MaleTexture = 279,
                FemaleTexture = 279,
                CorpseTexture = CorpseTexture(96, 2),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyOrcWarlordMove,
                BarkSound = (int)SoundClips.EnemyOrcWarlordBark,
                AttackSound = (int)SoundClips.EnemyOrcWarlordAttack,
                MinMetalToHit = WeaponMaterialTypes.None,

                // [OSORKON] The fourth most difficult boss of the game, tougher than Alternate Dragonlings but not as
                // tough as a Daedra Lord. Damage, HP, Level, and Armor greatly increased. Moves somewhat fast. Has much
                // greater chance of dropping good loot. They are the only boss who don't See Invisible. They are very rare.
                MinDamage = 42,
                MaxDamage = 73,
                MinHealth = 150,
                MaxHealth = 450,
                Level = 25,
                ArmorValue = -10,
                ParrySounds = true,
                MapChance = 2,
                Weight = 700,
                LootTableKey = "T",

                // [OSORKON] All boss Soul Gems are extremely valuable.
                SoulPts = 1500000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2, 3, 4, -1, 5 },
                ChanceForAttack2 = 33,
                PrimaryAttackAnimFrames2 = new int[] { 4, -1, 5, 0 },
                ChanceForAttack3 = 33,
                PrimaryAttackAnimFrames3 = new int[] { 0, 1, -1, 2, 3, 4 -1, 5, 0, 4, -1, 5, 0 },
                Team = MobileTeams.Orcs,
                MoveSpeed = 6f,
            },

            // Frost Daedra
            new MobileEnemy()
            {
                ID = 25,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Daedra,
                MaleTexture = 280,
                FemaleTexture = 280,
                CorpseTexture = CorpseTexture(400, 3),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyFrostDaedraMove,
                BarkSound = (int)SoundClips.EnemyFrostDaedraBark,
                AttackSound = (int)SoundClips.EnemyFrostDaedraAttack,
                MinMetalToHit = WeaponMaterialTypes.None,

                // [OSORKON] Much less damage and Armor, less HP. Moves slowly. Frostbite added to spell kit.
                // Common in Covens and outside in Mountain wilderness at night. Will very rarely spawn in Mountain
                // Woods wilderness at night. Never spawns in towns at night. Greater chance to drop good loot. 
                MinDamage = 1,
                MaxDamage = 100,
                MinHealth = 35,
                MaxHealth = 105,
                Level = 17,
                ArmorValue = 0,
                ParrySounds = true,
                MapChance = 0,
                Weight = 800,
                LootTableKey = "J",
                NoShadow = true,
                GlowColor = new Color(18, 68, 88) * 0.1f,
                SoulPts = 100000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2, 3, -1, 4, 5, 0 },
                ChanceForAttack2 = 50,
                PrimaryAttackAnimFrames2 = new int[] { -1, 4, 5, 0 },
                SpellAnimFrames = new int[] { 1, 1, 3, 3 },
                Team = MobileTeams.Daedra,
                MoveSpeed = 4f,
            },

            // Fire Daedra
            new MobileEnemy()
            {
                ID = 26,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Daedra,
                MaleTexture = 281,
                FemaleTexture = 281,
                CorpseTexture = CorpseTexture(400, 2),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyFireDaedraMove,
                BarkSound = (int)SoundClips.EnemyFireDaedraBark,
                AttackSound = (int)SoundClips.EnemyFireDaedraAttack,
                MinMetalToHit = WeaponMaterialTypes.None,

                // [OSORKON] Less damage, much more HP and Armor. Moves fast. God's Fire added to spell kit. Common
                // in a handful of dungeon types and outside in Desert daytime wilderness. Never spawns outside at
                // night. I don't recommend using Hand-to-Hand on them - doing so will damage you 4 HP every landed
                // attack. They have a greater chance to drop good loot.
                MinDamage = 1,
                MaxDamage = 50,
                MinHealth = 60,
                MaxHealth = 180,
                Level = 17,
                ArmorValue = -3,
                ParrySounds = true,
                MapChance = 0,
                Weight = 800,
                LootTableKey = "J",
                NoShadow = true,
                GlowColor = new Color(243, 239, 44) * 0.05f,
                SoulPts = 100000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2, 3, -1, 4 },
                ChanceForAttack2 = 50,
                PrimaryAttackAnimFrames2 = new int[] { 3, -1, 4 },
                SpellAnimFrames = new int[] { 1, 1, 3, 3 },
                Team = MobileTeams.Daedra,
                MoveSpeed = 7f,
            },

            // Daedroth
            new MobileEnemy()
            {
                ID = 27,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Daedra,
                MaleTexture = 282,
                FemaleTexture = 282,
                CorpseTexture = CorpseTexture(400, 1),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyLesserDaedraMove,
                BarkSound = (int)SoundClips.EnemyLesserDaedraBark,
                AttackSound = (int)SoundClips.EnemyLesserDaedraAttack,
                MinMetalToHit = WeaponMaterialTypes.None,

                // [OSORKON] Less damage, much more HP and Armor. Moves somewhat fast. Greatly increased spell
                // variety. Somewhat common in a handful of dungeon types, can rarely appear outside in tropical
                // wilderness, day or night. They look like crocodiles, so I would think they prefer hot climates.
                // Can rarely appear in Rainforest towns at night. Greater chance to drop good loot.
                MinDamage = 1,
                MaxDamage = 50,
                MinHealth = 66,
                MaxHealth = 198,
                Level = 18,
                ArmorValue = -4,
                ParrySounds = true,
                MapChance = 0,
                Weight = 400,
                LootTableKey = "E",
                SoulPts = 60000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2, 3, 4, -1, 5 },
                ChanceForAttack2 = 33,
                PrimaryAttackAnimFrames2 = new int[] { 4, -1, 5, 0 },
                ChanceForAttack3 = 33,
                PrimaryAttackAnimFrames3 = new int[] { 0, 1, -1, 2, 3, 4, -1, 5, 0, 4, -1, 5, 0 },
                SpellAnimFrames = new int[] { 1, 1, 3, 3 },
                Team = MobileTeams.Daedra,
                MoveSpeed = 6f,
            },

            // Vampire
            new MobileEnemy()
            {
                ID = 28,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Darkness,
                MaleTexture = 283,
                FemaleTexture = 283,
                CorpseTexture = CorpseTexture(96, 3),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyFemaleVampireMove,
                BarkSound = (int)SoundClips.EnemyFemaleVampireBark,
                AttackSound = (int)SoundClips.EnemyFemaleVampireAttack,
                MinMetalToHit = WeaponMaterialTypes.None,

                // [OSORKON] The easiest boss of the game. Greatly increased Level, damage, HP, and Armor. They move extremely
                // fast and take double damage from Silver. Spell variety greatly increased, they have Magicka for 30 spells.
                // 1.4% chance/hit of transmitting any disease, 0.6% chance/hit of transmitting stage one Vampirism. High
                // chance to drop good loot. Very common in Vampire Haunt dungeon types, very rare otherwise.
                MinDamage = 32,
                MaxDamage = 62,
                MinHealth = 80,
                MaxHealth = 240,
                Level = 23,
                ArmorValue = -6,
                ParrySounds = false,
                MapChance = 3,
                Weight = 400,
                SeesThroughInvisibility = true,
                LootTableKey = "Q",
                NoShadow = true,

                // [OSORKON] All boss Soul Gems are very valuable.
                SoulPts = 750000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, 2, 3, -1, 4, 5 },
                SpellAnimFrames = new int[] { 1, 1, 5, 5 },
                Team = MobileTeams.Undead,
                MoveSpeed = 9f,
            },

            // Daedra Seducer
            new MobileEnemy()
            {
                ID = 29,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Daedra,
                MaleTexture = 284,
                FemaleTexture = 284,
                CorpseTexture = CorpseTexture(400, 6),          // Has a winged and unwinged corpse, only using unwinged here
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                HasSeducerTransform1 = true,
                HasSeducerTransform2 = true,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemySeducerMove,
                BarkSound = (int)SoundClips.EnemySeducerBark,
                AttackSound = (int)SoundClips.EnemySeducerAttack,
                MinMetalToHit = WeaponMaterialTypes.None,

                // [OSORKON] The toughest non-boss enemy of the game. Much more damage, HP, and Armor. They move fast.
                // Spell variety greatly increased. They are relatively common in a handful of the toughest dungeon types.
                // Also, they will very rarely spawn outside in daytime wilderness wherever the player normally finds Nymphs.
                // My thought was every now and then a Daedra Seducer would pretend to be a Nymph and try to trick foolish
                // mortals. They have a higher chance of dropping good loot.
                MinDamage = 1,
                MaxDamage = 90,
                MinHealth = 75,
                MaxHealth = 225,
                Level = 19,
                ArmorValue = -5,
                ParrySounds = false,
                MapChance = 1,
                Weight = 200,
                SeesThroughInvisibility = true,
                LootTableKey = "Q",
                SoulPts = 200000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2 },
                SpellAnimFrames = new int[] { 0, 1, 2 },
                SeducerTransform1Frames = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 },
                SeducerTransform2Frames = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 },
                Team = MobileTeams.Daedra,
                MoveSpeed = 7.5f,
            },

            // Vampire Ancient
            new MobileEnemy()
            {
                ID = 30,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Darkness,
                MaleTexture = 285,
                FemaleTexture = 285,
                CorpseTexture = CorpseTexture(96, 3),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyVampireMove,
                BarkSound = (int)SoundClips.EnemyVampireBark,
                AttackSound = (int)SoundClips.EnemyVampireAttack,
                MinMetalToHit = WeaponMaterialTypes.None,

                // [OSORKON] The second toughest boss, tougher than a Daedra Lord but not as tough as an Ancient Lich. All
                // stats heavily buffed. They are incredibly fast and will outrun player with 100 Running and SPD. They take
                // double damage from Silver. Casts no spells. 1.4% chance/hit of transmitting any disease, 0.6% chance/hit
                // of transmitting stage one Vampirism. Incredibly high chance to drop good loot. They are very rare.
                MinDamage = 82,
                MaxDamage = 112,
                MinHealth = 180,
                MaxHealth = 540,
                Level = 28,
                ArmorValue = -12,
                ParrySounds = false,
                MapChance = 3,
                Weight = 400,
                SeesThroughInvisibility = true,
                LootTableKey = "Q",
                NoShadow = true,

                // [OSORKON] All boss Soul Gems are extremely valuable.
                SoulPts = 1500000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, 2, 3, -1, 4, 5 },
                SpellAnimFrames = new int[] { 1, 1, 5, 5 },
                Team = MobileTeams.Undead,
                MoveSpeed = 12f,
            },

            // Daedra Lord
            new MobileEnemy()
            {
                ID = 31,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Daedra,
                MaleTexture = 286,
                FemaleTexture = 286,
                CorpseTexture = CorpseTexture(400, 4),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyDaedraLordMove,
                BarkSound = (int)SoundClips.EnemyDaedraLordBark,
                AttackSound = (int)SoundClips.EnemyDaedraLordAttack,
                MinMetalToHit = WeaponMaterialTypes.None,

                // [OSORKON] The third toughest boss, tougher than Orc Warlords but not as tough as an Ancient Vampire.
                // All stats buffed. They move fast. Spell variety greatly increased, they have infinite Magicka.
                // Incredibly high chance to drop good loot. They are very rare.
                MinDamage = 30,
                MaxDamage = 60,
                MinHealth = 170,
                MaxHealth = 510,
                Level = 28,
                ArmorValue = -11,
                ParrySounds = true,
                MapChance = 0,
                Weight = 1000,
                SeesThroughInvisibility = true,
                LootTableKey = "S",

                // [OSORKON] All boss Soul Gems are extremely valuable.
                SoulPts = 1500000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2, 3, -1, 4 },
                ChanceForAttack2 = 33,
                PrimaryAttackAnimFrames2 = new int[] { 3, -1, 4, 0, -1, 4, 3, -1, 4, 0, -1, 4, 3 },
                ChanceForAttack3 = 33,
                PrimaryAttackAnimFrames3 = new int[] { 0, 1, -1, 2, 1, 0, 1, -1, 2, 1, 0 },
                SpellAnimFrames = new int[] { 1, 1, 3, 3 },
                Team = MobileTeams.Daedra,
                MoveSpeed = 7f,
            },

            // Lich
            new MobileEnemy()
            {
                ID = 32,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Undead,
                MaleTexture = 287,
                FemaleTexture = 287,
                CorpseTexture = CorpseTexture(306, 2),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                BloodIndex = 2,
                MoveSound = (int)SoundClips.EnemyLichMove,
                BarkSound = (int)SoundClips.EnemyLichBark,
                AttackSound = (int)SoundClips.EnemyLichAttack,
                MinMetalToHit = WeaponMaterialTypes.None,

                // [OSORKON] The second easiest boss, tougher than a Vampire but not as tough as an Assassin. Slightly
                // less damage than vanilla, higher Level, much more HP, much less Armor. They take double damage from
                // Silver. Spell variety greatly increased, has Magicka for 30 spells. Moves slowly. High chance of
                // dropping good loot. They are very rare.
                MinDamage = 60,
                MaxDamage = 90,
                MinHealth = 80,
                MaxHealth = 240,
                Level = 23,
                ArmorValue = -7,
                ParrySounds = false,
                MapChance = 4,
                Weight = 300,
                SeesThroughInvisibility = true,
                LootTableKey = "S",

                // [OSORKON] All boss Soul Gems are very valuable.
                SoulPts = 750000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, 1, 2, -1, 3, 4, 4 },
                SpellAnimFrames = new int[] { 0, 1, 2, 3, 4 },
                Team = MobileTeams.Undead,
                MoveSpeed = 4f,
            },

            // Ancient Lich
            new MobileEnemy()
            {
                ID = 33,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Undead,
                MaleTexture = 288,
                FemaleTexture = 288,
                CorpseTexture = CorpseTexture(306, 3),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                BloodIndex = 2,
                MoveSound = (int)SoundClips.EnemyLichKingMove,
                BarkSound = (int)SoundClips.EnemyLichKingBark,
                AttackSound = (int)SoundClips.EnemyLichKingAttack,
                MinMetalToHit = WeaponMaterialTypes.None,

                // [OSORKON] The toughest boss in the game. They have the highest HP, Armor, and damage, but they take double
                // damage from Silver (you probably won't be able to hit them with anything Silver - you generally need Daedric's
                // to-hit buffs to land any attacks). Dealing devastating damage with melee attacks and massive spell variety,
                // they are any adventurer's worst nightmare. To reliably defeat them, you need a Daedric high-damage weapon,
                // 300+ HP, plenty of Healing, and a very high weapon skill. Chipping away at them over time won't work, as they
                // regularly cast Heal on themselves and have infinite Magicka. To top it off, they will likely reflect or resist
                // any spell you throw at them. Their greatest weakness is their speed - they'll never chase you down as they move
                // slowly. They almost always drop excellent loot. They are very rare and can be found underwater.
                MinDamage = 115,
                MaxDamage = 145,
                MinHealth = 200,
                MaxHealth = 600,
                Level = 28,
                ArmorValue = -13,
                ParrySounds = false,
                MapChance = 4,
                Weight = 300,

                // [OSORKON] Ancient Liches now see Invisible.
                SeesThroughInvisibility = true,
                LootTableKey = "S",

                // [OSORKON] All boss Soul Gems are extremely valuable.
                SoulPts = 1500000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, 1, 2, -1, 3, 4, 4 },
                SpellAnimFrames = new int[] { 0, 1, 2, 3, 4 },
                Team = MobileTeams.Undead,
                MoveSpeed = 4.5f,
            },

            // Dragonling
            new MobileEnemy()
            {
                ID = 34,
                Behaviour = MobileBehaviour.Flying,
                Affinity = MobileAffinity.Daylight,
                MaleTexture = 289,
                FemaleTexture = 289,
                CorpseTexture = CorpseTexture(96, 0),
                HasIdle = false,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                MoveSound = (int)SoundClips.EnemyFaeryDragonMove,
                BarkSound = (int)SoundClips.EnemyFaeryDragonBark,
                AttackSound = (int)SoundClips.EnemyFaeryDragonAttack,
                MinMetalToHit = WeaponMaterialTypes.None,

                // [OSORKON] Much more damage, HP, and Armor, but much lower Level. Moves fast. Common in a handful
                // of dungeon types, they only spawn outside in Desert wilderness during the day. They never spawn
                // outside at night. 
                MinDamage = 1,
                MaxDamage = 30,
                MinHealth = 40,
                MaxHealth = 120,
                Level = 10,
                ArmorValue = 3,
                ParrySounds = false,
                MapChance = 0,
                Weight = 10000,
                SoulPts = 50000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, 2, -1, 3 },
                Team = MobileTeams.Dragonlings,
                MoveSpeed = 7.5f,
            },

            // Fire Atronach
            new MobileEnemy()
            {
                ID = 35,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Golem,
                MaleTexture = 290,
                FemaleTexture = 290,
                CorpseTexture = CorpseTexture(405, 2),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyFireAtronachMove,
                BarkSound = (int)SoundClips.EnemyFireAtronachBark,
                AttackSound = (int)SoundClips.EnemyFireAtronachAttack,
                MinMetalToHit = WeaponMaterialTypes.None,

                // [OSORKON] Less damage, similar HP, much more Armor than vanilla. They move fast.  Punching or kicking
                // a Fire Atronach inflicts 2 HP damage on player every hit. They are common in Laboratory and Volcanic
                // Cave dungeon types, and are very common in Desert daytime wilderness. They never spawn outside at night.
                MinDamage = 1,
                MaxDamage = 15,
                MinHealth = 40,
                MaxHealth = 120,
                Level = 16,
                ArmorValue = 3,
                ParrySounds = false,
                MapChance = 0,
                NoShadow = true,
                GlowColor = new Color(243, 150, 44) * 0.05f,
                Weight = 1000,
                SoulPts = 80000,
                PrimaryAttackAnimFrames = new int[] { 0, -1, 1, 2, 3, 4 },
                Team = MobileTeams.Magic,
                MoveSpeed = 7f,
            },

            // Iron Atronach
            new MobileEnemy()
            {
                ID = 36,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Golem,
                MaleTexture = 291,
                FemaleTexture = 291,
                CorpseTexture = CorpseTexture(405, 1),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyIronAtronachMove,
                BarkSound = (int)SoundClips.EnemyIronAtronachBark,
                AttackSound = (int)SoundClips.EnemyIronAtronachAttack,
                MinMetalToHit = WeaponMaterialTypes.None,

                // [OSORKON] More damage, much more HP and Armor. They move extremely slowly. Immune to Hand-to-Hand
                // and Archery, resistant to Long/Short Blades and Axes. They are common in Laboratory and Mine
                // dungeon types but they never spawn outside.
                MinDamage = 1,
                MaxDamage = 25,
                MinHealth = 66,
                MaxHealth = 198,
                Level = 16,
                ArmorValue = 2,
                ParrySounds = true,
                MapChance = 0,
                Weight = 1000,
                SoulPts = 80000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, 2, -1, 3, 4 },
                Team = MobileTeams.Magic,
                MoveSpeed = 3f,
            },

            // Flesh Atronach
            new MobileEnemy()
            {
                ID = 37,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Golem,
                MaleTexture = 292,
                FemaleTexture = 292,
                CorpseTexture = CorpseTexture(405, 0),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyFleshAtronachMove,
                BarkSound = (int)SoundClips.EnemyFleshAtronachBark,
                AttackSound = (int)SoundClips.EnemyFleshAtronachAttack,
                MinMetalToHit = WeaponMaterialTypes.None,

                // [OSORKON] Much less damage, more Armor, much more HP. They move extremely slowly. Resistant to Short
                // Blades, Hand-to-Hand, and Archery, takes double damage from Axes. Like all Atronachs, they are common
                // in Laboratory dungeon types. They also appear in Haunted Woodland towns at night and Haunted Woodland
                // wilderness, day or night. 
                MinDamage = 1,
                MaxDamage = 10,
                MinHealth = 60,
                MaxHealth = 180,
                Level = 16,
                ArmorValue = 4,
                ParrySounds = false,
                MapChance = 0,
                Weight = 1000,
                SoulPts = 60000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, 2, -1, 3, 4 },
                Team = MobileTeams.Magic,
                MoveSpeed = 3f,
            },

            // Ice Atronach
            new MobileEnemy()
            {
                ID = 38,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Golem,
                MaleTexture = 293,
                FemaleTexture = 293,
                CorpseTexture = CorpseTexture(405, 3),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyIceAtronachMove,
                BarkSound = (int)SoundClips.EnemyIceAtronachBark,
                AttackSound = (int)SoundClips.EnemyIceAtronachAttack,
                MinMetalToHit = WeaponMaterialTypes.None,

                // [OSORKON] Much more HP and Armor. They move extremely slowly. Immune to Hand-to-Hand and Archery,
                // resistant to Long/Short Blades. Ice Atronachs are common in Laboratory dungeon types and outside in
                // Mountain nighttime wilderness. They rarely appear in Mountain Woods nighttime wilderness and underwater.
                MinDamage = 1,
                MaxDamage = 20,
                MinHealth = 50,
                MaxHealth = 150,
                Level = 16,
                ArmorValue = 3,
                ParrySounds = true,
                MapChance = 0,
                Weight = 1000,
                SoulPts = 80000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, 2, -1, 3, 4 },
                ChanceForAttack2 = 50,
                PrimaryAttackAnimFrames2 = new int[] { 0, -1, 3, 4 },
                Team = MobileTeams.Magic,
                MoveSpeed = 3f,
            },

            // Weights in classic (From offset 0x1BD8D9 in FALL.EXE) only have entries
            // up through Horse. Dragonling, Dreugh and Lamia use nonsense values from
            // the adjacent data. For Daggerfall Unity, using values inferred from
            // other enemy types.

            // Horse (unused, but can appear in merchant-sold soul traps)
            new MobileEnemy()
            {
                ID = 39,
                SoulPts = 50000,
            },

            // Dragonling
            new MobileEnemy()
            {
                ID = 40,
                Behaviour = MobileBehaviour.Flying,
                Affinity = MobileAffinity.Daylight,
                MaleTexture = 295,
                FemaleTexture = 295,
                CorpseTexture = CorpseTexture(96, 0),
                HasIdle = false,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                MoveSound = (int)SoundClips.EnemyFaeryDragonMove,
                BarkSound = (int)SoundClips.EnemyFaeryDragonBark,
                AttackSound = (int)SoundClips.EnemyFaeryDragonAttack,
                MinMetalToHit = WeaponMaterialTypes.None,

                // [OSORKON] The fifth toughest boss, tougher than an Assassin but not as tough as an Orc Warlord.
                // All stats greatly buffed. They move extremely fast. They are very rare and usually drop good loot.
                MinDamage = 70,
                MaxDamage = 100,
                MinHealth = 130,
                MaxHealth = 390,
                Level = 25,
                ArmorValue = -9,
                ParrySounds = false,
                MapChance = 0,
                Weight = 10000, // Using same value as other dragonling

                // [OSORKON] All bosses see Invisible.
                SeesThroughInvisibility = true,

                // [OSORKON] All boss Soul Gems are extremely valuable.
                SoulPts = 1500000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2, 3 },
                Team = MobileTeams.Dragonlings,
                MoveSpeed = 9f,
            },

            // Dreugh
            new MobileEnemy()
            {
                ID = 41,
                Behaviour = MobileBehaviour.Aquatic,
                Affinity = MobileAffinity.Water,
                MaleTexture = 296,
                FemaleTexture = 296,
                CorpseTexture = CorpseTexture(305, 0),
                HasIdle = false,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyDreughMove,
                BarkSound = (int)SoundClips.EnemyDreughBark,
                AttackSound = (int)SoundClips.EnemyDreughAttack,
                MinMetalToHit = WeaponMaterialTypes.None,

                // [OSORKON] Slightly better Armor, much lower damage and Level, much more HP. They swim extremely
                // slowly and are very common, but only appear underwater.
                MinDamage = 1,
                MaxDamage = 10,
                MinHealth = 22,
                MaxHealth = 66,
                Level = 8,
                ArmorValue = 5,
                ParrySounds = false,
                MapChance = 0,
                Weight = 600, // Using same value as orc
                LootTableKey = "R",
                SoulPts = 60000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, 2, 3, -1, 4, 5, -1, 6, 7 },
                ChanceForAttack2 = 33,
                PrimaryAttackAnimFrames2 = new int[] { 0, 1, 2, 3, -1, 4 },
                ChanceForAttack3 = 33,
                PrimaryAttackAnimFrames3 = new int[] { 0, 5, -1, 6, 7 },
                Team = MobileTeams.Aquatic,
                MoveSpeed = 3.5f,
            },

            // Lamia
            new MobileEnemy()
            {
                ID = 42,
                Behaviour = MobileBehaviour.Aquatic,
                Affinity = MobileAffinity.Water,
                MaleTexture = 297,
                FemaleTexture = 297,
                CorpseTexture = CorpseTexture(305, 2),
                HasIdle = false,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyLamiaMove,
                BarkSound = (int)SoundClips.EnemyLamiaBark,
                AttackSound = (int)SoundClips.EnemyLamiaAttack,
                MinMetalToHit = WeaponMaterialTypes.None,

                // [OSORKON] Less damage, much more HP and Armor. They swim slowly and only appear underwater.
                // They are very rare.
                MinDamage = 1,
                MaxDamage = 15,
                MinHealth = 51,
                MaxHealth = 153,
                Level = 16,
                ArmorValue = 0,
                ParrySounds = false,
                MapChance = 0,
                LootTableKey = "R",
                Weight = 200, // Using same value as nymph
                SoulPts = 60000,
                PrimaryAttackAnimFrames = new int[] { 0, -1, 1, 2, 3, 4, -1, 5, 0 },
                ChanceForAttack2 = 33,
                PrimaryAttackAnimFrames2 = new int[] { 0, 3, -1, 5, 4, 3, 3, -1, 5, 4, 3, -1, 5, 0 },
                ChanceForAttack3 = 33,
                PrimaryAttackAnimFrames3 = new int[] { 0, 3, -1, 5, 0 },
                Team = MobileTeams.Aquatic,
                MoveSpeed = 4f,
            },

            // Mage

            // [OSORKON] Mages move slowly. Their level scales with player's level until player is level 7, then they
            // can be any level from 1-20 but will usually be around level 10. Their melee damage, armor, and loot quality
            // scales with their level. Their armor isn't changed by their equipment, it now only varies based on enemy level,
            // and they do far more melee damage at high levels. HP unchanged from vanilla. Greatly increased spell variety.
            // They are most commonly found outside in towns at night and are fairly common in most wilderness areas during
            // day or night, and in several dungeon types.
            new MobileEnemy()
            {
                ID = 128,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Human,
                MaleTexture = 486,
                FemaleTexture = 485,
                CorpseTexture = CorpseTexture(380, 1),
                HasIdle = true,
                HasRangedAttack1 = true,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyHumanMove,
                BarkSound = (int)SoundClips.EnemyHumanBark,
                AttackSound = (int)SoundClips.EnemyHumanAttack,
                ParrySounds = false,
                MapChance = 3,
                LootTableKey = "U",
                CastsMagic = true,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 3, 2, 1, 0, -1, 5, 4, 0 },
                ChanceForAttack2 = 33,
                PrimaryAttackAnimFrames2 = new int[] { 0, 1, -1, 3, 2, 1, 0 },
                ChanceForAttack3 = 33,
                PrimaryAttackAnimFrames3 = new int[] { 0, -1, 5, 4, 0 },
                HasSpellAnimation = true,
                SpellAnimFrames = new int[] { 0, 1, 2, 3, 3 },
                Team = MobileTeams.KnightsAndMages,
                MoveSpeed = 4f,
            },

            // Spellsword

            // [OSORKON] Spellswords move fast. Their level scales with player's level until player is level 7, then they
            // can be any level from 1-20 but will usually be around level 10. Their damage, armor, and loot quality
            // scales with their level. Their armor isn't changed by their equipment, it now only varies based on enemy level,
            // and they do far more damage at high levels. HP unchanged from vanilla. Greatly increased spell variety.
            // They are most commonly found outside in towns at night and are fairly common in most wilderness areas during
            // day or night, and in several dungeon types.
            new MobileEnemy()
            {
                ID = 129,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Human,
                MaleTexture = 476,
                FemaleTexture = 475,
                CorpseTexture = CorpseTexture(380, 1),
                HasIdle = true,
                HasRangedAttack1 = true,
                HasRangedAttack2 = false,       // Female has RangedAttack2, male variant does not. Setting false for consistency.
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyHumanMove,
                BarkSound = (int)SoundClips.EnemyHumanBark,
                AttackSound = (int)SoundClips.EnemyHumanAttack,
                ParrySounds = true,
                MapChance = 1,
                LootTableKey = "P",
                CastsMagic = true,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2, 3, 4, 5 },
                ChanceForAttack2 = 33,
                PrimaryAttackAnimFrames2 = new int[] { 5, 4, 3, -1, 2, 1, 0 },
                ChanceForAttack3 = 33,
                PrimaryAttackAnimFrames3 = new int[] { 0, 1, -1, 2, 2, 1, 0 },
                RangedAttackAnimFrames = new int[] { 3, 2, 0, 0, 0, -1, 1, 1, 2, 3 },
                HasSpellAnimation = true,
                SpellAnimFrames = new int[] { 0, 1, 2, 3, 3 },
                Team = MobileTeams.KnightsAndMages,
                MoveSpeed = 7f,
            },

            // Battlemage

            // [OSORKON] Battlemages move somewhat fast. Their level scales with player's level until player is level 7, then they
            // can be any level from 1-20 but will usually be around level 10. Their damage, armor, and loot quality
            // scales with their level. Their armor isn't changed by their equipment, it now only varies based on enemy level,
            // and they do far more damage at high levels. HP unchanged from vanilla. Greatly increased spell variety.
            // They are most commonly found outside in towns at night and are fairly common in most wilderness areas during
            // day or night, and in several dungeon types.
            new MobileEnemy()
            {
                ID = 130,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Human,
                MaleTexture = 490,
                FemaleTexture = 489,
                CorpseTexture = CorpseTexture(380, 1),
                HasIdle = true,
                HasRangedAttack1 = true,
                HasRangedAttack2 = true,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyHumanMove,
                BarkSound = (int)SoundClips.EnemyHumanBark,
                AttackSound = (int)SoundClips.EnemyHumanAttack,
                ParrySounds = true,
                MapChance = 2,
                LootTableKey = "U",
                CastsMagic = true,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2, 3, 4, -1, 5, 0 },
                ChanceForAttack2 = 33,
                PrimaryAttackAnimFrames2 = new int[] { 4, 4, -1, 5, 0, 0 },
                ChanceForAttack3 = 33,
                PrimaryAttackAnimFrames3 = new int[] { 4, -1, 5, 0, 0, 1, -1, 2, 3, 4, -1, 5, 0 },
                RangedAttackAnimFrames = new int[] { 3, 2, 0, 0, 0, -1, 1, 1, 2, 3 },
                HasSpellAnimation = true,
                SpellAnimFrames = new int[] { 0, 1, 2, 3, 3 },
                Team = MobileTeams.KnightsAndMages,
                MoveSpeed = 6f,
            },

            // Sorcerer

            // [OSORKON] Sorcerers move slowly. Their level scales with player's level until player is level 7, then they
            // can be any level from 1-20 but will usually be around level 10. Their damage, armor, and loot quality
            // scales with their level. Their armor isn't changed by their equipment, it now only varies based on enemy level,
            // and they do far more damage at high levels. HP unchanged from vanilla. They are most commonly found outside
            // in towns at night and are fairly common in most wilderness areas during day or night, and in several dungeon types.
            new MobileEnemy()
            {
                ID = 131,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Human,
                MaleTexture = 478,
                FemaleTexture = 477,
                CorpseTexture = CorpseTexture(380, 1),
                HasIdle = true,
                HasRangedAttack1 = true,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyHumanMove,
                BarkSound = (int)SoundClips.EnemyHumanBark,
                AttackSound = (int)SoundClips.EnemyHumanAttack,
                ParrySounds = false,
                MapChance = 3,
                LootTableKey = "U",
                CastsMagic = false,
                PrimaryAttackAnimFrames = new int[] { 0, 1, 2, -1, 3, 4, 5 },
                ChanceForAttack2 = 50,
                PrimaryAttackAnimFrames2 = new int[] { 4, 5, -1, 3, 2, 1, 0 },
                RangedAttackAnimFrames = new int[] { 3, 2, 0, 0, 0, -1, 1, 1, 2, 3 },
                Team = MobileTeams.KnightsAndMages,
                MoveSpeed = 4f,
            },

            // Healer

            // [OSORKON] Healers move slowly. Their level scales with player's level until player is level 7, then they
            // can be any level from 1-20 but will usually be around level 10. Their melee damage, armor, and loot quality
            // scales with their level. Their armor isn't changed by their equipment, it now only varies based on enemy level,
            // and they do far more melee damage at high levels. HP unchanged from vanilla. Greatly increased spell variety.
            // They are most commonly found outside in towns at night and are fairly common in most wilderness areas during
            // day or night, and in several dungeon types.
            new MobileEnemy()
            {
                ID = 132,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Human,
                MaleTexture = 486,
                FemaleTexture = 485,
                CorpseTexture = CorpseTexture(380, 1),
                HasIdle = true,
                HasRangedAttack1 = true,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyHumanMove,
                BarkSound = (int)SoundClips.EnemyHumanBark,
                AttackSound = (int)SoundClips.EnemyHumanAttack,
                ParrySounds = false,
                MapChance = 1,
                LootTableKey = "U",
                CastsMagic = true,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 3, 2, 1, 0, -1, 5, 4, 0 },
                ChanceForAttack2 = 33,
                PrimaryAttackAnimFrames2 = new int[] { 0, 1, -1, 3, 2, 1, 0 },
                ChanceForAttack3 = 33,
                PrimaryAttackAnimFrames3 = new int[] { 0, -1, 5, 4, 0 },
                HasSpellAnimation = true,
                SpellAnimFrames = new int[] { 0, 1, 2, 3, 3 },
                Team = MobileTeams.KnightsAndMages,
                MoveSpeed = 4f,
            },

            // Nightblade

            // [OSORKON] Nightblades move fast. Their level scales with player's level until player is level 7, then they
            // can be any level from 1-20 but will usually be around level 10. Their damage, armor, and loot quality
            // scales with their level. Their armor isn't changed by their equipment, it now only varies based on enemy level,
            // and they do far more damage at high levels. HP unchanged from vanilla. Greatly increased spell variety.
            // They are most commonly found outside in towns at night and are fairly common in most wilderness areas during
            // day or night, and in several dungeon types.
            new MobileEnemy()
            {
                ID = 133,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Human,
                MaleTexture = 490,
                FemaleTexture = 489,
                CorpseTexture = CorpseTexture(380, 1),
                HasIdle = true,
                HasRangedAttack1 = true,
                HasRangedAttack2 = true,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyHumanMove,
                BarkSound = (int)SoundClips.EnemyHumanBark,
                AttackSound = (int)SoundClips.EnemyHumanAttack,
                ParrySounds = true,
                MapChance = 1,
                LootTableKey = "U",
                CastsMagic = true,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2, 3, 4, -1, 5, 0 },
                ChanceForAttack2 = 33,
                PrimaryAttackAnimFrames2 = new int[] { 4, 4, -1, 5, 0, 0 },
                ChanceForAttack3 = 33,
                PrimaryAttackAnimFrames3 = new int[] { 4, -1, 5, 0, 0, 1, -1, 2, 3, 4, -1, 5, 0 },
                RangedAttackAnimFrames = new int[] { 3, 2, 0, 0, 0, -1, 1, 1, 2, 3 },
                HasSpellAnimation = true,
                SpellAnimFrames = new int[] { 0, 1, 2, 3, 3 },
                Team = MobileTeams.Criminals,
                MoveSpeed = 7.5f,
            },

            // Bard

            // [OSORKON] Bards move somewhat fast. Their level scales with player's level until player is level 7, then they
            // can be any level from 1-20 but will usually be around level 10. Their damage, armor, and loot quality
            // scales with their level. Their armor isn't changed by their equipment, it now only varies based on enemy level,
            // and they do far more damage at high levels. HP unchanged from vanilla. They are most commonly found outside
            // in towns at night and are fairly common in most wilderness areas during day or night, and in several dungeon types.
            new MobileEnemy()
            {
                ID = 134,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Human,
                MaleTexture = 484,
                FemaleTexture = 483,
                CorpseTexture = CorpseTexture(380, 1),
                HasIdle = true,
                HasRangedAttack1 = true,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyHumanMove,
                BarkSound = (int)SoundClips.EnemyHumanBark,
                AttackSound = (int)SoundClips.EnemyHumanAttack,
                ParrySounds = true,
                MapChance = 2,
                LootTableKey = "O",
                CastsMagic = false,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2, 3, 4, -1, 5, 0 },
                ChanceForAttack2 = 33,
                PrimaryAttackAnimFrames2 = new int[] { 4, 4, -1, 5, 0, 0 },
                ChanceForAttack3 = 33,
                PrimaryAttackAnimFrames3 = new int[] { 4, -1, 5, 0, 0, 1, -1, 2, 3, 4, -1, 5, 0 },
                RangedAttackAnimFrames = new int[] { 3, 2, 0, 0, 0, -1, 1, 1, 2, 3 },
                Team = MobileTeams.Criminals,
                MoveSpeed = 6.5f,
            },

            // Burglar

            // [OSORKON] Burglars move fast. Their level scales with player's level until player is level 7, then they
            // can be any level from 1-20 but will usually be around level 10. Their damage, armor, and loot quality
            // scales with their level. Their armor isn't changed by their equipment, it now only varies based on enemy level,
            // and they do far more damage at high levels. HP unchanged from vanilla. They are most commonly found outside
            // in towns at night and are fairly common in most wilderness areas during day or night, and in several dungeon types.
            new MobileEnemy()
            {
                ID = 135,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Human,
                MaleTexture = 484,
                FemaleTexture = 483,
                CorpseTexture = CorpseTexture(380, 1),
                HasIdle = true,
                HasRangedAttack1 = true,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyHumanMove,
                BarkSound = (int)SoundClips.EnemyHumanBark,
                AttackSound = (int)SoundClips.EnemyHumanAttack,
                ParrySounds = true,
                MapChance = 1,
                LootTableKey = "O",
                CastsMagic = false,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2, 3, 4, -1, 5, 0 },
                ChanceForAttack2 = 33,
                PrimaryAttackAnimFrames2 = new int[] { 4, 4, -1, 5, 0, 0 },
                ChanceForAttack3 = 33,
                PrimaryAttackAnimFrames3 = new int[] { 4, -1, 5, 0, 0, 1, -1, 2, 3, 4, -1, 5, 0 },
                RangedAttackAnimFrames = new int[] { 3, 2, 0, 0, 0, -1, 1, 1, 2, 3 },
                Team = MobileTeams.Criminals,
                MoveSpeed = 7.5f,
            },

            // Rogue

            // [OSORKON] Rogues move fast. Their level scales with player's level until player is level 7, then they
            // can be any level from 1-20 but will usually be around level 10. Their damage, armor, and loot quality
            // scales with their level. Their armor isn't changed by their equipment, it now only varies based on enemy level,
            // and they do far more damage at high levels. HP unchanged from vanilla. They are most commonly found outside
            // in towns at night and are fairly common in most wilderness areas during day or night, and in several dungeon types.
            new MobileEnemy()
            {
                ID = 136,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Human,
                MaleTexture = 480,
                FemaleTexture = 479,
                CorpseTexture = CorpseTexture(380, 1),
                HasIdle = true,
                HasRangedAttack1 = true,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyHumanMove,
                BarkSound = (int)SoundClips.EnemyHumanBark,
                AttackSound = (int)SoundClips.EnemyHumanAttack,
                ParrySounds = true,
                MapChance = 1,
                LootTableKey = "O",
                CastsMagic = false,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2, 3, 4, -1, 5, 0 },
                ChanceForAttack2 = 33,
                PrimaryAttackAnimFrames2 = new int[] { 4, 4, -1, 5, 0, 0 },
                ChanceForAttack3 = 33,
                PrimaryAttackAnimFrames3 = new int[] { 4, -1, 5, 0, 0, 1, -1, 2, 3, 4, -1, 5, 0 },
                RangedAttackAnimFrames = new int[] { 3, 2, 0, 0, 0, -1, 1, 1, 2, 3 },
                Team = MobileTeams.Criminals,
                MoveSpeed = 7.5f,
            },

            // Acrobat

            // [OSORKON] Acrobats move very fast. Their level scales with player's level until player is level 7, then they
            // can be any level from 1-20 but will usually be around level 10. Their damage, armor, and loot quality
            // scales with their level. Their armor isn't changed by their equipment, it now only varies based on enemy level,
            // and they do far more damage at high levels. HP unchanged from vanilla. They are most commonly found outside
            // in towns at night and are fairly common in most wilderness areas during day or night, and in several dungeon types.
            new MobileEnemy()
            {
                ID = 137,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Human,
                MaleTexture = 484,
                FemaleTexture = 483,
                CorpseTexture = CorpseTexture(380, 1),
                HasIdle = true,
                HasRangedAttack1 = true,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyHumanMove,
                BarkSound = (int)SoundClips.EnemyHumanBark,
                AttackSound = (int)SoundClips.EnemyHumanAttack,
                ParrySounds = true,
                MapChance = 0,
                LootTableKey = "O",
                CastsMagic = false,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2, 3, 4, -1, 5, 0 },
                ChanceForAttack2 = 33,
                PrimaryAttackAnimFrames2 = new int[] { 4, 4, -1, 5, 0, 0 },
                ChanceForAttack3 = 33,
                PrimaryAttackAnimFrames3 = new int[] { 4, -1, 5, 0, 0, 1, -1, 2, 3, 4, -1, 5, 0 },
                RangedAttackAnimFrames = new int[] { 3, 2, 0, 0, 0, -1, 1, 1, 2, 3 },
                Team = MobileTeams.Criminals,
                MoveSpeed = 8f,
            },

            // Thief

            // [OSORKON] Thieves move fast. Their level scales with player's level until player is level 7, then they
            // can be any level from 1-20 but will usually be around level 10. Their damage, armor, and loot quality
            // scales with their level. Their armor isn't changed by their equipment, it now only varies based on enemy level,
            // and they do far more damage at high levels. HP unchanged from vanilla. They are most commonly found outside
            // in towns at night and are fairly common in most wilderness areas during day or night, and in several dungeon types.
            new MobileEnemy()
            {
                ID = 138,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Human,
                MaleTexture = 484,
                FemaleTexture = 483,
                CorpseTexture = CorpseTexture(380, 1),
                HasIdle = true,
                HasRangedAttack1 = true,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyHumanMove,
                BarkSound = (int)SoundClips.EnemyHumanBark,
                AttackSound = (int)SoundClips.EnemyHumanAttack,
                ParrySounds = true,
                MapChance = 2,
                LootTableKey = "O",
                CastsMagic = false,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2, 3, 4, -1, 5, 0 },
                ChanceForAttack2 = 33,
                PrimaryAttackAnimFrames2 = new int[] { 4, 4, -1, 5, 0, 0 },
                ChanceForAttack3 = 33,
                PrimaryAttackAnimFrames3 = new int[] { 4, -1, 5, 0, 0, 1, -1, 2, 3, 4, -1, 5, 0 },
                RangedAttackAnimFrames = new int[] { 3, 2, 0, 0, 0, -1, 1, 1, 2, 3 },
                Team = MobileTeams.Criminals,
                MoveSpeed = 7.5f,
            },

            // Assassin

            // [OSORKON] The third easiest boss of the game, tougher than a Lich but not as tough as an Alternate Dragonling.
            // Assassins move extremely fast. Privateer's Hold is a Human Stronghold dungeon type, and Assassins are the bosses
            // of that dungeon type. Thus, Assassins can rarely spawn in Privateer's Hold, and I don't want an unlucky player
            // running into a boss that early on. To avoid that extremely frustrating scenario, Assassins follow standard class
            // enemy unleveling rules. If player is level 1-6, Assassins don't have boss stats and their level, armor, HP,
            // and damage scales with player's level. At player level 7+ Assassins will be Level 21-30, have -8 Armor, 100-300 HP,
            // and deal around 39-67 damage. Always wields a poisoned weapon unless player is level 1. Once player is level 7
            // Assassins will likely drop good loot and their poison will bypass player's Poison Immunity. They are very rare.
            new MobileEnemy()
            {
                ID = 139,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Human,
                MaleTexture = 480,
                FemaleTexture = 479,
                CorpseTexture = CorpseTexture(380, 1),
                HasIdle = true,
                HasRangedAttack1 = true,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyHumanMove,
                BarkSound = (int)SoundClips.EnemyHumanBark,
                AttackSound = (int)SoundClips.EnemyHumanAttack,
                ParrySounds = true,
                MapChance = 0,

                // [OSORKON] All bosses see Invisible.
                SeesThroughInvisibility = true,
                LootTableKey = "O",
                CastsMagic = false,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2, 3, 4, -1, 5, 0 },
                ChanceForAttack2 = 33,
                PrimaryAttackAnimFrames2 = new int[] { 4, 4, -1, 5, 0, 0 },
                ChanceForAttack3 = 33,
                PrimaryAttackAnimFrames3 = new int[] { 4, -1, 5, 0, 0, 1, -1, 2, 3, 4, -1, 5, 0 },
                RangedAttackAnimFrames = new int[] { 3, 2, 0, 0, 0, -1, 1, 1, 2, 3 },
                Team = MobileTeams.Criminals,
                MoveSpeed = 9f,
            },

            // Monk

            // [OSORKON] Monks move fast. Their level scales with player's level until player is level 7, then they
            // can be any level from 1-20 but will usually be around level 10. Their damage, armor, and loot quality
            // scales with their level. Their armor isn't changed by their equipment, it now only varies based on enemy level,
            // and they do far more damage at high levels. HP unchanged from vanilla. They are most commonly found outside
            // in towns at night and are fairly common in most wilderness areas during day or night, and in several dungeon types.
            new MobileEnemy()
            {
                ID = 140,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Human,
                MaleTexture = 488,
                FemaleTexture = 487,
                CorpseTexture = CorpseTexture(380, 1),
                HasIdle = true,
                HasRangedAttack1 = true,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyHumanMove,
                BarkSound = (int)SoundClips.EnemyHumanBark,
                AttackSound = (int)SoundClips.EnemyHumanAttack,
                ParrySounds = true,
                MapChance = 1,
                LootTableKey = "T",
                CastsMagic = false,
                PrimaryAttackAnimFrames = new int[] { 0, 0, 1, -1, 2, 2, 1, 0 },
                ChanceForAttack2 = 33,
                PrimaryAttackAnimFrames2 = new int[] { 0, 1, -1, 2, 3, 4, 5 },
                ChanceForAttack3 = 33,
                PrimaryAttackAnimFrames3 = new int[] { 5, 5, 3, -1, 2, 1, 0 },
                RangedAttackAnimFrames = new int[] { 3, 2, 0, 0, 0, -1, 1, 1, 2, 3 },
                Team = MobileTeams.KnightsAndMages,
                MoveSpeed = 7f,
            },

            // Archer

            // [OSORKON] Archers move slowly. Their level scales with player's level until player is level 7, then they
            // can be any level from 1-20 but will usually be around level 10. Their damage, armor, and loot quality
            // scales with their level. Their armor isn't changed by their equipment, it now only varies based on enemy level,
            // and they do far more damage at high levels. HP unchanged from vanilla. They are most commonly found outside
            // in towns at night and are fairly common in most wilderness areas during day or night, and in several dungeon types.
            // If using Enhanced Combat AI Archers will never voluntarily move into melee range and will always retreat if player
            // charges them, preferring to stay at range and shoot arrows.
            new MobileEnemy()
            {
                ID = 141,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Human,
                MaleTexture = 482,
                FemaleTexture = 481,
                CorpseTexture = CorpseTexture(380, 1),
                HasIdle = true,
                HasRangedAttack1 = true,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                PrefersRanged = true,
                MoveSound = (int)SoundClips.EnemyHumanMove,
                BarkSound = (int)SoundClips.EnemyHumanBark,
                AttackSound = (int)SoundClips.EnemyHumanAttack,
                ParrySounds = true,
                MapChance = 0,
                LootTableKey = "C",
                CastsMagic = false,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2, 3, 4, -1, 5 },
                ChanceForAttack2 = 50,
                PrimaryAttackAnimFrames2 = new int[] { 3, 4, -1, 5, 0 },
                RangedAttackAnimFrames = new int[] { 3, 2, 0, 0, 0, -1, 1, 1, 2, 3 },
                Team = MobileTeams.KnightsAndMages,
                MoveSpeed = 4.5f,
            },

            // Ranger

            // [OSORKON] Rangers move slowly. Their level scales with player's level until player is level 7, then they
            // can be any level from 1-20 but will usually be around level 10. Their damage, armor, and loot quality
            // scales with their level. Their armor isn't changed by their equipment, it now only varies based on enemy level,
            // and they do far more damage at high levels. HP unchanged from vanilla. They are most commonly found outside
            // in towns at night and are fairly common in most wilderness areas during day or night. Rangers are the only humans
            // brave (or stupid) enough to wander through Desert wilderness. They also spawn in Mountain and Mountain Woods
            // wilderness at night, where it is too cold for most humans. They are only in Natural Cave dungeon types. If using
            // Enhanced Combat AI Rangers will never voluntarily move into melee range and will always retreat if player charges
            // them, preferring to stay at range and shoot arrows.
            new MobileEnemy()
            {
                ID = 142,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Human,
                MaleTexture = 482,
                FemaleTexture = 481,
                CorpseTexture = CorpseTexture(380, 1),
                HasIdle = true,
                HasRangedAttack1 = true,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyHumanMove,
                BarkSound = (int)SoundClips.EnemyHumanBark,
                AttackSound = (int)SoundClips.EnemyHumanAttack,
                ParrySounds = true,
                MapChance = 1,
                LootTableKey = "C",
                CastsMagic = false,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2, 3, 4, -1, 5 },
                ChanceForAttack2 = 50,
                PrimaryAttackAnimFrames2 = new int[] { 3, 4, -1, 5, 0 },
                RangedAttackAnimFrames = new int[] { 3, 2, 0, 0, 0, -1, 1, 1, 2, 3 },
                Team = MobileTeams.KnightsAndMages,
                MoveSpeed = 4.5f,
            },

            // Barbarian

            // [OSORKON] Barbarians move somewhat fast. Their level scales with player's level until player is level 7, then they
            // can be any level from 1-20 but will usually be around level 10. Their damage, armor, and loot quality
            // scales with their level. Their armor isn't changed by their equipment, it now only varies based on enemy level,
            // and they do far more damage at high levels. HP unchanged from vanilla. They are most commonly found outside in towns
            // at night and are fairly common in most wilderness areas during day or night. They will frequently spawn in Mountain
            // and Mountain Woods wilderness at night, where it is too cold for most humans. They spawn in a handful of dungeon
            // types. Barbarians always charge.
            new MobileEnemy()
            {
                ID = 143,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Human,
                MaleTexture = 488,
                FemaleTexture = 487,
                CorpseTexture = CorpseTexture(380, 1),
                HasIdle = true,
                HasRangedAttack1 = true,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyHumanMove,
                BarkSound = (int)SoundClips.EnemyHumanBark,
                AttackSound = (int)SoundClips.EnemyHumanAttack,
                ParrySounds = true,
                MapChance = 0,
                LootTableKey = "T",
                CastsMagic = false,
                PrimaryAttackAnimFrames = new int[] { 0, 0, 1, -1, 2, 2, 1, 0 },
                ChanceForAttack2 = 33,
                PrimaryAttackAnimFrames2 = new int[] { 0, 1, -1, 2, 3, 4, 5 },
                ChanceForAttack3 = 33,
                PrimaryAttackAnimFrames3 = new int[] { 5, 5, 3, -1, 2, 1, 0 },
                RangedAttackAnimFrames = new int[] { 3, 2, 0, 0, 0, -1, 1, 1, 2, 3 },
                Team = MobileTeams.Criminals,
                MoveSpeed = 6.5f,
            },

            // Warrior

            // [OSORKON] Warriors move somewhat slowly. Their level scales with player's level until player is level 7, then they
            // can be any level from 1-20 but will usually be around level 10. Their damage, armor, and loot quality
            // scales with their level. Their armor isn't changed by their equipment, it now only varies based on enemy level,
            // and they do far more damage at high levels. HP unchanged from vanilla. They are most commonly found outside
            // in towns at night and are fairly common in most wilderness areas during day or night, and in several dungeon types.
            new MobileEnemy()
            {
                ID = 144,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Human,
                MaleTexture = 488,
                FemaleTexture = 487,
                CorpseTexture = CorpseTexture(380, 1),
                HasIdle = true,
                HasRangedAttack1 = true,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyHumanMove,
                BarkSound = (int)SoundClips.EnemyHumanBark,
                AttackSound = (int)SoundClips.EnemyHumanAttack,
                ParrySounds = true,
                MapChance = 0,
                LootTableKey = "T",
                CastsMagic = false,
                PrimaryAttackAnimFrames = new int[] { 0, 0, 1, -1, 2, 2, 1, 0 },
                ChanceForAttack2 = 33,
                PrimaryAttackAnimFrames2 = new int[] { 0, 1, -1, 2, 3, 4, 5 },
                ChanceForAttack3 = 33,
                PrimaryAttackAnimFrames3 = new int[] { 5, 5, 3, -1, 2, 1, 0 },
                RangedAttackAnimFrames = new int[] { 3, 2, 0, 0, 0, -1, 1, 1, 2, 3 },
                Team = MobileTeams.KnightsAndMages,
                MoveSpeed = 5.75f,
            },

            // Knight

            // [OSORKON] Knights move somewhat slowly. Their level scales with player's level until player is level 7, then they
            // can be any level from 1-20 but will usually be around level 10. Their damage, armor, and loot quality
            // scales with their level. Their armor isn't changed by their equipment, it now only varies based on enemy level,
            // and they do far more damage at high levels. HP unchanged from vanilla. They are most commonly found outside
            // in towns at night and are fairly common in most wilderness areas during day or night, and in several dungeon types.
            new MobileEnemy()
            {
                ID = 145,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Human,
                MaleTexture = 488,
                FemaleTexture = 487,
                CorpseTexture = CorpseTexture(380, 1),
                HasIdle = true,
                HasRangedAttack1 = true,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyHumanMove,
                BarkSound = (int)SoundClips.EnemyHumanBark,
                AttackSound = (int)SoundClips.EnemyHumanAttack,
                ParrySounds = true,
                MapChance = 1,
                LootTableKey = "T",
                CastsMagic = false,
                PrimaryAttackAnimFrames = new int[] { 0, 0, 1, -1, 2, 2, 1, 0 },
                ChanceForAttack2 = 33,
                PrimaryAttackAnimFrames2 = new int[] { 0, 1, -1, 2, 3, 4, 5 },
                ChanceForAttack3 = 33,
                PrimaryAttackAnimFrames3 = new int[] { 5, 5, 3, -1, 2, 1, 0 },
                RangedAttackAnimFrames = new int[] { 3, 2, 0, 0, 0, -1, 1, 1, 2, 3 },
                Team = MobileTeams.KnightsAndMages,
                MoveSpeed = 5f,
            },

            // City Watch - The Haltmeister

            // [OSORKON] Guards move very fast. Their level sort of scales with player's level until player is level 7 - I say
            // "sort of" because they always get a random level boost of 0 to 10 at any player level, so "scales" is not very
            // accurate as their level will vary wildly. Once player is level 7 Guards can be levels 1-30 but on average will
            // be around level 15. If you're not using "Roleplay and Realism: Items" Guard equipment scales with their level, which
            // means Guards will likely drop a ton of Daedric once player is level 7. Their armor isn't changed by their equipment -
            // it scales with their level - and they can do incredible damage at higher levels. HP unchanged from vanilla. They will
            // also happily riddle you with arrows. Lawbreakers beware! And by the way... HALT!
            new MobileEnemy()
            {
                ID = 146,
                Behaviour = MobileBehaviour.Guard,
                Affinity = MobileAffinity.Human,

                // [OSORKON] I changed hostile Guards to use Male Knight textures and animations so they could shoot arrows.
                // Non-hostile Guards use vanilla textures. HALT!
                MaleTexture = 488,
                FemaleTexture = 488,
                CorpseTexture = CorpseTexture(380, 1),
                HasIdle = true,

                // [OSORKON] I gave them RangedAttack1, same as Knights. HALT!
                HasRangedAttack1 = true,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.None,
                BarkSound = (int)SoundClips.Halt,
                AttackSound = (int)SoundClips.None,
                ParrySounds = true,
                MapChance = 0,
                CastsMagic = false,

                // [OSORKON] I copied and pasted all Knight PrimaryAttack and RangedAttack animations here. Now Guards shoot
                // arrows, which removes the old exploit of getting them stuck on something and mowing them down. Ranged attacks
                // from Guards also trigger the "do you surrender" pop-up, which makes escaping justice much tougher. HALT!
                PrimaryAttackAnimFrames = new int[] { 0, 0, 1, -1, 2, 2, 1, 0 },
                ChanceForAttack2 = 33,
                PrimaryAttackAnimFrames2 = new int[] { 0, 1, -1, 2, 3, 4, 5 },
                ChanceForAttack3 = 33,
                PrimaryAttackAnimFrames3 = new int[] { 5, 5, 3, -1, 2, 1, 0 },
                RangedAttackAnimFrames = new int[] { 3, 2, 0, 0, 0, -1, 1, 1, 2, 3 },
                Team = MobileTeams.CityWatch,
                MoveSpeed = 8f,

                // [OSORKON] HALT!
            },
        };

        #endregion

        #region Helpers

        public static int CorpseTexture(int archive, int record)
        {
            return ((archive << 16) + record);
        }

        public static void ReverseCorpseTexture(int corpseTexture, out int archive, out int record)
        {
            archive = corpseTexture >> 16;
            record = corpseTexture & 0xffff;
        }

        /// <summary>
        /// Build a dictionary of enemies keyed by ID.
        /// Use this once and store for faster enemy lookups.
        /// </summary>
        /// <returns>Resulting dictionary of mobile enemies.</returns>
        public static Dictionary<int, MobileEnemy> BuildEnemyDict()
        {
            Dictionary<int, MobileEnemy> enemyDict = new Dictionary<int, MobileEnemy>();
            foreach (var enemy in Enemies)
            {
                enemyDict.Add(enemy.ID, enemy);
            }

            return enemyDict;
        }

        /// <summary>
        /// Gets enemy definition based on type.
        /// Runs a brute force search for ID, so use sparingly.
        /// Store a dictionary from GetEnemyDict() for faster lookups.
        /// </summary>
        /// <param name="enemyType">Enemy type to extract definition.</param>
        /// <param name="mobileEnemyOut">Receives details of enemy type.</param>
        /// <returns>True if successful.</returns>
        public static bool GetEnemy(MobileTypes enemyType, out MobileEnemy mobileEnemyOut)
        {
            // Cast type enum to ID.
            // You can add additional IDs to enum to create new enemies.
            int id = (int)enemyType;

            // Search for matching definition in enemy list.
            // Don't forget to add new enemy IDs to Enemies definition array.
            for (int i = 0; i < Enemies.Length; i++)
            {
                if (Enemies[i].ID == id)
                {
                    mobileEnemyOut = Enemies[i];
                    return true;
                }
            }

            // No match found, just return an empty definition
            mobileEnemyOut = new MobileEnemy();
            return false;
        }

        /// <summary>
        /// Gets enemy definition based on name.
        /// Runs a brute force search for ID, so use sparingly.
        /// </summary>
        /// <param name="name">Enemy name to extract definition.</param>
        /// <param name="mobileEnemyOut">Receives details of enemy type if found.</param>
        /// <returns>True if successful.</returns>
        public static bool GetEnemy(string name, out MobileEnemy mobileEnemyOut)
        {
            for (int i = 0; i < Enemies.Length; i++)
            {
                if (0 == string.Compare(TextManager.Instance.GetLocalizedEnemyName(Enemies[i].ID), name, StringComparison.InvariantCultureIgnoreCase))
                {
                    mobileEnemyOut = Enemies[i];
                    return true;
                }
            }

            // No match found, just return an empty definition
            mobileEnemyOut = new MobileEnemy();
            return false;
        }

        #endregion

    }
}
