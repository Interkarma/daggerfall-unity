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

//#define DEBUG_BUTTON_PLACEMENT
//#define DEBUG_BUTTON_CLICKS

using UnityEngine;

namespace DaggerfallWorkshop.Game.UserInterface
{
    /// <summary>
    /// A simple button component.
    /// </summary>
    public class Button : Panel
    {
        TextLabel label = new TextLabel();
        AudioClip clickSound;
        HotkeySequence shortcutKey = HotkeySequence.None;

        public string ClickMessage { get; set; }
        public string DoubleClickMessage { get; set; }
        public bool DefaultButton { get; set; }

        public TextLabel Label
        {
            get { return label; }
        }

        public AudioClip ClickSound
        {
            get { return clickSound; }
            set { clickSound = value; }
        }

        public HotkeySequence Hotkey
        {
            get { return shortcutKey; }
            set { shortcutKey = value; }
        }

        public Button()
            : base()
        {
            label.Parent = this;
            label.HorizontalAlignment = UserInterface.HorizontalAlignment.Center;
            label.VerticalAlignment = UserInterface.VerticalAlignment.Middle;
            label.TextColor = DaggerfallUI.DaggerfallDefaultTextColor;
            label.ShadowColor = DaggerfallUI.DaggerfallDefaultShadowColor;
            label.ShadowPosition = DaggerfallUI.DaggerfallDefaultShadowPos;

            OnMouseClick += ClickHandler;
            OnMouseDoubleClick += DoubleClickHandler;

#if DEBUG_BUTTON_PLACEMENT
            BackgroundColor = new Color(1, 1, 0, 0.25f);
#endif
        }

        public override void Update()
        {
            base.Update();
            label.Update();
        }

        public override void Draw()
        {
            base.Draw();
            label.Draw();
        }

        new public bool ProcessHotkeySequences(HotkeySequence.KeyModifiers keyModifiers)
        {
            bool isKeyDown = DaggerfallUI.Instance.KeyEvent.type == EventType.KeyDown;
            bool isActivated = isKeyDown ? shortcutKey.IsDownWith(keyModifiers) : shortcutKey.IsUpWith(keyModifiers);
            if (isActivated)
            {
                if (!KeyboardEvent(DaggerfallUI.Instance.KeyEvent))
                {
                    // Legacy support fallback, OnMouseClick handlers receive KeyDown events as faked clicks
                    if (isKeyDown)
                        TriggerMouseClick();
                }
            }
            return isActivated;
        }

        void ClickHandler(BaseScreenComponent sender, Vector2 position)
        {
            if (clickSound != null)
                DaggerfallUI.Instance.PlayOneShot(clickSound);

            if (!string.IsNullOrEmpty(ClickMessage))
            {
                DaggerfallUI.PostMessage(ClickMessage);

#if DEBUG_BUTTON_CLICKS
                Debug.Log("Sending click message " + ClickMessage);
#endif
            }
        }

        void DoubleClickHandler(BaseScreenComponent sender, Vector2 position)
        {
            if (!string.IsNullOrEmpty(DoubleClickMessage))
            {
                DaggerfallUI.PostMessage(DoubleClickMessage);

#if DEBUG_BUTTON_CLICKS
                Debug.Log("Sending double-click message " + DoubleClickMessage);
#endif
            }
        }
    }
}
