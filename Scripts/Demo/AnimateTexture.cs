// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

using UnityEngine;
using System.Collections;

namespace DaggerfallWorkshop.Demo
{
    /// <summary>
    /// General purpose texture animation on a single material.
    /// </summary>
    public class AnimateTexture : MonoBehaviour
    {
        public Material TargetMaterial;
        public Texture[] TextureArray;
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
                if (TextureArray != null && TargetMaterial != null)
                {
                    TargetMaterial.mainTexture = TextureArray[currentTexture++];
                    if (currentTexture >= TextureArray.Length)
                        currentTexture = 0;
                }

                yield return new WaitForSeconds(1f / FramesPerSecond);
            }
        }
    }
}