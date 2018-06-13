using UnityEngine;
using System.Collections;
using DaggerfallWorkshop.Game;

namespace DaggerfallWorkshop.Game.UserInterface
{

    public class HUDFlickerFast : HUDFlickerBase
    {
        public override void Init()
        {
            Reset();
            alphaSpeed = 7.0f;
            alphaLower = 0.0f;
            alphaUpper = 0.4f;
            RedValue = 0.3984f;
            reversalCountThreshold = 7;
            InitAlphaDirection(AlphaDirection.Increasing);
        }
    }
}