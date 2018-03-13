// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut
// Contributors:    

using DaggerfallConnect;
using DaggerfallWorkshop.Game.Serialization;
using System.Collections.Generic;
using UnityEngine;

namespace DaggerfallWorkshop.Game.Entity
{
    public class DaggerfallDisease
    {
        public struct DiseaseData
        {
            // Affected stats
            public byte STR;
            public byte INT;
            public byte WIL;
            public byte AGI;
            public byte END;
            public byte PER;
            public byte SPD;
            public byte LUC;
            public byte HEA;
            public byte FAT;
            public byte SPL;
            public byte minDamage;
            public byte maxDamage;
            public byte daysOfSymptomsMin; // 0xFF means never-ending
            public byte daysOfSymptomsMax;

            // Constructor
            public DiseaseData(byte STRp, byte INTp,
                byte WILp, byte AGIp, byte ENDp, byte PERp,
                byte SPDp, byte LUCp, byte HEAp, byte FATp,
                byte SPLp, byte minDamagep, byte maxDamagep,
                byte daysOfSymptomsMinp, byte daysOfSymptomsMaxp)
            {
                STR = STRp;
                INT = INTp;
                WIL = WILp;
                AGI = AGIp;
                END = ENDp;
                PER = PERp;
                SPD = SPDp;
                LUC = LUCp;
                HEA = HEAp;
                FAT = FATp;
                SPL = SPLp;
                minDamage = minDamagep;
                maxDamage = maxDamagep;
                daysOfSymptomsMin = daysOfSymptomsMinp;
                daysOfSymptomsMax = daysOfSymptomsMaxp;
            }
        }

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

        Diseases diseaseType = Diseases.None;
        ulong contractedTime = 0;
        uint incubationTime = 0;
        public byte daysOfSymptoms = 0;
        public int totalDamage = 0;

        private const uint baseIncubation = 60 * 60 * 16;    // 16 hours

        private WorldTime worldTime;

        /*private static List<Diseases> affectsHealth = new List<Diseases>
            { Diseases.BloodRot, Diseases.BrainFever };

        private static Dictionary<Diseases, List<DFCareer.Stats>> affectedStats = new Dictionary<Diseases, List<DFCareer.Stats>>
        {
            { Diseases.BloodRot, new List<DFCareer.Stats>() { DFCareer.Stats.Personality, DFCareer.Stats.Willpower } },
            { Diseases.BrainFever, new List<DFCareer.Stats>() { DFCareer.Stats.Personality, DFCareer.Stats.Willpower } },
            { Diseases.CalironsCurse, new List<DFCareer.Stats>() { DFCareer.Stats.Strength, DFCareer.Stats.Speed, DFCareer.Stats.Agility } },
        };*/

        public DaggerfallDisease()
        {
            worldTime = DaggerfallUnity.Instance.WorldTime;
            //WorldTime.OnNewHour += OnNewHourEventHandler;
        }

        public DaggerfallDisease(Diseases disease) : this()
        {
            diseaseType = disease;
            contractedTime = worldTime.Now.ToSeconds();
            incubationTime = CalculateIncubationTime();

            // Get index to disease data
            byte index = (byte)((byte)disease - 100);

            // Get length of stat-worsening if not cured.
            // If the minimum length of the disease is set to 0xFF, this means stats never stop falling until cured. Otherwise, get a random length.
            daysOfSymptoms = diseaseData[index].daysOfSymptomsMin;

            if (daysOfSymptoms != 0xFF)
                daysOfSymptoms = (byte)Random.Range(diseaseData[index].daysOfSymptomsMin, diseaseData[index].daysOfSymptomsMax + 1);

            Debug.Log("Contracted " + disease);
        }

        public void ApplyDiseaseEffects(PlayerEntity player)
        {
            // Get index to disease data
            byte index = (byte)((byte)diseaseType - 100);

            // A value of 0xFE for remaining days is used in classic for a disease that has run its course and is no longer
            // damaging stats
            if (daysOfSymptoms == 0xFE)
                return;

            // Count down remaining days for diseases with limited time.
            if (daysOfSymptoms != 0xFF && (--daysOfSymptoms == 0))
            {
                daysOfSymptoms = 0xFE;
                // Classic returns here, which is probably a mistake, since it would shave off the final day from the expected number of days
            }

            int damageAmount = Random.Range(diseaseData[index].minDamage, diseaseData[index].maxDamage + 1);

            // Tally damage total. A tally like this seems to be used in classic for reversing stat damage when the disease is cured.
            totalDamage += damageAmount;

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
            if (contractedTime + incubationTime <= DaggerfallUnity.Instance.WorldTime.Now.ToSeconds())
                return true;
            else
                return false;
        }

        public Diseases Type { get { return diseaseType; } }

        public bool IsDiseased() { return diseaseType != Diseases.None; }

        public int GetMessageId()
        {
            if (diseaseType == Diseases.None || !HasFinishedIncubation())
                return 18;
            else
                return (int) diseaseType;
        }

        public DaggerfallDisease_v1 GetSaveData()
        {
            return new DaggerfallDisease_v1()
            {
                disease = diseaseType,
                diseaseContractedTime = contractedTime,
                diseaseInclubationTime = incubationTime,
                diseaseDaysOfSymptoms = daysOfSymptoms,
            };
        }

        public void RestoreSaveData(DaggerfallDisease_v1 data)
        {
            if (data != null)
            {
                diseaseType = data.disease;
                contractedTime = data.diseaseContractedTime;
                incubationTime = data.diseaseInclubationTime;
                daysOfSymptoms = data.diseaseDaysOfSymptoms;
            }
            else
            {
                diseaseType = Diseases.None;
            }
        }

        private uint CalculateIncubationTime()
        {
            int medical = GameManager.Instance.PlayerEntity.Skills.GetLiveSkillValue(DFCareer.Skills.Medical);
            return (uint) (medical * baseIncubation) / 200;
        }

        protected virtual void OnNewHourEventHandler()
        {
            Debug.Log("Hour...");
            // doesn't seem to work when resting.
        }
            
    }
}