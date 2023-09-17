using UnityEngine;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.MagicAndEffects;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;

namespace Game.Pet
{
    [RequireComponent(typeof(PetSenses))]
    [RequireComponent(typeof(EnemyBlood))]
    [RequireComponent(typeof(EnemySounds))]
    [RequireComponent(typeof(CharacterController))]
    public class PetMotor : MonoBehaviour
    {
        [SerializeField] private float maxApproachDistance;
        [SerializeField] private float openDoorDistance = 2f;

        private float _stopDistance = 1.7f;
        private const float DoorCrouchingHeight = 1.65f;
        private bool _flies;
        private bool _swims;
        private bool _pausePursuit;
        private float _moveInForAttackTimer;
        private bool _moveInForAttack;
        private float _retreatDistanceMultiplier;
        private float _changeStateTimer;
        private bool _doStrafe;
        private float _strafeTimer;
        private bool _pursuing;
        private bool _retreating;
        private bool _backingUp;
        private bool _fallDetected;
        private bool _obstacleDetected;
        private bool _foundUpwardSlope;
        private bool _foundDoor;
        private bool _rotating;
        private float _avoidObstaclesTimer;
        private bool _checkingClockwise;
        private float _checkingClockwiseTimer;
        private bool _didClockwiseCheck;
        private float _lastTimeWasStuck;
        private float _realHeight;
        private float _centerChange;
        private bool _resetHeight;
        private float _heightChangeTimer;
        private bool _strafeLeft;
        private float _strafeAngle;
        private int _searchMult;
        private int _ignoreMaskForShooting;
        private int _ignoreMaskForObstacles;
        private bool _canAct;
        private bool _falls;
        private bool _flyerFalls;
        private float _lastGroundedY;
        private float _originalHeight;
        private Vector3 _lastPosition;
        private Vector3 _lastDirection;
        private Vector3 _destination;
        private Vector3 _detourDestination;
        private PetSenses _senses;
        private CharacterController _controller;
        private MobileUnit _mobile;
        private DaggerfallEntityBehaviour _entityBehaviour;
        private EnemyBlood _entityBlood;
        private EntityEffectBundle _selectedSpell;
        private PetEntity _entity;

        public bool IsLevitating { get; set; }
        public float KnockbackSpeed { get; private set; }
        public Vector3 KnockbackDirection { get; set; }
        public bool Bashing { get; private set; }

        private void Start()
        {
            _senses = GetComponent<PetSenses>();
            _controller = GetComponent<CharacterController>();
            _mobile = GetComponentInChildren<MobileUnit>();
            _flies = CanFly();
            _swims = _mobile.Enemy.Behaviour == MobileBehaviour.Aquatic;
            _entityBehaviour = GetComponent<DaggerfallEntityBehaviour>();
            _entityBlood = GetComponent<EnemyBlood>();
            _entity = _entityBehaviour.Entity as PetEntity;

            // Add things AI should ignore when checking for a clear path to shoot.
            _ignoreMaskForShooting = ~(1 << LayerMask.NameToLayer("SpellMissiles") |
                                       1 << LayerMask.NameToLayer("Ignore Raycast"));

            // Also ignore arrows and "Ignore Raycast" layer for obstacles
            _ignoreMaskForObstacles = ~(1 << LayerMask.NameToLayer("SpellMissiles") |
                                        1 << LayerMask.NameToLayer("Ignore Raycast"));

            _lastGroundedY = transform.position.y;
            _originalHeight = _controller.height;
        }

        private void FixedUpdate()
        {
            _flies = CanFly();
            _canAct = true;
            _flyerFalls = false;
            _falls = false;

            KnockbackMovement();
            ApplyGravity();
            HandleNoAction();
            UpdateTimers();
            TakeAction();
            ApplyFallDamage();
            UpdateToIdleOrMoveAnim();
            OpenDoors();
            HeightAdjust();
        }

        /// <summary>
        /// Attempts to find the ground position below enemy, even if player is flying/falling
        /// </summary>
        /// <param name="distance">Distance to fire ray.</param>
        /// <returns>Hit point on surface below enemy, or enemy position if hit not found in distance.</returns>
        public Vector3 FindGroundPosition(float distance = 16)
        {
            RaycastHit hit;
            var ray = new Ray(transform.position, Vector3.down);
            if (Physics.Raycast(ray, out hit, distance))
                return hit.point;

            return transform.position;
        }

        /// <summary>
        /// Call this when floating origin ticks on Y to ensure enemy doesn't die from large "grounded" difference
        /// </summary>
        /// <param name="y">Amount to increment to fallstart</param>
        public void AdjustLastGrounded(float y)
        {
            _lastGroundedY += y;
        }

        /// <summary>
        /// Handles movement if the enemy has been knocked back.
        /// </summary>
        private void KnockbackMovement()
        {
            // Prevent stunlocking transforming Seducers
            if (_mobile.EnemyState == MobileStates.SeducerTransform1 ||
                _mobile.EnemyState == MobileStates.SeducerTransform2)
                return;

            // If hit, get knocked back
            if (KnockbackSpeed > 0)
            {
                // Limit KnockbackSpeed. This can be higher than what is actually used for the speed of motion,
                // making it last longer and do more damage if the enemy collides with something (TODO).
                if (KnockbackSpeed > (40 / (PlayerSpeedChanger.classicToUnitySpeedUnitRatio / 10)))
                    KnockbackSpeed = (40 / (PlayerSpeedChanger.classicToUnitySpeedUnitRatio / 10));

                if (KnockbackSpeed > (5 / (PlayerSpeedChanger.classicToUnitySpeedUnitRatio / 10)) &&
                    _mobile.EnemyState != MobileStates.PrimaryAttack)
                {
                    _mobile.ChangeEnemyState(MobileStates.Hurt);
                }

                // Actual speed of motion is limited
                Vector3 motion;
                if (KnockbackSpeed <= (25 / (PlayerSpeedChanger.classicToUnitySpeedUnitRatio / 10)))
                    motion = KnockbackDirection * KnockbackSpeed;
                else
                    motion = KnockbackDirection * (25 / (PlayerSpeedChanger.classicToUnitySpeedUnitRatio / 10));

                // Move in direction of knockback
                if (_swims)
                    WaterMove(motion);
                else if (_flies || IsLevitating)
                    _controller.Move(motion * Time.deltaTime);
                else
                    _controller.SimpleMove(motion);

                // Remove remaining knockback and restore animation
                if (GameManager.ClassicUpdate)
                {
                    KnockbackSpeed -= (5 / (PlayerSpeedChanger.classicToUnitySpeedUnitRatio / 10));
                    if (KnockbackSpeed <= (5 / (PlayerSpeedChanger.classicToUnitySpeedUnitRatio / 10))
                        && _mobile.EnemyState != MobileStates.PrimaryAttack)
                    {
                        _mobile.ChangeEnemyState(MobileStates.Move);
                    }
                }

                _moveInForAttack = true;
                _canAct = false;
                _flyerFalls = true;
            }
        }

        /// <summary>
        /// Apply gravity to ground-based enemies and paralyzed flyers.
        /// </summary>
        private void ApplyGravity()
        {
            // Apply gravity
            if (!_flies && !_swims && !IsLevitating && !_controller.isGrounded)
            {
                _controller.SimpleMove(Vector3.zero);
                _falls = true;

                // Only cancel movement if actually falling. Sometimes mobiles can get stuck where they are !isGrounded but SimpleMove(Vector3.zero) doesn't help.
                // Allowing them to continue and attempt a Move() frees them, but we don't want to allow that if we can avoid it so they aren't moving
                // while falling, which can also accelerate the fall due to anti-bounce downward movement in Move().
                if (_lastPosition != transform.position)
                    _canAct = false;
            }

            if (_flyerFalls && _flies && !IsLevitating)
            {
                _controller.SimpleMove(Vector3.zero);
                _falls = true;
            }
        }

        /// <summary>
        /// Do nothing if no target or after giving up finding the target or if target position hasn't been acquired yet.
        /// </summary>
        private void HandleNoAction()
        {
            if (_senses.Target == null || _senses.PredictedTargetPos == EnemySenses.ResetPlayerPos)
            {
                SetChangeStateTimer();
                _searchMult = 0;

                _canAct = false;
            }
        }

        /// <summary>
        /// Updates timers used in this class.
        /// </summary>
        private void UpdateTimers()
        {
            if (_moveInForAttackTimer > 0)
                _moveInForAttackTimer -= Time.deltaTime;

            if (_avoidObstaclesTimer > 0)
                _avoidObstaclesTimer -= Time.deltaTime;

            // Set avoidObstaclesTimer to 0 if got close enough to detourDestination. Only bother checking if possible to move.
            if (_avoidObstaclesTimer > 0 && _canAct)
            {
                var detourDestination2D = _detourDestination;
                detourDestination2D.y = transform.position.y;
                if ((detourDestination2D - transform.position).magnitude <= 0.3f)
                {
                    _avoidObstaclesTimer = 0;
                }
            }

            if (_checkingClockwiseTimer > 0)
                _checkingClockwiseTimer -= Time.deltaTime;

            if (_changeStateTimer > 0)
                _changeStateTimer -= Time.deltaTime;

            if (_strafeTimer > 0)
                _strafeTimer -= Time.deltaTime;
        }

        /// <summary>
        /// Make decision about what action to take.
        /// </summary>
        private void TakeAction()
        {
            // Monster speed of movement follows the same formula as for when the player walks
            var moveSpeed = (_entity.Stats.LiveSpeed + PlayerSpeedChanger.dfWalkBase) * MeshReader.GlobalScale;

            _stopDistance = maxApproachDistance;

            GetDestination();

            // Get direction & distance to destination.
            var direction = (_destination - transform.position).normalized;

            float distance;
            if (_avoidObstaclesTimer <= 0 && _senses.TargetInSight)
                distance = _senses.DistanceToTarget;
            else
                distance = (_destination - transform.position).magnitude;

            _moveInForAttack = true;

            // If detouring, always attempt to move
            if (_avoidObstaclesTimer > 0)
            {
                AttemptMove(direction, moveSpeed);
            }
            // Otherwise, if not still executing a retreat, approach target until close enough to be on-guard.
            // If decided to move in for attack, continue until within melee range. Classic always moves in for attack.
            else if ((!_retreating && distance >= (_stopDistance * 2.75)) ||
                     (distance > _stopDistance && _moveInForAttack))
            {
                // If state change timer is done, or we are continuing an already started pursuit, we can move immediately
                if (_changeStateTimer <= 0 || _pursuing)
                    AttemptMove(direction, moveSpeed);
                // Otherwise, look at target until timer finishes
                else if (!_senses.TargetIsWithinYawAngle(22.5f, _destination))
                    TurnToTarget(direction);
            }
            else if (DaggerfallUnity.Settings.EnhancedCombatAI && _strafeTimer <= 0)
            {
                StrafeDecision();
            }
            else if (_doStrafe && _strafeTimer > 0 && (distance >= _stopDistance * .8f))
            {
                AttemptMove(direction, moveSpeed / 4, false, true, distance);
            }
            // Back away from combat target if right next to it, or if decided to retreat and enemy is too close.
            // Classic AI never backs away.
            else if (DaggerfallUnity.Settings.EnhancedCombatAI && _senses.TargetInSight &&
                     (distance < _stopDistance * .8f ||
                      !_moveInForAttack && distance < _stopDistance * _retreatDistanceMultiplier &&
                      (_changeStateTimer <= 0 || _retreating)))
            {
                // If state change timer is done, or we are already executing a retreat, we can move immediately
                if (_changeStateTimer <= 0 || _retreating)
                    AttemptMove(direction, moveSpeed / 2, true);
            }
            // Not moving, just look at target
            else if (!_senses.TargetIsWithinYawAngle(22.5f, _destination))
            {
                TurnToTarget(direction);
            }
            else // Not moving, and no need to turn
            {
                SetChangeStateTimer();
                _pursuing = false;
                _retreating = false;
            }
        }

        /// <summary>
        /// Get the destination to move towards.
        /// </summary>
        private void GetDestination()
        {
            var targetController = _senses.Target.GetComponent<CharacterController>();
            // If detouring around an obstacle or fall, use the detour position
            if (_avoidObstaclesTimer > 0)
            {
                _destination = _detourDestination;
            }
            // Otherwise, try to get to the combat target if there is a clear path to it
            else if (ClearPathToPosition(_senses.PredictedTargetPos, (_destination - transform.position).magnitude) ||
                     (_senses.TargetInSight && _entity.CurrentMagicka > 0))
            {
                _destination = _senses.PredictedTargetPos;
                // Flying enemies and slaughterfish aim for target face
                if (_flies || IsLevitating || (_swims && _mobile.Enemy.ID == (int) MonsterCareers.Slaughterfish))
                    _destination.y += targetController.height * 0.5f;

                _searchMult = 0;
            }
            // Otherwise, search for target based on its last known position and direction
            else
            {
                var searchPosition =
                    _senses.LastKnownTargetPos + (_senses.LastPositionDiff.normalized * _searchMult);
                if (_searchMult <= 10 && (searchPosition - transform.position).magnitude <= _stopDistance)
                    _searchMult++;

                _destination = searchPosition;
            }

            if (_avoidObstaclesTimer <= 0 && !_flies && !IsLevitating && !_swims && _senses.Target)
            {
                // Ground enemies target at their own height
                // Otherwise, short enemies' vector can aim up towards the target, which could interfere with distance-to-target calculations.
                var deltaHeight = (targetController.height - _originalHeight) / 2;
                _destination.y -= deltaHeight;
            }
        }

        /// <summary>
        /// Decide whether to strafe, and get direction to strafe to.
        /// </summary>
        private void StrafeDecision()
        {
            _doStrafe = Random.Range(0, 4) == 0;
            _strafeTimer = Random.Range(1f, 2f);
            if (_doStrafe)
            {
                if (Random.Range(0, 2) == 0)
                    _strafeLeft = true;
                else
                    _strafeLeft = false;

                var north = _destination;
                north.z++; // Adding 1 to z so this Vector3 will be north of the destination Vector3.

                // Get angle between vector from destination to the north of it, and vector from destination to this enemy's position
                _strafeAngle = Vector3.SignedAngle(_destination - north, _destination - transform.position, Vector3.up);
                if (_strafeAngle < 0)
                    _strafeAngle = 360 + _strafeAngle;

                // Convert to radians
                _strafeAngle *= Mathf.PI / 180;
            }
        }

        /// <summary>
        /// Returns whether there is a clear path to move the given distance from the current location towards the given location. True if clear
        /// or if combat target is the first obstacle hit.
        /// </summary>
        private bool ClearPathToPosition(Vector3 location, float dist = 30)
        {
            var sphereCastDir = (location - transform.position).normalized;
            var sphereCastDir2d = sphereCastDir;
            sphereCastDir2d.y = 0;
            ObstacleCheck(sphereCastDir2d);
            FallCheck(sphereCastDir2d);

            if (_obstacleDetected || _fallDetected)
                return false;

            RaycastHit hit;
            if (Physics.SphereCast(transform.position, _controller.radius / 2, sphereCastDir, out hit, dist,
                    _ignoreMaskForShooting))
            {
                var hitTarget = hit.transform.GetComponent<DaggerfallEntityBehaviour>();
                if (hitTarget == _senses.Target)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Checks if enemy can fly based on behaviour.
        /// This can change in the case of a transformed Seducer.
        /// </summary>
        /// <returns>True if enemy can fly.</returns>
        private bool CanFly()
        {
            return _mobile.Enemy.Behaviour == MobileBehaviour.Flying ||
                   _mobile.Enemy.Behaviour == MobileBehaviour.Spectral;
        }

        /// <summary>
        /// Try to move in given direction.
        /// </summary>
        private void AttemptMove(Vector3 direction, float moveSpeed, bool backAway = false, bool strafe = false,
            float strafeDist = 0)
        {
            // Set whether pursuing or retreating, for bypassing changeStateTimer delay when continuing these actions
            if (!backAway && !strafe)
            {
                _pursuing = true;
                _retreating = false;
            }
            else
            {
                _retreating = true;
                _pursuing = false;
            }

            if (!_senses.TargetIsWithinYawAngle(5.625f, _destination))
            {
                TurnToTarget(direction);
                // Classic always turns in place. Enhanced only does so if enemy is not in sight,
                // for more natural-looking movement while pursuing.
                if (!DaggerfallUnity.Settings.EnhancedCombatAI || !_senses.TargetInSight)
                    return;
            }

            if (backAway)
                direction *= -1;

            if (strafe)
            {
                var strafeDest = new Vector3(_destination.x + (Mathf.Sin(_strafeAngle) * strafeDist),
                    transform.position.y, _destination.z + (Mathf.Cos(_strafeAngle) * strafeDist));
                direction = (strafeDest - transform.position).normalized;

                if ((strafeDest - transform.position).magnitude <= 0.2f)
                {
                    if (_strafeLeft)
                        _strafeAngle++;
                    else
                        _strafeAngle--;
                }
            }

            // Move downward some to eliminate bouncing down inclines
            if (!_flies && !_swims && !IsLevitating && _controller.isGrounded)
                direction.y = -2f;

            // Stop fliers from moving too near the floor during combat
            if (_flies && _avoidObstaclesTimer <= 0 && direction.y < 0 &&
                FindGroundPosition((_originalHeight / 2) + 1f) != transform.position)
                direction.y = 0.1f;

            var motion = direction * moveSpeed;

            // If using enhanced combat, avoid moving directly below targets
            if (!backAway && DaggerfallUnity.Settings.EnhancedCombatAI && _avoidObstaclesTimer <= 0)
            {
                var withinPitch = _senses.TargetIsWithinPitchAngle(45.0f);
                if (!_pausePursuit && !withinPitch)
                {
                    if (_flies || IsLevitating || _swims)
                    {
                        if (!_senses.TargetIsAbove())
                            motion = -transform.up * moveSpeed / 2;
                        else
                            motion = transform.up * moveSpeed;
                    }
                    // Causes a random delay after being out of pitch range
                    else if (_senses.TargetIsAbove() && _changeStateTimer <= 0)
                    {
                        SetChangeStateTimer();
                        _pausePursuit = true;
                    }
                }
                else if (withinPitch)
                {
                    _pausePursuit = false;
                    _backingUp = false;
                }

                if (_pausePursuit)
                {
                    if (_senses.TargetIsAbove() && !_senses.TargetIsWithinPitchAngle(55.0f) &&
                        (_changeStateTimer <= 0 || _backingUp))
                    {
                        // Back away from target
                        motion = -transform.forward * moveSpeed * 0.75f;
                        _backingUp = true;
                    }
                    else
                    {
                        // Stop moving
                        _backingUp = false;
                        return;
                    }
                }
            }

            SetChangeStateTimer();

            // Check if there is something to collide with directly in movement direction, such as upward sloping ground.
            var direction2d = direction;
            if (!_flies && !_swims && !IsLevitating)
                direction2d.y = 0;
            ObstacleCheck(direction2d);
            FallCheck(direction2d);

            if (_fallDetected || _obstacleDetected)
            {
                if (!strafe && !backAway)
                    FindDetour(direction2d);
            }
            else
                // Clear to move
            {
                if (_swims)
                    WaterMove(motion);
                else
                    _controller.Move(motion * Time.deltaTime);
            }
        }

        /// <summary>
        /// Try to find a way around an obstacle or fall.
        /// </summary>
        private void FindDetour(Vector3 direction2d)
        {
            float angle;
            var testMove = Vector3.zero;
            var foundUpDown = false;

            // Try up/down first
            if (_flies || _swims || IsLevitating)
            {
                var multiplier = 0.3f;
                if (Random.Range(0, 2) == 0)
                    multiplier = -0.3f;

                var upOrDown = new Vector3(0, 1, 0);
                upOrDown.y *= multiplier;

                testMove = (direction2d + upOrDown).normalized;

                ObstacleCheck(testMove);
                if (_obstacleDetected)
                {
                    upOrDown.y *= -1;
                    testMove = (direction2d + upOrDown).normalized;
                    ObstacleCheck(testMove);
                }

                if (!_obstacleDetected)
                    foundUpDown = true;
            }

            // Reset clockwise check if we've been clear of obstacles/falls for a while
            if (!foundUpDown && Time.time - _lastTimeWasStuck > 2f)
            {
                _checkingClockwiseTimer = 0;
                _didClockwiseCheck = false;
            }

            if (!foundUpDown && _checkingClockwiseTimer <= 0)
            {
                if (!_didClockwiseCheck)
                {
                    // Check 45 degrees in both ways first
                    // Pick first direction to check randomly
                    if (Random.Range(0, 2) == 0)
                        angle = 45;
                    else
                        angle = -45;

                    testMove = Quaternion.AngleAxis(angle, Vector3.up) * direction2d;
                    ObstacleCheck(testMove);
                    FallCheck(testMove);

                    if (!_obstacleDetected && !_fallDetected)
                    {
                        // First direction was clear, use that way
                        if (angle == 45)
                        {
                            _checkingClockwise = true;
                        }
                        else
                            _checkingClockwise = false;
                    }
                    else
                    {
                        // Tested 45 degrees in the clockwise/counter-clockwise direction we chose,
                        // but hit something, so try other one.
                        angle *= -1;
                        testMove = Quaternion.AngleAxis(angle, Vector3.up) * direction2d;
                        ObstacleCheck(testMove);
                        FallCheck(testMove);

                        if (!_obstacleDetected && !_fallDetected)
                        {
                            if (angle == 45)
                            {
                                _checkingClockwise = true;
                            }
                            else
                                _checkingClockwise = false;
                        }
                        else
                        {
                            // Both 45 degrees checks failed, pick clockwise/counterclockwise based on angle to target
                            var toTarget = _destination - transform.position;
                            var directionToTarget = toTarget.normalized;
                            angle = Vector3.SignedAngle(directionToTarget, direction2d, Vector3.up);

                            if (angle > 0)
                            {
                                _checkingClockwise = true;
                            }
                            else
                                _checkingClockwise = false;
                        }
                    }

                    _checkingClockwiseTimer = 5;
                    _didClockwiseCheck = true;
                }
                else
                {
                    _didClockwiseCheck = false;
                    _checkingClockwise = !_checkingClockwise;
                    _checkingClockwiseTimer = 5;
                }
            }

            angle = 0;
            var count = 0;

            if (!foundUpDown)
            {
                do
                {
                    if (_checkingClockwise)
                        angle += 45;
                    else
                        angle -= 45;

                    testMove = Quaternion.AngleAxis(angle, Vector3.up) * direction2d;
                    ObstacleCheck(testMove);
                    FallCheck(testMove);

                    // Break out of loop if can't find anywhere to go
                    count++;
                    if (count > 7)
                    {
                        break;
                    }
                } while (_obstacleDetected || _fallDetected);
            }

            _detourDestination = transform.position + testMove * 2;

            if (_avoidObstaclesTimer <= 0)
                _avoidObstaclesTimer = 0.75f;
            _lastTimeWasStuck = Time.time;
        }

        private void ObstacleCheck(Vector3 direction)
        {
            _obstacleDetected = false;
            // Rationale: follow walls at 45° incidence; is that optimal? At least it seems very good
            var checkDistance = _controller.radius / Mathf.Sqrt(2f);
            _foundUpwardSlope = false;
            _foundDoor = false;

            RaycastHit hit;
            // Climbable/not climbable step for the player seems to be at around a height of 0.65f. The player is 1.8f tall.
            // Using the same ratio to height as these values, set the capsule for the enemy. 
            var p1 = transform.position + (Vector3.up * -_originalHeight * 0.1388F);
            var p2 = p1 + (Vector3.up * Mathf.Min(_originalHeight, DoorCrouchingHeight) / 2);

            if (Physics.CapsuleCast(p1, p2, _controller.radius / 2, direction, out hit, checkDistance,
                    _ignoreMaskForObstacles))
            {
                // Debug.DrawRay(transform.position, direction, Color.red, 2.0f);
                _obstacleDetected = true;
                var entityBehaviour2 = hit.transform.GetComponent<DaggerfallEntityBehaviour>();
                var door = hit.transform.GetComponent<DaggerfallActionDoor>();
                var loot = hit.transform.GetComponent<DaggerfallLoot>();

                if (entityBehaviour2)
                {
                    if (entityBehaviour2 == _senses.Target)
                        _obstacleDetected = false;
                }
                else if (door)
                {
                    _obstacleDetected = false;
                    _foundDoor = true;
                    if (_senses.TargetIsWithinYawAngle(22.5f, door.transform.position))
                    {
                        _senses.LastKnownDoor = door;
                        _senses.DistanceToDoor = Vector3.Distance(transform.position, door.transform.position);
                    }
                }
                else if (loot)
                {
                    _obstacleDetected = false;
                }
                else if (!_swims && !_flies && !IsLevitating)
                {
                    // If an obstacle was hit, check for a climbable upward slope
                    var checkUp = transform.position + direction;
                    checkUp.y++;

                    direction = (checkUp - transform.position).normalized;
                    p1 = transform.position + (Vector3.up * -_originalHeight * 0.25f);
                    p2 = p1 + (Vector3.up * _originalHeight * 0.75f);

                    if (!Physics.CapsuleCast(p1, p2, _controller.radius / 2, direction, checkDistance))
                    {
                        _obstacleDetected = false;
                        _foundUpwardSlope = true;
                    }
                }
            }
        }

        private void FallCheck(Vector3 direction)
        {
            if (_flies || IsLevitating || _swims || _obstacleDetected || _foundUpwardSlope || _foundDoor)
            {
                _fallDetected = false;
                return;
            }

            var checkDistance = 1;
            var rayOrigin = transform.position;

            direction *= checkDistance;
            var ray = new Ray(rayOrigin + direction, Vector3.down);
            RaycastHit hit;

            _fallDetected = !Physics.Raycast(ray, out hit, (_originalHeight * 0.5f) + 1.5f);
        }

        /// <summary>
        /// Set timer for padding between state changes, for non-perfect reflexes.
        /// </summary>
        private void SetChangeStateTimer()
        {
            // No timer without enhanced AI
            if (!DaggerfallUnity.Settings.EnhancedCombatAI)
                return;

            if (_changeStateTimer <= 0)
                _changeStateTimer = Random.Range(0.2f, .8f);
        }

        /// <summary>
        /// Movement for water enemies.
        /// </summary>
        private void WaterMove(Vector3 motion)
        {
            // Don't allow aquatic enemies to go above the water level of a dungeon block
            if (GameManager.Instance.PlayerEnterExit.blockWaterLevel != 10000
                && _controller.transform.position.y
                < GameManager.Instance.PlayerEnterExit.blockWaterLevel * -1 * MeshReader.GlobalScale)
            {
                if (motion.y > 0 && _controller.transform.position.y + (100 * MeshReader.GlobalScale)
                    >= GameManager.Instance.PlayerEnterExit.blockWaterLevel * -1 * MeshReader.GlobalScale)
                {
                    motion.y = 0;
                }

                _controller.Move(motion * Time.deltaTime);
            }
        }

        /// <summary>
        /// Rotate toward target.
        /// </summary>
        private void TurnToTarget(Vector3 targetDirection)
        {
            const float turnSpeed = 20f;
            //Classic speed is 11.25f, too slow for Daggerfall Unity's agile player movement

            if (GameManager.ClassicUpdate)
            {
                transform.forward =
                    Vector3.RotateTowards(transform.forward, targetDirection, turnSpeed * Mathf.Deg2Rad, 0.0f);
            }
        }

        /// <summary>
        /// Set to either idle or move animation depending on whether the enemy has moved or rotated.
        /// </summary>
        private void UpdateToIdleOrMoveAnim()
        {
            if (!_mobile.IsPlayingOneShot())
            {
                // Rotation is done at classic update rate, so check at classic update rate
                if (GameManager.ClassicUpdate)
                {
                    var currentDirection = transform.forward;
                    currentDirection.y = 0;
                    _rotating = _lastDirection != currentDirection;
                    _lastDirection = currentDirection;
                }

                // Movement is done at regular update rate, so check position at regular update rate
                if (!_rotating && _lastPosition == transform.position)
                    _mobile.ChangeEnemyState(MobileStates.Idle);
                else
                    _mobile.ChangeEnemyState(MobileStates.Move);
            }

            _lastPosition = transform.position;
        }

        private void ApplyFallDamage()
        {
            const float fallingDamageThreshold = 5.0f;
            const float hpPerMetre = 5f;

            if (_controller.isGrounded)
            {
                // did just land?
                if (_falls)
                {
                    var fallDistance = _lastGroundedY - transform.position.y;
                    if (fallDistance > fallingDamageThreshold)
                    {
                        var damage = (int) (hpPerMetre * (fallDistance - fallingDamageThreshold));

                        var enemyEntity = _entityBehaviour.Entity as EnemyEntity;
                        enemyEntity.DecreaseHealth(damage);

                        if (_entityBlood)
                        {
                            // Like in classic, falling enemies bleed at the center. It must hurt the center of mass ;)
                            _entityBlood.ShowBloodSplash(0, transform.position);
                        }

                        DaggerfallUI.Instance.DaggerfallAudioSource.PlayClipAtPoint((int) SoundClips.FallDamage,
                            FindGroundPosition());
                    }
                }

                _lastGroundedY = transform.position.y;
            }
            // For flying enemies, "lastGroundedY" is really "lastAltitudeControlY"
            else if (_flies && !_flyerFalls)
                _lastGroundedY = transform.position.y;
        }

        /// <summary>
        /// Open doors that are in the way.
        /// </summary>
        private void OpenDoors()
        {
            // Try to open doors blocking way
            if (_mobile.Enemy.CanOpenDoors)
            {
                if (_senses.LastKnownDoor != null
                    && _senses.DistanceToDoor < openDoorDistance && !_senses.LastKnownDoor.IsOpen
                    && !_senses.LastKnownDoor.IsLocked)
                {
                    _senses.LastKnownDoor.ToggleDoor();
                    return;
                }

                // If door didn't open, and we are trying to get to the target, bash
                Bashing = DaggerfallUnity.Settings.EnhancedCombatAI && !_senses.TargetInSight && _moveInForAttack
                          && _senses.LastKnownDoor != null &&
                          _senses.LastKnownDoor.IsLocked;
            }
        }

        /// <summary>
        /// Limits maximum controller height.
        /// Tall sprites require this hack to get through doors.
        /// </summary>
        private void HeightAdjust()
        {
            // If enemy bumps into something, temporarily reduce their height to 1.65, which should be short enough to fit through most if not all doorways.
            // Unfortunately, while the enemy is shortened, projectiles will not collide with the top of the enemy for the difference in height.
            if (!_resetHeight && _controller && ((_controller.collisionFlags & CollisionFlags.CollidedSides) != 0) &&
                _originalHeight > DoorCrouchingHeight)
            {
                // Adjust the center of the controller so that sprite doesn't sink into the ground
                _centerChange = (DoorCrouchingHeight - _controller.height) / 2;
                var newCenter = _controller.center;
                newCenter.y += _centerChange;
                _controller.center = newCenter;
                // Adjust the height
                _controller.height = DoorCrouchingHeight;
                _resetHeight = true;
                _heightChangeTimer = 0.5f;
            }
            else if (_resetHeight && _heightChangeTimer <= 0)
            {
                // Restore the original center
                var newCenter = _controller.center;
                newCenter.y -= _centerChange;
                _controller.center = newCenter;
                // Restore the original height
                _controller.height = _originalHeight;
                _resetHeight = false;
            }

            if (_resetHeight && _heightChangeTimer > 0)
            {
                _heightChangeTimer -= Time.deltaTime;
            }
        }
    }
}