using UnityEngine;
using System.Collections;
using DaggerfallWorkshop.Game;

namespace DaggerfallWorkshop.Game.UserInterface
{

    public class HUDFlickerSlow : HUDFlicker
    {
        public override void Init()
        {
            Reset();
            alphaSpeed = 0.1f;
            alphaLower = 0.1f;
            alphaUpper = 0.4f;
            reversalCountThreshold = -1;
        }

        public override void SetAlphaDirection(bool randomReversal = true)
        {
            if (alphaDirection == AlphaDirection.None)
            { 
                alphaDirection = AlphaDirection.Increasing;
                AlphaValue = alphaLower;
            }
            base.SetAlphaDirection(randomReversal);
        }

    }
}