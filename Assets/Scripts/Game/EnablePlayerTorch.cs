// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
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

        DaggerfallUnity dfUnity;
        PlayerEnterExit playerEnterExit;
        PlayerEntity playerEntity;
        Light torchLight;
        float torchIntensity;
        float defaultTorchIntensity = 1.0f;
        float itemBasedTorchIntensity = 1.25f;
        float tickTimeBuffer = 0f;
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
                    {
                        torchLight.shadows = DaggerfallUnity.Settings.EnableSpellShadows ? LightShadows.Soft : LightShadows.None;
                        torchLight.transform.position = new Vector3(-0.3f, 1.2f, 0.2f);
                    }
                }
            }
        }

        void Update()
        {
            if (!dfUnity.IsReady || !playerEnterExit || !PlayerTorch || playerEntity == null || GameManager.IsGamePaused)
                return;

            bool enableTorch = false;
            float intensityMod = defaultTorchIntensity;
            if (DaggerfallUnity.Settings.PlayerTorchFromItems)
            {
                DaggerfallUnityItem lightSource = playerEntity.LightSource;
                if (lightSource != null)
                {
                    tickTimeBuffer += Time.deltaTime;
                    enableTorch = true;
                    torchLight.range = lightSource.ItemTemplate.capacityOrTarget;
                    // Consume durability / fuel
                    if (tickTimeBuffer > tickTimeInterval)
                    {
                        tickTimeBuffer = 0f;
                        if (lightSource.currentCondition > 0)
                            lightSource.currentCondition--;

                        if (lightSource.currentCondition == 0 && DaggerfallUnityItem.CompareItems(playerEntity.LightSource, lightSource))
                        {
                            DaggerfallUI.AddHUDText(TextManager.Instance.GetLocalizedText("lightDies").Replace("%it", lightSource.ItemName));
                            enableTorch = false;
                            playerEntity.LightSource = null;
                            if (!lightSource.IsOfTemplate(ItemGroups.UselessItems2, (int)UselessItems2.Lantern))
                                playerEntity.Items.RemoveItem(lightSource);
                        }
                    }
                    
                    if (lightSource.currentCondition < 3)
                    {
                        // Give warning signs if running low of fuel
                        intensityMod = 0.85f + (Mathf.Cos(guttering) * 0.2f);
                        guttering += Random.Range(-0.02f, 0.06f);
                    }
                    else
                    {
                        intensityMod = itemBasedTorchIntensity;
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