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
    /// Used a double-buffered rendering setup with a backbuffer texture and a presentation texture.
    /// The front and back textures are swapped at the end of every UI draw operation.
    /// </summary>
    public class UserInterfaceRenderTarget : MonoBehaviour
    {
        #region Fields

        int createCount = 0;
        RenderTexture targetTextureA = null;
        RenderTexture targetTextureB = null;
        Texture2D clearTexture = null;
        Rect targetRect = new Rect();
        bool flip = false;

        #endregion

        #region Properties

        /// <summary>
        /// Gets current presentation texture.
        /// </summary>
        public RenderTexture PresentationTexture
        {
            get { return GetPresentationTexture(); }
        }

        /// <summary>
        /// Gets current backbuffer texture.
        /// </summary>
        public RenderTexture BackBufferTexture
        {
            get { return GetBackBufferTexture(); }
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
            targetTextureA = CheckTargetTexture(targetTextureA);
            targetTextureB = CheckTargetTexture(targetTextureB);
            clearTexture = DaggerfallUI.CreateSolidTexture(Color.clear, 8);
            UpdateNonDiegeticOutput();
        }

        private void Update()
        {
            targetTextureA = CheckTargetTexture(targetTextureA);
            targetTextureB = CheckTargetTexture(targetTextureB);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Flip presentation and backbuffer texture so latest render becomes presentation texture.
        /// Updates non-diegetic canvas if currently enabled. If not using default non-diegetic canvas
        /// then some other system (e.g. a diegetic quad) must present UI to end user.
        /// </summary>
        public void Present()
        {
            flip = !flip;
            UpdateNonDiegeticOutput();
            ClearBackBufferTexture();
        }

        /// <summary>
        /// Sets the backbuffer to clear.
        /// </summary>
        public void ClearBackBufferTexture()
        {
            Graphics.Blit(clearTexture, GetBackBufferTexture());
        }

        /// <summary>
        /// Sets the presentation texture to clear.
        /// </summary>
        public void ClearPresentationTexture()
        {
            Graphics.Blit(clearTexture, GetPresentationTexture());
        }

        #endregion

        #region Private Methods

        RenderTexture GetPresentationTexture()
        {
            return (flip) ? targetTextureA : targetTextureB;
        }

        RenderTexture GetBackBufferTexture()
        {
            return (flip) ? targetTextureB : targetTextureA;
        }

        #endregion

        #region Drawing Methods

        public void DrawTexture(Rect position, Texture2D image)
        {
            if (!IsReady(GetBackBufferTexture()))
                return;

            GUI.DrawTexture(position, image);
        }

        public void DrawTexture(Rect position, Texture2D image, ScaleMode scaleMode, bool alphaBlend = true, float imageAspect = 0)
        {
            if (!IsReady(GetBackBufferTexture()))
                return;

            GUI.DrawTexture(position, image, scaleMode, alphaBlend);
        }

        public void DrawTextureWithTexCoords(Rect position, Texture image, Rect texCoords, bool alphaBlend = true)
        {
            if (!IsReady(GetBackBufferTexture()))
                return;

            GUI.DrawTextureWithTexCoords(position, image, texCoords, alphaBlend);
        }

        #endregion

        #region Render Texture Management

        // Check render texture is non-null and created
        bool IsReady(RenderTexture targetTexture)
        {
            return (targetTexture != null && targetTexture.IsCreated());
        }

        // Check render texture and recreate if not valid
        RenderTexture CheckTargetTexture(RenderTexture targetTexture)
        {
            // Just using screen dimensions for now while solving problems of redirecting rendering from UI components
            // Aiming for a baseline of 1:1 functionality with current setup before changing anything further
            int width = Screen.width;
            int height = Screen.height;
            targetRect = new Rect(0, 0, width, height);

            // Just return same texture if still valid
            if (!IsReady(targetTexture) || targetTexture.width != width || targetTexture.height != height)
            {
                // Create target texture matching screen dimensions
                targetTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
                targetTexture.name = string.Format("DaggerfallUI RenderTexture {0}", createCount++);
                targetTexture.Create();
                RaiseOnCreateTargetTexture();
                Debug.LogFormat("Created UI RenderTexture with dimensions {0}, {1}", width, height);
            }

            return targetTexture;
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

            // Set presentation render texture to raw image output
            rawImage.texture = GetPresentationTexture();
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