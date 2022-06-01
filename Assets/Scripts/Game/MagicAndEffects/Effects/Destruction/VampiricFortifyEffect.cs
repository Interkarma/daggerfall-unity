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
using DaggerfallConnect;
using DaggerfallWorkshop.Game.Entity;
using FullSerializer;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// WORK IN PROGRESS - Not currently implemented by any effects
    /// A custom effect unique to Daggerfall Unity.
    /// Also a test-case for future effect mod support (e.g. custom text tokens for spellmaker and spellbook).
    /// Works like a combined Drain/Fortify effect by draining target and fortifying caster by same amount.
    /// The vampiric link is permanent once establised until target dies or caster leaves the area.
    /// </summary>
    public abstract class VampiricFortifyEffect : IncumbentEffect
    {
        const float maxLinkDistance = 25f;

        protected DFCareer.Stats fortifyStat = DFCareer.Stats.None;
        int[] casterStatMods = new int[DaggerfallStats.Count];
        int forcedRoundsRemaining = 1;

        public DFCareer.Stats FortifyStat
        {
            get { return fortifyStat; }
        }

        // Always present at least one round remaining so effect system does not remove
        public override int RoundsRemaining
        {
            get { return forcedRoundsRemaining; }
        }

        // Transfer effects is permanent until dead or healed so we manage our own lifecycle
        protected override int RemoveRound()
        {
            return forcedRoundsRemaining;
        }

        public override void Start(EntityEffectManager manager, DaggerfallEntityBehaviour caster = null)
        {
            base.Start(manager, caster);
            PlayerAggro();
        }

        protected override bool IsLikeKind(IncumbentEffect other)
        {
            return (other is VampiricFortifyEffect && (other as VampiricFortifyEffect).fortifyStat == fortifyStat);
        }

        protected override void BecomeIncumbent()
        {
            // At this time intended to be cast by player only
            if (caster != GameManager.Instance.PlayerEntityBehaviour)
            {
                forcedRoundsRemaining = 0;
                ResignAsIncumbent();
                return;
            }

            // Local entity is drained by magnitude, caster is fortified by magnitude
            // Link will end once target is dead, passes out of scope, or exceeds maxLinkDistance
            int magnitude = GetMagnitude(caster);
            SetStatMod(fortifyStat, -magnitude);
            casterStatMods[(int)fortifyStat] = magnitude;
        }

        protected override void AddState(IncumbentEffect incumbent)
        {
        }

        public override void MagicRound()
        {
            base.MagicRound();

            // Caster must still be in range or vampiric link ends
            if (!CheckCasterDistance())
            {
                forcedRoundsRemaining = 0;
                ResignAsIncumbent();
                return;
            }

            // Send stat mods to caster effect manager
            EntityEffectManager casterManager = caster.GetComponent<EntityEffectManager>();
            casterManager.MergeDirectStatMods(casterStatMods);
        }

        bool CheckCasterDistance()
        {
            // If caster null then end immediately
            if (!caster)
                return false;

            // Get distance to caster
            float distance = Vector3.Distance(manager.transform.position, caster.transform.position);
            if (distance > maxLinkDistance)
                return false;

            return true;
        }

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public DFCareer.Stats fortifyStat;
            public int[] casterStatMods;
            public int forcedRoundsRemaining;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.fortifyStat = fortifyStat;
            data.casterStatMods = casterStatMods;
            data.forcedRoundsRemaining = forcedRoundsRemaining;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            fortifyStat = data.fortifyStat;
            casterStatMods = data.casterStatMods;
            forcedRoundsRemaining = data.forcedRoundsRemaining;
        }

        #endregion
    }
}
