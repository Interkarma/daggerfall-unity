// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2016 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors: Justin Steele
//
// Notes:
//

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.Utility;


namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Implements controls window.
    /// </summary>
    public class DaggerfallControlsWindow : DaggerfallPopupWindow
    {
        #region Fields

        Texture2D nativeTexture;
        Texture2D mLookAltTexture;
        Panel controlsPanel = new Panel();
        Panel mLookAltPanel = new Panel();
        List<Button> moveKeysOne = new List<Button>();
        List<Button> moveKeysTwo = new List<Button>();
        List<Button> modeKeys = new List<Button>();
        List<Button> magicKeys = new List<Button>();
        List<Button> weaponKeys = new List<Button>();
        List<Button> statusKeys = new List<Button>();
        List<Button> activateKeys = new List<Button>();
        List<Button> lookKeys = new List<Button>();
        List<Button> uiKeys = new List<Button>();
        List<List<Button>> allKeys = new List<List<Button>>();

        string[] actions = Enum.GetNames(typeof(InputManager.Actions));
        const string nativeTextureName = "CNFG00I0.IMG";
        const string mLookAltTextureName = "CNFG00I1.IMG";
        bool multipleAssignments = false;

        #endregion

        #region Constructors

        public DaggerfallControlsWindow(IUserInterfaceManager uiManager, IUserInterfaceWindow previousWindow = null)
            :base(uiManager, previousWindow)
        {
        }

        #endregion

        #region Setup

        protected override void Setup()
        {
            // Load textures
            nativeTexture = DaggerfallUI.GetTextureFromImg(nativeTextureName);
            if (!nativeTexture)
                throw new Exception("DaggerfallControlsWindow: Could not load native texture.");
            mLookAltTexture = DaggerfallUI.GetTextureFromImg(mLookAltTextureName);

            // Always dim background
            ParentPanel.BackgroundColor = ScreenDimColor;

            // Controls panel
            controlsPanel.HorizontalAlignment = HorizontalAlignment.Center;
            controlsPanel.Size = new Vector2(nativeTexture.width, nativeTexture.height);
            controlsPanel.BackgroundTexture = nativeTexture;
            NativePanel.Components.Add(controlsPanel);

            // Mouse Look Alternative Controls Panel
            mLookAltPanel.Position = new Vector2(152, 100);
            mLookAltPanel.Size = new Vector2(mLookAltTexture.width, mLookAltTexture.height);
            mLookAltPanel.BackgroundTexture = mLookAltTexture;
            controlsPanel.Components.Add(mLookAltPanel);

            #region Tab Buttons

            // Joystick
            Button joystickButton = DaggerfallUI.AddButton(new Rect(0, 190, 80, 10), controlsPanel);
            joystickButton.OnMouseClick += JoystickButton_OnMouseClick;

            // Mouse
            Button mouseButton = DaggerfallUI.AddButton(new Rect(80, 190, 80, 10), controlsPanel);
            mouseButton.OnMouseClick += MouseButton_OnMouseClick;

            // Default
            Button defaultButton = DaggerfallUI.AddButton(new Rect(160, 190, 80, 10), controlsPanel);
            defaultButton.BackgroundColor = new Color(1, 0, 0, 0.5f);
            defaultButton.OnMouseClick += DefaultButton_OnMouseClick;

            // Continue
            Button continueButton = DaggerfallUI.AddButton(new Rect(240, 190, 80, 10), controlsPanel);
            continueButton.OnMouseClick += ContinueButton_OnMouseClick;

            #endregion

            #region Keybind Buttons

            positionKeybindButtons(moveKeysOne, 2, 8, 56, 12);
            positionKeybindButtons(moveKeysTwo, 8, 14, 163, 12);
            positionKeybindButtons(modeKeys, 14, 20, 269, 12);
            positionKeybindButtons(magicKeys, 20, 24, 101, 79);
            positionKeybindButtons(weaponKeys, 24, 27, 101, 124);
            positionKeybindButtons(statusKeys, 27, 30, 101, 158);
            positionKeybindButtons(activateKeys, 30, 32, 269, 79);
            positionKeybindButtons(lookKeys, 32, 36, 269, 102);
            positionKeybindButtons(uiKeys, 36, 40, 269, 147);

            #endregion
        }

        #endregion

        #region Private Methods

        private void positionKeybindButtons(List<Button> buttonGroup, int startPoint, int endPoint, int leftOffset, int topOffset)
        {
            for (int i = startPoint; i < endPoint; i++)
            {
                InputManager.Actions key = (InputManager.Actions)Enum.Parse(typeof(InputManager.Actions), actions[i]);
                int j = i - startPoint;

                buttonGroup.Add(new Button());
                buttonGroup[j].Label.Text = InputManager.Instance.GetBinding(key).ToString();
                buttonGroup[j].Label.ShadowPosition = new Vector2(0, 0);
                buttonGroup[j].Size = new Vector2(48, 8);

                if (j == 0)
                    buttonGroup[j].Position = new Vector2(leftOffset, topOffset);
                else
                    buttonGroup[j].Position = new Vector2(leftOffset, buttonGroup[j - 1].Position.y + 11);

                controlsPanel.Components.Add(buttonGroup[j]);
                buttonGroup[j].Name = actions[i];
                buttonGroup[j].OnMouseClick += KeybindButton_OnMouseClick;

                if (i == endPoint - 1)
                {
                    allKeys.Add(buttonGroup);
                }
            }
        }

        #endregion

        #region Tab Button Event Handlers

        private void JoystickButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            uiManager.PostMessage(DaggerfallUIMessages.dfuiOpenJoystickControlsWindow);
        }

        private void MouseButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            uiManager.PostMessage(DaggerfallUIMessages.dfuiOpenMouseControlsWindow);
        }

        private void DefaultButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            // Set keybinds to default
        }

        private void ContinueButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (multipleAssignments)
            {
                // Prevent window from closing
            }
            else
            {
                CancelWindow();
            }
        }

        #endregion

        #region Keybind Event Handlers

        private void KeybindButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            Button thisKeybindButton = (Button)sender;
            InputManager.Instance.StartCoroutine(WaitForKeyPress(thisKeybindButton));
        }

        IEnumerator WaitForKeyPress(Button button)
        {
            string currentLabel = button.Label.Text;
            InputManager.Actions buttonAction = (InputManager.Actions)Enum.Parse(typeof(InputManager.Actions), button.Name);

            button.Label.Text = "";
            yield return new WaitForSecondsRealtime(0.05f);
            while(!Input.anyKeyDown)
            {
                yield return null;
            }
            foreach (KeyCode code in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(code))
                {
                    if (code.ToString() != "Escape")
                    {
                        foreach (List<Button> buttonGroup in allKeys)
                        {
                            foreach (Button keybindButton in buttonGroup)
                            {
                                // Prevent duplicate keybindings
                            }
                        }

                        button.Label.Text = code.ToString();
                        InputManager.Instance.ClearBinding(buttonAction);
                        InputManager.Instance.SetBinding(code, buttonAction);
                        InputManager.Instance.SaveKeyBinds();
                    }
                    else
                    {
                        button.Label.Text = currentLabel;
                    }
                }
            }
        }
        
        #endregion
    }
}