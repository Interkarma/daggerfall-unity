using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Companion.Utils;
using DaggerfallWorkshop.Game.Entity;
using UnityEngine;

namespace Game.Mods.Companion
{
    [RequireComponent(typeof(DaggerfallEntityBehaviour))]
    [RequireComponent(typeof(EnemyMotor))]
    public class PlayerFollower : MonoBehaviour
    {
        private static readonly MobileTeams followTeam = MobileTeams.PlayerEnemy;
        [SerializeField] private float followThreshold = 3;
        [SerializeField] private float teleportThreshold = 10;
        [SerializeField] [HideInInspector] private DaggerfallEntityBehaviour entity;
        [SerializeField] [HideInInspector] private EnemyMotor motor;

        private MobileTeams _initialTeam;
        private bool _isFollowing;

        private DaggerfallEntityBehaviour _player;
        private float FollowThresholdSqr => followThreshold * followThreshold;

        private MobileTeams Team
        {
            get => entity.Entity.Team;
            set => entity.Entity.Team = value;
        }

        private float TeleportThresholdSqr => teleportThreshold * teleportThreshold;

        private void Start()
        {
            _player = GameManager.Instance.PlayerEntityBehaviour;
            _isFollowing = motor.IsHostile;
        }

        public void Follow(bool value)
        {
            if (_isFollowing == value)
                return;

            if (value)
            {
                _initialTeam = Team;
                Team = followTeam;
            }
            else
            {
                Team = _initialTeam;
            }

            motor.IsHostile = value;
            _isFollowing = value;
        }

        private void Reset()
        {
            TryGetComponent(out entity);
            TryGetComponent(out motor);
        }

        private void Update()
        {
            if (!_player)
                return;

            float distanceToPlayer = (_player.transform.position - transform.position).sqrMagnitude;
            if (distanceToPlayer > TeleportThresholdSqr)
            {
                transform.position = _player.transform.position + RandomExt.OnUnitCircle * (followThreshold * Random.Range(0.5f, 0.95f));
                Follow(false);
            }


            Follow(distanceToPlayer > FollowThresholdSqr);
        }
    }
}