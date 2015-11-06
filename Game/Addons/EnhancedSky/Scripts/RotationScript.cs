//Enhanced Sky for Daggerfall Tools for Unity by Lypyl, contact at lypyl@dfworkshop.net
//http://www.reddit.com/r/dftfu
//http://www.dfworkshop.net/
//Author: LypyL
///Contact: Lypyl@dfworkshop.net
//License: MIT License (http://www.opensource.org/licenses/mit-license.php)

using UnityEngine;
using DaggerfallWorkshop;

namespace EnhancedSky
{
    public class RotationScript : MonoBehaviour
    {
        float _degreeRoation;
        float _dot = 1.0f;
        Quaternion _targetRot;
        SkyManager SkyMan { get { return SkyManager.instance; } }

        void FixedUpdate()
        {
            _degreeRoation = ((float)(SkyMan.CurrentSeconds - SkyManager.OFFSET) / SkyManager.DAYINSECONDS) * 360;
            _targetRot = (Quaternion.Euler(_degreeRoation, 270f, 0f));
            _dot = Mathf.Abs(Quaternion.Dot(_targetRot, transform.rotation));
            
            if(_dot < .999 && DaggerfallUnity.Instance.WorldTime.TimeScale <= 1000)
                this.transform.rotation = _targetRot;
            else
                this.transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(_degreeRoation, 270, 0), Time.deltaTime);
            
        }

    }
}