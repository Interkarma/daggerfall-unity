//Enhanced Sky for Daggerfall Tools for Unity by Lypyl, contact at lypyl@dfworkshop.net
//http://www.reddit.com/r/dftfu
//http://www.dfworkshop.net/
//Author: LypyL
///Contact: Lypyl@dfworkshop.net
//License: MIT License (http://www.opensource.org/licenses/mit-license.php)



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

            //SkyCamera.renderingPath = MainCamera.GetComponent<Camera>().renderingPath;
            GetCameraSettings();

        }

        void LateUpdate()
        {
            this.transform.rotation = MainCamera.transform.rotation;
        }

        void GetCameraSettings()
        {
            Camera mainCam = MainCamera.GetComponent<Camera>();
            if(mainCam)
            {
                SkyCamera.renderingPath = mainCam.renderingPath;
                SkyCamera.fieldOfView = mainCam.fieldOfView;

            }
            else
            {
                Debug.Log("Using default settings for SkyCamera");
                SkyCamera.fieldOfView = 65;
                SkyCamera.renderingPath = RenderingPath.DeferredShading;

            }




        }




    }
}