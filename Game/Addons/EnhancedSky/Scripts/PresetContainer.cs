//Enhanced Sky for Daggerfall Tools for Unity by Lypyl, contact at lypyl@dfworkshop.net
//http://www.reddit.com/r/dftfu
//http://www.dfworkshop.net/
//Author: LypyL
///Contact: Lypyl@dfworkshop.net
//License: MIT License (http://www.opensource.org/licenses/mit-license.php)

using UnityEngine;

namespace EnhancedSky
{
    public class PresetContainer : MonoBehaviour
    {
        public const int MOONSCALENORMAL = 4;
        public const int MOONSCALELARGE = 8;
        //public const float SUNSIZENORMAL = 0.11F;     //sizes for simple sun disk
        //public const float SUNSIZELARGE = 0.145F;
        public const float SUNSIZENORMAL = 0.05F;
        public const float SUNSIZELARGE = 0.06F;

        public const float SUNFLARESIZENORMAL = 0.57F;
        public const float SUNFLARESIZELARGE = 0.80F; 

        public const int MAXCLOUDDIMENSION = 1500;
        public const int MINCLOUDDIMENSION = 1;

        public static PresetContainer _instance;

        public Gradient colorBase;
        public Gradient colorOver;

        public Gradient fogBase;
        public Gradient fogOver;

        public Gradient starGradient;

        public Gradient cloudNoiseBase;
        public Gradient cloudNoiseOver;

        public AnimationCurve atmosphereBase;
        public AnimationCurve atmosphereOver;

        public AnimationCurve moonAlphaBase;
        public AnimationCurve moonAlphaOver;

        public Color skyTint;
        public Color MasserColor = new Color(.5216f, .5216f, .5216f);
        public Color SecundaColor = new Color(.7647f, .7647f, .7647f);

        public float atmsphrOffset = .5f;

        public static PresetContainer Instance { get { return (_instance != null) ? _instance : _instance = FindPreset(); } private set { _instance = value;} }

        void Awake()
        {
            if (_instance != null)
                this.enabled = false;
            _instance = this;

        }

        void Destroy()
        {
            Instance = null;
        }

        private static PresetContainer FindPreset()
        {
            PresetContainer pc = GameObject.FindObjectOfType<PresetContainer>();
            if (pc == null)
            {
                DaggerfallWorkshop.DaggerfallUnity.LogMessage("Could not locate PresetContainer in scene");
                return null;

            }
            else
            {
                return pc;
            }
                

        }

    }
}