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
using System.Collections;

namespace DaggerfallWorkshop.Utility
{
    /// <summary>
    /// General purpose material animation.
    /// Works by swapping texture maps on target material from an array of other materials.
    /// </summary>
    public class AnimatedMaterial : MonoBehaviour
    {
        public Material TargetMaterial;
        public CachedMaterial[] AnimationFrames;
        public float FramesPerSecond = 12;

        bool restartAnims;

        void Start()
        {
            StartCoroutine(AnimateTextures());
        }

        void OnDisable()
        {
            restartAnims = true;
        }

        void OnEnable()
        {
            // Restart animation coroutine if not running
            if (restartAnims)
            {
                StartCoroutine(AnimateTextures());
                restartAnims = false;
            }
        }

        IEnumerator AnimateTextures()
        {
            int currentTexture = 0;
            while (true)
            {
                if (AnimationFrames != null && TargetMaterial != null)
                {
                    TargetMaterial.mainTexture = AnimationFrames[currentTexture].albedoMap;
                    TargetMaterial.SetTexture("_BumpMap", AnimationFrames[currentTexture].normalMap);
                    TargetMaterial.SetTexture("_EmissionMap", AnimationFrames[currentTexture].emissionMap);
                    if (++currentTexture >= AnimationFrames.Length)
                        currentTexture = 0;
                }

                yield return new WaitForSeconds(1f / FramesPerSecond);
            }
        }
    }
}