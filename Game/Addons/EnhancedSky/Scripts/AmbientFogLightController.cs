//Enhanced Sky for Daggerfall Tools for Unity by Lypyl, contact at lypyl@dfworkshop.net
//http://www.reddit.com/r/dftfu
//http://www.dfworkshop.net/
//Author: LypyL
///Contact: Lypyl@dfworkshop.net
//License: MIT License (http://www.opensource.org/licenses/mit-license.php)


using UnityEngine;
using System.Collections;

namespace EnhancedSky
{
    public class AmbientFogLightController : MonoBehaviour
    {
        #region fields
        public SkyManager skyMan;
        Gradient fogGradient;
        Gradient groundGradient;
        AnimationCurve atmosphereCurve;
        Material skyMat;
        ///LensFlare sunFlare;

        float atmsphrOffset;
        Color skyTint;// = new Color(128, 128, 128);
        Color groundColor;// = new Color(132, 166, 195);
        #endregion

        #region Unity
        
        // Use this for initialization
        void Start()
        {
            skyMan = SkyManager.instance;

            //if (!sunFlare)
              //  sunFlare = GameObject.Find("Sun").GetComponent<LensFlare>();

            if (!skyMat)
                skyMat = skyMan.skyMat;
            SkyManager.fastTravelEvent += this.Init;
            SkyManager.toggleSkyObjectsEvent += this.ToggleState;
            Init(skyMan.IsOvercast);
        }

        void OnDestroy()
        {
            StopAllCoroutines();
            SkyManager.fastTravelEvent -= this.Init;
            SkyManager.toggleSkyObjectsEvent -= this.ToggleState;

        }

        #endregion

        public void ToggleState(bool onOff)
        {
            this.enabled = onOff;

            if (!onOff)
                StopAllCoroutines();
            else
            {
                this.enabled = onOff;
                Init(skyMan.IsOvercast);
            }
        }



        #region methods
        /// <summary>
        /// Selects gradients for fog & ground (horizon of procedural sky) based on isOvercast.  Disables sun flare if
        /// overcast, and sets various settings for sky before starting SetAmbient coroutine.
        /// </summary>
        /// <param name="isOverCast"></param>
        public void Init(bool isOverCast)
        {
            StopAllCoroutines();

            if(isOverCast)
            {
                fogGradient = PresetContainer.instance.fogOver;
                groundGradient = PresetContainer.instance.colorOver;
                atmosphereCurve = PresetContainer.instance.atmosphereOver;
                skyMat.SetFloat("_Exposure", .2f);
                //sunFlare.enabled = false;
            }
            else
            {
                fogGradient = PresetContainer.instance.fogBase;
                groundGradient = PresetContainer.instance.colorBase;
                atmosphereCurve = PresetContainer.instance.atmosphereBase;
                skyMat.SetFloat("_Exposure", .5f);
                
            }

            skyTint = PresetContainer.instance.skyTint;
            skyMat.SetColor("_SkyTint", skyTint);
            atmsphrOffset = PresetContainer.instance.atmsphrOffset;
            StartCoroutine(SetAmbient());

        }

        /// <summary>
        /// Controls some ambient sky settings.
        /// </summary>
        IEnumerator SetAmbient()
        {
            while (true)
            {
                atmsphrOffset = PresetContainer.instance.atmsphrOffset;
                //set fog color
                RenderSettings.fogColor = fogGradient.Evaluate(skyMan.TimeRatio);
     
                //set sky Material ground color & atmo. thickness
                skyMat.SetFloat("_AtmosphereThickness", atmosphereCurve.Evaluate(skyMan.TimeRatio) + atmsphrOffset); //1
                groundColor = groundGradient.Evaluate(skyMan.TimeRatio);
                skyMat.SetColor("_GroundColor", groundColor);
                
                
               // if (skyMan.UseSunFlare && !(skyMan.IsOvercast || skyMan.IsNight))
                //    sunFlare.enabled = true;
                //else
                 //   sunFlare.enabled = false;
                
                
                yield return new WaitForEndOfFrame();
            }

        }
        #endregion

    }
}