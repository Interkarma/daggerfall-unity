﻿using UnityEngine;
using System.Collections;
using DaggerfallWorkshop.Game;

namespace DaggerfallWorkshop.Game.UserInterface
{
    public class HUDFlickerMask : BaseScreenComponent
    {
        private enum PlayerCondition
        {
            Normal,
            Injured,
            Wounded
        }
        private PlayerCondition condition;
        private HealthChangeDetector healthDetector;
        private HUDFlickerFast flickerFast;
        private HUDFlickerSlow flickerSlow;
        private HUDFlickerFlash flickerFlash;
        private float newAlpha;

        private const float injuredThreshold = 0.4f; // Health percentage that flicker is triggered at.
        private const float woundedThreshold = 0.2f; // Health percentage that throb will continue at.
        public HUDFlickerMask()
        {
            healthDetector = GameManager.Instance.HealthChangeDetector;
            flickerFast = new HUDFlickerFast();
            flickerSlow = new HUDFlickerSlow();
            flickerFlash = new HUDFlickerFlash();
        }
        private PlayerCondition GetPlayerCondition()
        {
            if (IsBelowThreshold(woundedThreshold))
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
        public override void Draw()
        {
            PlayerCondition condition = GetPlayerCondition();

            // should the fast flicker be reset?
            if ((  condition == PlayerCondition.Injured 
                || condition == PlayerCondition.Wounded)
                && healthDetector.HealthLost > 0
                && flickerFast.IsBurnedOut)
            {
                flickerFast.Reset();
            }

            // should the slow flicker be reset?
            if ((  condition == PlayerCondition.Injured
                || condition == PlayerCondition.Normal)
                && healthDetector.HealthLost != 0
                && flickerSlow.IsBurnedOut)
            {
                flickerSlow.Reset();
            }

            // should the flash flicker be reset?
            if ( condition == PlayerCondition.Normal
                && healthDetector.HealthLost > 0
                && flickerFlash.IsBurnedOut)
            {
                flickerFlash.Reset();
            }

            if (condition == PlayerCondition.Normal)
            {
                flickerFlash.Cycle();
                newAlpha = ReplaceAlphaWithIfBurntOut(flickerFlash, 0);
            }
            else if (condition == PlayerCondition.Injured)
            {
                flickerFast.Cycle();
                newAlpha = ReplaceAlphaWithIfBurntOut(flickerFast, 0);
            }
            else if (condition == PlayerCondition.Wounded)
            {
                flickerFast.Cycle();
                // Flicker slow runs if flickerFast is burned out, and cannot if it isn't burned out
                flickerSlow.IsBurnedOut = !flickerFast.IsBurnedOut;
                flickerSlow.Cycle();
                // if Flicker fast is burnt out, use flicker slow value
                newAlpha = ReplaceAlphaWithIfBurntOut(flickerFast, flickerSlow.AlphaValue);
            }
            
            Parent.BackgroundColor = new Color(0.3984f, 0, 0, newAlpha);
            base.Draw();
        }

        private float ReplaceAlphaWithIfBurntOut(HUDFlicker flicker, float replacementValue)
        {
            if (flicker.IsBurnedOut)
                return replacementValue;
            else
                return flicker.AlphaValue;
        }
    }
}