using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using UnityEngine;

namespace DaggerfallWorkshop.Game.Utility.ModSupport.ModOrganizerSupport
{
    internal static class ModOrganizerLoader
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        internal static void LoadMO2Mods()
        {
            var pathData = ModOrganizerData.Load();
            if (pathData is null) return;

            if (Application.isPlaying)
            {
                var modManager = UnityEngine.Object.FindObjectOfType<ModManager>();

                if (modManager is null)
                {
                    Debug.LogError($"Failed to find Mod Manager. Make sure you're starting from the " +
                                   $"DaggerfallUnityStartup scene if you wish to load mods from Mod Organizer.");
                    return;
                }

                modManager.ModDirectory = ModOrganizerData.ModInstallPath;
            }

            if (string.IsNullOrWhiteSpace(pathData.modOrganizerDataPath) ||
                string.IsNullOrWhiteSpace(pathData.SelectedProfilePath))
            {
                Debug.LogWarning($"No ModOrganizer data");
                return;
            }

            // we copy our mods to a path elsewhere to avoid Unity attempting to import everything

            var modOrganizerModPath = Path.Combine(pathData.modOrganizerDataPath, "mods");
            modOrganizerModPath = Path.GetFullPath(modOrganizerModPath);

            string profileModList = Path.Combine(pathData.SelectedProfilePath, "modlist.txt");
            var rawModList = File.ReadAllLines(profileModList).Where(x => !x.StartsWith("#")).ToArray();
            var modList = rawModList.Where(x => x.StartsWith("+")).Select(x => x.Remove(0, 1)).Reverse()
                .ToArray();

            var loadedModPath = new DirectoryInfo(ModOrganizerData.ModInstallPath);
            var streamingAssetsDirectory = new DirectoryInfo(Application.streamingAssetsPath);
            foreach (string modName in modList)
            {
                DirectoryInfo modDir = new DirectoryInfo(Path.Combine(modOrganizerModPath, modName));
                foreach (DirectoryInfo subDirectory in modDir.GetDirectories())
                {
                    DirectoryInfo target = IsModSubdirectory(subDirectory) ? loadedModPath : streamingAssetsDirectory;
                    target = target.CreateSubdirectory(subDirectory.Name);
                    Debug.Log($"<color=green>Found Mod Organizer mod: {modName}.</color><color=grey> Copying files if necessary.</color>" +
                              $"\n<color=grey>{subDirectory.FullName}</color> => {target.FullName}\n");
                    CopyAll(subDirectory, target);
                }

                foreach (FileInfo strayFile in modDir.GetFiles())
                {
                    string target = Path.Combine(loadedModPath.FullName, strayFile.Name);
                    Console.WriteLine($"Copying stray files\n<color=grey>{strayFile.FullName}</color> => {loadedModPath.FullName}\n");
                    strayFile.CopyTo(target, true);
                }
            }
        }

        private static bool IsModSubdirectory(DirectoryInfo subDirectory)
        {
            return subDirectory.Name.ToLowerInvariant() == "mods";
        }

        /// <summary>
        /// Adapted from https://stackoverflow.com/questions/9053564/c-sharp-merge-one-directory-with-another
        /// Copies files from source to target
        /// </summary>
        private static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            var sameDirectory = string.Equals(source.FullName, target.FullName, StringComparison.CurrentCultureIgnoreCase);
            if (sameDirectory)
            {
                Debug.LogWarning($"Attempting to copy a directory into itself.\n<color=grey>What were you thinking?</color>\n");
                return;
            }

            var files = source.GetFiles();
            var subdirectories = source.GetDirectories();

            var empty = files.Length == 0 && subdirectories.Length == 0;
            if (empty)
                return;

            // Check if the target directory exists, if not, create it.
            if (Directory.Exists(target.FullName) == false)
            {
                Directory.CreateDirectory(target.FullName);
            }

            // Copy each file into it's new directory.
            var fileLogs = string.Empty;
            foreach (FileInfo file in files)
            {
                var targetFilePath = Path.Combine(target.FullName, file.Name);

                if (File.Exists(targetFilePath))
                {
                    var targetFile = new FileInfo(targetFilePath);
                    if (targetFile.LastWriteTimeUtc >= file.LastWriteTimeUtc)
                        continue;
                }

                fileLogs += $"\n<color=grey>{file.FullName}</color> => {target.FullName}/{file.Name}";
                file.CopyTo(targetFilePath, true);
            }

            if(fileLogs != string.Empty)
                Debug.Log($"Copied files:{fileLogs}\n");

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in subdirectories)
            {
                DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }
    }
}