// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

using UnityEngine;
using System.Collections;

namespace DaggerfallWorkshop.Demo
{
    /// <summary>
    /// Peer this with Player and PlayerEnterExit to change ambient light based on player surroundings.
    /// For example, Daggerfall scales ambient light up and down in dungeons with palace blocks (e.g. Wayrest).
    /// Ambient light is dimmed when player leaves palace block and brightened on return.
    /// </summary>
    public class PlayerAmbientLight : MonoBehaviour
    {
        public Color ExteriorAmbientLight = new Color(0.12f, 0.12f, 0.12f);
        public Color InteriorAmbientLight = new Color(0.18f, 0.18f, 0.18f);
        public Color DungeonAmbientLight = new Color(0.12f, 0.12f, 0.12f);
        public Color PalaceAmbientLight = new Color(0.58f, 0.58f, 0.58f);
        public float FadeDuration = 3f;
        public float FadeStep = 0.1f;

        PlayerEnterExit playerEnterExit;
        Color targetAmbientLight;
        bool fadeRunning;

        void Start()
        {
            playerEnterExit = GetComponent<PlayerEnterExit>();
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
                    targetAmbientLight = ExteriorAmbientLight;
                }
                else if (playerEnterExit.IsPlayerInside && !playerEnterExit.IsPlayerInsideDungeon)
                {
                    targetAmbientLight = InteriorAmbientLight;
                }
                else if (playerEnterExit.IsPlayerInside && playerEnterExit.IsPlayerInsideDungeon)
                {
                    if (playerEnterExit.IsPlayerInsideDungeonPalace)
                        targetAmbientLight = PalaceAmbientLight;
                    else
                        targetAmbientLight = DungeonAmbientLight;
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
    }
}