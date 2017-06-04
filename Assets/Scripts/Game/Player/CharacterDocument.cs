// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2016 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using System;
using System.IO;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game.Player
{
    /// <summary>
    /// A document filled in by character creation or import definining character starting state.
    /// </summary>
    public class CharacterDocument
    {
        public RaceTemplate raceTemplate;
        public Genders gender;
        public DFCareer career;
        public string name;
        public int faceIndex;
        public DaggerfallStats startingStats;
        public DaggerfallStats workingStats;
        public DaggerfallSkills startingSkills;
        public DaggerfallSkills workingSkills;
        public PlayerReflexes reflexes;
        public int currentHealth;
        public int maxHealth;
        public int currentSpellPoints;
        public int currentFatigue;
        public short[] skillUses;
        public int startingLevelUpSkillSum;

        public CharacterDocument()
        {
            SetDefaultValues();
        }

        // Set some default values for testing during development
        void SetDefaultValues()
        {
            raceTemplate = GetRaceTemplate(Races.Breton);
            gender = Genders.Male;
            career = DaggerfallEntity.GetClassCareerTemplate(ClassCareers.Mage);
            name = "Nameless";
            reflexes = PlayerReflexes.Average;
            workingSkills.SetDefaults();
            workingStats.SetFromCareer(career);
            startingLevelUpSkillSum = 0;
            faceIndex = 0;
            skillUses = new short[DaggerfallSkills.Count];
        }

        public static RaceTemplate GetRaceTemplate(Races race)
        {
            switch (race)
            {
                default:
                case Races.Breton:
                    return new Breton();
                case Races.Redguard:
                    return new Redguard();
                case Races.Nord:
                    return new Nord();
                case Races.DarkElf:
                    return new DarkElf();
                case Races.HighElf:
                    return new HighElf();
                case Races.WoodElf:
                    return new WoodElf();
                case Races.Khajiit:
                    return new Khajiit();
                case Races.Argonian:
                    return new Argonian();
            }
        }

        public static DaggerfallStats GetClassBaseStats(DFCareer dfClass)
        {
            DaggerfallStats stats = new DaggerfallStats();

            stats.Strength = dfClass.Strength;
            stats.Intelligence = dfClass.Intelligence;
            stats.Willpower = dfClass.Willpower;
            stats.Agility = dfClass.Agility;
            stats.Endurance = dfClass.Endurance;
            stats.Personality = dfClass.Personality;
            stats.Speed = dfClass.Speed;
            stats.Luck = dfClass.Luck;

            return stats;
        }
    }
}