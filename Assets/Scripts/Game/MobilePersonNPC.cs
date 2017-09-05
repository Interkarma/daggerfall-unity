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
        const int numPersonFaceVariants = 24;

        int[] maleRedguardFaceRecordIndex = new int[] { 336, 312, 336, 312 }; // matching textures 381, 382, 383, 384 from MobilePersonBillboard class texture definition
        int[] femaleRedguardFaceRecordIndex = new int[] { 144, 144, 120, 96 }; // matching texture 395, 396, 397, 398 from MobilePersonBillboard class texture definition

        int[] maleNordFaceRecordIndex = new int[] { 240, 264, 264, 216 }; // matching texture 387, 388, 389, 390 from MobilePersonBillboard class texture definition
        int[] femaleNordFaceRecordIndex = new int[] { 72, 0, 48, 72 }; // matching texture 392, 393, 451, 452 from MobilePersonBillboard class texture definition

        int[] maleBretonFaceRecordIndex = new int[] { 192, 216, 240, 240 }; // matching texture 385, 386, 391, 394 from MobilePersonBillboard class texture definition
        int[] femaleBretonFaceRecordIndex = new int[] { 72, 72, 24, 72 }; // matching texture 453, 454, 455, 456 from MobilePersonBillboard class texture definition

        public Races race = Races.Breton;
        public DisplayRaces displayRace = DisplayRaces.Breton;
        public Genders gender = Genders.Male;
        public string nameNPC;
        public int personOutfitVariant; // which basic outfit does the person wear
        public int personFaceRecordId; // used for portrait in talk window

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
            //SetPerson(race, gender, true);
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
            SetPerson(race, gender, true);
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
            {
                personOutfitVariant = Random.Range(0, numPersonOutfitVariants);                

                // Get person's face texture record index for this race and gender and outfit variant
                int[] recordIndices = null;
                switch (race)
                {
                    case Races.Redguard:
                        recordIndices = (gender == Genders.Male) ? maleRedguardFaceRecordIndex : femaleRedguardFaceRecordIndex;
                        break;
                    case Races.Nord:
                        recordIndices = (gender == Genders.Male) ? maleNordFaceRecordIndex : femaleNordFaceRecordIndex;
                        break;
                    case Races.Breton:
                    default:
                        recordIndices = (gender == Genders.Male) ? maleBretonFaceRecordIndex : femaleBretonFaceRecordIndex;
                        break;
                }
                int personFaceVariant = Random.Range(0, numPersonFaceVariants);
                this.personFaceRecordId = recordIndices[personOutfitVariant] + personFaceVariant;
            }
            this.race = race;
            this.gender = gender;
            this.nameNPC = "dummy name";
        }
    }
}