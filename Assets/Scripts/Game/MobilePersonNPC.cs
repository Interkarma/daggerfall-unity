// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2017 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// This will contain the actual NPC data for mobile NPCs.
    /// Not fully implemented at this time.
    /// </summary>
    public class MobilePersonNPC : MonoBehaviour
    {
        const int numPersonOutfitVariants = 4;

        public Races race = Races.Breton;
        public DisplayRaces displayRace = DisplayRaces.Breton;
        public Genders gender = Genders.Male;
        public string nameNPC;
        public int personOutfitVariant; // which basic outfit does the person wear
        public int personVariant; // used for portrait in talk window

        public MobilePersonBillboard billboard;
        public MobilePersonMotor motor;

        public enum DisplayRaces
        {
            Breton = 1,
            Redguard = 2,
            Nord = 3,
        }
        
        public void ApplyPersonSettings()
        {
            billboard = GetComponentInChildren<MobilePersonBillboard>();
            motor = GetComponentInChildren<MobilePersonMotor>();
            RandomiseNPC();
            SetPerson(race, gender, true);
            billboard.SetPerson(race, gender, personOutfitVariant);
        }
        

        public MobilePersonBillboard GetMobilePersonBillboardChildScript()
        {
            return GetComponentInChildren<MobilePersonBillboard>();
        }
        
        public MobilePersonMotor GetMobilePersonMotorChildScript()
        {
            return GetComponent<MobilePersonMotor>();
        }

        /// <summary>
        /// Setup a new random NPC inside this motor.
        /// </summary>
        /// <param name="race">Entity race of NPC in current location.</param>
        public void RandomiseNPC()
        {
            Genders gender = (Random.Range(0f, 1f) > 0.5f) ? gender = Genders.Female : gender = Genders.Male;
            SetPerson(race, gender);
        }


        void SetRace(Races race)
        {
            if (race == Races.Redguard || race == Races.Nord || race == Races.Breton)
                this.race = race;
            else
                this.race = Races.Breton;
        }

        /// <summary>
        /// Setup this person based on race and gender.
        /// </summary>
        public void SetPerson(Races race, Genders gender, bool newVariant = false)
        {
            // Allow for new random variant if specified
            if (newVariant)
                personOutfitVariant = -1;

            // Store values
            this.race = race;
            this.gender = gender;

            // Set texture archive at random if not already set
            if (personOutfitVariant == -1)
                personOutfitVariant = Random.Range(0, numPersonOutfitVariants);
        }
    }
}