// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
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
    /// Repaired data, used when left somewhere for repair
    /// </summary>
    public class ItemRepairData
    {
        private string sceneName;
        private ulong timeStarted = 0;

        public int RepairTime { get; set; }

        public ulong GetTimeDone()
        {
            return timeStarted + (uint)RepairTime;
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
            Debug.LogFormat("RepairFinished? doneTime={0}, now={1}, repairTime={2}", GetTimeDone(), DaggerfallUnity.Instance.WorldTime.Now.ToSeconds(), RepairTime);
            return IsBeingRepaired() && GetTimeDone() <= DaggerfallUnity.Instance.WorldTime.Now.ToSeconds();
        }

        public void LeaveForRepair(int repairTime)
        {
            if (!IsBeingRepaired())
            {
                Debug.LogFormat("Repair Leave: repairTime={0}", repairTime);
                sceneName = GameManager.Instance.PlayerEnterExit.Interior.name;
                timeStarted = DaggerfallUnity.Instance.WorldTime.Now.ToSeconds();
                RepairTime = repairTime;
            }
        }

        public void Collect()
        {
            if (IsBeingRepaired())
            {
                Debug.Log("Collecting");
                sceneName = null;
                timeStarted = 0;
            }
        }

        public int DaysUntilRepaired()
        {
            ulong timeNow = DaggerfallUnity.Instance.WorldTime.Now.ToSeconds();
            float timeLeft = GetTimeDone() - timeNow;
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
