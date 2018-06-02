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
        private const int flickerCountThreshold = 4;
        private int flickerCount = 0;
        private float alphaFadeValue = 0.0f;
        private const float flickerHealthThreshold = 0.25f;

        public HUDFlicker()
        {
            //backgroundTexture = __ExternalAssets.iTween.CameraTexture(new Color(0, 0, 0, 1));
        }

        private void AlphaChange()
        {
            // increment alpha depending on FadeCycle
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
        private void FlickerStateControl()
        {
            RandomlyReverseState();
            // reverse if alpha reached max or min
            if (alphaFadeValue >= alphaMax && alphaState == AlphaState.Increasing)
            {
                alphaState = AlphaState.Decreasing;
            }
            else if (alphaFadeValue <= alphaMin && alphaState == AlphaState.Decreasing)
            {
                alphaState = AlphaState.Increasing;
                flickerCount++;
            }

            // Start Flicker
            if (IsBelowThreshold() && alphaState == AlphaState.None)
            { 
                alphaState = AlphaState.Increasing;
            }
            // Slow Exit out If player's health goes above threshold
            else if (!IsBelowThreshold() 
                &&
                ( alphaState == AlphaState.Increasing 
                || alphaState == AlphaState.Decreasing)
                )
            {
                alphaState = AlphaState.Resetting;
                flickerCount = 0;
            }
            // Finish Flicker Exit
            else if ( alphaState == AlphaState.Resetting && alphaFadeValue <= 0)
            {
                alphaState = AlphaState.None;
            }
        }

        private void RandomlyReverseState()
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
                //Debug.Log("Reversed AlphaState");
                // chance gets reset each time it's randomly reversed.
                chanceReverseState = minChance;
                if (alphaState == AlphaState.Increasing)
                    alphaState = AlphaState.Decreasing;
                else if (alphaState == AlphaState.Decreasing)
                    alphaState = AlphaState.Increasing;
            }
        }

        private void SetAlphaSpeed()
        {
            if (alphaState == AlphaState.Resetting)
                alphaSpeed = fadeFast * 0.3f;
            else if (flickerCount > flickerCountThreshold)
                alphaSpeed = fadeSlow;
            else if (flickerCount <= flickerCountThreshold)
                alphaSpeed = fadeFast;
        }

        public override void Draw()
        {
            FlickerStateControl();
            SetAlphaSpeed();
            AlphaChange();

            Parent.BackgroundColor = new Color(0, 0, 0, alphaFadeValue);
            base.Draw();
        }
        private bool IsBelowThreshold()
        {
            float max = GameManager.Instance.PlayerEntity.MaxHealth;
            float current = GameManager.Instance.PlayerEntity.CurrentHealth;
            return ((current / max) < flickerHealthThreshold);
        }
    }
}