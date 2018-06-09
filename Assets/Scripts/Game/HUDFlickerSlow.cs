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
            alphaLower = 0.25f;
            alphaUpper = 0.5f;
            RedValue = 0.0f;
            reversalCountThreshold = -1;
            InitAlphaDirection(AlphaDirection.Increasing);
        }

        public override void CheckReverseAlphaDirection(bool randomReversal = true)
        {
            if (alphaDirection == AlphaDirection.None)
            { 
                alphaDirection = AlphaDirection.Increasing;
                AlphaValue = alphaLower;
            }
            base.CheckReverseAlphaDirection(randomReversal);
        }

    }
}