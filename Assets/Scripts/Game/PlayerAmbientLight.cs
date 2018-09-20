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
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// Peer this with Player and PlayerEnterExit to change ambient light based on player surroundings.
    /// For example, Daggerfall scales ambient light up and down in dungeons with castle blocks (e.g. Wayrest).
    /// Ambient light is dimmed when player leaves castle block and brightened on return.
    /// </summary>
    public class PlayerAmbientLight : MonoBehaviour
    {
        public Color ExteriorNoonAmbientLight = new Color(0.9f, 0.9f, 0.9f);
        public Color ExteriorNightAmbientLight = new Color(0.25f, 0.25f, 0.25f);
        public Color InteriorAmbientLight = new Color(0.18f, 0.18f, 0.18f);
        public Color InteriorNightAmbientLight = new Color(0.20f, 0.18f, 0.20f);
        public Color DungeonAmbientLight = new Color(0.12f, 0.12f, 0.12f);
        public Color CastleAmbientLight = new Color(0.58f, 0.58f, 0.58f);
        public Color SpecialAreaLight = new Color(0.58f, 0.58f, 0.58f);
        public float FadeDuration = 3f;
        public float FadeStep = 0.1f;

        PlayerEnterExit playerEnterExit;
        SunlightManager sunlightManager;
        Color targetAmbientLight;
        bool fadeRunning;

        void Start()
        {
            playerEnterExit = GetComponent<PlayerEnterExit>();
            sunlightManager = GameManager.Instance.SunlightManager;
            StartCoroutine(ManageAmbientLight());
        }

        void Update()
        {
            if (UnityEngine.RenderSettings.ambientLight != targetAmbientLight && !fadeRunning)
                StartCoroutine(ChangeAmbientLight());
        }

        // Polls PlayerEnterExit a few times each second to detect if player environment has changed
        IEnumerator ManageAmbientLight()
        {
            const float pollSpeed = 1f / 3f;

            while (playerEnterExit)
            {
                if (!playerEnterExit.IsPlayerInside && !playerEnterExit.IsPlayerInsideDungeon)
                {
                    targetAmbientLight = CalcDaytimeAmbientLight();
                }
                else if (playerEnterExit.IsPlayerInside && !playerEnterExit.IsPlayerInsideDungeon)
                {
                    if (DaggerfallUnity.Instance.WorldTime.Now.IsNight)
                        targetAmbientLight = InteriorNightAmbientLight;
                    else
                        targetAmbientLight = InteriorAmbientLight;
                }
                else if (playerEnterExit.IsPlayerInside && playerEnterExit.IsPlayerInsideDungeon)
                {
                    if (playerEnterExit.IsPlayerInsideDungeonCastle)
                        targetAmbientLight = CastleAmbientLight;
                    else if (playerEnterExit.IsPlayerInsideSpecialArea)
                        targetAmbientLight = SpecialAreaLight;
                    else
                        targetAmbientLight = DungeonAmbientLight * DaggerfallUnity.Settings.DungeonAmbientLightScale;
                }

                yield return new WaitForSeconds(pollSpeed);
            }
        }

        IEnumerator ChangeAmbientLight()
        {
            fadeRunning = true;

            float progress = 0;
            float increment = FadeStep / FadeDuration;
            Color startColor = UnityEngine.RenderSettings.ambientLight;
            while (progress < 1)
            {
                UnityEngine.RenderSettings.ambientLight = Color.Lerp(startColor, targetAmbientLight, progress);
                progress += increment;
                yield return new WaitForSeconds(FadeStep);
            }

            UnityEngine.RenderSettings.ambientLight = targetAmbientLight;
            fadeRunning = false;
        }

        Color CalcDaytimeAmbientLight()
        {
            float scale = sunlightManager.DaylightScale * sunlightManager.ScaleFactor;

            Color startColor = ExteriorNightAmbientLight;
            if (DaggerfallUnity.Instance.WorldTime.Now.IsNight)
                startColor *= DaggerfallUnity.Settings.NightAmbientLightScale;

            return Color.Lerp(startColor, ExteriorNoonAmbientLight, scale);
        }
    }
}