//Enhanced Sky for Daggerfall Tools for Unity by Lypyl, contact at lypyl@dfworkshop.net
//http://www.reddit.com/r/dftfu
//http://www.dfworkshop.net/
//Author: LypyL
///Contact: Lypyl@dfworkshop.net
//License: MIT License (http://www.opensource.org/licenses/mit-license.php)
// v. 1.9.1

/*  moon textures by Doubloonz @ nexus mods, w/ permission to use for DFTFU.
 *http://www.nexusmods.com/skyrim/mods/40785/
 * 
 */

using UnityEngine;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Utility;
using DaggerfallConnect.Utility;



/* Setup instructions:
 * 1. Add EnhancedSkyController prefab to scene (shouldn't be a child of something that gets disabled like Exterior object)
 * 
 * 2. Add new layer named SkyLayer.
 * 
 * 3. Change fog to expon. squared, and set density to something like .000012
 *
 * 4. Change Main Camera Clear flags to depth only.
 * 
 * 5. Uncheck SkyLayer in Main Camera's Culling Mask list (same for other cameras again - only skyCam should have it checked).
 *
 * 6. Make sure all the public refrences are set on the Controller in scene (DaggerfallSky Rig, 
 * Daggerfall WeatherManager, PlayerEnterExit & Exterior Parent).
 * 
 * Tested w/ Daggerfall tools for Unity v. 1.4.45 (WIP)
 */
namespace EnhancedSky
{
    public enum SkyObjectSize
    {
        Normal,
        Large,
    }

    public class SkyManager : MonoBehaviour
    {


        #region Fields
        public const int DAYINSECONDS = 86400;
        public const int OFFSET = 21600;
        public const int TIMEINSIDELIMIT = 1;

        private Material _skyMat;
        private Material _cloudMat;
        private Material _masserMat;
        private Material _secundaMat;
        private Material _starsMat;
        private Material _skyObjMat;
        private Shader _depthMaskShader;
        private Shader _UnlitAlphaFadeShader;
        public int cloudQuality = 400;
        public int cloudSeed = -1;
        public bool EnhancedSkyCurrentToggle = false;

        //daggerfall tools references
        public GameObject       dfallSky;
        public WeatherManager   weatherMan;
        public PlayerEnterExit  playerEE;
        public GameObject       exteriorParent;

        System.Diagnostics.Stopwatch _stopWatch;
        CloudGenerator  _cloudGen;
        GameObject      _container;
        GameObject      _containerPrefab;
        #endregion

        #region Properties
        DaggerfallUnity DfUnity         { get { return DaggerfallUnity.Instance;} }
        DaggerfallDateTime TimeScript   { get { return DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime; } }

        
        public Material SkyObjMat       { get { return (_skyObjMat != null) ? _skyObjMat : Resources.Load("SkyObjMat") as Material; } private set {_skyObjMat = value ;} }
        public Material SkyMat          { get { return (_skyMat) ? _skyMat : _skyMat = Resources.Load("Sky") as Material; } private set { _skyMat = value ;} }
        public Material CloudMat        { get { return (_cloudMat) ? _cloudMat : _cloudMat = GetInstanceMaterial(); } private set { _cloudMat = value; } }
        public Material MasserMat       { get { return (_masserMat) ? _masserMat : _masserMat = GetInstanceMaterial(); } private set { _masserMat = value; } }
        public Material SecundaMat      { get { return (_secundaMat) ? _secundaMat : _secundaMat = GetInstanceMaterial(); } private set { _secundaMat = value; } }
        public Material StarsMat        { get; private set; }
        public Material StarMaskMat     { get; private set; }

        public SkyObjectSize SkyObjectSizeSetting { get; set; }
        public CloudGenerator CloudGen  { get { return (_cloudGen != null) ? _cloudGen : _cloudGen = this.GetComponent<CloudGenerator>(); } }
        public bool UseSunFlare         { get; set; }
        public bool IsOvercast          { get { return (weatherMan != null) ? weatherMan.IsOvercast : false; } }
        public bool IsNight             { get { return (TimeScript != null) ? TimeScript.IsNight : false; } }
        public float CurrentSeconds     { get { return UpdateTime(); } }
        public float TimeRatio          { get {return (CurrentSeconds / DAYINSECONDS); }}
        public int DawnTime             { get; private set; }
        public int DuskTime             { get; private set; }
        public int TimeInside           { get; set; } 
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

        #region Events & Handlers
        //Events & handlers
        public delegate void SkyEvent(bool isOverCast);
        public delegate void UpdateSkyObjectSettings();

        public static event SkyEvent updateSkyEvent;
        public static event SkyEvent toggleSkyObjectsEvent; //no longer needed
        public static event UpdateSkyObjectSettings updateSkySettingsEvent;

        /// <summary>
        /// Get InteriorTransition & InteriorDungeonTransition events from PlayerEnterExit
        /// </summary>
        /// <param name="args"></param>
        public void InteriorTransitionEvent(PlayerEnterExit.TransitionEventArgs args)      //player went indoors (or dungeon), disable sky objects
        {
            _stopWatch.Reset();
            _stopWatch.Start();
        }

        /// <summary>
        /// Get ExteriorTransition & DungeonExteriorTransition events from PlayerEnterExit
        /// </summary>
        /// <param name="args"></param>
        public void ExteriorTransitionEvent(PlayerEnterExit.TransitionEventArgs args)   //player transitioned to exterior from indoors or dungeon
        {
            _stopWatch.Stop();
            TimeInside = _stopWatch.Elapsed.Minutes;
            if(EnhancedSkyCurrentToggle)
                ToggleSkyObjects(true);                 //enable sky objects
        }

        public void WeatherManagerSkyEventsHandler()
        {
            if (updateSkyEvent != null)
                updateSkyEvent(IsOvercast);

        }


        /// <summary>
        /// Disables / enables the Enhanced sky objects
        /// </summary>
        /// <param name="toggle"></param>
        private void ToggleSkyObjects(bool toggle)
        {

            try
            {
                if(!toggle && _container != null) 
                {
                    dfallSky.SetActive(true);
                    Destroy(_container);
                    
                }
                else if(toggle && !_container)
                {
                    GetRefrences();
                    
                    if(SkyMat)
                        RenderSettings.skybox = SkyMat;
                    else
                        throw new System.NullReferenceException();
                    if (_containerPrefab)
                    {
                        _container = Instantiate(_containerPrefab);
                        _container.transform.SetParent(exteriorParent.transform, true);
                    }
                    else
                        throw new System.NullReferenceException();

                    dfallSky.SetActive(false);
                    SkyObjectSizeChange(SkyObjectSizeSetting);
                }
                
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("Error enabling or diabling Daggerfall Sky object. ");
                Debug.LogWarning(ex.Message + " | in ToggleSkyObjects toggle: " + toggle);
            }
            
            //trigger toggleSkyObject event - this event is not used by ESKY anymore
            if(toggleSkyObjectsEvent != null)
               toggleSkyObjectsEvent(IsOvercast);

        }


        /// <summary>
        /// Updates enhanced sky objects
        /// </summary>
        /// <param name="worldPos"></param>
        public void EnhancedSkyUpdate(DFPosition worldPos)                         //player teleporting
        {
            //Debug.Log("EnhancedSkyUpdate");
            if (updateSkyEvent != null && SkyManager.instance.EnhancedSkyCurrentToggle)   //only trigger if eSky on
            {
                //Debug.Log("triggering fastTravelEvent");
                updateSkyEvent(IsOvercast);
            }

        }
        #endregion

        #region Unity
        void Awake()
        {
            if (_instance == null)
                _instance = this;
            else if (this != instance)
            {
                Destroy(this.gameObject);
            }
            _stopWatch = new System.Diagnostics.Stopwatch();

            PlayerEnterExit.OnTransitionInterior        += InteriorTransitionEvent; //interior transition
            PlayerEnterExit.OnTransitionDungeonInterior += InteriorTransitionEvent; //dungeon interior transition
            PlayerEnterExit.OnTransitionExterior        += ExteriorTransitionEvent; //exterior transition
            PlayerEnterExit.OnTransitionDungeonExterior += ExteriorTransitionEvent; //dungeon exterior transition
            StreamingWorld.OnTeleportToCoordinates      += EnhancedSkyUpdate;
            WeatherManager.OnClearOvercast              += WeatherManagerSkyEventsHandler;
            WeatherManager.OnSetRainOvercast            += WeatherManagerSkyEventsHandler;
            WeatherManager.OnSetSnowOvercast            += WeatherManagerSkyEventsHandler;

        }


        // Use this for initialization
        void Start()
        {
            DuskTime = DaggerfallDateTime.DuskHour * 3600;     
            DawnTime = DaggerfallDateTime.DawnHour * 3600;
          
            GetRefrences();
            //Register Console Commands
            EnhancedSkyConsoleCommands.RegisterCommands();

            
            if (DaggerfallUnity.Instance.IsReady)
                EnhancedSkyCurrentToggle = DaggerfallUnity.Settings.LypyL_EnhancedSky;


            // player starting outside & ESKY starting on
            if (playerEE != null && !playerEE.IsPlayerInside)
                ToggleEnhancedSky(EnhancedSkyCurrentToggle);

           
        }

        void OnDestroy()
        {
          
            
            ToggleSkyObjects(false);

            //Unsubscribe from events
            PlayerEnterExit.OnTransitionInterior        -= InteriorTransitionEvent; //interior transition
            PlayerEnterExit.OnTransitionDungeonInterior -= InteriorTransitionEvent; //dungeon interior transition
            PlayerEnterExit.OnTransitionExterior        -= ExteriorTransitionEvent; //exterior transition
            PlayerEnterExit.OnTransitionDungeonExterior -= ExteriorTransitionEvent; //dungeon exterior transition
            StreamingWorld.OnTeleportToCoordinates      -= EnhancedSkyUpdate;
            WeatherManager.OnClearOvercast              -= WeatherManagerSkyEventsHandler;
            WeatherManager.OnSetRainOvercast            -= WeatherManagerSkyEventsHandler;
            WeatherManager.OnSetSnowOvercast            -= WeatherManagerSkyEventsHandler;

            Destroy(StarsMat);
            Destroy(_skyObjMat);
            Destroy(StarMaskMat);
            Destroy(MasserMat);
            Destroy(SecundaMat);
            Resources.UnloadAsset(_depthMaskShader);
            Resources.UnloadAsset(_UnlitAlphaFadeShader);
            

            StopAllCoroutines();
            if (_instance == this)
                _instance = null;
        }
        #endregion

        #region methods

   

        private bool GetRefrences()
        {
            try
            {
                if (!_depthMaskShader)
                    _depthMaskShader = Resources.Load("DepthMask") as Shader;
                if (!_UnlitAlphaFadeShader)
                    _UnlitAlphaFadeShader = Resources.Load("UnlitAlphaWithFade") as Shader;
                if (!StarMaskMat)
                    StarMaskMat = new Material(_depthMaskShader);
                if (!_skyObjMat)
                    _skyObjMat = new Material(_UnlitAlphaFadeShader);
                if (!StarsMat)
                    StarsMat = Instantiate(Resources.Load("Stars")) as Material;
                if (!SkyMat)
                    SkyMat = Instantiate(Resources.Load("Sky")) as Material;
                if (!_cloudGen)
                    _cloudGen = this.GetComponent<CloudGenerator>();
                if(!_cloudGen)
                    _cloudGen = gameObject.AddComponent<CloudGenerator>();
                if (!_containerPrefab)
                    _containerPrefab = Resources.Load("EnhancedSkyContainer", typeof(GameObject)) as GameObject;
                if (!dfallSky)
                    dfallSky = GameManager.Instance.SkyRig.gameObject;
                if (!playerEE)
                    playerEE = GameManager.Instance.PlayerEnterExit;
                if (!exteriorParent)
                    exteriorParent = GameManager.Instance.ExteriorParent;
                if (!weatherMan)
                    weatherMan = GameManager.Instance.WeatherManager;


            }
            catch
            {
                DaggerfallUnity.LogMessage("Error in SkyManager.GetRefrences()", true);
                return false;
            }
            if (dfallSky && playerEE && exteriorParent && weatherMan && _cloudGen && _containerPrefab && _depthMaskShader && _UnlitAlphaFadeShader 
                && StarMaskMat && _skyObjMat && StarsMat && SkyMat)
                return true;
            else
                return false;

        }


        private Material GetInstanceMaterial()
        {
            return Instantiate(SkyObjMat);
        }

        private float UpdateTime()
        {
            try
            {
                return (TimeScript.MinuteOfDay * 60) + TimeScript.Second;
            }
            catch
            {
                GetRefrences();
                if (DfUnity != null && TimeScript != null)
                    return (TimeScript.MinuteOfDay * 60) + TimeScript.Second;
                else
                {
                    Debug.LogWarning("SkyManager couldn't UpdateTime");
                    return -1;
                }
            }
        }

        
        public void ToggleEnhancedSky(bool toggle)
        {
            if(!GetRefrences() && toggle)
            {
                DaggerfallUnity.LogMessage("Skymanager missing refrences, can't enable");
                return;

            }
            EnhancedSkyCurrentToggle = toggle;
            ToggleSkyObjects(toggle);
            
        }

    
        public void SkyObjectSizeChange(SkyObjectSize size)
        {
            SkyObjectSizeSetting = size;
            if(!EnhancedSkyCurrentToggle || SkyMat == null)
            {
                //Debug.Log("Sky Material was null");
                return;
            }

            if(size == SkyObjectSize.Normal)
                SkyMat.SetFloat("_SunSize", PresetContainer.SUNSIZENORMAL);
            else
                SkyMat.SetFloat("_SunSize", PresetContainer.SUNSIZELARGE);
            
            
            if (updateSkySettingsEvent != null)
                updateSkySettingsEvent();
        }


        public void SetCloudTextureResolution(int resolution)
        {
            if(resolution < PresetContainer.MINCLOUDDIMENSION)
                resolution = PresetContainer.MINCLOUDDIMENSION;
            else if(resolution > PresetContainer.MAXCLOUDDIMENSION)
                resolution = PresetContainer.MAXCLOUDDIMENSION;
            else
                cloudQuality = resolution;
            if (updateSkySettingsEvent != null)
                updateSkySettingsEvent();
        }





        #endregion

    }

}