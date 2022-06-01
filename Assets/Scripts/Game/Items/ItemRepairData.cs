// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut
// Contributors:    
// 
// Notes:
//

using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Utility;
using UnityEngine;

namespace DaggerfallWorkshop.Game.Items
{
    /// <summary>
    /// Repair data for an item, used when left somewhere for repair.
    /// </summary>
    public class ItemRepairData
    {
        private string sceneName;
        private ulong timeStarted = 0;

        public int RepairTime { get; set; }

        public int EstimatedRepairTime { get; set; }

        public ulong GetTimeDone()
        {
            return timeStarted + (uint)RepairTime;
        }

        public ulong GetEstimatedTimeDone()
        {
            if (IsBeingRepaired())
                return timeStarted + (uint)EstimatedRepairTime;
            else
                return DaggerfallUnity.Instance.WorldTime.Now.ToSeconds() + (uint)EstimatedRepairTime;
        }

        public bool IsBeingRepaired()
        {
            return timeStarted > 0;
        }

        public bool IsBeingRepairedHere()
        {
            return IsBeingRepaired() && sceneName == GameManager.Instance.PlayerEnterExit.Interior.name;
        }

        public bool IsRepairFinished()
        {
            return IsBeingRepaired() && GetTimeDone() <= DaggerfallUnity.Instance.WorldTime.Now.ToSeconds();
        }

        public void LeaveForRepair(int repairTime)
        {
            if (!IsBeingRepaired())
            {
                sceneName = GameManager.Instance.PlayerEnterExit.Interior.name;
                timeStarted = DaggerfallUnity.Instance.WorldTime.Now.ToSeconds();
                RepairTime = repairTime;
            }
        }

        public void Collect()
        {
            if (IsBeingRepaired())
            {
                sceneName = null;
                timeStarted = 0;
            }
        }

        public int DaysUntilRepaired()
        {
            return GetDaysLeftUntil(GetTimeDone());
        }

        public int EstimatedDaysUntilRepaired()
        {
            return GetDaysLeftUntil(GetEstimatedTimeDone());
        }

        private int GetDaysLeftUntil(ulong time)
        {
            ulong timeNow = DaggerfallUnity.Instance.WorldTime.Now.ToSeconds();
            float timeLeft = time - timeNow;
            return (int)Mathf.Ceil(timeLeft / DaggerfallDateTime.SecondsPerDay);
        }

        public ItemRepairData_v1 GetSaveData()
        {
            if (IsBeingRepaired())
                return new ItemRepairData_v1()
                {
                    sceneName = sceneName,
                    timeStarted = timeStarted,
                    repairTime = RepairTime
                };
            else
                return null;
        }

        public void RestoreRepairData(ItemRepairData_v1 data)
        {
            if (data != null)
            {
                sceneName = data.sceneName;
                timeStarted = data.timeStarted;
                RepairTime = data.repairTime;
            }
        }
    }

}
