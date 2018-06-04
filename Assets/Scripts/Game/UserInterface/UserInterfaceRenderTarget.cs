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

namespace DaggerfallWorkshop.Game.UserInterface
{
    /// <summary>
    /// Manages the render target texture for UI systems and provides helpers for drawing UI components.
    /// </summary>
    public class UserInterfaceRenderTarget : MonoBehaviour
    {
        #region Fields

        public UnityEngine.UI.RawImage OutputImage;

        int customWidth = 0;
        int customHeight = 0;
        int customGUIDepth = 0;
        Panel customParentPanel;

        int createCount = 0;
        RenderTexture targetTexture = null;
        Rect targetRect = new Rect();

        #endregion

        #region Properties

        /// <summary>
        /// Gets current render target texture.
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

        /// <summary>
        /// Gets or sets a custom width for target texture.
        /// </summary>
        public int CustomWidth
        {
            get { return customWidth; }
            set { customWidth = value; }
        }

        /// <summary>
        /// Gets or sets a custom height for target texture.
        /// </summary>
        public int CustomHeight
        {
            get { return customHeight; }
            set { customHeight = value; }
        }

        /// <summary>
        /// Gets or sets a custom GUI.Depth for OnGUI() draws.
        /// </summary>
        public int CustomGUIDepth
        {
            get { return customGUIDepth; }
            set { customGUIDepth = value; }
        }

        /// <summary>
        /// Gets custom parent panel for adding custom own UI controls.
        /// This will be set to custom width/height dimensions.
        /// </summary>
        public Panel CustomParentPanel
        {
            get { return customParentPanel; }
        }

        #endregion

        #region Unity

        private void Start()
        {
            customParentPanel = new Panel();
            CheckTargetTexture();
        }

        private void Update()
        {
            CheckTargetTexture();

            // Update parent panel
            customParentPanel.Update();
        }

        private void OnGUI()
        {
            // Set depth
            GUI.depth = customGUIDepth;

            if (Event.current.type != EventType.Repaint)
                return;

            if (IsReady())
            {
                // Clear UI
                Clear();

                // Draw parent panel
                GUI.depth = 0;
                customParentPanel.Draw();
            }
        }

        #endregion

        #region Public Methods
        #endregion

        #region Drawing Methods

        public void Clear()
        {
            RenderTexture oldRt = RenderTexture.active;
            RenderTexture.active = targetTexture;

            GL.Clear(true, true, Color.clear);

            RenderTexture.active = oldRt;
        }

        public void DrawTexture(Rect position, Texture2D image)
        {
            if (!IsReady())
                return;

            RenderTexture oldRt = RenderTexture.active;
            RenderTexture.active = targetTexture;

            GUI.DrawTexture(position, image);

            RenderTexture.active = oldRt;
        }

        public void DrawTexture(Rect position, Texture2D image, ScaleMode scaleMode, bool alphaBlend = true, float imageAspect = 0)
        {
            if (!IsReady())
                return;

            RenderTexture oldRt = RenderTexture.active;
            RenderTexture.active = targetTexture;

            GUI.DrawTexture(position, image, scaleMode, alphaBlend);

            RenderTexture.active = oldRt;
        }

        public void DrawTextureWithTexCoords(Rect position, Texture image, Rect texCoords, bool alphaBlend = true)
        {
            if (!IsReady())
                return;

            RenderTexture oldRt = RenderTexture.active;
            RenderTexture.active = targetTexture;

            GUI.DrawTextureWithTexCoords(position, image, texCoords, alphaBlend);

            RenderTexture.active = oldRt;
        }

        #endregion

        #region Render Texture Management

        // Check render texture is non-null and created
        bool IsReady()
        {
            return (targetTexture != null && targetTexture.IsCreated());
        }

        // Check render texture and recreate if not valid
        void CheckTargetTexture()
        {
            // Use either screen or custom dimensions
            int width = (customWidth == 0) ? Screen.width : customWidth;
            int height = (customHeight == 0) ? Screen.height : customHeight;

            // Set custom panel size
            customParentPanel.Size = new Vector2(width, height);

            // Just return same texture if still valid
            if (!IsReady() || targetTexture.width != width || targetTexture.height != height)
            {
                // Create target texture matching screen dimensions
                targetRect = new Rect(0, 0, width, height);
                targetTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
                targetTexture.name = string.Format("DaggerfallUI RenderTexture {0}", createCount++);
                targetTexture.Create();
                Debug.LogFormat("Created UI RenderTexture with dimensions {0}, {1}", width, height);

                // Update output image to new render texture if one is set
                if (OutputImage)
                    OutputImage.texture = targetTexture;

                // Raise event to notify other systems target texture has changed
                RaiseOnCreateTargetTexture();
            }
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