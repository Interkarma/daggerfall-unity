// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
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

namespace DaggerfallWorkshop.Utility
{
    /// <summary>
    /// Static definitions for random encounters based on dungeon type (from FALL.EXE.
    /// These are also in Daggerfall Chronicles, with almost all duplicate entries removed).
    /// All lists have 20 entries.
    /// These are generally ordered from low-level through to high-level encounters.
    /// There are also additional lists in FALL.EXE not shown here currently.
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

            // Cemetery - Index18 (TODO: Confirm and fill out)
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
            },

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
            }
        };
    }
}
