// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2020 Daggerfall Workshop
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
using System.Linq;

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// InputManager singleton class for Daggerfall-specific game actions.
    /// </summary>
    public class InputManager : MonoBehaviour
    {
        #region Fields

        public const float minAcceleration = 1.0f;
        public const float maxAcceleration = 10.0f;

        public Texture2D controllerCursorImage;

        Dictionary<int, String> axisKeyCodeStrings = new Dictionary<int, String>()
        {
            { 5000, "JoystickAxis9Button0" },
            { 5001, "JoystickAxis10Button0" },
            { 5002, "JoystickAxis7Button0" },
            { 5003, "JoystickAxis7Button1" },
            { 5004, "JoystickAxis6Button0" },
            { 5005, "JoystickAxis6Button1" }
        };

        //These three dictionaries are built for InputManager.GetAxisRaw(...), specifically to
        //deal with raising KeyUp events, since Unity does not provide an "OnAxisValueChange" event
        Dictionary<int, float> previousAxisRaw = new Dictionary<int, float>();
        Dictionary<int, bool> upAxisRaw = new Dictionary<int, bool>();
        Dictionary<int, AxisActions> axisKeyCodeToActionsMap = new Dictionary<int, AxisActions>()
        {
            { 5000, AxisActions.LeftTrigger },
            { 5001, AxisActions.RightTrigger },
            { 5002, AxisActions.DPadVertical },
            { 5003, AxisActions.DPadVertical },
            { 5004, AxisActions.DPadHorizontal },
            { 5005, AxisActions.DPadHorizontal }
        };

        Dictionary<int, System.Func<bool>> axisKeyCodePresses = new Dictionary<int, System.Func<bool>>()
        {
            { 5000, () => InputManager.Instance.GetAxisRaw(5000, 0) != 0 },
            { 5001, () => InputManager.Instance.GetAxisRaw(5001, 0) != 0 },
            { 5002, () => InputManager.Instance.GetAxisRaw(5002, 1) > 0 },
            { 5003, () => InputManager.Instance.GetAxisRaw(5003, -1) < 0 },
            { 5004, () => InputManager.Instance.GetAxisRaw(5004, -1) < 0 },
            { 5005, () => InputManager.Instance.GetAxisRaw(5005, 1) > 0 }
        };

        const string keyBindsFilename = "KeyBinds.txt";

        const float deadZone = 0.05f;
        const float inputWaitTotal = 0.0833f;

        IList keyCodeList;
        KeyCode[] reservedKeys = new KeyCode[] { KeyCode.Escape, KeyCode.BackQuote };
        Dictionary<KeyCode, Actions> actionKeyDict = new Dictionary<KeyCode, Actions>();
        Dictionary<String, AxisActions> axisActionKeyDict = new Dictionary<String, AxisActions>();
        KeyCode[] controllerUIDict = new KeyCode[3]; //leftClick, rightClick, MiddleClick

        List<Actions> currentActions = new List<Actions>();
        List<Actions> previousActions = new List<Actions>();
        bool isPaused;
        bool wasPaused;
        float inputWaitTimer;
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
        float acceleration = 5.0f;

        bool usingControllerCursor;
        Vector2 controllerCursorPosition = new Vector2(0,0);
        int controllerCursorWidth = 32;
        int controllerCursorHeight = 32;
        float controllerCursorHorizontalSpeed = 300.0F;
        float controllerCursorVerticalSpeed = 300.0F;

        #endregion

        #region Structures

        [fsObject("v1")]
        public class KeyBindData_v1
        {
            public Dictionary<String, Actions> actionKeyBinds;
            public Dictionary<String, AxisActions> axisActionKeyBinds;
        }

        #endregion

        #region Properties

        public bool IsPaused
        {
            get { return isPaused; }
            set { isPaused = value; }
        }

        public IList KeyCodeList {
            get { return GetKeyCodeList(); }
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

        public bool CursorVisible { get; set; }

        public Vector3 MousePosition {
            get {
                if(usingControllerCursor)
                    return controllerCursorPosition;
                else
                    return Input.mousePosition;
            }
        }

        #endregion

        #region Enums

        public enum AxisActions {
            MovementHorizontal,
            MovementVertical,
            CameraHorizontal,
            CameraVertical,
            DPadVertical,
            DPadHorizontal,
            LeftTrigger,
            RightTrigger,
        }

        public enum Actions
        {
            Escape,
            ToggleConsole,

            MoveForwards,
            MoveBackwards,
            TurnLeft,
            MoveLeft,
            TurnRight,
            MoveRight,

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
        }

        void Start()
        {
            // Read acceleration/deceleration setting
            acceleration = DaggerfallUnity.Settings.MoveSpeedAcceleration;

            try
            {
                // Load a keybind file if possible
                if (HasKeyBindsSave())
                    LoadKeyBinds();

                // Ensure defaults are deployed if missing
                SetupDefaults();

                // Update keybinds save
                SaveKeyBinds();
            }
            catch (Exception ex)
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
                inputWaitTimer = 0f;
                wasPaused = true;

                // Allow quickload during death
                if (GameManager.Instance.PlayerObject && GameManager.Instance.PlayerDeath.DeathInProgress)
                {
                    KeyCode quickLoadBinding = GetBinding(Actions.QuickLoad);
                    if (GetKey(quickLoadBinding))
                    {
                        currentActions.Add(Actions.QuickLoad);
                    }
                }

                return;
            }

            // Skip some time post-pause
            // This ensures GUI actions do not "fall-through" to main world
            // as closing GUI and picking up next input all happen same-frame
            // This also helps prevent fall-through of GUI mouse movements to
            // same-frame mouse-look actions
            if (wasPaused && inputWaitTimer < inputWaitTotal)
            {
                inputWaitTimer += Time.deltaTime;
                return;
            }

            // Lower was paused flag
            wasPaused = false;

            // Collect mouse axes
            mouseX = Input.GetAxisRaw("Mouse X");
            if(mouseX == 0F)
                mouseX = Input.GetAxis(GetAxisBinding(AxisActions.CameraHorizontal));

            mouseY = Input.GetAxisRaw("Mouse Y");
            if(mouseY == 0F)
                mouseY = Input.GetAxis(GetAxisBinding(AxisActions.CameraVertical));

            // Update look impulse
            UpdateLook();

            // Process actions from input sources
            FindKeyboardActions();
            FindInputAxisActions();

            // Apply friction to movement force
            ApplyFriction();
        }

        void OnGUI()
        {
            var horizBinding = GetAxisBinding(AxisActions.MovementHorizontal);
            var vertBinding = GetAxisBinding(AxisActions.MovementVertical);

            if(String.IsNullOrEmpty(horizBinding) || String.IsNullOrEmpty(vertBinding))
                return;

            var horizj = Input.GetAxis(horizBinding);
            var vertj = Input.GetAxis(vertBinding);

            if (!usingControllerCursor && (horizj != 0 || vertj != 0))
            {
                usingControllerCursor = true;
                controllerCursorPosition = Input.mousePosition;
            }
            else if (usingControllerCursor && (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0))
            {
                usingControllerCursor = false;
            }

            if (CursorVisible)
            {
                if (usingControllerCursor)
                {
                    Cursor.visible = false;
                    //Cursor.lockState = CursorLockMode.Locked;

                    GUI.depth = 0;

                    controllerCursorPosition.x += controllerCursorHorizontalSpeed * horizj * Time.fixedDeltaTime;
                    controllerCursorPosition.y += controllerCursorVerticalSpeed * vertj * Time.fixedDeltaTime;

                    controllerCursorPosition.x = Mathf.Clamp(controllerCursorPosition.x, 0, Screen.width);
                    controllerCursorPosition.y = Mathf.Clamp(controllerCursorPosition.y, 0, Screen.height);

                    GUI.DrawTexture(new Rect(controllerCursorPosition.x, Screen.height - controllerCursorPosition.y, controllerCursorWidth, controllerCursorHeight), controllerCursorImage);
                }
                else
                {
                    Cursor.visible = true;
                }
            }
            else
            {
                //note: we don't even need to do anything to hide the visibility of the controller cursor since it will stop rendering
                Cursor.visible = false;
            }
        }

        void OnApplicationQuit()
        {
            SaveKeyBinds();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Clears all queued actions and action state.
        /// </summary>
        public void ClearAllActions()
        {
            previousActions.Clear();
            currentActions.Clear();
            horizontal = 0;
            vertical = 0;
            mouseX = 0;
            mouseY = 0;
            lookX = 0;
            lookY = 0;
        }

        /// <summary>
        /// Adds an action
        /// </summary>
        public void AddAction(Actions action)
        {
            currentActions.Add(action);
        }

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
        /// Finds first unity axis input string bound to a specific action.
        /// </summary>
        public String GetAxisBinding(AxisActions action)
        {
            if (axisActionKeyDict.ContainsValue(action))
            {
                foreach (var k in axisActionKeyDict.Keys)
                {
                    if (axisActionKeyDict[k] == action)
                        return k;
                }
            }

            return String.Empty;
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

        // Bind a KeyCode to an action
        public void SetBinding(KeyCode code, Actions action)
        {
            // Not allowing multi-bind at this time as the front-end doesn't support it
            ClearBinding(action);

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

        // Bind an input axis string to an axis action
        public void SetAxisBinding(String code, AxisActions action)
        {
            // Not allowing multi-bind at this time as the front-end doesn't support it
            ClearAxisBinding(action);

            if (!axisActionKeyDict.ContainsKey(code))
            {
                axisActionKeyDict.Add(code, action);
            }
            else
            {
                axisActionKeyDict.Remove(code);
                axisActionKeyDict.Add(code, action);
            }
        }

        // Unbind a KeyCode or action
        public void ClearBinding(KeyCode code)
        {
            if (actionKeyDict.ContainsKey(code))
            {
                actionKeyDict.Remove(code);
            }
        }

        // Unbind a KeyCode or action
        public void ClearAxisBinding(String code)
        {
            if (axisActionKeyDict.ContainsKey(code))
            {
                axisActionKeyDict.Remove(code);
            }
        }

        public void ClearAxisBinding(AxisActions action)
        {
            foreach (var binding in axisActionKeyDict.Where(kvp => kvp.Value == action).ToList())
            {
                axisActionKeyDict.Remove(binding.Key);
            }
        }

        public void ClearBinding(Actions action)
        {
            foreach (var binding in actionKeyDict.Where(kvp => kvp.Value == action).ToList())
            {
                actionKeyDict.Remove(binding.Key);
            }
        }

        // Save keybindings
        public void SaveKeyBinds()
        {
            string path = GetKeyBindsSavePath();

            KeyBindData_v1 keyBindsData = new KeyBindData_v1();
            keyBindsData.actionKeyBinds = actionKeyDict.Select(x => x).ToDictionary(entry => GetKeyString(entry.Key), entry => (Actions)entry.Value);
            keyBindsData.axisActionKeyBinds = axisActionKeyDict;
            string json = SaveLoadManager.Serialize(keyBindsData.GetType(), keyBindsData);
            File.WriteAllText(path, json);
            RaiseSavedKeyBindsEvent();
        }

        // Set keybindings to defaults
        public void ResetDefaults()
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

            SetAxisBinding("Axis1", AxisActions.MovementHorizontal);
            SetAxisBinding("Axis2", AxisActions.MovementVertical);
            SetAxisBinding("Axis4", AxisActions.CameraHorizontal);
            SetAxisBinding("Axis5", AxisActions.CameraVertical);

            SetAxisBinding("Axis9", AxisActions.LeftTrigger);
            SetAxisBinding("Axis10", AxisActions.RightTrigger);
            SetAxisBinding("Axis6", AxisActions.DPadHorizontal);
            SetAxisBinding("Axis7", AxisActions.DPadVertical);

            controllerUIDict[0] = KeyCode.Joystick1Button0;
            controllerUIDict[1] = KeyCode.Joystick1Button1;
        }

        public bool GetMouseButtonDown(int button)
        {
            if(usingControllerCursor)
                return GetKeyDown(controllerUIDict[button]);

            return Input.GetMouseButtonDown(button);
        }

        public bool GetMouseButton(int button)
        {
            if(usingControllerCursor)
                return GetKey(controllerUIDict[button]);

            return Input.GetMouseButton(button);
        }

        public bool GetKey(KeyCode key)
        {
            return (((int)key) < 5000 && Input.GetKey(key)) || GetAxisKey((int)key);
        }

        public bool GetKeyDown(KeyCode key)
        {
            return (((int)key) < 5000 && Input.GetKeyDown(key)) || GetAxisKey((int)key);
        }

        public bool GetKeyUp(KeyCode key)
        {
            return (((int)key) < 5000 && Input.GetKeyUp(key)) || GetAxisKeyUp((int)key);
        }

        public bool AnyKeyDown {
            get { return Input.anyKeyDown || AnyAxisKeyDown; }
        }

        public String GetKeyString(KeyCode key)
        {
            if (axisKeyCodeStrings.ContainsKey((int)key))
                return axisKeyCodeStrings[(int)key];
            else
                return key.ToString();
        }

        public KeyCode ParseKeyCodeString(String s){
            try
            {
                return (KeyCode)Enum.Parse(typeof(KeyCode), s);
            }
            catch
            {
                return (KeyCode)axisKeyCodeStrings.FirstOrDefault(x => x.Value == s).Key;
            }
        }

        #endregion

        #region Public Static Methods

        public static bool FindSingleton(out InputManager singletonOut)
        {
            singletonOut = GameObject.FindObjectOfType<InputManager>();
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

        // Sets KeyCode binding only if action is missing
        // This is to ensure default actions are restored if missing
        // and to push out new actions to existing keybind files
        private void TestSetBinding(KeyCode code, Actions action)
        {
            if (!actionKeyDict.ContainsValue(action))
            {
                SetBinding(code, action);
            }
        }

        // Sets KeyCode binding only if action is missing
        // This is to ensure default actions are restored if missing
        // and to push out new actions to existing keybind files
        private void TestSetAxisBinding(String code, AxisActions action)
        {
            if (!axisActionKeyDict.ContainsValue(action))
            {
                SetAxisBinding(code, action);
            }
        }

        // Deploys default values if action missing from loaded keybinds
        private void SetupDefaults()
        {
            TestSetBinding(KeyCode.Escape, Actions.Escape);
            TestSetBinding(KeyCode.BackQuote, Actions.ToggleConsole);

            TestSetBinding(KeyCode.W, Actions.MoveForwards);
            TestSetBinding(KeyCode.S, Actions.MoveBackwards);
            TestSetBinding(KeyCode.A, Actions.MoveLeft);
            TestSetBinding(KeyCode.D, Actions.MoveRight);
            TestSetBinding(KeyCode.LeftArrow, Actions.TurnLeft);
            TestSetBinding(KeyCode.RightArrow, Actions.TurnRight);

            TestSetBinding(KeyCode.PageUp, Actions.FloatUp);
            TestSetBinding(KeyCode.PageDown, Actions.FloatDown);
            TestSetBinding(KeyCode.Space, Actions.Jump);
            TestSetBinding(KeyCode.C, Actions.Crouch);
            TestSetBinding(KeyCode.LeftControl, Actions.Slide);
            TestSetBinding(KeyCode.RightControl, Actions.Slide);
            TestSetBinding(KeyCode.LeftShift, Actions.Run);
            TestSetBinding(KeyCode.RightShift, Actions.Run);

            TestSetBinding(KeyCode.R, Actions.Rest);
            TestSetBinding(KeyCode.T, Actions.Transport);
            TestSetBinding(KeyCode.F1, Actions.StealMode);
            TestSetBinding(KeyCode.F2, Actions.GrabMode);
            TestSetBinding(KeyCode.F3, Actions.InfoMode);
            TestSetBinding(KeyCode.F4, Actions.TalkMode);

            TestSetBinding(KeyCode.Backspace, Actions.CastSpell);
            TestSetBinding(KeyCode.Q, Actions.RecastSpell);
            TestSetBinding(KeyCode.E, Actions.AbortSpell);
            TestSetBinding(KeyCode.U, Actions.UseMagicItem);

            TestSetBinding(KeyCode.Z, Actions.ReadyWeapon);
            TestSetBinding(KeyCode.Mouse1, Actions.SwingWeapon);
            TestSetBinding(KeyCode.H, Actions.SwitchHand);

            TestSetBinding(KeyCode.I, Actions.Status);
            TestSetBinding(KeyCode.F5, Actions.CharacterSheet);
            TestSetBinding(KeyCode.F6, Actions.Inventory);

            TestSetBinding(KeyCode.Mouse0, Actions.ActivateCenterObject);
            TestSetBinding(KeyCode.Return, Actions.ActivateCursor);

            TestSetBinding(KeyCode.Insert, Actions.LookUp);
            TestSetBinding(KeyCode.Delete, Actions.LookDown);
            TestSetBinding(KeyCode.Home, Actions.CenterView);
            TestSetBinding(KeyCode.LeftAlt, Actions.Sneak);
            TestSetBinding(KeyCode.RightAlt, Actions.Sneak);

            TestSetBinding(KeyCode.L, Actions.LogBook);
            TestSetBinding(KeyCode.N, Actions.NoteBook);
            TestSetBinding(KeyCode.M, Actions.AutoMap);
            TestSetBinding(KeyCode.V, Actions.TravelMap);

            TestSetBinding(KeyCode.F9, Actions.QuickSave);
            TestSetBinding(KeyCode.F12, Actions.QuickLoad);

            TestSetAxisBinding("Axis1", AxisActions.MovementHorizontal);
            TestSetAxisBinding("Axis2", AxisActions.MovementVertical);
            TestSetAxisBinding("Axis4", AxisActions.CameraHorizontal);
            TestSetAxisBinding("Axis5", AxisActions.CameraVertical);

            TestSetAxisBinding("Axis9", AxisActions.LeftTrigger);
            TestSetAxisBinding("Axis10", AxisActions.RightTrigger);
            TestSetAxisBinding("Axis6", AxisActions.DPadHorizontal);
            TestSetAxisBinding("Axis7", AxisActions.DPadVertical);

            controllerUIDict[0] = KeyCode.Joystick1Button0;
            controllerUIDict[1] = KeyCode.Joystick1Button1;
        }

        // Apply force to horizontal axis
        void ApplyHorizontalForce(float scale)
        {
            // Use acceleration setting or "just go" at max value
            if (acceleration < maxAcceleration)
                horizontal = Mathf.Clamp(horizontal + (acceleration * scale) * Time.deltaTime, -1, 1);
            else
                horizontal = scale;

            if (scale < 0)
                negHorizontalImpulse = true;
            else if (scale > 0)
                posHorizontalImpulse = true;
        }

        // Apply force to vertical axis
        void ApplyVerticalForce(float scale)
        {
            // Use acceleration setting or "just go" at max value
            if (acceleration < maxAcceleration)
                vertical = Mathf.Clamp(vertical + (acceleration * scale) * Time.deltaTime, -1, 1);
            else
                vertical = scale;

            if (scale < 0)
                negVerticalImpulse = true;
            else if (scale > 0)
                posVerticalImpulse = true;
        }

        // Apply friction to decelerate inactive movement impulses towards 0
        void ApplyFriction()
        {
            // Use acceleration setting or "just stop" at max value
            if (acceleration < maxAcceleration)
            {
                if (!posVerticalImpulse && vertical > 0)
                    vertical = Mathf.Clamp(vertical - acceleration * Time.deltaTime, 0, vertical);
                if (!negVerticalImpulse && vertical < 0)
                    vertical = Mathf.Clamp(vertical + acceleration * Time.deltaTime, vertical, 0);

                if (!posHorizontalImpulse && horizontal > 0)
                    horizontal = Mathf.Clamp(horizontal - acceleration * Time.deltaTime, 0, horizontal);
                if (!negHorizontalImpulse && horizontal < 0)
                    horizontal = Mathf.Clamp(horizontal + acceleration * Time.deltaTime, horizontal, 0);
            }
            else
            {
                if (!posVerticalImpulse && vertical > 0)
                    vertical = 0;
                if (!negVerticalImpulse && vertical < 0)
                    vertical = 0;

                if (!posHorizontalImpulse && horizontal > 0)
                    horizontal = 0;
                if (!negHorizontalImpulse && horizontal < 0)
                    horizontal = 0;
            }
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

        IList GetKeyCodeList()
        {
            if (keyCodeList != null){
                return keyCodeList;
            }

            List<KeyCode> list = new List<KeyCode>();

            foreach (var e in Enum.GetValues(typeof(KeyCode)))
                list.Add((KeyCode)e);

            foreach (var k in axisKeyCodePresses.Keys)
                list.Add((KeyCode)k);

            keyCodeList = list;

            return keyCodeList;
        }

        bool GetAxisKey(int key)
        {
            return axisKeyCodePresses.ContainsKey(key) && axisKeyCodePresses[key]();
        }

        bool GetAxisKeyUp(int key)
        {
            if(key < 5000)
                return false;

            //This is a hacky solution. Without this statement, when the game is paused on a window,
            //the GetAxisRaw function will stop running (if there is nothing in that script updating
            //for a "GetKey" or "GetKeyDown").

            //Because it stops running, GetAxisKeyUp(...) will be unable to listen for that "up" event,
            //(via the upAxisRaw dictionary), and thus if the user presses the toggle button on the window,
            //it will do nothing.
            axisKeyCodePresses[key]();

            return upAxisRaw.ContainsKey(key) && upAxisRaw[key];
        }

        float GetAxisRaw(int keyCode, int signage)
        {
            if(!axisKeyCodeToActionsMap.ContainsKey(keyCode))
                return 0;

            AxisActions action = axisKeyCodeToActionsMap[keyCode];
            String unityInputAxisString = GetAxisBinding(action);
            float ret = Input.GetAxisRaw(unityInputAxisString);

            if(previousAxisRaw.ContainsKey(keyCode))
            {
                float prev = previousAxisRaw[keyCode];
				
				bool statement = (prev != ret);
				if(signage < 0)
					statement = prev < 0;
				else if(signage > 0)
					statement = prev > 0;
				
                upAxisRaw[keyCode] = (ret == 0 && statement);
            }

            previousAxisRaw[keyCode] = ret;

            return ret;
        }

        bool AnyAxisKeyDown
        {
            get
            {
                foreach (var f in axisKeyCodePresses.Values)
                    if (f()) return true;
                return false;
            }
        }

        // Enumerate all keyboard actions in progress
        void FindKeyboardActions()
        {
            var enumerator = actionKeyDict.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var element = enumerator.Current;
                if (GetKey(element.Key))
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

        void FindInputAxisActions()
        {
            float horiz = Input.GetAxis(GetAxisBinding(AxisActions.MovementHorizontal));
            float vert = Input.GetAxis(GetAxisBinding(AxisActions.MovementVertical));

            //if the force is greater than this threshold, round it up to 1
            const float threshold = 0.95F;

            //this arbitrary float value seems to be the minimum force that can be given without unnecessarily
            //triggering FrictionMotor's UnstickHandling() method, which can create jagged movement at lower force
            const float minimum = 0.68f;

            if (vert != 0 || horiz != 0)
            {
                float dist = Mathf.Clamp(Mathf.Sqrt(horiz*horiz + vert*vert), minimum, 1.0F);

                if(dist > threshold)
                    dist = 1.0F;

                if (horiz > 0)
                {
                    currentActions.Add(Actions.MoveRight);
                    horiz = dist;
                }
                else if (horiz < 0)
                {
                    currentActions.Add(Actions.MoveLeft);
                    horiz = -dist;
                }

                if (vert > 0)
                {
                    currentActions.Add(Actions.MoveForwards);
                    vert = dist;
                }
                else if (vert < 0)
                {
                    currentActions.Add(Actions.MoveBackwards);
                    vert = -dist;
                }

                ApplyHorizontalForce(horiz);
                ApplyVerticalForce(vert);
            }
        }

        #endregion

        #region Save/Load Bindings

        string GetKeyBindsSavePath()
        {
            return Path.Combine(Application.persistentDataPath, keyBindsFilename);
        }

        bool HasKeyBindsSave()
        {
            if (File.Exists(GetKeyBindsSavePath()))
                return true;

            return false;
        }

        void LoadKeyBinds()
        {
            string path = GetKeyBindsSavePath();

            string json = File.ReadAllText(path);
            KeyBindData_v1 keyBindsData = SaveLoadManager.Deserialize(typeof(KeyBindData_v1), json) as KeyBindData_v1;
            foreach(var item in keyBindsData.actionKeyBinds)
            {
                KeyCode key = ParseKeyCodeString(item.Key);
                if (!actionKeyDict.ContainsKey(key))
                    actionKeyDict.Add(key, item.Value);
            }

            foreach(var item in keyBindsData.axisActionKeyBinds)
            {
                if (!axisActionKeyDict.ContainsKey((String)item.Key))
                    axisActionKeyDict.Add((String)item.Key, item.Value);
            }
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
