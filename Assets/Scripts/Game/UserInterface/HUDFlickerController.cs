using UnityEngine;
using System.Collections;
using DaggerfallWorkshop.Game;

namespace DaggerfallWorkshop.Game.UserInterface
{
    public class HUDFlickerController : BaseScreenComponent
    {
        private enum PlayerCondition
        {
            Normal,
            Injured,
            Wounded,
            Dead
        }
        private VitalsChangeDetector vitalsDetector;
        private HUDFlickerFast flickerFast;
        private HUDFlickerSlow flickerSlow;

        private const float injuredThreshold = 0.4f; // Health percentage that flicker is triggered at.
        private const float woundedThreshold = 0.2f; // Health percentage that throb will continue at.
        public HUDFlickerController()
        {
            vitalsDetector = GameManager.Instance.VitalsChangeDetector;
            flickerFast = new HUDFlickerFast();
            flickerSlow = new HUDFlickerSlow();
        }
        private PlayerCondition GetPlayerCondition()
        {
            if (GameManager.Instance.PlayerEntity.CurrentHealth <= 0)
                return PlayerCondition.Dead;
            else if (IsBelowThreshold(woundedThreshold))
                return PlayerCondition.Wounded;
            else if (IsBelowThreshold(injuredThreshold))
                return PlayerCondition.Injured;
            else
                return PlayerCondition.Normal;
        }
        private bool IsBelowThreshold(float threshold)
        {
            return ((GameManager.Instance.PlayerEntity.CurrentHealthPercent) < threshold);
        }
        public void NextCycle()
        {
            if (  !DaggerfallUnity.Settings.NearDeathWarning 
                || DaggerfallUI.Instance.FadeBehaviour.FadeInProgress
                || Parent.BackgroundColor.a > 0.9f) // prevents conflict with fade in from black
                return;

            PlayerCondition condition = GetPlayerCondition();

            if (vitalsDetector.HealthLost > 0 && flickerFast.IsTimedOut)
                flickerFast.Init();

            if (vitalsDetector.HealthGain > 0 && condition != PlayerCondition.Wounded)
                flickerSlow.IsTimedOut = true;

            Color backColor;

            switch (condition)
            {
                case PlayerCondition.Injured:
                    flickerFast.Cycle();
                    backColor = new Color(flickerFast.RedValue, 0, 0, flickerFast.AlphaValue);
                    break;
                case PlayerCondition.Wounded:
                    flickerFast.Cycle();
                    // Flicker slow runs if flickerFast is timed out, and cannot if it isn't timed out
                    flickerSlow.IsTimedOut = !flickerFast.IsTimedOut;
                    flickerSlow.Cycle();
                    if (flickerFast.IsTimedOut)
                        backColor = new Color(flickerSlow.RedValue, 0, 0, flickerSlow.AlphaValue);
                    else
                        backColor = new Color(flickerFast.RedValue, 0, 0, flickerFast.AlphaValue);
                    break;
                default:
                    backColor = new Color();
                    break;
            }

            if (condition != PlayerCondition.Dead && backColor != Parent.BackgroundColor)
                Parent.BackgroundColor = backColor;
        }
    }
}