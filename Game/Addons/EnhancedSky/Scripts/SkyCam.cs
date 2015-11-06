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
        public GameObject mainCamera;
        public Camera skyCamera;

        // Use this for initialization
        void Start()
        {
            if(!skyCamera)
                skyCamera = this.GetComponent<Camera>();
            if (!mainCamera)
                mainCamera = DaggerfallWorkshop.Game.GameManager.Instance.MainCameraObject;

            //SkyCamera.renderingPath = MainCamera.GetComponent<Camera>().renderingPath;
            GetCameraSettings();

        }

        void LateUpdate()
        {
            this.transform.rotation = mainCamera.transform.rotation;
        }

        void GetCameraSettings()
        {
            Camera mainCam = mainCamera.GetComponent<Camera>();
            if(mainCam)
            {
                skyCamera.renderingPath = mainCam.renderingPath;
                skyCamera.fieldOfView = mainCam.fieldOfView;

            }
            else
            {
                Debug.Log("Using default settings for SkyCamera");
                skyCamera.fieldOfView = 65;
                skyCamera.renderingPath = RenderingPath.DeferredShading;

            }




        }




    }
}