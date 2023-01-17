using System;
using UnityEngine;
using System.IO;
using System.Linq;
using UnityEditor;
using Directory = System.IO.Directory;

public class ModOrganizerMenuItems
{
    [MenuItem("Daggerfall Tools/Mod Organizer Import/Locate Data")]
    static void Select()
    {
        // Get existing open window or if none, make a new one:
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
            profilePath = EditorUtility.OpenFolderPanel("Select profile",
            Path.Combine(dataPath, "profiles"), String.Empty);

            // user exited
            if (string.IsNullOrWhiteSpace(profilePath))
                return;

            const string modListFileName = "modlist.txt";
            var hasModList = new DirectoryInfo(profilePath).GetFiles().Select(x => x.Name).Contains(modListFileName);
            if (hasModList) continue;

            Debug.LogError($"No mod list by the name of {modListFileName} found in {profilePath}");
            profilePath = string.Empty;
        }

        var data = new ModOrganizerData(dataPath, new DirectoryInfo(profilePath).Name);
        ModOrganizerData.Save(data);

        Debug.Log($"<color=green>Mod Organizer setup succeeded!</color>\n" +
                  $"Note that this means any mods in your StreamingAssets/Mods folder will be ignored. " +
                  $"Please bear in mind that Load Priority configured in ModOrganizer is not respected.");
    }

    [MenuItem("Daggerfall Tools/Mod Organizer Import/Clear Data")]
    static void Clear()
    {
        var deleted = ModOrganizerData.Delete();
        if (!deleted)
        {
            Debug.Log($"No Mod Organizer files to delete");
            return;
        }

        Debug.LogWarning($"<color=orange>Mod Organizer copies deleted.</color>\n" +
                         $"Any files not present within a mod's \"Mods\" subdirectory or root folder remain in the StreamingAssets " +
                         $"folder and must be cleaned manually to avoid deleting wanted files.");
    }
}