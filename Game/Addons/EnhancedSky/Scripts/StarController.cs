//Enhanced Sky for Daggerfall Tools for Unity by Lypyl, contact at lypyl@dfworkshop.net
//http://www.reddit.com/r/dftfu
//http://www.dfworkshop.net/
//Author: LypyL
///Contact: Lypyl@dfworkshop.net
//License: MIT License (http://www.opensource.org/licenses/mit-license.php)


using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace EnhancedSky
{
    public class StarController : MonoBehaviour
    {
        SkyManager skyMan;
        Gradient starFade;
        Renderer starRend;
        Color lastColor;
        Material starMat;

        bool setColors = true;
     
        // Use this for initialization
        void Start()
        {
            //event subscriptions
            skyMan = SkyManager.instance;
            if (!starRend)
                starRend = this.GetComponent<Renderer>();

            if (!starMat)
                starMat = skyMan.starMat;

            starFade = PresetContainer.instance.starGradient;
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

        public void ToggleState(bool on)
        {
            this.gameObject.SetActive(on);

            if (!on)
                StopAllCoroutines();
            else
            {
                gameObject.GetComponent<ParticleSystem>().enableEmission = on;
                Init(skyMan.IsOvercast);

            }
        }



        public void Init(bool isOverCast)
        {
            StopAllCoroutines();
            

            starRend.enabled = false;
            setColors = true;
            StartCoroutine(SetStars(isOverCast));
        }


        /// <summary>
        /// if not overcast, will brighten and dim stars over time.  If over cast, stars are turned off and loop exits.
        /// </summary>
        /// <param name="isOverCast"></param>
        /// <returns></returns>
        IEnumerator SetStars(bool isOverCast)
        {
            while (!isOverCast)
            {
                //currentSeconds = skyMan.CurrentSeconds;

                if (skyMan.CurrentSeconds < skyMan.DuskTime && skyMan.CurrentSeconds > skyMan.DawnTime)  //daytime, not overcast                                                            
                {
                    starRend.enabled = false;
                    yield return new WaitForEndOfFrame();
                }
                else
                    starRend.enabled = true;

                Color colorCheck = starFade.Evaluate(skyMan.TimeRatio);
                if (lastColor.a != colorCheck.a)
                    setColors = true;

                if (setColors)
                {
                    lastColor = colorCheck;
                    SetStarColor(lastColor);
                    setColors = false;
                }
                
                yield return new WaitForEndOfFrame();
            }
            
            yield break;
        }


        private void SetStarColor(Color color)
        {
            //Debug.Log("setting star colors");
            starMat.SetColor("_Color", color);
        }

    }



}
