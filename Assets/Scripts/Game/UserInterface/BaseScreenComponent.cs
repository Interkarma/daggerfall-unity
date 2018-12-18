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
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DaggerfallWorkshop.Game.UserInterface
{
    /// <summary>
    /// Provides features common to all screen components.
    /// </summary>
    public abstract class BaseScreenComponent : IDisposable
    {
        #region Fields

        const int colorTextureDim = 8;

        bool enabled;
        string name;
        object tag;
        BaseScreenComponent parent;
        Vector2 position;
        Vector2 size;
        Vector2 rootSize;
        bool useFocus = false;

        ToolTip toolTip = null;
        string toolTipText = string.Empty;
        bool suppressToolTip = false;

        Vector2 scale = Vector2.one;
        Vector2 localScale = Vector2.one;
        AutoSizeModes autoSizeMode = AutoSizeModes.None;
        HorizontalAlignment horizontalAlignment = HorizontalAlignment.None;
        VerticalAlignment verticalAlignment = VerticalAlignment.None;

        // restricted render area can be used to force background rendering inside this rect (must be used in conjunction with ui elements that also support restricted render area like textlabel)
        protected bool useRestrictedRenderArea = false;
        protected Rect rectRestrictedRenderArea;

        float doubleClickDelay = 0.3f;
        float leftClickTime;
        float lastLeftClickTime;
        float rightClickTime;
        float lastRightClickTime;
        float middleClickTime;
        float lastMiddleClickTime;

        float updateTime;
        float lastUpdateTime;
        float hoverTime;

        Vector2 lastMousePosition;
        Vector2 mousePosition;
        Vector2 lastScaledMousePosition;
        Vector2 scaledMousePosition;
        Vector2? customMousePosition = null;

        Color backgroundColor = Color.clear;
        Color mouseOverBackgroundColor = Color.clear;
        Texture2D backgroundColorTexture;
        protected Texture2D backgroundTexture;
        protected Texture2D[] animatedBackgroundTextures;
        protected BackgroundLayout backgroundTextureLayout = BackgroundLayout.StretchToFill;

        int animationFrame = 0;
        float animationDelay = 0.25f;
        float animationLastTickTime = 0f;

        bool mouseOverComponent = false;
        bool leftMouseWasHeldDown = false;
        bool rightMouseWasHeldDown = false;
        bool middleMouseWasHeldDown = false;

        float minAutoScale = 0;
        float maxAutoScale = 0;

        public delegate void OnMouseEnterHandler(BaseScreenComponent sender);
        public event OnMouseEnterHandler OnMouseEnter;

        public delegate void OnMouseLeaveHandler(BaseScreenComponent sender);
        public event OnMouseLeaveHandler OnMouseLeave;

        public delegate void OnMouseMoveHandler(int x, int y);
        public event OnMouseMoveHandler OnMouseMove;

        public delegate void OnMouseDownHandler(BaseScreenComponent sender, Vector2 position);
        public event OnMouseDownHandler OnMouseDown;

        public delegate void OnMouseUpHandler(BaseScreenComponent sender, Vector2 position);
        public event OnMouseUpHandler OnMouseUp;

        public delegate void OnMouseClickHandler(BaseScreenComponent sender, Vector2 position);
        public event OnMouseClickHandler OnMouseClick;

        public delegate void OnMouseDoubleClickHandler(BaseScreenComponent sender, Vector2 position);
        public event OnMouseDoubleClickHandler OnMouseDoubleClick;

        public delegate void OnRightMouseDownHandler(BaseScreenComponent sender, Vector2 position);
        public event OnRightMouseDownHandler OnRightMouseDown;
        
        public delegate void OnRightMouseUpHandler(BaseScreenComponent sender, Vector2 position);
        public event OnRightMouseUpHandler OnRightMouseUp;

        public delegate void OnRightMouseClickHandler(BaseScreenComponent sender, Vector2 position);
        public event OnRightMouseClickHandler OnRightMouseClick;

        public delegate void OnRightMouseDoubleClickHandler(BaseScreenComponent sender, Vector2 position);
        public event OnRightMouseDoubleClickHandler OnRightMouseDoubleClick;

        public delegate void OnMiddleMouseDownHandler(BaseScreenComponent sender, Vector2 position);
        public event OnMiddleMouseDownHandler OnMiddleMouseDown;

        public delegate void OnMiddleMouseUpHandler(BaseScreenComponent sender, Vector2 position);
        public event OnMiddleMouseUpHandler OnMiddleMouseUp;

        public delegate void OnMiddleMouseClickHandler(BaseScreenComponent sender, Vector2 position);
        public event OnMiddleMouseClickHandler OnMiddleMouseClick;

        public delegate void OnMiddleMouseDoubleClickHandler(BaseScreenComponent sender, Vector2 position);
        public event OnMiddleMouseDoubleClickHandler OnMiddleMouseDoubleClick;

        public delegate void OnMouseScrollUpEventHandler(BaseScreenComponent sender);
        public event OnMouseScrollUpEventHandler OnMouseScrollUp;

        public delegate void OnMouseScrollDownEventHandler(BaseScreenComponent sender);
        public event OnMouseScrollDownEventHandler OnMouseScrollDown;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets enabled flag.
        /// </summary>
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        /// <summary>
        /// Gets or sets flag to make control focus-senstitive.
        /// When enabled, this control will gain focus when clicked and lose focus when another control is clicked.
        /// How focus is implemented depends on inherited control.
        /// </summary>
        public bool UseFocus
        {
            get { return useFocus; }
            set { useFocus = value; }
        }

        /// <summary>
        /// Gets or sets name.
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Gets or sets custom tag.
        /// </summary>
        public object Tag
        {
            get { return tag; }
            set { tag = value; }
        }

        /// <summary>
        /// Gets position relative to parent panel.
        /// </summary>
        public virtual Vector2 Position
        {
            get { return position; }
            internal set { position = value;}
        }

        /// <summary>
        /// Gets size of component.
        /// </summary>
        public virtual Vector2 Size
        {
            get { return size; }
            internal set { size = value; }
        }

        /// <summary>
        /// Gets or sets root size when no parent is avaialable.
        /// Should only be set for root panel in UI stack.
        /// If root size is not set then screen dimensions will be used.
        /// </summary>
        public Vector2 RootSize
        {
            get { return rootSize; }
            set { rootSize = value; }
        }

        /// <summary>
        /// Gets parent panel.
        /// </summary>
        public virtual BaseScreenComponent Parent
        {
            get { return parent; }
            internal set { SetParent(value); }
        }

        /// <summary>
        /// Gets screen area occupied by component.
        /// </summary>
        public Rect Rectangle
        {
            get { return GetRectangle(); }
        }

        /// <summary>
        /// Gets or sets horizontal alignment.
        /// </summary>
        public HorizontalAlignment HorizontalAlignment
        {
            get { return horizontalAlignment; }
            set { horizontalAlignment = value; }
        }

        /// <summary>
        /// Gets or sets vertical alignment.
        /// </summary>
        public VerticalAlignment VerticalAlignment
        {
            get { return verticalAlignment; }
            set { verticalAlignment = value; }
        }

        /// <summary>
        /// get/set a restricted render area for background rendering - the background will only be rendered inside the specified Rect's bounds
        /// </summary>
        public Rect RectRestrictedRenderArea
        {
            get { return rectRestrictedRenderArea; }
            set
            {
                rectRestrictedRenderArea = value;
                useRestrictedRenderArea = true;
            }
        }

        /// <summary>
        /// Gets or sets custom mouse position.
        /// When null the screen mouse position will be used from Input system as normal.
        /// When non-null this value will be used in place of mouse coordinates from Input system.
        /// Will propagate down through Panel hierarchy from root to leaf controls.
        /// Allows for custom pointer input from rays and other sources.
        /// Input must be in "pixel coordinates" relatve to root panel dimensions.
        /// For example, if root panel is 256x256 pixels then coordinates are between 0,0 and 255,255.
        /// </summary>
        public Vector2? CustomMousePosition
        {
            get { return customMousePosition; }
            set { customMousePosition = value; }
        }

        /// <summary>
        /// Gets current mouse position from recent update.
        /// </summary>
        public Vector2 MousePosition
        {
            get { return mousePosition; }
        }

        /// <summary>
        /// Gets previous mouse position from last update.
        /// </summary>
        public Vector2 LastMousePosition
        {
            get { return lastMousePosition; }
        }

        /// <summary>
        /// Gets scaled mouse position relative to top-left of this control.
        /// If mouse is outside of control area this will return -1,-1.
        /// </summary>
        public Vector2 ScaledMousePosition
        {
            get { return scaledMousePosition; }
        }

        /// <summary>
        /// Gets or sets background colour.
        /// </summary>
        public Color BackgroundColor
        {
            get { return backgroundColor; }
            set { SetBackgroundColor(value); }
        }

        /// <summary>
        /// Gets or sets alternate background colour when mouse is over control.
        /// This colour will replace any other background colour for the time mouse is over control.
        /// </summary>
        public Color MouseOverBackgroundColor
        {
            get { return mouseOverBackgroundColor; }
            set { SetMouseOverBackgroundColor(value); }
        }

        /// <summary>
        /// Gets or sets background colour texture.
        /// </summary>
        public Texture2D BackgroundColorTexture
        {
            get { return backgroundColorTexture; }
            set { backgroundColorTexture = value; }
        }

        /// <summary>
        /// Gets or sets background texture.
        ///  Will replace BackgroundColor if set.
        /// </summary>
        public Texture2D BackgroundTexture
        {
            get { return backgroundTexture; }
            set { backgroundTexture = value; }
        }

        /// <summary>
        /// Gets or sets array of background textures for animated background.
        /// Set null to disable animated background.
        /// Any existing background texture is removed when setting an animated background.
        /// </summary>
        public Texture2D[] AnimatedBackgroundTextures
        {
            get { return animatedBackgroundTextures; }
            set
            {
                backgroundTexture = null;
                animatedBackgroundTextures = value;
                if (animatedBackgroundTextures != null && animationFrame >= animatedBackgroundTextures.Length)
                    animationFrame = 0;
            }
        }

        /// <summary>
        /// Gets or sets animation delay in seconds.
        /// </summary>
        public float AnimationDelayInSeconds
        {
            get { return animationDelay; }
            set { animationDelay = value; }
        }

        /// <summary>
        /// Gets or sets background texture layout behaviour.
        /// </summary>
        public BackgroundLayout BackgroundTextureLayout
        {
            get { return backgroundTextureLayout; }
            set { backgroundTextureLayout = value; }
        }

        /// <summary>
        /// Gets interior width between horizontal margins.
        /// </summary>
        public int InteriorWidth
        {
            get { return GetInteriorWidth(); }
        }

        /// <summary>
        /// Gets interior height between vertical margins.
        /// </summary>
        public int InteriorHeight
        {
            get { return GetInteriorHeight(); }
        }

        /// <summary>
        /// Gets or sets manual scale for when ScalingMode = None.
        /// </summary>
        public Vector2 Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        /// <summary>
        /// Gets or sets autosize mode.
        /// This is how control resizes itself to parent.
        /// </summary>
        public AutoSizeModes AutoSize
        {
            get { return autoSizeMode; }
            set { autoSizeMode = value; }
        }

        /// <summary>
        /// Gets local scale in screen space based on parent scale and scaling mode.
        /// </summary>
        public Vector2 LocalScale
        {
            get { return localScale; }
        }

        /// <summary>
        /// Gets or sets tooltip for this component.
        /// </summary>
        public ToolTip ToolTip
        {
            get { return toolTip; }
            set { toolTip = value; }
        }

        /// <summary>
        /// Gets or sets tooltip text for this component.
        /// </summary>
        public string ToolTipText
        {
            get { return toolTipText; }
            set { toolTipText = value; }
        }

        /// <summary>
        /// Gets or sets tooltip suppression, preventing tooltip from drawing.
        /// Tooltip hover time will be scrubbed on resume.
        /// </summary>
        public bool SuppressToolTip
        {
            get { return suppressToolTip; }
            set { SetSuppressToolTip(value); }
        }

        /// <summary>
        /// Gets or sets lower autoscale clamp. 0 to disable.
        /// </summary>
        public float MinAutoScale
        {
            get { return minAutoScale; }
            set { minAutoScale = value; }
        }

        /// <summary>
        /// Gets or sets upper autoscale clamp. 0 to disable.
        /// </summary>
        public float MaxAutoScale
        {
            get { return maxAutoScale; }
            set { maxAutoScale = value; }
        }

        /// <summary>
        /// True if mouse over this component.
        /// </summary>
        public bool MouseOverComponent
        {
            get { return mouseOverComponent; }
        }

        // Margin properties
        public int TopMargin { get; set; }
        public int BottomMargin { get; set; }
        public int LeftMargin { get; set; }
        public int RightMargin { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public BaseScreenComponent()
        {
            this.enabled = true;
            this.tag = null;
            this.position = Vector2.zero;
            this.size = Vector2.zero;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Called when screen component should update itself.
        /// </summary>
        public virtual void Update()
        {
            // Do nothing if disabled
            if (!enabled)
                return;

            // Update timers
            lastUpdateTime = updateTime;
            updateTime = Time.realtimeSinceStartup;

            // Get new mouse position
            lastMousePosition = mousePosition;
            if (customMousePosition != null)
            {
                // Use custom mouse position
                mousePosition = customMousePosition.Value;
            }
            else
            {
                // Update raw mouse screen position from Input - must invert mouse position Y as Unity 0,0 is bottom-left
                mousePosition = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
            }
            scaledMousePosition = -Vector2.one;

            // Check if mouse is inside rectangle
            Rect myRect = Rectangle;
            if (myRect.Contains(mousePosition))
            {
                if (mouseOverComponent == false)
                {
                    // Raise mouse entered event
                    MouseEnter();
                    mouseOverComponent = true;
                }
            }
            else
            {
                if (mouseOverComponent == true)
                {
                    // Raise mouse leaving event
                    MouseLeave(this);
                    mouseOverComponent = false;
                }
            }

            // When mouse is inside component
            if (mouseOverComponent)
            {
                // Update hover time
                if (lastMousePosition == mousePosition)
                    hoverTime += updateTime - lastUpdateTime;
                else
                    hoverTime = 0;

                // Get scaled mouse position relative to top-left of control
                scaledMousePosition = mousePosition - new Vector2(myRect.xMin, myRect.yMin);
                scaledMousePosition.x *= 1f / localScale.x;
                scaledMousePosition.y *= 1f / localScale.y;

                // Mouse moved
                if (scaledMousePosition != lastScaledMousePosition)
                {
                    lastScaledMousePosition = scaledMousePosition;
                    if (OnMouseMove != null)
                        OnMouseMove((int)scaledMousePosition.x, (int)scaledMousePosition.y);
                }
            }
            else
            {
                hoverTime = 0;
            }

            // Get left and right mouse down for general click handling and double-click sampling
            bool leftMouseDown = Input.GetMouseButtonDown(0);
            bool rightMouseDown = Input.GetMouseButtonDown(1);
            bool middleMouseDown = Input.GetMouseButtonDown(2);

            // Get left and right mouse down for up/down events
            bool leftMouseHeldDown = Input.GetMouseButton(0);
            bool rightMouseHeldDown = Input.GetMouseButton(1);
            bool middleMouseHeldDown = Input.GetMouseButton(2);

            // Handle left mouse down/up events
            // Can only trigger mouse down while over component but can release from anywhere
            if (mouseOverComponent && leftMouseHeldDown && !leftMouseWasHeldDown)
            {
                leftMouseWasHeldDown = true;
                if (OnMouseDown != null)
                    OnMouseDown(this, scaledMousePosition);
            }
            if (!leftMouseHeldDown && leftMouseWasHeldDown)
            {
                leftMouseWasHeldDown = false;
                if (OnMouseUp != null)
                    OnMouseUp(this, scaledMousePosition);
            }

            // Handle right mouse down/up events
            // Can only trigger mouse down while over component but can release from anywhere
            if (mouseOverComponent && rightMouseHeldDown && !rightMouseWasHeldDown)
            {
                rightMouseWasHeldDown = true;
                if (OnRightMouseDown != null)
                    OnRightMouseDown(this, scaledMousePosition);
            }
            if (!rightMouseHeldDown && rightMouseWasHeldDown)
            {
                rightMouseWasHeldDown = false;
                if (OnRightMouseUp != null)
                    OnRightMouseUp(this, scaledMousePosition);
            }

            // Handle middle mouse down/up events
            // Can only trigger mouse down while over component but can release from anywhere
            if (mouseOverComponent && middleMouseHeldDown && !middleMouseWasHeldDown)
            {
                middleMouseWasHeldDown = true;
                if (OnMiddleMouseDown != null)
                    OnMiddleMouseDown(this, scaledMousePosition);
            }
            if (!middleMouseHeldDown && middleMouseWasHeldDown)
            {
                middleMouseWasHeldDown = false;
                if (OnMiddleMouseUp != null)
                    OnMiddleMouseUp(this, scaledMousePosition);
            }

            // Handle left mouse clicks
            if (mouseOverComponent && leftMouseDown)
            {
                // Single click event
                MouseClick(scaledMousePosition);

                // Store mouse click timing
                lastLeftClickTime = leftClickTime;
                leftClickTime = Time.realtimeSinceStartup;

                // Handle left mouse double-clicks
                if (leftClickTime - lastLeftClickTime < doubleClickDelay)
                    MouseDoubleClick(scaledMousePosition);
            }

            // Handle right mouse clicks
            if (mouseOverComponent && rightMouseDown)
            {
                // Single click event
                RightMouseClick(scaledMousePosition);

                // Store mouse click timing
                lastRightClickTime = rightClickTime;
                rightClickTime = Time.realtimeSinceStartup;

                // Handle right mouse double-clicks
                if (rightClickTime - lastRightClickTime < doubleClickDelay)
                    RightMouseDoubleClick(scaledMousePosition);
            }

            // Handle middle mouse clicks
            if (mouseOverComponent && middleMouseDown)
            {
                // Single click event
                MiddleMouseClick(scaledMousePosition);

                // Store mouse click timing
                lastMiddleClickTime = middleClickTime;
                middleClickTime = Time.realtimeSinceStartup;

                // Handle middle mouse double-clicks
                if (middleClickTime - lastMiddleClickTime < doubleClickDelay)
                    MiddleMouseDoubleClick(scaledMousePosition);
            }

            // Handle mouse wheel
            float mouseScroll = Input.GetAxis("Mouse ScrollWheel");
            if (mouseOverComponent && mouseScroll != 0)
            {
                if (mouseScroll > 0)
                    MouseScrollUp();
                else if (mouseScroll < 0)
                    MouseScrollDown();

                // Not hovering while scrolling
                hoverTime = 0;
            }
        }

        /// <summary>
        /// Called when screen component should draw itself.
        /// </summary>
        public virtual void Draw()
        {
            // Do nothing if disabled
            if (!enabled)
                return;

            // Animated background textures
            if (animatedBackgroundTextures != null && animatedBackgroundTextures.Length > 0)
            {
                // Tick animation
                if (Time.realtimeSinceStartup > animationLastTickTime + animationDelay)
                {
                    if (++animationFrame >= animatedBackgroundTextures.Length)
                        animationFrame = 0;

                    animationLastTickTime = Time.realtimeSinceStartup;
                }

                // Assign current frame as background texture reference
                backgroundTexture = animatedBackgroundTextures[animationFrame];
            }

            // Calculate cutout rect
            Rect myRect = Rectangle;
            if (useRestrictedRenderArea)
            {
                Rect rect = new Rect(this.Parent.Position + this.Position, this.Size);

                Vector2 parentScale = parent.LocalScale;

                float leftCut = Math.Max(0, rectRestrictedRenderArea.xMin - rect.xMin) * parentScale.x;
                float rightCut = Math.Max(0, rect.xMax - rectRestrictedRenderArea.xMax) * parentScale.x;
                float topCut = Math.Max(0, rectRestrictedRenderArea.yMin - rect.yMin) * parentScale.y;
                float bottomCut = Math.Max(0, rect.yMax - rectRestrictedRenderArea.yMax) * parentScale.y;
                
                myRect = new Rect(new Vector2(Rectangle.xMin + leftCut, Rectangle.yMin + topCut), new Vector2(Rectangle.width - leftCut - rightCut, Rectangle.height - topCut - bottomCut));
            }

            // Draw background colour or mouse over background colour
            if (mouseOverComponent && mouseOverBackgroundColor != Color.clear && backgroundColorTexture)
            {
                Color color = GUI.color;
                GUI.color = mouseOverBackgroundColor;
                GUI.DrawTexture(myRect, backgroundColorTexture, ScaleMode.StretchToFill);
                GUI.color = color;
            }
            else if (backgroundColor != Color.clear && backgroundColorTexture)
            {
                Color color = GUI.color;
                GUI.color = backgroundColor;
                GUI.DrawTexture(myRect, backgroundColorTexture, ScaleMode.StretchToFill);
                GUI.color = color;
            }

            // Draw background texture if present
            if (backgroundTexture)
            {
                switch (backgroundTextureLayout)
                {
                    case BackgroundLayout.Tile:
                        backgroundTexture.wrapMode = TextureWrapMode.Repeat;
                        GUI.DrawTextureWithTexCoords(myRect, backgroundTexture, new Rect(0, 0, myRect.width / backgroundTexture.width, myRect.height / backgroundTexture.height));
                        break;
                    case BackgroundLayout.StretchToFill:
                        backgroundTexture.wrapMode = TextureWrapMode.Clamp;
                        GUI.DrawTexture(myRect, backgroundTexture, ScaleMode.StretchToFill);
                        break;
                    case BackgroundLayout.ScaleToFit:
                        backgroundTexture.wrapMode = TextureWrapMode.Clamp;
                        GUI.DrawTexture(myRect, backgroundTexture, ScaleMode.ScaleToFit);
                        break;
                }
            }

            // Draw tooltip on mouse hover
            if (toolTip != null && mouseOverComponent && hoverTime >= toolTip.ToolTipDelay)
            {
                if (!suppressToolTip)
                    toolTip.Draw(toolTipText);
            }
        }

        /// <summary>
        /// Converts a screen position to a local position relative to this control.
        /// </summary>
        /// <param name="screenPosition">Screen position.</param>
        /// <returns>Local position.</returns>
        public Vector2 ScreenToLocal(Vector2 screenPosition)
        {
            Rect myRect = Rectangle;
            float x = (screenPosition.x - myRect.x) / LocalScale.x;
            float y = (screenPosition.y - myRect.y) / LocalScale.y;

            Vector2 localPosition = new Vector2(x, y);

            return localPosition;
        }

        /// <summary>
        /// Converts a screen rect to a local rect relative to this control.
        /// </summary>
        /// <param name="screenRect">Screen rect.</param>
        /// <returns>Local rect.</returns>
        public Rect ScreenToLocal(Rect screenRect)
        {
            Rect myRect = Rectangle;
            float x = (screenRect.x - myRect.x) / LocalScale.x;
            float y = (screenRect.y - myRect.y) / LocalScale.y;
            float width = screenRect.width / LocalScale.x;
            float height = screenRect.height / LocalScale.y;

            Rect localRect = new Rect(x, y, width, height);

            return localRect;
        }

        /// <summary>
        /// Called when control gains focus in window.
        /// </summary>
        public virtual void GotFocus()
        {
        }

        /// <summary>
        /// Called when control loses focus in window.
        /// </summary>
        public virtual void LostFocus()
        {
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Mouse clicked inside control area.
        /// </summary>
        protected virtual void MouseClick(Vector2 clickPosition)
        {
            if (OnMouseClick != null)
                OnMouseClick(this, clickPosition);


            // Set focus on click
            if (UseFocus)
                SetFocus();
        }

        /// <summary>
        /// Right Mouse clicked inside control area.
        /// </summary>
        protected virtual void RightMouseClick(Vector2 clickPosition)
        {
            if (OnRightMouseClick != null)
                OnRightMouseClick(this, clickPosition);

            // Set focus on click
            if (UseFocus)
                SetFocus();
        }

        /// <summary>
        /// Middle Mouse clicked inside control area.
        /// </summary>
        protected virtual void MiddleMouseClick(Vector2 clickPosition)
        {
            if (OnMiddleMouseClick != null)
                OnMiddleMouseClick(this, clickPosition);

            // Set focus on click
            if (UseFocus)
                SetFocus();
        }

        /// <summary>
        /// Mouse double-clicked inside control area.
        /// </summary>
        protected virtual void MouseDoubleClick(Vector2 clickPosition)
        {
            if (OnMouseDoubleClick != null)
                OnMouseDoubleClick(this, clickPosition);
        }

        /// <summary>
        /// Right Mouse double-clicked inside control area.
        /// </summary>
        protected virtual void RightMouseDoubleClick(Vector2 clickPosition)
        {
            if (OnRightMouseDoubleClick != null)
                OnRightMouseDoubleClick(this, clickPosition);
        }

        /// <summary>
        /// Middle Mouse double-clicked inside control area.
        /// </summary>
        protected virtual void MiddleMouseDoubleClick(Vector2 clickPosition)
        {
            if (OnMiddleMouseDoubleClick != null)
                OnMiddleMouseDoubleClick(this, clickPosition);
        }

        /// <summary>
        /// Mouse entered control area.
        /// </summary>
        protected virtual void MouseEnter()
        {
            if (OnMouseEnter != null)
                OnMouseEnter(this);
        }

        /// <summary>
        /// Mouse exited control area.
        /// </summary>
        protected virtual void MouseLeave(BaseScreenComponent sender)
        {
            if (OnMouseLeave != null)
                OnMouseLeave(this);
        }

        /// <summary>
        /// Mouse is moving.
        /// </summary>
        protected virtual void MouseMove(int x, int y)
        {
            if (OnMouseMove != null)
                OnMouseMove(x, y);
        }

        /// <summary>
        /// Mouse wheel scrolled up.
        /// </summary>
        protected virtual void MouseScrollUp()
        {
            if (OnMouseScrollUp != null)
                OnMouseScrollUp(this);
        }

        /// <summary>
        /// Mouse wheel scrolled down.
        /// </summary>
        protected virtual void MouseScrollDown()
        {
            if (OnMouseScrollDown != null)
                OnMouseScrollDown(this);
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// Called when component is to be disposed.
        ///  Override if special handling needed
        ///  to dispose of component resources.
        /// </summary>
        public virtual void Dispose()
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets value for one or more margins using flags.
        /// </summary>
        /// <param name="margin">Margin flags.</param>
        /// <param name="value">Value to set.</param>
        public void SetMargins(Margins margin, int value)
        {
            if ((margin & Margins.Top) == Margins.Top)
                TopMargin = value;
            if ((margin & Margins.Bottom) == Margins.Bottom)
                BottomMargin = value;
            if ((margin & Margins.Left) == Margins.Left)
                LeftMargin = value;
            if ((margin & Margins.Right) == Margins.Right)
                RightMargin = value;
        }

        /// <summary>
        /// Offsets position of component relative to another component.
        /// </summary>
        /// <param name="component">Component to offset against.</param>
        /// <param name="side">The side of the component to offset from.</param>
        /// <param name="distance">Distance between offset components.</param>
        public void OffsetFrom(BaseScreenComponent component, Sides side, int distance)
        {
            // Exit if invalid offset
            if (component == null || side == Sides.None)
                return;

            // Get rectangles
            Rect myRect = Rectangle;
            Rect otherRect = component.Rectangle;

            // Offset based on side
            switch (side)
            {
                case Sides.Left:
                    position.x = otherRect.xMin - distance - myRect.width;
                    break;
                case Sides.Right:
                    position.x = otherRect.xMax + distance;
                    break;
                case Sides.Top:
                    position.y = otherRect.yMin - distance - myRect.height;
                    break;
                case Sides.Bottom:
                    position.y = otherRect.yMax + distance;
                    break;
            }
        }

        /// <summary>
        /// Set focus to this control.
        /// </summary>
        public void SetFocus()
        {
            IUserInterfaceWindow topWindow = DaggerfallUI.UIManager.TopWindow;
            if (topWindow != null)
            {
                topWindow.SetFocus(this);
            }
        }

        /// <summary>
        /// Checks if this control has focus.
        /// </summary>
        public bool HasFocus()
        {
            IUserInterfaceWindow topWindow = DaggerfallUI.UIManager.TopWindow;
            if (topWindow != null)
            {
                if (topWindow.FocusControl == this)
                    return true;
            }

            return false;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Sets parent panel.
        /// Should only be used at setup, never to move components between collections.
        /// </summary>
        /// <param name="parent">Parent.</param>
        private void SetParent(BaseScreenComponent parent)
        {
            this.parent = parent;
        }

        /// <summary>
        /// Helper to get parent rectangle.
        /// </summary>
        /// <returns>Parent's absolute rectangle in viewport area.</returns>
        private Rect GetParentRectangle()
        {
            Rect parentRect = new Rect();

            if (parent != null)
            {
                parentRect = parent.Rectangle;
            }
            else
            {
                if (rootSize == Vector2.zero)
                    parentRect = new Rect(0, 0, Screen.width, Screen.height);
                else
                    parentRect = new Rect(0, 0, rootSize.x, rootSize.y);
            }

            return parentRect;
        }

        /// <summary>
        /// Gets current screen space rectangle.
        /// </summary>
        /// <returns>Absolute rectangle in viewport area.</returns>
        private Rect GetRectangle()
        {
            Rect rectangle = new Rect();

            // Apply starting position and size
            Rect parentRect = GetParentRectangle();
            if (parent == null)
            {
                // Top-level panel always stretches to fill entire viewport
                rectangle = parentRect;
                size = new Vector2(rectangle.width, rectangle.height);
            }
            else
            {
                // Other panels are scaled within parent area
                rectangle.x = (int)parentRect.xMin;
                rectangle.y = (int)parentRect.yMin;
                rectangle.width = (int)size.x;
                rectangle.height = (int)size.y;
            }

            // Apply scaling
            switch (autoSizeMode)
            {
                case AutoSizeModes.None:
                    localScale = (parent != null) ? parent.LocalScale : scale;
                    break;
                case AutoSizeModes.ResizeToFill:
                    rectangle = ResizeToFill(rectangle);
                    break;
                case AutoSizeModes.ScaleToFit:
                    rectangle = ScaleToFit(rectangle);
                    break;
                case AutoSizeModes.ScaleFreely:
                    rectangle = ScaleFreely(rectangle);
                    break;
            }

            if (parent != null)
            {
                // Scale to parent
                Vector2 parentScale = parent.LocalScale;
                rectangle.x += position.x * parentScale.x;
                rectangle.y += position.y * parentScale.y;
                rectangle.width *= parentScale.x;
                rectangle.height *= parentScale.y;

                // Apply horizontal alignment
                switch (horizontalAlignment)
                {
                    case HorizontalAlignment.None:
                        rectangle.x = parentRect.xMin + (Parent.LeftMargin + position.x) * parentScale.x;
                        break;
                    case HorizontalAlignment.Left:
                        rectangle.x = parentRect.xMin + (Parent.LeftMargin * parentScale.x);
                        break;
                    case HorizontalAlignment.Right:
                        rectangle.x = parentRect.xMax - (Parent.RightMargin * parentScale.x) - rectangle.width;
                        break;
                    case HorizontalAlignment.Center:
                        rectangle.x = parentRect.xMin + parentRect.width / 2 - rectangle.width / 2;
                        break;
                }

                // Set vertical position based on alignment
                switch (verticalAlignment)
                {
                    case VerticalAlignment.None:
                        rectangle.y = parentRect.yMin + (Parent.TopMargin + position.y) * parentScale.y;
                        break;
                    case VerticalAlignment.Top:
                        rectangle.y = parentRect.yMin + (Parent.TopMargin * parentScale.y);
                        break;
                    case VerticalAlignment.Bottom:
                        rectangle.y = parentRect.yMax - (Parent.BottomMargin * parentScale.y) - rectangle.height;
                        break;
                    case VerticalAlignment.Middle:
                        rectangle.y = parentRect.yMin + parentRect.height / 2 - rectangle.height / 2;
                        break;
                }
            }

            return rectangle;
        }

        /// <summary>
        /// Gets interior width between horizontal margins.
        /// </summary>
        private int GetInteriorWidth()
        {
            return (int)Size.x - LeftMargin - RightMargin;
        }

        /// <summary>
        /// Gets interior height between vertical margins.
        /// </summary>
        private int GetInteriorHeight()
        {
            return (int)Size.y - TopMargin - BottomMargin;
        }

        /// <summary>
        /// Resize to fill parent.
        /// </summary>
        private Rect ResizeToFill(Rect srcRect)
        {
            Rect finalRect = srcRect;

            if (parent != null)
            {
                size.x = parent.InteriorWidth;
                size.y = parent.InteriorHeight;

                finalRect.width = size.x;
                finalRect.height = size.y;
            }

            return finalRect;
        }

        /// <summary>
        /// Scale to fit parent while maintaining aspect ratio.
        /// </summary>
        private Rect ScaleToFit(Rect srcRect)
        {
            Rect finalRect = srcRect;

            if (parent != null)
            {
                int parentWidth = parent.InteriorWidth;
                int parentHeight = parent.InteriorHeight;

                float scale;
                if (parentWidth > parentHeight)
                    scale = parentHeight / size.y;
                else
                    scale = parentWidth / size.x;

                if (finalRect.width * scale > parentWidth)
                    scale = parentWidth / size.x;

                // Clamp minimum autoscale
                if (minAutoScale != 0)
                    if (scale < minAutoScale) scale = minAutoScale;

                // Clamp maximum autoscale
                if (maxAutoScale != 0)
                    if (scale > maxAutoScale) scale = maxAutoScale;

                finalRect.width *= scale;
                finalRect.height *= scale;

                localScale.x = localScale.y = scale;
            }

            return finalRect;
        }

        /// <summary>
        /// Scale to fill parent with no regard to aspect ratio.
        /// </summary>
        private Rect ScaleFreely(Rect srcRect)
        {
            Rect finalRect = srcRect;

            if (parent != null)
            {
                int parentWidth = parent.InteriorWidth;
                int parentHeight = parent.InteriorHeight;

                float xScale = parentWidth / size.x;
                float yScale = parentHeight / size.y;

                finalRect.width *= xScale;
                finalRect.height *= yScale;

                localScale.x = xScale;
                localScale.y = yScale;
            }

            return finalRect;
        }

        /// <summary>
        /// Sets background colour and updates texture.
        /// </summary>
        /// <param name="color">Color to use as background colour.</param>
        private void SetBackgroundColor(Color color)
        {
            CreateBackgroundColorTexture();
            backgroundColor = color;
        }

        /// <summary>
        /// Sets mouse over background color and updates texture.
        /// </summary>
        /// <param name="color">Color to use as mouse over background colour.</param>
        private void SetMouseOverBackgroundColor(Color color)
        {
            CreateBackgroundColorTexture();
            mouseOverBackgroundColor = color;
        }

        /// <summary>
        /// Create a white texture for background colour setups.
        /// </summary>
        private void CreateBackgroundColorTexture()
        {
            if (backgroundColorTexture == null)
                backgroundColorTexture = DaggerfallUI.CreateSolidTexture(Color.white, colorTextureDim);
        }

        /// <summary>
        /// Suppress tooltip.
        /// </summary>
        /// <param name="suppress">True to suppress tooltip, false to resume.</param>
        private void SetSuppressToolTip(bool suppress)
        {
            // Scrub hover time on resume
            if (!suppress)
                hoverTime = 0;

            suppressToolTip = suppress;
        }

        #endregion
    }
}
