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

public class RuntimeTestUtilities
{

    public static IEnumerator LoadGameScene ()
    {
        Scene startupScene = SceneManager.LoadScene( SceneControl.StartupSceneIndex , new LoadSceneParameters(LoadSceneMode.Single,LocalPhysicsMode.None) );
        Debug.LogWarning("loading Startup Scene");
        while( !startupScene.isLoaded ) yield return null;

        Scene gameScene = SceneManager.LoadScene( SceneControl.GameSceneIndex , new LoadSceneParameters(LoadSceneMode.Single,LocalPhysicsMode.Physics3D) );
        Debug.LogWarning("loading Game Scene");
        while( !gameScene.isLoaded ) yield return null;
    }

}
