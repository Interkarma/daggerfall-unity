//Enhanced Sky for Daggerfall Tools for Unity by Lypyl, contact at lypyl@dfworkshop.net
//http://www.reddit.com/r/dftfu
//http://www.dfworkshop.net/
//Author: LypyL
///Contact: Lypyl@dfworkshop.net
//License: MIT License (http://www.opensource.org/licenses/mit-license.php)

using UnityEngine;
using System.Collections;

namespace EnhancedSky
{
    public class PresetContainer : MonoBehaviour
    {

        public static PresetContainer instance;

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

        public float atmsphrOffset = .5f;               //causing red sky at night

        void Awake()
        {
            if (instance != null)
                this.enabled = false;
            instance = this;

        }

        void Destroy()
        {
            instance = null;
        }

    }
}