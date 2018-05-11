// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut
// Contributors: Allofich

using UnityEngine;
using DaggerfallConnect.Save;
using DaggerfallWorkshop.Game.MagicAndEffects;

namespace DaggerfallWorkshop.Game.Entity
{
    public class DaggerfallDisease
    {
        // Disease data. Found in FALL.EXE (1.07.213) from offset 0x1C0053.
        public DiseaseData[] diseaseData = new DiseaseData[]
        {              //  STR  INT  WIL  AGI  END  PER  SPD  LUC  HEA  FAT  SPL MIND  MAXD  MINS  MAXS
            new DiseaseData( 1,   0,   0,   0,   1,   0,   0,   0,   1,   0,   0,   2,   10, 0xFF, 0xFF), // Witches' Pox
            new DiseaseData( 1,   0,   1,   1,   1,   1,   1,   1,   1,   1,   1,   3,   30, 0xFF, 0xFF), // Plague
            new DiseaseData( 0,   0,   1,   0,   1,   0,   0,   0,   1,   0,   0,   5,   10, 0xFF, 0xFF), // Yellow Fever
            new DiseaseData( 0,   0,   0,   0,   0,   0,   0,   0,   1,   0,   0,   1,    5, 0xFF, 0xFF), // Stomach Rot
            new DiseaseData( 1,   0,   1,   1,   0,   0,   0,   0,   0,   0,   0,   2,   10, 0xFF, 0xFF), // Consumption
            new DiseaseData( 0,   0,   1,   0,   0,   1,   0,   0,   1,   0,   0,   1,    5, 0xFF, 0xFF), // Brain Fever
            new DiseaseData( 1,   0,   1,   1,   0,   0,   0,   0,   0,   0,   0,   2,   10, 0xFF, 0xFF), // Swamp Rot
            new DiseaseData( 1,   0,   0,   1,   0,   0,   1,   0,   0,   0,   0,   5,   10,    3,   18), // Caliron's Curse
            new DiseaseData( 1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   5,   30, 0xFF, 0xFF), // Cholera
            new DiseaseData( 1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   0,   5,   30, 0xFF, 0xFF), // Leprosy
            new DiseaseData( 1,   0,   0,   0,   1,   0,   0,   0,   1,   0,   0,   2,    4, 0xFF, 0xFF), // Wound Rot
            new DiseaseData( 0,   0,   0,   0,   1,   1,   0,   0,   0,   1,   0,   2,   10, 0xFF, 0xFF), // Red Death
            new DiseaseData( 0,   0,   1,   0,   0,   1,   0,   0,   1,   0,   0,   5,   10,    3,   18), // Blood Rot
            new DiseaseData( 0,   1,   0,   0,   1,   0,   0,   0,   1,   0,   0,   2,   10, 0xFF, 0xFF), // Typhoid Fever
            new DiseaseData( 0,   1,   1,   0,   0,   1,   0,   0,   0,   0,   0,   2,   10, 0xFF, 0xFF), // Dementia
            new DiseaseData( 0,   1,   0,   0,   0,   0,   0,   0,   0,   0,   1,   5,   10, 0xFF, 0xFF), // Chrondiasis
            new DiseaseData( 0,   1,   0,   0,   0,   0,   0,   0,   0,   0,   1,   2,    4,    3,   18), // Wizard Fever
        };

        public Diseases diseaseType;
        bool incubationOver = false;
        byte daysOfSymptomsLeft = 0;

        public DaggerfallDisease(Diseases disease)
        {
            diseaseType = disease;

            // Get index to disease data
            byte index = (byte)disease;

            // Get length of stat-worsening if not cured.
            // If the minimum length of the disease is set to 0xFF, this means stats never stop falling until cured. Otherwise, get a random length.
            daysOfSymptomsLeft = diseaseData[index].daysOfSymptomsMin;

            if (daysOfSymptomsLeft != 0xFF)
                daysOfSymptomsLeft = (byte)Random.Range(diseaseData[index].daysOfSymptomsMin, diseaseData[index].daysOfSymptomsMax + 1);

            Debug.Log("Contracted " + disease);
        }

        public DaggerfallDisease(DiseaseOrPoisonRecord record)
        {
            diseaseType = (Diseases)record.ParsedData.ID;

            if (record.ParsedData.incubationOver == 1)
                incubationOver = true;

            daysOfSymptomsLeft = (byte)record.ParsedData.daysOfSymptomsLeft;
        }

        public void ApplyDiseaseEffects(PlayerEntity player)
        {
            // A value of 0xFE for remaining days is used in classic for a disease that has run its course and is no longer
            // damaging stats
            if (daysOfSymptomsLeft == 0xFE)
                return;

            // Count down remaining days for diseases with limited time.
            if (daysOfSymptomsLeft != 0xFF && (--daysOfSymptomsLeft == 0))
            {
                daysOfSymptomsLeft = 0xFE;
                // Classic returns here, which is probably a mistake, since it would shave off the final day from the expected number of days
            }

            // Incubation is over and disease description now shows in status
            if (!incubationOver)
                incubationOver = true;

            // Get index to disease data
            byte index = (byte)diseaseType;

            int damageAmount = Random.Range(diseaseData[index].minDamage, diseaseData[index].maxDamage + 1);

            if (diseaseData[index].STR != 0)
            {
                // TODO: lower by damageAmount
                if (player.Stats.LiveStrength <= 0)
                    player.SetHealth(0);
            }
            if (diseaseData[index].INT != 0)
            {
                // TODO: lower by damageAmount
                if (player.Stats.LiveIntelligence <= 0)
                    player.SetHealth(0);
            }
            if (diseaseData[index].WIL != 0)
            {
                // TODO: lower by damageAmount
                if (player.Stats.LiveWillpower <= 0)
                    player.SetHealth(0);
            }
            if (diseaseData[index].AGI != 0)
            {
                // TODO: lower by damageAmount
                if (player.Stats.LiveAgility <= 0)
                    player.SetHealth(0);
            }
            if (diseaseData[index].END != 0)
            {
                // TODO: lower by damageAmount
                if (player.Stats.LiveEndurance <= 0)
                    player.SetHealth(0);
            }
            if (diseaseData[index].PER != 0)
            {
                // TODO: lower by damageAmount
                if (player.Stats.LivePersonality <= 0)
                    player.SetHealth(0);
            }
            if (diseaseData[index].SPD != 0)
            {
                // TODO: lower by damageAmount
                if (player.Stats.LiveSpeed <= 0)
                    player.SetHealth(0);
            }
            if (diseaseData[index].LUC != 0)
            {
                // TODO: lower by damageAmount
                if (player.Stats.LiveLuck <= 0)
                    player.SetHealth(0);
            }
            if (diseaseData[index].HEA != 0)
                player.CurrentHealth -= damageAmount;
            if (diseaseData[index].FAT != 0)
                player.CurrentFatigue -= damageAmount;
            if (diseaseData[index].SPL != 0)
                player.CurrentMagicka -= damageAmount;

            DaggerfallUI.AddHUDText(UserInterfaceWindows.HardStrings.youFeelSomewhatBad);
        }

        public bool HasFinishedIncubation()
        {
            if (incubationOver)
                return true;
            else
                return false;
        }

        public Diseases Type { get { return diseaseType; } }

        public int GetMessageId()
        {
            return (int) diseaseType + 100;
        }
    }
}