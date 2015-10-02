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

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.Formulas;
using DaggerfallWorkshop.Game.Player;

namespace DaggerfallWorkshop.Game.Entity
{
    /// <summary>
    /// Implements DaggerfallEntity with properties specific to a Player.
    /// </summary>
    public class PlayerEntity : DaggerfallEntity
    {
        #region Fields

        const int testPlayerLevel = 1;
        const string testPlayerName = "Nameless";

        protected RaceTemplate race;
        protected int faceIndex;
        protected PlayerReflexes reflexes;

        #endregion

        #region Properties

        public RaceTemplate Race { get { return race; } set { race = value; } }
        public int FaceIndex { get { return faceIndex; } set { faceIndex = value; } }
        public PlayerReflexes Reflexes { get { return reflexes; } set { reflexes = value; } }

        #endregion

        #region Public Methods

        /// <summary>
        /// Assigns player entity settings from a character sheet.
        /// </summary>
        public void AssignCharacter(CharacterSheet character, int level = 1, int maxHealth = 0)
        {
            this.level = level;
            this.gender = character.gender;
            this.race = character.race;
            this.career = character.career;
            this.name = character.name;
            this.faceIndex = character.faceIndex;
            this.stats = character.workingStats;
            this.skills = character.workingSkills;
            this.reflexes = character.reflexes;

            if (maxHealth <= 0)
                this.maxHealth = FormulaHelper.RollMaxHealth(level, stats.Endurance, career.HitPointsPerLevelOrMonsterLevel);
            else
                this.maxHealth = maxHealth;
        }

        /// <summary>
        /// Assigns default entity settings.
        /// </summary>
        public override void SetEntityDefaults()
        {
            race = CharacterSheet.GetRaceTemplate(Races.Breton);
            faceIndex = 0;
            reflexes = PlayerReflexes.Average;
            gender = Genders.Male;
            career = CharacterSheet.GetCareerTemplate(Careers.Mage);
            level = testPlayerLevel;
            maxHealth = FormulaHelper.RollMaxHealth(level, stats.Endurance, career.HitPointsPerLevelOrMonsterLevel);
            name = testPlayerName;
            stats.SetDefaults();
            skills.SetDefaults();
            FillVitalSigns();
        }

        #endregion
    }
}