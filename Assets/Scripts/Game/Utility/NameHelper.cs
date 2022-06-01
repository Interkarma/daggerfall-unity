// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
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
            DarkElf,
            HighElf,
            WoodElf,
            Khajiit,
            Imperial,       // Imperial names appear where one would expect Argonian names.
            Monster1,
            Monster2,
            Monster3,
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
        /// Supports Breton, Redguard, Nord, DarkElf, HighElf, WoodElf, Khajiit, Imperial.
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
        /// Supports Breton, Redguard, Nord, DarkElf, HighElf, WoodElf, Khajiit, Imperial.
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
                case BankTypes.DarkElf:
                case BankTypes.HighElf:
                case BankTypes.WoodElf:
                case BankTypes.Khajiit:
                case BankTypes.Imperial:
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
        /// Supports Breton, Nord, DarkElf, HighElf, WoodElf, Khajiit, Imperial.
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
                case BankTypes.DarkElf:
                case BankTypes.HighElf:
                case BankTypes.WoodElf:
                case BankTypes.Khajiit:
                case BankTypes.Imperial:
                    lastName = GetRandomSurname(nameBank);
                    break;

                case BankTypes.Nord:
                    lastName = GetRandomNordSurname(nameBank);
                    break;
            }

            return lastName;
        }

        /// <summary>
        /// Gets random monster name for quests.
        /// </summary>
        public string MonsterName(Genders gender = Genders.Male)
        {
            // Bank dictionary must be ready
            if (bankDict == null)
                return string.Empty;

            return GetRandomMonsterName(gender);
        }

        #endregion

        #region Name Generation

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
            uint index = DFRandom.rand() % (uint)partsA.Length;
            string stringA = partsA[index];

            index = DFRandom.rand() % (uint)partsB.Length;
            string stringB = partsB[index];

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
            uint index = DFRandom.rand() % (uint)partsA.Length;
            string stringA = partsA[index];

            index = DFRandom.rand() % (uint)partsB.Length;
            string stringB = partsB[index];

            return stringA + stringB;
        }

        // Gets random surname for Nord names that follow 0+1+"sen" pattern
        string GetRandomNordSurname(NameBank nameBank)
        {
            // Get set parts
            string[] partsA, partsB;
            partsA = nameBank.sets[0].parts;
            partsB = nameBank.sets[1].parts;

            // Generate strings
            uint index = DFRandom.rand() % (uint)partsA.Length;
            string stringA = partsA[index];

            index = DFRandom.rand() % (uint)partsB.Length;
            string stringB = partsB[index];

            return stringA + stringB + "sen";
        }

        // Gets random Redguard name which follows 0+1+2+3(75%) (male), 0+1+2+4 (female) pattern
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
            uint index = DFRandom.rand() % (uint)partsA.Length;
            string stringA = partsA[index];

            index = DFRandom.rand() % (uint)partsB.Length;
            string stringB = partsB[index];

            index = DFRandom.rand() % (uint)partsC.Length;
            string stringC = partsC[index];

            string stringD = string.Empty;
            if (gender == Genders.Female || (DFRandom.rand() % 100 < 75))
            {
                index = DFRandom.rand() % (uint)partsD.Length;
                stringD = partsD[index];
            }

            return stringA + stringB + stringC + stringD;
        }

        // Get random monster name.
        // Monster1: 0+(50% +1)+2
        // Monster2: 0+(50% +1)+2+(if female, +3)
        // Monster3: (if male, 25% +3 + " ")+0+1+2
        string GetRandomMonsterName(Genders gender)
        {
            BankTypes type = (BankTypes)UnityEngine.Random.Range(8, 9 + 1); // Get random Monster1 or Monster2 for now.
            NameBank nameBank = bankDict[type];

            // Get set parts
            string[] partsA, partsB, partsC, partsD;
            partsA = nameBank.sets[0].parts;
            partsB = nameBank.sets[1].parts;
            partsC = nameBank.sets[2].parts;
            partsD = null;

            string stringA = string.Empty;
            string stringB = string.Empty;
            string stringC = string.Empty;
            string stringD = string.Empty;

            uint index = 0;

            // Additional set for Monster2 and Monster3
            if (nameBank.sets.Length >= 4)
                partsD = nameBank.sets[3].parts;

            // Generate strings
            if (type != BankTypes.Monster3) // Monster1 or Monster2
            {
                index = DFRandom.rand() % (uint)partsA.Length;
                stringA = partsA[index];

                stringB = string.Empty;
                if (DFRandom.rand() % 50 < 25)
                {
                    index = DFRandom.rand() % (uint)partsB.Length;
                    stringB = partsB[index];
                }

                index = DFRandom.rand() % (uint)partsC.Length;
                stringC = partsC[index];

                // Additional set for Monster2 female
                if (partsD != null && gender == Genders.Female)
                {
                    index = DFRandom.rand() % (uint)partsD.Length;
                    stringD = partsD[index];
                }
            }
            else // Monster3
            {
                if (gender == Genders.Female || DFRandom.rand() % 100 >= 25)
                {
                    index = DFRandom.rand() % (uint)partsA.Length;
                    stringA = partsA[index];
                }
                else
                {
                    index = DFRandom.rand() % (uint)partsD.Length;
                    stringA = partsD[index] + " ";

                    index = DFRandom.rand() % (uint)partsA.Length;
                    stringB = partsA[index];
                }

                index = DFRandom.rand() % (uint)partsB.Length;
                stringC = partsB[index];

                index = DFRandom.rand() % (uint)partsC.Length;
                stringD = partsC[index];
            }

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