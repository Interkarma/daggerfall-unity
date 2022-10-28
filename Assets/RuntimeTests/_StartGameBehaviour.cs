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

public static class _StartGameBehaviour
{

    private static IEnumerator _StartMethod_Change(StartGameBehaviour.StartMethods startMethod)
    {
        yield return RuntimeTestUtilities.LoadGameSceneRoutine();

        GameManager.Instance.StartGameBehaviour.StartMethod = startMethod;

        for (int i = 0; i < 100; i++)
            yield return null;
    }

    // DoNothing,                              // No startup action
    [UnityTest]
    public static IEnumerator StartMethod_DoNothing()
        => _StartMethod_Change(StartGameBehaviour.StartMethods.DoNothing);

    // Void,                                   // Start to the Void
    [UnityTest]
    public static IEnumerator StartMethod_Void()
        => _StartMethod_Change(StartGameBehaviour.StartMethods.Void);

    // TitleMenu,                              // Open title menu
    [UnityTest]
    public static IEnumerator StartMethod_TitleMenu()
        => _StartMethod_Change(StartGameBehaviour.StartMethods.TitleMenu);

    // TitleMenuFromDeath,                     // Open title menu after death
    [UnityTest]
    public static IEnumerator StartMethod_TitleMenuFromDeath()
        => _StartMethod_Change(StartGameBehaviour.StartMethods.TitleMenuFromDeath);

    // NewCharacter,                           // Spawn character to start location in INI
    [UnityTest]
    public static IEnumerator StartMethod_NewCharacter()
        => _StartMethod_Change(StartGameBehaviour.StartMethods.NewCharacter);

    // LoadDaggerfallUnitySave,                // Make this work with new save/load system
    [UnityTest]
    public static IEnumerator StartMethod_LoadDaggerfallUnitySave ()
    {
        Debug.Log($"<color=yellow><b>{nameof(_StartGameBehaviour)}::{nameof(StartMethod_LoadDaggerfallUnitySave)}()</b>: Make sure a save exists first</color>");

        yield return RuntimeTestUtilities.LoadGameSceneRoutine();

        var startGameBehaviour = GameManager.Instance.StartGameBehaviour;
        startGameBehaviour.SaveIndex = 0;
        startGameBehaviour.StartMethod = StartGameBehaviour.StartMethods.LoadDaggerfallUnitySave;

        for( int i=0 ; i<100 ; i++ )
            yield return null;
    }

    // LoadClassicSave,                        // Loads a classic save using start save index
    [UnityTest]
    public static IEnumerator StartMethod_LoadClassicSave ()
    {
        Debug.Log($"<color=yellow><b>{nameof(_StartGameBehaviour)}::{nameof(StartMethod_LoadClassicSave)}()</b>: Make sure a classic save exists first</color>");

        yield return RuntimeTestUtilities.LoadGameSceneRoutine();

        var startGameBehaviour = GameManager.Instance.StartGameBehaviour;
        startGameBehaviour.ClassicSaveIndex = 0;
        startGameBehaviour.StartMethod = StartGameBehaviour.StartMethods.LoadClassicSave;

        for( int i=0 ; i<100 ; i++ )
            yield return null;
    }

}
