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

using UnityEngine;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Utility.ModSupport;

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

        /// <summary>
        /// Daggerfall mobile billboard or custom implementation.
        /// This property is initialized in Awake() so it can be accessed from Start().
        /// </summary>
        public MobileUnit MobileUnit { get; private set; }

        private void Awake()
        {
            MobileUnit = FindMobileUnit();
        }

        private void Start()
        {
            // UESP describes acute hearing as "allows you to hear sounds from farther away"
            // https://en.uesp.net/wiki/Daggerfall:ClassMaker#Special_Advantages
            // Assuming this means enemy sounds specficially, rather than *all sounds* (which could get annoying)
            // If player has acute hearing advantage then enemy audio source max distance is increased by 25%
            // If player also has improved acute hearing enchantment then enemy audio source max distance is increased by 50%
            // TODO: Learn more about acute hearing and refine how this works
            // NOTE: This should feel like a fun advantage and not just bombard player with audio!
            if (GameManager.Instance.PlayerEntity.Career.AcuteHearing)
            {
                const float acuteHearingMultiplier = 1.25f;
                const float improvedAcuteHearingMultiplier = 1.5f;

                AudioSource audioSource = GetComponent<AudioSource>();
                if (audioSource)
                    audioSource.maxDistance *= (GameManager.Instance.PlayerEntity.ImprovedAcuteHearing) ? improvedAcuteHearingMultiplier : acuteHearingMultiplier;
            }
        }

        private MobileUnit FindMobileUnit()
        {
            var mobileUnit = GetComponentInChildren<MobileUnit>();

            if (ModManager.Instance && ModManager.Instance.TryGetAsset("DaggerfallMobileUnit", true, out GameObject customMobileUnitGo))
            {
                var customMobileUnit = customMobileUnitGo.GetComponent<MobileUnit>();
                if (customMobileUnit)
                {
                    // Disable deault implementation (it can't be removed because it's part of a prefab)
                    if (mobileUnit)
                        mobileUnit.gameObject.SetActive(false);

                    customMobileUnitGo.transform.SetParent(gameObject.transform);
                    mobileUnit = customMobileUnit;
                }
                else
                {
                    Debug.LogError("Failed to retrieve MobileUnit component from GameObject.", customMobileUnitGo);
                }
            }

            return mobileUnit;
        }
    }
}