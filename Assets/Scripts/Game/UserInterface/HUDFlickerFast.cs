using UnityEngine;
using System.Collections;
using DaggerfallWorkshop.Game;

namespace DaggerfallWorkshop.Game.UserInterface
{

    public class HUDFlickerFast : HUDFlickerBase
    {
        public override void Init()
        {
            IsTimedOut = false;
            alphaSpeed = 7.0f;
            alphaLower = 0.0f;
            alphaUpper = 0.4f;
            RedValue = 0.3984f;
            AlphaValue = alphaLower;
            reversalCount = 0;
            reversalCountThreshold = 7;
            alphaDirection = AlphaDirection.Increasing;
        }
    }
}