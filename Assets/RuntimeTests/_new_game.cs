using System.Collections;
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

public class _new_game
{

    [UnityTest]
    public static IEnumerator StartGameBehaviour_StartMethods_NewCharacter ()
    {
        var loadGameSceneRoutine = RuntimeTestUtilities.LoadGameScene();
        while( loadGameSceneRoutine.MoveNext() )
            yield return loadGameSceneRoutine.Current;

        GameManager.Instance.StartGameBehaviour.StartMethod = StartGameBehaviour.StartMethods.NewCharacter;

        for( int i=0 ; i<10 ; i++ )
            yield return null;
    }

}
