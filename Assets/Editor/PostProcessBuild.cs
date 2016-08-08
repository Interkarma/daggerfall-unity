using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;

public class PostProcessBuild
{
    [PostProcessBuildAttribute(1)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        const string defaultModsFolderName = "StreamingAssets";

        if (target == BuildTarget.StandaloneWindows || target == BuildTarget.StandaloneWindows64 ||
            target == BuildTarget.StandaloneLinux || target == BuildTarget.StandaloneLinux64 || target == BuildTarget.StandaloneLinuxUniversal ||
            target == BuildTarget.StandaloneOSXIntel || target == BuildTarget.StandaloneOSXIntel64 || target == BuildTarget.StandaloneOSXUniversal)
        {
            // Get build path
            string pureBuildPath = Path.GetDirectoryName(pathToBuiltProject);

            // Remove PDB files
            foreach (string file in Directory.GetFiles(pureBuildPath, "*.pdb"))
            {
                Debug.Log(file + " deleted!");
                File.Delete(file);
            }

            // Create default mods folder
            Directory.CreateDirectory(Path.Combine(pureBuildPath, defaultModsFolderName));
        }
    }
}