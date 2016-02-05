// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Daggerfall Workshop
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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using FullSerializer;
using DaggerfallWorkshop.Game.Serialization;

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// InputManager singleton class for Daggerfall-specific game actions.
    /// </summary>
    public class InputManager : MonoBehaviour
    {
        #region Fields

        const string keyBindsFilename = "KeyBinds.txt";

        const float acceleration = 3f;
        const float friction = 4f;
        const float deadZone = 0.1f;
        const float frameSkipTotal = 5;

        KeyCode[] reservedKeys = new KeyCode[] { KeyCode.Escape, KeyCode.BackQuote };
        Dictionary<KeyCode, Actions> actionKeyDict = new Dictionary<KeyCode, Actions>();
        List<Actions> currentActions = new List<Actions>();
        List<Actions> previousActions = new List<Actions>();
        bool isPaused;
        bool wasPaused;
        int frameSkipCount;
        float horizontal;
        float vertical;
        float lookX;
        float lookY;
        float mouseX;
        float mouseY;
        bool invertLookX;
        bool invertLookY;
        bool posHorizontalImpulse;
        bool negHorizontalImpulse;
        bool posVerticalImpulse;
        bool negVerticalImpulse;

        #endregion

        #region Structures

        [fsObject("v1")]
        public class KeyBindData_v1
        {
            public Dictionary<KeyCode, Actions> actionKeyBinds;
        }

        #endregion

        #region Properties

        public bool IsPaused
        {
            get { return isPaused; }
            set { isPaused = value; }
        }

        public KeyCode[] ReservedKeys
        {
            get { return (KeyCode[])reservedKeys.Clone(); }
        }

        public Actions[] CurrentActions
        {
            get { return currentActions.ToArray(); }
        }

        public float MouseX
        {
            get { return mouseX; }
        }

        public float MouseY
        {
            get { return mouseY; }
        }

        public float LookX
        {
            get { return lookX; }
        }

        public float LookY
        {
            get { return lookY; }
        }

        public bool InvertLookX
        {
            get { return invertLookX; }
            set { invertLookX = value; }
        }

        public bool InvertLookY
        {
            get { return invertLookY; }
            set { invertLookY = value; }
        }

        public float Horizontal
        {
            get { return (horizontal < -deadZone || horizontal > deadZone) ? horizontal : 0; }
        }

        public float Vertical
        {
            get { return (vertical < -deadZone || vertical > deadZone) ? vertical : 0; }
        }

        #endregion

        #region Enums

        public enum Actions
        {
            Escape,
            ToggleConsole,

            MoveForwards,
            MoveBackwards,
            MoveLeft,
            MoveRight,
            TurnLeft,
            TurnRight,

            FloatUp,
            FloatDown,
            Jump,
            Crouch,
            Slide,
            Run,

            Rest,
            Transport,
            StealMode,
            GrabMode,
            InfoMode,
            TalkMode,

            CastSpell,
            RecastSpell,
            AbortSpell,
            UseMagicItem,

            ReadyWeapon,
            SwingWeapon,
            SwitchHand,

            Status,
            CharacterSheet,
            Inventory,

            ActivateCenterObject,
            ActivateCursor,

            LookUp,
            LookDown,
            CenterView,
            Sneak,

            LogBook,
            NoteBook,
            AutoMap,
            TravelMap,

            QuickSave,
            QuickLoad,
        }

        #endregion

        #region Singleton

        static InputManager instance = null;
        public static InputManager Instance
        {
            get
            {
                if (instance == null)
                {
                    if (!FindSingleton(out instance))
                    {
                        GameObject go = new GameObject();
                        go.name = "InputManager";
                        instance = go.AddComponent<InputManager>();
                    }
                }
                return instance;
            }
        }

        public static bool HasInstance
        {
            get
            {
                return (instance != null);
            }
        }

        #endregion

        #region Unity

        void Awake()
        {
            SetupSingleton();
            SetupDefaults();
        }

        void Start()
        {
            // Load a keybind file if possible
            try
            {
                if (HasKeyBindsSave())
                    LoadKeyBinds();
            }
            catch(Exception ex)
            {
                DaggerfallUnity.LogMessage(string.Format("Could not load keybinds file. The exception was: '{0}'", ex.Message), true);
                DaggerfallUnity.LogMessage("Setting default key binds after failed load.", true);
            }
        }

        void Update()
        {
            // Move current actions to previous actions
            previousActions.Clear();
            previousActions.AddRange(currentActions);

            // Clear current actions
            currentActions.Clear();

            // Clear look and mouse axes
            mouseX = 0;
            mouseY = 0;
            lookX = 0;
            lookY = 0;

            // Clear axis impulse flags, these will be raised again on movement
            posHorizontalImpulse = false;
            negHorizontalImpulse = false;
            posVerticalImpulse = false;
            negVerticalImpulse = false;

            // Do nothing if paused
            if (isPaused)
            {
                frameSkipCount = 0;
                wasPaused = true;
                return;
            }

            // Skip some frame post-pause
            // This ensures GUI actions do not "fall-through" to main world
            // as closing GUI and picking up next input all happen same-frame
            // This also helps prevent fall-through of GUI mouse movements to
            // same-frame mouse-look actions
            if (wasPaused && frameSkipCount++ < frameSkipTotal)
            {
                return;
            }

            // Lower was paused flag
            wasPaused = false;

            // Collect mouse axes
            mouseX = Input.GetAxisRaw("Mouse X");
            mouseY = Input.GetAxisRaw("Mouse Y");

            // Update look impulse
            UpdateLook();

            // Process actions from input sources
            FindKeyboardActions();

            // Apply friction to movement force
            ApplyFriction();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns true when specified action is in progress for current frame.
        /// </summary>
        public bool HasAction(Actions action)
        {
            return currentActions.Contains(action);
        }

        /// <summary>
        /// Returns true when specified action is in progress for current frame but not for previous frame.
        /// </summary>
        public bool ActionStarted(Actions action)
        {
            return (!previousActions.Contains(action) && currentActions.Contains(action));
        }

        /// <summary>
        /// Returns true when specified action was in progress previous frame but not for current frame.
        /// </summary>
        public bool ActionComplete(Actions action)
        {
            return (previousActions.Contains(action) && !currentActions.Contains(action));
        }

        /// <summary>
        /// Finds first keycode bound to a specific action.
        /// </summary>
        public KeyCode GetBinding(Actions action)
        {
            if (actionKeyDict.ContainsValue(action))
            {
                foreach (var k in actionKeyDict.Keys)
                {
                    if (actionKeyDict[k] == action)
                        return k;
                }
            }

            return KeyCode.None;
        }

        /// <summary>
        /// Finds all keycodes made to a specific action.
        /// Will return empty array if no bindings found.
        /// </summary>
        public KeyCode[] GetBindings(Actions action)
        {
            List<KeyCode> keyCodes = new List<KeyCode>();
            if (actionKeyDict.ContainsValue(action))
            {
                foreach (var k in actionKeyDict.Keys)
                {
                    if (actionKeyDict[k] == action)
                        keyCodes.Add(k);
                }
            }

            return keyCodes.ToArray();
        }

        #endregion

        #region Public Static Methods

        public static bool FindSingleton(out InputManager singletonOut)
        {
            singletonOut = GameObject.FindObjectOfType(typeof(InputManager)) as InputManager;
            if (singletonOut == null)
            {
                DaggerfallUnity.LogMessage("Could not locate InputManager GameObject instance in scene!", true);
                return false;
            }

            return true;
        }

        #endregion

        #region Private Methods

        private void SetupSingleton()
        {
            if (instance == null)
                instance = this;
            else if (instance != this)
            {
                if (Application.isPlaying)
                {
                    DaggerfallUnity.LogMessage("Multiple InputManager instances detected in scene!", true);
                    Destroy(gameObject);
                }
            }
        }

        void SetupDefaults()
        {
            actionKeyDict.Clear();

            SetBinding(KeyCode.Escape, Actions.Escape);
            SetBinding(KeyCode.BackQuote, Actions.ToggleConsole);

            SetBinding(KeyCode.W, Actions.MoveForwards);
            SetBinding(KeyCode.S, Actions.MoveBackwards);
            SetBinding(KeyCode.A, Actions.MoveLeft);
            SetBinding(KeyCode.D, Actions.MoveRight);
            SetBinding(KeyCode.LeftArrow, Actions.TurnLeft);
            SetBinding(KeyCode.RightArrow, Actions.TurnRight);

            SetBinding(KeyCode.PageUp, Actions.FloatUp);
            SetBinding(KeyCode.PageDown, Actions.FloatDown);
            SetBinding(KeyCode.Space, Actions.Jump);
            SetBinding(KeyCode.C, Actions.Crouch);
            SetBinding(KeyCode.LeftControl, Actions.Slide);
            SetBinding(KeyCode.RightControl, Actions.Slide);
            SetBinding(KeyCode.LeftShift, Actions.Run);
            SetBinding(KeyCode.RightShift, Actions.Run);

            SetBinding(KeyCode.R, Actions.Rest);
            SetBinding(KeyCode.T, Actions.Transport);
            SetBinding(KeyCode.F1, Actions.StealMode);
            SetBinding(KeyCode.F2, Actions.GrabMode);
            SetBinding(KeyCode.F3, Actions.InfoMode);
            SetBinding(KeyCode.F4, Actions.TalkMode);

            SetBinding(KeyCode.Backspace, Actions.CastSpell);
            SetBinding(KeyCode.Q, Actions.RecastSpell);
            SetBinding(KeyCode.E, Actions.AbortSpell);
            SetBinding(KeyCode.U, Actions.UseMagicItem);

            SetBinding(KeyCode.Z, Actions.ReadyWeapon);
            SetBinding(KeyCode.Mouse1, Actions.SwingWeapon);
            SetBinding(KeyCode.H, Actions.SwitchHand);

            SetBinding(KeyCode.I, Actions.Status);
            SetBinding(KeyCode.F5, Actions.CharacterSheet);
            SetBinding(KeyCode.F6, Actions.Inventory);

            SetBinding(KeyCode.Mouse0, Actions.ActivateCenterObject);
            SetBinding(KeyCode.Return, Actions.ActivateCursor);

            SetBinding(KeyCode.Insert, Actions.LookUp);
            SetBinding(KeyCode.Delete, Actions.LookDown);
            SetBinding(KeyCode.Home, Actions.CenterView);
            SetBinding(KeyCode.LeftAlt, Actions.Sneak);
            SetBinding(KeyCode.RightAlt, Actions.Sneak);

            SetBinding(KeyCode.L, Actions.LogBook);
            SetBinding(KeyCode.N, Actions.NoteBook);
            SetBinding(KeyCode.M, Actions.AutoMap);
            SetBinding(KeyCode.V, Actions.TravelMap);

            SetBinding(KeyCode.F9, Actions.QuickSave);
            SetBinding(KeyCode.F12, Actions.QuickLoad);
        }

        // Bind a KeyCode to an action
        void SetBinding(KeyCode code, Actions action)
        {
            if (!actionKeyDict.ContainsKey(code))
            {
                actionKeyDict.Add(code, action);
            }
            else
            {
                actionKeyDict.Remove(code);
                actionKeyDict.Add(code, action);
            }
        }

        // Unbind a KeyCode from an action
        void ClearBinding(KeyCode code)
        {
            if (actionKeyDict.ContainsKey(code))
            {
                actionKeyDict.Remove(code);
            }
        }

        // Apply force to horizontal axis
        void ApplyHorizontalForce(float scale)
        {
            horizontal = Mathf.Clamp(horizontal + (acceleration * scale) * Time.deltaTime, -1, 1);
            if (scale < 0)
                negHorizontalImpulse = true;
            else if (scale > 0)
                posHorizontalImpulse = true;
        }

        // Apply force to vertical axis
        void ApplyVerticalForce(float scale)
        {
            vertical = Mathf.Clamp(vertical + (acceleration * scale) * Time.deltaTime, -1, 1);
            if (scale < 0)
                negVerticalImpulse = true;
            else if (scale > 0)
                posVerticalImpulse = true;
        }

        // Apply friction to decelerate inactive movement impulses towards 0
        void ApplyFriction()
        {
            if (!posVerticalImpulse && vertical > 0)
                vertical = Mathf.Clamp(vertical - friction * Time.deltaTime, 0, vertical);
            if (!negVerticalImpulse && vertical < 0)
                vertical = Mathf.Clamp(vertical + friction * Time.deltaTime, vertical, 0);

            if (!posHorizontalImpulse && horizontal > 0)
                horizontal = Mathf.Clamp(horizontal - friction * Time.deltaTime, 0, horizontal);
            if (!negHorizontalImpulse && horizontal < 0)
                horizontal = Mathf.Clamp(horizontal + friction * Time.deltaTime, horizontal, 0);
        }

        // Updates look axes based on supported input
        void UpdateLook()
        {
            // Assign mouse
            lookX = mouseX;
            lookY = mouseY;

            // Inversion
            lookX = (invertLookX) ? -lookX : lookX;
            lookY = (invertLookY) ? -lookY : lookY;
        }

        // Enumerate all keyboard actions in progress
        void FindKeyboardActions()
        {
            var enumerator = actionKeyDict.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var element = enumerator.Current;
                if (Input.GetKey(element.Key))
                {
                    // Add current action to list
                    currentActions.Add(element.Value);

                    // Handle movement impulses
                    switch (element.Value)
                    {
                        case Actions.MoveRight:
                            ApplyHorizontalForce(1);
                            break;
                        case Actions.MoveLeft:
                            ApplyHorizontalForce(-1);
                            break;
                        case Actions.MoveForwards:
                            ApplyVerticalForce(1);
                            break;
                        case Actions.MoveBackwards:
                            ApplyVerticalForce(-1);
                            break;
                    }
                }
            }
        }

        #endregion

        #region Save/Load Bindings

        string GetKeyBindsSavePath()
        {
            return Path.Combine(Application.dataPath, keyBindsFilename);
        }

        bool HasKeyBindsSave()
        {
            if (File.Exists(GetKeyBindsSavePath()))
                return true;

            return false;
        }

        void SaveKeyBinds()
        {
            string path = GetKeyBindsSavePath();

            KeyBindData_v1 keyBindsData = new KeyBindData_v1();
            keyBindsData.actionKeyBinds = actionKeyDict;
            string json = SaveLoadManager.Serialize(keyBindsData.GetType(), keyBindsData);
            File.WriteAllText(path, json);
            RaiseSavedKeyBindsEvent();
        }

        void LoadKeyBinds()
        {
            string path = GetKeyBindsSavePath();

            string json = File.ReadAllText(path);
            KeyBindData_v1 keyBindsData = SaveLoadManager.Deserialize(typeof(KeyBindData_v1), json) as KeyBindData_v1;
            actionKeyDict = keyBindsData.actionKeyBinds;
            RaiseLoadedKeyBindsEvent();
        }

        #endregion

        #region Events

        public delegate void OnLoadSaveKeyBinds();
        public static event OnLoadSaveKeyBinds OnLoadedKeyBinds;
        protected virtual void RaiseLoadedKeyBindsEvent()
        {
            if (OnLoadedKeyBinds != null)
                OnLoadedKeyBinds();
        }

        public static event OnLoadSaveKeyBinds OnSavedKeyBinds;
        protected virtual void RaiseSavedKeyBindsEvent()
        {
            if (OnSavedKeyBinds != null)
                OnSavedKeyBinds();
        }

        public delegate void OnUpdateKeyBind(KeyCode code);
        public static event OnUpdateKeyBind OnUpdatedKeyBind;
        protected virtual void RaiseUpdatedKeyBindsEvent(KeyCode code)
        {
            if (OnUpdatedKeyBind != null)
                OnUpdatedKeyBind(code);
        }

        #endregion
    }
}
