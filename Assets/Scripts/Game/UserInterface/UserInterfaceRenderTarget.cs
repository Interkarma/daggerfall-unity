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
using UnityEngine.UI;

namespace DaggerfallWorkshop.Game.UserInterface
{
    /// <summary>
    /// Manages the render target texture for UI systems and provides helpers for drawing UI components.
    /// </summary>
    public class UserInterfaceRenderTarget : MonoBehaviour
    {
        #region Fields

        int createCount = 0;
        RenderTexture targetTexture;
        Rect targetRect = new Rect();

        #endregion

        #region Properties

        /// <summary>
        /// Gets current target texture.
        /// Capture OnCreateTargetTexture to get latest render texture if this changes for any reason.
        /// </summary>
        public RenderTexture TargetTexture
        {
            get { return targetTexture; }
        }

        /// <summary>
        /// Gets rectangle of target texture.
        /// </summary>
        public Rect TargetRect
        {
            get { return targetRect; }
        }

        #endregion

        #region Unity

        private void Awake()
        {
            CreateTargetTexture();
        }

        private void Update()
        {
            if (!CheckTargetTextureSize() || !targetTexture.IsCreated())
            {
                CreateTargetTexture();
                return;
            }
        }

        #endregion

        #region Public Methods
        #endregion

        #region Drawing Methods

        public void DrawTexture(Rect position, Texture2D image)
        {
            if (!IsReady())
                return;

            RenderTexture.active = targetTexture;

            GUI.DrawTexture(position, image);

            RenderTexture.active = null;
        }

        public void DrawTexture(Rect position, Texture2D image, ScaleMode scaleMode, bool alphaBlend = true, float imageAspect = 0)
        {
            if (!IsReady())
                return;

            RenderTexture.active = targetTexture;

            GUI.DrawTexture(position, image, scaleMode, alphaBlend);

            RenderTexture.active = null;
        }

        public void DrawTextureWithTexCoords(Rect position, Texture image, Rect texCoords, bool alphaBlend = true)
        {
            if (!IsReady())
                return;

            RenderTexture.active = targetTexture;

            GUI.DrawTextureWithTexCoords(position, image, texCoords, alphaBlend);

            RenderTexture.active = null;
        }

        #endregion

        #region Render Texture Management

        bool IsReady()
        {
            return (targetTexture && targetTexture.IsCreated());
        }

        bool CheckTargetTextureSize()
        {
            if (!targetTexture)
                return false;

            if (targetTexture.width != Screen.width || targetTexture.height != Screen.height)
                return false;

            return true;
        }

        void CreateTargetTexture()
        {
            // Create target texture matching screen dimensions
            // Notes:
            //  - Just using screen dimensions for now while solving problems of redirecting rendering from UI components
            //  - Aiming for a baseline of 1:1 functionality with current setup before changing anything further
            int width = Screen.width;
            int height = Screen.height;
            targetTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
            targetTexture.name = string.Format("DaggerfallUI RenderTexture {0}", createCount++);
            targetTexture.Create();
            targetRect = new Rect(0, 0, width, height);
            UpdateNonDiegeticOutput();
            RaiseOnCreateTargetTexture();

            Debug.LogFormat("Created UI RenderTexture with dimensions {0}, {1}", width, height);
        }

        void UpdateNonDiegeticOutput()
        {
            // Must be able to find output canvas object
            GameObject nonDiegeticUIOutput = GameManager.Instance.NonDiegeticUIOutput;
            if (!nonDiegeticUIOutput)
                return;

            // Output canvas object must be active
            if (!nonDiegeticUIOutput.activeInHierarchy)
                return;

            // Get raw image component
            RawImage rawImage = nonDiegeticUIOutput.GetComponent<RawImage>();
            if (!rawImage)
                return;

            // Set render texture to raw image
            rawImage.texture = targetTexture;
            rawImage.SetNativeSize();
        }

        #endregion

        #region Events

        // OnCreateTargetTexture
        public delegate void OnCreateTargetTextureEventHandler();
        public event OnCreateTargetTextureEventHandler OnCreateTargetTexture;
        protected virtual void RaiseOnCreateTargetTexture()
        {
            if (OnCreateTargetTexture != null)
                OnCreateTargetTexture();
        }

        #endregion
    }
}