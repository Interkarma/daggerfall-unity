//Enhanced Sky for Daggerfall Tools for Unity by Lypyl, contact at lypyl@dfworkshop.net
//http://www.reddit.com/r/dftfu
//http://www.dfworkshop.net/
//Author: LypyL
///Contact: Lypyl@dfworkshop.net
//License: MIT License (http://www.opensource.org/licenses/mit-license.php)


using UnityEngine;

namespace EnhancedSky
{
    public class StarController : MonoBehaviour
    {
        Renderer _rend;
        Color   _lastColor;
        bool    _setColors = true;
        Renderer Rend     { get { return (_rend != null) ? _rend : _rend = this.GetComponent<Renderer>(); } }
        Gradient StarFade { get { return PresetContainer.Instance.starGradient ;} }
        SkyManager SkyMan { get { return SkyManager.instance; } }
 
        void OnEnable()
        {
            Init(SkyMan.IsOvercast);
            SkyManager.updateSkyEvent += this.Init;
        }


        void OnDisable()
        {
            StopAllCoroutines();
            SkyManager.updateSkyEvent -= this.Init;
        }

      

        public void Init(bool isOverCast)
        {
            Rend.material = SkyMan.StarsMat;
            Rend.enabled = false;
            _setColors = true;

        }

        void FixedUpdate()
        {
            if (SkyMan.IsOvercast)
                return;

            if (SkyMan.CurrentSeconds < SkyMan.DuskTime && SkyMan.CurrentSeconds > SkyMan.DawnTime)  //daytime, not overcast                                                            
            {
                Rend.enabled = false;
                return;
            }
            else
                Rend.enabled = true;

            Color colorCheck = StarFade.Evaluate(SkyMan.TimeRatio);
            if (_lastColor.a != colorCheck.a)
                _setColors = true;

            if (_setColors)
            {
                _lastColor = colorCheck;
                Rend.material.SetColor("_Color", _lastColor);
                _setColors = false;
            }

        }

    }



}
