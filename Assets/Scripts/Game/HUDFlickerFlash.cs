using UnityEngine;
using System.Collections;
using DaggerfallWorkshop.Game;
using System;

namespace DaggerfallWorkshop.Game.UserInterface
{

    public class HUDFlickerFlash : HUDFlicker
    {
        public override void Init()
        {
            Reset();
            alphaSpeed = 2.0f;
            alphaLower = 0.0f;
            alphaUpper = 0.75f;
            RedValue = 0.3984f;
            reversalCountThreshold = -1;
        }

        public override void CheckReverseAlphaDirection(bool randomReversal = false)
        {
            // No reversal of direction for this type of flicker
        }

        public override void Reset()
        {
            base.Reset();
            // starts with alpha at Upper
            InitAlphaDirection(AlphaDirection.Decreasing);
        }

        public override void Cycle()
        {
            base.Cycle();
            // burns out when alpha reaches lower value
            if (AlphaValue == alphaLower)
                IsBurnedOut = true;
        }
    }
}