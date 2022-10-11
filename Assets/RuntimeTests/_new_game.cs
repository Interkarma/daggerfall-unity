using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

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

public class _new_game
{

    [UnityTest]
    public static IEnumerator StartGameBehaviour_StartMethods_NewCharacter ()
    {
        SceneManager.LoadScene( SceneControl.StartupSceneIndex );
        yield return null;
        SceneManager.LoadScene( SceneControl.GameSceneIndex );
        yield return null;

        GameManager.Instance.StartGameBehaviour.StartMethod = StartGameBehaviour.StartMethods.NewCharacter;

        for( int i=0 ; i<10 ; i++ )
            yield return null;
    }

}
