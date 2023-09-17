using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Utility;
using UnityEngine;

namespace Game.Pet
{
    public class PetSenses : MonoBehaviour
    {
        private static readonly Vector3 ResetPlayerPos = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        private const float PredictionInterval = 0.0625f;
        private const float SystemTimerUpdatesDivisor = .0549254f;
        private const float ClassicSpawnDespawnExterior = 4096 * MeshReader.GlobalScale;

        [SerializeField] public float sightRadius = 4096 * MeshReader.GlobalScale;
        [SerializeField] public float hearingRadius = 25f;
        [SerializeField] public float fieldOfView = 180f;

        private MobileUnit _mobile;
        private DaggerfallEntityBehaviour _entityBehaviour;
        private PetEntity _enemyEntity;
        private DaggerfallEntityBehaviour _player;
        private DaggerfallEntityBehaviour _target;
        private DaggerfallActionDoor _actionDoor;
        private CharacterController _characterController;
        private Vector3 _directionToTarget;
        private Vector3 _lastKnownTargetPos = ResetPlayerPos;
        private Vector3 _oldLastKnownTargetPos = ResetPlayerPos;
        private Vector3 _predictedTargetPos = ResetPlayerPos;
        private Vector3 _predictedTargetPosWithoutLead = ResetPlayerPos;
        private Vector3 _lastPositionDiff;
        private bool _awareOfTargetForLastPrediction;
        private float _distanceToActionDoor;
        private bool _hasEncounteredPlayer;
        private bool _wouldBeSpawnedInClassic;
        private uint _timeOfLastStealthCheck;
        private float _lastHadLosTimer;
        private float _targetPosPredictTimer;
        private bool _targetInSight;
        private bool _playerInSight;
        private bool _targetInEarshot;
        private float _distanceToPlayer;
        private float _distanceToTarget;
        private bool _targetPosPredict;
        private float _classicTargetUpdateTimer;
        private float _classicSpawnXZDist;
        private float _classicSpawnYDistUpper;
        private float _classicSpawnYDistLower;
        private float _classicDespawnXZDist;
        private float _classicDespawnYDist;

        public DaggerfallEntityBehaviour Target => _target;
        public bool TargetInEarshot => _targetInEarshot;
        public Vector3 DirectionToTarget => _directionToTarget;
        public bool TargetInSight => _targetInSight;
        public bool DetectedTarget { get; set; }
        public float DistanceToPlayer => _distanceToPlayer;
        public float DistanceToTarget => _distanceToTarget;
        public Vector3 LastKnownTargetPos => _lastKnownTargetPos;
        public Vector3 LastPositionDiff => _lastPositionDiff;
        public Vector3 PredictedTargetPos => _predictedTargetPos;

        public DaggerfallActionDoor LastKnownDoor
        {
            get => _actionDoor;
            set => _actionDoor = value;
        }

        public float DistanceToDoor
        {
            get => _distanceToActionDoor;
            set => _distanceToActionDoor = value;
        }

        private void Start()
        {
            _mobile = GetComponent<DaggerfallEnemy>().MobileUnit;
            _entityBehaviour = GetComponent<DaggerfallEntityBehaviour>();
            _enemyEntity = _entityBehaviour.Entity as PetEntity;
            _characterController = GetComponent<CharacterController>();
            _player = GameManager.Instance.PlayerEntityBehaviour;

            short[] classicSpawnXZDistArray = {1024, 384, 640, 768, 768, 768, 768};
            short[] classicSpawnYDistUpperArray = {128, 128, 128, 384, 768, 128, 256};
            short[] classicSpawnYDistLowerArray = {0, 0, 0, 0, -128, -768, 0};
            short[] classicDespawnXZDistArray = {1024, 1024, 1024, 1024, 768, 768, 768};
            short[] classicDespawnYDistArray = {384, 384, 384, 384, 768, 768, 768};

            var index = _mobile.ClassicSpawnDistanceType;

            _classicSpawnXZDist = classicSpawnXZDistArray[index] * MeshReader.GlobalScale;
            _classicSpawnYDistUpper = classicSpawnYDistUpperArray[index] * MeshReader.GlobalScale;
            _classicSpawnYDistLower = classicSpawnYDistLowerArray[index] * MeshReader.GlobalScale;
            _classicDespawnXZDist = classicDespawnXZDistArray[index] * MeshReader.GlobalScale;
            _classicDespawnYDist = classicDespawnYDistArray[index] * MeshReader.GlobalScale;

            if (DaggerfallUnity.Settings.EnhancedCombatAI)
                fieldOfView = 190;
        }

        private void FixedUpdate()
        {
            _targetPosPredictTimer += Time.deltaTime;
            if (_targetPosPredictTimer >= PredictionInterval)
            {
                _targetPosPredictTimer = 0f;
                _targetPosPredict = true;
            }
            else
                _targetPosPredict = false;

            if (GameManager.ClassicUpdate)
            {
                if (_distanceToPlayer < 1094 * MeshReader.GlobalScale)
                {
                    float upperXZ;
                    float upperY = 0;
                    float lowerY = 0;
                    var playerInside = GameManager.Instance.PlayerGPS.GetComponent<PlayerEnterExit>().IsPlayerInside;

                    if (!playerInside)
                    {
                        upperXZ = ClassicSpawnDespawnExterior;
                    }
                    else
                    {
                        if (!_wouldBeSpawnedInClassic)
                        {
                            upperXZ = _classicSpawnXZDist;
                            upperY = _classicSpawnYDistUpper;
                            lowerY = _classicSpawnYDistLower;
                        }
                        else
                        {
                            upperXZ = _classicDespawnXZDist;
                            upperY = _classicDespawnYDist;
                        }
                    }

                    var yDiffToPlayer = transform.position.y - _player.transform.position.y;
                    var yDiffToPlayerAbs = Mathf.Abs(yDiffToPlayer);
                    var distanceToPlayerXZ =
                        Mathf.Sqrt(_distanceToPlayer * _distanceToPlayer - yDiffToPlayerAbs * yDiffToPlayerAbs);

                    _wouldBeSpawnedInClassic = true;

                    if (distanceToPlayerXZ > upperXZ)
                        _wouldBeSpawnedInClassic = false;

                    if (playerInside)
                    {
                        if (lowerY == 0)
                        {
                            if (yDiffToPlayerAbs > upperY)
                                _wouldBeSpawnedInClassic = false;
                        }
                        else if (yDiffToPlayer < lowerY || yDiffToPlayer > upperY)
                            _wouldBeSpawnedInClassic = false;
                    }
                }
                else
                    _wouldBeSpawnedInClassic = false;
            }

            if (GameManager.ClassicUpdate)
            {
                _classicTargetUpdateTimer += Time.deltaTime / SystemTimerUpdatesDivisor;

                if (_target != null && _target.Entity.CurrentHealth <= 0)
                {
                    _target = null;
                }

                if (_target == null)
                {
                    _lastKnownTargetPos = ResetPlayerPos;
                    _predictedTargetPos = ResetPlayerPos;
                    _directionToTarget = ResetPlayerPos;
                    _distanceToTarget = 0;
                }
            }

            if (_player != null)
            {
                var toPlayer = _player.transform.position - transform.position;
                _distanceToPlayer = toPlayer.magnitude;

                if (!_wouldBeSpawnedInClassic)
                {
                    _distanceToTarget = _distanceToPlayer;
                    _directionToTarget = toPlayer.normalized;
                    _playerInSight = CanSeeTarget(_player);
                }

                if (_classicTargetUpdateTimer > 5)
                {
                    _classicTargetUpdateTimer = 0f;

                    if (_wouldBeSpawnedInClassic || _playerInSight)
                    {
                        GetPlayerTarget();
                    }
                }

                if (_target == null)
                {
                    _targetInSight = false;
                    DetectedTarget = false;
                    return;
                }

                if (!_wouldBeSpawnedInClassic && _target == _player)
                {
                    _distanceToTarget = _distanceToPlayer;
                    _directionToTarget = toPlayer.normalized;
                    _targetInSight = _playerInSight;
                }
                else
                {
                    var toTarget = _target.transform.position - transform.position;
                    _distanceToTarget = toTarget.magnitude;
                    _directionToTarget = toTarget.normalized;
                    _targetInSight = CanSeeTarget(_target);
                }

                if (DetectedTarget && !_targetInSight)
                    _targetInEarshot = CanHearTarget();
                else
                    _targetInEarshot = false;

                if (GameManager.ClassicUpdate)
                {
                    if (_lastHadLosTimer > 0)
                        _lastHadLosTimer--;
                }

                if (_targetInSight || _targetInEarshot)
                {
                    DetectedTarget = true;
                    _lastKnownTargetPos = _target.transform.position;
                    _lastHadLosTimer = 200f;
                }
                else
                {
                    DetectedTarget = true;

                    if (_lastHadLosTimer <= 0)
                        _lastKnownTargetPos = _target.transform.position;
                }

                if (_oldLastKnownTargetPos == ResetPlayerPos)
                    _oldLastKnownTargetPos = _lastKnownTargetPos;

                if (_predictedTargetPos == ResetPlayerPos || !DaggerfallUnity.Settings.EnhancedCombatAI)
                    _predictedTargetPos = _lastKnownTargetPos;

                // Predict target's next position
                if (_targetPosPredict && _lastKnownTargetPos != ResetPlayerPos)
                {
                    // Be sure to only take difference of movement if we've seen the target for two consecutive prediction updates
                    if (_targetInSight)
                    {
                        if (_awareOfTargetForLastPrediction)
                            _lastPositionDiff = _lastKnownTargetPos - _oldLastKnownTargetPos;

                        // Store current last known target position for next prediction update
                        _oldLastKnownTargetPos = _lastKnownTargetPos;

                        _awareOfTargetForLastPrediction = true;
                    }
                    else
                    {
                        _awareOfTargetForLastPrediction = false;
                    }

                    if (DaggerfallUnity.Settings.EnhancedCombatAI)
                    {
                        var moveSpeed = (_enemyEntity.Stats.LiveSpeed + PlayerSpeedChanger.dfWalkBase) *
                                        MeshReader.GlobalScale;
                        _predictedTargetPos = PredictNextTargetPos(moveSpeed);
                    }
                }

                if (DetectedTarget && !_hasEncounteredPlayer && _target == _player)
                {
                    _hasEncounteredPlayer = true;
                }
            }
        }

        public Vector3 PredictNextTargetPos(float interceptSpeed)
        {
            Vector3 assumedCurrentPosition;
            RaycastHit tempHit;

            if (_predictedTargetPosWithoutLead == ResetPlayerPos)
            {
                _predictedTargetPosWithoutLead = _lastKnownTargetPos;
            }

            // If aware of target, if distance is too far or can see nothing is there, use last known position as assumed current position
            if (_targetInSight || _targetInEarshot || (_predictedTargetPos - transform.position).magnitude >
                sightRadius + _mobile.Enemy.SightModifier
                || !Physics.Raycast(transform.position,
                    (_predictedTargetPosWithoutLead - transform.position).normalized,
                    out tempHit, sightRadius + _mobile.Enemy.SightModifier))
            {
                assumedCurrentPosition = _lastKnownTargetPos;
            }
            // If not aware of target and predicted position may still be good, use predicted position
            else
            {
                assumedCurrentPosition = _predictedTargetPosWithoutLead;
            }

            var divisor = PredictionInterval;

            // Account for mid-interval call by DaggerfallMissile
            if (_targetPosPredictTimer != 0)
            {
                divisor = _targetPosPredictTimer;
                _lastPositionDiff = _lastKnownTargetPos - _oldLastKnownTargetPos;
            }

            // Let's solve cone / line intersection (quadratic equation)
            var d = assumedCurrentPosition - transform.position;
            var v = _lastPositionDiff / divisor;
            var a = v.sqrMagnitude - interceptSpeed * interceptSpeed;
            var b = 2 * Vector3.Dot(d, v);
            var c = d.sqrMagnitude;

            var prediction = assumedCurrentPosition;

            float t = -1;
            if (Mathf.Abs(a) >= 1e-5)
            {
                var disc = b * b - 4 * a * c;
                if (disc >= 0)
                {
                    // find the minimal positive solution
                    var discSqrt = Mathf.Sqrt(disc) * Mathf.Sign(a);
                    t = (-b - discSqrt) / (2 * a);
                    if (t < 0)
                        t = (-b + discSqrt) / (2 * a);
                }
            }
            else
            {
                // degenerated cases
                if (Mathf.Abs(b) >= 1e-5)
                    t = -d.sqrMagnitude / b;
            }

            if (t >= 0)
            {
                prediction = assumedCurrentPosition + v * t;

                // Don't predict target will move through obstacles (prevent predicting movement through walls)
                RaycastHit hit;
                var ray = new Ray(assumedCurrentPosition, (prediction - assumedCurrentPosition).normalized);
                if (Physics.Raycast(ray, out hit, (prediction - assumedCurrentPosition).magnitude))
                    prediction = assumedCurrentPosition;
            }

            // Store prediction minus lead for next prediction update
            _predictedTargetPosWithoutLead = assumedCurrentPosition + _lastPositionDiff;

            return prediction;
        }

        public bool TargetIsWithinYawAngle(float targetAngle, Vector3 targetPos)
        {
            var toTarget = targetPos - transform.position;
            toTarget.y = 0;

            var enemyDirection2D = transform.forward;
            enemyDirection2D.y = 0;

            return Vector3.Angle(toTarget, enemyDirection2D) < targetAngle;
        }

        public bool TargetIsWithinPitchAngle(float targetAngle)
        {
            var toTarget = _predictedTargetPos - transform.position;
            var directionToLastKnownTarget2D = toTarget.normalized;
            var verticalTransformToLastKnownPos =
                new Plane(_predictedTargetPos, transform.position, transform.position + Vector3.up);
            var enemyDirection2D = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
            enemyDirection2D = Vector3.ProjectOnPlane(enemyDirection2D, verticalTransformToLastKnownPos.normal);
            var angle = Vector3.Angle(directionToLastKnownTarget2D, enemyDirection2D);

            return angle < targetAngle;
        }

        public bool TargetIsAbove() =>
            _predictedTargetPos.y > transform.position.y;

        private void GetPlayerTarget()
        {
            var toTarget = _player.transform.position - transform.position;
            _directionToTarget = toTarget.normalized;
            _distanceToTarget = toTarget.magnitude;
            _targetInSight = CanSeeTarget(_player);
            _target = _player;
        }

        public bool CanSeeTarget(DaggerfallEntityBehaviour target)
        {
            var seen = false;
            _actionDoor = null;

            if (_distanceToTarget < sightRadius + _mobile.Enemy.SightModifier)
            {
                // Check if target in field of view
                var angle = Vector3.Angle(_directionToTarget, transform.forward);
                if (angle < fieldOfView * 0.5f)
                {
                    // Check if line of sight to target
                    RaycastHit hit;

                    // Set origin of ray to approximate eye position
                    var eyePos = transform.position + _characterController.center;
                    eyePos.y += _characterController.height / 3;

                    // Set destination to the target's approximate eye position
                    _characterController = target.transform.GetComponent<CharacterController>();
                    var targetEyePos = target.transform.position + _characterController.center;
                    targetEyePos.y += _characterController.height / 3;

                    // Check if can see.
                    var eyeToTarget = targetEyePos - eyePos;
                    var eyeDirectionToTarget = eyeToTarget.normalized;
                    var ray = new Ray(eyePos, eyeDirectionToTarget);

                    if (Physics.Raycast(ray, out hit, sightRadius))
                    {
                        // Check if hit was target
                        var entity =
                            hit.transform.gameObject.GetComponent<DaggerfallEntityBehaviour>();
                        if (entity == target)
                            seen = true;

                        // Check if hit was an action door
                        var door = hit.transform.gameObject.GetComponent<DaggerfallActionDoor>();
                        if (door != null)
                        {
                            _actionDoor = door;
                            _distanceToActionDoor =
                                Vector3.Distance(transform.position, _actionDoor.transform.position);
                        }
                    }
                }
            }

            return seen;
        }

        private bool CanHearTarget()
        {
            const float hearingScale = 1f;

            // If something is between enemy and target then return false (was reduce hearingScale by half), to minimize
            // enemies walking against walls.
            // Hearing is not impeded by doors or other non-static objects
            RaycastHit hit;
            var ray = new Ray(transform.position, _directionToTarget);
            if (Physics.Raycast(ray, out hit))
            {
                if (GameObjectHelper.IsStaticGeometry(hit.transform.gameObject))
                    return false;
            }

            return _distanceToTarget < (hearingRadius * hearingScale) + _mobile.Enemy.HearingModifier;
        }
    }
}