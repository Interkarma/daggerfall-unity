using System;
using System.IO;
using DaggerfallWorkshop;
using UnityEngine;

[Serializable]
internal class ModOrganizerData
{
    [SerializeField]
    internal string modOrganizerDataPath;
    [SerializeField]
    internal string profileName;

    internal ModOrganizerData(string modOrganizerDataPath, string profileName)
    {
        this.modOrganizerDataPath = modOrganizerDataPath;
        this.profileName = profileName;
    }

    internal string SelectedProfilePath => Path.Combine(modOrganizerDataPath, "profiles", profileName);

    private static string fileName = "mod-organizer-import-settings.json";
    private static string FilePath => Path.Combine(EditorPersistentModData, fileName);

    internal static string EditorPersistentModData =>
        Path.Combine(DaggerfallUnity.Settings.PersistentDataPath, "Mods", "EditorData");

    internal static string ModInstallPath => Path.Combine(EditorPersistentModData, "mod-organizer-mod-installs");

    internal static void Save(ModOrganizerData modOrganizerData)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
        File.WriteAllText(FilePath,JsonUtility.ToJson(modOrganizerData, true));
    }

    internal static ModOrganizerData Load()
    {
        if (!File.Exists(FilePath)) return null;

        string fileContents = File.ReadAllText(FilePath);
        Debug.Log($"<color=orange>Loaded {nameof(ModOrganizerData)}</color> from {FilePath}");
        return JsonUtility.FromJson<ModOrganizerData>(fileContents);
    }

    internal static bool Delete()
    {
        bool deleted = false;
        if (File.Exists(FilePath))
        {
            File.Delete(FilePath);
            deleted = true;
        }

        if (Directory.Exists(ModInstallPath))
        {
            Directory.Delete(ModInstallPath, true);
            deleted = true;
        }

        return deleted;
    }
}