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
        public Color masserColor;
        public Color secundaColor;

        SkyManager skyMan;
        DaggerfallUnity dfUnity;
        DaggerfallDateTime dateTime;
        Renderer masserRend;
        Renderer secundaRend;
        Material masserMat;
        Material secundaMat;
        AnimationCurve moonAlpha;
        


        int tempCheck = 0;
        int lastCheck = 0;           //last day checked, equal to (year * 360) + dayofyear
        bool updateMoonPhase = true;
        bool isNight = true;
        
     
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
        public MoonPhases secundaPhase = MoonPhases.Full;

        // Use this for initialization
        void Start()
        {
            dfUnity = DaggerfallUnity.Instance;
            dateTime = dfUnity.WorldTime.DaggerfallDateTime;
            skyMan = SkyManager.instance;
            if (!masser)
                masser = GameObject.Find("MoonMasser");
            if (!secunda)
                secunda = GameObject.Find("MoonSecunda");
            masserRend = masser.GetComponent<Renderer>();
            secundaRend = secunda.GetComponent<Renderer>();

            if (!masserMat)
                masserMat = masserRend.material;
            if (!secundaMat)
                secundaMat = secundaRend.material;

            moonAlpha = PresetContainer.instance.moonAlphaBase;
            SkyManager.fastTravelEvent += this.Init;
            SkyManager.toggleSkyObjectsEvent += this.ToggleState;
        }

        void FixedUpdate()
        {

            //if time between moons set & before dusk, set to night - this is when moon phase will be checked
            //to avoid moons being updated while in sky
            if(skyMan.CurrentSeconds > skyMan.DuskTime || skyMan.CurrentSeconds < skyMan.DawnTime + 7200)
                isNight = true;
            else
                isNight = false;

            //if(isNight)     //update moons' alpha
            if(isNight)
            {
              
                //set masser alpha & color
                if (MasserPhase != MoonPhases.New)
                    masserMat.SetColor("_Color", new Color(masserColor.r, masserColor.g, masserColor.b, moonAlpha.Evaluate(skyMan.TimeRatio)));
                else
                    masserMat.SetColor("_Color", new Color(masserColor.r, masserColor.g, masserColor.b, 0));
                
                //set secunda alpha & color
                if (secundaPhase != MoonPhases.New)
                    secundaMat.SetColor("_Color", new Color(secundaColor.r, secundaColor.g, secundaColor.b, moonAlpha.Evaluate(skyMan.TimeRatio)));
                else
                    secundaMat.SetColor("_Color", new Color(secundaColor.r, secundaColor.g, secundaColor.b, 0));
            }
            else if((tempCheck = GetDay()) != lastCheck)    //if last day moon checked != today and it's not night, update moon phase
            {
                updateMoonPhase = true;
            }

            if (updateMoonPhase)
            {
                lastCheck = tempCheck;
                GetLunarPhase(ref MasserPhase, lastCheck, true);
                GetLunarPhase(ref secundaPhase, lastCheck, false);
                updateMoonPhase = false;
            }
        }

        void OnDestroy()
        {
            StopAllCoroutines();
            SkyManager.fastTravelEvent -= this.Init;
            SkyManager.toggleSkyObjectsEvent -= this.ToggleState;
        }


        


        /// <summary>
        /// Handles interior / exterior transition events from skyman
        /// </summary>
        public void ToggleState(bool onOff)
        {
            if (!onOff)
                this.enabled = false;
            else
            {
                this.enabled = true;
                Init(skyMan.IsOvercast);
            }
        }

        /// <summary>
        /// Handles fast travel events from skyman
        /// </summary>
        /// <param name="isOverCast"></param>
        public void Init(bool isOverCast)
        {
            if (isOverCast)
            {
                moonAlpha = PresetContainer.instance.moonAlphaOver;
            }
            else
                moonAlpha = PresetContainer.instance.moonAlphaBase;
            //
            updateMoonPhase = true;
        }

        /// <summary>
        /// dateTime.DayOfYear adds 1 to day 
        /// </summary>
        private int GetDay()
        {
            int year = dateTime.Year;
            int day = dateTime.DayOfYear;
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
                moonRend = masserRend;
            }
            else
            {
                offset = -1;
                textureLookup = secundaTextureLookup;
                moonRend = secundaRend;
            }

            day += offset;
            if (dateTime.Year < 0)
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

    }

}