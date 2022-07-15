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
using UnityEngine.Rendering;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Jobs;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Collections;
using Unity.Profiling;
using DaggerfallConnect;
using DaggerfallWorkshop.Utility.AssetInjection;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop
{
    /// <summary>
    /// System to update transforms for all DaggerfallBillboard instances.
    /// (singleton)
    /// </summary>
    [AddComponentMenu("")]
    public class DaggerfallBillboardSystem : MonoBehaviour
    {

        static TransformAccessArray
            pointBillboards,
            axialBillboards;
        static Dictionary<Transform, int>
            pointIndices = new Dictionary<Transform, int>(),
            axialIndices = new Dictionary<Transform, int>();
        static ProfilerMarker
            pmCalculateQuaterions = new ProfilerMarker("calculate quaterions"),
            pmScheduleJobs = new ProfilerMarker("schedule jobs");
        Camera mainCamera = null;
        JobHandle dependency;

#if UNITY_EDITOR
        ~DaggerfallBillboardSystem() => OnDestroy();// fixes: ocassional alloc warning
#endif

        void Awake()
        {
            pointBillboards = new TransformAccessArray(32);
            axialBillboards = new TransformAccessArray(32);
        }

        void OnDestroy()
        {
            if (pointBillboards.isCreated) pointBillboards.Dispose();
            if (axialBillboards.isCreated) axialBillboards.Dispose();
            pointIndices.Clear();
            axialIndices.Clear();
        }

        void Update()
        {
            if (mainCamera == null)
                mainCamera = GameObject.FindGameObjectWithTag("MainCamera")?.GetComponent<Camera>();

            if (mainCamera != null)
            {
                pmCalculateQuaterions.Begin();
                var forward = mainCamera.transform.forward;
                var lookAxial = Quaternion.LookRotation(-new Vector3(forward.x, 0, forward.z));
                var lookPoint = Quaternion.LookRotation(-forward);
                pmCalculateQuaterions.End();

                pmScheduleJobs.Begin();
                {
                    dependency.Complete();// make sure these jobs take no more than a single frame:

                    var jobAxial = new SetRotationJob
                    {
                        Value = lookAxial
                    };
                    var jobHandleAxial = jobAxial.Schedule(axialBillboards);

                    var jobPoint = new SetRotationJob
                    {
                        Value = lookPoint
                    };
                    var jobHandlePoint = jobPoint.Schedule(pointBillboards);

                    dependency = JobHandle.CombineDependencies(jobHandleAxial, jobHandlePoint);
                }
                pmScheduleJobs.End();
            }
        }

        public static void AddPointBillboard(Transform t)
        {
            int index = pointBillboards.length;
            pointBillboards.Add(t);
            pointIndices.Add(t, index);
        }

        public static void RemoveBillboard(Transform t)
        {
#if UNITY_EDITOR
            // TransformAccessArray should always exists, *except* when in-editor game just stopped playing, hence this null-check.
            if (!axialBillboards.isCreated || !pointBillboards.isCreated) return;
#endif

            if (pointIndices.ContainsKey(t))
            {
                int index = pointIndices[t];
                pointBillboards.RemoveAtSwapBack(index);
                pointIndices.Remove(t);

                int indexThatGotReplaced = pointBillboards.length;
                var found = pointIndices.FirstOrDefault((next) => next.Value == indexThatGotReplaced);
                if (found.Key) pointIndices[found.Key] = index;
            }
            else if (axialIndices.ContainsKey(t))
            {
                int index = axialIndices[t];
                axialBillboards.RemoveAtSwapBack(index);
                axialIndices.Remove(t);

                int indexThatGotReplaced = axialBillboards.length;
                var found = axialIndices.FirstOrDefault((next) => next.Value == indexThatGotReplaced);
                if (found.Key) axialIndices[found.Key] = index;
            }
        }

        public static void AddAxialBillboard(Transform t)
        {
            int index = axialBillboards.length;
            axialBillboards.Add(t);
            axialIndices.Add(t, index);
        }

        [Unity.Burst.BurstCompile]
        public struct SetRotationJob : IJobParallelForTransform
        {
            public quaternion Value;
            void IJobParallelForTransform.Execute(int index, TransformAccess transform)
                => transform.rotation = Value;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void InitializeType()
            => DontDestroyOnLoad(new GameObject($"#{nameof(DaggerfallBillboardSystem)}").AddComponent<DaggerfallBillboardSystem>().gameObject);

    }
}
