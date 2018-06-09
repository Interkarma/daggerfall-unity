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
            Wounded,
            Dead
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
            condition = GetPlayerCondition();

            CheckResetFlickers();
            CalculateBackgroundColor();
        }
        private void CheckResetFlickers()
        {
            if (healthDetector.HealthLost > 0)
            {
                // should the fast flicker be reset?
                if (  condition == PlayerCondition.Injured 
                    || condition == PlayerCondition.Wounded
                    || condition == PlayerCondition.Dead)
                {
                    flickerFast.ResetIfBurnedOut();
                }

                // should the slow flicker be reset?
                /*if ((  condition == PlayerCondition.Dead)
                    && healthDetector.HealthLost != 0
                    && flickerSlow.IsBurnedOut)
                {
                    flickerSlow.Reset();
                }*/

                // should the flash flicker be reset?
                if ( condition == PlayerCondition.Normal )
                {
                    flickerFlash.ResetIfBurnedOut();
                }
            }
        }
        private void CalculateBackgroundColor()
        {
            Color backColor;
            // Decide what alpha and red to use in background color
            switch (condition)
            {
                case PlayerCondition.Normal:
                    flickerFlash.Cycle();
                    backColor = new Color(flickerFlash.RedValue, 0, 0, flickerFlash.AlphaValue);
                    break;
                case PlayerCondition.Injured:
                    flickerFast.Cycle();
                    backColor = new Color(flickerFast.RedValue, 0, 0, flickerFast.AlphaValue);
                    break;
                case PlayerCondition.Wounded:
                    flickerFast.Cycle();
                    // Flicker slow runs if flickerFast is burned out, and cannot if it isn't burned out
                    flickerSlow.IsBurnedOut = !flickerFast.IsBurnedOut;
                    //BurnOutSwap(flickerSlow, flickerFast);
                    flickerSlow.Cycle();
                    if (flickerFast.IsBurnedOut)
                        backColor = new Color(flickerSlow.RedValue, 0, 0, flickerSlow.AlphaValue);
                    else
                        backColor = new Color(flickerFast.RedValue, 0, 0, flickerFast.AlphaValue);
                    break;
                //case PlayerCondition.Dead:
                // Doesn't seem to display the color when dying
                //flickerFast.Cycle();
                //backColor.a = flickerFast.AlphaValue;
                //backColor.r = flickerFast.RedValue;
                //break;
                default:
                    backColor = new Color();
                    break;
            }

            if (condition != PlayerCondition.Dead)
                Parent.BackgroundColor = new Color(backColor.r, 0, 0, backColor.a);
        }
    }
}