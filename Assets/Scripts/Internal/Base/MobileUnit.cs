// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2020 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net), TheLacus
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using DaggerfallWorkshop.Utility;
using System;
using DaggerfallWorkshop.Utility.AssetInjection;

namespace DaggerfallWorkshop
{
    /// <summary>
    /// Daggerfall mobile enemy. Handles loading resources and rendering object (i.e. a billboard) based on orientation and state.
    /// Note for implementors: an instance of a component extending this class can be added to a prefab named "DaggerfallMobileUnit" and bundled with a mod.
    /// </summary>
    public abstract class MobileUnit : MonoBehaviour
    {
        #region Properties

        /// <summary>
        /// True if enemy has been set, false otherwise.
        /// </summary>
        public abstract bool IsSetup { get; protected set; }

        /// <summary>
        /// Basic properties of this enemy.
        /// </summary>
        public abstract MobileEnemy Enemy { get; protected set; }

        /// <summary>
        /// An index that defines spawn distance type.
        /// </summary>
        public abstract byte ClassicSpawnDistanceType { get; protected set; }

        /// <summary>
        /// Current animation in progrees. Set with <see cref="ChangeEnemyState(MobileStates)"/>.
        /// </summary>
        public abstract MobileStates EnemyState { get; protected set; }

        /// <summary>
        /// True if special transformation is completed, false otherwise. See <see cref="SetSpecialTransformationCompleted()"/>.
        /// </summary>
        public abstract bool SpecialTransformationCompleted { get; protected set; }

        /// <summary>
        /// True if animation shouldn't be performed, for example because enemy is paralyzed,
        /// false if animation should be performed as usual.
        /// </summary>
        public abstract bool FreezeAnims { get; set; }

        /// <summary>
        /// True if enemy is facing away from player and might be backstabbed, false otherwise.
        /// </summary>
        public abstract bool IsBackFacing { get; }

        /// <summary>
        /// True if melee damage is scheduled to be performed, false otherwise.
        /// This is set by implementation when required during animation.
        /// </summary>
        public abstract bool DoMeleeDamage { get; set; }

        /// <summary>
        /// True if arrow shoot is scheduled to be performed, false otherwise.
        /// This is set by implementation when required during animation.
        /// </summary>
        public abstract bool ShootArrow { get; set; }

        /// <summary>
        /// Slow down the animation of this mobile unit by a divisor of frames per second.
        /// Used when enemies are slowed by spells etc.
        /// </summary>
        public abstract int FrameSpeedDivisor { get; set; }

        /// <summary>
        /// Get the MobileUnitSummary data
        /// </summary>
        public MobileUnitSummary Summary { get { return summary; } }

        [SerializeField]
        protected MobileUnitSummary summary = new MobileUnitSummary();

        [Serializable]
        public struct MobileUnitSummary
        {
            public bool IsSetup;                                        // Flagged true when mobile settings are populated
            public Rect[] AtlasRects;                                   // Array of rectangles for atlased materials
            public RecordIndex[] AtlasIndices;                          // Indices into rect array for atlased materials, supports animations
            public Vector2[] RecordSizes;                               // Size and scale of individual records
            public int[] RecordFrames;                                  // Number of frames of individual records
            public MobileEnemy Enemy;                                   // Mobile enemy settings
            public MobileStates EnemyState;                             // Animation state
            public MobileAnimation[] StateAnims;                        // Animation frames for this state
            public MobileBillboardImportedTextures ImportedTextures;    // Textures imported from mods
            public int AnimStateRecord;                                 // Record number of animation state
            public int[] StateAnimFrames;                               // Sequence of frames to play for this animation. Used for attacks
            public byte ClassicSpawnDistanceType;                       // 0 through 6 value read from spawn marker that determines distance at which enemy spawns/despawns in classic.
            public bool specialTransformationCompleted;                 // Mobile has completed special transformation (e.g. Daedra Seducer)
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets enemy data for setup. 
        /// </summary>
        /// <param name="dfUnity">Current instance of Daggerfall Unity.</param>
        /// <param name="enemy">Basic properties of this enemy.</param>
        /// <param name="reaction">Reaction setting for this enemy.</param>
        /// <param name="classicSpawnDistanceType">An index that defines spawn distance type.</param>
        public void SetEnemy(DaggerfallUnity dfUnity, MobileEnemy enemy, MobileReactions reaction, byte classicSpawnDistanceType)
        {
            enemy.Reactions = reaction;

            Enemy = enemy;
            EnemyState = MobileStates.Move;
            ClassicSpawnDistanceType = classicSpawnDistanceType;

            ApplyEnemy(dfUnity);

            IsSetup = true;
        }

        /// <summary>
        /// Sets a new enemy state and restarts frame counter, if not freezed.
        /// Certain states are one-shot only (such as attack and hurt) and return to idle when completed.
        /// Continuous states (such as move) will keep looping until changed.
        /// </summary>
        /// <param name="newState">New state.</param>
        public void ChangeEnemyState(MobileStates newState)
        {
            // Don't change state during animation freeze
            if (FreezeAnims)
                return;

            // Only change if in a different state
            MobileStates currentState = EnemyState;
            if (currentState != newState)
                ApplyEnemyStateChange(currentState, EnemyState = newState);
        }

        /// <summary>
        /// Gets true if playing a one-shot animation like attack and hurt.
        /// </summary>
        /// <returns>True if playing one-shot animation.</returns>
        public bool IsPlayingOneShot()
        {
            switch (EnemyState)
            {
                case MobileStates.Hurt:
                case MobileStates.PrimaryAttack:
                case MobileStates.RangedAttack1:
                case MobileStates.RangedAttack2:
                case MobileStates.Spell:
                case MobileStates.SeducerTransform1:
                case MobileStates.SeducerTransform2:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Gets true if motor should prevent action state changes while playing current oneshot anim.
        /// </summary>
        /// <returns>True if motor should pause state changes while playing.</returns>
        public bool OneShotPauseActionsWhilePlaying()
        {
            switch (EnemyState)
            {
                case MobileStates.SeducerTransform1:        // Seducer should not move and attack while transforming
                case MobileStates.SeducerTransform2:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Gets true if playing any attack animation.
        /// </summary>
        /// <returns>True if playing attack animation.</returns>
        public bool IsAttacking()
        {
            switch (EnemyState)
            {
                case MobileStates.PrimaryAttack:
                case MobileStates.RangedAttack1:
                case MobileStates.RangedAttack2:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Set special transformation completed, e.g. Daedra Seducer into winged form.
        /// Used internally by mobile unit after playing seducer animations.
        /// Called when restoring save game if unit has raised transformation completed flag.
        /// </summary>
        public void SetSpecialTransformationCompleted()
        {
            switch ((MobileTypes)Enemy.ID)
            {
                case MobileTypes.DaedraSeducer:
                    MobileEnemy enemy = Enemy;
                    enemy.Behaviour = MobileBehaviour.Flying;
                    enemy.CorpseTexture = EnemyBasics.CorpseTexture(400, 5);
                    enemy.HasIdle = false;
                    enemy.HasSpellAnimation = true;
                    enemy.SpellAnimFrames = new int[] { 0, 1, 2, 3 };
                    Enemy = enemy;
                    break;
            }


            SpecialTransformationCompleted = true;
        }

        #endregion

        #region Abstract Methods

        /// <summary>
        /// Gets size of enemy in scene.
        /// </summary>
        /// <returns>2D or 3D size of enemy object in scene.</returns>
        public abstract Vector3 GetSize();

        /// <summary>
        /// Applies enemy properties. Implementation should perform setup for specific <see cref="Enemy"/>
        /// (for example load/instantiate objects and materials) and then start current animation (<see cref="EnemyState"/>).
        /// </summary>
        /// <param name="dfUnity">Current instance of Daggerfall Unity.</param>
        protected abstract void ApplyEnemy(DaggerfallUnity dfUnity);

        /// <summary>
        /// Applies change to enemy state. Implementation should perform transition to
        /// new animation (already assigned to <see cref="EnemyState"/> by caller).
        /// </summary>
        /// <param name="currentState">Enemy state before the change.</param>
        /// <param name="newState">Enemy state after the change.</param>
        protected abstract void ApplyEnemyStateChange(MobileStates currentState, MobileStates newState);

        #endregion
    }
}
