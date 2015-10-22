//Enhanced Sky for Daggerfall Tools for Unity by Lypyl, contact at lypyl@dfworkshop.net
//http://www.reddit.com/r/dftfu
//http://www.dfworkshop.net/
//Author: LypyL
///Contact: Lypyl@dfworkshop.net
//License: MIT License (http://www.opensource.org/licenses/mit-license.php)

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop;

namespace EnhancedSky
{
    public class RotationScript : MonoBehaviour
    {
        public SkyManager skyMan; 

        // Use this for initialization
        void Start()
        {
            //event subscriptions
            skyMan = SkyManager.instance;
            SkyManager.fastTravelEvent += this.Init;
            SkyManager.toggleSkyObjectsEvent += this.ToggleState;
            Init(false);
            
        }

        void OnDestroy()
        {
            StopAllCoroutines();
            SkyManager.fastTravelEvent -= this.Init;
            SkyManager.toggleSkyObjectsEvent -= this.ToggleState;

        }

        public void ToggleState(bool on)
        {
            //this.enabled = onOff;
            this.gameObject.SetActive(on);
            
            if (!on)
                StopAllCoroutines();
            else
                Init(skyMan.IsOvercast);
            
        }


        public void Init(bool isOverCast)
        {
            StopAllCoroutines();
            StartCoroutine(Rotate());
        }

        
        IEnumerator Rotate()
        {
            //jump to current rotation before lerping, so the sun/moons don't appear to slide when player fast travels
            //or has spent a long time inside
            float degreeRoation;
            degreeRoation = ((float)(skyMan.CurrentSeconds - SkyManager.offset) / SkyManager.dayInSeconds) * 360;
            this.transform.rotation = (Quaternion.Euler(degreeRoation, 270f, 0f));
            
            while (true)
            {
                yield return new WaitForEndOfFrame();
                degreeRoation = ((float)(skyMan.CurrentSeconds - SkyManager.offset) / SkyManager.dayInSeconds) * 360;
                this.transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(degreeRoation, 270, 0), Time.deltaTime);
            }


        }

        


    }
}