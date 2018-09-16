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
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop.Game.MagicAndEffects
{
    public class MagicCandleBehaviour : MonoBehaviour
    {
        const int candleArchive = 210;
        const int candleRecord = 3;
        const int candleFramesPerSecond = 5;
        const float moveSpeed = 8.0f;

        DaggerfallBillboard myBillboard;
        Vector3 startLocalPosition;
        Vector3 lastOffsetPosition = Vector3.zero;
        Vector3 nextOffsetPosition = Vector3.zero;
        float moveAmount = 0;

        private void Start()
        {
            GameObject go = GameObjectHelper.CreateDaggerfallBillboardGameObject(candleArchive, candleRecord, transform);
            go.transform.localPosition = Vector3.zero;
            myBillboard = go.GetComponent<DaggerfallBillboard>();
            myBillboard.FramesPerSecond = candleFramesPerSecond;
            myBillboard.FaceY = true;
            myBillboard.OneShot = false;
            myBillboard.GetComponent<MeshRenderer>().receiveShadows = false;

            startLocalPosition = transform.localPosition;
            lastOffsetPosition = startLocalPosition;
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
    }
}