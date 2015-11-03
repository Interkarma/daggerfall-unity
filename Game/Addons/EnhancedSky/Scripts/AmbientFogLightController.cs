//Enhanced Sky for Daggerfall Tools for Unity by Lypyl, contact at lypyl@dfworkshop.net
//http://www.reddit.com/r/dftfu
//http://www.dfworkshop.net/
//Author: LypyL
///Contact: Lypyl@dfworkshop.net
//License: MIT License (http://www.opensource.org/licenses/mit-license.php)


using UnityEngine;

namespace EnhancedSky
{
    public class AmbientFogLightController : MonoBehaviour
    {
        #region fields & properties
        float       _atmsphrOffset;
        LensFlare   _sunFlare;
        Color       _skyTint = Color.clear;
        Color       _groundColor = Color.clear;

        SkyManager SkyMan           { get { return SkyManager.instance;} }
        Material SkyMat             { get { return SkyManager.instance.SkyMat;}}
        LensFlare SunFlare          { get { return (_sunFlare != null) ? _sunFlare : _sunFlare = transform.GetComponentInChildren<LensFlare>(); } }
        Gradient FogGradient        { get; set; }
        Gradient HorizonGradient    { get; set; }
        AnimationCurve AtmosphereCurve { get; set; }

        
        #endregion

        #region Unity

        void OnEnable()
        {
            Init(SkyMan.IsOvercast);
            SkyManager.updateSkyEvent += this.Init;
            SkyManager.updateSkySettingsEvent += this.SetSkyObjectSize;

        }

        void OnDisable()
        {

            SkyManager.updateSkyEvent -= this.Init;
            SkyManager.updateSkySettingsEvent -= this.SetSkyObjectSize;

        }
       

        #endregion
     
        

        #region methods
        /// <summary>
        /// Selects gradients for fog & ground (horizon of procedural sky) based on isOvercast.  Disables sun flare if
        /// overcast, and sets various settings for sky before starting SetAmbient coroutine.
        /// </summary>
        /// <param name="isOverCast"></param>
        public void Init(bool isOverCast)
        {

            if(isOverCast)
            {
                FogGradient = PresetContainer.Instance.fogOver;
                HorizonGradient = PresetContainer.Instance.colorOver;
                AtmosphereCurve = PresetContainer.Instance.atmosphereOver;
                SkyMat.SetFloat("_Exposure", .3f);                              //##TODO - add to presets
                SunFlare.enabled = false;
            }
            else
            {
                FogGradient = PresetContainer.Instance.fogBase;
                HorizonGradient = PresetContainer.Instance.colorBase;
                AtmosphereCurve = PresetContainer.Instance.atmosphereBase;
                SkyMat.SetFloat("_Exposure", .5f);
            }

            SetSkyObjectSize();


            _skyTint = PresetContainer.Instance.skyTint;
            SkyMat.SetColor("_SkyTint", _skyTint);
            _atmsphrOffset = PresetContainer.Instance.atmsphrOffset;
            
        }

        void FixedUpdate()
        {
            _atmsphrOffset = PresetContainer.Instance.atmsphrOffset;
            //set fog color
            RenderSettings.fogColor = FogGradient.Evaluate(SkyMan.TimeRatio);

            //set sky Material ground color & atmo. thickness
            SkyMat.SetFloat("_AtmosphereThickness", AtmosphereCurve.Evaluate(SkyMan.TimeRatio) + _atmsphrOffset); //1
            _groundColor = HorizonGradient.Evaluate(SkyMan.TimeRatio);
            SkyMat.SetColor("_GroundColor", _groundColor);

            if (SkyMan.UseSunFlare && !(SkyMan.IsOvercast || SkyMan.IsNight))
            {
                if (!SunFlare)
                {
                    Debug.Log("ESKY: Ambient Controller couldnt find Sun Flare");
                    return;
                }
                else if (!SkyMan.IsNight)
                    SunFlare.enabled = true;
                else
                    SunFlare.enabled = false;
            }
            else
                SunFlare.enabled = false;
        }

        public void SetSkyObjectSize()
        {
           

            try
            {
                if (!SunFlare)
                {
                    Debug.Log("ESKY: Ambient Controller couldnt find Sun Flare");
                    return;
                }
                if (SkyMan.SkyObjectSizeSetting == SkyObjectSize.Normal)
                    SunFlare.brightness = PresetContainer.SUNFLARESIZENORMAL;
                else
                    SunFlare.brightness = PresetContainer.SUNFLARESIZELARGE;
            }
            catch
            {


            }



        }

        #endregion

    }
}