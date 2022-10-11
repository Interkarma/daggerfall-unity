using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallConnect.Utility;
using DaggerfallConnect.Save;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Player;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Game.Questing;
using DaggerfallConnect;
using DaggerfallWorkshop.Game.Banking;
using DaggerfallConnect.Arena2;

public class _travel
{
    
    [UnityTest]
    [Timeout( int.MaxValue )]
    public static IEnumerator visits_50_locations ()
    {
        // start routine:
        var startRoutine = _new_game.StartGameBehaviour_StartMethods_NewCharacter();
        while( startRoutine.MoveNext() )
            yield return startRoutine.Current;

        PlayerEnterExit playerEnterExit = GameManager.Instance.PlayerEnterExit;
        StreamingWorld streamingWorld = GameObject.FindObjectOfType<StreamingWorld>();
        int counter = 0;
        var rnd = new System.Random( 934527 );
        while( counter<50 )
        {
            int x = rnd.Next( 2 , 997 );
            int y = rnd.Next( 2 , 497 );

            // Get start parameters
            DFPosition mapPixel = new DFPosition( x , y );
            
            // Read location if any
            DFLocation location = default;
            bool hasLocation = DaggerfallUnity.Instance.ContentReader.HasLocation(mapPixel.X, mapPixel.Y, out var mapSummary);
            if (hasLocation)
            {
                if (!DaggerfallUnity.Instance.ContentReader.GetLocation(mapSummary.RegionIndex, mapSummary.MapIndex, out location))
                    hasLocation = false;
            }

            // Start at specified location
            playerEnterExit.EnableExteriorParent();
            if (streamingWorld)
            {
                counter++;
                Debug.LogWarning($"loading location: {location.Name} ({location.RegionName}) [{x},{y}]");

                streamingWorld.TeleportToCoordinates(mapPixel.X, mapPixel.Y);
                streamingWorld.SetAutoReposition(StreamingWorld.RepositionMethods.Origin, Vector3.zero);
                streamingWorld.suppressWorld = false;
            }

            for( int i=0 ; i<10 ; i++ )
                yield return null;
        }
    }

    [UnityTest]
    [Timeout( int.MaxValue )]
    public static IEnumerator visits_50_dungeons ()
    {
        // start routine:
        var startRoutine = _new_game.StartGameBehaviour_StartMethods_NewCharacter();
        while( startRoutine.MoveNext() )
            yield return startRoutine.Current;
        
        PlayerEnterExit playerEnterExit = GameManager.Instance.PlayerEnterExit;
        StreamingWorld streamingWorld = GameObject.FindObjectOfType<StreamingWorld>();
        
        // disable starting dungeon:
        playerEnterExit.DisableAllParents(cleanup: true);

        int counter = 0;
        var rnd = new System.Random( 12345 );
        while( counter<50 )
        {
            int x = rnd.Next( 2 , 997 );
            int y = rnd.Next( 2 , 497 );

            // Get start parameters
            DFPosition mapPixel = new DFPosition( x , y );
            
            // Read location if any
            DFLocation location = default;
            bool hasLocation = DaggerfallUnity.Instance.ContentReader.HasLocation(mapPixel.X, mapPixel.Y, out var mapSummary);
            if (hasLocation)
            {
                if (!DaggerfallUnity.Instance.ContentReader.GetLocation(mapSummary.RegionIndex, mapSummary.MapIndex, out location))
                    hasLocation = false;
            }

            // Start at specified location
            if (hasLocation && location.HasDungeon)
            {
                counter++;
                Debug.LogWarning($"loading dungeon: {location.Name} ({location.RegionName}) [{x},{y}]");

                if (streamingWorld)
                {
                    streamingWorld.TeleportToCoordinates(mapPixel.X, mapPixel.Y);
                    streamingWorld.suppressWorld = true;
                }
                playerEnterExit.EnableDungeonParent();
                playerEnterExit.StartDungeonInterior(location);

                // for( int i=0 ; i<10 ; i++ )
                    yield return null;
                
                playerEnterExit.DisableAllParents(cleanup: true);
            }
            else
            {
                // skip
            }
        }
    }

}
