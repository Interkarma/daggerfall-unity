
using System;
using System.Collections.Generic;
using System.Linq;
using DaggerfallWorkshop.Game.Entity;
using UnityEngine;

namespace DaggerfallWorkshop.Game.UserInterface {
    public class HUDVitalsPool {

        private Dictionary<GameObject, HUDVitalsEntity> _enemyVitalsPair;
        private List<HUDVitalsEntity> _vitalsPool;
        private Camera _camera;

        private float _raycastCooldown = 0.65f;
        private float _curRaycastCooldown = 0;

        public List<HUDVitalsEntity> VitalsPool => _vitalsPool;
        public GameObject[] Enemies => _enemyVitalsPair.Keys.ToArray();

        public HUDVitalsPool(int poolStackCount)
        {
            _camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

            // Fill pool with vitals panels
            _vitalsPool = new List<HUDVitalsEntity>(poolStackCount);
            for (int i = 0; i < poolStackCount; i++)
                _vitalsPool.Add(new HUDVitalsEntity(_camera));

            _enemyVitalsPair = new Dictionary<GameObject, HUDVitalsEntity>();
            GameManager.OnEnemySpawn += OnEnemySpawn;
        }

        public void Update()
        {
            // Wait for cooldown
            _curRaycastCooldown += Time.unscaledDeltaTime;
            if (_curRaycastCooldown < _raycastCooldown) return;

            foreach (var enemy in Enemies) {

                // Clear all destroyed enemies
                if (enemy == null) {
                    _enemyVitalsPair.Remove(enemy);
                    continue;
                }

                var mobileUnit = enemy.GetComponent<DaggerfallEnemy>().MobileUnit;
                var entityBehaviour = enemy.GetComponent<DaggerfallEntityBehaviour>();

                // If unit is visible, activate HealthBar from pool
                if (IsVisible(mobileUnit, entityBehaviour.Entity)) {
                    if (_enemyVitalsPair[enemy] != null) continue;
                    var freeVitals = GetFreeHUD();
                    freeVitals.SetOwner(mobileUnit, entityBehaviour.Entity);
                    freeVitals.Show(mobileUnit.Enemy.Team == MobileTeams.PlayerAlly);
                    _enemyVitalsPair[enemy] = freeVitals;
                }
                // Else, deactivate HealthBar
                else {
                    _enemyVitalsPair[enemy]?.Hide();
                    _enemyVitalsPair[enemy] = null;
                }
            }

            _curRaycastCooldown = 0;
        }

        private bool IsVisible(MobileUnit mobile, DaggerfallEntity entity)
        {
            // HealthBar is not visible if distance is greater than 15
            var sqrDistance = (mobile.transform.position - _camera.transform.position).sqrMagnitude;
            if (sqrDistance > 225) return false;

            var viewportPoint = _camera.WorldToViewportPoint(mobile.transform.position);
            var ray = _camera.ViewportPointToRay(viewportPoint);

            if (Physics.Raycast(ray, out var hit, 15f, LayerMask.GetMask("Enemies", "Default")))
                if (hit.collider.TryGetComponent(out DaggerfallEntityBehaviour beh) && beh.Entity == entity)
                    return true;

            return false;
        }

        private void OnEnemySpawn(GameObject go)
        {
            _enemyVitalsPair.Add(go, null);
        }

        private HUDVitalsEntity GetFreeHUD()
        {
            // Get first disabled object from pool, if no such, use first in pool
            return _vitalsPool.FirstOrDefault(v => !v.Enabled) ?? _vitalsPool.First();
        }

        ~HUDVitalsPool()
        {
            GameManager.OnEnemySpawn -= OnEnemySpawn;
        }
    }
}