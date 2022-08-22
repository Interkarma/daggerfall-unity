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

using System;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Utility.AssetInjection;
using UnityEngine;

namespace DaggerfallWorkshop
{
    public abstract class PetMobileUnit : MonoBehaviour
    {
        #region Properties

        /// <summary>
        /// True if pet has been set, false otherwise.
        /// </summary>
        public abstract bool IsSetup { get; protected set; }

        /// <summary>
        /// Basic properties of this Pet.
        /// </summary>
        public abstract MobilePet Pet { get; protected set; }

        /// <summary>
        /// An index that defines spawn distance type.
        /// </summary>
        public abstract byte ClassicSpawnDistanceType { get; protected set; }

        /// <summary>
        /// Current animation in progrees. Set with <see cref="ChangePetState"/>.
        /// </summary>
        public abstract MobileStates PetState { get; protected set; }

        /// <summary>
        /// True if special transformation is completed, false otherwise. See <see cref="SetSpecialTransformationCompleted()"/>.
        /// </summary>
        public abstract bool SpecialTransformationCompleted { get; protected set; }

        /// <summary>
        /// True if animation shouldn't be performed, 
        /// false if animation should be performed as usual.
        /// </summary>
        public abstract bool FreezeAnims { get; set; }


        /// <summary>
        /// Get the MobileUnitSummary data
        /// </summary>
        public MobilePetUnitSummary Summary
        {
            get { return summary; }
        }

        [SerializeField] protected MobilePetUnitSummary summary = new MobilePetUnitSummary();

        [Serializable]
        public struct MobilePetUnitSummary
        {
            public bool IsSetup; // Flagged true when mobile settings are populated
            public Rect[] AtlasRects; // Array of rectangles for atlased materials
            public RecordIndex[] AtlasIndices; // Indices into rect array for atlased materials, supports animations
            public Vector2[] RecordSizes; // Size and scale of individual records
            public int[] RecordFrames; // Number of frames of individual records
            public MobilePet Pet; // Mobile pet settings
            public MobileStates PetState; // Animation state
            public MobileAnimation[] StateAnims; // Animation frames for this state
            public MobileBillboardImportedTextures ImportedTextures; // Textures imported from mods
            public int AnimStateRecord; // Record number of animation state
            public int[] StateAnimFrames; // Sequence of frames to play for this animation. Used for attacks

            public byte
                ClassicSpawnDistanceType; // 0 through 6 value read from spawn marker that determines distance at which pet spawns/despawns in classic.

            public bool
                specialTransformationCompleted; // Mobile has completed special transformation (e.g. Daedra Seducer)
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets pet data for setup. 
        /// </summary>
        /// <param name="dfUnity">Current instance of Daggerfall Unity.</param>
        /// <param name="pet">Basic properties of this pet.</param>
        /// <param name="reaction">Reaction setting for this pet.</param>
        /// <param name="classicSpawnDistanceType">An index that defines spawn distance type.</param>
        public void SetPet(DaggerfallUnity dfUnity, MobilePet pet, MobileReactions reaction,
            byte classicSpawnDistanceType)
        {
            pet.Reactions = reaction;

            Pet = pet;
            PetState = MobileStates.Move;
            ClassicSpawnDistanceType = classicSpawnDistanceType;

            ApplyPet(dfUnity);

            IsSetup = true;
        }

        /// <summary>
        /// Sets a new pet state and restarts frame counter, if not freezed.
        /// Certain states are one-shot only (such as attack and hurt) and return to idle when completed.
        /// Continuous states (such as move) will keep looping until changed.
        /// </summary>
        /// <param name="newState">New state.</param>
        public void ChangePetState(MobileStates newState)
        {
            // Don't change state during animation freeze
            if (FreezeAnims)
                return;

            // Only change if in a different state
            MobileStates currentState = PetState;
            if (currentState != newState)
                ApplyPetStateChange(currentState, PetState = newState);
        }

        /// <summary>
        /// Set special transformation completed, e.g. Daedra Seducer into winged form.
        /// Used internally by mobile unit after playing seducer animations.
        /// Called when restoring save game if unit has raised transformation completed flag.
        /// </summary>
        public void SetSpecialTransformationCompleted()
        {
            switch ((MobileTypes)Pet.ID)
            {
                case MobileTypes.DaedraSeducer:
                    MobilePet pet = Pet;
                    pet.CorpseTexture = EnemyBasics.CorpseTexture(400, 5);
                    pet.HasIdle = false;
                    Pet = pet;
                    break;
            }


            SpecialTransformationCompleted = true;
        }

        #endregion

        #region Abstract Methods

        /// <summary>
        /// Gets size of pet in scene.
        /// </summary>
        /// <returns>2D or 3D size of pet object in scene.</returns>
        public abstract Vector3 GetSize();

        /// <summary>
        /// Applies pet properties. Implementation should perform setup for specific <see cref="Pet"/>
        /// (for example load/instantiate objects and materials) and then start current animation (<see cref="PetState"/>).
        /// </summary>
        /// <param name="dfUnity">Current instance of Daggerfall Unity.</param>
        protected abstract void ApplyPet(DaggerfallUnity dfUnity);

        /// <summary>
        /// Applies change to pet state. Implementation should perform transition to
        /// new animation (already assigned to <see cref="EnemyState"/> by caller).
        /// </summary>
        /// <param name="currentState">pet state before the change.</param>
        /// <param name="newState">pet state after the change.</param>
        protected abstract void ApplyPetStateChange(MobileStates currentState, MobileStates newState);

        #endregion
    }
}