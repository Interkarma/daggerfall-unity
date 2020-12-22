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

        //there are only 16 recognized axes
        const int numAxes = 16;
        public const int startingAxisKeyCode = 5000;
        public const int startingComboKeyCode = 65537;
        const int totalHeldKeys = 6;

        //if the force is greater than this threshold, round it up to 1
        float joystickMovementThreshold = 0.95F;

        public Texture2D controllerCursorImage;

        Dictionary<int, String> axisKeyCodeStrings = new Dictionary<int, String>();
        String[] axisKeyCodeToInputAxis = new String[numAxes * 2];

        const string keyBindsFilename = "KeyBinds.txt";

        const float deadZone = 0.05f;
        const float inputWaitTotal = 0.0833f;
        const float moveAccelerationConst = 9.8f;

        KeyCode[] keyCodeList;
        KeyCode[] reservedKeys = new KeyCode[] { };

        // KeyCode linkage between primary and secondary keybinds
        Dictionary<int, int> primarySecondaryKeybindDict = new Dictionary<int, int>();
        // All primary keybinds
        Dictionary<KeyCode, Actions> actionKeyDict = new Dictionary<KeyCode, Actions>();
        // All secondary keybinds
        Dictionary<KeyCode, Actions> secondaryActionKeyDict = new Dictionary<KeyCode, Actions>();
        // Attempts to fill the gaps for primary keybinds set to None if a secondary keybind exists. Used for GetKey and Action enumeration
        Dictionary<KeyCode, Actions> existingKeyDict = new Dictionary<KeyCode, Actions>();
        Dictionary<String, AxisActions> axisActionKeyDict = new Dictionary<String, AxisActions>();
        Dictionary<KeyCode, string> unknownActions = new Dictionary<KeyCode, string>();
        Dictionary<KeyCode, string> secondaryUnknownActions = new Dictionary<KeyCode, string>();
        // Making keys 'int' instead of AxisActions because enum keys cause GC overhead in Unity
        Dictionary<int, bool> axisActionInvertDict = new Dictionary<int, bool>();
        Dictionary<KeyCode, JoystickUIActions> joystickUIDict = new Dictionary<KeyCode, JoystickUIActions>();

        KeyCode[] joystickUICache = new KeyCode[3]; //leftClick, rightClick, MiddleClick
        String[] cameraAxisBindingCache = new String[2];
        String[] movementAxisBindingCache = new String[2];
        Dictionary<int, bool> modifierHeldFirstDict = new Dictionary<int, bool>();
        Dictionary<int, Tuple<KeyCode, KeyCode>> comboCache = new Dictionary<int, Tuple<KeyCode, KeyCode>>();

        List<Actions> currentActions = new List<Actions>();
        List<Actions> previousActions = new List<Actions>();
        KeyCode[] heldKeys = new KeyCode[totalHeldKeys];
        KeyCode[] previousKeys = new KeyCode[totalHeldKeys];
        KeyCode heldModifier;
        int heldKeyCounter;
        int previousKeyCounter;

        // Storing these methods to prevent GC alloc in the GetKey methods
        Func<KeyCode, bool> getKeyMethod;
        Func<KeyCode, bool> getKeyDownMethod;
        Func<KeyCode, bool> getKeyUpMethod;

        bool isPaused;
        bool wasPaused;
        float inputWaitTimer;
        float horizontal;
        float vertical;
        float negHorizontalLimit = 1f;
        float posHorizontalLimit = 1f;
        float negVerticalLimit = 1f;
        float posVerticalLimit = 1f;
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
        bool moveAcceleration;

        float joystickUIMouseSensitivity = 1.0f;
        bool cursorVisible = true;
        bool usingControllerCursor;
        Vector2 controllerCursorPosition = new Vector2(0,0);
        int controllerCursorWidth = 32;
        int controllerCursorHeight = 32;
        float controllerCursorHorizontalSpeed = 300.0F;
        float controllerCursorVerticalSpeed = 300.0F;

        bool pauseController = false;

        #endregion

        #region Structures

        [fsObject("v1")]
        public class KeyBindData_v1
        {
            public Dictionary<String, string> actionKeyBinds;
            public Dictionary<String, String> secondaryActionKeyBinds;
            public Dictionary<String, AxisActions> axisActionKeyBinds;
            public Dictionary<String, String> axisActionInversions;
            public Dictionary<String, String> joystickUIKeyBinds;
        }

        #endregion

        #region Properties

        public bool IsPaused
        {
            get { return isPaused; }
            set { isPaused = value; }
        }

        public KeyCode[] KeyCodeList
        {
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
            get { return Mathf.Clamp((horizontal < -deadZone || horizontal > deadZone) ? horizontal : 0, -negHorizontalLimit, posHorizontalLimit); }
        }

        public float Vertical
        {
            get { return Mathf.Clamp((vertical < -deadZone || vertical > deadZone) ? vertical : 0, -negVerticalLimit, posVerticalLimit); }
        }

        public bool ToggleAutorun { get; set; }

        public float NegHorizontalLimit
        {
            get { return negHorizontalLimit; }
            set { negHorizontalLimit = Mathf.Clamp(value, 0f, 1f); }
        }

        public float PosHorizontalLimit
        {
            get { return posHorizontalLimit; }
            set { posHorizontalLimit = Mathf.Clamp(value, 0f, 1f); }
        }

        public float NegVerticalLimit
        {
            get { return negVerticalLimit; }
            set { negVerticalLimit = Mathf.Clamp(value, 0f, 1f); }
        }

        public float PosVerticalLimit
        {
            get { return posVerticalLimit; }
            set { posVerticalLimit = Mathf.Clamp(value, 0f, 1f); }
        }

        public bool UsingController
        {
            get { return usingControllerCursor && EnableController && !pauseController; }
        }

        public KeyCode LastKeyDown { get; private set; }
        public KeyCode LastSingleKeyDown { get; private set; }

        public bool CursorVisible
        {
            get { return cursorVisible; }
            set { cursorVisible = value; }
        }

        public Vector3 MousePosition {
            get {
                if (UsingController)
                    return controllerCursorPosition;
                else
                    return Input.mousePosition;
            }
        }

        public float JoystickCursorSensitivity
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

        public float JoystickMovementThreshold
        {
            get { return joystickMovementThreshold; }
            set { joystickMovementThreshold = value; }
        }

        public bool MaximizeJoystickMovement { get; set; }
        public float JoystickDeadzone { get; set; }

        public bool EnableController { get; set; }

        #endregion

        #region Enums

        public enum AxisActions {
            MovementHorizontal,
            MovementVertical,
            CameraHorizontal,
            CameraVertical,
        }

        public enum JoystickUIActions {
            LeftClick,
            RightClick,
            MiddleClick
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

            AutoRun,
            
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
            getKeyMethod = (k) => heldKeyCounter > 0 && ContainsKeyCode(heldKeys, k, true);
            getKeyDownMethod = (k) => heldKeyCounter > 0 && (previousKeyCounter <= 0 || !ContainsKeyCode(previousKeys, k, false)) && ContainsKeyCode(heldKeys, k, true);
            getKeyUpMethod = (k) => previousKeyCounter > 0 && ContainsKeyCode(previousKeys, k, false) && (heldKeyCounter <= 0 || !ContainsKeyCode(heldKeys, k, true));

            // Read acceleration setting
            moveAcceleration = DaggerfallUnity.Settings.MovementAcceleration;

            //memoization for 'axis keycodes'
            for (int i = startingAxisKeyCode; i < startingAxisKeyCode + numAxes * 2; i++)
            {
                axisKeyCodeStrings[i] = AxisKeyCodeToString(i);
                axisKeyCodeToInputAxis[i % startingAxisKeyCode] = AxisKeyCodeToInputAxis(i);
            }

            try
            {
                // Load a keybind file if possible
                if (HasKeyBindsSave())
                    LoadKeyBinds();

                // Ensure defaults are deployed if missing
                ResetDefaults(true);

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

            PollInput();

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
            mouseY = Input.GetAxisRaw("Mouse Y");


            if (EnableController && (mouseX == 0F || mouseY == 0F) && !String.IsNullOrEmpty(cameraAxisBindingCache[0]))
            {
                var h = Input.GetAxis(cameraAxisBindingCache[0]);
                var v = Input.GetAxis(cameraAxisBindingCache[1]);

                if (Mathf.Sqrt(h*h + v*v) > JoystickDeadzone)
                {
                    mouseX = h;
                    mouseY = v;
                }

                if (GetAxisActionInversion(AxisActions.CameraHorizontal))
                    mouseX *= -1;
                if (GetAxisActionInversion(AxisActions.CameraVertical))
                    mouseY *= -1;
            }

            if (ToggleAutorun)
            {
                ApplyVerticalForce(1);
            }

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
            var cameraHorizJ = Input.GetAxis(cameraAxisBindingCache[0]);
            var cameraVertJ = Input.GetAxis(cameraAxisBindingCache[1]);

            float distMovement = Mathf.Sqrt(horizj * horizj + vertj * vertj);
            float distCamera = Mathf.Sqrt(cameraHorizJ * cameraHorizJ + cameraVertJ * cameraVertJ);

            bool movingMouse = (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0);
            pauseController = movingMouse;

            if (!UsingController && (distMovement > JoystickDeadzone || distCamera > JoystickDeadzone))
            {
                usingControllerCursor = true;
                controllerCursorPosition = Input.mousePosition;
            }

            if (movingMouse)
                usingControllerCursor = false;

            if (CursorVisible)
            {
                if (UsingController)
                {
                    Cursor.visible = false;
                    //Cursor.lockState = CursorLockMode.Locked;

                    GUI.depth = 0;

                    if (GetAxisActionInversion(AxisActions.MovementHorizontal))
                        horizj *= -1;
                    if (GetAxisActionInversion(AxisActions.MovementVertical))
                        vertj *= -1;

                    if (distMovement > JoystickDeadzone)
                    {
                        controllerCursorPosition.x += JoystickCursorSensitivity * controllerCursorHorizontalSpeed * horizj * Time.fixedDeltaTime;
                        controllerCursorPosition.y += JoystickCursorSensitivity * controllerCursorVerticalSpeed * vertj * Time.fixedDeltaTime;
                    }

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
        /// Finds first non-None KeyCode bound to a specific action
        /// </summary>
        public KeyCode GetBinding(Actions action)
        {
            if (existingKeyDict.ContainsValue(action))
            {
                foreach (var k in existingKeyDict.Keys)
                {
                    if (existingKeyDict[k] == action)
                        return k;
                }
            }

            return KeyCode.None;
        }

        /// <summary>
        /// Finds KeyCode bound to a specific action, under either primary or secondary bindings
        /// </summary>
        public KeyCode GetBinding(Actions action, bool primary)
        {
            var dict = primary ? actionKeyDict : secondaryActionKeyDict;
            if (dict.ContainsValue(action))
            {
                foreach (var k in dict.Keys)
                {
                    if (dict[k] == action)
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

        public KeyCode GetJoystickUIBinding(JoystickUIActions action)
        {
            if (joystickUIDict.ContainsValue(action))
            {
                foreach (var k in joystickUIDict.Keys)
                {
                    if (joystickUIDict[k] == action)
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

        /// <summary>
        /// Binds a KeyCode to an action
        /// </summary>
        public void SetBinding(KeyCode code, Actions action, bool primary = true)
        {
            var dict = primary ? actionKeyDict : secondaryActionKeyDict;
            var alt = primary ? secondaryActionKeyDict : actionKeyDict;

            if (alt.ContainsKey(code))
            {
                alt.Remove(code);
            }

            ClearBinding(action, primary);

            if (code != KeyCode.None)
            {
                if (!dict.ContainsKey(code))
                {
                    dict.Add(code, action);
                }
                else
                {
                    dict.Remove(code);
                    dict.Add(code, action);
                }
            }

            MapSecondaryBindings(action);
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

        public void SetJoystickUIBinding(KeyCode code, JoystickUIActions action)
        {
            // Not allowing multi-bind at this time as the front-end doesn't support it
            ClearJoystickUIBinding(action);

            if (!joystickUIDict.ContainsKey(code))
            {
                joystickUIDict.Add(code, action);
            }
            else
            {
                joystickUIDict.Remove(code);
                joystickUIDict.Add(code, action);
            }
        }

        /// <summary>
        /// Unbinds a KeyCode to an action via KeyCode
        /// </summary>
        public void ClearBinding(KeyCode code, bool primary = true)
        {
            var dict = primary ? actionKeyDict : secondaryActionKeyDict;

            if (dict.ContainsKey(code))
            {
                dict.Remove(code);
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
        public void ClearBinding(Actions action, bool removePrimary = true)
        {
            var dict = removePrimary ? actionKeyDict : secondaryActionKeyDict;
            foreach (var binding in dict.Where(kvp => kvp.Value == action).ToList())
            {
                dict.Remove(binding.Key);
            }
        }

        /// <summary>
        /// Unbinds a KeyCode to a JoystickUIAction via string name
        /// </summary>
        public void ClearJoystickUIBinding(KeyCode code)
        {
            if (joystickUIDict.ContainsKey(code))
            {
                joystickUIDict.Remove(code);
            }
        }

        /// <summary>
        /// Unbinds a KeyCode to a JoystickUIAction via the JoystickUIAction
        /// </summary>
        public void ClearJoystickUIBinding(JoystickUIActions action)
        {
            foreach (var binding in joystickUIDict.Where(kvp => kvp.Value == action).ToList())
            {
                joystickUIDict.Remove(binding.Key);
            }
        }

        // Save keybindings
        public void SaveKeyBinds()
        {
            string path = GetKeyBindsSavePath();

            KeyBindData_v1 keyBindsData = new KeyBindData_v1();
            keyBindsData.axisActionKeyBinds = axisActionKeyDict;
            keyBindsData.actionKeyBinds = new Dictionary<string, string>();
            keyBindsData.axisActionInversions = new Dictionary<string, string>();
            keyBindsData.joystickUIKeyBinds = new Dictionary<string, string>();
            keyBindsData.secondaryActionKeyBinds = new Dictionary<string, string>();

            foreach (var item in actionKeyDict)
            {
                keyBindsData.actionKeyBinds.Add(GetKeyString(item.Key), item.Value.ToString());
            }

            foreach (var item in secondaryActionKeyDict)
            {
                keyBindsData.secondaryActionKeyBinds.Add(GetKeyString(item.Key), item.Value.ToString());
            }

            // If unknown actions were detected in this run, make sure we append them back to the settings file, so we won't break
            // the newer builds potentially using them.
            // If the key has been rebinded, then ignore the unknown action, as it should be repopulated in a newer build
            foreach (var item in unknownActions)
            {
                var key = GetKeyString(item.Key);
                if (!keyBindsData.actionKeyBinds.ContainsKey(key))
                    keyBindsData.actionKeyBinds.Add(key, item.Value);
            }

            foreach (var item in secondaryUnknownActions)
            {
                var key = GetKeyString(item.Key);
                if (!keyBindsData.secondaryActionKeyBinds.ContainsKey(key))
                    keyBindsData.secondaryActionKeyBinds.Add(key, item.Value);
            }

            foreach (var item in joystickUIDict)
            {
                keyBindsData.joystickUIKeyBinds.Add(GetKeyString(item.Key), item.Value.ToString());
            }

            foreach (var item in axisActionInvertDict)
            {
                keyBindsData.axisActionInversions.Add(((AxisActions)item.Key).ToString(), item.Value ? "True" : "False");
            }

            string json = SaveLoadManager.Serialize(keyBindsData.GetType(), keyBindsData);
            File.WriteAllText(path, json);
            UpdateBindingCache();
            RaiseSavedKeyBindsEvent();
        }

        public bool GetAxisActionInversion(AxisActions axis)
        {
            return axisActionInvertDict.ContainsKey((int)axis) && axisActionInvertDict[(int)axis];
        }

        public void SetAxisActionInversion(AxisActions axis, bool invert)
        {
            axisActionInvertDict[(int)axis] = invert;
        }

        // 'autofill' false: Forcefully set keybindings to defaults
        // 'autofill' true: Deploys default values if action missing from loaded keybinds
        public void ResetDefaults(bool autofill = false)
        {
            if (!autofill)
                actionKeyDict.Clear();

            Action<KeyCode, Actions, bool> setBinding;
            Action<string, AxisActions> setAxisBinding;
            Action<KeyCode, JoystickUIActions> setJoystickUIBinding;

            if(autofill)
            {
                setBinding = TestSetBinding;
                setAxisBinding = TestSetAxisBinding;
                setJoystickUIBinding = TestSetJoystickUIBinding;
            }
            else
            {
                setBinding = SetBinding;
                setAxisBinding = SetAxisBinding;
                setJoystickUIBinding = SetJoystickUIBinding;
            }

            setBinding(KeyCode.Escape, Actions.Escape, true);
            setBinding(KeyCode.BackQuote, Actions.ToggleConsole, true);

            setBinding(KeyCode.W, Actions.MoveForwards, true);
            setBinding(KeyCode.S, Actions.MoveBackwards, true);
            setBinding(KeyCode.A, Actions.MoveLeft, true);
            setBinding(KeyCode.D, Actions.MoveRight, true);
            setBinding(KeyCode.LeftArrow, Actions.TurnLeft, true);
            setBinding(KeyCode.RightArrow, Actions.TurnRight, true);

            setBinding(KeyCode.PageUp, Actions.FloatUp, true);
            setBinding(KeyCode.PageDown, Actions.FloatDown, true);
            setBinding(KeyCode.Space, Actions.Jump, true);
            setBinding(KeyCode.C, Actions.Crouch, true);
            setBinding(KeyCode.LeftControl, Actions.Slide, true);
            setBinding(KeyCode.LeftShift, Actions.Run, true);
            setBinding(KeyCode.Mouse2, Actions.AutoRun, true);

            setBinding(KeyCode.R, Actions.Rest, true);
            setBinding(KeyCode.T, Actions.Transport, true);
            setBinding(KeyCode.F1, Actions.StealMode, true);
            setBinding(KeyCode.F2, Actions.GrabMode, true);
            setBinding(KeyCode.F3, Actions.InfoMode, true);
            setBinding(KeyCode.F4, Actions.TalkMode, true);

            setBinding(KeyCode.Backspace, Actions.CastSpell, true);
            setBinding(KeyCode.Q, Actions.RecastSpell, true);
            setBinding(KeyCode.E, Actions.AbortSpell, true);
            setBinding(KeyCode.U, Actions.UseMagicItem, true);

            setBinding(KeyCode.Z, Actions.ReadyWeapon, true);
            setBinding(KeyCode.Mouse1, Actions.SwingWeapon, true);
            setBinding(KeyCode.H, Actions.SwitchHand, true);

            setBinding(KeyCode.I, Actions.Status, true);
            setBinding(KeyCode.F5, Actions.CharacterSheet, true);
            setBinding(KeyCode.F6, Actions.Inventory, true);

            setBinding(KeyCode.Mouse0, Actions.ActivateCenterObject, true);
            setBinding(KeyCode.Return, Actions.ActivateCursor, true);

            setBinding(KeyCode.Insert, Actions.LookUp, true);
            setBinding(KeyCode.Delete, Actions.LookDown, true);
            setBinding(KeyCode.Home, Actions.CenterView, true);
            setBinding(KeyCode.LeftAlt, Actions.Sneak, true);

            setBinding(KeyCode.L, Actions.LogBook, true);
            setBinding(KeyCode.N, Actions.NoteBook, true);
            setBinding(KeyCode.M, Actions.AutoMap, true);
            setBinding(KeyCode.V, Actions.TravelMap, true);

            setBinding(KeyCode.F8, Actions.PrintScreen, true);
            setBinding(KeyCode.F9, Actions.QuickSave, true);
            setBinding(KeyCode.F12, Actions.QuickLoad, true);

            setAxisBinding("Axis1", AxisActions.MovementHorizontal);
            setAxisBinding("Axis2", AxisActions.MovementVertical);
            setAxisBinding("Axis4", AxisActions.CameraHorizontal);
            setAxisBinding("Axis5", AxisActions.CameraVertical);

            setJoystickUIBinding(KeyCode.JoystickButton0, JoystickUIActions.LeftClick);
            setJoystickUIBinding(KeyCode.JoystickButton1, JoystickUIActions.RightClick);
            UpdateBindingCache();

            foreach (AxisActions axisAction in Enum.GetValues(typeof(AxisActions)))
                if (!autofill || !axisActionInvertDict.ContainsKey((int)axisAction))
                    SetAxisActionInversion(axisAction, false);
        }

        public bool GetMouseButtonDown(int button)
        {
            return Input.GetMouseButtonDown(button) || (EnableController && GetKeyDown(joystickUICache[button], false));
        }

        public bool GetMouseButtonUp(int button)
        {
            return Input.GetMouseButtonUp(button) || (EnableController && GetKeyUp(joystickUICache[button], false));
        }

        public bool GetMouseButton(int button)
        {
            return Input.GetMouseButton(button) || (EnableController && GetKey(joystickUICache[button], false));
        }

        public bool GetKey(KeyCode k, bool useSecondary = true)
        {
            if (heldKeyCounter == 0)
                return false;
            return GetUnaryKey(k, getKeyMethod, true) || (useSecondary && GetUnaryKey(GetSecondaryBinding(k), getKeyMethod, true));
        }

        public bool GetKeyDown(KeyCode k, bool useSecondary = true)
        {
            if (heldKeyCounter == 0)
                return false;
            return GetUnaryKey(k, getKeyDownMethod, true) || (useSecondary && GetUnaryKey(GetSecondaryBinding(k), getKeyDownMethod, true));
        }

        public bool GetKeyUp(KeyCode k, bool useSecondary = true)
        {
            if (previousKeyCounter == 0)
                return false;
            return GetUnaryKey(k, getKeyUpMethod, false) || (useSecondary && GetUnaryKey(GetSecondaryBinding(k), getKeyUpMethod, false));
        }

        public bool AnyKeyDown
        {
            get
            {
                foreach (KeyCode k in KeyCodeList)
                    if (GetUnaryKey(k, getKeyDownMethod, true, false)) return true;
                return false;
            }
        }

        public bool AnyKeyUp
        {
            get
            {
                foreach (KeyCode k in KeyCodeList)
                    if (GetUnaryKey(k, getKeyUpMethod, false, false)) return true;
                return false;
            }
        }

        public KeyCode GetComboCode(KeyCode a, KeyCode b)
        {
            uint x = (uint)a;
            uint y = (uint)b;

            // Only positive 16-bit keycodes <= 32767 allowed to be used for combos.
            // We want our combo code to be a positive signed integer to easily check against the const startingComboKeyCode,
            // so we only allow keycodes <= 32767 to avoid the most significant bit being 1 and making the combo code negative
            if (x > 32767 || y > 32767 || x < 0 || y < 0)
                return KeyCode.None;

            //from left-to-right: bits 0-15 are the first, 16-31 are the second
            return (KeyCode)(x << 16 | y);
        }

        public KeyCode GetComboCode(String s)
        {
            var splice = s.Split('+');
            if (splice.Length < 2)
                return KeyCode.None;

            var mod = ParseKeyCodeString(splice[0].TrimEnd());
            var key = ParseKeyCodeString(splice[1].TrimStart());

            if (mod == KeyCode.None || key == KeyCode.None)
                return KeyCode.None;

            return GetComboCode(mod, key);
        }

        public Tuple<KeyCode, KeyCode> GetCombo(KeyCode comboCode)
        {
            Tuple<KeyCode, KeyCode> cb;
            if (!comboCache.TryGetValue((int)comboCode, out cb))
            {
                uint a = (uint)comboCode >> 16;
                uint b = ((uint)comboCode << 16) >> 16;

                cb = new Tuple<KeyCode, KeyCode>((KeyCode)a, (KeyCode)b);
                comboCache[(int)comboCode] = cb;
            }

            return cb;
        }

        public String GetComboString(KeyCode comboCode)
        {
            var c = GetCombo(comboCode);
            return GetComboString(c.Item1, c.Item2);
        }

        public String GetComboString(KeyCode a, KeyCode b)
        {
            return String.Format("{0} + {1}", GetKeyString(a), GetKeyString(b));
        }

        public String GetKeyString(KeyCode key)
        {
            if ((int)key >= startingComboKeyCode)
                return GetComboString(key) ?? key.ToString();
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
                var axis = (KeyCode)axisKeyCodeStrings.FirstOrDefault(x => x.Value == s).Key;
                if (axis == KeyCode.None)
                    return GetComboCode(s);
                return axis;
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

        private void UpdateBindingCache()
        {
            joystickUICache[0] = GetJoystickUIBinding(JoystickUIActions.LeftClick);
            joystickUICache[1] = GetJoystickUIBinding(JoystickUIActions.RightClick);
            joystickUICache[2] = GetJoystickUIBinding(JoystickUIActions.MiddleClick);

            cameraAxisBindingCache[0] = GetAxisBinding(AxisActions.CameraHorizontal);
            cameraAxisBindingCache[1] = GetAxisBinding(AxisActions.CameraVertical);
            movementAxisBindingCache[0] = GetAxisBinding(AxisActions.MovementHorizontal);
            movementAxisBindingCache[1] = GetAxisBinding(AxisActions.MovementVertical);

            // Populate existingKeyDict with primary bindings
            existingKeyDict = new Dictionary<KeyCode, Actions>(actionKeyDict);
            var enums = Enum.GetValues(typeof(Actions));
            var set = new HashSet<Actions>(existingKeyDict.Values);

            // Populate missing bindings in existingKeyDict with secondary bindings
            foreach (Actions en in enums)
            {
                // If the existingKeyDict does not contain an action,
                // but the secondary bindings does, add it to our existingKeyDict
                if (!set.Contains(en))
                {
                    var key = GetBinding(en, false);
                    if (key != KeyCode.None)
                        existingKeyDict[key] = en;
                }
            }

            primarySecondaryKeybindDict.Clear();
            foreach (Actions a in enums)
                MapSecondaryBindings(a);

            var mods = primarySecondaryKeybindDict.Keys.Where(x => (int)x >= startingComboKeyCode);

            modifierHeldFirstDict.Clear();
            foreach (var key in mods)
            {
                modifierHeldFirstDict[(int)this.GetCombo((KeyCode)key).Item1] = false;
            }
        }

        KeyCode GetSecondaryBinding(KeyCode a)
        {
            int ret;
            if (primarySecondaryKeybindDict.TryGetValue((int)a, out ret))
                return (KeyCode)ret;

            return KeyCode.None;
        }

        void SetSecondaryBinding(KeyCode primary, KeyCode secondary)
        {
            primarySecondaryKeybindDict[(int)primary] = (int)secondary;
            primarySecondaryKeybindDict[(int)secondary] = (int)primary;
        }

        void MapSecondaryBindings(Actions a)
        {
            KeyCode primKey = actionKeyDict.FirstOrDefault(x => x.Value == a).Key;
            KeyCode secKey = secondaryActionKeyDict.FirstOrDefault(x => x.Value == a).Key;

            if (primKey != KeyCode.None && secKey != KeyCode.None)
            {
                SetSecondaryBinding(primKey, secKey);
            }
            else
            {
                int detached = 0;
                int existingKey = primKey != KeyCode.None ? (int)primKey : (int)secKey;

                if (primarySecondaryKeybindDict.TryGetValue(existingKey, out detached))
                {
                    primarySecondaryKeybindDict[existingKey] = (int)KeyCode.None;
                }

                primarySecondaryKeybindDict.Remove(detached);
            }
        }

        // Sets KeyCode binding only if action is missing,
        // and its default key is not used in alternate bindings.
        // This is to ensure default actions are restored if missing
        // and to push out new actions to existing keybind files
        private void TestSetBinding(KeyCode code, Actions action, bool primary = true)
        {
            var dict = primary ? actionKeyDict : secondaryActionKeyDict;
            var alt = primary ? secondaryActionKeyDict : actionKeyDict;

            if (!dict.ContainsValue(action) && !alt.ContainsKey(code))
            {
                SetBinding(code, action, primary);
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

        // Sets KeyCode binding only if action is missing
        // This is to ensure default actions are restored if missing
        // and to push out new actions to existing keybind files
        private void TestSetJoystickUIBinding(KeyCode code, JoystickUIActions action)
        {
            if (!joystickUIDict.ContainsValue(action))
            {
                SetJoystickUIBinding(code, action);
            }
        }

        // Apply force to horizontal axis
        public void ApplyHorizontalForce(float scale)
        {
            // Use acceleration setting or "just go"
            if (moveAcceleration)
                horizontal = Mathf.Clamp(horizontal + (moveAccelerationConst * scale) * Time.deltaTime, -1, 1);
            else
                horizontal = scale;

            if (scale < 0)
                negHorizontalImpulse = true;
            else if (scale > 0)
                posHorizontalImpulse = true;
        }

        // Apply force to vertical axis
        public void ApplyVerticalForce(float scale)
        {
            // Use acceleration setting or "just go"
            if (moveAcceleration)
                vertical = Mathf.Clamp(vertical + (moveAccelerationConst * scale) * Time.deltaTime, -1, 1);
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
            // Use acceleration setting or "just stop"
            if (moveAcceleration)
            {
                if (!posVerticalImpulse && vertical > 0)
                    vertical = Mathf.Clamp(vertical - moveAccelerationConst * Time.deltaTime, 0, vertical);
                if (!negVerticalImpulse && vertical < 0)
                    vertical = Mathf.Clamp(vertical + moveAccelerationConst * Time.deltaTime, vertical, 0);

                if (!posHorizontalImpulse && horizontal > 0)
                    horizontal = Mathf.Clamp(horizontal - moveAccelerationConst * Time.deltaTime, 0, horizontal);
                if (!negHorizontalImpulse && horizontal < 0)
                    horizontal = Mathf.Clamp(horizontal + moveAccelerationConst * Time.deltaTime, horizontal, 0);
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
        KeyCode[] GetKeyCodeList()
        {
            if (keyCodeList != null)
                return keyCodeList;

            HashSet<KeyCode> list = new HashSet<KeyCode>();

            foreach (var e in Enum.GetValues(typeof(KeyCode)))
            {
                var k = (KeyCode)e;
                if (k < KeyCode.Joystick1Button0 || k > KeyCode.Joystick8Button19)
                    list.Add(k);
                else
                    list.Add(ConvertJoystickButtonKeyCode(k));
            }

            foreach (var k in axisKeyCodeStrings.Keys)
                list.Add((KeyCode)k);

            // Remove keys that require a 'shift'
            list.Remove(KeyCode.Tilde);
            list.Remove(KeyCode.None);
            list.Remove(KeyCode.Exclaim);
            list.Remove(KeyCode.At);
            list.Remove(KeyCode.Hash);
            list.Remove(KeyCode.Dollar);
            list.Remove(KeyCode.Percent);
            list.Remove(KeyCode.Caret);
            list.Remove(KeyCode.Ampersand);
            list.Remove(KeyCode.Asterisk);
            list.Remove(KeyCode.LeftParen);
            list.Remove(KeyCode.RightParen);
            list.Remove(KeyCode.Underscore);
            list.Remove(KeyCode.Plus);
            list.Remove(KeyCode.Pipe);

            list.Remove(KeyCode.LeftCurlyBracket);
            list.Remove(KeyCode.RightCurlyBracket);
            list.Remove(KeyCode.Colon);
            list.Remove(KeyCode.DoubleQuote);

            list.Remove(KeyCode.Less);
            list.Remove(KeyCode.Greater);
            list.Remove(KeyCode.Question);

            list.Remove(KeyCode.Numlock);

            keyCodeList = list.ToArray();

            return keyCodeList;
        }

        // Checks whether the modifier is being held solely without any other keys that are combo'd to that modifier
        // E.g. 'LeftShift' is a modifier, LeftShift+K jumps, LeftShift+L opens a menu. It checks to make sure that 
        // either 'K' or 'L' are not being held. It will ignore keys that are not combo'd to it, like 'W' for forward.
        // It will also check to make sure no other modifiers are being held.
        bool ModifierOnlyHeld(KeyCode modifier)
        {
            if (heldKeys.Length == 1)
                return heldKeys[0] == modifier;
            else if (heldKeys.Length > 1)
            {
                for (int i = 0; i < heldKeyCounter; i++)
                {
                    var k = heldKeys[i];
                    if (modifier != k
                    && (primarySecondaryKeybindDict.ContainsKey((int)GetComboCode(modifier, k))
                        || modifierHeldFirstDict.ContainsKey((int)k)))
                        return false;
                }

                return true;
            }

            return false;
        }

        bool GetPollKey(KeyCode k)
        {
            if ((int)k < startingAxisKeyCode)
                return Input.GetKey(k);
            else
                return GetAxisKey((int)k);
        }

        bool GetAxisKey(int key)
        {
            if (!EnableController || key < startingAxisKeyCode)
                return false;

            if (key % 2 == 0)
                return Input.GetAxisRaw(axisKeyCodeToInputAxis[key % startingAxisKeyCode]) > 0;
            else
                return Input.GetAxisRaw(axisKeyCodeToInputAxis[key % startingAxisKeyCode]) < 0;
        }

        bool GetUnaryKey(KeyCode k, System.Func<KeyCode, bool> method, bool keyDown, bool checkModHeldFirst = true)
        {
            if (k == KeyCode.None)
                return false;
            
            bool hit = false;

            // If this is a non-combo KeyCode
            if ((int)k < startingComboKeyCode)
            {
                // If this key is not key'd up/down/held in the first place, stop and return false
                if (!method(k))
                    return false;

                // Return false if we are checking that a modifier is being held, and that this key is combo'd with that modifier.
                // E.g. 'space' is jump, 'LeftShift+Space' opens inventory. We want to ignore jumping if we were holding shift
                // prior to pressing 'space' so that way we *only* open the inventory window.
                if (checkModHeldFirst && heldModifier != KeyCode.None && modifierHeldFirstDict[(int)heldModifier]
                    && primarySecondaryKeybindDict.ContainsKey((int)GetComboCode(heldModifier, k)))
                        return false;

                hit = true;
            }
            else
            {
                // Get the tuple combo of this keycode
                var combo = GetCombo(k);
                
                // If the modifier is currently being held down (getKeyMethod)
                if (GetUnaryKey(combo.Item1, getKeyMethod, keyDown))
                {
                    // If no other key that is combo'd with this modifier is being held, then set the modifier's flag
                    // of being held first to true
                    if (ModifierOnlyHeld(combo.Item1))
                    {
                        modifierHeldFirstDict[(int)combo.Item1] = true;
                    }
                }
                else
                {
                    // else if the modifier is not being held down, set flag to false
                    modifierHeldFirstDict[(int)combo.Item1] = false;
                }

                // This method should return true if the modifier was held first and the combo'd key has hit down/up/held
                hit = modifierHeldFirstDict[(int)combo.Item1] && GetUnaryKey(combo.Item2, method, keyDown, false);
            }

            if (hit && keyDown)
            {
                LastKeyDown = k;

                // Specifically made for the controls window that wants to grab individual keys when rebinding
                if ((int)k < startingComboKeyCode)
                    LastSingleKeyDown = k;
            }

            return hit;
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
        //Used by joystick controls window
        public String AxisKeyCodeToInputAxis(int key)
        {
            if (key < startingAxisKeyCode)
                return String.Empty;

            int axisNum = (key % startingAxisKeyCode) / 2 + 1;

            if (axisNum > numAxes)
                return String.Empty;

            return String.Concat("Axis", axisNum);
        }

        // Provides significant optimizations for polling
        bool ContainsKeyCode(KeyCode[] keys, KeyCode key, bool heldKeys)
        {
            int len = heldKeys ? heldKeyCounter : previousKeyCounter;
            for (int i = 0; i < len; i++)
            {
                if (keys[i] == key)
                    return true;
            }

            return false;
        }

        void AddHeldKey(KeyCode[] keys, KeyCode key)
        {
            if (heldKeyCounter >= totalHeldKeys)
                return;

            keys[heldKeyCounter++] = key;
        }

        void SetPreviousKeys(KeyCode[] keys1, KeyCode[] keys2)
        {
            for (int i = 0; i < heldKeyCounter; i++)
            {
                keys1[i] = keys2[i];
            }
        }

        void PollInput()
        {
            heldModifier = KeyCode.None;

            previousKeyCounter = heldKeyCounter;
            SetPreviousKeys(previousKeys, heldKeys);

            // Clear current actions
            heldKeyCounter = 0;

            foreach (KeyCode k in KeyCodeList)
            {
                if (GetPollKey(k))
                    AddHeldKey(heldKeys, k);

                if (getKeyDownMethod(k))
                    DaggerfallUI.Instance.OnKeyPress(k, true);

                if (getKeyUpMethod(k))
                    DaggerfallUI.Instance.OnKeyPress(k, false);
            }

            foreach (KeyCode modifier in modifierHeldFirstDict.Keys)
            {
                if (GetKey(modifier))
                    heldModifier = modifier;
            }
        }

        // Enumerate all keyboard actions in progress
        void FindKeyboardActions()
        {
            var enumerator = existingKeyDict.GetEnumerator();
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
                            ToggleAutorun = false;
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

            if (!EnableController || String.IsNullOrEmpty(movementAxisBindingCache[0]) || String.IsNullOrEmpty(movementAxisBindingCache[1]))
                return;

            float horiz = Input.GetAxis(movementAxisBindingCache[0]);
            float vert = Input.GetAxis(movementAxisBindingCache[1]);

            if (vert != 0 || horiz != 0)
            {
                if (GetAxisActionInversion(AxisActions.MovementHorizontal))
                    horiz *= -1;

                if (GetAxisActionInversion(AxisActions.MovementVertical))
                    vert *= -1;

                float jd = Mathf.Sqrt(horiz*horiz + vert*vert);

                if (jd <= JoystickDeadzone)
                    return;

                float dist = jd / JoystickMovementThreshold;

                if (MaximizeJoystickMovement || dist > 1.0F)
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
                    ToggleAutorun = false;
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
            return Path.Combine(DaggerfallUnity.Settings.PersistentDataPath, keyBindsFilename);
        }

        bool HasKeyBindsSave()
        {
            if (File.Exists(GetKeyBindsSavePath()))
                return true;

            return false;
        }

        void LoadActionKeybinds(bool primary, KeyBindData_v1 keyBindsData)
        {
            var saved = primary ? keyBindsData.actionKeyBinds : keyBindsData.secondaryActionKeyBinds;
            var dict = primary ? actionKeyDict : secondaryActionKeyDict;
            var unknown = primary ? unknownActions : secondaryUnknownActions;

            foreach (var item in saved)
            {
                KeyCode key = ParseKeyCodeString(item.Key);
                var actionVal = ActionNameToEnum(item.Value);
                if (!dict.ContainsKey(key) && actionVal != Actions.Unknown)
                    dict.Add(key, actionVal);
                else
                {
                    // This action is unknown in this game, make sure we still keep it so once we save the settings, we
                    // won't discard them.
                    unknown.Add(key, item.Value);
                }
            }
        }

        void LoadKeyBinds()
        {
            string path = GetKeyBindsSavePath();

            string json = File.ReadAllText(path);
            KeyBindData_v1 keyBindsData = SaveLoadManager.Deserialize(typeof(KeyBindData_v1), json) as KeyBindData_v1;

            LoadActionKeybinds(true, keyBindsData);

            if (keyBindsData.secondaryActionKeyBinds != null)
                LoadActionKeybinds(false, keyBindsData);

            if (keyBindsData.axisActionKeyBinds != null)
            {
                foreach (var item in keyBindsData.axisActionKeyBinds)
                {
                    if (!axisActionKeyDict.ContainsKey((String)item.Key))
                        axisActionKeyDict.Add((String)item.Key, item.Value);
                }
                UpdateBindingCache();
            }
            else
            {
                keyBindsData.axisActionKeyBinds = new Dictionary<String, AxisActions>();
            }

            if (keyBindsData.joystickUIKeyBinds != null)
            {
                foreach (var item in keyBindsData.joystickUIKeyBinds)
                {
                    KeyCode key = ParseKeyCodeString(item.Key);
                    JoystickUIActions val = (JoystickUIActions)Enum.Parse(typeof(JoystickUIActions), item.Value);
                    if (!joystickUIDict.ContainsKey(key))
                        joystickUIDict.Add(key, val);
                }
                UpdateBindingCache();
            }

            if (keyBindsData.axisActionInversions != null)
            {
                foreach (var item in keyBindsData.axisActionInversions)
                {
                    AxisActions key = (AxisActions)Enum.Parse(typeof(AxisActions), item.Key);
                    SetAxisActionInversion(key, item.Value == "True" ? true : false);
                }
            }
            else
            {
                keyBindsData.axisActionInversions = new Dictionary<String, string>();
                foreach (AxisActions axisAction in Enum.GetValues(typeof(AxisActions)))
                    SetAxisActionInversion(axisAction, false);
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
