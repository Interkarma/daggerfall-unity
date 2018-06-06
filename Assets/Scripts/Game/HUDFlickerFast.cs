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
            alphaSpeed = 3.9f;
            alphaLower = 0.15f;
            alphaUpper = 0.6f;
            reversalCountThreshold = 7;
            InitAlphaDirection(AlphaDirection.Increasing);
        }

        public override void CheckReverseAlphaDirection(bool randomReversal = true)
        {
            if (reversalCount <= reversalCountThreshold)
            {
                // do inherited reversal normally
                base.CheckReverseAlphaDirection(randomReversal);
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