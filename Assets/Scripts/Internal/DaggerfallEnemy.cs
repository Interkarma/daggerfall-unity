// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop
{
    /// <summary>
    /// Stores enemy settings for serialization and other tasks.
    /// </summary>
    public class DaggerfallEnemy : MonoBehaviour
    {
        ulong loadID = 0;
        bool questSpawn = false;

        public ulong LoadID
        {
            get { return loadID; }
            set { loadID = value; }
        }

        public bool QuestSpawn
        {
            get { return questSpawn; }
            set { questSpawn = value; }
        }

        private void Start()
        {
            // UESP describes acute hearing as "allows you to hear sounds from farther away"
            // https://en.uesp.net/wiki/Daggerfall:ClassMaker#Special_Advantages
            // Assuming this means enemy sounds specficially, rather than *all sounds* (which could get annoying)
            // If player has acute hearing advantage then enemy audio source max distance is increased by 25%
            // TODO: Learn more about acute hearing and refine how this works
            // NOTE: This should feel like a fun advantage and not just bombard player with audio!
            if (GameManager.Instance.PlayerEntity.Career.AcuteHearing)
            {
                AudioSource audioSource = GetComponent<AudioSource>();
                if (audioSource)
                    audioSource.maxDistance *= 1.25f;
            }
        }
    }
}