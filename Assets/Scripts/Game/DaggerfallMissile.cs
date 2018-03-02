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
using System;
using System.Collections;
using System.Collections.Generic;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Magic;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// Missile component for spell casters and archers.
    /// Designed to handle missile role in abstract way for other systems.
    /// Collects list of affected entities for involved system to process.
    /// Supports touch, target at range, area of effect, etc.
    /// Has some basic lighting effects that might expand later.
    /// Does not currently support serialization, but this will be added later.
    /// Not all settings are fully implemented or exposed to editor at this time.
    /// Currently ranged missiles can only move in a straight line as per classic.
    /// </summary>
    [RequireComponent(typeof(Light))]
    [RequireComponent(typeof(SphereCollider))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(DaggerfallAudioSource))]
    public class DaggerfallMissile : MonoBehaviour
    {
        #region Unity Properties

        public float MovementSpeed = 12.0f;                     // Speed missile moves through world
        public float ColliderRadius = 0.45f;                    // Radius of missile contact sphere
        public bool AreaOfEffect = false;                       // Make this an area of effect explosion that triggers on contact with entity or environment
        public float ExplosionRadius = 3.0f;                    // Radius of area of effect explosion
        public bool IsTouch = false;                            // Determines if missile performs a spherecast to entity target instead of firing a moving sphere
        public float TouchRange = 2.5f;                         // Maximum range for touch spherecast
        public bool EnableLight = true;                         // Show a light with this missile - player can force disable from settings
        public bool EnableShadows = true;                       // Light will cast shadows - player can force disable from settings
        public Color[] PulseColors;                             // Array of colours for pulse cycle, light will lerp from item-to-item and loop back to start - ignored if empty
        public float PulseSpeed = 0f;                           // Time in seconds light will lerp between pulse colours - 0 to disable
        public float FlickerMaxInterval = 0f;                   // Maximum interval for random flicker - 0 to disable
        public int BillboardFramesPerSecond = 5;                // Speed of billboard animatation
        public int ImpactBillboardFramesPerSecond = 15;         // Speed of contact billboard animation
        public float LifespanInSeconds = 8f;                    // How long missile will persist in world before self-destructing if no target found
        public float PostImpactLifespanInSeconds = 0.6f;        // Time in seconds missile will persist after impact
        public float PostImpactLightMultiplier = 0f;            // Scale of light intensity and range during post-impact lifespan - use 1.0 for no change, 0.0 for lights-out

        #endregion

        #region Fields

        const int coldMissileArchive = 376;
        const int fireMissileArchive = 375;
        const int magicMissileArchive = 379;
        const int poisonMissileArchive = 377;
        const int shockMissileArchive = 378;

        Vector3 direction;
        Light myLight;
        SphereCollider myCollider;
        Rigidbody myRigidbody;
        DaggerfallBillboard myBillboard;
        bool forceDisableSpellLighting;
        bool forceDisableSpellShadows;
        float lifespan = 0f;
        float postImpactLifespan = 0f;
        SpellTypes spellType = SpellTypes.None;
        bool impactDetected = false;
        float initialRange;
        float initialIntensity;

        #endregion

        #region Unity

        private void Start()
        {
            // Setup light and shadows
            myLight = GetComponent<Light>();
            myLight.enabled = EnableLight;
            forceDisableSpellLighting = !DaggerfallUnity.Settings.EnableSpellLighting;
            forceDisableSpellShadows = !DaggerfallUnity.Settings.EnableSpellShadows;
            if (forceDisableSpellLighting) myLight.enabled = false;
            if (forceDisableSpellShadows) myLight.shadows = LightShadows.None;
            initialRange = myLight.range;
            initialIntensity = myLight.intensity;

            // Setup collider
            myCollider = GetComponent<SphereCollider>();
            myCollider.radius = ColliderRadius;
            myCollider.isTrigger = true;

            // Setup rigidbody
            myRigidbody = GetComponent<Rigidbody>();
            myRigidbody.isKinematic = true;
            myRigidbody.useGravity = false;
        }

        private void Update()
        {
            // Handle missile lifespan pre and post-impact
            if (!impactDetected)
            {
                // Transform missile along direction vector
                transform.position += (direction * MovementSpeed) * Time.deltaTime;

                // Update lifespan and self-destruct if expired (e.g. spell fired straight up and will never hit anything)
                lifespan += Time.deltaTime;
                if (lifespan > LifespanInSeconds)
                    Destroy(gameObject);
            }
            else
            {
                // Track post impact lifespan
                postImpactLifespan += Time.deltaTime;
                if (postImpactLifespan > PostImpactLifespanInSeconds)
                    Destroy(gameObject);
            }

            // Update lighting effect
            UpdateLight();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Easily set billboard animations by spell type.
        /// </summary>
        /// <param name="spellType"></param>
        public void UseSpellBillboardAnims(SpellTypes spellType, int record = 0, bool oneShot = false)
        {
            // Destroy any existing billboard game object
            if (myBillboard)
            {
                myBillboard.gameObject.SetActive(false);
                Destroy(myBillboard.gameObject);
            }

            // Add new billboard parented to this missile
            GameObject go = GameObjectHelper.CreateDaggerfallBillboardGameObject(GetMissileTextureArchive(spellType), record, transform);
            go.transform.localPosition = Vector3.zero;
            myBillboard = go.GetComponent<DaggerfallBillboard>();
            myBillboard.FramesPerSecond = BillboardFramesPerSecond;
            myBillboard.FaceY = true;
            myBillboard.OneShot = oneShot;
            myBillboard.GetComponent<MeshRenderer>().receiveShadows = false;

            // Store spell type for target animation
            this.spellType = spellType;
        }

        /// <summary>
        /// Execute moving missile from starting point with given trajectory.
        /// Be careful not to start while intersecting with another collider (e.g. player) as this might trigger missile prematurely.
        /// </summary>
        public void ExecuteMobileMissile(Vector3 worldPosition, Vector3 direction)
        {
            transform.position = worldPosition;
            this.direction = direction;
        }

        #endregion

        #region Collision Handling

        private void OnTriggerEnter(Collider other)
        {
            List<DaggerfallEntityBehaviour> entities = new List<DaggerfallEntityBehaviour>();

            // Add targets
            if (AreaOfEffect)
            {
                // AOE targets
                Collider[] overlaps = Physics.OverlapSphere(transform.position, ExplosionRadius);
                for (int i = 0; i < overlaps.Length; i++)
                {
                    DaggerfallEntityBehaviour aoeEntity = overlaps[i].GetComponent<DaggerfallEntityBehaviour>();
                    if (aoeEntity)
                    {
                        entities.Add(aoeEntity);
                    }
                }
            }
            else
            {
                // Direct contact target
                DaggerfallEntityBehaviour contactEntity = other.GetComponent<DaggerfallEntityBehaviour>();
                if (contactEntity)
                {
                    entities.Add(contactEntity);
                }
            }

            // Play spell impact animation, this replaces spell missile animation
            if (spellType != SpellTypes.None)
            {
                UseSpellBillboardAnims(spellType, 1, true);
                myBillboard.FramesPerSecond = ImpactBillboardFramesPerSecond;
                impactDetected = true;
            }
        }

        #endregion

        #region Private Methods

        void UpdateLight()
        {
            // Do nothing if light disabled by missile properties or force disabled in user settings
            if (!EnableLight || forceDisableSpellLighting)
                return;

            // Scale post-impact
            if (impactDetected)
            {
                myLight.range = initialRange * PostImpactLightMultiplier;
                myLight.intensity = initialIntensity * PostImpactLightMultiplier;
            }
        }

        int GetMissileTextureArchive(SpellTypes spellType)
        {
            switch (spellType)
            {
                default:
                case SpellTypes.Cold:
                    return coldMissileArchive;
                case SpellTypes.Fire:
                    return fireMissileArchive;
                case SpellTypes.Magic:
                    return magicMissileArchive;
                case SpellTypes.Poison:
                    return poisonMissileArchive;
                case SpellTypes.Shock:
                    return shockMissileArchive;
            }
        }

        #endregion
    }
}