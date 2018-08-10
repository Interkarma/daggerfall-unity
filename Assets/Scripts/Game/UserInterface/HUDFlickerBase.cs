using UnityEngine;
using System.Collections;
using DaggerfallWorkshop.Game;

namespace DaggerfallWorkshop.Game.UserInterface
{
    public abstract class HUDFlickerBase
    {
        public enum AlphaDirection
        {
            None,
            Decreasing,
            Increasing
        }
        protected AlphaDirection alphaDirection;
        protected float chanceReverseState = 0;
        protected float alphaSpeed;
        protected float alphaUpper;
        protected float alphaLower;
        public float RedValue { get; protected set; }
        public float AlphaValue { get; protected set; }
        protected int reversalCount = 0;
        protected int reversalCountThreshold;
        public bool IsTimedOut { get; set; }

        public HUDFlickerBase()
        {
            Init();
        }
        public abstract void Init();
        public virtual void CheckReverseAlphaDirection()
        {
            // Alpha Direction Reversal Check
            if ((alphaDirection == AlphaDirection.Increasing && AlphaValue >= alphaUpper) ||
                (alphaDirection == AlphaDirection.Decreasing && AlphaValue <= alphaLower))
                ReverseAlphaDirection();
            else if (AlphaValue >= alphaLower && AlphaValue <= alphaUpper)
                RandomlyReverseAlphaDirection();
        }
        protected void RandomlyReverseAlphaDirection()
        {
            const float minChance = -0.01f; // negative so it takes a little while before it can reverse at least.
            const float chanceStep = 0.008f;

            if (alphaDirection == AlphaDirection.None)
                return;

            // chance of reversal grows over time.
            chanceReverseState += chanceStep * Time.deltaTime;

            // randomly reverse alpha
            if (Random.Range(0f, 1f) < chanceReverseState)
            {
                // chance gets reset each time it's randomly reversed.
                chanceReverseState = minChance;
                ReverseAlphaDirection();
            }
        }
        private void ReverseAlphaDirection()
        {
            if (alphaDirection == AlphaDirection.Increasing)
                alphaDirection = AlphaDirection.Decreasing;
            else if (alphaDirection == AlphaDirection.Decreasing)
                alphaDirection = AlphaDirection.Increasing;

            if (reversalCount != -1)
                reversalCount++;
        }
        protected void SetAlphaValue(float overrideValue = -1)
        {
            if (overrideValue != -1)
            {
                AlphaValue = overrideValue;
                return;
            }

            // set alpha depending on State
            if ((reversalCount >= reversalCountThreshold && reversalCountThreshold != -1)
                    || alphaDirection == AlphaDirection.None)
                AlphaValue = 0;
            else if (alphaDirection == AlphaDirection.Decreasing)
                AlphaValue -= alphaSpeed * Time.deltaTime;
            else if (alphaDirection == AlphaDirection.Increasing)
                AlphaValue += alphaSpeed * Time.deltaTime;

            AlphaValue = Mathf.Clamp(AlphaValue, 0, alphaUpper); 
        }
        public virtual void Cycle()
        {
            if (!IsTimedOut)
            {
                if (reversalCount >= reversalCountThreshold && reversalCountThreshold != -1)
                {
                    IsTimedOut = true;
                }

                CheckReverseAlphaDirection();
                SetAlphaValue();
            }
            else
                SetAlphaValue(0);
        }
    }
}