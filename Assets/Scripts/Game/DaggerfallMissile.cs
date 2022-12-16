// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    Allofich
// 
// Notes:
//

using UnityEngine;
using System.Collections.Generic;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.MagicAndEffects;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// Missile component for spell casters and archers.
    /// Designed to handle missile role in abstract way for other systems.
    /// Collects list of affected entities for involved system to process.
    /// Supports touch, target at range, area of effect.
    /// Has some basic lighting effects that might expand later.
    /// Does not currently support serialization, but this will be added later.
    /// Currently ranged missiles can only move in a straight line as per classic.
    /// </summary>
    [RequireComponent(typeof(Light))]
    [RequireComponent(typeof(SphereCollider))]
    [RequireComponent(typeof(MeshCollider))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(DaggerfallAudioSource))]
    public class DaggerfallMissile : MonoBehaviour
    {
        #region Unity Properties

        public float MovementSpeed = 25.0f;                     // Speed missile moves through world
        public float ColliderRadius = 0.45f;                    // Radius of missile contact sphere
        public float ExplosionRadius = 4.0f;                    // Radius of area of effect explosion
        public bool EnableLight = true;                         // Show a light with this missile - player can force disable from settings
        public bool EnableShadows = true;                       // Light will cast shadows - player can force disable from settings
        public Color[] PulseColors;                             // Array of colours for pulse cycle, light will lerp from item-to-item and loop back to start - ignored if empty
        public float PulseSpeed = 0f;                           // Time in seconds light will lerp between pulse colours - 0 to disable
        public float FlickerMaxInterval = 0f;                   // Maximum interval for random flicker - 0 to disable
        public int BillboardFramesPerSecond = 5;                // Speed of billboard animatation
        public int ImpactBillboardFramesPerSecond = 15;         // Speed of contact billboard animation
        public float LifespanInSeconds = 8f;                    // How long missile will persist in world before self-destructing if no target found
        public float PostImpactLifespanInSeconds = 0.6f;        // Time in seconds missile will persist after impact
        public float PostImpactLightMultiplier = 1f;            // Scale of light intensity and range during post-impact lifespan - use 1.0 for no change, 0.0 for lights-out
        public SoundClips ImpactSound = SoundClips.None;        // Impact sound of missile

        #endregion

        #region Fields

        const int coldMissileArchive = 376;
        const int fireMissileArchive = 375;
        const int magicMissileArchive = 379;
        const int poisonMissileArchive = 377;
        const int shockMissileArchive = 378;

        public const float SphereCastRadius = 0.25f;
        public const float TouchRange = 3.0f;

        Vector3 direction;
        Light myLight;
        SphereCollider myCollider;
        DaggerfallAudioSource audioSource;
        Rigidbody myRigidbody;
        Billboard myBillboard;
        bool forceDisableSpellLighting;
        bool noSpellsSpatialBlend = false;
        float lifespan = 0f;
        float postImpactLifespan = 0f;
        TargetTypes targetType = TargetTypes.None;
        ElementTypes elementType = ElementTypes.None;
        DaggerfallEntityBehaviour caster = null;
        bool missileReleased = false;
        bool impactDetected = false;
        bool impactAssigned = false;
        float initialRange;
        float initialIntensity;
        EntityEffectBundle payload;
        bool isArrow = false;
        bool isArrowSummoned = false;
        GameObject goModel = null;
        EnemySenses enemySenses;

        List<DaggerfallEntityBehaviour> targetEntities = new List<DaggerfallEntityBehaviour>();


        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets effect bundle payload carried by this missile.
        /// Any DaggerfallEntityBehaviour objects hit by this missile will
        /// receive instance of bundle payload against their EntityEffectManager on contact.
        /// </summary>
        public EntityEffectBundle Payload
        {
            get { return payload; }
            set { payload = value; }
        }

        /// <summary>
        /// Gets or sets target type.
        /// Target is set automatically from payload when available.
        /// </summary>
        public TargetTypes TargetType
        {
            get { return targetType; }
            set { targetType = value; }
        }

        /// <summary>
        /// Gets or sets element type.
        /// Element is set automatically from payload when available.
        /// </summary>
        public ElementTypes ElementType
        {
            get { return elementType; }
            set { elementType = value; }
        }

        /// <summary>
        /// Gets or sets caster who is origin of missile.
        /// This must be set for all missile target types.
        /// Caster is set automatically from payload when available.
        /// </summary>
        public DaggerfallEntityBehaviour Caster
        {
            get { return caster; }
            set { caster = value; }
        }

        public bool IsArrow
        {
            get { return isArrow; }
            set { isArrow = value; }
        }
        public bool IsArrowSummoned
        {
            get { return isArrowSummoned; }
            set { isArrowSummoned = value; }
        }

        /// <summary>
        /// Gets all target entities affected by this missile.
        /// Any effect bundle payload will be applied automatically.
        /// Use this property and OnComplete event for custom work.
        /// </summary>
        public DaggerfallEntityBehaviour[] Targets
        {
            get { return targetEntities.ToArray(); }
        }

        public Vector3 CustomAimPosition { get; set; }

        public Vector3 CustomAimDirection { get; set; }

        #endregion

        #region Unity

        private void Awake()
        {
            audioSource = transform.GetComponent<DaggerfallAudioSource>();
        }

        private void Start()
        {
            // Setup light and shadows
            myLight = GetComponent<Light>();
            myLight.enabled = EnableLight;
            forceDisableSpellLighting = !DaggerfallUnity.Settings.EnableSpellLighting;
            if (forceDisableSpellLighting) myLight.enabled = false;
            if (!DaggerfallUnity.Settings.EnableSpellShadows) myLight.shadows = LightShadows.None;
            initialRange = myLight.range;
            initialIntensity = myLight.intensity;

            // Setup collider
            myCollider = GetComponent<SphereCollider>();
            myCollider.radius = ColliderRadius;

            // Setup rigidbody
            myRigidbody = GetComponent<Rigidbody>();
            myRigidbody.useGravity = false;

            // Use payload when available
            if (payload != null)
            {
                // Set payload missile properties
                caster = payload.CasterEntityBehaviour;
                targetType = payload.Settings.TargetType;
                elementType = payload.Settings.ElementType;

                // Set spell billboard anims automatically from payload for mobile missiles
                if (targetType == TargetTypes.SingleTargetAtRange ||
                    targetType == TargetTypes.AreaAtRange)
                {
                    UseSpellBillboardAnims();
                }
            }

            // Setup senses
            if (caster && caster != GameManager.Instance.PlayerEntityBehaviour)
            {
                enemySenses = caster.GetComponent<EnemySenses>();
            }

            // Setup arrow
            if (isArrow)
            {
                // Create and orient 3d arrow
                goModel = GameObjectHelper.CreateDaggerfallMeshGameObject(99800, transform);
                MeshCollider arrowCollider = goModel.GetComponent<MeshCollider>();
                arrowCollider.sharedMesh = goModel.GetComponent<MeshFilter>().sharedMesh;
                arrowCollider.convex = true;
                arrowCollider.isTrigger = true;

                // Offset up so it comes from same place LOS check is done from
                Vector3 adjust;
                if (caster != GameManager.Instance.PlayerEntityBehaviour)
                {
                    CharacterController controller = caster.transform.GetComponent<CharacterController>();
                    adjust = caster.transform.forward * 0.6f;
                    adjust.y += controller.height / 3;
                }
                else
                {
                    // Adjust slightly downward to match bow animation
                    adjust = (GameManager.Instance.MainCamera.transform.rotation * -Caster.transform.up) * 0.11f;
                    // Offset forward to avoid collision with player
                    adjust += GameManager.Instance.MainCamera.transform.forward * 0.6f;
                    // Adjust to the right or left to match bow animation
                    if (!GameManager.Instance.WeaponManager.ScreenWeapon.FlipHorizontal)
                        adjust += GameManager.Instance.MainCamera.transform.right * 0.15f;
                    else
                        adjust -= GameManager.Instance.MainCamera.transform.right * 0.15f;
                }

                goModel.transform.localPosition = adjust;
                goModel.transform.rotation = Quaternion.LookRotation(GetAimDirection());
                goModel.layer = gameObject.layer;
            }

            // Ignore missile collision with caster (this is a different check to AOE targets)
            if (caster)
                Physics.IgnoreCollision(caster.GetComponent<Collider>(), this.GetComponent<Collider>());
        }

        private void Update()
        {
            // Execute based on target type
            if (!missileReleased)
            {
                switch (targetType)
                {
                    case TargetTypes.ByTouch:
                        DoTouch();
                        break;
                    case TargetTypes.SingleTargetAtRange:
                    case TargetTypes.AreaAtRange:
                        DoMissile();
                        break;
                    case TargetTypes.AreaAroundCaster:  // Must have a caster to perform area around caster
                        if (caster)
                            DoAreaOfEffect(caster.transform.position, true);
                        break;
                    default:
                        return;
                }
            }

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
                // Notify listeners work is done and automatically assign impact
                if (!impactAssigned)
                {
                    PlayImpactSound();
                    RaiseOnCompleteEvent();
                    if (!isArrow)
                        AssignPayloadToTargets();
                    impactAssigned = true;
                }

                // Track post impact lifespan and allow impact clip to finish playing
                postImpactLifespan += Time.deltaTime;
                if (postImpactLifespan > PostImpactLifespanInSeconds)
                {
                    myLight.enabled = false;
                    if (ImpactSound != SoundClips.None && !audioSource.IsPlaying())
                        Destroy(gameObject);
                }
            }

            // Update light
            UpdateLight();
        }

        #endregion

        #region Collision Handling

        private void OnCollisionEnter(Collision collision)
        {
            DoCollision(collision, null);
        }

        private void OnTriggerEnter(Collider other)
        {
            DoCollision(null, other);
        }

        void DoCollision(Collision collision, Collider other)
        {
            // Missile collision should only happen once
            if (impactDetected)
                return;

            // Set my collider to trigger and rigidbody to kinematic immediately after impact
            // This helps prevent mobiles from walking over low missiles or the missile bouncing off in some other direction
            // Seems to eliminate the combined worst-case scenario where mobile will "ride" a missile bounce, throwing them high into the air
            // Now the worst that seems to happen is mobile will "bump" over low missiles occasionally
            // TODO: Review later and find a better way to eliminate issue other than this quick workaround
            if (myCollider)
                myCollider.isTrigger = true;
            if (myRigidbody)
                myRigidbody.isKinematic = true;

            // Play spell impact animation, this replaces spell missile animation
            if (elementType != ElementTypes.None && targetType != TargetTypes.ByTouch)
            {
                UseSpellBillboardAnims(1, true);
                myBillboard.FramesPerSecond = ImpactBillboardFramesPerSecond;
                impactDetected = true;
            }

            // Get entity based on collision type
            DaggerfallEntityBehaviour entityBehaviour = null;
            if (collision != null && other == null)
                entityBehaviour = collision.gameObject.transform.GetComponent<DaggerfallEntityBehaviour>();
            else if (collision == null && other != null)
                entityBehaviour = other.gameObject.transform.GetComponent<DaggerfallEntityBehaviour>();
            else
                return;

            // If entity was hit then add to target list
            if (entityBehaviour)
            {
                targetEntities.Add(entityBehaviour);
                //Debug.LogFormat("Missile hit target {0} by range", entityBehaviour.name);
            }

            if (isArrow)
            {
                if (other != null)
                    AssignBowDamageToTarget(other);

                // Destroy 3d arrow
                Destroy(goModel.gameObject);
                impactDetected = true;
            }

            // If missile is area at range
            if (targetType == TargetTypes.AreaAtRange)
            {
                DoAreaOfEffect(transform.position);
            }
        }

        #endregion

        #region Static Methods

        public static DaggerfallEntityBehaviour GetEntityTargetInTouchRange(Vector3 aimPosition, Vector3 aimDirection)
        {
            // Fire ray along caster facing
            // Origin point of ray is set back slightly to fix issue where strikes against target capsules touching caster capsule do not connect
            RaycastHit hit;
            aimPosition -= aimDirection * 0.1f;
            Ray ray = new Ray(aimPosition, aimDirection);
            if (Physics.SphereCast(ray, SphereCastRadius, out hit, TouchRange))
                return hit.transform.GetComponent<DaggerfallEntityBehaviour>();
            else
                return null;
        }

        #endregion

        #region Private Methods

        // Touch can hit a single target at close range
        void DoTouch()
        {
            transform.position = caster.transform.position;

            // Touch does not use default missile collider
            // This prevent touch missile check colliding with self and blocking spell transfer
            if (myCollider)
                myCollider.enabled = false;

            DaggerfallEntityBehaviour entityBehaviour = GetEntityTargetInTouchRange(GetAimPosition(), GetAimDirection());
            if (entityBehaviour && entityBehaviour != caster)
            {
                targetEntities.Add(entityBehaviour);
                //Debug.LogFormat("Missile hit target {0} by touch", entityBehaviour.name);
            }

            // Touch always shows impact flash then expires
            missileReleased = true;
            impactDetected = true;
        }

        // Missile can hit environment or target at range
        void DoMissile()
        {
            direction = GetAimDirection();
            transform.position = GetAimPosition() + direction * ColliderRadius;
            missileReleased = true;
        }

        // AOE can strike any number of targets within range with an option to exclude caster
        void DoAreaOfEffect(Vector3 position, bool ignoreCaster = false)
        {
            List<DaggerfallEntityBehaviour> entities = new List<DaggerfallEntityBehaviour>();

            transform.position = position;

            // Collect AOE targets and ignore duplicates
            Collider[] overlaps = Physics.OverlapSphere(position, ExplosionRadius);
            for (int i = 0; i < overlaps.Length; i++)
            {
                DaggerfallEntityBehaviour aoeEntity = overlaps[i].GetComponent<DaggerfallEntityBehaviour>();

                if (ignoreCaster && aoeEntity == caster)
                    continue;

                if (aoeEntity && !targetEntities.Contains(aoeEntity))
                {
                    entities.Add(aoeEntity);
                    //Debug.LogFormat("Missile hit target {0} by AOE", aoeEntity.name);
                }
            }

            // Add collection to target entities
            if (entities.Count > 0)
                targetEntities.AddRange(entities);

            impactDetected = true;
            missileReleased = true;
        }

        // Get missile aim position from player or enemy mobile
        Vector3 GetAimPosition()
        {
            // Aim position from custom source
            if (CustomAimPosition != Vector3.zero)
                return CustomAimPosition;

            // Aim position is from eye level for player or origin for other mobile
            // Player must aim from camera position or it feels out of alignment
            Vector3 aimPosition = caster.transform.position;
            if (caster == GameManager.Instance.PlayerEntityBehaviour)
            {
                aimPosition = GameManager.Instance.MainCamera.transform.position;
            }

            return aimPosition;
        }

        // Get missile aim direction from player or enemy mobile
        Vector3 GetAimDirection()
        {
            // Aim direction from custom source
            if (CustomAimDirection != Vector3.zero)
                return CustomAimDirection;

            // Aim direction should be from camera for player or facing for other mobile
            Vector3 aimDirection = Vector3.zero;
            if (caster == GameManager.Instance.PlayerEntityBehaviour)
            {
                aimDirection = GameManager.Instance.MainCamera.transform.forward;
            }
            else if (enemySenses)
            {
                Vector3 predictedPosition;
                if (DaggerfallUnity.Settings.EnhancedCombatAI)
                    predictedPosition = enemySenses.PredictNextTargetPos(MovementSpeed);
                else
                    predictedPosition = enemySenses.LastKnownTargetPos;

                if (predictedPosition == EnemySenses.ResetPlayerPos)
                    aimDirection = caster.transform.forward;
                else
                    aimDirection = (predictedPosition - caster.transform.position).normalized;

                // Enemy archers must aim lower to compensate for crouched player capsule
                if (IsArrow && enemySenses.Target?.EntityType == EntityTypes.Player && GameManager.Instance.PlayerMotor.IsCrouching)
                    aimDirection += Vector3.down * 0.05f;
            }

            return aimDirection;
        }

        void UseSpellBillboardAnims(int record = 0, bool oneShot = false)
        {
            // Destroy any existing billboard game object
            if (myBillboard)
            {
                myBillboard.gameObject.SetActive(false);
                Destroy(myBillboard.gameObject);
            }

            // Add new billboard parented to this missile
            GameObject go = GameObjectHelper.CreateDaggerfallBillboardGameObject(GetMissileTextureArchive(), record, transform);
            go.transform.localPosition = Vector3.zero;
            go.layer = gameObject.layer;
            myBillboard = go.GetComponent<Billboard>();
            myBillboard.FramesPerSecond = BillboardFramesPerSecond;
            myBillboard.FaceY = true;
            myBillboard.OneShot = oneShot;
            myBillboard.GetComponent<MeshRenderer>().receiveShadows = false;
        }

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

        int GetMissileTextureArchive()
        {
            switch (elementType)
            {
                default:
                case ElementTypes.Cold:
                    return coldMissileArchive;
                case ElementTypes.Fire:
                    return fireMissileArchive;
                case ElementTypes.Magic:
                    return magicMissileArchive;
                case ElementTypes.Poison:
                    return poisonMissileArchive;
                case ElementTypes.Shock:
                    return shockMissileArchive;
            }
        }

        void AssignPayloadToTargets()
        {
            if (payload == null || targetEntities.Count == 0)
                return;

            foreach (DaggerfallEntityBehaviour entityBehaviour in targetEntities)
            {
                // Target must have an effect manager component
                EntityEffectManager effectManager = entityBehaviour.GetComponent<EntityEffectManager>();
                if (!effectManager)
                    continue;

                // Instantiate payload bundle on target
                effectManager.AssignBundle(payload, AssignBundleFlags.ShowNonPlayerFailures);
            }
        }

        void AssignBowDamageToTarget(Collider arrowHitCollider)
        {
            if (!isArrow || targetEntities.Count == 0)
            {
                return;
            }

            if (caster != GameManager.Instance.PlayerEntityBehaviour)
            {
                if (targetEntities[0] == caster.GetComponent<EnemySenses>().Target)
                {
                    EnemyAttack attack = caster.GetComponent<EnemyAttack>();
                    if (attack)
                    {
                        attack.BowDamage(goModel.transform.forward);
                    }
                }
            }
            else
            {
                Transform hitTransform = arrowHitCollider.gameObject.transform;
                GameManager.Instance.WeaponManager.WeaponDamage(GameManager.Instance.WeaponManager.LastBowUsed, true, isArrowSummoned, hitTransform, hitTransform.position, goModel.transform.forward);
            }
        }

        void PlayImpactSound()
        {
            if (audioSource && ImpactSound != SoundClips.None)
            {
                // Classic does not appear to use 3D sound for spell impact at all
                float spatialBlend = !isArrow && noSpellsSpatialBlend ? 0f : 1f;
                audioSource.PlayOneShot(ImpactSound, spatialBlend);
            }
        }

        #endregion

        #region Events

        // OnComplete
        public delegate void OnCompleteEventHandler();
        public static event OnCompleteEventHandler OnComplete;
        protected virtual void RaiseOnCompleteEvent()
        {
            if (OnComplete != null)
                OnComplete();
        }

        #endregion
    }
}
