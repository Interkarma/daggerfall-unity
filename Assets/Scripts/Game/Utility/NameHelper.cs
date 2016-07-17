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

using System;
using System.Collections.Generic;
using UnityEngine;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Serialization;

namespace DaggerfallWorkshop.Game.Utility
{
    /// <summary>
    /// Generates names for Daggerfall NPCs and locations.
    /// Based on an incomplete understanding of NAMEGEN.DAT.
    /// This code should be considered experimental for now.
    /// </summary>
    public class NameHelper
    {
        #region Fields

        const string nameGenFilename = "NameGen";

        Dictionary<BankTypes, NameBank> bankDict = null;

        #endregion

        #region Structs & Enums

        /// <summary>
        /// Name banks available for generation.
        /// </summary>
        public enum BankTypes
        {
            Breton,
            Redguard,
            Nord,
            Noble,
            Imperial,
            DarkElf,
            HighElf,
            WoodElf,
            Location1,      // Location banks not well understood, treat as unknown
            Location2,
            Location3,
        }

        /// <summary>
        /// A bank is an array of sets.
        /// </summary>
        public struct NameBank
        {
            public int setCount;
            public NameSet[] sets;
        }

        /// <summary>
        /// A set is an array of string parts.
        /// </summary>
        public struct NameSet
        {
            public int setIndex;
            public string[] parts;
        }

        #endregion

        #region Constructors

        public NameHelper()
        {
            LoadNameGenData();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets random full name (first name + surname) for an NPC.
        /// Supports Breton, Redguard, Nord, Imperial, DarkElf, HighElf, WoodElf.
        /// All other types return empty string.
        /// </summary>
        public string FullName(BankTypes type, Genders gender)
        {
            // Get parts
            string firstName = FirstName(type, gender);
            string lastName = Surname(type);

            // Compose full name
            string fullName = firstName;
            if (!string.IsNullOrEmpty(lastName))
                fullName += " " + lastName;

            return fullName;
        }

        /// <summary>
        /// Gets random first name for an NPC.
        /// Supports Breton, Redguard, Nord, Imperial, DarkElf, HighElf, WoodElf.
        /// All other types return empty string.
        /// </summary>
        public string FirstName(BankTypes type, Genders gender)
        {
            // Bank dictionary must be ready
            if (bankDict == null)
                return string.Empty;

            // Generate name by type
            NameBank nameBank = bankDict[type];
            string firstName = string.Empty;
            switch (type)
            {
                case BankTypes.Breton:                                                  // These banks all work the same
                case BankTypes.Nord:
                case BankTypes.Imperial:
                case BankTypes.DarkElf:
                case BankTypes.HighElf:
                case BankTypes.WoodElf:
                    firstName = GetRandomFirstName(nameBank, gender);
                    break;

                case BankTypes.Redguard:                                                // Redguards have just a single name
                    firstName = GetRandomRedguardName(nameBank, gender);
                    break;
            }

            return firstName;
        }

        /// <summary>
        /// Gets random surname for an NPC.
        /// Supports Breton, Nord, Imperial, DarkElf, HighElf, WoodElf.
        /// All other types return empty string.
        /// </summary>
        public string Surname(BankTypes type)
        {
            // Bank dictionary must be ready
            if (bankDict == null)
                return string.Empty;

            // Generate name by type
            NameBank nameBank = bankDict[type];
            string lastName = string.Empty;
            switch (type)
            {
                case BankTypes.Breton:                                                  // These banks all work the same
                case BankTypes.Imperial:
                case BankTypes.DarkElf:
                case BankTypes.HighElf:
                case BankTypes.WoodElf:
                    lastName = GetRandomSurname(nameBank);
                    break;

                case BankTypes.Nord:
                    lastName = GetRandomSurname(bankDict[BankTypes.Breton]);            // Nords appear to share Breton surnames
                    break;
            }

            return lastName;
        }

        #endregion

        #region Name Generation

        //
        // -= Rules For Name Generation, By Bank =-
        //
        // Breton:
        //  Male FirstName          : Sets 0 + 1
        //  Female FirstName        : Sets 2 + 3
        //  Surname                 : Sets 4 + 5
        //
        // Redguard:
        //  Male FullName           : Sets 0 + 1 + 2 + 3
        //  Female FullName         : Sets 0 + 1 + 2 + 4
        //
        // Nord:
        //  Male FirstName          : Sets 0 + 1
        //  Female FirstName        : Sets 2 + 3
        //  Surname (Use Breton)    : Sets 4 + 5
        //
        // Noble:
        //  Title                   : Sets 0
        //  FullName                : Sets 1 + 2 + 3
        //
        // Imperial:
        //  Male FirstName          : Sets 0 + 1
        //  Female FirstName        : Sets 2 + 3
        //  Surname                 : Sets 4 + 5
        //
        // DarkElf:
        //  Male FirstName          : Sets 0 + 1
        //  Female FirstName        : Sets 2 + 3
        //  Surname                 : Sets 4 + 5
        //
        // HighElf:
        //  Male FirstName          : Sets 0 + 1
        //  Female FirstName        : Sets 2 + 3
        //  Surname                 : Sets 4 + 5
        //
        // WoodElf:
        //  Male FirstName          : Sets 0 + 1
        //  Female FirstName        : Sets 2 + 3
        //  Surname                 : Sets 4 + 5
        //
        // Location1:
        //  Masculine FirstName     : Sets 0 + 1
        //  Feminine FirstName      : Sets 2 + 3
        //  Surname                 : Sets 4 + 5
        //
        // Location2:
        //  FullName                : Sets 0 + 1 + 2
        //
        // Location3:
        //  Masculine FullName      : Sets 0 + 1 + 2
        //  Feminine FullName       : Sets 0 + 1 + 2 + 3
        //
        // -= END =-
        //

        // Gets random first name by gender for names that follow 0+1 (male), 2+3 (female) pattern
        string GetRandomFirstName(NameBank nameBank, Genders gender)
        {
            // Get set parts
            string[] partsA, partsB;
            if (gender == Genders.Male)
            {
                partsA = nameBank.sets[0].parts;
                partsB = nameBank.sets[1].parts;
            }
            else
            {
                partsA = nameBank.sets[2].parts;
                partsB = nameBank.sets[3].parts;
            }

            // Generate strings
            string stringA = partsA[DFRandom.random_range(partsA.Length)];
            string stringB = partsB[DFRandom.random_range(partsB.Length)];

            return stringA + stringB;
        }

        // Gets random surname for names that follow 4+5 pattern
        string GetRandomSurname(NameBank nameBank)
        {
            // Get set parts
            string[] partsA, partsB;
            partsA = nameBank.sets[4].parts;
            partsB = nameBank.sets[5].parts;

            // Generate strings
            string stringA = partsA[DFRandom.random_range(partsA.Length)];
            string stringB = partsB[DFRandom.random_range(partsB.Length)];

            return stringA + stringB;
        }

        // Gets random Redguard name which follows 0+1+2+3 (male), 0+1+2+4 (female) pattern
        string GetRandomRedguardName(NameBank nameBank, Genders gender)
        {
            // Get set parts
            string[] partsA, partsB, partsC, partsD;
            if (gender == Genders.Male)
            {
                partsA = nameBank.sets[0].parts;
                partsB = nameBank.sets[1].parts;
                partsC = nameBank.sets[2].parts;
                partsD = nameBank.sets[3].parts;
            }
            else
            {
                partsA = nameBank.sets[0].parts;
                partsB = nameBank.sets[1].parts;
                partsC = nameBank.sets[2].parts;
                partsD = nameBank.sets[4].parts;
            }

            // Generate strings
            string stringA = partsA[DFRandom.random_range(partsA.Length)];
            string stringB = partsB[DFRandom.random_range(partsB.Length)];
            string stringC = partsC[DFRandom.random_range(partsC.Length)];
            string stringD = partsD[DFRandom.random_range(partsD.Length)];

            return stringA + stringB + stringC + stringD;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Loads namegen data from JSON file.
        /// </summary>
        void LoadNameGenData()
        {
            try
            {
                TextAsset nameGenText = Resources.Load<TextAsset>(nameGenFilename) as TextAsset;
                bankDict = SaveLoadManager.Deserialize(typeof(Dictionary<BankTypes, NameBank>), nameGenText.text) as Dictionary<BankTypes, NameBank>;
            }
            catch
            {
                Debug.Log("Could not load NameGen database from Resources. Check file exists and is in correct format.");
            }
        }

        #endregion
    }
}