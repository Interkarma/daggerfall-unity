using System;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using DaggerfallWorkshop.Game.Utility.ModSupport.ModOrganizerSupport;
using UnityEditor;
using Directory = System.IO.Directory;

namespace DaggerfallWorkshop.Game.Utility.ModSupport.ModOrganizerSupport
{
    internal class ModOrganizerMenuItems
    {
        private const string ParentMenu = "Daggerfall Tools";
        private const string SubMenu = "Mod Organizer Import";
        private const string FullMenu = ParentMenu + "/" + SubMenu + "/";

        [MenuItem(FullMenu + "Locate Data")]
        static async void Select()
        {
            Debug.Log($"<color=orange>Select your Mod Organizer data directory.</color> " +
                      $"This path should contain your `profiles` and `mods` subfolders.");

            await Task.Delay(500);

            string dataPath = EditorUtility.OpenFolderPanel("Select Mod Organizer data path",
                String.Empty, String.Empty);

            if (string.IsNullOrWhiteSpace(dataPath))
            {
                return;
            }

            var requiredDirectories = new string[] { "profiles", "mods" };
            bool missingPath = false;

            foreach (var subfolder in requiredDirectories)
            {
                var directory = Path.Combine(dataPath, subfolder);
                if (Directory.Exists(directory)) continue;

                Debug.LogError(
                    $"Selected mod organizer data path either does not exist or does not have a \"{subfolder}\" subdirectory.\n" +
                    $"Has it moved? Reselect your mod organizer data path or fix your data.");
                missingPath = true;
            }

            if (missingPath)
            {
                return;
            }

            string profilePath = string.Empty;

            while (profilePath == string.Empty)
            {

                Debug.Log($"<color=orange>Select your profile directory.</color>");
                await Task.Delay(500);

                profilePath = EditorUtility.OpenFolderPanel("Select profile",
                    Path.Combine(dataPath, "profiles"), String.Empty);

                // user exited
                if (string.IsNullOrWhiteSpace(profilePath))
                    return;

                const string modListFileName = "modlist.txt";
                var hasModList = new DirectoryInfo(profilePath).GetFiles().Select(x => x.Name)
                    .Contains(modListFileName);
                if (hasModList) continue;

                Debug.LogError($"No mod list by the name of {modListFileName} found in {profilePath}");
                profilePath = string.Empty;
            }

            var data = new ModOrganizerData(dataPath, new DirectoryInfo(profilePath).Name);
            ModOrganizerData.Save(data);
            ModOrganizerLoader.DeleteModInstalls();
            ModOrganizerLoader.LoadMO2Mods();

            Debug.Log($"<color=green>Mod Organizer setup succeeded!</color>\n" +
                      $"Note that this means any mods in your StreamingAssets/Mods folder will be ignored. " +
                      $"Please bear in mind that Load Priority configured in ModOrganizer is not respected.");
        }

        [MenuItem(FullMenu + "Clear Data")]
        static void Clear()
        {
            var deleted = ModOrganizerLoader.DeleteModInstalls();
            if (!deleted)
            {
                Debug.Log($"No Mod Organizer files to delete");
                return;
            }

            Debug.LogWarning($"<color=orange>Mod Organizer copies deleted.</color>\n" +
                             $"Any files not present within a mod's \"Mods\" subdirectory or root folder remain in the StreamingAssets " +
                             $"folder and must be cleaned manually to avoid deleting wanted files.");
        }

        [MenuItem("Daggerfall Tools/Mod Organizer Import/Update mods from current profile")]
        static void Refresh()
        {
            ModOrganizerLoader.DeleteModInstalls();
            ModOrganizerLoader.LoadMO2Mods();
        }

        [MenuItem(FullMenu + "Open Imported Mod Install Path")]
        static void OpenModPath()
        {
            if (!Directory.Exists(ModOrganizerLoader.ModInstallPath))
            {
                Debug.LogError($"No Mod Organizer mods are currently installed.");
                return;
            }

            Application.OpenURL($"file:///{ModOrganizerLoader.ModInstallPath}");
        }

        private const string EnabledMenuName = FullMenu + "Enable";

        private static bool IsEnabled
        {
            get => ModOrganizerData.Enabled;
            set => ModOrganizerData.SetEnabled(value);
        }

        [MenuItem(EnabledMenuName)]
        private static void ToggleAction()
        {
            IsEnabled = !IsEnabled;
        }

        [MenuItem(EnabledMenuName, true)]
        private static bool ToggleActionValidate()
        {
            Menu.SetChecked(EnabledMenuName, IsEnabled);
            return true;
        }
    }
}