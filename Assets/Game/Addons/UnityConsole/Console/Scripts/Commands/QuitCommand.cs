using UnityEngine;

namespace Wenzil.Console.Commands
{
    /// <summary>
    /// QUIT command. Quit the application.
    /// </summary>
    public static class QuitCommand
    {
        public static readonly string name = "QUIT";
        public static readonly string description = "Quit the application.";
        public static readonly string usage = "QUIT";

        public static string Execute(params string[] args)
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            return "Quitting...";
        }
    }
}