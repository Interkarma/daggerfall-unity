using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace Wenzil.Console
{

    /// <summary>
    /// The interactive front-end of the console.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ConsoleController))]
    public class ConsoleUI : MonoBehaviour, IScrollHandler
    {
        public event Action<bool> onToggleConsole;
        public event Action<string> onSubmitCommand;
        public event Action onClearConsole;

        public Scrollbar scrollbar;
        public Text outputText;
        public ScrollRect outputArea;
        public InputField inputField;

        bool previousPauseState = false;
        CanvasGroup cg;
        int maxOutputLength = 12000;

        /// <summary>
        /// Indicates whether the console is currently open or close.
        /// </summary>
        public bool isConsoleOpen { get { return enabled; } }

        void Awake()
        {
            Show(false);
        }

        /// <summary>
        /// Opens or closes the console.
        /// </summary>
        public void ToggleConsole(bool force = false)
        {
            //refresh keybinds if player rebinded it
            transform.GetComponent<ConsoleController>().GetConsoleKeyBind();

            // Do nothing if HUD is not top window (e.g. player in some other menu)
            if (!DaggerfallWorkshop.Game.GameManager.Instance.IsPlayerOnHUD && !force)
                return;

            // Do nothing if console not enabled in settings
            if (!DaggerfallWorkshop.DaggerfallUnity.Settings.LypyL_GameConsole && !force)
                return;

            inputField.text = string.Empty;
            enabled = !enabled; 
            if(enabled)
            {
                previousPauseState = DaggerfallWorkshop.Game.GameManager.IsGamePaused;
                DaggerfallWorkshop.Game.GameManager.Instance.PauseGame(enabled, true);
            }
            else
                DaggerfallWorkshop.Game.GameManager.Instance.PauseGame(previousPauseState);

            if (!cg)
                cg = this.GetComponent<CanvasGroup>();
            if (cg != null)
                cg.blocksRaycasts = enabled;
        }

        /// <summary>
        /// Opens the console.
        /// </summary>
        public void OpenConsole()
        {
            enabled = true;
        }

        /// <summary>
        /// Closes the console.
        /// </summary>
        public void CloseConsole()
        {
            enabled = false;
        }

        void OnEnable()
        {
            OnToggle(true);
        }

        void OnDisable()
        {
            OnToggle(false);
        }

        private void OnToggle(bool open)
        {
            Show(open);

            if (open)
                inputField.ActivateInputField();
            else
                ClearInput();

            if (onToggleConsole != null)
                onToggleConsole(open);
        }

        private void Show(bool show)
        {
            inputField.gameObject.SetActive(show);
            outputArea.gameObject.SetActive(show);
            scrollbar.gameObject.SetActive(show);
        }

        /// <summary>
        /// What to do when the user wants to submit a command.
        /// </summary>
        public void OnSubmit(string input)
        {
            if (EventSystem.current.alreadySelecting) // if user selected something else, don't treat as a submit
                return;

            if (input.Length > 0)
            {
                if (onSubmitCommand != null)
                    onSubmitCommand(input);
                scrollbar.value = 0;
                ClearInput();
            }

            inputField.ActivateInputField();
        }

        /// <summary>
        /// What to do when the user uses the scrollwheel while hovering the console input.
        /// </summary>
        public void OnScroll(PointerEventData eventData)
        {
            scrollbar.value += 0.08f * eventData.scrollDelta.y;
        }

        /// <summary>
        /// Displays the given message as a new entry in the console output.
        /// </summary>
        public void AddNewOutputLine(string line)
        {
            string newOutput = outputText.text += Environment.NewLine + line;

            //if output length gets too high Unity's Text Mesh will throw errors
            if(newOutput != null && newOutput.Length > maxOutputLength)
            {
                newOutput = newOutput.Substring(newOutput.Length-maxOutputLength);
                //start at first line break to try and avoid breaking rich text formatting
                int newStartPosition = line.IndexOf("\n");
                if(newStartPosition > 0)
                    newOutput = newOutput.Substring(newStartPosition);
            }
            outputText.text = newOutput;
        }

        /// <summary>
        /// Clears the console output.
        /// </summary>
        public void ClearOutput()
        {
            outputText.text = "";
            outputText.SetLayoutDirty();
            if(onClearConsole != null)
                onClearConsole();
        }

        /// <summary>
        /// Clears the console input.
        /// </summary>
        public void ClearInput()
        {
            SetInputText("");
        }

        /// <summary>
        /// Writes the given string into the console input, ready to be user submitted.
        /// </summary>
        public void SetInputText(string input) 
        {
            inputField.MoveTextStart(false);
            inputField.text = input;
            inputField.MoveTextEnd(false);
        }
    }
}