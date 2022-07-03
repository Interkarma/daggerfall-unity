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

namespace DaggerfallWorkshop.Game.UserInterface
{
    /// <summary>
    /// Manages the render target texture for UI systems and provides helpers for drawing UI components.
    /// </summary>
    public class UserInterfaceRenderTarget : MonoBehaviour
    {
        #region Fields

        public UnityEngine.UI.RawImage OutputImage;
        public bool EnableCustomStack = false;

        int customWidth = 0;
        int customHeight = 0;
        Panel parentPanel;

        int createCount = 0;
        RenderTexture targetTexture = null;
        Vector2 targetSize = new Vector2();
        Color clearColor = Color.clear;
        FilterMode filterMode;

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
        /// Gets size of target texture.
        /// </summary>
        public Vector2 TargetSize
        {
            get { return targetSize; }
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
        /// Gets custom parent panel for adding custom own UI controls.
        /// This will be set to custom width/height dimensions.
        /// </summary>
        public Panel ParentPanel
        {
            get { return parentPanel; }
        }

        /// <summary>
        /// Gets or sets custom clear colour.
        /// </summary>
        public Color ClearColor
        {
            get { return clearColor; }
            set { clearColor = value; }
        }

        /// <summary>
        /// Gets or sets render texture filter mode.
        /// </summary>
        public FilterMode FilterMode
        {
            get { return filterMode; }
            set { filterMode = value; }
        }

        #endregion

        #region Unity

        private void Awake()
        {
            parentPanel = new Panel();
            filterMode = DaggerfallUI.Instance.GlobalFilterMode;
        }

        private void Start()
        {
            CheckTargetTexture();
        }

        private void Update()
        {
            CheckTargetTexture();

            // Update parent panel
            parentPanel.Update();
        }

        private void OnGUI()
        {
            GUI.depth = 0;

            if (Event.current.type != EventType.Repaint || !EnableCustomStack)
                return;

            RenderTexture oldRt = RenderTexture.active;
            RenderTexture.active = targetTexture;

            if (IsReady())
            {
                // Clear UI
                Clear(clearColor);

                // Draw parent panel
                parentPanel.Draw();
            }

            RenderTexture.active = oldRt;
        }

        #endregion

        #region Drawing Methods

        public void Clear()
        {
            GL.Clear(true, true, Color.clear);
        }

        public void Clear(Color color)
        {
            GL.Clear(true, true, color);
        }

        public void DrawTexture(Rect position, Texture2D image)
        {
            if (!IsReady())
                return;

            DaggerfallUI.DrawTexture(position, image);
        }

        public void DrawTexture(Rect position, Texture2D image, ScaleMode scaleMode, bool alphaBlend = true, float imageAspect = 0)
        {
            if (!IsReady())
                return;

            DaggerfallUI.DrawTexture(position, image, scaleMode, alphaBlend);
        }

        public void DrawTextureWithTexCoords(Rect position, Texture image, Rect texCoords, bool alphaBlend = true)
        {
            if (!IsReady())
                return;

            DaggerfallUI.DrawTextureWithTexCoords(position, image, texCoords, alphaBlend);
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
            targetSize = new Vector2(width, height);

            // Set custom panel root size and scale
            // Unity likes to scale drawing to render texture by current screen dimensions
            // This is a hack to use panel scaling to compensate
            float scaleX = (float)Screen.width / (float)CustomWidth;
            float scaleY = (float)Screen.height / (float)CustomHeight;
            parentPanel.RootSize = targetSize;
            parentPanel.Scale = new Vector2(scaleX, scaleY);
            parentPanel.AutoSize = AutoSizeModes.None;

            // Just return same texture if still valid
            if (!IsReady() || targetTexture.width != width || targetTexture.height != height)
            {
                // Create target texture matching screen dimensions
                targetTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
                targetTexture.filterMode = filterMode;
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