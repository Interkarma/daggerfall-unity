using UnityEngine;
using System.Collections;
using DaggerfallWorkshop.Game;

namespace DaggerfallWorkshop.Game.UserInterface
{

    public class HUDFlickerFast : HUDFlicker
    {
        public override void Init()
        {
            Reset();
            alphaSpeed = 3.7f;
            alphaLower = 0.0f;
            alphaUpper = 0.4f;
            reversalCountThreshold = 7;
        }

        public override void SetAlphaDirection(bool randomReversal = true)
        {
            if (reversalCount <= reversalCountThreshold)
            {
                // do inherited reversal normally
                base.SetAlphaDirection(randomReversal);
            }
            else if (IsBurnedOut == false)
            {
                IsBurnedOut = true;
                alphaDirection = AlphaDirection.Decreasing;
            }
            else if (IsBurnedOut && AlphaValue <= 0 && alphaDirection != AlphaDirection.None)
            {
                AlphaValue = 0;
                alphaDirection = AlphaDirection.None;
            }    
        }
    }
}