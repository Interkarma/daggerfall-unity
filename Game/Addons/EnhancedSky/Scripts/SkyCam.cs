//Enhanced Sky for Daggerfall Tools for Unity by Lypyl, contact at lypyl@dfworkshop.net
//http://www.reddit.com/r/dftfu
//http://www.dfworkshop.net/
//Author: LypyL
///Contact: Lypyl@dfworkshop.net
//License: MIT License (http://www.opensource.org/licenses/mit-license.php)
// v. 1.7.0


using UnityEngine;

namespace EnhancedSky
{
    public class SkyCam : MonoBehaviour
    {
        public GameObject MainCamera;
        public Camera SkyCamera;

        // Use this for initialization
        void Start()
        {
            SkyCamera = this.GetComponent<Camera>();
            if (!MainCamera)
                MainCamera = GameObject.FindGameObjectWithTag("MainCamera");

            SkyCamera.renderingPath = MainCamera.GetComponent<Camera>().renderingPath;

        }

        void LateUpdate()
        {
            this.transform.rotation = MainCamera.transform.rotation;
        }

    }
}