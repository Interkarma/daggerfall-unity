// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2023 Daggerfall Workshop
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
        public Color InteriorAmbientLight_AmbientOnly = new Color(0.8f, 0.8f, 0.8f);
        public Color InteriorNightAmbientLight_AmbientOnly = new Color(0.5f, 0.5f, 0.5f);
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
            PlayerEnterExit.OnTransitionExterior += OnTransitionToExterior;
            playerEnterExit = GetComponent<PlayerEnterExit>();
            sunlightManager = GameManager.Instance.SunlightManager;
        }
        
        private void OnTransitionToExterior(PlayerEnterExit.TransitionEventArgs args)
        {
            sunlightManager.Update(); // Ensure that SunlightManager is in a ready state
            UpdateAmbientLight();
        }

        void Update()
        {
            if (UnityEngine.RenderSettings.ambientLight != targetAmbientLight && !fadeRunning)
                StartCoroutine(ChangeAmbientLight());
        }
        
        void LateUpdate()
        {
            UpdateAmbientLight(); // Would be better to call this just prior to world frame rendering
        }

        public void UpdateAmbientLight()
        {
            if (!playerEnterExit)
                return;

            if (!playerEnterExit.IsPlayerInside && !playerEnterExit.IsPlayerInsideDungeon)
            {
                targetAmbientLight = CalcDaytimeAmbientLight();
            }
            else if (playerEnterExit.IsPlayerInside && !playerEnterExit.IsPlayerInsideDungeon)
            {
                if (DaggerfallUnity.Instance.WorldTime.Now.IsNight)
                    targetAmbientLight = (DaggerfallUnity.Settings.AmbientLitInteriors) ? InteriorNightAmbientLight_AmbientOnly : InteriorNightAmbientLight;
                else
                    targetAmbientLight = (DaggerfallUnity.Settings.AmbientLitInteriors) ? InteriorAmbientLight_AmbientOnly : InteriorAmbientLight;
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
        }

        IEnumerator ChangeAmbientLight()
        {
            if (!playerEnterExit.IsPlayerInsideDungeon)
            {
                // Do not smoothly change ambient light outside of dungeons
                fadeRunning = false;
                RenderSettings.ambientLight = targetAmbientLight;
                yield break;
            }
            else
            {
                // Smoothly lerp ambient light inside dungeons when target ambient level changes
                fadeRunning = true;
                float progress = 0;
                float increment = FadeStep / FadeDuration;
                Color startColor = RenderSettings.ambientLight;
                while (progress < 1)
                {
                    RenderSettings.ambientLight = Color.Lerp(startColor, targetAmbientLight, progress);
                    progress += increment;
                    yield return new WaitForSeconds(FadeStep);
                }
                RenderSettings.ambientLight = targetAmbientLight;
                fadeRunning = false;
            }
        }

        Color CalcDaytimeAmbientLight()
        {
            float scale = sunlightManager.DaylightScale * sunlightManager.ScaleFactor;
            Color startColor = ExteriorNightAmbientLight * DaggerfallUnity.Settings.NightAmbientLightScale;
            return Color.Lerp(startColor, ExteriorNoonAmbientLight, scale);
        }
    }
}
