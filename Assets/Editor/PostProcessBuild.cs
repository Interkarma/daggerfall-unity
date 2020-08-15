using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;

public class PostProcessBuild
{
    [PostProcessBuildAttribute(1)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        //const string defaultModsFolderName = "StreamingAssets";
        const string manualFileName = "Daggerfall Unity Manual.pdf";
        //const string readMeFilename = "readme.txt";
        //const string modReadMeText = "Place your .dfmod files in this folder for the mod system.";

        if (target == BuildTarget.StandaloneWindows || target == BuildTarget.StandaloneWindows64 ||
            target == BuildTarget.StandaloneLinux64 ||
            target == BuildTarget.StandaloneOSX)
        {
            // Get build path
            string pureBuildPath = Path.GetDirectoryName(pathToBuiltProject);

            // Remove PDB files
            foreach (string file in Directory.GetFiles(pureBuildPath, "*.pdb"))
            {
                Debug.Log(file + " deleted!");
                File.Delete(file);
            }

            //// Create default mods folder
            //string modsPath = Path.Combine(pureBuildPath, defaultModsFolderName);
            //Directory.CreateDirectory(modsPath);

            //// Write readme text
            //StreamWriter stream = File.CreateText(Path.Combine(modsPath, readMeFilename));
            //stream.WriteLine(modReadMeText);
            //stream.Close();

            // Copy manual
            FileUtil.CopyFileOrDirectory(Path.Combine("Assets/Docs", manualFileName), Path.Combine(pureBuildPath, manualFileName));
        }
    }
}