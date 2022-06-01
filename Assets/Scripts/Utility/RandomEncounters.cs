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
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.Utility;

namespace DaggerfallWorkshop.Utility
{
    #region Encounter Tables

    /// <summary>
    /// Static definitions for random encounters based on dungeon type, from FALL.EXE.
    /// All lists from classic have 20 entries.
    /// These are generally ordered from low-level through to high-level encounters.
    /// </summary>
    public static class RandomEncounters
    {
        public static RandomEncounterTable[] EncounterTables = new RandomEncounterTable[]
        {
            // Crypt - Index0
            new RandomEncounterTable()
            {
                DungeonType = DFRegion.DungeonTypes.Crypt,
                Enemies = new MobileTypes[]
                {
                    MobileTypes.Rat,
                    MobileTypes.SkeletalWarrior,
                    MobileTypes.GiantBat,
                    MobileTypes.Rat,
                    MobileTypes.SkeletalWarrior,
                    MobileTypes.GiantBat,
                    MobileTypes.Mummy,
                    MobileTypes.SkeletalWarrior,
                    MobileTypes.Spider,
                    MobileTypes.Zombie,
                    MobileTypes.Ghost,
                    MobileTypes.Zombie,
                    MobileTypes.Zombie,
                    MobileTypes.Ghost,
                    MobileTypes.Ghost,
                    MobileTypes.Wraith,
                    MobileTypes.Wraith,
                    MobileTypes.Vampire,
                    MobileTypes.VampireAncient,
                    MobileTypes.Lich,
                },
            },

            // Orc Stronghold - Index1
            new RandomEncounterTable()
            {
                DungeonType = DFRegion.DungeonTypes.OrcStronghold,
                Enemies = new MobileTypes[]
                {
                    MobileTypes.Rat,
                    MobileTypes.GiantBat,
                    MobileTypes.Orc,
                    MobileTypes.OrcSergeant,
                    MobileTypes.GrizzlyBear,
                    MobileTypes.Orc,
                    MobileTypes.SkeletalWarrior,
                    MobileTypes.Orc,
                    MobileTypes.OrcSergeant,
                    MobileTypes.SabertoothTiger,
                    MobileTypes.Giant,
                    MobileTypes.OrcShaman,
                    MobileTypes.Spider,
                    MobileTypes.OrcShaman,
                    MobileTypes.OrcWarlord,
                    MobileTypes.GiantScorpion,
                    MobileTypes.Daedroth,
                    MobileTypes.GiantScorpion,
                    MobileTypes.OrcShaman,
                    MobileTypes.OrcWarlord,
                },
            },

            // Human Stronghold - Index2
            new RandomEncounterTable()
            {
                DungeonType = DFRegion.DungeonTypes.HumanStronghold,
                Enemies = new MobileTypes[]
                {
                    MobileTypes.Warrior,
                    MobileTypes.Rogue,
                    MobileTypes.Rat,
                    MobileTypes.GrizzlyBear,
                    MobileTypes.Archer,
                    MobileTypes.Nightblade,
                    MobileTypes.GiantBat,
                    MobileTypes.Spellsword,
                    MobileTypes.Centaur,
                    MobileTypes.Knight,
                    MobileTypes.Warrior,
                    MobileTypes.Rogue,
                    MobileTypes.Barbarian,
                    MobileTypes.OrcWarlord,
                    MobileTypes.Archer,
                    MobileTypes.Wraith,
                    MobileTypes.Spellsword,
                    MobileTypes.OrcShaman,
                    MobileTypes.Warrior,
                    MobileTypes.Vampire,
                },
            },

            // Prison - Index3
            new RandomEncounterTable()
            {
                DungeonType = DFRegion.DungeonTypes.Prison,
                Enemies = new MobileTypes[]
                {
                    MobileTypes.GiantBat,
                    MobileTypes.Rat,
                    MobileTypes.Bard,
                    MobileTypes.SkeletalWarrior,
                    MobileTypes.Burglar,
                    MobileTypes.Spider,
                    MobileTypes.Nightblade,
                    MobileTypes.Spider,
                    MobileTypes.Barbarian,
                    MobileTypes.Thief,
                    MobileTypes.FleshAtronach,
                    MobileTypes.Thief,
                    MobileTypes.Assassin,
                    MobileTypes.Zombie,
                    MobileTypes.Nightblade,
                    MobileTypes.IronAtronach,
                    MobileTypes.Wraith,
                    MobileTypes.Ghost,
                    MobileTypes.Zombie,
                    MobileTypes.Burglar,
                },
            },

            // Desecrated Temple - Index4
            new RandomEncounterTable()
            {
                DungeonType = DFRegion.DungeonTypes.DesecratedTemple,
                Enemies = new MobileTypes[]
                {
                    MobileTypes.GiantBat,
                    MobileTypes.Imp,
                    MobileTypes.Healer,
                    MobileTypes.Monk,
                    MobileTypes.Sorcerer,
                    MobileTypes.SkeletalWarrior,
                    MobileTypes.Harpy,
                    MobileTypes.Monk,
                    MobileTypes.Mummy,
                    MobileTypes.Monk,
                    MobileTypes.OrcShaman,
                    MobileTypes.Gargoyle,
                    MobileTypes.Wraith,
                    MobileTypes.Daedroth,
                    MobileTypes.DaedraSeducer,
                    MobileTypes.Monk,
                    MobileTypes.FrostDaedra,
                    MobileTypes.FireDaedra,
                    MobileTypes.Dragonling,
                    MobileTypes.DaedraLord,
                },
            },

            // Mine - Index5
            new RandomEncounterTable()
            {
                DungeonType = DFRegion.DungeonTypes.Mine,
                Enemies = new MobileTypes[]
                {
                    MobileTypes.Rat,
                    MobileTypes.GiantBat,
                    MobileTypes.GrizzlyBear,
                    MobileTypes.Spider,
                    MobileTypes.SkeletalWarrior,
                    MobileTypes.Spider,
                    MobileTypes.Nightblade,
                    MobileTypes.Orc,
                    MobileTypes.OrcSergeant,
                    MobileTypes.Giant,
                    MobileTypes.Thief,
                    MobileTypes.Giant,
                    MobileTypes.Warrior,
                    MobileTypes.IceAtronach,
                    MobileTypes.IronAtronach,
                    MobileTypes.FireDaedra,
                    MobileTypes.Thief,
                    MobileTypes.Vampire,
                    MobileTypes.Nightblade,
                    MobileTypes.VampireAncient,
                },
            },

            // Natural Cave - Index6
            new RandomEncounterTable()
            {
                DungeonType = DFRegion.DungeonTypes.NaturalCave,
                Enemies = new MobileTypes[]
                {
                    MobileTypes.Rat,
                    MobileTypes.GiantBat,
                    MobileTypes.GrizzlyBear,
                    MobileTypes.SabertoothTiger,
                    MobileTypes.GrizzlyBear,
                    MobileTypes.Spider,
                    MobileTypes.Orc,
                    MobileTypes.Werewolf,
                    MobileTypes.Barbarian,
                    MobileTypes.Harpy,
                    MobileTypes.Wereboar,
                    MobileTypes.Giant,
                    MobileTypes.GiantScorpion,
                    MobileTypes.OrcShaman,
                    MobileTypes.Ghost,
                    MobileTypes.Warrior,
                    MobileTypes.Monk,
                    MobileTypes.Barbarian,
                    MobileTypes.Dragonling,
                    MobileTypes.Lich,
                },
            },

            // Coven - Index7
            new RandomEncounterTable()
            {
                DungeonType = DFRegion.DungeonTypes.Coven,
                Enemies = new MobileTypes[]
                {
                    MobileTypes.Imp,
                    MobileTypes.GiantBat,
                    MobileTypes.Mage,
                    MobileTypes.Sorcerer,
                    MobileTypes.Harpy,
                    MobileTypes.Nightblade,
                    MobileTypes.Nymph,
                    MobileTypes.FleshAtronach,
                    MobileTypes.FireAtronach,
                    MobileTypes.IronAtronach,
                    MobileTypes.Nymph,
                    MobileTypes.Daedroth,
                    MobileTypes.Sorcerer,
                    MobileTypes.Spellsword,
                    MobileTypes.DaedraSeducer,
                    MobileTypes.FireDaedra,
                    MobileTypes.Battlemage,
                    MobileTypes.Mage,
                    MobileTypes.FrostDaedra,
                    MobileTypes.DaedraLord,
                },
            },

            // Vampire Haunt - Index8
            new RandomEncounterTable()
            {
                DungeonType = DFRegion.DungeonTypes.VampireHaunt,
                Enemies = new MobileTypes[]
                {
                    MobileTypes.Rat,
                    MobileTypes.Rat,
                    MobileTypes.GiantBat,
                    MobileTypes.Spider,
                    MobileTypes.Werewolf,
                    MobileTypes.Nightblade,
                    MobileTypes.SkeletalWarrior,
                    MobileTypes.SkeletalWarrior,
                    MobileTypes.Ghost,
                    MobileTypes.Mummy,
                    MobileTypes.Wraith,
                    MobileTypes.Ghost,
                    MobileTypes.Vampire,
                    MobileTypes.Wraith,
                    MobileTypes.Vampire,
                    MobileTypes.Vampire,
                    MobileTypes.VampireAncient,
                    MobileTypes.Vampire,
                    MobileTypes.VampireAncient,
                    MobileTypes.VampireAncient,
                },
            },

            // Laboratory - Index9
            new RandomEncounterTable()
            {
                DungeonType = DFRegion.DungeonTypes.Laboratory,
                Enemies = new MobileTypes[]
                {
                    MobileTypes.Imp,
                    MobileTypes.Mage,
                    MobileTypes.Sorcerer,
                    MobileTypes.Battlemage,
                    MobileTypes.Zombie,
                    MobileTypes.Mage,
                    MobileTypes.Sorcerer,
                    MobileTypes.FleshAtronach,
                    MobileTypes.Battlemage,
                    MobileTypes.IceAtronach,
                    MobileTypes.FireAtronach,
                    MobileTypes.IronAtronach,
                    MobileTypes.Gargoyle,
                    MobileTypes.Daedroth,
                    MobileTypes.Sorcerer,
                    MobileTypes.Lich,
                    MobileTypes.Mage,
                    MobileTypes.Sorcerer,
                    MobileTypes.Lich,
                    MobileTypes.AncientLich,
                },
            },

            // Harpy Nest - Index10
            new RandomEncounterTable()
            {
                DungeonType = DFRegion.DungeonTypes.HarpyNest,
                Enemies = new MobileTypes[]
                {
                    MobileTypes.Rat,
                    MobileTypes.Warrior,
                    MobileTypes.GiantBat,
                    MobileTypes.Harpy,
                    MobileTypes.GiantBat,
                    MobileTypes.Harpy,
                    MobileTypes.SkeletalWarrior,
                    MobileTypes.Spider,
                    MobileTypes.Burglar,
                    MobileTypes.Harpy,
                    MobileTypes.GiantScorpion,
                    MobileTypes.OrcShaman,
                    MobileTypes.Nightblade,
                    MobileTypes.Harpy,
                    MobileTypes.Rogue,
                    MobileTypes.Vampire,
                    MobileTypes.DaedraSeducer,
                    MobileTypes.Harpy,
                    MobileTypes.DaedraSeducer,
                    MobileTypes.DaedraLord,
                },
            },

            // Ruined Castle - Index11
            new RandomEncounterTable()
            {
                DungeonType = DFRegion.DungeonTypes.RuinedCastle,
                Enemies = new MobileTypes[]
                {
                    MobileTypes.Rat,
                    MobileTypes.GiantBat,
                    MobileTypes.Warrior,
                    MobileTypes.Orc,
                    MobileTypes.SkeletalWarrior,
                    MobileTypes.Spider,
                    MobileTypes.OrcSergeant,
                    MobileTypes.Werewolf,
                    MobileTypes.Knight,
                    MobileTypes.Wereboar,
                    MobileTypes.Zombie,
                    MobileTypes.Giant,
                    MobileTypes.Knight,
                    MobileTypes.Ghost,
                    MobileTypes.Wraith,
                    MobileTypes.Warrior,
                    MobileTypes.Knight,
                    MobileTypes.Lich,
                    MobileTypes.AncientLich,
                    MobileTypes.VampireAncient,
                },
            },

            // Spider Nest - Index12
            new RandomEncounterTable()
            {
                DungeonType = DFRegion.DungeonTypes.SpiderNest,
                Enemies = new MobileTypes[]
                {
                    MobileTypes.Rat,
                    MobileTypes.GiantBat,
                    MobileTypes.GiantBat,
                    MobileTypes.Spider,
                    MobileTypes.SkeletalWarrior,
                    MobileTypes.Spriggan,
                    MobileTypes.Ranger,
                    MobileTypes.Spider,
                    MobileTypes.GiantScorpion,
                    MobileTypes.Rogue,
                    MobileTypes.Harpy,
                    MobileTypes.Spider,
                    MobileTypes.Thief,
                    MobileTypes.Ghost,
                    MobileTypes.Mummy,
                    MobileTypes.Spider,
                    MobileTypes.Wraith,
                    MobileTypes.Spider,
                    MobileTypes.Assassin,
                    MobileTypes.Lich,
                },
            },

            // Giant Stronghold - Index13
            new RandomEncounterTable()
            {
                DungeonType = DFRegion.DungeonTypes.GiantStronghold,
                Enemies = new MobileTypes[]
                {
                    MobileTypes.Rat,
                    MobileTypes.GrizzlyBear,
                    MobileTypes.SabertoothTiger,
                    MobileTypes.Orc,
                    MobileTypes.Giant,
                    MobileTypes.OrcSergeant,
                    MobileTypes.GiantScorpion,
                    MobileTypes.Giant,
                    MobileTypes.Wereboar,
                    MobileTypes.Giant,
                    MobileTypes.OrcWarlord,
                    MobileTypes.GiantScorpion,
                    MobileTypes.OrcShaman,
                    MobileTypes.Giant,
                    MobileTypes.Daedroth,
                    MobileTypes.FireDaedra,
                    MobileTypes.Dragonling,
                    MobileTypes.Giant,
                    MobileTypes.OrcWarlord,
                    MobileTypes.FrostDaedra,
                },
            },

            // Dragon's Den - Index14
            new RandomEncounterTable()
            {
                DungeonType = DFRegion.DungeonTypes.DragonsDen,
                Enemies = new MobileTypes[]
                {
                    MobileTypes.Rat,
                    MobileTypes.GiantBat,
                    MobileTypes.Centaur,
                    MobileTypes.Burglar,
                    MobileTypes.GiantScorpion,
                    MobileTypes.Werewolf,
                    MobileTypes.Knight,
                    MobileTypes.Harpy,
                    MobileTypes.Spider,
                    MobileTypes.Gargoyle,
                    MobileTypes.Sorcerer,
                    MobileTypes.Nymph,
                    MobileTypes.Nightblade,
                    MobileTypes.Mage,
                    MobileTypes.Knight,
                    MobileTypes.Vampire,
                    MobileTypes.Battlemage,
                    MobileTypes.Thief,
                    MobileTypes.AncientLich,
                    MobileTypes.DaedraLord,
                },
            },

            // Barbarian Stronghold - Index15
            new RandomEncounterTable()
            {
                DungeonType = DFRegion.DungeonTypes.BarbarianStronghold,
                Enemies = new MobileTypes[]
                {
                    MobileTypes.Warrior,
                    MobileTypes.Barbarian,
                    MobileTypes.Rat,
                    MobileTypes.GiantBat,
                    MobileTypes.Warrior,
                    MobileTypes.Centaur,
                    MobileTypes.Rogue,
                    MobileTypes.Archer,
                    MobileTypes.Werewolf,
                    MobileTypes.Barbarian,
                    MobileTypes.Archer,
                    MobileTypes.Wereboar,
                    MobileTypes.Spider,
                    MobileTypes.Warrior,
                    MobileTypes.Vampire,
                    MobileTypes.Rogue,
                    MobileTypes.Barbarian,
                    MobileTypes.VampireAncient,
                    MobileTypes.Sorcerer,
                    MobileTypes.AncientLich,
                },
            },

            // Volcanic Caves - Index16
            new RandomEncounterTable()
            {
                DungeonType = DFRegion.DungeonTypes.VolcanicCaves,
                Enemies = new MobileTypes[]
                {
                    MobileTypes.GiantBat,
                    MobileTypes.Imp,
                    MobileTypes.SabertoothTiger,
                    MobileTypes.SabertoothTiger,
                    MobileTypes.Barbarian,
                    MobileTypes.FireAtronach,
                    MobileTypes.Harpy,
                    MobileTypes.Wereboar,
                    MobileTypes.Giant,
                    MobileTypes.Ghost,
                    MobileTypes.GiantScorpion,
                    MobileTypes.IronAtronach,
                    MobileTypes.FireAtronach,
                    MobileTypes.Mage,
                    MobileTypes.IronAtronach,
                    MobileTypes.Wraith,
                    MobileTypes.Daedroth,
                    MobileTypes.FireDaedra,
                    MobileTypes.DaedraLord,
                    MobileTypes.AncientLich,
                },
            },

            // Scorpion Nest - Index17
            new RandomEncounterTable()
            {
                DungeonType = DFRegion.DungeonTypes.ScorpionNest,
                Enemies = new MobileTypes[]
                {
                    MobileTypes.Rat,
                    MobileTypes.GiantBat,
                    MobileTypes.Spider,
                    MobileTypes.SkeletalWarrior,
                    MobileTypes.GiantScorpion,
                    MobileTypes.Thief,
                    MobileTypes.GiantScorpion,
                    MobileTypes.Zombie,
                    MobileTypes.Imp,
                    MobileTypes.Ghost,
                    MobileTypes.GiantScorpion,
                    MobileTypes.Gargoyle,
                    MobileTypes.Wraith,
                    MobileTypes.Healer,
                    MobileTypes.GiantScorpion,
                    MobileTypes.Nightblade,
                    MobileTypes.DaedraSeducer,
                    MobileTypes.GiantScorpion,
                    MobileTypes.Daedroth,
                    MobileTypes.DaedraLord,
                },
            },

            // Cemetery - Index18
            new RandomEncounterTable()
            {
                Enemies = new MobileTypes[]
                {
                    MobileTypes.Rat,
                    MobileTypes.Rat,
                    MobileTypes.GiantBat,
                    MobileTypes.Rat,
                    MobileTypes.Thief,
                    MobileTypes.GiantBat,
                    MobileTypes.SkeletalWarrior,
                    MobileTypes.GiantBat,
                    MobileTypes.Orc,
                    MobileTypes.Archer,
                    MobileTypes.Orc,
                    MobileTypes.Imp,
                    MobileTypes.Orc,
                    MobileTypes.Imp,
                    MobileTypes.Zombie,
                    MobileTypes.Mummy,
                    MobileTypes.Bard,
                    MobileTypes.Rogue,
                    MobileTypes.Rat,
                    MobileTypes.Rat,
                },
            },

            /*
            // Cemetery - DF Unity version
            new RandomEncounterTable()
            {
                DungeonType = DFRegion.DungeonTypes.Cemetery,
                Enemies = new MobileTypes[]
                {
                    MobileTypes.Rat,
                    MobileTypes.SkeletalWarrior,
                    MobileTypes.GiantBat,
                    MobileTypes.Mummy,
                    MobileTypes.Spider,
                    MobileTypes.Zombie,
                    MobileTypes.Ghost,
                    MobileTypes.Wraith,
                    MobileTypes.Vampire,
                    MobileTypes.VampireAncient,
                    MobileTypes.Lich,
                },
            },*/

            // Underwater - Index19
            new RandomEncounterTable()
            {
                Enemies = new MobileTypes[]
                {
                    MobileTypes.Slaughterfish,
                    MobileTypes.Slaughterfish,
                    MobileTypes.SkeletalWarrior,
                    MobileTypes.Slaughterfish,
                    MobileTypes.Slaughterfish,
                    MobileTypes.Dreugh,
                    MobileTypes.Dreugh,
                    MobileTypes.Slaughterfish,
                    MobileTypes.Zombie,
                    MobileTypes.Dreugh,
                    MobileTypes.Slaughterfish,
                    MobileTypes.IceAtronach,
                    MobileTypes.Slaughterfish,
                    MobileTypes.Dreugh,
                    MobileTypes.Lamia,
                    MobileTypes.Dreugh,
                    MobileTypes.Lamia,
                    MobileTypes.Zombie,
                    MobileTypes.Wraith,
                    MobileTypes.Ghost,
                },
            },

            // Desert, in location, night - Index20
            new RandomEncounterTable()
            {
                Enemies = new MobileTypes[]
                {
                    MobileTypes.Rat,
                    MobileTypes.GiantBat,
                    MobileTypes.Thief,
                    MobileTypes.Assassin,
                    MobileTypes.Werewolf,
                    MobileTypes.Rogue,
                    MobileTypes.Wereboar,
                    MobileTypes.Warrior,
                    MobileTypes.Thief,
                    MobileTypes.Acrobat,
                    MobileTypes.Rogue,
                    MobileTypes.Burglar,
                    MobileTypes.Battlemage,
                    MobileTypes.Mage,
                    MobileTypes.Warrior,
                    MobileTypes.Thief,
                    MobileTypes.Orc,
                    MobileTypes.Vampire,
                    MobileTypes.GiantScorpion,
                    MobileTypes.Assassin,
                },
            },

            // Desert, not in location, day - Index21
            new RandomEncounterTable()
            {
                Enemies = new MobileTypes[]
                {
                    MobileTypes.Rat,
                    MobileTypes.SabertoothTiger,
                    MobileTypes.SabertoothTiger,
                    MobileTypes.Centaur,
                    MobileTypes.Nymph,
                    MobileTypes.Dragonling,
                    MobileTypes.Harpy,
                    MobileTypes.Giant,
                    MobileTypes.Rogue,
                    MobileTypes.GiantScorpion,
                    MobileTypes.OrcShaman,
                    MobileTypes.Assassin,
                    MobileTypes.Thief,
                    MobileTypes.Warrior,
                    MobileTypes.OrcWarlord,
                    MobileTypes.Spellsword,
                    MobileTypes.Bard,
                    MobileTypes.Rogue,
                    MobileTypes.Assassin,
                    MobileTypes.Knight,
                },
            },

            // Desert, not in location, night - Index22
            new RandomEncounterTable()
            {
                Enemies = new MobileTypes[]
                {
                    MobileTypes.GiantBat,
                    MobileTypes.GiantBat,
                    MobileTypes.SabertoothTiger,
                    MobileTypes.Spider,
                    MobileTypes.Orc,
                    MobileTypes.Nightblade,
                    MobileTypes.Assassin,
                    MobileTypes.Werewolf,
                    MobileTypes.Harpy,
                    MobileTypes.Wereboar,
                    MobileTypes.Mummy,
                    MobileTypes.GiantScorpion,
                    MobileTypes.FireAtronach,
                    MobileTypes.OrcWarlord,
                    MobileTypes.IronAtronach,
                    MobileTypes.Warrior,
                    MobileTypes.Nightblade,
                    MobileTypes.Vampire,
                    MobileTypes.Knight,
                    MobileTypes.Dragonling,
                },
            },

            // Mountain, in location, night - Index23
            new RandomEncounterTable()
            {
                Enemies = new MobileTypes[]
                {
                    MobileTypes.Rat,
                    MobileTypes.GiantBat,
                    MobileTypes.Rogue,
                    MobileTypes.Orc,
                    MobileTypes.Assassin,
                    MobileTypes.Spellsword,
                    MobileTypes.Werewolf,
                    MobileTypes.Knight,
                    MobileTypes.Wereboar,
                    MobileTypes.Warrior,
                    MobileTypes.Mage,
                    MobileTypes.Sorcerer,
                    MobileTypes.Burglar,
                    MobileTypes.Thief,
                    MobileTypes.Monk,
                    MobileTypes.Archer,
                    MobileTypes.Warrior,
                    MobileTypes.Barbarian,
                    MobileTypes.Rogue,
                    MobileTypes.Assassin,
                },
            },

            // Mountain, not in location, day - Index24
            new RandomEncounterTable()
            {
                Enemies = new MobileTypes[]
                {
                    MobileTypes.GrizzlyBear,
                    MobileTypes.Spriggan,
                    MobileTypes.Rogue,
                    MobileTypes.Orc,
                    MobileTypes.Nymph,
                    MobileTypes.Harpy,
                    MobileTypes.OrcSergeant,
                    MobileTypes.Giant,
                    MobileTypes.Knight,
                    MobileTypes.OrcShaman,
                    MobileTypes.Monk,
                    MobileTypes.Archer,
                    MobileTypes.Barbarian,
                    MobileTypes.OrcWarlord,
                    MobileTypes.IronAtronach,
                    MobileTypes.Rogue,
                    MobileTypes.Ranger,
                    MobileTypes.Vampire,
                    MobileTypes.Knight,
                    MobileTypes.Assassin,
                },
            },

            // Mountain, not in location, night - Index25
            new RandomEncounterTable()
            {
                Enemies = new MobileTypes[]
                {
                    MobileTypes.GiantBat,
                    MobileTypes.GrizzlyBear,
                    MobileTypes.Spriggan,
                    MobileTypes.Centaur,
                    MobileTypes.Assassin,
                    MobileTypes.Ranger,
                    MobileTypes.Werewolf,
                    MobileTypes.Dragonling,
                    MobileTypes.OrcSergeant,
                    MobileTypes.Giant,
                    MobileTypes.Ghost,
                    MobileTypes.OrcShaman,
                    MobileTypes.Sorcerer,
                    MobileTypes.Gargoyle,
                    MobileTypes.Battlemage,
                    MobileTypes.Wraith,
                    MobileTypes.Spellsword,
                    MobileTypes.Vampire,
                    MobileTypes.Ranger,
                    MobileTypes.Knight,
                },
            },

            // Rainforest, in location, night - Index26
            new RandomEncounterTable()
            {
                Enemies = new MobileTypes[]
                {
                    MobileTypes.Rat,
                    MobileTypes.Thief,
                    MobileTypes.Rat,
                    MobileTypes.GiantBat,
                    MobileTypes.Nightblade,
                    MobileTypes.GiantBat,
                    MobileTypes.Harpy,
                    MobileTypes.Wereboar,
                    MobileTypes.Bard,
                    MobileTypes.Barbarian,
                    MobileTypes.Rogue,
                    MobileTypes.Thief,
                    MobileTypes.Warrior,
                    MobileTypes.Archer,
                    MobileTypes.Assassin,
                    MobileTypes.Battlemage,
                    MobileTypes.Nightblade,
                    MobileTypes.Vampire,
                    MobileTypes.Warrior,
                    MobileTypes.Knight,
                },
            },

            // Rainforest, not in location, day - Index27
            new RandomEncounterTable()
            {
                Enemies = new MobileTypes[]
                {
                    MobileTypes.Rat,
                    MobileTypes.GrizzlyBear,
                    MobileTypes.SabertoothTiger,
                    MobileTypes.Spider,
                    MobileTypes.Thief,
                    MobileTypes.Harpy,
                    MobileTypes.Rogue,
                    MobileTypes.Giant,
                    MobileTypes.Ranger,
                    MobileTypes.GiantScorpion,
                    MobileTypes.Warrior,
                    MobileTypes.Knight,
                    MobileTypes.Thief,
                    MobileTypes.Barbarian,
                    MobileTypes.Ranger,
                    MobileTypes.Archer,
                    MobileTypes.Acrobat,
                    MobileTypes.Bard,
                    MobileTypes.Mage,
                    MobileTypes.Spellsword,
                },
            },

            // Rainforest, not in location, night - Index28
            new RandomEncounterTable()
            {
                Enemies = new MobileTypes[]
                {
                    MobileTypes.GiantBat,
                    MobileTypes.GiantBat,
                    MobileTypes.SabertoothTiger,
                    MobileTypes.Spider,
                    MobileTypes.Spellsword,
                    MobileTypes.Rogue,
                    MobileTypes.Werewolf,
                    MobileTypes.Harpy,
                    MobileTypes.Wereboar,
                    MobileTypes.Giant,
                    MobileTypes.Ghost,
                    MobileTypes.GiantScorpion,
                    MobileTypes.Werewolf,
                    MobileTypes.SabertoothTiger,
                    MobileTypes.Wraith,
                    MobileTypes.Assassin,
                    MobileTypes.Daedroth,
                    MobileTypes.Nightblade,
                    MobileTypes.Warrior,
                    MobileTypes.Battlemage,
                },
            },

            // Subtropical, in location, night - Index29
            new RandomEncounterTable()
            {
                Enemies = new MobileTypes[]
                {
                    MobileTypes.Rat,
                    MobileTypes.Burglar,
                    MobileTypes.Thief,
                    MobileTypes.Monk,
                    MobileTypes.Sorcerer,
                    MobileTypes.Bard,
                    MobileTypes.Rogue,
                    MobileTypes.Warrior,
                    MobileTypes.Nightblade,
                    MobileTypes.Thief,
                    MobileTypes.Acrobat,
                    MobileTypes.Ranger,
                    MobileTypes.Archer,
                    MobileTypes.Vampire,
                    MobileTypes.Barbarian,
                    MobileTypes.Warrior,
                    MobileTypes.Mage,
                    MobileTypes.Spellsword,
                    MobileTypes.Assassin,
                    MobileTypes.Knight,
                },
            },

            // Subtropical, not in location, day - Index30
            new RandomEncounterTable()
            {
                Enemies = new MobileTypes[]
                {
                    MobileTypes.Rat,
                    MobileTypes.Imp,
                    MobileTypes.GrizzlyBear,
                    MobileTypes.SabertoothTiger,
                    MobileTypes.Centaur,
                    MobileTypes.Nymph,
                    MobileTypes.Dragonling,
                    MobileTypes.Giant,
                    MobileTypes.Thief,
                    MobileTypes.GiantScorpion,
                    MobileTypes.Warrior,
                    MobileTypes.Sorcerer,
                    MobileTypes.Nightblade,
                    MobileTypes.Rogue,
                    MobileTypes.Knight,
                    MobileTypes.Battlemage,
                    MobileTypes.Monk,
                    MobileTypes.Thief,
                    MobileTypes.Barbarian,
                    MobileTypes.Nightblade,
                },
            },

            // Subtropical, not in location, night - Index31
            new RandomEncounterTable()
            {
                Enemies = new MobileTypes[]
                {
                    MobileTypes.GiantBat,
                    MobileTypes.GrizzlyBear,
                    MobileTypes.SabertoothTiger,
                    MobileTypes.Spider,
                    MobileTypes.Zombie,
                    MobileTypes.Werewolf,
                    MobileTypes.Wereboar,
                    MobileTypes.Harpy,
                    MobileTypes.Ghost,
                    MobileTypes.GiantScorpion,
                    MobileTypes.Warrior,
                    MobileTypes.Nightblade,
                    MobileTypes.GrizzlyBear,
                    MobileTypes.Barbarian,
                    MobileTypes.Assassin,
                    MobileTypes.Battlemage,
                    MobileTypes.Bard,
                    MobileTypes.Nightblade,
                    MobileTypes.Lich,
                    MobileTypes.Assassin,
                },
            },

            // Swamp/woodlands, in location, night - Index32
            new RandomEncounterTable()
            {
                Enemies = new MobileTypes[]
                {
                    MobileTypes.Rat,
                    MobileTypes.GiantBat,
                    MobileTypes.Burglar,
                    MobileTypes.Bard,
                    MobileTypes.Rogue,
                    MobileTypes.Archer,
                    MobileTypes.Werewolf,
                    MobileTypes.Acrobat,
                    MobileTypes.Wereboar,
                    MobileTypes.Warrior,
                    MobileTypes.Battlemage,
                    MobileTypes.Burglar,
                    MobileTypes.Monk,
                    MobileTypes.Rogue,
                    MobileTypes.Nightblade,
                    MobileTypes.Assassin,
                    MobileTypes.Thief,
                    MobileTypes.Vampire,
                    MobileTypes.Knight,
                    MobileTypes.Battlemage,
                },
            },

            // Swamp/woodlands, not in location, day - Index33
            new RandomEncounterTable()
            {
                Enemies = new MobileTypes[]
                {
                    MobileTypes.Rat,
                    MobileTypes.GrizzlyBear,
                    MobileTypes.Spriggan,
                    MobileTypes.Orc,
                    MobileTypes.Centaur,
                    MobileTypes.Dragonling,
                    MobileTypes.OrcSergeant,
                    MobileTypes.Wereboar,
                    MobileTypes.Giant,
                    MobileTypes.Archer,
                    MobileTypes.OrcShaman,
                    MobileTypes.Bard,
                    MobileTypes.Barbarian,
                    MobileTypes.OrcWarlord,
                    MobileTypes.Battlemage,
                    MobileTypes.Assassin,
                    MobileTypes.Ranger,
                    MobileTypes.Rogue,
                    MobileTypes.Knight,
                    MobileTypes.Spellsword,
                },
            },

            // Swamp/woodlands, not in location, night - Index34
            new RandomEncounterTable()
            {
                Enemies = new MobileTypes[]
                {
                    MobileTypes.GiantBat,
                    MobileTypes.GrizzlyBear,
                    MobileTypes.Spider,
                    MobileTypes.Zombie,
                    MobileTypes.Nightblade,
                    MobileTypes.Knight,
                    MobileTypes.Werewolf,
                    MobileTypes.Dragonling,
                    MobileTypes.Wereboar,
                    MobileTypes.Giant,
                    MobileTypes.Ranger,
                    MobileTypes.Gargoyle,
                    MobileTypes.Spellsword,
                    MobileTypes.Monk,
                    MobileTypes.Bard,
                    MobileTypes.Rogue,
                    MobileTypes.Ranger,
                    MobileTypes.Nightblade,
                    MobileTypes.Knight,
                    MobileTypes.Sorcerer,
                },
            },

            // Haunted woodlands, in location, night - Index35
            new RandomEncounterTable()
            {
                Enemies = new MobileTypes[]
                {
                    MobileTypes.GiantBat,
                    MobileTypes.Nightblade,
                    MobileTypes.Burglar,
                    MobileTypes.Zombie,
                    MobileTypes.Rogue,
                    MobileTypes.Monk,
                    MobileTypes.Wereboar,
                    MobileTypes.Barbarian,
                    MobileTypes.Ghost,
                    MobileTypes.Sorcerer,
                    MobileTypes.Warrior,
                    MobileTypes.Assassin,
                    MobileTypes.Wraith,
                    MobileTypes.Knight,
                    MobileTypes.Battlemage,
                    MobileTypes.Thief,
                    MobileTypes.Rogue,
                    MobileTypes.Vampire,
                    MobileTypes.Barbarian,
                    MobileTypes.VampireAncient,
                },
            },

            // Haunted woodlands, not in location, day - Index36
            new RandomEncounterTable()
            {
                Enemies = new MobileTypes[]
                {
                    MobileTypes.Imp,
                    MobileTypes.GrizzlyBear,
                    MobileTypes.Spriggan,
                    MobileTypes.Spider,
                    MobileTypes.Centaur,
                    MobileTypes.Nymph,
                    MobileTypes.Dragonling,
                    MobileTypes.Harpy,
                    MobileTypes.Giant,
                    MobileTypes.Sorcerer,
                    MobileTypes.OrcShaman,
                    MobileTypes.Gargoyle,
                    MobileTypes.Knight,
                    MobileTypes.OrcWarlord,
                    MobileTypes.IronAtronach,
                    MobileTypes.Battlemage,
                    MobileTypes.Assassin,
                    MobileTypes.Archer,
                    MobileTypes.Ranger,
                    MobileTypes.Knight,
                },
            },

            // Haunted woodlands, not in location, night - Index37
            new RandomEncounterTable()
            {
                Enemies = new MobileTypes[]
                {
                    MobileTypes.GiantBat,
                    MobileTypes.GrizzlyBear,
                    MobileTypes.Spider,
                    MobileTypes.Zombie,
                    MobileTypes.SkeletalWarrior,
                    MobileTypes.Werewolf,
                    MobileTypes.Dragonling,
                    MobileTypes.Wereboar,
                    MobileTypes.Giant,
                    MobileTypes.FleshAtronach,
                    MobileTypes.Ghost,
                    MobileTypes.FireAtronach,
                    MobileTypes.Spider,
                    MobileTypes.Wraith,
                    MobileTypes.FrostDaedra,
                    MobileTypes.IronAtronach,
                    MobileTypes.Daedroth,
                    MobileTypes.Vampire,
                    MobileTypes.VampireAncient,
                    MobileTypes.Lich,
                },
            },

            // Unused - Index38
            new RandomEncounterTable()
            {
                Enemies = new MobileTypes[]
                {
                    MobileTypes.Rat,
                    MobileTypes.Rat,
                    MobileTypes.Thief,
                    MobileTypes.Rat,
                    MobileTypes.Rat,
                    MobileTypes.GiantBat,
                    MobileTypes.GiantBat,
                    MobileTypes.Zombie,
                    MobileTypes.Ghost,
                    MobileTypes.Rat,
                    MobileTypes.Assassin,
                    MobileTypes.GiantBat,
                    MobileTypes.Rogue,
                    MobileTypes.Rat,
                    MobileTypes.GiantBat,
                    MobileTypes.Rat,
                    MobileTypes.GiantBat,
                    MobileTypes.Rat,
                    MobileTypes.Rat,
                    MobileTypes.Vampire,
                },
            },

            // Default building - Index39
            new RandomEncounterTable()
            {
                Enemies = new MobileTypes[]
                {
                    MobileTypes.Thief,
                    MobileTypes.Warrior,
                    MobileTypes.Burglar,
                    MobileTypes.Warrior,
                    MobileTypes.Burglar,
                    MobileTypes.Bard,
                    MobileTypes.Warrior,
                    MobileTypes.Acrobat,
                    MobileTypes.Burglar,
                    MobileTypes.Rogue,
                    MobileTypes.Thief,
                    MobileTypes.Warrior,
                    MobileTypes.Burglar,
                    MobileTypes.Nightblade,
                    MobileTypes.Rogue,
                    MobileTypes.Warrior,
                    MobileTypes.Burglar,
                    MobileTypes.Thief,
                    MobileTypes.Warrior,
                    MobileTypes.Rogue,
                },
            },

            // Guildhall - Index40
            new RandomEncounterTable()
            {
                Enemies = new MobileTypes[]
                {
                    MobileTypes.Mage,
                    MobileTypes.Imp,
                    MobileTypes.Battlemage,
                    MobileTypes.Healer,
                    MobileTypes.Nightblade,
                    MobileTypes.Spellsword,
                    MobileTypes.Mage,
                    MobileTypes.Sorcerer,
                    MobileTypes.Battlemage,
                    MobileTypes.Healer,
                    MobileTypes.FireAtronach,
                    MobileTypes.Spellsword,
                    MobileTypes.Mage,
                    MobileTypes.Sorcerer,
                    MobileTypes.Battlemage,
                    MobileTypes.Nightblade,
                    MobileTypes.Spellsword,
                    MobileTypes.Mage,
                    MobileTypes.DaedraSeducer,
                    MobileTypes.Battlemage,
                },
            },

            // Temple - Index41
            new RandomEncounterTable()
            {
                Enemies = new MobileTypes[]
                {
                    MobileTypes.Monk,
                    MobileTypes.Monk,
                    MobileTypes.Knight,
                    MobileTypes.Monk,
                    MobileTypes.Monk,
                    MobileTypes.Monk,
                    MobileTypes.Monk,
                    MobileTypes.Knight,
                    MobileTypes.Monk,
                    MobileTypes.Monk,
                    MobileTypes.Monk,
                    MobileTypes.Monk,
                    MobileTypes.Monk,
                    MobileTypes.Knight,
                    MobileTypes.Monk,
                    MobileTypes.Monk,
                    MobileTypes.Monk,
                    MobileTypes.Monk,
                    MobileTypes.Monk,
                    MobileTypes.Knight,
                },
            },

            // Palace, House1 - Index42
            new RandomEncounterTable()
            {
                Enemies = new MobileTypes[]
                {
                    MobileTypes.Warrior,
                    MobileTypes.Archer,
                    MobileTypes.Bard,
                    MobileTypes.Spellsword,
                    MobileTypes.Knight,
                    MobileTypes.Warrior,
                    MobileTypes.Knight,
                    MobileTypes.Warrior,
                    MobileTypes.Archer,
                    MobileTypes.Bard,
                    MobileTypes.Spellsword,
                    MobileTypes.Knight,
                    MobileTypes.Warrior,
                    MobileTypes.Knight,
                    MobileTypes.Warrior,
                    MobileTypes.Archer,
                    MobileTypes.Spellsword,
                    MobileTypes.Knight,
                    MobileTypes.Warrior,
                    MobileTypes.Knight,
                },
            },

            // House2 - Index43
            new RandomEncounterTable()
            {
                Enemies = new MobileTypes[]
                {
                    MobileTypes.Bard,
                    MobileTypes.Warrior,
                    MobileTypes.Rogue,
                    MobileTypes.Thief,
                    MobileTypes.Warrior,
                    MobileTypes.Spellsword,
                    MobileTypes.Burglar,
                    MobileTypes.Rogue,
                    MobileTypes.Monk,
                    MobileTypes.Mage,
                    MobileTypes.Nightblade,
                    MobileTypes.Acrobat,
                    MobileTypes.Warrior,
                    MobileTypes.Bard,
                    MobileTypes.Healer,
                    MobileTypes.Sorcerer,
                    MobileTypes.Thief,
                    MobileTypes.Vampire,
                    MobileTypes.Rogue,
                    MobileTypes.Warrior,
                },
            },

            // House3 - Index44
            new RandomEncounterTable()
            {
                Enemies = new MobileTypes[]
                {
                    MobileTypes.Rat,
                    MobileTypes.Rat,
                    MobileTypes.Burglar,
                    MobileTypes.Thief,
                    MobileTypes.Bard,
                    MobileTypes.Monk,
                    MobileTypes.Bard,
                    MobileTypes.Healer,
                    MobileTypes.Rogue,
                    MobileTypes.Monk,
                    MobileTypes.Burglar,
                    MobileTypes.Thief,
                    MobileTypes.Bard,
                    MobileTypes.Rogue,
                    MobileTypes.Ranger,
                    MobileTypes.Burglar,
                    MobileTypes.Thief,
                    MobileTypes.Bard,
                    MobileTypes.Monk,
                    MobileTypes.Rogue,
                },
            },
        };
        #endregion

        #region Public methods

        // Enemy selection method from classic. Returns an enemy ID based on environment and player level.
        public static MobileTypes ChooseRandomEnemy(bool chooseUnderWaterEnemy)
        {
            int encounterTableIndex = 0;
            Game.PlayerEnterExit playerEnterExit = Game.GameManager.Instance.PlayerEnterExit;
            PlayerGPS playerGPS = Game.GameManager.Instance.PlayerGPS;

            if (!playerEnterExit || !playerGPS)
                return MobileTypes.None;

            if (chooseUnderWaterEnemy)
                encounterTableIndex = 19;
            else if (playerEnterExit.IsPlayerInsideDungeon)
                encounterTableIndex = ((int)playerEnterExit.Dungeon.Summary.DungeonType);
            else if (playerEnterExit.IsPlayerInsideBuilding)
            {
                DFLocation.BuildingTypes buildingType = playerEnterExit.BuildingType;

                if (buildingType == DFLocation.BuildingTypes.GuildHall)
                    encounterTableIndex = 40;
                else if (buildingType == DFLocation.BuildingTypes.Temple)
                    encounterTableIndex = 41;
                else if (buildingType == DFLocation.BuildingTypes.Palace
                    || buildingType == DFLocation.BuildingTypes.House1)
                    encounterTableIndex = 42;
                else if (buildingType == DFLocation.BuildingTypes.House2)
                    encounterTableIndex = 43;
                else if (buildingType == DFLocation.BuildingTypes.House3)
                    encounterTableIndex = 44;
                else
                    encounterTableIndex = 39;
            }
            else
            {
                int climate = playerGPS.CurrentClimateIndex;
                bool isDay = DaggerfallUnity.Instance.WorldTime.Now.IsDay;

                if (playerGPS.IsPlayerInLocationRect)
                {
                    if (isDay)
                        return MobileTypes.None;

                    // Player in location rectangle, night
                    switch (climate)
                    {
                        case (int)MapsFile.Climates.Desert:
                        case (int)MapsFile.Climates.Desert2:
                            encounterTableIndex = 20;
                            break;
                        case (int)MapsFile.Climates.Mountain:
                            encounterTableIndex = 23;
                            break;
                        case (int)MapsFile.Climates.Rainforest:
                            encounterTableIndex = 26;
                            break;
                        case (int)MapsFile.Climates.Subtropical:
                            encounterTableIndex = 29;
                            break;
                        case (int)MapsFile.Climates.Swamp:
                        case (int)MapsFile.Climates.MountainWoods:
                        case (int)MapsFile.Climates.Woodlands:
                            encounterTableIndex = 32;
                            break;
                        case (int)MapsFile.Climates.HauntedWoodlands:
                            encounterTableIndex = 35;
                            break;

                        default:
                            return MobileTypes.None;
                    }
                }
                else
                {
                    if (isDay)
                    {
                        // Player not in location rectangle, day
                        switch (climate)
                        {
                            case (int)MapsFile.Climates.Desert:
                            case (int)MapsFile.Climates.Desert2:
                                encounterTableIndex = 21;
                                break;
                            case (int)MapsFile.Climates.Mountain:
                                encounterTableIndex = 24;
                                break;
                            case (int)MapsFile.Climates.Rainforest:
                                encounterTableIndex = 27;
                                break;
                            case (int)MapsFile.Climates.Subtropical:
                                encounterTableIndex = 30;
                                break;
                            case (int)MapsFile.Climates.Swamp:
                            case (int)MapsFile.Climates.MountainWoods:
                            case (int)MapsFile.Climates.Woodlands:
                                encounterTableIndex = 33;
                                break;
                            case (int)MapsFile.Climates.HauntedWoodlands:
                                encounterTableIndex = 36;
                                break;

                            default:
                                return MobileTypes.None;
                        }
                    }
                    else
                    {
                        // Player not in location rectangle, night
                        switch (climate)
                        {
                            case (int)MapsFile.Climates.Desert:
                            case (int)MapsFile.Climates.Desert2:
                                encounterTableIndex = 22;
                                break;
                            case (int)MapsFile.Climates.Mountain:
                                encounterTableIndex = 25;
                                break;
                            case (int)MapsFile.Climates.Rainforest:
                                encounterTableIndex = 28;
                                break;
                            case (int)MapsFile.Climates.Subtropical:
                                encounterTableIndex = 31;
                                break;
                            case (int)MapsFile.Climates.Swamp:
                            case (int)MapsFile.Climates.MountainWoods:
                            case (int)MapsFile.Climates.Woodlands:
                                encounterTableIndex = 34;
                                break;
                            case (int)MapsFile.Climates.HauntedWoodlands:
                                encounterTableIndex = 37;
                                break;

                            default:
                                return MobileTypes.None;
                        }
                    }
                }
            }

            int roll = Dice100.Roll();
            int playerLevel = Game.GameManager.Instance.PlayerEntity.Level;
            int min;
            int max;

            // Random/player level based adjustments from classic. These assume enemy lists of length 20.
            if (roll > 80)
            {
                if (roll > 95)
                {
                    if (playerLevel <= 5)
                    {
                        min = 0;
                        max = playerLevel + 2;
                    }
                    else
                    {
                        min = 0;
                        max = 19;
                    }
                }
                else
                {
                    min = 0;
                    max = playerLevel + 1;
                }
            }
            else
            {
                min = playerLevel - 3;
                max = playerLevel + 3;
            }
            if (min < 0)
            {
                min = 0;
                max = 5;
            }
            if (max > 19)
            {
                min = 14;
                max = 19;
            }

            RandomEncounterTable encounterTable = EncounterTables[encounterTableIndex];

            // Adding a check here (not in classic) for lists of shorter length than 20
            if (max + 1 > encounterTable.Enemies.Length)
            {
                max = encounterTable.Enemies.Length - 1;
                if (max >= 5)
                    min = max - 5;
                else
                    min = UnityEngine.Random.Range(0, max);
            }

            return encounterTable.Enemies[UnityEngine.Random.Range(min, max + 1)];
        }
    }
    #endregion
}
