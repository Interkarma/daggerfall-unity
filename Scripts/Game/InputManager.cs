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

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// InputManager singleton class for Daggerfall-specific game actions.
    /// </summary>
    public class InputManager : MonoBehaviour
    {
        #region Fields

        const float acceleration = 3f;
        const float friction = 3f;
        const float deadZone = 0.1f;

        KeyCode[] reservedKeys = new KeyCode[] { KeyCode.Escape };
        Dictionary<KeyCode, Actions> actionKeyDict = new Dictionary<KeyCode, Actions>();
        List<Actions> currentActions = new List<Actions>();
        bool paused;
        float horizontal;
        float vertical;
        bool posHorizontalImpulse;
        bool negHorizontalImpulse;
        bool posVerticalImpulse;
        bool negVerticalImpulse;

        #endregion

        #region Properties

        public bool IsPaused
        {
            get { return paused; }
            set { paused = value; }
        }

        public KeyCode[] ReservedKeys
        {
            get { return (KeyCode[])reservedKeys.Clone(); }
        }

        public Actions[] CurrentActions
        {
            get { return currentActions.ToArray(); }
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

        void Start()
        {
            SetupSingleton();
            SetupDefaults();
        }

        void Update()
        {
            // Current actions are cleared at start of every frame
            currentActions.Clear();

            // Clear axis impulse flags, these will be raised again on movement
            posHorizontalImpulse = false;
            negHorizontalImpulse = false;
            posVerticalImpulse = false;
            negVerticalImpulse = false;

            // Do nothing if paused
            if (paused)
                return;

            // Process actions from input sources
            RunKeyboardActions();

            // Apply friction to movement force
            ApplyFriction();
        }

        #endregion

        #region Public Methods

        public bool HasAction(Actions action)
        {
            return currentActions.Contains(action);
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

        // Enumerate all keyboard actions in progress
        void RunKeyboardActions()
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
    }
}
