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
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Allows an effect to override player's racial display information such as race name and portrait.
    /// Used for vampirism and lycanthropy and possibly could be used for future racial overrides.
    /// Considered a minimal implementation at this time for core game to support vamp/were only.
    /// Only intended to be used on player entity. Will be permanent until removed.
    /// Only a single racial override incumbent effect can be active on player at one time.
    /// </summary>
    public abstract class RacialOverrideEffect : IncumbentEffect
    {
        #region Fields

        protected int forcedRoundsRemaining = 1;

        #endregion

        #region Properties

        /// <summary>
        /// Allow racial override to suppress Combat Voices option as required.
        /// </summary>
        public virtual bool SuppressOptionalCombatVoices
        {
            get { return false; }
        }

        /// <summary>
        /// Allow racial override to suppress paper doll body and items to show background only.
        /// </summary>
        public virtual bool SuppressPaperDollBodyAndItems
        {
            get { return false; }
        }

        /// <summary>
        /// Allows racial override to suppress crimes by player.
        /// </summary>
        public virtual bool SuppressCrime
        {
            get { return false; }
        }

        /// <summary>
        /// Allows racial override to suppress population spawns.
        /// </summary>
        public virtual bool SuppressPopulationSpawns
        {
            get { return false; }
        }

        #endregion

        #region Overrides

        // Always present at least one round remaining so effect system does not remove
        public override int RoundsRemaining
        {
            get { return forcedRoundsRemaining; }
        }

        // Racial overrides are permanent until removed so we manage our own lifecycle
        protected override int RemoveRound()
        {
            return forcedRoundsRemaining;
        }

        protected override bool IsLikeKind(IncumbentEffect other)
        {
            return (other is RacialOverrideEffect);
        }

        protected override void AddState(IncumbentEffect incumbent)
        {
            return;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets custom paper doll background texture for this override.
        /// Return false to use standard background for race.
        /// </summary>
        public virtual bool GetCustomPaperDollBackgroundTexture(PlayerEntity playerEntity, out Texture2D textureOut)
        {
            textureOut = null;
            return false;
        }

        /// <summary>
        /// Gets custom head ImageData for this override.
        /// Return false to just use standard head.
        /// </summary>
        public virtual bool GetCustomHeadImageData(PlayerEntity playerEntity, out ImageData imageDataOut)
        {
            imageDataOut = new ImageData();
            return false;
        }

        /// <summary>
        /// Gets custom attack sound clip for race and gender.
        /// </summary>
        public virtual bool GetCustomRaceGenderAttackSoundData(PlayerEntity playerEntity, out SoundClips soundClipOut)
        {
            soundClipOut = SoundClips.None;
            return false;
        }

        /// <summary>
        /// Called by WeaponManager when player hits an entity with a weapon (includes hand-to-hand).
        /// Target entity may be null, racial overrides should handle this.
        /// </summary>
        public virtual void OnWeaponHitEntity(PlayerEntity playerEntity, DaggerfallEntity targetEntity = null)
        {
        }

        /// <summary>
        /// Checks if custom race can initiate fast travel.
        /// Return true to allow fast travel or false to block it.
        /// </summary>
        public virtual bool CheckFastTravel(PlayerEntity playerEntity)
        {
            return true;
        }

        /// <summary>
        /// Checks if custom race can initiate rest.
        /// Return true to allow rest or false to block it.
        /// </summary>
        public virtual bool CheckStartRest(PlayerEntity playerEntity)
        {
            return true;
        }

        /// <summary>
        /// Starts custom racial quest.
        ///  * Called every 38 days with isCureQuest = false
        ///  * Called every 84 days with isCureQuest = true
        /// Mainly used by vampirism and lycanthropy in core.
        /// Custom racial override effects can ignore this virtual to start and manage quests however they like.
        /// </summary>
        /// <param name="isCureQuest">True when this should start cure quest.</param>
        public virtual void StartQuest(bool isCureQuest)
        {
        }

        /// <summary>
        /// Set state of current FPS weapon from WeaponManager.
        /// Allows the racial override to set a unique weapon type such as wereclaws.
        /// </summary>
        /// <param name="target">Target FPSWeapon to change.</param>
        /// <returns>True if weapon changed.</returns>
        public virtual bool SetFPSWeapon(FPSWeapon target)
        {
            return false;
        }

        /// <summary>
        /// Allow racial override to suppress inventory UI.
        /// Some care might need to be taken by other systems this does not crash game like classic.
        /// </summary>
        /// <param name="suppressInventoryMessage">Optional message to display when inventory suppressed.</param>
        /// <returns>True if inventory should be suppressed.</returns>
        public virtual bool GetSuppressInventory(out string suppressInventoryMessage)
        {
            suppressInventoryMessage = string.Empty;
            return false;
        }

        /// <summary>
        /// Allow racial overrides to suppress talk UI.
        /// </summary>
        /// <param name="suppressTalkMessage">Optional message to display when talk suppressed.</param>
        /// <returns>True if talk should be suppressed.</returns>
        public virtual bool GetSuppressTalk(out string suppressTalkMessage)
        {
            suppressTalkMessage = string.Empty;
            return false;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets custom race exposed by this override
        /// </summary>
        public abstract RaceTemplate CustomRace { get; }

        #endregion
    }
}