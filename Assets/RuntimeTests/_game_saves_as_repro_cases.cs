using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Player;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Game.Questing;
using DaggerfallConnect;
using DaggerfallConnect.Utility;
using DaggerfallConnect.Save;
using DaggerfallWorkshop.Game.Banking;
using DaggerfallConnect.Arena2;

public static class _game_saves_as_repro_cases
{

    // copied from SaveLoadManager.EnumerateSaveFolders() because it was private
    static Dictionary<int, string> _EnumerateSaveFolders()
    {
        string UnitySavePath = SaveLoadManager.Instance.UnitySavePath;
        string savePrefix = "SAVE";
        string saveInfoFilename = "SaveInfo.txt";

        // Get directories in save path matching prefix
        string[] directories = Directory.GetDirectories(UnitySavePath, savePrefix + "*", SearchOption.TopDirectoryOnly);

        // Build dictionary keyed by save index
        var saveFolders = new Dictionary<int, string>();
        foreach (string directory in directories)
        {
            // Get everything right of prefix in folder name (should be a number)
            int key;
            string indexStr = Path.GetFileName(directory).Substring(savePrefix.Length);
            if (int.TryParse(indexStr, out key))
            {
                // Must contain a save info file to be a valid save folder
                if (File.Exists(Path.Combine(directory, saveInfoFilename)))
                    saveFolders.Add(key, directory);
            }
        }

        return saveFolders;
    }

    [UnityTest]
    public static IEnumerator load_every_save()
    {
        yield return RuntimeTestUtilities.LoadGameSceneRoutine();

        RuntimeTestUtilities.HideMainMenuUi();

        SaveLoadManager.Instance.EnumerateSaves();
        foreach (var next in _EnumerateSaveFolders())
        {
            Debug.Log($"<color=green><b>{nameof(_game_saves_as_repro_cases)}::{nameof(load_every_save)}()</b>: loading \'{next.Value}\' save</color>");

            SaveLoadManager.Instance.Load(next.Key);

            for (int i = 0; i < 100; i++)
                yield return null;
        }
    }

    // [UnityTest]
    // public static IEnumerator load_every_classic_save()
    // {
    //     yield return RuntimeTestUtilities.LoadGameSceneRoutine();

    //     var startGameBehaviour = GameManager.Instance.StartGameBehaviour;
    //     startGameBehaviour.ClassicSaveIndex = 0;
    //     startGameBehaviour.StartMethod = StartGameBehaviour.StartMethods.LoadClassicSave;

    //     for (int i = 0; i < 100; i++)
    //         yield return null;
    // }

}
