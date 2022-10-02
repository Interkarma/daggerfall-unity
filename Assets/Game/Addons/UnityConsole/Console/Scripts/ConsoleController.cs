using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;


namespace Wenzil.Console
{
    /// <summary>
    /// The behavior of the console.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ConsoleController))]
    public class ConsoleController : MonoBehaviour
    {
        private const int inputHistoryCapacity = 20;

        public ConsoleUI ui;
        public KeyCode toggleKey = KeyCode.BackQuote;
        public bool closeOnEscape = false;
        public string AwakeMessage = "Type Help for list of commands and Help <command> for more details on a specific command";

        private ConsoleInputHistory inputHistory = new ConsoleInputHistory(inputHistoryCapacity);

        void Awake()
        {
            /* This instantiation causes a bug when Unity rebuilds the project while in play mode
               Solution: move it to class level initialization, and make inputHistoryCapacity a const */
            // inputHistory = new ConsoleInputHistory(inputHistoryCapacity); 
        }

        void Start()
        {
            DaggerfallWorkshop.Game.InputManager.OnSavedKeyBinds += GetConsoleKeyBind;
        }

        void OnEnable()
        {
            DaggerfallWorkshop.Game.InputManager.OnLoadedKeyBinds += GetConsoleKeyBind;
            Console.OnConsoleLog += ui.AddNewOutputLine;
            ui.onSubmitCommand += ExecuteCommand;
            ui.onClearConsole += inputHistory.Clear;
            ui.AddNewOutputLine(AwakeMessage);
        }

        void OnDisable()
        {
            DaggerfallWorkshop.Game.InputManager.OnLoadedKeyBinds -= GetConsoleKeyBind;
            Console.OnConsoleLog -= ui.AddNewOutputLine;
            ui.onSubmitCommand -= ExecuteCommand;
            ui.onClearConsole -= inputHistory.Clear;
        }

        void Update()
        {
            if (DaggerfallWorkshop.Game.InputManager.Instance.GetKeyDown(toggleKey))
                ui.ToggleConsole();
            else if (DaggerfallWorkshop.Game.InputManager.Instance.GetBackButtonDown() && closeOnEscape)
                ui.CloseConsole();
            else if (Input.GetKeyDown(KeyCode.UpArrow))
                NavigateInputHistory(true);
            else if (Input.GetKeyDown(KeyCode.DownArrow))
                NavigateInputHistory(false);
        }

        private void NavigateInputHistory(bool up)
        {
            string navigatedToInput = inputHistory.Navigate(up);
            ui.SetInputText(navigatedToInput);
        }

        private void ExecuteCommand(string input)
        {
            string[] parts = input.Split(' ');
            string command = parts[0];
            string[] args = parts.Skip(1).ToArray();

            Console.Log("> " + input);
            Console.Log(ConsoleCommandsDatabase.ExecuteCommand(command, args));
            inputHistory.AddNewInputEntry(input);
        }


        public void GetConsoleKeyBind()
        {
            toggleKey = DaggerfallWorkshop.Game.InputManager.Instance.GetBinding(DaggerfallWorkshop.Game.InputManager.Actions.ToggleConsole);
            if (toggleKey == KeyCode.None)
                toggleKey = KeyCode.BackQuote;
        }



    }
}
