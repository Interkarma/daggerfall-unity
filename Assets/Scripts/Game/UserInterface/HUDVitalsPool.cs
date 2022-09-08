
using System;
using System.Collections.Generic;
using System.Linq;
using DaggerfallWorkshop.Game.Entity;
using UnityEngine;

namespace DaggerfallWorkshop.Game.UserInterface {
    public class HUDVitalsPool : IDisposable {

        private Dictionary<GameObject, HUDVitalsEntity> _enemyVitalsPair;
        private List<HUDVitalsEntity> _vitalsPool;
        private Camera _camera;

        private float _raycastCooldown = 0.65f;
        private float _curRaycastCooldown = 0;

        public List<HUDVitalsEntity> VitalsPool => _vitalsPool;
        public GameObject[] Enemies => _enemyVitalsPair.Keys.ToArray();

        public HUDVitalsPool(int poolStackCount) : base()
        {
            _camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

            _enemyVitalsPair = new Dictionary<GameObject, HUDVitalsEntity>();
            _vitalsPool = new List<HUDVitalsEntity>(poolStackCount);
            for (int i = 0; i < poolStackCount; i++)
                _vitalsPool.Add(new HUDVitalsEntity(_camera));

            GameManager.OnEnemySpawn += OnEnemySpawn;
        }

        public void Update() {
            _curRaycastCooldown += Time.unscaledDeltaTime;
            if (_curRaycastCooldown < _raycastCooldown) return;

            foreach (var enemy in Enemies) {
                var mobileUnit = enemy.GetComponent<DaggerfallEnemy>().MobileUnit;
                var entityBehaviour = enemy.GetComponent<DaggerfallEntityBehaviour>();

                if (IsVisible(mobileUnit, entityBehaviour.Entity)) {
                    if (_enemyVitalsPair[enemy] != null) continue;
                    var freeVitals = GetFreeHUD();
                    freeVitals.SetOwner(mobileUnit, entityBehaviour.Entity);
                    freeVitals.Show(mobileUnit.Enemy.Team == MobileTeams.PlayerAlly);
                    _enemyVitalsPair[enemy] = freeVitals;
                }
                else {
                    _enemyVitalsPair[enemy]?.Hide();
                    _enemyVitalsPair[enemy] = null;
                }
            }

            _curRaycastCooldown = 0;
        }

        bool IsVisible(MobileUnit mobile, DaggerfallEntity entity)
        {
            var viewportPoint = _camera.WorldToViewportPoint(mobile.transform.position);
            var ray = _camera.ViewportPointToRay(viewportPoint);

            if (Physics.Raycast(ray, out var hit, 20f, LayerMask.GetMask("Enemies", "Default")))
                if (hit.collider.TryGetComponent(out DaggerfallEntityBehaviour beh) && beh.Entity == entity)
                    return true;

            return false;
        }

        void OnEnemySpawn(GameObject go)
        {
            _enemyVitalsPair.Add(go, null);
        }

        HUDVitalsEntity GetFreeHUD()
        {
            // Get first disabled object from pool, if no such, use first in pool
            return _vitalsPool.FirstOrDefault(v => !v.Enabled) ?? _vitalsPool.First();
        }

        public void Dispose() {
            GameManager.OnEnemySpawn -= OnEnemySpawn;
        }
    }
}