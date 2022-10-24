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

public class _travel
{

    [UnityTest]
    [Timeout(int.MaxValue)]
    public static IEnumerator visits_50_locations()
    {
        yield return RuntimeTestUtilities.LoadGameSceneRoutine();

        int counter = 0;
        var rnd = new System.Random(934527);
        while (counter < 50)
        {
            int x = rnd.Next(2, 997);
            int y = rnd.Next(2, 497);

            // Get start parameters
            DFPosition mapPixel = new DFPosition(x, y);

            // Read location if any
            bool hasLocation = RuntimeTestUtilities.ReadLocation(x, y, out DFLocation location, throwWhenNoLocation: false);
            if( hasLocation )
            {
                counter++;

                RuntimeTestUtilities.MoveToExteriorLocation(x, y, location);

                for (int i = 0; i < 10; i++)
                    yield return null;
            }
            else
            {
                // skip
            }
        }
    }

    [UnityTest]
    [Timeout(int.MaxValue)]
    public static IEnumerator visits_50_dungeons()
    {
        yield return RuntimeTestUtilities.LoadGameSceneRoutine();

        PlayerEnterExit playerEnterExit = GameManager.Instance.PlayerEnterExit;
        StreamingWorld streamingWorld = GameManager.Instance.StreamingWorld;
        {
            // disable a starting dungeon
            playerEnterExit.DisableAllParents(cleanup: true);
        }

        int counter = 0;
        var rnd = new System.Random(12345);
        while (counter < 50)
        {
            int x = rnd.Next(2, 997);
            int y = rnd.Next(2, 497);

            bool hasLocation = RuntimeTestUtilities.ReadLocation(x, y, out DFLocation location, throwWhenNoLocation: false);
            if (hasLocation && location.HasDungeon)
            {
                counter++;

                RuntimeTestUtilities.MoveToDungeonLocation(x, y, location);
                
                for (int i = 0; i < 10; i++)
                    yield return null;
            }
            else
            {
                // skip
            }
        }
    }

}
