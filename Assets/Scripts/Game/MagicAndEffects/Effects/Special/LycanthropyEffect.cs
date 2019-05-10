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
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Game.Questing;
using DaggerfallWorkshop.Game.Items;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Utility;
using DaggerfallConnect;
using DaggerfallConnect.Utility;
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

        #region Overrides

        public LycanthropyTypes InfectionType
        {
            get { return infectionType; }
            set { infectionType = value; }
        }

        public override RaceTemplate CustomRace
        {
            get { return GetCompoundRace(); }
        }

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
            ApplyLycanthropeAdvantages();
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
            // TODO: Apply advantages
        }

        #endregion

        #region Serialization
        #endregion

        #region Console Commands
        #endregion
    }
}