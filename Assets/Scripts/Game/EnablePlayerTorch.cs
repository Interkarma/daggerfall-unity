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
using System.Collections;

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// Enable or disable player torch.
    /// This component is used by Workshop demo scenes.
    /// </summary>
    public class EnablePlayerTorch : MonoBehaviour
    {
        public GameObject PlayerTorch;

        DaggerfallUnity dfUnity;
        PlayerEnterExit playerEnterExit;

        void Start()
        {
            dfUnity = DaggerfallUnity.Instance;
            playerEnterExit = GetComponent<PlayerEnterExit>();
        }

        void Update()
        {
            if (!dfUnity.IsReady || !playerEnterExit || !PlayerTorch)
                return;

            bool enableTorch = false;
            if (!playerEnterExit.IsPlayerInside && dfUnity.WorldTime.Now.IsCityLightsOn)
                enableTorch = true;
            if (playerEnterExit.IsPlayerInsideDungeon && !playerEnterExit.IsPlayerInsideDungeonCastle)
                enableTorch = true;

            PlayerTorch.SetActive(enableTorch);
        }
    }
}