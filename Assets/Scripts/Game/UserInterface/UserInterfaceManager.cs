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

namespace DaggerfallWorkshop.Game.UserInterface
{
    public interface IUserInterfaceManager
    {
        event EventHandler OnWindowChange;
        UserInterfaceWindow TopWindow { get; }
        void PopWindow();
        void PushWindow(UserInterfaceWindow window);
        bool ContainsWindow(UserInterfaceWindow window);
        void ChangeWindow(UserInterfaceWindow newWindow);
        int MessageCount { get; }
        int WindowCount { get; }
        void PostMessage(string message);
        string GetMessage();
        string PeekMessage();
    }

    /// <summary>
    /// Manages a stack of user interface windows.
    /// Provides a simple window messaging system for passing information and events via string.
    /// </summary>
    public class UserInterfaceManager : IUserInterfaceManager
    {
        const int maxMessageCount = 10;

        Queue<string> messages = new Queue<string>();
        Stack<UserInterfaceWindow> windows = new Stack<UserInterfaceWindow>();
        public event EventHandler OnWindowChange;

        /// <summary>
        /// Constructor.
        /// </summary>
        public UserInterfaceManager()
        {
        }

        /// <summary>
        /// Peeks window at top of stack.
        /// </summary>
        public UserInterfaceWindow TopWindow
        {
            get { return (windows.Count > 0) ? windows.Peek() : null; }
        }

        /// <summary>
        /// Number of messages in stack.
        /// </summary>
        public int MessageCount
        {
            get { return messages.Count; }
        }

        /// <summary>
        /// Number of windows in stack (not including HUD)
        /// </summary>
        public int WindowCount
        {
            get { return windows.Count-1; }
        }

        /// <summary>
        /// Push a new window onto the stack.
        /// </summary>
        /// <param name="window">New window.</param>
        public void PushWindow(UserInterfaceWindow window)
        {
            // Add window
            AddWindow(window);

            // Raise event
            if (OnWindowChange != null)
                OnWindowChange(this, null);
        }

        /// <summary>
        /// Pop a window from the stack.
        /// </summary>
        public void PopWindow()
        {
            // Remove window
            RemoveWindow();

            // Raise event
            if (OnWindowChange != null)
                OnWindowChange(this, null);
        }

        /// <summary>
        /// Checks stack for a specific window.
        /// </summary>
        /// <param name="window">Window to look for.</param>
        /// <returns>True if window exists on stack.</returns>
        public bool ContainsWindow(UserInterfaceWindow window)
        {
            return (windows.Contains(window));
        }

        /// <summary>
        /// Replace entire stack with a new window.
        /// </summary>
        /// <param name="window">New window.</param>
        public void ChangeWindow(UserInterfaceWindow window)
        {
            // We are changing windows so pop everything
            while (windows.Count > 0)
                RemoveWindow();

            // Reset draw order
            AddWindow(window);

            if (OnWindowChange != null)
                OnWindowChange(this, null);
        }

        /// <summary>
        /// Post message to end of queue.
        /// </summary>
        public void PostMessage(string message)
        {
            if (MessageCount < maxMessageCount)
            {
                messages.Enqueue(message);
            }
            else
            {
                // Clear message queue on overflow as not all handlers implemented yet
                // TODO: Implement a more suitable collection for messaging
                messages.Clear();
                messages.Enqueue(message);
            }
        }

        /// <summary>
        /// Get message at front of queue and remove message.
        /// </summary>
        public string GetMessage()
        {
            if (MessageCount > 0)
                return messages.Dequeue();
            else
                return string.Empty;
        }

        /// <summary>
        /// Peek message at front of queue with removing.
        /// </summary>
        public string PeekMessage()
        {
            if (MessageCount > 0)
                return messages.Peek();
            else
                return string.Empty;
        }

        /// <summary>
        /// Add window to stack.
        /// </summary>
        private void AddWindow(UserInterfaceWindow window)
        {
            windows.Push(window);
            window.OnPush();
            //OnWindowChange += window.WindowChanged;
            if(window.PauseWhileOpen && GameManager.HasInstance)
                GameManager.Instance.PauseGame(true);
        }

        /// <summary>
        /// Remove window from stack.
        /// </summary>
        private void RemoveWindow()
        {
            UserInterfaceWindow oldWindow = TopWindow;
            if (oldWindow != null)
            {
                //OnWindowChange -= oldWindow.WindowChanged;
                oldWindow.OnPop();
                windows.Pop();
            }
            if (windows.Count <= 1 && GameManager.HasInstance)
                GameManager.Instance.PauseGame(false);
        }

        /// <summary>
        /// Builds a parameter dictionary from message string.
        /// </summary>
        public static Dictionary<string, string> BuildParamDict(string message)
        {
            char[] paramDelimiter = { '?' };
            char[] equalDelimiter = { '=' };

            Dictionary<string, string> dict = new Dictionary<string, string>();
            string[] parameters = message.Split(paramDelimiter, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < parameters.Length; i++)
            {
                string[] parts = parameters[i].Split(equalDelimiter);
                if (parts.Length != 2)
                    continue;
                else
                    dict.Add(parts[0].Trim(), parts[1].Trim());
            }

            return dict;
        }
    }
}
