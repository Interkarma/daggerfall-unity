// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2019 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    Michael Rauter (Nystul)
// 
// Notes:
//

using UnityEngine;
using DaggerfallConnect;
using DaggerfallConnect.Utility;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Utility;

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// This contains the actual NPC data for mobile NPCs.
    /// </summary>
    public class MobilePersonNPC : MonoBehaviour
    {
        #region Fields

        const int numPersonOutfitVariants = 4;  // there are 4 outfit variants per climate 
        const int numPersonFaceVariants = 24;   // there are 24 face variants per outfit variant

        int[] maleRedguardFaceRecordIndex = new int[] { 336, 312, 336, 312 };   // matching textures 381, 382, 383, 384 from MobilePersonBillboard class texture definition
        int[] femaleRedguardFaceRecordIndex = new int[] { 144, 144, 120, 96 };  // matching texture 395, 396, 397, 398 from MobilePersonBillboard class texture definition

        int[] maleNordFaceRecordIndex = new int[] { 240, 264, 264, 216 };       // matching texture 387, 388, 389, 390 from MobilePersonBillboard class texture definition
        int[] femaleNordFaceRecordIndex = new int[] { 72, 0, 48, 72 };          // matching texture 392, 393, 451, 452 from MobilePersonBillboard class texture definition

        int[] maleBretonFaceRecordIndex = new int[] { 192, 216, 240, 240 };     // matching texture 385, 386, 391, 394 from MobilePersonBillboard class texture definition
        int[] femaleBretonFaceRecordIndex = new int[] { 72, 72, 24, 72 };       // matching texture 453, 454, 455, 456 from MobilePersonBillboard class texture definition

        // display races for npcs (only Breton, Redguard and Nord mobile billboards for displaying exist)
        public enum DisplayRaces
        {
            Breton = 1,
            Redguard = 2,
            Nord = 3,
        }

        private Races race = Races.Breton;                      // race of the npc
        private DisplayRaces displayRace = DisplayRaces.Breton; // display race of the npc
        private Genders gender = Genders.Male;                  // gender of the npc
        private string nameNPC;                                 // name of the npc
        private int personOutfitVariant;                        // which basic outfit does the person wear
        private int personFaceRecordId;                         // used for portrait in talk window
        private bool pickpocketByPlayerAttempted = false;       // player can only attempt pickpocket on a mobile NPC once
        private bool isGuard = false;                           // is a city watch guard

        private MobilePersonBillboard billboard;    // billboard for npc
        private MobilePersonMotor motor;            // motor for npc

        // these public fields are used for setting person attributes through Unity Inspector Window with function ApplyPersonSettings
        // (properties not available) as fields are randomized (e.g. name and face variant)
        public Races raceToBeSet = Races.Breton;        // race used for ApplyCustomPersonSettings function (which can be used for testing through Unity Inspector Window)
        public Genders genderToBeSet = Genders.Male;    // gender used for ApplyCustomPersonSettings function (which can be used for testing through Unity Inspector Window)
        [Range(-1, numPersonOutfitVariants)]
        public int outfitVariantToBeSet = -1;           // person outfit variant used for ApplyCustomPersonSettings function (which can be used for testing through Unity Inspector Window)        

        #endregion

        #region Properties

        public Races Race
        {
            get { return (race); }
        }
        
        public DisplayRaces DisplayRace
        {
            get { return (displayRace); }
        }
        
        public Genders Gender
        {
            get { return (gender); }
        }

        public string NameNPC
        {
            get { return (nameNPC); }
        }

        public int PersonOutfitVariant
        {
            get { return (personOutfitVariant); }
        }

        public int PersonFaceRecordId
        {
            get { return (personFaceRecordId); }
        }

        public bool PickpocketByPlayerAttempted
        {
            get { return (pickpocketByPlayerAttempted); }
            set { pickpocketByPlayerAttempted = value; }
        }

        public MobilePersonBillboard Billboard
        {
            get { return (billboard); }
        }

        public MobilePersonMotor Motor
        {
            get { return (motor); }
            set { motor = value; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// randomize NPC with current race - set current race before calling this function with property Race.
        /// </summary>
        /// <param name="race">Entity race of NPC in current location.</param>
        public void RandomiseNPC(Races race)
        {
            // Randomly set guards
            if (Random.Range(0, 32) == 0)
            {
                gender = Genders.Male;
                personOutfitVariant = 0;
                isGuard = true;
            }
            else
            {
                // Randomize gender
                gender = (Random.Range(0, 2) == 1) ? Genders.Female : Genders.Male;
                // Set outfit variant for npc
                personOutfitVariant = Random.Range(0, numPersonOutfitVariants);
                isGuard = false;
            }
            // Set race (set current race before calling this function with property Race)
            SetRace(race);
            // Set remaining fields and update billboards
            SetPerson();
        }

        /// <summary>
        /// apply person settings via Unity Inspector through public fields raceToBeSet, genderToBeSet and outfitVariantToBeSet.
        /// </summary>
        public void ApplyPersonSettingsViaInspector()
        {
            // get gender for npc from inspector value
            this.gender = genderToBeSet;

            // set race for npc from inspector value
            SetRace(raceToBeSet);

            // set outfit variant for npc from inspector value
            if (outfitVariantToBeSet == -1)
                this.personOutfitVariant = Random.Range(0, numPersonOutfitVariants);
            else
                this.personOutfitVariant = outfitVariantToBeSet;

            // set remaining fields and update billboards
            SetPerson();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// used to set remaining fields of person (with random values) and update billboards
        /// this function should be called after race, gender and personOutfitVariant has been set beforehand
        /// </summary>
        void SetPerson()
        {
            // do several things in switch statement:
            // get person's face texture record index for this race and gender and outfit variant
            // get correct nameBankType for this race
            int[] recordIndices = null;
            NameHelper.BankTypes nameBankType;
            switch (race)
            {
                case Races.Redguard:
                    recordIndices = (gender == Genders.Male) ? maleRedguardFaceRecordIndex : femaleRedguardFaceRecordIndex;
                    //nameBankType = NameHelper.BankTypes.Redguard;
                    break;
                case Races.Nord:
                    recordIndices = (gender == Genders.Male) ? maleNordFaceRecordIndex : femaleNordFaceRecordIndex;
                    //nameBankType = NameHelper.BankTypes.Nord;
                    break;
                case Races.Breton:
                default:
                    recordIndices = (gender == Genders.Male) ? maleBretonFaceRecordIndex : femaleBretonFaceRecordIndex;
                    //nameBankType = NameHelper.BankTypes.Breton;
                    break;
            }

            // create name for npc
            DFLocation.ClimateSettings climateSettings = MapsFile.GetWorldClimateSettings(GameManager.Instance.PlayerGPS.ClimateSettings.WorldClimate);
            switch (climateSettings.Names)
            {                
                case FactionFile.FactionRaces.Breton:
                default:
                    nameBankType = NameHelper.BankTypes.Breton;
                    break;
                case FactionFile.FactionRaces.Nord:
                    nameBankType = NameHelper.BankTypes.Nord;
                    break;
                case FactionFile.FactionRaces.Redguard:
                    nameBankType = NameHelper.BankTypes.Redguard;
                    break;
            }
            this.nameNPC = DaggerfallUnity.Instance.NameHelper.FullName(nameBankType, gender);

            // get face record id to use (randomize portrait for current person outfit variant)
            int personFaceVariant = Random.Range(0, numPersonFaceVariants);
            this.personFaceRecordId = recordIndices[personOutfitVariant] + personFaceVariant;

            // set billboard to correct race, gender and outfit variant
            billboard = GetComponentInChildren<MobilePersonBillboard>();
            billboard.SetPerson(race, gender, personOutfitVariant, isGuard);
        }

        /// <summary>
        /// sets person race and display race   
        /// </summary>
        /// <param name="race">race to be set</param>
        void SetRace(Races race)
        {
            // set race
            this.race = race;

            // set display race
            switch (race)
            {
                case Races.Redguard:
                    this.displayRace = DisplayRaces.Redguard;
                    break;
                case Races.Nord:
                    this.displayRace = DisplayRaces.Nord;
                    break;
                case Races.Breton:
                default:
                    this.displayRace = DisplayRaces.Breton;
                    break;
            }
        }

        #endregion
    }
}