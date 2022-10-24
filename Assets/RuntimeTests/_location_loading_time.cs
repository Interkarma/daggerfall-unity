using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

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

public class _location_loading_time
{

    [UnityTest]
    public static IEnumerator tutorial_dungeon_loads_under_100_ms()
    {
        yield return RuntimeTestUtilities.LoadGameSceneRoutine();

        var watch = Stopwatch.StartNew();
        {
            RuntimeTestUtilities.LoadDungeonLocation(109, 158);
        }
        watch.Stop();

        int ms = watch.Elapsed.Milliseconds;
        Assert.Less(ms, 100, $"it took {ms} ms");

        for (int i = 0; i < 100; i++)
            yield return null;
    }

    [UnityTest]
    public static IEnumerator daggerfall_city_loads_under_100_ms()
    {
        yield return RuntimeTestUtilities.LoadGameSceneRoutine();

        var watch = Stopwatch.StartNew();
        {
            RuntimeTestUtilities.LoadExteriorLocation(207, 213);
        }
        watch.Stop();

        int ms = watch.Elapsed.Milliseconds;
        Assert.Less(ms, 100, $"it took {ms} ms");

        for (int i = 0; i < 100; i++)
            yield return null;
    }

}
