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
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Game.Serialization;

namespace DaggerfallWorkshop.Game.MagicAndEffects
{
    [RequireComponent(typeof(Light))]
    public class MagicCandleBehaviour : MonoBehaviour
    {
        const int candleArchive = 210;
        const int candleRecord = 3;
        const float moveSpeed = 8.0f;

        Vector3 startLocalPosition;
        Vector3 lastOffsetPosition = Vector3.zero;
        Vector3 nextOffsetPosition = Vector3.zero;
        float moveAmount = 0;

        private void Start()
        {
            GameObject go = GameObjectHelper.CreateDaggerfallBillboardGameObject(candleArchive, candleRecord, transform);
            go.transform.localPosition = Vector3.zero;

            startLocalPosition = transform.localPosition;
            lastOffsetPosition = startLocalPosition;

            SaveLoadManager.OnStartLoad += SaveLoadManager_OnStartLoad;
            StartGameBehaviour.OnNewGame += StartGameBehaviour_OnNewGame;

            // Observe spell shadow setting
            GetComponent<Light>().shadows = (DaggerfallUnity.Settings.EnableSpellShadows) ? LightShadows.Soft : LightShadows.None;
        }

        private void Update()
        {
            if (nextOffsetPosition == Vector3.zero)
            {
                moveAmount = 0;
                nextOffsetPosition = startLocalPosition + Random.insideUnitSphere * 0.125f;
            }

            transform.localPosition = Vector3.Lerp(lastOffsetPosition, nextOffsetPosition, moveAmount);
            moveAmount += moveSpeed * Time.deltaTime;
            if (moveAmount >= 1.0)
            {
                lastOffsetPosition = nextOffsetPosition;
                nextOffsetPosition = Vector3.zero;
            }
        }

        public void DestroyCandle()
        {
            SaveLoadManager.OnStartLoad -= SaveLoadManager_OnStartLoad;
            StartGameBehaviour.OnNewGame -= StartGameBehaviour_OnNewGame;
            if (gameObject != null)
                Destroy(gameObject);
        }

        private void StartGameBehaviour_OnNewGame()
        {
            DestroyCandle();
        }

        private void SaveLoadManager_OnStartLoad(SaveData_v1 saveData)
        {
            DestroyCandle();
        }
    }
}
