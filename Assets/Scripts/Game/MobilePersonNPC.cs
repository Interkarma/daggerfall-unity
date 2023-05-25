// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
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

        int[] maleNordFaceRecordIndex = new int[] { 240, 264, 168, 192 };       // matching texture 387, 388, 389, 390 from MobilePersonBillboard class texture definition
        int[] femaleNordFaceRecordIndex = new int[] { 72, 0, 48, 0 };          // matching texture 392, 393, 451, 452 from MobilePersonBillboard class texture definition

        int[] maleBretonFaceRecordIndex = new int[] { 192, 216, 288, 240 };     // matching texture 385, 386, 391, 394 from MobilePersonBillboard class texture definition
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
        private MobilePersonMotor motor;                        // motor for npc

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
            set { race = value; }
        }
        
        public DisplayRaces DisplayRace
        {
            get { return (displayRace); }
        }
        
        public Genders Gender
        {
            get { return (gender); }
            set { gender = value; }
        }

        public string NameNPC
        {
            get { return (nameNPC); }
            set { nameNPC = value; }
        }

        public int PersonOutfitVariant
        {
            get { return (personOutfitVariant); }
            set { personOutfitVariant = value; }
        }

        public int PersonFaceRecordId
        {
            get { return (personFaceRecordId); }
            set { personFaceRecordId = value; }
        }

        public bool PickpocketByPlayerAttempted
        {
            get { return (pickpocketByPlayerAttempted); }
            set { pickpocketByPlayerAttempted = value; }
        }

        /// <summary>
        /// True if this npc is a city watch guard.
        /// </summary>
        public bool IsGuard { get; set; }

        /// <summary>
        /// Billboard or custom asset for npc.
        /// </summary>
        public MobilePersonAsset Asset { get; set; }

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
                IsGuard = true;
            }
            else
            {
                // Randomize gender
                gender = (Random.Range(0, 2) == 1) ? Genders.Female : Genders.Male;
                // Set outfit variant for npc
                personOutfitVariant = Random.Range(0, numPersonOutfitVariants);
                IsGuard = false;
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
            // get person's face texture record index for this race and gender and outfit variant
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

            // get correct nameBankType for this race and create name for npc
            NameHelper.BankTypes nameBankType = NameHelper.BankTypes.Breton;
            if (GameManager.Instance.PlayerGPS.CurrentRegionIndex > -1)
                nameBankType = (NameHelper.BankTypes) MapsFile.RegionRaces[GameManager.Instance.PlayerGPS.CurrentRegionIndex];

            this.nameNPC = DaggerfallUnity.Instance.NameHelper.FullName(nameBankType, gender);

            // get face record id to use (randomize portrait for current person outfit variant)
            int personFaceVariant = Random.Range(0, numPersonFaceVariants);
            this.personFaceRecordId = recordIndices[personOutfitVariant] + personFaceVariant;

            // set billboard to correct race, gender and outfit variant
            Asset.SetPerson(race, gender, personOutfitVariant, IsGuard, personFaceVariant, personFaceRecordId);
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
