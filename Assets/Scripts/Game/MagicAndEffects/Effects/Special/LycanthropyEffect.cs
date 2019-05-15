// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2019 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using FullSerializer;
using DaggerfallConnect;
using DaggerfallConnect.Utility;
using DaggerfallConnect.FallExe;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Items;
using Wenzil.Console;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Stage two curse effect for lycanthropy deployed after stage one infection completed.
    /// Handles buffs and other long-running werebeast effects.
    /// Note: This effect should only be assigned to player entity by stage one disease effect or classic character import.
    /// </summary>
    public class LycanthropyEffect : RacialOverrideEffect
    {
        #region Fields

        public const string LycanthropyCurseKey = "Lycanthropy-Curse";

        const int paperDollWidth = 110;
        const int paperDollHeight = 184;

        RaceTemplate compoundRace;
        LycanthropyTypes infectionType = LycanthropyTypes.None;
        uint lastKilledInnocent;
        bool hasStartedInitialLycanthropyQuest;
        bool wearingHircineRing;
        bool isTransformed;

        DFSize backgroundFullSize = new DFSize(125, 198);
        Rect backgroundSubRect = new Rect(8, 7, paperDollWidth, paperDollHeight);
        Texture2D backgroundTexture;

        #endregion

        #region Constructors

        public LycanthropyEffect()
        {
            // TODO: Register commands
        }

        #endregion

        #region Properties

        public LycanthropyTypes InfectionType
        {
            get { return infectionType; }
            set { infectionType = value; }
        }

        public override RaceTemplate CustomRace
        {
            get { return GetCompoundRace(); }
        }

        public bool IsTransformed
        {
            get { return isTransformed; }
        }

        #endregion

        #region Overrides

        public override void SetProperties()
        {
            properties.Key = LycanthropyCurseKey;
            properties.ShowSpellIcon = false;
            bypassSavingThrows = true;
        }

        public override void Start(EntityEffectManager manager, DaggerfallEntityBehaviour caster = null)
        {
            base.Start(manager, caster);

            // Create compound lycanthrope race from birth race
            CreateCompoundRace();

            // Get infection type from stage one disease
            // Note: Classic save import will start this effect and set correct type after load
            LycanthropyInfection infection = (LycanthropyInfection)GameManager.Instance.PlayerEffectManager.FindIncumbentEffect<LycanthropyInfection>();
            if (infection != null)
                infectionType = infection.InfectionType;

            // Considered sated on first start
            UpdateSatiation();

            // Our transformation is complete - cure everything on player (including stage one disease)
            GameManager.Instance.PlayerEffectManager.CureAll();
        }

        public override void ConstantEffect()
        {
            base.ConstantEffect();

            // Get peered entity gameobject
            DaggerfallEntityBehaviour entityBehaviour = GetPeeredEntityBehaviour(manager);
            if (!entityBehaviour)
                return;

            // Assign constant state changes for lycanthropes
            entityBehaviour.Entity.IsImmuneToDisease = true;
            entityBehaviour.Entity.IsImmuneToParalysis = true;

            // TODO: Assign minimum metal to hit only while transformed
            //entityBehaviour.Entity.MinMetalToHit = WeaponMaterialTypes.Silver;
        }

        public override void MagicRound()
        {
            base.MagicRound();

            // Check if player is wearing Hircine's Ring at start of each magic round
            // This item will change certain lycanthropy payload behaviours when equipped
            wearingHircineRing = IsWearingHircineRing();

            ApplyLycanthropeAdvantages();

            // Some temp debug info used during development
            Debug.LogFormat(
                "Lycanthropy MagicRound(). Type={0}, HircineRing={1}, IsTransformed={2}, Massar={3}, Secunda={4}",
                infectionType,
                wearingHircineRing,
                isTransformed,
                DaggerfallUnity.Instance.WorldTime.Now.MassarLunarPhase,
                DaggerfallUnity.Instance.WorldTime.Now.SecundaLunarPhase);
        }

        public override bool SetFPSWeapon(FPSWeapon target)
        {
            if (isTransformed)
            {
                target.WeaponType = WeaponTypes.Werecreature;
                target.MetalType = MetalTypes.None;
                target.DrawWeaponSound = SoundClips.None;
                target.SwingWeaponSound = SoundClips.None;
                target.Reach = WeaponManager.defaultWeaponReach;
                return true;
            }

            return false;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets lycanthrope need to kill sated from current point in time.
        /// </summary>
        public void UpdateSatiation()
        {
            lastKilledInnocent = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime();
        }

        public virtual void MorphSelf()
        {
            // TODO: Implement transformation

            // Simplistic implementation just to bootstrap various payloads
            if (!isTransformed)
            {
                isTransformed = true;

                // Unequip any items held in hands
                GameManager.Instance.PlayerEntity.ItemEquipTable.UnequipItem(EquipSlots.RightHand);
                GameManager.Instance.PlayerEntity.ItemEquipTable.UnequipItem(EquipSlots.LeftHand);

                // TODO: Show claws

                // TODO: Set last transform time for 24-hour cooldown
            }
            else
            {
                isTransformed = false;

                // TODO: Show unarmed
            }
        }

        #endregion

        #region Private Methods

        void CreateCompoundRace()
        {
            // Clone birth race and assign custom settings
            // New compound races will retain almost everything from birth race
            compoundRace = GameManager.Instance.PlayerEntity.BirthRaceTemplate.Clone();

            // TODO: Get race name based on infection type
            //compoundRace.Name = TextManager.Instance.GetText(racesTextDatabase, "weretype");

            // Set special lycanthropy flags
            compoundRace.ImmunityFlags |= DFCareer.EffectFlags.Disease;
        }

        RaceTemplate GetCompoundRace()
        {
            // Create compound race if one doesn't already exist
            if (compoundRace == null)
                CreateCompoundRace();

            return compoundRace;
        }

        void ApplyLycanthropeAdvantages()
        {
            // Set stat mods
            const int statModAmount = 40;
            SetStatMod(DFCareer.Stats.Strength, statModAmount);
            SetStatMod(DFCareer.Stats.Agility, statModAmount);
            SetStatMod(DFCareer.Stats.Endurance, statModAmount);
            SetStatMod(DFCareer.Stats.Speed, statModAmount);

            // Set skill mods
            const int skillModAmount = 30;
            SetSkillMod(DFCareer.Skills.Swimming, skillModAmount);
            SetSkillMod(DFCareer.Skills.Running, skillModAmount);
            SetSkillMod(DFCareer.Skills.Stealth, skillModAmount);
            SetSkillMod(DFCareer.Skills.CriticalStrike, skillModAmount);
            SetSkillMod(DFCareer.Skills.Climbing, skillModAmount);
            SetSkillMod(DFCareer.Skills.HandToHand, skillModAmount);
        }

        bool IsWearingHircineRing()
        {
            DaggerfallUnityItem[] equipTable = GameManager.Instance.PlayerEntity.ItemEquipTable.EquipTable;
            if (equipTable == null || equipTable.Length == 0)
                return false;

            return IsHircineRingItem(equipTable[(int)EquipSlots.Ring0]) || IsHircineRingItem(equipTable[(int)EquipSlots.Ring1]);
        }

        bool IsHircineRingItem(DaggerfallUnityItem item)
        {
            return
                item != null &&
                item.IsArtifact &&
                item.ContainsEnchantment(EnchantmentTypes.SpecialArtifactEffect, (short)ArtifactsSubTypes.Hircine_Ring);
        }

        #endregion

        #region Serialization

        [fsObject("v1")]
        public struct CustomSaveData_v1
        {
            public RaceTemplate compoundRace;
            public LycanthropyTypes infectionType;
            public uint lastKilledInnocent;
            public bool hasStartedInitialLycanthropyQuest;
            public bool wearingHircineRing;
            public bool isTransformed;
        }

        public override object GetSaveData()
        {
            CustomSaveData_v1 data = new CustomSaveData_v1();
            data.compoundRace = compoundRace;
            data.infectionType = infectionType;
            data.lastKilledInnocent = lastKilledInnocent;
            data.hasStartedInitialLycanthropyQuest = hasStartedInitialLycanthropyQuest;
            data.wearingHircineRing = wearingHircineRing;
            data.isTransformed = isTransformed;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            CustomSaveData_v1 data = (CustomSaveData_v1)dataIn;
            compoundRace = data.compoundRace;
            infectionType = data.infectionType;
            lastKilledInnocent = data.lastKilledInnocent;
            hasStartedInitialLycanthropyQuest = data.hasStartedInitialLycanthropyQuest;
            wearingHircineRing = data.wearingHircineRing;
            isTransformed = data.isTransformed;
        }

        #endregion

        #region Console Commands
        #endregion
    }
}