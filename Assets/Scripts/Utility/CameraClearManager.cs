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
using UnityStandardAssets.ImageEffects;

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
    //public bool toggleGlobalFog = true;

    GlobalFog globalFog;
    bool lastInside = false;

    void Start()
    {
        if (playerEnterExit == null)
            playerEnterExit = GameManager.Instance.PlayerEnterExit;
        if (mainCamera == null)
            mainCamera = GameManager.Instance.MainCamera;

        if (mainCamera != null)
        {
            globalFog = mainCamera.GetComponent<GlobalFog>();
        }
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
                //Debug.Log("Camera clear set to inside");

                // Comment this out, We're not Unity 5.5 anymore - MeteoricDragon
                // Disable global fog inside
                // This fixes an issue with solid camera clear not working in Unity 5.5
                //if (toggleGlobalFog && globalFog)
                //{
                //    globalFog.enabled = false;
                //}

                lastInside = isInside;
            }
            else if (!isInside && lastInside)
            {
                // Now outside
                mainCamera.clearFlags = cameraClearExterior;
                //Debug.Log("Camera clear set to outside");

                // Enable global fog outside
                //if (toggleGlobalFog && globalFog)
                //{
                //    globalFog.enabled = true;
                //}

                lastInside = isInside;
            }   
        }
    }
}
