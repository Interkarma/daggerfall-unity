//Enhanced Sky for Daggerfall Tools for Unity by Lypyl, contact at lypyl@dfworkshop.net
//http://www.reddit.com/r/dftfu
//http://www.dfworkshop.net/
//Author: LypyL
///Contact: Lypyl@dfworkshop.net
//License: MIT License (http://www.opensource.org/licenses/mit-license.php)
// v. 1.8.1

/*  moon textures by Doubloonz @ nexus mods, w/ permission to use for DFTFU.
 *http://www.nexusmods.com/skyrim/mods/40785/
 * 
 */

using UnityEngine;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Utility;
using DaggerfallConnect.Utility;
using System;



/* Setup instructions:
 * 1. Add EnhancedSkyController prefab to scene.
 *
 * 2. Add new layer named SkyLayer.
 * 
 * 3. Change fog to expon. squared, and set density to something like .000012
 *
 * 4. Change Main Camera Clear flags to depth only.
 * 
 * 5. Uncheck SkyLayer in Main Camera's Culling Mask list (same for other cameras again - only skyCam should have it checked).
 *
 * 6. Add ToggleEnhancedSky to input manager.
 * 
 * Tested w/ Daggerfall tools for Unity v. 1.4.16 (WIP)
 */
namespace EnhancedSky
{

    public class SkyManager : MonoBehaviour
    {


        #region Fields
        public const int dayInSeconds = 86400;
        public const int offset = 21600;

        //Enhanced sky objects & materials
        //public GameObject CloudpreFab;
        public Material cloudMat;
        public Material starMat;
        public Material skyMat;
       
        public GameObject MoonMasser;
        public GameObject MoonSecunda;
        public RotationScript rotScript;
        public GameObject Stars;
        public GameObject SkyCamObject;

        //daggerfall tools references
        DaggerfallUnity dfUnity;
        DaggerfallDateTime timeScript;
        public GameObject dfallSky;
        public WeatherManager weatherMan;
        public PlayerEnterExit playerEE;

        System.Diagnostics.Stopwatch stopWatch;
        public int TimeInsideLimit = 3;
        private bool _useSunFlare = true;
        private bool _enhancedSkyToggle = true;
        #endregion

        #region Properties

        public bool UseSunFlare { get { return _useSunFlare; } set { _useSunFlare = value;} }
        public bool EnhancedSkyToggle { get { return _enhancedSkyToggle; } set { _enhancedSkyToggle = value;} }
        public bool IsOvercast { get; private set; }
        public bool IsNight { get { if (timeScript == null) return false; else return timeScript.IsNight; } }
        public float CurrentSeconds { get; set; }
        public float TimeRatio { get {return (CurrentSeconds / dayInSeconds); }}
        public int DawnTime { get; private set; }
        public int DuskTime { get; private set; }
        public int TimeInside { get; set; }
        #endregion

        #region Singleton
        private static SkyManager _instance;


        public static SkyManager instance
        {
            get { 
                if(_instance == null)
                    _instance = GameObject.FindObjectOfType<SkyManager>();
                return _instance;
            }
            private set { _instance = value; }
        }
        #endregion

        #region Events
        //Events & handlers
        public delegate void SkyEvent(bool isOverCast);                                     
        public static event SkyEvent fastTravelEvent;
        public static event SkyEvent toggleSkyObjectsEvent;

        /// <summary>
        /// Get InteriorTransition & InteriorDungeonTransition events from PlayerEnterExit, and triggers event
        /// </summary>
        /// <param name="args"></param>
        public void InteriorTransitionEvent(PlayerEnterExit.TransitionEventArgs args)      //player went indoors (or dungeon), disable sky objects
        {
            CurrentSeconds = UpdateTime();

            if (SkyManager.instance.EnhancedSkyToggle)
            {
                //Debug.Log("IndoorTransitionEvent");
                stopWatch.Reset();
                stopWatch.Start();
                ToggleSkyObjects(false);
            }

        }

        /// <summary>
        /// Get ExteriorTransition & DungeonExteriorTransition events from PlayerEnterExit & triggers event
        /// </summary>
        /// <param name="args"></param>
        public void ExteriorTransitionEvent(PlayerEnterExit.TransitionEventArgs args)   //player transitioned to exterior from indoors or dungeon
        {
            CurrentSeconds = UpdateTime();


            if (SkyManager.instance.EnhancedSkyToggle)
            {
                IsOvercast = weatherMan.IsOvercast;
                stopWatch.Stop();
                TimeInside = stopWatch.Elapsed.Minutes;
                //Debug.Log("time inside: " + _timeInside);
                ToggleSkyObjects(true);                 //enable sky objects
            }

        }
        
        /// <summary>
        /// Disables / enables the Enhanced sky objects
        /// </summary>
        /// <param name="toggle"></param>
        public void ToggleSkyObjects(bool toggle)
        {
            Debug.Log("ToggleSkyObjects toggle: " + toggle);
            CurrentSeconds = UpdateTime();

            try
            {
                dfallSky.SetActive(!toggle);
                MoonMasser.SetActive(toggle);
                MoonSecunda.SetActive(toggle);
                SkyCamObject.SetActive(toggle);
            }
            catch (Exception ex)
            {
                Debug.LogWarning("Error enabling or diabling Daggerfall Sky object. ");
                Debug.LogWarning(ex.Message + " | in ToggleSkyObjects toggle: " + toggle);
            }
            
            if(toggleSkyObjectsEvent != null)
                toggleSkyObjectsEvent(toggle);

        }

        


        /// <summary>
        /// Updates enhanced sky objects when Player Teleports using StreamingWorld.TeleportToCoordinates()
        /// </summary>
        /// <param name="worldPos"></param>
        public void EnhancedSkyUpdate(DFPosition worldPos)                         //player teleporting
        {
            CurrentSeconds = UpdateTime();

            //Debug.Log("EnhancedSkyUpdate");
            if (fastTravelEvent != null && SkyManager.instance.EnhancedSkyToggle)   //only trigger if eSky on
            {
                //Debug.Log("triggering fastTravelEvent");
                fastTravelEvent(weatherMan.IsOvercast);

            }

        }
        #endregion

        #region Unity
        void Awake()
        {
            if (_instance == null)
                _instance = this;
            else if(this != instance)
            {
                Destroy(this.gameObject);
            }
            
            if (!cloudMat)
                cloudMat = Resources.Load("MainCloud") as Material;
            //cloudMat.renderQueue = 2998;    //put clouds behind horizon block - not used anymore
            if (!starMat)
                starMat = Resources.Load("Stars") as Material;
            if (!skyMat)
                skyMat = Resources.Load("Sky") as Material;
            RenderSettings.skybox = skyMat;
            stopWatch = new System.Diagnostics.Stopwatch();

            //Transition event subscriptions.  These will trigger sky events
            PlayerEnterExit.OnTransitionInterior += InteriorTransitionEvent; //interior transition
            PlayerEnterExit.OnTransitionDungeonInterior += InteriorTransitionEvent; //dungeon interior transition
            PlayerEnterExit.OnTransitionExterior += ExteriorTransitionEvent; //exterior transition
            PlayerEnterExit.OnTransitionDungeonExterior += ExteriorTransitionEvent; //dungeon exterior transition
            StreamingWorld.OnTeleportToCoordinates += EnhancedSkyUpdate;
        }


        // Use this for initialization
        void Start()
        {
            dfUnity = DaggerfallUnity.Instance;
            timeScript = dfUnity.WorldTime.DaggerfallDateTime;		
            DuskTime = DaggerfallDateTime.DuskHour * 3600;     
            DawnTime = DaggerfallDateTime.DawnHour * 3600;      

            if (!weatherMan)
                weatherMan = GameObject.Find("WeatherManager").GetComponent<WeatherManager>();
            if(!dfallSky)
                dfallSky = GameObject.Find("SkyRig");
            if (!playerEE)
                playerEE = GameObject.Find("PlayerAdvanced").GetComponent<PlayerEnterExit>();
            if (!MoonMasser)
                MoonMasser = GameObject.Find("MoonMasser");
            if (!MoonSecunda)
                MoonSecunda = GameObject.Find("MoonSecunda");
            if(rotScript)
                rotScript = GameObject.Find("Rotator").GetComponent<RotationScript>();
            if (!SkyCamObject)
                SkyCamObject = GameObject.Find("SkyCam");

            CurrentSeconds = UpdateTime();
            IsOvercast = weatherMan.IsOvercast;
            ToggleSkyObjects(EnhancedSkyToggle);
            Debug.Log("");
        }

        // Update is called once per frame
        void Update()
        {
            CurrentSeconds = UpdateTime();
            IsOvercast = weatherMan.IsOvercast;

            //if(Input.GetButtonDown("ToggleEnhancedSky") && !playerEE.IsPlayerInside)
            //{
            //    EnhancedSkyToggle = !EnhancedSkyToggle;
            //    ToggleAdvancedSky(EnhancedSkyToggle);
            //}
 
        }

        void OnDestroy()
        {
            //Unsubscribe from events
            PlayerEnterExit.OnTransitionInterior -= InteriorTransitionEvent; //interior transition
            PlayerEnterExit.OnTransitionDungeonInterior -= InteriorTransitionEvent; //dungeon interior transition
            PlayerEnterExit.OnTransitionExterior -= ExteriorTransitionEvent; //exterior transition
            PlayerEnterExit.OnTransitionDungeonExterior -= ExteriorTransitionEvent; //dungeon exterior transition
            StreamingWorld.OnTeleportToCoordinates -= EnhancedSkyUpdate;

            StopAllCoroutines();
            if (_instance == this)
                _instance = null;
        }
        #endregion

        #region methods
        public float UpdateTime()
        {
            try
            {
                return (timeScript.MinuteOfDay * 60) + timeScript.Second;
            }
            catch
            {
                dfUnity = DaggerfallUnity.Instance;
                timeScript = dfUnity.WorldTime.DaggerfallDateTime;
                if (dfUnity != null && timeScript != null)
                    return (timeScript.MinuteOfDay * 60) + timeScript.Second;
                else
                    return -1f;
            }
        }

        
        void ToggleAdvancedSky(bool toggle)
        {
            ToggleSkyObjects(toggle);
        }

        #endregion

    }

}