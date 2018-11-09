// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using DaggerfallWorkshop.Game;

/// <summary>
/// Changes camera clear setting if player is in interior or exterior.
/// </summary>
public class CameraClearManager : MonoBehaviour
{
    public Camera mainCamera;
    public PlayerEnterExit playerEnterExit;
    public CameraClearFlags cameraClearExterior = CameraClearFlags.Depth;
    public CameraClearFlags cameraClearInterior = CameraClearFlags.Color;
    public Color cameraClearColor = Color.black;

    bool lastInside = false;

    void Start()
    {
        if (playerEnterExit == null)
            playerEnterExit = GameManager.Instance.PlayerEnterExit;
        if (mainCamera == null)
            mainCamera = GameManager.Instance.MainCamera;
    }

    void Update()
    {
        if (playerEnterExit && mainCamera)
        {
            bool isInside = playerEnterExit.IsPlayerInside;
            if (isInside && !lastInside)
            {
                // Now inside
                mainCamera.clearFlags = cameraClearInterior;
                mainCamera.backgroundColor = cameraClearColor;
                lastInside = isInside;
            }
            else if (!isInside && lastInside)
            {
                // Now outside
                mainCamera.clearFlags = cameraClearExterior;
                lastInside = isInside;
            }   
        }
    }
}
