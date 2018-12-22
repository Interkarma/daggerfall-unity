// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    Numidium
// 
// Notes:
//

using DaggerfallConnect;
using DaggerfallWorkshop.Game.Entity;
using System.Collections.Generic;

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
        public DaggerfallStats startingStats = new DaggerfallStats();
        public DaggerfallStats workingStats = new DaggerfallStats();
        public DaggerfallSkills startingSkills = new DaggerfallSkills();
        public DaggerfallSkills workingSkills = new DaggerfallSkills();
        public PlayerReflexes reflexes;
        public int currentHealth;
        public int maxHealth;
        public int currentSpellPoints;
        public short reputationCommoners;
        public short reputationMerchants;
        public short reputationNobility;
        public short reputationScholars;
        public short reputationUnderworld;
        public int currentFatigue;
        public short[] skillUses;
        public uint skillsRaisedThisLevel1;
        public uint skillsRaisedThisLevel2;
        public int startingLevelUpSkillSum;
        public byte minMetalToHit;
        public sbyte[] armorValues = new sbyte[DaggerfallEntity.NumberBodyParts];
        public uint lastTimePlayerBoughtTraining;
        public uint timeForThievesGuildLetter;
        public uint timeForDarkBrotherhoodLetter;
        public byte vampireClan;
        public byte darkBrotherhoodRequirementTally;
        public byte thievesGuildRequirementTally;
        public uint timeToBecomeVampireOrWerebeast;
        public byte hasStartedInitialVampireQuest;
        public uint lastTimeVampireNeedToKillSatiated;
        public uint lastTimePlayerAteOrDrankAtTavern;
        public sbyte biographyReactionMod;
        public List<string> biographyEffects;
        public int classIndex;
        public List<string> backStory;
        public bool isCustom = false;
        public Races classicTransformedRace = Races.None;

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
            workingStats.SetPermanentFromCareer(career);
            startingLevelUpSkillSum = 0;
            faceIndex = 0;
            skillUses = new short[DaggerfallSkills.Count];
            for (int i = 0; i < armorValues.Length; i++)
            {
                armorValues[i] = 100;
            }
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

            stats.SetPermanentStatValue(DFCareer.Stats.Strength, dfClass.Strength);
            stats.SetPermanentStatValue(DFCareer.Stats.Intelligence, dfClass.Intelligence);
            stats.SetPermanentStatValue(DFCareer.Stats.Willpower, dfClass.Willpower);
            stats.SetPermanentStatValue(DFCareer.Stats.Agility, dfClass.Agility);
            stats.SetPermanentStatValue(DFCareer.Stats.Endurance, dfClass.Endurance);
            stats.SetPermanentStatValue(DFCareer.Stats.Personality, dfClass.Personality);
            stats.SetPermanentStatValue(DFCareer.Stats.Speed, dfClass.Speed);
            stats.SetPermanentStatValue(DFCareer.Stats.Luck, dfClass.Luck);

            return stats;
        }
    }
}
