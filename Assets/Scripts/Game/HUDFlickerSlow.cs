using UnityEngine;
using System.Collections;
using DaggerfallWorkshop.Game;

namespace DaggerfallWorkshop.Game.UserInterface
{

    public class HUDFlickerSlow : HUDFlickerBase
    {
        public override void Init()
        {
            Reset();
            alphaSpeed = 0.2f;

            alphaLower = 0.1f;
            alphaUpper = 0.4f;
            RedValue = 0.0f;
            reversalCountThreshold = -1;
            InitAlphaDirection(AlphaDirection.Increasing);
        }
    }
}