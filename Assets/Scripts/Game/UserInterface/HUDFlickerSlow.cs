using UnityEngine;
using System.Collections;
using DaggerfallWorkshop.Game;

namespace DaggerfallWorkshop.Game.UserInterface
{

    public class HUDFlickerSlow : HUDFlickerBase
    {
        public override void Init()
        {
            IsTimedOut = false;
            alphaSpeed = 0.2f;
            alphaLower = 0.1f;
            alphaUpper = 0.4f;
            AlphaValue = alphaLower;
            RedValue = 0.0f;
            reversalCount = 0;
            reversalCountThreshold = -1;
            alphaDirection = AlphaDirection.Increasing;
        }
    }
}