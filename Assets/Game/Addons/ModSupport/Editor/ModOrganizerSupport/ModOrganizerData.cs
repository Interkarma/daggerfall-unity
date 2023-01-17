using System;
using System.IO;
using DaggerfallWorkshop;
using UnityEngine;

namespace DaggerfallWorkshop.Game.Utility.ModSupport.ModOrganizerSupport
{
    [Serializable]
    internal class ModOrganizerData
    {
        [SerializeField] internal string modOrganizerDataPath;
        [SerializeField] internal string profileName;
        [SerializeField] internal bool enableLoad;

        private static ModOrganizerData instance;

        private static ModOrganizerData Instance
        {
            get
            {
                if (instance is null)
                    Load();

                return instance;
            }
            set => instance = value;
        }

        internal ModOrganizerData(string modOrganizerDataPath, string profileName)
        {
            this.modOrganizerDataPath = modOrganizerDataPath;
            this.profileName = profileName;
            enableLoad = Instance is null || Instance.enableLoad;
            Instance = this;
        }

        internal string SelectedProfilePath => Path.Combine(modOrganizerDataPath, "profiles", profileName);

        private static string fileName = "mod-organizer-import-settings.json";
        private static string FilePath => Path.Combine(ModOrganizerLoader.EditorPersistentModData, fileName);

        public static bool Enabled => Instance?.enableLoad ?? false;

        internal static void Save(ModOrganizerData modOrganizerData)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
            File.WriteAllText(FilePath, JsonUtility.ToJson(modOrganizerData, true));
        }

        internal static ModOrganizerData Load()
        {
            if (!File.Exists(FilePath)) return null;

            string fileContents = File.ReadAllText(FilePath);
            ModOrganizerData loaded = JsonUtility.FromJson<ModOrganizerData>(fileContents);

            if (loaded != null)
                Instance = loaded;

            return loaded;
        }

        internal static void SetEnabled(bool on)
        {
            if (Instance is null)
                return;

            Instance.enableLoad = on;
            Save(Instance);
        }
    }
}