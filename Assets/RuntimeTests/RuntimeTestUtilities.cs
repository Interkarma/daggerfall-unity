// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Andrzej Łukasik (andrew.r.lukasik)
// Contributors:    
// 
// Notes:
//

using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using Unity.Profiling;

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

using Stopwatch = System.Diagnostics.Stopwatch;

public class RuntimeTestUtilities
{

    public static IEnumerator LoadGameSceneRoutine()
    {
        var ___pm = new ProfilerMarker($"{nameof(RuntimeTestUtilities)}::{nameof(LoadGameSceneRoutine)}()");
        ___pm.Begin();

        // load startup scene
        Debug.Log("<color=green>loading <b>Startup Scene</b></color>");
        Scene startupScene = SceneManager.LoadScene(SceneControl.StartupSceneIndex, new LoadSceneParameters(LoadSceneMode.Single, LocalPhysicsMode.None));
        while (!startupScene.isLoaded)
        {
            ___pm.End();
            yield return null;
            ___pm.Begin();
        }

        // load game scene
        Debug.Log("<color=green>loading <b>Game Scene</b></color>");
        Scene gameScene = SceneManager.LoadScene(SceneControl.GameSceneIndex, new LoadSceneParameters(LoadSceneMode.Single, LocalPhysicsMode.Physics3D));
        while (!gameScene.isLoaded)
        {
            ___pm.End();
            yield return null;
            ___pm.Begin();
        }

        // trigger immediate start method
        GameManager.Instance.StartGameBehaviour.StartMethod = StartGameBehaviour.StartMethods.Void;

        // hide intro video and main menu ui
        yield return null;
        yield return null;
        DaggerfallUI.Instance.PopToHUD();
        // I don't know why, but this formula works. But this means it can break in the future - pls report if that happens

        ___pm.End();
    }

    /// <returns> hasLocation </returns>
    public static bool ReadLocation(
        int x, int y,
        out DFLocation location,
        bool throwWhenNoLocation = true
    )
    {
        var ___pm = new ProfilerMarker($"{nameof(RuntimeTestUtilities)}::{nameof(ReadLocation)}({x},{y},...)");
        ___pm.Begin();

        var contentReader = DaggerfallUnity.Instance.ContentReader;
        if (contentReader.HasLocation(x, y, out var mapSummary))
        {
            var watch = Stopwatch.StartNew();
            bool hasLocation = contentReader.GetLocation(mapSummary.RegionIndex, mapSummary.MapIndex, out location);
            watch.Stop();

            {
                var message = new System.Text.StringBuilder($"<b>{nameof(RuntimeTestUtilities)}.{nameof(ReadLocation)}({x},{y},...)</b>");
                double seconds = watch.Elapsed.TotalSeconds;
                message.Append(seconds > 1 ? $" took {seconds:0.00}s" : $" took {watch.ElapsedMilliseconds}ms");
                Debug.Log($"<color=green>{message}</color>");
            }

            ___pm.End();
            return hasLocation;
        }
        else if (throwWhenNoLocation)
        {
            ___pm.End();
            throw new System.Exception($"<color=red>coordinate [{x},{y}] has no location</color>");
        }
        else
        {
            ___pm.End();
            location = default;
            return false;
        }
    }

    public static void MoveToExteriorLocation(
        int x, int y,
        DFLocation location
    )
    {
        var ___pm = new ProfilerMarker($"{nameof(RuntimeTestUtilities)}::{nameof(MoveToExteriorLocation)}({x},{y},...)");
        ___pm.Begin();

        PlayerEnterExit playerEnterExit = GameManager.Instance.PlayerEnterExit;
        StreamingWorld streamingWorld = GameManager.Instance.StreamingWorld;
        
        playerEnterExit.EnableExteriorParent();
        streamingWorld.TeleportToCoordinates(x, y);
        streamingWorld.SetAutoReposition(StreamingWorld.RepositionMethods.Origin, Vector3.zero);
        streamingWorld.suppressWorld = false;

        ___pm.End();
    }

    public static void MoveToDungeonLocation(
        int x, int y,
        DFLocation location,
        bool throwWhenNoDungeon = true
    )
    {
        Assert.IsTrue(location.HasDungeon, $"location has no dungeon: {location.Name} ({location.RegionName}) [{x},{y}]");

        var ___pm = new ProfilerMarker($"{nameof(RuntimeTestUtilities)}::{nameof(MoveToDungeonLocation)}({x},{y},...)");
        ___pm.Begin();

        PlayerEnterExit playerEnterExit = GameManager.Instance.PlayerEnterExit;
        StreamingWorld streamingWorld = GameManager.Instance.StreamingWorld;

        streamingWorld.TeleportToCoordinates(x, y);
        streamingWorld.suppressWorld = true;
        playerEnterExit.EnableDungeonParent();
        playerEnterExit.StartDungeonInterior(location);

        // playerEnterExit.DisableAllParents(cleanup: true);

        ___pm.End();
    }

    public static void LoadExteriorLocation(
        int x, int y
    )
    {
        var ___pm = new ProfilerMarker($"{nameof(RuntimeTestUtilities)}::{nameof(LoadExteriorLocation)}({x},{y})");
        ___pm.Begin();

        Debug.Log($"<color=green><b>{nameof(RuntimeTestUtilities)}.{nameof(LoadExteriorLocation)}({x},{y})</b></color>");

        var watch = Stopwatch.StartNew();
        ReadLocation(x, y, out DFLocation location);
        MoveToExteriorLocation(x, y, location);
        watch.Stop();

        Debug.Log($"<color=green>\t loaded: {location.Name} ({location.RegionName})</color>");

        {
            var message = new System.Text.StringBuilder($"<b>{nameof(RuntimeTestUtilities)}.{nameof(LoadExteriorLocation)}({x},{y})</b> {location.Name}@{location.RegionName}");
            double seconds = watch.Elapsed.TotalSeconds;
            message.Append(seconds > 1 ? $" took {seconds:0.00}s" : $" took {watch.ElapsedMilliseconds}ms");
            Debug.Log($"<color=green>{message}</color>");
        }

        ___pm.End();
    }

    public static void LoadDungeonLocation(
        int x, int y
    )
    {
        var ___pm = new ProfilerMarker($"{nameof(RuntimeTestUtilities)}::{nameof(LoadDungeonLocation)}({x},{y})");
        ___pm.Begin();

        Debug.Log($"<color=green><b>{nameof(RuntimeTestUtilities)}.{nameof(LoadDungeonLocation)}({x},{y})</b></color>");

        var watch = Stopwatch.StartNew();
        ReadLocation(x, y, out DFLocation location);
        MoveToDungeonLocation(x, y, location);
        watch.Stop();

        Debug.Log($"<color=green>\t loaded: {location.Name} ({location.RegionName})</color>");

        {
            var message = new System.Text.StringBuilder($"<b>{nameof(RuntimeTestUtilities)}.{nameof(LoadDungeonLocation)}({x},{y})</b> {location.Name}@{location.RegionName}");
            double seconds = watch.Elapsed.TotalSeconds;
            message.Append(seconds > 1 ? $" took {seconds:0.00}s" : $" took {watch.ElapsedMilliseconds}ms");
            Debug.Log($"<color=green>{message}</color>");
        }

        ___pm.End();
    }

}
