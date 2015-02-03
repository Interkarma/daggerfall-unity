// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

using System;
using DaggerfallConnect;

namespace DaggerfallWorkshop.Utility
{
    /// <summary>
    /// Static definitions for random encounters based on dungeon type (from Daggerfall Chronicles).
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
                    MobileTypes.SkeletalWarrior,
                    MobileTypes.SabertoothTiger,
                    MobileTypes.Giant,
                    MobileTypes.OrcShaman,
                    MobileTypes.Spider,
                    MobileTypes.OrcWarlord,
                    MobileTypes.GiantScorpion,
                    MobileTypes.Daedroth,
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
                    MobileTypes.Barbarian,
                    MobileTypes.OrcWarlord,
                    MobileTypes.Wraith,
                    MobileTypes.OrcShaman,
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
                    MobileTypes.Barbarian,
                    MobileTypes.Thief,
                    MobileTypes.FleshAtronach,
                    MobileTypes.Assassin,
                    MobileTypes.Zombie,
                    MobileTypes.IronAtronach,
                    MobileTypes.Wraith,
                    MobileTypes.Ghost,
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
                    MobileTypes.Mummy,
                    MobileTypes.OrcShaman,
                    MobileTypes.Gargoyle,
                    MobileTypes.Wraith,
                    MobileTypes.Daedroth,
                    MobileTypes.DaedraSeducer,
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
                    MobileTypes.Nightblade,
                    MobileTypes.Orc,
                    MobileTypes.OrcSergeant,
                    MobileTypes.Giant,
                    MobileTypes.Thief,
                    MobileTypes.Warrior,
                    MobileTypes.IceAtronach,
                    MobileTypes.IronAtronach,
                    MobileTypes.FireDaedra,
                    MobileTypes.Vampire,
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
                    MobileTypes.Daedroth,
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
                    MobileTypes.GiantBat,
                    MobileTypes.Spider,
                    MobileTypes.Werewolf,
                    MobileTypes.Nightblade,
                    MobileTypes.SkeletalWarrior,
                    MobileTypes.Ghost,
                    MobileTypes.Mummy,
                    MobileTypes.Wraith,
                    MobileTypes.Vampire,
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
                    MobileTypes.FleshAtronach,
                    MobileTypes.IceAtronach,
                    MobileTypes.FireAtronach,
                    MobileTypes.IronAtronach,
                    MobileTypes.Gargoyle,
                    MobileTypes.Daedroth,
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
                    MobileTypes.SkeletalWarrior,
                    MobileTypes.Spider,
                    MobileTypes.Burglar,
                    MobileTypes.GiantScorpion,
                    MobileTypes.OrcShaman,
                    MobileTypes.Nightblade,
                    MobileTypes.Rogue,
                    MobileTypes.Vampire,
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
                    MobileTypes.Ghost,
                    MobileTypes.Wraith,
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
                    MobileTypes.Spider,
                    MobileTypes.SkeletalWarrior,
                    MobileTypes.Spriggan,
                    MobileTypes.Ranger,
                    MobileTypes.GiantScorpion,
                    MobileTypes.Rogue,
                    MobileTypes.Harpy,
                    MobileTypes.Thief,
                    MobileTypes.Ghost,
                    MobileTypes.Mummy,
                    MobileTypes.Wraith,
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
                    MobileTypes.Wereboar,
                    MobileTypes.OrcWarlord,
                    MobileTypes.OrcShaman,
                    MobileTypes.Daedroth,
                    MobileTypes.FireDaedra,
                    MobileTypes.Dragonling,
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
                    MobileTypes.Centaur,
                    MobileTypes.Rogue,
                    MobileTypes.Archer,
                    MobileTypes.Werewolf,
                    MobileTypes.Wereboar,
                    MobileTypes.Spider,
                    MobileTypes.Vampire,
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
                    MobileTypes.Barbarian,
                    MobileTypes.FireAtronach,
                    MobileTypes.Harpy,
                    MobileTypes.Wereboar,
                    MobileTypes.Giant,
                    MobileTypes.Ghost,
                    MobileTypes.GiantScorpion,
                    MobileTypes.IronAtronach,
                    MobileTypes.Mage,
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
                    MobileTypes.Zombie,
                    MobileTypes.Imp,
                    MobileTypes.Ghost,
                    MobileTypes.Gargoyle,
                    MobileTypes.Wraith,
                    MobileTypes.Healer,
                    MobileTypes.Nightblade,
                    MobileTypes.DaedraSeducer,
                    MobileTypes.Daedroth,
                    MobileTypes.DaedraLord,
                },
            },

            // Cemetery - Index18
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
        };
    }
}
