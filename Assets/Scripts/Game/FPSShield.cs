using UnityEngine;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using DaggerfallWorkshop.Game.Utility.ModSupport.ModSettings; //required for modding features
using System.IO;
using System.Collections;
using Random = UnityEngine.Random;

namespace DaggerfallWorkshop.Game.OutfitManager
{
    public class FPSShield : MonoBehaviour
    {

        #region UI Rects
        Rect shieldPos = new Rect(0, Screen.height - 400, 850, 850 );
        #endregion     

        //initiates mod instances for mod manager.
        static Mod mod;
        static FPSShield instance;
        static ModSettings settings;
        string blockKey;

        //initiates 2d textures for storing texture data.
        static Texture2D shieldTex;

        WeaponAnimation[] weaponAnims;
        Rect curAnimRect;
        Rect[] weaponRects;
        // Get weapon scale
        float shieldScaleX;
        float shieldScaleY;
        //used for storing texture path for shield.
        string shieldTexture_Path;

        //used for lerp calculator. Need to create custom mod scripting hook files for myself and others.
        bool lerpfinished;
        bool isBlocking;
        bool cycled;
        bool blockFinished;
        bool breatheTrigger;
        float TimeCovered = 0;
        float hPos;
        float vPos;
        float size;
        float endTime;
        float shieldSize;
        int cycleNum;

        //starts mod manager on game begin. Grabs mod initializing paramaters.
        //ensures SateTypes is set to .Start for proper save data restore values.
        [Invoke(StateManager.StateTypes.Start, 0)]
        public static void Init(InitParams initParams)
        {
            Debug.Log("SHIELD MODULE STARTED!");
            //sets up instance of class/script/mod.
            GameObject go = new GameObject("Shield Mod");
            instance = go.AddComponent<FPSShield>();
            //initiates mod paramaters for class/script.
            mod = initParams.Mod;
            //loads mods settings.
            settings = mod.GetSettings();
            //initiates save paramaters for class/script.
            //mod.SaveDataInterface = instance;
            //after finishing, set the mod's IsReady flag to true.
            mod.IsReady = true;
        }

        private void Start()
        {
            //sets shield texture path, using the application base directory.
            shieldTexture_Path = Application.dataPath + "/Scripts/Game/shield.png";
            // Get weapon scale
            shieldScaleX = (float)Screen.width / 320;
            shieldScaleY = (float)Screen.height / 200;

            StartCoroutine(debug());

            //assigns loaded texture to shieldTex using loadPNG method.
            shieldTex = LoadPNG(shieldTexture_Path);

            blockKey = settings.GetValue<string>("Settings", "blockKey");
        }

        private void Update()
        {
            if (Input.GetKey(blockKey))
            {
                blockFinished = false;
                isBlocking = true;
            } else if (Input.GetKeyUp(blockKey))
                blockFinished = true;

            shieldPos = new Rect((Screen.width * 1.75f) * vPos, Screen.height * (1.75f - hPos) - 200 * shieldScaleY, size * shieldScaleX, size * shieldScaleY);

            if (!isBlocking)
            {
                //bobbing system. Need to simplify this if then check.
                if ((InputManager.Instance.HasAction(InputManager.Actions.MoveRight) || InputManager.Instance.HasAction(InputManager.Actions.MoveLeft) || InputManager.Instance.HasAction(InputManager.Actions.MoveForwards) || InputManager.Instance.HasAction(InputManager.Actions.MoveBackwards)))
                {
                    hPos = LerpCalculator(3f, 0, 0, .13f, "smootherstep", true, true, out cycleNum);
                    vPos = LerpCalculator(3f, 0, 0, .025f, "linear", true, true, out cycleNum);
                    size = 150;
                }
            }
            else if (isBlocking == true)
            {
                 if (blockFinished == false)
                {
                    hPos = LerpCalculator(2f, 0, 0, .33f, "smootherstep", false, false,out cycleNum,1);
                    vPos = LerpCalculator(2f, 0, 0, .025f, "linear", false, false, out cycleNum, 1);
                    size = LerpCalculator(2f, 0, 150, 200, "linear", false, false, out cycleNum, 1);
                }
                else if (blockFinished == true)
                {
                    hPos = LerpCalculator(2f, 0, .33f, 0, "smootherstep", false, false, out cycleNum, 1);
                    vPos = LerpCalculator(2f, 0, .025f, 0, "linear", false, false, out cycleNum, 1);
                    size = LerpCalculator(2f, 0, 200, 150, "linear", false, false, out cycleNum, 1);

                    if (hPos <= .03f)
                    {
                        blockFinished = false;
                        isBlocking = false;
                    }
                }
            }
        }

        private void OnGUI()
        {
            //loads shield texture if wapon is showing.
            if (GameManager.Instance.WeaponManager.ScreenWeapon.ShowWeapon == true)
            {
                GUI.DrawTextureWithTexCoords(shieldPos, shieldTex, new Rect(0.0f, 0.0f, 1, 1));
                if (GameManager.Instance.WeaponManager.ScreenWeapon.WeaponState != WeaponStates.Idle)
                {                    
                    GUI.DrawTextureWithTexCoords(shieldPos, shieldTex, new Rect(0.0f, 0.0f, 1, 1));
                }
                else
                    GUI.DrawTextureWithTexCoords(shieldPos, shieldTex, new Rect(0.0f, 0.0f, 1, 1));
            }
        }

        float LerpCalculator(float duration, float startTime, float startValue, float endValue, string lerpEquation, bool loop, bool breathe, out int cycleNum, int cycles = 0)
        {
            float lerpvalue = 0;
            cycleNum = 0;
            if (loop == false)
            {
                for (int i = 0; i <= cycles; i++)
                {
                    cycleNum = i;
                    lerping();
                }

                TimeCovered = 0;
            }
            else
            {
                for (int i = 0; ; i++)
                {
                    cycleNum = i;
                    lerping();
                }
            }

            void lerping()
            {
                if (TimeCovered >= duration && breatheTrigger)
                    breatheTrigger = false;
                else if (TimeCovered <= 0 && !breatheTrigger)
                    breatheTrigger = true;

                if (breatheTrigger)
                    // Distance moved equals elapsed time times speed.
                    TimeCovered += Time.deltaTime + startTime;
                else
                    // Distance moved equals elapsed time times speed.
                    TimeCovered -= Time.deltaTime + startTime;


                // Fraction of journey completed equals current time divided by total movement time.
                float fractionOfJourney = TimeCovered / duration;

                if (loop == true && (fractionOfJourney < 0f || fractionOfJourney > 1f))
                {
                    lerpfinished = false;

                    if (breathe == false)
                    {
                        TimeCovered = 0;
                    }
                }
                else if (fractionOfJourney < 0f || fractionOfJourney > 1f)
                {
                    lerpfinished = true;
                }

                //stops the dodge if the dodge is below 0% or over 100% of dodge time.
                if (lerpfinished == false)
                {
                    //reprocesses time passed into a sin graph function to provide a ease out movement shape instead of basic linear movement.
                    if (lerpEquation == "linear" || lerpEquation == null || lerpEquation == "")
                        ; //do nothing to keep basic linear lerping;
                    else if (lerpEquation == "easeout")
                        fractionOfJourney = Mathf.Sin(fractionOfJourney * Mathf.PI * 0.5f);
                    else if (lerpEquation == "easein")
                        fractionOfJourney = 1f - Mathf.Cos(fractionOfJourney * Mathf.PI * 0.5f);
                    else if (lerpEquation == "exponential")
                        fractionOfJourney = fractionOfJourney * fractionOfJourney;
                    else if (lerpEquation == "smoothstep")
                        fractionOfJourney = fractionOfJourney * fractionOfJourney * (3f - 2f * fractionOfJourney);
                    else if (lerpEquation == "smootherstep")
                        fractionOfJourney = fractionOfJourney * fractionOfJourney * fractionOfJourney * (fractionOfJourney * (6f * fractionOfJourney - 15f) + 10f);

                    lerpvalue = Mathf.Lerp(startValue, endValue, fractionOfJourney);
                }
                else
                    lerpvalue = endValue;
            }

            return lerpvalue;
        }


        //texture loading method.
        public static Texture2D LoadPNG(string filePath)
        {

            Texture2D tex = null;
            byte[] fileData;

            if (File.Exists(filePath))
            {
                fileData = File.ReadAllBytes(filePath);
                tex = new Texture2D(2, 2);
                tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
            }
            else
                Debug.Log("FilePath Broken!");

            return tex;
        }

        //Debug coroutine for fine tuning.
        IEnumerator debug()
        {
            while (true)
            {
                DaggerfallUI.Instance.PopupMessage("Blocking: " + isBlocking.ToString() + " | Finished: " + blockFinished.ToString() + " | Cycled: " + cycled.ToString() + " | Lerp: " + lerpfinished.ToString());
                yield return new WaitForSeconds(.5f);
            }
        }
    }
}