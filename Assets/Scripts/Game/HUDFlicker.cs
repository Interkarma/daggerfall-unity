using UnityEngine;
using System.Collections;
using DaggerfallWorkshop.Game;

namespace DaggerfallWorkshop.Game.UserInterface
{
    public class HUDFlicker : BaseScreenComponent
    {
        private enum AlphaState
        {
            None,
            Resetting,
            Decreasing,
            Increasing
        }
        private AlphaState alphaState;
        private HealthLossDetector healthDetector;
        private float chanceReverseState;
        private const float fadeFast = 3.7f;
        private const float fadeSlow = 0.1f;
        private float alphaSpeed = fadeFast;
        private const float alphaUpperMax = 0.4f;
        private const float alphaLowerMax = 0.1f;
        private float alphaUpper;
        private float alphaLower;
        private int reversalCountThreshold;
        private const int reversalCountThresholdMax = 10;

        private int reversalCount = 0;
        private float alphaFadeValue = 0.0f;
        private const float flickerHealthThreshold = 0.35f; // Health percentage that flicker is triggered at.

        public HUDFlicker()
        {
            healthDetector = GameManager.Instance.HealthLossDetector;
        }

        private void AlphaValueChange()
        {
            // increment alpha depending on State
            if (alphaState == AlphaState.Decreasing || alphaState == AlphaState.Resetting)
            {
                alphaFadeValue -= alphaSpeed * Time.deltaTime;
            }
            else if (alphaState == AlphaState.Increasing)
            {
                alphaFadeValue += alphaSpeed * Time.deltaTime;
            }
            else if (alphaState == AlphaState.None)
            {
                alphaFadeValue = 0;
            }
        }
        private void AlphaStateControl()
        {
            bool isBelowThreshold = IsBelowThreshold();
            // reverse if alpha reached max or min
            if (isBelowThreshold)
            {
                // Start Flickering
                if (alphaState == AlphaState.None)
                { 
                    alphaState = AlphaState.Increasing;
                    alphaFadeValue = alphaLower;
                }

                // TODO: doesn't seem to flash fast every time health is lost
                // each loss of health resets reversal count so it flickers again.
                if (healthDetector.healthLost > 0)
                    reversalCount = 0;

                if ((alphaState == AlphaState.Increasing && alphaFadeValue > alphaUpper) ||
                    (alphaState == AlphaState.Decreasing && alphaFadeValue < alphaLower))
                    ReverseAlphaDirection();
                else if (alphaFadeValue != alphaLower)
                    RandomlyReverseAlphaDirection();
            }
            // Slow Exit out if player's health goes above threshold
            else if (alphaState != AlphaState.Resetting && alphaState != AlphaState.None)
            {
                alphaState = AlphaState.Resetting;
                reversalCount = 0;
            }
            // Finish Exit
            else if ( alphaState == AlphaState.Resetting && alphaFadeValue <= 0)
            {
                alphaState = AlphaState.None;
            }
        }

        private void RandomlyReverseAlphaDirection()
        {
            const float minChance = -0.01f; // negative so it takes a little while before it can reverse at least.
            const float chanceStep = 0.008f;

            if (alphaState != AlphaState.Decreasing && alphaState != AlphaState.Increasing)
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
            if (alphaState == AlphaState.Increasing)
                alphaState = AlphaState.Decreasing;
            else if (alphaState == AlphaState.Decreasing)
                alphaState = AlphaState.Increasing;
            reversalCount++;
            //Debug.Log("Reversed AlphaState");
        }

        /// <summary>
        /// Sets how fast the screen flashes and number of flashes before going to slow throb
        /// </summary>
        private void SetStunSeverity()
        {
            // stun multiplier max is 1
            float stunMultiplier = Mathf.Max(0.5f, 1 - CurrentPercentOfHealthThreshold());
            // max number of times the alpha will reverse direction before slowing down
            reversalCountThreshold = (int)Mathf.Ceil(stunMultiplier * reversalCountThresholdMax);
            alphaLower = stunMultiplier * alphaLowerMax;
            alphaUpper = stunMultiplier * alphaUpperMax;

            // Resetting somewhat quickly
            if (alphaState == AlphaState.Resetting)
            { 
                alphaSpeed = fadeFast * 0.3f;
            }
            // throb slow because health loss isn't fresh anymore
            else if (reversalCount > reversalCountThreshold)
            {
                alphaSpeed = fadeSlow;
            }
            // Flash fast because health loss is fresh.
            else if (reversalCount <= reversalCountThreshold)
            {
                alphaSpeed = fadeFast;
            }   
        }
        /// <summary>
        /// Finds the percentage of health between 0 and the threshold of the flicker.
        /// </summary>
        /// <returns></returns>
        private float CurrentPercentOfHealthThreshold()
        {
            return GameManager.Instance.PlayerEntity.CurrentHealthPercent / flickerHealthThreshold;
        }
        private bool IsBelowThreshold()
        {
            return ((GameManager.Instance.PlayerEntity.CurrentHealthPercent) < flickerHealthThreshold);
        }
        public override void Draw()
        {
            AlphaStateControl();
            if (alphaState != AlphaState.None)
                SetStunSeverity();
            AlphaValueChange();

            float redValue = 0;
            if (reversalCount <= reversalCountThreshold)
                redValue = alphaFadeValue;

            Parent.BackgroundColor = new Color(0, 0, 0, alphaFadeValue);
            base.Draw();
        }
    }
}