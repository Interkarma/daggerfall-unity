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
        private float chanceReverseState;
        private const float fadeFast = 3.7f;
        private const float fadeSlow = 0.1f;
        private float alphaSpeed = fadeFast;
        private const float alphaMax = 0.4f;
        private const float alphaMin = 0.1f;
        private const int reversalCountThreshold = 7;
        private int reversalCount = 0;
        private float alphaFadeValue = 0.0f;
        private const float flickerHealthThreshold = 0.25f;

        private void AlphaChange()
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
                    alphaFadeValue = alphaMin;
                }

                if ((alphaState == AlphaState.Increasing && alphaFadeValue > alphaMax) ||
                    (alphaState == AlphaState.Decreasing && alphaFadeValue < alphaMin))
                    ReverseAlphaDirection();
                else if (alphaFadeValue != alphaMin)
                    RandomlyReverseAlphaDirection();
            }
            // Slow Exit out If player's health goes above threshold
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

        private void SetAlphaSpeed()
        {
            if (alphaState == AlphaState.Resetting)
                alphaSpeed = fadeFast * 0.3f;
            else if (reversalCount > reversalCountThreshold)
                alphaSpeed = fadeSlow;
            else if (reversalCount <= reversalCountThreshold)
                alphaSpeed = fadeFast;
        }

        public override void Draw()
        {
            AlphaStateControl();
            SetAlphaSpeed();
            AlphaChange();

            Parent.BackgroundColor = new Color(0, 0, 0, alphaFadeValue);
            base.Draw();
        }
        private bool IsBelowThreshold()
        {
            float max = GameManager.Instance.PlayerEntity.MaxHealth;
            float current = GameManager.Instance.PlayerEntity.CurrentHealth;
            return ((GameManager.Instance.PlayerEntity.CurrentHealthPercent) < flickerHealthThreshold);
        }
    }
}