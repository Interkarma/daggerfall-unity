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
            alphaSpeed = 0.2f;
            alphaLower = 0.15f;
            alphaUpper = 0.5f;
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