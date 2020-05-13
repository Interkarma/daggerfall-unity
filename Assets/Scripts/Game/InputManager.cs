// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2020 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    jefetienne
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

        //there are only 16 recognized axes
        const int numAxes = 16;
        const int startingAxisKeyCode = 5000;

        //if the force is greater than this threshold, round it up to 1
        float joystickMovementThreshold = 0.95F;

        //this arbitrary float value seems to be the minimum force that can be given without unnecessarily
        //triggering FrictionMotor's UnstickHandling() method, which can create jagged movement at lower force
        const float controllerMinimumAxisFloat = 0.68f;

        public Texture2D controllerCursorImage;

        Dictionary<int, String> axisKeyCodeStrings = new Dictionary<int, String>();
        Dictionary<int, String> axisKeyCodeToInputAxis = new Dictionary<int, String>();
        Dictionary<int, System.Func<bool>> axisKeyCodePresses = new Dictionary<int, System.Func<bool>>();

        //These three dictionaries are built for InputManager.GetAxisRaw(...), specifically to
        //deal with raising KeyUp events, since Unity does not provide an "OnAxisValueChange" event
        Dictionary<int, float> previousAxisRaw = new Dictionary<int, float>();
        Dictionary<int, bool> upAxisRaw = new Dictionary<int, bool>();
        Dictionary<int, bool> downAxisRaw = new Dictionary<int, bool>();

        const string keyBindsFilename = "KeyBinds.txt";

        const float deadZone = 0.05f;
        const float inputWaitTotal = 0.0833f;

        IList keyCodeList;
        KeyCode[] reservedKeys = new KeyCode[] { };
        Dictionary<KeyCode, Actions> actionKeyDict = new Dictionary<KeyCode, Actions>();
        Dictionary<String, AxisActions> axisActionKeyDict = new Dictionary<String, AxisActions>();
        Dictionary<KeyCode, string> unknownActions = new Dictionary<KeyCode, string>();
        KeyCode[] controllerUIDict = new KeyCode[3]; //leftClick, rightClick, MiddleClick
        String[] cameraAxisBindingCache = new String[2];
        String[] movementAxisBindingCache = new String[2];

        List<Actions> currentActions = new List<Actions>();
        List<Actions> previousActions = new List<Actions>();
        bool isPaused;
        bool wasPaused;
        float inputWaitTimer;
        float horizontal;
        float vertical;
        float lookX;
        float lookY;
        float keyboardLookX;
        float keyboardLookY;
        float mouseX;
        float mouseY;
        bool invertLookX;
        bool invertLookY;
        bool posHorizontalImpulse;
        bool negHorizontalImpulse;
        bool posVerticalImpulse;
        bool negVerticalImpulse;
        float acceleration = 5.0f;


        float joystickCameraSensitivity = 1.0f;
        float joystickUIMouseSensitivity = 1.0f;
        bool cursorVisible = true;
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
            public Dictionary<String, string> actionKeyBinds;
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

        public bool UsingController
        {
            get { return usingControllerCursor; }
        }

        public KeyCode LastKeyDown { get; private set; }

        public bool CursorVisible
        {
            get { return cursorVisible; }
            set { cursorVisible = value; }
        }

        public Vector3 MousePosition {
            get {
                if (usingControllerCursor)
                    return controllerCursorPosition;
                else
                    return Input.mousePosition;
            }
        }

        //TODO: have this value be adjustable and serializable for the future joystick window
        public float JoystickCameraSensitivity
        {
            get
            {
                return joystickCameraSensitivity;
            }
            set
            {
                if (value < 0.0f)
                    value = 0.0f;
                joystickCameraSensitivity = value;
            }
        }

        //TODO: have this value be adjustable and serializable for the future joystick window
        public float JoystickUIMouseSensitivity
        {
            get
            {
                return joystickUIMouseSensitivity;
            }
            set
            {
                if (value < 0.0f)
                    value = 0.0f;
                joystickUIMouseSensitivity = value;
            }
        }

        //TODO: have this value be adjustable and serializable for the future joystick window
        public float JoystickMovementThreshold
        {
            get { return joystickMovementThreshold; }
            set { joystickMovementThreshold = value; }
        }

        #endregion

        #region Enums

        public enum AxisActions {
            MovementHorizontal,
            MovementVertical,
            CameraHorizontal,
            CameraVertical,
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

            PrintScreen,
            
            Unknown,
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

            //memoization for 'axis keycodes'
            for (int i = startingAxisKeyCode; i < startingAxisKeyCode + numAxes * 2; i++)
            {
                axisKeyCodeStrings[i] = AxisKeyCodeToString(i);
                axisKeyCodePresses[i] = AxisKeyCodePress(i);
                axisKeyCodeToInputAxis[i] = AxisKeyCodeToInputAxis(i);
            }

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
            keyboardLookX = 0;
            keyboardLookY = 0;

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
            if (mouseX == 0F && !String.IsNullOrEmpty(cameraAxisBindingCache[0]))
                mouseX = Input.GetAxis(cameraAxisBindingCache[0]) * JoystickCameraSensitivity;

            mouseY = Input.GetAxisRaw("Mouse Y");
            if (mouseY == 0F && !String.IsNullOrEmpty(cameraAxisBindingCache[1]))
                mouseY = Input.GetAxis(cameraAxisBindingCache[1]) * JoystickCameraSensitivity;

            // Process actions from input sources
            FindKeyboardActions();
            FindInputAxisActions();

            // Update look impulse
            UpdateLook();

            // Apply friction to movement force
            ApplyFriction();
        }

        void OnGUI()
        {
            var horizBinding = movementAxisBindingCache[0];
            var vertBinding = movementAxisBindingCache[1];

            if (String.IsNullOrEmpty(horizBinding) || String.IsNullOrEmpty(vertBinding))
                return;

            var horizj = Input.GetAxis(horizBinding);
            var vertj = Input.GetAxis(vertBinding);

            if (!usingControllerCursor &&
                (horizj != 0
                || vertj != 0
                || Input.GetAxis(cameraAxisBindingCache[0]) != 0
                || Input.GetAxis(cameraAxisBindingCache[1]) != 0))
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

                    controllerCursorPosition.x += JoystickUIMouseSensitivity * controllerCursorHorizontalSpeed * horizj * Time.fixedDeltaTime;
                    controllerCursorPosition.y += JoystickUIMouseSensitivity * controllerCursorVerticalSpeed * vertj * Time.fixedDeltaTime;

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

        /// <summary>
        /// Binds a KeyCode to an action
        /// </summary>
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

        /// <summary>
        /// Binds an Input Axis to an AxisAction
        /// </summary>
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

        /// <summary>
        /// Unbinds a KeyCode to an action via KeyCode
        /// </summary>
        public void ClearBinding(KeyCode code)
        {
            if (actionKeyDict.ContainsKey(code))
            {
                actionKeyDict.Remove(code);
            }
        }

        /// <summary>
        /// Unbinds an Input Axis to an AxisAction via Axis string name
        /// </summary>
        public void ClearAxisBinding(String code)
        {
            if (axisActionKeyDict.ContainsKey(code))
            {
                axisActionKeyDict.Remove(code);
            }
        }

        /// <summary>
        /// Unbinds an Input Axis to an AxisAction via the AxisAction
        /// </summary>
        public void ClearAxisBinding(AxisActions action)
        {
            foreach (var binding in axisActionKeyDict.Where(kvp => kvp.Value == action).ToList())
            {
                axisActionKeyDict.Remove(binding.Key);
            }
        }


        /// <summary>
        /// Unbinds a KeyCode to an action via Action
        /// </summary>
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
            keyBindsData.axisActionKeyBinds = axisActionKeyDict;
            keyBindsData.actionKeyBinds = new Dictionary<string, string>();

            foreach (var item in actionKeyDict)
            {
                keyBindsData.actionKeyBinds.Add(GetKeyString(item.Key), item.Value.ToString());
            }

            // If unknown actions were detected in this run, make sure we append them back to the settings file, so we won't break
            // the newer builds potentially using them.
            foreach (var item in unknownActions)
            {
                keyBindsData.actionKeyBinds.Add(GetKeyString(item.Key), item.Value);
            }

            string json = SaveLoadManager.Serialize(keyBindsData.GetType(), keyBindsData);
            File.WriteAllText(path, json);
            UpdateAxisBindingCache();
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

            SetBinding(KeyCode.F8, Actions.PrintScreen);
            SetBinding(KeyCode.F9, Actions.QuickSave);
            SetBinding(KeyCode.F12, Actions.QuickLoad);

            SetAxisBinding("Axis1", AxisActions.MovementHorizontal);
            SetAxisBinding("Axis2", AxisActions.MovementVertical);
            SetAxisBinding("Axis4", AxisActions.CameraHorizontal);
            SetAxisBinding("Axis5", AxisActions.CameraVertical);

            controllerUIDict[0] = KeyCode.JoystickButton0;
            controllerUIDict[1] = KeyCode.JoystickButton1;
            UpdateAxisBindingCache();
        }

        public bool GetMouseButtonDown(int button)
        {
            if (usingControllerCursor)
                return GetKeyDown(controllerUIDict[button]);

            return Input.GetMouseButtonDown(button);
        }

        public bool GetMouseButtonUp(int button)
        {
            if (usingControllerCursor)
                return GetKeyUp(controllerUIDict[button]);

            return Input.GetMouseButtonUp(button);
        }

        public bool GetMouseButton(int button)
        {
            if (usingControllerCursor)
                return GetKey(controllerUIDict[button]);

            return Input.GetMouseButton(button);
        }

        public bool GetKey(KeyCode key)
        {
            KeyCode conv = ConvertJoystickButtonKeyCode(key);
            var k = (((int)conv) < startingAxisKeyCode && Input.GetKey(conv)) || GetAxisKey((int)conv);
            if (k)
                LastKeyDown = conv;
            return k;
        }

        public bool GetKeyDown(KeyCode key)
        {
            KeyCode conv = ConvertJoystickButtonKeyCode(key);
            var kd = (((int)conv) < startingAxisKeyCode && Input.GetKeyDown(conv)) || GetAxisKeyDown((int)conv);
            if (kd)
                LastKeyDown = conv;
            return kd;
        }

        public bool GetKeyUp(KeyCode key)
        {
            KeyCode conv = ConvertJoystickButtonKeyCode(key);
            return (((int)conv) < startingAxisKeyCode && Input.GetKeyUp(conv)) || GetAxisKeyUp((int)conv);
        }

        public bool AnyKeyDown
        {
            get
            {
                foreach (KeyCode k in KeyCodeList)
                    if (GetKeyDown(k)) return true;
                return false;
            }
        }

        public String GetKeyString(KeyCode key)
        {
            if (axisKeyCodeStrings.ContainsKey((int)key))
                return axisKeyCodeStrings[(int)key];
            else
                return key.ToString();
        }

        public KeyCode ParseKeyCodeString(String s)
        {
            if (Enum.IsDefined(typeof(KeyCode), s))
            {
                return (KeyCode)Enum.Parse(typeof(KeyCode), s);
            }
            else
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

        private void UpdateAxisBindingCache()
        {
            cameraAxisBindingCache[0] = GetAxisBinding(AxisActions.CameraHorizontal);
            cameraAxisBindingCache[1] = GetAxisBinding(AxisActions.CameraVertical);
            movementAxisBindingCache[0] = GetAxisBinding(AxisActions.MovementHorizontal);
            movementAxisBindingCache[1] = GetAxisBinding(AxisActions.MovementVertical);
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

            TestSetBinding(KeyCode.F8, Actions.PrintScreen);
            TestSetBinding(KeyCode.F9, Actions.QuickSave);
            TestSetBinding(KeyCode.F12, Actions.QuickLoad);

            TestSetAxisBinding("Axis1", AxisActions.MovementHorizontal);
            TestSetAxisBinding("Axis2", AxisActions.MovementVertical);
            TestSetAxisBinding("Axis4", AxisActions.CameraHorizontal);
            TestSetAxisBinding("Axis5", AxisActions.CameraVertical);

            controllerUIDict[0] = KeyCode.JoystickButton0;
            controllerUIDict[1] = KeyCode.JoystickButton1;
            UpdateAxisBindingCache();
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
            lookX = (keyboardLookX == 0) ? mouseX : keyboardLookX;
            lookY = (keyboardLookY == 0) ? mouseY : keyboardLookY;

            // Inversion
            lookX = (invertLookX) ? -lookX : lookX;
            lookY = (invertLookY) ? -lookY : lookY;
        }

        // returns a list of all the Unity Input.KeyCodes and the custom axis KeyCodes
        IList GetKeyCodeList()
        {
            if (keyCodeList != null)
                return keyCodeList;

            List<KeyCode> list = new List<KeyCode>();

            foreach (var e in Enum.GetValues(typeof(KeyCode)))
                list.Add((KeyCode)e);

            foreach (var k in axisKeyCodePresses.Keys)
                list.Add((KeyCode)k);

            keyCodeList = list;

            return keyCodeList;
        }

        //Converts all joystick KeyCodes to be controller-agnostic (e.g. "Joystick3Button0" to "JoystickButton0")
        //Sometimes, Unity will recognize input from the controller as Joystick1ButtonX, and other times as JoystickButtonX
        //This method deals with this inconsistency by converting them to JoystickButtonX
        KeyCode ConvertJoystickButtonKeyCode(KeyCode k)
        {
            if (k < KeyCode.Joystick1Button0 || k > KeyCode.Joystick8Button19)
                return k;

            //Retrieves the button number from the enum. There are twenty buttons per joystick number
            //Returns a range from 0 to 19
            int num = (((int)k) + 10) % 20;

            //Add that number starting at JoystickButton0
            return KeyCode.JoystickButton0 + num;
        }

        //Returns a string that will visibly appear in the Keybinds window
        String AxisKeyCodeToString(int key)
        {
            if (key < startingAxisKeyCode)
                return String.Empty;

            return String.Concat("Joystick", AxisKeyCodeToInputAxis(key), "Button", key % 2);
        }

        //Returns the name of the axis that Unity's Input Manager uses
        //there are always two keys to a single axis for positive and negative floats
        String AxisKeyCodeToInputAxis(int key)
        {
            if (key < startingAxisKeyCode)
                return String.Empty;

            int axisNum = (key % startingAxisKeyCode) / 2 + 1;

            if (axisNum > numAxes)
                return String.Empty;

            return String.Concat("Axis", axisNum);
        }

        //Returns a function to determine if an axis button has been pressed
        System.Func<bool> AxisKeyCodePress(int key){
            if (key < startingAxisKeyCode)
                return () => false;

            //even-numbered keys are positive axis, odd are negative
            if (key % 2 == 0)
                return () => InputManager.Instance.GetAxisRaw(key, 1) > 0;
            else
                return () => InputManager.Instance.GetAxisRaw(key, -1) < 0;
        }

        bool GetAxisKey(int key)
        {
            return axisKeyCodePresses.ContainsKey(key) && axisKeyCodePresses[key]();
        }

        bool GetAxisKeyDown(int key)
        {
            if (key < startingAxisKeyCode)
                return false;

            //This is a hacky solution. Without this statement, when the game is paused on a window,
            //the GetAxisRaw function will stop running (if there is nothing in that script updating
            //for a "GetKey" or "GetKeyDown").

            //Because it stops running, GetAxisKeyDown(...) will be unable to listen for that "up" event,
            //(via the downAxisRaw dictionary), and thus if the user presses the toggle button on the window,
            //it will do nothing.
            axisKeyCodePresses[key]();
            return downAxisRaw.ContainsKey(key) && downAxisRaw[key];
        }

        bool GetAxisKeyUp(int key)
        {
            if (key < startingAxisKeyCode)
                return false;

            //Same hacky solution as GetAxisKeyDown
            axisKeyCodePresses[key]();

            return upAxisRaw.ContainsKey(key) && upAxisRaw[key];
        }

        // Returns the raw axis value based on the custom axis KeyCode and input direction via signage
        // Also updates upAxisRaw and downAxisRaw dictionaries to process GetAxisKeyDown and GetAxisKeyUp events
        float GetAxisRaw(int keyCode, int signage)
        {
            if (keyCode < startingAxisKeyCode || !axisKeyCodeToInputAxis.ContainsKey(keyCode))
                return 0;

            String unityInputAxisString = axisKeyCodeToInputAxis[keyCode];

            float ret = Input.GetAxisRaw(unityInputAxisString);

            if (previousAxisRaw.ContainsKey(keyCode))
            {
                float prev = previousAxisRaw[keyCode];

                // keyup -> if the previous frame captured input, but the current frame has not
                bool statement = (prev != ret);
                if (signage < 0)
                    statement = prev < 0;
                else if (signage > 0)
                    statement = prev > 0;

                upAxisRaw[keyCode] = (ret == 0 && statement);

                // keydown -> if the previous frame did not captured input, but the current frame has
                statement = (prev != ret);
                if (signage < 0)
                    statement = ret < 0;
                else if (signage > 0)
                    statement = ret > 0;

                downAxisRaw[keyCode] = (prev == 0 && statement);

                if (downAxisRaw[keyCode])
                    LastKeyDown = (KeyCode)keyCode;
            }

            previousAxisRaw[keyCode] = ret;

            return ret;
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

                    // Handle movement and keyboard look impulses
                    // Keyboard look overrides current mouseX and mouseY values
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
                        case Actions.TurnLeft:
                            keyboardLookX = -1;
                            break;
                        case Actions.TurnRight:
                            keyboardLookX = 1;
                            break;
                        case Actions.LookUp:
                            keyboardLookY = 1;
                            break;
                        case Actions.LookDown:
                            keyboardLookY = -1;
                            break;
                    }
                }
            }
        }

        // processes player movement via joystick
        void FindInputAxisActions()
        {

            if (String.IsNullOrEmpty(movementAxisBindingCache[0]) || String.IsNullOrEmpty(movementAxisBindingCache[1]))
                return;

            float horiz = Input.GetAxis(movementAxisBindingCache[0]);
            float vert = Input.GetAxis(movementAxisBindingCache[1]);

            if (vert != 0 || horiz != 0)
            {
                float dist = Mathf.Clamp(Mathf.Sqrt(horiz*horiz + vert*vert), controllerMinimumAxisFloat, 1.0F);

                if (dist > JoystickMovementThreshold)
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
                var actionVal = ActionNameToEnum(item.Value);
                if (!actionKeyDict.ContainsKey(key) && actionVal != Actions.Unknown)
                    actionKeyDict.Add(key, actionVal);
                else
                {
                    // This action is unknown in this game, make sure we still keep it so once we save the settings, we
                    // won't discard them.
                    unknownActions.Add(key, item.Value);
                }
            }

            if (keyBindsData.axisActionKeyBinds != null)
            {
                foreach (var item in keyBindsData.axisActionKeyBinds)
                {
                    if (!axisActionKeyDict.ContainsKey((String)item.Key))
                        axisActionKeyDict.Add((String)item.Key, item.Value);
                }
                UpdateAxisBindingCache();
            }
            else
            {
                keyBindsData.axisActionKeyBinds = new Dictionary<String, AxisActions>();
            }

            RaiseLoadedKeyBindsEvent();
        }

        static Actions ActionNameToEnum(string value)
        {
            Actions action;

            try
            {
                action = (Actions)Enum.Parse(typeof(Actions), value, true);
                return action;
            }
            catch (ArgumentException)
            {
                DaggerfallUnity.LogMessage("Unknown key action detected: " + value, true);
                return Actions.Unknown;
            }
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
