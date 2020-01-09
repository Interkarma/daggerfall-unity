// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2020 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    Hazelnut
// 
// Notes:
//

using UnityEngine;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// Enable or disable player torch.
    /// This component is used by Workshop demo scenes.
    /// </summary>
    public class EnablePlayerTorch : MonoBehaviour
    {
        public GameObject PlayerTorch;

        const string textDatabase = "DaggerfallUI";

        DaggerfallUnity dfUnity;
        PlayerEnterExit playerEnterExit;
        PlayerEntity playerEntity;
        Light torchLight;
        float torchIntensity;
        float intensityMod = 1f;
        float lastTickTime;
        float tickTimeInterval = 20f;
        float guttering = 0;

        void Start()
        {
            dfUnity = DaggerfallUnity.Instance;
            playerEnterExit = GetComponent<PlayerEnterExit>();
            playerEntity = GameManager.Instance.PlayerEntity;

            if (PlayerTorch)
            {
                torchLight = PlayerTorch.GetComponent<Light>();
                if (torchLight)
                {
                    torchIntensity = torchLight.intensity;
                    if (DaggerfallUnity.Settings.PlayerTorchFromItems)
                        torchLight.shadows = LightShadows.Soft;
                }
            }
        }

        void Update()
        {
            if (!dfUnity.IsReady || !playerEnterExit || !PlayerTorch || playerEntity == null || GameManager.IsGamePaused)
                return;

            bool enableTorch = false;
            if (DaggerfallUnity.Settings.PlayerTorchFromItems)
            {
                DaggerfallUnityItem lightSource = playerEntity.LightSource;
                if (lightSource != null)
                {
                    enableTorch = true;
                    torchLight.range = lightSource.ItemTemplate.capacityOrTarget;
                    // Consume durability / fuel
                    if (Time.realtimeSinceStartup > lastTickTime + tickTimeInterval)
                    {
                        lastTickTime = Time.realtimeSinceStartup;
                        if (lightSource.currentCondition > 0)
                            lightSource.currentCondition--;

                        if (lightSource.currentCondition == 0 && DaggerfallUnityItem.CompareItems(playerEntity.LightSource, lightSource))
                        {
                            DaggerfallUI.MessageBox(TextManager.Instance.GetText(textDatabase, "lightDies"), false, lightSource);
                            enableTorch = false;
                            playerEntity.LightSource = null;
                            if (!lightSource.IsOfTemplate(ItemGroups.UselessItems2, (int)UselessItems2.Lantern))
                                playerEntity.Items.RemoveItem(lightSource);
                        }
                    }
                    
                    if (lightSource.currentCondition < 3)
                    {
                        // Give warning signs if running low of fuel
                        intensityMod = 0.6f + (Mathf.Cos(guttering) * 0.2f);
                        guttering += Random.Range(-0.02f, 0.06f);
                    }
                    else
                    {
                        intensityMod = 1;
                        guttering = 0;
                    }
                }
            }
            else
            {
                enableTorch = (!playerEnterExit.IsPlayerInside && dfUnity.WorldTime.Now.IsCityLightsOn) || playerEnterExit.IsPlayerInsideDungeon;
            }
            if (torchLight)
                torchLight.intensity = torchIntensity * DaggerfallUnity.Settings.PlayerTorchLightScale * intensityMod;

            PlayerTorch.SetActive(enableTorch);
        }
    }
}