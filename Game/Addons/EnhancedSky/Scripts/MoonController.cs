//Enhanced Sky for Daggerfall Tools for Unity by Lypyl, contact at lypyl@dfworkshop.net
//http://www.reddit.com/r/dftfu
//http://www.dfworkshop.net/
//Author: LypyL
///Contact: Lypyl@dfworkshop.net
//License: MIT License (http://www.opensource.org/licenses/mit-license.php)

using UnityEngine;
using System.Collections.Generic;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Utility;
using System;

namespace EnhancedSky
{
    public class MoonController : MonoBehaviour
    {
        public GameObject masser;
        public GameObject secunda;
        public GameObject masserStarMask;
        public GameObject secundaStarMask;
        public bool autoUpdatePhase = true;     //if false moon phases won't be updated

        int _tempCheck          = 0;
        int _lastCheck          = 0;           //last day checked, equal to (year * 360) + dayofyear
        bool _updateMoonPhase   = true;
        bool _isNight           = true;

        Color _masserColor      = Color.clear;
        Color _secundaColor     = Color.clear;
        Renderer _masserRend;
        Renderer _secundaRend;
        AnimationCurve _moonAlpha;

        Material MasserMat          { get {return SkyMan.MasserMat;}  }
        Material SecundaMat         { get {return SkyMan.SecundaMat;} }
        Material StarBlock          { get { return SkyMan.StarMaskMat; } }
        DaggerfallDateTime DateTime { get { return DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime; } }
        DaggerfallUnity DfUnity     { get { return DaggerfallUnity.Instance ;} }
        SkyManager SkyMan           { get { return SkyManager.instance; } }

     
        public enum MoonPhases
        {
            New,
            OneWax,
            HalfWax,
            ThreeWax,
            Full,
            ThreeWane,
            HalfWane,
            OneWane,
        }

        List<string> masserTextureLookup = new List<string>()
        {
            null,
            "masser_one_wax",
            "masser_half_wax",
            "masser_three_wax",
            "masser_full",
            "masser_three_wan",
            "masser_half_wan",
            "masser_one_wan"
        };


        List<string> secundaTextureLookup = new List<string>()
        {
            null,
            "secunda_one_wax",
            "secunda_half_wax",
            "secunda_three_wax",
            "secunda_full",
            "secunda_three_wan",
            "secunda_half_wan",
            "secunda_one_wan",
        };

        public MoonPhases MasserPhase = MoonPhases.Full;
        public MoonPhases SecundaPhase = MoonPhases.Full;


        void OnDisable()
        {
            StopAllCoroutines();
            SkyManager.updateSkyEvent -= this.Init;
            SkyManager.updateSkySettingsEvent -= this.SetSkyObjectSize;

        }


        void OnEnable()
        {
            Init(SkyMan.IsOvercast);
            SkyManager.updateSkyEvent += this.Init;
            SkyManager.updateSkySettingsEvent += this.SetSkyObjectSize;
        }


        /// <summary>
        /// Handles fast travel events from skyman
        /// </summary>
        /// <param name="isOverCast"></param>
        public void Init(bool isOverCast)
        {
            GetRefrences();
            SetSkyObjectSize();
            _updateMoonPhase = true;
        }

        void FixedUpdate()
        {

            //if time between moons set & before dusk, set to night - this is when moon phase will be checked
            //to avoid moons being updated while in sky
            if(SkyMan.CurrentSeconds > SkyMan.DuskTime || SkyMan.CurrentSeconds < SkyMan.DawnTime + 7200)
                _isNight = true;
            else
                _isNight = false;

            //if(isNight)     //update moons' alpha
            if(_isNight)
            {
                if (SkyMan.IsOvercast)
                {
                    _moonAlpha = PresetContainer.Instance.moonAlphaOver;
                }
                else
                    _moonAlpha = PresetContainer.Instance.moonAlphaBase;

                //set masser alpha & color
                if (MasserPhase != MoonPhases.New)
                     _masserRend.material.SetColor("_Color", new Color(_masserColor.r, _masserColor.g, _masserColor.b, _moonAlpha.Evaluate(SkyMan.TimeRatio)));
                else
                    _masserRend.material.SetColor("_Color", new Color(_masserColor.r, _masserColor.g, _masserColor.b, 0));
                
                //set secunda alpha & color
                if (SecundaPhase != MoonPhases.New)
                    _secundaRend.material.SetColor("_Color", new Color(_secundaColor.r, _secundaColor.g, _secundaColor.b, _moonAlpha.Evaluate(SkyMan.TimeRatio)));
                else
                    _secundaRend.material.SetColor("_Color", new Color(_secundaColor.r, _secundaColor.g, _secundaColor.b, 0));
            }
            else if((_tempCheck = GetDay()) != _lastCheck)    //if last day moon checked != today and it's not night, update moon phase
            {
                _updateMoonPhase = true;
            }

            if (_updateMoonPhase && autoUpdatePhase)
            {
                _lastCheck = _tempCheck;
                GetLunarPhase(ref MasserPhase, _lastCheck, true);
                GetLunarPhase(ref SecundaPhase, _lastCheck, false);
                _updateMoonPhase = false;
            }
        }

        
        public void SetPhase(MoonPhases masserPhase, MoonPhases secundaPhase)
        {
            if (Enum.IsDefined(typeof(MoonPhases), masserPhase))
                this.MasserPhase = masserPhase;

            if (Enum.IsDefined(typeof(MoonPhases), secundaPhase))
                this.SecundaPhase = secundaPhase;

            if(MasserPhase != MoonPhases.New)
            {
                Texture2D masserTexture = GetTexture(MasserPhase, masserTextureLookup);
                if (masserTexture != null)
                    _masserRend.material.mainTexture = masserTexture;
            }
            if(SecundaPhase != MoonPhases.New)
            {
               Texture2D secundaTexture = GetTexture(SecundaPhase, secundaTextureLookup);
               if (secundaTexture != null)
                   _secundaRend.material.mainTexture = secundaTexture;

            }
            
            
            
        }

        /// <summary>
        /// dateTime.DayOfYear adds 1 to day 
        /// </summary>
        private int GetDay()
        {
            int year = DateTime.Year;
            int day = DateTime.DayOfYear;
            //Debug.Log("Year: " + year + " day: " + day + " result: " + ((year * 360) + day - 1));
            return (year * 360) + day-1;
        }

        /// <summary>
        /// Looks up file name for moon texture and returns texture
        /// </summary>
        private Texture2D GetTexture(MoonPhases phase, List<string> textureLookup)
        {
            Texture2D moonText = null;
            if(textureLookup == null)
            {
                Debug.LogError("Invalid texture lookup dictionary");
                return moonText;
            }
            string textVal = textureLookup[(int)phase];
            if(string.IsNullOrEmpty(textVal))
            {
                Debug.LogError("Texture value is null or empty");
                return moonText;
            }
            moonText = Resources.Load(textVal) as Texture2D;

            if(moonText == null)
            {
                Debug.Log("failed to load moon texture for: " + phase.ToString());
                try
                {
                moonText = Resources.Load(textureLookup[4]) as Texture2D;             //try to load full as default
                }
                catch(Exception ex)
                {
                    Debug.LogError(ex.Message);
                }
            }

            return moonText;

        }


        /// <summary>
        /// Finds the current phase for either Masser or Secunda, and sets the correct texture using GetTexture
        /// </summary>
        private MoonPhases GetLunarPhase(ref MoonPhases moonPhase, int day, bool isMasser = true)
        {
            List<string> textureLookup = null;
            Renderer moonRend;
            int offset = 3;//3 aligns full moon with vanilla DF for Masser
            if (isMasser)   //-1 for secunda
            {
                offset = 3;
                textureLookup = masserTextureLookup;
                moonRend = _masserRend;
            }
            else
            {
                offset = -1;
                textureLookup = secundaTextureLookup;
                moonRend = _secundaRend;
            }

            day += offset;
            if (DateTime.Year < 0)
            {
                Debug.LogError("Year < 0 not supported.");
            }

            int moonRatio = day % 32;               //what moon phase the day falls in
            MoonPhases tempPhase = MoonPhases.Full;

            if (moonRatio == 0)//Full moon
            {
                tempPhase = MoonPhases.Full;
            }
            else if (moonRatio == 16)//new moon, half way
            {
                tempPhase = MoonPhases.New;
            }
            else if (moonRatio <= 5)   //three wane
            {
                tempPhase = MoonPhases.ThreeWane;
            }
            else if (moonRatio <= 10)  //half wane
            {
                tempPhase = MoonPhases.HalfWane;
            }
            else if (moonRatio <= 15) // one wane
            {
                tempPhase = MoonPhases.OneWane;
            }
            else if (moonRatio <= 22)//one wax
            {
                tempPhase = MoonPhases.OneWax;
            }
            else if (moonRatio <= 28)//half wax
            {
                tempPhase = MoonPhases.HalfWax;
            }
            else if (moonRatio <= 31)//three wax
            {
                tempPhase = MoonPhases.ThreeWax;
            }
            if (tempPhase != MoonPhases.New)
            {
                
                try
                {
                    Resources.UnloadAsset(moonRend.material.mainTexture);
                }
                catch(Exception ex)
                {
                    Debug.LogError(ex.Message + " | isMasser: " + isMasser);

                }
                Texture2D tempText = GetTexture(tempPhase, textureLookup);
                if (tempText != null)
                    moonRend.material.mainTexture = tempText;
                else
                    Debug.LogError("Failed load texture for: " + tempPhase + " isMasser: " + isMasser);
            }
            
            return (moonPhase = tempPhase);

        }
       
        private void GetRefrences()
        {
            try
            {
                if (!masser)
                    masser = transform.FindChild("MoonMasser").gameObject;
                if (!secunda)
                    secunda = transform.FindChild("MoonSecunda").gameObject;
                if (!masserStarMask)
                    masserStarMask = masser.transform.FindChild("StarBlock").gameObject;
                if (!secundaStarMask)
                    secundaStarMask = secunda.transform.FindChild("StarBlock").gameObject;
                _masserRend = masser.GetComponent<Renderer>();
                _secundaRend = secunda.GetComponent<Renderer>();
                _masserRend.material = MasserMat;
                _secundaRend.material = SecundaMat;
                masserStarMask.GetComponent<Renderer>().material = StarBlock;
                secundaStarMask.GetComponent<Renderer>().material = StarBlock;

               

                _moonAlpha = PresetContainer.Instance.moonAlphaBase;
                _masserColor = PresetContainer.Instance.MasserColor;
                _secundaColor = PresetContainer.Instance.SecundaColor;
            }
            catch(Exception ex)
            {
                Debug.LogError(ex.Message);
            }

        }

        public void SetSkyObjectSize()
        {
            if(!masser || !secunda)
                GetRefrences();

            Vector3 scale = Vector3.zero;
            if (SkyMan.SkyObjectSizeSetting == SkyObjectSize.Normal)
            {
                scale = new Vector3(PresetContainer.MOONSCALENORMAL, PresetContainer.MOONSCALENORMAL, PresetContainer.MOONSCALENORMAL);
                masser.transform.localScale = scale;
                secunda.transform.localScale = scale * .5f;
            }
            else
            {
                scale = new Vector3(PresetContainer.MOONSCALELARGE, PresetContainer.MOONSCALELARGE, PresetContainer.MOONSCALELARGE);
                masser.transform.localScale = scale;
                secunda.transform.localScale = scale * .5f;
            }



        }

    }

}