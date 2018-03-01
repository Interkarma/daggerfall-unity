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
        Diseases diseaseType = Diseases.None;
        ulong contractedTime = 0;
        uint incubationTime = 0;

        private const uint baseIncubation = 60 * 60 * 16;    // 16 hours

        private WorldTime worldTime;

        private static List<Diseases> affectsHealth = new List<Diseases>
            { Diseases.BloodRot, Diseases.BrainFever };

        private static Dictionary<Diseases, List<DFCareer.Stats>> affectedStats = new Dictionary<Diseases, List<DFCareer.Stats>>
        {
            { Diseases.BloodRot, new List<DFCareer.Stats>() { DFCareer.Stats.Personality, DFCareer.Stats.Willpower } },
            { Diseases.BrainFever, new List<DFCareer.Stats>() { DFCareer.Stats.Personality, DFCareer.Stats.Willpower } },
            { Diseases.CalironsCurse, new List<DFCareer.Stats>() { DFCareer.Stats.Strength, DFCareer.Stats.Speed, DFCareer.Stats.Agility } },
        };

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
            Debug.Log("Contracted " + disease);

        }

        public Diseases Type { get { return diseaseType; } }

        public bool IsDiseased() { return diseaseType != Diseases.None; }

        public int GetMessageId()
        {
            if (diseaseType == Diseases.None) // || contractedTime + incubationTime > worldTime.Now.ToSeconds())
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
            };
        }

        public void RestoreSaveData(DaggerfallDisease_v1 data)
        {
            if (data != null)
            {
                diseaseType = data.disease;
                contractedTime = data.diseaseContractedTime;
                incubationTime = data.diseaseInclubationTime;
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